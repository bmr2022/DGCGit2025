using eTactWeb.DOM.Models;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Services.Interface
{
    public interface ISaleRejection
    {
        Task<SaleRejectionModel> FillSaleRejectionGrid(string mrnNo, int mrnEntryId, int mrnYC,int yearCode);
        Task<ResponseResult> SaveSaleRejection(SaleRejectionModel model, DataTable SBGrid, DataTable TaxDetailDT, DataTable DrCrDetailDT, DataTable AdjDetailDT);
        Task<ResponseResult> GetDashboardData(string summaryDetail, string custInvoiceNo, string custName, string mrnNo, string gateNo, string partCode, string itemName, string againstBillNo, string docName, string voucherNo, string fromdate, string toDate);
        Task<SaleRejectionModel> GetViewByID(int ID, string Mode, int YC);
        Task<ResponseResult> FillCustomerName(string fromDate, string toDate);
        Task<ResponseResult> FillItemName(string fromDate, string toDate);
        Task<ResponseResult> FillPartCode(string fromDate, string toDate);
        Task<ResponseResult> FillInvoiceNo(string fromDate, string toDate);
        Task<ResponseResult> FillVoucherNo(string fromDate, string toDate);
        Task<ResponseResult> FillDocument(string fromDate, string toDate);
        Task<ResponseResult> FillAgainstSaleBillNo(string fromDate, string toDate);
        Task<ResponseResult> NewEntryId(int YearCode);
        Task<ResponseResult> DeleteByID(int ID, int YC, int accountCode, int createdBy, string machineName, string cc);
    }
}
