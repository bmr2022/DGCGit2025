using eTactWeb.DOM.Models;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Services.Interface
{
    public interface IPurchaseRejection
    {
        Task<AccPurchaseRejectionModel> GetViewByID(int ID, int YearCode, string mode);
        Task<ResponseResult> NewEntryId(int YearCode);
        Task<ResponseResult> FillDocument(string ShowAllDoc);
        Task<ResponseResult> FillCustomerName(string ShowAllParty, int? PurchaseRejYearCode);
        Task<ResponseResult> GetStateGST(int Code);
    }
}
