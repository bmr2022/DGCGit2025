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
    public class HRLeaveOpeningMasterBLL: IHRLeaveOpeningMaster
    {
        private readonly IDataLogic _DataLogicDAL;
        private readonly HRLeaveOpeningMasterDAL _HRLeaveOpeningMasterDAL;
        public HRLeaveOpeningMasterBLL(IConfiguration configuration, IDataLogic iDataLogic, ConnectionStringService connectionStringService)
        {
            _DataLogicDAL = iDataLogic;
            _HRLeaveOpeningMasterDAL = new HRLeaveOpeningMasterDAL(configuration, iDataLogic, connectionStringService);
        }

        public async Task<ResponseResult> GetEmpCat()
        {
            return await _HRLeaveOpeningMasterDAL.GetEmpCat();
        }

        public async Task<ResponseResult> GetDepartment(int empid)
        {
            return await _HRLeaveOpeningMasterDAL.GetDepartment(empid);
        }

        public async Task<ResponseResult> GetDesignation(int empid)
        {
            return await _HRLeaveOpeningMasterDAL.GetDesignation(empid);
        }

        public async Task<ResponseResult> GetLeaveName()
        {
            return await _HRLeaveOpeningMasterDAL.GetLeaveName();
        }

        public async Task<ResponseResult> GetShiftName(int EmpId)
        {
            return await _HRLeaveOpeningMasterDAL.GetShiftName(EmpId);
        }

        public async Task<ResponseResult> GetEmpCode()
        {
            return await _HRLeaveOpeningMasterDAL.GetEmpCode();
        }

        public async Task<ResponseResult> FillEntryId()
        {
            return await _HRLeaveOpeningMasterDAL.FillEntryId();
        }

        public async Task<ResponseResult> SaveMainData(HRLeaveOpeningMasterModel model,DataTable GIGrid)
        {
            return await _HRLeaveOpeningMasterDAL.SaveMainData(model, GIGrid);
        }

        public async Task<ResponseResult> GetDashboardData()
        {
            return await _HRLeaveOpeningMasterDAL.GetDashboardData();
        }
        public async Task<HRLeaveOpeningDashBoardModel> GetDashboardDetailData(string ReportType,string FromDate, string ToDate)
        {
            return await _HRLeaveOpeningMasterDAL.GetDashboardDetailData(ReportType,FromDate, ToDate);
        }

        public async Task<HRLeaveOpeningMasterModel> GetViewByID(int id,int year)
        {
            return await _HRLeaveOpeningMasterDAL.GetViewByID(id,year);
        }
        public async Task<ResponseResult> DeleteByID(int id,int year, string EntryByMachineName)
        {
            return await _HRLeaveOpeningMasterDAL.DeleteByID(id,year, EntryByMachineName);
        }

    }
}
