using eTactWeb.Data.Common;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using static eTactWeb.Data.Common.CommonFunc;
using static eTactWeb.DOM.Models.Common;
using eTactWeb.DOM.Models;
using System.Net;
using System.Data;
using eTactWeb.Controllers;

public class CustomerDiscountCategoryController : Controller
{
    private readonly IDataLogic _IDataLogic;
    public ICustomerDiscountCategory _ICustomerDiscountCategory { get; }
    private readonly ILogger<CustomerDiscountCategoryController> _logger;
    private readonly IConfiguration iconfiguration;
    public IWebHostEnvironment _IWebHostEnvironment { get; }
    public CustomerDiscountCategoryController(ILogger<CustomerDiscountCategoryController> logger, IDataLogic iDataLogic, ICustomerDiscountCategory iCustomerDiscountCategory, EncryptDecrypt encryptDecrypt, IWebHostEnvironment iWebHostEnvironment, IConfiguration iconfiguration)
    {
        _logger = logger;
        _IDataLogic = iDataLogic;
        _ICustomerDiscountCategory = iCustomerDiscountCategory;
        _IWebHostEnvironment = iWebHostEnvironment;
        this.iconfiguration = iconfiguration;
    }

    public async Task<IActionResult> CustomerDiscountCategory(long id = 0, string mode = "I")
    {
        var model = new CustomerDiscountCategoryModel { Mode = mode };

        if (id > 0)
        {
            model = await _ICustomerDiscountCategory.GetById(id);
            model.Mode = "U";
        }

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> CustomerDiscountCategory(CustomerDiscountCategoryModel model)
    {
        // Step 1: Check duplicate if inserting
        if (model.Mode != "U")
        {
            var checkResult = await _ICustomerDiscountCategory.CheckCategoryExists(model.DiscountCategoryName);
            if (checkResult.Result is DataTable dt && dt.Rows.Count > 0)
            {
                TempData["Error"] = "Category name already exists!";
                return RedirectToAction(nameof(CustomerDiscountCategory));
            }
        }

        // Step 2: Save category
        var Result = await _ICustomerDiscountCategory.SaveCustomerDiscountCategory(model);

        if (Result.StatusText == "Success")
        {
            TempData["Message"] = model.Mode == "U" ? "Updated successfully!" : "Saved successfully!";
        }
        else
        {
            TempData["Error"] = Result.Result?.ToString() ?? "Error saving data!";
        }

        return RedirectToAction(nameof(CustomerDiscountCategoryDashboard));
    }




    [HttpGet]
    public async Task<IActionResult> CustomerDiscountCategoryDashboard()
    {
        var data = await _ICustomerDiscountCategory.GetDashboard();
        return View(data);
    }

    [HttpPost]
    public async Task<IActionResult> Delete(long id)
    {
        var result = await _ICustomerDiscountCategory.Delete(id);

        if (result.StatusText == "Success")
        {
            return Json(new { success = true, message = "Category deleted successfully!" });
        }
        else
        {
            return Json(new { success = false, message = result.Result?.ToString() ?? "Cannot delete this category." });
        }
    }
}

