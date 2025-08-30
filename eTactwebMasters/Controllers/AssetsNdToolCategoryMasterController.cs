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

public class AssetsNdToolCategoryMasterController : Controller
{
    private readonly IDataLogic _IDataLogic;
    public IAssetsNdToolCategoryMaster _IAssetsNdToolCategoryMaster { get; }
    private readonly ILogger<AssetsNdToolCategoryMasterController> _logger;
    private readonly IConfiguration iconfiguration;
    public IWebHostEnvironment _IWebHostEnvironment { get; }

    public AssetsNdToolCategoryMasterController(ILogger<AssetsNdToolCategoryMasterController> logger, IDataLogic iDataLogic, IAssetsNdToolCategoryMaster iAssetsNdToolCategoryMaster, EncryptDecrypt encryptDecrypt, IWebHostEnvironment iWebHostEnvironment, IConfiguration iconfiguration)
    {
        _logger = logger;
        _IDataLogic = iDataLogic;
        _IAssetsNdToolCategoryMaster = iAssetsNdToolCategoryMaster;
        _IWebHostEnvironment = iWebHostEnvironment;
        this.iconfiguration = iconfiguration;
    }

    [HttpGet]
    public async Task<IActionResult> AssetsNdToolCategoryMaster(long id = 0, string categoryName = "", string mode = "I")
    {
        AssetsNdToolCategoryMasterModel model;

        if (id == 0)
        {
            // INSERT → get a new generated Id
            model = await _IAssetsNdToolCategoryMaster.GetNewEntryId();
            model.Mode = "I"; // Insert mode
        }
        else
        {
            // EDIT / VIEW → fetch existing record
            model = await _IAssetsNdToolCategoryMaster.ViewByIdAsync(id, categoryName);
            model.Mode = mode; // "U" = Update, "V" = View
        }

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> AssetsNdToolCategoryMasterDashboard()
    {
        var data = await _IAssetsNdToolCategoryMaster.GetDashboardAsync();
        return View(data); // Will render AssetsNdToolCategoryMasterDashboard.cshtml
    }


    [HttpPost]
    public async Task<IActionResult> AssetsNdToolCategoryMaster(AssetsNdToolCategoryMasterModel model)
    {

        if (model.Mode == "V")
        {
            // Read-only → don’t save
            return RedirectToAction("AssetsNdToolCategoryMasterDashboard");
        }
        var response = await _IAssetsNdToolCategoryMaster.SaveAsync(model);

        if (response.StatusCode == HttpStatusCode.OK)
        { 
            TempData["Message"] = model.Mode == "U"
               ? "Updated successfully!"
               : "Saved successfully!";
        return RedirectToAction("AssetsNdToolCategoryMasterDashBoard");
        }
        ModelState.AddModelError("", response.StatusText);
        return View(model);
    }

    //public async Task<IActionResult> AssetsNdToolCategoryMaster(long id, string mode = "V")
    //{
    //    if (id == 0)
    //    {
    //        var newModel = await _IAssetsNdToolCategoryMaster.GetNewEntryId();
    //        newModel.Mode = "I";
    //        return View(newModel);
    //    }

    //    var model = await _IAssetsNdToolCategoryMaster.ViewByIdAsync(id);
    //    model.Mode = mode; // "U" = Update, "V" = View
    //    return View(model);
    //}

    [HttpPost]
    public async Task<IActionResult> Delete(long id, string categoryName)
    {
        var response = await _IAssetsNdToolCategoryMaster.DeleteAsync(id, categoryName);
        return Json(new { success = response.StatusCode == HttpStatusCode.OK, message = response.StatusText });
    }
}
