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
		public async Task<ResponseResult> SavePOApprovalPolicy(POApprovalPolicyModel model)
		{
			return await _POApprovalPolicyDAL.SavePOApprovalPolicy(model);
		}
        public async Task<ResponseResult> GetDashboardData(POApprovalPolicyModel model)
        {
            return await _POApprovalPolicyDAL.GetDashboardData(model);
        }
        public async Task<POApprovalPolicyModel> GetDashboardDetailData(string FromDate, string ToDate, string ReportType, string GroupName, string CateName, string ItemName)
        {
            return await _POApprovalPolicyDAL.GetDashboardDetailData(FromDate, ToDate, ReportType, GroupName, CateName, ItemName);
        }
        public async Task<ResponseResult> DeleteByID(int EntryId, string EntryDate, int EntryByempId)
        {
            return await _POApprovalPolicyDAL.DeleteByID(EntryId, EntryDate, EntryByempId);
        }
        public async Task<POApprovalPolicyModel> GetViewByID(int ID, int YC, string FromDate, string ToDate)
        {
            return await _POApprovalPolicyDAL.GetViewByID(ID, YC, FromDate, ToDate);
        }
        public async Task<ResponseResult> FillItems(string SearchItemCode)
        {
            return await _POApprovalPolicyDAL.FillItems(SearchItemCode);
        }
		 public async Task<ResponseResult> FillGroups(string SearchGroupName)
        {
            return await _POApprovalPolicyDAL.FillGroups(SearchGroupName);
        }
		public async Task<ResponseResult> FillCateName(string SearchCatName)
        {
            return await _POApprovalPolicyDAL.FillCateName(SearchCatName);
        }
        public async Task<ResponseResult> CheckGroupExists(string GroupName)
        {
            return await _POApprovalPolicyDAL.CheckGroupExists(GroupName);
        }

    }
}
