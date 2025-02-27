using eTactWeb.DOM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Services.Interface
{
    public interface ITransferMaterialReport
    {
        Task<ResponseResult> FillFromWorkCenter(string FromDate, string ToDate);
        Task<ResponseResult> FillToWorkCenter(string FromDate, string ToDate);
        Task<ResponseResult> FillToStore(string FromDate, string ToDate);
        Task<ResponseResult> FillPartCode(string FromDate, string ToDate);
        Task<ResponseResult> FillItemName(string FromDate, string ToDate);
        Task<ResponseResult> FillTransferSlipNo(string FromDate, string ToDate);
        Task<ResponseResult> FillProdSlipNo(string FromDate, string ToDate);
        Task<ResponseResult> FillProdPlanNo(string FromDate, string ToDate);
        Task<ResponseResult> FillProdSchNo(string FromDate, string ToDate);
        Task<ResponseResult> FillProcessName(string FromDate, string ToDate);
        Task<TransferMaterialReportModel> GetTransferMaterialReport(string ReportType, string FromDate, string ToDate, string FromWorkCenter, string ToWorkCenter, string Tostore, string PartCode, string ItemName, string TransferSlipNo, string ProdSlipNo, string ProdPlanNo, string ProdSchNo, string ProcessName);
    }
}
