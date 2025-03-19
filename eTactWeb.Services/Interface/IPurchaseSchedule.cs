using eTactWeb.DOM.Models;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Services.Interface;

public interface IPurchaseSchedule
{
    Task<DataSet> BindAllDropDown();

    Task<ResponseResult> FillMRPNo();
    Task<ResponseResult> FillMRPDetail(string MRPNo);

    Task<ResponseResult> DeleteByID(int ID, int YC, int createdBy, string entryByMachineName);
    Task<ResponseResult> AltUnitConversion(int ItemCode, decimal AltQty, decimal UnitQty);

    Task<ResponseResult> GetDashboardData(string Todate);
    Task<ResponseResult> CheckLockYear( int YearCode);
    Task<ResponseResult> FillEntry( int YearCode);
    Task<ResponseResult> GetAddress(int AccountCode);
    Task<ResponseResult> FillAmendEntry( int YearCode);

    Task<ResponseResult> GetSearchData(PSDashboard model);
    Task<ResponseResult> GetDetailData(PSDashboard model);
    Task<ResponseResult> GetPSAmmData(PSDashboard model);
    Task<ResponseResult> GetUpdPSAmmData(PSDashboard model);

    Task<string> GetPODATA(int AccountCode, string PONO, int Year);

    Task<string> GetPOItem(int AccountCode, string PONO, int Year, int ItemCode);

    Task<string> GetPONO(int AccountCode);
    Task<ResponseResult> GetItemCode(string PartCode);
    Task<ResponseResult> GetExchangeRate(string Currency);
    Task<ResponseResult> GetCurrency(string PONo, int POYearCode, int ItemCode, int AccountCode, string SchNo, int SchYearCode, string Flag);

    Task<string> GetPOYear(int AccountCode, string PONO);

    Task<PurchaseSubScheduleModel> GetViewByID(int ID, int YearCode,string Mode);
    Task<ResponseResult> GetFormRights(int uId);

    Task<ResponseResult> SavePurchSchedule(PurchaseSubScheduleModel model, DataTable DTSSGrid);
}