using eTactWeb.DOM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Services.Interface
{
    public interface IAgainstAdjustVoucher
    {
      
        Task<ResponseResult> FillLedgerName(string VoucherType, string Type);
        Task<ResponseResult> FillVoucherType(int yearcode);
        Task<ResponseResult> FillVoucherNo(int YearCode, string VoucherType, string FromDate, string ToDate,int AccountCode);
        Task<ResponseResult> FillInvoiceNo(int YearCode, string VoucherType, string FromDate, string ToDate, string VoucherNo, int AccountCode);
        Task<ResponseResult> GetAccEntryId(int YearCode, string VoucherType, string VoucherNo, int AccountCode, string InvoiceNo);
        Task<ResponseResult> FillModeofAdjust(string VoucherType);
        Task<ResponseResult> GetLedgerBalance(int OpeningYearCode, int AccountCode, string VoucherDate);
        //Task<AgainstAdjustVoucherModel> GetAdjustedData(int YearCode, string VoucherType, string VoucherNo, int AccountCode, string InvoiceNo, int AccEntryId);
        Task<List<AgainstAdjustVoucherModel>> GetAdjustedData(int YearCode, string VoucherType, string VoucherNo, int AccountCode, string InvoiceNo, int AccEntryId);
        Task<ResponseResult> FillCostCenterName();
        Task<ResponseResult> FillEntryID(int YearCode, string VoucherDate);
        Task<ResponseResult> FillCurrency();
        Task<ResponseResult> GetFormRights(int uId);
        Task<AgainstAdjustVoucherModel> GetViewByID(int ID, int YearCode, string VoucherNo);
        //Task<ResponseResult> SaveAgainstAdjustVoucher(AgainstAdjustVoucherModel model, DataTable GIGrid);
    }
}
