using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;

namespace eTactwebAccounts.Controllers
{
    public class AccGroupLedgerController : Controller
    {
        private readonly IDataLogic _IDataLogic;
        public IAccGroupLedger _IAccGroupLedger { get; }
        private readonly ILogger<AccGroupLedgerController> _logger;
        private readonly IConfiguration iconfiguration;
        public IWebHostEnvironment _IWebHostEnvironment { get; }
        public AccGroupLedgerController(ILogger<AccGroupLedgerController> logger, IDataLogic iDataLogic, IAccGroupLedger iAccGroupLedger, EncryptDecrypt encryptDecrypt, IWebHostEnvironment iWebHostEnvironment, IConfiguration iconfiguration)
        {
            _logger = logger;
            _IDataLogic = iDataLogic;
            _IAccGroupLedger = iAccGroupLedger;
            _IWebHostEnvironment = iWebHostEnvironment;
            this.iconfiguration = iconfiguration;
        }
        [Route("{controller}/Index")]
        [HttpGet]
        public async Task<ActionResult> AccGroupLedger()
        {
            var MainModel = new AccGroupLedgerModel();
            MainModel.AccGroupLedgerGrid = new List<AccGroupLedgerModel>();
            MainModel.FromDate = HttpContext.Session.GetString("FromDate");
            MainModel.ToDate = HttpContext.Session.GetString("ToDate");            
            return View(MainModel);
        }
        public async Task<JsonResult> FillGroupName(string FromDate, string ToDate)
        {
            var JSON = await _IAccGroupLedger.FillGroupName((FromDate),ToDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<IActionResult> GetGroupLedgerDetailsData(string FromDate, string ToDate, int GroupCode, string ReportType)
        {
            var model = new AccGroupLedgerModel();
            model = await _IAccGroupLedger.GetGroupLedgerDetailsData(FromDate, ToDate, GroupCode, ReportType);
            if (ReportType == "GROUPSUMMARY")
            {
                return PartialView("_GroupLedgerSummaryGrid", model);
            }
            if (ReportType == "GROUPDETAIL")
            {
                return PartialView("_GroupLedgerDetailGrid", model);
            }
            return null;
        }
    }
}
