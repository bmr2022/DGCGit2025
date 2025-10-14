using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Data.DAL
{
    public class POApprovalPolicyDAL
    {
		private readonly IDataLogic _IDataLogic;
		private readonly string DBConnectionString = string.Empty;
		private IDataReader? Reader;
		private readonly ConnectionStringService _connectionStringService;
		public POApprovalPolicyDAL(IConfiguration configuration, IDataLogic iDataLogic, ConnectionStringService connectionStringService)
		{
			_connectionStringService = connectionStringService;
			DBConnectionString = _connectionStringService.GetConnectionString();
			_IDataLogic = iDataLogic;
		}
		public async Task<ResponseResult> FillItemName()
		{
			var _ResponseResult = new ResponseResult();
			try
			{
				var SqlParams = new List<dynamic>();
				SqlParams.Add(new SqlParameter("@Flag", "FillPartCodeItemName"));
				_ResponseResult = await _IDataLogic.ExecuteDataTable("SpPOApprovalPolicy", SqlParams);
			}
			catch (Exception ex)
			{
				dynamic Error = new ExpandoObject();
				Error.Message = ex.Message;
				Error.Source = ex.Source;
			}

			return _ResponseResult;
		}
		public async Task<ResponseResult> FillPartCode()
		{
			var _ResponseResult = new ResponseResult();
			try
			{
				var SqlParams = new List<dynamic>();
				SqlParams.Add(new SqlParameter("@Flag", "FillPartCodeItemName"));
				_ResponseResult = await _IDataLogic.ExecuteDataTable("SpPOApprovalPolicy", SqlParams);
			}
			catch (Exception ex)
			{
				dynamic Error = new ExpandoObject();
				Error.Message = ex.Message;
				Error.Source = ex.Source;
			}

			return _ResponseResult;
		}
		public async Task<ResponseResult> FillEmpName()
		{
			var _ResponseResult = new ResponseResult();
			try
			{
				var SqlParams = new List<dynamic>();
				SqlParams.Add(new SqlParameter("@Flag", "FillEmployeeList"));
				_ResponseResult = await _IDataLogic.ExecuteDataTable("SpPOApprovalPolicy", SqlParams);
			}
			catch (Exception ex)
			{
				dynamic Error = new ExpandoObject();
				Error.Message = ex.Message;
				Error.Source = ex.Source;
			}

			return _ResponseResult;
		}
		public async Task<ResponseResult> FillCatName()
		{
			var _ResponseResult = new ResponseResult();
			try
			{
				var SqlParams = new List<dynamic>();
				SqlParams.Add(new SqlParameter("@Flag", "FillCategory"));
				_ResponseResult = await _IDataLogic.ExecuteDataTable("SpPOApprovalPolicy", SqlParams);
			}
			catch (Exception ex)
			{
				dynamic Error = new ExpandoObject();
				Error.Message = ex.Message;
				Error.Source = ex.Source;
			}

			return _ResponseResult;
		}
		public async Task<ResponseResult> FillGroupName()
		{
			var _ResponseResult = new ResponseResult();
			try
			{
				var SqlParams = new List<dynamic>();
				SqlParams.Add(new SqlParameter("@Flag", "FillGroup"));
				_ResponseResult = await _IDataLogic.ExecuteDataTable("SpPOApprovalPolicy", SqlParams);
			}
			catch (Exception ex)
			{
				dynamic Error = new ExpandoObject();
				Error.Message = ex.Message;
				Error.Source = ex.Source;
			}

			return _ResponseResult;
		}
		public async Task<ResponseResult> FillEntryID()
		{
			var _ResponseResult = new ResponseResult();
			try
			{
				var SqlParams = new List<dynamic>();
				SqlParams.Add(new SqlParameter("@Flag", "NewEntryId"));
				_ResponseResult = await _IDataLogic.ExecuteDataTable("SpPOApprovalPolicy", SqlParams);
			}
			catch (Exception ex)
			{
				dynamic Error = new ExpandoObject();
				Error.Message = ex.Message;
				Error.Source = ex.Source;
			}

			return _ResponseResult;
		}
		public async Task<ResponseResult> SavePOApprovalPolicy(POApprovalPolicyModel model)
		{
			var _ResponseResult = new ResponseResult();

			try
			{
				var sqlParams = new List<dynamic>();

				
				sqlParams.Add(new SqlParameter("@POTYPE", model.POTYPE));
				sqlParams.Add(new SqlParameter("@ItemGroupId", model.GroupCode));
				sqlParams.Add(new SqlParameter("@ItemCategoryId", model.CatId));
				sqlParams.Add(new SqlParameter("@Itemcode", model.Itemcode));
				sqlParams.Add(new SqlParameter("@FromAmt", model.FromAmt));
				sqlParams.Add(new SqlParameter("@ToAmt", model.ToAmt));
				sqlParams.Add(new SqlParameter("@FirstApprovalRequired", model.FirstApprovalRequired));
				sqlParams.Add(new SqlParameter("@FinalApprovalRequired1", model.FinalApprovalRequired1));
				sqlParams.Add(new SqlParameter("@OnlyDirectorApproval", model.OnlyDirectorApproval));
				sqlParams.Add(new SqlParameter("@ONLY1stApprovalRequired", model.ONLY1stApprovalRequired));
				sqlParams.Add(new SqlParameter("@OnlyFinalApprovalRequired", model.OnlyFinalApprovalRequired));
				sqlParams.Add(new SqlParameter("@All3ApprovalRequired", model.All3ApprovalRequired));
				sqlParams.Add(new SqlParameter("@EmpidForFirstApproval1", model.EmpidForFirstApproval1));
				sqlParams.Add(new SqlParameter("@EmpidForFirstApproval2", model.EmpidForFirstApproval2));
				sqlParams.Add(new SqlParameter("@EmpidForFirstApproval3", model.EmpidForFirstApproval3));
				sqlParams.Add(new SqlParameter("@EmpidForFinalApproval1", model.EmpidForFinalApproval1));
				sqlParams.Add(new SqlParameter("@EmpidForFinalApproval2", model.EmpidForFinalApproval2));
				sqlParams.Add(new SqlParameter("@EmpidForFinalApproval3", model.EmpidForFinalApproval3));
				sqlParams.Add(new SqlParameter("@EmpidForMgmtApproval1", model.EmpidForMgmtApproval1));
				sqlParams.Add(new SqlParameter("@EmpidForMgmtApproval12", model.EmpidForMgmtApproval12));
				sqlParams.Add(new SqlParameter("@EmpidForMgmtApproval13", model.EmpidForMgmtApproval13));
				sqlParams.Add(new SqlParameter("@EntryByMachine", model.EntryByMachine ?? Environment.MachineName));
				sqlParams.Add(new SqlParameter("@ActualEntryByEmpId", model.ActualEntryByEmpId));
				sqlParams.Add(new SqlParameter("@ActualEntryDate", model.ActualEntryDate));
				sqlParams.Add(new SqlParameter("@CC", model.CC));

				if (model.Mode == "U") 
				{
					sqlParams.Add(new SqlParameter("@Flag", "Update"));
					sqlParams.Add(new SqlParameter("@POApprovalEntryId", model.POApprovalEntryId));
					sqlParams.Add(new SqlParameter("@LastUpdatedByEmpId", model.LastUpdatedByEmpId));
					sqlParams.Add(new SqlParameter("@LastUpdatedDate", model.LastUpdatedDate));
				}
				else 
				{
					sqlParams.Add(new SqlParameter("@Flag", "Insert"));
					sqlParams.Add(new SqlParameter("@POApprovalEntryId", model.EntryId));
				}

				
				_ResponseResult = await _IDataLogic.ExecuteDataTable("SpPOApprovalPolicy", sqlParams);
			}
			catch (Exception ex)
			{
				_ResponseResult.StatusCode = HttpStatusCode.InternalServerError;
				_ResponseResult.StatusText = "Error";
				_ResponseResult.Result = new { ex.Message, ex.StackTrace };
			}

			return _ResponseResult;
		}


	}
}
