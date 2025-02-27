using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using static eTactWeb.Data.Common.CommonFunc;
using static eTactWeb.DOM.Models.Common;

using System.Net;
using System.Data;

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
        [Route("{controller}/Index")]
        [HttpPost]
        public async Task<IActionResult> HRShiftMaster(HRShiftMasterModel model)
        {
            try
            {
                model.CreatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));

                var Result = await _IHRShiftMaster.SaveHrShiftMaster(model);
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

                return RedirectToAction(nameof(HRShiftMaster));

            }
            catch (Exception ex)
            {
                // Log and return the error
                LogException<HRShiftMasterController>.WriteException(_logger, ex);
                var ResponseResult = new ResponseResult
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    StatusText = "Error",
                    Result = ex
                };
                return View("Error", ResponseResult);
            }
        }
        public async Task<JsonResult> GetShiftId()
        {
            var JSON = await _IHRShiftMaster.GetShiftId();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        [HttpGet]
        public async Task<IActionResult> HRShiftMasterDashBoard()
        {
            try
            {
                var model = new HRShiftMasterModel();
                //model.FromDate = HttpContext.Session.GetString("FromDate");
               // model.ToDate = HttpContext.Session.GetString("ToDate");
                var Result = await _IHRShiftMaster.GetDashBoardData().ConfigureAwait(true);
                //if (Result != null)
                //{
                //    DataSet ds = Result.Result;
                //    if (ds != null && ds.Tables.Count > 0)
                //    {
                //        var dt = ds.Tables[0];
                //        model.CostCenterMasterGrid = CommonFunc.DataTableToList<CostCenterMasterModel>(dt, "CostCenterMasterDashBoard");
                //    }
                //}
                return View(model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IActionResult> GetDashBoardDetailData()
        {
            var model = new HRShiftMasterModel();
            model = await _IHRShiftMaster.GetDashBoardDetailData();
            return PartialView("_HRShiftMasterDashBoardGrid", model);
        }
    }
}
