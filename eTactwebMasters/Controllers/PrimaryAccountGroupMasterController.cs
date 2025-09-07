using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
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
    public class PrimaryAccountGroupMasterController : Controller
    {
        private readonly IDataLogic _IDataLogic;
        public IPrimaryAccountGroupMaster _IPrimaryAccountGroupMaster { get; }
        private readonly ILogger<PrimaryAccountGroupMasterController> _logger;
        private readonly IConfiguration iconfiguration;
        public IWebHostEnvironment _IWebHostEnvironment { get; }
        public PrimaryAccountGroupMasterController(ILogger<PrimaryAccountGroupMasterController> logger, IDataLogic iDataLogic, IPrimaryAccountGroupMaster IPrimaryAccountGroupMaster, EncryptDecrypt encryptDecrypt, IWebHostEnvironment iWebHostEnvironment, IConfiguration iconfiguration)
        {
            _logger = logger;
            _IDataLogic = iDataLogic;
            _IPrimaryAccountGroupMaster = IPrimaryAccountGroupMaster;
            _IWebHostEnvironment = iWebHostEnvironment;
            this.iconfiguration = iconfiguration;
        }
        [Route("{controller}/Index")]
        [HttpGet]
        public async Task<ActionResult> PrimaryAccountGroupMaster(int EnteredEMPID, string Mode,int Account_Code,string Account_Name,int Parent_Account_Code,string Main_Group,string ParentAccountName, string SubGroup,int SubSubGroup,string UnderGroup)
        { 
            _logger.LogInformation("\n \n ********** Page Gate Inward ********** \n \n " + _IWebHostEnvironment.EnvironmentName.ToString() + "\n \n");

            TempData.Clear();
            var MainModel = new PrimaryAccountGroupMasterModel();
            MainModel.FinFromDate = HttpContext.Session.GetString("FromDate");
            MainModel.FinToDate = HttpContext.Session.GetString("ToDate");
            MainModel.EnteredEMPID = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
            //MainModel.PrefixEntryId= Convert.ToInt32(HttpContext.Session.GetString("UID"));

            HttpContext.Session.Remove("PrimaryAccountGroupMasterGrid");

            if (!string.IsNullOrEmpty(Mode) && EnteredEMPID > 0 && Mode == "U")
            {
                MainModel = await _IPrimaryAccountGroupMaster.GetViewByID(Account_Code).ConfigureAwait(false);


                MainModel.Mode = Mode; // Set Mode to Update
                MainModel.EnteredEMPID = EnteredEMPID;
                MainModel.Account_Name = Account_Name;
                MainModel.Account_Code = Account_Code;
                MainModel.Parent_Account_Code = Parent_Account_Code;
                MainModel.Main_Group = Main_Group;
                MainModel.SubGroup = SubGroup;
                MainModel.SubSubGroup = SubSubGroup;
                MainModel.UnderGroup = UnderGroup;
                MainModel.ParentAccountName = ParentAccountName;

                // You can use caching if required here
                MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                    SlidingExpiration = TimeSpan.FromMinutes(55),
                    Size = 1024
                };
            }



            // When updating the record, make sure to capture updated info
            if (Mode == "U")
            {
                MainModel.UpdatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
            }

            return View(MainModel);


        }
        [Route("{controller}/Index")]
        [HttpPost]
        public async Task<IActionResult> PrimaryAccountGroupMaster(PrimaryAccountGroupMasterModel model)
        {

            try
            {
                //var GIGrid = new DataTable();

                //_MemoryCache.TryGetValue("KeyLedgerOpeningEntryGrid", out List<LedgerOpeningEntryGridModel> LedgerOpeningEntryGrid);

                model.CreatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                //model.EnteredEMPID = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));

                //ResponseResult Result;
                //if (model.Mode == "U")
                //{
                //    Result = await _IPrimaryAccountGroupMaster.UpdatePrimaryAccountGroupMaster(model);
                //}
                //else
                //{
                //    // Call insert method for any other mode
                //    Result = await _IPrimaryAccountGroupMaster.SavePrimaryAccountGroupMaster(model);
                //}
                var Result = await _IPrimaryAccountGroupMaster.SavePrimaryAccountGroupMaster(model);
                if (Result != null)
                {
                    if (Result.StatusText == "Success" && Result.StatusCode == HttpStatusCode.OK)
                    {
                        ViewBag.isSuccess = true;
                        TempData["200"] = "200";
                       // _MemoryCache.Remove("PrimaryAccountGroupMasterGrid");
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
                return RedirectToAction(nameof(PrimaryAccountGroupMasterDashBoard));
            }
            catch (Exception ex)
            {
                // Log and return the error
                LogException<PrimaryAccountGroupMasterController>.WriteException(_logger, ex);
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
            var JSON = await _IPrimaryAccountGroupMaster.GetFormRights(userID);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetParentGroup()
        {
            var JSON = await _IPrimaryAccountGroupMaster.GetParentGroup();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        [HttpGet]
        public async Task<IActionResult> GetAccountGroupDetails(int parentAccountCode)
        {
            var JSON = await _IPrimaryAccountGroupMaster.GetAccountGroupDetailsByParentCode(parentAccountCode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<IActionResult> PrimaryAccountGroupMasterDashBoard(string FromDate = "", string ToDate = "", string Flag = "DASHBOARD")
        {
            try
            {
                HttpContext.Session.Remove("PrimaryAccountGroupMasterGrid");

                var model = new PrimaryAccountGroupMasterDashBoardModel();
                var Result = await _IPrimaryAccountGroupMaster.GetDashboardData().ConfigureAwait(true);  // Adjust interface/service as needed
             
                var now = DateTime.Today;

                model.FromDate = new DateTime(now.Year, now.Month, 1).ToString("dd/MM/yyyy");

                model.ToDate = now.ToString("dd/MM/yyyy");

                if (Result != null)
                {
                    var _List = new List<TextValue>();
                    DataSet DS = Result.Result;

                    if (DS != null)
                    {
                        // Convert DataSet to DataTable with fields matching 'DASHBOARD' SQL query
                        var DT = DS.Tables[0].DefaultView.ToTable(true,
                            "Account_Code", "Account_Name", "Parent_Account_Code", "ParentAccountName",
                            "Main_Group", "SubGroup", "SubSubGroup", "UnderGroup",
                            "Balance_sheet_Category", "Allow_sub_ledger");

                        model.PrimaryAccountGroupMasterDashBoardGrid = CommonFunc.DataTableToList<PrimaryAccountGroupMasterDashBoardModel>(DT, "Primary_Account_Head_Master");
                    }

                    // Custom FromDate and ToDate display based on Flag
                    if (Flag != "DASHBOARD")
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
        public async Task<IActionResult> GetDetailData(string Account_Name, string ParentAccountName)
        {
            var model = new PrimaryAccountGroupMasterDashBoardModel();
            model = await _IPrimaryAccountGroupMaster.GetDashboardDetailData( Account_Name,  ParentAccountName);
            return PartialView("_PrimaryAccountGroupMasterDashBoardGrid", model);

        }
        public async Task<IActionResult> DeleteByID (int AccountCode)
        {
            var Result = await _IPrimaryAccountGroupMaster.DeleteByID( AccountCode);

            if (Result.StatusText == "Success" || Result.StatusCode == HttpStatusCode.Gone)
            {
                ViewBag.isSuccess = true;
                TempData["410"] = "410";
            }
            else if (Result.StatusText == "Error" || Result.StatusCode == HttpStatusCode.Accepted)
            {
                ViewBag.isSuccess = true;
                TempData["423"] = "423";
                ///TempData["ErrorMessage"] = Result.StatusText;
            }
            else if (Result.StatusCode == HttpStatusCode.BadRequest) 
            {
                ViewBag.isSuccess = false;
                TempData["ErrorMessage"] = Result.StatusText; 
            }
            else
            {
                ViewBag.isSuccess = false;
                TempData["500"] = "500";
            }

            return RedirectToAction("PrimaryAccountGroupMasterDashBoard");

        }
    }
}
