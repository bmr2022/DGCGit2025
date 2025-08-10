using eTactWeb.Data.Common;
using eTactWeb.Data.DAL;
using eTactWeb.Services.Interface;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
	}
}
