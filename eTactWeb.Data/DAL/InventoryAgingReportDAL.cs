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
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace eTactWeb.Data.DAL
{
    public class InventoryAgingReportDAL
    {
        private readonly IDataLogic _IDataLogic;
        private readonly string DBConnectionString = string.Empty;
        private IDataReader? Reader;
        private readonly ConnectionStringService _connectionStringService;
        public InventoryAgingReportDAL(IConfiguration configuration, IDataLogic iDataLogic, ConnectionStringService connectionStringService)
        {
            //DBConnectionString = configuration.GetConnectionString("eTactDB");
            _connectionStringService = connectionStringService;
            DBConnectionString = _connectionStringService.GetConnectionString();
            _IDataLogic = iDataLogic;
            _connectionStringService = connectionStringService;
        }
        public async Task<ResponseResult> FillRMItemName(string FromDate, string ToDate, string CurrentDate, int Storeid)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@flag", "RMItemName"));
                SqlParams.Add(new SqlParameter("@Fromdate", FromDate));
                SqlParams.Add(new SqlParameter("@ToDate", ToDate));
                SqlParams.Add(new SqlParameter("@CurrentDate", CurrentDate));
                SqlParams.Add(new SqlParameter("@StoreId", Storeid));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SpInventoryAgingReport", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> FillRMPartCode(string FromDate, string ToDate, string CurrentDate, int Storeid)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "RMPARTCODE"));
                SqlParams.Add(new SqlParameter("@Fromdate", FromDate));
                SqlParams.Add(new SqlParameter("@ToDate", ToDate));
                SqlParams.Add(new SqlParameter("@CurrentDate", CurrentDate));
                SqlParams.Add(new SqlParameter("@StoreId", Storeid));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SpInventoryAgingReport", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> FillStoreName(string FromDate, string ToDate, string CurrentDate, int Storeid)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FillStore"));
                SqlParams.Add(new SqlParameter("@Fromdate", FromDate));
                SqlParams.Add(new SqlParameter("@ToDate", ToDate));
                SqlParams.Add(new SqlParameter("@CurrentDate", CurrentDate));
                SqlParams.Add(new SqlParameter("@StoreId", Storeid));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SpInventoryAgingReport", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> FillWorkCenterName(string FromDate, string ToDate, string CurrentDate, int Storeid)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FillWorkcenter"));
                SqlParams.Add(new SqlParameter("@Fromdate", FromDate));
                SqlParams.Add(new SqlParameter("@ToDate", ToDate));
                SqlParams.Add(new SqlParameter("@CurrentDate", CurrentDate));
                SqlParams.Add(new SqlParameter("@StoreId", Storeid));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SpInventoryAgingReport", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<InventoryAgingReportModel> GetInventoryAgingReportDetailsData(string fromDate, string toDate,string CurrentDate, int WorkCenterid, string ReportType, int RMItemCode, int Storeid)
        {
            var resultList = new InventoryAgingReportModel();
            DataSet oDataSet = new DataSet();

            try
            {
                using (SqlConnection connection = new SqlConnection(DBConnectionString))
                {
                    SqlCommand command = new SqlCommand("SpInventoryAgingReport", connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    command.Parameters.AddWithValue("@flag", ReportType);
                    command.Parameters.AddWithValue("@Fromdate", fromDate);
                    command.Parameters.AddWithValue("@ToDate", toDate);
                    command.Parameters.AddWithValue("@CurrentDate", CurrentDate);
                    command.Parameters.AddWithValue("@Storeid", Storeid);
                    command.Parameters.AddWithValue("@WCID", WorkCenterid);
                   // command.Parameters.AddWithValue("@RMItemcode", RMItemCode);


                    // Open connection
                    await connection.OpenAsync();

                    // Execute command and fill dataset
                    using (SqlDataAdapter dataAdapter = new SqlDataAdapter(command))
                    {
                        dataAdapter.Fill(oDataSet);
                    }
                }
                if (ReportType == "Aging(SLAB WISE)")
                {
                    // Check if data exists and map it to the model
                    if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                    {
                        resultList.InventoryAgingReportGrid = (from DataRow row in oDataSet.Tables[0].Rows
                                                            select new InventoryAgingReportModel
                                                            {
                                                                PartCode = row["PartCode"] != DBNull.Value ? row["PartCode"].ToString() : string.Empty,
                                                                ItemName = row["ItemName"] != DBNull.Value ? row["ItemName"].ToString() : string.Empty,
                                                                MRNNo = row["MRNNo"] != DBNull.Value ? row["MRNNo"].ToString() : string.Empty,
                                                                MRNEntryDate = row["MRNEntryDate"] != DBNull.Value ? Convert.ToDateTime(row["MRNEntryDate"]).ToString("dd/MM/yyyy") : string.Empty,
                                                                PONo = row["pono"] != DBNull.Value ? row["pono"].ToString() : string.Empty,
                                                                PODate = row["PODate"] != DBNull.Value ? Convert.ToDateTime(row["PODate"]).ToString("dd/MM/yyyy") : string.Empty,
                                                                RecQty = row["RecQty"] != DBNull.Value ? Convert.ToDecimal(row["RecQty"]) : 0,
                                                                BatchNo = row["Batchno"] != DBNull.Value ? row["Batchno"].ToString() : string.Empty,
                                                                UniqueBatchNo = row["Uniquebatchno"] != DBNull.Value ? row["Uniquebatchno"].ToString() : string.Empty,
                                                                Rate = row["Rate"] != DBNull.Value ? Convert.ToDecimal(row["Rate"]) : 0,
                                                                //TotalValue = row["TotalValue"] != DBNull.Value ? Convert.ToDecimal(row["TotalValue"]) : 0,
                                                                Unit = row["Unit"] != DBNull.Value ? row["Unit"].ToString() : string.Empty,
                                                                PartyName = row["PartyName"] != DBNull.Value ? row["PartyName"].ToString() : string.Empty,
                                                                //LastMovementDate = row["LastMovementDate"] != DBNull.Value ? Convert.ToDateTime(row["LastMovementDate"]).ToString("dd/MM/yyyy") : string.Empty,
                                                                //InvDays = row["InvDays"] != DBNull.Value ? Convert.ToInt32(row["InvDays"]) : 0,
                                                                //AgingStatus = row["AgingStatus"] != DBNull.Value ? row["AgingStatus"].ToString() : string.Empty,
                                                                //AccountCode = row["AccountCode"] != DBNull.Value ? Convert.ToInt32(row["AccountCode"]) : 0,
                                                                //ItemCode = row["ItemCode"] != DBNull.Value ? Convert.ToInt32(row["ItemCode"]) : 0
                                                                Aging_0_30 = row["0-30"] != DBNull.Value ? Convert.ToInt32(row["0-30"]) : 0,
                                                                Aging_31_60 = row["31-60"] != DBNull.Value ? Convert.ToInt32(row["31-60"]) : 0,
                                                                Aging_61_90 = row["61-90"] != DBNull.Value ? Convert.ToInt32(row["61-90"]) : 0,
                                                                Aging_90_180 = row["90-180"] != DBNull.Value ? Convert.ToInt32(row["90-180"]) : 0,
                                                                Aging_180_360 = row["180-360"] != DBNull.Value ? Convert.ToInt32(row["180-360"]) : 0,
                                                                Aging_Above360 = row[">360"] != DBNull.Value ? Convert.ToInt32(row[">360"]) : 0
                                                            }).ToList();
                    }
                }
                else if (ReportType.ToString() == "Aging(Detail)")
                {
                    if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                    {


                        resultList.InventoryAgingReportGrid = (from DataRow row in oDataSet.Tables[0].Rows
                                                            select new InventoryAgingReportModel
                                                            {
                                                                PartCode = row["PartCode"] != DBNull.Value ? row["PartCode"].ToString() : string.Empty,
                                                                ItemName = row["ItemName"] != DBNull.Value ? row["ItemName"].ToString() : string.Empty,
                                                                StoreStock = row["StoreStock"] != DBNull.Value ? Convert.ToInt32(row["StoreStock"]) : 0,
                                                                WIPStock = row["WIPStock"] != DBNull.Value ? Convert.ToInt32(row["WIPStock"]) : 0,
                                                                MRNNo = row["MRNNo"] != DBNull.Value ? row["MRNNo"].ToString() : string.Empty,
                                                                MRNEntryDate = row["MRNEntryDate"] != DBNull.Value ? Convert.ToDateTime(row["MRNEntryDate"]).ToString("dd/MM/yyyy") : string.Empty,
                                                                PONo = row["pono"] != DBNull.Value ? row["pono"].ToString() : string.Empty,
                                                                PODate = row["PODate"] != DBNull.Value ? Convert.ToDateTime(row["PODate"]).ToString("dd/MM/yyyy") : string.Empty,
                                                                RecQty = row["RecQty"] != DBNull.Value ? Convert.ToDecimal(row["RecQty"]) : 0,
                                                                BatchNo = row["Batchno"] != DBNull.Value ? row["Batchno"].ToString() : string.Empty,
                                                                UniqueBatchNo = row["Uniquebatchno"] != DBNull.Value ? row["Uniquebatchno"].ToString() : string.Empty,
                                                                Rate = row["Rate"] != DBNull.Value ? Convert.ToDecimal(row["Rate"]) : 0,
                                                                TotalValue = row["TotalValue"] != DBNull.Value ? Convert.ToDecimal(row["TotalValue"]) : 0,
                                                                Unit = row["Unit"] != DBNull.Value ? row["Unit"].ToString() : string.Empty,
                                                                PartyName = row["PartyName"] != DBNull.Value ? row["PartyName"].ToString() : string.Empty,
                                                                LastMovementDate = row["LastMovementDate"] != DBNull.Value ? Convert.ToDateTime(row["LastMovementDate"]).ToString("dd/MM/yyyy") : string.Empty,
                                                                InvDays = row["InvDays"] != DBNull.Value ? Convert.ToInt32(row["InvDays"]) : 0,
                                                                AgingStatus = row["AgingStatus"] != DBNull.Value ? row["AgingStatus"].ToString() : string.Empty,
                                                                AccountCode = row["AccountCode"] != DBNull.Value ? Convert.ToInt32(row["AccountCode"]) : 0,
                                                                ItemCode = row["ItemCode"] != DBNull.Value ? Convert.ToInt32(row["ItemCode"]) : 0,
                                                                
                                                            }).ToList();
                    }
                }
                

            }
            catch (Exception ex)
            {
                // Handle exception (log it or rethrow)
                throw new Exception("Error fetching BOM tree data.", ex);
            }

            return resultList;
        }
    }
}
