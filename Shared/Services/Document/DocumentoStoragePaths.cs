using System.Text;

namespace Coactivos_Predial.Shared.Services.Document;

/// <summary>
/// Reglas puras de nombres y rutas del almacenamiento documental. No toca la
/// base de datos ni conoce el municipio: recibe siempre la carpeta base.
/// </summary>
public static class DocumentoStoragePaths
{
    public const string PdfExtension = ".pdf";
    private const int MaxFileNameLength = 180;

    // Se aplican también en Linux para que el nombre de descarga sea portable.
    private static readonly char[] InvalidNameChars = ['<', '>', ':', '"', '/', '\\', '|', '?', '*'];

    /// <summary>Clave opaca del archivo. Nunca se deriva del nombre original.</summary>
    public static string CreateStorageKey() => $"{Guid.NewGuid():N}{PdfExtension}";

    /// <summary>
    /// Distribuye los archivos en dos niveles de dos caracteres para no concentrar
    /// miles de PDF en una sola carpeta: ab/cd/abcd....pdf
    /// </summary>
    public static string GetRelativePath(string storageKey)
    {
        var fileName = ValidateStorageKey(storageKey);
        return Path.Combine(fileName[..2], fileName[2..4], fileName);
    }

    public static string GetFullPath(string baseFolder, string storageKey)
    {
        if (string.IsNullOrWhiteSpace(baseFolder))
            throw new ArgumentException("La carpeta de almacenamiento no está configurada.", nameof(baseFolder));

        var fullBase = Path.GetFullPath(baseFolder);
        var fullPath = Path.GetFullPath(Path.Combine(fullBase, GetRelativePath(storageKey)));
        EnsureInsideBase(fullBase, fullPath);
        return fullPath;
    }

    /// <summary>Valida que la clave sea un nombre de archivo simple y con extensión PDF.</summary>
    public static string ValidateStorageKey(string storageKey)
    {
        if (string.IsNullOrWhiteSpace(storageKey))
            throw new ArgumentException("La clave de almacenamiento está vacía.", nameof(storageKey));

        var fileName = Path.GetFileName(storageKey);
        if (!string.Equals(fileName, storageKey, StringComparison.Ordinal)
            || fileName is "." or ".."
            || fileName.IndexOfAny(['/', '\\', ':', '\0']) >= 0)
        {
            throw new ArgumentException("La clave de almacenamiento no es válida.", nameof(storageKey));
        }

        if (!string.Equals(Path.GetExtension(fileName), PdfExtension, StringComparison.OrdinalIgnoreCase))
            throw new ArgumentException("La clave de almacenamiento debe tener extensión .pdf.", nameof(storageKey));

        if (fileName.Length < 4)
            throw new ArgumentException("La clave de almacenamiento es demasiado corta.", nameof(storageKey));

        return fileName;
    }

    /// <summary>
    /// Resuelve una ruta heredada. Solo se conserva el nombre del archivo y se
    /// busca dentro de la carpeta del sector; nunca se confía en la ruta completa.
    /// </summary>
    public static bool TryResolveLegacyPath(string baseFolder, string? storedPath, out string fileName, out string fullPath)
    {
        var normalizedStoredPath = storedPath?.Replace('\\', '/') ?? string.Empty;
        fileName = Path.GetFileName(normalizedStoredPath);
        fullPath = string.Empty;

        if (string.IsNullOrWhiteSpace(baseFolder)
            || string.IsNullOrWhiteSpace(storedPath)
            || fileName is "" or "." or ".."
            || normalizedStoredPath.Split('/').Any(segment => segment is "." or ".."))
        {
            return false;
        }

        var fullBase = Path.GetFullPath(baseFolder);
        fullPath = Path.GetFullPath(Path.Combine(fullBase, fileName));
        return IsInsideBase(fullBase, fullPath);
    }

    /// <summary>
    /// Limpia el nombre original para mostrarlo y descargarlo. No se usa para
    /// construir rutas físicas.
    /// </summary>
    public static string SanitizeFileName(string? original, string fallback = "documento.pdf")
    {
        var name = Path.GetFileName((original ?? string.Empty).Trim());
        if (string.IsNullOrWhiteSpace(name) || name is "." or "..")
            name = fallback;

        var builder = new StringBuilder(name.Length);
        foreach (var character in name)
        {
            builder.Append(InvalidNameChars.Contains(character) || char.IsControl(character) ? '_' : character);
        }

        name = builder.ToString().Trim().TrimEnd('.');
        if (string.IsNullOrWhiteSpace(name))
            name = fallback;

        if (!string.Equals(Path.GetExtension(name), PdfExtension, StringComparison.OrdinalIgnoreCase))
            name += PdfExtension;

        if (name.Length <= MaxFileNameLength)
            return name;

        var extension = Path.GetExtension(name);
        var stem = Path.GetFileNameWithoutExtension(name);
        var maxStem = Math.Max(1, MaxFileNameLength - extension.Length);
        return stem[..Math.Min(stem.Length, maxStem)] + extension;
    }

    private static void EnsureInsideBase(string fullBase, string fullPath)
    {
        if (!IsInsideBase(fullBase, fullPath))
            throw new InvalidOperationException("La ruta calculada queda fuera de la carpeta de almacenamiento.");
    }

    private static bool IsInsideBase(string fullBase, string fullPath)
    {
        var normalizedBase = fullBase.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
            + Path.DirectorySeparatorChar;
        var comparison = OperatingSystem.IsWindows()
            ? StringComparison.OrdinalIgnoreCase
            : StringComparison.Ordinal;
        return fullPath.StartsWith(normalizedBase, comparison);
    }
}
