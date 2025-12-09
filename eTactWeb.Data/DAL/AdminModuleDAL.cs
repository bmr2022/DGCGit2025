using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.Extensions.Configuration;
using Org.BouncyCastle.Ocsp;
using System.Data;
using System.Reflection;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Data.DAL
{
    public class AdminModuleDAL
    {
        private readonly string DBConnectionString = string.Empty;
        private readonly IDataLogic _IDataLogic;
        private IDataReader? Reader;
        //private readonly IConfiguration configuration;
        private readonly ConnectionStringService _connectionStringService;

        public AdminModuleDAL(IConfiguration configuration, IDataLogic iDataLogic,ConnectionStringService connectionStringService)
        {
            //configuration = config;
            _connectionStringService = connectionStringService;
            DBConnectionString = connectionStringService.GetConnectionString();
            //DBConnectionString = configuration.GetConnectionString("eTactDB");
            _IDataLogic = iDataLogic;
        }

        public ResponseResult DeleteRightByID(int ID)
        {
            dynamic _ResponseResult = null;

            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    SqlCommand oCmd = new SqlCommand("SP_UserRights", myConnection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    oCmd.Parameters.AddWithValue("@UID", ID);
                    oCmd.Parameters.AddWithValue("@Flag", "DeleteByID");

                    myConnection.Open();
                    Reader = oCmd.ExecuteReader();
                    if (Reader != null)
                    {
                        while (Reader.Read())
                        {
                            _ResponseResult = new ResponseResult()
                            {
                                StatusCode = Convert.ToInt32(Reader["StatusCode"].ToString()) == 410
                                    ? HttpStatusCode.Gone
                                    : HttpStatusCode.BadRequest,
                                StatusText = "Success",
                                Result = Reader["Result"].ToString()
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _ResponseResult.StatusCode = HttpStatusCode.InternalServerError;
                _ResponseResult.StatusText = "Exception";
                _ResponseResult.Result = ex.Message.ToString();
                return _ResponseResult;
            }
            finally
            {
                if (Reader != null)
                {
                    Reader.Close();
                    Reader.Dispose();
                }
            }

            return _ResponseResult;
        }

        public async Task<ResponseResult> DeleteUserByID(int ID)
        {
            dynamic _ResponseResult = null;

            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    var SqlParams = new List<dynamic>();
                    SqlParams.Add(new SqlParameter("@Flag", "DeleteByID"));
                    SqlParams.Add(new SqlParameter("@EmpID", ID));
                    

                   
                   _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_UserMaster", SqlParams);
                }
            }
            catch (Exception ex)
            {
                _ResponseResult.StatusCode = HttpStatusCode.InternalServerError;
                _ResponseResult.StatusText = "Exception";
                _ResponseResult.Result = ex.Message.ToString();
                return _ResponseResult;
            }
            finally
            {
                if (Reader != null)
                {
                    Reader.Close();
                    Reader.Dispose();
                }
            }

            return _ResponseResult;
        }

        public async Task<IList<UserMasterModel>> GetDashBoardData(string Flag, string Usertype, string EmpCode, string EmpName, string UserName)
        {
            var UserMasterList = new List<UserMasterModel>();
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
                                      select new UserMasterModel
                                      {
                                          ID = Convert.ToInt32(dr["UID"]),
                                          EmpID = dr["EmpID"].ToString(),
                                          EmpCode = dr["EmpCode"].ToString(),
                                          UserType = dr["UserType"].ToString(),
                                          UserName = dr["UserName"].ToString(),
                                          MobileNo = dr["MobileNo"].ToString(),
                                          EmailID = dr["EmailID"].ToString(),
                                          CreatedBy = string.IsNullOrEmpty(dr["CreatedBy"].ToString()) ? 0 : Convert.ToInt32(dr["CreatedBy"].ToString()),
                                          CreatedOn = string.IsNullOrEmpty(oDataSet.Tables[0].Rows[0]["CreatedOn"].ToString()) ? new DateTime() : Convert.ToDateTime(oDataSet.Tables[0].Rows[0]["CreatedOn"]),
                                          Active = dr["Active"].ToString(),
                                          EmpName = string.IsNullOrEmpty(dr["EmpName"].ToString()) ? "" : dr["EmpName"].ToString(),

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


        public async Task<IList<TextValue>> GetMenuList(string Flag, string Module, string MainMenu)
        {
            List<TextValue>? _List = new List<TextValue>();
            dynamic Listval = new TextValue();

            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    SqlCommand oCmd = new SqlCommand("SP_GetMenuList", myConnection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    oCmd.Parameters.AddWithValue("@Flag", Flag);
                    oCmd.Parameters.AddWithValue("@Module", Module);
                    oCmd.Parameters.AddWithValue("@MainMenu", MainMenu);

                    await myConnection.OpenAsync();

                    Reader = await oCmd.ExecuteReaderAsync();

                    if (Reader != null)
                    {
                        while (Reader.Read())
                        {
                            if (Flag == "Module")
                            {
                                Listval = new TextValue()
                                {
                                    Text = Reader["MainModule"].ToString(),
                                    Value = Reader["MainModule"].ToString()
                                };
                            }
                            else if (Flag == "MainMenu")
                            {
                                Listval = new TextValue()
                                {
                                    Text = Reader["MainMenuHeading"].ToString(),
                                    Value = Reader["MainMenuHeading"].ToString()
                                };
                            }
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
        public async Task<IList<TextValue>> GetUserList(string ShowAllUsers)
        {
            List<TextValue>? _List = new List<TextValue>();
            dynamic Listval = new TextValue();

            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    SqlCommand oCmd = new SqlCommand("SP_UserRights", myConnection)
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
                                Value = Reader["UID"].ToString()
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

        public async Task<UserMasterModel> GetUserByID(int ID)
        {
            UserMasterModel? _UserMasterModel = new UserMasterModel();
            try
            {
                DataTable? oDataTable = new DataTable();
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    SqlCommand oCmd = new SqlCommand("SP_UserMaster", myConnection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    oCmd.Parameters.AddWithValue("@EmpID", ID);
                    oCmd.Parameters.AddWithValue("@Flag", "GetByID");
                    await myConnection.OpenAsync();
                    using (SqlDataAdapter oDataAdapter = new SqlDataAdapter(oCmd))
                    {
                        oDataAdapter.Fill(oDataTable);
                    }
                }

                if (oDataTable.Rows.Count > 0)
                {
                    foreach (DataRow dr in oDataTable.Rows)
                    {
                        _UserMasterModel.ID = Convert.ToInt32(dr["UID"]);
                        _UserMasterModel.EmpID = dr["EmpID"].ToString();
                        _UserMasterModel.EmpCode = dr["EmpCode"].ToString();
                        _UserMasterModel.UserType = dr["UserType"].ToString();
                        _UserMasterModel.UserName = dr["UserName"].ToString();
                        _UserMasterModel.MobileNo = dr["MobileNo"].ToString();
                        _UserMasterModel.EmailID = dr["EmailID"].ToString();
                        _UserMasterModel.Password = dr["Password"].ToString();
                        _UserMasterModel.CnfPass = dr["Password"].ToString();
                        _UserMasterModel.CreatedBy = string.IsNullOrEmpty(dr["CreatedBy"].ToString()) ? 0 : Convert.ToInt32(dr["CreatedBy"].ToString());
                        _UserMasterModel.CreatedOn = string.IsNullOrEmpty(dr["CreatedOn"].ToString()) ? new DateTime() : Convert.ToDateTime(dr["CreatedOn"]);
                        _UserMasterModel.UpdatedOn = string.IsNullOrEmpty(dr["UpdatedOn"].ToString()) ? new DateTime() : Convert.ToDateTime(dr["UpdatedOn"]);
                        _UserMasterModel.UpdatedBy = string.IsNullOrEmpty(dr["UpdatedBy"].ToString()) ? 0 : Convert.ToInt32(dr["UpdatedBy"].ToString());
                        _UserMasterModel.UpdatedByName = string.IsNullOrEmpty(dr["UpdatedByName"].ToString()) ? "" : dr["UpdatedByName"].ToString();
                        _UserMasterModel.CreatedByName = string.IsNullOrEmpty(dr["CreatedByName"].ToString()) ? "" : dr["CreatedByName"].ToString();
                        _UserMasterModel.Active = dr["Active"].ToString();
                    }
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

            return _UserMasterModel;
        }
        public async Task<ResponseResult> CheckAdminExists()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@Flag", "AdminExists"));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_UserMaster", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> GetAllUserRights(int EmpId)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@Flag", "GetAllUserRights1"));
                SqlParams.Add(new SqlParameter("@EmpID", EmpId));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_UserMaster", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }

        public async Task<UserRightModel> GetUserRightByID(int ID)
        {
            UserRightModel? _UserRightModel = new UserRightModel();
            try
            {
                DataTable? oDataTable = new DataTable();
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    SqlCommand oCmd = new SqlCommand("SP_UserRights", myConnection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    oCmd.Parameters.AddWithValue("@UID", ID);
                    oCmd.Parameters.AddWithValue("@Flag", "GetByID");
                    await myConnection.OpenAsync();
                    using (SqlDataAdapter oDataAdapter = new SqlDataAdapter(oCmd))
                    {
                        oDataAdapter.Fill(oDataTable);
                    }
                }

                if (oDataTable.Rows.Count > 0)
                {
                    foreach (DataRow dr in oDataTable.Rows)
                    {
                        _UserRightModel.ID = Convert.ToInt32(dr["UID"]);
                        _UserRightModel.EmpID = Convert.ToInt32(dr["EmpID"].ToString());
                        _UserRightModel.EmpName = dr["EmpName"].ToString();
                        _UserRightModel.Module = dr["Module"].ToString();
                        _UserRightModel.MainMenu = dr["MainMenu"].ToString();
                        //_UserRightModel.SubMenu = dr["SubMenu"].ToString();
                        _UserRightModel.All = dr["OptAll"].ToString() == "True" ? "true" : "false";
                        _UserRightModel.Save = dr["OptSave"].ToString() == "True" ? "true" : "false";
                        _UserRightModel.Update = dr["OptUpdate"].ToString() == "True" ? "true" : "false";
                        _UserRightModel.Delete = dr["OptDelete"].ToString() == "True" ? "true" : "false";
                        _UserRightModel.View = dr["OptView"].ToString() == "True" ? "true" : "false";
                        _UserRightModel.CreatedBy = Convert.ToInt32(dr["CreatedBy"]);
                        _UserRightModel.CreatedOn = Convert.ToDateTime(dr["CreatedOn"]);
                        _UserRightModel.Active = dr["Active"].ToString();
                    }
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
        public async Task<UserRightModel> GetSearchData(string EmpName, string UserName, string Module, string MainMenu)
        {
            DataSet? oDataSet = new DataSet();
            var _UserRightModel = new UserRightModel();
            try
            {
                DataTable? oDataTable = new DataTable();
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    SqlCommand oCmd = new SqlCommand("SP_UserRights", myConnection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    oCmd.Parameters.AddWithValue("@EmpName", EmpName);
                    oCmd.Parameters.AddWithValue("@UserName", UserName);
                    //oCmd.Parameters.AddWithValue("@SubMenu", SubMenu);
                    oCmd.Parameters.AddWithValue("@Flag", "SEARCH");
                    await myConnection.OpenAsync();
                    using (SqlDataAdapter oDataAdapter = new SqlDataAdapter(oCmd))
                    {
                        oDataAdapter.Fill(oDataSet);
                    }
                }

                if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                {
                    _UserRightModel.UserRights = (from DataRow dr in oDataSet.Tables[0].Rows
                                                  select new UserRightModel
                                                  {
                                                      ID = string.IsNullOrEmpty(dr["UID"].ToString()) ? 0 : Convert.ToInt32(dr["UID"].ToString()),
                                                      EmpID = string.IsNullOrEmpty(dr["EmpID"].ToString()) ? 0 : Convert.ToInt32(dr["EmpID"].ToString()),
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

            return _UserRightModel;
        }
        public async Task<UserRightModel> GetSearchData(int EmpID)
        {
            DataSet? oDataSet = new DataSet();
            var _UserRightModel = new UserRightModel();
            try
            {
                DataTable? oDataTable = new DataTable();
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    SqlCommand oCmd = new SqlCommand("SP_UserRights", myConnection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    oCmd.Parameters.AddWithValue("@EmpID", EmpID);
                    oCmd.Parameters.AddWithValue("@Flag", "SEARCHBYUSER");
                    await myConnection.OpenAsync();
                    using (SqlDataAdapter oDataAdapter = new SqlDataAdapter(oCmd))
                    {
                        oDataAdapter.Fill(oDataSet);
                    }
                }
                _UserRightModel.UserRights = new List<UserRightModel>();
                int Cntr = 1;
                if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                {
                    _UserRightModel.UserRights = (from DataRow dr in oDataSet.Tables[0].Rows
                                                  select new UserRightModel
                                                  {
                                                      Seqno = Cntr++,
                                                      ID = Convert.ToInt32(dr["UID"]),
                                                      EmpID = Convert.ToInt32(dr["EmpID"].ToString()),
                                                      EmpName = dr["EmpName"].ToString(),
                                                      Module = dr["Module"].ToString(),
                                                      MainMenu = dr["MainMenu"].ToString(),
                                                      //SubMenu = dr["SubMenu"].ToString(),
                                                      All = dr["OptAll"].ToString() == "True" ? "true" : "false",
                                                      Save = dr["OptSave"].ToString() == "True" ? "true" : "false",
                                                      Update = dr["OptUpdate"].ToString() == "True" ? "true" : "false",
                                                      Delete = dr["OptDelete"].ToString() == "True" ? "true" : "false",
                                                      View = dr["OptView"].ToString() == "True" ? "true" : "false",
                                                      CreatedBy = string.IsNullOrEmpty(dr["CreatedBy"].ToString()) ? 0 : Convert.ToInt32(dr["CreatedBy"].ToString()),
                                                      CreatedOn = string.IsNullOrEmpty(dr["CreatedOn"].ToString()) ? new DateTime() : Convert.ToDateTime(dr["CreatedOn"]),
                                                      Active = dr["Active"].ToString(),
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
        public async Task<UserRightModel> GetSearchDetailData(string EmpName, string UserName)
        {
            DataSet? oDataSet = new DataSet();
            var _UserRightModel = new UserRightModel();
            try
            {
                DataTable? oDataTable = new DataTable();
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    SqlCommand oCmd = new SqlCommand("SP_UserRights", myConnection)
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
                _UserRightModel.UserRights = new List<UserRightModel>();
                int Cntr = 1;
                if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                {
                    _UserRightModel.UserRights = (from DataRow dr in oDataSet.Tables[0].Rows
                                                  select new UserRightModel
                                                  {
                                                      Seqno = Cntr++,
                                                      ID = Convert.ToInt32(dr["UID"]),
                                                      EmpID = Convert.ToInt32(dr["EmpID"].ToString()),
                                                      EmpName = dr["EmpName"].ToString(),
                                                      UserName = dr["UserName"].ToString(),
                                                      Module = dr["Module"].ToString(),
                                                      MainMenu = dr["MainMenu"].ToString(),
                                                      UserType = dr["UserType"].ToString(),
                                                      //SubMenu = dr["SubMenu"].ToString(),
                                                      All = dr["OptAll"].ToString() == "True" ? "true" : "false",
                                                      Save = dr["OptSave"].ToString() == "True" ? "true" : "false",
                                                      Update = dr["OptUpdate"].ToString() == "True" ? "true" : "false",
                                                      Delete = dr["OptDelete"].ToString() == "True" ? "true" : "false",
                                                      View = dr["OptView"].ToString() == "True" ? "true" : "false",
                                                      CreatedBy = string.IsNullOrEmpty(dr["CreatedBy"].ToString()) ? 0 : Convert.ToInt32(dr["CreatedBy"].ToString()),
                                                      CreatedOn = string.IsNullOrEmpty(dr["CreatedOn"].ToString()) ? new DateTime() : Convert.ToDateTime(dr["CreatedOn"]),
                                                      Active = dr["Active"].ToString(),
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

        public async Task<List<UserRightModel>> GetUserRightDashboard(string Flag)
        {
            List<UserRightModel>? UserRightList = new List<UserRightModel>();
            DataSet? oDataSet = new DataSet();

            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    SqlCommand oCmd = new SqlCommand("SP_UserRights", myConnection)
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
                                     select new UserRightModel
                                     {
                                         ID = string.IsNullOrEmpty(dr["UID"].ToString()) ? 0 : Convert.ToInt32(dr["UID"].ToString()),
                                         EmpID = string.IsNullOrEmpty(dr["EmpID"].ToString()) ? 0 : Convert.ToInt32(dr["EmpID"].ToString()),
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

        public async Task<ResponseResult> SaveUserMaster(UserMasterModel model)
        {
            dynamic _ResponseResult = null;

            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    SqlCommand oCmd = new SqlCommand("SP_UserMaster", myConnection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    oCmd.Parameters.AddWithValue("@Flag", model.Mode);
                    //oCmd.Parameters.AddWithValue("@RowID", model.ID);
                    oCmd.Parameters.AddWithValue("@UserType", model.UserType);
                    oCmd.Parameters.AddWithValue("@UserName", model.UserName);
                    oCmd.Parameters.AddWithValue("@EmpID", model.EmpID);
                    oCmd.Parameters.AddWithValue("@EmpCode", model.EmpCode);
                    oCmd.Parameters.AddWithValue("@Password", model.CnfPass);
                    oCmd.Parameters.AddWithValue("@MobileNo", model.MobileNo);
                    oCmd.Parameters.AddWithValue("@EmailID", model.EmailID);
                    oCmd.Parameters.AddWithValue("@CreatedBy", model.CreatedBy);
                    oCmd.Parameters.AddWithValue("@Active", model.Active);

                    myConnection.Open();
                    Reader = await oCmd.ExecuteReaderAsync();
                    if (Reader != null)
                    {
                        while (Reader.Read())
                        {
                            _ResponseResult = new ResponseResult()
                            {
                                StatusCode = (HttpStatusCode)Reader["StatusCode"],
                                StatusText = "Success",
                                Result = Reader["Result"].ToString()
                            };
                        }
                    }
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

            return _ResponseResult;
        }

        public async Task<ResponseResult> SaveUserRights(UserRightModel model, DataTable URGrid)
        {
            dynamic _ResponseResult = null;

            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    SqlCommand oCmd = new SqlCommand("SP_UserRights", myConnection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    oCmd.Parameters.AddWithValue("@Flag", model.Mode);
                    oCmd.Parameters.AddWithValue("@UID", model.ID);
                    oCmd.Parameters.AddWithValue("@EmpID", model.EmpID);
                    oCmd.Parameters.AddWithValue("@EmpName", model.EmpName);
                    oCmd.Parameters.AddWithValue("@Module", model.Module);
                    oCmd.Parameters.AddWithValue("@MainMenu", model.MainMenu);
                    oCmd.Parameters.AddWithValue("@CreatedBy", model.CreatedBy);
                    oCmd.Parameters.AddWithValue("@DTItemGrid", URGrid);

                    myConnection.Open();
                    Reader = await oCmd.ExecuteReaderAsync();
                    if (Reader != null)
                    {
                        while (Reader.Read())
                        {
                            _ResponseResult = new ResponseResult()
                            {
                                StatusCode = (HttpStatusCode)Reader["StatusCode"],
                                StatusText = Reader["StatusText"].ToString(),
                                Result = Reader["Result"].ToString()
                            };
                        }
                    }
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
            return _ResponseResult;
        }

        public async Task<ResponseResult> GetUserCount()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@Flag", "UserCount"));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_UserMaster", SqlParams);
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