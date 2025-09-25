using eTactWeb.Data.Common;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System.Runtime.Caching;
using static eTactWeb.Data.Common.CommonFunc;
using static eTactWeb.DOM.Models.Common;
using eTactWeb.DOM.Models;
using System.Net;
using System.Data;
using System.Globalization;
using eTactWeb.Services.Interface;

namespace eTactWeb.Controllers
{
    public class LedgerOpeningEntryController : Controller
    {
        private readonly IDataLogic _IDataLogic;
        public ILedgerOpeningEntry _ILedgerOpeningEntry { get; }
        private readonly ILogger<LedgerOpeningEntryController> _logger;
        private readonly IConfiguration iconfiguration;
        public IWebHostEnvironment _IWebHostEnvironment { get; }
        public LedgerOpeningEntryController(ILogger<LedgerOpeningEntryController> logger, IDataLogic iDataLogic, ILedgerOpeningEntry iLedgerOpeningEntry, EncryptDecrypt encryptDecrypt, IWebHostEnvironment iWebHostEnvironment, IConfiguration iconfiguration)
        {
            _logger = logger;
            _IDataLogic = iDataLogic;
            _ILedgerOpeningEntry = iLedgerOpeningEntry;
            _IWebHostEnvironment = iWebHostEnvironment;
            this.iconfiguration = iconfiguration;
        }
        [Route("{controller}/Index")]
        [HttpGet]
        public async Task<ActionResult> LedgerOpeningEntry(int ID, int EntryByEmpId, int YC, string DrCr, string GlobalSearch, string LedgerName, string YearCode, float Amount, string GroupName, string Mode, int AccountCode, int GroupAccountCode, string CC, string Account_Name, string FromDate = "", string ToDate = "")
        {
            _logger.LogInformation("\n \n ********** Page Gate Inward ********** \n \n " + _IWebHostEnvironment.EnvironmentName.ToString() + "\n \n");

            // Clear TempData and set session variables
            TempData.Clear();
            var MainModel = new LedgerOpeningEntryModel();
            MainModel.FinFromDate = HttpContext.Session.GetString("FromDate");
            MainModel.FinToDate = HttpContext.Session.GetString("ToDate");
            MainModel.OpeningForYear = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
            MainModel.CreatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
            MainModel.EntryByEmpId = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
            MainModel.CC = HttpContext.Session.GetString("Branch");
            MainModel.EntryByEmpName = HttpContext.Session.GetString("EmpName");
            MainModel.ActualEntryDate = DateTime.Now.ToString();
            int currentYear = DateTime.Now.Year;
            int currentMonth = DateTime.Now.Month;
            int financialYearStart = (currentMonth < 4) ? currentYear - 1 : currentYear;
            MainModel.ClosingYearCode = financialYearStart - 1;
            //MainModel.ClosingYearCode = DateTime.Now.Year - 1;
            HttpContext.Session.Remove("KeyLedgerOpeningEntryGrid");

            // Check if Mode is "Update" (U) and the ID is valid
            if (!string.IsNullOrEmpty(Mode) && AccountCode > 0 && Mode == "U")
            {
                // Retrieve the old data by AccountCode and populate the model with existing values
                MainModel = await _ILedgerOpeningEntry.GetViewByID(AccountCode).ConfigureAwait(false);
                MainModel.Mode = Mode; // Set Mode to Update
                MainModel.ID = ID;
                MainModel.OpeningForYear = YC;
                MainModel.GroupName = GroupName;
                MainModel.LedgerName = LedgerName;
                MainModel.Amount =Amount;
                MainModel.GroupAccountCode = GroupAccountCode;
                MainModel.AccountCode = AccountCode;

                MainModel = await BindModels(MainModel).ConfigureAwait(false);
                MainModel.FinFromDate = HttpContext.Session.GetString("FromDate");
                MainModel.FinToDate = HttpContext.Session.GetString("ToDate");
            }

            // If not in "Update" mode, bind new model data
            else
            {
                MainModel = await BindModels(MainModel);
            }

            // When updating the record, make sure to capture updated info
            if (Mode == "U")
            {
                MainModel.UpdatedByEmpId = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                MainModel.UpdateByEmpName = HttpContext.Session.GetString("EmpName");
                MainModel.Updationdate = DateTime.Now.ToString();
            }

            return View(MainModel); // Pass the model with old data to the view
        }

        [Route("{controller}/Index")]
        [HttpPost]
        public async Task<IActionResult> LedgerOpeningEntry(LedgerOpeningEntryModel model)
        {
            try
            {
                string modelJson = HttpContext.Session.GetString("KeyLedgerOpeningEntryGrid");
                List<LedgerOpeningEntryGridModel> LedgerOpeningEntryGrid = new List<LedgerOpeningEntryGridModel>();
                if (!string.IsNullOrEmpty(modelJson))
                {
                    LedgerOpeningEntryGrid = JsonConvert.DeserializeObject<List<LedgerOpeningEntryGridModel>>(modelJson);
                }

                model.CreatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));

                var Result = await _ILedgerOpeningEntry.SaveWorkOrderProcess(model);
                if (Result != null)
                {
                    if (Result.StatusText == "Success" && Result.StatusCode == HttpStatusCode.OK)
                    {
                        ViewBag.isSuccess = true;
                        TempData["200"] = "200";
                        TempData.Keep("200");
                        HttpContext.Session.Remove("KeyLedgerOpeningEntryGrid");
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

                return RedirectToAction(nameof(LedgerOpeningEntry));

            }
            catch (Exception ex)
            {
                // Log and return the error
                LogException<LedgerOpeningEntryController>.WriteException(_logger, ex);
                var ResponseResult = new ResponseResult
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    StatusText = "Error",
                    Result = ex
                };
                return View("Error", ResponseResult);
            }
        }
        public async Task<JsonResult> GetFormRights()
        {
            var userID = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
            var JSON = await _ILedgerOpeningEntry.GetFormRights(userID);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }


