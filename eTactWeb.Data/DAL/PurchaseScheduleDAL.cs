using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Data;
using System.Globalization;
using static eTactWeb.DOM.Models.Common;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace eTactWeb.Data.DAL;

public class PurchaseScheduleDAL
{
    private readonly ConnectionStringService _connectionStringService;
    public PurchaseScheduleDAL(IConfiguration configuration, IDataLogic iDataLogic, ConnectionStringService connectionStringService)
    {
        //DBConnectionString = configuration.GetConnectionString("eTactDB");
        _IDataLogic = iDataLogic;
        _connectionStringService = connectionStringService;
        DBConnectionString = _connectionStringService.GetConnectionString();
    }

    public IDataLogic? _IDataLogic { get; }

    public IConfiguration? Configuration { get; }

    private string DBConnectionString { get; }

    public async Task<ResponseResult> FillMRPNo()
    {
        var _ResponseResult = new ResponseResult();
        try
        {
            var SqlParams = new List<dynamic>();
            SqlParams.Add(new SqlParameter("@Flag", "MRPDETAIL"));
            


            _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_PurchaseSchedule", SqlParams);
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

            _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_PurchaseSchedule", SqlParams);

        }
        catch (Exception ex)
        {
            dynamic Error = new ExpandoObject();
            Error.Message = ex.Message;
            Error.Source = ex.Source;
        }
        return _ResponseResult;
    }
    public async Task<ResponseResult> FillMRPDetail(string MRPNo)
    {
        var _ResponseResult = new ResponseResult();
        try
        {
            var SqlParams = new List<dynamic>();
            SqlParams.Add(new SqlParameter("@Flag", "MRPYEAR"));
            SqlParams.Add(new SqlParameter("@MRPNO", MRPNo));


            _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_PurchaseOrder", SqlParams);
        }
        catch (Exception ex)
        {
            dynamic Error = new ExpandoObject();
            Error.Message = ex.Message;
            Error.Source = ex.Source;
        }

        return _ResponseResult;
    }


    public async Task<DataSet> BindAllDropDown()
    {
        var oDataSet = new DataSet();

        try
        {
            var SqlParams = new List<dynamic>();
            SqlParams.Add(new SqlParameter("@Flag", "BINDALLDROPDOWN"));
            var _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_PurchaseSchedule", SqlParams);
            if (_ResponseResult.Result != null && _ResponseResult.StatusCode == HttpStatusCode.OK && _ResponseResult.StatusText == "Success")
            {
                _ResponseResult.Result.Tables[0].TableName = "BranchList";
                _ResponseResult.Result.Tables[1].TableName = "AccountList";
                oDataSet = _ResponseResult.Result;
            }
        }
        catch (Exception ex)
        {
            dynamic Error = new ExpandoObject();
            Error.Message = ex.Message;
            Error.Source = ex.Source;
        }

        return oDataSet;
    }

