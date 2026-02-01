using eTactWeb.DOM.Models;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Services.Interface
{
    public interface ITaxMaster
    {
        Task<DataSet> BindAllDropDown(string TaxName, decimal TaxPercent);

        Task<ResponseResult> DeleteByID(int iD, int CreatedBy);
        Task<IList<TaxMasterDashboard>> GetSearchData(string TaxName, string TaxType, string HSNNo, int userID);

        Task<IList<TaxMasterDashboard>> GetDashBoardData(int userID);

        Task<ResponseResult> SaveTaxMaster(TaxMasterModel model, DataTable TaxMasterDT);

        Task<TaxMasterModel> ViewByID(int iD);
        Task<ResponseResult> FillHSN(string taxCategory);
        Task<ResponseResult> GetFormRights(int uId);
        Task<ResponseResult> GetSGSTByTaxPercent(decimal taxPercent);

    }
}