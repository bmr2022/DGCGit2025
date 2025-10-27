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
    public class HRLeaveMasterBLL: IHRLeaveMaster
    {
        private HRLeaveMasterDAL _HRLeaveMasterDAL;
        private readonly IDataLogic _DataLogicDAL;

        //private readonly IConfiguration configuration;

        public HRLeaveMasterBLL(IConfiguration config, IDataLogic dataLogicDAL, ConnectionStringService connectionStringService)
        {
            //configuration = config;
            _HRLeaveMasterDAL = new HRLeaveMasterDAL(config, dataLogicDAL, connectionStringService);
            _DataLogicDAL = dataLogicDAL;
        }

        public async Task<ResponseResult> GetleaveType()
        {
            return await _HRLeaveMasterDAL.GetleaveType();
        }
        public async Task<ResponseResult> GetApprovalleval()
        {
            return await _HRLeaveMasterDAL.GetApprovalleval();
        }

        public async Task<ResponseResult> GetLeaveCategory()
        {
            return await _HRLeaveMasterDAL.GetLeaveCategory();
        }

        public async Task<DataSet> GetEmployeeCategory()
        {
            return await _HRLeaveMasterDAL.GetEmployeeCategory();
        }
        public async Task<DataSet> GetDepartment()
        {
            return await _HRLeaveMasterDAL.GetDepartment();
        }
        public async Task<DataSet> GetLocation()
        {
            return await _HRLeaveMasterDAL.GetLocation();
        }
        public async Task<ResponseResult> FillLeaveId()
        {
            return await _HRLeaveMasterDAL.FillLeaveId();
        }
        public async Task<ResponseResult> SaveData(HRLeaveMasterModel model, List<string> HREmpCatDT, List<string> HRDeptCatDT,List<string> HRLocationDT)
        {
            return await _HRLeaveMasterDAL.SaveData(model, HREmpCatDT, HRDeptCatDT, HRLocationDT);
        }
        public async Task<ResponseResult> GetDashboardData()
        {
            return await _HRLeaveMasterDAL.GetDashboardData();
        }
        public async Task<HRLeaveMasterModel> GetDashboardDetailData()
        {
            return await _HRLeaveMasterDAL.GetDashboardDetailData();
        }
        public async Task<HRLeaveMasterModel> GetViewByID(int id)
        {
            return await _HRLeaveMasterDAL.GetViewByID(id);
        }
        public async Task<ResponseResult> GetFormRights(int uId)
        {
            return await _HRLeaveMasterDAL.GetFormRights(uId);
        }
    }
}
