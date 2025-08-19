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
        public Task<TransactionLedgerModel> GetDetailsData(string FromDate, string ToDate, int AccountCode,string ReportType, int Ledger, string VoucherType);
        public Task<TransactionLedgerModel> GetTransactionLedgerMonthlySummaryDetailsData(string FromentryDate,string ToEntryDate,int AccountCode);
        Task<TransactionLedgerModel> GetTransactionLedgerGroupSummaryDetailsData(string FromDate, string ToDate, string ReportType, int LedgerGroup, int AccountCode, string VoucherType);
		public Task<ResponseResult> FillVoucherName();
    }
}
