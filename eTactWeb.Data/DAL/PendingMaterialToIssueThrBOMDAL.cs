using eTactWeb.Data.Common;
using eTactWeb.Services.Interface;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Data.DAL
{
    public class PendingMaterialToIssueThrBOMDAL
    {
        private readonly IDataLogic _IDataLogic;
        private readonly string DBConnectionString = string.Empty;
        private IDataReader? Reader;
        private readonly ConnectionStringService _connectionStringService;
        //public static decimal BatchStockQty { get; private set; }

        public PendingMaterialToIssueThrBOMDAL(IConfiguration configuration, IDataLogic iDataLogic, ConnectionStringService connectionStringService)
        {
            _IDataLogic = iDataLogic;
            _connectionStringService = connectionStringService;
            DBConnectionString = _connectionStringService.GetConnectionString();
            //DBConnectionString = configuration.GetConnectionString("eTactDB");
        }
        public async Task<DataSet> BindAllDropDowns(string Flag, int YearCode)
        {
            var oDataSet = new DataSet();

            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", Flag));
                SqlParams.Add(new SqlParameter("@toDate", DateTime.Today));
                SqlParams.Add(new SqlParameter("@yearCode", DateTime.Now.Year));
                var _ResponseResult = await _IDataLogic.ExecuteDataSet("GetDataForRequitionThroughBOM", SqlParams);
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


                _ResponseResult = await _IDataLogic.ExecuteDataSet("GetDataForRequitionThroughBOM", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }

        public async Task<ResponseResult> ShowDetail(string FromDate, string ToDate, string ReqNo, int YearCode, int ItemCode, string WoNo, int WorkCenter, int DeptName, int ReqYear, string IssueDate, string GlobalSearch, string FromStore, int StoreId)
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
                SqlParams.Add(new SqlParameter("@ItemCode", ItemCode));
                SqlParams.Add(new SqlParameter("@WONO", WoNo == null ? "" : WoNo));
                SqlParams.Add(new SqlParameter("@ToWC", WorkCenter));
                SqlParams.Add(new SqlParameter("@ToDepartment", DeptName));
                SqlParams.Add(new SqlParameter("@reqYearcode", ReqYear));
                SqlParams.Add(new SqlParameter("@Issuedate", issDt));
                SqlParams.Add(new SqlParameter("@storeid", StoreId));

                _ResponseResult = await _IDataLogic.ExecuteDataSet("GetDataForRequitionThroughBOM", SqlParams);
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

                _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_IssueWithBOM", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }
        public static string ParseFormattedDate(string dateString)
        {
            if (string.IsNullOrEmpty(dateString))
            {
                return string.Empty;
            }
            DateTime date;
            string[] formats = { "yyyy-MM-dd", "dd-MM-yyyy", "dd/MM/yyyy", "dd/MM/yy", "MM/dd/yyyy", "MM/dd/yy", "dd-MM-yy", "d-M-yy", "dd/MMM/yyyy", "dd/MMM/yyyy HH:mm:ss", "HH:mm:ss", "" };

            if (DateTime.TryParseExact(dateString, formats, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out date))
            {
                return date.ToString("yyyy/MM/dd");
            }
            else
            {
                return string.Empty;
            }
        }
        public async Task<ResponseResult> CheckTransDate(int ItemCode, string IssueDate, string BatchNo, string UniqBatchNo, int YearCode)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                //DateTime transDate = DateTime.ParseExact(IssueDate, "yyyy/MM/dd", CultureInfo.InvariantCulture);
                var transDate = ParseFormattedDate(IssueDate);
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@TransDate", transDate));
                SqlParams.Add(new SqlParameter("@ItemCode", ItemCode));
                SqlParams.Add(new SqlParameter("@SToreIdWCID", 1));
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
    }
}
