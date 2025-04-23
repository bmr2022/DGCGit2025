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
    public class RecChallanReportDAL
    {
        private readonly IDataLogic _IDataLogic;
        private readonly string DBConnectionString = string.Empty;
        private IDataReader? Reader;
        private readonly ConnectionStringService _connectionStringService;
        public RecChallanReportDAL(IConfiguration configuration, IDataLogic iDataLogic, ConnectionStringService connectionStringService)
        {
            //DBConnectionString = configuration.GetConnectionString("eTactDB");
            _connectionStringService = connectionStringService;
            DBConnectionString = _connectionStringService.GetConnectionString();
            _IDataLogic = iDataLogic;
            _connectionStringService = connectionStringService;
        }
        public async Task<RecChallanReportModel> GetInventoryAgingReportDetailsData(string fromDate, string toDate, int EntryId, int YearCode)
        {
            var resultList = new RecChallanReportModel();
            DataSet oDataSet = new DataSet();

            try
            {
                using (SqlConnection connection = new SqlConnection(DBConnectionString))
                {
                    SqlCommand command = new SqlCommand("SPReportRecChallanReport", connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    var fromDt = CommonFunc.ParseFormattedDate(fromDate);
                    var toDt = CommonFunc.ParseFormattedDate(toDate);
                    //command.Parameters.AddWithValue("@Fromdate", fromDt);
                    //command.Parameters.AddWithValue("@ToDate", toDt);
                    command.Parameters.AddWithValue("@rcmentryid", EntryId);
                    command.Parameters.AddWithValue("@RCMYearcode", YearCode);

                  
                    await connection.OpenAsync();

                    using (SqlDataAdapter dataAdapter = new SqlDataAdapter(command))
                    {
                        dataAdapter.Fill(oDataSet);
                    }
               
                    if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                    {
                        resultList.RecChallanReportGrid = (from DataRow row in oDataSet.Tables[0].Rows
                                                               select new RecChallanReportModel
                                                               {
                                                                   EntryId = row["EntryId"] != DBNull.Value ? Convert.ToInt32(row["EntryId"]) : 0,
                                                                   YearCode = row["YearCode"] != DBNull.Value ? Convert.ToInt32(row["YearCode"]) : 0,
                                                                   RCMChallanNo = row["RCMChallanNo"] != DBNull.Value ? row["RCMChallanNo"].ToString() : string.Empty,
                                                                   RetNonRetChallan = row["RetNonRetChallan"] != DBNull.Value ? row["RetNonRetChallan"].ToString() : string.Empty,
                                                                   GateNo = row["GateNo"] != DBNull.Value ? row["GateNo"].ToString() : string.Empty,
                                                                   GateDate = row["GateDate"] != DBNull.Value ? Convert.ToDateTime(row["GateDate"]).ToString("dd/MM/yyyy") : string.Empty,
                                                                   BillChallan = row["BillChallan"] != DBNull.Value ? row["BillChallan"].ToString() : string.Empty,
                                                                   ChallanNo = row["ChallanNo"] != DBNull.Value ? row["ChallanNo"].ToString() : string.Empty,
                                                                   ChallanDate = row["ChallanDate"] != DBNull.Value ? Convert.ToDateTime(row["ChallanDate"]).ToString("dd/MM/yyyy") : string.Empty,
                                                                   PartyName = row["PartyName"] != DBNull.Value ? row["PartyName"].ToString() : string.Empty,
                                                                   ChallanType = row["ChallanType"] != DBNull.Value ? row["ChallanType"].ToString() : string.Empty,
                                                                   MRNNo = row["MRNNo"] != DBNull.Value ? row["MRNNo"].ToString() : string.Empty,
                                                                   MRNDate = row["MRNDate"] != DBNull.Value ? Convert.ToDateTime(row["MRNDate"]).ToString("dd/MM/yyyy") : string.Empty,
                                                                   PartCode = row["PartCode"] != DBNull.Value ? row["PartCode"].ToString() : string.Empty,
                                                                   ItemName = row["ItemName"] != DBNull.Value ? row["ItemName"].ToString() : string.Empty,
                                                                   GateQty = row["GateQty"] != DBNull.Value ? Convert.ToDecimal(row["GateQty"]) : 0,
                                                                   RecQty = row["RecQty"] != DBNull.Value ? Convert.ToDecimal(row["RecQty"]) : 0,
                                                                   Rate = row["Rate"] != DBNull.Value ? Convert.ToDecimal(row["Rate"]) : 0,
                                                                   Amount = row["Amount"] != DBNull.Value ? Convert.ToDecimal(row["Amount"]) : 0,
                                                                   IssueChallanNo = row["IssueChallanNo"] != DBNull.Value ? row["IssueChallanNo"].ToString() : string.Empty,
                                                                   IssueChallanYearCode = row["IssueChallanYearCode"] != DBNull.Value ? Convert.ToInt32(row["IssueChallanYearCode"]) : 0,
                                                                   IssuedQty = row["IssuedQty"] != DBNull.Value ? Convert.ToDecimal(row["IssuedQty"]) : 0,
                                                                   Remark = row["Remark"] != DBNull.Value ? row["Remark"].ToString() : string.Empty,
                                                                   RecInStore = row["RecInStore"] != DBNull.Value ? row["RecInStore"].ToString() : string.Empty,
                                                                   DocTypeCode = row["DocTypeCode"] != DBNull.Value ? row["DocTypeCode"].ToString() : string.Empty,
                                                                   DocName = row["DocName"] != DBNull.Value ? row["DocName"].ToString() : string.Empty
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
