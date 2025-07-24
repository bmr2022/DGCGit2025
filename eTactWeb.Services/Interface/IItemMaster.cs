using eTactWeb.DOM.Models;
using System.Security.Cryptography.X509Certificates;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Services.Interface
{
    public interface IItemMaster
    {
        Task<ResponseResult> DeleteItemByID(int ID);

        Task<IList<ItemMasterModel>> GetAllItemMaster(string Flag);

        Task<IList<ItemMasterModel>> GetDashBoardData(string ItemName, string PartCode, string ItemGroup, string ItemCategory, string HsnNo,string UniversalPartCode, string Flag);

        FeatureOption GetFeatureOption();

        int GetisDelete();
        Task<ResponseResult> GetFormRights(int uId);
        Task<ResponseResult> GetPartCode(int ParentCode, int ItemType);
        Task<ResponseResult> GetItemCategory(string ItemServAssets);
        Task<ResponseResult> GetItemGroup(string ItemServAssets);
		Task<ResponseResult> GetUnitList();
		Task<ResponseResult> GetStoreCode( string StoreName);
		Task<ResponseResult> GetStoreList();
		Task<ResponseResult> GetWorkCenterList();
		Task<ResponseResult> GetProdInWorkcenter();
		Task<ResponseResult> GetWorkCenterId(string WorkCenterDescription);
        Task<ResponseResult> GetItemGroupCode(string GroupCode);
        Task<ResponseResult> GetAccountCode(string AccountName);
        Task<ResponseResult> GetItemCatCode(string CatCode);
        //Task<ResponseResult> GetDupItemNameFeatureOpt();
        Task<ItemMasterModel> GetItemMasterByID(int ID);

        Task<ResponseResult> SaveData(ItemMasterModel model);
        Task<ResponseResult> SaveMultipleItemData(DataTable ItemDetailGrid);
        Task<ResponseResult> UpdateMultipleItemData(DataTable ItemDetailGrid);
        Task<ResponseResult> UpdateSelectedItemData(DataTable ItemDetailGrid,string flag);

    }
}