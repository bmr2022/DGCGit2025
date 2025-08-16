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
    public class JournalVoucherBLL : IJournalVoucher
    {
        private JournalVoucherDAL _JournalVoucherDAL;
        private readonly IDataLogic _DataLogicDAL;
        public JournalVoucherBLL(IConfiguration config, IDataLogic dataLogicDAL, ConnectionStringService connectionStringService)
        {
            _JournalVoucherDAL = new JournalVoucherDAL(config, dataLogicDAL, connectionStringService);
            _DataLogicDAL = dataLogicDAL;
        }
        public async Task<ResponseResult> FillLedgerName(string VoucherType, string Type)
        {
            return await _JournalVoucherDAL.FillLedgerName(VoucherType, Type);
        }
        public async Task<ResponseResult> GetFormRights(int ID)
        {
            return await _JournalVoucherDAL.GetFormRights(ID);
        }
        public async Task<ResponseResult> FillSubVoucherName(string VoucherType)
        {
            return await _JournalVoucherDAL.FillSubVoucherName(VoucherType);
        }
        public async Task<ResponseResult> FillBankType(int AccountCode)
        {
            return await _JournalVoucherDAL.FillBankType(AccountCode);
        }
        public async Task<ResponseResult> FillIntrument(string VoucherType)
        {
            return await _JournalVoucherDAL.FillIntrument(VoucherType);
        }
        public async Task<ResponseResult> FillModeofAdjust(string VoucherType)
        {
            return await _JournalVoucherDAL.FillModeofAdjust(VoucherType);
        }
        public async Task<ResponseResult> FillCostCenterName()
        {
            return await _JournalVoucherDAL.FillCostCenterName();
        }
        public async Task<ResponseResult> FillEntryID(int YearCode, string VoucherDate)
        {
            return await _JournalVoucherDAL.FillEntryID(YearCode, VoucherDate);
        }
        public async Task<ResponseResult> FillCurrency()
        {
            return await _JournalVoucherDAL.FillCurrency();
        }
        public async Task<ResponseResult> SaveBankReceipt(JournalVoucherModel model, DataTable GIGrid)
        {
            return await _JournalVoucherDAL.SaveBankReceipt(model, GIGrid);
        }
        public async Task<ResponseResult> GetLedgerBalance(int OpeningYearCode, int AccountCode, string VoucherDate)
        {
            return await _JournalVoucherDAL.GetLedgerBalance(OpeningYearCode, AccountCode, VoucherDate);
        }
        public async Task<ResponseResult> GetDashBoardData(string FromDate, string ToDate)
        {
            return await _JournalVoucherDAL.GetDashBoardData(FromDate, ToDate);
        }
        public async Task<JournalVoucherModel> GetDashBoardDetailData(string FromDate, string ToDate, string LedgerName, string VoucherNo, string AgainstVoucherRefNo, string AgainstVoucherNo)
        {
            return await _JournalVoucherDAL.GetDashBoardDetailData(FromDate, ToDate, LedgerName, VoucherNo, AgainstVoucherRefNo, AgainstVoucherNo);
        }
        public async Task<JournalVoucherModel> GetDashBoardSummaryData(string FromDate, string ToDate, string LedgerName, string VoucherNo, string AgainstVoucherRefNo, string AgainstVoucherNo)
        {
            return await _JournalVoucherDAL.GetDashBoardSummaryData(FromDate, ToDate, LedgerName, VoucherNo, AgainstVoucherRefNo, AgainstVoucherNo);
        }
        public async Task<ResponseResult> DeleteByID(int ID, int YearCode, int ActualEntryBy, string EntryByMachine, string ActualEntryDate, string VoucherType)
        {
            return await _JournalVoucherDAL.DeleteByID(ID, YearCode, ActualEntryBy, EntryByMachine, ActualEntryDate, VoucherType);
        }
        public async Task<JournalVoucherModel> PopUpForPendingVouchers(PopUpDataTableAgainstRef DataTable)
        {
            return await _JournalVoucherDAL.PopUpForPendingVouchers(DataTable);
        }
        public async Task<JournalVoucherModel> GetViewByID(int ID, int YearCode, string VoucherNo)
        {
            return await _JournalVoucherDAL.GetViewByID(ID, YearCode, VoucherNo);
        }
        public async Task<ResponseResult> CheckAmountBeforeSave(string VoucherDate, int YearCode, int AgainstVoucherYearCode, int AgainstVoucherEntryId, string AgainstVoucherNo, int AccountCode)
        {
            return await _JournalVoucherDAL.CheckAmountBeforeSave(VoucherDate, YearCode, AgainstVoucherYearCode, AgainstVoucherEntryId, AgainstVoucherNo, AccountCode);
        }
        public async Task<ResponseResult> FillSONO(string accountcode, string VoucherDate)
        {
            return await _JournalVoucherDAL.FillSONO(accountcode, VoucherDate);
        }
        public async Task<ResponseResult> GetSODetail(int SONO, string accountcode, string VoucherDate)
        {
            return await _JournalVoucherDAL.GetSODetail(SONO, accountcode, VoucherDate);
        }
        public async Task<ResponseResult> GetSODate(int SONO, string accountcode, string VoucherDate, string SOYearCode)
        {
            return await _JournalVoucherDAL.GetSODate(SONO, accountcode, VoucherDate, SOYearCode);
        }
        public async Task<ResponseResult> FillLedgerInDashboard(string FromDate, string ToDate, string VoucherType)
        {
            return await _JournalVoucherDAL.FillLedgerInDashboard(FromDate, ToDate, VoucherType);
        }
        public async Task<ResponseResult> FillBankInDashboard(string FromDate, string ToDate, string VoucherType)
        {
            return await _JournalVoucherDAL.FillBankInDashboard(FromDate, ToDate, VoucherType);
        }
        public async Task<ResponseResult> FillVoucherNoInDashboard(string FromDate, string ToDate, string VoucherType)
        {
            return await _JournalVoucherDAL.FillVoucherNoInDashboard(FromDate, ToDate, VoucherType);
        }
        public async Task<ResponseResult> FillAgainstVoucherRefNoInDashboard(string FromDate, string ToDate, string VoucherType)
        {
            return await _JournalVoucherDAL.FillAgainstVoucherRefNoInDashboard(FromDate, ToDate, VoucherType);
        }
        public async Task<ResponseResult> FillAgainstVoucherNoInDashboard(string FromDate, string ToDate, string VoucherType)
        {
            return await _JournalVoucherDAL.FillAgainstVoucherNoInDashboard(FromDate, ToDate, VoucherType);
        }
    }
}
