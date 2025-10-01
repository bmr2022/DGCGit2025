using eTactWeb.DOM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Services.Interface
{
    public interface IProductionEntryReport
    {
        Task<ResponseResult> FillFGPartCode(string FromDate,string ToDate);
        Task<ResponseResult> FillFGItemName(string FromDate, string ToDate);
        Task<ResponseResult> FillRMPartCode(string FromDate, string ToDate);
        Task<ResponseResult> FillRMItemName(string FromDate, string ToDate);
        Task<ResponseResult> FillProdSlipNo(string FromDate, string ToDate);
        Task<ResponseResult> FillProdPlanNo(string FromDate, string ToDate);
        Task<ResponseResult> FillProdSchNo(string FromDate, string ToDate);
        Task<ResponseResult> FillReqNo(string FromDate, string ToDate);
        Task<ResponseResult> FillShiftName();
        Task<ResponseResult> filltranstore();
        Task<ResponseResult> GetCompanyName();
        Task<ResponseResult> filltranworkcenter();
        Task<ResponseResult> FillWorkCenter(string FromDate, string ToDate);
        Task<ResponseResult> FillMachinName(string FromDate, string ToDate);
        Task<ResponseResult> FillOperatorName(string FromDate, string ToDate);
        Task<ResponseResult> FillProcess(string FromDate, string ToDate);
        Task<ProductionEntryReportModel> GetProductionEntryReport(string ReportType, string FromDate, string ToDate, string FGPartCode, string FGItemName, string RMPartCode, string RMItemName, string ProdSlipNo, string ProdPlanNo, string ProdSchNo, string ReqNo, string WorkCenter, string MachineName, string OperatorName, string Process, string ShiftName,int StoreID,int WCID);
    }
}
