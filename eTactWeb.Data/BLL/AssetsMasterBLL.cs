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
    public class AssetsMasterBLL:IAssetsMaster
    {
        private AssetsMasterDAL _AssetsMasterDAL;
        private readonly IDataLogic _DataLogicDAL;

        public AssetsMasterBLL(IConfiguration config, IDataLogic dataLogicDAL, ConnectionStringService connectionStringService)
        {
            _AssetsMasterDAL = new AssetsMasterDAL(config, dataLogicDAL, connectionStringService);
            _DataLogicDAL = dataLogicDAL;
        }
		public async Task<ResponseResult> FillItemName()
		{
			return await _AssetsMasterDAL.FillItemName();
		}
        public async Task<ResponseResult> FillCostCenterName()
		{
			return await _AssetsMasterDAL.FillCostCenterName();
		}
        public async Task<ResponseResult> FillDepartmentName()
		{
			return await _AssetsMasterDAL.FillDepartmentName();
		}

	}
}
