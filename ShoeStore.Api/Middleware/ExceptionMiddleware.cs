using Microsoft.Data.SqlClient;
using ShoeStoreException;
using System.Net;
using System.Text.Json;

namespace ShoeStore.Api.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (SqlException ex)
        {
            var msg = ExceptionLogger.HandleException(Exceptions.DatabaseConnection(ex));
            await WriteError(context, HttpStatusCode.InternalServerError, msg);
        }
        catch (BaseException ex)
        {
            var msg = ExceptionLogger.HandleException(ex);
            await WriteError(context, HttpStatusCode.BadRequest, msg);
        }
        catch (Exception ex)
        {
            var msg = ExceptionLogger.HandleException(ex);
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