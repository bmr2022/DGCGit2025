using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;

namespace eTactWeb.Controllers
{
    public class ConsumptionReportController : Controller
    {
        private readonly IDataLogic _IDataLogic;
        public IConsumptionReport _IConsumptionReport { get; }

        private readonly ILogger<ConsumptionReportController> _logger;
        private readonly IConfiguration iconfiguration;
        private readonly IMemoryCache _MemoryCache;
        public IWebHostEnvironment _IWebHostEnvironment { get; }
        public ConsumptionReportController(ILogger<ConsumptionReportController> logger, IDataLogic iDataLogic, IConsumptionReport iConsumptionReport, IMemoryCache iMemoryCache, EncryptDecrypt encryptDecrypt, IWebHostEnvironment iWebHostEnvironment, IConfiguration iconfiguration)
        {
            _logger = logger;
            _IDataLogic = iDataLogic;
            _IConsumptionReport = iConsumptionReport;
            _MemoryCache = iMemoryCache;
            _IWebHostEnvironment = iWebHostEnvironment;
            this.iconfiguration = iconfiguration;
        }

        [HttpGet("{controller}/Index")]
        public async Task<ActionResult> ConsumptionReport()
        {
            var model = new ConsumptionReportModel();
            model.ConsumptionReportGrid = new List<ConsumptionReportModel>();
            //model.YearCode= Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
            return View(model);
        }

        public async Task<JsonResult> FillFGItemName()
        {
            var JSON = await _IConsumptionReport.FillFGItemName();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillFGPartCode()
        {
            var JSON = await _IConsumptionReport.FillFGPartCode();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillRMItemName()
        {
            var JSON = await _IConsumptionReport.FillRMItemName();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillRMPartCode()
        {
            var JSON = await _IConsumptionReport.FillRMPartCode();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillStoreName()
        {
            var JSON = await _IConsumptionReport.FillStoreName();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillWorkCenterName()
        {
            var JSON = await _IConsumptionReport.FillWorkCenterName();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }

        public async Task<IActionResult> GetConsumptionDetailsData(string fromDate, string toDate, int WorkCenterid, string ReportType, int FGItemCode, int RMItemCode, int Storeid)
        {
            var model = new ConsumptionReportModel();
            model = await _IConsumptionReport
                .GetConsumptionDetailsData(fromDate, toDate, WorkCenterid, ReportType, FGItemCode, RMItemCode, Storeid);

            if (ReportType == "ProductionConsumptionReport(SUMMARY)")
            {
                return PartialView("_ConsumptionReportSummaryGrid", model);
            }
            if (ReportType == "ProductionConsumptionReport(DETAIL)")
            {
                return PartialView("_ConsumptionReportDetailGrid", model);
            }
            if (ReportType == "ProductionConsumptionReport(CONSOLIDATED)")
            {
                return PartialView("_ConsumptionReportCONSOLIDATEDGrid", model);
            }
            return null;
        }
    }
}
