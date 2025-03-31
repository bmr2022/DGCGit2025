using eTactWeb.DOM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Services.Interface
{
    public interface IJournalVoucher
    {
        Task<ResponseResult> FillLedgerName(string VoucherType, string Type);
        Task<ResponseResult> FillSubVoucherName(string VoucherType);
        Task<ResponseResult> FillIntrument(string VoucherType);
        Task<ResponseResult> FillModeofAdjust(string VoucherType);
        Task<ResponseResult> FillCostCenterName();
        Task<ResponseResult> FillEntryID(int YearCode, string VoucherDate);
        Task<ResponseResult> FillCurrency();
        Task<ResponseResult> GetLedgerBalance(int OpeningYearCode, int AccountCode, string VoucherDate);
        Task<ResponseResult> SaveBankReceipt(JournalVoucherModel model, DataTable GIGrid);
        Task<ResponseResult> GetDashBoardData(string FromDate, string ToDate);
        Task<JournalVoucherModel> GetDashBoardDetailData(string FromDate, string ToDate);
        Task<JournalVoucherModel> GetDashBoardSummaryData(string FromDate, string ToDate);
        Task<ResponseResult> DeleteByID(int ID, int YearCode, int ActualEntryBy, string EntryByMachine, string ActualEntryDate, string VoucherType);
        Task<ResponseResult> FillBankType(int AccountCode);
        Task<JournalVoucherModel> PopUpForPendingVouchers(PopUpDataTableAgainstRef DataTable);
        Task<JournalVoucherModel> GetViewByID(int ID, int YearCode, string VoucherNo);
        Task<ResponseResult> CheckAmountBeforeSave(string VoucherDate, int YearCode, int AgainstVoucherYearCode, int AgainstVoucherEntryId, string AgainstVoucherNo, int AccountCode);
        Task<ResponseResult> FillSONO(string accountcode, string VoucherDate);
        Task<ResponseResult> GetSODetail(int SONO, string accountcode, string VoucherDate);
        Task<ResponseResult> GetSODate(int SONO, string accountcode, string VoucherDate, string SOYearCode);
    }
}
