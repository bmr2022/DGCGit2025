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
        Task<ResponseResult> GetIndustryType();

        DataTable GetBomDetail(string FGC, int BMNo, string Flag);
        BomModel GetGridData(int IC, int BMNo);
        int GetBomNo(int ID, string Flag);
        Task<string> VerifyPartCode(DataTable bomDataTable);
        int GetBomStatus(int ItemCode, int BomNo);
        Task<ResponseResult> GetFormRights(int uId);
        Task<ResponseResult> FillItems(string Flag, string SearchPartCode);
        Task<ResponseResult> GetItemCode(string FGPartCode, string RMPartCode, string RunnerPartCode, string AltPartCode1, string AltPartCode2, string AltPartCode3, string AltPartCode4, string AltPartCode5);
        Task<ResponseResult> IsBomExists(string FGPartCode, string RMPartCode, int BomNo);
        Task<ResponseResult> GetAltItemCode(string AltPartCode);
        Task<ResponseResult> GetSearchData(BomDashboard model, int userID);
        Task<BomDashboard> GetDetailSearchData(BomDashboard model);
        string GetUnit(string IC, string Mode);
        Task<ResponseResult> SaveBomData(DataTable DT, BomModel model);
        Task<ResponseResult> SaveMultipleBomData(DataTable BomDetailGrid);
        Task<ResponseResult> GetByProdItemName(int MainItemcode);
        Task<ResponseResult> GetRMPartCodeList(string SearchRMPartcode, string CTRL);
        Task<ResponseResult> GetRMItemNameList(string SearchRMItemName, string CTRL);
        Task<ResponseResult> GetFGItemNameList(string SearchFGItemName, string CTRL);
        Task<ResponseResult> GetFGPartCodeList(string SearchFGPartCode, string CTRL);
        Task<ResponseResult> GetBomMultiLevelGrid();
        Task<ResponseResult> ChangeBomRMQtySameAsGrossWeight();
        Task<ResponseResult> AllowChangeFGBomQty();
        Task<BomModel> GetItemData(string FGPartCode, string FGItemName, string RMPartCode, string RMItemName);
        Task<ResponseResult> UpdateMultipleBOMData(DataTable ItemDetailGrid, int oldItemCode,
        string ApplyType);

    }
}