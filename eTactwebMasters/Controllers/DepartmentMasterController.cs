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
    public class DepartmentMasterController : Controller
    {
        private readonly IDataLogic _IDataLogic;
        public IDepartmentMaster _IDepartmentMaster { get; }
        private readonly ILogger<DepartmentMasterController> _logger;
        private readonly IConfiguration iconfiguration;
        public IWebHostEnvironment _IWebHostEnvironment { get; }
        public DepartmentMasterController(ILogger<DepartmentMasterController> logger, IDataLogic iDataLogic, IDepartmentMaster iDepartmentMaster, EncryptDecrypt encryptDecrypt, IWebHostEnvironment iWebHostEnvironment, IConfiguration iconfiguration)
        {
            _logger = logger;
            _IDataLogic = iDataLogic;
            _IDepartmentMaster = iDepartmentMaster;
            _IWebHostEnvironment = iWebHostEnvironment;
            this.iconfiguration = iconfiguration;
        }
        [Route("{controller}/Index")]
        [HttpGet]
        public async Task<ActionResult> DepartmentMaster(int ID, string Mode, string DeptName, string DeptType, string CC, string Entry_Date,string departmentcode)
        {
            _logger.LogInformation("\n \n ********** Page Gate Inward ********** \n \n " + _IWebHostEnvironment.EnvironmentName.ToString() + "\n \n");
            TempData.Clear();
            var MainModel = new DepartmentMasterModel();
            MainModel.DeptName = HttpContext.Session.GetString("DeptName");
            MainModel.CC = HttpContext.Session.GetString("CC");
            MainModel.Entry_Date = HttpContext.Session.GetString("Entry_Date");
            MainModel.DeptType = HttpContext.Session.GetString("DeptType");
            MainModel.departmentcode = HttpContext.Session.GetString("departmentcode");

            if (!string.IsNullOrEmpty(Mode) && ID > 0 && Mode == "U")
            {
                MainModel = await _IDepartmentMaster.GetViewByID(ID).ConfigureAwait(false);
                MainModel.Mode = Mode; // Set Mode to Update
                MainModel.DeptId = ID;
                MainModel.DeptName = DeptName;
                MainModel.DeptType = DeptType;
                MainModel.CC = CC;
                MainModel.Entry_Date = Entry_Date;
                MainModel.departmentcode = departmentcode;

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
        public async Task<IActionResult> DepartmentMaster(DepartmentMasterModel model)
        {
            try
            {
                model.CreatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));

                var Result = await _IDepartmentMaster.SaveDeptMaster(model);
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

                return RedirectToAction(nameof(DepartmentMasterDashBoard));

            }
            catch (Exception ex)
            {
                // Log and return the error
                LogException<DepartmentMasterController>.WriteException(_logger, ex);
                var ResponseResult = new ResponseResult
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    StatusText = "Error",
                    Result = ex
                };
                return View("Error", ResponseResult);
            }
        }

        public async Task<JsonResult> FillDeptType()
        {
            var JSON = await _IDepartmentMaster.FillDeptType();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }

        public async Task<JsonResult> FillDeptID()
        {
            var JSON = await _IDepartmentMaster.FillDeptID();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }

        [HttpGet]
        public async Task<IActionResult> DepartmentMasterDashBoard()
        {
            try
            {
                var model = new DepartmentMasterModel();
                //model.FromDate = HttpContext.Session.GetString("FromDate");
                //model.ToDate = HttpContext.Session.GetString("ToDate");
                var Result = await _IDepartmentMaster.GetDashBoardData().ConfigureAwait(true);
                if (Result != null)
                {
                    DataSet ds = Result.Result;
                    if (ds != null && ds.Tables.Count > 0)
                    {
                        var dt = ds.Tables[0];
                        model.DepartmentMasterDashBoardGrid = CommonFunc.DataTableToList<DepartmentMasterModel>(dt, "DepartmentMasterDashBoard");
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
            var model = new DepartmentMasterModel();
            model = await _IDepartmentMaster.GetDashBoardDetailData();
            return PartialView("_DepartmentMasterDashBoardGrid", model);
        }

        public async Task<IActionResult> DeleteByID(int ID)
        {
            var Result = await _IDepartmentMaster.DeleteByID(ID);

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

            return RedirectToAction("DepartmentMasterDashBoard");

        }
    }
}
