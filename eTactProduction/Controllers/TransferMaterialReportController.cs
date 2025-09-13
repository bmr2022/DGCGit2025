using eTactWeb.Data.Common;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using eTactWeb.DOM.Models;
using System.Globalization;
using FastReport.Web;

namespace eTactWeb.Controllers
{
    public class TransferMaterialReportController : Controller
    {
        private readonly IDataLogic _IDataLogic;
        public WebReport webReport;
        public ITransferMaterialReport _ITransferMaterialReport { get; set;}
        private readonly ILogger<TransferMaterialReportController> _logger;
        private readonly IConfiguration iconfiguration;
        private readonly ConnectionStringService _connectionStringService;
        public IWebHostEnvironment _IWebHostEnvironment { get; }
        public TransferMaterialReportController(ILogger<TransferMaterialReportController> logger, IDataLogic iDataLogic, ITransferMaterialReport ITransferMaterialReport, EncryptDecrypt encryptDecrypt, IWebHostEnvironment iWebHostEnvironment, IConfiguration iconfiguration, ConnectionStringService connectionStringService)
        {
            _logger = logger;
            _IDataLogic = iDataLogic;
            _ITransferMaterialReport = ITransferMaterialReport;
            _IWebHostEnvironment = iWebHostEnvironment;
            this.iconfiguration = iconfiguration;
            _connectionStringService = connectionStringService;
        }
        [Route("{controller}/Index")]
        public IActionResult TransferMaterialReport()
        {
            var model = new TransferMaterialReportModel();
            model.TransferMaterialReportDetail = new List<TransferMaterialReportDetail>();
            return View(model);
        }
        public IActionResult PrintReport(int EntryId = 0, int YearCode = 0, string PONO = "")
        {

            string my_connection_string;
            string contentRootPath = _IWebHostEnvironment.ContentRootPath;
            string webRootPath = _IWebHostEnvironment.WebRootPath;
            webReport = new WebReport();
            var ReportName = _ITransferMaterialReport.GetReportName();
            ViewBag.EntryId = EntryId;
            ViewBag.YearCode = YearCode;
            ViewBag.PONO = PONO;
            if (!string.Equals(ReportName.Result.Result.Rows[0].ItemArray[0], System.DBNull.Value))
            {
                webReport.Report.Load(webRootPath + "\\" + ReportName.Result.Result.Rows[0].ItemArray[0] + ".frx"); // from database
            }
            else
            {
                webReport.Report.Load(webRootPath + "\\PO.frx"); // default report

            }
            //webReport.Report.SetParameterValue("entryparam", EntryId);
            //webReport.Report.SetParameterValue("yearparam", YearCode);
            //webReport.Report.SetParameterValue("ponoparam", PONO);
            //my_connection_string = iconfiguration.GetConnectionString("eTactDB");
            //webReport.Report.SetParameterValue("MyParameter", my_connection_string);
            // return View(webReport);
            //my_connection_string = iconfiguration.GetConnectionString("eTactDB");
            my_connection_string = _connectionStringService.GetConnectionString();
            webReport.Report.Dictionary.Connections[0].ConnectionString = my_connection_string;
            webReport.Report.Dictionary.Connections[0].ConnectionStringExpression = "";
            webReport.Report.SetParameterValue("entryparam", EntryId);
            webReport.Report.SetParameterValue("yearparam", YearCode);
            webReport.Report.SetParameterValue("MyParameter", my_connection_string);
            webReport.Report.Refresh();
            return View(webReport);
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
        public async Task<JsonResult> FillFromWorkCenter(string FromDate, string ToDate)
        {
            var JSON = await _ITransferMaterialReport.FillFromWorkCenter(FromDate, ToDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillToWorkCenter(string FromDate, string ToDate)
        {
            var JSON = await _ITransferMaterialReport.FillToWorkCenter(FromDate, ToDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillToStore(string FromDate, string ToDate)
        {
            var JSON = await _ITransferMaterialReport.FillToStore(FromDate, ToDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillPartCode(string FromDate, string ToDate)
        {
            var JSON = await _ITransferMaterialReport.FillPartCode(FromDate, ToDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillItemName(string FromDate, string ToDate)
        {
            var JSON = await _ITransferMaterialReport.FillItemName(FromDate, ToDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillTransferSlipNo(string FromDate, string ToDate)
        {
            var JSON = await _ITransferMaterialReport.FillTransferSlipNo(FromDate, ToDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillProdSlipNo(string FromDate, string ToDate)
        {
            var JSON = await _ITransferMaterialReport.FillProdSlipNo(FromDate, ToDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillProdPlanNo(string FromDate, string ToDate)
        {
            var JSON = await _ITransferMaterialReport.FillProdPlanNo(FromDate, ToDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillProdSchNo(string FromDate, string ToDate)
        {
            var JSON = await _ITransferMaterialReport.FillProdSchNo(FromDate, ToDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillProcessName(string FromDate, string ToDate)
        {
            var JSON = await _ITransferMaterialReport.FillProcessName(FromDate, ToDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<IActionResult> GetTransferMaterialReport(string ReportType, string FromDate, string ToDate, string FromWorkCenter, string ToWorkCenter, string Tostore, string PartCode, string ItemName, string TransferSlipNo, string ProdSlipNo, string ProdPlanNo, string ProdSchNo, string ProcessName)
        {
            var model = new TransferMaterialReportModel();
            string ReplaceZero(string value) => value == "0" ? "" : value;
            string ExtractBeforeArrow(string value) => value.Contains("--->") ? value.Split("--->")[0] : value;

            FromWorkCenter = ReplaceZero(FromWorkCenter);
            ToWorkCenter = ExtractBeforeArrow(ReplaceZero(ToWorkCenter));  // Extract value before `--->`
            Tostore = ReplaceZero(Tostore);
            PartCode = ReplaceZero(PartCode);
            ItemName = ReplaceZero(ItemName);
            TransferSlipNo = ReplaceZero(TransferSlipNo);
            ProdSlipNo = ReplaceZero(ProdSlipNo);
            ProdPlanNo = ReplaceZero(ProdPlanNo);
            ProdSchNo = ReplaceZero(ProdSchNo);
            ProcessName = ReplaceZero(ProcessName);

            model = await _ITransferMaterialReport.GetTransferMaterialReport(
                ReportType, FromDate, ToDate, FromWorkCenter, ToWorkCenter, Tostore, PartCode,
                ItemName,TransferSlipNo,ProdSlipNo, ProdPlanNo, ProdSchNo, ProcessName
            );

            model.ReportType = ReportType;
            return PartialView("_TransferMaterialReportGrid", model);
        }
    }
}
