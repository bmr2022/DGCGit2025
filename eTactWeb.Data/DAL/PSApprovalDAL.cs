using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Data.DAL
{
    public class PSApprovalDAL
    {
        private readonly IDataLogic _IDataLogic;
        private readonly string DBConnectionString = string.Empty;
        private IDataReader? Reader;


        public PSApprovalDAL(IConfiguration configuration, IDataLogic iDataLogic)
        {
            _IDataLogic = iDataLogic;
            DBConnectionString = configuration.GetConnectionString("eTactDB");
        }
        public async Task<ResponseResult> GetPSData(string Flag,string FromDate, string ToDate, string ApprovalType, string PONO,string SchNo, string VendorName, int Eid, string uid)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                DateTime fromdt = DateTime.ParseExact(FromDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                DateTime todt = DateTime.ParseExact(ToDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@Flag", Flag));
                SqlParams.Add(new SqlParameter("@FromDate", fromdt.ToString("yyyy/MM/dd")));
                SqlParams.Add(new SqlParameter("@ToDate", todt.ToString("yyyy/MM/dd")));
                SqlParams.Add(new SqlParameter("@EmpId", Eid));
                SqlParams.Add(new SqlParameter("@UId", uid));
                SqlParams.Add(new SqlParameter("@AccountName", VendorName));
                SqlParams.Add(new SqlParameter("@PONo", PONO));
                SqlParams.Add(new SqlParameter("@SchNo", SchNo));


                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_ApproveUnapprovePurchaseSchedule", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }

        public async Task<List<PSApprovalDetail>> ShowPSDetail(int ID, int YC, string SchNo)
        {
            var oDataSet = new DataSet();
            var SqlParams = new List<dynamic>();

            var MainModel = new List<PSApprovalDetail>();


            try
            {
                SqlParams.Add(new SqlParameter("@Flag", "SHOWPSDETAIL"));
                SqlParams.Add(new SqlParameter("@PSEntryId", ID));
                SqlParams.Add(new SqlParameter("@PSyearcode", YC));
                SqlParams.Add(new SqlParameter("@SchNo", SchNo));

                var ResponseResult = await _IDataLogic.ExecuteDataSet("SP_ApproveUnapprovePurchaseSchedule", SqlParams);

                if (((DataSet)ResponseResult.Result).Tables.Count > 0 && ((DataSet)ResponseResult.Result).Tables[0].Rows.Count > 0)
                {
                    oDataSet = ResponseResult.Result;
                    oDataSet.Tables[0].TableName = "PSAppMain";

                    if (oDataSet.Tables.Count != 0 && oDataSet.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow row in oDataSet.Tables[0].Rows)
                        {

                            var poApprovalDetail = CommonFunc.DataRowToClass<PSApprovalDetail>(row);

                            MainModel.Add(poApprovalDetail);
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
                    SqlParams.Add(new SqlParameter("@Flag", "UnApprovePS"));

                }
                if (type == "Approval")
                {
                    SqlParams.Add(new SqlParameter("@Flag", "ApprovePS"));

                }
                if (type == "Unapproving Amendment")
                {
                    SqlParams.Add(new SqlParameter("@Flag", "AmendmentUnApproval"));

                }
                if (type == "Approving Amendment")
                {
                    SqlParams.Add(new SqlParameter("@Flag", "AmendmentApproval"));

                }
                SqlParams.Add(new SqlParameter("@PSEntryId", EntryId));
                SqlParams.Add(new SqlParameter("@PSyearcode", YC));
                SqlParams.Add(new SqlParameter("@SchNo", SchNo));
                SqlParams.Add(new SqlParameter("@ApprovalDate", DateTime.Today));
                SqlParams.Add(new SqlParameter("@Approvedby", EmpID));


                _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_ApproveUnapprovePurchaseSchedule", SqlParams);
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
