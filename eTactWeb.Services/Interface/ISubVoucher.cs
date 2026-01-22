using eTactWeb.DOM.Models;
using eTactWeb.DOM.Models.Master;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Services.Interface
{
    public interface  ISubVoucher
    {
      public  Task<ResponseResult> GetMainVoucherNames();
      public  Task<ResponseResult> GetEmployeeList();
      public  Task<ResponseResult> GetTableName(string MainVoucherName);
      public Task<ResponseResult> SaveSubVoucher(SubVoucherModel model);
      public  Task<ResponseResult> GetDashboardData();
      public  Task<SubVoucherDashBoardGridModel> GetDashboardDetailData();
       public Task<ResponseResult> UpdateSubVoucherPrefixSetting(SubVoucherDashBoardModel model);
      public  Task<SubVoucherModel> GetViewByID(int PrefixEntryId, string MainVoucherName, string MainVoucherTableName);
      public  Task<ResponseResult> DeleteByID(string MainVoucherName, string MainVoucherTableName, string StartSubVouchDiffSeries, int ActualEntryBy, int UpdatedBy, int PrefixEntryId);
    }
}
