using eTactWeb.Data.Common;
using eTactWeb.Data.DAL;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Data.BLL
{
    public class TransactionLedgerBLL : ITransactionLedger

    {
        private TransactionLedgerDAL _TransactionLedgerDAL;
        private readonly IDataLogic _DataLogicDAL;
            
        public TransactionLedgerBLL(IConfiguration config, IDataLogic dataLogicDAL, ConnectionStringService connectionStringService)
        {
            _TransactionLedgerDAL = new TransactionLedgerDAL(config, dataLogicDAL, connectionStringService);
            _DataLogicDAL = dataLogicDAL;
        }
        public async Task<ResponseResult> GetLedgerName(int? ParentAccountCode)
        {
            return await _TransactionLedgerDAL.GetLedgerName(ParentAccountCode);
        }
        public async Task<ResponseResult> FillLedgerName()
        {
            return await _TransactionLedgerDAL.FillLedgerName();
        }
        public async Task<TransactionLedgerModel> GetDetailsData(string FromDate, string ToDate, string ReportType, string GroupOrLedger, int? ParentAccountCode, int? AccountCode, string VoucherType, string VoucherNo, string InvoiceNo, string Narration, float? Amount, string? DR, string? CR, string Ledger)
        {
            return await _TransactionLedgerDAL.GetDetailsData(FromDate, ToDate, ReportType, GroupOrLedger, ParentAccountCode,AccountCode, VoucherType, VoucherNo, InvoiceNo, Narration, Amount,DR,CR,Ledger);
        }
        public async Task<TransactionLedgerModel> GetTransactionLedgerMonthlySummaryDetailsData(string FromentryDate, string ToEntryDate, int AccountCode)
        {
            return await _TransactionLedgerDAL.GetTransactionLedgerMonthlySummaryDetailsData( FromentryDate,  ToEntryDate,  AccountCode);
        }
		public async Task<TransactionLedgerModel> GetTransactionLedgerGroupSummaryDetailsData(string FromDate, string ToDate, string ReportType, string GroupOrLedger, int? ParentAccountCode = null, int AccountCode = 0, string? VoucherType = null, string? VoucherNo = null, string? InvoiceNo = null, string? Narration = null, float? Amount = null, string? DR = null, string? CR = null, string? Ledger = null)
		{
			return await _TransactionLedgerDAL.GetTransactionLedgerGroupSummaryDetailsData(FromDate, ToDate, ReportType, GroupOrLedger, ParentAccountCode, AccountCode,VoucherType, VoucherNo, InvoiceNo, Narration, Amount, DR, CR, Ledger);
		}
		public async Task<ResponseResult> FillVoucherName()
        {
            return await _TransactionLedgerDAL.FillVoucherName();
        }
    }
}
