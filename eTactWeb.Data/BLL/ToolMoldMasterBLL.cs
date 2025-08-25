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
    public class ToolMoldMasterBLL: IToolMoldMaster
	{
        private ToolMoldMasterDAL _ToolMoldMasterDAL;
        private readonly IDataLogic _DataLogicDAL;

        public ToolMoldMasterBLL(IConfiguration config, IDataLogic dataLogicDAL, ConnectionStringService connectionStringService)
        {
            _ToolMoldMasterDAL = new ToolMoldMasterDAL(config, dataLogicDAL, connectionStringService);
            _DataLogicDAL = dataLogicDAL;
        }
		public async Task<ResponseResult> FillItemName()
		{
			return await _ToolMoldMasterDAL.FillItemName();
		}
		public async Task<ResponseResult> FillEntryId(int YearCode, string EntryDate)
		{
			return await _ToolMoldMasterDAL.FillEntryId(YearCode, EntryDate);
		}
		public async Task<ResponseResult> FillCostCenterName()
		{
			return await _ToolMoldMasterDAL.FillCostCenterName();
		}
		public async Task<ResponseResult> FillDepartmentName()
		{
			return await _ToolMoldMasterDAL.FillDepartmentName();
		}
		public async Task<ResponseResult> FillParentAccountName()
		{
			return await _ToolMoldMasterDAL.FillParentAccountName();
		}
		public async Task<ResponseResult> FillCategoryName()
		{
			return await _ToolMoldMasterDAL.FillCategoryName();
		}
		public async Task<ResponseResult> FillCustoidianEmpName()
		{
			return await _ToolMoldMasterDAL.FillCustoidianEmpName();
		}

		public async Task<ResponseResult> FillParentGoupDetail(int ParentAccountCode)
		{
			return await _ToolMoldMasterDAL.FillParentGoupDetail(ParentAccountCode);
		}
		public async Task<ResponseResult> SaveToolMoldMaster(ToolMoldMasterModel model)
		{
			return await _ToolMoldMasterDAL.SaveToolMoldMaster(model);
		}
	}
}
