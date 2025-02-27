using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;
using static eTactWeb.Data.Common.CommonFunc;

namespace eTactWeb.Data.DAL
{
    public class SOApprovalDAL
    {
        private readonly IDataLogic _IDataLogic;
        private readonly string DBConnectionString = string.Empty;
        private IDataReader? Reader;


        public SOApprovalDAL(IConfiguration configuration, IDataLogic iDataLogic)
        {
            _IDataLogic = iDataLogic;
            DBConnectionString = configuration.GetConnectionString("eTactDB");
        }

        public async Task<ResponseResult> GetProcData(string Flag, string UId, int EmpId, string SONO, string AccountName)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                DateTime now = DateTime.Now;

                DateTime firstDayOfMonth = new DateTime(now.Year, now.Month, 1);

                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", Flag));
                SqlParams.Add(new SqlParameter("@EmpId", EmpId));
                SqlParams.Add(new SqlParameter("@UId", UId));
                SqlParams.Add(new SqlParameter("@SONO", SONO));
                SqlParams.Add(new SqlParameter("@AccountName", AccountName));
                SqlParams.Add(new SqlParameter("@Fromdate", firstDayOfMonth));
                SqlParams.Add(new SqlParameter("@ToDate", DateTime.Today));


                _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_ApproveUnapproveSaleOrder", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }
        public async Task<ResponseResult> GetSearchData(string FromDate, string ToDate, string ApprovalType, string Uid, int EmpId, string SONO, string AccountName, string CustOrderNo)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                //DateTime fromdt = DateTime.ParseExact(FromDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                //DateTime todt = DateTime.ParseExact(ToDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                String fromdt = ConvertToDesiredFormat(FromDate);
                String todt = ConvertToDesiredFormat(ToDate);
                var SqlParams = new List<dynamic>();
                if(ApprovalType == "UnAppSO")
                SqlParams.Add(new SqlParameter("@Flag", "UNAPPROVEDSOSUMM"));
                if(ApprovalType=="AppSO")
                SqlParams.Add(new SqlParameter("@Flag", "APPROVEDSOSUMM"));
                if(ApprovalType== "AppAmmSO")
                SqlParams.Add(new SqlParameter("@Flag", "UNAPPROVEDAMMSOSUMM"));
                if(ApprovalType== "UnAppAmmSO")
                SqlParams.Add(new SqlParameter("@Flag", "APPROVEDAMMSOSUMM"));
                SqlParams.Add(new SqlParameter("@EmpId", EmpId));
                SqlParams.Add(new SqlParameter("@UId", Uid));
                SqlParams.Add(new SqlParameter("@SONO", SONO));
                SqlParams.Add(new SqlParameter("@AccountName", AccountName));
                SqlParams.Add(new SqlParameter("@CustOrderNo", CustOrderNo));
                SqlParams.Add(new SqlParameter("@Fromdate", fromdt));
                SqlParams.Add(new SqlParameter("@ToDate", todt));


                _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_ApproveUnapproveSaleOrder", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }

        public async Task<List<SoApprovalDetail>> ShowSODetail(int ID, int YC, string SONo)
        {
            var oDataSet = new DataSet();
            var SqlParams = new List<dynamic>();

            var MainModel = new List<SoApprovalDetail>();


            try
            {
                SqlParams.Add(new SqlParameter("@Flag", "SHOWSODETAIL"));
                SqlParams.Add(new SqlParameter("@SOEntryId", ID));
                SqlParams.Add(new SqlParameter("@Soyearcode", YC));
                SqlParams.Add(new SqlParameter("@SoNo", SONo));

                var ResponseResult = await _IDataLogic.ExecuteDataSet("SP_ApproveUnapproveSaleOrder", SqlParams);

                if (((DataSet)ResponseResult.Result).Tables.Count > 0 && ((DataSet)ResponseResult.Result).Tables[0].Rows.Count > 0)
                {
                    oDataSet = ResponseResult.Result;
                    oDataSet.Tables[0].TableName = "SOAppMain";

                    if (oDataSet.Tables.Count != 0 && oDataSet.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow row in oDataSet.Tables[0].Rows)
                        {
                            MainModel.Add(new SoApprovalDetail
                            {
                                SONO = row["SONO"].ToString(),
                                SODate = string.IsNullOrEmpty(row["SODate"].ToString()) ? "" : row["SODate"].ToString(),
                                WEF = string.IsNullOrEmpty(row["WEF"].ToString()) ? "" : row["WEF"].ToString(),
                                OrderType = row["OrderType"].ToString(),
                                SOType = row["SOType"].ToString(),
                                SOFor = row["SOFor"].ToString(),
                                CustomerName = row["CustomerName"].ToString(),
                                ItemName = row["ItemName"].ToString(),
                                PartCode = row["PartCode"].ToString(),
                                HSNNo = Convert.ToInt32(row["HSNNo"]),
                                Qty = Convert.ToDecimal(row["Qty"]),
                                Unit = row["Unit"].ToString(),
                                AltQty = Convert.ToDecimal(row["AltQty"]),
                                AltUnit = row["AltUnit"].ToString(),
                                Rate = Convert.ToDecimal(row["Rate"]),
                                UnitRate = row["UnitRate"].ToString(),
                                Remark = string.IsNullOrEmpty(row["Remark"].ToString()) ? "" : row["Remark"].ToString(),
                                PendQty = row["PendQty"] == DBNull.Value ? 0 : Convert.ToDecimal(row["PendQty"]),
                                PendAltQty = row["PendAltQty"] == DBNull.Value ? 0 : Convert.ToDecimal(row["PendAltQty"]),
                                EntryID = Convert.ToInt32(row["EntryID"]),
                                Year = Convert.ToInt32(row["Year"]),                                
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

        public async Task<ResponseResult> SaveApproval(int EntryId, int YC, string SONO, string CustOrderNo, string type, int EmpID)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                if (type == "SO Approval")
                {
                    SqlParams.Add(new SqlParameter("@Flag", "ApproveSO"));

                }
                if (type == "SO Unapproval")
                {
                    SqlParams.Add(new SqlParameter("@Flag", "UNApproveSO"));
                    SqlParams.Add(new SqlParameter("@custOrderNo", CustOrderNo));

                }
                if (type == "SO Amendment Approval")
                {
                    SqlParams.Add(new SqlParameter("@Flag", "AmendmentApproval"));

                }
                if (type == "SO Amendment Unapproval")
                {
                    SqlParams.Add(new SqlParameter("@Flag", "AmendmentUnApproval"));

                }
                
                SqlParams.Add(new SqlParameter("@SOEntryId", EntryId));
                SqlParams.Add(new SqlParameter("@Soyearcode", YC));
                SqlParams.Add(new SqlParameter("@SoNo", SONO));
                SqlParams.Add(new SqlParameter("@ApprovalDate", DateTime.Today));
                SqlParams.Add(new SqlParameter("@Approvedby", EmpID));


                _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_ApproveUnapproveSaleOrder", SqlParams);
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
