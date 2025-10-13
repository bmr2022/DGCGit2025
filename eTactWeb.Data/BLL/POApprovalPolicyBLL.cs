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
    public class POApprovalPolicyBLL:IPOApprovalPolicy
    {
		private POApprovalPolicyDAL _POApprovalPolicyDAL;
		private readonly IDataLogic _DataLogicDAL;

		public POApprovalPolicyBLL(IConfiguration config, IDataLogic dataLogicDAL, ConnectionStringService connectionStringService)
		{
			_POApprovalPolicyDAL = new POApprovalPolicyDAL(config, dataLogicDAL, connectionStringService);
			_DataLogicDAL = dataLogicDAL;
		}
		public async Task<ResponseResult> FillItemName()
		{
			return await _POApprovalPolicyDAL.FillItemName();
		}
		public async Task<ResponseResult> FillPartCode()
		{
			return await _POApprovalPolicyDAL.FillPartCode();
		}
		public async Task<ResponseResult> FillEmpName()
		{
			return await _POApprovalPolicyDAL.FillEmpName();
		}
		public async Task<ResponseResult> FillCatName()
		{
			return await _POApprovalPolicyDAL.FillCatName();
		}
		public async Task<ResponseResult> FillGroupName()
		{
			return await _POApprovalPolicyDAL.FillGroupName();
		}
		public async Task<ResponseResult> FillEntryID()
		{
			return await _POApprovalPolicyDAL.FillEntryID();
		}

	}
}
