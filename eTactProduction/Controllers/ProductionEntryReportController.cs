using eTactWeb.Data.Common;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using eTactWeb.DOM.Models;
using System.Globalization;

namespace eTactWeb.Controllers
{
    public class ProductionEntryReportController : Controller
    {
        private readonly IDataLogic _IDataLogic;
        public IProductionEntryReport _IProductionEntryReport { get; }
        private readonly ILogger<ProductionEntryReportController> _logger;
        private readonly IConfiguration iconfiguration;
        public IWebHostEnvironment _IWebHostEnvironment { get; }
        public ProductionEntryReportController(ILogger<ProductionEntryReportController> logger, IDataLogic iDataLogic, IProductionEntryReport IProductionEntryReport, EncryptDecrypt encryptDecrypt, IWebHostEnvironment iWebHostEnvironment, IConfiguration iconfiguration)
        {
            _logger = logger;
            _IDataLogic = iDataLogic;
            _IProductionEntryReport = IProductionEntryReport;
            _IWebHostEnvironment = iWebHostEnvironment;
            this.iconfiguration = iconfiguration;
        }
        [Route("{controller}/Index")]
        public IActionResult ProductionEntryReport()
        {
            var model = new ProductionEntryReportModel();
            model.ProductionEntryReportDetail = new List<ProductionEntryReportDetail>();
            return View(model);
        }
        public async Task<JsonResult> GetServerDate()
        {
            try
            {
                DateTime time = DateTime.Now;
                string format = "MMM ddd d HH:mm yyyy";
                string formattedDate = time.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                var dt = time.ToString(format);
                return Json(formattedDate);
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"HttpRequestException: {ex.Message}");
                return Json(new { error = "Failed to fetch server date and time: " + ex.Message });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected Exception: {ex.Message}");
                return Json(new { error = "An unexpected error occurred: " + ex.Message });
            }
        }
        public async Task<JsonResult> FillFGPartCode(string FromDate, string ToDate)
        {
            var JSON = await _IProductionEntryReport.FillFGPartCode(FromDate, ToDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillFGItemName(string FromDate, string ToDate)
        {
            var JSON = await _IProductionEntryReport.FillFGItemName(FromDate, ToDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillRMPartCode(string FromDate, string ToDate)
        {
            var JSON = await _IProductionEntryReport.FillRMPartCode(FromDate, ToDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillRMItemName(string FromDate, string ToDate)
        {
            var JSON = await _IProductionEntryReport.FillRMItemName(FromDate, ToDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillProdSlipNo(string FromDate, string ToDate)
        {
            var JSON = await _IProductionEntryReport.FillProdSlipNo(FromDate, ToDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillProdPlanNo(string FromDate, string ToDate)
        {
            var JSON = await _IProductionEntryReport.FillProdPlanNo(FromDate, ToDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillProdSchNo(string FromDate, string ToDate)
        {
            var JSON = await _IProductionEntryReport.FillProdSchNo(FromDate, ToDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillReqNo(string FromDate, string ToDate)
        {
            var JSON = await _IProductionEntryReport.FillReqNo(FromDate, ToDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillWorkCenter(string FromDate, string ToDate)
        {
            var JSON = await _IProductionEntryReport.FillWorkCenter(FromDate, ToDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillMachinName(string FromDate, string ToDate)
        {
            var JSON = await _IProductionEntryReport.FillMachinName(FromDate, ToDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillOperatorName(string FromDate, string ToDate)
        {
            var JSON = await _IProductionEntryReport.FillOperatorName(FromDate, ToDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillProcess(string FromDate, string ToDate)
        {
            var JSON = await _IProductionEntryReport.FillProcess(FromDate, ToDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<IActionResult> GetProductionEntryReport(string ReportType, string FromDate, string ToDate, string FGPartCode, string FGItemName, string RMPartCode, string RMItemName, string ProdSlipNo, string ProdPlanNo, string ProdSchNo, string ReqNo, string WorkCenter, string MachineName, string OperatorName, string Process)
        {
            var model = new ProductionEntryReportModel();
            string ReplaceZero(string value) => value == "0" ? "" : value;
            string ExtractBeforeArrow(string value) => value.Contains("--->") ? value.Split("--->")[0] : value;

            FGPartCode = ReplaceZero(FGPartCode);
            FGItemName = ExtractBeforeArrow(ReplaceZero(FGItemName));  // Extract value before `--->`
            RMPartCode = ReplaceZero(RMPartCode);
            RMItemName = ReplaceZero(RMItemName);
            ProdSlipNo = ReplaceZero(ProdSlipNo);
            ProdPlanNo = ReplaceZero(ProdPlanNo);
            ProdSchNo = ReplaceZero(ProdSchNo);
            ReqNo = ReplaceZero(ReqNo);
            WorkCenter = ReplaceZero(WorkCenter);
            MachineName = ReplaceZero(MachineName);
            OperatorName = ReplaceZero(OperatorName);
            Process = ReplaceZero(Process);

            model = await _IProductionEntryReport.GetProductionEntryReport(
                ReportType, FromDate, ToDate, FGPartCode, FGItemName, RMPartCode, RMItemName,
                ProdSlipNo, ProdPlanNo, ProdSchNo, ReqNo, WorkCenter, MachineName, OperatorName, Process
            );

            model.ReportType = ReportType;

            string serializedGrid = JsonConvert.SerializeObject(model.ProductionEntryReportDetail);
            HttpContext.Session.SetString("keyproductionentry", serializedGrid);
            return PartialView("_ProductionReportDetailGrid", model);
        }


        public IActionResult GetDataForPDF()
        {
            string modelJson = HttpContext.Session.GetString("keyproductionentry");
            List<ProductionEntryReportDetail> prodentrylist = new List<ProductionEntryReportDetail>();
            if (!string.IsNullOrEmpty(modelJson))
            {
                prodentrylist = JsonConvert.DeserializeObject<List<ProductionEntryReportDetail>>(modelJson);
            }



            return Json(prodentrylist);
        }
    }
}
