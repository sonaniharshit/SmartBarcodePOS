using SmartBarcodePOS_Pro.Data;
using SmartBarcodePOS_Pro.Models;

namespace SmartBarcodePOS_Pro.Services;

public class ErrorLogService : IErrorLogService
{
    private readonly ApplicationDbContext _context;

    public ErrorLogService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task LogAsync(Exception exception, string? controllerName = null, string? actionName = null, string? requestPath = null, string? userName = null)
    {
        try
        {
            var errorLog = new ErrorLog
            {
                ControllerName = controllerName,
                ActionName = actionName,
                RequestPath = requestPath,
                ErrorMessage = exception.Message,
                StackTrace = exception.StackTrace,
                InnerException = exception.InnerException?.Message,
                UserName = userName,
                CreatedDate = DateTime.UtcNow
            };

            _context.ErrorLogs.Add(errorLog);
            await _context.SaveChangesAsync();
        }
        catch
        {
            // Do not throw from error logger.
        }
    }
}
