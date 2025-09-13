using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.Extensions.Configuration;
using Microsoft.SqlServer.Server;
using Newtonsoft.Json;
using System;
using System.Data;
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
            _connectionStringService = connectionStringService;
            //DBConnectionString = configuration.GetConnectionString("eTactDB");
            _IDataLogic = iDataLogic;
            DBConnectionString = _connectionStringService.GetConnectionString();
        }
        public async Task<ResponseResult> NewEntryId(int YearCode,string entrydate)
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

        public async Task<SalepersonWiseRateMasterModel> DashBoard()
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

                    command.Parameters.AddWithValue("@Flag", "DashBoard");
                    

                    await connection.OpenAsync();

                    using (SqlDataAdapter dataAdapter = new SqlDataAdapter(command))
                    {
                        dataAdapter.Fill(oDataSet);
                    }
                }

                if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                {
                    resultList.DashboardDetail = (from DataRow row in oDataSet.Tables[0].Rows
                                                 select new SalepersonWiseRateMasterModel
                                                 {
                                                     EntryId = row["SalesPersonRateEntryid"] == DBNull.Value ? 0 : Convert.ToInt32(row["SalesPersonRateEntryid"]),
                                                    
                                                     YearCode = row["SalesPersonRateyearcode"] == DBNull.Value ? 0 : Convert.ToInt32(row["SalesPersonRateyearcode"]),
                                                     EntryDate = row["SalesPersonRateentrydate"] == DBNull.Value ? string.Empty : row["SalesPersonRateentrydate"].ToString(),
                                                     SalesPersonId = row["salespersonid"] == DBNull.Value ? 0 : Convert.ToInt32(row["salespersonid"]),
                                                     ItemGroupId = row["itemgroupId"] == DBNull.Value ? 0 : Convert.ToInt32(row["itemgroupId"]),
                                                     ItemCode = row["itemcode"] == DBNull.Value ? 0 : Convert.ToInt32(row["itemcode"]),
                                                     OriginalRate = row["Originalrate"] == DBNull.Value ? 0 : Convert.ToDecimal(row["Originalrate"]),
                                                     NewRate = row["Newrate"] == DBNull.Value ? 0 : Convert.ToDecimal(row["Newrate"]),
                                                     ActualEntryBy = row["actualentryby"] == DBNull.Value ? 0 : Convert.ToInt32(row["actualentryby"]),
                                                     UpdatedBy = row["updatedby"] == DBNull.Value ? 0 : Convert.ToInt32(row["updatedby"]),
                                                     UpdationDate = row["updationdate"] == DBNull.Value ? string.Empty : row["updationdate"].ToString(),
                                                     MachineName = row["machinename"] == DBNull.Value ? string.Empty : row["machinename"].ToString(),
                                                     CC = row["CC"] == DBNull.Value ? string.Empty : row["CC"].ToString(),
                                                     ActualEntryDate = row["ActualEntryDate"] == DBNull.Value ? string.Empty : row["ActualEntryDate"].ToString(),
                                                     GroupName = row["Group_name"] == DBNull.Value ? string.Empty : row["Group_name"].ToString(),
                                                     SalePersonName = row["SalePersonName"] == DBNull.Value ? string.Empty : row["SalePersonName"].ToString(),
                                                     ActualEntryByName = row["ActualEntryByName"] == DBNull.Value ? string.Empty : row["ActualEntryByName"].ToString(),
                                                     PartCode = row["PartCode"] == DBNull.Value ? string.Empty : row["PartCode"].ToString(),
                                                     ItemName = row["Item_Name"] == DBNull.Value ? string.Empty : row["Item_Name"].ToString(),



                                                 }).ToList();
                }

            }
            catch (Exception ex)
            {
                throw new Exception("Error fetching data.", ex);
            }

            return resultList;
        }

        internal async Task<SalepersonWiseRateMasterModel> GetViewByID(int ID, int YearCode)
        {
            var oDataSet = new DataSet();
            var model = new SalepersonWiseRateMasterModel();
            var _ItemList = new List<ItemDetail>();
            

            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    using (SqlCommand oCmd = new SqlCommand("SPSalepersonWiseRateMaster", myConnection))
                    {
                        oCmd.CommandType = CommandType.StoredProcedure;
                        oCmd.Parameters.AddWithValue("@Flag", "VIEWBYID");
                        oCmd.Parameters.AddWithValue("@EntryID", ID);
                        oCmd.Parameters.AddWithValue("@YearCode", YearCode);
                        await myConnection.OpenAsync();
                        using SqlDataAdapter oDataAdapter = new SqlDataAdapter(oCmd);
                        oDataAdapter.Fill(oDataSet);
                        oDataSet.Tables[0].TableName = "Salepersonratemaster";
                        
                        var SalepersonratemasterList = new List<SalepersonWiseRateDetail>();
                       
                        if (oDataSet.Tables[0] != null && oDataSet.Tables[0].Rows.Count > 0)
                        {
                            model.EntryId = Convert.ToInt32(oDataSet.Tables[0].Rows[0]["SalesPersonRateEntryid"]);
                            model.YearCode = Convert.ToInt32(oDataSet.Tables[0].Rows[0]["SalesPersonRateyearcode"]);
                            model.SalesPersonId = Convert.ToInt32(oDataSet.Tables[0].Rows[0]["salespersonid"]);
                            model.ItemGroupId = Convert.ToInt32(oDataSet.Tables[0].Rows[0]["itemgroupId"]);
                            model.ActualEntryBy = Convert.ToInt32(oDataSet.Tables[0].Rows[0]["actualentryby"]);
                            model.EntryDate =(oDataSet.Tables[0].Rows[0]["SalesPersonRateentrydate"].ToString());
                            model.ActualEntryByName =(oDataSet.Tables[0].Rows[0]["ActualEntryByName"].ToString());
                            foreach (DataRow row in oDataSet.Tables[0].Rows)
                            {
                                SalepersonratemasterList.Add(new SalepersonWiseRateDetail
                                {
                                    //EntryId = row["SalesPersonRateEntryid"] == DBNull.Value ? 0 : Convert.ToInt32(row["SalesPersonRateEntryid"]),

                                    //YearCode = row["SalesPersonRateyearcode"] == DBNull.Value ? 0 : Convert.ToInt32(row["SalesPersonRateyearcode"]),
                                    //EntryDate = row["SalesPersonRateentrydate"] == DBNull.Value ? string.Empty : row["SalesPersonRateentrydate"].ToString(),
                                    //SalesPersonId = row["salespersonid"] == DBNull.Value ? 0 : Convert.ToInt32(row["salespersonid"]),
                                    //ItemGroupId = row["itemgroupId"] == DBNull.Value ? 0 : Convert.ToInt32(row["itemgroupId"]),
                                    ItemCode = row["itemcode"] == DBNull.Value ? 0 : Convert.ToInt32(row["itemcode"]),
                                    OriginalRate = row["Originalrate"] == DBNull.Value ? 0 : Convert.ToDecimal(row["Originalrate"]),
                                    NewRate = row["Newrate"] == DBNull.Value ? 0 : Convert.ToDecimal(row["Newrate"]),
                                    //ActualEntryBy = row["actualentryby"] == DBNull.Value ? 0 : Convert.ToInt32(row["actualentryby"]),
                                    //UpdatedBy = row["updatedby"] == DBNull.Value ? 0 : Convert.ToInt32(row["updatedby"]),
                                    //UpdationDate = row["updationdate"] == DBNull.Value ? string.Empty : row["updationdate"].ToString(),
                                    //MachineName = row["machinename"] == DBNull.Value ? string.Empty : row["machinename"].ToString(),
                                    //CC = row["CC"] == DBNull.Value ? string.Empty : row["CC"].ToString(),
                                    //ActualEntryDate = row["ActualEntryDate"] == DBNull.Value ? string.Empty : row["ActualEntryDate"].ToString(),
                                    //GroupName = row["Group_name"] == DBNull.Value ? string.Empty : row["Group_name"].ToString(),
                                    //SalePersonName = row["SalePersonName"] == DBNull.Value ? string.Empty : row["SalePersonName"].ToString(),
                                    //ActualEntryByName = row["ActualEntryByName"] == DBNull.Value ? string.Empty : row["ActualEntryByName"].ToString(),
                                    PartCode = row["PartCode"] == DBNull.Value ? string.Empty : row["PartCode"].ToString(),
                                    ItemName = row["Item_Name"] == DBNull.Value ? string.Empty : row["Item_Name"].ToString(),
                                });


                                }
                            model.ItemDetailGrid = SalepersonratemasterList;
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
                oDataSet.Dispose();
            }
            return model;
        }


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

        public async Task<ResponseResult> DeleteByID(int entryid, int yearcode, string machinename, string CC, int actualentryby, int salespersonid)
        {
            var oDataTable = new DataTable();
            var _ResponseResult = new ResponseResult();
            var model = new SalepersonWiseRateMasterModel();
            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    using (SqlCommand oCmd = new SqlCommand("SPSalepersonWiseRateMaster", myConnection))
                    {
                        oCmd.CommandType = CommandType.StoredProcedure;
                        oCmd.Parameters.AddWithValue("@Flag", "DeleteBYID");
                        oCmd.Parameters.AddWithValue("@EntryID", entryid);
                        oCmd.Parameters.AddWithValue("@YearCode", yearcode);
                        oCmd.Parameters.AddWithValue("@machinename", machinename);
                        oCmd.Parameters.AddWithValue("@CC", CC);
                        oCmd.Parameters.AddWithValue("@actualentryby", actualentryby);
                        oCmd.Parameters.AddWithValue("@salespersonid", salespersonid);
                        await myConnection.OpenAsync();
                        using (SqlDataAdapter oDataAdapter = new SqlDataAdapter(oCmd))
                        {
                            oDataAdapter.Fill(oDataTable);
                        }

                        if (oDataTable.Rows.Count > 0)
                        {
                            _ResponseResult = new ResponseResult()
                            {
                                StatusCode = HttpStatusCode.OK,
                                StatusText = oDataTable.Rows[0]["StatusText"].ToString(),
                                Result = oDataTable.Rows[0]["Result"]
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
                oDataTable.Dispose();
            }
            return _ResponseResult;
        }

        public async Task<ResponseResult> GetExcelData(string Code)
        {
            var oDataTable = new DataTable();
            var _ResponseResult= new ResponseResult();

            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    using (SqlCommand oCmd = new SqlCommand("SPSalepersonWiseRateMaster", myConnection))
                    {
                        oCmd.CommandType = CommandType.StoredProcedure;
                        oCmd.Parameters.AddWithValue("@Flag", "GetExcelItemData");
                        oCmd.Parameters.AddWithValue("@PartCode", Code);
                        await myConnection.OpenAsync();
                        using (SqlDataAdapter oDataAdapter = new SqlDataAdapter(oCmd))
                        {
                            oDataAdapter.Fill(oDataTable);
                        }

                        if (oDataTable.Rows.Count > 0)
                        {
                            _ResponseResult = new ResponseResult()
                            {
                                StatusCode = HttpStatusCode.OK,
                                StatusText = "Success",
                                Result = oDataTable
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
                if (oDataTable != null)
                {
                    oDataTable.Dispose();
                }
            }
            return _ResponseResult;
        }



    }
}
