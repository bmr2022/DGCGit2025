using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Security.Claims;
using System.Xml.Linq;
using eTactWeb.Data.Common;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Caching.Memory;
using static eTactWeb.DOM.Models.Common;
using eTactWeb.DOM.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System.Runtime.Caching;
using Newtonsoft.Json;
using NuGet.Packaging;
using FastReport.Web;
using common = eTactWeb.Data.Common;
using static eTactWeb.Data.Common.CommonFunc;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using FastReport.Data;
using FastReport;
using System.Configuration;


namespace eTactWeb.Controllers
{

    
    public class SaleInvoiceController : Controller
    {
        private readonly IDataLogic _IDataLogic;

        public ISaleBill _SaleBill { get; }
        private readonly IConfiguration _iconfiguration;
        private readonly ILogger<SaleBillController> _logger;
        private readonly IMemoryCache _MemoryCache;
        private readonly ICustomerJobWorkIssue _ICustomerJobWorkIssue;
        public IWebHostEnvironment _IWebHostEnvironment { get; }
        public SaleInvoiceController(ILogger<SaleBillController> logger, IDataLogic iDataLogic, ISaleBill iSaleBill, IConfiguration configuration, IMemoryCache memoryCache, EncryptDecrypt encryptDecrypt, IWebHostEnvironment iWebHostEnvironment, ICustomerJobWorkIssue CustomerJobWorkIssue)
        {
            _logger = logger;
            _IDataLogic = iDataLogic;
            _SaleBill = iSaleBill;
            _MemoryCache = memoryCache;
            _IWebHostEnvironment = iWebHostEnvironment;
            _iconfiguration = configuration;
            _ICustomerJobWorkIssue = CustomerJobWorkIssue;
        }
        [HttpPost]
        public async Task<IActionResult> SaleInvoice(SaleBillModel model)
        {
            var SBGrid = new DataTable();
            DataTable TaxDetailDT = null;
            DataTable AdjDetailDT = null;
            DataTable DrCrDetailDT = null;
            _MemoryCache.TryGetValue("SaleBillModel", out SaleBillModel MainModel);

            _MemoryCache.TryGetValue("KeySaleBillGrid", out IList<SaleBillDetail> saleBillDetail);
            _MemoryCache.TryGetValue("KeyTaxGrid", out List<TaxModel> TaxGrid);
            _MemoryCache.TryGetValue("KeyDrCrGrid", out List<DbCrModel> DrCrGrid);
            _MemoryCache.TryGetValue("KeyAdjChallanGrid", out List<CustomerJobWorkIssueAdjustDetail> AdjChallanGrid);
            if (saleBillDetail == null)
            {
                ModelState.Clear();
                ModelState.TryAddModelError("SaleBillDetail", "Sale Bill Grid Should Have Atleast 1 Item...!");
                return View("StockADjustment", model);
            }
            else if (saleBillDetail == null)
            {
                ModelState.Clear();
                ModelState.TryAddModelError("TaxDetail", "Tax Grid Should Have Atleast 1 Item...!");
                return View("StockADjustment", model);
            }
            else
            {
                model.CC = HttpContext.Session.GetString("Branch");
                //model.ActualEnteredBy   = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
                model.Uid = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                if (model.Mode == "U")
                {
                    model.LastUpdatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                    model.LastUpdatedByName = HttpContext.Session.GetString("EmpName");
                    SBGrid = GetDetailTable(saleBillDetail);
                }
                else
                {
                    model.ActualEnteredBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                    model.ActualEnteredByName = HttpContext.Session.GetString("EmpName");
                    model.EntryByempId = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                    SBGrid = GetDetailTable(saleBillDetail);
                }

                //if (AdjChallanGrid != null && AdjChallanGrid.Count > 0)
                //{
                //    AdjChallanGrid = GetAdjChallanDetailTable(AdjChallanGrid);
                //}

                if (TaxGrid != null && TaxGrid.Count > 0)
                {
                    TaxDetailDT = GetTaxDetailTable(TaxGrid);
                }
                if (DrCrGrid != null && DrCrGrid.Count > 0)
                {
                    DrCrDetailDT = CommonController.GetDrCrDetailTable(DrCrGrid);
                }

                if (MainModel.adjustmentModel != null && MainModel.adjustmentModel.AdjAdjustmentDetailGrid != null && MainModel.adjustmentModel.AdjAdjustmentDetailGrid.Count > 0)
                {
                    AdjDetailDT = CommonController.GetAdjDetailTable(MainModel.adjustmentModel.AdjAdjustmentDetailGrid.ToList(), model.SaleBillEntryId, model.SaleBillYearCode, model.AccountCode);
                }
                string serverFolderPath = Path.Combine(_IWebHostEnvironment.WebRootPath, "Uploads", "SaleBill");
                if (!Directory.Exists(serverFolderPath))
                {
                    Directory.CreateDirectory(serverFolderPath);
                }

                if (model.AttachmentFile1 != null)
                {
                    var ImagePath = Path.Combine("Uploads", "SaleBill", Guid.NewGuid().ToString() + "_" + model.AttachmentFile1.FileName);

                    string ServerPath = Path.Combine(_IWebHostEnvironment.WebRootPath, ImagePath);
                    using (FileStream FileStream = new FileStream(ServerPath, FileMode.Create))
                    {
                        await model.AttachmentFile1.CopyToAsync(FileStream);
                    }
                    model.AttachmentFilePath1 = ImagePath;
                }

                if (model.AttachmentFile2 != null)
                {
                    var ImagePath = Path.Combine("Uploads", "SaleBill", Guid.NewGuid().ToString() + "_" + model.AttachmentFile2.FileName);
                    string ServerPath = Path.Combine(_IWebHostEnvironment.WebRootPath, ImagePath);
                    using (FileStream FileStream = new FileStream(ServerPath, FileMode.Create))
                    {
                        await model.AttachmentFile2.CopyToAsync(FileStream);
                    }
                    model.AttachmentFilePath2 = ImagePath;
                }

                if (model.AttachmentFile3 != null)
                {
                    var ImagePath = Path.Combine("Uploads", "SaleBill", Guid.NewGuid().ToString() + "_" + model.AttachmentFile3.FileName);

                    string ServerPath = Path.Combine(_IWebHostEnvironment.WebRootPath, ImagePath);
                    using (FileStream FileStream = new FileStream(ServerPath, FileMode.Create))
                    {
                        await model.AttachmentFile3.CopyToAsync(FileStream);
                    }
                    model.AttachmentFilePath3 = ImagePath;
                }

                var Result = await _SaleBill.SaveSaleBill(model, SBGrid, TaxDetailDT, DrCrDetailDT, AdjDetailDT);

                if (Result != null)
                {
                    if (Result.StatusText == "Success" && Result.StatusCode == HttpStatusCode.OK)
                    {
                        ViewBag.isSuccess = true;
                        TempData["200"] = "200";
                        _MemoryCache.Remove(SBGrid);

                        var model1 = new SaleBillModel();
                        model1.adjustmentModel = model1.adjustmentModel ?? new AdjustmentModel();

                        model1.FinFromDate = HttpContext.Session.GetString("FromDate");
                        model1.FinToDate = HttpContext.Session.GetString("ToDate");
                        var yearCodeStr = HttpContext.Session.GetString("YearCode");
                        model1.SaleBillYearCode = !string.IsNullOrEmpty(yearCodeStr) ? Convert.ToInt32(yearCodeStr) : 0;
                        model1.CC = HttpContext.Session.GetString("Branch");
                        var uidStr = HttpContext.Session.GetString("UID");
                        model1.CreatedBy = !string.IsNullOrEmpty(uidStr) ? Convert.ToInt32(uidStr) : 0;
                        //model1.ActualEnteredByName = HttpContext.Session.GetString("EmpName");
                        model1.CreatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                        _MemoryCache.Remove("KeySaleBillGrid");
                        _MemoryCache.Remove("SaleBillModel");

                        //return View(model1);
                        return RedirectToAction(nameof(SaleInvoice), new { Id = 0, Mode = "", YC = 0 });
                    }
                    if (Result.StatusText == "Updated" && Result.StatusCode == HttpStatusCode.Accepted)
                    {
                        ViewBag.isSuccess = true;
                        TempData["202"] = "202";
                        var model1 = new SaleBillModel();
                        model1.adjustmentModel = new AdjustmentModel();
                        model1.adjustmentModel = model.adjustmentModel ?? new AdjustmentModel();
                        model1.FinFromDate = HttpContext.Session.GetString("FromDate");
                        model1.FinToDate = HttpContext.Session.GetString("ToDate");
                        model1.SaleBillYearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
                        model1.CC = HttpContext.Session.GetString("Branch");
                        //model1.ActualEnteredByName = HttpContext.Session.GetString("EmpName");
                        model1.CreatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                        _MemoryCache.Remove("KeySaleBillGrid");
                        _MemoryCache.Remove("SaleBillModel");
                        return View(model1);
                    }
                    if (Result.StatusText == "Error" && Result.StatusCode == HttpStatusCode.InternalServerError)
                    {
                        var errNum = Result.Result.Message.ToString().Split(":")[1];
                        model.adjustmentModel = model.adjustmentModel ?? new AdjustmentModel();
                        if (errNum == " 2627")
                        {
                            ViewBag.isSuccess = false;
                            TempData["2627"] = "2627";
                            _logger.LogError("\n \n ********** LogError ********** \n " + JsonConvert.SerializeObject(Result) + "\n \n");

                            return View(model);
                        }

                        ViewBag.isSuccess = false;
                        TempData["500"] = "500";
                        _logger.LogError("\n \n ********** LogError ********** \n " + JsonConvert.SerializeObject(Result) + "\n \n");
                        // return View("Error", Result);
                        return View(model);
                    }
                    HttpContext.Session.SetString("SaleInvoice", JsonConvert.SerializeObject(model));
                }
                return View();
            }
        }

