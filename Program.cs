using System;
using Coactivos_Predial.Auth;
using Coactivos_Predial;
using Coactivos_Predial.Shared.AppDbContext;
using Coactivos_Predial.Shared.Components;
using Coactivos_Predial.Shared.Services;
using Coactivos_Predial.Shared.Services.DapperHandler;
using Coactivos_Predial.Shared.Services.Document;
using Coactivos_Predial.Shared.Services.FileHandler;
using Coactivos_Predial.Shared.Services.FontResolver;
using Coactivos_Predial.Shared.Services.SCoactivos;
using Coactivos_Predial.Shared.Services.SContribuyentes;
using Coactivos_Predial.Shared.Services.SDatabase;
using Coactivos_Predial.Shared.Services.DocumentoFiles;
using Coactivos_Predial.Shared.Services.SImportados;
using Coactivos_Predial.Shared.Services.SLogin;
using Coactivos_Predial.Shared.Services.SParametros;
using Coactivos_Predial.Shared.Services.Spersuasivo;
using Coactivos_Predial.Shared.Services.SPredios;
using Coactivos_Predial.Shared.Services.SPropietarios;
using Coactivos_Predial.Shared.Services.SUser;
using Coactivos_Predial.Shared.Services.UserAccesor;
using Coactivos_Predial.Shared.Services.SPropietarios;
using Dapper;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using PdfSharp.Fonts;
using Radzen;

var builder = WebApplication.CreateBuilder(args);

if (args.Contains("--migrate-legacy-data", StringComparer.Ordinal))
{
    await Coactivos_Predial.Database.LegacyDataMigration.RunAsync(builder.Configuration);
    return;
}

if (args.Contains("--seed-demo", StringComparer.Ordinal))
{
    await Coactivos_Predial.Database.DemoSeeder.RunAsync(builder.Configuration);
    return;
}

builder.Services.AddRazorPages();
builder.Services.AddHttpClient();
builder.Services.AddRadzenComponents();
builder.Services.AddHttpContextAccessor();

// DbContext Maestra (cadena fija)
builder.Services.AddDbContextFactory<MasterDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("MasterConnection")));
builder.Services.AddScoped(sp =>
    sp.GetRequiredService<IDbContextFactory<MasterDbContext>>().CreateDbContext());

// Tenant Provider (Scoped por circuito de Blazor)
builder.Services.AddScoped<ITenantProvider, TenantProvider>();

// Tenant DbContext (cadena dinámica por circuito)
builder.Services.AddDbContext<TenantDbContext>((sp, options) =>
{
    var tenant = sp.GetRequiredService<ITenantProvider>();
    if (tenant.Municipio is null)
        throw new InvalidOperationException("Tenant no inicializado");

    var template = builder.Configuration.GetConnectionString("DefaultConnectionTemplate")!;
    options.UseNpgsql(template.Replace("{Database}", tenant.Municipio));
});

// Fábrica de conexiones de tenant para Dapper
builder.Services.AddScoped<ITenantConnectionFactory, TenantConnectionFactory>();

// Servicios de conexión y usuario
builder.Services.AddScoped<IConnectionService, ConnectionService>();
builder.Services.AddScoped<IUserContextService, UserContextService>();
builder.Services.AddScoped<IDocumentoFileService, DocumentoFileService>();
builder.Services.AddScoped<IDocumentoLinkService, DocumentoLinkService>();

// Servicios de negocio
builder.Services.AddScoped<ICoactivosService, CoactivosService>();
builder.Services.AddScoped<IArchivoService, ArchivoService>();
builder.Services.AddScoped<IPersuasivoService, PersuasivoService>();
builder.Services.AddScoped<IPrediosService, PrediosService>();
builder.Services.AddScoped<IContribuyentesService, ContribuyentesService>();
builder.Services.AddScoped<IParametroService, ParametroService>();
builder.Services.AddScoped<IimportadosService, ImportadosService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IpropietariosService, PropietariosService>();
builder.Services.AddScoped<ILoginService, LoginService>();
builder.Services.AddScoped<TokenProvider>();
builder.Services.AddSingleton<BusquedaService>();

GlobalFontSettings.FontResolver = new CustomFontResolver();
SqlMapper.AddTypeHandler(new DateOnlyTypeHandler());

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddCascadingAuthenticationState();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.ExpireTimeSpan = TimeSpan.FromMinutes(10);
        options.AccessDeniedPath = "/Forbidden/";
        options.LoginPath = "/";
        options.Events.OnRedirectToLogin = context =>
        {
            if (context.Request.Path.StartsWithSegments("/api"))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Task.CompletedTask;
            }
            context.Response.Redirect(context.RedirectUri);
            return Task.CompletedTask;
        };
        options.Events.OnRedirectToAccessDenied = context =>
        {
            if (context.Request.Path.StartsWithSegments("/api"))
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                return Task.CompletedTask;
            }
            context.Response.Redirect(context.RedirectUri);
            return Task.CompletedTask;
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddDataProtection()
    .SetApplicationName("myapp");

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles(DocumentoEndpoints.PdfJsStaticFileOptions());
app.UseAuthentication();
app.UseAntiforgery();
app.UseAuthorization();
app.Use(async (context, next) =>
{
    if (context.Request.Path.StartsWithSegments("/api") && context.User.Identity?.IsAuthenticated == true)
    {
        var tenant = context.RequestServices.GetRequiredService<ITenantProvider>();
        await tenant.InitializeAsync();
        if (tenant.Municipio is null)
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            return;
        }
    }
    await next();
});
app.MapRazorPages();
app.MapControllers();
app.MapDocumentoEndpoints();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

var cookiePolicyOptions = new CookiePolicyOptions
{
    MinimumSameSitePolicy = SameSiteMode.Strict,
};

app.UseCookiePolicy(cookiePolicyOptions);

app.Run();

public partial class Program { }
