using eTactWeb.DOM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Services.Interface
{
    public interface IHRShiftMaster
    {
        Task<ResponseResult> GetShiftId();
        Task<ResponseResult> SaveHrShiftMaster(HRShiftMasterModel model);
        Task<ResponseResult> GetDashBoardData();
        Task<HRShiftMasterModel> GetDashBoardDetailData();
        Task<ResponseResult> DeleteByID(int ID);
        Task<HRShiftMasterModel> GetViewByID(int ID);
    }
}
