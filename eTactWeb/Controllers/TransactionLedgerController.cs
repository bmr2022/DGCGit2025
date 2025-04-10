using eTactWeb.Data.Common;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using eTactWeb.DOM.Models;

namespace eTactWeb.Controllers
{
    public class TransactionLedgerController : Controller
    {
        private readonly IDataLogic _IDataLogic;
        public ITransactionLedger _TransactionLedger { get; }

        private readonly ILogger<TransactionLedgerController> _logger;
        private readonly IConfiguration iconfiguration;
        public IWebHostEnvironment _IWebHostEnvironment { get; }
        public TransactionLedgerController(ILogger<TransactionLedgerController> logger, IDataLogic iDataLogic, ITransactionLedger iTransactionLedger, EncryptDecrypt encryptDecrypt, IWebHostEnvironment iWebHostEnvironment, IConfiguration iconfiguration)
        {
            _logger = logger;
            _IDataLogic = iDataLogic;
            _TransactionLedger = iTransactionLedger;
            _IWebHostEnvironment = iWebHostEnvironment;
            this.iconfiguration = iconfiguration;
        }
        [Route("{controller}/Index")]
        [HttpGet]
        public async Task<ActionResult> TransactionLedger()
        {
            var MainModel = new TransactionLedgerModel();
            MainModel.TransactionLedgerGrid = new List<TransactionLedgerModel>();
            MainModel.FromDate = HttpContext.Session.GetString("FromDate");
            MainModel.ToDate = HttpContext.Session.GetString("ToDate");
            //MainModel.YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));

            return View(MainModel); // Pass the model with old data to the view
        }
        public async Task<JsonResult> GetLedgerName()
        {
            var JSON = await _TransactionLedger.GetLedgerName();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<IActionResult> GetDetailsData(string FromDate, string ToDate, int AccountCode, string ReportType)
        {
            var model = new TransactionLedgerModel();
            model = await _TransactionLedger.GetDetailsData(FromDate, ToDate, AccountCode,  ReportType);
            return PartialView("_TransactionLedgerGrid", model);

        } 
        public async Task<IActionResult> GetTransactionLedgerMonthlySummaryDetailsData(string FromentryDate, string ToEntryDate, int AccountCode)
        {
            var model = new TransactionLedgerModel();
            model = await _TransactionLedger.GetTransactionLedgerMonthlySummaryDetailsData(FromentryDate, ToEntryDate, AccountCode);
            return PartialView("_TransactionLedgerMonthlySummaryGrid", model);

        }
    }
}
