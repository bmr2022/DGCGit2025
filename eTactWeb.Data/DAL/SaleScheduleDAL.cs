using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.Extensions.Configuration;
using Microsoft.SqlServer.Server;
using Newtonsoft.Json;
using System.Diagnostics.Eventing.Reader;
using System.Globalization;
using System.Reflection;
using static eTactWeb.DOM.Models.Common;
using static eTactWeb.Data.Common.CommonFunc;


namespace eTactWeb.Data.DAL;

internal class SaleScheduleDAL
{
    private readonly ConnectionStringService _connectionStringService;
    public SaleScheduleDAL(IConfiguration configuration, IDataLogic iDataLogic, ConnectionStringService connectionStringService)
    {
        //DBConnectionString = configuration.GetConnectionString("eTactDB");
        _connectionStringService = connectionStringService;
        DBConnectionString = _connectionStringService.GetConnectionString();
        _IDataLogic = iDataLogic;
    }

    public IDataLogic? _IDataLogic { get; }

    public IConfiguration? Configuration { get; }

    private string DBConnectionString { get; }

    internal async Task<DataSet> BindAllDropDown()
    {
        var oDataSet = new DataSet();

        try
        {
            var SqlParams = new List<dynamic>();
            SqlParams.Add(new SqlParameter("@Flag", "BINDALLDROPDOWN"));
            var ResponseResult = await _IDataLogic.ExecuteDataSet("SP_SaleSchedule", SqlParams);
            if (ResponseResult.Result != null && ResponseResult.StatusCode == HttpStatusCode.OK && ResponseResult.StatusText == "Success")
            {
                ResponseResult.Result.Tables[0].TableName = "BranchList";
                ResponseResult.Result.Tables[1].TableName = "AccountList";
                ResponseResult.Result.Tables[2].TableName = "CustomerOrderList";
                oDataSet = ResponseResult.Result;
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
    public async Task<SSDashboard> GetSOAmmCompleted(string toDt)
    {
        var Result = new SSDashboard();
        var oDataTable = new DataTable();

        try
        {
            using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
            {
                using (SqlCommand oCmd = new SqlCommand("SP_SaleSchedule", myConnection))
                {
                    DateTime now = DateTime.Now;
                    DateTime firstDayOfMonth = new DateTime(now.Year, now.Month, 1);
                    oCmd.CommandType = CommandType.StoredProcedure;
                    oCmd.Parameters.AddWithValue("@Flag", "SSAMMCompleted");
                    oCmd.Parameters.AddWithValue("@StartDate", firstDayOfMonth);
                    oCmd.Parameters.AddWithValue("@EndDate", toDt);
                    await myConnection.OpenAsync();
                    using SqlDataAdapter oDataAdapter = new SqlDataAdapter(oCmd);
                    oDataAdapter.Fill(oDataTable);

                    var oDT = oDataTable.DefaultView.ToTable(true, "EntryID", "SONO", "AccountCode", "CustomerName", "DeliveryAddress",
                         "SchApproved", "SODate", "SchNo", "SchDate", "SchYear", "SOYearCode", "CustomerOrderNo", "SOCloseDate", "SchCompleted", "SchAmendNo", "CreatedBYName",
                        "SchEffFromDate", "SchEffTillDate", "CreatedBy", "CreatedOn", "ApprovedBy");
                    oDT.TableName = "SaleScheduleComp";

                    Result.SSDashboard = CommonFunc.DataTableToList<SaleScheduleDashboard>(oDT);
                }
            }
        }
        catch (Exception ex)
        {
            dynamic Error = new ExpandoObject();
            Error.Message = ex.Message;
            if (ex.Source != null) Error.Source = ex.Source;
        }
        finally
        {
            oDataTable.Dispose();
        }
        return Result;
    }
    public async Task<ResponseResult> GetFormRights(int userId)
    {
        var _ResponseResult = new ResponseResult();
        try
        {
            var SqlParams = new List<dynamic>();
            SqlParams.Add(new SqlParameter("@Flag", "GetRights"));
            SqlParams.Add(new SqlParameter("@EmpId", userId));
            SqlParams.Add(new SqlParameter("@MainMenu", "Sale Schedule"));
           // SqlParams.Add(new SqlParameter("@SubMenu", "Sale Schedule"));

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

    public async Task<ResponseResult> GetFormRightsAmen(int userId)
    {
        var _ResponseResult = new ResponseResult();
        try
        {
            var SqlParams = new List<dynamic>();
            SqlParams.Add(new SqlParameter("@Flag", "GetRights"));
            SqlParams.Add(new SqlParameter("@EmpId", userId));
            SqlParams.Add(new SqlParameter("@MainMenu", "Sale Schedule Amendment"));
            // SqlParams.Add(new SqlParameter("@SubMenu", "Sale Schedule"));

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

    public async Task<ResponseResult> GetCurrency(string Flag)
    {
        var _ResponseResult = new ResponseResult();
        try
        {
            var SqlParams = new List<dynamic>();
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
    public async Task<ResponseResult> FillCustomer(string SchEffFromDate)
    {
        var _ResponseResult = new ResponseResult();
        try
        {
            var SqlParams = new List<dynamic>();
            SqlParams.Add(new SqlParameter("@Flag", "FILLCUSTOMER"));
            SqlParams.Add(new SqlParameter("@SchEffFromDate", ParseFormattedDate(SchEffFromDate)));

            _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_SaleSchedule", SqlParams);
        }
        catch (Exception ex)
        {
            dynamic Error = new ExpandoObject();
            Error.Message = ex.Message;
            Error.Source = ex.Source;
        }
        return _ResponseResult;
    }
    public async Task<ResponseResult> FillCustomerOrderNo(int AccountCode,string SchEffFromDate)
    {
        var _ResponseResult = new ResponseResult();
        try
        {
            var SqlParams = new List<dynamic>();
            SqlParams.Add(new SqlParameter("@Flag", "FILLCUSTOMER"));
            SqlParams.Add(new SqlParameter("@AccountCode", AccountCode));
            SqlParams.Add(new SqlParameter("@SchEffFromDate", ParseFormattedDate(SchEffFromDate)));

            _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_SaleSchedule", SqlParams);
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
    public async Task<ResponseResult> GetAllData(int ID, int YC)
    {
        var _ResponseResult = new ResponseResult();
        try
        {
            var SqlParams = new List<dynamic>();
            SqlParams.Add(new SqlParameter("@Flag", "GetAllMonthlyData"));
            SqlParams.Add(new SqlParameter("@EntryID", ID));
            SqlParams.Add(new SqlParameter("@YearCode", YC));

            _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_SaleSchedule", SqlParams);
        }
        catch (Exception ex)
        {
            dynamic Error = new ExpandoObject();
            Error.Message = ex.Message;
            Error.Source = ex.Source;
        }
        return _ResponseResult;
    }

    public async Task<ResponseResult> GetItemCode(string PartCode)
    {
        var _ResponseResult = new ResponseResult();
        try
        {
            var SqlParams = new List<dynamic>();
            SqlParams.Add(new SqlParameter("@Flag", "GetItemCode"));
            SqlParams.Add(new SqlParameter("@PartCode", PartCode));

            _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_SaleSchedule", SqlParams);

        }
        catch (Exception ex)
        {
            dynamic Error = new ExpandoObject();
            Error.Message = ex.Message;
            Error.Source = ex.Source;
        }
        return _ResponseResult;
    }
    public async Task<SSDashboard> GetAmmSearchData(SSDashboard model)
    {
        var oDataTable = new DataTable();

        try
        {
            using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
            {
                using (SqlCommand oCmd = new SqlCommand("SP_SaleSchedule", myConnection))
                {
                    DateTime StartDate = new DateTime();
                    DateTime EndDate = new DateTime();
                    StartDate = DateTime.ParseExact(model.FromDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    EndDate = DateTime.ParseExact(model.ToDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    oCmd.CommandType = CommandType.StoredProcedure;
                    oCmd.Parameters.AddWithValue("@Flag", "SSAMMDASHBOARD");
                    oCmd.Parameters.AddWithValue("@CustomerName", model.CustomerName);
                    oCmd.Parameters.AddWithValue("@CustomerOrderNo", model.CustomerOrderNo);
                    oCmd.Parameters.AddWithValue("@SONo", model.SONO);
                    oCmd.Parameters.AddWithValue("@ItemName", model.ItemName);
                    oCmd.Parameters.AddWithValue("@StartDate", StartDate.ToString("yyyy/MM/dd"));
                    oCmd.Parameters.AddWithValue("@EndDate", EndDate.ToString("yyyy/MM/dd"));
                    await myConnection.OpenAsync();
                    using SqlDataAdapter oDataAdapter = new SqlDataAdapter(oCmd);
                    oDataAdapter.Fill(oDataTable);

                    var oDT = oDataTable.DefaultView.ToTable(true, "SONO", "CustomerName", "AccountCode", "SODate", "CustomerOrderNo", "SOYearCode", "SchEffFromDate", "SchAmendNo",
                    "SchApproved", "SchAmendApprove", "SchClosed", "SchCompleted", "EntryID", "DeliveryAddress", "SchNo", "SchDate", "SchYear",
                    "SOCloseDate", "CreatedBYName", "CreatedBy", "CreatedOn", "ApprovedBy", "SchEffTillDate");

                    oDT.TableName = "SaleSchedule";

                    model.SSDashboard = CommonFunc.DataTableToList<SaleScheduleDashboard>(oDT);
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
        return model;
    }
    public async Task<SSDashboard> GetUpdAmmData(SSDashboard model)
    {
        var oDataTable = new DataTable();

        try
        {
            using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
            {
                using (SqlCommand oCmd = new SqlCommand("SP_SaleSchedule", myConnection))
                {
                    DateTime StartDate = new DateTime();
                    DateTime EndDate = new DateTime();
                    StartDate = DateTime.ParseExact(model.FromDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    EndDate = DateTime.ParseExact(model.ToDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    oCmd.CommandType = CommandType.StoredProcedure;
                    oCmd.Parameters.AddWithValue("@Flag", "UPDAMMSSDASHBOARD");
                    oCmd.Parameters.AddWithValue("@CustomerName", model.CustomerName);
                    oCmd.Parameters.AddWithValue("@CustomerOrderNo", model.CustomerOrderNo);
                    oCmd.Parameters.AddWithValue("@SONo", model.SONO);
                    oCmd.Parameters.AddWithValue("@ItemName", model.ItemName);
                    oCmd.Parameters.AddWithValue("@StartDate", StartDate.ToString("yyyy/MM/dd"));
                    oCmd.Parameters.AddWithValue("@EndDate", EndDate.ToString("yyyy/MM/dd"));
                    await myConnection.OpenAsync();
                    using SqlDataAdapter oDataAdapter = new SqlDataAdapter(oCmd);
                    oDataAdapter.Fill(oDataTable);

                    var oDT = oDataTable.DefaultView.ToTable(true, "EntryID", "SONO", "AccountCode", "CustomerName", "CustomerOrderNo", "DeliveryAddress",
            "SchApproved", "SchAmendApprove", "SODate", "SchNo", "SchDate", "SchYear", "CreatedBYName", "SOYearCode", "SchEffFromDate", "SchEffTillDate", "SOCloseDate",
            "SchCompleted", "SchClosed", "SchAmendNo", "CreatedBy", "CreatedOn", "ApprovedBy");

                    oDT.TableName = "SaleSchedule";

                    model.SSDashboard = CommonFunc.DataTableToList<SaleScheduleDashboard>(oDT);
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
        return model;
    }
    internal async Task<ResponseResult> DeleteByID(int ID, int YC)
    {
        var _ResponseResult = new ResponseResult();
        try
        {
            var SqlParams = new List<dynamic>();
            SqlParams.Add(new SqlParameter("@Flag", "DELETEBYID"));
            SqlParams.Add(new SqlParameter("@ID", ID));
            SqlParams.Add(new SqlParameter("@YearCode", YC));
            SqlParams.Add(new SqlParameter("@EntryByMachineName", "AB"));
            SqlParams.Add(new SqlParameter("@CreatedBy", "1"));
            _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_SaleSchedule", SqlParams);
        }
        catch (Exception ex)
        {
            dynamic Error = new ExpandoObject();
            Error.Message = ex.Message;
            Error.Source = ex.Source;
        }

        return _ResponseResult;
    }

    internal async Task<ResponseResult> GetDashboardData()
    {
        var _ResponseResult = new ResponseResult();
        try
        {
            DateTime now = DateTime.Parse(DateTime.Now.ToString("dd/MMM/yyyy"));
            DateTime firstDayOfMonth = new DateTime(now.Year, now.Month, 1);
            var SqlParams = new List<dynamic>();
            SqlParams.Add(new SqlParameter("@Flag", "DASHBOARD"));
            SqlParams.Add(new SqlParameter("@StartDate", firstDayOfMonth));
            SqlParams.Add(new SqlParameter("@EndDate", now));

            _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_SaleSchedule", SqlParams);
        }
        catch (Exception ex)
        {
            dynamic Error = new ExpandoObject();
            Error.Message = ex.Message;
            Error.Source = ex.Source;
        }
        return _ResponseResult;
    }
    internal async Task<object> GetAmmStatus(int EntryID, int YearCode)
    {
        object AmmStatus = 0;

        try
        {
            using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
            {
                using (SqlCommand oCmd = new SqlCommand("SP_SaleSchedule", myConnection))
                {
                    oCmd.CommandType = CommandType.StoredProcedure;

                    oCmd.Parameters.AddWithValue("@Flag", "CHECKSCHAMMSTATUS");
                    oCmd.Parameters.AddWithValue("@EntryID", EntryID);
                    oCmd.Parameters.AddWithValue("@YearCode", YearCode);

                    await myConnection.OpenAsync();
                    AmmStatus = await oCmd.ExecuteScalarAsync();
                }
            }
        }
        catch (Exception ex)
        {
            dynamic Error = new ExpandoObject();
            Error.Message = ex.Message;
            Error.Source = ex.Source;
        }
        return AmmStatus;
    }


    public async Task<ResponseResult> GetAmmDashboardData(string ToDate)
    {
        var _ResponseResult = new ResponseResult();
        try
        {
            DateTime now = DateTime.Now;
            DateTime firstDayOfMonth = new DateTime(now.Year, now.Month, 1);
            var SqlParams = new List<dynamic>();
            SqlParams.Add(new SqlParameter("@Flag", "SSAMMDASHBOARD"));
            SqlParams.Add(new SqlParameter("@StartDate", firstDayOfMonth));
            SqlParams.Add(new SqlParameter("@EndDate", ToDate));

            _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_SaleSchedule", SqlParams);
        }
        catch (Exception ex)
        {
            dynamic Error = new ExpandoObject();
            Error.Message = ex.Message;
            Error.Source = ex.Source;
        }
        return _ResponseResult;
    }

    internal async Task<ResponseResult> GetSearchData(SSDashboard model)
    {
        var _ResponseResult = new ResponseResult();
        try
        {
            string flag = "";
            if (model.Mode == "SSAMMCOMPLETED")
            {
                flag = "SSAMMCompleted";
            }
            else
            {
                flag = "SEARCH";
            }
            DateTime StartDate = new DateTime();
            DateTime EndDate = new DateTime();
            StartDate = DateTime.ParseExact(model.FromDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            EndDate = DateTime.ParseExact(model.ToDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            EndDate = EndDate.AddDays(1);
            var SqlParams = new List<dynamic>();
            SqlParams.Add(new SqlParameter("@Flag", flag));
            SqlParams.Add(new SqlParameter("@CustomerName", model.CustomerName));
            SqlParams.Add(new SqlParameter("@CustomerOrderNo", model.CustOrderNo));
            SqlParams.Add(new SqlParameter("@SONo", model.SONO));
            SqlParams.Add(new SqlParameter("@ItemName", model.ItemName));
            //SqlParams.Add(new SqlParameter("@StartDate", model.FromDate));
            //SqlParams.Add(new SqlParameter("@EndDate", model.ToDate));
            SqlParams.Add(new SqlParameter("@StartDate", StartDate.ToString("yyyy/MM/dd")));
            SqlParams.Add(new SqlParameter("@EndDate", EndDate.ToString("yyyy/MM/dd")));

            _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_SaleSchedule", SqlParams);
        }
        catch (Exception ex)
        {
            dynamic Error = new ExpandoObject();
            Error.Message = ex.Message;
            Error.Source = ex.Source;
        }
        return _ResponseResult;
    }
    internal async Task<ResponseResult> NewEntryId(int YearCode)
    {
        var _ResponseResult = new ResponseResult();
        try
        {
            var SqlParams = new List<dynamic>();
            SqlParams.Add(new SqlParameter("@Flag", "NewEntryId"));
            SqlParams.Add(new SqlParameter("@YearCode", YearCode));

            _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_SaleSchedule", SqlParams);
        }
        catch (Exception ex)
        {
            dynamic Error = new ExpandoObject();
            Error.Message = ex.Message;
            Error.Source = ex.Source;
        }
        return _ResponseResult;
    }

    internal async Task<ResponseResult> NewAmmEntryId()
    {
        var _ResponseResult = new ResponseResult();
        try
        {
            var SqlParams = new List<dynamic>();
            SqlParams.Add(new SqlParameter("@Flag", "GetNewAmmEntry"));

            _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_SaleSchedule", SqlParams);
        }
        catch (Exception ex)
        {
            dynamic Error = new ExpandoObject();
            Error.Message = ex.Message;
            Error.Source = ex.Source;
        }
        return _ResponseResult;
    }
    internal async Task<ResponseResult> GetAltUnitQty(int ItemCode, float AltSchQty, float UnitQty)
    {
        var _ResponseResult = new ResponseResult();
        try
        {
            var SqlParams = new List<dynamic>();
            SqlParams.Add(new SqlParameter("@Itemcode", ItemCode));
            SqlParams.Add(new SqlParameter("@ALtQty", AltSchQty));
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

    internal async Task<string> GetSODATA(int AccountCode, int SONO, int Year)
    {
        var JsonString = string.Empty;
        try
        {
            var SqlParams = new List<dynamic>();

            SqlParams.Add(new SqlParameter("@Flag", "SODATA"));
            SqlParams.Add(new SqlParameter("@AccountCode", AccountCode));
            SqlParams.Add(new SqlParameter("@SONO", SONO));
            SqlParams.Add(new SqlParameter("@YearCode", Year));
            var ResponseResult = await _IDataLogic.ExecuteDataTable("SP_SaleSchedule", SqlParams);
            JsonString = JsonConvert.SerializeObject(ResponseResult);
        }
        catch (Exception ex)
        {
            dynamic Error = new ExpandoObject();
            Error.Message = ex.Message;
            Error.Source = ex.Source;
        }
        return JsonString;
    }

    internal async Task<string> GetSOItem(object AccountCode, object SONO, object Year, int ItemCode)
    {
        var JsonString = string.Empty;
        try
        {
            var SqlParams = new List<dynamic>();

            SqlParams.Add(new SqlParameter("@Flag", "SOITEM"));
            SqlParams.Add(new SqlParameter("@AccountCode", AccountCode));
            SqlParams.Add(new SqlParameter("@SONO", SONO));
            SqlParams.Add(new SqlParameter("@YearCode", Year));
            SqlParams.Add(new SqlParameter("@ID", ItemCode));
            var ResponseResult = await _IDataLogic.ExecuteDataTable("SP_SaleSchedule", SqlParams);
            JsonString = JsonConvert.SerializeObject(ResponseResult);
        }
        catch (Exception ex)
        {
            dynamic Error = new ExpandoObject();
            Error.Message = ex.Message;
            Error.Source = ex.Source;
        }

        return JsonString;
    }

    internal async Task<string> GetSONO(int AccountCode)
    {
        var JsonString = string.Empty;
        try
        {
            var SqlParams = new List<dynamic>();
            SqlParams.Add(new SqlParameter("@Flag", "SONOBYACCOUNT"));
            SqlParams.Add(new SqlParameter("@AccountCode", AccountCode));
            var ResponseResult = await _IDataLogic.ExecuteDataTable("SP_SaleSchedule", SqlParams);
            JsonString = JsonConvert.SerializeObject(ResponseResult);
        }
        catch (Exception ex)
        {
            dynamic Error = new ExpandoObject();
            Error.Message = ex.Message;
            Error.Source = ex.Source;
        }

        return JsonString;
    }

    internal async Task<string> GetSOYear(int AccountCode, int SONO)
    {
        var JsonString = string.Empty;
        try
        {
            var SqlParams = new List<dynamic>();

            SqlParams.Add(new SqlParameter("@Flag", "GETSOYEAR"));
            SqlParams.Add(new SqlParameter("@AccountCode", AccountCode));
            SqlParams.Add(new SqlParameter("@SONO", SONO));
            var ResponseResult = await _IDataLogic.ExecuteDataTable("SP_SaleSchedule", SqlParams);
            JsonString = JsonConvert.SerializeObject(ResponseResult);
        }
        catch (Exception ex)
        {
            dynamic Error = new ExpandoObject();
            Error.Message = ex.Message;
            Error.Source = ex.Source;
        }

        return JsonString;
    }

    internal async Task<SaleSubScheduleModel> GetViewByID(int ID, int YearCode, string Mode)
    {
        var model = new SaleSubScheduleModel();
        try
        {
            var SqlParams = new List<dynamic>();

            SqlParams.Add(new SqlParameter("@Flag", "VIEWBYID"));
            SqlParams.Add(new SqlParameter("@ID", ID));
            SqlParams.Add(new SqlParameter("@YearCode", YearCode));

            var ResponseResult = await _IDataLogic.ExecuteDataSet("SP_SaleSchedule", SqlParams);

            if (ResponseResult.Result != null && ResponseResult.StatusCode == HttpStatusCode.OK && ResponseResult.StatusText == "Success")
            {
                PrepareView(ResponseResult.Result, ref model, Mode);
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
    //public static DateTime ParseDate(string dateString)
    //{
    //    if (string.IsNullOrEmpty(dateString))
    //    {
    //        return default;
    //    }

    //    if (DateTime.TryParseExact(dateString, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate))
    //    {
    //        return parsedDate;
    //    }
    //    else
    //    {
    //        return DateTime.Parse(dateString);
    //    }

    //    //    throw new FormatException("Invalid date format. Expected format: dd/MM/yyyy");

    //}

    public static DateTime ParseDate(string dateString)
    {
        //if (string.IsNullOrEmpty(dateString))
        //{
        //    return default;
        //}

        if (DateTime.TryParseExact(dateString, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate))
        {
            string parsedatestring = parsedDate.ToString("yyyy/MM/dd");
            DateTime outputDateString = DateTime.ParseExact(parsedatestring, "yyyy/MM/dd", CultureInfo.InvariantCulture);
            return outputDateString;
        }
        //if (DateTime.TryParseExact(dateString, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate))
        //{
        //    return parsedDate;
        //}
        //else
        //{
        //    if (DateTime.TryParseExact(dateString, "dd-MMM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDateWithMonth))
        //    {
        //        return parsedDateWithMonth;
        //    }
        else
        {
            return DateTime.Parse(dateString);
        }
        //}
    }

    internal async Task<ResponseResult> SaveSaleSchedule(SaleSubScheduleModel model, DataTable DTSSGrid)
    {
        var _ResponseResult = new ResponseResult();
        try
        {
            DateTime EntryDt = new DateTime();
            DateTime SoDt = new DateTime();
            DateTime SchDt = new DateTime();
            DateTime SchAmmDt = new DateTime();
            DateTime SchAppDate = new DateTime();
            DateTime SchAmendAppDate = new DateTime();
            DateTime SchEffFromDt = new DateTime();
            DateTime SchEffTillDt = new DateTime();
            DateTime SoCloseDt = new DateTime();

            EntryDt = DateTime.Parse(ConvertToDesiredFormat(model.EntryDate));
            SoDt = DateTime.Parse((model.SODate));
            SchDt = DateTime.Parse(ConvertToDesiredFormat(model.ScheduleDate));
            SchAmmDt = DateTime.Parse(ConvertToDesiredFormat(model.SchAmendmentDate));
            SoCloseDt = DateTime.Parse(ConvertToDesiredFormat(model.SOCloseDate));

            if (model.SchApprovalDate == null)
            {
                SchAppDate = DateTime.Now;
            }
            else
            {
                SchAppDate = ParseDate(model.SchApprovalDate);
            }

            if (model.SchAmendApprovalDate == null)
            {
                SchAmendAppDate = DateTime.Now;
            }
            else
            {
                SchAmendAppDate = ParseDate(model.SchAmendApprovalDate);
            }

            //SchAmendAppDate = DateTime.Parse( ConvertToDesiredFormat(model.SchAmendApprovalDate));
            SchEffFromDt = DateTime.Parse((model.SchEffFromDate));
            SchEffTillDt = DateTime.Parse((model.SchEffTillDate));
            var SqlParams = new List<dynamic>();
            SqlParams.Add(new SqlParameter("@Flag", model.Mode));
            SqlParams.Add(new SqlParameter("@ID", model.ID));
            SqlParams.Add(new SqlParameter("@EntryID", model.EntryID));
            SqlParams.Add(new SqlParameter("@YearCode", model.YearCode));
            SqlParams.Add(new SqlParameter("@EntryDate", EntryDt == default ? DateTime.Now.ToString("yyyy/MM/dd") : EntryDt));
            SqlParams.Add(new SqlParameter("@SONO", model.SONO));
            SqlParams.Add(new SqlParameter("@CustomerOrderNo", model.CustomerOrderNo));
            SqlParams.Add(new SqlParameter("@SOYearCode", model.SOYearCode));
            SqlParams.Add(new SqlParameter("@SODate", SoDt == default ? DateTime.Now.ToString("yyyy/MM/dd") : SoDt));
            SqlParams.Add(new SqlParameter("@SOCloseDate", SoCloseDt == default ? DateTime.Now.ToString("yyyy/MM/dd") : SoCloseDt));
            SqlParams.Add(new SqlParameter("@AccountCode", model.AccountCode));
            SqlParams.Add(new SqlParameter("@DeliveryAddress", model.DeliveryAddress));
            SqlParams.Add(new SqlParameter("@ScheduleNo", model.ScheduleNo));

            SqlParams.Add(new SqlParameter("@ScheduleDate", SchDt == default ? DateTime.Now.ToString("yyyy/MM/dd") : SchDt.ToString("dd/MMM/yyyy")));
            if (model.Mode == "SSA")
            {
                SqlParams.Add(new SqlParameter("@SSAmmYearCode", model.AmmYearCode));
                SqlParams.Add(new SqlParameter("@SchApproved", model.SchApproved));
                SqlParams.Add(new SqlParameter("@SchAppBy", model.SchAppBy));
                SqlParams.Add(new SqlParameter("@SchApprovalDate", SchAppDate == default ? DateTime.Now.ToString("yyyy/MM/dd") : SchAppDate));
                SqlParams.Add(new SqlParameter("@SchAmendApproved", model.SchAmendApproved));
                SqlParams.Add(new SqlParameter("@SchAmendApprovedBy", model.SchAmendAppBy));
                SqlParams.Add(new SqlParameter("@SchAmendApprovalDate", SchAmendAppDate == default ? DateTime.Now.ToString("yyyy/MM/dd") : SchAmendAppDate));
                SqlParams.Add(new SqlParameter("@UpdatedBy", model.UpdatedBy));
            }
            else
            {
                SqlParams.Add(new SqlParameter("@SSAmmYearCode", "0"));
                SqlParams.Add(new SqlParameter("@SchApproved", "N"));
                SqlParams.Add(new SqlParameter("@SchAppBy", ""));
                SqlParams.Add(new SqlParameter("@SchApprovalDate", "1900/04/01"));
                SqlParams.Add(new SqlParameter("@SchAmendApproved", "N"));
                SqlParams.Add(new SqlParameter("@SchAmendApprovedBy", ""));
                SqlParams.Add(new SqlParameter("@SchAmendApprovalDate", "1900/04/01"));

            }
            SqlParams.Add(new SqlParameter("@SchAmendNo", model.SchAmendmentNo));
            SqlParams.Add(new SqlParameter("@SchAmendDate", SchAmmDt == default ? DateTime.Now.ToString("yyyy/MM/dd") : SchAmmDt));
            SqlParams.Add(new SqlParameter("@EntryByMachineName", model.EnterByMachineName));

            SqlParams.Add(new SqlParameter("@ModeOfTransport", model.ModeOfTransport??""));
            SqlParams.Add(new SqlParameter("@TentetiveConfirm", model.TentetiveConfirm));
            SqlParams.Add(new SqlParameter("@OrderPriority", model.OrderPriority));
            SqlParams.Add(new SqlParameter("@SchEffFromDate", SchEffFromDt == default ? DateTime.Now.ToString("yyyy/MM/dd") : SchEffFromDt));
            SqlParams.Add(new SqlParameter("@SchEffTillDate", SchEffTillDt == default ? DateTime.Now.ToString("yyyy/MM/dd") : SchEffTillDt.ToString("dd/MMM/yyyy")));

            SqlParams.Add(new SqlParameter("@CC", model.CC));
            SqlParams.Add(new SqlParameter("@DTSSGrid", DTSSGrid));
            SqlParams.Add(new SqlParameter("@CreatedBY", model.CreatedBy));
            SqlParams.Add(new SqlParameter("@ManaulMonthSplit", model.ManualMonthSplit??""));
            if (model.Mode == "UPDATE")
            {
                SqlParams.Add(new SqlParameter("@UpdatedBy", model.UpdatedBy));
            }

            _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_SaleSchedule", SqlParams);
        }
        catch (Exception ex)
        {
            dynamic Error = new ExpandoObject();
            Error.Message = ex.Message;
            Error.Source = ex.Source;
        }

        return _ResponseResult;
    }



    private static SaleSubScheduleModel PrepareView(DataSet DS, ref SaleSubScheduleModel? model, string Mode)
    {
        var ItemList = new List<SaleScheduleGrid>();
        DS.Tables[0].TableName = "SSMain";
        DS.Tables[1].TableName = "SSDetail";

        int cnt = 1;
        if (Mode == "SSC")
        {
            model.AmmEntryId = Convert.ToInt32(DS.Tables[0].Rows[0]["EntryID"].ToString());
            model.AmmYearCode = Convert.ToInt32(DS.Tables[0].Rows[0]["YearCode"].ToString());
            model.EntryID = Convert.ToInt32(DS.Tables[0].Rows[0]["SCHEntryID"].ToString());
            model.YearCode = Convert.ToInt32(DS.Tables[0].Rows[0]["SchYearCode"].ToString());
        }
        else
        {
            model.EntryID = Convert.ToInt32(DS.Tables[0].Rows[0]["EntryID"].ToString());
            model.YearCode = Convert.ToInt32(DS.Tables[0].Rows[0]["YearCode"].ToString());
        }
        var formattedDate = DS.Tables[0].Rows[0]["SODate"].ToString();

        model.EntryDate = DS.Tables[0].Rows[0]["EntryDate"].ToString();
        model.SONO = Convert.ToInt32(DS.Tables[0].Rows[0]["SONO"].ToString());
        model.CustomerOrderNo = DS.Tables[0].Rows[0]["CustomerOrderNo"].ToString();
        model.SOYearCode = Convert.ToInt32(DS.Tables[0].Rows[0]["SOYearCode"].ToString());
        model.SODate = formattedDate.Replace("-", "/");
        model.SOCloseDate = DS.Tables[0].Rows[0]["SOCloseDate"].ToString();
        model.SchEffFromDate = DS.Tables[0].Rows[0]["ScheduleEffectiveFromDate"].ToString();
        model.SchEffTillDate = DS.Tables[0].Rows[0]["ScheduleEffectiveTillDate"].ToString();
        model.AccountCode = Convert.ToInt32(DS.Tables[0].Rows[0]["Accountcode"].ToString());
        model.DeliveryAddress = DS.Tables[0].Rows[0]["DeliveryAddress"].ToString();
        model.ScheduleNo = DS.Tables[0].Rows[0]["ScheduleNo"].ToString();
        model.ScheduleDate = DS.Tables[0].Rows[0]["ScheduleDate"].ToString();
        model.SchAmendmentDate = DS.Tables[0].Rows[0]["SchAmendDate"].ToString();
        model.ModeOfTransport = DS.Tables[0].Rows[0]["ModeOfTransport"].ToString();
        model.TentetiveConfirm = DS.Tables[0].Rows[0]["TentetiveConfirm"].ToString();
        model.OrderPriority = DS.Tables[0].Rows[0]["OrderPriority"].ToString();
        model.SchEffFromDate = DS.Tables[0].Rows[0]["ScheduleEffectiveFromDate"].ToString();
        model.SchEffTillDate = DS.Tables[0].Rows[0]["ScheduleEffectiveTillDate"].ToString();
        model.SOCloseDate = DS.Tables[0].Rows[0]["SOCloseDate"].ToString();
        model.ManualMonthSplit = DS.Tables[0].Rows[0]["ManualMonthSplit"].ToString();
        model.CC = DS.Tables[0].Rows[0]["CC"].ToString();
        model.CreatedBy = Convert.ToInt32(DS.Tables[0].Rows[0]["CreatedBy"].ToString());
        model.CreatedOn = string.IsNullOrEmpty(DS.Tables[0].Rows[0]["CreatedOn"].ToString()) ? new DateTime() : Convert.ToDateTime(DS.Tables[0].Rows[0]["CreatedOn"].ToString());
        model.CreatedByName = DS.Tables[0].Rows[0]["CreatedBYName"].ToString();
        if (Mode == "SSA")
        {
            model.SchApproved = DS.Tables[0].Rows[0]["SchApproved"].ToString();
            model.SchAppBy = DS.Tables[0].Rows[0]["SchAppBy"].ToString();
            model.SchApprovalDate = DS.Tables[0].Rows[0]["SchApprovalDate"].ToString();
            model.SchAmendApproved = DS.Tables[0].Rows[0]["SchAmendApprove"].ToString();
            model.SchAmendAppBy = DS.Tables[0].Rows[0]["SchAmendAppBy"].ToString();
            model.SchAmendApprovalDate = DS.Tables[0].Rows[0]["SchAmendAppDate"].ToString();
            model.SchAmendmentNo = Convert.ToInt32(DS.Tables[0].Rows[0]["SchAmendNo"].ToString()) + 1;
        }
        else if (Mode == "U")
        {
            if (DS.Tables[0].Rows[0]["UpdatedBy"].ToString() != "")
            {
                model.UpdatedBy = Convert.ToInt32(DS.Tables[0].Rows[0]["UpdatedBy"].ToString());
                model.UpdatedOn = string.IsNullOrEmpty(DS.Tables[0].Rows[0]["UpdatedOn"].ToString()) ? new DateTime() : Convert.ToDateTime(DS.Tables[0].Rows[0]["UpdatedOn"].ToString());
                model.UpdatedByName = DS.Tables[0].Rows[0]["UpdatedByName"].ToString();
            }
        }
        else
        {
            //do nothing
        }

        if (DS.Tables.Count != 0 && DS.Tables[1].Rows.Count > 0)
        {
            foreach (DataRow row in DS.Tables[1].Rows)
            {

                ItemList.Add(new SaleScheduleGrid
                {
                    SeqNo = cnt++,
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
                    DeliveryDate = row["DeliveryDate"].ToString(),
                    ItemSize = row["ItemSize"].ToString(),
                    ItemColor = row["ItemColor"].ToString(),
                    OtherDetail = row["OtherDetail"].ToString(),
                    Remarks = row["Remarks"].ToString(),
                });
            }
            model.SaleScheduleList = ItemList;
        }

        return model;
    }
    internal async Task<SaleSubScheduleModel> GetViewSSCcompletedByID(int ID, int YC, int SONO, string Mode)
    {
        var model = new SaleSubScheduleModel();
        try
        {
            var SqlParams = new List<dynamic>();
            SqlParams.Add(new SqlParameter("@Flag", "ViewSSCompleted"));
            SqlParams.Add(new SqlParameter("@EntryID", ID));
            SqlParams.Add(new SqlParameter("@YearCode", YC));
            SqlParams.Add(new SqlParameter("@SONO", SONO));

            var ResponseResult = await _IDataLogic.ExecuteDataSet("SP_SaleSchedule", SqlParams);

            if (ResponseResult.Result != null && ResponseResult.StatusCode == HttpStatusCode.OK && ResponseResult.StatusText == "Success")
            {
                PrepareView(ResponseResult.Result, ref model, Mode);
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
}
