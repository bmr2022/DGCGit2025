using DocumentFormat.OpenXml.EMMA;
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
    public class POApprovalPolicyDAL
    {
		private readonly IDataLogic _IDataLogic;
		private readonly string DBConnectionString = string.Empty;
		private IDataReader? Reader;
		private readonly ConnectionStringService _connectionStringService;
		public POApprovalPolicyDAL(IConfiguration configuration, IDataLogic iDataLogic, ConnectionStringService connectionStringService)
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
				SqlParams.Add(new SqlParameter("@Flag", "FillPartCodeItemName"));
				_ResponseResult = await _IDataLogic.ExecuteDataTable("SpPOApprovalPolicy", SqlParams);
			}
			catch (Exception ex)
			{
				dynamic Error = new ExpandoObject();
				Error.Message = ex.Message;
				Error.Source = ex.Source;
			}

			return _ResponseResult;
		}
		public async Task<ResponseResult> FillPartCode()
		{
			var _ResponseResult = new ResponseResult();
			try
			{
				var SqlParams = new List<dynamic>();
				SqlParams.Add(new SqlParameter("@Flag", "FillPartCodeItemName"));
				_ResponseResult = await _IDataLogic.ExecuteDataTable("SpPOApprovalPolicy", SqlParams);
			}
			catch (Exception ex)
			{
				dynamic Error = new ExpandoObject();
				Error.Message = ex.Message;
				Error.Source = ex.Source;
			}

			return _ResponseResult;
		}
		public async Task<ResponseResult> FillEmpName()
		{
			var _ResponseResult = new ResponseResult();
			try
			{
				var SqlParams = new List<dynamic>();
				SqlParams.Add(new SqlParameter("@Flag", "FillEmployeeList"));
				_ResponseResult = await _IDataLogic.ExecuteDataTable("SpPOApprovalPolicy", SqlParams);
			}
			catch (Exception ex)
			{
				dynamic Error = new ExpandoObject();
				Error.Message = ex.Message;
				Error.Source = ex.Source;
			}

			return _ResponseResult;
		}
		public async Task<ResponseResult> FillCatName()
		{
			var _ResponseResult = new ResponseResult();
			try
			{
				var SqlParams = new List<dynamic>();
				SqlParams.Add(new SqlParameter("@Flag", "FillCategory"));
				_ResponseResult = await _IDataLogic.ExecuteDataTable("SpPOApprovalPolicy", SqlParams);
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
				SqlParams.Add(new SqlParameter("@Flag", "FillGroup"));
				_ResponseResult = await _IDataLogic.ExecuteDataTable("SpPOApprovalPolicy", SqlParams);
			}
			catch (Exception ex)
			{
				dynamic Error = new ExpandoObject();
				Error.Message = ex.Message;
				Error.Source = ex.Source;
			}

			return _ResponseResult;
		}
		public async Task<ResponseResult> FillEntryID()
		{
			var _ResponseResult = new ResponseResult();
			try
			{
				var SqlParams = new List<dynamic>();
				SqlParams.Add(new SqlParameter("@Flag", "NewEntryId"));
				_ResponseResult = await _IDataLogic.ExecuteDataTable("SpPOApprovalPolicy", SqlParams);
			}
			catch (Exception ex)
			{
				dynamic Error = new ExpandoObject();
				Error.Message = ex.Message;
				Error.Source = ex.Source;
			}

			return _ResponseResult;
		}
		public async Task<ResponseResult> SavePOApprovalPolicy(POApprovalPolicyModel model)
		{
			var _ResponseResult = new ResponseResult();


            var entDate = CommonFunc.ParseFormattedDate(model.EntryDate);
            var upDate = CommonFunc.ParseFormattedDate(model.LastUpdatedDate);
			try
			{
				var sqlParams = new List<dynamic>();

				
				sqlParams.Add(new SqlParameter("@POTYPE", model.POTYPE));
				sqlParams.Add(new SqlParameter("@ItemGroupId", model.GroupCode));
				sqlParams.Add(new SqlParameter("@ItemCategoryId", model.CatId));
				sqlParams.Add(new SqlParameter("@Itemcode", model.Itemcode));
				sqlParams.Add(new SqlParameter("@FromAmt", model.FromAmt));
				sqlParams.Add(new SqlParameter("@ToAmt", model.ToAmt));
				sqlParams.Add(new SqlParameter("@FirstApprovalRequired", model.FirstApprovalRequired));
				sqlParams.Add(new SqlParameter("@FinalApprovalRequired1", model.FinalApprovalRequired1));
				sqlParams.Add(new SqlParameter("@OnlyDirectorApproval", model.OnlyDirectorApproval));
				sqlParams.Add(new SqlParameter("@ONLY1stApprovalRequired", model.ONLY1stApprovalRequired));
				sqlParams.Add(new SqlParameter("@OnlyFinalApprovalRequired", model.OnlyFinalApprovalRequired));
				sqlParams.Add(new SqlParameter("@All3ApprovalRequired", model.All3ApprovalRequired));
				sqlParams.Add(new SqlParameter("@EmpidForFirstApproval1", model.EmpidForFirstApproval1));
				sqlParams.Add(new SqlParameter("@EmpidForFirstApproval2", model.EmpidForFirstApproval2));
				sqlParams.Add(new SqlParameter("@EmpidForFirstApproval3", model.EmpidForFirstApproval3));
				sqlParams.Add(new SqlParameter("@EmpidForFinalApproval1", model.EmpidForFinalApproval1));
				sqlParams.Add(new SqlParameter("@EmpidForFinalApproval2", model.EmpidForFinalApproval2));
				sqlParams.Add(new SqlParameter("@EmpidForFinalApproval3", model.EmpidForFinalApproval3));
				sqlParams.Add(new SqlParameter("@EmpidForMgmtApproval1", model.EmpidForMgmtApproval1));
				sqlParams.Add(new SqlParameter("@EmpidForMgmtApproval12", model.EmpidForMgmtApproval12));
				sqlParams.Add(new SqlParameter("@EmpidForMgmtApproval13", model.EmpidForMgmtApproval13));
				sqlParams.Add(new SqlParameter("@EmpidForDirectorApproval", model.EmpidForDirectorApproval));
				sqlParams.Add(new SqlParameter("@EmpidForFinalApproval", model.EmpidForFinalApproval));
				sqlParams.Add(new SqlParameter("@EmpidForFirstApproval", model.EmpidForFirstApproval));
				sqlParams.Add(new SqlParameter("@EntryByMachine", model.EntryByMachine ?? Environment.MachineName));
				sqlParams.Add(new SqlParameter("@ActualEntryByEmpId", model.ActualEntryByEmpId));
				sqlParams.Add(new SqlParameter("@ActualEntryDate", entDate));
				sqlParams.Add(new SqlParameter("@CC", model.CC));

				if (model.Mode == "U") 
				{
					sqlParams.Add(new SqlParameter("@Flag", "Update"));
					sqlParams.Add(new SqlParameter("@POApprovalEntryId", model.EntryId));
					sqlParams.Add(new SqlParameter("@LastUpdatedByEmpId", model.LastUpdatedByEmpId));
					sqlParams.Add(new SqlParameter("@LastUpdatedDate", upDate));
				}
				else 
				{
					sqlParams.Add(new SqlParameter("@Flag", "Insert"));
					sqlParams.Add(new SqlParameter("@POApprovalEntryId", model.EntryId));
				}

				
				_ResponseResult = await _IDataLogic.ExecuteDataTable("SpPOApprovalPolicy", sqlParams);
			}
			catch (Exception ex)
			{
				_ResponseResult.StatusCode = HttpStatusCode.InternalServerError;
				_ResponseResult.StatusText = "Error";
				_ResponseResult.Result = new { ex.Message, ex.StackTrace };
			}

			return _ResponseResult;
		}

        internal async Task<ResponseResult> GetDashboardData(POApprovalPolicyModel model)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "DASHBOARD"));
                SqlParams.Add(new SqlParameter("@Fromdate", model.FromDate));
                SqlParams.Add(new SqlParameter("@Todate", model.ToDate));
                _ResponseResult = await _IDataLogic.ExecuteDataSet("SpPOApprovalPolicy", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }
        public async Task<POApprovalPolicyModel> GetDashboardDetailData(string FromDate, string ToDate, string ReportType, string GroupName, string CateName, string ItemName)
        {
            DataSet? oDataSet = new DataSet();
            var model = new POApprovalPolicyModel();
            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    SqlCommand oCmd = new SqlCommand("SpPOApprovalPolicy", myConnection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    oCmd.Parameters.AddWithValue("@Flag", "DASHBOARD");
                    oCmd.Parameters.AddWithValue("@Fromdate", FromDate);
                    oCmd.Parameters.AddWithValue("@Todate", ToDate);
                    oCmd.Parameters.AddWithValue("@itemname", ItemName);
                    oCmd.Parameters.AddWithValue("@GroupName", GroupName);
                    oCmd.Parameters.AddWithValue("@CategoryName", CateName);

                    await myConnection.OpenAsync();
                    using (SqlDataAdapter oDataAdapter = new SqlDataAdapter(oCmd))
                    {
                        oDataAdapter.Fill(oDataSet);
                    }
                }
                if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                {
                    model.POApprovalPolicyDashBoardGrid = (from DataRow dr in oDataSet.Tables[0].Rows
                                      select new POApprovalPolicyModel
                                      {
                                          POApprovalEntryId = dr["POApprovalEntryId"] != DBNull.Value ? Convert.ToInt32(dr["POApprovalEntryId"]) : 0,
                                          POTYPE = dr["POTYPE"] != DBNull.Value ? Convert.ToString(dr["POTYPE"]) : string.Empty,
                                          GroupName = dr["Group_name"] != DBNull.Value ? Convert.ToString(dr["Group_name"]) : string.Empty,
                                          CatName = dr["CategoryName"] != DBNull.Value ? Convert.ToString(dr["CategoryName"]) : string.Empty,
                                          ItemName = dr["Item_Name"] != DBNull.Value ? Convert.ToString(dr["Item_Name"]) : string.Empty,
                                          FromAmt = dr["FromAmt"] != DBNull.Value ? Convert.ToDecimal(dr["FromAmt"]) : 0,
                                          ToAmt = dr["ToAmt"] != DBNull.Value ? Convert.ToDecimal(dr["ToAmt"]) : 0,
                                          FirstApprovalRequired = dr["FirstApprovalRequired"] != DBNull.Value ? Convert.ToString(dr["FirstApprovalRequired"]) : string.Empty,
                                          FinalApprovalRequired1 = dr["FinalApprovalRequired1"] != DBNull.Value ? Convert.ToString(dr["FinalApprovalRequired1"]) : string.Empty,
                                          OnlyDirectorApproval = dr["OnlyDirectorApproval"] != DBNull.Value ? Convert.ToString(dr["OnlyDirectorApproval"]) : string.Empty,
                                          ONLY1stApprovalRequired = dr["ONLY1stApprovalRequired"] != DBNull.Value ? Convert.ToString(dr["ONLY1stApprovalRequired"]) : string.Empty,
                                          OnlyFinalApprovalRequired = dr["OnlyFinalApprovalRequired"] != DBNull.Value ? Convert.ToString(dr["OnlyFinalApprovalRequired"]) : string.Empty,
                                          All3ApprovalRequired = dr["All3ApprovalRequired"] != DBNull.Value ? Convert.ToString(dr["All3ApprovalRequired"]) : string.Empty,

                                          EmpNameForFirstApproval1 = dr["EmpNameForFirstApproval1"] != DBNull.Value ? Convert.ToString(dr["EmpNameForFirstApproval1"]) : string.Empty,
                                          EmpNameForFirstApproval2 = dr["EmpNameForFirstApproval2"] != DBNull.Value ? Convert.ToString(dr["EmpNameForFirstApproval2"]) : string.Empty,
                                          EmpNameForFirstApproval3 = dr["EmpNameForFirstApproval3"] != DBNull.Value ? Convert.ToString(dr["EmpNameForFirstApproval3"]) : string.Empty,
                                          EmpNameForFinalApproval1 = dr["EmpNameForFinalApproval1"] != DBNull.Value ? Convert.ToString(dr["EmpNameForFinalApproval1"]) : string.Empty,
                                          EmpNameForFinalApproval2 = dr["EmpNameForFinalApproval2"] != DBNull.Value ? Convert.ToString(dr["EmpNameForFinalApproval2"]) : string.Empty,
                                          EmpNameForFinalApproval3 = dr["EmpNameForFinalApproval3"] != DBNull.Value ? Convert.ToString(dr["EmpNameForFinalApproval3"]) : string.Empty,
                                          EmpNameForMgmtApproval1 = dr["EmpNameForMgmtApproval1"] != DBNull.Value ? Convert.ToString(dr["EmpNameForMgmtApproval1"]) : string.Empty,
                                          EmpNameForMgmtApproval12 = dr["EmpNameForMgmtApproval12"] != DBNull.Value ? Convert.ToString(dr["EmpNameForMgmtApproval12"]) : string.Empty,
                                          EmpNameForMgmtApproval13 = dr["EmpNameForMgmtApproval13"] != DBNull.Value ? Convert.ToString(dr["EmpNameForMgmtApproval13"]) : string.Empty,

                                          EmpidForDirectorApproval = dr["EmpidForDirectorApproval"] != DBNull.Value ? Convert.ToInt32(dr["EmpidForDirectorApproval"]) : 0,
                                          EmpNameForDirectorApproval = dr["EmpNameForDirectorApproval"] != DBNull.Value ? Convert.ToString(dr["EmpNameForDirectorApproval"]) : string.Empty,

                                          EmpidForFinalApproval = dr["EmpidForFinalApproval"] != DBNull.Value ? Convert.ToInt32(dr["EmpidForFinalApproval"]) : 0,
                                          EmpNameForFinalApproval = dr["EmpNameForFinalApproval"] != DBNull.Value ? Convert.ToString(dr["EmpNameForFinalApproval"]) : string.Empty,

                                          EmpidForFirstApproval = dr["EmpidForFirstApproval"] != DBNull.Value ? Convert.ToInt32(dr["EmpidForFirstApproval"]) : 0,
                                          EmpNameForFirstApproval = dr["EmpNameForFirstApproval"] != DBNull.Value ? Convert.ToString(dr["EmpNameForFirstApproval"]) : string.Empty,


                                          EntryByMachine = dr["EntryByMachine"] != DBNull.Value ? Convert.ToString(dr["EntryByMachine"]) : string.Empty,
                                          ActualEntryByEmpId = dr["ActualEntryByEmpId"] != DBNull.Value ? Convert.ToInt32(dr["ActualEntryByEmpId"]) : 0,
                                          ActualEntryByName = dr["ActualEntryByEmployee"] != DBNull.Value ? Convert.ToString(dr["ActualEntryByEmployee"]) : string.Empty,
                                          ActualEntryDate = dr["ActualEntryDate"] != DBNull.Value ? Convert.ToString(dr["ActualEntryDate"]) : string.Empty,
                                          LastUpdatedByEmpId = dr["LastUpdatedByEmpId"] != DBNull.Value ? Convert.ToInt32(dr["LastUpdatedByEmpId"]) : 0,
                                          LastUpdatedByName = dr["LastUpdatedByEmployee"] != DBNull.Value ? Convert.ToString(dr["LastUpdatedByEmployee"]) : string.Empty,
                                          LastUpdatedDate = dr["LastUpdatedDate"] != DBNull.Value ? Convert.ToString(dr["LastUpdatedDate"]) : string.Empty,
                                          CC = dr["CC"] != DBNull.Value ? Convert.ToString(dr["CC"]) : string.Empty
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

        public async Task<ResponseResult> DeleteByID(int EntryId,string EntryDate, int EntryByempId)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var entrydate = CommonFunc.ParseFormattedDate(EntryDate);
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "DELETE"));
                SqlParams.Add(new SqlParameter("@POApprovalEntryId", EntryId));
                SqlParams.Add(new SqlParameter("@ActualEntryDate", entrydate));
                SqlParams.Add(new SqlParameter("@ActualEntryByEmpId", EntryByempId));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SpPOApprovalPolicy", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<POApprovalPolicyModel> GetViewByID(int ID, int YC, string FromDate, string ToDate)
        {
            var model = new POApprovalPolicyModel();
            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@flag", "VIEWBYID"));
                SqlParams.Add(new SqlParameter("@POApprovalEntryId", ID));
                SqlParams.Add(new SqlParameter("@POApprovalYearCode", YC));
                var _ResponseResult = await _IDataLogic.ExecuteDataSet("SpPOApprovalPolicy", SqlParams);

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
        private static POApprovalPolicyModel PrepareView(DataSet DS, ref POApprovalPolicyModel? model)
        {
            try
            {
                var ItemList = new List<POApprovalPolicyModel>();
                DS.Tables[0].TableName = "POApprovalPolicy";
                int cnt = 0;

                model.EntryId = Convert.ToInt32(DS.Tables[0].Rows[0]["POApprovalEntryId"].ToString());
                //1model.YearCode = Convert.ToInt32(DS.Tables[0].Rows[0]["POApprovalYearCode"].ToString());
                model.POTYPE = DS.Tables[0].Rows[0]["POTYPE"].ToString();
                model.EntryDate = DS.Tables[0].Rows[0]["ActualEntryDate"].ToString();
                model.GroupCode = Convert.ToInt32(DS.Tables[0].Rows[0]["ItemGroupId"].ToString());
                model.GroupName = DS.Tables[0].Rows[0]["ItemGroupName"].ToString();
                model.CatId = Convert.ToInt32(DS.Tables[0].Rows[0]["ItemCategoryId"].ToString());
                //model.EmpId = Convert.ToInt32(DS.Tables[0].Rows[0]["ItemCategoryId"].ToString());
                //model.EmpName = DS.Tables[0].Rows[0]["ItemCategoryName"].ToString();
                model.CatName = DS.Tables[0].Rows[0]["ItemCategoryName"].ToString();
                model.ItemCode = Convert.ToInt32(DS.Tables[0].Rows[0]["Itemcode"].ToString());
                model.ItemName = DS.Tables[0].Rows[0]["Item_Name"].ToString();
                model.FromAmt = Convert.ToDecimal(DS.Tables[0].Rows[0]["FromAmt"].ToString());
                model.ToAmt = Convert.ToDecimal(DS.Tables[0].Rows[0]["ToAmt"].ToString());
                model.FirstApprovalRequired = DS.Tables[0].Rows[0]["FirstApprovalRequired"].ToString();
                model.FinalApprovalRequired1 = DS.Tables[0].Rows[0]["FinalApprovalRequired1"].ToString();
                model.OnlyDirectorApproval = DS.Tables[0].Rows[0]["OnlyDirectorApproval"].ToString();
                model.ONLY1stApprovalRequired = DS.Tables[0].Rows[0]["ONLY1stApprovalRequired"].ToString();
                model.OnlyFinalApprovalRequired = DS.Tables[0].Rows[0]["OnlyFinalApprovalRequired"].ToString();
                model.All3ApprovalRequired = DS.Tables[0].Rows[0]["All3ApprovalRequired"].ToString();
                model.EmpidForFirstApproval1 = Convert.ToInt32(DS.Tables[0].Rows[0]["EmpidForFirstApproval1"].ToString());
                model.EmpNameForFirstApproval1 = DS.Tables[0].Rows[0]["EmpNameForFirstApproval1"].ToString();
                model.EmpidForFirstApproval2 = Convert.ToInt32(DS.Tables[0].Rows[0]["EmpidForFirstApproval2"].ToString());
                model.EmpNameForFirstApproval2 = DS.Tables[0].Rows[0]["EmpNameForFirstApproval2"].ToString();
                model.EmpidForFirstApproval3 = Convert.ToInt32(DS.Tables[0].Rows[0]["EmpidForFirstApproval3"].ToString());
                model.EmpNameForFirstApproval3 = DS.Tables[0].Rows[0]["EmpNameForFirstApproval3"].ToString();
                model.EmpidForFinalApproval1 = Convert.ToInt32(DS.Tables[0].Rows[0]["EmpidForFinalApproval1"].ToString());
                model.EmpNameForFinalApproval1 = DS.Tables[0].Rows[0]["EmpNameForFinalApproval1"].ToString();
                model.EmpidForFinalApproval2 = Convert.ToInt32(DS.Tables[0].Rows[0]["EmpidForFinalApproval2"].ToString());
                model.EmpNameForFinalApproval2 = DS.Tables[0].Rows[0]["EmpNameForFinalApproval2"].ToString();
                model.EmpidForFinalApproval3 = Convert.ToInt32(DS.Tables[0].Rows[0]["EmpidForFinalApproval3"].ToString());
                model.EmpNameForFinalApproval3 = DS.Tables[0].Rows[0]["EmpNameForFinalApproval3"].ToString();
                model.EmpidForMgmtApproval1 = Convert.ToInt32(DS.Tables[0].Rows[0]["EmpidForMgmtApproval1"].ToString());
                model.EmpNameForMgmtApproval1 = DS.Tables[0].Rows[0]["EmpNameForMgmtApproval1"].ToString();
                model.EmpidForMgmtApproval12 = Convert.ToInt32(DS.Tables[0].Rows[0]["EmpidForMgmtApproval12"].ToString());
                model.EmpNameForMgmtApproval12 = DS.Tables[0].Rows[0]["EmpNameForMgmtApproval12"].ToString();
                model.EmpidForMgmtApproval13 = Convert.ToInt32(DS.Tables[0].Rows[0]["EmpidForMgmtApproval13"].ToString());
                model.EmpNameForMgmtApproval13 = DS.Tables[0].Rows[0]["EmpNameForMgmtApproval13"].ToString();
                
                model.EmpidForDirectorApproval = Convert.ToInt32(DS.Tables[0].Rows[0]["EmpidForDirectorApproval"].ToString());
                model.EmpNameForDirectorApproval = DS.Tables[0].Rows[0]["EmpNameForDirectorApproval"].ToString();

                model.EmpidForFinalApproval = Convert.ToInt32(DS.Tables[0].Rows[0]["EmpidForFinalApproval"].ToString());
                model.EmpNameForFinalApproval = DS.Tables[0].Rows[0]["EmpNameForFinalApproval"].ToString();

                model.EmpidForFirstApproval = Convert.ToInt32(DS.Tables[0].Rows[0]["EmpidForFirstApproval"].ToString());
                model.EmpNameForFirstApproval = DS.Tables[0].Rows[0]["EmpNameForFirstApproval"].ToString();

                model.EntryByMachine = DS.Tables[0].Rows[0]["EntryByMachine"].ToString();
                model.ActualEntryByEmpId = Convert.ToInt32(DS.Tables[0].Rows[0]["ActualEntryByEmpId"].ToString());
                model.ActualEntryByName = DS.Tables[0].Rows[0]["ActualEntryByEmployee"].ToString();
                model.ActualEntryDate = DS.Tables[0].Rows[0]["ActualEntryDate"].ToString();
                model.LastUpdatedByEmpId = Convert.ToInt32(DS.Tables[0].Rows[0]["LastUpdatedByEmpId"].ToString());
                model.LastUpdatedByName = DS.Tables[0].Rows[0]["LastUpdatedByEmployee"].ToString();
                model.LastUpdatedDate = DS.Tables[0].Rows[0]["LastUpdatedDate"].ToString();
                model.CC = DS.Tables[0].Rows[0]["CC"].ToString();

                return model;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<ResponseResult> FillItems(string SearchItemCode)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "fillitem"));

                SqlParams.Add(new SqlParameter("@ItemName", SearchItemCode ?? ""));
                _ResponseResult = await _IDataLogic.ExecuteDataSet("SpPOApprovalPolicy", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }
        public async Task<ResponseResult> FillGroups(string SearchGroupName)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "fillgroupname"));
                SqlParams.Add(new SqlParameter("@itemname", SearchGroupName ?? ""));
                _ResponseResult = await _IDataLogic.ExecuteDataSet("SpPOApprovalPolicy", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }
        public async Task<ResponseResult> FillCateName(string SearchCatName)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "fillcategoryname"));
                SqlParams.Add(new SqlParameter("@itemname", SearchCatName ?? ""));
                _ResponseResult = await _IDataLogic.ExecuteDataSet("SpPOApprovalPolicy", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }
        public async Task<ResponseResult> CheckGroupExists(string GroupName)
        {
            var _ResponseResult = new ResponseResult();
			var model = new POApprovalPolicyModel();
			try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "CHECKGROUP"));
                SqlParams.Add(new SqlParameter("@GroupName", GroupName ?? ""));

                _ResponseResult = await _IDataLogic.ExecuteDataSet("SpPOApprovalPolicy", SqlParams);
				
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
