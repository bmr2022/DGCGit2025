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
    public interface IWorkCenterMaster
    {
        //Task<ResponseResult> GetNewEntry(string Flag, int YearCode, string SPName);
        Task<WorkCenterMasterModel> ViewByID(int id);
        Task<WorkCenterMasterModel> GetDashboardData(WorkCenterMasterModel model);
        Task<ResponseResult> SaveData(WorkCenterMasterModel model);
        Task<ResponseResult> DeleteMachine(string WorkCenterDescription);
    }
}
