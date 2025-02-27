using eTactWeb.DOM.Models;
using static eTactWeb.DOM.Models.Common;
namespace eTactWeb.Services.Interface
{
    public interface IPartCodePartyWise
    {
        Task<ResponseResult> FillItems(string Type, string ShowAllItem);
        Task<ResponseResult> FillPartCode(string Type, string ShowAllItem);
        Task<ResponseResult> FillAccountName(string Type);
        Task<PartCodePartyWiseModel> GetListForUpdate(int ItemCode);
        Task<ResponseResult> DeleteByID(int ItemCode, string EntryByMachineName);
        string GetUnit(int ItemCode, string Flag);
        Task<PartCodePartyWiseDashboard> GetDashboardData(string ItemName, string PartCode, string AccountName, string CustvendPartCode, string CustvendItemName, string DashboardType);
        Task<ResponseResult> GetDashboardData();
        Task<ResponseResult> SavePartCodePartWise(PartCodePartyWiseModel model, DataTable DT);
        Task<PartCodePartyWiseModel> GetViewByID(int ItemCode);
        Task<ResponseResult> GetFormRights(int uId);

    }
}