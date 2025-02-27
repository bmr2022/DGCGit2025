using eTactWeb.Data.Common;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using static eTactWeb.Data.Common.CommonFunc;
using static eTactWeb.DOM.Models.Common;
using eTactWeb.DOM.Models;
using System.Net;
using System.Data;

namespace eTactWeb.Controllers
{
    public class UnitMasterController : Controller
    {
        private readonly IDataLogic _IDataLogic;
        public IUnitMaster _IUnitMaster { get; }

        private readonly ILogger<UnitMasterController> _logger;
        private readonly IConfiguration iconfiguration;
        private readonly IMemoryCache _MemoryCache;
        public IWebHostEnvironment _IWebHostEnvironment { get; }
        public UnitMasterController(ILogger<UnitMasterController> logger, IDataLogic iDataLogic, IUnitMaster iUnitMaster, IMemoryCache iMemoryCache, EncryptDecrypt encryptDecrypt, IWebHostEnvironment iWebHostEnvironment, IConfiguration iconfiguration)
        {
            _logger = logger;
            _IDataLogic = iDataLogic;
            _IUnitMaster = iUnitMaster;
            _MemoryCache = iMemoryCache;
            _IWebHostEnvironment = iWebHostEnvironment;
            this.iconfiguration = iconfiguration;
        }
        [Route("{controller}/Index")]
        [HttpGet]
        public async Task<ActionResult> UnitMaster(string Unit_Name, string Mode, string UnitDetail, string CC,String Round_Off)
        {
            _logger.LogInformation("\n \n ********** Page Gate Inward ********** \n \n " + _IWebHostEnvironment.EnvironmentName.ToString() + "\n \n");
            TempData.Clear();
            var MainModel = new UnitMasterModel();
            MainModel.FromDate = HttpContext.Session.GetString("FromDate");
            MainModel.ToDate = HttpContext.Session.GetString("ToDate");
            MainModel.CC = HttpContext.Session.GetString("Branch");
            //MainModel.EntryDate = HttpContext.Session.GetString("EntryDate");

            if (!string.IsNullOrEmpty(Mode) && Unit_Name!=" " && Mode == "U")
            {
                MainModel = await _IUnitMaster.GetViewByID(Unit_Name).ConfigureAwait(false);
                MainModel.Mode = Mode; // Set Mode to Update
                MainModel.Unit_Name = Unit_Name;
                MainModel.Round_Off = Round_Off;
                MainModel.UnitDetail = UnitDetail;               
                MainModel.CC = CC;
                                
                MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                    SlidingExpiration = TimeSpan.FromMinutes(55),
                    Size = 1024
                };
            }

            return View(MainModel);
        }

        [Route("{controller}/Index")]
        [HttpPost]
        public async Task<IActionResult> UnitMaster(UnitMasterModel model)
        {
            try
            {
                model.CreatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));

                var Result = await _IUnitMaster.SaveUnitMaster(model);
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

                return RedirectToAction(nameof(UnitMasterDashBoard));

            }
            catch (Exception ex)
            {
                // Log and return the error
                LogException<UnitMasterController>.WriteException(_logger, ex);
                var ResponseResult = new ResponseResult
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    StatusText = "Error",
                    Result = ex
                };
                return View("Error", ResponseResult);
            }
        }

        [HttpGet]
        public async Task<IActionResult> UnitMasterDashBoard()
        {
            try
            {
                var model = new UnitMasterModel();
                model.FromDate = HttpContext.Session.GetString("FromDate");
                model.ToDate = HttpContext.Session.GetString("ToDate");
                var Result = await _IUnitMaster.GetDashBoardData().ConfigureAwait(true);
                if (Result != null)
                {
                    DataSet ds = Result.Result;
                    if (ds != null && ds.Tables.Count > 0)
                    {
                        var dt = ds.Tables[0];
                        model.UnitMasterDashBoardGrid = CommonFunc.DataTableToList<UnitMasterModel>(dt, "UnitMasterDashBoard");
                    }
                }
                return View(model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IActionResult> GetDashBoardDetailData()
        {
            var model = new UnitMasterModel();
            model = await _IUnitMaster.GetDashBoardDetailData();
            return PartialView("_UnitMasterDashBoardGrid", model);
        }


        public async Task<IActionResult> DeleteByID(String Unit_Name)
        {
            var Result = await _IUnitMaster.DeleteByID(Unit_Name);

            if (Result.StatusText == "Success" || Result.StatusCode == HttpStatusCode.Gone)
            {
                ViewBag.isSuccess = true;
                TempData["410"] = "410";
                //TempData["Message"] = "Data deleted successfully.";
            }
            else if (Result.StatusText == "Error" || Result.StatusCode == HttpStatusCode.Accepted)
            {
                ViewBag.isSuccess = true;
                TempData["423"] = "423";
            }
            else
            {
                ViewBag.isSuccess = false;
                TempData["500"] = "500";
            }

            return RedirectToAction("UnitMasterDashBoard");

        }
    }
}

