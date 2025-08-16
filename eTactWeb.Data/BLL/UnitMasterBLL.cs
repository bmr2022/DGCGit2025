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
    public class UnitMasterBLL: IUnitMaster
    {
        private UnitMasterDAL _UnitMasterDAL;
        private readonly IDataLogic _DataLogicDAL;

        public UnitMasterBLL(IConfiguration config, IDataLogic dataLogicDAL, ConnectionStringService connectionStringService)
        {
            _UnitMasterDAL = new UnitMasterDAL(config, dataLogicDAL, connectionStringService);
            _DataLogicDAL = dataLogicDAL;
        }
        public async Task<ResponseResult> GetFormRights(int ID)
        {
            return await _UnitMasterDAL.GetFormRights(ID);
        }
        public async Task<ResponseResult> SaveUnitMaster(UnitMasterModel model)
        {
            return await _UnitMasterDAL.SaveUnitMaster(model);
        }

        public async Task<ResponseResult> GetDashBoardData()
        {
            return await _UnitMasterDAL.GetDashBoardData();
        }
        public async Task<UnitMasterModel> GetDashBoardDetailData()
        {
            return await _UnitMasterDAL.GetDashBoardDetailData();
        }

        public async Task<UnitMasterModel> GetViewByID(string Unit_Name)
        {
            return await _UnitMasterDAL.GetViewByID(Unit_Name);
        }

        public async Task<ResponseResult> DeleteByID(String Unit_Name)
        {
            return await _UnitMasterDAL.DeleteByID(Unit_Name);
        }
    }
}
