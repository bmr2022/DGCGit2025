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
        private readonly IMemoryCache _MemoryCache;
        public IWebHostEnvironment _IWebHostEnvironment { get; }

        public MaterialReqPlanningController(ILogger<MaterialReqPlanningController> logger, IDataLogic iDataLogic, IMemoryCache iMemoryCache, EncryptDecrypt encryptDecrypt, IWebHostEnvironment iWebHostEnvironment, IConfiguration iconfiguration, IMaterialReqPlanning iMaterialReqPlanning)
        {
            _logger = logger;
            _IDataLogic = iDataLogic;
            _IMaterialReqPlanning = iMaterialReqPlanning;
            _MemoryCache = iMemoryCache;
            _IWebHostEnvironment = iWebHostEnvironment;
            this.iconfiguration = iconfiguration;
        }

        public IActionResult MaterialReqPlanning()
        {
            var model = new MaterialReqPlanningModel();
            model.Year_Code = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
            model.DayWiseMRPData = new List<DayWiseMRPData>();
            return View(model);
        }
        public async Task<JsonResult> GetMRPNo(int YearCode)
        {
            var JSON = await _IMaterialReqPlanning.GetMRPNo(YearCode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetMonthList()
        {
            var JSON = await _IMaterialReqPlanning.GetMonthList();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
    }
}
