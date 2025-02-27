using eTactWeb.Data.DAL;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.Extensions.Configuration;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Data.BLL;

public class SaleOrderBLL : ISaleOrder
{
    private SaleOrderDAL _SaleOrderDAL;
    private readonly IDataLogic _DataLogicDAL;


    public SaleOrderBLL(IConfiguration configuration, IDataLogic dataLogicDAL)
    {
        _SaleOrderDAL = new SaleOrderDAL(configuration, dataLogicDAL);
        _DataLogicDAL = dataLogicDAL;
    }

    public async Task<ResponseResult> DeleteByID(int ID, int YearCode, string Flag)
    {
        return await _SaleOrderDAL.DeleteByID(ID, YearCode, Flag);
    }

    public async Task<ResponseResult> GetAddress(string Code)
    {
        return await _SaleOrderDAL.GetAddress(Code);
    }
    public async Task<ResponseResult> GetFillCurrency(string CTRL)
    {
        return await _SaleOrderDAL.GetFillCurrency(CTRL);
    }
    public async Task<ResponseResult> GetFormRights(int userID)
    {
        return await _SaleOrderDAL.GetFormRights(userID);
    }
    public async Task<ResponseResult> NewEntryId(int YearCode)
    {
        return await _SaleOrderDAL.NewEntryId(YearCode);
    }
    public async Task<ResponseResult> NewAmmEntryId()
    {
        return await _SaleOrderDAL.NewAmmEntryId();
    }
    public async Task<SaleOrderDashboard> GetAmmDashboardData()
    {
        return await _SaleOrderDAL.GetAmmDashboardData();
    }

    public async Task<SaleOrderDashboard> GetAmmSearchData(SaleOrderDashboard model)
    {
        return await _SaleOrderDAL.GetAmmSearchData(model);
    }
    public async Task<SaleOrderDashboard> GetUpdAmmData(SaleOrderDashboard model)
    {
        return await _SaleOrderDAL.GetUpdAmmData(model);
    }
    public async Task<ResponseResult> GetTotalStockList(int store, int Itemcode)
    {
        return await _SaleOrderDAL.GetTotalStockList(store, Itemcode);
    }
    public async Task<ResponseResult> GetAllowMultiBuyerProp()
    {
        return await _SaleOrderDAL.GetAllowMultiBuyerProp();
    }
    public async Task<ResponseResult> GetCurrency(string Currency)
    {
        return await _SaleOrderDAL.GetCurrency(Currency);
    }
    public async Task<ResponseResult> GetAltQty(int ItemCode, float UnitQty, float ALtQty)
    {
        return await _SaleOrderDAL.GetAltQty(ItemCode, UnitQty, ALtQty);
    }
    public async Task<ResponseResult> GetLockedYear(int YearCode)
    {
        return await _SaleOrderDAL.GetLockedYear(YearCode);
    }

    public async Task<object> GetAmmStatus(int EntryID, int YearCode)
    {
        return await _SaleOrderDAL.GetAmmStatus(EntryID, YearCode);
    }

    public async Task<ResponseResult> GetCurrencyDetail(string CurrentDate, string Currency)
    {
        return await _SaleOrderDAL.GetCurrencyDetail(CurrentDate, Currency);
    }

    public async Task<SaleOrderDashboard> GetDashboardData(string EndDate)
    {
        return await _SaleOrderDAL.GetDashboardData(EndDate);
    }

    public async Task<ResponseResult> GetItemPartCode(string Code, string Flag)
    {
        return await _SaleOrderDAL.GetItemPartCode(Code, Flag);
    }

    public async Task<ResponseResult> GetPartyList(string Check)
    {
        return await _SaleOrderDAL.GetPartyList(Check);
    }
    public async Task<ResponseResult> GetItemCode(string PartCode)
    {
        return await _SaleOrderDAL.GetItemCode(PartCode);
    }

    public async Task<ResponseResult> GetQuotData(string Code, string Flag)
    {
        return await _SaleOrderDAL.GetQuotData(Code, Flag);
    }

    public async Task<SaleOrderDashboard> GetSearchData(SaleOrderDashboard model)
    {
        return await _SaleOrderDAL.GetSearchData(model);
    }

    public async Task<SaleOrderDashboard> GetSOAmmCompleted()
    {
        return await _SaleOrderDAL.GetSOAmmCompleted();
    }

    public async Task<SaleOrderDashboard> GetSOAmmCompletedSearchData(SaleOrderDashboard model)
    {
        return await _SaleOrderDAL.GetSOAmmCompletedSearchData(model);
    }

    public async Task<SaleOrderModel> GetViewByID(int ID, int YearCode, string Flag)
    {
        return await _SaleOrderDAL.GetViewByID(ID, YearCode, Flag);
    }

    public async Task<SaleOrderModel> GetViewSOCcompletedByID(int ID, int YearCode, string Flag)
    {
        return await _SaleOrderDAL.GetViewSOCcompletedByID(ID, YearCode, Flag);
    }

    public async Task<ResponseResult> SaveSaleOrder(DataTable DTItemGrid, DataTable DTSchedule, DataTable DTTaxGrid, DataTable MultiBuyersDT, SaleOrderModel model)
    {
        return await _SaleOrderDAL.SaveSaleOrder(DTItemGrid, DTSchedule, DTTaxGrid, MultiBuyersDT, model);
    }
}
