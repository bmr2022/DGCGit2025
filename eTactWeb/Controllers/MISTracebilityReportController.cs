using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;

namespace eTactWeb.Controllers
{
    public class MISTracebilityReportController : Controller
    {
        private readonly IDataLogic _IDataLogic;
        public IMISTracebilityReport _IMISTracebilityReport { get; }
        private readonly ILogger<MISTracebilityReportController> _logger;
        private readonly IConfiguration iconfiguration;
        private readonly IMemoryCache _MemoryCache;
        public IWebHostEnvironment _IWebHostEnvironment { get; }
        public MISTracebilityReportController(ILogger<MISTracebilityReportController> logger, IDataLogic iDataLogic, IMISTracebilityReport IMISTracebilityReport, EncryptDecrypt encryptDecrypt, IWebHostEnvironment iWebHostEnvironment, IConfiguration iconfiguration, IMemoryCache iMemoryCache)
        {
            _logger = logger;
            _IDataLogic = iDataLogic;
            _IMISTracebilityReport = IMISTracebilityReport;
            _IWebHostEnvironment = iWebHostEnvironment;
            this.iconfiguration = iconfiguration;
            _MemoryCache = iMemoryCache;
        }
        [Route("{controller}/Index")]
        public async Task<ActionResult> MISTracebilityReport()
        {
            var model = new MISTracebilityReportModel();
            model.FromDate = HttpContext.Session.GetString("FromDate");
            return View(model);
        }
        public async Task<IActionResult> GetMISTracebilityReportData(string FromDate, string ToDate, string SaleBillNo)
        {
            var model = await _IMISTracebilityReport.GetMISTracebilityReportData(FromDate, ToDate, SaleBillNo);

            // Optional: cache raw dataset for export later
            _MemoryCache.Set("KeyMISTraceabilityReport", model, new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                SlidingExpiration = TimeSpan.FromMinutes(55)
            });

            // Return PartialView based on report type (if required)
            return PartialView("_MISTraceabilityReportGrid", model);
        }

        public async Task<JsonResult> FillSaleBillNoList(string FromDate, string ToDate)
        {
            var JSON = await _IMISTracebilityReport.FillSaleBillNoList(FromDate, ToDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
    }
}
