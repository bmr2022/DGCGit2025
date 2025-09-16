using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Data;

namespace eTactwebAccounts.Controllers
{
    public class ProfitAndLossController : Controller
    {
        private readonly IDataLogic _IDataLogic;
        public IProfitAndLoss _IProfitAndLoss { get; }
        private readonly ILogger<ProfitAndLossController> _logger;
        private readonly IConfiguration iconfiguration;
        public IWebHostEnvironment _IWebHostEnvironment { get; }
        public ProfitAndLossController(ILogger<ProfitAndLossController> logger, IDataLogic iDataLogic, IProfitAndLoss IProfitAndLoss, EncryptDecrypt encryptDecrypt, IWebHostEnvironment iWebHostEnvironment, IConfiguration iconfiguration)
        {
            _logger = logger;
            _IDataLogic = iDataLogic;
            _IProfitAndLoss = IProfitAndLoss;
            _IWebHostEnvironment = iWebHostEnvironment;
            this.iconfiguration = iconfiguration;
        }
        [Route("{controller}/Index")]
        [HttpGet]
        public async Task<ActionResult> ProfitAndLoss()
        {
            var MainModel = new ProfitAndLossModel();
            return View(MainModel); // Pass the model with old data to the view
        }
        public async Task<IActionResult> GetProfitAndLossData(string FromDate, string ToDate,string Flag, string ReportType, string ShowOpening, string ShowRecordWithZeroAmt)
        {
            var model = new ProfitAndLossModel();
            model.EntryByMachine = Environment.MachineName;
            model = await _IProfitAndLoss.GetProfitAndLossData(FromDate, ToDate, Flag, ReportType, ShowOpening, ShowRecordWithZeroAmt);

            var sessionData = JsonConvert.SerializeObject(model);
            HttpContext.Session.SetString("ProfitAndLossData", sessionData);

            if (ReportType == "SUMMARY")
            {
                return PartialView("_ProfitAndLossSummaryGrid", model);
            }
            if (ReportType == "DETAIL")
            {
                return PartialView("_ProfitAndLossDetailGrid", model);
            }
            return null;
        }

        [HttpGet]
        public IActionResult ExportProfitAndLossToExcel(string ReportType, string FromDate, string ToDate)
        {
            var BranchName = HttpContext.Session.GetString("Branch");
            var CompanyName = HttpContext.Session.GetString("CompanyName");

            // Get data from session
            var sessionData = HttpContext.Session.GetString("ProfitAndLossData");
            if (string.IsNullOrEmpty(sessionData))
                return BadRequest("Profit & Loss data not found in session.");

            var result = JsonConvert.DeserializeObject<ProfitAndLossModel>(sessionData);

            string sheetName = ReportType == "SUMMARY" ? "ProfitAndLossSummary" :
                               ReportType == "DETAIL" ? "ProfitAndLossDetail" :
                               "ProfitAndLoss";

            DataTable dt = new DataTable("ProfitAndLoss");

            int sr = 1;

            // Determine all dynamic column names from the first row
            var allColumns = new HashSet<string>();
            foreach (var row in result.ProfitAndLossGrid)
            {
                foreach (var key in row.DynamicColumns.Keys)
                {
                    allColumns.Add(key);
                }
            }

            // Add a Sr# column first
            dt.Columns.Add("Sr#", typeof(int));

            // Add dynamic columns
            foreach (var col in allColumns)
            {
                dt.Columns.Add(col, typeof(string)); // Use string to handle any value type dynamically
            }

            // Fill rows
            foreach (var row in result.ProfitAndLossGrid)
            {
                var dr = dt.NewRow();
                dr["Sr#"] = sr++;

                foreach (var col in allColumns)
                {
                    dr[col] = row.DynamicColumns.ContainsKey(col) ? row.DynamicColumns[col]?.ToString() : string.Empty;
                }

                dt.Rows.Add(dr);
            }

            // Generate Excel
            var stream = ExcelHelper.GenerateExcel(
                dt,
                sheetName,
                CompanyName,
                BranchName,
                FromDate,
                ToDate
            );

            string excelName = $"ProfitAndLoss_{ReportType}_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
            return File(
                stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                excelName
            );
        }
    }
}
