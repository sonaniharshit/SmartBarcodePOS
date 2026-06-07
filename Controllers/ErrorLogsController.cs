using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartBarcodePOS_Pro.Data;

namespace SmartBarcodePOS_Pro.Controllers;

[Authorize(Roles = "Admin")]
public class ErrorLogsController : Controller
{
    private readonly ApplicationDbContext _context;

    public ErrorLogsController(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Handles the Index request.
    /// </summary>
    public async Task<IActionResult> Index()
    {
        var logs = await _context.ErrorLogs
            .OrderByDescending(x => x.ErrorLogId)
            .Take(500)
            .ToListAsync();

        return View(logs);
    }

    /// <summary>
    /// Handles the Details request.
    /// </summary>
    public async Task<IActionResult> Details(int id)
    {
        var log = await _context.ErrorLogs.FirstOrDefaultAsync(x => x.ErrorLogId == id);
        if (log == null)
            return RedirectToAction(nameof(Index));

        return View(log);
    }
}