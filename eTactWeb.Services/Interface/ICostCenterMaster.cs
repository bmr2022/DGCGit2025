using eTactWeb.DOM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Services.Interface
{
    public interface ICostCenterMaster
    {
        Task<ResponseResult> FillCostCenterID();
        Task<ResponseResult> FillCostCenterGroupName();
        Task<ResponseResult> FillDeptName();
        Task<ResponseResult> FillUnderGroupName();
        Task<ResponseResult> SaveCostCenterMaster(CostCenterMasterModel model);
        Task<ResponseResult> GetDashBoardData();
        Task<CostCenterMasterModel> GetDashBoardDetailData();
        Task<CostCenterMasterModel> GetViewByID(int ID);
        Task<ResponseResult> DeleteByID(int ID);
        Task<ResponseResult> ChkForDuplicate(string CostCenterName);
    }
}
