using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;

namespace eTactWeb.Controllers
{
    public class TracebilityReportController : Controller
    {
        private readonly IDataLogic _IDataLogic;
        public ITracebilityReport _IMISTracebilityReport { get; }
        private readonly ILogger<TracebilityReportController> _logger;
        private readonly IConfiguration iconfiguration;
        private readonly IMemoryCache _MemoryCache;
        public IWebHostEnvironment _IWebHostEnvironment { get; }
        public TracebilityReportController(ILogger<TracebilityReportController> logger, IDataLogic iDataLogic, ITracebilityReport IMISTracebilityReport, EncryptDecrypt encryptDecrypt, IWebHostEnvironment iWebHostEnvironment, IConfiguration iconfiguration, IMemoryCache iMemoryCache)
        {
            _logger = logger;
            _IDataLogic = iDataLogic;
            _IMISTracebilityReport = IMISTracebilityReport;
            _IWebHostEnvironment = iWebHostEnvironment;
            this.iconfiguration = iconfiguration;
            _MemoryCache = iMemoryCache;
        }
        [Route("{controller}/Index")]
        public async Task<ActionResult> TracebilityReport()
        {
            var model = new TracebilityReportModel();
            model.FromDate = HttpContext.Session.GetString("FromDate");
            return View(model);
        }
        public async Task<IActionResult> GetTracebilityReportData(string FromDate, string ToDate, string SaleBillNo)
        {
            var model = await _IMISTracebilityReport.GetTracebilityReportData(FromDate, ToDate, SaleBillNo);

            // Optional: cache raw dataset for export later
            _MemoryCache.Set("KeyMISTraceabilityReport", model, new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                SlidingExpiration = TimeSpan.FromMinutes(55)
            });

            // Return PartialView based on report type (if required)
            return PartialView("_TraceabilityReportGrid", model);
        }
        [HttpPost]
        public IActionResult ExportTraceabilityGrid(string grid)   // grid = upper or lower
        {
            var model = _MemoryCache.Get<TracebilityReportModel>("KeyMISTraceabilityReport");

            if (model == null)
                return BadRequest("Traceability data not found. Please run the report again.");

            DataTable dt = null;
            string sheetName = "";

            if (grid == "upper")
            {
                dt = model.HeaderTable;
                sheetName = "Traceability_Header";
            }
            else if (grid == "lower")
            {
                dt = model.DetailTable;
                sheetName = "Traceability_Detail";
            }
            else
            {
                return BadRequest("Invalid grid type.");
            }

            if (dt == null || dt.Rows.Count == 0)
                return BadRequest("No data found for export.");

            using (var package = new OfficeOpenXml.ExcelPackage())
            {
                var ws = package.Workbook.Worksheets.Add(sheetName);
                ws.Cells["A1"].LoadFromDataTable(dt, true);
                ws.Cells.AutoFitColumns();

                var excelBytes = package.GetAsByteArray();

                Response.Headers.Add("Content-Disposition", $"attachment; filename=\"{sheetName}.xlsx\"");

                return File(
                    excelBytes,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
                );
            }
        }
        public async Task<JsonResult> FillSaleBillNoList(string FromDate, string ToDate)
        {
            var JSON = await _IMISTracebilityReport.FillSaleBillNoList(FromDate, ToDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
    }
}
