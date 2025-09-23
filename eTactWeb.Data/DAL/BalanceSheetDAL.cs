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
    public class BalanceSheetDAL
    {
        private readonly IDataLogic _IDataLogic;
        private readonly string DBConnectionString = string.Empty;
        private IDataReader? Reader;
        private readonly ConnectionStringService _connectionStringService;
        public BalanceSheetDAL(IConfiguration configuration, IDataLogic iDataLogic, ConnectionStringService connectionStringService)
        {
            //DBConnectionString = configuration.GetConnectionString("eTactDB");
            _connectionStringService = connectionStringService;
            DBConnectionString = _connectionStringService.GetConnectionString();
            _IDataLogic = iDataLogic;
        }
        public async Task<BalanceSheetModel> GetBalanceSheetData(string FromDate, string ToDate, string ReportType)
        {
            var resultList = new BalanceSheetModel();
            DataSet oDataSet = new DataSet();

            try
            {
                var fromDt = CommonFunc.ParseFormattedDate(FromDate);
                var toDt = CommonFunc.ParseFormattedDate(ToDate);

                using (SqlConnection connection = new SqlConnection(DBConnectionString))
                {
                    SqlCommand command = new SqlCommand("AccSpBalanceSheet", connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    command.Parameters.AddWithValue("@FromDate", fromDt);
                    command.Parameters.AddWithValue("@ToDate", toDt);
                    command.Parameters.AddWithValue("@ReportType", ReportType);
                    await connection.OpenAsync();

                    using (SqlDataAdapter dataAdapter = new SqlDataAdapter(command))
                    {
                        dataAdapter.Fill(oDataSet);
                    }
                }

                if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                {
                    var table = oDataSet.Tables[0];

                    foreach (DataRow dr in table.Rows)
                    {
                        var rowData = new BalanceSheetRow();

                        foreach (DataColumn col in table.Columns)
                        {
                            rowData.DynamicColumns[col.ColumnName] = dr[col] == DBNull.Value ? null : dr[col];
                        }

                        resultList.BalanceSheetGrid.Add(rowData);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error fetching Profit and Loss data.", ex);
            }

            return resultList;
        }
    }
}
