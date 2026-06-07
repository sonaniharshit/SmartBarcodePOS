using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartBarcodePOS_Pro.Models;
using SmartBarcodePOS_Pro.Services;
using SmartBarcodePOS_Pro.ViewModels;

namespace SmartBarcodePOS_Pro.Controllers;

[Authorize(Roles = "Admin,SubAdmin")]
public class ReportsController : BaseController
{
    private readonly IReportService _reportService;

    public ReportsController(IReportService reportService, IErrorLogService errorLogService) : base(errorLogService)
    {
        _reportService = reportService;
    }

    /// <summary>
    /// Handles the SalesReport request.
    /// </summary>
    public async Task<IActionResult> SalesReport()
    {
        try
        {
            return View(await _reportService.GetSalesReportAsync());
        }
        catch (Exception ex)
        {
            await LogErrorAsync(ex);
            TempData["Error"] = "Something went wrong while loading sales report.";
            return View(new SalesReportViewModel());
        }
    }

    /// <summary>
    /// Handles the ProductSales request.
    /// </summary>
    public async Task<IActionResult> ProductSales()
    {
        try
        {
            return View(await _reportService.GetProductSalesAsync());
        }
        catch (Exception ex)
        {
            await LogErrorAsync(ex);
            TempData["Error"] = "Something went wrong while loading product sales report.";
            return View(new List<SalesBillItem>());
        }
    }

    /// <summary>
    /// Handles the TopSelling request.
    /// </summary>
    public async Task<IActionResult> TopSelling()
    {
        try
        {
            return View(await _reportService.GetTopSellingProductsAsync());
        }
        catch (Exception ex)
        {
            await LogErrorAsync(ex);
            TempData["Error"] = "Something went wrong while loading top selling report.";
            return View(new List<TopSellingProductViewModel>());
        }
    }

    /// <summary>
    /// Handles the StockReport request.
    /// </summary>
    public async Task<IActionResult> StockReport()
    {
        try
        {
            return View(await _reportService.GetStockReportAsync());
        }
        catch (Exception ex)
        {
            await LogErrorAsync(ex);
            TempData["Error"] = "Something went wrong while loading stock report.";
            return View(new List<Product>());
        }
    }
}