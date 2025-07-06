using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices.JavaScript;
using static eTactWeb.DOM.Models.Common;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace eTactWeb.Data.DAL;

public class PurchaseBillDAL
{
    private readonly IDataLogic _IDataLogic;
    private readonly string DBConnectionString = string.Empty;
    private readonly ConnectionStringService _connectionStringService;
    public PurchaseBillDAL(IConfiguration configuration, IDataLogic iDataLogic, ConnectionStringService connectionStringService)
    {
        _IDataLogic = iDataLogic;
        _connectionStringService = connectionStringService;
        DBConnectionString = _connectionStringService.GetConnectionString();
        //DBConnectionString = configuration.GetConnectionString("eTactDB");
    }
    public async Task<string> GetItemServiceFORPO(string ItemService)
    {
        var JsonString = string.Empty;
        // throw new NotImplementedException();
        var _ResponseResult = new ResponseResult();

        try
        {
            var SqlParams = new List<dynamic>();

            SqlParams.Add(new SqlParameter("@Flag", "GetItemServiceFORPO"));
            //SqlParams.Add(new SqlParameter("@POTypeServItem", ItemSErv));
            SqlParams.Add(new SqlParameter("@POTypeServItem", ItemService));


            _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_PurchaseBillMainDetail", SqlParams);
            JsonString = JsonConvert.SerializeObject(_ResponseResult);
        }
        catch (Exception ex)
        {
            dynamic Error = new ExpandoObject();
            Error.Message = ex.Message;
            Error.Source = ex.Source;
        }

        return JsonString;
    }

