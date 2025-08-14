using eTactWeb.DOM.Models;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Services.Interface
{
    public interface ISaleOrder
    {
        Task<ResponseResult> DeleteByID(int ID, int YearCode, string Flag);
		

		Task<ResponseResult> GetAddress(string Code);

        Task<ResponseResult> CheckOrderNo(int year,int accountcode,int entryid, string custorderno);
        Task<ResponseResult> GetFillCurrency(string CTRL);
        Task<SaleOrderModel> ShowGroupWiseItems(int Group_Code,int AccountCode);
        Task<ResponseResult> getdiscCategoryName(int Group_Code, int AccountCode);
        Task<string> GetSOItem(int AccountCode, int SONO, int Year, int ItemCode);


		Task<SaleOrderDashboard> GetAmmDashboardData();
        Task<ResponseResult> GetTotalStockList(int store, int Itemcode);
        Task<ResponseResult> GetAllowMultiBuyerProp();
        Task<ResponseResult> GetCurrency(string Currency);
        Task<ResponseResult> GetAltQty(int ItemCode, float UnitQty, float ALtQty);
        Task<ResponseResult> GetLockedYear(int YearCode);
        Task<SaleOrderDashboard> GetAmmSearchData(SaleOrderDashboard model);
        Task<SaleOrderDashboard> GetUpdAmmData(SaleOrderDashboard model);

        Task<object> GetAmmStatus(int EntryID, int YearCode);

        Task<ResponseResult> GetCurrencyDetail(string CurrentDate, string Currency);

        Task<SaleOrderDashboard> GetDashboardData(string EndDate);
        Task<ResponseResult> GetFormRights(int userID);
        Task<ResponseResult> GetFormRightsAmm(int userID);
        Task<ResponseResult> NewEntryId(int YearCode);
        Task<ResponseResult> NewAmmEntryId(int YearCode);

        Task<ResponseResult> GetItemPartCode(string Code, string Flag);
        Task<ResponseResult> GetItemGroup();
        Task<ResponseResult> GETGROUPWISEITEM(int Group_Code);

		Task<ResponseResult> GetExcelData(string Code);

		Task<ResponseResult> GetPartyList(string Check);
        Task<ResponseResult> GetItemCode(string PartCode);

        Task<ResponseResult> GetQuotData(string Code, string Flag);

        Task<SaleOrderDashboard> GetSearchData(SaleOrderDashboard model);

        Task<SaleOrderDashboard> GetSOAmmCompleted();

        Task<SaleOrderDashboard> GetSOAmmCompletedSearchData(SaleOrderDashboard model);

        Task<SaleOrderModel> GetViewByID(int ID, int YearCode, string Flag);

        Task<SaleOrderModel> GetViewSOCcompletedByID(int iD, int yC, string v);

        Task<ResponseResult> SaveSaleOrder(DataTable DTItemGrid, DataTable DTSchedule, DataTable DTTaxGrid, DataTable MultiBuyersDT, SaleOrderModel model);
    }
}
