using eTactWeb.Data.Common;
using eTactWeb.Data.DAL;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Data.BLL
{
    public class TransferFromWorkCenterBLL : ITransferFromWorkCenter
    {
        private readonly IDataLogic _DataLogicDAL;
        private readonly TranferFromWorkCenterDAL _TranferFromWorkCenterDAL;
        public TransferFromWorkCenterBLL(IConfiguration configuration, IDataLogic iDataLogic, ConnectionStringService connectionStringService)
        {
            _DataLogicDAL = iDataLogic;
            _TranferFromWorkCenterDAL = new TranferFromWorkCenterDAL(configuration, iDataLogic, connectionStringService);
        }
        public async Task<ResponseResult> GetFormRights(int userID)
        {
            return await _TranferFromWorkCenterDAL.GetFormRights(userID);
        }
        public async Task<DataSet> BindAllDropDown()
        {
            return await _TranferFromWorkCenterDAL.BindAllDropDown();
        }

        public async Task<ResponseResult> GetReportName()
        {
            return await _TranferFromWorkCenterDAL.GetReportName();
        }
        public async Task<TransferFromWorkCenterModel> GetViewByID(int ID, int YearCode)
        {
            return await _TranferFromWorkCenterDAL.GetViewByID(ID, YearCode);
        }
        public async Task<ResponseResult> FillEntryandGate(string Flag, int YearCode, string SPName)
        {
            return await _TranferFromWorkCenterDAL.FillEntryandGate(Flag, YearCode, SPName);
        }
        public async Task<ResponseResult> BindEmpList()
        {
            return await _TranferFromWorkCenterDAL.BindEmpList();
        }
        public async Task<ResponseResult> FillStoreName()
        {
            return await _TranferFromWorkCenterDAL.FillStoreName();
        }
        public async Task<ResponseResult> ChkWIPStockBeforeSaving(int WcId, string TransferMatEntryDate, int TransferMatYearCode,int TransferMatEntryId, DataTable TransferGrid, string Mode)
        {
            return await _TranferFromWorkCenterDAL.ChkWIPStockBeforeSaving(WcId,TransferMatEntryDate,TransferMatYearCode, TransferMatEntryId,TransferGrid,Mode);
        }
        public async Task<ResponseResult> FillProcessName()
        {
            return await _TranferFromWorkCenterDAL.FillProcessName();
        }
        public async Task<ResponseResult> FillWorkCenter()
        {
            return await _TranferFromWorkCenterDAL.FillWorkCenter();
        }
        public async Task<ResponseResult> FillItemName(int TransferMatYearCode)
        {
            return await _TranferFromWorkCenterDAL.FillItemName(TransferMatYearCode);
        }
        public async Task<ResponseResult> FillPartCode(int TransferMatYearCode)
        {
            return await _TranferFromWorkCenterDAL.FillPartCode(TransferMatYearCode);
        }
        public async Task<ResponseResult> FillItems(string Type, string ShowAllItem, string SearchItemCode, string SearchPartCode)
        {
            return await _TranferFromWorkCenterDAL.FillItems(Type, ShowAllItem, SearchItemCode, SearchPartCode);
        }
        public async Task<ResponseResult> GetBatchNumber(string SPName, int ItemCode, int YearCode, float WcId, string TransDate, string BatchNo)
        {
            return await _TranferFromWorkCenterDAL.GetBatchNumber(SPName, ItemCode, YearCode, WcId, TransDate, BatchNo);
        }
        public async Task<ResponseResult> GetWorkCenterTotalStock(string Flag, int ItemCode, int WcId, string TillDate)
        {
            return await _TranferFromWorkCenterDAL.GetWorkCenterTotalStock(Flag, ItemCode, WcId, TillDate);
        }
        public async Task<ResponseResult> GetWorkCenterQty(string SPName, int ItemCode, int WcId, string TillDate, string BatchNo, string UniqueBatchNo)
        {
            return await _TranferFromWorkCenterDAL.GetWorkCenterQty(SPName, ItemCode, WcId, TillDate, BatchNo, UniqueBatchNo);
        }
        public async Task<ResponseResult> GetUnit(int ItemCode)
        {
            return await _TranferFromWorkCenterDAL.GetUnit(ItemCode);
        }
        public async Task<ResponseResult> SaveTransferFromWorkCenter(TransferFromWorkCenterModel model, DataTable TransferGrid)
        {
            return await _TranferFromWorkCenterDAL.SaveTransferFromWorkCenter(model, TransferGrid);
        }
        public async Task<ResponseResult> GetDashboardData()
        {
            return await _TranferFromWorkCenterDAL.GetDashboardData();
        }
        public async Task<TransferFromWorkCenterDashboard> GetDashboardData(string FromDate, string ToDate, string TransferMatSlipNo, string ItemName, string PartCode, string TransferFromWC, string TransferToWC, string TransferToStore, string ProdSlipNo, string ProdSchNo, string DashboardType)
        {
            return await _TranferFromWorkCenterDAL.GetDashboardData(FromDate,ToDate,TransferMatSlipNo,ItemName,PartCode, TransferFromWC, TransferToWC, TransferToStore, ProdSlipNo, ProdSchNo, DashboardType);
        }
        public async Task<TransferFromWorkCenterDashboard> GetDashboardDetailData(string FromDate, string ToDate, string TransferMatSlipNo, string ItemName, string PartCode, string TransferFromWC, string TransferToWC, string TransferToStore, string ProdSlipNo, string ProdSchNo, string DashboardType)
        {
            return await _TranferFromWorkCenterDAL.GetDashboardDetailData(FromDate, ToDate,TransferMatSlipNo,ItemName,PartCode,TransferFromWC,TransferToWC, TransferToStore, ProdSlipNo,ProdSchNo,DashboardType);
        }
        public async Task<ResponseResult> DeleteByID(int ID, int YC, string CC, string EntryByMachineName, string EntryDate,int EmpID)
        {
            return await _TranferFromWorkCenterDAL.DeleteByID(ID, YC, CC, EntryByMachineName, EntryDate,EmpID);
        }
        public async Task<ResponseResult> CheckEditOrDelete(int TransferEntryId, int TransferYearCode)
        {
            return await _TranferFromWorkCenterDAL.CheckEditOrDelete(TransferEntryId, TransferYearCode);
        }

        public async Task<TransferFromWorkCenterModel> selectMultipleItem(int WCID, string FromDate, string ToDate, string PartCode)
        {
            return await _TranferFromWorkCenterDAL.selectMultipleItem( WCID,  FromDate,  ToDate,  PartCode);
        }
    }
}
