using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ShoeStore.Api.Middleware;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class RoleAuthorizeAttribute : Attribute, IAsyncAuthorizationFilter
{
    private readonly string[] _allowed;

    public RoleAuthorizeAttribute(params string[] roles) => _allowed = roles;

    public Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var roleId = context.HttpContext.Request.Headers["X-Role-Id"].FirstOrDefault();
        var userId = context.HttpContext.Request.Headers["X-User-Id"].FirstOrDefault();

        if (string.IsNullOrEmpty(roleId) || string.IsNullOrEmpty(userId))
        {
            context.Result = new UnauthorizedObjectResult(new { error = "Требуется авторизация" });
            return Task.CompletedTask;
        }

        if (!_allowed.Contains(roleId))
        {
            context.Result = new ObjectResult(new { error = "Недостаточно прав" })
            {
                StatusCode = StatusCodes.Status403Forbidden
            };
        }

        return Task.CompletedTask;
    }
}