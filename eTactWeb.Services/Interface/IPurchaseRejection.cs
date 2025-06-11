using eTactWeb.DOM.Models;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Services.Interface
{
    public interface IPurchaseRejection
    {
        Task<AccPurchaseRejectionModel> GetViewByID(int ID, int YearCode, string mode);
        Task<ResponseResult> NewEntryId(int YearCode);
        Task<ResponseResult> FillDocument(string ShowAllDoc);
        Task<ResponseResult> FillCustomerName(string ShowAllParty, int? PurchaseRejYearCode, string DebitNotePurchaseRejection);
        Task<ResponseResult> GetStateGST(int Code);
        Task<ResponseResult> FillItems(int YearCode, int accountCode, string showAllItems, string Flag);
        Task<ResponseResult> GetExchangeRate(string Currency);
        Task<ResponseResult> FillCurrency(int? AccountCode);
        Task<ResponseResult> FillStore();
        Task<ResponseResult> FillSubvoucher(int? PurchaseRejYearCode, string DebitNotePurchaseRejection);
        Task<ResponseResult> GetHSNUNIT(int itemCode);
        Task<ResponseResult> GetCostCenter();
        Task<ResponseResult> FillPurchaseRejectionPopUp(string DebitNotePurchaseRejection, string fromBillDate, string toBillDate, int itemCode, int accountCode, int yearCode, string showAllBill);
        Task<ResponseResult> FillDetailFromPopupGrid(DataTable model, int itemCode, int poopCt);
        Task<ResponseResult> SavePurchaseRejection(AccPurchaseRejectionModel model, DataTable SBGrid, DataTable TaxDetailDT, DataTable DrCrDetailDT, DataTable AdjDetailDT, DataTable DTAgainstBillDetail);
        Task<AccPurchaseRejectionDashboard> GetDashBoardData();
        Task<AccPurchaseRejectionDashboard> GetSearchData(AccPurchaseRejectionDashboard model);
        Task<ResponseResult> CheckLockYear(int YearCode);
        Task<ResponseResult> GetFormRights(int uId);
        Task<ResponseResult> DeleteByID(int ID, int YearCode, string Flag, string VoucherNo, string CC, int AccountCode, string InvNo, int EntryBy, string EntryByMachineName, DateTime EntryDate);
    }
}
