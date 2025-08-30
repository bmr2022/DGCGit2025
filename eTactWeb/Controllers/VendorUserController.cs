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

        [Route("{controller}/Index")]
        [HttpGet]
        public async Task<ActionResult> VendorUser(int ID, string Mode)
        {
            _logger.LogInformation("\n \n ********** Page Gate Inward ********** \n \n " + _IWebHostEnvironment.EnvironmentName.ToString() + "\n \n");
            TempData.Clear();
            var MainModel = new VendorUserModel();

            MainModel.FinFromDate = HttpContext.Session.GetString("FromDate");
            MainModel.FinToDate = HttpContext.Session.GetString("ToDate");
            MainModel.BranchName = HttpContext.Session.GetString("Branch");
            MainModel.ActualEntryBy = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
            MainModel.ActualEntryByName = HttpContext.Session.GetString("EmpName");

            if (!string.IsNullOrEmpty(Mode) && ID > 0 && Mode == "U")
            {
                //MainModel = await _IMachineMaster.GetViewByID().ConfigureAwait(false);
                //MainModel.Mode = Mode; // Set Mode to Update
                //MainModel.ID = ID;
                //MainModel.MachineId = MachineId;
                //MainModel.MachGroupId = MachGroupId;
                //MainModel.WorkCenterId = WorkCenterId;
                //MainModel.MachineCode = MachineCode;
                //MainModel.MachineName = MachineName;
                //MainModel.LabourCost = LabourCost;
                //MainModel.NeedHelper = NeedHelper;
                //MainModel.TotalHelper = TotalHelper;
                //MainModel.HelperCost = HelperCost;
                //MainModel.ElectricityCost = ElectricityCost;
                //MainModel.OtherCost = OtherCost;
                //MainModel.TotalCost = TotalCost;
                //MainModel.EntryDate = EntryDate;
                //MainModel.Make = Make;
                //MainModel.Location = Location;
                //MainModel.TechSpecification = TechSpecification;
                //MainModel.LastCalibraDate = LastCalibraDate;
                //MainModel.CalibraDur = CalibraDur;
                //MainModel.UId = UId;
                //MainModel.CC = CC;
                //MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
                //{
                //    AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                //    SlidingExpiration = TimeSpan.FromMinutes(55),
                //    Size = 1024
                //};
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
                    if (Result.StatusText == "Success" && Result.StatusCode == HttpStatusCode.OK)
                    {
                        ViewBag.isSuccess = true;
                        TempData["200"] = "200";
                        var modelSuccess = new ProductionScheduleModel();
                        return RedirectToAction("ProductionSchedule", modelSuccess);
                    }
                    if (Result.StatusText == "Updated" && Result.StatusCode == HttpStatusCode.Accepted)
                    {
                        ViewBag.isSuccess = true;
                        TempData["202"] = "202";
                        var modelUpdate = new ProductionScheduleModel();
                        return RedirectToAction("ProductionSchedule", modelUpdate);
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
                        string psGrid = HttpContext.Session.GetString("KeyProductionScheduleGrid");
                        List<ProductionScheduleDetail> PSGridDetails = new List<ProductionScheduleDetail>();
                        if (!string.IsNullOrEmpty(psGrid))
                        {
                            PSGridDetails = JsonConvert.DeserializeObject<List<ProductionScheduleDetail>>(psGrid);
                        }
                        ModelState.Clear();
                        return View(model);
                    }

                    string modelJson = HttpContext.Session.GetString("KeyProductionScheduleGrid");
                    List<ProductionScheduleDetail> PSDetail = new List<ProductionScheduleDetail>();
                    if (!string.IsNullOrEmpty(modelJson))
                    {
                        PSDetail = JsonConvert.DeserializeObject<List<ProductionScheduleDetail>>(modelJson);
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
        public async Task<JsonResult> FillVendorList(string isShowAll)
        {
            var JSON = await _IVendorMater.FillVendorList(isShowAll);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
    }
}
