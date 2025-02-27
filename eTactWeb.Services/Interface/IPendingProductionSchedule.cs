using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Services.Interface
{
    public interface IPendingProductionSchedule
    {
        Task<ResponseResult> FillStore();
        Task<ResponseResult> FillItemName();
        Task<ResponseResult> FillPartCode();
        Task<ResponseResult> FillPendingProdPlanNo();
        Task<ResponseResult> FillWorkCenter();
        Task<ResponseResult> FillPendingProdPlanYearCode(string ProdPlanNo);
        Task<ResponseResult> FillProdScheduleNo(string ProdPlanNo,int ProdPlanYearCode);
        Task<ResponseResult> GetDataForPendingProductionSchedule(string Flag, string FromDate, string ToDate, int StoreId, int YearCode, string GlobalSearch, string ProdSchNo, int WcId);
    }
}
