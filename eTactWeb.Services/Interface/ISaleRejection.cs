using eTactWeb.DOM.Models;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Services.Interface
{
	public interface ISaleRejection
	{
		Task<SaleRejectionModel> FillSaleRejectionGrid(string mrnNo, int mrnEntryId, int mrnYC, int yearCode);
		Task<ResponseResult> SaveSaleRejection(SaleRejectionModel model, DataTable SBGrid, DataTable TaxDetailDT, DataTable DrCrDetailDT, DataTable AdjDetailDT);
		Task<ResponseResult> GetDashboardData(string fromDate, string toDate, string custInvoiceNo, int AccountCode, string mrnNo, string gateNo, int ItemCode, string againstBillNo, string voucherNo, string summaryDetail, string searchBox);
		Task<SaleRejectionModel> GetViewByID(int ID, string Mode, int YC);
		Task<ResponseResult> FillCustomerName(string fromDate, string toDate);
		Task<ResponseResult> GetFormRights(int uId);
		Task<ResponseResult> FillSubvoucher();
		Task<ResponseResult> FillItemName(string fromDate, string toDate);
		Task<ResponseResult> FillPartCode(string fromDate, string toDate);
		Task<ResponseResult> FillGateNo(string fromDate, string toDate);
		Task<ResponseResult> FillMRNNo(string fromDate, string toDate);
		Task<ResponseResult> FillInvoiceNo(string fromDate, string toDate);
		Task<ResponseResult> FillVoucherNo(string fromDate, string toDate);
		Task<ResponseResult> FillDocument(string fromDate, string toDate);
		Task<ResponseResult> FillAgainstSaleBillNo(string fromDate, string toDate);
		Task<ResponseResult> NewEntryId(int YearCode, string SaleRejEntryDate, string SubVoucherName);
		Task<ResponseResult> GetTotalAmount(SaleRejectionFilter model);
		Task<ResponseResult> DeleteByID(int ID, int YC, int accountCode, int createdBy, string machineName, string cc);
        Task<List<SaleRejectionDetail>> GetSaleBillData(string saleBillNo, int AccountCode);
        Task<ResponseResult> GETSaleBillNo(int AccountCode, string SaleBillNo);

    }
}
