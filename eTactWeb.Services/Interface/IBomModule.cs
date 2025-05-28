using eTactWeb.DOM.Models;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Services.Interface
{
    public interface IBomModule
    {
        Task<ResponseResult> DeleteByID(string FIC, int BMNo, string Flag);
        Task<BomModel> EditBomDetail(string FIC, int BMNo, string Flag);
        Task<DataTable> EditBomSeq(BomModel model);
        Task<DataTable> CheckDupeConstraint();
        Task<DataSet> GetBomDashboard(string Flag);
        DataTable GetBomDetail(string FGC, int BMNo, string Flag);
        BomModel GetGridData(int IC, int BMNo);
        int GetBomNo(int ID, string Flag);
        Task<string> VerifyPartCode(DataTable bomDataTable);
        int GetBomStatus(int ItemCode, int BomNo);
        Task<ResponseResult> GetFormRights(int uId);
        Task<ResponseResult> GetItemCode(string FGPartCode, string RMPartCode);
        Task<ResponseResult> GetAltItemCode(string AltPartCode);
        Task<BomDashboard> GetSearchData(BomDashboard model);   
        Task<BomDashboard> GetDetailSearchData(BomDashboard model);   
        string GetUnit(string IC, string Mode);
        Task<ResponseResult> SaveBomData(DataTable DT, BomModel model);
        Task<ResponseResult> SaveMultipleBomData(DataTable BomDetailGrid);
        Task<ResponseResult> GetByProdItemName(int MainItemcode);

    }
}