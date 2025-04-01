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
using OfficeOpenXml;
using System.Data;
using Newtonsoft.Json.Linq;
using static eTactWeb.Data.Common.CommonFunc;
using System.Net;
using System.Data;
using System.Globalization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static System.Runtime.InteropServices.JavaScript.JSType;
using FastReport.Export.PdfSimple;

namespace eTactWeb.Controllers;

[Authorize]
public class PurchaseOrderController : Controller
{
    private readonly IMemoryCacheService _iMemoryCacheService;
    private readonly IWebHostEnvironment _IWebHostEnvironment;
    private readonly IConfiguration iconfiguration;
    public WebReport webReport;

    public PurchaseOrderController(IPurchaseOrder iPurchaseOrder, IDataLogic iDataLogic, IMemoryCache iMemoryCache, ILogger<PurchaseOrderModel> logger, EncryptDecrypt encryptDecrypt, IMemoryCacheService iMemoryCacheService, IWebHostEnvironment iWebHostEnvironment, IConfiguration configuration)
    {
        _iMemoryCacheService = iMemoryCacheService;
        IPurchaseOrder = iPurchaseOrder;
        IDataLogic = iDataLogic;
        IMemoryCache = iMemoryCache;
        _Logger = logger;
        EncryptDecrypt = encryptDecrypt;
        CI = new CultureInfo("en-GB");
        _IWebHostEnvironment = iWebHostEnvironment;
        iconfiguration = configuration;
    }

    public ILogger<PurchaseOrderModel> _Logger { get; set; }
    public CultureInfo CI { get; private set; }
    public EncryptDecrypt EncryptDecrypt { get; private set; }
    public IDataLogic IDataLogic { get; private set; }
    public IMemoryCache IMemoryCache { get; private set; }
    public IPurchaseOrder IPurchaseOrder { get; set; }

    public IActionResult PrintReport(int EntryId = 0, int YearCode = 0, string PONO = "")
    {
        
        string my_connection_string;
        string contentRootPath = _IWebHostEnvironment.ContentRootPath;
        string webRootPath = _IWebHostEnvironment.WebRootPath;
        webReport = new WebReport();
        var ReportName = IPurchaseOrder.GetReportName();
        ViewBag.EntryId = EntryId;
        ViewBag.YearCode =YearCode;
        ViewBag.PONO = PONO;
        if (!string.Equals(ReportName.Result.Result.Rows[0].ItemArray[0], System.DBNull.Value))
        {
            webReport.Report.Load(webRootPath + "\\" + ReportName.Result.Result.Rows[0].ItemArray[0] + ".frx"); // from database
        }
        else
        {
            webReport.Report.Load(webRootPath + "\\PO.frx"); // default report

        }
        webReport.Report.SetParameterValue("entryparam", EntryId);
        webReport.Report.SetParameterValue("yearparam", YearCode);
        webReport.Report.SetParameterValue("ponoparam", PONO);
        my_connection_string = iconfiguration.GetConnectionString("eTactDB");
        webReport.Report.SetParameterValue("MyParameter", my_connection_string);
        
        return View(webReport);
    }
  
