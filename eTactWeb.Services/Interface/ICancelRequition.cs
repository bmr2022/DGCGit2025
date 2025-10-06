using eTactWeb.DOM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Services.Interface
{
    public interface ICancelRequition
    {
        Task<ResponseResult> FillItemName(string Fromdate,string ToDate,string ReportType,string PendCancelReq,string RequitionType,string ReqNo,int ItemCode);
        Task<ResponseResult> FillReqNo(string Fromdate,string ToDate,string ReportType,string PendCancelReq,string RequitionType,string ReqNo,int ItemCode);
        Task<ResponseResult> FillPartcode(string Fromdate,string ToDate,string ReportType,string PendCancelReq,string RequitionType,string ReqNo,int ItemCode);
        Task<CancelRequitionModel> GetSearchData(string Fromdate,string ToDate,string ReportType,string PendCancelReq,string RequitionType,string ReqNo,int ItemCode);
    }
}
