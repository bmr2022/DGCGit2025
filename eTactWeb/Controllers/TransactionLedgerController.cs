using eTactWeb.Data.Common;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using eTactWeb.DOM.Models;
using eTactWeb.Services;
using eTactWeb.DOM;
using eTactWeb.Services.Helpers;

namespace eTactWeb.Controllers
{
    public class TransactionLedgerController : BaseNavigationController
    {
        private readonly IDataLogic _IDataLogic;
        public ITransactionLedger _TransactionLedger { get; }

        private readonly ILogger<TransactionLedgerController> _logger;
        private readonly IConfiguration iconfiguration;
        public IWebHostEnvironment _IWebHostEnvironment { get; }
        public TransactionLedgerController(ILogger<TransactionLedgerController> logger, IDataLogic iDataLogic, ITransactionLedger iTransactionLedger, EncryptDecrypt encryptDecrypt, IWebHostEnvironment iWebHostEnvironment, IConfiguration iconfiguration)
        {
            _logger = logger;
            _IDataLogic = iDataLogic;
            _TransactionLedger = iTransactionLedger;
            _IWebHostEnvironment = iWebHostEnvironment;
            this.iconfiguration = iconfiguration;
        }
        [Route("{controller}/Index")]
        [HttpGet]
        public async Task<ActionResult> TransactionLedger()
        {
            var MainModel = new TransactionLedgerModel();
            MainModel.TransactionLedgerGrid = new List<TransactionLedgerModel>();
            MainModel.FromDate = HttpContext.Session.GetString("FromDate");
            const string filterKey = "TransactionLedgerFilters";

            // Try to get previously saved filters from session
            var savedFilters = FilterHelper.GetFilters(HttpContext, filterKey);

            if (savedFilters != null)
            {
                MainModel.FromDate = savedFilters.FromDate;
                MainModel.ToDate = savedFilters.ToDate;
                MainModel.ReportType = savedFilters.ReportType;
                MainModel.GroupOrLedger = savedFilters.GroupOrLedger;
                MainModel.ParentAccountCode = savedFilters.ParentAccountCode;
                MainModel.AccountCode = savedFilters.AccountCode;
                MainModel.VoucherType = savedFilters.VoucherType;
                MainModel.VoucherNo = savedFilters.VoucherNo;
                MainModel.INVNo = savedFilters.InvoiceNo;
                MainModel.Narration = savedFilters.Narration;
                MainModel.Amount = savedFilters.Amount;
                MainModel.Dr = savedFilters.DR;
                MainModel.Cr = savedFilters.CR;
                MainModel.LedgerName = savedFilters.Ledger;
            }

            // Empty grid initially
            MainModel.TransactionLedgerGrid = new List<TransactionLedgerModel>();
            //MainModel.ToDate = HttpContext.Session.GetString("ToDate");
            return View(MainModel); 
        }
        public async Task<JsonResult> GetLedgerName(int? ParentAccountCode)
        {
            var JSON = await _TransactionLedger.GetLedgerName(ParentAccountCode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillLedgerName()
        {
            var JSON = await _TransactionLedger.FillLedgerName();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<IActionResult> GetDetailsData(
                string FromDate = null, string ToDate = null, string ReportType = null,
                string GroupOrLedger = null, int? ParentAccountCode = null, int? AccountCode = null,
                string VoucherType = null, string VoucherNo = null, string InvoiceNo = null,
                string Narration = null, float? Amount = null, string DR = null, string CR = null,
                string Ledger = null)
        {
            const string filterKey = "TransactionLedgerFilters";

            // Get existing saved filters from session
            var saved = FilterHelper.GetFilters(HttpContext, filterKey) ?? new ReportFilter();

            // Merge (prefer new incoming filters)
            var filters = new ReportFilter
            {
                FromDate = FromDate ?? saved.FromDate,
                ToDate = ToDate ?? saved.ToDate,
                ReportType = ReportType ?? saved.ReportType,
                GroupOrLedger = GroupOrLedger ?? saved.GroupOrLedger,
                ParentAccountCode = ParentAccountCode ?? saved.ParentAccountCode,
                AccountCode = (AccountCode == null || AccountCode == 0) ? saved.AccountCode : AccountCode,
                VoucherType = VoucherType ?? saved.VoucherType,
                VoucherNo = VoucherNo ?? saved.VoucherNo,
                InvoiceNo = InvoiceNo ?? saved.InvoiceNo,
                Narration = Narration ?? saved.Narration,
                Amount = Amount ?? saved.Amount,
                DR = DR ?? saved.DR,
                CR = CR ?? saved.CR,
                Ledger = Ledger ?? saved.Ledger
            };

            // Save merged filters back
            FilterHelper.SaveFilters(HttpContext, filterKey, filters);

            // Call your data service with unified filters
            var model = await _TransactionLedger.GetDetailsData(
                filters.FromDate, filters.ToDate, filters.ReportType, filters.GroupOrLedger,
                filters.ParentAccountCode, filters.AccountCode, filters.VoucherType,
                filters.VoucherNo, filters.InvoiceNo, filters.Narration, filters.Amount,
                filters.DR, filters.CR, filters.Ledger
            );

            // Optional cache (useful for Back navigation)
            HttpContext.Session.SetString("TransactionLedgerData", JsonConvert.SerializeObject(model));

            return PartialView("_TransactionLedgerGrid", model);
        }
        public async Task<IActionResult> GetTransactionLedgerMonthlySummaryDetailsData(string FromentryDate, string ToEntryDate, int AccountCode)
        {
            var model = new TransactionLedgerModel();
            model = await _TransactionLedger.GetTransactionLedgerMonthlySummaryDetailsData(FromentryDate, ToEntryDate, AccountCode);
            var sessionData = JsonConvert.SerializeObject(model);
            HttpContext.Session.SetString("TransactionLedgerData", sessionData);
            return PartialView("_TransactionLedgerMonthlySummaryGrid", model);

        }
		public async Task<IActionResult> GetTransactionLedgerGroupSummaryDetailsData(string FromDate, string ToDate, string ReportType, string GroupOrLedger, int? ParentAccountCode = null, int AccountCode = 0, string? VoucherType = null, string? VoucherNo = null, string? InvoiceNo = null, string? Narration = null, float? Amount = null, string? DR = null, string? CR = null, string? Ledger = null)
		{
			var model = new TransactionLedgerModel();
			model = await _TransactionLedger.GetTransactionLedgerGroupSummaryDetailsData(FromDate, ToDate, ReportType, GroupOrLedger,ParentAccountCode, AccountCode, VoucherType,VoucherNo,InvoiceNo,Narration,Amount,DR,CR,Ledger);
            var sessionData = JsonConvert.SerializeObject(model);
            HttpContext.Session.SetString("TransactionLedgerData", sessionData);
            return PartialView("_TransactionLedgerGroupSummaryGrid", model);
		}
		public async Task<JsonResult> FillVoucherName()
        {
            var JSON = await _TransactionLedger.FillVoucherName();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        [HttpGet]
        public IActionResult ExportTransactionLedgerToExcel(string ReportType, string FromDate, string ToDate)
        {
            var BranchName = HttpContext.Session.GetString("Branch");
            var CompanyName = HttpContext.Session.GetString("CompanyName");

            // Get session data
            var sessionData = HttpContext.Session.GetString("TransactionLedgerData");
            if (string.IsNullOrEmpty(sessionData))
                return BadRequest("Transaction ledger data not found in session.");

            var model = JsonConvert.DeserializeObject<TransactionLedgerModel>(sessionData);
            DataTable dt = new DataTable("TransactionLedger");
            int sr = 1;
            string sheetName = ReportType;

            if (ReportType == "TransactionLedgerSummary" || ReportType == "TransactionLedgerDetail")
            {
                dt.Columns.Add("Sr#", typeof(int));
                dt.Columns.Add("Voucher Doc Date", typeof(string));
                dt.Columns.Add("Particulars", typeof(string));
                dt.Columns.Add("Voucher Type", typeof(string));
                dt.Columns.Add("Inv/Vch No", typeof(string));
                dt.Columns.Add("DR Amt", typeof(decimal));
                dt.Columns.Add("CR Amt", typeof(decimal));
                dt.Columns.Add("Balance", typeof(decimal));
                dt.Columns.Add("Types", typeof(string));
                dt.Columns.Add("HeadWiseNarration", typeof(string));
                dt.Columns.Add("Inv No", typeof(string));
                dt.Columns.Add("Inv Date", typeof(string));
                dt.Columns.Add("Doc Entry Id", typeof(int));
                dt.Columns.Add("SumDet", typeof(string));
                dt.Columns.Add("VCHEMark", typeof(string));
                dt.Columns.Add("AccountCode", typeof(int));
                dt.Columns.Add("ReportType", typeof(string));
                dt.Columns.Add("Vch No", typeof(string));
                dt.Columns.Add("AccEntryId", typeof(int));
                dt.Columns.Add("AccEntry YearCode", typeof(int));

                sr = 1;
                foreach (var row in model.TransactionLedgerGrid)
                {
                    dt.Rows.Add(
                        sr++,
                        row.VoucherDocDate,
                        row.Particulars,
                        row.VoucherType,
                        row.InvoiceVoucherNo,
                        row.DrAmt,
                        row.CrAmt,
                        row.Balance,
                        row.Types,
                        row.HeadWiseNarration,
                        row.INVNo,
                        row.BillDate,
                        row.DocEntryId,
                        row.SumDet,
                        row.VCHEMark,
                        row.AccountCode,
                        row.ReportType,
                        row.VchNo,
                        row.AccEntryId,
                        row.AccEntryYearCode
                    );
                }

                sheetName = "TransactionLedgerDetail";
            }
            else if (ReportType == "MonthlySummary")
            {
                dt.Columns.Add("Sr#", typeof(int));
                dt.Columns.Add("MonthFullName", typeof(string));
                dt.Columns.Add("Total DR", typeof(decimal));
                dt.Columns.Add("Total CR", typeof(decimal));
                dt.Columns.Add("Closing Amt", typeof(decimal));
                dt.Columns.Add("Dr/Cr", typeof(string));
                dt.Columns.Add("YearCode", typeof(string));
                dt.Columns.Add("SeqNo", typeof(int));

                foreach (var row in model.TransactionLedgerGrid)
                {
                    dt.Rows.Add(
                        sr++,
                        row.MonthFullName,
                        row.TotalDr,
                        row.TotalCr,
                        row.ClosingAmt,
                        row.Dr_CR,
                        row.YearCode,
                        row.SeqNo
                    );
                }
            }
            else if (ReportType == "GROUPSUMMARY" || ReportType == "GROUPDETAIL" || ReportType == "BalanceConfirmation")
            {
                dt.Columns.Add("Sr#", typeof(int));
                dt.Columns.Add("Parent Group Name", typeof(string));
                dt.Columns.Add("Account Name", typeof(string));
                dt.Columns.Add("Open DR", typeof(decimal));
                dt.Columns.Add("Open CR", typeof(decimal));
                dt.Columns.Add("Total Opening", typeof(decimal));
                dt.Columns.Add("Current DR", typeof(decimal));
                dt.Columns.Add("Current CR", typeof(decimal));
                dt.Columns.Add("Net Current Amt", typeof(decimal));
                dt.Columns.Add("Net Amt", typeof(decimal));
                dt.Columns.Add("Group/Ledger", typeof(string));

                sr = 1;
                foreach (var row in model.TransactionLedgerGrid) // assuming GroupSummary uses TransactionLedgerGrid
                {
                    dt.Rows.Add(
                        sr++,
                        row.ParentLedgerName,
                        row.AccountName,
                        row.OpnDr,
                        row.OpnCr,
                        row.TotalOpening,
                        row.CurrDrAmt,
                        row.CurrCrAmt,
                        row.NetCurrentAmt,
                        row.NetAmount,
                        row.GroupLedger == "G" ? "Group" : "Ledger"
                    );
                }

                sheetName = "GroupSummary";
            }
            else
            {
                return BadRequest("Invalid report type.");
            }

            // Generate Excel
            var stream = ExcelHelper.GenerateExcel(
                dt,
                sheetName,
                CompanyName,
                BranchName,
                FromDate,
                ToDate
            );

            string excelName = $"{sheetName}.xlsx";

            // Force ASCII-safe header
            Response.Headers.Add("Content-Disposition", $"attachment; filename=\"{excelName}\"");

            return File(
                stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
            );
        }
    }
}
