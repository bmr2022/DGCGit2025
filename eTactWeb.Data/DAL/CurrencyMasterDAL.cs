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
    public class CurrencyMasterDAL
    {
        private readonly IDataLogic _IDataLogic;
        private readonly string DBConnectionString = string.Empty;
        private IDataReader? Reader;
        private readonly ConnectionStringService _connectionStringService;
        public CurrencyMasterDAL(IConfiguration configuration, IDataLogic iDataLogic, ConnectionStringService connectionStringService)
        {
            //configuration = config;
            //DBConnectionString = configuration.GetConnectionString("eTactDB");
            _IDataLogic = iDataLogic;
            _connectionStringService = connectionStringService;
            DBConnectionString = _connectionStringService.GetConnectionString();
        }
        public async Task<ResponseResult> SaveCurrencyMaster(CurrencyMasterModel model)
        {
            var response = new ResponseResult();

            try
            {
                using (var conn = new SqlConnection(DBConnectionString)) // Use your actual connection string
                using (var cmd = new SqlCommand("SPCurrencyMaster", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    if (model.Mode == "U" || model.Mode == "V")
                    {
                        cmd.Parameters.AddWithValue("@Flag", "Update");
                        cmd.Parameters.AddWithValue("@currencyId", model.CurrencyId > 0 ? model.CurrencyId : (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Currency", string.IsNullOrEmpty(model.Currency) ? DBNull.Value : model.Currency);
                        cmd.Parameters.AddWithValue("@IsDefault", string.IsNullOrEmpty(model.IsDefault) ? DBNull.Value : model.IsDefault);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@Flag", "Insert");
                        cmd.Parameters.AddWithValue("@currencyId", model.CurrencyId > 0 ? model.CurrencyId : (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Currency", string.IsNullOrEmpty(model.Currency) ? DBNull.Value : model.Currency);
                        cmd.Parameters.AddWithValue("@StageShortName", string.IsNullOrEmpty(model.IsDefault) ? DBNull.Value : model.IsDefault);
                    }

                    await conn.OpenAsync();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            response.StatusText = reader["StatusText"].ToString();
                            response.StatusCode = (HttpStatusCode)Convert.ToInt32(reader["StatusCode"]);
                            response.Result = reader["Result"];
                        }
                        else
                        {
                            response.StatusText = "Error";
                            response.StatusCode = HttpStatusCode.InternalServerError;
                            response.Result = "No response from stored procedure.";
                        }
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                if (sqlEx.Message.Contains("is already set as default"))
                {
                    response.StatusCode = HttpStatusCode.BadRequest;
                    response.StatusText = "Error";
                    response.Result = new { Message = sqlEx.Message };
                }
                else
                {
                    response.StatusCode = HttpStatusCode.InternalServerError;
                    response.StatusText = "Error";
                    response.Result = new { Message = sqlEx.Message };
                }
            }
            catch (Exception ex)
            {
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.StatusText = "Error";
                response.Result = new { Message = ex.Message };
            }

            return response;
        }


        //     public async Task<ResponseResult> SaveCurrencyMaster(CurrencyMasterModel model)
        //     {
        //         var _ResponseResult = new ResponseResult();
        //         try
        //         {
        //             var SqlParams = new List<dynamic>();
        //             if (model.Mode == "U" || model.Mode == "V")
        //             {
        //                 SqlParams.Add(new SqlParameter("@Flag", "Update"));
        //                 SqlParams.Add(new SqlParameter("@currencyId", model.CurrencyId > 0 ? model.CurrencyId : (object)DBNull.Value));
        //                 SqlParams.Add(new SqlParameter("@Currency", string.IsNullOrEmpty(model.Currency) ? DBNull.Value : model.Currency));
        //                 SqlParams.Add(new SqlParameter("@IsDefault", string.IsNullOrEmpty(model.IsDefault) ? DBNull.Value : model.IsDefault));

        //             }
        //             else
        //             {
        //                 SqlParams.Add(new SqlParameter("@Flag", "Insert"));
        //                 SqlParams.Add(new SqlParameter("@currencyId", model.CurrencyId > 0 ? model.CurrencyId : (object)DBNull.Value));
        //                 SqlParams.Add(new SqlParameter("@Currency", string.IsNullOrEmpty(model.Currency) ? DBNull.Value : model.Currency));
        //                 SqlParams.Add(new SqlParameter("@StageShortName", string.IsNullOrEmpty(model.IsDefault) ? DBNull.Value : model.IsDefault));

        //             }


        //             // Call the stored procedure with the provided parameters
        //             _ResponseResult = await _IDataLogic.ExecuteDataTable("SPCurrencyMaster", SqlParams);
        //         }
        ////catch (Exception ex)
        ////{
        ////    // Handle exceptions and prepare the error response
        ////    _ResponseResult.StatusCode = HttpStatusCode.InternalServerError;
        ////    _ResponseResult.StatusText = "Error";
        ////    _ResponseResult.Result = new { ex.Message, ex.StackTrace };
        ////}
        //catch (SqlException sqlEx)
        //{
        //	// Check for your custom error message text pattern
        //	if (sqlEx.Message.Contains("is already set as default"))
        //	{
        //		_ResponseResult.StatusCode = HttpStatusCode.BadRequest;
        //		_ResponseResult.StatusText = "Error";
        //		_ResponseResult.Result = new { Message = sqlEx.Message };
        //	}
        //	else
        //	{
        //		_ResponseResult.StatusCode = HttpStatusCode.InternalServerError;
        //		_ResponseResult.StatusText = "Error";
        //		_ResponseResult.Result = new { Message = sqlEx.Message };
        //	}
        //}
        //catch (Exception ex)
        //{
        //	_ResponseResult.StatusCode = HttpStatusCode.InternalServerError;
        //	_ResponseResult.StatusText = "Error";
        //	_ResponseResult.Result = new { Message = ex.Message };
        //}
        //return _ResponseResult;
        //     }


        public async Task<ResponseResult> GetDashBoardData()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "DASHBOARD"));
                _ResponseResult = await _IDataLogic.ExecuteDataSet("SPCurrencyMaster", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }

        public async Task<CurrencyMasterModel> GetDashBoardDetailData()
        {
            DataSet? oDataSet = new DataSet();
            var model = new CurrencyMasterModel();
            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    SqlCommand oCmd = new SqlCommand("SPCurrencyMaster", myConnection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    oCmd.Parameters.AddWithValue("@Flag", "DASHBOARD");
                    await myConnection.OpenAsync();
                    using (SqlDataAdapter oDataAdapter = new SqlDataAdapter(oCmd))
                    {
                        oDataAdapter.Fill(oDataSet);
                    }
                }
                if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                {
                    model.CurrencyMasterDashBoardGrid = (from DataRow dr in oDataSet.Tables[0].Rows
                                                      select new CurrencyMasterModel
                                                      {
                                                          CurrencyId = Convert.ToInt32(dr["CurrencyId"]),
                                                          Currency = dr["Currency"].ToString(),
                                                          IsDefault = dr["IsDefault"].ToString(),
                                                          
                                                          //EntryDate = dr["Entry_Date"].ToString(),

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

        public async Task<CurrencyMasterModel> GetViewByID(int ID)
        {
            var model = new CurrencyMasterModel();
            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@flag", "ViewById"));
                SqlParams.Add(new SqlParameter("@currencyId", ID));
                var _ResponseResult = await _IDataLogic.ExecuteDataSet("SPCurrencyMaster", SqlParams);

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


        public async Task<ResponseResult> DeleteByID(int ID)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "DeleteByID"));
                SqlParams.Add(new SqlParameter("@CurrencyId", ID));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPCurrencyMaster", SqlParams);
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
