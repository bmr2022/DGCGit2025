using eTactWeb.Data.Common;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Office.Interop.Excel;
using Newtonsoft.Json;
using eTactWeb.DOM.Models;
using System.Collections.Generic;

namespace eTactWeb.Controllers
{
    public class BOMReportController : Controller
    {
        private readonly IDataLogic _IDataLogic;
        public IBOMReport _IBOMReport { get; }

        private readonly ILogger<BOMReportController> _logger;
        private readonly IConfiguration iconfiguration;
        private readonly IMemoryCache _MemoryCache;
        public IWebHostEnvironment _IWebHostEnvironment { get; }
        public BOMReportController(ILogger<BOMReportController> logger, IDataLogic iDataLogic, IBOMReport iBOMReport, IMemoryCache iMemoryCache, EncryptDecrypt encryptDecrypt, IWebHostEnvironment iWebHostEnvironment, IConfiguration iconfiguration)
        {
            _logger = logger;
            _IDataLogic = iDataLogic;
            _IBOMReport = iBOMReport;
            _MemoryCache = iMemoryCache;
            _IWebHostEnvironment = iWebHostEnvironment;
            this.iconfiguration = iconfiguration;
        }
        [Route("{controller}/Index")]
        public async Task<ActionResult> BOMReport()
        {
            var model = new BOMReportModel();
            model.BOMReportGrid = new List<BOMReportModel>();
            model.YearCode= Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
            return View(model);
        }
        public async Task<JsonResult> GetBOMTree()
        {
            var JSON = await _IBOMReport.GetBOMTree();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetDirectBOMTree()
        {
            var JSON = await _IBOMReport.GetDirectBOMTree();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        } 
        public async Task<JsonResult> GetBOMStockTree()
        {
            var JSON = await _IBOMReport.GetBOMStockTree();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillFinishPartCode()
        {
            var JSON = await _IBOMReport.FillFinishPartCode();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillFinishItemName()
        {
            var JSON = await _IBOMReport.FillFinishItemName();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillRMItemName()
        {
            var JSON = await _IBOMReport.FillRMItemName();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        } 
        public async Task<JsonResult> FillRMPartCode()
        {
            var JSON = await _IBOMReport.FillRMPartCode();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillStoreName()
        {
            var JSON = await _IBOMReport.FillStoreName();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillWorkCenterName()
        {
            var JSON = await _IBOMReport.FillWorkCenterName();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<IActionResult> GetBomTreeDetailsData(string fromDate, string toDate, int Yearcode, string ReportType, string FGPartCode, string RMPartCode, int Storeid,float calculateQty)
        {
            var model = new BOMReportModel();
            model = await _IBOMReport.GetBomTreeDetailsData(fromDate, toDate, Yearcode,ReportType, FGPartCode, RMPartCode, Storeid, calculateQty);

            if (ReportType == "BOMTREE")
            {
                 return PartialView("_BOMReportGrid", model);
            }
            if (ReportType == "BOMSTOCK")
            {
                 return PartialView("_BOMSTOCK", model);
            }
            if (ReportType == "DirectBOM")
            {
                 return PartialView("_DirectBOM", model);
            }
            if (ReportType == "BOMSTOCK(WITH SUB BOM)")
            {
                 return PartialView("_WITHSUBBOM", model);
            }
            if (ReportType == "BOMSTOCK(DIRECT CHILD)")
            {
                 return PartialView("_DIRECTCHILD", model);
            }

            return null;
        }

        //public async Task<IActionResult> GetBOMTree(string ReportType, string FromDate, string ToDate)
        //{
        //    // Fetch data from the service
        //    var BOMReportData = await _IBOMReport.GetBOMTree();

        //    // Pass data to a partial view
        //    return PartialView("_BOMTableRows", BOMReportData);
        //}
    }
}
