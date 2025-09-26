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
        Task<ResponseResult> FillSubVoucherName(string VoucherType);
       
        Task<ResponseResult> FillModeofAdjust(string VoucherType);
        Task<ResponseResult> FillCostCenterName();
        Task<ResponseResult> FillEntryID(int YearCode, string VoucherDate);
        Task<ResponseResult> FillCurrency();
        Task<ResponseResult> GetFormRights(int uId);
        Task<AgainstAdjustVoucherModel> GetViewByID(int ID, int YearCode, string VoucherNo);
        //Task<ResponseResult> SaveAgainstAdjustVoucher(AgainstAdjustVoucherModel model, DataTable GIGrid);
    }
}
