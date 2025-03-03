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
        Task<ResponseResult> FillItems(int YearCode, int accountCode, string showAllItems);
        Task<ResponseResult> GetExchangeRate(string Currency);
        Task<ResponseResult> FillCurrency(int? AccountCode);
        Task<ResponseResult> FillStore();
        Task<ResponseResult> FillSubvoucher(int? PurchaseRejYearCode, string DebitNotePurchaseRejection);
        Task<ResponseResult> GetHSNUNIT(int itemCode);
    }
}
