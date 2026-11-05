namespace Coactivos_Predial.Shared.Services.Common;

/// <summary>
/// Centraliza el try/catch de los servicios para que cada método solo exprese su caso de uso.
/// </summary>
public static class ResultExecution
{
    public static async Task<Result<T>> ExecuteAsync<T>(Func<Task<T>> action, string fallbackError)
    {
        try
        {
            return Result<T>.Ok(await action());
        }
        catch (Exception ex)
        {
            return Result<T>.Fail(ToErrorMessage(ex, fallbackError), ToErrorType(ex));
        }
    }

    public static async Task<Result> ExecuteAsync(Func<Task> action, string fallbackError)
    {
        try
        {
            await action();
            return Result.Ok();
        }
        catch (Exception ex)
        {
            return Result.Fail(ToErrorMessage(ex, fallbackError), ToErrorType(ex));
        }
    }

    private static ResultErrorType ToErrorType(Exception ex) => ex switch
    {
        ArgumentException => ResultErrorType.Validation,
        KeyNotFoundException => ResultErrorType.NotFound,
        UnauthorizedAccessException => ResultErrorType.Unauthorized,
        NotSupportedException => ResultErrorType.NotSupported,
        InvalidOperationException => ResultErrorType.Conflict,
        _ => ResultErrorType.Unexpected
    };

    private static string ToErrorMessage(Exception ex, string fallbackError) =>
        ToErrorType(ex) == ResultErrorType.Unexpected
            ? fallbackError
            : ex.Message;
}
