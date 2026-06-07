using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SmartBarcodePOS_Pro.Data;
using SmartBarcodePOS_Pro.Models;
using SmartBarcodePOS_Pro.ViewModels;

namespace SmartBarcodePOS_Pro.Services;

public class ProductService : IProductService
{
    private readonly ApplicationDbContext _context;
    private readonly IBarcodeService _barcodeService;

    public ProductService(ApplicationDbContext context, IBarcodeService barcodeService)
    {
        _context = context;
        _barcodeService = barcodeService;
    }

    public async Task<List<Product>> GetAllAsync()
    {
        return await _context.Products.Include(x => x.Category).Include(x => x.Brand).OrderByDescending(x => x.ProductId).ToListAsync();
    }

    public async Task<Product?> GetByIdAsync(int id)
    {
        return await _context.Products.Include(x => x.Category).Include(x => x.Brand).FirstOrDefaultAsync(x => x.ProductId == id);
    }

    public async Task<ProductViewModel> GetCreateModelAsync()
    {
        return new ProductViewModel
        {
            SKU = await GenerateSkuAsync(),
            Categories = new List<SelectListItem>(),
            Brands = await GetBrandsAsync()
        };
    }

    public async Task<ProductViewModel?> GetEditModelAsync(int id)
    {
        var product = await GetByIdAsync(id);
        if (product == null) return null;

        return new ProductViewModel
        {
            ProductId = product.ProductId,
            SKU = product.SKU,
            ProductName = product.ProductName,
            CategoryId = product.CategoryId,
            BrandId = product.BrandId,
            Description = product.Description,
            Price = product.Price,
            CostPrice = product.CostPrice,
            StockQty = product.StockQty,
            MinimumStockAlert = product.MinimumStockAlert,
            IsActive = product.IsActive,
            BarcodeImage = product.BarcodeImage,
            Categories = await GetCategoriesByBrandAsync(product.BrandId),
            Brands = await GetBrandsAsync()
        };
    }

    public async Task<Product> CreateAsync(ProductViewModel model)
    {
        var sku = await GenerateSkuAsync();

        var categoryValid = await _context.Categories
            .AnyAsync(x => x.CategoryId == model.CategoryId && x.BrandId == model.BrandId && x.IsActive);

        if (!categoryValid)
            throw new InvalidOperationException("Selected category does not belong to selected brand.");

        var product = new Product
        {
            SKU = sku,
            ProductName = model.ProductName,
            CategoryId = model.CategoryId,
            BrandId = model.BrandId,
            Description = model.Description,
            Price = model.Price,
            CostPrice = model.CostPrice,
            StockQty = model.StockQty,
            MinimumStockAlert = model.MinimumStockAlert,
            IsActive = model.IsActive,
            CreatedDate = DateTime.UtcNow
        };

        product.BarcodeImage = _barcodeService.GenerateCode128Barcode(sku);

        _context.Products.Add(product);
        await _context.SaveChangesAsync();
        return product;
    }

    public async Task<bool> UpdateAsync(ProductViewModel model)
    {
        var product = await _context.Products.FirstOrDefaultAsync(x => x.ProductId == model.ProductId);
        if (product == null) return false;

        var categoryValid = await _context.Categories
            .AnyAsync(x => x.CategoryId == model.CategoryId && x.BrandId == model.BrandId && x.IsActive);

        if (!categoryValid)
            throw new InvalidOperationException("Selected category does not belong to selected brand.");

        product.ProductName = model.ProductName;
        product.CategoryId = model.CategoryId;
        product.BrandId = model.BrandId;
        product.Description = model.Description;
        product.Price = model.Price;
        product.CostPrice = model.CostPrice;
        product.StockQty = model.StockQty;
        product.MinimumStockAlert = model.MinimumStockAlert;
        product.IsActive = model.IsActive;

        if (string.IsNullOrWhiteSpace(product.BarcodeImage))
            product.BarcodeImage = _barcodeService.GenerateCode128Barcode(product.SKU);

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null) return false;

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();
        return true;
    }

    private async Task<string> GenerateSkuAsync()
    {
        var lastId = await _context.Products.OrderByDescending(x => x.ProductId).Select(x => x.ProductId).FirstOrDefaultAsync();
        return $"PRD-{lastId + 1:000000}";
    }

    private async Task<List<SelectListItem>> GetCategoriesAsync()
    {
        return await _context.Categories.Where(x => x.IsActive)
            .OrderBy(x => x.CategoryName)
            .Select(x => new SelectListItem { Value = x.CategoryId.ToString(), Text = x.CategoryName })
            .ToListAsync();
    }


    private async Task<List<SelectListItem>> GetCategoriesByBrandAsync(int? brandId)
    {
        if (!brandId.HasValue || brandId.Value <= 0)
            return new List<SelectListItem>();

        return await _context.Categories
            .Where(x => x.IsActive && x.BrandId == brandId.Value)
            .OrderBy(x => x.CategoryName)
            .Select(x => new SelectListItem { Value = x.CategoryId.ToString(), Text = x.CategoryName })
            .ToListAsync();
    }

    private async Task<List<SelectListItem>> GetBrandsAsync()
    {
        return await _context.Brands.Where(x => x.IsActive)
            .OrderBy(x => x.BrandName)
            .Select(x => new SelectListItem { Value = x.BrandId.ToString(), Text = x.BrandName })
            .ToListAsync();
    }
}

