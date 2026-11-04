using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Net.Http.Headers;

namespace Coactivos_Predial.Shared.Services.DocumentoFiles;

public static class DocumentoEndpoints
{
    public static IEndpointRouteBuilder MapDocumentoEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/api/documentos/{id:int}/ver", async (int id, IDocumentoFileService files, HttpContext context, CancellationToken cancellationToken) =>
            await ServeAsync(id, files, context, cancellationToken, download: false)).RequireAuthorization();
        endpoints.MapGet("/api/documentos/{id:int}/descargar", async (int id, IDocumentoFileService files, HttpContext context, CancellationToken cancellationToken) =>
            await ServeAsync(id, files, context, cancellationToken, download: true)).RequireAuthorization();
        return endpoints;
    }

    private static async Task<IResult> ServeAsync(int id, IDocumentoFileService files, HttpContext context, CancellationToken cancellationToken, bool download)
    {
        DocumentoFile? document;
        try
        {
            document = await files.GetForCurrentUserAsync(id, cancellationToken);
        }
        catch (DocumentoAccessDeniedException)
        {
            return Results.Forbid();
        }
        if (document is null || !File.Exists(document.Path)) return Results.NotFound();

        var info = new FileInfo(document.Path);
        var modified = info.LastWriteTimeUtc;
        var entityTag = new EntityTagHeaderValue($"\"{info.Length:x}-{modified.Ticks:x}\"");
        context.Response.Headers["X-Content-Type-Options"] = "nosniff";
        context.Response.Headers["X-Frame-Options"] = "SAMEORIGIN";
        context.Response.Headers["Content-Security-Policy"] = "frame-ancestors 'self'";
        context.Response.Headers["Cache-Control"] = "private, max-age=0, must-revalidate";
        context.Response.Headers["Accept-Ranges"] = "bytes";
        context.Response.Headers["Content-Disposition"] = new ContentDispositionHeaderValue(download ? "attachment" : "inline")
        {
            FileNameStar = Path.GetFileName(document.Path)
        }.ToString();

        var stream = new FileStream(document.Path, FileMode.Open, FileAccess.Read, FileShare.Read, 64 * 1024, FileOptions.Asynchronous | FileOptions.SequentialScan);
        return Results.Stream(stream, "application/pdf", lastModified: modified, entityTag: entityTag, enableRangeProcessing: true);
    }

    public static StaticFileOptions PdfJsStaticFileOptions()
    {
        var provider = new FileExtensionContentTypeProvider();
        provider.Mappings[".mjs"] = "text/javascript";
        provider.Mappings[".wasm"] = "application/wasm";
        provider.Mappings[".properties"] = "text/plain";
        provider.Mappings[".ftl"] = "text/plain";
        return new StaticFileOptions { ContentTypeProvider = provider };
    }
}
