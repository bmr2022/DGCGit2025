using eTactWeb.DOM.Models;
using eTactWeb.DOM.Models.Master;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Services.Interface
{
    public interface ICloseProductionPlan
    {
        Task<ResponseResult> GetOpenItemName(int EmpId,int ActualEntryId);
        Task<ResponseResult> GetOpenPlanNo(int EmpId,int ActualEntryId);
        Task<ResponseResult> GetCloseItemName(int EmpId,int ActualEntryId);
        Task<ResponseResult> GetClosePlanNo(int EmpId,int ActualEntryId);
        Task<CloseProductionPlanModel> GetGridDetailData(int EmpId, string ActualEntryByEmpName, string ReportType, string FromDate, string ToDate, string CloseOpen);
        Task<ResponseResult> SaveCloseProductionPlan(CloseProductionPlanModel model, DataTable GIGrid);
    }
}
