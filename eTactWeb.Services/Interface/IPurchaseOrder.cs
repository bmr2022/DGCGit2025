using eTactWeb.DOM.Models;
using Microsoft.AspNetCore.Mvc;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Services.Interface
{
    public interface IPurchaseOrder
    {
        //Task<ResponseResult> GetItemServiceFORPO(string ItemSErv);
        Task<string> GetItemServiceFORPO(string ItemService);
        Task<ResponseResult> DeleteByID(int ID, int YearCode,int createdBy,string entryByMachineName, string Flag);
        Task<ResponseResult> CheckLockYear(int YearCode);
        Task<ResponseResult> GetExchangeRate(string Currency);
        Task<ResponseResult> FillCurrency(string Ctrl);
        Task<ResponseResult> NewAmmEntryId(int PoAmendYearCode);
        Task<ResponseResult> FillVendors();
        Task<ResponseResult> GetAllPartyName(string CTRL);
        Task<ResponseResult> GetGstRegister(string Flag, int Code);
        Task<PODashBoard> GetUpdAmmData(PODashBoard model);
        Task<ResponseResult> GetItemCode(string PartCode);
        Task<ResponseResult> GetReportName();
        Task<PODashBoard> GetDashBoardData();

        Task<object?> GetMrpYear(string Flag, string MRPNo);
        Task<PODashBoard> GetAmmDashboardData();
        Task<PODashBoard> GetAmmCompletedData(string summaryDetail);
        Task<PurchaseOrderModel> GetViewPOCcompletedByID(int iD, int yC, string PONO, string v);

        Task<ResponseResult> GetQuotData(string Flag, string QuotNo);
        Task<ResponseResult> FillEntryandPONumber(int YearCode);
        Task<ResponseResult> FillItems(string Type, string ShowAllItem);
        Task<ResponseResult> FillPONumber(int YearCode, string Ordertype, string Podate);
        Task<ResponseResult> AltUnitConversion(int ItemCode, decimal AltQty, decimal UnitQty);

        Task<PODashBoard> GetSearchData(PODashBoard model);
        Task<PODashBoard> GetDetailData(PODashBoard model);
        Task<PODashBoard> GetSearchCompData(PODashBoard model);

        Task<PurchaseOrderModel> GetViewByID(int ID, int YearCode, string Flag);
        Task<ResponseResult> GetFormRights(int uId);
        Task<ResponseResult> GetFormRightsAmm(int uId);
        Task<ResponseResult> GetPendQty(string PONo, int POYearCode, int ItemCode, int AccountCode, string SchNo, int SchYearCode, string Flag);
        Task<ResponseResult> FillIndentDetail(string itemName, string partCode, int itemCode);
        Task<ResponseResult> SavePurchaseOrder(DataTable itemDetailDt, DataTable delieveryScheduleDt, DataTable taxDetailDt, DataTable IndentDetailDT, PurchaseOrderModel model);
    }
}