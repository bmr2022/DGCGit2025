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
    public  class CloseProductionPlanBLL:ICloseProductionPlan
    {
        public CloseProductionPlanDAL _CloseProductionPlanDAL;
        private readonly IDataLogic _DataLogicDAL;

        public CloseProductionPlanBLL(IConfiguration config, IDataLogic dataLogicDAL)
        {
            _CloseProductionPlanDAL = new CloseProductionPlanDAL(config, dataLogicDAL);
            _DataLogicDAL = dataLogicDAL;
        }
        public async Task<ResponseResult> GetOpenItemName(int EmpId, int ActualEntryId)
        {
            return await _CloseProductionPlanDAL.GetOpenItemName( EmpId,  ActualEntryId);
        }
        public async Task<ResponseResult> GetOpenPlanNo(int EmpId, int ActualEntryId)
        {
            return await _CloseProductionPlanDAL.GetOpenPlanNo( EmpId,  ActualEntryId);
        }
        public async Task<ResponseResult> GetCloseItemName(int EmpId, int ActualEntryId)
        {
            return await _CloseProductionPlanDAL.GetCloseItemName( EmpId,  ActualEntryId);
        }
        public async Task<ResponseResult> GetClosePlanNo(int EmpId, int ActualEntryId)
        {
            return await _CloseProductionPlanDAL.GetClosePlanNo( EmpId,  ActualEntryId);
        }
        public async Task<CloseProductionPlanModel> GetGridDetailData(int EmpId, string ActualEntryByEmpName, string ReportType, string FromDate, string ToDate)
        {
            return await _CloseProductionPlanDAL.GetGridDetailData( EmpId, ActualEntryByEmpName, ReportType,  FromDate,  ToDate);
        }

    }
}
