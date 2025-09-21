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
        public async Task<ProfitAndLossModel> GetProfitAndLossData(string FromDate, string ToDate, string Flag, string ReportType, string ShowOpening ,  string ShowRecordWithZeroAmt, int? ParentAccountCode)
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
                    command.Parameters.AddWithValue("@flag", "ProfitAndLoss");
                    command.Parameters.AddWithValue("@ShowOpening", ShowOpening);
                    command.Parameters.AddWithValue("@ShowRecordWithZeroAmt", ShowRecordWithZeroAmt);
                    command.Parameters.AddWithValue("@ParentAccountCode", (object)ParentAccountCode ?? DBNull.Value);
                    command.Parameters.AddWithValue("@reportCallingFrom", "FromP&LForm");
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
                        var rowData = new ProfitAndLossRow();

                        foreach (DataColumn col in table.Columns)
                        {
                            rowData.DynamicColumns[col.ColumnName] = dr[col] == DBNull.Value ? null : dr[col];
                        }

                        resultList.ProfitAndLossGrid.Add(rowData);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error fetching Profit and Loss data.", ex);
            }

            return resultList;
        }

        public async Task<ResponseResult> GetGroupData(string FromDate, string ToDate, string ReportType, string ShowOpening, string ShowRecordWithZeroAmt)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var fromDt = CommonFunc.ParseFormattedDate(FromDate);
                var toDt = CommonFunc.ParseFormattedDate(ToDate);

                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@FromDate", fromDt));
                SqlParams.Add(new SqlParameter("@ToDate", toDt));
                SqlParams.Add(new SqlParameter("@ReportType",ReportType));
                SqlParams.Add(new SqlParameter("@Flag", "FillGroupData"));
                SqlParams.Add(new SqlParameter("@ShowOpening", ShowOpening));
                SqlParams.Add(new SqlParameter("@ShowRecordWithZeroAmt", ShowRecordWithZeroAmt));
                SqlParams.Add(new SqlParameter("@reportCallingFrom", "FromP&LForm"));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("AccSpProfitAndLoss", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;

        }
    }
}
