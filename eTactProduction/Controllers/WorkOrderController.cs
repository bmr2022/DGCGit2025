using eTactWeb.Data.Common;
using eTactWeb.Services.Interface;
using FastReport.Export.Html;
using FastReport.Export.Image;
using FastReport.Web;
using FastReport;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Security.Cryptography.X509Certificates;
using static eTactWeb.Data.Common.CommonFunc;
using static eTactWeb.DOM.Models.Common;
using static eTactWeb.DOM.Models.JobWorkReceiveModel;
using eTactWeb.DOM.Models;
using System.Data;
using System.Net;
using System.Globalization;

namespace eTactWeb.Controllers
{
    public class WorkOrderController : Controller
    {
        private readonly IDataLogic _IDataLogic;
        public IWorkOrder _IworkOrder { get; }
        private readonly ILogger<WorkOrderController> _logger;
        public IWebHostEnvironment IWebHostEnvironment { get; }

        public WorkOrderController(ILogger<WorkOrderController> logger, IDataLogic iDataLogic, IWorkOrder iWorkOrder, EncryptDecrypt encryptDecrypt, IWebHostEnvironment iWebHostEnvironment)
        {
            _logger = logger;
            _IDataLogic = iDataLogic;
            _IworkOrder = iWorkOrder;
            IWebHostEnvironment = iWebHostEnvironment;
        }

        [HttpGet]
        [Route("{controller}/Index")]
        public async Task<IActionResult> WorkOrder()
        {
            WorkOrderModel model = new WorkOrderModel();
            if (string.IsNullOrEmpty(model.Mode) && model.ID == 0)
            {
                
                model.Mode = "INSERT";
            }
            ViewData["Title"] = "WorkOrder Details";
            TempData.Clear();
            HttpContext.Session.Remove("KeyWorkOrderGrid");
            var MainModel = new RoutingModel();

            if (model.Mode != "U")
            {
                model.Uid = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                model.ActualEntryBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                model.ActualEntryByEmpName = HttpContext.Session.GetString("EmpName");
            }
            model.FinFromDate = HttpContext.Session.GetString("FromDate");
            model.FinToDate = HttpContext.Session.GetString("ToDate");
            model.YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
            model.CC = HttpContext.Session.GetString("Branch");
            string serializedGrid = JsonConvert.SerializeObject(model);
            HttpContext.Session.SetString("KeyWorkOrderGrid", serializedGrid);
            return View(model);
        }

