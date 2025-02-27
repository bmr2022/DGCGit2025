using eTactWeb.DOM.Models;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Services.Interface
{
    public interface IBankMaster
    {
        Task<ResponseResult> DeleteByID(int ID);

        Task<BankMasterModel> GetByID(int ID);

        Task<BankMasterModel> GetDashboardData(BankMasterModel model);
        Task<BankMasterModel> GetDetailDashboardData(BankMasterModel model);

        Task<IList<TextValue>> GetDropDownList(string Flag);

        Task<ResponseResult> GetParentGroupDetail(string iD);
         

        Task<ResponseResult> SaveAccountMaster(BankMasterModel model);
        Task<ResponseResult> GetFormRights(int uId);

    }
}