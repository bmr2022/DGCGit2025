using eTactWeb.DOM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Services.Interface
{
    public interface ICurrencyMaster
    {
        //Task<ResponseResult> FillCurrencyID();
        Task<ResponseResult> GetFormRights(int uId);

        Task<ResponseResult> SaveCurrencyMaster(CurrencyMasterModel model);
        Task<ResponseResult> GetDashBoardData() ;

        Task<CurrencyMasterModel> GetDashBoardDetailData();
        Task<ResponseResult> DeleteByID(int ID);

        Task<CurrencyMasterModel> GetViewByID(int ID);
    }
}
