using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartBarcodePOS_Pro.Data;
using SmartBarcodePOS_Pro.Models;
using SmartBarcodePOS_Pro.Services;
using SmartBarcodePOS_Pro.ViewModels;

namespace SmartBarcodePOS_Pro.Controllers;

[Authorize(Roles = "Admin,SubAdmin")]
public class CustomersController : BaseController
{
    private readonly ApplicationDbContext _context;

    public CustomersController(ApplicationDbContext context, IErrorLogService errorLogService) : base(errorLogService)
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
            return View(await _context.Customers.OrderByDescending(x => x.CustomerId).ToListAsync());
        }
        catch (Exception ex)
        {
            await LogErrorAsync(ex);
            TempData["Error"] = "Something went wrong while loading customers.";
            return View(new List<Customer>());
        }
    }

    /// <summary>
    /// Handles the Create request.
    /// </summary>
    public async Task<IActionResult> Create()
    {
        try
        {
            return View(new CustomerViewModel());
        }
        catch (Exception ex)
        {
            await LogErrorAsync(ex);
            TempData["Error"] = "Something went wrong while opening customer form.";
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    /// <summary>
    /// Handles the Create request.
    /// </summary>
    public async Task<IActionResult> Create(CustomerViewModel model)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Please fix validation errors.";
                return View(model);
            }

            _context.Customers.Add(new Customer
            {
                CustomerName = model.CustomerName.Trim(),
                MobileNo = model.MobileNo.Trim(),
                Address = model.Address,
                LoyaltyPoints = model.LoyaltyPoints,
                IsActive = model.IsActive
            });

            await _context.SaveChangesAsync();

            TempData["Success"] = "Customer added successfully.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            await LogErrorAsync(ex);
            TempData["Error"] = "Something went wrong while saving customer.";
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
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                TempData["Error"] = "Customer not found.";
                return RedirectToAction(nameof(Index));
            }

            return View(new CustomerViewModel
            {
                CustomerId = customer.CustomerId,
                CustomerName = customer.CustomerName,
                MobileNo = customer.MobileNo,
                Address = customer.Address,
                LoyaltyPoints = customer.LoyaltyPoints,
                IsActive = customer.IsActive
            });
        }
        catch (Exception ex)
        {
            await LogErrorAsync(ex);
            TempData["Error"] = "Something went wrong while loading customer.";
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    /// <summary>
    /// Handles the Edit request.
    /// </summary>
    public async Task<IActionResult> Edit(CustomerViewModel model)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Please fix validation errors.";
                return View(model);
            }

            var customer = await _context.Customers.FindAsync(model.CustomerId);
            if (customer == null)
            {
                TempData["Error"] = "Customer not found.";
                return RedirectToAction(nameof(Index));
            }

            customer.CustomerName = model.CustomerName.Trim();
            customer.MobileNo = model.MobileNo.Trim();
            customer.Address = model.Address;
            customer.LoyaltyPoints = model.LoyaltyPoints;
            customer.IsActive = model.IsActive;

            await _context.SaveChangesAsync();

            TempData["Success"] = "Customer updated successfully.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            await LogErrorAsync(ex);
            TempData["Error"] = "Something went wrong while updating customer.";
            return View(model);
        }
    }
}