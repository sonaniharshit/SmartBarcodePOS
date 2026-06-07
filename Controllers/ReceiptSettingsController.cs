using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartBarcodePOS_Pro.Data;
using SmartBarcodePOS_Pro.Models;
using SmartBarcodePOS_Pro.Services;
using SmartBarcodePOS_Pro.ViewModels;

namespace SmartBarcodePOS_Pro.Controllers;

[Authorize(Roles = "Admin,SubAdmin")]
public class ReceiptSettingsController : BaseController
{
    private readonly ApplicationDbContext _context;

    public ReceiptSettingsController(ApplicationDbContext context, IErrorLogService errorLogService) : base(errorLogService)
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
            var setting = await GetOrCreateSettingAsync();

            var model = new ReceiptSettingViewModel
            {
                FooterMessage = setting.FooterMessage,
                PrintSize = setting.PrintSize
            };

            return View(model);
        }
        catch (Exception ex)
        {
            await LogErrorAsync(ex);
            TempData["Error"] = "Something went wrong while loading receipt settings.";
            return View(new ReceiptSettingViewModel());
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    /// <summary>
    /// Handles the Index request.
    /// </summary>
    public async Task<IActionResult> Index(ReceiptSettingViewModel model)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Please fix validation errors.";
                return View(model);
            }

            var setting = await GetOrCreateSettingAsync();

            setting.FooterMessage = model.FooterMessage.Trim();
            setting.PrintSize = model.PrintSize;

            await _context.SaveChangesAsync();

            TempData["Success"] = "Receipt settings saved successfully.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            await LogErrorAsync(ex);
            TempData["Error"] = "Something went wrong while saving receipt settings.";
            return View(model);
        }
    }

    /// <summary>
    /// Handles the GetOrCreateSettingAsync request.
    /// </summary>
    private async Task<AppSetting> GetOrCreateSettingAsync()
    {
        var setting = await _context.AppSettings.FirstOrDefaultAsync();

        if (setting == null)
        {
            setting = new AppSetting
            {
                ShopName = "Smart POS Store",
                Address = "Surat, Gujarat",
                Phone = "9876543210",
                CurrencySymbol = "₹",
                FooterMessage = "Thank you, visit again!",
                PrintSize = "80mm"
            };

            _context.AppSettings.Add(setting);
            await _context.SaveChangesAsync();
        }

        return setting;
    }
}