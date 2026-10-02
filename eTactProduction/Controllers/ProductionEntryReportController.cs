using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System.Data;
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
        public IActionResult ProductionEntryReport(string formKey)
        {
            ViewBag.formKey = formKey;
            var uniqueKey = Guid.NewGuid().ToString();
            ViewBag.uniqueKey = uniqueKey;

            var model = new ProductionEntryReportModel();
            model.ProductionEntryReportDetail = new List<ProductionEntryReportDetail>();
            return View(model);
        }
        public async Task<JsonResult> GetCompanyName()
        {
            var JSON = await _IProductionEntryReport.GetCompanyName();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
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

        public async Task<JsonResult> FillShiftName()
        {
            var JSON = await _IProductionEntryReport.FillShiftName();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillTranStore()
        {
            var JSON = await _IProductionEntryReport.filltranstore();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }

        public async Task<JsonResult> FillTranWorkCenter()
        {
            var JSON = await _IProductionEntryReport.filltranworkcenter();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<IActionResult> GetProductionEntryReport(
    string ReportType,
    string FromDate,
    string ToDate,
    string FGPartCode,
    string FGItemName,
    string RMPartCode,
    string RMItemName,
    string ProdSlipNo,
    string ProdPlanNo,
    string ProdSchNo,
    string ReqNo,
    string WorkCenter,
    string MachineName,
    string OperatorName,
    string Process,
    string ShiftName,
    int StoreID,
    int WCID,
    string FromSlipNo,
    string ToSlipNo,
    DateTime FromTime,
    DateTime ToTime,
    string SearchBox,
    string Flag = "True")
        {
            try
            {
                HttpContext.Session.Remove("keyproductionentry");

                var model = new ProductionEntryReportModel();

                string ReplaceZero(string value) =>
                    value == "0" ? "" : value;

                string ExtractBeforeArrow(string value) =>
                    !string.IsNullOrWhiteSpace(value) && value.Contains("--->")
                        ? value.Split("--->")[0]
                        : value;

                FGPartCode = ReplaceZero(FGPartCode);
                FGItemName = ExtractBeforeArrow(ReplaceZero(FGItemName));
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
                model.ReportType = ReportType;

                var result = await _IProductionEntryReport.GetProductionEntryReport(
                    ReportType,
                    FromDate,
                    ToDate,
                    FGPartCode,
                    FGItemName,
                    RMPartCode,
                    RMItemName,
                    ProdSlipNo,
                    ProdPlanNo,
                    ProdSchNo,
                    ReqNo,
                    WorkCenter,
                    MachineName,
                    OperatorName,
                    Process,
                    ShiftName,
                    StoreID,
                    WCID,
                    FromSlipNo,
                    ToSlipNo,
                    FromTime,
                    ToTime);

                if (result != null && result.Result is DataTable dt)
                {
                    model.Headers = dt.Columns
                        .Cast<DataColumn>()
                        .Select(c => new DashboardColumn
                        {
                            Title = c.ColumnName,
                            Field = c.ColumnName
                        })
                        .ToList();

                    model.Rows = dt.AsEnumerable()
                        .Select(r => dt.Columns
                            .Cast<DataColumn>()
                            .ToDictionary(
                                c => c.ColumnName,
                                c => r[c] == DBNull.Value ? null : r[c]
                            ))
                        .ToList();
                }

                string serializedGrid = JsonConvert.SerializeObject(model.Rows);
                HttpContext.Session.SetString("keyproductionentry", serializedGrid);

                return PartialView("_ProductionReportDetailGrid", model);
            }
            catch
            {
                throw;
            }
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
