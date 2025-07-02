using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System.Data;
using System.Net;
using System.Runtime.Caching;
using static eTactWeb.Data.Common.CommonFunc;
using static eTactWeb.DOM.Models.Common;

namespace eTactwebAccounts.Controllers
{
    public class BankPaymentController : Controller
    {
        private readonly IDataLogic _IDataLogic;
        public IBankPayment _IBankPayment { get; }
        private readonly ILogger<BankPaymentController> _logger;
        private readonly IConfiguration iconfiguration;
        public IWebHostEnvironment _IWebHostEnvironment { get; }
        public BankPaymentController(ILogger<BankPaymentController> logger, IDataLogic iDataLogic, IBankPayment iBankPayment, EncryptDecrypt encryptDecrypt, IWebHostEnvironment iWebHostEnvironment, IConfiguration iconfiguration)
        {
            _logger = logger;
            _IDataLogic = iDataLogic;
            _IBankPayment = iBankPayment;
            _IWebHostEnvironment = iWebHostEnvironment;
            this.iconfiguration = iconfiguration;
        }
        [Route("{controller}/Index")]
        [HttpGet]
        public async Task<ActionResult> BankPayment(int ID, string Mode, int YearCode, string VoucherNo, string FromDate = "", string ToDate = "", string LedgerName = "", string AgainstVoucherRefNo = "", string AgainstVoucherNo = "", string Searchbox = "", string DashboardType = "")
        {
            HttpContext.Session.Remove("KeyBankPaymentGrid");
            HttpContext.Session.Remove("KeyBankPaymentGridEdit");
            TempData.Clear();
            var MainModel = new BankPaymentModel();
            MainModel.CC = HttpContext.Session.GetString("Branch");
            MainModel.YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
            MainModel.ActualEntryby = Convert.ToInt32(HttpContext.Session.GetString("UID"));
            MainModel.ActualEntryBy = HttpContext.Session.GetString("EmpName");
            MainModel.ActualEntryDate = DateTime.Now.ToString("dd/MM/yy");
            MainModel.UID = Convert.ToInt32(HttpContext.Session.GetString("UID"));
            MainModel.FromDate = HttpContext.Session.GetString("FromDate");
            MainModel.ToDate = HttpContext.Session.GetString("ToDate");

            if (!string.IsNullOrEmpty(Mode) && ID > 0 && Mode == "U" || Mode == "V")
            {
                MainModel = await _IBankPayment.GetViewByID(ID, YearCode, VoucherNo).ConfigureAwait(false);
                MainModel.Mode = Mode; // Set Mode to Update
                MainModel.ID = ID;
                MainModel.VoucherNo = VoucherNo;

                string serializedGrid = JsonConvert.SerializeObject(MainModel.BankPaymentGrid);
                HttpContext.Session.SetString("KeyBankPaymentGridEdit", serializedGrid);
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
        public async Task<IActionResult> BankPayment(BankPaymentModel model)
        {
            try
            {
                var GIGrid = new DataTable();
                string modelJson = HttpContext.Session.GetString("KeyBankPaymentGrid");
                List<BankPaymentModel> BankPaymentGrid = new List<BankPaymentModel>();
                if (!string.IsNullOrEmpty(modelJson))
                {
                    BankPaymentGrid = JsonConvert.DeserializeObject<List<BankPaymentModel>>(modelJson);
                }
                string modelEditJson = HttpContext.Session.GetString("KeyBankPaymentGridEdit");
                List<BankPaymentModel> BankPaymentGridEdit = new List<BankPaymentModel>();
                if (!string.IsNullOrEmpty(modelEditJson))
                {
                    BankPaymentGridEdit = JsonConvert.DeserializeObject<List<BankPaymentModel>>(modelEditJson);
                }

                model.CreatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                if (model.Mode == "U")
                {
                    model.UpdatedOn = DateTime.Now;
                    GIGrid = GetDetailTable(BankPaymentGridEdit);
                }
                else
                {
                    GIGrid = GetDetailTable(BankPaymentGrid);
                }
                var Result = await _IBankPayment.SaveBankPayment(model, GIGrid);
                if (Result != null)
                {
                    if (Result.StatusText == "Success" && Result.StatusCode == HttpStatusCode.OK)
                    {
                        ViewBag.isSuccess = true;
                        TempData["200"] = "200";
                        HttpContext.Session.Remove("KeyBankPaymentGrid");
                        HttpContext.Session.Remove("KeyBankPaymentGridEdit");
                    }
                    else if (Result.StatusText == "Success" && Result.StatusCode == HttpStatusCode.Accepted)
                    {
                        ViewBag.isSuccess = true;
                        TempData["202"] = "202";
                        HttpContext.Session.Remove("KeyBankPaymentGrid");
                        HttpContext.Session.Remove("KeyBankPaymentGridEdit");
                    }
                    else if (Result.StatusText == "Error" && Result.StatusCode == HttpStatusCode.InternalServerError)
                    {
                        ViewBag.isSuccess = false;
                        TempData["500"] = "500";
                        _logger.LogError($"\n \n ********** LogError ********** \n {JsonConvert.SerializeObject(Result)}\n \n");
                        return View("Error", Result);
                    }
                }
                return RedirectToAction(nameof(BankPayment));
            }
            catch (Exception ex)
            {
                LogException<BankPaymentController>.WriteException(_logger, ex);
                var ResponseResult = new ResponseResult
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    StatusText = "Error",
                    Result = ex
                };
                return View("Error", ResponseResult);
            }
        }
        private static DataTable GetDetailTable(IList<BankPaymentModel> DetailList)
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
                Item.SoDate = DateTime.Now.ToString("dd/MMM/yyyy") ,
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
                Item.SubVoucher ?? string.Empty
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
            var JSON = await _IBankPayment.GetLedgerBalance(OpeningYearCode, AccountCode, VoucherDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillLedgerName(string VoucherType, string ShowAll)
        {
            var JSON = await _IBankPayment.FillLedgerName(VoucherType, ShowAll);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillSubVoucherName(string VoucherType)
        {
            var JSON = await _IBankPayment.FillSubVoucherName(VoucherType);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillBankType(int AccountCode)
        {
            var JSON = await _IBankPayment.FillBankType(AccountCode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillIntrument(string VoucherType)
        {
            var JSON = await _IBankPayment.FillIntrument(VoucherType);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillModeofAdjust(string VoucherType)
        {
            var JSON = await _IBankPayment.FillModeofAdjust(VoucherType);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillCostCenterName()
        {
            var JSON = await _IBankPayment.FillCostCenterName();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillEntryID(int YearCode, string VoucherDate)
        {
            var JSON = await _IBankPayment.FillEntryID(YearCode, VoucherDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillCurrency()
        {
            var JSON = await _IBankPayment.FillCurrency();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillPONO(string accountcode, string VoucherDate)
        {
            var JSON = await _IBankPayment.FillPONO(accountcode, VoucherDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetPOYearCode(string PONO, string accountcode, string VoucherDate)
        {
            var JSON = await _IBankPayment.GetPOYearCode(PONO, accountcode, VoucherDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetPODate(string PONO, string accountcode, string VoucherDate, string POYearCode)
        {
            var JSON = await _IBankPayment.GetPODate(PONO, accountcode, VoucherDate, POYearCode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> CheckAmountBeforeSave(string VoucherDate, int YearCode, int AgainstVoucherYearCode, int AgainstVoucherEntryId, string AgainstVoucherNo, int AccountCode)
        {
            var JSON = await _IBankPayment.CheckAmountBeforeSave(VoucherDate, YearCode, AgainstVoucherYearCode, AgainstVoucherEntryId, AgainstVoucherNo, AccountCode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public IActionResult AddBankPaymentDetail(BankPaymentModel model)
        {
            try
            {
                if (model.Mode == "U" || model.Mode == "V")
                {
                    string modelJson = HttpContext.Session.GetString("KeyBankPaymentGridEdit");
                    List<BankPaymentModel> BankPaymentGrid = new List<BankPaymentModel>();
                    if (!string.IsNullOrEmpty(modelJson))
                    {
                        BankPaymentGrid = JsonConvert.DeserializeObject<List<BankPaymentModel>>(modelJson);
                    }

                    var MainModel = new BankPaymentModel();
                    var WorkOrderPGrid = new List<BankPaymentModel>();
                    var OrderGrid = new List<BankPaymentModel>();
                    var ssGrid = new List<BankPaymentModel>();

                    var count = 0;
                    if (model != null)
                    {
                        if (BankPaymentGrid == null)
                        {
                            model.SrNO = 1;
                            OrderGrid.Add(model);
                        }
                        else
                        {
                            if (model.BankType.ToLower() == "bank")
                            {
                                if (BankPaymentGrid.Any(x => (x.LedgerName == model.LedgerName) || x.BankType == "Bank"))
                                {
                                    return StatusCode(210, "Duplicate");
                                }
                                else
                                {
                                    model.SrNO = BankPaymentGrid.Count + 1;
                                    OrderGrid = BankPaymentGrid.Where(x => x != null).ToList();
                                    ssGrid.AddRange(OrderGrid);
                                    OrderGrid.Add(model);

                                }
                            }
                            else if (model.ModeOfAdjustment.ToLower() == "new ref")
                            {
                                if (BankPaymentGrid.Any(x => (x.LedgerName == model.LedgerName) && (x.ModeOfAdjustment == model.ModeOfAdjustment)))
                                {
                                    return StatusCode(207, "Duplicate");
                                }
                                else
                                {
                                    model.SrNO = BankPaymentGrid.Count + 1;
                                    OrderGrid = BankPaymentGrid.Where(x => x != null).ToList();
                                    ssGrid.AddRange(OrderGrid);
                                    OrderGrid.Add(model);

                                }
                            }
                            else if (model.ModeOfAdjustment.ToLower() == "advance")
                            {
                                if (BankPaymentGrid.Any(x => (x.LedgerName == model.LedgerName) && (x.ModeOfAdjustment == model.ModeOfAdjustment)))
                                {
                                    return StatusCode(208, "Duplicate");
                                }
                                else
                                {
                                    model.SrNO = BankPaymentGrid.Count + 1;
                                    OrderGrid = BankPaymentGrid.Where(x => x != null).ToList();
                                    ssGrid.AddRange(OrderGrid);
                                    OrderGrid.Add(model);

                                }
                            }
                            else if (model.ModeOfAdjustment.ToLower() == "against ref")
                            {
                                if (BankPaymentGrid.Any(x => (x.LedgerName == model.LedgerName) && x.AgainstVoucherNo == model.AgainstVoucherNo))
                                {
                                    return StatusCode(209, "Duplicate");
                                }
                                else
                                {
                                    model.SrNO = BankPaymentGrid.Count + 1;
                                    OrderGrid = BankPaymentGrid.Where(x => x != null).ToList();
                                    ssGrid.AddRange(OrderGrid);
                                    OrderGrid.Add(model);

                                }
                            }
                        }

                        MainModel.BankPaymentGrid = OrderGrid.OrderBy(x => x.SrNO).ToList();

                        string serializedGrid = JsonConvert.SerializeObject(MainModel.BankPaymentGrid);
                        HttpContext.Session.SetString("KeyBankPaymentGridEdit", serializedGrid);
                    }
                    else
                    {
                        ModelState.TryAddModelError("Error", "Schedule List Cannot Be Empty...!");
                    }
                    return PartialView("_BankPaymentGrid", MainModel);
                }
                else
                {
                    string modelJson = HttpContext.Session.GetString("KeyBankPaymentGrid");
                    List<BankPaymentModel> BankPaymentGrid = new List<BankPaymentModel>();
                    if (!string.IsNullOrEmpty(modelJson))
                    {
                        BankPaymentGrid = JsonConvert.DeserializeObject<List<BankPaymentModel>>(modelJson);
                    }

                    var MainModel = new BankPaymentModel();
                    var WorkOrderPGrid = new List<BankPaymentModel>();
                    var OrderGrid = new List<BankPaymentModel>();
                    var ssGrid = new List<BankPaymentModel>();

                    if (model != null)
                    {
                        if (BankPaymentGrid == null)
                        {
                            model.SrNO = 1;
                            OrderGrid.Add(model);
                        }
                        else
                        {
                            if (model.BankType.ToLower() == "bank")
                            {
                                if (BankPaymentGrid.Any(x => (x.LedgerName == model.LedgerName) || x.BankType == "Bank"))
                                {
                                    return StatusCode(210, "Duplicate");
                                }
                                else
                                {
                                    model.SrNO = BankPaymentGrid.Count + 1;
                                    OrderGrid = BankPaymentGrid.Where(x => x != null).ToList();
                                    ssGrid.AddRange(OrderGrid);
                                    OrderGrid.Add(model);

                                }
                            }
                            else if (model.ModeOfAdjustment.ToLower() == "new ref")
                            {
                                if (BankPaymentGrid.Any(x => (x.LedgerName == model.LedgerName) && (x.ModeOfAdjustment == model.ModeOfAdjustment)))
                                {
                                    return StatusCode(207, "Duplicate");
                                }
                                else
                                {
                                    model.SrNO = BankPaymentGrid.Count + 1;
                                    OrderGrid = BankPaymentGrid.Where(x => x != null).ToList();
                                    ssGrid.AddRange(OrderGrid);
                                    OrderGrid.Add(model);

                                }
                            }
                            else if (model.ModeOfAdjustment.ToLower() == "advance")
                            {
                                if (BankPaymentGrid.Any(x => (x.LedgerName == model.LedgerName) && (x.ModeOfAdjustment == model.ModeOfAdjustment)))
                                {
                                    return StatusCode(208, "Duplicate");
                                }
                                else
                                {
                                    model.SrNO = BankPaymentGrid.Count + 1;
                                    OrderGrid = BankPaymentGrid.Where(x => x != null).ToList();
                                    ssGrid.AddRange(OrderGrid);
                                    OrderGrid.Add(model);

                                }
                            }
                            else if (model.ModeOfAdjustment.ToLower() == "against ref")
                            {
                                if (BankPaymentGrid.Any(x => (x.LedgerName == model.LedgerName) && x.AgainstVoucherNo == model.AgainstVoucherNo))
                                {
                                    return StatusCode(209, "Duplicate");
                                }
                                else
                                {
                                    model.SrNO = BankPaymentGrid.Count + 1;
                                    OrderGrid = BankPaymentGrid.Where(x => x != null).ToList();
                                    ssGrid.AddRange(OrderGrid);
                                    OrderGrid.Add(model);

                                }
                            }
                        }

                        MainModel.BankPaymentGrid = OrderGrid.OrderBy(x => x.SrNO).ToList();

                        string serializedGrid = JsonConvert.SerializeObject(MainModel.BankPaymentGrid);
                        HttpContext.Session.SetString("KeyBankPaymentGrid", serializedGrid);
                    }
                    else
                    {
                        ModelState.TryAddModelError("Error", "Schedule List Cannot Be Empty...!");
                    }
                    return PartialView("_BankPaymentGrid", MainModel);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IActionResult AddBankPaymentAdjustDetail(List<BankPaymentModel> model, string Mode)
        {
            try
            {
                var MainModel = new BankPaymentModel();
                var ProductionEntryDetail = new List<BankPaymentModel>();
                if (Mode != "U" && Mode != "V")
                {
                    string modelJson = HttpContext.Session.GetString("KeyBankPaymentGrid");
                    List<BankPaymentModel> ProductionEntryItemDetail = new List<BankPaymentModel>();
                    if (!string.IsNullOrEmpty(modelJson))
                    {
                        ProductionEntryItemDetail = JsonConvert.DeserializeObject<List<BankPaymentModel>>(modelJson);
                    }
                    // If cache exists, use it; otherwise, initialize a new list
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
                    MainModel.BankPaymentGrid = ProductionEntryDetail.OrderBy(x => x.SrNO).ToList();

                    string serializedGrid = JsonConvert.SerializeObject(MainModel.BankPaymentGrid);
                    HttpContext.Session.SetString("KeyBankPaymentGrid", serializedGrid);
                }
                else
                {
                    string modelJson = HttpContext.Session.GetString("KeyBankPaymentGridEdit");
                    List<BankPaymentModel> ProductionEntryItemDetail = new List<BankPaymentModel>();
                    if (!string.IsNullOrEmpty(modelJson))
                    {
                        ProductionEntryItemDetail = JsonConvert.DeserializeObject<List<BankPaymentModel>>(modelJson);
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
                    // Update the main model and cache
                    MainModel.BankPaymentGrid = ProductionEntryDetail.OrderBy(x => x.SrNO).ToList();

                    string serializedGrid = JsonConvert.SerializeObject(MainModel.BankPaymentGrid);
                    HttpContext.Session.SetString("KeyBankPaymentGridEdit", serializedGrid);
                }
                return PartialView("_BankPaymentGrid", MainModel);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public JsonResult EditItemRows(int SrNO, string Mode)
        {
            if (Mode != "U" && Mode != "V")
            {
                var MainModel = new BankPaymentModel();
                string modelJson = HttpContext.Session.GetString("KeyBankPaymentGrid");
                List<BankPaymentModel> GridDetail = new List<BankPaymentModel>();
                if (!string.IsNullOrEmpty(modelJson))
                {
                    GridDetail = JsonConvert.DeserializeObject<List<BankPaymentModel>>(modelJson);
                }

                IEnumerable<BankPaymentModel> SSGrid = GridDetail;
                if (GridDetail != null)
                {
                    SSGrid = GridDetail.Where(x => x.SrNO == SrNO);
                }
                string JsonString = JsonConvert.SerializeObject(SSGrid);
                return Json(JsonString);
            }
            else
            {
                var MainModel = new BankPaymentModel();
                string modelJson = HttpContext.Session.GetString("KeyBankPaymentGridEdit");
                List<BankPaymentModel> GridDetail = new List<BankPaymentModel>();
                if (!string.IsNullOrEmpty(modelJson))
                {
                    GridDetail = JsonConvert.DeserializeObject<List<BankPaymentModel>>(modelJson);
                }

                IEnumerable<BankPaymentModel> SSGrid = GridDetail;
                if (GridDetail != null)
                {
                    SSGrid = GridDetail.Where(x => x.SrNO == SrNO);
                }
                string JsonString = JsonConvert.SerializeObject(SSGrid);
                return Json(JsonString);
            } 
        }
        public IActionResult DeleteItemRow(int SeqNo, string Mode, string PopUpData)
        {
            if (PopUpData == "PopUpData")
            {
                var MainModel = new BankPaymentModel();
                string modelJson = HttpContext.Session.GetString("KeyBankPaymentGridPopUpData");
                List<BankPaymentModel> BankPaymentGrid = new List<BankPaymentModel>();
                if (!string.IsNullOrEmpty(modelJson))
                {
                    BankPaymentGrid = JsonConvert.DeserializeObject<List<BankPaymentModel>>(modelJson);
                }
               
                int Indx = Convert.ToInt32(SeqNo) - 1;

                if (BankPaymentGrid != null && BankPaymentGrid.Count > 0)
                {
                    BankPaymentGrid.RemoveAt(Convert.ToInt32(Indx));
                    Indx = 0;

                    foreach (var item in BankPaymentGrid)
                    {
                        Indx++;
                        item.SrNO = Indx;
                    }
                    MainModel.BankPaymentGrid = BankPaymentGrid.OrderBy(x => x.SrNO).ToList();


                    string serializedGrid = JsonConvert.SerializeObject(MainModel.BankPaymentGrid);
                    HttpContext.Session.SetString("KeyBankPaymentGridPopUpData", serializedGrid);
                }
                return PartialView("_DisplayPopupForPendingVouchers", MainModel);
            }
            else
            {
                var MainModel = new BankPaymentModel();
                if (Mode != "U" && Mode != "V")
                {
                    string modelJson = HttpContext.Session.GetString("KeyBankPaymentGrid");
                    List<BankPaymentModel> BankPaymentGrid = new List<BankPaymentModel>();
                    if (!string.IsNullOrEmpty(modelJson))
                    {
                        BankPaymentGrid = JsonConvert.DeserializeObject<List<BankPaymentModel>>(modelJson);
                    }
                    
                    int Indx = Convert.ToInt32(SeqNo) - 1;

                    if (BankPaymentGrid != null && BankPaymentGrid.Count > 0)
                    {
                        BankPaymentGrid.RemoveAt(Convert.ToInt32(Indx));
                        Indx = 0;

                        foreach (var item in BankPaymentGrid)
                        {
                            Indx++;
                            item.SrNO = Indx;
                        }
                        MainModel.BankPaymentGrid = BankPaymentGrid.OrderBy(x => x.SrNO).ToList();


                        string serializedGrid = JsonConvert.SerializeObject(MainModel.BankPaymentGrid);
                        HttpContext.Session.SetString("KeyBankPaymentGrid", serializedGrid);
                    }
                }
                else
                {
                    string modelJson = HttpContext.Session.GetString("KeyBankPaymentGridEdit");
                    List<BankPaymentModel> BankPaymentGrid = new List<BankPaymentModel>();
                    if (!string.IsNullOrEmpty(modelJson))
                    {
                        BankPaymentGrid = JsonConvert.DeserializeObject<List<BankPaymentModel>>(modelJson);
                    }
                    
                    int Indx = Convert.ToInt32(SeqNo) - 1;

                    if (BankPaymentGrid != null && BankPaymentGrid.Count > 0)
                    {
                        BankPaymentGrid.RemoveAt(Convert.ToInt32(Indx));
                        Indx = 0;

                        foreach (var item in BankPaymentGrid)
                        {
                            Indx++;
                            item.SrNO = Indx;
                        }
                        MainModel.BankPaymentGrid = BankPaymentGrid.OrderBy(x => x.SrNO).ToList();

                        string serializedGrid = JsonConvert.SerializeObject(MainModel.BankPaymentGrid);
                        HttpContext.Session.SetString("KeyBankPaymentGridEdit", serializedGrid);
                    }
                }   
                return PartialView("_BankPaymentGrid", MainModel);
            }
        }
        public async Task<IActionResult> BankPaymentDashBoard(string FromDate = "", string ToDate = "", string Flag = "True", string LedgerName = "", string Bank = "", string VoucherNo = "", string AgainstVoucherRefNo = "", string AgainstVoucherNo = "", string Searchbox = "", string DashboardType = "")
        {
            try
            {
                HttpContext.Session.Remove("KeyBankPaymentGrid");
                HttpContext.Session.Remove("KeyBankPaymentGridEdit");
                var model = new BankPaymentModel();
                var Result = await _IBankPayment.GetDashBoardData(FromDate, ToDate).ConfigureAwait(true);
                if (Result != null)
                {
                    DataSet ds = Result.Result;
                    if (ds != null && ds.Tables.Count > 0)
                    {
                        var dt = ds.Tables[0];
                        model.BankPaymentGrid = CommonFunc.DataTableToList<BankPaymentModel>(dt, "BankPaymentDashBoard");

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
        public async Task<IActionResult> GetDashBoardDetailData(string FromDate, string ToDate, string LedgerName,string Bank, string VoucherNo, string AgainstVoucherNo, string PoNo, string AgainstBillno)
        {
            var model = new BankPaymentModel();
            model = await _IBankPayment.GetDashBoardDetailData(FromDate, ToDate,LedgerName,Bank,VoucherNo, AgainstVoucherNo, PoNo,AgainstBillno);
            return PartialView("_BankPaymentDashBoardDetailGrid", model);
        }
        public async Task<IActionResult> GetDashBoardSummaryData(string FromDate, string ToDate, string LedgerName, string Bank, string VoucherNo, string AgainstVoucherNo, string PONo, string AgainstBillno)
        {
            var model = new BankPaymentModel();
            model = await _IBankPayment.GetDashBoardSummaryData(FromDate, ToDate, LedgerName, Bank,VoucherNo, AgainstVoucherNo, PONo,AgainstBillno);
            return PartialView("_BankPaymentDashBoardGrid", model);
        }
        public async Task<IActionResult> PopUpForPendingVouchers(PopUpDataTableAgainstRef DataTable)
        {
            HttpContext.Session.Remove("KeyBankPaymentGridPopUpData");
            var model = await _IBankPayment.PopUpForPendingVouchers(DataTable);
            string serializedGrid = JsonConvert.SerializeObject(model.BankPaymentGrid);
            HttpContext.Session.SetString("KeyBankPaymentGridPopUpData", serializedGrid);
            return PartialView("_DisplayPopupForPendingVouchers", model);
        }
        public async Task<IActionResult> DeleteByID(int ID, int YearCode, int ActualEntryBy, string EntryByMachine, string ActualEntryDate, string FromDate = "", string ToDate = "", string LedgerName = "", string Bank = "", string VoucherNo = "", string AgainstVoucherRefNo = "", string AgainstVoucherNo = "", string Searchbox = "", string DashboardType = "")
        {
            var Result = await _IBankPayment.DeleteByID(ID, YearCode, ActualEntryBy, EntryByMachine, ActualEntryDate);

            if (Result.StatusText == "Success" || Result.StatusCode == HttpStatusCode.Gone)
            {
                ViewBag.isSuccess = true;
                TempData["410"] = "410";
                //TempData["Message"] = "Data deleted successfully.";
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

            return RedirectToAction("BankPaymentDashBoard", new {EntryByMachine = EntryByMachine,ActualEntryDate = ActualEntryDate, Flag = "False", FromDate = FromDate, ToDate = ToDate,LedgerName = LedgerName, Bank=Bank,VoucherNo = VoucherNo, AgainstVoucherRefNo = AgainstVoucherRefNo, AgainstVoucherNo = AgainstVoucherNo, Searchbox = Searchbox, DashboardType = DashboardType});

        }
        public async Task<JsonResult> FillLedgerInDashboard(string FromDate, string ToDate, string VoucherType)
        {
            var JSON = await _IBankPayment.FillLedgerInDashboard(FromDate,ToDate,VoucherType);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillBankInDashboard(string FromDate, string ToDate, string VoucherType)
        {
            var JSON = await _IBankPayment.FillBankInDashboard(FromDate, ToDate, VoucherType);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillVoucherNoInDashboard(string FromDate, string ToDate, string VoucherType)
        {
            var JSON = await _IBankPayment.FillVoucherNoInDashboard(FromDate, ToDate, VoucherType);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillAgainstVoucherRefNoInDashboard(string FromDate, string ToDate, string VoucherType)
        {
            var JSON = await _IBankPayment.FillAgainstVoucherRefNoInDashboard(FromDate, ToDate, VoucherType);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillAgainstVoucherNoInDashboard(string FromDate, string ToDate, string VoucherType)
        {
            var JSON = await _IBankPayment.FillAgainstVoucherNoInDashboard(FromDate, ToDate, VoucherType);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
    }
}
