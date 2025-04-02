using eTactWeb.DOM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Services.Interface
{
    public interface IPrimaryAccountGroupMaster
    {
        Task<ResponseResult> GetFormRights(int userID);
        Task<ResponseResult> GetParentGroup();
        Task<ResponseResult> GetAccountGroupDetailsByParentCode(int parentAccountCode);
        Task<ResponseResult> SavePrimaryAccountGroupMaster(PrimaryAccountGroupMasterModel model);
        Task<ResponseResult> UpdatePrimaryAccountGroupMaster(PrimaryAccountGroupMasterModel model);
        
        Task<ResponseResult> GetDashboardData();
        Task<PrimaryAccountGroupMasterDashBoardModel> GetDashboardDetailData();
        Task<PrimaryAccountGroupMasterModel> GetViewByID(int accountCode);
        Task<ResponseResult> DeleteByID(int accountCode);
    }
}
