using eTactWeb.Data.Common;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using static eTactWeb.Data.Common.CommonFunc;
using static eTactWeb.DOM.Models.Common;
using eTactWeb.DOM.Models;
using System.Net;
using System.Data;
using System.Globalization;
using FastReport.Web;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System.Configuration;


namespace eTactWeb.Controllers
{
    public class InterStoreTransferController : Controller
    {
        public WebReport webReport;
        public IDataLogic IDataLogic { get; }
        public IMemoryCache IMemoryCache { get; }
        public IInterStoreTransfer IInterStore { get; }
        public IWebHostEnvironment IWebHostEnvironment { get; }
        public ILogger<InterStoreTransferController> Logger { get; }
        private EncryptDecrypt EncryptDecrypt { get; }
        private readonly IConfiguration iconfiguration;
        public InterStoreTransferController(IInterStoreTransfer iInterStore, IConfiguration configuration, IDataLogic iDataLogic, IMemoryCache iMemoryCache, ILogger<InterStoreTransferController> logger, EncryptDecrypt encryptDecrypt, IWebHostEnvironment iWebHostEnvironment)
        {
            IInterStore = iInterStore;
            IDataLogic = iDataLogic;
            IMemoryCache = iMemoryCache;
            Logger = logger;
            EncryptDecrypt = encryptDecrypt;
            IWebHostEnvironment = iWebHostEnvironment;
            iconfiguration = configuration;
        }

        [HttpGet]
        [Route("{controller}/Index")]
        public async Task<IActionResult> InterStoreTransfer(int ID, string Mode, int YC, string FromDate = "", string ToDate = "", string SlipNo = "", string BatchNo = "", string PartCode = "", string ItemName = "", string Searchbox = "", string SummaryDetail = "")
        {
            //RoutingModel model = new RoutingModel();  
            ViewData["Title"] = "InterStoreTransfer Details";
            TempData.Clear();
            IMemoryCache.Remove("KeyInterStoreTransferGrid");
            var MainModel = new InterStoreTransferModel();

            MainModel.FinFromDate = ParseDate(HttpContext.Session.GetString("FromDate")).ToString().Replace("-", "/");
            MainModel.FinToDate = ParseDate(HttpContext.Session.GetString("ToDate")).ToString().Replace("-", "/");
            MainModel.CC = HttpContext.Session.GetString("Branch");
            MainModel.YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));

