using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.Extensions.Configuration;
using Org.BouncyCastle.Ocsp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Data.DAL
{
    public class XONUserRightDashboardDAL
    {
        private readonly IDataLogic _IDataLogic;
        private readonly string DBConnectionString = string.Empty;
        private IDataReader? Reader;
        private readonly ConnectionStringService _connectionStringService;

        public XONUserRightDashboardDAL(IConfiguration configuration, IDataLogic iDataLogic, ConnectionStringService connectionStringService)
        {
            _IDataLogic = iDataLogic;
            _connectionStringService = connectionStringService;
            DBConnectionString = _connectionStringService.GetConnectionString();
        }
        public async Task<IList<TextValue>> GetUserList(string ShowAllUsers)
        {
            List<TextValue>? _List = new List<TextValue>();
            dynamic Listval = new TextValue();

            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    SqlCommand oCmd = new SqlCommand("SP_UserRightDashboard", myConnection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    oCmd.Parameters.AddWithValue("@Flag", "GetUserList");
                    oCmd.Parameters.AddWithValue("@ShowAllUser", ShowAllUsers);

                    await myConnection.OpenAsync();

                    Reader = await oCmd.ExecuteReaderAsync();

                    if (Reader != null)
                    {
                        while (Reader.Read())
                        {
                            Listval = new TextValue()
                            {
                                Text = Reader["UserName"].ToString(),
                                Value = Reader["UID"].ToString(),
                                EmpId = Convert.ToInt32(Reader["EmpID"])
                            };
                            _List.Add(Listval);
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
            finally
            {
                if (Reader != null)
                {
                    Reader.Close();
                    Reader.Dispose();
                }
            }
            return _List;
        }
        public async Task<IList<TextValue>> GetDashboardName()
        {
            List<TextValue>? _List = new List<TextValue>();
            dynamic Listval = new TextValue();

            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    SqlCommand oCmd = new SqlCommand("SP_UserRightDashboard", myConnection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    oCmd.Parameters.AddWithValue("@Flag", "DashboardName");

                    await myConnection.OpenAsync();

                    Reader = await oCmd.ExecuteReaderAsync();

                    if (Reader != null)
                    {
                        while (Reader.Read())
                        {
                            Listval = new TextValue()
                            {
                                Text = Reader["DashboardName"].ToString(),
                                Value = Reader["DashboardName"].ToString()
                            };
                            _List.Add(Listval);
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
            finally
            {
                if (Reader != null)
                {
                    Reader.Close();
                    Reader.Dispose();
                }
            }
            return _List;
        }
        public async Task<ResponseResult> GetDashboardSubScreen(string DashboardName)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@Flag", "DashboardSubScreen"));
                SqlParams.Add(new SqlParameter("@DashboardName", DashboardName));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_UserRightDashboard", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> SaveUserRightDashboard(UserRightDashboardModel model, DataTable UserRightDashboardGrid)
        {
            var _ResponseResult = new ResponseResult();
            try
            {

                var SqlParams = new List<dynamic>();
                if (model.Mode == "U" || model.Mode == "V")
                {
                    SqlParams.Add(new SqlParameter("@Flag", "UPDATE"));
                    SqlParams.Add(new SqlParameter("@UpdatedBy", model.UpdatedById));
                }
                else
                {
                    SqlParams.Add(new SqlParameter("@Flag", "INSERT"));
                    SqlParams.Add(new SqlParameter("@CreatedBy", model.CreatedById));
                }
                SqlParams.Add(new SqlParameter("@DTUserRightDashboardGrid", UserRightDashboardGrid));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_UserRightDashboard", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }
        public async Task<List<UserRightDashboardModel>> GetUserRightDashboard(string Flag)
        {
            List<UserRightDashboardModel>? UserRightList = new List<UserRightDashboardModel>();
            DataSet? oDataSet = new DataSet();

            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    SqlCommand oCmd = new SqlCommand("SP_UserRightDashboard", myConnection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    oCmd.Parameters.AddWithValue("@Flag", Flag);
                    await myConnection.OpenAsync();
                    using (SqlDataAdapter oDataAdapter = new SqlDataAdapter(oCmd))
                    {
                        oDataAdapter.Fill(oDataSet);
                    }
                }

                if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                {
                    UserRightList = (from DataRow dr in oDataSet.Tables[0].Rows
                                     select new UserRightDashboardModel
                                     {
                                         UserId = string.IsNullOrEmpty(dr["UID"].ToString()) ? 0 : Convert.ToInt32(dr["UID"].ToString()),
                                         EmpId = string.IsNullOrEmpty(dr["EmpID"].ToString()) ? 0 : Convert.ToInt32(dr["EmpID"].ToString()),
                                         EmpName = dr["EmpName"].ToString(),
                                         UserName = dr["UserName"].ToString()
                                     }).ToList();
                }
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
                return Error;
            }
            finally
            {
                if (Reader != null)
                {
                    Reader.Close();
                    Reader.Dispose();
                }
            }
            return UserRightList;
        }
        public async Task<IList<UserRightDashboardModel>> GetDashBoardData(string Flag, string Usertype, string EmpCode, string EmpName, string UserName)
        {
            var UserMasterList = new List<UserRightDashboardModel>();
            var oDataSet = new DataSet();

            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    SqlCommand oCmd = new SqlCommand("SP_UserMaster", myConnection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    oCmd.Parameters.AddWithValue("@Flag", Flag);
                    oCmd.Parameters.AddWithValue("@UserType", Usertype);
                    oCmd.Parameters.AddWithValue("@EmpCode", EmpCode);
                    oCmd.Parameters.AddWithValue("@EmpName", EmpName);
                    oCmd.Parameters.AddWithValue("@UserName", UserName);
                    await myConnection.OpenAsync();
                    using (SqlDataAdapter oDataAdapter = new SqlDataAdapter(oCmd))
                    {
                        oDataAdapter.Fill(oDataSet);
                    }
                }

                if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                {
                    UserMasterList = (from DataRow dr in oDataSet.Tables[0].Rows
                                      select new UserRightDashboardModel
                                      {
                                          UserId = Convert.ToInt32(dr["UID"]),
                                          EmpId = Convert.ToInt32(dr["EmpID"]),
                                          UserName = dr["UserName"].ToString(),
                                          CreatedById = string.IsNullOrEmpty(dr["CreatedBy"].ToString()) ? 0 : Convert.ToInt32(dr["CreatedBy"].ToString()),
                                          CreatedByName = dr["CreatedByName"].ToString(),
                                          CreatedOn = string.IsNullOrEmpty(oDataSet.Tables[0].Rows[0]["CreatedOn"].ToString()) ? new DateTime() : Convert.ToDateTime(oDataSet.Tables[0].Rows[0]["CreatedOn"]),
                                          EmpName = string.IsNullOrEmpty(dr["EmpName"].ToString()) ? "" : dr["EmpName"].ToString()

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
                if (Reader != null)
                {
                    Reader.Close();
                    Reader.Dispose();
                }
            }

            return UserMasterList;
        }
        public async Task<UserRightDashboardModel> GetSearchData(string EmpName, string UserName, string DashboardName, string DashboardSubScreen)
        {
            DataSet? oDataSet = new DataSet();
            var _UserRightDashboardModel = new UserRightDashboardModel();
            try
            {
                DataTable? oDataTable = new DataTable();
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    SqlCommand oCmd = new SqlCommand("SP_UserRightDashboard ", myConnection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    oCmd.Parameters.AddWithValue("@EmpName", EmpName);
                    oCmd.Parameters.AddWithValue("@UserName", UserName);
                    oCmd.Parameters.AddWithValue("@Flag", "SEARCH");
                    await myConnection.OpenAsync();
                    using (SqlDataAdapter oDataAdapter = new SqlDataAdapter(oCmd))
                    {
                        oDataAdapter.Fill(oDataSet);
                    }
                }

                if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                {
                    _UserRightDashboardModel.UserRightsDashboard = (from DataRow dr in oDataSet.Tables[0].Rows
                                                                    select new UserRightDashboardModel
                                                                    {
                                                                        UserId = string.IsNullOrEmpty(dr["UID"].ToString()) ? 0 : Convert.ToInt32(dr["UID"].ToString()),
                                                                        EmpId = string.IsNullOrEmpty(dr["Empid"].ToString()) ? 0 : Convert.ToInt32(dr["Empid"].ToString()),
                                                                        EmpName = dr["Emp_Name"].ToString(),
                                                                        UserName = dr["UserName"].ToString()
                                                                    }).ToList();
                }
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
                return Error;
            }
            finally
            {
                if (Reader != null)
                {
                    Reader.Close();
                    Reader.Dispose();
                }
            }

            return _UserRightDashboardModel;
        }
        public async Task<UserRightDashboardModel> GetSearchDetailData(string EmpName, string UserName)
        {
            DataSet? oDataSet = new DataSet();
            var _UserRightModel = new UserRightDashboardModel();
            try
            {
                DataTable? oDataTable = new DataTable();
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    SqlCommand oCmd = new SqlCommand("SP_UserRightDashboard", myConnection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    oCmd.Parameters.AddWithValue("@EmpName", EmpName);
                    oCmd.Parameters.AddWithValue("@UserName", UserName);
                    oCmd.Parameters.AddWithValue("@Flag", "DetailDashboard");
                    await myConnection.OpenAsync();
                    using (SqlDataAdapter oDataAdapter = new SqlDataAdapter(oCmd))
                    {
                        oDataAdapter.Fill(oDataSet);
                    }
                }
                _UserRightModel.UserRightsDashboard = new List<UserRightDashboardModel>();
                int Cntr = 1;
                if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                {
                    _UserRightModel.UserRightsDashboard = (from DataRow dr in oDataSet.Tables[0].Rows
                                                  select new UserRightDashboardModel
                                                  {
                                                      SeqNo = Cntr++,
                                                      UserId = Convert.ToInt32(dr["UID"]),
                                                      EmpId = Convert.ToInt32(dr["EmpID"].ToString()),
                                                      EmpName = dr["Emp_Name"].ToString(),
                                                      UserName = dr["UserName"].ToString(),
                                                      DashboardName = dr["DashboardName"].ToString(),
                                                      DashboardSubScreen = dr["DashboardSubScreen"].ToString(),
                                                      IsView = (bool)dr["IsView"],
                                                      CreatedById = string.IsNullOrEmpty(dr["CreatedBy"].ToString()) ? 0 : Convert.ToInt32(dr["CreatedBy"].ToString()),
                                                      CreatedOn = string.IsNullOrEmpty(dr["CreatedOn"].ToString()) ? new DateTime() : Convert.ToDateTime(dr["CreatedOn"]),
                                                  }).ToList();
                }
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
                return Error;
            }
            finally
            {
                if (Reader != null)
                {
                    Reader.Close();
                    Reader.Dispose();
                }
            }

            return _UserRightModel;
        }
    }
}
