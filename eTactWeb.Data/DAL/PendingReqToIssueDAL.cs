using eTactWeb.Services.Interface;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace eTactWeb.Data.DAL
{
    public class PendingReqToIssueDAL
    {
        private readonly IDataLogic _IDataLogic;
        private readonly string DBConnectionString = string.Empty;
        private IDataReader? Reader;

        //public static decimal BatchStockQty { get; private set; }

        public PendingReqToIssueDAL(IConfiguration configuration, IDataLogic iDataLogic)
        {
            _IDataLogic = iDataLogic;
            DBConnectionString = configuration.GetConnectionString("eTactDB");
        }

        public async Task<DataSet> BindAllDropDowns(string Flag,int YearCode)
        {
            var oDataSet = new DataSet();

            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", Flag));
                SqlParams.Add(new SqlParameter("@toDate", DateTime.Today));
                SqlParams.Add(new SqlParameter("@yearCode", YearCode));
                var _ResponseResult = await _IDataLogic.ExecuteDataSet("GetDataForRequitionwithoutBOM", SqlParams);
                if (_ResponseResult.Result != null && _ResponseResult.StatusCode == HttpStatusCode.OK && _ResponseResult.StatusText == "Success")
                {
                    _ResponseResult.Result.Tables[0].TableName = "ReqList";

                    oDataSet = _ResponseResult.Result;
                }
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return oDataSet;
        }

        public async Task<ResponseResult> FillItemCode(string reqNo, int workCenter, int deptName, int yearCode, string todate)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                DateTime todt = DateTime.ParseExact(todate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "ITEM"));
                SqlParams.Add(new SqlParameter("@yearCode", yearCode));
                SqlParams.Add(new SqlParameter("@toDate", todt.ToString("yyyy/MM/dd")));
                SqlParams.Add(new SqlParameter("@ToDepartment", deptName));
                SqlParams.Add(new SqlParameter("@ToWC", workCenter));
                SqlParams.Add(new SqlParameter("@reqno", reqNo));


                _ResponseResult = await _IDataLogic.ExecuteDataSet("GetDataForRequitionwithoutBOM", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }

        public async Task<ResponseResult> FillWorkCenter(string reqNo, int itemcode, int deptName, int yearCode, string todate)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                DateTime todt = DateTime.ParseExact(todate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "ISSTOWC"));
                SqlParams.Add(new SqlParameter("@yearCode", yearCode));
                SqlParams.Add(new SqlParameter("@toDate", todt.ToString("yyyy/MM/dd")));
                SqlParams.Add(new SqlParameter("@ToDepartment", deptName));
                SqlParams.Add(new SqlParameter("@ItemCode", itemcode));
                SqlParams.Add(new SqlParameter("@reqno", reqNo));


                _ResponseResult = await _IDataLogic.ExecuteDataSet("GetDataForRequitionwithoutBOM", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }
        public async Task<ResponseResult> FillDeptName(string reqNo, int itemcode, int workCenter, int yearCode, string todate)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                DateTime todt = DateTime.ParseExact(todate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "ISSUETODEP"));
                SqlParams.Add(new SqlParameter("@yearCode", yearCode));
                SqlParams.Add(new SqlParameter("@toDate", todt.ToString("yyyy/MM/dd")));
                SqlParams.Add(new SqlParameter("@ItemCode", itemcode));
                SqlParams.Add(new SqlParameter("@reqno", reqNo));


                _ResponseResult = await _IDataLogic.ExecuteDataSet("GetDataForRequitionwithoutBOM", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }
        public async Task<ResponseResult> FillRequisition(int toDept, int itemcode, int workCenter, int yearCode, string todate)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                DateTime todt = DateTime.ParseExact(todate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "REQNO"));
                SqlParams.Add(new SqlParameter("@yearCode", yearCode));
                SqlParams.Add(new SqlParameter("@toDate", todt.ToString("yyyy/MM/dd")));
                SqlParams.Add(new SqlParameter("@ItemCode", itemcode));
                SqlParams.Add(new SqlParameter("@ToDepartment", toDept));


                _ResponseResult = await _IDataLogic.ExecuteDataSet("GetDataForRequitionwithoutBOM", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }

        public async Task<ResponseResult> ShowDetail(string FromDate, string ToDate, string ReqNo, int YearCode, int itemCode, string WoNo, int WorkCenter, int DeptName, int ReqYear, string IssueDate,string GlobalSearch, string FromStore, int StoreId)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                DateTime todt = DateTime.ParseExact(ToDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                DateTime fromdt = DateTime.ParseExact(FromDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                DateTime issDt = DateTime.ParseExact(IssueDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "ShowDetail"));
                SqlParams.Add(new SqlParameter("@fromDate", fromdt.ToString("yyyy/MM/dd")));
                SqlParams.Add(new SqlParameter("@toDate", todt.ToString("yyyy/MM/dd")));
                SqlParams.Add(new SqlParameter("@reqno", ReqNo == null ? "" : ReqNo));
                SqlParams.Add(new SqlParameter("@yearCode", YearCode));
                SqlParams.Add(new SqlParameter("@ItemCode", itemCode));
                SqlParams.Add(new SqlParameter("@WONO", WoNo == null ? "" : WoNo));
                SqlParams.Add(new SqlParameter("@ToWC", WorkCenter));
                SqlParams.Add(new SqlParameter("@ToDepartment", DeptName));
                SqlParams.Add(new SqlParameter("@reqYearcode", ReqYear));
                SqlParams.Add(new SqlParameter("@Issuedate", issDt));
                SqlParams.Add(new SqlParameter("@storeid", StoreId));

                _ResponseResult = await _IDataLogic.ExecuteDataSet("GetDataForRequitionwithoutBOM", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }

        public async Task<ResponseResult> BindReqYear(int yCode, string todate)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
               // DateTime todt = DateTime.ParseExact(todate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "BinDReqYear"));
                SqlParams.Add(new SqlParameter("@yearCode", yCode));
                SqlParams.Add(new SqlParameter("@toDate", DateTime.Today));

                _ResponseResult = await _IDataLogic.ExecuteDataSet("GetDataForRequitionwithoutBOM", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }
        public async Task<ResponseResult> CheckTransDate(int ItemCode, string IssueDate,string BatchNo, string UniqBatchNo, int YearCode, int StoreId)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@TransDate", IssueDate));
                SqlParams.Add(new SqlParameter("@ItemCode", ItemCode));
                SqlParams.Add(new SqlParameter("@SToreIdWCID", StoreId));
                SqlParams.Add(new SqlParameter("@YearCode", YearCode));
                SqlParams.Add(new SqlParameter("@batchno", BatchNo));
                SqlParams.Add(new SqlParameter("@uniquebatchno", UniqBatchNo));

                _ResponseResult = await _IDataLogic.ExecuteDataSet("CheckLastTransDate", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }
        public async Task<ResponseResult> EnableOrDisableIssueDate()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "AllowBackIssueWOBOM"));

                _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_IssueWithoutBomM", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }
        public async Task<ResponseResult> GetAlternateItemCode(int MainIC)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "GetAltItemCode"));
                SqlParams.Add(new SqlParameter("@ItemCode", MainIC));

                _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_IssueWithoutBomM", SqlParams);
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
