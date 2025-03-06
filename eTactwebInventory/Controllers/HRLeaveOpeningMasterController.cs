using eTactWeb.Controllers;
using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;

namespace eTactwebInventory.Controllers
{
    public class HRLeaveOpeningMasterController : Controller
    {
        private readonly IDataLogic _IDataLogic;
        private readonly IHRLeaveOpeningMaster _IHRLeaveOpeningMaster;
        private readonly ILogger<HRLeaveOpeningMasterController> _logger;
        private readonly IMemoryCache _MemoryCache;
        private readonly IConfiguration iconfiguration;
        private readonly IIssueWithoutBom _IIssueWOBOM;
        public IWebHostEnvironment _IWebHostEnvironment { get; }

        public HRLeaveOpeningMasterController(ILogger<HRLeaveOpeningMasterController> logger, IDataLogic iDataLogic, IHRLeaveOpeningMaster iHRLeaveOpeningMaster, IMemoryCache iMemoryCache, EncryptDecrypt encryptDecrypt, IWebHostEnvironment iWebHostEnvironment, IConfiguration iconfiguration, IIssueWithoutBom IIssueWOBOM)
        {
            _logger = logger;
            _IDataLogic = iDataLogic;
            _IHRLeaveOpeningMaster = iHRLeaveOpeningMaster;
            _MemoryCache = iMemoryCache;
            _MemoryCache = iMemoryCache;
            _IWebHostEnvironment = iWebHostEnvironment;
            this.iconfiguration = iconfiguration;
            _IIssueWOBOM = IIssueWOBOM;
        }

        [Route("{controller}/Index")]
        public async Task<IActionResult> HRLeaveOpeningMaster()
        {
            ViewData["Title"] = "Job Work Details";
            TempData.Clear();
            _MemoryCache.Remove("KeyJobWorkIssue");
            _MemoryCache.Remove("JobWorkIssue");
            _MemoryCache.Remove("KeyTaxGrid");
            var MainModel = new HRLeaveOpeningMasterModel();
            MainModel.LeaveOpnYearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
            MainModel.ActualEntryBy = Convert.ToInt32(HttpContext.Session.GetString("UId"));
            //var TaxModel = new TaxModel();
            //MainModel = await BindModel(MainModel);
            //MainModel.FinFromDate = HttpContext.Session.GetString("FromDate");
            //MainModel.FinToDate = HttpContext.Session.GetString("ToDate");
            //MainModel.YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
            //MainModel.CC = HttpContext.Session.GetString("Branch");
            //MainModel.EnterByMachineName = Environment.MachineName;
            MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                SlidingExpiration = TimeSpan.FromMinutes(55),
                Size = 1024,
            };

            _MemoryCache.Set("KeyJobWorkIssue", MainModel, cacheEntryOptions);
            _MemoryCache.Set("HRLeaveOpeningMaster", MainModel, cacheEntryOptions);
            HttpContext.Session.SetString("HRLeaveOpeningMaster", JsonConvert.SerializeObject(MainModel));
            return View(MainModel);
        }

        public async Task<JsonResult> GetEmpCat()
        {
            var JSON = await _IHRLeaveOpeningMaster.GetEmpCat();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }

        public async Task<JsonResult> GetLeaveName()
        {
            var JSON = await _IHRLeaveOpeningMaster.GetLeaveName();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }

        public async Task<JsonResult> GetShiftName()
        {
            var JSON = await _IHRLeaveOpeningMaster.GetShiftName();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetEmpCode()
        {
            var JSON = await _IHRLeaveOpeningMaster.GetEmpCode();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
    }
}
