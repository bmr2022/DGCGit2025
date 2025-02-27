using eTactWeb.Data.Common;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using static eTactWeb.Data.Common.CommonFunc;
using static eTactWeb.DOM.Models.Common;
using eTactWeb.DOM.Models;
namespace eTactWeb.Controllers
{
    public class CloseProductionPlanController : Controller
    {
        private readonly IDataLogic _IDataLogic;
        public ICloseProductionPlan _ICloseProductionPlan { get; }

        private readonly ILogger<CloseProductionPlanController> _logger;
        private readonly IConfiguration iconfiguration;
        private readonly IMemoryCache _MemoryCache;
        public IWebHostEnvironment _IWebHostEnvironment { get; }
        public CloseProductionPlanController(ILogger<CloseProductionPlanController> logger, IDataLogic iDataLogic, ICloseProductionPlan iCloseProductionPlan, IMemoryCache iMemoryCache, EncryptDecrypt encryptDecrypt, IWebHostEnvironment iWebHostEnvironment, IConfiguration iconfiguration)
        {
            _logger = logger;
            _IDataLogic = iDataLogic;
            _ICloseProductionPlan = iCloseProductionPlan;
            _MemoryCache = iMemoryCache;
            _IWebHostEnvironment = iWebHostEnvironment;
            this.iconfiguration = iconfiguration;
        }
        [Route("{controller}/Index")]
        [HttpGet]
        public async Task<ActionResult> CloseProductionPlan(int ID, string Mode)
        {
            _logger.LogInformation("\n \n ********** Page Gate Inward ********** \n \n " + _IWebHostEnvironment.EnvironmentName.ToString() + "\n \n");

            // Clear TempData and set session variables
            TempData.Clear();
            var MainModel = new CloseProductionPlanModel();
            MainModel.FromDate = HttpContext.Session.GetString("FromDate");
            MainModel.ToDate = HttpContext.Session.GetString("ToDate");
            MainModel.CreatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
            MainModel.YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
            MainModel.ActualEntryId = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
            MainModel.EmpId = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
            MainModel.EntryByEmpName = HttpContext.Session.GetString("EmpName");
            MainModel.ActualEntryDate = DateTime.Today.ToString("MM/dd/yyyy").Replace("-", "/");
           
            _MemoryCache.Remove("CloseProductionPlanGrid");

            
            if (!string.IsNullOrEmpty(Mode) && ID > 0 && Mode == "U")
            {

                
                //MainModel = await _IAlternateItemMaster.GetViewByID(MainItemCode, AlternateItemCode).ConfigureAwait(false);
                //MainModel.Mode = Mode; // Set Mode to Update
                //MainModel.ID = ID;
                //MainModel.MainPartCode = MainPartCode;
                //MainModel.MainItemName = MainItemName;
                //MainModel.AlternatePartCode = AlternatePartCode;
                //MainModel.AltItemName = AltItemName;
                //MainModel.MainItemCode = MainItemCode;
                //MainModel.AlternateItemCode = AlternateItemCode;

                ////MainModel = await BindModels(MainModel).ConfigureAwait(false);
                //MainModel.FinFromDate = HttpContext.Session.GetString("FromDate");
                //MainModel.FinToDate = HttpContext.Session.GetString("ToDate");
                //if (Mode == "U")
                //{
                //    MainModel.UpdatedByEmp = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                //    MainModel.LastUpdatedBy = HttpContext.Session.GetString("EmpName");
                //    MainModel.UpdationDate = DateTime.Now.ToString();
                //    MainModel.ActualEntryDate = DateTime.Today.ToString("MM/dd/yyyy").Replace("-", "/");
                //    MainModel.ActualEntryByEmp = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
                //    MainModel.EffectiveDate = DateTime.Today.ToString("MM/dd/yyyy").Replace("-", "/");

                //}
                //MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
                //{
                //    AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                //    SlidingExpiration = TimeSpan.FromMinutes(55),
                //    Size = 1024
                //};
                //_MemoryCache.Set("AlternateItemMasterGrid", MainModel.AlternateItemMasterGrid, cacheEntryOptions);
            }
            else
            {
                // MainModel = await BindModels(MainModel);
            }

            return View(MainModel); 
        }
        [Route("{controller}/Index")]
        [HttpPost]
        public async Task<IActionResult> CloseProductionPlan(CloseProductionPlanModel model)
        {
            try
            {
                model.CreatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));

                var Result = await _ICloseProductionPlan.SaveCloseProductionPlan(model);
                if (Result != null)
                {
                    if (Result.StatusText == "Success" && Result.StatusCode == HttpStatusCode.OK)
                    {
                        ViewBag.isSuccess = true;
                        TempData["200"] = "200";
                        //_MemoryCache.Remove("KeyLedgerOpeningEntryGrid");
                    }
                    else if (Result.StatusText == "Success" && Result.StatusCode == HttpStatusCode.Accepted)
                    {
                        ViewBag.isSuccess = true;
                        TempData["202"] = "202";
                    }
                    else if (Result.StatusText == "Error" && Result.StatusCode == HttpStatusCode.InternalServerError)
                    {
                        ViewBag.isSuccess = false;
                        TempData["500"] = "500";
                        _logger.LogError($"\n \n ********** LogError ********** \n {JsonConvert.SerializeObject(Result)}\n \n");
                        return View("Error", Result);
                    }
                }

                return RedirectToAction(nameof(CloseProductionPlan));

            }
            catch (Exception ex)
            {
                // Log and return the error
                LogException<CloseProductionPlanController>.WriteException(_logger, ex);
                var ResponseResult = new ResponseResult
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    StatusText = "Error",
                    Result = ex
                };
                return View("Error", ResponseResult);
            }
        }

        public async Task<JsonResult> GetOpenItemName(int EmpId, int ActualEntryId)
        {
            var JSON = await _ICloseProductionPlan.GetOpenItemName( EmpId,  ActualEntryId);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
         public async Task<JsonResult> GetOpenPlanNo(int EmpId, int ActualEntryId)
        {
            var JSON = await _ICloseProductionPlan.GetOpenPlanNo( EmpId,  ActualEntryId);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
         public async Task<JsonResult> GetCloseItemName(int EmpId, int ActualEntryId)
        {
            var JSON = await _ICloseProductionPlan.GetCloseItemName( EmpId,  ActualEntryId);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
         public async Task<JsonResult> GetClosePlanNo(int EmpId, int ActualEntryId)
        {
            var JSON = await _ICloseProductionPlan.GetClosePlanNo( EmpId,  ActualEntryId);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<IActionResult> GetDetailData(int EmpId, string ActualEntryByEmpName, string ReportType, string FromDate, string ToDate)
        {
            //model.Mode = "Search";
            var model = new CloseProductionPlanModel();
           // ActualEntryByEmpName = HttpContext.Session.GetString("EmpName");
            ActualEntryByEmpName = HttpContext.Session.GetString("EmpName");
            EmpId = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
            ReportType ??= "SUMMARY";
            model = await _ICloseProductionPlan.GetGridDetailData(EmpId, ActualEntryByEmpName, ReportType, FromDate, ToDate);
            if(ReportType== "SUMMARY")
            {
                return PartialView("_CloseProductionPlanSummaryGrid", model);
            }
            if(ReportType== "DETAIL")
            {
                return PartialView("_CloseProductionPlanDetailGrid", model);
            }
            return null;
        }

        }
    }
