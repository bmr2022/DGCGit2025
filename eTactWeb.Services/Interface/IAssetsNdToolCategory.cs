using eTactWeb.DOM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Services.Interface
{
    public interface IAssetsNdToolCategoryMaster
    {
        Task<AssetsNdToolCategoryMasterModel> GetNewEntryId();
        Task<ResponseResult> SaveAsync(AssetsNdToolCategoryMasterModel model); // Insert/Update both handled
        Task<ResponseResult> DeleteAsync(long id, string categoryName);
        Task<List<AssetsNdToolCategoryMasterModel>> GetDashboardAsync();
        Task<AssetsNdToolCategoryMasterModel> ViewByIdAsync(long id,string categoryName);
    
    }
}
