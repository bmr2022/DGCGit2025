using eTactWeb.Data.Common;
using eTactWeb.Data.DAL;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Data.BLL;

public class DataLogicBLL : IDataLogic
{  
    private DataLogicDAL _DataLogicDAL;
    private readonly IConnectionStringHelper _connectionStringHelper;
    private readonly UserContextService _userContextService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public DataLogicBLL(IConfiguration configuration, IConnectionStringHelper connectionStringHelper, UserContextService userContextService, IHttpContextAccessor httpContextAccessor, ConnectionStringService connectionStringService)
    {
        _connectionStringHelper = connectionStringHelper;
        _DataLogicDAL = new DataLogicDAL(configuration, _connectionStringHelper,connectionStringService);
        _userContextService = userContextService;
        _httpContextAccessor = httpContextAccessor;
    }

    public IList<TextValue> AutoComplete(string Flag, string Column,string FromDate,string ToDate,int ItemCode,int StoreId)
    {
        return _DataLogicDAL.AutoComplete(Flag, Column,FromDate,ToDate,ItemCode,StoreId);
    }

    public async Task<DataSet> BindAllDropDown(string FormName, string SPName, List<KeyValuePair<string, string>> SQLParams)
    {
        return await _DataLogicDAL.GetDataSet(FormName, SPName, SQLParams);
    }

    public async Task<ResponseResult> ExecuteDataSet(string SPName, IList<dynamic> SQLParams)
    {
        return await _DataLogicDAL.ExecuteDataSet(SPName, SQLParams);
    }

    public async Task<ResponseResult> ExecuteDataTable(string SPName, IList<dynamic> SQLParams)
    {
        return await _DataLogicDAL.ExecuteDataTable(SPName, SQLParams);
    }

    public async Task<object> ExecuteScalar(string SPName, IList<dynamic> SQLParams)
    {
        return await _DataLogicDAL.ExecuteScalar(SPName, SQLParams);
    }

    public string GetDBConnection()
    {
        return _DataLogicDAL.GetDBConnection;
    }

    public async Task<DataSet> GetDropDownList(string Flag)
    {
        return await _DataLogicDAL.GetDropDownList(Flag);
    }

    public async Task<IList<TextValue>> GetDropDownList(string Flag, string SPName)
    {
        return await _DataLogicDAL.GetDropDownList(Flag, SPName);
    }
    public async Task<DataSet> GetDropDownList(string Flag,int AccountCode,string TaxType,string vendorStateCode)
    {
        return await _DataLogicDAL.GetDropDownList(Flag, AccountCode, TaxType,vendorStateCode);
    }

    public async Task<IList<TextValue>> GetDropDownList(string Flag, string CTRL, string SPName)
    {
        return await _DataLogicDAL.GetDropDownList(Flag, CTRL, SPName);
    }
    public async Task<DataSet> GetTDSAccountList(string Flag, int AccountCode, string TaxType)
    {
        return await _DataLogicDAL.GetTDSAccountList(Flag, AccountCode, TaxType);
    }

    public int GetEntryID(string TableName, int YearCode, string ColName, string Yearcodecolumn)
    {
        return _DataLogicDAL.GetEntryID(TableName, YearCode, ColName, Yearcodecolumn);
    }

    public int IsDelete(int ID, string Type)
    {
        return _DataLogicDAL.IsDelete(ID, Type);
    }

    public ResponseResult isDuplicate(string ColumnValue, string ColumnName, string TableName)
    {
        return _DataLogicDAL.isDuplicate(ColumnValue, ColumnName, TableName);
    }
    public async Task<ResponseResult> GetDbCrDataGrid(DataTable DbCrGridd, DataTable TaxDetailDT, DataTable TDSDetailDT, string FormName,int? docAccountCode, int? AccountCode, decimal? ItemNetAmount, decimal? NetTotal,int?  RoundOffAccountCode,decimal?  RoundOffAmount )
    {
        return await _DataLogicDAL.GetDbCrDataGrid(DbCrGridd, TaxDetailDT, TDSDetailDT, FormName, docAccountCode, AccountCode, ItemNetAmount, NetTotal, RoundOffAccountCode, RoundOffAmount);
    }
    public async Task<AdjustmentModel> GetPendVouchBillAgainstRefPopupByID(int AC, int? YC, int? PayRecEntryId, int? PayRecYearcode, string DRCR, string TransVouchType, string TransVouchDate, string Flag)
    {
        return await _DataLogicDAL.GetPendVouchBillAgainstRefPopupByID(AC, YC, PayRecEntryId, PayRecYearcode, DRCR, TransVouchType, TransVouchDate, Flag);
    }
    public async Task<IList<TextValue>> GetDropDownListWithCustomeVar(string SPName, Dictionary<string, object> parameters, bool? IsTextandValueSame = false, bool? IsValueInFirstcolumn = true)
    {
        return await _DataLogicDAL.GetDropDownListWithCustomeVar(SPName, parameters, IsTextandValueSame, IsValueInFirstcolumn);
    }
}