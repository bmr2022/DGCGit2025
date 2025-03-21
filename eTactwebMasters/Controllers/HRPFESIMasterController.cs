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
    public class HRPFESIMasterController : Controller
    {
        private readonly EncryptDecrypt _EncryptDecrypt;
        private readonly IDataLogic _IDataLogic;
        private readonly IHRPFESIMaster _IHRPFESIMaster;
        private readonly IWebHostEnvironment _IWebHostEnvironment;
        private readonly IMemoryCache _MemoryCache;
        private readonly ILogger<HRPFESIMasterController> _logger;



        public HRPFESIMasterController(IDataLogic iDataLogic, IWebHostEnvironment iWebHostEnvironment, EncryptDecrypt encryptDecrypt, IHRPFESIMaster iHRPFESIMaster, IMemoryCache iMemoryCache, ILogger<HRPFESIMasterController> logger)
        {
            _IDataLogic = iDataLogic;
            _IHRPFESIMaster = iHRPFESIMaster;
            _IWebHostEnvironment = iWebHostEnvironment;
            _EncryptDecrypt = encryptDecrypt;
            _MemoryCache = iMemoryCache;
            _logger = logger;
        }

        public async Task<ActionResult> HRPFESIMaster(int ID, string Mode)//, ILogger logger)
        {
            //_logger.LogInformation("\n \n ********** Page Gate Inward ********** \n \n " + IWebHostEnvironment.EnvironmentName.ToString() + "\n \n");
            TempData.Clear();
            var MainModel = new HRPFESIMasterModel();

            MainModel.EntryId = ID;
            MainModel.Mode = Mode;

            if (Mode != "U")
            {
                MainModel.CreatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                //MainModel.CreatedByEmpName = HttpContext.Session.GetString("EmpName");

            }
            if (!string.IsNullOrEmpty(Mode) && ID > 0 && Mode == "U" || Mode == "V")
            {


                MainModel = await _IHRPFESIMaster.GetViewByID(ID).ConfigureAwait(false);
                MainModel.Mode = Mode;
                MainModel.EntryId = ID;
                if (Mode == "U")
                {
                    MainModel.UpdatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                    //MainModel.UpdatedByEmpName = HttpContext.Session.GetString("EmpName");

                    MainModel.UpdatedOn = HttpContext.Session.GetString("LastUpdatedOn");

                }
                MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                SlidingExpiration = TimeSpan.FromMinutes(55),
                Size = 1024
            };
            _MemoryCache.Set("HRPFESIDashboard", MainModel.HRPFESIDashboard, cacheEntryOptions);
            }





            MainModel = await BindModel(MainModel).ConfigureAwait(false);
            MainModel = await BindModel1(MainModel).ConfigureAwait(false);
            //MainModel = await BindModel2(MainModel).ConfigureAwait(false);
            return View(MainModel);
        }

        private async Task<HRPFESIMasterModel> BindModel(HRPFESIMasterModel model)
        {
            var oDataSet = new DataSet();
            var _List = new List<TextValue>();
            oDataSet = await _IHRPFESIMaster.GetExemptedCategories().ConfigureAwait(true);

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
                model.ExemptedCategoriesList = _List;
                _List = new List<TextValue>();

            }

            return model;
        }

        private async Task<HRPFESIMasterModel> BindModel1(HRPFESIMasterModel model)
        {
            var oDataSet = new DataSet();
            var _List = new List<TextValue>();
            oDataSet = await _IHRPFESIMaster.GetSalaryHead().ConfigureAwait(true);

            if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in oDataSet.Tables[0].Rows)
                {
                    _List.Add(new TextValue
                    {
                        Value = row["SalHeadEntryId"].ToString(),
                        Text = row["SalaryHead"].ToString()
                    });
                }
                model.ApplicableOnSalaryHeadList = _List;
                _List = new List<TextValue>();

            }

            return model;
        }
        public async Task<JsonResult> GetESIDispensary()
        {
            var JSON = await _IHRPFESIMaster.GetESIDispensary();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }

        public async Task<JsonResult> FillEntryId()
        {
            var JSON = await _IHRPFESIMaster.FillEntryId();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }

        [HttpGet]
        [Route("HRPFESIMasterDashBoard")]
        public async Task<IActionResult> HRPFESIMasterDashBoard()
        {
            try
            {
                var model = new HRPFESIMasterModel();
                var result = await _IHRPFESIMaster.GetDashboardData().ConfigureAwait(true);
                DateTime now = DateTime.Now;

                if (result != null && result.Result != null)
                {
                    DataSet ds = result.Result;
                    if (ds != null && ds.Tables.Count > 0)
                    {
                        var dt = ds.Tables[0];
                        model.HRPFESIDashboard = CommonFunc.DataTableToList<HRPFESIMasterModel>(dt, "HRPFESIMaster");
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
            var model = new HRPFESIMasterModel();
            model = await _IHRPFESIMaster.GetDashboardDetailData();
            return PartialView("_HRPFESIMasterDashBoardGrid", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> HRPFESIMaster(HRPFESIMasterModel model)
        {
            //Logger.LogInformation("\n \n ********** Save Tax Master ********** \n \n");

            try
            {
                if (model != null)
                {
                    model.Mode = model.Mode == "U" ? "Update" : "INSERT";
                    model.CreatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));

                    var HRSalaryHeadTable = new List<string>();
                    var _SalaryHeadDetail = new List<ApplicableOnSalaryHeadDetail>();
                    bool v = model.ApplicableOnSalaryHead != null;
                    if (v)
                    {
                        foreach (var item in model.ApplicableOnSalaryHead)
                        {
                            var _SalaryHead = new ApplicableOnSalaryHeadDetail()
                            {

                                SalHeadEntryId = item.ToString(),
                                PFESIEntryId = model.EntryId,
                                SchemeType=model.SchemeType,



                            };
                            _SalaryHeadDetail.Add(_SalaryHead);
                        }
                    }
                    HRSalaryHeadTable = _SalaryHeadDetail.Select(x => x.SalHeadEntryId).ToList();





                    if (model.Mode == "Update")
                    {
                        model.UpdatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));

                    }
                    else
                    {
                        model.UpdatedBy = 0;

                    }
                    var Result = await _IHRPFESIMaster.SaveData(model, HRSalaryHeadTable).ConfigureAwait(false);

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
                 return RedirectToAction(nameof(HRPFESIMasterDashBoard));
                return View(model);

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

        public async Task<JsonResult> GetFormRights()
        {
            var userID = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
            var JSON = await _IHRPFESIMaster.GetFormRights(userID);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }

        public async Task<IActionResult> DeleteByID(int ID,string machinename)
        {
            var Result = await _IHRPFESIMaster.DeleteByID(ID,machinename);

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

            return RedirectToAction("HRPFESIMasterDashBoard");

        }


    }
}
