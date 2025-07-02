using eTactWeb.Data.Common;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Office.Interop.Excel;
using Newtonsoft.Json;
using eTactWeb.DOM.Models;
using System.Collections.Generic;
using FastReport.Data;
using FastReport.Web;
using FastReport;

namespace eTactWeb.Controllers
{
    public class BOMReportController : Controller
    {
        private readonly IDataLogic _IDataLogic;
        public IBOMReport _IBOMReport { get; }
        private readonly ILogger<BOMReportController> _logger;
        private readonly IConfiguration _iconfiguration;
        public IWebHostEnvironment _IWebHostEnvironment { get; }
       
        public BOMReportController(ILogger<BOMReportController> logger, IDataLogic iDataLogic, IBOMReport iBOMReport, EncryptDecrypt encryptDecrypt, IWebHostEnvironment iWebHostEnvironment, IConfiguration iconfiguration)
        {
            _logger = logger;
            _IDataLogic = iDataLogic;
            _IBOMReport = iBOMReport;
            _IWebHostEnvironment = iWebHostEnvironment;
            _iconfiguration = iconfiguration;
        }
        [Route("{controller}/Index")]
        public async Task<ActionResult> BOMReport()
        {
            var model = new BOMReportModel();
            model.BOMReportGrid = new List<BOMReportModel>();
            model.YearCode= Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
            return View(model);
        }
        public IActionResult PrintReport(string FGPartCode = "", string FGName = "", string RMPartCode = "", string RMName = "", int StoreId = 0,int WCID = 0,string CurrentDate="",int YearCode=0,int CalForQty=0)
        {
            string my_connection_string;
            string contentRootPath = _IWebHostEnvironment.ContentRootPath;
            string webRootPath = _IWebHostEnvironment.WebRootPath;
            var webReport = new WebReport();
            webReport.Report.Clear();
            
            webReport.Report.Dispose();
            webReport.Report = new Report();
           
                webReport.Report.Load(webRootPath + "\\BOMPrintReportForDirectBOMStockShortExcess.frx"); // default report
           
            webReport.Report.SetParameterValue("FGPartcodeparam", FGPartCode);
            webReport.Report.SetParameterValue("FGNameparam", FGName);
            webReport.Report.SetParameterValue("RMPartcodeparam", RMPartCode);
            webReport.Report.SetParameterValue("RMNameparam", RMName);
            webReport.Report.SetParameterValue("Storeidparam", StoreId);
            webReport.Report.SetParameterValue("WCIDparam", WCID);
            webReport.Report.SetParameterValue("CurrentDateparam", CommonFunc.ParseFormattedDate( CurrentDate));
            webReport.Report.SetParameterValue("Yearcodeparam", YearCode);
            webReport.Report.SetParameterValue("CalForQtyparam", CalForQty);
            //webReport.Report.SetParameterValue("WcName ", WCID);
            my_connection_string = _iconfiguration.GetConnectionString("eTactDB");
            webReport.Report.Dictionary.Connections[0].ConnectionString = my_connection_string;
            webReport.Report.Dictionary.Connections[0].ConnectionStringExpression = "";
            webReport.Report.SetParameterValue("MyParameter", my_connection_string);
            webReport.Report.Refresh();
            return View(webReport);
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
        public async Task<IActionResult> GetBomTreeDetailsData(string fromDate, string toDate, int Yearcode, string ReportType, string FGPartCode, string RMPartCode, int Storeid,float calculateQty, int FGItemCode)
        {
            var model = new BOMReportModel();
            model = await _IBOMReport.GetBomTreeDetailsData(fromDate, toDate, Yearcode,ReportType, FGPartCode, RMPartCode, Storeid, calculateQty, FGItemCode);

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
