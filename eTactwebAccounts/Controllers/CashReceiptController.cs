using DocumentFormat.OpenXml.Wordprocessing;
using eTactWeb.Controllers;
using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using FastReport;
using FastReport.Web;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Data;
using System.Globalization;
using System.Net;
using static eTactWeb.Data.Common.CommonFunc;

namespace eTactwebAccounts.Controllers
{
    public class CashReceiptController : Controller
    {
        private readonly IDataLogic _IDataLogic;
        public ICashReceipt _ICashReceipt { get; }
        private readonly ILogger<CashReceiptController> _logger;
        private readonly IConfiguration iconfiguration;
        public IWebHostEnvironment _IWebHostEnvironment { get; }
        private readonly ConnectionStringService _connectionStringService;
        public EncryptDecrypt EncryptDecrypt { get; }
        public CashReceiptController(ILogger<CashReceiptController> logger, IDataLogic iDataLogic, ICashReceipt ICashReceipt, EncryptDecrypt encryptDecrypt, IWebHostEnvironment iWebHostEnvironment, IConfiguration iconfiguration, ConnectionStringService connectionStringService)
        {
            _logger = logger;
            _IDataLogic = iDataLogic;
            _ICashReceipt = ICashReceipt;
            EncryptDecrypt = encryptDecrypt;
            _IWebHostEnvironment = iWebHostEnvironment;
            this.iconfiguration = iconfiguration;
            _connectionStringService = connectionStringService;
        }
        public async Task<JsonResult> GetFormRights(string formKey)
        {
            var userID = Convert.ToInt32(HttpContext.Session.GetString($"EmpID_{formKey}"));
            var JSON = await _ICashReceipt.GetFormRights(userID);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public IActionResult PrintReport(int EntryId = 0, int YearCode = 0, string VoucherName = "")
        {
            string my_connection_string;
            string contentRootPath = _IWebHostEnvironment.ContentRootPath;
            string webRootPath = Path.Combine(_IWebHostEnvironment.WebRootPath, "reports");
            var webReport = new WebReport();
            webReport.Report.Clear();
            webReport.Report.Dispose();
            webReport.Report = new Report();
            string ReportName = "";
            var ReportNamepara = _IDataLogic.GetReportName();
            if (ReportNamepara != null &&
                ReportNamepara.Result != null &&
                ReportNamepara.Result.Result != null &&
                ReportNamepara.Result.Result.Rows.Count > 0)
            {
                DataRow row = ReportNamepara.Result.Result.Rows[0];
                ReportName = row["CashRecVoucherPrintReportName"]?.ToString() ?? "";
            }
            webReport.Report.Clear();
            webReport.Report.Dispose();
            webReport.Report = new Report();
            if (!string.IsNullOrEmpty(ReportName))
            {
                webReport.Report.Load(webRootPath + "\\" + ReportName + ".frx");
            }

            else
            {
                webReport.Report.Load(webRootPath + "\\VoucherReport.frx");
            }
            // webReport.Report.Load(webRootPath + "\\VoucherReport.frx");
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
        public async Task<ActionResult> CashReceipt(string formKey, int ID, string Mode, int YearCode, string VoucherNo, string FromDate = "", string ToDate = "", string LedgerName = "", string AgainstVoucherRefNo = "", string AgainstVoucherNo = "", string Searchbox = "", string DashboardType = "")
        {
            ViewBag.formKey = formKey;
            var uniqueKey = Guid.NewGuid().ToString();
            ViewBag.uniqueKey = uniqueKey;
            HttpContext.Session.Remove($"KeyCashReceiptGrid_{uniqueKey}");
            HttpContext.Session.Remove($"KeyCashReceiptGridEdit_{uniqueKey}");
            //TempData.Clear();
            int userID = Convert.ToInt32(HttpContext.Session.GetString($"EmpID_{formKey}"));
            var rights = await _ICashReceipt.GetFormRights(userID);
            if (rights?.Result == null || rights.Result.Tables.Count == 0 || rights.Result.Tables[0].Rows.Count == 0)
            {
                return RedirectToAction("Dashboard", "Home", new { formKey = formKey });
            }


            string encID = Request.Query["ID"].ToString();
            string encYC = Request.Query["YearCode"].ToString();

            if (!string.IsNullOrEmpty(encID) || !string.IsNullOrEmpty(encYC))
            {
                int decryptedID = EncryptDecrypt.DecodeID(encID);
                int decryptedYC = EncryptDecrypt.DecodeID(encYC);
                string decryptedMode = EncryptDecrypt.Decrypt(Mode);
                string decryptedVoucherNo = EncryptDecrypt.Decrypt(VoucherNo);
                ID = decryptedID;
                YearCode = decryptedYC;
                Mode = decryptedMode;
                VoucherNo = decryptedVoucherNo;
            }
            var table = rights.Result.Tables[0];
            bool optAll = Convert.ToBoolean(table.Rows[0]["OptAll"]);
            bool optView = Convert.ToBoolean(table.Rows[0]["OptView"]);
            bool optUpdate = Convert.ToBoolean(table.Rows[0]["OptUpdate"]);
            bool optSave = Convert.ToBoolean(table.Rows[0]["OptSave"]);


            if (Mode == "U")
            {
                if (!(optUpdate))
                {
                    return RedirectToAction("Dashboard", "Home", new { formKey = formKey });
                }
            }
            if (Mode == "V")
            {
                if (!(optView))
                {
                    return RedirectToAction("Dashboard", "Home", new { formKey = formKey });
                }
            }
            if (ID <= 0)
            {
                if (!optSave)
                {
                    return RedirectToAction("BankPaymentDashBoard", "BankPayment", new { formKey = formKey });
                }
            }


            var MainModel = new CashReceiptModel();
            MainModel.CC = HttpContext.Session.GetString($"Branch_{formKey}");
            MainModel.YearCode = Convert.ToInt32(HttpContext.Session.GetString($"YearCode_{formKey}"));
            MainModel.ActualEntryby = Convert.ToInt32(HttpContext.Session.GetString($"UID_{formKey}"));
            MainModel.ActualEntryBy = HttpContext.Session.GetString($"EmpName_{formKey}");
            MainModel.ActualEntryDate = DateTime.Now.ToString("dd/MM/yy");
            MainModel.UID = Convert.ToInt32(HttpContext.Session.GetString($"UID_{formKey}"));
            //MainModel.FromDate = HttpContext.Session.GetString($"FromDate_{formKey}");
            //MainModel.ToDate = HttpContext.Session.GetString($"ToDate_{formKey}");

            if (!string.IsNullOrEmpty(Mode) && ID > 0 && (Mode == "U" || Mode == "V"))
            {
                MainModel = await _ICashReceipt.GetViewByID(ID, YearCode, VoucherNo).ConfigureAwait(false);
                MainModel.Mode = Mode; // Set Mode to Update
                MainModel.ID = ID;
                MainModel.YearCode = YearCode;
                MainModel.VoucherNo = VoucherNo;

                string serializedGrid = JsonConvert.SerializeObject(MainModel.CashReceiptGrid);
                HttpContext.Session.SetString($"KeyCashReceiptGridEdit_{uniqueKey}", serializedGrid);
            }

            if (Mode == "U" && MainModel.UpdatedBy == 0)
            {
                MainModel.UpdatedBy = Convert.ToInt32(HttpContext.Session.GetString($"UID_{formKey}"));
                MainModel.UpdatedByEmp = HttpContext.Session.GetString($"EmpName_{formKey}");
                MainModel.UpdatedOn = DateTime.Now;
            }
            MainModel.FromDate = CommonFunc.ParseFormattedDate(HttpContext.Session.GetString($"FromDate_{formKey}"));
            MainModel.ToDate = CommonFunc.ParseFormattedDate(HttpContext.Session.GetString($"ToDate_{formKey}"));


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
        public async Task<IActionResult> CashReceipt(CashReceiptModel model)
        {
            try
            {
                var GIGrid = new DataTable();
                var formKey = model.formKey;
                var uniqueKey = model.uniqueKey;
                string modelJson = HttpContext.Session.GetString($"KeyCashReceiptGrid_{uniqueKey}");
                List<CashReceiptModel> CashReceiptGrid = new List<CashReceiptModel>();
                if (!string.IsNullOrEmpty(modelJson))
                {
                    CashReceiptGrid = JsonConvert.DeserializeObject<List<CashReceiptModel>>(modelJson);
                }
                string modelEditJson = HttpContext.Session.GetString($"KeyCashReceiptGridEdit_{uniqueKey}");
                List<CashReceiptModel> CashReceiptGridEdit = new List<CashReceiptModel>();
                if (!string.IsNullOrEmpty(modelEditJson))
                {
                    CashReceiptGridEdit = JsonConvert.DeserializeObject<List<CashReceiptModel>>(modelEditJson);
                }

                model.ActualEntryby = Convert.ToInt32(HttpContext.Session.GetString($"UID_{formKey}"));
                model.ActualEntryBy = HttpContext.Session.GetString($"UID_{formKey}");
                if (model.Mode == "U")
                {
                    GIGrid = GetDetailTable(CashReceiptGridEdit);
                }
                else
                {
                    GIGrid = GetDetailTable(CashReceiptGrid);
                }
                model.EntryByMachine = HttpContext.Session.GetString($"ClientMachineName_{formKey}");
                model.IPAddress = HttpContext.Session.GetString($"ClientIP_{formKey}");
                var Result = await _ICashReceipt.SaveCashReceipt(model, GIGrid);
                if (Result != null)
                {
                    if (Result.StatusText == "Success" && Result.StatusCode == HttpStatusCode.OK)
                    {
                        ViewBag.isSuccess = true;
                        TempData["200"] = "200";
                        HttpContext.Session.Remove($"KeyCashReceiptGrid_{uniqueKey}");
                        return Json(new
                        {
                            success = true,
                            message = "Cash Receipt Save successfully",
                            redirectUrl = Url.Action(
        "Index",
        "CashReceipt",
        new { formKey = formKey }

    )
                        });
                    }
                    else if (Result.StatusText == "Success" && Result.StatusCode == HttpStatusCode.Accepted)
                    {
                        ViewBag.isSuccess = true;
                        TempData["202"] = "202";
                        HttpContext.Session.Remove($"KeyCashReceiptGridEdit_{uniqueKey}");
                        return Json(new
                        {
                            success = true,
                            message = "Cash Receipt Updated successfully",
                            redirectUrl = Url.Action(
       "Index",
       "CashReceipt",
               new { formKey = formKey }


   )
                        });
                    }
                    else if (Result.StatusText == "Error" && Result.StatusCode == HttpStatusCode.InternalServerError)
                    {
                        ViewBag.isSuccess = false;
                        TempData["500"] = "500";
                        _logger.LogError($"\n \n ********** LogError ********** \n {JsonConvert.SerializeObject(Result)}\n \n");
                        //return View("Error", Result);
                        return Json(new
                        {
                            success = false,
                            message = "An unexpected error occurred."
                        });
                    }
                    else if (!string.IsNullOrEmpty(Result.StatusText))
                    {
                        // If SP returned a message (like adjustment error)
                        TempData["ErrorMessage"] = Result.StatusText;
                        HttpContext.Session.Remove($"KeyCashReceiptGrid_{uniqueKey}");
                        HttpContext.Session.Remove($"KeyCashReceiptGridEdit_{uniqueKey}");
                        //return View(model);
                        return Json(new
                        {
                            success = false,
                            message = Result.StatusText
                        });
                    }
                }

                //return RedirectToAction(nameof(CashReceipt), new {formKey=formKey});
                return Json(new
                {
                    success = false,
                    message = "An unexpected error occurred."
                });

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private static DataTable GetDetailTable(IList<CashReceiptModel> DetailList)
        {
            try
            {
                var GIGrid = new DataTable();
                GIGrid.Columns.Add("AccEntryId", typeof(int));
                GIGrid.Columns.Add("AccYearCode", typeof(int));
                GIGrid.Columns.Add("EntryDate", typeof(string));
                GIGrid.Columns.Add("DocEntryId", typeof(int));
                GIGrid.Columns.Add("VoucherDocNo", typeof(string));
                GIGrid.Columns.Add("BillVouchNo", typeof(string));
                GIGrid.Columns.Add("VoucherDocDate", typeof(string));
                GIGrid.Columns.Add("BillInvoiceDate", typeof(string));
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
                GIGrid.Columns.Add("chequeDate", typeof(string));
                GIGrid.Columns.Add("chequeClearDate", typeof(string));
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
                GIGrid.Columns.Add("PoDate", typeof(string));
                GIGrid.Columns.Add("POYear", typeof(int));
                GIGrid.Columns.Add("SONo", typeof(int));
                GIGrid.Columns.Add("CustOrderNo", typeof(string));
                GIGrid.Columns.Add("SoDate", typeof(string));
                GIGrid.Columns.Add("SOYear", typeof(int));
                GIGrid.Columns.Add("ApprovedBy", typeof(int));
                GIGrid.Columns.Add("ApprovedDate", typeof(string));
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
                GIGrid.Columns.Add("MRNDate", typeof(string));
                GIGrid.Columns.Add("MRNYearCode", typeof(int));
                GIGrid.Columns.Add("CostCenterId", typeof(int));
                GIGrid.Columns.Add("PaymentMode", typeof(string));
                GIGrid.Columns.Add("EntryTypebankcashLedger", typeof(string));
                GIGrid.Columns.Add("TDSApplicable", typeof(string));
                GIGrid.Columns.Add("TDSChallanNo", typeof(string));
                GIGrid.Columns.Add("TDSChallanDate", typeof(string));
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
                GIGrid.Columns.Add("ProjectDate", typeof(string));
                GIGrid.Columns.Add("ActualEntryBy", typeof(int));
                GIGrid.Columns.Add("ActualEntryDate", typeof(string));
                GIGrid.Columns.Add("UpdatedBy", typeof(int));
                GIGrid.Columns.Add("LastUpdatedDate", typeof(string));
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
                Item.EntryDate=DateTime.Now.ToString("dd/MMM/yyyy", CultureInfo.InvariantCulture) ,
                Item.DocEntryId ,
                Item.VoucherDocNo ?? string.Empty,
                Item.BillVouchNo ?? string.Empty,
                Item.VoucherDocDate=DateTime.Now.ToString("dd/MMM/yyyy", CultureInfo.InvariantCulture) ,
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
                Item.PoDate =DateTime.Now.ToString("dd/MMM/yyyy",CultureInfo.InvariantCulture),
                Item.POYear ,
                Item.SONo ,
                Item.CustOrderNo ?? string.Empty,
                Item.SoDate  =DateTime.Now.ToString("dd/MMM/yyyy",CultureInfo.InvariantCulture),
                Item.SOYear ,
                Item.ApprovedBy,
                Item.ApprovedDate =DateTime.Now.ToString("dd/MMM/yyyy",CultureInfo.InvariantCulture),
                Item.Approved ,
                Item.PartyWiseNaration ?? string.Empty,
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
                Item.MRNDate =DateTime.Now.ToString("dd/MMM/yyyy",CultureInfo.InvariantCulture),
                Item.MRNYearCode ,
                Item.CostCenterId ,
                Item.PaymentMode ?? string.Empty,
                Item.EntryTypebankcashLedger ?? string.Empty,
                Item.TDSApplicable ?? string.Empty,
                Item.TDSChallanNo ?? string.Empty,
                Item.TDSChallanDate = DateTime.Now.ToString("dd/MMM/yyyy",CultureInfo.InvariantCulture),
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
                Item.ProjectDate = DateTime.Now.ToString("dd/MMM/yyyy",CultureInfo.InvariantCulture),
                Item.ActualEntryby ,
                Item.ActualEntryDate = DateTime.Now.ToString("dd/MMM/yyyy",CultureInfo.InvariantCulture),
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
            var JSON = await _ICashReceipt.GetLedgerBalance(OpeningYearCode, AccountCode, VoucherDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillLedgerName(string VoucherType, string ShowAll)
        {
            var JSON = await _ICashReceipt.FillLedgerName(VoucherType, ShowAll);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillSubVoucherName(string VoucherType)
        {
            var JSON = await _ICashReceipt.FillSubVoucherName(VoucherType);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillBankType(int AccountCode)
        {
            var JSON = await _ICashReceipt.FillBankType(AccountCode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillIntrument(string VoucherType)
        {
            var JSON = await _ICashReceipt.FillIntrument(VoucherType);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillModeofAdjust(string VoucherType)
        {
            var JSON = await _ICashReceipt.FillModeofAdjust(VoucherType);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillCostCenterName()
        {
            var JSON = await _ICashReceipt.FillCostCenterName();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillEntryID(int YearCode, string VoucherDate)
        {
            var JSON = await _ICashReceipt.FillEntryID(YearCode, VoucherDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillCurrency()
        {
            var JSON = await _ICashReceipt.FillCurrency();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillSONO(string accountcode, string VoucherDate)
        {
            var JSON = await _ICashReceipt.FillSONO(accountcode, VoucherDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetSoYearCode(int SONO, string accountcode, string VoucherDate)
        {
            var JSON = await _ICashReceipt.GetSoYearCode(SONO, accountcode, VoucherDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetSODate(int SONO, string accountcode, string VoucherDate, string SOYearCode)
        {
            var JSON = await _ICashReceipt.GetSODate(SONO, accountcode, VoucherDate, SOYearCode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> CheckAmountBeforeSave(string VoucherDate, int YearCode, int AgainstVoucherYearCode, int AgainstVoucherEntryId, string AgainstVoucherNo, int AccountCode)
        {
            var JSON = await _ICashReceipt.CheckAmountBeforeSave(VoucherDate, YearCode, AgainstVoucherYearCode, AgainstVoucherEntryId, AgainstVoucherNo, AccountCode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public IActionResult AddCashReceiptDetail(CashReceiptModel model, string uniqueKey)
        {
            try
            {
                if (model.Mode == "U" || model.Mode == "V")
                {
                    string modelJson = HttpContext.Session.GetString($"KeyCashReceiptGridEdit_{uniqueKey}");
                    List<CashReceiptModel> CashReceiptGrid = new List<CashReceiptModel>();
                    if (!string.IsNullOrEmpty(modelJson))
                    {
                        CashReceiptGrid = JsonConvert.DeserializeObject<List<CashReceiptModel>>(modelJson);
                    }

                    var MainModel = new CashReceiptModel();
                    var WorkOrderPGrid = new List<CashReceiptModel>();
                    var OrderGrid = new List<CashReceiptModel>();
                    var ssGrid = new List<CashReceiptModel>();

                    var count = 0;
                    if (model != null)
                    {
                        if (CashReceiptGrid == null)
                        {
                            model.SrNO = 1;
                            OrderGrid.Add(model);
                        }
                        else
                        {
                            if (model.BankType.ToLower() == "bank")
                            {
                                if (CashReceiptGrid.Any(x => (x.LedgerName == model.LedgerName) || x.BankType == "Bank"))
                                {
                                    return StatusCode(210, "Duplicate");
                                }
                                else
                                {
                                    model.SrNO = CashReceiptGrid.Count + 1;
                                    OrderGrid = CashReceiptGrid.Where(x => x != null).ToList();
                                    ssGrid.AddRange(OrderGrid);
                                    OrderGrid.Add(model);

                                }
                            }
                            else if (model.ModeOfAdjustment.ToLower() == "new ref")
                            {
                                if (CashReceiptGrid.Any(x => (x.LedgerName == model.LedgerName) && (x.ModeOfAdjustment == model.ModeOfAdjustment)))
                                {
                                    return StatusCode(207, "Duplicate");
                                }
                                else
                                {
                                    model.SrNO = CashReceiptGrid.Count + 1;
                                    OrderGrid = CashReceiptGrid.Where(x => x != null).ToList();
                                    ssGrid.AddRange(OrderGrid);
                                    OrderGrid.Add(model);

                                }
                            }
                            else if (model.ModeOfAdjustment.ToLower() == "advance")
                            {
                                if (CashReceiptGrid.Any(x => (x.LedgerName == model.LedgerName) && (x.ModeOfAdjustment == model.ModeOfAdjustment)))
                                {
                                    return StatusCode(208, "Duplicate");
                                }
                                else
                                {
                                    model.SrNO = CashReceiptGrid.Count + 1;
                                    OrderGrid = CashReceiptGrid.Where(x => x != null).ToList();
                                    ssGrid.AddRange(OrderGrid);
                                    OrderGrid.Add(model);

                                }
                            }
                            else if (model.ModeOfAdjustment.ToLower() == "against ref")
                            {
                                if (CashReceiptGrid.Any(x => (x.LedgerName == model.LedgerName) && x.AgainstVoucherNo == model.AgainstVoucherNo))
                                {
                                    return StatusCode(209, "Duplicate");
                                }
                                else
                                {
                                    model.SrNO = CashReceiptGrid.Count + 1;
                                    OrderGrid = CashReceiptGrid.Where(x => x != null).ToList();
                                    ssGrid.AddRange(OrderGrid);
                                    OrderGrid.Add(model);

                                }
                            }

                        }

                        MainModel.CashReceiptGrid = OrderGrid.OrderBy(x => x.SrNO).ToList();

                        string serializedGrid = JsonConvert.SerializeObject(MainModel.CashReceiptGrid);
                        HttpContext.Session.SetString($"KeyCashReceiptGridEdit_{uniqueKey}", serializedGrid);
                    }
                    else
                    {
                        ModelState.TryAddModelError("Error", "Schedule List Cannot Be Empty...!");
                    }
                    return PartialView("_CashReceiptGrid", MainModel);
                }
                else
                {
                    string modelJson = HttpContext.Session.GetString($"KeyCashReceiptGrid_{uniqueKey}");
                    List<CashReceiptModel> CashReceiptGrid = new List<CashReceiptModel>();
                    if (!string.IsNullOrEmpty(modelJson))
                    {
                        CashReceiptGrid = JsonConvert.DeserializeObject<List<CashReceiptModel>>(modelJson);
                    }

                    var MainModel = new CashReceiptModel();
                    var WorkOrderPGrid = new List<CashReceiptModel>();
                    var OrderGrid = new List<CashReceiptModel>();
                    var ssGrid = new List<CashReceiptModel>();

                    if (model != null)
                    {
                        if (CashReceiptGrid == null)
                        {
                            model.SrNO = 1;
                            OrderGrid.Add(model);
                        }
                        else
                        {
                            if (model.BankType.ToLower() == "bank")
                            {
                                if (CashReceiptGrid.Any(x => (x.LedgerName == model.LedgerName) || x.BankType == "Bank"))
                                {
                                    return StatusCode(210, "Duplicate");
                                }
                                else
                                {
                                    model.SrNO = CashReceiptGrid.Count + 1;
                                    OrderGrid = CashReceiptGrid.Where(x => x != null).ToList();
                                    ssGrid.AddRange(OrderGrid);
                                    OrderGrid.Add(model);

                                }
                            }
                            else if (model.ModeOfAdjustment.ToLower() == "new ref")
                            {
                                if (CashReceiptGrid.Any(x => (x.LedgerName == model.LedgerName) && (x.ModeOfAdjustment == model.ModeOfAdjustment)))
                                {
                                    return StatusCode(207, "Duplicate");
                                }
                                else
                                {
                                    model.SrNO = CashReceiptGrid.Count + 1;
                                    OrderGrid = CashReceiptGrid.Where(x => x != null).ToList();
                                    ssGrid.AddRange(OrderGrid);
                                    OrderGrid.Add(model);

                                }
                            }
                            else if (model.ModeOfAdjustment.ToLower() == "advance")
                            {
                                if (CashReceiptGrid.Any(x => (x.LedgerName == model.LedgerName) && (x.ModeOfAdjustment == model.ModeOfAdjustment)))
                                {
                                    return StatusCode(208, "Duplicate");
                                }
                                else
                                {
                                    model.SrNO = CashReceiptGrid.Count + 1;
                                    OrderGrid = CashReceiptGrid.Where(x => x != null).ToList();
                                    ssGrid.AddRange(OrderGrid);
                                    OrderGrid.Add(model);

                                }
                            }
                            else if (model.ModeOfAdjustment.ToLower() == "against ref")
                            {
                                if (CashReceiptGrid.Any(x => (x.LedgerName == model.LedgerName) && x.AgainstVoucherNo == model.AgainstVoucherNo))
                                {
                                    return StatusCode(209, "Duplicate");
                                }
                                else
                                {
                                    model.SrNO = CashReceiptGrid.Count + 1;
                                    OrderGrid = CashReceiptGrid.Where(x => x != null).ToList();
                                    ssGrid.AddRange(OrderGrid);
                                    OrderGrid.Add(model);

                                }
                            }
                        }

                        MainModel.CashReceiptGrid = OrderGrid.OrderBy(x => x.SrNO).ToList();

                        string serializedGrid = JsonConvert.SerializeObject(MainModel.CashReceiptGrid);
                        HttpContext.Session.SetString($"KeyCashReceiptGrid_{uniqueKey}", serializedGrid);
                    }
                    else
                    {
                        ModelState.TryAddModelError("Error", "Schedule List Cannot Be Empty...!");
                    }
                    return PartialView("_CashReceiptGrid", MainModel);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IActionResult AddCashReceiptAdjustDetail(List<CashReceiptModel> model, string Mode, string uniqueKey)
        {
            try
            {
                var MainModel = new CashReceiptModel();
                var CashReceiptDetail = new List<CashReceiptModel>();

                if (Mode != "U" && Mode != "V")
                {
                    string modelJson = HttpContext.Session.GetString($"KeyCashReceiptGrid_{uniqueKey}");
                    List<CashReceiptModel> CashReceiptItemDetail = new List<CashReceiptModel>();
                    if (!string.IsNullOrEmpty(modelJson))
                    {
                        CashReceiptItemDetail = JsonConvert.DeserializeObject<List<CashReceiptModel>>(modelJson);
                    }

                    if (CashReceiptItemDetail != null)
                    {
                        CashReceiptDetail = CashReceiptItemDetail;
                    }

                    foreach (var item in model)
                    {
                        if (item != null)
                        {
                            var existingItem = CashReceiptDetail.FirstOrDefault(x => x.LedgerName == item.LedgerName && x.AgainstVoucherNo == item.AgainstVoucherNo && x.ModeOfAdjustment == item.ModeOfAdjustment);
                            if (existingItem != null)
                            {
                                CashReceiptDetail.Remove(existingItem);
                            }
                            item.SrNO = CashReceiptDetail.Count + 1;
                            item.Type = item.Type.ToLower() == "dr" ? "CR" : "DR";
                            CashReceiptDetail.Add(item);
                        }
                    }
                    for (int i = 0; i < CashReceiptDetail.Count; i++)
                    {
                        CashReceiptDetail[i].SrNO = i + 1; // Ensure proper sequence numbers
                    }
                    MainModel.CashReceiptGrid = CashReceiptDetail.OrderBy(x => x.SrNO).ToList();

                    string serializedGrid = JsonConvert.SerializeObject(MainModel.CashReceiptGrid);
                    HttpContext.Session.SetString($"KeyCashReceiptGrid_{uniqueKey}", serializedGrid);
                }
                else
                {
                    string modelJson = HttpContext.Session.GetString($"KeyCashReceiptGridEdit_{uniqueKey}");
                    List<CashReceiptModel> CashReceiptItemDetail = new List<CashReceiptModel>();
                    if (!string.IsNullOrEmpty(modelJson))
                    {
                        CashReceiptItemDetail = JsonConvert.DeserializeObject<List<CashReceiptModel>>(modelJson);
                    }

                    if (CashReceiptItemDetail != null)
                    {
                        CashReceiptDetail = CashReceiptItemDetail;
                    }

                    foreach (var item in model)
                    {
                        if (item != null)
                        {
                            var existingItem = CashReceiptDetail.FirstOrDefault(x => x.LedgerName == item.LedgerName && x.AgainstVoucherNo == item.AgainstVoucherNo && x.ModeOfAdjustment == item.ModeOfAdjustment);
                            if (existingItem != null)
                            {
                                CashReceiptDetail.Remove(existingItem);
                            }
                            item.Type = item.Type.ToLower() == "dr" ? "CR" : "DR";
                            CashReceiptDetail.Add(item);
                        }
                    }
                    for (int i = 0; i < CashReceiptDetail.Count; i++)
                    {
                        CashReceiptDetail[i].SrNO = i + 1; // Ensure proper sequence numbers
                    }
                    MainModel.CashReceiptGrid = CashReceiptDetail.OrderBy(x => x.SrNO).ToList();
                    string serializedGrid = JsonConvert.SerializeObject(MainModel.CashReceiptGrid);
                    HttpContext.Session.SetString($"KeyCashReceiptGridEdit_{uniqueKey}", serializedGrid);
                }
                return PartialView("_CashReceiptGrid", MainModel);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<JsonResult> EditItemRows(int SrNO, string Mode, string uniqueKey)
        {
            if (Mode != "U" && Mode != "V")
            {
                var MainModel = new CashReceiptModel();
                string modelJson = HttpContext.Session.GetString($"KeyCashReceiptGrid_{uniqueKey}");
                List<CashReceiptModel> GridDetail = new List<CashReceiptModel>();
                if (!string.IsNullOrEmpty(modelJson))
                {
                    GridDetail = JsonConvert.DeserializeObject<List<CashReceiptModel>>(modelJson);
                }
                var SAGrid = GridDetail.Where(x => x.SrNO == SrNO);
                string JsonString = JsonConvert.SerializeObject(SAGrid);
                return Json(JsonString);
            }
            else
            {
                var MainModel = new CashReceiptModel();
                string modelJson = HttpContext.Session.GetString($"KeyCashReceiptGridEdit_{uniqueKey}");
                List<CashReceiptModel> GridDetail = new List<CashReceiptModel>();
                if (!string.IsNullOrEmpty(modelJson))
                {
                    GridDetail = JsonConvert.DeserializeObject<List<CashReceiptModel>>(modelJson);
                }

                var SAGrid = GridDetail.Where(x => x.SrNO == SrNO);
                string JsonString = JsonConvert.SerializeObject(SAGrid);
                return Json(JsonString);
            }
        }
        public IActionResult DeleteItemRow(int SeqNo, string Mode, string PopUpData, string uniqueKey)
        {
            if (PopUpData == "PopUpData")
            {
                var MainModel = new CashReceiptModel();
                string modelJson = HttpContext.Session.GetString($"KeyCashReceiptGridPopUpData_{uniqueKey}");
                List<CashReceiptModel> CashReceiptGrid = new List<CashReceiptModel>();
                if (!string.IsNullOrEmpty(modelJson))
                {
                    CashReceiptGrid = JsonConvert.DeserializeObject<List<CashReceiptModel>>(modelJson);
                }

                int Indx = Convert.ToInt32(SeqNo) - 1;

                if (CashReceiptGrid != null && CashReceiptGrid.Count > 0)
                {
                    CashReceiptGrid.RemoveAt(Convert.ToInt32(Indx));
                    Indx = 0;

                    foreach (var item in CashReceiptGrid)
                    {
                        Indx++;
                    }
                    MainModel.CashReceiptGrid = CashReceiptGrid.OrderBy(x => x.SrNO).ToList();

                    string serializedGrid = JsonConvert.SerializeObject(MainModel.CashReceiptGrid);
                    HttpContext.Session.SetString($"KeyCashReceiptGridPopUpData_{uniqueKey}", serializedGrid);
                }
                return PartialView("_DisplayPopupForPendingVouchers", MainModel);
            }
            else
            {
                var MainModel = new CashReceiptModel();
                if (Mode != "U" && Mode != "V")
                {
                    string modelJson = HttpContext.Session.GetString($"KeyCashReceiptGrid_{uniqueKey}");
                    List<CashReceiptModel> CashReceiptGrid = new List<CashReceiptModel>();
                    if (!string.IsNullOrEmpty(modelJson))
                    {
                        CashReceiptGrid = JsonConvert.DeserializeObject<List<CashReceiptModel>>(modelJson);
                    }

                    int Indx = Convert.ToInt32(SeqNo) - 1;

                    if (CashReceiptGrid != null && CashReceiptGrid.Count > 0)
                    {
                        CashReceiptGrid.RemoveAt(Convert.ToInt32(Indx));
                        Indx = 0;

                        foreach (var item in CashReceiptGrid)
                        {
                            Indx++;
                            item.SrNO = Indx;
                        }
                        MainModel.CashReceiptGrid = CashReceiptGrid.OrderBy(x => x.SrNO).ToList();

                        string serializedGrid = JsonConvert.SerializeObject(MainModel.CashReceiptGrid);
                        HttpContext.Session.SetString($"KeyCashReceiptGrid_{uniqueKey}", serializedGrid);
                    }
                }
                else
                {
                    string modelJson = HttpContext.Session.GetString($"KeyCashReceiptGridEdit_{uniqueKey}");
                    List<CashReceiptModel> CashReceiptGrid = new List<CashReceiptModel>();
                    if (!string.IsNullOrEmpty(modelJson))
                    {
                        CashReceiptGrid = JsonConvert.DeserializeObject<List<CashReceiptModel>>(modelJson);
                    }

                    int Indx = Convert.ToInt32(SeqNo) - 1;

                    if (CashReceiptGrid != null && CashReceiptGrid.Count > 0)
                    {
                        CashReceiptGrid.RemoveAt(Convert.ToInt32(Indx));
                        Indx = 0;

                        foreach (var item in CashReceiptGrid)
                        {
                            Indx++;
                            item.SrNO = Indx;
                        }
                        MainModel.CashReceiptGrid = CashReceiptGrid.OrderBy(x => x.SrNO).ToList();

                        string serializedGrid = JsonConvert.SerializeObject(MainModel.CashReceiptGrid);
                        HttpContext.Session.SetString($"KeyCashReceiptGridEdit_{uniqueKey}", serializedGrid);
                    }
                }
                return PartialView("_CashReceiptGrid", MainModel);
            }
        }
        public async Task<IActionResult> CashReceiptDashBoard(string formKey, string FromDate, string ToDate)
        {
            try
            {
                ViewBag.formKey = formKey;
                var uniqueKey = Guid.NewGuid().ToString();
                ViewBag.uniqueKey = uniqueKey;
                HttpContext.Session.Remove($"KeyCashReceiptGrid_{uniqueKey}");
                HttpContext.Session.Remove($"KeyCashReceiptGridEdit_{uniqueKey}");
                var model = new CashReceiptModel();
                FromDate = HttpContext.Session.GetString($"FromDate_{formKey}");
                ToDate = HttpContext.Session.GetString($"ToDate_{formKey}");
                int userID = Convert.ToInt32(HttpContext.Session.GetString($"EmpID_{formKey}"));
                var rights = await _ICashReceipt.GetFormRights(userID);
                if (rights?.Result == null || rights.Result.Tables.Count == 0 || rights.Result.Tables[0].Rows.Count == 0)
                {
                    return RedirectToAction("Dashboard", "Home", new { formKey = formKey });
                }
                var table = rights.Result.Tables[0];

                bool optAll = Convert.ToBoolean(table.Rows[0]["OptAll"]);
                bool optView = Convert.ToBoolean(table.Rows[0]["OptView"]);
                bool optUpdate = Convert.ToBoolean(table.Rows[0]["OptUpdate"]);
                bool optDelete = Convert.ToBoolean(table.Rows[0]["OptDelete"]);
                if (!(optAll || optView || optUpdate || optDelete))
                {
                    return RedirectToAction("Dashboard", "Home", new { formKey = formKey });
                }
                //var Result = await _ICashReceipt.GetDashBoardData(FromDate, ToDate).ConfigureAwait(true);
                //if (Result != null)
                //{
                //    DataSet ds = Result.Result;
                //    if (ds != null && ds.Tables.Count > 0)
                //    {
                //        var dt = ds.Tables[0];
                //        model.CashReceiptGrid = CommonFunc.DataTableToList<CashReceiptModel>(dt, "CashReceiptDashBoard");
                //    }
                //}
                return View(model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IActionResult> GetSearchData(string formKey, string uniqueKey, string FromDate, string ToDate, string LedgerName, string Bank, string VoucherNo, string AgainstVoucherNo, string SoNo, string AgainstBillno, string summaryDetail, string searchBox, string Flag = "True", int pageNumber = 1, int pageSize = 10)
        {
            ViewBag.formKey = formKey;

            try
            {
                var model = new CashReceiptModel
                {
                    YearCode = Convert.ToInt32(HttpContext.Session.GetString($"YearCode_{formKey}")),
                    Searchbox = searchBox,
                    formKey = formKey,

                    uniqueKey = uniqueKey
                };

                var result = await _ICashReceipt
                //.GetDashboardData(ParseFormattedDate(fromDate), ParseFormattedDate(toDate), custInvoiceNo, AccountCode, mrnNo, gateNo, ItemCode, againstBillNo, voucherNo, summaryDetail, searchBox);
                .GetDashBoardData(summaryDetail, ParseFormattedDate(FromDate), ParseFormattedDate(ToDate), LedgerName, Bank, VoucherNo, AgainstVoucherNo, SoNo, AgainstBillno);

                if (result == null || !(result.Result is DataTable dt))
                {
                    model.TotalRecords = 0;
                    model.Rows = new List<Dictionary<string, object>>();
                    return PartialView("_CashReceiptDashBoardGrid", model);
                }

                model.TotalRecords = dt.Rows.Count;
                model.PageNumber = pageNumber;
                model.PageSize = pageSize;

                model.Headers = dt.Columns
                    .Cast<DataColumn>()
                    .Select(c => new DashboardColumn
                    {
                        Title = c.ColumnName,
                        Field = c.ColumnName
                    })
                    .ToList();

                //model.Rows = dt.AsEnumerable()
                //    .Select(r => dt.Columns
                //        .Cast<DataColumn>()
                //        .ToDictionary(
                //            c => c.ColumnName,
                //            c => r[c] == DBNull.Value ? null : r[c]
                //        ))
                //    .ToList();

                model.Rows = dt.AsEnumerable()
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(r => dt.Columns
                    .Cast<DataColumn>()
                    .ToDictionary(
                        c => c.ColumnName,
                        c => r[c] == DBNull.Value ? null : r[c]
                    ))
                .ToList();

                return PartialView("_CashReceiptDashBoardGrid", model);
            }
            catch
            {
                throw;
            }
        }
        public async Task<IActionResult> GetDashBoardDetailData(string formKey, string uniqueKey, string FromDate, string ToDate, string LedgerName, string Bank, string VoucherNo, string AgainstVoucherNo, string SoNo, string AgainstBillno)
        {
            HttpContext.Session.Remove($"KeyCashReceiptGrid_{uniqueKey}");
            HttpContext.Session.Remove($"KeyCashReceiptGridEdit_{uniqueKey}");
            var model = new CashReceiptModel();
            model = await _ICashReceipt.GetDashBoardDetailData(FromDate, ToDate, LedgerName, Bank, VoucherNo, AgainstVoucherNo, SoNo, AgainstBillno);
            model.formKey = formKey;
            model.uniqueKey = uniqueKey;
            return PartialView("_CashReceiptDashBoardDetailGrid", model);
        }
        public async Task<IActionResult> GetDashBoardSummaryData(string formKey, string uniqueKey, string FromDate, string ToDate, string LedgerName, string Bank, string VoucherNo, string AgainstVoucherNo, string SoNo, string AgainstBillno)
        {
            HttpContext.Session.Remove($"KeyCashReceiptGrid_{uniqueKey}");
            HttpContext.Session.Remove($"KeyCashReceiptGridEdit_{uniqueKey}");
            var model = new CashReceiptModel();
            model = await _ICashReceipt.GetDashBoardSummaryData(FromDate, ToDate, LedgerName, Bank, VoucherNo, AgainstVoucherNo, SoNo, AgainstBillno);
            model.formKey = formKey;
            model.uniqueKey = uniqueKey;
            return PartialView("_CashReceiptDashBoardGrid", model);
        }
        public async Task<IActionResult> PopUpForPendingVouchers(PopUpDataTableAgainstRef DataTable, string uniqueKey)
        {
            HttpContext.Session.Remove($"KeyCashReceiptGridPopUpData_{uniqueKey}");
            var model = await _ICashReceipt.PopUpForPendingVouchers(DataTable);
            string serializedGrid = JsonConvert.SerializeObject(model.CashReceiptGrid);
            HttpContext.Session.SetString($"KeyCashReceiptGridPopUpData_{uniqueKey}", serializedGrid);
            return PartialView("_DisplayPopupForPendingVouchers", model);
        }
        public async Task<IActionResult> DeleteByID(string formKey, int ID, int YearCode, int ActualEntryBy, string EntryByMachine, string ActualEntryDate, string VoucherType)
        {
            var Result = await _ICashReceipt.DeleteByID(ID, YearCode, ActualEntryBy, EntryByMachine, ActualEntryDate, VoucherType);

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

            return RedirectToAction("CashReceiptDashBoard", new { formKey = formKey });

        }
        public async Task<JsonResult> FillLedgerInDashboard(string FromDate, string ToDate, string VoucherType)
        {
            var JSON = await _ICashReceipt.FillLedgerInDashboard(FromDate, ToDate, VoucherType);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillBankInDashboard(string FromDate, string ToDate, string VoucherType)
        {
            var JSON = await _ICashReceipt.FillBankInDashboard(FromDate, ToDate, VoucherType);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillVoucherNoInDashboard(string FromDate, string ToDate, string VoucherType)
        {
            var JSON = await _ICashReceipt.FillVoucherNoInDashboard(FromDate, ToDate, VoucherType);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillAgainstVoucherRefNoInDashboard(string FromDate, string ToDate, string VoucherType)
        {
            var JSON = await _ICashReceipt.FillAgainstVoucherRefNoInDashboard(FromDate, ToDate, VoucherType);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillAgainstVoucherNoInDashboard(string FromDate, string ToDate, string VoucherType)
        {
            var JSON = await _ICashReceipt.FillAgainstVoucherNoInDashboard(FromDate, ToDate, VoucherType);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }

        [HttpPost]
        public IActionResult RemoveAgainstRef(string mode, string uniqueKey)
        {
            try
            {
                string sessionKey = (mode == "U" || mode == "V")
                    ? $"KeyCashReceiptGridEdit_{uniqueKey}"
                    : $"KeyCashReceiptGrid_{uniqueKey}";

                string modelJson = HttpContext.Session.GetString(sessionKey);

                List<CashReceiptModel> CashReceiptGrid = new List<CashReceiptModel>();

                if (!string.IsNullOrEmpty(modelJson))
                {
                    CashReceiptGrid = JsonConvert.DeserializeObject<List<CashReceiptModel>>(modelJson);
                }

                // Remove rows where ModeOfAdjustment = Against Ref
                CashReceiptGrid = CashReceiptGrid
                    .Where(x => x.ModeOfAdjustment?.ToLower() != "against ref")
                    .ToList();

                // Reorder SrNO
                int sr = 1;
                foreach (var item in CashReceiptGrid)
                {
                    item.SrNO = sr++;
                }

                HttpContext.Session.SetString(sessionKey, JsonConvert.SerializeObject(CashReceiptGrid));

                var MainModel = new CashReceiptModel
                {
                    CashReceiptGrid = CashReceiptGrid
                };

                return PartialView("_CashReceiptGrid", MainModel);
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }
    }
}
