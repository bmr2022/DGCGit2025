using eTactWeb.Data.Common;
using eTactWeb.Data.DAL;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.Extensions.Configuration;
using System.Reflection;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Data.BLL;

public class ItemMasterBLL : IItemMaster
{
    private ItemMasterDAL _ItemMasterDAL;
    private readonly IDataLogic _DataLogicDAL;

    //private readonly IConfiguration configuration;

    public ItemMasterBLL(IConfiguration config, IDataLogic dataLogicDAL, ConnectionStringService connectionStringService)
    {
        //configuration = config;
        _ItemMasterDAL = new ItemMasterDAL(config,dataLogicDAL, connectionStringService);
        _DataLogicDAL = dataLogicDAL;
    }

    public async Task<ResponseResult> DeleteItemByID(int ID)
    {
        return await _ItemMasterDAL.DeleteItemByID(ID);
    }

    public async Task<ResponseResult> GetFormRights(int ID)
    {
        return await _ItemMasterDAL.GetFormRights(ID);
    }
    public async Task<ResponseResult> GetPartCode(int ParentCode, int ItemType)
    {
        return await _ItemMasterDAL.GetPartCode(ParentCode, ItemType);
    }
    public async Task<ResponseResult> GetItemCategory(string ItemServAssets)
    {
        return await _ItemMasterDAL.GetItemCategory(ItemServAssets);
    }
    public async Task<ResponseResult> GetItemGroup(string ItemServAssets)
    {
        return await _ItemMasterDAL.GetItemGroup(ItemServAssets);
    }
	public async Task<ResponseResult> GetUnitList()
	{
		return await _ItemMasterDAL.GetUnitList();
	}
	public async Task<ResponseResult> GetProdInWorkcenter()
    {
        return await _ItemMasterDAL.GetProdInWorkcenter();
    }public async Task<ResponseResult> GetWorkCenterId(string WorkCenterDescription)
    {
        return await _ItemMasterDAL.GetWorkCenterId(WorkCenterDescription);
    }
  
    public async Task<IList<ItemMasterModel>> GetAllItemMaster(string Flag)
    {
        return await _ItemMasterDAL.GetAllItemMaster(Flag);
    }

    public async Task<IList<ItemMasterModel>> GetDashBoardData(string ItemName, string PartCode, string ItemGroup, string ItemCategory, string HsnNo, string UniversalPartCode, string Flag)
    {
        return await _ItemMasterDAL.GetDashBoardData(ItemName, PartCode, ItemGroup, ItemCategory, HsnNo,UniversalPartCode, Flag);
    }

    public FeatureOption GetFeatureOption()
    {
        return _ItemMasterDAL.GetFeatureOption();
    }

    public int GetisDelete()
    {
        return _ItemMasterDAL.GetisDelete();
    }

    public async Task<ItemMasterModel> GetItemMasterByID(int ID)
    {
        return await _ItemMasterDAL.GetItemMasterByID(ID);
    }

    public async Task<ResponseResult> SaveData(ItemMasterModel model)
    {
        return await _ItemMasterDAL.SaveData(model);
    }
    public async Task<ResponseResult> SaveMultipleItemData(DataTable ItemDetailGrid)
    {
        return await _ItemMasterDAL.SaveMultipleItemData(ItemDetailGrid);
    }
    public async Task<ResponseResult> UpdateMultipleItemData(DataTable ItemDetailGrid)
    {
        return await _ItemMasterDAL.UpdateMultipleItemData(ItemDetailGrid);
    }

    public Task<ResponseResult> GetItemGroupCode(string GroupCode)
    {
        return _ItemMasterDAL.GetItemGroupCode(GroupCode);
    }

    public Task<ResponseResult> GetItemCatCode(string CatCode)
    {
        return _ItemMasterDAL.GetItemCatCode(CatCode);
    }
}