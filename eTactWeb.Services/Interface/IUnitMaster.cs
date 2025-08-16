using eTactWeb.DOM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Services.Interface
{
    public interface IUnitMaster
    {
        Task<ResponseResult> SaveUnitMaster(UnitMasterModel model);
        Task<ResponseResult> GetDashBoardData();
        Task<UnitMasterModel> GetDashBoardDetailData();

        Task<UnitMasterModel> GetViewByID(String Unit_Name);
        Task<ResponseResult> DeleteByID(String Unit_Name);
        Task<ResponseResult> GetFormRights(int uId);

    }
}
