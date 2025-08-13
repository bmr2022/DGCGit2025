using eTactWeb.Data.Common;
using eTactWeb.Data.DAL;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Data.BLL
{
    public class ProductionScheduleBLL : IProductionSchedule
    {
        private readonly IDataLogic _DataLogicDAL;
        private readonly ProductionScheduleDAL _ProductionScheduleDAL;

        public ProductionScheduleBLL(IConfiguration configuration, IDataLogic iDataLogic, ConnectionStringService connectionStringService)
        {
            _DataLogicDAL = iDataLogic;
            _ProductionScheduleDAL = new ProductionScheduleDAL(configuration, iDataLogic, connectionStringService);
        }

        public async Task<ResponseResult> GetFormRights(int userID)
        {
            return await _ProductionScheduleDAL.GetFormRights(userID);
        }

        public async Task<ResponseResult> NewEntryId(int YearCode)
        {
            return await _ProductionScheduleDAL.NewEntryId(YearCode);
        }
        public async Task<ResponseResult> GetItems()
        {
            return await _ProductionScheduleDAL.GetItems();
        }
        public async Task<ResponseResult> GetBomMultiLevelGrid()
        {
            return await _ProductionScheduleDAL.GetBomMultiLevelGrid();
        }
        public async Task<ProductionScheduleModel> PSBomDetail(int YearCode, DataTable itemGrid)
        {
            return await _ProductionScheduleDAL.PSBomDetail(YearCode,itemGrid);
        }
        public async Task<ResponseResult> GetPendWOData(string PendWoType, int YearCode, string SOEffFromDate, string CurrentDate)
        {
            return await _ProductionScheduleDAL.GetPendWOData(PendWoType,YearCode,SOEffFromDate,CurrentDate);
        }
        public async Task<ResponseResult> GetMachineName(int WorkCenterId)
        {
            return await _ProductionScheduleDAL.GetMachineName(WorkCenterId);
        }
        public async Task<ResponseResult> GetWorkCenter()
        {
            return await _ProductionScheduleDAL.GetWorkCenter();
        }
        public async Task<ResponseResult> SaveProductionSchedule(ProductionScheduleModel model, DataTable PSGrid, DataTable prodPlanDetail, DataTable bomChildDetail, DataTable bomSummaryDetail)
        {
            return await _ProductionScheduleDAL.SaveProductionSchedule(model, PSGrid,prodPlanDetail,bomChildDetail,bomSummaryDetail);
        }
        public async Task<ResponseResult> GetDashboardData(string partCode, string itemName, string accountName, string FromDate, string ToDate, int YearCode)
        {
            return await _ProductionScheduleDAL.GetDashboardData(partCode,itemName,accountName,FromDate, ToDate,YearCode);
        }
        public async Task<ProductionScheduleModel> AddPendingProdPlans(int yearCode, string schFromDate, string schTillDate, string displayFlag, int noOfDays,DataTable PendingProdPlans)
        {
            return await _ProductionScheduleDAL.AddPendingProdPlans(yearCode,schFromDate,schTillDate,displayFlag,noOfDays, PendingProdPlans);
        }
        public async Task<ProductionScheduleModel> GetViewByID(int ID, string Mode, int YC)
        {
            return await _ProductionScheduleDAL.GetViewByID(ID, YC, Mode);
        }
        //public async Task<DataSet> BindItem(string Flag)
        //{
        //    return await _ProductionScheduleDAL.BindItem(Flag);
        //}
        public async Task<ResponseResult> DeleteByID(int ID, int YC, int createdBy, string entryByMachineName, int ActualEntryBy, string EntryDate)
        {
            return await _ProductionScheduleDAL.DeleteByID( ActualEntryBy,  EntryDate,ID, YC, createdBy,entryByMachineName);
        }
    }
}
