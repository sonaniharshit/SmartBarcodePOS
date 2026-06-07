namespace SmartBarcodePOS_Pro.Services;

public interface IErrorLogService
{
    Task LogAsync(Exception exception, string? controllerName = null, string? actionName = null, string? requestPath = null, string? userName = null);
}
