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
                    sqlParams.Add(new SqlParameter("@Flag", "UPDATE"));
                }
                else
                {
                    sqlParams.Add(new SqlParameter("@Flag", "INSERT"));
                }
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
        public async Task<ResponseResult> GetDashboardData(SalesPersonTransferModel model)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                var Flag = "";
                SqlParams.Add(new SqlParameter("@Flag", "DASHBOARD"));
                //SqlParams.Add(new SqlParameter("@REportType", model.ReportType));
                //SqlParams.Add(new SqlParameter("@Fromdate", model.FromDate));
                //SqlParams.Add(new SqlParameter("@Todate", model.ToDate));

                _ResponseResult = await _IDataLogic.ExecuteDataSet("SPSalesPersonTransfer", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }
        public async Task<SalesPersonTransferModel> GetDashboardDetailData(string FromDate, string ToDate, string ReportType)
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
                    oCmd.Parameters.AddWithValue("@Flag", "DASHBOARD");
                    //oCmd.Parameters.AddWithValue("@REportType", ReportType);
                    //oCmd.Parameters.AddWithValue("@Fromdate", FromDate);
                    //oCmd.Parameters.AddWithValue("@Todate", ToDate);

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
                                                            SalesPersTransfEntryId = dr["SalesPersTransfEntryId"] != DBNull.Value ? Convert.ToInt32(dr["SalesPersTransfEntryId"]) : 0,
                                                            SalesPersTransfYearCode = dr["SalesPersTransfYearCode"] != DBNull.Value ? Convert.ToInt32(dr["SalesPersTransfYearCode"]) : 0,
                                                            SalesPersTransfSlipNo = dr["SalesPersTransfSlipNo"] != DBNull.Value ? Convert.ToString(dr["SalesPersTransfSlipNo"]) : string.Empty,
                                                            SalesPersTransfEntryDate = dr["SalesPersTransfEntryDate"] != DBNull.Value ? Convert.ToString(dr["SalesPersTransfEntryDate"]) : string.Empty,
                                                            
                                                            RevNo = dr["RevNo"] != DBNull.Value ? Convert.ToInt32(dr["RevNo"]) : 0,
                                                            NewSalesEmpId = dr["NewSalesEmpId"] != DBNull.Value ? Convert.ToInt32(dr["NewSalesEmpId"]) : 0,
                                                            NewSalesEmpName = dr["NewSalesEmpName"] != DBNull.Value ? Convert.ToString(dr["NewSalesEmpName"]) : string.Empty,
                                                            OldSalesEmpId = dr["OldSalesEmpId"] != DBNull.Value ? Convert.ToInt32(dr["OldSalesEmpId"]) : 0,
                                                            OldSalesEmpName = dr["OldSalesEmpName"] != DBNull.Value ? Convert.ToString(dr["OldSalesEmpName"]) : string.Empty,
                                                            NewSalesPersEffdate = dr["NewSalesPersEffdate"] != DBNull.Value ? Convert.ToString(dr["NewSalesPersEffdate"]) : string.Empty,
                                                            OldSalesPersTillDate = dr["OldSalesPersTillDate"] != DBNull.Value ? Convert.ToString(dr["OldSalesPersTillDate"]) : string.Empty,
                                                            
                                                            AccountCode = dr["AccountCode"] != DBNull.Value ? Convert.ToInt32(dr["AccountCode"]) : 0,
                                                            CustomerName = dr["Account_Name"] != DBNull.Value ? Convert.ToString(dr["Account_Name"]) : string.Empty,
                                                            EntryByEmpId = dr["EntryByEmpId"] != DBNull.Value ? Convert.ToInt32(dr["EntryByEmpId"]) : 0,
                                                            UpdatedByEmpId = dr["UpdatedByEmpId"] != DBNull.Value ? Convert.ToInt32(dr["UpdatedByEmpId"]) : 0,
                                                            //UpdationDate = dr["UpdationDate"] != DBNull.Value ? Convert.ToDateTime(dr["UpdationDate"]) : DateTime.MinValue,
                                                            UpdationDate = dr["UpdationDate"] != DBNull.Value ? Convert.ToString(dr["UpdationDate"]) : string.Empty,
                                                            EntryByMachine = dr["EntryByMachine"] != DBNull.Value ? Convert.ToString(dr["EntryByMachine"]) : string.Empty,
                                                            CC = dr["CC"] != DBNull.Value ? Convert.ToString(dr["CC"]) : string.Empty,

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

        public async Task<ResponseResult> DeleteByID(int EntryId, int YearCode, string EntryDate, int EntryByempId)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var entrydate = CommonFunc.ParseFormattedDate(EntryDate);
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "DELETE"));
                SqlParams.Add(new SqlParameter("@SalesPersTransfEntryId", EntryId));
                SqlParams.Add(new SqlParameter("@SalesPersTransfYearCode", YearCode));
                //SqlParams.Add(new SqlParameter("@ActualEntryDate", EntryDate));
                //SqlParams.Add(new SqlParameter("@ActualEntryDate", DateTime.ParseExact(EntryDate, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("dd/MMM/yyyy")));
                
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
        public async Task<SalesPersonTransferModel> GetViewByID(int ID, int YC, string FromDate, string ToDate)
        {
            var model = new SalesPersonTransferModel();
            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@flag", "VIEWBYID"));
                SqlParams.Add(new SqlParameter("@SalesPersTransfEntryId", ID));
                SqlParams.Add(new SqlParameter("@SalesPersTransfYearCode", YC));
                var _ResponseResult = await _IDataLogic.ExecuteDataSet("SPSalesPersonTransfer", SqlParams);

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
        private static SalesPersonTransferModel PrepareView(DataSet DS, ref SalesPersonTransferModel? model)
        {
            try
            {
                var ItemList = new List<SalesPersonTransferModel>();
                var DetailList = new List<SalesPersonTransferModel>();
                DS.Tables[0].TableName = "SalesPersonTransfer";
                //DS.Tables[1].TableName = "MaterialConversionDetail";
                int cnt = 0;

                model.SalesPersTransfEntryId = Convert.ToInt32(DS.Tables[0].Rows[0]["SalesPersTransfEntryId"].ToString());
                model.SalesPersTransfYearCode = Convert.ToInt32(DS.Tables[0].Rows[0]["SalesPersTransfYearCode"].ToString());
                model.SalesPersTransfSlipNo = DS.Tables[0].Rows[0]["SalesPersTransfSlipNo"].ToString();
                model.SalesPersTransfEntryDate = DS.Tables[0].Rows[0]["SalesPersTransfEntryDate"].ToString();
                model.RevNo = Convert.ToInt32(DS.Tables[0].Rows[0]["RevNo"].ToString());
                model.NewSalesEmpId = Convert.ToInt32(DS.Tables[0].Rows[0]["NewSalesEmpId"].ToString());
                model.NewSalesEmpName = DS.Tables[0].Rows[0]["NewSalesEmpName"].ToString();
                model.OldSalesEmpId = Convert.ToInt32(DS.Tables[0].Rows[0]["OldSalesEmpId"].ToString());
                model.OldSalesEmpName = DS.Tables[0].Rows[0]["OldSalesEmpName"].ToString();
                model.NewSalesPersEffdate = DS.Tables[0].Rows[0]["NewSalesPersEffdate"].ToString();
                model.OldSalesPersTillDate = DS.Tables[0].Rows[0]["OldSalesPersTillDate"].ToString();
                model.AccountCode = Convert.ToInt32(DS.Tables[0].Rows[0]["AccountCode"].ToString());
                model.CustomerName = DS.Tables[0].Rows[0]["Account_Name"].ToString();
                model.EntryByEmpId = Convert.ToInt32(DS.Tables[0].Rows[0]["EntryByEmpId"].ToString());
                model.UpdatedByEmpId = Convert.ToInt32(DS.Tables[0].Rows[0]["UpdatedByEmpId"].ToString());
                model.UpdationDate = DS.Tables[0].Rows[0]["UpdationDate"].ToString();
                model.EntryByMachine = DS.Tables[0].Rows[0]["EntryByMachine"].ToString();
                model.CC = DS.Tables[0].Rows[0]["CC"].ToString();


                return model;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
