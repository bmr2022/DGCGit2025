using eTactWeb.Data.DAL;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.Extensions.Configuration;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Data.BLL;

public class BankMasterBLL : IBankMaster
{
    private BankMasterDAL _BankMasterDAL;
    private readonly IDataLogic _DataLogicDAL;

    public BankMasterBLL(IConfiguration config, IDataLogic dataLogicDAL)
    {
        _BankMasterDAL = new BankMasterDAL(config, dataLogicDAL);
        _DataLogicDAL = dataLogicDAL;
    }

    public async Task<ResponseResult> DeleteByID(int ID)
    {
        return await _BankMasterDAL.DeleteByID(ID);
    }
    public async Task<ResponseResult> GetFormRights(int ID)
    {
        return await _BankMasterDAL.GetFormRights(ID);
    }

    public async Task<BankMasterModel> GetByID(int ID)
    {
        return await _BankMasterDAL.GetByID(ID);
    }

    public async Task<BankMasterModel> GetDashboardData(BankMasterModel model)
    {
        return await _BankMasterDAL.GetDashboardData(model);
    }
    public async Task<BankMasterModel> GetDetailDashboardData(BankMasterModel model)
    {
        return await _BankMasterDAL.GetDetailDashboardData(model);
    }

    public async Task<IList<TextValue>> GetDropDownList(string Flag)
    {
        return await _BankMasterDAL.GetDropDownList(Flag);
    }

    public async Task<ResponseResult> GetParentGroupDetail(string iD)
    {
        return await _BankMasterDAL.GetParentGroupDetail(iD);
    }

    public async Task<ResponseResult> GetTDSPartyList()
    {
        return await _BankMasterDAL.GetTDSPartyList();
    }

    public async Task<ResponseResult> SaveAccountMaster(BankMasterModel model)
    {
        return await _BankMasterDAL.SaveAccountMaster(model);
    }
}