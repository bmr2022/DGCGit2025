using eTactWeb.Data.Common;
using eTactWeb.Data.DAL;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Data.BLL
{
    public class StockAdjustmentBLL : IStockAdjustment
    {
        public StockAdjustmentBLL(IConfiguration configuration, IDataLogic iDataLogic, ConnectionStringService connectionStringService)
        {
            _StockAdjustmentDAL = new StockAdjustmentDAL(configuration, iDataLogic, connectionStringService);
        }

        private StockAdjustmentDAL? _StockAdjustmentDAL { get; }

        public async Task<ResponseResult> GetFormRights(int userID)
        {
            return await _StockAdjustmentDAL.GetFormRights(userID);
        }

        public async Task<DataSet> BindAllDropDowns(string Flag)
        {
            return await _StockAdjustmentDAL.BindAllDropDowns(Flag);
        }
        public async Task<ResponseResult> StockAdjBackDatePassword()
        {
            return await _StockAdjustmentDAL.StockAdjBackDatePassword();
        }

        public async Task<ResponseResult> NewEntryId(int YearCode)
        {
            return await _StockAdjustmentDAL.NewEntryId(YearCode);
        }
        public async Task<ResponseResult> FillPartCode(string Flag)
        {
            return await _StockAdjustmentDAL.FillPartCode(Flag);
        }
        public async Task<ResponseResult> FillItemName(string Flag)
        {
            return await _StockAdjustmentDAL.FillItemName(Flag);
        }
        public async Task<ResponseResult> GetmaxStockAdjustDate(string Flag, int ItemCode)
        {
            return await _StockAdjustmentDAL.GetmaxStockAdjustDate(Flag, ItemCode);
        }
        public async Task<ResponseResult> StockAdjustByFeaturesOptions(string Flag)
        {
            return await _StockAdjustmentDAL.StockAdjustByFeaturesOptions(Flag);
        }
        public async Task<ResponseResult> FillLotStock(int ItemCode, int Storeid, string UniqueBatchNo, string BatchNo)
        {
            return await _StockAdjustmentDAL.FillLotStock(ItemCode, Storeid, UniqueBatchNo, BatchNo);
        }
        public async Task<ResponseResult> FillRateAmount(int ItemCode, int YearCode, string UniqueBatchNo = "", string BatchNo = "")
        {
            return await _StockAdjustmentDAL.FillRateAmount(ItemCode, YearCode, UniqueBatchNo, BatchNo);
        }
        public async Task<ResponseResult> FillTotalStock(int ItemCode, int Storeid)
        {
            return await _StockAdjustmentDAL.FillTotalStock(ItemCode, Storeid);
        }
        public async Task<ResponseResult> FillWorkCenter(string Flag)
        {
            return await _StockAdjustmentDAL.FillWorkCenter(Flag);
        }
        public async Task<ResponseResult> CheckLastTransDate(string TransDate, int ItemCode, int StoreWC, int YearCode, string batchno, string uniquebatchno, string Flag)
        {
            return await _StockAdjustmentDAL.CheckLastTransDate(TransDate, ItemCode, StoreWC, YearCode, batchno, uniquebatchno, Flag);
        }
        public async Task<ResponseResult> GetAltUnitQty(int ItemCode, float AltQty, float UnitQty)
        {
            return await _StockAdjustmentDAL.GetAltUnitQty(ItemCode, AltQty, UnitQty);
        }
        public async Task<ResponseResult> FillStore(string Flag)
        {
            return await _StockAdjustmentDAL.FillStore(Flag);
        }
        public async Task<ResponseResult> SaveStockAdjust(StockAdjustmentModel model, DataTable SAGrid)
        {
            return await _StockAdjustmentDAL.SaveStockAdjust(model, SAGrid);
        }
        public async Task<ResponseResult> FillCurrentBatchINStore(int ItemCode, int YearCode, string FinstartDate, string TrDate, string StoreName, string batchno)
        {
            return await _StockAdjustmentDAL.FillCurrentBatchINStore(ItemCode, YearCode, FinstartDate, TrDate,  StoreName, batchno);
        }
        public async Task<ResponseResult> FillCurrentBatchINWIP(int ItemCode, int YearCode, int Wcid, string batchno, string TransDate)
        {
            return await _StockAdjustmentDAL.FillCurrentBatchINWIP(ItemCode, YearCode, Wcid, batchno, TransDate);
        }
        public async Task<ResponseResult> GetDashboardData(SADashborad model)
        {
            return await _StockAdjustmentDAL.GetDashboardData(model);
        }
        public async Task<ResponseResult> GETWIPotalSTOCK(int ItemCode, int WCID)
        {
            return await _StockAdjustmentDAL.GETWIPotalSTOCK(ItemCode, WCID);
        }
        public async Task<ResponseResult> GetItemCode(string PartCode)
        {
            return await _StockAdjustmentDAL.GetItemCode(PartCode);
        }
        public async Task<ResponseResult> GetStoreId(string StoreName)
        {
            return await _StockAdjustmentDAL.GetStoreId(StoreName);
        }
        public async Task<ResponseResult> GetWorkCenterId(string WCName)
        {
            return await _StockAdjustmentDAL.GetWorkCenterId(WCName);
        }
        public async Task<ResponseResult> GetWIPStockBatchWise(int ItemCode, int WCID, string uniquebatchno, string batchno)
        {
            return await _StockAdjustmentDAL.GetWIPStockBatchWise(ItemCode, WCID, uniquebatchno, batchno);
        }
        public async Task<ResponseResult> GetItems(int ItemCode)
        {
            return await _StockAdjustmentDAL.GetItems(ItemCode);
        }
        public async Task<ResponseResult> GetWCName(int Wcid)
        {
            return await _StockAdjustmentDAL.GetWCName(Wcid);
        }
        public async Task<ResponseResult> GetStoreName(int storeid)
        {
            return await _StockAdjustmentDAL.GetStoreName(storeid);
        }
        public async Task<ResponseResult> DeleteByID(int ID, int YC, int entryByEmp, string EntryByMachineName, string EntryDate)
        {
            return await _StockAdjustmentDAL.DeleteByID(ID, YC,entryByEmp,EntryByMachineName,EntryDate);
        }
        public async Task<StockAdjustmentModel> GetViewByID(int ID, string Mode, int YC)
        {
            return await _StockAdjustmentDAL.GetViewByID(ID, Mode, YC);
        }
        public async Task<ResponseResult> GetDashItemName(string FromDate, string ToDate)
        {
            return await _StockAdjustmentDAL.GetDashItemName( FromDate, ToDate);
        }
        public async Task<ResponseResult> GetDashPartCode(string FromDate, string ToDate)
        {
            return await _StockAdjustmentDAL.GetDashPartCode(FromDate, ToDate);
        }
        public async Task<ResponseResult> GetDashStoreName(string FromDate, string ToDate)
        {
            return await _StockAdjustmentDAL.GetDashStoreName(FromDate, ToDate);
        }
        public async Task<ResponseResult> GetDashWorkCenter(string FromDate, string ToDate)
        {
            return await _StockAdjustmentDAL.GetDashWorkCenter(FromDate, ToDate);
        }


    }
}
