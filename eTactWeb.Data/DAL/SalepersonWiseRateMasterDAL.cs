using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.Extensions.Configuration;
using Microsoft.SqlServer.Server;
using Newtonsoft.Json;
using System;
using System.Globalization;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Data.DAL
{
    public class SalepersonWiseRateMasterDAL
    {
        private readonly IDataLogic _IDataLogic;
        private readonly string DBConnectionString = string.Empty;
        private IDataReader? Reader;
        private readonly ConnectionStringService _connectionStringService;
        public SalepersonWiseRateMasterDAL(IConfiguration configuration, IDataLogic iDataLogic, ConnectionStringService connectionStringService)
        {
            //configuration = config;
            DBConnectionString = configuration.GetConnectionString("eTactDB");
            _IDataLogic = iDataLogic;
            _connectionStringService = connectionStringService;
            DBConnectionString = _connectionStringService.GetConnectionString();
        }
        public async Task<ResponseResult> NewEntryId(int YearCode)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "NewEntryId"));
               
                SqlParams.Add(new SqlParameter("@YearCode", YearCode));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPSalepersonWiseRateMaster", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> FillSalePerson()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@Flag", "FillSalePerson"));



                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPSalepersonWiseRateMaster", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }

        //public async Task<SalepersonWiseRateMasterModel> GetItemData(int ItemGroupId)
        //{
        //    var resultList = new SalepersonWiseRateMasterModel();
        //    DataSet oDataSet = new DataSet();

        //    try
        //    {
        //        using (SqlConnection connection = new SqlConnection(DBConnectionString))
        //        {
        //            SqlCommand command = new SqlCommand("SPSalepersonWiseRateMaster", connection)
        //            {
        //                CommandType = CommandType.StoredProcedure
        //            };
        //            ;

        //            command.Parameters.AddWithValue("@Flag", "GetItemData");
        //            command.Parameters.AddWithValue("@itemgroupid", ItemGroupId);

        //            await connection.OpenAsync();

        //            using (SqlDataAdapter dataAdapter = new SqlDataAdapter(command))
        //            {
        //                dataAdapter.Fill(oDataSet);
        //            }
        //        }

        //        if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
        //        {
        //            resultList.DashboardDetail = (from DataRow row in oDataSet.Tables[0].Rows
        //                                         select new SalepersonWiseRateMasterModel
        //                                         {
        //                                             EntryId = row["SalesPersonRateEntryId"] == DBNull.Value ? 0 : Convert.ToInt32(row["SalesPersonRateEntryId"]),
        //                                             YearCode = row["SalesPersonRateYearCode"] == DBNull.Value ? string.Empty : row["SalesPersonRateYearCode"].ToString(),
        //                                             EntryDate = row["SalesPersonRateEntryDate"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(row["SalesPersonRateEntryDate"]),
        //                                             SalesPersonId = row["SalesPersonId"] == DBNull.Value ? 0 : Convert.ToInt32(row["SalesPersonId"]),
        //                                             ItemGroupId = row["ItemGroupId"] == DBNull.Value ? 0 : Convert.ToInt32(row["ItemGroupId"]),
        //                                             ItemCode = row["ItemCode"] == DBNull.Value ? 0 : Convert.ToInt32(row["ItemCode"]),
        //                                             OriginalRate = row["OriginalRate"] == DBNull.Value ? 0 : Convert.ToDecimal(row["OriginalRate"]),
        //                                             NewRate = row["NewRate"] == DBNull.Value ? 0 : Convert.ToDecimal(row["NewRate"]),
        //                                             ActualEntryBy = row["ActualEntryBy"] == DBNull.Value ? 0 : Convert.ToInt32(row["ActualEntryBy"]),
        //                                             UpdatedBy = row["UpdatedBy"] == DBNull.Value ? 0 : Convert.ToInt32(row["UpdatedBy"]),
        //                                             UpdationDate = row["UpdationDate"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(row["UpdationDate"]),
        //                                             MachineName = row["MachineName"] == DBNull.Value ? string.Empty : row["MachineName"].ToString(),
        //                                             CC = row["CC"] == DBNull.Value ? string.Empty : row["CC"].ToString(),
        //                                             ActualEntryDate = row["ActualEntryDate"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(row["ActualEntryDate"]),
        //                                             GroupName = row["Group_Name"] == DBNull.Value ? string.Empty : row["Group_Name"].ToString(),
        //                                             SalePersonName = row["SalePersonName"] == DBNull.Value ? string.Empty : row["SalePersonName"].ToString(),
        //                                             ActualEntryByName = row["ActualEntryByName"] == DBNull.Value ? string.Empty : row["ActualEntryByName"].ToString(),


        //                                         }).ToList();
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("Error fetching data.", ex);
        //    }

        //    return resultList;
        //}



        public async Task<SalepersonWiseRateMasterModel> GetItemData(int ItemGroupId)
        {
            var resultList = new SalepersonWiseRateMasterModel();
            DataSet oDataSet = new DataSet();

            try
            {
                using (SqlConnection connection = new SqlConnection(DBConnectionString))
                {
                    SqlCommand command = new SqlCommand("SPSalepersonWiseRateMaster", connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    ;

                    command.Parameters.AddWithValue("@Flag", "GetItemData");
                    command.Parameters.AddWithValue("@itemgroupid", ItemGroupId);
                   
                    await connection.OpenAsync();

                    using (SqlDataAdapter dataAdapter = new SqlDataAdapter(command))
                    {
                        dataAdapter.Fill(oDataSet);
                    }
                }

                if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                {
                    resultList.ItemDetailGrid = (from DataRow row in oDataSet.Tables[0].Rows
                                                 select new SalepersonWiseRateDetail
                                                 {
                                                     ItemCode = row["ItemCode"] == DBNull.Value ? 0 : Convert.ToInt32(row["ItemCode"]),
                                                     
                                                     ItemName = row["Item_Name"] == DBNull.Value ? string.Empty : row["Item_Name"].ToString(),
                                                     PartCode = row["PartCode"] == DBNull.Value ? string.Empty : row["PartCode"].ToString(),
                                                  
                                                     OriginalRate = row["SalePrice"] == DBNull.Value ? 0 : Convert.ToDecimal(row["SalePrice"]),
                                                    
                                                 }).ToList();
                }

            }
            catch (Exception ex)
            {
                throw new Exception("Error fetching data.", ex);
            }

            return resultList;
        }

        public async Task<ResponseResult> SaveSalePersonWiseRate(DataTable DTItemGrid,  SalepersonWiseRateMasterModel model)
        {
            var _ResponseResult = new ResponseResult();
            var oDataTable = new DataTable();
            var SqlParams = new List<dynamic>();
            try
            {
                
               

                var EntryDate = CommonFunc.ParseFormattedDate(model.EntryDate);
               


                SqlParams.Add(new SqlParameter("@Flag", model.Mode));
                SqlParams.Add(new SqlParameter("@entryid", model.EntryId));
                SqlParams.Add(new SqlParameter("@yearcode", model.YearCode));
                SqlParams.Add(new SqlParameter("@entrydate", EntryDate));
                SqlParams.Add(new SqlParameter("@salespersonid", model.SalesPersonId));
                SqlParams.Add(new SqlParameter("@itemgroupId", model.ItemGroupId));
                SqlParams.Add(new SqlParameter("@actualentryby", model.ActualEntryBy));
                SqlParams.Add(new SqlParameter("@updatedby", model.UpdatedBy));
                SqlParams.Add(new SqlParameter("@updationdate", model.UpdationDate));
                SqlParams.Add(new SqlParameter("@machinename", model.MachineName));
                SqlParams.Add(new SqlParameter("@CC", model.CC));
              
                SqlParams.Add(new SqlParameter("@dt", DTItemGrid));
               
                // await myConnection.OpenAsync();
                // 
                // using SqlDataAdapter oDataAdapter = new SqlDataAdapter(oCmd);
                // oDataAdapter.Fill(oDataTable);
                // 
                // if (oDataTable.Rows.Count > 0)
                // {
                //     _ResponseResult = new ResponseResult()
                //     {
                //         StatusCode = HttpStatusCode.OK,
                //         StatusText = "Success",
                //         Result = oDataTable
                //     };
                // }
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPSalepersonWiseRateMaster", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
                Error.StackTrace = ex.StackTrace;
                Error.Exception = ex;
                _ResponseResult = new ResponseResult()
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    StatusText = Error.Message,
                    Result = oDataTable
                };
            }
            finally
            {
                oDataTable.Dispose();
            }
            return _ResponseResult;
        }




    }
}
