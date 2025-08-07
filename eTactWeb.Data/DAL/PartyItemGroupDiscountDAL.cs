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
        public async Task<PartyItemGroupDiscountModel> GetDashboardDetailData(string FromDate, string ToDate, string ReportType)
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
                                              PartyWIseGrpDiscEntryId = dr["PartyWIseGrpDiscEntryId"] != DBNull.Value ? Convert.ToInt32(dr["PartyWIseGrpDiscEntryId"]) : 0,
                                              AccountCode = dr["AccountCode"] != DBNull.Value ? Convert.ToInt32(dr["AccountCode"]) : 0,
                                              ActualEntryByEmpName = dr["EmpName"] != DBNull.Value ? Convert.ToString(dr["EmpName"]) : string.Empty,
                                              EntryByMachine = dr["EntryByMachine"] != DBNull.Value ? Convert.ToString(dr["EntryByMachine"]) : string.Empty,
                                              CC = dr["Branch"] != DBNull.Value ? Convert.ToString(dr["Branch"]) : string.Empty,
                                              GroupCode = dr["Group_Code"] != DBNull.Value ? Convert.ToString(dr["Group_Code"]) : string.Empty,
                                              GroupName = dr["Group_name"] != DBNull.Value ? Convert.ToString(dr["Group_name"]) : string.Empty,

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
    }
}
