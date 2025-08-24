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
    public class ToolMoldMasterBLL: IToolMoldMaster
	{
        private ToolMoldMasterDAL _ToolMoldMasterDAL;
        private readonly IDataLogic _DataLogicDAL;

        public ToolMoldMasterBLL(IConfiguration config, IDataLogic dataLogicDAL, ConnectionStringService connectionStringService)
        {
            _ToolMoldMasterDAL = new ToolMoldMasterDAL(config, dataLogicDAL, connectionStringService);
            _DataLogicDAL = dataLogicDAL;
        }
    }
}
