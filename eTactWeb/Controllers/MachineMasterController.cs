using eTactWeb.Data.Common;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Memory;
using static eTactWeb.DOM.Models.Common;
using System.Runtime.Caching;
using eTactWeb.DOM.Models.Master;
using Newtonsoft.Json.Linq;
using eTactWeb.DOM.Models;
using Newtonsoft.Json;
using static eTactWeb.Data.Common.CommonFunc;

namespace eTactWeb.Controllers
{
    public class MachineMasterController : Controller
    {
        private readonly IDataLogic _IDataLogic;
        private readonly IMachineMaster _IMachineMaster;
        private readonly ILogger<MachineMasterController> _logger;
        private readonly IConfiguration iconfiguration;
        public IWebHostEnvironment _IWebHostEnvironment { get; }

        public MachineMasterController(ILogger<MachineMasterController> logger, IDataLogic iDataLogic, IMachineMaster iMachineMaster, EncryptDecrypt encryptDecrypt, IWebHostEnvironment iWebHostEnvironment, IConfiguration iconfiguration)
        {
            _logger = logger;
            _IDataLogic = iDataLogic;
            _IMachineMaster = iMachineMaster;
            _IWebHostEnvironment = iWebHostEnvironment;
            this.iconfiguration = iconfiguration;

        }
        [Route("{controller}/Index")]
        [HttpGet]
        public async Task<ActionResult> MachineMaster(int ID, string Mode, int MachineId, int MachGroupId, int WorkCenterId, string MachineCode, string MachineName,
                                   double LabourCost, string NeedHelper, int TotalHelper, double HelperCost,
                                   double ElectricityCost, double OtherCost, double TotalCost, string EntryDate,
                                   string Make, string Location, string TechSpecification, string LastCalibraDate,
                                   double CalibraDur, string UId, string CC)
        {
            _logger.LogInformation("\n \n ********** Page Gate Inward ********** \n \n " + _IWebHostEnvironment.EnvironmentName.ToString() + "\n \n");
            TempData.Clear();
            var MainModel = new MachineMasterModel();
            MainModel.FromDate = HttpContext.Session.GetString("FromDate");
            MainModel.ToDate = HttpContext.Session.GetString("ToDate");
            MainModel.CC = HttpContext.Session.GetString("Branch");
            MainModel.EntryDate = HttpContext.Session.GetString("EntryDate");
            MainModel.UId = HttpContext.Session.GetString("UID");

            if (!string.IsNullOrEmpty(Mode) && ID > 0 && Mode == "U")
            {
                MainModel = await _IMachineMaster.GetViewByID().ConfigureAwait(false);
                MainModel.Mode = Mode; // Set Mode to Update
                MainModel.ID = ID;
                MainModel.MachineId = MachineId;
                MainModel.MachGroupId = MachGroupId;
                MainModel.WorkCenterId = WorkCenterId;
                MainModel.MachineCode = MachineCode;
                MainModel.MachineName = MachineName;
                MainModel.LabourCost = LabourCost;
                MainModel.NeedHelper = NeedHelper;
                MainModel.TotalHelper = TotalHelper;
                MainModel.HelperCost = HelperCost;
                MainModel.ElectricityCost = ElectricityCost;
                MainModel.OtherCost = OtherCost;
                MainModel.TotalCost = TotalCost;
                MainModel.EntryDate = EntryDate;
                MainModel.Make = Make;
                MainModel.Location = Location;
                MainModel.TechSpecification = TechSpecification;
                MainModel.LastCalibraDate = LastCalibraDate;
                MainModel.CalibraDur = CalibraDur;
                MainModel.UId = UId;
                MainModel.CC = CC;
                MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                    SlidingExpiration = TimeSpan.FromMinutes(55),
                    Size = 1024
                };
            }

            return View(MainModel);
        }
        [Route("{controller}/Index")]
        [HttpPost]
        public async Task<IActionResult> MachineMaster(MachineMasterModel model)
        {
            try
            {
                model.CreatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));

                var Result = await _IMachineMaster.SaveMachineMaster(model);
                if (Result != null)
                {
                    if (Result.StatusText == "Success" && Result.StatusCode == HttpStatusCode.OK)
                    {
                        ViewBag.isSuccess = true;
                        TempData["200"] = "200";
                        //_MemoryCache.Remove("KeyLedgerOpeningEntryGrid");
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

                return RedirectToAction(nameof(MachineMasterDashboard));

            }
            catch (Exception ex)
            {
                // Log and return the error
                LogException<MachineMasterController>.WriteException(_logger, ex);
                var ResponseResult = new ResponseResult
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    StatusText = "Error",
                    Result = ex
                };
                return View("Error", ResponseResult);
            }
        }
        public async Task<JsonResult> FillMachineGroup()
        {
            var JSON = await _IMachineMaster.FillMachineGroup();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillMachineWorkCenter()
        {
            var JSON = await _IMachineMaster.FillMachineWorkCenter();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<IActionResult> MachineMasterDashboard()
        {
            try
            {
                var model = new MachineMasterModel();
                model.FromDate = HttpContext.Session.GetString("FromDate");
                model.ToDate = HttpContext.Session.GetString("ToDate");
                var Result = await _IMachineMaster.GetDashBoardData().ConfigureAwait(true);
                if (Result != null)
                {
                    DataSet ds = Result.Result;
                    if (ds != null && ds.Tables.Count > 0)
                    {
                        var dt = ds.Tables[0];
                        model.MachineMasterGrid = CommonFunc.DataTableToList<MachineMasterModel>(dt, "MachineMasterDashBoard");
                    }
                }
                return View(model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IActionResult> GetDashBoardDetailData()
        {
            var model = new MachineMasterModel();
            model = await _IMachineMaster.GetDashBoardDetailData();
            return PartialView("_MachineMasterDashBoardGrid", model);
        }
        public async Task<IActionResult> DeleteByID(int ID)
        {
            var Result = await _IMachineMaster.DeleteByID(ID);

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

            return RedirectToAction("MachineMasterDashboard");

        }
    }
}