        public static DataTable GetAdjustChallanDetailTable(List<CustomerJobWorkIssueAdjustDetail> model)
        {
            DataTable Table = new();
            Table.Columns.Add("ItemCode", typeof(int));
            Table.Columns.Add("Unit", typeof(string));
            Table.Columns.Add("BillQty", typeof(float));
            Table.Columns.Add("JWRate", typeof(float));
            Table.Columns.Add("ProcessId", typeof(int));
            Table.Columns.Add("SONO", typeof(string));
            Table.Columns.Add("CustOrderNo", typeof(string));
            Table.Columns.Add("SOYearCode", typeof(int));
            Table.Columns.Add("SchNo", typeof(string));
            Table.Columns.Add("SchYearCode", typeof(int));
            Table.Columns.Add("BOMIND", typeof(string));
            Table.Columns.Add("BOMNO", typeof(int));
            Table.Columns.Add("BOMEffDate", typeof(string));
            Table.Columns.Add("Produnprod", typeof(string));
            Table.Columns.Add("fromChallanOrSalebill", typeof(string));
            Table.Columns.Add("ItemAdjustmentRequired", typeof(string));

            if (model != null && model.Count > 0)
            {
                foreach (var Item in model)
                {
                    Table.Rows.Add(
                    new object[]
                    {
                        Item.ItemCode,
                        Item.Unit  ?? string.Empty,
                        Item.BillQty,
                        Item.JWRate,
                        Item.ProcessId,
                        Item.SONO ?? string.Empty,
                        Item.CustOrderNo ?? string.Empty,
                        Item.SOYearCode,
                        Item.SchNo ?? string.Empty,
                        Item.SchYearcode,
                        Item.BOMIND ?? string.Empty,
                        Item.BOMNO,
                        Item.BOMEffDate == null ? string.Empty : ParseFormattedDate(Item.BOMEffDate) ,
                        Item.Produnprod ?? string.Empty,
                        Item.fromChallanOrSalebill ?? string.Empty,
                        Item.ItemAdjustmentRequired ?? string.Empty
                    });
                }
            }
            return Table;
        }

        [HttpPost]
        public async Task<IActionResult> GetAdjustedChallanDetailsData(List<CustomerJobWorkIssueAdjustDetail> model, int YearCode, string EntryDate, string ChallanDate, int AccountCode,int itemCode)
        {
            try
            {
                if (model == null || !model.Any())
                {
                    return Json(new { success = false, message = "No data received." });
                }

                var adjustChallanDt = GetAdjustChallanDetailTable(model);
                var result = await _SaleBill.GetAdjustedChallanDetailsData(adjustChallanDt, YearCode, EntryDate, ChallanDate, AccountCode);

                MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                    SlidingExpiration = TimeSpan.FromMinutes(55),
                    Size = 1024,
                };

                _MemoryCache.Set("KeyAdjChallanGrid", result, cacheEntryOptions);

                return PartialView("_CustomerJwisschallanAdjustment", result);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }


        [HttpGet]
        public async Task<IActionResult> SaleInvoice(int ID, string Mode, int YC, string dashboardType = "", string fromDate = "", string toDate = "", string partCode = "", string itemName = "", string saleBillNo = "", string custName = "", string sono = "", string custOrderNo = "", string schNo = "", string performaInvNo = "", string saleQuoteNo = "", string domExportNEPZ = "", string Searchbox = "", string summaryDetail = "")
        {
            var model = new SaleBillModel(); // Create a new model instance for the view
             
            ViewData["Title"] = "Sale Bill Details";
            TempData.Clear();
            _MemoryCache.Remove("KeySaleBillGrid");
            _MemoryCache.Remove("SaleBillModel");
            _MemoryCache.Remove("KeyAdjGrid");

            // var model = await BindModel(MainModel);

            if (model.Mode != "U")
            {
                model.Uid = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                model.ActualEnteredBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                model.ActualEnteredByName = HttpContext.Session.GetString("EmpName");
            }
            model.adjustmentModel = new AdjustmentModel();

            if (!string.IsNullOrEmpty(Mode) && ID > 0 && (Mode == "V" || Mode == "U"))
            {
                model =  await _SaleBill.GetViewByID(ID, Mode, YC);
                model.Mode = Mode;
                model.ID = ID;
            }

            MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                SlidingExpiration = TimeSpan.FromMinutes(55),
                Size = 1024,
            };
            model.FinFromDate = HttpContext.Session.GetString("FromDate");
            model.FinToDate = HttpContext.Session.GetString("ToDate");
            model.SaleBillYearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
            model.CC = HttpContext.Session.GetString("Branch");
            _MemoryCache.Set("KeySaleBillGrid", model.saleBillDetails, cacheEntryOptions);
            _MemoryCache.Set("KeyAdjGrid", model.adjustmentModel == null ? new AdjustmentModel() : model.adjustmentModel, DateTimeOffset.Now.AddMinutes(60));
            _MemoryCache.Set("SaleBillModel", model, cacheEntryOptions);
            _MemoryCache.Set("KeyTaxGrid", model.TaxDetailGridd == null ? new List<TaxModel>() : model.TaxDetailGridd, DateTimeOffset.Now.AddMinutes(60));
            HttpContext.Session.SetString("SaleInvoice", JsonConvert.SerializeObject(model));

            //, string fromDate = "", string toDate = "", string partCode = "", string itemName = "", string saleBillNo = "", string custName = "", string sono = "", string custOrderNo = "", string schNo = ""
            //, string performaInvNo = "", string saleQuoteNo = "",string domExportNEPZ = "", string Searchbox = ""

            //model.FromDateBack = fromDate;
            //model.ToDateBack = toDate;
            //model.PartCodeBack = partCode;
            //model.ItemNameBack = itemName;
            //model.SaleBillNoBack = saleBillNo;
            //model.CustNameBack = custName;
            //model.SonoBack = sono;
            //model.CustOrderNoBack = custOrderNo;
            //model.SchNoBack = schNo;
            //model.PerformaInvNoBack = performaInvNo;
            //model.SaleQuoteNoBack = saleQuoteNo;
            //model.DomesticExportNEPZBack = domExportNEPZ;
            //model.SearchBoxBack = Searchbox;
            //model.SummaryDetailBack = summaryDetail;
            return View(model);
        }

