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
    public class TransactionLedgerDAL
    {
        private readonly IDataLogic _IDataLogic;
        private readonly string DBConnectionString = string.Empty;
        private IDataReader? Reader;

        public TransactionLedgerDAL(IConfiguration configuration, IDataLogic iDataLogic)
        {
            DBConnectionString = configuration.GetConnectionString("eTactDB");
            _IDataLogic = iDataLogic;
        }
        public async Task<ResponseResult> GetLedgerName()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "GetLedgerName"));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("AccSpTRansactionLedgerAndGroupList", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;

        }
        public async Task<TransactionLedgerModel> GetDetailsData(string FromDate, string ToDate, int AccountCode, string ReportType)
        {
            var resultList = new TransactionLedgerModel();
            DataSet oDataSet = new DataSet();

            try
            {
                using (SqlConnection connection = new SqlConnection(DBConnectionString))
                {
                    SqlCommand command = new SqlCommand("AccSpTransactionLedger", connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    DateTime currentDate = DateTime.Today;
                    DateTime firstDateOfMonth = new DateTime(currentDate.Year, currentDate.Month, 1);
                    command.Parameters.AddWithValue("@flag", "VoucherDetail");
                    command.Parameters.Add(new SqlParameter("fromDate", firstDateOfMonth.ToString("yyyy/MM/dd")));
                    command.Parameters.Add(new SqlParameter("@ToDate", currentDate.ToString("yyyy/MM/dd")));
                    command.Parameters.AddWithValue("@ACCOUNTCODE", AccountCode);
                    command.Parameters.AddWithValue("@ReportType", ReportType);

                    await connection.OpenAsync();

                    using (SqlDataAdapter dataAdapter = new SqlDataAdapter(command))
                    {
                        dataAdapter.Fill(oDataSet);
                    }
                }


                if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                {
                    resultList.TransactionLedgerGrid = (from DataRow row in oDataSet.Tables[0].Rows
                                                        select new TransactionLedgerModel
                                                        {
                                                            AccEntryId = row["AccEntryId"] == DBNull.Value ? 0 : Convert.ToInt32(row["AccEntryId"]),
                                                            AccEntryYearCode = row["AccYearCode"] == DBNull.Value ? 0 : Convert.ToInt32(row["AccYearCode"]),
                                                            VoucherDocDate = row["VoucherDocDate"] == DBNull.Value ? string.Empty : Convert.ToDateTime(row["VoucherDocDate"]).ToString("dd-MM-yyyy"),
                                                            Particulars = row["Particulars"] == DBNull.Value ? string.Empty : row["Particulars"].ToString(),
                                                            VoucherType = row["VoucherType"] == DBNull.Value ? string.Empty : row["VoucherType"].ToString(),
                                                            InvoiceVoucherNo = row["Inv/VchNo"] == DBNull.Value ? string.Empty : row["Inv/VchNo"].ToString(),
                                                            DrAmt = row["DrAmt"] == DBNull.Value ? 0 : Convert.ToDecimal(row["DrAmt"]),
                                                            CrAmt = row["CrAmt"] == DBNull.Value ? 0 : Convert.ToDecimal(row["CrAmt"]),
                                                            Balance = row["BALANCE"] == DBNull.Value ? 0 : Convert.ToDecimal(row["BALANCE"]),
                                                            Types = row["TYPES"] == DBNull.Value ? string.Empty : row["Types"].ToString(),
                                                            HeadWiseNarration = row["HeadWiseNarration"] == DBNull.Value ? string.Empty : row["HeadWiseNarration"].ToString(),
                                                            BillDate = row["BILL DATE"] == DBNull.Value ? string.Empty : Convert.ToDateTime(row["BILL DATE"]).ToString("dd-MM-yyyy"),
                                                            DocEntryId = row["DocEntryId"] == DBNull.Value ? 0 : Convert.ToInt32(row["DocEntryId"]),
                                                            SumDet = row["SUMDET"] == DBNull.Value ? string.Empty : row["SUMDET"].ToString(),
                                                            VCHEMark = row["VCHEMARK"] == DBNull.Value ? string.Empty : row["VCHEMark"].ToString(),
                                                            AccountCode = row["ACCOUNTCODE"] == DBNull.Value ? 0 : Convert.ToInt32(row["ACCOUNTCODE"]),
                                                            ReportType = row["REPORTTYPE"] == DBNull.Value ? string.Empty : row["REPORTTYPE"].ToString(),
                                                            VchNo = row["VCH NO"] == DBNull.Value ? string.Empty : row["VCH NO"].ToString()
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
        public async Task<TransactionLedgerModel> GetTransactionLedgerMonthlySummaryDetailsData(string FromentryDate, string ToEntryDate, int AccountCode)
        {
            var resultList = new TransactionLedgerModel();
            DataSet oDataSet = new DataSet();

            try
            {
                using (SqlConnection connection = new SqlConnection(DBConnectionString))
                {
                    SqlCommand command = new SqlCommand("AccSpTransactionLedgerMonthlySummary", connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    command.Parameters.AddWithValue("@FromentryDate", FromentryDate);
                    command.Parameters.AddWithValue("@ToEntryDate", ToEntryDate);
                    command.Parameters.AddWithValue("@ACCOUNTCODE", AccountCode);



                    await connection.OpenAsync();

                    using (SqlDataAdapter dataAdapter = new SqlDataAdapter(command))
                    {
                        dataAdapter.Fill(oDataSet);
                    }


                    if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                    {
                        resultList.TransactionLedgerGrid = (from DataRow row in oDataSet.Tables[0].Rows
                                                            select new TransactionLedgerModel
                                                            {
                                                                MOnthFullName = row["MOnthFullName"] == DBNull.Value ? string.Empty : row["MOnthFullName"].ToString(),

                                                                TotalCr = row["TotalCr"] == DBNull.Value ? 0 : Convert.ToDecimal(row["TotalCr"]),
                                                                TotalDr = row["TotalDr"] == DBNull.Value ? 0 : Convert.ToDecimal(row["TotalDr"]),
                                                                ClosingAmt = row["ClosingAmt"] == DBNull.Value ? 0 : Convert.ToDecimal(row["ClosingAmt"]),
                                                                Dr_CR = row["Dr/CR"] == DBNull.Value ? string.Empty : row["Dr/CR"].ToString(),
                                                                YearCode = row["YearCode"] == DBNull.Value ? 0 : Convert.ToInt32(row["YearCode"]),
                                                                SeqNo = row["SeqNo"] == DBNull.Value ? 0 : Convert.ToInt32(row["SeqNo"])

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
