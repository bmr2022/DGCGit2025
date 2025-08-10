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
    public class PartyItemGroupDiscountDAL
    {
		private readonly IDataLogic _IDataLogic;
		private readonly string DBConnectionString = string.Empty;
		private IDataReader? Reader;
		private readonly ConnectionStringService _connectionStringService;
		public PartyItemGroupDiscountDAL(IConfiguration configuration, IDataLogic iDataLogic, ConnectionStringService connectionStringService)
		{
			_connectionStringService = connectionStringService;
			DBConnectionString = _connectionStringService.GetConnectionString();
			_IDataLogic = iDataLogic;
		}
		public async Task<ResponseResult> FillPartyName()
		{
			var _ResponseResult = new ResponseResult();
			try
			{
				var SqlParams = new List<dynamic>();
				SqlParams.Add(new SqlParameter("@Flag", "FillPartyName"));
				_ResponseResult = await _IDataLogic.ExecuteDataTable("SPPartyWiseItemGroupDiscountDetail", SqlParams);
			}
			catch (Exception ex)
			{
				dynamic Error = new ExpandoObject();
				Error.Message = ex.Message;
				Error.Source = ex.Source;
			}

			return _ResponseResult;
		}
		public async Task<ResponseResult> FillEntryID(string EntryDate)
		{
			var _ResponseResult = new ResponseResult();
			try
			{
				var SqlParams = new List<dynamic>();
				SqlParams.Add(new SqlParameter("@Flag", "NewEntry"));
				SqlParams.Add(new SqlParameter("@EntryDate", EntryDate));
				_ResponseResult = await _IDataLogic.ExecuteDataTable("SPPartyWiseItemGroupDiscountDetail", SqlParams);
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
				SqlParams.Add(new SqlParameter("@Flag", "ItemgroupDiscountCategoryMaster"));
				_ResponseResult = await _IDataLogic.ExecuteDataTable("SPPartyWiseItemGroupDiscountDetail", SqlParams);
			}
			catch (Exception ex)
			{
				dynamic Error = new ExpandoObject();
				Error.Message = ex.Message;
				Error.Source = ex.Source;
			}

			return _ResponseResult;
		}
		public async Task<ResponseResult> FillCategoryCode()
		{
			var _ResponseResult = new ResponseResult();
			try
			{
				var SqlParams = new List<dynamic>();
				SqlParams.Add(new SqlParameter("@Flag", "ItemgroupDiscountCategoryMaster"));
				_ResponseResult = await _IDataLogic.ExecuteDataTable("SPPartyWiseItemGroupDiscountDetail", SqlParams);
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
				SqlParams.Add(new SqlParameter("@Flag", "FillGroupDetail"));
				_ResponseResult = await _IDataLogic.ExecuteDataTable("SPPartyWiseItemGroupDiscountDetail", SqlParams);
			}
			catch (Exception ex)
			{
				dynamic Error = new ExpandoObject();
				Error.Message = ex.Message;
				Error.Source = ex.Source;
			}

			return _ResponseResult;
		}
		public async Task<ResponseResult> FillGroupCode()
		{
			var _ResponseResult = new ResponseResult();
			try
			{
				var SqlParams = new List<dynamic>();
				SqlParams.Add(new SqlParameter("@Flag", "FillGroupDetail"));
				_ResponseResult = await _IDataLogic.ExecuteDataTable("SPPartyWiseItemGroupDiscountDetail", SqlParams);
			}
			catch (Exception ex)
			{
				dynamic Error = new ExpandoObject();
				Error.Message = ex.Message;
				Error.Source = ex.Source;
			}

			return _ResponseResult;
		}
		public async Task<ResponseResult> FillPartyNameDashBoard()
		{
			var _ResponseResult = new ResponseResult();
			try
			{
				var SqlParams = new List<dynamic>();
				SqlParams.Add(new SqlParameter("@Flag", "FillAccountINDashBoard"));
				_ResponseResult = await _IDataLogic.ExecuteDataTable("SPPartyWiseItemGroupDiscountDetail", SqlParams);
			}
			catch (Exception ex)
			{
				dynamic Error = new ExpandoObject();
				Error.Message = ex.Message;
				Error.Source = ex.Source;
			}

			return _ResponseResult;
		}
		

		public async Task<ResponseResult> FillCategoryNameDashBoard()
		{
			var _ResponseResult = new ResponseResult();
			try
			{
				var SqlParams = new List<dynamic>();
				SqlParams.Add(new SqlParameter("@Flag", "FillCategoryINDashBoard"));
				_ResponseResult = await _IDataLogic.ExecuteDataTable("SPPartyWiseItemGroupDiscountDetail", SqlParams);
			}
			catch (Exception ex)
			{
				dynamic Error = new ExpandoObject();
				Error.Message = ex.Message;
				Error.Source = ex.Source;
			}

			return _ResponseResult;
		}
		public async Task<ResponseResult> FillCategoryCodeDashBoard()
		{
			var _ResponseResult = new ResponseResult();
			try
			{
				var SqlParams = new List<dynamic>();
				SqlParams.Add(new SqlParameter("@Flag", "FillCategoryINDashBoard"));
				_ResponseResult = await _IDataLogic.ExecuteDataTable("SPPartyWiseItemGroupDiscountDetail", SqlParams);
			}
			catch (Exception ex)
			{
				dynamic Error = new ExpandoObject();
				Error.Message = ex.Message;
				Error.Source = ex.Source;
			}

			return _ResponseResult;
		}
		public async Task<ResponseResult> FillGroupNameDashBoard()
		{
			var _ResponseResult = new ResponseResult();
			try
			{
				var SqlParams = new List<dynamic>();
				SqlParams.Add(new SqlParameter("@Flag", "FillGroupINDashBoard"));
				_ResponseResult = await _IDataLogic.ExecuteDataTable("SPPartyWiseItemGroupDiscountDetail", SqlParams);
			}
			catch (Exception ex)
			{
				dynamic Error = new ExpandoObject();
				Error.Message = ex.Message;
				Error.Source = ex.Source;
			}

			return _ResponseResult;
		}
		public async Task<ResponseResult> FillGroupCodeDashBoard()
		{
			var _ResponseResult = new ResponseResult();
			try
			{
				var SqlParams = new List<dynamic>();
				SqlParams.Add(new SqlParameter("@Flag", "FillGroupINDashBoard"));
				_ResponseResult = await _IDataLogic.ExecuteDataTable("SPPartyWiseItemGroupDiscountDetail", SqlParams);
			}
			catch (Exception ex)
			{
				dynamic Error = new ExpandoObject();
				Error.Message = ex.Message;
				Error.Source = ex.Source;
			}

			return _ResponseResult;
		}
		public async Task<ResponseResult> SavePartyItemGroupDiscount(PartyItemGroupDiscountModel model, DataTable GIGrid)
		{
			var _ResponseResult = new ResponseResult();
			try
			{

				var SqlParams = new List<dynamic>();

				if (model.Mode == "U" || model.Mode == "V")
				{

					SqlParams.Add(new SqlParameter("@Flag", "Update"));

				}
				else
				{
					SqlParams.Add(new SqlParameter("@Flag", "Insert"));

				}
				var entDate = CommonFunc.ParseFormattedDate(model.EntryDate);
				var ActentDate = CommonFunc.ParseFormattedDate(model.ActualEntryDate);
				var upDate = CommonFunc.ParseFormattedDate(model.Updateddate);
				SqlParams.Add(new SqlParameter("@PartyWIseGrpDiscEntryId", model.EntryId));
				SqlParams.Add(new SqlParameter("@EntryDate", entDate));
				SqlParams.Add(new SqlParameter("@AccountCode", model.AccountCode));
				SqlParams.Add(new SqlParameter("@DiscCategoryEntryId", model.CategoryId ));
				//SqlParams.Add(new SqlParameter("@SeqNo", model.SeqNo ));
				//SqlParams.Add(new SqlParameter("@GroupId", model.GroupId));
				//SqlParams.Add(new SqlParameter("@SaleDiscount", model.SaleDiscount));
				SqlParams.Add(new SqlParameter("@EntryBy", model.EntryBy));
				SqlParams.Add(new SqlParameter("@EntryByMachine", model.EntryByMachine));
				SqlParams.Add(new SqlParameter("@CC", model.CC));
				
				SqlParams.Add(new SqlParameter("@dt", GIGrid));
				_ResponseResult = await _IDataLogic.ExecuteDataTable("SPPartyWiseItemGroupDiscountDetail", SqlParams);
			}
			catch (Exception ex)
			{
				dynamic Error = new ExpandoObject();
				Error.Message = ex.Message;
				Error.Source = ex.Source;
			}
			return _ResponseResult;
		}

        internal async Task<ResponseResult> GetDashboardData(PartyItemGroupDiscountModel model)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                var Flag = "";
                SqlParams.Add(new SqlParameter("@Flag", "DashBoard"));
                SqlParams.Add(new SqlParameter("@ReportType", model.ReportType));
                SqlParams.Add(new SqlParameter("@fromdate", model.FromDate));
                SqlParams.Add(new SqlParameter("@todate", model.ToDate));

                _ResponseResult = await _IDataLogic.ExecuteDataSet("SPPartyWiseItemGroupDiscountDetail", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }
        public async Task<PartyItemGroupDiscountModel> GetDashboardDetailData(string FromDate, string ToDate, string ReportType,int AccountCode,int CategoryId,string GroupName)
        {
            DataSet? oDataSet = new DataSet();
            var model = new PartyItemGroupDiscountModel();
            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    SqlCommand oCmd = new SqlCommand("SPPartyWiseItemGroupDiscountDetail", myConnection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    oCmd.Parameters.AddWithValue("@Flag", "DashBoard");
                    oCmd.Parameters.AddWithValue("@ReportType", ReportType);
                    oCmd.Parameters.AddWithValue("@fromdate", FromDate);
                    oCmd.Parameters.AddWithValue("@todate", ToDate);
                    oCmd.Parameters.AddWithValue("@AccountCode", AccountCode);
                    oCmd.Parameters.AddWithValue("@DiscCategoryEntryId", CategoryId);

                    await myConnection.OpenAsync();
                    using (SqlDataAdapter oDataAdapter = new SqlDataAdapter(oCmd))
                    {
                        oDataAdapter.Fill(oDataSet);
                    }
                }
				if(ReportType== "SUMMARY")
				{
                    if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                    {
                        model.PartyItemGroupDiscountGrid = (from DataRow dr in oDataSet.Tables[0].Rows
                                          select new PartyItemGroupDiscountModel
                                          {
                                              EntryDate = dr["EntryDate"] != DBNull.Value ? Convert.ToString(dr["EntryDate"]) : string.Empty,
                                              AccountName = dr["PartyName"] != DBNull.Value ? Convert.ToString(dr["PartyName"]) : string.Empty,
                                              CategoryName = dr["DiscCategoryName"] != DBNull.Value ? Convert.ToString(dr["DiscCategoryName"]) : string.Empty,
                                              CategoryCode = dr["DiscCategoryCode"] != DBNull.Value ? Convert.ToString(dr["DiscCategoryCode"]) : string.Empty,
                                              CategoryId = dr["DiscCategoryEntryId"] != DBNull.Value ? Convert.ToInt32(dr["DiscCategoryEntryId"]) : 0,
                                              PartyWIseGrpDiscEntryId = dr["PartyWIseGrpDiscEntryId"] != DBNull.Value ? Convert.ToInt32(dr["PartyWIseGrpDiscEntryId"]) : 0,
                                              AccountCode = dr["AccountCode"] != DBNull.Value ? Convert.ToInt32(dr["AccountCode"]) : 0,
                                              ActualEntryByEmpName = dr["EmpName"] != DBNull.Value ? Convert.ToString(dr["EmpName"]) : string.Empty,
                                              EntryByMachine = dr["EntryByMachine"] != DBNull.Value ? Convert.ToString(dr["EntryByMachine"]) : string.Empty,
                                              CC = dr["Branch"] != DBNull.Value ? Convert.ToString(dr["Branch"]) : string.Empty,
                                              
                                          }).ToList();


                    }
                }
                else if(ReportType== "DETAIL")
				{
                    if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                    {
                        model.PartyItemGroupDiscountGrid = (from DataRow dr in oDataSet.Tables[0].Rows
                                          select new PartyItemGroupDiscountModel
                                          {
                                              EntryDate = dr["EntryDate"] != DBNull.Value ? Convert.ToString(dr["EntryDate"]) : string.Empty,
                                              AccountName = dr["PartyName"] != DBNull.Value ? Convert.ToString(dr["PartyName"]) : string.Empty,
                                              CategoryName = dr["DiscCategoryName"] != DBNull.Value ? Convert.ToString(dr["DiscCategoryName"]) : string.Empty,
                                              CategoryCode = dr["DiscCategoryCode"] != DBNull.Value ? Convert.ToString(dr["DiscCategoryCode"]) : string.Empty,
                                              CategoryId = dr["DiscCategoryEntryId"] != DBNull.Value ? Convert.ToInt32(dr["DiscCategoryEntryId"]) : 0,
                                              PartyWIseGrpDiscEntryId = dr["PartyWIseGrpDiscEntryId"] != DBNull.Value ? Convert.ToInt32(dr["PartyWIseGrpDiscEntryId"]) : 0,
                                              AccountCode = dr["AccountCode"] != DBNull.Value ? Convert.ToInt32(dr["AccountCode"]) : 0,
                                              ActualEntryByEmpName = dr["EmpName"] != DBNull.Value ? Convert.ToString(dr["EmpName"]) : string.Empty,
                                              GroupCode = dr["Group_Code"] != DBNull.Value ? Convert.ToString(dr["Group_Code"]) : string.Empty,
                                              GroupName = dr["Group_name"] != DBNull.Value ? Convert.ToString(dr["Group_name"]) : string.Empty,
                                              GroupId = dr["GroupId"] != DBNull.Value ? Convert.ToInt32(dr["GroupId"]) : 0,
                                              PurchaseDiscount = dr["PurchaseDiscount"] != DBNull.Value ? Convert.ToInt32(dr["PurchaseDiscount"]) : 0,
                                              SaleDiscount = dr["SaleDiscount"] != DBNull.Value ? Convert.ToInt32(dr["SaleDiscount"]) : 0,
                                              EntryByMachine = dr["EntryByMachine"] != DBNull.Value ? Convert.ToString(dr["EntryByMachine"]) : string.Empty,
                                              CC = dr["Branch"] != DBNull.Value ? Convert.ToString(dr["Branch"]) : string.Empty
											  
											  
                                          }).ToList();


                    }
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

        public async Task<ResponseResult> DeleteByID(int EntryId, int AccountCode, string EntryDate)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var entrydate = CommonFunc.ParseFormattedDate(EntryDate);
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "DELETE"));
                SqlParams.Add(new SqlParameter("@EntryBy", EntryId));
                SqlParams.Add(new SqlParameter("@AccountCode", AccountCode));
               
                SqlParams.Add(new SqlParameter("@EntryDate", entrydate));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPPartyWiseItemGroupDiscountDetail", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<PartyItemGroupDiscountModel> GetViewByID(int ID)
        {
            var model = new PartyItemGroupDiscountModel();
            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@flag", "VIEWBYID"));
                SqlParams.Add(new SqlParameter("@PartyWIseGrpDiscEntryId", ID));
                var _ResponseResult = await _IDataLogic.ExecuteDataSet("SPPartyWiseItemGroupDiscountDetail", SqlParams);

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
        private static PartyItemGroupDiscountModel PrepareView(DataSet DS, ref PartyItemGroupDiscountModel? model)
        {
            try
            {
                var ItemList = new List<PartyItemGroupDiscountModel>();
                var DetailList = new List<PartyItemGroupDiscountModel>();
                DS.Tables[0].TableName = "PartyItemGroupDiscount";
                int cnt = 0;

                model.EntryDate = DS.Tables[0].Rows[0]["EntryDate"].ToString();
                model.EntryId = Convert.ToInt32(DS.Tables[0].Rows[0]["PartyWIseGrpDiscEntryId"]);
                model.CategoryId = Convert.ToInt32(DS.Tables[0].Rows[0]["DiscCategoryEntryId"]);

                model.AccountName = DS.Tables[0].Rows[0]["PartyName"].ToString();

                model.CategoryName = DS.Tables[0].Rows[0]["DiscCategoryName"].ToString();
                model.CategoryCode = DS.Tables[0].Rows[0]["DiscCategoryCode"].ToString();

                model.GroupName = DS.Tables[0].Rows[0]["Group_name"].ToString();

                model.GroupCode = DS.Tables[0].Rows[0]["Group_Code"].ToString();

                model.AccountCode = Convert.ToInt32(DS.Tables[0].Rows[0]["AccountCode"]);

                model.ActualEntryByEmpName = DS.Tables[0].Rows[0]["EmpName"].ToString();

                model.EntryByMachine = DS.Tables[0].Rows[0]["EntryByMachine"].ToString();

                model.CC = DS.Tables[0].Rows[0]["Branch"].ToString();

				if (DS.Tables.Count != 0 && DS.Tables[0].Rows.Count > 0)
				{
					foreach (DataRow row in DS.Tables[0].Rows)
					{
						ItemList.Add(new PartyItemGroupDiscountModel
						{
							GroupCode = row["Group_Code"].ToString(),
							GroupName = row["Group_name"].ToString(),
							SeqNo = Convert.ToInt32(row["SeqNo"].ToString()),
							PurchaseDiscount = Convert.ToDecimal(row["PurchaseDiscount"].ToString()),
							SaleDiscount = Convert.ToDecimal(row["SaleDiscount"].ToString()),

						});
					}
					model.PartyItemGroupDiscountGrid = ItemList;
				}
				return model;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
		public async Task<(bool Exists, int EntryId, int AccountCode, string AccountName, string CategoryCode, string CategoryName, int CategoryId)> CheckPartyExists(int AccountCode)
		{
			using (SqlConnection con = new SqlConnection(DBConnectionString))
			{
				SqlCommand cmd = new SqlCommand("SPPartyWiseItemGroupDiscountDetail", con);
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue("@Flag", "CheckPartyExists");
				cmd.Parameters.AddWithValue("@AccountCode", AccountCode);

				await con.OpenAsync();
				using var reader = await cmd.ExecuteReaderAsync();
				if (await reader.ReadAsync())
				{
					bool exists = Convert.ToInt32(reader["PartyExists"]) == 1;
					int entryId = Convert.ToInt32(reader["EntryId"]);
					int accountCode = Convert.ToInt32(reader["AccountCode"]);
					string accountName = reader["AccountName"].ToString();
					string categoryCode = reader["CategoryCode"].ToString();
					string categoryName = reader["CategoryName"].ToString();
					int categoryId = Convert.ToInt32(reader["CategoryId"]);

					return (exists, entryId, accountCode, accountName, categoryCode, categoryName, categoryId);
				}
				return (false, 0, 0, "", "", "", 0);
			}
		}



	}
}
