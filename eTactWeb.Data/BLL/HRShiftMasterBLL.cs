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
    public  class HRShiftMasterBLL: IHRShiftMaster
    {
        private HRShiftMasterDAL _HRShiftMasterDAL;
        private readonly IDataLogic _DataLogicDAL;

        public HRShiftMasterBLL(IConfiguration config, IDataLogic dataLogicDAL)
        {
            _HRShiftMasterDAL = new HRShiftMasterDAL(config, dataLogicDAL);
            _DataLogicDAL = dataLogicDAL;
        }
        public async Task<ResponseResult> SaveHrShiftMaster(HRShiftMasterModel model)
        {
            return await _HRShiftMasterDAL.SaveHrShiftMaster(model);
        }
        public async Task<ResponseResult> GetShiftId()
        {
            return await _HRShiftMasterDAL.GetShiftId();
        }
        public async Task<ResponseResult> GetDashBoardData()
        {
            return await _HRShiftMasterDAL.GetDashBoardData();
        }
        public async Task<HRShiftMasterModel> GetDashBoardDetailData()
        {
            return await _HRShiftMasterDAL.GetDashBoardDetailData();
        }
    }
}
