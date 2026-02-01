using eTactWeb.Data.Common;
using eTactWeb.Services.Interface;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using static eTactWeb.Data.Common.CommonFunc;
using static eTactWeb.DOM.Models.Common;
using eTactWeb.DOM.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Data;
using Microsoft.AspNetCore.Authorization;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Spreadsheet;
using PdfSharp.Drawing.BarCodes;

namespace eTactWeb.Controllers
{
    [Authorize]
    public class TaxMasterController : Controller
    {
        private static readonly Action<ILogger, string, Exception> _loggerMessage = LoggerMessage.Define<string>(LogLevel.Error, eventId: new EventId(id: 0, name: "ERROR"), formatString: "{Message}");
        public EncryptDecrypt EncryptDecrypt { get; }
        private readonly IDataLogic IDataLogic;
        private readonly ITaxMaster ITaxMaster;
        private readonly ILogger<TaxMasterController> Logger;

        public TaxMasterController(ITaxMaster iTaxMaster, IDataLogic iDataLogic, ILogger<TaxMasterController> logger, EncryptDecrypt encryptDecrypt)
        {
            IDataLogic = iDataLogic;
            ITaxMaster = iTaxMaster;
            Logger = logger;
            EncryptDecrypt = encryptDecrypt;
        }

        [HttpGet]
        // GET: TaxMasterController/Dashboard
        [Route("{controller}/Dashboard")]
        public async Task<ActionResult> DashBoard()
        {
            int userID = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
            var rights = await ITaxMaster.GetFormRights(userID);
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
            Logger.LogInformation("\n \n ********** Dashboard Page Tax Master ********** \n \n");

            var model = new TaxMasterDashboard();

            try
            {
                model.TMDashboard = await ITaxMaster.GetDashBoardData(userID);
            }
            catch (Exception ex)
            {
                var ResponseResult = new ResponseResult()
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    StatusText = "Something Went Wrong....Please Try Again Later....!",
                    Result = ex
                };

                return View("Error", ResponseResult);
            }
            return View(model);
        }
        public async Task<JsonResult> FillHSN(string taxCategory)
        {
            var JSON = await ITaxMaster.FillHSN(taxCategory);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }

        [HttpGet]
        public async Task<IActionResult> DeleteByID(int ID)
        {
            var IsDelete = IDataLogic.IsDelete(ID, "ACCOUNT");

            if (IsDelete == 0)
            {
                int CreatedBy = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
                var Result = await ITaxMaster.DeleteByID(ID, CreatedBy).ConfigureAwait(false);

                if (Result.StatusText == "Deleted" || Result.StatusCode == HttpStatusCode.Gone)
                {
                    ViewBag.isSuccess = true;
                    TempData["410"] = "410";
                }
                else if (!string.IsNullOrEmpty(Result.StatusText))
                {
                    // If SP returned a message (like adjustment error)
                    TempData["ErrorMessage"] = Result.StatusText;
                    //return View(model);
                }
            }
            else
            {
                ViewBag.isSuccess = false;
                TempData["423"] = "423";
            }

            return RedirectToAction(nameof(DashBoard));
        }
        public async Task<IActionResult> GetSearchData(string TaxName, string TaxType, string HSNNo)
        {
            int userID = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
            TaxMasterDashboard model = new TaxMasterDashboard();
            TaxName = string.IsNullOrEmpty(TaxName) ? "" : TaxName.Trim();
            TaxType = string.IsNullOrEmpty(TaxType) ? "" : TaxType.Trim();
            HSNNo = string.IsNullOrEmpty(HSNNo) ? "" : HSNNo.Trim();
            model.TMDashboard = await ITaxMaster.GetSearchData(TaxName, TaxType, HSNNo, userID);
            return PartialView("_TMDashboardGrid", model);
        }
        [HttpPost]
        public async Task<IActionResult> FillSGST(decimal taxPercent)
        {
            var response = await ITaxMaster.GetSGSTByTaxPercent(taxPercent);

            if (response?.Result == null)
                return Json(new List<object>());

            DataSet ds = response.Result as DataSet;
            if (ds == null || ds.Tables.Count == 0)
                return Json(new List<object>());

            DataTable table = ds.Tables[0];

            var result = table.AsEnumerable()
                .Select(r => new
                {
                    value = r["Account_Code"].ToString(),
                    text = r["Tax_Name"].ToString()
                })
                .ToList();

            return Json(result);
        }

