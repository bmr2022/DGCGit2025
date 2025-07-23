using System.Diagnostics;
using System.Runtime.Caching;
using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using FastReport.Export.Html;
using FastReport.Export.Image;
using FastReport;
using FastReport.Web;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using NuGet.Packaging;

using static eTactWeb.DOM.Models.Common;
using static eTactWeb.Data.Common.CommonFunc;
using OfficeOpenXml;
using System.Data;
using Newtonsoft.Json.Linq;
//using static Grpc.Core.Metadata;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Net;

using System.Globalization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using eTactWeb.Data.DAL;

namespace eTactWeb.Controllers;

[Authorize]
public class PurchaseBillController : Controller
{
    private readonly IWebHostEnvironment _IWebHostEnvironment;
    private readonly IConfiguration iconfiguration;
    private readonly ICompositeViewEngine _viewEngine;
    private readonly IMemoryCache _MemoryCache;
    public PurchaseBillController(IPurchaseBill iPurchaseBill, IDataLogic iDataLogic, ILogger<PurchaseBillModel> logger, EncryptDecrypt encryptDecrypt, IMemoryCacheService iMemoryCacheService, IWebHostEnvironment iWebHostEnvironment, IConfiguration configuration, ICompositeViewEngine viewEngine, IMemoryCache iMemoryCache)
    {
        IPurchaseBill = iPurchaseBill;
        IDataLogic = iDataLogic;
        _Logger = logger;
        EncryptDecrypt = encryptDecrypt;
        CI = new CultureInfo("en-GB");
        _IWebHostEnvironment = iWebHostEnvironment;
        iconfiguration = configuration;
        _MemoryCache = iMemoryCache;
        _viewEngine = viewEngine;
    }

    public ILogger<PurchaseBillModel> _Logger { get; set; }
    public CultureInfo CI { get; private set; }
    public EncryptDecrypt EncryptDecrypt { get; private set; }
    public IDataLogic IDataLogic { get; private set; }
    public IPurchaseBill IPurchaseBill { get; set; }

    public IActionResult PrintReport(int EntryId = 0, int YearCode = 0, string PONO = "")
    {
        string my_connection_string;
        string contentRootPath = _IWebHostEnvironment.ContentRootPath;
        string webRootPath = _IWebHostEnvironment.WebRootPath;
        //string frx = Path.Combine(_env.ContentRootPath, "reports", value.file);
        var webReport = new WebReport();

        var ReportName = IPurchaseBill.GetReportName();
        webReport.Report.Load(webRootPath + "\\PurchaseBill.frx");
        //if (ReportName.Result.Result.Rows[0].ItemArray[0] != System.DBNull.Value)
        //{
        //    webReport.Report.Load(webRootPath + "\\PurchaseBill.frx"); // from database
        //}
        //else
        //{
        //    webReport.Report.Load(webRootPath + "\\PO.frx"); // default report

        //}
        //webReport.Report.SetParameterValue("flagparam", "PURCHASEORDERPRINT");
        webReport.Report.SetParameterValue("entryparam", EntryId);
        webReport.Report.SetParameterValue("yearparam", YearCode);
        //webReport.Report.SetParameterValue("ponoparam", PONO);


        my_connection_string = iconfiguration.GetConnectionString("eTactDB");

        webReport.Report.SetParameterValue("MyParameter", my_connection_string);


        // webReport.Report.SetParameterValue("accountparam", 1731);


        // webReport.Report.Dictionary.Connections[0].ConnectionString = @"Data Source=103.10.234.95;AttachDbFilename=;Initial Catalog=eTactWeb;Integrated Security=False;Persist Security Info=True;User ID=web;Password=bmr2401";
        //ViewBag.WebReport = webReport;
        return View(webReport);
    }
    public ActionResult HtmlSave(int EntryId = 0, int YearCode = 0, string PONO = "")
    {
        using (Report report = new Report())
        {
            string webRootPath = _IWebHostEnvironment.WebRootPath;
            var webReport = new WebReport();


            webReport.Report.Load(webRootPath + "\\PO.frx");
            //webReport.Report.SetParameterValue("flagparam", "PURCHASEORDERPRINT");
            webReport.Report.SetParameterValue("entryparam", EntryId);
            webReport.Report.SetParameterValue("yearparam", YearCode);
            webReport.Report.SetParameterValue("ponoparam", PONO);
            webReport.Report.Prepare();// Preparing a report

            // Creating the HTML export
            using (HTMLExport html = new HTMLExport())
            {
                using (FileStream st = new FileStream(webRootPath + "\\test.html", FileMode.Create))
                {
                    webReport.Report.Export(html, st);
                    return File("App_Data/test.html", "application/octet-stream", "Test.html");
                }
            }
        }
    }

