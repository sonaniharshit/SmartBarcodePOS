using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartBarcodePOS_Pro.Data;
using SmartBarcodePOS_Pro.Models;
using SmartBarcodePOS_Pro.Services;

namespace SmartBarcodePOS_Pro.Controllers;

[Authorize(Roles = "Admin,SubAdmin")]
public class InventoryController : BaseController
{
    private readonly ApplicationDbContext _context;

    public InventoryController(ApplicationDbContext context, IErrorLogService errorLogService) : base(errorLogService)
    {
        _context = context;
    }

    /// <summary>
    /// Handles the StockIn request.
    /// </summary>
    public async Task<IActionResult> StockIn()
    {
        try
        {
            return View();
        }
        catch (Exception ex)
        {
            await LogErrorAsync(ex);
            TempData["Error"] = "Something went wrong while opening Stock In.";
            return RedirectToAction("Index", "Dashboard");
        }
    }

    /// <summary>
    /// Handles the StockAdjustment request.
    /// </summary>
    public async Task<IActionResult> StockAdjustment()
    {
        try
        {
            return View();
        }
        catch (Exception ex)
        {
            await LogErrorAsync(ex);
            TempData["Error"] = "Something went wrong while opening Stock Adjustment.";
            return RedirectToAction("Index", "Dashboard");
        }
    }

    /// <summary>
    /// Handles the LowStock request.
    /// </summary>
    public async Task<IActionResult> LowStock()
    {
        try
        {
            var products = await _context.Products
                .Include(x => x.Category)
                .Where(x => x.StockQty <= x.MinimumStockAlert)
                .OrderBy(x => x.StockQty)
                .ToListAsync();

            return View(products);
        }
        catch (Exception ex)
        {
            await LogErrorAsync(ex);
            TempData["Error"] = "Something went wrong while loading low stock products.";
            return View(new List<Product>());
        }
    }

    /// <summary>
    /// Handles the StockLedger request .
    /// </summary>
    public async Task<IActionResult> StockLedger()
    {
        try
        {
            return View();
        }
        catch (Exception ex)
        {
            await LogErrorAsync(ex);
            TempData["Error"] = "Something went wrong while opening stock ledger.";
            return RedirectToAction("Index", "Dashboard");
        }
    }
}