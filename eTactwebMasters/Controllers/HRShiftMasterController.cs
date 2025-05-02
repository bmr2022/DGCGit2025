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
using Microsoft.Office.Interop.Excel;
using System.Security.Cryptography.X509Certificates;
using System.Reflection;

namespace eTactWeb.Controllers
{
    public class HRShiftMasterController : Controller
    {
        private readonly IDataLogic _IDataLogic;
        public IHRShiftMaster _IHRShiftMaster { get; }
        private readonly ILogger<HRShiftMasterController> _logger;
        private readonly IConfiguration iconfiguration;
        public IWebHostEnvironment _IWebHostEnvironment { get; }
        public HRShiftMasterController(ILogger<HRShiftMasterController> logger, IDataLogic iDataLogic, IHRShiftMaster iHRShiftMaster, EncryptDecrypt encryptDecrypt, IWebHostEnvironment iWebHostEnvironment, IConfiguration iconfiguration)
        {
            _logger = logger;
            _IDataLogic = iDataLogic;
            _IHRShiftMaster = iHRShiftMaster;
            _IWebHostEnvironment = iWebHostEnvironment;
            this.iconfiguration = iconfiguration;
        }

        [Route("{controller}/Index")]
        [HttpGet]
        public async Task<ActionResult> HRShiftMaster(string Mode,int ID, string EntryDate, int
            YearCode, string ShiftCode, string ShiftName, string FromTime, string ToTime,
            float GraceTimeIn, float GraceTimeOut,
    string Lunchfrom, string Lunchto, string FirstBreakFrom, string FirstBreakTo,
    string SecondBreakFrom, string SecondBreakTo, string CC, int UID,
    string ThirdBreakFrom, string ThirdBreakTo, int duration, string ShiftForProdOnly,
    string OutDay, string ShiftTypeFixRotFlex, string ApplicableToEmployeeCategory,
    string AttandanceMode, int MinimumHourRequiredForFullDay)
        {
            var MainModel = new HRShiftMasterModel();
            //MainModel.FromTime = HttpContext.Session.GetString("FromDate");
            //MainModel.ToTime = HttpContext.Session.GetString("ToDate");
            MainModel.FromTime = $"01/04/1900 {MainModel.FromTime}";
            MainModel.ToTime = $"01/04/1900 {MainModel.ToTime}";
            MainModel.YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
            MainModel.CC = HttpContext.Session.GetString("Branch");
            MainModel.UID = Convert.ToInt32(HttpContext.Session.GetString("UID"));
            if (!string.IsNullOrEmpty(Mode) && ID > 0 && Mode == "U")
            {
                MainModel = await _IHRShiftMaster.GetViewByID(ID).ConfigureAwait(false);
                MainModel.Mode = Mode;
                MainModel.ShiftId = ID;
                MainModel.EntryDate = EntryDate;
                MainModel.YearCode = YearCode;
                MainModel.ShiftCode = ShiftCode;
                MainModel.ShiftName = ShiftName;
                MainModel.FromTime = FromTime;
                MainModel.ToTime = ToTime;
                MainModel.GraceTimeIn = GraceTimeIn;
                MainModel.GraceTimeOut = GraceTimeOut;
                MainModel.Lunchfrom = Lunchfrom;
                MainModel.Lunchto = Lunchto;
                MainModel.FirstBreakFrom = FirstBreakFrom;
                MainModel.FirstBreakTo = FirstBreakTo;
                MainModel.SecondBreakFrom = SecondBreakFrom;
                MainModel.SecondBreakTo = SecondBreakTo;
                MainModel.CC = CC;
                MainModel.UID = UID;
                MainModel.ThirdBreakFrom = ThirdBreakFrom;
                MainModel.ThirdBreakTo = ThirdBreakTo;
                MainModel.duration = duration;
                MainModel.ShiftForProdOnly = ShiftForProdOnly;
                MainModel.OutDay = OutDay;
                MainModel.ShiftTypeFixRotFlex = ShiftTypeFixRotFlex;
                MainModel.ApplicableToEmployeeCategory = ApplicableToEmployeeCategory;
                MainModel.AttandanceMode = AttandanceMode;
                MainModel.MinimumHourRequiredForFullDay = MinimumHourRequiredForFullDay;

                MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                    SlidingExpiration = TimeSpan.FromMinutes(55),
                    Size = 1024
                };
            }
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

                return RedirectToAction(nameof(HRShiftMasterDashBoard));

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
        public async Task<IActionResult> DeleteByID(int ID)
        {
            var Result = await _IHRShiftMaster.DeleteByID(ID);

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

            return RedirectToAction("HRShiftMasterDashBoard");

        }
    }
}
