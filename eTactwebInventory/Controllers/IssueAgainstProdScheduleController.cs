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

namespace eTactWeb.Controllers
{
    public class IssueAgainstProdScheduleController : Controller
    {
        private readonly IDataLogic _IDataLogic;
        private readonly IIssueAgainstProdSchedule _IIssueAgainstProdSchedule;
        private readonly ILogger<IssueAgainstProdScheduleController> _logger;
        private readonly IMemoryCache _MemoryCache;
        private readonly IWebHostEnvironment _IWebHostEnvironment;
        public IssueAgainstProdScheduleController(ILogger<IssueAgainstProdScheduleController> logger, IDataLogic iDataLogic, IIssueAgainstProdSchedule IIssueAgainstProdSchedule, IMemoryCache iMemoryCache, IWebHostEnvironment iWebHostEnvironment)
        {
            _logger = logger;
            _IDataLogic = iDataLogic;
            _IIssueAgainstProdSchedule = IIssueAgainstProdSchedule;
            _MemoryCache = iMemoryCache;
            _IWebHostEnvironment = iWebHostEnvironment;
        }
        [Route("{controller}/Index")]
        public IActionResult IssueAgainstProdSchedule()
        {
            ViewData["Title"] = "Issue Against Production Schedule";
            TempData.Clear();
            _MemoryCache.Remove("KeyIssueAgainstProdScheduleGrid");
            _MemoryCache.Remove("KeyIssueAgainstProdScheduleDetail");
            var MainModel = new IssueAgainstProdSchedule();
            MainModel.FromDate = HttpContext.Session.GetString("FromDate");
            MainModel.ToDate = HttpContext.Session.GetString("ToDate");
            MainModel.IssAgtProdSchYearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
            MainModel.CC = HttpContext.Session.GetString("Branch");
            MainModel.ActualEntryById = Convert.ToInt32(HttpContext.Session.GetString("UID"));
            MainModel.UID = Convert.ToInt32(HttpContext.Session.GetString("UID"));
            MainModel.ActualEntryByName = HttpContext.Session.GetString("EmpName");
            MainModel.ActualEntryDate = HttpContext.Session.GetString("EntryDate");
            MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                SlidingExpiration = TimeSpan.FromMinutes(55),
                Size = 1024,
            };

            _MemoryCache.Set("KeyIssueAgainstProdScheduleGrid", MainModel, cacheEntryOptions);
            return View(MainModel);
        }
        [Route("{controller}/Index")]
        [HttpGet]
        public async Task<ActionResult> IssueAgainstProdSchedule(int ID, string Mode,string ProdSchSlipNo, int YC, string FromDate = "", string ToDate = "", string TransferSlipNo = "", string ItemName = "", string PartCode = "", string FromWorkCenter = "", string ToWorkCenter = "", string StoreName = "", string ProdSlipNo = "", string ProdSchNo = "", string Searchbox = "", string DashboardType = "")//, ILogger logger)
        {
            //_logger.LogInformation("\n \n ********** Page Gate Inward ********** \n \n " + IWebHostEnvironment.EnvironmentName.ToString() + "\n \n");
            TempData.Clear();
            var MainModel = new IssueAgainstProdSchedule();
            MainModel.CC = HttpContext.Session.GetString("Branch");
            MainModel.IssAgtProdSchYearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
            MainModel.FromDate = HttpContext.Session.GetString("FromDate");
            MainModel.ToDate = HttpContext.Session.GetString("ToDate");
            _MemoryCache.Remove("KeyIssueAgainstProdScheduleGrid");
            if (!string.IsNullOrEmpty(Mode) && ID > 0 && (Mode == "V" || Mode == "U"))
            {
                MainModel = await _IIssueAgainstProdSchedule.GetViewByID(ID, YC,ProdSchSlipNo).ConfigureAwait(false);
                MainModel.Mode = Mode;
                MainModel.ID = ID;
                MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                    SlidingExpiration = TimeSpan.FromMinutes(55),
                    Size = 1024,
                };
                _MemoryCache.Set("KeyIssueAgainstProdScheduleGrid", MainModel.ItemDetailGrid, cacheEntryOptions);
            }
            else
            {
                // MainModel = await BindModel(MainModel);
            }

