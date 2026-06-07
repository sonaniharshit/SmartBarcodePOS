using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartBarcodePOS_Pro.Data;
using SmartBarcodePOS_Pro.Services;

namespace SmartBarcodePOS_Pro.Controllers;

[Authorize(Roles = "Admin,SubAdmin")]
public class BillsController : BaseController
{
    private readonly IBillingService _billingService;
    private readonly ApplicationDbContext _context;

    public BillsController(IBillingService billingService, ApplicationDbContext context, IErrorLogService errorLogService) : base(errorLogService)
    {
        _billingService = billingService;
        _context = context;
    }

    /// <summary>
    /// Handles the Index request.
    /// </summary>
    public async Task<IActionResult> Index()
    {
        try
        {
            return View(await _billingService.GetBillsAsync());
        }
        catch (Exception ex)
        {
            await LogErrorAsync(ex);
            TempData["Error"] = "Something went wrong while loading bill history.";
            return View(new List<Models.SalesBill>());
        }
    }

    /// <summary>
    /// Handles the Print request.
    /// </summary>
    public async Task<IActionResult> Print(int id)
    {
        try
        {
            var bill = await _billingService.GetBillAsync(id);
            if (bill == null)
            {
                TempData["Error"] = "Bill not found.";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.AppSetting = await _context.AppSettings.FirstOrDefaultAsync();
            return View(bill);
        }
        catch (Exception ex)
        {
            await LogErrorAsync(ex);
            TempData["Error"] = "Something went wrong while printing bill.";
            return RedirectToAction(nameof(Index));
        }
    }
}