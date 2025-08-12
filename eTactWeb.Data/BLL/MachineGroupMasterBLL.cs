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
    public class MachineGroupMasterBLL:IMachineGroupMaster
    {
		private MachineGroupMasterDAL _MachineGroupMasterDAL;
		private readonly IDataLogic _DataLogicDAL;

		public MachineGroupMasterBLL(IConfiguration config, IDataLogic dataLogicDAL, ConnectionStringService connectionStringService)
		{
			_MachineGroupMasterDAL = new MachineGroupMasterDAL(config, dataLogicDAL, connectionStringService);
			_DataLogicDAL = dataLogicDAL;
		}
		public async Task<ResponseResult> FillMachineGroup()
		{
			return await _MachineGroupMasterDAL.FillMachineGroup();
		}
		public async Task<ResponseResult> SaveMachineGroupMaster(MachineGroupMasterModel model)
		{
			return await _MachineGroupMasterDAL.SaveMachineGroupMaster(model);
		}
        public async Task<ResponseResult> GetDashboardData(MachineGroupMasterModel model)
        {
            return await _MachineGroupMasterDAL.GetDashboardData(model);
        }
        public async Task<MachineGroupMasterModel> GetDashboardDetailData(string FromDate, string ToDate)
        {
            return await _MachineGroupMasterDAL.GetDashboardDetailData(FromDate, ToDate);
        }
        public async Task<ResponseResult> DeleteByID(int EntryId)
        {
            return await _MachineGroupMasterDAL.DeleteByID(EntryId);
        }
        public async Task<MachineGroupMasterModel> GetViewByID(int ID)
        {
            return await _MachineGroupMasterDAL.GetViewByID(ID);
        }
        public async Task<(bool Exists, int EntryId, string MachGroup, long UId, string CC)> CheckMachineGroupExists(string machGroup)
        {
            return await _MachineGroupMasterDAL.CheckMachineGroupExists(machGroup);
        }
    }
}
