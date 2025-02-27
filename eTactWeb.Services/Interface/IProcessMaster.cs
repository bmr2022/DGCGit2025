using eTactWeb.DOM.Models;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Services.Interface
{
    public interface IProcessMaster
    {
        Task<ResponseResult> DeleteByID(int ID);

        Task<ProcessMasterModel> GetByID(int ID);

        Task<ProcessMasterModel> GetDashboardData(ProcessMasterModel model);
        Task<ProcessMasterModel> GetSearchData(ProcessMasterModel model, string ProcessShortName, string ProcessCode);
        Task<ResponseResult> CheckBeforeUpdate(int Type);
        Task<IList<TextValue>> GetDropDownList(string Flag);
        Task<ResponseResult> GetFormRights(int uId);
        Task<ResponseResult> SaveProcessMasterMaster(ProcessMasterModel model);
    }
}
