using Microsoft.EntityFrameworkCore;
using SmartBarcodePOS_Pro.Data;
using SmartBarcodePOS_Pro.Models;
using SmartBarcodePOS_Pro.ViewModels;

namespace SmartBarcodePOS_Pro.Services;

public class CategoryService : ICategoryService
{
    private readonly ApplicationDbContext _context;

    public CategoryService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Category>> GetAllAsync()
    {
        return await _context.Categories
            .Include(x => x.Brand)
            .OrderByDescending(x => x.CategoryId)
            .ToListAsync();
    }

    public async Task<List<Category>> GetActiveAsync()
    {
        return await _context.Categories
            .Include(x => x.Brand)
            .Where(x => x.IsActive)
            .OrderBy(x => x.CategoryName)
            .ToListAsync();
    }

    public async Task<List<Category>> GetActiveByBrandAsync(int brandId)
    {
        return await _context.Categories
            .Where(x => x.IsActive && x.BrandId == brandId)
            .OrderBy(x => x.CategoryName)
            .ToListAsync();
    }

    public async Task<Category?> GetByIdAsync(int id)
    {
        return await _context.Categories
            .Include(x => x.Brand)
            .FirstOrDefaultAsync(x => x.CategoryId == id);
    }

    public async Task<(bool Success, string Message)> CreateAsync(CategoryViewModel model)
    {
        var name = model.CategoryName.Trim();

        var brandExists = await _context.Brands.AnyAsync(x => x.BrandId == model.BrandId && x.IsActive);
        if (!brandExists)
            return (false, "Please select a valid active brand.");

        var exists = await _context.Categories
            .AnyAsync(x => x.BrandId == model.BrandId && x.CategoryName.ToLower() == name.ToLower());

        if (exists)
            return (false, "Category already exists for this brand.");

        var category = new Category
        {
            CategoryName = name,
            BrandId = model.BrandId,
            IsActive = model.IsActive
        };

        _context.Categories.Add(category);
        await _context.SaveChangesAsync();

        return (true, "Category created successfully.");
    }

    public async Task<(bool Success, string Message)> UpdateAsync(CategoryViewModel model)
    {
        var category = await _context.Categories.FirstOrDefaultAsync(x => x.CategoryId == model.CategoryId);

        if (category == null)
            return (false, "Category not found.");

        var name = model.CategoryName.Trim();

        var brandExists = await _context.Brands.AnyAsync(x => x.BrandId == model.BrandId && x.IsActive);
        if (!brandExists)
            return (false, "Please select a valid active brand.");

        var exists = await _context.Categories
            .AnyAsync(x => x.CategoryId != model.CategoryId &&
                           x.BrandId == model.BrandId &&
                           x.CategoryName.ToLower() == name.ToLower());

        if (exists)
            return (false, "Category already exists for this brand.");

        category.CategoryName = name;
        category.BrandId = model.BrandId;
        category.IsActive = model.IsActive;

        await _context.SaveChangesAsync();

        return (true, "Category updated successfully.");
    }

    public async Task<(bool Success, string Message)> DeleteAsync(int id)
    {
        var category = await _context.Categories.FirstOrDefaultAsync(x => x.CategoryId == id);

        if (category == null)
            return (false, "Category not found.");

        var hasProducts = await _context.Products.AnyAsync(x => x.CategoryId == id);

        if (hasProducts)
            return (false, "This category is used in products. You can inactive it instead of deleting.");

        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();

        return (true, "Category deleted successfully.");
    }

    public async Task<(bool Success, string Message)> ToggleStatusAsync(int id)
    {
        var category = await _context.Categories.FirstOrDefaultAsync(x => x.CategoryId == id);

        if (category == null)
            return (false, "Category not found.");

        category.IsActive = !category.IsActive;
        await _context.SaveChangesAsync();

        return (true, category.IsActive ? "Category activated successfully." : "Category inactivated successfully.");
    }
}
