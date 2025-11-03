using eTactWeb.Data.Common;
using eTactWeb.Services.Interface;
using Microsoft.Extensions.Configuration;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Data.DAL;

public class TDSModuleDAL
{
    private readonly string DBConnectionString = string.Empty;

    private readonly DataSet? oDataSet = new();

    private readonly DataTable oDataTable = new();

    private object Result = "";
    private readonly ConnectionStringService _connectionStringService;
    public TDSModuleDAL(IConfiguration configuration, IDataLogic iDataLogic, ConnectionStringService connectionStringService)
    {
        _connectionStringService = connectionStringService;
        DBConnectionString = _connectionStringService.GetConnectionString();
        //DBConnectionString = configuration.GetConnectionString("eTactDB");
        _IDataLogic = iDataLogic;
    }

    private IDataLogic? _IDataLogic { get; }
    private string GetDBConnection => DBConnectionString;

    public string GetTDSPercentage(string Flag, string TxCode)
    {
        try
        {
            using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
            {
                SqlCommand oCmd = new SqlCommand("SP_Filtertaxmaster", myConnection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                oCmd.Parameters.AddWithValue("@Flag", Flag);
                oCmd.Parameters.AddWithValue("@TaxAccountcode", TxCode);

                myConnection.Open();

                Result = oCmd.ExecuteScalar();
                Result = Result ?? 0;
            }
        }
        catch (Exception ex)
        {
            dynamic Error = new ExpandoObject();
            Error.Message = ex.Message;
            Error.Source = ex.Source;
        }
        finally {  }
        return Result.ToString();
    }

    internal async Task<ResponseResult> GetHSNTaxInfo(HSNTAX HSNTaxParam)
    {
        var _ResponseResult = new ResponseResult();
        try
        {
            var SqlParams = new List<dynamic>();

            SqlParams.Add(new SqlParameter("@HSNNO", HSNTaxParam.HSNNo));
            SqlParams.Add(new SqlParameter("@Account_code", HSNTaxParam.AC));

            _ResponseResult = await _IDataLogic.ExecuteDataTable("GetHSNTAX", SqlParams);
        }
        catch (Exception ex)
        {
            dynamic Error = new ExpandoObject();
            Error.Message = ex.Message;
            Error.Source = ex.Source;
        }

        return _ResponseResult;
    }

    internal DataTable SgstCgst(int TxCode)
    {
        try
        {
            using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
            {
                SqlCommand oCmd = new SqlCommand("SP_TaxModule", myConnection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                oCmd.Parameters.AddWithValue("@Flag", "SGSTCGST");
                oCmd.Parameters.AddWithValue("@TxCode", TxCode);

                myConnection.Open();

                using (var DA = new SqlDataAdapter(oCmd))
                {
                    DA.Fill(oDataTable);
                }
            }
        }
        catch (Exception ex)
        {
            dynamic Error = new ExpandoObject();
            Error.Message = ex.Message;
            Error.Source = ex.Source;
        }
        finally
        {
            oDataTable.Dispose();
        }
        return oDataTable;
    }
}