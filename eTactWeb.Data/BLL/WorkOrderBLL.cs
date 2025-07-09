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
    public class WorkOrderBLL : IWorkOrder
    {
        private readonly IDataLogic _DataLogicDAL;
        private readonly WorkOrderDAL _WorkOrderDAL;

        public WorkOrderBLL(IConfiguration configuration, IDataLogic iDataLogic, ConnectionStringService connectionStringService)
        {
            _DataLogicDAL = iDataLogic;
            _WorkOrderDAL = new WorkOrderDAL(configuration, iDataLogic, connectionStringService);
        }
        public async Task<ResponseResult> GetFormRights(int userID)
        {
            return await _WorkOrderDAL.GetFormRights(userID);
        }
        public async Task<ResponseResult> GetSaleOrderData(string Flag, string SPName, int YearCode, string WODate, string EffFrom, string EffTill)
        {
            return await _WorkOrderDAL.GetSaleOrderData(Flag, SPName, YearCode, WODate,EffFrom,EffTill);
        }

        public async Task<ResponseResult> GetStoreList(string Flag, string SPName)
        {
            return await _WorkOrderDAL.GetStoreList(Flag, SPName);
        }
        public async Task<ResponseResult> GetTotalStockList(int store, int Itemcode)
        {
            return await _WorkOrderDAL.GetTotalStockList(store, Itemcode);
        }
        public async Task<ResponseResult> NewEntryId(int YearCode)
        {
            return await _WorkOrderDAL.NewEntryId(YearCode);
        }
        public async Task<ResponseResult> FillWorkCenter()
        {
            return await _WorkOrderDAL.FillWorkCenter();
        }
        public async Task<ResponseResult> GetBomNo(int ItemCode)
        {
            return await _WorkOrderDAL.GetBomNo(ItemCode);
        }
        public async Task<ResponseResult> DisplayBomDetail(int ItemCode, float WOQty, int BomRevNo)
        {
            return await _WorkOrderDAL.DisplayBomDetail(ItemCode,WOQty,BomRevNo);
        }
        public async Task<ResponseResult> GetBomName(int BomNo,int FinishedItemCode)
        {
            return await _WorkOrderDAL.GetBomName(BomNo, FinishedItemCode);
        }
        public async Task<ResponseResult> GetEffDate(int BomNo,int FinishedItemCode)
        {
            return await _WorkOrderDAL.GetEffDate(BomNo, FinishedItemCode);
        }
        public async Task<ResponseResult> GetMachineGroupName()
        {
            return await _WorkOrderDAL.GetMachineGroupName();
        }
        public async Task<ResponseResult> GetMachineName(int MachGroupId)
        {
            return await _WorkOrderDAL.GetMachineName(MachGroupId);
        }
        public async Task<ResponseResult> SaveWorkOrder(WorkOrderModel model, DataTable WOGrid)
        {
            return await _WorkOrderDAL.SaveWorkOrder(model, WOGrid);
        }

        public async Task<ResponseResult> GetDashboardData(string FromDate, string ToDate)
        {
            return await _WorkOrderDAL.GetDashboardData(FromDate, ToDate);
        }

        public async Task<WorkOrderGridDashboard> GetDashboardData(string SummaryDetail, string WONO, string CC, string SONO, string SchNo, string AccountName, string PartCode, string ItemName, string FromDate, string ToDate)
        {
            return await _WorkOrderDAL.GetDashboardData(SummaryDetail, WONO, CC, SONO, SchNo, AccountName, PartCode, ItemName, FromDate, ToDate);
        }
        public async Task<ResponseResult> DeleteByID(int ID, int YC, int ActualEntryBy, string MachineName)
        {
            return await _WorkOrderDAL.DeleteByID(ID, YC, ActualEntryBy, MachineName);
        }
        public async Task<WorkOrderModel> GetViewByID(int ID, int YC, string Mode)
        {
            return await _WorkOrderDAL.GetViewByID(ID, YC, Mode);
        }
    }
}