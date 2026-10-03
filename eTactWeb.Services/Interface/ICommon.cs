using eTactWeb.DOM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Services.Interface
{
    public interface ICommon
    {
        Task<ResponseResult> CheckFinYearBeforeSave(int YearCode, string Date, string DateName);
        Task<ResponseResult> FillReportTypes(string TableName, string MainReportType);
        Task<ResponseResult> GetDashboardData(string spName, string flag, Dictionary<string, object> parameters);
        Task<ResponseResult> GetUnit(int ItemCode);
        Task<ResponseResult> CheckRoundOff(string unit);
        Task<AccessTokenResponse> GetAccessTokenAsync();
        List<Dictionary<string, object>> DataTableToList(DataTable dt);
        Task<ResponseResult> FillCurrentBatchINStore(int ItemCode, int YearCode, string FinStartDate, string StoreName, string batchno);
        Task<ResponseResult> GetBatchNumber(string SPName, int StoreId, string StoreName, string FinStartDate, int ItemCode, string TransDate, int YearCode, string BatchNo);
        Task<DataTable> GetInvoiceDetailToExportInXml(string FromDate, string ToDate, string InvoiceNo, int AccountCode, string InvoiceType);
        Task<ResponseResult> GetFeatureOptions();
        Task<ResponseResult> GetFormRights(int userId, int menuId);
    }
}
