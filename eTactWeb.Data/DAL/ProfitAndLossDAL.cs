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
    public class ProfitAndLossDAL
    {
        private readonly IDataLogic _IDataLogic;
        private readonly string DBConnectionString = string.Empty;
        private IDataReader? Reader;
        private readonly ConnectionStringService _connectionStringService;
        public ProfitAndLossDAL(IConfiguration configuration, IDataLogic iDataLogic, ConnectionStringService connectionStringService)
        {
            //DBConnectionString = configuration.GetConnectionString("eTactDB");
            _connectionStringService = connectionStringService;
            DBConnectionString = _connectionStringService.GetConnectionString();
            _IDataLogic = iDataLogic;
        }
        public async Task<ProfitAndLossModel> GetProfitAndLossData(string FromDate, string ToDate, string Flag, string ReportType)
        {
            var resultList = new ProfitAndLossModel();
            DataSet oDataSet = new DataSet();

            try
            {
                var fromDt = CommonFunc.ParseFormattedDate(FromDate);
                var toDt = CommonFunc.ParseFormattedDate(ToDate);
                using (SqlConnection connection = new SqlConnection(DBConnectionString))
                {
                    SqlCommand command = new SqlCommand("AccSpProfitAndLoss", connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    command.Parameters.AddWithValue("@FromDate", fromDt);
                    command.Parameters.AddWithValue("@ToDate", toDt);
                    command.Parameters.AddWithValue("@ReportType", ReportType);
                    command.Parameters.AddWithValue("@flag",Flag);

                    await connection.OpenAsync();

                    using (SqlDataAdapter dataAdapter = new SqlDataAdapter(command))
                    {
                        dataAdapter.Fill(oDataSet);
                    }
                }
                if (ReportType == "SUMMARY")
                {
                    if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                    {
                        resultList.ProfitAndLossGrid = (from DataRow row in oDataSet.Tables[0].Rows
                                                       select new ProfitAndLossModel
                                                       {
                                                           TotalCRbeforeGrossProfit = row["TotalCRbeforeGrossProfit"] == DBNull.Value ? 0 : Convert.ToDecimal(row["TotalCRbeforeGrossProfit"]),
                                                           TotalDRbeforeGrossProfit = row["TotalDRbeforeGrossProfit"] == DBNull.Value ? 0 : Convert.ToDecimal(row["TotalDRbeforeGrossProfit"]),

                                                       }).ToList();
                    }
                }
                else
                {
                    if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                    {
                        resultList.ProfitAndLossGrid = (from DataRow row in oDataSet.Tables[0].Rows
                                                       select new ProfitAndLossModel
                                                       {
                                                           TotalCRbeforeGrossProfit = row["TotalCRbeforeGrossProfit"] == DBNull.Value ? 0 : Convert.ToDecimal(row["TotalCRbeforeGrossProfit"]),
                                                           TotalDRbeforeGrossProfit = row["TotalDRbeforeGrossProfit"] == DBNull.Value ? 0 : Convert.ToDecimal(row["TotalDRbeforeGrossProfit"]),

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
