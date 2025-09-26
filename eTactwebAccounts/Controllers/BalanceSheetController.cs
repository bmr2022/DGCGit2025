using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace eTactwebAccounts.Controllers
{
    public class BalanceSheetController : Controller
    {
        private readonly IDataLogic _IDataLogic;
        public IBalanceSheet _IBalanceSheet { get; }
        private readonly ILogger<BalanceSheetController> _logger;
        private readonly IConfiguration iconfiguration;
        public IWebHostEnvironment _IWebHostEnvironment { get; }
        public BalanceSheetController(ILogger<BalanceSheetController> logger, IDataLogic iDataLogic, IBalanceSheet IBalanceSheet, EncryptDecrypt encryptDecrypt, IWebHostEnvironment iWebHostEnvironment, IConfiguration iconfiguration)
        {
            _logger = logger;
            _IDataLogic = iDataLogic;
            _IBalanceSheet = IBalanceSheet;
            _IWebHostEnvironment = iWebHostEnvironment;
            this.iconfiguration = iconfiguration;
        }
        [Route("{controller}/Index")]
        [HttpGet]
        public async Task<ActionResult> BalanceSheet()
        {
            var MainModel = new BalanceSheetModel();
            MainModel.FromDate = HttpContext.Session.GetString("FromDate");
            return View(MainModel); // Pass the model with old data to the view
        }
        public async Task<IActionResult> GetBalanceSheetData(string FromDate, string ToDate, string ReportType, int? BalParentAccountCode)
        {
            var model = new BalanceSheetModel();
            model.EntryByMachine = Environment.MachineName;
            model = await _IBalanceSheet.GetBalanceSheetData(FromDate, ToDate, ReportType, BalParentAccountCode);

            var sessionData = JsonConvert.SerializeObject(model);
            HttpContext.Session.SetString("BalanceSheetData", sessionData);
            return PartialView("_BalanceSheetGridData", model);
        }
        public async Task<JsonResult> GetLiabilitiesAndAssetsData(string FromDate, string ToDate)
        {
            var JSON = await _IBalanceSheet.GetLiabilitiesAndAssetsData(FromDate, ToDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetParentAccountData(string FromDate, string ToDate)
        {
            var JSON = await _IBalanceSheet.GetParentAccountData(FromDate, ToDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetAccountData(string FromDate, string ToDate)
        {
            var JSON = await _IBalanceSheet.GetAccountData(FromDate, ToDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
    }
}
