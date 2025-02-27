using eTactWeb.DOM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Services.Interface
{
    public interface IUserRightReport
    {
        Task<UserRightReportModel> GetUserRightsReportDetailData(string fromDate, string toDate, string ReportType, string UserName, string EmployeeName, string FormName, string ModuleName, string MachineName);
        Task<ResponseResult> FillUserName(string ReportType);
        Task<ResponseResult> FillEmployeeName(string ReportType);
        Task<ResponseResult> FillFormName(string ReportType);
        Task<ResponseResult> FillModuleName(string ReportType);
        Task<ResponseResult> FillMachineName(string ReportType);
    }
}
