using System.Net;
using System.Text.Json;
using ShoeStoreException;

namespace ShoeStore.Api.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (BaseException ex)
        {
            // Известное ("наше") исключение — лог + 400 клиенту
            var msg = ExceptionLogger.HandleException(ex);
            _logger.LogWarning(ex, "BaseException: {Message}", msg);
            await WriteError(context, HttpStatusCode.BadRequest, msg);
        }
        catch (Exception ex)
        {
            // Неизвестное — 500
            var msg = ExceptionLogger.HandleException(ex);
            _logger.LogError(ex, "Необработанная ошибка: {Message}", msg);
            await WriteError(context, HttpStatusCode.InternalServerError, msg);
        }
    }

    private static async Task WriteError(HttpContext ctx, HttpStatusCode code, string message)
    {
        ctx.Response.ContentType = "application/json";
        ctx.Response.StatusCode = (int)code;
        await ctx.Response.WriteAsync(JsonSerializer.Serialize(new { error = message }));
    }
}