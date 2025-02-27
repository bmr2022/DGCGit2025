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
    public  class CostCenterMasterBLL:ICostCenterMaster
    {
        private CostCenterMasterDAL _CostCenterMasterDAL;
        private readonly IDataLogic _DataLogicDAL;

        public CostCenterMasterBLL(IConfiguration config, IDataLogic dataLogicDAL)
        {
            _CostCenterMasterDAL = new CostCenterMasterDAL(config, dataLogicDAL);
            _DataLogicDAL = dataLogicDAL;

        }
        public async Task<ResponseResult> FillCostCenterID()
        {
            return await _CostCenterMasterDAL.FillCostCenterID();
        } 
        public async Task<ResponseResult> FillCostCenterGroupName()
        {
            return await _CostCenterMasterDAL.FillCostCenterGroupName();
        }
        public async Task<ResponseResult> FillDeptName()
        {
            return await _CostCenterMasterDAL.FillDeptName();
        }
        public async Task<ResponseResult> FillUnderGroupName()
        {
            return await _CostCenterMasterDAL.FillUnderGroupName();
        }
        public async Task<ResponseResult> SaveCostCenterMaster(CostCenterMasterModel model)
        {
            return await _CostCenterMasterDAL.SaveCostCenterMaster(model);
        }
        public async Task<ResponseResult> GetDashBoardData()
        {
            return await _CostCenterMasterDAL.GetDashBoardData();
        }
        public async Task<CostCenterMasterModel> GetDashBoardDetailData()
        {
            return await _CostCenterMasterDAL.GetDashBoardDetailData();
        }
        public async Task<CostCenterMasterModel> GetViewByID(int ID)
        {
            return await _CostCenterMasterDAL.GetViewByID(ID);
        }
        public async Task<ResponseResult> DeleteByID(int ID)
        {
            return await _CostCenterMasterDAL.DeleteByID(ID);
        } 
        public async Task<ResponseResult> ChkForDuplicate(string CostCenterName)
        {
            return await _CostCenterMasterDAL.ChkForDuplicate(CostCenterName);
        }
    }
}
