using eTactWeb.DOM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Services.Interface
{
    public interface IBOMReport
    {
        Task<ResponseResult> GetBOMTree();
        Task<ResponseResult> GetDirectBOMTree();
        Task<ResponseResult> GetBOMStockTree();
        Task<BOMReportModel> GetBomTreeDetailsData(string fromDate, string toDate, int Yearcode, string ReportType, string FGPartCode, string RMPartCode, int Storeid,float calculateQty);
        Task<ResponseResult> FillFinishPartCode();
        Task<ResponseResult> FillFinishItemName();
        Task<ResponseResult> FillRMItemName();
        Task<ResponseResult> FillRMPartCode();
        Task<ResponseResult> FillStoreName();
        Task<ResponseResult> FillWorkCenterName();

    }
}
