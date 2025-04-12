using eTactWeb.Data.Common;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using eTactWeb.DOM.Models;

namespace eTactWeb.Controllers
{
    public class StockValuationController : Controller
    {
        private readonly IDataLogic _IDataLogic;
        public IStockValuation _IStockValuation { get; }
        private readonly ILogger<StockValuationController> _logger;
        private readonly IConfiguration iconfiguration;
        public IWebHostEnvironment _IWebHostEnvironment { get; }
        public StockValuationController(ILogger<StockValuationController> logger, IDataLogic iDataLogic, IStockValuation iStockValuation, EncryptDecrypt encryptDecrypt, IWebHostEnvironment iWebHostEnvironment, IConfiguration iconfiguration)
        {
            _logger = logger;
            _IDataLogic = iDataLogic;
            _IStockValuation = iStockValuation;
            _IWebHostEnvironment = iWebHostEnvironment;
            this.iconfiguration = iconfiguration;
        }
        [Route("{controller}/Index")]
        public async Task<ActionResult> StockValuation()
        {
            var model = new StockValuationModel();
            model.StockValuationGrid = new List<StockValuationModel>();
            //model.YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
            return View(model);
        }
        public async Task<JsonResult> FillStoreName(string FromDate, string CurrentDate)
        {
            var JSON = await _IStockValuation.FillStoreName( FromDate, CurrentDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillItemName(string FromDate, string CurrentDate)
        {
            var JSON = await _IStockValuation.FillItemName( FromDate, CurrentDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillPartCode(string FromDate, string CurrentDate)
        {
            var JSON = await _IStockValuation.FillPartCode( FromDate, CurrentDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }

        public async Task<IActionResult> GetStockValuationDetailsData(string FromDate, string ToDate,string StoreId,string ReportType, int ItemCode)
        {
            var model = new StockValuationModel();
            model = await _IStockValuation.GetStockValuationDetailsData(FromDate, ToDate, StoreId, ReportType,  ItemCode);
          
            if (ReportType == "BatchWise Stock Valuation")
            {
                return PartialView("_BatchWiseStockValuation", model);
            }
            if (ReportType == "BatchWise Stock+WIP Valuation")
            {
                return PartialView("_BatchWiseStock+WIPValuation", model);
            }
            if ( ReportType == "BatchWise WIP Valuation")
            {
                return PartialView("_BatchWiseWIPValuation", model);
            }
            
            if (ReportType == "Stock Valuation")
            {
                return PartialView("_StockValuation", model);
            }

            return null;
        }
    }
}
