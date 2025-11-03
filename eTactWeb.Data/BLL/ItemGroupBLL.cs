using eTactWeb.Data.Common;
using eTactWeb.Data.DAL;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.Extensions.Configuration;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Data.BLL;

public class ItemGroupBLL : IItemGroup
{
    private ItemGroupDAL _ItemGroupDAL;
    private readonly IDataLogic _DataLogicDAL;

    public ItemGroupBLL(IConfiguration config, IDataLogic dataLogicDAL, ConnectionStringService connectionStringService)
    {
        _ItemGroupDAL = new ItemGroupDAL(config, dataLogicDAL,connectionStringService);
        _DataLogicDAL = dataLogicDAL;
    }

    public async Task<ResponseResult> DeleteByID(int ID)
    {
        return await _ItemGroupDAL.DeleteByID(ID);
    }
    public async Task<ResponseResult> GetFormRights(int ID)
    {
        return await _ItemGroupDAL.GetFormRights(ID);
    }
    public async Task<ResponseResult> GetUnderCategory(string Mode, string Type)
    {
        return await _ItemGroupDAL.GetUnderCategory(Mode, Type);
    }
    public async Task<ResponseResult> CheckBeforeUpdate(int GroupCode)
    {
        return await _ItemGroupDAL.CheckBeforeUpdate(GroupCode);
    }
    public async Task<ResponseResult> GetAllItemGroup()
    {
        return await _ItemGroupDAL.GetAllItemGroup();
    }

    public async Task<ItemGroupModel> GetByID(int ID)
    {
        return await _ItemGroupDAL.GetByID(ID);
    }
    public Task<ResponseResult> GetItemCatCode(string CatCode)
    {
        return _ItemGroupDAL.GetItemCatCode(CatCode);
    }
    public async Task<ResponseResult> UpdateMultipleItemDataFromExcel(DataTable ItemDetailGrid, string flag)
    {
        return await _ItemGroupDAL.UpdateMultipleItemDataFromExcel(ItemDetailGrid, flag);
    }
    public async Task<ItemGroupModel> GetDashboardData(ItemGroupModel model)
    {
        return await _ItemGroupDAL.GetDashboardData(model);
    }
    public async Task<IList<TextValue>> GetDropDownList(string Flag)
    {
        return await _ItemGroupDAL.GetDropDownList(Flag);
    }
    public async Task<ResponseResult> SaveItemGroup(ItemGroupModel model)
    {
        return await _ItemGroupDAL.SaveItemGroup(model);
    }
}
