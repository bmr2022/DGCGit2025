using eTactWeb.Data.Common;
using eTactWeb.Data.DAL;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Data.BLL
{
    public class InterStoreTransferBLL : IInterStoreTransfer
    {

        public InterStoreTransferBLL(IConfiguration configuration, IDataLogic iDataLogic, ConnectionStringService connectionStringService)
        {
            _InterStoreTransferDAL = new InterStoreTransferDAL(configuration, iDataLogic,connectionStringService);
        }

        private InterStoreTransferDAL? _InterStoreTransferDAL { get; }

        public async Task<ResponseResult> GetFormRights(int userID)
        {
            return await _InterStoreTransferDAL.GetFormRights(userID);
        }
        public async Task<ResponseResult> NewEntryId(int yearCode)
        {
            return await _InterStoreTransferDAL.NewEntryId(yearCode);
        }
        public async Task<ResponseResult> FillStore()
        {
            return await _InterStoreTransferDAL.FillStore();
        }
        public async Task<ResponseResult> GetPrevQty(int EntryId,int  YearCode,int ItemCode,string uniqueBatchno)
        {
            return await _InterStoreTransferDAL.GetPrevQty(EntryId,YearCode,ItemCode,uniqueBatchno);
        }
        public async Task<ResponseResult> DeleteByID(int ID,int YC, string EntryDate, int ActualEntryBy, string MachineName)
        {
            return await _InterStoreTransferDAL.DeleteByID(ID,YC,EntryDate, ActualEntryBy, MachineName);
        }
        public async Task<ResponseResult> GetDashboardData(ISTDashboard model)
        {
            return await _InterStoreTransferDAL.GetDashboardData(model);
        }
        public async Task<InterStoreTransferModel> GetViewByID(int ID, string Mode, int YC)
        {
            return await _InterStoreTransferDAL.GetViewByID(ID,Mode,YC);
        }
        public async Task<ResponseResult> GetUnitAltUnit(int ItemCode)
        {
            return await _InterStoreTransferDAL.GetUnitAltUnit(ItemCode);
        }
        public async Task<ResponseResult> FillStockBatchNo(int ItemCode, string StoreName, int YearCode, string batchno,string FinStartDate)
        {
            return await _InterStoreTransferDAL.FillStockBatchNo(ItemCode, StoreName, YearCode, batchno,FinStartDate);
        }
        public async Task<ResponseResult> FillLoadToStoreName()
        {
            return await _InterStoreTransferDAL.FillLoadToStoreName();
        }
        public async Task<ResponseResult> FillLoadTOWorkcenter()
        {
            return await _InterStoreTransferDAL.FillLoadTOWorkcenter();
        }
        public async Task<ResponseResult> GetAllowBackDate()
        {
            return await _InterStoreTransferDAL.GetAllowBackDate();
        }
        
        public async Task<ResponseResult> CheckIssuedTransStock(int ItemCode, int YearCode, int EntryId, string TransDate, string TransNo, int Storeid, string batchno, string uniquebatchno,string Flag)
        {
            return await _InterStoreTransferDAL.CheckIssuedTransStock(ItemCode,YearCode,EntryId,TransDate,TransNo,Storeid,batchno,uniquebatchno,Flag);
        }
        public async Task<ResponseResult> FillPartCode(string ShowAllItems)
        {
            return await _InterStoreTransferDAL.FillPartCode(ShowAllItems);
        }
        public async Task<ResponseResult> FillItems(string ShowAllItems)
        {
            return await _InterStoreTransferDAL.FillItems(ShowAllItems);
        }
        public async Task<ResponseResult> SaveInterStore(InterStoreTransferModel model,DataTable ISTGrid)
        {
            return await _InterStoreTransferDAL.SaveInterStore(model,ISTGrid);
        }
    }
}
