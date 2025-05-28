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

    public BomModuleBLL(IConfiguration configuration, IDataLogic dataLogicDAL, ConnectionStringService connectionStringService)
    {
        _BomModuleDAL = new BomModuleDAL(configuration,dataLogicDAL,connectionStringService);
        _DataLogicDAL = dataLogicDAL;
    }

    public async Task<ResponseResult> DeleteByID(string FIC, int BMNo, string Flag)
    {
        return await _BomModuleDAL.DeleteByID(FIC, BMNo, Flag);
    }

    public async Task<ResponseResult> GetFormRights(int ID)
    {
        return await _BomModuleDAL.GetFormRights(ID);
    }
    
    public async Task<ResponseResult> GetItemCode(string FGPartCode, string RMPartCode)
    {
        return await _BomModuleDAL.GetItemCode(FGPartCode,RMPartCode);
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

    public async Task<BomDashboard> GetSearchData(BomDashboard model)
    {
        return await _BomModuleDAL.GetSearchData(model);
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
}