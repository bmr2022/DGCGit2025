using eTactWeb.DOM.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Services.Interface
{
    public interface IInventoryAgingReport
    {
        Task<ResponseResult> FillRMItemName(string FromDate,string ToDate, string CurrentDate,int Storeid);
        Task<ResponseResult> FillRMPartCode(string FromDate, string ToDate, string CurrentDate, int Storeid);
        Task<ResponseResult> FillStoreName(string FromDate, string ToDate, string CurrentDate, int Storeid);
        Task<ResponseResult> FillWorkCenterName(string FromDate, string ToDate, string CurrentDate, int Storeid);

        Task<InventoryAgingReportModel> GetInventoryAgingReportDetailsData(string fromDate, string toDate,string CurrentDate, int WorkCenterid, string ReportType, int RMItemCode, int Storeid,int Foduration);

    }
}
