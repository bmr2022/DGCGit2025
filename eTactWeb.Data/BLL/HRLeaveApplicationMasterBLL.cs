using eTactWeb.Data.Common;
using eTactWeb.Data.DAL;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Data.BLL
{
    public class HRLeaveApplicationMasterBLL:IHRLeaveApplicationMaster
    {
        private readonly IDataLogic _DataLogicDAL;
        private HRLeaveApplicationMasterDAL _HRLeaveApplicationMasterDAL;

        public HRLeaveApplicationMasterBLL(IConfiguration configuration, IDataLogic dataLogicDAL, ConnectionStringService connectionStringService)
        {
            _DataLogicDAL = dataLogicDAL;
            _HRLeaveApplicationMasterDAL = new HRLeaveApplicationMasterDAL(configuration, dataLogicDAL, connectionStringService);
        }
        public async Task<ResponseResult> GetEmpName()
        {
            return await _HRLeaveApplicationMasterDAL.GetEmpName();
        }
        public async Task<ResponseResult> GetLeaveName()
        {
            return await _HRLeaveApplicationMasterDAL.GetLeaveName();
        }
        public async Task<ResponseResult> GetShiftName()
        {
            return await _HRLeaveApplicationMasterDAL.GetShiftName();
        }
        public async Task<ResponseResult> GetEmpCode()
        {
            return await _HRLeaveApplicationMasterDAL.GetEmpCode();
        }
        public async Task<ResponseResult> FillEntryId(int YearCode)
        {
            return await _HRLeaveApplicationMasterDAL.FillEntryId( YearCode);
        }
        public async Task<ResponseResult> GetEmployeeDetail(int empid)
        {
            return await _HRLeaveApplicationMasterDAL.GetEmployeeDetail(empid);
        }

        public async Task<ResponseResult> SaveData(HRLeaveApplicationMasterModel model, DataTable DT)
        {
            //throw new NotImplementedException();
            return await _HRLeaveApplicationMasterDAL.SaveData(model, DT);
        }

        public async Task<ResponseResult> GetDashboardData()
        {
            return await _HRLeaveApplicationMasterDAL.GetDashboardData();
        }
        public async Task<HRLeaveApplicationDashBoard> GetDashboardDetailData(string ReportType, string FromDate, string ToDate)
        {
            return await _HRLeaveApplicationMasterDAL.GetDashboardDetailData(ReportType, FromDate, ToDate);
        }
        public async Task<HRLeaveApplicationMasterModel> GetViewByID(int id, int year)
        {
            return await _HRLeaveApplicationMasterDAL.GetViewByID(id, year);
        }

        public async Task<ResponseResult> DeleteByID(int id, int year)
        {
            return await _HRLeaveApplicationMasterDAL.DeleteByID(id, year);
        }
    }
}
