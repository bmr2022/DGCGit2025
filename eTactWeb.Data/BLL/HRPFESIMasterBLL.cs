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
    public class HRPFESIMasterBLL: IHRPFESIMaster
    {
        private HRPFESIMasterDAL _HRPFESIMasterDAL;
        private readonly IDataLogic _DataLogicDAL;

        //private readonly IConfiguration configuration;

        public HRPFESIMasterBLL(IConfiguration config, IDataLogic dataLogicDAL)
        {
            //configuration = config;
            _HRPFESIMasterDAL = new HRPFESIMasterDAL(config, dataLogicDAL);
            _DataLogicDAL = dataLogicDAL;
        }
        public async Task<ResponseResult> GetESIDispensary()
        {
            return await _HRPFESIMasterDAL.GetESIDispensary();
        }
        public async Task<ResponseResult> FillEntryId()
        {
            return await _HRPFESIMasterDAL.FillEntryId();
        }
        public async Task<ResponseResult> SaveData(HRPFESIMasterModel model)
        {
            return await _HRPFESIMasterDAL.SaveData(model);
        }
        public async Task<DataSet> GetExemptedCategories()
        {
            return await _HRPFESIMasterDAL.GetExemptedCategories();
        }
        public async Task<ResponseResult> GetDashboardData()
        {
            return await _HRPFESIMasterDAL.GetDashboardData();
        }
        public async Task<HRPFESIMasterModel> GetDashboardDetailData()
        {
            return await _HRPFESIMasterDAL.GetDashboardDetailData();
        }
        public async Task<HRPFESIMasterModel> GetViewByID(int id)
        {
            return await _HRPFESIMasterDAL.GetViewByID(id);
        }
        public async Task<ResponseResult> GetFormRights(int uId)
        {
            return await _HRPFESIMasterDAL.GetFormRights(uId);
        }
        public async Task<ResponseResult> DeleteByID(int ID,string machineName)
        {
            return await _HRPFESIMasterDAL.DeleteByID(ID,machineName);
        }

    }
}
