using eTactWeb.DOM.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Services.Interface
{
    public interface IStockAdjustment
    {
        Task<ResponseResult> GetFormRights(int userID);
        Task<ResponseResult> StockAdjBackDatePassword();
        Task<DataSet> BindAllDropDowns(string Flag);
        Task<ResponseResult> NewEntryId(int YearCode);
        Task<ResponseResult> FillPartCode(string Flag,string search);
        Task<ResponseResult> FillItemName(string Flag, string search);
        Task<ResponseResult> GetmaxStockAdjustDate(string Flag, int ItemCode);
        Task<ResponseResult> StockAdjustByFeaturesOptions(string Flag);
        Task<ResponseResult> FillLotStock(int ItemCode, int Storeid, string UniqueBatchNo, string BatchNo);
        Task<ResponseResult> FillRateAmount(int ItemCode, int YearCode, string UniqueBatchNo = "", string BatchNo = "");
        Task<ResponseResult> FillTotalStock(int ItemCode, int Storeid);
        Task<ResponseResult> GetAltUnitQty(int ItemCode, float AltQty, float UnitQty);
        Task<ResponseResult> FillWorkCenter(string Flag);
        Task<ResponseResult> CheckLastTransDate(string TransDate, int ItemCode, int StoreWC, int YearCode, string batchno, string uniquebatchno, string Flag);
        Task<ResponseResult> FillStore(string Flag);
        Task<ResponseResult> SaveStockAdjust(StockAdjustmentModel model, DataTable SAGrid);
        Task<ResponseResult> FillCurrentBatchINStore(int ItemCode, int YearCode, string FinStartDate,string TrDate, string StoreName, string batchno);
        Task<ResponseResult> FillCurrentBatchINWIP(int ItemCode, int YearCode, int WCid, string batchno, string TransDate);
        Task<ResponseResult> GetDashboardData(SADashborad model);
        Task<ResponseResult> GETWIPotalSTOCK(int ItemCode, int WCID);
        Task<ResponseResult> GetItemCode(string PartCode);
        Task<ResponseResult> GetStoreId(string StoreName);
        Task<ResponseResult> GetWorkCenterId(string WCName);
        Task<ResponseResult> GetWIPStockBatchWise(int ItemCode, int WCID, string uniquebatchno, string batchno);
        Task<ResponseResult> GetItems(int ItemCode);
        Task<ResponseResult> GetWCName(int Wcid);
        Task<ResponseResult> GetStoreName(int storeid);

        Task<ResponseResult> GetDashItemName(string FromDate,string ToDate);
        Task<ResponseResult> GetDashPartCode(string FromDate, string ToDate);
        Task<ResponseResult> GetDashStoreName(string FromDate, string ToDate);
        Task<ResponseResult> GetDashWorkCenter(string FromDate, string ToDate);
        Task<ResponseResult> DeleteByID(int ID, int YC, int entryByEmp, string EntryByMachineName,string EntryDate);
        Task<StockAdjustmentModel> GetViewByID(int ID, string Mode, int YC);
    }
}
