using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.Extensions.Configuration;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Data.DAL
{
    public class ProcessMasterDAL
    {
        private readonly string DBConnectionString = string.Empty;
        private IDataReader? Reader;

        private readonly IDataLogic _DataLogicDAL;

        public ProcessMasterDAL(IConfiguration configuration, IDataLogic dataLogicDAL)
        {
            DBConnectionString = configuration.GetConnectionString("eTactDB");
            _DataLogicDAL = dataLogicDAL;
        }

        public async Task<ResponseResult> DeleteByID(int ID)
        {
            dynamic _ResponseResult = null;
            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    SqlCommand oCmd = new SqlCommand("SPProcessMaster", myConnection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    oCmd.Parameters.AddWithValue("@ProcessId", ID);
                    oCmd.Parameters.AddWithValue("@Flag", "DeleteByID");
                    await myConnection.OpenAsync();
                    Reader = await oCmd.ExecuteReaderAsync();
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
                                Result = Reader["Result"].ToString() ?? string.Empty
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

        public async Task<ProcessMasterModel> GetByID(int ID)
        {
            ProcessMasterModel? _ItemCategoryModel = new ProcessMasterModel();
            DataTable? oDataTable = new DataTable();

            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    SqlCommand oCmd = new SqlCommand("SPProcessMaster", myConnection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    oCmd.Parameters.AddWithValue("@ProcessId", ID);
                    oCmd.Parameters.AddWithValue("@Flag", "ViewByID");
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
                        _ItemCategoryModel.Process_Id = Convert.ToInt32(dr["ProcessId"].ToString());
                        _ItemCategoryModel.StageCode = dr["StageCode"].ToString();
                        _ItemCategoryModel.StageDescription = dr["StageDescription"].ToString();
                        _ItemCategoryModel.StageShortName = dr["StageShortName"].ToString();
                        _ItemCategoryModel.MaterialReqForRework = Convert.ToInt32(dr["MaterialReqForRework"]);
                        _ItemCategoryModel.QCRequired = Convert.ToInt32(dr["QCRequired"]);
                        _ItemCategoryModel.UID = Convert.ToInt32(dr["UID"].ToString());
                        _ItemCategoryModel.CC = Convert.ToString(dr["CC"].ToString());
                        _ItemCategoryModel.MaxPendQtyReq = Convert.ToInt32(dr["MaxPendQtyReq"].ToString());
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
                oDataTable.Dispose();
            }

            return _ItemCategoryModel;
        }

        public async Task<ProcessMasterModel> GetDashboardData(ProcessMasterModel model)
        {
            DataSet? oDataSet = new DataSet();

            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    SqlCommand oCmd = new SqlCommand("SPProcessMaster", myConnection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    oCmd.Parameters.AddWithValue("@Flag", model.Mode);
                    await myConnection.OpenAsync();
                    using (SqlDataAdapter oDataAdapter = new SqlDataAdapter(oCmd))
                    {
                        oDataAdapter.Fill(oDataSet);
                    }
                }
                model.ProcessMasterList = model.ProcessMasterList ?? new List<ProcessMasterModel>();
                if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                {
                    model.ProcessMasterList = (from DataRow dr in oDataSet.Tables[0].Rows
                                               select new ProcessMasterModel
                                               {
                                                   Process_Id = Convert.ToInt32(dr["ProcessId"]),
                                                   StageCode = dr["StageCode"].ToString(),
                                                   StageDescription = dr["StageDescription"].ToString(),
                                                   StageShortName = dr["StageShortName"].ToString(),
                                                   MaterialReqForRework = Convert.ToInt32(dr["MaterialReqForRework"]),
                                                   QCRequired =Convert.ToInt32(dr["QCRequired"]),
                                                   CC = dr["CC"].ToString(),
                                                   UID = Convert.ToInt32(dr["Uid"].ToString()),
                                                   MaxPendQtyReq = Convert.ToInt32(dr["MaxPendQtyReq"].ToString()),
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

        public async Task<ResponseResult> GetFormRights(int userId)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "GetRights"));
                SqlParams.Add(new SqlParameter("@EmpId", userId));
                SqlParams.Add(new SqlParameter("@MainMenu", "Process Master"));

                _ResponseResult = await _DataLogicDAL.ExecuteDataSet("SP_ItemGroup", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }

        public async Task<ProcessMasterModel> GetSearchData(ProcessMasterModel model, string StageCode, string StageShortName)
        {
            DataSet? oDataSet = new DataSet();

            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    SqlCommand oCmd = new SqlCommand("SPProcessMaster", myConnection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    oCmd.Parameters.AddWithValue("@Flag", model.Mode);
                    oCmd.Parameters.AddWithValue("@StageCode", StageCode);
                    oCmd.Parameters.AddWithValue("@StageShortName", StageShortName);
                    await myConnection.OpenAsync();
                    using (SqlDataAdapter oDataAdapter = new SqlDataAdapter(oCmd))
                    {
                        oDataAdapter.Fill(oDataSet);
                    }
                }

                if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                {
                    model.ProcessMasterList = (from DataRow dr in oDataSet.Tables[0].Rows
                                               select new ProcessMasterModel
                                               {
                                                   Process_Id = Convert.ToInt32(dr["ProcessId"]),
                                                   StageCode = dr["StageCode"].ToString(),
                                                   StageDescription = dr["StageDescription"].ToString(),
                                                   StageShortName = dr["StageShortName"].ToString(),
                                                   MaterialReqForRework = Convert.ToInt32(dr["MaterialReqForRework"]),
                                                   QCRequired = Convert.ToInt32(dr["QCRequired"]),
                                                   CC = dr["CC"].ToString(),
                                                   UID = Convert.ToInt32(dr["Uid"].ToString()),
                                                   MaxPendQtyReq = Convert.ToInt32(dr["MaxPendQtyReq"].ToString()),
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

        public async Task<IList<TextValue>> GetDropDownList(string Flag)
        {
            List<TextValue>? List = new List<TextValue>();
            DataTable? oDataTable = new DataTable();

            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    SqlCommand oCmd = new SqlCommand("SP_GetDropDownList", myConnection);
                    oCmd.CommandType = CommandType.StoredProcedure;
                    oCmd.Parameters.AddWithValue("@Flag", Flag);
                    await myConnection.OpenAsync();
                    using (SqlDataAdapter oDataAdapter = new SqlDataAdapter(oCmd))
                    {
                        oDataAdapter.Fill(oDataTable);
                    }


                }
            }
            catch (Exception ex)
            {
                List.Add(new TextValue
                {
                    Text = ex.Message,
                    Value = ex.Source
                });
            }
            finally
            {
                oDataTable.Dispose();
            }
            return List;
        }

        public async Task<ResponseResult> CheckBeforeUpdate(int Type)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                // if(Mode=="U")
                SqlParams.Add(new SqlParameter("@Flag", "CheckBeforeUpdate"));
                SqlParams.Add(new SqlParameter("@Type", Type));

                _ResponseResult = await _DataLogicDAL.ExecuteDataSet("SPProcessMaster", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }

        public async Task<ResponseResult> SaveProcessMaster(ProcessMasterModel model)
        {
            dynamic _ResponseResult = null;

            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    SqlCommand oCmd = new SqlCommand("SPProcessMaster", myConnection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    
                    oCmd.Parameters.AddWithValue("@Flag", model.Mode);
                    oCmd.Parameters.AddWithValue("@ProcessId", model.Process_Id);
                    oCmd.Parameters.AddWithValue("@StageCode", model.StageCode);
                    oCmd.Parameters.AddWithValue("@StageDescription", model.StageDescription);
                    oCmd.Parameters.AddWithValue("@StageShortName", model.StageShortName);
                    oCmd.Parameters.AddWithValue("@MaterialReqForRework", model.MaterialReqForRework);
                    oCmd.Parameters.AddWithValue("@QCRequired", model.QCRequired);
                    oCmd.Parameters.AddWithValue("@MaxPendQtyReq", model.MaxPendQtyReq);
                    oCmd.Parameters.AddWithValue("@CC", model.CC);
                    oCmd.Parameters.AddWithValue("@UID", model.UID);
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
    }
}
