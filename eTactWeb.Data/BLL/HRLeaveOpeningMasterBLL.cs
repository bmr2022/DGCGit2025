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
    public class HRLeaveOpeningMasterBLL: IHRLeaveOpeningMaster
    {
        private readonly IDataLogic _DataLogicDAL;
        private readonly HRLeaveOpeningMasterDAL _HRLeaveOpeningMasterDAL;
        public HRLeaveOpeningMasterBLL(IConfiguration configuration, IDataLogic iDataLogic)
        {
            _DataLogicDAL = iDataLogic;
            _HRLeaveOpeningMasterDAL = new HRLeaveOpeningMasterDAL(configuration, iDataLogic);
        }

        public async Task<ResponseResult> GetEmpCat()
        {
            return await _HRLeaveOpeningMasterDAL.GetEmpCat();
        }

        public async Task<ResponseResult> GetLeaveName()
        {
            return await _HRLeaveOpeningMasterDAL.GetLeaveName();
        }

        public async Task<ResponseResult> GetShiftName()
        {
            return await _HRLeaveOpeningMasterDAL.GetShiftName();
        }

        public async Task<ResponseResult> GetEmpCode()
        {
            return await _HRLeaveOpeningMasterDAL.GetEmpCode();
        }

    }
}
