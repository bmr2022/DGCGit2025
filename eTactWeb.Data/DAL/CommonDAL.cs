using eTactWeb.Services.Interface;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;
using static eTactWeb.Data.Common.CommonFunc;
using eTactWeb.Data.Common;

namespace eTactWeb.Data.DAL
{
    public class CommonDAL
    {
        private readonly string DBConnectionString = string.Empty;
        private readonly IDataLogic _IDataLogic;
        private IDataReader? Reader;
        private readonly ConnectionStringService _connectionStringService;

        public CommonDAL(IConfiguration configuration, IDataLogic iDataLogic, ConnectionStringService connectionStringService)
        {
            //configuration = config;
            _connectionStringService = connectionStringService;
            DBConnectionString = _connectionStringService.GetConnectionString();
            //DBConnectionString = configuration.GetConnectionString("eTactDB");
            _IDataLogic = iDataLogic;
        }
        public async Task<ResponseResult> CheckFinYearBeforeSave(int YearCode, string Date, string DateName)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@YearCode", YearCode));
                SqlParams.Add(new SqlParameter("@Date1", ParseFormattedDate(Date)));
                SqlParams.Add(new SqlParameter("@Date1Name", ""));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("ChkDateFallInFinyear", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }

        public async Task<ResponseResult> FillReportTypes(string TableName, string MainReportType)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@RelatedTable", TableName));
                SqlParams.Add(new SqlParameter("@MainReportType", string.IsNullOrEmpty(MainReportType)
            ? (object)DBNull.Value
            : MainReportType));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_GetReportTypes", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }

        public async Task<ResponseResult> GetDashboardData(
            string spName,
            string flag,
            Dictionary<string, object> parameters)
        {
            var response = new ResponseResult();

            try
            {
                IList<SqlParameter> sqlParams = new List<SqlParameter>
        {
            new SqlParameter("@Flag", flag)
        };

                foreach (var p in parameters)
                {
                    sqlParams.Add(new SqlParameter(p.Key, p.Value ?? DBNull.Value));
                }

                // 🔑 Convert ONLY here
                IList<dynamic> dynamicParams = sqlParams.Cast<dynamic>().ToList();

                response = await _IDataLogic.ExecuteDataTable(spName, dynamicParams);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }

            return response;
        }

        public async Task<ResponseResult> GetUnit(int ItemCode)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "Unit"));
                SqlParams.Add(new SqlParameter("@ItemCode", ItemCode));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_CommonData", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }

        public async Task<ResponseResult> CheckRoundOff(string unit)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "CheckRoundOff"));
                SqlParams.Add(new SqlParameter("@Unit", unit));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_GetDropDownList", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> GetCredential()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@flag", "GetEinvoiceCredential"));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SPIRNEInvoiceAndEwayBillData", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> FillCurrentBatchINStore(int ItemCode, int YearCode, string FinStartDate, string StoreName, string batchno)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var Date = DateTime.Now;
                var finStDt = new DateTime();
                finStDt = Convert.ToDateTime(FinStartDate);
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@itemCode", ItemCode));
                SqlParams.Add(new SqlParameter("@Yearcode", YearCode));
                SqlParams.Add(new SqlParameter("@StorName", StoreName));
                SqlParams.Add(new SqlParameter("@FinStartDate", FinStartDate));
                SqlParams.Add(new SqlParameter("@transDate", Date));
                SqlParams.Add(new SqlParameter("@batchno", batchno));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("FillCurrentBatchINStore", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }
        public async Task<ResponseResult> GetBatchNumber(string SPName, int StoreId, string FinStartDate, string StoreName, int ItemCode, string TransDate, int YearCode, string BatchNo)
        {
            var Result = new ResponseResult();

            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@StorName", StoreName));
                SqlParams.Add(new SqlParameter("@itemCode", ItemCode));
                SqlParams.Add(new SqlParameter("@FinStartDate", FinStartDate));
                SqlParams.Add(new SqlParameter("@transDate", TransDate));
                SqlParams.Add(new SqlParameter("@Yearcode", YearCode));
                SqlParams.Add(new SqlParameter("@batchno", BatchNo));

                Result = await _IDataLogic.ExecuteDataTable(SPName, SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return Result;
        }
        public async Task<DataTable> GetInvoiceDetailToExportInXml(string FromDate, string ToDate, string InvoiceNo, int AccountCode, string InvoiceType)
        {
            var sqlParams = new List<dynamic>();
            sqlParams.Add(new SqlParameter("@FromDate", ParseFormattedDate(FromDate)));
            sqlParams.Add(new SqlParameter("@ToDate", ParseFormattedDate(ToDate)));
            sqlParams.Add(new SqlParameter("@InvoiceNo", InvoiceNo));
            sqlParams.Add(new SqlParameter("@AccountCode", AccountCode));
            sqlParams.Add(new SqlParameter("@InvoiceType", InvoiceType));
            var response = await _IDataLogic.ExecuteDataTable("GetInvoiceDetailToExportInXml", sqlParams);
            if (response.Result is DataTable dt)
            {
                return dt;
            }
            throw new Exception(response.Result?.ToString());
        }
        public async Task<ResponseResult> GetFeatureOptions()
        {
            var Result = new ResponseResult();

            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "GetFeaturesOption"));

                Result = await _IDataLogic.ExecuteDataTable("SP_SaleBillMainDetail", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return Result;
        }
        public async Task<ResponseResult> GetFormRights(int userId, int menuId)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "GetRights"));
                SqlParams.Add(new SqlParameter("@EmpId", userId));
                SqlParams.Add(new SqlParameter("@MenuId", menuId));
                _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_ItemGroup1", SqlParams);
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
