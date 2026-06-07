using Microsoft.EntityFrameworkCore;
using SmartBarcodePOS_Pro.Data;
using SmartBarcodePOS_Pro.Models;
using SmartBarcodePOS_Pro.ViewModels;

namespace SmartBarcodePOS_Pro.Services;

public class BillingService : IBillingService
{
    private readonly ApplicationDbContext _context;

    public BillingService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<BillingProductViewModel?> GetProductBySkuAsync(string sku)
    {
        var p = await _context.Products.Include(x => x.Category)
            .FirstOrDefaultAsync(x => x.SKU == sku && x.IsActive);

        if (p == null) return null;

        return new BillingProductViewModel
        {
            ProductId = p.ProductId,
            SKU = p.SKU,
            ProductName = p.ProductName,
            CategoryName = p.Category!.CategoryName,
            Price = p.Price,
            StockQty = p.StockQty
        };
    }

    public async Task<(bool Success, string Message, int BillId, string? BillNo)> GenerateBillAsync(GenerateBillViewModel model)
    {
        if (model.Items.Count == 0)
            return (false, "Cart is empty.", 0, null);

        using var trx = await _context.Database.BeginTransactionAsync();

        try
        {
            var ids = model.Items.Select(x => x.ProductId).ToList();
            var products = await _context.Products.Where(x => ids.Contains(x.ProductId)).ToListAsync();

            foreach (var item in model.Items)
            {
                var p = products.FirstOrDefault(x => x.ProductId == item.ProductId);
                if (p == null) return (false, $"Product not found: {item.ProductName}", 0, null);
                if (item.Qty <= 0) return (false, $"Invalid qty: {p.ProductName}", 0, null);
                if (p.StockQty < item.Qty) return (false, $"Only {p.StockQty} stock available for {p.ProductName}", 0, null);
            }

            var subTotal = Math.Round(model.Items.Sum(item =>
            {
                var product = products.First(x => x.ProductId == item.ProductId);
                return product.Price * item.Qty;
            }), 2);

            var discountPercent = Math.Round(model.DiscountPercent, 2);

            if (discountPercent < 0 || discountPercent > 100)
                return (false, "Discount percent must be between 0 and 100.", 0, null);

            var discountAmount = Math.Round((subTotal * discountPercent) / 100, 2);
            var total = Math.Round(subTotal - discountAmount, 2);

            var bill = new SalesBill
            {
                BillNo = await GenerateBillNoAsync(),
                CreatedDate = DateTime.UtcNow,
                SubTotal = subTotal,
                DiscountPercent = discountPercent,
                DiscountAmount = discountAmount,
                TotalAmount = total,
            };

            foreach (var item in model.Items)
            {
                var p = products.First(x => x.ProductId == item.ProductId);
                bill.Items.Add(new SalesBillItem
                {
                    ProductId = p.ProductId,
                    Qty = item.Qty,
                    Price = p.Price,
                    Total = p.Price * item.Qty
                });

                p.StockQty -= item.Qty;
            }

            _context.SalesBills.Add(bill);
            await _context.SaveChangesAsync();
            await trx.CommitAsync();

            return (true, "Bill generated successfully.", bill.SalesBillId, bill.BillNo);
        }
        catch (Exception ex)
        {
            await trx.RollbackAsync();
            return (false, ex.Message, 0, null);
        }
    }

    public async Task<List<SalesBill>> GetBillsAsync()
    {
        return await _context.SalesBills.OrderByDescending(x => x.SalesBillId).ToListAsync();
    }

    public async Task<SalesBill?> GetBillAsync(int id)
    {
        return await _context.SalesBills
            .Include(x => x.Items)
            .ThenInclude(x => x.Product)
            .FirstOrDefaultAsync(x => x.SalesBillId == id);
    }

    private async Task<string> GenerateBillNoAsync()
    {
        var year = DateTime.UtcNow.Year;
        var last = await _context.SalesBills.Where(x => x.BillNo.StartsWith($"INV-{year}"))
            .OrderByDescending(x => x.SalesBillId)
            .FirstOrDefaultAsync();

        var next = 1;
        if (last != null)
        {
            var n = last.BillNo.Replace($"INV-{year}", "");
            if (int.TryParse(n, out var lastNo)) next = lastNo + 1;
        }

        return $"INV-{year}{next:00001}";
    }
}
