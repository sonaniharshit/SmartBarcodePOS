using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartBarcodePOS_Pro.Services;
using SmartBarcodePOS_Pro.ViewModels;

namespace SmartBarcodePOS_Pro.Controllers;

[Authorize(Roles = "Admin,SubAdmin")]
public class SettingsController : BaseController
{
    private readonly ISettingService _settingService;

    public SettingsController(ISettingService settingService, IErrorLogService errorLogService) : base(errorLogService)
    {
        _settingService = settingService;
    }

    /// <summary>
    /// Handles the Index request.
    /// </summary>
    public async Task<IActionResult> Index()
    {
        try
        {
            return View(await _settingService.GetBusinessSettingsAsync());
        }
        catch (Exception ex)
        {
            await LogErrorAsync(ex);
            TempData["Error"] = "Something went wrong while loading settings.";
            return View(new AppSettingViewModel());
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    /// <summary>
    /// Handles the Index request.
    /// </summary>
    public async Task<IActionResult> Index(AppSettingViewModel model)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Please fix validation errors.";
                return View(model);
            }

            await _settingService.SaveBusinessSettingsAsync(model);

            TempData["Success"] = "Business settings updated successfully.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            await LogErrorAsync(ex);
            TempData["Error"] = "Something went wrong while saving business settings.";
            return View(model);
        }
    }
}