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
    public class IssueVSConsumptionReportDAL
    {
        private readonly IDataLogic _IDataLogic;
        private readonly string DBConnectionString = string.Empty;
        private IDataReader? Reader;
        private readonly ConnectionStringService _connectionStringService;
        public IssueVSConsumptionReportDAL(IConfiguration configuration, IDataLogic iDataLogic, ConnectionStringService connectionStringService)
        {
            //DBConnectionString = configuration.GetConnectionString("eTactDB");
            _connectionStringService = connectionStringService;
            DBConnectionString = _connectionStringService.GetConnectionString();
            _IDataLogic = iDataLogic;
        }

        public async Task<IssueVSConsumptionReportModel> GetDetailData()
        {
            var resultList = new IssueVSConsumptionReportModel();
            DataSet oDataSet = new DataSet();

            try
            {
                var currentDt = CommonFunc.ParseFormattedDate(DateTime.Now.ToString("dd/MM/yyyy"));
                using (SqlConnection connection = new SqlConnection(DBConnectionString))
                {
                    SqlCommand command = new SqlCommand("SPReportRequisitionVsIssueVsConsumption", connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    command.Parameters.AddWithValue("@Flag", "Req + Issue + Cons Summary");

                    // Open connection
                    await connection.OpenAsync();

                    // Execute command and fill dataset
                    using (SqlDataAdapter dataAdapter = new SqlDataAdapter(command))
                    {
                        dataAdapter.Fill(oDataSet);
                    }
                }
                
                    // Check if data exists and map it to the model
                    if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                    {
                        resultList.IssueVSConsumptionReportGrid = (from DataRow row in oDataSet.Tables[0].Rows
                                                    select new IssueVSConsumptionReportModel
                                                    {
                                                        PartCode = row["PartCode"].ToString(),
                                                        ItemName = row["ItemName"].ToString(),
                                                        ReqQty = Convert.ToSingle(row["ReqQty"]),
                                                        IssuQty = Convert.ToSingle(row["IssuQty"]),
                                                        ConsQty = Convert.ToSingle(row["ConsQty"]),
                                                        RMUnit = row["RMUnit"].ToString(),
                                                       
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
