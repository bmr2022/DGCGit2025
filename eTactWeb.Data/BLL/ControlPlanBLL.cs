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
    public class ControlPlanBLL: IControlPlan
    {
        private ControlPlanDAL _ControlPlanDAL;
        private readonly IDataLogic _DataLogicDAL;

        public ControlPlanBLL(IConfiguration config, IDataLogic dataLogicDAL, ConnectionStringService connectionStringService)
        {
            _ControlPlanDAL = new ControlPlanDAL(config, dataLogicDAL, connectionStringService);
            _DataLogicDAL = dataLogicDAL;
        }
        public async Task<ResponseResult> GetNewEntryId(int Yearcode)
        {
            return await _ControlPlanDAL.GetNewEntryId(Yearcode);
        }
        public async Task<ResponseResult> GetItemName()
        {
            return await _ControlPlanDAL.GetItemName();
        }
        public async Task<ResponseResult> GetPartCode()
        {
            return await _ControlPlanDAL.GetPartCode();
        }
         public async Task<ResponseResult> GetCharacteristic()
        {
            return await _ControlPlanDAL.GetCharacteristic();
        }
        public async Task<ResponseResult> GetEvMeasureTech()
        {
            return await _ControlPlanDAL.GetEvMeasureTech();
        }
        public async Task<ResponseResult> SaveControlPlan(ControlPlanModel model, DataTable GIGrid)
        {
            return await _ControlPlanDAL.SaveControlPlan(model, GIGrid);
        }
        public async Task<ResponseResult> GetDashboardData(ControlPlanModel model)
        {
            return await _ControlPlanDAL.GetDashboardData(model);
        }
        public async Task<ControlPlanModel> GetDashboardDetailData(string FromDate, string ToDate, string ReportType)
        {
            return await _ControlPlanDAL.GetDashboardDetailData(FromDate, ToDate, ReportType);
        }

        public async Task<ResponseResult> DeleteByID(int EntryId, int YearCode, string EntryDate, int EntryByempId)
        {
            return await _ControlPlanDAL.DeleteByID(EntryId, YearCode, EntryDate, EntryByempId);
        }
        public async Task<ControlPlanModel> GetViewByID(int ID, int YC, string FromDate, string ToDate)
        {
            return await _ControlPlanDAL.GetViewByID(ID, YC, FromDate, ToDate);
        } 
        public async Task<ControlPlanModel> GetByItemOrPartCode(int ItemCode)
        {
            return await _ControlPlanDAL.GetByItemOrPartCode( ItemCode);
        }
        public async Task<ResponseResult> SaveMultipleControlPlanData(DataTable ControlPlanDetailGrid, int YearCode, string EntryDate, string ForInOutInprocess, int EngApprovedBy, string CC, int UId, string ActualEntryDate)
        {
            return await _ControlPlanDAL.SaveMultipleControlPlanData(ControlPlanDetailGrid, YearCode, EntryDate, ForInOutInprocess, EngApprovedBy, CC, UId, ActualEntryDate);
        }
    }
}
