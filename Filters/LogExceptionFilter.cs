using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using SmartBarcodePOS_Pro.Services;

namespace SmartBarcodePOS_Pro.Filters;

public class LogExceptionFilter : IAsyncExceptionFilter
{
    private readonly IErrorLogService _errorLogService;
    private readonly ITempDataDictionaryFactory _tempDataFactory;

    public LogExceptionFilter(IErrorLogService errorLogService, ITempDataDictionaryFactory tempDataFactory)
    {
        _errorLogService = errorLogService;
        _tempDataFactory = tempDataFactory;
    }

    public async Task OnExceptionAsync(ExceptionContext context)
    {
        var routeValues = context.HttpContext.Request.RouteValues;

        await _errorLogService.LogAsync(
            context.Exception,
            routeValues["controller"]?.ToString(),
            routeValues["action"]?.ToString(),
            context.HttpContext.Request.Path,
            context.HttpContext.User?.Identity?.Name
        );

        context.HttpContext.Items["ErrorAlreadyLogged"] = true;

        var request = context.HttpContext.Request;
        var isAjax = request.Headers["X-Requested-With"] == "XMLHttpRequest";
        var acceptsJson = request.Headers["Accept"].Any(x => x != null && x.Contains("application/json"));

        if (isAjax || acceptsJson || request.Path.Value?.Contains("GenerateBill", StringComparison.OrdinalIgnoreCase) == true)
        {
            context.Result = new BadRequestObjectResult(new
            {
                success = false,
                message = "Something went wrong. Error has been logged."
            });
        }
        else
        {
            var tempData = _tempDataFactory.GetTempData(context.HttpContext);
            tempData["Error"] = "Something went wrong. Error has been logged.";

            context.Result = new RedirectToActionResult("Index", "Dashboard", null);
        }

        context.ExceptionHandled = true;
    }
}
