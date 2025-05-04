using eTactWeb.Controllers;
using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;

namespace eTactwebPurchase.Controllers
{
    public class MaterialReqPlanningController : Controller
    {
        private readonly IDataLogic _IDataLogic;
        public IMaterialReqPlanning _IMaterialReqPlanning { get; }
        private readonly ILogger<MaterialReqPlanningController> _logger;
        private readonly IConfiguration iconfiguration;
        public IWebHostEnvironment _IWebHostEnvironment { get; }

        public MaterialReqPlanningController(ILogger<MaterialReqPlanningController> logger, IDataLogic iDataLogic, EncryptDecrypt encryptDecrypt, IWebHostEnvironment iWebHostEnvironment, IConfiguration iconfiguration, IMaterialReqPlanning iMaterialReqPlanning)
        {
            _logger = logger;
            _IDataLogic = iDataLogic;
            _IMaterialReqPlanning = iMaterialReqPlanning;
            _IWebHostEnvironment = iWebHostEnvironment;
            this.iconfiguration = iconfiguration;
        }

        public IActionResult MaterialReqPlanning()
        {
            var model = new MaterialReqPlanningModel();
            model.Year_Code = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
            model.DayWiseMRPDataGrid = new List<DayWiseMRPData>();
            return View(model);
        }
        public async Task<JsonResult> GetMRPNo(int YearCode)
        {
            var JSON = await _IMaterialReqPlanning.GetMRPNo(YearCode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetMonthList(int YearCode)
        {
            var JSON = await _IMaterialReqPlanning.GetMonthList(YearCode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
          public async Task<JsonResult> GetPartCode()
        {
            var JSON = await _IMaterialReqPlanning.GetPartCode();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
          public async Task<JsonResult> GetItemName()
        {
            var JSON = await _IMaterialReqPlanning.GetItemName();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }

        public async Task<IActionResult> GetDetailData(string ReportType, string mrpno, string Month, int YearCode, string FromDate, string ToDate, string ItemName, string PartCode)
        {
            var model = new MaterialReqPlanningModel();
            model = await _IMaterialReqPlanning.GetDetailData( ReportType,mrpno, Month, YearCode,  FromDate,  ToDate,  ItemName,  PartCode);
               if(ReportType== "DAYWISEMRPDATA")
               {
                   return PartialView("_MaterialReqPlanningGrid", model);
               }
               if(ReportType== "MRPCONSOLIDATED")
               {
                   return PartialView("_MaterialReqPlanningCONSOLIDATEDGrid", model);
               }
               if(ReportType== "MRPDataonly")
               {
                   return PartialView("_MaterialReqPlanningMRPDataonlyGrid", model);
               }
               if(ReportType== "MRPCONSOLIDATE (With PO + Party)")
               {
                   return PartialView("_MaterialReqPlanningMRPCONSOLIDATE(WithPO+Party)Grid", model);
               }
               if(ReportType== "MRP DATA WITH PARTY And SOB")
               {
                   return PartialView("_MaterialReqPlanningPartyAndSOBGrid", model);
               }

               return null;
            
        }
    }
}
