using eTactWeb.Data.Common;
using eTactWeb.Data.DAL;
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
    public class PendingProductionScheduleBLL : IPendingProductionSchedule
    {
        private readonly PendingProductionScheduleDAL _PendingProductionScheduleDAL;
        private readonly IDataLogic _IDataLogic;
        public PendingProductionScheduleBLL(IConfiguration configuration, IDataLogic iDataLogic, ConnectionStringService connectionStringService)
        {
            _IDataLogic = iDataLogic;
            _PendingProductionScheduleDAL = new PendingProductionScheduleDAL(configuration, iDataLogic, connectionStringService);
        }
        public async Task<ResponseResult> FillStore()
        {
            return await _PendingProductionScheduleDAL.FillStore();
        }
        public async Task<ResponseResult> FillWorkCenter()
        {
            return await _PendingProductionScheduleDAL.FillWorkCenter();
        }
        public async Task<ResponseResult> FillItemName()
        {
            return await _PendingProductionScheduleDAL.FillItemName();
        }
        public async Task<ResponseResult> FillPendingProdPlanNo()
        {
            return await _PendingProductionScheduleDAL.FillPendingProdPlanNo();
        }
        public async Task<ResponseResult> FillPendingProdPlanYearCode(string ProdPlanNo)
        {
            return await _PendingProductionScheduleDAL.FillPendingProdPlanYearCode(ProdPlanNo);
        }
        public async Task<ResponseResult> FillPartCode()
        {
            return await _PendingProductionScheduleDAL.FillPartCode();
        }
        public async Task<ResponseResult> FillProdScheduleNo(string ProdPlanNo,int ProdPlanYearCode)
        {
            return await _PendingProductionScheduleDAL.FillProdScheduleNo(ProdPlanNo,ProdPlanYearCode);
        }
        public async Task<ResponseResult> GetDataForPendingProductionSchedule(string Flag, string FromDate, string ToDate, int StoreId, int YearCode, string GlobalSearch, string ProdSchNo, int WcId)
        {
            return await _PendingProductionScheduleDAL.GetDataForPendingProductionSchedule(Flag, FromDate, ToDate, StoreId, YearCode, GlobalSearch, ProdSchNo, WcId);
        }
    }
}
