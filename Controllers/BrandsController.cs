using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartBarcodePOS_Pro.Data;
using SmartBarcodePOS_Pro.Models;
using SmartBarcodePOS_Pro.Services;
using SmartBarcodePOS_Pro.ViewModels;

namespace SmartBarcodePOS_Pro.Controllers;

[Authorize(Roles = "Admin,SubAdmin")]
public class BrandsController : BaseController
{
    private readonly ApplicationDbContext _context;

    public BrandsController(ApplicationDbContext context, IErrorLogService errorLogService) : base(errorLogService)
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
            return View(await _context.Brands.OrderByDescending(x => x.BrandId).ToListAsync());
        }
        catch (Exception ex)
        {
            await LogErrorAsync(ex);
            TempData["Error"] = "Something went wrong while loading brands.";
            return View(new List<Brand>());
        }
    }

    /// <summary>
    /// Handles the Create request.
    /// </summary>
    public IActionResult Create()
    {
        return View(new BrandViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    /// <summary>
    /// Handles the Create request.
    /// </summary>
    public async Task<IActionResult> Create(BrandViewModel model)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Please fix validation errors.";
                return View(model);
            }

            var brandName = model.BrandName.Trim();

            var exists = await _context.Brands.AnyAsync(x => x.BrandName.ToLower() == brandName.ToLower());
            if (exists)
            {
                TempData["Error"] = "Brand already exists.";
                return View(model);
            }

            _context.Brands.Add(new Brand
            {
                BrandName = brandName,
                Description = model.Description,
                IsActive = model.IsActive
            });

            await _context.SaveChangesAsync();

            TempData["Success"] = "Brand added successfully.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            await LogErrorAsync(ex);
            TempData["Error"] = "Something went wrong while saving brand.";
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
            var brand = await _context.Brands.FindAsync(id);
            if (brand == null)
            {
                TempData["Error"] = "Brand not found.";
                return RedirectToAction(nameof(Index));
            }

            return View(new BrandViewModel
            {
                BrandId = brand.BrandId,
                BrandName = brand.BrandName,
                Description = brand.Description,
                IsActive = brand.IsActive
            });
        }
        catch (Exception ex)
        {
            await LogErrorAsync(ex);
            TempData["Error"] = "Something went wrong while loading brand.";
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    /// <summary>
    /// Handles the Edit request.
    /// </summary>
    public async Task<IActionResult> Edit(BrandViewModel model)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Please fix validation errors.";
                return View(model);
            }

            var brand = await _context.Brands.FindAsync(model.BrandId);
            if (brand == null)
            {
                TempData["Error"] = "Brand not found.";
                return RedirectToAction(nameof(Index));
            }

            var brandName = model.BrandName.Trim();

            var exists = await _context.Brands.AnyAsync(x => x.BrandId != model.BrandId && x.BrandName.ToLower() == brandName.ToLower());
            if (exists)
            {
                TempData["Error"] = "Brand already exists.";
                return View(model);
            }

            brand.BrandName = brandName;
            brand.Description = model.Description;
            brand.IsActive = model.IsActive;

            await _context.SaveChangesAsync();

            TempData["Success"] = "Brand updated successfully.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            await LogErrorAsync(ex);
            TempData["Error"] = "Something went wrong while updating brand.";
            return View(model);
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
            var brand = await _context.Brands.FindAsync(id);
            if (brand == null)
            {
                TempData["Error"] = "Brand not found.";
                return RedirectToAction(nameof(Index));
            }

            var isUsed = await _context.Categories.AnyAsync(x => x.BrandId == id)
                         || await _context.Products.AnyAsync(x => x.BrandId == id);

            if (isUsed)
            {
                TempData["Error"] = "This brand is used in categories or products. Please inactive it instead of deleting.";
                return RedirectToAction(nameof(Index));
            }

            _context.Brands.Remove(brand);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Brand deleted successfully.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            await LogErrorAsync(ex);
            TempData["Error"] = "Something went wrong while deleting brand.";
            return RedirectToAction(nameof(Index));
        }
    }
}