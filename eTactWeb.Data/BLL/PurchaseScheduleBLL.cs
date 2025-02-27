using eTactWeb.Data.DAL;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.Extensions.Configuration;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Data.BLL;

public class PurchaseScheduleBLL : IPurchaseSchedule
{
    public PurchaseScheduleBLL(IConfiguration configuration, IDataLogic iDataLogic)
    {
        _PurchaseScheduleDAL = new PurchaseScheduleDAL(configuration, iDataLogic);
    }

    private PurchaseScheduleDAL? _PurchaseScheduleDAL { get; }
    //private PurchaseScheduleDAL? _PurchaseScheduleDAL { get; }

    public async Task<DataSet> BindAllDropDown()
    {
        return await _PurchaseScheduleDAL.BindAllDropDown();
    }
    public async Task<ResponseResult> GetFormRights(int ID)
    {
        return await _PurchaseScheduleDAL.GetFormRights(ID);
    }

    public async Task<ResponseResult> DeleteByID(int ID, int YC, int createdBy, string entryByMachineName)
    {
        return await _PurchaseScheduleDAL.DeleteByID(ID, YC,createdBy,entryByMachineName);
    }
    public async Task<ResponseResult> AltUnitConversion(int ItemCode, decimal AltQty, decimal UnitQty)
    {
        return await _PurchaseScheduleDAL.AltUnitConversion(ItemCode,AltQty,UnitQty);
    }

    public async Task<ResponseResult> GetDashboardData(string ToDate)
    {
        return await _PurchaseScheduleDAL.GetDashboardData(ToDate);
    }
    public async Task<ResponseResult> CheckLockYear(int YearCode)
    {
        return await _PurchaseScheduleDAL.CheckLockYear(YearCode);
    }
    public async Task<ResponseResult> FillEntry(int YearCode)
    {
        return await _PurchaseScheduleDAL.FillEntry(YearCode);
    }
    public async Task<ResponseResult> GetAddress(int AccountCode)
    {
        return await _PurchaseScheduleDAL.GetAddress(AccountCode);
    }
    public async Task<ResponseResult> FillAmendEntry(int YearCode)
    {
        return await _PurchaseScheduleDAL.FillAmendEntry(YearCode);
    }
     
    public async Task<string> GetPOYear(int AccountCode, string PONO)
    {
        //throw new NotImplementedException();
        return await _PurchaseScheduleDAL.GetPOYear(AccountCode,PONO);
    }

    public async Task<ResponseResult> GetSearchData(PSDashboard model)
    {
        return await _PurchaseScheduleDAL.GetSearchData(model);
    }
    public async Task<ResponseResult> GetDetailData(PSDashboard model)
    {
        return await _PurchaseScheduleDAL.GetDetailData(model);
    }
    public async Task<ResponseResult> GetPSAmmData(PSDashboard model)
    {
        return await _PurchaseScheduleDAL.GetPSAmmData(model);
    }
    public async Task<ResponseResult> GetUpdPSAmmData(PSDashboard model)
    {
        return await _PurchaseScheduleDAL.GetUpdPSAmmData(model);
    }

    public async Task<string> GetPODATA(int AccountCode, string PONO, int Year)
    {
        return await _PurchaseScheduleDAL.GetPODATA(AccountCode, PONO, Year);
    }

    public async Task<string> GetPOItem(int AccountCode, string PONO, int Year, int ItemCode)
    {
        return await _PurchaseScheduleDAL.GetPOItem(AccountCode, PONO, Year, ItemCode);
    }

    public async Task<string> GetPONO(int AccountCode)
    {
        return await _PurchaseScheduleDAL.GetPONO(AccountCode);
    }

    //public Task GetSOYear(int AccountCode, int SONO)
    //{
    //    return _PurchaseScheduleDAL.GetSOYear(AccountCode, SONO);
    //} 

    public async Task<PurchaseSubScheduleModel> GetViewByID(int ID, int YearCode,string Mode)
    {
        return await _PurchaseScheduleDAL.GetViewByID(ID, YearCode,Mode);
    }

    public async Task<ResponseResult> SavePurchSchedule(PurchaseSubScheduleModel model, DataTable DTSSGrid)
    {
        return await _PurchaseScheduleDAL.SavePurchaseSchedule(model, DTSSGrid);
    }
    public async Task<ResponseResult> GetItemCode(string PartCode)
    {
        return await _PurchaseScheduleDAL.GetItemCode(PartCode);
    }
    public async Task<ResponseResult> GetExchangeRate(string Currency)
    {
        return await _PurchaseScheduleDAL.GetExchangeRate(Currency);
    }
    public async Task<ResponseResult> GetCurrency(string PONo, int POYearCode, int ItemCode, int AccountCode, string SchNo, int SchYearCode,string Flag)
    {
        return await _PurchaseScheduleDAL.GetCurrency(PONo, POYearCode, ItemCode, AccountCode, SchNo, SchYearCode, Flag);
    }

     
}