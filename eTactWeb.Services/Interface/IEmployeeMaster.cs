using eTactWeb.DOM.Models;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Services.Interface
{
    public interface IEmployeeMaster
    {
        Task<ResponseResult> DeleteByID(int ID, string EmpName);

        Task<EmployeeMasterModel> GetByID(int ID);

        Task<EmployeeMasterModel> GetDashboardData(EmployeeMasterModel model);
        Task<EmployeeMasterModel> GetSearchData(EmployeeMasterModel model, string EmpCode);

        Task<ResponseResult> SaveEmployeeMaster(EmployeeMasterModel model);
        Task<ResponseResult> GetFormRights(int uId);
        Task<ResponseResult> GetEmpIdandEmpCode(string designation, string department);
        Task<ResponseResult> GetSalaryHead();
        Task<ResponseResult> GetSalaryMode(int SalaryHeadId);
        Task<ResponseResult> GetJobDepartMent();
        Task<ResponseResult> GetJobDesignation();
        Task<ResponseResult> GetJobShift();
    }
}
