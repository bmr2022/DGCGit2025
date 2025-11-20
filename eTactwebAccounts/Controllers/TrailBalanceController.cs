using ClosedXML.Excel;
using DocumentFormat.OpenXml.EMMA;
using eTactWeb.Controllers;
using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services;
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
        [Route("{controller}/TrailBalance")]
        [HttpGet]
        public async Task<ActionResult> TrailBalance()
        {
            HttpContext.Session.Remove("TrailBalanceData");
            var MainModel = new TrailBalanceModel();
            MainModel.TrailBalanceGrid = new List<TrailBalanceModel>();
            MainModel.FromDate = HttpContext.Session.GetString("FromDate");
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

            var sessionData = JsonConvert.SerializeObject(model);
            HttpContext.Session.SetString("TrailBalanceData", sessionData);

            if (ReportType== "TRAILSUMMARY")
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
        public IActionResult ExportTrailBalanceToExcel(string ReportType, string FromDate, string ToDate)
        {
            var BranchName = HttpContext.Session.GetString("Branch");
            var CompanyName = HttpContext.Session.GetString("CompanyName");

            // Get data from session
            var sessionData = HttpContext.Session.GetString("TrailBalanceData");
            if (string.IsNullOrEmpty(sessionData))
                return BadRequest("Trail balance data not found in session.");

            var result = Newtonsoft.Json.JsonConvert.DeserializeObject<TrailBalanceModel>(sessionData);
            string sheetName = ReportType == "TRAILSUMMARY" ? "TrailBalanceSummary" :
                   ReportType == "TRAILDETAIL" ? "TrailBalanceDetail" :
                   "TrailBalance";
            DataTable dt = new DataTable("TrailBalance");
            int sr = 1;

            if (ReportType == "TRAILSUMMARY")
            {
                // Summary Columns
                dt.Columns.Add("Sr#", typeof(int));
                dt.Columns.Add("Group Name", typeof(string));
                dt.Columns.Add("Open DR", typeof(decimal));
                dt.Columns.Add("Open CR", typeof(decimal));
                dt.Columns.Add("Total Opening", typeof(decimal));
                dt.Columns.Add("Curr Dr Amt", typeof(decimal));
                dt.Columns.Add("Curr Cr Amt", typeof(decimal));
                dt.Columns.Add("Net Current Amt", typeof(decimal));
                dt.Columns.Add("Net Amt", typeof(decimal));

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
                        row.NetAmt
                    );
                }
            }
            else if (ReportType == "TRAILDETAIL")
            {
                // Detail Columns matching your HTML table
                dt.Columns.Add("Sr#", typeof(int));
                dt.Columns.Add("Group Name", typeof(string));
                dt.Columns.Add("Parent Group Name", typeof(string));
                dt.Columns.Add("Account Name", typeof(string));
                dt.Columns.Add("Group Opn Dr", typeof(decimal));
                dt.Columns.Add("Group Opn Cr", typeof(decimal));
                dt.Columns.Add("Opn Dr", typeof(decimal));
                dt.Columns.Add("Opn Cr", typeof(decimal));
                dt.Columns.Add("Total Group Opening", typeof(decimal));
                dt.Columns.Add("Total Opening", typeof(decimal));
                dt.Columns.Add("Curr Dr", typeof(decimal));
                dt.Columns.Add("Curr Cr", typeof(decimal));
                dt.Columns.Add("Group Curr Dr", typeof(decimal));
                dt.Columns.Add("Group Curr Cr", typeof(decimal));
                dt.Columns.Add("Net Amt", typeof(decimal));
                dt.Columns.Add("Group Net Amt", typeof(decimal));
                dt.Columns.Add("Sub Group Parent", typeof(string));
                dt.Columns.Add("Under Group", typeof(string));

                foreach (var row in result.TrailBalanceGrid)
                {
                    dt.Rows.Add(
                        sr++,
                        row.TrailBalanceGroupName,
                        row.ParentGroupName,
                        row.AccountName,
                        row.GroupOpnDr,
                        row.GroupOpnCr,
                        row.OpnDr,
                        row.OpnCr,
                        row.TotalGroupOpening,
                        row.TotalOpening,
                        row.CurrDrAmt,
                        row.CurrCrAmt,
                        row.GroupCurrDrAmt,
                        row.GroupCurrCrAmt,
                        row.NetAmt,
                        row.GroupNetAmt,
                        row.SubGroupParent,
                        row.UnderGroup
                    );
                }
            }
            else
            {
                return BadRequest("Invalid report type.");
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

            string excelName = $"TrailBalance_{ReportType}_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
            return File(
                stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                excelName
            );
        }
    }
}
