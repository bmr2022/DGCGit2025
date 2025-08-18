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
    public interface IMachineMaster
    {
        //Task<ResponseResult> GetNewEntry(string Flag, int YearCode, string SPName);
        Task<ResponseResult> GetFormRights(int uId);

        Task<ResponseResult> FillMachineGroup();
        Task<ResponseResult> FillMachineWorkCenter();
        Task<ResponseResult> SaveMachineMaster(MachineMasterModel model);
        Task<ResponseResult> GetDashBoardData();
        Task<MachineMasterModel> GetDashBoardDetailData();
        Task<MachineMasterModel> GetViewByID();
        Task<ResponseResult> DeleteByID(int ID);

       // Task<MachineMasterModel> ViewByID(int id);
    }
}
