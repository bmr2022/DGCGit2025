using eTactWeb.Data.DAL;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.Extensions.Configuration;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Data.BLL;

public class ItemCategoryBLL : IItemCategory
{
    private ItemCategoryDAL _ItemCategoryDAL;
    private readonly IDataLogic _DataLogicDAL;

    public ItemCategoryBLL(IConfiguration config, IDataLogic dataLogicDAL)
    {
        _ItemCategoryDAL = new ItemCategoryDAL(config, dataLogicDAL);
        _DataLogicDAL = dataLogicDAL;
    }

    public async Task<ResponseResult> DeleteByID(int ID)
    {
        return await _ItemCategoryDAL.DeleteByID(ID);
    }
    public async Task<ItemCategoryModel> GetByID(int ID)
    {
        return await _ItemCategoryDAL.GetByID(ID);
        //return await _ItemCategoryDAL.GetByID(ID);

    }
    public async Task<ResponseResult> CheckBeforeUpdate(int Type)
    {
        return await _ItemCategoryDAL.CheckBeforeUpdate(Type);
    }
    public async Task<ResponseResult> GetAllItemCategory()
    {
        return await _ItemCategoryDAL.GetAllItemCategory();
    }

    public async Task<ItemCategoryModel> GetDashboardData(ItemCategoryModel model)
    {
        return await _ItemCategoryDAL.GetDashboardData(model);
    }
    public async Task<ItemCategoryModel> GetSearchData(ItemCategoryModel model, string CategoryName, string TypeItem)
    {
        return await _ItemCategoryDAL.GetSearchData(model, CategoryName, TypeItem);
    }
    public async Task<ResponseResult> GetFormRights(int ID)
    {
        return await _ItemCategoryDAL.GetFormRights(ID);
    }

    async Task<IList<TextValue>> IItemCategory.GetDropDownList(string Flag)
    {
        return await _ItemCategoryDAL.GetDropDownList(Flag);
    }

    async Task<ResponseResult> IItemCategory.SaveItemCategoryMaster(ItemCategoryModel model)
    {
        // return await _AccountMasterDAL.SaveAccountMaster(model);
        //return await _AccountMasterDAL.SaveAccountMaster(model);
        //
        return await _ItemCategoryDAL.SaveItemCategory(model);
    }
}

