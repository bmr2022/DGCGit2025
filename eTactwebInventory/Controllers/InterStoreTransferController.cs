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
using System.Runtime.Caching;


namespace eTactWeb.Controllers
{
    public class InterStoreTransferController : Controller
    {
        public WebReport webReport;
        public IDataLogic IDataLogic { get; }
        public IInterStoreTransfer IInterStore { get; }
        public IWebHostEnvironment IWebHostEnvironment { get; }
        public ILogger<InterStoreTransferController> Logger { get; }
        private readonly IMemoryCache _memoryCache;
        private EncryptDecrypt EncryptDecrypt { get; }
        private readonly IConfiguration iconfiguration;
        private readonly ConnectionStringService _connectionStringService;
        public InterStoreTransferController(IInterStoreTransfer iInterStore, IConfiguration configuration, IDataLogic iDataLogic, ILogger<InterStoreTransferController> logger, EncryptDecrypt encryptDecrypt, IWebHostEnvironment iWebHostEnvironment, IMemoryCache memoryCache, ConnectionStringService connectionStringService)
        {
            IInterStore = iInterStore;
            IDataLogic = iDataLogic;
            Logger = logger;
            EncryptDecrypt = encryptDecrypt;
            IWebHostEnvironment = iWebHostEnvironment;
            iconfiguration = configuration;
            _memoryCache = memoryCache;
            _connectionStringService = connectionStringService;
        }

        [HttpGet]
        [Route("{controller}/Index")]
        public async Task<IActionResult> InterStoreTransfer(string formKey, int ID, string Mode, int YC, string FromDate = "", string ToDate = "", string SlipNo = "", string BatchNo = "", string PartCode = "", string ItemName = "", string Searchbox = "", string SummaryDetail = "")
        {

            ViewBag.formKey = formKey;
            var uniqueKey = Guid.NewGuid().ToString();
            ViewBag.uniqueKey = uniqueKey;
            //RoutingModel model = new RoutingModel();  
            ViewData["Title"] = "InterStoreTransfer Details";
            //TempData.Clear();
            HttpContext.Session.Remove($"KeyInterStoreTransferGrid_{uniqueKey}");
            var MainModel = new InterStoreTransferModel();
            MainModel.FinFromDate = CommonFunc.ParseFormattedDate(HttpContext.Session.GetString($"FromDate_{formKey}"));
            MainModel.FinToDate = CommonFunc.ParseFormattedDate(HttpContext.Session.GetString($"ToDate_{formKey}"));
            MainModel.CC = HttpContext.Session.GetString($"Branch_{formKey}");
            MainModel.YearCode = Convert.ToInt32(HttpContext.Session.GetString($"YearCode_{formKey}"));

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
                MainModel.Uid = Convert.ToInt32(HttpContext.Session.GetString($"UID_{formKey}"));
                MainModel.ActualEntryBy = Convert.ToInt32(HttpContext.Session.GetString($"EmpID_{formKey}"));
                MainModel.ActualEntryByName = HttpContext.Session.GetString($"EmpName_{formKey}");
                MainModel.ActualEntryDate = DateTime.Now.ToString();
            }
            else
            {
                MainModel.Uid = Convert.ToInt32(HttpContext.Session.GetString($"UID_{formKey}"));
                MainModel.UpdatedBy = Convert.ToInt32(HttpContext.Session.GetString($"EmpID_{formKey}"));
                MainModel.LastUpdatedByName = HttpContext.Session.GetString($"EmpName_{formKey}");
                MainModel.LastUpdationDate = DateTime.Now.ToString();
                MainModel.FinFromDate = CommonFunc.ParseFormattedDate(HttpContext.Session.GetString($"FromDate_{formKey}"));
                MainModel.FinToDate = CommonFunc.ParseFormattedDate(HttpContext.Session.GetString($"ToDate_{formKey}"));
            }
            MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                SlidingExpiration = TimeSpan.FromMinutes(55),
                Size = 1024,
            };

