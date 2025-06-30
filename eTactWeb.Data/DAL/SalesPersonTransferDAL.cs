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
    public class SalesPersonTransferDAL
    {
		private readonly IDataLogic _IDataLogic;
		private readonly string DBConnectionString = string.Empty;
		private IDataReader? Reader;
		private readonly ConnectionStringService _connectionStringService;
		public SalesPersonTransferDAL(IConfiguration configuration, IDataLogic iDataLogic, ConnectionStringService connectionStringService)
		{
			_connectionStringService = connectionStringService;
			DBConnectionString = _connectionStringService.GetConnectionString();
			_IDataLogic = iDataLogic;
		}
		public async Task<ResponseResult> FillNewSalesEmpName(string ShowAllEmp)
		{
			var _ResponseResult = new ResponseResult();
			try
			{
				var SqlParams = new List<dynamic>();

				SqlParams.Add(new SqlParameter("@Flag", "FillNewSalesPerson"));
				SqlParams.Add(new SqlParameter("@ShowAllEmp", ShowAllEmp));

				_ResponseResult = await _IDataLogic.ExecuteDataTable("SPSalesPersonTransfer", SqlParams);
			}
			catch (Exception ex)
			{
				dynamic Error = new ExpandoObject();
				Error.Message = ex.Message;
				Error.Source = ex.Source;
			}

			return _ResponseResult;
		}
		public async Task<ResponseResult> FillOldSalesEmpName(string ShowAllEmp)
		{
			var _ResponseResult = new ResponseResult();
			try
			{
				var SqlParams = new List<dynamic>();

				SqlParams.Add(new SqlParameter("@Flag", "FillOldSalesPerson"));
				SqlParams.Add(new SqlParameter("@ShowAllEmp", ShowAllEmp));

				_ResponseResult = await _IDataLogic.ExecuteDataTable("SPSalesPersonTransfer", SqlParams);
			}
			catch (Exception ex)
			{
				dynamic Error = new ExpandoObject();
				Error.Message = ex.Message;
				Error.Source = ex.Source;
			}

			return _ResponseResult;
		}
        public async Task<SalesPersonTransferModel> FillCustomerList(string ShowAllCust)
        {
            DataSet? oDataSet = new DataSet();
            var model = new SalesPersonTransferModel();
            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    SqlCommand oCmd = new SqlCommand("SPSalesPersonTransfer", myConnection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    oCmd.Parameters.AddWithValue("@Flag", "FillCustomerList");
                    oCmd.Parameters.AddWithValue("@ShowAllCust", ShowAllCust);
                    await myConnection.OpenAsync();
                    using (SqlDataAdapter oDataAdapter = new SqlDataAdapter(oCmd))
                    {
                        oDataAdapter.Fill(oDataSet);
                    }
                }
                if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                {
                    model.SalesPersonTransferGrid = (from DataRow dr in oDataSet.Tables[0].Rows
                                      select new SalesPersonTransferModel
                                      {
                                          CustomerName = dr["CustomerName"] != DBNull.Value ? Convert.ToString(dr["CustomerName"]) : string.Empty,
                                          CustAddress = dr["ComAddress"] != DBNull.Value ? Convert.ToString(dr["ComAddress"]) : string.Empty,
                                          CustCity = dr["City"] != DBNull.Value ? Convert.ToString(dr["City"]) : string.Empty,
                                          CustState = dr["StateName"] != DBNull.Value ? Convert.ToString(dr["StateName"]) : string.Empty,
                                          CustStateCode = dr["State"] != DBNull.Value ? Convert.ToString(dr["State"]) : string.Empty,
                                          CustNameCode = dr["Account_Code"] != DBNull.Value ? Convert.ToString(dr["Account_Code"]) : string.Empty,
                                          
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
