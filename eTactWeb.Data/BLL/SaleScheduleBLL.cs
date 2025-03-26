using eTactWeb.Data.DAL;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.Extensions.Configuration;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Data.BLL;

public class SaleScheduleBLL : ISaleSchedule
{
    public SaleScheduleBLL(IConfiguration configuration, IDataLogic iDataLogic)
    {
        _SaleScheduleDAL = new SaleScheduleDAL(configuration, iDataLogic);
    }

    private SaleScheduleDAL? _SaleScheduleDAL { get; }

    public async Task<DataSet> BindAllDropDown()
    {
        return await _SaleScheduleDAL.BindAllDropDown();
    }
    public async Task<ResponseResult> GetFormRights(int ID)
    {
        return await _SaleScheduleDAL.GetFormRights(ID);
    }
    public async Task<ResponseResult> GetFormRightsAmen(int ID)
    {
        return await _SaleScheduleDAL.GetFormRightsAmen(ID);
    }
    public async Task<ResponseResult> DeleteByID(int ID, int YC)
    {
        return await _SaleScheduleDAL.DeleteByID(ID, YC);
    }

    public async Task<ResponseResult> GetDashboardData()
    {
        return await _SaleScheduleDAL.GetDashboardData();
    }

    public async Task<ResponseResult> GetSearchData(SSDashboard model)
    {
        return await _SaleScheduleDAL.GetSearchData(model);
    }

    public async Task<ResponseResult> GetAmmDashboardData(string ToDate)
    {
        return await _SaleScheduleDAL.GetAmmDashboardData(ToDate);
    }
    public async Task<ResponseResult> NewEntryId(int YearCode)
    {
        return await _SaleScheduleDAL.NewEntryId(YearCode);
    }
    public async Task<ResponseResult> NewAmmEntryId()
    {
        return await _SaleScheduleDAL.NewAmmEntryId();
    }
    public async Task<ResponseResult> GetAltUnitQty(int ItemCode, float AltSchQty, float UnitQty)
    {
        return await _SaleScheduleDAL.GetAltUnitQty(ItemCode, AltSchQty, UnitQty);
    }


    public async Task<string> GetSODATA(int AccountCode, int SONO, int Year)
    {
        return await _SaleScheduleDAL.GetSODATA(AccountCode, SONO, Year);
    }

    public async Task<string> GetSOItem(int AccountCode, int SONO, int Year, int ItemCode)
    {
        return await _SaleScheduleDAL.GetSOItem(AccountCode, SONO, Year, ItemCode);
    }

    public async Task<string> GetSONO(int AccountCode)
    {
        return await _SaleScheduleDAL.GetSONO(AccountCode);
    }

    //public Task GetSOYear(int AccountCode, int SONO)
    //{
    //    return _SaleScheduleDAL.GetSOYear(AccountCode, SONO);
    //}
    public async Task<string> GetSOYear(int AccountCode, int SONO)
    {
        return await _SaleScheduleDAL.GetSOYear(AccountCode, SONO);
    }

    public async Task<SaleSubScheduleModel> GetViewByID(int ID, int YearCode, string Mode)
    {
        return await _SaleScheduleDAL.GetViewByID(ID, YearCode, Mode);
    }
    public async Task<SSDashboard> GetAmmSearchData(SSDashboard model)
    {
        return await _SaleScheduleDAL.GetAmmSearchData(model);
    }
    public async Task<SSDashboard> GetUpdAmmData(SSDashboard model)
    {
        return await _SaleScheduleDAL.GetUpdAmmData(model);
    }

    public async Task<ResponseResult> GetAllData(int ID, int YC)
    {
        return await _SaleScheduleDAL.GetAllData(ID, YC);
    }
    public async Task<ResponseResult> GetCurrency(string Flag)
    {
        return await _SaleScheduleDAL.GetCurrency(Flag);
    }
    public async Task<ResponseResult> GetExchangeRate(string Currency)
    {
        return await _SaleScheduleDAL.GetExchangeRate(Currency);
    }
    public async Task<ResponseResult> GetItemCode(string PartCode)
    {
        return await _SaleScheduleDAL.GetItemCode(PartCode);
    }
    public async Task<SSDashboard> GetSOAmmCompleted(string toDt)
    {
        return await _SaleScheduleDAL.GetSOAmmCompleted(toDt);
    }
    public async Task<SaleSubScheduleModel> GetViewSSCcompletedByID(int Id, int YC, int SONO, string Mode)
    {
        return await _SaleScheduleDAL.GetViewSSCcompletedByID(Id, YC, SONO, Mode);
    }

    public async Task<ResponseResult> SaveSaleSchedule(SaleSubScheduleModel model, DataTable DTSSGrid)
    {
        return await _SaleScheduleDAL.SaveSaleSchedule(model, DTSSGrid);
    }
    public async Task<object> GetAmmStatus(int EntryID, int YearCode)
    {
        return await _SaleScheduleDAL.GetAmmStatus(EntryID, YearCode);
    }
}