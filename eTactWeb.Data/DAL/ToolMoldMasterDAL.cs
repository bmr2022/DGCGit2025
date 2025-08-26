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
        public async Task<ResponseResult> GetDashboardData(ToolMoldMasterModel model)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "DASHBOARD"));
                SqlParams.Add(new SqlParameter("@fromDate", model.FromDate));
                SqlParams.Add(new SqlParameter("@toDate", model.ToDate));

                _ResponseResult = await _IDataLogic.ExecuteDataSet("AccSpPPCToolsMaster", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }
        public async Task<ToolMoldMasterModel> GetDashboardDetailData(string FromDate, string ToDate, string ToolName)
        {
            DataSet? oDataSet = new DataSet();
            var model = new ToolMoldMasterModel();
            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    SqlCommand oCmd = new SqlCommand("AccSpPPCToolsMaster", myConnection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    oCmd.Parameters.AddWithValue("@Flag", "DASHBOARD");
                    oCmd.Parameters.AddWithValue("@fromDate", FromDate);
                    oCmd.Parameters.AddWithValue("@toDate", ToDate);
                    oCmd.Parameters.AddWithValue("@ToolName", ToolName);

                    await myConnection.OpenAsync();
                    using (SqlDataAdapter oDataAdapter = new SqlDataAdapter(oCmd))
                    {
                        oDataAdapter.Fill(oDataSet);
                    }
                }
                if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                {

                    model.ToolMoldGrid = (from DataRow dr in oDataSet.Tables[0].Rows
                                              select new ToolMoldMasterModel
                                              {
                                                  ToolEntryId = dr["ToolEntryId"] != DBNull.Value ? Convert.ToInt32(dr["ToolEntryId"]) : 0,
                                                  AccountCode = dr["AccountCode"] != DBNull.Value ? Convert.ToInt32(dr["AccountCode"]) : 0,
                                                  EntryDate = dr["EntryDate"] != DBNull.Value ? Convert.ToString(dr["EntryDate"]) : "",
                                                  ToolOrMold = dr["ToolOrMold"] != DBNull.Value ? Convert.ToString(dr["ToolOrMold"]) : "",
                                                  ToolCode = dr["ToolCode"] != DBNull.Value ? Convert.ToString(dr["ToolCode"]) : "",
                                                  ToolName = dr["ToolName"] != DBNull.Value ? Convert.ToString(dr["ToolName"]) : "",
                                                  ItemCode = dr["ItemCode"] != DBNull.Value ? Convert.ToInt32(dr["ItemCode"]) : 0,
                                                  ItemName = dr["Item_Name"] != DBNull.Value ? Convert.ToString(dr["Item_Name"]) : "",
                                                  ToolCateogryId = dr["ToolCateogryId"] != DBNull.Value ? Convert.ToInt32(dr["ToolCateogryId"]) : 0,
                                                  ConsiderAsFixedAssets = dr["ConsiderAsFixedAssets"] != DBNull.Value ? Convert.ToString(dr["ConsiderAsFixedAssets"]) : "",
                                                  ConsiderInInvetory = dr["ConsiderInInvetory"] != DBNull.Value ? Convert.ToString(dr["ConsiderInInvetory"]) : "",
                                                  ParentAccountCode = dr["ParentAccountCode"] != DBNull.Value ? Convert.ToInt32(dr["ParentAccountCode"]) : 0,
                                                  ParentAccountName = dr["ParentAccountName"] != DBNull.Value ? Convert.ToString(dr["ParentAccountName"]) : "",
                                                  MainGroup = dr["MainGroup"] != DBNull.Value ? Convert.ToString(dr["MainGroup"]) : "",
                                                  SubGroup = dr["SubGroup"] != DBNull.Value ? Convert.ToString(dr["SubGroup"]) : "",
                                                  UnderGroup = dr["UnderGroup"] != DBNull.Value ? Convert.ToString(dr["UnderGroup"]) : "",
                                                  SubSubGroup = dr["SubSubGroup"] != DBNull.Value ? Convert.ToInt32(dr["SubSubGroup"]) : 0,
                                                  CostCenterId = dr["CostCenterId"] != DBNull.Value ? Convert.ToInt32(dr["CostCenterId"]) : 0,
                                                  Fiscalyear = dr["ToolYear"] != DBNull.Value ? Convert.ToInt32(dr["ToolYear"]) : 0,
                                                  VendoreAccountCode = dr["VendoreAccountCode"] != DBNull.Value ? Convert.ToInt32(dr["VendoreAccountCode"]) : 0,
                                                  VendoreAccountName = dr["VendorName"] != DBNull.Value ? Convert.ToString(dr["VendorName"]) : "",
                                                  CostCenterName = dr["CostCenterName"] != DBNull.Value ? Convert.ToString(dr["CostCenterName"]) : "",
                                                  PONO = dr["PONO"] != DBNull.Value ? Convert.ToString(dr["PONO"]) : "",
                                                  PODate = dr["PODate"] != DBNull.Value ? Convert.ToString(dr["PODate"]) : "",
                                                  POYear = dr["POYear"] != DBNull.Value ? Convert.ToInt32(dr["POYear"]) : 0,
                                                  InvoiceNo = dr["InvoiceNo"] != DBNull.Value ? Convert.ToString(dr["InvoiceNo"]) : "",
                                                  InvoiceDate = dr["InvoiceDate"] != DBNull.Value ? Convert.ToString(dr["InvoiceDate"]) : "",
                                                  InvoiceYearCode = dr["InvoiceYearCode"] != DBNull.Value ? Convert.ToInt32(dr["InvoiceYearCode"]) : 0,
                                                  NetBookValue = dr["NetBookValue"] != DBNull.Value ? Convert.ToDecimal(dr["NetBookValue"]) : 0,
                                                  PurchaseValue = dr["PurchaseValue"] != DBNull.Value ? Convert.ToDecimal(dr["PurchaseValue"]) : 0,
                                                  ResidualValue = dr["ResidualValue"] != DBNull.Value ? Convert.ToDecimal(dr["ResidualValue"]) : 0,
                                                  DepreciationMethod = dr["DepreciationMethod"] != DBNull.Value ? Convert.ToString(dr["DepreciationMethod"]) : "",
                                                  DepreciationRate = dr["DepreciationRate"] != DBNull.Value ? Convert.ToDecimal(dr["DepreciationRate"]) : 0,
                                                  DepriciationAmt = dr["DepriciationAmt"] != DBNull.Value ? Convert.ToDecimal(dr["DepriciationAmt"]) : 0,
                                                  CountryOfOrigin = dr["CountryOfOrigin"] != DBNull.Value ? Convert.ToString(dr["CountryOfOrigin"]) : "",
                                                  FirstAqusitionOn = dr["FirstAqusitionOn"] != DBNull.Value ? Convert.ToString(dr["FirstAqusitionOn"]) : "",
                                                  OriginalValue = dr["OriginalValue"] != DBNull.Value ? Convert.ToDecimal(dr["OriginalValue"]) : 0,
                                                  CapatalizationDate = dr["CapatalizationDate"] != DBNull.Value ? Convert.ToString(dr["CapatalizationDate"]) : "",
                                                  BarCode = dr["BarCode"] != DBNull.Value ? Convert.ToString(dr["BarCode"]) : "",
                                                  SerialNo = dr["SerialNo"] != DBNull.Value ? Convert.ToString(dr["SerialNo"]) : "",
                                                  LocationOfInsallation = dr["LocationOfInsallation"] != DBNull.Value ? Convert.ToString(dr["LocationOfInsallation"]) : "",
                                                  ForDepartmentId = dr["ForDepartmentId"] != DBNull.Value ? Convert.ToInt32(dr["ForDepartmentId"]) : 0,
                                                  ExpectedLife = dr["ExpectedLife"] != DBNull.Value ? Convert.ToInt32(dr["ExpectedLife"]) : 0,
                                                  CalibrationRequired = dr["CalibrationRequired"] != DBNull.Value ? Convert.ToString(dr["CalibrationRequired"]) : "",
                                                  CalibrationFrequencyInMonth = dr["CalibrationFrequencyInMonth"] != DBNull.Value ? Convert.ToInt32(dr["CalibrationFrequencyInMonth"]) : 0,
                                                  LastCalibrationDate = dr["LastCalibrationDate"] != DBNull.Value ? Convert.ToString(dr["LastCalibrationDate"]) : "",
                                                  NextCalibrationDate = dr["NextCalibrationDate"] != DBNull.Value ? Convert.ToString(dr["NextCalibrationDate"]) : "",
                                                  CalibrationAgencyId = dr["CalibrationAgencyId"] != DBNull.Value ? Convert.ToInt32(dr["CalibrationAgencyId"]) : 0,
                                                  LastCalibrationCertificateNo = dr["LastCalibrationCertificateNo"] != DBNull.Value ? Convert.ToString(dr["LastCalibrationCertificateNo"]) : "",
                                                  CalibrationResultPassFail = dr["CalibrationResultPassFail"] != DBNull.Value ? Convert.ToString(dr["CalibrationResultPassFail"]) : "",
                                                  TolrenceRange = dr["TolrenceRange"] != DBNull.Value ? Convert.ToString(dr["TolrenceRange"]) : "",
                                                  CalibrationRemark = dr["CalibrationRemark"] != DBNull.Value ? Convert.ToString(dr["CalibrationRemark"]) : "",
                                                  Technician = dr["Technician"] != DBNull.Value ? Convert.ToString(dr["Technician"]) : "",
                                                  TechnicialcontactNo = dr["TechnicialcontactNo"] != DBNull.Value ? Convert.ToString(dr["TechnicialcontactNo"]) : "",
                                                  TechEmployeeName = dr["TechEmployeeName"] != DBNull.Value ? Convert.ToString(dr["TechEmployeeName"]) : "",
                                                  CustoidianEmpId = dr["CustoidianEmpId"] != DBNull.Value ? Convert.ToInt32(dr["CustoidianEmpId"]) : 0,
                                                  InsuranceCompany = dr["InsuranceCompany"] != DBNull.Value ? Convert.ToString(dr["InsuranceCompany"]) : "",
                                                  InsuredAmount = dr["InsuredAmount"] != DBNull.Value ? Convert.ToDecimal(dr["InsuredAmount"]) : 0,
                                                  InsuranceDetail = dr["InsuranceDetail"] != DBNull.Value ? Convert.ToString(dr["InsuranceDetail"]) : "",
                                                  CC = dr["CC"] != DBNull.Value ? Convert.ToString(dr["CC"]) : "",
                                                  UID = dr["UID"] != DBNull.Value ? Convert.ToString(dr["UID"]) : "",
                                                  ActualEntryBy = dr["ActualEntryBy"] != DBNull.Value ? Convert.ToInt32(dr["ActualEntryBy"]) : 0,
                                                  ActualEntryDate = dr["ActualEntryDate"] != DBNull.Value ? Convert.ToString(dr["ActualEntryDate"]) : "",
                                                  LastupdatedBy = dr["LastUpdatedBy"] != DBNull.Value ? Convert.ToInt32(dr["LastUpdatedBy"]) : 0,
                                                  LastUpdatedDate = dr["LastUpdatedDate"] != DBNull.Value ? Convert.ToString(dr["LastUpdatedDate"]) : "",
                                                  LastUpdatedbyEmpName = dr["UpdatedByEmployee"] != DBNull.Value ? Convert.ToString(dr["UpdatedByEmployee"]) : "",
                                                  EntryByMachine = dr["EntryByMachine"] != DBNull.Value ? Convert.ToString(dr["EntryByMachine"]) : "",
                                                  CustoidianEmpName = dr["CustodiaEmployee"] != DBNull.Value ? Convert.ToString(dr["CustodiaEmployee"]) : "",
                                                  ActualEntryByEmpName = dr["ActualEntryByEmployee"] != DBNull.Value ? Convert.ToString(dr["ActualEntryByEmployee"]) : "",

                                              }).ToList();


                }

            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            finally
            {
                oDataSet.Dispose();
            }
            return model;
        }
    }
}