        public async Task<LedgerOpeningEntryModel> BindModels(LedgerOpeningEntryModel model)
        {
            if (model == null)
            {
                model = new LedgerOpeningEntryModel();

                model.OpeningForYear = Constants.FinincialYear;

            }

            return model;
        }
        public async Task<JsonResult> GetGroupByAccountCode(int AccountCode)
        {
            var JSON = await _ILedgerOpeningEntry.GetGroupByAccountCode(AccountCode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetAmountAndType(int AccountCode, int OpeningForYear, string ActualEntryDate)
        {
            var JSON = await _ILedgerOpeningEntry.GetAmountAndType(AccountCode, OpeningForYear, ActualEntryDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetLedgersByGroup(string groupAccountCode)
        {
            var JSON = await _ILedgerOpeningEntry.GetLedgersByGroup(groupAccountCode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetLedgersALLGroup()
        {
            var JSON = await _ILedgerOpeningEntry.GetLedgersALLGroup();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetAllGroupName()
        {
            var JSON = await _ILedgerOpeningEntry.GetAllGroupName();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }


        private static DataTable GetDetailTable(IList<LedgerOpeningEntryGridModel> DetailList)
        {
            try
            {
                var GIGrid = new DataTable();

                GIGrid.Columns.Add("GroupName", typeof(string));
                GIGrid.Columns.Add("LedgerName", typeof(string));
                GIGrid.Columns.Add("OpeningForYear", typeof(int));
                GIGrid.Columns.Add("SrNO", typeof(int));
                GIGrid.Columns.Add("PreviousAmount", typeof(float));
                GIGrid.Columns.Add("DrCr", typeof(string));

                foreach (var Item in DetailList)
                {
                    GIGrid.Rows.Add(
                        new object[]
                        {
             Item.GroupName == null ? "" : Item.GroupName,
             Item.LedgerName == null ? "" : Item.LedgerName,
             Item.OpeningForYear == 0 ? 0 : Item.OpeningForYear,
             Item.SrNO == 0 ? 0 : Item.SrNO,
             Item.PreviousAmount == 0 ?0:Item.PreviousAmount,
             Item.DrCr== null ? "":Item.DrCr,


                        });
                }
                GIGrid.Dispose();
                return GIGrid;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        //DashBoard
        [HttpGet]
        public async Task<IActionResult> LedgerOpeningEntryDashBoard(string FromDate = "", string ToDate = "", string Flag = "True")
        {
            try
            {
                HttpContext.Session.Remove("KeyLedgerOpeningEntryGrid");
                var model = new LedgerOpeningEntryDashBoardModel();
                model.OpeningForYear = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
                //model.DashboardType = "SUMMARY";
                var Result = await _ILedgerOpeningEntry.GetDashboardData().ConfigureAwait(true);
                DateTime now = DateTime.Now;

                model.FromDate = new DateTime(now.Year, now.Month, 1).ToString("dd/MM/yyyy").Replace("-", "/");
                model.ToDate = new DateTime(DateTime.Today.Year + 1, 3, 31).ToString("dd/MM/yyyy").Replace("-", "/");

                if (Result != null)
                {
                    var _List = new List<TextValue>();
                    DataSet DS = Result.Result;
                    if (DS != null)
                    {
                        var DT = DS.Tables[0].DefaultView.ToTable(true, "EntryByEmpId",
                            "GroupName", "LedgerName", "ClosingYearCode", "AccountCode", "DrCr",
                            "Amount", "CC", "EntryByEmployee", "ActualEntryDate",
                            "UpdatedByEmployee", "GroupAccountCode", "Updationdate", "EntryByMachine");

                        model.LedgerOpeningEntryDashBoardGrid = CommonFunc.DataTableToList<LedgerOpeningEntryDashBoardGridModel>(DT, "AccLedgerOpening");
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
        public async Task<IActionResult> GetDetailData(string GroupName, string LedgerName, float PreviousAmount, string DrCr)
        {
            //model.Mode = "Search";
            var model = new LedgerOpeningEntryDashBoardGridModel();
            model = await _ILedgerOpeningEntry.GetDashboardDetailData(GroupName, LedgerName, PreviousAmount, DrCr);
            return PartialView("_LedgerOpeningEntryDashBoardGrid", model);
        }
        public async Task<IActionResult> DeleteByID(int YC, int AC, string EntryByMachine = "", string FromDate = "", string ToDate = "", string Searchbox = "", string GroupName = "", string LedgerName = "", string ClosingYearCode = "", string DrCr = "", string Amount = "", string CC = "", string EntryByEmployee = "", string ActualEntryDate = "", string UpdatedByEmployee = "", string Updationdate = "")
        {
            var Result = await _ILedgerOpeningEntry.DeleteByID(AC, YC);

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

            return RedirectToAction("LedgerOpeningEntryDashBoard", new { Flag = "False", FromDate = FromDate, ToDate = ToDate, GroupName = GroupName, LedgerName = LedgerName, Searchbox = Searchbox, ClosingYearCode = ClosingYearCode, EntryByMachine = EntryByMachine, DrCr = DrCr, Amount = Amount, CC = CC, EntryByEmployee = EntryByEmployee, ActualEntryDate = ActualEntryDate, UpdatedByEmployee = UpdatedByEmployee, Updationdate = Updationdate });

        }
    }
}
