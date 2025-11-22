using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.Wordprocessing;
using eTactWeb.Data.Common;
using eTactWeb.Data.DAL;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using FastReport;
using FastReport.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
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
        private readonly IWebHostEnvironment _IWebHostEnvironment;
        private readonly IConfiguration _iconfiguration;
        private readonly IMemoryCache _MemoryCache;
        private readonly ConnectionStringService _connectionStringService;
        public IssueThrBOMController(ILogger<IssueThrBOMController> logger, IConfiguration iconfiguration, IDataLogic iDataLogic, IIssueThrBOM IIssueWOBOM, IWebHostEnvironment iWebHostEnvironment, IMemoryCache iMemoryCache, ConnectionStringService connectionStringService)
        {
            _logger = logger;
            _IDataLogic = iDataLogic;
            _IIssueThrBOM = IIssueWOBOM;
            _IWebHostEnvironment = iWebHostEnvironment;
            _iconfiguration = iconfiguration;
            _MemoryCache = iMemoryCache;
            _connectionStringService = connectionStringService;
        }

        [Route("{controller}/Index")]
        public async Task<IActionResult> IssueThrBOM()
        {
            ViewData["Title"] = "Issue Through BOM Details";
            TempData.Clear();
            HttpContext.Session.Remove("KeyIssThrBomGrid");
            HttpContext.Session.Remove("KeyIssThrBomScannedGrid");
            HttpContext.Session.Remove("KeyIssThrBomFGGrid");
            HttpContext.Session.Remove("AllowBatchChange");
            var MainModel = new IssueThrBom();
            MainModel.FromDate = HttpContext.Session.GetString("FromDate");
            MainModel.ToDate = HttpContext.Session.GetString("ToDate");
            MainModel.YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
            MainModel.CC = HttpContext.Session.GetString("Branch");
            MainModel.ActualEnteredBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
            MainModel.ActualEnteredByName = HttpContext.Session.GetString("EmpName");
            MainModel.ActualEntrydate = DateTime.Now;

            string serializedGrid = JsonConvert.SerializeObject(MainModel);
            HttpContext.Session.SetString("KeyIssThrBomGrid", serializedGrid);
            return View(MainModel);
        }
        public IActionResult PrintReport(int EntryId = 0, int YearCode = 0)
        {
            string my_connection_string;
            string contentRootPath = _IWebHostEnvironment.ContentRootPath;
            string webRootPath = _IWebHostEnvironment.WebRootPath;
            var webReport = new WebReport();
    
            webReport.Report.Load(webRootPath + "\\IssueThrBom.frx"); // default report
            my_connection_string = _connectionStringService.GetConnectionString();
            //my_connection_string = _iconfiguration.GetConnectionString("eTactDB");
            webReport.Report.Dictionary.Connections[0].ConnectionString = my_connection_string;
            webReport.Report.Dictionary.Connections[0].ConnectionStringExpression = "";
           
            webReport.Report.SetParameterValue("entryparam", EntryId);
            webReport.Report.SetParameterValue("yearparam", YearCode);
            webReport.Report.SetParameterValue("MyParameter", my_connection_string);
            webReport.Report.Refresh();
            return View(webReport);

           
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
            HttpContext.Session.Remove("KeyIssThrBomGrid");
            HttpContext.Session.Remove("KeyIssThrBomScannedGrid");
            HttpContext.Session.Remove("KeyIssThrBomFGGrid");
            HttpContext.Session.Remove("AllowBatchChange");
            if (!string.IsNullOrEmpty(Mode) && ID > 0 && (Mode == "V" || Mode == "U"))
            {
                MainModel = await _IIssueThrBOM.GetViewByID(ID, YC).ConfigureAwait(false);
                MainModel.Mode = Mode;
                MainModel.ID = ID;
                MainModel = await BindModel(MainModel);

                string serializedItemGrid = JsonConvert.SerializeObject(MainModel.ItemDetailGrid);
                HttpContext.Session.SetString("KeyIssThrBomGrid", serializedItemGrid);
                string serializedGrid = JsonConvert.SerializeObject(MainModel.FGItemDetailGrid);
                HttpContext.Session.SetString("KeyIssThrBomFGGrid", serializedGrid);
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
        public async Task<JsonResult> PassForCloseReq()
        {
            var JSON = await _IIssueThrBOM.PassForCloseReq();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("{controller}/Index")]
        public async Task<IActionResult> IssueThrBOM(IssueThrBom model, string ShouldPrint)
        {
            try
            {
                var RMGrid = new DataTable(); // memoryGrid(down)
                var FGGrid = new DataTable(); // FGGrid(top)
                string modelJson = HttpContext.Session.GetString("KeyIssThrBomGrid");
                List<IssueThrBomDetail> IssueGrid = new List<IssueThrBomDetail>();
                if (!string.IsNullOrEmpty(modelJson))
                {
                    IssueGrid = JsonConvert.DeserializeObject<List<IssueThrBomDetail>>(modelJson);
                }
                string modelFGJson = HttpContext.Session.GetString("KeyIssThrBomFGGrid");
                List<IssueThrBomFGData> IssueFGGrid = new List<IssueThrBomFGData>();
                if (!string.IsNullOrEmpty(modelFGJson))
                {
                    IssueFGGrid = JsonConvert.DeserializeObject<List<IssueThrBomFGData>>(modelFGJson);
                }
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
                            //if (ShouldPrint == "true")
                            //{
                            //    return RedirectToAction("PrintReport", new { EntryId = model.EntryId, YearCode = model.YearCode });
                            //}
                            if (ShouldPrint == "true")
                            {
                                return Json(new
                                {
                                    status = "Success",
                                    entryId = model.EntryId,
                                    yearCode = model.YearCode
                                });
                            }

                            HttpContext.Session.Remove("KeyIssThrBomGrid");
                        }
                        if (Result.StatusText == "Updated" && Result.StatusCode == HttpStatusCode.Accepted)
                        {
                            ViewBag.isSuccess = true;
                            TempData["202"] = "202";
                            HttpContext.Session.Remove("KeyIssThrBomGrid"); 
                        }
                        if (Result.StatusText == "Error" && Result.StatusCode == HttpStatusCode.InternalServerError)
                        {
                            ViewBag.isSuccess = false;
                            TempData["500"] = "500";
                            _logger.LogError("\n \n ********** LogError ********** \n " + JsonConvert.SerializeObject(Result) + "\n \n");
                            return View("Error", Result);
                        }
                    }
                    //return RedirectToAction("PendingMaterialToIssueThrBOM", "PendingMaterialToIssueThrBOM");
                    return Json(new { status = "Success" });
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
                string modelJson = HttpContext.Session.GetString("KeyIssThrBom");
                List<IssueThrBomDetail> IssueThrBomDetailGrid = new List<IssueThrBomDetail>();
                if (!string.IsNullOrEmpty(modelJson))
                {
                    IssueThrBomDetailGrid = JsonConvert.DeserializeObject<List<IssueThrBomDetail>>(modelJson);
                }
                
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

                            string serializedGrid = JsonConvert.SerializeObject(MainModel.ItemDetailGrid);
                            HttpContext.Session.SetString("KeyIssThrBom", serializedGrid);
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

        public async Task<JsonResult> GetReqByName(string reqno, int yearcode)
        {
            var JSON = await _IIssueThrBOM.GetReqByName(reqno, yearcode);
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

        public async Task<IActionResult> GETWIPSTOCKBATCHWISE(int ItemCode, int WCID, int LAST_YEAR, string BatchNo, string UniqBatchNo)
        {
            var JSON = await _IIssueThrBOM.GETWIPSTOCKBATCHWISE(ItemCode, WCID, LAST_YEAR, BatchNo, UniqBatchNo);
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

                var seqNo = 1;
                if (model != null)
                {
                    foreach (var item in model)
                    {

                        var isStockable = _IIssueThrBOM.GetIsStockable(item.ItemCode);
                        var stockable = isStockable.Result.Result.Rows[0].ItemArray[0];
                        string modelJson = HttpContext.Session.GetString("KeyIssThrBomGrid");
                        List<IssueThrBomDetail> IssueThrBomDetailGrid = new List<IssueThrBomDetail>();
                        if (!string.IsNullOrEmpty(modelJson))
                        {
                            IssueThrBomDetailGrid = JsonConvert.DeserializeObject<List<IssueThrBomDetail>>(modelJson);
                        }
                        
                        if (item != null)
                        {
                            if (IssueThrBomDetailGrid == null)
                            {
                                if (stockable == "Y")
                                {
                                    if (item.LotStock <= 0 || item.TotalStock <= 0)
                                    {
                                        return StatusCode(203, $"Stock can't be zero for PartCode: {item.PartCode}");
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
                                        return StatusCode(203, $"Stock can't be zero for PartCode: {item.PartCode}");
                                    }
                                }

                                var duplicateItem = IssueThrBomDetailGrid.FirstOrDefault(x => x.ItemName == item.ItemName && x.BatchNo == item.BatchNo && x.uniqueBatchNo == item.uniqueBatchNo);

                                if (duplicateItem != null)
                                {
                                    var message = $"Duplicate found: ItemName = {duplicateItem.ItemName}, " +
                                     $"BatchNo = {duplicateItem.BatchNo}, " +
                                     $"uniqueBatchNo = {duplicateItem.uniqueBatchNo}";
                                    return StatusCode(207, message);
                                }
                                else
                                {
                                    item.seqno = IssueThrBomDetailGrid.Count + 1;
                                    IssueGrid = IssueThrBomDetailGrid.Where(x => x != null).ToList();
                                    SSGrid.AddRange(IssueGrid);
                                    IssueGrid.Add(item);
                                }

                                //if (IssueThrBomDetailGrid.Where(x => x.ItemCode == item.ItemCode && x.BatchNo == item.BatchNo && x.uniqueBatchNo == item.uniqueBatchNo).Any())
                                //{
                                //    return StatusCode(207, "Duplicate");
                                //}
                                //else
                                //{
                                //    item.seqno = IssueThrBomDetailGrid.Count + 1;
                                //    IssueGrid = IssueThrBomDetailGrid.Where(x => x != null).ToList();
                                //    SSGrid.AddRange(IssueGrid);
                                //    IssueGrid.Add(item);
                                //}
                            }
                            MainModel.ItemDetailGrid = IssueGrid;

                            string serializedGrid = JsonConvert.SerializeObject(MainModel.ItemDetailGrid);
                            HttpContext.Session.SetString("KeyIssThrBomGrid", serializedGrid);
                        }
                    }
                }
                HttpContext.Session.Remove("KeyIssThrBom");
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
                        string modelJson = HttpContext.Session.GetString("KeyIssThrBomFGGrid");
                        List<IssueThrBomFGData> IssueThrBomFGDetailGrid = new List<IssueThrBomFGData>();
                        if (!string.IsNullOrEmpty(modelJson))
                        {
                            IssueThrBomFGDetailGrid = JsonConvert.DeserializeObject<List<IssueThrBomFGData>>(modelJson);
                        }

                        if (item != null)
                        {
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

                            string serializedGrid = JsonConvert.SerializeObject(MainModel.FGItemDetailGrid);
                            HttpContext.Session.SetString("KeyIssThrBomFGGrid", serializedGrid);
                        }
                    }
                }
                return Json("done");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<JsonResult> GetIssueScanFeature()
        {
            var JSON = await _IIssueThrBOM.GetIssueScanFeature();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        [HttpPost]
        public IActionResult SetAllowBatchChangeFlag(bool allow)
        {
            HttpContext.Session.SetString("AllowBatchChange", allow ? "Y" : "N");
            return Ok();
        }

        public async Task<IActionResult> GetItemDetailFromUniqBatch(string UniqBatchNo, int YearCode, string TransDate, string ReqNo, int ReqYearCode, string ReqDate)
        {
            var MainModel = new IssueThrBom();
            try
            {
                ResponseResult StockData = new ResponseResult();
                var ItemDetailData = await _IIssueThrBOM.GetItemDetailFromUniqBatch(UniqBatchNo, YearCode, TransDate);

                ResponseResult ReqQty = await _IIssueThrBOM.GetReqQtyForScan(ReqNo, ReqYearCode, ReqDate, Convert.ToInt32(ItemDetailData.Result.Rows[0].ItemArray[4]));
                ResponseResult ReqStoreId = await _IIssueThrBOM.GetStoreIdReqForScan(ReqNo, ReqYearCode, ReqDate, Convert.ToInt32(ItemDetailData.Result.Rows[0].ItemArray[4]));

                decimal ReqQuantity = 0;
                string modelJson1 = HttpContext.Session.GetString("KeyIssThrBom");
                List<IssueThrBom> sessionModels = JsonConvert.DeserializeObject<List<IssueThrBom>>(modelJson1);

                
                int dbItemCode = Convert.ToInt32(ItemDetailData.Result.Rows[0].ItemArray[4]);

                var matchedItem = sessionModels.FirstOrDefault(m => m.ItemCode == dbItemCode);

                if (matchedItem == null)
                {
                    return StatusCode(203, "Invalid barcode this item " + ItemDetailData.Result.Rows[0].ItemArray[0] + " do not exist in this requisition");
                }

                //string modelJson1 = HttpContext.Session.GetString("KeyIssThrBom");
                //IssueThrBom sessionModel = JsonConvert.DeserializeObject<IssueThrBom>(modelJson1);

                //if (ReqQty.Result.Rows.Count != 0)
                //{
                //    ReqQuantity = Convert.ToDecimal(ReqQty.Result.Rows[0].ItemArray[0]);
                //}
                //else
                //{
                //    return StatusCode(203, "Invalid barcode this item " + ItemDetailData.Result.Rows[0].ItemArray[0] + " do not exist in this requisition");
                //}

                if (ItemDetailData.Result != null)
                {
                    if (ItemDetailData.Result.Rows.Count != 0)
                    {

                        StockData = await _IIssueThrBOM.FillLotandTotalStock(Convert.ToInt32(ItemDetailData.Result.Rows[0].ItemArray[4]), Convert.ToInt32(matchedItem.StoreId), TransDate, ItemDetailData.Result.Rows[0].ItemArray[2], UniqBatchNo);
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
               
                var lotStock = Convert.ToDecimal(StockData.Result.Rows[0].ItemArray[0]);
                var totStock = Convert.ToDecimal(StockData.Result.Rows[0].ItemArray[1]);

                var stock = lotStock <= totStock ? lotStock : totStock;

                var issueQty = stock <= ReqQuantity ? stock : ReqQuantity;
                
                var JSON = await _IIssueThrBOM.ShowDetail(ReqDate, ReqDate, ReqNo, YearCode, Convert.ToInt32(ItemDetailData.Result.Rows[0].ItemArray[4]), "", 0, 0, ReqYearCode, ReqDate, "", "", Convert.ToInt32(matchedItem.StoreId));

                var ItemList = new List<IssueThrBomDetail>();

                if (JSON?.Result != null && JSON.Result.Tables.Count > 0)
                {
                    var table = JSON.Result.Tables[0];
                    foreach (DataRow row in table.Rows)
                    {
                        var item = new IssueThrBomDetail
                        {
                            ItemName = ItemDetailData.Result.Rows[0].ItemArray[0],
                    PartCode = ItemDetailData.Result.Rows[0].ItemArray[1],
                    ItemCode = Convert.ToInt32(ItemDetailData.Result.Rows[0].ItemArray[4]),
                    BatchNo = ItemDetailData.Result.Rows[0].ItemArray[2],
                    uniqueBatchNo = UniqBatchNo,
                    Unit = ItemDetailData.Result.Rows[0].ItemArray[3],
                    LotStock = lotStock,
                    TotalStock = totStock,
                    IssueQty = row["IssueQty"] != DBNull.Value ? Convert.ToDecimal(row["IssueQty"]) : 0,
                    ReqQty = row["ReqQty"] != DBNull.Value ? Convert.ToDecimal(row["ReqQty"]) : 0,
                            StdPacking = row["StdPacking"] != DBNull.Value ? Convert.ToSingle(row["StdPacking"]) : 0,
                    StoreName = row["StoreName"]?.ToString(),
                    AltQty = row["AltQty"] != DBNull.Value ? Convert.ToDecimal(row["AltQty"]) : 0,
                    AltUnit = row["AltUnit"]?.ToString(),
                    Rate = row["Rate"] != DBNull.Value ? Convert.ToDecimal(row["Rate"]) : 0,
                    Remark = row["Remark"]?.ToString(),
                    AltItemCode = row["AltItemCode"] != DBNull.Value ? Convert.ToInt32(row["AltItemCode"]) : 0,
                    CostCenterId = row["CostCenterId"] != DBNull.Value ? Convert.ToInt32(row["CostCenterId"]) : 0,
                    ItemSize = row["ItemSize"]?.ToString(),
                    ItemColor = row["ItemColor"]?.ToString(),
                    StoreId = row["storeid"] != DBNull.Value ? Convert.ToInt32(row["storeid"]) : 0,
                    WCId = row["WCId"] != DBNull.Value ? Convert.ToInt32(row["WCId"]) : 0,
                    WorkCenter = row["WorkCenter"]?.ToString(),
                    TransactionDate = row["TransactionDate"]?.ToString(),
                            WipStock = row["WIPStock"] != DBNull.Value? Convert.ToSingle(Convert.ToDecimal(row["WIPStock"])): 0


                        };

                        ItemList.Add(item);
                    }
                }
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
                        string modelJson = HttpContext.Session.GetString("KeyIssThrBomScannedGrid");
                        List<IssueThrBomDetail> IssueThrBomDetailGrid = new List<IssueThrBomDetail>();
                        if (!string.IsNullOrEmpty(modelJson))
                        {
                            IssueThrBomDetailGrid = JsonConvert.DeserializeObject<List<IssueThrBomDetail>>(modelJson);
                        }
                        if (item != null)
                        {
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
                                decimal alreadyIssuedQty = IssueThrBomDetailGrid
                                                .Where(x => x.ItemCode == item.ItemCode)
                                                .Sum(x => x.IssueQty);

                                decimal requiredQty = item.ReqQty; // ReqQty from requisition
                                decimal newQty = item.IssueQty;    // Qty for this batch

                                
                                var allowBatchChange = HttpContext.Session.GetString("AllowBatchChange");
                                if (allowBatchChange != "Y")
                                {
                                    if (JSON?.Result != null && JSON.Result.Tables.Count > 0)
                                    {
                                        foreach (DataRow row in JSON.Result.Tables[0].Rows)
                                        {
                                            if (row["uniqueBatchNo"] != DBNull.Value &&
                                                row["uniqueBatchNo"].ToString() != UniqBatchNo)
                                            {
                                                return StatusCode(209, "can not add new stock");
                                            }
                                        }
                                    }
                                }

                                if (IssueThrBomDetailGrid.Where(x => x.uniqueBatchNo == item.uniqueBatchNo).Any())
                                {
                                    return StatusCode(207, "Duplicate");
                                }
                                if (alreadyIssuedQty + newQty > requiredQty)
                                {
                                    return StatusCode(208, "Cannot scan another batch.Req. Qty already fulfilled for item.");
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

                            string serializedGrid = JsonConvert.SerializeObject(MainModel.ItemDetailGrid);
                            HttpContext.Session.SetString("KeyIssThrBomScannedGrid", serializedGrid);
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return PartialView("_IssueThrByScanningGrid", MainModel);
        }
        [HttpPost]
        public IActionResult DeleteFromZeroStockMemoryGrid(bool deleteZeroStockOnly, int? seqNo = null)
        {
            var MainModel = new IssueThrBom();
            string modelJson = HttpContext.Session.GetString("KeyIssThrBom");
            List<IssueThrBomDetail> IssueThrBomGrid = new List<IssueThrBomDetail>();

            if (!string.IsNullOrEmpty(modelJson))
            {
                IssueThrBomGrid = JsonConvert.DeserializeObject<List<IssueThrBomDetail>>(modelJson);
            }

            if (deleteZeroStockOnly)
            {
                var deletedPartCodes = new List<string>();
                IssueThrBomGrid.RemoveAll(x =>
                {
                    bool toDelete = (x.BatchNo =="" || x.BatchNo==null);

                    if (toDelete)
                        deletedPartCodes.Add(x.PartCode);
                    return toDelete;
                });

                ViewBag.DeletedPartCodes = string.Join(", ", deletedPartCodes);
            }
            else if (seqNo != null)
            {
                var itemToRemove = IssueThrBomGrid.FirstOrDefault(x => x.seqno == seqNo);
                if (itemToRemove != null)
                {
                    IssueThrBomGrid.Remove(itemToRemove);
                }
            }

            int newSeq = 1;
            foreach (var item in IssueThrBomGrid)
            {
                item.seqno = newSeq++;
            }

            
            MainModel.ItemDetailGrid = IssueThrBomGrid;


            if (IssueThrBomGrid.Count == 0)
            {
                HttpContext.Session.Remove("KeyIssThrBom");
            }
            else
            {
                string updatedJson = JsonConvert.SerializeObject(IssueThrBomGrid);
                HttpContext.Session.SetString("KeyIssThrBom", updatedJson);
            }

            return PartialView("_IssueThrBOMMemoryGrid", MainModel);
        }

        public IActionResult DeleteFromMemoryGrid(int SeqNo)
        {
            var MainModel = new IssueThrBom();
            string modelJson = HttpContext.Session.GetString("KeyIssThrBom");
            List<IssueThrBomDetail> IssueThrBomGrid = new List<IssueThrBomDetail>();
            if (!string.IsNullOrEmpty(modelJson))
            {
                IssueThrBomGrid = JsonConvert.DeserializeObject<List<IssueThrBomDetail>>(modelJson);
            }
            
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
                    HttpContext.Session.Remove("KeyIssThrBom");
                }
                string serializedGrid = JsonConvert.SerializeObject(MainModel.ItemDetailGrid);
                HttpContext.Session.SetString("KeyIssThrBom", serializedGrid);
            }
            return PartialView("_IssueThrBOMMemoryGrid", MainModel);
        }

        public IActionResult DeleteScannedItemRow(int SeqNo)
        {
            var MainModel = new IssueThrBom();
            string modelJson = HttpContext.Session.GetString("KeyIssThrBomScannedGrid");
            var IssueThrBomGrid = new List<IssueThrBomDetail>();

            if (!string.IsNullOrEmpty(modelJson))
            {
                IssueThrBomGrid = JsonConvert.DeserializeObject<List<IssueThrBomDetail>>(modelJson);
            }

            if (IssueThrBomGrid != null && IssueThrBomGrid.Count > 0)
            {
                if (SeqNo > 0 && SeqNo <= IssueThrBomGrid.Count)
                {
                    IssueThrBomGrid.RemoveAt(SeqNo - 1);

                    // resequence
                    int newSeq = 1;
                    foreach (var item in IssueThrBomGrid)
                    {
                        item.seqno = newSeq++;
                    }
                }

                MainModel.ItemDetailGrid = IssueThrBomGrid;

                if (IssueThrBomGrid.Count == 0)
                {
                    HttpContext.Session.Remove("KeyIssThrBomScannedGrid");
                }
                else
                {
                    string serializedGrid = JsonConvert.SerializeObject(IssueThrBomGrid);
                    HttpContext.Session.SetString("KeyIssThrBomScannedGrid", serializedGrid);
                }
            }

            return PartialView("_IssueThrByScanningGrid", MainModel);
        }


        public IActionResult DeleteRowsWithNullBatchNo()
        {
            var MainModel = new IssueThrBom();
            string modelJson = HttpContext.Session.GetString("KeyIssThrBom");
            List<IssueThrBomDetail> IssueThrBomGrid = new List<IssueThrBomDetail>();
            if (!string.IsNullOrEmpty(modelJson))
            {
                IssueThrBomGrid = JsonConvert.DeserializeObject<List<IssueThrBomDetail>>(modelJson);
            }

            if ( IssueThrBomGrid != null && IssueThrBomGrid.Count > 0)
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
                    HttpContext.Session.Remove("KeyIssThrBom");
                }
                else
                {
                    string serializedGrid = JsonConvert.SerializeObject(IssueThrBomGrid);
                    HttpContext.Session.SetString("KeyIssThrBom", serializedGrid);
                }
            }

            return PartialView("_IssueThrBOMMemoryGrid", MainModel);
        }


        public IActionResult DeleteItemRow(int SeqNo)
        {
            var MainModel = new IssueThrBom();
            string modelJson = HttpContext.Session.GetString("KeyIssThrBomGrid");
            List<IssueThrBomDetail> IssueThrBomGrid = new List<IssueThrBomDetail>();
            if (!string.IsNullOrEmpty(modelJson))
            {
                IssueThrBomGrid = JsonConvert.DeserializeObject<List<IssueThrBomDetail>>(modelJson);
            }
            
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
                    HttpContext.Session.Remove("KeyIssThrBomGrid");
                }
                string serializedGrid = JsonConvert.SerializeObject(MainModel.ItemDetailGrid);
                HttpContext.Session.SetString("KeyIssThrBomGrid", serializedGrid);
            }
            return PartialView("_IssueThrBomGrid", MainModel);
        }
        public IActionResult EditItemRow(int SeqNo)
        {
            string modelJson = HttpContext.Session.GetString("KeyIssThrBomGrid");
            List<IssueThrBomDetail> IssueGrid = new List<IssueThrBomDetail>();
            if (!string.IsNullOrEmpty(modelJson))
            {
                IssueGrid = JsonConvert.DeserializeObject<List<IssueThrBomDetail>>(modelJson);
            }

            var SSGrid = IssueGrid.Where(x => x.seqno == SeqNo);
            string JsonString = JsonConvert.SerializeObject(SSGrid);
            return Json(JsonString);
        }

        public async Task<IActionResult> DeleteByID(int ID, int YC, string REQNo, string FGItemName, string FGPartCode, string FromDate, string ToDate, string IssueSlipNo, string DashboardType = "SUMM")
        {
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
            string fromDt =CommonFunc.ParseFormattedDate (FromDate);
            
            string toDt = CommonFunc.ParseFormattedDate(ToDate);
            

            return RedirectToAction("Dashboard", new { FromDate = "", ToDate = "", Flag = "", REQNo = REQNo, FGItemName = FGItemName, FGPartCode = FGPartCode, IssueSlipNo = IssueSlipNo, DashboardType = DashboardType });
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
        public async Task<JsonResult> DisplayBomDetail(int ItemCode, float WOQty, int BomRevNo)
        {
            var JSON = await _IIssueThrBOM.DisplayBomDetail(ItemCode, WOQty, BomRevNo);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }

        public async Task<IActionResult> Dashboard(string FromDate="", string Todate="", string Flag = "", string DashboardType = "SUMM", string IssueSlipNo = "", string ReqNo = "", string FGPartCode = "", string FGItemName = "")
        {
            try
            {
                HttpContext.Session.Remove("KeyIssThrBomGrid");
                var model = new IssueThrBomDashboard();
                var Result = await _IIssueThrBOM.GetDashboardData(FromDate, Todate, DashboardType, IssueSlipNo, ReqNo).ConfigureAwait(true);

                //if (Result != null)
                //{
                //    var _List = new List<TextValue>();
                //    DataSet DS = Result.Result;
                //    if (DS != null)
                //    {
                //        var DT = DS.Tables[0].DefaultView.ToTable(false, "IssueSlipNo", "IssueDate", "ReqNo", "ReqDate", "ReqYearCode", "WONO", "WODate", "EntryId", "YearCode", "jobCardNo", "JobcardDate", "Remark", "ActENterdByEmpName", "ActENterdByEmpCode", "ActualEntryDate", "UpdatedEmpName", "UpdatedByEmpcode", "RecByEmpCode", "IssuedByEmpCode");
                //        model.IssueThrBOMDashboard = CommonFunc.DataTableToList<IssueThrBomMainDashboard>(DT, "IssueThrSUMMDashboard");
                //    }
                //}

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

        public async Task<IActionResult> FGDetailData(string FromDate, string Todate, string Flag = "", string DashboardType = "FGSUMM", string IssueSlipNo = "", string ReqNo = "", string FGPartCode = "", string FGItemName = "", int pageNumber = 1, int pageSize = 50, string SearchBox = "")
        {
            //model.Mode = "Search";
            var model = new IssueThrBomMainDashboard();
            model = await _IIssueThrBOM.FGDetailData(FromDate, Todate, Flag, DashboardType, IssueSlipNo, ReqNo, FGPartCode, FGItemName);
            model.Mode = "FGSUMM";
            var modelList = model?.IssueThrBOMDashboard ?? new List<IssueThrBomMainDashboard>();


            if (string.IsNullOrWhiteSpace(SearchBox))
            {
                model.TotalRecords = modelList.Count();
                model.PageNumber = pageNumber;
                model.PageSize = pageSize;
                model.IssueThrBOMDashboard = modelList
                .Skip((pageNumber - 1) * pageSize)
                   .Take(pageSize)
                   .ToList();
            }
            else
            {
                List<IssueThrBomMainDashboard> filteredResults;
                if (string.IsNullOrWhiteSpace(SearchBox))
                {
                    filteredResults = modelList.ToList();
                }
                else
                {
                    filteredResults = modelList
                        .Where(i => i.GetType().GetProperties()
                            .Where(p => p.PropertyType == typeof(string))
                            .Select(p => p.GetValue(i)?.ToString())
                            .Any(value => !string.IsNullOrEmpty(value) &&
                                          value.Contains(SearchBox, StringComparison.OrdinalIgnoreCase)))
                        .ToList();


                    if (filteredResults.Count == 0)
                    {
                        filteredResults = modelList.ToList();
                    }
                }

                model.TotalRecords = filteredResults.Count;
                model.IssueThrBOMDashboard = filteredResults.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
                model.PageNumber = pageNumber;
                model.PageSize = pageSize;
            }
            MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                SlidingExpiration = TimeSpan.FromMinutes(55),
                Size = 1024,
            };

            _MemoryCache.Set("KeyIssThrBOMList_FGSUMM", modelList, cacheEntryOptions);
            return PartialView("_IssueWithBomDashboardGrid", model);
        }
        public async Task<IActionResult> RMDetailData(string FromDate, string Todate, string WCName, string PartCode, string ItemName, string Flag = "", string DashboardType = "RMDetail", string IssueSlipNo = "", string ReqNo = "", string GlobalSearch = "", string FGPartCode = "", string FGItemName = "", int pageNumber = 1, int pageSize = 50, string SearchBox = "")
        {
            //model.Mode = "Search";
            var model = new IssueThrBomMainDashboard();
            model = await _IIssueThrBOM.RMDetailData(FromDate, Todate, WCName, PartCode, ItemName, Flag, DashboardType, IssueSlipNo, ReqNo, GlobalSearch, FGPartCode, FGItemName);
            model.Mode = "RMDETAIL";
            var modelList = model?.IssueThrBOMDashboard ?? new List<IssueThrBomMainDashboard>();


            if (string.IsNullOrWhiteSpace(SearchBox))
            {
                model.TotalRecords = modelList.Count();
                model.PageNumber = pageNumber;
                model.PageSize = pageSize;
                model.IssueThrBOMDashboard = modelList
                .Skip((pageNumber - 1) * pageSize)
                   .Take(pageSize)
                   .ToList();
            }
            else
            {
                List<IssueThrBomMainDashboard> filteredResults;
                if (string.IsNullOrWhiteSpace(SearchBox))
                {
                    filteredResults = modelList.ToList();
                }
                else
                {
                    filteredResults = modelList
                        .Where(i => i.GetType().GetProperties()
                            .Where(p => p.PropertyType == typeof(string))
                            .Select(p => p.GetValue(i)?.ToString())
                            .Any(value => !string.IsNullOrEmpty(value) &&
                                          value.Contains(SearchBox, StringComparison.OrdinalIgnoreCase)))
                        .ToList();


                    if (filteredResults.Count == 0)
                    {
                        filteredResults = modelList.ToList();
                    }
                }

                model.TotalRecords = filteredResults.Count;
                model.IssueThrBOMDashboard = filteredResults.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
                model.PageNumber = pageNumber;
                model.PageSize = pageSize;
            }
            MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                SlidingExpiration = TimeSpan.FromMinutes(55),
                Size = 1024,
            };

            _MemoryCache.Set("KeyIssThrBOMList_RMDETAIL", modelList, cacheEntryOptions);
            return PartialView("_IssueWithBomDashboardGrid", model);
        }
        public async Task<IActionResult> SummaryData(string FromDate, string Todate, string Flag = "", string DashboardType = "SUMM", string IssueSlipNo = "", string ReqNo = "", int pageNumber = 1, int pageSize = 50, string SearchBox = "")
        {
            //model.Mode = "Search";
            var model = new IssueThrBomMainDashboard();
            model = await _IIssueThrBOM.SummaryData(FromDate, Todate, Flag, DashboardType, IssueSlipNo, ReqNo);
            model.Mode = "SUMM";
            var modelList = model?.IssueThrBOMDashboard ?? new List<IssueThrBomMainDashboard>();


            if (string.IsNullOrWhiteSpace(SearchBox))
            {
                model.TotalRecords = modelList.Count();
                model.PageNumber = pageNumber;
                model.PageSize = pageSize;
                model.IssueThrBOMDashboard = modelList
                .Skip((pageNumber - 1) * pageSize)
                   .Take(pageSize)
                   .ToList();
            }
            else
            {
                List<IssueThrBomMainDashboard> filteredResults;
                if (string.IsNullOrWhiteSpace(SearchBox))
                {
                    filteredResults = modelList.ToList();
                }
                else
                {
                    filteredResults = modelList
                        .Where(i => i.GetType().GetProperties()
                            .Where(p => p.PropertyType == typeof(string))
                            .Select(p => p.GetValue(i)?.ToString())
                            .Any(value => !string.IsNullOrEmpty(value) &&
                                          value.Contains(SearchBox, StringComparison.OrdinalIgnoreCase)))
                        .ToList();


                    if (filteredResults.Count == 0)
                    {
                        filteredResults = modelList.ToList();
                    }
                }

                model.TotalRecords = filteredResults.Count;
                model.IssueThrBOMDashboard = filteredResults.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
                model.PageNumber = pageNumber;
                model.PageSize = pageSize;
            }
            MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                SlidingExpiration = TimeSpan.FromMinutes(55),
                Size = 1024,
            };

            _MemoryCache.Set("KeyIssThrBOMList_SUMM", modelList, cacheEntryOptions);
            return PartialView("_IssueWithBomDashboardGrid", model);
        }
        [HttpGet]
        public IActionResult GlobalSearch(string searchString, string dashboardType = "Summary", int pageNumber = 1, int pageSize = 50)
        {
            IssueThrBomMainDashboard model = new IssueThrBomMainDashboard();
            if (string.IsNullOrWhiteSpace(searchString))
            {
                return PartialView("_IssueWithBomDashboardGrid", new List<IssueThrBomMainDashboard>());
            }
            string cacheKey = $"KeyIssThrBOMList_{dashboardType}";
            if (!_MemoryCache.TryGetValue(cacheKey, out IList<IssueThrBomMainDashboard> IssueThrBOMDashboard) || IssueThrBOMDashboard == null)
            {
                return PartialView("_IssueWithBomDashboardGrid", new List<IssueThrBomMainDashboard>());
            }

            List<IssueThrBomMainDashboard> filteredResults;

            if (string.IsNullOrWhiteSpace(searchString))
            {
                filteredResults = IssueThrBOMDashboard.ToList();
            }
            else
            {
                filteredResults = IssueThrBOMDashboard
                    .Where(i => i.GetType().GetProperties()
                        .Where(p => p.PropertyType == typeof(string))
                        .Select(p => p.GetValue(i)?.ToString())
                        .Any(value => !string.IsNullOrEmpty(value) &&
                                      value.Contains(searchString, StringComparison.OrdinalIgnoreCase)))
                    .ToList();


                if (filteredResults.Count == 0)
                {
                    filteredResults = IssueThrBOMDashboard.ToList();
                }
            }

            model.TotalRecords = filteredResults.Count;
            model.IssueThrBOMDashboard = filteredResults.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            model.PageNumber = pageNumber;
            model.PageSize = pageSize;

            return PartialView("_IssueWithBomDashboardGrid", model);
        }
        public async Task<JsonResult> GetServerDate()
        {
            try
            {
                DateTime time = DateTime.Now;
                string isoDate = time.ToString("yyyy-MM-dd"); // ISO format
                return Json(isoDate);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected Exception: {ex.Message}");
                return Json(new { error = "An unexpected error occurred: " + ex.Message });
            }
        }

        public async Task<JsonResult> ChkStockBeforeSaving(string ReqNo, int ReqYearCode, int EntryId, int YearCode)
        {
            var DTItemGrid = new DataTable();
            string modelJson = HttpContext.Session.GetString("KeyIssThrBomGrid");
            List<IssueThrBomDetail> IssueGrid = new List<IssueThrBomDetail>();
            if (!string.IsNullOrEmpty(modelJson))
            {
                IssueGrid = JsonConvert.DeserializeObject<List<IssueThrBomDetail>>(modelJson);
            }
            //_MemoryCache.TryGetValue("KeyTransferFromWorkCenterGrid", out List<TransferFromWorkCenterDetail> TransferFromWorkCenterDetail);
            DTItemGrid = GetRMDetailTable(IssueGrid);
            var ChechedData = await _IIssueThrBOM.ChkStockBeforeSaving( ReqNo,  ReqYearCode,  EntryId,  YearCode,  DTItemGrid);
            if (ChechedData.StatusCode == HttpStatusCode.OK && ChechedData.StatusText == "Success")
            {
                DataTable dt = ChechedData.Result;

                List<string> errorMessages = new List<string>();

                foreach (DataRow row in dt.Rows)
                {
                    string itemName = row["Item_Name"].ToString();
                    string PartCode = row["PartCode"].ToString();
                    //decimal availableQty = Convert.ToDecimal(row["CalWIPStock"]);

                    string error = $"{itemName} + {PartCode} already inserted";
                    errorMessages.Add(error);
                }

                return Json(new
                {
                    success = false,
                    errors = errorMessages
                });
            }
            return Json(new
            {
                success = true,
                message = "No errors found."
            });
        }




        //public async Task<JsonResult> GetServerDate()
        //{
        //    try
        //    {
        //        DateTime time = DateTime.Now;
        //        string format = "MMM ddd d HH:mm yyyy";
        //        string formattedDate = time.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);

        //        var dt = time.ToString(format);
        //        return Json(formattedDate);

        //    }
        //    catch (HttpRequestException ex)
        //    {
        //        // Log the exception for debugging purposes
        //        Console.WriteLine($"HttpRequestException: {ex.Message}");
        //        return Json(new { error = "Failed to fetch server date and time: " + ex.Message });
        //    }
        //    catch (Exception ex)
        //    {
        //        // Log any other unexpected exceptions
        //        Console.WriteLine($"Unexpected Exception: {ex.Message}");
        //        return Json(new { error = "An unexpected error occurred: " + ex.Message });
        //    }
        //}
    }
}
