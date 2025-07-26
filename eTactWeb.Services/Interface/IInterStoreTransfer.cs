using eTactWeb.DOM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Services.Interface
{
    public interface IInterStoreTransfer
    {
        Task<ResponseResult> GetFormRights(int userID);
        Task<ResponseResult> NewEntryId(int yearCode);
        Task<ResponseResult> FillPartCode(string ShowAllItems);
        Task<ResponseResult> FillItems(string ShowAllItems);
        Task<ResponseResult> FillStockBatchNo(int ItemCode, string StoreName, int YearCode, string batchno,string FinStartDate);
        Task<ResponseResult> FillStore();
        public Task<ResponseResult> AutoFillitem(string Flag, string showallitem, string SearchItemCode, string SearchPartCode);

        Task<ResponseResult> GetPrevQty(int EntryId,int YearCode,int ItemCode,string uniqueBatchno);
        Task<ResponseResult> DeleteByID(int ID,int YC,string EntryDate,int ActualEntryBy, string MachineName);
        Task<ResponseResult> GetDashboardData(ISTDashboard model);
        Task<InterStoreTransferModel> GetViewByID(int ID, string Mode, int YC);
        Task<ResponseResult> GetUnitAltUnit(int ItemCode);
        Task<ResponseResult> FillLoadToStoreName();
        Task<ResponseResult> CheckIssuedTransStock(int ItemCode, int YearCode, int EntryId, string TransDate, string TransNo, int Storeid, string batchno, string uniquebatchno,string Flag);
        Task<ResponseResult> GetAllowBackDate();
        Task<ResponseResult> FillLoadTOWorkcenter();
        Task<ResponseResult> SaveInterStore(InterStoreTransferModel model, DataTable ISTGrid);
    }
}