using eTactWeb.DOM.Models;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Services.Interface
{
    public interface IEmployeeMaster
    {
        Task<ResponseResult> DeleteByID(int ID, string EmpName,int ActualEntrybyId, string EntryByMachineName);

        Task<EmployeeMasterModel> GetByID(int ID);

        //Task<EmployeeMasterModel> GetSearchData(EmployeeMasterModel model, string EmpCode, string ReportType);
        //Task<EmployeeMasterModel> GetDashboardData(EmployeeMasterModel model);
        Task<ResponseResult> GetDashboardData(EmployeeMasterModel model);
        Task<EmployeeMasterModel> GetDashboardDetailData();
        Task<ResponseResult> SaveEmployeeMaster(EmployeeMasterModel model, DataTable DtAllDed,DataTable DtEdu,DataTable dtexp,DataTable dtNjob);
        Task<ResponseResult> GetFormRights(int uId);
        Task<ResponseResult> GetEmpIdandEmpCode(string designation, string department);
        Task<ResponseResult> GetSalaryHead();
        Task<ResponseResult> GetSalaryMode(int SalaryHeadId);
        Task<ResponseResult> GetJobDepartMent();
        Task<ResponseResult> GetJobDesignation();
        Task<ResponseResult> GetJobShift();
        Task<ResponseResult> GetEmployeeType();
        Task<ResponseResult> GetReportingMg();
        Task<ResponseResult> FILLAllowanceMode();
        Task<ResponseResult> GetWorkLocation();
        Task<ResponseResult> GetRefThrough();
        Task<ResponseResult> GetReqNo(string EntryDate);
        Task<ResponseResult> GetEmpGrade();
        Task<ResponseResult> GetWagesType();
        Task<ResponseResult> GetCalculatePfOn();
    }
}
