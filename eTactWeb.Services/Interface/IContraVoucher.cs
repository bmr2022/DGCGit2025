using eTactWeb.DOM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Services.Interface
{
    public interface IContraVoucher
    {
        Task<ResponseResult> FillLedgerName(string VoucherType, string Type);
        Task<ResponseResult> FillSubVoucherName(string VoucherType);
        Task<ResponseResult> FillIntrument(string VoucherType);
        Task<ResponseResult> FillModeofAdjust(string VoucherType);
        Task<ResponseResult> FillCostCenterName();
        Task<ResponseResult> FillEntryID(int YearCode, string VoucherDate);
        Task<ResponseResult> FillCurrency();
        Task<ResponseResult> GetLedgerBalance(int OpeningYearCode, int AccountCode, string VoucherDate);
        Task<ResponseResult> SaveContraVoucher(ContraVoucherModel model, DataTable GIGrid);
        Task<ResponseResult> GetDashBoardData(string FromDate, string ToDate);
        Task<ContraVoucherModel> GetDashBoardDetailData(string FromDate, string ToDate, string LedgerName, string Bank, string VoucherNo);
        Task<ContraVoucherModel> GetDashBoardSummaryData(string FromDate, string ToDate, string LedgerName, string Bank, string VoucherNo);
        Task<ResponseResult> DeleteByID(int ID, int YearCode, int ActualEntryBy, string EntryByMachine, string ActualEntryDate, string VoucherType);
        Task<ResponseResult> FillBankType(int AccountCode);
        Task<ContraVoucherModel> PopUpForPendingVouchers(PopUpDataTableAgainstRef DataTable);
        Task<ContraVoucherModel> GetViewByID(int ID, int YearCode, string VoucherNo);
        Task<ResponseResult> CheckAmountBeforeSave(string VoucherDate, int YearCode, int AgainstVoucherYearCode, int AgainstVoucherEntryId, string AgainstVoucherNo, int AccountCode);
        Task<ResponseResult> FillSONO(string accountcode, string VoucherDate);
        Task<ResponseResult> GetSoYearCode(int SONO, string accountcode, string VoucherDate);
        Task<ResponseResult> GetSODate(int SONO, string accountcode, string VoucherDate, string SOYearCode);
        Task<ResponseResult> FillLedgerInDashboard(string FromDate, string ToDate, string VoucherType);
        Task<ResponseResult> FillBankInDashboard(string FromDate, string ToDate, string VoucherType);
        Task<ResponseResult> FillVoucherNoInDashboard(string FromDate, string ToDate, string VoucherType);
      
    }
}