        public async Task<JsonResult> GetFormRights()
        {
            var userID = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
            var JSON = await _IworkOrder.GetFormRights(userID);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("{controller}/Index")]
        public async Task<IActionResult> WorkOrder(WorkOrderModel model)
        {
            try
            {
                var WorkOrderDetail = new DataTable();

                string modelJson = HttpContext.Session.GetString("KeyWorkOrderGrid");
                List<WorkOrderDetail> WODetailGrid = new List<WorkOrderDetail>();
                if (!string.IsNullOrEmpty(modelJson))
                {
                    WODetailGrid = JsonConvert.DeserializeObject<List<WorkOrderDetail>>(modelJson);
                }
                if (WODetailGrid == null)
                {
                    ModelState.Clear();
                    ModelState.TryAddModelError("WorkOrderDetailGrid", "WorkOrder Grid Should Have Atleast 1 Item...!");
                    return View("WorkOrder", model);
                }
                else
                {
                    model.ActualEntryBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                    WorkOrderDetail = GetDetailTable(WODetailGrid);
                    if (model.Mode == "U")
                    {
                        model.LastUpdateByEmpName = HttpContext.Session.GetString("EmpName");
                        model.LastUpdateBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                    }
                    var Result = await _IworkOrder.SaveWorkOrder(model, WorkOrderDetail);

                    if (Result != null)
                    {
                        if (Result.StatusText == "Success" && Result.StatusCode == HttpStatusCode.OK)
                        {
                            ViewBag.isSuccess = true;
                            TempData["200"] = "200";
                        }
                        if (Result.StatusText == "Updated" && Result.StatusCode == HttpStatusCode.Accepted)
                        {
                            ViewBag.isSuccess = true;
                            TempData["202"] = "202";
                        }

                        if (Result.StatusText == "Error" && Result.StatusCode == HttpStatusCode.InternalServerError)
                        {
                            ViewBag.isSuccess = false;
                            var input = "";
                            if (Result != null)
                            {
                                input = Result.Result.ToString();
                                int index = input.IndexOf("#ERROR_MESSAGE");

                                if (index != -1)
                                {
                                    string errorMessage = input.Substring(index + "#ERROR_MESSAGE :".Length).Trim();
                                    TempData["ErrorMessage"] = errorMessage;
                                }
                                else
                                {
                                    TempData["500"] = "500";
                                }
                            }
                            else
                            {
                                TempData["500"] = "500";
                            }


                            _logger.LogError("\n \n ********** LogError ********** \n " + JsonConvert.SerializeObject(Result) + "\n \n");
                            //model.IsError = "true";
                            //return View("Error", Result);
                        }
                    }
                    string woData = HttpContext.Session.GetString("KeyWorkOrderGrid");
                    List<WorkOrderDetail> WODetail = new List<WorkOrderDetail>();
                    if (!string.IsNullOrEmpty(woData))
                    {
                        WODetail = JsonConvert.DeserializeObject<List<WorkOrderDetail>>(woData);
                    }
                    model.WorkDetailGrid = WODetail;
                    ModelState.Clear();
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                LogException<WorkOrderController>.WriteException(_logger, ex);

                var ResponseResult = new ResponseResult()
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    StatusText = "Error",
                    Result = ex
                };

                return View("Error", ResponseResult);
            }
        }

