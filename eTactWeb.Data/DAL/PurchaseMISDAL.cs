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
    public class PurchaseMISDAL
    {
        private readonly IDataLogic _IDataLogic;
        private readonly string DBConnectionString = string.Empty;
        private IDataReader? Reader;
        private readonly ConnectionStringService _connectionStringService;
        public PurchaseMISDAL(IConfiguration configuration, IDataLogic iDataLogic, ConnectionStringService connectionStringService)
        {
            //DBConnectionString = configuration.GetConnectionString("eTactDB");
            _connectionStringService = connectionStringService;
            DBConnectionString = _connectionStringService.GetConnectionString();
            _IDataLogic = iDataLogic;
            _connectionStringService = connectionStringService;
        }
        public async Task<ResponseResult> FillItemName()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@flag", "FillItemName"));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPReportPurchasePriceAnalysis", SqlParams);
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
                SqlParams.Add(new SqlParameter("@flag", "FillPartCode"));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPReportPurchasePriceAnalysis", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> FillAccountName()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@flag", "FillVendore"));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPReportPurchasePriceAnalysis", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<PurchaseMISModel> GetPurchaseMISDetailsData(string ReportType, string ToDate, int YearCode, int Itemcode, int AccountCode)
        {
            var resultList = new PurchaseMISModel();
            DataSet oDataSet = new DataSet();

            try
            {
                using (SqlConnection connection = new SqlConnection(DBConnectionString))
                {
                    SqlCommand command = new SqlCommand("SPReportPurchasePriceAnalysis", connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    var toDt = CommonFunc.ParseFormattedDate(ToDate);
                    command.Parameters.AddWithValue("@flag", ReportType);
                    command.Parameters.AddWithValue("@todate", toDt);
                    command.Parameters.AddWithValue("@itemCode", Itemcode);
                    command.Parameters.AddWithValue("@YEARCODE", YearCode);
                    command.Parameters.AddWithValue("@accountcode", AccountCode);
                    //command.Parameters.AddWithValue("@WCID", WorkCenterid);
                    // command.Parameters.AddWithValue("@RMItemcode", RMItemCode);


                    // Open connection
                    await connection.OpenAsync();

                    // Execute command and fill dataset
                    using (SqlDataAdapter dataAdapter = new SqlDataAdapter(command))
                    {
                        dataAdapter.Fill(oDataSet);
                    }
                }
                if (ReportType == "Monthly Rate Analysis")
                {
                    // Check if data exists and map it to the model
                    if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                    {
                        resultList.PurchaseMISGrid = (from DataRow row in oDataSet.Tables[0].Rows
                                                               select new PurchaseMISModel
                                                               {
                                                                   ItemName = row["ItemName"] != DBNull.Value ? row["ItemName"].ToString() : string.Empty,
                                                                   PartCode = row["PartName"] != DBNull.Value ? row["PartName"].ToString() : string.Empty,
                                                                   AccountName = row["VendorName"] != DBNull.Value ? row["VendorName"].ToString() : string.Empty,

                                                                   APR = row["APR"] != DBNull.Value ? Convert.ToDouble(row["APR"]) : 0,
                                                                   MAY = row["MAY"] != DBNull.Value ? Convert.ToDouble(row["MAY"]) : 0,
                                                                   JUN = row["JUN"] != DBNull.Value ? Convert.ToDouble(row["JUN"]) : 0,
                                                                   JUL = row["JUL"] != DBNull.Value ? Convert.ToDouble(row["JUL"]) : 0,
                                                                   AUG = row["AUG"] != DBNull.Value ? Convert.ToDouble(row["AUG"]) : 0,
                                                                   SEP = row["SEP"] != DBNull.Value ? Convert.ToDouble(row["SEP"]) : 0,
                                                                   OCT = row["OCT"] != DBNull.Value ? Convert.ToDouble(row["OCT"]) : 0,
                                                                   NOV = row["NOV"] != DBNull.Value ? Convert.ToDouble(row["NOV"]) : 0,
                                                                   DEC = row["DEC"] != DBNull.Value ? Convert.ToDouble(row["DEC"]) : 0,
                                                                   JAN = row["JAN"] != DBNull.Value ? Convert.ToDouble(row["JAN"]) : 0,
                                                                   FEB = row["FEB"] != DBNull.Value ? Convert.ToDouble(row["FEB"]) : 0,
                                                                   MAR = row["MAR"] != DBNull.Value ? Convert.ToDouble(row["MAR"]) : 0

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
