using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartBarcodePOS_Pro.Data;
using SmartBarcodePOS_Pro.Services;
using SmartBarcodePOS_Pro.ViewModels;

namespace SmartBarcodePOS_Pro.Controllers;

[Authorize(Roles = "Admin,SubAdmin")]
public class DashboardController : BaseController
{
    private readonly ApplicationDbContext _context;

    public DashboardController(ApplicationDbContext context, IErrorLogService errorLogService) : base(errorLogService)
    {
        _context = context;
    }

    /// <summary>
    /// Handles the Index request.
    /// </summary>
    public async Task<IActionResult> Index()
    {
        try
        {
            var today = DateTime.Today;

            var model = new DashboardViewModel
            {
                TotalProducts = await _context.Products.CountAsync(),
                TotalSales = await _context.SalesBills.SumAsync(x => (decimal?)x.TotalAmount) ?? 0,
                TodaysSales = await _context.SalesBills.Where(x => x.CreatedDate.Date == today).SumAsync(x => (decimal?)x.TotalAmount) ?? 0,
                TotalBills = await _context.SalesBills.CountAsync(),
                LowStockProducts = await _context.Products.CountAsync(x => x.StockQty <= x.MinimumStockAlert)
            };

            return View(model);
        }
        catch (Exception ex)
        {
            await LogErrorAsync(ex);
            TempData["Error"] = "Something went wrong while loading dashboard.";
            return View(new DashboardViewModel());
        }
    }
}