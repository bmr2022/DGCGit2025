using ClosedXML.Excel;
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
    public class BankReceiptBLL : IBankReceipt
    {
        private BankReceiptDAL _BankReceiptDAL;
        private readonly IDataLogic _DataLogicDAL;
        private readonly ICommon _common;
        public BankReceiptBLL(IConfiguration config, IDataLogic dataLogicDAL, ConnectionStringService connectionStringService, ICommon common)
        {
            _BankReceiptDAL = new BankReceiptDAL(config, dataLogicDAL, connectionStringService, common);
            _DataLogicDAL = dataLogicDAL;
            _common = common;
        }
        public async Task<ResponseResult> FillLedgerName(string VoucherType, string Type)
        {
            return await _BankReceiptDAL.FillLedgerName(VoucherType, Type);
        }
        public async Task<ResponseResult> FillSubVoucherName(string VoucherType)
        {
            return await _BankReceiptDAL.FillSubVoucherName(VoucherType);
        }
        public async Task<ResponseResult> FillBankType(int AccountCode)
        {
            return await _BankReceiptDAL.FillBankType(AccountCode);
        }
        public async Task<ResponseResult> FillIntrument(string VoucherType)
        {
            return await _BankReceiptDAL.FillIntrument(VoucherType);
        }
        public async Task<ResponseResult> GetFormRights(int ID)
        {
            return await _BankReceiptDAL.GetFormRights(ID);
        }

        public async Task<ResponseResult> FillModeofAdjust(string VoucherType)
        {
            return await _BankReceiptDAL.FillModeofAdjust(VoucherType);
        }
        public async Task<ResponseResult> FillCostCenterName()
        {
            return await _BankReceiptDAL.FillCostCenterName();
        }
        public async Task<ResponseResult> FillEntryID(int YearCode, string VoucherDate)
        {
            return await _BankReceiptDAL.FillEntryID(YearCode, VoucherDate);
        }
        public async Task<ResponseResult> FillCurrency()
        {
            return await _BankReceiptDAL.FillCurrency();
        }
        public async Task<ResponseResult> SaveBankReceipt(BankReceiptModel model, DataTable GIGrid)
        {
            return await _BankReceiptDAL.SaveBankReceipt(model, GIGrid);
        }
        public async Task<ResponseResult> GetLedgerBalance(int OpeningYearCode, int AccountCode, string VoucherDate)
        {
            return await _BankReceiptDAL.GetLedgerBalance(OpeningYearCode, AccountCode, VoucherDate);
            //GetLedgerBalance
            //return await _BankReceiptDAL.GetOpeningAmt(OpeningYearCode, AccountCode);
        }
        public async Task<ResponseResult> GetDashBoardData(string summaryDetail, string FromDate, string ToDate, string LedgerName, string Bank, string VoucherNo, string AgainstVoucherNo, string SoNo, string AgainstBillno)
        {
            return await _BankReceiptDAL.GetDashBoardData(summaryDetail, FromDate, ToDate, LedgerName, Bank, VoucherNo, AgainstVoucherNo, SoNo, AgainstBillno);
        }
        public async Task<BankReceiptModel> GetDashBoardDetailData(string FromDate, string ToDate, string LedgerName, string Bank, string VoucherNo, string AgainstVoucherNo, string SoNo, string AgainstBillno)
        {
            return await _BankReceiptDAL.GetDashBoardDetailData(FromDate, ToDate, LedgerName, Bank, VoucherNo, AgainstVoucherNo, SoNo, AgainstBillno);
        }
        public async Task<BankReceiptModel> GetDashBoardSummaryData(string FromDate, string ToDate, string LedgerName, string Bank, string VoucherNo, string AgainstVoucherNo, string SoNo, string AgainstBillno)
        {
            return await _BankReceiptDAL.GetDashBoardSummaryData(FromDate, ToDate, LedgerName, Bank, VoucherNo, AgainstVoucherNo, SoNo, AgainstBillno);
        }

        public async Task<ResponseResult> DeleteByID(int ID, int YearCode, int ActualEntryBy, string EntryByMachine, string ActualEntryDate, string VoucherType)
        {
            return await _BankReceiptDAL.DeleteByID(ID, YearCode, ActualEntryBy, EntryByMachine, ActualEntryDate, VoucherType);
        }
        public async Task<BankReceiptModel> PopUpForPendingVouchers(PopUpDataTableAgainstRef DataTable)
        {
            return await _BankReceiptDAL.PopUpForPendingVouchers(DataTable);
        }
        public async Task<BankReceiptModel> GetViewByID(int ID, int YearCode, string VoucherNo)
        {
            return await _BankReceiptDAL.GetViewByID(ID, YearCode, VoucherNo);

        }
        public async Task<ResponseResult> CheckAmountBeforeSave(string VoucherDate, int YearCode, int AgainstVoucherYearCode, int AgainstVoucherEntryId, string AgainstVoucherNo, int AccountCode)
        {
            return await _BankReceiptDAL.CheckAmountBeforeSave(VoucherDate, YearCode, AgainstVoucherYearCode, AgainstVoucherEntryId, AgainstVoucherNo, AccountCode);

        }
        public async Task<ResponseResult> FillSONO(string accountcode, string VoucherDate)
        {
            return await _BankReceiptDAL.FillSONO(accountcode, VoucherDate);
        }
        public async Task<ResponseResult> GetSoYearCode(int SONO, string accountcode, string VoucherDate)
        {
            return await _BankReceiptDAL.GetSoYearCode(SONO, accountcode, VoucherDate);
        }
        public async Task<ResponseResult> GetSODate(int SONO, string accountcode, string VoucherDate, string SOYearCode)
        {
            return await _BankReceiptDAL.GetSODate(SONO, accountcode, VoucherDate, SOYearCode);
        }
        public async Task<ResponseResult> FillLedgerInDashboard(string FromDate, string ToDate, string VoucherType)
        {
            return await _BankReceiptDAL.FillLedgerInDashboard(FromDate, ToDate, VoucherType);
        }
        public async Task<ResponseResult> FillBankInDashboard(string FromDate, string ToDate, string VoucherType)
        {
            return await _BankReceiptDAL.FillBankInDashboard(FromDate, ToDate, VoucherType);
        }
        public async Task<ResponseResult> FillVoucherNoInDashboard(string FromDate, string ToDate, string VoucherType)
        {
            return await _BankReceiptDAL.FillVoucherNoInDashboard(FromDate, ToDate, VoucherType);
        }
        public async Task<ResponseResult> FillAgainstVoucherRefNoInDashboard(string FromDate, string ToDate, string VoucherType)
        {
            return await _BankReceiptDAL.FillAgainstVoucherRefNoInDashboard(FromDate, ToDate, VoucherType);
        }
        public async Task<ResponseResult> FillAgainstVoucherNoInDashboard(string FromDate, string ToDate, string VoucherType)
        {
            return await _BankReceiptDAL.FillAgainstVoucherNoInDashboard(FromDate, ToDate, VoucherType);
        }
        public async Task<ResponseResult> FillAgainstBillNoInDashboard(string FromDate, string ToDate, string VoucherType)
        {
            return await _BankReceiptDAL.FillAgainstBillNoInDashboard(FromDate, ToDate, VoucherType);
        }
        public async Task<ResponseResult> FillSoNoInDashboard(string FromDate, string ToDate, string VoucherType)
        {
            return await _BankReceiptDAL.FillSoNoInDashboard(FromDate, ToDate, VoucherType);
        }


        public async Task<ResponseResult> BackDateEntryAllowed()
        {
            return await _BankReceiptDAL.BackDateEntryAllowed();
        }
    }
}
