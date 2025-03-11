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
    public class StockValuationDAL
    {
        private readonly IDataLogic _IDataLogic;
        private readonly string DBConnectionString = string.Empty;
        private IDataReader? Reader;

        public StockValuationDAL(IConfiguration configuration, IDataLogic iDataLogic)
        {
            DBConnectionString = configuration.GetConnectionString("eTactDB");
            _IDataLogic = iDataLogic;
        }
        public async Task<ResponseResult> FillStoreName(string FromDate,string CurrentDate)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FillStore"));
                SqlParams.Add(new SqlParameter("@FromDate", FromDate));
                SqlParams.Add(new SqlParameter("@CurrentDate", CurrentDate));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("AccSPStockValuation", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<StockValuationModel> GetStockValuationDetailsData(string FromDate, string ToDate,string StoreId,string ReportType)
        {
            var resultList = new StockValuationModel();
            DataSet oDataSet = new DataSet();

            try
            {
                using (SqlConnection connection = new SqlConnection(DBConnectionString))
                {
                    SqlCommand command = new SqlCommand("AccSPStockValuation", connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    command.Parameters.AddWithValue("@FromDate", FromDate);
                    command.Parameters.AddWithValue("@CurrentDate", ToDate);
                    command.Parameters.AddWithValue("@StoreId", StoreId);
                    command.Parameters.AddWithValue("@Flag", ReportType);

                    await connection.OpenAsync();

                    using (SqlDataAdapter dataAdapter = new SqlDataAdapter(command))
                    {
                        dataAdapter.Fill(oDataSet);
                    }
                }
                if (ReportType.ToString() == "BatchWise Stock Valuation")
                {
                    if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                    {
                        resultList.StockValuationGrid = (from DataRow row in oDataSet.Tables[0].Rows
                                                         select new StockValuationModel
                                                         {
                                                             PartCode = row["PartCode"] == DBNull.Value ? string.Empty : row["PartCode"].ToString(),
                                                             ItemName = row["ItemName"] == DBNull.Value ? string.Empty : row["ItemName"].ToString(),
                                                             OpeningStock = row["OpeningStock"] == DBNull.Value ? 0 : Convert.ToInt32(row["OpeningStock"]),
                                                             OpenRate = row["OpenRate"] == DBNull.Value ? 0 : Convert.ToInt32(row["OpenRate"]),
                                                             OpeningValue = row["OpeningValue"] == DBNull.Value ? 0 : Convert.ToInt32(row["OpeningValue"]),
                                                             RecQty = row["RecQty"] == DBNull.Value ? 0 : Convert.ToInt32(row["RecQty"]),
                                                             IssueQty = row["IssueQty"] == DBNull.Value ? 0 : Convert.ToInt32(row["IssueQty"]),
                                                             ClosingStock = row["ClosingStock"] == DBNull.Value ? 0 : Convert.ToInt32(row["ClosingStock"]),
                                                             Rate = row["Rate"] == DBNull.Value ? 0 : Convert.ToInt32(row["Rate"]),
                                                             ClosingValue = row["ClosingValue"] == DBNull.Value ? 0 : Convert.ToInt32(row["ClosingValue"]),
                                                             StoreName = row["StoreName"] == DBNull.Value ? string.Empty : row["StoreName"].ToString(),
                                                             //BatchNo = row["BatchNo"] == DBNull.Value ? 0 : Convert.ToInt32(row["BatchNo"]),
                                                             //UniqueBatchNo = row["UniqueBatchNo"] == DBNull.Value ? 0 : Convert.ToInt32(row["UniqueBatchNo"])

                                                         }).ToList();
                    }
                }
                else if (ReportType.ToString() == "BatchWise Stock+WIP Valuation"|| ReportType.ToString() == "BatchWise WIP Valuation")
                {
                    if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                    {
                        resultList.StockValuationGrid = (from DataRow row in oDataSet.Tables[0].Rows
                                                         select new StockValuationModel
                                                         {
                                                             PartCode = row["PartCode"] == DBNull.Value ? string.Empty : row["PartCode"].ToString(),
                                                             ItemName = row["ItemName"] == DBNull.Value ? string.Empty : row["ItemName"].ToString(),
                                                             OpeningStock = row["OpeningStock"] == DBNull.Value ? 0 : Convert.ToInt32(row["OpeningStock"]),
                                                             OpenRate = row["OpenRate"] == DBNull.Value ? 0 : Convert.ToInt32(row["OpenRate"]),
                                                             OpeningValue = row["OpeningValue"] == DBNull.Value ? 0 : Convert.ToInt32(row["OpeningValue"]),
                                                             RecQty = row["RecQty"] == DBNull.Value ? 0 : Convert.ToInt32(row["RecQty"]),
                                                             IssueQty = row["IssueQty"] == DBNull.Value ? 0 : Convert.ToInt32(row["IssueQty"]),
                                                             ClosingStock = row["ClosingStock"] == DBNull.Value ? 0 : Convert.ToInt32(row["ClosingStock"]),
                                                             Rate = row["Rate"] == DBNull.Value ? 0 : Convert.ToInt32(row["Rate"]),
                                                             ClosingValue = row["ClosingValue"] == DBNull.Value ? 0 : Convert.ToInt32(row["ClosingValue"]),
                                                             StoreName = row["StoreName"] == DBNull.Value ? string.Empty : row["StoreName"].ToString(),
                                                             BatchNo = row["BatchNo"] == DBNull.Value ? string.Empty : row["BatchNo"].ToString(),
                                                             UniqueBatchNo = row["UniqueBatchNo"] == DBNull.Value ? string.Empty : row["UniqueBatchNo"].ToString()

                                                         }).ToList();
                    }
                }
                else if (ReportType.ToString() == "Stock Valuation")
                {
                    if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                    {
                        resultList.StockValuationGrid = (from DataRow row in oDataSet.Tables[0].Rows
                                                         select new StockValuationModel
                                                         {
                                                             PartCode = row["PartCode"] == DBNull.Value ? string.Empty : row["PartCode"].ToString(),
                                                             ItemName = row["ItemName"] == DBNull.Value ? string.Empty : row["ItemName"].ToString(),
                                                             OpeningStock = row["OpeningStock"] == DBNull.Value ? 0 : Convert.ToInt32(row["OpeningStock"]),
                                                             OpenRate = row["OpenRate"] == DBNull.Value ? 0 : Convert.ToInt32(row["OpenRate"]),
                                                             OpeningValue = row["OpeningValue"] == DBNull.Value ? 0 : Convert.ToInt32(row["OpeningValue"]),
                                                             RecQty = row["RecQty"] == DBNull.Value ? 0 : Convert.ToInt32(row["RecQty"]),
                                                             IssueQty = row["IssueQty"] == DBNull.Value ? 0 : Convert.ToInt32(row["IssueQty"]),
                                                             ClosingStock = row["ClosingStock"] == DBNull.Value ? 0 : Convert.ToInt32(row["ClosingStock"]),
                                                             Rate = row["Rate"] == DBNull.Value ? 0 : Convert.ToInt32(row["Rate"]),
                                                             ClosingValue = row["ClosingValue"] == DBNull.Value ? 0 : Convert.ToInt32(row["ClosingValue"]),
                                                             StoreName = row["StoreName"] == DBNull.Value ? string.Empty : row["StoreName"].ToString()


                                                         }).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error fetching data.", ex);
            }

            return resultList;
        }
    }
}
