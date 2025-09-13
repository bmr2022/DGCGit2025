using eTactWeb.DOM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Services.Interface
{
    public interface IHRWeekOffMaster
    {
        Task<ResponseResult> FillEntryId();
        Task<ResponseResult> SaveData(HRWeekOffMasterModel model, List<string> HREmployeeDT);
        Task<ResponseResult> GetDashboardData();
        Task<HRWeekOffMasterModel> GetDashboardDetailData();
        Task<HRWeekOffMasterModel> GetViewByID(int ID);
        Task<ResponseResult> DeleteByID(int ID);
        Task<DataSet> GetEmpCat();
        Task<ResponseResult> GetDeptCat();

    }
}
