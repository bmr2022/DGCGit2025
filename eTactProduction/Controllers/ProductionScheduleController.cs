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
    public class ProductionScheduleController : Controller
    {
        private readonly IDataLogic _IDataLogic;
        //private readonly IGateInward _IGateInward;
        public IProductionSchedule _IProductionSchedule { get; }

        private readonly ILogger<ProductionScheduleController> _logger;
        private readonly IMemoryCache _MemoryCache;
        public IWebHostEnvironment IWebHostEnvironment { get; }

        public ProductionScheduleController(ILogger<ProductionScheduleController> logger, IDataLogic iDataLogic, IProductionSchedule iProductionSchedule, IMemoryCache iMemoryCache, EncryptDecrypt encryptDecrypt, IWebHostEnvironment iWebHostEnvironment)
        {
            _logger = logger;
            _IDataLogic = iDataLogic;
            _IProductionSchedule = iProductionSchedule;
            _MemoryCache = iMemoryCache;
            IWebHostEnvironment = iWebHostEnvironment;
        }

           //<input asp-for="FromDateBack" type="hidden" value="@Model.FromDateBack" />
           //                 <input asp-for="ToDateBack" type="hidden" value="@Model.ToDateBack" />
           //                 <input asp-for="PartCodeBack" type="hidden" value="@Model.PartCodeBack" />
           //                 <input asp-for="ItemNameBack" type="hidden" value="@Model.ItemNameBack" />
           //                 <input asp-for="AccountNameBack" type="hidden" value="@Model.AccountNameBack" />
           //                 <input asp-for="ProdSchNoBack" type="hidden" value="@Model.ProdSchNoBack" />
           //                 <input asp-for="WONOBack" type="hidden" value="@Model.WONOBack" />
           //                 <input asp-for="SummaryDetailBack" type="hidden" value="@Model.SummaryDetailBack" />
           //                 <input asp-for="SearchBoxBack" type="hidden" value="@Model.SearchBoxBack" />

        [HttpGet]
        [Route("{controller}/Index")]
        public async Task<IActionResult> ProductionSchedule(int ID, string Mode, int YC,string fromDate="",string toDate="",string partCode = "",string itemName="",string accountName="",string prodSchNo ="",string wono="",string summaryDetail = "",string searchBox ="")
        {
            ProductionScheduleModel model = new ProductionScheduleModel();
            ViewData["Title"] = "Production Schdeule Details";
            TempData.Clear();
            _MemoryCache.Remove("KeyProductionScheduleGrid");
            _MemoryCache.Remove("KeyProdPlanGrid");
            _MemoryCache.Remove("KeyBomChildDetailGrid");
            _MemoryCache.Remove("KeyBomChildSummaryGrid");
            _MemoryCache.Remove("KeyAdjustedQty");

            // var model = await BindModel(MainModel);

            if (model.Mode != "U")
            {
                model.UID = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                model.ActualEntryBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                model.ActualEntryByName = HttpContext.Session.GetString("EmpName");
            }

            if (!string.IsNullOrEmpty(Mode) && ID > 0 && (Mode == "V" || Mode == "U"))
            {
                model = await _IProductionSchedule.GetViewByID(ID, Mode, YC);
                model.Mode = Mode;
                model.ID = ID;
                model.EffectiveFrom = Convert.ToDateTime(model.EffectiveFrom).ToString("dd/MM/yyyy");
                model.EffectiveTo = Convert.ToDateTime(model.EffectiveTo).ToString("dd/MM/yyyy");
                
            }

            MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                SlidingExpiration = TimeSpan.FromMinutes(55),
                Size = 1024,
            };
            model.FinFromDate = HttpContext.Session.GetString("FromDate");
            model.FinToDate = HttpContext.Session.GetString("ToDate");
            model.YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
            model.CC = HttpContext.Session.GetString("Branch");
            _MemoryCache.Set("KeyProductionScheduleGrid", model.ProductionScheduleDetails, cacheEntryOptions);
            _MemoryCache.Set("KeyProdPlanGrid", model.prodPlanDetails, cacheEntryOptions);
            _MemoryCache.Set("KeyBomChildDetailGrid", model.BomDatamodel?.BomDetails, cacheEntryOptions);
            _MemoryCache.Set("KeyBomChildSummaryGrid", model.BomDatamodel?.BomSummaries, cacheEntryOptions);
            //int ID, string Mode, int YC,string fromDate = "",string toDate = "",string partCode = "",string itemName = "",string accountName = "",string prodSchNo = "",string wono = "",string summaryDetail = "",string searchBox = ""
            model.PartCodeBack = partCode;
            model.ItemNameBack = itemName;
            model.AccountNameBack = accountName;
            model.ProdSchNoBack = prodSchNo;
            model.WONOBack = wono;
            model.SummaryDetailBack = wono;
            model.SearchBoxBack = searchBox;

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddPendingProdPlans([FromForm] int yearCode, string schFromDate, string schTillDate, string displayFlag, int noOfDays, List<ProductionScheduleDetail> prodPlanList)
        {
            try
            {
                var PendingProdPlans = new DataTable();
                PendingProdPlans = GetProdPlanDetailTable(prodPlanList);
                //var PendingProdPlans = new DataTable();
                //PendingProdPlans = GetProdInputDetailTable(prodPlanInputList);

                schFromDate = ParseFormattedDate(schFromDate);
                schTillDate = ParseFormattedDate(schTillDate);

                var model = await _IProductionSchedule.AddPendingProdPlans(yearCode, schFromDate, schTillDate, displayFlag, noOfDays, PendingProdPlans);
                MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                    SlidingExpiration = TimeSpan.FromMinutes(55),
                    Size = 1024,
                };
                List<ProductionScheduleProdPlanDetail> prodPlans = new();
                foreach (var item in prodPlanList)
                {
                    ProductionScheduleProdPlanDetail prodPlanDetail = new ProductionScheduleProdPlanDetail
                    {
                        PlanNo = item.WONo,
                        PlanNoEntryId = item.WOEntryId,
                        PlanNoYearCode = item.WOYearCode,
                        PlanNoDate = item.WODate,
                        SONO = item.SONo,
                        CustOrderNo = item.CustOrderNo,
                        SOEntryId = item.SOEntryId,
                        SOYearCode = item.SOYearCode,
                        AccountCode = item.AccountCode,
                        //WOEffectiveFrom = item.EffectiveFrom,
                        //WOEndDate = item
                        //SaleSchNo = item.SchNo,
                        //SaleSchEntryId = item.schEntryId,
                        //SaleSchYearCode = item.SchYearCode,
                        SaleSchDate = item.SchDate
                    };
                    prodPlans.Add(prodPlanDetail);
                }

                model.prodPlanDetails = prodPlans;
                _MemoryCache.Set("KeyProdPlanGrid", model.prodPlanDetails, cacheEntryOptions);
                _MemoryCache.Set("KeyProductionScheduleGrid", model.ProductionScheduleDetails, cacheEntryOptions);
                return PartialView("_ProductionScheduleGrid", model);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        //public async Task<IActionResult> DeleteByID(int ID, int YC, int createdBy, string entryByMachineName)
        //{
        //    return View();
        //}

        public async Task<JsonResult> GetFormRights()
        {
            var userID = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
            var JSON = await _IProductionSchedule.GetFormRights(userID);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<IActionResult> DeleteByID(int ID, int YC, int createdBy, string entryByMachineName,int ActualEntryBy,string EntryDate,string FromDate,string ToDate,string PartCode,string ItemName,string AccountName,string ProdSchNo,string WONo,string SummaryDetail,string SearchBox)
        {
            var Result = await _IProductionSchedule.DeleteByID(ID, YC, createdBy, entryByMachineName, ActualEntryBy, EntryDate).ConfigureAwait(false);

            if (Result.StatusText == "Success" || Result.StatusCode == HttpStatusCode.Gone)
            {
                ViewBag.isSuccess = true;
                TempData["410"] = "410";
            }
            else
            {
                ViewBag.isSuccess = false;
                TempData["500"] = "500";
            }

            return RedirectToAction("PSDashboard",new {flag= "False",fromDate =  FromDate,toDate =  ToDate,partCode = PartCode,itemName =  ItemName,accountName =  AccountName,prodSchNo = ProdSchNo,wono = WONo,summaryDetail =  SummaryDetail,searchBox = SearchBox });
        }

        public async Task<IActionResult> ReloadDelete()
        {
            _MemoryCache.TryGetValue("KeyProductionScheduleGrid", out List<ProductionScheduleDetail> PSDetailGrid);
            var model = new ProductionScheduleModel();
            model.ProductionScheduleDetails = PSDetailGrid;
            return Json(model);
        }
        public async Task<IActionResult> PSBomDetail(int YearCode)
        {
            _MemoryCache.TryGetValue("KeyAdjustedQty", out List<ProductionScheduleDetail> PSDetailGrid);
            DataTable? itemGrid = new();
            if (PSDetailGrid != null)
            {
                itemGrid = GetDetailTable(PSDetailGrid);
            }
            var mainModel = new ProductionScheduleModel();
            var model = _IProductionSchedule.PSBomDetail(YearCode, itemGrid);
            mainModel = model.Result;
            MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                SlidingExpiration = TimeSpan.FromMinutes(55),
                Size = 1024,
            };
            
            _MemoryCache.Set("KeyBomChildDetailGrid", mainModel.BomDatamodel?.BomDetails, cacheEntryOptions);
            return PartialView("_PSBOMDetail", mainModel);
        }
        public async Task<IActionResult> PSBomSummary(int YearCode)
        {
            _MemoryCache.TryGetValue("KeyAdjustedQty", out List<ProductionScheduleDetail> PSDetailGrid);
            DataTable? itemGrid = new();
            if (PSDetailGrid != null) {
                itemGrid = GetDetailTable(PSDetailGrid);
            }
            var mainModel = new ProductionScheduleModel();
            var model = _IProductionSchedule.PSBomDetail(YearCode, itemGrid);
            mainModel = model.Result;
            MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                SlidingExpiration = TimeSpan.FromMinutes(55),
                Size = 1024,
            };
            _MemoryCache.Set("KeyBomChildSummaryGrid", mainModel.BomDatamodel?.BomSummaries, cacheEntryOptions);
            return PartialView("_PSBomSummary", mainModel);
        }
        public async Task<JsonResult> GetPendProduction()
        {
            _MemoryCache.TryGetValue("KeyProductionScheduleGrid", out IList<ProductionScheduleDetail> PSDetail);
            string JsonString = JsonConvert.SerializeObject(PSDetail);
            return Json(JsonString);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("{controller}/Index")]
        public async Task<IActionResult> ProductionSchedule(ProductionScheduleModel model)
        {
            try
            {
                var ProductionScheduleDetail = new DataTable();
                var BomChildDetailDT = new DataTable();
                var BomSummaryDetailDT = new DataTable();
                var prodPlanDetail = new DataTable();

                _MemoryCache.TryGetValue("KeyProductionScheduleGrid", out List<ProductionScheduleDetail> PSDetailGrid);
                _MemoryCache.TryGetValue("KeyBomChildDetailGrid", out List<ProductionScheduleBomDetail> bomChildDetail);
                _MemoryCache.TryGetValue("KeyBomChildSummaryGrid", out List<ProductionScheduleBomSummary> bomSummaryDetail);
                _MemoryCache.TryGetValue("KeyProdPlanGrid", out List<ProductionScheduleProdPlanDetail> prodPlanGrid);
                _MemoryCache.TryGetValue("KeyAdjustedQty", out List<ProductionScheduleDetail> AdjustedDetails);
                if (prodPlanDetail == null)
                {
                    ModelState.Clear();
                    ModelState.TryAddModelError("ProductionScheduleGrid", "ProductionSchedule Grid Should Have Atleast 1 Item...!");
                    return View("ProductionSchedule", model);
                }
                else if (AdjustedDetails == null)
                {
                    ModelState.Clear();
                    ModelState.TryAddModelError("ProductionScheduleGrid", "Please adjust qty first..!");
                    return View("ProductionSchedule", model);
                }
                else
                {
                    model.ActualEntryBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                    //ProductionScheduleDetail = GetDetailTable(PSDetailGrid);
                    ProductionScheduleDetail = GetDetailTable(AdjustedDetails);
                    
                    BomChildDetailDT = bomChildDetail == null ? new DataTable() : GetBomChildDetailTable(bomChildDetail);
                    BomSummaryDetailDT = bomSummaryDetail == null ? new DataTable() : GetBomSummaryTable(bomSummaryDetail);
                    if (prodPlanGrid != null)
                    {
                        prodPlanDetail = GetProdPlansDetailTable(prodPlanGrid);
                    }

                    if (model.Mode == "U")
                    {
                        model.LastUpdatedByName = HttpContext.Session.GetString("EmpName");
                        model.UID = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                        model.LastUpdatedBy = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
                    }
                    var Result = await _IProductionSchedule.SaveProductionSchedule(model, ProductionScheduleDetail, prodPlanDetail, BomChildDetailDT, BomSummaryDetailDT);

                    if (Result != null)
                    {
                        if (Result.StatusText == "Success" && Result.StatusCode == HttpStatusCode.OK)
                        {
                            ViewBag.isSuccess = true;
                            TempData["200"] = "200";
                            var modelSuccess = new ProductionScheduleModel();
                            return RedirectToAction("ProductionSchedule", modelSuccess);
                        }
                        if (Result.StatusText == "Updated" && Result.StatusCode == HttpStatusCode.Accepted)
                        {
                            ViewBag.isSuccess = true;
                            TempData["202"] = "202";
                            var modelUpdate = new ProductionScheduleModel();
                            return RedirectToAction("ProductionSchedule", modelUpdate);
                        }
                        //if (Result.StatusText == "Error" && Result.StatusCode == HttpStatusCode.InternalServerError)
                        //{
                        //    ViewBag.isSuccess = false;
                        //    TempData["500"] = "500";
                        //    _logger.LogError("\n \n ******** LogError ******** \n " + JsonConvert.SerializeObject(Result) + "\n \n");
                        //    return View("Error", Result);
                        //}
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
                            _MemoryCache.TryGetValue("KeyProductionScheduleGrid", out List<ProductionScheduleDetail> PSGridDetails);
                            model.ProductionScheduleDetails = AdjustedDetails;
                            ModelState.Clear();
                            return View(model);
                        }
                    }
                    _MemoryCache.TryGetValue("KeyProductionScheduleGrid", out List<ProductionScheduleDetail> PSDetail);
                    model.ProductionScheduleDetails = PSDetail;
                    ModelState.Clear();
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                LogException<ProductionScheduleController>.WriteException(_logger, ex);

                var ResponseResult = new ResponseResult()
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    StatusText = "Error",
                    Result = ex
                };


                return View("Error", ResponseResult);
            }
        }
        public IActionResult ClearProductionSchGrid()
        {
            _MemoryCache.Remove("KeyProdPlanGrid");
            _MemoryCache.Remove("KeyProductionScheduleGrid");
            var MainModel = new ProductionScheduleModel();
            return PartialView("_ProductionScheduleGrid", MainModel);
        }
        public IActionResult ClearBomGrid()
        {
            _MemoryCache.Remove("KeyBomChildSummaryGrid");
            _MemoryCache.Remove("KeyBomChildDetailGrid");
            var MainModel = new ProductionScheduleModel();
            return PartialView("_PSBomSummary", MainModel);
        }
        public async Task<IActionResult> GetSearchData(string summaryDetail, string partCode, string itemName, string accountName, string fromDate, string toDate)
        {
            var model = new ProductionScheduleDashboard();
            model.ActualEntryBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
            model.EntryDate = DateTime.Now.ToString();
            model.YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
            var Result = await _IProductionSchedule.GetDashboardData(partCode, itemName, accountName, fromDate, toDate, model.YearCode).ConfigureAwait(true);
            if (Result != null)
            {
                var _List = new List<TextValue>();
                DataSet DS = Result.Result;
                if (DS != null)
                {
                    var DT = DS.Tables[0].DefaultView.ToTable(true, "Entrydate", "WCID", "ProdSchNo", "EffectiveFrom", "EffectiveTo",
                            "RevNo", "Revdate", "CC", "UID", "actualEntryDate", "ActualEntryBy", "LastUpdateDate", "EntryByMachineName",
                            "Closed", "Completed", "ForTheMonth", "Remark", "ShowWOWithOrWOItem", "FromSchDate", "ToSchDate", "PlanForNoOFDays", "EntryID",
                            "YearCode", "ItemCode", "PartCode", "ItemName", "BOMNO", "BOMEffDate", "SchDate", "ShiftID", "ShiftName",
                            "ProdInWC", "WorkCenter", "Qty", "Originalqty", "PlannedMachineid1", "PlannedMachineid2", "PlannedMachineid3", "SchForTheDate",
                            "PlanNo", "PlanNoYearCode", "PlanNoDate", "CustOrderNo", "SOYearCode", "SODate", "SubBOM", "InhouseJOBProd",
                            "DrawingNo", "RemarkItem", "ProdCompleted", "ProdCanceled", "QCMandatory", "ProdSeq",
                            "AccountCode", "SaleSchNo", "SaleSchDate", "SaleSchYearCode", "ProdSchDate", "ActualEntryByName",
                             "LastUpdatedByName", "LastUpdatedDate", "LastUpdatedBy", "PRODPENDQTY", "TotalWOQty", "SONO", "AccountName");
                    model.productionScheduleDashboards = CommonFunc.DataTableToList<ProductionScheduleDashboard>(DT, "ProductionSchTable");
                    if (summaryDetail == "Summary")
                    {
                        model.productionScheduleDashboards = model.productionScheduleDashboards
                            .GroupBy(psd => psd.EntryID)
                            .Select(group => group.First())
                            .ToList();
                    }

                }
            }
            model.SummaryDetail = summaryDetail;
            return PartialView("_PSDashboardGrid", model);
        }
        public async Task<IActionResult> PSDashboard(string fromDate,string toDate,string partCode,string itemName ,string accountName,string prodSchNo,string wono,string summaryDetail,string searchBox, string flag = "True")
        {
            try
            {
                _MemoryCache.Remove("KeyProductionScheduleGrid");
                var model = new ProductionScheduleDashboard();

                var FromDt = HttpContext.Session.GetString("FromDate");
                model.FinFromDate = ParseFormattedDate(FromDt);
                    //Convert.ToDateTime(FromDt).ToString("dd/MM/yyyy");
                DateTime ToDate = DateTime.Today;
                model.FinToDate = ParseFormattedDate( ToDate.ToString());

                model.YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
                var Result = await _IProductionSchedule.GetDashboardData(partCode, itemName, accountName, ParseFormattedDate(model.FinFromDate.Split(" ")[0]), ParseFormattedDate(model.FinToDate.Split(" ")[0]), model.YearCode).ConfigureAwait(true);
                if (Result != null)
                {
                    var _List = new List<TextValue>();
                    DataSet DS = Result.Result;
                    if (DS != null)
                    {
                        var DT = DS.Tables[0].DefaultView.ToTable(true, "Entrydate", "WCID", "ProdSchNo", "EffectiveFrom", "EffectiveTo",
                                "RevNo", "Revdate", "CC", "UID", "actualEntryDate", "ActualEntryBy", "LastUpdateDate", "EntryByMachineName",
                                "Closed", "Completed", "ForTheMonth", "Remark", "ShowWOWithOrWOItem", "FromSchDate", "ToSchDate", "PlanForNoOFDays", "EntryID",
                                "YearCode", "ItemCode", "PartCode", "ItemName", "BOMNO", "BOMEffDate", "SchDate", "ShiftID", "ShiftName",
                                "ProdInWC", "WorkCenter", "Qty", "Originalqty", "PlannedMachineid1", "PlannedMachineid2", "PlannedMachineid3", "SchForTheDate",
                                "PlanNo", "PlanNoYearCode", "PlanNoDate", "CustOrderNo", "SOYearCode", "SODate", "SubBOM", "InhouseJOBProd",
                                "DrawingNo", "RemarkItem", "ProdCompleted", "ProdCanceled", "QCMandatory", "ProdSeq",
                                "AccountCode", "SaleSchNo", "SaleSchDate", "SaleSchYearCode", "ProdSchDate", "ActualEntryByName",
                                 "LastUpdatedByName", "LastUpdatedDate", "LastUpdatedBy", "PRODPENDQTY", "TotalWOQty", "SONO", "AccountName");
                        model.productionScheduleDashboards = CommonFunc.DataTableToList<ProductionScheduleDashboard>(DT, "ProductionSchTable");

                    }
                }
                model.FinFromDate = ParseFormattedDate(fromDate);
                model.FinToDate = ParseFormattedDate( toDate);
                model.PartCode = partCode;
                model.ItemName= itemName;
                model.AccountName = accountName;
                model.ProdSchNo = prodSchNo;
                model.WONo = wono;
                model.SummaryDetail = summaryDetail;
                model.SearchBox = searchBox;
                return View(model);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        private static DataTable GetDetailTable(IList<ProductionScheduleDetail> DetailList)
        {
            var PSGrid = new DataTable();

            PSGrid.Columns.Add("EntryID", typeof(int));
            PSGrid.Columns.Add("YearCode", typeof(int));
            PSGrid.Columns.Add("ItemCode", typeof(int));
            PSGrid.Columns.Add("BOMNO", typeof(int));
            PSGrid.Columns.Add("BOMEffDate", typeof(string));
            PSGrid.Columns.Add("Schdate", typeof(string));
            PSGrid.Columns.Add("ShiftID", typeof(int));
            PSGrid.Columns.Add("ProdInWC", typeof(int));
            PSGrid.Columns.Add("Qty", typeof(float));
            PSGrid.Columns.Add("PRODPENDQTY", typeof(float));
            PSGrid.Columns.Add("Originalqty", typeof(float));
            PSGrid.Columns.Add("TotalWOQty", typeof(float));
            PSGrid.Columns.Add("PlannedMachineid1", typeof(int));
            PSGrid.Columns.Add("PlannedMachineid2", typeof(int));
            PSGrid.Columns.Add("PlannedMachineid3", typeof(int));
            PSGrid.Columns.Add("StartFromTime", typeof(string)); // datetime
            PSGrid.Columns.Add("ToTime", typeof(string)); // datetime
            PSGrid.Columns.Add("WONo", typeof(string));
            PSGrid.Columns.Add("WOEntryId", typeof(int));
            PSGrid.Columns.Add("WOYearCode", typeof(int));
            PSGrid.Columns.Add("WODate", typeof(string));
            PSGrid.Columns.Add("SOEntryId", typeof(int));
            PSGrid.Columns.Add("SONo", typeof(string));
            PSGrid.Columns.Add("CustOrderrNo", typeof(string));
            PSGrid.Columns.Add("SOYearCode", typeof(int));
            PSGrid.Columns.Add("SODate", typeof(string));
            PSGrid.Columns.Add("SubBOM", typeof(string));
            PSGrid.Columns.Add("InhouseJOBProd", typeof(string));
            PSGrid.Columns.Add("DrawingNo", typeof(string));
            PSGrid.Columns.Add("RemarkItem", typeof(string));
            PSGrid.Columns.Add("ProdCompleted", typeof(string));
            PSGrid.Columns.Add("ProdCanceled", typeof(string));
            PSGrid.Columns.Add("QCMandatory", typeof(string));
            PSGrid.Columns.Add("ProdSeq", typeof(int));
            PSGrid.Columns.Add("SchForTheDate", typeof(string));
            PSGrid.Columns.Add("AccountCode", typeof(int));
            PSGrid.Columns.Add("SaleSchNo", typeof(string));
            PSGrid.Columns.Add("SaleSchDate", typeof(string));
            PSGrid.Columns.Add("SaleSchYearCode", typeof(int));

            foreach (var Item in DetailList)
            {
                //string SODt = Item.SODATE;
                //DateTime inputDate = DateTime.Parse(SODt);
                //string formattedDate = inputDate.ToString("yyyy/MM/dd");

                PSGrid.Rows.Add(
                    new object[]
                    {1,2023,
                    Item.ItemCode,
                    Item.BOMNO,
                    Item.BomEffDate == null ? string.Empty : ParseFormattedDate(Item.BomEffDate.Split(" ")[0]),
                    Item.SchDate == null ? string.Empty : ParseFormattedDate(Item.SchDate.Split(" ")[0]),
                    Item.ShiftID,
                    Item.ProdInWC,
                    Item.Qty,
                    Item.ProdPendQty,
                    Item.Originalqty,
                    Item.TotalWOQty,
                    Item.PlannedMachineid1,
                    Item.PlannedMachineid2,
                    Item.PlannedMachineid3,
                    Item.SchDate == null ? string.Empty : ParseFormattedDate(Item.SchDate.Split(" ")[0]),// startTime
                    Item.SchDate == null ? string.Empty : ParseFormattedDate(Item.SchDate.Split(" ")[0]),// ToTime
                    Item.WONo ?? "",
                    Item.WOEntryId,
                    Item.WOYearCode,
                    Item.WODate == null ? string.Empty : ParseFormattedDate(Item.WODate.Split(" ")[0]),
                    Item.SOEntryId,
                    Item.SONo ?? "",
                    Item.CustOrderNo ?? "",
                    Item.SOYearCode,
                    Item.SODate == null ? string.Empty : ParseFormattedDate(Item.SODate.Split(" ")[0]),
                    Item.SubBOM ?? "",
                    Item.InhouseJOBProd ?? "",
                    Item.DrawingNo ?? "",
                    Item.RemarkItem ?? "",
                    Item.ProdCompleted ?? "",
                    Item.ProdCanceled ?? "",
                    Item.QCMandatory ?? "",
                    Item.ProdSeq,
                    Item.SchDate == null ? string.Empty : ParseFormattedDate(Item.SchDate.Split(" ")[0]), //SchForTheDate
                    Item.AccountCode,
                    0,
                    Item.SchDate == null ? string.Empty : ParseFormattedDate(Item.SchDate.Split(" ")[0]), // SchDate
                    2024
                    });
            }
            PSGrid.Dispose();
            return PSGrid;
        }

        private static DataTable GetBomChildDetailTable(IList<ProductionScheduleBomDetail> DetailList)
        {
            try
            {
                var PSGrid = new DataTable();

                PSGrid.Columns.Add("EntryID", typeof(int));
                PSGrid.Columns.Add("YearCode", typeof(int));
                PSGrid.Columns.Add("FGItemCode", typeof(int));
                PSGrid.Columns.Add("FGQty", typeof(float));
                PSGrid.Columns.Add("BOMNO", typeof(int));
                PSGrid.Columns.Add("BOMEffectiveDate", typeof(string));
                PSGrid.Columns.Add("RMItemCode", typeof(int));
                PSGrid.Columns.Add("BOMQTY", typeof(float));
                PSGrid.Columns.Add("RMQTY", typeof(float));
                PSGrid.Columns.Add("PendToIssueFromStore", typeof(float));

                foreach (var Item in DetailList)
                {
                    //string SODt = Item.SODATE;
                    //DateTime inputDate = DateTime.Parse(SODt);
                    //string formattedDate = inputDate.ToString("yyyy/MM/dd");

                    PSGrid.Rows.Add(
                        new object[]
                        {1,2023,
                    Item.FGItemCode,
                    Item.FGQty,
                    Item.BomNo,
                    Item.BomEffDate == null ? string.Empty : ParseFormattedDate((Item.BomEffDate).Split(" ")[0]),
                    Item.RMItemCode,
                    Item.BomQty,
                    Item.ReqQty,
                    Item.PendQty
                        });
                }
                PSGrid.Dispose();
                return PSGrid;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        private static DataTable GetBomSummaryTable(IList<ProductionScheduleBomSummary> DetailList)
        {
            try
            {
                var PSGrid = new DataTable();

                PSGrid.Columns.Add("EntryId", typeof(int));
                PSGrid.Columns.Add("YearCode", typeof(int));
                PSGrid.Columns.Add("RMitemCode", typeof(int));
                PSGrid.Columns.Add("RMQTY", typeof(float));
                PSGrid.Columns.Add("PendQtyToIssueFromStore", typeof(int));
                PSGrid.Columns.Add("MainstoreStock", typeof(float));
                PSGrid.Columns.Add("qcstoreStock", typeof(float));
                PSGrid.Columns.Add("WIPStock", typeof(float));

                foreach (var Item in DetailList)
                {
                    //string SODt = Item.SODATE;
                    //DateTime inputDate = DateTime.Parse(SODt);
                    //string formattedDate = inputDate.ToString("yyyy/MM/dd");

                    PSGrid.Rows.Add(
                        new object[]
                        {1,2023,
                    Item.RMItemCode,
                    Item.TotalReqQty,
                    Item.PendQty,
                    Item.MainStoreStock,
                    Item.QcStoreStock,
                    Item.WIPStock
                        });
                }
                PSGrid.Dispose();
                return PSGrid;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private static DataTable GetProdPlansDetailTable(IList<ProductionScheduleProdPlanDetail> DetailList)
        {
            var PSGrid = new DataTable();

            PSGrid.Columns.Add("EntryId", typeof(int));
            PSGrid.Columns.Add("YearCode", typeof(int));
            PSGrid.Columns.Add("ProdSchNo", typeof(string));
            PSGrid.Columns.Add("WONo", typeof(string));
            PSGrid.Columns.Add("WOEntryId", typeof(int));
            PSGrid.Columns.Add("WOYearCode", typeof(int));
            PSGrid.Columns.Add("WODate", typeof(string));
            PSGrid.Columns.Add("SONO", typeof(string));
            PSGrid.Columns.Add("CustOrderNo", typeof(string));
            PSGrid.Columns.Add("SOEntryId", typeof(int));
            PSGrid.Columns.Add("SOyearCode", typeof(int));
            PSGrid.Columns.Add("AccountCode", typeof(int));
            PSGrid.Columns.Add("WOEffectiveFrom", typeof(string));
            PSGrid.Columns.Add("WOEndDate", typeof(string));
            PSGrid.Columns.Add("seqno", typeof(int));
            PSGrid.Columns.Add("SaleSchNo", typeof(string));
            PSGrid.Columns.Add("SaleSchEntryId", typeof(int));
            PSGrid.Columns.Add("SaleSchYearCode", typeof(int));
            PSGrid.Columns.Add("SaleSchDate", typeof(string));

            foreach (var Item in DetailList)
            {
                PSGrid.Rows.Add(
                    new object[]
                    {1,2023,
                    Item.ProdSchNo ?? "",
                    Item.PlanNo ?? "",
                    Item.PlanNoEntryId,
                    Item.PlanNoYearCode,
                    Item.PlanNoDate == null ? string.Empty : ParseFormattedDate(Item.PlanNoDate.Split(" ")[0]),
                    Item.SONO ?? "",
                    Item.CustOrderNo ?? "",
                    Item.SOEntryId,
                    Item.SOYearCode,
                    Item.AccountCode,
                    Item.WOEffectiveFrom == null ? string.Empty : ParseFormattedDate(Item.WOEffectiveFrom.Split(" ")[0]),
                    Item.WOEndDate  == null ? string.Empty : ParseFormattedDate(Item.WOEndDate.Split(" ")[0]),
                    Item.SeqNo,
                    Item.SaleSchNo ?? "",
                    Item.SaleSchEntryId,
                    Item.SaleSchYearCode,
                    Item.SaleSchDate == null ? string.Empty : ParseFormattedDate(Item.SaleSchDate)
                    });
            }
            PSGrid.Dispose();
            return PSGrid;
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
        [HttpPost]
        public IActionResult ViewProduction(List<ProductionScheduleDetail> model)
        {
            ProductionScheduleModel ProdSchModel = new();
            ProdSchModel.ProductionScheduleDetails = model;
            return PartialView("_ProductionScheduleGrid", ProdSchModel);
        }

        [HttpPost]
        public IActionResult AddProductionScheduleGrid(List<ProductionScheduleDetail> model)
        {
            try
            {
                //_MemoryCache.Remove("KeyProductionScheduleGrid");
                _MemoryCache.Remove("KeyAdjustedQty");
                var PSDetails = new ProductionScheduleModel();
                MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                    SlidingExpiration = TimeSpan.FromMinutes(55),
                    Size = 1024,
                };
                PSDetails.ProductionScheduleDetails = model;
                //_MemoryCache.Set("KeyProductionScheduleGrid", PSDetails.ProductionScheduleDetails, cacheEntryOptions);
                _MemoryCache.Set("KeyAdjustedQty", PSDetails.ProductionScheduleDetails, cacheEntryOptions);
                return Json("Ok");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public IActionResult AddNewItemInGrid([FromBody] List<ProductionScheduleDetail> model)
        {
            if (model == null)
            {
                return BadRequest("Values are null");
            }
            try
            {
                _MemoryCache.Remove("KeyProductionScheduleGrid");
                var PSDetails = new ProductionScheduleModel
                {
                    ProductionScheduleDetails = model
                };

                var cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(60),
                    SlidingExpiration = TimeSpan.FromMinutes(55),
                    Size = 1024,
                };
                _MemoryCache.Set("KeyProductionScheduleGrid", PSDetails.ProductionScheduleDetails, cacheEntryOptions);

                return Json(PSDetails);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the request.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }

        public async Task<JsonResult> GetBomMultiLevelGrid()
        {
            var JSON = await _IProductionSchedule.GetBomMultiLevelGrid();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> NewEntryId(int YearCode)
        {
            var JSON = await _IProductionSchedule.NewEntryId(YearCode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetNewProductionList()
        {
            var model = new ProductionScheduleModel();
            _MemoryCache.TryGetValue("KeyProductionScheduleGrid", out List<ProductionScheduleDetail> PSDetail);
            string JsonString = JsonConvert.SerializeObject(PSDetail);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetItems()
        {
            var JSON = await _IProductionSchedule.GetItems();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetMachineName()
        {
            var JSON = await _IProductionSchedule.GetMachineName();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetWorkCenter()
        {
            var JSON = await _IProductionSchedule.GetWorkCenter();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetPendWOData(string PendWoType, int YearCode, string SOEffFromDate, string CurrentDate)
        {
            SOEffFromDate = ParseFormattedDate(SOEffFromDate);
            CurrentDate = ParseFormattedDate(CurrentDate);
            var JSON = await _IProductionSchedule.GetPendWOData(PendWoType, YearCode, SOEffFromDate, CurrentDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }

        public IActionResult DeleteItemRow(int SeqNo, int itemCode)
        {
            var MainModel = new ProductionScheduleModel();
            _MemoryCache.TryGetValue("KeyProductionScheduleGrid", out List<ProductionScheduleDetail> PSDetail);
            int Indx = Convert.ToInt32(SeqNo) - 1;

            if (PSDetail != null && PSDetail.Count > 0)
            {
                var itemToRemove = PSDetail.Where(x => x.ItemCode == itemCode).ToList();
                if (itemToRemove != null)
                {
                    foreach (var item in itemToRemove)
                    {
                        PSDetail.Remove(item);
                    }
                }
                //PSDetail.RemoveAt(Convert.ToInt32(Indx));
                Indx = 0;

                foreach (var item in PSDetail)
                {
                    Indx++;
                    // item.SequenceNo = Indx;
                }
                MainModel.ProductionScheduleDetails = PSDetail;

                MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                    SlidingExpiration = TimeSpan.FromMinutes(55),
                    Size = 1024,
                };

                _MemoryCache.Set("KeyProductionScheduleGrid", MainModel.ProductionScheduleDetails, cacheEntryOptions);
            }
            return PartialView("_ProductionScheduleGrid", MainModel);
        }

        public IActionResult GetViewQty()
        {
            ProductionScheduleModel model = new();
            MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                SlidingExpiration = TimeSpan.FromMinutes(55),
                Size = 1024,
            };
            _MemoryCache.TryGetValue("KeyProductionScheduleGrid", out IList<ProductionScheduleDetail> PSDetail);
            model.ProductionScheduleDetails = PSDetail.ToList();
            string JsonString = JsonConvert.SerializeObject(model);
            return Json(JsonString);
        }

        private static DataTable GetProdPlanDetailTable(IList<ProductionScheduleDetail> DetailList)
        {
            var PPGrid = new DataTable();

            PPGrid.Columns.Add("PlanNo", typeof(string));
            PPGrid.Columns.Add("PLanEntryId", typeof(int));
            PPGrid.Columns.Add("PlanYearCode", typeof(int));
            PPGrid.Columns.Add("PlanDate", typeof(string));
            PPGrid.Columns.Add("SOEntryId", typeof(int));
            PPGrid.Columns.Add("SONo", typeof(string));
            PPGrid.Columns.Add("CustOrderNo", typeof(string));
            PPGrid.Columns.Add("SOYearCode", typeof(string));
            PPGrid.Columns.Add("SODate", typeof(string));
            PPGrid.Columns.Add("itemCode", typeof(int));
            PPGrid.Columns.Add("AccountCode", typeof(int));

            foreach (var Item in DetailList)
            {
                PPGrid.Rows.Add(
                    new object[]
                    {
                    Item.WONo,
                    Item.WOEntryId,
                    Item.WOYearCode,
                    Item.WODate == null ? string.Empty : ParseDate(Item.WODate),
                    Item.SOEntryId,
                    Item.SONo,
                    Item.CustOrderNo,
                    Item.SOYearCode,
                    Item.SODate == null ? string.Empty : ParseDate(Item.SODate),
                    Item.ItemCode,
                    Item.AccountCode
                    });
            }
            PPGrid.Dispose();
            return PPGrid;
        }
        //private static DataTable GetProdInputDetailTable(IList<ProductionScheduleInputData> DetailList)
        //{
        //    var PPGrid = new DataTable();

        //    PPGrid.Columns.Add("PlanNo", typeof(string));
        //    PPGrid.Columns.Add("PLanEntryId", typeof(int));
        //    PPGrid.Columns.Add("PlanYearCode", typeof(int));
        //    PPGrid.Columns.Add("PlanDate", typeof(string));
        //    PPGrid.Columns.Add("SOEntryId", typeof(int));
        //    PPGrid.Columns.Add("SONo", typeof(string));
        //    PPGrid.Columns.Add("CustOrderNo", typeof(string));
        //    PPGrid.Columns.Add("SOYearCode", typeof(string));
        //    PPGrid.Columns.Add("SODate", typeof(string));
        //    PPGrid.Columns.Add("itemCode", typeof(int));
        //    PPGrid.Columns.Add("AccountCode", typeof(int));

        //    foreach (var Item in DetailList)
        //    {
        //        PPGrid.Rows.Add(
        //            new object[]
        //            {
        //            Item.WONo,
        //            Item.WOEntryId,
        //            Item.WOYearCode,
        //            Item.WODate == null ? string.Empty : ParseDate(Item.WODate),
        //            Item.SOEntryId,
        //            Item.SONo,
        //            Item.CustOrderNo,
        //            Item.SOYearCode,
        //            Item.SODate == null ? string.Empty : ParseDate(Item.SODate),
        //            Item.ItemCode,
        //            Item.AccountCode
        //            });
        //    }
        //    PPGrid.Dispose();
        //    return PPGrid;
        //}
    }

    public class Prod
    {
        public string id { get; set; }
        public string Name { get; set; }
    }
}
