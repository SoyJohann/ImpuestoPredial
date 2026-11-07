using System.Globalization;
using System.Text;
using Coactivos_Predial.Shared.AppDbContext;
using Coactivos_Predial.Shared.Models;
using Coactivos_Predial.Shared.Services.Document.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Coactivos_Predial.Database;

/// <summary>
/// Carga datos ficticios de industria y comercio para demostración.
/// Es idempotente: se puede ejecutar varias veces sin duplicar registros.
/// </summary>
internal static class DemoSeeder
{
    private const string TargetDatabase = "Cimitarra";
    private const string NitTypeName = "NIT";

    private sealed record DemoNegocio(
        string Nombre,
        string Matricula,
        string Ciiu,
        string Direccion,
        DateTime InicioActividad,
        string[] Documentos);

    private sealed record DemoPropietario(
        string Documento,
        string Nombre,
        string Telefono,
        string Correo,
        DemoNegocio[] Negocios);

    private static readonly DemoPropietario[] Datos =
    [
        new("900123456-1", "Comercializadora La Espiga S.A.S.", "310 555 0101", "contacto@laespiga.demo",
            [
                new("Panadería La Espiga", "00123456", "5611", "Calle 10 # 5-23", new DateTime(2019, 3, 12),
                    ["Registro mercantil", "Cámara de comercio", "RUT"]),
                new("Cafetería El Trigal", "00123457", "5611", "Carrera 8 # 4-12", new DateTime(2021, 8, 2),
                    ["Registro mercantil", "Cámara de comercio"])
            ]),
        new("900654321-2", "Ferretería El Tornillo S.A.S.", "310 555 0202", "contacto@eltornillo.demo",
            [
                new("Ferretería El Tornillo", "00234567", "4752", "Carrera 12 # 9-45", new DateTime(2017, 6, 20),
                    ["Registro mercantil", "RUT"])
            ]),
        new("901111222-3", "Distribuciones del Norte Ltda.", "310 555 0303", "contacto@delnorte.demo",
            [
                new("Supermercado El Ahorro", "00345678", "4711", "Calle 15 # 3-08", new DateTime(2015, 2, 9),
                    ["Registro mercantil", "Cámara de comercio", "RUT"]),
                new("Droguería Central", "00345679", "4711", "Calle 9 # 7-21", new DateTime(2020, 11, 16),
                    ["Cámara de comercio", "RUT"])
            ]),
        new("901333444-5", "Alimentos del Valle S.A.S.", "310 555 0404", "contacto@alimentosdemo.demo",
            [
                new("Restaurante Sabor Local", "00456789", "5611", "Carrera 6 # 11-30", new DateTime(2018, 9, 5),
                    ["Registro mercantil", "Cámara de comercio", "RUT"])
            ]),
        new("901555666-7", "Inversiones La Pradera S.A.S.", "310 555 0505", "contacto@lapradera.demo",
            [
                new("Tienda La Pradera", "00567890", "4711", "Calle 20 # 2-15", new DateTime(2022, 4, 25),
                    ["Registro mercantil", "RUT"])
            ])
    ];

    public static async Task RunAsync(IConfiguration configuration)
    {
        var template = configuration.GetConnectionString("DefaultConnectionTemplate")
            ?? throw new InvalidOperationException("Falta DefaultConnectionTemplate.");

        var connectionString = template.Replace("{Database}", TargetDatabase, StringComparison.Ordinal);
        var options = new DbContextOptionsBuilder<TenantDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        await using var db = new TenantDbContext(options);

        var tipoNit = await db.TiposIdentificacion
            .FirstOrDefaultAsync(tipo => tipo.Nombre == NitTypeName);

        if (tipoNit is null)
        {
            tipoNit = new TiposIdentificacion { Nombre = NitTypeName };
            db.TiposIdentificacion.Add(tipoNit);
            await db.SaveChangesAsync();
            Console.WriteLine($"Tipo de identificación creado: {NitTypeName} ({tipoNit.Id}).");
        }

        var storage = ResolveStorageFolder(configuration);
        Directory.CreateDirectory(storage);

        var propietariosCreados = 0;
        var negociosCreados = 0;
        var documentosCreados = 0;

        foreach (var dato in Datos)
        {
            var propietario = await db.Propietarios
                .FirstOrDefaultAsync(p => p.TipoId == tipoNit.Id && p.Documento == dato.Documento);

            if (propietario is null)
            {
                propietario = new Propietarios
                {
                    Documento = dato.Documento,
                    TipoId = tipoNit.Id,
                    Nombre = dato.Nombre,
                    Telefono = dato.Telefono,
                    Correo = dato.Correo
                };

                db.Propietarios.Add(propietario);
                await db.SaveChangesAsync();
                propietariosCreados++;
            }

            foreach (var negocio in dato.Negocios)
            {
                var contribuyente = await db.Contribuyentes
                    .FirstOrDefaultAsync(c => c.PropietarioId == propietario.Id
                                              && c.NombreComercial == negocio.Nombre);

                if (contribuyente is null)
                {
                    contribuyente = new Contribuyente
                    {
                        PropietarioId = propietario.Id,
                        NombreComercial = negocio.Nombre,
                        MatriculaMercantil = negocio.Matricula,
                        ActividadEconomica = negocio.Ciiu,
                        Direccion = negocio.Direccion,
                        FechaInicioActividad = negocio.InicioActividad,
                        Estado = EstadoContribuyente.Activo
                    };

                    db.Contribuyentes.Add(contribuyente);
                    await db.SaveChangesAsync();
                    negociosCreados++;
                }

                foreach (var nombreDocumento in negocio.Documentos)
                {
                    var archivo = NombreArchivo(dato.Documento, negocio.Nombre, nombreDocumento);
                    var rutaCompleta = Path.Combine(storage, archivo);

                    var existeDocumento = await db.Documentos
                        .AnyAsync(d => d.ContribuyenteId == contribuyente.Id && d.Nombre == nombreDocumento);

                    if (!existeDocumento)
                    {
                        db.Documentos.Add(new Documentos
                        {
                            ContribuyenteId = contribuyente.Id,
                            Modulo = Modulo.IndustriaComercio,
                            Nombre = nombreDocumento,
                            Fecha = negocio.InicioActividad,
                            Ruta = archivo
                        });

                        await db.SaveChangesAsync();
                        documentosCreados++;
                    }

                    if (!File.Exists(rutaCompleta))
                    {
                        var titulo = $"{negocio.Nombre} - {nombreDocumento}";
                        await File.WriteAllBytesAsync(rutaCompleta, GenerarPdf(titulo));
                    }
                }
            }
        }

        Console.WriteLine($"Datos de demostración listos en {TargetDatabase}: " +
                          $"{propietariosCreados} propietarios, {negociosCreados} negocios, " +
                          $"{documentosCreados} documentos nuevos. Carpeta: {storage}");
    }

