using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System.Data;
using System.Globalization;
using System.Net;
using static eTactWeb.Data.Common.CommonFunc;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Controllers
{
    public class TransferFromWorkCenterController : Controller
    {
        private readonly IDataLogic _IDataLogic;
        private readonly ITransferFromWorkCenter _ITransferFromWorkCenter;
        private readonly ILogger<TransferFromWorkCenterController> _logger;
        private readonly IMemoryCache _MemoryCache;
        private readonly IWebHostEnvironment _IWebHostEnvironment;
        public TransferFromWorkCenterController(ILogger<TransferFromWorkCenterController> logger, IDataLogic iDataLogic, ITransferFromWorkCenter ITransferFromWorkCenter, IMemoryCache iMemoryCache, IWebHostEnvironment iWebHostEnvironment)
        {
            _logger = logger;
            _IDataLogic = iDataLogic;
            _ITransferFromWorkCenter = ITransferFromWorkCenter;
            _MemoryCache = iMemoryCache;
            _IWebHostEnvironment = iWebHostEnvironment;
        }
        [Route("{controller}/Index")]
        public async Task<IActionResult> TransferFromWorkCenter()
        {
            ViewData["Title"] = "Transfer From WorkCenter Detail";
            TempData.Clear();
            _MemoryCache.Remove("KeyTransferFromWorkCenterGrid");
            var MainModel = new TransferFromWorkCenterModel();
            MainModel.FromDate = HttpContext.Session.GetString("FromDate");
            MainModel.ToDate = HttpContext.Session.GetString("ToDate");
            MainModel.TransferMatYearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
            MainModel.CC = HttpContext.Session.GetString("Branch");
            MainModel.ActualEnteredBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
            MainModel.ActualEnteredByName = HttpContext.Session.GetString("EmpName");
            //MainModel.ActualEntrydate = HttpContext.Session.GetString("EntryDate");
            MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                SlidingExpiration = TimeSpan.FromMinutes(55),
                Size = 1024,
            };

            _MemoryCache.Set("KeyTransferFromWorkCenterGrid", MainModel, cacheEntryOptions);
            return View(MainModel);
        }
        [Route("{controller}/Index")]
        [HttpGet]
        public async Task<ActionResult> TransferFromWorkCenter(int ID, string Mode, int YC, string FromDate = "", string ToDate = "", string TransferSlipNo = "", string ItemName = "", string PartCode = "", string FromWorkCenter = "", string ToWorkCenter = "",string StoreName = "",string ProdSlipNo="",string ProdSchNo="", string Searchbox = "", string DashboardType = "" )//, ILogger logger)
        {
            //_logger.LogInformation("\n \n ********** Page Gate Inward ********** \n \n " + IWebHostEnvironment.EnvironmentName.ToString() + "\n \n");
            TempData.Clear();
            var MainModel = new TransferFromWorkCenterModel();
            MainModel.CC = HttpContext.Session.GetString("Branch");
            MainModel.TransferMatYearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
            MainModel.FromDate = HttpContext.Session.GetString("FromDate");
            MainModel.ToDate = HttpContext.Session.GetString("ToDate");
            _MemoryCache.Remove("KeyTransferFromWorkCenterGrid");
            if (!string.IsNullOrEmpty(Mode) && ID > 0 && (Mode == "V" || Mode == "U"))
            {
                MainModel = await _ITransferFromWorkCenter.GetViewByID(ID, YC).ConfigureAwait(false);
                MainModel.Mode = Mode;
                MainModel.ID = ID;
                MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                    SlidingExpiration = TimeSpan.FromMinutes(55),
                    Size = 1024,
                };
                _MemoryCache.Set("KeyTransferFromWorkCenterGrid", MainModel.ItemDetailGrid, cacheEntryOptions);
            }
            else
            {
                // MainModel = await BindModel(MainModel);
            }

            if (Mode != "U")
            {
                MainModel.ActualEnteredBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                MainModel.ActualEnteredByName = HttpContext.Session.GetString("EmpName");
                MainModel.ActualEntrydate =DateTime.Now;
            }
            else
            {
                MainModel.ActualEnteredByName = HttpContext.Session.GetString("EmpName");
                MainModel.UpdatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                MainModel.UpdatedByName = HttpContext.Session.GetString("EmpName");
                MainModel.UpdatedOn = DateTime.Now;
            }
            MainModel.FromDateBack = FromDate;
            MainModel.ToDateBack = ToDate;
            MainModel.TransferSlipNoBack = TransferSlipNo;
            MainModel.PartCodeBack = PartCode;
            MainModel.ItemNameBack = ItemName;
            MainModel.FromWorkCenterBack = FromWorkCenter;
            MainModel.ToWorkCenterBack = ToWorkCenter;
            MainModel.StoreNameBack = StoreName;
            MainModel.ProdSlipNoBack=ProdSlipNo;
            MainModel.ProdSchNoBack=ProdSchNo;
            MainModel.GlobalSearchBack = Searchbox;
            MainModel.DashboardTypeBack = DashboardType;
            return View(MainModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("{controller}/Index")]
        public async Task<IActionResult> TransferFromWorkCenter(TransferFromWorkCenterModel model)
        {
            try
            {
                var TransferGrid = new DataTable();

                _MemoryCache.TryGetValue("KeyTransferFromWorkCenterGrid", out List<TransferFromWorkCenterDetail> TransferFromWorkCenterDetail);

                if (TransferFromWorkCenterDetail == null)
                {
                    ModelState.Clear();
                    ModelState.TryAddModelError("TransferFromWorkCenterItemDetail", "Transfer From WorkCenter Grid Should Have Atleast 1 Item...!");
                    return View("TransferFromWorkCenter", model);
                }

                else
                {
                    model.CC = HttpContext.Session.GetString("Branch");
                    model.ActualEnteredBy = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
                    model.Uid   = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
                    // model.ac = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                    if (model.Mode == "U")
                    {
                        model.UpdatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                        model.UpdatedByName = HttpContext.Session.GetString("EmpName");
                        TransferGrid = GetDetailTable(TransferFromWorkCenterDetail);
                    }
                    else
                    {
                        TransferGrid = GetDetailTable(TransferFromWorkCenterDetail);
                    }

                    var Result = await _ITransferFromWorkCenter.SaveTransferFromWorkCenter(model, TransferGrid);

                    if (Result != null)
                    {
                        if (Result.StatusText == "Success" && Result.StatusCode == HttpStatusCode.OK)
                        {
                            ViewBag.isSuccess = true;
                            TempData["200"] = "200";
                            _MemoryCache.Remove(TransferGrid);
                        }
                        if (Result.StatusText == "Success" && Result.StatusCode == HttpStatusCode.Accepted)
                        {
                            ViewBag.isSuccess = true;
                            TempData["202"] = "202";
                        }
                        if (Result.StatusText == "Error" && Result.StatusCode == HttpStatusCode.InternalServerError)
                        {

                            ViewBag.isSuccess = false;
                            TempData["500"] = "500";
                            _logger.LogError("\n \n ********** LogError ********** \n " + JsonConvert.SerializeObject(Result) + "\n \n");
                            return View("Error", Result);
                        }
                    }
                }
                return RedirectToAction(nameof(TransferFromWorkCenter));
            }
            catch (Exception ex)
            {
                LogException<TransferFromWorkCenterController>.WriteException(_logger, ex);


                var ResponseResult = new ResponseResult()
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    StatusText = "Error",
                    Result = ex
                };

                return View("Error", ResponseResult);
            }
        }
        public async Task<JsonResult> FillEntryandGate(int YearCode)
        {
            var JSON = await _ITransferFromWorkCenter.FillEntryandGate("NewEntryId", YearCode, "SP_TransferMaterialFromWc");
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> BindEmpList()
        {
            var JSON = await _ITransferFromWorkCenter.BindEmpList();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillStoreName()
        {
            var JSON = await _ITransferFromWorkCenter.FillStoreName();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillProcessName()
        {
            var JSON = await _ITransferFromWorkCenter.FillProcessName();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillWorkCenter()
        {
            var JSON = await _ITransferFromWorkCenter.FillWorkCenter();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillItemName(int TransferMatYearCode)
        {
            var JSON = await _ITransferFromWorkCenter.FillItemName(TransferMatYearCode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillPartCode(int TransferMatYearCode)
        {
            var JSON = await _ITransferFromWorkCenter.FillPartCode(TransferMatYearCode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public IActionResult AddTransferFromWorkCenter(TransferFromWorkCenterDetail model)
        {
            try
            {
                if (model.Mode == "U")
                {
                    _MemoryCache.TryGetValue("KeyTransferFromWorkCenterGrid", out IList<TransferFromWorkCenterDetail> TransferFromWorkCenterDetail);

                    var MainModel = new TransferFromWorkCenterModel();
                    var TransferWcDetail = new List<TransferFromWorkCenterDetail>();
                    var TransferGrid = new List<TransferFromWorkCenterDetail>();
                    var SSGrid = new List<TransferFromWorkCenterDetail>();

                    if (model != null)
                    {
                        if (TransferFromWorkCenterDetail == null)
                        {
                            model.SeqNo = 1;
                            TransferGrid.Add(model);
                        }
                        else
                        {
                            if (TransferFromWorkCenterDetail.Where(x => x.PartCode == model.PartCode && x.BatchNo == model.BatchNo).Any())
                            {
                                return StatusCode(207, "Duplicate");
                            }
                            else
                            {
                                model.SeqNo = TransferFromWorkCenterDetail.Count + 1;
                                TransferGrid = TransferFromWorkCenterDetail.Where(x => x != null).ToList();
                                SSGrid.AddRange(TransferGrid);
                                TransferGrid.Add(model);
                            }
                        }

                        MainModel.ItemDetailGrid = TransferGrid;

                        MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
                        {
                            AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                            SlidingExpiration = TimeSpan.FromMinutes(55),
                            Size = 1024,
                        };

                        _MemoryCache.Set("KeyTransferFromWorkCenterGrid", MainModel.ItemDetailGrid, cacheEntryOptions);
                    }
                    else
                    {
                        ModelState.TryAddModelError("Error", "Schedule List Cannot Be Empty...!");
                    }

                    return PartialView("_TransferFromWcGrid", MainModel);
                }
                else
                {
                    _MemoryCache.TryGetValue("KeyTransferFromWorkCenterGrid", out IList<TransferFromWorkCenterDetail> TransferFromWorkCenterDetail);

                    var MainModel = new TransferFromWorkCenterModel();
                    var TransferWcDetail = new List<TransferFromWorkCenterDetail>();
                    var TransferGrid = new List<TransferFromWorkCenterDetail>();
                    var SSGrid = new List<TransferFromWorkCenterDetail>();

                    if (model != null)
                    {
                        if (TransferFromWorkCenterDetail == null)
                        {
                            model.SeqNo = 1;
                            TransferGrid.Add(model);
                        }
                        else
                        {
                            if (TransferFromWorkCenterDetail.Where(x => x.PartCode == model.PartCode && x.BatchNo == model.BatchNo).Any())
                            {
                                return StatusCode(207, "Duplicate");
                            }
                            else
                            {
                                model.SeqNo = TransferFromWorkCenterDetail.Count + 1;
                                TransferGrid = TransferFromWorkCenterDetail.Where(x => x != null).ToList();
                                SSGrid.AddRange(TransferGrid);
                                TransferGrid.Add(model);
                            }

                        }

                        MainModel.ItemDetailGrid = TransferGrid;

                        MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
                        {
                            AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                            SlidingExpiration = TimeSpan.FromMinutes(55),
                            Size = 1024,
                        };

                        _MemoryCache.Set("KeyTransferFromWorkCenterGrid", MainModel.ItemDetailGrid, cacheEntryOptions);
                    }
                    else
                    {
                        ModelState.TryAddModelError("Error", "Schedule List Cannot Be Empty...!");
                    }

                    return PartialView("_TransferFromWcGrid", MainModel);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<JsonResult> GetBatchNumber(int ItemCode, int YearCode, float WcId, string TransDate, string BatchNo)
        {
            if(BatchNo == "-Select-")
            {
                BatchNo = "";
            }
            var JSON = await _ITransferFromWorkCenter.GetBatchNumber("FillCurrentBatchINWIP", ItemCode, YearCode, WcId, TransDate, BatchNo);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetWorkCenterTotalStock(int ItemCode, int WcId, string TillDate)
        {
            var JSON = await _ITransferFromWorkCenter.GetWorkCenterTotalStock("GETWIPotalSTOCK", ItemCode, WcId, TillDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetWorkCenterQty(int ItemCode, int WcId, string TillDate, string BatchNo, string UniqueBatchNo)
        {
            var JSON = await _ITransferFromWorkCenter.GetWorkCenterQty("GETWIPSTOCKBATCHWISE", ItemCode, WcId, TillDate, BatchNo, UniqueBatchNo);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetUnit(int ItemCode)
        {
            var JSON = await _ITransferFromWorkCenter.GetUnit(ItemCode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetServerDate()
        {
            try
            {
                DateTime time = DateTime.Now;
                string format = "MMM ddd d HH:mm yyyy";
                string formattedDate = time.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);

                var dt = time.ToString(format);
                return Json(formattedDate);
                //string apiUrl = "https://worldtimeapi.org/api/ip";

                //using (HttpClient client = new HttpClient())
                //{
                //    HttpResponseMessage response = await client.GetAsync(apiUrl);

                //    if (response.IsSuccessStatusCode)
                //    {
                //        string content = await response.Content.ReadAsStringAsync();
                //        JObject jsonObj = JObject.Parse(content);

                //        string datetimestring = (string)jsonObj["datetime"];
                //        var formattedDateTime = datetimestring.Split(" ")[0];

                //        DateTime parsedDate = DateTime.ParseExact(formattedDateTime, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                //        string formattedDate = parsedDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);

                //        return Json(formattedDate);
                //    }
                //    else
                //    {
                //        string errorContent = await response.Content.ReadAsStringAsync();
                //        throw new HttpRequestException($"Failed to fetch server date and time. Status Code: {response.StatusCode}. Content: {errorContent}");
                //    }
                //}
            }
            catch (HttpRequestException ex)
            {
                // Log the exception for debugging purposes
                Console.WriteLine($"HttpRequestException: {ex.Message}");
                return Json(new { error = "Failed to fetch server date and time: " + ex.Message });
            }
            catch (Exception ex)
            {
                // Log any other unexpected exceptions
                Console.WriteLine($"Unexpected Exception: {ex.Message}");
                return Json(new { error = "An unexpected error occurred: " + ex.Message });
            }
        }
        public IActionResult DeleteItemRow(int SeqNo, string Mode)
        {
            var MainModel = new TransferFromWorkCenterModel();
            if (Mode == "U")
            {
                _MemoryCache.TryGetValue("KeyTransferFromWorkCenterGrid", out List<TransferFromWorkCenterDetail> TransferFromWorkCenterDetail);
                int Indx = Convert.ToInt32(SeqNo) - 1;

                if (TransferFromWorkCenterDetail != null && TransferFromWorkCenterDetail.Count > 0)
                {
                    TransferFromWorkCenterDetail.RemoveAt(Convert.ToInt32(Indx));

                    Indx = 0;

                    foreach (var item in TransferFromWorkCenterDetail)
                    {
                        Indx++;
                        item.SeqNo = Indx;
                    }
                    MainModel.ItemDetailGrid = TransferFromWorkCenterDetail;

                    MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
                    {
                        AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                        SlidingExpiration = TimeSpan.FromMinutes(55),
                        Size = 1024,
                    };

                    _MemoryCache.Set("KeyTransferFromWorkCenterGrid", MainModel.ItemDetailGrid, cacheEntryOptions);
                }
            }
            else
            {
                _MemoryCache.TryGetValue("KeyTransferFromWorkCenterGrid", out List<TransferFromWorkCenterDetail> TransferFromWorkCenterDetail);
                int Indx = Convert.ToInt32(SeqNo) - 1;

                if (TransferFromWorkCenterDetail != null && TransferFromWorkCenterDetail.Count > 0)
                {
                    TransferFromWorkCenterDetail.RemoveAt(Convert.ToInt32(Indx));

                    Indx = 0;

                    foreach (var item in TransferFromWorkCenterDetail)
                    {
                        Indx++;
                        item.SeqNo = Indx;
                    }
                    MainModel.ItemDetailGrid = TransferFromWorkCenterDetail;

                    MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
                    {
                        AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                        SlidingExpiration = TimeSpan.FromMinutes(55),
                        Size = 1024,
                    };

                    _MemoryCache.Set("KeyTransferFromWorkCenterGrid", MainModel.ItemDetailGrid, cacheEntryOptions);
                }
            }

            return PartialView("_TransferFromWcGrid", MainModel);
        }
        public IActionResult EditItemRow(int SeqNo, string Mode)
        {
            IList<TransferFromWorkCenterDetail> TransferFromWorkCenterDetail = new List<TransferFromWorkCenterDetail>();
            if (Mode == "U")
            {
                _MemoryCache.TryGetValue("KeyTransferFromWorkCenterGrid", out TransferFromWorkCenterDetail);
            }
            else
            {
                _MemoryCache.TryGetValue("KeyTransferFromWorkCenterGrid", out TransferFromWorkCenterDetail);
            }
            IEnumerable<TransferFromWorkCenterDetail> SSGrid = TransferFromWorkCenterDetail;
            if (TransferFromWorkCenterDetail != null)
            {
                SSGrid = TransferFromWorkCenterDetail.Where(x => x.SeqNo == SeqNo);
            }
            string JsonString = JsonConvert.SerializeObject(SSGrid);
            return Json(JsonString);
        }
        private static DataTable GetDetailTable(IList<TransferFromWorkCenterDetail> DetailList)
        {
            try
            {
                var TransferGrid = new DataTable();

                TransferGrid.Columns.Add("TransferMatEntryId", typeof(int));
                TransferGrid.Columns.Add("TransferMatYearCode", typeof(int));
                TransferGrid.Columns.Add("seqno", typeof(int));
                TransferGrid.Columns.Add("ProdEntryId", typeof(int));
                TransferGrid.Columns.Add("ProdSlipNo", typeof(string));
                TransferGrid.Columns.Add("ProdYearCode", typeof(int));
                TransferGrid.Columns.Add("ProdEntryDate", typeof(string));
                TransferGrid.Columns.Add("ProdPlanNo", typeof(string));
                TransferGrid.Columns.Add("ProdPlanYearCode", typeof(int));
                TransferGrid.Columns.Add("ProdSchNo", typeof(string));
                TransferGrid.Columns.Add("ProdSchYearCode", typeof(int));
                TransferGrid.Columns.Add("ParentProdSchNo", typeof(string));
                TransferGrid.Columns.Add("ParentProdSchYearCode", typeof(int));
                TransferGrid.Columns.Add("ItemCode", typeof(int));
                TransferGrid.Columns.Add("TransferQty", typeof(float));
                TransferGrid.Columns.Add("QCOkQty", typeof(float));
                TransferGrid.Columns.Add("ProdQty", typeof(float));
                TransferGrid.Columns.Add("Unit", typeof(string));
                TransferGrid.Columns.Add("AltTransferQty", typeof(float));
                TransferGrid.Columns.Add("AltUnit", typeof(string));
                TransferGrid.Columns.Add("Remark", typeof(string));
                TransferGrid.Columns.Add("PendingToAcknowledge", typeof(string));
                TransferGrid.Columns.Add("PendingQtyToAcknowledge", typeof(float));
                TransferGrid.Columns.Add("ItemSize", typeof(string));
                TransferGrid.Columns.Add("ItemColor", typeof(string));
                TransferGrid.Columns.Add("InProcQCSlipNo", typeof(string));
                TransferGrid.Columns.Add("InProcQCEntryId", typeof(int));
                TransferGrid.Columns.Add("QCClearingDate", typeof(string));
                TransferGrid.Columns.Add("InProcQCYearCode", typeof(int));
                TransferGrid.Columns.Add("ProcessId", typeof(int));
                TransferGrid.Columns.Add("TotalStock", typeof(float));
                TransferGrid.Columns.Add("BatchNo", typeof(string));
                TransferGrid.Columns.Add("uniquebatchno", typeof(string));
                TransferGrid.Columns.Add("BatchStock", typeof(float));
                TransferGrid.Columns.Add("ReceivedByStoreQty", typeof(float));
                TransferGrid.Columns.Add("ReceivedCompleted", typeof(string));
                TransferGrid.Columns.Add("ReceivedByEmpId", typeof(int));
                TransferGrid.Columns.Add("Rate", typeof(float));
                TransferGrid.Columns.Add("ItemWeight", typeof(float));
                foreach (var Item in DetailList)
                {
                    TransferGrid.Rows.Add(
                        new object[]
                        {

                    Item.TransferMatEntryId == 0 ? 0 : Item.TransferMatEntryId,
                    Item.TransferMatYearCode== 0 ? 0 : Item.TransferMatYearCode,
                    Item.SeqNo==0?0:Item.SeqNo,
                    Item.ProdEntryId == 0 ? 0:Item.ProdEntryId,
                    Item.ProdSlipNo==null?"":Item.ProdSlipNo,
                    Item.ProdEntryYearCode== 0 ? 0:Item.ProdEntryYearCode,
                    Item.ProdDate == null ? string.Empty : ParseFormattedDate(Item.ProdDate.Split(" ")[0]),
                    Item.ProdPlanNo == null ? "" : Item.ProdPlanNo,
                    Item.ProdPlanYearCode== 0 ? 0:Item.ProdPlanYearCode,
                    Item.ProdSchNo==null?"":Item.ProdSchNo,
                    Item.ProdSchYearCode== 0 ? 0:Item.ProdSchYearCode,
                    Item.ParentProdSchNo == null ? "" : Item.ParentProdSchNo,
                    Item.ParentProdSchYearCode== 0 ? 0:Item.ParentProdSchYearCode,
                    Item.ItemCode == 0 ? 0 : Item.ItemCode ,
                    Item.TransferQty==0?0:Item.TransferQty,
                    Item.QcOkQty==0?0:Item.QcOkQty,
                    Item.ProdQty==0?0:Item.ProdQty,
                    Item.Unit == null ? "" : Item.Unit,
                    Item.AltTransferQty==0?0:Item.AltTransferQty,
                    Item.AltUnit == null ? "" : Item.AltUnit,
                    Item.Remark == null ? "" : Item.Remark,
                    Item.PendingToAcknowledge == null ? "" : Item.PendingToAcknowledge,
                    Item.PendingQtyToAcknowledge==0?0:Item.PendingQtyToAcknowledge,
                    Item.ItemSize == null ? "" : Item.ItemSize,
                    Item.ItemColor==null?"":Item.ItemColor,
                    Item.InProcessQcSlipNo == null ? "" : Item.InProcessQcSlipNo,
                    Item.InProcessQcEntryId==0?0:Item.InProcessQcEntryId,
                    Item.QcCleaningDate == null ? string.Empty : ParseFormattedDate(Item.QcCleaningDate.Split(" ")[0]),
                    Item.InProcessQcYearCode==0?0:Item.InProcessQcYearCode,
                    Item.ProcessId==0?0:Item.ProcessId,
                    Item.TotalStock==0?0:Item.TotalStock,
                    Item.BatchNo == null ? "" : Item.BatchNo,
                    Item.UniqueBatchNo == null ? "" : Item.UniqueBatchNo,
                    Item.BatchStock==0?0:Item.BatchStock,
                    Item.ReceivedByStoreQty==0?0:Item.ReceivedByStoreQty,
                    Item.ReceivedCompleted == null ? "" : Item.ReceivedCompleted,
                    Item.ReceivedByEmpId==0?0:Item.ReceivedByEmpId,
                    Item.Rate==0?0:Item.Rate,
                    Item.ItemWeight==0?0:Item.ItemWeight,
                        });
                }
                TransferGrid.Dispose();
                return TransferGrid;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<IActionResult> TransferFromWorkCenterDashboard(string FromDate = "", string ToDate = "", string Flag = "True", string TransferSlipNo = "", string ItemName = "", string PartCode = "", string FromWorkCenter = "", string ToWorkCenter = "", string StoreName = "", string ProdSlipNo = "", string ProdSchNo = "", string Searchbox = "", string DashboardType = "")
        {
            try
            {
                _MemoryCache.Remove("KeyTransferFromWorkCenterGrid");

                var model = new TransferFromWorkCenterDashboard();
                var Result = await _ITransferFromWorkCenter.GetDashboardData().ConfigureAwait(true);
                DateTime now = DateTime.Now;

                model.FromDate = new DateTime(now.Year, now.Month, 1).ToString("dd/MM/yyyy").Replace("-", "/");
                model.ToDate = new DateTime(DateTime.Today.Year + 1, 3, 31).ToString("dd/MM/yyyy").Replace("-", "/");

                if (Result != null)
                {
                    var _List = new List<TextValue>();
                    DataSet DS = Result.Result;
                    if (DS != null)
                    {
                        var DT = DS.Tables[0].DefaultView.ToTable(true, "TransferMatSlipNo", "TransferMatEntrydate",
                            "TransferMatSlipDate", "IssueToStoreWC", "TransferFromWC", "TransferToWC",
                            "TransferToStore", "Remark", "PendingToRecByStore", "IssuedByEmpName", "ActualEntryByEmpName",
                             "UID", "CC", "EntryByMachineNo", "ActualEntryDate", "UpdatedByEmpName", "LastUpdationDate"
                             , "TransferMatEntryId", "TransferMatYearCode");
                        model.TransferFromWorkCenterDashboard = CommonFunc.DataTableToList<TransferFromDashboard>(DT, "TransferMaterialFromWc");

                    }
                    if (Flag != "True")
                    {
                        model.FromDate1 = FromDate;
                        model.ToDate1 = ToDate;
                        model.TransferMatSlipNo=TransferSlipNo;
                        model.ItemName=ItemName;
                        model.PartCode=PartCode;
                        model.TransferFromWC=FromWorkCenter;
                        model.TransferToWC=ToWorkCenter;
                        model.TransferToStore=StoreName;
                        model.ProdSlipNo=ProdSlipNo;
                        model.ProdSchNo=ProdSchNo;
                        model.Searchbox=Searchbox;
                        model.DashboardType=DashboardType;
                        return View(model);
                    }
                }
                return View(model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IActionResult> GetSearchData(string FromDate, string ToDate, string TransferMatSlipNo, string ItemName, string PartCode, string TransferFromWC, string TransferToWC, string TransferToStore, string ProdSlipNo, string ProdSchNo, string DashboardType)
        {
            //model.Mode = "Search";
            var model = new TransferFromWorkCenterDashboard();
            model = await _ITransferFromWorkCenter.GetDashboardData(FromDate, ToDate, TransferMatSlipNo, ItemName, PartCode, TransferFromWC, TransferToWC, TransferToStore, ProdSlipNo, ProdSchNo, DashboardType);
            model.DashboardType = "SUMMARY";
            return PartialView("_TransferFromWorkCenterDashboard", model);
        }
        public async Task<IActionResult> GetDetailData(string FromDate, string ToDate, string TransferMatSlipNo, string ItemName, string PartCode, string TransferFromWC, string TransferToWC, string TransferToStore, string ProdSlipNo, string ProdSchNo, string DashboardType)
        {
            //model.Mode = "Search";
            var model = new TransferFromWorkCenterDashboard();
            model = await _ITransferFromWorkCenter.GetDashboardDetailData(FromDate, ToDate, TransferMatSlipNo,ItemName,PartCode,TransferFromWC,TransferToWC,TransferToStore,ProdSlipNo,ProdSchNo, DashboardType);
            model.DashboardType = "DETAIL";
            return PartialView("_TransferFromWorkCenterDashboardDetail", model);
        }
        public async Task<IActionResult> DeleteByID(int ID, int YC, string CC, string EntryByMachineName, string EntryDate, string FromDate = "", string ToDate = "", string TransferSlipNo = "", string ItemName = "", string PartCode = "", string FromWorkCenter = "", string ToWorkCenter = "", string StoreName = "", string ProdSlipNo = "", string ProdSchNo = "", string Searchbox = "", string DashboardType = "")
        {
            var Result = await _ITransferFromWorkCenter.DeleteByID(ID, YC, CC, EntryByMachineName, EntryDate);

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

            return RedirectToAction("TransferFromWorkCenterDashboard", new { EntryDate = EntryDate, Flag = "False", CC = CC, EntryByMachineName = EntryByMachineName,FromDate=FromDate,ToDate=ToDate,TransferSlipNo=TransferSlipNo,ItemName=ItemName,PartCode=PartCode,FromWorkCenter=FromWorkCenter,ToWorkCenter=ToWorkCenter,StoreName=StoreName,ProdSlipNo=ProdSlipNo,ProdSchNo=ProdSchNo,DashboardType=DashboardType });
        }
        public async Task<JsonResult> CheckEditOrDelete(int TransferEntryId, int TransferYearCode)
        {
            var JSON = await _ITransferFromWorkCenter.CheckEditOrDelete(TransferEntryId, TransferYearCode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
    }
}
