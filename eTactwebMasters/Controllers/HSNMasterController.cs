
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

public class HSNMasterController : Controller
{
    private readonly IDataLogic _IDataLogic;
    public IHSNMaster _IHSNMaster { get; }
    private readonly ILogger<HSNMasterController> _logger;
    private readonly IConfiguration iconfiguration;
    public IWebHostEnvironment _IWebHostEnvironment { get; }
    public HSNMasterController(ILogger<HSNMasterController> logger, IDataLogic iDataLogic, IHSNMaster iHSNMaster, EncryptDecrypt encryptDecrypt, IWebHostEnvironment iWebHostEnvironment, IConfiguration iconfiguration)
    {
        _logger = logger;
        _IDataLogic = iDataLogic;
        _IHSNMaster = iHSNMaster;
        _IWebHostEnvironment = iWebHostEnvironment;
        this.iconfiguration = iconfiguration;
    }

    [HttpGet]
    public async Task<IActionResult> HSNMaster(int id, string mode = "I")
    {
        var model = new HSNMasterModel();
       
        if (id > 0)
        {
            model = await _IHSNMaster.GetById(id);
            if (model == null) model = new HSNMasterModel();
        }

        // Only assign new ID for insert mode
        if (mode == "I")
        {

            var newId = await _IHSNMaster.GetNewEntryId();
            model.HSNEntryID = newId;
        }

        model.Mode = mode;
        model.ActualEntryByName = HttpContext.Session.GetString("EmpName");
        model.ActualEntryBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
        return View(model);
    }


    [HttpPost]
    public async Task<IActionResult> HSNMaster(HSNMasterModel model)
    {


        ResponseResult result = null;

        if (model.Mode == "U") // Update
        {
            result = await _IHSNMaster.Update(model);
        }
        else if (model.Mode == "I") // Insert
        {
            result = await _IHSNMaster.Insert(model);
        }
        else if (model.Mode == "V") // View only (no save)
        {
            return RedirectToAction(nameof(HSNMasterDashboard));
        }

        if (result != null && result.StatusText == "Success")
        {
            TempData["Message"] = model.Mode == "U" ? "Updated successfully!" : "Saved successfully!";
            return RedirectToAction(nameof(HSNMasterDashboard));
        }

        TempData["Error"] = "Error saving data!";
        return View(model);
    }



    [HttpGet]
    public async Task<IActionResult> HSNMasterDashboard()
    {
        var data = await _IHSNMaster.GetDashboard();
        Console.WriteLine($"Dashboard rows: {data.Count()}");
        return View(data);
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _IHSNMaster.Delete(id);

        if (result != null && result.StatusText == "Success")
            return Json(new { success = true, message = "HSN deleted successfully!" });
        else
            return Json(new { success = false, message = "Error deleting HSN record." });
    }

}


