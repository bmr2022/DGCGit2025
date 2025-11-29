using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.Extensions.Caching.Memory;
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
        var MainModel = await IHRAttendance.GetHRAListData(string.Empty, string.Empty, string.Empty, model);
        var MRNType = "MRN";
        DateTime now = DateTime.Now;
        DateTime firstDayOfMonth = new DateTime(now.Year, now.Month, 1);
        DateTime today = DateTime.Now;
        var commonparams = new Dictionary<string, object>()
        {
            { "@MRNTYpe", MRNType },
            { "@Fromdate", firstDayOfMonth },
            { "@ToDate", today }
        };
        MainModel = await BindPBList(MainModel, commonparams);
        MainModel.FromDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).ToString("dd/MM/yyyy").Replace("-", "/");
        MainModel.ToDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day).ToString("dd/MM/yyyy").Replace("-", "/");// Last day in January next year

        return View(MainModel);
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
    public async Task<HRAListDataModel> BindPBList(HRAListDataModel MainModel, Dictionary<string, object> commonparams)
    {
        //var partyparams = new Dictionary<string, object>() { { "@flag", "FillPartyName" } };
        //partyparams.AddRange(commonparams);
        //MainModel.PartyNameList = await IDataLogic.GetDropDownListWithCustomeVar("GetPendingMRNListForPurchaseBill", partyparams, true);

        //var mrnnoparams = new Dictionary<string, object>() { { "@flag", "FillMRNNO" } };
        //mrnnoparams.AddRange(commonparams);
        //MainModel.MRNNoList = await IDataLogic.GetDropDownListWithCustomeVar("GetPendingMRNListForPurchaseBill", mrnnoparams, true);

        //var ponoparams = new Dictionary<string, object>() { { "@flag", "PONO" } };
        //ponoparams.AddRange(commonparams);
        //MainModel.PONOList = await IDataLogic.GetDropDownListWithCustomeVar("GetPendingMRNListForPurchaseBill", ponoparams, true);

        //var invparams = new Dictionary<string, object>() { { "@flag", "FillInvoiceNo" } };
        //invparams.AddRange(commonparams);
        //MainModel.InvoiceNoList = await IDataLogic.GetDropDownListWithCustomeVar("GetPendingMRNListForPurchaseBill", invparams, true);

        //var gatenoparams = new Dictionary<string, object>() { { "@flag", "FillGateNo" } };
        //gatenoparams.AddRange(commonparams);
        //MainModel.GateNoList = await IDataLogic.GetDropDownListWithCustomeVar("GetPendingMRNListForPurchaseBill", gatenoparams, true);

        //var docnameparams = new Dictionary<string, object>() { { "@flag", "FillDocument" } };
        //docnameparams.AddRange(commonparams);
        //MainModel.DocumentNameList = await IDataLogic.GetDropDownListWithCustomeVar("GetPendingMRNListForPurchaseBill", docnameparams, true);

        //var itemnameparams = new Dictionary<string, object>() { { "@flag", "FillItemName" } };
        //itemnameparams.AddRange(commonparams);
        //MainModel.ItemNameList = await IDataLogic.GetDropDownListWithCustomeVar("GetPendingMRNListForPurchaseBill", itemnameparams, true);

        //var partcodeparams = new Dictionary<string, object>() { { "@flag", "FillPartCode" } };
        //partcodeparams.AddRange(commonparams);
        //MainModel.PartCodeList = await IDataLogic.GetDropDownListWithCustomeVar("GetPendingMRNListForPurchaseBill", partcodeparams, true);
        return MainModel;
    }
}

