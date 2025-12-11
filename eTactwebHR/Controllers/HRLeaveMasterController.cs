using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System.Data;
using System.Net;
using static eTactWeb.Data.Common.CommonFunc;
using static eTactWeb.DOM.Models.Common;

namespace eTactwebMasters.Controllers
{
    [Authorize]
    public class HRLeaveMasterController : Controller
    {
            private readonly EncryptDecrypt _EncryptDecrypt;
            private readonly IDataLogic _IDataLogic;
            private readonly IHRLeaveMaster _IHRLeaveMaster;
            private readonly IWebHostEnvironment _IWebHostEnvironment;
            private readonly ILogger<HRLeaveMasterController> _logger;

            public HRLeaveMasterController(IDataLogic iDataLogic, IWebHostEnvironment iWebHostEnvironment, EncryptDecrypt encryptDecrypt, IHRLeaveMaster iHRLeaveMaster, ILogger<HRLeaveMasterController> logger)
            {
                _IDataLogic = iDataLogic;
                _IHRLeaveMaster = iHRLeaveMaster;
                _IWebHostEnvironment = iWebHostEnvironment;
                _EncryptDecrypt = encryptDecrypt;
                _logger = logger;
            }

        public async Task<ActionResult> HRLeaveMaster(int ID, string Mode)//, ILogger logger)
        {
            //_logger.LogInformation("\n \n ********** Page Gate Inward ********** \n \n " + IWebHostEnvironment.EnvironmentName.ToString() + "\n \n");
            TempData.Clear();
            var MainModel = new HRLeaveMasterModel();
            
            MainModel.LeaveId = ID;
            MainModel.Mode = Mode;
            MainModel.CC = HttpContext.Session.GetString("Branch");
            MainModel.LeaveYearcode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
            MainModel.EntryByMachine= Environment.MachineName;
            ////MainModel.SalHeadEntryDate = SalHeadEntryDate;
            if (Mode != "U")
            {
                MainModel.CreatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                MainModel.CreatedByEmpName = HttpContext.Session.GetString("EmpName");

            }
            if (!string.IsNullOrEmpty(Mode) && ID > 0 && Mode == "U" || Mode == "V")
            {

                //    //Retrieve the old data by AccountCode and populate the model with existing values
                MainModel = await _IHRLeaveMaster.GetViewByID(ID).ConfigureAwait(false);
                MainModel.Mode = Mode; // Set Mode to Update
                MainModel.LeaveId = ID;

                if (Mode == "U")
                {
                    MainModel.UpdatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                    MainModel.UpdatedByEmpName = HttpContext.Session.GetString("EmpName");

                    MainModel.UpdatedOn = HttpContext.Session.GetString("LastUpdatedOn");

                }
                HttpContext.Session.SetString("HRLeaveDashboard", JsonConvert.SerializeObject(MainModel.HRLeaveDashboard));
            }

            MainModel = await BindModel(MainModel).ConfigureAwait(false);
            MainModel = await BindModel1(MainModel).ConfigureAwait(false);
            MainModel = await BindModel2(MainModel).ConfigureAwait(false);
            return View(MainModel);
        }

        private async Task<HRLeaveMasterModel> BindModel(HRLeaveMasterModel model)
        {
            var oDataSet = new DataSet();
            var _List = new List<TextValue>();
            oDataSet = await _IHRLeaveMaster.GetEmployeeCategory().ConfigureAwait(true);

            if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
            {
            foreach (DataRow row in oDataSet.Tables[0].Rows)
                {
                    _List.Add(new TextValue
                    {
                        Value = row["CategoryId"].ToString(),
                        Text = row["EmpCateg"].ToString()
                    });
                }
                model.EmpCategList = _List;
                _List = new List<TextValue>();

            }

            return model;
        }

        private async Task<HRLeaveMasterModel> BindModel1(HRLeaveMasterModel model)
        {
            var oDataSet = new DataSet();
            var _List = new List<TextValue>();
            oDataSet = await _IHRLeaveMaster.GetDepartment().ConfigureAwait(true);

            if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in oDataSet.Tables[0].Rows)
                {
                    _List.Add(new TextValue
                    {
                        Value = row["DeptId"].ToString(),
                        Text = row["DeptName"].ToString()
                    });
                }
                model.DeptWiseCategList = _List;
                _List = new List<TextValue>();

            }

