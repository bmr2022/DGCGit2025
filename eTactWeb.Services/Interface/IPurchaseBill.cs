using eTactWeb.DOM.Models;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Services.Interface
{
    public interface IPurchaseBill
    {
        //Task<ResponseResult> GetItemServiceFORPO(string ItemSErv);
        Task<string> GetItemServiceFORPO(string ItemService);
        Task<ResponseResult> DeleteByID(int ID, int YearCode, string Flag, string PurchVoucherNo, string InvNo, int EntryBy, string EntryByMachineName, DateTime EntryDate);
        Task<ResponseResult> CheckLockYear(int YearCode);
        Task<ResponseResult> GetExchangeRate(string Currency);
        Task<ResponseResult> FillCurrency(int? AccountCode);
        Task<ResponseResult> FillDocName(string ShowAll);
        Task<ResponseResult> NewAmmEntryId();
        Task<ResponseResult> GetGstRegister(string Flag, int Code);
        Task<ResponseResult> GetStateGST(string Flag, int Code);
        Task<ResponseResult> GetItemCode(string PartCode);
        Task<ResponseResult> GetReportName();
        Task<PBDashBoard> GetDashBoardData();
        Task<PBListDataModel> GetPurchaseBillListData(string? flag, string? MRNType, string? dashboardtype, string? firstdate, string? todate, PBListDataModel model);
        Task<PurchaseBillModel> GetPurchaseBillItemData(string? flag, string? FlagMRNJWCHALLAN, string? Mrnno, int? mrnyearcode, int? accountcode);
        Task<PurchaseBillModel> GetViewPOCcompletedByID(int iD, int yC,string PONO, string v);
         
        Task<ResponseResult> fillEntryandVouchNo(int YearCode, string VODate);
        Task<ResponseResult> GetAllowDocName();
        Task<ResponseResult> GetAllowInvoiceNo();
        Task<ResponseResult> FillItems(string Type,string ShowAllItem);
        Task<ResponseResult> GetItems(string ShowAll);
        Task<ResponseResult> FillPONumber(int YearCode, string Ordertype, string Podate);
        Task<ResponseResult> GetPOData(string Billdate, int? AccountCode, int? ItemCode);
        Task<ResponseResult> GetScheduleData(string PONo, int? POYear, string Billdate, int? AccountCode, int? ItemCode);
        Task<ResponseResult> FillVouchNumber(int YearCode, string VODate);
        Task<ResponseResult> AltUnitConversion(int ItemCode, decimal AltQty, decimal UnitQty);

        Task<PBDashBoard> GetSummaryData(PBDashBoard model);
        Task<PBDashBoard> GetDetailData(PBDashBoard model);
        Task<PBDashBoard> GetSearchCompData(PBDashBoard model);

        Task<PurchaseBillModel> GetViewByID(int ID, int YearCode, string Flag);
        Task<ResponseResult> GetFormRights(int uId);
         
        Task<ResponseResult> SavePurchaseBILL(DataTable itemDetailDt, DataTable taxDetailDt, DataTable TDSDetailDT, PurchaseBillModel model, DataTable DrCrDetailDT, DataTable AdjDetailDT);
    }
}