    public IActionResult GetImage(int EntryId = 0, int YearCode = 0, string PONO = "")
    {
        // Creatint the Report object
        using (Report report = new Report())
        {
            string webRootPath = _IWebHostEnvironment.WebRootPath;
            var webReport = new WebReport();


            webReport.Report.Load(webRootPath + "\\PO.frx");
            //webReport.Report.SetParameterValue("flagparam", "PURCHASEORDERPRINT");
            webReport.Report.SetParameterValue("entryparam", EntryId);
            webReport.Report.SetParameterValue("yearparam", YearCode);
            webReport.Report.SetParameterValue("ponoparam", PONO);
            webReport.Report.Prepare();// Preparing a report

            // Creating the Image export
            using (ImageExport image = new ImageExport())
            {
                image.ImageFormat = ImageExportFormat.Jpeg;
                image.JpegQuality = 100; // Set up the quality
                image.Resolution = 100; // Set up a resolution 
                image.SeparateFiles = false; // We need all pages in one big single file

                using (MemoryStream st = new MemoryStream())// Using stream to save export
                {
                    webReport.Report.Export(image, st);
                    return base.File(st.ToArray(), "image/jpeg");
                }
            }
        }
    }
    public IActionResult AddItem2Grid(PurchaseBillModel model)
    {
        bool TF = false;
        string taxModelJson = HttpContext.Session.GetString("KeyTaxGrid");
        List<TaxModel> PBTaxdetail = new List<TaxModel>();
        if (!string.IsNullOrEmpty(taxModelJson))
        {
            PBTaxdetail = JsonConvert.DeserializeObject<List<TaxModel>>(taxModelJson);
        }
        string tdsModelJson = HttpContext.Session.GetString("KeyTDSGrid");
        List<TDSModel> PBTDSdetail = new List<TDSModel>();
        if (!string.IsNullOrEmpty(tdsModelJson))
        {
            PBTDSdetail = JsonConvert.DeserializeObject<List<TDSModel>>(tdsModelJson);
        }
        string mainModelJson = HttpContext.Session.GetString("PurchaseBill");
        PurchaseBillModel MainModel = new PurchaseBillModel();
        if (!string.IsNullOrEmpty(mainModelJson))
        {
            MainModel = JsonConvert.DeserializeObject<PurchaseBillModel>(mainModelJson);
        }

        if (MainModel != null && MainModel.ItemDetailGrid != null)
        {
            TF = MainModel.ItemDetailGrid.Any(x => x.ItemCode == model.ItemCode && x.DocTypeID == model.DocTypeID && x.Description == model.Description);
        }

        if (TF)
        {
            return StatusCode(208, "Duplicate Item");
        }
        else
        {
            model = BindItem4Grid(model);
            HttpContext.Session.Remove("PurchaseBill");
            string modelJson = JsonConvert.SerializeObject(model);
            HttpContext.Session.SetString("PurchaseBill", modelJson);
        }

        return PartialView("_PBItemGrid", model);
    }
    public async Task<JsonResult> GetFormRights()
    {
        var userID = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
        var JSON = await IPurchaseBill.GetFormRights(userID);
        string JsonString = JsonConvert.SerializeObject(JSON);
        return Json(JsonString);
    }
    public async Task<JsonResult> CheckLockYear(int YearCode)
    {
        var JSON = await IPurchaseBill.CheckLockYear(YearCode);
        string JsonString = JsonConvert.SerializeObject(JSON);
        return Json(JsonString);
    }
    public async Task<JsonResult> FillCurrency(int? AccountCode)
    {
        var JSON = await IPurchaseBill.FillCurrency(AccountCode);
        string JsonString = JsonConvert.SerializeObject(JSON);
        return Json(JsonString);
    }
    public async Task<JsonResult> GetExchangeRate(string Currency)
    {
        var JSON = await IPurchaseBill.GetExchangeRate(Currency);
        string JsonString = JsonConvert.SerializeObject(JSON);
        return Json(JsonString);
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
    public async Task<PurchaseBillModel> BindModels(PurchaseBillModel model)
    {
        CommonFunc.LogException<PurchaseBillModel>.LogInfo(_Logger, "**********  *************");

        _Logger.LogInformation("********** Binding Model *************");

        var oDataSet = new DataSet();
        var SqlParams = new List<KeyValuePair<string, string>>();
        model.AccountList = new List<TextValue>();
        model.BranchList = new List<TextValue>();
        model.PartCodeList = new List<TextValue>();
        model.ItemNameList = new List<TextValue>();
        model.DepartmentList = new List<TextValue>();
        model.CostCenterList = new List<TextValue>();
        model.PreparedByList = new List<TextValue>();
        model.ProcessList = new List<TextValue>();
        return model;
    }

    [Route("{controller}/Dashboard")]
    public async Task<IActionResult> DashBoard(string FromDate = "", string ToDate = "", string DashboardType = "", string MRNType = "", string DocumentName = "", string VendorName = "", string VoucherNo = "", string InvoiceNo = "", string MRNNo = "", string GateNo = "", string PartCode = "", string ItemName = "", string HSNNo = "", string Searchbox = "", string Flag = "True")
    {
        HttpContext.Session.Remove("PurchaseBill");
        HttpContext.Session.Remove("TaxGrid");
        HttpContext.Session.Remove("KeyTaxGrid");

        var _List = new List<TextValue>();

        var MainModel = await IPurchaseBill.GetDashBoardData();
        PBDashBoard model = new PBDashBoard();
        DateTime now = DateTime.Now;
        DateTime firstDayOfMonth = new DateTime(now.Year, now.Month, 1);
        DateTime today = DateTime.Now;
        var commonparams = new Dictionary<string, object>()
        {
            { "@Fromdate", firstDayOfMonth },
            { "@ToDate", today }
        };
        MainModel = await BindDashboardList(MainModel, commonparams);
        MainModel.FromDate = new DateTime(DateTime.Today.Year, 4, 1).ToString("dd/MM/yyyy").Replace("-", "/");
        MainModel.ToDate = new DateTime(DateTime.Today.Year + 1, 3, 31).ToString("dd/MM/yyyy").Replace("-", "/");// Last day in January next year
        if (Flag != "True")
        {
            MainModel.FromDate = FromDate;
            MainModel.ToDate = ToDate;
            MainModel.DashboardType = DashboardType != null && DashboardType != "0" && DashboardType != "undefined" ? DashboardType : "0";
            MainModel.MRNType = MRNType != null && MRNType != "0" && MRNType != "undefined" ? MRNType : "";
            MainModel.DocumentName = DocumentName != null && DocumentName != "0" && DocumentName != "undefined" ? DocumentName : "0";
            MainModel.VendorName = VendorName != null && VendorName != "0" && VendorName != "undefined" ? VendorName : "0";
            MainModel.VoucherNo = VoucherNo != null && VoucherNo != "0" && VoucherNo != "undefined" ? VoucherNo : "0";
            MainModel.InvoiceNo = InvoiceNo != null && InvoiceNo != "0" && InvoiceNo != "undefined" ? InvoiceNo : "0";
            MainModel.MRNNo = MRNNo != null && MRNNo != "0" && MRNNo != "undefined" ? MRNNo : "0";
            MainModel.GateNo = GateNo != null && GateNo != "0" && GateNo != "undefined" ? GateNo : "0";
            MainModel.PartCode = PartCode != null && PartCode != "0" && PartCode != "undefined" ? PartCode : "0";
            MainModel.ItemName = ItemName != null && ItemName != "0" && ItemName != "undefined" ? ItemName : "0";
            MainModel.HSNNO = HSNNo != null && HSNNo != "0" && HSNNo != "undefined" ? HSNNo : "0";
            MainModel.Searchbox = Searchbox != null && Searchbox != "0" && Searchbox != "undefined" ? Searchbox : "";
        }
        return View(MainModel);
    }

    public async Task<IActionResult> PurchaseBillList(string? FromDate,string? ToDate)
    {
        var _List = new List<TextValue>();
        PBListDataModel model = new PBListDataModel();
       FromDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).ToString("dd/MM/yyyy").Replace("-", "/");

        ToDate = string.IsNullOrEmpty(model.ToDate) ? DateTime.Now.ToString("dd/MM/yyyy") : model.ToDate;
        var MainModel = await IPurchaseBill.GetPurchaseBillListData(string.Empty, string.Empty, string.Empty,FromDate,ToDate, model);
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
    public async Task<IActionResult> GetSearchPBListData(PBListDataModel model)
    {
        var _List = new List<TextValue>();
        var MRNType = model.MRNType ?? "MRN";
        DateTime now = DateTime.Now;
        DateTime firstDayOfMonth = new DateTime(now.Year, now.Month, 1);
        DateTime today = DateTime.Now;

        string fromDate = (model.FromDate);
        string toDate = (model.ToDate);

        var MainModel = await IPurchaseBill.GetPurchaseBillListData("DisplayPendingData", MRNType, model.DashboardType ?? "SUMMARY", fromDate, toDate, model);

        //fromDate = DateTime.ParseExact(model.FromDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
        //toDate = DateTime.ParseExact(model.ToDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);

        var commonparams = new Dictionary<string, object>()
        {
            { "@MRNTYpe", MRNType },
            { "@Fromdate", model.FromDate != null ? fromDate : firstDayOfMonth },
            { "@ToDate", model.ToDate != null ? toDate : today }
        };
        MainModel = await BindPBList(MainModel, commonparams);
        MainModel.FromDate = new DateTime(DateTime.Today.Year, 4, 1).ToString("dd/MM/yyyy").Replace("-", "/");
        MainModel.ToDate = new DateTime(DateTime.Today.Year + 1, 3, 31).ToString("dd/MM/yyyy").Replace("-", "/");// Last day in January next year

        //MainModel.DashboardType = "Summary";
        return PartialView("_PBListDataGrid", MainModel);
    }
    public async Task<IActionResult> GetPBListDropdownData(PBListDataModel model)
    {
        var _List = new List<TextValue>();
        var MRNType = model.MRNType ?? "MRN";
        DateTime now = DateTime.Now;
        DateTime firstDayOfMonth = new DateTime(now.Year, now.Month, 1);
        DateTime today = DateTime.Now;
        var MainModel = model;
        DateTime fromDate = DateTime.ParseExact(model.FromDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
        DateTime toDate = DateTime.ParseExact(model.ToDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);

        var commonparams = new Dictionary<string, object>()
        {
            { "@MRNTYpe", MRNType },
            { "@Fromdate", model.FromDate != null ? fromDate : firstDayOfMonth },
            { "@ToDate", model.ToDate != null ? toDate : today }
        };
        MainModel = await BindPBList(MainModel, commonparams);
        return PartialView("_SearchParamForPBList", MainModel);
    }
    public async Task<PBListDataModel> BindPBList(PBListDataModel MainModel, Dictionary<string, object> commonparams)
    {
        var partyparams = new Dictionary<string, object>() { { "@flag", "FillPartyName" } };
        partyparams.AddRange(commonparams);
        MainModel.PartyNameList = await IDataLogic.GetDropDownListWithCustomeVar("GetPendingMRNListForPurchaseBill", partyparams, true);

        var mrnnoparams = new Dictionary<string, object>() { { "@flag", "FillMRNNO" } };
        mrnnoparams.AddRange(commonparams);
        MainModel.MRNNoList = await IDataLogic.GetDropDownListWithCustomeVar("GetPendingMRNListForPurchaseBill", mrnnoparams, true);

        var ponoparams = new Dictionary<string, object>() { { "@flag", "PONO" } };
        ponoparams.AddRange(commonparams);
        MainModel.PONOList = await IDataLogic.GetDropDownListWithCustomeVar("GetPendingMRNListForPurchaseBill", ponoparams, true);

        var invparams = new Dictionary<string, object>() { { "@flag", "FillInvoiceNo" } };
        invparams.AddRange(commonparams);
        MainModel.InvoiceNoList = await IDataLogic.GetDropDownListWithCustomeVar("GetPendingMRNListForPurchaseBill", invparams, true);

        var gatenoparams = new Dictionary<string, object>() { { "@flag", "FillGateNo" } };
        gatenoparams.AddRange(commonparams);
        MainModel.GateNoList = await IDataLogic.GetDropDownListWithCustomeVar("GetPendingMRNListForPurchaseBill", gatenoparams, true);

        var docnameparams = new Dictionary<string, object>() { { "@flag", "FillDocument" } };
        docnameparams.AddRange(commonparams);
        MainModel.DocumentNameList = await IDataLogic.GetDropDownListWithCustomeVar("GetPendingMRNListForPurchaseBill", docnameparams, true);

        var itemnameparams = new Dictionary<string, object>() { { "@flag", "FillItemName" } };
        itemnameparams.AddRange(commonparams);
        MainModel.ItemNameList = await IDataLogic.GetDropDownListWithCustomeVar("GetPendingMRNListForPurchaseBill", itemnameparams, true);

        var partcodeparams = new Dictionary<string, object>() { { "@flag", "FillPartCode" } };
        partcodeparams.AddRange(commonparams);
        MainModel.PartCodeList = await IDataLogic.GetDropDownListWithCustomeVar("GetPendingMRNListForPurchaseBill", partcodeparams, true);
        return MainModel;
    }
    public async Task<PBDashBoard> BindDashboardList(PBDashBoard MainModel, Dictionary<string, object> commonparams)
    {
        var docnameparams = new Dictionary<string, object>() { { "@flag", "FillDocumentDASHBOARD" } };
        docnameparams.AddRange(commonparams);
        MainModel.DocumentNameList = await IDataLogic.GetDropDownListWithCustomeVar("AccSP_PurchaseBillMainDetail", docnameparams, true);

        var vendornameparams = new Dictionary<string, object>() { { "@flag", "FillVendorNameDASHBOARD" } };
        vendornameparams.AddRange(commonparams);
        MainModel.VendorNameList = await IDataLogic.GetDropDownListWithCustomeVar("AccSP_PurchaseBillMainDetail", vendornameparams, true);

        var vouchnoparams = new Dictionary<string, object>() { { "@flag", "FillVoucherNoDASHBOARD" } };
        vouchnoparams.AddRange(commonparams);
        MainModel.VoucherNoList = await IDataLogic.GetDropDownListWithCustomeVar("AccSP_PurchaseBillMainDetail", vouchnoparams, true);

        var invparams = new Dictionary<string, object>() { { "@flag", "FillInvoiceNoDASHBOARD" } };
        invparams.AddRange(commonparams);
        MainModel.InvoiceNoList = await IDataLogic.GetDropDownListWithCustomeVar("AccSP_PurchaseBillMainDetail", invparams, true);

        var mrnnoparams = new Dictionary<string, object>() { { "@flag", "FillMrnNoDASHBOARD" } };
        mrnnoparams.AddRange(commonparams);
        MainModel.MRNNoList = await IDataLogic.GetDropDownListWithCustomeVar("AccSP_PurchaseBillMainDetail", mrnnoparams, true);

        var gatenoparams = new Dictionary<string, object>() { { "@flag", "FillGateNoDASHBOARD" } };
        gatenoparams.AddRange(commonparams);
        MainModel.GateNoList = await IDataLogic.GetDropDownListWithCustomeVar("AccSP_PurchaseBillMainDetail", gatenoparams, true);

        var partcodeparams = new Dictionary<string, object>() { { "@flag", "FillPartCodeDASHBOARD" } };
        partcodeparams.AddRange(commonparams);
        MainModel.PartCodeList = await IDataLogic.GetDropDownListWithCustomeVar("AccSP_PurchaseBillMainDetail", partcodeparams, true);

        var itemnameparams = new Dictionary<string, object>() { { "@flag", "FillItemNameDASHBOARD" } };
        itemnameparams.AddRange(commonparams);
        MainModel.ItemNameList = await IDataLogic.GetDropDownListWithCustomeVar("AccSP_PurchaseBillMainDetail", itemnameparams, true);

        var hsnnoparams = new Dictionary<string, object>() { { "@flag", "FillHSNNODASHBOARD" } };
        hsnnoparams.AddRange(commonparams);
        MainModel.HSNNOList = await IDataLogic.GetDropDownListWithCustomeVar("AccSP_PurchaseBillMainDetail", hsnnoparams, true);

        return MainModel;
    }

    public async Task<IActionResult> DeleteByIDOld(int ID, int YC, string PurchVoucherNo, string InvNo = "", bool? IsDetail = false)
    {
        int EntryBy = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
        string EntryByMachineName = GetEmpByMachineName();
        DateTime EntryDate = DateTime.Today;
        var Result = await IPurchaseBill.DeleteByID(ID, YC, "DELETE", PurchVoucherNo, InvNo, EntryBy, EntryByMachineName, EntryDate);

        if (Result.StatusText == "Success" || Result.StatusCode == HttpStatusCode.Gone)
        {
            ViewBag.isSuccess = true;
            TempData["410"] = "410";
        }
        else if (Result.StatusText == "Error" || Result.StatusCode == HttpStatusCode.Locked)
        {
            ViewBag.isSuccess = true;
            TempData["423"] = "423";
        }
        if ((Result.StatusText == "Deleted Successfully" || Result.StatusText == "deleted Successfully") && (Result.StatusCode == HttpStatusCode.Accepted || Result.StatusCode == HttpStatusCode.OK))
        {
            ViewBag.isSuccess = true;
            TempData["410"] = "410";
        }
        else
        {
            ViewBag.isSuccess = false;
            TempData["500"] = "500";
        }

        return RedirectToAction(nameof(DashBoard));
    }
    public async Task<JsonResult> DeleteByID(int ID, int YC, string PurchVoucherNo, string EnteredBy, string InvNo = "", bool? IsDetail = false)
    {
        int EntryBy = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
        DateTime EntryDate = DateTime.Today;
        var Result = await IPurchaseBill.DeleteByID(ID, YC, "DELETE", PurchVoucherNo, InvNo,EntryBy, EnteredBy, EntryDate);

        var rslt = string.Empty;
        if (Result.StatusText == "Success" || Result.StatusCode == HttpStatusCode.Gone)
        {
            ViewBag.isSuccess = true;
            TempData["410"] = "410";
            rslt = "true";
        }
        else if (Result.StatusText == "Error" || Result.StatusCode == HttpStatusCode.Locked)
        {
            ViewBag.isSuccess = true;
            TempData["423"] = "423";
            rslt = "true";
        }
        if ((Result.StatusText == "Deleted Successfully" || Result.StatusText == "deleted Successfully") && (Result.StatusCode == HttpStatusCode.Accepted || Result.StatusCode == HttpStatusCode.OK))
        {
            ViewBag.isSuccess = true;
            TempData["410"] = "410";
            rslt = "true";
        }
        else
        {
            ViewBag.isSuccess = false;
            TempData["500"] = "500";
            rslt = "false";
        }
        return Json(new { success=rslt, message = Result.StatusText });

    //    return Json(rslt);
        //return RedirectToAction(nameof(DashBoard));   
    }

    public IActionResult DeleteItemRow(string SeqNo)
    {
        bool exists = false;

        string tdsGridJson = HttpContext.Session.GetString("KeyTDSGrid");
        List<TDSModel> TDSGrid = !string.IsNullOrEmpty(tdsGridJson)
            ? JsonConvert.DeserializeObject<List<TDSModel>>(tdsGridJson)
            : new List<TDSModel>();
        string taxGridJson = HttpContext.Session.GetString("KeyTaxGrid");
        List<TaxModel> TaxGrid = !string.IsNullOrEmpty(taxGridJson)
            ? JsonConvert.DeserializeObject<List<TaxModel>>(taxGridJson)
            : new List<TaxModel>();
        string mainModelJson = HttpContext.Session.GetString("PurchaseBill");
        PurchaseBillModel MainModel = !string.IsNullOrEmpty(mainModelJson)
            ? JsonConvert.DeserializeObject<PurchaseBillModel>(mainModelJson)
            : new PurchaseBillModel();

        int Indx = Convert.ToInt32(SeqNo) - 1;

        if (MainModel.ItemDetailGrid.Count != 0)
        {
            var itemfound = MainModel.ItemDetailGrid.FirstOrDefault(item => item.SeqNo == Convert.ToInt32(SeqNo)).PartCode;

            var ItmPartCode = (MainModel.ItemDetailGrid.Where(item => item.SeqNo == Convert.ToInt32(SeqNo)).Select(item => item.PartCode)).FirstOrDefault();

            if (TaxGrid != null)
            {
                //exists = TaxGrid.Any(x => x.TxPartCode == ItmPartCode);
            }

            if (exists)
            {
                return StatusCode(207, "Duplicate");
            }

            MainModel.ItemDetailGrid.RemoveAt(Convert.ToInt32(Indx));

            Indx = 0;

            foreach (PBItemDetail item in MainModel.ItemDetailGrid)
            {
                Indx++;
                item.SeqNo = Indx;
            }
            MainModel.ItemNetAmount = MainModel.ItemDetailGridd?.Sum(x => Convert.ToDecimal(x.Amount ?? 0)) ?? 0;
            HttpContext.Session.SetString("PurchaseBill", JsonConvert.SerializeObject(MainModel));
        }
        return PartialView("_PBItemGrid", MainModel);
    }
    public async Task<JsonResult> fillEntryandVouchNo(int YearCode, string VODate)
    {
        var JSON = await IPurchaseBill.fillEntryandVouchNo(YearCode, VODate);
        string JsonString = JsonConvert.SerializeObject(JSON);
        return Json(JsonString);
    }
    public async Task<JsonResult> GetAllowDocName()
    {
        var JSON = await IPurchaseBill.GetAllowDocName();
        string JsonString = JsonConvert.SerializeObject(JSON);
        return Json(JsonString);
    }
    public async Task<JsonResult> GetAllowInvoiceNo()
    {
        var JSON = await IPurchaseBill.GetAllowInvoiceNo();
        string JsonString = JsonConvert.SerializeObject(JSON);
        return Json(JsonString);
    }
    public async Task<JsonResult> FillDocName(string ShowAll)
    {
        var JSON = await IPurchaseBill.FillDocName(ShowAll);
        string JsonString = JsonConvert.SerializeObject(JSON);
        return Json(JsonString);
    }
    public async Task<JsonResult> ClearTaxGrid(int YearCode, string VODate)
    {
        HttpContext.Session.Remove("KeyTaxGrid");
        var JSON = await IPurchaseBill.fillEntryandVouchNo(YearCode, VODate);
        string JsonString = JsonConvert.SerializeObject(JSON);
        return Json(JsonString);
    }
    public async Task<JsonResult> ClearTDSGrid(int YearCode, string VODate)
    {
        HttpContext.Session.Remove("KeyTDSGrid");
        var JSON = await IPurchaseBill.fillEntryandVouchNo(YearCode, VODate);
        string JsonString = JsonConvert.SerializeObject(JSON);
        return Json(JsonString);
    }
    public async Task<IActionResult> GetSearchData(PBDashBoard model, int pageNumber = 1, int pageSize = 5, string SearchBox = "")
    {
        model = await IPurchaseBill.GetSummaryData(model);
        model.DashboardType = "Summary";
        var modelList = model?.PBDashboard ?? new List<PBDashBoard>();


        if (string.IsNullOrWhiteSpace(SearchBox))
        {
            model.TotalRecords = modelList.Count();
            model.PageNumber = pageNumber;
            model.PageSize = pageSize;
            model.PBDashboard = modelList
            .Skip((pageNumber - 1) * pageSize)
               .Take(pageSize)
               .ToList();
        }
        else
        {
            List<PBDashBoard> filteredResults;
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
            model.PBDashboard = filteredResults.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            model.PageNumber = pageNumber;
            model.PageSize = pageSize;
        }
        MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
        {
            AbsoluteExpiration = DateTime.Now.AddMinutes(60),
            SlidingExpiration = TimeSpan.FromMinutes(55),
            Size = 1024,
        };

        _MemoryCache.Set("KeyPurchaseBillList_Summary", modelList, cacheEntryOptions);
        return PartialView("_DashBoardGrid", model);
    }
    public async Task<IActionResult> GetDetailData(PBDashBoard model, int pageNumber = 1, int pageSize = 5, string SearchBox = "")
    {
        model.Mode = "SEARCH";
        var type = model.DashboardType;
        model = await IPurchaseBill.GetDetailData(model);
        model.DashboardType = type;
        var modelList = model?.PBDashboard ?? new List<PBDashBoard>();


        if (string.IsNullOrWhiteSpace(SearchBox))
        {
            model.TotalRecords = modelList.Count();
            model.PageNumber = pageNumber;
            model.PageSize = pageSize;
            model.PBDashboard = modelList
            .Skip((pageNumber - 1) * pageSize)
               .Take(pageSize)
               .ToList();
        }
        else
        {
            List<PBDashBoard> filteredResults;
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
            model.PBDashboard = filteredResults.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            model.PageNumber = pageNumber;
            model.PageSize = pageSize;
        }
        MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
        {
            AbsoluteExpiration = DateTime.Now.AddMinutes(60),
            SlidingExpiration = TimeSpan.FromMinutes(55),
            Size = 1024,
        };
        if (type == "TAXDetail")
        {
            _MemoryCache.Set("KeyPurchaseBillList_TAXDetail", modelList, cacheEntryOptions);
        }
        else
        {
            _MemoryCache.Set("KeyPurchaseBillList_Detail", modelList, cacheEntryOptions);

        }

        return PartialView("_DashBoardGrid", model);
    }
    [HttpGet]
    public IActionResult GlobalSearch(string searchString, string dashboardType = "Summary", int pageNumber = 1, int pageSize = 50)
    {
        PBDashBoard model = new PBDashBoard();
        if (string.IsNullOrWhiteSpace(searchString))
        {
            return PartialView("_DashBoardGrid", new List<PBDashBoard>());
        }
        string cacheKey = $"KeyPurchaseBillList_{dashboardType}";
        if (!_MemoryCache.TryGetValue(cacheKey, out IList<PBDashBoard> purchaseBillDashboard) || purchaseBillDashboard == null)
        {
            return PartialView("_DashBoardGrid", new List<PBDashBoard>());
        }

        List<PBDashBoard> filteredResults;

        if (string.IsNullOrWhiteSpace(searchString))
        {
            filteredResults = purchaseBillDashboard.ToList();
        }
        else
        {
            filteredResults = purchaseBillDashboard
                .Where(i => i.GetType().GetProperties()
                    .Where(p => p.PropertyType == typeof(string))
                    .Select(p => p.GetValue(i)?.ToString())
                    .Any(value => !string.IsNullOrEmpty(value) &&
                                  value.Contains(searchString, StringComparison.OrdinalIgnoreCase)))
                .ToList();


            if (filteredResults.Count == 0)
            {
                filteredResults = purchaseBillDashboard.ToList();
            }
        }

        model.TotalRecords = filteredResults.Count;
        model.PBDashboard = filteredResults.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
        model.PageNumber = pageNumber;
        model.PageSize = pageSize;
        if (dashboardType == "Summary")
        {

            return PartialView("_DashBoardGrid", model);
        }
        else
        {
            return PartialView("_DashboardDetailGrid", model);
        }
    }
    // GET: PurchaseOrderController
    [HttpGet]
    [Route("{controller}/Index")]
    public async Task<IActionResult> PurchaseBill(int ID, int YearCode, string Mode, string? flag, string? FlagMRNJWCHALLAN, string? Mrnno, int? mrnyearcode, int? accountcode, string FromDate = "", string ToDate = "", string DashboardType = "", string MRNType = "", string DocumentName = "", string VendorName = "", string VoucherNo = "", string InvoiceNo = "", string MRNNo = "", string GateNo = "", string PartCode = "", string ItemName = "", string HSNNo = "", string Searchbox = "")
    {
        HttpContext.Session.Remove("PBTaxGrid");
        HttpContext.Session.Remove("KeyTaxGrid");
        HttpContext.Session.Remove("PBTDSGrid");
        HttpContext.Session.Remove("KeyTDSGrid");
        HttpContext.Session.Remove("PurchaseBill");
        HttpContext.Session.Remove("KeyAdjGrid");

        //_Logger.LogInformation("\n \n ********** Page Direct Purchase Bill ********** \n \n " + _IWebHostEnvironment.EnvironmentName.ToString() + "\n \n");
        //TempData.Clear();
        var MainModel = new PurchaseBillModel();
        MainModel.YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
        MainModel.FinFromDate = HttpContext.Session.GetString("FromDate");
        MainModel.FinToDate = HttpContext.Session.GetString("ToDate");
        MainModel.PreparedBy = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
        MainModel.PreparedByName = GetEmpByMachineName();
        MainModel.Branch = HttpContext.Session.GetString("Branch");
        //var txGrid = MainModel.TaxDetailGridd == null ? new TaxModel() : MainModel.TaxDetailGridd

        if (!string.IsNullOrEmpty(Mode) && ID > 0 && (Mode == "V" || Mode == "U"))
        {
            MainModel = await IPurchaseBill.GetViewByID(ID, YearCode, "ViewByID").ConfigureAwait(false);
            MainModel.Mode = Mode;
            MainModel.TDSMode = Mode;
            MainModel.ID = ID;
            MainModel.YearCode = YearCode;
            MainModel = await BindModels(MainModel).ConfigureAwait(false);
            MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                SlidingExpiration = TimeSpan.FromMinutes(55),
                Size = 1024,
            };
            MainModel.Mode = Mode;
            MainModel.FinFromDate = HttpContext.Session.GetString("FromDate");
            MainModel.FinToDate = HttpContext.Session.GetString("ToDate");
            MainModel.PreparedBy = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
            MainModel.PreparedByName = GetEmpByMachineName();
            MainModel.Branch = HttpContext.Session.GetString("Branch");
        }
        else if (!string.IsNullOrEmpty(Mode) && (Mode == "PBI"))
        {
            var model = new PurchaseBillModel();
            model = await IPurchaseBill.GetPurchaseBillItemData(flag, FlagMRNJWCHALLAN, Mrnno, mrnyearcode, accountcode).ConfigureAwait(true);

            model.Mode = Mode;
            model.YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
            model.FinFromDate = HttpContext.Session.GetString("FromDate");
            model.FinToDate = HttpContext.Session.GetString("ToDate");
            model.PreparedBy = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
            model.PreparedByName = GetEmpByMachineName();
            model.Branch = HttpContext.Session.GetString("Branch");
            MainModel = model;
        }
        else
        {
            if (string.IsNullOrEmpty(Mode) && ID == 0)
            {
                MainModel = await BindModels(MainModel).ConfigureAwait(false);
                MainModel.EID = EncryptDecrypt.Encrypt(ID.ToString(CI));
                MainModel.Mode = "INSERT";
                MainModel.TDSMode = "INSERT";
            }
            else
            {
                MainModel = await BindModels(MainModel).ConfigureAwait(false);
                MainModel.EID = EncryptDecrypt.Encrypt(ID.ToString(CI));
            }
        }

        if (Mode != "U" && Mode != "V")
        {
            MainModel.CreatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
            MainModel.CretaedByName = GetEmpByMachineName();
            MainModel.CreatedOn = DateTime.Now;
        }
        else
        {
            MainModel.UpdatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
            MainModel.UpdatedByName = GetEmpByMachineName();
            MainModel.UpdatedOn = DateTime.Now;
        }

        MainModel.FromDateBack = FromDate;
        MainModel.ToDateBack = ToDate;
        MainModel.DashboardTypeBack = DashboardType != null && DashboardType != "0" && DashboardType != "undefined" ? DashboardType : "";
        MainModel.MRNTypeBack = MRNType != null && MRNType != "0" && MRNType != "undefined" ? MRNType : "";
        MainModel.DocumentNameBack = DocumentName != null && DocumentName != "0" && DocumentName != "undefined" ? DocumentName : "";
        MainModel.VendorNameBack = VendorName != null && VendorName != "0" && VendorName != "undefined" ? VendorName : "";
        MainModel.VoucherNoBack = VoucherNo != null && VoucherNo != "0" && VoucherNo != "undefined" ? VoucherNo : "";
        MainModel.InvoiceNoBack = InvoiceNo != null && InvoiceNo != "0" && InvoiceNo != "undefined" ? InvoiceNo : "";
        MainModel.MRNNoBack = MRNNo != null && MRNNo != "0" && MRNNo != "undefined" ? MRNNo : "";
        MainModel.GateNoBack = GateNo != null && GateNo != "0" && GateNo != "undefined" ? GateNo : "";
        MainModel.PartCodeBack = PartCode != null && PartCode != "0" && PartCode != "undefined" ? PartCode : "";
        MainModel.ItemNameBack = ItemName != null && ItemName != "0" && ItemName != "undefined" ? ItemName : "";
        MainModel.HSNNoBack = HSNNo != null && HSNNo != "0" && HSNNo != "undefined" ? HSNNo : "";
        MainModel.GlobalSearchBack = Searchbox != null && Searchbox != "0" && Searchbox != "undefined" ? Searchbox : "";

        MainModel.ItemNetAmount = MainModel.ItemDetailGridd?.Sum(x => Convert.ToDecimal(x.Amount ?? 0)) ?? 0;
        HttpContext.Session.SetString("PurchaseBill", JsonConvert.SerializeObject(MainModel));
        HttpContext.Session.SetString("KeyTaxGrid", JsonConvert.SerializeObject(
    MainModel.TaxDetailGridd ?? new List<TaxModel>()));
        HttpContext.Session.SetString("KeyTDSGrid", JsonConvert.SerializeObject(
    MainModel.TDSDetailGridd ?? new List<TDSModel>()
));
        HttpContext.Session.SetString("KeyAdjGrid", JsonConvert.SerializeObject(
    MainModel.adjustmentModel ?? new AdjustmentModel()
));
        MainModel.adjustmentModel = (MainModel.adjustmentModel != null && MainModel.adjustmentModel.AdjAdjustmentDetailGrid != null) ? MainModel.adjustmentModel : new AdjustmentModel();
        MainModel.adjustmentModel.AdjAdjustmentDetailGrid = (MainModel.adjustmentModel.AdjAdjustmentDetailGrid != null) ? MainModel.adjustmentModel.AdjAdjustmentDetailGrid : new List<AdjustmentModel>();
        MainModel.ItemDetailGridd = (MainModel.ItemDetailGridd != null) ? MainModel.ItemDetailGridd : new List<PBItemDetail>();
        HttpContext.Session.SetString("PurchaseBill", JsonConvert.SerializeObject(MainModel));
        return View(MainModel);
    }
    public IActionResult AddtoItemGrid(IList<PBItemDetail> gridList)
    {
        try
        {
            var MainModel = new PurchaseBillModel();
            var ItemGrid = new List<PBItemDetail>();
            MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                SlidingExpiration = TimeSpan.FromMinutes(55),
                Size = 1024,
            };
            var seqNo = 1;
            if (gridList != null)
            {
                string modelJson = HttpContext.Session.GetString("PurchaseBill");
                PurchaseBillModel model = !string.IsNullOrEmpty(modelJson)
    ? JsonConvert.DeserializeObject<PurchaseBillModel>(modelJson)
    : new PurchaseBillModel();
                if (model != null)
                {
                    MainModel = model;
                    if (model != null && model.ItemDetailGridd == null || model.ItemDetailGridd.Count() == 0)
                    {
                        ItemGrid = new List<PBItemDetail>();
                    }
                    else { }
                }
                else { }

                if (model != null && model.ItemDetailGridd != null)
                {
                    if (1 != 1 /*model.ItemDetailGridd.Where(x => x.ItemCode == item.ItemCode).Any()*/)
                    {
                        return StatusCode(207, "Duplicate");
                    }
                    else
                    {
                        seqNo = model.ItemDetailGridd.Count + 1;
                        ItemGrid = model.ItemDetailGridd.ToList();
                    }
                }

                foreach (var item in gridList)
                {
                    if (item != null)
                    {
                        item.SeqNo = seqNo;
                        ItemGrid.Add(item);
                        seqNo++;
                    }
                }
            }
            MainModel.ItemDetailGridd = ItemGrid;
            MainModel.ItemDetailGrid = null;
            MainModel.ItemNetAmount = MainModel.ItemDetailGridd?.Sum(x => Convert.ToDecimal(x.Amount ?? 0)) ?? 0;
            HttpContext.Session.SetString("PurchaseBill", JsonConvert.SerializeObject(MainModel));
            return PartialView("_PBItemGrid", MainModel);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
    //public IActionResult DeletItemRow(int SeqNo)
    //{
    //    IMemoryCache.TryGetValue("PurchaseBill", out PurchaseBillModel model);

    //    var ItemGrid = model?.ItemDetailGridd?.Where(x => x.SeqNo == SeqNo);
    //    string JsonString = JsonConvert.SerializeObject(ItemGrid);
    //    return Json(JsonString);
    //}
    public IActionResult EditItemRow(int SeqNo)
    {
        var MainModel = new PurchaseBillModel();
        string modelJson = HttpContext.Session.GetString("PurchaseBill");

        PurchaseBillModel PBItemGrid = !string.IsNullOrEmpty(modelJson)
            ? JsonConvert.DeserializeObject<PurchaseBillModel>(modelJson)
            : new PurchaseBillModel();
        int Indx = Convert.ToInt32(SeqNo) - 1;
        var docName = string.Empty;
        var docId = 0;

        if (PBItemGrid != null && PBItemGrid.ItemDetailGridd?.Count > 0)
        {
            if (PBItemGrid.ItemDetailGrid == null)
            {
                PBItemGrid.ItemDetailGrid = new List<PBItemDetail>();
            }
            PBItemGrid.ItemDetailGrid.Add(PBItemGrid.ItemDetailGridd.Where(x => x.SeqNo == SeqNo).FirstOrDefault());

            docName = PBItemGrid.ItemDetailGridd?.Where(x => x.SeqNo == SeqNo).Select(a => a.DocTypeText).FirstOrDefault();
            docId = PBItemGrid.ItemDetailGridd?.Where(x => x.SeqNo == SeqNo).Select(a => a.DocTypeID).FirstOrDefault() ?? 0;

            int index = PBItemGrid.ItemDetailGridd?.ToList().FindIndex(x => x.SeqNo == SeqNo) ?? -1;
            PBItemGrid.ItemDetailGridd?.RemoveAt(index);

            Indx = 0;
            foreach (var item in PBItemGrid.ItemDetailGridd)
            {
                Indx++;
                item.SeqNo = Indx;
            }
            MainModel.ItemDetailGridd = PBItemGrid.ItemDetailGridd;
            MainModel.ItemDetailGrid = PBItemGrid.ItemDetailGrid;

            MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                SlidingExpiration = TimeSpan.FromMinutes(55),
                Size = 1024,
            };
            MainModel.ItemNetAmount = MainModel.ItemDetailGridd?.Sum(x => Convert.ToDecimal(x.Amount ?? 0)) ?? 0;
            HttpContext.Session.SetString("PurchaseBill", JsonConvert.SerializeObject(MainModel));
        }
        //return PartialView("_PBItemGrid", MainModel);
        var partialView1 = RenderPartialView("_PBItemGrid", MainModel);
        var partialView2 = RenderPartialView("_PBItemDetail", MainModel);

        // Return JSON object with rendered views
        return Json(new { partialView1 = partialView1, partialView2 = partialView2, docName = docName, docId = docId });
    }
    private string RenderPartialView(string viewName, object model)
    {
        ViewData.Model = model;
        using var sw = new StringWriter();
        var viewResult = _viewEngine.FindView(ControllerContext, viewName, false);
        if (viewResult.View == null)
        {
            throw new ArgumentNullException($"View {viewName} not found.");
        }
        var viewContext = new ViewContext(
            ControllerContext,
            viewResult.View,
            ViewData,
            TempData,
            sw,
            new HtmlHelperOptions()
        );
        viewResult.View.RenderAsync(viewContext).Wait();
        return sw.GetStringBuilder().ToString();
    }

    public IActionResult DeleteFromMemoryGrid(int SeqNo)
    {
        var MainModel = new PurchaseBillModel();
        string modelJson = HttpContext.Session.GetString("PurchaseBill");

        PurchaseBillModel PBItemGrid = !string.IsNullOrEmpty(modelJson)
    ? JsonConvert.DeserializeObject<PurchaseBillModel>(modelJson)
    : new PurchaseBillModel();
        int Indx = Convert.ToInt32(SeqNo) - 1;

        if (PBItemGrid != null && PBItemGrid.ItemDetailGrid.Count > 0)
        {
            PBItemGrid.ItemDetailGrid.RemoveAt(Convert.ToInt32(Indx));

            Indx = 0;

            foreach (var item in PBItemGrid.ItemDetailGrid)
            {
                Indx++;
                item.SeqNo = Indx;
            }
            MainModel.ItemDetailGrid = PBItemGrid.ItemDetailGrid;

            MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                SlidingExpiration = TimeSpan.FromMinutes(55),
                Size = 1024,
            };


            if (PBItemGrid.ItemDetailGrid.Count == 0)
            {
                HttpContext.Session.Remove("PurchaseBill");
            }
            MainModel.ItemNetAmount = MainModel.ItemDetailGridd?.Sum(x => Convert.ToDecimal(x.Amount ?? 0)) ?? 0;
            HttpContext.Session.SetString("PurchaseBill", JsonConvert.SerializeObject(MainModel));
        }
        return PartialView("_PBItemDetail", MainModel);
    }
    public JsonResult ResetGridItems()
    {
        HttpContext.Session.Remove("POItemList");
        HttpContext.Session.Remove("PurchaseBill");
        HttpContext.Session.Remove("KeyTaxGrid");
        HttpContext.Session.Remove("KeyTDSGrid");

        var MainModel = new PurchaseBillModel();
        List<TaxModel> taxList = new List<TaxModel>();
        List<TDSModel> tdsList = new List<TDSModel>();

        MainModel.YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
        MainModel.FinFromDate = HttpContext.Session.GetString("FromDate");
        MainModel.FinToDate = HttpContext.Session.GetString("ToDate");
        MainModel.PreparedBy = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
        MainModel.PreparedByName = GetEmpByMachineName();
        MainModel.Branch = HttpContext.Session.GetString("Branch");

        HttpContext.Session.SetString("PurchaseBill", JsonConvert.SerializeObject(MainModel));
        HttpContext.Session.SetString("KeyTaxGrid", JsonConvert.SerializeObject(taxList));
        HttpContext.Session.SetString("KeyTDSGrid", JsonConvert.SerializeObject(tdsList));
        string purchaseBillJson = HttpContext.Session.GetString("PurchaseBill");
        PurchaseBillModel MainModel1 = !string.IsNullOrEmpty(purchaseBillJson)
            ? JsonConvert.DeserializeObject<PurchaseBillModel>(purchaseBillJson)
            : new PurchaseBillModel();
        string taxGridJson = HttpContext.Session.GetString("KeyTaxGrid");
        List<TaxModel> TaxGrid = !string.IsNullOrEmpty(taxGridJson)
            ? JsonConvert.DeserializeObject<List<TaxModel>>(taxGridJson)
            : new List<TaxModel>();
        string tdsGridJson = HttpContext.Session.GetString("KeyTDSGrid");
        List<TDSModel> TdsGrid = !string.IsNullOrEmpty(tdsGridJson)
            ? JsonConvert.DeserializeObject<List<TDSModel>>(tdsGridJson)
            : new List<TDSModel>();

        return new(StatusCodes.Status200OK);
    }

