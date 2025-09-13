using eTactWeb.DOM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Services.Interface
{
    public interface IOutStanding
    {
        Task<ResponseResult> GetPartyName(string outstandingType,string TillDate);

        Task<ResponseResult> GetGroupName(string outstandingType, string TillDate);

        Task<ResponseResult> GetVoucherNo(int CurrentYear);

        Task<ResponseResult> GetVoucherType(int CurrentYear);
        public Task<OutStandingModel> GetDetailsData(string outstandingType, string TillDate,string GroupName,string[] AccountNameList,int AccountCode,string ShowOnlyApprovedBill, bool ShowZeroBal);

        Task<OutStandingModel> GetVoucherList(int AccountCode, string VoucherNo, string VoucherType, int VoucherYearCode, int CurrentYear);
    }
}
