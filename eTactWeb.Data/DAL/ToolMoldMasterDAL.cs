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
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace eTactWeb.Data.DAL
{
	public class ToolMoldMasterDAL
	{
        private readonly IDataLogic _IDataLogic;
        private readonly string DBConnectionString = string.Empty;
        private IDataReader? Reader;
        private readonly ConnectionStringService _connectionStringService;
        public ToolMoldMasterDAL(IConfiguration configuration, IDataLogic iDataLogic, ConnectionStringService connectionStringService)
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
				SqlParams.Add(new SqlParameter("@Flag", "FillItemName"));
				_ResponseResult = await _IDataLogic.ExecuteDataTable("AccSpPPCToolsMaster", SqlParams);
			}
			catch (Exception ex)
			{
				dynamic Error = new ExpandoObject();
				Error.Message = ex.Message;
				Error.Source = ex.Source;
			}

			return _ResponseResult;
		}
		public async Task<ResponseResult> FillEntryId(int YearCode, string EntryDate)
		{
			var _ResponseResult = new ResponseResult();
			try
			{
				var SqlParams = new List<dynamic>();
				SqlParams.Add(new SqlParameter("@Flag", "NewEntryId"));
				SqlParams.Add(new SqlParameter("@toolyear", YearCode));
				SqlParams.Add(new SqlParameter("@EntryDate", EntryDate));
				_ResponseResult = await _IDataLogic.ExecuteDataTable("AccSpPPCToolsMaster", SqlParams);
			}
			catch (Exception ex)
			{
				dynamic Error = new ExpandoObject();
				Error.Message = ex.Message;
				Error.Source = ex.Source;
			}

			return _ResponseResult;
		}
		public async Task<ResponseResult> FillCostCenterName()
		{
			var _ResponseResult = new ResponseResult();
			try
			{
				var SqlParams = new List<dynamic>();
				SqlParams.Add(new SqlParameter("@Flag", "CostCenter"));
				_ResponseResult = await _IDataLogic.ExecuteDataTable("AccSpPPCToolsMaster", SqlParams);
			}
			catch (Exception ex)
			{
				dynamic Error = new ExpandoObject();
				Error.Message = ex.Message;
				Error.Source = ex.Source;
			}

			return _ResponseResult;
		}
		public async Task<ResponseResult> FillDepartmentName()
		{
			var _ResponseResult = new ResponseResult();
			try
			{
				var SqlParams = new List<dynamic>();
				SqlParams.Add(new SqlParameter("@Flag", "FillDepartment"));
				_ResponseResult = await _IDataLogic.ExecuteDataTable("AccSpPPCToolsMaster", SqlParams);
			}
			catch (Exception ex)
			{
				dynamic Error = new ExpandoObject();
				Error.Message = ex.Message;
				Error.Source = ex.Source;
			}

			return _ResponseResult;
		}
		public async Task<ResponseResult> FillParentAccountName()
		{
			var _ResponseResult = new ResponseResult();
			try
			{
				var SqlParams = new List<dynamic>();
				SqlParams.Add(new SqlParameter("@Flag", "FillParentGroup"));
				_ResponseResult = await _IDataLogic.ExecuteDataTable("AccSpPPCToolsMaster", SqlParams);
			}
			catch (Exception ex)
			{
				dynamic Error = new ExpandoObject();
				Error.Message = ex.Message;
				Error.Source = ex.Source;
			}

			return _ResponseResult;
		}
		public async Task<ResponseResult> FillCategoryName()
		{
			var _ResponseResult = new ResponseResult();
			try
			{
				var SqlParams = new List<dynamic>();
				SqlParams.Add(new SqlParameter("@Flag", "FillCategoryName"));
				_ResponseResult = await _IDataLogic.ExecuteDataTable("AccSpPPCToolsMaster", SqlParams);
			}
			catch (Exception ex)
			{
				dynamic Error = new ExpandoObject();
				Error.Message = ex.Message;
				Error.Source = ex.Source;
			}

			return _ResponseResult;
		}
		public async Task<ResponseResult> FillCustoidianEmpName()
		{
			var _ResponseResult = new ResponseResult();
			try
			{
				var SqlParams = new List<dynamic>();
				SqlParams.Add(new SqlParameter("@Flag", "FillCustoidianEmpName"));
				_ResponseResult = await _IDataLogic.ExecuteDataTable("AccSpPPCToolsMaster", SqlParams);
			}
			catch (Exception ex)
			{
				dynamic Error = new ExpandoObject();
				Error.Message = ex.Message;
				Error.Source = ex.Source;
			}

			return _ResponseResult;
		}

		public async Task<ResponseResult> FillParentGoupDetail(int ParentAccountCode)
		{
			var _ResponseResult = new ResponseResult();
			try
			{
				var SqlParams = new List<dynamic>();
				SqlParams.Add(new SqlParameter("@Flag", "ParentGroupDetail"));
				SqlParams.Add(new SqlParameter("@ParentAccountCode", ParentAccountCode));
				_ResponseResult = await _IDataLogic.ExecuteDataTable("AccSpPPCToolsMaster", SqlParams);
			}
			catch (Exception ex)
			{
				dynamic Error = new ExpandoObject();
				Error.Message = ex.Message;
				Error.Source = ex.Source;
			}	

			return _ResponseResult;
		}
		public async Task<ResponseResult> SaveToolMoldMaster(ToolMoldMasterModel model)
		{
			var _ResponseResult = new ResponseResult();

			try
			{
				var sqlParams = new List<dynamic>();
				var entryDate = CommonFunc.ParseFormattedDate(model.EntryDate);
				var poDate = CommonFunc.ParseFormattedDate(model.PODate);
				var invoiceDate = CommonFunc.ParseFormattedDate(model.InvoiceDate);
				var firstAcqDate = CommonFunc.ParseFormattedDate(model.FirstAqusitionOn);
				var capDate = CommonFunc.ParseFormattedDate(model.CapatalizationDate);
				var lastCalibDate = CommonFunc.ParseFormattedDate(model.LastCalibrationDate);
				var nextCalibDate = CommonFunc.ParseFormattedDate(model.NextCalibrationDate);
				var actualEntryDate = CommonFunc.ParseFormattedDate(model.ActualEntryDate);
				var lastUpdatedDate = CommonFunc.ParseFormattedDate(model.LastUpdatedDate);
				if (model.Mode == "U" || model.Mode == "V")
				{
					sqlParams.Add(new SqlParameter("@Flag", "UPDATE"));
					sqlParams.Add(new SqlParameter("@ToolEntryId", model.ToolEntryId));
					sqlParams.Add(new SqlParameter("@LastupdatedBy", model.LastupdatedBy));
					sqlParams.Add(new SqlParameter("@LastUpdatedDate", lastUpdatedDate));
				}
				else
				{
					sqlParams.Add(new SqlParameter("@Flag", "INSERT"));
					sqlParams.Add(new SqlParameter("@ToolEntryId", model.ToolEntryId));
				}

				sqlParams.Add(new SqlParameter("@AccountCode", model.AccountCode));
				sqlParams.Add(new SqlParameter("@EntryDate", entryDate));
				sqlParams.Add(new SqlParameter("@ToolOrMold", model.ToolOrMold));
				sqlParams.Add(new SqlParameter("@ToolCode", model.ToolCode));
				sqlParams.Add(new SqlParameter("@ToolName", model.ToolName));
				sqlParams.Add(new SqlParameter("@ItemCode", model.ItemCode));
				sqlParams.Add(new SqlParameter("@ToolCateogryId", model.ToolCateogryId));
				sqlParams.Add(new SqlParameter("@ConsiderAsFixedAssets", model.ConsiderAsFixedAssets));
				sqlParams.Add(new SqlParameter("@ConsiderInInvetory", model.ConsiderInInvetory));
				sqlParams.Add(new SqlParameter("@ParentAccountCode", model.ParentAccountCode));
				sqlParams.Add(new SqlParameter("@MainGroup", model.MainGroup));
				sqlParams.Add(new SqlParameter("@SubGroup", model.SubGroup));
				sqlParams.Add(new SqlParameter("@UnderGroup", model.UnderGroup));
				sqlParams.Add(new SqlParameter("@SubSubGroup", model.SubSubGroup));
				sqlParams.Add(new SqlParameter("@CostCenterId", model.CostCenterId));
				sqlParams.Add(new SqlParameter("@toolyear", model.Fiscalyear));
				sqlParams.Add(new SqlParameter("@VendoreAccountCode", model.VendoreAccountCode));
				sqlParams.Add(new SqlParameter("@PONO", model.PONO));
				sqlParams.Add(new SqlParameter("@PODate", poDate));
				sqlParams.Add(new SqlParameter("@POYear", model.POYear));
				sqlParams.Add(new SqlParameter("@InvoiceNo", model.InvoiceNo));
				sqlParams.Add(new SqlParameter("@InvoiceDate", invoiceDate));
				sqlParams.Add(new SqlParameter("@InvoiceYearCode", model.InvoiceYearCode));
				sqlParams.Add(new SqlParameter("@NetBookValue", model.NetBookValue));
				sqlParams.Add(new SqlParameter("@PurchaseValue", model.PurchaseValue));
				sqlParams.Add(new SqlParameter("@ResidualValue", model.ResidualValue));
				sqlParams.Add(new SqlParameter("@DepreciationMethod", model.DepreciationMethod));
				sqlParams.Add(new SqlParameter("@DepreciationRate", model.DepreciationRate));
				sqlParams.Add(new SqlParameter("@DepriciationAmt", model.DepriciationAmt));
				sqlParams.Add(new SqlParameter("@CountryOfOrigin", model.CountryOfOrigin));
				sqlParams.Add(new SqlParameter("@FirstAqusitionOn", firstAcqDate));
				sqlParams.Add(new SqlParameter("@OriginalValue", model.OriginalValue));
				sqlParams.Add(new SqlParameter("@CapatalizationDate", capDate));
				sqlParams.Add(new SqlParameter("@BarCode", model.BarCode));
				sqlParams.Add(new SqlParameter("@SerialNo", model.SerialNo));
				sqlParams.Add(new SqlParameter("@LocationOfInsallation", model.LocationOfInsallation));
				sqlParams.Add(new SqlParameter("@ForDepartmentId", model.ForDepartmentId));
				sqlParams.Add(new SqlParameter("@ExpectedLife", model.ExpectedLife));
				sqlParams.Add(new SqlParameter("@CalibrationRequired", model.CalibrationRequired));
				sqlParams.Add(new SqlParameter("@CalibrationFrequencyInMonth", model.CalibrationFrequencyInMonth));
				sqlParams.Add(new SqlParameter("@LastCalibrationDate", lastCalibDate));
				sqlParams.Add(new SqlParameter("@NextCalibrationDate", nextCalibDate));
				sqlParams.Add(new SqlParameter("@CalibrationAgencyId", model.CalibrationAgencyId));
				sqlParams.Add(new SqlParameter("@LastCalibrationCertificateNo", model.LastCalibrationCertificateNo));
				sqlParams.Add(new SqlParameter("@CalibrationResultPassFail", model.CalibrationResultPassFail));
				sqlParams.Add(new SqlParameter("@TolrenceRange", model.TolrenceRange));
				sqlParams.Add(new SqlParameter("@CalibrationRemark", model.CalibrationRemark));
				sqlParams.Add(new SqlParameter("@Technician", model.Technician));
				sqlParams.Add(new SqlParameter("@TechnicialcontactNo", model.TechnicialcontactNo));
				sqlParams.Add(new SqlParameter("@TechEmployeeName", model.TechEmployeeName));
				sqlParams.Add(new SqlParameter("@CustoidianEmpId", model.CustoidianEmpId));
				sqlParams.Add(new SqlParameter("@InsuranceCompany", model.InsuranceCompany));
				sqlParams.Add(new SqlParameter("@InsuredAmount", model.InsuredAmount));
				sqlParams.Add(new SqlParameter("@InsuranceDetail", model.InsuranceDetail));
				sqlParams.Add(new SqlParameter("@CC", model.CC));
				sqlParams.Add(new SqlParameter("@UID", model.UID));
				sqlParams.Add(new SqlParameter("@ActualEntryBy", model.ActualEntryBy));
				sqlParams.Add(new SqlParameter("@ActualEntryDate", actualEntryDate));
				sqlParams.Add(new SqlParameter("@EntryByMachine", model.EntryByMachine));
				
				_ResponseResult = await _IDataLogic.ExecuteDataTable("AccSpPPCToolsMaster", sqlParams);
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
