using eTactWeb.Data.DAL;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.Extensions.Configuration;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Data.BLL
{
    public class PurchaseRejectionBLL : IPurchaseRejection
    {
        private readonly IDataLogic _iDtaLogic;
        private readonly PurchaseRejectionDAL _purchRejDAL;

        public PurchaseRejectionBLL(IConfiguration configuration, IDataLogic iDtaLogic)
        {
            _iDtaLogic = iDtaLogic;
            _purchRejDAL = new PurchaseRejectionDAL(configuration, iDtaLogic);
        }

        public async Task<AccPurchaseRejectionModel> GetViewByID(int ID, int YearCode, string mode)
        {
            return await _purchRejDAL.GetViewByID(ID, YearCode, mode);
        }
        public async Task<ResponseResult> NewEntryId(int YearCode)
        {
            return await _purchRejDAL.NewEntryId(YearCode);
        }
        public async Task<ResponseResult> FillDocument(string ShowAllDoc)
        {
            return await _purchRejDAL.FillDocument(ShowAllDoc);
        }
        public async Task<ResponseResult> FillCustomerName(string ShowAllParty, int? PurchaseRejYearCode, string DebitNotePurchaseRejection)
        {
            return await _purchRejDAL.FillCustomerName(ShowAllParty, PurchaseRejYearCode, DebitNotePurchaseRejection);
        }
        public async Task<ResponseResult> GetStateGST(int Code)
        {
            return await _purchRejDAL.GetStateGST(Code);
        }
        public async Task<ResponseResult> FillItems(int YearCode, int accountCode, string showAllItems)
        {
            return await _purchRejDAL.FillItems(YearCode, accountCode, showAllItems);
        }
        public async Task<ResponseResult> FillCurrency(int? AccountCode)
        {
            return await _purchRejDAL.FillCurrency(AccountCode);
        }
        public async Task<ResponseResult> GetExchangeRate(string Currency)
        {
            return await _purchRejDAL.GetExchangeRate(Currency);
        }
        public async Task<ResponseResult> FillStore()
        {
            return await _purchRejDAL.FillStore();
        }
        public async Task<ResponseResult> FillSubvoucher(int? PurchaseRejYearCode, string DebitNotePurchaseRejection)
        {
            return await _purchRejDAL.FillSubvoucher(PurchaseRejYearCode, DebitNotePurchaseRejection);
        }
        public async Task<ResponseResult> GetHSNUNIT(int itemCode)
        {
            return await _purchRejDAL.GetHSNUNIT(itemCode);
        }
        public async Task<ResponseResult> FillPurchaseRejectionPopUp(string DebitNotePurchaseRejection, string fromBillDate, string toBillDate, int itemCode, int accountCode, int yearCode, string showAllBill)
        {
            return await _purchRejDAL.FillPurchaseRejectionPopUp(DebitNotePurchaseRejection, fromBillDate, toBillDate, itemCode, accountCode, yearCode, showAllBill);
        }
    }
}
