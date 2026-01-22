using eTactWeb.Data.BLL;
using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.DOM.Models.Master;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System.Data;
using System.Net;
using static eTactWeb.Data.Common.CommonFunc;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Controllers
{
    public class SubVoucherController : Controller
    {
        private readonly IDataLogic _IDataLogic;
        public ISubVoucher _ISubVoucher { get; }
        private readonly ILogger<SubVoucherController> _logger;
        private readonly IConfiguration iconfiguration;
        public IWebHostEnvironment _IWebHostEnvironment { get; }
        public SubVoucherController(ILogger<SubVoucherController> logger, IDataLogic iDataLogic, ISubVoucher iSubVoucher, EncryptDecrypt encryptDecrypt, IWebHostEnvironment iWebHostEnvironment, IConfiguration iconfiguration)
        {
            _logger = logger;
            _IDataLogic = iDataLogic;
            _ISubVoucher = iSubVoucher;
            _IWebHostEnvironment = iWebHostEnvironment;
            this.iconfiguration = iconfiguration;
        }
        [Route("SubVoucher/Index")]
        [HttpGet]
        public async Task<ActionResult> SubVoucher(int PrefixEntryId, int YC, string MainVoucherTableName, string MainVoucherName, string GlobalSearch, string LedgerName, string YearCode, float Amount, string GroupName, string Mode, int AccountCode, int GroupAccountCode, string CC, string Account_Name, string FromDate = "", string ToDate = "")
        {
            _logger.LogInformation("\n \n ********** Page Gate Inward ********** \n \n " + _IWebHostEnvironment.EnvironmentName.ToString() + "\n \n");

            TempData.Clear();
            var MainModel = new SubVoucherModel();
            MainModel.FinFromDate = HttpContext.Session.GetString("FromDate");
            MainModel.FinToDate = HttpContext.Session.GetString("ToDate");
            MainModel.ActualEntryBy = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
            MainModel.EntryByEmpName = HttpContext.Session.GetString("EmpName");
            HttpContext.Session.Remove("KeySubVoucherGrid");

            if (!string.IsNullOrEmpty(Mode) && PrefixEntryId > 0 && (Mode == "U" || Mode == "V"))
            {
                MainModel = await _ISubVoucher.GetViewByID(PrefixEntryId, MainVoucherName, MainVoucherTableName).ConfigureAwait(false);
                MainModel.Mode = Mode; // Set Mode to Update
                MainModel.ID = PrefixEntryId;
                MainModel.MainVoucherName = MainVoucherName;
                MainModel.MainVoucherTableName = MainVoucherTableName;
                if (!string.IsNullOrWhiteSpace(MainModel.SelectedEmployeeIds))
                {
                    MainModel.EmployeeName = MainModel.SelectedEmployeeIds
                        .Split(',', StringSplitOptions.RemoveEmptyEntries)
                        .Select(x => Convert.ToInt32(x))
                        .ToList();
                }
                else
                {
                    MainModel.EmployeeName = new List<int>();
                }
                MainModel.FinFromDate = HttpContext.Session.GetString("FromDate");
                MainModel.FinToDate = HttpContext.Session.GetString("ToDate");
                MainModel.UpdatedBy = Convert.ToInt32(HttpContext.Session.GetString("ID"));
                MainModel.UpdationDate = DateTime.Now.ToString();

            }
            // When updating the record, make sure to capture updated info
            if (Mode == "U")
            {
                MainModel.UpdatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                MainModel.UpdationDate = DateTime.Now.ToString();
            }

            return View(MainModel);
        }

        [Route("{controller}/Index")]
        [HttpPost]
        public async Task<IActionResult> SubVoucher(SubVoucherModel model)
        {

            try
            {
                //var GIGrid = new DataTable();

                //_MemoryCache.TryGetValue("KeyLedgerOpeningEntryGrid", out List<LedgerOpeningEntryGridModel> LedgerOpeningEntryGrid);

                model.CreatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));

                if (model.EmployeeName != null && model.EmployeeName.Count > 0)
                {
                    model.SelectedEmployeeIds = string.Join(",", model.EmployeeName);
                }
                else
                {
                    model.SelectedEmployeeIds = null;
                }



                var Result = await _ISubVoucher.SaveSubVoucher(model);
                if (Result != null)
                {
                    if (Result.StatusText == "Success" && Result.StatusCode == HttpStatusCode.OK)
                    {
                        ViewBag.isSuccess = true;
                        TempData["200"] = "200";
                        HttpContext.Session.Remove("KeySubVoucherGrid");
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
                return RedirectToAction(nameof(SubVoucherDashBoard));
            }
            catch (Exception ex)
            {
                // Log and return the error
                LogException<SubVoucherController>.WriteException(_logger, ex);
                var ResponseResult = new ResponseResult
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    StatusText = "Error",
                    Result = ex
                };
                return View("Error", ResponseResult);
            }
        }
        public async Task<JsonResult> GetMainVoucherNames()
        {
            var JSON = await _ISubVoucher.GetMainVoucherNames();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }

        public async Task<JsonResult> GetEmployeeList()
        {
            var JSON = await _ISubVoucher.GetEmployeeList();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetPriceFrom()
        {
            var JSON = await _ISubVoucher.GetPriceFrom();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetTableName(string MainVoucherName)
        {
            var JSON = await _ISubVoucher.GetTableName(MainVoucherName);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        //DashBoard
        public async Task<IActionResult> SubVoucherDashBoard(string FromDate = "", string ToDate = "", string Flag = "True")
        {
            try
            {
                HttpContext.Session.Remove("SubVoucherGrid");

                var model = new SubVoucherDashBoardModel();
                //model.DashboardType = "SUMMARY";
                var Result = await _ISubVoucher.GetDashboardData().ConfigureAwait(true);
                DateTime now = DateTime.Now;

                model.FromDate = new DateTime(now.Year, now.Month, 1).ToString("dd/MM/yyyy").Replace("-", "/");
                model.ToDate = new DateTime(DateTime.Today.Year + 1, 3, 31).ToString("dd/MM/yyyy").Replace("-", "/");

                if (Result != null)
                {
                    var _List = new List<TextValue>();
                    DataSet DS = Result.Result;
                    if (DS != null)
                    {
                        var DT = DS.Tables[0].DefaultView.ToTable(true, "PrefixEntryId",
                            "MainVoucherName", "MainVoucherTableName", "SubVoucherName", "VoucherRotationType",
                            "StartSubVouchDiffSeries", "SubVouchPrefix", "FromYearPrefix", "ToYearPreFix",
                            "MonthPrefix", "DayPrefix", "SeparatorApplicable", "Separator", "TotalLength", "ActualEntryBy",
                            "ActualEntryDate", "UpdatedBy", "UpdationDate", "EntryByMachine");

                        model.SubVoucherDashBoardGrid = CommonFunc.DataTableToList<SubVoucherDashBoardGridModel>(DT, "SubVoucherPrefixSetting");
                    }

                    if (Flag != "True")
                    {
                        model.FromDate1 = FromDate;
                        model.ToDate1 = ToDate;
                        return View(model);
                    }
                }
                return View(model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IActionResult> GetDetailData()
        {
            var model = new SubVoucherDashBoardGridModel();
            model = await _ISubVoucher.GetDashboardDetailData();
            return PartialView("_SubVoucherDashBoardGrid", model);
        }

        public async Task<IActionResult> DeleteByID(string MainVoucherName, string MainVoucherTableName, string StartSubVouchDiffSeries, int ActualEntryBy, int UpdatedBy, int PrefixEntryId)
        {
            var Result = await _ISubVoucher.DeleteByID(MainVoucherName, MainVoucherTableName, StartSubVouchDiffSeries, ActualEntryBy, UpdatedBy, PrefixEntryId);

            if (Result.StatusText == "Success" || Result.StatusCode == HttpStatusCode.Gone)
            {
                ViewBag.isSuccess = true;
                TempData["410"] = "410";
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

            return RedirectToAction("SubVoucherDashBoard", new { Flag = "False", MainVoucherName = MainVoucherName, MainVoucherTableName = MainVoucherTableName, StartSubVouchDiffSeries = StartSubVouchDiffSeries, ActualEntryBy = ActualEntryBy, UpdatedBy = UpdatedBy, PrefixEntryId = PrefixEntryId });

        }
        //[HttpPost]
        //public async Task<IActionResult> UpdateSubVoucherPrefixSetting(SubVoucherDashBoardModel model)
        //{

        //        try
        //        {
        //        model.CreatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
        //        model.ActualEntryBy = HttpContext.Session.GetString("EmpID");
        //        model.UpdatedBy = HttpContext.Session.GetString("EmpID");
        //        model.UpdationDate = DateTime.Now;
        //        var result = await _ISubVoucher.UpdateSubVoucherPrefixSetting(model);
        //            if (result.StatusText == "Success")
        //            {
        //                return Json(new { status = "Updated Successfully" });
        //            }
        //            else
        //            {
        //                return Json(new { status = "Unsuccess", message = result.StatusText });
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            return Json(new { status = "Error", message = ex.Message });
        //        }

        //    return Json(new { status = "Invalid Model" });
        //}
        //public async Task<IActionResult> GetPrefixId(string mainVoucherName)
        //{
        //    // Await the asynchronous BLL method to get the PrefixEntryId
        //    var prefixEntryId = await _ISubVoucher.GetPrefixEntryIdByVoucherName(mainVoucherName);

        //    // Create the model with the retrieved PrefixEntryId
        //    var model = new SubVoucherModel
        //    {
        //        PrefixEntryId = prefixEntryId
        //    };

        //    // Return the view with the model
        //    return View(model);
        //}
        //public async Task<int> GetPrefixId(string mainVoucherName)
        //{
        //    // Await the asynchronous BLL method to get the PrefixEntryId
        //    var prefixEntryId = await _ISubVoucher.GetPrefixEntryIdByVoucherName(mainVoucherName);

        //    // Return the PrefixEntryId directly as an integer
        //    return prefixEntryId;
        //}
    }
}
