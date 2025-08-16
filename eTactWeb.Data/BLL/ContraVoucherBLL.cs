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
    
     public class ContraVoucherBLL : IContraVoucher
    {
        private ContraVoucherDAL _ContraVoucherDAL;
        private readonly IDataLogic _DataLogicDAL;

        public ContraVoucherBLL(IConfiguration config, IDataLogic dataLogicDAL, ConnectionStringService connectionStringService)
        {
            _ContraVoucherDAL = new ContraVoucherDAL(config, dataLogicDAL, connectionStringService);
            _DataLogicDAL = dataLogicDAL;
        }
        public async Task<ResponseResult> GetFormRights(int ID)
        {
            return await _ContraVoucherDAL.GetFormRights(ID);
        }
        public async Task<ResponseResult> FillLedgerName(string VoucherType, string Type)
        {
            return await _ContraVoucherDAL.FillLedgerName(VoucherType, Type);
        }
        public async Task<ResponseResult> FillSubVoucherName(string VoucherType)
        {
            return await _ContraVoucherDAL.FillSubVoucherName(VoucherType);
        }
        public async Task<ResponseResult> FillBankType(int AccountCode)
        {
            return await _ContraVoucherDAL.FillBankType(AccountCode);
        }
        public async Task<ResponseResult> FillIntrument(string VoucherType)
        {
            return await _ContraVoucherDAL.FillIntrument(VoucherType);
        }
        public async Task<ResponseResult> FillModeofAdjust(string VoucherType)
        {
            return await _ContraVoucherDAL.FillModeofAdjust(VoucherType);
        }
        public async Task<ResponseResult> FillCostCenterName()
        {
            return await _ContraVoucherDAL.FillCostCenterName();
        }
        public async Task<ResponseResult> FillEntryID(int YearCode, string VoucherDate)
        {
            return await _ContraVoucherDAL.FillEntryID(YearCode, VoucherDate);
        }
        public async Task<ResponseResult> FillCurrency()
        {
            return await _ContraVoucherDAL.FillCurrency();
        }
     
        public async Task<ResponseResult> GetLedgerBalance(int OpeningYearCode, int AccountCode, string VoucherDate)
        {
            return await _ContraVoucherDAL.GetLedgerBalance(OpeningYearCode, AccountCode, VoucherDate);
            //GetLedgerBalance
            //return await _BankReceiptDAL.GetOpeningAmt(OpeningYearCode, AccountCode);
        }
        public async Task<ResponseResult> GetDashBoardData(string FromDate, string ToDate)
        {
            return await _ContraVoucherDAL.GetDashBoardData(FromDate, ToDate);
        }
        public async Task<ContraVoucherModel> GetDashBoardDetailData(string FromDate, string ToDate, string LedgerName, string Bank, string VoucherNo)
        {
            return await _ContraVoucherDAL.GetDashBoardDetailData(FromDate, ToDate, LedgerName, Bank, VoucherNo);
        }
        public async Task<ContraVoucherModel> GetDashBoardSummaryData(string FromDate, string ToDate, string LedgerName, string Bank, string VoucherNo)
        {
            return await _ContraVoucherDAL.GetDashBoardSummaryData(FromDate, ToDate, LedgerName, Bank, VoucherNo);
        }
        public async Task<ResponseResult> DeleteByID(int ID, int YearCode, int ActualEntryBy, string EntryByMachine, string ActualEntryDate, string VoucherType)
        {
            return await _ContraVoucherDAL.DeleteByID(ID, YearCode, ActualEntryBy, EntryByMachine, ActualEntryDate, VoucherType);
        }
        public async Task<ContraVoucherModel> PopUpForPendingVouchers(PopUpDataTableAgainstRef DataTable)
        {
            return await _ContraVoucherDAL.PopUpForPendingVouchers(DataTable);
        }
        public async Task<ContraVoucherModel> GetViewByID(int ID, int YearCode, string VoucherNo)
        {
            return await _ContraVoucherDAL.GetViewByID(ID, YearCode, VoucherNo);

        }
        public async Task<ResponseResult> CheckAmountBeforeSave(string VoucherDate, int YearCode, int AgainstVoucherYearCode, int AgainstVoucherEntryId, string AgainstVoucherNo, int AccountCode)
        {
            return await _ContraVoucherDAL.CheckAmountBeforeSave(VoucherDate, YearCode, AgainstVoucherYearCode, AgainstVoucherEntryId, AgainstVoucherNo, AccountCode);

        }
        public async Task<ResponseResult> FillSONO(string accountcode, string VoucherDate)
        {
            return await _ContraVoucherDAL.FillSONO(accountcode, VoucherDate);
        }
        public async Task<ResponseResult> GetSoYearCode(int SONO, string accountcode, string VoucherDate)
        {
            return await _ContraVoucherDAL.GetSoYearCode(SONO, accountcode, VoucherDate);
        }
        public async Task<ResponseResult> GetSODate(int SONO, string accountcode, string VoucherDate, string SOYearCode)
        {
            return await _ContraVoucherDAL.GetSODate(SONO, accountcode, VoucherDate, SOYearCode);
        }
        public async Task<ResponseResult> FillLedgerInDashboard(string FromDate, string ToDate, string VoucherType)
        {
            return await _ContraVoucherDAL.FillLedgerInDashboard(FromDate, ToDate, VoucherType);
        }
        public async Task<ResponseResult> FillBankInDashboard(string FromDate, string ToDate, string VoucherType)
        {
            return await _ContraVoucherDAL.FillBankInDashboard(FromDate, ToDate, VoucherType);
        }
        public async Task<ResponseResult> FillVoucherNoInDashboard(string FromDate, string ToDate, string VoucherType)
        {
            return await _ContraVoucherDAL.FillVoucherNoInDashboard(FromDate, ToDate, VoucherType);
        }
        public async Task<ResponseResult> FillAgainstVoucherRefNoInDashboard(string FromDate, string ToDate, string VoucherType)
        {
            return await _ContraVoucherDAL.FillAgainstVoucherRefNoInDashboard(FromDate, ToDate, VoucherType);
        }
        public async Task<ResponseResult> FillAgainstVoucherNoInDashboard(string FromDate, string ToDate, string VoucherType)
        {
            return await _ContraVoucherDAL.FillAgainstVoucherNoInDashboard(FromDate, ToDate, VoucherType);
        }
        public async Task<ResponseResult> FillAgainstBillNoInDashboard(string FromDate, string ToDate, string VoucherType)
        {
            return await _ContraVoucherDAL.FillAgainstBillNoInDashboard(FromDate, ToDate, VoucherType);
        }
        public async Task<ResponseResult> FillSoNoInDashboard(string FromDate, string ToDate, string VoucherType)
        {
            return await _ContraVoucherDAL.FillSoNoInDashboard(FromDate, ToDate, VoucherType);
        }
        public async Task<ResponseResult> SaveContraVoucher(ContraVoucherModel model, DataTable GIGrid)
        {
            return await _ContraVoucherDAL.SaveContraVoucher(model, GIGrid);
        }
    }
}
