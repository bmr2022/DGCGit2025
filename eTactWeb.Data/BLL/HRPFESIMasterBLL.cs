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

    }
}
