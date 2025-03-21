using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Office.Interop.Excel;
using Newtonsoft.Json;
using System.Data;
using System.Net;
using static eTactWeb.Data.Common.CommonFunc;
using static eTactWeb.DOM.Models.Common;

namespace eTactwebInventory.Controllers
{
    public class HRLeaveOpeningMasterController : Controller
    {
        private readonly IDataLogic _IDataLogic;
        private readonly IHRLeaveOpeningMaster _IHRLeaveOpeningMaster;
        private readonly ILogger<HRLeaveOpeningMasterController> _logger;
        private readonly IMemoryCache _MemoryCache;
        private readonly IConfiguration iconfiguration;
        private readonly IIssueWithoutBom _IIssueWOBOM;
        public IWebHostEnvironment _IWebHostEnvironment { get; }

        public HRLeaveOpeningMasterController(ILogger<HRLeaveOpeningMasterController> logger, IDataLogic iDataLogic, IHRLeaveOpeningMaster iHRLeaveOpeningMaster, IMemoryCache iMemoryCache, EncryptDecrypt encryptDecrypt, IWebHostEnvironment iWebHostEnvironment, IConfiguration iconfiguration, IIssueWithoutBom IIssueWOBOM)
        {
            _logger = logger;
            _IDataLogic = iDataLogic;
            _IHRLeaveOpeningMaster = iHRLeaveOpeningMaster;
            _MemoryCache = iMemoryCache;
            _MemoryCache = iMemoryCache;
            _IWebHostEnvironment = iWebHostEnvironment;
            this.iconfiguration = iconfiguration;
            _IIssueWOBOM = IIssueWOBOM;
        }

        [Route("{controller}/Index")]
        public async Task<IActionResult> HRLeaveOpeningMaster(int Id,int year,string Mode)
        {
            TempData.Clear();
            _MemoryCache.Remove("KeyLeaveOpeningGrid");
            var MainModel = new HRLeaveOpeningMasterModel();
           
            MainModel.LeaveOpnYearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
            MainModel.ActualEntryBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
            MainModel.EntryByMachine= Environment.MachineName;

            if (!string.IsNullOrEmpty(Mode) &&  (Mode == "V" || Mode == "U"))
            {
                MainModel = await _IHRLeaveOpeningMaster.GetViewByID(Id,year).ConfigureAwait(false);
                MainModel.Mode = Mode;
               
                MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                    SlidingExpiration = TimeSpan.FromMinutes(55),
                    Size = 1024,
                };
                _MemoryCache.Set("KeyLeaveOpeningGrid", MainModel.HRLeaveOpeningDetailGrid, cacheEntryOptions);
            }

           

           
            HttpContext.Session.SetString("HRLeaveOpeningMaster", JsonConvert.SerializeObject(MainModel));
            return View(MainModel);
        }

        public async Task<JsonResult> GetEmpCat()
        {
            var JSON = await _IHRLeaveOpeningMaster.GetEmpCat();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }

        public async Task<JsonResult> GetDepartment(int empid)
        {
            var JSON = await _IHRLeaveOpeningMaster.GetDepartment(empid);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }

        public async Task<JsonResult> GetDesignation(int empid)
        {
            var JSON = await _IHRLeaveOpeningMaster.GetDesignation(empid);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }


        public async Task<JsonResult> GetLeaveName()
        {
            var JSON = await _IHRLeaveOpeningMaster.GetLeaveName();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }

