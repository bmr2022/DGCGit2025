using eTactWeb.Data.Common;
using eTactWeb.Services.Interface;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using static eTactWeb.Data.Common.CommonFunc;
using static eTactWeb.DOM.Models.Common;
using eTactWeb.DOM.Models;
using MessagePack;

namespace eTactWeb.Controllers
{
    public class HRLeaveApplicationMasterController : Controller
    {
        private readonly IDataLogic _IDataLogic;
        private readonly IHRLeaveApplicationMaster _IHRLeaveApplicationMaster;
        private readonly ILogger<HRLeaveApplicationMasterController> _logger;
        private readonly IWebHostEnvironment _IWebHostEnvironment;
        public HRLeaveApplicationMasterController(ILogger<HRLeaveApplicationMasterController> logger, IDataLogic iDataLogic, IHRLeaveApplicationMaster iHRLeaveApplicationMaster, EncryptDecrypt encryptDecrypt, IWebHostEnvironment iWebHostEnvironment)
        {
            _logger = logger;
            _IDataLogic = iDataLogic;
            _IHRLeaveApplicationMaster = iHRLeaveApplicationMaster;
            _IWebHostEnvironment = iWebHostEnvironment;
        }
        [Route("{controller}/Index")]
        public async Task<IActionResult> HRLeaveApplicationMaster()
        {
            ViewData["Title"] = "Leave Application Master";
            TempData.Clear();
            HttpContext.Session.Remove("KeyLeaveApplicationGrid");
            var model = new HRLeaveApplicationMasterModel();
        
            model.ActualEntryBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
            model.LeaveAppYearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
            model.BranchCC = Convert.ToString(HttpContext.Session.GetString("Branch"));

            return View(model);
        }
        [Route("{controller}/Index")]
        [HttpGet]
        public async Task<ActionResult> HRLeaveApplicationMaster(string Mode,int ID,int year)//, ILogger logger)
        {
            //_logger.LogInformation("\n \n ********** Page Gate Inward ********** \n \n " + IWebHostEnvironment.EnvironmentName.ToString() + "\n \n");
            TempData.Clear();
            var MainModel = new HRLeaveApplicationMasterModel();
            
            HttpContext.Session.Remove("KeyLeaveApplicationGrid");
            MainModel.Mode = Mode;
           year= Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
            MainModel.LeaveAppEntryId = ID;
            if (!string.IsNullOrEmpty(Mode) && ID > 0 && (Mode == "V" || Mode == "U"))
            {
                MainModel = await _IHRLeaveApplicationMaster.GetViewByID(ID, year).ConfigureAwait(false);
                MainModel.Mode = Mode;
                MainModel.LeaveAppYearCode = year;
                MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                    SlidingExpiration = TimeSpan.FromMinutes(55),
                    Size = 1024,
                };
                string serializedGrid = JsonConvert.SerializeObject(MainModel.ItemDetailGrid);
                HttpContext.Session.SetString("KeyLeaveApplicationGrid", serializedGrid);
            }
            else
            {
                // MainModel = await BindModel(MainModel);
            }

            if (Mode == "U")
            {
                MainModel.Updatedby = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                //MainModel.ActualEnteredByName = HttpContext.Session.GetString("EmpName");
            }
            else
            {
                //MainModel.ActualEnteredByName = HttpContext.Session.GetString("EmpName");
                MainModel.ActualEntryBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                MainModel.LeaveAppYearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
                MainModel.BranchCC = Convert.ToString(HttpContext.Session.GetString("Branch"));
                //MainModel.UpdatedByName = HttpContext.Session.GetString("EmpName");
                MainModel.LastUPdatedDate = HttpContext.Session.GetString("LastUpdatedDate");
            }
           
            return View(MainModel);
        }

