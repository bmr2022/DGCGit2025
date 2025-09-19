using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Data.DAL
{
    public class POApprovalDAL
    {
        private readonly IDataLogic _IDataLogic;
        private readonly string DBConnectionString = string.Empty;
        private IDataReader? Reader;
        private readonly ConnectionStringService _connectionStringService;

        public POApprovalDAL(IConfiguration configuration, IDataLogic iDataLogic, ConnectionStringService connectionStringService)
        {
            _IDataLogic = iDataLogic;
            _connectionStringService = connectionStringService;
            DBConnectionString = _connectionStringService.GetConnectionString();
            //DBConnectionString = configuration.GetConnectionString("eTactDB");
        }

        public async Task<ResponseResult> GetInitialData(string Flag, string UId, int EmpId)
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
                SqlParams.Add(new SqlParameter("@StartDate", firstDayOfMonth));
                SqlParams.Add(new SqlParameter("@EndDate", DateTime.Today));


                _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_ApproveUnapprovePurchaseOrder", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }
        public async Task<ResponseResult> GetReportName()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "GetReportName"));

                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_PurchaseOrder", SqlParams);

            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        } 
        public async Task<ResponseResult> GetFeaturesOptions()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "GetFeaturesOptions"));

                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_ApproveUnapprovePurchaseOrder", SqlParams);

            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }
        public async Task<ResponseResult> GetMobileNo(int ID, int YearCode, string PoNo)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "GetwhatsappMobileNo"));
                SqlParams.Add(new SqlParameter("@PoNo", PoNo));
                SqlParams.Add(new SqlParameter("@POEntryId", ID));
                SqlParams.Add(new SqlParameter("@Poyearcode", YearCode));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_ApproveUnapprovePurchaseOrder", SqlParams);

            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }
        public async Task<ResponseResult> GetSearchData(string FromDate, string ToDate, string ApprovalType, string PONO, string VendorName,int Eid, string uid)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                DateTime fromdt = DateTime.ParseExact(FromDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                DateTime todt = DateTime.ParseExact(ToDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                var SqlParams = new List<dynamic>();
                string flag = "";
                if (ApprovalType == "firstLvlUnApp")
                    flag = "APPROVEDFirstlevelPO";
                if (ApprovalType == "firstLvlApproval")
                    flag = "UNAPPROVEDFIRSTLVLPOSUMM";
                if (ApprovalType == "amdUnApp")
                    flag = "APPROVEDAmendmentPO";
                if (ApprovalType == "amdApproval")
                    flag = "UNAPPROVEDAmendmentPOSUMM";
                if (ApprovalType == "finalUnApp")
                    flag = "APPROVEDFinallevelPO";
                if (ApprovalType == "finalApproval")
                    flag = "UNAPPROVEDFINALAPPPOSUMM";

                SqlParams.Add(new SqlParameter("@Flag", flag));
                SqlParams.Add(new SqlParameter("@StartDate", fromdt.ToString("yyyy/MM/dd")));
                SqlParams.Add(new SqlParameter("@EndDate", todt.ToString("yyyy/MM/dd")));
                SqlParams.Add(new SqlParameter("@EmpId", Eid));
                SqlParams.Add(new SqlParameter("@UId", uid));
                //SqlParams.Add(new SqlParameter("@ApprovalType", ApprovalType));
                SqlParams.Add(new SqlParameter("@VendorName", VendorName));
                SqlParams.Add(new SqlParameter("@PONo", PONO));


                _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_ApproveUnapprovePurchaseOrder", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }
        public async Task<ResponseResult> GetAllowedAction(string Flag, int EmpId)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", Flag));
                SqlParams.Add(new SqlParameter("@FormName", "PurchaseOrder"));
                SqlParams.Add(new SqlParameter("@EmpId", EmpId));


                _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_ApproveUnapprovePurchaseOrder", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }
        public async Task<ResponseResult> SaveApproval(int EntryId, int YC, string PONO, string type, int EmpID)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                if(type== "First Level Approval")
                {
                    SqlParams.Add(new SqlParameter("@Flag", "ApproveFirstLevel"));

                }if(type== "Approving Final Level")
                {
                    SqlParams.Add(new SqlParameter("@Flag", "ApproveFinalLevel"));

                }if(type== "Approving Amendment Level")
                {
                    SqlParams.Add(new SqlParameter("@Flag", "AmendmentApproval"));

                }if(type== "Unapproving Final Level")
                {
                    SqlParams.Add(new SqlParameter("@Flag", "UNApproveFinalLevel"));

                }if(type== "Unapproving first Level")
                {
                    SqlParams.Add(new SqlParameter("@Flag", "UNApproveFirstLevel"));

                }if(type== "Unapproving 20Amendment Level")
                {
                    SqlParams.Add(new SqlParameter("@Flag", "UNApproveFirstLevel"));

                }
                SqlParams.Add(new SqlParameter("@POEntryId", EntryId));
                SqlParams.Add(new SqlParameter("@Poyearcode", YC));
                SqlParams.Add(new SqlParameter("@PoNo", PONO));
                SqlParams.Add(new SqlParameter("@ApprovalDate", DateTime.Today));
                SqlParams.Add(new SqlParameter("@Approvedby", EmpID));


                _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_ApproveUnapprovePurchaseOrder", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }

        public async Task<List<POApprovalDetail>> ShowPODetail(int ID, int YC, string PONo, string TypeOfApproval,string showonlyamenditem)
        {
            var oDataSet = new DataSet();
            var SqlParams = new List<dynamic>();

            var MainModel = new List<POApprovalDetail>();
            

            try
            {
                SqlParams.Add(new SqlParameter("@Flag", "SHOWPODETAIL"));
                SqlParams.Add(new SqlParameter("@POEntryId", ID));
                SqlParams.Add(new SqlParameter("@Poyearcode", YC));
                SqlParams.Add(new SqlParameter("@PoNo", PONo));
                SqlParams.Add(new SqlParameter("@showonlyamenditem", showonlyamenditem));

                var ResponseResult = await _IDataLogic.ExecuteDataSet("SP_ApproveUnapprovePurchaseOrder", SqlParams);

                if (((DataSet)ResponseResult.Result).Tables.Count > 0 && ((DataSet)ResponseResult.Result).Tables[0].Rows.Count > 0)
                {
                    oDataSet = ResponseResult.Result;
                    oDataSet.Tables[0].TableName = "POAppMain";

                    if (oDataSet.Tables.Count != 0 && oDataSet.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow row in oDataSet.Tables[0].Rows)
                        {

                            var poApprovalDetail = CommonFunc.DataRowToClass<POApprovalDetail>(row);

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
    }
}
