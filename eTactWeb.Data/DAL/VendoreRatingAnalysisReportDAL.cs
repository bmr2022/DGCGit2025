using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eTactWeb.Data.DAL
{
    public class VendoreRatingAnalysisReportDAL
    {
        private readonly IDataLogic _IDataLogic;
        private readonly string DBConnectionString = string.Empty;
        private IDataReader? Reader;
        private readonly ConnectionStringService _connectionStringService;
        public VendoreRatingAnalysisReportDAL(IConfiguration configuration, IDataLogic iDataLogic, ConnectionStringService connectionStringService)
        {
            _connectionStringService = connectionStringService;
            DBConnectionString = _connectionStringService.GetConnectionString();
            _IDataLogic = iDataLogic;
            _connectionStringService = connectionStringService;
        }
        public async Task<VendoreRatingAnalysisReportModel> GetVendoreRatingDetailsData(string ReportType, string RatingType, string CurrentDate, string PartCode, string ItemName, string CustomerName, int YearCode)
        {
            var resultList = new VendoreRatingAnalysisReportModel();
            DataSet oDataSet = new DataSet();

            try
            {
                using (SqlConnection connection = new SqlConnection(DBConnectionString))
                {
                    SqlCommand command = new SqlCommand("SPReportVendoreRatingAnalysisAndVendoreReport", connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    var currentDt = CommonFunc.ParseFormattedDate(CurrentDate);
                    command.Parameters.AddWithValue("@FLAG", RatingType);
                    command.Parameters.AddWithValue("@ReportType", ReportType);
                    command.Parameters.AddWithValue("@CurrentDate", currentDt);
                    command.Parameters.AddWithValue("@itemcode", ItemName);
                    command.Parameters.AddWithValue("@AccountCode", CustomerName);
                    command.Parameters.AddWithValue("@CurrentYear", YearCode);
                    await connection.OpenAsync();

                    using (SqlDataAdapter dataAdapter = new SqlDataAdapter(command))
                    {
                        dataAdapter.Fill(oDataSet);
                    }
                }
                if (ReportType == "SUMMARY" && RatingType == "DELIVERY RATING")
                {

                    if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                    {
                        resultList.VendoreRatingAnalysisReportGrid = (from DataRow row in oDataSet.Tables[0].Rows
                                                                      select new VendoreRatingAnalysisReportModel
                                                                      {
                                                                          CustomerName = row["VendorName"] != DBNull.Value ? row["VendorName"].ToString() : string.Empty,

                                                                          DeliveryRating = row["AvgDeliveryRating"] != DBNull.Value ? Convert.ToDecimal(row["AvgDeliveryRating"]) : 0,
                                                                          AccountCode = row["Accountcode"] != DBNull.Value ? Convert.ToInt32(row["Accountcode"]) : 0,

                                                                      }).ToList();
                    }
                }
                else if (ReportType == "SUMMARY"&& RatingType == "QUALITY RATING")
                {
                    if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                    {
                        resultList.VendoreRatingAnalysisReportGrid = (from DataRow row in oDataSet.Tables[0].Rows
                                                                      select new VendoreRatingAnalysisReportModel
                                                                      {
                                                                          CustomerName = row["VendorName"] != DBNull.Value ? row["VendorName"].ToString() : string.Empty,
                                                                          QualityPercent = row["QualityPercent"] != DBNull.Value ? Convert.ToDecimal(row["QualityPercent"]) : 0m,
                                                                          QualityRating = row["QualityRating"] != DBNull.Value ? Convert.ToDecimal(row["QualityRating"]) : 0m,
                                                                          AccountCode = row["AccountCode"] != DBNull.Value ? Convert.ToInt32(row["AccountCode"]) : 0,

                                                                      }).ToList();
                    }
                }
                else if (ReportType == "DETAIL"&& RatingType == "QUALITY RATING")
                {
                    if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                    {
                        resultList.VendoreRatingAnalysisReportGrid = (from DataRow row in oDataSet.Tables[0].Rows
                                                                      select new VendoreRatingAnalysisReportModel
                                                                      {
                                                                          CustomerName = row["VendorName"] != DBNull.Value ? row["VendorName"].ToString() : string.Empty,
                                                                          PartCode = row["PartCode"] != DBNull.Value ? row["PartCode"].ToString() : string.Empty,
                                                                          ItemName = row["ItemName"] != DBNull.Value ? row["ItemName"].ToString() : string.Empty,
                                                                          PONO = row["PONO"] != DBNull.Value ? row["PONO"].ToString() : string.Empty,
                                                                          SchNo = row["SchNo"] != DBNull.Value ? row["SchNo"].ToString() : string.Empty,
                                                                          POQty = row["POQty"] != DBNull.Value ? Convert.ToDecimal(row["POQty"]) : 0,
                                                                          BillQty = row["BillQty"] != DBNull.Value ? Convert.ToDecimal(row["BillQty"]) : 0,
                                                                          RecQty = row["RecQty"] != DBNull.Value ? Convert.ToDecimal(row["RecQty"]) : 0,
                                                                          RejectedQty = row["RejectedQty"] != DBNull.Value ? Convert.ToDecimal(row["RejectedQty"]) : 0,
                                                                          ReWorkQty = row["ReWorkQty"] != DBNull.Value ? Convert.ToDecimal(row["ReWorkQty"]) : 0,
                                                                          QualityRating = row["QualityRating"] != DBNull.Value ? Convert.ToDecimal(row["QualityRating"]) : 0,
                                                                          //AccountCode = row["Accountcode"] != DBNull.Value ? Convert.ToInt32(row["Accountcode"]) : 0,


                                                                      }).ToList();
                    }
                }

                else if (RatingType == "PRICE RATING")
                {
                    if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                    {
                        resultList.VendoreRatingAnalysisReportGrid = (from DataRow row in oDataSet.Tables[0].Rows
                                                                      select new VendoreRatingAnalysisReportModel
                                                                      {
                                                                          CustomerName = row["VendorName"] != DBNull.Value ? row["VendorName"].ToString() : string.Empty,
                                                                          PartCode = row["PartCode"] != DBNull.Value ? row["PartCode"].ToString() : string.Empty,
                                                                          ItemName = row["ItemName"] != DBNull.Value ? row["ItemName"].ToString() : string.Empty,
                                                                          VendorRate = row["VendorRate"] != DBNull.Value ? Convert.ToDecimal(row["VendorRate"]) : 0,
                                                                          MinPORate = row["MinPORate"] != DBNull.Value ? Convert.ToDecimal(row["MinPORate"]) : 0,
                                                                          MaxPORate = row["MaxPORate"] != DBNull.Value ? Convert.ToDecimal(row["MaxPORate"]) : 0,
                                                                          PriceRating = row["PriceRating"] != DBNull.Value ? Convert.ToDecimal(row["PriceRating"]) : 0,

                                                                      }).ToList();
                    }
                }
                
                else if (ReportType == "DETAIL" && RatingType == "DELIVERY RATING")
                {
                    if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                    {


                        resultList.VendoreRatingAnalysisReportGrid = (from DataRow row in oDataSet.Tables[0].Rows
                                                               select new VendoreRatingAnalysisReportModel
                                                               {
                                                                   CustomerName = row["VendorName"] != DBNull.Value ? row["VendorName"].ToString() : string.Empty,
                                                                   PartCode = row["PartCode"] != DBNull.Value ? row["PartCode"].ToString() : string.Empty,
                                                                   ItemName = row["ItemName"] != DBNull.Value ? row["ItemName"].ToString() : string.Empty,
                                                                   POQty = row["POQty"] != DBNull.Value ? Convert.ToDecimal(row["POQty"]) : 0,
                                                                   GateQty = row["GateQty"] != DBNull.Value ? Convert.ToDecimal(row["GateQty"]) : 0,
                                                                   DeliveryRating = row["AvgDeliveryRating"] != DBNull.Value ? Convert.ToDecimal(row["AvgDeliveryRating"]) : 0,
                                                                   ItemCode = row["ItemCode"] != DBNull.Value ? Convert.ToInt32(row["ItemCode"]) : 0,
                                                                   AccountCode = row["Accountcode"] != DBNull.Value ? Convert.ToInt32(row["Accountcode"]) : 0,
                                                                 
                                                               }).ToList();
                    }
                }
                else if (ReportType == "DETAIL" && RatingType == "DELIVERY RATING")
                {
                    if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                    {


                        resultList.VendoreRatingAnalysisReportGrid = (from DataRow row in oDataSet.Tables[0].Rows
                                                               select new VendoreRatingAnalysisReportModel
                                                               {
                                                                   CustomerName = row["VendorName"] != DBNull.Value ? row["VendorName"].ToString() : string.Empty,
                                                                   PartCode = row["PartCode"] != DBNull.Value ? row["PartCode"].ToString() : string.Empty,
                                                                   ItemName = row["ItemName"] != DBNull.Value ? row["ItemName"].ToString() : string.Empty,
                                                                   POQty = row["POQty"] != DBNull.Value ? Convert.ToDecimal(row["POQty"]) : 0,
                                                                   GateQty = row["GateQty"] != DBNull.Value ? Convert.ToDecimal(row["GateQty"]) : 0,
                                                                   DeliveryRating = row["AvgDeliveryRating"] != DBNull.Value ? Convert.ToDecimal(row["AvgDeliveryRating"]) : 0,
                                                                   ItemCode = row["ItemCode"] != DBNull.Value ? Convert.ToInt32(row["ItemCode"]) : 0,
                                                                   AccountCode = row["Accountcode"] != DBNull.Value ? Convert.ToInt32(row["Accountcode"]) : 0,
                                                                 
                                                               }).ToList();
                    }
                }


            }
            catch (Exception ex)
            {
                throw new Exception("Error fetching BOM tree data.", ex);
            }

            return resultList;
        }
    }
}
