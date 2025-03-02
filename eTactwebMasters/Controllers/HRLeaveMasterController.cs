using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using static eTactWeb.DOM.Models.Common;
using System.Data;
using System.Net;

namespace eTactwebMasters.Controllers
{
    [Authorize]
    public class HRLeaveMasterController : Controller
    {
          private readonly EncryptDecrypt _EncryptDecrypt;
            private readonly IDataLogic _IDataLogic;
            private readonly IHRLeaveMaster _IHRLeaveMaster;
            private readonly IWebHostEnvironment _IWebHostEnvironment;
            private readonly IMemoryCache _MemoryCache;
            private readonly ILogger<HRLeaveMasterController> _logger;



            public HRLeaveMasterController(IDataLogic iDataLogic, IWebHostEnvironment iWebHostEnvironment, EncryptDecrypt encryptDecrypt, IHRLeaveMaster iHRLeaveMaster, IMemoryCache iMemoryCache, ILogger<HRLeaveMasterController> logger)
            {
                _IDataLogic = iDataLogic;
                _IHRLeaveMaster = iHRLeaveMaster;
                _IWebHostEnvironment = iWebHostEnvironment;
                _EncryptDecrypt = encryptDecrypt;
                _MemoryCache = iMemoryCache;
                _logger = logger;
            }

        public async Task<ActionResult> HRLeaveMaster(int ID, string Mode)//, ILogger logger)
        {
            //_logger.LogInformation("\n \n ********** Page Gate Inward ********** \n \n " + IWebHostEnvironment.EnvironmentName.ToString() + "\n \n");
            TempData.Clear();
            var MainModel = new HRLeaveMasterModel();
            
            MainModel.LeaveId = ID;
            MainModel.Mode = Mode;
            ////MainModel.SalHeadEntryDate = SalHeadEntryDate;
            if (Mode != "U")
            {
                MainModel.CreatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));

            }
            if (!string.IsNullOrEmpty(Mode) && ID > 0 && Mode == "U" || Mode == "V")
            {

                //    //Retrieve the old data by AccountCode and populate the model with existing values
                MainModel = await _IHRLeaveMaster.GetViewByID(ID).ConfigureAwait(false);
                MainModel.Mode = Mode; // Set Mode to Update
                MainModel.LeaveId = ID;


                //    if (Mode == "U")
                //    {
                //        MainModel.LastUpdatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));

                //        MainModel.LastUpdatedOn = HttpContext.Session.GetString("LastUpdatedOn");

                //    }
                MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                SlidingExpiration = TimeSpan.FromMinutes(55),
                Size = 1024
            };
                _MemoryCache.Set("HRLeaveDashboard", MainModel.HRLeaveDashboard, cacheEntryOptions);
            }

            //// If not in "Update" mode, bind new model data
            //else
            //{
            //    // MainModel = await BindModels(MainModel);
            //}



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
                        Value = row["emp_id"].ToString(),
                        Text = row["EmployeeName"].ToString()
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
                    model.CreatedBy = Constants.UserID;

                    var HREmpLeaveMasterTable = new DataTable();
                    var _EmpCategDetail = new List<LeaveEmpCategDetail>();
                    if (model.RestrictedToEmployeeCategory != null)
                    {
                        foreach (var item in model.RestrictedToEmployeeCategory)
                        {
                            var _EmpCateg = new LeaveEmpCategDetail()
                            {

                                CategoryId = item,
                                LeaveId = model.LeaveId


                            };
                            _EmpCategDetail.Add(_EmpCateg);
                        }
                    }
                    HREmpLeaveMasterTable = CommonFunc.ConvertListToTable<LeaveEmpCategDetail>(_EmpCategDetail);

                    var HRSalaryMasterDeptWiseDT = new DataTable();
                    var _DeptWiseCategDetail = new List<LeaveDeptWiseCategDetail>();
                    if (model.RestrictedToDepartment != null)
                    {
                        foreach (var item in model.RestrictedToDepartment)
                        {
                            var _DeptWiseCateg = new LeaveDeptWiseCategDetail()
                            {

                                DeptId = item,
                                LeaveId = model.LeaveId


                            };
                            _DeptWiseCategDetail.Add(_DeptWiseCateg);
                        }
                    }
                    HRSalaryMasterDeptWiseDT = CommonFunc.ConvertListToTable<LeaveDeptWiseCategDetail>(_DeptWiseCategDetail);


                    var HRSalaryMasterLocationWiseDT = new DataTable();
                    var _LocationWiseCategDetail = new List<LeaveLocationDetail>();
                    if (model.RestrictedToDepartment != null)
                    {
                        foreach (var item in model.RestrictedToDepartment)
                        {
                            var _LocationWiseCateg = new LeaveLocationDetail()
                            {

                                LocationId = item,
                                LeaveId = model.LeaveId


                            };
                            _LocationWiseCategDetail.Add(_LocationWiseCateg);
                        }
                    }
                    HRSalaryMasterLocationWiseDT = CommonFunc.ConvertListToTable<LeaveLocationDetail>(_LocationWiseCategDetail);

                    if (model.Mode == "UPDATE")
                    {
                        model.UpdatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));

                    }
                    else
                    {
                        model.UpdatedBy = 0;

                    }
                    var Result = await _IHRLeaveMaster.SaveData(model, HREmpLeaveMasterTable, HRSalaryMasterDeptWiseDT, HRSalaryMasterLocationWiseDT).ConfigureAwait(false);

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
