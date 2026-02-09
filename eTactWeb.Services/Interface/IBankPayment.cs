using ClosedXML.Excel;
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
        Task<ResponseResult> GetFormRights(int uId);
        Task<ResponseResult> FillPONO(string accountcode, string VoucherDate);
        Task<ResponseResult> GetPOYearCode(string PONO, string accountcode, string VoucherDate);
        Task<ResponseResult> GetPODate(string PONO, string accountcode, string VoucherDate, string POYearCode);
        Task<ResponseResult> GetLedgerBalance(int OpeningYearCode, int AccountCode, string VoucherDate);
        Task<ResponseResult> SaveBankPayment(BankPaymentModel model, DataTable GIGrid);
        //Task<ResponseResult> GetDashBoardData(string FromDate, string ToDate);
        Task<ResponseResult> GetDashBoardData(string summaryDetail, string FromDate, string ToDate, string LedgerName, string Bank, string VoucherNo, string AgainstVoucherNo, string PONo, string AgainstBillno);
        Task<BankPaymentModel> GetDashBoardDetailData(string FromDate, string ToDate, string LedgerName, string Bank, string VoucherNo, string AgainstVoucherNo, string PONo, string AgainstBillno);
        Task<BankPaymentModel> GetDashBoardSummaryData(string FromDate, string ToDate, string LedgerName, string Bank, string VoucherNo, string AgainstVoucherNo, string PONo, string AgainstBillno);
        Task<ResponseResult> DeleteByID(int ID, int YearCode, int ActualEntryBy, string EntryByMachine, string ActualEntryDate);
        Task<ResponseResult> FillBankType(int AccountCode);
        Task<BankPaymentModel> PopUpForPendingVouchers(PopUpDataTableAgainstRef DataTable);
        Task<BankPaymentModel> GetViewByID(int ID, int YearCode, string VoucherNo);
        Task<ResponseResult> CheckAmountBeforeSave(string VoucherDate, int YearCode, int AgainstVoucherYearCode, int AgainstVoucherEntryId, string AgainstVoucherNo, int AccountCode);
        Task<ResponseResult> FillLedgerInDashboard(string FromDate, string ToDate, string VoucherType);
        Task<ResponseResult> FillBankInDashboard(string FromDate, string ToDate, string VoucherType);
        Task<ResponseResult> FillVoucherNoInDashboard(string FromDate, string ToDate, string VoucherType);
        Task<ResponseResult> FillAgainstVoucherRefNoInDashboard(string FromDate, string ToDate, string VoucherType);
        Task<ResponseResult> FillAgainstVoucherNoInDashboard(string FromDate, string ToDate, string VoucherType);
    }
}
