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
    public class AssetsMasterDAL
    {
        private readonly IDataLogic _IDataLogic;
        private readonly string DBConnectionString = string.Empty;
        private IDataReader? Reader;
        private readonly ConnectionStringService _connectionStringService;
        public AssetsMasterDAL(IConfiguration configuration, IDataLogic iDataLogic, ConnectionStringService connectionStringService)
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
				_ResponseResult = await _IDataLogic.ExecuteDataTable("AccSpAssetsMaster", SqlParams);
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
				_ResponseResult = await _IDataLogic.ExecuteDataTable("AccSpAssetsMaster", SqlParams);
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
				_ResponseResult = await _IDataLogic.ExecuteDataTable("AccSpAssetsMaster", SqlParams);
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
				_ResponseResult = await _IDataLogic.ExecuteDataTable("AccSpAssetsMaster", SqlParams);
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
				_ResponseResult = await _IDataLogic.ExecuteDataTable("AccSpAssetsMaster", SqlParams);
			}
			catch (Exception ex)
			{
				dynamic Error = new ExpandoObject();
				Error.Message = ex.Message;
				Error.Source = ex.Source;
			}

			return _ResponseResult;
		}
		public async Task<ResponseResult> SaveAssetsMaster(AssetsMasterModel model)
		{
			var _ResponseResult = new ResponseResult();

			try
			{
				var sqlParams = new List<dynamic>();
				var actualentDate = CommonFunc.ParseFormattedDate(model.ActualEntryDate);
				var updationDate = CommonFunc.ParseFormattedDate(model.LastupDationDate);
				if (model.Mode == "U" || model.Mode == "V")
				{ 
					//sqlParams.Add(new SqlParameter("@Flag", "UPDATE"));
					//sqlParams.Add(new SqlParameter("@DiscountCustCatEntryId", model.AssetsEntryId));
					//sqlParams.Add(new SqlParameter("@LastUpdatedbyEmpId", model.LastUpdatedbyEmpId));
					//sqlParams.Add(new SqlParameter("@LastupDationDate", updationDate));
				}
				else
				{
					sqlParams.Add(new SqlParameter("@Flag", "INSERT"));
					sqlParams.Add(new SqlParameter("@AssetsEntryId", model.AssetsEntryId));
				}

				sqlParams.Add(new SqlParameter("@AccountCode", model.AccountCode));
				sqlParams.Add(new SqlParameter("@EntryDate", model.EntryDate));
				sqlParams.Add(new SqlParameter("@AssetsCode", model.AssetsCode));
				sqlParams.Add(new SqlParameter("@AssetsName", model.AssetsName));
				sqlParams.Add(new SqlParameter("@ItemCode", model.ItemCode));
				sqlParams.Add(new SqlParameter("@AssetsCateogryId", model.AssetsCateogryId));
				sqlParams.Add(new SqlParameter("@ParentAccountCode", model.ParentAccountCode));
				sqlParams.Add(new SqlParameter("@MainGroup", model.MainGroup));
				sqlParams.Add(new SqlParameter("@SubGroup", model.SubGroup));
				sqlParams.Add(new SqlParameter("@UnderGroup", model.UnderGroup));
				sqlParams.Add(new SqlParameter("@SubSubGroup", model.SubSubGroup));
				sqlParams.Add(new SqlParameter("@CostCenterId", model.CostCenterId));
				sqlParams.Add(new SqlParameter("@Fiscalyear", model.YearCode));
				sqlParams.Add(new SqlParameter("@VendoreAccountCode", model.VendoreAccountCode));
				sqlParams.Add(new SqlParameter("@PONO", model.PONO));
				sqlParams.Add(new SqlParameter("@PODate", model.PODate));
				sqlParams.Add(new SqlParameter("@POYear", model.POYear));
				sqlParams.Add(new SqlParameter("@InvoiceNo", model.InvoiceNo));
				sqlParams.Add(new SqlParameter("@InvoiceDate", model.InvoiceDate));
				sqlParams.Add(new SqlParameter("@InvoiceYearCode", model.InvoiceYearCode));
				sqlParams.Add(new SqlParameter("@NetBookValue", model.NetBookValue));
				sqlParams.Add(new SqlParameter("@PurchaseValue", model.PurchaseValue));
				sqlParams.Add(new SqlParameter("@ResidualValue", model.ResidualValue));
				sqlParams.Add(new SqlParameter("@DepreciationMethod", model.DepreciationMethod));
				sqlParams.Add(new SqlParameter("@PurchaseNewUsed", model.PurchaseNewUsed));
				sqlParams.Add(new SqlParameter("@CountryOfOrigin", model.CountryOfOrigin));
				sqlParams.Add(new SqlParameter("@FirstAqusitionOn", model.FirstAqusitionOn));
				sqlParams.Add(new SqlParameter("@OriginalValue", model.OriginalValue));
				sqlParams.Add(new SqlParameter("@CapatalizationDate", model.CapatalizationDate));
				sqlParams.Add(new SqlParameter("@BarCode", model.BarCode));
				sqlParams.Add(new SqlParameter("@SerialNo", model.SerialNo));
				sqlParams.Add(new SqlParameter("@LocationOfInsallation", model.LocationOfInsallation));
				sqlParams.Add(new SqlParameter("@ForDepartmentId", model.ForDepartmentId));
				sqlParams.Add(new SqlParameter("@Technician", model.Technician));
				sqlParams.Add(new SqlParameter("@TechnicialcontactNo", model.TechnicialcontactNo));
				sqlParams.Add(new SqlParameter("@TechEmployeeName", model.TechEmployeeName));
				sqlParams.Add(new SqlParameter("@CustoidianEmpId", model.CustoidianEmpId));
				sqlParams.Add(new SqlParameter("@ConsiderInInvetory", model.ConsiderInInvetory));
				sqlParams.Add(new SqlParameter("@InsuranceCompany", model.InsuranceCompany));
				sqlParams.Add(new SqlParameter("@InsuredAmount", model.InsuredAmount));
				sqlParams.Add(new SqlParameter("@InsuranceDetail", model.InsuranceDetail));
				sqlParams.Add(new SqlParameter("@CC", model.CC));
				sqlParams.Add(new SqlParameter("@UID", model.UID));
				sqlParams.Add(new SqlParameter("@ActualEntryBy", model.ActualEntryByEmpId));
				sqlParams.Add(new SqlParameter("@ActualEntryDate", model.ActualEntryDate));
				sqlParams.Add(new SqlParameter("@LastupdatedBy", model.LastUpdatedbyEmpId));
				sqlParams.Add(new SqlParameter("@LastUpdatedDate", model.LastupDationDate));
				sqlParams.Add(new SqlParameter("@EntryByMachine", model.EntryByMachine));
				sqlParams.Add(new SqlParameter("@DepreciationRate", model.DepreciationRate));
				sqlParams.Add(new SqlParameter("@DepriciationAmt", model.DepriciationAmt));
				sqlParams.Add(new SqlParameter("@UseFullLifeInYear", model.UseFullLifeInYear));
				
				_ResponseResult = await _IDataLogic.ExecuteDataTable("SPSalesDiscountCustomerCategoryMaster", sqlParams);
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
