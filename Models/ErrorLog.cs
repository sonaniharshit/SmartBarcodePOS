namespace SmartBarcodePOS_Pro.Models;

public class ErrorLog
{
    public int ErrorLogId { get; set; }
    public string? ControllerName { get; set; }
    public string? ActionName { get; set; }
    public string? RequestPath { get; set; }
    public string? ErrorMessage { get; set; }
    public string? StackTrace { get; set; }
    public string? InnerException { get; set; }
    public string? UserName { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
}
