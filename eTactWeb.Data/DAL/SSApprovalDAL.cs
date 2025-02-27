using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;
using static eTactWeb.Data.Common.CommonFunc;

namespace eTactWeb.Data.DAL
{
    public class SSApprovalDAL
    {
        private readonly IDataLogic _IDataLogic;
        private readonly string DBConnectionString = string.Empty;
        private IDataReader? Reader;


        public SSApprovalDAL(IConfiguration configuration, IDataLogic iDataLogic)
        {
            _IDataLogic = iDataLogic;
            DBConnectionString = configuration.GetConnectionString("eTactDB");
        }
        public async Task<ResponseResult> GetSSData(string Flag, string FromDate, string ToDate, string ApprovalType, string SONO, string SchNo, string VendorName, int Eid, string uid)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                //DateTime fromdt = DateTime.ParseExact(FromDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                DateTime fromdt = DateTime.Parse( ConvertToDesiredFormat (FromDate));
                DateTime todt = DateTime.Parse(ConvertToDesiredFormat(ToDate));
               // DateTime todt = DateTime.ParseExact(ToDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@Flag", Flag));
                SqlParams.Add(new SqlParameter("@FromDate", fromdt.ToString("yyyy/MM/dd")));
                SqlParams.Add(new SqlParameter("@ToDate", todt.ToString("yyyy/MM/dd")));
                SqlParams.Add(new SqlParameter("@EmpId", Eid));
                SqlParams.Add(new SqlParameter("@UId", uid));
                SqlParams.Add(new SqlParameter("@AccountName", VendorName));
                SqlParams.Add(new SqlParameter("@SONO", SONO));
                SqlParams.Add(new SqlParameter("@SchNo", SchNo));


                _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_ApproveUnapproveSaleSchedule", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }
        public async Task<List<SSApprovalDetail>> ShowSSDetail(int ID, int YC, string SchNo)
        {
            var oDataSet = new DataSet();
            var SqlParams = new List<dynamic>();

            var MainModel = new List<SSApprovalDetail>();


            try
            {
                SqlParams.Add(new SqlParameter("@Flag", "SHOWSSDETAIL"));
                SqlParams.Add(new SqlParameter("@SSEntryId", ID));
                SqlParams.Add(new SqlParameter("@SSyearcode", YC));
                SqlParams.Add(new SqlParameter("@SchNo", SchNo));

                var ResponseResult = await _IDataLogic.ExecuteDataSet("SP_ApproveUnapproveSaleSchedule", SqlParams);

                if (((DataSet)ResponseResult.Result).Tables.Count > 0 && ((DataSet)ResponseResult.Result).Tables[0].Rows.Count > 0)
                {
                    oDataSet = ResponseResult.Result;
                    oDataSet.Tables[0].TableName = "SSAppMain";

                    if (oDataSet.Tables.Count != 0 && oDataSet.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow row in oDataSet.Tables[0].Rows)
                        {

                            var ssApprovalDetail = CommonFunc.DataRowToClass<SSApprovalDetail>(row);

                            MainModel.Add(ssApprovalDetail);
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
        public async Task<ResponseResult> SaveApproval(int EntryId, int YC, string SchNo, string type, int EmpID)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                if (type == "UnApproval")
                {
                    SqlParams.Add(new SqlParameter("@Flag", "UnApproveSS"));

                }
                if (type == "Approval")
                {
                    SqlParams.Add(new SqlParameter("@Flag", "ApproveSS"));

                }
                if (type == "Unapproving Amendment")
                {
                    SqlParams.Add(new SqlParameter("@Flag", "AmendmentUnApproval"));

                }
                if (type == "Approving Amendment")
                {
                    SqlParams.Add(new SqlParameter("@Flag", "AmendmentApproval"));

                }
                SqlParams.Add(new SqlParameter("@SSEntryId", EntryId));
                SqlParams.Add(new SqlParameter("@SSyearcode", YC));
                SqlParams.Add(new SqlParameter("@SchNo", SchNo));
                SqlParams.Add(new SqlParameter("@ApprovalDate", DateTime.Today));
                SqlParams.Add(new SqlParameter("@Approvedby", EmpID));


                _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_ApproveUnapproveSaleSchedule", SqlParams);
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
