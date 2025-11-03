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
    public class HRWeekOffMasterBLL: IHRWeekOffMaster
    {
        private HRWeekOffMasterDAL _HRWeekOffMasterDAL;
        private readonly IDataLogic _DataLogicDAL;

        public HRWeekOffMasterBLL(IConfiguration config, IDataLogic dataLogicDAL, ConnectionStringService connectionStringService)
        {
            _HRWeekOffMasterDAL = new HRWeekOffMasterDAL(config, dataLogicDAL, connectionStringService);
            _DataLogicDAL = dataLogicDAL;
        }
        public async Task<ResponseResult> FillEntryId()
        {
            return await _HRWeekOffMasterDAL.FillEntryId();
        }
        public async Task<ResponseResult> SaveData(HRWeekOffMasterModel model, List<string> HREmployeeDT)
        {
            return await _HRWeekOffMasterDAL.SaveData(model, HREmployeeDT);
        }
        public async Task<ResponseResult> GetDashboardData()
        {
            return await _HRWeekOffMasterDAL.GetDashboardData();
        }
        public async Task<HRWeekOffMasterModel> GetDashboardDetailData()
        {
            return await _HRWeekOffMasterDAL.GetDashboardDetailData();
        }
        public async Task<HRWeekOffMasterModel> GetViewByID(int ID)
        {
            return await _HRWeekOffMasterDAL.GetViewByID(ID);
        }
        public async Task<ResponseResult> DeleteByID(int ID)
        {
            return await _HRWeekOffMasterDAL.DeleteByID(ID);
        }
       
        public async Task<DataSet> GetEmpCat()
        {
            return await _HRWeekOffMasterDAL.GetEmpCat();
        }
        public async Task<ResponseResult> GetDeptCat()
        {
            return await _HRWeekOffMasterDAL.GetDeptCat();
        }

    }
}