    public async Task<ResponseResult> GetFormRights(int userId)
    {
        var _ResponseResult = new ResponseResult();
        try
        {
            var SqlParams = new List<dynamic>();
            SqlParams.Add(new SqlParameter("@Flag", "GetRights"));
            SqlParams.Add(new SqlParameter("@EmpId", userId));
            SqlParams.Add(new SqlParameter("@MainMenu", "PO"));

            _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_ItemGroup", SqlParams);
        }
        catch (Exception ex)
        {
            dynamic Error = new ExpandoObject();
            Error.Message = ex.Message;
            Error.Source = ex.Source;
        }
        return _ResponseResult;
    }
    public async Task<ResponseResult> fillEntryandVouchNo(int YearCode, string VODate)
    {
        var _ResponseResult = new ResponseResult();
        try
        {
            DateTime vodt = DateTime.ParseExact(VODate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            var SqlParams = new List<dynamic>();
            SqlParams.Add(new SqlParameter("@Flag", "NewEntryId"));
            SqlParams.Add(new SqlParameter("@YearCode", YearCode));
            SqlParams.Add(new SqlParameter("@EntryDate", vodt));

            _ResponseResult = await _IDataLogic.ExecuteDataSet("AccSP_PurchaseBillMainDetail", SqlParams);
        }
        catch (Exception ex)
        {
            dynamic Error = new ExpandoObject();
            Error.Message = ex.Message;
            Error.Source = ex.Source;
        }
        return _ResponseResult;
    }
    public async Task<ResponseResult> GetAllowDocName()
    {
        var _ResponseResult = new ResponseResult();

        try
        {
            var SqlParams = new List<dynamic>();

            SqlParams.Add(new SqlParameter("@Flag", "AllowTochangeDocument"));

            _ResponseResult = await _IDataLogic.ExecuteDataTable("AccSP_PurchaseBillMainDetail", SqlParams);
        }
        catch (Exception ex)
        {
            dynamic Error = new ExpandoObject();
            Error.Message = ex.Message;
            Error.Source = ex.Source;
        }
        return _ResponseResult;
    }
    public async Task<ResponseResult> GetAllowInvoiceNo()
    {
        var _ResponseResult = new ResponseResult();

        try
        {
            var SqlParams = new List<dynamic>();

            SqlParams.Add(new SqlParameter("@Flag", "AllowTochangeInvoiceNo"));

            _ResponseResult = await _IDataLogic.ExecuteDataTable("AccSP_PurchaseBillMainDetail", SqlParams);
        }
        catch (Exception ex)
        {
            dynamic Error = new ExpandoObject();
            Error.Message = ex.Message;
            Error.Source = ex.Source;
        }
        return _ResponseResult;
    }
    public async Task<ResponseResult> FillItems(string Type, string ShowAllItem)
    {
        var _ResponseResult = new ResponseResult();
        try
        {
            var SqlParams = new List<dynamic>();
            SqlParams.Add(new SqlParameter("@Flag", "GETITEMS"));
            SqlParams.Add(new SqlParameter("@AType", Type));
            SqlParams.Add(new SqlParameter("@ShowAll", ShowAllItem));

            _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_PurchaseBillMainDetail", SqlParams);
        }
        catch (Exception ex)
        {
            dynamic Error = new ExpandoObject();
            Error.Message = ex.Message;
            Error.Source = ex.Source;
        }
        return _ResponseResult;
    }
    public async Task<ResponseResult> GetItems(string ShowAll)
    {
        var _ResponseResult = new ResponseResult();
        try
        {
            var SqlParams = new List<dynamic>();
            SqlParams.Add(new SqlParameter("@Flag", "GETITEMS"));
            SqlParams.Add(new SqlParameter("@ShowAll", ShowAll));

            _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_PurchaseBillMainDetail", SqlParams);
        }
        catch (Exception ex)
        {
            dynamic Error = new ExpandoObject();
            Error.Message = ex.Message;
            Error.Source = ex.Source;
        }
        return _ResponseResult;
    }
    public async Task<ResponseResult> CheckLockYear(int YearCode)
    {
        var _ResponseResult = new ResponseResult();
        try
        {
            var SqlParams = new List<dynamic>();
            SqlParams.Add(new SqlParameter("@Flag", "CheckLockYear"));
            SqlParams.Add(new SqlParameter("@YearCode", YearCode));
            SqlParams.Add(new SqlParameter("@Module", "purchase"));

            _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_PurchaseBillMainDetail", SqlParams);
        }
        catch (Exception ex)
        {
            dynamic Error = new ExpandoObject();
            Error.Message = ex.Message;
            Error.Source = ex.Source;
        }
        return _ResponseResult;
    }
    public async Task<ResponseResult> FillCurrency(int? AccountCode)
    {
        var _ResponseResult = new ResponseResult();
        try
        {
            var SqlParams = new List<dynamic>();
            SqlParams.Add(new SqlParameter("@Flag", "Currency"));
            SqlParams.Add(new SqlParameter("@accountcode", AccountCode ?? 0));

            _ResponseResult = await _IDataLogic.ExecuteDataSet("AccSP_PurchaseBillMainDetail", SqlParams);
        }
        catch (Exception ex)
        {
            dynamic Error = new ExpandoObject();
            Error.Message = ex.Message;
            Error.Source = ex.Source;
        }
        return _ResponseResult;
    }
    public async Task<ResponseResult> FillDocName(string ShowAll)
    {
        var _ResponseResult = new ResponseResult();
        try
        {
            var SqlParams = new List<dynamic>();
            SqlParams.Add(new SqlParameter("@Flag", "FILLDocName"));
            SqlParams.Add(new SqlParameter("@ShowAll", ShowAll));

            _ResponseResult = await _IDataLogic.ExecuteDataSet("AccSP_PurchaseBillMainDetail", SqlParams);
        }
        catch (Exception ex)
        {
            dynamic Error = new ExpandoObject();
            Error.Message = ex.Message;
            Error.Source = ex.Source;
        }
        return _ResponseResult;
    }
    public async Task<ResponseResult> FillPONumber(int YearCode, string Ordertype, string POdate)
    {
        var _ResponseResult = new ResponseResult();
        try
        {
            var SqlParams = new List<dynamic>();
            DateTime podt = DateTime.ParseExact(POdate, "dd/MM/yyyy", CultureInfo.InvariantCulture);

            SqlParams.Add(new SqlParameter("@Flag", "NewPO"));
            SqlParams.Add(new SqlParameter("@YearCode", YearCode));
            SqlParams.Add(new SqlParameter("@orderType", Ordertype));
            SqlParams.Add(new SqlParameter("@PODate", podt.ToString("yyyy/MM/dd")));

            _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_PurchaseBillMainDetail", SqlParams);
        }
        catch (Exception ex)
        {
            dynamic Error = new ExpandoObject();
            Error.Message = ex.Message;
            Error.Source = ex.Source;
        }
        return _ResponseResult;
    }
    public async Task<ResponseResult> GetPOData(string BillDate, int? AccountCode = 0, int? ItemCode = 0)
    {
        var _ResponseResult = new ResponseResult();
        try
        {
            var SqlParams = new List<dynamic>();
            DateTime billdt = DateTime.ParseExact(BillDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);

            SqlParams.Add(new SqlParameter("@Flag", "FILLPONO"));
            SqlParams.Add(new SqlParameter("@billdate", billdt.ToString("yyyy/MM/dd")));
            SqlParams.Add(new SqlParameter("@AccountCode", AccountCode));
            SqlParams.Add(new SqlParameter("@ItemCode", ItemCode));

            _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_PurchaseBillMainDetail", SqlParams);
        }
        catch (Exception ex)
        {
            dynamic Error = new ExpandoObject();
            Error.Message = ex.Message;
            Error.Source = ex.Source;
        }
        return _ResponseResult;
    }
    public async Task<ResponseResult> GetScheduleData(string PONo, string BillDate, int? POYear = 0, int? AccountCode = 0, int? ItemCode = 0)
    {
        var _ResponseResult = new ResponseResult();
        try
        {
            var SqlParams = new List<dynamic>();
            DateTime billdt = DateTime.ParseExact(BillDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);

            SqlParams.Add(new SqlParameter("@Flag", "FILLPOSchedule"));
            SqlParams.Add(new SqlParameter("@POno", PONo));
            SqlParams.Add(new SqlParameter("@poYearCode", POYear));
            SqlParams.Add(new SqlParameter("@billdate", billdt.ToString("yyyy/MM/dd")));
            SqlParams.Add(new SqlParameter("@AccountCode", AccountCode));
            SqlParams.Add(new SqlParameter("@ItemCode", ItemCode));

            _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_PurchaseBillMainDetail", SqlParams);
        }
        catch (Exception ex)
        {
            dynamic Error = new ExpandoObject();
            Error.Message = ex.Message;
            Error.Source = ex.Source;
        }
        return _ResponseResult;
    }
    public async Task<ResponseResult> FillVouchNumber(int YearCode, string VODate)
    {
        var _ResponseResult = new ResponseResult();
        try
        {
            using (SqlConnection connection = new SqlConnection(DBConnectionString))
            {
                DateTime vodt = DateTime.ParseExact(VODate, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                SqlCommand command = new SqlCommand("GenrerateVoucherPrefix", connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.Add("@yearcode", SqlDbType.BigInt).Value = YearCode;
                command.Parameters.Add("@tableName", SqlDbType.NVarChar, 100).Value = "PurchaseBillMain";
                command.Parameters.Add("@ColName", SqlDbType.NVarChar, 100).Value = "purvoucherno";
                command.Parameters.Add("@transdate", SqlDbType.DateTime).Value = vodt;

                SqlParameter outputParameter = new SqlParameter("@MaxSrID", SqlDbType.NVarChar, 200);
                outputParameter.Direction = ParameterDirection.Output;
                command.Parameters.Add(outputParameter);

                await connection.OpenAsync();
                await command.ExecuteNonQueryAsync();

                _ResponseResult.Result = outputParameter.Value.ToString();
            }
        }
        catch (Exception ex)
        {
            dynamic Error = new ExpandoObject();
            Error.Message = ex.Message;
            Error.Source = ex.Source;
        }
        return _ResponseResult;
    }

    public async Task<ResponseResult> AltUnitConversion(int ItemCode, decimal AltQty, decimal UnitQty)
    {
        var _ResponseResult = new ResponseResult();
        try
        {
            var SqlParams = new List<dynamic>();

            SqlParams.Add(new SqlParameter("@Itemcode", ItemCode));
            SqlParams.Add(new SqlParameter("@ALtQty", AltQty));
            SqlParams.Add(new SqlParameter("@UnitQty", UnitQty));

            _ResponseResult = await _IDataLogic.ExecuteDataTable("AltUnitConversion", SqlParams);
        }
        catch (Exception ex)
        {
            dynamic Error = new ExpandoObject();
            Error.Message = ex.Message;
            Error.Source = ex.Source;
        }
        return _ResponseResult;
    }

    internal async Task<ResponseResult> DeleteByID(int ID, int YearCode, string Flag, string PurchVoucherNo, string InvNo, int EntryBy, string EntryByMachineName, DateTime EntryDate)
    {
        var _ResponseResult = new ResponseResult();

        try
        {
            var SqlParams = new List<dynamic>();

            SqlParams.Add(new SqlParameter("@Flag", Flag));
            SqlParams.Add(new SqlParameter("@EntryID", ID));
            SqlParams.Add(new SqlParameter("@YearCode", YearCode));
            SqlParams.Add(new SqlParameter("@PurchVoucherNo", PurchVoucherNo));
            SqlParams.Add(new SqlParameter("@InvNo", InvNo));
            SqlParams.Add(new SqlParameter("@EneterdBy", EntryBy));
            SqlParams.Add(new SqlParameter("@EntryByMachineName", EntryByMachineName));
            SqlParams.Add(new SqlParameter("@EntryDate", EntryDate));

            _ResponseResult = await _IDataLogic.ExecuteDataTable("AccSP_PurchaseBillMainDetail", SqlParams);
        }
        catch (Exception ex)
        {
            dynamic Error = new ExpandoObject();
            Error.Message = ex.Message;
            Error.Source = ex.Source;
        }

        return _ResponseResult;
    }
    public async Task<ResponseResult> GetExchangeRate(string Currency)
    {
        var _ResponseResult = new ResponseResult();

        try
        {
            var SqlParams = new List<dynamic>();

            SqlParams.Add(new SqlParameter("@flag", "ExchangeRate"));
            SqlParams.Add(new SqlParameter("@EntryDate", DateTime.Today));
            SqlParams.Add(new SqlParameter("@Currency", Currency));

            _ResponseResult = await _IDataLogic.ExecuteDataTable("AccSP_PurchaseBillMainDetail", SqlParams);
        }
        catch (Exception ex)
        {
            dynamic Error = new ExpandoObject();
            Error.Message = ex.Message;
            Error.Source = ex.Source;
        }

        return _ResponseResult;
    }
    public async Task<ResponseResult> GetGstRegister(string Flag, int Code)
    {
        var _ResponseResult = new ResponseResult();

        try
        {
            var SqlParams = new List<dynamic>();

            SqlParams.Add(new SqlParameter("@Flag", Flag));
            SqlParams.Add(new SqlParameter("@AccountCode", Code));

            _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_PurchaseBillMainDetail", SqlParams);
        }
        catch (Exception ex)
        {
            dynamic Error = new ExpandoObject();
            Error.Message = ex.Message;
            Error.Source = ex.Source;
        }

        return _ResponseResult;
    }
    public async Task<ResponseResult> GetStateGST(string Flag, int Code)
    {
        var _ResponseResult = new ResponseResult();

        try
        {
            var SqlParams = new List<dynamic>();

            SqlParams.Add(new SqlParameter("@Flag", Flag));
            SqlParams.Add(new SqlParameter("@AccountCode", Code));

            _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_PurchaseBillMainDetail", SqlParams);
        }
        catch (Exception ex)
        {
            dynamic Error = new ExpandoObject();
            Error.Message = ex.Message;
            Error.Source = ex.Source;
        }

        return _ResponseResult;
    }
    public async Task<ResponseResult> NewAmmEntryId()
    {
        var _ResponseResult = new ResponseResult();

        try
        {
            var SqlParams = new List<dynamic>();

            SqlParams.Add(new SqlParameter("@Flag", "GetNewAmmEntry"));

            _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_PurchaseBillMainDetail", SqlParams);
        }
        catch (Exception ex)
        {
            dynamic Error = new ExpandoObject();
            Error.Message = ex.Message;
            Error.Source = ex.Source;
        }

        return _ResponseResult;
    }

    internal async Task<PBDashBoard> GetDashBoardData()
    {
        var DashBoardData = new PBDashBoard();
        var SqlParams = new List<dynamic>();
        DataSet? oDataSet = new DataSet();

        try
        {
            using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
            {
                DateTime now = DateTime.Now;
                DateTime firstDayOfMonth = new DateTime(now.Year, now.Month, 1);
                SqlCommand oCmd = new SqlCommand("AccSP_PurchaseBillMainDetail", myConnection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                oCmd.Parameters.AddWithValue("@Flag", "DASHBOARD");
                oCmd.Parameters.AddWithValue("@FlagMRNJWCHALLAN", "MRN");
                oCmd.Parameters.AddWithValue("@SummDetail", "summary");
                oCmd.Parameters.AddWithValue("@FromDate", firstDayOfMonth);
                oCmd.Parameters.AddWithValue("@ToDate", now);
                oCmd.Parameters.AddWithValue("@PurchVoucherNo", string.Empty);
                oCmd.Parameters.AddWithValue("@InvNo", string.Empty);
                oCmd.Parameters.AddWithValue("@partcode", string.Empty);
                oCmd.Parameters.AddWithValue("@Vendorname", string.Empty);
                oCmd.Parameters.AddWithValue("@ItemName", string.Empty);
                oCmd.Parameters.AddWithValue("@GSTType", string.Empty);
                oCmd.Parameters.AddWithValue("@TypeITEMSERVASSETS", string.Empty);
                oCmd.Parameters.AddWithValue("@DocumentType", string.Empty);
                oCmd.Parameters.AddWithValue("@HSNNo", string.Empty);
                await myConnection.OpenAsync();
                using (SqlDataAdapter oDataAdapter = new SqlDataAdapter(oCmd))
                {
                    oDataAdapter.Fill(oDataSet);
                }
            }

            if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
            {
                DashBoardData.PBDashboard = (from DataRow dr in oDataSet.Tables[0].Rows
                                             select new PBDashBoard
                                             {
                                                 EntryID = !string.IsNullOrEmpty(dr["PurchBillEntryId"].ToString()) ? Convert.ToInt32(dr["PurchBillEntryId"]) : 0,
                                                 PurchBillEntryId = !string.IsNullOrEmpty(dr["PurchBillEntryId"].ToString()) ? dr["PurchBillEntryId"].ToString() : "",
                                                 EntryDate = string.IsNullOrEmpty(dr["EntryDate"].ToString()) ? new DateTime() : Convert.ToDateTime(dr["EntryDate"]),
                                                 InvoiceNo = dr["InvoiceNo"].ToString(),
                                                 InvoiceDate = string.IsNullOrEmpty(dr["InvoiceDate"].ToString()) ? new DateTime() : Convert.ToDateTime(dr["InvoiceDate"]),
                                                 VoucherNo = dr["VoucherNo"].ToString(),
                                                 VoucherDate = dr["VoucherDate"].ToString(),
                                                 VendorName = dr["VendorName"].ToString(),
                                                 VendorAddress = dr["VendorAddress"].ToString(),
                                                 StateName = dr["StateName"].ToString(),
                                                 GSTType = dr["GSTType"].ToString(),
                                                 MRNNo = dr["MRNNo"].ToString(),
                                                 MRNDate = dr["MRNDate"].ToString(),
                                                 GateNo = dr["GateNo"].ToString(),
                                                 GateDate = dr["GateDate"].ToString(),
                                                 DomesticImport = dr["DomesticImport"].ToString(),
                                                 Currency = dr["Currency"].ToString(),
                                                 PurchBillYearCode = !string.IsNullOrEmpty(dr["PurchBillYearCode"].ToString()) ? Convert.ToInt32(dr["PurchBillYearCode"]) : 0,
                                                 PurchaseBillDirectPB = !string.IsNullOrEmpty(dr["PurchaseBillDirectPB"].ToString()) ? dr["PurchaseBillDirectPB"].ToString() : "",
                                                 Branch = dr["Branch"].ToString(),
                                                 PaymentTerm = dr["PaymentTerm"].ToString(),
                                                 Transporter = dr["Transporter"].ToString(),
                                                 VehicleNo = dr["Vehicleno"].ToString(),
                                                 ExchangeRate = dr["ExchangeRate"].ToString(),
                                                 RoundoffType = dr["RoundoffType"].ToString(),
                                                 PONetAmt = !string.IsNullOrEmpty(dr["PONetAmt"].ToString()) ? Convert.ToSingle(dr["PONetAmt"]) : 0,
                                                 TotalDiscountPercent = !string.IsNullOrEmpty(dr["ToatlDiscountPercent"].ToString()) ? Convert.ToSingle(dr["ToatlDiscountPercent"]) : 0,
                                                 TDSAmount = !string.IsNullOrEmpty(dr["TDSAmount"].ToString()) ? Convert.ToSingle(dr["TDSAmount"]) : 0,
                                                 Remark = dr["Remark"].ToString(),
                                                 ModeOfTrans = dr["ModeOfTrans"].ToString(),
                                                 Approved = dr["Approved"].ToString(),
                                                 ApprovedDate = dr["ApprovedDate"].ToString(),
                                                 ApprovedBy = dr["Approvedby"].ToString(),
                                                 BOEDate = dr["BOEDate"].ToString(),
                                                 TotalAmtInOtherCurr = dr["TotalAmtInOtherCurr"].ToString(),
                                                 boeno = dr["boeno"].ToString(),
                                                 Commodity = dr["Commodity"].ToString(),
                                                 PathOfFile1 = dr["PathOfFile1"].ToString(),
                                                 PathOfFile2 = dr["pathoffile2"].ToString(),
                                                 PathOfFile3 = dr["pathoffile3"].ToString(),
                                                 BalanceSheetClosed = dr["BalanceSheetClosed"].ToString(),
                                                 PORemarks = dr["PORemarks"].ToString(),
                                                 UID = dr["Uid"].ToString(),
                                                 EntryByMachine = dr["EntryByMachine"].ToString(),
                                                 ItemOrService = dr["Item/Service"].ToString(),
                                                 UpdatedByName = dr["EntryByMachine"].ToString(),
                                                 UpdatedBy = string.IsNullOrEmpty(dr["LastUpdatedBy"].ToString()) ? 0 : Convert.ToInt32(dr["LastUpdatedBy"].ToString()),
                                                 EnteredBy = dr["EntryByMachine"].ToString(),
                                                 UpdatedOn = string.IsNullOrEmpty(dr["LastUpdatedDate"].ToString()) ? new DateTime() : Convert.ToDateTime(dr["LastUpdatedDate"]),
                                                 CreatedBy = string.IsNullOrEmpty(dr["Uid"].ToString()) ? 0 : Convert.ToInt32(dr["Uid"].ToString()),
                                                 CreatedOn = string.IsNullOrEmpty(dr["ActualEntryDate"].ToString()) ? new DateTime() : Convert.ToDateTime(dr["ActualEntryDate"]),
                                                 BillAmount = !string.IsNullOrEmpty(dr["BillAmt"].ToString()) ? Convert.ToSingle(dr["BillAmt"]) : 0,
                                                 TaxableAmount = !string.IsNullOrEmpty(dr["Taxableamt"].ToString()) ? Convert.ToSingle(dr["Taxableamt"]) : 0,
                                                 GSTAmount = !string.IsNullOrEmpty(dr["GSTAmount"].ToString()) ? Convert.ToSingle(dr["GSTAmount"]) : 0,
                                                 RoundOffAmt = !string.IsNullOrEmpty(dr["RoundOffAmt"].ToString()) ? Convert.ToSingle(dr["RoundOffAmt"]) : 0,
                                                 NetAmt = !string.IsNullOrEmpty(dr["NetAmt"].ToString()) ? Convert.ToSingle(dr["NetAmt"]) : 0,
                                                 PendAmt = !string.IsNullOrEmpty(dr["PendAmt"].ToString()) ? Convert.ToSingle(dr["PendAmt"]) : 0,
                                                 PaidAmt = !string.IsNullOrEmpty(dr["PaidAmt"].ToString()) ? Convert.ToSingle(dr["PaidAmt"]) : 0,
                                                 MRNType = dr["PurchaseBillTypeMRNJWChallan"].ToString(),
                                                 DocumentName = dr["DocumentType"].ToString(),
                                                 PartCode = dr["PartCode"].ToString(),
                                                 ItemName = dr["ItemName"].ToString(),
                                                 HSNNO = dr["HSNNO"].ToString()
                                             }).OrderBy(a => a.EntryID).ToList();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            dynamic Error = new ExpandoObject();
            Error.Message = ex.Message;
            Error.Source = ex.Source;
        }

        return DashBoardData;
    }
    public async Task<PBDashBoard> GetSummaryData(PBDashBoard model)
    {
        var DashBoardData = new PBDashBoard();
        DataSet? oDataSet = new DataSet();
        try
        {
            using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
            {
                DateTime now = DateTime.Now;
                DateTime firstDayOfMonth = new DateTime(now.Year, now.Month, 1);
                DateTime fromDate = DateTime.ParseExact(model.FromDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                DateTime toDate = DateTime.ParseExact(model.ToDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                SqlCommand oCmd = new SqlCommand("AccSP_PurchaseBillMainDetail", myConnection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                oCmd.Parameters.AddWithValue("@Flag", "DASHBOARD");
                oCmd.Parameters.AddWithValue("@FlagMRNJWCHALLAN", !string.IsNullOrEmpty(model.MRNType) ? (model.MRNType != "0" ? model.MRNType : string.Empty) : string.Empty);
                oCmd.Parameters.AddWithValue("@SummDetail", model.DashboardType.ToLower());
                oCmd.Parameters.AddWithValue("@FromDate", fromDate);
                oCmd.Parameters.AddWithValue("@ToDate", toDate);
                oCmd.Parameters.AddWithValue("@PurchVoucherNo", !string.IsNullOrEmpty(model.VoucherNo) ? (model.VoucherNo != "0" ? model.VoucherNo : string.Empty) : string.Empty);
                oCmd.Parameters.AddWithValue("@InvNo", !string.IsNullOrEmpty(model.InvoiceNo) ? (model.InvoiceNo != "0" ? model.InvoiceNo : string.Empty) : string.Empty);
                oCmd.Parameters.AddWithValue("@partcode", !string.IsNullOrEmpty(model.PartCode) ? (model.PartCode != "0" ? model.PartCode : string.Empty) : string.Empty);
                oCmd.Parameters.AddWithValue("@Vendorname", !string.IsNullOrEmpty(model.VendorName) ? (model.VendorName != "0" ? model.VendorName : string.Empty) : string.Empty);
                oCmd.Parameters.AddWithValue("@ItemName", !string.IsNullOrEmpty(model.ItemName) ? (model.ItemName != "0" ? model.ItemName : string.Empty) : string.Empty);
                oCmd.Parameters.AddWithValue("@GSTType", !string.IsNullOrEmpty(model.GSTType) ? (model.GSTType != "0" ? model.GSTType : string.Empty) : string.Empty);
                oCmd.Parameters.AddWithValue("@TypeITEMSERVASSETS", !string.IsNullOrEmpty(model.ItemOrService) ? (model.ItemOrService != "0" ? model.ItemOrService : string.Empty) : string.Empty);
                oCmd.Parameters.AddWithValue("@DocumentType", !string.IsNullOrEmpty(model.DocumentName) ? (model.DocumentName != "0" ? model.DocumentName : string.Empty) : string.Empty);
                oCmd.Parameters.AddWithValue("@HSNNo", !string.IsNullOrEmpty(model.HSNNO) ? (model.HSNNO != "0" ? model.HSNNO : string.Empty) : string.Empty);
                oCmd.Parameters.AddWithValue("@Mrnno", !string.IsNullOrEmpty(model.MRNNo) ? (model.MRNNo != "0" ? model.MRNNo : string.Empty) : string.Empty);
                oCmd.Parameters.AddWithValue("@POno", !string.IsNullOrEmpty(model.PONo) ? (model.PONo != "0" ? model.PONo : string.Empty) : string.Empty);
                oCmd.Parameters.AddWithValue("@Gateno", !string.IsNullOrEmpty(model.GateNo) ? (model.GateNo != "0" ? model.GateNo : string.Empty) : string.Empty);
                await myConnection.OpenAsync();
                using (SqlDataAdapter oDataAdapter = new SqlDataAdapter(oCmd))
                {
                    oDataAdapter.Fill(oDataSet);
                }
            }

            if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
            {
                DashBoardData.PBDashboard = (from DataRow dr in oDataSet.Tables[0].Rows
                                             select new PBDashBoard
                                             {
                                                 EntryID = !string.IsNullOrEmpty(dr["PurchBillEntryId"].ToString()) ? Convert.ToInt32(dr["PurchBillEntryId"]) : 0,
                                                 PurchBillEntryId = !string.IsNullOrEmpty(dr["PurchBillEntryId"].ToString()) ? dr["PurchBillEntryId"].ToString() : "",
                                                 EntryDate = string.IsNullOrEmpty(dr["EntryDate"].ToString()) ? new DateTime() : Convert.ToDateTime(dr["EntryDate"]),
                                                 InvoiceNo = dr["InvoiceNo"].ToString(),
                                                 InvoiceDate = string.IsNullOrEmpty(dr["InvoiceDate"].ToString()) ? new DateTime() : Convert.ToDateTime(dr["InvoiceDate"]),
                                                 VoucherNo = dr["VoucherNo"].ToString(),
                                                 VoucherDate = dr["VoucherDate"].ToString(),
                                                 VendorName = dr["VendorName"].ToString(),
                                                 VendorAddress = dr["VendorAddress"].ToString(),
                                                 StateName = dr["StateName"].ToString(),
                                                 GSTType = dr["GSTType"].ToString(),
                                                 MRNNo = dr["MRNNo"].ToString(),
                                                 MRNDate = dr["MRNDate"].ToString(),
                                                 GateNo = dr["GateNo"].ToString(),
                                                 GateDate = dr["GateDate"].ToString(),
                                                 DomesticImport = dr["DomesticImport"].ToString(),
                                                 Currency = dr["Currency"].ToString(),
                                                 PurchBillYearCode = !string.IsNullOrEmpty(dr["PurchBillYearCode"].ToString()) ? Convert.ToInt32(dr["PurchBillYearCode"]) : 0,
                                                 PurchaseBillDirectPB = !string.IsNullOrEmpty(dr["PurchaseBillDirectPB"].ToString()) ? dr["PurchaseBillDirectPB"].ToString() : "",
                                                 Branch = dr["Branch"].ToString(),
                                                 PaymentTerm = dr["PaymentTerm"].ToString(),
                                                 Transporter = dr["Transporter"].ToString(),
                                                 VehicleNo = dr["Vehicleno"].ToString(),
                                                 ExchangeRate = dr["ExchangeRate"].ToString(),
                                                 RoundoffType = dr["RoundoffType"].ToString(),
                                                 PONetAmt = !string.IsNullOrEmpty(dr["PONetAmt"].ToString()) ? Convert.ToSingle(dr["PONetAmt"]) : 0,
                                                 TotalDiscountPercent = !string.IsNullOrEmpty(dr["ToatlDiscountPercent"].ToString()) ? Convert.ToSingle(dr["ToatlDiscountPercent"]) : 0,
                                                 TDSAmount = !string.IsNullOrEmpty(dr["TDSAmount"].ToString()) ? Convert.ToSingle(dr["TDSAmount"]) : 0,
                                                 Remark = dr["Remark"].ToString(),
                                                 ModeOfTrans = dr["ModeOfTrans"].ToString(),
                                                 Approved = dr["Approved"].ToString(),
                                                 ApprovedDate = dr["ApprovedDate"].ToString(),
                                                 ApprovedBy = dr["Approvedby"].ToString(),
                                                 BOEDate = dr["BOEDate"].ToString(),
                                                 TotalAmtInOtherCurr = dr["TotalAmtInOtherCurr"].ToString(),
                                                 boeno = dr["boeno"].ToString(),
                                                 Commodity = dr["Commodity"].ToString(),
                                                 PathOfFile1 = dr["PathOfFile1"].ToString(),
                                                 PathOfFile2 = dr["pathoffile2"].ToString(),
                                                 PathOfFile3 = dr["pathoffile3"].ToString(),
                                                 BalanceSheetClosed = dr["BalanceSheetClosed"].ToString(),
                                                 PORemarks = dr["PORemarks"].ToString(),
                                                 UID = dr["Uid"].ToString(),
                                                 ItemOrService = dr["Item/Service"].ToString(),
                                                 UpdatedByName = dr["LastUpdatedBy"].ToString(),
                                                 EnteredBy = dr["EntryByMachine"].ToString(),
                                                 UpdatedOn = string.IsNullOrEmpty(dr["LastUpdatedDate"].ToString()) ? new DateTime() : Convert.ToDateTime(dr["LastUpdatedDate"]),
                                                 CreatedBy = string.IsNullOrEmpty(dr["Uid"].ToString()) ? 0 : Convert.ToInt32(dr["Uid"].ToString()),
                                                 CreatedOn = string.IsNullOrEmpty(dr["ActualEntryDate"].ToString()) ? new DateTime() : Convert.ToDateTime(dr["ActualEntryDate"]),
                                                 BillAmount = !string.IsNullOrEmpty(dr["BillAmt"].ToString()) ? Convert.ToSingle(dr["BillAmt"]) : 0,
                                                 TaxableAmount = !string.IsNullOrEmpty(dr["Taxableamt"].ToString()) ? Convert.ToSingle(dr["Taxableamt"]) : 0,
                                                 GSTAmount = !string.IsNullOrEmpty(dr["GSTAmount"].ToString()) ? Convert.ToSingle(dr["GSTAmount"]) : 0,
                                                 RoundOffAmt = !string.IsNullOrEmpty(dr["RoundOffAmt"].ToString()) ? Convert.ToSingle(dr["RoundOffAmt"]) : 0,
                                                 NetAmt = !string.IsNullOrEmpty(dr["NetAmt"].ToString()) ? Convert.ToSingle(dr["NetAmt"]) : 0,
                                                 PendAmt = !string.IsNullOrEmpty(dr["PendAmt"].ToString()) ? Convert.ToSingle(dr["PendAmt"]) : 0,
                                                 PaidAmt = !string.IsNullOrEmpty(dr["PaidAmt"].ToString()) ? Convert.ToSingle(dr["PaidAmt"]) : 0,
                                                 MRNType = dr["PurchaseBillTypeMRNJWChallan"].ToString(),
                                                 DocumentName = dr["DocumentType"].ToString(),
                                                 PartCode = dr["PartCode"].ToString(),
                                                 ItemName = dr["ItemName"].ToString(),
                                                 HSNNO = dr["HSNNO"].ToString(),
                                             }).OrderBy(a => a.EntryID).ToList();
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
            oDataSet.Dispose();
        }
        return DashBoardData;
    }

    public async Task<PBDashBoard> GetDetailData(PBDashBoard model)
    {
        var DashBoardData = new PBDashBoard();
        DataSet? oDataSet = new DataSet();
        var model1 = new PBDashBoard();
        try
        {
            using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
            {
                DateTime now = DateTime.Now;
                DateTime firstDayOfMonth = new DateTime(now.Year, now.Month, 1);
                DateTime fromDate = DateTime.ParseExact(model.FromDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                DateTime toDate = DateTime.ParseExact(model.ToDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                SqlCommand oCmd = new SqlCommand("AccSP_PurchaseBillMainDetail", myConnection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                oCmd.Parameters.AddWithValue("@Flag", "DASHBOARD");
                oCmd.Parameters.AddWithValue("@FlagMRNJWCHALLAN", !string.IsNullOrEmpty(model.MRNType) ? (model.MRNType != "0" ? model.MRNType : string.Empty) : string.Empty);
                oCmd.Parameters.AddWithValue("@SummDetail", model.DashboardType?.ToLower() ?? "summary");
                oCmd.Parameters.AddWithValue("@FromDate", fromDate);
                oCmd.Parameters.AddWithValue("@ToDate", toDate);
                oCmd.Parameters.AddWithValue("@PurchVoucherNo", !string.IsNullOrEmpty(model.VoucherNo) ? (model.VoucherNo != "0" ? model.VoucherNo : string.Empty) : string.Empty);
                oCmd.Parameters.AddWithValue("@InvNo", !string.IsNullOrEmpty(model.InvoiceNo) ? (model.InvoiceNo != "0" ? model.InvoiceNo : string.Empty) : string.Empty);
                oCmd.Parameters.AddWithValue("@partcode", !string.IsNullOrEmpty(model.PartCode) ? (model.PartCode != "0" ? model.PartCode : string.Empty) : string.Empty);
                oCmd.Parameters.AddWithValue("@Vendorname", !string.IsNullOrEmpty(model.VendorName) ? (model.VendorName != "0" ? model.VendorName : string.Empty) : string.Empty);
                oCmd.Parameters.AddWithValue("@ItemName", !string.IsNullOrEmpty(model.ItemName) ? (model.ItemName != "0" ? model.ItemName : string.Empty) : string.Empty);
                oCmd.Parameters.AddWithValue("@GSTType", !string.IsNullOrEmpty(model.GSTType) ? (model.GSTType != "0" ? model.GSTType : string.Empty) : string.Empty);
                oCmd.Parameters.AddWithValue("@TypeITEMSERVASSETS", !string.IsNullOrEmpty(model.ItemOrService) ? (model.ItemOrService != "0" ? model.ItemOrService : string.Empty) : string.Empty);
                oCmd.Parameters.AddWithValue("@DocumentType", !string.IsNullOrEmpty(model.DocumentName) ? (model.DocumentName != "0" ? model.DocumentName : string.Empty) : string.Empty);
                oCmd.Parameters.AddWithValue("@HSNNo", !string.IsNullOrEmpty(model.HSNNO) ? (model.HSNNO != "0" ? model.HSNNO : string.Empty) : string.Empty);
                await myConnection.OpenAsync();
                using (SqlDataAdapter oDataAdapter = new SqlDataAdapter(oCmd))
                {
                    oDataAdapter.Fill(oDataSet);
                }
            }

            if (model.DashboardType != null && model.DashboardType.ToLower() == "taxdetail")
            {
                if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                {
                    DashBoardData.PBDashboard = (from DataRow dr in oDataSet.Tables[0].Rows
                                                 select new PBDashBoard
                                                 {
                                                     //EntryID = !string.IsNullOrEmpty(dr["PurchBillEntryId"].ToString()) ? Convert.ToInt32(dr["PurchBillEntryId"]) : 0,
                                                     //PurchBillEntryId = !string.IsNullOrEmpty(dr["PurchBillEntryId"].ToString()) ? dr["PurchBillEntryId"].ToString() : string.Empty,
                                                     //PurchBillYearCode = !string.IsNullOrEmpty(dr["PurchBillYearCode"].ToString()) ? Convert.ToInt32(dr["PurchBillYearCode"]) : 0,
                                                     VoucherNo = dr["PurchvoucherNo"].ToString(),
                                                     InvoiceNo = dr["InvoiceNo"].ToString(),
                                                     InvoiceDate = string.IsNullOrEmpty(dr["InvoiceDate"].ToString()) ? new DateTime() : Convert.ToDateTime(dr["InvoiceDate"]),
                                                     PurchaseBillDirectPB = !string.IsNullOrEmpty(dr["PurchaseBillDirectPB"].ToString()) ? dr["PurchaseBillDirectPB"].ToString() : string.Empty,
                                                     VendorName = dr["VendorName"].ToString(),
                                                     PartCode = dr["PartCode"].ToString(),
                                                     ItemName = dr["ItemName"].ToString(),
                                                     BillAmount = !string.IsNullOrEmpty(dr["BillAmt"].ToString()) ? Convert.ToSingle(dr["BillAmt"]) : 0,
                                                     NetAmt = !string.IsNullOrEmpty(dr["NetAmt"].ToString()) ? Convert.ToSingle(dr["NetAmt"]) : 0,
                                                     TaxableAmount = !string.IsNullOrEmpty(dr["TaxableAmt"].ToString()) ? Convert.ToSingle(dr["TaxableAmt"]) : 0,
                                                     ExpenseHead = !string.IsNullOrEmpty(dr["ExpenseHead"].ToString()) ? dr["ExpenseHead"].ToString() : string.Empty,
                                                     ExpenseAmt = !string.IsNullOrEmpty(dr["ExpenseAmt"].ToString()) ? Convert.ToDecimal(dr["ExpenseAmt"]) : 0,
                                                     CGSTHead = !string.IsNullOrEmpty(dr["CGSTHead"].ToString()) ? dr["CGSTHead"].ToString() : string.Empty,
                                                     CGSTper = !string.IsNullOrEmpty(dr["CGSTper"].ToString()) ? Convert.ToDecimal(dr["CGSTper"]) : 0,
                                                     CGSTAmt = !string.IsNullOrEmpty(dr["CGSTAmt"].ToString()) ? Convert.ToDecimal(dr["CGSTAmt"]) : 0,
                                                     SGSTHead = !string.IsNullOrEmpty(dr["SGSTHead"].ToString()) ? dr["SGSTHead"].ToString() : string.Empty,
                                                     SGSTper = !string.IsNullOrEmpty(dr["SGSTper"].ToString()) ? Convert.ToDecimal(dr["SGSTper"]) : 0,
                                                     SGSTAmt = !string.IsNullOrEmpty(dr["SGSTAmt"].ToString()) ? Convert.ToDecimal(dr["SGSTAmt"]) : 0,
                                                     IGSTHead = !string.IsNullOrEmpty(dr["IGSTHead"].ToString()) ? dr["IGSTHead"].ToString() : string.Empty,
                                                     IGSTper = !string.IsNullOrEmpty(dr["IGSTper"].ToString()) ? Convert.ToDecimal(dr["IGSTper"]) : 0,
                                                     IGSTAmt = !string.IsNullOrEmpty(dr["IGSTAmt"].ToString()) ? Convert.ToDecimal(dr["IGSTAmt"]) : 0,
                                                     GSTAmount = !string.IsNullOrEmpty(dr["GSTAmount"].ToString()) ? Convert.ToSingle(dr["GSTAmount"]) : 0,
                                                 }).OrderBy(a => a.EntryID).ToList();
                }
            }
            else
            {
                if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                {
                    DashBoardData.PBDashboard = (from DataRow dr in oDataSet.Tables[0].Rows
                                                 select new PBDashBoard
                                                 {
                                                     EntryID = !string.IsNullOrEmpty(dr["PurchBillEntryId"].ToString()) ? Convert.ToInt32(dr["PurchBillEntryId"]) : 0,
                                                     PurchBillEntryId = !string.IsNullOrEmpty(dr["PurchBillEntryId"].ToString()) ? dr["PurchBillEntryId"].ToString() : "",
                                                     EntryDate = string.IsNullOrEmpty(dr["EntryDate"].ToString()) ? new DateTime() : Convert.ToDateTime(dr["EntryDate"]),
                                                     InvoiceNo = dr["InvoiceNo"].ToString(),
                                                     InvoiceDate = string.IsNullOrEmpty(dr["InvoiceDate"].ToString()) ? new DateTime() : Convert.ToDateTime(dr["InvoiceDate"]),
                                                     VoucherNo = dr["VoucherNo"].ToString(),
                                                     VoucherDate = dr["VoucherDate"].ToString(),
                                                     VendorName = dr["VendorName"].ToString(),
                                                     VendorAddress = dr["VendorAddress"].ToString(),
                                                     StateName = dr["StateName"].ToString(),
                                                     GSTType = dr["GSTType"].ToString(),
                                                     MRNNo = dr["MRNNo"].ToString(),
                                                     MRNDate = dr["MRNDate"].ToString(),
                                                     GateNo = dr["GateNo"].ToString(),
                                                     GateDate = dr["GateDate"].ToString(),
                                                     DomesticImport = dr["DomesticImport"].ToString(),
                                                     Currency = dr["Currency"].ToString(),
                                                     PurchBillYearCode = !string.IsNullOrEmpty(dr["PurchBillYearCode"].ToString()) ? Convert.ToInt32(dr["PurchBillYearCode"]) : 0,
                                                     PurchaseBillDirectPB = !string.IsNullOrEmpty(dr["PurchaseBillDirectPB"].ToString()) ? dr["PurchaseBillDirectPB"].ToString() : "",
                                                     Branch = dr["Branch"].ToString(),
                                                     PaymentTerm = dr["PaymentTerm"].ToString(),
                                                     Transporter = dr["Transporter"].ToString(),
                                                     VehicleNo = dr["Vehicleno"].ToString(),
                                                     ExchangeRate = dr["ExchangeRate"].ToString(),
                                                     RoundoffType = dr["RoundoffType"].ToString(),
                                                     PONetAmt = !string.IsNullOrEmpty(dr["PONetAmt"].ToString()) ? Convert.ToSingle(dr["PONetAmt"]) : 0,
                                                     TotalDiscountPercent = !string.IsNullOrEmpty(dr["ToatlDiscountPercent"].ToString()) ? Convert.ToSingle(dr["ToatlDiscountPercent"]) : 0,
                                                     TDSAmount = !string.IsNullOrEmpty(dr["TDSAmount"].ToString()) ? Convert.ToSingle(dr["TDSAmount"]) : 0,
                                                     Remark = dr["Remark"].ToString(),
                                                     ModeOfTrans = dr["ModeOfTrans"].ToString(),
                                                     Approved = dr["Approved"].ToString(),
                                                     ApprovedDate = dr["ApprovedDate"].ToString(),
                                                     ApprovedBy = dr["Approvedby"].ToString(),
                                                     BOEDate = dr["BOEDate"].ToString(),
                                                     TotalAmtInOtherCurr = dr["TotalAmtInOtherCurr"].ToString(),
                                                     boeno = dr["boeno"].ToString(),
                                                     Commodity = dr["Commodity"].ToString(),
                                                     PathOfFile1 = dr["PathOfFile1"].ToString(),
                                                     PathOfFile2 = dr["pathoffile2"].ToString(),
                                                     PathOfFile3 = dr["pathoffile3"].ToString(),
                                                     BalanceSheetClosed = dr["BalanceSheetClosed"].ToString(),
                                                     PORemarks = dr["PORemarks"].ToString(),
                                                     UID = dr["Uid"].ToString(),
                                                     ItemOrService = dr["Item/Service"].ToString(),
                                                     PartCode = dr["PartCode"].ToString(),
                                                     ItemName = dr["ItemName"].ToString(),
                                                     HSNNO = dr["HSNNO"].ToString(),
                                                     Unit = dr["Unit"].ToString(),
                                                     NoOfCase = dr["NoOfCase"].ToString(),
                                                     BillQty = dr["BillQty"].ToString(),
                                                     RecQty = dr["RecQty"].ToString(),
                                                     Rate = dr["Rate"].ToString(),
                                                     PORate = dr["PORate"].ToString(),
                                                     DiscountPer = dr["DiscountPer"].ToString(),
                                                     DiscountAmt = dr["DiscountAmt"].ToString(),
                                                     PONo = dr["PONo"].ToString(),
                                                     PODate = dr["PODate"].ToString(),
                                                     POType = dr["POType"].ToString(),
                                                     SchNo = dr["SchNo"].ToString(),
                                                     SchDate = dr["SchDate"].ToString(),
                                                     ItemSize = dr["Itemsize"].ToString(),
                                                     OtherDetail = dr["OtherDetail"].ToString(),
                                                     MRP = dr["MRP"].ToString(),
                                                     RateUnit = dr["RateUnit"].ToString(),
                                                     RateIncludingTaxes = dr["RateIncludingTaxes"].ToString(),
                                                     AmtinOtherCurr = dr["AmtinOtherCurr"].ToString(),
                                                     RateConversionFactor = dr["RateConversionFactor"].ToString(),
                                                     CostCenterName = dr["CostCenterName"].ToString(),
                                                     ItemColor = dr["ItemColor"].ToString(),
                                                     POYearCode = dr["POYearCode"].ToString(),
                                                     ItemModel = dr["ItemModel"].ToString(),
                                                     SchYearCode = dr["SchYearCode"].ToString(),
                                                     POAmmNo = dr["POAmmNo"].ToString(),
                                                     PoRate = dr["PoRate"].ToString(),
                                                     ProjectNo = dr["ProjectNo"].ToString(),
                                                     ProjectDate = dr["ProjectDate"].ToString(),
                                                     ProjectYearCode = dr["ProjectYearCode"].ToString(),
                                                     AgainstImportAccountCode = dr["AgainstImportAccountCode"].ToString(),
                                                     AgainstImportInvoiceNo = dr["AgainstImportInvoiceNo"].ToString(),
                                                     AgainstImportYearCode = dr["AgainstImportYearCode"].ToString(),
                                                     AgainstImportInvDate = dr["AgainstImportInvDate"].ToString(),
                                                     UpdatedByName = dr["EntryByMachine"].ToString(),
                                                     UpdatedBy = string.IsNullOrEmpty(dr["LastUpdatedBy"].ToString()) ? 0 : Convert.ToInt32(dr["LastUpdatedBy"].ToString()),
                                                     EnteredBy = dr["EntryByMachine"].ToString(),
                                                     UpdatedOn = string.IsNullOrEmpty(dr["LastUpdatedDate"].ToString()) ? new DateTime() : Convert.ToDateTime(dr["LastUpdatedDate"]),
                                                     CreatedBy = string.IsNullOrEmpty(dr["Uid"].ToString()) ? 0 : Convert.ToInt32(dr["Uid"].ToString()),
                                                     CreatedOn = string.IsNullOrEmpty(dr["ActualEntryDate"].ToString()) ? new DateTime() : Convert.ToDateTime(dr["ActualEntryDate"]),
                                                     BillAmount = !string.IsNullOrEmpty(dr["BillAmt"].ToString()) ? Convert.ToSingle(dr["BillAmt"]) : 0,
                                                     TaxableAmount = !string.IsNullOrEmpty(dr["Taxableamt"].ToString()) ? Convert.ToSingle(dr["Taxableamt"]) : 0,
                                                     GSTAmount = !string.IsNullOrEmpty(dr["GSTAmount"].ToString()) ? Convert.ToSingle(dr["GSTAmount"]) : 0,
                                                     RoundOffAmt = !string.IsNullOrEmpty(dr["RoundOffAmt"].ToString()) ? Convert.ToSingle(dr["RoundOffAmt"]) : 0,
                                                     NetAmt = !string.IsNullOrEmpty(dr["NetAmt"].ToString()) ? Convert.ToSingle(dr["NetAmt"]) : 0,
                                                     Amount = !string.IsNullOrEmpty(dr["Amount"].ToString()) ? Convert.ToDecimal(dr["Amount"]) : 0,
                                                     MRNType = dr["PurchaseBillTypeMRNJWChallan"].ToString(),
                                                     DocumentName = dr["DocumentType"].ToString(),
                                                 }).OrderBy(a => a.EntryID).ToList();
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
            oDataSet.Dispose();
        }
        return DashBoardData;
    }

    public async Task<PurchaseBillModel> GetViewByID(int ID, int YC, string Flag)
    {
        var oDataSet = new DataSet();
        var MainModel = new PurchaseBillModel();
        var _ItemList = new List<PBItemDetail>();
        var _TaxList = new List<TaxModel>();
        var _TdsList = new List<TDSModel>();
        MainModel.adjustmentModel = new AdjustmentModel();
        var _AdjList = new List<AdjustmentModel>();
        var SqlParams = new List<dynamic>();
        //var listObject = new List<DeliverySchedule>();

        try
        {
            SqlParams.Add(new SqlParameter("@flag", "ViewByID"));
            SqlParams.Add(new SqlParameter("@EntryID", ID));
            SqlParams.Add(new SqlParameter("@YearCode", YC));

            var ResponseResult = await _IDataLogic.ExecuteDataSet("AccSP_PurchaseBillMainDetail", SqlParams);

            if (((DataSet)ResponseResult.Result).Tables.Count > 0 && ((DataSet)ResponseResult.Result).Tables[0].Rows.Count > 0)
            {
                oDataSet = ResponseResult.Result;
                oDataSet.Tables[0].TableName = "PBMain";
                oDataSet.Tables[1].TableName = "ItemDetailGrid";
                oDataSet.Tables[2].TableName = "PBTaxDetail";
                oDataSet.Tables[3].TableName = "PBTDSDetail";
                oDataSet.Tables[4].TableName = "PBAdjDetail";

                if (oDataSet.Tables.Count != 0 && oDataSet.Tables[0].Rows.Count > 0)
                {
                    MainModel.EntryID = Convert.ToInt32(oDataSet.Tables[0].Rows[0]["PurchBillEntryId"]);
                    MainModel.YearCode = Convert.ToInt32(oDataSet.Tables[0].Rows[0]["PurchBillYearCode"]);
                    MainModel.EntryDate = oDataSet.Tables[0].Rows[0]["EntryDate"].ToString();
                    MainModel.Branch = oDataSet.Tables[0].Rows[0]["CC"].ToString();
                    MainModel.CurrencyId = Convert.ToInt32(oDataSet.Tables[0].Rows[0]["CurrencyId"]);
                    MainModel.Currency = oDataSet.Tables[0].Rows[0]["Currency"].ToString();
                    MainModel.AccountCode = Convert.ToInt32(oDataSet.Tables[0].Rows[0]["AccountCode"]);
                    MainModel.InvNo = oDataSet.Tables[0].Rows[0]["InvoiceNo"].ToString();
                    MainModel.InvDate = oDataSet.Tables[0].Rows[0]["InvoiceDate"].ToString();
                    MainModel.PurchVouchNo = oDataSet.Tables[0].Rows[0]["purvoucherno"].ToString();
                    MainModel.VouchDate = oDataSet.Tables[0].Rows[0]["VoucherDate"].ToString();
                    MainModel.VendorStateName = oDataSet.Tables[0].Rows[0]["StateName"].ToString();
                    MainModel.StateName = oDataSet.Tables[0].Rows[0]["StateName"].ToString();
                    MainModel.VendorAddress = oDataSet.Tables[0].Rows[0]["VendorAddress"].ToString();
                    MainModel.GSTType = oDataSet.Tables[0].Rows[0]["GSTType"].ToString();
                    MainModel.PBType = oDataSet.Tables[0].Rows[0]["DomesticImport"].ToString();
                    MainModel.PaymentTerms = oDataSet.Tables[0].Rows[0]["PaymentTerm"].ToString();
                    MainModel.ModeOfTransport = oDataSet.Tables[0].Rows[0]["ModeOfTrans"].ToString();
                    MainModel.Transport = oDataSet.Tables[0].Rows[0]["Transporter"].ToString();
                    MainModel.VehicleNo = oDataSet.Tables[0].Rows[0]["Vehicleno"].ToString();
                    MainModel.VendorName = oDataSet.Tables[0].Rows[0]["VendorName"].ToString();
                    MainModel.MRNNo = oDataSet.Tables[0].Rows[0]["MRNNo"].ToString();
                    MainModel.MRNYearCode = string.IsNullOrEmpty(oDataSet.Tables[0].Rows[0]["MRNYearCode"].ToString()) ? 0 : Convert.ToInt32(oDataSet.Tables[0].Rows[0]["MRNYearCode"]); 
                    MainModel.StrMRNEntryDate = oDataSet.Tables[0].Rows[0]["MRNDate"].ToString();
                    MainModel.GateNo = oDataSet.Tables[0].Rows[0]["GateNo"].ToString();
                    MainModel.GateYearCode = string.IsNullOrEmpty(oDataSet.Tables[0].Rows[0]["GateYearcode"].ToString()) ? 0 : Convert.ToInt32(oDataSet.Tables[0].Rows[0]["GateYearcode"]); 
                    MainModel.StrGateDate = oDataSet.Tables[0].Rows[0]["GateDate"].ToString();
                    MainModel.GSTNO = oDataSet.Tables[0].Rows[0]["GSTNO"].ToString();
                    MainModel.GSTRegistered = oDataSet.Tables[0].Rows[0]["GSTRegistered"].ToString();
                    MainModel.MRNRemark = oDataSet.Tables[0].Rows[0]["MrnRemark"].ToString(); 
                    MainModel.PurchaseBillDirectPB = oDataSet.Tables[0].Rows[0]["PurchaseBillDirectPB"].ToString();
                    MainModel.TypeITEMSERVASSETS = oDataSet.Tables[0].Rows[0]["TypeITEMSERVASSETS"].ToString();
                    MainModel.PurchaseBillTypeMRNJWChallan = oDataSet.Tables[0].Rows[0]["PurchaseBillTypeMRNJWChallan"].ToString();
                    MainModel.ExchangeRate = string.IsNullOrEmpty(oDataSet.Tables[0].Rows[0]["ExchangeRate"].ToString()) ? 0 : Convert.ToSingle(oDataSet.Tables[0].Rows[0]["ExchangeRate"]);
                    MainModel.RateOfConvFactor = string.IsNullOrEmpty(oDataSet.Tables[0].Rows[0]["ConversionFactor"].ToString()) ? 0 : Convert.ToInt32(oDataSet.Tables[0].Rows[0]["ConversionFactor"]);
                    MainModel.ItemNetAmount = string.IsNullOrEmpty(oDataSet.Tables[0].Rows[0]["NetAmt"].ToString()) ? 0 : Convert.ToDecimal(oDataSet.Tables[0].Rows[0]["NetAmt"]);
                    MainModel.GSTAmount = string.IsNullOrEmpty(oDataSet.Tables[0].Rows[0]["GSTAmount"].ToString()) ? 0 : Convert.ToDecimal(oDataSet.Tables[0].Rows[0]["GSTAmount"]);
                    MainModel.Taxableamt = string.IsNullOrEmpty(oDataSet.Tables[0].Rows[0]["Taxableamt"].ToString()) ? 0 : Convert.ToDecimal(oDataSet.Tables[0].Rows[0]["Taxableamt"]);
                    MainModel.NetTotal = string.IsNullOrEmpty(oDataSet.Tables[0].Rows[0]["BillAmt"].ToString()) ? 0 : (oDataSet.Tables[0].Rows[0]["RoundoffType"].ToString().ToLower() == "y") ? Convert.ToDecimal(Math.Round(Convert.ToDecimal(oDataSet.Tables[0].Rows[0]["BillAmt"]))) : Convert.ToDecimal(oDataSet.Tables[0].Rows[0]["BillAmt"]);
                    MainModel.PaymentDays = string.IsNullOrEmpty(oDataSet.Tables[0].Rows[0]["PaymentDays"].ToString()) ? 0 : Convert.ToInt32(oDataSet.Tables[0].Rows[0]["PaymentDays"]);
                    MainModel.PreparedBy = string.IsNullOrEmpty(oDataSet.Tables[0].Rows[0]["ActualEntryBy"].ToString()) ? 0 : Convert.ToInt32(oDataSet.Tables[0].Rows[0]["ActualEntryBy"]);
                    MainModel.PreparedByName = string.IsNullOrEmpty(oDataSet.Tables[0].Rows[0]["EntryByMachine"].ToString()) ? string.Empty : oDataSet.Tables[0].Rows[0]["EntryByMachine"].ToString();
                    MainModel.PathOfFile1URL = oDataSet.Tables[0].Rows[0]["PathOfFile1"].ToString();
                    MainModel.PathOfFile2URL = oDataSet.Tables[0].Rows[0]["pathoffile2"].ToString();
                    MainModel.PathOfFile3URL = oDataSet.Tables[0].Rows[0]["pathoffile3"].ToString();
                    MainModel.TotalDiscountPercentage = string.IsNullOrEmpty(oDataSet.Tables[0].Rows[0]["ToatlDiscountPercent"].ToString()) ? 0 : Convert.ToDecimal(oDataSet.Tables[0].Rows[0]["ToatlDiscountPercent"]);
                    MainModel.TotalAmtAftrDiscount = string.IsNullOrEmpty(oDataSet.Tables[0].Rows[0]["TotalDiscountAmount"].ToString()) ? 0 : Convert.ToDecimal(oDataSet.Tables[0].Rows[0]["TotalDiscountAmount"]);
                    MainModel.TotalRoundOff = oDataSet.Tables[0].Rows[0]["RoundoffType"].ToString();
                    MainModel.TotalRoundOffAmt = Convert.ToDecimal(oDataSet.Tables[0].Rows[0]["RoundOffAmt"]);
                    MainModel.UID = Convert.ToInt32(oDataSet.Tables[0].Rows[0]["Uid"].ToString());
                    MainModel.CreatedBy = Convert.ToInt32(oDataSet.Tables[0].Rows[0]["ActualEntryBy"].ToString());
                    MainModel.CretaedByName = oDataSet.Tables[0].Rows[0]["EntryByMachine"].ToString();
                    MainModel.Approved = oDataSet.Tables[0].Rows[0]["Approved"].ToString();
                    MainModel.ApprovedDate = oDataSet.Tables[0].Rows[0]["ApprovedDate"].ToString();
                    MainModel.Approvedby = Convert.ToInt32(oDataSet.Tables[0].Rows[0]["Approvedby"].ToString());
                    MainModel.CretaedByName = oDataSet.Tables[0].Rows[0]["EntryByMachine"].ToString();
                    MainModel.TaxVariationPOvsBill = oDataSet.Tables[0].Rows[0]["TaxVariationPOvsBill"].ToString();

                    MainModel.Remark = string.IsNullOrEmpty(oDataSet.Tables[0].Rows[0]["Remark"].ToString()) ? string.Empty : oDataSet.Tables[0].Rows[0]["Remark"].ToString();
                    MainModel.CreatedOn = string.IsNullOrEmpty(oDataSet.Tables[0].Rows[0]["ActualEntryDate"].ToString()) ? new DateTime() : Convert.ToDateTime(oDataSet.Tables[0].Rows[0]["ActualEntryDate"]);
                    if (!string.IsNullOrEmpty(oDataSet.Tables[0].Rows[0]["UpdatedBy"].ToString()))
                    {
                        MainModel.UpdatedBy = string.IsNullOrEmpty(oDataSet.Tables[0].Rows[0]["UpdatedBy"].ToString()) ? 0 : Convert.ToInt32(oDataSet.Tables[0].Rows[0]["UpdatedBy"]);
                        MainModel.UpdatedOn = string.IsNullOrEmpty(oDataSet.Tables[0].Rows[0]["LastUpdatedDate"].ToString()) ? new DateTime() : Convert.ToDateTime(oDataSet.Tables[0].Rows[0]["LastUpdatedDate"]);
                    }
                }
                int cnt = 1;
                if (oDataSet.Tables.Count != 0 && oDataSet.Tables[1].Rows.Count > 0)
                {
                    foreach (DataRow row in oDataSet.Tables[1].Rows)
                    {
                        _ItemList.Add(new PBItemDetail
                        {
                            SeqNo = cnt++,
                            DocTypeText = row["DocumentType"].ToString(),
                            ItemText = (!string.IsNullOrEmpty(row["ItemName"].ToString())) ? row["ItemName"].ToString() : string.Empty,
                            PBQty = (!string.IsNullOrEmpty(row["RecQty"].ToString())) ? Convert.ToDecimal(row["RecQty"]) : 0,
                            Rate = (!string.IsNullOrEmpty(row["Rate"].ToString())) ? Convert.ToDecimal(row["Rate"]) : 0,
                            OtherRateCurr = (!string.IsNullOrEmpty(row["AmtinOtherCurr"].ToString())) ? Convert.ToDecimal(row["AmtinOtherCurr"]) : 0,
                            Amount = Convert.ToDecimal(row["Amount"]),
                            PartText = row["PartCode"].ToString(),
                            Description = row["OtherDetail"].ToString(),
                            UnitRate = row["RateUnit"].ToString(),
                            Process = (!string.IsNullOrEmpty(row["ProcessId"].ToString())) ? Convert.ToInt32(row["ProcessId"]) : 0,
                            CostCenter = (!string.IsNullOrEmpty(row["CostCenterId"].ToString())) ? Convert.ToInt32(row["CostCenterId"]) : 0,
                            CostCenterName = row["CostCenterName"].ToString(),

                            ParentCode = row["ParentCode"].ToString(),
                            DocumentName = row["DocumentType"].ToString(),
                            DocTypeID = string.IsNullOrEmpty(row["DocTypeID"].ToString()) ? 0 : Convert.ToInt32(row["DocTypeID"]),
                            ItemCode = string.IsNullOrEmpty(row["ItemCode"].ToString()) ? 0 : Convert.ToInt32(row["ItemCode"]),
                            Item_Name = row["ItemName"].ToString(),
                            PartCode = row["PartCode"].ToString(),
                            HSNNO = string.IsNullOrEmpty(row["HSNNO"].ToString()) ? 0 : Convert.ToInt32(row["HSNNO"]),
                            BillQty = !string.IsNullOrEmpty(row["BillQty"].ToString()) ? Convert.ToDecimal(row["BillQty"]) : 0,
                            RecQty = !string.IsNullOrEmpty(row["RecQty"].ToString()) ? Convert.ToDecimal(row["RecQty"]) : 0,
                            rejectedQty = !string.IsNullOrEmpty(row["rejectedQty"].ToString()) ? Convert.ToDecimal(row["rejectedQty"]) : 0,
                            Unit = row["unit"].ToString(),
                            NoOfCase = string.IsNullOrEmpty(row["NoOfCase"].ToString()) ? 0 : Convert.ToInt32(row["NoOfCase"].ToString()),
                            AcceptedQty = !string.IsNullOrEmpty(row["AcceptedQty"].ToString()) ? Convert.ToDecimal(row["AcceptedQty"]) : 0,
                            ReworkQty = !string.IsNullOrEmpty(row["ReworkQty"].ToString()) ? Convert.ToDecimal(row["ReworkQty"]) : 0,
                            HoldQty = !string.IsNullOrEmpty(row["HoldQty"].ToString()) ? Convert.ToDecimal(row["HoldQty"]) : 0,
                            AltRecQty = !string.IsNullOrEmpty(row["AltRecQty"].ToString()) ? Convert.ToDecimal(row["AltRecQty"]) : 0,
                            AltUnit = row["AltUnit"].ToString(),
                            BillRate = !string.IsNullOrEmpty(row["Rate"].ToString()) ? Convert.ToDecimal(row["Rate"]) : 0,
                            MRP = !string.IsNullOrEmpty(row["MRP"].ToString()) ? Convert.ToDecimal(row["MRP"]) : 0,
                            RateIncludingTax = !string.IsNullOrEmpty(row["RateIncludingTaxes"].ToString()) ? Convert.ToDecimal(row["RateIncludingTaxes"]) : 0,
                            AmtInOtherCurrency = !string.IsNullOrEmpty(row["AmtinOtherCurr"].ToString()) ? Convert.ToInt32(row["AmtinOtherCurr"]) : 0,
                            RateOfConvFactor = !string.IsNullOrEmpty(row["RateConversionFactor"].ToString()) ? Convert.ToInt32(row["RateConversionFactor"]) : 0,
                            AssessRate = !string.IsNullOrEmpty(row["AssesAmount"].ToString()) ? Convert.ToDecimal(row["AssesAmount"]) : 0,
                            DisPer = !string.IsNullOrEmpty(row["DiscountPer"].ToString()) ? Convert.ToInt32(row["DiscountPer"]) : 0,
                            DisAmt = !string.IsNullOrEmpty(row["DiscountAmt"].ToString()) ? Convert.ToInt32(row["DiscountAmt"]) : 0,
                            ItemSize = row["ItemSize"].ToString(),
                            ItemColor = row["ItemColor"].ToString(),
                            ItemModel = row["ItemModel"].ToString(),
                            DepartmentId = !string.IsNullOrEmpty(row["Deaprtmentid"].ToString()) ? Convert.ToInt32(row["Deaprtmentid"]) : 0,
                            ForDepartment = row["DeptName"].ToString(),
                            ProcessId = !string.IsNullOrEmpty(row["ProcessId"].ToString()) ? Convert.ToInt32(row["ProcessId"]) : 0,
                            ProcessName = row["ProcessName"].ToString(),
                            NewPoRate = !string.IsNullOrEmpty(row["NewPoRate"].ToString()) ? Convert.ToDecimal(row["NewPoRate"]) : 0,
                            pono = row["pono"].ToString(),
                            poyearcode = string.IsNullOrEmpty(row["poyearcode"].ToString()) ? 0 : Convert.ToInt32(row["poyearcode"].ToString()),
                            PODate = row["PODate"].ToString(),
                            schno = row["schno"].ToString(),
                            schyearcode = string.IsNullOrEmpty(row["schyearcode"].ToString()) ? 0 : Convert.ToInt32(row["schyearcode"].ToString()),
                            SchDate = row["SchDate"].ToString(),
                            PoAmendNo = string.IsNullOrEmpty(row["POAmmNo"].ToString()) ? 0 : Convert.ToInt32(row["POAmmNo" +
                            ""].ToString()),
                            PORate = string.IsNullOrEmpty(row["PORate"].ToString()) ? 0 : Convert.ToDecimal(row["PORate"].ToString()),
                            MIRNO = row["MIRNO"].ToString(),
                            MIRYEARCODE = string.IsNullOrEmpty(row["MIRYearCode"].ToString()) ? 0 : Convert.ToInt32(row["MIRYearCode"].ToString()),
                            MIRDATE = row["MIRDate"].ToString(),
                            ProjectNo = row["ProjectNo"].ToString(),
                            ProjectDate = row["ProjectDate"].ToString(),
                            ProjectyearCode = string.IsNullOrEmpty(row["ProjectYearCode"].ToString()) ? 0 : Convert.ToInt32(row["ProjectYearCode"].ToString()),
                            AgainstImportAccountCode = string.IsNullOrEmpty(row["AgainstImportAccountCode"].ToString()) ? 0 : Convert.ToInt32(row["AgainstImportAccountCode"].ToString()),
                            AgainstImportInvoiceNo = string.IsNullOrEmpty(row["AgainstImportInvoiceNo"].ToString()) ? 0 : Convert.ToInt32(row["AgainstImportInvoiceNo"].ToString()),
                            AgainstImportYearCode = string.IsNullOrEmpty(row["AgainstImportYearCode"].ToString()) ? 0 : Convert.ToInt32(row["AgainstImportYearCode"].ToString()),
                            AgainstImportInvDate = row["AgainstImportInvDate"].ToString(),
                        });
                    }

                    MainModel.ItemDetailGridd = _ItemList;
                    MainModel.ItemDetailGrid = new List<PBItemDetail>();
                    MainModel.ItemNetAmount = decimal.Parse(MainModel.ItemDetailGridd.Sum(x => x.Amount ?? 0).ToString("#.#0"));
                }

                if (oDataSet.Tables.Count != 0 && oDataSet.Tables[2].Rows.Count > 0)
                {
                    foreach (DataRow row in oDataSet.Tables[2].Rows)
                    {
                        _TaxList.Add(new TaxModel
                        {
                            TxSeqNo = Convert.ToInt32(row["SeqNo"]),
                            TxType = row["Type"].ToString(),
                            TxPartCode = Convert.ToInt32(row["ItemCode"]),
                            TxPartName = row["PartCode"].ToString(),
                            TxItemCode = Convert.ToInt32(row["ItemCode"]),
                            TxItemName = row["ItemName"].ToString(),
                            TxOnExp = Convert.ToDecimal(row["TaxonExp"]),
                            TxPercentg = Convert.ToDecimal(row["TaxPer"]),
                            TxAdInTxable = row["AddInTaxable"].ToString(),
                            TxRoundOff = row["RoundOff"].ToString(),
                            TxTaxType = Convert.ToInt32(row["TaxTypeID"]),
                            TxTaxTypeName = row["Type"].ToString(),
                            TxAccountCode = Convert.ToInt32(row["TaxAccountCode"]),
                            TxAccountName = row["TaxAccountName"].ToString(),
                            TxAmount = Convert.ToDecimal(row["Amount"]),
                            TxRefundable = row["TaxRefundable"].ToString(),
                            TxRemark = row["Remarks"].ToString(),
                        });
                    }
                    MainModel.TaxDetailGridd = _TaxList;
                }

                if (oDataSet.Tables.Count != 0 && oDataSet.Tables[3].Rows.Count > 0)
                {
                    foreach (DataRow row in oDataSet.Tables[3].Rows)
                    {
                        _TdsList.Add(new TDSModel
                        {
                            TDSSeqNo = Convert.ToInt32(row["SeqNo"]),
                            TDSTaxType = Convert.ToInt32(row["TaxTypeID"]),
                            TDSTaxTypeName = row["TaxType"].ToString(),
                            TDSAccountCode = Convert.ToInt32(row["TaxNameCode"]),
                            TDSAccountName = row["TaxName"].ToString(),
                            TDSAmount = Convert.ToDecimal(row["TDSAmount"]),
                            TDSPercentg = Convert.ToDecimal(row["TaxPer"]),
                            TDSRoundOff = row["RoundOff"].ToString(),
                            TDSRoundOffAmt = Convert.ToDecimal(row["RoundOffAmt"]),
                            TDSRemark = row["Remark"].ToString(),
                        });
                    }
                    MainModel.TDSDetailGridd = _TdsList;
                }

                if (oDataSet.Tables.Count != 0 && oDataSet.Tables[5].Rows.Count > 0)
                {
                    var cnt1 = 1;
                    foreach (DataRow row in oDataSet.Tables[5].Rows)
                    {
                        _AdjList.Add(new AdjustmentModel
                        {
                            AdjSeqNo = cnt1,
                            AdjModeOfAdjstment = row["ModOfAdjust"].ToString(),
                            AdjDescription = row["Description"].ToString(),
                            AdjDueDate = string.IsNullOrEmpty(row["DueDate"].ToString()) ? new DateTime() : Convert.ToDateTime(row["DueDate"]),
                            AdjNewRefNo = row["NewRefNo"].ToString(),
                            AdjPendAmt = Convert.ToSingle(row["PendAmt"]),
                            AdjDrCr = row["DR/CR"].ToString(),
                            AdjPurchOrderNo = string.Empty, //row["RoundOff"].ToString(),
                            AdjPOYear = 0, //Convert.ToInt32(row["RoundOffAmt"]),
                            AdjPODate = new DateTime(),//string.IsNullOrEmpty(row["LastUpdatedDate"].ToString()) ? new DateTime() : Convert.ToDateTime(row["LastUpdatedDate"]),
                            AdjOpenEntryID = Convert.ToInt32(row["OpenEntryId"].ToString()),
                            AdjOpeningYearCode = Convert.ToInt32(row["OpeningYearCode"].ToString()),
                            AdjAgnstAccEntryID = Convert.ToInt32(row["AccEntryId"].ToString()),
                            AdjAgnstAccYearCode = Convert.ToInt32(row["AccYearCode"].ToString()),
                            AdjAgnstVouchNo = row["AgainstVoucherNo"].ToString(),
                            AdjAgnstVouchDate = string.IsNullOrEmpty(row["voucherDate"].ToString()) ? new DateTime() : Convert.ToDateTime(row["voucherDate"]),
                            AdjAgnstVouchType = row["VoucherType"].ToString(),
                            AdjAdjstedAmt = !string.IsNullOrEmpty(row["AdjustmentAmt"].ToString()) ? Convert.ToInt32(row["AdjustmentAmt"]) : 0,
                            AdjAgnstTotalAmt = !string.IsNullOrEmpty(row["BillAmt"].ToString()) ? Convert.ToInt32(row["BillAmt"]) : 0,
                        });
                        cnt1++;
                    }
                    MainModel.adjustmentModel.AdjAdjustmentDetailGrid = _AdjList;
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
            oDataSet.Dispose();
        }
        return MainModel;
    }

    public async Task<ResponseResult> GetItemCode(string PartCode)
    {
        var _ResponseResult = new ResponseResult();
        try
        {
            var SqlParams = new List<dynamic>();
            SqlParams.Add(new SqlParameter("@Flag", "GetItemCode"));
            SqlParams.Add(new SqlParameter("@PartCode", PartCode));

            _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_PurchaseBillMainDetail", SqlParams);

        }
        catch (Exception ex)
        {
            dynamic Error = new ExpandoObject();
            Error.Message = ex.Message;
            Error.Source = ex.Source;
        }
        return _ResponseResult;
    }
    public async Task<ResponseResult> GetReportName()
    {
        var _ResponseResult = new ResponseResult();
        try
        {
            var SqlParams = new List<dynamic>();
            SqlParams.Add(new SqlParameter("@Flag", "GetReportName"));

            _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_PurchaseBillMainDetail", SqlParams);

        }
        catch (Exception ex)
        {
            dynamic Error = new ExpandoObject();
            Error.Message = ex.Message;
            Error.Source = ex.Source;
        }
        return _ResponseResult;
    }


    public static DateTime ParseDate(string dateString)
    {
        if (string.IsNullOrEmpty(dateString))
        {
            return default;
        }

        if (DateTime.TryParseExact(dateString, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate))
        {
            return parsedDate;
        }
        else
        {
            return DateTime.Parse(dateString);
        }

        //    throw new FormatException("Invalid date format. Expected format: dd/MM/yyyy");

    }


    public async Task<ResponseResult> SavePurchaseBILL(DataTable ItemDetailDT, DataTable TaxDetailDT, DataTable TDSDetailDT, PurchaseBillModel model, DataTable DrCrDetailDT, DataTable AdjDetailDT)
    {
        var _ResponseResult = new ResponseResult();
        try
        {
            var SqlParams = new List<dynamic>();

            //DateTime EntryDt = new DateTime();
            //DateTime VouchDate = new DateTime();
            //DateTime InvDate = new DateTime();
            //DateTime GateDate = new DateTime();
            //DateTime MrnDate = new DateTime();
            //DateTime CurrentDate = new DateTime();
            //DateTime AppDate = new DateTime();

            var AppDate = "";
            var EntryDt = CommonFunc.ParseFormattedDate(model.EntryDate);
            var VouchDate = CommonFunc.ParseFormattedDate(model.VouchDate);
            var InvDate = CommonFunc.ParseFormattedDate(model.InvDate);
            var GateDate = CommonFunc.ParseFormattedDate(model.StrGateDate);
            var MrnDate = CommonFunc.ParseFormattedDate(model.StrMRNEntryDate);
            var CurrentDate = CommonFunc.ParseFormattedDate(DateTime.Today.ToString("dd/MM/yyyy"));

            SqlParams.Add(new SqlParameter("@Flag", model.Mode == "COPY" ? "INSERT" : model.Mode));
            //SqlParams.Add(new SqlParameter("@ID", model.ID));
            if (model.Mode == "INSERT")
            {
                SqlParams.Add(new SqlParameter("@EntryID", 0));
                EntryDt = CommonFunc.ParseFormattedDate(DateTime.Today.ToString("dd/MM/yyyy"));
            }
            else
            {
                SqlParams.Add(new SqlParameter("@EntryID", model.EntryID));
            }
            if (model.Mode == "POA")
            {
                AppDate = CommonFunc.ParseFormattedDate(model.ApprovedDate);
                SqlParams.Add(new SqlParameter("@ApprovedDate", AppDate == default ? string.Empty : AppDate));
                SqlParams.Add(new SqlParameter("@Approved", model.Approved));
                SqlParams.Add(new SqlParameter("@Approvedby", model.Approvedby));
            }

            SqlParams.Add(new SqlParameter("@YearCode", model.YearCode));
            SqlParams.Add(new SqlParameter("@EntryDate", EntryDt == default ? string.Empty : EntryDt));
            SqlParams.Add(new SqlParameter("@InvNo", model.InvNo));
            SqlParams.Add(new SqlParameter("@InvoiceTime", InvDate == default ? string.Empty : InvDate));
            SqlParams.Add(new SqlParameter("@InvoiceDate", InvDate == default ? string.Empty : InvDate));
            SqlParams.Add(new SqlParameter("@GateDate", GateDate == default ? string.Empty : GateDate));
            SqlParams.Add(new SqlParameter("@GateYearCode", model.GateYearCode));
            SqlParams.Add(new SqlParameter("@Gateno", model.GateNo));
            SqlParams.Add(new SqlParameter("@MrnDate", MrnDate == default ? string.Empty : MrnDate));
            SqlParams.Add(new SqlParameter("@mrnyearcode", model.MRNYearCode));
            SqlParams.Add(new SqlParameter("@Mrnno", model.MRNNo));
            SqlParams.Add(new SqlParameter("@PurchVoucherNo", model.PurchVouchNo));
            SqlParams.Add(new SqlParameter("@VoucherDate", VouchDate == default ? string.Empty : VouchDate));
            SqlParams.Add(new SqlParameter("@accountcode", model.AccountCode));
            SqlParams.Add(new SqlParameter("@StateName", model.StateName));
            SqlParams.Add(new SqlParameter("@GSTType", model.GSTType));
            SqlParams.Add(new SqlParameter("@TypeITEMSERVASSETS", model.TypeITEMSERVASSETS));
            SqlParams.Add(new SqlParameter("@PurchaseBillTypeMRNJWChallan", model.PurchaseBillTypeMRNJWChallan));
            SqlParams.Add(new SqlParameter("@DomesticImport", model.PBType));
            SqlParams.Add(new SqlParameter("@PaymentTerm", model.PaymentTerms));
            SqlParams.Add(new SqlParameter("@Transporter", model.Transport));
            SqlParams.Add(new SqlParameter("@Vehicleno", model.VehicleNo));
            SqlParams.Add(new SqlParameter("@CurrencyId", Convert.ToInt32(model.CurrencyId)));
            SqlParams.Add(new SqlParameter("@Currency", model.Currency));
            SqlParams.Add(new SqlParameter("@ExchangeRate", model.ExchangeRate));
            SqlParams.Add(new SqlParameter("@ConversionFactor", model.ExchangeRate));
            SqlParams.Add(new SqlParameter("@BillAmt", (float)(model.ItemNetAmount)));
            SqlParams.Add(new SqlParameter("@RoundOffAmt", (float)Math.Round(model.TotalRoundOffAmt, 2)));
            SqlParams.Add(new SqlParameter("@RoundoffType", model.TotalRoundOff));
            SqlParams.Add(new SqlParameter("@GSTAmount", 0));
            SqlParams.Add(new SqlParameter("@Taxableamt", (float)Math.Round(model.TxAmount, 2)));
            SqlParams.Add(new SqlParameter("@ToatlDiscountPercent", (float)Math.Round(model.TotalDiscountPercentage, 2)));
            SqlParams.Add(new SqlParameter("@TotalDiscountAmount", (float)Math.Round(model.TotalAmtAftrDiscount, 2)));
            SqlParams.Add(new SqlParameter("@NetAmt", (float)model.NetTotal));
            SqlParams.Add(new SqlParameter("@Remark", model.Remark));
            SqlParams.Add(new SqlParameter("@CC", model.Branch));
            SqlParams.Add(new SqlParameter("@UID", model.CreatedBy));
            SqlParams.Add(new SqlParameter("@TaxVariationPOvsBill", string.Empty));
            SqlParams.Add(new SqlParameter("@PONetAmt", (float)Math.Round(model.NetTotal, 2)));
            SqlParams.Add(new SqlParameter("@BOEDate", EntryDt == default ? string.Empty : EntryDt));
            SqlParams.Add(new SqlParameter("@ModeOfTrans", model.ModeOfTransport));
            SqlParams.Add(new SqlParameter("@TotalAmtInOtherCurr", (float)Math.Round(model.NetTotal, 2)));
            SqlParams.Add(new SqlParameter("@boeno", string.Empty));
            SqlParams.Add(new SqlParameter("@Commodity", string.Empty));
            SqlParams.Add(new SqlParameter("@PathOfFile1", model.PathOfFile1URL));
            SqlParams.Add(new SqlParameter("@pathoffile2", model.PathOfFile2URL));
            SqlParams.Add(new SqlParameter("@pathoffile3", model.PathOfFile3URL));
            SqlParams.Add(new SqlParameter("@BalanceSheetClosed", 'N'));
            SqlParams.Add(new SqlParameter("@GateRemark", string.Empty));
            SqlParams.Add(new SqlParameter("@MrnRemark", string.Empty));
            SqlParams.Add(new SqlParameter("@ActualEntryBy", model.PreparedBy));
            SqlParams.Add(new SqlParameter("@ActualEntryDate", CurrentDate == default ? string.Empty : CurrentDate));
            SqlParams.Add(new SqlParameter("@LastQcDate", EntryDt == default ? string.Empty : EntryDt));
            SqlParams.Add(new SqlParameter("@PORemarks", string.Empty));
            SqlParams.Add(new SqlParameter("@VendoreAddress", model.VendorAddress));
            SqlParams.Add(new SqlParameter("@paymentDay", model.PaymentDays));

            RoundFloatColumns(ItemDetailDT);
            RoundFloatColumns(TaxDetailDT);

            SqlParams.Add(new SqlParameter("@DTItemGrid", ItemDetailDT));
            SqlParams.Add(new SqlParameter("@DTTaxGrid", TaxDetailDT));
            SqlParams.Add(new SqlParameter("@DTTDSGrid", TDSDetailDT));
            SqlParams.Add(new SqlParameter("@DRCRDATA", DrCrDetailDT));
            SqlParams.Add(new SqlParameter("@AgainstRef", AdjDetailDT));
            if (model.Mode == "UPDATE")
            {
                SqlParams.Add(new SqlParameter("@UpdatedBy", model.UpdatedBy));
                SqlParams.Add(new SqlParameter("@EneterdBy", model.UpdatedBy));
                SqlParams.Add(new SqlParameter("@LastUpdatedDate", CurrentDate == default ? string.Empty : CurrentDate));
            }
            else if (model.Mode == "INSERT")
            {
                SqlParams.Add(new SqlParameter("@CreatedBy", model.CreatedBy));
                SqlParams.Add(new SqlParameter("@CreatedDate", CurrentDate == default ? string.Empty : CurrentDate));
                SqlParams.Add(new SqlParameter("@LastUpdatedDate", CurrentDate == default ? string.Empty : CurrentDate));
            }
            SqlParams.Add(new SqlParameter("@EntryByMachine", model.EntryByMachineName));
            SqlParams.Add(new SqlParameter("@EntryByMachineName", model.EntryByMachineName));

            _ResponseResult = await _IDataLogic.ExecuteDataTable("AccSP_PurchaseBillMainDetail", SqlParams);
        }
        catch (Exception ex)
        {
            dynamic Error = new ExpandoObject();
            Error.Message = ex.Message;
            Error.Source = ex.Source;
        }

        return _ResponseResult;
    }

    private void RoundFloatColumns(DataTable table)
    {
        if (table == null) { return; }
        else if (table.Rows.Count == 0) { return; }
        foreach (DataRow row in table.Rows)
        {
            foreach (DataColumn col in table.Columns)
            {
                if (col.DataType == typeof(float))
                {
                    float value = (float)row[col];
                    row[col] = (float)Math.Round(value, 2);
                }
            }
        }
    }
    internal async Task<PBListDataModel> GetPurchaseBillListData(string? flag, string? MRNType, string? dashboardtype, DateTime? firstdate, DateTime? todate, PBListDataModel model)
    {
        var PBListData = new PBListDataModel();
        var SqlParams = new List<dynamic>();
        DataSet? oDataSet = new DataSet();

        try
        {
            using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
            {
                DateTime now = DateTime.Now;
                DateTime firstDayOfMonth = new DateTime(now.Year, now.Month, 1);
                DateTime today = DateTime.Now;
                SqlCommand oCmd = new SqlCommand("GetPendingMRNListForPurchaseBill", myConnection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                flag = !string.IsNullOrEmpty(flag) ? flag : "DisplayPendingData";
                MRNType = !string.IsNullOrEmpty(MRNType) ? MRNType : "MRN";
                dashboardtype = !string.IsNullOrEmpty(dashboardtype) ? dashboardtype : "SUMMARY";
                firstdate = (firstdate != null) ? firstdate : firstDayOfMonth;
                todate = (todate != null) ? todate : today;
                string fromDate = firstdate.HasValue ? CommonFunc.ParseFormattedDate(firstdate.Value.Date.ToString()) : CommonFunc.ParseFormattedDate(firstDayOfMonth.Date.ToString());
                string toDate = todate.HasValue ? CommonFunc.ParseFormattedDate(todate.Value.Date.ToString()) : CommonFunc.ParseFormattedDate(today.Date.ToString());
                oCmd.Parameters.AddWithValue("@flag", flag);
                oCmd.Parameters.AddWithValue("@MRNTYpe", MRNType);
                //oCmd.Parameters.AddWithValue("@Fromdate", firstdate);
                //oCmd.Parameters.AddWithValue("@ToDate", todate);
                oCmd.Parameters.AddWithValue("@Fromdate", fromDate);
                oCmd.Parameters.AddWithValue("@Todate", toDate);
                oCmd.Parameters.AddWithValue("@SummaryDetail", dashboardtype.ToUpper());
                oCmd.Parameters.AddWithValue("@VendorName", !string.IsNullOrEmpty(model.PartyName) && model.PartyName != "0" ? model.PartyName : string.Empty);
                oCmd.Parameters.AddWithValue("@Mrnno", !string.IsNullOrEmpty(model.MRNNo) && model.MRNNo != "0" ? model.MRNNo : string.Empty);
                oCmd.Parameters.AddWithValue("@Pono", !string.IsNullOrEmpty(model.PONO) && model.PONO != "0" ? model.PONO : string.Empty);
                oCmd.Parameters.AddWithValue("@InvNo", !string.IsNullOrEmpty(model.InvoiceNo) && model.InvoiceNo != "0" ? model.InvoiceNo : string.Empty);
                oCmd.Parameters.AddWithValue("@gateNo", !string.IsNullOrEmpty(model.GateNo) && model.GateNo != "0" ? model.GateNo : string.Empty);
                oCmd.Parameters.AddWithValue("@Itemname", !string.IsNullOrEmpty(model.ItemName) && model.ItemName != "0" ? model.ItemName : string.Empty);
                oCmd.Parameters.AddWithValue("@docname", !string.IsNullOrEmpty(model.DocumentName) && model.DocumentName != "0" ? model.DocumentName : string.Empty);
                oCmd.Parameters.AddWithValue("@partcode", !string.IsNullOrEmpty(model.PartCode) && model.PartCode != "0" ? model.PartCode : string.Empty);
                await myConnection.OpenAsync();
                using (SqlDataAdapter oDataAdapter = new SqlDataAdapter(oCmd))
                {
                    oDataAdapter.Fill(oDataSet);
                }
            }

            if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
            {
                PBListData.PBListData = new List<PBListDataModel>();
                int seq = 1; // Start sequence number

                // Loop through each DataRow
                foreach (DataRow dr in oDataSet.Tables[0].Rows)
                {
                    // Create and populate a new PBListDataModel object for each row
                    PBListData.PBListData.Add(new PBListDataModel
                    {
                        PBListDataSeqNo = seq++,
                        MRNNo = dr["MRNNo"].ToString(),
                        MRNYearCode = string.IsNullOrEmpty(dr["MRNYearCode"].ToString()) ? 0 : Convert.ToInt32(dr["MRNYearCode"]),
                        MRNEntryDate = string.IsNullOrEmpty(dr["MRNEntryDate"].ToString()) ? DateTime.MinValue : Convert.ToDateTime(dr["MRNEntryDate"]),
                        PartyName = dr["VendorName"].ToString(),
                        InvoiceNo = dr["InvoiceNo"].ToString(),
                        InvoiceDate = string.IsNullOrEmpty(dr["InvDate"].ToString()) ? DateTime.MinValue : Convert.ToDateTime(dr["InvDate"]),
                        GateNo = dr["GateNo"].ToString(),
                        GateDate = string.IsNullOrEmpty(dr["GateDate"].ToString()) ? DateTime.MinValue : Convert.ToDateTime(dr["GateDate"]),
                        DocumentName = string.Empty, // Default as empty
                        CheckQc = dr["CheckQc"].ToString(),
                        QCCompleted = dr["QCCompleted"].ToString(),
                        TotalMRNItemCount = string.IsNullOrEmpty(dr["TotalMRNItemCount"].ToString()) ? 0 : Convert.ToInt32(dr["TotalMRNItemCount"]),
                        QCtotalQty = string.IsNullOrEmpty(dr["QCtotalQty"].ToString()) ? 0 : Convert.ToInt32(dr["QCtotalQty"]),
                        ItemQCCompledCount = string.IsNullOrEmpty(dr["ItemQCCompledCount"].ToString()) ? 0 : Convert.ToInt32(dr["ItemQCCompledCount"]),
                        AccountCode = string.IsNullOrEmpty(dr["AccountCode"].ToString()) ? 0 : Convert.ToInt32(dr["AccountCode"])
                    });
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            dynamic Error = new ExpandoObject();
            Error.Message = ex.Message;
            Error.Source = ex.Source;
        }

        return PBListData;
    }

    internal async Task<PurchaseBillModel> GetPurchaseBillItemData(string? flag, string? FlagMRNJWCHALLAN, string? Mrnno, int? mrnyearcode, int? accountcode)
    {
        var PBItemData = new PurchaseBillModel();
        var SqlParams = new List<dynamic>();
        DataSet? oDataSet = new DataSet();

        try
        {
            using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
            {
                SqlCommand oCmd = new SqlCommand("AccSP_PurchaseBillMainDetail", myConnection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                flag = !string.IsNullOrEmpty(flag) ? flag : "DisplayPendmrn";
                FlagMRNJWCHALLAN = !string.IsNullOrEmpty(FlagMRNJWCHALLAN) ? FlagMRNJWCHALLAN : "MRN";
                Mrnno = !string.IsNullOrEmpty(Mrnno) ? Mrnno : string.Empty;
                mrnyearcode = mrnyearcode != null && mrnyearcode > 0 ? mrnyearcode : DateTime.Now.Year;
                accountcode = accountcode != null && accountcode > 0 ? accountcode : 0;
                oCmd.Parameters.AddWithValue("@flag", flag);
                oCmd.Parameters.AddWithValue("@FlagMRNJWCHALLAN", FlagMRNJWCHALLAN);
                oCmd.Parameters.AddWithValue("@Mrnno", Mrnno);
                oCmd.Parameters.AddWithValue("@mrnyearcode", mrnyearcode);
                oCmd.Parameters.AddWithValue("@accountcode", accountcode);
                await myConnection.OpenAsync();
                using (SqlDataAdapter oDataAdapter = new SqlDataAdapter(oCmd))
                {
                    oDataAdapter.Fill(oDataSet);
                }
            }

            if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
            {
                var seq = 0;
                PurchaseBillModel pbItemData = null;

                foreach (DataRow dr in oDataSet.Tables[0].Rows)
                {
                    pbItemData = new PurchaseBillModel
                    {
                        SeqNo = seq++,
                        MRNNo = dr["MRNNo"].ToString(),
                        MRNYearCode = string.IsNullOrEmpty(dr["MRNYearCode"].ToString()) ? 0 : Convert.ToInt32(dr["MRNYearCode"]),
                        MRNEntryDate = string.IsNullOrEmpty(dr["MRNEntryDate"].ToString()) ? new DateTime() : Convert.ToDateTime(dr["MRNEntryDate"]),
                        StrMRNEntryDate = string.IsNullOrEmpty(dr["MRNEntryDate"].ToString()) ? string.Empty : dr["MRNEntryDate"].ToString(),
                        GateNo = dr["GateNo"].ToString(),
                        GateYearCode = string.IsNullOrEmpty(dr["GateYearCode"].ToString()) ? 0 : Convert.ToInt32(dr["GateYearCode"]),
                        GateDate = string.IsNullOrEmpty(dr["GateDate"].ToString()) ? new DateTime() : Convert.ToDateTime(dr["GateDate"]),
                        StrGateDate = string.IsNullOrEmpty(dr["GateDate"].ToString()) ? string.Empty : dr["GateDate"].ToString(),
                        InvNo = dr["InvNo"].ToString(),
                        InvoiceDate = string.IsNullOrEmpty(dr["InvDate"].ToString()) ? new DateTime() : Convert.ToDateTime(dr["InvDate"]),
                        InvDate = string.IsNullOrEmpty(dr["InvDate"].ToString()) ? string.Empty : dr["InvDate"].ToString(),
                        AccountCode = !string.IsNullOrEmpty(dr["AccountCode"].ToString()) ? Convert.ToInt32(dr["AccountCode"].ToString()) : 0,
                        VendorName = dr["VendorName"].ToString(),
                        StateId = !string.IsNullOrEmpty(dr["State"].ToString()) ? Convert.ToInt32(dr["State"]) : 0,
                        StateName = dr["StateName"].ToString(),
                        GSTNO = dr["GSTNO"].ToString(),
                        GSTType = dr["GSTTYPE"].ToString(),
                        GSTRegistered = dr["GSTRegistered"].ToString(),
                        VendorAddress = dr["VendorAddress"].ToString(),
                        PurchaseBillDirectPB = dr["PurchaseBillDirectPB"].ToString(),
                        TypeITEMSERVASSETS = dr["TypeITEMSERVASSETS"].ToString(),
                        PurchaseBillTypeMRNJWChallan = dr["PurchaseBillTypeMRNJWChallan"].ToString(),
                        PaymentTerms = dr["PaymentTerm"].ToString(),
                        PaymentDays = !string.IsNullOrEmpty(dr["PaymentDays"].ToString()) ? Convert.ToInt32(dr["PaymentDays"]) : 0,
                        ModeOfTransport = dr["ModeOfTransport"].ToString(),
                        FOC = dr["FOC"].ToString(),
                        Currency = dr["Currency"].ToString(),
                        CurrencyId = !string.IsNullOrEmpty(dr["CurrencyId"].ToString()) ? Convert.ToInt32(dr["CurrencyId"]) : 0,
                        CC = dr["CC"].ToString(),
                        UID = string.IsNullOrEmpty(dr["UID"].ToString()) ? 0 : Convert.ToInt32(dr["UID"].ToString()),
                        ActualEntryBy = string.IsNullOrEmpty(dr["ActualEntryBy"].ToString()) ? 0 : Convert.ToInt32(dr["ActualEntryBy"].ToString()),
                        MRNRemark = dr["MRNRemark"].ToString(),
                        CreatedOn = string.IsNullOrEmpty(dr["ActualEntryDate"].ToString()) ? new DateTime() : Convert.ToDateTime(dr["ActualEntryDate"]),
                        CreatedBy = string.IsNullOrEmpty(dr["ActualEntryBy"].ToString()) ? 0 : Convert.ToInt32(dr["ActualEntryBy"].ToString()),
                    };
                    break;
                }

                PBItemData = pbItemData;
            }

            if (oDataSet.Tables.Count > 1 && oDataSet.Tables[1].Rows.Count > 0)
            {
                var seq = 1;
                var itemDetailGrid = new List<PBItemDetail>();

                foreach (DataRow dr in oDataSet.Tables[1].Rows)
                {
                    var itemDetail = new PBItemDetail
                    {
                        SeqNo = seq++,
                        ParentCode = dr["ParentCode"].ToString(),
                        DocumentName = dr["DocumentName"].ToString(),
                        DocTypeID = string.IsNullOrEmpty(dr["DocTypeID"].ToString()) ? 0 : Convert.ToInt32(dr["DocTypeID"]),
                        ItemCode = string.IsNullOrEmpty(dr["ItemCode"].ToString()) ? 0 : Convert.ToInt32(dr["ItemCode"]),
                        Item_Name = dr["Item_Name"].ToString(),
                        PartCode = dr["PartCode"].ToString(),
                        HSNNO = string.IsNullOrEmpty(dr["HSNNO"].ToString()) ? 0 : Convert.ToInt32(dr["HSNNO"]),
                        BillQty = !string.IsNullOrEmpty(dr["BillQty"].ToString()) ? Convert.ToDecimal(dr["BillQty"]) : 0,
                        RecQty = !string.IsNullOrEmpty(dr["RecQty"].ToString()) ? Convert.ToDecimal(dr["RecQty"]) : 0,
                        AcceptedQty = !string.IsNullOrEmpty(dr["AcceptedQty"].ToString()) ? Convert.ToDecimal(dr["AcceptedQty"]) : 0,
                        rejectedQty = !string.IsNullOrEmpty(dr["rejectedQty"].ToString()) ? Convert.ToDecimal(dr["rejectedQty"]) : 0,
                        ReworkQty = !string.IsNullOrEmpty(dr["ReworkQty"].ToString()) ? Convert.ToDecimal(dr["ReworkQty"]) : 0,
                        HoldQty = !string.IsNullOrEmpty(dr["HoldQty"].ToString()) ? Convert.ToDecimal(dr["HoldQty"]) : 0,
                        Unit = dr["unit"].ToString(),
                        NoOfCase = string.IsNullOrEmpty(dr["NoOfCase"].ToString()) ? 0 : Convert.ToInt32(dr["NoOfCase"].ToString()),
                        AltRecQty = !string.IsNullOrEmpty(dr["AltRecQty"].ToString()) ? Convert.ToDecimal(dr["AltRecQty"]) : 0,
                        AltUnit = dr["AltUnit"].ToString(),
                        BillRate = !string.IsNullOrEmpty(dr["BillRate"].ToString()) ? Convert.ToDecimal(dr["BillRate"]) : 0,
                        MRP = !string.IsNullOrEmpty(dr["MRP"].ToString()) ? Convert.ToDecimal(dr["MRP"]) : 0,
                        //Amount = !string.IsNullOrEmpty(dr["Amount"].ToString()) ? Convert.ToDecimal(dr["Amount"]) : 0,
                        RateIncludingTax = !string.IsNullOrEmpty(dr["RateIncludingTax"].ToString()) ? Convert.ToDecimal(dr["RateIncludingTax"]) : 0,
                        AmtInOtherCurrency = !string.IsNullOrEmpty(dr["AmtInOtherCurrency"].ToString()) ? Convert.ToInt32(dr["AmtInOtherCurrency"]) : 0,
                        RateOfConvFactor = !string.IsNullOrEmpty(dr["RateOfConvFactor"].ToString()) ? Convert.ToInt32(dr["RateOfConvFactor"]) : 0,
                        AssessRate = !string.IsNullOrEmpty(dr["AssessRate"].ToString()) ? Convert.ToDecimal(dr["AssessRate"]) : 0,
                        DisPer = !string.IsNullOrEmpty(dr["DisPer"].ToString()) ? Convert.ToInt32(dr["DisPer"]) : 0,
                        DisAmt = !string.IsNullOrEmpty(dr["DisAmt"].ToString()) ? Convert.ToInt32(dr["DisAmt"]) : 0,
                        ItemSize = dr["ItemSize"].ToString(),
                        ItemColor = dr["ItemColor"].ToString(),
                        ItemModel = dr["ItemModel"].ToString(),
                        DepartmentId = !string.IsNullOrEmpty(dr["DepartmentId"].ToString()) ? Convert.ToInt32(dr["DepartmentId"]) : 0,
                        ForDepartment = dr["ForDepartment"].ToString(),
                        ProcessId = !string.IsNullOrEmpty(dr["ProcessId"].ToString()) ? Convert.ToInt32(dr["ProcessId"]) : 0,
                        ProcessName = dr["ProcessName"].ToString(),
                        NewPoRate = !string.IsNullOrEmpty(dr["NewPoRate"].ToString()) ? Convert.ToDecimal(dr["NewPoRate"]) : 0,
                        pono = dr["pono"].ToString(),
                        poyearcode = string.IsNullOrEmpty(dr["poyearcode"].ToString()) ? 0 : Convert.ToInt32(dr["poyearcode"].ToString()),
                        PODate = dr["PODate"].ToString(),
                        schno = dr["schno"].ToString(),
                        schyearcode = string.IsNullOrEmpty(dr["schyearcode"].ToString()) ? 0 : Convert.ToInt32(dr["schyearcode"].ToString()),
                        SchDate = dr["SchDate"].ToString(),
                        PoAmendNo = string.IsNullOrEmpty(dr["PoAmendNo"].ToString()) ? 0 : Convert.ToInt32(dr["PoAmendNo"].ToString()),
                        PORate = string.IsNullOrEmpty(dr["PORate"].ToString()) ? 0 : Convert.ToDecimal(dr["PORate"].ToString()),
                        MIRNO = dr["MIRNO"].ToString(),
                        MIRYEARCODE = string.IsNullOrEmpty(dr["MIRYEARCODE"].ToString()) ? 0 : Convert.ToInt32(dr["MIRYEARCODE"].ToString()),
                        MIRDATE = dr["MIRDATE"].ToString(),
                        ProjectNo = dr["ProjectNo"].ToString(),
                        ProjectDate = dr["ProjectDate"].ToString(),
                        ProjectyearCode = string.IsNullOrEmpty(dr["ProjectyearCode"].ToString()) ? 0 : Convert.ToInt32(dr["ProjectyearCode"].ToString()),
                        AgainstImportAccountCode = string.IsNullOrEmpty(dr["AgainstImportAccountCode"].ToString()) ? 0 : Convert.ToInt32(dr["AgainstImportAccountCode"].ToString()),
                        AgainstImportInvoiceNo = string.IsNullOrEmpty(dr["AgainstImportInvoiceNo"].ToString()) ? 0 : Convert.ToInt32(dr["AgainstImportInvoiceNo"].ToString()),
                        AgainstImportYearCode = string.IsNullOrEmpty(dr["AgainstImportYearCode"].ToString()) ? 0 : Convert.ToInt32(dr["AgainstImportYearCode"].ToString()),
                        AgainstImportInvDate = dr["AgainstImportInvDate"].ToString(),
                    };

                    itemDetailGrid.Add(itemDetail);
                }

                PBItemData.ItemDetailGrid = itemDetailGrid;
            }
            if (oDataSet.Tables.Count > 2 && oDataSet.Tables[2].Rows.Count > 0)
            {
                var seq = 1;
                var taxDetails = new List<TaxModel>();

                foreach (DataRow dr in oDataSet.Tables[2].Rows)
                {
                    var taxDetail = new TaxModel
                    {
                        TxSeqNo = seq++,
                        TxType = dr["TaxExpType"].ToString(),
                        TxPartCode = !string.IsNullOrEmpty(dr["ItemCode"].ToString()) ? Convert.ToInt32(dr["ItemCode"]) : 0,
                        TxPartName = dr["PartCode"].ToString(),
                        TxItemCode = !string.IsNullOrEmpty(dr["ItemCode"].ToString()) ? Convert.ToInt32(dr["ItemCode"]) : 0,
                        TxItemName = dr["ItemName"].ToString(),
                        TxOnExp = !string.IsNullOrEmpty(dr["TaxOnExp"].ToString()) ? Convert.ToDecimal(dr["TaxOnExp"]) : 0,
                        TxPercentg = !string.IsNullOrEmpty(dr["TaxPer"].ToString()) ? Convert.ToDecimal(dr["TaxPer"]) : 0,
                        TxAdInTxable = dr["AddInTaxable"].ToString(),
                        TxRoundOff = dr["RoundOffYesNo"].ToString(),
                        TxTaxType = !string.IsNullOrEmpty(dr["TaxTypeID"].ToString()) ? Convert.ToInt32(dr["TaxTypeID"]) : 0,
                        TxTaxTypeName = dr["TaxExpType"].ToString(),
                        TxAccountCode = !string.IsNullOrEmpty(dr["TaxAccountCode"].ToString()) ? Convert.ToInt32(dr["TaxAccountCode"]) : 0,
                        TxAccountName = dr["TaxName"].ToString(),
                        TxAmount = !string.IsNullOrEmpty(dr["TaxAmount"].ToString()) ? Convert.ToDecimal(dr["TaxAmount"]) : 0,
                        TxRefundable = dr["TaxRefundable"].ToString(),
                        TxRemark = dr["POTaxRemark"].ToString(),
                    };

                    taxDetails.Add(taxDetail);
                }

                PBItemData.TaxDetailGridd = taxDetails;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            dynamic Error = new ExpandoObject();
            Error.Message = ex.Message;
            Error.Source = ex.Source;
        }

        return PBItemData;
    }
}