using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SmartBarcodePOS_Pro.Models;
using SmartBarcodePOS_Pro.Services;
using SmartBarcodePOS_Pro.ViewModels;

namespace SmartBarcodePOS_Pro.Controllers;

public class AccountController : BaseController
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IEmailService _emailService;

    public AccountController(
        SignInManager<ApplicationUser> signInManager,
        UserManager<ApplicationUser> userManager,
        IEmailService emailService,
        IErrorLogService errorLogService) : base(errorLogService)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _emailService = emailService;
    }

    [AllowAnonymous]
    /// <summary>
    /// Handles the Login request.
    /// </summary>
    public async Task<IActionResult> Login()
    {
        try
        {
            if (User.Identity?.IsAuthenticated == true)
                return RedirectToAction("Index", "Dashboard");

            return View();
        }
        catch (Exception ex)
        {
            await LogErrorAsync(ex);
            TempData["Error"] = "Something went wrong while opening login page.";
            return View();
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [AllowAnonymous]
    /// <summary>
    /// Handles the Login request.
    /// </summary>
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        try
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);

            if (result.Succeeded)
            {
                TempData["Success"] = "Login successful.";
                return RedirectToAction("Index", "Dashboard");
            }

            TempData["Error"] = "Invalid email or password.";
            return View(model);
        }
        catch (Exception ex)
        {
            await LogErrorAsync(ex);
            TempData["Error"] = "Something went wrong during login.";
            return View(model);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    /// <summary>
    /// Handles the Logout request.
    /// </summary>
    public async Task<IActionResult> Logout()
    {
        try
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(Login));
        }
        catch (Exception ex)
        {
            await LogErrorAsync(ex);
            TempData["Error"] = "Something went wrong during logout.";
            return RedirectToAction(nameof(Login));
        }
    }

    [AllowAnonymous]
    /// <summary>
    /// Handles the ForgotPassword request.
    /// </summary>
    public IActionResult ForgotPassword()
    {
        return View(new ForgotPasswordViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [AllowAnonymous]
    /// <summary>
    /// Handles the ForgotPassword request.
    /// </summary>
    public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
    {
        try
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);

            // Do not expose whether email exists.
            if (user == null)
            {
                TempData["Error"] = "User not found.";
                return RedirectToAction(nameof(Login));
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var resetUrl = Url.Action(
                nameof(ResetPassword),
                "Account",
                new { email = user.Email, token },
                Request.Scheme);

            var safeUrl = HtmlEncoder.Default.Encode(resetUrl ?? "");

            var body = $@"
                <div style='font-family:Arial,sans-serif;background:#f4f7fb;padding:25px;'>
                    <div style='max-width:560px;margin:auto;background:#fff;border-radius:16px;padding:25px;'>
                        <h2 style='margin-top:0;color:#0f172a;'>Reset Your Password</h2>
                        <p>Hello,</p>
                        <p>We received a request to reset your Smart Barcode POS password.</p>
                        <p>
                            <a href='{safeUrl}'
                               style='display:inline-block;background:#2563eb;color:#fff;padding:12px 18px;border-radius:10px;text-decoration:none;font-weight:bold;'>
                                Reset Password
                            </a>
                        </p>
                        <p style='color:#64748b;font-size:13px;'>If you did not request this, please ignore this email.</p>
                    </div>
                </div>";

            await _emailService.SendEmailAsync(user.Email!, "Reset Password - Smart Barcode POS", body);

            TempData["Success"] = "A password reset link has been sent to your registered email address.";
            return RedirectToAction(nameof(Login));
        }
        catch (Exception ex)
        {
            await LogErrorAsync(ex);
            TempData["Error"] = "Something went wrong while sending reset password email.";
            return View(model);
        }
    }

    [AllowAnonymous]
    /// <summary>
    /// Handles the ResetPassword request.
    /// </summary>
    public IActionResult ResetPassword(string email, string token)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(token))
        {
            TempData["Error"] = "Invalid password reset link.";
            return RedirectToAction(nameof(Login));
        }

        return View(new ResetPasswordViewModel
        {
            Email = email,
            Token = token
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [AllowAnonymous]
    /// <summary>
    /// Handles the ResetPassword request.
    /// </summary>
    public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
    {
        try
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                TempData["Error"] = "Invalid password reset request.";
                return RedirectToAction(nameof(Login));
            }

            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);

            if (result.Succeeded)
            {
                TempData["Success"] = "Password reset successfully. Please login.";
                return RedirectToAction(nameof(Login));
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);

            return View(model);
        }
        catch (Exception ex)
        {
            await LogErrorAsync(ex);
            TempData["Error"] = "Something went wrong while resetting password.";
            return View(model);
        }
    }

    [Authorize]
    /// <summary>
    /// Handles the ChangePassword request.
    /// </summary>
    public IActionResult ChangePassword()
    {
        return View(new ChangePasswordViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    /// <summary>
    /// Handles the ChangePassword request.
    /// </summary>
    public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
    {
        try
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                TempData["Error"] = "User not found.";
                return RedirectToAction("Index", "Dashboard");
            }

            var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);

            if (result.Succeeded)
            {
                await _signInManager.RefreshSignInAsync(user);
                TempData["Success"] = "Password changed successfully.";
                return RedirectToAction("Index", "Dashboard");
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);

            return View(model);
        }
        catch (Exception ex)
        {
            await LogErrorAsync(ex);
            TempData["Error"] = "Something went wrong while changing password.";
            return View(model);
        }
    }


    [Authorize]
    /// <summary>
    /// Handles the MyProfile request.
    /// </summary>
    public async Task<IActionResult> MyProfile()
    {
        try
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                TempData["Error"] = "User not found.";
                return RedirectToAction("Index", "Dashboard");
            }

            var roles = await _userManager.GetRolesAsync(user);

            return View(new MyProfileViewModel
            {
                FullName = user.FullName,
                Email = user.Email ?? string.Empty,
                PhoneNumber = user.PhoneNumber,
                RoleName = roles.FirstOrDefault() ?? string.Empty
            });
        }
        catch (Exception ex)
        {
            await LogErrorAsync(ex);
            TempData["Error"] = "Something went wrong while loading profile.";
            return RedirectToAction("Index", "Dashboard");
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    /// <summary>
    /// Handles the MyProfile request .
    /// </summary>
    public async Task<IActionResult> MyProfile(MyProfileViewModel model)
    {
        try
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                TempData["Error"] = "User not found.";
                return RedirectToAction("Index", "Dashboard");
            }

            var roles = await _userManager.GetRolesAsync(user);
            model.FullName = user.FullName;
            model.Email = user.Email ?? string.Empty;
            model.PhoneNumber = user.PhoneNumber;
            model.RoleName = roles.FirstOrDefault() ?? string.Empty;

            if (!ModelState.IsValid)
                return View(model);

            var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);

            if (result.Succeeded)
            {
                await _signInManager.RefreshSignInAsync(user);
                TempData["Success"] = "Password changed successfully.";
                return RedirectToAction(nameof(MyProfile));
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);

            return View(model);
        }
        catch (Exception ex)
        {
            await LogErrorAsync(ex);
            TempData["Error"] = "Something went wrong while changing password.";
            return View(model);
        }
    }

}