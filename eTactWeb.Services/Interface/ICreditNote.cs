using eTactWeb.DOM.Models;
using Microsoft.AspNetCore.Mvc;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Services.Interface
{
    public interface ICreditNote
    {
        Task<AccCreditNoteModel> GetViewByID(int ID, int YearCode,string mode);
        Task<ResponseResult> GetHSNUNIT(int itemCode);
        Task<ResponseResult> FillDetailFromPopupGrid(DataTable model,int itemCode,int poopCt);
        Task<ResponseResult> NewEntryId(int YearCode, string CreditNoteVoucherDate, string SubVoucherName);
        Task<ResponseResult> FillCustomerName(string againstSalePurchase);
        Task<ResponseResult> FillCreditNotePopUp(string againstSalePurchase, string fromBillDate, string toBillDate,int itemCode, int accountCode, int yearCode,string showAllBill);
        Task<ResponseResult> FillItems(string fromSaleBillDate, string toSaleBillDate,int accountCode, string showAllItems);
        Task<ResponseResult> GetCostCenter();
        Task<ResponseResult> GetFormRights(int uId);
        Task<ResponseResult> SaveCreditNote(AccCreditNoteModel model, DataTable SBGrid, DataTable TaxDetailDT, DataTable DrCrDetailDT, DataTable AdjDetailDT,DataTable DTAgainstBillDetail);
        Task<ResponseResult> GetDashboardData(string summaryDetail,string fromdate, string toDate);
        Task<ResponseResult> DeleteByID(int ID, int YC,int accountCode, string machineName);
        Task<ResponseResult> FillSubVoucher();
        Task<AccCreditNoteDashboard> GetDashboardData(string FromDate, string ToDate, string ItemName, string PartCode, string CustomerName, string CreditNoteInvoiceNumber, string CreditNoteVoucherNo, string DashboardType);
    }
}