            return model;
        }

        private async Task<HRLeaveMasterModel> BindModel2(HRLeaveMasterModel model)
        {
            var oDataSet = new DataSet();
            var _List = new List<TextValue>();
            oDataSet = await _IHRLeaveMaster.GetLocation().ConfigureAwait(true);

            if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in oDataSet.Tables[0].Rows)
                {
                    _List.Add(new TextValue
                    {
                        Value = row["LocationId"].ToString(),
                        Text = row["Location"].ToString()
                    });
                }
                model.LocationList = _List;
                _List = new List<TextValue>();

            }

            return model;
        }

        public async Task<JsonResult> GetleaveType()
        {
            var JSON = await _IHRLeaveMaster.GetleaveType();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }

        public async Task<JsonResult> GetApprovalleval()
        {
            var JSON = await _IHRLeaveMaster.GetApprovalleval();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetLeaveCategory()
        {
            var JSON = await _IHRLeaveMaster.GetLeaveCategory();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }

        public async Task<JsonResult> FillLeaveId()
        {
            var JSON = await _IHRLeaveMaster.FillLeaveId();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetFormRights()
        {
            var userID = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
            var JSON = await _IHRLeaveMaster.GetFormRights(userID);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> HRLeaveMaster(HRLeaveMasterModel model, DataTable HRLeaveMasterDT)
        {
            //Logger.LogInformation("\n \n ********** Save Tax Master ********** \n \n");

            try
            {
                if (model != null)
                {
                    model.Mode = model.Mode == "U" ? "UPDATE" : "INSERT";
                    model.CreatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));

                    var HREmpLeaveMasterTable = new List<string>();
                    var _EmpCategDetail = new List<LeaveEmpCategDetail>();
                    bool v = model.RestrictedToEmployeeCategory != null;
                    if (v)
                    {
                        foreach (var item in model.RestrictedToEmployeeCategory)
                        {
                            var _EmpCateg = new LeaveEmpCategDetail()
                            {

                                CategoryId = item.ToString(),
                                LeaveEntryId = model.LeaveId


                            };
                            _EmpCategDetail.Add(_EmpCateg);
                        }
                    }
                    HREmpLeaveMasterTable = _EmpCategDetail.Select(x => x.CategoryId).ToList();

                    var HRSalaryMasterDeptWiseDT = new List<string>();
                    var _DeptWiseCategDetail = new List<LeaveDeptWiseCategDetail>();
                    if (model.RestrictedToDepartment != null)
                    {
                        foreach (var item in model.RestrictedToDepartment)
                        {
                            var _DeptWiseCateg = new LeaveDeptWiseCategDetail()
                            {

                                DeptId = item.ToString(),
                                LeaveEntryId = model.LeaveId


                            };
                            _DeptWiseCategDetail.Add(_DeptWiseCateg);
                        }
                    }
                    HRSalaryMasterDeptWiseDT = _DeptWiseCategDetail.Select(x => x.DeptId).ToList();

                    var HRLocationDT = new List<string>();
                    var _LocationDetail = new List<LeaveLocationDetail>();
                    if (model.RestrictedToLocation != null)
                    {
                        foreach (var item in model.RestrictedToLocation)
                        {
                            var _Location = new LeaveLocationDetail()
                            {

                                LocationId = item.ToString(),
                                LeaveEntryId = model.LeaveId


                            };
                            _LocationDetail.Add(_Location);
                        }
                    }
                    HRLocationDT = _LocationDetail.Select(x => x.LocationId).ToList();




                    if (model.Mode == "UPDATE")
                    {
                        model.UpdatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));

                    }
                    else
                    {
                        model.UpdatedBy = 0;

                    }
                    model.EntryByMachine = HttpContext.Session.GetString("ClientMachineName");
                    model.IPAddress = HttpContext.Session.GetString("ClientIP");
                    var Result = await _IHRLeaveMaster.SaveData(model, HREmpLeaveMasterTable, HRSalaryMasterDeptWiseDT, HRLocationDT).ConfigureAwait(false);

                    if (Result != null)
                    {
                        if (Result.StatusText == "Success" && Result.StatusCode == HttpStatusCode.OK)
                        {
                            ViewBag.isSuccess = true;
                            TempData["200"] = "200";
                        }
                        if (Result.StatusText == "Updated" && Result.StatusCode == HttpStatusCode.Accepted)
                        {
                            ViewBag.isSuccess = true;
                            TempData["202"] = "202";
                        }
                        if (Result.StatusText == "Error" && Result.StatusCode == HttpStatusCode.InternalServerError)
                        {
                            ViewBag.isSuccess = false;
                            TempData["500"] = "500";
                            // Logger.LogError("\n \n ********** LogError ********** \n " + JsonConvert.SerializeObject(Result) + "\n \n");
                            return View("Error", Result);
                        }
                    }
                }
                return RedirectToAction(nameof(HRLeaveMasterDashBoard));
                
            }
            catch (Exception ex)
            {
                //  LogException<TaxMasterModel>.WriteException((ILogger<TaxMasterModel>)Logger, ex);

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

        public async Task<IActionResult> DeleteByID(int ID, int LeaveYearcode, string EntryByMachine, int CreatedByEmpid, string CreationDate, string LeaveCode)
        {
            EntryByMachine= Environment.MachineName;

            var Result = await _IHRLeaveMaster.DeleteByID( ID,  LeaveYearcode,  EntryByMachine,  CreatedByEmpid,  CreationDate,  LeaveCode);

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
            if (Result.StatusText == "Used" )
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
                        TempData["ErrorMessage"] = Result.Result?.ToString()
                                ?? "Used record cannot be deleted.";
                    }
                }
                else
                {
                    TempData["500"] = "500";
                }


                _logger.LogError("\n \n ********** LogError ********** \n " + JsonConvert.SerializeObject(Result) + "\n \n");
                //model.IsError = "true";
                //return View("Error", Result);
            }
            else
            {
                ViewBag.isSuccess = false;
                TempData["500"] = "500";
            }

            return RedirectToAction("HRLeaveMasterDashBoard");

        }

        [HttpGet]
        public async Task<IActionResult> HRLeaveMasterDashBoard()
        {
            try
            {
                var model = new HRLeaveMasterModel();
                var result = await _IHRLeaveMaster.GetDashboardData().ConfigureAwait(true);

                DateTime now = DateTime.Now;

                if (result != null && result.Result != null)
                {
                    DataSet ds = result.Result;
                    if (ds != null && ds.Tables.Count > 0)
                    {
                        var dt = ds.Tables[0];
                        model.HRLeaveDashboard = CommonFunc.DataTableToList<HRLeaveMasterModel>(dt, "HRLeaveMaster");
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
            //model.Mode = "Search";
            var model = new HRLeaveMasterModel();
            model = await _IHRLeaveMaster.GetDashboardDetailData();
            return PartialView("_HRLeaveMasterDashBoardGrid", model);
        }

    }
}
