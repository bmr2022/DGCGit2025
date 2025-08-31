using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eTactWeb.Data.DAL
{
    public class AccDepriciationCalculationdetailDAL
	{
		private readonly IDataLogic _IDataLogic;
		private readonly string DBConnectionString = string.Empty;
		private IDataReader? Reader;
		private readonly ConnectionStringService _connectionStringService;
		public AccDepriciationCalculationdetailDAL(IConfiguration configuration, IDataLogic iDataLogic, ConnectionStringService connectionStringService)
		{
			_connectionStringService = connectionStringService;
			DBConnectionString = _connectionStringService.GetConnectionString();
			_IDataLogic = iDataLogic;
		}
		public async Task<AccDepriciationCalculationdetailModel> GetAssets()
		{
			DataSet? oDataSet = new DataSet();
			var model = new AccDepriciationCalculationdetailModel();
			try
			{
				using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
				{
					SqlCommand oCmd = new SqlCommand("AccSPDepriciationCalculationMainDetail", myConnection)
					{
						CommandType = CommandType.StoredProcedure
					};
					oCmd.Parameters.AddWithValue("@Flag", "GetaAssetsData");

					await myConnection.OpenAsync();
					using (SqlDataAdapter oDataAdapter = new SqlDataAdapter(oCmd))
					{
						oDataAdapter.Fill(oDataSet);
					}
				}
				if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
				{

					model.AccDepriciationCalculationdetailGrid = (from DataRow dr in oDataSet.Tables[0].Rows
										  select new AccDepriciationCalculationdetailModel
										  {
											  AssetsName = dr["AssetsName"] != DBNull.Value ? Convert.ToString(dr["AssetsName"]) : "",
											  AccountCode = dr["AccountCode"] != DBNull.Value ? Convert.ToInt32(dr["AccountCode"]) : 0,
											  AccountName = dr["VendorName"] != DBNull.Value ? Convert.ToString(dr["VendorName"]) : "",
											  MainGroup = dr["MainGroup"] != DBNull.Value ? Convert.ToString(dr["MainGroup"]) : "",
											  ParentAccountCode = dr["ParentAccountCode"] != DBNull.Value ? Convert.ToInt32(dr["ParentAccountCode"]) : 0,
											  ParentAccountName = dr["ParentAccountName"] != DBNull.Value ? Convert.ToString(dr["ParentAccountName"]) : "",
											  SubGroup = dr["SubGroup"] != DBNull.Value ? Convert.ToString(dr["SubGroup"]) : "",
											  UnderGroup = dr["UnderGroup"] != DBNull.Value ? Convert.ToString(dr["UnderGroup"]) : "",
											  SubSubGroup = dr["SubSubGroup"] != DBNull.Value ? Convert.ToString(dr["SubSubGroup"]) : "",
											  OriginalNetBookValue = dr["OriginalValue"] != DBNull.Value ? Convert.ToDecimal(dr["OriginalValue"]) : 0,
											  DepreciationMethod = dr["DepreciationMethod"] != DBNull.Value ? Convert.ToString(dr["DepreciationMethod"]) : "",
											  DepreciationRate = dr["DepreciationRate"] != DBNull.Value ? Convert.ToDecimal(dr["DepreciationRate"]) : 0,
											  AfterDepriciationNetValue = dr["DepriciationAmt"] != DBNull.Value ? Convert.ToDecimal(dr["DepriciationAmt"]) : 0,

                                              RemainingUseFullLifeInYear = dr["UseFullLifeInYear"] != DBNull.Value ? Convert.ToDecimal(dr["UseFullLifeInYear"]) : 0
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
