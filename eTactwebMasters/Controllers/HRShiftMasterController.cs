using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;

namespace eTactWeb.Controllers
{
    public class HRShiftMasterController : Controller
    {
        private readonly IDataLogic _IDataLogic;
        public IHRShiftMaster _IHRShiftMaster { get; }

        private readonly ILogger<HRShiftMasterController> _logger;
        private readonly IConfiguration iconfiguration;
        private readonly IMemoryCache _MemoryCache;
        public IWebHostEnvironment _IWebHostEnvironment { get; }
        public HRShiftMasterController(ILogger<HRShiftMasterController> logger, IDataLogic iDataLogic, IHRShiftMaster iHRShiftMaster, IMemoryCache iMemoryCache, EncryptDecrypt encryptDecrypt, IWebHostEnvironment iWebHostEnvironment, IConfiguration iconfiguration)
        {
            _logger = logger;
            _IDataLogic = iDataLogic;
            _IHRShiftMaster = iHRShiftMaster;
            _MemoryCache = iMemoryCache;
            _IWebHostEnvironment = iWebHostEnvironment;
            this.iconfiguration = iconfiguration;
        }

        [Route("{controller}/Index")]
        [HttpGet]
        public async Task<ActionResult> HRShiftMaster()
        {
            var MainModel = new HRShiftMasterModel();
            MainModel.FromTime = HttpContext.Session.GetString("FromDate");
            MainModel.ToTime = HttpContext.Session.GetString("ToDate");
            MainModel.YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
            MainModel.CC = HttpContext.Session.GetString("Branch");
            MainModel.UID = Convert.ToInt32(HttpContext.Session.GetString("UID"));
            return View(MainModel); // Pass the model with old data to the view
        }
        public async Task<JsonResult> GetShiftId()
        {
            var JSON = await _IHRShiftMaster.GetShiftId();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
    }
}
