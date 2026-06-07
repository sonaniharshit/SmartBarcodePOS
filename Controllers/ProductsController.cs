using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SmartBarcodePOS_Pro.Data;
using SmartBarcodePOS_Pro.Services;
using SmartBarcodePOS_Pro.ViewModels;

namespace SmartBarcodePOS_Pro.Controllers;

[Authorize(Roles = "Admin,SubAdmin")]
public class ProductsController : BaseController
{
    private readonly IProductService _productService;
    private readonly ApplicationDbContext _context;

    public ProductsController(IProductService productService, ApplicationDbContext context, IErrorLogService errorLogService) : base(errorLogService)
    {
        _productService = productService;
        _context = context;
    }

    /// <summary>
    /// Handles the Index request.
    /// </summary>
    public async Task<IActionResult> Index()
    {
        try
        {
            return View(await _productService.GetAllAsync());
        }
        catch (Exception ex)
        {
            await LogErrorAsync(ex);
            TempData["Error"] = "Something went wrong while loading products.";
            return View(new List<Models.Product>());
        }
    }

    /// <summary>
    /// Handles the Create request.
    /// </summary>
    public async Task<IActionResult> Create()
    {
        try
        {
            return View(await _productService.GetCreateModelAsync());
        }
        catch (Exception ex)
        {
            await LogErrorAsync(ex);
            TempData["Error"] = "Something went wrong while opening product form.";
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    /// <summary>
    /// Handles the Create request .
    /// </summary>
    public async Task<IActionResult> Create(ProductViewModel model)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var newModel = await _productService.GetCreateModelAsync();
                model.SKU = newModel.SKU;
                model.Categories = await GetCategorySelectListByBrandAsync(model.BrandId);
                model.Brands = newModel.Brands;
                TempData["Error"] = "Please fix validation errors.";
                return View(model);
            }

            var product = await _productService.CreateAsync(model);
            TempData["Success"] = "Product saved and barcode generated successfully.";
            return RedirectToAction(nameof(Barcode), new { id = product.ProductId });
        }
        catch (InvalidOperationException ex)
        {
            await LogErrorAsync(ex);
            var newModel = await _productService.GetCreateModelAsync();
            model.Categories = await GetCategorySelectListByBrandAsync(model.BrandId);
            model.Brands = newModel.Brands;
            TempData["Error"] = ex.Message;
            return View(model);
        }
        catch (Exception ex)
        {
            await LogErrorAsync(ex);
            var newModel = await _productService.GetCreateModelAsync();
            model.Categories = await GetCategorySelectListByBrandAsync(model.BrandId);
            model.Brands = newModel.Brands;
            TempData["Error"] = "Something went wrong while saving product.";
            return View(model);
        }
    }

    /// <summary>
    /// Handles the Edit request.
    /// </summary>
    public async Task<IActionResult> Edit(int id)
    {
        try
        {
            var model = await _productService.GetEditModelAsync(id);
            if (model == null)
            {
                TempData["Error"] = "Product not found.";
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }
        catch (Exception ex)
        {
            await LogErrorAsync(ex);
            TempData["Error"] = "Something went wrong while loading product.";
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    /// <summary>
    /// Handles the Edit request.
    /// </summary>
    public async Task<IActionResult> Edit(ProductViewModel model)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var editModel = await _productService.GetEditModelAsync(model.ProductId);
                model.Categories = await GetCategorySelectListByBrandAsync(model.BrandId);
                model.Brands = editModel?.Brands ?? new List<SelectListItem>();
                TempData["Error"] = "Please fix validation errors.";
                return View(model);
            }

            var ok = await _productService.UpdateAsync(model);
            TempData[ok ? "Success" : "Error"] = ok ? "Product updated successfully." : "Product not found.";
            return RedirectToAction(nameof(Index));
        }
        catch (InvalidOperationException ex)
        {
            await LogErrorAsync(ex);
            var editModel = await _productService.GetEditModelAsync(model.ProductId);
            model.Categories = await GetCategorySelectListByBrandAsync(model.BrandId);
            model.Brands = editModel?.Brands ?? new List<SelectListItem>();
            TempData["Error"] = ex.Message;
            return View(model);
        }
        catch (Exception ex)
        {
            await LogErrorAsync(ex);
            var editModel = await _productService.GetEditModelAsync(model.ProductId);
            model.Categories = await GetCategorySelectListByBrandAsync(model.BrandId);
            model.Brands = editModel?.Brands ?? new List<SelectListItem>();
            TempData["Error"] = "Something went wrong while updating product.";
            return View(model);
        }
    }

    /// <summary>
    /// Handles the Details request.
    /// </summary>
    public async Task<IActionResult> Details(int id)
    {
        try
        {
            var product = await _productService.GetByIdAsync(id);
            if (product == null)
            {
                TempData["Error"] = "Product not found.";
                return RedirectToAction(nameof(Index));
            }

            return View(product);
        }
        catch (Exception ex)
        {
            await LogErrorAsync(ex);
            TempData["Error"] = "Something went wrong while loading product details.";
            return RedirectToAction(nameof(Index));
        }
    }

    /// <summary>
    /// Handles the Barcode request.
    /// </summary>
    public async Task<IActionResult> Barcode(int id)
    {
        try
        {
            var product = await _productService.GetByIdAsync(id);
            if (product == null)
            {
                TempData["Error"] = "Product not found.";
                return RedirectToAction(nameof(Index));
            }

            return View(product);
        }
        catch (Exception ex)
        {
            await LogErrorAsync(ex);
            TempData["Error"] = "Something went wrong while loading barcode.";
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    /// <summary>
    /// Handles the Delete request.
    /// </summary>
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var ok = await _productService.DeleteAsync(id);
            TempData[ok ? "Success" : "Error"] = ok ? "Product deleted successfully." : "Product not found.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            await LogErrorAsync(ex);
            TempData["Error"] = "Something went wrong while deleting product.";
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpGet]
    /// <summary>
    /// Handles the GetCategoriesByBrand request.
    /// </summary>
    public async Task<IActionResult> GetCategoriesByBrand(int brandId)
    {
        try
        {
            var categories = await _context.Categories
                .Where(x => x.IsActive && x.BrandId == brandId)
                .OrderBy(x => x.CategoryName)
                .Select(x => new
                {
                    id = x.CategoryId,
                    name = x.CategoryName
                })
                .ToListAsync();

            return Json(categories);
        }
        catch (Exception ex)
        {
            await LogErrorAsync(ex);
            return BadRequest(new { success = false, message = "Something went wrong while loading categories." });
        }
    }

    /// <summary>
    /// Handles the DownloadBarcodeLabel request.
    /// </summary>
    public async Task<IActionResult> DownloadBarcodeLabel(int id)
    {
        try
        {
            var product = await _productService.GetByIdAsync(id);

            if (product == null || string.IsNullOrWhiteSpace(product.BarcodeImage))
            {
                TempData["Error"] = "Barcode not found.";
                return RedirectToAction(nameof(Index));
            }

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", product.BarcodeImage.TrimStart('/'));

            if (!System.IO.File.Exists(filePath))
            {
                TempData["Error"] = "Barcode image file not found.";
                return RedirectToAction(nameof(Index));
            }

            var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
            return File(fileBytes, "image/png", $"{product.SKU}_barcode.png");
        }
        catch (Exception ex)
        {
            await LogErrorAsync(ex);
            TempData["Error"] = "Something went wrong while downloading barcode.";
            return RedirectToAction(nameof(Index));
        }
    }

    /// <summary>
    /// Handles the GetCategorySelectListByBrandAsync request.
    /// </summary>
    private async Task<List<SelectListItem>> GetCategorySelectListByBrandAsync(int? brandId)
    {
        if (!brandId.HasValue || brandId.Value <= 0)
            return new List<SelectListItem>();

        return await _context.Categories
            .Where(x => x.IsActive && x.BrandId == brandId.Value)
            .OrderBy(x => x.CategoryName)
            .Select(x => new SelectListItem
            {
                Value = x.CategoryId.ToString(),
                Text = x.CategoryName
            })
            .ToListAsync();
    }
}