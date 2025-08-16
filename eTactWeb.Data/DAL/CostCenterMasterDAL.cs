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
    public class CostCenterMasterDAL
    {
        private readonly IDataLogic _IDataLogic;
        private readonly string DBConnectionString = string.Empty;
        private IDataReader? Reader;
        private readonly ConnectionStringService _connectionStringService;
        //private readonly IConfiguration configuration;

        public CostCenterMasterDAL(IConfiguration configuration, IDataLogic iDataLogic, ConnectionStringService connectionStringService)
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
                SqlParams.Add(new SqlParameter("@MainMenu", "CostCenter Master"));

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

        public async Task<ResponseResult> FillCostCenterID()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "NewRef"));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPCostCenterMaster", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }
        public async Task<ResponseResult> FillCostCenterGroupName()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "CostcenetrGroupName"));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPCostCenterMaster", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }
        public async Task<ResponseResult> FillDeptName()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "Departmentmaster"));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPCostCenterMaster", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        
        }public async Task<ResponseResult> FillUnderGroupName()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "UnderGroupName"));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPCostCenterMaster", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }
        public async Task<ResponseResult> SaveCostCenterMaster(CostCenterMasterModel model)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                if (model.Mode == "U" || model.Mode == "V")
                {
                    SqlParams.Add(new SqlParameter("@Flag", "Update"));
                    SqlParams.Add(new SqlParameter("@CostcenterId", model.CostcenterId > 0 ? model.CostcenterId : (object)DBNull.Value));
                    SqlParams.Add(new SqlParameter("@EntryDate", string.IsNullOrEmpty(model.EntryDate) ? DBNull.Value : model.EntryDate));
                    SqlParams.Add(new SqlParameter("@CostCenterName", string.IsNullOrEmpty(model.CostCenterName) ? DBNull.Value : model.CostCenterName));
                    SqlParams.Add(new SqlParameter("@ShortName", string.IsNullOrEmpty(model.ShortName) ? DBNull.Value : model.ShortName));
                    SqlParams.Add(new SqlParameter("@DeptId", model.DepartmentID > 0 ? model.DepartmentID : (object)DBNull.Value));
                    SqlParams.Add(new SqlParameter("@UnderGroupId", model.UnderGroupId > 0 ? model.UnderGroupId : (object)DBNull.Value));
                    SqlParams.Add(new SqlParameter("@Remarks", string.IsNullOrEmpty(model.Remarks) ? DBNull.Value : model.Remarks));
                    SqlParams.Add(new SqlParameter("@CC", string.IsNullOrEmpty(model.CC) ? DBNull.Value : model.CC));
                    SqlParams.Add(new SqlParameter("@CostCenterCode", string.IsNullOrEmpty(model.CostCenterCode) ? DBNull.Value : model.CostCenterCode));
                    SqlParams.Add(new SqlParameter("@CostcentergroupID", model.CostcentergroupID > 0 ? model.CostcentergroupID : (object)DBNull.Value));
                    SqlParams.Add(new SqlParameter("@CostcenetrGroupName", string.IsNullOrEmpty(model.CostcenetrGroupName) ? DBNull.Value : model.CostcenetrGroupName));
                    //    SqlParams.Add(new SqlParameter("@StoreId", model.StoreId > 0 ? model.StoreId : (object)DBNull.Value));
                    //    SqlParams.Add(new SqlParameter("@Store_Name", string.IsNullOrEmpty(model.Store_Name) ? DBNull.Value : model.Store_Name));
                    //    SqlParams.Add(new SqlParameter("@Entry_Date", string.IsNullOrEmpty(model.EntryDate) ? DBNull.Value : model.EntryDate));
                    //    SqlParams.Add(new SqlParameter("@CC", string.IsNullOrEmpty(model.CC) ? DBNull.Value : model.CC));
                    //    SqlParams.Add(new SqlParameter("@StoreType", string.IsNullOrEmpty(model.StoreType) ? DBNull.Value : model.StoreType));
                }
                else
                {
                    SqlParams.Add(new SqlParameter("@Flag", "Insert"));
                    SqlParams.Add(new SqlParameter("@CostcenterId", model.CostcenterId > 0 ? model.CostcenterId : (object)DBNull.Value));
                    SqlParams.Add(new SqlParameter("@EntryDate", string.IsNullOrEmpty(model.EntryDate) ? DBNull.Value : model.EntryDate));
                    SqlParams.Add(new SqlParameter("@CostCenterName", string.IsNullOrEmpty(model.CostCenterName) ? DBNull.Value : model.CostCenterName));
                    SqlParams.Add(new SqlParameter("@ShortName", string.IsNullOrEmpty(model.ShortName) ? DBNull.Value : model.ShortName));
                    SqlParams.Add(new SqlParameter("@DeptId", model.DepartmentID >0?  model.DepartmentID : (object)DBNull.Value));
                    SqlParams.Add(new SqlParameter("@UnderGroupId", model.UnderGroupId > 0?  model.UnderGroupId : (object)DBNull.Value));
                    SqlParams.Add(new SqlParameter("@Remarks", string.IsNullOrEmpty(model.Remarks) ? DBNull.Value : model.Remarks));
                    SqlParams.Add(new SqlParameter("@CC", string.IsNullOrEmpty(model.CC) ? DBNull.Value : model.CC));
                    SqlParams.Add(new SqlParameter("@CostCenterCode", string.IsNullOrEmpty(model.CostCenterCode) ? DBNull.Value : model.CostCenterCode));
                    SqlParams.Add(new SqlParameter("@CostcentergroupID", model.CostcentergroupID > 0?  model.CostcentergroupID : (object)DBNull.Value));
                    SqlParams.Add(new SqlParameter("@CostcenetrGroupName", string.IsNullOrEmpty(model.CostcenetrGroupName) ? DBNull.Value : model.CostcenetrGroupName));
                }


                // Call the stored procedure with the provided parameters
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPCostCenterMaster", SqlParams);
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
                SqlParams.Add(new SqlParameter("@Flag", "DashBaord"));
                _ResponseResult = await _IDataLogic.ExecuteDataSet("SPCostCenterMaster", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }
        public async Task<CostCenterMasterModel> GetDashBoardDetailData()
        {
            DataSet? oDataSet = new DataSet();
            var model = new CostCenterMasterModel();
            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    SqlCommand oCmd = new SqlCommand("SPCostCenterMaster", myConnection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    oCmd.Parameters.AddWithValue("@Flag", "DashBaord");
                    await myConnection.OpenAsync();
                    using (SqlDataAdapter oDataAdapter = new SqlDataAdapter(oCmd))
                    {
                        oDataAdapter.Fill(oDataSet);
                    }
                }
                if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                {
                    model.CostCenterMasterGrid = (from DataRow dr in oDataSet.Tables[0].Rows
                                                      select new CostCenterMasterModel
                                                      {
                                                          CostcenterId = Convert.ToInt32(dr["CostcenterId"]),
                                                          EntryDate = dr["EntryDate"].ToString(),
                                                          CostCenterName = dr["CostCenterName"].ToString(),
                                                          ShortName = dr["ShortName"].ToString(),
                                                          DepartmentName = dr["DeptName"].ToString(),
                                                          DepartmentID = Convert.ToInt32(dr["DeptId"].ToString()),
                                                          UnderGroupId = Convert.ToInt32(dr["UnderGroupId"].ToString()),
                                                          UnderGroupName = dr["GroupName"].ToString(),
                                                          Remarks = dr["Remarks"].ToString(),
                                                          CC = dr["CC"].ToString(),
                                                          CostCenterCode = dr["CostCenterCode"].ToString(),
                                                          CostcentergroupID = Convert.ToInt32(dr["CostcentergroupID"]),
                                                          CostcenetrGroupName = dr["CostcenetrGroupName"].ToString()

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
        public async Task<CostCenterMasterModel> GetViewByID(int ID)
        {
            var model = new CostCenterMasterModel();
            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@flag", "ViewById"));
                SqlParams.Add(new SqlParameter("@CostcenterId", ID));
                var _ResponseResult = await _IDataLogic.ExecuteDataSet("SPCostCenterMaster", SqlParams);

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
                SqlParams.Add(new SqlParameter("@Flag", "Delete"));
                SqlParams.Add(new SqlParameter("@CostcenterId", ID));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPCostCenterMaster", SqlParams);
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
        public async Task<ResponseResult> ChkForDuplicate(string CostCenterName)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "ChkForDuplicate"));
                SqlParams.Add(new SqlParameter("@CostCenterName", CostCenterName));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPCostCenterMaster", SqlParams);
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
