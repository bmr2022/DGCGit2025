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

namespace eTactWeb.Controllers
{
    [Authorize]
    public class HRSalaryHeadMasterController : Controller
    {
        private readonly EncryptDecrypt _EncryptDecrypt;
        private readonly IDataLogic _IDataLogic;
        private readonly IHRSalaryHeadMaster _ISalaryHeadMaster;
        private readonly IWebHostEnvironment _IWebHostEnvironment;
        private readonly ILogger<HRSalaryHeadMasterController> _logger;

        public HRSalaryHeadMasterController(IDataLogic iDataLogic, IWebHostEnvironment iWebHostEnvironment, EncryptDecrypt encryptDecrypt, IHRSalaryHeadMaster iSalaryHeadMaster, ILogger<HRSalaryHeadMasterController> logger)
        {
            _IDataLogic = iDataLogic;
            _ISalaryHeadMaster = iSalaryHeadMaster;
            _IWebHostEnvironment = iWebHostEnvironment;
            _EncryptDecrypt = encryptDecrypt;
            _logger = logger;
        }
        
        public async Task<ActionResult> HRSalaryHeadMaster(int ID, string Mode)//, ILogger logger)
        {
            //_logger.LogInformation("\n \n ********** Page Gate Inward ********** \n \n " + IWebHostEnvironment.EnvironmentName.ToString() + "\n \n");
            //TempData.Clear();
            if (TempData["Error"] != null)
            {
                ViewBag.Error = TempData["Error"].ToString();
            }

            var MainModel = new HRSalaryHeadMasterModel();
            MainModel.SalHeadEntryId = ID;
            MainModel.Mode = Mode;
            if (Mode != "U")
            {
                MainModel.ActualEntryby = Convert.ToInt32(HttpContext.Session.GetString("UID"));
            }
            if (!string.IsNullOrEmpty(Mode) && ID > 0 && Mode == "U"|| Mode == "V")
            {
                MainModel = await _ISalaryHeadMaster.GetViewByID(ID).ConfigureAwait(false);
                MainModel.Mode = Mode; // Set Mode to Update
                MainModel.SalHeadEntryId = ID;
               
                if (Mode == "U")
                {
                    MainModel.LastUpdatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                    MainModel.LastUpdatedOn = HttpContext.Session.GetString("LastUpdatedOn");
                }               
                HttpContext.Session.SetString("HRSalaryDashboard", JsonConvert.SerializeObject(MainModel.HRSalaryDashboard));
            }

            else
            {
                // MainModel = await BindModels(MainModel);
            }

            MainModel = await BindModel(MainModel).ConfigureAwait(false);
            MainModel = await BindModel1(MainModel).ConfigureAwait(false);
            return View(MainModel);
        }

