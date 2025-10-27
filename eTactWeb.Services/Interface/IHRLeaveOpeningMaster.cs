using eTactWeb.DOM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Services.Interface
{
    public interface IHRLeaveOpeningMaster
    {
        Task<ResponseResult> GetEmpCat();
        Task<ResponseResult> GetDepartment(int empid);
        Task<ResponseResult> GetDesignation(int empid);
        Task<ResponseResult> GetLeaveName();
        Task<ResponseResult> GetShiftName(int EmpId);
        Task<ResponseResult> GetEmpCode();
        Task<ResponseResult> FillEntryId();
        Task<ResponseResult> SaveMainData(HRLeaveOpeningMasterModel model, DataTable GIGrid);

        Task<ResponseResult> GetDashboardData();
        Task<HRLeaveOpeningDashBoardModel> GetDashboardDetailData(string ReportType,string FromDate, string ToDate);

        Task<HRLeaveOpeningMasterModel> GetViewByID(int id,int year);

        Task<ResponseResult> DeleteByID(int Id,int year, string EntryByMachineName);

    }
}
