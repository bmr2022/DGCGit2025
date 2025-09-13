using ClosedXML.Excel;
using DocumentFormat.OpenXml.EMMA;
using eTactWeb.Controllers;
using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System.Data;

namespace eTactwebAccounts.Controllers
{
    public class TrailBalanceController : Controller
    {
        private readonly IDataLogic _IDataLogic;
        public ITrailBalance _ITrailBalance { get; }
        private readonly ILogger<TrailBalanceController> _logger;
        private readonly IConfiguration iconfiguration;
        public IWebHostEnvironment _IWebHostEnvironment { get; }
        public TrailBalanceController(ILogger<TrailBalanceController> logger, IDataLogic iDataLogic, ITrailBalance iTrailBalance, EncryptDecrypt encryptDecrypt, IWebHostEnvironment iWebHostEnvironment, IConfiguration iconfiguration)
        {
            _logger = logger;
            _IDataLogic = iDataLogic;
            _ITrailBalance = iTrailBalance;
            _IWebHostEnvironment = iWebHostEnvironment;
            this.iconfiguration = iconfiguration;
        }
        [Route("{controller}/Index")]
        [HttpGet]
        public async Task<ActionResult> TrailBalance()
        {
            var MainModel = new TrailBalanceModel();
            MainModel.TrailBalanceGrid = new List<TrailBalanceModel>();
            //MainModel.FromDate = HttpContext.Session.GetString("FromDate");
            //MainModel.ToDate = HttpContext.Session.GetString("ToDate");
            MainModel.EntryByMachine = Environment.MachineName;
            //MainModel.YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));

            return View(MainModel); // Pass the model with old data to the view
        }
        public async Task<IActionResult> GetTrailBalanceDetailsData(string FromDate, string ToDate, int? TrailBalanceGroupCode, int? ParentAccountCode, string ReportType)
        {
            var model = new TrailBalanceModel();
            model.EntryByMachine = Environment.MachineName;
            model = await _ITrailBalance.GetTrailBalanceDetailsData(FromDate, ToDate, TrailBalanceGroupCode, ParentAccountCode, ReportType);
            if(ReportType== "TRAILSUMMARY")
            {
                return PartialView("_TrailBalanceSummaryGrid", model);
            } 
            if(ReportType== "TRAILDETAIL")
            {
                return PartialView("_TrailBalanceDetailGrid", model);
            }
            return null;
        }
        public async Task<JsonResult> FillGroupList(string FromDate, string ToDate)
        {
            var JSON = await _ITrailBalance.FillGroupList(FromDate, ToDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillParentGroupList(string FromDate, string ToDate,int? GroupCode)
        {
            var JSON = await _ITrailBalance.FillParentGroupList(FromDate, ToDate,GroupCode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillAccountList(string FromDate, string ToDate, int? GroupCode,int? ParentGroupCode)
        {
            var JSON = await _ITrailBalance.FillAccountList(FromDate, ToDate, GroupCode,ParentGroupCode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        [HttpGet]
        public async Task<IActionResult> ExportTrailBalanceToExcel(
    string FromDate,
    string ToDate,
    int? TrailBalanceGroupCode,
    int? ParentAccountCode,
    string ReportType)
        {
            var BranchName = HttpContext.Session.GetString("Branch");
            var CompanyName = HttpContext.Session.GetString("CompanyName");

            // 1️⃣ Get data
            var result = await _ITrailBalance.GetTrailBalanceDetailsData(
                FromDate, ToDate, TrailBalanceGroupCode, ParentAccountCode, ReportType);

            // 2️⃣ Convert data to DataTable
            DataTable dt = new DataTable("TrailBalance");
            dt.Columns.Add("Sr#", typeof(int));
            dt.Columns.Add("Group Name", typeof(string));
            dt.Columns.Add("Open DR", typeof(decimal));
            dt.Columns.Add("Open CR", typeof(decimal));
            dt.Columns.Add("Total Opening", typeof(decimal));
            dt.Columns.Add("Curr DRAmt", typeof(decimal));
            dt.Columns.Add("Curr CRAmt", typeof(decimal));
            dt.Columns.Add("Net Current Amt", typeof(decimal));
            dt.Columns.Add("Net Amt", typeof(decimal));
            dt.Columns.Add("Curr DrTotal", typeof(decimal));
            dt.Columns.Add("Curr CrTotal", typeof(decimal));

            int sr = 1;
            foreach (var row in result.TrailBalanceGrid)
            {
                dt.Rows.Add(
                    sr++,
                    row.TrailBalanceGroupName,
                    row.OpnDr,
                    row.OpnCr,
                    row.TotalOpening,
                    row.CurrDrAmt,
                    row.CurrCrAmt,
                    row.NetCurrentAmt,
                    row.NetAmt,
                    row.CurrDrTotal,
                    row.CurrCrTotal
                );
            }

            // 3️⃣ Create Excel file using ClosedXML
            using (var wb = new XLWorkbook())
            {
                var ws = wb.Worksheets.Add("TrailBalance");

                int totalColumns = dt.Columns.Count;

                // 🔹 Company + Branch
                ws.Cell(1, 1).Value = "Company: " + CompanyName + "    Branch: " + BranchName;
                ws.Range(1, 1, 1, totalColumns).Merge();
                ws.Row(1).Style.Fill.BackgroundColor = XLColor.FromHtml("#6c9dc6");
                ws.Row(1).Style.Font.FontColor = XLColor.White;
                ws.Row(1).Style.Font.Bold = true;
                ws.Row(1).Style.Font.FontSize = 14;
                ws.Row(1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                // 🔹 From / To Date
                ws.Cell(2, 1).Value = $"From Date: {FromDate}   To Date: {ToDate}";
                ws.Range(2, 1, 2, totalColumns).Merge();
                ws.Row(2).Style.Fill.BackgroundColor = XLColor.FromHtml("#6c9dc6");
                ws.Row(2).Style.Font.FontColor = XLColor.White;
                ws.Row(2).Style.Font.Bold = true;
                ws.Row(2).Style.Font.FontSize = 14;
                ws.Row(2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                // 🔹 Insert table headers + data (start row 4)
                ws.Cell(4, 1).InsertTable(dt, "TrailBalanceTable", true);

                // Style header row
                var headerRow = ws.Row(4);
                headerRow.Style.Fill.BackgroundColor = XLColor.FromHtml("#6c9dc6");
                headerRow.Style.Font.FontColor = XLColor.Black;
                headerRow.Style.Font.Bold = true;
                headerRow.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                // 4️⃣ Add Totals row
                int dataRowStart = 5; // first data row
                int dataRowEnd = dt.Rows.Count + 4;
                int totalRow = dataRowEnd + 1;

                ws.Cell(totalRow, 1).Value = "Total";
                ws.Range(totalRow, 1, totalRow, 2).Merge();
                ws.Cell(totalRow, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Cell(totalRow, 1).Style.Font.Bold = true;

                // Totals for numeric columns (3–11)
                for (int col = 3; col <= totalColumns; col++)
                {
                    string colLetter = ws.Cell(1, col).Address.ColumnLetter;
                    ws.Cell(totalRow, col).FormulaA1 = $"SUM({colLetter}{dataRowStart}:{colLetter}{dataRowEnd})";
                }

                // Style totals row
                ws.Range(totalRow, 1, totalRow, totalColumns).Style.Font.Bold = true;
                ws.Range(totalRow, 1, totalRow, totalColumns).Style.Fill.BackgroundColor = XLColor.FromHtml("#6c9dc6");
                ws.Range(totalRow, 1, totalRow, totalColumns).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                // 5️⃣ Auto-fit columns
                ws.Columns().AdjustToContents();

                // 6️⃣ Return Excel file
                using (var stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    stream.Position = 0;

                    string excelName = $"TrailBalance_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
                    return File(
                        stream.ToArray(),
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        excelName
                    );
                }
            }
        }
    }
}
