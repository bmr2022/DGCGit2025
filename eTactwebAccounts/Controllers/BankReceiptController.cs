using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using FastReport;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System.Data;
using System.Net;
using static eTactWeb.Data.Common.CommonFunc;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Controllers
{
    public class BankReceiptController : Controller
    {
        private readonly IDataLogic _IDataLogic;
        public IBankReceipt _IBankReceipt { get; }
        private readonly ILogger<BankReceiptController> _logger;
        private readonly IConfiguration iconfiguration;
        public IWebHostEnvironment _IWebHostEnvironment { get; }
        public BankReceiptController(ILogger<BankReceiptController> logger, IDataLogic iDataLogic, IBankReceipt iBankReceipt, EncryptDecrypt encryptDecrypt, IWebHostEnvironment iWebHostEnvironment, IConfiguration iconfiguration)
        {
            _logger = logger;
            _IDataLogic = iDataLogic;
            _IBankReceipt = iBankReceipt;
            _IWebHostEnvironment = iWebHostEnvironment;
            this.iconfiguration = iconfiguration;
        }
        [Route("{controller}/Index")]
        [HttpGet]
        public async Task<ActionResult> BankReceipt(int ID, string Mode, int YearCode, string VoucherNo, string FromDate = "", string ToDate = "", string LedgerName = "", string AgainstVoucherRefNo = "", string AgainstVoucherNo = "", string Searchbox = "", string DashboardType = "")
        {
            HttpContext.Session.Remove("KeyBankReceiptGrid");
            HttpContext.Session.Remove("KeyBankReceiptGridEdit");
            TempData.Clear();
            var MainModel = new BankReceiptModel();
            MainModel.CC = HttpContext.Session.GetString("Branch");
            MainModel.YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
            MainModel.ActualEntryby = Convert.ToInt32(HttpContext.Session.GetString("UID"));
            MainModel.ActualEntryBy = HttpContext.Session.GetString("EmpName");
            MainModel.ActualEntryDate = DateTime.Now.ToString("dd/MM/yy");
            MainModel.UID = Convert.ToInt32(HttpContext.Session.GetString("UID"));
            MainModel.FromDate = HttpContext.Session.GetString("FromDate");
            MainModel.ToDate = HttpContext.Session.GetString("ToDate");

            if (MainModel.Mode == "U")
            {
                MainModel.UpdatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                MainModel.UpdatedByEmp = HttpContext.Session.GetString("EmpName");
                MainModel.UpdatedOn = DateTime.Now;
            }
            if (!string.IsNullOrEmpty(Mode) && ID > 0 && (Mode == "U" || Mode == "V"))
            {
                MainModel = await _IBankReceipt.GetViewByID(ID, YearCode, VoucherNo).ConfigureAwait(false);
                MainModel.Mode = Mode; // Set Mode to Update
                MainModel.ID = ID;
                MainModel.VoucherNo = VoucherNo;

                string serializedGrid = JsonConvert.SerializeObject(MainModel.BankReceiptGrid);
                HttpContext.Session.SetString("KeyBankReceiptGridEdit", serializedGrid);
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
        public async Task<IActionResult> BankReceipt(BankReceiptModel model)
        {
            try
            {
                var GIGrid = new DataTable();
                string modelJson = HttpContext.Session.GetString("KeyBankReceiptGrid");
                List<BankReceiptModel> BankReceiptGrid = new List<BankReceiptModel>();
                if (!string.IsNullOrEmpty(modelJson))
                {
                    BankReceiptGrid = JsonConvert.DeserializeObject<List<BankReceiptModel>>(modelJson);
                }
                string modelEditJson = HttpContext.Session.GetString("KeyBankReceiptGridEdit");
                List<BankReceiptModel> BankReceiptGridEdit = new List<BankReceiptModel>();
                if (!string.IsNullOrEmpty(modelEditJson))
                {
                    BankReceiptGridEdit = JsonConvert.DeserializeObject<List<BankReceiptModel>>(modelEditJson);
                }

                model.ActualEntryby = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                model.ActualEntryBy = HttpContext.Session.GetString("UID");
                if (model.Mode == "U")
                {
                    GIGrid = GetDetailTable(BankReceiptGridEdit);
                }
                else
                {
                    GIGrid = GetDetailTable(BankReceiptGrid);
                }
                var Result = await _IBankReceipt.SaveBankReceipt(model, GIGrid);
                if (Result != null)
                {
                    if (Result.StatusText == "Success" && Result.StatusCode == HttpStatusCode.OK)
                    {
                        ViewBag.isSuccess = true;
                        TempData["200"] = "200";
                        HttpContext.Session.Remove("KeyBankReceiptGrid");
                    }
                    else if (Result.StatusText == "Success" && Result.StatusCode == HttpStatusCode.Accepted)
                    {
                        ViewBag.isSuccess = true;
                        TempData["202"] = "202";
                        HttpContext.Session.Remove("KeyBankReceiptGridEdit");
                    }
                    else if (Result.StatusText == "Error" && Result.StatusCode == HttpStatusCode.InternalServerError)
                    {
                        ViewBag.isSuccess = false;
                        TempData["500"] = "500";
                        _logger.LogError($"\n \n ********** LogError ********** \n {JsonConvert.SerializeObject(Result)}\n \n");
                        return View("Error", Result);
                    }
                }

                return RedirectToAction(nameof(BankReceipt));

            }
            catch (Exception ex)
            {
                LogException<BankReceiptController>.WriteException(_logger, ex);
                var ResponseResult = new ResponseResult
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    StatusText = "Error",
                    Result = ex
                };
                return View("Error", ResponseResult);
            }
        }
        private static DataTable GetDetailTable(IList<BankReceiptModel> DetailList)
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
            var JSON = await _IBankReceipt.GetLedgerBalance(OpeningYearCode, AccountCode, VoucherDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillLedgerName(string VoucherType, string ShowAll)
        {
            var JSON = await _IBankReceipt.FillLedgerName(VoucherType, ShowAll);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillSubVoucherName(string VoucherType)
        {
            var JSON = await _IBankReceipt.FillSubVoucherName(VoucherType);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillBankType(int AccountCode)
        {
            var JSON = await _IBankReceipt.FillBankType(AccountCode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillIntrument(string VoucherType)
        {
            var JSON = await _IBankReceipt.FillIntrument(VoucherType);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillModeofAdjust(string VoucherType)
        {
            var JSON = await _IBankReceipt.FillModeofAdjust(VoucherType);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillCostCenterName()
        {
            var JSON = await _IBankReceipt.FillCostCenterName();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillEntryID(int YearCode, string VoucherDate)
        {
            var JSON = await _IBankReceipt.FillEntryID(YearCode, VoucherDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillCurrency()
        {
            var JSON = await _IBankReceipt.FillCurrency();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillSONO(string accountcode, string VoucherDate)
        {
            var JSON = await _IBankReceipt.FillSONO(accountcode, VoucherDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetSoYearCode(int SONO, string accountcode, string VoucherDate)
        {
            var JSON = await _IBankReceipt.GetSoYearCode(SONO, accountcode, VoucherDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetSODate(int SONO, string accountcode, string VoucherDate, string SOYearCode)
        {
            var JSON = await _IBankReceipt.GetSODate(SONO, accountcode, VoucherDate, SOYearCode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> CheckAmountBeforeSave(string VoucherDate, int YearCode, int AgainstVoucherYearCode, int AgainstVoucherEntryId, string AgainstVoucherNo, int AccountCode)
        {
            var JSON = await _IBankReceipt.CheckAmountBeforeSave(VoucherDate, YearCode, AgainstVoucherYearCode, AgainstVoucherEntryId, AgainstVoucherNo, AccountCode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public IActionResult AddBankReceiptDetail(BankReceiptModel model)
        {
            try
            {
                if (model.Mode == "U" || model.Mode == "V")
                {
                    string modelJson = HttpContext.Session.GetString("KeyBankReceiptGridEdit");
                    List<BankReceiptModel> BankReceiptGrid = new List<BankReceiptModel>();
                    if (!string.IsNullOrEmpty(modelJson))
                    {
                        BankReceiptGrid = JsonConvert.DeserializeObject<List<BankReceiptModel>>(modelJson);
                    }

                    var MainModel = new BankReceiptModel();
                    var OrderGrid = BankReceiptGrid?.Where(x => x != null).ToList() ?? new List<BankReceiptModel>();

                    if (model != null)
                    {
                        bool isDuplicate = false;

                        if (model.BankType?.ToLower() == "bank")
                        {
                            isDuplicate = OrderGrid.Any(x => x.LedgerName == model.LedgerName || x.BankType == "Bank");
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
                        MainModel.BankReceiptGrid = OrderGrid.OrderBy(x => x.SrNO).ToList();
                        HttpContext.Session.SetString("KeyBankReceiptGridEdit", JsonConvert.SerializeObject(MainModel.BankReceiptGrid));
                    }
                    else
                    {
                        ModelState.TryAddModelError("Error", "Schedule List Cannot Be Empty...!");
                    }
                    return PartialView("_BankReceiptGrid", MainModel);
                }
                else
                {
                    string modelJson = HttpContext.Session.GetString("KeyBankReceiptGrid");
                    List<BankReceiptModel> BankReceiptGrid = new List<BankReceiptModel>();
                    if (!string.IsNullOrEmpty(modelJson))
                    {
                        BankReceiptGrid = JsonConvert.DeserializeObject<List<BankReceiptModel>>(modelJson);
                    }

                    var MainModel = new BankReceiptModel();
                    var OrderGrid = BankReceiptGrid?.Where(x => x != null).ToList() ?? new List<BankReceiptModel>();

                    if (model != null)
                    {
                        bool isDuplicate = false;

                        if (model.BankType?.ToLower() == "bank")
                        {
                            isDuplicate = OrderGrid.Any(x => x.LedgerName == model.LedgerName || x.BankType == "Bank");
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
                        MainModel.BankReceiptGrid = OrderGrid.OrderBy(x => x.SrNO).ToList();
                        HttpContext.Session.SetString("KeyBankReceiptGrid", JsonConvert.SerializeObject(MainModel.BankReceiptGrid));
                    }
                    else
                    {
                        ModelState.TryAddModelError("Error", "Schedule List Cannot Be Empty...!");
                    }
                    return PartialView("_BankReceiptGrid", MainModel);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IActionResult AddBankReceiptAdjustDetail(List<BankReceiptModel> model, string Mode)
        {
            try
            {
                var MainModel = new BankReceiptModel();
                var ProductionEntryDetail = new List<BankReceiptModel>();

                if (Mode != "U" && Mode != "V")
                {
                    string modelJson = HttpContext.Session.GetString("KeyBankReceiptGrid");
                    List<BankReceiptModel> ProductionEntryItemDetail = new List<BankReceiptModel>();
                    if (!string.IsNullOrEmpty(modelJson))
                    {
                        ProductionEntryItemDetail = JsonConvert.DeserializeObject<List<BankReceiptModel>>(modelJson);
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
                    MainModel.BankReceiptGrid = ProductionEntryDetail.OrderBy(x => x.SrNO).ToList();

                    string serializedGrid = JsonConvert.SerializeObject(MainModel.BankReceiptGrid);
                    HttpContext.Session.SetString("KeyBankReceiptGrid", serializedGrid);
                }
                else
                {
                    string modelJson = HttpContext.Session.GetString("KeyBankReceiptGridEdit");
                    List<BankReceiptModel> ProductionEntryItemDetail = new List<BankReceiptModel>();
                    if (!string.IsNullOrEmpty(modelJson))
                    {
                        ProductionEntryItemDetail = JsonConvert.DeserializeObject<List<BankReceiptModel>>(modelJson);
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
                    MainModel.BankReceiptGrid = ProductionEntryDetail.OrderBy(x => x.SrNO).ToList();
                    string serializedGrid = JsonConvert.SerializeObject(MainModel.BankReceiptGrid);
                    HttpContext.Session.SetString("KeyBankReceiptGridEdit", serializedGrid);
                }
                return PartialView("_BankReceiptGrid", MainModel);
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
                var MainModel = new BankReceiptModel();
                string modelJson = HttpContext.Session.GetString("KeyBankReceiptGrid");
                List<BankReceiptModel> GridDetail = new List<BankReceiptModel>();
                if (!string.IsNullOrEmpty(modelJson))
                {
                    GridDetail = JsonConvert.DeserializeObject<List<BankReceiptModel>>(modelJson);
                }
                var SAGrid = GridDetail.Where(x => x.SrNO == SrNO);
                string JsonString = JsonConvert.SerializeObject(SAGrid);
                return Json(JsonString);
            }
            else
            {
                var MainModel = new BankReceiptModel();
                string modelJson = HttpContext.Session.GetString("KeyBankReceiptGridEdit");
                List<BankReceiptModel> GridDetail = new List<BankReceiptModel>();
                if (!string.IsNullOrEmpty(modelJson))
                {
                    GridDetail = JsonConvert.DeserializeObject<List<BankReceiptModel>>(modelJson);
                }

                var SAGrid = GridDetail.Where(x => x.SrNO == SrNO);
                string JsonString = JsonConvert.SerializeObject(SAGrid);
                return Json(JsonString);
            }
        }
        public IActionResult DeleteItemRow(int SeqNo, string Mode, string PopUpData)
        {
            var MainModel = new BankReceiptModel();

            if (PopUpData == "PopUpData")
            {
                string modelJson = HttpContext.Session.GetString("KeyBankReceiptGridPopUpData");
                List<BankReceiptModel> BankReceiptGrid = new List<BankReceiptModel>();

                if (!string.IsNullOrEmpty(modelJson))
                {
                    BankReceiptGrid = JsonConvert.DeserializeObject<List<BankReceiptModel>>(modelJson);
                }

                int Indx = Convert.ToInt32(SeqNo) - 1;

                if (BankReceiptGrid != null && BankReceiptGrid.Count > 0 && Indx >= 0 && Indx < BankReceiptGrid.Count)
                {
                    BankReceiptGrid.RemoveAt(Indx);

                    // Don't update SrNO — keep as is
                    MainModel.BankReceiptGrid = BankReceiptGrid.OrderBy(x => x.SrNO).ToList();

                    string serializedGrid = JsonConvert.SerializeObject(MainModel.BankReceiptGrid);
                    HttpContext.Session.SetString("KeyBankReceiptGridPopUpData", serializedGrid);
                }

                return PartialView("_DisplayPopupForPendingVouchers", MainModel);
            }
            else
            {
                if (Mode != "U" && Mode != "V")
                {
                    string modelJson = HttpContext.Session.GetString("KeyBankReceiptGrid");
                    List<BankReceiptModel> BankReceiptGrid = new List<BankReceiptModel>();

                    if (!string.IsNullOrEmpty(modelJson))
                    {
                        BankReceiptGrid = JsonConvert.DeserializeObject<List<BankReceiptModel>>(modelJson);
                    }

                    int Indx = Convert.ToInt32(SeqNo) - 1;

                    if (BankReceiptGrid != null && BankReceiptGrid.Count > 0 && Indx >= 0 && Indx < BankReceiptGrid.Count)
                    {
                        BankReceiptGrid.RemoveAt(Indx);

                        // Don't update SrNO — keep as is
                        MainModel.BankReceiptGrid = BankReceiptGrid.OrderBy(x => x.SrNO).ToList();

                        string serializedGrid = JsonConvert.SerializeObject(MainModel.BankReceiptGrid);
                        HttpContext.Session.SetString("KeyBankReceiptGrid", serializedGrid);
                    }
                }
                else
                {
                    string modelJson = HttpContext.Session.GetString("KeyBankReceiptGridEdit");
                    List<BankReceiptModel> BankReceiptGrid = new List<BankReceiptModel>();

                    if (!string.IsNullOrEmpty(modelJson))
                    {
                        BankReceiptGrid = JsonConvert.DeserializeObject<List<BankReceiptModel>>(modelJson);
                    }

                    int Indx = Convert.ToInt32(SeqNo) - 1;

                    if (BankReceiptGrid != null && BankReceiptGrid.Count > 0 && Indx >= 0 && Indx < BankReceiptGrid.Count)
                    {
                        BankReceiptGrid.RemoveAt(Indx);

                        // Don't update SrNO — keep as is
                        MainModel.BankReceiptGrid = BankReceiptGrid.OrderBy(x => x.SrNO).ToList();

                        string serializedGrid = JsonConvert.SerializeObject(MainModel.BankReceiptGrid);
                        HttpContext.Session.SetString("KeyBankReceiptGridEdit", serializedGrid);
                    }
                }

                return PartialView("_BankReceiptGrid", MainModel);
            }
        }
        public async Task<IActionResult> BankReceiptDashBoard(string FromDate = "", string ToDate = "",string Flag = "True", string LedgerName = "", string Bank = "", string VoucherNo = "", string AgainstVoucherRefNo = "", string AgainstVoucherNo = "", string Searchbox = "", string DashboardType = "")
        {
            try
            {
                HttpContext.Session.Remove("KeyBankReceiptGrid");
                HttpContext.Session.Remove("KeyBankReceiptGridEdit");
                var model = new BankReceiptModel();
                FromDate = HttpContext.Session.GetString("FromDate");
                ToDate = HttpContext.Session.GetString("ToDate");
                var Result = await _IBankReceipt.GetDashBoardData(FromDate, ToDate).ConfigureAwait(true);
                if (Result != null)
                {
                    DataSet ds = Result.Result;
                    if (ds != null && ds.Tables.Count > 0)
                    {
                        var dt = ds.Tables[0];
                        model.BankReceiptGrid = CommonFunc.DataTableToList<BankReceiptModel>(dt, "BankReceiptDashBoard");

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
        public async Task<IActionResult> GetDashBoardDetailData(string FromDate, string ToDate, string LedgerName, string VoucherNo, string AgainstVoucherRefNo, string AgainstVoucherNo)
        {
            HttpContext.Session.Remove("KeyBankReceiptGrid");
            HttpContext.Session.Remove("KeyBankReceiptGridEdit");
            var model = new BankReceiptModel();
            model = await _IBankReceipt.GetDashBoardDetailData(FromDate, ToDate, LedgerName, VoucherNo, AgainstVoucherRefNo, AgainstVoucherNo);
            return PartialView("_BankReceiptDashBoardDetailGrid", model);
        }
        public async Task<IActionResult> GetDashBoardSummaryData(string FromDate, string ToDate, string LedgerName, string VoucherNo, string AgainstVoucherRefNo, string AgainstVoucherNo)
        {
            HttpContext.Session.Remove("KeyBankReceiptGrid");
            HttpContext.Session.Remove("KeyBankReceiptGridEdit");
            var model = new BankReceiptModel();
            model = await _IBankReceipt.GetDashBoardSummaryData(FromDate, ToDate, LedgerName, VoucherNo, AgainstVoucherRefNo, AgainstVoucherNo);
            return PartialView("_BankReceiptDashBoardGrid", model);
        }
        public async Task<IActionResult> PopUpForPendingVouchers(PopUpDataTableAgainstRef DataTable)
        {
            HttpContext.Session.Remove("KeyBankReceiptGridPopUpData");
            var model = await _IBankReceipt.PopUpForPendingVouchers(DataTable);
            string serializedGrid = JsonConvert.SerializeObject(model.BankReceiptGrid);
            HttpContext.Session.SetString("KeyBankReceiptGridPopUpData", serializedGrid);
            return PartialView("_DisplayPopupForPendingVouchers", model);
        }
        public async Task<IActionResult> DeleteByID(int ID, int YearCode, int ActualEntryBy, string EntryByMachine, string ActualEntryDate, string VoucherType, string FromDate = "", string ToDate = "", string LedgerName = "", string Bank = "", string VoucherNo = "", string AgainstVoucherRefNo = "", string AgainstVoucherNo = "", string Searchbox = "", string DashboardType = "")
        {
            var Result = await _IBankReceipt.DeleteByID(ID, YearCode, ActualEntryBy, EntryByMachine, ActualEntryDate, VoucherType);

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

            return RedirectToAction("BankReceiptDashBoard", new { FromDate = formattedFromDate, ToDate = formattedToDate, Flag = "False", LedgerName = LedgerName, Bank = Bank, VoucherNo = VoucherNo, AgainstVoucherRefNo = AgainstVoucherRefNo, AgainstVoucherNo = AgainstVoucherNo, Searchbox = Searchbox, DashboardType = DashboardType });

        }
        public async Task<JsonResult> FillLedgerInDashboard(string FromDate, string ToDate, string VoucherType)
        {
            var JSON = await _IBankReceipt.FillLedgerInDashboard(FromDate, ToDate, VoucherType);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillBankInDashboard(string FromDate, string ToDate, string VoucherType)
        {
            var JSON = await _IBankReceipt.FillBankInDashboard(FromDate, ToDate, VoucherType);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillVoucherNoInDashboard(string FromDate, string ToDate, string VoucherType)
        {
            var JSON = await _IBankReceipt.FillVoucherNoInDashboard(FromDate, ToDate, VoucherType);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillAgainstVoucherRefNoInDashboard(string FromDate, string ToDate, string VoucherType)
        {
            var JSON = await _IBankReceipt.FillAgainstVoucherRefNoInDashboard(FromDate, ToDate, VoucherType);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillAgainstVoucherNoInDashboard(string FromDate, string ToDate, string VoucherType)
        {
            var JSON = await _IBankReceipt.FillAgainstVoucherNoInDashboard(FromDate, ToDate, VoucherType);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
    }
}
