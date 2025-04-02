using eTactWeb.Data.Common;
using eTactWeb.Data.DAL;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.Extensions.Configuration;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Data.BLL;

public class PartCodePartyWiseBLL : IPartCodePartyWise
{
    private readonly IDataLogic _DataLogicDAL;
    private PartCodePartyWiseDAL _PartCodePartyWiseDAL;

    public PartCodePartyWiseBLL(IConfiguration configuration, IDataLogic dataLogicDAL, ConnectionStringService connectionStringService)
    {
        _DataLogicDAL = dataLogicDAL;
        _PartCodePartyWiseDAL = new PartCodePartyWiseDAL(configuration,dataLogicDAL, connectionStringService);
    }

    public async Task<ResponseResult> FillItems(string Type, string ShowAllItem)
    {
        return await _PartCodePartyWiseDAL.FillItems(Type, ShowAllItem);
    }
    public async Task<ResponseResult> FillPartCode(string Type, string ShowAllItem)
    {
        return await _PartCodePartyWiseDAL.FillPartCode(Type, ShowAllItem);
    }
    public async Task<ResponseResult> FillAccountName(string Type)
    {
        return await _PartCodePartyWiseDAL.FillAccountName(Type);
    }
    public async Task<PartCodePartyWiseModel> GetListForUpdate(int ItemCode)
    {
        return await _PartCodePartyWiseDAL.GetListForUpdate(ItemCode);
    }
    public async Task<ResponseResult> DeleteByID(int ItemCode,string EntryByMachineName)
    {
        return await _PartCodePartyWiseDAL.DeleteByID(ItemCode, EntryByMachineName);
    }
    public string GetUnit(int Itemcode, string Flag)
    {
        return _PartCodePartyWiseDAL.GetUnit(Itemcode, Flag);
    }
    public async  Task<PartCodePartyWiseDashboard> GetDashboardData(string ItemName, string PartCode, string AccountName, string CustvendPartCode, string CustvendItemName, string DashboardType)
    {
        return await _PartCodePartyWiseDAL.GetDashboardData(ItemName, PartCode, AccountName, CustvendPartCode, CustvendItemName,DashboardType);
    }
    public async Task<ResponseResult> GetDashboardData()
    {
        return await _PartCodePartyWiseDAL.GetDashboardData();
    }
    public async Task<ResponseResult> SavePartCodePartWise(PartCodePartyWiseModel model, DataTable DT)
    {
        //throw new NotImplementedException();
        return await _PartCodePartyWiseDAL.SavePartCodePartWise(model, DT);
    }
    public async Task<PartCodePartyWiseModel> GetViewByID(int ItemCode)
    {
        return await _PartCodePartyWiseDAL.GetViewByID(ItemCode);
    }
    public async Task<ResponseResult> GetFormRights(int ID)
    {
        return await _PartCodePartyWiseDAL.GetFormRights(ID);
    }
   
}