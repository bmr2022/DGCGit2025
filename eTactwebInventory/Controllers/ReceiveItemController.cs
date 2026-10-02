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
    public class ReceiveItemController : Controller
    {
        private readonly IDataLogic _IDataLogic;
        private readonly IReceiveItem _IReceiveItem;
        private readonly ILogger<ReceiveItemController> _logger;
        private readonly IWebHostEnvironment _IWebHostEnvironment;
        public ReceiveItemController(ILogger<ReceiveItemController> logger, IDataLogic iDataLogic, IReceiveItem IReceiveItem, IWebHostEnvironment iWebHostEnvironment)
        {
            _logger = logger;
            _IDataLogic = iDataLogic;
            _IReceiveItem = IReceiveItem;
            _IWebHostEnvironment = iWebHostEnvironment;
        }
        [Route("{controller}/Index")]
        public IActionResult ReceiveItem(string formKey, string uniqueKey)
        {
            ViewBag.formKey = formKey;

            ViewBag.uniqueKey = uniqueKey;
            ViewData["Title"] = "Receive Item Detail";
            TempData.Clear();
            HttpContext.Session.Remove($"KeyReceiveItemGrid_{uniqueKey}");
            HttpContext.Session.Remove($"KeyReceiveItemGridDetail_{uniqueKey}");
            var MainModel = new ReceiveItemModel();
            MainModel.FromDate = HttpContext.Session.GetString($"FromDate_{formKey}");
            MainModel.ToDate = HttpContext.Session.GetString($"ToDate_{formKey}");
            HttpContext.Session.SetString($"KeyReceiveItemGrid_{uniqueKey}", JsonConvert.SerializeObject(MainModel));
            return View(MainModel);
        }
        public async Task<JsonResult> GetFormRights(string formKey)
        {
            var userID = Convert.ToInt32(HttpContext.Session.GetString($"EmpID_{formKey}"));
            var JSON = await _IReceiveItem.GetFormRights(userID);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        [Route("{controller}/Index")]
        [HttpGet]
        public async Task<ActionResult> ReceiveItem(string formKey, string uniqueKey, int ID, string Mode, int YC, string FromDate = "", string ToDate = "", string ItemName = "", string PartCode = "", string DashboardType = "", string SearchBox = "")//, ILogger logger)
        {
            //_logger.LogInformation("\n \n ********** Page Gate Inward ********** \n \n " + IWebHostEnvironment.EnvironmentName.ToString() + "\n \n");
            TempData.Clear();
            ViewBag.formKey = formKey;

            ViewBag.uniqueKey = uniqueKey;
            var MainModel = new ReceiveItemModel();
            HttpContext.Session.Remove($"KeyReceiveItemGridDetail_{uniqueKey}");
            MainModel.CC = HttpContext.Session.GetString($"Branch_{formKey}");
            MainModel.RecMatYearCode = Convert.ToInt32(HttpContext.Session.GetString($"YearCode_{formKey}"));
            MainModel.FromDate = HttpContext.Session.GetString($"FromDate_{formKey}");
            MainModel.ToDate = HttpContext.Session.GetString($"ToDate_{formKey}");
            MainModel.UID = Convert.ToInt32(HttpContext.Session.GetString($"UID_{formKey}"));
            //MainModel = await BindEmpList(MainModel);
            HttpContext.Session.Remove($"KeyReceiveItemGrid_{uniqueKey}");
            if (!string.IsNullOrEmpty(Mode) && ID > 0 && (Mode == "V" || Mode == "U"))
            {
                MainModel = await _IReceiveItem.GetViewByID(ID, YC).ConfigureAwait(false);
                MainModel.Mode = Mode;
                MainModel.ID = ID;
                HttpContext.Session.SetString($"KeyReceiveItemGrid_{uniqueKey}", JsonConvert.SerializeObject(MainModel.ItemDetailGrid));
            }
            else
            {
                // MainModel = await BindModel(MainModel);
            }

            if (Mode != "U")
            {
                MainModel.ActualEnteredBy = Convert.ToInt32(HttpContext.Session.GetString($"UID_{formKey}"));
                MainModel.ActualEnteredByName = HttpContext.Session.GetString($"EmpName_{formKey}");
                MainModel.ActualEntrydate = HttpContext.Session.GetString($"EntryDate_{formKey}");
            }
            else
            {
                MainModel.ActualEnteredBy = Convert.ToInt32(HttpContext.Session.GetString($"UID_{formKey}"));
                MainModel.ActualEnteredByName = HttpContext.Session.GetString($"EmpName_{formKey}");
                MainModel.UpdatedBy = Convert.ToInt32(HttpContext.Session.GetString($"UID_{formKey}"));
                MainModel.UpdatedByName = HttpContext.Session.GetString($"EmpName_{formKey}");
                MainModel.UpdatedOn = HttpContext.Session.GetString($"LastUpdatedDate_{formKey}");
            }
            //MainModel.ActualEnteredBy = Convert.ToInt32(HttpContext.Session.GetString($"UID_{formKey}"));
            //MainModel.ActualEnteredByName = HttpContext.Session.GetString($"EmpName_{formKey}");
            MainModel.FromDateBack = FromDate;
            MainModel.ToDateBack = ToDate;
            MainModel.PartCodeBack = PartCode;
            MainModel.ItemNameBack = ItemName;
            MainModel.DashboardTypeBack = DashboardType;
            MainModel.GlobalSearchBack = SearchBox;
            return View(MainModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("{controller}/Index")]
        public async Task<IActionResult> ReceiveItem(ReceiveItemModel model)
        {
            try
            {
                var formKey = model.formKey;
                var uniqueKey = model.uniqueKey;
                var ReceiveItemGrid = new DataTable();
                string modelJson = HttpContext.Session.GetString($"KeyReceiveItemGridDetail_{uniqueKey}");
                List<ReceiveItemDetail> ReceiveItemDetail = new List<ReceiveItemDetail>();
                if (!string.IsNullOrEmpty(modelJson))
                {
                    ReceiveItemDetail = JsonConvert.DeserializeObject<List<ReceiveItemDetail>>(modelJson);
                }
                string receiveItemJson = HttpContext.Session.GetString($"KeyReceiveItemGrid_{uniqueKey}");
                List<ReceiveItemDetail> ReceiveItemDetailEdit = new List<ReceiveItemDetail>();
                if (!string.IsNullOrEmpty(receiveItemJson))
                {
                    ReceiveItemDetailEdit = JsonConvert.DeserializeObject<List<ReceiveItemDetail>>(receiveItemJson);
                }

                if (ReceiveItemDetail == null && ReceiveItemDetailEdit == null)
                {
                    ModelState.Clear();
                    ModelState.TryAddModelError("ReceiveItemDetail", "Receive Item Detail Should Have Atleast 1 Item...!");
                    return View("ReceiveItem", model);
                }

                else
                {
                    model.UID = Convert.ToInt32(HttpContext.Session.GetString($"UID_{formKey}"));
                    model.CC = HttpContext.Session.GetString($"Branch_{formKey}");
                    model.ActualEnteredBy = Convert.ToInt32(HttpContext.Session.GetString($"UID_{formKey}"));
                    if (model.Mode == "U")
                    {
                        model.UpdatedBy = Convert.ToInt32(HttpContext.Session.GetString($"UID_{formKey}"));
                        model.UpdatedByName = HttpContext.Session.GetString($"EmpName_{formKey}");
                        ReceiveItemGrid = GetDetailTable(ReceiveItemDetailEdit);
                    }
                    else
                    {
                        ReceiveItemGrid = GetDetailTable(ReceiveItemDetail);
                    }
                    model.EnteredbyMachineName = HttpContext.Session.GetString($"ClientMachineName_{formKey}");
                    model.IPAddress = HttpContext.Session.GetString($"ClientIP_{formKey}");
                    var Result = await _IReceiveItem.SaveInprocessQc(model, ReceiveItemGrid);

                    if (Result != null)
                    {
                        if (Result.StatusText == "Success" && Result.StatusCode == HttpStatusCode.OK)
                        {
                            ViewBag.isSuccess = true;
                            TempData["200"] = "200";
                            //_MemoryCache.Remove(ReceiveItemDetail);
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
                    return RedirectToAction("PendingToReceiveItem", "PendingToReceiveItem", new { formKey = formKey });
                }
            }
            catch (Exception ex)
            {
                throw;
                //LogException<ReceiveItemController>.WriteException(_logger, ex);


                //var ResponseResult = new ResponseResult()
                //{
                //    StatusCode = HttpStatusCode.InternalServerError,
                //    StatusText = "Error",
                //    Result = ex
                //};

                //return View("Error", ResponseResult);
            }
        }
        public async Task<JsonResult> FillEntryId(int YearCode)
        {
            var JSON = await _IReceiveItem.FillEntryId("NewEntryId", YearCode, "SP_ReceiveMaterialInStore");
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public IActionResult FillGridFromMemoryCache(string uniqueKey, string formKey)
        {
            try
            {
                string modelJson = HttpContext.Session.GetString($"KeyReceiveItem_{uniqueKey}");
                IList<ReceiveItemDetail> ReceiveItemDetailGrid = new List<ReceiveItemDetail>();
                if (!string.IsNullOrEmpty(modelJson))
                {
                    ReceiveItemDetailGrid = JsonConvert.DeserializeObject<List<ReceiveItemDetail>>(modelJson);
                }
                var MainModel = new ReceiveItemModel();
                var ReceiveItem = new List<ReceiveItemDetail>();
                var SSGrid = new List<ReceiveItemDetail>();
                MainModel.FromDate = HttpContext.Session.GetString($"FromDate_{formKey}");
                MainModel.ToDate = HttpContext.Session.GetString($"ToDate_{formKey}");

                MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                    SlidingExpiration = TimeSpan.FromMinutes(55),
                    Size = 1024,
                };
                var seqNo = 1;
                if (ReceiveItemDetailGrid != null)
                {
                    for (int i = 0; i < ReceiveItemDetailGrid.Count; i++)
                    {
                        if (ReceiveItemDetailGrid[i] != null)
                        {
                            ReceiveItemDetailGrid[i].SeqNo = seqNo++;
                            SSGrid.AddRange(ReceiveItem);
                            ReceiveItem.Add(ReceiveItemDetailGrid[i]);

                            MainModel.ItemDetailGrid = ReceiveItem;

                            HttpContext.Session.SetString($"KeyReceiveItemGrid_{uniqueKey}", JsonConvert.SerializeObject(MainModel.ItemDetailGrid));
                        }
                    }
                }
                return PartialView("_ReceiveItemGrid", MainModel);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IActionResult DeleteItemRow(int SeqNo, string Mode, string uniqueKey)
        {
            var MainModel = new ReceiveItemModel();
            if (Mode == "U")
            {
                string modelJson = HttpContext.Session.GetString($"KeyReceiveItemGrid_{uniqueKey}");
                List<ReceiveItemDetail> ReceiveItemDetail = new List<ReceiveItemDetail>();
                if (!string.IsNullOrEmpty(modelJson))
                {
                    ReceiveItemDetail = JsonConvert.DeserializeObject<List<ReceiveItemDetail>>(modelJson);
                }
                int Indx = Convert.ToInt32(SeqNo) - 1;

                if (ReceiveItemDetail != null && ReceiveItemDetail.Count > 0)
                {
                    ReceiveItemDetail.RemoveAt(Convert.ToInt32(Indx));

                    Indx = 0;

                    foreach (var item in ReceiveItemDetail)
                    {
                        Indx++;
                        item.SeqNo = Indx;
                    }
                    MainModel.ItemDetailGrid = ReceiveItemDetail;

                    HttpContext.Session.SetString($"KeyReceiveItemGrid_{uniqueKey}", JsonConvert.SerializeObject(MainModel.ItemDetailGrid));
                }
            }
            else
            {
                string modelJson = HttpContext.Session.GetString($"KeyReceiveItemGridDetail_{uniqueKey}");
                List<ReceiveItemDetail> ReceiveItemDetail = new List<ReceiveItemDetail>();
                if (!string.IsNullOrEmpty(modelJson))
                {
                    ReceiveItemDetail = JsonConvert.DeserializeObject<List<ReceiveItemDetail>>(modelJson);
                }
                int Indx = Convert.ToInt32(SeqNo) - 1;

                if (ReceiveItemDetail != null && ReceiveItemDetail.Count > 0)
                {
                    ReceiveItemDetail.RemoveAt(Convert.ToInt32(Indx));

                    Indx = 0;

                    foreach (var item in ReceiveItemDetail)
                    {
                        Indx++;
                        item.SeqNo = Indx;
                    }
                    MainModel.ItemDetailGrid = ReceiveItemDetail;

                    HttpContext.Session.SetString($"KeyReceiveItemGridDetail_{uniqueKey}", JsonConvert.SerializeObject(MainModel.ItemDetailGrid));
                }
            }

            return PartialView("_ReceiveItemDetailGrid", MainModel);
        }
        public IActionResult EditItemRow(int SeqNo, string Mode, string uniqueKey)
        {
            IList<ReceiveItemDetail> ReceiveItemDetail = new List<ReceiveItemDetail>();
            if (Mode == "U")
            {
                string modelJson = HttpContext.Session.GetString($"KeyReceiveItemGrid_{uniqueKey}");
                if (!string.IsNullOrEmpty(modelJson))
                {
                    ReceiveItemDetail = JsonConvert.DeserializeObject<List<ReceiveItemDetail>>(modelJson);
                }
            }
            else
            {
                string modelJson = HttpContext.Session.GetString($"KeyReceiveItemGridDetail_{uniqueKey}");
                if (!string.IsNullOrEmpty(modelJson))
                {
                    ReceiveItemDetail = JsonConvert.DeserializeObject<List<ReceiveItemDetail>>(modelJson);
                }
            }
            IEnumerable<ReceiveItemDetail> SSGrid = ReceiveItemDetail;
            if (ReceiveItemDetail != null)
            {
                SSGrid = ReceiveItemDetail.Where(x => x.SeqNo == SeqNo);
            }
            string JsonString = JsonConvert.SerializeObject(SSGrid);
            return Json(JsonString);
        }
        public async Task<IActionResult> ReceiveItemInStoreDashboard(string formKey, string FromDate = "", string ToDate = "", string Flag = "True")
        {
            try
            {
                ViewBag.formKey = formKey;
                var uniqueKey = Guid.NewGuid().ToString();
                ViewBag.uniqueKey = uniqueKey;
                HttpContext.Session.Remove($"KeyReceiveItemGridDetail_{uniqueKey}");
                var model = new ReceiveItemDashboard();
                var Result = await _IReceiveItem.GetDashboardData().ConfigureAwait(true);
                DateTime now = DateTime.Now;

                model.FromDate = new DateTime(now.Year, now.Month, 1).ToString("dd/MM/yyyy").Replace("-", "/");
                model.ToDate = new DateTime(DateTime.Today.Year + 1, 3, 31).ToString("dd/MM/yyyy").Replace("-", "/");

                if (Result != null)
                {
                    var _List = new List<TextValue>();
                    DataSet DS = Result.Result;
                    if (DS != null)
                    {
                        var DT = DS.Tables[0].DefaultView.ToTable(true, "RecMatEntryId", "RecMatYearCode",
                            "RecMatEntryDate", "RecMatSlipNo", "RecMatSlipDate", "CC", "ActualEntryByEmpid",
                            "ActualEntryByname", "ActualEntryDate", "UpdatedByName", "UpdationDate", "EntryByMachine");
                        model.ReceiveItemDashboard = CommonFunc.DataTableToList<ReceiveItemDetailDashboard>(DT, "ReceiveMaterialInStore");

                    }
                    if (Flag != "True")
                    {
                        model.FromDate1 = FromDate;
                        model.ToDate1 = ToDate;
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
        public async Task<IActionResult> GetSearchData(string formKey, string uniqueKey, string FromDate, string ToDate, string ItemName, string PartCode, string DashboardType)
        {
            ViewBag.formKey = formKey;

            //model.Mode = "Search";
            var model = new ReceiveItemDashboard();
            model = await _IReceiveItem.GetDashboardData(FromDate, ToDate, ItemName, PartCode, DashboardType);
            model.DashboardType = "Summary";
            model.formKey = formKey;
            model.uniqueKey = uniqueKey;
            return PartialView("_ReceiveItemDashboard", model);
        }
        public async Task<IActionResult> GetDashboardDetailData(string formKey, string uniqueKey, string FromDate, string ToDate, string ItemName, string PartCode, string DashboardType)
        {
            //model.Mode = "Search";
            var model = new ReceiveItemDashboard();
            model = await _IReceiveItem.GetDashboardDetailData(FromDate, ToDate, ItemName, PartCode, DashboardType);
            model.DashboardType = "Detail";

            model.formKey = formKey;
            model.uniqueKey = uniqueKey;
            return PartialView("_ReceiveItemDashboardDetail", model);
        }
        public async Task<JsonResult> BindDepartmentList(ReceiveItemModel model, string formKey)
        {
            model.FromDate = HttpContext.Session.GetString($"FromDate_{formKey}");
            model.ToDate = HttpContext.Session.GetString($"ToDate_{formKey}");
            var JSON = await _IReceiveItem.BindDepartmentList(model.FromDate, model.ToDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public IActionResult AddReceiveItemGridEntry(List<ReceiveItemDetail> model, string Mode, string uniqueKey)
        {
            try
            {
                var MainModel = new ReceiveItemModel();
                var ReceiveItemDetailGrid = new List<ReceiveItemDetail>();
                var SSGrid = new List<ReceiveItemDetail>();
                if (Mode == "U")
                {
                    string modelJson = HttpContext.Session.GetString($"KeyReceiveItemGrid_{uniqueKey}");
                    IList<ReceiveItemDetail> ReceiveItemDetail = new List<ReceiveItemDetail>();
                    if (!string.IsNullOrEmpty(modelJson))
                    {
                        ReceiveItemDetail = JsonConvert.DeserializeObject<List<ReceiveItemDetail>>(modelJson);
                    }
                    var seqNo = 0;
                    foreach (var item in model)
                    {
                        if (item != null)
                        {
                            if (ReceiveItemDetail == null)
                            {
                                item.SeqNo = seqNo + 1;
                                ReceiveItemDetailGrid.Add(item);
                                seqNo++;
                            }
                            else
                            {
                                item.SeqNo = ReceiveItemDetail.Count + 1;
                                //ReceiveItemDetailGrid = ReceiveItemDetail.Where(x => x != null).ToList();
                                SSGrid.AddRange(ReceiveItemDetailGrid);
                                ReceiveItemDetailGrid.Add(item);
                            }
                            MainModel.ItemDetailGrid = ReceiveItemDetailGrid;
                            HttpContext.Session.SetString($"KeyReceiveItemGrid_{uniqueKey}", JsonConvert.SerializeObject(MainModel.ItemDetailGrid));
                        }
                    }
                }
                else
                {
                    string modelJson = HttpContext.Session.GetString($"KeyReceiveItemGridDetail_{uniqueKey}");
                    IList<ReceiveItemDetail> ReceiveItemDetail = new List<ReceiveItemDetail>();
                    if (!string.IsNullOrEmpty(modelJson))
                    {
                        ReceiveItemDetail = JsonConvert.DeserializeObject<List<ReceiveItemDetail>>(modelJson);
                    }
                    var seqNo = 0;
                    foreach (var item in model)
                    {
                        if (item != null)
                        {
                            if (ReceiveItemDetail == null)
                            {
                                item.SeqNo = seqNo + 1;
                                ReceiveItemDetailGrid.Add(item);
                                seqNo++;
                            }
                            else
                            {
                                item.SeqNo = ReceiveItemDetail.Count + 1;
                                //ReceiveItemDetailGrid = ReceiveItemDetail.Where(x => x != null).ToList();
                                SSGrid.AddRange(ReceiveItemDetailGrid);
                                ReceiveItemDetailGrid.Add(item);
                            }
                            MainModel.ItemDetailGrid = ReceiveItemDetailGrid;
                            HttpContext.Session.SetString($"KeyReceiveItemGridDetail_{uniqueKey}", JsonConvert.SerializeObject(MainModel.ItemDetailGrid));
                        }
                    }
                }
                return PartialView("_ReceiveItemDetailGrid", MainModel);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private static DataTable GetDetailTable(IList<ReceiveItemDetail> ReceiveItemDetail)
        {
            try
            {
                var ReceiveItem = new DataTable();

                ReceiveItem.Columns.Add("RecMatEntryId", typeof(int));
                ReceiveItem.Columns.Add("RecMatEntryDate", typeof(string));
                ReceiveItem.Columns.Add("RecMatYearCode", typeof(int));
                ReceiveItem.Columns.Add("RecMatSlipNo", typeof(string));
                ReceiveItem.Columns.Add("RecMatSlipDate", typeof(string));
                ReceiveItem.Columns.Add("SeqNo", typeof(int));
                ReceiveItem.Columns.Add("ReceiveDate", typeof(string));
                ReceiveItem.Columns.Add("MaterialType", typeof(string));
                ReceiveItem.Columns.Add("FromDepWorkCenter", typeof(string));
                ReceiveItem.Columns.Add("FromWCID", typeof(int));
                ReceiveItem.Columns.Add("ItemCode", typeof(int));
                ReceiveItem.Columns.Add("ActualRecQtyInStr", typeof(decimal));
                ReceiveItem.Columns.Add("ActualTransferQtyFrmWC", typeof(decimal));
                ReceiveItem.Columns.Add("Unit", typeof(string));
                ReceiveItem.Columns.Add("AltQty", typeof(decimal));
                ReceiveItem.Columns.Add("AltUnit", typeof(string));
                ReceiveItem.Columns.Add("Remark", typeof(string));
                ReceiveItem.Columns.Add("RecInStore", typeof(int));
                ReceiveItem.Columns.Add("Prodentryid", typeof(int));
                ReceiveItem.Columns.Add("ProdyearCode", typeof(int));
                ReceiveItem.Columns.Add("ProdDateAndTime", typeof(string));
                ReceiveItem.Columns.Add("PlanNoEntryId", typeof(int));
                ReceiveItem.Columns.Add("ProdPlanNo", typeof(string));
                ReceiveItem.Columns.Add("ProdPlanYearCode", typeof(int));
                ReceiveItem.Columns.Add("ProdSchEntryId", typeof(int));
                ReceiveItem.Columns.Add("ProdSchNo", typeof(string));
                ReceiveItem.Columns.Add("ProdSchYearCode", typeof(int));
                ReceiveItem.Columns.Add("InProcQCSlipNo", typeof(string));
                ReceiveItem.Columns.Add("InProcQCEntryId", typeof(int));
                ReceiveItem.Columns.Add("InProcQCYearCode", typeof(int));
                ReceiveItem.Columns.Add("ProdQty", typeof(decimal));
                ReceiveItem.Columns.Add("RejQty", typeof(decimal));
                ReceiveItem.Columns.Add("QCOkQty", typeof(decimal));
                ReceiveItem.Columns.Add("Batchno", typeof(string));
                ReceiveItem.Columns.Add("UniqueBatchno", typeof(string));
                ReceiveItem.Columns.Add("TransferMatEntryId", typeof(int));
                ReceiveItem.Columns.Add("TransferMatYearCode", typeof(int));
                ReceiveItem.Columns.Add("TransferMatSlipNo", typeof(string));
                foreach (var Item in ReceiveItemDetail)
                {
                    ReceiveItem.Rows.Add(
                        new object[]
                        {

                    Item.RecMatEntryId == 0 ? 0 : Item.RecMatEntryId,
                    Item.RecMatEntryDate==null ? string.Empty : CommonFunc.ParseFormattedDate(Item.RecMatEntryDate),
                    Item.RecMatYearCode== 0 ? 0 : Item.RecMatYearCode,
                    Item.RecMatSlipNo==null ? "" : Item.RecMatSlipNo,
                    Item.RecMatSlipDate == null ? string.Empty : CommonFunc.ParseFormattedDate(Item.RecMatSlipDate),
                    Item.SeqNo== 0 ? 0 : Item.SeqNo,
                    Item.ReceiveDate==null ? string.Empty : CommonFunc.ParseFormattedDate(Item.ReceiveDate),
                    Item.MaterialType== null ? "":Item.MaterialType,
                    Item.FromDepWorkCenter == null ? "" : Item.FromDepWorkCenter,
                    Item.WCID== 0 ? 0:Item.WCID,
                    Item.ItemCode== 0 ? 0:Item.ItemCode,
                    Item.Qty== 0 ? 0:Item.Qty,
                    Item.AltTransferQty== 0 ? 0:Item.AltTransferQty,
                    Item.Unit == null ? "" : Item.Unit,
                    Item.Qty== 0 ? 0:Item.Qty,
                    Item.AltUnit == null ? "" : Item.AltUnit,
                    Item.Itemremark == null ? "" : Item.Itemremark,
                    Item.RecStoreId == 0 ? 1  : Item.RecStoreId,
                    Item.ProdEntryId == 0 ? 0 : Item.ProdEntryId,
                    Item.ProdYearCode == 0 ? 0 : Item.ProdYearCode,
                    Item.ProdEntryDate == null ? string.Empty : CommonFunc.ParseFormattedDate(Item.ProdEntryDate),
                    Item.ProdEntryId == 0 ? 0 : Item.ProdEntryId,
                    Item.ProdPlanNo == null ? "" : Item.ProdPlanNo,
                    Item.ProdPlanYearCode == 0 ? 0 : Item.ProdPlanYearCode,
                    Item.ProdEntryId==0?0:Item.ProdEntryId,
                    Item.ProdSchNo == null ? "" : Item.ProdSchNo,
                    Item.ProdSchYearCode==0?0:Item.ProdSchYearCode,
                    Item.InProcQCSlipNo==null?"":Item.InProcQCSlipNo,
                    Item.InProcQCEntryId==0?0:Item.InProcQCEntryId,
                    Item.InProcQCYearCode == 0 ? 0 : Item.InProcQCYearCode,
                    Item.ProdQty==0?0:Item.ProdQty,
                    Item.RejQty==0?0:Item.RejQty,
                    Item.QCOkQty==0?0:Item.QCOkQty,
                    Item.BatchNo==null?"":Item.BatchNo,
                    Item.uniquebatchno==null?"":Item.uniquebatchno,
                    Item.TransferMatEntryId==0?0:Item.TransferMatEntryId,
                    Item.TransferMatYearCode==0?0:Item.TransferMatYearCode,
                    Item.TransferMatSlipNo==null?"":Item.TransferMatSlipNo
                    });
                }
                ReceiveItem.Dispose();
                return ReceiveItem;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<IActionResult> DeleteByID(int ID, int YC, string RecMatSlipNo, string EntryByMachineName, int ActualEntryBy)
        {
            var Result = await _IReceiveItem.DeleteByID(ID, YC, RecMatSlipNo, EntryByMachineName, ActualEntryBy);

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

            return RedirectToAction("ReceiveItemInStoreDashboard", new { ActualEntryBy = ActualEntryBy, Flag = "False", RecMatSlipNo = RecMatSlipNo, EntryByMachineName = EntryByMachineName });
        }
    }
}
