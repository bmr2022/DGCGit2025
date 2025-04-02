using eTactWeb.Data.Common;
using eTactWeb.Data.DAL;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Data.BLL
{
    public class UserRightReportBLL:IUserRightReport
    {
        private UserRightReportDAL _UserRightReportDAL;
        private readonly IDataLogic _DataLogicDAL;

        public UserRightReportBLL(IConfiguration config, IDataLogic dataLogicDAL, ConnectionStringService connectionStringService)
        {
            _UserRightReportDAL = new UserRightReportDAL(config, dataLogicDAL, connectionStringService);
            _DataLogicDAL = dataLogicDAL;
        }
        public async Task<UserRightReportModel> GetUserRightsReportDetailData(string fromDate, string toDate, string ReportType, string UserName, string EmployeeName, string FormName, string ModuleName, string MachineName)
        {
            return await _UserRightReportDAL.GetUserRightsReportDetailData(fromDate, toDate, ReportType, UserName, EmployeeName, FormName, ModuleName, MachineName);
        }
        public async Task<ResponseResult> FillUserName(string ReportType)
        {
            return await _UserRightReportDAL.FillUserName(ReportType);
        }
        public async Task<ResponseResult> FillEmployeeName(string ReportType)
        {
            return await _UserRightReportDAL.FillEmployeeName(ReportType);
        }
        public async Task<ResponseResult> FillFormName(string ReportType)
        {
            return await _UserRightReportDAL.FillFormName(ReportType);
        }
        public async Task<ResponseResult> FillModuleName(string ReportType)
        {
            return await _UserRightReportDAL.FillModuleName(ReportType);
        }
        public async Task<ResponseResult> FillMachineName(string ReportType)
        {
            return await _UserRightReportDAL.FillMachineName(ReportType);
        }
    }
}

