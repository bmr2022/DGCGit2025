using eTactWeb.DOM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Services.Interface
{
    public interface IBankReceipt
    {
        Task<ResponseResult> FillLedgerName(string VoucherType,string Type);
        Task<ResponseResult> FillSubVoucherName(string VoucherType);
        Task<ResponseResult> FillIntrument(string VoucherType);
        Task<ResponseResult> FillModeofAdjust(string VoucherType);
        Task<ResponseResult> FillCostCenterName();
        Task<ResponseResult> FillEntryID();
        Task<ResponseResult> FillCurrency();
        Task<ResponseResult> GetLedgerBalance(int OpeningYearCode, int AccountCode,string VoucherDate);
        Task<ResponseResult> SaveBankReceipt(BankReceiptModel model, DataTable GIGrid);
        Task<ResponseResult> GetDashBoardData(string FromDate, string ToDate);
        Task<BankReceiptModel> GetDashBoardDetailData(string FromDate, string ToDate);
        Task<BankReceiptModel> GetDashBoardSummaryData(string FromDate, string ToDate);
        Task<ResponseResult> DeleteByID(int ID,int YearCode);
        Task<ResponseResult> FillBankType(int AccountCode);
        Task<BankReceiptModel> PopUpForPendingVouchers(PopUpDataTable DataTable);
        Task<BankReceiptModel> GetViewByID(int ID, int YearCode, string VoucherNo);
    }
}
