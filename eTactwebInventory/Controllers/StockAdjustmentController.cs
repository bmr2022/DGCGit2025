using eTactWeb.Data.Common;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Office.Interop.Excel;
using Newtonsoft.Json;
//using NuGet.Configuration;
//using NuGet.Packaging;
using OfficeOpenXml;
using System;
using eTactWeb.DOM.Models;
using System.Runtime.Caching;
using static eTactWeb.Data.Common.CommonFunc;
using static eTactWeb.DOM.Models.Common;
using DataTable = System.Data.DataTable;
using eTactWeb.DOM.Models;
using System.Net;
using System.Data;

namespace eTactWeb.Controllers
{
    public class StockAdjustmentController : Controller
    {
        public IDataLogic IDataLogic { get; }
        public IMemoryCache IMemoryCache { get; }
        public IStockAdjustment IStockAdjust { get; }
        public IWebHostEnvironment IWebHostEnvironment { get; }
        public ILogger<StockAdjustmentController> Logger { get; }
        private EncryptDecrypt EncryptDecrypt { get; }
        public StockAdjustmentController(IStockAdjustment iStockAdjust, IDataLogic iDataLogic, IMemoryCache iMemoryCache, ILogger<StockAdjustmentController> logger, EncryptDecrypt encryptDecrypt, IWebHostEnvironment iWebHostEnvironment)
        {
            IStockAdjust = iStockAdjust;
            IDataLogic = iDataLogic;
            IMemoryCache = iMemoryCache;
            Logger = logger;
            EncryptDecrypt = encryptDecrypt;
            IWebHostEnvironment = iWebHostEnvironment;
        }
        [HttpGet]
        [Route("{controller}/Index")]
        public async Task<IActionResult> StockAdjustment(int ID, string Mode, int YC, string FromDate = "", string ToDate = "", string PartCode = "", string ItemName = "", string StoreWorkcenter = "", string StoreName = "", string WCName = "", string SummaryDetail = "", string Searchbox = "")
        {
            //RoutingModel model = new RoutingModel();
            ViewData["Title"] = "StockAdjustment Details";
            TempData.Clear();
            IMemoryCache.Remove("KeyStockAdjustGrid");
            IMemoryCache.Remove("KeyStockMultiBatchAdjustGrid");
            var MainModel = new StockAdjustmentModel();

            MainModel.FinFromDate = HttpContext.Session.GetString("FromDate");
            MainModel.FinToDate = HttpContext.Session.GetString("ToDate");
            MainModel.CC = HttpContext.Session.GetString("Branch");
            MainModel.YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));

            if (Mode != "U")
            {
                MainModel.Uid = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                MainModel.entryByEmp = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                MainModel.entryByEmpName = HttpContext.Session.GetString("EmpName");
            }


            if (!string.IsNullOrEmpty(Mode) && ID > 0 && (Mode == "V" || Mode == "U"))
            {
                MainModel = await IStockAdjust.GetViewByID(ID, Mode, YC);
                MainModel.Mode = Mode;
                MainModel.ID = ID;
                MainModel = await BindModel(MainModel);

            }
            else
            {
                MainModel = await BindModel(MainModel);
            }

            MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                SlidingExpiration = TimeSpan.FromMinutes(55),
                Size = 1024,
            };

            MainModel.FromDateBack = FromDate;
            MainModel.ToDateBack = ToDate;
            MainModel.PartCodeBack = PartCode;
            MainModel.ItemNameBack = ItemName;
            MainModel.StoreWorkcenterBack = StoreWorkcenter;
            MainModel.StoreNameBack = StoreName;
            MainModel.WorkCenterBack = WCName;
            MainModel.SummaryDetailBack = SummaryDetail;
            MainModel.GlobalSearchBack = Searchbox;
            IMemoryCache.Set("KeyStockAdjustGrid", MainModel.StockAdjustModelGrid, cacheEntryOptions);
            //IMemoryCache.Set("KeyStockAdjustGrid", model, cacheEntryOptions);
            return View(MainModel);
        }

        public IActionResult AddMultipleStockDetail(List<StockAdjustmentDetail> model)
        {
            try
            {
                var MainModel = new StockAdjustmentModel();
                var StockGrid = new List<StockAdjustmentDetail>();
                var StockAdjustGrid = new List<StockAdjustmentDetail>();

                var SeqNo = 1;
                foreach (var item in model)
                {
                    IMemoryCache.TryGetValue("KeyStockMultiBatchAdjustGrid", out IList<StockAdjustmentDetail> StockAdjustmentGrid);

                    if (model != null)
                    {
                        if (StockAdjustmentGrid == null)
                        {
                            item.SeqNo = SeqNo++;
                            StockGrid.Add(item);
                        }
                        else
                        {
                            if (item.Storeid != 0)
                            {
                                if (StockAdjustmentGrid.Any(x => x.ItemCode == item.ItemCode && x.Storeid == item.Storeid && x.batchno == item.batchno && x.uniqbatchno == item.uniqbatchno))
                                {
                                    return StatusCode(207, "Duplicate");
                                }
                            }
                            else
                            {
                                if (StockAdjustmentGrid.Any(x => x.ItemCode == item.ItemCode && x.Wcid == item.Wcid && x.batchno == item.batchno && x.uniqbatchno == item.uniqbatchno))
                                {
                                    return StatusCode(207, "Duplicate");
                                }
                            }

                            item.SeqNo = StockAdjustmentGrid.Count + 1;
                            StockGrid = StockAdjustmentGrid.Where(x => x != null).ToList();
                            StockAdjustGrid.AddRange(StockGrid);
                            StockGrid.Add(item);
                        }
                        MainModel.StockAdjustModelGrid = StockGrid;

                        MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
                        {
                            AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                            SlidingExpiration = TimeSpan.FromMinutes(55),
                            Size = 1024,
                        };

                        IMemoryCache.Set("KeyStockMultiBatchAdjustGrid", MainModel.StockAdjustModelGrid, cacheEntryOptions);
                        IMemoryCache.Set("KeyStockAdjustGrid", MainModel.StockAdjustModelGrid, cacheEntryOptions);

                    }
                    else
                    {
                        ModelState.TryAddModelError("Error", "Schedule List Cannot Be Empty...!");
                    }
                }


                return PartialView("_StockAdjustGrid", MainModel);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<JsonResult> StockAdjBackDatePassword()
        {
            var JSON = await IStockAdjust.StockAdjBackDatePassword();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public IActionResult AddStockDetail(StockAdjustmentDetail model)
        {
            try
            {
                IMemoryCache.TryGetValue("KeyStockAdjustGrid", out IList<StockAdjustmentDetail> StockAdjustmentGrid);

                var MainModel = new StockAdjustmentModel();
                var StockGrid = new List<StockAdjustmentDetail>();
                var StockAdjustGrid = new List<StockAdjustmentDetail>();


                if (model != null)
                {
                    if (StockAdjustmentGrid == null)
                    {
                        model.SeqNo = 1;
                        StockGrid.Add(model);
                    }
                    else
                    {
                        if (model.Storeid != 0)
                        {
                            if (StockAdjustmentGrid.Any(x => x.ItemCode == model.ItemCode && x.Storeid == model.Storeid && x.batchno == model.batchno))
                            {
                                return StatusCode(207, "Duplicate");
                            }
                        }
                        else
                        {
                            if (StockAdjustmentGrid.Any(x => x.ItemCode == model.ItemCode && x.Wcid == model.Wcid && x.batchno == model.batchno /*&& x.uniqbatchno == model.uniqbatchno*/))
                            {
                                return StatusCode(207, "Duplicate");
                            }
                        }

                        model.SeqNo = StockAdjustmentGrid.Count + 1;
                        StockGrid = StockAdjustmentGrid.Where(x => x != null).ToList();
                        StockAdjustGrid.AddRange(StockGrid);
                        StockGrid.Add(model);

                    }

                    MainModel.StockAdjustModelGrid = StockGrid;

                    MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
                    {
                        AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                        SlidingExpiration = TimeSpan.FromMinutes(55),
                        Size = 1024,
                    };

                    IMemoryCache.Set("KeyStockAdjustGrid", MainModel.StockAdjustModelGrid, cacheEntryOptions);
                }
                else
                {
                    ModelState.TryAddModelError("Error", "Schedule List Cannot Be Empty...!");
                }
                return PartialView("_StockAdjustGrid", MainModel);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("{controller}/Index")]
        public async Task<IActionResult> StockAdjustment(StockAdjustmentModel model)
        {
            try
            {
                var SAGrid = new DataTable();

                IMemoryCache.TryGetValue("KeyStockAdjustGrid", out IList<StockAdjustmentDetail> StocAdjustmentDetail);

                if (StocAdjustmentDetail == null)
                {
                    ModelState.Clear();
                    ModelState.TryAddModelError("StockAdjustmentDetail", "Stock Adjustment Grid Should Have Atleast 1 Item...!");
                    return View("StockADjustment", model);
                }

                else
                {
                    model.CC = HttpContext.Session.GetString("Branch");
                    //model.ActualEnteredBy   = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
                    model.Uid = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                    if (model.Mode == "U")
                    {
                        model.UpdatedByEmp = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                        model.UpdatedByEmpName = HttpContext.Session.GetString("EmpName");
                        SAGrid = GetDetailTable(StocAdjustmentDetail);
                    }
                    else
                    {
                        model.entryByEmp = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                        model.entryByEmpName = HttpContext.Session.GetString("EmpName");
                        SAGrid = GetDetailTable(StocAdjustmentDetail);
                    }

                    var Result = await IStockAdjust.SaveStockAdjust(model, SAGrid);

                    if (Result != null)
                    {
                        if (Result.StatusText == "Success" && Result.StatusCode == HttpStatusCode.OK)
                        {
                            ViewBag.isSuccess = true;
                            TempData["200"] = "200";
                            IMemoryCache.Remove(SAGrid);
                            var model1 = new StockAdjustmentModel();
                            model1.FinFromDate = HttpContext.Session.GetString("FromDate");
                            model1.FinToDate = HttpContext.Session.GetString("ToDate");
                            model1.YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
                            model1.CC = HttpContext.Session.GetString("Branch");
                            //model1.ActualEnteredByName = HttpContext.Session.GetString("EmpName");
                            model1.CreatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                            IMemoryCache.Remove("KeyStockAdjustGrid");
                            return View(model1);
                        }
                        if (Result.StatusText == "Updated" && Result.StatusCode == HttpStatusCode.Accepted)
                        {
                            ViewBag.isSuccess = true;
                            TempData["202"] = "202";
                            var model1 = new StockAdjustmentModel();
                            model1.FinFromDate = HttpContext.Session.GetString("FromDate");
                            model1.FinToDate = HttpContext.Session.GetString("ToDate");
                            model1.YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
                            model1.CC = HttpContext.Session.GetString("Branch");
                            //model1.ActualEnteredByName = HttpContext.Session.GetString("EmpName");
                            model1.CreatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                            IMemoryCache.Remove("KeyStockAdjustGrid");
                            return View(model1);
                        }
                        if (Result.StatusText == "Error" && Result.StatusCode == HttpStatusCode.InternalServerError)
                        {
                            var errNum = Result.Result.Message.ToString().Split(":")[1];
                            if (errNum == " 2627")
                            {
                                ViewBag.isSuccess = false;
                                TempData["2627"] = "2627";
                                Logger.LogError("\n \n ********** LogError ********** \n " + JsonConvert.SerializeObject(Result) + "\n \n");
                                var model2 = await BindModel(null);
                                model2.FinFromDate = HttpContext.Session.GetString("FromDate");
                                model2.FinToDate = HttpContext.Session.GetString("ToDate");
                                model2.YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
                                model2.CC = HttpContext.Session.GetString("Branch");
                                // model2.ActualEnteredByName = HttpContext.Session.GetString("EmpName");
                                model2.CreatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                                return View(model2);
                            }

                            ViewBag.isSuccess = false;
                            TempData["500"] = "500";
                            Logger.LogError("\n \n ********** LogError ********** \n " + JsonConvert.SerializeObject(Result) + "\n \n");
                            // return View("Error", Result);
                            return View(model);
                        }
                    }
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                LogException<StockAdjustmentController>.WriteException(Logger, ex);


                var ResponseResult = new ResponseResult()
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    StatusText = "Error",
                    Result = ex
                };

                return View("Error", ResponseResult);
                //return View(model);
            }
        }

        //public async Task<IActionResult> AddMainStockDetail(List<StockAdjustmentDetail> model,string StoreWork)
        //{
        //    try
        //    {

        //        var MainModel = new StockAdjustmentModel();
        //        MainModel.StoreWorkCenter = StoreWork;
        //        var StockGrid = new List<StockAdjustmentDetail>();
        //        var StockAdjustGrid = new List<StockAdjustmentDetail>();
        //        List<StockAdjustmentDetail> StockAdjustmentGrid = new List<StockAdjustmentDetail>();
        //        var SeqNo = 1;
        //        foreach (var item in model)
        //        {
        //            IMemoryCache.TryGetValue("KeyStockMultiBatchAdjustGrid", out StockAdjustmentGrid);

        //            if (model != null)
        //            {
        //                if (StockAdjustmentGrid == null)
        //                {
        //                    item.SeqNo = SeqNo++;
        //                    StockGrid.Add(item);
        //                }
        //                else
        //                {
        //                    if (item.Storeid != 0)
        //                    {
        //                        if (StockAdjustmentGrid.Any(x => x.ItemCode == item.ItemCode && x.Storeid == item.Storeid && x.batchno == item.batchno && x.uniqbatchno == item.uniqbatchno))
        //                        {
        //                            return StatusCode(207, "Duplicate");
        //                        }
        //                    }
        //                    else
        //                    {
        //                        if (StockAdjustmentGrid.Any(x => x.ItemCode == item.ItemCode && x.Wcid == item.Wcid && x.batchno == item.batchno && x.uniqbatchno == item.uniqbatchno))
        //                        {
        //                            return StatusCode(207, "Duplicate");
        //                        }
        //                    }

        //                    item.SeqNo = StockAdjustmentGrid.Count + 1;
        //                    StockGrid = StockAdjustmentGrid.Where(x => x != null).ToList();
        //                    StockAdjustGrid.AddRange(StockGrid);
        //                    StockGrid.Add(item);
        //                }
        //                MainModel.StockAdjustModelGrid = StockGrid;

        //                MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
        //                {
        //                    AbsoluteExpiration = DateTime.Now.AddMinutes(60),
        //                    SlidingExpiration = TimeSpan.FromMinutes(55),
        //                    Size = 1024,
        //                };

        //                IMemoryCache.Set("KeyStockMultiBatchAdjustGrid", MainModel.StockAdjustModelGrid, cacheEntryOptions);
        //                IMemoryCache.Set("KeyStockAdjustGrid", MainModel.StockAdjustModelGrid, cacheEntryOptions);

        //            }
        //            else
        //            {
        //                ModelState.TryAddModelError("Error", "Schedule List Cannot Be Empty...!");
        //            }
        //        }

        //        MainModel.CC = HttpContext.Session.GetString("Branch");
        //        MainModel.Uid = Convert.ToInt32(HttpContext.Session.GetString("UID"));
        //        MainModel.entryByEmp = Convert.ToInt32(HttpContext.Session.GetString("UID"));
        //        MainModel.entryByEmpName = HttpContext.Session.GetString("EmpName");
        //        MainModel.YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
        //        MainModel.EntryDate = DateTime.Now.ToString();
        //        MainModel.MachineNo = Environment.MachineName;
        //        var ItemGridList = new DataTable();
        //        ItemGridList = GetDetailTable(MainModel.StockAdjustModelGrid);

        //          var Result = await IStockAdjust.SaveStockAdjust(MainModel,ItemGridList);

        //        if (Result != null)
        //        {
        //            if (Result.StatusText == "Success" && Result.StatusCode == HttpStatusCode.OK)
        //            {
        //                ViewBag.isSuccess = true;
        //                TempData["200"] = "200";
        //            }
        //            if (Result.StatusText == "Updated" && Result.StatusCode == HttpStatusCode.Accepted)
        //            {
        //                ViewBag.isSuccess = true;
        //                TempData["202"] = "202";
        //            }
        //            if (Result.StatusText == "Error" && Result.StatusCode == HttpStatusCode.InternalServerError)
        //            {
        //                ViewBag.isSuccess = false;
        //                TempData["500"] = "500";
        //                Logger.LogError("\n \n ********** LogError ********** \n " + JsonConvert.SerializeObject(Result) + "\n \n");
        //                return View("Error", Result);
        //            }
        //        }

        //        return RedirectToAction(nameof(ImportStock));
        //    }
        //    catch (Exception ex)
        //    {
        //        LogException<StockAdjustmentController>.WriteException(Logger, ex);

        //        var ResponseResult = new ResponseResult()
        //        {
        //            StatusCode = HttpStatusCode.InternalServerError,
        //            StatusText = "Error",
        //            Result = ex
        //        };

        //        return View("Error", ResponseResult);
        //    }
        //}

        public IActionResult ClearGrid()
        {
            IMemoryCache.Remove("KeyStockAdjustGrid");
            var MainModel = new StockAdjustmentModel();
            return PartialView("_StockAdjustGrid", MainModel);
        }
        public IActionResult ClearMultiBatchGrid()
        {
            IMemoryCache.Remove("KeyStockMultiBatchAdjustGrid");
            var MainModel = new StockAdjustmentModel();
            return PartialView("_StockAdjustGrid", MainModel);
        }
        public async Task<IActionResult> Dashboard(int ItemCode = 0, string PartCode = "", string ItemName = "", string StoreWorkCenter = "", string StoreName = "", string WCName = "", string FromDate = "", string ToDate = "", string SummaryDetail = "")
        {
            IMemoryCache.Remove("KeyStockAdjustGrid");
            var model = new SADashborad();
            DateTime now = DateTime.Now;
            //DateTime firstDayOfMonth = new DateTime(now.Year, now.Month, 1);
            if (FromDate == "" && ToDate == "")
            {
                model.FromDate = new DateTime(now.Year, now.Month, 1).ToString("dd/MM/yyyy").Replace("-", "/");
                //model.FromDate = ParseFormattedDate((model.FromDate).Split(" ")[0]);
                model.ToDate = (DateTime.Today).ToString("dd/MM/yyyy").Replace("-", "/");
                //model.ToDate = ParseFormattedDate((model.ToDate).Split(" ")[0]);
            }
            else
            {
                model.FromDate = FromDate;
                model.ToDate = ToDate;
            }
            model.FromDate = ParseFormattedDate((model.FromDate).Split(" ")[0]);
            model.ToDate = ParseFormattedDate((model.ToDate).Split(" ")[0]);
            var Result = await IStockAdjust.GetDashboardData(model);

            if (Result != null)
            {
                var _List = new List<TextValue>();
                DataSet DS = Result.Result;

                var DT = DS.Tables[0].DefaultView.ToTable(true, "EntryId", "Yearcode",
                    "SlipNo", "StockAdjustmentDate", "StoreWorkcenter", "EntryDate", "entryByEmp",
                                "enterByEmpName", "UpdatedByEmpName", "UpdatedbyEmp", "UpdatedOn", "EntryByMachineName");

                model.SADashboard = CommonFunc.DataTableToList<StockAdjustDashboard>(DT, "StockAdjustment");
                model.ItemCode = ItemCode;
                model.PartCode = PartCode;
                model.ItemName = ItemName;
                model.StoreWorkcenter = StoreWorkCenter;
                model.StoreName = StoreName;
                model.WCName = WCName;
                model.FromDate1 = FromDate;
                model.ToDate1 = ToDate;
                model.SummaryDetail = SummaryDetail;
            }

            return View(model);
        }

        public async Task<IActionResult> DetailDashboard(SADashborad model)
        {
            IMemoryCache.Remove("KeyStockAdjustGrid");
            DateTime now = DateTime.Now;
            //DateTime firstDayOfMonth = new DateTime(now.Year, now.Month, 1);
            //model.FromDate = new DateTime(now.Year, now.Month, 1).ToString("dd/MM/yyyy").Replace("-", "/");
            //model.ToDate = new DateTime(DateTime.Today.Year + 1, 3, 31).ToString("dd/MM/yyyy").Replace("-", "/");
            var Result = await IStockAdjust.GetDashboardData(model);

            if (Result != null)
            {
                var _List = new List<TextValue>();
                DataSet DS = Result.Result;

                var DT = DS.Tables[0].DefaultView.ToTable(true, "EntryId", "Yearcode",
                    "SlipNo", "StockAdjustmentDate", "StoreWorkcenter", "PartCode", "ItemName", "storeid", "wcid", "Unit",
                    "TotalStock", "LotStock", "altUnit", "AltQty", "ActuleStockQty", "AdjType", "Rate"
                    , "WorkCenter", "Amount", "batchno", "uniquebatchno", "reasonOfAdjutment", "AdjQty",
                    "StoreName", "EntryDate", "entryByEmp", "Itemcode", "UpdatedbyEmp", "enterByEmpName", "UpdatedByEmpName"
                    , "UpdatedOn", "EntryByMachineName");

                model.SADashboard = CommonFunc.DataTableToList<StockAdjustDashboard>(DT, "StockAdjustmentDetail");

            }

            return PartialView("_SADetailDashboardGrid", model);
        }

        public async Task<IActionResult> DeleteByID(int ID, int YC, int ItemCode, string PartCode, string ItemName, string StoreWorkCenter, string StoreName, string WCName, string FromDate, string ToDate, string SummaryDetail,int entryByEmp,string EntryByMachineName)
        {
            var Result = await IStockAdjust.DeleteByID(ID, YC,entryByEmp,EntryByMachineName).ConfigureAwait(false);

            if (Result.StatusText == "Deleted" || Result.StatusCode == HttpStatusCode.Gone)
            {
                ViewBag.isSuccess = true;
                TempData["410"] = "410";
            }
            else
            {
                ViewBag.isSuccess = false;
                TempData["500"] = "500";
            }
            return RedirectToAction("Dashboard", new { ItemCode = ItemCode, PartCode = PartCode, ItemName = ItemName, StoreWorkCenter = StoreWorkCenter, StoreName = StoreName, WCName = WCName, FromDate = FromDate, ToDate = ToDate, SummaryDetail = SummaryDetail, entryByEmp=entryByEmp,EntryByMachineName=EntryByMachineName });
        }
        public async Task<IActionResult> GetSearchData(SADashborad model)
        {
            var Result = await IStockAdjust.GetDashboardData(model);
            DataSet DS = Result.Result;
            var DT = new DataTable();
            if (model.SummaryDetail == "Summary")
            {
                 DT = DS.Tables[0].DefaultView.ToTable(true, "EntryId", "Yearcode",
                        "SlipNo", "StockAdjustmentDate", "StoreWorkcenter", "EntryDate", "entryByEmp",
                                    "enterByEmpName", "UpdatedbyEmp", "UpdatedByEmpName", "UpdatedOn", "EntryByMachineName");
            }
            else
            {
                 DT = DS.Tables[0].DefaultView.ToTable(true, "EntryId", "Yearcode",
                        "SlipNo", "StockAdjustmentDate", "StoreWorkcenter", "PartCode", "ItemName", "storeid", "wcid", "Unit", "TotalStock", "LotStock", "altUnit", 
                        "AltQty", "ActuleStockQty", "AdjQty", "AdjType", "Rate", "Amount", "batchno", "uniquebatchno", "reasonOfAdjutment", "StoreName", "WorkCenter",
                        "EntryDate", "entryByEmp", "Itemcode", "UpdatedbyEmp",
                                    "enterByEmpName", "UpdatedByEmpName", "UpdatedOn", "EntryByMachineName");
            }

            model.SADashboard = CommonFunc.DataTableToList<StockAdjustDashboard>(DT, "StockAdjustment");

            return PartialView("_SADashboardGrid", model);
        }

        public IActionResult DeleteItemRow(int SeqNo, string Mode)
        {
            var MainModel = new StockAdjustmentModel();
            if (Mode == "U")
            {
                int Indx = Convert.ToInt32(SeqNo) - 1;
                IMemoryCache.TryGetValue("KeyStockAdjustGrid", out List<StockAdjustmentDetail> StockAdjustDetail);

                if (StockAdjustDetail != null && StockAdjustDetail.Count > 0)
                {
                    StockAdjustDetail.RemoveAt(Convert.ToInt32(Indx));

                    Indx = 0;

                    foreach (var item in StockAdjustDetail)
                    {
                        Indx++;
                        item.SeqNo = Indx;
                    }
                    MainModel.StockAdjustModelGrid = StockAdjustDetail;

                    MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
                    {
                        AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                        SlidingExpiration = TimeSpan.FromMinutes(55),
                        Size = 1024,
                    };

                    IMemoryCache.Set("KeyStockAdjustGrid", MainModel.StockAdjustModelGrid, cacheEntryOptions);
                }
            }
            else
            {
                IMemoryCache.TryGetValue("KeyStockAdjustGrid", out List<StockAdjustmentDetail> StockAdjustDetail);
                int Indx = Convert.ToInt32(SeqNo) - 1;

                if (StockAdjustDetail != null && StockAdjustDetail.Count > 0)
                {
                    StockAdjustDetail.RemoveAt(Convert.ToInt32(Indx));

                    Indx = 0;

                    foreach (var item in StockAdjustDetail)
                    {
                        Indx++;
                        item.SeqNo = Indx;
                    }
                    MainModel.StockAdjustModelGrid = StockAdjustDetail;

                    MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
                    {
                        AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                        SlidingExpiration = TimeSpan.FromMinutes(55),
                        Size = 1024,
                    };

                    IMemoryCache.Set("KeyStockAdjustGrid", MainModel.StockAdjustModelGrid, cacheEntryOptions);
                }
            }

            return PartialView("_StockAdjustGrid", MainModel);
        }

        //public IActionResult DeleteExcelItemRow(int SeqNo, string Mode)
        //{
        //    var MainModel = new StockAdjustmentModel();
        //    if (Mode == "U")
        //    {
        //        int Indx = Convert.ToInt32(SeqNo) - 1;
        //        IMemoryCache.TryGetValue("KeyStockAdjustGrid", out List<StockAdjustmentDetail> StockAdjustDetail);

        //        if (StockAdjustDetail != null && StockAdjustDetail.Count > 0)
        //        {
        //            StockAdjustDetail.RemoveAt(Convert.ToInt32(Indx));

        //            Indx = 0;

        //            foreach (var item in StockAdjustDetail)
        //            {
        //                Indx++;
        //                item.SeqNo = Indx;
        //            }
        //            MainModel.ExcelDetailGrid = StockAdjustDetail;
        //            MainModel.ImportMode = "Y";
        //            MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
        //            {
        //                AbsoluteExpiration = DateTime.Now.AddMinutes(60),
        //                SlidingExpiration = TimeSpan.FromMinutes(55),
        //                Size = 1024,
        //            };

        //            IMemoryCache.Set("KeyStockAdjustGrid", MainModel.ExcelDetailGrid, cacheEntryOptions);
        //        }
        //    }
        //    else
        //    {
        //        IMemoryCache.TryGetValue("KeyStockAdjustGrid", out List<StockAdjustmentDetail> StockAdjustDetail);
        //        int Indx = Convert.ToInt32(SeqNo) - 1;

        //        if (StockAdjustDetail != null && StockAdjustDetail.Count > 0)
        //        {
        //            StockAdjustDetail.RemoveAt(Convert.ToInt32(Indx));

        //            Indx = 0;

        //            foreach (var item in StockAdjustDetail)
        //            {
        //                Indx++;
        //                item.SeqNo = Indx;
        //            }
        //            MainModel.ExcelDetailGrid = StockAdjustDetail;
        //            MainModel.ImportMode = "Y";

        //            MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
        //            {
        //                AbsoluteExpiration = DateTime.Now.AddMinutes(60),
        //                SlidingExpiration = TimeSpan.FromMinutes(55),
        //                Size = 1024,
        //            };

        //            IMemoryCache.Set("KeyStockAdjustGrid", MainModel.ExcelDetailGrid, cacheEntryOptions);
        //        }
        //    }

        //    return PartialView("_StockAdjustGrid", MainModel);
        //}

        private static DataTable GetExcelDetailTable(IList<StockAdjustmentModel> DetailList)
        {
            var DTSSGrid = new DataTable();

            DTSSGrid.Columns.Add("EntryId", typeof(int));
            DTSSGrid.Columns.Add("YearCode", typeof(int));
            DTSSGrid.Columns.Add("SeqNo", typeof(int));
            DTSSGrid.Columns.Add("ItemCode", typeof(int));
            DTSSGrid.Columns.Add("Unit", typeof(string));
            DTSSGrid.Columns.Add("LotStock", typeof(float));
            DTSSGrid.Columns.Add("TotalStock", typeof(float));
            DTSSGrid.Columns.Add("altUnit", typeof(string));
            DTSSGrid.Columns.Add("AltQty", typeof(float));
            DTSSGrid.Columns.Add("ActuleStockQty", typeof(float));
            DTSSGrid.Columns.Add("AdjQty", typeof(float));
            DTSSGrid.Columns.Add("AdjType", typeof(string));
            DTSSGrid.Columns.Add("storeid", typeof(int));
            DTSSGrid.Columns.Add("wcid", typeof(int));
            DTSSGrid.Columns.Add("rate", typeof(float));
            DTSSGrid.Columns.Add("Amount", typeof(float));
            DTSSGrid.Columns.Add("batchno", typeof(string));
            DTSSGrid.Columns.Add("uniquebatchno", typeof(string));
            DTSSGrid.Columns.Add("reasonOfAdjustment", typeof(string));
            //DateTime DeliveryDt = new DateTime();
            foreach (var Item in DetailList)
            {
                string uniqueString = Guid.NewGuid().ToString();
                DTSSGrid.Rows.Add(
                    new object[]
                    {
                   0,
                   0,
                    Item.SeqNo,
                    Item.ItemCode,
                    Item.Unit,
                    Item.LotStock,
                    Item.TotalStock,
                    Item.altUnit,
                    Item.AltQty,
                    Item.ActualStockQty,
                    Item.AdjQty,
                    Item.AdjType,
                    Item.Storeid,
                    Item.Wcid,
                    Item.Rate,
                    Item.Amount,
                    Item.batchno,
                    Item.uniqbatchno == null ? "" : Item.uniqbatchno, // this is uniqbatchno
                    Item.reasonOfAdjustment,
                    });
            }
            DTSSGrid.Dispose();
            return DTSSGrid;
        }

        private static DataTable GetDetailTable(IList<StockAdjustmentDetail> DetailList)
        {
            var DTSSGrid = new DataTable();

            DTSSGrid.Columns.Add("EntryId", typeof(int));
            DTSSGrid.Columns.Add("YearCode", typeof(int));
            DTSSGrid.Columns.Add("SeqNo", typeof(int));
            DTSSGrid.Columns.Add("ItemCode", typeof(int));
            DTSSGrid.Columns.Add("Unit", typeof(string));
            DTSSGrid.Columns.Add("LotStock", typeof(float));
            DTSSGrid.Columns.Add("TotalStock", typeof(float));
            DTSSGrid.Columns.Add("altUnit", typeof(string));
            DTSSGrid.Columns.Add("AltQty", typeof(float));
            DTSSGrid.Columns.Add("ActuleStockQty", typeof(float));
            DTSSGrid.Columns.Add("AdjQty", typeof(float));
            DTSSGrid.Columns.Add("AdjType", typeof(string));
            DTSSGrid.Columns.Add("storeid", typeof(int));
            DTSSGrid.Columns.Add("wcid", typeof(int));
            DTSSGrid.Columns.Add("rate", typeof(float));
            DTSSGrid.Columns.Add("Amount", typeof(float));
            DTSSGrid.Columns.Add("batchno", typeof(string));
            DTSSGrid.Columns.Add("uniquebatchno", typeof(string));
            DTSSGrid.Columns.Add("reasonOfAdjustment", typeof(string));
            //DateTime DeliveryDt = new DateTime();
            foreach (var Item in DetailList)
            {
                string uniqueString = Guid.NewGuid().ToString();
                DTSSGrid.Rows.Add(
                    new object[]
                    {
                   0,
                   0,
                    Item.SeqNo,
                    Item.ItemCode,
                    Item.Unit,
                    Item.LotStock,
                    Item.TotalStock,
                    Item.altUnit ?? "",
                    Item.AltQty,
                    Item.ActualStockQty,
                    Item.AdjQty,
                    Item.AdjType,
                    Item.Storeid,
                    Item.Wcid,
                    Item.Rate,
                    Item.Amount,
                    Item.batchno,
                    Item.uniqbatchno == null ? "" : Item.uniqbatchno, // this is uniqbatchno
                    Item.reasonOfAdjustment,
                    });
            }
            DTSSGrid.Dispose();
            return DTSSGrid;
        }
        public async Task<JsonResult> EditItemRows(int SeqNo)
        {
            var MainModel = new StockAdjustmentModel();
            IMemoryCache.TryGetValue("KeyStockAdjustGrid", out List<StockAdjustmentDetail> StockAdjutGrid);
            var SAGrid = StockAdjutGrid.Where(x => x.SeqNo == SeqNo);
            string JsonString = JsonConvert.SerializeObject(SAGrid);
            return Json(JsonString);
        }
        private async Task<StockAdjustmentModel> BindModel(StockAdjustmentModel model)
        {
            var oDataSet = new DataSet();
            var _List = new List<TextValue>();
            oDataSet = await IStockAdjust.BindAllDropDowns("BINDALLDROPDOWN");

            if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
            {

                //foreach (DataRow row in oDataSet.Tables[1].Rows)
                //{
                //    _List.Add(new TextValue
                //    {
                //        Value = row["entryid"].ToString(),
                //        Text = row["StageDescription"].ToString()
                //    });
                //}
                //model.StageList = _List;
                //_List = new List<TextValue>();


            }
            return model;
        }
        public async Task<JsonResult> FillPartCode()
        {
            var JSON = await IStockAdjust.FillPartCode("FillPartCode");
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillItemName()
        {
            var JSON = await IStockAdjust.FillItemName("FillItemName");
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetmaxStockAdjustDate(int ItemCode)
        {
            var JSON = await IStockAdjust.GetmaxStockAdjustDate("GetEachItemsSADate", ItemCode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> StockAdjustByFeaturesOptions()
        {
            var JSON = await IStockAdjust.StockAdjustByFeaturesOptions("StockAdjustByFeaturesOptions");
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillLotStock(int ItemCode, int Storeid, string UniqueBatchNo, string BatchNo)
        {
            var JSON = await IStockAdjust.FillLotStock(ItemCode, Storeid, UniqueBatchNo, BatchNo);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillRateAmount(int ItemCode, int YearCode, string UniqueBatchNo = "", string BatchNo = "")
        {
            var JSON = await IStockAdjust.FillRateAmount(ItemCode, YearCode, UniqueBatchNo, BatchNo);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillTotalStock(int ItemCode, int Storeid)
        {
            var JSON = await IStockAdjust.FillTotalStock(ItemCode, Storeid);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetAltUnitQty(int ItemCode, float AltQty, float UnitQty)
        {
            var JSON = await IStockAdjust.GetAltUnitQty(ItemCode, AltQty, UnitQty);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillStore()
        {
            var JSON = await IStockAdjust.FillStore("FillStore");
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillWorkCenter()
        {
            var JSON = await IStockAdjust.FillWorkCenter("FillWorkCenter");
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }

        public async Task<JsonResult> CheckLastTransDate(string TransDate, int ItemCode, int StoreWC, int YearCode, string batchno, string uniquebatchno, string Flag)
        {
            var JSON = await IStockAdjust.CheckLastTransDate(TransDate, ItemCode, StoreWC, YearCode, batchno, uniquebatchno, Flag);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> NewEntryId(int YearCode)
        {
            var JSON = await IStockAdjust.NewEntryId(YearCode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillCurrentBatchINStore(int ItemCode, int YearCode, string StoreName, string batchno = "")
        {
            var FinStartDate = HttpContext.Session.GetString("FromDate");
            var TrDate = HttpContext.Session.GetString("ToDate");
            //var TrDate = ParseFormattedDate(TransDate);
            var JSON = await IStockAdjust.FillCurrentBatchINStore(ItemCode, YearCode, FinStartDate, TrDate, StoreName, batchno);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillCurrentBatchINWIP(int ItemCode, int YearCode, int WCid, string TransDate, string batchno = "")
        {
            var JSON = await IStockAdjust.FillCurrentBatchINWIP(ItemCode, YearCode, WCid, batchno, TransDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GETWIPotalSTOCK(int ItemCode, int WCID)
        {
            var JSON = await IStockAdjust.GETWIPotalSTOCK(ItemCode, WCID);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetWIPStockBatchWise(int ItemCode, int WCID, string uniquebatchno, string batchno)
        {
            var JSON = await IStockAdjust.GetWIPStockBatchWise(ItemCode, WCID, uniquebatchno, batchno);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }

        //public ActionResult ImportStock()
        //{
        //    StockAdjustmentModel model = new StockAdjustmentModel();
        //    IMemoryCache.Remove("KeyStockMultiBatchAdjustGrid");
        //    model.YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
        //    return View(model);
        //}

        [HttpPost]
        public IActionResult UploadExcel(IFormFile excelFile)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            List<StockAdjustmentModel> data = new List<StockAdjustmentModel>();

            using (var stream = excelFile.OpenReadStream())
            using (var package = new ExcelPackage(stream))
            {
                var worksheet = package.Workbook.Worksheets[0];
                for (int row = 2; row <= worksheet.Dimension.Rows; row++)
                {

                    //var itemCatCode = IStockAdjust.GetItemCatCode(worksheet.Cells[row, 6].Value.ToString());
                    var itemCode = IStockAdjust.GetItemCode(worksheet.Cells[row, 2].Value.ToString());
                    var storeIdResult = 0;
                    var WCResult = 0;


                    //var duplicatePartCode = IStockAdjust.isDuplicate(worksheet.Cells[row, 1].Value.ToString(), "PartCode", "Item_Master");
                    //var duplicateItemName = IStockAdjust.isDuplicate(worksheet.Cells[row, 2].Value.ToString(), "Item_Name", "Item_Master");

                    //var PartCodeExists = Convert.ToInt32(duplicatePartCode.Result) > 0 ? "Y" : "N";
                    //var ItemNameExists = Convert.ToInt32(duplicateItemName.Result) > 0 ? "Y" : "N";

                    //var dupeItemNameFeatureOpt = _IItemMaster.GetFeatureOption();

                    //  ItemNameExists = dupeItemNameFeatureOpt.DuplicateItemName ? "N" : ItemNameExists;

                    int itemCCode = 0;
                    string itemName = "";

                    if (itemCode.Result.Result != null)
                    {
                        itemCCode = itemCode.Result.Result.Rows.Count <= 0 ? 0 : (int)itemCode.Result.Result.Rows[0].ItemArray[0];
                        itemName = itemCode.Result.Result.Rows.Count <= 0 ? "" : itemCode.Result.Result.Rows[0].ItemArray[1];
                    }
                    var StockAdjustmentDate = IStockAdjust.GetmaxStockAdjustDate("GetEachItemsSADate", itemCCode);

                    var StockDateResult = StockAdjustmentDate.Result.Result != null && StockAdjustmentDate.Result.Result.Rows.Count > 0 ? StockAdjustmentDate.Result.Result.Rows[0].ItemArray[0] : "";


                    var GetStoreTotalStock = 0;
                    var WorkCenterTotalStock = 0;
                    var StoreLotStockResult = 0;
                    var WCLotStockResult = 0;

                    var batchno = worksheet.Cells[row, 8].Value.ToString();
                    var uniquebatchno = worksheet.Cells[row, 9].Value.ToString();

                    if (worksheet.Cells[row, 1].Value.ToString() == "S")
                    {
                        var storeId = IStockAdjust.GetStoreId(worksheet.Cells[row, 3].Value.ToString());
                        storeIdResult = storeId.Result.Result != null && storeId.Result.Result.Rows.Count > 0 ? (int)storeId.Result.Result.Rows[0].ItemArray[0] : 0;
                        var StoreTotalStock = IStockAdjust.FillTotalStock(itemCCode, storeIdResult);
                        GetStoreTotalStock = StoreTotalStock.Result.Result != null && StoreTotalStock.Result.Result.Rows.Count > 0 ? (int)StoreTotalStock.Result.Result.Rows[0].ItemArray[0] : 0;
                        var StoreLotStock = IStockAdjust.FillLotStock(itemCCode, storeIdResult, uniquebatchno, batchno);
                        StoreLotStockResult = StoreLotStock.Result.Result != null && StoreLotStock.Result.Result.Rows.Count > 0 ? (int)StoreLotStock.Result.Result.Rows[0].ItemArray[0] : 0;
                    }
                    else
                    {
                        var WCId = IStockAdjust.GetWorkCenterId(worksheet.Cells[row, 4].Value.ToString());
                        WCResult = WCId.Result.Result != null && WCId.Result.Result.Rows.Count > 0 ? (int)WCId.Result.Result.Rows[0].ItemArray[0] : 0;
                        var WCTotalStock = IStockAdjust.GETWIPotalSTOCK(itemCCode, WCResult);
                        WorkCenterTotalStock = WCTotalStock.Result.Result != null && WCTotalStock.Result.Result.Rows.Count > 0 ? (int)WCTotalStock.Result.Result.Rows[0].ItemArray[0] : 0;
                        var WIPLotStock = IStockAdjust.GetWIPStockBatchWise(itemCCode, WCResult, uniquebatchno, batchno);
                        WCLotStockResult = WIPLotStock.Result.Result != null && WIPLotStock.Result.Result.Rows.Count > 0 ? (int)WIPLotStock.Result.Result.Rows[0].ItemArray[0] : 0;
                    }
                    var YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
                    var AltQty = IStockAdjust.GetAltUnitQty(itemCCode, 0, worksheet.Cells[row, 1].Value.ToString() == "S" ? GetStoreTotalStock : WorkCenterTotalStock);
                    var AltQtyResult = AltQty.Result.Result != null && AltQty.Result.Result.Rows.Count > 0 ? (int)AltQty.Result.Result.Rows[0].ItemArray[0] : 0;
                    var Rate = IStockAdjust.FillRateAmount(itemCCode, YearCode, uniquebatchno, batchno);
                    var ActualRate = Rate.Result.Result != null && Rate.Result.Result.Rows.Count > 0 ? (int)Rate.Result.Result.Rows[0].ItemArray[0] : 0;

                    var ActualStock = Convert.ToSingle(worksheet.Cells[row, 5].Value.ToString());
                    var AdjQty = ActualStock - (worksheet.Cells[row, 1].Value.ToString() == "S" ? StoreLotStockResult : WCLotStockResult);
                    var AdjType = AdjQty > 0 ? "+" : "-";
                    var Amount = (worksheet.Cells[row, 1].Value.ToString() == "S" ? GetStoreTotalStock : WorkCenterTotalStock) * ActualRate;

                    //data.Add(new StockAdjustmentModel()
                    //{
                    //    StoreWorkCenter = worksheet.Cells[row, 1].Value.ToString(),
                    //    PartCode = worksheet.Cells[row, 2].Value.ToString(),
                    //    ItemName = itemName,
                    //    StoreName = worksheet.Cells[row, 3].Value == null ? string.Empty : worksheet.Cells[row, 3].Value.ToString(),
                    //    WCName = worksheet.Cells[row, 4].Value == null ? string.Empty : worksheet.Cells[row, 4].Value.ToString(),
                    //    ActualStockQty = Convert.ToSingle(worksheet.Cells[row, 5].Value.ToString()),
                    //    Unit = worksheet.Cells[row, 6].Value.ToString(),
                    //    altUnit = worksheet.Cells[row, 7].Value.ToString() == null ? string.Empty : worksheet.Cells[row, 7].Value.ToString(),
                    //    batchno = worksheet.Cells[row, 8].Value.ToString(),
                    //    uniqbatchno = worksheet.Cells[row, 9].Value.ToString(),
                    //    reasonOfAdjustment = worksheet.Cells[row, 10].Value.ToString() == null ? string.Empty : worksheet.Cells[row, 10].Value.ToString(),
                    //    TotalStock = worksheet.Cells[row, 1].Value.ToString() == "S" ? GetStoreTotalStock : WorkCenterTotalStock,
                    //    LotStock = worksheet.Cells[row, 1].Value.ToString() == "S" ? StoreLotStockResult : WCLotStockResult,
                    //    AltQty = AltQtyResult,
                    //    AdjQty = Math.Abs(AdjQty),
                    //    AdjType = AdjType,
                    //    Rate = ActualRate,
                    //    Amount = Amount,
                    //    ItemCode = itemCCode,
                    //    Wcid = WCResult,
                    //    Storeid = storeIdResult,
                    //    StockAdjustmentDate ="01/feb/2025"
                    //    //StockDateResult.ToString()
                    //});

                    data.Add(new StockAdjustmentModel()
                    {
                        StoreWorkCenter = worksheet.Cells[row, 1].Value.ToString(),
                        PartCode = worksheet.Cells[row, 2].Value.ToString(),
                        ItemName = itemName,
                        StoreName = worksheet.Cells[row, 3].Value?.ToString() ?? string.Empty,
                        WCName = worksheet.Cells[row, 4].Value?.ToString() ?? string.Empty,
                        ActualStockQty = Convert.ToSingle(worksheet.Cells[row, 5].Value.ToString()),
                        Unit = worksheet.Cells[row, 6].Value.ToString(),
                        altUnit = worksheet.Cells[row, 7].Value?.ToString() ?? string.Empty,
                        batchno = worksheet.Cells[row, 8].Value.ToString(),
                        uniqbatchno = worksheet.Cells[row, 9].Value.ToString(),
                        reasonOfAdjustment = worksheet.Cells[row, 10].Value?.ToString() ?? string.Empty,
                        TotalStock = worksheet.Cells[row, 1].Value.ToString() == "S" ? GetStoreTotalStock : WorkCenterTotalStock,
                        LotStock = worksheet.Cells[row, 1].Value.ToString() == "S" ? StoreLotStockResult : WCLotStockResult,
                        AltQty = AltQtyResult,
                        AdjQty = Math.Abs(AdjQty),
                        AdjType = AdjType,
                        Rate = ActualRate,
                        Amount = Amount,
                        ItemCode = itemCCode,
                        Wcid = WCResult,
                        Storeid = storeIdResult,
                        StockAdjustmentDate = "01/feb/2025"
                        //StockDateResult.ToString()
                    });
                }
            }
            MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                SlidingExpiration = TimeSpan.FromMinutes(55),
                Size = 1024,
            };
            var model = new StockAdjustmentModel();
            model.ImportMode = "Y";
            model.ExcelDetailGrid = data;
            var SAGrid = new DataTable();
            SAGrid = GetExcelDetailTable(model.ExcelDetailGrid);

            List<StockAdjustmentDetail> list = new List<StockAdjustmentDetail>();
            int cnt = 1;
            foreach (DataRow row in SAGrid.Rows)
            {
                var GetItems = IStockAdjust.GetItems(Convert.ToInt32(row["ItemCode"]));
                var GetWorkCenmterName = IStockAdjust.GetWCName(Convert.ToInt32(row["wcid"]));
                var GetStoreName = IStockAdjust.GetStoreName(Convert.ToInt32(row["storeid"]));

                StockAdjustmentDetail stockDetail = new StockAdjustmentDetail
                {

                    SeqNo = cnt++,
                    ItemCode = Convert.ToInt32(row["ItemCode"]),
                    Unit = row["Unit"].ToString(),
                    LotStock = Convert.ToInt32(row["LotStock"]),
                    TotalStock = Convert.ToInt32(row["TotalStock"]),
                    altUnit = row["altUnit"].ToString(),
                    AltQty = Convert.ToInt32(row["AltQty"]),
                    ActualStockQty = Convert.ToInt32(row["ActuleStockQty"]),
                    AdjQty = Convert.ToInt32(row["AdjQty"]),
                    AdjType = row["AdjType"].ToString(),
                    Storeid = Convert.ToInt32(row["storeid"]),
                    Wcid = Convert.ToInt32(row["wcid"].ToString()),
                    Rate = Convert.ToInt32(row["rate"]),
                    Amount = Convert.ToInt32(row["Amount"].ToString()),
                    batchno = row["batchno"].ToString(),
                    uniqbatchno = row["uniquebatchno"].ToString(),
                    reasonOfAdjustment = row["reasonOfAdjustment"].ToString(),
                    PartCode = GetItems.Result.Result.Rows.Count <= 0 ? "" : GetItems.Result.Result.Rows[0].ItemArray[1],
                    ItemName = GetItems.Result.Result.Rows.Count <= 0 ? "" : GetItems.Result.Result.Rows[0].ItemArray[0],
                    WCName = GetWorkCenmterName.Result.Result.Rows.Count <= 0 ? "" : GetWorkCenmterName.Result.Result.Rows[0].ItemArray[0],
                    StoreName = GetStoreName.Result.Result.Rows.Count <= 0 ? "" : GetStoreName.Result.Result.Rows[0].ItemArray[0]
                };
                list.Add(stockDetail);
            }


            IMemoryCache.Set("KeyStockAdjustGrid", list, cacheEntryOptions);

            return PartialView("_StockAdjustGrid", model);
        }

        public async Task<JsonResult> GetDashItemName(string FromDate, string ToDate)
        {
            var JSON = await IStockAdjust.GetDashItemName( FromDate,  ToDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }

        public async Task<JsonResult> GetDashPartCode(string FromDate, string ToDate)
        {
            var JSON = await IStockAdjust.GetDashPartCode(FromDate, ToDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }

        public async Task<JsonResult> GetDashStoreName(string FromDate, string ToDate)
        {
            var JSON = await IStockAdjust.GetDashStoreName(FromDate, ToDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }

        public async Task<JsonResult> GetDashWorkCenter(string FromDate, string ToDate)
        {
            var JSON = await IStockAdjust.GetDashWorkCenter(FromDate, ToDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }

    }
}