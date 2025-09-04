using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eTactWeb.DOM.Models;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Services.Interface
{
    public interface IVendorMater
    {
        Task<ResponseResult> FillEntryId();
        Task<ResponseResult> ViewDataByVendor(int accountCode);
        Task<ResponseResult> FillVendorList(string isShowAll);
        Task<ResponseResult> CheckUserDuplication(int userId);
        Task<ResponseResult> SaveVendorUser(VendorUserModel model);
        Task<VendorUserModel> GetViewByID(int ID,string mode);
        Task<ResponseResult> DeleteByID(int userEntryId, int accountCode,int userId, string entryByMachineName, int actualEntryBy, string actualEntryDate);
        Task<ResponseResult> GetDashboardData(string accountName = "", int? userId = null);
    }
}
