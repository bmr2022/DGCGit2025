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
    public class DepartmentMasterDAL
    {
        private readonly IDataLogic _IDataLogic;
        private readonly string DBConnectionString = string.Empty;
        private IDataReader? Reader;

        public DepartmentMasterDAL(IConfiguration configuration, IDataLogic iDataLogic)
        {
            //configuration = config;
            DBConnectionString = configuration.GetConnectionString("eTactDB");
            _IDataLogic = iDataLogic;
        }

        public async Task<ResponseResult> FillDeptType()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@flag", "DepartmentType"));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("HRSPDepartmentMaster", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }

        public async Task<ResponseResult> FillDeptID()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "NewEntryId"));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("HRSPDepartmentMaster", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }

        public async Task<ResponseResult> SaveDeptMaster(DepartmentMasterModel model)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                if (model.Mode == "U" || model.Mode == "V")
                {
                    SqlParams.Add(new SqlParameter("@Flag", "Update"));
                    SqlParams.Add(new SqlParameter("@DeptId", model.DeptId > 0 ? model.DeptId : (object)DBNull.Value));
                    SqlParams.Add(new SqlParameter("@DeptName", string.IsNullOrEmpty(model.DeptName) ? DBNull.Value : model.DeptName));
                    SqlParams.Add(new SqlParameter("@EntryDate", string.IsNullOrEmpty(model.Entry_Date) ? DBNull.Value : model.Entry_Date));
                    SqlParams.Add(new SqlParameter("@CC", string.IsNullOrEmpty(model.CC) ? DBNull.Value : model.CC));
                    SqlParams.Add(new SqlParameter("@DeptType", string.IsNullOrEmpty(model.DeptType) ? DBNull.Value : model.DeptType));
                    SqlParams.Add(new SqlParameter("@departmentcode", string.IsNullOrEmpty(model.departmentcode) ? DBNull.Value : model.departmentcode));
                }
                else
                {
                    SqlParams.Add(new SqlParameter("@Flag", "Insert"));
                    SqlParams.Add(new SqlParameter("@DeptId", model.DeptId > 0 ? model.DeptId : (object)DBNull.Value));
                    SqlParams.Add(new SqlParameter("@DeptName", string.IsNullOrEmpty(model.DeptName) ? DBNull.Value : model.DeptName));
                    SqlParams.Add(new SqlParameter("@EntryDate", string.IsNullOrEmpty(model.Entry_Date) ? DBNull.Value : model.Entry_Date));
                    SqlParams.Add(new SqlParameter("@CC", string.IsNullOrEmpty(model.CC) ? DBNull.Value : model.CC));
                    SqlParams.Add(new SqlParameter("@DeptType", string.IsNullOrEmpty(model.DeptType) ? DBNull.Value : model.DeptType));
                    SqlParams.Add(new SqlParameter("@departmentcode", string.IsNullOrEmpty(model.departmentcode) ? DBNull.Value : model.departmentcode));
                }


                // Call the stored procedure with the provided parameters
                _ResponseResult = await _IDataLogic.ExecuteDataTable("HRSPDepartmentMaster", SqlParams);
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
                _ResponseResult = await _IDataLogic.ExecuteDataSet("HRSPDepartmentMaster", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }
        public async Task<DepartmentMasterModel> GetDashBoardDetailData()
        {
            DataSet? oDataSet = new DataSet();
            var model = new DepartmentMasterModel();
            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    SqlCommand oCmd = new SqlCommand("HRSPDepartmentMaster", myConnection)
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
                    model.DepartmentMasterDashBoardGrid = (from DataRow dr in oDataSet.Tables[0].Rows
                                                      select new DepartmentMasterModel
                                                      {
                                                          DeptId = Convert.ToInt32(dr["DeptId"]),
                                                          departmentcode = dr["departmentcode"].ToString(),
                                                          DeptName = dr["DeptName"].ToString(),
                                                          DeptType = dr["DeptType"].ToString(),
                                                         

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

        public async Task<DepartmentMasterModel> GetViewByID(int ID)
        {
            var model = new DepartmentMasterModel();
            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@flag", "ViewID"));
                SqlParams.Add(new SqlParameter("@departmentCode", ID));
                var _ResponseResult = await _IDataLogic.ExecuteDataSet("HRSPDepartmentMaster", SqlParams);

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
                SqlParams.Add(new SqlParameter("@DeptId", ID));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("HRSPDepartmentMaster", SqlParams);
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
