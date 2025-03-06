using eTactWeb.Controllers;
using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using static eTactWeb.DOM.Models.Common;
using System.Data;
using System.Net;
using Microsoft.Office.Interop.Excel;

namespace eTactwebMasters.Controllers
{
    public class HRHolidaysMasterController : Controller
    {
        private readonly IDataLogic _IDataLogic;
        public IHRHolidaysMaster _IHRHolidaysMaster { get; }

        private readonly ILogger<HRHolidaysMasterController> _logger;
        private readonly IConfiguration iconfiguration;
        private readonly IMemoryCache _MemoryCache;
        public IWebHostEnvironment _IWebHostEnvironment { get; }
        public HRHolidaysMasterController(ILogger<HRHolidaysMasterController> logger, IDataLogic iDataLogic, IHRHolidaysMaster iHRHolidaysMaster, IMemoryCache iMemoryCache, EncryptDecrypt encryptDecrypt, IWebHostEnvironment iWebHostEnvironment, IConfiguration iconfiguration)
        {
            _logger = logger;
            _IDataLogic = iDataLogic;
            _IHRHolidaysMaster = iHRHolidaysMaster;
            _MemoryCache = iMemoryCache;
            _IWebHostEnvironment = iWebHostEnvironment;
            this.iconfiguration = iconfiguration;
        }

        [Route("{controller}/Index")]
        [HttpGet]
        public async Task<ActionResult> HRHolidaysMaster(int ID, string Mode,int year)
        {
            _logger.LogInformation("\n \n ********** Page Gate Inward ********** \n \n " + _IWebHostEnvironment.EnvironmentName.ToString() + "\n \n");
            TempData.Clear();
            var MainModel = new HRHolidaysMasterModel();
            MainModel.EffectiveFrom = HttpContext.Session.GetString("EffectiveFrom");
            MainModel.HolidayYear = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
            MainModel.Branch = Convert.ToString(HttpContext.Session.GetString("Branch"));

            if (!string.IsNullOrEmpty(Mode) && ID > 0 && Mode == "U" || Mode == "V")
            {
                MainModel = await _IHRHolidaysMaster.GetViewByID(ID,year).ConfigureAwait(false);
                MainModel.Mode = Mode; // Set Mode to Update
                MainModel.HolidayId = ID;
                


                MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                    SlidingExpiration = TimeSpan.FromMinutes(55),
                    Size = 1024
                };
                _MemoryCache.Set("HRHolidayDashboard", MainModel.HRHolidayDashboard, cacheEntryOptions);
            }
            MainModel = await BindModel(MainModel).ConfigureAwait(false);
            MainModel = await BindModel1(MainModel).ConfigureAwait(false);
            return View(MainModel);
        }

        public async Task<JsonResult> GetHolidayType()
        {
            var JSON = await _IHRHolidaysMaster.GetHolidayType();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetHolidayCountry()
        {
            var JSON = await _IHRHolidaysMaster.GetHolidayCountry();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetHolidayState()
        {
            var JSON = await _IHRHolidaysMaster.GetHolidayState();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }

        private async Task<HRHolidaysMasterModel> BindModel(HRHolidaysMasterModel model)
        {
            var oDataSet = new DataSet();
            var _List = new List<TextValue>();
            oDataSet = await _IHRHolidaysMaster.GetEmployeeCategory().ConfigureAwait(true);

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
                model.EmployeeCategoryList = _List;
                _List = new List<TextValue>();

            }

            return model;
        }

        private async Task<HRHolidaysMasterModel> BindModel1(HRHolidaysMasterModel model)
        {
            var oDataSet = new DataSet();
            var _List = new List<TextValue>();
            oDataSet = await _IHRHolidaysMaster.GetDepartment().ConfigureAwait(true);

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
                model.DepartmentList = _List;
                _List = new List<TextValue>();

            }

            return model;
        }

