using eTactWeb.Data.Common;
using eTactWeb.Data.DAL;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using FastReport;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Net;
using System.Runtime.Caching;
using static eTactWeb.Data.Common.CommonFunc;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Controllers
{
    public class IssueThrBOMController : Controller
    {
        private readonly IDataLogic _IDataLogic;
        private readonly IIssueThrBOM _IIssueThrBOM;
        private readonly ILogger<IssueThrBOMController> _logger;
        private readonly IMemoryCache _MemoryCache;
        private readonly IWebHostEnvironment _IWebHostEnvironment;

        public IssueThrBOMController(ILogger<IssueThrBOMController> logger, IDataLogic iDataLogic, IIssueThrBOM IIssueWOBOM, IMemoryCache iMemoryCache, IWebHostEnvironment iWebHostEnvironment)
        {
            _logger = logger;
            _IDataLogic = iDataLogic;
            _IIssueThrBOM = IIssueWOBOM;
            _MemoryCache = iMemoryCache;
            _IWebHostEnvironment = iWebHostEnvironment;
        }

        [Route("{controller}/Index")]
        public async Task<IActionResult> IssueThrBOM()
        {
            ViewData["Title"] = "Issue Through BOM Details";
            TempData.Clear();
            _MemoryCache.Remove("KeyIssThrBomGrid");
            _MemoryCache.Remove("KeyIssThrBomScannedGrid");
            _MemoryCache.Remove("KeyIssThrBomFGGrid");
            var MainModel = new IssueThrBom();
            MainModel.FromDate = HttpContext.Session.GetString("FromDate");
            MainModel.ToDate = HttpContext.Session.GetString("ToDate");
            MainModel.YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
            MainModel.CC = HttpContext.Session.GetString("Branch");
            MainModel.ActualEnteredBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
            MainModel.ActualEnteredByName = HttpContext.Session.GetString("EmpName");
            MainModel.ActualEntrydate = DateTime.Now;
            //MainModel = await BindModel(MainModel);
            MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                SlidingExpiration = TimeSpan.FromMinutes(55),
                Size = 1024,
            };

            _MemoryCache.Set("KeyIssThrBomGrid", MainModel, cacheEntryOptions);
            //MainModel.DateIntact = "N";
            return View(MainModel);
        }

        [Route("{controller}/Index")]
        [HttpGet]
        public async Task<ActionResult> IssueThrBOM(int ID, string Mode, int YC, string REQNo = "", string ItemName = "", string PartCode = "", string WorkCenter = "", string DashboardType = "", string FromDate = "", string ToDate = "", string SearchBox = "")//, ILogger logger)
        {
            //_logger.LogInformation("\n \n ********** Page Gate Inward ********** \n \n " + IWebHostEnvironment.EnvironmentName.ToString() + "\n \n");
            TempData.Clear();
            var MainModel = new IssueThrBom();
            MainModel = await BindModel(MainModel);
            MainModel.CC = HttpContext.Session.GetString("Branch");
            MainModel.YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
            MainModel.FromDate = HttpContext.Session.GetString("FromDate");
            MainModel.ToDate = HttpContext.Session.GetString("ToDate");
            _MemoryCache.Remove("KeyIssThrBomGrid");
            _MemoryCache.Remove("KeyIssThrBomScannedGrid");
            _MemoryCache.Remove("KeyIssThrBomFGGrid");
            if (!string.IsNullOrEmpty(Mode) && ID > 0 && (Mode == "V" || Mode == "U"))
            {
                MainModel = await _IIssueThrBOM.GetViewByID(ID, YC).ConfigureAwait(false);
                MainModel.Mode = Mode;
                MainModel.ID = ID;
                MainModel = await BindModel(MainModel);

                //MainModel = await BindModel(MainModel).ConfigureAwait(false);
                MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                    SlidingExpiration = TimeSpan.FromMinutes(55),
                    Size = 1024,
                };
                _MemoryCache.Set("KeyIssThrBomGrid", MainModel.ItemDetailGrid, cacheEntryOptions);
                _MemoryCache.Set("KeyIssThrBomFGGrid", MainModel.FGItemDetailGrid, cacheEntryOptions);
            }
            else
            {
                // MainModel = await BindModel(MainModel);
            }

            if (Mode != "U")
            {
                MainModel.ActualEnteredBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                MainModel.ActualEnteredByName = HttpContext.Session.GetString("EmpName");
                MainModel.ActualEntrydate = DateTime.Now;
                MainModel.IssuedByEmpCode = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
                MainModel.IssuedByEmpName = HttpContext.Session.GetString("EmpName");
                MainModel.RecByEmpCode = Convert.ToInt32(HttpContext.Session.GetString("EmployeeList"));
            }
            else
            {
                MainModel.LastupdatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                MainModel.LastupdatedByName = HttpContext.Session.GetString("EmpName");
                MainModel.LastUpdationDate = DateTime.Now;
            }

            MainModel.FromDateBack = FromDate;
            MainModel.ToDateBack = ToDate;
            MainModel.REQNoBack = REQNo;
            MainModel.PartCodeBack = PartCode;
            MainModel.ItemNameBack = ItemName;
            MainModel.WorkCenterBack = WorkCenter;
            MainModel.DashboardTypeBack = DashboardType;
            MainModel.GlobalSearchBack = SearchBox;
            return View(MainModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("{controller}/Index")]
        public async Task<IActionResult> IssueThrBOM(IssueThrBom model)
        {
            try
            {
                var RMGrid = new DataTable(); // memoryGrid(down)
                var FGGrid = new DataTable(); // FGGrid(top)
                _MemoryCache.TryGetValue("KeyIssThrBomGrid", out List<IssueThrBomDetail> IssueGrid);
                _MemoryCache.TryGetValue("KeyIssThrBomFGGrid", out List<IssueThrBomFGData> IssueFGGrid);
                if (IssueGrid == null)
                {
                    ModelState.Clear();
                    ModelState.TryAddModelError("Issue Thr BOM", "Issue Thr BOM Grid Should Have At least 1 Item...!");

                    return View("IssueThrBom", model);
                }
                else
                {
                    var userID = Convert.ToInt32(HttpContext.Session.GetString("UID"));

                    model.CreatedBy = userID;
                    if (model.Mode == "U")
                        model.LastupdatedBy = userID;
                    model.Uid=userID;
                    RMGrid = GetRMDetailTable(IssueGrid);
                    FGGrid = GetFGDetailTable(IssueFGGrid);

                    var Result = await _IIssueThrBOM.SaveIssueThrBom(model, RMGrid, FGGrid);

                    if (Result != null)
                    {
                        if (Result.StatusText == "Success" && Result.StatusCode == HttpStatusCode.OK)
                        {
                            ViewBag.isSuccess = true;
                            TempData["200"] = "200";
                            _MemoryCache.Remove("KeyIssThrBomGrid");
                            //var MainModel = new IssueThrBom();
                            //MainModel.EmployeeList = await _IIssueThrBOM.GetEmployeeList();
                        }
                        if (Result.StatusText == "Updated" && Result.StatusCode == HttpStatusCode.Accepted)
                        {
                            ViewBag.isSuccess = true;
                            TempData["202"] = "202";
                            _MemoryCache.Remove("KeyIssThrBomGrid");
                            //var MainModel = new IssueThrBom();
                            //MainModel.EmployeeList = await _IIssueThrBOM.GetEmployeeList();
                        }
                        if (Result.StatusText == "Error" && Result.StatusCode == HttpStatusCode.InternalServerError)
                        {
                            ViewBag.isSuccess = false;
                            TempData["500"] = "500";
                            _logger.LogError("\n \n ********** LogError ********** \n " + JsonConvert.SerializeObject(Result) + "\n \n");
                            return View("Error", Result);
                        }
                    }
                    return RedirectToAction("PendingMaterialToIssueThrBOM", "PendingMaterialToIssueThrBOM");
                }
            }
            catch (Exception ex)
            {
                LogException<IssueThrBOMController>.WriteException(_logger, ex);

                var ResponseResult = new ResponseResult()
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    StatusText = "Error",
                    Result = ex
                };

                return View("Error", ResponseResult);
            }
        }

        public async Task<JsonResult> GetFormRights()
        {
            var userID = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
            var JSON = await _IIssueThrBOM.GetFormRights(userID);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        private async Task<IssueThrBom> BindModel(IssueThrBom model)
        {
            var oDataSet = new DataSet();
            var _List = new List<TextValue>();
            oDataSet = await _IIssueThrBOM.FillEmployee("BINDRecByEmployee");

            if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in oDataSet.Tables[0].Rows)
                {
                    _List.Add(new TextValue
                    {
                        Value = row["Emp_Id"].ToString(),
                        Text = row["EmpNameCode"].ToString()

                    });
                }
                var _EmployeeList = _List.DistinctBy(x => x.Value).ToList();
                model.EmployeeList = _EmployeeList;
                // _List = new List<TextValue>();
            }
            return model;
        }

        public IActionResult FillThrGridFromMemoryCache()
        {
            try
            {

                _MemoryCache.TryGetValue("KeyIssThrBom", out IList<IssueThrBomDetail> IssueThrBomDetailGrid);
                var MainModel = new IssueThrBom();
                var IssueGrid = new List<IssueThrBomDetail>();
                var SSGrid = new List<IssueThrBomDetail>();
                MainModel.FromDate = HttpContext.Session.GetString("FromDate");
                MainModel.ToDate = HttpContext.Session.GetString("ToDate");
                MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                    SlidingExpiration = TimeSpan.FromMinutes(55),
                    Size = 1024,
                };
                var seqNo = 1;
                if (IssueThrBomDetailGrid != null)
                {
                    for (int i = 0; i < IssueThrBomDetailGrid.Count; i++)
                    {


                        if (IssueThrBomDetailGrid[i] != null)
                        {
                            IssueThrBomDetailGrid[i].seqno = seqNo++;
                            SSGrid.AddRange(IssueGrid);
                            IssueGrid.Add(IssueThrBomDetailGrid[i]);

                            MainModel.ItemDetailGrid = IssueGrid;

                            _MemoryCache.Set("KeyIssThrBom", MainModel.ItemDetailGrid, cacheEntryOptions);
                        }
                    }
                }

                return PartialView("_IssueThrBOMMemoryGrid", MainModel);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<JsonResult> GetNewEntry()
        {
            int YC = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
            var JSON = await _IIssueThrBOM.GetNewEntry(YC);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillFGDataList(string Reqno, int ReqYC)
        {
            var JSON = await _IIssueThrBOM.FillFGDataList(Reqno, ReqYC);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }

        public async Task<JsonResult> FillProjectNo()
        {
            var JSON = await _IIssueThrBOM.FillProjectNo();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }

        public async Task<IActionResult> FillLotandTotalStock(int ItemCode, int StoreId, string TillDate, string BatchNo, string UniqBatchNo)
        {
            var JSON = await _IIssueThrBOM.FillLotandTotalStock(ItemCode, StoreId, TillDate, BatchNo, UniqBatchNo);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }

        public async Task<JsonResult> CheckStockBeforeSaving(int ItemCode, int StoreId, string TillDate, string BatchNo, string UniqBatchNo)
        {
            var JSON = await _IIssueThrBOM.CheckStockBeforeSaving(ItemCode, StoreId, TillDate, BatchNo, UniqBatchNo);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }

        public async Task<JsonResult> CheckRequisitionBeforeSaving(string ReqNo, int ReqyearCode, int ItemCode)
        {
            var JSON = await _IIssueThrBOM.CheckRequisitionBeforeSaving(ReqNo, ReqyearCode, ItemCode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetIsStockable(int ItemCode)
        {
            var JSON = await _IIssueThrBOM.GetIsStockable(ItemCode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<IActionResult> FillBatchUnique(int ItemCode, int YearCode, string StoreName, string BatchNo, string IssuedDate)
        {
            var FinStartDate = HttpContext.Session.GetString("FromDate");
            var JSON = await _IIssueThrBOM.FillBatchUnique(ItemCode, YearCode, StoreName, BatchNo, IssuedDate, FinStartDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetAllowBatch()
        {
            var JSON = await _IIssueThrBOM.GetAllowBatch();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public IActionResult AddtoIssueThrBomGrid(List<IssueThrBomDetail> model)
        {
            try
            {
                var MainModel = new IssueThrBom();
                var IssueThrBomGrid = new List<IssueThrBomDetail>();
                var IssueGrid = new List<IssueThrBomDetail>();
                var SSGrid = new List<IssueThrBomDetail>();
                MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                    SlidingExpiration = TimeSpan.FromMinutes(55),
                    Size = 1024,
                };
                var seqNo = 1;
                if (model != null)
                {
                    foreach (var item in model)
                    {

                        var isStockable = _IIssueThrBOM.GetIsStockable(item.ItemCode);
                        var stockable = isStockable.Result.Result.Rows[0].ItemArray[0];
                        _MemoryCache.TryGetValue("KeyIssThrBomGrid", out IList<IssueThrBomDetail> IssueThrBomDetailGrid);
                        if (item != null)
                        {
                            //if(item.LotStock < item.ReqQty)
                            //{
                            //    return StatusCode(203, "Stock can't be zero");
                            //}
                            if (IssueThrBomDetailGrid == null)
                            {
                                if (stockable == "Y")
                                {
                                    if (item.LotStock <= 0 || item.TotalStock <= 0)
                                    {
                                        return StatusCode(203, "Stock can't be zero");
                                    }
                                }
                                item.seqno += seqNo;
                                IssueGrid.Add(item);
                                seqNo++;
                            }
                            else
                            {
                                if (stockable == "Y")
                                {
                                    if (item.LotStock <= 0 || item.TotalStock <= 0)
                                    {
                                        return StatusCode(203, "Stock can't be zero");
                                    }
                                }
                                if (IssueThrBomDetailGrid.Where(x => x.ItemCode == item.ItemCode && x.BatchNo == item.BatchNo && x.uniqueBatchNo == item.uniqueBatchNo).Any())
                                {
                                    return StatusCode(207, "Duplicate");
                                }
                                else
                                {
                                    item.seqno = IssueThrBomDetailGrid.Count + 1;
                                    IssueGrid = IssueThrBomDetailGrid.Where(x => x != null).ToList();
                                    SSGrid.AddRange(IssueGrid);
                                    IssueGrid.Add(item);
                                }
                            }
                            MainModel.ItemDetailGrid = IssueGrid;

                            _MemoryCache.Set("KeyIssThrBomGrid", MainModel.ItemDetailGrid, cacheEntryOptions);
                        }
                    }
                }
                _MemoryCache.Remove("KeyIssThrBom");
                return PartialView("_IssueThrBomGrid", MainModel);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IActionResult AddtoIssueFGThrBomGrid(List<IssueThrBomFGData> model)
        {
            try
            {
                var MainModel = new IssueThrBom();
                var IssueThrBomGrid = new List<IssueThrBomFGData>();
                var IssueGrid = new List<IssueThrBomFGData>();
                var SSGrid = new List<IssueThrBomFGData>();
                MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                    SlidingExpiration = TimeSpan.FromMinutes(55),
                    Size = 1024,
                };
                var seqNo = 1;
                if (model != null)
                {
                    foreach (var item in model)
                    {
                        _MemoryCache.TryGetValue("KeyIssThrBomFGGrid", out IList<IssueThrBomFGData> IssueThrBomFGDetailGrid);
                        if (item != null)
                        {
                            //if(item.LotStock < item.ReqQty)
                            //{
                            //    return StatusCode(203, "Stock can't be zero");
                            //}
                            if (IssueThrBomFGDetailGrid == null)
                            {
                                item.Seqno += seqNo;
                                IssueGrid.Add(item);
                                seqNo++;
                            }
                            else
                            {
                                item.Seqno = IssueThrBomFGDetailGrid.Count + 1;
                                IssueGrid = IssueThrBomFGDetailGrid.Where(x => x != null).ToList();
                                SSGrid.AddRange(IssueGrid);
                                IssueGrid.Add(item);
                            }
                            MainModel.FGItemDetailGrid = IssueGrid;

                            _MemoryCache.Set("KeyIssThrBomFGGrid", MainModel.FGItemDetailGrid, cacheEntryOptions);
                        }
                    }
                }
                //_MemoryCache.Remove("KeyIssThrBom");
                return Json("done");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IActionResult> GetItemDetailFromUniqBatch(string UniqBatchNo, int YearCode, string TransDate, string ReqNo, int ReqYearCode, string ReqDate)
        {
            var MainModel = new IssueThrBom();
            try
            {
                ResponseResult StockData = new ResponseResult();
                var ItemDetailData = await _IIssueThrBOM.GetItemDetailFromUniqBatch(UniqBatchNo, YearCode, TransDate);
                if (ItemDetailData.Result != null)
                {
                    if (ItemDetailData.Result.Rows.Count != 0)
                    {
                        StockData = await _IIssueThrBOM.FillLotandTotalStock(Convert.ToInt32(ItemDetailData.Result.Rows[0].ItemArray[4]), 1, TransDate, ItemDetailData.Result.Rows[0].ItemArray[2], UniqBatchNo);
                    }
                    else
                    {
                        return StatusCode(203, "Invalid barcode, item do not exist in this requisition");

                    }
                }
                else
                {
                    return StatusCode(203, "Invalid barcode, item do not exist in this requisition");

                }
                ResponseResult ReqQty = await _IIssueThrBOM.GetReqQtyForScan(ReqNo, ReqYearCode, ReqDate, Convert.ToInt32(ItemDetailData.Result.Rows[0].ItemArray[4]));

                decimal ReqQuantity = 0;

                if (ReqQty.Result.Rows.Count != 0)
                {
                    ReqQuantity = Convert.ToDecimal(ReqQty.Result.Rows[0].ItemArray[0]);
                }
                else
                {
                    return StatusCode(203, "Invalid barcode this item " + ItemDetailData.Result.Rows[0].ItemArray[0] + " do not exist in this requisition");
                }
                //var t = StockData.
                //string JsonString = JsonConvert.SerializeObject(JSON);

                var ItemList = new List<IssueThrBomDetail>();

                var lotStock = Convert.ToDecimal(StockData.Result.Rows[0].ItemArray[0]);
                var totStock = Convert.ToDecimal(StockData.Result.Rows[0].ItemArray[1]);

                var stock = lotStock <= totStock ? lotStock : totStock;

                var issueQty = stock <= ReqQuantity ? stock : ReqQuantity;

                ItemList.Add(new IssueThrBomDetail
                {
                    ItemName = ItemDetailData.Result.Rows[0].ItemArray[0],
                    PartCode = ItemDetailData.Result.Rows[0].ItemArray[1],
                    ItemCode = Convert.ToInt32(ItemDetailData.Result.Rows[0].ItemArray[4]),
                    BatchNo = ItemDetailData.Result.Rows[0].ItemArray[2],
                    uniqueBatchNo = UniqBatchNo,
                    Unit = ItemDetailData.Result.Rows[0].ItemArray[3],
                    LotStock = lotStock,
                    TotalStock = totStock,
                    IssueQty = issueQty,
                    ReqQty = ReqQuantity
                });

                var model = ItemList;

                var IssueThrBomGrid = new List<IssueThrBomDetail>();
                var IssueGrid = new List<IssueThrBomDetail>();
                var SSGrid = new List<IssueThrBomDetail>();
                MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                    SlidingExpiration = TimeSpan.FromMinutes(55),
                    Size = 1024,
                };
                var seqNo = 1;
                if (model != null)
                {
                    foreach (var item in model)
                    {
                        _MemoryCache.TryGetValue("KeyIssThrBomScannedGrid", out IList<IssueThrBomDetail> IssueThrBomDetailGrid);
                        if (item != null)
                        {
                            //if(item.LotStock < item.ReqQty)
                            //{
                            //    return StatusCode(203, "Stock can't be zero");
                            //}
                            if (IssueThrBomDetailGrid == null)
                            {
                                if (item.LotStock <= 0 || item.TotalStock <= 0)
                                {
                                    return StatusCode(203, "Stock can't be zero");
                                }
                                item.seqno += seqNo;
                                IssueGrid.Add(item);
                                seqNo++;
                            }
                            else
                            {
                                if (IssueThrBomDetailGrid.Where(x => x.uniqueBatchNo == item.uniqueBatchNo).Any())
                                {
                                    return StatusCode(207, "Duplicate");
                                }
                                if (item.LotStock <= 0 || item.TotalStock <= 0)
                                {
                                    return StatusCode(203, "Stock can't be zero");
                                }
                                else
                                {
                                    item.seqno = IssueThrBomDetailGrid.Count + 1;
                                    IssueGrid = IssueThrBomDetailGrid.Where(x => x != null).ToList();
                                    SSGrid.AddRange(IssueGrid);
                                    IssueGrid.Add(item);
                                }
                            }
                            MainModel.ItemDetailGrid = IssueGrid;

                            _MemoryCache.Set("KeyIssThrBomScannedGrid", MainModel.ItemDetailGrid, cacheEntryOptions);
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return PartialView("_IssueByScanningThrGrid", MainModel);
        }

        public IActionResult DeleteFromMemoryGrid(int SeqNo)
        {
            var MainModel = new IssueThrBom();
            _MemoryCache.TryGetValue("KeyIssThrBom", out List<IssueThrBomDetail> IssueThrBomGrid);
            int Indx = Convert.ToInt32(SeqNo) - 1;

            if (IssueThrBomGrid != null && IssueThrBomGrid.Count > 0)
            {
                IssueThrBomGrid.RemoveAt(Convert.ToInt32(Indx));

                Indx = 0;

                foreach (var item in IssueThrBomGrid)
                {
                    Indx++;
                    item.seqno = Indx;
                }
                MainModel.ItemDetailGrid = IssueThrBomGrid;

                MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                    SlidingExpiration = TimeSpan.FromMinutes(55),
                    Size = 1024,
                };


                if (IssueThrBomGrid.Count == 0)
                {
                    _MemoryCache.Remove("KeyIssThrBom");
                }
                _MemoryCache.Set("KeyIssThrBom", MainModel.ItemDetailGrid, cacheEntryOptions);
            }
            return PartialView("_IssueThrBOMMemoryGrid", MainModel);
        }


        public IActionResult DeleteRowsWithNullBatchNo()
        {
            var MainModel = new IssueThrBom();

            if (_MemoryCache.TryGetValue("KeyIssThrBom", out List<IssueThrBomDetail> IssueThrBomGrid)
                && IssueThrBomGrid != null && IssueThrBomGrid.Count > 0)
            {
                // Remove all rows where batchno is null (or string.IsNullOrWhiteSpace if needed)
                IssueThrBomGrid = IssueThrBomGrid
                    .Where(x => !string.IsNullOrWhiteSpace(x.BatchNo))
                    .ToList();

                // Reassign sequence numbers
                int newSeq = 1;
                foreach (var item in IssueThrBomGrid)
                {
                    item.seqno = newSeq++;
                }

                MainModel.ItemDetailGrid = IssueThrBomGrid;

                if (IssueThrBomGrid.Count == 0)
                {
                    _MemoryCache.Remove("KeyIssThrBom");
                }
                else
                {
                    MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
                    {
                        AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                        SlidingExpiration = TimeSpan.FromMinutes(55),
                        Size = 1024,
                    };

                    _MemoryCache.Set("KeyIssThrBom", IssueThrBomGrid, cacheEntryOptions);
                }
            }

            return PartialView("_IssueThrBOMMemoryGrid", MainModel);
        }


        public IActionResult DeleteItemRow(int SeqNo)
        {
            var MainModel = new IssueThrBom();
            _MemoryCache.TryGetValue("KeyIssThrBomGrid", out List<IssueThrBomDetail> IssueThrBomGrid);
            int Indx = Convert.ToInt32(SeqNo) - 1;

            if (IssueThrBomGrid != null && IssueThrBomGrid.Count > 0)
            {
                IssueThrBomGrid.RemoveAt(Convert.ToInt32(Indx));

                Indx = 0;

                foreach (var item in IssueThrBomGrid)
                {
                    Indx++;
                    item.seqno = Indx;
                }
                MainModel.ItemDetailGrid = IssueThrBomGrid;

                MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                    SlidingExpiration = TimeSpan.FromMinutes(55),
                    Size = 1024,
                };
                if (IssueThrBomGrid.Count == 0)
                {
                    _MemoryCache.Remove("KeyIssThrBomGrid");
                }
                _MemoryCache.Set("KeyIssThrBomGrid", MainModel.ItemDetailGrid, cacheEntryOptions);
            }
            return PartialView("_IssueThrBomGrid", MainModel);
        }
        public IActionResult EditItemRow(int SeqNo)
        {
            _MemoryCache.TryGetValue("KeyIssThrBomGrid", out List<IssueThrBomDetail> IssueGrid);

            var SSGrid = IssueGrid.Where(x => x.seqno == SeqNo);
            string JsonString = JsonConvert.SerializeObject(SSGrid);
            return Json(JsonString);
        }

        public async Task<IActionResult> DeleteByID(int ID, int YC, string REQNo, string FGItemName, string FGPartCode, string FromDate, string ToDate, string IssueSlipNo, string DashboardType = "SUMM")
        {
            //var getData = _IIssueThrBOM.GetDataForDelete(ID, YC);

            //long[] ICArray = new long[getData.Result.Result.Rows.Count];
            //string[] batchNoArray = new string[getData.Result.Result.Rows.Count];
            //string[] uniqBatchArray = new string[getData.Result.Result.Rows.Count];

            //if (getData.Result.Result != null)
            //{
            //    for (int i = 0; i < getData.Result.Result.Rows.Count; i++)
            //    {
            //        ICArray[i] = getData.Result.Result.Rows[i].ItemArray[0];
            //        batchNoArray[i] = getData.Result.Result.Rows[i].ItemArray[1];
            //        uniqBatchArray[i] = getData.Result.Result.Rows[i].ItemArray[2];
            //        var checkLasTransDate = _IIssueThrBOM.CheckLastTransDate(ICArray[i], batchNoArray[i], uniqBatchArray[i]);
            //        if (checkLasTransDate.Result.Result.Rows[0].ItemArray[0] != "Successful")
            //        {
            //            ViewBag.isSuccess = true;
            //            TempData["423"] = "423";
            //            return RedirectToAction("Dashboard", new { FromDate = DateTime.Now.AddDays(-(DateTime.Now.Day - 1)), Todate = DateTime.Now, Flag = "True" });
            //        }
            //    }
            //}

            var Result = await _IIssueThrBOM.DeleteByID(ID, YC);

            if (Result.StatusText == "Success" || Result.StatusCode == HttpStatusCode.Gone)
            {
                ViewBag.isSuccess = true;
                TempData["410"] = "410";
            }
            else if (Result.StatusText != "Success" || Result.StatusCode == HttpStatusCode.Accepted)
            {
                ViewBag.isSuccess = true;
                TempData["423"] = "423";
            }
            else
            {
                ViewBag.isSuccess = false;
                TempData["500"] = "500";
            }
            DateTime fromDt = DateTime.ParseExact(FromDate, "dd/MM/yyyy", null);
            string formattedFromDate = fromDt.ToString("dd/MMM/yyyy 00:00:00");
            DateTime toDt = DateTime.ParseExact(ToDate, "dd/MM/yyyy", null);
            string formattedToDate = toDt.ToString("dd/MMM/yyyy 00:00:00");

            return RedirectToAction("Dashboard", new { FromDate = formattedFromDate, ToDate = formattedToDate, Flag = "False", REQNo = REQNo, FGItemName = FGItemName, FGPartCode = FGPartCode, IssueSlipNo = IssueSlipNo, DashboardType = DashboardType });
        }
        private static DataTable GetRMDetailTable(IList<IssueThrBomDetail> DetailList)
        {
            var MRGrid = new DataTable();

            MRGrid.Columns.Add("EntryId", typeof(int));
            MRGrid.Columns.Add("YearCode", typeof(int));
            MRGrid.Columns.Add("seqno", typeof(int));
            MRGrid.Columns.Add("FGItemCode", typeof(int));
            MRGrid.Columns.Add("RMItemCode", typeof(int));
            MRGrid.Columns.Add("ReqQty", typeof(decimal));
            MRGrid.Columns.Add("AltReqQty", typeof(decimal));
            MRGrid.Columns.Add("StoreId", typeof(int));
            MRGrid.Columns.Add("BatchNo", typeof(string));
            MRGrid.Columns.Add("uniqueBatchNo", typeof(string));
            MRGrid.Columns.Add("IssueQty", typeof(decimal));
            MRGrid.Columns.Add("AltIssueQty", typeof(decimal));
            MRGrid.Columns.Add("PendQty", typeof(decimal));
            MRGrid.Columns.Add("Unit", typeof(string));
            MRGrid.Columns.Add("AltUnit", typeof(string));
            MRGrid.Columns.Add("LotStock", typeof(decimal));
            MRGrid.Columns.Add("TotalStock", typeof(decimal));
            MRGrid.Columns.Add("AltQty", typeof(decimal));
            MRGrid.Columns.Add("Rate", typeof(decimal));
            MRGrid.Columns.Add("WCId", typeof(int));
            MRGrid.Columns.Add("STDPkg", typeof(decimal));
            MRGrid.Columns.Add("IssuedAlternateItem", typeof(string));
            MRGrid.Columns.Add("OriginalitemCode", typeof(int));
            MRGrid.Columns.Add("AltItemCode", typeof(int));
            MRGrid.Columns.Add("Remark", typeof(string));
            MRGrid.Columns.Add("CostCenterId", typeof(int));
            MRGrid.Columns.Add("ItemSize", typeof(string));
            MRGrid.Columns.Add("ItemColor", typeof(string));
            MRGrid.Columns.Add("WIPStock", typeof(float));

            foreach (var Item in DetailList)
            {
                //DateTime ReqDate = new DateTime();
                //ReqDate = ParseDate(Item.ReqDate);
                if (Item.AltUnit == "null")
                    Item.AltUnit = "";
                if (Item.uniqueBatchNo == "null")
                    Item.uniqueBatchNo = "";
                if (Item.BatchNo == "null")
                    Item.BatchNo = "";
                MRGrid.Rows.Add(
                    new object[]
                    {
                    1,
                    2023,
                    Item.seqno,
                    Item.ItemCode,//FGItemCode -> change
                    Item.ItemCode,//RMItemCode
                    Item.ReqQty.ToString("F6"),
                    Item.ReqQty.ToString("F6"), // altrecqty
                    Item.StoreId,
                    Item.BatchNo ?? "",
                    Item.uniqueBatchNo ?? "",
                    Item.IssueQty.ToString("F6"),
                    Item.IssueQty.ToString("F6"), // altissueqty
                    Item.PendQty, // pendqty
                    Item.Unit,
                    Item.AltUnit ?? "",
                    Item.LotStock,
                    Item.TotalStock,
                    Item.AltQty,
                    Item.Rate,
                    Item.WCId,
                    Item.StdPacking ?? 0,
                    Item.IssuedAlternateItem ?? "N",//issuedalternateitem
                    Item.OriginalItemCode ?? Item.ItemCode,//originalitemcode
                    Item.AltItemCode,
                    Item.Remark,
                    Item.CostCenterId,
                    Item.ItemSize,
                    Item.ItemColor,
                    Item.WipStock// WIPStock -> change
                    });
            }
            MRGrid.Dispose();
            return MRGrid;
        }
        private static DataTable GetFGDetailTable(IList<IssueThrBomFGData> DetailList)
        {
            var MRGrid = new DataTable();

            MRGrid.Columns.Add("EntryId", typeof(int));
            MRGrid.Columns.Add("YearCode", typeof(int));
            MRGrid.Columns.Add("EntryDate", typeof(DateTime));
            MRGrid.Columns.Add("IssueSlipNo", typeof(string));
            MRGrid.Columns.Add("IssueDate", typeof(DateTime));
            MRGrid.Columns.Add("WONO", typeof(string));
            MRGrid.Columns.Add("WOYearCode", typeof(int));
            MRGrid.Columns.Add("WOdate", typeof(DateTime));
            MRGrid.Columns.Add("FGItemCode", typeof(int));
            MRGrid.Columns.Add("Unit", typeof(string));
            MRGrid.Columns.Add("FGQty", typeof(decimal));
            MRGrid.Columns.Add("BOMNO", typeof(int));
            MRGrid.Columns.Add("BOMDate", typeof(DateTime));
            MRGrid.Columns.Add("FGStockInStore", typeof(float));
            MRGrid.Columns.Add("IssueFromStoreID", typeof(int));
            MRGrid.Columns.Add("Remark", typeof(string));
            MRGrid.Columns.Add("WCID", typeof(int));

            foreach (var Item in DetailList)
            {
                MRGrid.Rows.Add(
                    new object[]
                    {
                    1,
                    2023,
                    DateTime.Today,//entrydate
                    "",//issueslipno
                    DateTime.Today,//issuedate
                    Item.WONO ?? "",
                    Item.WOYearCode,
                    DateTime.Today,//WODATE
                    Item.FGItemCode,
                    Item.Unit ?? "",
                    Item.FGQty,
                    Item.BOMNO,
                    Item.BOMDate,
                    0,//FGStockINstore - > change
                    0,//Issueintstore -> change
                    Item.Remark ?? "",
                    Item.WCID
                    });
            }
            MRGrid.Dispose();
            return MRGrid;
        }
        //public async Task<IActionResult> GetSearchData(string DashBoardSearchType, string FromDate, string ToDate, string IssueSlipNo, string ReqNo, string WCName, string ItemName, string PartCode)
        //{
        //    var model = new IssueThrBomDashboard();
        //    model = await _IIssueThrBOM.GetDashboardData(DashboardType, FromDate, ToDate, IssueSlipNo, ReqNo, WCName, ItemName, PartCode);
        //    return PartialView("_IssueWithBomDashboardGrid", model);
        //}

        //public async Task<IActionResult> GetDashboardData(string FromDate, string ToDate, string DashboardType, string IssueSlipNo, string ReqNo)
        //{
        //    var model = new IssueThrBomDashboard();
        //    model = await _IIssueThrBOM.GetDashboardData(FromDate, ToDate,DashboardType,IssueSlipNo,ReqNo);
        //    return PartialView("_IssueWithBomDashboardGrid", model);
        //}
        public async Task<JsonResult> DisplayBomDetail(int ItemCode, float WOQty, int BomRevNo)
        {
            var JSON = await _IIssueThrBOM.DisplayBomDetail(ItemCode, WOQty, BomRevNo);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }

        public async Task<IActionResult> Dashboard(string FromDate, string Todate, string Flag = "", string DashboardType = "SUMM", string IssueSlipNo = "", string ReqNo = "", string FGPartCode = "", string FGItemName = "")
        {
            try
            {
                _MemoryCache.Remove("KeyIssThrBomGrid");
                var model = new IssueThrBomDashboard();
                var Result = await _IIssueThrBOM.GetDashboardData(FromDate, Todate, DashboardType, IssueSlipNo, ReqNo).ConfigureAwait(true);

                if (Result != null)
                {
                    var _List = new List<TextValue>();
                    DataSet DS = Result.Result;
                    if (DS != null)
                    {
                        var DT = DS.Tables[0].DefaultView.ToTable(false, "IssueSlipNo", "IssueDate", "ReqNo", "ReqDate", "ReqYearCode", "WONO", "WODate", "EntryId", "YearCode", "jobCardNo", "JobcardDate", "Remark", "ActENterdByEmpName", "ActENterdByEmpCode", "ActualEntryDate", "UpdatedEmpName", "UpdatedByEmpcode", "RecByEmpCode", "IssuedByEmpCode");
                        model.IssueThrBOMDashboard = CommonFunc.DataTableToList<IssueThrBomMainDashboard>(DT, "IssueThrSUMMDashboard");
                    }
                }
                //model.FromDate1 = FromDate;
                //model.ToDate1 = Todate;
                //model.ReqNo = REQNo;
                ////model.WorkCenterDescription = WCName;
                //model.FGPartCode = PartCode;
                //model.FGItemName = ItemName;

                if (Flag != "True")
                {
                    model.FromDate1 = FromDate;
                    model.ToDate1 = Todate;
                    model.ReqNo = ReqNo;
                    model.FGItemName = FGItemName;
                    model.FGPartCode = FGPartCode;
                    model.IssueSlipNo = IssueSlipNo;
                    model.DashboardType = DashboardType;
                }

                return View(model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IActionResult> FGDetailData(string FromDate, string Todate, string Flag = "", string DashboardType = "FGSUMM", string IssueSlipNo = "", string ReqNo = "", string FGPartCode = "", string FGItemName = "")
        {
            //model.Mode = "Search";
            var model = new IssueThrBomMainDashboard();
            model = await _IIssueThrBOM.FGDetailData(FromDate, Todate, Flag, DashboardType, IssueSlipNo, ReqNo, FGPartCode, FGItemName);
            model.Mode = "FGDetail";
            return PartialView("_IssueWithBomDashboardGrid", model);
        }
        public async Task<IActionResult> RMDetailData(string FromDate, string Todate, string WCName, string PartCode, string ItemName, string Flag = "", string DashboardType = "RMDetail", string IssueSlipNo = "", string ReqNo = "", string GlobalSearch = "", string FGPartCode = "", string FGItemName = "")
        {
            //model.Mode = "Search";
            var model = new IssueThrBomMainDashboard();
            model = await _IIssueThrBOM.RMDetailData(FromDate, Todate, WCName, PartCode, ItemName, Flag, DashboardType, IssueSlipNo, ReqNo, GlobalSearch, FGPartCode, FGItemName);
            model.Mode = "RMDetail";
            return PartialView("_IssueWithBomDashboardGrid", model);
        }
        public async Task<IActionResult> SummaryData(string FromDate, string Todate, string Flag = "", string DashboardType = "SUMM", string IssueSlipNo = "", string ReqNo = "")
        {
            //model.Mode = "Search";
            var model = new IssueThrBomMainDashboard();
            model = await _IIssueThrBOM.SummaryData(FromDate, Todate, Flag, DashboardType, IssueSlipNo, ReqNo);
            model.Mode = "Summary";
            return PartialView("_IssueWithBomDashboardGrid", model);
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
    }
}
