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
		public async Task<ResponseResult> FillEntryId(int YearCode, string EntryDate)
		{
			var _ResponseResult = new ResponseResult();
			try
			{
				var SqlParams = new List<dynamic>();
				SqlParams.Add(new SqlParameter("@Flag", "NewEntryId"));
				SqlParams.Add(new SqlParameter("@Fiscalyear", YearCode));
				SqlParams.Add(new SqlParameter("@EntryDate", EntryDate));
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
        public async Task<ResponseResult> FillCategoryName()
		{
			var _ResponseResult = new ResponseResult();
			try
			{
				var SqlParams = new List<dynamic>();
				SqlParams.Add(new SqlParameter("@Flag", "FillCategoryName"));
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
        public async Task<ResponseResult> FillCustoidianEmpName()
		{
			var _ResponseResult = new ResponseResult();
			try
			{
				var SqlParams = new List<dynamic>();
				SqlParams.Add(new SqlParameter("@Flag", "FillCustoidianEmpName"));
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
				var cptDate = CommonFunc.ParseFormattedDate(model.CapatalizationDate);
				var fsteqDate = CommonFunc.ParseFormattedDate(model.FirstAqusitionOn);
				var invDate = CommonFunc.ParseFormattedDate(model.InvoiceDate);
				var PODate = CommonFunc.ParseFormattedDate(model.PODate);
				var entDate = CommonFunc.ParseFormattedDate(model.EntryDate);
				if (model.Mode == "U" || model.Mode == "V")
				{
                    sqlParams.Add(new SqlParameter("@Flag", "UPDATE"));
                    sqlParams.Add(new SqlParameter("@AssetsEntryId", model.AssetsEntryId));
                    sqlParams.Add(new SqlParameter("@LastupdatedBy", model.LastUpdatedbyEmpId));
                    sqlParams.Add(new SqlParameter("@LastUpdatedDate", updationDate));
                }
				else
				{
					sqlParams.Add(new SqlParameter("@Flag", "INSERT"));
					sqlParams.Add(new SqlParameter("@AssetsEntryId", model.AssetsEntryId));
				}

				sqlParams.Add(new SqlParameter("@AccountCode", model.AccountCode));
				sqlParams.Add(new SqlParameter("@EntryDate",entDate));
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
				sqlParams.Add(new SqlParameter("@PODate", PODate));
				sqlParams.Add(new SqlParameter("@POYear", model.POYear));
				sqlParams.Add(new SqlParameter("@InvoiceNo", model.InvoiceNo));
				sqlParams.Add(new SqlParameter("@InvoiceDate",invDate));
				sqlParams.Add(new SqlParameter("@InvoiceYearCode", model.InvoiceYearCode));
				sqlParams.Add(new SqlParameter("@NetBookValue", model.NetBookValue));
				sqlParams.Add(new SqlParameter("@PurchaseValue", model.PurchaseValue));
				sqlParams.Add(new SqlParameter("@ResidualValue", model.ResidualValue));
				sqlParams.Add(new SqlParameter("@DepreciationMethod", model.DepreciationMethod));
				sqlParams.Add(new SqlParameter("@PurchaseNewUsed", model.PurchaseNewUsed));
				sqlParams.Add(new SqlParameter("@CountryOfOrigin", model.CountryOfOrigin));
                sqlParams.Add(new SqlParameter("@FirstAqusitionOn",
    string.IsNullOrEmpty(fsteqDate) ? " " :fsteqDate));
                sqlParams.Add(new SqlParameter("@OriginalValue", model.OriginalValue));
				sqlParams.Add(new SqlParameter("@CapatalizationDate",cptDate));
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
				sqlParams.Add(new SqlParameter("@ActualEntryDate", actualentDate));
				sqlParams.Add(new SqlParameter("@EntryByMachine", model.EntryByMachine));
				sqlParams.Add(new SqlParameter("@DepreciationRate", model.DepreciationRate));
				sqlParams.Add(new SqlParameter("@DepriciationAmt", model.DepriciationAmt));
				sqlParams.Add(new SqlParameter("@UseFullLifeInYear", model.UseFullLifeInYear));
				
				_ResponseResult = await _IDataLogic.ExecuteDataTable("AccSpAssetsMaster", sqlParams);
			}
			catch (Exception ex)
			{
				_ResponseResult.StatusCode = HttpStatusCode.InternalServerError;
				_ResponseResult.StatusText = "Error";
				_ResponseResult.Result = new { ex.Message, ex.StackTrace };
			}

			return _ResponseResult;
		}

        public async Task<ResponseResult> GetDashboardData(AssetsMasterModel model)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "DASHBOARD"));
                SqlParams.Add(new SqlParameter("@fromDate", model.FromDate));
                SqlParams.Add(new SqlParameter("@toDate", model.ToDate));

                _ResponseResult = await _IDataLogic.ExecuteDataSet("AccSpAssetsMaster", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }
        public async Task<AssetsMasterModel> GetDashboardDetailData(string FromDate, string ToDate)
        {
            DataSet? oDataSet = new DataSet();
            var model = new AssetsMasterModel();
            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    SqlCommand oCmd = new SqlCommand("AccSpAssetsMaster", myConnection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    oCmd.Parameters.AddWithValue("@Flag", "DASHBOARD");
                    oCmd.Parameters.AddWithValue("@fromDate", FromDate);
                    oCmd.Parameters.AddWithValue("@toDate", ToDate);

                    await myConnection.OpenAsync();
                    using (SqlDataAdapter oDataAdapter = new SqlDataAdapter(oCmd))
                    {
                        oDataAdapter.Fill(oDataSet);
                    }
                }
                if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                {

                    model.AssetsMasterGrid = (from DataRow dr in oDataSet.Tables[0].Rows
                                                                select new AssetsMasterModel
                                                                {
                                                                    AssetsEntryId = dr["AssetsEntryId"] != DBNull.Value ? Convert.ToInt32(dr["AssetsEntryId"]) : 0,
                                                                    AccountCode = dr["AccountCode"] != DBNull.Value ? Convert.ToInt32(dr["AccountCode"]) : 0,
                                                                    EntryDate = dr["EntryDate"] != DBNull.Value ? Convert.ToString(dr["EntryDate"]) : "",
                                                                    AssetsCode = dr["AssetsCode"] != DBNull.Value ? Convert.ToString(dr["AssetsCode"]) : string.Empty,
                                                                    AssetsName = dr["AssetsName"] != DBNull.Value ? Convert.ToString(dr["AssetsName"]) : string.Empty,
                                                                    ItemCode = dr["ItemCode"] != DBNull.Value ? Convert.ToInt32(dr["ItemCode"]) : 0,
                                                                    AssetsCateogryId = dr["AssetsCateogryId"] != DBNull.Value ? Convert.ToInt32(dr["AssetsCateogryId"]) : 0,
                                                                    ParentAccountName = dr["ParentAccountName"] != DBNull.Value ? Convert.ToString(dr["ParentAccountName"]) : string.Empty,
                                                                    ParentAccountCode = dr["ParentAccountCode"] != DBNull.Value ? Convert.ToInt32(dr["ParentAccountCode"]) : 0,
                                                                    MainGroup = dr["MainGroup"] != DBNull.Value ? Convert.ToString(dr["MainGroup"]) : string.Empty,
                                                                    SubGroup = dr["SubGroup"] != DBNull.Value ? Convert.ToString(dr["SubGroup"]) : string.Empty,
                                                                    UnderGroup = dr["UnderGroup"] != DBNull.Value ? Convert.ToString(dr["UnderGroup"]) : string.Empty,
                                                                    CostCenterName = dr["CostCenterName"] != DBNull.Value ? Convert.ToString(dr["CostCenterName"]) : string.Empty,
                                                                    SubSubGroup = dr["SubSubGroup"] != DBNull.Value ? Convert.ToInt32(dr["SubSubGroup"]) : 0,
                                                                    CostCenterId = dr["CostCenterId"] != DBNull.Value ? Convert.ToInt32(dr["CostCenterId"]) : 0,
                                                                    FiscalYear = dr["Fiscalyear"] != DBNull.Value ? Convert.ToInt32(dr["Fiscalyear"]) : 0,
                                                                    //Vend = dr["VendorName"] != DBNull.Value ? Convert.ToString(dr["VendorName"]) : string.Empty,
                                                                    VendoreAccountCode = dr["VendoreAccountCode"] != DBNull.Value ? Convert.ToInt32(dr["VendoreAccountCode"]) : 0,
                                                                    PONO = dr["PONO"] != DBNull.Value ? Convert.ToString(dr["PONO"]) : string.Empty,
                                                                    PODate = dr["PODate"] != DBNull.Value ? Convert.ToString(dr["PODate"]) : "",
                                                                    POYear = dr["POYear"] != DBNull.Value ? Convert.ToInt32(dr["POYear"]) : 0,
                                                                    InvoiceNo = dr["InvoiceNo"] != DBNull.Value ? Convert.ToString(dr["InvoiceNo"]) : string.Empty,
                                                                    InvoiceDate = dr["InvoiceDate"] != DBNull.Value ? Convert.ToString(dr["InvoiceDate"]) : "",
                                                                    InvoiceYearCode = dr["InvoiceYearCode"] != DBNull.Value ? Convert.ToInt32(dr["InvoiceYearCode"]) : 0,
                                                                    NetBookValue = dr["NetBookValue"] != DBNull.Value ? Convert.ToDecimal(dr["NetBookValue"]) : 0,
                                                                    PurchaseValue = dr["PurchaseValue"] != DBNull.Value ? Convert.ToDecimal(dr["PurchaseValue"]) : 0,
                                                                    ResidualValue = dr["ResidualValue"] != DBNull.Value ? Convert.ToDecimal(dr["ResidualValue"]) : 0,
                                                                    DepreciationMethod = dr["DepreciationMethod"] != DBNull.Value ? Convert.ToString(dr["DepreciationMethod"]) : string.Empty,
                                                                    DepartmentName = dr["DeptName"] != DBNull.Value ? Convert.ToString(dr["DeptName"]) : string.Empty,
                                                                    PurchaseNewUsed = dr["PurchaseNewUsed"] != DBNull.Value ? Convert.ToString(dr["PurchaseNewUsed"]) : string.Empty,
                                                                    CountryOfOrigin = dr["CountryOfOrigin"] != DBNull.Value ? Convert.ToString(dr["CountryOfOrigin"]) : string.Empty,
                                                                    FirstAqusitionOn = dr["FirstAqusitionOn"] != DBNull.Value ? Convert.ToString(dr["FirstAqusitionOn"]) : "",
                                                                    OriginalValue = dr["OriginalValue"] != DBNull.Value ? Convert.ToDecimal(dr["OriginalValue"]) : 0,
                                                                    CapatalizationDate = dr["CapatalizationDate"] != DBNull.Value ? Convert.ToString(dr["CapatalizationDate"]) : "",
                                                                    BarCode = dr["BarCode"] != DBNull.Value ? Convert.ToString(dr["BarCode"]) : string.Empty,
                                                                    SerialNo = dr["SerialNo"] != DBNull.Value ? Convert.ToString(dr["SerialNo"]) : string.Empty,
                                                                    LocationOfInsallation = dr["LocationOfInsallation"] != DBNull.Value ? Convert.ToString(dr["LocationOfInsallation"]) : string.Empty,
                                                                    ForDepartmentId = dr["ForDepartmentId"] != DBNull.Value ? Convert.ToInt32(dr["ForDepartmentId"]) : 0,
                                                                    Technician = dr["Technician"] != DBNull.Value ? Convert.ToString(dr["Technician"]) : string.Empty,
                                                                    TechnicialcontactNo = dr["TechnicialcontactNo"] != DBNull.Value ? Convert.ToString(dr["TechnicialcontactNo"]) : string.Empty,
                                                                    TechEmployeeName = dr["TechEmployeeName"] != DBNull.Value ? Convert.ToString(dr["TechEmployeeName"]) : string.Empty,
                                                                    //Custo = dr["CustodiaEmployee"] != DBNull.Value ? Convert.ToString(dr["CustodiaEmployee"]) : string.Empty,
                                                                    CustoidianEmpId = dr["CustoidianEmpId"] != DBNull.Value ? Convert.ToInt32(dr["CustoidianEmpId"]) : 0,
                                                                    ConsiderInInvetory = dr["ConsiderInInvetory"] != DBNull.Value ? Convert.ToString(dr["ConsiderInInvetory"]) : string.Empty,
                                                                    InsuranceCompany = dr["InsuranceCompany"] != DBNull.Value ? Convert.ToString(dr["InsuranceCompany"]) : string.Empty,
                                                                    InsuredAmount = dr["InsuredAmount"] != DBNull.Value ? Convert.ToDecimal(dr["InsuredAmount"]) : 0,
                                                                    InsuranceDetail = dr["InsuranceDetail"] != DBNull.Value ? Convert.ToString(dr["InsuranceDetail"]) : string.Empty,
                                                                    CC = dr["CC"] != DBNull.Value ? Convert.ToString(dr["CC"]) : string.Empty,
                                                                    UID = dr["UID"] != DBNull.Value ? Convert.ToInt32(dr["UID"]) : 0,
                                                                    ActualEntryBy = dr["ActualEntryBy"] != DBNull.Value ? Convert.ToInt32(dr["ActualEntryBy"]) : 0,
                                                                    ActualEntryByEmpName = dr["ActualEntryByEmployee"] != DBNull.Value ? Convert.ToString(dr["ActualEntryByEmployee"]) : string.Empty,
                                                                    ActualEntryDate = dr["ActualEntryDate"] != DBNull.Value ? Convert.ToString(dr["ActualEntryDate"]) : "",
                                                                    LastupdatedBy = dr["LastupdatedBy"] != DBNull.Value ? Convert.ToInt32(dr["LastupdatedBy"]) : 0,
                                                                    LastUpdatedDate = dr["LastUpdatedDate"] != DBNull.Value ? Convert.ToString(dr["LastUpdatedDate"]) : "",
                                                                    EntryByMachine = dr["EntryByMachine"] != DBNull.Value ? Convert.ToString(dr["EntryByMachine"]) : string.Empty,
                                                                    LastUpdatedbyEmpName = dr["UpdatedByEmployee"] != DBNull.Value ? Convert.ToString(dr["UpdatedByEmployee"]) : string.Empty,

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
        public async Task<ResponseResult> DeleteByID(int EntryId, int YearCode)
        {
            var _ResponseResult = new ResponseResult();
            try
            {

                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "DELETE"));
                SqlParams.Add(new SqlParameter("@AssetsEntryId", EntryId));
                SqlParams.Add(new SqlParameter("@Fiscalyear", YearCode));
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
        public async Task<AssetsMasterModel> GetViewByID(int AssetsEntryId, int Fiscalyear)
        {
            var model = new AssetsMasterModel();
            try
            {
                var SqlParams = new List<dynamic>
        {
            new SqlParameter("@flag", "VIEWBYID"),
            new SqlParameter("@AssetsEntryId", AssetsEntryId),
            new SqlParameter("@Fiscalyear", Fiscalyear)
        };

                var _ResponseResult = await _IDataLogic.ExecuteDataSet("AccSpAssetsMaster", SqlParams);

                if (_ResponseResult.Result != null && _ResponseResult.StatusCode == HttpStatusCode.OK && _ResponseResult.StatusText == "Success")
                {
                    PrepareView(_ResponseResult.Result, ref model);
                }
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return model;
        }

        private static AssetsMasterModel PrepareView(DataSet DS, ref AssetsMasterModel? model)
        {
            try
            {
                DS.Tables[0].TableName = "AssetsMaster";
                var row = DS.Tables[0].Rows[0];

                model.AssetsEntryId = Convert.ToInt32(row["AssetsEntryId"]);
                model.AccountCode = Convert.ToInt32(row["AccountCode"]);
                model.EntryDate = row["EntryDate"].ToString();
                model.AssetsCode = row["AssetsCode"].ToString();
                model.AssetsName = row["AssetsName"].ToString();
                model.ItemCode = Convert.ToInt32(row["ItemCode"]);
                model.AssetsCateogryId = Convert.ToInt32(row["AssetsCateogryId"]);
                model.ParentAccountCode = Convert.ToInt32(row["ParentAccountCode"]);
                model.MainGroup = row["MainGroup"].ToString();
                model.SubGroup = row["SubGroup"].ToString();
                model.UnderGroup = row["UnderGroup"].ToString();
                model.SubSubGroup = Convert.ToInt32(row["SubSubGroup"].ToString());
                model.CostCenterId = Convert.ToInt32(row["CostCenterId"]);
                model.FiscalYear = Convert.ToInt32(row["Fiscalyear"]);
                model.VendoreAccountCode = Convert.ToInt32(row["VendoreAccountCode"]);
                model.PONO = row["PONO"].ToString();
                model.PODate = row["PODate"].ToString();
                model.POYear = Convert.ToInt32(row["POYear"]);
                model.InvoiceNo = row["InvoiceNo"].ToString();
                model.InvoiceDate = row["InvoiceDate"].ToString();
                model.InvoiceYearCode = Convert.ToInt32(row["InvoiceYearCode"]);
                model.NetBookValue = Convert.ToDecimal(row["NetBookValue"]);
                model.PurchaseValue = Convert.ToDecimal(row["PurchaseValue"]);
                model.ResidualValue = Convert.ToDecimal(row["ResidualValue"]);
                model.DepreciationMethod = row["DepreciationMethod"].ToString();
                model.PurchaseNewUsed = row["PurchaseNewUsed"].ToString();
                model.CountryOfOrigin = row["CountryOfOrigin"].ToString();
                model.FirstAqusitionOn = row["FirstAqusitionOn"].ToString();
                model.OriginalValue = Convert.ToDecimal(row["OriginalValue"]);
                model.CapatalizationDate = row["CapatalizationDate"].ToString();
                model.BarCode = row["BarCode"].ToString();
                model.SerialNo = row["SerialNo"].ToString();
                model.LocationOfInsallation = row["LocationOfInsallation"].ToString();
                model.ForDepartmentId = Convert.ToInt32(row["ForDepartmentId"]);
                model.Technician = row["Technician"].ToString();
                model.TechnicialcontactNo = row["TechnicialcontactNo"].ToString();
                model.TechEmployeeName = row["TechEmployeeName"].ToString();
                model.CustoidianEmpId = Convert.ToInt32(row["CustoidianEmpId"]);
                model.ConsiderInInvetory = row["ConsiderInInvetory"].ToString();
                model.InsuranceCompany = row["InsuranceCompany"].ToString();
                model.InsuredAmount = Convert.ToDecimal(row["InsuredAmount"]);
                model.InsuranceDetail = row["InsuranceDetail"].ToString();
                model.CC = row["CC"].ToString();
                model.UID = Convert.ToInt32(row["UID"]);
                model.ActualEntryBy = Convert.ToInt32(row["ActualEntryBy"]);
                model.ActualEntryDate = row["ActualEntryDate"].ToString();
                model.LastupdatedBy = Convert.ToInt32(row["LastupdatedBy"]);
                model.LastUpdatedDate = row["LastUpdatedDate"].ToString();
                model.EntryByMachine = row["EntryByMachine"].ToString();
                model.DepreciationRate = Convert.ToDecimal(row["DepreciationRate"]);
                model.DepriciationAmt = Convert.ToDecimal(row["DepriciationAmt"]);
                model.UseFullLifeInYear = Convert.ToInt32(row["UseFullLifeInYear"]);

                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}
