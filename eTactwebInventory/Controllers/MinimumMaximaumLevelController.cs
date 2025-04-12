using eTactWeb.Data.Common;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using eTactWeb.DOM.Models;

namespace eTactWeb.Controllers
{
    public class MinimumMaximaumLevelController : Controller
    {
        private readonly IDataLogic _IDataLogic;
        public IMinimumMaximaumLevel _IMinimumMaximaumLevel { get; }

        private readonly ILogger<MinimumMaximaumLevelController> _logger;
        private readonly IConfiguration iconfiguration;
        public IWebHostEnvironment _IWebHostEnvironment { get; }
        public MinimumMaximaumLevelController(ILogger<MinimumMaximaumLevelController> logger, IDataLogic iDataLogic, IMinimumMaximaumLevel iMinimumMaximaumLevel, EncryptDecrypt encryptDecrypt, IWebHostEnvironment iWebHostEnvironment, IConfiguration iconfiguration)
        {
            _logger = logger;
            _IDataLogic = iDataLogic;
            _IMinimumMaximaumLevel = iMinimumMaximaumLevel;
            _IWebHostEnvironment = iWebHostEnvironment;
            this.iconfiguration = iconfiguration;
        }
        [Route("{controller}/Index")]
        [HttpGet]
        public async Task<ActionResult> MinimumMaximaumLevel()
        {
            var MainModel = new MinimumMaximaumLevelModel();
            MainModel.MinimumMaximaumLevelGrid = new List<MinimumMaximaumLevelModel>();
            MainModel.FromDate = HttpContext.Session.GetString("FromDate");
            MainModel.ToDate = HttpContext.Session.GetString("ToDate");
            MainModel.YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
            
            return View(MainModel); // Pass the model with old data to the view
        }
        public async Task<JsonResult> FillStoreName()
        {
            var JSON = await _IMinimumMaximaumLevel.FillStoreName();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillPartCode()
        {
            var JSON = await _IMinimumMaximaumLevel.FillPartCode();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillItemName()
        {
            var JSON = await _IMinimumMaximaumLevel.FillItemName();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<IActionResult> GetStandardDetailsData(string fromDate, string toDate, string ReportType, string PartCode, string StoreName, int Yearcode, string CurrentDate,string ShowItem)
        {
            var model = new MinimumMaximaumLevelModel();
            model = await _IMinimumMaximaumLevel.GetStandardDetailsData(fromDate, toDate,ReportType, PartCode, StoreName,  Yearcode,  CurrentDate, ShowItem);

            if (ReportType == "Standard")
            {
                return PartialView("_StandardGrid", model);
            }
            if (ReportType == "SHOWMINMAXWITHSTOCK")
            {
                return PartialView("_MinMaxWithStockGrid", model);
            }
            if (ReportType == "SHOWMINMAXWITHSTOCKWITHPENDPOQTY")
            {
                return PartialView("_ShowMinMaxWithStockWithPendPoQTYGrid", model);
            }
            if (ReportType == "SHOWMINMAXWITHSTOCKWITHPENDPODETAIL")
            {
                return PartialView("_ShowMinMaxWithStockWithPendPoDetailGrid", model);
            }
            return null;
        }

    }
}
