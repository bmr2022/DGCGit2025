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
    public class CloseJobWorkChallanDAL
    {
        private readonly IDataLogic _IDataLogic;
        private readonly string DBConnectionString = string.Empty;
        private IDataReader? Reader;
        private readonly ConnectionStringService _connectionStringService;

        public CloseJobWorkChallanDAL(IConfiguration configuration, IDataLogic iDataLogic, ConnectionStringService connectionStringService)
        {
            _IDataLogic = iDataLogic;
            _connectionStringService = connectionStringService;
            DBConnectionString = _connectionStringService.GetConnectionString();
            //DBConnectionString = configuration.GetConnectionString("eTactDB");
        }


        public async Task<ResponseResult> GetSearchData(string FromDate, string ToDate, int AccountCode, string ChallanNO,string ShowClsoedPendingAll)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                //DateTime fromdt = DateTime.ParseExact(FromDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                //DateTime todt = DateTime.ParseExact(ToDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                String fromdt = CommonFunc.ParseFormattedDate(FromDate);
                String todt =CommonFunc.ParseFormattedDate(ToDate);
                var SqlParams = new List<dynamic>();
                //if (CancelType == "LISTOFActiveSO")
                //    SqlParams.Add(new SqlParameter("@Flag", "LISTOFActiveSO"));
                //if (CancelType == "LISTOFDeActiveSO")
                //    SqlParams.Add(new SqlParameter("@Flag", "LISTOFDeActiveSO"));

                SqlParams.Add(new SqlParameter("@Flag", "FillChallan"));
                SqlParams.Add(new SqlParameter("@VendCustomerJW", "vendor"));
                SqlParams.Add(new SqlParameter("@SummeryDetail", "SUMMARY"));


                SqlParams.Add(new SqlParameter("@ShowClsoedPendingAll", ShowClsoedPendingAll));
              
                SqlParams.Add(new SqlParameter("@AccountCode", AccountCode));
                SqlParams.Add(new SqlParameter("@JWChallanNo", ChallanNO));
                SqlParams.Add(new SqlParameter("@Fromdate", fromdt));
                SqlParams.Add(new SqlParameter("@ToDate", todt));


                _ResponseResult = await _IDataLogic.ExecuteDataSet("SPCloseJobworkChallanManual", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }

        public async Task<List<CloseJobWorkChallanModel>> ShowDetail(int ID, int YC)
        {
            var oDataSet = new DataSet();
            var SqlParams = new List<dynamic>();

            var MainModel = new List<CloseJobWorkChallanModel>();


            try
            {
                SqlParams.Add(new SqlParameter("@Flag", "FillChallan"));
                SqlParams.Add(new SqlParameter("@VendCustomerJW", "vendor"));
                SqlParams.Add(new SqlParameter("@SummeryDetail", "Detail"));
                SqlParams.Add(new SqlParameter("@JWCloseEntryId", ID));
                //SqlParams.Add(new SqlParameter("@Soyearcode", YC));
                //SqlParams.Add(new SqlParameter("@SoNo", SONo));

                var ResponseResult = await _IDataLogic.ExecuteDataSet("SPCloseJobworkChallanManual", SqlParams);

                if (((DataSet)ResponseResult.Result).Tables.Count > 0 && ((DataSet)ResponseResult.Result).Tables[0].Rows.Count > 0)
                {
                    oDataSet = ResponseResult.Result;
                    oDataSet.Tables[0].TableName = "SOAppMain";

                    if (oDataSet.Tables.Count != 0 && oDataSet.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow row in oDataSet.Tables[0].Rows)
                        {
                            MainModel.Add(new CloseJobWorkChallanModel
                            {
                                VendJWIssChallanNo = row["VendJWIssChallanNo"].ToString(),
                                VendJWIssChallanDate = string.IsNullOrEmpty(row["VendJWIssChallanDate"].ToString())
                            ? ""
                            : row["VendJWIssChallanDate"].ToString(),

                                TolApprVal = row["TolApprVal"] == DBNull.Value ? 0 : Convert.ToDecimal(row["TolApprVal"]),
                                Closed = row["Closed"].ToString(),
                                TotalAmount = row["TotalAmount"] == DBNull.Value ? 0 : Convert.ToDecimal(row["TotalAmount"]),

                                ItemCode = row["ItemCode"] == DBNull.Value ? 0 : Convert.ToInt32(row["ItemCode"]),
                                Rate = row["Rate"] == DBNull.Value ? 0 : Convert.ToDecimal(row["Rate"]),
                                Amount = row["Amount"] == DBNull.Value ? 0 : Convert.ToDecimal(row["Amount"]),
                                IssQty = row["issQty"] == DBNull.Value ? 0 : Convert.ToDecimal(row["issQty"]),
                                //ItemClosed = row["Closed"].ToString(),  // careful: jd.Closed also exists (alias if needed)

                                PendQty = row["pendqty"] == DBNull.Value ? 0 : Convert.ToDecimal(row["pendqty"]),
                                PendAltQty = row["PendAltQty"] == DBNull.Value ? 0 : Convert.ToDecimal(row["PendAltQty"]),

                                VendJWIssEntryId = row["VendJWIssEntryId"] == DBNull.Value ? 0 : Convert.ToInt32(row["VendJWIssEntryId"]),
                                VendJWIssYearCode = row["VendJWIssYearCode"] == DBNull.Value ? 0 : Convert.ToInt32(row["VendJWIssYearCode"])
                            });
                        }


                    }


                }
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            finally
            {
                oDataSet.Dispose();
            }
            return MainModel;
        }


    }
}
