using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SmartBarcodePOS_Pro.Data;
using SmartBarcodePOS_Pro.Models;
using SmartBarcodePOS_Pro.Services;
using SmartBarcodePOS_Pro.ViewModels;

namespace SmartBarcodePOS_Pro.Controllers;

[Authorize(Roles = "Admin,SubAdmin")]
public class CategoriesController : BaseController
{
    private readonly ICategoryService _categoryService;
    private readonly ApplicationDbContext _context;

    public CategoriesController(ICategoryService categoryService, ApplicationDbContext context, IErrorLogService errorLogService) : base(errorLogService)
    {
        _categoryService = categoryService;
        _context = context;
    }

    /// <summary>
    /// Handles the Index request.
    /// </summary>
    public async Task<IActionResult> Index()
    {
        try
        {
            var categories = await _categoryService.GetAllAsync();
            return View(categories);
        }
        catch (Exception ex)
        {
            await LogErrorAsync(ex);
            TempData["Error"] = "Something went wrong while loading categories.";
            return View(new List<Category>());
        }
    }

    /// <summary>
    /// Handles the Create request.
    /// </summary>
    public async Task<IActionResult> Create()
    {
        try
        {
            return View(new CategoryViewModel { IsActive = true, Brands = await GetBrandsAsync() });
        }
        catch (Exception ex)
        {
            await LogErrorAsync(ex);
            TempData["Error"] = "Something went wrong while opening category form.";
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    /// <summary>
    /// Handles the Create request.
    /// </summary>
    public async Task<IActionResult> Create(CategoryViewModel model)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                model.Brands = await GetBrandsAsync();
                TempData["Error"] = "Please fix validation errors.";
                return View(model);
            }

            var result = await _categoryService.CreateAsync(model);
            TempData[result.Success ? "Success" : "Error"] = result.Message;

            if (!result.Success)
            {
                model.Brands = await GetBrandsAsync();
                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            await LogErrorAsync(ex);
            model.Brands = await GetBrandsAsync();
            TempData["Error"] = "Something went wrong while saving category.";
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
            var category = await _categoryService.GetByIdAsync(id);

            if (category == null)
            {
                TempData["Error"] = "Category not found.";
                return RedirectToAction(nameof(Index));
            }

            var model = new CategoryViewModel
            {
                CategoryId = category.CategoryId,
                CategoryName = category.CategoryName,
                BrandId = category.BrandId,
                IsActive = category.IsActive,
                Brands = await GetBrandsAsync()
            };

            return View(model);
        }
        catch (Exception ex)
        {
            await LogErrorAsync(ex);
            TempData["Error"] = "Something went wrong while loading category.";
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    /// <summary>
    /// Handles the Edit request.
    /// </summary>
    public async Task<IActionResult> Edit(CategoryViewModel model)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                model.Brands = await GetBrandsAsync();
                TempData["Error"] = "Please fix validation errors.";
                return View(model);
            }

            var result = await _categoryService.UpdateAsync(model);
            TempData[result.Success ? "Success" : "Error"] = result.Message;

            if (!result.Success)
            {
                model.Brands = await GetBrandsAsync();
                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            await LogErrorAsync(ex);
            model.Brands = await GetBrandsAsync();
            TempData["Error"] = "Something went wrong while updating category.";
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
            var result = await _categoryService.DeleteAsync(id);
            TempData[result.Success ? "Success" : "Error"] = result.Message;
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            await LogErrorAsync(ex);
            TempData["Error"] = "Something went wrong while deleting category.";
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    /// <summary>
    /// Handles the ToggleStatus request.
    /// </summary>
    public async Task<IActionResult> ToggleStatus(int id)
    {
        try
        {
            var result = await _categoryService.ToggleStatusAsync(id);
            TempData[result.Success ? "Success" : "Error"] = result.Message;
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            await LogErrorAsync(ex);
            TempData["Error"] = "Something went wrong while updating category status.";
            return RedirectToAction(nameof(Index));
        }
    }

    /// <summary>
    /// Handles the GetBrandsAsync request.
    /// </summary>
    private async Task<List<SelectListItem>> GetBrandsAsync()
    {
        return await _context.Brands
            .Where(x => x.IsActive)
            .OrderBy(x => x.BrandName)
            .Select(x => new SelectListItem
            {
                Value = x.BrandId.ToString(),
                Text = x.BrandName
            })
            .ToListAsync();
    }
}