namespace ShoeStore.Api.Middleware;

/// <summary>Исключение уровня API — middleware превратит его в HTTP-ответ.</summary>
public class ApiException : Exception
{
    public int StatusCode { get; }

    public ApiException(string message, int statusCode = 400) : base(message)
        => StatusCode = statusCode;
}

public static class ApiStatus
{
    public const int BadRequest = 400;
    public const int Forbidden = 403;
    public const int NotFound = 404;
    public const int Conflict = 409;
}