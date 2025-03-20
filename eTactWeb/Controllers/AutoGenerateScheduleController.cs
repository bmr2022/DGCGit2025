using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;

namespace eTactWeb.Controllers
{
    public class AutoGenerateScheduleController : Controller
    {
        private readonly IDataLogic _IDataLogic;
        public IAutoGenerateSchedule _IAutoGenerateSchedule { get; }

        private readonly ILogger<AutoGenerateScheduleController> _logger;
        private readonly IConfiguration iconfiguration;
        private readonly IMemoryCache _MemoryCache;
        public IWebHostEnvironment _IWebHostEnvironment { get; }
        public AutoGenerateScheduleController(ILogger<AutoGenerateScheduleController> logger, IDataLogic iDataLogic, IAutoGenerateSchedule iAutoGenerateSchedule, IMemoryCache iMemoryCache, EncryptDecrypt encryptDecrypt, IWebHostEnvironment iWebHostEnvironment, IConfiguration iconfiguration)
        {
            _logger = logger;
            _IDataLogic = iDataLogic;
            _IAutoGenerateSchedule = iAutoGenerateSchedule;
            _MemoryCache = iMemoryCache;
            _IWebHostEnvironment = iWebHostEnvironment;
            this.iconfiguration = iconfiguration;   
        }
        [Route("{controller}/Index")]
        [HttpGet]
        public async Task<ActionResult> AutoGenerateSchedule()
        {
            var MainModel = new AutoGenerateScheduleModel();
            MainModel.AutoGenerateScheduleGrid = new List<AutoGenerateScheduleModel>();
            //MainModel.FromDate = HttpContext.Session.GetString("FromDate");
            //MainModel.ToDate = HttpContext.Session.GetString("ToDate");
            MainModel.MrpYearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
            MainModel.CreatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
            MainModel.UID = Convert.ToInt32(HttpContext.Session.GetString("UID"));
            MainModel.CC = HttpContext.Session.GetString("Branch");
            return View(MainModel); // Pass the model with old data to the view
        }
        public async Task<JsonResult> GetMrpNo(string SchEffectivedate, string MrpYearCode)
        {
            var JSON = await _IAutoGenerateSchedule.GetMrpNo(SchEffectivedate, MrpYearCode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<IActionResult> GetAutoGenSchDetailData(string ReportType, string SchEffectivedate, string MrpNo, int MrpYearCode, int MrpEntryId, int CreatedBy, string CC, int UID, string MachineName)
        {
            var model = new AutoGenerateScheduleModel();
            model = await _IAutoGenerateSchedule.GetAutoGenSchDetailData(ReportType,SchEffectivedate,MrpNo,MrpYearCode, MrpEntryId,  CreatedBy,  CC,  UID,  MachineName);
            if(ReportType== "List Of Item For Schedule")
            {
                return PartialView("_AutoGenSchListOfItem", model);
            }
            if(ReportType== "Generate Purchase Schedule")
            {
                return PartialView("_AutoGenPurchaseSch", model);
            }
            return null;

        }
    }
}
