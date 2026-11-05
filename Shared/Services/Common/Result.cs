namespace Coactivos_Predial.Shared.Services.Common;

/// <summary>Clasifica el origen de un resultado fallido para que la UI decida cómo presentarlo.</summary>
public enum ResultErrorType
{
    None,
    Validation,
    NotFound,
    Conflict,
    Unauthorized,
    NotSupported,
    Unexpected
}

/// <summary>Resultado de una operación que no devuelve datos.</summary>
public sealed record Result(bool IsSuccess, string? Error, ResultErrorType Type)
{
    public bool IsFailure => !IsSuccess;

    public static Result Ok() => new(true, null, ResultErrorType.None);

    public static Result Fail(string error, ResultErrorType type = ResultErrorType.Unexpected) =>
        new(false, error, type);
}

/// <summary>Resultado de una operación que devuelve datos.</summary>
public sealed record Result<T>(bool IsSuccess, T? Value, string? Error, ResultErrorType Type)
{
    public bool IsFailure => !IsSuccess;

    public static Result<T> Ok(T value) => new(true, value, null, ResultErrorType.None);

    public static Result<T> Fail(string error, ResultErrorType type = ResultErrorType.Unexpected) =>
        new(false, default, error, type);
}
