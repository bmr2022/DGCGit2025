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
    public class MachineGroupMasterDAL
    {
		private readonly IDataLogic _IDataLogic;
		private readonly string DBConnectionString = string.Empty;
		private IDataReader? Reader;
		private readonly ConnectionStringService _connectionStringService;
		public MachineGroupMasterDAL(IConfiguration configuration, IDataLogic iDataLogic, ConnectionStringService connectionStringService)
		{
			_connectionStringService = connectionStringService;
			DBConnectionString = _connectionStringService.GetConnectionString();
			_IDataLogic = iDataLogic;
		}
		public async Task<ResponseResult> FillMachineGroup()
		{
			var _ResponseResult = new ResponseResult();
			try
			{
				var SqlParams = new List<dynamic>();
				SqlParams.Add(new SqlParameter("@Flag", "FillMachineGroup"));
				_ResponseResult = await _IDataLogic.ExecuteDataTable("SPMachineGroupMaster", SqlParams);
			}
			catch (Exception ex)
			{
				dynamic Error = new ExpandoObject();
				Error.Message = ex.Message;
				Error.Source = ex.Source;
			}

			return _ResponseResult;
		}
		public async Task<ResponseResult> SaveMachineGroupMaster(MachineGroupMasterModel model)
		{
			var _ResponseResult = new ResponseResult();

			try
			{
				var sqlParams = new List<dynamic>();

				if (model.Mode == "U" || model.Mode == "V")
				{
					sqlParams.Add(new SqlParameter("@Flag", "UPDATE"));
					sqlParams.Add(new SqlParameter("@EntryId", model.EntryId));
					//sqlParams.Add(new SqlParameter("@LastupDationDate", model.LastUpdationDate));
				}
				else
				{
					sqlParams.Add(new SqlParameter("@Flag", "INSERT"));
					sqlParams.Add(new SqlParameter("@EntryId", model.EntryId));
				}

				sqlParams.Add(new SqlParameter("@MachGroup", model.MachineGroup));
				sqlParams.Add(new SqlParameter("@UId", model.LastUpdatedBy));
				sqlParams.Add(new SqlParameter("@CC", model.CC));
				
				//sqlParams.Add(new SqlParameter("@ActualEntryByEmpId", model.ApprovedByEmpId));
				//sqlParams.Add(new SqlParameter("@ActualEntryDate", model.ActualEntryDate));
				//sqlParams.Add(new SqlParameter("@EntryByMachine", model.EntryByMachine));
				//sqlParams.Add(new SqlParameter("@ApprovedByEmpId", model.ApprovedByEmpId));


				_ResponseResult = await _IDataLogic.ExecuteDataTable("SPMachineGroupMaster", sqlParams);
			}
			catch (Exception ex)
			{
				_ResponseResult.StatusCode = HttpStatusCode.InternalServerError;
				_ResponseResult.StatusText = "Error";
				_ResponseResult.Result = new { ex.Message, ex.StackTrace };
			}

			return _ResponseResult;
		}
        public async Task<ResponseResult> GetDashboardData(MachineGroupMasterModel model)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "Dashboard"));
                //SqlParams.Add(new SqlParameter("@FromDate", model.FromDate));
                //SqlParams.Add(new SqlParameter("@ToDate", model.ToDate));

                _ResponseResult = await _IDataLogic.ExecuteDataSet("SPMachineGroupMaster", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }
        public async Task<MachineGroupMasterModel> GetDashboardDetailData(string FromDate, string ToDate)
        {
            DataSet? oDataSet = new DataSet();
            var model = new MachineGroupMasterModel();
            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    SqlCommand oCmd = new SqlCommand("SPMachineGroupMaster", myConnection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    oCmd.Parameters.AddWithValue("@Flag", "Dashboard");
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

                    model.MachineGroupGrid = (from DataRow dr in oDataSet.Tables[0].Rows
                                                                select new MachineGroupMasterModel
                                                                {
                                                                    EntryId = dr["entryid"] != DBNull.Value ? Convert.ToInt32(dr["entryid"]) : 0,
                                                                    MachineGroup = dr["MachGroup"] != DBNull.Value ? Convert.ToString(dr["MachGroup"]) : string.Empty,
                                                                    CC = dr["CC"] != DBNull.Value ? Convert.ToString(dr["CC"]) : string.Empty,
                                                                    UId = dr["UId"] != DBNull.Value ? Convert.ToInt32(dr["UId"]) : 0,
                                                                    
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

		//public async Task<ResponseResult> DeleteByID(int EntryId)
		//{
		//    var _ResponseResult = new ResponseResult();
		//    try
		//    {

		//        var SqlParams = new List<dynamic>();
		//        SqlParams.Add(new SqlParameter("@Flag", "DeleteByID"));
		//        SqlParams.Add(new SqlParameter("@Entryid", EntryId));
		//        _ResponseResult = await _IDataLogic.ExecuteDataTable("SPMachineGroupMaster", SqlParams);
		//    }
		//    catch (Exception ex)
		//    {
		//        dynamic Error = new ExpandoObject();
		//        Error.Message = ex.Message;
		//        Error.Source = ex.Source;
		//    }

		//    return _ResponseResult;
		//}
		public async Task<ResponseResult> DeleteByID(int EntryId)
		{
			var response = new ResponseResult();

			try
			{
				using (var conn = new SqlConnection(DBConnectionString)) // ensure you have your connection string
				using (var cmd = new SqlCommand("SPMachineGroupMaster", conn))
				{
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddWithValue("@Flag", "DeleteByID");
					cmd.Parameters.AddWithValue("@Entryid", EntryId);

					await conn.OpenAsync();

					using (var reader = await cmd.ExecuteReaderAsync())
					{
						if (await reader.ReadAsync())
						{
							response.StatusText = reader["StatusText"].ToString();
							response.StatusCode = (HttpStatusCode)Convert.ToInt32(reader["StatusCode"]);
							response.Result = reader["Result"].ToString();
						}
						else
						{
							response.StatusText = "Error";
							response.StatusCode = (HttpStatusCode)500;
							response.Result = "No response from stored procedure.";
						}
					}
				}
			}
			catch (Exception ex)
			{
				response.StatusText = "Exception";
				response.StatusCode = (HttpStatusCode)500;
				response.Result = ex.Message;
			}

			return response;
		}

		public async Task<MachineGroupMasterModel> GetViewByID(int ID)
        {
            var model = new MachineGroupMasterModel();
            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@flag", "ViewByID"));
                SqlParams.Add(new SqlParameter("@Entryid", ID));
                var _ResponseResult = await _IDataLogic.ExecuteDataSet("SPMachineGroupMaster", SqlParams);

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
        private static MachineGroupMasterModel PrepareView(DataSet DS, ref MachineGroupMasterModel? model)
        {
            try
            {
                var ItemList = new List<MachineGroupMasterModel>();
                var DetailList = new List<MachineGroupMasterModel>();
                DS.Tables[0].TableName = "MachineGroupMaster";
                int cnt = 0;
                model.EntryId = Convert.ToInt32(DS.Tables[0].Rows[0]["entryid"].ToString());
                model.MachineGroup = DS.Tables[0].Rows[0]["MachGroup"].ToString();
                model.CC = DS.Tables[0].Rows[0]["CC"].ToString();
                model.UId = Convert.ToInt32(DS.Tables[0].Rows[0]["UId"].ToString());
               
                return model;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<(bool Exists, int EntryId, string MachGroup, long UId, string CC)> CheckMachineGroupExists(string machGroup)
        {
            using (SqlConnection con = new SqlConnection(DBConnectionString))
            {
                SqlCommand cmd = new SqlCommand("SPMachineGroupMaster", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Flag", "CheckPartyExists");
                cmd.Parameters.AddWithValue("@MachGroup", machGroup);

                await con.OpenAsync();
                using var reader = await cmd.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    bool exists = true; // since it returned a row
                    int entryId = Convert.ToInt32(reader["EntryId"]);
                    string machGroupVal = reader["MachGroup"].ToString();
                    long uId = Convert.ToInt64(reader["UId"]);
                    string cc = reader["CC"].ToString();

                    return (exists, entryId, machGroupVal, uId, cc);
                }

                return (false, 0, "", 0, "");
            }
        }
    }
}
