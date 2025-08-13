using eTactWeb.Data.Common;
using eTactWeb.Data.DAL;
using eTactWeb.DOM.Models;
using eTactWeb.DOM.Models.Master;
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
    public class MachineMasterBLL : IMachineMaster
    {
        private MachineMasterDAL _MachineMasterDAL;
        private readonly IDataLogic _DataLogicDAL;

        public MachineMasterBLL(IConfiguration config, IDataLogic dataLogicDAL, ConnectionStringService connectionStringService)
        {
            _MachineMasterDAL = new MachineMasterDAL(config, dataLogicDAL, connectionStringService);
            _DataLogicDAL = dataLogicDAL;
        }
        public async Task<ResponseResult> FillMachineGroup()
        {
            return await _MachineMasterDAL.FillMachineGroup();
        } 
        public async Task<ResponseResult> FillMachineWorkCenter()
        {
            return await _MachineMasterDAL.FillMachineWorkCenter();
        }
        public async Task<ResponseResult> SaveMachineMaster(MachineMasterModel model)
        {
            return await _MachineMasterDAL.SaveMachineMaster(model);
        }
        public async Task<ResponseResult> GetDashBoardData()
        {
            return await _MachineMasterDAL.GetDashBoardData();
        }
        public async Task<MachineMasterModel> GetDashBoardDetailData()
        {
            return await _MachineMasterDAL.GetDashBoardDetailData();
        }
        public async Task<MachineMasterModel> GetViewByID()
        {
            return await _MachineMasterDAL.GetViewByID();
        }
        public async Task<ResponseResult> DeleteByID(int ID)
        {
            return await _MachineMasterDAL.DeleteByID(ID);
        }



        //public async Task<ResponseResult> DeleteMachine(int id)
        //{
        //    return await _MachineMasterDAL.DeleteMachine(id);
        //}

        //public async Task<MachineMasterModel> ViewByID(int id)
        //{
        //    return await _MachineMasterDAL.ViewByID(id);
        //}
    }
}