    private static string ResolveStorageFolder(IConfiguration configuration)
    {
        var template = configuration["FileStorageTemplate"] ?? string.Empty;
        return template
            .Replace("{Municipio}", TargetDatabase, StringComparison.Ordinal)
            .Replace("{Database}", TargetDatabase, StringComparison.Ordinal)
            .Replace("{Sector}", "IndustriaComercio", StringComparison.Ordinal);
    }

    private static string NombreArchivo(string documento, string negocio, string nombreDocumento) =>
        $"{Slug(documento)}_{Slug(negocio)}_{Slug(nombreDocumento)}.pdf";

    private static string Slug(string valor)
    {
        var normalizado = valor.Normalize(NormalizationForm.FormD);
        var constructor = new StringBuilder();

        foreach (var caracter in normalizado)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(caracter) == UnicodeCategory.NonSpacingMark)
            {
                continue;
            }

            if (char.IsLetterOrDigit(caracter))
            {
                constructor.Append(char.ToLowerInvariant(caracter));
            }
            else if (constructor.Length > 0 && constructor[^1] != '-')
            {
                constructor.Append('-');
            }
        }

        return constructor.ToString().Trim('-');
    }

    /// <summary>Genera un PDF mínimo válido para que los documentos de demostración se puedan abrir.</summary>
    private static byte[] GenerarPdf(string titulo)
    {
        var texto = SinAcentos(titulo).Replace("(", "\\(").Replace(")", "\\)");
        var contenido = $"BT /F1 14 Tf 72 720 Td ({texto}) Tj ET";

        var objetos = new List<string>
        {
            "<< /Type /Catalog /Pages 2 0 R >>",
            "<< /Type /Pages /Kids [3 0 R] /Count 1 >>",
            "<< /Type /Page /Parent 2 0 R /MediaBox [0 0 612 792] /Resources << /Font << /F1 5 0 R >> >> /Contents 4 0 R >>",
            $"<< /Length {contenido.Length} >>\nstream\n{contenido}\nendstream",
            "<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica >>"
        };

        var pdf = new StringBuilder("%PDF-1.4\n");
        var offsets = new List<int>();

        for (var i = 0; i < objetos.Count; i++)
        {
            offsets.Add(pdf.Length);
            pdf.Append($"{i + 1} 0 obj\n{objetos[i]}\nendobj\n");
        }

        var inicioXref = pdf.Length;
        pdf.Append($"xref\n0 {objetos.Count + 1}\n0000000000 65535 f \n");

        foreach (var offset in offsets)
        {
            pdf.Append($"{offset:D10} 00000 n \n");
        }

        pdf.Append($"trailer\n<< /Size {objetos.Count + 1} /Root 1 0 R >>\nstartxref\n{inicioXref}\n%%EOF");

        return Encoding.ASCII.GetBytes(pdf.ToString());
    }

    private static string SinAcentos(string valor)
    {
        var normalizado = valor.Normalize(NormalizationForm.FormD);
        var constructor = new StringBuilder();

        foreach (var caracter in normalizado)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(caracter) != UnicodeCategory.NonSpacingMark)
            {
                constructor.Append(caracter);
            }
        }

        return constructor.ToString();
    }
}
