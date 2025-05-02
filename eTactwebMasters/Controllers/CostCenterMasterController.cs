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
    public class CostCenterMasterController : Controller
    {
        private readonly IDataLogic _IDataLogic;
        public ICostCenterMaster _ICostCenterMaster { get; }
        private readonly ILogger<CostCenterMasterController> _logger;
        private readonly IConfiguration iconfiguration;
        public IWebHostEnvironment _IWebHostEnvironment { get; }
        public CostCenterMasterController(ILogger<CostCenterMasterController> logger, IDataLogic iDataLogic, ICostCenterMaster iCostCenterMaster, EncryptDecrypt encryptDecrypt, IWebHostEnvironment iWebHostEnvironment, IConfiguration iconfiguration)
        {
            _logger = logger;
            _IDataLogic = iDataLogic;
            _ICostCenterMaster = iCostCenterMaster;
            _IWebHostEnvironment = iWebHostEnvironment;
            this.iconfiguration = iconfiguration;
        }
        [Route("{controller}/Index")]
        [HttpGet]
        public async Task<ActionResult> CostCenterMaster(int ID, string Mode,string EntryDate,string CostCenterCode,string CostCenterName,string ShortName,int DeptId,int UnderGroupId,string CC,string Remarks,int CostcentergroupID, string CostcenetrGroupName)
        {
            _logger.LogInformation("\n \n ********** Page Gate Inward ********** \n \n " + _IWebHostEnvironment.EnvironmentName.ToString() + "\n \n");
            TempData.Clear();
            var MainModel = new CostCenterMasterModel();
            MainModel.FromDate = HttpContext.Session.GetString("FromDate");
            MainModel.ToDate = HttpContext.Session.GetString("ToDate");
            MainModel.CC = HttpContext.Session.GetString("Branch");
            MainModel.EntryDate = DateTime.Now.ToString();

            if (!string.IsNullOrEmpty(Mode) && ID > 0 && Mode == "U")
            {
                MainModel = await _ICostCenterMaster.GetViewByID(ID).ConfigureAwait(false);
                MainModel.Mode = Mode; // Set Mode to Update
                MainModel.CostcenterId = ID;
                MainModel.EntryDate = EntryDate;
                MainModel.CostCenterName = CostCenterName;
                MainModel.CostCenterCode = CostCenterCode;
                MainModel.ShortName = ShortName;
                MainModel.DepartmentID = DeptId;
                MainModel.UnderGroupId = UnderGroupId;
                MainModel.Remarks = Remarks;
                MainModel.CC = CC;
                MainModel.CostcentergroupID = CostcentergroupID;
                MainModel.CostcenetrGroupName = CostcenetrGroupName;

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
        public async Task<IActionResult> CostCenterMaster(CostCenterMasterModel model)
        {
            try
            {
                model.CreatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));

                var Result = await _ICostCenterMaster.SaveCostCenterMaster(model);
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

                return RedirectToAction(nameof(CostCenterMasterDashBoard));

            }
            catch (Exception ex)
            {
                // Log and return the error
                LogException<CostCenterMasterController>.WriteException(_logger, ex);
                var ResponseResult = new ResponseResult
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    StatusText = "Error",
                    Result = ex
                };
                return View("Error", ResponseResult);
            }
        }
        public async Task<JsonResult> FillCostCenterID()
        {
            var JSON = await _ICostCenterMaster.FillCostCenterID();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        } 
        public async Task<JsonResult> FillCostCenterGroupName()
        {
            var JSON = await _ICostCenterMaster.FillCostCenterGroupName();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        } 
        public async Task<JsonResult> FillDeptName()
        {
            var JSON = await _ICostCenterMaster.FillDeptName();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillUnderGroupName()
        {
            var JSON = await _ICostCenterMaster.FillUnderGroupName();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        [HttpGet]
        public async Task<IActionResult> CostCenterMasterDashBoard()
        {
            try
            {
                var model = new CostCenterMasterModel();
                model.FromDate = HttpContext.Session.GetString("FromDate");
                model.ToDate = HttpContext.Session.GetString("ToDate");
                var Result = await _ICostCenterMaster.GetDashBoardData().ConfigureAwait(true);
                if (Result != null)
                {
                    DataSet ds = Result.Result;
                    if (ds != null && ds.Tables.Count > 0)
                    {
                        var dt = ds.Tables[0];
                        model.CostCenterMasterGrid = CommonFunc.DataTableToList<CostCenterMasterModel>(dt, "CostCenterMasterDashBoard");
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
            var model = new CostCenterMasterModel();
            model = await _ICostCenterMaster.GetDashBoardDetailData();
            return PartialView("_CostCenterMasterDashBoardGrid", model);
        }
        public async Task<IActionResult> DeleteByID(int ID)
        {
            var Result = await _ICostCenterMaster.DeleteByID(ID);

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

            return RedirectToAction("CostCenterMasterDashBoard");

        }
        public async Task<JsonResult> ChkForDuplicate(string CostCenterName)
        {
            var JSON = await _ICostCenterMaster.ChkForDuplicate(CostCenterName);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }

    }
}
