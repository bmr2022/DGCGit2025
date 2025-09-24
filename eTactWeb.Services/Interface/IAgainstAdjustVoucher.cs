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
       
        Task<ResponseResult> GetFormRights(int uId);
       
    }
}
