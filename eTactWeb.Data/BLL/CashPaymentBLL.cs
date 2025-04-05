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
    public class CashPaymentBLL : ICashPayment
    {
        private CashPaymentDAL _CashPaymentDAL;
        private readonly IDataLogic _DataLogicDAL;

        public CashPaymentBLL(IConfiguration config, IDataLogic dataLogicDAL, ConnectionStringService connectionStringService)
        {
            _CashPaymentDAL = new CashPaymentDAL(config, dataLogicDAL, connectionStringService);
            _DataLogicDAL = dataLogicDAL;
        }
        public async Task<ResponseResult> FillLedgerName(string VoucherType, string Type)
        {
            return await _CashPaymentDAL.FillLedgerName(VoucherType, Type);
        }
        public async Task<ResponseResult> FillSubVoucherName(string VoucherType)
        {
            return await _CashPaymentDAL.FillSubVoucherName(VoucherType);
        }
        public async Task<ResponseResult> FillBankType(int AccountCode)
        {
            return await _CashPaymentDAL.FillBankType(AccountCode);
        }
        public async Task<ResponseResult> FillIntrument(string VoucherType)
        {
            return await _CashPaymentDAL.FillIntrument(VoucherType);
        }
        public async Task<ResponseResult> FillModeofAdjust(string VoucherType)
        {
            return await _CashPaymentDAL.FillModeofAdjust(VoucherType);
        }
        public async Task<ResponseResult> FillCostCenterName()
        {
            return await _CashPaymentDAL.FillCostCenterName();
        }
        public async Task<ResponseResult> FillEntryID(int YearCode, string VoucherDate)
        {
            return await _CashPaymentDAL.FillEntryID(YearCode, VoucherDate);
        }
        public async Task<ResponseResult> FillCurrency()
        {
            return await _CashPaymentDAL.FillCurrency();
        }
        public async Task<ResponseResult> FillPONO(string accountcode, string VoucherDate)
        {
            return await _CashPaymentDAL.FillPONO(accountcode, VoucherDate);
        }
        public async Task<ResponseResult> GetPODetail(int PONO, string accountcode, string VoucherDate)
        {
            return await _CashPaymentDAL.GetPODetail(PONO, accountcode, VoucherDate);
        }
        public async Task<ResponseResult> GetPODate(int PONO, string accountcode, string VoucherDate, string POYearCode)
        {
            return await _CashPaymentDAL.GetPODate(PONO, accountcode, VoucherDate, POYearCode);
        }
        public async Task<ResponseResult> SaveCashPayment(CashPaymentModel model, DataTable GIGrid)
        {
            return await _CashPaymentDAL.SaveCashPayment(model, GIGrid);
        }
        public async Task<ResponseResult> GetLedgerBalance(int OpeningYearCode, int AccountCode, string VoucherDate)
        {
            return await _CashPaymentDAL.GetLedgerBalance(OpeningYearCode, AccountCode, VoucherDate);
            //GetLedgerBalance
            //return await _BankReceiptDAL.GetOpeningAmt(OpeningYearCode, AccountCode);
        }
        public async Task<ResponseResult> GetDashBoardData(string FromDate, string ToDate)
        {
            return await _CashPaymentDAL.GetDashBoardData(FromDate, ToDate);
        }
        public async Task<CashPaymentModel> GetDashBoardDetailData(string FromDate, string ToDate)
        {
            return await _CashPaymentDAL.GetDashBoardDetailData(FromDate, ToDate);
        }
        public async Task<CashPaymentModel> GetDashBoardSummaryData(string FromDate, string ToDate)
        {
            return await _CashPaymentDAL.GetDashBoardSummaryData(FromDate, ToDate);
        }
        public async Task<ResponseResult> DeleteByID(int ID, int YearCode, int ActualEntryBy, string EntryByMachine, string ActualEntryDate)
        {
            return await _CashPaymentDAL.DeleteByID(ID, YearCode, ActualEntryBy, EntryByMachine, ActualEntryDate);
        }
        public async Task<CashPaymentModel> PopUpForPendingVouchers(PopUpDataTableAgainstRef DataTable)
        {
            return await _CashPaymentDAL.PopUpForPendingVouchers(DataTable);
        }
        public async Task<CashPaymentModel> GetViewByID(int ID, int YearCode, string VoucherNo)
        {
            return await _CashPaymentDAL.GetViewByID(ID, YearCode, VoucherNo);

        }
        public async Task<ResponseResult> CheckAmountBeforeSave(string VoucherDate, int YearCode, int AgainstVoucherYearCode, int AgainstVoucherEntryId, string AgainstVoucherNo, int AccountCode)
        {
            return await _CashPaymentDAL.CheckAmountBeforeSave(VoucherDate, YearCode, AgainstVoucherYearCode, AgainstVoucherEntryId, AgainstVoucherNo, AccountCode);

        }
    }
}
