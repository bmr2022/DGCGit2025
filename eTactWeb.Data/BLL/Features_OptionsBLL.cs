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
    public class Features_OptionsBLL:IFeatures_Options
    {
        private Features_OptionsDAL _Features_OptionsDAL;
        private readonly IDataLogic _DataLogicDAL;

        public Features_OptionsBLL(IConfiguration config, IDataLogic dataLogicDAL, ConnectionStringService connectionStringService)
        {
            _Features_OptionsDAL = new Features_OptionsDAL(config, dataLogicDAL, connectionStringService);
            _DataLogicDAL = dataLogicDAL;
        }
        public async Task<ResponseResult> GetDashboardData()
        {
            return await _Features_OptionsDAL.GetDashboardData();
        }
        public async Task<Features_OptionsModel> GetDashboardDetailData(string Type)
        {
            return await _Features_OptionsDAL.GetDashboardDetailData( Type);
        }
        public async Task<Features_OptionsModel> GetViewByID(string Type)
        {
            return await _Features_OptionsDAL.GetViewByID( Type);
        }
        public async Task<ResponseResult> SaveFeatures_Options(Features_OptionsModel model)
        {
            return await _Features_OptionsDAL.SaveFeatures_Options(model);
        }
    }
}
