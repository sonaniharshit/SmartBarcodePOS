using SmartBarcodePOS_Pro.Services;

namespace SmartBarcodePOS_Pro.Middleware;

public class ErrorLoggingMiddleware
{
    private readonly RequestDelegate _next;

    public ErrorLoggingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IErrorLogService errorLogService)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            var routeValues = context.Request.RouteValues;

            if (!context.Items.ContainsKey("ErrorAlreadyLogged"))
            {
                await errorLogService.LogAsync(
                    ex,
                    routeValues["controller"]?.ToString(),
                    routeValues["action"]?.ToString(),
                    context.Request.Path,
                    context.User?.Identity?.Name
                );
            }

            throw;
        }
    }
}