            string serializedGrid = JsonConvert.SerializeObject(MainModel.InterStoreDetails);
            HttpContext.Session.SetString($"KeyInterStoreTransferGrid_{uniqueKey}", serializedGrid);

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
        public IActionResult PrintReport(int EntryId, int YearCode, string PONO = "")
        {
            string my_connection_string;
            string contentRootPath = IWebHostEnvironment.ContentRootPath;
            string webRootPath = IWebHostEnvironment.WebRootPath;
            webReport = new WebReport();

            ViewBag.EntryId = EntryId;
            ViewBag.YearCode = YearCode;
            ViewBag.PONO = PONO;
            webReport.Report.Load(webRootPath + "\\InterStoreTRansfer.frx"); // default report
            my_connection_string = _connectionStringService.GetConnectionString();
            //my_connection_string = iconfiguration.GetConnectionString("eTactDB");
            webReport.Report.Dictionary.Connections[0].ConnectionString = my_connection_string;
            webReport.Report.Dictionary.Connections[0].ConnectionStringExpression = "";
            webReport.Report.SetParameterValue("entryparam", EntryId);
            webReport.Report.SetParameterValue("yearparam", YearCode);
            webReport.Report.SetParameterValue("MyParameter", my_connection_string);
            webReport.Report.Refresh();
            return View(webReport);
        }
        public async Task<JsonResult> AutoFillPartCode(string showallitem, string SearchItemCode, string SearchPartCode)
        {
            var JSON = await IInterStore.AutoFillitem("AutoFillPartCode", showallitem, SearchItemCode, SearchPartCode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> AutoFillItemName(string showallitem, string SearchItemCode, string SearchPartCode)
        {
            var JSON = await IInterStore.AutoFillitem("AutoFillItemName", showallitem, SearchItemCode, SearchPartCode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetFormRights(string formKey)
        {
            var userID = Convert.ToInt32(HttpContext.Session.GetString($"EmpID_{formKey}"));
            var JSON = await IInterStore.GetFormRights(userID);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public IActionResult DeleteItemRow(int SeqNo, string Mode, string uniqueKey)
        {
            var MainModel = new InterStoreTransferModel();
            if (Mode == "U")
            {
                int Indx = Convert.ToInt32(SeqNo) - 1;

                string modelJson = HttpContext.Session.GetString($"KeyInterStoreTransferGrid_{uniqueKey}");
                List<InterStoreTransferDetail> ISTDetail = new List<InterStoreTransferDetail>();
                if (!string.IsNullOrEmpty(modelJson))
                {
                    ISTDetail = JsonConvert.DeserializeObject<List<InterStoreTransferDetail>>(modelJson);
                }

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

                    string serializedGrid = JsonConvert.SerializeObject(MainModel.InterStoreDetails);
                    HttpContext.Session.SetString($"KeyInterStoreTransferGrid_{uniqueKey}", serializedGrid);
                }
            }
            else
            {
                string modelJson = HttpContext.Session.GetString($"KeyInterStoreTransferGrid_{uniqueKey}");
                List<InterStoreTransferDetail> ISTDetail = new List<InterStoreTransferDetail>();
                if (!string.IsNullOrEmpty(modelJson))
                {
                    ISTDetail = JsonConvert.DeserializeObject<List<InterStoreTransferDetail>>(modelJson);
                }

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

                    string serializedGrid = JsonConvert.SerializeObject(MainModel.InterStoreDetails);
                    HttpContext.Session.SetString($"KeyInterStoreTransferGrid_{uniqueKey}", serializedGrid);
                }
            }
            return PartialView("_InterStoreTransferDetail", MainModel);
        }
        public IActionResult AddInterStoreTransferDetail(InterStoreTransferDetail model, string uniqueKey)
        {
            try
            {
                string modelJson = HttpContext.Session.GetString($"KeyInterStoreTransferGrid_{uniqueKey}");
                List<InterStoreTransferDetail> ISTDetail = new List<InterStoreTransferDetail>();
                if (!string.IsNullOrEmpty(modelJson))
                {
                    ISTDetail = JsonConvert.DeserializeObject<List<InterStoreTransferDetail>>(modelJson);
                }

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

                            ISTGrid = ISTDetail.Where(x => x != null).ToList();
                            ISTList.AddRange(ISTGrid);
                            ISTGrid.Add(model);
                        }
                    }

                    ISTGrid = ISTGrid.OrderBy(item => item.SeqNo).ToList();
                    MainModel.InterStoreDetails = ISTGrid;

                    string serializedGrid = JsonConvert.SerializeObject(MainModel.InterStoreDetails);
                    HttpContext.Session.SetString($"KeyInterStoreTransferGrid_{uniqueKey}", serializedGrid);
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



        public IActionResult AddMultipleItemDetail(List<InterStoreTransferDetail> model, string uniqueKey)
        {
            try
            {
                var MainModel = new InterStoreTransferModel();
                List<InterStoreTransferDetail> StockGrid;

                // ---------- Read session ONCE ----------
                string modelJson = HttpContext.Session.GetString($"KeyInterStoreTransferGrid_{uniqueKey}");

                if (!string.IsNullOrEmpty(modelJson))
                {
                    StockGrid = JsonConvert.DeserializeObject<List<InterStoreTransferDetail>>(modelJson);
                }
                else
                {
                    StockGrid = new List<InterStoreTransferDetail>();
                }

                if (model == null || model.Count == 0)
                {
                    ModelState.TryAddModelError("Error", " List Cannot Be Empty...!");
                    return PartialView("_InterStoreTransferDetail", MainModel);
                }

                int seqNo = StockGrid.Count + 1;

                foreach (var item in model)
                {
                    // ---------- Duplicate check (UNCHANGED LOGIC) ----------
                    if (StockGrid.Any(x =>
                        x.ItemCode == item.ItemCode &&
                        x.BatchNo == item.BatchNo &&
                        x.UniqueBatchNo == item.UniqueBatchNo))
                    {
                        return StatusCode(207, "Duplicate");
                    }

                    // ---------- Assign sequence ----------
                    item.SeqNo = seqNo;

                    seqNo++;

                    StockGrid.Add(item);
                }

                MainModel.InterStoreDetails = StockGrid;

                // ---------- Write session ONCE ----------
                HttpContext.Session.SetString(
                    $"KeyInterStoreTransferGrid_{uniqueKey}",
                    JsonConvert.SerializeObject(MainModel.InterStoreDetails)
                );

                //HttpContext.Session.SetString(
                //    "IssueNRGP",
                //    JsonConvert.SerializeObject(MainModel)
                //);

                return PartialView("_InterStoreTransferDetail", MainModel);
            }
            catch (Exception ex)
            {
                throw;
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
                var uniqueKey = model.uniqueKey;
                var formKey = model.formKey;
                string modelJson = HttpContext.Session.GetString($"KeyInterStoreTransferGrid_{uniqueKey}");
                List<InterStoreTransferDetail> ISTDetail = new List<InterStoreTransferDetail>();
                if (!string.IsNullOrEmpty(modelJson))
                {
                    ISTDetail = JsonConvert.DeserializeObject<List<InterStoreTransferDetail>>(modelJson);
                }

                if (ISTDetail == null)
                {
                    ModelState.Clear();
                    ModelState.TryAddModelError("InterStoreTransferDetail", "InterStoreTransfer Grid Should Have Atleast 1 Item...!");
                    return Json(new
                    {
                        success = false,
                        message = "An unexpected error occurred."
                    });
                }
                else
                {
                    model.CC = HttpContext.Session.GetString($"Branch_{formKey}");
                    model.Uid = Convert.ToInt32(HttpContext.Session.GetString($"UID_{formKey}"));
                    model.ActualEntryBy = Convert.ToInt32(HttpContext.Session.GetString($"UID_{formKey}"));
                    model.ActualEntryByName = HttpContext.Session.GetString($"EmpName_{formKey}");
                    if (model.Mode == "U")
                    {
                        model.LastUpdatedBy = Convert.ToInt32(HttpContext.Session.GetString($"UID_{formKey}"));
                        model.LastUpdatedByName = HttpContext.Session.GetString($"EmpName_{formKey}");
                    }

                    ISTGrid = GetDetailTable(ISTDetail);
                    model.MachineName = HttpContext.Session.GetString($"ClientMachineName_{formKey}");
                    model.IPAddress = HttpContext.Session.GetString($"ClientIP_{formKey}");

                    var Result = await IInterStore.SaveInterStore(model, ISTGrid);

                    if (Result != null)
                    {
                        if (Result.StatusText == "Success" && Result.StatusCode == HttpStatusCode.OK)
                        {
                            ViewBag.isSuccess = true;
                            TempData["200"] = "200";
                            var model1 = new InterStoreTransferModel();
                            model1.FinFromDate = HttpContext.Session.GetString($"FromDate_{formKey}");
                            model1.FinToDate = HttpContext.Session.GetString($"ToDate_{formKey}");
                            model1.YearCode = Convert.ToInt32(HttpContext.Session.GetString($"YearCode_{formKey}"));
                            model1.CC = HttpContext.Session.GetString($"Branch_{formKey}");
                            model1.CreatedBy = Convert.ToInt32(HttpContext.Session.GetString($"UID_{formKey}"));
                            HttpContext.Session.Remove($"KeyInterStoreTransferGrid_{uniqueKey}");
                            return Json(new
                            {
                                success = true,
                                message = "Data Saved successfully",
                                redirectUrl = Url.Action(
               "Index",
               "InterStoreTransfer",
                       new { formKey = formKey }


           )
                            });
                        }
                        if (Result.StatusText == "Updated" && Result.StatusCode == HttpStatusCode.Accepted)
                        {
                            ViewBag.isSuccess = true;
                            TempData["202"] = "202";
                            var model1 = new InterStoreTransferModel();
                            model1.FinFromDate = HttpContext.Session.GetString($"FromDate_{formKey}");
                            model1.FinToDate = HttpContext.Session.GetString($"ToDate_{formKey}");
                            model1.YearCode = Convert.ToInt32(HttpContext.Session.GetString($"YearCode_{formKey}"));
                            model1.CC = HttpContext.Session.GetString($"Branch_{formKey}");
                            model1.CreatedBy = Convert.ToInt32(HttpContext.Session.GetString($"UID_{formKey}"));
                            HttpContext.Session.Remove($"KeyInterStoreTransferGrid_{uniqueKey}");
                            return Json(new
                            {
                                success = true,
                                message = "Data Saved successfully",
                                redirectUrl = Url.Action(
               "Index",
               "InterStoreTransfer",
                       new { formKey = formKey }


           )
                            });
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
                                model2.FinFromDate = HttpContext.Session.GetString($"FromDate_{formKey}");
                                model2.FinToDate = HttpContext.Session.GetString($"ToDate_{formKey}");
                                model2.YearCode = Convert.ToInt32(HttpContext.Session.GetString($"YearCode_{formKey}"));
                                model2.CC = HttpContext.Session.GetString($"Branch_{formKey}");
                                model2.CreatedBy = Convert.ToInt32(HttpContext.Session.GetString($"UID_{formKey}"));
                                //return RedirectToAction(nameof(InterStoreTransfer), new { formKey = formKey });
                                return Json(new
                                {
                                    success = false,
                                    message = "An unexpected error occurred."
                                });
                            }

                            ViewBag.isSuccess = false;
                            TempData["500"] = "500";
                            Logger.LogError("\n \n ********** LogError ********** \n " + JsonConvert.SerializeObject(Result) + "\n \n");
                            //return RedirectToAction(nameof(ISTDashboard), new { formKey = formKey });
                            return Json(new
                            {
                                success = false,
                                message = "An unexpected error occurred."
                            });
                        }
                        if (Result.StatusText == "TransDate" || Result.StatusCode == HttpStatusCode.InternalServerError)
                        {
                            ViewBag.isSuccess = false;
                            var input = "";
                            if (Result?.Result != null)
                            {
                                if (Result.Result is string str)
                                {
                                    input = str;
                                }
                                else
                                {
                                    //input = JsonConvert.SerializeObject(Result.Result);
                                    var json = JsonConvert.SerializeObject(Result.Result);

                                    var list = JsonConvert.DeserializeObject<List<dynamic>>(json);

                                    if (list != null && list.Count > 0)
                                    {
                                        input = list[0].Result;
                                    }
                                }

                                TempData["ErrorMessage"] = input;
                            }
                            else
                            {
                                TempData["500"] = "500";
                            }


                            Logger.LogError("\n \n ********** LogError ********** \n " + JsonConvert.SerializeObject(Result) + "\n \n");
                            //model.IsError = "true";
                            //return View("Error", Result);
                        }
                    }
                    //return RedirectToAction(nameof(ISTDashboard), new { formKey = formKey });
                    return Json(new
                    {
                        success = true,
                        message = "Data Saved successfully",
                        redirectUrl = Url.Action(
       "Index",
       "InterStoreTransfer",
               new { formKey = formKey }


   )
                    });
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
        public async Task<IActionResult> ISTDashboard(string formKey, string SlipNo, string PartCode, string ItemName, string BatchNo, string SummaryDetail, string Flag = "True", string FromDate = "", string ToDate = "")
        {
            ViewBag.formKey = formKey;
            var uniqueKey = Guid.NewGuid().ToString();
            ViewBag.uniqueKey = uniqueKey;
            HttpContext.Session.Remove($"KeyInterStoreTransferGrid_{uniqueKey}");
            var model = new ISTDashboard();
            var yearCode = Convert.ToInt32(HttpContext.Session.GetString($"YearCode_{formKey}"));
            DateTime now = DateTime.Now;
            DateTime firstDayOfMonth = new DateTime(yearCode, now.Month, 1);
            model.FromDate = new DateTime(yearCode, now.Month, 1).ToString("dd/MM/yyyy").Replace("-", "/");
            model.ToDate = new DateTime(yearCode + 1, 3, 31).ToString("dd/MM/yyyy").Replace("-", "/");

            var Result = await IInterStore.GetDashboardData(model);

            if (Result.Result != null)
            {
                var _List = new List<TextValue>();
                DataSet DS = Result.Result;

                var DT = DS.Tables[0].DefaultView.ToTable(true, "EntryId", "Yearcode", "EntryDate",
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

        public async Task<IActionResult> DeleteByID(int ID, int YC, string EntryDate, int ActualEntryBy, string MachineName, string SummaryDetail, string FromDate = "", string ToDate = "", string SlipNo = "", string PartCode = "", string ItemName = "", string BatchNo = "")
        {
            var Result = await IInterStore.DeleteByID(ID, YC, EntryDate, ActualEntryBy, MachineName).ConfigureAwait(false);

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
            return RedirectToAction("Dashboard", new { Flag = "false", FromDate = FromDate, ToDate = ToDate, SlipNo = SlipNo, PartCode = PartCode, ItemName = ItemName, BatchNo = BatchNo, SummaryDetail = SummaryDetail });
        }

        public async Task<IActionResult> GetSearchData(ISTDashboard model)
        {
            ViewBag.formKey = model.formKey;
            model.FromDate = ParseFormattedDate(model.FromDate);
            model.ToDate = ParseFormattedDate(model.ToDate);
            var Result = await IInterStore.GetDashboardData(model);
            if (Result.Result != null)
            {
                DataSet DS = Result.Result;

                var DT = DS.Tables[0].DefaultView.ToTable(true, "EntryId", "Yearcode", "EntryDate",
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
            _memoryCache.Set("InterstoreList", model.ISTDashboardGrid);
            return PartialView("_ISTDashboardGrid", model);
        }

        [HttpGet]
        public IActionResult GetInterStoreDashboardListForPDF()
        {
            if (_memoryCache.TryGetValue("InterstoreList", out List<InterStoreDashboard> interStoreTransList))
            {
                return Json(interStoreTransList);
            }
            return Json(new List<InterStoreDashboard>());
        }
        private static DataTable GetDetailTable(IList<InterStoreTransferDetail> DetailList)
        {
            var DTSSGrid = new DataTable();

            DTSSGrid.Columns.Add("Entryid", typeof(int));
            DTSSGrid.Columns.Add("Yearcode", typeof(int));
            DTSSGrid.Columns.Add("ItemCode", typeof(int));
            DTSSGrid.Columns.Add("SeqNo", typeof(int));
            DTSSGrid.Columns.Add("TotalStockQty", typeof(decimal));
            DTSSGrid.Columns.Add("LotStockQty", typeof(decimal));
            DTSSGrid.Columns.Add("Qty", typeof(decimal));
            DTSSGrid.Columns.Add("Unit", typeof(string));
            DTSSGrid.Columns.Add("AltQty", typeof(decimal));
            DTSSGrid.Columns.Add("AltUnit", typeof(string));
            DTSSGrid.Columns.Add("Rate", typeof(decimal));
            DTSSGrid.Columns.Add("Batchno", typeof(string));
            DTSSGrid.Columns.Add("Uniquebatchno", typeof(string));
            DTSSGrid.Columns.Add("ReasonOfransfer", typeof(string));
            DTSSGrid.Columns.Add("RecStoreStock", typeof(decimal));
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
        public async Task<JsonResult> EditItemRows(int SeqNo, string uniqueKey)
        {
            var MainModel = new InterStoreTransferModel();
            string modelJson = HttpContext.Session.GetString($"KeyInterStoreTransferGrid_{uniqueKey}");
            List<InterStoreTransferDetail> InterStoreGrid = new List<InterStoreTransferDetail>();
            if (!string.IsNullOrEmpty(modelJson))
            {
                InterStoreGrid = JsonConvert.DeserializeObject<List<InterStoreTransferDetail>>(modelJson);
            }
            var ISTGrid = InterStoreGrid.Where(x => x.SeqNo == SeqNo);
            string JsonString = JsonConvert.SerializeObject(ISTGrid);
            return Json(JsonString);
        }
        public IActionResult ClearGrid(string uniqueKey)
        {
            HttpContext.Session.Remove($"KeyInterStoreTransferGrid_{uniqueKey}");
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
        }
        public async Task<JsonResult> FillStore()
        {
            var JSON = await IInterStore.FillStore();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> CheckIssuedTransStock(int ItemCode, int YearCode, int EntryId, string TransDate, string TransNo, int Storeid, string batchno, string uniquebatchno, string Flag)
        {
            var JSON = await IInterStore.CheckIssuedTransStock(ItemCode, YearCode, EntryId, TransDate, TransNo, Storeid, batchno, uniquebatchno, Flag);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetPrevQty(int EntryId, int YearCode, int ItemCode, string uniqueBatchno)
        {
            var JSON = await IInterStore.GetPrevQty(EntryId, YearCode, ItemCode, uniqueBatchno);
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
        public async Task<JsonResult> FillStockBatchNo(int ItemCode, string formKey, string StoreName, int YearCode, string batchno)
        {
            var FinStartDate = HttpContext.Session.GetString($"FromDate_{formKey}");
            var JSON = await IInterStore.FillStockBatchNo(ItemCode, StoreName, YearCode, batchno, FinStartDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetUnitAltUnit(int ItemCode)
        {
            var JSON = await IInterStore.GetUnitAltUnit(ItemCode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }

        public async Task<IActionResult> selectMultipleItem(string formKey, string GroupName, string CatName, int StoreID, string ToDate, string PartCode, string ItemName)
        {
            var model = new InterStoreTransferModel();
            var FromDate = CommonFunc.ParseFormattedDate(HttpContext.Session.GetString($"FromDate_{formKey}"));
            model = await IInterStore.selectMultipleItem(GroupName, CatName, StoreID, FromDate, ToDate, PartCode, ItemName);


            return PartialView("_InterStoreTransferShowAllItemGrid", model);

        }
    }
}