        private static DataTable GetDetailTable(IList<WorkOrderDetail> DetailList)
        {
            var JWGrid = new DataTable();

            JWGrid.Columns.Add("Entryid", typeof(int));
            JWGrid.Columns.Add("YearCode", typeof(int));
            JWGrid.Columns.Add("Entrydate", typeof(string));
            JWGrid.Columns.Add("Accountcode", typeof(int));
            JWGrid.Columns.Add("SONO", typeof(string));
            JWGrid.Columns.Add("CustomerOrderNo", typeof(string));
            JWGrid.Columns.Add("SOYearCode", typeof(int));
            JWGrid.Columns.Add("SODATE", typeof(string));
            JWGrid.Columns.Add("SchNo", typeof(string));
            JWGrid.Columns.Add("SchYearcode", typeof(int));
            JWGrid.Columns.Add("SCHDATE", typeof(string));
            JWGrid.Columns.Add("Itemcode", typeof(int));
            JWGrid.Columns.Add("COLOR", typeof(string));
            JWGrid.Columns.Add("OrderQty", typeof(float));
            JWGrid.Columns.Add("PendRouteSheetQTy", typeof(float));
            JWGrid.Columns.Add("PendProdQty", typeof(float));
            JWGrid.Columns.Add("WOQty", typeof(float));
            JWGrid.Columns.Add("FGStock", typeof(float));
            JWGrid.Columns.Add("WCID", typeof(int));
            JWGrid.Columns.Add("WIPStock", typeof(float));
            JWGrid.Columns.Add("drawingNo", typeof(string));
            JWGrid.Columns.Add("ProdInst1", typeof(string));
            JWGrid.Columns.Add("ProdInst2", typeof(string));
            JWGrid.Columns.Add("SOInstruction", typeof(string));
            JWGrid.Columns.Add("PkgInstruction", typeof(string));
            JWGrid.Columns.Add("PendingRouteForSheet", typeof(string));
            JWGrid.Columns.Add("RouteSheetNo", typeof(string));
            JWGrid.Columns.Add("RouteSheetYearCode", typeof(int));
            JWGrid.Columns.Add("RouteSheetDate", typeof(string));
            JWGrid.Columns.Add("RouteSheetEntryNo", typeof(int));
            JWGrid.Columns.Add("WovrevNo", typeof(string));
            JWGrid.Columns.Add("WovrevDate", typeof(string));
            JWGrid.Columns.Add("PrevWOQty", typeof(float));
            JWGrid.Columns.Add("ItemActive", typeof(string));
            JWGrid.Columns.Add("FGStoreId", typeof(int));
            JWGrid.Columns.Add("Bomno", typeof(int));
            JWGrid.Columns.Add("BomName", typeof(string));
            JWGrid.Columns.Add("BomeffectiveDate", typeof(string));
            JWGrid.Columns.Add("MainBomSubBom", typeof(string));
            JWGrid.Columns.Add("MachineGroup", typeof(int));
            JWGrid.Columns.Add("PrefMachineId1", typeof(int));
            JWGrid.Columns.Add("PrefMachineId2", typeof(int));
            JWGrid.Columns.Add("PrefMachineId3", typeof(int));
            JWGrid.Columns.Add("ApproxStartDate", typeof(string));
            JWGrid.Columns.Add("ApproxEndDate", typeof(string));                                                            
            JWGrid.Columns.Add("StoreId", typeof(int));
            JWGrid.Columns.Add("OrderType", typeof(string));
            JWGrid.Columns.Add("OrderWEF", typeof(string));
            JWGrid.Columns.Add("AmendNo", typeof(int));
            JWGrid.Columns.Add("AmendEffDate", typeof(string));
            JWGrid.Columns.Add("Unit", typeof(string));
            JWGrid.Columns.Add("AltUnit", typeof(string));
            JWGrid.Columns.Add("AltOrderQty", typeof(float));
            JWGrid.Columns.Add("SOCloseDate", typeof(string));
            JWGrid.Columns.Add("SchEffTillDate", typeof(string));
            JWGrid.Columns.Add("StoreStock", typeof(float));
            JWGrid.Columns.Add("AltQty", typeof(float));                                    
            JWGrid.Columns.Add("ItemDescription", typeof(string));
            JWGrid.Columns.Add("SOPendQty", typeof(float));
            JWGrid.Columns.Add("WOPendQty", typeof(float));
            JWGrid.Columns.Add("ProdQty", typeof(float));

            foreach (var Item in DetailList)
            {
                //string SODt = Item.SODATE;
                //DateTime inputDate = DateTime.Parse(SODt);
                //string formattedDate = inputDate.ToString("yyyy/MM/dd");

                JWGrid.Rows.Add(
                    new object[]
                    {1,2023,
                       ParseDate(Item.SODATE),
                    Item.Accountcode,
                    Item.SONO,
                    Item.CustomerOrderNo,
                    Item.SOYearCode,
                    Item.SODATE == null ? string.Empty : ParseDate(Item.SODATE),
                    Item.SchNo ?? "",
                    Item.SchYearcode,
                    Item.SCHDATE == null ? string.Empty : ParseDate(Item.SCHDATE),
                    Item.Itemcode,
                    Item.COLOR ?? "",
                    Item.OrderQty,
                    Item.PendRoutSheetQTy,
                    Item.PendProdQty,
                    Item.WOQty,
                    Item.FGStock,
                    Item.WIPStoreId, //WCID
                    Item.WIPStock,
                    Item.drawingNo ?? "",
                    Item.ProdInst1 ?? "",
                    Item.ProdInst2 ?? "",
                    Item.SOInstruction ?? "",
                    Item.PkgInstruction ?? "",
                    Item.PendingRouteForSheet ?? "",
                    Item.RouteSheetNo ?? "",
                    Item.RouteSheetYearCode,
                    Item.RouteSheetDate == null ? string.Empty : ParseDate(Item.RouteSheetDate),
                    Item.RouteSheetEntryNo,
                    Item.WorevNo ?? "",
                    Item.WORevDate == null ? string.Empty : ParseDate(Item.WORevDate),
                    Item.PrevWOQty,
                    "Y",
                    Item.FGStoreId,
                    Item.Bomno,
                    Item.BomName ?? "",
                    Item.BomEffectiveDate == null ? string.Empty : ParseDate(Item.BomEffectiveDate),
                    Item.MainBomSubBom ?? "",
                    Item.MachineGroupId,
                    Item.PrefMachineId1,
                    Item.PrefMachineId2,
                    Item.PrefMachineId3,
                    Item.ApproxStartDate == null ? string.Empty : ParseDate(Item.ApproxStartDate),
                    Item.ApproxEndDate == null ? string.Empty : ParseDate(Item.ApproxEndDate),
                    Item.WIPStoreId, //StoreId
                    Item.OrderType ?? "",
                    Item.OrderWEF == null ? string.Empty : ParseDate(Item.OrderWEF),
                    Item.AmendNo,
                    Item.AmendEffDate == null ? string.Empty : ParseDate(Item.AmendEffDate),
                    Item.Unit ?? "",
                    Item.AltUnit ?? "",
                    Item.AltOrderQty,
                    Item.SOCloseDate ?? "",
                    Item.SchEffTillDate == null ? string.Empty : ParseDate(Item.SchEffTillDate),
                    Item.StoreStock,
                    Item.AltQty,
                    Item.ItemDescription ?? "",
                    Item.PendSOQty,
                    Item.PendWOQty,
                    Item.ProdQty,
                    });
            }
            JWGrid.Dispose();
            return JWGrid;
        }
        public IActionResult ClearGrid()
        {
            HttpContext.Session.Remove("KeyWorkOrderGrid");
            var MainModel = new WorkOrderModel();
            return PartialView("_WorkOrderGrid", MainModel);
        }

