using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;            
using System.Data.SqlClient; 
using System.Net;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;


namespace eTactWeb.Data.DAL
{
    public class HSNMasterDAL
    {
        private readonly IDataLogic _IDataLogic;
        private readonly string DBConnectionString = string.Empty;
        private readonly ConnectionStringService _connectionStringService;

        public HSNMasterDAL(IConfiguration configuration, IDataLogic iDataLogic, ConnectionStringService connectionStringService)
        {
            _IDataLogic = iDataLogic;
            _connectionStringService = connectionStringService;
            DBConnectionString = _connectionStringService.GetConnectionString();
        }

        // ---------------------- INSERT / UPDATE ----------------------
        public async Task<ResponseResult> SaveHSN(HSNMasterModel model)
        {
            var responseResult = new ResponseResult();

            try
            {
                if (string.IsNullOrWhiteSpace(model.HSNNo) ||
                    string.IsNullOrWhiteSpace(model.HSNTypeItemServ) ||
                    string.IsNullOrWhiteSpace(model.EntryByMachineName) ||
                    model.ActualEntryBy <= 0)
                {
                    throw new ArgumentException("Required fields are missing or invalid.");
                }

                var sqlParams = new List<dynamic>
        {
            new SqlParameter("@Flag", model.Mode == "U" ? "UPDATE" : "INSERT"),
            new SqlParameter("@HSNEntryID", model.Mode == "U" ? (object)model.HSNEntryID : 0),
            new SqlParameter("@hsnno", model.HSNNo),
            new SqlParameter("@HSNTypeItemServ", model.HSNTypeItemServ),
            new SqlParameter("@EntryByMachineName", model.EntryByMachineName),
            new SqlParameter("@ActualEntryBy", model.ActualEntryBy)
        };

                responseResult = await _IDataLogic.ExecuteDataTable("[SPHSNMaster]", sqlParams);

                // Extract returned DataTable
                if (responseResult.Result is DataTable dt && dt.Rows.Count > 0)
                {
                    responseResult.StatusCode = HttpStatusCode.OK;
                    responseResult.StatusText = "Success";
                    responseResult.Rows = dt.Rows.Count;
                    responseResult.Result = dt;
                }
                else
                {
                    responseResult.StatusCode = HttpStatusCode.BadRequest;
                    responseResult.StatusText = "No data returned from SP";
                    responseResult.Rows = 0;
                }
            }
            catch (Exception ex)
            {
                responseResult.StatusCode = HttpStatusCode.InternalServerError;
                responseResult.StatusText = "Error";
                responseResult.Result = new { ex.Message, ex.StackTrace };
                responseResult.Rows = 0;
            }

            return responseResult;
        }



        // ---------------------- GET NEW ENTRY ID ----------------------
        //public async Task<ResponseResult> GetNewEntryId()
        //{
        //    var response = new ResponseResult();
        //    try
        //    {
        //        var sqlParams = new List<dynamic>
        //{
        //    new SqlParameter("@Flag", "NewEntryId"),
        //    new SqlParameter
        //    {
        //        ParameterName = "@HSNEntryID",
        //        SqlDbType = SqlDbType.BigInt,
        //        Direction = ParameterDirection.Output
        //    }
        //};

        //        // Still use ExecuteDataTable
        //        await _IDataLogic.ExecuteDataTable("[SPHSNMaster]", sqlParams);

        //        // ✅ Output parameter will now contain value
        //        long newId = sqlParams
        //            .First(p => p.ParameterName == "@HSNEntryID").Value != DBNull.Value
        //            ? Convert.ToInt64(sqlParams.First(p => p.ParameterName == "@HSNEntryID").Value)
        //            : 0;

        //        response.StatusCode = HttpStatusCode.OK;
        //        response.StatusText = "Success";
        //        response.Result = newId;
        //    }
        //    catch (Exception ex)
        //    {
        //        response.StatusCode = HttpStatusCode.InternalServerError;
        //        response.StatusText = "Error";
        //        response.Result = ex.Message;
        //    }
        //    return response;
        //}


        public async Task<int> GetNewEntryId()
        {
            int newId = 0;
            try
            {
                var sqlParams = new List<SqlParameter>
        {
            new SqlParameter("@Flag", "NewEntryId"),
            new SqlParameter
            {
                ParameterName = "@HSNEntryID",
                SqlDbType = SqlDbType.BigInt,
                Direction = ParameterDirection.Output
            }
        };

                // Execute SP
                await _IDataLogic.ExecuteDataTable("[SPHSNMaster]", sqlParams.Cast<dynamic>().ToList());

                // Read OUTPUT parameter manually
                var outputParam = sqlParams.First(p => p.ParameterName == "@HSNEntryID");
                if (outputParam.Value != DBNull.Value)
                    newId = Convert.ToInt32(outputParam.Value);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[GetNewEntryId] Error: {ex.Message}");
                newId = 0;
            }

            return newId;
        }






        // ---------------------- GET DASHBOARD ----------------------
        public async Task<IEnumerable<HSNMasterModel>> GetDashboard()
        {
            var sqlParams = new List<dynamic>
            {
                new SqlParameter("@Flag", "DASHBAORD"),
                new SqlParameter("@HSNEntryID", DBNull.Value)
            };

            var response = await _IDataLogic.ExecuteDataTable("[SPHSNMaster]", sqlParams);
            var result = new List<HSNMasterModel>();

            if (response.Result is DataTable dt)
            {
                foreach (DataRow row in dt.Rows)
                {
                    result.Add(new HSNMasterModel
                    {
                        HSNEntryID = row["HSNEntryID"] != DBNull.Value ? Convert.ToInt32(row["HSNEntryID"]) : 0,
                        HSNNo = row["HSNNo"] != DBNull.Value ? row["HSNNo"].ToString() : string.Empty,
                        HSNTypeItemServ = row["HSNTypeItemServ"] != DBNull.Value ? row["HSNTypeItemServ"].ToString() : string.Empty
                    });
                }
            }

            return result;
        }

        // ---------------------- GET BY ID ----------------------
        public async Task<HSNMasterModel> GetById(int id)
        {
            var sqlParams = new List<dynamic>
            {
                new SqlParameter("@Flag", "VIEWBYID"),
                new SqlParameter("@HSNEntryID", id)
            };

            var response = await _IDataLogic.ExecuteDataTable("[SPHSNMaster]", sqlParams);

            if (response.Result is DataTable dt && dt.Rows.Count > 0)
            {
                var row = dt.Rows[0];
                return new HSNMasterModel
                {
                    HSNEntryID = row["HSNEntryID"] != DBNull.Value ? Convert.ToInt32(row["HSNEntryID"]) : 0,
                    HSNNo = row["HSNNo"] != DBNull.Value ? row["HSNNo"].ToString() : string.Empty,
                    HSNTypeItemServ = row["HSNTypeItemServ"] != DBNull.Value ? row["HSNTypeItemServ"].ToString() : string.Empty
                };
            }
            return null;
        }

        // ---------------------- DELETE ----------------------
        public async Task<ResponseResult> Delete(int id)
        {
            var sqlParams = new List<dynamic>
            {
                new SqlParameter("@Flag", "DELETE"),
                new SqlParameter("@HSNEntryID", id)
            };

            var result = await _IDataLogic.ExecuteDataTable("[SPHSNMaster]", sqlParams);

            if (result.Result == null)
            {
                result.StatusText = "Success"; // treat empty result as success
                result.StatusCode = HttpStatusCode.OK;
            }

            return result;
        }

    }
}
