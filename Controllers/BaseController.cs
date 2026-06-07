using Microsoft.AspNetCore.Mvc;
using SmartBarcodePOS_Pro.Services;

namespace SmartBarcodePOS_Pro.Controllers;

public abstract class BaseController : Controller
{
    protected readonly IErrorLogService ErrorLogService;

    protected BaseController(IErrorLogService errorLogService)
    {
        ErrorLogService = errorLogService;
    }

    /// <summary>
    /// Handles the LogErrorAsync request.
    /// </summary>
    protected async Task LogErrorAsync(Exception ex)
    {
        await ErrorLogService.LogAsync(
            ex,
            RouteData.Values["controller"]?.ToString(),
            RouteData.Values["action"]?.ToString(),
            HttpContext?.Request?.Path,
            User?.Identity?.Name
        );
    }
}