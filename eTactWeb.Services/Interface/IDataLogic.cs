using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Services.Interface
{
    public interface IDataLogic
    {
        IList<TextValue> AutoComplete(string Flag, string Column, string FromDate, string ToDate, int ItemCode, int StoreId);

        Task<DataSet> BindAllDropDown(string FormName, string SPName, List<KeyValuePair<string, string>> SQLParams);

        Task<ResponseResult> ExecuteDataSet(string SPName, IList<dynamic> SQLParams);

        Task<ResponseResult> ExecuteDataTable(string SPName, IList<dynamic> SQLParams);

        Task<object> ExecuteScalar(string SPName, IList<dynamic> SQLParams);

        string GetDBConnection();

        Task<DataSet> GetDropDownList(string Flag);

        Task<IList<TextValue>> GetDropDownList(string Flag, string SPName);
        Task<DataSet> GetDropDownList(string Flag, int AccountCode, string TaxType, string vendorStateCode);
        Task<DataSet> GetTDSAccountList(string Flag, int AccountCode, string TaxType);

        Task<IList<TextValue>> GetDropDownList(string Flag, string CTRL, string SPName);

        int GetEntryID(string TableName, int YearCode, string ColName, string Yearcodeolumn);

        int IsDelete(int ID, string Type);

        ResponseResult isDuplicate(string ColumnValue, string ColumnName, string TableName);
        Task<ResponseResult> GetDbCrDataGrid(DataTable DbCrGridd, DataTable TaxDetailDT, DataTable TDSDetailDT, string FormName,int? docAccountCode, int? AccountCode, decimal? ItemNetAmount, decimal? NetTotal, int? RoundOffAccountCode,decimal?  RoundOffAmount, decimal? CashDis, decimal? AdditionalDis);
        Task<AdjustmentModel> GetPendVouchBillAgainstRefPopupByID(int AC, int? YC, int? PayRecEntryId, int? PayRecYearcode, string DRCR, string TransVouchType, string TransVouchDate, string Flag);
        Task<IList<TextValue>> GetDropDownListWithCustomeVar(string SPName, Dictionary<string, object> parameters, bool? IsTextandValueSame = false, bool? IsValueInFirstcolumn = true, bool? IsExcludeFirstcolumn = false);
    }
}