using eTactWeb.DOM.Models;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Services.Interface;

public interface ISaleSchedule
{
    Task<DataSet> BindAllDropDown();

    Task<ResponseResult> DeleteByID(int ID, int YC);

    Task<ResponseResult> GetDashboardData();
    Task<ResponseResult> GetAmmDashboardData(string ToDate);

    Task<ResponseResult> GetSearchData(SSDashboard model);
    Task<ResponseResult> NewEntryId(int YearCode);
    Task<ResponseResult> NewAmmEntryId();
    Task<ResponseResult> GetAltUnitQty(int ItemCode, float AlySchQty, float UnitQty);
    Task<string> GetSODATA(int AccountCode, int SONO, int Year);

    Task<string> GetSOItem(int AccountCode, int SONO, int Year, int ItemCode);

    Task<string> GetSONO(int AccountCode);
    Task<ResponseResult> GetFormRights(int uId);
    Task<ResponseResult> GetFormRightsAmen(int uId);
    Task<ResponseResult> FillCustomer(string SchEffFromDate);
    Task<ResponseResult> FillCustomerOrderNo(int AccountCode, string SchEffFromDate);
    Task<string> GetSOYear(int AccountCode, int SONO);
    Task<SaleSubScheduleModel> GetViewByID(int ID, int YearCode, string Mode);
    Task<SSDashboard> GetAmmSearchData(SSDashboard model);
    Task<SSDashboard> GetUpdAmmData(SSDashboard model);
    Task<ResponseResult> GetAllData(int ID, int YC);
    Task<ResponseResult> GetCurrency(string Flag);
    Task<ResponseResult> GetExchangeRate(string Currency);
    Task<ResponseResult> GetItemCode(string PartCode);
    Task<SSDashboard> GetSOAmmCompleted(string toDt);
    Task<SaleSubScheduleModel> GetViewSSCcompletedByID(int Id, int YC, int SONO, string Mode);
    Task<ResponseResult> SaveSaleSchedule(SaleSubScheduleModel model, DataTable DTSSGrid);
    Task<object> GetAmmStatus(int EntryID, int YearCode);
}
