using eTactWeb.DOM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Services.Interface
{
    public interface IItemCategory
    {
        Task<ResponseResult> DeleteByID(int ID);

        Task<ItemCategoryModel> GetByID(int ID);

        Task<ItemCategoryModel> GetDashboardData(ItemCategoryModel model);
        Task<ItemCategoryModel> GetSearchData(ItemCategoryModel model, string CategoryName, string TypeItem);
        Task<ResponseResult> CheckBeforeUpdate(int Type);
        Task<ResponseResult> GetAllItemCategory();
        Task<IList<TextValue>> GetDropDownList(string Flag);
        Task<ResponseResult> GetFormRights(int uId);

        Task<ResponseResult> SaveItemCategoryMaster(ItemCategoryModel model);
    }
}