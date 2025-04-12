using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace eTactProduction.Controllers
{
    public class RecChallanReportController : Controller
    {
        private readonly IDataLogic _IDataLogic;
        public IRecChallanReport _IRecChallanReport { get; }
        private readonly ILogger<RecChallanReportController> _logger;
        private readonly IConfiguration iconfiguration;
        public IWebHostEnvironment _IWebHostEnvironment { get; }
        public RecChallanReportController(ILogger<RecChallanReportController> logger, IDataLogic iDataLogic, IRecChallanReport iRecChallanReport, EncryptDecrypt encryptDecrypt, IWebHostEnvironment iWebHostEnvironment, IConfiguration iconfiguration)
        {
            _logger = logger;
            _IDataLogic = iDataLogic;
            _IRecChallanReport = iRecChallanReport;
            _IWebHostEnvironment = iWebHostEnvironment;
            this.iconfiguration = iconfiguration;
        }
        [Route("{controller}/Index")]
        public async Task<ActionResult> RecChallanReport()
        {
            var model = new RecChallanReportModel();
            model.RecChallanReportGrid = new List<RecChallanReportModel>();
            return View(model);
        }
        public async Task<IActionResult> GetRecChallanReportGridData(string fromDate, string toDate, int EntryId, int YearCode)
        {
            var model = new RecChallanReportModel();
            model = await _IRecChallanReport.GetRecChallanReportGridData( fromDate,  toDate,  EntryId,  YearCode);

            return PartialView("_RecChallanReportGrid", model);
           
        }
    }
}
