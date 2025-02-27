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
    public class IssueAgainstProdScheduleBLL : IIssueAgainstProdSchedule
    {
        private readonly IDataLogic _DataLogicDAL;
        private readonly IssueAgainstProdScheduleDAL _IssueAgainstProdScheduleDAL;
        public IssueAgainstProdScheduleBLL(IConfiguration configuration, IDataLogic iDataLogic)
        {
            _DataLogicDAL = iDataLogic;
            _IssueAgainstProdScheduleDAL = new IssueAgainstProdScheduleDAL(configuration, iDataLogic);
        }
        public async Task<ResponseResult> FillEntryId(string Flag, int YearCode, string SPName)
        {
            return await _IssueAgainstProdScheduleDAL.FillEntryId(Flag, YearCode, SPName);
        }
        public async Task<ResponseResult> FillWorkCenter()
        {
            return await _IssueAgainstProdScheduleDAL.FillWorkCenter();
        }
        public async Task<ResponseResult> FillIssueByEmp()
        {
            return await _IssueAgainstProdScheduleDAL.FillIssueByEmp();
        }
        public async Task<ResponseResult> FillRecByEmp()
        {
            return await _IssueAgainstProdScheduleDAL.FillRecByEmp();
        }
        public async Task<ResponseResult> GetIsStockable(int ItemCode)
        {
            return await _IssueAgainstProdScheduleDAL.GetIsStockable(ItemCode);
        }
        public async Task<ResponseResult> DisplayRoutingDetail(int ItemCode)
        {
            return await _IssueAgainstProdScheduleDAL.DisplayRoutingDetail(ItemCode);
        }
        public async Task<ResponseResult> DisplayBomDetail(int ItemCode, float WOQty,int BomNo)
        {
            return await _IssueAgainstProdScheduleDAL.DisplayBomDetail(ItemCode, WOQty,BomNo);
        }
        public async Task<ResponseResult> DeleteByID(int ID, int YC, string ProdSchSlipNo)
        {
            return await _IssueAgainstProdScheduleDAL.DeleteByID(ID, YC, ProdSchSlipNo);
        }
        public async Task<IssueAgainstProdSchedule> GetViewByID(int ID,int YearCode,string ProdSlipNo)
        {
            return await _IssueAgainstProdScheduleDAL.GetViewByID(ID,YearCode, ProdSlipNo);
        }
        public async Task<ResponseResult> GetSummaryDetail(string FromDate, string ToDate, string IssAgtProdSchSlipNo, string IssueFromStore, string PartCode, string ItemName, string ProdPlanNo, string ProdSchNo, string DashboardType)
        {
            return await _IssueAgainstProdScheduleDAL.GetSummaryDetail(FromDate,ToDate,IssAgtProdSchSlipNo,IssueFromStore,PartCode,ItemName,ProdPlanNo,ProdSchNo,DashboardType);
        }
        public async Task<ResponseResult> FillBatchNo(int ItemCode, int YearCode, string StoreName, string BatchNo, string IssuedDate, string FinStartDate)
        {
            return await _IssueAgainstProdScheduleDAL.FillBatchNo(ItemCode,YearCode,StoreName,BatchNo,IssuedDate,FinStartDate);
        }
        public async Task<ResponseResult> FillLotandTotalStock(int ItemCode, int StoreId, string TillDate, string BatchNo, string UniqBatchNo)
        {
            return await _IssueAgainstProdScheduleDAL.FillLotandTotalStock(ItemCode, StoreId, TillDate, BatchNo, UniqBatchNo);
        }
        public async Task<ResponseResult> SaveIssueAgainstProductionSchedule(IssueAgainstProdSchedule model, DataTable IssueAgainstProductionScheduleGrid)
        {
            return await _IssueAgainstProdScheduleDAL.SaveIssueAgainstProductionSchedule(model, IssueAgainstProductionScheduleGrid);
        }
        public async Task<IssueAgainstProdScheduleDashboard> GetSearchData(string FromDate,string IssAgtProdSchSlipNo, string ToDate, string IssueFromStore, string ItemName, string PartCode, string ProdPlanNo, string ProdSchNo, string DashboardType)
        {
            return await _IssueAgainstProdScheduleDAL.GetSearchData(FromDate, ToDate, IssAgtProdSchSlipNo, IssueFromStore, PartCode, ItemName, ProdPlanNo, ProdSchNo, DashboardType);
        }
    }
}
