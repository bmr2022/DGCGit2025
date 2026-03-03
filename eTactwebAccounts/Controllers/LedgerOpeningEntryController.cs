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
        public EncryptDecrypt EncryptDecrypt { get; }
        public IWebHostEnvironment _IWebHostEnvironment { get; }
        public LedgerOpeningEntryController(ILogger<LedgerOpeningEntryController> logger, IDataLogic iDataLogic, ILedgerOpeningEntry iLedgerOpeningEntry, EncryptDecrypt encryptDecrypt, IWebHostEnvironment iWebHostEnvironment, IConfiguration iconfiguration)
        {
            _logger = logger;
            _IDataLogic = iDataLogic;
            _ILedgerOpeningEntry = iLedgerOpeningEntry;
            _IWebHostEnvironment = iWebHostEnvironment;
            this.iconfiguration = iconfiguration;
            EncryptDecrypt = encryptDecrypt;
        }
        [Route("{controller}/Index")]
        [HttpGet]
        public async Task<ActionResult> LedgerOpeningEntry(int ID, int EntryByEmpId, int YC, string DrCr, string GlobalSearch, string LedgerName, string YearCode, decimal Amount, string GroupName, string Mode, int AccountCode, int GroupAccountCode, string CC, string Account_Name, string FromDate = "", string ToDate = "")
        {
            int userID = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
            var rights = await _ILedgerOpeningEntry.GetFormRights(userID);
            if (rights?.Result == null || rights.Result.Tables.Count == 0 || rights.Result.Tables[0].Rows.Count == 0)
            {
                return RedirectToAction("Dashboard", "Home");
            }


            var table = rights.Result.Tables[0];

            string encID = Request.Query["ID"].ToString();
            string encYC = Request.Query["YC"].ToString();

            if (!string.IsNullOrEmpty(encID) || !string.IsNullOrEmpty(encYC))
            {
                int decryptedID = EncryptDecrypt.DecodeID(encID);
                int decryptedYC = EncryptDecrypt.DecodeID(encYC);
                string decryptedMode = EncryptDecrypt.Decrypt(Mode);

                ID = decryptedID;
                YC = decryptedYC;
                Mode = decryptedMode;

            }

            bool optAll = Convert.ToBoolean(table.Rows[0]["OptAll"]);
            bool optView = Convert.ToBoolean(table.Rows[0]["OptView"]);
            bool optUpdate = Convert.ToBoolean(table.Rows[0]["OptUpdate"]);
            bool optSave = Convert.ToBoolean(table.Rows[0]["OptSave"]);


            if (Mode == "U")
            {
                if (!(optUpdate))
                {
                    return RedirectToAction("Dashboard", "Home");
                }
            }
            else if (Mode == "V")
            {
                if (!(optView))
                {
                    return RedirectToAction("Dashboard", "Home");
                }
            }
            else if (ID <= 0)
            {
                if (!optSave)
                {
                    return RedirectToAction("BankReceiptDashBoard", "BankReceipt");
                }
                //if (!(optAll || optSave))
                //{
                //    return RedirectToAction("Dashboard", "Home");
                //}

            }




            _logger.LogInformation("\n \n ********** Page Gate Inward ********** \n \n " + _IWebHostEnvironment.EnvironmentName.ToString() + "\n \n");

            // Clear TempData and set session variables
            //   TempData.Clear();
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
            if (!string.IsNullOrEmpty(Mode) && AccountCode > 0 && (Mode == "U" || Mode == "V"))
            {
                // Retrieve the old data by AccountCode and populate the model with existing values
                MainModel = await _ILedgerOpeningEntry.GetViewByID(AccountCode).ConfigureAwait(false);
                MainModel.Mode = Mode; // Set Mode to Update
                MainModel.ID = ID;
                MainModel.OpeningForYear = YC;
                MainModel.GroupName = GroupName;
                MainModel.LedgerName = LedgerName;
                MainModel.Amount = Amount;
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
                model.EntryByMachineName = HttpContext.Session.GetString("ClientMachineName");
                model.IPAddress = HttpContext.Session.GetString("ClientIP");
                int userID = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
                string modelJson = HttpContext.Session.GetString("KeyLedgerOpeningEntryGrid");
                List<LedgerOpeningEntryGridModel> LedgerOpeningEntryGrid = new List<LedgerOpeningEntryGridModel>();
                if (!string.IsNullOrEmpty(modelJson))
                {
                    LedgerOpeningEntryGrid = JsonConvert.DeserializeObject<List<LedgerOpeningEntryGridModel>>(modelJson);
                }

                model.Createdby = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));

                var Result = await _ILedgerOpeningEntry.SaveWorkOrderProcess(model);
                if (Result != null)
                {
                    if (Result.StatusText == "Success" && Result.StatusCode == HttpStatusCode.OK)
                    {
                        ViewBag.isSuccess = true;
                        TempData["200"] = "200";
                        TempData.Keep("200");
                        HttpContext.Session.Remove("KeyLedgerOpeningEntryGrid");
                        ViewBag.isSuccess = true;
                        TempData["200"] = "200";
                        return Json(new
                        {
                            success = true,
                            message = "Data saved successfully",
                            redirectUrl = Url.Action(
          "LedgerOpeningEntry",
          "LedgerOpeningEntry"

      )
                        });
                    }
                    else if (Result.StatusText == "Success" && Result.StatusCode == HttpStatusCode.Accepted)
                    {
                        ViewBag.isSuccess = true;
                        TempData["202"] = "202";
                        ViewBag.isSuccess = true;
                        TempData["200"] = "200";
                        return Json(new
                        {
                            success = true,
                            message = "Data saved successfully",
                            redirectUrl = Url.Action(
          "LedgerOpeningEntry",
          "LedgerOpeningEntry"

      )
                        });
                    }
                    else if (Result.StatusText == "Error" && Result.StatusCode == HttpStatusCode.InternalServerError)
                    {
                        ViewBag.isSuccess = false;
                        TempData["500"] = "500";
                        _logger.LogError($"\n \n ********** LogError ********** \n {JsonConvert.SerializeObject(Result)}\n \n");
                        return Json(new
                        {
                            success = false,
                            message = "An unexpected error occurred."
                        });
                        //return View("Error", Result);
                    }
                    else if (!string.IsNullOrEmpty(Result.StatusText))
                    {
                        // If SP returned a message (like adjustment error)
                        // TempData["ErrorMessage"] = Result.StatusText;
                        //return View(model);
                        return Json(new
                        {
                            success = false,
                            message = Result.StatusText
                        });

                    }
                    else
                    {
                        TempData["ErrorMessage"] = Result.StatusText;
                    }
                }

                // return RedirectToAction(nameof(LedgerOpeningEntry));
                return Json(new
                {
                    success = false,
                    message = "An unexpected error occurred."
                });

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
                int userID = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
                var rights = await _ILedgerOpeningEntry.GetFormRights(userID);
                if (rights?.Result == null || rights.Result.Tables.Count == 0 || rights.Result.Tables[0].Rows.Count == 0)
                {
                    return RedirectToAction("Dashboard", "Home");
                }
                var table = rights.Result.Tables[0];

                bool optAll = Convert.ToBoolean(table.Rows[0]["OptAll"]);
                bool optView = Convert.ToBoolean(table.Rows[0]["OptView"]);
                bool optUpdate = Convert.ToBoolean(table.Rows[0]["OptUpdate"]);
                bool optDelete = Convert.ToBoolean(table.Rows[0]["OptDelete"]);
                if (!(optAll || optView || optUpdate || optDelete))
                {
                    return RedirectToAction("Dashboard", "Home");
                }
                HttpContext.Session.Remove("KeyLedgerOpeningEntryGrid");
                var model = new LedgerOpeningEntryDashBoardModel();
                model.OpeningForYear = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
                //model.DashboardType = "SUMMARY";
                var Result = await _ILedgerOpeningEntry.GetDashboardData(userID).ConfigureAwait(true);
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
            int userID = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
            //model.Mode = "Search";
            var model = new LedgerOpeningEntryDashBoardGridModel();
            model = await _ILedgerOpeningEntry.GetDashboardDetailData(GroupName, userID, LedgerName, PreviousAmount, DrCr);
            return PartialView("_LedgerOpeningEntryDashBoardGrid", model);
        }
        public async Task<IActionResult> DeleteByID(int YC, int AC, string EntryByMachine = "", string FromDate = "", string ToDate = "", string Searchbox = "", string GroupName = "", string LedgerName = "", string ClosingYearCode = "", string DrCr = "", string Amount = "", string CC = "", string EntryByEmployee = "", string ActualEntryDate = "", string UpdatedByEmployee = "", string Updationdate = "")
        {
            int userID = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
            var Result = await _ILedgerOpeningEntry.DeleteByID(AC, YC, userID);

            if (Result.StatusText == "Success" || Result.StatusCode == HttpStatusCode.Gone)
            {
                ViewBag.isSuccess = true;
                TempData["410"] = "410";
            }
            else if (!string.IsNullOrEmpty(Result.StatusText))
            {
                // If SP returned a message (like adjustment error)
                ViewBag.isSuccess = false;
                TempData["ErrorMessage"] = Result.StatusText;

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

        public async Task<IActionResult> ImportAndUpdateLedgerOpening()
        {
            int userID = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
            var rights = await _ILedgerOpeningEntry.GetFormRights(userID);
            if (rights?.Result == null || rights.Result.Tables.Count == 0 || rights.Result.Tables[0].Rows.Count == 0)
            {
                return RedirectToAction("Dashboard", "Home");
            }
            var table = rights.Result.Tables[0];
            bool optAll = Convert.ToBoolean(table.Rows[0]["OptAll"]);
            bool optSave = Convert.ToBoolean(table.Rows[0]["OptSave"]);
            bool optUpdate = Convert.ToBoolean(table.Rows[0]["OptUpdate"]);

            if (!(optAll || optUpdate || optSave))
            {
                return RedirectToAction("Dashboard", "Home");
            }
            var model = new LedgerOpeningEntryModel();
            return View(model);
        }


        public async Task<IActionResult> UpdateFromExcel([FromBody] ExcelUpdateRequest request)
        {
            var response = new ResponseResult();
            var flag = request.Flag;
            var YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
            var CloseingYearCode = YearCode - 1;
            var MachineName = HttpContext.Session.GetString("ClientMachineName");
            var IPAddress = HttpContext.Session.GetString("ClientIP");
            var CC = HttpContext.Session.GetString("Branch");
            var EntryByEmpId = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));

            try
            {
                DataTable dt = new DataTable();

                // Define columns based on SQL table
                dt.Columns.Add("Account_Name", typeof(string));
                dt.Columns.Add("ParentAccountCode", typeof(int));
                dt.Columns.Add("DrCr", typeof(string));
                dt.Columns.Add("Amount", typeof(decimal));

                List<string> errorList = new List<string>();
                int rowIndex = 1;

                foreach (var excelRow in request.ExcelData)
                {
                    DataRow row = dt.NewRow();
                    bool hasError = false;
                    string accountNameForError = "";


                    foreach (var map in request.Mapping)
                    {
                        string dbCol = map.Key;      // DB column name
                        string excelCol = map.Value; // Excel column name
                        

                        object value = DBNull.Value;

                        if (excelRow.ContainsKey(excelCol) && !string.IsNullOrEmpty(excelRow[excelCol]))
                        {
                            value = excelRow[excelCol];
                            Type columnType = dt.Columns[dbCol].DataType;

                            try
                            {
                                if (dbCol == "Account_Name")
                                {
                                    string Account_Name = value.ToString().Trim();
                                    int AccountCode = 0;
                                    int ParentAccountCode = 0;
                                    var AccCode = _ILedgerOpeningEntry.GetAccountCodeandParentAccountCode(Account_Name);

                                    if (AccCode.Result.Result != null && AccCode.Result.Result.Rows.Count > 0)
                                    {
                                        AccountCode = (int)AccCode.Result.Result.Rows[0].ItemArray[0];
                                        ParentAccountCode = (int)AccCode.Result.Result.Rows[0].ItemArray[1];
                                    }

                                    if (AccountCode != 0)

                                    {
                                        value = AccountCode;
                                        row["ParentAccountCode"] = ParentAccountCode;
                                    }

                                    else
                                    {
                                        errorList.Add($"Row {rowIndex} - Account '{Account_Name}' not found.");
                                        hasError = true;
                                        break;
                                    }
                                }



                                if (columnType == typeof(int))
                                    value = int.Parse(value.ToString());
                                else if (columnType == typeof(decimal))
                                    value = decimal.Parse(value.ToString());
                                else if (columnType == typeof(bool))
                                {
                                    string s = value.ToString().Trim().ToLower();
                                    value = (s == "1" || s == "true" || s == "y");
                                }
                                else if (columnType == typeof(DateTime))
                                    value = DateTime.Parse(value.ToString());
                                else
                                    value = value.ToString();
                            }
                            catch
                            {
                                errorList.Add($"Row {rowIndex} - Invalid value for column '{dbCol}' in Account '{accountNameForError}'.");
                                hasError = true;
                                break;
                            }
                        }

                        row[dbCol] = value;
                    }

                    //dt.Rows.Add(row);
                    if (!hasError)
                        dt.Rows.Add(row);

                    rowIndex++;
                }

                // Pass to repository/service layer
                if (dt.Rows.Count > 0)
                {
                    response = await _ILedgerOpeningEntry
                        .UpdateMultipleDataFromExcel(dt, flag, CloseingYearCode,
                            MachineName, IPAddress, CC, EntryByEmpId);
                }
                if (response != null)
                {
                    if ((response.StatusText == "Success" || response.StatusText == "Updated") &&
                        (response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.Accepted))
                    {
                        return Json(new
                        {
                            StatusCode = 200,
                            StatusText = "Process completed",
                            SuccessCount = dt.Rows.Count,
                            ErrorList = errorList,
                            RedirectUrl = Url.Action("ImportAndUpdateLedgerOpening",
        "LedgerOpeningEntry", new { Flag = "" })
                        });
                    }
                    else
                    {
                        return Json(new
                        {

                            StatusText = response.StatusText,
                            StatusCode = 201,
                            RedirectUrl = ""
                        });
                    }
                }

                return Json(new
                {
                    StatusCode = 500,
                    StatusText = "Unknown error occurred"
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

    }
}
