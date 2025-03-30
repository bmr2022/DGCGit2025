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
    public class BankPaymentBLL:IBankPayment
    {
        private BankPaymentDAL _BankPaymentDAL;
        private readonly IDataLogic _DataLogicDAL;

        public BankPaymentBLL(IConfiguration config, IDataLogic dataLogicDAL)
        {
            _BankPaymentDAL = new BankPaymentDAL(config, dataLogicDAL);
            _DataLogicDAL = dataLogicDAL;
        }
        public async Task<ResponseResult> FillLedgerName(string VoucherType, string Type)
        {
            return await _BankPaymentDAL.FillLedgerName(VoucherType, Type);
        }
        public async Task<ResponseResult> FillSubVoucherName(string VoucherType)
        {
            return await _BankPaymentDAL.FillSubVoucherName(VoucherType);
        }
        public async Task<ResponseResult> FillBankType(int AccountCode)
        {
            return await _BankPaymentDAL.FillBankType(AccountCode);
        }
        public async Task<ResponseResult> FillIntrument(string VoucherType)
        {
            return await _BankPaymentDAL.FillIntrument(VoucherType);
        }
        public async Task<ResponseResult> FillModeofAdjust(string VoucherType)
        {
            return await _BankPaymentDAL.FillModeofAdjust(VoucherType);
        }
        public async Task<ResponseResult> FillCostCenterName()
        {
            return await _BankPaymentDAL.FillCostCenterName();
        }
        public async Task<ResponseResult> FillEntryID(int YearCode, string VoucherDate)
        {
            return await _BankPaymentDAL.FillEntryID(YearCode, VoucherDate);
        }
        public async Task<ResponseResult> FillCurrency()
        {
            return await _BankPaymentDAL.FillCurrency();
        }
        public async Task<ResponseResult> FillPONO( string accountcode, string VoucherDate)
        {
            return await _BankPaymentDAL.FillPONO( accountcode, VoucherDate);
        }
        public async Task<ResponseResult> GetPODetail(int PONO,string accountcode, string VoucherDate)
        {
            return await _BankPaymentDAL.GetPODetail(PONO, accountcode,  VoucherDate);
        }
        public async Task<ResponseResult> GetPODate(int PONO, string accountcode, string VoucherDate,string POYearCode)
        {
            return await _BankPaymentDAL.GetPODate(PONO, accountcode, VoucherDate, POYearCode);
        }
        public async Task<ResponseResult> SaveBankPayment(BankPaymentModel model, DataTable GIGrid)
        {
            return await _BankPaymentDAL.SaveBankPayment(model, GIGrid);
        }
        public async Task<ResponseResult> GetLedgerBalance(int OpeningYearCode, int AccountCode, string VoucherDate)
        {
            return await _BankPaymentDAL.GetLedgerBalance(OpeningYearCode, AccountCode, VoucherDate);
            //GetLedgerBalance
            //return await _BankReceiptDAL.GetOpeningAmt(OpeningYearCode, AccountCode);
        }
        public async Task<ResponseResult> GetDashBoardData(string FromDate, string ToDate)
        {
            return await _BankPaymentDAL.GetDashBoardData(FromDate, ToDate);
        }
        public async Task<BankPaymentModel> GetDashBoardDetailData(string FromDate, string ToDate)
        {
            return await _BankPaymentDAL.GetDashBoardDetailData(FromDate, ToDate);
        }
        public async Task<BankPaymentModel> GetDashBoardSummaryData(string FromDate, string ToDate)
        {
            return await _BankPaymentDAL.GetDashBoardSummaryData(FromDate, ToDate);
        }
        public async Task<ResponseResult> DeleteByID(int ID, int YearCode, int ActualEntryBy, string EntryByMachine, string ActualEntryDate)
        {
            return await _BankPaymentDAL.DeleteByID(ID, YearCode, ActualEntryBy, EntryByMachine, ActualEntryDate);
        }
        public async Task<BankPaymentModel> PopUpForPendingVouchers(PopUpDataTableAgainstRef DataTable)
        {
            return await _BankPaymentDAL.PopUpForPendingVouchers(DataTable);
        }
        public async Task<BankPaymentModel> GetViewByID(int ID, int YearCode, string VoucherNo)
        {
            return await _BankPaymentDAL.GetViewByID(ID, YearCode, VoucherNo);

        }
        public async Task<ResponseResult> CheckAmountBeforeSave(string VoucherDate, int YearCode, int AgainstVoucherYearCode, int AgainstVoucherEntryId, string AgainstVoucherNo, int AccountCode)
        {
            return await _BankPaymentDAL.CheckAmountBeforeSave(VoucherDate, YearCode, AgainstVoucherYearCode, AgainstVoucherEntryId, AgainstVoucherNo, AccountCode);

        }
    }
}
