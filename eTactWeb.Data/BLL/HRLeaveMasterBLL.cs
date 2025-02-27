using eTactWeb.Data.DAL;
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

        public HRLeaveMasterBLL(IConfiguration config, IDataLogic dataLogicDAL)
        {
            //configuration = config;
            _HRLeaveMasterDAL = new HRLeaveMasterDAL(config, dataLogicDAL);
            _DataLogicDAL = dataLogicDAL;
        }

        public async Task<ResponseResult> GetleaveType()
        {
            return await _HRLeaveMasterDAL.GetleaveType();
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
    }
}
