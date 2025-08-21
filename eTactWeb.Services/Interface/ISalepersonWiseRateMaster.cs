using eTactWeb.DOM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Services.Interface
{
    public interface ISalepersonWiseRateMaster
    {
        Task<ResponseResult> NewEntryId(int YearCode);
        Task<ResponseResult> FillSalePerson();
        Task<SalepersonWiseRateMasterModel> GetItemData(int ItemGroupId);
        //Task<SalepersonWiseRateMasterModel> DashBoard();
        Task<ResponseResult> SaveSalePersonWiseRate(DataTable DTItemGrid,  SalepersonWiseRateMasterModel model);


    }
}
