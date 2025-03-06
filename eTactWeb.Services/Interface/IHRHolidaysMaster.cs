using eTactWeb.DOM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Services.Interface
{
    public interface IHRHolidaysMaster
    {
        Task<ResponseResult> GetHolidayType();
        Task<ResponseResult> GetHolidayCountry();
        Task<ResponseResult> GetHolidayState();

        Task<DataSet> GetEmployeeCategory();
        Task<DataSet> GetDepartment();
        Task<ResponseResult> FillEntryId(int yearcode);

        Task<ResponseResult> SaveData(HRHolidaysMasterModel model, List<string> HREmployeeDT, List<string> HRDepartmentDT);
        Task<ResponseResult> GetDashboardData();
        Task<HRHolidaysMasterModel> GetDashboardDetailData(string FromDate, string ToDate);

        Task<HRHolidaysMasterModel> GetViewByID(int ID,int year);
        Task<ResponseResult> DeleteByID(int ID, int year);
    }
}

