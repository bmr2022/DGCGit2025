using eTactWeb.DOM.Models;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Services.Interface
{
    public interface IItemGroup
    {
        Task<ResponseResult> DeleteByID(int ID);

        Task<ItemGroupModel> GetByID(int ID);
        Task<ResponseResult> GetItemCatCode(string CatCode);
        Task<ItemGroupModel> GetDashboardData(ItemGroupModel model);
        Task<IList<TextValue>> GetDropDownList(string Flag);
        Task<ResponseResult> UpdateMultipleItemDataFromExcel(DataTable ItemDetailGrid, string flag);
        Task<ResponseResult> GetFormRights(int uId);
        Task<ResponseResult> GetUnderCategory(string Mode, string Type);
        Task<ResponseResult> CheckBeforeUpdate(int GroupCode);
        Task<ResponseResult> GetAllItemGroup();

        Task<ResponseResult> SaveItemGroup(ItemGroupModel model);
    }
}
