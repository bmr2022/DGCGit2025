using eTactWeb.DOM.Models;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Services.Interface
{
    public interface ITaxMaster
    {
        Task<DataSet> BindAllDropDown();

        Task<ResponseResult> DeleteByID(int iD);
        Task<IList<TaxMasterDashboard>> GetSearchData(string TaxName, string TaxType,string HSNNo);

        Task<IList<TaxMasterDashboard>> GetDashBoardData();

        Task<ResponseResult> SaveTaxMaster(TaxMasterModel model, DataTable TaxMasterDT);

        Task<TaxMasterModel> ViewByID(int iD);
        Task<ResponseResult> GetFormRights(int uId);

    }
}