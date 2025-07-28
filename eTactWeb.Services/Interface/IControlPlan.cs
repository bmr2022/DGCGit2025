using eTactWeb.DOM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Services.Interface
{
    public interface IControlPlan
    {
        Task<ResponseResult> GetNewEntryId(int Yearcode);
        Task<ResponseResult> GetItemName();
        Task<ResponseResult> GetPartCode();
        Task<ResponseResult> GetEvMeasureTech();
        Task<ResponseResult> GetCharacteristic();
        Task<ResponseResult> SaveControlPlan(ControlPlanModel model, DataTable GIGrid);
        Task<ResponseResult> GetDashboardData(ControlPlanModel  model);
        Task<ControlPlanModel> GetDashboardDetailData(string FromDate, string ToDate, string ReportType);
        Task<ResponseResult> DeleteByID(int EntryId, int YearCode, string EntryDate, int EntryByempId);
        Task<ControlPlanModel> GetViewByID(int ID, int YC, string FromDate, string TODate);
        Task<ControlPlanModel> GetByItemOrPartCode(int ItemCode);
        Task<ResponseResult> SaveMultipleControlPlanData(DataTable ControlPlanDetailGrid);
    }
}
