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
    public class AccGroupLedgerDAL
    {
        private readonly IDataLogic _IDataLogic;
        private readonly string DBConnectionString = string.Empty;
        private IDataReader? Reader;

        public AccGroupLedgerDAL(IConfiguration configuration, IDataLogic iDataLogic)
        {
            DBConnectionString = configuration.GetConnectionString("eTactDB");
            _IDataLogic = iDataLogic;
        }
        public async Task<ResponseResult> FillGroupName(string FromDate, string ToDate)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FillGroups"));
                SqlParams.Add(new SqlParameter("@FromFormName", "GROUPSUMMARYFORM"));
                SqlParams.Add(new SqlParameter("@FromDate", FromDate));
                SqlParams.Add(new SqlParameter("@toDate", ToDate));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("AccSpTrailBalancesheetProfitLossGroupLedger", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<AccGroupLedgerModel> GetGroupLedgerDetailsData(string FromDate, string ToDate, int GroupCode, string ReportType)
        {
            var resultList = new AccGroupLedgerModel();
            DataSet oDataSet = new DataSet();

            try
            {
                DateTime currentDate = DateTime.Today;
                DateTime firstDateOfMonth = new DateTime(currentDate.Year, currentDate.Month, 1);
                using (SqlConnection connection = new SqlConnection(DBConnectionString))
                {
                    SqlCommand command = new SqlCommand("AccSpTrailBalancesheetProfitLossGroupLedger", connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    // command.Parameters.AddWithValue("@flag", "GROUPSUMMARYFORM");
                    command.Parameters.Add(new SqlParameter("@FromDate", firstDateOfMonth.ToString("yyyy/MM/dd")));
                    command.Parameters.Add(new SqlParameter("@ToDate", currentDate.ToString("yyyy/MM/dd")));
                    command.Parameters.AddWithValue("@GroupCode", GroupCode);
                    command.Parameters.AddWithValue("@ReportTypeSummDetail", ReportType);
                    command.Parameters.AddWithValue("@FromFormName", "GROUPSUMMARYFORM");

                    await connection.OpenAsync();

                    using (SqlDataAdapter dataAdapter = new SqlDataAdapter(command))
                    {
                        dataAdapter.Fill(oDataSet);
                    }
                }
                if (ReportType == "GROUPSUMMARY")
                {
                    if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                    {
                        resultList.AccGroupLedgerGrid = (from DataRow row in oDataSet.Tables[0].Rows
                                                       select new AccGroupLedgerModel
                                                       {
                                                           //TrailBalanceGroupName = row["TrailBalanceGroupName"] == DBNull.Value ? string.Empty : row["TrailBalanceGroupName"].ToString(),
                                                           //OpnDr = row["OpnDr"] == DBNull.Value ? 0 : Convert.ToDecimal(row["OpnDr"]),
                                                           //OpnCr = row["OpnCr"] == DBNull.Value ? 0 : Convert.ToDecimal(row["OpnCr"]),
                                                           //TotalOpening = row["TotalOpening"] == DBNull.Value ? 0 : Convert.ToDecimal(row["TotalOpening"]),
                                                           //CurrDrAmt = row["CurrDrAmt"] == DBNull.Value ? 0 : Convert.ToDecimal(row["CurrDrAmt"]),
                                                           //CurrCrAmt = row["CurrCrAmt"] == DBNull.Value ? 0 : Convert.ToDecimal(row["CurrCrAmt"]),
                                                           //NetCurrentAmt = row["NetCurrentAmt"] == DBNull.Value ? 0 : Convert.ToDecimal(row["NetCurrentAmt"]),
                                                           //NetAmt = row["NetAmount"] == DBNull.Value ? 0 : Convert.ToDecimal(row["NetAmount"]),
                                                           //CurrDrTotal = row["CurrDrTotal"] == DBNull.Value ? 0 : Convert.ToDecimal(row["CurrDrTotal"]),
                                                           //CurrCrTotal = row["CurrCrTotal"] == DBNull.Value ? 0 : Convert.ToDecimal(row["CurrCrTotal"]),
                                                           //TBSeq = row["TBSeq"] == DBNull.Value ? 0 : Convert.ToInt32(row["TBSeq"]),
                                                           //TrailBalanceGroupCode = row["TrailBalanceGroupCode"] == DBNull.Value ? 0 : Convert.ToInt32(row["TrailBalanceGroupCode"]),

                                                           ParentGroupName = row.IsNull("ParentGroupName") ? "" : Convert.ToString(row["ParentGroupName"]),
                                                           //ParentAccountCode = row.IsNull("ParentAccountCode") ? 0 : Convert.ToInt32(row["ParentAccountCode"]),
                                                           //SubGroupParent = row.IsNull("SubGroupParent") ? "" : Convert.ToString(row["SubGroupParent"]),
                                                           //UnderGroup = row.IsNull("UnderGroup") ? "" : Convert.ToString(row["UnderGroup"]),
                                                           AccountName = row.IsNull("AccountName") ? "" : Convert.ToString(row["AccountName"]),
                                                           //Account_Code = row.IsNull("Accountcode") ? 0 : Convert.ToInt32(row["Accountcode"]),
                                                           //GroupOpnDr = row.IsNull("GroupOpnDr") ? 0 : Convert.ToDecimal(row["GroupOpnDr"]),
                                                           //GroupOpnCr = row.IsNull("GroupOpnCr") ? 0 : Convert.ToDecimal(row["GroupOpnCr"]),
                                                           OpnDr = row.IsNull("OpnDr") ? 0 : Convert.ToDecimal(row["OpnDr"]),
                                                           OpnCr = row.IsNull("OpnCr") ? 0 : Convert.ToDecimal(row["OpnCr"]),
                                                           //TotalGroupOpening = row.IsNull("TotalGroupOpening") ? 0 : Convert.ToDecimal(row["TotalGroupOpening"]),
                                                           TotalOpening = row.IsNull("TotalOpening") ? 0 : Convert.ToDecimal(row["TotalOpening"]),
                                                           CurrDrAmt = row.IsNull("CurrDrAmt") ? 0 : Convert.ToDecimal(row["CurrDrAmt"]),
                                                           CurrCrAmt = row.IsNull("CurrCrAmt") ? 0 : Convert.ToDecimal(row["CurrCrAmt"]),
                                                           //GroupCurrDrAmt = row.IsNull("GroupCurrDrAmt") ? 0 : Convert.ToDecimal(row["GroupCurrDrAmt"]),
                                                           //GroupCurrCrAmt = row.IsNull("GroupCurrCrAmt") ? 0 : Convert.ToDecimal(row["GroupCurrCrAmt"]),
                                                           NetAmt = row.IsNull("NetAmount") ? 0 : Convert.ToDecimal(row["NetAmount"]),
                                                           NetCurrentAmt = row.IsNull("NetCurrentAmt") ? 0 : Convert.ToDecimal(row["NetCurrentAmt"]),
                                                           //SeqNo = row.IsNull("SeqNo") ? 0 : Convert.ToInt32(row["SeqNo"]),
                                                           //TrailBalanceGroupId = row.IsNull("trailbalanceGroupid") ? 0 : Convert.ToInt32(row["trailbalanceGroupid"]),
                                                           TBSeq = row.IsNull("TBSeq") ? 0 : Convert.ToInt32(row["TBSeq"]),
                                                           //TrailBalanceGroupCode = row.IsNull("TrailBalanceGroupCode") ? 0 : Convert.ToInt32(row["TrailBalanceGroupCode"]),
                                                           GroupLedger = row.IsNull("GroupLedger") ? "" : Convert.ToString(row["GroupLedger"])


                                                       }).ToList();
                    }
                }
                else
                {
                    if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                    {
                        resultList.AccGroupLedgerGrid = (from DataRow row in oDataSet.Tables[0].Rows
                                                       select new AccGroupLedgerModel
                                                       {
                                                           //TrailBalanceGroupName = row.IsNull("GroupName") ? "" : Convert.ToString(row["GroupName"]),
                                                           ParentGroupName = row.IsNull("ParentGroupName") ? "" : Convert.ToString(row["ParentGroupName"]),
                                                           //ParentAccountCode = row.IsNull("ParentAccountCode") ? 0 : Convert.ToInt32(row["ParentAccountCode"]),
                                                           //SubGroupParent = row.IsNull("SubGroupParent") ? "" : Convert.ToString(row["SubGroupParent"]),
                                                           //UnderGroup = row.IsNull("UnderGroup") ? "" : Convert.ToString(row["UnderGroup"]),
                                                           AccountName = row.IsNull("AccountName") ? "" : Convert.ToString(row["AccountName"]),
                                                           //Account_Code = row.IsNull("Accountcode") ? 0 : Convert.ToInt32(row["Accountcode"]),
                                                           //GroupOpnDr = row.IsNull("GroupOpnDr") ? 0 : Convert.ToDecimal(row["GroupOpnDr"]),
                                                           //GroupOpnCr = row.IsNull("GroupOpnCr") ? 0 : Convert.ToDecimal(row["GroupOpnCr"]),
                                                           OpnDr = row.IsNull("OpnDr") ? 0 : Convert.ToDecimal(row["OpnDr"]),
                                                           OpnCr = row.IsNull("OpnCr") ? 0 : Convert.ToDecimal(row["OpnCr"]),
                                                           //TotalGroupOpening = row.IsNull("TotalGroupOpening") ? 0 : Convert.ToDecimal(row["TotalGroupOpening"]),
                                                           TotalOpening = row.IsNull("TotalOpening") ? 0 : Convert.ToDecimal(row["TotalOpening"]),
                                                           CurrDrAmt = row.IsNull("CurrDrAmt") ? 0 : Convert.ToDecimal(row["CurrDrAmt"]),
                                                           CurrCrAmt = row.IsNull("CurrCrAmt") ? 0 : Convert.ToDecimal(row["CurrCrAmt"]),
                                                           //GroupCurrDrAmt = row.IsNull("GroupCurrDrAmt") ? 0 : Convert.ToDecimal(row["GroupCurrDrAmt"]),
                                                           //GroupCurrCrAmt = row.IsNull("GroupCurrCrAmt") ? 0 : Convert.ToDecimal(row["GroupCurrCrAmt"]),
                                                           NetAmt = row.IsNull("NetAmount") ? 0 : Convert.ToDecimal(row["NetAmount"]),
                                                           NetCurrentAmt = row.IsNull("NetCurrentAmt") ? 0 : Convert.ToDecimal(row["NetCurrentAmt"]),
                                                           //SeqNo = row.IsNull("SeqNo") ? 0 : Convert.ToInt32(row["SeqNo"]),
                                                           //TrailBalanceGroupId = row.IsNull("trailbalanceGroupid") ? 0 : Convert.ToInt32(row["trailbalanceGroupid"]),
                                                           TBSeq = row.IsNull("TBSeq") ? 0 : Convert.ToInt32(row["TBSeq"]),
                                                           //TrailBalanceGroupCode = row.IsNull("TrailBalanceGroupCode") ? 0 : Convert.ToInt32(row["TrailBalanceGroupCode"]),
                                                           GroupLedger = row.IsNull("GroupLedger") ? "" : Convert.ToString(row["GroupLedger"])

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
