using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.DOM.Models.Master;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using static eTactWeb.Data.Common.CommonFunc;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Controllers
{
    public class VendorUserController : Controller
    {
        private readonly ILogger<VendorUserController> _logger;
        private readonly IVendorMater _IVendorMater;
        public IWebHostEnvironment _IWebHostEnvironment { get; }

        public VendorUserController(ILogger<VendorUserController> logger, IWebHostEnvironment iWebHostEnvironment, IVendorMater iVendorMater)
        {
            _logger = logger;
            _IWebHostEnvironment = iWebHostEnvironment;
            _IVendorMater = iVendorMater;
        }

        // databasename and servername,vendempname

        [Route("{controller}/Index")]
        [HttpGet]
        public async Task<ActionResult> VendorUser(int ID, string Mode)
        {
            _logger.LogInformation("\n \n ********** Page Gate Inward ********** \n \n " + _IWebHostEnvironment.EnvironmentName.ToString() + "\n \n");
            //TempData.Clear();
            var MainModel = new VendorUserModel();

            MainModel.FinFromDate = HttpContext.Session.GetString("FromDate");
            MainModel.FinToDate = HttpContext.Session.GetString("ToDate");
            MainModel.BranchName = HttpContext.Session.GetString("Branch");
            MainModel.ActualEntryBy = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
            MainModel.ActualEntryByName = HttpContext.Session.GetString("EmpName");

            if (!string.IsNullOrEmpty(Mode) && ID > 0 && (Mode == "U"|| Mode=="V"))
            {
                MainModel = await _IVendorMater.GetViewByID(ID,Mode).ConfigureAwait(false);
                MainModel.Mode = Mode; // Set Mode to Update
                MainModel.ID = ID;

                MainModel.LastUpdatedBy = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
                MainModel.UpdatedByName = HttpContext.Session.GetString("EmpName");

            }

            return View(MainModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("{controller}/Index")]
        public async Task<IActionResult> VendorUser(VendorUserModel model)
        {
            try
            {
                model.ActualEntryBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));

                if (model.Mode == "U")
                {
                    model.UpdatedByName = HttpContext.Session.GetString("EmpName");
                    model.LastUpdatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                }
                var Result = await _IVendorMater.SaveVendorUser(model);

                if (Result != null)
                {
                    //if (Result.StatusText == "Success" && Result.StatusCode == HttpStatusCode.OK)
                    //{
                    //    ViewBag.isSuccess = true;
                    //    TempData["200"] = "200";
                    //    var modelSuccess = new VendorUserModel();
                    //    return RedirectToAction("VendorUser", modelSuccess);
                    //}

                    if (Result.StatusCode == HttpStatusCode.OK)
                    {
                        if (Result.StatusText == "Success")
                        {
                            TempData["Message"] = "Vendor created successfully!";
                            return RedirectToAction("Index", "VendorUser");
                        }
                        else
                        {
                            TempData["422"] = Result.StatusText;
                            return RedirectToAction("Index", "VendorUser");
                        }
                    }

                    if (Result.StatusText == "Updated" && Result.StatusCode == HttpStatusCode.Accepted)
                    {
                        ViewBag.isSuccess = true;
                        TempData["202"] = "202";
                        var modelUpdate = new VendorUserModel();
                        return RedirectToAction("VendorUser", modelUpdate);
                    }
                    if (Result.StatusText == "Error" && Result.StatusCode == HttpStatusCode.InternalServerError)
                    {
                        ViewBag.isSuccess = false;
                        var input = "";
                        if (Result != null)
                        {
                            input = Result.Result.ToString();
                            int index = input.IndexOf("#ERROR_MESSAGE");

                            if (index != -1)
                            {
                                string errorMessage = input.Substring(index + "#ERROR_MESSAGE :".Length).Trim();
                                TempData["ErrorMessage"] = errorMessage;
                            }
                            else
                            {
                                TempData["500"] = "500";
                            }
                        }
                        else
                        {
                            TempData["500"] = "500";
                        }

                        _logger.LogError("\n \n ********** LogError ********** \n " + JsonConvert.SerializeObject(Result) + "\n \n");
                        
                        return View(model);
                    }

                    ModelState.Clear();
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                LogException<VendorUserController>.WriteException(_logger, ex);

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
        public async Task<JsonResult> FillEntryId()
        {
            var JSON = await _IVendorMater.FillEntryId();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> ViewDataByVendor(int accountCode)
        {
            var JSON = await _IVendorMater.ViewDataByVendor(accountCode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillVendorList(string isShowAll)
        {
            var JSON = await _IVendorMater.FillVendorList(isShowAll);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        
        public async Task<JsonResult> CheckUserDuplication(int userId)
        {
            var JSON = await _IVendorMater.CheckUserDuplication(userId);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }

        public async Task<IActionResult> VUDashboard()
        {
            try
            {
                var model = new VendorUserDashboard();

                var Result = await _IVendorMater.GetDashboardData().ConfigureAwait(true);
                if (Result != null)
                {
                    var _List = new List<TextValue>();
                    DataSet DS = Result.Result;
                    if (DS != null)
                    {
                        var DT = DS.Tables[0].DefaultView.ToTable(true, "UserEntryId", "AccountCode", "AccountName", "UserId", "Password", "Active", "AllowTodelete", "AllowtoUpdate"
                                , "rightsForReport", "RightsForPurchaseModule", "RightsForQCmodule", "RightsforAccountModule"
                                 , "AdminUser", "ourServerName", "databaseName", "BranchName", "ActualEntryBy", "ActualEntryDate", "SaleBillPrefix", "VendorEmpName"
                                 , "LastUpdationdate", "EntryByMachineName", "ActualEntryBYName", "UpdatedByName", "LastUpdatedBy");

                        model.VendorUserDashboards = CommonFunc.DataTableToList<VendorUserDashboard>(DT, "VendorUserDashboard");
                    }
                }
                //model.FinFromDate = ParseFormattedDate(fromDate);
                //model.FinToDate = ParseFormattedDate(toDate);
                //model.PartCode = partCode;
                //model.ItemName = itemName;
                //model.AccountName = accountName;
                //model.ProdSchNo = prodSchNo;
                //model.WONo = wono;
                //model.SummaryDetail = summaryDetail;
                //model.SearchBox = searchBox;
                return View(model);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public async Task<IActionResult> GetSearchData(string accountName, int userId)
        {
            var model = new VendorUserDashboard();
            var Result = await _IVendorMater.GetDashboardData(accountName,userId).ConfigureAwait(true);
            if (Result != null)
            {
                var _List = new List<TextValue>();
                DataSet DS = Result.Result;
                if (DS != null)
                {
                    var DT = DS.Tables[0].DefaultView.ToTable(true, "UserEntryId", "AccountCode", "AccountName", "UserId", "Password", "Active", "AllowTodelete", "AllowtoUpdate"
                            , "rightsForReport", "RightsForPurchaseModule", "RightsForQCmodule", "RightsforAccountModule"
                             , "AdminUser", "ourServerName", "databaseName", "BranchName", "ActualEntryBy", "ActualEntryDate", "SaleBillPrefix", "VendorEmpName"
                             , "LastUpdationdate", "EntryByMachineName", "ActualEntryBYName", "UpdatedByName", "LastUpdatedBy");

                    model.VendorUserDashboards = CommonFunc.DataTableToList<VendorUserDashboard>(DT, "VendorUserDashboard");
                }
            }
            return PartialView("_VendorUserDashboardGrid", model);
        }

        public async Task<IActionResult> DeleteByID(int userEntryId, int accountCode,int userId, string entryByMachineName, int actualEntryBy, string actualEntryDate)
        {
            var Result = await _IVendorMater.DeleteByID(userEntryId,accountCode,userId,entryByMachineName,actualEntryBy,actualEntryDate).ConfigureAwait(false);

            if (Result.StatusText == "Success" || Result.StatusText == "deleted" || Result.StatusCode == HttpStatusCode.Gone)
            {
                ViewBag.isSuccess = true;
                TempData["410"] = "410";
            }
            else if (Result.StatusText == "Error" || Result.StatusCode == HttpStatusCode.Accepted)
            {
                ViewBag.isSuccess = true;
                //  TempData["423"] = "423";
                TempData["DeleteMessage"] = Result.StatusText;

            }
            else
            {
                ViewBag.isSuccess = false;
                TempData["500"] = "500";

            }

            return RedirectToAction("VUDashboard");
        }

    }
}
