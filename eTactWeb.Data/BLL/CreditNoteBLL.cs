using eTactWeb.Data.Common;
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

        public CreditNoteBLL(IConfiguration configuration, IDataLogic iDtaLogic, ConnectionStringService connectionStringService)
        {
            _iDtaLogic = iDtaLogic;
            _creditNoteDAL = new CreditNoteDAL(configuration, iDtaLogic, connectionStringService);
        }

        public async Task<AccCreditNoteModel> GetViewByID(int ID, int YearCode,string mode)
        {
            return await _creditNoteDAL.GetViewByID(ID,YearCode,mode);
        }
        public async Task<ResponseResult> GetHSNUNIT(int itemCode)
        {
            return await _creditNoteDAL.GetHSNUNIT(itemCode);
        }
        public async Task<ResponseResult> NewEntryId(int YearCode, string CreditNoteVoucherDate, string SubVoucherName)
        {
            return await _creditNoteDAL.NewEntryId(YearCode,CreditNoteVoucherDate,SubVoucherName);
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
        public async Task<ResponseResult> SaveCreditNote(AccCreditNoteModel model, DataTable SBGrid, DataTable TaxDetailDT, DataTable DrCrDetailDT, DataTable AdjDetailDT, DataTable DTAgainstBillDetail)
        {
            return await _creditNoteDAL.SaveCreditNote(model, SBGrid, TaxDetailDT, DrCrDetailDT, AdjDetailDT,DTAgainstBillDetail);
        }
        public async Task<ResponseResult> GetDashboardData(string summaryDetail,string fromdate, string toDate)
        {
            return await _creditNoteDAL.GetDashboardData(summaryDetail,fromdate,toDate);
        }
        public async Task<ResponseResult> DeleteByID(int ID, int YC, int accountCode, string entryByMachineName)
        {
            return await _creditNoteDAL.DeleteByID(ID,YC,accountCode,entryByMachineName);
        }
    }
}
