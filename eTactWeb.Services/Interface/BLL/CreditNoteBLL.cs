using eTactWeb.Data.DAL;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Data.BLL
{
    public class CreditNoteBLL : ICreditNote
    {
        private readonly IDataLogic _iDtaLogic;
        private readonly CreditNoteDAL _creditNoteDAL;

        public CreditNoteBLL(IConfiguration configuration, IDataLogic iDtaLogic)
        {
            _iDtaLogic = iDtaLogic;
            _creditNoteDAL = new CreditNoteDAL(configuration, iDtaLogic);
        }

        public async Task<AccCreditNoteModel> GetViewByID(int ID, int YearCode)
        {
            return await _creditNoteDAL.GetViewByID(ID,YearCode);
        }
        public async Task<ResponseResult> GetHSNUNIT(int itemCode)
        {
            return await _creditNoteDAL.GetHSNUNIT(itemCode);
        }
        public async Task<ResponseResult> NewEntryId(int YearCode)
        {
            return await _creditNoteDAL.NewEntryId(YearCode);
        }
        public async Task<ResponseResult> FillDetailFromPopupGrid(DataTable model,int itemCode,int popCt)
        {
            return await _creditNoteDAL.FillDetailFromPopupGrid(model,itemCode,popCt);
        }
        public async Task<ResponseResult> FillCustomerName(string againstSalePurchase)
        {
            return await _creditNoteDAL.FillCustomerName(againstSalePurchase);
        }
        public async Task<ResponseResult> FillCreditNotePopUp(string againstSalePurchase, string fromBillDate, string toBillDate,int itemCode, int accountCode, int yearCode,string showAllBill)
        {
            return await _creditNoteDAL.FillCreditNotePopUp( againstSalePurchase,  fromBillDate,  toBillDate,itemCode, accountCode,  yearCode, showAllBill);
        }
        public async Task<ResponseResult> FillItems(string fromSaleBillDate, string toSaleBillDate,int accountCode, string showAllItems)
        {
            return await _creditNoteDAL.FillItems(fromSaleBillDate,toSaleBillDate,accountCode,showAllItems);
        }
        public async Task<ResponseResult> GetCostCenter()
        {
            return await _creditNoteDAL.GetCostCenter();
        }
    }
}
