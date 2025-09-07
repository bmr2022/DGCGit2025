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
    public class PrimaryAccountGroupMasterDAL
    {
        private readonly IDataLogic _IDataLogic;
        private readonly string DBConnectionString = string.Empty;
        private IDataReader? Reader;
        private readonly ConnectionStringService _connectionStringService;

        //private readonly IConfiguration configuration;

        public PrimaryAccountGroupMasterDAL(IConfiguration configuration, IDataLogic iDataLogic, ConnectionStringService connectionStringService)
        {
            //configuration = config;
            //DBConnectionString = configuration.GetConnectionString("eTactDB");
            _IDataLogic = iDataLogic;
            _connectionStringService = connectionStringService;
            DBConnectionString = _connectionStringService.GetConnectionString();
        }
        public async Task<ResponseResult> GetFormRights(int userID)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "GetRights"));
                SqlParams.Add(new SqlParameter("@EmpId", userID));
                SqlParams.Add(new SqlParameter("@MainMenu", "PrimaryAccountGroupMaster"));
                //SqlParams.Add(new SqlParameter("@SubMenu", "Sale Order"));

                _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_ItemGroup", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }
        public async Task<ResponseResult> GetParentGroup()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FillParentGroup"));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("AccSpPrimaryAccountHeadMaster", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
       
        public async Task<ResponseResult> GetAccountGroupDetailsByParentCode(int parentAccountCode)
        {
            var responseResult = new ResponseResult();
            try
            {
                var sqlParams = new List<dynamic>
        {
            new SqlParameter("@Flag", "ParentGroupDetail"),
            new SqlParameter("@ParentAccountCode", parentAccountCode)
        };

                responseResult = await _IDataLogic.ExecuteDataTable("AccSpPrimaryAccountHeadMaster", sqlParams);
            }
            catch (Exception ex)
            {
                dynamic error = new ExpandoObject();
                error.Message = ex.Message;
                error.Source = ex.Source;
            }

            return responseResult;
        }
        public async Task<ResponseResult> SavePrimaryAccountGroupMaster(PrimaryAccountGroupMasterModel model)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                
                //string flag = model.EnteredEMPID > 0 ? "UPDATE" : "INSERT";
                var SqlParams = new List<dynamic>();
                if (model.Mode == "U")
                {
                    SqlParams.Add(new SqlParameter("@Flag", "UPDATE"));
                    SqlParams.Add(new SqlParameter("@AccountCode", model.Account_Code == 0 ? 0 : model.Account_Code));
                    SqlParams.Add(new SqlParameter("@AccountName", model.Account_Name == "" ? "" : model.Account_Name));
                    SqlParams.Add(new SqlParameter("@parentAccount", model.Parent_Account_Code == 0 ? "" : model.Parent_Account_Code));
                    SqlParams.Add(new SqlParameter("@Main_Group", model.Main_Group == "" ? "" : model.Main_Group));
                    SqlParams.Add(new SqlParameter("@SubGroup", model.SubGroup == "" ? "" : model.SubGroup));
                    SqlParams.Add(new SqlParameter("@SubSubGroup", model.SubSubGroup == 0 ? 0 : model.SubSubGroup));
                    SqlParams.Add(new SqlParameter("@UnderGroup", model.UnderGroup == "" ? "" : model.UnderGroup));
                    SqlParams.Add(new SqlParameter("@EnteredEMPID", model.EnteredEMPID == 0 ? 0 : model.EnteredEMPID));
                    SqlParams.Add(new SqlParameter("@EntryByMachineName", model.EntryByMachineName ?? ""));

                }
                else
                {
                    SqlParams.Add(new SqlParameter("@Flag", "INSERT"));

                    SqlParams.Add(new SqlParameter("@AccountName", model.Account_Name == "" ? "" : model.Account_Name));
                    SqlParams.Add(new SqlParameter("@parentAccount", model.Parent_Account_Code == 0 ? "" : model.Parent_Account_Code));
                    SqlParams.Add(new SqlParameter("@Main_Group", model.Main_Group == "" ? "" : model.Main_Group));
                    SqlParams.Add(new SqlParameter("@SubGroup", model.SubGroup == "" ? "" : model.SubGroup));
                    SqlParams.Add(new SqlParameter("@SubSubGroup", model.SubSubGroup == 0 ? 0 : model.SubSubGroup));
                    SqlParams.Add(new SqlParameter("@UnderGroup", model.UnderGroup == "" ? "" : model.UnderGroup));
                    SqlParams.Add(new SqlParameter("@EnteredEMPID", model.EnteredEMPID == 0 ? 0 : model.EnteredEMPID));
                    SqlParams.Add(new SqlParameter("@EntryByMachineName", model.EntryByMachineName ?? ""));
                }
                

                _ResponseResult = await _IDataLogic.ExecuteDataTable("AccSpPrimaryAccountHeadMaster", SqlParams);
                
            }                                                        

            catch (Exception ex)
            {
                _ResponseResult.StatusCode = HttpStatusCode.InternalServerError;
                _ResponseResult.StatusText = "Error";
                _ResponseResult.Result = new { ex.Message, ex.StackTrace };
            }
            return _ResponseResult;
        }
        public async Task<ResponseResult> UpdatePrimaryAccountGroupMaster(PrimaryAccountGroupMasterModel model)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>
        {
            new SqlParameter("@Flag", "UPDATE"),
            new SqlParameter("@AccountCode", model.Account_Code),
            new SqlParameter("@AccountName", string.IsNullOrEmpty(model.Account_Name) ? DBNull.Value : model.Account_Name),
            new SqlParameter("@parentAccount", model.Account_Code > 0 ? model.Account_Code : (object)DBNull.Value),
            new SqlParameter("@Main_Group", string.IsNullOrEmpty(model.Main_Group) ? DBNull.Value : model.Main_Group),
            new SqlParameter("@SubGroup", string.IsNullOrEmpty(model.SubGroup) ? DBNull.Value : model.SubGroup),
            new SqlParameter("@SubSubGroup", model.SubSubGroup > 0 ? model.SubSubGroup : (object)DBNull.Value),
            new SqlParameter("@UnderGroup", string.IsNullOrEmpty(model.UnderGroup) ? DBNull.Value : model.UnderGroup),
            new SqlParameter("@EnteredEMPID", model.EnteredEMPID),
            new SqlParameter("@EntryByMachineName", model.EntryByMachineName ?? "")

        };

                _ResponseResult = await _IDataLogic.ExecuteDataTable("AccSpPrimaryAccountHeadMaster", SqlParams);
            }
            catch (Exception ex)
            {
                _ResponseResult.StatusCode = HttpStatusCode.InternalServerError;
                _ResponseResult.StatusText = "Error";
                _ResponseResult.Result = new { ex.Message, ex.StackTrace };
            }
            return _ResponseResult;
        }

        public async Task<ResponseResult> GetDashboardData()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                DateTime currentDate = DateTime.Today;
                DateTime firstDateOfMonth = new DateTime(currentDate.Year, currentDate.Month, 1);

                // Initialize SQL parameters with the "DASHBOARD" flag
                var SqlParams = new List<dynamic>
        {
            new SqlParameter("@Flag", "DASHBOARD")
        };

                // Execute the stored procedure related to the dashboard
                _ResponseResult = await _IDataLogic.ExecuteDataSet("AccSpPrimaryAccountHeadMaster", SqlParams);
            }
            catch (Exception ex)
            {
                _ResponseResult.StatusCode = HttpStatusCode.InternalServerError;
                _ResponseResult.StatusText = "Error";
                _ResponseResult.Result = new { ex.Message, ex.StackTrace };
            }
            return _ResponseResult;
        }
        public async Task<PrimaryAccountGroupMasterDashBoardModel> GetDashboardDetailData(string Account_Name,string ParentAccountName)
        {
            DataSet? oDataSet = new DataSet();
            var model = new PrimaryAccountGroupMasterDashBoardModel();
            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    SqlCommand oCmd = new SqlCommand("AccSpPrimaryAccountHeadMaster", myConnection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    oCmd.Parameters.AddWithValue("@Flag", "DASHBOARD");
                    oCmd.Parameters.AddWithValue("@AccountName", Account_Name);
                    oCmd.Parameters.AddWithValue("@parentAccount", ParentAccountName);

                    await myConnection.OpenAsync();
                    using (SqlDataAdapter oDataAdapter = new SqlDataAdapter(oCmd))
                    {
                        oDataAdapter.Fill(oDataSet);
                    }
                }

                if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                {
                    model.PrimaryAccountGroupMasterDashBoardGrid = (from DataRow dr in oDataSet.Tables[0].Rows
                                                  select new PrimaryAccountGroupMasterDashBoardModel
                                                  {
                                                      Account_Code = Convert.ToInt32(dr["Account_Code"]),
                                                      Account_Name = dr["Account_Name"].ToString(),
                                                      ParentAccountName = dr["ParentAccountName"].ToString(),
                                                      Parent_Account_Code = Convert.ToInt32(dr["Parent_Account_Code"]),
                                                      Main_Group = dr["Main_Group"].ToString(),
                                                      SubGroup = dr["SubGroup"].ToString(),
                                                      SubSubGroup = Convert.ToInt32(dr["SubSubGroup"]),
                                                      UnderGroup = dr["UnderGroup"].ToString(),
                                                  }).ToList();
                }
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
                // Log or handle the error as needed
            }
            finally
            {
                oDataSet.Dispose();
            }

            return model;
        }
        public async Task<PrimaryAccountGroupMasterModel> GetViewByID(int accountCode)
        {
            var model = new PrimaryAccountGroupMasterModel();
            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@Flag", "ViewByID"));
                SqlParams.Add(new SqlParameter("@AccountCode", accountCode));
                var _ResponseResult = await _IDataLogic.ExecuteDataSet("AccSpPrimaryAccountHeadMaster", SqlParams);

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
        private static PrimaryAccountGroupMasterModel PrepareView(DataSet DS, ref PrimaryAccountGroupMasterModel? model)
        {
            try
            {
                var ItemList = new List<PrimaryAccountGroupMasterModel>();
                DS.Tables[0].TableName = "SSMain";
                int cnt = 0;
                model.Account_Name = DS.Tables[0].Rows[0]["Account_Name"].ToString();
                model.Parent_Account_Code =Convert.ToInt32(DS.Tables[0].Rows[0]["Parent_Account_Code"].ToString());
                model.ParentAccountName = DS.Tables[0].Rows[0]["ParentAccountName"].ToString();
                model.Main_Group = DS.Tables[0].Rows[0]["Main_Group"].ToString();
                model.SubGroup = DS.Tables[0].Rows[0]["SubGroup"].ToString();
                model.SubSubGroup =Convert.ToInt32(DS.Tables[0].Rows[0]["SubSubGroup"].ToString());
                model.UnderGroup = DS.Tables[0].Rows[0]["UnderGroup"].ToString();
                model.Account_Code = Convert.ToInt32(DS.Tables[0].Rows[0]["Account_Code"].ToString());

                if (DS.Tables.Count != 0 && DS.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow row in DS.Tables[0].Rows)
                    {
                        ItemList.Add(new PrimaryAccountGroupMasterModel
                        {
                            //SrNO = Convert.ToInt32(row["SrNO"].ToString()),
                            Account_Name = row["Account_Name"].ToString(),
                            Parent_Account_Code =Convert.ToInt32(row["Parent_Account_Code"].ToString()),
                            ParentAccountName = row["ParentAccountName"].ToString(),
                            Main_Group = row["Main_Group"].ToString(),
                            SubGroup = row["SubGroup"].ToString(),
                            SubSubGroup = Convert.ToInt32(row["SubSubGroup"].ToString()),
                            UnderGroup = row["UnderGroup"].ToString(),
                           
                        });
                    }
                    model.PrimaryAccountGroupMasterGrid = ItemList;
                }
                return model;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<ResponseResult> DeleteByID( int AccountCode)
        {
            //var _ResponseResult = new ResponseResult();
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "DELETE"));
                SqlParams.Add(new SqlParameter("@AccountCode", AccountCode));
                //_ResponseResult = await _IDataLogic.ExecuteDataTable("AccSpPrimaryAccountHeadMaster", SqlParams);
                var response = await _IDataLogic.ExecuteDataTable("AccSpPrimaryAccountHeadMaster", SqlParams);

                if (response.Result is DataTable dt && dt.Rows.Count > 0)
                {
                    _ResponseResult.StatusCode = (HttpStatusCode)Convert.ToInt32(dt.Rows[0]["StatusCode"]);
                    _ResponseResult.StatusText = dt.Rows[0]["StatusText"].ToString();

                    if (dt.Columns.Contains("msg"))
                    {
                        _ResponseResult.StatusText = dt.Rows[0]["msg"].ToString(); // Extract error message
                    }
                }
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
