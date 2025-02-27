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
    public class PendingMRNtoQcDAL
    {
        private readonly IDataLogic _IDataLogic;
        private readonly string DBConnectionString = string.Empty;
        private IDataReader? Reader;


        public PendingMRNtoQcDAL(IConfiguration configuration, IDataLogic iDataLogic)
        {
            _IDataLogic = iDataLogic;
            DBConnectionString = configuration.GetConnectionString("eTactDB");
        }

        public async Task<DataSet> BindData(string Flag)
        {
            var oDataSet = new DataSet();

            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", Flag));
                var _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_MRN", SqlParams);
                if (_ResponseResult.Result != null && _ResponseResult.StatusCode == HttpStatusCode.OK && _ResponseResult.StatusText == "Success")
                {
                    _ResponseResult.Result.Tables[0].TableName = "AccountList";
                    _ResponseResult.Result.Tables[1].TableName = "InvNoList";
                    _ResponseResult.Result.Tables[2].TableName = "MRNNoList";
                    _ResponseResult.Result.Tables[3].TableName = "ItemList";
                    _ResponseResult.Result.Tables[4].TableName = "DeptList";
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

        public async Task<ResponseResult> GetDataForPendingMRN(string Flag,string MRNJW, int YearCode, string FromDate, string ToDate, int AccountCode, string MrnNo, int ItemCode, string InvoiceNo, int DeptId)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                DateTime fromdt = DateTime.ParseExact(FromDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                DateTime todt = DateTime.ParseExact(ToDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", Flag));
                SqlParams.Add(new SqlParameter("@MRNJWCJ", MRNJW));
                SqlParams.Add(new SqlParameter("@yeacode", YearCode));
                SqlParams.Add(new SqlParameter("@fromdate", fromdt.ToString("yyyy/MM/dd")));
                SqlParams.Add(new SqlParameter("@todate", todt.ToString("yyyy/MM/dd")));
                SqlParams.Add(new SqlParameter("@accountcode", AccountCode));
                SqlParams.Add(new SqlParameter("@MRNNO", MrnNo));
                SqlParams.Add(new SqlParameter("@ItemCode", ItemCode));
                SqlParams.Add(new SqlParameter("@invoiceNo", InvoiceNo));
                SqlParams.Add(new SqlParameter("@DeptId", DeptId));


                _ResponseResult = await _IDataLogic.ExecuteDataSet("GetPendingMRNFORQC", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }
        public async Task<ResponseResult> GetDeptForUser(int EmpId)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "GetDeptForUser"));
                SqlParams.Add(new SqlParameter("@EmpId", EmpId));

                _ResponseResult = await _IDataLogic.ExecuteDataSet("GetPendingMRNFORQC", SqlParams);
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
