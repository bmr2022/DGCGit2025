using eTactWeb.DOM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Services.Interface
{
    public interface IStoreMaster
    {
        Task<ResponseResult> GetFormRights(int uId);

        Task<ResponseResult> FillStoreType();
        Task<ResponseResult> FillStoreID();
        Task<ResponseResult> SaveStoreMaster(StoreMasterModel model);
        Task<ResponseResult> GetDashBoardData();
        Task<StoreMasterModel> GetDashBoardDetailData();
        Task<StoreMasterModel> GetViewByID(int ID);
        Task<ResponseResult> DeleteByID(int ID);
        Task<ResponseResult> ChkForDuplicate(string StoreName);
        Task<ResponseResult> ChkForDuplicateStoreType(string StoreType);


    }
}
