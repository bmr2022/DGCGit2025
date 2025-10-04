using eTactWeb.Controllers;
using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using static eTactWeb.Data.Common.CommonFunc;
using static eTactWeb.DOM.Models.Common;
using System.Data;
using System.Net;
using FastReport.Web;
using FastReport;
using System.Globalization;
using NuGet.Packaging;

namespace eTactwebAccounts.Controllers
{

  
    public class AgainstAdjustVoucherController : Controller
    {
        private readonly IDataLogic _IDataLogic;
        public IAgainstAdjustVoucher _IAgainstAdjustVoucher { get; }
        private readonly ILogger<AgainstAdjustVoucherController> _logger;
        private readonly IConfiguration iconfiguration;
        public IWebHostEnvironment _IWebHostEnvironment { get; }
        private readonly ConnectionStringService _connectionStringService;
        public AgainstAdjustVoucherController(ILogger<AgainstAdjustVoucherController> logger, IDataLogic iDataLogic, IAgainstAdjustVoucher IAgainstAdjustVoucher, EncryptDecrypt encryptDecrypt, IWebHostEnvironment iWebHostEnvironment, IConfiguration iconfiguration, ConnectionStringService connectionStringService)
        {
            _logger = logger;
            _IDataLogic = iDataLogic;
            _IAgainstAdjustVoucher = IAgainstAdjustVoucher;
            _IWebHostEnvironment = iWebHostEnvironment;
            this.iconfiguration = iconfiguration;
            _connectionStringService = connectionStringService;
        }
        public async Task<JsonResult> GetFormRights()
        {
            var userID = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
            var JSON = await _IAgainstAdjustVoucher.GetFormRights(userID);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public IActionResult PrintReport(int EntryId = 0, int YearCode = 0, string VoucherName = "")
        {
            string my_connection_string;
            string contentRootPath = _IWebHostEnvironment.ContentRootPath;
            string webRootPath = _IWebHostEnvironment.WebRootPath;
            var webReport = new WebReport();
            webReport.Report.Clear();
            webReport.Report.Dispose();
            webReport.Report = new Report();

            webReport.Report.Load(webRootPath + "\\VoucherReport.frx");
            my_connection_string = _connectionStringService.GetConnectionString();
            my_connection_string = iconfiguration.GetConnectionString("eTactDB");
            webReport.Report.Dictionary.Connections[0].ConnectionString = my_connection_string;
            webReport.Report.Dictionary.Connections[0].ConnectionStringExpression = "";
            webReport.Report.SetParameterValue("vouchernameparam", VoucherName);
            webReport.Report.SetParameterValue("yearcodeparam", YearCode);
            webReport.Report.SetParameterValue("entryidparam", EntryId);
            webReport.Report.SetParameterValue("MyParameter", my_connection_string);
            webReport.Report.Refresh();
            return View(webReport);
        }
        [Route("{controller}/Index")]
        [HttpGet]
        public async Task<ActionResult> AgainstAdjustVoucher(int ID, string Mode, int YearCode, string VoucherNo, string FromDate = "", string ToDate = "", string LedgerName = "", string AgainstVoucherRefNo = "", string AgainstVoucherNo = "", string Searchbox = "", string DashboardType = "")
        {
            HttpContext.Session.Remove("KeyAgainstAdjustVoucherGrid");
            HttpContext.Session.Remove("KeyAgainstAdjustVoucherGridEdit");
            TempData.Clear();
            var MainModel = new AgainstAdjustVoucherModel();
            MainModel.CC = HttpContext.Session.GetString("Branch");
            MainModel.YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
            MainModel.ActualEntryBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
            MainModel.ActualEntryByEmp = HttpContext.Session.GetString("EmpName");
            MainModel.ActualEntryDate = DateTime.Now.ToString("dd/MM/yy");
            MainModel.UID = Convert.ToInt32(HttpContext.Session.GetString("UID"));
            MainModel.FromDate = HttpContext.Session.GetString("FromDate");
            MainModel.ToDate = HttpContext.Session.GetString("ToDate");

            if (!string.IsNullOrEmpty(Mode) && ID > 0 && (Mode == "U" || Mode == "V"))
            {
                MainModel = await _IAgainstAdjustVoucher.GetViewByID(ID, YearCode, VoucherNo).ConfigureAwait(false);
                MainModel.Mode = Mode; // Set Mode to Update
                MainModel.ID = ID;
                MainModel.VoucherNo = VoucherNo;

                string serializedGrid = JsonConvert.SerializeObject(MainModel.AgainstAdjustVoucherList);
                HttpContext.Session.SetString("KeyAgainstAdjustVoucherGridEdit", serializedGrid);
            }

            if (Mode == "U" && MainModel.UpdatedBy == 0)
            {
                MainModel.UpdatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                MainModel.UpdatedByEmp = HttpContext.Session.GetString("EmpName");
                MainModel.UpdatedOn = DateTime.Now;
            }

            MainModel.FromDateBack = FromDate;
            MainModel.ToDateBack = ToDate;
            MainModel.VoucherNoBack = VoucherNo;
            MainModel.LedgerNameBack = LedgerName;
            MainModel.AgainstVoucherRefNo = AgainstVoucherRefNo;
            MainModel.AgainstVoucherNoBack = AgainstVoucherNo;
            MainModel.GlobalSearchBack = Searchbox;
            MainModel.DashboardTypeBack = DashboardType;
            return View(MainModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("{controller}/Index")]
        public async Task<IActionResult> AgainstAdjustVoucher(AgainstAdjustVoucherModel model)
        {
            try
            {
                var GIGrid = new DataTable();
                string modelJson = HttpContext.Session.GetString("KeyAgainstAdjustVoucherGrid");
                List<AgainstAdjustVoucherModel> AgainstAdjustVoucherGrid = new List<AgainstAdjustVoucherModel>();
                if (!string.IsNullOrEmpty(modelJson))
                {
                    AgainstAdjustVoucherGrid = JsonConvert.DeserializeObject<List<AgainstAdjustVoucherModel>>(modelJson);
                }
                string modelEditJson = HttpContext.Session.GetString("KeyAgainstAdjustVoucherGridEdit");
                List<AgainstAdjustVoucherModel> AgainstAdjustVoucherGridEdit = new List<AgainstAdjustVoucherModel>();
                if (!string.IsNullOrEmpty(modelEditJson))
                {
                    AgainstAdjustVoucherGridEdit = JsonConvert.DeserializeObject<List<AgainstAdjustVoucherModel>>(modelEditJson);
                }

                model.ActualEntryBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                model.ActualEntryByEmp = HttpContext.Session.GetString("UID");
                if (model.Mode == "U")
                {
                    GIGrid = GetDetailTable(AgainstAdjustVoucherGridEdit);
                }
                else
                {
                    GIGrid = GetDetailTable(AgainstAdjustVoucherGrid);
                }
                //var Result = await _IAgainstAdjustVoucher.SaveAgainstAdjustVoucher(model, GIGrid);
                //if (Result != null)
                //{
                //    if (Result.StatusText == "Success" && Result.StatusCode == HttpStatusCode.OK)
                //    {
                //        ViewBag.isSuccess = true;
                //        TempData["200"] = "200";
                //        HttpContext.Session.Remove("KeyAgainstAdjustVoucherGrid");
                //        HttpContext.Session.Remove("KeyAgainstAdjustVoucherGridEdit");
                //    }
                //    else if (Result.StatusText == "Success" && Result.StatusCode == HttpStatusCode.Accepted)
                //    {
                //        ViewBag.isSuccess = true;
                //        TempData["202"] = "202";
                //        HttpContext.Session.Remove("KeyAgainstAdjustVoucherGrid");
                //        HttpContext.Session.Remove("KeyAgainstAdjustVoucherGridEdit");
                //    }
                //    else if (Result.StatusText == "Error" && Result.StatusCode == HttpStatusCode.InternalServerError)
                //    {
                //        ViewBag.isSuccess = false;
                //        TempData["500"] = "500";
                //        _logger.LogError($"\n \n ********** LogError ********** \n {JsonConvert.SerializeObject(Result)}\n \n");
                //        return View("Error", Result);
                //    }
                //}
                return RedirectToAction(nameof(AgainstAdjustVoucher));
            }
            catch (Exception ex)
            {
                // Log and return the error
                LogException<AgainstAdjustVoucherController>.WriteException(_logger, ex);
                var ResponseResult = new ResponseResult
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    StatusText = "Error",
                    Result = ex
                };
                return View("Error", ResponseResult);
            }
        }
        private static DataTable GetDetailTable(IList<AgainstAdjustVoucherModel> DetailList)
        {
            try
            {
                var GIGrid = new DataTable();
                GIGrid.Columns.Add("AccEntryId", typeof(int));
                GIGrid.Columns.Add("AccYearCode", typeof(int));
                GIGrid.Columns.Add("EntryDate", typeof(DateTime));
                GIGrid.Columns.Add("DocEntryId", typeof(int));
                GIGrid.Columns.Add("VoucherDocNo", typeof(string));
                GIGrid.Columns.Add("BillVouchNo", typeof(string));
                GIGrid.Columns.Add("VoucherDocDate", typeof(DateTime));
                GIGrid.Columns.Add("BillInvoiceDate", typeof(DateTime));
                GIGrid.Columns.Add("BillYearCode", typeof(int));
                GIGrid.Columns.Add("VoucherRefNo", typeof(string));
                GIGrid.Columns.Add("SeqNo", typeof(int));
                GIGrid.Columns.Add("Accountcode", typeof(int));
                GIGrid.Columns.Add("BankCashAccountCode", typeof(int));
                GIGrid.Columns.Add("AccountGroupType", typeof(string));
                GIGrid.Columns.Add("Description", typeof(string));
                GIGrid.Columns.Add("VoucherRemark", typeof(string));
                GIGrid.Columns.Add("DrAmt", typeof(decimal));
                GIGrid.Columns.Add("CrAmt", typeof(decimal));
                GIGrid.Columns.Add("entryBankCash", typeof(string));
                GIGrid.Columns.Add("Vouchertype", typeof(string));
                GIGrid.Columns.Add("chequeDate", typeof(DateTime));
                GIGrid.Columns.Add("chequeClearDate", typeof(DateTime));
                GIGrid.Columns.Add("UID", typeof(int));
                GIGrid.Columns.Add("CC", typeof(string));
                GIGrid.Columns.Add("TDSNatureOfPayment", typeof(string));
                GIGrid.Columns.Add("RoundDr", typeof(decimal));
                GIGrid.Columns.Add("RoundCr", typeof(decimal));
                GIGrid.Columns.Add("AgainstEntryid", typeof(int));
                GIGrid.Columns.Add("AgainstVoucheryearcode", typeof(int));
                GIGrid.Columns.Add("AgainstVoucherType", typeof(string));
                GIGrid.Columns.Add("againstVoucherRefNo", typeof(string));
                GIGrid.Columns.Add("AgainstVoucherNo", typeof(string));
                GIGrid.Columns.Add("AgainstBillno", typeof(string));
                GIGrid.Columns.Add("PONo", typeof(string));
                GIGrid.Columns.Add("PoDate", typeof(DateTime));
                GIGrid.Columns.Add("POYear", typeof(int));
                GIGrid.Columns.Add("SONo", typeof(int));
                GIGrid.Columns.Add("CustOrderNo", typeof(string));
                GIGrid.Columns.Add("SoDate", typeof(DateTime));
                GIGrid.Columns.Add("SOYear", typeof(int));
                GIGrid.Columns.Add("ApprovedBy", typeof(int));
                GIGrid.Columns.Add("ApprovedDate", typeof(DateTime));
                GIGrid.Columns.Add("Approved", typeof(string));
                GIGrid.Columns.Add("AccountNarration", typeof(string));
                GIGrid.Columns.Add("CurrencyId", typeof(int));
                GIGrid.Columns.Add("CurrentValue", typeof(decimal));
                GIGrid.Columns.Add("AdjAmountInOtherCurrency", typeof(decimal));
                GIGrid.Columns.Add("AmountInOtherCurr", typeof(decimal));
                GIGrid.Columns.Add("ItemCode", typeof(int));
                GIGrid.Columns.Add("VoucherNo", typeof(string));
                GIGrid.Columns.Add("ChequePrintAC", typeof(string));
                GIGrid.Columns.Add("EmpCode", typeof(int));
                GIGrid.Columns.Add("DeptCode", typeof(int));
                GIGrid.Columns.Add("MRNNO", typeof(string));
                GIGrid.Columns.Add("MRNDate", typeof(DateTime));
                GIGrid.Columns.Add("MRNYearCode", typeof(int));
                GIGrid.Columns.Add("CostCenterId", typeof(int));
                GIGrid.Columns.Add("PaymentMode", typeof(string));
                GIGrid.Columns.Add("EntryTypebankcashLedger", typeof(string));
                GIGrid.Columns.Add("TDSApplicable", typeof(string));
                GIGrid.Columns.Add("TDSChallanNo", typeof(string));
                GIGrid.Columns.Add("TDSChallanDate", typeof(DateTime));
                GIGrid.Columns.Add("PreparedByEmpId", typeof(int));
                GIGrid.Columns.Add("CGSTAccountCode", typeof(int));
                GIGrid.Columns.Add("CGSTPer", typeof(decimal));
                GIGrid.Columns.Add("CGSTAmt", typeof(decimal));
                GIGrid.Columns.Add("SGSTAccountCode", typeof(int));
                GIGrid.Columns.Add("SGSTPer", typeof(decimal));
                GIGrid.Columns.Add("SGSTAmt", typeof(decimal));
                GIGrid.Columns.Add("IGSTAccountCode", typeof(int));
                GIGrid.Columns.Add("IGSTPer", typeof(decimal));
                GIGrid.Columns.Add("IGSTAmt", typeof(decimal));
                GIGrid.Columns.Add("ModeOfAdjustment", typeof(string));
                GIGrid.Columns.Add("NameOnCheque", typeof(string));
                GIGrid.Columns.Add("BalanceSheetClosed", typeof(string));
                GIGrid.Columns.Add("ProjectNo", typeof(string));
                GIGrid.Columns.Add("ProjectYearcode", typeof(int));
                GIGrid.Columns.Add("ProjectDate", typeof(DateTime));
                GIGrid.Columns.Add("ActualEntryBy", typeof(int));
                GIGrid.Columns.Add("ActualEntryDate", typeof(DateTime));
                GIGrid.Columns.Add("UpdatedBy", typeof(int));
                GIGrid.Columns.Add("LastUpdatedDate", typeof(DateTime));
                GIGrid.Columns.Add("EntryByMachine", typeof(string));
                GIGrid.Columns.Add("OursalespersonId", typeof(int));
                GIGrid.Columns.Add("SubVoucherName", typeof(string));


                foreach (var Item in DetailList)
                {
                    GIGrid.Rows.Add(
                        new object[]
                        {
                Item.AccEntryId ,
                Item.YearCode ,
                Item.EntryDate=DateTime.Now.ToString("dd/MMM/yyyy") ,
                Item.DocEntryId ,
                Item.VoucherDocNo ?? string.Empty,
                Item.BillVouchNo ?? string.Empty,
                Item.VoucherDocDate=DateTime.Now.ToString("dd/MMM/yyyy") ,
                Item.BillInvoiceDate=DateTime.Now.ToString("dd/MMM/yyyy",CultureInfo.InvariantCulture) ,
                Item.BillYearCode ,
                Item.VoucherRefNo ?? string.Empty,
                Item.SrNO ,
                Item.AccountCode,
                Item.BankCashAccountCode ,
                Item.AccountGroupType ?? string.Empty,
                Item.Description ?? string.Empty,
                Item.VoucherRemark ?? string.Empty,
                Item.DrAmt,
                Item.CrAmt ,
                Item.EntryBankCash,
                Item.VoucherType ?? string.Empty,
                Item.ChequeDate != null ? Item.ChequeDate : null,
                Item.BankRECO != null ? Item.ChequeDate : null,
                Item.UID ,
                Item.CC ?? string.Empty,
                Item.TDSNatureOfPayment ?? string.Empty,
                Item.RoundDr ,
                Item.RoundCr ,
                Item.AgainstVoucherEntryId ,
                Item.AgainstVoucheryearCode ,
                Item.AgainstVoucherType ?? string.Empty,
                Item.AgainstVoucherRefNo ?? string.Empty,
                Item.AgainstVoucherNo ?? string.Empty,
                Item.AgainstBillno ?? string.Empty,
                Item.PONo ?? string.Empty,
                Item.PoDate =DateTime.Now.ToString("dd/MMM/yyyy"),
                Item.POYear ,
                Item.SONo ,
                Item.CustOrderNo ?? string.Empty,
                Item.SoDate  =DateTime.Now.ToString("dd/MMM/yyyy"),
                Item.SOYear ,
                Item.ApprovedBy,
                Item.ApprovedDate = DateTime.Now.ToString("dd/MMM/yyyy"),
                Item.Approved ,
                Item.AccountNarration ?? string.Empty,
                Item.CurrencyId ,
                Item.CurrentValue ,
                Item.AdjustmentAmtOthCur ,
                Item.AdjustmentAmt ,
                Item.ItemCode ,
                Item.VoucherNo ?? string.Empty,
                Item.ChequePrintAC ?? string.Empty,
                Item.EmpCode ,
                Item.DeptCode,
                Item.MRNO ?? string.Empty,
                Item.MRNDate =DateTime.Now.ToString("dd/MMM/yyyy") ,
                Item.MRNYearCode ,
                Item.CostCenterId ,
                Item.PaymentMode ?? string.Empty,
                Item.EntryTypebankcashLedger ?? string.Empty,
                Item.TDSApplicable ?? string.Empty,
                Item.TDSChallanNo ?? string.Empty,
                Item.TDSChallanDate = DateTime.Now.ToString("dd/MMM/yyyy") ,
                Item.PreparedByEmpId ,
                Item.CGSTAccountCode ,
                Item.CGSTPer ,
                Item.CGSTAmt,
                Item.SGSTAccountCode ,
                Item.SGSTPer,
                Item.SGSTAmt ,
                Item.IGSTAccountCode ,
                Item.IGSTPer,
                Item.IGSTAmt ,
                Item.ModeOfAdjustment ?? string.Empty,
                Item.NameOnCheque ?? string.Empty,
                Item.BalanceSheetClosed ?? string.Empty,
                Item.ProjectNo ,
                Item.ProjectYearcode ,
                Item.ProjectDate = DateTime.Now.ToString("dd/MMM/yyyy"),
                Item.ActualEntryBy ,
                Item.ActualEntryDate = DateTime.Now.ToString("dd/MMM/yyyy"),
                Item.UpdatedBy ,
                Item.UpdatedOn ,
                Item.EntryByMachine ?? string.Empty,
                Item.OursalespersonId ,
                Item.SubVoucher ?? string.Empty,


                        });
                }
                GIGrid.Dispose();
                return GIGrid;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<JsonResult> FillEntryID(int YearCode, string VoucherDate)
        {
            var JSON = await _IAgainstAdjustVoucher.FillEntryID(YearCode, VoucherDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        } 
        public async Task<JsonResult> GetAccEntryId(int YearCode, string VoucherType, string VoucherNo, int AccountCode, string InvoiceNo)
        {
            var JSON = await _IAgainstAdjustVoucher.GetAccEntryId(YearCode, VoucherType, VoucherNo, AccountCode, InvoiceNo);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillVoucherType(int yearcode)
        { 
            yearcode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
            var JSON = await _IAgainstAdjustVoucher.FillVoucherType(yearcode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        //public async Task<IActionResult> GetAdjustedData(int YearCode, string VoucherType, string VoucherNo, int AccountCode, string InvoiceNo, int AccEntryId)
        //{
        //    //var model = new AgainstAdjustVoucherModel();
        //    //model = await _IAgainstAdjustVoucher.GetAdjustedData(YearCode, VoucherType, VoucherNo, AccountCode, InvoiceNo, AccEntryId);
        //    //return PartialView("_AgainstAdjustVoucher", model);
        //    var models = await _IAgainstAdjustVoucher.GetAdjustedData(YearCode, VoucherType, VoucherNo, AccountCode, InvoiceNo, AccEntryId);
        //    return PartialView("_AgainstAdjustVoucher", models);
        //}

        public async Task<IActionResult> GetAdjustedData(int YearCode, string VoucherType, string VoucherNo, int AccountCode, string InvoiceNo, int AccEntryId)
        {
            var model = new AgainstAdjustVoucherModel();
            model.Mode = "Adjust";
            model.AgainstAdjustVoucherList = await _IAgainstAdjustVoucher.GetAdjustedData(YearCode, VoucherType, VoucherNo, AccountCode, InvoiceNo, AccEntryId);
            return PartialView("_AgainstAdjustVoucher", model);
        }

        //[HttpPost]
        //public IActionResult AddAgnstRefToAdjstmntDetail([FromBody] AgainstAdjustVoucherModel model)
        //{
        //    if (model == null || model.AdjAdjustmentDetailGrid == null || !model.AdjAdjustmentDetailGrid.Any())
        //    {
        //        return BadRequest("No adjustment data provided.");
        //    }

        //    // Get existing model from session
        //    string modelJson = HttpContext.Session.GetString("KeyAdjGrid");
        //    var existingGrid = !string.IsNullOrEmpty(modelJson)
        //        ? JsonConvert.DeserializeObject<AgainstAdjustVoucherModel>(modelJson)
        //        : new AgainstAdjustVoucherModel { AdjAdjustmentDetailGrid = new List<AdjustmentModel>() };

        //    if (existingGrid.AdjAdjustmentDetailGrid == null)
        //        existingGrid.AdjAdjustmentDetailGrid = new List<AdjustmentModel>();

        //    // Force mode = AgainstRef
        //    foreach (var row in model.AdjAdjustmentDetailGrid)
        //    {
        //        row.AdjModeOfAdjstment = "AgainstRef";
        //    }

        //    // Duplicate check (based on Mode + RefNo)
        //    foreach (var newRow in model.AdjAdjustmentDetailGrid)
        //    {
        //        bool isDuplicate = existingGrid.AdjAdjustmentDetailGrid.Any(a =>
        //            a.AdjModeOfAdjstment != null &&
        //            a.AdjModeOfAdjstment.Equals(newRow.AdjModeOfAdjstment, StringComparison.OrdinalIgnoreCase) &&
        //            a.AdjNewRefNo != null &&
        //            a.AdjNewRefNo.Equals(newRow.AdjNewRefNo, StringComparison.OrdinalIgnoreCase)
        //        );

        //        if (isDuplicate)
        //        {
        //            return Conflict("Duplicate adjustment found.");
        //        }
        //    }

        //    // Assign sequence numbers
        //    int seqStart = existingGrid.AdjAdjustmentDetailGrid.Count + 1;
        //    foreach (var row in model.AdjAdjustmentDetailGrid)
        //    {
        //        row.AdjSeqNo = seqStart++;
        //        existingGrid.AdjAdjustmentDetailGrid.Add(row);
        //    }

        //    // Save back into session
        //    string updatedJson = JsonConvert.SerializeObject(existingGrid);
        //    HttpContext.Session.SetString("KeyAdjGrid", updatedJson);

        //    return Json(existingGrid.AdjAdjustmentDetailGrid);
        //}

        //public IActionResult GetUpdatedAdjGridData()
        //{
        //    // Retrieve the updated data from the cache
        //    string modelJson = HttpContext.Session.GetString("KeyAdjGrid");
        //    AgainstAdjustVoucherModel AdjGrid = new AgainstAdjustVoucherModel();
        //    if (!string.IsNullOrEmpty(modelJson))
        //    {
        //        AdjGrid = JsonConvert.DeserializeObject<AgainstAdjustVoucherModel>(modelJson);
        //    }

        //    if (AdjGrid != null)
        //    {
        //        return Json(AdjGrid.AdjAdjustmentDetailGrid);
        //    }
        //    return Json(new List<AgainstAdjustVoucherModel>());
        //}
        public void StoreInSession(string sessionKey, object sessionObject)
        {
            string serializedObject = JsonConvert.SerializeObject(sessionObject);
            HttpContext.Session.SetString(sessionKey, serializedObject);
        }
        //public IList<AgainstAdjustVoucherModel> Add2List(AgainstAdjustVoucherModel model, IList<AgainstAdjustVoucherModel> AdjGrid, bool? IsAgnstRefPopupData = false)
        //{
        //    var _List = new List<AgainstAdjustVoucherModel>();
        //    if (IsAgnstRefPopupData != true)
        //    {
        //        _List.Add(new AgainstAdjustVoucherModel
        //        {
        //            AdjSeqNo = AdjGrid == null ? 1 : AdjGrid.Count + 1,
        //            AdjModeOfAdjstment = model.AdjModeOfAdjstment,
        //            AdjModeOfAdjstmentName = model.AdjModeOfAdjstmentName,
        //            AdjDescription = model.AdjDescription,
        //            AdjDueDate = model.AdjDueDate,
        //            AdjNewRefNo = model.AdjNewRefNo,
        //            AdjPendAmt = model.AdjPendAmt,
        //            AdjDrCr = model.AdjDrCr,
        //            AdjDrCrName = model.AdjDrCrName,
        //            AdjPurchOrderNo = model.AdjPurchOrderNo,
        //            AdjPOYear = model.AdjPOYear,
        //            AdjPODate = model.AdjPODate,
        //            AdjOpenEntryID = model.AdjOpenEntryID ?? 0,
        //            AdjOpeningYearCode = model.AdjOpeningYearCode ?? 0,
        //            AdjAgnstAccEntryID = model.AdjAgnstAccEntryID ?? 0,
        //            AdjAgnstAccYearCode = model.AdjAgnstAccYearCode ?? 0,
        //        });
        //    }
        //    else
        //    {
        //        foreach (var item in model.AdjAdjustmentDetailGrid)
        //        {
        //            _List.Add(new AgainstAdjustVoucherModel
        //            {
        //                AdjSeqNo = AdjGrid == null ? 1 : AdjGrid.Count + 1,
        //                AdjModeOfAdjstment = "AgainstRef",
        //                AdjModeOfAdjstmentName = "Against Ref",
        //                AdjDescription = item.AdjDescription,
        //                AdjDueDate = item.AdjAgnstVouchDate,
        //                AdjNewRefNo = item.AdjNewRefNo,
        //                AdjPendAmt = Convert.ToDecimal(item.AdjAgnstAdjstedAmt),
        //                AdjDrCr = item.AdjAgnstDrCr,
        //                AdjDrCrName = item.AdjAgnstDrCr,
        //                AdjPurchOrderNo = string.Empty,
        //                AdjPOYear = 0,
        //                AdjPODate = null,
        //                AdjAgnstVouchNo = item.AdjAgnstVouchNo,
        //                AdjAgnstVouchType = item.AdjAgnstVouchType,
        //                AdjOpenEntryID = item.AdjAgnstOpenEntryID ?? 0,
        //                AdjOpeningYearCode = item.AdjAgnstOpeningYearCode ?? 0,
        //                AdjAgnstAccEntryID = item.AdjAgnstAccEntryID ?? 0,
        //                AdjAgnstAccYearCode = item.AdjAgnstAccYearCode ?? 0,
        //            });
        //        }
        //    }
        //    return _List;
        //}
        public async Task<JsonResult> GetLedgerBalance(int OpeningYearCode, int AccountCode, string VoucherDate)
        {
            var JSON = await _IAgainstAdjustVoucher.GetLedgerBalance(OpeningYearCode, AccountCode, VoucherDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillVoucherNo(int YearCode, string VoucherType, string FromDate, string ToDate, int AccountCode)
        {
            YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
            var JSON = await _IAgainstAdjustVoucher.FillVoucherNo(YearCode,VoucherType, FromDate,ToDate,AccountCode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        } 
        public async Task<JsonResult> FillInvoiceNo(int YearCode, string VoucherType, string FromDate, string ToDate, string VoucherNo, int AccountCode)
        {
            YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
            var JSON = await _IAgainstAdjustVoucher.FillInvoiceNo(YearCode,VoucherType, FromDate,ToDate, VoucherNo,AccountCode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillLedgerName(string VoucherType, string ShowAll)
        {
            var JSON = await _IAgainstAdjustVoucher.FillLedgerName(VoucherType, ShowAll);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        //public IActionResult AddAgainstAdjustVoucherDetail(AgainstAdjustVoucherModel model)
        //{
        //    try
        //    {
        //        if (model.Mode == "U" || model.Mode == "V")
        //        {
        //            string modelJson = HttpContext.Session.GetString("KeyAgainstAdjustVoucherGridEdit");
        //            List<AgainstAdjustVoucherModel> AgainstAdjustVoucherGrid = new List<AgainstAdjustVoucherModel>();
        //            if (!string.IsNullOrEmpty(modelJson))
        //            {
        //                AgainstAdjustVoucherGrid = JsonConvert.DeserializeObject<List<AgainstAdjustVoucherModel>>(modelJson);
        //            }

        //            var MainModel = new AgainstAdjustVoucherModel();
        //            var AgainstAdjustVchGrid = new List<AgainstAdjustVoucherModel>();
        //            var AgainstAdjustGrid = new List<AgainstAdjustVoucherModel>();
        //            var ssGrid = new List<AgainstAdjustVoucherModel>();

        //            var count = 0;
        //            if (model != null)
        //            {
        //                if (AgainstAdjustVoucherGrid == null)
        //                {
        //                    model.SrNO = 1;
        //                    AgainstAdjustGrid.Add(model);
        //                }
        //                else
        //                {
        //                    if (model.BankType.ToLower() == "bank")
        //                    {
        //                        if (AgainstAdjustVoucherGrid.Any(x => (x.LedgerName == model.LedgerName) || x.BankType == "Bank"))
        //                        {
        //                            return StatusCode(210, "Duplicate");
        //                        }
        //                        else
        //                        {
        //                            model.SrNO = AgainstAdjustVoucherGrid.Count + 1;
        //                            AgainstAdjustGrid = AgainstAdjustVoucherGrid.Where(x => x != null).ToList();
        //                            ssGrid.AddRange(AgainstAdjustGrid);
        //                            AgainstAdjustGrid.Add(model);

        //                        }
        //                    }
        //                    else if (model.ModeOfAdjustment.ToLower() == "newref")
        //                    {
        //                        if (AgainstAdjustVoucherGrid.Any(x => (x.LedgerName == model.LedgerName) && (x.ModeOfAdjustment == model.ModeOfAdjustment)))
        //                        {
        //                            return StatusCode(207, "Duplicate");
        //                        }
        //                        else
        //                        {
        //                            model.SrNO = AgainstAdjustVoucherGrid.Count + 1;
        //                            AgainstAdjustGrid = AgainstAdjustVoucherGrid.Where(x => x != null).ToList();
        //                            ssGrid.AddRange(AgainstAdjustGrid);
        //                            AgainstAdjustGrid.Add(model);

        //                        }
        //                    }
        //                    else if (model.ModeOfAdjustment.ToLower() == "advance")
        //                    {
        //                        if (AgainstAdjustVoucherGrid.Any(x => (x.LedgerName == model.LedgerName) && (x.ModeOfAdjustment == model.ModeOfAdjustment)))
        //                        {
        //                            return StatusCode(208, "Duplicate");
        //                        }
        //                        else
        //                        {
        //                            model.SrNO = AgainstAdjustVoucherGrid.Count + 1;
        //                            AgainstAdjustGrid = AgainstAdjustVoucherGrid.Where(x => x != null).ToList();
        //                            ssGrid.AddRange(AgainstAdjustGrid);
        //                            AgainstAdjustGrid.Add(model);

        //                        }
        //                    }
        //                    else if (model.ModeOfAdjustment.ToLower() == "againstref")
        //                    {
        //                        if (AgainstAdjustVoucherGrid.Any(x => (x.LedgerName == model.LedgerName) && x.AgainstVoucherNo == model.AgainstVoucherNo))
        //                        {
        //                            return StatusCode(209, "Duplicate");
        //                        }
        //                        else
        //                        {
        //                            model.SrNO = AgainstAdjustVoucherGrid.Count + 1;
        //                            AgainstAdjustGrid = AgainstAdjustVoucherGrid.Where(x => x != null).ToList();
        //                            ssGrid.AddRange(AgainstAdjustGrid);
        //                            AgainstAdjustGrid.Add(model);

        //                        }
        //                    }

        //                }

        //                MainModel.AgainstAdjustVoucherList = AgainstAdjustGrid.OrderBy(x => x.SrNO).ToList();

        //                string serializedGrid = JsonConvert.SerializeObject(MainModel.AgainstAdjustVoucherList);
        //                HttpContext.Session.SetString("KeyAgainstAdjustVoucherGridEdit", serializedGrid);
        //            }
        //            else
        //            {
        //                ModelState.TryAddModelError("Error", "Schedule List Cannot Be Empty...!");
        //            }
        //            return PartialView("_AgainstAdjustVoucherGrid", MainModel);
        //        }
        //        else
        //        {
        //            string modelJson = HttpContext.Session.GetString("KeyAgainstAdjustVoucherGrid");
        //            List<AgainstAdjustVoucherModel> AgainstAdjustVoucherGrid = new List<AgainstAdjustVoucherModel>();
        //            if (!string.IsNullOrEmpty(modelJson))
        //            {
        //                AgainstAdjustVoucherGrid = JsonConvert.DeserializeObject<List<AgainstAdjustVoucherModel>>(modelJson);
        //            }

        //            var MainModel = new AgainstAdjustVoucherModel();
        //            var AgainstAdjustVchGrid = new List<AgainstAdjustVoucherModel>();
        //            var AgainstAdjustGrid = new List<AgainstAdjustVoucherModel>();
        //            var ssGrid = new List<AgainstAdjustVoucherModel>();

        //            if (model != null)
        //            {
        //                if (AgainstAdjustVoucherGrid == null)
        //                {
        //                    model.SrNO = 1;
        //                    AgainstAdjustGrid.Add(model);
        //                }
        //                else
        //                {
        //                    //if (model.BankType.ToLower() == "bank")
        //                    //{
        //                    //    if (AgainstAdjustVoucherGrid.Any(x => (x.LedgerName == model.LedgerName) || x.BankType == "Bank"))
        //                    //    {
        //                    //        return StatusCode(210, "Duplicate");
        //                    //    }
        //                    //    else
        //                    //    {
        //                    //        model.SrNO = AgainstAdjustVoucherGrid.Count + 1;
        //                    //        AgainstAdjustGrid = AgainstAdjustVoucherGrid.Where(x => x != null).ToList();
        //                    //        ssGrid.AddRange(AgainstAdjustGrid);
        //                    //        AgainstAdjustGrid.Add(model);

        //                    //    }
        //                    //}
        //                    //else 
        //                    if (model.ModeOfAdjustment.ToLower() == "newref")
        //                    {
        //                        if (AgainstAdjustVoucherGrid.Any(x => (x.LedgerName == model.LedgerName) && (x.ModeOfAdjustment == model.ModeOfAdjustment)))
        //                        {
        //                            return StatusCode(207, "Duplicate");
        //                        }
        //                        else
        //                        {
        //                            model.SrNO = AgainstAdjustVoucherGrid.Count + 1;
        //                            AgainstAdjustGrid = AgainstAdjustVoucherGrid.Where(x => x != null).ToList();
        //                            ssGrid.AddRange(AgainstAdjustGrid);
        //                            AgainstAdjustGrid.Add(model);

        //                        }
        //                    }
        //                    else if (model.ModeOfAdjustment.ToLower() == "advance")
        //                    {
        //                        if (AgainstAdjustVoucherGrid.Any(x => (x.LedgerName == model.LedgerName) && (x.ModeOfAdjustment == model.ModeOfAdjustment)))
        //                        {
        //                            return StatusCode(208, "Duplicate");
        //                        }
        //                        else
        //                        {
        //                            model.SrNO = AgainstAdjustVoucherGrid.Count + 1;
        //                            AgainstAdjustGrid = AgainstAdjustVoucherGrid.Where(x => x != null).ToList();
        //                            ssGrid.AddRange(AgainstAdjustGrid);
        //                            AgainstAdjustGrid.Add(model);

        //                        }
        //                    }
        //                    else if (model.ModeOfAdjustment.ToLower() == "againstref")
        //                    {
        //                        if (AgainstAdjustVoucherGrid.Any(x => (x.LedgerName == model.LedgerName) && x.AgainstVoucherNo == model.AgainstVoucherNo))
        //                        {
        //                            return StatusCode(209, "Duplicate");
        //                        }
        //                        else
        //                        {
        //                            model.SrNO = AgainstAdjustVoucherGrid.Count + 1;
        //                            AgainstAdjustGrid = AgainstAdjustVoucherGrid.Where(x => x != null).ToList();
        //                            ssGrid.AddRange(AgainstAdjustGrid);
        //                            AgainstAdjustGrid.Add(model);

        //                        }
        //                    }
        //                }

        //                MainModel.AgainstAdjustVoucherList = AgainstAdjustGrid.OrderBy(x => x.SrNO).ToList();

        //                string serializedGrid = JsonConvert.SerializeObject(MainModel.AgainstAdjustVoucherList);
        //                HttpContext.Session.SetString("KeyAgainstAdjustVoucherGrid", serializedGrid);
        //            }
        //            else
        //            {
        //                ModelState.TryAddModelError("Error", "Schedule List Cannot Be Empty...!");
        //            }
        //            return PartialView("_AgainstAdjustVoucherGrid", MainModel);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
        [HttpPost]
        public IActionResult AddAgainstAdjustVoucherDetail([FromBody] AgainstAdjustVoucherModel models)
        {
            try
            {
                if (models == null || models.AgainstAdjustVoucherList == null || !models.AgainstAdjustVoucherList.Any())
                    return BadRequest("No rows to add.");

                string sessionKey = (models.AgainstAdjustVoucherList.First().Mode == "U" ||
                                     models.AgainstAdjustVoucherList.First().Mode == "V")
                                    ? "KeyAgainstAdjustVoucherGridEdit"
                                    : "KeyAgainstAdjustVoucherGrid";

                var existingGridJson = HttpContext.Session.GetString(sessionKey);
                var gridList = string.IsNullOrEmpty(existingGridJson)
                    ? new List<AgainstAdjustVoucherModel>()
                    : JsonConvert.DeserializeObject<List<AgainstAdjustVoucherModel>>(existingGridJson);

                foreach (var model in models.AgainstAdjustVoucherList)
                {
                    var mode = model.ModeOfAdjustment?.Replace(" ", "").ToLower();

                    bool isDuplicate = mode switch
                    {
                        "newref" => gridList.Any(x => x.LedgerName == model.LedgerName && x.ModeOfAdjustment?.Replace(" ", "").ToLower() == mode),
                        "advance" => gridList.Any(x => x.LedgerName == model.LedgerName && x.ModeOfAdjustment?.Replace(" ", "").ToLower() == mode),
                        "againstref" => gridList.Any(x => x.LedgerName == model.LedgerName && x.AgainstVoucherNo == model.AgainstVoucherNo),
                        _ => model.BankType?.ToLower() == "bank" && gridList.Any(x => x.BankType == "Bank" || x.LedgerName == model.LedgerName)
                    };

                    //if (isDuplicate)
                    //{
                    //    return mode switch
                    //    {
                    //        "newref" => StatusCode(207, "Duplicate"),
                    //        "advance" => StatusCode(208, "Duplicate"),
                    //        "againstref" => StatusCode(209, "Duplicate"),
                    //        _ => StatusCode(210, "Duplicate")
                    //    };
                    //}

                    model.SrNO = gridList.Count + 1;
                    gridList.Add(model);
                }

                HttpContext.Session.SetString(sessionKey, JsonConvert.SerializeObject(gridList));

                var mainModel = new AgainstAdjustVoucherModel
                {
                    AgainstAdjustVoucherList = gridList.OrderBy(x => x.SrNO).ToList()
                };

                return PartialView("_AgainstAdjustVoucherGrid", mainModel); // or Json(mainModel)
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        public async Task<JsonResult> GetPendVouchBillAgainstRefPopupByID(int AC, int? YC, int? PayRecEntryId, int? PayRecYearcode, string DRCR, string TransVouchType, string TransVouchDate)
        {
            string Flag = "";
            var JSON = await _IDataLogic.GetPendVouchBillAgainstRefPopupByID(AC, YC, PayRecEntryId, PayRecYearcode, DRCR, TransVouchType, TransVouchDate, Flag);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
    }
}