        [HttpGet]
        // GET: TaxMasterController/TaxMaster
        //[Route("{controller}/Index")]
        public async Task<ActionResult> TaxMaster(int ID, string Mode, int YC)
        {
            int userID = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
            var rights = await ITaxMaster.GetFormRights(userID);
            if (rights?.Result == null || rights.Result.Tables.Count == 0 || rights.Result.Tables[0].Rows.Count == 0)
            {
                return RedirectToAction("Dashboard", "Home");
            }


            var table = rights.Result.Tables[0];

            // string encID = Request.Query["ID"].ToString();
            string encID = RouteData.Values["id"]?.ToString();
            //  string encYC = Request.Query["YearCode"].ToString();

            if (!string.IsNullOrEmpty(encID) && encID != "0" && !string.IsNullOrEmpty(Mode))
            {
                int decryptedID = EncryptDecrypt.DecodeID(encID);
                // int decryptedYC = EncryptDecrypt.DecodeID(encYC);
                string decryptedMode = EncryptDecrypt.Decrypt(Mode);

                ID = decryptedID;
                // YearCode = decryptedYC;
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
                    return RedirectToAction("DashBoard", "TaxMaster");
                }
                //if (!(optAll || optSave))
                //{
                //    return RedirectToAction("Dashboard", "Home");
                //}

            }




            Logger.LogInformation("\n \n ********** Page Tax Master ********** \n \n");

            var model = new TaxMasterModel();

            if (ID > 0 && (Mode == "V" || Mode == "U"))
            {
                model = await ITaxMaster.ViewByID(ID);
                model.UpdatedBy = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
                model.UpdatedByName = HttpContext.Session.GetString("EmpID");
                model.UpdatedOn = DateTime.Now;
            }

            model.ID = ID;
            model.Mode = Mode;

            if (model.Mode != "U" || model.Mode != "V")
            {

                model.CreatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                model.CreatedByName = HttpContext.Session.GetString("EmpName");
                model.CreatedOn = DateTime.Now;
            }




            model = await BindModel(model).ConfigureAwait(false);

