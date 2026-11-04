using System.Runtime.InteropServices;
using PdfSharp.Fonts;

namespace Coactivos_Predial.Shared.Services.FontResolver;

public class CustomFontResolver : IFontResolver
{
    private readonly static string[] FontPaths =
    [
        // Windows
        @"C:\Windows\Fonts",
            // Linux
            "/usr/share/fonts/truetype/liberation",
            "/usr/share/fonts/truetype/dejavu",
            "/usr/share/fonts/truetype/ubuntu",
            // Mac
            "/Library/Fonts",
            "/System/Library/Fonts"
    ];

    private static readonly Dictionary<string, string> FontMappings = new()
        {
            { "Arial", "arial.ttf" },
            { "Arial-Bold", "arialbd.ttf" },
            { "Arial-Italic", "ariali.ttf" },
            { "Arial-BoldItalic", "arialbi.ttf" },
            { "Verdana", "verdana.ttf" },
            { "Verdana-Bold", "verdanab.ttf" },
            { "Verdana-Italic", "verdanai.ttf" },
            { "Verdana-BoldItalic", "verdanaz.ttf" },
            { "Times New Roman", "times.ttf" },
            { "Times New Roman-Bold", "timesbd.ttf" },
            { "Times New Roman-Italic", "timesi.ttf" },
            { "Times New Roman-BoldItalic", "timesbi.ttf" },
            { "Courier New", "cour.ttf" },
            { "Courier New-Bold", "courbd.ttf" },
            { "Courier New-Italic", "couri.ttf" },
            { "Courier New-BoldItalic", "courbi.ttf" }
        };

    public FontResolverInfo? ResolveTypeface(string familyName, bool bold, bool italic)
    {
        string key = familyName;
        if (bold && italic)
            key += "-BoldItalic";
        else if (bold)
            key += "-Bold";
        else if (italic)
            key += "-Italic";

        if (FontMappings.TryGetValue(key, out string? fileName))
        {
            return new FontResolverInfo(fileName);
        }

        // Fallback a Arial regular si no encuentra
        if (FontMappings.TryGetValue("Arial", out string? fallback))
            return new FontResolverInfo(fallback);

        return null;
    }

    public byte[]? GetFont(string faceName)
    {
        // Buscar en todas las rutas configuradas
        foreach (string path in FontPaths)
        {
            string fullPath = Path.Combine(path, faceName);
            if (File.Exists(fullPath))
            {
                return File.ReadAllBytes(fullPath);
            }
        }

        // Buscar recursivamente en /usr/share/fonts (Linux)
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            var fontFiles = Directory.GetFiles("/usr/share/fonts", faceName, SearchOption.AllDirectories);
            if (fontFiles.Length > 0)
            {
                return File.ReadAllBytes(fontFiles[0]);
            }
        }

        return null;
    }
}