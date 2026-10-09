namespace ShoeStore.Web.Services;

public record ApiResult<T>(bool Success, T? Value, string? Error, int StatusCode)
{
    public static ApiResult<T> Ok(T value) => new(true, value, null, 200);
    public static ApiResult<T> Fail(int code, string e) => new(false, default, e, code);
}