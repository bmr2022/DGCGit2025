using eTactWeb.Data.Common;
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
    public class ControlPlanBLL: IControlPlan
    {
        private ControlPlanDAL _ControlPlanDAL;
        private readonly IDataLogic _DataLogicDAL;

        public ControlPlanBLL(IConfiguration config, IDataLogic dataLogicDAL, ConnectionStringService connectionStringService)
        {
            _ControlPlanDAL = new ControlPlanDAL(config, dataLogicDAL, connectionStringService);
            _DataLogicDAL = dataLogicDAL;
        }
        public async Task<ResponseResult> GetNewEntryId(int Yearcode)
        {
            return await _ControlPlanDAL.GetNewEntryId(Yearcode);
        }
        public async Task<ResponseResult> GetItemName()
        {
            return await _ControlPlanDAL.GetItemName();
        }
        public async Task<ResponseResult> GetPartCode()
        {
            return await _ControlPlanDAL.GetPartCode();
        }
         public async Task<ResponseResult> GetCharacteristic()
        {
            return await _ControlPlanDAL.GetCharacteristic();
        }
         public async Task<ResponseResult> GetEvMeasureTech()
        {
            return await _ControlPlanDAL.GetEvMeasureTech();
        }

    }
}
