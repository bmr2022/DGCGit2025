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
    public class HRHolidaysMasterBLL:IHRHolidaysMaster
    {
        private HRHolidaysMasterDAL _HRHolidaysMasterDAL;
        private readonly IDataLogic _DataLogicDAL;

        public HRHolidaysMasterBLL(IConfiguration config, IDataLogic dataLogicDAL)
        {
            _HRHolidaysMasterDAL = new HRHolidaysMasterDAL(config, dataLogicDAL);
            _DataLogicDAL = dataLogicDAL;
        }

        public async Task<ResponseResult> GetHolidayType()
        {
            return await _HRHolidaysMasterDAL.GetHolidayType();
        }
        public async Task<ResponseResult> GetHolidayCountry()
        {
            return await _HRHolidaysMasterDAL.GetHolidayCountry();
        }
        public async Task<ResponseResult> GetHolidayState()
        {
            return await _HRHolidaysMasterDAL.GetHolidayState();
        }

        public async Task<DataSet> GetEmployeeCategory()
        {
            return await _HRHolidaysMasterDAL.GetEmployeeCategory();
        }
        public async Task<DataSet> GetDepartment()
        {
            return await _HRHolidaysMasterDAL.GetDepartment();
        }

        public async Task<ResponseResult> SaveData(HRHolidaysMasterModel model, List<string> HREmployeeDT, List<string> HRDepartmentDT)
        {
            return await _HRHolidaysMasterDAL.SaveData(model, HREmployeeDT, HRDepartmentDT);
        }

        public async Task<ResponseResult> FillEntryId(int yearcode)
        {
            return await _HRHolidaysMasterDAL.FillEntryId(yearcode);
        }

        public async Task<ResponseResult> GetDashboardData()
        {
            return await _HRHolidaysMasterDAL.GetDashboardData();
        }
        public async Task<HRHolidaysMasterModel> GetDashboardDetailData()
        {
            return await _HRHolidaysMasterDAL.GetDashboardDetailData();
        }
        public async Task<HRHolidaysMasterModel> GetViewByID(int id)
        {
            return await _HRHolidaysMasterDAL.GetViewByID(id);
        }
    }
}
