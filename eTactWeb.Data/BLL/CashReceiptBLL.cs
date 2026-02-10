using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eTactWeb.Data.Common;
using eTactWeb.Data.DAL;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.Extensions.Configuration;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Data.BLL
{
    public class CashReceiptBLL : ICashReceipt
    {
        private CashReceiptDAL _CashReceiptDAL;
        private readonly IDataLogic _DataLogicDAL;


        public CashReceiptBLL(IConfiguration config, IDataLogic dataLogicDAL, ConnectionStringService connectionStringService, ICommon common)
        {
            _CashReceiptDAL = new CashReceiptDAL(config, dataLogicDAL, connectionStringService, common);
            _DataLogicDAL = dataLogicDAL;
        }
        public async Task<ResponseResult> FillLedgerName(string VoucherType, string Type)
        {
            return await _CashReceiptDAL.FillLedgerName(VoucherType, Type);
        }
        public async Task<ResponseResult> FillSubVoucherName(string VoucherType)
        {
            return await _CashReceiptDAL.FillSubVoucherName(VoucherType);
        }
        public async Task<ResponseResult> FillBankType(int AccountCode)
        {
            return await _CashReceiptDAL.FillBankType(AccountCode);
        }
        public async Task<ResponseResult> GetFormRights(int ID)
        {
            return await _CashReceiptDAL.GetFormRights(ID);
        }
        public async Task<ResponseResult> FillIntrument(string VoucherType)
        {
            return await _CashReceiptDAL.FillIntrument(VoucherType);
        }
        public async Task<ResponseResult> FillModeofAdjust(string VoucherType)
        {
            return await _CashReceiptDAL.FillModeofAdjust(VoucherType);
        }
        public async Task<ResponseResult> FillCostCenterName()
        {
            return await _CashReceiptDAL.FillCostCenterName();
        }
        public async Task<ResponseResult> FillEntryID(int YearCode, string VoucherDate)
        {
            return await _CashReceiptDAL.FillEntryID(YearCode, VoucherDate);
        }
        public async Task<ResponseResult> FillCurrency()
        {
            return await _CashReceiptDAL.FillCurrency();
        }
        public async Task<ResponseResult> SaveCashReceipt(CashReceiptModel model, DataTable GIGrid)
        {
            return await _CashReceiptDAL.SaveCashReceipt(model, GIGrid);
        }
        public async Task<ResponseResult> GetLedgerBalance(int OpeningYearCode, int AccountCode, string VoucherDate)
        {
            return await _CashReceiptDAL.GetLedgerBalance(OpeningYearCode, AccountCode, VoucherDate);
        }
        //public async Task<ResponseResult> GetDashBoardData(string FromDate, string ToDate)
        //{
        //    return await _CashReceiptDAL.GetDashBoardData(FromDate, ToDate);
        //}
        public async Task<ResponseResult> GetDashBoardData(string summaryDetail, string FromDate, string ToDate, string LedgerName, string Bank, string VoucherNo, string AgainstVoucherNo, string SoNo, string AgainstBillno)
        {
            return await _CashReceiptDAL.GetDashBoardData(summaryDetail, FromDate, ToDate, LedgerName, Bank, VoucherNo, AgainstVoucherNo, SoNo, AgainstBillno);
        }
        public async Task<CashReceiptModel> GetDashBoardDetailData(string FromDate, string ToDate, string LedgerName, string Bank, string VoucherNo, string AgainstVoucherNo, string SoNo, string AgainstBillno)
        {
            return await _CashReceiptDAL.GetDashBoardDetailData(FromDate, ToDate, LedgerName, Bank, VoucherNo, AgainstVoucherNo, SoNo, AgainstBillno);
        }
        public async Task<CashReceiptModel> GetDashBoardSummaryData(string FromDate, string ToDate, string LedgerName, string Bank, string VoucherNo, string AgainstVoucherNo, string SoNo, string AgainstBillno)
        {
            return await _CashReceiptDAL.GetDashBoardSummaryData(FromDate, ToDate, LedgerName, Bank, VoucherNo, AgainstVoucherNo, SoNo, AgainstBillno);
        }
        public async Task<ResponseResult> DeleteByID(int ID, int YearCode, int ActualEntryBy, string EntryByMachine, string ActualEntryDate, string VoucherType)
        {
            return await _CashReceiptDAL.DeleteByID(ID, YearCode, ActualEntryBy, EntryByMachine, ActualEntryDate, VoucherType);
        }
        public async Task<CashReceiptModel> PopUpForPendingVouchers(PopUpDataTableAgainstRef DataTable)
        {
            return await _CashReceiptDAL.PopUpForPendingVouchers(DataTable);
        }
        public async Task<CashReceiptModel> GetViewByID(int ID, int YearCode, string VoucherNo)
        {
            return await _CashReceiptDAL.GetViewByID(ID, YearCode, VoucherNo);
        }
        public async Task<ResponseResult> CheckAmountBeforeSave(string VoucherDate, int YearCode, int AgainstVoucherYearCode, int AgainstVoucherEntryId, string AgainstVoucherNo, int AccountCode)
        {
            return await _CashReceiptDAL.CheckAmountBeforeSave(VoucherDate, YearCode, AgainstVoucherYearCode, AgainstVoucherEntryId, AgainstVoucherNo, AccountCode);
        }
        public async Task<ResponseResult> FillSONO(string accountcode, string VoucherDate)
        {
            return await _CashReceiptDAL.FillSONO(accountcode, VoucherDate);
        }
        public async Task<ResponseResult> GetSoYearCode(int SONO, string accountcode, string VoucherDate)
        {
            return await _CashReceiptDAL.GetSoYearCode(SONO, accountcode, VoucherDate);
        }
        public async Task<ResponseResult> GetSODate(int SONO, string accountcode, string VoucherDate, string SOYearCode)
        {
            return await _CashReceiptDAL.GetSODate(SONO, accountcode, VoucherDate, SOYearCode);
        }
        public async Task<ResponseResult> FillLedgerInDashboard(string FromDate, string ToDate, string VoucherType)
        {
            return await _CashReceiptDAL.FillLedgerInDashboard(FromDate, ToDate, VoucherType);
        }
        public async Task<ResponseResult> FillBankInDashboard(string FromDate, string ToDate, string VoucherType)
        {
            return await _CashReceiptDAL.FillBankInDashboard(FromDate, ToDate, VoucherType);
        }
        public async Task<ResponseResult> FillVoucherNoInDashboard(string FromDate, string ToDate, string VoucherType)
        {
            return await _CashReceiptDAL.FillVoucherNoInDashboard(FromDate, ToDate, VoucherType);
        }
        public async Task<ResponseResult> FillAgainstVoucherRefNoInDashboard(string FromDate, string ToDate, string VoucherType)
        {
            return await _CashReceiptDAL.FillAgainstVoucherRefNoInDashboard(FromDate, ToDate, VoucherType);
        }
        public async Task<ResponseResult> FillAgainstVoucherNoInDashboard(string FromDate, string ToDate, string VoucherType)
        {
            return await _CashReceiptDAL.FillAgainstVoucherNoInDashboard(FromDate, ToDate, VoucherType);
        }
    }
}
