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

            //if (Mode != "U")
            //{
            //    MainModel.CreatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
            //    MainModel.CreatedByEmpName = HttpContext.Session.GetString("EmpName");

            //}
            //if (!string.IsNullOrEmpty(Mode) && ID > 0 && Mode == "U" || Mode == "V")
            //{


            //    //MainModel = await _IHRPFESIMaster.GetViewByID(ID).ConfigureAwait(false);
            //    //MainModel.Mode = Mode; 
            //    //MainModel.LeaveId = ID;


            //    if (Mode == "U")
            //    {
            //        MainModel.UpdatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
            //        MainModel.UpdatedByEmpName = HttpContext.Session.GetString("EmpName");

            //        MainModel.UpdatedOn = HttpContext.Session.GetString("LastUpdatedOn");

            //    }
            MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                SlidingExpiration = TimeSpan.FromMinutes(55),
                Size = 1024
            };
            //_MemoryCache.Set("HRLeaveDashboard", MainModel.HRLeaveDashboard, cacheEntryOptions);
            //}





            MainModel = await BindModel(MainModel).ConfigureAwait(false);
            //MainModel = await BindModel1(MainModel).ConfigureAwait(false);
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> HRPFESIMaster(HRPFESIMasterModel model)
        {
            //Logger.LogInformation("\n \n ********** Save Tax Master ********** \n \n");

            try
            {
                if (model != null)
                {
                    model.Mode = model.Mode == "U" ? "UPDATE" : "INSERT";
                    model.CreatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));

                    




                    if (model.Mode == "UPDATE")
                    {
                        model.UpdatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));

                    }
                    else
                    {
                        model.UpdatedBy = 0;

                    }
                    var Result = await _IHRPFESIMaster.SaveData(model).ConfigureAwait(false);

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
                // return RedirectToAction(nameof(HRLeaveMasterDashBoard));
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
    }
}
