using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartBarcodePOS_Pro.Services;
using SmartBarcodePOS_Pro.ViewModels;

namespace SmartBarcodePOS_Pro.Controllers;

[Authorize(Roles = "Admin")]
public class UsersController : BaseController
{
    private readonly IUserManagementService _userManagementService;

    public UsersController(
        IUserManagementService userManagementService,
        IErrorLogService errorLogService) : base(errorLogService)
    {
        _userManagementService = userManagementService;
    }

    /// <summary>
    /// Handles the Index request.
    /// </summary>
    public async Task<IActionResult> Index()
    {
        try
        {
            return View(await _userManagementService.GetUsersAsync());
        }
        catch (Exception ex)
        {
            await LogErrorAsync(ex);
            TempData["Error"] = "Something went wrong while loading users.";
            return View(new List<UserListViewModel>());
        }
    }

    /// <summary>
    /// Handles the Create request.
    /// </summary>
    public IActionResult Create()
    {
        return View(_userManagementService.GetCreateModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    /// <summary>
    /// Handles the Create request.
    /// </summary>
    public async Task<IActionResult> Create(CreateUserViewModel model)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Please fix validation errors.";
                return View(model);
            }

            var (result, errors) = await _userManagementService.CreateUserAsync(model);

            AddServiceErrors(errors);

            if (!result.Success)
                return View(model);

            TempData["Success"] = result.Message;
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            await LogErrorAsync(ex);
            TempData["Error"] = "Something went wrong while creating user.";
            return View(model);
        }
    }

    /// <summary>
    /// Handles the Edit request.
    /// </summary>
    public async Task<IActionResult> Edit(string id)
    {
        try
        {
            var model = await _userManagementService.GetEditModelAsync(id);

            if (model == null)
            {
                TempData["Error"] = "User not found.";
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }
        catch (Exception ex)
        {
            await LogErrorAsync(ex);
            TempData["Error"] = "Something went wrong while loading user.";
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    /// <summary>
    /// Handles the Edit request.
    /// </summary>
    public async Task<IActionResult> Edit(EditUserViewModel model)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Please fix validation errors.";
                return View(model);
            }

            var (result, errors) = await _userManagementService.UpdateUserAsync(model);

            AddServiceErrors(errors);

            if (!result.Success)
            {
                if (!ModelState.Any(x => x.Value?.Errors.Count > 0))
                    TempData["Error"] = result.Message;

                return View(model);
            }

            TempData["Success"] = result.Message;
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            await LogErrorAsync(ex);
            TempData["Error"] = "Something went wrong while updating user.";
            return View(model);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    /// <summary>
    /// Handles the ToggleStatus request.
    /// </summary>
    public async Task<IActionResult> ToggleStatus(string id)
    {
        try
        {
            var result = await _userManagementService.ToggleStatusAsync(id);
            TempData[result.Success ? "Success" : "Error"] = result.Message;
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            await LogErrorAsync(ex);
            TempData["Error"] = "Something went wrong while updating user status.";
            return RedirectToAction(nameof(Index));
        }
    }

    /// <summary>
    /// Handles the ResetPassword request.
    /// </summary>
    public async Task<IActionResult> ResetPassword(string id)
    {
        try
        {
            var (model, userName, email) = await _userManagementService.GetResetPasswordModelAsync(id);

            if (model == null)
            {
                TempData["Error"] = "User not found.";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.UserName = userName;
            ViewBag.Email = email;

            return View(model);
        }
        catch (Exception ex)
        {
            await LogErrorAsync(ex);
            TempData["Error"] = "Something went wrong while opening reset password.";
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    /// <summary>
    /// Handles the ResetPassword request.
    /// </summary>
    public async Task<IActionResult> ResetPassword(AdminResetPasswordViewModel model)
    {
        try
        {
            if (!ModelState.IsValid)
                return View(model);

            var (result, errors, userName, email) = await _userManagementService.ResetPasswordAsync(model);

            AddServiceErrors(errors);

            if (!result.Success)
            {
                ViewBag.UserName = userName;
                ViewBag.Email = email;
                return View(model);
            }

            TempData["Success"] = result.Message;
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            await LogErrorAsync(ex);
            TempData["Error"] = "Something went wrong while resetting password.";
            return View(model);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    /// <summary>
    /// Handles the Delete request.
    /// </summary>
    public async Task<IActionResult> Delete(string id)
    {
        try
        {
            var result = await _userManagementService.DeleteUserAsync(id);
            TempData[result.Success ? "Success" : "Error"] = result.Message;
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            await LogErrorAsync(ex);
            TempData["Error"] = "Something went wrong while deleting user.";
            return RedirectToAction(nameof(Index));
        }
    }

    /// <summary>
    /// Handles the AddServiceErrors request.
    /// </summary>
    private void AddServiceErrors(Dictionary<string, List<string>> errors)
    {
        foreach (var item in errors)
        {
            foreach (var error in item.Value)
                ModelState.AddModelError(item.Key, error);
        }
    }
}