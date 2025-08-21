using eTactWeb.DOM.Models;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Services.Interface
{
    public interface IAccountMaster
    {
        Task<ResponseResult> DeleteByID(int ID);

        Task<AccountMasterModel> GetByID(int ID);

        Task<AccountMasterModel> GetDashboardData(AccountMasterModel model);
        Task<AccountMasterModel> GetDetailDashboardData(AccountMasterModel model);

        Task<IList<TextValue>> GetDropDownList(string Flag);

        Task<ResponseResult> GetParentGroupDetail(string iD);

        Task<ResponseResult> GetTDSPartyList();
        Task<ResponseResult> GetSalePersonName();

        Task<ResponseResult> SaveAccountMaster(AccountMasterModel model);
        Task<ResponseResult> GetFormRights(int uId);

    }
}