        [HttpGet]
        [Route("{controller}/Dashboard")]
        public async Task<IActionResult> SBDashboard(string summaryDetail = "", string Flag = "True", string partCode = "", string itemName = "", string saleBillno = "", string customerName = "", string sono = "", string custOrderNo = "", string schNo = "", string performaInvNo = "", string saleQuoteNo = "", string domensticExportNEPZ = "", string fromdate = "", string toDate = "", string searchBox = "")
        {
            try
            {
                _MemoryCache.Remove("KeySaleBillGrid");
                _MemoryCache.Remove("KeyTaxGrid");
                var model = new SaleBillDashboard();

                var FromDt = HttpContext.Session.GetString("FromDate");
                model.FinFromDate = Convert.ToDateTime(FromDt).ToString("dd/MM/yyyy");
                //DateTime ToDate = DateTime.Today;
                var ToDt = HttpContext.Session.GetString("ToDate");
                model.FinToDate = Convert.ToDateTime(ToDt).ToString("dd/MM/yyyy");

                model.SaleBillYearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
                model.SummaryDetail = "Summary";
                model.SONO = "";
                var Result = await _SaleBill.GetDashboardData(model.SummaryDetail, partCode, itemName, saleBillno, customerName, sono, custOrderNo, schNo, performaInvNo, saleQuoteNo, domensticExportNEPZ, ParseFormattedDate(model.FinFromDate.Split(" ")[0]), common.CommonFunc.ParseFormattedDate(model.FinToDate.Split(" ")[0])).ConfigureAwait(true);
                if (Result != null)
                {
                    var _List = new List<TextValue>();
                    DataSet DS = Result.Result;
                    if (DS != null)
                    {
                        var DT = DS.Tables[0].DefaultView.ToTable(true, "SaleBillNo", "SaleBillDate", "GSTNO", "AccountCode", "AccountName", "SupplyType", "CustAddress", "StateNameofSupply"
                            , "CityofSupply", "DocumentHead", "ConsigneeAccountName", "ConsigneeAddress", "PaymentTerm", "Currency", "BillAmt", "TaxableAmt", "GSTAmount", "RoundType", "RoundOffAmt", "INVNetAmt"
                            , "Ewaybillno", "EInvNo", "EinvGenerated", "CountryOfSupply", "TransporterdocNo", "TransportModeBYRoadAIR", "DispatchTo", "DispatchThrough", "Remark", "Approved", "ApprovDate", "ApprovedBy", "ExchangeRate"
                            , "SaleBillEntryId", "SaleBillYearCode", "SaleBillEntryDate", "Shippingdate", "DistanceKM", "vehicleNo", "TransporterName", "DomesticExportNEPZ", "PaymentCreditDay", "ReceivedAmt", "pendAmount"
                            , "CancelBill", "Canceldate", "CancelBy", "Cancelreason", "BankName", "FreightPaid", "DispatchDelayReason", "AttachmentFilePath1", "AttachmentFilePath2", "AttachmentFilePath3", "DocketNo", "DispatchDelayreson", "Commodity", "CC"
                            , "Uid", "MachineName", "ActualEnteredByName", "ActualEntryDate", "LastUpdatedByName"
                            , "LastUpdationDate", "TypeItemServAssets", "SaleBillJobwork", "PerformaInvNo", "PerformaInvDate", "PerformaInvYearCode"
                            , "BILLAgainstWarrenty", "ExportInvoiceNo", "InvoiceTime", "RemovalDate", "RemovalTime", "EntryFreezToAccounts", "BalanceSheetClosed", "SaleQuotNo", "SaleQuotDate", "salesperson_name", "SalesPersonMobile"
                            );
                        model.SaleBillDataDashboard = CommonFunc.DataTableToList<SaleBillDashboard>(DT, "SaleBillSummTable");
                    }

                    if (Flag != "True")
                    {
                        model.FromDate1 = fromdate;
                        model.ToDate1 = toDate;
                        model.FinFromDate = fromdate;
                        model.FinToDate = toDate;
                        model.SaleBillNo = saleBillno;
                        model.PartCode = partCode;
                        model.ItemName = itemName;
                        model.AccountName = customerName;
                        model.SONO = sono;
                        model.CustOrderNo = custOrderNo;
                        model.SchNo = schNo;
                        model.PerformaInvNo = performaInvNo;
                        model.SaleQuotNo = saleQuoteNo;
                        model.DomesticExportNEPZ = domensticExportNEPZ;
                        model.SummaryDetail = summaryDetail;
                        model.Searchbox = searchBox;
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
        public async Task<IActionResult> GetSearchData(string summaryDetail, string partCode, string itemName, string saleBillno, string customerName, string sono, string custOrderNo, string schNo, string performaInvNo, string saleQuoteNo, string domensticExportNEPZ, string fromdate, string toDate)
        {
            try
            {
                var model = new SaleBillDashboard();
                model.SaleBillYearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
                var Result = await _SaleBill.GetDashboardData(summaryDetail, partCode, itemName, saleBillno, customerName, sono, custOrderNo, schNo, performaInvNo, saleQuoteNo, domensticExportNEPZ, ParseFormattedDate((fromdate).Split(" ")[0]), ParseFormattedDate(toDate.Split(" ")[0])).ConfigureAwait(true);
                if (Result != null)
                {
                    var _List = new List<TextValue>();
                    DataSet DS = Result.Result;
                    if (DS != null)
                    {
                        if (summaryDetail == "Summary")
                        {
                            var DT = DS.Tables[0].DefaultView.ToTable(true, "SaleBillNo", "SaleBillDate", "GSTNO", "AccountCode", "AccountName", "SupplyType", "CustAddress", "StateNameofSupply"
                                    , "CityofSupply", "DocumentHead", "ConsigneeAccountName", "ConsigneeAddress", "PaymentTerm", "Currency", "BillAmt", "TaxableAmt", "GSTAmount", "RoundType", "RoundOffAmt", "INVNetAmt"
                                    , "Ewaybillno", "EInvNo", "EinvGenerated", "CountryOfSupply", "TransporterdocNo", "TransportModeBYRoadAIR", "DispatchTo", "DispatchThrough", "Remark", "Approved", "ApprovDate", "ApprovedBy", "ExchangeRate"
                                    , "SaleBillEntryId", "SaleBillYearCode", "SaleBillEntryDate", "Shippingdate", "DistanceKM", "vehicleNo", "TransporterName", "DomesticExportNEPZ", "PaymentCreditDay", "ReceivedAmt", "pendAmount"
                                    , "CancelBill", "Canceldate", "CancelBy", "Cancelreason", "BankName", "FreightPaid", "DispatchDelayReason", "AttachmentFilePath1", "AttachmentFilePath2", "AttachmentFilePath3", "DocketNo", "DispatchDelayreson", "Commodity", "CC"
                                    , "Uid", "MachineName", "ActualEnteredByName", "ActualEntryDate", "LastUpdatedByName"
                                    , "LastUpdationDate", "TypeItemServAssets", "SaleBillJobwork", "PerformaInvNo", "PerformaInvDate", "PerformaInvYearCode"
                                    , "BILLAgainstWarrenty", "ExportInvoiceNo", "InvoiceTime", "RemovalDate", "RemovalTime", "EntryFreezToAccounts", "BalanceSheetClosed", "SaleQuotNo", "SaleQuotDate", "salesperson_name", "SalesPersonMobile"
                                    );
                            model.SaleBillDataDashboard = CommonFunc.DataTableToList<SaleBillDashboard>(DT, "SaleBillSummTable");

                            model.SaleBillDataDashboard = model.SaleBillDataDashboard
                                .GroupBy(psd => psd.SaleBillEntryId)
                                .Select(group => group.First())
                                .ToList();

                        }
                        else
                        {
                            var DT = DS.Tables[0].DefaultView.ToTable(true, "SaleBillNo", "SaleBillDate", "CustomerName", "GSTNO", "SupplyType", "CustAddress", "StateNameofSupply"
                                    , "CityofSupply", "DocumentHead", "ConsigneeName", "ConsigneeAddress", "ProdSchEntryId", "ProdSchDate", "SchdeliveryDate"
                                    , "PaymentTerm", "currency", "BillAmt", "SONO", "CustOrderNo", "SODate", "SchNo", "Schdate", "SOAmendNo", "SchAmendNo", "SchAmendDate", "PartCode", "ItemName", "CustomerPartCode", "HSNNO"
                                    , "Unit", "NoofCase", "Qty", "Rate", "ItemAmount", "StoreId", "StoreName", "batchno", "uniquebatchno"
                                    , "LotStock", "TotalStock", "RateInOtherCurr", "AltUnit", "AltQty", "SOPendQty", "AltSOPendQty", "AccountName"
                                    , "DiscountPer", "DiscountAmt", "Itemcolor", "ItemSize", "PacketsDetail", "OtherDetail", "ItemRemark", "TaxableAmt"
                                    , "GSTAmount", "RoundType", "RoundOffAmt", "INVNetAmt", "Ewaybillno", "EInvNo", "EinvGenerated", "CountryOfSupply"
                                    , "TransporterdocNo", "TransportModeBYRoadAIR", "DispatchTo", "DispatchThrough", "Remark", "Approved", "ApprovDate", "ApprovedBy"
                                    , "ExchangeRate", "SaleBillEntryId", "SaleBillYearCode", "SaleBillEntryDate", "Shippingdate", "DistanceKM", "vehicleNo"
                                    , "TransporterName", "DomesticExportNEPZ", "PaymentCreditDay", "ReceivedAmt", "pendAmount", "CancelBill", "Canceldate"
                                    , "CancelBy", "Cancelreason", "BankName", "FreightPaid", "DispatchDelayReason", "AttachmentFilePath1", "AttachmentFilePath2"
                                    , "AttachmentFilePath3", "DocketNo", "DispatchDelayreson", "Commodity", "CC", "Uid", "MachineName", "ActualEnteredByName"
                                    , "ActualEntryDate", "LastUpdatedByName", "LastUpdationDate", "TypeItemServAssets", "SaleBillJobwork", "PerformaInvNo",
                                    "AgainstProdPlanNo", "AgainstProdPlanYearCode", "AgaisntProdPlanDate", "GSTPer", "GSTType", "ProdSchno", "CostCenterId", "ProdSchYearcode"
                                    , "PerformaInvDate", "PerformaInvYearCode", "BILLAgainstWarrenty", "ExportInvoiceNo", "InvoiceTime", "RemovalDate", "ProcessId"
                                    , "RemovalTime", "EntryFreezToAccounts", "BalanceSheetClosed", "SaleQuotNo", "SaleQuotDate", "AdviceNo", "AdviceYearCode", "AdviseDate", "AdviseEntryId"
                                );
                            model.SaleBillDataDashboard = CommonFunc.DataTableToList<SaleBillDashboard>(DT, "SaleBillDetailTable");
                        }
                    }
                }
                model.SummaryDetail = summaryDetail;
                return PartialView("_SBDashboardGrid", model);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<JsonResult> GetBatchInventory()
        {
            var JSON = await _SaleBill.GetBatchInventory();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> NewEntryId(int YearCode)
        {
            var JSON = await _SaleBill.NewEntryId(YearCode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetCustomerBasedDetails(int Code)
        {
            var JSON = await _SaleBill.GetCustomerBasedDetails(Code);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillCurrency(int accountCode)
        {
            var JSON = await _SaleBill.FillCurrency(accountCode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetAutocompleteValue()
        {
            var JSON = await _SaleBill.GetAutocompleteValue();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }

        public IActionResult DeleteItemRow(int SeqNo, string Mode)
        {
            var MainModel = new SaleBillModel();
            if (Mode == "U")
            {
                int Indx = Convert.ToInt32(SeqNo) - 1;
                _MemoryCache.TryGetValue("KeySaleBillGrid", out List<SaleBillDetail> saleBillDetail);

                if (saleBillDetail != null && saleBillDetail.Count > 0)
                {
                    saleBillDetail.RemoveAt(Convert.ToInt32(Indx));

                    Indx = 0;

                    foreach (var item in saleBillDetail)
                    {
                        Indx++;
                        //item.SeqNo = Indx;
                    }
                    MainModel.saleBillDetails = saleBillDetail;

                    MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
                    {
                        AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                        SlidingExpiration = TimeSpan.FromMinutes(55),
                        Size = 1024,
                    };

                    _MemoryCache.Set("KeySaleBillGrid", MainModel.saleBillDetails, cacheEntryOptions);
                }
            }
            else
            {
                _MemoryCache.TryGetValue("KeySaleBillGrid", out List<SaleBillDetail> saleBillGrid);
                int Indx = Convert.ToInt32(SeqNo) - 1;

                if (saleBillGrid != null && saleBillGrid.Count > 0)
                {
                    saleBillGrid.RemoveAt(Convert.ToInt32(Indx));

                    Indx = 0;

                    foreach (var item in saleBillGrid)
                    {
                        Indx++;
                        //item.SeqNo = Indx;
                    }
                    MainModel.saleBillDetails = saleBillGrid;

                    MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
                    {
                        AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                        SlidingExpiration = TimeSpan.FromMinutes(55),
                        Size = 1024,
                    };

                    _MemoryCache.Set("KeySaleBillGrid", MainModel.saleBillDetails, cacheEntryOptions);
                }
            }

            return PartialView("_SaleBillGrid", MainModel);
        }
        public async Task<JsonResult> EditItemRows(int SeqNo)
        {
            var MainModel = new SaleBillModel();
            _MemoryCache.TryGetValue("KeySaleBillGrid", out List<SaleBillDetail> saleBillGrid);
            var SAGrid = saleBillGrid.Where(x => x.SeqNo == SeqNo);
            string JsonString = JsonConvert.SerializeObject(SAGrid);
            return Json(JsonString);
        }
        public IActionResult AddSaleBillDetail(SaleBillDetail model)
        {
            try
            {
                _MemoryCache.TryGetValue("KeySaleBillGrid", out IList<SaleBillDetail> SaleBillDetail);
                _MemoryCache.TryGetValue("SaleBillModel", out SaleBillModel saleBillModel);

                var MainModel = new SaleBillModel();
                var saleBillDetail = new List<SaleBillDetail>();
                var rangeSaleBillGrid = new List<SaleBillDetail>();

                if (model != null)
                {
                    if (SaleBillDetail == null)
                    {
                        //model.SeqNo = 1;
                        saleBillDetail.Add(model);
                    }
                    else
                    {
                        if (SaleBillDetail.Any(x => x.ItemCode == model.ItemCode && x.StoreId == model.StoreId && x.Batchno == model.Batchno))
                        {
                            return StatusCode(207, "Duplicate");
                        }

                        //model.SeqNo = SaleBillDetail.Count + 1;
                        saleBillDetail = SaleBillDetail.Where(x => x != null).ToList();
                        rangeSaleBillGrid.AddRange(saleBillDetail);
                        saleBillDetail.Add(model);

                    }
                    //MainModel = BindItem4Grid(model);
                    saleBillDetail = saleBillDetail.OrderBy(item => item.SeqNo).ToList();
                    MainModel.saleBillDetails = saleBillDetail;
                    MainModel.ItemDetailGrid = saleBillDetail;

                    MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
                    {
                        AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                        SlidingExpiration = TimeSpan.FromMinutes(55),
                        Size = 1024,
                    };


                    _MemoryCache.Set("KeySaleBillGrid", MainModel.saleBillDetails, cacheEntryOptions);

                    MainModel = BindItem4Grid(MainModel);
                    MainModel.saleBillDetails = saleBillDetail;
                    MainModel.ItemDetailGrid = saleBillDetail;
                    _MemoryCache.Set("KeySaleBillGrid", MainModel.saleBillDetails, cacheEntryOptions);
                    _MemoryCache.Set("SaleBillModel", MainModel, cacheEntryOptions);
                }
                else
                {
                    ModelState.TryAddModelError("Error", "SaleBill List Cannot Be Empty...!");
                }
                return PartialView("_SaleBillGrid", MainModel);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IActionResult ClearDRCRGrid()
        {
            _MemoryCache.Remove("KeyDrCrGrid");
            return Json("Ok");
        }


        private SaleBillModel BindItem4Grid(SaleBillModel model)
        {
            var _List = new List<DPBItemDetail>();

            _MemoryCache.TryGetValue("SaleBillModel", out SaleBillModel MainModel);
            //var SeqNo = 0;
            //if(MainModel == null)
            //{
            //    SeqNo++;
            //}
            _List.Add(
                new DPBItemDetail
                {
                    SeqNo = MainModel.ItemDetailGrid == null ? 1 : MainModel.ItemDetailGrid.Count + 1,
                    docTypeId = 1, //model.docTypeId,
                    DocTypeText = string.Empty, //model.DocTypeText,
                    BillQty = Convert.ToDecimal(model.Qty),

                    Amount = model.Amount,
                    Description = string.Empty, //model.Description,
                    DiscPer = Convert.ToDecimal(model.DiscountPer),
                    DiscRs = model.DiscountAmt,

                    HSNNo = model.HSNNo,
                    ItemCode = model.ItemCode,
                    ItemText = model.ItemName,

                    OtherRateCurr = Convert.ToDecimal(model.RateInOtherCurr),
                    PartCode = model.ItemCode,
                    PartText = model.PartCode,

                    DPBQty = Convert.ToDecimal(model.Qty),
                    Process = model.ProcessId,
                    ProcessName = model.ProcessName,
                    CostCenter = model.CostCenterId,
                    CostCenterName = model.CostCenterName,
                    Rate = Convert.ToDecimal(model.Rate),

                    PONo = string.Empty,//model.pono,
                    POYear = 0,//model.POYear,
                    PODate = string.Empty,//model.PODate,
                    ScheduleNo = model.SchNo,
                    ScheduleYear = model.SaleSchYearCode,
                    ScheduleDate = model.Schdate,

                    Unit = model.Unit,
                });

            if (MainModel.DPBItemDetails == null)
                MainModel.DPBItemDetails = _List;
            else
                MainModel.DPBItemDetails.AddRange(_List);

            MainModel.ItemNetAmount = decimal.Parse(MainModel.DPBItemDetails.Sum(x => x.Amount).ToString("#.#0"));

            return MainModel;
        }

        //private static DataTable GetAdjChallanDetailTable(IList<CustomerJobWorkIssueAdjustDetail> DetailList)
        //{
        //    var DTSSGrid = new DataTable();

        //    DTSSGrid.Columns.Add("EntryDate", typeof(int));
        //    DTSSGrid.Columns.Add("SONO", typeof(int));
        //    DTSSGrid.Columns.Add("CustOrderNo", typeof(string));
        //    DTSSGrid.Columns.Add("SOYearCode", typeof(int));
        //    DTSSGrid.Columns.Add("SODate", typeof(string));
        //    DTSSGrid.Columns.Add("SchNo", typeof(string));
        //    DTSSGrid.Columns.Add("SchDate", typeof(string));
        //    DTSSGrid.Columns.Add("SaleSchYearCode", typeof(int));
        //    DTSSGrid.Columns.Add("SOAmendNo", typeof(string));
        //    DTSSGrid.Columns.Add("SOAmendDate", typeof(string));
        //     DTSSGrid.Columns.Add("SchAmendDate", typeof(string));
        //    DTSSGrid.Columns.Add("ItemCode", typeof(int));
        //    DTSSGrid.Columns.Add("HSNNO", typeof(string));
      
        //    //DateTime DeliveryDt = new DateTime();
        //    foreach (var Item in DetailList)
        //    {
        //        string uniqueString = Guid.NewGuid().ToString();
        //        DTSSGrid.Rows.Add(
        //            new object[]
        //            {
        //            Item.SeqNo,
        //            Item.SONO,
        //            Item.CustOrderNo ?? string.Empty,
        //            Item.SOYearCode,
        //            //Item.SODate == null ? string.Empty : (Item.SODate.Split(" ")[0]),
        //            Item.SODate == null ? string.Empty : common.CommonFunc.ParseFormattedDate(Item.SODate.Split(" ")[0]),
        //            Item.SchNo ?? "",
        //            Item.Schdate == null ? string.Empty : common.CommonFunc.ParseFormattedDate(Item.Schdate.Split(" ")[0])
        //            });
        //    }
        //    DTSSGrid.Dispose();
        //    return DTSSGrid;
        //}
        
        private static DataTable GetDetailTable(IList<SaleBillDetail> DetailList)
        {
            var DTSSGrid = new DataTable();

            DTSSGrid.Columns.Add("SeqNo", typeof(int));
            DTSSGrid.Columns.Add("SONO", typeof(int));
            DTSSGrid.Columns.Add("CustOrderNo", typeof(string));
            DTSSGrid.Columns.Add("SOYearCode", typeof(int));
            DTSSGrid.Columns.Add("SODate", typeof(string));
            DTSSGrid.Columns.Add("SchNo", typeof(string));
            DTSSGrid.Columns.Add("SchDate", typeof(string));
            DTSSGrid.Columns.Add("SaleSchYearCode", typeof(int));
            DTSSGrid.Columns.Add("SOAmendNo", typeof(string));
            DTSSGrid.Columns.Add("SOAmendDate", typeof(string));
             DTSSGrid.Columns.Add("SchAmendDate", typeof(string));
            DTSSGrid.Columns.Add("ItemCode", typeof(int));
            DTSSGrid.Columns.Add("HSNNO", typeof(string));
            DTSSGrid.Columns.Add("Unit", typeof(string));
            DTSSGrid.Columns.Add("NoofCase", typeof(float));
            DTSSGrid.Columns.Add("Qty", typeof(float));
            DTSSGrid.Columns.Add("UnitOfRate", typeof(string));
            DTSSGrid.Columns.Add("RateInOtherCurr", typeof(float));
            DTSSGrid.Columns.Add("Rate", typeof(float));
            DTSSGrid.Columns.Add("AltUnit", typeof(string));
            DTSSGrid.Columns.Add("AltQty", typeof(float));
            DTSSGrid.Columns.Add("ItemWeight", typeof(float));
            DTSSGrid.Columns.Add("NoofPcs", typeof(int));
            DTSSGrid.Columns.Add("CustomerPartCode", typeof(string));
            DTSSGrid.Columns.Add("MRP", typeof(float));
            DTSSGrid.Columns.Add("OriginalMRP", typeof(float));
            DTSSGrid.Columns.Add("SOPendQty", typeof(float));
            DTSSGrid.Columns.Add("AltSOPendQty", typeof(int));
            DTSSGrid.Columns.Add("DisountPer", typeof(float));
            DTSSGrid.Columns.Add("DiscountAmt", typeof(float));
            DTSSGrid.Columns.Add("ItemSize", typeof(string));
            DTSSGrid.Columns.Add("Itemcolor", typeof(string));
            DTSSGrid.Columns.Add("StoreId", typeof(int));
            DTSSGrid.Columns.Add("ItemAmount", typeof(float));
            DTSSGrid.Columns.Add("AdviceNo", typeof(string));
            DTSSGrid.Columns.Add("AdviseEntryId", typeof(int));
            DTSSGrid.Columns.Add("AdviceYearCode", typeof(int));
            DTSSGrid.Columns.Add("AdviseDate", typeof(string));
            DTSSGrid.Columns.Add("ProcessId", typeof(int));
            DTSSGrid.Columns.Add("batchno", typeof(string));
            DTSSGrid.Columns.Add("uniquebatchno", typeof(string));
            DTSSGrid.Columns.Add("LotStock", typeof(float));
            DTSSGrid.Columns.Add("TotalStock", typeof(float));
            DTSSGrid.Columns.Add("AgainstProdPlanNo", typeof(string));
            DTSSGrid.Columns.Add("AgainstProdPlanYearCode", typeof(int));
            DTSSGrid.Columns.Add("AgaisntProdPlanDate", typeof(string));
            DTSSGrid.Columns.Add("GSTPer", typeof(float));
            DTSSGrid.Columns.Add("GSTType", typeof(string));
            DTSSGrid.Columns.Add("PacketsDetail", typeof(string));
            DTSSGrid.Columns.Add("OtherDetail", typeof(string));
            DTSSGrid.Columns.Add("ItemRemark", typeof(string));
            DTSSGrid.Columns.Add("prodSchno", typeof(string));
            DTSSGrid.Columns.Add("ProdSchYearcode", typeof(int));
            DTSSGrid.Columns.Add("prodSchEntryId", typeof(int));
            DTSSGrid.Columns.Add("ProdSchDate", typeof(string));
            DTSSGrid.Columns.Add("SchdeliveryDate", typeof(string));
            DTSSGrid.Columns.Add("CostCenterid", typeof(int));
            DTSSGrid.Columns.Add("ProdUnProduced", typeof(string));
            DTSSGrid.Columns.Add("BOMInd", typeof(string));
            DTSSGrid.Columns.Add("CustJWmanadatory", typeof(string));
            DTSSGrid.Columns.Add("StockableNonStockable", typeof(string));
            //DateTime DeliveryDt = new DateTime();
            foreach (var Item in DetailList)
            {
                string uniqueString = Guid.NewGuid().ToString();
                DTSSGrid.Rows.Add(
                    new object[]
                    {
                    Item.SeqNo,
                    Item.SONO,
                    Item.CustOrderNo ?? string.Empty,
                    Item.SOYearCode,
                    //Item.SODate == null ? string.Empty : (Item.SODate.Split(" ")[0]),
                    Item.SODate == null ? string.Empty : common.CommonFunc.ParseFormattedDate(Item.SODate.Split(" ")[0]),
                    Item.SchNo ?? "",
                    Item.Schdate == null ? string.Empty : common.CommonFunc.ParseFormattedDate(Item.Schdate.Split(" ")[0]),
                    Item.SaleSchYearCode,
                    Item.SOAmendNo ?? string.Empty,
                    Item.SOAmendDate == null ? string.Empty : common.CommonFunc.ParseFormattedDate(Item.SOAmendDate.Split(" ")[0]),
                    Item.SchAmendDate == null ? string.Empty : common.CommonFunc.ParseFormattedDate(Item.SchAmendDate.Split(" ")[0]),
                    Item.ItemCode,
                    Item.HSNNo.ToString() ?? string.Empty,
                    Item.Unit ?? string.Empty,
                    Item.NoofCase,
                    Item.Qty,
                    Item.UnitOfRate ?? string.Empty,
                    Item.RateInOtherCurr,
                    Item.Rate,
                    Item.AltUnit ?? string.Empty,
                    Item.AltQty,
                    Item.ItemWeight,
                    Item.NoofPcs,
                    Item.CustomerPartCode ?? string.Empty,
                    Item.MRP,
                    Item.OriginalMRP,
                    Item.SOPendQty,
                    Item.AltSOPendQty,
                    Item.DiscountPer,
                    Item.DiscountAmt,
                    Item.ItemSize ?? string.Empty,
                    Item.Itemcolor ?? string.Empty,
                    Item.StoreId,
                    Item.Amount,
                    Item.AdviceNo ?? string.Empty,
                    Item.AdviseEntryId,
                    Item.AdviceYearCode,
                    Item.AdviseDate == null ? string.Empty : common.CommonFunc.ParseFormattedDate(Item.AdviseDate.Split(" ")[0]),
                    Item.ProcessId,
                    Item.Batchno ?? string.Empty,
                    Item.Uniquebatchno ?? string.Empty,
                    Item.LotStock,
                    Item.TotalStock,
                    Item.AgainstProdPlanNo ?? string.Empty,
                    Item.AgainstProdPlanYearCode,
                    Item.AgaisntProdPlanDate == null ? string.Empty : common.CommonFunc.ParseFormattedDate(Item.AgaisntProdPlanDate.Split(" ")[0]),
                    Item.GSTPer,
                    Item.GSTType ?? string.Empty,
                    Item.PacketsDetail ?? string.Empty,
                    Item.OtherDetail ?? string.Empty,
                    Item.ItemRemark ?? string.Empty,
                    Item.ProdSchno ?? string.Empty,
                    Item.ProdSchYearcode,
                    Item.ProdSchEntryId,
                    Item.ProdSchDate == null ? string.Empty : common.CommonFunc.ParseFormattedDate(Item.ProdSchDate.Split(" ")[0]),
                    Item.SchdeliveryDate == null ? string.Empty : common.CommonFunc.ParseFormattedDate(Item.SchdeliveryDate.Split(" ")[0]),
                    Item.CostCenterId,
                    Item.ProducedUnprod ?? string.Empty,
                    Item.BOMInd ?? string.Empty,
                    Item.CustJwAdjustmentMandatory ?? string.Empty,
                    Item.StockableNonStockable ?? string.Empty,
                    });
            }
            DTSSGrid.Dispose();
            return DTSSGrid;
        }

        public async Task<JsonResult> GetDistance(int accountCode)
        {
            var JSON = await _SaleBill.GetDistance(accountCode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillCustomerList(string ShowAllCustomer)
        {
            var JSON = await _SaleBill.FillCustomerList(ShowAllCustomer);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillDocument(string ShowAllDoc)
        {
            var JSON = await _SaleBill.FillDocument(ShowAllDoc);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FILLSOScheduleDate(string sono, int accountCode, int soYearCode, string schNo, int schYearCode)
        {
            var JSON = await _SaleBill.FILLSOScheduleDate(sono, accountCode, soYearCode, schNo, schYearCode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillSONO(string billDate, string accountCode)
        {
            var JSON = await _SaleBill.FillSONO(billDate, accountCode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillSOPendQty(int saleBillEntryId, string saleBillNo, int saleBillYearCode, string sono, int soYearcode, string custOrderNo, int itemCode, int accountCode, string schNo, int schYearCode)
        {
            var JSON = await _SaleBill.FillSOPendQty(saleBillEntryId, saleBillNo, saleBillYearCode, sono, soYearcode, custOrderNo, itemCode, accountCode, schNo, soYearcode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillConsigneeList(string showAllConsignee)
        {
            var JSON = await _SaleBill.FillConsigneeList(showAllConsignee);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillSOYearCode(string sono, string accountCode)
        {
            var JSON = await _SaleBill.FillSOYearCode(sono, accountCode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> DisplaySODetail(string accountName, string itemName, string partCode, string sono, int soYearCode, string custOrderNo, string schNo, int schYearCode)
        {
            var JSON = await _SaleBill.DisplaySODetail(accountName, itemName, partCode, sono, soYearCode, custOrderNo, schNo, schYearCode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillItems(string showAll, string TypeItemServAssets,string sbJobWork)
        {
            var JSON = await _SaleBill.FillItems( showAll, TypeItemServAssets,sbJobWork);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillSOWiseItems(string invoiceDate, string sono,int soYearCode,int accountCode, string sbJobWork)
        {
            invoiceDate = ParseFormattedDate(invoiceDate);
            var JSON = await _SaleBill.FillSOWiseItems( invoiceDate, sono,soYearCode,accountCode,sbJobWork);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> JWItemList(string typeItemServAssets, string showAll, string bomInd)
        {
            var JSON = await _SaleBill.JWItemList(typeItemServAssets,showAll,bomInd);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillStore()
        {
            var JSON = await _SaleBill.FillStore();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillSOItemRate(string sono, int soYearCode, int accountCode, string custOrderNo, int itemCode)
        {
            var JSON = await _SaleBill.FillSOItemRate(sono, soYearCode, accountCode, custOrderNo, itemCode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FILLCustomerOrderAndSPDate(string billDate, int accountCode, string sono, int soYearCode)
        {
            billDate = (billDate);
            var JSON = await _SaleBill.FILLCustomerOrderAndSPDate(billDate, accountCode, sono, soYearCode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> AllowTaxPassword()
        {
            var JSON = await _SaleBill.AllowTaxPassword();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillSOSchedule(string sono, string accountCode, int soYearCode, int ItemCode)
        {
            var JSON = await _SaleBill.FillSOSchedule(sono, accountCode, soYearCode, ItemCode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public IActionResult PrintReport(int EntryId, int YearCode = 0, string Type = "")
        {
            //string my_connection_string;
            //string contentRootPath = _IWebHostEnvironment.ContentRootPath;
            //string webRootPath = _IWebHostEnvironment.WebRootPath;
            //var webReport = new WebReport();
            //webReport.Report.Load(webRootPath + "\\SaleBill.frx");
            //webReport.Report.SetParameterValue("entryparam", EntryId);
            //webReport.Report.SetParameterValue("yearparam", YearCode);
            //my_connection_string = iconfiguration.GetConnectionString("eTactDB");
            //webReport.Report.SetParameterValue("MyParameter", my_connection_string);
            //return View(webReport);
            string my_connection_string;
            string contentRootPath = _IWebHostEnvironment.ContentRootPath;
            string webRootPath = _IWebHostEnvironment.WebRootPath;
            var webReport = new WebReport();
            webReport.Report.Clear();
            var ReportName = _SaleBill.GetReportName();
            webReport.Report.Dispose();
            webReport.Report = new Report();
            if (!String.Equals(ReportName.Result.Result.Rows[0].ItemArray[0], System.DBNull.Value))
            {
                webReport.Report.Load(webRootPath + "\\" + ReportName.Result.Result.Rows[0].ItemArray[0]); // from database
            }
            else
            {
                webReport.Report.Load(webRootPath + "\\SaleBill.frx"); // default report
            }
            webReport.Report.SetParameterValue("entryparam", EntryId);
            webReport.Report.SetParameterValue("yearparam", YearCode);
            my_connection_string = _iconfiguration.GetConnectionString("eTactDB");
            webReport.Report.SetParameterValue("MyParameter", my_connection_string);
            webReport.Report.Dictionary.Connections[0].ConnectionString = my_connection_string;
            webReport.Report.Prepare();
            foreach (var dataSource in webReport.Report.Dictionary.DataSources)
            {
                if (dataSource is TableDataSource tableDataSource)
                {
                    tableDataSource.Enabled = true;
                    tableDataSource.Init(); // Refresh the data source
                }
            }
            return View(webReport);
        }
        private static DataTable GetTaxDetailTable(List<TaxModel> TaxDetailList)
        {
            DataTable Table = new();
            Table.Columns.Add("TxSeqNo", typeof(int));
            Table.Columns.Add("TxType", typeof(string));
            Table.Columns.Add("TxItemCode", typeof(int));
            Table.Columns.Add("TxTaxType", typeof(int));
            Table.Columns.Add("TxAccountCode", typeof(int));
            Table.Columns.Add("TxPercentg", typeof(float));
            Table.Columns.Add("TxAdInTxable", typeof(string));
            Table.Columns.Add("TxRoundOff", typeof(string));
            Table.Columns.Add("TxAmount", typeof(float));
            Table.Columns.Add("TxRefundable", typeof(string));
            Table.Columns.Add("TxOnExp", typeof(float));
            Table.Columns.Add("TxRemark", typeof(string));

            foreach (TaxModel Item in TaxDetailList)
            {
                Table.Rows.Add(
                    new object[]
                    {
                    Item.TxSeqNo,
                    Item.TxType,
                    Item.TxItemCode,
                    Item.TxTaxType,
                    Item.TxAccountCode,
                    Item.TxPercentg,
                    Item.TxAdInTxable,
                    Item.TxRoundOff,
                    Item.TxAmount,
                    Item.TxRefundable,
                    Item.TxOnExp,
                    Item.TxRemark,
                    });
            }

            return Table;
        }

        public async Task<IActionResult> DeleteByID(int ID, int YC, string entryByMachineName, string partCode = "", string itemName = "", string saleBillno = "", string customerName = "", int sono = 0, string custOrderNo = "", string schNo = "", string performaInvNo = "", string saleQuoteNo = "", string domensticExportNEPZ = "", string fromdate = "", string toDate = "", string Searchbox = "")
        {
            var Result = await _SaleBill.DeleteByID(ID, YC, entryByMachineName).ConfigureAwait(false);

            if (Result.StatusText == "Deleted" || Result.StatusCode == HttpStatusCode.Gone || Result.StatusText == "Success")
            {
                ViewBag.isSuccess = true;
                TempData["410"] = "410";
            }
            else
            {
                ViewBag.isSuccess = false;
                TempData["500"] = "500";
            }
            return RedirectToAction("SBDashboard", new { Flag = "False", ItemName = itemName, PartCode = partCode, saleBillno = saleBillno, customerName = customerName, sono = sono, custOrderNo = custOrderNo, schNo = schNo, performaInvNo = performaInvNo, saleQuoteNo = saleQuoteNo, domensticExportNEPZ = domensticExportNEPZ, fromdate = fromdate, todate = toDate, searchBox = Searchbox });
        }
    }
}
