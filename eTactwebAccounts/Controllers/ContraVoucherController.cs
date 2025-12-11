using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using FastReport;
using FastReport.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System.Data;
using System.Net;
using static eTactWeb.Data.Common.CommonFunc;
using static eTactWeb.DOM.Models.Common;

namespace eTactwebAccounts.Controllers
{
      public class ContraVoucherController : Controller
    {
        private readonly IDataLogic _IDataLogic;
        public IContraVoucher _IContraVoucher { get; }
        private readonly ILogger<ContraVoucherController> _logger;
        private readonly IConfiguration iconfiguration;
        public IWebHostEnvironment _IWebHostEnvironment { get; }
        private readonly ConnectionStringService _connectionStringService;
        public ContraVoucherController(ILogger<ContraVoucherController> logger, IDataLogic iDataLogic, IContraVoucher IContraVoucher, EncryptDecrypt encryptDecrypt, IWebHostEnvironment iWebHostEnvironment, IConfiguration iconfiguration, ConnectionStringService connectionStringService)
        {
            _logger = logger;
            _IDataLogic = iDataLogic;
            _IContraVoucher = IContraVoucher;
            _IWebHostEnvironment = iWebHostEnvironment;
            this.iconfiguration = iconfiguration;
            _connectionStringService = connectionStringService;
        }
        public async Task<JsonResult> GetFormRights()
        {
            var userID = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
            var JSON = await _IContraVoucher.GetFormRights(userID);
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
			//my_connection_string = iconfiguration.GetConnectionString("eTactDB");
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
        public async Task<ActionResult> ContraVoucher(int ID, string Mode, int YearCode, string VoucherNo, string FromDate = "", string ToDate = "", string LedgerName = "", string AgainstVoucherRefNo = "", string AgainstVoucherNo = "", string Searchbox = "", string DashboardType = "")
        {
            HttpContext.Session.Remove("KeyContraVoucherGrid");
            HttpContext.Session.Remove("KeyContraVoucherGridEdit");
            TempData.Clear();
            var MainModel = new ContraVoucherModel();
            MainModel.CC = HttpContext.Session.GetString("Branch");
            MainModel.YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
            MainModel.ActualEntryby = Convert.ToInt32(HttpContext.Session.GetString("UID"));
            MainModel.ActualEntryBy = HttpContext.Session.GetString("EmpName");
            MainModel.ActualEntryDate = DateTime.Now.ToString("dd/MM/yy");
            MainModel.UID = Convert.ToInt32(HttpContext.Session.GetString("UID"));
            MainModel.FromDate = HttpContext.Session.GetString("FromDate");
            MainModel.ToDate = HttpContext.Session.GetString("ToDate");

            if (!string.IsNullOrEmpty(Mode) && ID > 0 && (Mode == "U" || Mode == "V"))
            {
                MainModel = await _IContraVoucher.GetViewByID(ID, YearCode, VoucherNo).ConfigureAwait(false);
                MainModel.Mode = Mode; // Set Mode to Update
                MainModel.ID = ID;
                MainModel.VoucherNo = VoucherNo;

                string serializedGrid = JsonConvert.SerializeObject(MainModel.ContraVoucherGrid);
                HttpContext.Session.SetString("KeyContraVoucherGridEdit", serializedGrid);
            }
            if (MainModel.Mode == "U" && MainModel.UpdatedBy == 0)
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
        public async Task<IActionResult> ContraVoucher(ContraVoucherModel model)
        {
            try
            {
                var GIGrid = new DataTable();
                string modelJson = HttpContext.Session.GetString("KeyContraVoucherGrid");
                List<ContraVoucherModel> ContraVoucherGrid = new List<ContraVoucherModel>();
                if (!string.IsNullOrEmpty(modelJson))
                {
                    ContraVoucherGrid = JsonConvert.DeserializeObject<List<ContraVoucherModel>>(modelJson);
                }
                string modelEditJson = HttpContext.Session.GetString("KeyContraVoucherGridEdit");
                List<ContraVoucherModel> ContraVoucherGridEdit = new List<ContraVoucherModel>();
                if (!string.IsNullOrEmpty(modelEditJson))
                {
                    ContraVoucherGridEdit = JsonConvert.DeserializeObject<List<ContraVoucherModel>>(modelEditJson);
                }
                if (model.Mode == "U")
                {
                    GIGrid = GetDetailTable(ContraVoucherGridEdit);
                }
                else
                {
                    GIGrid = GetDetailTable(ContraVoucherGrid);
                }
                model.EntryByMachine = HttpContext.Session.GetString("ClientMachineName");
                model.IPAddress = HttpContext.Session.GetString("ClientIP");
                var Result = await _IContraVoucher.SaveContraVoucher(model, GIGrid);
                if (Result != null)
                {
                    if (Result.StatusText == "Success" && Result.StatusCode == HttpStatusCode.OK)
                    {
                        ViewBag.isSuccess = true;
                        TempData["200"] = "200";
                        HttpContext.Session.Remove("KeyContraVoucherGrid");
                    }
                    else if (Result.StatusText == "Success" && Result.StatusCode == HttpStatusCode.Accepted)
                    {
                        ViewBag.isSuccess = true;
                        TempData["202"] = "202";
                        HttpContext.Session.Remove("KeyContraVoucherGridEdit");
                    }
                    else if (Result.StatusText == "Error" && Result.StatusCode == HttpStatusCode.InternalServerError)
                    {
                        ViewBag.isSuccess = false;
                        TempData["500"] = "500";
                        _logger.LogError($"\n \n ********** LogError ********** \n {JsonConvert.SerializeObject(Result)}\n \n");
                        return View("Error", Result);
                    }
                }

                return RedirectToAction(nameof(ContraVoucher));

            }
            catch (Exception ex)
            {
                LogException<ContraVoucherController>.WriteException(_logger, ex);
                var ResponseResult = new ResponseResult
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    StatusText = "Error",
                    Result = ex
                };
                return View("Error", ResponseResult);
            }
        }
        private static DataTable GetDetailTable(IList<ContraVoucherModel> DetailList)
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
                Item.BillInvoiceDate=DateTime.Now.ToString("dd/MMM/yyyy") ,
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
                string.IsNullOrEmpty(Item.ChequeClearDate) ? null : ParseFormattedDate(Item.ChequeClearDate),
                string.IsNullOrEmpty(Item.ChequeClearDate) ? null : ParseFormattedDate(Item.ChequeClearDate),
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
                Item.ActualEntryby ,
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
        public async Task<JsonResult> GetLedgerBalance(int OpeningYearCode, int AccountCode, string VoucherDate)
        {
            var JSON = await _IContraVoucher.GetLedgerBalance(OpeningYearCode, AccountCode, VoucherDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillLedgerName(string VoucherType, string ShowAll)
        {
            var JSON = await _IContraVoucher.FillLedgerName(VoucherType, ShowAll);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillSubVoucherName(string VoucherType)
        {
            var JSON = await _IContraVoucher.FillSubVoucherName(VoucherType);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillBankType(int AccountCode)
        {
            var JSON = await _IContraVoucher.FillBankType(AccountCode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillIntrument(string VoucherType)
        {
            var JSON = await _IContraVoucher.FillIntrument(VoucherType);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillModeofAdjust(string VoucherType)
        {
            var JSON = await _IContraVoucher.FillModeofAdjust(VoucherType);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillCostCenterName()
        {
            var JSON = await _IContraVoucher.FillCostCenterName();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillEntryID(int YearCode, string VoucherDate)
        {
            var JSON = await _IContraVoucher.FillEntryID(YearCode, VoucherDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillCurrency()
        {
            var JSON = await _IContraVoucher.FillCurrency();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillSONO(string accountcode, string VoucherDate)
        {
            var JSON = await _IContraVoucher.FillSONO(accountcode, VoucherDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetSoYearCode(int SONO, string accountcode, string VoucherDate)
        {
            var JSON = await _IContraVoucher.GetSoYearCode(SONO, accountcode, VoucherDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetSODate(int SONO, string accountcode, string VoucherDate, string SOYearCode)
        {
            var JSON = await _IContraVoucher.GetSODate(SONO, accountcode, VoucherDate, SOYearCode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> CheckAmountBeforeSave(string VoucherDate, int YearCode, int AgainstVoucherYearCode, int AgainstVoucherEntryId, string AgainstVoucherNo, int AccountCode)
        {
            var JSON = await _IContraVoucher.CheckAmountBeforeSave(VoucherDate, YearCode, AgainstVoucherYearCode, AgainstVoucherEntryId, AgainstVoucherNo, AccountCode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public IActionResult AddContraVoucherDetail(ContraVoucherModel model)
        {
            try
            {
                if (model.Mode == "U" || model.Mode == "V")
                {
                    string modelJson = HttpContext.Session.GetString("KeyContraVoucherGridEdit");
                    List<ContraVoucherModel> ContraVoucherGrid = new List<ContraVoucherModel>();
                    if (!string.IsNullOrEmpty(modelJson))
                    {
                        ContraVoucherGrid = JsonConvert.DeserializeObject<List<ContraVoucherModel>>(modelJson);
                    }

                    var MainModel = new ContraVoucherModel();
                    var OrderGrid = ContraVoucherGrid?.Where(x => x != null).ToList() ?? new List<ContraVoucherModel>();

                    if (model != null)
                    {
                        bool isDuplicate = false;

                        if (model.BankType?.ToLower() == "bank")
                        {
                            isDuplicate = OrderGrid.Any(x => x.LedgerName == model.LedgerName );
                            if (isDuplicate) return StatusCode(210, "Duplicate");
                        }
                        else if (model.ModeOfAdjustment?.ToLower() == "new ref")
                        {
                            isDuplicate = OrderGrid.Any(x => x.LedgerName == model.LedgerName && x.ModeOfAdjustment == model.ModeOfAdjustment);
                            if (isDuplicate) return StatusCode(207, "Duplicate");
                        }
                        else if (model.ModeOfAdjustment?.ToLower() == "advance")
                        {
                            isDuplicate = OrderGrid.Any(x => x.LedgerName == model.LedgerName && x.ModeOfAdjustment == model.ModeOfAdjustment);
                            if (isDuplicate) return StatusCode(208, "Duplicate");
                        }
                        else if (model.ModeOfAdjustment?.ToLower() == "against ref")
                        {
                            isDuplicate = OrderGrid.Any(x => x.LedgerName == model.LedgerName && x.AgainstVoucherNo == model.AgainstVoucherNo);
                            if (isDuplicate) return StatusCode(209, "Duplicate");
                        }

                        // Assign the smallest missing SrNO
                        var usedSrNOs = OrderGrid.Select(x => x.SrNO).OrderBy(x => x).ToList();
                        int nextSrNo = 1;
                        foreach (var num in usedSrNOs)
                        {
                            if (num == nextSrNo)
                                nextSrNo++;
                            else
                                break;
                        }
                        model.SrNO = nextSrNo;

                        OrderGrid.Add(model);
                        MainModel.ContraVoucherGrid = OrderGrid.OrderBy(x => x.SrNO).ToList();
                        HttpContext.Session.SetString("KeyContraVoucherGridEdit", JsonConvert.SerializeObject(MainModel.ContraVoucherGrid));
                    }
                    else
                    {
                        ModelState.TryAddModelError("Error", "Schedule List Cannot Be Empty...!");
                    }
                    return PartialView("_ContraVoucherGrid", MainModel);
                }
                else
                {
                    string modelJson = HttpContext.Session.GetString("KeyContraVoucherGrid");
                    List<ContraVoucherModel> ContraVoucherGrid = new List<ContraVoucherModel>();
                    if (!string.IsNullOrEmpty(modelJson))
                    {
                        ContraVoucherGrid = JsonConvert.DeserializeObject<List<ContraVoucherModel>>(modelJson);
                    }

                    var MainModel = new ContraVoucherModel();
                    var OrderGrid = ContraVoucherGrid?.Where(x => x != null).ToList() ?? new List<ContraVoucherModel>();

                    if (model != null)
                    {
                        bool isDuplicate = false;

                        if (model.BankType?.ToLower() == "bank")
                        {
                            isDuplicate = OrderGrid.Any(x => x.LedgerName == model.LedgerName);
                            if (isDuplicate) return StatusCode(210, "Duplicate");
                        }
                        else if (model.ModeOfAdjustment?.ToLower() == "new ref")
                        {
                            isDuplicate = OrderGrid.Any(x => x.LedgerName == model.LedgerName && x.ModeOfAdjustment == model.ModeOfAdjustment);
                            if (isDuplicate) return StatusCode(207, "Duplicate");
                        }
                        else if (model.ModeOfAdjustment?.ToLower() == "advance")
                        {
                            isDuplicate = OrderGrid.Any(x => x.LedgerName == model.LedgerName && x.ModeOfAdjustment == model.ModeOfAdjustment);
                            if (isDuplicate) return StatusCode(208, "Duplicate");
                        }
                        else if (model.ModeOfAdjustment?.ToLower() == "against ref")
                        {
                            isDuplicate = OrderGrid.Any(x => x.LedgerName == model.LedgerName && x.AgainstVoucherNo == model.AgainstVoucherNo);
                            if (isDuplicate) return StatusCode(209, "Duplicate");
                        }

                        // Assign the smallest missing SrNO
                        var usedSrNOs = OrderGrid.Select(x => x.SrNO).OrderBy(x => x).ToList();
                        int nextSrNo = 1;
                        foreach (var num in usedSrNOs)
                        {
                            if (num == nextSrNo)
                                nextSrNo++;
                            else
                                break;
                        }
                        model.SrNO = nextSrNo;

                        OrderGrid.Add(model);
                        MainModel.ContraVoucherGrid = OrderGrid.OrderBy(x => x.SrNO).ToList();
                        HttpContext.Session.SetString("KeyContraVoucherGrid", JsonConvert.SerializeObject(MainModel.ContraVoucherGrid));
                    }
                    else
                    {
                        ModelState.TryAddModelError("Error", "Schedule List Cannot Be Empty...!");
                    }
                    return PartialView("_ContraVoucherGrid", MainModel);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IActionResult AddContraVoucherAdjustDetail(List<ContraVoucherModel> model, string Mode)
        {
            try
            {
                var MainModel = new ContraVoucherModel();
                var ProductionEntryDetail = new List<ContraVoucherModel>();

                if (Mode != "U" && Mode != "V")
                {
                    string modelJson = HttpContext.Session.GetString("KeyContraVoucherGrid");
                    List<ContraVoucherModel> ProductionEntryItemDetail = new List<ContraVoucherModel>();
                    if (!string.IsNullOrEmpty(modelJson))
                    {
                        ProductionEntryItemDetail = JsonConvert.DeserializeObject<List<ContraVoucherModel>>(modelJson);
                    }

                    if (ProductionEntryItemDetail != null)
                    {
                        ProductionEntryDetail = ProductionEntryItemDetail;
                    }

                    foreach (var item in model)
                    {
                        if (item != null)
                        {
                            var existingItem = ProductionEntryDetail.FirstOrDefault(x => x.LedgerName == item.LedgerName && x.AgainstVoucherNo == item.AgainstVoucherNo && x.ModeOfAdjustment == item.ModeOfAdjustment);
                            if (existingItem != null)
                            {
                                ProductionEntryDetail.Remove(existingItem);
                            }
                            item.SrNO = ProductionEntryDetail.Count + 1;
                            item.Type = item.Type.ToLower() == "dr" ? "CR" : "DR";
                            ProductionEntryDetail.Add(item);
                        }
                    }
                    for (int i = 0; i < ProductionEntryDetail.Count; i++)
                    {
                        ProductionEntryDetail[i].SrNO = i + 1; // Ensure proper sequence numbers
                    }
                    MainModel.ContraVoucherGrid = ProductionEntryDetail.OrderBy(x => x.SrNO).ToList();

                    string serializedGrid = JsonConvert.SerializeObject(MainModel.ContraVoucherGrid);
                    HttpContext.Session.SetString("KeyContraVoucherGrid", serializedGrid);
                }
                else
                {
                    string modelJson = HttpContext.Session.GetString("KeyContraVoucherGridEdit");
                    List<ContraVoucherModel> ProductionEntryItemDetail = new List<ContraVoucherModel>();
                    if (!string.IsNullOrEmpty(modelJson))
                    {
                        ProductionEntryItemDetail = JsonConvert.DeserializeObject<List<ContraVoucherModel>>(modelJson);
                    }

                    if (ProductionEntryItemDetail != null)
                    {
                        ProductionEntryDetail = ProductionEntryItemDetail;
                    }

                    foreach (var item in model)
                    {
                        if (item != null)
                        {
                            var existingItem = ProductionEntryDetail.FirstOrDefault(x => x.LedgerName == item.LedgerName && x.AgainstVoucherNo == item.AgainstVoucherNo && x.ModeOfAdjustment == item.ModeOfAdjustment);
                            if (existingItem != null)
                            {
                                ProductionEntryDetail.Remove(existingItem);
                            }
                            item.Type = item.Type.ToLower() == "dr" ? "CR" : "DR";
                            ProductionEntryDetail.Add(item);
                        }
                    }
                    for (int i = 0; i < ProductionEntryDetail.Count; i++)
                    {
                        ProductionEntryDetail[i].SrNO = i + 1; // Ensure proper sequence numbers
                    }
                    MainModel.ContraVoucherGrid = ProductionEntryDetail.OrderBy(x => x.SrNO).ToList();
                    string serializedGrid = JsonConvert.SerializeObject(MainModel.ContraVoucherGrid);
                    HttpContext.Session.SetString("KeyContraVoucherGridEdit", serializedGrid);
                }
                return PartialView("_ContraVoucherGrid", MainModel);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<JsonResult> EditItemRows(int SrNO, string Mode)
        {
            if (Mode != "U" && Mode != "V")
            {
                var MainModel = new ContraVoucherModel();
                string modelJson = HttpContext.Session.GetString("KeyContraVoucherGrid");
                List<ContraVoucherModel> GridDetail = new List<ContraVoucherModel>();
                if (!string.IsNullOrEmpty(modelJson))
                {
                    GridDetail = JsonConvert.DeserializeObject<List<ContraVoucherModel>>(modelJson);
                }
                var SAGrid = GridDetail.Where(x => x.SrNO == SrNO);
                string JsonString = JsonConvert.SerializeObject(SAGrid);
                return Json(JsonString);
            }
            else
            {
                var MainModel = new ContraVoucherModel();
                string modelJson = HttpContext.Session.GetString("KeyContraVoucherGridEdit");
                List<ContraVoucherModel> GridDetail = new List<ContraVoucherModel>();
                if (!string.IsNullOrEmpty(modelJson))
                {
                    GridDetail = JsonConvert.DeserializeObject<List<ContraVoucherModel>>(modelJson);
                }

                var SAGrid = GridDetail.Where(x => x.SrNO == SrNO);
                string JsonString = JsonConvert.SerializeObject(SAGrid);
                return Json(JsonString);
            }
        }
        public IActionResult DeleteItemRow(int SeqNo, string Mode, string PopUpData)
        {
            var MainModel = new ContraVoucherModel();

            if (PopUpData == "PopUpData")
            {
                string modelJson = HttpContext.Session.GetString("KeyContraVoucherGridPopUpData");
                List<ContraVoucherModel> ContraVoucherGrid = new List<ContraVoucherModel>();

                if (!string.IsNullOrEmpty(modelJson))
                {
                    ContraVoucherGrid = JsonConvert.DeserializeObject<List<ContraVoucherModel>>(modelJson);
                }

                int Indx = Convert.ToInt32(SeqNo) - 1;

                if (ContraVoucherGrid != null && ContraVoucherGrid.Count > 0 && Indx >= 0 && Indx < ContraVoucherGrid.Count)
                {
                    ContraVoucherGrid.RemoveAt(Indx);

                    // Don't update SrNO — keep as is
                    MainModel.ContraVoucherGrid = ContraVoucherGrid.OrderBy(x => x.SrNO).ToList();

                    string serializedGrid = JsonConvert.SerializeObject(MainModel.ContraVoucherGrid);
                    HttpContext.Session.SetString("KeyContraVoucherGridPopUpData", serializedGrid);
                }

                return PartialView("_DisplayPopupForPendingVouchers", MainModel);
            }
            else
            {
                if (Mode != "U" && Mode != "V")
                {
                    string modelJson = HttpContext.Session.GetString("KeyContraVoucherGrid");
                    List<ContraVoucherModel> ContraVoucherGrid = new List<ContraVoucherModel>();

                    if (!string.IsNullOrEmpty(modelJson))
                    {
                        ContraVoucherGrid = JsonConvert.DeserializeObject<List<ContraVoucherModel>>(modelJson);
                    }

                    int Indx = Convert.ToInt32(SeqNo) - 1;

                    if (ContraVoucherGrid != null && ContraVoucherGrid.Count > 0 && Indx >= 0 && Indx < ContraVoucherGrid.Count)
                    {
                        ContraVoucherGrid.RemoveAt(Indx);

                        // Don't update SrNO — keep as is
                        MainModel.ContraVoucherGrid = ContraVoucherGrid.OrderBy(x => x.SrNO).ToList();

                        string serializedGrid = JsonConvert.SerializeObject(MainModel.ContraVoucherGrid);
                        HttpContext.Session.SetString("KeyContraVoucherGrid", serializedGrid);
                    }
                }
                else
                {
                    string modelJson = HttpContext.Session.GetString("KeyContraVoucherGridEdit");
                    List<ContraVoucherModel> ContraVoucherGrid = new List<ContraVoucherModel>();

                    if (!string.IsNullOrEmpty(modelJson))
                    {
                        ContraVoucherGrid = JsonConvert.DeserializeObject<List<ContraVoucherModel>>(modelJson);
                    }

                    int Indx = Convert.ToInt32(SeqNo) - 1;

                    if (ContraVoucherGrid != null && ContraVoucherGrid.Count > 0 && Indx >= 0 && Indx < ContraVoucherGrid.Count)
                    {
                        ContraVoucherGrid.RemoveAt(Indx);

                        // Don't update SrNO — keep as is
                        MainModel.ContraVoucherGrid = ContraVoucherGrid.OrderBy(x => x.SrNO).ToList();

                        string serializedGrid = JsonConvert.SerializeObject(MainModel.ContraVoucherGrid);
                        HttpContext.Session.SetString("KeyContraVoucherGridEdit", serializedGrid);
                    }
                }

                return PartialView("_ContraVoucherGrid", MainModel);
            }
        }
        public async Task<IActionResult> ContraVoucherDashBoard(string FromDate = "", string ToDate = "", string Flag = "True", string LedgerName = "", string Bank = "", string VoucherNo = "", string AgainstVoucherRefNo = "", string AgainstVoucherNo = "", string Searchbox = "", string DashboardType = "")
        {
            try
            {
                HttpContext.Session.Remove("KeyContraVoucherGrid");
                HttpContext.Session.Remove("KeyContraVoucherGridEdit");
                var model = new ContraVoucherModel();
                FromDate = HttpContext.Session.GetString("FromDate");
                ToDate = HttpContext.Session.GetString("ToDate");
                var Result = await _IContraVoucher.GetDashBoardData(FromDate, ToDate).ConfigureAwait(true);
                if (Result != null)
                {
                    DataSet ds = Result.Result;
                    if (ds != null && ds.Tables.Count > 0)
                    {
                        var dt = ds.Tables[0];
                        model.ContraVoucherGrid = CommonFunc.DataTableToList<ContraVoucherModel>(dt, "ContraVoucherDashBoard");

                        if (Flag != "True")
                        {
                            model.FromDate1 = FromDate;
                            model.ToDate1 = ToDate;
                            model.LedgerName = LedgerName;
                            model.Bank = Bank;
                            model.VoucherNo = VoucherNo;
                            model.AgainstVoucherRefNo = AgainstVoucherRefNo;
                            model.AgainstVoucherNo = AgainstVoucherNo;
                            model.Searchbox = Searchbox;
                            model.DashboardType = DashboardType;
                            return View(model);
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
        public async Task<IActionResult> GetDashBoardDetailData(string FromDate, string ToDate, string LedgerName, string Bank, string VoucherNo)
        {
            HttpContext.Session.Remove("KeyContraVoucherGrid");
            HttpContext.Session.Remove("KeyContraVoucherGridEdit");
            var model = new ContraVoucherModel();
            model = await _IContraVoucher.GetDashBoardDetailData(FromDate, ToDate, LedgerName, Bank, VoucherNo);
            return PartialView("_ContraVoucherDashBoardDetailGrid", model);
        }
        public async Task<IActionResult> GetDashBoardSummaryData(string FromDate, string ToDate, string LedgerName, string Bank, string VoucherNo)
        {
            HttpContext.Session.Remove("KeyContraVoucherGrid");
            HttpContext.Session.Remove("KeyContraVoucherGridEdit");
            var model = new ContraVoucherModel();
            model = await _IContraVoucher.GetDashBoardSummaryData(FromDate, ToDate, LedgerName, Bank, VoucherNo);
            return PartialView("_ContraVoucherDashBoardGrid", model);
        }
        public async Task<IActionResult> PopUpForPendingVouchers(PopUpDataTableAgainstRef DataTable)
        {
            HttpContext.Session.Remove("KeyContraVoucherGridPopUpData");
            var model = await _IContraVoucher.PopUpForPendingVouchers(DataTable);
            string serializedGrid = JsonConvert.SerializeObject(model.ContraVoucherGrid);
            HttpContext.Session.SetString("KeyContraVoucherGridPopUpData", serializedGrid);
            return PartialView("_DisplayPopupForPendingVouchers", model);
        }
        public async Task<IActionResult> DeleteByID(int ID, int YearCode, int ActualEntryBy, string EntryByMachine, string ActualEntryDate, string VoucherType, string FromDate = "", string ToDate = "", string LedgerName = "", string Bank = "", string VoucherNo = "", string AgainstVoucherRefNo = "", string AgainstVoucherNo = "", string Searchbox = "", string DashboardType = "")
        {
            var Result = await _IContraVoucher.DeleteByID(ID, YearCode, ActualEntryBy, EntryByMachine, ActualEntryDate, VoucherType);

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

            DateTime fromDt = DateTime.ParseExact(FromDate, "dd/MM/yyyy", null);
            string formattedFromDate = fromDt.ToString("dd/MMM/yyyy 00:00:00");
            DateTime toDt = DateTime.ParseExact(ToDate, "dd/MM/yyyy", null);
            string formattedToDate = toDt.ToString("dd/MMM/yyyy 00:00:00");

            return RedirectToAction("ContraVoucherDashBoard", new { FromDate = formattedFromDate, ToDate = formattedToDate, Flag = "False", LedgerName = LedgerName, Bank = Bank, VoucherNo = VoucherNo, AgainstVoucherRefNo = AgainstVoucherRefNo, AgainstVoucherNo = AgainstVoucherNo, Searchbox = Searchbox, DashboardType = DashboardType });

        }
        public async Task<JsonResult> FillLedgerInDashboard(string FromDate, string ToDate, string VoucherType)
        {
            var JSON = await _IContraVoucher.FillLedgerInDashboard(FromDate, ToDate, VoucherType);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillBankInDashboard(string FromDate, string ToDate, string VoucherType)
        {
            var JSON = await _IContraVoucher.FillBankInDashboard(FromDate, ToDate, VoucherType);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillVoucherNoInDashboard(string FromDate, string ToDate, string VoucherType)
        {
            var JSON = await _IContraVoucher.FillVoucherNoInDashboard(FromDate, ToDate, VoucherType);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
       
    }
}