    public async Task<ResponseResult> GetFormRights(int userId)
    {
        var _ResponseResult = new ResponseResult();
        try
        {
            var SqlParams = new List<dynamic>();
            SqlParams.Add(new SqlParameter("@Flag", "GetRights"));
            SqlParams.Add(new SqlParameter("@EmpId", userId));
            SqlParams.Add(new SqlParameter("@MainMenu", "Purchase Schedule"));
            //SqlParams.Add(new SqlParameter("@SubMenu", "Purchase Schedule"));

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

    public async Task<ResponseResult> GetFormRightsAmm(int userId)
    {
        var _ResponseResult = new ResponseResult();
        try
        {
            var SqlParams = new List<dynamic>();
            SqlParams.Add(new SqlParameter("@Flag", "GetRights"));
            SqlParams.Add(new SqlParameter("@EmpId", userId));
            SqlParams.Add(new SqlParameter("@MainMenu", "Purchase Schedule Amendment"));
            //SqlParams.Add(new SqlParameter("@SubMenu", "Purchase Schedule"));

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


    public async Task<ResponseResult> DeleteByID(int ID, int YC, int createdBy, string entryByMachineName)
    {
        var _ResponseResult = new ResponseResult();
        try
        {
            var SqlParams = new List<dynamic>();
            SqlParams.Add(new SqlParameter("@Flag", "DELETEBYID"));
            SqlParams.Add(new SqlParameter("@EntryID", ID));
            SqlParams.Add(new SqlParameter("@Schyearcode", YC));
            SqlParams.Add(new SqlParameter("@Yearcode", YC));
            SqlParams.Add(new SqlParameter("@CreatedBy", createdBy));
            SqlParams.Add(new SqlParameter("@EntryByMachineName", entryByMachineName));
            _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_PurchaseSchedule", SqlParams);
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

    public async Task<ResponseResult> GetDashboardData(string ToDate)
    {
        var _ResponseResult = new ResponseResult();
        try
        {
            DateTime now = DateTime.Now;
            DateTime firstDayOfMonth = new DateTime(now.Year, now.Month, 1);
            var SqlParams = new List<dynamic>();
            SqlParams.Add(new SqlParameter("@Flag", "DASHBOARD"));
            SqlParams.Add(new SqlParameter("@StartDate", firstDayOfMonth));
            SqlParams.Add(new SqlParameter("@EndDate", ToDate));

            _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_PurchaseSchedule", SqlParams);
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

            _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_PurchaseOrder", SqlParams);
        }
        catch (Exception ex)
        {
            dynamic Error = new ExpandoObject();
            Error.Message = ex.Message;
            Error.Source = ex.Source;
        }
        return _ResponseResult;
    }
    public async Task<ResponseResult> FillEntry(int YearCode)
    {
        var _ResponseResult = new ResponseResult();
        try
        {
            var SqlParams = new List<dynamic>();
            SqlParams.Add(new SqlParameter("@Flag", "NewEntryId"));
            SqlParams.Add(new SqlParameter("@YearCode", YearCode));
            SqlParams.Add(new SqlParameter("@Schyearcode", YearCode));
            SqlParams.Add(new SqlParameter("@EntryDate", DateTime.Today));

            _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_PurchaseSchedule", SqlParams);
        }
        catch (Exception ex)
        {
            dynamic Error = new ExpandoObject();
            Error.Message = ex.Message;
            Error.Source = ex.Source;
        }
        return _ResponseResult;
    }
    public async Task<ResponseResult> GetAddress(int AcountCode)
    {
        var _ResponseResult = new ResponseResult();
        try
        {
            var SqlParams = new List<dynamic>();
            SqlParams.Add(new SqlParameter("@Flag", "GETADDRESS"));
            SqlParams.Add(new SqlParameter("@Accountcode", AcountCode));

            _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_PurchaseSchedule", SqlParams);
        }
        catch (Exception ex)
        {
            dynamic Error = new ExpandoObject();
            Error.Message = ex.Message;
            Error.Source = ex.Source;
        }
        return _ResponseResult;
    }
    public async Task<ResponseResult> FillAmendEntry(int YearCode)
    {
        var _ResponseResult = new ResponseResult();
        try
        {
            var SqlParams = new List<dynamic>();
            SqlParams.Add(new SqlParameter("@Flag", "GetAmendEntry"));
            SqlParams.Add(new SqlParameter("@AmmYearCode", YearCode));

            _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_PurchaseSchedule", SqlParams);
        }
        catch (Exception ex)
        {
            dynamic Error = new ExpandoObject();
            Error.Message = ex.Message;
            Error.Source = ex.Source;
        }
        return _ResponseResult;
    }

    public async Task<ResponseResult> GetSearchData(PSDashboard model)
    {
        var _ResponseResult = new ResponseResult();
        try
        {
            var SqlParams = new List<dynamic>();
            DateTime fromDt = DateTime.ParseExact(model.FromDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            DateTime toDt = DateTime.ParseExact(model.ToDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            SqlParams.Add(new SqlParameter("@Flag", "SEARCH"));
            SqlParams.Add(new SqlParameter("@vendorName", model.VendorName));
            SqlParams.Add(new SqlParameter("@PONo", model.PONO));
            SqlParams.Add(new SqlParameter("@ScheduleNo", model.SchNo));
            SqlParams.Add(new SqlParameter("@ItemName", model.ItemName));
            SqlParams.Add(new SqlParameter("@StartDate", fromDt.ToString("yyyy/MM/dd")));
            SqlParams.Add(new SqlParameter("@EndDate", toDt.ToString("yyyy/MM/dd")));

            _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_PurchaseSchedule", SqlParams);
        }
        catch (Exception ex)
        {
            dynamic Error = new ExpandoObject();
            Error.Message = ex.Message;
            Error.Source = ex.Source;
        }
        return _ResponseResult;
    }
    public async Task<ResponseResult> GetDetailData(PSDashboard model)
    {
        var _ResponseResult = new ResponseResult();
        try
        {
            var SqlParams = new List<dynamic>();
            DateTime fromDt = DateTime.ParseExact(model.FromDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            DateTime toDt = DateTime.ParseExact(model.ToDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            SqlParams.Add(new SqlParameter("@Flag", "DETAILSEARCH"));
            SqlParams.Add(new SqlParameter("@vendorName", model.VendorName));
            SqlParams.Add(new SqlParameter("@PONo", model.PONO));
            SqlParams.Add(new SqlParameter("@ScheduleNo", model.SchNo));
            SqlParams.Add(new SqlParameter("@ItemName", model.ItemName));
            SqlParams.Add(new SqlParameter("@StartDate", fromDt.ToString("yyyy/MM/dd")));
            SqlParams.Add(new SqlParameter("@EndDate", toDt.ToString("yyyy/MM/dd")));

            _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_PurchaseSchedule", SqlParams);
        }
        catch (Exception ex)
        {
            dynamic Error = new ExpandoObject();
            Error.Message = ex.Message;
            Error.Source = ex.Source;
        }
        return _ResponseResult;
    }
    public async Task<ResponseResult> GetPSAmmData(PSDashboard model)
    {
        var _ResponseResult = new ResponseResult();
        try
        {
            var SqlParams = new List<dynamic>();
            DateTime fromDt = DateTime.ParseExact(model.FromDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            DateTime toDt = DateTime.ParseExact(model.ToDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            SqlParams.Add(new SqlParameter("@Flag", "PSAMMDASHBOARD"));
            SqlParams.Add(new SqlParameter("@vendorName", model.VendorName));
            SqlParams.Add(new SqlParameter("@PONo", model.PONO));
            SqlParams.Add(new SqlParameter("@ScheduleNo", model.SchNo));
            SqlParams.Add(new SqlParameter("@ItemName", model.ItemName));
            SqlParams.Add(new SqlParameter("@StartDate", fromDt.ToString("yyyy/MM/dd")));
            SqlParams.Add(new SqlParameter("@EndDate", toDt.ToString("yyyy/MM/dd")));

            _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_PurchaseSchedule", SqlParams);
        }
        catch (Exception ex)
        {
            dynamic Error = new ExpandoObject();
            Error.Message = ex.Message;
            Error.Source = ex.Source;
        }
        return _ResponseResult;
    }
    public async Task<ResponseResult> GetUpdPSAmmData(PSDashboard model)
    {
        var _ResponseResult = new ResponseResult();
        try
        {
            var SqlParams = new List<dynamic>();
            DateTime fromDt = DateTime.ParseExact(model.FromDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            DateTime toDt = DateTime.ParseExact(model.ToDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            SqlParams.Add(new SqlParameter("@Flag", "UPDPSAMMDASHBOARD"));
            SqlParams.Add(new SqlParameter("@vendorName", model.VendorName));
            SqlParams.Add(new SqlParameter("@PONo", model.PONO));
            SqlParams.Add(new SqlParameter("@ScheduleNo", model.SchNo));
            SqlParams.Add(new SqlParameter("@ItemName", model.ItemName));
            SqlParams.Add(new SqlParameter("@StartDate", fromDt.ToString("yyyy/MM/dd")));
            SqlParams.Add(new SqlParameter("@EndDate", toDt.ToString("yyyy/MM/dd")));

            _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_PurchaseSchedule", SqlParams);
        }
        catch (Exception ex)
        {
            dynamic Error = new ExpandoObject();
            Error.Message = ex.Message;
            Error.Source = ex.Source;
        }
        return _ResponseResult;
    }

    public async Task<string> GetPODATA(int AccountCode, string PONO, int Year)
    {
        var JsonString = string.Empty;
        try
        {
            var SqlParams = new List<dynamic>();

            SqlParams.Add(new SqlParameter("@Flag", "PODATA"));
            SqlParams.Add(new SqlParameter("@AccountCode", AccountCode));
            SqlParams.Add(new SqlParameter("@PONO", PONO));
            //SqlParams.Add(new SqlParameter("@YearCode", Year));
            SqlParams.Add(new SqlParameter("@POYearCode", Year));
            var _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_PurchaseSchedule", SqlParams);
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

    public async Task<string> GetPOItem(object AccountCode, object PONO, object Year, int ItemCode)
    {
        var JsonString = string.Empty;
        try
        {
            var SqlParams = new List<dynamic>();

            SqlParams.Add(new SqlParameter("@Flag", "POITEM"));
            SqlParams.Add(new SqlParameter("@AccountCode", AccountCode));
            SqlParams.Add(new SqlParameter("@PONO", PONO));
            SqlParams.Add(new SqlParameter("@POYearCode", Year));
            //SqlParams.Add(new SqlParameter("@ID", 0));
            SqlParams.Add(new SqlParameter("@ID", ItemCode));
            var _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_PurchaseSchedule", SqlParams);
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

    public async Task<string> GetPONO(int AccountCode)
    {
        var JsonString = string.Empty;
        try
        {
            var SqlParams = new List<dynamic>();
            SqlParams.Add(new SqlParameter("@Flag", "PONOBYACCOUNT"));
            SqlParams.Add(new SqlParameter("@AccountCode", AccountCode));
            var _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_PurchaseSchedule", SqlParams);
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

    public async Task<string> GetPOYear(int AccountCode, string PONO)
    {
        var JsonString = string.Empty;
        try
        {
            var SqlParams = new List<dynamic>();

            SqlParams.Add(new SqlParameter("@Flag", "GETPOYEAR"));
            SqlParams.Add(new SqlParameter("@AccountCode", AccountCode));
            SqlParams.Add(new SqlParameter("@PONO", PONO));
            var _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_PurchaseSchedule", SqlParams);
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

    public async Task<PurchaseSubScheduleModel> GetViewByID(int ID, int YearCode, string Mode)
    {
        var model = new PurchaseSubScheduleModel();                                                       
        try
        {
            var SqlParams = new List<dynamic>();

            SqlParams.Add(new SqlParameter("@Flag", "VIEWBYID"));
            SqlParams.Add(new SqlParameter("@EntryID", ID));
            SqlParams.Add(new SqlParameter("@Schyearcode", YearCode));

            var _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_PurchaseSchedule", SqlParams);

            if (_ResponseResult.Result != null && _ResponseResult.StatusCode == HttpStatusCode.OK && _ResponseResult.StatusText == "Success")
            {
                PrepareView(_ResponseResult.Result,Mode, ref model);
            }
        }
        catch (Exception ex)
        {
            dynamic Error = new ExpandoObject();
            Error.Message = ex.Message;
            Error.Source = ex.Source;
        }

        return model;
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
    public async Task<ResponseResult> GetItemCode(string PartCode)
    {
        var _ResponseResult = new ResponseResult();
        try
        {
            var SqlParams = new List<dynamic>();
            SqlParams.Add(new SqlParameter("@Flag", "GetItemCode"));
            SqlParams.Add(new SqlParameter("@PartCode", PartCode));

            _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_PurchaseSchedule", SqlParams);

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

            SqlParams.Add(new SqlParameter("@Date", DateTime.Today));
            SqlParams.Add(new SqlParameter("@Currency", Currency));

            _ResponseResult = await _IDataLogic.ExecuteDataTable("getExchangeRate", SqlParams);
        }
        catch (Exception ex)
        {
            dynamic Error = new ExpandoObject();
            Error.Message = ex.Message;
            Error.Source = ex.Source;
        }

        return _ResponseResult;
    }
    public async Task<ResponseResult> GetCurrency(string PONo, int POYearCode, int ItemCode, int AccountCode, string SchNo, int SchYearCode, string Flag)
    {
        var _ResponseResult = new ResponseResult();
        try
        {
            var SqlParams = new List<dynamic>();
            SqlParams.Add(new SqlParameter("@POno", PONo));
            SqlParams.Add(new SqlParameter("@POYearcode", POYearCode));
            SqlParams.Add(new SqlParameter("@itemCode", ItemCode));
            SqlParams.Add(new SqlParameter("@AccountCode", AccountCode));
            SqlParams.Add(new SqlParameter("@SchNo", SchNo));
            SqlParams.Add(new SqlParameter("@SchYearCode", SchYearCode));
            SqlParams.Add(new SqlParameter("@Flag", Flag));

            _ResponseResult = await _IDataLogic.ExecuteDataSet("SPGetPendPOQty", SqlParams);
        }
        catch (Exception ex)
        {
            dynamic Error = new ExpandoObject();
            Error.Message = ex.Message;
            Error.Source = ex.Source;
        }
        return _ResponseResult;
    }
    public async Task<ResponseResult> SavePurchaseSchedule(PurchaseSubScheduleModel model, DataTable DTSSGrid)
    {
        var _ResponseResult = new ResponseResult();
        try
        {
            var SqlParams = new List<dynamic>();
            DateTime entDt = new DateTime();
            DateTime poDt = new DateTime();
            DateTime poAmdDt = new DateTime();
            DateTime schDt = new DateTime();
            DateTime schAmdDt = new DateTime();
            DateTime schEffFromDt = new DateTime();
            DateTime schEffToDate = new DateTime();
            DateTime AppDate = new DateTime();
            DateTime AmmAppDate = new DateTime();

            entDt = ParseDate(model.EntryDate);
            poDt = ParseDate(model.PODate);
            poAmdDt = ParseDate(model.POAmendDate);
            schDt = ParseDate(model.ScheduleDate);
            schAmdDt = ParseDate(model.SchAmendmentDate);
            schEffFromDt = ParseDate(model.SchEffFromDate);
            schEffToDate = ParseDate(model.SchEffTillDate);

            if (model.Mode == "UPDATE")
            {
                SqlParams.Add(new SqlParameter("@UpdatedBy", model.UpdatedBy));
            }
            SqlParams.Add(new SqlParameter("@EntryDate", entDt == default ? string.Empty : entDt));
            SqlParams.Add(new SqlParameter("@PODate", poDt == default ? string.Empty : poDt));
            SqlParams.Add(new SqlParameter("@POAmendDate", poAmdDt == default ? string.Empty : poAmdDt));
            SqlParams.Add(new SqlParameter("@ScheduleDate", schDt == default ? string.Empty : schDt));
            SqlParams.Add(new SqlParameter("@SchAmendDate", schAmdDt == default ? string.Empty : schAmdDt));
            SqlParams.Add(new SqlParameter("@ScheduleEffectiveFromDate", schEffFromDt == default ? string.Empty : schEffFromDt));
            SqlParams.Add(new SqlParameter("@ScheduleEffectiveTillDate", schEffToDate == default ? string.Empty : schEffToDate));
            
            if (model.Mode == "PSA")
                SqlParams.Add(new SqlParameter("@PSAmendYC", model.AmmYearCode));

            SqlParams.Add(new SqlParameter("@Flag", model.Mode == "COPY" ? "INSERT" : model.Mode));
            SqlParams.Add(new SqlParameter("@ID", model.ID));
            SqlParams.Add(new SqlParameter("@EntryID", model.EntryID));
            SqlParams.Add(new SqlParameter("@SchYearCode", model.YearCode));
            SqlParams.Add(new SqlParameter("@PONO", model.PONO ?? string.Empty));
            SqlParams.Add(new SqlParameter("@POYearCode", model.POYearCode));
            SqlParams.Add(new SqlParameter("@POAmenNo", model.POAmenNo));
            SqlParams.Add(new SqlParameter("@POAmendYearCode", model.POAmendYearCode));
            SqlParams.Add(new SqlParameter("@OrderTypeJWPurch", model.OrderTypeJWPurch ?? string.Empty));
            SqlParams.Add(new SqlParameter("@ItemService", model.ItemService ?? string.Empty));
            SqlParams.Add(new SqlParameter("@AccountCode", model.AccountCode));
            SqlParams.Add(new SqlParameter("@DeliveryAddress", model.DeliveryAddress ?? string.Empty));
            SqlParams.Add(new SqlParameter("@ScheduleNo", model.ScheduleNo ?? string.Empty));
            SqlParams.Add(new SqlParameter("@SchAmendNo", model.SchAmendmentNo));
            SqlParams.Add(new SqlParameter("@MRPNO", model.MRPNO));
            SqlParams.Add(new SqlParameter("@MRPentry_Id", model.MRPentryId));
            SqlParams.Add(new SqlParameter("@MRPNoYearCode", model.MRPNoYearCode));
            SqlParams.Add(new SqlParameter("@ModeOfTransport", model.ModeOfTransport ?? string.Empty));
            SqlParams.Add(new SqlParameter("@TentetiveConfirm", model.TentetiveConfirm ?? string.Empty));
            SqlParams.Add(new SqlParameter("@OrderPriority", model.OrderPriority ?? string.Empty));
            SqlParams.Add(new SqlParameter("@FirstMonthTentRatio", model.FirstMonthTentRatio));
            SqlParams.Add(new SqlParameter("@SecMonthTentRatio", model.SecMonthTentRatio));
            
            SqlParams.Add(new SqlParameter("@CC", model.CC));
            if (model.Mode == "PSA")
            {
                AmmAppDate = ParseDate(model.AmmApprovedDate);
                AppDate = ParseDate(model.ApprovedDate);
                SqlParams.Add(new SqlParameter("@SchAmmAppDate", AppDate == default ? string.Empty : AppDate));
                SqlParams.Add(new SqlParameter("@SchApprovalDate", AmmAppDate == default ? string.Empty : AmmAppDate));

                SqlParams.Add(new SqlParameter("@SchApproved", model.Approved ?? string.Empty));
                SqlParams.Add(new SqlParameter("@SchAppBy", model.Approvedby));
                SqlParams.Add(new SqlParameter("@SchAmmApprove", model.AmmApproved ?? string.Empty));
                SqlParams.Add(new SqlParameter("@SchAmmAppby", model.AmmApprovedby));
            }
            SqlParams.Add(new SqlParameter("@CreatedBY", model.CreatedBy));
            SqlParams.Add(new SqlParameter("@BillingAddress", model.BillingAddress ?? string.Empty));
            SqlParams.Add(new SqlParameter("@EntryByMachineName", model.EntryByMachineName ?? string.Empty)); 
            SqlParams.Add(new SqlParameter("@DTSSGrid", DTSSGrid));

            _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_PurchaseSchedule", SqlParams);
        }
        catch (Exception ex)
        {
            dynamic Error = new ExpandoObject();
            Error.Message = ex.Message;
            Error.Source = ex.Source;
        }

        return _ResponseResult;
    }

    private static PurchaseSubScheduleModel PrepareView(DataSet DS,string mode, ref PurchaseSubScheduleModel? model)
    {
        var ItemList = new List<PurchaseScheduleGrid>();
        DS.Tables[0].TableName = "SSMain";
        DS.Tables[1].TableName = "SSDetail";

        

        model.EntryID = Convert.ToInt32(DS.Tables[0].Rows[0]["EntryID"].ToString());
        model.YearCode = Convert.ToInt32(DS.Tables[0].Rows[0]["YearCode"].ToString());
        model.EntryDate = (DS.Tables[0].Rows[0]["EntryDate"].ToString());
        model.PONO = DS.Tables[0].Rows[0]["PONO"].ToString();
        model.POYearCode = Convert.ToInt32(DS.Tables[0].Rows[0]["POYearCode"].ToString());
        model.PODate = (DS.Tables[0].Rows[0]["PODate"].ToString());
        model.AccountCode = Convert.ToInt32(DS.Tables[0].Rows[0]["AccountCode"].ToString());
        model.DeliveryAddress = DS.Tables[0].Rows[0]["DeliveryAddress"].ToString();
        model.ScheduleNo = DS.Tables[0].Rows[0]["ScheduleNo"].ToString();
        model.ScheduleDate = (DS.Tables[0].Rows[0]["ScheduleDate"].ToString());
        model.MRPNO = Convert.ToInt32(DS.Tables[0].Rows[0]["MRPNO"].ToString());
        model.MRPNoYearCode = Convert.ToInt32(DS.Tables[0].Rows[0]["MRPNoYearCode"].ToString());
        model.MRPentryId = Convert.ToInt32(DS.Tables[0].Rows[0]["MRPentry_Id"].ToString());

        if (mode=="PSA")
        {

            model.SchAmendmentNo = Convert.ToInt32(DS.Tables[0].Rows[0]["SchAmendNo"].ToString()) + 1;
            model.Approved = DS.Tables[0].Rows[0]["SchApproved"].ToString();
            model.ApprovedDate = DS.Tables[0].Rows[0]["SchApprovalDate"].ToString();
            model.Approvedby = Convert.ToInt32(DS.Tables[0].Rows[0]["SchAppBy"].ToString());
            model.AmmApproved = DS.Tables[0].Rows[0]["SchAmendApprove"].ToString();
            model.AmmApprovedDate = DS.Tables[0].Rows[0]["SchAmendApproveDate"].ToString();
            model.AmmApprovedby = string.IsNullOrEmpty(DS.Tables[0].Rows[0]["SchAmendApproveBy"].ToString()) ? 0 : Convert.ToInt32(DS.Tables[0].Rows[0]["SchAmendApproveBy"].ToString());

        }
        else
            model.SchAmendmentNo = Convert.ToInt32(DS.Tables[0].Rows[0]["SchAmendNo"].ToString());
        model.SchAmendmentDate =  DS.Tables[0].Rows[0]["SchAmendDate"].ToString();
        model.ModeOfTransport = DS.Tables[0].Rows[0]["ModeOfTransport"].ToString();
        model.TentetiveConfirm = DS.Tables[0].Rows[0]["TentetiveConfirm"].ToString();
        model.OrderPriority = DS.Tables[0].Rows[0]["OrderPriority"].ToString();
        model.SchEffFromDate = (DS.Tables[0].Rows[0]["ScheduleEffectiveFromDate"].ToString());
        model.SchEffTillDate = (DS.Tables[0].Rows[0]["ScheduleEffectiveTillDate"].ToString());
        model.CC = DS.Tables[0].Rows[0]["CC"].ToString();

        model.CreatedBy = Convert.ToInt32(DS.Tables[0].Rows[0]["CreatedBy"].ToString());
        model.CreatedByName = DS.Tables[0].Rows[0]["CreatedByName"].ToString();
        model.CreatedOn = string.IsNullOrEmpty(DS.Tables[0].Rows[0]["CreatedOn"].ToString()) ? new DateTime() : Convert.ToDateTime(DS.Tables[0].Rows[0]["CreatedOn"]);



        if (!string.IsNullOrEmpty(DS.Tables[0].Rows[0]["UpdatedByName"].ToString()))
        {
            model.UpdatedByName = DS.Tables[0].Rows[0]["UpdatedByName"].ToString();

            model.UpdatedBy = string.IsNullOrEmpty(DS.Tables[0].Rows[0]["UpdatedBy"].ToString()) ? 0 : Convert.ToInt32(DS.Tables[0].Rows[0]["UpdatedBy"]);
            model.UpdatedOn = string.IsNullOrEmpty(DS.Tables[0].Rows[0]["UpdatedOn"].ToString()) ? new DateTime() : Convert.ToDateTime(DS.Tables[0].Rows[0]["UpdatedOn"]);
        }


        if (DS.Tables.Count != 0 && DS.Tables[1].Rows.Count > 0)
        {
            int cnt = 1;
            foreach (DataRow row in DS.Tables[1].Rows)
            {
                ItemList.Add(new PurchaseScheduleGrid
                {
                    SeqNo = cnt ++,
                    ItemCode = Convert.ToInt32(row["ItemCode"]),
                    PartCode = row["PartCode"].ToString(),
                    ItemName = row["Item_Name"].ToString(),
                    Rate = Convert.ToDecimal(row["Rate"]),
                    RateInOthCurr = Convert.ToDecimal(row["RateInOthCurr"]),
                    Unit = row["Unit"].ToString(),
                    AltUnit = row["AltUnit"].ToString(),
                    SchQty = Convert.ToDecimal(row["SchQty"]),
                    AltSchQty = Convert.ToDecimal(row["AltSchQty"]),
                    PendQty = Convert.ToDecimal(row["PendQty"]),
                    AltPendQty = Convert.ToDecimal(row["AltPendQty"]),
                    DeliveryDate = Convert.ToString(row["DeliveryDate"].ToString()),
                    ItemSize = row["ItemSize"].ToString(),
                    ItemColor = row["ItemColor"].ToString(),
                    OtherDetail = row["OtherDetail"].ToString(),
                    Remarks = row["Remarks"].ToString(),
                    schAmendNo = Convert.ToInt16(row["schAmendNo"].ToString()),
                    schAmendDate = Convert.ToString(row["schAmendDate"].ToString()),
                    SchAmendYear = Convert.ToInt16(row["SchAmendYear"].ToString()),
                    TentQtyFor1stMonth = Convert.ToDecimal(row["TentQtyFor1stMonth"].ToString()),
                    TentQtyFor2stMonth = Convert.ToDecimal(row["TentQtyFor2stMonth"].ToString()),
                });
            }
            model.PurchaseScheduleList = ItemList;
        }
        return model;
    }
}