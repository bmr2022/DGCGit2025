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
    public class UnitMasterDAL
    {
        private readonly IDataLogic _IDataLogic;
        private readonly string DBConnectionString = string.Empty;
        private IDataReader? Reader;
        private readonly ConnectionStringService _connectionStringService;
        public UnitMasterDAL(IConfiguration configuration, IDataLogic iDataLogic, ConnectionStringService connectionStringService)
        {
            //configuration = config;
            //DBConnectionString = configuration.GetConnectionString("eTactDB");
            _IDataLogic = iDataLogic;
            _connectionStringService = connectionStringService;
            DBConnectionString = _connectionStringService.GetConnectionString();
        }
        public async Task<ResponseResult> GetFormRights(int userId)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "GetRights"));
                SqlParams.Add(new SqlParameter("@EmpId", userId));
                SqlParams.Add(new SqlParameter("@MainMenu", "Unit Master"));

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

        public async Task<ResponseResult> SaveUnitMaster(UnitMasterModel model)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                if (model.Mode == "U" || model.Mode == "V")
                {
                    SqlParams.Add(new SqlParameter("@Flag", "Update"));
                    //SqlParams.Add(new SqlParameter("@Unit_Name", model.Unit_Name != "" ? model.Unit_Name : (object)DBNull.Value));
                    SqlParams.Add(new SqlParameter("@Unit_Name", string.IsNullOrEmpty(model.Unit_Name) ? DBNull.Value : model.Unit_Name));
                    SqlParams.Add(new SqlParameter("@Round_Off", string.IsNullOrEmpty(model.Round_Off) ? DBNull.Value : model.Round_Off));
                    SqlParams.Add(new SqlParameter("@CC", string.IsNullOrEmpty(model.CC) ? DBNull.Value : model.CC));
                    SqlParams.Add(new SqlParameter("@UnitDetail", string.IsNullOrEmpty(model.UnitDetail) ? DBNull.Value : model.UnitDetail));
                }
                else
                {
                    SqlParams.Add(new SqlParameter("@Flag", "Insert"));
                    //SqlParams.Add(new SqlParameter("@Unit_Name", model.Unit_Name != "" ? model.Unit_Name : (object)DBNull.Value));
                    SqlParams.Add(new SqlParameter("@Unit_Name", string.IsNullOrEmpty(model.Unit_Name) ? DBNull.Value : model.Unit_Name));
                    SqlParams.Add(new SqlParameter("@Round_Off", string.IsNullOrEmpty(model.Round_Off) ? DBNull.Value : model.Round_Off));
                    SqlParams.Add(new SqlParameter("@CC", string.IsNullOrEmpty(model.CC) ? DBNull.Value : model.CC));
                    SqlParams.Add(new SqlParameter("@UnitDetail", string.IsNullOrEmpty(model.UnitDetail) ? DBNull.Value : model.UnitDetail));
                }


                // Call the stored procedure with the provided parameters
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_UnitMaster", SqlParams);
            }
            catch (Exception ex)
            {
                // Handle exceptions and prepare the error response
                _ResponseResult.StatusCode = HttpStatusCode.InternalServerError;
                _ResponseResult.StatusText = "Error";
                _ResponseResult.Result = new { ex.Message, ex.StackTrace };
            }

            return _ResponseResult;
        }

        public async Task<ResponseResult> GetDashBoardData()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "Dashboard"));
                _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_UnitMaster", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }
        public async Task<UnitMasterModel> GetDashBoardDetailData()
        {
            DataSet? oDataSet = new DataSet();
            var model = new UnitMasterModel();
            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    SqlCommand oCmd = new SqlCommand("SP_UnitMaster", myConnection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    oCmd.Parameters.AddWithValue("@Flag", "Dashboard");
                    await myConnection.OpenAsync();
                    using (SqlDataAdapter oDataAdapter = new SqlDataAdapter(oCmd))
                    {
                        oDataAdapter.Fill(oDataSet);
                    }
                }
                if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                {
                    model.UnitMasterDashBoardGrid = (from DataRow dr in oDataSet.Tables[0].Rows
                                                      select new UnitMasterModel
                                                      {

                                                          Unit_Name = dr["Unit_Name"].ToString(),
                                                          Round_Off = dr["Round_Off"].ToString(),
                                                          CC = dr["CC"].ToString(),
                                                          UnitDetail = dr["UnitDetail"].ToString(),

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

        public async Task<UnitMasterModel> GetViewByID(string Unit_Name)
        {
            var model = new UnitMasterModel();
            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@flag", "GetByID"));
                SqlParams.Add(new SqlParameter("@Unit_Name", Unit_Name));
                var _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_UnitMaster", SqlParams);

                if (_ResponseResult.Result != null && _ResponseResult.StatusCode == HttpStatusCode.OK && _ResponseResult.StatusText == "Success")
                {
                    //PrepareView(_ResponseResult.Result, ref model);
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
        public async Task<ResponseResult> DeleteByID(string Unit_Name)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "DeleteByID"));
                SqlParams.Add(new SqlParameter("@Unit_Name", Unit_Name));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_UnitMaster", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
                // Optional: log the error using a logger
                Console.WriteLine($"Error: {Error.Message}");
                Console.WriteLine($"Source: {Error.Source}");
            }

            return _ResponseResult;
        }
    }
}
