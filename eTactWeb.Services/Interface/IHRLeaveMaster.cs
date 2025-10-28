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
        Task<ResponseResult> GetApprovalleval();
        Task<ResponseResult> GetLeaveCategory();

        Task<DataSet> GetEmployeeCategory();
        Task<DataSet> GetDepartment();
        Task<DataSet> GetLocation();
        Task<ResponseResult> GetFormRights(int uId);

        Task<ResponseResult> FillLeaveId();

        Task<ResponseResult> SaveData(HRLeaveMasterModel model, List<string> HREmpCatDT, List<string> HRDeptCatDT, List<string> HRLocationDT);

        Task<ResponseResult> GetDashboardData();
        Task<HRLeaveMasterModel> GetDashboardDetailData();
        Task<HRLeaveMasterModel> GetViewByID(int Id);
        Task<ResponseResult> DeleteByID(int ID,int LeaveYearcode,string EntryByMachine,int CreatedByEmpid,string CreationDate,string LeaveCode);
    }
}
