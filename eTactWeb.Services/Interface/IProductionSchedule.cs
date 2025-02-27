using eTactWeb.DOM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Services.Interface
{
    public interface IProductionSchedule
    {
        Task<ResponseResult> NewEntryId(int YearCode);
        Task<ResponseResult> GetItems();
        Task<ResponseResult> GetBomMultiLevelGrid();
        Task<ResponseResult> SaveProductionSchedule(ProductionScheduleModel model, DataTable PSGrid,DataTable prodPlanDetail,DataTable bomChildDetail,DataTable bomSummaryDetail);
        Task<ResponseResult> GetMachineName();
        Task<ResponseResult> GetWorkCenter();
        Task<ProductionScheduleModel> PSBomDetail(int YearCode,DataTable itemGrid);
        Task<ResponseResult> GetPendWOData(string PendWoType,int YearCode,string SOEffFromDate,string CurrentDate);
        Task<ResponseResult> GetDashboardData(string partCode,string itemName,string accountName,string FromDate, string ToDate,int YearCode);
        Task<ProductionScheduleModel> AddPendingProdPlans(int yearCode,string schFromDate,string schTillDate,string displayFlag,int noOfDays,DataTable PendingProdPlans);
        Task<ProductionScheduleModel> GetViewByID(int ID, string Mode, int YC);
        Task<ResponseResult> DeleteByID(int ID, int YC, int createdBy, string entryByMachineName,int ActualEntryBy,string EntryDate);
    }
}