    public IActionResult Pdf(int EntryId = 0, int YearCode = 0, string PONO = "")
    {
        try
        {
            string webRootPath = _IWebHostEnvironment.WebRootPath;
            var report = new Report(); // Use the Report class instead of WebReport for exports

            // Get the report name (await properly if async)
            var ReportNameResult = IPurchaseOrder.GetReportName().Result.Result;
            string reportFileName = !string.Equals(ReportNameResult.Rows[0].ItemArray[0], DBNull.Value)
                ? ReportNameResult.Rows[0].ItemArray[0].ToString()
                : "PO"; // Default to "PO.frx"

            string reportPath = Path.Combine(webRootPath, $"{reportFileName}.frx");

            // Ensure the report file exists
            if (!System.IO.File.Exists(reportPath))
                throw new FileNotFoundException($"Report file not found: {reportPath}");

            // Load the report
            report.Load(reportPath);

            // Set parameters
            report.SetParameterValue("entryparam", EntryId);
            report.SetParameterValue("yearparam", YearCode);
            report.SetParameterValue("ponoparam", PONO);

            // Set connection string
            string myConnectionString = iconfiguration.GetConnectionString("eTactDB");
            report.SetParameterValue("MyParameter", myConnectionString);

            // Prepare the report (generate data)
            report.Prepare();

            // Export to PDF
            using (MemoryStream ms = new MemoryStream())
            {
                PDFSimpleExport pdfExport = new PDFSimpleExport();
                pdfExport.Export(report, ms);
                ms.Position = 0;

                return File(ms.ToArray(), "application/pdf", $"PurchaseOrder_{EntryId}.pdf");
            }
        }
        catch (Exception ex)
        {
            // Log the error (e.g., using ILogger)
            return StatusCode(500, $"Error generating PDF: {ex.Message}");
        }
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
    [HttpPost]
    public IActionResult AddIndentDetail([FromBody] List<PendingIndentDetailModel> model)
    {
        bool TF = false;
        PurchaseOrderModel mainModel = new();
        foreach (var item in model)
        {
            if (mainModel != null && mainModel.PendingIndentDetailGrid != null)
            {
                TF = mainModel.PendingIndentDetailGrid.Any(x => x.ItemCode == item.ItemCode);
            }
            if (TF)
                break;
        }
        if (TF)
        {
            return Json(208, "Duplicate Item");
        }
        IMemoryCache.Remove("KeyPendingIndentDetail");
        mainModel.PendingIndentDetailGrid = model;
        IMemoryCache.Set("KeyPendingIndentDetail", mainModel, DateTimeOffset.Now.AddMinutes(60));
        return Json("OK");
    }
    public IActionResult AddItem2Grid(PurchaseOrderModel model)
    {
        bool TF = false;

        IMemoryCache.TryGetValue("KeyTaxGrid", out IList<TaxModel> POTaxGrid);
        IMemoryCache.TryGetValue("PurchaseOrder", out PurchaseOrderModel MainModel);

        //if (POTaxGrid != null && POTaxGrid.Count > 0)
        //{
        //    return StatusCode(205, "Reset Tax Detail");
        //}

        if (MainModel != null && MainModel.ItemDetailGrid != null)
        {
            TF = MainModel.ItemDetailGrid.Any(x => x.ItemCode == model.ItemCode);
        }

        if (TF)
        {
            return StatusCode(208, "Duplicate Item");
        }
        else
        {
            model = BindItem4Grid(model);
            IMemoryCache.Remove("PurchaseOrder");
            IMemoryCache.Set("PurchaseOrder", model, DateTimeOffset.Now.AddMinutes(60));
        }

        return PartialView("_POItemGrid", model);
    }
    public async Task<JsonResult> GetFormRights()
    {
        var userID = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
        var JSON = await IPurchaseOrder.GetFormRights(userID);
        string JsonString = JsonConvert.SerializeObject(JSON);
        return Json(JsonString);
    }

    public async Task<JsonResult> GetFormRightsAmm()
    {
        var userID = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
        var JSON = await IPurchaseOrder.GetFormRightsAmm(userID);
        string JsonString = JsonConvert.SerializeObject(JSON);
        return Json(JsonString);
    }
    public async Task<JsonResult> FillItems(string Type, string ShowAllItem)
    {
        var JSON = await IPurchaseOrder.FillItems(Type, ShowAllItem);
        string JsonString = JsonConvert.SerializeObject(JSON);
        return Json(JsonString);
    }
    public async Task<JsonResult> CheckLockYear(int YearCode)
    {
        var JSON = await IPurchaseOrder.CheckLockYear(YearCode);
        string JsonString = JsonConvert.SerializeObject(JSON);
        return Json(JsonString);
    }
    public async Task<JsonResult> FillCurrency(string Ctrl)
    {
        var JSON = await IPurchaseOrder.FillCurrency(Ctrl);
        string JsonString = JsonConvert.SerializeObject(JSON);
        return Json(JsonString);
    }
    public async Task<JsonResult> GetExchangeRate(string Currency)
    {
        var JSON = await IPurchaseOrder.GetExchangeRate(Currency);
        string JsonString = JsonConvert.SerializeObject(JSON);
        return Json(JsonString);
    }

    public async Task<IActionResult> GetItemServiceFORPO(string ItemService)
    {
        var JSONString = await IPurchaseOrder.GetItemServiceFORPO(ItemService);

        //var Dlist = JsonConvert.DeserializeObject<Dictionary<object, object>>(JSONString);
        //JObject json = JObject.Parse(JSONString);
        //object obj = JsonConvert.DeserializeObject(JSONString, typeof(object));
        //JToken jToken = (JToken)json;

        //object value = "";
        //Dlist.TryGetValue(key: "Result", out value);

        return Json(JSONString);
    }

    public PartialViewResult AddSchedule(DeliverySchedule model)
    {
        //var MainModel = new PurchaseOrderModel();
        var ScheduleList = new List<DeliverySchedule>();
        IMemoryCache.TryGetValue("PurchaseOrder", out PurchaseOrderModel MainModel);
        //var ItemDetailList = MainModel.ItemDetailGrid;

        foreach (POItemDetail item in MainModel.ItemDetailGrid)
        {
            if (item.PartCode == model.DPartCode)
            {
                if (item.DeliveryScheduleList == null)
                {
                    ScheduleList.Add(
                        new DeliverySchedule()
                        {
                            SRNo = 1,
                            DPartCode = model.DPartCode,
                            AltQty = model.AltQty,
                            Date = Convert.ToDateTime(model.Date).ToString("dd/MM/yyyy"),
                            Days = model.Days,
                            Qty = model.Qty,
                            TotalQty = model.Qty,
                            Remarks = model.Remarks,
                        });
                    item.DeliveryScheduleList = ScheduleList;
                    MainModel.DPartCode = model.DPartCode;
                    //MainModel.ItemDetailGrid = ItemDetailList;
                    IMemoryCache.Set("PurchaseOrder", MainModel, DateTimeOffset.Now.AddMinutes(60));
                    //HttpContext.Session.SetString("ItemList", JsonConvert.SerializeObject(MainModel.ItemDetailGrid));
                }
                else
                {
                    item.DeliveryScheduleList.Add(
                        new DeliverySchedule()
                        {
                            SRNo = item.DeliveryScheduleList.Count + 1,
                            DPartCode = model.DPartCode,
                            AltQty = model.AltQty,
                            Date = Convert.ToDateTime(model.Date).ToString("dd/MM/yyyy"),
                            Days = model.Days,
                            Qty = model.Qty,
                            TotalQty = item.DeliveryScheduleList.Sum(x => x.Qty) + model.Qty,
                            Remarks = model.Remarks,
                        });
                    MainModel.DPartCode = model.DPartCode;
                    //MainModel.ItemDetailGrid = ItemDetailList;
                    IMemoryCache.Set("PurchaseOrder", MainModel, DateTimeOffset.Now.AddMinutes(60));
                    //HttpContext.Session.SetString("ItemList", JsonConvert.SerializeObject(MainModel.ItemDetailGrid));
                }
            }
        }

        return PartialView("_PODeliveryGrid", MainModel);
    }

    public async Task<PurchaseOrderModel> BindModels(PurchaseOrderModel model)
    {
        CommonFunc.LogException<PurchaseOrderModel>.LogInfo(_Logger, "********** Mohan Kumar *************");

        _Logger.LogInformation("********** Binding Model *************");

        var oDataSet = new DataSet();
        var SqlParams = new List<KeyValuePair<string, string>>();

        SqlParams.Add(new KeyValuePair<string, string>("@Flag", "BINDALLDROPDOWN"));

        oDataSet = await IDataLogic.BindAllDropDown("PurchaseOrder", "SP_PurchaseOrder", SqlParams).ConfigureAwait(false);
        model.POForList = new List<TextValue>();
        //model.AccountList = new List<TextValue>();
        model.BranchList = new List<TextValue>();
        //model.PartCodeList = new List<TextValue>();
        //model.ItemNameList = new List<TextValue>();
        model.DepartmentList = new List<TextValue>();
        model.CostCenterList = new List<TextValue>();
        model.PreparedByList = new List<TextValue>();
        model.ProcessList = new List<TextValue>();
        model.MRPNoList = new List<TextValue>();
        model.QuotList = new List<TextValue>();

        if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
        {
            //model.AccountList = (from DataRow row in oDataSet.Tables[0].Rows select new Common.TextValue { Value = row[Constants.AccountCode].ToString(), Text = row[Constants.AccountName].ToString() }).ToList();

            model.POForList = (from DataRow row in oDataSet.Tables[0].Rows select new Common.TextValue { Value = row["POFor"].ToString(), Text = row["POFor"].ToString() }).ToList();

            //model.CurrencyList = (from DataRow row in oDataSet.Tables[2].Rows select new Common.TextValue { Value = row["Entry_ID"].ToString(), Text = row["Currency"].ToString() }).ToList();

            model.BranchList = (from DataRow row in oDataSet.Tables[1].Rows select new Common.TextValue { Value = row[Constants.EntryID].ToString(), Text = row[Constants.CompanyName].ToString() }).ToList();

            //model.PartCodeList = (from DataRow row in oDataSet.Tables[2].Rows select new Common.TextValue { Value = row[Constants.ItemCode].ToString(), Text = row[Constants.PartCode].ToString() }).ToList();

            //model.ItemNameList = (from DataRow row in oDataSet.Tables[3].Rows select new Common.TextValue { Value = row[Constants.ItemCode].ToString(), Text = row[Constants.ItemName].ToString() }).ToList();

            model.DepartmentList = (from DataRow row in oDataSet.Tables[2].Rows select new Common.TextValue { Value = row[Constants.EntryID].ToString(), Text = row[Constants.DeptName].ToString() }).ToList();

            model.CostCenterList = (from DataRow row in oDataSet.Tables[3].Rows select new Common.TextValue { Value = row[Constants.EntryID].ToString(), Text = row[Constants.CostCenterName].ToString() }).ToList();

            model.PreparedByList = (from DataRow row in oDataSet.Tables[4].Rows select new Common.TextValue { Value = row[Constants.EmpyID].ToString(), Text = row[Constants.EmpyName].ToString() }).ToList();

            model.isPONo = oDataSet.Tables[5].Rows.Count == 0 ? false : oDataSet.Tables[5].Rows[0]["Purchasecodetype"].ToString() != "A";


            model.ProcessList = (from DataRow row in oDataSet.Tables[6].Rows select new Common.TextValue { Value = row[Constants.EntryID].ToString(), Text = row[Constants.StageDesc].ToString() }).ToList();

            model.MRPNoList = (from DataRow row in oDataSet.Tables[7].Rows select new Common.TextValue { Value = row[Constants.EntryID].ToString(), Text = row[Constants.MRPNo].ToString() }).ToList();

            model.QuotList = (from DataRow row in oDataSet.Tables[8].Rows select new Common.TextValue { Value = row[Constants.EntryID].ToString(), Text = row[Constants.QuotNo].ToString() }).ToList();
        }

        //if (model.Mode == null && model.ID == 0)
        //{
        //    model.YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
        //    model.EntryID = IDataLogic.GetEntryID("PurchaseOrderMain", Constants.FinincialYear, "EntryID");
        //    model.OrderNo = model.EntryID;
        //    //model.EntryDate = DateTime.Today.ToString("dd/MM/yyyy", CI);
        //    //model.EntryDate = DateTime.Now;
        //    model.PODate = DateTime.Today.ToString("dd/MM/yyyy", CI);
        //    model.WEF = DateTime.Today.ToString("dd/MM/yyyy", CI);
        //    model.POCloseDate = DateTime.Today.AddYears(1).ToString("dd/MM/yyyy", CI);
        //    model.AmmDate = DateTime.Today.ToString("dd/MM/yyyy", CI);
        //    model.RefDate = DateTime.Today.ToString("dd/MM/yyyy", CI);
        //    model.FOC = "N";
        //    model.GSTIncludedONRate = "N";
        //}
        return model;
    }



    public async Task<IActionResult> DashBoard()
    {
        HttpContext.Session.Remove("PurchaseOrder");
        IMemoryCache.Remove("PurchaseOrder");
        HttpContext.Session.Remove("TaxGrid");
        IMemoryCache.Remove("KeyTaxGrid");

        var _List = new List<TextValue>();

        var MainModel = await IPurchaseOrder.GetDashBoardData();

        //MainModel.PONoList = new List<TextValue>
        //{
        //    new ()
        //    {
        //        Text = MainModel.PODashboard.Select(x => x.PONo).ToString(),
        //        Value = MainModel.PODashboard.Select(x => x.PONo).ToString()
        //    }
        //};

        //var lst = new TextValue();
        //foreach (var item in MainModel.PODashboard)
        //{
        //    lst.Text = MainModel.PODashboard.Select(x => x.PONo).ToString();
        //    lst.Value = MainModel.PODashboard.Select(x => x.PONo).ToString();
        //}
        //MainModel.PONoList.Add(lst);

        //MainModel.PONoList = (
        //    from item in MainModel.PODashboard
        //    select new TextValue
        //    {
        //        Text = MainModel.PODashboard.Select(x => x.PONo).ToString(),
        //        Value = MainModel.PODashboard.Select(x => x.PONo).ToString(),
        //    }).ToList();

        if (MainModel.PODashboard == null || MainModel.PODashboard.Count == 0)
        {
            MainModel.PONoList = _List;
        }
        else
        {
            foreach (var item in MainModel.PODashboard)
            {
                if (!string.IsNullOrEmpty(item.PONo))
                {
                    TextValue _PONo = new()
                    {
                        Text = item.PONo.ToString(),
                        Value = item.PONo.ToString(),
                    };
                    _List.Add(_PONo);
                }
            }
            MainModel.PONoList = _List;
        }

        MainModel.FromDate = new DateTime(DateTime.Today.Year, 4, 1).ToString("dd/MM/yyyy").Replace("-", "/"); // 1st Feb this year
        MainModel.ToDate = new DateTime(DateTime.Today.Year + 1, 3, 31).ToString("dd/MM/yyyy").Replace("-", "/");//.AddDays(-1); // Last day in January next year

        return View(MainModel);
    }

    public async Task<IActionResult> DeleteByID(int ID, int YC, int createdBy, string entryByMachineName)
    {
        var Result = await IPurchaseOrder.DeleteByID(ID, YC, createdBy, entryByMachineName, "DELETEBYID");

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
        else
        {
            ViewBag.isSuccess = false;
            TempData["500"] = "500";
        }

        return RedirectToAction(nameof(DashBoard));
    }

    public PartialViewResult DeleteDeliveryRow(string SRNo, string DPC)
    {
        PurchaseOrderModel MainModel = new();
        IMemoryCache.TryGetValue("PurchaseOrder", out MainModel);

        int Indx = Convert.ToInt32(SRNo) - 1;
        int PCode = Convert.ToInt32(DPC);

        List<POItemDetail> DeliveryList = MainModel.ItemDetailGrid.Where(x => x.PartCode == PCode).ToList();
        List<DeliverySchedule> ScheduleList = DeliveryList
            .SelectMany(x => x.DeliveryScheduleList)
            .ToList();
        ScheduleList.RemoveAt(Indx);

        Indx = 0;
        foreach (DeliverySchedule item in ScheduleList)
        {
            Indx++;
            item.SRNo = Indx;
            item.TotalQty = ScheduleList.Sum(x => x.Qty);
            MainModel.DPartCode = item.DPartCode;
        }

        foreach (POItemDetail ID in MainModel.ItemDetailGrid)
        {
            if (ID.PartCode == PCode && ID.DeliveryScheduleList != null)
            {
                foreach (DeliverySchedule item in ID.DeliveryScheduleList)
                {
                    if (item.DPartCode == PCode)
                    {
                        ID.DeliveryScheduleList = ScheduleList;
                        break;
                    }
                }
            }
        }

        IMemoryCache.Set("PurchaseOrder", MainModel, DateTimeOffset.Now.AddMinutes(60));

        return PartialView("_PODeliveryGrid", MainModel);
    }

    public IActionResult DeleteItemRow(string SeqNo)
    {
        bool exists = false;

        //IMemoryCache.TryGetValue("KeyPOTaxGrid", out List<TaxModel> TaxGrid);
        IMemoryCache.TryGetValue("KeyTaxGrid", out List<TaxModel> TaxGrid);
        IMemoryCache.TryGetValue("PurchaseOrder", out PurchaseOrderModel MainModel);

        int Indx = Convert.ToInt32(SeqNo) - 1;

        if (MainModel.ItemDetailGrid.Count != 0)
        {
            var itemfound = MainModel.ItemDetailGrid.FirstOrDefault(item => item.SeqNo == Convert.ToInt32(SeqNo)).PartCode;

            var ItmPartCode = (MainModel.ItemDetailGrid.Where(item => item.SeqNo == Convert.ToInt32(SeqNo)).Select(item => item.PartCode)).FirstOrDefault();

            if (TaxGrid != null)
            {
                exists = TaxGrid.Any(x => x.TxPartCode == ItmPartCode);
            }

            if (exists)
            {
                return StatusCode(207, "Duplicate");
            }

            MainModel.ItemDetailGrid.RemoveAt(Convert.ToInt32(Indx));

            Indx = 0;

            foreach (POItemDetail item in MainModel.ItemDetailGrid)
            {
                Indx++;
                item.SeqNo = Indx;
            }
            MainModel.ItemNetAmount = MainModel.ItemDetailGrid.Sum(x => x.Amount);

            //if (MainModel.ItemDetailGrid.Count <= 0)
            //{
            //    IMemoryCache.Remove("PurchaseOrder");
            //}
            //else
            //{
            //    IMemoryCache.Set("PurchaseOrder", MainModel, DateTimeOffset.Now.AddMinutes(60));
            //}

            IMemoryCache.Set("PurchaseOrder", MainModel, DateTimeOffset.Now.AddMinutes(60));
        }
        return PartialView("_POItemGrid", MainModel);
    }

    public IActionResult EditItemRow(PurchaseOrderModel model)
    {
        IMemoryCache.TryGetValue("PurchaseOrder", out PurchaseOrderModel MainModel);
        var SSGrid = MainModel.ItemDetailGrid.Where(x => x.SeqNo == model.SeqNo);
        string JsonString = JsonConvert.SerializeObject(SSGrid);
        return Json(JsonString);
    }

    public async Task<IActionResult> GetMrpYear(string MRPNo)
    {
        var MrpYear = await IPurchaseOrder.GetMrpYear("MRPYEAR", MRPNo);
        _Logger.LogError(JsonConvert.SerializeObject(MrpYear));
        return Json(MrpYear);
    }

    public async Task<JsonResult> GetPartyList(string Check)
    {
        var JSON = await IPurchaseOrder.GetAllPartyName(Check);
        _Logger.LogError(JsonConvert.SerializeObject(JSON));
        string JSONString = JsonConvert.SerializeObject(JSON);
        return Json(JSONString);
    }
    public async Task<JsonResult> GetGstRegister(int Code)
    {
        var JSON = await IPurchaseOrder.GetGstRegister("GSTRegistered", Code);
        string JSONString = JsonConvert.SerializeObject(JSON);
        _Logger.LogError(JsonConvert.SerializeObject(JSON));
        return Json(JSONString);
    }
    public async Task<JsonResult> FillEntryandPONumber(int YearCode)
    {
        var JSON = await IPurchaseOrder.FillEntryandPONumber(YearCode);
        string JsonString = JsonConvert.SerializeObject(JSON);
        return Json(JsonString);
    }

    public async Task<JsonResult> PoallowtoprintWithoutApproval()
    {
        var JSON = await IPurchaseOrder.PoallowtoprintWithoutApproval();
        string JsonString = JsonConvert.SerializeObject(JSON);
        return Json(JsonString);
    }
    public async Task<JsonResult> ClearTaxGrid(int YearCode)
    {
        IMemoryCache.Remove("KeyTaxGrid");
        var JSON = await IPurchaseOrder.FillEntryandPONumber(YearCode);
        string JsonString = JsonConvert.SerializeObject(JSON);
        return Json(JsonString);
    }
    public async Task<JsonResult> FillPONumber(int YearCode, string OrderType, string PODate)
    {
        var JSON = await IPurchaseOrder.FillPONumber(YearCode, OrderType, PODate);
        string JsonString = JsonConvert.SerializeObject(JSON);
        return Json(JsonString);
    }

    public async Task<IActionResult> GetQuotData(string QuotNo)
    {
        var QuotData = await IPurchaseOrder.GetQuotData("QUOTDATA", QuotNo);
        _Logger.LogError(JsonConvert.SerializeObject(QuotData));
        return Json(JsonConvert.SerializeObject(QuotData));
    }

    public async Task<IActionResult> GetSearchData(PODashBoard model)
    {
        model.Mode = "SEARCH";
        model = await IPurchaseOrder.GetSearchData(model);
        model.DashboardType = "Summary";
        return PartialView("_DashBoardGrid", model);
    }
    public async Task<IActionResult> GetDetailData(PODashBoard model)
    {
        model.Mode = "SEARCH";
        model = await IPurchaseOrder.GetDetailData(model);
        model.DashboardType = "Detail";
        return PartialView("_DashBoardGrid", model);
    }
    public async Task<IActionResult> GetAmmSearchData(PODashBoard model)
    {
        model.Mode = "SEARCH";
        model = await IPurchaseOrder.GetSearchData(model);
        model.Mode = "Pending";
        return PartialView("_AmmListGrid", model);
    }
    public async Task<IActionResult> GetAmmCompSearchData(PODashBoard model)
    {
        model = await IPurchaseOrder.GetSearchCompData(model);
        model.Mode = "Completed";
        return PartialView("_AmmListGrid", model);
    }

    // GET: PurchaseOrderController
    [HttpGet]
    [Route("{controller}/Index")]
    public async Task<IActionResult> PODetail(int ID, int YC, string Mode)
    {
        IMemoryCache.Remove("POTaxGrid");
        IMemoryCache.Remove("KeyTaxGrid");
        IMemoryCache.Remove("PurchaseOrder");
        IMemoryCache.Remove("KeyPendingIndentDetail");

        var MainModel = new PurchaseOrderModel();

        if (string.IsNullOrEmpty(Mode) && ID == 0)
        {
            MainModel = await BindModels(MainModel).ConfigureAwait(false);
            MainModel.EID = EncryptDecrypt.Encrypt(ID.ToString(CI));
            MainModel.Mode = "INSERT";
        }
        else if (!string.IsNullOrEmpty(Mode) && ID > 0 && (Mode == "V" || Mode == "U" || Mode == "POA" || Mode == "C" || Mode == "PAU"))
        {

            MainModel = await IPurchaseOrder.GetViewByID(ID, YC, Mode == "POA" ? "POA" : "VIEWBYID").ConfigureAwait(true);
            MainModel = await BindModels(MainModel);
            MainModel.ID = ID;
            MainModel.Mode = Mode;
            MainModel.EntryID = ID;
            MainModel.EID = EncryptDecrypt.Encrypt(ID.ToString(CI));
            IMemoryCache.Set("KeyPendingIndentDetail", MainModel.PendingIndentDetailGrid, DateTimeOffset.Now.AddMinutes(60));
        }
        else
        {
            //do nthing
        }
        if (Mode != "POA" && Mode != "PAU" && Mode != "POC")
            MainModel.YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
        else
        {
            //MainModel.OldRate = MainModel.Rate;
            MainModel.YearCode = YC;
            MainModel.AmmYearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
        }
        //MainModel.YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
        MainModel.YearCode = Constants.FinincialYear;
        MainModel.FinFromDate = HttpContext.Session.GetString("FromDate");
        MainModel.FinToDate = HttpContext.Session.GetString("ToDate");
        MainModel.PreparedBy = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
        MainModel.PreparedByName = HttpContext.Session.GetString("EmpName");
        MainModel.Branch = HttpContext.Session.GetString("Branch");
        //var txGrid = MainModel.TaxDetailGridd == null ? new TaxModel() : MainModel.TaxDetailGridd
        IMemoryCache.Set("PurchaseOrder", MainModel, DateTimeOffset.Now.AddMinutes(60));
        IMemoryCache.Set("KeyTaxGrid", MainModel.TaxDetailGridd == null ? new List<TaxModel>() : MainModel.TaxDetailGridd, DateTimeOffset.Now.AddMinutes(60));
        IMemoryCache.Set("KeyPendingIndentDetail", MainModel.PendingIndentDetailGrid);

        HttpContext.Session.SetString("PurchaseOrder", JsonConvert.SerializeObject(MainModel));

        if (Mode != "U")
        {
            MainModel.CreatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
            MainModel.CretaedByName = HttpContext.Session.GetString("EmpName");
            MainModel.CreatedOn = DateTime.Now;
        }
        else
        {
            MainModel.UpdatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
            MainModel.UpdatedByName = HttpContext.Session.GetString("EmpName");
            MainModel.UpdatedOn = DateTime.Now;
        }

        return View(MainModel);
    }

    public IActionResult RefreshSchedule(int PCode, string Typ)
    {
        var MainModel = new PurchaseOrderModel();
        Dictionary<string, string> SchVal = new();
        IMemoryCache.TryGetValue("PurchaseOrder", out MainModel);

        if (MainModel.ItemDetailGrid != null && MainModel.ItemDetailGrid.Count > 0)
        {
            var DeliveryList = MainModel.ItemDetailGrid.FirstOrDefault(x => x.PartCode == PCode);

            if (DeliveryList != null)
            {
                SchVal.Add("Qty", DeliveryList.POQty.ToString());
                SchVal.Add("AltQty", DeliveryList.AltPOQty.ToString());
                SchVal.Add("Remark", DeliveryList.PIRemark == null ? DeliveryList.PIRemark : DeliveryList.PIRemark.ToString());
            }

            if (DeliveryList != null && DeliveryList.DeliveryScheduleList != null)
            {
                MainModel.DPartCode = PCode;
            }
            else
            {
                MainModel.DPartCode = PCode;
            }
        }

        if (Typ == "SchVal")
        {
            return Json(new { SchVal });
        }
        return PartialView("_PODeliveryGrid", MainModel);
    }

    public JsonResult ResetGridItems()
    {
        HttpContext.Session.Remove("POItemList");
        IMemoryCache.Remove("PurchaseOrder");
        IMemoryCache.Remove("KeyTaxGrid");

        var MainModel = new PurchaseOrderModel();
        List<TaxModel> taxList = new List<TaxModel>();

        MainModel.YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
        MainModel.FinFromDate = HttpContext.Session.GetString("FromDate");
        MainModel.FinToDate = HttpContext.Session.GetString("ToDate");
        MainModel.PreparedBy = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
        MainModel.PreparedByName = HttpContext.Session.GetString("EmpName");
        MainModel.Branch = HttpContext.Session.GetString("Branch");

        IMemoryCache.Set("PurchaseOrder", MainModel, DateTimeOffset.Now.AddMinutes(60));
        IMemoryCache.Set("KeyTaxGrid", taxList, DateTimeOffset.Now.AddMinutes(60));
        HttpContext.Session.SetString("PurchaseOrder", JsonConvert.SerializeObject(MainModel));
        IMemoryCache.TryGetValue("PurchaseOrder", out MainModel);
        IMemoryCache.TryGetValue("KeyTaxGrid", out List<TaxModel> TaxGrid);

        return new(StatusCodes.Status200OK);
    }

    // POST: PurchaseOrderController
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("{controller}/Index")]
    public async Task<IActionResult> SavePODetail(PurchaseOrderModel model)
    {
        try
        {
            bool isError = true;
            DataSet DS = new();
            DataTable ItemDetailDT = null;
            DataTable IndentDetailDT = null;
            DataTable TaxDetailDT = null;
            ResponseResult Result = new();
            DataTable DelieveryScheduleDT = null;
            Dictionary<string, string> ErrList = new();
            string modePOA = "data";
            var stat = new MemoryCacheStatistics();

            IMemoryCache.TryGetValue("PurchaseOrder", out PurchaseOrderModel MainModel);
            IMemoryCache.TryGetValue("KeyPendingIndentDetail", out PurchaseOrderModel PendingModel);

            IMemoryCache.TryGetValue("KeyTaxGrid", out List<TaxModel> TaxGrid);

            var cc = stat.CurrentEntryCount;
            var pp = stat.CurrentEstimatedSize;

            ModelState.Clear();

            if (MainModel.ItemDetailGrid != null && MainModel.ItemDetailGrid.Count > 0)
            {
                DS = GetItemDetailTable(MainModel.ItemDetailGrid);
                ItemDetailDT = DS.Tables[0];
                DelieveryScheduleDT = DS.Tables[1];
                model.ItemDetailGrid = MainModel.ItemDetailGrid;

                isError = false;
            }
            else
            {
                ErrList.Add("ItemDetailGrid", "Item Details Cannot Be Blank..!");
            }

            if (PendingModel != null)
            {
                DS = GetPOIndentDetailTable(PendingModel.PendingIndentDetailGrid);
                IndentDetailDT = DS.Tables[0];
                model.PendingIndentDetailGrid = PendingModel.PendingIndentDetailGrid;

                isError = false;
            }

            if (TaxGrid != null && TaxGrid.Count > 0)
            {
                TaxDetailDT = GetTaxDetailTable(TaxGrid);
            }

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
                        string ImagePath = "Uploads/PurchaseOrder/";

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
                        string ImagePath = "Uploads/PurchaseOrder/";

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
                        string ImagePath = "Uploads/PurchaseOrder/";

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

                    //model.FinFromDate = HttpContext.Session.GetString("FromDate");
                    //model.FinToDate = HttpContext.Session.GetString("ToDate");
                    //model.YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
                    //model.CC = HttpContext.Session.GetString("Branch");
                    //model.PreparedByEmp = HttpContext.Session.GetString("EmpName");
                    //model.ActualEnteredByName = HttpContext.Session.GetString("EmpName");
                    model.CreatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                    Result = await IPurchaseOrder.SavePurchaseOrder(ItemDetailDT, DelieveryScheduleDT, TaxDetailDT, IndentDetailDT, model);
                }

                if (Result != null)
                {
                    MainModel.Mode = "INSERT";
                    model.Mode = "INSERT";
                    var stringResponse = JsonConvert.SerializeObject(Result);
                    if (stringResponse.Contains("Constraint") || stringResponse.Contains("error"))
                    {
                        ViewBag.isSuccess = true;
                        TempData["500"] = "500";
                    }
                    else if (model.TypeOfSave != "PS")
                    {
                        var MainModel1 = new PurchaseOrderModel();

                        MainModel1 = await BindModels(MainModel1).ConfigureAwait(false);
                        //MainModel1.EID = EncryptDecrypt.Encrypt(ID.ToString(CI));
                        MainModel1.Mode = "INSERT";
                        MainModel1.YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
                        MainModel1.FinFromDate = HttpContext.Session.GetString("FromDate");
                        MainModel1.FinToDate = HttpContext.Session.GetString("ToDate");
                        MainModel1.PreparedBy = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
                        MainModel1.PreparedByName = HttpContext.Session.GetString("EmpName");
                        MainModel1.Branch = HttpContext.Session.GetString("Branch");

                        IMemoryCache.Set("PurchaseOrder", MainModel1, DateTimeOffset.Now.AddMinutes(60));
                        IMemoryCache.Set("KeyTaxGrid", MainModel1.TaxDetailGridd, DateTimeOffset.Now.AddMinutes(60));
                        HttpContext.Session.SetString("PurchaseOrder", JsonConvert.SerializeObject(MainModel1));
                        return View("PODetail", MainModel1);
                    }
                    else if (modePOA == "POA")
                    {
                        dynamic jsonObj = JsonConvert.DeserializeObject(stringResponse);
                        int resultValue = jsonObj.Result[0].Result;
                        int ycValue = jsonObj.Result[0].YC;
                        return RedirectToAction("PODetail", new { ID = resultValue, YC = ycValue, Mode = "POA" });
                    }
                    else
                    {
                        dynamic jsonObj = JsonConvert.DeserializeObject(stringResponse);
                        int resultValue = jsonObj.Result[0].Result;
                        int ycValue = jsonObj.Result[0].YC;
                        return RedirectToAction("PODetail", new { ID = resultValue, YC = ycValue, Mode = "U" });
                    }



                }
                else
                {
                    model = await BindModels(model);

                    foreach (KeyValuePair<string, string> Err in ErrList)
                    {
                        ModelState.AddModelError(Err.Key, Err.Value);
                    }
                }
            }
            else
            {
                model = await BindModels(model);

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

            return View("Error", ResponseResult);
        }
        model = await BindModels(model);
        model.Mode = "INSERT";
        return View("PODetail", model);
    }
    //---    Item.BOMRevDate == null ? string.Empty : ParseFormattedDate(Item.BOMRevDate.Split(" ")[0]),
    private static DataTable GetDeliveryTable(POItemDetail itemDetail, ref DataTable TblSch)
    {
        foreach (var Item in itemDetail.DeliveryScheduleList)
        {
            TblSch.Rows.Add(
            new object[]
            {
                itemDetail.ItemCode,
                Item.Qty,
                Item.AltQty,
                Item.Days,
                
                 Item.Date == null ? string.Empty : ParseFormattedDate(Item.Date.Split(" ")[0]),
                Item.Remarks,
            });
        }

        return TblSch;
    }
    ////Item.Date == null ? string.Empty : ParseDate(Item.Date),
    private static DataSet GetItemDetailTable(IList<POItemDetail> itemDetailList)
    {
        DataSet DS = new();
        DataTable Table = new();

        Table.Columns.Add("SeqNo", typeof(int));
        Table.Columns.Add("ItemCode", typeof(int));
        Table.Columns.Add("HSNNo", typeof(int));
        Table.Columns.Add("POQty", typeof(decimal));
        Table.Columns.Add("Unit", typeof(string));
        Table.Columns.Add("AltPOQty", typeof(decimal));
        Table.Columns.Add("AltUnit", typeof(string));
        Table.Columns.Add("TolLimitPercent", typeof(decimal));
        Table.Columns.Add("TolLimitQty", typeof(decimal));
        Table.Columns.Add("UnitRate", typeof(string));
        Table.Columns.Add("Rate", typeof(decimal));
        Table.Columns.Add("OtherRateCurr", typeof(decimal));
        Table.Columns.Add("DiscPer", typeof(decimal));
        Table.Columns.Add("DiscRs", typeof(decimal));
        Table.Columns.Add("Amount", typeof(decimal));
        Table.Columns.Add("AdditionalRate", typeof(decimal));
        Table.Columns.Add("OldRate", typeof(decimal));
        Table.Columns.Add("PIRemark", typeof(string));
        Table.Columns.Add("Description", typeof(string));
        Table.Columns.Add("PendQty", typeof(decimal));
        Table.Columns.Add("AltPendQty", typeof(decimal));
        Table.Columns.Add("Process", typeof(int));
        Table.Columns.Add("PkgStd", typeof(decimal));
        Table.Columns.Add("AmendmentNo", typeof(int));
        Table.Columns.Add("AmendmentDate", typeof(string));
        Table.Columns.Add("AmendmentReason", typeof(string));
        Table.Columns.Add("FirstMonthTentQty", typeof(decimal));
        Table.Columns.Add("SecMonthTentQty", typeof(decimal));
        Table.Columns.Add("SizeDetail", typeof(string));
        Table.Columns.Add("Color", typeof(string));
        Table.Columns.Add("CostCenter", typeof(int));

        DataTable TblSch = new();

        TblSch.Columns.Add("ItemCode", typeof(int));
        TblSch.Columns.Add("Qty", typeof(float));
        TblSch.Columns.Add("AltQty", typeof(float));
        TblSch.Columns.Add("Days", typeof(int));
        TblSch.Columns.Add("Date", typeof(string));
        TblSch.Columns.Add("Remarks", typeof(string));

        foreach (POItemDetail Item in itemDetailList)
        {
            Table.Rows.Add(
                new object[]
                {
                    Item.SeqNo,
                    Item.ItemCode,
                    Item.HSNNo,
                    Item.POQty,
                    Item.Unit,
                    Item.AltPOQty,
                    Item.AltUnit,
                    Item.TolLimitPercent,
                    Item.TolLimitQty,
                    Item.UnitRate,
                    Item.Rate,
                    Item.OtherRateCurr,
                    Item.DiscPer,
                    Item.DiscRs,
                    Item.Amount,
                    Item.AdditionalRate,
                    Item.OldRate,
                    Item.PIRemark,
                    Item.Description,
                    Item.PendQty,
                    Item.AltPendQty,
                    Item.Process,
                    Item.PkgStd,
                    Item.AmendmentNo,
                    Item.AmendmentDate == null ? string.Empty : ParseFormattedDate(Item.AmendmentDate.Split(" ")[0]),
                    Item.AmendmentReason,
                    Item.FirstMonthTentQty,
                    Item.SecMonthTentQty,
                    Item.SizeDetail,
                    Item.Color,
                    Item.CostCenter
                });

            if (Item.DeliveryScheduleList != null && Item.DeliveryScheduleList.Count > 0)
            {
                GetDeliveryTable(Item, ref TblSch);
            }
        }

        DS.Tables.Add(Table);
        DS.Tables.Add(TblSch);
        return DS;
    }
    // Item.AmendmentDate == null ? string.Empty : ParseDate(Item.AmendmentDate),
    private static DataSet GetPOIndentDetailTable(IList<PendingIndentDetailModel> itemDetailList)
    {
        DataSet DS = new();
        DataTable Table = new();

        Table.Columns.Add("IndentNo", typeof(string));
        Table.Columns.Add("IndentEntryId", typeof(int));
        Table.Columns.Add("IndentYearCode", typeof(int));
        Table.Columns.Add("PONo", typeof(string));
        Table.Columns.Add("POEntryID", typeof(int));
        Table.Columns.Add("POYearCode", typeof(int));
        Table.Columns.Add("POAccountCode", typeof(int));
        Table.Columns.Add("ItemCode", typeof(int));
        Table.Columns.Add("Qty", typeof(float));
        Table.Columns.Add("Unit", typeof(string));
        Table.Columns.Add("AltQty", typeof(float));
        Table.Columns.Add("AltUnit", typeof(string));
        Table.Columns.Add("IndentDate", typeof(string));
        Table.Columns.Add("RequiredDate", typeof(string));
        Table.Columns.Add("PODate", typeof(string));

        foreach (var Item in itemDetailList)
        {
            Table.Rows.Add(
                new object[]
                {
                    Item.IndentNo,
                    Item.IndentEntryId  == "" ? 0 : Convert.ToInt32(Item.IndentEntryId),
                     2024,//Item.IndentYearCode,
                    Item.PendQtyForPO, // POno
                    Item.POEntryId   == "" ? 0 : Convert.ToInt32(Item.POEntryId), // POEntryId
                    Item.POYearCode   == "" ? 0 : Convert.ToInt32(Item.POYearCode),//POYC
                    Item.POAccountCode   == "" ? 0 : Convert.ToInt32(Item.POAccountCode),//POAC
                   Item.ItemCode   == "" ? 0 : Convert.ToInt32(Item.ItemCode),
                    Item.Qty   == "" ? 0 : Convert.ToSingle(Item.Qty),
                    Item.Unit,
                    Item.AltQty   == "" ? 0 : Convert.ToSingle(Item.AltQty), // AltQty
                    Item.AltUnit ,// AltUnit
                    Item.IndentDate == "" ? string.Empty : ParseFormattedDate((Item.IndentDate).Split(" ")[0]) ,
                    Item.ReqDate  == "" ? string.Empty : ParseFormattedDate((Item.ReqDate).Split(" ")[0]),
                    Item.PODate  == "" ? string.Empty : ParseFormattedDate((Item.PODate).Split(" ")[0])
                });
        }
        DS.Tables.Add(Table);
        return DS;
    }

    //public static string ParseDate(string dateString)
    //{
    //    if (string.IsNullOrEmpty(dateString))
    //    {
    //        return string.Empty;
    //    }

    //    if (DateTime.TryParseExact(dateString, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate))
    //    {
    //        return parsedDate.ToString("yyyy/MM/dd");
    //    }
    //    else
    //    {
    //        if (DateTime.TryParse(dateString, out DateTime nonFormattedDate))
    //        {
    //            return nonFormattedDate.ToString("yyyy/MM/dd");
    //        }

    //        return string.Empty;
    //    }
    //}


    private static DataTable GetTaxDetailTable(List<TaxModel> TaxDetailList)
    {
        DataTable Table = new();
        Table.Columns.Add("TxSeqNo", typeof(int));
        Table.Columns.Add("TxType", typeof(string));
        Table.Columns.Add("TxItemCode", typeof(int));
        Table.Columns.Add("TxTaxType", typeof(int));
        Table.Columns.Add("TxAccountCode", typeof(int));
        Table.Columns.Add("TxPercentg", typeof(float));
        Table.Columns.Add("TxAdInTxable", typeof(string));
        Table.Columns.Add("TxRoundOff", typeof(string));
        Table.Columns.Add("TxAmount", typeof(float));
        Table.Columns.Add("TxRefundable", typeof(string));
        Table.Columns.Add("TxOnExp", typeof(float));
        Table.Columns.Add("TxRemark", typeof(string));

        foreach (TaxModel Item in TaxDetailList)
        {
            Table.Rows.Add(
                new object[]
                {
                    Item.TxSeqNo,
                    Item.TxType,
                    Item.TxItemCode,
                    Item.TxTaxType,
                    Item.TxAccountCode,
                    Item.TxPercentg,
                    Item.TxAdInTxable,
                    Item.TxRoundOff,
                    Item.TxAmount,
                    Item.TxRefundable,
                    Item.TxOnExp,
                    Item.TxRemark,
                });
        }

        return Table;
    }

    private PurchaseOrderModel BindItem4Grid(PurchaseOrderModel model)
    {
        var _List = new List<POItemDetail>();

        IMemoryCache.TryGetValue("PurchaseOrder", out PurchaseOrderModel MainModel);
        //var SeqNo = 0;
        //if(MainModel == null)
        //{
        //    SeqNo++;
        //}
        _List.Add(
            new POItemDetail
            {
                SeqNo = MainModel.ItemDetailGrid == null ? 1 : MainModel.ItemDetailGrid.Count + 1,
                ItemCode = model.ItemCode,
                AdditionalRate = model.AdditionalRate,
                AltPendQty = model.AltPendQty,
                AltPOQty = model.AltPOQty,
                AltUnit = model.AltUnit,
                AmendmentDate = model.AmendmentDate,
                AmendmentNo = model.AmendmentNo,
                AmendmentReason = model.AmendmentReason,
                Amount = model.Amount,
                Color = model.Color,
                CostCenter = model.CostCenter,
                Description = model.Description,
                DiscPer = model.DiscPer,
                DiscRs = model.DiscRs,
                FirstMonthTentQty = model.FirstMonthTentQty,
                HSNNo = model.HSNNo,
                ItemNetAmount = model.ItemNetAmount,
                ItemText = model.ItemText,
                OldRate = model.OldRate,
                OtherRateCurr = model.OtherRateCurr,
                PartCode = model.PartCode,
                PartText = model.PartText,
                PendQty = model.PendQty,
                PIRemark = model.PIRemark,
                PkgStd = model.PkgStd,
                POQty = model.POQty,
                Process = model.Process,
                Rate = model.Rate,
                SecMonthTentQty = model.SecMonthTentQty,
                SizeDetail = model.SizeDetail,
                TolLimitPercent = model.TolLimitPercent,
                TolLimitQty = model.TolLimitQty,
                Unit = model.Unit,
                UnitRate = model.UnitRate,
            });

        if (MainModel.ItemDetailGrid == null)
            MainModel.ItemDetailGrid = _List;
        else
            MainModel.ItemDetailGrid.AddRange(_List);

        MainModel.ItemNetAmount = decimal.Parse(MainModel.ItemDetailGrid.Sum(x => x.Amount).ToString("#.#0"));

        return MainModel;
    }

    public async Task<JsonResult> AltUnitConversion(int ItemCode, decimal AltQty, decimal UnitQty)
    {
        var JSON = await IPurchaseOrder.AltUnitConversion(ItemCode, AltQty, UnitQty);
        string JsonString = JsonConvert.SerializeObject(JSON);
        return Json(JsonString);
    }

    [HttpGet]
    public async Task<IActionResult> POAmendmentList()
    {
        IMemoryCache.Remove("POTaxGrid");
        IMemoryCache.Remove("KeyTaxGrid");
        IMemoryCache.Remove("PurchaseOrder");
        var _List = new List<TextValue>();

        var model = await IPurchaseOrder.GetAmmDashboardData().ConfigureAwait(true);

        //foreach (var item in model.SODashboard)
        //{
        //    item.EID = _EncryptDecrypt.Encrypt(item.EntryID.ToString(new CultureInfo("en-GB")));
        //    TextValue _SONo = new()
        //    {
        //        Text = item.SONo.ToString(new CultureInfo("en-IN")),
        //        Value = item.SONo.ToString(new CultureInfo("en-IN")),
        //    };
        //    _List.Add(_SONo);
        //}
        //model.SONoList = _List;
        model.Mode = "Pending";
        model.CC = HttpContext.Session.GetString("Branch");

        return View(model);
    }
    [HttpGet]
    public async Task<IActionResult> POAmmCompleted()
    {
        IMemoryCache.Remove("POTaxGrid");
        IMemoryCache.Remove("KeyTaxGrid");
        IMemoryCache.Remove("PurchaseOrder");
        var _List = new List<TextValue>();
        var summaryDetail = "Summary";
        var model = await IPurchaseOrder.GetAmmCompletedData(summaryDetail).ConfigureAwait(true);
        model.Mode = "Completed";
        model.CC = HttpContext.Session.GetString("Branch");
        model.SummaryDetail = "Summary";
        return View(model);
    }
    public async Task<ActionResult> ViewPOCompleted(string Mode, int ID, int YC, string PONO)
    {
        var model = new PurchaseOrderModel();
        MemoryCacheEntryOptions cacheEntryOptions = new()
        {
            AbsoluteExpiration = DateTime.Now.AddMinutes(55),
            SlidingExpiration = TimeSpan.FromMinutes(60),
            Size = 1024,
        };
        if (!string.IsNullOrEmpty(Mode) && ID > 0 && (Mode == "POC"))
        {
            model = await IPurchaseOrder.GetViewPOCcompletedByID(ID, YC, PONO, "VIEWPOCOMPLETEDBYID").ConfigureAwait(true);

            model.Mode = Mode;
            model = await BindModels(model);

            model.ID = ID;

            if (model.ItemDetailGrid?.Count != 0 && model.ItemDetailGrid != null)
            {
                IMemoryCache.Set("PurchaseOrder", model.ItemDetailGrid, cacheEntryOptions);

            }

            if (model.TaxDetailGridd != null)
            {


                IMemoryCache.Set("KeyTaxGrid", model.TaxDetailGridd, cacheEntryOptions);
            }
        }
        else
        {
            model = await BindModels(null);
            IMemoryCache.Remove("POTaxGrid");
            IMemoryCache.Remove("KeyTaxGrid");
            IMemoryCache.Remove("PurchaseOrder");
        }


        return View("PODetail", model);
    }
    public async Task<JsonResult> FillVendors()
    {
        var JSON = await IPurchaseOrder.FillVendors();
        string JsonString = JsonConvert.SerializeObject(JSON);
        return Json(JsonString);
    }
    public async Task<JsonResult> NewAmmEntryId(int PoAmendYearCode)
    {
        var JSON = await IPurchaseOrder.NewAmmEntryId(PoAmendYearCode);
        string JsonString = JsonConvert.SerializeObject(JSON);
        return Json(JsonString);
    }
    public async Task<IActionResult> GetUpdAmmData(PODashBoard model)
    {
        model = await IPurchaseOrder.GetUpdAmmData(model);
        model.Mode = "U";
        return PartialView("_AmmListGrid", model);
    }

    public async Task<JsonResult> GetPendQty(string PONo, int POYearCode, int ItemCode, int AccountCode, string SchNo, int SchYearCode, string Flag)
    {
        var JSON = await IPurchaseOrder.GetPendQty(PONo, POYearCode, ItemCode, AccountCode, SchNo, SchYearCode, Flag);
        string JsonString = JsonConvert.SerializeObject(JSON);
        return Json(JsonString);
    }
    public async Task<JsonResult> FillIndentDetail(string itemName, string partCode, int itemCode)
    {
        var JSON = await IPurchaseOrder.FillIndentDetail(itemName, partCode, itemCode);
        string JsonString = JsonConvert.SerializeObject(JSON);
        return Json(JsonString);
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
        List<POItemDetail> data = new List<POItemDetail>();
        var errors = new List<string>();

        using (var stream = excelFile.OpenReadStream())
        using (var package = new ExcelPackage(stream))
        {
            var worksheet = package.Workbook.Worksheets[0];
            int cnt = 1;
            for (int row = 2; row <= worksheet.Dimension.Rows; row++)
            {
                //var itemCatCode = IStockAdjust.GetItemCatCode(worksheet.Cells[row, 6].Value.ToString());
                var itemCode = IPurchaseOrder.GetItemCode(worksheet.Cells[row, 1].Value.ToString());
                var partcode = 0;
                var itemCodeValue = 0;
                var itemname = "";
                if (itemCode.Result.Result != null && itemCode.Result.Result.Rows.Count > 0)
                {
                    partcode = Convert.ToInt32(itemCode.Result.Result.Rows[0].ItemArray[0]);
                    itemCodeValue = Convert.ToInt32(itemCode.Result.Result.Rows[0].ItemArray[0]);
                    itemname = itemCode.Result.Result.Rows[0].ItemArray[1]?.ToString() ?? "Unknown Item"; // Ensure string conversion
                }
                else
                {
                    partcode = 0;
                    itemCodeValue = 0;
                    itemname = "Unknown Item"; // Set default name if not found
                }

                if (partcode == 0)
                {
                    errors.Add($"Invalid PartCode at row {row}");
                    continue;
                }
                // for pending qty validation -- still need to change
                var POQty = Convert.ToDecimal(worksheet.Cells[row, 4].Value.ToString());



                string poType = Request.Form["POType"];
                bool isPOTypeClose = poType.Equals("Close", StringComparison.OrdinalIgnoreCase);

                // **Quantity and Rate Validation**
                decimal qty = isPOTypeClose
                    ? decimal.TryParse(worksheet.Cells[row, 4].Value?.ToString(), out decimal tempQty) ? tempQty : 0
                    : 0;
                var DisRs = Convert.ToDecimal(worksheet.Cells[row, 7].Value);

                if (isPOTypeClose && qty <= 0)
                {
                    errors.Add($"Qty is less then 0 at row: {row}");
                    continue;
                }
                else if(!isPOTypeClose)
                {
                    DisRs = 0;
                }


                //for altunit conversion
                //var altUnitConversion = AltUnitConversion(partcode, 0, Convert.ToDecimal(worksheet.Cells[row, 4].Value.ToString()));
                //JObject AltUnitCon = JObject.Parse(altUnitConversion.Result.Value.ToString());
                //decimal altUnitValue = (decimal)AltUnitCon["Result"][0]["AltUnitValue"];

                var pendQty = GetPendQty(pono, poYearcode, itemCodeValue, AccountCode, SchNo, SchYearCode, Flag);

                //JObject AltPendQTy = JObject.Parse(pendQty.Result.Value.ToString());
                //JToken recqtyToken = AltPendQTy["Result"]["Table"][0]["RECQTY"];
                //JToken poRateToken = AltPendQTy["Result"]["Table"][0]["PORATE"];

                var GetExhange = GetExchangeRate(Currency);

                //JObject AltRate = JObject.Parse(GetExhange.Result.Value.ToString());
                //decimal AltRateToken = (decimal)AltRate["Result"][0]["Rate"];
                //var RateInOther = Convert.ToDecimal(worksheet.Cells[row, 14].Value) * AltRateToken;

                //decimal recqtyValue = recqtyToken.Value<decimal>();
                //decimal poRateValue = poRateToken.Value<decimal>();
                //decimal AltPendQTyValue = Convert.ToDecimal(worksheet.Cells[row, 6].Value.ToString()) - recqtyValue;

               
                var Amount = (qty * Convert.ToDecimal(worksheet.Cells[row, 3].Value)) - DisRs;

                //if (AltPendQTyValue < 0)
                //{
                //    return Json("Not Done");
                //}

                data.Add(new POItemDetail()
                {
                    SeqNo = cnt++,
                    PartText = worksheet.Cells[row, 1].Value?.ToString() ?? string.Empty,
                    ItemText = itemname,
                    ItemCode = itemCodeValue,
                    PartCode = partcode,
                    HSNNo = Convert.ToInt32(worksheet.Cells[row, 2].Value?.ToString() ?? "0"),
                    POQty = qty,
                    Unit = worksheet.Cells[row, 5].Value?.ToString() ?? string.Empty,
                    AltPOQty = qty,
                    AltUnit = worksheet.Cells[row, 11].Value?.ToString() ?? string.Empty,
                    PendQty = Convert.ToDecimal(worksheet.Cells[row, 11].Value?.ToString() ?? "0"),
                    AltPendQty = Convert.ToDecimal(worksheet.Cells[row, 11].Value?.ToString() ?? "0"),
                    PkgStd = Convert.ToDecimal(worksheet.Cells[row, 12].Value?.ToString() ?? "0"),
                    Process = Convert.ToInt32(worksheet.Cells[row, 13].Value?.ToString() ?? "0"),
                    Rate = Convert.ToDecimal(worksheet.Cells[row, 3].Value?.ToString() ?? "0"),
                    OldRate = Convert.ToDecimal(worksheet.Cells[row, 3].Value?.ToString() ?? "0"),
                    OtherRateCurr = Convert.ToDecimal(worksheet.Cells[row, 3].Value?.ToString() ?? "0"),
                    UnitRate = worksheet.Cells[row, 17].Value?.ToString() ?? string.Empty,
                    DiscPer = Convert.ToDecimal(worksheet.Cells[row, 6].Value?.ToString() ?? "0"),
                    DiscRs = Convert.ToDecimal(worksheet.Cells[row, 7].Value?.ToString() ?? "0"),
                    Amount = Amount,
                    TolLimitQty = Convert.ToDecimal(worksheet.Cells[row, 21].Value?.ToString() ?? "0"),
                    TolLimitPercent = Convert.ToDecimal(worksheet.Cells[row, 22].Value?.ToString() ?? "0"),
                    SizeDetail = worksheet.Cells[row, 23].Value?.ToString() ?? string.Empty,
                    TxRemark = worksheet.Cells[row, 24].Value?.ToString() ?? string.Empty,
                    Description = worksheet.Cells[row, 25].Value?.ToString() ?? string.Empty,
                    AdditionalRate = Convert.ToDecimal(worksheet.Cells[row, 26].Value?.ToString() ?? "0"),
                    Color = worksheet.Cells[row, 27].Value?.ToString() ?? string.Empty,
                    CostCenter = Convert.ToInt32(worksheet.Cells[row, 28].Value?.ToString() ?? "0"),
                    FirstMonthTentQty = Convert.ToDecimal(worksheet.Cells[row, 29].Value?.ToString() ?? "0"),
                    SecMonthTentQty = Convert.ToDecimal(worksheet.Cells[row, 30].Value?.ToString() ?? "0"),
                    AmendmentNo = Convert.ToInt32(worksheet.Cells[row, 31].Value?.ToString() ?? "0"),
                    AmendmentDate = worksheet.Cells[row, 32].Value?.ToString() ?? string.Empty,
                    AmendmentReason = worksheet.Cells[row, 33].Value?.ToString() ?? string.Empty,
                });
            }

            if (errors.Count > 0)
            {
                return BadRequest(string.Join("\n", errors));
            }
        }


        var MainModel = new PurchaseOrderModel();
        var POItemGrid = new List<POItemDetail>();
        var POGrid = new List<POItemDetail>();
        var SSGrid = new List<POItemDetail>();

        MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
        {
            AbsoluteExpiration = DateTime.Now.AddMinutes(60),
            SlidingExpiration = TimeSpan.FromMinutes(55),
            Size = 1024,
        };
        var seqNo = 0;
        IMemoryCache.Remove("PurchaseOrder");

        foreach (var item in data)
        {
            if (item != null)
            {
                IMemoryCache.TryGetValue("PurchaseOrder", out PurchaseOrderModel Model);

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

                IMemoryCache.Set("PurchaseOrder", MainModel, cacheEntryOptions);

            }
        }
        IMemoryCache.TryGetValue("PurchaseOrder", out PurchaseOrderModel MainModel1);

        return PartialView("_POItemGrid", MainModel);
    }

}