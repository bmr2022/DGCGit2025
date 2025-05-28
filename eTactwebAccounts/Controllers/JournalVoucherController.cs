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

namespace eTactwebAccounts.Controllers
{
    public class JournalVoucherController : Controller
    {
        private readonly IDataLogic _IDataLogic;
        public IJournalVoucher _IJournalVoucher { get; }
        private readonly ILogger<JournalVoucherController> _logger;
        private readonly IConfiguration iconfiguration;
        public IWebHostEnvironment _IWebHostEnvironment { get; }
        public JournalVoucherController(ILogger<JournalVoucherController> logger, IDataLogic iDataLogic, IJournalVoucher IJournalVoucher, EncryptDecrypt encryptDecrypt, IWebHostEnvironment iWebHostEnvironment, IConfiguration iconfiguration)
        {
            _logger = logger;
            _IDataLogic = iDataLogic;
            _IJournalVoucher = IJournalVoucher;
            _IWebHostEnvironment = iWebHostEnvironment;
            this.iconfiguration = iconfiguration;
        }
        [Route("{controller}/Index")]
        [HttpGet]
        public async Task<ActionResult> JournalVoucher(int ID, string Mode, int YearCode, string VoucherNo, string FromDate = "", string ToDate = "", string LedgerName = "", string AgainstVoucherRefNo = "", string AgainstVoucherNo = "", string Searchbox = "", string DashboardType = "")
        {
            HttpContext.Session.Remove("KeyJournalVoucherGrid");
            HttpContext.Session.Remove("KeyJournalVoucherGridEdit");
            TempData.Clear();
            var MainModel = new JournalVoucherModel();
            MainModel.CC = HttpContext.Session.GetString("Branch");
            MainModel.YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
            MainModel.ActualEntryBy = HttpContext.Session.GetString("UID");
            MainModel.ActualEntryDate = DateTime.Now.ToString("dd/MM/yy");
            MainModel.UID = Convert.ToInt32(HttpContext.Session.GetString("UID"));

            if (!string.IsNullOrEmpty(Mode) && ID > 0 && (Mode == "U" || Mode == "V"))
            {
                MainModel = await _IJournalVoucher.GetViewByID(ID, YearCode, VoucherNo).ConfigureAwait(false);
                MainModel.Mode = Mode; // Set Mode to Update
                MainModel.ID = ID;
                MainModel.VoucherNo = VoucherNo;

                string serializedGrid = JsonConvert.SerializeObject(MainModel.JournalVoucherList);
                HttpContext.Session.SetString("KeyJournalVoucherGridEdit", serializedGrid);
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
        public async Task<IActionResult> JournalVoucher(JournalVoucherModel model)
        {
            try
            {
                var GIGrid = new DataTable();
                string modelJson = HttpContext.Session.GetString("KeyJournalVoucherGrid");
                List<JournalVoucherModel> JournalVoucherGrid = new List<JournalVoucherModel>();
                if (!string.IsNullOrEmpty(modelJson))
                {
                    JournalVoucherGrid = JsonConvert.DeserializeObject<List<JournalVoucherModel>>(modelJson);
                }
                string modelEditJson = HttpContext.Session.GetString("KeyJournalVoucherGridEdit");
                List<JournalVoucherModel> JournalVoucherGridEdit = new List<JournalVoucherModel>();
                if (!string.IsNullOrEmpty(modelEditJson))
                {
                    JournalVoucherGridEdit = JsonConvert.DeserializeObject<List<JournalVoucherModel>>(modelEditJson);
                }

                model.ActualEntryby = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                model.ActualEntryBy = HttpContext.Session.GetString("UID");
                if (model.Mode == "U")
                {
                    GIGrid = GetDetailTable(JournalVoucherGridEdit);
                }
                else
                {
                    GIGrid = GetDetailTable(JournalVoucherGrid);
                }
                var Result = await _IJournalVoucher.SaveBankReceipt(model, GIGrid);
                if (Result != null)
                {
                    if (Result.StatusText == "Success" && Result.StatusCode == HttpStatusCode.OK)
                    {
                        ViewBag.isSuccess = true;
                        TempData["200"] = "200";
                        HttpContext.Session.Remove("KeyJournalVoucherGrid");
                        HttpContext.Session.Remove("KeyJournalVoucherGridEdit");
                    }
                    else if (Result.StatusText == "Success" && Result.StatusCode == HttpStatusCode.Accepted)
                    {
                        ViewBag.isSuccess = true;
                        TempData["202"] = "202";
                        HttpContext.Session.Remove("KeyJournalVoucherGrid");
                        HttpContext.Session.Remove("KeyJournalVoucherGridEdit");
                    }
                    else if (Result.StatusText == "Error" && Result.StatusCode == HttpStatusCode.InternalServerError)
                    {
                        ViewBag.isSuccess = false;
                        TempData["500"] = "500";
                        _logger.LogError($"\n \n ********** LogError ********** \n {JsonConvert.SerializeObject(Result)}\n \n");
                        return View("Error", Result);
                    }
                }
                return RedirectToAction(nameof(JournalVoucher));
            }
            catch (Exception ex)
            {
                // Log and return the error
                LogException<JournalVoucherController>.WriteException(_logger, ex);
                var ResponseResult = new ResponseResult
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    StatusText = "Error",
                    Result = ex
                };
                return View("Error", ResponseResult);
            }
        }
        private static DataTable GetDetailTable(IList<JournalVoucherModel> DetailList)
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
            var JSON = await _IJournalVoucher.GetLedgerBalance(OpeningYearCode, AccountCode, VoucherDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillLedgerName(string VoucherType, string Type)
        {
            var JSON = await _IJournalVoucher.FillLedgerName(VoucherType, Type);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillSubVoucherName(string VoucherType)
        {
            var JSON = await _IJournalVoucher.FillSubVoucherName(VoucherType);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillBankType(int AccountCode)
        {
            var JSON = await _IJournalVoucher.FillBankType(AccountCode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillIntrument(string VoucherType)
        {
            var JSON = await _IJournalVoucher.FillIntrument(VoucherType);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillModeofAdjust(string VoucherType)
        {
            var JSON = await _IJournalVoucher.FillModeofAdjust(VoucherType);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillCostCenterName()
        {
            var JSON = await _IJournalVoucher.FillCostCenterName();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillEntryID(int YearCode, string VoucherDate)
        {
            var JSON = await _IJournalVoucher.FillEntryID(YearCode, VoucherDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillCurrency()
        {
            var JSON = await _IJournalVoucher.FillCurrency();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillSONO(string accountcode, string VoucherDate)
        {
            var JSON = await _IJournalVoucher.FillSONO(accountcode, VoucherDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetSODetail(int SONO, string accountcode, string VoucherDate)
        {
            var JSON = await _IJournalVoucher.GetSODetail(SONO, accountcode, VoucherDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetSODate(int SONO, string accountcode, string VoucherDate, string SOYearCode)
        {
            var JSON = await _IJournalVoucher.GetSODate(SONO, accountcode, VoucherDate, SOYearCode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> CheckAmountBeforeSave(string VoucherDate, int YearCode, int AgainstVoucherYearCode, int AgainstVoucherEntryId, string AgainstVoucherNo, int AccountCode)
        {
            var JSON = await _IJournalVoucher.CheckAmountBeforeSave(VoucherDate, YearCode, AgainstVoucherYearCode, AgainstVoucherEntryId, AgainstVoucherNo, AccountCode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public IActionResult AddJournalVoucherDetail(JournalVoucherModel model)
        {
            try
            {
                if (model.Mode == "U" || model.Mode == "V")
                {
                    string modelJson = HttpContext.Session.GetString("KeyJournalVoucherGridEdit");
                    List<JournalVoucherModel> JournalVoucherGrid = new List<JournalVoucherModel>();
                    if (!string.IsNullOrEmpty(modelJson))
                    {
                        JournalVoucherGrid = JsonConvert.DeserializeObject<List<JournalVoucherModel>>(modelJson);
                    }

                    var MainModel = new JournalVoucherModel();
                    var JournalVchGrid = new List<JournalVoucherModel>();
                    var JournalGrid = new List<JournalVoucherModel>();
                    var ssGrid = new List<JournalVoucherModel>();

                    var count = 0;
                    if (model != null)
                    {
                        if (JournalVoucherGrid == null)
                        {
                            model.SrNO = 1;
                            JournalGrid.Add(model);
                        }
                        else
                        {
                            if (model.BankType.ToLower() == "bank")
                            {
                                if (JournalVoucherGrid.Any(x => (x.LedgerName == model.LedgerName) || x.BankType == "Bank"))
                                {
                                    return StatusCode(210, "Duplicate");
                                }
                                else
                                {
                                    model.SrNO = JournalVoucherGrid.Count + 1;
                                    JournalGrid = JournalVoucherGrid.Where(x => x != null).ToList();
                                    ssGrid.AddRange(JournalGrid);
                                    JournalGrid.Add(model);

                                }
                            }
                            else if (model.ModeOfAdjustment.ToLower() == "new ref")
                            {
                                if (JournalVoucherGrid.Any(x => (x.LedgerName == model.LedgerName) && (x.ModeOfAdjustment == model.ModeOfAdjustment)))
                                {
                                    return StatusCode(207, "Duplicate");
                                }
                                else
                                {
                                    model.SrNO = JournalVoucherGrid.Count + 1;
                                    JournalGrid = JournalVoucherGrid.Where(x => x != null).ToList();
                                    ssGrid.AddRange(JournalGrid);
                                    JournalGrid.Add(model);

                                }
                            }
                            else if (model.ModeOfAdjustment.ToLower() == "advance")
                            {
                                if (JournalVoucherGrid.Any(x => (x.LedgerName == model.LedgerName) && (x.ModeOfAdjustment == model.ModeOfAdjustment)))
                                {
                                    return StatusCode(208, "Duplicate");
                                }
                                else
                                {
                                    model.SrNO = JournalVoucherGrid.Count + 1;
                                    JournalGrid = JournalVoucherGrid.Where(x => x != null).ToList();
                                    ssGrid.AddRange(JournalGrid);
                                    JournalGrid.Add(model);

                                }
                            }
                            else if (model.ModeOfAdjustment.ToLower() == "against ref")
                            {
                                if (JournalVoucherGrid.Any(x => (x.LedgerName == model.LedgerName) && x.AgainstVoucherNo == model.AgainstVoucherNo))
                                {
                                    return StatusCode(209, "Duplicate");
                                }
                                else
                                {
                                    model.SrNO = JournalVoucherGrid.Count + 1;
                                    JournalGrid = JournalVoucherGrid.Where(x => x != null).ToList();
                                    ssGrid.AddRange(JournalGrid);
                                    JournalGrid.Add(model);

                                }
                            }

                        }

                        MainModel.JournalVoucherList = JournalGrid.OrderBy(x => x.SrNO).ToList();

                        string serializedGrid = JsonConvert.SerializeObject(MainModel.JournalVoucherList);
                        HttpContext.Session.SetString("KeyJournalVoucherGridEdit", serializedGrid);
                    }
                    else
                    {
                        ModelState.TryAddModelError("Error", "Schedule List Cannot Be Empty...!");
                    }
                    return PartialView("_JournalVoucherGrid", MainModel);
                }
                else
                {
                    string modelJson = HttpContext.Session.GetString("KeyJournalVoucherGrid");
                    List<JournalVoucherModel> JournalVoucherGrid = new List<JournalVoucherModel>();
                    if (!string.IsNullOrEmpty(modelJson))
                    {
                        JournalVoucherGrid = JsonConvert.DeserializeObject<List<JournalVoucherModel>>(modelJson);
                    }

                    var MainModel = new JournalVoucherModel();
                    var JournalVchGrid = new List<JournalVoucherModel>();
                    var JournalGrid = new List<JournalVoucherModel>();
                    var ssGrid = new List<JournalVoucherModel>();

                    if (model != null)
                    {
                        if (JournalVoucherGrid == null)
                        {
                            model.SrNO = 1;
                            JournalGrid.Add(model);
                        }
                        else
                        {
                            if (model.BankType.ToLower() == "bank")
                            {
                                if (JournalVoucherGrid.Any(x => (x.LedgerName == model.LedgerName) || x.BankType == "Bank"))
                                {
                                    return StatusCode(210, "Duplicate");
                                }
                                else
                                {
                                    model.SrNO = JournalVoucherGrid.Count + 1;
                                    JournalGrid = JournalVoucherGrid.Where(x => x != null).ToList();
                                    ssGrid.AddRange(JournalGrid);
                                    JournalGrid.Add(model);

                                }
                            }
                            else if (model.ModeOfAdjustment.ToLower() == "new ref")
                            {
                                if (JournalVoucherGrid.Any(x => (x.LedgerName == model.LedgerName) && (x.ModeOfAdjustment == model.ModeOfAdjustment)))
                                {
                                    return StatusCode(207, "Duplicate");
                                }
                                else
                                {
                                    model.SrNO = JournalVoucherGrid.Count + 1;
                                    JournalGrid = JournalVoucherGrid.Where(x => x != null).ToList();
                                    ssGrid.AddRange(JournalGrid);
                                    JournalGrid.Add(model);

                                }
                            }
                            else if (model.ModeOfAdjustment.ToLower() == "advance")
                            {
                                if (JournalVoucherGrid.Any(x => (x.LedgerName == model.LedgerName) && (x.ModeOfAdjustment == model.ModeOfAdjustment)))
                                {
                                    return StatusCode(208, "Duplicate");
                                }
                                else
                                {
                                    model.SrNO = JournalVoucherGrid.Count + 1;
                                    JournalGrid = JournalVoucherGrid.Where(x => x != null).ToList();
                                    ssGrid.AddRange(JournalGrid);
                                    JournalGrid.Add(model);

                                }
                            }
                            else if (model.ModeOfAdjustment.ToLower() == "against ref")
                            {
                                if (JournalVoucherGrid.Any(x => (x.LedgerName == model.LedgerName) && x.AgainstVoucherNo == model.AgainstVoucherNo))
                                {
                                    return StatusCode(209, "Duplicate");
                                }
                                else
                                {
                                    model.SrNO = JournalVoucherGrid.Count + 1;
                                    JournalGrid = JournalVoucherGrid.Where(x => x != null).ToList();
                                    ssGrid.AddRange(JournalGrid);
                                    JournalGrid.Add(model);

                                }
                            }
                        }

                        MainModel.JournalVoucherList = JournalGrid.OrderBy(x => x.SrNO).ToList();

                        string serializedGrid = JsonConvert.SerializeObject(MainModel.JournalVoucherList);
                        HttpContext.Session.SetString("KeyJournalVoucherGrid", serializedGrid);
                    }
                    else
                    {
                        ModelState.TryAddModelError("Error", "Schedule List Cannot Be Empty...!");
                    }
                    return PartialView("_JournalVoucherGrid", MainModel);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IActionResult AddJournalVoucherAdjustDetail(List<JournalVoucherModel> model, string Mode)
        {
            try
            {
                var MainModel = new JournalVoucherModel();
                var JournalVoucherDetail = new List<JournalVoucherModel>();

                if (Mode != "U" && Mode != "V")
                {
                    string modelJson = HttpContext.Session.GetString("KeyJournalVoucherGrid");
                    List<JournalVoucherModel> JournalVoucherItemDetail = new List<JournalVoucherModel>();
                    if (!string.IsNullOrEmpty(modelJson))
                    {
                        JournalVoucherItemDetail = JsonConvert.DeserializeObject<List<JournalVoucherModel>>(modelJson);
                    }

                    if (JournalVoucherItemDetail != null)
                    {
                        JournalVoucherDetail = JournalVoucherItemDetail;
                    }

                    foreach (var item in model)
                    {
                        if (item != null)
                        {
                            var existingItem = JournalVoucherDetail.FirstOrDefault(x => x.LedgerName == item.LedgerName && x.AgainstVoucherNo == item.AgainstVoucherNo && x.ModeOfAdjustment == item.ModeOfAdjustment);
                            if (existingItem != null)
                            {
                                JournalVoucherDetail.Remove(existingItem);
                            }

                            // Assign sequence number correctly
                            item.SrNO = JournalVoucherDetail.Count + 1;

                            // Swap Type values
                            item.Type = item.Type.ToLower() == "dr" ? "CR" : "DR";

                            // Add new item to list
                            JournalVoucherDetail.Add(item);
                        }
                    }
                    for (int i = 0; i < JournalVoucherDetail.Count; i++)
                    {
                        JournalVoucherDetail[i].SrNO = i + 1; // Ensure proper sequence numbers
                    }
                    // Update the main model and cache
                    MainModel.JournalVoucherList = JournalVoucherDetail.OrderBy(x => x.SrNO).ToList();
                    string serializedGrid = JsonConvert.SerializeObject(MainModel.JournalVoucherList);
                    HttpContext.Session.SetString("KeyJournalVoucherGrid", serializedGrid);
                }
                else
                {
                    string modelJson = HttpContext.Session.GetString("KeyJournalVoucherGridEdit");
                    List<JournalVoucherModel> JournalVoucherItemDetail = new List<JournalVoucherModel>();
                    if (!string.IsNullOrEmpty(modelJson))
                    {
                        JournalVoucherItemDetail = JsonConvert.DeserializeObject<List<JournalVoucherModel>>(modelJson);
                    }

                    // If cache exists, use it; otherwise, initialize a new list
                    if (JournalVoucherItemDetail != null)
                    {
                        JournalVoucherDetail = JournalVoucherItemDetail;
                    }

                    foreach (var item in model)
                    {
                        if (item != null)
                        {
                            var existingItem = JournalVoucherDetail.FirstOrDefault(x => x.LedgerName == item.LedgerName && x.AgainstVoucherNo == item.AgainstVoucherNo && x.ModeOfAdjustment == item.ModeOfAdjustment);
                            if (existingItem != null)
                            {
                                JournalVoucherDetail.Remove(existingItem);
                            }

                            // Assign sequence number correctly
                            // item.SrNO = ProductionEntryDetail.Count + 1;

                            // Swap Type values
                            item.Type = item.Type.ToLower() == "dr" ? "CR" : "DR";

                            // Add new item to list
                            JournalVoucherDetail.Add(item);
                        }
                    }
                    for (int i = 0; i < JournalVoucherDetail.Count; i++)
                    {
                        JournalVoucherDetail[i].SrNO = i + 1; // Ensure proper sequence numbers
                    }

                    // Update the main model and cache
                    MainModel.JournalVoucherList = JournalVoucherDetail.OrderBy(x => x.SrNO).ToList();
                    string serializedGrid = JsonConvert.SerializeObject(MainModel.JournalVoucherList);
                    HttpContext.Session.SetString("KeyJournalVoucherGridEdit", serializedGrid);
                }
                return PartialView("_JournalVoucherGrid", MainModel);
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
                var MainModel = new JournalVoucherModel();
                string modelJson = HttpContext.Session.GetString("KeyJournalVoucherGrid");
                List<JournalVoucherModel> GridDetail = new List<JournalVoucherModel>();
                if (!string.IsNullOrEmpty(modelJson))
                {
                    GridDetail = JsonConvert.DeserializeObject<List<JournalVoucherModel>>(modelJson);
                }
                var SAGrid = GridDetail.Where(x => x.SrNO == SrNO);
                string JsonString = JsonConvert.SerializeObject(SAGrid);
                return Json(JsonString);
            }
            else
            {
                var MainModel = new JournalVoucherModel();
                string modelJson = HttpContext.Session.GetString("KeyJournalVoucherGridEdit");
                List<JournalVoucherModel> GridDetail = new List<JournalVoucherModel>();
                if (!string.IsNullOrEmpty(modelJson))
                {
                    GridDetail = JsonConvert.DeserializeObject<List<JournalVoucherModel>>(modelJson);
                }
                var SAGrid = GridDetail.Where(x => x.SrNO == SrNO);
                string JsonString = JsonConvert.SerializeObject(SAGrid);
                return Json(JsonString);
            }
        }
        public IActionResult DeleteItemRow(int SeqNo, string Mode, string PopUpData)
        {
            if (PopUpData == "PopUpData")
            {
                var MainModel = new JournalVoucherModel();
                string modelJson = HttpContext.Session.GetString("KeyJournalVoucherGridPopUpData");
                List<JournalVoucherModel> JournalVoucherGrid = new List<JournalVoucherModel>();
                if (!string.IsNullOrEmpty(modelJson))
                {
                    JournalVoucherGrid = JsonConvert.DeserializeObject<List<JournalVoucherModel>>(modelJson);
                }

                int Indx = Convert.ToInt32(SeqNo) - 1;

                if (JournalVoucherGrid != null && JournalVoucherGrid.Count > 0)
                {
                    JournalVoucherGrid.RemoveAt(Convert.ToInt32(Indx));
                    Indx = 0;

                    foreach (var item in JournalVoucherGrid)
                    {
                        Indx++;
                        // item.SequenceNo = Indx;
                    }
                    MainModel.JournalVoucherList = JournalVoucherGrid.OrderBy(x => x.SrNO).ToList();
                    string serializedGrid = JsonConvert.SerializeObject(MainModel.JournalVoucherList);
                    HttpContext.Session.SetString("KeyJournalVoucherGridPopUpData", serializedGrid);
                }
                return PartialView("_DisplayPopupForPendingVouchers", MainModel);
            }
            else
            {
                var MainModel = new JournalVoucherModel();
                if (Mode != "U" && Mode != "V")
                {
                    string modelJson = HttpContext.Session.GetString("KeyJournalVoucherGrid");
                    List<JournalVoucherModel> JournalVoucherGrid = new List<JournalVoucherModel>();
                    if (!string.IsNullOrEmpty(modelJson))
                    {
                        JournalVoucherGrid = JsonConvert.DeserializeObject<List<JournalVoucherModel>>(modelJson);
                    }

                    int Indx = Convert.ToInt32(SeqNo) - 1;

                    if (JournalVoucherGrid != null && JournalVoucherGrid.Count > 0)
                    {
                        JournalVoucherGrid.RemoveAt(Convert.ToInt32(Indx));
                        Indx = 0;

                        foreach (var item in JournalVoucherGrid)
                        {
                            Indx++;
                            item.SrNO = Indx;
                        }
                        MainModel.JournalVoucherList = JournalVoucherGrid.OrderBy(x => x.SrNO).ToList();

                        string serializedGrid = JsonConvert.SerializeObject(MainModel.JournalVoucherList);
                        HttpContext.Session.SetString("KeyJournalVoucherGrid", serializedGrid);
                    }
                }
                else
                {
                    string modelJson = HttpContext.Session.GetString("KeyJournalVoucherGridEdit");
                    List<JournalVoucherModel> JournalVoucherGrid = new List<JournalVoucherModel>();
                    if (!string.IsNullOrEmpty(modelJson))
                    {
                        JournalVoucherGrid = JsonConvert.DeserializeObject<List<JournalVoucherModel>>(modelJson);
                    }

                    int Indx = Convert.ToInt32(SeqNo) - 1;

                    if (JournalVoucherGrid != null && JournalVoucherGrid.Count > 0)
                    {
                        JournalVoucherGrid.RemoveAt(Convert.ToInt32(Indx));
                        Indx = 0;

                        foreach (var item in JournalVoucherGrid)
                        {
                            Indx++;
                            item.SrNO = Indx;
                        }
                        MainModel.JournalVoucherList = JournalVoucherGrid.OrderBy(x => x.SrNO).ToList();

                        string serializedGrid = JsonConvert.SerializeObject(MainModel.JournalVoucherList);
                        HttpContext.Session.SetString("KeyJournalVoucherGridEdit", serializedGrid);
                    }
                }
                return PartialView("_JournalVoucherGrid", MainModel);
            }
        }
        public async Task<IActionResult> JournalVoucherDashBoard(string FromDate, string ToDate)
        {
            try
            {
                HttpContext.Session.Remove("KeyJournalVoucherGrid");
                HttpContext.Session.Remove("KeyJournalVoucherGridEdit");
                var model = new JournalVoucherModel();
                FromDate = HttpContext.Session.GetString("FromDate");
                ToDate = HttpContext.Session.GetString("ToDate");
                var Result = await _IJournalVoucher.GetDashBoardData(FromDate, ToDate).ConfigureAwait(true);
                if (Result != null)
                {
                    DataSet ds = Result.Result;
                    if (ds != null && ds.Tables.Count > 0)
                    {
                        var dt = ds.Tables[0];
                        model.JournalVoucherList = CommonFunc.DataTableToList<JournalVoucherModel>(dt, "JournalVoucherDashBoard");
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
            HttpContext.Session.Remove("KeyJournalVoucherGrid");
            HttpContext.Session.Remove("KeyJournalVoucherGridEdit");
            var model = new JournalVoucherModel();
            model = await _IJournalVoucher.GetDashBoardDetailData(FromDate, ToDate, LedgerName, VoucherNo, AgainstVoucherRefNo, AgainstVoucherNo);
            return PartialView("_JournalVoucherDashBoardDetailGrid", model);
        }
        public async Task<IActionResult> GetDashBoardSummaryData(string FromDate, string ToDate, string LedgerName, string VoucherNo, string AgainstVoucherRefNo, string AgainstVoucherNo)
        {
            HttpContext.Session.Remove("KeyJournalVoucherGrid");
            HttpContext.Session.Remove("KeyJournalVoucherGridEdit");
            var model = new JournalVoucherModel();
            model = await _IJournalVoucher.GetDashBoardSummaryData(FromDate, ToDate, LedgerName, VoucherNo, AgainstVoucherRefNo, AgainstVoucherNo);
            return PartialView("_JournalVoucherDashBoardGrid", model);
        }
        public async Task<IActionResult> PopUpForPendingVouchers(PopUpDataTableAgainstRef DataTable)
        {
            HttpContext.Session.Remove("KeyJournalVoucherGridPopUpData");

            var model = await _IJournalVoucher.PopUpForPendingVouchers(DataTable);
            string serializedGrid = JsonConvert.SerializeObject(model.JournalVoucherList);
            HttpContext.Session.SetString("KeyJournalVoucherGridPopUpData", serializedGrid);
            return PartialView("_DisplayPopupForPendingVouchers", model);
        }
        public async Task<IActionResult> DeleteByID(int ID, int YearCode, int ActualEntryBy, string EntryByMachine, string ActualEntryDate, string VoucherType)
        {
            var Result = await _IJournalVoucher.DeleteByID(ID, YearCode, ActualEntryBy, EntryByMachine, ActualEntryDate, VoucherType);

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
            return RedirectToAction("JournalVoucherDashBoard");
        }
        public async Task<JsonResult> FillLedgerInDashboard(string FromDate, string ToDate, string VoucherType)
        {
            var JSON = await _IJournalVoucher.FillLedgerInDashboard(FromDate, ToDate, VoucherType);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillBankInDashboard(string FromDate, string ToDate, string VoucherType)
        {
            var JSON = await _IJournalVoucher.FillBankInDashboard(FromDate, ToDate, VoucherType);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillVoucherNoInDashboard(string FromDate, string ToDate, string VoucherType)
        {
            var JSON = await _IJournalVoucher.FillVoucherNoInDashboard(FromDate, ToDate, VoucherType);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillAgainstVoucherRefNoInDashboard(string FromDate, string ToDate, string VoucherType)
        {
            var JSON = await _IJournalVoucher.FillAgainstVoucherRefNoInDashboard(FromDate, ToDate, VoucherType);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillAgainstVoucherNoInDashboard(string FromDate, string ToDate, string VoucherType)
        {
            var JSON = await _IJournalVoucher.FillAgainstVoucherNoInDashboard(FromDate, ToDate, VoucherType);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
    }
}
