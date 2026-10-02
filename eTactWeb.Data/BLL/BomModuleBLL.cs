using eTactWeb.Data.Common;
using eTactWeb.Data.DAL;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.Extensions.Configuration;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Data.BLL;

public class BomModuleBLL : IBomModule
{
    private BomModuleDAL _BomModuleDAL;
    private readonly IDataLogic _DataLogicDAL;
    private readonly ICommon _common;

    public BomModuleBLL(IConfiguration configuration, IDataLogic dataLogicDAL, ConnectionStringService connectionStringService, ICommon common)
    {
        _BomModuleDAL = new BomModuleDAL(configuration, dataLogicDAL, connectionStringService, common);
        _DataLogicDAL = dataLogicDAL;
        _common = common;
    }

    public async Task<ResponseResult> DeleteByID(string FIC, int BMNo, string Flag)
    {
        return await _BomModuleDAL.DeleteByID(FIC, BMNo, Flag);
    }

    public async Task<ResponseResult> GetFormRights(int ID)
    {
        return await _BomModuleDAL.GetFormRights(ID);
    }
    public async Task<ResponseResult> FillItems(string Flag, string SearchPartCode)
    {
        return await _BomModuleDAL.FillItems(Flag, SearchPartCode);
    }
    public async Task<ResponseResult> GetIndustryType()
    {
        return await _BomModuleDAL.GetIndustryType();
    }

    public async Task<ResponseResult> GetItemCode(string FGPartCode, string RMPartCode, string RunnerPartCode, string AltPartCode1, string AltPartCode2, string AltPartCode3, string AltPartCode4, string AltPartCode5)
    {
        return await _BomModuleDAL.GetItemCode(FGPartCode, RMPartCode, RunnerPartCode, AltPartCode1, AltPartCode2, AltPartCode3, AltPartCode4, AltPartCode5);
    }
    public async Task<ResponseResult> IsBomExists(string FGPartCode, string RMPartCode, int BomNo)
    {
        return await _BomModuleDAL.IsBomExists(FGPartCode, RMPartCode, BomNo);
    }
    public async Task<ResponseResult> GetAltItemCode(string AltPartCode)
    {
        return await _BomModuleDAL.GetAltItemCode(AltPartCode);
    }

    public async Task<BomModel> EditBomDetail(string FIC, int BMNo, string Flag)
    {
        return await _BomModuleDAL.EditBomDetail(FIC, BMNo, Flag);
    }

    public async Task<DataTable> EditBomSeq(BomModel model)
    {
        return await _BomModuleDAL.EditBomSeq(model);
    }
    public async Task<DataTable> CheckDupeConstraint()
    {
        return await _BomModuleDAL.CheckDupeConstraint();
    }

    public async Task<DataSet> GetBomDashboard(string Flag)
    {
        return await _BomModuleDAL.GetBomDashboard(Flag);
    }

    public DataTable GetBomDetail(string FGC, int BMNo, string Flag)
    {
        return _BomModuleDAL.GetBomDetail(FGC, BMNo, Flag);
    }
    public BomModel GetGridData(int IC, int BMNo)
    {
        return _BomModuleDAL.GetGridData(IC, BMNo);
    }

    public int GetBomNo(int ID, string Flag)
    {
        return _BomModuleDAL.GetBomNo(ID, Flag);
    }
    public async Task<string> VerifyPartCode(DataTable bomDataTable)
    {
        return await _BomModuleDAL.VerifyPartCode(bomDataTable);
    }

    public int GetBomStatus(int ItemCode, int BomNo)
    {
        return _BomModuleDAL.GetBomStatus(ItemCode, BomNo);
    }

    public async Task<ResponseResult> SaveMultipleBomData(DataTable BomDetailGrid)
    {
        return await _BomModuleDAL.SaveMultipleBomData(BomDetailGrid);
    }

    public async Task<ResponseResult> GetSearchData(BomDashboard model, int userID)
    {
        return await _BomModuleDAL.GetSearchData(model, userID);
    }
    public async Task<BomDashboard> GetDetailSearchData(BomDashboard model)
    {
        return await _BomModuleDAL.GetDetailSearchData(model);
    }

    public string GetUnit(string IC, string Mode)
    {
        return _BomModuleDAL.GetUnit(IC, Mode);
    }

    public async Task<ResponseResult> SaveBomData(DataTable DT, BomModel model)
    {
        return await _BomModuleDAL.SaveBomData(DT, model);
    }
    public async Task<ResponseResult> GetByProdItemName(int MainItemcode)
    {
        return await _BomModuleDAL.GetByProdItemName(MainItemcode);
    }
    public async Task<ResponseResult> GetBomMultiLevelGrid()
    {
        return await _BomModuleDAL.GetBomMultiLevelGrid();
    }
    public async Task<ResponseResult> GetRMPartCodeList(string SearchRMPartcode, string CTRL)
    {
        return await _BomModuleDAL.GetRMPartCodeList(SearchRMPartcode, CTRL);
    }
    public async Task<ResponseResult> GetRMItemNameList(string SearchRMItemName, string CTRL)
    {
        return await _BomModuleDAL.GetRMItemNameList(SearchRMItemName, CTRL);
    }
    public async Task<ResponseResult> GetFGItemNameList(string SearchFGItemName, string CTRL)
    {
        return await _BomModuleDAL.GetFGItemNameList(SearchFGItemName, CTRL);
    }
    public async Task<ResponseResult> GetFGPartCodeList(string SearchFGPartCode, string CTRL)
    {
        return await _BomModuleDAL.GetFGPartCodeList(SearchFGPartCode, CTRL);
    }
    public async Task<ResponseResult> ChangeBomRMQtySameAsGrossWeight()
    {
        return await _BomModuleDAL.ChangeBomRMQtySameAsGrossWeight();
    }
    public async Task<ResponseResult> AllowChangeFGBomQty()
    {
        return await _BomModuleDAL.AllowChangeFGBomQty();
    }

    public async Task<BomModel> GetItemData(string FGPartCode, string FGItemName, string RMPartCode, string RMItemName)
    {
        return await _BomModuleDAL.GetItemData(FGPartCode, FGItemName, RMPartCode, RMItemName);
    }

    public async Task<ResponseResult> UpdateMultipleBOMData(DataTable ItemDetailGrid, int oldItemCode,
        string ApplyType)
    {
        return await _BomModuleDAL.UpdateMultipleBOMData(ItemDetailGrid, oldItemCode, ApplyType);
    }

}