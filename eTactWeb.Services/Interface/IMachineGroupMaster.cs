using eTactWeb.DOM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Services.Interface
{
    public interface IMachineGroupMaster
    {
		Task<ResponseResult> FillMachineGroup();
		Task<ResponseResult> SaveMachineGroupMaster(MachineGroupMasterModel model);
        Task<ResponseResult> GetDashboardData(MachineGroupMasterModel model);
        Task<MachineGroupMasterModel> GetDashboardDetailData(string FromDate, string ToDate);
        Task<ResponseResult> DeleteByID(int EntryId);
        Task<MachineGroupMasterModel> GetViewByID(int ID);
    }
}