        private async Task<HRSalaryHeadMasterModel> BindModel(HRSalaryHeadMasterModel model)
        {
            var oDataSet = new DataSet();
            var _List = new List<TextValue>();
            oDataSet = await _ISalaryHeadMaster.GetEmployeeCategory().ConfigureAwait(true);

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

        private async Task<HRSalaryHeadMasterModel> BindModel1(HRSalaryHeadMasterModel model)
        {
            var oDataSet = new DataSet();
            var _List = new List<TextValue>();
            oDataSet = await _ISalaryHeadMaster.GetDepartment().ConfigureAwait(true);

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

        public async Task<JsonResult> CheckBeforeDelete(int SalHeadEntryId)
        {
            var JSON = await _ISalaryHeadMaster.CheckBeforeDelete(SalHeadEntryId);
            _logger.LogError(JsonConvert.SerializeObject(JSON));
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }

        public async Task<JsonResult> ChkForDuplicateHeadName(string SalaryHead, int SalHeadEntryId)
        {
            var JSON = await _ISalaryHeadMaster.ChkForDuplicateHeadName( SalaryHead,  SalHeadEntryId);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }

        [HttpGet]
        public async Task<IActionResult> DashBoard()
        {
            try
            {
                var model = new HRSalaryHeadMasterModel();
                var result = await _ISalaryHeadMaster.GetDashboardData().ConfigureAwait(true);
                DateTime now = DateTime.Now;

                

                if (result != null && result.Result != null)
                {
                    DataSet ds = result.Result;
                    if (ds != null && ds.Tables.Count > 0)
                    {
                        var dt = ds.Tables[0];
                        model.HRSalaryDashboard = CommonFunc.DataTableToList<HRSalaryHeadMasterModel>(dt, "HRSalaryHeadMaster");
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
            var model = new HRSalaryHeadMasterModel();
            model = await _ISalaryHeadMaster.GetDashboardDetailData();
            return PartialView("_HRSalaryDashBoardGrid", model);
        }
        public async Task<IActionResult> DeleteByID(int ID)
        {
            var Result = await _ISalaryHeadMaster.DeleteByID(ID);

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

            return RedirectToAction("DashBoard");

        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> HRSalaryHeadMaster(HRSalaryHeadMasterModel model, DataTable HRSalaryMasterDT)
        {
            //Logger.LogInformation("\n \n ********** Save Tax Master ********** \n \n");

            try
            {
                if (model != null)
                {
                    model.Mode = model.Mode == "U" ? "UPDATE" : "Insert";
                    model.ActualEntryby = Constants.UserID;

                    var HRSalaryHeadMasterDT = new DataTable();
                    var _EmpCategDetail = new List<EmpCategDetail>();
                    if (model.EmpCateg != null)
                    {
                        foreach (var item in model.EmpCateg)
                        {
                            var _EmpCateg = new EmpCategDetail()
                            {

                                CategoryId = item,
                                SalHeadEntryId= model.SalHeadEntryId


                            };
                            _EmpCategDetail.Add(_EmpCateg);
                        }
                    }
                    HRSalaryHeadMasterDT = CommonFunc.ConvertListToTable<EmpCategDetail>(_EmpCategDetail);

                    var HRSalaryMasterDeptWiseDT = new DataTable();
                    var _DeptWiseCategDetail = new List<DeptWiseCategDetail>();
                    if (model.DeptName != null)
                    {
                        foreach (var item in model.DeptName)
                        {
                            var _DeptWiseCateg = new DeptWiseCategDetail()
                            {

                                DeptId = item,
                                SalHeadEntryId=model.SalHeadEntryId


                            };
                            _DeptWiseCategDetail.Add(_DeptWiseCateg);
                        }
                    }
                    HRSalaryMasterDeptWiseDT = CommonFunc.ConvertListToTable<DeptWiseCategDetail>(_DeptWiseCategDetail);

                    if (model.Mode == "UPDATE")
                    {
                        model.LastUpdatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                       
                    }
                    else
                    {
                        model.UpdatedBy = 0;

                    }
                    var Result = await _ISalaryHeadMaster.SaveData(model, HRSalaryHeadMasterDT, HRSalaryMasterDeptWiseDT).ConfigureAwait(false);

                    if (Result != null)
                    {
                        if (Result.StatusText == "Success" && Result.StatusCode == HttpStatusCode.OK)
                        {
                            ViewBag.isSuccess = true;
                            TempData["200"] = "200";
                        }
                        if (Result.StatusText == "Unsuccess")
                        {
                            TempData["Error"] = "Please Enter Unique HeadName";
                            TempData.Keep("Error");
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
                return RedirectToAction(nameof(HRSalaryHeadMaster));
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

        public async Task<JsonResult> FillEntryId()
        {
            var JSON = await _ISalaryHeadMaster.FillEntryId();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }


        public async Task<JsonResult> GetTypeofSalaryHead()
        {
            var JSON = await _ISalaryHeadMaster.GetTypeofSalaryHead();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }

        public async Task<JsonResult> GetSalaryCalculationType()
        {
            var JSON = await _ISalaryHeadMaster.GetSalaryCalculationType();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetPartOf()
        {
            var JSON = await _ISalaryHeadMaster.GetPartOf();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }

        public async Task<JsonResult> GetSalaryPaymentMode()
        {
            var JSON = await _ISalaryHeadMaster.GetSalaryPaymentMode();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetCurrency()
        {
            var JSON = await _ISalaryHeadMaster.GetCurrency();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetAmountPercentageOfCalculation()
        {
            var JSON = await _ISalaryHeadMaster.GetAmountPercentageOfCalculation();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }

        public async Task<JsonResult> GetYesOrNo()
        {
            var JSON = await _ISalaryHeadMaster.GetYesOrNo();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetRoundOff()
        {
            var JSON = await _ISalaryHeadMaster.GetRoundOff();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetPaymentFrequency()
        {
            var JSON = await _ISalaryHeadMaster.GetPaymentFrequency();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetDeductionOfTax()
        {
            var JSON = await _ISalaryHeadMaster.GetDeductionOfTax();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetFormRights()
        {
            var userID = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
            var JSON = await _ISalaryHeadMaster.GetFormRights(userID);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
    }
}
