using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;

namespace eTactWeb.Controllers
{
    public class InventoryAgingReportController : Controller
    {
        private readonly IDataLogic _IDataLogic;
        public IInventoryAgingReport _IInventoryAgingReport { get; }
        private readonly ILogger<InventoryAgingReportController> _logger;
        private readonly IConfiguration iconfiguration;
        public IWebHostEnvironment _IWebHostEnvironment { get; }
        public InventoryAgingReportController(ILogger<InventoryAgingReportController> logger, IDataLogic iDataLogic, IInventoryAgingReport iInventoryAgingReport, EncryptDecrypt encryptDecrypt, IWebHostEnvironment iWebHostEnvironment, IConfiguration iconfiguration)
        {
            _logger = logger;
            _IDataLogic = iDataLogic;
            _IInventoryAgingReport = iInventoryAgingReport;
            _IWebHostEnvironment = iWebHostEnvironment;
            this.iconfiguration = iconfiguration;
        }
        [Route("{controller}/Index")]
        public async Task<ActionResult> InventoryAgingReport()
        {
            var model = new InventoryAgingReportModel();
            model.InventoryAgingReportGrid = new List<InventoryAgingReportModel>();

            return View(model);
        }
        public async Task<JsonResult> FillRMItemName(string FromDate, string ToDate, string CurrentDate, int Storeid)
        {
            var JSON = await _IInventoryAgingReport.FillRMItemName( FromDate,  ToDate,  CurrentDate,  Storeid);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillRMPartCode(string FromDate, string ToDate, string CurrentDate, int Storeid)
        {
            var JSON = await _IInventoryAgingReport.FillRMPartCode( FromDate,  ToDate,  CurrentDate,  Storeid);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillStoreName(string FromDate, string ToDate, string CurrentDate, int Storeid)
        {
            var JSON = await _IInventoryAgingReport.FillStoreName( FromDate,  ToDate,  CurrentDate,  Storeid);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillWorkCenterName(string FromDate, string ToDate, string CurrentDate, int Storeid)
        {
            var JSON = await _IInventoryAgingReport.FillWorkCenterName( FromDate,  ToDate,  CurrentDate,  Storeid);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<IActionResult> GetInventoryAgingReportDetailsData(string fromDate, string toDate,string CurrentDate, int WorkCenterid, string ReportType, int RMItemCode, int Storeid)
        {
            var model = new InventoryAgingReportModel();
            model = await _IInventoryAgingReport.GetInventoryAgingReportDetailsData(fromDate, toDate, CurrentDate, WorkCenterid, ReportType, RMItemCode, Storeid);
   

            if (ReportType == "Aging(SLAB WISE)")
            {
                return PartialView("_InventoryAgingReportSlabWiseGrid", model);
            }
            if (ReportType == "Aging(Detail)")
            {
                return PartialView("_InventoryAgingReportDetailGrid", model);
            }
            return null;
        }
    }
}
