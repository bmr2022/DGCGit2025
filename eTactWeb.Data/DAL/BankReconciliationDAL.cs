
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
    public class BankReconciliationDAL
    {
        private readonly IDataLogic _IDataLogic;
        private readonly string DBConnectionString = string.Empty;
        private IDataReader? Reader;
        private readonly ConnectionStringService _connectionStringService;
        public BankReconciliationDAL(IConfiguration configuration, IDataLogic iDataLogic, ConnectionStringService connectionStringService)
        {
            //DBConnectionString = configuration.GetConnectionString("eTactDB");
            _connectionStringService = connectionStringService;
            DBConnectionString = _connectionStringService.GetConnectionString();
            _IDataLogic = iDataLogic;
        }

        public async Task<ResponseResult> GetBankName(string DateFrom, string DateTo, string NewOrEdit)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FillBankNAme"));
                SqlParams.Add(new SqlParameter("@NewOrEdit", NewOrEdit));
                SqlParams.Add(new SqlParameter("@DateFrom", ParseFormattedDate(DateFrom)));
                SqlParams.Add(new SqlParameter("@DateTo", ParseFormattedDate(DateTo)));


                _ResponseResult = await _IDataLogic.ExecuteDataTable("AccSpBankreconcilation", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;

        }


        public async Task<BankReconciliationModel> GetDetailsData(string DateFrom, string DateTo, string chequeNo, string NewOrEdit,string Account_Code)
        {
            var resultList = new BankReconciliationModel();
            DataSet oDataSet = new DataSet();

            try
            {
                using (SqlConnection connection = new SqlConnection(DBConnectionString))
                {
                    SqlCommand command = new SqlCommand("AccSpBankreconcilation", connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    command.Parameters.AddWithValue("@Flag", "BanckRecoNewEntryDataDisplayOnGrid");
                    command.Parameters.AddWithValue("@NewOrEdit", NewOrEdit);
                    command.Parameters.AddWithValue("@chequeNo", chequeNo);
                    command.Parameters.AddWithValue("@BankAccountCode", Account_Code);
                    command.Parameters.AddWithValue("@DateFrom", ParseFormattedDate(DateFrom));
                    command.Parameters.AddWithValue("@DateTo", ParseFormattedDate(DateTo));


                    await connection.OpenAsync();

                    using (SqlDataAdapter dataAdapter = new SqlDataAdapter(command))
                    {
                        dataAdapter.Fill(oDataSet);
                    }
                }

                if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                {
                    resultList.BankReconciliationGrid = (from DataRow row in oDataSet.Tables[0].Rows
                                                         select new BankReconciliationModel
                                                         {
                                                             Date = row["Date"] == DBNull.Value ? string.Empty : Convert.ToDateTime(row["Date"]).ToString("dd-MM-yyyy"),
                                                             Perticuler = row["Perticuler"] == DBNull.Value ? string.Empty : row["Perticuler"].ToString(),
                                                             Type = row["Type"] == DBNull.Value ? string.Empty : row["Type"].ToString(),
                                                             BankDate = row["BankDate"] == DBNull.Value ? string.Empty : Convert.ToDateTime(row["BankDate"]).ToString("dd-MM-yyyy"),
                                                             DrAmt = row["DrAmt"] == DBNull.Value ? string.Empty : row["DrAmt"].ToString(),
                                                             CrAmt = row["CrAmt"] == DBNull.Value ? string.Empty : row["CrAmt"].ToString(),
                                                             ChequeNo = row["ChequeNo"] == DBNull.Value ? string.Empty : row["ChequeNo"].ToString(),
                                                             entryid = row["entryid"] == DBNull.Value ? string.Empty : row["entryid"].ToString(),
                                                             AccYearCode = row["AccYearCode"] == DBNull.Value ? string.Empty : row["AccYearCode"].ToString(),


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