            if (Mode != "U")
            {
                MainModel.ActualEntryById = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                MainModel.ActualEntryByName = HttpContext.Session.GetString("EmpName");
                MainModel.ActualEntryDate = HttpContext.Session.GetString("EntryDate");
                MainModel.AckDate = HttpContext.Session.GetString("EntryDate");
            }
            else
            {
                MainModel.ActualEntryByName = HttpContext.Session.GetString("EmpName");
                MainModel.UpdatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                MainModel.LastUpdatedByName = HttpContext.Session.GetString("EmpName");
                MainModel.LastUpdatedDate = HttpContext.Session.GetString("LastUpdatedDate");
            }
            //MainModel.FromDateBack = FromDate;
            //MainModel.ToDateBack = ToDate;
            //MainModel.TransferSlipNoBack = TransferSlipNo;
            //MainModel.PartCodeBack = PartCode;
            //MainModel.ItemNameBack = ItemName;
            //MainModel.FromWorkCenterBack = FromWorkCenter;
            //MainModel.ToWorkCenterBack = ToWorkCenter;
            //MainModel.StoreNameBack = StoreName;
            //MainModel.ProdSlipNoBack=ProdSlipNo;
            //MainModel.ProdSchNoBack=ProdSchNo;
            //MainModel.GlobalSearchBack = Searchbox;
            //MainModel.DashboardTypeBack = DashboardType;
            return View(MainModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("{controller}/Index")]
        public async Task<IActionResult> IssueAgainstProdSchedule(IssueAgainstProdSchedule model)
        {
            try
            {
                var IssueAgainstProductionSchedule = new DataTable();

                _MemoryCache.TryGetValue("KeyIssueAgainstProdScheduleGrid", out List<IssueAgainstProdScheduleDetail> IssueAgainstProdScheduleDetail);

                if (IssueAgainstProdScheduleDetail == null)
                {
                    ModelState.Clear();
                    ModelState.TryAddModelError("IssueAgainstProductionSchedule", "Issue Against Production Schedule Grid Should Have Atleast 1 Item...!");
                    return View("IssueAgainstProdSchedule", model);
                }

                else
                {
                    model.CC = HttpContext.Session.GetString("Branch");
                    model.ActualEntryById = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
                    model.UID   = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
                    // model.ac = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                    if (model.Mode == "U")
                    {
                        model.UpdatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                        model.LastUpdatedByName = HttpContext.Session.GetString("EmpName");
                        IssueAgainstProductionSchedule = GetDetailTable(IssueAgainstProdScheduleDetail);
                    }
                    else
                    {
                        IssueAgainstProductionSchedule = GetDetailTable(IssueAgainstProdScheduleDetail);
                    }

                    var Result = await _IIssueAgainstProdSchedule.SaveIssueAgainstProductionSchedule(model, IssueAgainstProductionSchedule);

                    if (Result != null)
                    {
                        if (Result.StatusText == "Success" && Result.StatusCode == HttpStatusCode.OK)
                        {
                            ViewBag.isSuccess = true;
                            TempData["200"] = "200";
                            _MemoryCache.Remove(IssueAgainstProdScheduleDetail);
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
                return RedirectToAction("PendingProductionSchedule", "PendingProductionSchedule");
            }
            catch (Exception ex)
            {
                LogException<IssueAgainstProdScheduleController>.WriteException(_logger, ex);


                var ResponseResult = new ResponseResult()
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    StatusText = "Error",
                    Result = ex
                };

                return View("Error", ResponseResult);
            }
        }
        private static DataTable GetDetailTable(IList<IssueAgainstProdScheduleDetail> DetailList)
        {
            try
            {
                var TransferGrid = new DataTable();

                TransferGrid.Columns.Add("IssAgtProdSchEntryId", typeof(int));
                TransferGrid.Columns.Add("IssAgtProdSchYearCode", typeof(int));
                TransferGrid.Columns.Add("SeqNo", typeof(int));
                TransferGrid.Columns.Add("Wcid", typeof(int));
                TransferGrid.Columns.Add("ProdPlanNo", typeof(string));
                TransferGrid.Columns.Add("PlanNoYearCode", typeof(int));
                TransferGrid.Columns.Add("PlanNoEntryId", typeof(int));
                TransferGrid.Columns.Add("PlanDate", typeof(string));
                TransferGrid.Columns.Add("PRODPlanFGItemCode", typeof(int));
                TransferGrid.Columns.Add("ProdSchNo", typeof(string));
                TransferGrid.Columns.Add("ProdSchEntryId", typeof(int));
                TransferGrid.Columns.Add("ProdSchYearCode", typeof(int));
                TransferGrid.Columns.Add("ProdSchDate", typeof(string));
                TransferGrid.Columns.Add("ParentProdSchNo", typeof(string));
                TransferGrid.Columns.Add("ParentProdSchEntryId", typeof(int));
                TransferGrid.Columns.Add("ParentProdSchYearCode", typeof(int));
                TransferGrid.Columns.Add("ParentProdSchDate", typeof(string));
                TransferGrid.Columns.Add("PRODSCHFGItemCode", typeof(int));
                TransferGrid.Columns.Add("IssueItemCode", typeof(int));
                TransferGrid.Columns.Add("StoreId", typeof(int));
                TransferGrid.Columns.Add("BatchNo", typeof(string));
                TransferGrid.Columns.Add("UniqueBatchno", typeof(string));
                TransferGrid.Columns.Add("TotalStock", typeof(float));
                TransferGrid.Columns.Add("BatchStock", typeof(float));
                TransferGrid.Columns.Add("WIPStock", typeof(float));
                TransferGrid.Columns.Add("StdPkg", typeof(float));
                TransferGrid.Columns.Add("MaxIssueQtyAllowed", typeof(float));
                TransferGrid.Columns.Add("IssueQty", typeof(float));
                TransferGrid.Columns.Add("Unit", typeof(string));
                TransferGrid.Columns.Add("AltIssueQty", typeof(float));
                TransferGrid.Columns.Add("Altunit", typeof(string));
                TransferGrid.Columns.Add("PrevissueQty", typeof(float));
                TransferGrid.Columns.Add("PreIssueAltQty", typeof(float));
                TransferGrid.Columns.Add("BOMNo", typeof(int));
                TransferGrid.Columns.Add("BomQty", typeof(float));
                TransferGrid.Columns.Add("WIPMaxQty", typeof(float));
                TransferGrid.Columns.Add("InProcessQCEntryId", typeof(int));
                TransferGrid.Columns.Add("InprocessQCSlipNo", typeof(string));
                TransferGrid.Columns.Add("InProcessQCYearCode", typeof(int));
                TransferGrid.Columns.Add("InProcessQCDate", typeof(string));
                TransferGrid.Columns.Add("ItemHasSubBom", typeof(string));
                TransferGrid.Columns.Add("MaterialIsIssuedDirectlyFrmWC", typeof(string));
                TransferGrid.Columns.Add("IssuedFromWC", typeof(int));
                TransferGrid.Columns.Add("IssueFrmWCSlipNo", typeof(string));
                TransferGrid.Columns.Add("IssueFrmWCYearCode", typeof(int));
                TransferGrid.Columns.Add("IssueFrmWCDate", typeof(string));
                TransferGrid.Columns.Add("IssueFrmQCorTransferForm", typeof(string));
                TransferGrid.Columns.Add("ItemRemark", typeof(string));
                TransferGrid.Columns.Add("otherdetail1", typeof(string));
                TransferGrid.Columns.Add("otherdetail2", typeof(string));
                TransferGrid.Columns.Add("Itemrate", typeof(float));
                TransferGrid.Columns.Add("Itemcolor", typeof(string));
                TransferGrid.Columns.Add("ItemWeight", typeof(float));
                TransferGrid.Columns.Add("AltItemCode", typeof(int));
                TransferGrid.Columns.Add("IssuedAlternateItem", typeof(int));
                TransferGrid.Columns.Add("OriginalItemCode", typeof(int));
                foreach (var Item in DetailList)
                {
                    TransferGrid.Rows.Add(
                        new object[]
                        {

                    Item.IssAgtProdSchEntryId == 0 ? 0 : Item.IssAgtProdSchEntryId,
                    Item.IssAgtProdSchYearCode== 0 ? 0 : Item.IssAgtProdSchYearCode,
                    Item.seqno==0?0:Item.seqno,
                    Item.WCId == 0 ? 0:Item.WCId,
                    Item.ProdPlanNo==null?"":Item.ProdPlanNo,
                    Item.ProdPlanYearcode== 0 ? 0:Item.ProdPlanYearcode,
                    Item.ProdPlanEntryId == 0? 0 : Item.ProdPlanEntryId,
                    Item.ProdPlanDate == null ? string.Empty : ParseFormattedDate(Item.ProdPlanDate.Split(" ")[0]),
                    Item.ProdPlanFGItemCode== 0 ? 0:Item.ProdPlanFGItemCode,
                    Item.ProdSchNo==null?"":Item.ProdSchNo,
                    Item.ProdSchEntryId== 0 ? 0:Item.ProdSchEntryId,
                    Item.ProdSchYearcode== 0 ? 0:Item.ProdSchYearcode,
                    Item.ProdSchDate == null ? string.Empty : ParseFormattedDate(Item.ProdSchDate.Split(" ")[0]),
                    Item.ParentProdSchNo == null ? "" : Item.ParentProdSchNo,
                    Item.ParentProdSchEntryId == 0 ? 0 : Item.ParentProdSchEntryId,
                    Item.ParentProdSchYearCode == 0 ? 0 : Item.ParentProdSchYearCode ,
                    Item.ParentProdSchDate == null ? string.Empty : ParseFormattedDate(Item.ParentProdSchDate.Split(" ")[0]),
                    Item.PRODSCHFGItemCode==0?0:Item.PRODSCHFGItemCode,
                    Item.IssueItemCode==0?0:Item.IssueItemCode,
                    Item.StoreId==0?0:Item.StoreId,
                    Item.BatchNo == null ? "" : Item.BatchNo,
                    Item.UniqueBatchNo == null ? "" : Item.UniqueBatchNo,
                    Item.ToatlStock==0?0:Item.ToatlStock,
                    Item.BatchStock==0?0:Item.BatchStock,
                    Item.WIPStock == 0 ? 0 : Item.WIPStock,
                    Item.StdPacking == 0 ? 0 : Item.StdPacking,
                    Item.MaxIssueQty == 0 ? 0 : Item.MaxIssueQty,
                    Item.IssueQty==0 ? 0:Item.IssueQty,
                    Item.Unit == null ? "" : Item.Unit,
                    Item.AltQty == 0 ? 0:Item.AltQty,
                    Item.AltUnit == null ? "": Item.AltUnit,
                    Item.PreIssueAltQty == 0 ? 0 : Item.PreIssueAltQty,
                    Item.PreIssueAltQty == 0 ? 0 : Item.PreIssueAltQty,
                    Item.BomNo == 0 ? 0 : Item.BomNo,
                    Item.BomQty == 0 ? 0 : Item.BomQty,
                    Item.WIPMaxQty == 0 ? 0 : Item.WIPMaxQty,
                    Item.InProcessQCEntryId == 0 ? 0 : Item.InProcessQCEntryId,
                    Item.InprocessQCSlipNo == null ? "" : Item.InprocessQCSlipNo,
                    Item.InProcessQCYearCode == 0 ? 0 : Item.InProcessQCYearCode,
                    Item.InProcessQCDate == null ? string.Empty : ParseFormattedDate(Item.InProcessQCDate.Split(" ")[0]),
                    Item.ItemHasSubBom == null ? "" : Item.ItemHasSubBom,
                    Item.MaterialIsIssuedDirectlyFrmWC == null ? "" : Item.MaterialIsIssuedDirectlyFrmWC,
                    Item.IssuedFromWC == 0 ? 0 : Item.IssuedFromWC,
                    Item.IssueFrmWCSlipNo == null ? "" : Item.IssueFrmWCSlipNo,
                    Item.IssueFrmWCYearCode == 0 ? 0 : Item.IssueFrmWCYearCode,
                    Item.IssueFrmWCDate == null ? string.Empty : ParseFormattedDate(Item.IssueFrmWCDate.Split(" ")[0]),
                    Item.IssueFrmQCorTransferForm == null ? "" : Item.IssueFrmQCorTransferForm,
                    Item.Remark == null ? "" : Item.Remark,
                    Item.otherdetail1 == null ? "" : Item.otherdetail1,
                    Item.otherdetail2 == null ? "" : Item.otherdetail2,
                    Item.Rate==0?0:Item.Rate,
                    Item.ItemColor == null ? "" : Item.ItemColor,
                    Item.ItemWeight == 0 ? 0:Item.ItemWeight,
                    1,
                    Item.IssuedAlternateItem == 0 ? 0 : Item.IssuedAlternateItem,
                    Item.OriginalItemCode == 0 ? 0 : Item.OriginalItemCode
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
        public async Task<JsonResult> FillEntryId(int YearCode)
        {
            var JSON = await _IIssueAgainstProdSchedule.FillEntryId("NewEntryId", YearCode, "SP_IssueAgainstProdSchedule");
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillWorkCenter()
        {
            var JSON = await _IIssueAgainstProdSchedule.FillWorkCenter();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillIssueByEmp()
        {
            var JSON = await _IIssueAgainstProdSchedule.FillIssueByEmp();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillRecByEmp()
        {
            var JSON = await _IIssueAgainstProdSchedule.FillRecByEmp();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetIsStockable(int ItemCode)
        {
            var JSON = await _IIssueAgainstProdSchedule.GetIsStockable(ItemCode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> DisplayRoutingDetail(int ItemCode)
        {
            var JSON = await _IIssueAgainstProdSchedule.DisplayRoutingDetail(ItemCode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillBatchNo(int ItemCode, int YearCode, string StoreName, string BatchNo, string IssuedDate, string FinStartDate)
        {
            if (StoreName == default)
            {
                StoreName = "";
            }
            if (FinStartDate == default)
            {
                FinStartDate = "";
            }
            var JSON = await _IIssueAgainstProdSchedule.FillBatchNo(ItemCode, YearCode, StoreName, BatchNo, IssuedDate, FinStartDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public IActionResult FillThrGridFromMemoryCache()
        {
            try
            {
                _MemoryCache.TryGetValue("KeyIssAgainstProduction", out IList<IssueAgainstProdScheduleDetail> IssueAgainstProdScheduleDetail);
                var MainModel = new IssueAgainstProdSchedule();
                var IssueGrid = new List<IssueAgainstProdScheduleDetail>();
                var SSGrid = new List<IssueAgainstProdScheduleDetail>();
                MainModel.FromDate = HttpContext.Session.GetString("FromDate");
                MainModel.ToDate = HttpContext.Session.GetString("ToDate");
                MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                    SlidingExpiration = TimeSpan.FromMinutes(55),
                    Size = 1024,
                };
                var seqNo = 1;
                if (IssueAgainstProdScheduleDetail != null)
                {
                    for (int i = 0; i < IssueAgainstProdScheduleDetail.Count; i++)
                    {


                        if (IssueAgainstProdScheduleDetail[i] != null)
                        {
                            IssueAgainstProdScheduleDetail[i].seqno = seqNo++;
                            SSGrid.AddRange(IssueGrid);
                            IssueGrid.Add(IssueAgainstProdScheduleDetail[i]);

                            MainModel.ItemDetailGrid = IssueGrid;
                            MainModel.StoreId=IssueAgainstProdScheduleDetail[i].StoreId;
                            MainModel.StoreName=IssueAgainstProdScheduleDetail[i].StoreName;
                            _MemoryCache.Set("KeyIssAgainstProduction", MainModel.ItemDetailGrid, cacheEntryOptions);
                        }
                    }
                }

                return PartialView("_IssueAgainstProductionSchedule", MainModel);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IActionResult EditFromMemoryGrid(int SeqNo, string Mode)
        {
            IList<IssueAgainstProdScheduleDetail> IssueAgainstProdScheduleDetail = new List<IssueAgainstProdScheduleDetail>();
            if (Mode == "U")
            {
                _MemoryCache.TryGetValue("KeyIssueAgainstProdScheduleGrid", out IssueAgainstProdScheduleDetail);
            }
            else
            {
                _MemoryCache.TryGetValue("KeyIssueAgainstProdScheduleGrid", out IssueAgainstProdScheduleDetail);
            }
            IEnumerable<IssueAgainstProdScheduleDetail> SSGrid = IssueAgainstProdScheduleDetail;
            if (IssueAgainstProdScheduleDetail != null)
            {
                SSGrid = IssueAgainstProdScheduleDetail.Where(x => x.seqno == SeqNo);
            }
            string JsonString = JsonConvert.SerializeObject(SSGrid);
            return Json(JsonString);
        }
        public IActionResult DeleteItemRow(int SeqNo, string Mode)
        {
            var MainModel = new IssueAgainstProdSchedule();
            if (Mode == "U")
            {
                _MemoryCache.TryGetValue("KeyIssueAgainstProdScheduleGrid", out List<IssueAgainstProdScheduleDetail> IssueAgainstProdScheduleDetail);
                int Indx = Convert.ToInt32(SeqNo) - 1;

                if (IssueAgainstProdScheduleDetail != null && IssueAgainstProdScheduleDetail.Count > 0)
                {
                    IssueAgainstProdScheduleDetail.RemoveAt(Convert.ToInt32(Indx));

                    Indx = 0;

                    foreach (var item in IssueAgainstProdScheduleDetail)
                    {
                        Indx++;
                        item.seqno = Indx;
                    }
                    MainModel.ItemDetailGrid = IssueAgainstProdScheduleDetail;

                    MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
                    {
                        AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                        SlidingExpiration = TimeSpan.FromMinutes(55),
                        Size = 1024,
                    };

                    _MemoryCache.Set("KeyIssueAgainstProdScheduleGrid", MainModel.ItemDetailGrid, cacheEntryOptions);
                }
            }
            else
            {
                _MemoryCache.TryGetValue("KeyIssueAgainstProdScheduleGrid", out List<IssueAgainstProdScheduleDetail> IssueAgainstProdScheduleDetail);
                int Indx = Convert.ToInt32(SeqNo) - 1;

                if (IssueAgainstProdScheduleDetail != null && IssueAgainstProdScheduleDetail.Count > 0)
                {
                    IssueAgainstProdScheduleDetail.RemoveAt(Convert.ToInt32(Indx));

                    Indx = 0;

                    foreach (var item in IssueAgainstProdScheduleDetail)
                    {
                        Indx++;
                        item.seqno = Indx;
                    }
                    MainModel.ItemDetailGrid = IssueAgainstProdScheduleDetail;

                    MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
                    {
                        AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                        SlidingExpiration = TimeSpan.FromMinutes(55),
                        Size = 1024,
                    };

                    _MemoryCache.Set("KeyIssueAgainstProdScheduleGrid", MainModel.ItemDetailGrid, cacheEntryOptions);
                }
            }

            return PartialView("_IssueAgainstProductionScheduleGrid", MainModel);
        }
        public IActionResult DeleteFromMemoryGrid(int SeqNo)
        {
            var MainModel = new IssueAgainstProdSchedule();
            _MemoryCache.TryGetValue("KeyIssAgainstProduction", out List<IssueAgainstProdScheduleDetail> IssueAgainstProdScheduleDetail);
            int Indx = Convert.ToInt32(SeqNo) - 1;

            if (IssueAgainstProdScheduleDetail != null && IssueAgainstProdScheduleDetail.Count > 0)
            {
                IssueAgainstProdScheduleDetail.RemoveAt(Convert.ToInt32(Indx));

                Indx = 0;

                foreach (var item in IssueAgainstProdScheduleDetail)
                {
                    Indx++;
                    item.seqno = Indx;
                }
                MainModel.ItemDetailGrid = IssueAgainstProdScheduleDetail;

                MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                    SlidingExpiration = TimeSpan.FromMinutes(55),
                    Size = 1024,
                };


                if (IssueAgainstProdScheduleDetail.Count == 0)
                {
                    _MemoryCache.Remove("KeyIssAgainstProduction");
                }
                _MemoryCache.Set("KeyIssAgainstProduction", MainModel.ItemDetailGrid, cacheEntryOptions);
            }
            return PartialView("_IssueAgainstProductionSchedule", MainModel);
        }
        public async Task<JsonResult> DisplayBomDetail(int ItemCode, float WOQty,int BomNo=1)
        {
            var JSON = await _IIssueAgainstProdSchedule.DisplayBomDetail(ItemCode, WOQty,BomNo);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<IActionResult> DeleteByID(int ID, string Mode, string ProdSchSlipNo, int YC, string FromDate = "", string ToDate = "", string TransferSlipNo = "", string ItemName = "", string PartCode = "", string FromWorkCenter = "", string ToWorkCenter = "", string StoreName = "", string ProdSlipNo = "", string ProdSchNo = "", string Searchbox = "", string DashboardType = "")
        {
            var Result = await _IIssueAgainstProdSchedule.DeleteByID(ID, YC, ProdSchSlipNo);

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

            return RedirectToAction("IssueAgainstProdScheduleDashboard", new { Flag = "False", FromDate = FromDate, ToDate = ToDate, TransferSlipNo = TransferSlipNo, ItemName = ItemName, PartCode = PartCode, FromWorkCenter = FromWorkCenter, ToWorkCenter = ToWorkCenter, StoreName = StoreName, ProdSlipNo = ProdSlipNo, ProdSchNo = ProdSchNo, DashboardType = DashboardType });
        }
        public async Task<IActionResult> GetSearchData(string FromDate = "", string IssueAgainstSlipNo = "", string ToDate = "", string IssueFromStore = "", string ItemName = "", string PartCode = "", string Flag = "True", string ProdPlanNo = "", string ProdSchNo = "", string Searchbox = "", string DashboardType = "")
        {
            var model = new IssueAgainstProdScheduleDashboard();
            model = await _IIssueAgainstProdSchedule.GetSearchData(FromDate, IssueAgainstSlipNo, ToDate, IssueFromStore, ItemName, PartCode, ProdPlanNo, ProdSchNo, DashboardType);
            if (DashboardType == "SUMMARY")
            {
                model.DashboardType = "SUMMARY";
                return PartialView("_IssueAgainstProductionScheduleDashboard", model);
            }
            if (DashboardType == "DETAIL")
            {
                model.DashboardType = "DETAIL";
                return PartialView("_IssueAgainstProductionScheduleDashboard", model);
            }
            return View(model);
        }
        public async Task<IActionResult> IssueAgainstProdScheduleDashboard(string FromDate = "", string ToDate = "", string IssAgtProdSchSlipNo = "", string IssueFromStore = "", string ProdPlanNo = "", string Flag = "True", string TransferSlipNo = "", string ItemName = "", string PartCode = "", string FromWorkCenter = "", string ToWorkCenter = "", string StoreName = "", string ProdSlipNo = "", string ProdSchNo = "", string Searchbox = "", string DashboardType = "")
        {
            try
            {
                _MemoryCache.Remove("KeyIssueAgainstProdScheduleGrid");

                var model = new IssueAgainstProdScheduleDashboard();
                var Result = await _IIssueAgainstProdSchedule.GetSummaryDetail(FromDate, ToDate, IssAgtProdSchSlipNo, IssueFromStore, PartCode, ItemName, ProdPlanNo, ProdSchNo, "SUMMARY").ConfigureAwait(true);
                DateTime now = DateTime.Now;

                model.FromDate = new DateTime(now.Year, now.Month, 1).ToString("dd/MM/yyyy").Replace("-", "/");
                model.ToDate = new DateTime(DateTime.Today.Year + 1, 3, 31).ToString("dd/MM/yyyy").Replace("-", "/");

                if (Result != null)
                {
                    var _List = new List<TextValue>();
                    DataSet DS = Result.Result;
                    if (DS != null)
                    {
                        var DT = DS.Tables[0].DefaultView.ToTable(true, "IssAgtProdSchEntryId", "IssAgtProdSchYearCode",
                            "IssAgtProdSchEntryDate", "IssAgtProdSchSlipNo", "IssAgtProdSchSlipDate", "IssuedByEmp", "IssuedByEmpId",
                            "ReceivedByEmp", "ReceivedByEmpId", "AcknowledgedByProd", "AckDate", "ActualEntryDate", "ActualEntryBy",
                            "ActualEntryByEmp","UpdatedByEmp", "LastUpdatedBy","LastUpdatedDate", "CC", "MachineName", "UID", "IssuedFromStoreId"
                             , "IssueFromStore" );
                        model.IssueAgainstProdScheduleDashboard = CommonFunc.DataTableToList<IssueAgainstProductionScheduleDashboard>(DT, "IssueAgainstProdSchedule");

                    }
                    if (Flag != "True")
                    {
                        //model.FromDate1 = FromDate;
                        //model.ToDate1 = ToDate;
                        //model.TransferMatSlipNo=TransferSlipNo;
                        //model.ItemName=ItemName;
                        //model.PartCode=PartCode;
                        //model.TransferFromWC=FromWorkCenter;
                        //model.TransferToWC=ToWorkCenter;
                        //model.TransferToStore=StoreName;
                        //model.ProdSlipNo=ProdSlipNo;
                        //model.ProdSchNo=ProdSchNo;
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
        public async Task<IActionResult> FillLotandTotalStock(int ItemCode, int StoreId, string TillDate, string BatchNo, string UniqBatchNo)
        {
            var JSON = await _IIssueAgainstProdSchedule.FillLotandTotalStock(ItemCode, StoreId, TillDate, BatchNo, UniqBatchNo);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public IActionResult AddtoIssueAgainstProductionSchedule(List<IssueAgainstProdScheduleDetail> model)
        {
            try
            {
                var MainModel = new IssueAgainstProdSchedule();
                var IssueProdSchedule = new List<IssueAgainstProdScheduleDetail>();
                var IssueGrid = new List<IssueAgainstProdScheduleDetail>();
                var SSGrid = new List<IssueAgainstProdScheduleDetail>();
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

                        var isStockable = _IIssueAgainstProdSchedule.GetIsStockable(item.ProdPlanFGItemCode);
                        var stockable = isStockable.Result.Result.Rows[0].ItemArray[0];
                        _MemoryCache.TryGetValue("KeyIssueAgainstProdScheduleGrid", out IList<IssueAgainstProdScheduleDetail> IssueAgainstProdScheduleDetail);
                        if (item != null)
                        {
                            if (IssueAgainstProdScheduleDetail == null)
                            {
                                if (stockable == "Y")
                                {
                                    if (item.ToatlStock <= 0 || item.ToatlStock <= 0)
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
                                    if (item.ToatlStock <= 0 || item.ToatlStock <= 0)
                                    {
                                        return StatusCode(203, "Stock can't be zero");
                                    }
                                }
                                item.seqno = IssueAgainstProdScheduleDetail.Count + 1;
                                IssueGrid = IssueAgainstProdScheduleDetail.Where(x => x != null).ToList();
                                SSGrid.AddRange(IssueGrid);
                                IssueGrid.Add(item);
                            }
                            MainModel.ItemDetailGrid = IssueGrid;

                            _MemoryCache.Set("KeyIssueAgainstProdScheduleGrid", MainModel.ItemDetailGrid, cacheEntryOptions);
                        }
                    }
                }
                _MemoryCache.Remove("KeyIssueAgainstProdSchedule");
                return PartialView("_IssueAgainstProductionScheduleGrid", MainModel);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
