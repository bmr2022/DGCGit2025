using eTactWeb.DOM.Models;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Services.Interface
{
    public interface IDirectPurchaseBill
    {
        //Task<ResponseResult> GetItemServiceFORPO(string ItemSErv);
        Task<string> GetItemServiceFORPO(string ItemService);
        Task<ResponseResult> DeleteByID(int ID, int YearCode, string Flag, string PurchVoucherNo, string InvNo, int EntryBy, string EntryByMachineName, DateTime EntryDate);
        Task<ResponseResult> CheckLockYear(int YearCode);
        Task<ResponseResult> CheckEditOrDelete(int ID,int YearCode);
        Task<ResponseResult> GetExchangeRate(string Currency);
        Task<ResponseResult> FillCurrency(string Ctrl);
        Task<ResponseResult> NewAmmEntryId();
        Task<ResponseResult> GetGstRegister(string Flag, int Code);
        Task<ResponseResult> GetStateGST(string Flag, int Code);
        Task<ResponseResult> GetItemCode(string PartCode);
        Task<ResponseResult> GetReportName();
        Task<DPBDashBoard> GetDashBoardData();
          
        Task<DirectPurchaseBillModel> GetViewPOCcompletedByID(int iD, int yC,string PONO, string v);
         
        Task<ResponseResult> FillEntryandVouchNoNumber(int YearCode, string VODate);
        Task<ResponseResult> FillItems(string Type,string ShowAllItem);
        Task<ResponseResult> FILLDocumentList(string ShowAll);
        Task<ResponseResult> FillPONumber(int YearCode, string Ordertype, string Podate);
        Task<ResponseResult> GetPOData(string Billdate, int? AccountCode, int? ItemCode);
        Task<ResponseResult> GetScheduleData(string PONo, int? POYear, string Billdate, int? AccountCode, int? ItemCode);
        Task<ResponseResult> FillVouchNumber(int YearCode, string VODate);
        Task<ResponseResult> AltUnitConversion(int ItemCode, decimal AltQty, decimal UnitQty);

        Task<DPBDashBoard> GetSummaryData(DPBDashBoard model);
        Task<DPBDashBoard> GetDetailData(DPBDashBoard model);
        Task<DPBDashBoard> GetSearchCompData(DPBDashBoard model);

        Task<DirectPurchaseBillModel> GetViewByID(int ID, int YearCode, string Flag);
        Task<ResponseResult> GetFormRights(int uId);
        Task<ResponseResult> CheckDuplicateEntry(int YearCode, int AccountCode, string InvNo, int EntryId);
        Task<ResponseResult> SaveDirectPurchaseBILL(DataTable itemDetailDt, DataTable taxDetailDt, DataTable TDSDetailDT, DirectPurchaseBillModel model, DataTable DrCrDetailDT, DataTable AdjDetailDT);
    }
}