namespace ShoeStore.Api.Middleware;

public static class HttpContextExtensions
{
    public static int GetUserId(this HttpContext ctx)
        => int.TryParse(ctx.Request.Headers["X-User-Id"].FirstOrDefault(), out var id) ? id : 0;

    public static int GetRoleId(this HttpContext ctx)
        => int.TryParse(ctx.Request.Headers["X-Role-Id"].FirstOrDefault(), out var id) ? id : 0;

    public static string GetLogin(this HttpContext ctx)
        => ctx.Request.Headers["X-Login"].FirstOrDefault() ?? string.Empty;
}