using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System.Data;
using System.Globalization;
using static eTactWeb.DOM.Models.Common;

namespace eTactwebHR.Controllers;
[Authorize]
public class HRAttendanceController : Controller
{
    private readonly IWebHostEnvironment _IWebHostEnvironment;
    private readonly IConfiguration iconfiguration;
    private readonly ICompositeViewEngine _viewEngine;
    private readonly IMemoryCache _MemoryCache;
    private readonly ConnectionStringService _connectionStringService;
    public HRAttendanceController(IHRAttendance iHRAttendance, IDataLogic iDataLogic, ILogger<HRAttendanceModel> logger, EncryptDecrypt encryptDecrypt, IMemoryCacheService iMemoryCacheService, IWebHostEnvironment iWebHostEnvironment, IConfiguration configuration, ICompositeViewEngine viewEngine, IMemoryCache iMemoryCache, ConnectionStringService connectionStringService)
    {
        IHRAttendance = iHRAttendance;
        IDataLogic = iDataLogic;
        _Logger = logger;
        EncryptDecrypt = encryptDecrypt;
        CI = new CultureInfo("en-GB");
        _IWebHostEnvironment = iWebHostEnvironment;
        iconfiguration = configuration;
        _MemoryCache = iMemoryCache;
        _viewEngine = viewEngine;
        _connectionStringService = connectionStringService;
    }

    public ILogger<HRAttendanceModel> _Logger { get; set; }
    public CultureInfo CI { get; private set; }
    public EncryptDecrypt EncryptDecrypt { get; private set; }
    public IDataLogic IDataLogic { get; private set; }
    public IHRAttendance IHRAttendance { get; set; }
    public async Task<IActionResult> HRAttendanceList(string? FromDate, string? ToDate)
    {
        var _List = new List<TextValue>();
        HRAListDataModel model = new HRAListDataModel();
        FromDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).ToString("dd/MM/yyyy").Replace("-", "/");

        ToDate = string.IsNullOrEmpty(model.ToDate) ? DateTime.Now.ToString("dd/MM/yyyy") : model.ToDate;
        var MainModel = new HRAListDataModel();
        //var MainModel = await IHRAttendance.GetHRAListData(string.Empty, string.Empty, string.Empty, model);

        DateTime now = DateTime.Now;
        DateTime firstDayOfMonth = new DateTime(now.Year, now.Month, 1);
        DateTime today = DateTime.Now;
        var commonparams = new Dictionary<string, object>()
        {
            { "@Fromdate", firstDayOfMonth },
            { "@ToDate", today }
        };
        MainModel = await BindPBList(MainModel, commonparams);
        MainModel.FromDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).ToString("dd/MM/yyyy").Replace("-", "/");
        MainModel.ToDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day).ToString("dd/MM/yyyy").Replace("-", "/");// Last day in January next year

        return View(MainModel);
    }
    public async Task<IActionResult> GetSearchHRAListData(HRAListDataModel model)
    {
        var _List = new List<TextValue>();
        DateTime now = DateTime.Now;
        DateTime firstDayOfMonth = new DateTime(now.Year, now.Month, 1);
        DateTime today = DateTime.Now;

        string fromDate = (model.FromDate);
        string toDate = (model.ToDate);
        var MainModel = new HRAListDataModel();
       // var MainModel = await IHRAttendance.GetHRAttendanceListData("DisplayPendingData", MRNType, model.DashboardType ?? "SUMMARY", fromDate, toDate, model);

        //fromDate = DateTime.ParseExact(model.FromDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
        //toDate = DateTime.ParseExact(model.ToDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);

        var commonparams = new Dictionary<string, object>()
        {
            { "@Fromdate", model.FromDate != null ? fromDate : firstDayOfMonth },
            { "@ToDate", model.ToDate != null ? toDate : today }
        };
        MainModel = await BindPBList(MainModel, commonparams);
        MainModel.FromDate = new DateTime(DateTime.Today.Year, 4, 1).ToString("dd/MM/yyyy").Replace("-", "/");
        MainModel.ToDate = new DateTime(DateTime.Today.Year + 1, 3, 31).ToString("dd/MM/yyyy").Replace("-", "/");// Last day in January next year

        //MainModel.DashboardType = "Summary";
        return PartialView("_HRAListDataGrid", MainModel);
    }
    public async Task<IActionResult> GetHRAListDropdownData(HRAListDataModel model)
    {
        var _List = new List<TextValue>();
        DateTime now = DateTime.Now;
        DateTime firstDayOfMonth = new DateTime(now.Year, now.Month, 1);
        DateTime today = DateTime.Now;
        var MainModel = model;
        DateTime fromDate = DateTime.ParseExact(model.FromDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
        DateTime toDate = DateTime.ParseExact(model.ToDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);

        var commonparams = new Dictionary<string, object>()
        {
            { "@Fromdate", model.FromDate != null ? fromDate : firstDayOfMonth },
            { "@ToDate", model.ToDate != null ? toDate : today }
        };
        MainModel = await BindPBList(MainModel, commonparams);
        return PartialView("_SearchParamList", MainModel);
    }
    public async Task<HRAListDataModel> BindPBList(HRAListDataModel MainModel, Dictionary<string, object> commonparams)
    {
        Dictionary<string, object> Build(string flag)
        {
            var p = new Dictionary<string, object>() { { "@flag", flag } };

            foreach (var kv in commonparams)
                if (kv.Key != "@flag") p[kv.Key] = kv.Value;

            return p;
        }
        var dashdep = Build("DashboardFillDepartment");
        var dashcat = Build("DashboardFillCategory");
        var dashdesg = Build("DashboardFillDesignation");
        var dashemp = Build("DashboardFillEmployee");
        MainModel.DashDepartmentList = await IDataLogic.GetDropDownListWithCustomeVar("HRSPHRAttendanceMainDetail", dashdep, false, false);
        MainModel.DashCategoryList = await IDataLogic.GetDropDownListWithCustomeVar("HRSPHRAttendanceMainDetail", dashcat, false, true);
        MainModel.DashDesignationList = await IDataLogic.GetDropDownListWithCustomeVar("HRSPHRAttendanceMainDetail", dashdesg, false, false);
        MainModel.DashEmployeeList = await IDataLogic.GetDropDownListWithCustomeVar("HRSPHRAttendanceMainDetail", dashemp, false, false);
        return MainModel;
    }
    public string GetEmpByMachineName()
    {
        try
        {
            string empname = string.Empty;
            empname = HttpContext.Session.GetString("EmpName").ToString();
            if (string.IsNullOrEmpty(empname)) { empname = Environment.UserDomainName; }
            return empname;
        }
        catch
        {
            return "";
        }
    }
    public async Task<JsonResult> GetFormRights()
    {
        var userID = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
        var JSON = await IHRAttendance.GetFormRights(userID);
        string JsonString = JsonConvert.SerializeObject(JSON);
        return Json(JsonString);
    }
    public async Task<JsonResult> FillEntryId(int YearCode)
    {
        var JSON = await IHRAttendance.FillEntryId(YearCode);
        string JsonString = JsonConvert.SerializeObject(JSON);
        return Json(JsonString);
    }
}

