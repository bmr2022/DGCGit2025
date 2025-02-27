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

public class DirectPurchaseBillDAL
{
    private readonly IDataLogic _IDataLogic;
    private readonly string DBConnectionString = string.Empty;

    public DirectPurchaseBillDAL(IConfiguration configuration, IDataLogic iDataLogic)
    {
        _IDataLogic = iDataLogic;
        DBConnectionString = configuration.GetConnectionString("eTactDB");
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


            _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_DirectPurchaseBillMainDetail", SqlParams);
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
    public async Task<ResponseResult> FillEntryandVouchNoNumber(int YearCode, string VODate)
    {
        var _ResponseResult = new ResponseResult();
        try
        {
            DateTime vodt = DateTime.ParseExact(VODate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            var SqlParams = new List<dynamic>();
            SqlParams.Add(new SqlParameter("@Flag", "NewEntryId"));
            SqlParams.Add(new SqlParameter("@YearCode", YearCode));
            SqlParams.Add(new SqlParameter("@EntryDate", vodt));

            _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_DirectPurchaseBillMainDetail", SqlParams);
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

            _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_DirectPurchaseBillMainDetail", SqlParams);
        }
        catch (Exception ex)
        {
            dynamic Error = new ExpandoObject();
            Error.Message = ex.Message;
            Error.Source = ex.Source;
        }
        return _ResponseResult;
    }
    public async Task<ResponseResult> FILLDocumentList(string ShowAllDocument)
    {
        var _ResponseResult = new ResponseResult();
        try
        {
            var SqlParams = new List<dynamic>();
            SqlParams.Add(new SqlParameter("@Flag", "FILLDocumentList"));
            SqlParams.Add(new SqlParameter("@ShowAllDocument", ShowAllDocument));

            _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_DirectPurchaseBillMainDetail", SqlParams);
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

            _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_DirectPurchaseBillMainDetail", SqlParams);
        }
        catch (Exception ex)
        {
            dynamic Error = new ExpandoObject();
            Error.Message = ex.Message;
            Error.Source = ex.Source;
        }
        return _ResponseResult;
    }
    public async Task<ResponseResult> FillCurrency(string Ctrl)
    {
        var _ResponseResult = new ResponseResult();
        try
        {
            var SqlParams = new List<dynamic>();
            SqlParams.Add(new SqlParameter("@Flag", "FILLCURRENCY"));
            SqlParams.Add(new SqlParameter("@Ctrl", Ctrl));

            _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_GetDropDownList", SqlParams);
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

            _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_DirectPurchaseBillMainDetail", SqlParams);
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

            _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_DirectPurchaseBillMainDetail", SqlParams);
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

            _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_DirectPurchaseBillMainDetail", SqlParams);
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

            _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_DirectPurchaseBillMainDetail", SqlParams);
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

            _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_DirectPurchaseBillMainDetail", SqlParams);
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

            _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_DirectPurchaseBillMainDetail", SqlParams);
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

            _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_DirectPurchaseBillMainDetail", SqlParams);
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

            _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_DirectPurchaseBillMainDetail", SqlParams);
        }
        catch (Exception ex)
        {
            dynamic Error = new ExpandoObject();
            Error.Message = ex.Message;
            Error.Source = ex.Source;
        }

        return _ResponseResult;
    }

    internal async Task<DPBDashBoard> GetDashBoardData()
    {
        var DashBoardData = new DPBDashBoard();
        var SqlParams = new List<dynamic>();
        DataSet? oDataSet = new DataSet();

        try
        {
            using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
            {
                DateTime now = DateTime.Now;
                DateTime firstDayOfMonth = new DateTime(now.Year, now.Month, 1);
                DateTime today = DateTime.Now;
                SqlCommand oCmd = new SqlCommand("SP_DirectPurchaseBillMainDetail", myConnection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                oCmd.Parameters.AddWithValue("@Flag", "DASHBOARD");
                oCmd.Parameters.AddWithValue("@YearCode", now.Year);
                oCmd.Parameters.AddWithValue("@SummDetail", "summary");
                oCmd.Parameters.AddWithValue("@PurchVoucherNo", string.Empty);
                oCmd.Parameters.AddWithValue("@InvNo", string.Empty);
                oCmd.Parameters.AddWithValue("@partcode", string.Empty);
                oCmd.Parameters.AddWithValue("@Vendorname", string.Empty);
                oCmd.Parameters.AddWithValue("@ItemName", string.Empty);
                oCmd.Parameters.AddWithValue("@GSTType", string.Empty);
                oCmd.Parameters.AddWithValue("@DocumentType", string.Empty);
                oCmd.Parameters.AddWithValue("@HSNNo", string.Empty);
                oCmd.Parameters.AddWithValue("@TypeITEMSERVASSETS", string.Empty);
                oCmd.Parameters.AddWithValue("@FromDate", firstDayOfMonth);
                oCmd.Parameters.AddWithValue("@ToDate",today);
                await myConnection.OpenAsync();
                using (SqlDataAdapter oDataAdapter = new SqlDataAdapter(oCmd))
                {
                    oDataAdapter.Fill(oDataSet);
                }
            }

            if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
            {
                DashBoardData.DPBDashboard = (from DataRow dr in oDataSet.Tables[0].Rows
                                              select new DPBDashBoard
                                              {
                                                  EntryID = !string.IsNullOrEmpty(dr["PurchBillEntryId"].ToString()) ? Convert.ToInt32(dr["PurchBillEntryId"]) : 0,
                                                  PurchVouchNo = dr["purvoucherno"].ToString(),
                                                  EntryDate = string.IsNullOrEmpty(dr["EntryDate"].ToString()) ? new DateTime() : Convert.ToDateTime(dr["EntryDate"]),
                                                  InvoiceNo = dr["InvoiceNo"].ToString(),
                                                  InvoiceDate = string.IsNullOrEmpty(dr["InvoiceDate"].ToString()) ? new DateTime() : Convert.ToDateTime(dr["InvoiceDate"]),
                                                  YearCode = !string.IsNullOrEmpty(dr["PurchBillYearCode"].ToString()) ? Convert.ToInt32(dr["PurchBillYearCode"]) : 0,
                                                  VendorName = dr["VendorName"].ToString(),
                                                  VendorAddress = dr["VendorAddress"].ToString(),
                                                  StateName = dr["StateName"].ToString(),
                                                  GSTType = dr["GSTType"].ToString(),
                                                  Currency = dr["Currency"].ToString(),
                                                  TypeITEMSERVASSETS = dr["TypeITEMSERVASSETS"].ToString(),
                                                  CC = dr["Branch"].ToString(),
                                                  DomesticImport = dr["DomesticImport"].ToString(),
                                                  PaymentTerms = dr["PaymentTerm"].ToString(),
                                                  Transporter = dr["Transporter"].ToString(),
                                                  VehicleNo = dr["Vehicleno"].ToString(),
                                                  ModeOfTrans = dr["ModeOfTrans"].ToString(),
                                                  BalanceSheetClosed = dr["BalanceSheetClosed"].ToString(),
                                                  VoucherDate = dr["VoucherDate"].ToString(),
                                                  Approved = dr["Approved"].ToString(),
                                                  UpdatedByName = dr["EntryByMachine"].ToString(),
                                                  EnteredBy = dr["EntryByMachine"].ToString(),
                                                  UpdatedBy = !string.IsNullOrEmpty(dr["UpdatedBy"].ToString()) ? 0 : Convert.ToInt32(dr["UpdatedBy"].ToString()),
                                                  UpdatedOn = string.IsNullOrEmpty(dr["LastUpdatedDate"].ToString()) ? new DateTime() : Convert.ToDateTime(dr["LastUpdatedDate"]),
                                                  CreatedBy = !string.IsNullOrEmpty(dr["ActualEntryBy"].ToString()) ? 0 : Convert.ToInt32(dr["ActualEntryBy"].ToString()),
                                                  CreatedOn = string.IsNullOrEmpty(dr["ActualEntryDate"].ToString()) ? new DateTime() : Convert.ToDateTime(dr["ActualEntryDate"]),
                                                  BasicAmount = !string.IsNullOrEmpty(dr["BillAmt"].ToString()) ? Convert.ToSingle(dr["BillAmt"]) : 0,
                                                  NetAmount = !string.IsNullOrEmpty(dr["NetAmt"].ToString()) ? Convert.ToSingle(dr["NetAmt"]) : 0,
                                                  AgainstInvNo = dr["AgainstInvNo"].ToString(),
                                                  AgainstVoucherNo = dr["AgainstVoucherNo"].ToString(),
                                                  AgainstVoucherDate = string.IsNullOrEmpty(dr["AgainstVoucherDate"].ToString()) ? new DateTime() : Convert.ToDateTime(dr["AgainstVoucherDate"]),
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
    public async Task<DPBDashBoard> GetSummaryData(DPBDashBoard model)
    {
        var DashBoardData = new DPBDashBoard();
        DataSet? oDataSet = new DataSet();
        try
        {
            using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
            {
                DateTime now = DateTime.Now;
                DateTime firstDayOfMonth = new DateTime(now.Year, now.Month, 1);
                SqlCommand oCmd = new SqlCommand("SP_DirectPurchaseBillMainDetail", myConnection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                oCmd.Parameters.AddWithValue("@Flag", "DASHBOARD");
                oCmd.Parameters.AddWithValue("@YearCode", now.Year);
                oCmd.Parameters.AddWithValue("@SummDetail", model.DashboardType.ToLower());
                oCmd.Parameters.AddWithValue("@PurchVoucherNo", model.PurchVouchNo);
                oCmd.Parameters.AddWithValue("@InvNo", model.InvoiceNo);
                oCmd.Parameters.AddWithValue("@partcode", model.PartCode);
                oCmd.Parameters.AddWithValue("@Vendorname", model.VendorName);
                oCmd.Parameters.AddWithValue("@ItemName", model.ItemName);
                oCmd.Parameters.AddWithValue("@GSTType", model.GSTType);
                oCmd.Parameters.AddWithValue("@TypeITEMSERVASSETS", model.TypeITEMSERVASSETS);
                oCmd.Parameters.AddWithValue("@DocumentType", model.DocumentType);
                oCmd.Parameters.AddWithValue("@HSNNo", model.HsnNo);
                oCmd.Parameters.AddWithValue("@FromDate", Convert.ToDateTime(model.FromDate).ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture));
                oCmd.Parameters.AddWithValue("@ToDate", Convert.ToDateTime(model.ToDate).ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture));
                await myConnection.OpenAsync();
                using (SqlDataAdapter oDataAdapter = new SqlDataAdapter(oCmd))
                {
                    oDataAdapter.Fill(oDataSet);
                }
            }

            if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
            {
                DashBoardData.DPBDashboard = (from DataRow dr in oDataSet.Tables[0].Rows
                                              select new DPBDashBoard
                                              {
                                                  EntryID = !string.IsNullOrEmpty(dr["PurchBillEntryId"].ToString()) ? Convert.ToInt32(dr["PurchBillEntryId"]) : 0,
                                                  PurchVouchNo = dr["purvoucherno"].ToString(),
                                                  EntryDate = string.IsNullOrEmpty(dr["EntryDate"].ToString()) ? new DateTime() : Convert.ToDateTime(dr["EntryDate"]),
                                                  InvoiceNo = dr["InvoiceNo"].ToString(),
                                                  InvoiceDate = string.IsNullOrEmpty(dr["InvoiceDate"].ToString()) ? new DateTime() : Convert.ToDateTime(dr["InvoiceDate"]),
                                                  YearCode = !string.IsNullOrEmpty(dr["PurchBillYearCode"].ToString()) ? Convert.ToInt32(dr["PurchBillYearCode"]) : 0,
                                                  VendorName = dr["VendorName"].ToString(),
                                                  VendorAddress = dr["VendorAddress"].ToString(),
                                                  StateName = dr["StateName"].ToString(),
                                                  GSTType = dr["GSTType"].ToString(),
                                                  Currency = dr["Currency"].ToString(),
                                                  TypeITEMSERVASSETS = dr["TypeITEMSERVASSETS"].ToString(),
                                                  CC = dr["Branch"].ToString(),
                                                  DomesticImport = dr["DomesticImport"].ToString(),
                                                  PaymentTerms = dr["PaymentTerm"].ToString(),
                                                  Transporter = dr["Transporter"].ToString(),
                                                  VehicleNo = dr["Vehicleno"].ToString(),
                                                  ModeOfTrans = dr["ModeOfTrans"].ToString(),
                                                  BalanceSheetClosed = dr["BalanceSheetClosed"].ToString(),
                                                  VoucherDate = dr["VoucherDate"].ToString(),
                                                  Approved = dr["Approved"].ToString(),
                                                  UpdatedByName = dr["EntryByMachine"].ToString(),
                                                  UpdatedBy = !string.IsNullOrEmpty(dr["UpdatedBy"].ToString()) ? 0 : Convert.ToInt32(dr["UpdatedBy"].ToString()),
                                                  EnteredBy = dr["EntryByMachine"].ToString(),
                                                  UpdatedOn = string.IsNullOrEmpty(dr["LastUpdatedDate"].ToString()) ? new DateTime() : Convert.ToDateTime(dr["LastUpdatedDate"]),
                                                  CreatedBy = !string.IsNullOrEmpty(dr["ActualEntryBy"].ToString()) ? 0 : Convert.ToInt32(dr["ActualEntryBy"].ToString()),
                                                  CreatedOn = string.IsNullOrEmpty(dr["ActualEntryDate"].ToString()) ? new DateTime() : Convert.ToDateTime(dr["ActualEntryDate"]),
                                                  BasicAmount = !string.IsNullOrEmpty(dr["BillAmt"].ToString()) ? Convert.ToSingle(dr["BillAmt"]) : 0,
                                                  NetAmount = !string.IsNullOrEmpty(dr["NetAmt"].ToString()) ? Convert.ToSingle(dr["NetAmt"]) : 0,
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

    public async Task<DPBDashBoard> GetDetailData(DPBDashBoard model)
    {
        var DashBoardData = new DPBDashBoard();
        DataSet? oDataSet = new DataSet();
        var model1 = new DPBDashBoard();
        try
        {
            using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
            {
                DateTime now = DateTime.Now;
                DateTime firstDayOfMonth = new DateTime(now.Year, now.Month, 1);
                SqlCommand oCmd = new SqlCommand("SP_DirectPurchaseBillMainDetail", myConnection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                oCmd.Parameters.AddWithValue("@Flag", "DASHBOARD");
                oCmd.Parameters.AddWithValue("@YearCode", now.Year);
                oCmd.Parameters.AddWithValue("@SummDetail", model.DashboardType);
                oCmd.Parameters.AddWithValue("@PurchVoucherNo", model.PurchVouchNo);
                oCmd.Parameters.AddWithValue("@InvNo", model.InvoiceNo);
                oCmd.Parameters.AddWithValue("@partcode", model.PartCode);
                oCmd.Parameters.AddWithValue("@VendorName", model.VendorName);
                oCmd.Parameters.AddWithValue("@TypeITEMSERVASSETS", model.TypeITEMSERVASSETS);
                oCmd.Parameters.AddWithValue("@DocumentType", model.DocumentType);
                oCmd.Parameters.AddWithValue("@HSNNo", model.HsnNo);
                oCmd.Parameters.AddWithValue("@GSTType", model.GSTType);
                oCmd.Parameters.AddWithValue("@FromDate", Convert.ToDateTime(model.FromDate).ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture));
                oCmd.Parameters.AddWithValue("@ToDate", Convert.ToDateTime(model.ToDate).ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture));
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
                    DashBoardData.DPBDashboard = (from DataRow dr in oDataSet.Tables[0].Rows
                                                  select new DPBDashBoard
                                                  {
                                                      //EntryID = !string.IsNullOrEmpty(dr["PurchBillEntryId"].ToString()) ? Convert.ToInt32(dr["PurchBillEntryId"]) : 0,
                                                      PurchVouchNo = dr["PurchvoucherNo"].ToString(),
                                                      InvoiceNo = dr["InvoiceNo"].ToString(),
                                                      InvoiceDate = string.IsNullOrEmpty(dr["InvoiceDate"].ToString()) ? new DateTime() : Convert.ToDateTime(dr["InvoiceDate"]),
                                                      //YearCode = !string.IsNullOrEmpty(dr["PurchBillYearCode"].ToString()) ? Convert.ToInt32(dr["PurchBillYearCode"]) : 0,
                                                      VendorName = dr["VendorName"].ToString(),
                                                      PartCode = dr["PartCode"].ToString(),
                                                      ItemName = dr["ItemName"].ToString(),
                                                      DPBAmt = !string.IsNullOrEmpty(dr["BillAmt"].ToString()) ? Convert.ToDecimal(dr["BillAmt"]) : 0,
                                                      //PONetAmt = !string.IsNullOrEmpty(dr["PONetAmt"].ToString()) ? (dr["PONetAmt"].ToString()) : string.Empty,
                                                      NetAmount = !string.IsNullOrEmpty(dr["BillAmt"].ToString()) ? Convert.ToSingle(dr["BillAmt"]) : 0,
                                                      BasicAmount = !string.IsNullOrEmpty(dr["NetAmt"].ToString()) ? Convert.ToSingle(dr["NetAmt"]) : 0,
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
                    DashBoardData.DPBDashboard = (from DataRow dr in oDataSet.Tables[0].Rows
                                                  select new DPBDashBoard
                                                  {
                                                      EntryID = !string.IsNullOrEmpty(dr["PurchBillEntryId"].ToString()) ? Convert.ToInt32(dr["PurchBillEntryId"]) : 0,
                                                      PurchVouchNo = dr["purvoucherno"].ToString(),
                                                      InvoiceNo = dr["InvoiceNo"].ToString(),
                                                      InvoiceDate = string.IsNullOrEmpty(dr["InvoiceDate"].ToString()) ? new DateTime() : Convert.ToDateTime(dr["InvoiceDate"]),
                                                      YearCode = !string.IsNullOrEmpty(dr["PurchBillYearCode"].ToString()) ? Convert.ToInt32(dr["PurchBillYearCode"]) : 0,
                                                      VendorName = dr["VendorName"].ToString(),
                                                      VendorAddress = dr["VendorAddress"].ToString(),
                                                      StateName = dr["StateName"].ToString(),
                                                      GSTType = dr["GSTType"].ToString(),
                                                      Currency = dr["Currency"].ToString(),
                                                      DocumentType = dr["PurchasetHead"].ToString(),
                                                      PartCode = dr["PartCode"].ToString(),
                                                      ItemName = dr["ItemName"].ToString(),
                                                      HsnNo = dr["HSNNO"].ToString(),
                                                      Unit = dr["Unit"].ToString(),
                                                      BillQty = !string.IsNullOrEmpty(dr["BillQty"].ToString()) ? Convert.ToSingle(dr["BillQty"]) : 0,
                                                      Rate = !string.IsNullOrEmpty(dr["Rate"].ToString()) ? dr["Rate"].ToString() : string.Empty,
                                                      DiscPer = !string.IsNullOrEmpty(dr["DiscountPer"].ToString()) ? Convert.ToSingle(dr["DiscountPer"]) : 0,
                                                      DiscRs = !string.IsNullOrEmpty(dr["DiscountAmt"].ToString()) ? Convert.ToSingle(dr["DiscountAmt"]) : 0,
                                                      Amount = !string.IsNullOrEmpty(dr["Amount"].ToString()) ? Convert.ToDecimal(dr["Amount"]) : 0,
                                                      PONo = !string.IsNullOrEmpty(dr["PONo"].ToString()) ? dr["PONo"].ToString() : string.Empty,
                                                      PODate = dr["PODate"].ToString(),
                                                      POYearCode = dr["POYearCode"].ToString(),
                                                      SchNo = !string.IsNullOrEmpty(dr["SchNo"].ToString()) ? dr["SchNo"].ToString() : string.Empty,
                                                      SchDate = dr["SchDate"].ToString(),
                                                      SchYearCode = dr["SchYearCode"].ToString(),
                                                      DPBAmt = !string.IsNullOrEmpty(dr["BillAmt"].ToString()) ? Convert.ToDecimal(dr["BillAmt"]) : 0,
                                                      PaymentTerms = dr["PaymentTerm"].ToString(),
                                                      Transporter = dr["Transporter"].ToString(),
                                                      VehicleNo = dr["Vehicleno"].ToString(),
                                                      TypeITEMSERVASSETS = dr["TypeITEMSERVASSETS"].ToString(),
                                                      CC = dr["Branch"].ToString(),
                                                      DomesticImport = dr["DomesticImport"].ToString(),
                                                      ModeOfTrans = dr["ModeOfTrans"].ToString(),
                                                      VoucherDate = dr["VoucherDate"].ToString(),
                                                      Approved = dr["Approved"].ToString(),
                                                      UpdatedByName = dr["EntryByMachine"].ToString(),
                                                      UpdatedBy = !string.IsNullOrEmpty(dr["UpdatedBy"].ToString()) ? 0 : Convert.ToInt32(dr["UpdatedBy"].ToString()),
                                                      EnteredBy = dr["EntryByMachine"].ToString(),
                                                      UpdatedOn = string.IsNullOrEmpty(dr["LastUpdatedDate"].ToString()) ? new DateTime() : Convert.ToDateTime(dr["LastUpdatedDate"]),
                                                      CreatedBy = !string.IsNullOrEmpty(dr["ActualEntryBy"].ToString()) ? 0 : Convert.ToInt32(dr["ActualEntryBy"].ToString()),
                                                      CreatedOn = string.IsNullOrEmpty(dr["ActualEntryDate"].ToString()) ? new DateTime() : Convert.ToDateTime(dr["ActualEntryDate"]),
                                                      BasicAmount = !string.IsNullOrEmpty(dr["BillAmt"].ToString()) ? Convert.ToSingle(dr["BillAmt"]) : 0,
                                                      NetAmount = !string.IsNullOrEmpty(dr["NetAmt"].ToString()) ? Convert.ToSingle(dr["NetAmt"]) : 0,
                                                      GSTAmount = !string.IsNullOrEmpty(dr["GSTAmount"].ToString()) ? Convert.ToSingle(dr["GSTAmount"]) : 0,
                                                      TaxableAmount = !string.IsNullOrEmpty(dr["Taxableamt"].ToString()) ? Convert.ToSingle(dr["Taxableamt"]) : 0,
                                                      TotalDiscPer = !string.IsNullOrEmpty(dr["ToatlDiscountPercent"].ToString()) ? Convert.ToSingle(dr["ToatlDiscountPercent"]) : 0,
                                                      TDSAmount = !string.IsNullOrEmpty(dr["TDSAmount"].ToString()) ? Convert.ToSingle(dr["TDSAmount"]) : 0,
                                                      Remark = !string.IsNullOrEmpty(dr["Remark"].ToString()) ? dr["Remark"].ToString() : string.Empty,
                                                      ApprovedDate = !string.IsNullOrEmpty(dr["ApprovedDate"].ToString()) ? dr["ApprovedDate"].ToString() : string.Empty,
                                                      ApprovedBy = !string.IsNullOrEmpty(dr["ApprovedBy"].ToString()) ? dr["ApprovedBy"].ToString() : string.Empty,
                                                      TaxVariationPOvsBill = !string.IsNullOrEmpty(dr["TaxVariationPOvsBill"].ToString()) ? dr["TaxVariationPOvsBill"].ToString() : string.Empty,
                                                      PONetAmt = !string.IsNullOrEmpty(dr["PONetAmt"].ToString()) ? dr["PONetAmt"].ToString() : string.Empty,
                                                      BlockRateVariation = !string.IsNullOrEmpty(dr["BlockRateVariation"].ToString()) ? dr["BlockRateVariation"].ToString() : string.Empty,
                                                      BlockReworkDebitNote = !string.IsNullOrEmpty(dr["BlockReworkDebitNote"].ToString()) ? dr["BlockReworkDebitNote"].ToString() : string.Empty,
                                                      BOEDate = !string.IsNullOrEmpty(dr["BOEDate"].ToString()) ? dr["BOEDate"].ToString() : string.Empty,
                                                      TotalAmtInOtherCurr = !string.IsNullOrEmpty(dr["TotalAmtInOtherCurr"].ToString()) ? Convert.ToSingle(dr["TotalAmtInOtherCurr"]) : 0,
                                                      Commodity = !string.IsNullOrEmpty(dr["Commodity"].ToString()) ? dr["Commodity"].ToString() : string.Empty,
                                                      BalanceSheetClosed = !string.IsNullOrEmpty(dr["BalanceSheetClosed"].ToString()) ? dr["BalanceSheetClosed"].ToString() : string.Empty,
                                                      PORemarks = !string.IsNullOrEmpty(dr["PORemarks"].ToString()) ? dr["PORemarks"].ToString() : string.Empty,
                                                      MRP = !string.IsNullOrEmpty(dr["MRP"].ToString()) ? Convert.ToSingle(dr["MRP"]) : 0,
                                                      RateUnit = !string.IsNullOrEmpty(dr["RateUnit"].ToString()) ? Convert.ToSingle(dr["RateUnit"]) : 0,
                                                      RateIncludingTaxes = !string.IsNullOrEmpty(dr["RateIncludingTaxes"].ToString()) ? Convert.ToSingle(dr["RateIncludingTaxes"]) : 0,
                                                      AmtinOtherCurr = !string.IsNullOrEmpty(dr["AmtinOtherCurr"].ToString()) ? Convert.ToSingle(dr["AmtinOtherCurr"]) : 0,
                                                      RateConversionFactor = !string.IsNullOrEmpty(dr["RateConversionFactor"].ToString()) ? Convert.ToSingle(dr["RateConversionFactor"]) : 0,
                                                      CostCenterName = !string.IsNullOrEmpty(dr["CostCenterName"].ToString()) ? dr["CostCenterName"].ToString() : string.Empty,
                                                      ItemColor = !string.IsNullOrEmpty(dr["ItemColor"].ToString()) ? dr["ItemColor"].ToString() : string.Empty,
                                                      ItemModel = !string.IsNullOrEmpty(dr["ItemModel"].ToString()) ? dr["ItemModel"].ToString() : string.Empty,
                                                      POAmmNo = !string.IsNullOrEmpty(dr["POAmmNo"].ToString()) ? dr["POAmmNo"].ToString() : string.Empty,
                                                      PoRate = !string.IsNullOrEmpty(dr["PoRate"].ToString()) ? Convert.ToSingle(dr["PoRate"]) : 0,
                                                      POType = !string.IsNullOrEmpty(dr["POType"].ToString()) ? dr["POType"].ToString() : string.Empty,
                                                      ProjectNo = !string.IsNullOrEmpty(dr["ProjectNo"].ToString()) ? dr["ProjectNo"].ToString() : string.Empty,
                                                      ProjectDate = !string.IsNullOrEmpty(dr["ProjectDate"].ToString()) ? dr["ProjectDate"].ToString() : string.Empty,
                                                      ProjectYearCode = !string.IsNullOrEmpty(dr["ProjectYearCode"].ToString()) ? dr["ProjectYearCode"].ToString() : string.Empty,
                                                      AgainstImportAccountCode = !string.IsNullOrEmpty(dr["AgainstImportAccountCode"].ToString()) ? dr["AgainstImportAccountCode"].ToString() : string.Empty,
                                                      AgainstImportInvoiceNo = !string.IsNullOrEmpty(dr["AgainstImportInvoiceNo"].ToString()) ? dr["AgainstImportInvoiceNo"].ToString() : string.Empty,
                                                      AgainstImportYearCode = !string.IsNullOrEmpty(dr["AgainstImportYearCode"].ToString()) ? dr["AgainstImportYearCode"].ToString() : string.Empty,
                                                      AgainstImportInvDate = !string.IsNullOrEmpty(dr["AgainstImportInvDate"].ToString()) ? dr["AgainstImportInvDate"].ToString() : string.Empty,
                                                      AgainstInvNo = dr["AgainstInvNo"].ToString(),
                                                      AgainstVoucherNo = dr["AgainstVoucherNo"].ToString(),
                                                      AgainstVoucherDate = string.IsNullOrEmpty(dr["AgainstVoucherDate"].ToString()) ? new DateTime() : Convert.ToDateTime(dr["AgainstVoucherDate"]),
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

    public async Task<DirectPurchaseBillModel> GetViewByID(int ID, int YC, string Flag)
    {
        var oDataSet = new DataSet();
        var MainModel = new DirectPurchaseBillModel();
        var _ItemList = new List<DPBItemDetail>();
        var _TaxList = new List<TaxModel>();
        var _TdsList = new List<TDSModel>();
        MainModel.adjustmentModel = new AdjustmentModel();
        var _AdjList = new List<AdjustmentModel>();
        var SqlParams = new List<dynamic>();
        //var listObject = new List<DeliverySchedule>();

        try
        {
            SqlParams.Add(new SqlParameter("@Flag", "ViewByID"));
            SqlParams.Add(new SqlParameter("@EntryID", ID));
            SqlParams.Add(new SqlParameter("@YearCode", YC));

            var ResponseResult = await _IDataLogic.ExecuteDataSet("SP_DirectPurchaseBillMainDetail", SqlParams);

            if (((DataSet)ResponseResult.Result).Tables.Count > 0 && ((DataSet)ResponseResult.Result).Tables[0].Rows.Count > 0)
            {
                oDataSet = ResponseResult.Result;
                oDataSet.Tables[0].TableName = "DPBMain";
                oDataSet.Tables[1].TableName = "ItemDetailGrid";
                oDataSet.Tables[2].TableName = "DPBTaxDetail";
                oDataSet.Tables[3].TableName = "DPBTDSDetail";
                oDataSet.Tables[4].TableName = "DPBAdjDetail";

                if (oDataSet.Tables.Count != 0 && oDataSet.Tables[0].Rows.Count > 0)
                {
                    MainModel.EntryID = Convert.ToInt32(oDataSet.Tables[0].Rows[0]["PurchBillEntryId"]);
                    MainModel.YearCode = Convert.ToInt32(oDataSet.Tables[0].Rows[0]["PurchBillYearCode"]);
                    MainModel.EntryDate = oDataSet.Tables[0].Rows[0]["EntryDate"].ToString();
                    MainModel.Branch = oDataSet.Tables[0].Rows[0]["CC"].ToString();
                    MainModel.Currency = Convert.ToInt32(oDataSet.Tables[0].Rows[0]["CurrencyId"]);
                    MainModel.AccountCode = Convert.ToInt32(oDataSet.Tables[0].Rows[0]["AccountCode"]);
                    MainModel.InvoiceNo = oDataSet.Tables[0].Rows[0]["InvoiceNo"].ToString();
                    MainModel.InvDate = oDataSet.Tables[0].Rows[0]["InvoiceDate"].ToString();
                    MainModel.PurchVouchNo = oDataSet.Tables[0].Rows[0]["purvoucherno"].ToString();
                    MainModel.VouchDate = oDataSet.Tables[0].Rows[0]["VoucherDate"].ToString();
                    MainModel.VendorStateName = oDataSet.Tables[0].Rows[0]["StateName"].ToString();
                    MainModel.VendorAddress = oDataSet.Tables[0].Rows[0]["VendorAddress"].ToString();
                    MainModel.GstType = oDataSet.Tables[0].Rows[0]["GSTType"].ToString();
                    MainModel.DPBType = oDataSet.Tables[0].Rows[0]["DomesticImport"].ToString();
                    MainModel.PaymentTerms = oDataSet.Tables[0].Rows[0]["PaymentTerm"].ToString();
                    MainModel.ModeOfTransport = oDataSet.Tables[0].Rows[0]["ModeOfTrans"].ToString();
                    MainModel.Transport = oDataSet.Tables[0].Rows[0]["Transporter"].ToString();
                    MainModel.VehicleNo = oDataSet.Tables[0].Rows[0]["Vehicleno"].ToString();
                    MainModel.ExchangeRate = string.IsNullOrEmpty(oDataSet.Tables[0].Rows[0]["ExchangeRate"].ToString()) ? 0 : Convert.ToSingle(oDataSet.Tables[0].Rows[0]["ExchangeRate"]);
                    MainModel.ItemNetAmount = string.IsNullOrEmpty(oDataSet.Tables[0].Rows[0]["NetAmt"].ToString()) ? 0 : Convert.ToDecimal(oDataSet.Tables[0].Rows[0]["NetAmt"]);
                    MainModel.NetTotal = string.IsNullOrEmpty(oDataSet.Tables[0].Rows[0]["BillAmt"].ToString()) ? 0 : (oDataSet.Tables[0].Rows[0]["RoundoffType"].ToString().ToLower() == "y") ? Convert.ToDecimal(Math.Round(Convert.ToDecimal(oDataSet.Tables[0].Rows[0]["BillAmt"]))) : Convert.ToDecimal(oDataSet.Tables[0].Rows[0]["BillAmt"]);
                    MainModel.PaymentDays = string.IsNullOrEmpty(oDataSet.Tables[0].Rows[0]["PaymentDays"].ToString()) ? 0 : Convert.ToInt32(oDataSet.Tables[0].Rows[0]["PaymentDays"]);
                    MainModel.PreparedBy = string.IsNullOrEmpty(oDataSet.Tables[0].Rows[0]["ActualEntryBy"].ToString()) ? 0 : Convert.ToInt32(oDataSet.Tables[0].Rows[0]["ActualEntryBy"]);
                    MainModel.PreparedByName = string.IsNullOrEmpty(oDataSet.Tables[0].Rows[0]["EntryByMachine"].ToString()) ? string.Empty : oDataSet.Tables[0].Rows[0]["EntryByMachine"].ToString();
                    MainModel.PathOfFile1URL = oDataSet.Tables[0].Rows[0]["PathOfFile1"].ToString();
                    MainModel.PathOfFile2URL = oDataSet.Tables[0].Rows[0]["PathOfFile2"].ToString();
                    MainModel.PathOfFile3URL = oDataSet.Tables[0].Rows[0]["PathOfFile3"].ToString();
                    MainModel.TotalDiscountPercentage = string.IsNullOrEmpty(oDataSet.Tables[0].Rows[0]["ToatlDiscountPercent"].ToString()) ? 0 : Convert.ToDecimal(oDataSet.Tables[0].Rows[0]["ToatlDiscountPercent"]);
                    MainModel.TotalAmtAftrDiscount = string.IsNullOrEmpty(oDataSet.Tables[0].Rows[0]["TotalDiscountAmount"].ToString()) ? 0 : Convert.ToDecimal(oDataSet.Tables[0].Rows[0]["TotalDiscountAmount"]);
                    MainModel.TotalRoundOff = oDataSet.Tables[0].Rows[0]["RoundoffType"].ToString();
                    MainModel.TotalRoundOffAmt = Convert.ToDecimal(oDataSet.Tables[0].Rows[0]["RoundOffAmt"]);
                    MainModel.CreatedBy = Convert.ToInt32(oDataSet.Tables[0].Rows[0]["ActualEntryBy"].ToString());
                    MainModel.CretaedByName = oDataSet.Tables[0].Rows[0]["EntryByMachine"].ToString();

                    MainModel.Remark = string.IsNullOrEmpty(oDataSet.Tables[0].Rows[0]["Remark"].ToString()) ? string.Empty : oDataSet.Tables[0].Rows[0]["Remark"].ToString();
                    MainModel.CreatedOn = string.IsNullOrEmpty(oDataSet.Tables[0].Rows[0]["LastUpdatedDate"].ToString()) ? new DateTime() : Convert.ToDateTime(oDataSet.Tables[0].Rows[0]["LastUpdatedDate"]);
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
                        _ItemList.Add(new DPBItemDetail
                        {
                            SeqNo = cnt++,
                            docTypeId = Convert.ToInt32(row["DocTypeID"]),
                            DocTypeText = row["DocumentType"].ToString(),
                            PartCode = Convert.ToInt32(row["ItemCode"]),
                            ItemCode = (!string.IsNullOrEmpty(row["ItemCode"].ToString())) ? Convert.ToInt32(row["ItemCode"]) : 0,
                            ItemText = (!string.IsNullOrEmpty(row["ItemName"].ToString())) ? row["ItemName"].ToString() : string.Empty,
                            PONo = row["PONo"].ToString(),
                            POYear = (!string.IsNullOrEmpty(row["POYearCode"].ToString())) ? Convert.ToInt32(row["POYearCode"]) : 0,
                            PODate = row["PODate"].ToString(),
                            ScheduleNo = row["SchNo"].ToString(),
                            ScheduleYear = (!string.IsNullOrEmpty(row["SchYearCode"].ToString())) ? Convert.ToInt32(row["SchYearCode"]) : 0,
                            ScheduleDate = row["SchDate"].ToString(),
                            HSNNo = (!string.IsNullOrEmpty(row["HSNNO"].ToString())) ? Convert.ToInt32(row["HSNNO"]) : 0,
                            DPBQty = (!string.IsNullOrEmpty(row["RecQty"].ToString())) ? Convert.ToDecimal(row["RecQty"]) : 0,
                            Unit = (!string.IsNullOrEmpty(row["Unit"].ToString())) ? row["Unit"].ToString() : string.Empty,
                            BillQty = (!string.IsNullOrEmpty(row["BillQty"].ToString())) ? Convert.ToDecimal(row["BillQty"]) : 0,
                            Rate = (!string.IsNullOrEmpty(row["ItemCode"].ToString())) ? Convert.ToDecimal(row["ItemCode"]) : 0,
                            OtherRateCurr = (!string.IsNullOrEmpty(row["AmtinOtherCurr"].ToString())) ? Convert.ToDecimal(row["AmtinOtherCurr"]) : 0,
                            DiscPer = Convert.ToDecimal(row["DiscountPer"]),
                            DiscRs = Convert.ToDecimal(row["DiscountAmt"]),
                            Amount = Convert.ToDecimal(row["Amount"]),
                            PartText = row["PartCode"].ToString(),
                            Description = row["OtherDetail"].ToString(),
                            UnitRate = row["RateUnit"].ToString(),
                            Process = (!string.IsNullOrEmpty(row["ProcessId"].ToString())) ? Convert.ToInt32(row["ProcessId"]) : 0,
                            ProcessName = row["ProcessName"].ToString(),
                            CostCenter = (!string.IsNullOrEmpty(row["CostCenterId"].ToString())) ? Convert.ToInt32(row["CostCenterId"]) : 0,
                            CostCenterName = row["CostCenterName"].ToString(),
                        });
                    }

                    MainModel.ItemDetailGrid = _ItemList;
                    MainModel.ItemNetAmount = decimal.Parse(MainModel.ItemDetailGrid.Sum(x => x.Amount).ToString("#.#0"));
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

            _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_DirectPurchaseBillMainDetail", SqlParams);

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

            _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_DirectPurchaseBillMainDetail", SqlParams);

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


    public async Task<ResponseResult> SaveDirectPurchaseBILL(DataTable ItemDetailDT, DataTable TaxDetailDT, DataTable TDSDetailDT, DirectPurchaseBillModel model, DataTable DrCrDetailDT, DataTable AdjDetailDT)
    {
        var _ResponseResult = new ResponseResult();
        try
        {
            var SqlParams = new List<dynamic>();

            DateTime EntryDt = new DateTime();
            DateTime VouchDate = new DateTime();
            DateTime InvDate = new DateTime();
            DateTime AppDate = new DateTime();
            DateTime CurrentDate = new DateTime();

            EntryDt = ParseDate(model.EntryDate);
            VouchDate = ParseDate(model.VouchDate);
            InvDate = ParseDate(model.InvDate);
            CurrentDate = DateTime.Today;

            SqlParams.Add(new SqlParameter("@Flag", model.Mode == "COPY" ? "INSERT" : model.Mode));
            //SqlParams.Add(new SqlParameter("@ID", model.ID));
            if (model.Mode == "INSERT")
                SqlParams.Add(new SqlParameter("@EntryID", 0));
            else
                SqlParams.Add(new SqlParameter("@EntryID", model.EntryID));
            if (model.Mode == "POA")
            {
                AppDate = ParseDate(model.ApprovedDate);
                SqlParams.Add(new SqlParameter("@ApprovedDate", AppDate == default ? string.Empty : AppDate));
                SqlParams.Add(new SqlParameter("@Approved", model.Approved));
                SqlParams.Add(new SqlParameter("@Approvedby", model.Approvedby));
            }

            SqlParams.Add(new SqlParameter("@YearCode", model.YearCode));
            SqlParams.Add(new SqlParameter("@EntryDate", EntryDt == default ? string.Empty : EntryDt));
            SqlParams.Add(new SqlParameter("@InvNo", model.InvoiceNo));
            SqlParams.Add(new SqlParameter("@InvoiceTime", InvDate == default ? string.Empty : InvDate));
            SqlParams.Add(new SqlParameter("@InvoiceDate", InvDate == default ? string.Empty : InvDate));
            SqlParams.Add(new SqlParameter("@PurchVoucherNo", model.PurchVouchNo));
            SqlParams.Add(new SqlParameter("@VoucherDate", VouchDate == default ? string.Empty : VouchDate));
            SqlParams.Add(new SqlParameter("@AccountCode", model.AccountCode));
            SqlParams.Add(new SqlParameter("@StateName", model.VendorStateName));
            SqlParams.Add(new SqlParameter("@GSTType", model.GstType));
            SqlParams.Add(new SqlParameter("@TypeITEMSERVASSETS", model.DPBTypeServItem));

            SqlParams.Add(new SqlParameter("@DomesticImport", model.DPBType));
            SqlParams.Add(new SqlParameter("@PaymentTerm", model.PaymentTerms));
            SqlParams.Add(new SqlParameter("@Transporter", model.Transport));
            SqlParams.Add(new SqlParameter("@Vehicleno", model.VehicleNo));
            SqlParams.Add(new SqlParameter("@CurrencyId", Convert.ToInt32(model.Currency)));
            SqlParams.Add(new SqlParameter("@ExchangeRate", model.ExchangeRate));
            SqlParams.Add(new SqlParameter("@ConversionFactor", model.ExchangeRate));
            SqlParams.Add(new SqlParameter("@BillAmt", (float)Math.Round(model.NetTotal, 2)));
            SqlParams.Add(new SqlParameter("@RoundOffAmt", (float)Math.Round(model.TotalRoundOffAmt, 2)));
            SqlParams.Add(new SqlParameter("@RoundoffType", model.TotalRoundOff));
            SqlParams.Add(new SqlParameter("@GSTAmount", 0));
            SqlParams.Add(new SqlParameter("@Taxableamt", (float)Math.Round(model.TxAmount, 2)));
            SqlParams.Add(new SqlParameter("@ToatlDiscountPercent", (float)Math.Round(model.TotalDiscountPercentage, 2)));
            SqlParams.Add(new SqlParameter("@TotalDiscountAmount", (float)Math.Round(model.TotalAmtAftrDiscount, 2)));
            SqlParams.Add(new SqlParameter("@NetAmt", (float)model.ItemNetAmount));
            SqlParams.Add(new SqlParameter("@Remark", model.Remark));
            SqlParams.Add(new SqlParameter("@CC", model.Branch));
            SqlParams.Add(new SqlParameter("@Uid", model.CreatedBy));
            SqlParams.Add(new SqlParameter("@TaxVariationPOvsBill", string.Empty));
            SqlParams.Add(new SqlParameter("@PONetAmt", (float)Math.Round(model.NetTotal, 2)));
            SqlParams.Add(new SqlParameter("@BOEDate", EntryDt == default ? string.Empty : EntryDt));
            SqlParams.Add(new SqlParameter("@ModeOfTrans", model.ModeOfTransport));
            SqlParams.Add(new SqlParameter("@TotalAmtInOtherCurr", (float)Math.Round(model.NetTotal, 2)));
            SqlParams.Add(new SqlParameter("@boeno", string.Empty));
            SqlParams.Add(new SqlParameter("@Commodity", string.Empty));
            SqlParams.Add(new SqlParameter("@PathOfFile1", model.PathOfFile1URL));
            SqlParams.Add(new SqlParameter("@PathOfFile2", model.PathOfFile2URL));
            SqlParams.Add(new SqlParameter("@PathOfFile3", model.PathOfFile3URL));
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
                SqlParams.Add(new SqlParameter("@CreatedBY", model.CreatedBy));
            }
            SqlParams.Add(new SqlParameter("@EntryByMachine", model.EntryByMachineName));
            SqlParams.Add(new SqlParameter("@EntryByMachineName", model.EntryByMachineName));

            _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_DirectPurchaseBillMainDetail", SqlParams);
        }
        catch (Exception ex)
        {
            dynamic Error = new ExpandoObject();  
            Error.Message = ex.Message;
            Error.Source = ex.Source;
        }

        return _ResponseResult;
    }

    //private void RoundFloatColumns(DataTable table)
    //{
    //    if(table == null) { return; }
    //    else if(table.Rows.Count == 0) { return; }
    //    foreach (DataRow row in table.Rows)
    //    {
    //        foreach (DataColumn col in table.Columns)
    //        {
    //            if (col.DataType == typeof(float))
    //            {
    //                float value = (float)row[col];
    //                row[col] = (float)Math.Round(value, 2);
    //            }
    //        }
    //    }
    //}
    private void RoundFloatColumns(DataTable table)
    {
        if (table == null || table.Rows.Count == 0)
        {
            return;
        }
        foreach (DataRow row in table.Rows)
        {
            foreach (DataColumn col in table.Columns)
                if (col.DataType == typeof(float))
                {
                    if (row[col] != DBNull.Value)
                    {
                        float value = Convert.ToSingle(row[col]);
                        row[col] = (float)Math.Round(value, 2);
                    }
                    else
                    {
                        row[col] = 0.0; 
                    }
                }
        }
    }
}
