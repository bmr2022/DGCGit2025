using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.Wordprocessing;
using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
        HttpContext.Session.Remove("HRAttendanceList");
        var _List = new List<TextValue>();
        var MainModel = new HRAListDataModel();
        MainModel.HRAttYearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));

        FromDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).ToString("dd/MM/yyyy").Replace("-", "/");
        ToDate = string.IsNullOrEmpty(MainModel.ToDate) ? DateTime.Now.ToString("dd/MM/yyyy") : MainModel.ToDate;

        DateTime now = DateTime.Now;
        DateTime firstDayOfMonth = new DateTime(now.Year, now.Month, 1);
        DateTime today = DateTime.Now;
        MainModel.FromDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).ToString("dd/MM/yyyy").Replace("-", "/");
        MainModel.ToDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day).ToString("dd/MM/yyyy").Replace("-", "/");

        MainModel.FromDate = new DateTime(DateTime.Today.Year, 3, 1).ToString("dd/MM/yyyy").Replace("-", "/");
        MainModel.ToDate = new DateTime(DateTime.Today.Year, 3, 30).ToString("dd/MM/yyyy").Replace("-", "/");

        var fromdate = CommonFunc.ParseSafeDate(CommonFunc.ParseDate(MainModel.FromDate).ToString("dd/MM/yyyy"));
        var todate = CommonFunc.ParseSafeDate(CommonFunc.ParseDate(MainModel.ToDate).ToString("dd/MM/yyyy"));
        var commonparams = new Dictionary<string, object>()
        {
            { "@Fromdate", fromdate == default ? string.Empty : fromdate },
            { "@ToDate", todate == default ? string.Empty : todate }
        };
        MainModel = await BindPBList(MainModel, commonparams);
        if (MainModel == null) { MainModel = new HRAListDataModel(); }
        string serializedGrid = JsonConvert.SerializeObject(MainModel);
        HttpContext.Session.SetString("HRAttendanceList", serializedGrid);
         return View(MainModel);
    }
    public async Task<IActionResult> GetSearchHRAListData(HRAListDataModel model, int pageNumber = 1, int pageSize = 25, string SearchBox = "")
    {
        var _List = new List<TextValue>();
        DateTime now = DateTime.Now;
        DateTime firstDayOfMonth = new DateTime(now.Year, now.Month, 1);
        DateTime today = DateTime.Now;

        string fromDate = (model.FromDate);
        string toDate = (model.ToDate);
        var MainModel = new HRAListDataModel();
        MainModel = await IHRAttendance.GetHRAttendanceListData("PendingGateAttendance", fromDate, toDate, model);

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
        var modelList = model?.HRAListData ?? new List<HRAListDataModel>();
        if (string.IsNullOrWhiteSpace(SearchBox))
        {
            model.TotalRecords = modelList.Count();
            model.PageNumber = pageNumber;
            model.PageSize = pageSize;
            model.HRAListData = modelList
            .Skip((pageNumber - 1) * pageSize)
               .Take(pageSize)
               .ToList();
        }
        else
        {
            List<HRAListDataModel> filteredResults;
            if (string.IsNullOrWhiteSpace(SearchBox))
            {
                filteredResults = modelList.ToList();
            }
            else
            {
                filteredResults = modelList
                    .Where(i => i.GetType().GetProperties()
                        .Where(p => p.PropertyType == typeof(string))
                        .Select(p => p.GetValue(i)?.ToString())
                        .Any(value => !string.IsNullOrEmpty(value) &&
                                      value.Contains(SearchBox, StringComparison.OrdinalIgnoreCase)))
                    .ToList();
                if (filteredResults.Count == 0)
                {
                    filteredResults = modelList.ToList();
                }
            }
            model.TotalRecords = filteredResults.Count;
            model.HRAListData = filteredResults.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            model.PageNumber = pageNumber;
            model.PageSize = pageSize;
        }
        MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
        {
            AbsoluteExpiration = DateTime.Now.AddMinutes(60),
            SlidingExpiration = TimeSpan.FromMinutes(55),
            Size = 1024,
        };

        //_MemoryCache.Set($"KeyHRAttList_{model.DashboardType}", modelList, cacheEntryOptions);
        _MemoryCache.Set($"KeyHRAttList", modelList, cacheEntryOptions);
        return PartialView("_HRAListDataGrid", MainModel);
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
        var dashdep = Build("FillDepartment");
        var dashdesg = Build("FillDesignation");
        var dashemp = Build("FillEmployee");
        var dashcat = Build("FillCategory");
        MainModel.DashDepartmentList = await IDataLogic.GetDropDownListWithCustomeVar("HRSPHRAttendanceMainDetail", dashdep, false, false);
        MainModel.DashDesignationList = await IDataLogic.GetDropDownListWithCustomeVar("HRSPHRAttendanceMainDetail", dashdesg, false, false);
        MainModel.DashEmployeeList = await IDataLogic.GetDropDownListWithCustomeVar("HRSPHRAttendanceMainDetail", dashemp, false, false);
        MainModel.DashCategoryList = await IDataLogic.GetDropDownListWithCustomeVar("HRSPHRAttendanceMainDetail", dashcat, false, true);
        MainModel.DashAttendanceDateList = new List<TextValue>();
        return MainModel;
    }
    [HttpPost]
    public async Task<IActionResult> RefreshFilters(int YearCode, string fromDate, string toDate, string employeeId)
    {
        var model = new HRAListDataModel();
        var fromdate = CommonFunc.ParseSafeDate(CommonFunc.ParseDate(fromDate).ToString("dd/MM/yyyy"));
        var todate = CommonFunc.ParseSafeDate(CommonFunc.ParseDate(toDate).ToString("dd/MM/yyyy"));

        var common = new Dictionary<string, object>()
        {
            { "@FromDate", fromdate },
            { "@ToDate", todate }
        };
        if(employeeId != "0")
        {
            common.Add("@EmployeeId", employeeId);
        }
        // Rebind ONLY the required 2 dropdowns
        model = await BindPBList(model, common);

        // Bind attendance-date + category list from SP
        var spParams = new Dictionary<string, object>()
        {
            { "@flag", "PendingGateAttendanceSummary" },
            { "@HRAttYearCode", YearCode },
            { "@FromDate", fromdate },
            { "@ToDate", todate }
        };

        model.DashAttendanceDateList =  await IDataLogic.GetDropDownListWithCustomeVar("HRSPHRAttendanceMainDetail", spParams, false, true);
        model.DashAttendanceDateList = model.DashAttendanceDateList.GroupBy(a => a.Text).Select(a => new TextValue { Text = a.Key, Value = a.Key }).ToList();
        var DashCategList = await IDataLogic.GetDropDownListWithCustomeVar("HRSPHRAttendanceMainDetail", spParams, false, false, true);
        model.DashCategoryList = model.DashCategoryList ?? (DashCategList ?? new List<TextValue>());
        if (DashCategList != null && DashCategList.Any()) { model.DashCategoryList = model.DashCategoryList.Where(a => DashCategList.Any(d => d?.Text == a?.Value)).ToList(); }

        return Json(new
        {
            attendanceDates = model.DashAttendanceDateList,
            categories = model.DashCategoryList
        });
    }
    [HttpGet]
    public IActionResult GlobalSearch(string searchString, string dashboardType = "SUMMARY", int pageNumber = 1, int pageSize = 25)
    {
        HRAListDataModel model = new HRAListDataModel();
        if (string.IsNullOrWhiteSpace(searchString))
        {
            return PartialView("_HRAListDataGrid", new List<HRAListDataModel>());
        }
        //string cacheKey = $"KeyHRAttList_{dashboardType}";
        string cacheKey = $"KeyHRAttList";
        if (!_MemoryCache.TryGetValue(cacheKey, out IList<HRAListDataModel> Dashboard) || Dashboard == null)
        {
            return PartialView("_HRAListDataGrid", new List<HRAListDataModel>());
        }

        List<HRAListDataModel> filteredResults;

        if (string.IsNullOrWhiteSpace(searchString))
        {
            filteredResults = Dashboard.ToList();
        }
        else
        {
            filteredResults = Dashboard
                .Where(i => i.GetType().GetProperties()
                    .Where(p => p.PropertyType == typeof(string))
                    .Select(p => p.GetValue(i)?.ToString())
                    .Any(value => !string.IsNullOrEmpty(value) &&
                                  value.Contains(searchString, StringComparison.OrdinalIgnoreCase)))
                .ToList();


            if (filteredResults.Count == 0)
            {
                filteredResults = Dashboard.ToList();
            }
        }

        model.TotalRecords = filteredResults.Count;
        model.HRAListData = filteredResults.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
        model.PageNumber = pageNumber;
        model.PageSize = pageSize;
        //model.DashboardType = dashboardType;
        return PartialView("_HRAListDataGrid", model);
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
    public async Task<JsonResult> CheckLockYear(int YearCode)
    {
        var JSON = await IHRAttendance.CheckLockYear(YearCode);
        string JsonString = JsonConvert.SerializeObject(JSON);
        return Json(JsonString);
    }
}

