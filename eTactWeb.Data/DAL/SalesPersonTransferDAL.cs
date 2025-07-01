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
        public async Task<ResponseResult> FillEntryID(int YearCode)
		{
			var _ResponseResult = new ResponseResult();
			try
			{
				var SqlParams = new List<dynamic>();

				SqlParams.Add(new SqlParameter("@Flag", "NewEntryId"));
				SqlParams.Add(new SqlParameter("@SalesPersTransfYearCode", YearCode));

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
		public async Task<ResponseResult> SaveSalesPersonTransfer(SalesPersonTransferModel model, DataTable GIGrid)
		{
			var _ResponseResult = new ResponseResult();


			try
			{
				var sqlParams = new List<dynamic>();
				if (model.Mode == "U" || model.Mode == "V")
				{
					//sqlParams.Add(new SqlParameter("@Flag", "UPDATE"));
					//sqlParams.Add(new SqlParameter("@Flag", "UPDATE"));
					//sqlParams.Add(new SqlParameter("@MatConvEntryId", model.EntryId));
					//sqlParams.Add(new SqlParameter("@YearCode", model.OpeningYearCode));
					//sqlParams.Add(new SqlParameter("@MatConvSlipNo", model.SlipNo));
					//sqlParams.Add(new SqlParameter("@MatConvSlipDate", model.SlipDate));
					//sqlParams.Add(new SqlParameter("@StoreWorkcenter", model.IssueToStoreWC));
					//sqlParams.Add(new SqlParameter("@Remarks", model.Remarks));
					//sqlParams.Add(new SqlParameter("@ApprovedBy", model.ApprovedBy));
					//sqlParams.Add(new SqlParameter("@Uid", model.Uid));
					//sqlParams.Add(new SqlParameter("@cc", model.cc));
					//sqlParams.Add(new SqlParameter("@ActualEntryByEmpid", model.ActualEntryByEmpid));
					//sqlParams.Add(new SqlParameter("@ActualEntryDate", model.ActualEntryDate));
					//sqlParams.Add(new SqlParameter("@UpdatedByEmpId", model.UpdatedByEmpId));
					//sqlParams.Add(new SqlParameter("@UpdationDate", model.UpdationDate));
					//sqlParams.Add(new SqlParameter("@EntryByMachine", model.EntryByMachine));

					//sqlParams.Add(new SqlParameter("@dt", GIGrid));
				}
				else
				{
					sqlParams.Add(new SqlParameter("@Flag", "INSERT"));
					sqlParams.Add(new SqlParameter("@SalesPersTransfEntryId", model.SalesPersTransfEntryId));
					sqlParams.Add(new SqlParameter("@SalesPersTransfYearCode", model.SalesPersTransfYearCode));
					sqlParams.Add(new SqlParameter("@SalesPersTransfSlipNo", model.SalesPersTransfSlipNo));//not null
					sqlParams.Add(new SqlParameter("@SalesPersTransfEntryDate", model.SalesPersTransfEntryDate));
					sqlParams.Add(new SqlParameter("@RevNo", model.RevNo));
					sqlParams.Add(new SqlParameter("@NewSalesEmpId", model.NewSalesEmpId));
					sqlParams.Add(new SqlParameter("@OldSalesEmpId", model.OldSalesEmpId));
					sqlParams.Add(new SqlParameter("@NewSalesPersEffdate", model.EffFrom));
					sqlParams.Add(new SqlParameter("@OldSalesPersTillDate", model.EffTillDate));
					//sqlParams.Add(new SqlParameter("@AccountCode", model.CustNameCode));//null
					sqlParams.Add(new SqlParameter("@EntryByEmpId", model.EntryByEmpId));//null
					sqlParams.Add(new SqlParameter("@UpdatedByEmpId", model.UpdatedByEmpId));
					sqlParams.Add(new SqlParameter("@UpdationDate", model.UpdationDate));
					sqlParams.Add(new SqlParameter("@CC", model.CC));//null
					sqlParams.Add(new SqlParameter("@EntryByMachine", model.EntryByMachine));

					sqlParams.Add(new SqlParameter("@dt", GIGrid));
				}

				_ResponseResult = await _IDataLogic.ExecuteDataTable("SPSalesPersonTransfer", sqlParams);

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
