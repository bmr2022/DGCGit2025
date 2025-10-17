using eTactWeb.DOM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Services.Interface
{
    public interface IConsumptionReport
    {
        Task<ResponseResult> FillFGItemName();
        Task<ResponseResult> FillFGPartCode();
        Task<ResponseResult> FillRMItemName();
        Task<ResponseResult> FillRMPartCode();
        Task<ResponseResult> FillStoreName();
        Task<ResponseResult> FillWorkCenterName();
        Task<ConsumptionReportModel> GetConsumptionDetailsData(string fromDate, string toDate, int WorkCenterid, string ReportType, int FGItemCode, int RMItemCode, int Storeid, string GroupName, string ItemCateg);
        Task<DataSet> GetCategory();
        Task<DataSet> GetGroupName();
    }
}
