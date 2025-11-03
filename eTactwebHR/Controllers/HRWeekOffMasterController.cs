using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
//using Microsoft.Office.Interop.Excel;
using Newtonsoft.Json;
using System.Data;
using System.Net;
using static eTactWeb.Data.Common.CommonFunc;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Controllers
{
    public class HRWeekOffMasterController : Controller
    {
        private readonly IDataLogic _IDataLogic;
        public IHRWeekOffMaster _IHRWeekOffMaster { get; }
        private readonly ILogger<HRWeekOffMasterController> _logger;
        private readonly IConfiguration iconfiguration;
        public IWebHostEnvironment _IWebHostEnvironment { get; }
        public HRWeekOffMasterController(ILogger<HRWeekOffMasterController> logger, IDataLogic iDataLogic, IHRWeekOffMaster iHRWeekOffMaster, EncryptDecrypt encryptDecrypt, IWebHostEnvironment iWebHostEnvironment, IConfiguration iconfiguration)
        {
            _logger = logger;
            _IDataLogic = iDataLogic;
            _IHRWeekOffMaster = iHRWeekOffMaster;
            _IWebHostEnvironment = iWebHostEnvironment;
            this.iconfiguration = iconfiguration;
        }

        [Route("{controller}/Index")]
        [HttpGet]
        public async Task<ActionResult> HRWeekOffMaster(int ID, string Mode,string EmpCateName)
        {
            _logger.LogInformation("\n \n ********** Page Gate Inward ********** \n \n " + _IWebHostEnvironment.EnvironmentName.ToString() + "\n \n");
            TempData.Clear();
            var MainModel = new HRWeekOffMasterModel();
            MainModel.EffectiveFrom = HttpContext.Session.GetString("EffectiveFrom");
            MainModel.WeekoffYearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));

            if (!string.IsNullOrEmpty(Mode) && ID > 0 && Mode == "U"|| Mode=="V")
            {
                MainModel = await _IHRWeekOffMaster.GetViewByID(ID).ConfigureAwait(false);
                MainModel.Mode = Mode; // Set Mode to Update
                MainModel.WeekoffEntryId = ID;
                MainModel.EmpCateName = EmpCateName;


                MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                    SlidingExpiration = TimeSpan.FromMinutes(55),
                    Size = 1024
                };
            }
            MainModel = await BindModel(MainModel).ConfigureAwait(false);
            return View(MainModel);
        }

        public async Task<JsonResult> GetEmpCat()
        {
            var JSON = await _IHRWeekOffMaster.GetEmpCat();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }

        public async Task<JsonResult> GetDeptCat()
        {
            var JSON = await _IHRWeekOffMaster.GetDeptCat();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }



        public async Task<JsonResult> FillEntryId()
        {
            var JSON = await _IHRWeekOffMaster.FillEntryId();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }

        [HttpPost]
        [Route("{controller}/Index")]
        public async Task<ActionResult> HRWeekOffMaster(HRWeekOffMasterModel model)
        {
            //Logger.LogInformation("\n \n ********** Save Tax Master ********** \n \n");

            try
            {
                if (model != null)
                {
                    model.Mode = model.Mode == "U" ? "update" : "INSERT";
                    model.EntryByEmpId = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                    var HREmployeeTable = new List<string>();
                    var _EmployeeDetail = new List<HRWeekOffMasterModel>();
                    bool v = model.EmpCateg != null;
                    if (v)
                    {
                        foreach (var item in model.EmpCateg)
                        {
                            var _Employee = new HRWeekOffMasterModel()
                            {

                                EmpCategoryId = item.ToString(),
                                WeekoffEntryId = model.WeekoffEntryId,
                                WeekoffYearCode = model.WeekoffEntryId,
                                WeekoffName = model.WeekoffName,
                                //StateId = model.StateId,
                                //StateName = model.State,

                            };
                            _EmployeeDetail.Add(_Employee);
                        }
                    }
                    HREmployeeTable = _EmployeeDetail.Select(x => x.EmpCategoryId).ToList();



                    if (model.Mode == "update")
                    {
                        model.UpdatedbyId = Convert.ToInt32(HttpContext.Session.GetString("UID"));

                    }
                    else
                    {
                        model.UpdatedBy = 0;

                    }
                    var Result = await _IHRWeekOffMaster.SaveData(model, HREmployeeTable).ConfigureAwait(false);

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
                return RedirectToAction(nameof(HRWeekOffMasterDashBoard));
               
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
        private async Task<HRWeekOffMasterModel> BindModel(HRWeekOffMasterModel model)
        {
            var oDataSet = new DataSet();
            var _List = new List<TextValue>();
            oDataSet = await _IHRWeekOffMaster.GetEmpCat().ConfigureAwait(true);

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
        [HttpGet]
        public async Task<IActionResult> HRWeekOffMasterDashBoard()
        {
            try
            {
                var model = new HRWeekOffMasterModel();
                var result = await _IHRWeekOffMaster.GetDashboardData().ConfigureAwait(true);
                DateTime now = DateTime.Now;



                if (result != null && result.Result != null)
                {
                    DataSet ds = result.Result;
                    if (ds != null && ds.Tables.Count > 0)
                    {
                        var dt = ds.Tables[0];
                        model.HRWeekOffMasterDashBoard = CommonFunc.DataTableToList<HRWeekOffMasterModel>(dt, "HRWeekOffMaster");
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
            var model = new HRWeekOffMasterModel();
            model = await _IHRWeekOffMaster.GetDashboardDetailData();
            return PartialView("_HRWeekOffMasterDashBBoardGrid", model);
        }

        public async Task<IActionResult> DeleteByID(int ID)
        {
            var Result = await _IHRWeekOffMaster.DeleteByID(ID);

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

            return RedirectToAction("HRWeekOffMasterDashBoard");

        }





    }
}
