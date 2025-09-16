using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

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
        public async Task<IActionResult> GetProfitAndLossData(string FromDate, string ToDate,string Flag, string ReportType)
        {
            var model = new ProfitAndLossModel();
            model.EntryByMachine = Environment.MachineName;
            model = await _IProfitAndLoss.GetProfitAndLossData(FromDate, ToDate, Flag, ReportType);

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
    }
}
