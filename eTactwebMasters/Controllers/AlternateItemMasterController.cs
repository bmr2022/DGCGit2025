using eTactWeb.Data.Common;
using eTactWeb.DOM.Models.Master;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System.Runtime.Caching;
using static eTactWeb.Data.Common.CommonFunc;
using static eTactWeb.DOM.Models.Common;
using eTactWeb.DOM.Models;
using System.Net;
using System.Data;


namespace eTactWeb.Controllers
{
    public class AlternateItemMasterController : Controller
    {
        private readonly IDataLogic _IDataLogic;
        public IAlternateItemMaster _IAlternateItemMaster { get; }

        private readonly ILogger<AlternateItemMasterController> _logger;
        private readonly IConfiguration iconfiguration;
        private readonly IMemoryCache _MemoryCache;
        public IWebHostEnvironment _IWebHostEnvironment { get; }
        public AlternateItemMasterController(ILogger<AlternateItemMasterController> logger, IDataLogic iDataLogic, IAlternateItemMaster iAlternateItemMaster, IMemoryCache iMemoryCache, EncryptDecrypt encryptDecrypt, IWebHostEnvironment iWebHostEnvironment, IConfiguration iconfiguration)
        {
            _logger = logger;
            _IDataLogic = iDataLogic;
            _IAlternateItemMaster = iAlternateItemMaster;
            _MemoryCache = iMemoryCache;
            _IWebHostEnvironment = iWebHostEnvironment;
            this.iconfiguration = iconfiguration;
        }
        [Route("{controller}/Index")]
        [HttpGet]
        public async Task<ActionResult> AlternateItemMaster(int ID,string Mode,int MainItemCode,int AlternateItemCode, string MainPartCode,string MainItemName,string AlternatePartCode, string AltItemName)
        {
            _logger.LogInformation("\n \n ********** Page Gate Inward ********** \n \n " + _IWebHostEnvironment.EnvironmentName.ToString() + "\n \n");

            // Clear TempData and set session variables
            TempData.Clear();
            var MainModel = new AlternateItemMasterModel();
            MainModel.FinFromDate = HttpContext.Session.GetString("FromDate");
            MainModel.FinToDate = HttpContext.Session.GetString("ToDate");
            MainModel.CreatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
            MainModel.YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
            MainModel.ActualEntryByEmp = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
            MainModel.EntryByEmpName = HttpContext.Session.GetString("EmpName");
            MainModel.ActualEntryDate = DateTime.Today.ToString("MM/dd/yyyy").Replace("-", "/");
            MainModel.EffectiveDate = DateTime.Today.ToString("MM/dd/yyyy").Replace("-", "/");

            MainModel.EntryByempId = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
            _MemoryCache.Remove("AlternateItemMasterGrid");

            // Check if Mode is "Update" (U) and the ID is valid
            if (!string.IsNullOrEmpty(Mode) && ID > 0 && Mode == "U")
            {

                //Retrieve the old data by AccountCode and populate the model with existing values
                MainModel = await _IAlternateItemMaster.GetViewByID(MainItemCode, AlternateItemCode).ConfigureAwait(false);
                MainModel.Mode = Mode; // Set Mode to Update
                MainModel.ID = ID;
                MainModel.MainPartCode = MainPartCode;
                MainModel.MainItemName = MainItemName;
                MainModel.AlternatePartCode = AlternatePartCode;
                MainModel.AltItemName = AltItemName;
                MainModel.MainItemCode = MainItemCode;
                MainModel.AlternateItemCode = AlternateItemCode;

                //MainModel = await BindModels(MainModel).ConfigureAwait(false);
                MainModel.FinFromDate = HttpContext.Session.GetString("FromDate");
                MainModel.FinToDate = HttpContext.Session.GetString("ToDate");
                if (Mode == "U")
                {
                    MainModel.UpdatedByEmp = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                    MainModel.LastUpdatedBy = HttpContext.Session.GetString("EmpName");
                    MainModel.UpdationDate = DateTime.Now.ToString();
                    MainModel.ActualEntryDate = DateTime.Today.ToString("MM/dd/yyyy").Replace("-", "/");
                    MainModel.ActualEntryByEmp = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
                    MainModel.EffectiveDate = DateTime.Today.ToString("MM/dd/yyyy").Replace("-", "/");

                }
                MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                    SlidingExpiration = TimeSpan.FromMinutes(55),
                    Size = 1024
                };
                _MemoryCache.Set("AlternateItemMasterGrid", MainModel.AlternateItemMasterGrid, cacheEntryOptions);
            }

