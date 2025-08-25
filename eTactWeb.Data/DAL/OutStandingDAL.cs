using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;
using static eTactWeb.Data.Common.CommonFunc;
using eTactWeb.Data.Common;

namespace eTactWeb.Data.DAL
{
    public class OutStandingDAL
    {
        private readonly IDataLogic _IDataLogic;
        private readonly string DBConnectionString = string.Empty;
        private IDataReader? Reader;
        private readonly ConnectionStringService _connectionStringService;
        public OutStandingDAL(IConfiguration configuration, IDataLogic iDataLogic, ConnectionStringService connectionStringService)
        {
            _connectionStringService = connectionStringService;
            DBConnectionString = _connectionStringService.GetConnectionString();
            _IDataLogic = iDataLogic;
        }

        public async Task<ResponseResult> GetPartyName(string outstandingType, string TillDate)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FillAccountName"));
                SqlParams.Add(new SqlParameter("@outstandingType", outstandingType));
                //SqlParams.Add(new SqlParameter("Debtors", underGroup));
                SqlParams.Add(new SqlParameter("@TillDate", ParseFormattedDate(TillDate)));



                _ResponseResult = await _IDataLogic.ExecuteDataTable("AccSpLedgerOutstanding", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;

        }

        public async Task<ResponseResult> GetGroupName(string outstandingType, string TillDate)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FillGroupName"));
                SqlParams.Add(new SqlParameter("@outstandingType", outstandingType));
                SqlParams.Add(new SqlParameter("@TillDate", ParseFormattedDate(TillDate)));
                SqlParams.Add(new SqlParameter("@ReportCallingFrom", "OutstandingForm"));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("AccSpLedgerOutstanding", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;

        }

        public async Task<OutStandingModel> GetDetailsData(string outstandingType, string TillDate, string GroupName, string[] AccountNameList, int AccountCode, string ShowOnlyApprovedBill, bool ShowZeroBal)
        {
            var resultList = new OutStandingModel();
            DataSet oDataSet = new DataSet();

            try
            {
                var tillDt = CommonFunc.ParseFormattedDate(TillDate);
                string accountNameCsv = string.Join(",", AccountNameList ?? new string[0]);
                using (SqlConnection connection = new SqlConnection(DBConnectionString))
                {
                    SqlCommand command = new SqlCommand("AccSpLedgerOutstanding", connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    command.Parameters.AddWithValue("@Flag", "Outstanding");
                    command.Parameters.AddWithValue("@outstandingType", outstandingType);
                    command.Parameters.AddWithValue("@TillDate", tillDt);
                    command.Parameters.AddWithValue("@Groupname", GroupName);
                    command.Parameters.AddWithValue("@AccountNamwList", accountNameCsv);
                    command.Parameters.AddWithValue("@ACCOUNTCODE", AccountCode);
                    command.Parameters.AddWithValue("@ShowOnlyApprovedBill", ShowOnlyApprovedBill);
                    command.Parameters.AddWithValue("@ShowZeroBal", ShowZeroBal);

                    await connection.OpenAsync();

                    using (SqlDataAdapter dataAdapter = new SqlDataAdapter(command))
                    {
                        dataAdapter.Fill(oDataSet);
                    }
                }

				if (oDataSet.Tables.Count > 0)
				{
					if (outstandingType == "Receive Outstanding" || outstandingType == "Payable Outstanding")
					{
						if (oDataSet.Tables[0].Rows.Count > 0)
						{
							resultList.OutStandingGrid = (from DataRow row in oDataSet.Tables[0].Rows
														  select new OutStandingModel
														  {
															  LedgerDescription = row["LedgerDescription"] == DBNull.Value ? string.Empty : row["LedgerDescription"].ToString(),
															  VoucherNo = row["VoucherNo"] == DBNull.Value ? string.Empty : row["VoucherNo"].ToString(),
															  VoucherDate = row["VoucherDate"] == DBNull.Value ? string.Empty : Convert.ToDateTime(row["VoucherDate"]).ToString("dd-MM-yyyy"),
															  VoucherType = row["VoucherType"] == DBNull.Value ? string.Empty : row["VoucherType"].ToString(),
															  DrAmt = row["DrAmt"] == DBNull.Value ? string.Empty : row["DrAmt"].ToString(),
															  CrAmt = row["CrAmt"] == DBNull.Value ? string.Empty : row["CrAmt"].ToString(),
															  BillAmt = row["BillAmt"] == DBNull.Value ? string.Empty : row["BillAmt"].ToString(),
															  PendingAmt = row["PendingAmt"] == DBNull.Value ? string.Empty : row["PendingAmt"].ToString(),
															  DueDate = row["DueDate"] == DBNull.Value ? string.Empty : Convert.ToDateTime(row["DueDate"]).ToString("dd-MM-yyyy"),
															  OverDueDays = row["OverDueDays"] == DBNull.Value ? string.Empty : row["OverDueDays"].ToString(),
															  TotBalanceAmt = row["TotBalanceAmt"] == DBNull.Value ? string.Empty : row["TotBalanceAmt"].ToString(),
															  AccEntryId = row["AccEntryId"] == DBNull.Value ? string.Empty : row["AccEntryId"].ToString(),
															  AccYearCode = row["AccYearCode"] == DBNull.Value ? string.Empty : row["AccYearCode"].ToString(),
                                                              DocEntryId = row["DocEntryId"] == DBNull.Value ? 0 : Convert.ToInt32(row["DocEntryId"]),
                                                              SalesPersonName = row["SalesPersonName"] == DBNull.Value ? string.Empty : row["SalesPersonName"].ToString(),
														  }).ToList();
						}
						else
						{
							// If no rows, return empty list instead of null
							resultList.OutStandingGrid = new List<OutStandingModel>();
						}
					}
					else
					{
						var table = oDataSet.Tables[0];
						resultList.OutStandingRow = new List<OutStandingRow>();

						if (table.Rows.Count > 0)
						{
							// Normal case: fill with data rows
							foreach (DataRow dr in table.Rows)
							{
								var rowData = new OutStandingRow();

								foreach (DataColumn col in table.Columns)
								{
									rowData.DynamicColumns[col.ColumnName] = dr[col] == DBNull.Value ? null : dr[col];
								}

								resultList.OutStandingRow.Add(rowData);
							}
						}
						else
						{
							// No rows: still return headers as empty values
							var emptyRow = new OutStandingRow();
							foreach (DataColumn col in table.Columns)
							{
								emptyRow.DynamicColumns[col.ColumnName] = null; // or "" if you prefer
							}
							resultList.OutStandingRow.Add(emptyRow);
						}
					}
				}
				else
				{
					// If dataset has no tables at all
					resultList.OutStandingGrid = new List<OutStandingModel>();
					resultList.OutStandingRow = new List<OutStandingRow>();
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
