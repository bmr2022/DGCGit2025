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
        public async Task<ResponseResult> GetLedgerName()
        {
            return await _TransactionLedgerDAL.GetLedgerName();
        }
        public async Task<ResponseResult> FillLedgerName()
        {
            return await _TransactionLedgerDAL.FillLedgerName();
        }
        public async Task<TransactionLedgerModel> GetDetailsData(string FromDate, string ToDate, int AccountCode,string ReportType, int Ledger, string VoucherType)
        {
            return await _TransactionLedgerDAL.GetDetailsData(FromDate, ToDate, AccountCode, ReportType, Ledger,VoucherType);
        }
        public async Task<TransactionLedgerModel> GetTransactionLedgerMonthlySummaryDetailsData(string FromentryDate, string ToEntryDate, int AccountCode)
        {
            return await _TransactionLedgerDAL.GetTransactionLedgerMonthlySummaryDetailsData( FromentryDate,  ToEntryDate,  AccountCode);
        }
		public async Task<TransactionLedgerModel> GetTransactionLedgerGroupSummaryDetailsData(string FromDate, string ToDate, string ReportType, int LedgerGroup, int AccountCode, string VoucherType)
		{
			return await _TransactionLedgerDAL.GetTransactionLedgerGroupSummaryDetailsData(FromDate, ToDate, ReportType, LedgerGroup, AccountCode,VoucherType);
		}
		public async Task<ResponseResult> FillVoucherName()
        {
            return await _TransactionLedgerDAL.FillVoucherName();
        }
    }
}
