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
    public class DayBookDAL
    {
        private readonly IDataLogic _IDataLogic;
        private readonly string DBConnectionString = string.Empty;
        private IDataReader? Reader;
        private readonly ConnectionStringService _connectionStringService;
        public DayBookDAL(IConfiguration configuration, IDataLogic iDataLogic, ConnectionStringService connectionStringService)
        {
            //DBConnectionString = configuration.GetConnectionString("eTactDB");
            _connectionStringService = connectionStringService;
            DBConnectionString = _connectionStringService.GetConnectionString();
            _IDataLogic = iDataLogic;
        }
        public async Task<ResponseResult> FillLedgerName(string FromDate, string ToDate)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "LEDGER"));
                SqlParams.Add(new SqlParameter("@Date", ParseFormattedDate(FromDate)));
                SqlParams.Add(new SqlParameter("@ToDate", ParseFormattedDate(ToDate)));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("AccSPDayBook", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> FillVoucherName(string FromDate, string ToDate)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "VOUCHERTYPE"));
                SqlParams.Add(new SqlParameter("@Date", ParseFormattedDate(FromDate)));
                SqlParams.Add(new SqlParameter("@ToDate", ParseFormattedDate(ToDate)));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("AccSPDayBook", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<DayBookModel> GetDayBookDetailsData(string FromDate, string ToDate, string Ledger, string VoucherType, string CrAmt, string DrAmt)
        {
            var resultList = new DayBookModel();
            DataSet oDataSet = new DataSet();

            try
            {
                DateTime currentDate = DateTime.Today;
                DateTime firstDateOfMonth = new DateTime(currentDate.Year, currentDate.Month, 1);
                using (SqlConnection connection = new SqlConnection(DBConnectionString))
                {
                    SqlCommand command = new SqlCommand("AccSPDayBook", connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    var fromDt = CommonFunc.ParseFormattedDate(FromDate);
                    var toDt = CommonFunc.ParseFormattedDate(ToDate);

                    command.Parameters.AddWithValue("@flag", "DAYBOOK");
                    command.Parameters.Add(new SqlParameter("@Date", fromDt));
                    command.Parameters.Add(new SqlParameter("@ToDate", toDt));
                    command.Parameters.AddWithValue("@LedgerHead", Ledger);
                    command.Parameters.AddWithValue("@amtcr", CrAmt);
                    command.Parameters.AddWithValue("@amtdr", DrAmt);
                    command.Parameters.AddWithValue("@VoucherType", VoucherType);
                    command.Parameters.AddWithValue("@DateType", "D");

                    await connection.OpenAsync();

                    using (SqlDataAdapter dataAdapter = new SqlDataAdapter(command))
                    {
                        dataAdapter.Fill(oDataSet);
                    }
                }
                if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                {
                    resultList.DayBookGrid = (from DataRow row in oDataSet.Tables[0].Rows
                                              select new DayBookModel
                                              {
                                                  Ledger = row["Ledger"] == DBNull.Value ? string.Empty : row["Ledger"].ToString(),
                                                  VoucherNo = row["VoucherNo"] == DBNull.Value ? string.Empty : row["VoucherNo"].ToString(),
                                                  InvoiceNo = row["InvoiceNo"] == DBNull.Value ? string.Empty : row["InvoiceNo"].ToString(),
                                                  VoucherDate = row["VoucherDate"] == DBNull.Value ? string.Empty : row["VoucherDate"].ToString(),
                                                  VoucherType = row["VoucherType"] == DBNull.Value ? string.Empty : row["VoucherType"].ToString(),
                                                  DrAmt = row["DrAmt"] == DBNull.Value ? string.Empty : row["DrAmt"].ToString(),
                                                  CrAmt = row["CrAmt"] == DBNull.Value ? string.Empty : row["CrAmt"].ToString(),
                                                  EntryDate = row["Entry_Date"] == DBNull.Value ? string.Empty : row["Entry_Date"].ToString(),
                                                  Category = row["Category"] == DBNull.Value ? "N" : row["Category"].ToString(),
                                                  ParentHead = row["ParentHead"] == DBNull.Value ? "N" : row["ParentHead"].ToString(),
                                                  EnteredBy = row["EnteredBy"] == DBNull.Value ? "N" : row["EnteredBy"].ToString(),
                                                  EntryByMachine = row["EntryByMachine"] == DBNull.Value ? "N" : row["EntryByMachine"].ToString(),
                                                  AccountCode = row["AccountCode"] == DBNull.Value ? 0 : Convert.ToInt32(row["AccountCode"]),
                                                  AccEntryId = row["AccEntryId"] == DBNull.Value ? 0 : Convert.ToInt32(row["AccEntryId"]),
                                                  AccYearCode = row["AccYearCode"] == DBNull.Value ? 0 : Convert.ToInt32(row["AccYearCode"])
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
