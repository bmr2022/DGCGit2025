using eTactWeb.Data.Common;
using eTactWeb.Data.DAL;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.Extensions.Configuration;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Data.BLL;

public class AccountMasterBLL : IAccountMaster
{
    private AccountMasterDAL _AccountMasterDAL;
    private readonly IDataLogic _DataLogicDAL;

    public AccountMasterBLL(IConfiguration config, IDataLogic dataLogicDAL, ConnectionStringService connectionStringService)
    {
        _AccountMasterDAL = new AccountMasterDAL(config, dataLogicDAL, connectionStringService);
        _DataLogicDAL = dataLogicDAL;
    }

    public async Task<ResponseResult> DeleteByID(int ID)
    {
        return await _AccountMasterDAL.DeleteByID(ID);
    }
   
    public async Task<ResponseResult> GetFormRights(int ID)
    {
        return await _AccountMasterDAL.GetFormRights(ID);
    }

    public async Task<AccountMasterModel> GetByID(int ID)
    {
        return await _AccountMasterDAL.GetByID(ID);
    }

    public async Task<AccountMasterModel> GetDashboardData(AccountMasterModel model)
    {
        return await _AccountMasterDAL.GetDashboardData(model);
    }
    public async Task<AccountMasterModel> GetDetailDashboardData(AccountMasterModel model)
    {
        return await _AccountMasterDAL.GetDetailDashboardData(model);
    }

    public async Task<IList<TextValue>> GetDropDownList(string Flag)
    {
        return await _AccountMasterDAL.GetDropDownList(Flag);
    }

    public async Task<ResponseResult> GetParentGroupDetail(string iD)
    {
        return await _AccountMasterDAL.GetParentGroupDetail(iD);
    }

    public async Task<ResponseResult> GetTDSPartyList()
    {
        return await _AccountMasterDAL.GetTDSPartyList();
    }
    public async Task<ResponseResult> GetSalePersonName()
    {
        return await _AccountMasterDAL.GetSalePersonName();
    }
    public async Task<ResponseResult> SaveAccountMaster(AccountMasterModel model)
    {
        return await _AccountMasterDAL.SaveAccountMaster(model);
    }
    public Task<ResponseResult> GetItemGroupCode(string GroupCode)
    {
        return _AccountMasterDAL.GetItemGroupCode(GroupCode);
    }
    public Task<ResponseResult> GetItemCatCode(string CatCode)
    {
        return _AccountMasterDAL.GetItemCatCode(CatCode);
    }
    public async Task<ResponseResult> UpdateMultipleItemDataFromExcel(DataTable ItemDetailGrid, string flag)
    {
        return await _AccountMasterDAL.UpdateMultipleItemDataFromExcel(ItemDetailGrid, flag);
    }
}