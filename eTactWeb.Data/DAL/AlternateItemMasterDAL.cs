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
    public class AlternateItemMasterDAL
    {
        private readonly IDataLogic _IDataLogic;
        private readonly string DBConnectionString = string.Empty;
        private IDataReader? Reader;
        private readonly ConnectionStringService _connectionStringService;
        public AlternateItemMasterDAL(IConfiguration configuration, IDataLogic iDataLogic, ConnectionStringService connectionStringService)
        {
            //DBConnectionString = configuration.GetConnectionString("eTactDB");
            _connectionStringService = connectionStringService;
            DBConnectionString = _connectionStringService.GetConnectionString();
            _IDataLogic = iDataLogic;
        }
        public async Task<ResponseResult> GetMainItem()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FillMainItem"));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SpAlternateItemMaster", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        } 
        public async Task<ResponseResult> GetMainPartCode()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FillMainPartCode"));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SpAlternateItemMaster", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
            public async Task<ResponseResult> GetAltPartCode(int MainItemcode)
            {
                var _ResponseResult = new ResponseResult();
                try
                {
                    var SqlParams = new List<dynamic>();
                    SqlParams.Add(new SqlParameter("@Flag", "FillAltPartCode"));
                    SqlParams.Add(new SqlParameter("@MainItemcode", MainItemcode));
                    _ResponseResult = await _IDataLogic.ExecuteDataTable("SpAlternateItemMaster", SqlParams);
                }
                catch (Exception ex)
                {
                    dynamic Error = new ExpandoObject();
                    Error.Message = ex.Message;
                    Error.Source = ex.Source;
                }

                return _ResponseResult;
            }
        public async Task<ResponseResult> GetAltItemName(int MainItemcode)
            {
                var _ResponseResult = new ResponseResult();
                try
                {
                    var SqlParams = new List<dynamic>();
                    SqlParams.Add(new SqlParameter("@Flag", "FillAltItem"));
                    SqlParams.Add(new SqlParameter("@MainItemcode", MainItemcode));
                    _ResponseResult = await _IDataLogic.ExecuteDataTable("SpAlternateItemMaster", SqlParams);
                }
                catch (Exception ex)
                {
                    dynamic Error = new ExpandoObject();
                    Error.Message = ex.Message;
                    Error.Source = ex.Source;
                }

                return _ResponseResult;
        }
        private string GetMainItemCodeFromGIGrid(DataTable GIGrid)
        {
            if (GIGrid == null || GIGrid.Rows.Count == 0)
                return null; // Return null if GIGrid is empty or null

            // Assuming "MainItemCode" is the column name
            return GIGrid.Rows[0]["MainItemCode"]?.ToString(); // Get the value from the first row
        }
        private string GetAltItemCodeFromGIGrid(DataTable GIGrid)
        {
            if (GIGrid == null || GIGrid.Rows.Count == 0)
                return null; // Return null if GIGrid is empty or null

            // Assuming "MainItemCode" is the column name
            return GIGrid.Rows[0]["AlternateItemCode"]?.ToString(); // Get the value from the first row
        }

        public async Task<ResponseResult> SaveAlternateItemMaster(AlternateItemMasterModel model, DataTable GIGrid)
        {
            var _ResponseResult = new ResponseResult();

            try
            {
                var sqlParams = new List<dynamic>();
                if (model.Mode == "U" || model.Mode == "V")
                {
                    sqlParams.Add(new SqlParameter("@Flag", "UPDATE"));
                    sqlParams.Add(new SqlParameter("@EffectiveDate", model.EffectiveDate));
                    sqlParams.Add(new SqlParameter("@ActualEntryByEmp", model.ActualEntryByEmp));
                    sqlParams.Add(new SqlParameter("@ActualEntryDate", model.ActualEntryDate));
                    sqlParams.Add(new SqlParameter("@MachineName", model.MachineName ?? Environment.MachineName));
                    sqlParams.Add(new SqlParameter("@MainItemcode", GetMainItemCodeFromGIGrid(GIGrid))); // Ensure model.MainPartCode is correctly assigned to MainItemcode
                    sqlParams.Add(new SqlParameter("@AlternateItemCode", model.AlternateItemCode)); // Ensure AlternateItemCode is assigned correctly
                    sqlParams.Add(new SqlParameter("@UpdatedByEmp", model.UpdatedByEmp));
                    sqlParams.Add(new SqlParameter("@UpdationDate", DateTime.Now));
                    sqlParams.Add(new SqlParameter("@ProcGrid", GIGrid));
                }
                else
                {
                    sqlParams.Add(new SqlParameter("@Flag", "INSERT"));
                    sqlParams.Add(new SqlParameter("@EffectiveDate", model.EffectiveDate));
                    sqlParams.Add(new SqlParameter("@ActualEntryByEmp", model.ActualEntryByEmp));
                    sqlParams.Add(new SqlParameter("@ActualEntryDate", model.ActualEntryDate));
                    sqlParams.Add(new SqlParameter("@MachineName", model.MachineName ?? Environment.MachineName));
                    sqlParams.Add(new SqlParameter("@MainItemcode", model.MainPartCode));
                   // sqlParams.Add(new SqlParameter("@MainItemcode", GetMainItemCodeFromGIGrid(GIGrid)));
                    //sqlParams.Add(new SqlParameter("@AlternateItemCode", GetAltItemCodeFromGIGrid(GIGrid)));
                    sqlParams.Add(new SqlParameter("@AlternateItemCode", model.AltPartCode));
                    sqlParams.Add(new SqlParameter("@ProcGrid", GIGrid));
                }
               

                //// Add main item code and other required parameters
                //new SqlParameter("@MainItemcode", model.MainPartCode),
                //new SqlParameter("@AlternateItemCode", model.AltPartCode),
                //new SqlParameter("@effectivedate", model.EffectiveDate),
                //new SqlParameter("@ActualEntryByEmp", model.ActualEntryByEmp),
                //new SqlParameter("@ActualEntryDate", model.ActualEntryDate),
                //new SqlParameter("@MachineName", model.MachineName ?? Environment.MachineName),
                //new SqlParameter("@ProcGrid", GIGrid)

            

                // Execute the stored procedure with parameters
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SpAlternateItemMaster", sqlParams);
                
                // Set success response if data is inserted successfully
                //_ResponseResult.StatusCode = HttpStatusCode.OK;
                //_ResponseResult.StatusText = "Success";
            }
            catch (Exception ex)
            {
                // Set error response
                _ResponseResult.StatusCode = HttpStatusCode.InternalServerError;
                _ResponseResult.StatusText = "Error";
                _ResponseResult.Result = new { ex.Message, ex.StackTrace };
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> GetDashboardData()
        {
            var responseResult = new ResponseResult();
            try
            {
                var sqlParams = new List<dynamic>
        {
            new SqlParameter("@Flag", "DASHBOARD")
        };

                responseResult = await _IDataLogic.ExecuteDataSet("SpAlternateItemMaster", sqlParams).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                dynamic error = new ExpandoObject();
                error.Message = ex.Message;
                error.Source = ex.Source;
            }
            return responseResult;
        }
        public async Task<AlternateItemMasterDashBoardModel> GetDashboardDetailData()
        {
            DataSet? oDataSet = new DataSet();
            var model = new AlternateItemMasterDashBoardModel();
            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    SqlCommand oCmd = new SqlCommand("SpAlternateItemMaster", myConnection)
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
                    model.AlternateItemMasterDashBoardGrid = (from DataRow dr in oDataSet.Tables[0].Rows
                                                             select new AlternateItemMasterDashBoardModel
                                                             {
                                                                 MainItemCode = Convert.ToInt32(dr["MainItemCode"]),
                                                                 MainItemName = dr["MainItemName"].ToString(),
                                                                 AlternateItemCode = Convert.ToInt32(dr["AlternateItemCode"]),
                                                                 AltItemName = dr["AlternateItemName"].ToString(),
                                                                 EffectiveDate = dr["EffectiveDate"].ToString(),
                                                                 AlternatePartCode = dr["AlternatePartCode"].ToString(),
                                                                 MainPartCode = dr["MainPartCode"].ToString(),
                                                                 MatchingFactorPercentage = Convert.ToInt32(dr["MatchingFactorPercentage"]),
                                                                 ActualEntryBy = dr["ActualEntryBy"].ToString(),
                                                                 ActualEntryDate = dr["ActualEntryDate"].ToString(),
                                                                 LastUpdatedBy = dr["LastUpdatedBy"].ToString(),
                                                                 UpdationDate = dr["UpdationDate"].ToString(),
                                                                 MachineName = dr["MachineName"].ToString()
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
        public async Task<AlternateItemMasterModel> GetViewByID(int MainItemcode, int AlternateItemCode)
        {
            var model = new AlternateItemMasterModel();
            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@flag", "ViewByID"));
                SqlParams.Add(new SqlParameter("@MainItemcode", MainItemcode));
                SqlParams.Add(new SqlParameter("@AlternateItemCode", AlternateItemCode));
                var _ResponseResult = await _IDataLogic.ExecuteDataSet("SpAlternateItemMaster", SqlParams);

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
        public async Task<ResponseResult> DeleteByID(int MainItemCode, int AlternateItemCode, string MachineName, int EntryByempId)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "DELETE"));
                SqlParams.Add(new SqlParameter("@MainItemCode", MainItemCode));
                SqlParams.Add(new SqlParameter("@AlternateItemCode", AlternateItemCode));
                SqlParams.Add(new SqlParameter("@MachineName", MachineName));
                SqlParams.Add(new SqlParameter("@EntryByempId", EntryByempId));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SpAlternateItemMaster", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        private static AlternateItemMasterModel PrepareView(DataSet DS, ref AlternateItemMasterModel? model)
        {
            try
            {
                var ItemList = new List<AlternateItemMasterGridModel>();
                DS.Tables[0].TableName = "AlternateItemMaster";
                int cnt = 0;

                model.AlternateItemCode = Convert.ToInt32(DS.Tables[0].Rows[0]["AlternateItemCode"].ToString());
                model.MainItemCode = Convert.ToInt32(DS.Tables[0].Rows[0]["MainItemCode"].ToString());
                model.effectivedate = DS.Tables[0].Rows[0]["effectivedate"].ToString();
                model.MatchingFactorPercentage = Convert.ToInt32(DS.Tables[0].Rows[0]["MatchingFactorPercentage"].ToString());
                model.ActualEntryByEmp = Convert.ToInt32(DS.Tables[0].Rows[0]["ActualEntryByEmp"].ToString());
                model.ActualEntryDate = DS.Tables[0].Rows[0]["ActualEntryDate"].ToString();
                if (DS.Tables.Count != 0 && DS.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow row in DS.Tables[0].Rows)
                    {
                        ItemList.Add(new AlternateItemMasterGridModel
                        {
                            FGItemCode = Convert.ToInt32(DS.Tables[0].Rows[0]["MainItemCode"].ToString()),
                            AlternateItemCode = Convert.ToInt32(DS.Tables[0].Rows[0]["AlternateItemCode"].ToString()),
                            MainItemCode = Convert.ToInt32(DS.Tables[0].Rows[0]["MainItemCode"].ToString()),
                            AltPartCode = DS.Tables[0].Rows[0]["AlternatePartCode"].ToString(),
                            AltItemName = DS.Tables[0].Rows[0]["AlternateItemName"].ToString(),
                            MainPartCode = DS.Tables[0].Rows[0]["MainPartCode"].ToString(),
                            MainItemName = DS.Tables[0].Rows[0]["MainItemName"].ToString(),
                        });
                    }
                    model.AlternateItemMasterGrid = ItemList;
                }
                return model;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
