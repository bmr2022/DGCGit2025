using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using FastReport.Web;
using FastReport;
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
    public class CashPaymentController : Controller
    {
        private readonly IDataLogic _IDataLogic;
        public ICashPayment _ICashPayment { get; }
        private readonly ILogger<CashPaymentController> _logger;
        private readonly IConfiguration iconfiguration;
        public IWebHostEnvironment _IWebHostEnvironment { get; }
        public CashPaymentController(ILogger<CashPaymentController> logger, IDataLogic iDataLogic, ICashPayment iCashPayment, EncryptDecrypt encryptDecrypt, IWebHostEnvironment iWebHostEnvironment, IConfiguration iconfiguration)
        {
            _logger = logger;
            _IDataLogic = iDataLogic;
            _ICashPayment = iCashPayment;
            _IWebHostEnvironment = iWebHostEnvironment;
            this.iconfiguration = iconfiguration;
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
        public async Task<JsonResult> GetFormRights()
        {
            var userID = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
            var JSON = await _ICashPayment.GetFormRights(userID);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        [Route("{controller}/Index")]
        [HttpGet]
        public async Task<ActionResult> CashPayment(int ID, string Mode, int YearCode, string VoucherNo, string FromDate = "", string ToDate = "", string LedgerName = "", string AgainstVoucherRefNo = "", string AgainstVoucherNo = "", string Searchbox = "", string DashboardType = "")
        {
            HttpContext.Session.Remove("KeyCashPaymentGrid");
            HttpContext.Session.Remove("KeyCashPaymentGridEdit");
            TempData.Clear();
            var MainModel = new CashPaymentModel();
            MainModel.CC = HttpContext.Session.GetString("Branch");
            MainModel.YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
            MainModel.ActualEntryby = Convert.ToInt32(HttpContext.Session.GetString("UID"));
            MainModel.ActualEntryBy = HttpContext.Session.GetString("EmpName");
            MainModel.ActualEntryDate = DateTime.Now.ToString("dd/MM/yy");
            MainModel.UID = Convert.ToInt32(HttpContext.Session.GetString("UID"));
            MainModel.FromDate = HttpContext.Session.GetString("FromDate");
            MainModel.ToDate = HttpContext.Session.GetString("ToDate");

            if (!string.IsNullOrEmpty(Mode) && ID > 0 && Mode == "U"|| Mode == "V")
            {
                MainModel = await _ICashPayment.GetViewByID(ID, YearCode, VoucherNo).ConfigureAwait(false);
                MainModel.Mode = Mode; // Set Mode to Update
                MainModel.ID = ID;
                MainModel.VoucherNo = VoucherNo;
                string serializedGrid = JsonConvert.SerializeObject(MainModel.CashPaymentGrid);
                HttpContext.Session.SetString("KeyCashPaymentGridEdit", serializedGrid);
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
        public async Task<IActionResult> CashPayment(CashPaymentModel model)
        {
            try
            {
                var GIGrid = new DataTable();
                string cashModel = HttpContext.Session.GetString("KeyCashPaymentGrid");
                List<CashPaymentModel> CashPaymentGrid = new List<CashPaymentModel>();
                if (!string.IsNullOrEmpty(cashModel))
                {
                    CashPaymentGrid = JsonConvert.DeserializeObject<List<CashPaymentModel>>(cashModel);
                }
                string cashEditModel = HttpContext.Session.GetString("KeyCashPaymentGridEdit");
                List<CashPaymentModel> CashPaymentGridEdit = new List<CashPaymentModel>();
                if (!string.IsNullOrEmpty(cashEditModel))
                {
                    CashPaymentGridEdit = JsonConvert.DeserializeObject<List<CashPaymentModel>>(cashEditModel);
                }

                model.CreatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                if (model.Mode == "U")
                {
                    model.UpdatedOn = DateTime.Now;
                    GIGrid = GetDetailTable(CashPaymentGridEdit);
                }
                else
                {
                    GIGrid = GetDetailTable(CashPaymentGrid);
                }
                var Result = await _ICashPayment.SaveCashPayment(model, GIGrid);
                if (Result != null)
                {
                    if (Result.StatusText == "Success" && Result.StatusCode == HttpStatusCode.OK)
                    {
                        ViewBag.isSuccess = true;
                        TempData["200"] = "200";
                        HttpContext.Session.Remove("KeyCashPaymentGrid");   
                    }
                    else if (Result.StatusText == "Success" && Result.StatusCode == HttpStatusCode.Accepted)
                    {
                        ViewBag.isSuccess = true;
                        TempData["202"] = "202";
                        HttpContext.Session.Remove("KeyCashPaymentGridEdit");
                    }
                    else if (Result.StatusText == "Error" && Result.StatusCode == HttpStatusCode.InternalServerError)
                    {
                        ViewBag.isSuccess = false;
                        TempData["500"] = "500";
                        _logger.LogError($"\n \n ********** LogError ********** \n {JsonConvert.SerializeObject(Result)}\n \n");
                        return View("Error", Result);
                    }
                }

                return RedirectToAction(nameof(CashPaymentDashBoard));

            }
            catch (Exception ex)
            {
                // Log and return the error
                LogException<CashPaymentController>.WriteException(_logger, ex);
                var ResponseResult = new ResponseResult
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    StatusText = "Error",
                    Result = ex
                };
                return View("Error", ResponseResult);
            }
        }
        private static DataTable GetDetailTable(IList<CashPaymentModel> DetailList)
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
                Item.SoDate = DateTime.Now.ToString("dd/MMM/yyyy"),
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
            var JSON = await _ICashPayment.GetLedgerBalance(OpeningYearCode, AccountCode, VoucherDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillLedgerName(string VoucherType, string ShowAll)
        {
            var JSON = await _ICashPayment.FillLedgerName(VoucherType, ShowAll);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillSubVoucherName(string VoucherType)
        {
            var JSON = await _ICashPayment.FillSubVoucherName(VoucherType);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillBankType(int AccountCode)
        {
            var JSON = await _ICashPayment.FillBankType(AccountCode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillIntrument(string VoucherType)
        {
            var JSON = await _ICashPayment.FillIntrument(VoucherType);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillModeofAdjust(string VoucherType)
        {
            var JSON = await _ICashPayment.FillModeofAdjust(VoucherType);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillCostCenterName()
        {
            var JSON = await _ICashPayment.FillCostCenterName();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillEntryID(int YearCode, string VoucherDate)
        {
            var JSON = await _ICashPayment.FillEntryID(YearCode, VoucherDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillCurrency()
        {
            var JSON = await _ICashPayment.FillCurrency();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillPONO(string accountcode, string VoucherDate)
        {
            var JSON = await _ICashPayment.FillPONO(accountcode, VoucherDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetPOYearCode(string PONO, string accountcode, string VoucherDate)
        {
            var JSON = await _ICashPayment.GetPOYearCode(PONO, accountcode, VoucherDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetPODate(string PONO, string accountcode, string VoucherDate, string POYearCode)
        {
            var JSON = await _ICashPayment.GetPODate(PONO, accountcode, VoucherDate, POYearCode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> CheckAmountBeforeSave(string VoucherDate, int YearCode, int AgainstVoucherYearCode, int AgainstVoucherEntryId, string AgainstVoucherNo, int AccountCode)
        {
            var JSON = await _ICashPayment.CheckAmountBeforeSave(VoucherDate, YearCode, AgainstVoucherYearCode, AgainstVoucherEntryId, AgainstVoucherNo, AccountCode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public IActionResult AddCashPaymentDetail(CashPaymentModel model)
        {
            try
            {
                if (model.Mode == "U" || model.Mode == "V")
                {
                    string cashEditModel = HttpContext.Session.GetString("KeyCashPaymentGridEdit");
                    List<CashPaymentModel> CashPaymentGrid = new List<CashPaymentModel>();
                    if (!string.IsNullOrEmpty(cashEditModel))
                    {
                        CashPaymentGrid = JsonConvert.DeserializeObject<List<CashPaymentModel>>(cashEditModel);
                    }

                    var MainModel = new CashPaymentModel();
                    var WorkOrderPGrid = new List<CashPaymentModel>();
                    var OrderGrid = new List<CashPaymentModel>();
                    var ssGrid = new List<CashPaymentModel>();

                    var count = 0;
                    if (model != null)
                    {
                        if (CashPaymentGrid == null)
                        {
                            model.SrNO = 1;
                            OrderGrid.Add(model);   
                        }
                        else
                        {
                            if (model.BankType.ToLower() == "bank")
                            {
                                if (CashPaymentGrid.Any(x => (x.LedgerName == model.LedgerName) || x.BankType == "Bank"))
                                {
                                    return StatusCode(210, "Duplicate");
                                }
                                else
                                {
                                    model.SrNO = CashPaymentGrid.Count + 1;
                                    OrderGrid = CashPaymentGrid.Where(x => x != null).ToList();
                                    ssGrid.AddRange(OrderGrid);
                                    OrderGrid.Add(model);

                                }
                            }
                            else if (model.ModeOfAdjustment.ToLower() == "new ref")
                            {
                                if (CashPaymentGrid.Any(x => (x.LedgerName == model.LedgerName) && (x.ModeOfAdjustment == model.ModeOfAdjustment)))
                                {
                                    return StatusCode(207, "Duplicate");
                                }
                                else
                                {
                                    model.SrNO = CashPaymentGrid.Count + 1;
                                    OrderGrid = CashPaymentGrid.Where(x => x != null).ToList();
                                    ssGrid.AddRange(OrderGrid);
                                    OrderGrid.Add(model);

                                }
                            }
                            else if (model.ModeOfAdjustment.ToLower() == "advance")
                            {
                                if (CashPaymentGrid.Any(x => (x.LedgerName == model.LedgerName) && (x.ModeOfAdjustment == model.ModeOfAdjustment)))
                                {
                                    return StatusCode(208, "Duplicate");
                                }
                                else
                                {
                                    model.SrNO = CashPaymentGrid.Count + 1;
                                    OrderGrid = CashPaymentGrid.Where(x => x != null).ToList();
                                    ssGrid.AddRange(OrderGrid);
                                    OrderGrid.Add(model);

                                }
                            }
                            else if (model.ModeOfAdjustment.ToLower() == "against ref")
                            {
                                if (CashPaymentGrid.Any(x => (x.LedgerName == model.LedgerName) && x.AgainstVoucherNo == model.AgainstVoucherNo))
                                {
                                    return StatusCode(209, "Duplicate");
                                }
                                else
                                {
                                    model.SrNO = CashPaymentGrid.Count + 1;
                                    OrderGrid = CashPaymentGrid.Where(x => x != null).ToList();
                                    ssGrid.AddRange(OrderGrid);
                                    OrderGrid.Add(model);

                                }
                            }
                        }

                        MainModel.CashPaymentGrid = OrderGrid;

                        string serializedGrid = JsonConvert.SerializeObject(MainModel.CashPaymentGrid);
                        HttpContext.Session.SetString("KeyCashPaymentGridEdit", serializedGrid);
                    }
                    else
                    {
                        ModelState.TryAddModelError("Error", "Schedule List Cannot Be Empty...!");
                    }
                    return PartialView("_CashPaymentGrid", MainModel);
                }
                else
                {
                    string modelJson = HttpContext.Session.GetString("KeyCashPaymentGrid");
                    List<CashPaymentModel> CashPaymentGrid = new List<CashPaymentModel>();
                    if (!string.IsNullOrEmpty(modelJson))
                    {
                        CashPaymentGrid = JsonConvert.DeserializeObject<List<CashPaymentModel>>(modelJson);
                    }

                    var MainModel = new CashPaymentModel();
                    var WorkOrderPGrid = new List<CashPaymentModel>();
                    var OrderGrid = new List<CashPaymentModel>();
                    var ssGrid = new List<CashPaymentModel>();

                    if (model != null)
                    {
                        if (CashPaymentGrid == null)
                        {
                            model.SrNO = 1;
                            OrderGrid.Add(model);
                        }
                        else
                        {
                            if (model.BankType.ToLower() == "bank")
                            {
                                if (CashPaymentGrid.Any(x => (x.LedgerName == model.LedgerName) && x.BankType == "Bank"))
                                {
                                    return StatusCode(210, "Duplicate");
                                }
                                else
                                {
                                    model.SrNO = CashPaymentGrid.Count + 1;
                                    OrderGrid = CashPaymentGrid.Where(x => x != null).ToList();
                                    ssGrid.AddRange(OrderGrid);
                                    OrderGrid.Add(model);

                                }
                            }
                            else if (model.ModeOfAdjustment.ToLower() == "new ref")
                            {
                                if (CashPaymentGrid.Any(x => (x.LedgerName == model.LedgerName) && (x.ModeOfAdjustment == model.ModeOfAdjustment)))
                                {
                                    return StatusCode(207, "Duplicate");
                                }
                                else
                                {
                                    model.SrNO = CashPaymentGrid.Count + 1;
                                    OrderGrid = CashPaymentGrid.Where(x => x != null).ToList();
                                    ssGrid.AddRange(OrderGrid);
                                    OrderGrid.Add(model);

                                }
                            }
                            else if (model.ModeOfAdjustment.ToLower() == "advance")
                            {
                                if (CashPaymentGrid.Any(x => (x.LedgerName == model.LedgerName) && (x.ModeOfAdjustment == model.ModeOfAdjustment)))
                                {
                                    return StatusCode(208, "Duplicate");
                                }
                                else
                                {
                                    model.SrNO = CashPaymentGrid.Count + 1;
                                    OrderGrid = CashPaymentGrid.Where(x => x != null).ToList();
                                    ssGrid.AddRange(OrderGrid);
                                    OrderGrid.Add(model);

                                }
                            }
                            else if (model.ModeOfAdjustment.ToLower() == "against ref")
                            {
                                if (CashPaymentGrid.Any(x => (x.LedgerName == model.LedgerName) && x.AgainstVoucherNo == model.AgainstVoucherNo))
                                {
                                    return StatusCode(209, "Duplicate");
                                }
                                else
                                {
                                    model.SrNO = CashPaymentGrid.Count + 1;
                                    OrderGrid = CashPaymentGrid.Where(x => x != null).ToList();
                                    ssGrid.AddRange(OrderGrid);
                                    OrderGrid.Add(model);

                                }
                            }
                        }

                        MainModel.CashPaymentGrid = OrderGrid;

                        string serializedGrid = JsonConvert.SerializeObject(MainModel.CashPaymentGrid);
                        HttpContext.Session.SetString("KeyCashPaymentGrid", serializedGrid);
                    }
                    else
                    {
                        ModelState.TryAddModelError("Error", "Schedule List Cannot Be Empty...!");
                    }
                    return PartialView("_CashPaymentGrid", MainModel);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IActionResult AddCashPaymentAdjustDetail(List<CashPaymentModel> model, string Mode)
        {
            try
            {
                var MainModel = new CashPaymentModel();
                var ProductionEntryDetail = new List<CashPaymentModel>();
                if (Mode != "U" && Mode != "V")
                {
                    string modelJson = HttpContext.Session.GetString("KeyCashPaymentGrid");
                    List<CashPaymentModel> ProductionEntryItemDetail = new List<CashPaymentModel>();
                    if (!string.IsNullOrEmpty(modelJson))
                    {
                        ProductionEntryItemDetail = JsonConvert.DeserializeObject<List<CashPaymentModel>>(modelJson);
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
                            var existingItem = ProductionEntryDetail.FirstOrDefault(x => x.LedgerName == item.LedgerName && x.InVoiceNo == item.InVoiceNo);
                            if (existingItem != null)
                            {
                                ProductionEntryDetail.Remove(existingItem);
                            }

                            // Assign sequence number correctly
                            //item.SeqNo = ProductionEntryDetail.Count + 1;

                            // Swap Type values
                            item.Type = item.Type.ToLower() == "dr" ? "CR" : "DR";

                            // Add new item to list
                            ProductionEntryDetail.Add(item);
                        }
                    }
                    for (int i = 0; i < ProductionEntryDetail.Count; i++)
                    {
                        ProductionEntryDetail[i].SrNO = i + 1; // Ensure proper sequence numbers
                    }
                    // Update the main model and cache
                    MainModel.CashPaymentGrid = ProductionEntryDetail;

                    string serializedGrid = JsonConvert.SerializeObject(MainModel.CashPaymentGrid);
                    HttpContext.Session.SetString("KeyCashPaymentGrid", serializedGrid);

                }
                else
                {

                    string modelJson = HttpContext.Session.GetString("KeyCashPaymentGridEdit");
                    List<CashPaymentModel> ProductionEntryItemDetail = new List<CashPaymentModel>();
                    if (!string.IsNullOrEmpty(modelJson))
                    {
                        ProductionEntryItemDetail = JsonConvert.DeserializeObject<List<CashPaymentModel>>(modelJson);
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
                            var existingItem = ProductionEntryDetail.FirstOrDefault(x => x.LedgerName == item.LedgerName && x.InVoiceNo == item.InVoiceNo);
                            if (existingItem != null)
                            {
                                ProductionEntryDetail.Remove(existingItem);
                            }

                            // Assign sequence number correctly
                            //item.SeqNo = ProductionEntryDetail.Count + 1;

                            // Swap Type values
                            item.Type = item.Type.ToLower() == "dr" ? "CR" : "DR";

                            // Add new item to list
                            ProductionEntryDetail.Add(item);
                        }
                    }
                    for (int i = 0; i < ProductionEntryDetail.Count; i++)
                    {
                        ProductionEntryDetail[i].SrNO = i + 1; // Ensure proper sequence numbers
                    }
                    // Update the main model and cache
                    MainModel.CashPaymentGrid = ProductionEntryDetail;

                    string serializedGrid = JsonConvert.SerializeObject(MainModel.CashPaymentGrid);
                    HttpContext.Session.SetString("KeyCashPaymentGridEdit", serializedGrid);
                }

                return PartialView("_CashPaymentGrid", MainModel);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<JsonResult> EditItemRows(int SrNO, string Mode)
        {
            List<CashPaymentModel> GridDetail = new List<CashPaymentModel>();
            if (Mode != "U" && Mode != "V")
            {
                var MainModel = new CashPaymentModel();
                string modelJson = HttpContext.Session.GetString("KeyCashPaymentGrid");
                
                if (!string.IsNullOrEmpty(modelJson))
                {
                    GridDetail = JsonConvert.DeserializeObject<List<CashPaymentModel>>(modelJson);
                }
            }
            else
            {
                var MainModel = new CashPaymentModel();
                string modelJson = HttpContext.Session.GetString("KeyCashPaymentGridEdit");
                if (!string.IsNullOrEmpty(modelJson))
                {
                    GridDetail = JsonConvert.DeserializeObject<List<CashPaymentModel>>(modelJson);
                }
            }
            var SAGrid = GridDetail.Where(x => x.SrNO == SrNO);
            string JsonString = JsonConvert.SerializeObject(SAGrid);
            return Json(JsonString);
        }
        public IActionResult DeleteItemRow(int SeqNo, string Mode, string PopUpData)
        {
            if (PopUpData == "PopUpData")
            {
                var MainModel = new CashPaymentModel();
                string modelJson = HttpContext.Session.GetString("KeyCashPaymentGridPopUpData");
                List<CashPaymentModel> CashPaymentGrid = new List<CashPaymentModel>();
                if (!string.IsNullOrEmpty(modelJson))
                {
                    CashPaymentGrid = JsonConvert.DeserializeObject<List<CashPaymentModel>>(modelJson);
                }

                int Indx = Convert.ToInt32(SeqNo) - 1;

                if (CashPaymentGrid != null && CashPaymentGrid.Count > 0)
                {
                    CashPaymentGrid.RemoveAt(Convert.ToInt32(Indx));
                    Indx = 0;

                    foreach (var item in CashPaymentGrid)
                    {
                        Indx++;
                        // item.SequenceNo = Indx;
                    }
                    MainModel.CashPaymentGrid = CashPaymentGrid;

                    string serializedGrid = JsonConvert.SerializeObject(MainModel.CashPaymentGrid);
                    HttpContext.Session.SetString("KeyCashPaymentGridPopUpData", serializedGrid);
                }
                return PartialView("_DisplayPopupForPendingVouchers", MainModel);
            }
            else
            {
                var MainModel = new CashPaymentModel();
                if (Mode != "U" && Mode != "V")
                {
                    string modelJson = HttpContext.Session.GetString("KeyCashPaymentGrid");
                    List<CashPaymentModel> CashPaymentGrid = new List<CashPaymentModel>();
                    if (!string.IsNullOrEmpty(modelJson))
                    {
                        CashPaymentGrid = JsonConvert.DeserializeObject<List<CashPaymentModel>>(modelJson);
                    }

                    int Indx = Convert.ToInt32(SeqNo) - 1;

                    if (CashPaymentGrid != null && CashPaymentGrid.Count > 0)
                    {
                        CashPaymentGrid.RemoveAt(Convert.ToInt32(Indx));
                        Indx = 0;

                        foreach (var item in CashPaymentGrid)
                        {
                            Indx++;
                            // item.SequenceNo = Indx;
                        }
                        MainModel.CashPaymentGrid = CashPaymentGrid;

                        string serializedGrid = JsonConvert.SerializeObject(MainModel.CashPaymentGrid);
                        HttpContext.Session.SetString("KeyCashPaymentGrid", serializedGrid);
                    }
                }
                else
                {
                    string modelJson = HttpContext.Session.GetString("KeyCashPaymentGridEdit");
                    List<CashPaymentModel> CashPaymentGrid = new List<CashPaymentModel>();
                    if (!string.IsNullOrEmpty(modelJson))
                    {
                        CashPaymentGrid = JsonConvert.DeserializeObject<List<CashPaymentModel>>(modelJson);
                    }

                    int Indx = Convert.ToInt32(SeqNo) - 1;

                    if (CashPaymentGrid != null && CashPaymentGrid.Count > 0)
                    {
                        CashPaymentGrid.RemoveAt(Convert.ToInt32(Indx));
                        Indx = 0;

                        foreach (var item in CashPaymentGrid)
                        {
                            Indx++;
                            // item.SequenceNo = Indx;
                        }
                        MainModel.CashPaymentGrid = CashPaymentGrid;
                        string serializedGrid = JsonConvert.SerializeObject(MainModel.CashPaymentGrid);
                        HttpContext.Session.SetString("KeyCashPaymentGridEdit", serializedGrid);

                    }
                }
                return PartialView("_CashPaymentGrid", MainModel);
            }
        }
        public async Task<IActionResult> CashPaymentDashBoard(string FromDate, string ToDate)
        {
            try
            {
                HttpContext.Session.Remove("KeyCashPaymentGrid");
                HttpContext.Session.Remove("KeyCashPaymentGridEdit");
                var model = new CashPaymentModel();
                FromDate = HttpContext.Session.GetString("FromDate");
                ToDate = HttpContext.Session.GetString("ToDate");
                var Result = await _ICashPayment.GetDashBoardData(FromDate, ToDate).ConfigureAwait(true);
                if (Result != null)
                {
                    DataSet ds = Result.Result;
                    if (ds != null && ds.Tables.Count > 0)
                    {
                        var dt = ds.Tables[0];
                        model.CashPaymentGrid = CommonFunc.DataTableToList<CashPaymentModel>(dt, "CashPaymentDashBoard");
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
            HttpContext.Session.Remove("KeyCashPaymentGrid");
            HttpContext.Session.Remove("KeyCashPaymentGridEdit");
            var model = new CashPaymentModel();
            model = await _ICashPayment.GetDashBoardDetailData(FromDate, ToDate, LedgerName,Bank, VoucherNo, AgainstVoucherNo, PoNo, AgainstBillno);
            return PartialView("_CashPaymentDashBoardDetailGrid", model);
        }
        public async Task<IActionResult> GetDashBoardSummaryData(string FromDate, string ToDate, string LedgerName,string Bank, string VoucherNo, string AgainstVoucherNo, string PoNo, string AgainstBillno)
        {
            HttpContext.Session.Remove("KeyCashPaymentGrid");
            HttpContext.Session.Remove("KeyCashPaymentGridEdit");
            var model = new CashPaymentModel();
            model = await _ICashPayment.GetDashBoardSummaryData(FromDate, ToDate, LedgerName,Bank, VoucherNo, AgainstVoucherNo, PoNo, AgainstBillno);
            return PartialView("_CashPaymentDashBoardGrid", model);
        }
        public async Task<IActionResult> PopUpForPendingVouchers(PopUpDataTableAgainstRef DataTable)
        {
            HttpContext.Session.Remove("KeyCashPaymentGridPopUpData");
            var model = await _ICashPayment.PopUpForPendingVouchers(DataTable);
            string serializedGrid = JsonConvert.SerializeObject(model.CashPaymentGrid);
            HttpContext.Session.SetString("KeyCashPaymentGridPopUpData", serializedGrid);
            return PartialView("_DisplayPopupForPendingVouchers", model);
        }
        public async Task<IActionResult> DeleteByID(int ID, int YearCode, int ActualEntryBy, string EntryByMachine, string ActualEntryDate, string VoucherType  )
        {
            var Result = await _ICashPayment.DeleteByID(ID, YearCode, ActualEntryBy, EntryByMachine, ActualEntryDate,VoucherType);

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
                var dt = Result.Result as DataTable;

                if (dt != null && dt.Rows.Count > 0)
                {

                    TempData["DeleteMessage"] = dt.Rows[0]["Result"].ToString();

                }
            }

            return RedirectToAction("CashPaymentDashBoard");

        }
        public async Task<JsonResult> FillLedgerInDashboard(string FromDate, string ToDate, string VoucherType)
        {
            var JSON = await _ICashPayment.FillLedgerInDashboard(FromDate, ToDate, VoucherType);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillBankInDashboard(string FromDate, string ToDate, string VoucherType)
        {
            var JSON = await _ICashPayment.FillBankInDashboard(FromDate, ToDate, VoucherType);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillVoucherNoInDashboard(string FromDate, string ToDate, string VoucherType)
        {
            var JSON = await _ICashPayment.FillVoucherNoInDashboard(FromDate, ToDate, VoucherType);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillAgainstVoucherRefNoInDashboard(string FromDate, string ToDate, string VoucherType)
        {
            var JSON = await _ICashPayment.FillAgainstVoucherRefNoInDashboard(FromDate, ToDate, VoucherType);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillAgainstVoucherNoInDashboard(string FromDate, string ToDate, string VoucherType)
        {
            var JSON = await _ICashPayment.FillAgainstVoucherNoInDashboard(FromDate, ToDate, VoucherType);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
    }
}
