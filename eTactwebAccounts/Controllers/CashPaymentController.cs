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
using System.Globalization;
using ClosedXML.Excel;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;

namespace eTactwebAccounts.Controllers
{
    public class CashPaymentController : Controller
    {
        private readonly IDataLogic _IDataLogic;
        public ICashPayment _ICashPayment { get; }
        private readonly ILogger<CashPaymentController> _logger;
        private readonly IConfiguration iconfiguration;
        public IWebHostEnvironment _IWebHostEnvironment { get; }
        private readonly ConnectionStringService _connectionStringService;
        public EncryptDecrypt EncryptDecrypt { get; }
        public CashPaymentController(ILogger<CashPaymentController> logger, IDataLogic iDataLogic, ICashPayment iCashPayment, EncryptDecrypt encryptDecrypt, IWebHostEnvironment iWebHostEnvironment, IConfiguration iconfiguration, ConnectionStringService connectionStringService)
        {
            _logger = logger;
            _IDataLogic = iDataLogic;
            _ICashPayment = iCashPayment;
            EncryptDecrypt = encryptDecrypt;
            _IWebHostEnvironment = iWebHostEnvironment;
            this.iconfiguration = iconfiguration;
            _connectionStringService = connectionStringService;
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
                ReportName = row["CashPaymentVoucherPrintReportName"]?.ToString() ?? "";
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
        public async Task<JsonResult> GetFormRights(string formKey)
        {
            var userID = Convert.ToInt32(HttpContext.Session.GetString($"EmpID_{formKey}"));
            var JSON = await _ICashPayment.GetFormRights(userID);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        [Route("{controller}/Index")]
        [HttpGet]
        public async Task<ActionResult> CashPayment(string formKey, int ID, string Mode, int YearCode, string VoucherNo, string FromDate = "", string ToDate = "", string LedgerName = "", string AgainstVoucherRefNo = "", string AgainstVoucherNo = "", string Searchbox = "", string DashboardType = "")
        {
            ViewBag.formKey = formKey;
            var uniqueKey = Guid.NewGuid().ToString();
            ViewBag.uniqueKey = uniqueKey;
            HttpContext.Session.Remove($"KeyCashPaymentGrid_{uniqueKey}");
            HttpContext.Session.Remove($"KeyCashPaymentGridEdit_{uniqueKey}");
            //TempData.Clear();

            int userID = Convert.ToInt32(HttpContext.Session.GetString($"EmpID_{formKey}"));
            var rights = await _ICashPayment.GetFormRights(userID);
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



            var MainModel = new CashPaymentModel();
            MainModel.CC = HttpContext.Session.GetString($"Branch_{formKey}");
            MainModel.YearCode = Convert.ToInt32(HttpContext.Session.GetString($"YearCode_{formKey}"));
            MainModel.ActualEntryby = Convert.ToInt32(HttpContext.Session.GetString($"UID_{formKey}"));
            MainModel.ActualEntryBy = HttpContext.Session.GetString($"EmpName_{formKey}");
            MainModel.ActualEntryDate = DateTime.Now.ToString("dd/MM/yy");
            MainModel.UID = Convert.ToInt32(HttpContext.Session.GetString($"UID_{formKey}"));
            //MainModel.FromDate = HttpContext.Session.GetString($"FromDate_{formKey}");
            //MainModel.ToDate = HttpContext.Session.GetString($"ToDate_{formKey}");

            if (!string.IsNullOrEmpty(Mode) && ID > 0 && Mode == "U" || Mode == "V")
            {
                MainModel = await _ICashPayment.GetViewByID(ID, YearCode, VoucherNo).ConfigureAwait(false);
                MainModel.Mode = Mode; // Set Mode to Update
                MainModel.ID = ID;
                MainModel.YearCode = YearCode;
                MainModel.VoucherNo = VoucherNo;
                string serializedGrid = JsonConvert.SerializeObject(MainModel.CashPaymentGrid);
                HttpContext.Session.SetString($"KeyCashPaymentGridEdit_{uniqueKey}", serializedGrid);
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
        public async Task<IActionResult> CashPayment(CashPaymentModel model)
        {
            try
            {
                string formKey = model.formKey;
                string uniqueKey = model.uniqueKey;
                var GIGrid = new DataTable();
                string cashModel = HttpContext.Session.GetString($"KeyCashPaymentGrid_{uniqueKey}");
                List<CashPaymentModel> CashPaymentGrid = new List<CashPaymentModel>();
                if (!string.IsNullOrEmpty(cashModel))
                {
                    CashPaymentGrid = JsonConvert.DeserializeObject<List<CashPaymentModel>>(cashModel);
                }
                string cashEditModel = HttpContext.Session.GetString($"KeyCashPaymentGridEdit_{uniqueKey}");
                List<CashPaymentModel> CashPaymentGridEdit = new List<CashPaymentModel>();
                if (!string.IsNullOrEmpty(cashEditModel))
                {
                    CashPaymentGridEdit = JsonConvert.DeserializeObject<List<CashPaymentModel>>(cashEditModel);
                }

                model.CreatedBy = Convert.ToInt32(HttpContext.Session.GetString($"UID_{formKey}"));
                if (model.Mode == "U")
                {
                    model.UpdatedOn = DateTime.Now;
                    GIGrid = GetDetailTable(CashPaymentGridEdit);
                }
                else
                {
                    GIGrid = GetDetailTable(CashPaymentGrid);
                }
                model.EntryByMachine = HttpContext.Session.GetString($"ClientMachineName_{formKey}");
                model.IPAddress = HttpContext.Session.GetString($"ClientIP_{formKey}");
                var Result = await _ICashPayment.SaveCashPayment(model, GIGrid);
                if (Result != null)
                {
                    if (Result.StatusText == "Success" && Result.StatusCode == HttpStatusCode.OK)
                    {
                        ViewBag.isSuccess = true;
                        TempData["200"] = "200";
                        HttpContext.Session.Remove($"KeyCashPaymentGrid_{uniqueKey}");
                        return Json(new
                        {
                            success = true,
                            message = "Cash Payment Save successfully",
                            redirectUrl = Url.Action(
        "Index",
        "CashPayment",
        new { formKey = formKey }

    )
                        });
                    }
                    else if (Result.StatusText == "Success" && Result.StatusCode == HttpStatusCode.Accepted)
                    {
                        ViewBag.isSuccess = true;
                        TempData["202"] = "202";
                        HttpContext.Session.Remove($"KeyCashPaymentGridEdit_{uniqueKey}");
                        return Json(new
                        {
                            success = true,
                            message = "Cash Payment Updated successfully",
                            redirectUrl = Url.Action(
       "Index",
       "CashPayment",
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
                        HttpContext.Session.Remove($"KeyCashPaymentGrid_{uniqueKey}");
                        HttpContext.Session.Remove($"KeyCashPaymentGridEdit_{uniqueKey}");
                        //return View(model);
                        return Json(new
                        {
                            success = false,
                            message = Result.StatusText
                        });
                    }
                }
                return Json(new
                {
                    success = false,
                    message = "An unexpected error occurred."
                });
                //return RedirectToAction(nameof(CashPaymentDashBoard),new {formKey=formKey});

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
                Item.EntryDate=DateTime.Now.ToString("dd/MMM/yyyy", CultureInfo.InvariantCulture)  ,
                Item.DocEntryId ,
                Item.VoucherDocNo ?? string.Empty,
                Item.BillVouchNo ?? string.Empty,
                Item.VoucherDocDate=DateTime.Now.ToString("dd/MMM/yyyy", CultureInfo.InvariantCulture)  ,
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
                Item.PoDate =DateTime.Now.ToString("dd/MMM/yyyy", CultureInfo.InvariantCulture) ,
                Item.POYear ,
                Item.SONo ,
                Item.CustOrderNo ?? string.Empty,
                Item.SoDate = DateTime.Now.ToString("dd/MMM/yyyy", CultureInfo.InvariantCulture) ,
                Item.SOYear ,
                Item.ApprovedBy,
                Item.ApprovedDate = DateTime.Now.ToString("dd/MMM/yyyy", CultureInfo.InvariantCulture) ,
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
                Item.MRNDate =DateTime.Now.ToString("dd/MMM/yyyy", CultureInfo.InvariantCulture) ,
                Item.MRNYearCode ,
                Item.CostCenterId ,
                Item.PaymentMode ?? string.Empty,
                Item.EntryTypebankcashLedger ?? string.Empty,
                Item.TDSApplicable ?? string.Empty,
                Item.TDSChallanNo ?? string.Empty,
                Item.TDSChallanDate = DateTime.Now.ToString("dd/MMM/yyyy", CultureInfo.InvariantCulture) ,
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
                Item.ProjectDate =DateTime.Now.ToString("dd/MMM/yyyy", CultureInfo.InvariantCulture) ,
                Item.ActualEntryby ,
                Item.ActualEntryDate =DateTime.Now.ToString("dd/MMM/yyyy", CultureInfo.InvariantCulture) ,
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
        public IActionResult AddCashPaymentDetail(CashPaymentModel model, string uniqueKey)
        {
            try
            {
                if (model.Mode == "U" || model.Mode == "V")
                {
                    string cashEditModel = HttpContext.Session.GetString($"KeyCashPaymentGridEdit_{uniqueKey}");
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
                        HttpContext.Session.SetString($"KeyCashPaymentGridEdit_{uniqueKey}", serializedGrid);
                    }
                    else
                    {
                        ModelState.TryAddModelError("Error", "Schedule List Cannot Be Empty...!");
                    }
                    return PartialView("_CashPaymentGrid", MainModel);
                }
                else
                {
                    string modelJson = HttpContext.Session.GetString($"KeyCashPaymentGrid_{uniqueKey}");
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
                        HttpContext.Session.SetString($"KeyCashPaymentGrid_{uniqueKey}", serializedGrid);
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
        public IActionResult AddCashPaymentAdjustDetail(List<CashPaymentModel> model, string Mode, string uniqueKey)
        {
            try
            {
                var MainModel = new CashPaymentModel();
                var ProductionEntryDetail = new List<CashPaymentModel>();
                if (Mode != "U" && Mode != "V")
                {
                    string modelJson = HttpContext.Session.GetString($"KeyCashPaymentGrid_{uniqueKey}");
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
                    HttpContext.Session.SetString($"KeyCashPaymentGrid_{uniqueKey}", serializedGrid);

                }
                else
                {

                    string modelJson = HttpContext.Session.GetString($"KeyCashPaymentGridEdit_{uniqueKey}");
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
                    HttpContext.Session.SetString($"KeyCashPaymentGridEdit_{uniqueKey}", serializedGrid);
                }

                return PartialView("_CashPaymentGrid", MainModel);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<JsonResult> EditItemRows(int SrNO, string Mode, string uniqueKey)
        {
            List<CashPaymentModel> GridDetail = new List<CashPaymentModel>();
            if (Mode != "U" && Mode != "V")
            {
                var MainModel = new CashPaymentModel();
                string modelJson = HttpContext.Session.GetString($"KeyCashPaymentGrid_{uniqueKey}");

                if (!string.IsNullOrEmpty(modelJson))
                {
                    GridDetail = JsonConvert.DeserializeObject<List<CashPaymentModel>>(modelJson);
                }
            }
            else
            {
                var MainModel = new CashPaymentModel();
                string modelJson = HttpContext.Session.GetString($"KeyCashPaymentGridEdit_{uniqueKey}");
                if (!string.IsNullOrEmpty(modelJson))
                {
                    GridDetail = JsonConvert.DeserializeObject<List<CashPaymentModel>>(modelJson);
                }
            }
            var SAGrid = GridDetail.Where(x => x.SrNO == SrNO);
            string JsonString = JsonConvert.SerializeObject(SAGrid);
            return Json(JsonString);
        }
        public IActionResult DeleteItemRow(int SeqNo, string Mode, string PopUpData, string uniqueKey, bool isedit = false)
        {
            if (PopUpData == "PopUpData")
            {
                var MainModel = new CashPaymentModel();
                string modelJson = HttpContext.Session.GetString($"KeyCashPaymentGridPopUpData_{uniqueKey}");
                List<CashPaymentModel> CashPaymentGrid = new List<CashPaymentModel>();
                if (!string.IsNullOrEmpty(modelJson))
                {
                    CashPaymentGrid = JsonConvert.DeserializeObject<List<CashPaymentModel>>(modelJson);
                }

                int Indx = Convert.ToInt32(SeqNo) - 1;

                if (CashPaymentGrid != null && CashPaymentGrid.Count > 0)
                {
                    CashPaymentGrid.RemoveAt(Convert.ToInt32(Indx));
                    //Indx = 0;

                    //foreach (var item in CashPaymentGrid)
                    //{
                    //    Indx++;
                    //    item.SequenceNo = Indx;
                    //}
                    //MainModel.CashPaymentGrid = CashPaymentGrid;
                    MainModel.CashPaymentGrid = ProcessGrid(CashPaymentGrid, isedit);//kinjal

                    string serializedGrid = JsonConvert.SerializeObject(MainModel.CashPaymentGrid);
                    HttpContext.Session.SetString($"KeyCashPaymentGridPopUpData_{uniqueKey}", serializedGrid);
                }
                return PartialView("_DisplayPopupForPendingVouchers", MainModel);
            }
            else
            {
                var MainModel = new CashPaymentModel();
                if (Mode != "U" && Mode != "V")
                {
                    string modelJson = HttpContext.Session.GetString($"KeyCashPaymentGrid_{uniqueKey}");
                    List<CashPaymentModel> CashPaymentGrid = new List<CashPaymentModel>();
                    if (!string.IsNullOrEmpty(modelJson))
                    {
                        CashPaymentGrid = JsonConvert.DeserializeObject<List<CashPaymentModel>>(modelJson);
                    }

                    int Indx = Convert.ToInt32(SeqNo) - 1;

                    if (CashPaymentGrid != null && CashPaymentGrid.Count > 0)
                    {
                        CashPaymentGrid.RemoveAt(Convert.ToInt32(Indx));
                        //Indx = 0;

                        //foreach (var item in CashPaymentGrid)
                        //{
                        //    Indx++;
                        //    // item.SequenceNo = Indx;
                        //}
                        //MainModel.CashPaymentGrid = CashPaymentGrid;

                        MainModel.CashPaymentGrid = ProcessGrid(CashPaymentGrid, isedit);//kinjal

                        string serializedGrid = JsonConvert.SerializeObject(MainModel.CashPaymentGrid);
                        HttpContext.Session.SetString($"KeyCashPaymentGrid_{uniqueKey}", serializedGrid);
                    }
                }
                else
                {
                    string modelJson = HttpContext.Session.GetString($"KeyCashPaymentGridEdit_{uniqueKey}");
                    List<CashPaymentModel> CashPaymentGrid = new List<CashPaymentModel>();
                    if (!string.IsNullOrEmpty(modelJson))
                    {
                        CashPaymentGrid = JsonConvert.DeserializeObject<List<CashPaymentModel>>(modelJson);
                    }

                    int Indx = Convert.ToInt32(SeqNo) - 1;

                    if (CashPaymentGrid != null && CashPaymentGrid.Count > 0)
                    {
                        CashPaymentGrid.RemoveAt(Convert.ToInt32(Indx));
                        //Indx = 0;

                        //foreach (var item in CashPaymentGrid)
                        //{
                        //    Indx++;
                        //    // item.SequenceNo = Indx;
                        //}
                        //MainModel.CashPaymentGrid = CashPaymentGrid;
                        MainModel.CashPaymentGrid = ProcessGrid(CashPaymentGrid, isedit);//kinjal
                        string serializedGrid = JsonConvert.SerializeObject(MainModel.CashPaymentGrid);
                        HttpContext.Session.SetString($"KeyCashPaymentGridEdit_{uniqueKey}", serializedGrid);

                    }
                }
                return PartialView("_CashPaymentGrid", MainModel);
            }
        }

        //kinjal add this processgrid fun
        private List<CashPaymentModel> ProcessGrid(List<CashPaymentModel> list, bool isedit)
        {
            if (list == null || list.Count == 0)
                return new List<CashPaymentModel>();

            var orderedList = list.OrderBy(x => x.SrNO).ToList();

            if (!isedit)
            {
                // Resequence SrNO
                for (int i = 0; i < orderedList.Count; i++)
                {
                    orderedList[i].SrNO = i + 1;
                }
            }

            return orderedList;
        }
        public async Task<IActionResult> CashPaymentDashBoard(string formKey, string FromDate, string ToDate, string LedgerName = "", string Bank = "", string VoucherNo = "", string AgainstVoucherRefNo = "", string AgainstVoucherNo = "", string PONo = "", string AgainstBillno = "", string DashboardType = "", string Mode = "")
        {
            try
            {
                ViewBag.formKey = formKey;
                var uniqueKey = Guid.NewGuid().ToString();
                ViewBag.uniqueKey = uniqueKey;
                HttpContext.Session.Remove($"KeyCashPaymentGrid_{uniqueKey}");
                HttpContext.Session.Remove($"KeyCashPaymentGridEdit_{uniqueKey}");
                var model = new CashPaymentModel();
                int userID = Convert.ToInt32(HttpContext.Session.GetString($"EmpID_{formKey}"));
                var rights = await _ICashPayment.GetFormRights(userID);
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
                //FromDate = HttpContext.Session.GetString($"FromDate_{formKey}");
                //ToDate = HttpContext.Session.GetString($"ToDate_{formKey}");
                //var Result = await _ICashPayment.GetDashBoardData(FromDate, ToDate).ConfigureAwait(true);
                //if (Result != null)
                //{
                //    DataSet ds = Result.Result;
                //    if (ds != null && ds.Tables.Count > 0)
                //    {
                //        var dt = ds.Tables[0];
                //        model.CashPaymentGrid = CommonFunc.DataTableToList<CashPaymentModel>(dt, "CashPaymentDashBoard");
                //    }
                //}
                model.FromDate = FromDate;
                model.ToDate = ToDate;
                model.LedgerName = LedgerName;
                model.Bank = Bank;
                model.VoucherNo = VoucherNo;
                model.AgainstVoucherRefNo = AgainstVoucherRefNo;
                model.AgainstVoucherNo = AgainstVoucherNo;

                model.DashboardType = DashboardType;
                model.Mode = Mode;
                return View(model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IActionResult> GetDashBoardDetailData(string formKey, string uniqueKey, string FromDate, string ToDate, string LedgerName, string Bank, string VoucherNo, string AgainstVoucherNo, string PoNo, string AgainstBillno)
        {
            HttpContext.Session.Remove($"KeyCashPaymentGrid_{uniqueKey}");
            HttpContext.Session.Remove($"KeyCashPaymentGridEdit_{uniqueKey}");
            var model = new CashPaymentModel();
            model = await _ICashPayment.GetDashBoardDetailData(FromDate, ToDate, LedgerName, Bank, VoucherNo, AgainstVoucherNo, PoNo, AgainstBillno);
            return PartialView("_CashPaymentDashBoardDetailGrid", model);
        }
        public async Task<IActionResult> GetSearchData(string formKey, string uniqueKey, string FromDate, string ToDate, string LedgerName, string Bank, string VoucherNo, string AgainstVoucherNo, string PoNo, string AgainstBillno, string summaryDetail, string searchBox, string Flag = "True", int pageNumber = 1, int pageSize = 10)
        {
            ViewBag.formKey = formKey;

            try
            {
                ViewBag.formKey = formKey;

                var model = new CashPaymentModel
                {
                    YearCode = Convert.ToInt32(HttpContext.Session.GetString($"YearCode_{formKey}")),
                    Searchbox = searchBox,
                    formKey = formKey,
                    uniqueKey = uniqueKey
                };

                var result = await _ICashPayment.GetDashBoardData(summaryDetail, ParseFormattedDate(FromDate), ParseFormattedDate(ToDate), LedgerName, Bank, VoucherNo, AgainstVoucherNo, PoNo, AgainstBillno);

                if (result == null || !(result.Result is DataTable dt))
                {
                    model.TotalRecords = 0;
                    model.Rows = new List<Dictionary<string, object>>();
                    return PartialView("_CashPaymentDashBoardGrid", model);
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

                return PartialView("_CashPaymentDashBoardGrid", model);
            }
            catch
            {
                throw;
            }
        }

        public async Task<IActionResult> GetDashBoardSummaryData(string formKey, string uniqueKey, string FromDate, string ToDate, string LedgerName, string Bank, string VoucherNo, string AgainstVoucherNo, string PoNo, string AgainstBillno)
        {
            ViewBag.formKey = formKey;

            HttpContext.Session.Remove($"KeyCashPaymentGrid_{uniqueKey}");
            HttpContext.Session.Remove($"KeyCashPaymentGridEdit_{uniqueKey}");
            var model = new CashPaymentModel();
            model = await _ICashPayment.GetDashBoardSummaryData(FromDate, ToDate, LedgerName, Bank, VoucherNo, AgainstVoucherNo, PoNo, AgainstBillno);
            model.formKey = formKey;
            model.uniqueKey = uniqueKey;
            return PartialView("_CashPaymentDashBoardGrid", model);
        }
        public async Task<IActionResult> PopUpForPendingVouchers(PopUpDataTableAgainstRef DataTable, string uniqueKey)
        {
            HttpContext.Session.Remove($"KeyCashPaymentGridPopUpData_{uniqueKey}");
            var model = await _ICashPayment.PopUpForPendingVouchers(DataTable);
            string serializedGrid = JsonConvert.SerializeObject(model.CashPaymentGrid);
            HttpContext.Session.SetString($"KeyCashPaymentGridPopUpData_{uniqueKey}", serializedGrid);
            return PartialView("_DisplayPopupForPendingVouchers", model);
        }
        public async Task<IActionResult> DeleteByID(string formKey, int ID, int YearCode, int ActualEntryBy, string EntryByMachine, string ActualEntryDate, string VoucherType, string FromDate, string ToDate, string LedgerName = "", string Bank = "", string VoucherNo = "", string AgainstVoucherRefNo = "", string AgainstVoucherNo = "", string PONo = "", string AgainstBillno = "", string DashboardType = "", string Mode = "")
        {
            var Result = await _ICashPayment.DeleteByID(ID, YearCode, ActualEntryBy, EntryByMachine, ActualEntryDate, VoucherType);

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

            return RedirectToAction("CashPaymentDashBoard", new { formKey = formKey, FromDate = FromDate, ToDate = ToDate, LedgerName = LedgerName, Bank = Bank, VoucherNo = VoucherNo, AgainstVoucherRefNo = AgainstVoucherRefNo, AgainstVoucherNo = AgainstVoucherNo, PONo = PONo, AgainstBillno = AgainstBillno = "", DashboardType = DashboardType = "", Mode = "Search" });

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
        [HttpPost]
        public IActionResult RemoveAgainstRef(string mode, string uniqueKey)
        {
            try
            {
                string sessionKey = (mode == "U" || mode == "V")
                    ? $"KeyCashPaymentGridEdit_{uniqueKey}"
                    : $"KeyCashPaymentGrid_{uniqueKey}";

                string modelJson = HttpContext.Session.GetString(sessionKey);

                List<CashPaymentModel> CashPaymentGrid = new List<CashPaymentModel>();

                if (!string.IsNullOrEmpty(modelJson))
                {
                    CashPaymentGrid = JsonConvert.DeserializeObject<List<CashPaymentModel>>(modelJson);
                }

                // Remove rows where ModeOfAdjustment = Against Ref
                CashPaymentGrid = CashPaymentGrid
                    .Where(x => x.ModeOfAdjustment?.ToLower() != "against ref")
                    .ToList();

                // Reorder SrNO
                int sr = 1;
                foreach (var item in CashPaymentGrid)
                {
                    item.SrNO = sr++;
                }

                HttpContext.Session.SetString(sessionKey, JsonConvert.SerializeObject(CashPaymentGrid));

                var MainModel = new CashPaymentModel
                {
                    CashPaymentGrid = CashPaymentGrid
                };

                return PartialView("_CashPaymentGrid", MainModel);
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }
    }
}
