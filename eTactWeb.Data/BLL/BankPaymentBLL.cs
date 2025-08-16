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
    public class BankPaymentBLL:IBankPayment
    {
        private BankPaymentDAL _BankPaymentDAL;
        private readonly IDataLogic _DataLogicDAL;

        public BankPaymentBLL(IConfiguration config, IDataLogic dataLogicDAL, ConnectionStringService connectionStringService)
        {
            _BankPaymentDAL = new BankPaymentDAL(config, dataLogicDAL, connectionStringService);
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
        public async Task<ResponseResult> GetFormRights(int ID)
        {
            return await _BankPaymentDAL.GetFormRights(ID);
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
        public async Task<ResponseResult> GetPOYearCode(string PONO,string accountcode, string VoucherDate)
        {
            return await _BankPaymentDAL.GetPOYearCode(PONO, accountcode,  VoucherDate);
        }
        public async Task<ResponseResult> GetPODate(string PONO, string accountcode, string VoucherDate,string POYearCode)
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
        public async Task<BankPaymentModel> GetDashBoardDetailData(string FromDate, string ToDate, string LedgerName,string Bank, string VoucherNo, string AgainstVoucherNo, string PoNo, string AgainstBillno)
        {
            return await _BankPaymentDAL.GetDashBoardDetailData(FromDate, ToDate,LedgerName,Bank,VoucherNo, AgainstVoucherNo, PoNo,AgainstBillno);
        }
        public async Task<BankPaymentModel> GetDashBoardSummaryData(string FromDate, string ToDate, string LedgerName, string Bank,string VoucherNo, string AgainstVoucherNo, string PoNo,string AgainstBillno)
        {
            return await _BankPaymentDAL.GetDashBoardSummaryData(FromDate, ToDate, LedgerName,Bank, VoucherNo, AgainstVoucherNo, PoNo,AgainstBillno);
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
        public async Task<ResponseResult> FillLedgerInDashboard(string FromDate, string ToDate, string VoucherType)
        {
            return await _BankPaymentDAL.FillLedgerInDashboard(FromDate, ToDate, VoucherType);
        }
        public async Task<ResponseResult> FillBankInDashboard(string FromDate, string ToDate, string VoucherType)
        {
            return await _BankPaymentDAL.FillBankInDashboard(FromDate, ToDate, VoucherType);
        }
        public async Task<ResponseResult> FillVoucherNoInDashboard(string FromDate, string ToDate, string VoucherType)
        {
            return await _BankPaymentDAL.FillVoucherNoInDashboard(FromDate, ToDate, VoucherType);
        }
        public async Task<ResponseResult> FillAgainstVoucherRefNoInDashboard(string FromDate, string ToDate, string VoucherType)
        {
            return await _BankPaymentDAL.FillAgainstVoucherRefNoInDashboard(FromDate, ToDate, VoucherType);
        }
        public async Task<ResponseResult> FillAgainstVoucherNoInDashboard(string FromDate, string ToDate, string VoucherType)
        {
            return await _BankPaymentDAL.FillAgainstVoucherNoInDashboard(FromDate, ToDate, VoucherType);
        }
    }
}