            return View(model);
        }

        // POST: TaxMasterController/TaxMaster
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> TaxMaster(TaxMasterModel model)
        {
            Logger.LogInformation("\n \n ********** Save Tax Master ********** \n \n");

            try
            {
                if (model != null)
                {
                    //model.Mode = model.Mode == "U" ? "UPDATE" : "INSERT";
                    //model.CreatedBy = Constants.UserID;

                    var TaxMasterDT = new DataTable();
                    var _HSNDetail = new List<HSNDetail>();
                    if (model.HSN != null)
                    {
                        foreach (var item in model.HSN)
                        {
                            var _HSN = new HSNDetail()
                            {
                                AccountCode = model.EntryID,
                                HSNNO = item,
                                TaxCategory = model.TaxCategory,
                                TaxPercent = model.TaxPercent
                            };
                            _HSNDetail.Add(_HSN);
                        }
                    }
                    TaxMasterDT = CommonFunc.ConvertListToTable<HSNDetail>(_HSNDetail);
                    if (model.Mode == "U")
                    {
                        model.UpdatedBy = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
                        model.UpdatedByName = HttpContext.Session.GetString("EmpName");
                    }
                    else
                    {
                        model.UpdatedBy = 0;

                    }
                    model.EntryByMachineName = HttpContext.Session.GetString("ClientMachineName");
                    model.IPAddress = HttpContext.Session.GetString("ClientIP");
                    var Result = await ITaxMaster.SaveTaxMaster(model, TaxMasterDT).ConfigureAwait(false);

                    if (Result != null)
                    {
                        if (Result.StatusText == "Success" && Result.StatusCode == HttpStatusCode.OK)
                        {
                            ViewBag.isSuccess = true;
                            TempData["200"] = "200";
                        }

                        else if (Result.StatusText == "Updated" && Result.StatusCode == HttpStatusCode.Accepted)
                        {
                            ViewBag.isSuccess = true;
                            TempData["202"] = "202";

                        }

                        else if (!string.IsNullOrEmpty(Result.StatusText))
                        {
                            // Set TempData message
                            TempData["ErrorMessage"] = Result.StatusText;
                            model.Mode = model.Mode == "UPDATE" ? "U" : "I";
                            // Rebind dropdowns / multi-select lists
                            model = await BindModel(model);
                            return View(model);
                        }
                        else if (Result.StatusText == "Error" && Result.StatusCode == HttpStatusCode.InternalServerError)
                        {
                            ViewBag.isSuccess = false;
                            TempData["500"] = "500";
                            Logger.LogError("\n \n ********** LogError ********** \n " + JsonConvert.SerializeObject(Result) + "\n \n");

                        }
                        else
                        {
                            TempData["ErrorMessage"] = "Error while deleting transaction.";
                        }

                    }
                }

                return RedirectToAction(nameof(DashBoard));
            }
            catch (Exception ex)
            {
                LogException<TaxMasterModel>.WriteException((ILogger<TaxMasterModel>)Logger, ex);

                var ResponseResult = new ResponseResult()
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    StatusText = "Error",
                    Result = ex
                };

                return View("Error", ResponseResult);
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetHSNByTax(string taxName, decimal taxPercent)
        {
            var model = new TaxMasterModel();
            var list = new List<TextValue>();

            var ds = await ITaxMaster.BindAllDropDown(taxName, taxPercent);

            if (ds.Tables.Count > 1)
            {
                foreach (DataRow row in ds.Tables[1].Rows)
                {
                    list.Add(new TextValue
                    {
                        Value = row["HsnNo"].ToString(),
                        Text = row["HsnNo"].ToString()
                    });
                }
            }

            return Json(list);
        }


        private async Task<TaxMasterModel> BindModel(TaxMasterModel model)
        {
            var oDataSet = new DataSet();
            var _List = new List<TextValue>();
            oDataSet = await ITaxMaster.BindAllDropDown(model.TaxName,model.TaxPercent).ConfigureAwait(true);

            if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in oDataSet.Tables[0].Rows)
                {
                    _List.Add(new TextValue
                    {
                        Value = row["TaxID"].ToString(),
                        Text = row["TaxType"].ToString()
                    });
                }
                model.TaxTypeList = _List;
                _List = new List<TextValue>();

                foreach (DataRow row in oDataSet.Tables[1].Rows)
                {
                    _List.Add(new TextValue
                    {
                        Value = row["HsnNo"].ToString(),
                        Text = row["HsnNo"].ToString()
                    });
                }
                model.HSNList = _List;
                _List = new List<TextValue>();

                foreach (DataRow row in oDataSet.Tables[2].Rows)
                {
                    _List.Add(new TextValue
                    {
                        Value = row["Account_Code"].ToString(),
                        Text = row["Account_Name"].ToString()
                    });
                }
                model.ParentGroupList = _List;
                _List = new List<TextValue>();

                //foreach (DataRow row in oDataSet.Tables[3].Rows)
                //{
                //    _List.Add(new TextValue
                //    {
                //        Value = row["Account_Code"].ToString(),
                //        Text = row["Tax_Name"].ToString()
                //    });
                //}
                //model.SGSTHeadList = _List;
                //_List = new List<TextValue>();
            }

            if (string.IsNullOrEmpty(model.Mode) && model.ID == 0)
            {
                model.EntryID = IDataLogic.GetEntryID("Account_Head_Master", Constants.FinincialYear, "EntryID", "yearcode");
                model.EffectiveDate = new DateOnly(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day).ToString().Replace("-", "/");
            }
            return model;
        }

        public async Task<JsonResult> GetFormRights()
        {
            var userID = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
            var JSON = await ITaxMaster.GetFormRights(userID);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
    }
}