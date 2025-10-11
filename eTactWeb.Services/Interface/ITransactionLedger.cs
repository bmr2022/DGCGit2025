using eTactWeb.DOM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Services.Interface
{
    public interface ITransactionLedger
    {
        Task<ResponseResult> GetLedgerName();
        Task<ResponseResult> FillLedgerName();
        public Task<TransactionLedgerModel> GetDetailsData(string FromDate, string ToDate, string ReportType, string GroupOrLedger, int? ParentAccountCode, int AccountCode, string VoucherType, string VoucherNo, string InvoiceNo, string Narration, float? Amount, string? DR, string? CR, string Ledger);
        public Task<TransactionLedgerModel> GetTransactionLedgerMonthlySummaryDetailsData(string FromentryDate,string ToEntryDate,int AccountCode);
        Task<TransactionLedgerModel> GetTransactionLedgerGroupSummaryDetailsData(string FromDate, string ToDate, string ReportType, int LedgerGroup, int AccountCode, string VoucherType);
		public Task<ResponseResult> FillVoucherName();
    }
}
