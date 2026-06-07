using Microsoft.EntityFrameworkCore;
using SmartBarcodePOS_Pro.Data;
using SmartBarcodePOS_Pro.Models;
using SmartBarcodePOS_Pro.ViewModels;

namespace SmartBarcodePOS_Pro.Services;

public class ReportService : IReportService
{
    private readonly ApplicationDbContext _context;

    public ReportService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<SalesReportViewModel> GetSalesReportAsync()
    {
        var today = DateTime.Today;
        var monthStart = new DateTime(today.Year, today.Month, 1);

        var dailyBills = await _context.SalesBills
            .Where(x => x.CreatedDate.Date == today)
            .OrderByDescending(x => x.SalesBillId)
            .ToListAsync();

        var monthlyBills = await _context.SalesBills
            .Where(x => x.CreatedDate >= monthStart)
            .OrderByDescending(x => x.SalesBillId)
            .ToListAsync();

        return new SalesReportViewModel
        {
            DailyBills = dailyBills,
            MonthlyBills = monthlyBills,
            DailyTotal = dailyBills.Sum(x => x.TotalAmount),
            MonthlyTotal = monthlyBills.Sum(x => x.TotalAmount)
        };
    }

    public async Task<List<SalesBillItem>> GetProductSalesAsync()
    {
        return await _context.SalesBillItems
            .Include(x => x.Product)
            .OrderByDescending(x => x.SalesBillItemId)
            .Take(100)
            .ToListAsync();
    }

    public async Task<List<TopSellingProductViewModel>> GetTopSellingProductsAsync()
    {
        return await _context.SalesBillItems
            .Include(x => x.Product)
            .GroupBy(x => new { x.ProductId, x.Product!.ProductName })
            .Select(g => new TopSellingProductViewModel
            {
                ProductName = g.Key.ProductName,
                TotalQty = g.Sum(x => x.Qty),
                TotalAmount = g.Sum(x => x.Total)
            })
            .OrderByDescending(x => x.TotalQty)
            .Take(20)
            .ToListAsync();
    }

    public async Task<List<Product>> GetStockReportAsync()
    {
        return await _context.Products
            .Include(x => x.Category)
            .OrderBy(x => x.ProductName)
            .ToListAsync();
    }
}
