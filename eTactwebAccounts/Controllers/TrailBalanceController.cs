using eTactWeb.Controllers;
using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace eTactwebAccounts.Controllers
{
    public class TrailBalanceController : Controller
    {
        private readonly IDataLogic _IDataLogic;
        public ITrailBalance _ITrailBalance { get; }

        private readonly ILogger<TrailBalanceController> _logger;
        private readonly IConfiguration iconfiguration;
        private readonly IMemoryCache _MemoryCache;
        public IWebHostEnvironment _IWebHostEnvironment { get; }
        public TrailBalanceController(ILogger<TrailBalanceController> logger, IDataLogic iDataLogic, ITrailBalance iTrailBalance, IMemoryCache iMemoryCache, EncryptDecrypt encryptDecrypt, IWebHostEnvironment iWebHostEnvironment, IConfiguration iconfiguration)
        {
            _logger = logger;
            _IDataLogic = iDataLogic;
            _ITrailBalance = iTrailBalance;
            _MemoryCache = iMemoryCache;
            _IWebHostEnvironment = iWebHostEnvironment;
            this.iconfiguration = iconfiguration;
        }
        [Route("{controller}/Index")]
        [HttpGet]
        public async Task<ActionResult> TrailBalance()
        {
            var MainModel = new TrailBalanceModel();
            MainModel.TrailBalanceGrid = new List<TrailBalanceModel>();
            MainModel.FromDate = HttpContext.Session.GetString("FromDate");
            MainModel.ToDate = HttpContext.Session.GetString("ToDate");
            MainModel.EntryByMachine = Environment.MachineName;
            //MainModel.YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));

            return View(MainModel); // Pass the model with old data to the view
        }
        public async Task<IActionResult> GetTrailBalanceDetailsData(string FromDate, string ToDate, string EntryByMachine,string ReportType)
        {
            var model = new TrailBalanceModel();
            model.EntryByMachine = Environment.MachineName;
            model = await _ITrailBalance.GetTrailBalanceDetailsData(FromDate, ToDate, EntryByMachine, ReportType);
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
    }
}