        public async Task<JsonResult> GetEmpName()
        {
            var JSON = await _IHRLeaveApplicationMaster.GetEmpName();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetLeaveName()
        {
            var JSON = await _IHRLeaveApplicationMaster.GetLeaveName();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetShiftName()
        {
            var JSON = await _IHRLeaveApplicationMaster.GetShiftName();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetEmpCode()
        {
            var JSON = await _IHRLeaveApplicationMaster.GetEmpCode();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillEntryId(int YearCode)
        {
            var JSON = await _IHRLeaveApplicationMaster.FillEntryId(YearCode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }

        public async Task<JsonResult> GetEmployeeDetail(int empid)
        {
            var JSON = await _IHRLeaveApplicationMaster.GetEmployeeDetail(empid);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }

        public async Task<JsonResult> GetLeaveDetail(int empid, string LeaveAppEntryDate)
        {
            var JSON = await _IHRLeaveApplicationMaster.GetLeaveDetail(empid, LeaveAppEntryDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }

        public async Task<JsonResult> GetBalanceandMaxLeaveTypeWise(int empid, string LeaveAppEntryDate,int LeaveEntryId)
        {
            var JSON = await _IHRLeaveApplicationMaster.GetBalanceandMaxLeaveTypeWise(empid, LeaveAppEntryDate, LeaveEntryId);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }




        public IActionResult AddDetailGrid(HRLeaveApplicationDetail model)
        {
            try
            {
                if (model.Mode == "U")
                {
                    string modelJson = HttpContext.Session.GetString("KeyLeaveApplicationGrid");
                    List<HRLeaveApplicationDetail> HRLeaveApplicationDetail = new List<HRLeaveApplicationDetail>();
                    if (!string.IsNullOrEmpty(modelJson))
                    {
                        HRLeaveApplicationDetail = JsonConvert.DeserializeObject<List<HRLeaveApplicationDetail>>(modelJson);
                    }                    

                    var MainModel = new HRLeaveApplicationMasterModel();
                    var ProductionEntryGrid = new List<HRLeaveApplicationDetail>();
                    var ProductionGrid = new List<HRLeaveApplicationDetail>();
                    var SSGrid = new List<HRLeaveApplicationDetail>();

                    if (model != null)
                    {
                        if (HRLeaveApplicationDetail == null)
                        {
                            model.SeqNo = 1;
                            ProductionGrid.Add(model);
                        }
                        else
                        {
                            if (HRLeaveApplicationDetail.Where(x => x.LeaveName == model.LeaveName).Any())
                            {
                                return StatusCode(207, "Duplicate");
                            }
                            else
                            {
                                model.SeqNo = HRLeaveApplicationDetail.Count + 1;
                                ProductionGrid = HRLeaveApplicationDetail.Where(x => x != null).ToList();
                                SSGrid.AddRange(ProductionGrid);
                                ProductionGrid.Add(model);
                            }
                        }

                        MainModel.ItemDetailGrid = ProductionGrid;

                        string serializedGrid = JsonConvert.SerializeObject(MainModel.ItemDetailGrid);
                        HttpContext.Session.SetString("KeyLeaveApplicationGrid", serializedGrid);
                    }
                    else
                    {
                        ModelState.TryAddModelError("Error", "Schedule List Cannot Be Empty...!");
                    }

                    return PartialView("_HRLeaveApplicationDetailGrid", MainModel);
                }
                else
                {
                    string modelJson = HttpContext.Session.GetString("KeyLeaveApplicationGrid");
                    List<HRLeaveApplicationDetail> HRLeaveApplicationDetail = new List<HRLeaveApplicationDetail>();
                    if (!string.IsNullOrEmpty(modelJson))
                    {
                        HRLeaveApplicationDetail = JsonConvert.DeserializeObject<List<HRLeaveApplicationDetail>>(modelJson);
                    }

                    var MainModel = new HRLeaveApplicationMasterModel();
                    var ProductionEntryGrid = new List<HRLeaveApplicationDetail>();
                    var ProductionGrid = new List<HRLeaveApplicationDetail>();
                    var SSGrid = new List<HRLeaveApplicationDetail>();

                    if (model != null)
                    {
                        if (HRLeaveApplicationDetail == null)
                        {
                            model.SeqNo = 1;
                            ProductionGrid.Add(model);
                        }
                        else
                        {
                            if (HRLeaveApplicationDetail.Where(x => x.LeaveName == model.LeaveName).Any())
                            {
                                return StatusCode(207, "Duplicate");
                            }
                            else
                            {
                                model.SeqNo = HRLeaveApplicationDetail.Count + 1;
                                ProductionGrid = HRLeaveApplicationDetail.Where(x => x != null).ToList();
                                SSGrid.AddRange(ProductionGrid);
                                ProductionGrid.Add(model);
                            }

                        }

                        MainModel.ItemDetailGrid = ProductionGrid;

                        string serializedGrid = JsonConvert.SerializeObject(MainModel.ItemDetailGrid);
                        HttpContext.Session.SetString("KeyLeaveApplicationGrid", serializedGrid);
                    }
                    else
                    {
                        ModelState.TryAddModelError("Error", "Schedule List Cannot Be Empty...!");
                    }

                    return PartialView("_HRLeaveApplicationDetailGrid", MainModel);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IActionResult DeleteItemFromGrid(int SeqNo, string Mode)
        {
            var MainModel = new HRLeaveApplicationMasterModel();
            if (Mode == "U")
            {
                string modelJson = HttpContext.Session.GetString("KeyLeaveApplicationGrid");
                List<HRLeaveApplicationDetail> ItemDetailGrid = new List<HRLeaveApplicationDetail>();
                if (!string.IsNullOrEmpty(modelJson))
                {
                    ItemDetailGrid = JsonConvert.DeserializeObject<List<HRLeaveApplicationDetail>>(modelJson);
                }

                //_MemoryCache.TryGetValue("KeyPartCodePartyWiseGrid", out List<ItemDetailGrid> ItemDetailGrid);
                int Indx = Convert.ToInt32(SeqNo) - 1;

                if (ItemDetailGrid != null && ItemDetailGrid.Count > 0)
                {
                    ItemDetailGrid.RemoveAt(Convert.ToInt32(Indx));

                    Indx = 0;

                    foreach (var item in ItemDetailGrid)
                    {
                        Indx++;
                        item.SeqNo = Indx;
                    }
                    MainModel.ItemDetailGrid = ItemDetailGrid;

                    string serializedGrid = JsonConvert.SerializeObject(MainModel.ItemDetailGrid);
                    HttpContext.Session.SetString("KeyLeaveApplicationGrid", serializedGrid);
                }
            }
            else
            {
                string modelJson = HttpContext.Session.GetString("KeyLeaveApplicationGrid");
                List<HRLeaveApplicationDetail> HRLeaveApplicationDetail = new List<HRLeaveApplicationDetail>();
                if (!string.IsNullOrEmpty(modelJson))
                {
                    HRLeaveApplicationDetail = JsonConvert.DeserializeObject<List<HRLeaveApplicationDetail>>(modelJson);
                }

                //_MemoryCache.TryGetValue("KeyPartCodePartyWiseGrid", out List<ItemDetailGrid> ItemDetailGrid);
                int Indx = Convert.ToInt32(SeqNo) - 1;

                if (HRLeaveApplicationDetail != null && HRLeaveApplicationDetail.Count > 0)
                {
                    HRLeaveApplicationDetail.RemoveAt(Convert.ToInt32(Indx));

                    Indx = 0;

                    foreach (var item in HRLeaveApplicationDetail)
                    {
                        Indx++;
                        item.SeqNo = Indx;
                    }
                    MainModel.ItemDetailGrid = HRLeaveApplicationDetail;

                    MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
                    {
                        AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                        SlidingExpiration = TimeSpan.FromMinutes(55),
                        Size = 1024,
                    };
                    string serializedGrid = JsonConvert.SerializeObject(MainModel.ItemDetailGrid);
                    HttpContext.Session.SetString("KeyLeaveApplicationGrid", serializedGrid);
                }
            }

            return PartialView("_HRLeaveApplicationDetailGrid", MainModel);
        }
        public IActionResult EditItemRow(int SeqNo, string Mode)
        {
            IList<HRLeaveApplicationDetail> HRLeaveApplicationDetail = new List<HRLeaveApplicationDetail>();
            if (Mode == "U")
            {
                string modelJson = HttpContext.Session.GetString("KeyLeaveApplicationGrid");
                if (!string.IsNullOrEmpty(modelJson))
                {
                    HRLeaveApplicationDetail = JsonConvert.DeserializeObject<List<HRLeaveApplicationDetail>>(modelJson);
                }
            }
            else
            {
                string modelJson = HttpContext.Session.GetString("KeyLeaveApplicationGrid");
                if (!string.IsNullOrEmpty(modelJson))
                {
                    HRLeaveApplicationDetail = JsonConvert.DeserializeObject<List<HRLeaveApplicationDetail>>(modelJson);
                }
            }
            IEnumerable<HRLeaveApplicationDetail> SSGrid = HRLeaveApplicationDetail;
            if (HRLeaveApplicationDetail != null)
            {
                SSGrid = HRLeaveApplicationDetail.Where(x => x.SeqNo == SeqNo);
            }
            string JsonString = JsonConvert.SerializeObject(SSGrid);
            return Json(JsonString);
        }


        private static DataTable GetDetailTable(IList<HRLeaveApplicationDetail> DetailList)
        {
            try
            {
                var GIGrid = new DataTable();
                GIGrid.Columns.Add("LeaveAppEntryId", typeof(int));
                GIGrid.Columns.Add("LeaveAppYearCode", typeof(int));
                GIGrid.Columns.Add("EmpId", typeof(int));
                GIGrid.Columns.Add("SeqNo", typeof(int));
                GIGrid.Columns.Add("LeaveEntryId", typeof(int));
                GIGrid.Columns.Add("FromDate", typeof(DateTime));
                GIGrid.Columns.Add("ToDate", typeof(DateTime));
                GIGrid.Columns.Add("Duration", typeof(float));
                GIGrid.Columns.Add("HalfDayFullDay", typeof(string));
                GIGrid.Columns.Add("BalanceLeaveMonthly", typeof(float));
                GIGrid.Columns.Add("BalanceLeaveYearly", typeof(float));
                GIGrid.Columns.Add("MaxLeaveInMonth", typeof(float));
                GIGrid.Columns.Add("Approved", typeof(string));
                GIGrid.Columns.Add("Canceled", typeof(string));
                

                foreach (var Item in DetailList)
                {
                    GIGrid.Rows.Add(
                        new object[]
                        {
                            Item.LeaveAppEntryId==0?0:Item.LeaveAppEntryId,
                            Item.LeaveAppYearCode==0?0:Item.LeaveAppYearCode,
                            Item.EmpId==0?0:Item.EmpId,
                    Item.SeqNo==0?0:Item.SeqNo,
                    Item.LeaveEntryId == null ? 0 : Item.LeaveEntryId,
                    Item.FromDate == null ? "" : DateTime.Parse(Item.FromDate),
                    Item.ToDate == null ? "" : DateTime.Parse(Item.ToDate),
                    Item.Duration == 0 ? 0:Item.Duration,
                    Item.HalfDayFullDay == null ? "" : Item.HalfDayFullDay,
                    Item.BalanceLeaveMonthly == 0 ? 0 :Item.BalanceLeaveMonthly,
                    Item.BalanceLeaveYearly== 0 ? 0 : Item.BalanceLeaveYearly,
                    Item.MaxLeaveInMonth== 0 ? 0:Item.MaxLeaveInMonth,
                     Item.Approved == null ? "" : Item.Approved,
                      Item.Canceled == null ? "" : Item.Canceled,
                      
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

        [HttpPost]
        
        [Route("{controller}/Index")]

        public async Task<IActionResult> HRLeaveApplicationMaster(HRLeaveApplicationMasterModel model)
        {
            try
            {
                var GIGrid = new DataTable();
                string modelJson = HttpContext.Session.GetString("KeyLeaveApplicationGrid");
                List<HRLeaveApplicationDetail> HRLeaveApplicationDetail = new List<HRLeaveApplicationDetail>();
                if (!string.IsNullOrEmpty(modelJson))
                {
                    HRLeaveApplicationDetail = JsonConvert.DeserializeObject<List<HRLeaveApplicationDetail>>(modelJson);
                }

                var MainModel = new HRLeaveApplicationMasterModel();
                var ProductionEntryGrid = new List<HRLeaveApplicationDetail>();
                var ProductionGrid = new List<HRLeaveApplicationDetail>();
                var SSGrid = new List<HRLeaveApplicationDetail>();
                if (HRLeaveApplicationDetail == null)
                {
                    ModelState.Clear();
                    model = new HRLeaveApplicationMasterModel();
                    TempData["EmptyError"] = "EmptyError";
                    return View(model);
                }
                else
                {
                    if (model.Mode == "U")
                    {
                       // model.UpdatedBy = HttpContext.Session.GetString("EmpName");
                        GIGrid = GetDetailTable(HRLeaveApplicationDetail);
                    }
                    else
                    {
                        GIGrid = GetDetailTable(HRLeaveApplicationDetail);
                    }

                    var Result = await _IHRLeaveApplicationMaster.SaveData(model, GIGrid);

                    if (Result != null)
                    {
                        if (Result.StatusText == "Success" && Result.StatusCode == HttpStatusCode.OK)
                        {
                            ViewBag.isSuccess = true;
                            TempData["200"] = "200";
                            HttpContext.Session.Remove("KeyLeaveApplicationGrid");
                        }
                        if (Result.StatusText == "Success" && Result.StatusCode == HttpStatusCode.Accepted)
                        {
                            ViewBag.isSuccess = true;
                            TempData["202"] = "202";
                        }
                        if (Result.StatusText == "Error" && Result.StatusCode == HttpStatusCode.InternalServerError)
                        {
                           
                        }
                    }
                    var model1 = new HRLeaveApplicationMasterModel();
                    model1.ActualEntryBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));

                    return RedirectToAction(nameof(HRLeaveApplicationMasterDashBoard));
                }
            }
            catch (Exception ex)
            {
                LogException<HRLeaveApplicationMasterController>.WriteException(_logger, ex);


                var ResponseResult = new ResponseResult()
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    StatusText = "Error",
                    Result = ex
                };

                return View("Error", ResponseResult);
            }
        }


        [HttpGet]
        [Route("HRLeaveApplicationMasterDashBoard")]
        public async Task<IActionResult> HRLeaveApplicationMasterDashBoard()
        {
            try
            {
                var model = new HRLeaveApplicationDashBoard();
                var result = await _IHRLeaveApplicationMaster.GetDashboardData().ConfigureAwait(true);
                DateTime now = DateTime.Now;

                if (result != null && result.Result != null)
                {
                    DataSet ds = result.Result;
                    if (ds != null && ds.Tables.Count > 0)
                    {
                        var dt = ds.Tables[0];
                        model.HRLeaveApplicationDashBoardDetail = CommonFunc.DataTableToList<HRLeaveApplicationDashBoard>(dt, "HRLeaveApplicationMaster");
                    }

                }

                return View(model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IActionResult> GetDetailData(string ReportType, string FromDate, string ToDate,int Empid,int LeaveEntryId)
        {
            //model.Mode = "Search";
            var model = new HRLeaveApplicationDashBoard();
            model = await _IHRLeaveApplicationMaster.GetDashboardDetailData(ReportType, FromDate, ToDate, Empid, LeaveEntryId);


            if (ReportType == "SUMMARY")
            {
                return PartialView("_HRLeaveAppDashBoardSummaryGrid", model);
            }
            else if (ReportType == "DETAIL")
            {
                return PartialView("_HRLeaveAppDashBoardDetailGrid", model);

            }
            return null;
        }


        public async Task<IActionResult> DeleteByID(int ID, int year)
        {
            var Result = await _IHRLeaveApplicationMaster.DeleteByID(ID, year);

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

            return RedirectToAction("HRLeaveApplicationMasterDashBoard");

        }
    }
}
