using eTactWeb.DOM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Services.Interface
{
    public interface IBankPayment
    {
        Task<ResponseResult> FillLedgerName(string VoucherType, string Type);
        Task<ResponseResult> FillSubVoucherName(string VoucherType);
        Task<ResponseResult> FillIntrument(string VoucherType);
        Task<ResponseResult> FillModeofAdjust(string VoucherType);
        Task<ResponseResult> FillCostCenterName();
        Task<ResponseResult> FillEntryID(int YearCode, string VoucherDate);
        Task<ResponseResult> FillCurrency();
        Task<ResponseResult> FillPONO(string accountcode, string VoucherDate);
        Task<ResponseResult> GetPODetail(int PONO,string accountcode,string VoucherDate);
        Task<ResponseResult> GetPODate(int PONO, string accountcode, string VoucherDate,string POYearCode);

        Task<ResponseResult> GetLedgerBalance(int OpeningYearCode, int AccountCode, string VoucherDate);
        Task<ResponseResult> SaveBankPayment(BankPaymentModel model, DataTable GIGrid);
        Task<ResponseResult> GetDashBoardData(string FromDate, string ToDate);
        Task<BankPaymentModel> GetDashBoardDetailData(string FromDate, string ToDate);
        Task<BankPaymentModel> GetDashBoardSummaryData(string FromDate, string ToDate);
        Task<ResponseResult> DeleteByID(int ID, int YearCode, int ActualEntryBy, string EntryByMachine, string ActualEntryDate);
        Task<ResponseResult> FillBankType(int AccountCode);
        Task<BankPaymentModel> PopUpForPendingVouchers(PopUpDataTableAgainstRef DataTable);
        Task<BankPaymentModel> GetViewByID(int ID, int YearCode, string VoucherNo);
        Task<ResponseResult> CheckAmountBeforeSave(string VoucherDate, int YearCode, int AgainstVoucherYearCode, int AgainstVoucherEntryId, string AgainstVoucherNo, int AccountCode);
    }
}
