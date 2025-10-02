using eTactWeb.DOM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Services.Interface
{
    public interface IOrderAmendHistory
    {
        Task<OrderAmendHistoryModel> GetOrderAmendHistoryData(string FromDate, string ToDate, string ReportType, int AccountCode, string PartCode, string ItemName, string PONO, int ItemCode,string HistoryReportMode,string AmmNo);
        Task<ResponseResult> FillPONO(string FromDate, string ToDate, int AccountCode,string HistoryReportMode);
        Task<ResponseResult> FillPONOAmendNo(string FromDate, string ToDate,int AccountCode,string PoNo,string HistoryReportMode);
        Task<ResponseResult> FillVendorName(string FromDate, string ToDate);
        Task<ResponseResult> FillItemName(string FromDate, string ToDate);
        Task<ResponseResult> FillPartCode(string FromDate, string ToDate);
    }
}
