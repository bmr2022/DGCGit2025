using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace eTactWeb.Controllers
{
    public class IssueVSConsumptionReportController : Controller
    {
        private readonly IDataLogic _IDataLogic;
        public IIssueVSConsumptionReport _IIssueVSConsumptionReport { get; }
        private readonly ILogger<IssueVSConsumptionReportController> _logger;
        private readonly IConfiguration _iconfiguration;
        public IWebHostEnvironment _IWebHostEnvironment { get; }

        public IssueVSConsumptionReportController(ILogger<IssueVSConsumptionReportController> logger, IDataLogic iDataLogic, IIssueVSConsumptionReport iIssueVSConsumptionReport, EncryptDecrypt encryptDecrypt, IWebHostEnvironment iWebHostEnvironment, IConfiguration iconfiguration)
        {
            _logger = logger;
            _IDataLogic = iDataLogic;
            _IIssueVSConsumptionReport = iIssueVSConsumptionReport;
            _IWebHostEnvironment = iWebHostEnvironment;
            _iconfiguration = iconfiguration;
        }
        [Route("{controller}/Index")]
        public async Task<ActionResult> IssueVSConsumptionReport()
        {
            var model = new IssueVSConsumptionReportModel();
            model.IssueVSConsumptionReportGrid = new List<IssueVSConsumptionReportModel>();
            //model.YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
            return View(model);
        }

        public async Task<IActionResult> GetDetailData()
        {
            var model = new IssueVSConsumptionReportModel();
            model = await _IIssueVSConsumptionReport.GetDetailData();


            return PartialView("_IssueVSConsumptionGrid", model);

        }

    }
}