            // If not in "Update" mode, bind new model data
            else
            {
               // MainModel = await BindModels(MainModel);
            }

            // When updating the record, make sure to capture updated info
            
            return View(MainModel); // Pass the model with old data to the view
        }
        [Route("{controller}/Index")]
        [HttpPost]
        public async Task<IActionResult> AlternateItemMaster(AlternateItemMasterModel model)
        {
            try
            {
                var GIGrid = new DataTable();
                _MemoryCache.TryGetValue("AlternateItemMasterGrid", out List<AlternateItemMasterGridModel> AlternateItemMasterGrid);


                model.CreatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                if (model.Mode == "U")
                {
                    GIGrid = GetDetailTable(AlternateItemMasterGrid);
                    var mainItemCodes = AlternateItemMasterGrid
        .Select(item => item.MainItemCode)
        .ToList();
                    var altItemCode = AlternateItemMasterGrid
        .Select(item => item.AlternateItemCode)
        .ToList();
                    foreach (var maincode in mainItemCodes)
                    {
                        model.MainItemCode = maincode;
                    }
                    foreach (var code in altItemCode)
                    {
                        model.AlternateItemCode = code;
                    }
                }
                else
                {
                    GIGrid = GetDetailTable(AlternateItemMasterGrid);
                }
                var Result = await _IAlternateItemMaster.SaveAlternetItemMaster(model,GIGrid);
                if (Result != null)
                {
                    if (Result.StatusText == "Success" && Result.StatusCode == HttpStatusCode.OK)
                    {
                        ViewBag.isSuccess = true;
                        TempData["200"] = "200";
                        _MemoryCache.Remove("AlternateItemMasterGrid");
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

                return RedirectToAction(nameof(AlternateItemMasterDashBoard));

            }
            catch (Exception ex)
            {
                // Log and return the error
                LogException<AlternateItemMasterController>.WriteException(_logger, ex);
                var ResponseResult = new ResponseResult
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    StatusText = "Error",
                    Result = ex
                };
                return View("Error", ResponseResult);
            }
        }
        public async Task<JsonResult> GetMainPartCode()
        {
            var JSON = await _IAlternateItemMaster.GetMainPartCode();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetMainItemName()
        {
            var JSON = await _IAlternateItemMaster.GetMainItem();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetAltPartCode(int MainItemcode)
       {
            var JSON = await _IAlternateItemMaster.GetAltPartCode(MainItemcode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        } 
        public async Task<JsonResult> GetAltItemName(int MainItemcode)
       {
            var JSON = await _IAlternateItemMaster.GetAltItemName(MainItemcode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public IActionResult AddAlternateItemMasterDetail(AlternateItemMasterGridModel model)
        {
            try
            {
                if (model.Mode == "U")
                {

                    _MemoryCache.TryGetValue("KeyAlternateItemMasterGrid", out IList<AlternateItemMasterGridModel> AlternateItemMasterDGrid);

                    var MainModel = new AlternateItemMasterModel();
                    var WorkOrderPGrid = new List<AlternateItemMasterGridModel>();
                    var OrderGrid = new List<AlternateItemMasterGridModel>();
                    var ssGrid = new List<AlternateItemMasterGridModel>();

                    var count = 0;
                    if (model != null)
                    {
                        if (AlternateItemMasterDGrid == null)
                        {
                            model.SrNO = 1;
                            OrderGrid.Add(model);
                        }
                        else
                        {
                            if (AlternateItemMasterDGrid.Any(x => (x.AltPartCode == model.AltPartCode)))
                            {
                                return StatusCode(207, "Duplicate");
                            }
                            else
                            {
                                //count = WorkOrderProcessGrid.Count();
                                model.SrNO = AlternateItemMasterDGrid.Count + 1;
                                OrderGrid = AlternateItemMasterDGrid.Where(x => x != null).ToList();
                                ssGrid.AddRange(OrderGrid);
                                OrderGrid.Add(model);

                            }

                        }

                        MainModel.AlternateItemMasterGrid = OrderGrid;

                        MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
                        {
                            AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                            SlidingExpiration = TimeSpan.FromMinutes(55),
                            Size = 1024,
                        };

                        _MemoryCache.Set("KeyAlternateItemMasterGrid", MainModel.AlternateItemMasterGrid, cacheEntryOptions);
                    }
                    else
                    {
                        ModelState.TryAddModelError("Error", "Schedule List Cannot Be Empty...!");
                    }
                    return PartialView("AlternateItemMaster", MainModel);
                }
                else
                {

                    _MemoryCache.TryGetValue("AlternateItemMasterGrid", out IList<AlternateItemMasterGridModel> AlternateItemMasterGrid);

                    var MainModel = new AlternateItemMasterModel();
                    var WorkOrderPGrid = new List<AlternateItemMasterGridModel>();
                    var OrderGrid = new List<AlternateItemMasterGridModel>();
                    var ssGrid = new List<AlternateItemMasterGridModel>();

                    if (model != null)
                    {
                        if (AlternateItemMasterGrid == null)
                        {
                            model.SrNO = 1;
                            OrderGrid.Add(model);
                        }
                        else
                        {
                            if (AlternateItemMasterGrid.Any(x => (x.AltPartCode == model.AltPartCode) ))
                            {
                                return StatusCode(207, "Duplicate");
                            }
                            else
                            {
                                //count = WorkOrderProcessGrid.Count();
                                model.SrNO = AlternateItemMasterGrid.Count + 1;
                                OrderGrid = AlternateItemMasterGrid.Where(x => x != null).ToList();
                                ssGrid.AddRange(OrderGrid);
                                OrderGrid.Add(model);

                            }

                        }

                        MainModel.AlternateItemMasterGrid = OrderGrid;

                        MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
                        {
                            AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                            SlidingExpiration = TimeSpan.FromMinutes(55),
                            Size = 1024,
                        };

                        _MemoryCache.Set("AlternateItemMasterGrid", MainModel.AlternateItemMasterGrid, cacheEntryOptions);
                    }
                    else
                    {
                        ModelState.TryAddModelError("Error", "Schedule List Cannot Be Empty...!");
                    }
                    return PartialView("_AlternateItemMasterGrid", MainModel);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IActionResult EditItemRow(int SeqNo, string Mode)
        {
            IList<AlternateItemMasterGridModel> AlternateItemMasterGrid = new List<AlternateItemMasterGridModel>();
            if (Mode == "U")
            {
                _MemoryCache.TryGetValue("KeyAlternateItemMasterGrid", out AlternateItemMasterGrid);
            }
            else
            {
                _MemoryCache.TryGetValue("KeyAlternateItemMasterGrid", out AlternateItemMasterGrid);
            }
            IEnumerable<AlternateItemMasterGridModel> SSBreakdownGrid = AlternateItemMasterGrid;
            if (AlternateItemMasterGrid != null)
            {
                SSBreakdownGrid = AlternateItemMasterGrid.Where(x => x.SrNO == SeqNo);
            }
            string JsonString = JsonConvert.SerializeObject(SSBreakdownGrid);
            return Json(JsonString);
        }
        public IActionResult DeleteItemRow(int SeqNo, string Mode)
        {
            var MainModel = new AlternateItemMasterModel();
            if (Mode == "U")
            {
                _MemoryCache.TryGetValue("KeyAlternateItemMasterGrid", out List<AlternateItemMasterGridModel> AlternateItemMasterGrid);
                int Indx = SeqNo - 1;

                if (AlternateItemMasterGrid != null && AlternateItemMasterGrid.Count > 0)
                {
                    AlternateItemMasterGrid.RemoveAt(Convert.ToInt32(Indx));

                    Indx = 0;

                    foreach (var item in AlternateItemMasterGrid)
                    {
                        Indx++;
                        item.SrNO = Indx;
                    }
                    MainModel.AlternateItemMasterGrid = AlternateItemMasterGrid;

                    MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
                    {
                        AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                        SlidingExpiration = TimeSpan.FromMinutes(55),
                        Size = 1024,
                    };

                    _MemoryCache.Set("KeyAlternateItemMasterGrid", MainModel.AlternateItemMasterGrid, cacheEntryOptions);
                }
            }
            else
            {
                _MemoryCache.TryGetValue("KeyAlternateItemMasterGrid", out List<AlternateItemMasterGridModel> AlternateItemMasterGrid);
                int Indx = SeqNo;

                if (AlternateItemMasterGrid != null && AlternateItemMasterGrid.Count > 0)
                {
                    AlternateItemMasterGrid.RemoveAt(Indx);

                    Indx = 0;

                    foreach (var item in AlternateItemMasterGrid)
                    {
                        Indx++;
                        item.SrNO = Indx;
                    }
                    MainModel.AlternateItemMasterGrid = AlternateItemMasterGrid;

                    MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
                    {
                        AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                        SlidingExpiration = TimeSpan.FromMinutes(55),
                        Size = 1024,
                    };

                    _MemoryCache.Set("KeyAlternateItemMasterGrid", MainModel.AlternateItemMasterGrid, cacheEntryOptions);
                }
            }

            return PartialView("AlternateItemMaster", MainModel);
        }
        private static DataTable GetDetailTable(IList<AlternateItemMasterGridModel> DetailList)
        {
            try
            {
                var GIGrid = new DataTable();

                GIGrid.Columns.Add("FGItemCode", typeof(int));
                GIGrid.Columns.Add("MainItemCode", typeof(int));
                GIGrid.Columns.Add("MainItemName", typeof(string));
                GIGrid.Columns.Add("AlternateItemCode", typeof(int));
                GIGrid.Columns.Add("AltItemName", typeof(string));
                foreach (var Item in DetailList)
                {
                    GIGrid.Rows.Add(
                        new object[]
                        {
             Item.FGItemCode == 0 ? 0 : Item.FGItemCode,
             Item.MainItemCode == 0 ? 0 : Item.MainItemCode ,
             Item.MainItemName == null ? "" : Item.MainItemName ,
             Item.AlternateItemCode == 0 ? 0 : Item.AlternateItemCode,
             Item.AltItemName == null ? "" : Item.AltItemName,
             

                        });
                }
                GIGrid.Dispose();
                return GIGrid;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        [HttpGet]
        public async Task<IActionResult> AlternateItemMasterDashBoard(string FromDate = "", string ToDate = "")
        {
            try
            {
                var model = new AlternateItemMasterDashBoardModel();
                var result = await _IAlternateItemMaster.GetDashboardData().ConfigureAwait(true);
                DateTime now = DateTime.Now;

                model.FromDate = new DateTime(now.Year, now.Month, 1).ToString("dd/MM/yyyy");
                model.ToDate = new DateTime(now.Year + 1, 3, 31).ToString("dd/MM/yyyy");

                if (result != null && result.Result != null)
                {
                    DataSet ds = result.Result;
                    if (ds != null && ds.Tables.Count > 0)
                    {
                        var dt = ds.Tables[0];
                        model.AlternateItemMasterDashBoardGrid = CommonFunc.DataTableToList<AlternateItemMasterDashBoardModel>(dt, "AlternateItemMasterDashboard");
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
            var model = new AlternateItemMasterDashBoardModel();
            model = await _IAlternateItemMaster.GetDashboardDetailData();
            return PartialView("_AlternateItemMasterDashBoardGrid", model);
        }
        public async Task<IActionResult> DeleteByID(int MainItemCode, int AlternateItemCode, string MachineName, int EntryByempId)
        {
            var Result = await _IAlternateItemMaster.DeleteByID(MainItemCode, AlternateItemCode, MachineName, EntryByempId);

            if (Result.StatusText == "Success" || Result.StatusCode == HttpStatusCode.Gone)
            {
                ViewBag.isSuccess = true;
                TempData["410"] = "410";
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

            return RedirectToAction("AlternateItemMasterDashBoard");

        }
    }
}
