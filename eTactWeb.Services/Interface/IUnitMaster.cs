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
        Task<ResponseResult> GetDashBoardData(int userID);
        Task<UnitMasterModel> GetDashBoardDetailData(int userID);

        Task<UnitMasterModel> GetViewByID(String Unit_Name);
        Task<ResponseResult> DeleteByID(String Unit_Name, int userID);
        Task<ResponseResult> GetFormRights(int uId);

    }
}
