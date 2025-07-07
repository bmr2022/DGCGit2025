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
    public class InProcessInspectionDAL
    {
        private readonly IDataLogic _IDataLogic;
        private readonly string DBConnectionString = string.Empty;
        private IDataReader? Reader;
        private readonly ConnectionStringService _connectionStringService;
        public InProcessInspectionDAL(IConfiguration configuration, IDataLogic iDataLogic, ConnectionStringService connectionStringService)
        {
            _connectionStringService = connectionStringService;
            DBConnectionString = _connectionStringService.GetConnectionString();
            _IDataLogic = iDataLogic;
        }
		public async Task<ResponseResult> FillEntryID(int YearCode)
		{
			var _ResponseResult = new ResponseResult();
			try
			{
				var SqlParams = new List<dynamic>();
				SqlParams.Add(new SqlParameter("@Flag", "NewEntryId"));
				SqlParams.Add(new SqlParameter("@InspYearCode", YearCode));
				_ResponseResult = await _IDataLogic.ExecuteDataTable("SPInprocessInpection", SqlParams);
			}
			catch (Exception ex)
			{
				dynamic Error = new ExpandoObject();
				Error.Message = ex.Message;
				Error.Source = ex.Source;
			}

			return _ResponseResult;
		}
		public async Task<ResponseResult> FillPartCode(string InspectionType)
		{
			var _ResponseResult = new ResponseResult();
			try
			{
				var SqlParams = new List<dynamic>();
				SqlParams.Add(new SqlParameter("@Flag", "FillPartCode"));
				SqlParams.Add(new SqlParameter("@InprocessOutIncoming", InspectionType));
				_ResponseResult = await _IDataLogic.ExecuteDataTable("SPInprocessInpection", SqlParams);
			}
			catch (Exception ex)
			{
				dynamic Error = new ExpandoObject();
				Error.Message = ex.Message;
				Error.Source = ex.Source;
			}

			return _ResponseResult;
		}
		public async Task<ResponseResult> FillItemName()
		{
			var _ResponseResult = new ResponseResult();
			try
			{
				var SqlParams = new List<dynamic>();
				SqlParams.Add(new SqlParameter("@Flag", "FillItemName"));
				_ResponseResult = await _IDataLogic.ExecuteDataTable("SPInprocessInpection", SqlParams);
			}
			catch (Exception ex)
			{
				dynamic Error = new ExpandoObject();
				Error.Message = ex.Message;
				Error.Source = ex.Source;
			}

			return _ResponseResult;
		}
		public async Task<ResponseResult> FillMachineName()
		{
			var _ResponseResult = new ResponseResult();
			try
			{
				var SqlParams = new List<dynamic>();
				SqlParams.Add(new SqlParameter("@Flag", "FillMachineName"));
				_ResponseResult = await _IDataLogic.ExecuteDataTable("SPInprocessInpection", SqlParams);
			}
			catch (Exception ex)
			{
				dynamic Error = new ExpandoObject();
				Error.Message = ex.Message;
				Error.Source = ex.Source;
			}

			return _ResponseResult;
		}
		public async Task<ResponseResult> FillShift()
		{
			var _ResponseResult = new ResponseResult();
			try
			{
				var SqlParams = new List<dynamic>();
				SqlParams.Add(new SqlParameter("@Flag", "FillShift"));
				_ResponseResult = await _IDataLogic.ExecuteDataTable("SPInprocessInpection", SqlParams);
			}
			catch (Exception ex)
			{
				dynamic Error = new ExpandoObject();
				Error.Message = ex.Message;
				Error.Source = ex.Source;
			}

			return _ResponseResult;
		}
		public async Task<ResponseResult> FillCustomer()
		{
			var _ResponseResult = new ResponseResult();
			try
			{
				var SqlParams = new List<dynamic>();
				SqlParams.Add(new SqlParameter("@Flag", "FillCustomer"));
				_ResponseResult = await _IDataLogic.ExecuteDataTable("SPInprocessInpection", SqlParams);
			}
			catch (Exception ex)
			{
				dynamic Error = new ExpandoObject();
				Error.Message = ex.Message;
				Error.Source = ex.Source;
			}

			return _ResponseResult;
		}
		public async Task<ResponseResult> FillColor(string PartNo)
		{
			var _ResponseResult = new ResponseResult();
			try
			{
				var SqlParams = new List<dynamic>();
				SqlParams.Add(new SqlParameter("@Flag", "FillColor"));
				SqlParams.Add(new SqlParameter("@Itemcode", PartNo));
				_ResponseResult = await _IDataLogic.ExecuteDataTable("SPInprocessInpection", SqlParams);
			}
			catch (Exception ex)
			{
				dynamic Error = new ExpandoObject();
				Error.Message = ex.Message;
				Error.Source = ex.Source;
			}

			return _ResponseResult;
		}
		public async Task<InProcessInspectionModel> GetInprocessInspectionGridData(int ItemCode, int SampleSize)
		{
			DataSet? oDataSet = new DataSet();
			var model = new InProcessInspectionModel();
			try
			{
				using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
				{
					SqlCommand oCmd = new SqlCommand("SPInprocessInpection", myConnection)
					{
						CommandType = CommandType.StoredProcedure
					};
					oCmd.Parameters.AddWithValue("@Flag", "FillCharectristicDetail");
					oCmd.Parameters.AddWithValue("@Itemcode", ItemCode);

					await myConnection.OpenAsync();
					using (SqlDataAdapter oDataAdapter = new SqlDataAdapter(oCmd))
					{
						oDataAdapter.Fill(oDataSet);
					}
				}
				if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
				{
					
						model.DTSSGrid = (from DataRow dr in oDataSet.Tables[0].Rows
														select new InProcessInspectionDetailModel
														{
															Characteristic = dr["Characteristic"] != DBNull.Value ? Convert.ToString(dr["Characteristic"]) : string.Empty,
															EvalutionMeasurmentTechnique = dr["EvalutionMeasurmentTechnique"] != DBNull.Value ? Convert.ToString(dr["EvalutionMeasurmentTechnique"]) : string.Empty,
															ControlMethod = dr["ControlMethod"] != DBNull.Value ? Convert.ToString(dr["ControlMethod"]) : string.Empty,
															SpecificationFrom = dr["SpecificationFrom"] != DBNull.Value ? Convert.ToString(dr["SpecificationFrom"]) : string.Empty,
															SpecificationTo = dr["SpecificationTo"] != DBNull.Value ? Convert.ToString(dr["SpecificationTo"]) : string.Empty,
															
															RejectionPlan = dr["RejectionPlan"] != DBNull.Value ? Convert.ToString(dr["RejectionPlan"]) : string.Empty,
															FrequencyofTesting = dr["FrequencyofTesting"] != DBNull.Value ? Convert.ToString(dr["FrequencyofTesting"]) : string.Empty,

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
