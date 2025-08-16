using eTactWeb.DOM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Services.Interface
{
    public interface IDepartmentMaster
    {
        Task<ResponseResult> GetFormRights(int uId);

        Task<ResponseResult> FillDeptType();
        Task<ResponseResult> FillDeptID();

        Task<ResponseResult> SaveDeptMaster(DepartmentMasterModel model);

        Task<ResponseResult> GetDashBoardData();
        Task<DepartmentMasterModel> GetDashBoardDetailData();

        Task<DepartmentMasterModel> GetViewByID(int ID);
        Task<ResponseResult> DeleteByID(int ID);
    }
}