        public async Task<JsonResult> GetShiftName()
        {
            var JSON = await _IHRLeaveOpeningMaster.GetShiftName();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetEmpCode()
        {
            var JSON = await _IHRLeaveOpeningMaster.GetEmpCode();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }

        public async Task<JsonResult> FillEntryId()
        {
            var JSON = await _IHRLeaveOpeningMaster.FillEntryId();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }

        public IActionResult AddLeaveOpeningDetail(HRLeaveOpeningMasterModel model)
        {
            try
            {
                if (model.Mode == "U")
                {

                    _MemoryCache.TryGetValue("KeyLeaveOpeningGrid", out IList<HRLeaveOpeningDetail> LeaveOpeningDetail);

                    var MainModel = new HRLeaveOpeningMasterModel();
                    var WorkOrderPGrid = new List<HRLeaveOpeningDetail>();
                    var OrderGrid = new List<HRLeaveOpeningDetail>();
                    var ssGrid = new List<HRLeaveOpeningDetail>();

                    var count = 0;
                    if (model != null)
                    {
                        if (LeaveOpeningDetail == null)
                        {
                            model.SeqNo = 1;
                            OrderGrid.Add(model);
                        }
                        else
                        {
                            if (LeaveOpeningDetail.Any(x => (x.LeaveName == model.LeaveName)))
                            {
                                return StatusCode(207, "Duplicate");
                            }
                            else
                            {
                                //count = WorkOrderProcessGrid.Count();
                                model.SeqNo = LeaveOpeningDetail.Count + 1;
                                OrderGrid = LeaveOpeningDetail.Where(x => x != null).ToList();
                                ssGrid.AddRange(OrderGrid);
                                OrderGrid.Add(model);

                            }

                        }

                        MainModel.HRLeaveOpeningDetailGrid = OrderGrid;

                        MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
                        {
                            AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                            SlidingExpiration = TimeSpan.FromMinutes(55),
                            Size = 1024,
                        };

                        _MemoryCache.Set("KeyLeaveOpeningGrid", MainModel.HRLeaveOpeningDetailGrid, cacheEntryOptions);
                    }
                    else
                    {
                        ModelState.TryAddModelError("Error", "Schedule List Cannot Be Empty...!");
                    }
                    return PartialView("_HRLeaveOpeningDetailGrid", MainModel);
                }
                else
                {

                    _MemoryCache.TryGetValue("KeyLeaveOpeningGrid", out IList<HRLeaveOpeningDetail> LeaveOpeningDetail);

                    var MainModel = new HRLeaveOpeningMasterModel();
                    var WorkOrderPGrid = new List<HRLeaveOpeningDetail>();
                    var OrderGrid = new List<HRLeaveOpeningDetail>();
                    var ssGrid = new List<HRLeaveOpeningDetail>();


                    if (model != null)
                    {
                        if (LeaveOpeningDetail == null)
                        {
                            model.SeqNo = 1;
                            OrderGrid.Add(model);
                        }
                        else
                        {
                            if (LeaveOpeningDetail.Any(x => (x.LeaveName == model.LeaveName)))
                            {
                                return StatusCode(207, "Duplicate");
                            }
                            else
                            {
                                //count = WorkOrderProcessGrid.Count();
                                model.SeqNo = LeaveOpeningDetail.Count + 1;
                                OrderGrid = LeaveOpeningDetail.Where(x => x != null).ToList();
                                ssGrid.AddRange(OrderGrid);
                                OrderGrid.Add(model);

                            }

                        }

                        MainModel.HRLeaveOpeningDetailGrid = OrderGrid;

                        MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
                        {
                            AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                            SlidingExpiration = TimeSpan.FromMinutes(55),
                            Size = 1024,
                        };

                        _MemoryCache.Set("KeyLeaveOpeningGrid", MainModel.HRLeaveOpeningDetailGrid, cacheEntryOptions);
                    }
                    else
                    {
                        ModelState.TryAddModelError("Error", "Schedule List Cannot Be Empty...!");
                    }
                    return PartialView("_HRLeaveOpeningDetailGrid", MainModel);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IActionResult ClearGrid()
        {
            _MemoryCache.Remove("KeyLeaveOpeningGrid");
            _MemoryCache.Remove("HRLeaveOpeningMasterModel");
            var MainModel = new HRLeaveOpeningMasterModel();
            return PartialView("_HRLeaveOpeningDetailGrid", MainModel);
        }

        public IActionResult EditItemRow(int SeqNo, string Mode)
        {
            IList<HRLeaveOpeningDetail> HRLeaveOpeningDetail = new List<HRLeaveOpeningDetail>();
            if (Mode == "U")
            {
                _MemoryCache.TryGetValue("KeyLeaveOpeningGrid", out HRLeaveOpeningDetail);
            }
            else
            {
                _MemoryCache.TryGetValue("KeyLeaveOpeningGrid", out HRLeaveOpeningDetail);
            }
            IEnumerable<HRLeaveOpeningDetail> SSBreakdownGrid = HRLeaveOpeningDetail;
            if (HRLeaveOpeningDetail != null)
            {
                SSBreakdownGrid = HRLeaveOpeningDetail.Where(x => x.SeqNo == SeqNo);

            }
            string JsonString = JsonConvert.SerializeObject(SSBreakdownGrid);
            return Json(JsonString);
        }

        public IActionResult DeleteItemFromGrid(int SeqNo, string Mode)
        {
            var MainModel = new HRLeaveOpeningMasterModel();
            if (Mode == "U")
            {
                _MemoryCache.TryGetValue("KeyLeaveOpeningGrid", out IList<HRLeaveOpeningDetail> ItemDetailGrid);

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
                    MainModel.HRLeaveOpeningDetailGrid = ItemDetailGrid;

                    MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
                    {
                        AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                        SlidingExpiration = TimeSpan.FromMinutes(55),
                        Size = 1024,
                    };

                    _MemoryCache.Set("KeyLeaveOpeningGrid", MainModel.HRLeaveOpeningDetailGrid, cacheEntryOptions);
                }
            }
            else
            {
                _MemoryCache.TryGetValue("KeyLeaveOpeningGrid", out IList<HRLeaveOpeningDetail> HRLeaveOpeningDetail);

                //_MemoryCache.TryGetValue("KeyPartCodePartyWiseGrid", out List<ItemDetailGrid> ItemDetailGrid);
                int Indx = Convert.ToInt32(SeqNo) - 1;

                if (HRLeaveOpeningDetail != null && HRLeaveOpeningDetail.Count > 0)
                {
                    HRLeaveOpeningDetail.RemoveAt(Convert.ToInt32(Indx));

                    Indx = 0;

                    foreach (var item in HRLeaveOpeningDetail)
                    {
                        Indx++;
                        item.SeqNo = Indx;
                    }
                    MainModel.HRLeaveOpeningDetailGrid = HRLeaveOpeningDetail;

                    MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
                    {
                        AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                        SlidingExpiration = TimeSpan.FromMinutes(55),
                        Size = 1024,
                    };

                    _MemoryCache.Set("KeyLeaveOpeningGrid", MainModel.HRLeaveOpeningDetailGrid, cacheEntryOptions);
                }
            }

            return PartialView("_HRLeaveOpeningDetailGrid", MainModel);
        }



        private static System.Data.DataTable GetDetailTable(IList<HRLeaveOpeningDetail> DetailList)
        {
            try
            {
                var GIGrid = new System.Data.DataTable();

                GIGrid.Columns.Add("LeaveOpnEntryId", typeof(int));
                GIGrid.Columns.Add("LeaveOpnYearCode", typeof(int));
                GIGrid.Columns.Add("EmpId", typeof(int));
                GIGrid.Columns.Add("SeqNo", typeof(int));
                GIGrid.Columns.Add("LeaveEntryId", typeof(int));
                GIGrid.Columns.Add("LeaveAccrualType", typeof(string));
                GIGrid.Columns.Add("OpeningBalance", typeof(decimal));
                GIGrid.Columns.Add("CarriedForward", typeof(decimal));
                GIGrid.Columns.Add("TotalLeaves", typeof(decimal));
                GIGrid.Columns.Add("MaxCarryForward", typeof(decimal));
                GIGrid.Columns.Add("LeaveEncashmentAllowed", typeof(string));
                GIGrid.Columns.Add("LeaveValidityPeriod", typeof(string));
                GIGrid.Columns.Add("MandatoryLeaveAfterdays", typeof(int));
                GIGrid.Columns.Add("MaxAllowedLeaves", typeof(decimal));
               
                foreach (var Item in DetailList)
                {
                    if (Item == null)
                    {
                        // Log if Item is null
                        Console.WriteLine("Item is null in the DetailList.");
                        continue; // Skip null items
                    }

                    GIGrid.Rows.Add(
                        new object[]
                        {
                    Item.LeaveOpnEntryId??0,
                    Item.LeaveOpnYearCode??0,
                    Item.EmpId ?? 0,
                    Item.SeqNo ?? 0 ,
                    Item.LeaveId ?? 0,
                    Item.LeaveAccrualType ?? "",
                    Item.OpeningBalance ?? 0 ,
                    Item.CarriedForward ?? 0,
                    Item.TotalLeaves ?? 0,
                    Item.MaxCarryForward ?? 0,
                    Item.LeaveEncashmentAllowed??"" ,
                    Item.LeaveValidityPeriod ?? "",
                    Item.MandatoryLeaveAfter ?? 0 ,
                    Item.MaxAllowedLeaves ?? 0  
                    
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
        [Route("{controller}/Index")]
        [HttpPost]
        public async Task<IActionResult> HRLeaveOpeningMaster(HRLeaveOpeningMasterModel model)
        {
            try
            {
                var GIGrid = new System.Data.DataTable();
                _MemoryCache.TryGetValue("KeyLeaveOpeningGrid", out List<HRLeaveOpeningDetail> HRLeaveOpeningDetail);


                model.CreatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                if (model.Mode == "U")
                {
                    model.UpdatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                    GIGrid = GetDetailTable(HRLeaveOpeningDetail);
                }
                else
                {
                    GIGrid = GetDetailTable(HRLeaveOpeningDetail);
                }
                var Result = await _IHRLeaveOpeningMaster.SaveMainData(model, GIGrid);
                if (Result != null)
                {
                    if (Result.StatusText == "Success" && Result.StatusCode == HttpStatusCode.OK)
                    {
                        ViewBag.isSuccess = true;
                        TempData["200"] = "200";
                        _MemoryCache.Remove("KeyLeaveOpeningGrid");
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

                return RedirectToAction("HRLeaveOpeningMasterDashBoard");

            }
            catch (Exception ex)
            {
                // Log and return the error
                //LogException<LedgerPartyWiseOpeningController>.WriteException(_logger, ex);
                var ResponseResult = new ResponseResult
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    StatusText = "Error",
                    Result = ex
                };
                return View("Error", ResponseResult);
            }
        }



        [HttpGet]
        [Route("HRLeaveOpeningMasterDashBoard")]
        public async Task<IActionResult> HRLeaveOpeningMasterDashBoard()
        {
            try
            {
                var model = new HRLeaveOpeningDashBoardModel();
                var result = await _IHRLeaveOpeningMaster.GetDashboardData().ConfigureAwait(true);
                DateTime now = DateTime.Now;

                if (result != null && result.Result != null)
                {
                    DataSet ds = result.Result;
                    if (ds != null && ds.Tables.Count > 0)
                    {
                        var dt = ds.Tables[0];
                        model.HRLeaveOpeningDashBoardDetail = CommonFunc.DataTableToList<HRLeaveOpeningDashBoardModel>(dt, "HRLeaveOpeningMaster");
                    }

                }

                return View(model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IActionResult> GetDetailData(string ReportType,string FromDate, string ToDate)
        {
            //model.Mode = "Search";
            var model = new HRLeaveOpeningDashBoardModel();
            model = await _IHRLeaveOpeningMaster.GetDashboardDetailData(ReportType,FromDate, ToDate);
           

            if (ReportType == "SUMMARY")
            {
                return PartialView("_HRLeaveOpeningDashBoardSummaryGrid", model);
            }
            else if (ReportType == "DETAIL")
            {
                return PartialView("_HRLeaveOpeningDashBoardDetailGrid", model);

            }
            return null;
        }

        public async Task<IActionResult> DeleteByID(int ID, int year,string EntryByMachineName)
        {
            var Result = await _IHRLeaveOpeningMaster.DeleteByID(ID, year, EntryByMachineName);

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

            return RedirectToAction("HRLeaveOpeningMasterDashBoard");

        }
    }
}