            if (!string.IsNullOrEmpty(Mode) && ID > 0 && (Mode == "V" || Mode == "U"))
            {
                MainModel = await IInterStore.GetViewByID(ID, Mode, YC);
                MainModel.Mode = Mode;
                MainModel.ID = ID;
                //  MainModel = await BindModel(MainModel);

            }
            else
            {
                // MainModel = await BindModel(MainModel);
            }
            if (Mode != "U")
            {
                MainModel.Uid = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                MainModel.ActualEntryBy = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
                MainModel.ActualEntryByName = HttpContext.Session.GetString("EmpName");
                MainModel.ActualEntryDate = DateTime.Now.ToString();
            }
            else
            {
                MainModel.Uid = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                MainModel.UpdatedBy = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
                MainModel.LastUpdatedByName = HttpContext.Session.GetString("EmpName");
                MainModel.LastUpdationDate = DateTime.Now.ToString();
            }
            MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                SlidingExpiration = TimeSpan.FromMinutes(55),
                Size = 1024,
            };

            IMemoryCache.Set("KeyInterStoreTransferGrid", MainModel.InterStoreDetails, cacheEntryOptions);
            //IMemoryCache.Set("KeyInterStoreTransferGrid", model, cacheEntryOptions);
            MainModel.FromDateBack = FromDate;
            MainModel.ToDateBack = ToDate;
            MainModel.SlipNoBack = SlipNo;
            MainModel.BatchNoback = BatchNo;
            MainModel.PartCodeBack = PartCode;
            MainModel.ItemNameBack = ItemName;
            MainModel.DashboardTypeBack = SummaryDetail;
            MainModel.GlobalSearchBack = Searchbox;
            return View(MainModel);
        }
        public IActionResult PrintReport(int EntryId , int YearCode , string PONO = "")
        {
          



            string my_connection_string;
            string contentRootPath = IWebHostEnvironment.ContentRootPath;
            string webRootPath = IWebHostEnvironment.WebRootPath;
            webReport = new WebReport();
            
            ViewBag.EntryId = EntryId;
            ViewBag.YearCode = YearCode;
            ViewBag.PONO = PONO;
            webReport.Report.Load(webRootPath + "\\InterStoreTRansfer.frx"); // default report
            my_connection_string = iconfiguration.GetConnectionString("eTactDB");
            webReport.Report.Dictionary.Connections[0].ConnectionString = my_connection_string;
            webReport.Report.Dictionary.Connections[0].ConnectionStringExpression = "";
            webReport.Report.SetParameterValue("entryparam", EntryId);
            webReport.Report.SetParameterValue("yearparam", YearCode);
            webReport.Report.SetParameterValue("MyParameter", my_connection_string);
            webReport.Report.Refresh();
            return View(webReport);


            return View(webReport);
        }
        public async Task<JsonResult> GetFormRights()
        {
            var userID = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
            var JSON = await IInterStore.GetFormRights(userID);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public IActionResult DeleteItemRow(int SeqNo, string Mode)
        {
            var MainModel = new InterStoreTransferModel();
            if (Mode == "U")
            {
                int Indx = Convert.ToInt32(SeqNo) - 1;
                IMemoryCache.TryGetValue("KeyInterStoreTransferGrid", out List<InterStoreTransferDetail> ISTDetail);

                if (ISTDetail != null && ISTDetail.Count > 0)
                {
                    ISTDetail.RemoveAt(Convert.ToInt32(Indx));

                    Indx = 0;

                    foreach (var item in ISTDetail)
                    {
                        Indx++;
                        //item.SeqNo = Indx;
                    }
                    MainModel.InterStoreDetails = ISTDetail;

                    MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
                    {
                        AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                        SlidingExpiration = TimeSpan.FromMinutes(55),
                        Size = 1024,
                    };

                    IMemoryCache.Set("KeyInterStoreTransferGrid", MainModel.InterStoreDetails, cacheEntryOptions);
                }
            }
            else
            {
                IMemoryCache.TryGetValue("KeyInterStoreTransferGrid", out List<InterStoreTransferDetail> ISTDetail);
                int Indx = Convert.ToInt32(SeqNo) - 1;

                if (ISTDetail != null && ISTDetail.Count > 0)
                {
                    ISTDetail.RemoveAt(Convert.ToInt32(Indx));

                    Indx = 0;

                    foreach (var item in ISTDetail)
                    {
                        Indx++;
                        //item.SeqNo = Indx;
                    }
                    MainModel.InterStoreDetails = ISTDetail;

                    MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
                    {
                        AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                        SlidingExpiration = TimeSpan.FromMinutes(55),
                        Size = 1024,
                    };

                    IMemoryCache.Set("KeyInterStoreTransferGrid", MainModel.InterStoreDetails, cacheEntryOptions);
                }
            }

            return PartialView("_InterStoreTransferDetail", MainModel);
        }
        public IActionResult AddInterStoreTransferDetail(InterStoreTransferDetail model)
        {
            try
            {
                IMemoryCache.TryGetValue("KeyInterStoreTransferGrid", out IList<InterStoreTransferDetail> ISTDetail);

                var MainModel = new InterStoreTransferModel();
                var ISTGrid = new List<InterStoreTransferDetail>();
                var ISTList = new List<InterStoreTransferDetail>();

                if (model != null)
                {
                    if (ISTDetail == null)
                    {
                       // model.SeqNo = 1;
                        ISTGrid.Add(model);
                    }
                    else
                    {
                        if (ISTDetail.Any(x => x.ItemCode == model.ItemCode && x.BatchNo == model.BatchNo))
                        {
                            return StatusCode(207, "Duplicate");
                        }
                        else
                        {
                            //if (ISTDetail.Any(x => x.ItemCode == model.ItemCode && x.Wcid == model.Wcid && x.batchno == model.batchno /*&& x.uniqbatchno == model.uniqbatchno*/))
                            //{
                            //    return StatusCode(207, "Duplicate");
                            //}

                           // model.SeqNo = ISTDetail.Count + 1;
                            ISTGrid = ISTDetail.Where(x => x != null).ToList();
                            ISTList.AddRange(ISTGrid);
                            ISTGrid.Add(model);
                        }
                    }

                    ISTGrid = ISTGrid.OrderBy(item => item.SeqNo).ToList();
                    MainModel.InterStoreDetails = ISTGrid;
                    
                    MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
                    {
                        AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                        SlidingExpiration = TimeSpan.FromMinutes(55),
                        Size = 1024,
                    };

                    IMemoryCache.Set("KeyInterStoreTransferGrid", MainModel.InterStoreDetails, cacheEntryOptions);
                }
                else
                {
                    ModelState.TryAddModelError("Error", "Schedule List Cannot Be Empty...!");
                }
                return PartialView("_InterStoreTransferDetail", MainModel);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("{controller}/Index")]
        public async Task<IActionResult> InterStoreTransfer(InterStoreTransferModel model)
        {
            try
            {
                var ISTGrid = new DataTable();

                IMemoryCache.TryGetValue("KeyInterStoreTransferGrid", out IList<InterStoreTransferDetail> ISTDetail);

                if (ISTDetail == null)
                {
                    ModelState.Clear();
                    ModelState.TryAddModelError("InterStoreTransferDetail", "InterStoreTransfer Grid Should Have Atleast 1 Item...!");
                    return View("InterStoreTransfer", model);
                }
                else
                {
                    model.CC = HttpContext.Session.GetString("Branch");
                    model.Uid = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                    model.ActualEntryBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                    model.ActualEntryByName = HttpContext.Session.GetString("EmpName");
                    if (model.Mode == "U")
                    {
                        model.LastUpdatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                        model.LastUpdatedByName = HttpContext.Session.GetString("EmpName");
                    }

                    ISTGrid = GetDetailTable(ISTDetail);
                    var Result = await IInterStore.SaveInterStore(model, ISTGrid);

                    if (Result != null)
                    {
                        if (Result.StatusText == "Success" && Result.StatusCode == HttpStatusCode.OK)
                        {
                            ViewBag.isSuccess = true;
                            TempData["200"] = "200";
                            IMemoryCache.Remove(ISTGrid);
                            var model1 = new InterStoreTransferModel();
                            model1.FinFromDate = HttpContext.Session.GetString("FromDate");
                            model1.FinToDate = HttpContext.Session.GetString("ToDate");
                            model1.YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
                            model1.CC = HttpContext.Session.GetString("Branch");
                            //model1.ActualEnteredByName = HttpContext.Session.GetString("EmpName");
                            model1.CreatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                            IMemoryCache.Remove("KeyInterStoreTransferGrid");
                            //return View(model1);
                            return RedirectToAction(nameof(ISTDashboard));
                        }
                        if (Result.StatusText == "Updated" && Result.StatusCode == HttpStatusCode.Accepted)
                        {
                            ViewBag.isSuccess = true;
                            TempData["202"] = "202";
                            var model1 = new InterStoreTransferModel();
                            model1.FinFromDate = HttpContext.Session.GetString("FromDate");
                            model1.FinToDate = HttpContext.Session.GetString("ToDate");
                            model1.YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
                            model1.CC = HttpContext.Session.GetString("Branch");
                            //model1.ActualEnteredByName = HttpContext.Session.GetString("EmpName");
                            model1.CreatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                            IMemoryCache.Remove("KeyInterStoreTransferGrid");
                            //return View(model1);
                            return RedirectToAction(nameof(ISTDashboard));
                        }
                        if (Result.StatusText == "Error" && Result.StatusCode == HttpStatusCode.InternalServerError)
                        {
                            var errNum = Result.Result.Message.ToString().Split(":")[1];
                            if (errNum == " 2627")
                            {
                                ViewBag.isSuccess = false;
                                TempData["2627"] = "2627";
                                Logger.LogError("\n \n ********** LogError ********** \n " + JsonConvert.SerializeObject(Result) + "\n \n");
                                var model2 = new InterStoreTransferModel();
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
                            return RedirectToAction(nameof(ISTDashboard));
                        }
                    }
                    return RedirectToAction(nameof(ISTDashboard));
                }
            }
            catch (Exception ex)
            {
                LogException<InterStoreTransferController>.WriteException(Logger, ex);


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

        [Route("{controller}/Dashboard")]
        public async Task<IActionResult> ISTDashboard(string SlipNo,string PartCode,string ItemName,string BatchNo,string SummaryDetail, string Flag = "True", string FromDate = "", string ToDate = "")
        {
            IMemoryCache.Remove("KeyInterStoreTransferGrid");
            var model = new ISTDashboard();
            var yearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
            DateTime now = DateTime.Now;
            DateTime firstDayOfMonth = new DateTime(yearCode, now.Month, 1);
            model.FromDate = new DateTime(yearCode, now.Month, 1).ToString("dd/MM/yyyy").Replace("-", "/");
            model.ToDate = new DateTime(yearCode + 1, 3, 31).ToString("dd/MM/yyyy").Replace("-", "/");

            var Result = await IInterStore.GetDashboardData(model);

            if (Result.Result != null)
            {
                var _List = new List<TextValue>();
                DataSet DS = Result.Result;

                var DT = DS.Tables[0].DefaultView.ToTable(true, "EntryId", "Yearcode","EntryDate",
                    "SlipNo", "SlipDate", "IssueToStoreWC", "Remark", "ActualEntryDate", "ItemCode", "Partcode", "ItemName", "LastUpdatetionDate", "FromStoreName",
                    "ToStorename", "TOWCName", "ActualEntryByName", "LastUpdatedByName", "TransferReason", "CC", "MAchineName", "TotalStockQty", "LotStockQty",
                    "Qty", "Unit", "AltQty", "Rate", "Batchno", "Uniquebatchno", "ReasonOfTransfer", "RecStoreStock", "AltUnit", "ToStoreId", "ToWCID", "ActualEntryBy", "LastUpdatedBy");

                model.ISTDashboardGrid = CommonFunc.DataTableToList<InterStoreDashboard>(DT, "ISTDashboard");
                model.ISTDashboardGrid = model.ISTDashboardGrid
                    .GroupBy(d => d.EntryId)
                    .Select(g => g.First())
                    .ToList();

                if (Flag != "True")
                {
                    model.SlipNo = SlipNo;
                    model.PartCode = PartCode;
                    model.ItemName = ItemName;
                    model.Batchno = BatchNo;
                    model.FromDate = FromDate;
                    model.ToDate = ToDate;
                    model.SummaryDetail = SummaryDetail;
                }
            }
            model.SummaryDetail = "Summary";
            return View(model);
        }

        public async Task<IActionResult> DeleteByID(int ID,int YC, string EntryDate,int ActualEntryBy, string MachineName, string SummaryDetail,string FromDate = "",string ToDate = "",string SlipNo = "",string PartCode = "",string ItemName = "",string BatchNo = "")
        {
            var Result = await IInterStore.DeleteByID(ID, YC,EntryDate,ActualEntryBy,MachineName).ConfigureAwait(false);

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
            return RedirectToAction("Dashboard", new {Flag = "false",FromDate = FromDate,ToDate = ToDate,SlipNo = SlipNo,PartCode= PartCode,ItemName = ItemName,BatchNo = BatchNo,SummaryDetail = SummaryDetail});
        }

        public async Task<IActionResult> GetSearchData(ISTDashboard model)
        {
            model.FromDate = ParseFormattedDate(model.FromDate);    
            model.ToDate = ParseFormattedDate(model.ToDate);
            var Result = await IInterStore.GetDashboardData(model);
            if (Result.Result !=null)
            {
                DataSet DS = Result.Result;

                var DT = DS.Tables[0].DefaultView.ToTable(true, "EntryId", "Yearcode","EntryDate",
                        "SlipNo", "SlipDate", "IssueToStoreWC", "Remark", "ActualEntryDate", "ItemCode", "Partcode", "ItemName", "LastUpdatetionDate", "FromStoreName",
                        "ToStorename", "TOWCName", "ActualEntryByName", "LastUpdatedByName", "TransferReason", "CC", "MAchineName", "TotalStockQty", "LotStockQty",
                        "Qty", "Unit", "AltQty", "Rate", "Batchno", "Uniquebatchno", "ReasonOfTransfer", "RecStoreStock", "AltUnit", "ToStoreId", "ToWCID", "ActualEntryBy", "LastUpdatedBy");

                model.ISTDashboardGrid = CommonFunc.DataTableToList<InterStoreDashboard>(DT, "ISTDashboard");
                if (model.SummaryDetail == "Summary")
                {
                    model.ISTDashboardGrid = model.ISTDashboardGrid
                        .GroupBy(d => d.EntryId)
                        .Select(g => g.First())
                        .ToList();
                }
            }
            return PartialView("_ISTDashboardGrid", model);
        }

        private static DataTable GetDetailTable(IList<InterStoreTransferDetail> DetailList)
        {
            var DTSSGrid = new DataTable();

            DTSSGrid.Columns.Add("Entryid", typeof(int));
            DTSSGrid.Columns.Add("Yearcode", typeof(int));
            DTSSGrid.Columns.Add("ItemCode", typeof(int));
            DTSSGrid.Columns.Add("SeqNo", typeof(int));
            DTSSGrid.Columns.Add("TotalStockQty", typeof(float));
            DTSSGrid.Columns.Add("LotStockQty", typeof(float));
            DTSSGrid.Columns.Add("Qty", typeof(float));
            DTSSGrid.Columns.Add("Unit", typeof(string));
            DTSSGrid.Columns.Add("AltQty", typeof(float));
            DTSSGrid.Columns.Add("AltUnit", typeof(string));
            DTSSGrid.Columns.Add("Rate", typeof(float));
            DTSSGrid.Columns.Add("Batchno", typeof(string));
            DTSSGrid.Columns.Add("Uniquebatchno", typeof(string));
            DTSSGrid.Columns.Add("ReasonOfransfer", typeof(string));
            DTSSGrid.Columns.Add("RecStoreStock", typeof(float));
            //DateTime DeliveryDt = new DateTime();
            foreach (var Item in DetailList)
            {
                string uniqueString = Guid.NewGuid().ToString();
                DTSSGrid.Rows.Add(
                    new object[]
                    {
                   0,
                   0,
                    Item.ItemCode,
                    Item.SeqNo,
                    Item.TotalStockQty,
                    Item.LotStockQty,
                    Item.Qty,
                    Item.Unit ?? "",
                    Item.AltQty,
                    Item.AltUnit ?? "",
                    Item.Rate,
                    Item.BatchNo ?? "",
                    Item.UniqueBatchNo ?? "",
                    Item.ReasonOfTransfer ?? "",
                    Item.RecStoreStock
                    });
            }
            DTSSGrid.Dispose();
            return DTSSGrid;
        }
        public async Task<JsonResult> EditItemRows(int SeqNo)
        {
            var MainModel = new InterStoreTransferModel();
            IMemoryCache.TryGetValue("KeyInterStoreTransferGrid", out List<InterStoreTransferDetail> InterStoreGrid);
            var ISTGrid = InterStoreGrid.Where(x => x.SeqNo == SeqNo);
            string JsonString = JsonConvert.SerializeObject(ISTGrid);
            return Json(JsonString);
        }
        public IActionResult ClearGrid()
        {
            IMemoryCache.Remove("KeyInterStoreTransferGrid");
            var MainModel = new InterStoreTransferModel();
            return PartialView("_InterStoreTransferDetail", MainModel);
        }
        public static DateTime ParseDate(string dateString)
        {
            if (string.IsNullOrEmpty(dateString))
            {
                return default;
            }

            if (DateTime.TryParseExact(dateString, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate))
            {
                return parsedDate;
            }
            else
            {
                return DateTime.Parse(dateString);
            }

            //    throw new FormatException("Invalid date format. Expected format: dd/MM/yyyy");

        }
        public async Task<JsonResult> FillStore()
        {
            var JSON = await IInterStore.FillStore();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> CheckIssuedTransStock(int ItemCode,int YearCode,int EntryId,string TransDate,string TransNo,int Storeid,string batchno,string uniquebatchno,string Flag)
        {
            var JSON = await IInterStore.CheckIssuedTransStock(ItemCode,YearCode,EntryId,TransDate,TransNo,Storeid,batchno,uniquebatchno,Flag);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetPrevQty(int EntryId,int YearCode,int ItemCode,string uniqueBatchno)
        {
            var JSON = await IInterStore.GetPrevQty(EntryId,YearCode,ItemCode,uniqueBatchno);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillLoadToStoreName()
        {
            var JSON = await IInterStore.FillLoadToStoreName();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetAllowBackDate()
        {
            var JSON = await IInterStore.GetAllowBackDate();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillLoadTOWorkcenter()
        {
            var JSON = await IInterStore.FillLoadTOWorkcenter();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> NewEntryId(int yearCode)
        {
            var JSON = await IInterStore.NewEntryId(yearCode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillPartCode(string ShowAllItems)
        {
            var JSON = await IInterStore.FillPartCode(ShowAllItems);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillItem(string ShowAllItems)
        {
            var JSON = await IInterStore.FillItems(ShowAllItems);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillStockBatchNo(int ItemCode, string StoreName, int YearCode, string batchno)
        {
            var FinStartDate = HttpContext.Session.GetString("FromDate");
            var JSON = await IInterStore.FillStockBatchNo(ItemCode, StoreName, YearCode, batchno,FinStartDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetUnitAltUnit(int ItemCode)
        {
            var JSON = await IInterStore.GetUnitAltUnit(ItemCode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
    }
}