    // POST: PurchaseOrderController
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("{controller}/Index")]
    public async Task<IActionResult> SavePurchaseBILL(PurchaseBillModel model)
    {
        try
        {
            bool isError = true;
            DataSet DS = new();
            DataTable ItemDetailDT = null;
            DataTable TaxDetailDT = null;
            DataTable TDSDetailDT = null;
            DataTable DrCrDetailDT = null;
            DataTable AdjDetailDT = null;
            ResponseResult Result = new();
            DataTable DelieveryScheduleDT = null;
            Dictionary<string, string> ErrList = new();
            string modePOA = "data";
            var stat = new MemoryCacheStatistics();

            string purchaseBillJson = HttpContext.Session.GetString("PurchaseBill");
            PurchaseBillModel MainModel = purchaseBillJson != null ? JsonConvert.DeserializeObject<PurchaseBillModel>(purchaseBillJson) : null;

            string taxGridJson = HttpContext.Session.GetString("KeyTaxGrid");
            List<TaxModel> TaxGrid = taxGridJson != null ? JsonConvert.DeserializeObject<List<TaxModel>>(taxGridJson) : null;

            string tdsGridJson = HttpContext.Session.GetString("KeyTDSGrid");
            List<TDSModel> TdsGrid = tdsGridJson != null ? JsonConvert.DeserializeObject<List<TDSModel>>(tdsGridJson) : null;

            string drCrGridJson = HttpContext.Session.GetString("KeyDrCrGrid");
            List<DbCrModel> DrCrGrid = drCrGridJson != null ? JsonConvert.DeserializeObject<List<DbCrModel>>(drCrGridJson) : null;

            string AdjGridJson = HttpContext.Session.GetString("KeyAdjGrid");
            PBItemDetail AdjGrid=  JsonConvert.DeserializeObject<PBItemDetail>(AdjGridJson);

            string serializedGrid = HttpContext.Session.GetString("KeyAdjGrid");
            AdjustmentModel adjustmentModel = JsonConvert.DeserializeObject<AdjustmentModel>(serializedGrid);
            List<AdjustmentModel> gridData = adjustmentModel.AdjAdjustmentDetailGrid;


            var cc = stat.CurrentEntryCount;
            var pp = stat.CurrentEstimatedSize;

            ModelState.Clear();

            if (MainModel.ItemDetailGridd != null && MainModel.ItemDetailGridd.Count > 0)
            {
                DS = GetItemDetailTable(MainModel.ItemDetailGridd, model.Mode, MainModel.EntryID, MainModel.YearCode);
                ItemDetailDT = DS.Tables[0];
                model.ItemDetailGridd = MainModel.ItemDetailGridd;

                isError = false;
                if (MainModel.ItemDetailGridd != null && MainModel.ItemDetailGridd.Any())
                {
                    var hasDupes = MainModel.ItemDetailGridd.GroupBy(x => new { x.ItemCode, x.DocTypeID, x.Description })
                   .Where(x => x.Skip(1).Any()).Any();
                    if (hasDupes)
                    {
                        isError = true;
                        ErrList.Add("ItemDetailGridd", "Document Type + ItemCode + Description In ItemDetails can not be Duplicate...!");
                    }
                }
            }
            else
            {
                ErrList.Add("ItemDetailGridd", "Item Details Cannot Be Blank..!");
            }

            if (TaxGrid != null && TaxGrid.Count > 0)
            {
                TaxDetailDT = GetTaxDetailTable(TaxGrid);
            }

            if (TdsGrid != null && TdsGrid.Count > 0)
            {
                TDSDetailDT = GetTDSDetailTable(TdsGrid, MainModel);
            }

            if (DrCrGrid != null && DrCrGrid.Count > 0)
            {
                DrCrDetailDT = CommonController.GetDrCrDetailTable(DrCrGrid);
            }

           
                //AdjDetailDT = CommonController.GetAdjDetailTable(MainModel.adjustmentModel.AdjAdjustmentDetailGrid.ToList(), MainModel.EntryID, MainModel.YearCode, MainModel.AccountCode);
                AdjDetailDT = CommonController.GetAdjDetailTable(gridData, MainModel.EntryID, MainModel.YearCode, MainModel.AccountCode);
            

            if (model.PreparedBy == 0)
            {
                ErrList.Add("PreparedBy", "Please Select Prepared By From List..!");
            }

            if (!isError)
            {
                if (ItemDetailDT.Rows.Count > 0 || TaxDetailDT.Rows.Count > 0)
                {
                    if (model.Mode == "U")
                    {
                        model.Mode = "UPDATE";
                    }
                    else if (model.Mode == "C")
                    {
                        model.Mode = "COPY";
                    }
                    else if (model.Mode == "POA")
                    {
                        model.Mode = "POA";
                        modePOA = "POA";
                    }
                    else if (model.Mode == "PAU")
                    {
                        model.Mode = "UPDATE";
                    }
                    else
                    {
                        model.Mode = "INSERT";
                    }
                    //model.Mode = model.Mode == "U" ? "UPDATE" : "INSERT";

                    if (model.PathOfFile1 != null)
                    {
                        string ImagePath = "Uploads/PurchaseBill/";

                        if (!Directory.Exists(_IWebHostEnvironment.WebRootPath + "\\" + ImagePath))
                        {
                            Directory.CreateDirectory(_IWebHostEnvironment.WebRootPath + "\\" + ImagePath);
                        }

                        ImagePath += Guid.NewGuid().ToString() + "_" + model.PathOfFile1.FileName;
                        model.PathOfFile1URL = "/" + ImagePath;
                        string ServerPath = Path.Combine(_IWebHostEnvironment.WebRootPath, ImagePath);
                        using (FileStream FileStream = new FileStream(ServerPath, FileMode.Create))
                        {
                            await model.PathOfFile1.CopyToAsync(FileStream);
                        }
                    }

                    if (model.PathOfFile2 != null)
                    {
                        string ImagePath = "Uploads/PurchaseBill/";

                        if (!Directory.Exists(_IWebHostEnvironment.WebRootPath + "\\" + ImagePath))
                        {
                            Directory.CreateDirectory(_IWebHostEnvironment.WebRootPath + "\\" + ImagePath);
                        }

                        ImagePath += Guid.NewGuid().ToString() + "_" + model.PathOfFile2.FileName;
                        model.PathOfFile2URL = "/" + ImagePath;
                        string ServerPath = Path.Combine(_IWebHostEnvironment.WebRootPath, ImagePath);
                        using (FileStream FileStream = new FileStream(ServerPath, FileMode.Create))
                        {
                            await model.PathOfFile2.CopyToAsync(FileStream);
                        }
                    }

                    if (model.PathOfFile3 != null)
                    {
                        string ImagePath = "Uploads/PurchaseBill/";

                        if (!Directory.Exists(_IWebHostEnvironment.WebRootPath + "\\" + ImagePath))
                        {
                            Directory.CreateDirectory(_IWebHostEnvironment.WebRootPath + "\\" + ImagePath);
                        }

                        ImagePath += Guid.NewGuid().ToString() + "_" + model.PathOfFile3.FileName;
                        model.PathOfFile3URL = "/" + ImagePath;
                        string ServerPath = Path.Combine(_IWebHostEnvironment.WebRootPath, ImagePath);
                        using (FileStream FileStream = new FileStream(ServerPath, FileMode.Create))
                        {
                            await model.PathOfFile3.CopyToAsync(FileStream);
                        }
                    }

                    model.FinFromDate = HttpContext.Session.GetString("FromDate");
                    model.FinToDate = HttpContext.Session.GetString("ToDate");
                    model.YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
                    model.Branch = HttpContext.Session.GetString("Branch");
                    model.EntryByMachineName = GetEmpByMachineName();
                    model.PreparedByName = GetEmpByMachineName();
                    model.CreatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                    Result = await IPurchaseBill.SavePurchaseBILL(ItemDetailDT, TaxDetailDT, TDSDetailDT, model, DrCrDetailDT, AdjDetailDT);
                }

                if (Result != null)
                {
                    if (Result.StatusText == "Success" && Result.StatusCode == HttpStatusCode.OK)
                    {
                        ViewBag.isSuccess = true;
                        TempData["200"] = "200";
                        HttpContext.Session.Remove("PurchaseBill");
                        HttpContext.Session.Remove("KeyTaxGrid");
                        HttpContext.Session.Remove("KeyTDSGrid");
                    }
                    else if (Result.StatusText == "Inserted Successfully" && Result.StatusCode == HttpStatusCode.Accepted)
                    {
                        ViewBag.isSuccess = true;
                        TempData["200"] = "200";
                        HttpContext.Session.Remove("PurchaseBill");
                        HttpContext.Session.Remove("KeyTaxGrid");
                        HttpContext.Session.Remove("KeyTDSGrid");
                    }
                    else if (Result.StatusText == "Updated Successfully" && Result.StatusCode == HttpStatusCode.Accepted)
                    {
                        ViewBag.isSuccess = true;
                        TempData["202"] = "202";
                        HttpContext.Session.Remove("PurchaseBill");
                        HttpContext.Session.Remove("KeyTaxGrid");
                        HttpContext.Session.Remove("KeyTDSGrid");
                        return RedirectToAction(nameof(PurchaseBillList));
                    }
                    else if (Result.StatusText == "Deleted Successfully" && Result.StatusCode == HttpStatusCode.Accepted)
                    {
                        ViewBag.isSuccess = true;
                        TempData["410"] = "410";
                        HttpContext.Session.Remove("PurchaseBill");
                        HttpContext.Session.Remove("KeyTaxGrid");
                        HttpContext.Session.Remove("KeyTDSGrid");
                    }
                    else if (Result.StatusText == "Error" && Result.StatusCode == HttpStatusCode.InternalServerError)
                    {
                        var errNum = Result.ToString(); //.Result.Message.ToString().Split(":")[1];
                        if (errNum == " 2627")
                        {
                            ViewBag.isSuccess = false;
                            ViewBag.ResponseResult = Result.StatusCode + "Occurred while saving data";
                            TempData["2627"] = "2627";
                            _Logger.LogError("\n \n ********** LogError ********** \n " + JsonConvert.SerializeObject(Result) + "\n \n");
                            var model2 = await BindModels(model);
                            model2.FinFromDate = HttpContext.Session.GetString("FromDate");
                            model2.FinToDate = HttpContext.Session.GetString("ToDate");
                            model2.YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
                            model2.Branch = HttpContext.Session.GetString("Branch");
                            model2.PreparedByName = GetEmpByMachineName();
                            model2.UpdatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                            model2.UpdatedByName = GetEmpByMachineName();
                            model2.CreatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                            return View("PurchaseBill", model);
                        }
                        else
                        {
                            ViewBag.ResponseResult = Result.StatusCode + "Occurred while saving data";
                            TempData["500"] = "500";
                            model = await BindModels(model);
                            model.adjustmentModel = model.adjustmentModel ?? new AdjustmentModel();
                            model.FinFromDate = HttpContext.Session.GetString("FromDate");
                            model.FinToDate = HttpContext.Session.GetString("ToDate");
                            model.YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
                            model.Branch = HttpContext.Session.GetString("Branch");
                            model.PreparedByName = GetEmpByMachineName();
                            model.UpdatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                            model.UpdatedByName = GetEmpByMachineName();
                            model.CreatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                            return View("PurchaseBill", model);
                        }
                    }
                    else
                    {
                        model = await BindModels(model);
                        model.adjustmentModel = model.adjustmentModel ?? new AdjustmentModel();
                        model.FinFromDate = HttpContext.Session.GetString("FromDate");
                        model.FinToDate = HttpContext.Session.GetString("ToDate");
                        model.YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
                        model.Branch = HttpContext.Session.GetString("Branch");
                        model.PreparedByName = GetEmpByMachineName();
                        model.UpdatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                        model.UpdatedByName = GetEmpByMachineName();
                        model.CreatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                        if (Result.StatusText.Contains("success") && (Result.StatusCode == HttpStatusCode.OK || Result.StatusCode == HttpStatusCode.Accepted))
                        {
                            ViewBag.isSuccess = true;
                            TempData["202"] = "202";
                            HttpContext.Session.Remove("PurchaseBill");
                            HttpContext.Session.Remove("KeyTaxGrid");
                            HttpContext.Session.Remove("KeyTDSGrid");
                            return RedirectToAction(nameof(PurchaseBillList));
                        }
                        else
                        {
                            TempData["ErrorMessage"] = Result.StatusText;
                            HttpContext.Session.Remove("KeyTaxGrid");
                            HttpContext.Session.Remove("KeyTDSGrid");
                            return View("PurchaseBill", model);
                        }
                    }
                }
                var model1 = await BindModels(model);
                model1.adjustmentModel = model.adjustmentModel ?? new AdjustmentModel();
                model1.FinFromDate = HttpContext.Session.GetString("FromDate");
                model1.FinToDate = HttpContext.Session.GetString("ToDate");
                model1.YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
                model1.Branch = HttpContext.Session.GetString("Branch");
                model1.PreparedBy = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
                model1.PreparedByName = GetEmpByMachineName();
                model1.UpdatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                model1.UpdatedByName = GetEmpByMachineName();
                model1.CreatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));

                return RedirectToAction(nameof(PurchaseBillList));

            }
            else
            {
                model = await BindModels(model);
                model.adjustmentModel = model.adjustmentModel ?? new AdjustmentModel();
                foreach (KeyValuePair<string, string> Err in ErrList)
                {
                    ModelState.AddModelError(Err.Key, Err.Value);
                }
            }
        }
        catch (Exception ex)
        {
            Debug.Print(ex.Message);
            _Logger.LogError("\n \n" + ex, ex.Message, model);

            var ResponseResult = new ResponseResult()
            {
                StatusCode = HttpStatusCode.InternalServerError,
                StatusText = "Error",
                Result = ex
            };
            ViewBag.ResponseResult = ResponseResult;
            return View("Error", ResponseResult);
        }
        model = await BindModels(model);
        model.adjustmentModel = model.adjustmentModel ?? new AdjustmentModel();
        return View("PurchaseBill", model);
    }

    private static DataSet GetItemDetailTable(IList<PBItemDetail> itemDetailList, string Mode, int? EntryID, int? YearCode)
    {
        DataSet DS = new();
        DataTable Table = new();

        #region
        Table.Columns.Add("PurchBillEntryID", typeof(int));
        Table.Columns.Add("PurchBillYearCode", typeof(int));
        Table.Columns.Add("SeqNo", typeof(int));
        Table.Columns.Add("Parentcode", typeof(int));
        Table.Columns.Add("DocTypeID", typeof(int));
        Table.Columns.Add("ItemCode", typeof(int));
        Table.Columns.Add("Unit", typeof(string));
        Table.Columns.Add("NoOfCase", typeof(float));
        Table.Columns.Add("BillQty", typeof(float));
        Table.Columns.Add("RecQty", typeof(float));
        Table.Columns.Add("RejectedQty", typeof(float));
        Table.Columns.Add("AltQty", typeof(float));
        Table.Columns.Add("AltUnit", typeof(string));
        Table.Columns.Add("Rate", typeof(float));
        Table.Columns.Add("MRP", typeof(float));
        Table.Columns.Add("RateUnit", typeof(string));
        Table.Columns.Add("RateIncludingTaxes", typeof(float));
        Table.Columns.Add("AmtinOtherCurr", typeof(float));
        Table.Columns.Add("RateConversionFactor", typeof(float));
        Table.Columns.Add("CostCenterId", typeof(int));
        Table.Columns.Add("AssesRate", typeof(float));
        Table.Columns.Add("AssesAmount", typeof(float));
        Table.Columns.Add("DiscountPer", typeof(float));
        Table.Columns.Add("DiscountAmt", typeof(float));
        Table.Columns.Add("Amount", typeof(float));
        Table.Columns.Add("Itemsize", typeof(string));
        Table.Columns.Add("ItemColor", typeof(string));
        Table.Columns.Add("ItemModel", typeof(string));
        Table.Columns.Add("Deaprtmentid", typeof(int));
        Table.Columns.Add("OtherDetail", typeof(string));
        Table.Columns.Add("DebitNoteType", typeof(string));
        Table.Columns.Add("ProcessId", typeof(int));
        Table.Columns.Add("NewPoRate", typeof(float));
        Table.Columns.Add("PONo", typeof(string));
        Table.Columns.Add("POYearCode", typeof(int));
        Table.Columns.Add("PODate", typeof(DateTime));
        Table.Columns.Add("SchNo", typeof(string));
        Table.Columns.Add("SchYearCode", typeof(int));
        Table.Columns.Add("SchDate", typeof(DateTime));
        Table.Columns.Add("POAmmNo", typeof(string));
        Table.Columns.Add("PoRate", typeof(float));
        Table.Columns.Add("POType", typeof(string));
        Table.Columns.Add("MIRNO", typeof(string));
        Table.Columns.Add("MIRYearCode", typeof(int));
        Table.Columns.Add("MIRDate", typeof(DateTime));
        Table.Columns.Add("AllowDebitNote", typeof(string));
        Table.Columns.Add("DebitNotePending", typeof(string));
        Table.Columns.Add("ProjectNo", typeof(string));
        Table.Columns.Add("ProjectDate", typeof(DateTime));
        Table.Columns.Add("ProjectYearCode", typeof(int));
        Table.Columns.Add("AgainstImportAccountCode", typeof(int));
        Table.Columns.Add("AgainstImportInvoiceNo", typeof(string));
        Table.Columns.Add("AgainstImportYearCode", typeof(int));
        Table.Columns.Add("AgainstImportInvDate", typeof(DateTime));
        Table.Columns.Add("HSNNO", typeof(string));
        Table.Columns.Add("AcceptedQty", typeof(float));
        Table.Columns.Add("ReworkQty", typeof(float));
        Table.Columns.Add("HoldQty", typeof(float));
        #endregion

        foreach (PBItemDetail Item in itemDetailList)
        {
            DateTime poDate = ParseSafeDate(Item.PODate);
            DateTime schDate = ParseSafeDate(Item.SchDate);
            DateTime mirDate = ParseSafeDate(Item.MIRDATE);
            DateTime projectDate = ParseSafeDate(Item.ProjectDate);
            DateTime againstImportInvDate = ParseSafeDate(Item.AgainstImportInvDate);

            Table.Rows.Add(
            new object[]
            {
                EntryID ?? 0,
                YearCode ?? 0,
                Item.SeqNo,
                0,
                Item.DocTypeID > 0 ? Convert.ToInt32(Item.DocTypeID) : 0,
                Item.ItemCode ?? 0,
                Item.Unit ?? string.Empty,
                Item.NoOfCase > 0 ? Convert.ToInt32(Item.NoOfCase ?? 0) : 0,
                Item.BillQty > 0 ? Math.Round(Convert.ToDecimal(Item.BillQty), 2) : 0,
                Item.RecQty > 0 ? Math.Round(Item.RecQty ?? 0, 2) : 0,
                Item.rejectedQty > 0 ? Math.Round(Convert.ToDecimal(Item.rejectedQty ?? 0), 2) : 0,
                Item.AltRecQty > 0 ? Math.Round(Convert.ToDecimal(Item.AltRecQty ?? 0), 2) : 0,
                Item.AltUnit ?? string.Empty,
                Item.BillRate > 0 ? Math.Round(Convert.ToDecimal(Item.BillRate ?? 0), 2) : 0,
                Item.MRP > 0 ? Math.Round(Convert.ToDecimal(Item.MRP ?? 0), 2) : 0,
                Item.UnitRate ?? string.Empty,
                Item.RateIncludingTax > 0 ? Math.Round(Convert.ToDecimal(Item.RateIncludingTax ?? 0), 2) : 0,
                Item.OtherRateCurr > 0 ? Math.Round(Item.OtherRateCurr, 2) : 0,
                Item.RateOfConvFactor > 0 ? Convert.ToInt32(Item.RateOfConvFactor ?? 0) : 0,
                Item.CostCenter > 0 ? Item.CostCenter : 0,
                Item.AssessRate > 0 ? Math.Round(Convert.ToDecimal(Item.AssessRate ?? 0), 2) : 0,
                0f,
                Item.DisPer > 0 ? Math.Round(Convert.ToDecimal(Item.DisPer), 2) : 0,
                Item.DisAmt > 0 ? Math.Round(Convert.ToDecimal(Item.DisAmt), 2) : 0,
                Item.Amount > 0 ? Math.Round(Convert.ToDecimal(Item.Amount ?? 0), 2) : 0,
                Item.ItemSize ?? string.Empty,
                Item.Color ?? string.Empty,
                Item.ItemModel ?? string.Empty,
                Item.DepartmentId > 0 ? Convert.ToInt32(Item.DepartmentId) : 0,
                Item.Description ?? string.Empty,
                string.Empty,
                Item.Process > 0 ? Item.Process : 0,
                Item.NewPoRate,
                Item.pono ?? string.Empty,
                Item.poyearcode ?? 0,
                poDate,
                Item.schno ?? string.Empty,
                Item.schyearcode ?? 0,
                schDate,
                Item.PoAmendNo?.ToString() ?? string.Empty,
                Item.PORate > 0 ? Math.Round(Convert.ToDecimal(Item.PORate ?? 0), 2) : 0,
                string.Empty,
                Item.MIRNO ?? string.Empty,
                Item.MIRYEARCODE > 0 ? Convert.ToInt32(Item.MIRYEARCODE) : 0,
                mirDate,
                string.Empty,
                string.Empty,
                Item.ProjectNo ?? string.Empty,
                projectDate,
                Item.ProjectyearCode > 0 ? Convert.ToInt32(Item.ProjectyearCode) : 0,
                Item.AgainstImportAccountCode > 0 ? Convert.ToInt32(Item.AgainstImportAccountCode) : 0,
                Item.AgainstImportInvoiceNo?.ToString() ?? string.Empty,
                Item.AgainstImportYearCode > 0 ? Convert.ToInt32(Item.AgainstImportYearCode) : 0,
                againstImportInvDate,
                Item.HSNNO.ToString(),
                Item.AcceptedQty > 0 ? Math.Round(Convert.ToDecimal(Item.AcceptedQty ?? 0), 2) : 0,
                Item.ReworkQty > 0 ? Math.Round(Convert.ToDecimal(Item.ReworkQty ?? 0), 2) : 0,
                Item.HoldQty > 0 ? Math.Round(Convert.ToDecimal(Item.HoldQty ?? 0), 2) : 0
            });
        }

        DS.Tables.Add(Table);
        return DS;
    }

    private static DataTable GetTaxDetailTable(List<TaxModel> TaxDetailList)
    {
        DataTable Table = new();
        Table.Columns.Add("SeqNo", typeof(int));
        Table.Columns.Add("[Type]", typeof(string));
        Table.Columns.Add("ItemCode", typeof(int));
        Table.Columns.Add("TaxTypeID", typeof(int));
        Table.Columns.Add("TaxAccountCode", typeof(string));
        Table.Columns.Add("TaxPercentg", typeof(float));
        Table.Columns.Add("AddInTaxable", typeof(char));
        Table.Columns.Add("RountOff", typeof(string));
        Table.Columns.Add("Amount", typeof(float));
        Table.Columns.Add("TaxRefundable", typeof(char));
        Table.Columns.Add("TaxonExp", typeof(string));
        Table.Columns.Add("Remark", typeof(string));

        if (TaxDetailList != null && TaxDetailList.Count > 0)
        {
            var groupedTaxDetails = TaxDetailList
                .GroupBy(item => item.TxItemCode)
                .Select(group => new
                {
                    FirstItem = group.First(),
                    TotalAmount = group.Sum(item => item.TxAmount)
                });
            int rowNo = 1;
            foreach (var group in groupedTaxDetails)
            {
                var Item = group.FirstItem;
                Table.Rows.Add(
                new object[]
                {
                    rowNo++,
                    Item.TxType ?? string.Empty,
                    Item.TxItemCode,
                    Item.TxTaxType,
                    Item.TxAccountCode,
                    Item.TxPercentg,
                    !string.IsNullOrEmpty(Item.TxAdInTxable) && Item.TxAdInTxable.Length == 1 ? Convert.ToChar(Item.TxAdInTxable) : 'N',
                    Item.TxRoundOff,
                    //Math.Round(Item.TxAmount, 2, MidpointRounding.AwayFromZero),
                    Math.Round(group.TotalAmount, 2, MidpointRounding.AwayFromZero),
                    !string.IsNullOrEmpty(Item.TxRefundable) && Item.TxRefundable.Length == 1 ? Convert.ToChar(Item.TxRefundable) : 'N',
                    Item.TxOnExp,
                    Item.TxRemark,
                    });
            }
        }

        return Table;
    }
    private static DataTable GetTDSDetailTable(List<TDSModel> TDSDetailList, PurchaseBillModel MainModel)
    {
        DataTable Table = new();

        #region Columns
        Table.Columns.Add("PurchBillEntryId", typeof(int));
        Table.Columns.Add("PurchBillYearCode", typeof(int));
        Table.Columns.Add("SeqNo", typeof(int));
        Table.Columns.Add("InvoiceNo", typeof(string));
        Table.Columns.Add("InvoiceDate", typeof(DateTime));
        Table.Columns.Add("PurchVoucherNo", typeof(string));
        Table.Columns.Add("AccountCode", typeof(int));
        Table.Columns.Add("TaxTypeID", typeof(int));
        Table.Columns.Add("TaxNameCode", typeof(int));
        Table.Columns.Add("TaxPer", typeof(float));
        Table.Columns.Add("RoundOff", typeof(string));
        Table.Columns.Add("TDSAmount", typeof(float));
        Table.Columns.Add("InvBasicAmt", typeof(float));
        Table.Columns.Add("InvNetAmt", typeof(float));
        Table.Columns.Add("Remark", typeof(string));
        Table.Columns.Add("TypePBDirectPBVouch", typeof(string));
        Table.Columns.Add("BankChallanNo", typeof(string));
        Table.Columns.Add("challanDate", typeof(DateTime));
        Table.Columns.Add("BankVoucherNo", typeof(string));
        Table.Columns.Add("BankVoucherDate", typeof(DateTime));
        Table.Columns.Add("BankVouchEntryId", typeof(int));
        Table.Columns.Add("BankYearCode", typeof(int));
        Table.Columns.Add("RemainingAmt", typeof(float));
        Table.Columns.Add("RoundoffAmt", typeof(float));
        #endregion

        if (TDSDetailList != null && TDSDetailList.Count > 0)
        {
            foreach (TDSModel Item in TDSDetailList)
            {
                DateTime invoiceDate = ParseSafeDate(MainModel.InvDate);
                DateTime challanDate = DateTime.Today;
                DateTime bankVoucherDate = DateTime.Today;

                Table.Rows.Add(
                    new object[]
                    {
                    MainModel.EntryID > 0 ? MainModel.EntryID : 0,
                    MainModel.YearCode > 0 ? MainModel.YearCode : 0,
                    Item.TDSSeqNo,
                    MainModel.InvNo ?? string.Empty,
                    invoiceDate,
                    !string.IsNullOrEmpty(MainModel.PurchVouchNo) ? MainModel.PurchVouchNo : string.Empty,
                    MainModel.AccountCode,
                    Item.TDSTaxType,
                    Item.TDSAccountCode,
                    Item.TDSPercentg,
                    Item.TDSRoundOff,
                    Item.TDSAmount,
                    MainModel.ItemNetAmount,
                    MainModel.NetTotal,
                    Item.TDSRemark ?? string.Empty,
                    "PurchaseBill",
                    string.Empty,
                    challanDate,
                    string.Empty,
                    bankVoucherDate,
                    0,
                    0,
                    0f,
                    Item.TDSRoundOffAmt ?? 0
                    });
            }
        }

        return Table;
    }
    private static DataTable GetDbCrDetailTable(PurchaseBillModel MainModel)
    {
        DataTable Table = new();
        Table.Columns.Add("AccEntryId", typeof(int));
        Table.Columns.Add("AccYearCode", typeof(int));
        Table.Columns.Add("SeqNo", typeof(int));
        Table.Columns.Add("InvoiceNo", typeof(string));
        Table.Columns.Add("VoucherNo", typeof(string));
        Table.Columns.Add("AginstInvNo", typeof(string));
        Table.Columns.Add("AginstVoucherYearCode", typeof(int));
        Table.Columns.Add("AccountCode", typeof(int));
        Table.Columns.Add("DocTypeID", typeof(int));
        Table.Columns.Add("ItemCode", typeof(int));
        Table.Columns.Add("BillQty", typeof(float));
        Table.Columns.Add("Rate", typeof(float));
        Table.Columns.Add("DiscountPer", typeof(float));
        Table.Columns.Add("DiscountAmt", typeof(float));
        Table.Columns.Add("AccountAmount", typeof(float));
        Table.Columns.Add("DRCR", typeof(string));

        IList<PBItemDetail> itemDetailList = MainModel.ItemDetailGridd ?? MainModel.ItemDetailGrid;
        foreach (var Item in itemDetailList)
        {
            Table.Rows.Add(
            new object[]
            {
                    MainModel.EntryID,
                    MainModel.YearCode,
                    Item.SeqNo,
                    MainModel.InvNo ?? string.Empty,
                    MainModel.PurchVouchNo ?? string.Empty,
                    string.Empty,
                    0,
                    MainModel.AccountCode,
                    Item.DocTypeID > 0 ? Math.Round(Convert.ToDecimal(Item.DocTypeID), 2, MidpointRounding.AwayFromZero) : 0,
                    Item.ItemCode,
                    Math.Round(Convert.ToDecimal(Item.BillQty), 2, MidpointRounding.AwayFromZero),
                    Math.Round(Convert.ToDecimal(Item.BillRate ?? 0), 2, MidpointRounding.AwayFromZero),
                    Math.Round(Convert.ToDecimal(Item.DisPer), 2, MidpointRounding.AwayFromZero),
                    Math.Round(Convert.ToDecimal(Item.DisAmt), 2, MidpointRounding.AwayFromZero),
                    Math.Round(Convert.ToDecimal(Item.Amount ?? 0), 2, MidpointRounding.AwayFromZero),
                    "CR",
                });
        }

        return Table;
    }
    public async Task<JsonResult> GetDbCrDataGrid()
    {
        string purchaseBillJson = HttpContext.Session.GetString("PurchaseBill");
        PurchaseBillModel MainModel = null;
        if (!string.IsNullOrEmpty(purchaseBillJson))
        {
            MainModel = JsonConvert.DeserializeObject<PurchaseBillModel>(purchaseBillJson);
        }
        string taxGridJson = HttpContext.Session.GetString("KeyTaxGrid");
        List<TaxModel> TaxGrid = new List<TaxModel>();
        if (!string.IsNullOrEmpty(taxGridJson))
        {
            TaxGrid = JsonConvert.DeserializeObject<List<TaxModel>>(taxGridJson);
        }
        string tdsGridJson = HttpContext.Session.GetString("KeyTDSGrid");
        List<TDSModel> TdsGrid = new List<TDSModel>();
        if (!string.IsNullOrEmpty(tdsGridJson))
        {
            TdsGrid = JsonConvert.DeserializeObject<List<TDSModel>>(tdsGridJson);
        }
        DataTable DbCrGridd = new DataTable();
        DataTable TaxGridd = new DataTable();
        DataTable TdsGridd = new DataTable();

        DbCrGridd = GetDbCrDetailTable(MainModel);
        TaxGridd = GetTaxDetailTable(TaxGrid);
        TdsGridd = GetTDSDetailTable(TdsGrid, MainModel);

        var JSON = await IDataLogic.GetDbCrDataGrid(DbCrGridd, TaxGridd, TdsGridd, "PurchaseBill", MainModel.DocTypeID, MainModel.AccountCode, MainModel.ItemNetAmount, MainModel.NetTotal);
        string JsonString = JsonConvert.SerializeObject(JSON);
        return Json(JsonString);
    }
    private PurchaseBillModel BindItem4Grid(PurchaseBillModel model)
    {
        var _List = new List<PBItemDetail>();

        string purchaseBillJson = HttpContext.Session.GetString("PurchaseBill");
        PurchaseBillModel MainModel = null;
        if (!string.IsNullOrEmpty(purchaseBillJson))
        {
            MainModel = JsonConvert.DeserializeObject<PurchaseBillModel>(purchaseBillJson);
        }
        _List.Add(
            new PBItemDetail
            {
                SeqNo = MainModel.ItemDetailGrid == null ? 1 : MainModel.ItemDetailGrid.Count + 1,
                DocTypeID = model.DocTypeID,
                DocTypeText = model.DocTypeText,
                BillQty = (model.BillQty > 0) ? Convert.ToDecimal(model.BillQty) : 0,

                Amount = model.Amount,
                Description = model.Description,
                DisPer = model.DisPer,
                DisAmt = model.DisAmt,

                HSNNO = model.HSNNO,
                ItemCode = model.ItemCode,
                ItemText = model.ItemText,

                OtherRateCurr = model.OtherRateCurr,
                //PartCode = model.PartCode,
                PartText = model.PartText,

                PBQty = model.PBQty,
                Process = model.Process,
                ProcessName = model.ProcessName,
                CostCenter = model.CostCenter,
                CostCenterName = model.CostCenterName,
                Rate = model.Rate,

                pono = model.pono,
                poyearcode = model.poyearcode,
                PODate = model.PODate,
                schno = model.schno,
                schyearcode = model.schyearcode,
                SchDate = model.SchDate,

                Unit = model.Unit,
            });

        if (MainModel.ItemDetailGrid == null)
            MainModel.ItemDetailGrid = _List;
        else
            MainModel.ItemDetailGrid.AddRange(_List);
        var list = MainModel.ItemDetailGridd;
        MainModel.ItemNetAmount = MainModel.ItemDetailGridd?.Sum(x => Convert.ToDecimal(x.Amount ?? 0)) ?? 0;
        return MainModel;
    }

    public async Task<JsonResult> AltUnitConversion(int ItemCode, decimal AltQty, decimal UnitQty)
    {
        var JSON = await IPurchaseBill.AltUnitConversion(ItemCode, AltQty, UnitQty);
        string JsonString = JsonConvert.SerializeObject(JSON);
        return Json(JsonString);
    }

    [HttpGet]

    public async Task<ActionResult> ViewPOCompleted(string Mode, int ID, int YC, string PONO)
    {
        var model = new PurchaseBillModel();
        MemoryCacheEntryOptions cacheEntryOptions = new()
        {
            AbsoluteExpiration = DateTime.Now.AddMinutes(55),
            SlidingExpiration = TimeSpan.FromMinutes(60),
            Size = 1024,
        };
        if (!string.IsNullOrEmpty(Mode) && ID > 0 && (Mode == "POC"))
        {
            model = await IPurchaseBill.GetViewPOCcompletedByID(ID, YC, PONO, "VIEWPOCOMPLETEDBYID").ConfigureAwait(true);

            model.Mode = Mode;
            model = await BindModels(model);

            model.ID = ID;

            if (model.ItemDetailGrid?.Count != 0 && model.ItemDetailGrid != null)
            {
                string json = JsonConvert.SerializeObject(model.ItemDetailGrid);
                HttpContext.Session.SetString("PurchaseBill", json);
            }

            if (model.TaxDetailGridd != null)
            {
                string json = JsonConvert.SerializeObject(model.TaxDetailGridd);
                HttpContext.Session.SetString("KeyTaxGrid", json);
            }
            if (model.TDSDetailGridd != null)
            {
                string json = JsonConvert.SerializeObject(model.TDSDetailGridd);
                HttpContext.Session.SetString("KeyTDSGrid", json);
            }
        }
        else
        {
            model = await BindModels(null);
            HttpContext.Session.Remove("POTaxGrid");
            HttpContext.Session.Remove("KeyTaxGrid");
            HttpContext.Session.Remove("KeyTDSGrid");
            HttpContext.Session.Remove("PurchaseBill");
        }
        return View("PurchaseBill", model);
    }

    public async Task<JsonResult> GetPurchaseBillItemData(string? Mode, string? flag, string? FlagMRNJWCHALLAN, string? Mrnno, int? mrnyearcode, int? accountcode)
    {
        try
        {
            var model = new PurchaseBillModel();
            MemoryCacheEntryOptions cacheEntryOptions = new()
            {
                AbsoluteExpiration = DateTime.Now.AddMinutes(55),
                SlidingExpiration = TimeSpan.FromMinutes(60),
                Size = 1024,
            };
            var result = string.Empty;
            if (!string.IsNullOrEmpty(Mode))
            {
                model = await IPurchaseBill.GetPurchaseBillItemData(flag, FlagMRNJWCHALLAN, Mrnno, mrnyearcode, accountcode).ConfigureAwait(true);

                model.Mode = Mode;

                if (model != null)
                {
                    string json = JsonConvert.SerializeObject(model);
                    HttpContext.Session.SetString("PurchaseBill", json);
                }
                if (model.TaxDetailGridd != null)
                {
                    string json = JsonConvert.SerializeObject(model.TaxDetailGridd);
                    HttpContext.Session.SetString("KeyTaxGrid", json);
                }
                if (model.TDSDetailGridd != null)
                {
                    string json = JsonConvert.SerializeObject(model.TDSDetailGridd);
                    HttpContext.Session.SetString("KeyTDSGrid", json);
                }
                TempData["PurchaseBillModel"] = model;
                result = "Done";
            }
            else
            {
                model = await BindModels(null);
                HttpContext.Session.Remove("POTaxGrid");
                HttpContext.Session.Remove("KeyTaxGrid");
                HttpContext.Session.Remove("KeyTDSGrid");
                HttpContext.Session.Remove("PurchaseBill");
                TempData.Clear();
                result = "error";
            }
            string JsonString = JsonConvert.SerializeObject(result);
            return Json(JsonString);
        }
        catch (Exception ex)
        {
            // Log detailed error info
            Console.WriteLine($"Error: {ex.Message}");
            Console.WriteLine($"Stack Trace: {ex.StackTrace}");

            // Optionally return the error details (for debugging)
            return Json(new { success = false, message = ex.Message, stackTrace = ex.StackTrace });
        }
    }

    [HttpPost]
    public IActionResult UploadExcel()
    {
        var excelFile = Request.Form.Files[0];
        string pono = Request.Form.Where(x => x.Key == "PoNo").FirstOrDefault().Value;
        int poYearcode = Convert.ToInt32(Request.Form.Where(x => x.Key == "POYearcode").FirstOrDefault().Value);
        int AccountCode = Convert.ToInt32(Request.Form.Where(x => x.Key == "AccountCode").FirstOrDefault().Value);
        string SchNo = Request.Form.Where(x => x.Key == "SchNo").FirstOrDefault().Value;
        var SchYearCode = Convert.ToInt32(Request.Form.Where(x => x.Key == "SchYearCode").FirstOrDefault().Value);
        string Currency = Request.Form.Where(x => x.Key == "Currency").FirstOrDefault().Value;
        string Flag = Request.Form.Where(x => x.Key == "Flag").FirstOrDefault().Value;


        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        List<PBItemDetail> data = new List<PBItemDetail>();

        using (var stream = excelFile.OpenReadStream())
        using (var package = new ExcelPackage(stream))
        {
            var worksheet = package.Workbook.Worksheets[0];
            int cnt = 1;
            for (int row = 2; row <= worksheet.Dimension.Rows; row++)
            {
                //var itemCatCode = IStockAdjust.GetItemCatCode(worksheet.Cells[row, 6].Value.ToString());
                var itemCode = IPurchaseBill.GetItemCode(worksheet.Cells[row, 3].Value.ToString());
                var partcode = 0;
                var itemCodeValue = 0;
                if (itemCode.Result.Result != null)
                {
                    partcode = itemCode.Result.Result.Rows.Count <= 0 ? 0 : (int)itemCode.Result.Result.Rows[0].ItemArray[0];
                    itemCodeValue = itemCode.Result.Result.Rows.Count <= 0 ? 0 : (int)itemCode.Result.Result.Rows[0].ItemArray[0];
                }

                if (partcode == 0)
                {
                    return Json("Partcode not available");
                }
                // for pending qty validation -- still need to change
                var PBQty = Convert.ToDecimal(worksheet.Cells[row, 6].Value.ToString());


                //for altunit conversion
                var altUnitConversion = AltUnitConversion(partcode, 0, Convert.ToDecimal(worksheet.Cells[row, 6].Value.ToString()));
                JObject AltUnitCon = JObject.Parse(altUnitConversion.Result.Value.ToString());
                decimal altUnitValue = (decimal)AltUnitCon["Result"][0]["AltUnitValue"];


                var GetExhange = GetExchangeRate(Currency);

                JObject AltRate = JObject.Parse(GetExhange.Result.Value.ToString());
                decimal AltRateToken = (decimal)AltRate["Result"][0]["Rate"];
                var RateInOther = Convert.ToDecimal(worksheet.Cells[row, 14].Value) * AltRateToken;


                var DisRs = PBQty * Convert.ToDecimal(worksheet.Cells[row, 14].Value) * (Convert.ToDecimal(worksheet.Cells[row, 18].Value.ToString()) / 100);
                var Amount = (PBQty * Convert.ToDecimal(worksheet.Cells[row, 14].Value)) - DisRs;



                data.Add(new PBItemDetail()
                {
                    SeqNo = cnt++,
                    PartText = worksheet.Cells[row, 3].Value.ToString(),
                    ItemText = worksheet.Cells[row, 4].Value.ToString(),
                    ItemCode = itemCodeValue,
                    PartCode = Convert.ToString(partcode),
                    HSNNO = Convert.ToInt32(worksheet.Cells[row, 5].Value.ToString()),
                    PBQty = Convert.ToDecimal(worksheet.Cells[row, 6].Value.ToString()),
                    Unit = worksheet.Cells[row, 7].Value.ToString(),
                    AltQty = altUnitValue,
                    AltUnit = worksheet.Cells[row, 9].Value.ToString(),

                    AltPendQty = Convert.ToDecimal(worksheet.Cells[row, 11].Value.ToString()),
                    Process = Convert.ToInt32(worksheet.Cells[row, 13].Value.ToString()),
                    Rate = Convert.ToDecimal(worksheet.Cells[row, 14].Value.ToString()),
                    OtherRateCurr = RateInOther,
                    UnitRate = worksheet.Cells[row, 17].Value.ToString(),
                    DisPer = Convert.ToDecimal(worksheet.Cells[row, 18].Value.ToString()),
                    DisAmt = Convert.ToDecimal(DisRs.ToString("F2")),
                    Amount = Convert.ToDecimal(Amount.ToString("F2")),
                    TxRemark = worksheet.Cells[row, 24].Value == null ? "" : worksheet.Cells[row, 24].Value.ToString(),
                    Description = worksheet.Cells[row, 25].Value == null ? "" : worksheet.Cells[row, 25].Value.ToString(),
                    AdditionalRate = Convert.ToDecimal(worksheet.Cells[row, 26].Value.ToString()),
                    Color = worksheet.Cells[row, 27].Value == null ? "" : worksheet.Cells[row, 27].Value.ToString(),
                    CostCenter = Convert.ToInt32(worksheet.Cells[row, 28].Value.ToString()),

                });
            }
        }


        var MainModel = new PurchaseBillModel();
        var POItemGrid = new List<PBItemDetail>();
        var POGrid = new List<PBItemDetail>();
        var SSGrid = new List<PBItemDetail>();

        MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
        {
            AbsoluteExpiration = DateTime.Now.AddMinutes(60),
            SlidingExpiration = TimeSpan.FromMinutes(55),
            Size = 1024,
        };
        var seqNo = 0;
        HttpContext.Session.Remove("PurchaseBill");

        foreach (var item in data)
        {
            if (item != null)
            {
                string purchaseBillDataJson = HttpContext.Session.GetString("PurchaseBill");
                PurchaseBillModel Model = null;

                if (!string.IsNullOrEmpty(purchaseBillDataJson))
                {
                    Model = JsonConvert.DeserializeObject<PurchaseBillModel>(purchaseBillDataJson);
                }

                if (Model == null)
                {
                    item.SeqNo += seqNo + 1;
                    POItemGrid.Add(item);
                    seqNo++;
                }
                else
                {
                    if (Model.ItemDetailGrid.Where(x => x.ItemCode == item.ItemCode).Any())
                    {
                        return StatusCode(207, "Duplicate");
                    }
                    else
                    {
                        item.SeqNo = Model.ItemDetailGrid.Count + 1;
                        POItemGrid = Model.ItemDetailGrid.Where(x => x != null).ToList();
                        SSGrid.AddRange(POItemGrid);
                        POItemGrid.Add(item);
                    }
                }
                MainModel.ItemDetailGrid = POItemGrid;
                MainModel.ItemNetAmount = MainModel.ItemDetailGrid.Sum(x => Convert.ToDecimal(x.Amount ?? 0));
                string json = JsonConvert.SerializeObject(MainModel);
                HttpContext.Session.SetString("PurchaseBill", json);
            }
        }
        string purchaseBillJson = HttpContext.Session.GetString("PurchaseBill");
        PurchaseBillModel MainModel1 = null;

        if (!string.IsNullOrEmpty(purchaseBillJson))
        {
            MainModel1 = JsonConvert.DeserializeObject<PurchaseBillModel>(purchaseBillJson);
        }

        return PartialView("_POItemGrid", MainModel);
    }

}