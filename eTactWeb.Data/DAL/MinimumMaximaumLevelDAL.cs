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
    public class MinimumMaximaumLevelDAL
    {
        private readonly IDataLogic _IDataLogic;
        private readonly string DBConnectionString = string.Empty;
        private IDataReader? Reader;
        private readonly ConnectionStringService _connectionStringService;
        public MinimumMaximaumLevelDAL(IConfiguration configuration, IDataLogic iDataLogic, ConnectionStringService connectionStringService)
        {
            //DBConnectionString = configuration.GetConnectionString("eTactDB");
            _connectionStringService = connectionStringService;
            DBConnectionString = _connectionStringService.GetConnectionString();
            _IDataLogic = iDataLogic;
        }
        public async Task<ResponseResult> FillStoreName()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FILLSTORENAME"));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPReportMinMaxLevel", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> FillPartCode()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FILLPARTCODE"));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPReportMinMaxLevel", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> FillItemName()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FILLTEMNAME"));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPReportMinMaxLevel", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
  
        public async Task<MinimumMaximaumLevelModel> GetStandardDetailsData(string fromDate, string toDate, string ReportType, string PartCode,string  StoreName,int Yearcode,string CurrentDate,string ShowItem)
        {
            var resultList = new MinimumMaximaumLevelModel();
            DataSet oDataSet = new DataSet();

            try
            {
                using (SqlConnection connection = new SqlConnection(DBConnectionString))
                {
                    SqlCommand command = new SqlCommand("SPReportMinMaxLevel", connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    var currentDt = CommonFunc.ParseFormattedDate(DateTime.Now.ToString("dd/MM/yyyy"));
                    command.Parameters.AddWithValue("@Flag", ReportType);
                    command.Parameters.AddWithValue("@PartCode", PartCode);
                    command.Parameters.AddWithValue("@Yearcode", Yearcode);
                    command.Parameters.AddWithValue("@CurrentDate", currentDt);
                    command.Parameters.AddWithValue("@StoreName", StoreName);
                 
                    await connection.OpenAsync();

                    // Execute command and fill dataset
                    using (SqlDataAdapter dataAdapter = new SqlDataAdapter(command))
                    {
                        dataAdapter.Fill(oDataSet);
                    }
                }
                if (ReportType.ToString() == "Standard")
                {
                    // Check if data exists and map it to the model
                    if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                    {
                        resultList.MinimumMaximaumLevelGrid = (from DataRow row in oDataSet.Tables[0].Rows
                                                    select new MinimumMaximaumLevelModel                                                    {
                                                        ItemName = row["ItemName"].ToString(),
                                                        PartCode = row["PartCode"].ToString(),
                                                        MinimumLevel = Convert.ToInt32(row["MinimumLevel"]),
                                                        MaximumLevel = Convert.ToInt32(row["MaximumLevel"]),
                                                        MaxWipStock = Convert.ToInt32(row["MaxWipStock"]),
                                                        Unit = row["Unit"].ToString(),
                                                        ParentGroup = row["ParentGroup"].ToString(),
                                                        ItemCategory = row["ItemCategory"].ToString()
                                                        
                                                    }).ToList();
                    }
                }
                else if (ReportType == "SHOWMINMAXWITHSTOCK")
                {
                    if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                    {
                        resultList.MinimumMaximaumLevelGrid = (from DataRow row in oDataSet.Tables[0].Rows
                                                               select new MinimumMaximaumLevelModel
                                                               {
                                                                   ItemName = row["ItemName"].ToString(),
                                                                   PartCode = row["PartCode"].ToString(),
                                                                   MinimumLevel = Convert.ToInt32(row["MinimumLevel"]),
                                                                   MaximumLevel = Convert.ToInt32(row["MaximumLevel"]),
                                                                   MaxWipStock = Convert.ToInt32(row["MaxWipStock"]),
                                                                   StoreStock = Convert.ToInt32(row["StoreStock"]),
                                                                   WIPStock = Convert.ToInt32(row["WIPStock"]),
                                                                   StoreShortQty = Convert.ToInt32(row["StoreShortQty"]),
                                                                   StoreExcessQty = Convert.ToInt32(row["StoreExcessQty"]),
                                                                   WIPExcessQty = Convert.ToInt32(row["WIPExcessQty"]),
                                                                   Unit = row["Unit"].ToString(),
                                                                   ParentGroup = row["ParentGroup"].ToString(),
                                                                   ItemCategory = row["ItemCategory"].ToString()
                                                               }).ToList();
                    }
                }
                //else if (ReportType == "SHOWMINMAXWITHSTOCKWITHPENDPOQTY" || ReportType == "SHOWMINMAXWITHSTOCKWITHPENDPODETAIL")
                //{
                //    if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                //    {
                //        resultList.MinimumMaximaumLevelGrid = (from DataRow row in oDataSet.Tables[0].Rows
                //                                               select new MinimumMaximaumLevelModel
                //                                               {
                //                                                   AccountCode = Convert.ToInt32(row["Accountcode"]),
                //                                                   PONO = row["PONO"].ToString(),
                //                                                   PODate = Convert.ToDateTime(row["PODate"]),
                //                                                   POYearCode = Convert.ToInt32(row["POYearCioide"]),
                //                                                   ScheduleNo = row["SchNo"].ToString(),
                //                                                   ScheduleYearCode = Convert.ToInt32(row["SchYearcode"]),
                //                                                   ScheduleDate = row.IsNull("Schdate") ? null : Convert.ToString(row["Schdate"]),
                //                                                   ItemCode = Convert.ToInt32(row["ItemCode"]),
                //                                                   POQty = Convert.ToInt32(row["POQty"]),
                //                                                   PendPOQty = Convert.ToInt32(row["PendPOQty"]),
                //                                                   Rate = Convert.ToInt32(row["Rate"]),
                //                                                   PendValue = Convert.ToInt32(row["PendValue"])
                //                                               }).ToList();
                //    }
                //}
                else if (ReportType == "SHOWMINMAXWITHSTOCKWITHPENDPOQTY")
                {
                    resultList.MinimumMaximaumLevelGrid = (from DataRow row in oDataSet.Tables[0].Rows
                                                           select new MinimumMaximaumLevelModel
                                                           {
                                                               ItemName = row["ItemName"].ToString(),
                                                               PartCode = row["PartCode"].ToString(),
                                                               MinimumLevel = Convert.ToInt32(row["MinimumLevel"]),
                                                               MaximumLevel = Convert.ToInt32(row["MaximumLevel"]),
                                                               ReorderLevel = Convert.ToInt32(row["ReorderLvl"]),
                                                               MaxWipStock = Convert.ToInt32(row["MaxWipStock"]),
                                                               StoreStock = Convert.ToInt32(row["StoreStock"]),
                                                               WIPStock = Convert.ToInt32(row["WIPStock"]),
                                                               StoreShortQty = Convert.ToInt32(row["StoreShortQty"]),
                                                               StoreExcessQty = Convert.ToInt32(row["StoreExcessQty"]),
                                                               PendPOQty = Convert.ToInt32(row["PendPOQty"]),
                                                               ShortQtyAfterPOPendQty = Convert.ToInt32(row["ShortQtyAfterPOPendQTy"]),
                                                               Unit = row["Unit"].ToString(),
                                                               WIPExcessQty = Convert.ToInt32(row["WIPExcessQty"]),
                                                               ParentGroup = row["ParentGroup"].ToString(),
                                                               ItemCategory = row["ItemCategory"].ToString()
                                                           }).ToList();
                }
                else if (ReportType == "SHOWMINMAXWITHSTOCKWITHPENDPODETAIL")
                {
                    resultList.MinimumMaximaumLevelGrid = (from DataRow row in oDataSet.Tables[0].Rows
                                                           select new MinimumMaximaumLevelModel
                                                           {
                                                               ItemName = row.IsNull("ItemName") ? null : row["ItemName"].ToString(),
                                                               PartCode = row.IsNull("PartCode") ? null : row["PartCode"].ToString(),
                                                               MinimumLevel = Convert.ToInt32(row["MinimumLevel"]),
                                                               MaximumLevel = Convert.ToInt32(row["MaximumLevel"]),
                                                               ReorderLevel = Convert.ToInt32(row["ReorderLvl"]),
                                                               MaxWipStock = Convert.ToInt32(row["MaxWipStock"]),
                                                               StoreStock = Convert.ToInt32(row["StoreStock"]),
                                                               WIPStock = Convert.ToInt32(row["WIPStock"]),
                                                               StoreShortQty = Convert.ToInt32(row["StoreShortQty"]),
                                                               StoreExcessQty = Convert.ToInt32(row["StoreExcessQty"]),
                                                               VendorName = row.IsNull("VendorName") ? null : row["VendorName"].ToString(),
                                                               PONO = row.IsNull("PONO") ? null : row["PONO"].ToString(),
                                                               PODate = row.IsNull("PODate") ? (DateTime?)null : Convert.ToDateTime(row["PODate"]),
                                                               SchNO = row.IsNull("SchNo") ? null : row["SchNo"].ToString(),
                                                               SchDate = row.IsNull("Schdate") ? (DateTime?)null : Convert.ToDateTime(row["Schdate"]),
                                                               PendPOQty = Convert.ToInt32(row["PendPOQty"]),
                                                               POQty = Convert.ToInt32(row["POQty"]),
                                                               Rate = Convert.ToInt32(row["Rate"]),
                                                               PendValue = Convert.ToInt32(row["PendValue"]),
                                                               ShortQtyAfterPOPendQty = Convert.ToInt32(row["ShortQtyAfterPOPendQTy"]),
                                                               Unit = row.IsNull("Unit") ? null : row["Unit"].ToString(),
                                                               WIPExcessQty =  Convert.ToInt32(row["WIPExcessQty"]),
                                                               ParentGroup = row.IsNull("ParentGroup") ? null : row["ParentGroup"].ToString(),
                                                               ItemCategory = row.IsNull("ItemCategory") ? null : row["ItemCategory"].ToString()
                                                           }).ToList();
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