        public async Task<JsonResult> EditItemRows(int SeqNo)
        {
            var MainModel = new WorkOrderModel();
            string modelJson = HttpContext.Session.GetString("KeyWorkOrderGrid");
            List<WorkOrderDetail> GridDetail = new List<WorkOrderDetail>();
            if (!string.IsNullOrEmpty(modelJson))
            {
                GridDetail = JsonConvert.DeserializeObject<List<WorkOrderDetail>>(modelJson);
            }
            var WOGrid = GridDetail.Where(x => x.SeqNo == SeqNo);
            string JsonString = JsonConvert.SerializeObject(WOGrid);
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

        public static string ParseDate(string dateString)
        {
            if (string.IsNullOrEmpty(dateString))
            {
                return string.Empty;
            }

            if (DateTime.TryParseExact(dateString, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate))
            {
                return parsedDate.ToString("yyyy/MM/dd");
            }
            else
            {
                if (DateTime.TryParse(dateString, out DateTime nonFormattedDate))
                {
                    return nonFormattedDate.ToString("yyyy/MM/dd");
                }

                return string.Empty;
            }
        }

        public async Task<IActionResult> WorkOrderDashboard(string CC="",string FromDate = "",string ToDatee = "",string WONO = "",string SONO = "",string SchNo = "",string AccountName = "",string PartCode = "",string ItemName= "",string SummaryDetail = "",string Flag = "True")
        {
            try
            {
                HttpContext.Session.Remove("KeyWorkOrderGrid");
                var model = new WorkOrderMainDashboard();
                var FromDt = HttpContext.Session.GetString("FromDate");
                model.FromDate = Convert.ToDateTime(FromDt).ToString("dd/MM/yyyy");
                DateTime ToDate = DateTime.Today;
                model.ToDate = ToDate.ToString();
                var Result = await _IworkOrder.GetDashboardData(model.FromDate, model.ToDate).ConfigureAwait(true);
                if (Result != null)
                {
                    var _List = new List<TextValue>();
                    DataSet DS = Result.Result;
                    if (DS != null)
                    {
                        var DT = DS.Tables[0].DefaultView.ToTable(true, "Entryid", "Yearcode", "WONO", "WODate", "EffectiveFrom",
                            "EffectiveTill", "WoStataus", "RemarkForProduction", "approved", "CloseWo", "WorevNo", "WORevDate");
                        model.WorkOrderGrid = CommonFunc.DataTableToList<WorkOrderGridDashboard>(DT, "WorkOrderTable");
                        if (Flag != "True")
                        {
                            model.CC = CC;
                            model.FromDate = FromDate;
                            model.ToDate = ToDatee;
                            model.WONO = WONO;
                            model.SONO = SONO;
                            model.SchNo = SchNo;
                            model.AccountName = AccountName;
                            model.PartCode = PartCode;
                            model.ItemName = ItemName;
                            model.SummaryDetail = SummaryDetail;
                        }
                    }
                }
                return View(model);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public async Task<JsonResult> GetSaleOrderData(int YearCode, string WODate,string EffFrom,string EffTill)
        {
            var JSON = await _IworkOrder.GetSaleOrderData("DisplaySALEORDERDETAIL", "SP_WorkOrder", YearCode, WODate,EffFrom,EffTill);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);    
        }
        public async Task<JsonResult> GetBomNo(int ItemCode)
        {
            var JSON = await _IworkOrder.GetBomNo(ItemCode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetBomName(int BomNo,int FinishedItemCode)
        {
            var JSON = await _IworkOrder.GetBomName(BomNo, FinishedItemCode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetEffDate(int BomNo,int FinishedItemCode)
        {
            var JSON = await _IworkOrder.GetEffDate(BomNo, FinishedItemCode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetMachineGroupName()
        {
            var JSON = await _IworkOrder.GetMachineGroupName();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetMachineName(int MachGroupId)
        {
            var JSON = await _IworkOrder.GetMachineName(MachGroupId);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }

        public async Task<IActionResult> GetSearchData(string SummaryDetail, string WONO, string CC, string SONO, string SchNo, string AccountName, string PartCode, string ItemName, string FromDate, string ToDate)
        {
            //model.Mode = "Search";
            var model = new WorkOrderGridDashboard();
            model = await _IworkOrder.GetDashboardData(SummaryDetail, WONO, CC, SONO, SchNo, AccountName, PartCode, ItemName, FromDate, ToDate);
            return PartialView("_WorkOrderDashboardGrid", model);
        }

        public IActionResult PrintReport(int EntryId = 0, int YearCode = 0, string Type = "")
        {
            string contentRootPath = IWebHostEnvironment.ContentRootPath;
            string webRootPath = IWebHostEnvironment.WebRootPath;
            //string frx = Path.Combine(_env.ContentRootPath, "reports", value.file);
            var webReport = new WebReport();

            webReport.Report.Load(webRootPath + "\\WorkOrder.frx"); // summary report

            webReport.Report.SetParameterValue("entryparam", EntryId);
            webReport.Report.SetParameterValue("yearparam", YearCode);
            return View(webReport);
        }
        public ActionResult HtmlSave(int EntryId = 0, int YearCode = 0)
        {
            using (Report report = new Report())
            {
                string webRootPath = IWebHostEnvironment.WebRootPath;
                var webReport = new WebReport();

                webReport.Report.Load(webRootPath + "\\WorkOrder.frx");
                //webReport.Report.SetParameterValue("flagparam", "PURCHASEORDERPRINT");
                webReport.Report.SetParameterValue("entryparam", EntryId);
                webReport.Report.SetParameterValue("yearparam", YearCode);
                webReport.Report.Prepare();// Preparing a report

                // Creating the HTML export
                using (HTMLExport html = new HTMLExport())
                {
                    using (FileStream st = new FileStream(webRootPath + "\\test.html", FileMode.Create))
                    {
                        webReport.Report.Export(html, st);
                        return File("App_Data/test.html", "application/octet-stream", "Test.html");
                    }
                }
            }
        }

        public IActionResult GetImage(int EntryId = 0, int YearCode = 0)
        {
            // Creatint the Report object
            using (Report report = new Report())
            {
                string webRootPath = IWebHostEnvironment.WebRootPath;
                var webReport = new WebReport();


                webReport.Report.Load(webRootPath + "\\WorkOrder.frx");
                //webReport.Report.SetParameterValue("flagparam", "PURCHASEORDERPRINT");
                webReport.Report.SetParameterValue("entryparam", EntryId);
                webReport.Report.SetParameterValue("yearparam", YearCode);
                webReport.Report.Prepare();// Preparing a report

                // Creating the Image export
                using (ImageExport image = new ImageExport())
                {
                    image.ImageFormat = ImageExportFormat.Jpeg;
                    image.JpegQuality = 100; // Set up the quality
                    image.Resolution = 100; // Set up a resolution 
                    image.SeparateFiles = false; // We need all pages in one big single file

                    using (MemoryStream st = new MemoryStream())// Using stream to save export
                    {
                        webReport.Report.Export(image, st);
                        return base.File(st.ToArray(), "image/jpeg");
                    }
                }
            }
        }


        public IActionResult AddWorkOrderGrid(List<WorkOrderDetail> model)
        {
            try
            {
                var MainModel = new WorkOrderModel();
                var WorkOrderDetails = new List<WorkOrderDetail>();
                var WODetails = new List<WorkOrderDetail>();
                var SSGrid = new List<WorkOrderDetail>();
                MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                    SlidingExpiration = TimeSpan.FromMinutes(55),
                    Size = 1024,
                };
                var seqNo = 0;
                foreach (var item in model)
                {
                    string modelJson = HttpContext.Session.GetString("KeyWorkOrderGrid");
                    List<WorkOrderDetail> WODetail = new List<WorkOrderDetail>();
                    if (!string.IsNullOrEmpty(modelJson))
                    {
                        WODetail = JsonConvert.DeserializeObject<List<WorkOrderDetail>>(modelJson);
                    }
                    if (item != null)
                    {
                        item.OrderWEF = item.OrderWEF == "" ? string.Empty : ParseDate(item.OrderWEF);
                        item.AmendEffDate = item.AmendEffDate == "" ? string.Empty : ParseDate(item.AmendEffDate);
                        item.BomEffectiveDate = item.BomEffectiveDate == "" ? string.Empty : ParseDate(item.BomEffectiveDate);
                        item.ApproxStartDate = item.ApproxStartDate == "" ? string.Empty : ParseDate(item.ApproxStartDate);
                        item.ApproxEndDate = item.ApproxEndDate == "" ? string.Empty : ParseDate(item.ApproxEndDate);
                        item.WORevDate = item.WORevDate == "" ? string.Empty : ParseDate(item.WORevDate);
                        item.SODATE = item.SODATE == "" ? string.Empty : ParseDate(item.SODATE);
                        item.SCHDATE = item.SCHDATE == "" ? string.Empty : ParseDate(item.SCHDATE);
                        item.SOCloseDate = item.SOCloseDate == "" ? string.Empty : ParseDate(item.SOCloseDate);
                        item.SchEffTillDate = item.SchEffTillDate == "" ? string.Empty : ParseDate(item.SchEffTillDate);
                        if (WODetail == null)
                        {
                            item.SeqNo = seqNo + 1;
                            WorkOrderDetails.Add(item);
                            seqNo++;
                        }
                        else
                        {
                            if (WODetail.Where(x => x.AccountName == item.AccountName && x.SONO == item.SONO && x.SchNo == item.SchNo && x.Itemcode == item.Itemcode && x.CustomerOrderNo == item.CustomerOrderNo).Any())
                            {
                                return StatusCode(207, "Duplicate");
                            }
                            else
                            {
                                item.SeqNo = WODetail.Count + 1;
                                WorkOrderDetails = WODetail.Where(x => x != null).ToList();
                                SSGrid.AddRange(WorkOrderDetails);
                                WorkOrderDetails.Add(item);
                            }
                        }
                        WorkOrderDetails = WorkOrderDetails.Where(x => x != null).ToList();
                        MainModel.WorkDetailGrid = WorkOrderDetails;

                        string serializedGrid = JsonConvert.SerializeObject(MainModel.WorkDetailGrid);
                        HttpContext.Session.SetString("KeyWorkOrderGrid", serializedGrid);
                    }
                }
                return PartialView("_WorkOrderGrid", MainModel);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IActionResult DeleteItemRow(int SeqNo)
        {
            var MainModel = new WorkOrderModel();
            string modelJson = HttpContext.Session.GetString("KeyWorkOrderGrid");
            List<WorkOrderDetail> WorkOrderDetail = new List<WorkOrderDetail>();
            if (!string.IsNullOrEmpty(modelJson))
            {
                WorkOrderDetail = JsonConvert.DeserializeObject<List<WorkOrderDetail>>(modelJson);
            }
            
            int Indx = Convert.ToInt32(SeqNo) - 1;

            if (WorkOrderDetail != null && WorkOrderDetail.Count > 0)
            {
                WorkOrderDetail.RemoveAt(Convert.ToInt32(Indx));

                Indx = 0;

                foreach (var item in WorkOrderDetail)
                {
                    Indx++;
                    item.SeqNo = Indx;
                }
                MainModel.WorkDetailGrid = WorkOrderDetail;

                string serializedGrid = JsonConvert.SerializeObject(MainModel.WorkDetailGrid);
                HttpContext.Session.SetString("KeyWorkOrderGrid", serializedGrid);
            }
            return PartialView("_WorkOrderGrid", MainModel);
        }

        public async Task<JsonResult> GetStoreList()
        {
            var JSON = await _IworkOrder.GetStoreList("GetStore", "SP_WorkOrder");
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetTotalStock(int store, int Itemcode)
        {
            var JSON = await _IworkOrder.GetTotalStockList(store, Itemcode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> NewEntryId(int YearCode)
        {
            var JSON = await _IworkOrder.NewEntryId(YearCode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillWorkCenter()
        {
            var JSON = await _IworkOrder.FillWorkCenter();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> DisplayBomDetail(int ItemCode, float WOQty, int BomRevNo)
        {
            var JSON = await _IworkOrder.DisplayBomDetail(ItemCode, WOQty, BomRevNo);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<IActionResult> DeleteByID(int ID, int YC,string CC,string FromDate,string ToDate,string WONO, string SONO,string SchNo,string AccountName,string PartCode,string ItemName,string SummaryDetail)
        {
            var Result = await _IworkOrder.DeleteByID(ID, YC);
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

            return RedirectToAction("WorkOrderDashboard", new { CC=CC,FromDate = FromDate, ToDatee = ToDate,WONO = WONO,SONO=SONO,SchNo,AccountName = AccountName,PartCode = PartCode,ItemName = ItemName,SummaryDetail = SummaryDetail,Flag = "false" });
        }

        public async Task<ActionResult> WorkOrderEdit(int ID, string Mode, int YC)//, ILogger logger)
        {
            //_logger.LogInformation("\n \n ******** Page Gate Inward ******** \n \n " + IWebHostEnvironment.EnvironmentName.ToString() + "\n \n");
            TempData.Clear();
            var MainModel = new WorkOrderModel();
            HttpContext.Session.Remove("KeyWorkOrderGrid");
            if (!string.IsNullOrEmpty(Mode) && ID > 0 && (Mode == "V" || Mode == "U"))
            {
                MainModel = await _IworkOrder.GetViewByID(ID, YC, Mode).ConfigureAwait(false);
                MainModel.Mode = Mode;
                MainModel.ID = ID;
                MainModel.CC = HttpContext.Session.GetString("Branch");
                MainModel.FinFromDate = HttpContext.Session.GetString("FromDate");
                MainModel.FinToDate = HttpContext.Session.GetString("ToDate");

                string serializedGrid = JsonConvert.SerializeObject(MainModel.WorkDetailGrid);
                HttpContext.Session.SetString("KeyWorkOrderGrid", serializedGrid);
            }
            else
            {
                MainModel.CC = HttpContext.Session.GetString("Branch");
                MainModel.FinFromDate = HttpContext.Session.GetString("FromDate");
                MainModel.FinToDate = HttpContext.Session.GetString("ToDate");
            }
            return View("WorkOrder", MainModel);
        }
    }
}