        public async Task<JsonResult> FillEntryId(int yearcode)
        {
            var JSON = await _IHRHolidaysMaster.FillEntryId(yearcode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }

        [HttpGet]
        [Route("HRHolidaysMasterDashBoard")]
        public async Task<IActionResult> HRHolidaysMasterDashBoard()
        {
            try
            {
                var model = new HRHolidaysMasterModel();
                var result = await _IHRHolidaysMaster.GetDashboardData().ConfigureAwait(true);
                DateTime now = DateTime.Now;

                if (result != null && result.Result != null)
                {
                    DataSet ds = result.Result;
                    if (ds != null && ds.Tables.Count > 0)
                    {
                        var dt = ds.Tables[0];
                        model.HRHolidayDashboard = CommonFunc.DataTableToList<HRHolidaysMasterModel>(dt, "HRHolidayMaster");
                    }

                }

                return View(model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IActionResult> GetDetailData(string FromDate,string ToDate)
        {
            //model.Mode = "Search";
            var model = new HRHolidaysMasterModel();
            model = await _IHRHolidaysMaster.GetDashboardDetailData(FromDate,ToDate);
            return PartialView("_HRHolidayMasterDashBoardGrid", model);
        }

        public async Task<IActionResult> DeleteByID(int ID, int year)
        {
            var Result = await _IHRHolidaysMaster.DeleteByID(ID, year);

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

            return RedirectToAction("HRHolidaysMasterDashBoard");

        }


        [HttpPost]
        [Route("{controller}/Index")]

        public async Task<ActionResult> HRHolidaysMaster(HRHolidaysMasterModel model)
        {
            //Logger.LogInformation("\n \n ********** Save Tax Master ********** \n \n");

            try
            {
                if (model != null)
                {
                    model.Mode = model.Mode == "U" ? "update" : "INSERT";
                    model.CreatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                    model.CreatedByEmp = Convert.ToString(HttpContext.Session.GetString("EmpName"));
                    model.Branch = Convert.ToString(HttpContext.Session.GetString("Branch"));

                    var HREmployeeTable = new List<string>();
                    var _EmployeeDetail = new List<HolidayEmployeeCategoryDetail>();
                    bool v = model.EmployeeCategory != null;
                    if (v)
                    {
                        foreach (var item in model.EmployeeCategory)
                        {
                            var _Employee = new HolidayEmployeeCategoryDetail()
                            {

                                CategoryId = item.ToString(),
                                HolidayEntryId = model.HolidayId,
                                HolidayYear = model.HolidayYear,
                                Country = model.Country,
                                StateId = model.StateId,
                                StateName = model.State,
                 
                            };
                            _EmployeeDetail.Add(_Employee);
                        }
                    }
                    HREmployeeTable = _EmployeeDetail.Select(x => x.CategoryId).ToList();

                    var HRDepartmentTable = new List<string>();
                    var _DepartmentDetail = new List<HoliDayDepartmentDetail>();
                    bool v1 = model.Department != null;
                    if (v1)
                    {
                        foreach (var item in model.Department)
                        {
                            var _Department = new HoliDayDepartmentDetail()
                            {

                                DeptId = item.ToString(),
                                HolidayEntryId = model.HolidayId,
                                HolidayYear = model.HolidayYear,
                                Country = model.Country,
                                StateId = model.StateId,
                                StateName = model.State,

                            };
                            _DepartmentDetail.Add(_Department);
                        }
                    }
                    HRDepartmentTable = _DepartmentDetail.Select(x => x.DeptId).ToList();

                    if (model.Mode == "update")
                    {
                        model.UpdatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                        model.UpdatedByEmp = Convert.ToString(HttpContext.Session.GetString("EmpName"));

                    }
                    else
                    {
                        model.UpdatedBy = 0;

                    }
                    var Result = await _IHRHolidaysMaster.SaveData(model, HREmployeeTable, HRDepartmentTable).ConfigureAwait(false);

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
                return RedirectToAction(nameof(HRHolidaysMasterDashBoard));
                //return View(model);

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

        }
    }
}
