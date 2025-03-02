using eTactWeb.DOM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Services.Interface
{
    public interface IHRLeaveMaster
    {
        Task<ResponseResult> GetleaveType();
        Task<ResponseResult> GetLeaveCategory();

        Task<DataSet> GetEmployeeCategory();
        Task<DataSet> GetDepartment();
        Task<DataSet> GetLocation();

        Task<ResponseResult> FillLeaveId();

        Task<ResponseResult> SaveData(HRLeaveMasterModel model, DataTable HREmpCatDT, DataTable HRDeptCatDT,DataTable HRLocationDT);

        Task<ResponseResult> GetDashboardData();
        Task<HRLeaveMasterModel> GetDashboardDetailData();
        Task<HRLeaveMasterModel> GetViewByID(int Id);
    }
}
