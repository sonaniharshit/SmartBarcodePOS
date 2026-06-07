using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartBarcodePOS_Pro.Services;
using SmartBarcodePOS_Pro.ViewModels;

namespace SmartBarcodePOS_Pro.Controllers;

[Authorize(Roles = "Admin,SubAdmin,Cashier")]
public class BillingController : BaseController
{
    private readonly IBillingService _billingService;

    public BillingController(IBillingService billingService, IErrorLogService errorLogService) : base(errorLogService)
    {
        _billingService = billingService;
    }

    /// <summary>
    /// Handles the Index request .
    /// </summary>
    public async Task<IActionResult> Index()
    {
        try
        {
            return View();
        }
        catch (Exception ex)
        {
            await LogErrorAsync(ex);
            TempData["Error"] = "Something went wrong while opening POS billing.";
            return RedirectToAction("Index", "Dashboard");
        }
    }

    [HttpGet]
    /// <summary>
    /// Handles the GetProductBySku request.
    /// </summary>
    public async Task<IActionResult> GetProductBySku(string sku)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(sku))
                return BadRequest(new { success = false, message = "SKU is required." });

            var product = await _billingService.GetProductBySkuAsync(sku.Trim());
            if (product == null)
                return NotFound(new { success = false, message = "Product not found." });

            if (product.StockQty <= 0)
                return BadRequest(new { success = false, message = "Product is out of stock." });

            return Ok(new { success = true, data = product });
        }
        catch (Exception ex)
        {
            await LogErrorAsync(ex);
            return BadRequest(new { success = false, message = "Something went wrong. Error has been logged." });
        }
    }

    [HttpPost]
    /// <summary>
    /// Handles the GenerateBill request.
    /// </summary>
    public async Task<IActionResult> GenerateBill([FromBody] GenerateBillViewModel model)
    {
        try
        {
            var result = await _billingService.GenerateBillAsync(model);
            if (!result.Success)
                return BadRequest(new { success = false, message = result.Message });

            return Ok(new
            {
                success = true,
                message = result.Message,
                billId = result.BillId,
                billNo = result.BillNo,
                printUrl = Url.Action("Print", "Bills", new { id = result.BillId })
            });
        }
        catch (Exception ex)
        {
            await LogErrorAsync(ex);
            return BadRequest(new { success = false, message = "Something went wrong while generating bill. Error has been logged." });
        }
    }
}