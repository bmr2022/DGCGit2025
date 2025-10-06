using eTactWeb.Data.Common;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using static eTactWeb.Data.Common.CommonFunc;
using static eTactWeb.DOM.Models.Common;
using FastReport.Export.Html;
using FastReport.Export.Image;
using FastReport;
using FastReport.Web;
using Microsoft.AspNetCore.JsonPatch.Internal;
using eTactWeb.Services.Interface;
using static eTactWeb.DOM.Models.JobWorkReceiveModel;
using eTactWeb.DOM.Models;
using System.Drawing.Printing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Globalization;
using System.Net;
using static System.Runtime.InteropServices.JavaScript.JSType;
using ClosedXML.Excel;

namespace eTactWeb.Controllers
{

    [Authorize]
    public class ProductionEntryController : Controller
    {
        private readonly IDataLogic _IDataLogic;
        public IProductionEntry _IProductionEntry { get; }
        private readonly ILogger<ProductionEntryController> _logger;
        private readonly IConfiguration iconfiguration;
        private readonly IMemoryCache _MemoryCache;
        private readonly ConnectionStringService _connectionStringService;
        public IWebHostEnvironment _IWebHostEnvironment { get; }
        public ProductionEntryController(ILogger<ProductionEntryController> logger, IDataLogic iDataLogic, IProductionEntry iProductionEntry, EncryptDecrypt encryptDecrypt, IWebHostEnvironment iWebHostEnvironment, IConfiguration iconfiguration, IMemoryCache iMemoryCache, ConnectionStringService connectionStringService)
        {
            _logger = logger;
            _IDataLogic = iDataLogic;
            _IProductionEntry = iProductionEntry;
            _IWebHostEnvironment = iWebHostEnvironment;
            this.iconfiguration = iconfiguration;
            _MemoryCache = iMemoryCache;
            _connectionStringService = connectionStringService;
        }
        public IActionResult PrintReport(int EntryId = 0, int YearCode = 0)
        {
            string my_connection_string;
            string contentRootPath = _IWebHostEnvironment.ContentRootPath;
            string webRootPath = _IWebHostEnvironment.WebRootPath;
            //string frx = Path.Combine(_env.ContentRootPath, "reports", value.file);
            var webReport = new WebReport();

            webReport.Report.Load(webRootPath + "\\ProductionSlipEntry.frx"); // default report
                                                                              // webReport.Report.Load(webRootPath + "\\ProductionEntryPrint.frx"); // default report


            //webReport.Report.SetParameterValue("flagparam", "PURCHASEORDERPRINT");
            webReport.Report.SetParameterValue("entryidparam", EntryId);
            webReport.Report.SetParameterValue("yearcodeparam", YearCode);


            //my_connection_string = iconfiguration.GetConnectionString("eTactDB");
            my_connection_string = _connectionStringService.GetConnectionString();
            //my_connection_string = "Data Source=192.168.1.224\\sqlexpress;Initial  Catalog = etactweb; Integrated Security = False; Persist Security Info = False; User
            //         ID = web; Password = bmr2401";
            webReport.Report.SetParameterValue("MyParameter", my_connection_string);


            // webReport.Report.SetParameterValue("accountparam", 1731);


            // webReport.Report.Dictionary.Connections[0].ConnectionString = @"Data Source=103.10.234.95;AttachDbFilename=;Initial Catalog=eTactWeb;Integrated Security=False;Persist Security Info=True;User ID=web;Password=bmr2401";
            //ViewBag.WebReport = webReport;
            return View(webReport);
        }
        public ActionResult HtmlSave(int EntryId = 0, int YearCode = 0)
        {
            using (Report report = new Report())
            {
                string webRootPath = _IWebHostEnvironment.WebRootPath;
                var webReport = new WebReport();


                webReport.Report.Load(webRootPath + "\\ProductionEntryPrint.frx");
                //webReport.Report.SetParameterValue("flagparam", "PURCHASEORDERPRINT");
                webReport.Report.SetParameterValue("entryparam", EntryId);
                webReport.Report.SetParameterValue("yearparam", YearCode);
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
        [Route("{controller}/PendingProductionEntry")]
        [HttpGet]
        public async Task<ActionResult> PendingProductionEntry()
        {
            var MainModel = new PendingProductionEntryModel();
            MainModel.PendingProductionEntryGrid = new List<PendingProductionEntryModel>();
            return View(MainModel);
        }
        public async Task<IActionResult> GetPendingProductionEntry(int Yearcode)
        {
            var model = new PendingProductionEntryModel();
            model = await _IProductionEntry.GetPendingProductionEntry(Yearcode);
            return PartialView("_PendingProductionEntry", model);
        }
        public async Task<IActionResult> GetDataForProductionEntry(PendingProductionEntryModel ItemData)
        {
            HttpContext.Session.Remove("KeyProductionEntryDataGrid");
            string serializedGrid = JsonConvert.SerializeObject(ItemData);
            HttpContext.Session.SetString("KeyProductionEntryDataGrid", serializedGrid);
            return PartialView("_PendingProductionEntry", ItemData);
        }
        public IActionResult GetImage(int EntryId = 0, int YearCode = 0)
        {
            // Creatint the Report object
            using (Report report = new Report())
            {
                string webRootPath = _IWebHostEnvironment.WebRootPath;
                var webReport = new WebReport();


                webReport.Report.Load(webRootPath + "\\ProductionEntryPrint.frx");
                //webReport.Report.SetParameterValue("flagparam", "PURCHASEORDERPRINT");
                webReport.Report.SetParameterValue("entryparam", EntryId);
                webReport.Report.SetParameterValue("yearparam", YearCode);
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
        public IActionResult Index()
        {
            return View();
        }
        [Route("{controller}/Index")]
        public async Task<IActionResult> ProductionEntry()
        {
            ViewData["Title"] = "Production Entry Details";
            TempData.Clear();
            HttpContext.Session.Remove("KeyProductionEntryGrid");
            HttpContext.Session.Remove("KeyProductionEntryOperatordetail");
            HttpContext.Session.Remove("KeyProductionEntryBreakdowndetail");
            HttpContext.Session.Remove("KeyProductionEntryScrapdetail");

            var model = await BindModels(null);
            model.FinFromDate = HttpContext.Session.GetString("FromDate");
            model.FinToDate = HttpContext.Session.GetString("ToDate");
            model.YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
            model.CC = HttpContext.Session.GetString("Branch");
            model.PreparedByEmp = HttpContext.Session.GetString("EmpName");
            model.ActualEnteredByName = HttpContext.Session.GetString("EmpName");
            model.CreatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));

            string serializedGrid = JsonConvert.SerializeObject(model);
            HttpContext.Session.SetString("KeyProductionEntryGrid", serializedGrid);
            return View(model);
        }
        [Route("{controller}/Index")]
        [HttpGet]
        public async Task<ActionResult> ProductionEntry(int ID, string Mode, int YC, string FromDate = "", string ToDate = "", string SlipNo = "", string ItemName = "", string PartCode = "", string ProdPlanNo = "", string ProdSchNo = "", string ReqNo = "", string Searchbox = "", string DashboardType = "")
        {
            _logger.LogInformation("\n \n ********** Page Gate Inward ********** \n \n " + _IWebHostEnvironment.EnvironmentName.ToString() + "\n \n");
            TempData.Clear();
            var MainModel = new ProductionEntryModel();
            var model = new PendingProductionEntryModel();
            MainModel.FinFromDate = HttpContext.Session.GetString("FromDate");
            MainModel.FinToDate = HttpContext.Session.GetString("ToDate");
            MainModel.YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
            MainModel.CC = HttpContext.Session.GetString("Branch");
            MainModel.PreparedByEmp = HttpContext.Session.GetString("EmpName");
            MainModel.CreatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
            MainModel.ActualEnteredByName = HttpContext.Session.GetString("EmpName");
            MainModel.Uid = Convert.ToInt32(HttpContext.Session.GetString("UID"));
            HttpContext.Session.Remove("KeyProductionEntryGrid");
            HttpContext.Session.Remove("KeyProductionEntryBreakdowndetail");
            HttpContext.Session.Remove("KeyProductionEntryOperatordetail");
            HttpContext.Session.Remove("KeyProductionEntryScrapdetail");
            HttpContext.Session.Remove("KeyProductionEntryProductdetail");

            if (!string.IsNullOrEmpty(Mode) && ID > 0 && (Mode == "V" || Mode == "U"))
            {
                MainModel = await _IProductionEntry.GetViewByID(ID, YC).ConfigureAwait(false);
                MainModel.Mode = Mode;
                MainModel.ID = ID;
                MainModel.YearCode = YC;
                MainModel = await BindModels(MainModel).ConfigureAwait(false);
                MainModel.FinFromDate = HttpContext.Session.GetString("FromDate");
                MainModel.FinToDate = HttpContext.Session.GetString("ToDate");
                string serializedProductionGrid = JsonConvert.SerializeObject(MainModel.ItemDetailGrid);
                HttpContext.Session.SetString("KeyProductionEntryGrid", serializedProductionGrid);
                string serializedBreakdownGrid = JsonConvert.SerializeObject(MainModel.BreakdownDetailGrid);
                HttpContext.Session.SetString("KeyProductionEntryBreakdowndetail", serializedBreakdownGrid);
                string serializedOperatorGrid = JsonConvert.SerializeObject(MainModel.OperatorDetailGrid);
                HttpContext.Session.SetString("KeyProductionEntryOperatordetail", serializedOperatorGrid);
                string serializedScrapGrid = JsonConvert.SerializeObject(MainModel.ScrapDetailGrid);
                HttpContext.Session.SetString("KeyProductionEntryScrapdetail", serializedScrapGrid); 
                string serializedProductGrid = JsonConvert.SerializeObject(MainModel.ProductDetailGrid);
                HttpContext.Session.SetString("KeyProductionEntryProductdetail", serializedProductGrid);
            }
            else
            {
                string modelJson = HttpContext.Session.GetString("KeyProductionEntryDataGrid");
                PendingProductionEntryModel PendingProductionEntryModel = new();
                if (!string.IsNullOrEmpty(modelJson))
                {
                    PendingProductionEntryModel = JsonConvert.DeserializeObject<PendingProductionEntryModel>(modelJson);
                }
                MainModel = await BindModels(MainModel);
                if (PendingProductionEntryModel != null)
                {
                    MainModel.WorkCenter = PendingProductionEntryModel.WorkCenter;
                    MainModel.ProdInWCID = PendingProductionEntryModel.WcId;
                    MainModel.PartCode = PendingProductionEntryModel.PartCode;
                    MainModel.ItemName = PendingProductionEntryModel.Item_Name;
                    MainModel.ItemCode = PendingProductionEntryModel.Item_Code;
                    MainModel.ProdSchNo = PendingProductionEntryModel.ProdSchNo;
                    MainModel.ProdSchYear = PendingProductionEntryModel.ProdSchYearCode;
                    MainModel.SchDate = PendingProductionEntryModel.ProdSchdate;
                    MainModel.ProdPlan = PendingProductionEntryModel.PlanNo;
                    MainModel.ProdPlanYear = PendingProductionEntryModel.PlanNoYearCode;
                    MainModel.ProdPlanDate = PendingProductionEntryModel.PlanNoDate;
                    MainModel.BOMNo = PendingProductionEntryModel.BomNo;
                }
            }
            if (Mode != "U")
            {
                MainModel.ActualEnteredBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                MainModel.ActualEnteredByName = HttpContext.Session.GetString("EmpName");
                MainModel.ActualEntryDate = HttpContext.Session.GetString("ActualEntryDate");
            }
            else
            {
                MainModel.ActualEnteredByName = HttpContext.Session.GetString("EmpName");
                MainModel.UpdatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                MainModel.UpdatedByName = HttpContext.Session.GetString("EmpName");
                MainModel.LastUpdatedDate = HttpContext.Session.GetString("LastUpdatedDate");
            }
            MainModel.FromDateBack = FromDate;
            MainModel.ToDateBack = ToDate;
            MainModel.SlipNoBack = SlipNo;
            MainModel.PartCodeBack = PartCode;
            MainModel.ItemNameBack = ItemName;
            MainModel.ProdPlanNoBack = ProdPlanNo;
            MainModel.ProdSchNoBack = ProdSchNo;
            MainModel.ReqNoBack = ReqNo;
            MainModel.GlobalSearchBack = Searchbox;
            MainModel.DashboardTypeBack = DashboardType;
            return View(MainModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("{controller}/Index")]
        public async Task<IActionResult> ProductionEntry(ProductionEntryModel model)
        {
            try
            {
                var GIGrid = new DataTable();
                var BreakDownGrid = new DataTable();
                var OperatorGrid = new DataTable();
                var ScrapGrid = new DataTable();
                var ProductGrid = new DataTable();
                string serializedProductionGrid = HttpContext.Session.GetString("KeyProductionEntryGrid");
                List<ProductionEntryItemDetail> ProductionEntryItemDetail = new();
                if (!string.IsNullOrEmpty(serializedProductionGrid))
                {
                    ProductionEntryItemDetail = JsonConvert.DeserializeObject<List<ProductionEntryItemDetail>>(serializedProductionGrid);
                }
                string serializedBreakdownGrid = HttpContext.Session.GetString("KeyProductionEntryBreakdowndetail");
                List<ProductionEntryItemDetail> ProductionEntryBreakdownDetail = new();
                if (!string.IsNullOrEmpty(serializedBreakdownGrid))
                {
                    ProductionEntryBreakdownDetail = JsonConvert.DeserializeObject<List<ProductionEntryItemDetail>>(serializedBreakdownGrid);
                }
                string serializedOperatorGrid = HttpContext.Session.GetString("KeyProductionEntryOperatordetail");
                List<ProductionEntryItemDetail> ProductionEntryOperatorDetail = new();
                if (!string.IsNullOrEmpty(serializedOperatorGrid))
                {
                    ProductionEntryOperatorDetail = JsonConvert.DeserializeObject<List<ProductionEntryItemDetail>>(serializedOperatorGrid);
                }
                string serializedScrapGrid = HttpContext.Session.GetString("KeyProductionEntryScrapdetail");
                List<ProductionEntryItemDetail> ProductionEntryScrapDetail = new();
                if (!string.IsNullOrEmpty(serializedScrapGrid))
                {
                    ProductionEntryScrapDetail = JsonConvert.DeserializeObject<List<ProductionEntryItemDetail>>(serializedScrapGrid);
                }
                string serializedProductGrid = HttpContext.Session.GetString("KeyProductionEntryProductdetail");
                List<ProductionEntryItemDetail> ProductionEntryProductDetail = new();
                if (!string.IsNullOrEmpty(serializedProductGrid))
                {
                    ProductionEntryProductDetail = JsonConvert.DeserializeObject<List<ProductionEntryItemDetail>>(serializedProductGrid);
                }
                if (ProductionEntryItemDetail == null && ProductionEntryOperatorDetail == null && ProductionEntryBreakdownDetail == null && ProductionEntryScrapDetail == null)
                {
                    ModelState.Clear();
                    ModelState.TryAddModelError("ProductionEntryItemDetail", "Production Entry Grid Should Have Atleast 1 Item...!");
                    model = await BindModels(model);
                    return View("ProductionEntry", model);
                }

                else
                {
                    model.CC = HttpContext.Session.GetString("Branch");
                    model.PreparedByEmpId = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
                    //model.ActualEnteredBy   = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
                    model.CreatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                    if (model.Mode == "U")
                    {
                        model.LastUpdatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                        model.UpdatedByName = HttpContext.Session.GetString("EmpName");
                        GIGrid = GetDetailTable(ProductionEntryItemDetail);
                        BreakDownGrid = GetBreakdownDetailTable(ProductionEntryBreakdownDetail);
                        OperatorGrid = GetOperatorDetailTable(ProductionEntryOperatorDetail);
                        ScrapGrid = GetScrapDetailTable(ProductionEntryScrapDetail);
                        ProductGrid = GetProductDetailTable(ProductionEntryProductDetail);
                    }
                    else
                    {
                        GIGrid = GetDetailTable(ProductionEntryItemDetail);
                        BreakDownGrid = GetBreakdownDetailTable(ProductionEntryBreakdownDetail);
                        OperatorGrid = GetOperatorDetailTable(ProductionEntryOperatorDetail);
                        ScrapGrid = GetScrapDetailTable(ProductionEntryScrapDetail);
                        ProductGrid = GetProductDetailTable(ProductionEntryProductDetail);
                    }

                    var Result = await _IProductionEntry.SaveProductionEntry(model, GIGrid, BreakDownGrid, OperatorGrid, ScrapGrid, ProductGrid);

                    if (Result != null)
                    {
                        if (Result.StatusText == "Success" && Result.StatusCode == HttpStatusCode.OK)
                        {
                            ViewBag.isSuccess = true;
                            TempData["200"] = "200";
                            HttpContext.Session.Remove("KeyProductionEntryGrid");
                            HttpContext.Session.Remove("KeyProductionEntryBreakdowndetail");
                            HttpContext.Session.Remove("KeyProductionEntryOperatordetail");
                            HttpContext.Session.Remove("KeyProductionEntryScrapdetail");
                            HttpContext.Session.Remove("KeyProductionEntryProductdetail");
                        }
                        if (Result.StatusText == "Success" && Result.StatusCode == HttpStatusCode.Accepted)
                        {
                            ViewBag.isSuccess = true;
                            TempData["202"] = "202";
                        }
                        if (Result.StatusText == "Error" && Result.StatusCode == HttpStatusCode.InternalServerError)
                        {
                            ViewBag.isSuccess = false;
                            TempData["500"] = "500";
                            _logger.LogError("\n \n ********** LogError ********** \n " + JsonConvert.SerializeObject(Result) + "\n \n");
                            return View("Error", Result);
                        }
                    }
                    return RedirectToAction(nameof(ProductionEntry));
                }
            }
            catch (Exception ex)
            {
                LogException<ProductionEntryController>.WriteException(_logger, ex);


                var ResponseResult = new ResponseResult()
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    StatusText = "Error",
                    Result = ex
                };

                return View("Error", ResponseResult);
            }
        }

        public async Task<JsonResult> ChkWIPStockBeforeSaving(int WcId, string TransferMatEntryDate, int TransferMatYearCode, int TransferMatEntryId,string Mode)
        {
            var TransferGrid = new DataTable();
            string serializedGrid = HttpContext.Session.GetString("KeyProductionEntryGrid");
            List<ProductionEntryItemDetail> ProductionEntryItemDetail = new();
            if (!string.IsNullOrEmpty(serializedGrid))
            {
                ProductionEntryItemDetail = JsonConvert.DeserializeObject<List<ProductionEntryItemDetail>>(serializedGrid);
            }
            //_MemoryCache.TryGetValue("KeyTransferFromWorkCenterGrid", out List<TransferFromWorkCenterDetail> TransferFromWorkCenterDetail);
            TransferGrid = GetDetailTable(ProductionEntryItemDetail);
            var ChechedData = await _IProductionEntry.ChkWIPStockBeforeSaving(WcId, TransferMatEntryDate, TransferMatYearCode, TransferMatEntryId, TransferGrid,Mode);
            if (ChechedData.StatusCode == HttpStatusCode.OK && ChechedData.StatusText == "Success")
            {
                DataTable dt = ChechedData.Result;

                List<string> errorMessages = new List<string>();

                foreach (DataRow row in dt.Rows)
                {
                    string itemName = row["ItemName"].ToString();
                    string batchNo = row["BatchNo"].ToString();
                    decimal availableQty = Convert.ToDecimal(row["CalWIPStock"]);

                    string error = $"{itemName} + {batchNo} has only {availableQty} quantity available in stock.";
                    errorMessages.Add(error);
                }

                return Json(new
                {
                    success = false,
                    errors = errorMessages
                });
            }
            return Json(new
            {
                success = true,
                message = "No errors found."
            });
        }

        public async Task<JsonResult> GetFormRights()
        {
            var userID = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
            var JSON = await _IProductionEntry.GetFormRights(userID);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> ClearGridAjax()
        {
            HttpContext.Session.Remove("KeyProductionEntryGrid");
            HttpContext.Session.Remove("KeyProductionEntryBreakdowndetail");
            HttpContext.Session.Remove("KeyProductionEntryOperatordetail");
            HttpContext.Session.Remove("KeyProductionEntryScrapdetail");
            return Json("done");
        }
        public async Task<IActionResult> GetSearchData(string FromDate, string ToDate, string SlipNo, string ItemName, string PartCode, string ProdPlanNo, string ProdSchNo, string ReqNo, string DashboardType, int pageNumber = 1, int pageSize = 50, string SearchBox = "")
        {
            //model.Mode = "Search";
            var model = new ProductionEntryDashboard();
            model = await _IProductionEntry.GetDashboardData(FromDate, ToDate, SlipNo, ItemName, PartCode, ProdPlanNo, ProdSchNo, ReqNo, DashboardType);
            model.DashboardType = "Summary";
            var modelList = model?.ProductionDashboard ?? new List<ProductionEntryDashboard>();


            if (string.IsNullOrWhiteSpace(SearchBox))
            {
                model.TotalRecords = modelList.Count();
                model.PageNumber = pageNumber;
                model.PageSize = pageSize;
                model.ProductionDashboard = modelList
                .Skip((pageNumber - 1) * pageSize)
                   .Take(pageSize)
                   .ToList();
            }
            else
            {
                List<ProductionEntryDashboard> filteredResults;
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
                model.ProductionDashboard = filteredResults.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
                model.PageNumber = pageNumber;
                model.PageSize = pageSize;
            }
            MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                SlidingExpiration = TimeSpan.FromMinutes(55),
                Size = 1024,
            };

            _MemoryCache.Set("KeyProdList_Summary", modelList, cacheEntryOptions);
            return PartialView("_ProductionEntryDashboardGrid", model);
        }
        public async Task<IActionResult> GetDetailData(string FromDate, string ToDate, string SlipNo, string ItemName, string PartCode, string ProdPlanNo, string ProdSchNo, string ReqNo, string DashboardType, int pageNumber = 1, int pageSize = 50, string SearchBox = "")
        {
            var model = new ProductionEntryDashboard();
            model = await _IProductionEntry.GetDashboardDetailData(FromDate, ToDate, SlipNo, ItemName, PartCode, ProdPlanNo, ProdSchNo, ReqNo, DashboardType);
            model.DashboardType = "Detail";
            var modelList = model?.ProductionDashboard ?? new List<ProductionEntryDashboard>();


            if (string.IsNullOrWhiteSpace(SearchBox))
            {
                model.TotalRecords = modelList.Count();
                model.PageNumber = pageNumber;
                model.PageSize = pageSize;
                model.ProductionDashboard = modelList
                .Skip((pageNumber - 1) * pageSize)
                   .Take(pageSize)
                   .ToList();
            }
            else
            {
                List<ProductionEntryDashboard> filteredResults;
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
                model.ProductionDashboard = filteredResults.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
                model.PageNumber = pageNumber;
                model.PageSize = pageSize;
            }
            MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                SlidingExpiration = TimeSpan.FromMinutes(55),
                Size = 1024,
            };

            _MemoryCache.Set("KeyProdList_Detail", modelList, cacheEntryOptions);
            return PartialView("_ProductionEntryDashboardGrid", model);
        }
        public async Task<IActionResult> GetBatchwiseDetail(string FromDate, string ToDate, string SlipNo, string ItemName, string PartCode, string ProdPlanNo, string ProdSchNo, string ReqNo, string DashboardType, int pageNumber = 1, int pageSize = 50, string SearchBox = "")
        {
            //model.Mode = "Search";
            var model = new ProductionEntryDashboard();
            model = await _IProductionEntry.GetBatchwiseDetail(FromDate, ToDate, SlipNo, ItemName, PartCode, ProdPlanNo, ProdSchNo, ReqNo, DashboardType);
            model.DashboardType = "DetailWithBatchwise";
            var modelList = model?.ProductionDashboard ?? new List<ProductionEntryDashboard>();


            if (string.IsNullOrWhiteSpace(SearchBox))
            {
                model.TotalRecords = modelList.Count();
                model.PageNumber = pageNumber;
                model.PageSize = pageSize;
                model.ProductionDashboard = modelList
                .Skip((pageNumber - 1) * pageSize)
                   .Take(pageSize)
                   .ToList();
            }
            else
            {
                List<ProductionEntryDashboard> filteredResults;
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
                model.ProductionDashboard = filteredResults.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
                model.PageNumber = pageNumber;
                model.PageSize = pageSize;
            }
            MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                SlidingExpiration = TimeSpan.FromMinutes(55),
                Size = 1024,
            };

            _MemoryCache.Set("KeyProdList_DetailWithBatchwise", modelList, cacheEntryOptions);
            return PartialView("_ProductionEntryDashboardGrid", model);
        }
        public async Task<IActionResult> GetBreakdownData(string FromDate, string ToDate, int pageNumber = 1, int pageSize = 50, string SearchBox = "")
        {
            //model.Mode = "Search";
            var model = new ProductionEntryDashboard();
            model = await _IProductionEntry.GetBreakdownData(FromDate, ToDate);
            model.DashboardType = "Breakdown";
            var modelList = model?.ProductionDashboard ?? new List<ProductionEntryDashboard>();


            if (string.IsNullOrWhiteSpace(SearchBox))
            {
                model.TotalRecords = modelList.Count();
                model.PageNumber = pageNumber;
                model.PageSize = pageSize;
                model.ProductionDashboard = modelList
                .Skip((pageNumber - 1) * pageSize)
                   .Take(pageSize)
                   .ToList();
            }
            else
            {
                List<ProductionEntryDashboard> filteredResults;
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
                model.ProductionDashboard = filteredResults.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
                model.PageNumber = pageNumber;
                model.PageSize = pageSize;
            }
            MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                SlidingExpiration = TimeSpan.FromMinutes(55),
                Size = 1024,
            };

            _MemoryCache.Set("KeyProdList_Breakdown", modelList, cacheEntryOptions);
            return PartialView("_ProductionEntryDashboardGrid", model);
        }
        public async Task<IActionResult> GetOperationData(string FromDate, string ToDate, int pageNumber = 1, int pageSize = 50, string SearchBox = "")
        {
            //model.Mode = "Search";
            var model = new ProductionEntryDashboard();
            model = await _IProductionEntry.GetOperationData(FromDate, ToDate);
            model.DashboardType = "Operator";
            var modelList = model?.ProductionDashboard ?? new List<ProductionEntryDashboard>();


            if (string.IsNullOrWhiteSpace(SearchBox))
            {
                model.TotalRecords = modelList.Count();
                model.PageNumber = pageNumber;
                model.PageSize = pageSize;
                model.ProductionDashboard = modelList
                .Skip((pageNumber - 1) * pageSize)
                   .Take(pageSize)
                   .ToList();
            }
            else
            {
                List<ProductionEntryDashboard> filteredResults;
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
                model.ProductionDashboard = filteredResults.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
                model.PageNumber = pageNumber;
                model.PageSize = pageSize;
            }
            MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                SlidingExpiration = TimeSpan.FromMinutes(55),
                Size = 1024,
            };

            _MemoryCache.Set("KeyProdList_Operator", modelList, cacheEntryOptions);
            return PartialView("_ProductionEntryDashboardGrid", model);
        }
        public async Task<IActionResult> GetScrapData(string FromDate, string ToDate, int pageNumber = 1, int pageSize = 50, string SearchBox = "")
        {
            //model.Mode = "Search";
            var model = new ProductionEntryDashboard();
            model = await _IProductionEntry.GetScrapData(FromDate, ToDate);
            model.DashboardType = "Scrap";
            var modelList = model?.ProductionDashboard ?? new List<ProductionEntryDashboard>();


            if (string.IsNullOrWhiteSpace(SearchBox))
            {
                model.TotalRecords = modelList.Count();
                model.PageNumber = pageNumber;
                model.PageSize = pageSize;
                model.ProductionDashboard = modelList
                .Skip((pageNumber - 1) * pageSize)
                   .Take(pageSize)
                   .ToList();
            }
            else
            {
                List<ProductionEntryDashboard> filteredResults;
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
                model.ProductionDashboard = filteredResults.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
                model.PageNumber = pageNumber;
                model.PageSize = pageSize;
            }
            MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                SlidingExpiration = TimeSpan.FromMinutes(55),
                Size = 1024,
            };

            _MemoryCache.Set("KeyProdList_Scrap", modelList, cacheEntryOptions);
            return PartialView("_ProductionEntryDashboardGrid", model);
        }
        public async Task<IActionResult> GetProductData(string FromDate, string ToDate, int pageNumber = 1, int pageSize = 50, string SearchBox = "")
        {
            //model.Mode = "Search";
            var model = new ProductionEntryDashboard();
            model = await _IProductionEntry.GetProductData(FromDate, ToDate);
            model.DashboardType = "Product";
            var modelList = model?.ProductionDashboard ?? new List<ProductionEntryDashboard>();


            if (string.IsNullOrWhiteSpace(SearchBox))
            {
                model.TotalRecords = modelList.Count();
                model.PageNumber = pageNumber;
                model.PageSize = pageSize;
                model.ProductionDashboard = modelList
                .Skip((pageNumber - 1) * pageSize)
                   .Take(pageSize)
                   .ToList();
            }
            else
            {
                List<ProductionEntryDashboard> filteredResults;
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
                model.ProductionDashboard = filteredResults.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
                model.PageNumber = pageNumber;
                model.PageSize = pageSize;
            }
            MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                SlidingExpiration = TimeSpan.FromMinutes(55),
                Size = 1024,
            };

            _MemoryCache.Set("KeyProdList_Scrap", modelList, cacheEntryOptions);
            return PartialView("_ProductionEntryDashboardGrid", model);
        }
        [HttpGet]
        public IActionResult GlobalSearch(string searchString, string dashboardType = "Summary", int pageNumber = 1, int pageSize = 50)
        {
            ProductionEntryDashboard model = new ProductionEntryDashboard();
            if (string.IsNullOrWhiteSpace(searchString))
            {
                return PartialView("_ProductionEntryDashboardGrid", new List<ProductionEntryDashboard>());
            }
            string cacheKey = $"KeyProdList_{dashboardType}";
            if (!_MemoryCache.TryGetValue(cacheKey, out IList<ProductionEntryDashboard> productionEntryDashboard) || productionEntryDashboard == null)
            {
                return PartialView("_ProductionEntryDashboardGrid", new List<ProductionEntryDashboard>());
            }

            List<ProductionEntryDashboard> filteredResults;

            if (string.IsNullOrWhiteSpace(searchString))
            {
                filteredResults = productionEntryDashboard.ToList();
            }
            else
            {
                filteredResults = productionEntryDashboard
                    .Where(i => i.GetType().GetProperties()
                        .Where(p => p.PropertyType == typeof(string))
                        .Select(p => p.GetValue(i)?.ToString())
                        .Any(value => !string.IsNullOrEmpty(value) &&
                                      value.Contains(searchString, StringComparison.OrdinalIgnoreCase)))
                    .ToList();


                if (filteredResults.Count == 0)
                {
                    filteredResults = productionEntryDashboard.ToList();
                }
            }

            model.TotalRecords = filteredResults.Count;
            model.ProductionDashboard = filteredResults.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            model.PageNumber = pageNumber;
            model.PageSize = pageSize;

            return PartialView("_ProductionEntryDashboardGrid", model);
        }
        public async Task<IActionResult> ExportProductionDataToExcel(string ReportType)
        {
            string cacheKey = $"KeyProdList_{ReportType}";
            IList<ProductionEntryDashboard> stockRegisterList;

            // Check if data is in memory cache
            if (!_MemoryCache.TryGetValue(cacheKey, out stockRegisterList))
            {
                return NotFound("No data available to export.");
            }

            if (stockRegisterList == null || !stockRegisterList.Any())
                return NotFound("No data available to export.");

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("ProductionPlan Register");

            var reportGenerators = new Dictionary<string, Action<IXLWorksheet, IList<ProductionEntryDashboard>>>
            {
                { "Summary", ExportProductionSummary },
                { "Detail", ExportProductionDetail },
                { "DetailWithBatchwise", ExportProductionDetailWithBatchwise },
                { "Breakdown", ExportProductionBreakdown },
                { "Operation", ExportProductionOperation },
                { "Scrap", ExportProductionScrap },
            };

            if (reportGenerators.TryGetValue(ReportType, out var generator))
            {
                generator(worksheet, stockRegisterList);
            }
            else
            {
                return BadRequest("Invalid report type.");
            }

            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Position = 0;

            return File(
                stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "ProductionPlanReport.xlsx"
            );
        }
        private void ExportProductionSummary(IXLWorksheet sheet, IList<ProductionEntryDashboard> list)
        {
            string[] headers = {
            "Sr#","Part Code", "Item Name", "Production Date", "Production Slip No", "Produced Against",
    "New Production/Rework", "Production In WC", "Stage Description", "Scheduled Qty",
    "FG Produced Qty", "FG OK Qty", "FG Rejected Qty", "Next To Store", "Next To Work Center",
    "Transfer FG to WC/Store", "Rejected Qty In WC", "Rejected Qty", "Startup Rej Qty",
    "Transfer To QC", "Store WC", "BOM No", "BOM Date", "Machine Name", "QC Checked",
    "Production Plan No", "Production Plan Year Code", "Production Plan Date",
    "Production Plan Schedule No", "Production Plan Schedule Year", "Production Plan Schedule Date",
    "Req No", "Req Year Code", "Req Date", "Batch No", "Unique Batch No", "Start Time", "To Time",
    "Setup Time", "Previous WC", "Production Entry ID", "Entry Date", "Production Year Code",
    "WO Qty", "Pending Qty to Issue", "Rej Qty Due to Trial", "Pending Qty for Prod",
    "Pending Qty for QC", "Rejected Store", "QC Mandatory", "Produced In Line No", "QC Checked",
    "Initial Reading", "Final Reading", "Shots", "Completed", "Utilised Hours", "Production Line No",
    "Standard Shots", "Standard Cycle Time", "Remark", "Cyclic Time", "Production Hour",
    "Item Model", "Cavity", "Efficiency", "Actual Time Required", "Parent Prod Schedule No",
    "Parent Prod Schedule Date", "Parent Prod Schedule Year Code", "SO No", "SO Year Code", "SO Date",
    "SO Type", "QC Offer Date", "QC Qty", "OK Qty", "Stock Qty", "Material Transferred",
    "Rework QC Year Code", "Rework QC Date", "Shift Close", "Completed Production", "CC",
    "Entry By Machine No", "Actual Entry By Name", "Actual Entry Date", "Last Updated By",
    "Last Updated Date", "Entry Designation", "Operator Name", "Supervisor"
            };


            for (int i = 0; i < headers.Length; i++)
                sheet.Cell(1, i + 1).Value = headers[i];

            int row = 2, srNo = 1;
            foreach (var item in list)
            {
                sheet.Cell(row, 1).Value = srNo++; // Sr No
                sheet.Cell(row, 2).Value = item.FGPartCode;
                sheet.Cell(row, 3).Value = item.FGItemName;
                sheet.Cell(row, 4).Value = item.ProdDate?.Split(" ")[0] ?? "";
                sheet.Cell(row, 5).Value = item.ProdSlipNo;
                sheet.Cell(row, 6).Value = item.ProdAgainstPlanManual;
                sheet.Cell(row, 7).Value = item.NewProdRework;
                sheet.Cell(row, 8).Value = item.ProdInWC;
                sheet.Cell(row, 9).Value = item.StageDescription;
                sheet.Cell(row, 10).Value = item.ProdSchQty;
                sheet.Cell(row, 11).Value = item.FGProdQty;
                sheet.Cell(row, 12).Value = item.FGOKQty;
                sheet.Cell(row, 13).Value = item.FGRejQty;
                sheet.Cell(row, 14).Value = item.NextToStore;
                sheet.Cell(row, 15).Value = item.NextToWorkCenter;
                sheet.Cell(row, 16).Value = item.TransferFGToWCorSTORE;
                sheet.Cell(row, 17).Value = item.RejQtyInWC;
                sheet.Cell(row, 18).Value = item.RejQTy;
                sheet.Cell(row, 19).Value = item.startupRejQty;
                sheet.Cell(row, 20).Value = item.TransferToQc;
                sheet.Cell(row, 21).Value = item.StoreWC;
                sheet.Cell(row, 22).Value = item.BOMNO;
                sheet.Cell(row, 23).Value = item.BOMDate?.Split(" ")[0] ?? "";
                sheet.Cell(row, 24).Value = item.MachineName;
                sheet.Cell(row, 25).Value = item.QCChecked;
                sheet.Cell(row, 26).Value = item.ProdPlanNo;
                sheet.Cell(row, 27).Value = item.ProdPlanYearCode;
                sheet.Cell(row, 28).Value = item.ProdPlanDate?.Split(" ")[0] ?? "";
                sheet.Cell(row, 29).Value = item.ProdPlanSchNo;
                sheet.Cell(row, 30).Value = item.ProdPlanSchYearCode;
                sheet.Cell(row, 31).Value = item.ProdPlanSchDate?.Split(" ")[0] ?? "";
                sheet.Cell(row, 32).Value = item.Reqno;
                sheet.Cell(row, 33).Value = item.ReqThrBOMYearCode;
                sheet.Cell(row, 34).Value = item.ReqDate?.Split(" ")[0] ?? "";
                sheet.Cell(row, 35).Value = item.BatchNo;
                sheet.Cell(row, 36).Value = item.UniqueBatchNo;
                sheet.Cell(row, 37).Value = item.StartTime?.Split(" ")[0] ?? "";
                sheet.Cell(row, 38).Value = item.ToTime?.Split(" ")[0] ?? "";
                sheet.Cell(row, 39).Value = item.setupTime;
                sheet.Cell(row, 40).Value = item.PrevWC;
                sheet.Cell(row, 41).Value = item.PRODEntryId;
                sheet.Cell(row, 42).Value = item.Entrydate?.Split(" ")[0] ?? "";
                sheet.Cell(row, 43).Value = item.PRODYearcode;
                sheet.Cell(row, 44).Value = item.WOQTY;
                sheet.Cell(row, 45).Value = item.PendingQtyToIssue;
                sheet.Cell(row, 46).Value = item.RejQtyDuetoTrail;
                sheet.Cell(row, 47).Value = item.PendQtyForProd;
                sheet.Cell(row, 48).Value = item.PendQtyForQC;
                sheet.Cell(row, 49).Value = item.rejStore;
                sheet.Cell(row, 50).Value = item.QCMandatory;
                sheet.Cell(row, 51).Value = item.ProducedINLineNo;
                sheet.Cell(row, 52).Value = item.QCChecked;
                sheet.Cell(row, 53).Value = item.InitialReading;
                sheet.Cell(row, 54).Value = item.FinalReading;
                sheet.Cell(row, 55).Value = item.Shots;
                sheet.Cell(row, 56).Value = item.Completed;
                sheet.Cell(row, 57).Value = item.UtilisedHours;
                sheet.Cell(row, 58).Value = item.ProdLineNo;
                sheet.Cell(row, 59).Value = item.stdShots;
                sheet.Cell(row, 60).Value = item.stdCycletime;
                sheet.Cell(row, 61).Value = item.Remark;
                sheet.Cell(row, 62).Value = item.CyclicTime;
                sheet.Cell(row, 63).Value = item.ProductionHour;
                sheet.Cell(row, 64).Value = item.ItemModel;
                sheet.Cell(row, 65).Value = item.cavity;
                sheet.Cell(row, 66).Value = item.efficiency;
                sheet.Cell(row, 67).Value = item.ActualTimeRequired;
                sheet.Cell(row, 68).Value = item.parentProdSchNo;
                sheet.Cell(row, 69).Value = item.parentProdSchDate;
                sheet.Cell(row, 70).Value = item.parentProdSchYearcode;
                sheet.Cell(row, 71).Value = item.SONO;
                sheet.Cell(row, 72).Value = item.SOYearcode;
                sheet.Cell(row, 73).Value = item.SODate?.Split(" ")[0] ?? "";
                sheet.Cell(row, 74).Value = item.sotype;
                sheet.Cell(row, 75).Value = item.QCOfferDate?.Split(" ")[0] ?? "";
                sheet.Cell(row, 76).Value = item.QCQTy;
                sheet.Cell(row, 77).Value = item.OKQty;
                sheet.Cell(row, 78).Value = item.StockQTy;
                sheet.Cell(row, 79).Value = item.matTransferd;
                sheet.Cell(row, 80).Value = item.RewQcYearCode;
                sheet.Cell(row, 81).Value = item.RewQcDate?.Split(" ")[0] ?? "";
                sheet.Cell(row, 82).Value = item.shiftclose;
                sheet.Cell(row, 83).Value = item.ComplProd;
                sheet.Cell(row, 84).Value = item.CC;
                sheet.Cell(row, 85).Value = item.EntryByMachineNo;
                sheet.Cell(row, 86).Value = item.ActualEmpByName;
                sheet.Cell(row, 87).Value = item.ActualEntryDate?.Split(" ")[0] ?? "";
                sheet.Cell(row, 88).Value = item.LastUpdatedByName;
                sheet.Cell(row, 89).Value = item.LastUpdationDate;
                sheet.Cell(row, 90).Value = item.EntryByDesignation;
                sheet.Cell(row, 91).Value = item.OperatorName;
                sheet.Cell(row, 92).Value = item.supervisior;


                row++;
            }
        }
        private void ExportProductionDetail(IXLWorksheet sheet, IList<ProductionEntryDashboard> list)
        {
            string[] headers = {
            "Sr#","WONO", "WO Date", "WO Status", "Effective From", "Effective Till", "Entry Date", "Year Code",
    "Remark (Production)", "Remark (Supply Stage)", "Remark (Routing)", "Remark (Packing)",
    "Other Instruction", "Billing Status", "Pending Route Sheet", "Approved By", "Approved Date",
    "Close WO", "Close Date", "WO Rev No", "WO Rev Date", "Actual Entry By", "Actual Entry Date",
    "Last Updated By", "Last Updated Date", "Machine Name"
            };
            for (int i = 0; i < headers.Length; i++)
                sheet.Cell(1, i + 1).Value = headers[i];

            int row = 2, srNo = 1;
            foreach (var item in list)
            {
                sheet.Cell(row, 1).Value = srNo++;
                sheet.Cell(row, 2).Value = item.NewProdRework;
                sheet.Cell(row, 3).Value = item.ProdSlipNo;
                sheet.Cell(row, 4).Value = item.ProdDate?.Split(" ")[0];
                sheet.Cell(row, 5).Value = item.FGPartCode;
                sheet.Cell(row, 6).Value = item.FGItemName;
                sheet.Cell(row, 7).Value = item.WorkCenter;
                sheet.Cell(row, 8).Value = item.ProdQty;
                sheet.Cell(row, 9).Value = item.Unit;
                sheet.Cell(row, 10).Value = item.RMPartCode;
                sheet.Cell(row, 11).Value = item.RMItemName;
                sheet.Cell(row, 12).Value = item.ConsumedRMQTY;
                sheet.Cell(row, 13).Value = item.ConsumedRMUnit;
                sheet.Cell(row, 14).Value = item.MainRMQTY;
                sheet.Cell(row, 15).Value = item.MainRMUnit;
                sheet.Cell(row, 16).Value = item.TotalReqRMQty;
                sheet.Cell(row, 17).Value = item.TotalStock;
                sheet.Cell(row, 18).Value = item.AltRMQty;
                sheet.Cell(row, 19).Value = item.AltRMUnit;
                sheet.Cell(row, 20).Value = item.RMNetWt;
                sheet.Cell(row, 21).Value = item.GrossWt;
                sheet.Cell(row, 22).Value = item.BOMNO;
                sheet.Cell(row, 23).Value = item.BOMRevDate?.Split(" ")[0];
                sheet.Cell(row, 24).Value = item.ManualAutoEntry;
                sheet.Cell(row, 25).Value = item.EntryId;
                sheet.Cell(row, 26).Value = item.Yearcode;
                sheet.Cell(row, 27).Value = item.QCMandatory;
                sheet.Cell(row, 28).Value = item.PendQtyForQC;

                row++;
            }

        }
        private void ExportProductionDetailWithBatchwise(IXLWorksheet sheet, IList<ProductionEntryDashboard> list)
        {
            string[] headers = {
            "Sr#",    "NewProdRework", "ProdSlipNo", "ProdDate", "FGPartCode", "FGItemName", "WorkCenter", "ProdQty",
    "RMPartCode", "RMItemName", "Unit", "ConsumedRMQTY", "ConsumedRMUnit", "MainRMQTY", "MainRMUnit",
    "FGProdQty", "TotalReqRMQty", "TotalStock", "BatchStock", "AltRMQty", "AltRMUnit",
    "RMNetWt", "GrossWt", "Batchwise", "BatchNo", "UniqueBatchNo",
    "BOMNO", "BOMRevDate", "ManualAutoEntry", "EntryId", "Yearcode",
    "QCMandatory", "PendQtyForQC"
            };


            for (int i = 0; i < headers.Length; i++)
                sheet.Cell(1, i + 1).Value = headers[i];

            int row = 2, srNo = 1;
            foreach (var item in list)
            {
                sheet.Cell(row, 1).Value = srNo++;
                sheet.Cell(row, 2).Value = item.NewProdRework;
                sheet.Cell(row, 3).Value = item.ProdSlipNo;
                sheet.Cell(row, 4).Value = item.ProdDate;
                sheet.Cell(row, 5).Value = item.FGPartCode;
                sheet.Cell(row, 6).Value = item.FGItemName;
                sheet.Cell(row, 7).Value = item.WorkCenter;
                sheet.Cell(row, 8).Value = item.ProdQty;
                sheet.Cell(row, 9).Value = item.RMPartCode;
                sheet.Cell(row, 10).Value = item.RMItemName;
                sheet.Cell(row, 11).Value = item.Unit;
                sheet.Cell(row, 12).Value = item.ConsumedRMQTY;
                sheet.Cell(row, 13).Value = item.ConsumedRMUnit;
                sheet.Cell(row, 14).Value = item.MainRMQTY;
                sheet.Cell(row, 15).Value = item.MainRMUnit;
                sheet.Cell(row, 16).Value = item.FGProdQty;
                sheet.Cell(row, 17).Value = item.TotalReqRMQty;
                sheet.Cell(row, 18).Value = item.TotalStock;
                sheet.Cell(row, 19).Value = item.BatchStock;
                sheet.Cell(row, 20).Value = item.AltRMQty;
                sheet.Cell(row, 21).Value = item.AltRMUnit;
                sheet.Cell(row, 22).Value = item.RMNetWt;
                sheet.Cell(row, 23).Value = item.GrossWt;
                sheet.Cell(row, 24).Value = item.Batchwise;
                sheet.Cell(row, 25).Value = item.BatchNo;
                sheet.Cell(row, 26).Value = item.UniqueBatchNo;
                sheet.Cell(row, 27).Value = item.BOMNO;
                sheet.Cell(row, 28).Value = item.BOMRevDate;
                sheet.Cell(row, 29).Value = item.ManualAutoEntry;
                sheet.Cell(row, 30).Value = item.EntryId;
                sheet.Cell(row, 31).Value = item.Yearcode;
                sheet.Cell(row, 32).Value = item.QCMandatory;
                sheet.Cell(row, 33).Value = item.PendQtyForQC;

                row++;
            }
        }
        private void ExportProductionBreakdown(IXLWorksheet sheet, IList<ProductionEntryDashboard> list)
        {
            string[] headers = {
            "Sr#", "Entry ID", "Year Code", "Prod Date", "Break From Time", "Break To Time", "Reason Detail",
    "Break Time (Min)", "Responsible Employee Name", "Responsible Factor", "QC Mandatory", "Pending Qty for QC"
            };


            for (int i = 0; i < headers.Length; i++)
                sheet.Cell(1, i + 1).Value = headers[i];

            int row = 2, srNo = 1;
            foreach (var item in list)
            {
                sheet.Cell(row, 1).Value = srNo++;
                sheet.Cell(row, 1).Value = srNo++;
                sheet.Cell(row, 2).Value = item.EntryId;
                sheet.Cell(row, 3).Value = item.Yearcode;
                sheet.Cell(row, 4).Value = item.Proddate?.Split(" ")[0];
                sheet.Cell(row, 5).Value = item.BreakfromTime;
                sheet.Cell(row, 6).Value = item.BreaktoTime;
                sheet.Cell(row, 7).Value = item.ResonDetail;
                sheet.Cell(row, 8).Value = item.BreakTimeMin;
                sheet.Cell(row, 9).Value = item.ResEmpName;
                sheet.Cell(row, 10).Value = item.ResFactor;
                sheet.Cell(row, 11).Value = item.QCMandatory;
                sheet.Cell(row, 12).Value = item.PendQtyForQC;

                row++;
            }
        }
        private void ExportProductionOperation(IXLWorksheet sheet, IList<ProductionEntryDashboard> list)
        {
            string[] headers = {
            "Sr#", "EntryId", "Yearcode", "NewProdRework", "ProdSlipNo", "ProdDate",
    "FGPartCode", "FGItemName", "WorkCenter", "MachineName", "OperatorName",
    "Fromtime", "ToTime", "TotalHrs", "OverTimeHrs", "MachineCharges",
    "QCMandatory", "PendQtyForQC"
            };


            for (int i = 0; i < headers.Length; i++)
                sheet.Cell(1, i + 1).Value = headers[i];

            int row = 2, srNo = 1;
            foreach (var item in list)
            {
                sheet.Cell(row, 1).Value = srNo++;
                sheet.Cell(row, 2).Value = item.EntryId;
                sheet.Cell(row, 3).Value = item.Yearcode;
                sheet.Cell(row, 4).Value = item.NewProdRework;
                sheet.Cell(row, 5).Value = item.ProdSlipNo;
                sheet.Cell(row, 6).Value = item.ProdDate?.Split(" ")[0];
                sheet.Cell(row, 7).Value = item.FGPartCode;
                sheet.Cell(row, 8).Value = item.FGItemName;
                sheet.Cell(row, 9).Value = item.WorkCenter;
                sheet.Cell(row, 10).Value = item.MachineName;
                sheet.Cell(row, 11).Value = item.OperatorName;
                sheet.Cell(row, 12).Value = item.Fromtime;
                sheet.Cell(row, 13).Value = item.ToTime;
                sheet.Cell(row, 14).Value = item.TotalHrs;
                sheet.Cell(row, 15).Value = item.OverTimeHrs;
                sheet.Cell(row, 16).Value = item.MachineCharges;
                sheet.Cell(row, 17).Value = item.QCMandatory;
                sheet.Cell(row, 18).Value = item.PendQtyForQC;

                row++;
            }
        }
        private void ExportProductionScrap(IXLWorksheet sheet, IList<ProductionEntryDashboard> list)
        {
            string[] headers = {
            "Sr#","Entry ID", "Year Code", "New Prod/Rework", "Prod Slip No", "Prod Date",
    "FG Part Code", "FG Item Name", "Work Center", "Machine Name", "Scrap Type",
    "Scrap Qty", "Scrap Unit", "QC Mandatory", "Pending Qty for QC"
            };


            for (int i = 0; i < headers.Length; i++)
                sheet.Cell(1, i + 1).Value = headers[i];

            int row = 2, srNo = 1;
            foreach (var item in list)
            {
                sheet.Cell(row, 1).Value = srNo++;
                sheet.Cell(row, 1).Value = srNo++;
                sheet.Cell(row, 2).Value = item.EntryId;
                sheet.Cell(row, 3).Value = item.Yearcode;
                sheet.Cell(row, 4).Value = item.NewProdRework;
                sheet.Cell(row, 5).Value = item.ProdSlipNo;
                sheet.Cell(row, 6).Value = item.ProdDate?.Split(" ")[0];
                sheet.Cell(row, 7).Value = item.FGPartCode;
                sheet.Cell(row, 8).Value = item.FGItemName;
                sheet.Cell(row, 9).Value = item.WorkCenter;
                sheet.Cell(row, 10).Value = item.MachineName;
                sheet.Cell(row, 11).Value = item.ScrapType;
                sheet.Cell(row, 12).Value = item.ScrapQty;
                sheet.Cell(row, 13).Value = item.Scrapunit;
                sheet.Cell(row, 14).Value = item.QCMandatory;
                sheet.Cell(row, 15).Value = item.PendQtyForQC;

                row++;
            }
        }
        public async Task<ProductionEntryModel> BindModels(ProductionEntryModel model)
        {
            if (model == null)
            {
                model = new ProductionEntryModel();
                model.YearCode = Constants.FinincialYear;
                model.EntryId = _IDataLogic.GetEntryID("SP_ProductionEntry", Constants.FinincialYear, "PRODEntryId", "PRODYearcode");
                model.EntryTime = DateTime.Now.ToString("hh:mm tt");

            }
            return model;
        }
        public async Task<JsonResult> GetFeatureOption()
        {
            var JSON = await _IProductionEntry.GetFeatureOption("FeatureOption", "SP_ProductionEntry");
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetDefaultBranch()
        {
            var username = HttpContext.Session.GetString("Branch");
            return Json(username);
        }
        public async Task<JsonResult> CCEnableDisable()
        {
            var JSON = await _IProductionEntry.CCEnableDisable();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetServerDate()
        {
            try
            {
                var time = CommonFunc.ParseFormattedDate(DateTime.Now.ToString());
                return Json(DateTime.Now.ToString("yyyy-MM-dd"));
            }
            catch (HttpRequestException ex)
            {
                // Log the exception for debugging purposes
                Console.WriteLine($"HttpRequestException: {ex.Message}");
                return Json(new { error = "Failed to fetch server date and time: " + ex.Message });
            }
            catch (Exception ex)
            {
                // Log any other unexpected exceptions
                Console.WriteLine($"Unexpected Exception: {ex.Message}");
                return Json(new { error = "An unexpected error occurred: " + ex.Message });
            }
        }
        public IActionResult EditItemRow(int SeqNo)
        {
            string serializedProductionGrid = HttpContext.Session.GetString("KeyProductionEntryGrid");
            List<ProductionEntryItemDetail> ProductionEntryItemDetail = new();
            if (!string.IsNullOrEmpty(serializedProductionGrid))
            {
                ProductionEntryItemDetail = JsonConvert.DeserializeObject<List<ProductionEntryItemDetail>>(serializedProductionGrid);
            }
            var SSGrid = ProductionEntryItemDetail.Where(x => x.SeqNo == SeqNo);
            string JsonString = JsonConvert.SerializeObject(SSGrid);
            return Json(JsonString);
        }
        public IActionResult DeleteItemRow(int SeqNo, string ProdType)
        {
            var MainModel = new ProductionEntryModel();
            string serializedProductionGrid = HttpContext.Session.GetString("KeyProductionEntryGrid");
            List<ProductionEntryItemDetail> ProductionEntryItemDetail = new();
            if (!string.IsNullOrEmpty(serializedProductionGrid))
            {
                ProductionEntryItemDetail = JsonConvert.DeserializeObject<List<ProductionEntryItemDetail>>(serializedProductionGrid);
            }
            int Indx = Convert.ToInt32(SeqNo) - 1;

            if (ProductionEntryItemDetail != null && ProductionEntryItemDetail.Count > 0)
            {
                ProductionEntryItemDetail.RemoveAt(Convert.ToInt32(Indx));

                Indx = 0;

                foreach (var item in ProductionEntryItemDetail)
                {
                    Indx++;
                    item.SeqNo = Indx;
                }
                MainModel.ItemDetailGrid = ProductionEntryItemDetail;
                if (ProductionEntryItemDetail.Count == 0)
                {
                    HttpContext.Session.Remove("KeyProductionEntryGrid");
                }
                //_MemoryCache.Set("KeyMaterialReceiptGrid", MainModel.ItemDetailGrid, cacheEntryOptions);
            }
            MainModel.ProdType = ProdType;
            return PartialView("_ProductionEntryGrid", MainModel);
        }
        public IActionResult AddProductionEntryGrid(List<ProductionEntryItemDetail> model)
        {
            try
            {

                var MainModel = new ProductionEntryModel();
                var ProductionEntryDetail = new List<ProductionEntryItemDetail>();
                var ProductionGrid = new List<ProductionEntryItemDetail>();
                var SSGrid = new List<ProductionEntryItemDetail>();
                var seqNo = 0;
                foreach (var item in model)
                {
                    string serializedProductionGrid = HttpContext.Session.GetString("KeyProductionEntryGrid");
                    List<ProductionEntryItemDetail> ProductionEntryItemDetail = new();
                    if (!string.IsNullOrEmpty(serializedProductionGrid))
                    {
                        ProductionEntryItemDetail = JsonConvert.DeserializeObject<List<ProductionEntryItemDetail>>(serializedProductionGrid);
                    }
                    if (item != null)
                    {
                        if (ProductionEntryItemDetail == null)
                        {
                            item.SeqNo = seqNo;
                            item.SeqNo += seqNo + 1;
                            ProductionEntryDetail.Add(item);
                            seqNo++;
                        }
                        else
                        {
                            item.SeqNo = ProductionEntryDetail.Count + 1;
                            ProductionEntryDetail = ProductionEntryDetail.Where(x => x != null).ToList();
                            SSGrid.AddRange(ProductionEntryDetail);
                            ProductionEntryDetail.Add(item);
                        }
                        MainModel.ItemDetailGrid = ProductionEntryDetail;
                        var ProdType = item.ProdType;
                        MainModel.ProdType = ProdType;
                        var ProdEntryAllow = item.ProdEntryAllowToAddRMItem;
                        MainModel.ProdEntryAllowToAddRMItem = ProdEntryAllow;
                        string serializedGrid = JsonConvert.SerializeObject(MainModel.ItemDetailGrid);
                        HttpContext.Session.SetString("KeyProductionEntryGrid", serializedGrid);
                    }
                }
                return PartialView("_ProductionEntryGrid", MainModel);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IActionResult AddChilddetailGrid(ProductionEntryItemDetail modelData)
        {
            try
            {
                string serializedProductionGrid = HttpContext.Session.GetString("KeyProductionEntryGrid");
                List<ProductionEntryItemDetail> ProductionEntryItemDetail = new();
                if (!string.IsNullOrEmpty(serializedProductionGrid))
                {
                    ProductionEntryItemDetail = JsonConvert.DeserializeObject<List<ProductionEntryItemDetail>>(serializedProductionGrid);
                }
                var MainModel = new ProductionEntryModel();
                var ProductionEntryGrid = new List<ProductionEntryItemDetail>();
                var ProductionGrid = new List<ProductionEntryItemDetail>();
                var SSGrid = new List<ProductionEntryItemDetail>();
                var seqNo = 0;

                if (modelData != null)
                {
                    if (ProductionEntryItemDetail == null)
                    {
                        modelData.SeqNo += seqNo + 1;
                        ProductionEntryGrid.Add(modelData);
                        seqNo++;
                    }
                    else
                    {
                        modelData.SeqNo = ProductionEntryItemDetail.Count + 1;
                        ProductionEntryGrid = ProductionEntryItemDetail.Where(x => x != null && x.BatchNo != modelData.BatchNo).ToList();
                        SSGrid.AddRange(ProductionEntryGrid);
                        ProductionEntryGrid.Add(modelData);
                    }
                    MainModel.ItemDetailGrid = ProductionEntryGrid;
                    var ProdType = modelData.ProdType;
                    MainModel.ProdType = ProdType;
                    var ProdEntryAllow = modelData.ProdEntryAllowToAddRMItem;
                    MainModel.ProdEntryAllowToAddRMItem = ProdEntryAllow;
                    string serializedGrid = JsonConvert.SerializeObject(MainModel.ItemDetailGrid);
                    HttpContext.Session.SetString("KeyProductionEntryGrid", serializedGrid);
                }

                return PartialView("_ProductionEntryGrid", MainModel);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IActionResult AddOperatordetailGrid(ProductionEntryItemDetail model)
        {
            try
            {
                string serializedOperatorGrid = HttpContext.Session.GetString("KeyProductionEntryOperatordetail");
                List<ProductionEntryItemDetail> ProductionEntryOperatorDetail = new();
                if (!string.IsNullOrEmpty(serializedOperatorGrid))
                {
                    ProductionEntryOperatorDetail = JsonConvert.DeserializeObject<List<ProductionEntryItemDetail>>(serializedOperatorGrid);
                }
                var MainModel = new ProductionEntryModel();
                var ProductionEntryGrid = new List<ProductionEntryItemDetail>();
                var ProductionGrid = new List<ProductionEntryItemDetail>();
                var SSGrid = new List<ProductionEntryItemDetail>();
                if (model != null)
                {
                    if (ProductionEntryOperatorDetail == null)
                    {
                        model.SeqNo = 1;
                        ProductionEntryGrid.Add(model);
                    }
                    else
                    {
                        if (ProductionEntryOperatorDetail.Where(x => x.OperatorName == model.OperatorName).Any())
                        {
                            return StatusCode(207, "Duplicate");
                        }
                        else
                        {
                            model.SeqNo = ProductionEntryOperatorDetail.Count + 1;
                            ProductionEntryGrid = ProductionEntryOperatorDetail.Where(x => x != null).ToList();
                            SSGrid.AddRange(ProductionEntryGrid);
                            ProductionEntryGrid.Add(model);
                        }
                    }
                    MainModel.OperatorDetailGrid = ProductionEntryGrid;
                    string serializedGrid = JsonConvert.SerializeObject(MainModel.OperatorDetailGrid);
                    HttpContext.Session.SetString("KeyProductionEntryOperatordetail", serializedGrid);
                }

                return PartialView("_ProductionEntryOperatorDetail", MainModel);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IActionResult AddBreakdowndetailGrid(ProductionEntryItemDetail model)
        {
            try
            {
                string serializedBreakdownGrid = HttpContext.Session.GetString("KeyProductionEntryBreakdowndetail");
                List<ProductionEntryItemDetail> ProductionEntryBreakdownDetail = new();
                if (!string.IsNullOrEmpty(serializedBreakdownGrid))
                {
                    ProductionEntryBreakdownDetail = JsonConvert.DeserializeObject<List<ProductionEntryItemDetail>>(serializedBreakdownGrid);
                }

                var MainModel = new ProductionEntryModel();
                var ProductionEntryGrid = new List<ProductionEntryItemDetail>();
                var ProductionGrid = new List<ProductionEntryItemDetail>();
                var SSGrid = new List<ProductionEntryItemDetail>();

                if (model != null)
                {
                    if (ProductionEntryBreakdownDetail == null)
                    {
                        model.SeqNo = 1;
                        ProductionEntryGrid.Add(model);
                    }
                    else
                    {
                        if (ProductionEntryBreakdownDetail.Where(x => x.ReasonDetail == model.ReasonDetail).Any())
                        {
                            return StatusCode(207, "Duplicate");
                        }
                        else
                        {
                            model.SeqNo = ProductionEntryBreakdownDetail.Count + 1;
                            ProductionEntryGrid = ProductionEntryBreakdownDetail.Where(x => x != null).ToList();
                            SSGrid.AddRange(ProductionEntryGrid);
                            ProductionEntryGrid.Add(model);
                        }
                    }
                    MainModel.BreakdownDetailGrid = ProductionEntryGrid;
                    string serializedGrid = JsonConvert.SerializeObject(MainModel.ItemDetailGrid);
                    HttpContext.Session.SetString("KeyProductionEntryBreakdowndetail", serializedGrid);
                }

                return PartialView("_ProductionEntryBreakdownDetail", MainModel);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IActionResult AddScrapdetailGrid(ProductionEntryItemDetail model)
        {
            try
            {
                string serializedScrapGrid = HttpContext.Session.GetString("KeyProductionEntryScrapdetail");
                List<ProductionEntryItemDetail> ProductionEntryScrapDetail = new();
                if (!string.IsNullOrEmpty(serializedScrapGrid))
                {
                    ProductionEntryScrapDetail = JsonConvert.DeserializeObject<List<ProductionEntryItemDetail>>(serializedScrapGrid);
                }

                var MainModel = new ProductionEntryModel();
                var ProductionEntryGrid = new List<ProductionEntryItemDetail>();
                var ProductionGrid = new List<ProductionEntryItemDetail>();
                var SSGrid = new List<ProductionEntryItemDetail>();

                if (model != null)
                {
                    if (ProductionEntryScrapDetail == null)
                    {
                        model.SeqNo = 1;
                        ProductionEntryGrid.Add(model);
                    }
                    else
                    {
                        if (ProductionEntryScrapDetail.Where(x => x.ScrapPartCode == model.ScrapPartCode && x.ScrapItemName == model.ScrapItemName).Any())
                        {
                            return StatusCode(207, "Duplicate");
                        }
                        model.SeqNo = ProductionEntryScrapDetail.Count + 1;
                        ProductionEntryGrid = ProductionEntryScrapDetail.Where(x => x != null).ToList();
                        SSGrid.AddRange(ProductionEntryGrid);
                        ProductionEntryGrid.Add(model);
                    }
                    MainModel.ScrapDetailGrid = ProductionEntryGrid;
                    string serializedGrid = JsonConvert.SerializeObject(MainModel.ItemDetailGrid);
                    HttpContext.Session.SetString("KeyProductionEntryScrapdetail", serializedGrid);
                }

                return PartialView("_ProductionEntryScrapDetail", MainModel);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IActionResult AddProductdetailGrid(ProductionEntryItemDetail model)
        {
            try
            {
                string serializedProductGrid = HttpContext.Session.GetString("KeyProductionEntryProductdetail");
                List<ProductionEntryItemDetail> ProductionEntryProductDetail = new();
                if (!string.IsNullOrEmpty(serializedProductGrid))
                {
                    ProductionEntryProductDetail = JsonConvert.DeserializeObject<List<ProductionEntryItemDetail>>(serializedProductGrid);
                }

                var MainModel = new ProductionEntryModel();
                var ProductionEntryGrid = new List<ProductionEntryItemDetail>();
                var ProductionGrid = new List<ProductionEntryItemDetail>();
                var SSGrid = new List<ProductionEntryItemDetail>();

                if (model != null)
                {
                    if (ProductionEntryProductDetail == null)
                    {
                        model.SeqNo = 1;
                        ProductionEntryGrid.Add(model);
                    }
                    else
                    {
                        if (ProductionEntryProductDetail.Where(x => x.ProductPartCode == model.ProductPartCode && x.ProductItemName == model.ProductItemName).Any())
                        {
                            return StatusCode(207, "Duplicate");
                        }
                        model.SeqNo = ProductionEntryProductDetail.Count + 1;
                        ProductionEntryGrid = ProductionEntryProductDetail.Where(x => x != null).ToList();
                        SSGrid.AddRange(ProductionEntryGrid);
                        ProductionEntryGrid.Add(model);
                    }
                    MainModel.ProductDetailGrid = ProductionEntryGrid;
                    string serializedGrid = JsonConvert.SerializeObject(MainModel.ProductDetailGrid);
                    HttpContext.Session.SetString("KeyProductionEntryProductdetail", serializedGrid);
                }

                return PartialView("_ProductionEntryProductDetail", MainModel);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<JsonResult> AltUnitConversion(int ItemCode, int AltQty, int UnitQty)
        {
            var JSON = await _IProductionEntry.AltUnitConversion(ItemCode, AltQty, UnitQty);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetAccountList(string Check)
        {
            var JSON = await _IDataLogic.GetDropDownList("CREDITORDEBTORLIST", Check, "SP_GetDropDownList");
            _logger.LogError(JsonConvert.SerializeObject(JSON));
            return Json(JSON);
        }
        public async Task<JsonResult> GetBranchList(string Check)
        {
            var JSON = await _IDataLogic.GetDropDownList("BRANCHLIST", Check, "SP_GetDropDownList");
            _logger.LogError(JsonConvert.SerializeObject(JSON));
            return Json(JSON);
        }
        public async Task<JsonResult> FillEntryandGate(int YearCode)
        {
            var JSON = await _IProductionEntry.FillEntryandGate("NewEntryId", YearCode, "SP_ProductionEntry");
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillShift()
        {
            var JSON = await _IProductionEntry.FillShift();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillTool()
        {
            var JSON = await _IProductionEntry.FillTool();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<IActionResult> FillScrapData(int FGItemCode, decimal FgProdQty, string BomNo)
        {
            var model = new ProductionEntryModel();
            try
            {
                var response = await _IProductionEntry.FillScrapData(FGItemCode, FgProdQty, BomNo);
                model.ScrapDetailGrid = response.ScrapDetailGrid;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            string serializedGrid = JsonConvert.SerializeObject(model.ScrapDetailGrid);
            HttpContext.Session.SetString("KeyProductionEntryScrapdetail", serializedGrid);
            return PartialView("_ProductionEntryScrapDetail", model);
        }

        public async Task<IActionResult> FillProductDetail(int FGItemCode, decimal FgProdQty, string BomNo)
        {
            var model = new ProductionEntryModel();
            try
            {
                var response = await _IProductionEntry.FillProductDetail(FGItemCode, FgProdQty, BomNo);
                model.ProductDetailGrid = response.ProductDetailGrid;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            string serializedGrid = JsonConvert.SerializeObject(model.ProductDetailGrid);
            HttpContext.Session.SetString("KeyProductionEntryProductdetail", serializedGrid);
            return PartialView("_ProductionEntryProductDetail", model);
        }

        public async Task<JsonResult> FillShiftTime(int ShiftId)
        {
            var JSON = await _IProductionEntry.FillShiftTime(ShiftId);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillStore()
        {
            var JSON = await _IProductionEntry.FillStore();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetWorkCenterTotalStock(int ItemCode, int WcId, string TillDate)
        {
            var JSON = await _IProductionEntry.GetWorkCenterTotalStock("GETWIPotalSTOCK", ItemCode, WcId, TillDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetWorkCenterQty(int ItemCode, int WcId, string TillDate, string BatchNo, string UniqueBatchNo)
        {
            var JSON = await _IProductionEntry.GetWorkCenterQty("GETWIPSTOCKBATCHWISE", ItemCode, WcId, TillDate, BatchNo, UniqueBatchNo);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetUnit(int RmItemCode)
        {
            var JSON = await _IProductionEntry.GetUnit(RmItemCode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetScrapUnit(int ScrapItemCode)
        {
            var JSON = await _IProductionEntry.GetScrapUnit(ScrapItemCode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillTransferWorkCenter()
        {
            var JSON = await _IProductionEntry.FillWorkcenter();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillWorkcenter()
        {
            var JSON = await _IProductionEntry.FillWorkcenter();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillIssWorkcenter(string QcMandatory)
        {
            var JSON = await _IProductionEntry.FillIssWorkcenter(QcMandatory);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillIssWorkcenterForQcMandatory(string QcMandatory)
        {
            var JSON = await _IProductionEntry.FillIssWorkcenterForQcMandatory(QcMandatory);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> CheckForEditandDelete(string ProdSlipNo, int YearCode)
        {
            var JSON = await _IProductionEntry.CheckForEditandDelete(ProdSlipNo, YearCode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillIssStore()
        {
            var JSON = await _IProductionEntry.FillIssStore();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillOperation(int ItemCode, int WcId)
        {
            var JSON = await _IProductionEntry.FillOperation(ItemCode, WcId);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillMachineGroup(int machineId)
        {
            var JSON = await _IProductionEntry.FillMachineGroup(machineId);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillMachineName(int groupId)
        {
            var JSON = await _IProductionEntry.FillMachineName(groupId);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillSuperwiser()
        {
            var JSON = await _IProductionEntry.FillSuperwiser();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillOperator()
        {
            var JSON = await _IProductionEntry.FillOperator();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillOperatorName()
        {
            var JSON = await _IProductionEntry.FillOperatorName();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillBreakdownreason()
        {
            var JSON = await _IProductionEntry.FillBreakdownreason();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillResponsibleEmp()
        {
            var JSON = await _IProductionEntry.FillResponsibleEmp();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillScrapItems()
        {
            var JSON = await _IProductionEntry.FillScrapItems();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillProductItems(int FgItemCode, string BomNo)
        {
            var JSON = await _IProductionEntry.FillProductItems(FgItemCode, BomNo);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillProductPartCode(int FgItemCode, string BomNo)
        {
            var JSON = await _IProductionEntry.FillProductPartCode(FgItemCode, BomNo);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillProductUnit(int ProductItemCode)
        {
            var JSON = await _IProductionEntry.FillProductUnit(ProductItemCode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillResFactor()
        {
            var JSON = await _IProductionEntry.FillResFactor();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillScrapPartCode()
        {
            var JSON = await _IProductionEntry.FillScrapPartCode();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillScrapType()
        {
            var JSON = await _IProductionEntry.FillScrapType();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillProductType()
        {
            var JSON = await _IProductionEntry.FillProductType();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillRMItemName()
        {
            var JSON = await _IProductionEntry.FillRMItemName();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillRMPartCode()
        {
            var JSON = await _IProductionEntry.FillRMPartCode();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillAltItemName()
        {
            var JSON = await _IProductionEntry.FillAltItemName();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillAltPartCode()
        {
            var JSON = await _IProductionEntry.FillAltPartCode();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> CheckAllowToAddNegativeStock()
        {
            var JSON = await _IProductionEntry.CheckAllowToAddNegativeStock();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetLastProddate(int YearCode)
        {
            var JSON = await _IProductionEntry.GetLastProddate(YearCode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetBatchNumber(int ItemCode, int YearCode, float WcId, string TransDate, string BatchNo)
        {
            var JSON = await _IProductionEntry.GetBatchNumber("FillCurrentBatchINWIP", ItemCode, YearCode, WcId, TransDate, BatchNo);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> DisplayBomDetail(int ItemCode, float WOQty, int BomRevNo)
        {
            var JSON = await _IProductionEntry.DisplayBomDetail(ItemCode, WOQty, BomRevNo);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> DisplayRoutingDetail(int ItemCode)
        {
            var JSON = await _IProductionEntry.DisplayRoutingDetail(ItemCode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetItems(string ProdAgainst, int YearCode, string ItemName,int WCID)
        {
            var JSON = await _IProductionEntry.GetItems(ProdAgainst, YearCode,ItemName,WCID);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetPartCode(string ProdAgainst, int YearCode, string PartCode,int WCID)
        {
            var JSON = await _IProductionEntry.GetPartCode(ProdAgainst, YearCode   ,PartCode, WCID);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillBomNo(int ItemCode)
        {
            var JSON = await _IProductionEntry.FillBomNo(ItemCode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetProcessDetail(int ItemCode, int ProcessId, int WcId)
        {
            var JSON = await _IProductionEntry.GetProcessDetail(ItemCode, ProcessId, WcId);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public IActionResult DeleteByItemCode(int SeqNo)
        {
            var MainModel = new ProductionEntryModel();
            string serializedGrid = HttpContext.Session.GetString("KeyProductionGridOnLoad");
            List<ProductionEntryItemDetail> EntryItemDetail = new();
            if (!string.IsNullOrEmpty(serializedGrid))
            {
                EntryItemDetail = JsonConvert.DeserializeObject<List<ProductionEntryItemDetail>>(serializedGrid);
            }
            int Indx = Convert.ToInt32(SeqNo) - 1;

            if (EntryItemDetail != null && EntryItemDetail.Count > 0)
            {
                EntryItemDetail.RemoveAt(Convert.ToInt32(Indx));

                Indx = 0;

                foreach (var item in EntryItemDetail)
                {
                    Indx++;
                    item.SeqNo = Indx;
                }
                MainModel.ItemDetailGrid = EntryItemDetail;

                if (EntryItemDetail.Count == 0)
                {
                    HttpContext.Session.Remove("KeyProductionGridOnLoad");
                }
            }
            return View(MainModel);
        }
        public async Task<IActionResult> GetChildData(int WcId, int YearCode, float ProdQty, int ItemCode, string ProdDate, int BomNo)
        {
            MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                SlidingExpiration = TimeSpan.FromMinutes(55),
                Size = 1024,
            };
            var model = await _IProductionEntry.GetChildData("RMCONSUMPTION", "SpGetBomitemWithWorkcenterStock", WcId, YearCode, ProdQty, ItemCode, ProdDate, BomNo);
            string serializedGrid = JsonConvert.SerializeObject(model.ProductionChilDataDetail);
            HttpContext.Session.SetString("KeyGetChidData", serializedGrid);
            if (model.ProductionChilDataDetail != null)
            {
                model.ProductionChilDataDetail = model.ProductionChilDataDetail.ToList();
            }
            return PartialView("_ProductionChildDataDetail", model);
        }
        public IActionResult DeleteByItem(int SeqNo)
        {
            var MainModel = new ProductionEntryModel();
            string serializedGrid = HttpContext.Session.GetString("KeyGetChidData");
            List<ProductionEntryItemDetail> ProductionChilDataDetail = new();
            if (!string.IsNullOrEmpty(serializedGrid))
            {
                ProductionChilDataDetail = JsonConvert.DeserializeObject<List<ProductionEntryItemDetail>>(serializedGrid);
            }

            int Indx = Convert.ToInt32(SeqNo) - 1;

            if (ProductionChilDataDetail != null && ProductionChilDataDetail.Count > 0)
            {
                ProductionChilDataDetail.RemoveAt(Convert.ToInt32(Indx));

                Indx = 0;

                foreach (var item in ProductionChilDataDetail)
                {
                    Indx++;
                    item.SeqNo = Indx;
                }
                MainModel.ProductionChilDataDetail = ProductionChilDataDetail;

                if (ProductionChilDataDetail.Count == 0)
                {
                    HttpContext.Session.Remove("KeyGetChidData");
                }
            }
            return PartialView("_ProductionChildDataDetail", MainModel);
        }
        public async Task<JsonResult> FillReqNo(string FromDate, string ToDate, string ProdAgainst, int ItemCode)
        {

            var JSON = await _IProductionEntry.FillReqNo(FromDate, ToDate, ProdAgainst, ItemCode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillReqYear(string FromDate, string ToDate, string ProdAgainst, string ReqNo)
        {

            var JSON = await _IProductionEntry.FillReqYear(FromDate, ToDate, ProdAgainst, ReqNo);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillReqDate(string FromDate, string ToDate, string ProdAgainst, string ReqNo, int ReqYearCode, int ItemCode)
        {

            var JSON = await _IProductionEntry.FillReqDate(FromDate, ToDate, ProdAgainst, ReqNo, ReqYearCode, ItemCode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillProdDate(string FromDate, string ToDate, string ProdAgainst, string ProdSchNo, int ProdPlanYearCode)
        {

            var JSON = await _IProductionEntry.FillProdDate(FromDate, ToDate, ProdAgainst, ProdSchNo, ProdPlanYearCode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillProdSchYear(string FromDate, string ToDate, string ProdAgainst, string ProdSch)
        {

            var JSON = await _IProductionEntry.FillProdSchYear(FromDate, ToDate, ProdAgainst, ProdSch);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillReqQty(string CurrentDate, string ProdAgainst, string ReqNo, int YearCode, int ItemCode, int ReqYearCode)
        {

            var JSON = await _IProductionEntry.FillReqQty(CurrentDate, ProdAgainst, ReqNo, YearCode, ItemCode, ReqYearCode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillPendQty(string CurrentDate, string ProdAgainst, string ProdSchNo, int YearCode, int ItemCode, int ProdSchYear, int EntryId)
        {

            var JSON = await _IProductionEntry.FillPendQty(CurrentDate, ProdAgainst, ProdSchNo, YearCode, ItemCode, ProdSchYear, EntryId);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillProdSchNo(string FromDate, string ToDate, string ProdAgainst, int ItemCode)
        {

            var JSON = await _IProductionEntry.FillProdSchNo(FromDate, ToDate, ProdAgainst, ItemCode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillProdPlanDetail(string FromDate, string ToDate, string ProdAgainst, string ProdSchNo, int ProdYearCode)
        {
            var JSON = await _IProductionEntry.FillProdPlanDetail(FromDate, ToDate, ProdAgainst, ProdSchNo, ProdYearCode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetPOList(string Code, string Type, int Year, int DocTypeId)
        {
            var JSON = await _IProductionEntry.GetPoNumberDropDownList("PENDINGPOLIST", Type, "SP_GateMainDetail", Code, Year, DocTypeId);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetPOYearList(string accountCode, string yearCode, string poNo)
        {
            var JSON = await _IProductionEntry.GetScheDuleByYearCodeandAccountCode("PENDINGPOLIST", accountCode, yearCode, poNo);
            _logger.LogError(JsonConvert.SerializeObject(JSON));
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetPopUpData(int AccountCode, string PONO)
        {
            var JSON = await _IProductionEntry.GetPopUpData("POPUPDATA", AccountCode, PONO);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> CheckDuplicateEntry(int YearCode, int AccountCode, string InvNo, int DocType)
        {
            var JSON = await _IProductionEntry.CheckDuplicateEntry(YearCode, AccountCode, InvNo, DocType);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetScheDuleByYearCodeandAccountCode(string accountCode, string Year, string poNo)
        {
            var JSON = await _IProductionEntry.GetScheDuleByYearCodeandAccountCode("PURCHSCHEDULE", accountCode, Year, poNo);
            _logger.LogError(JsonConvert.SerializeObject(JSON));
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> CheckEditOrDelete(string ProdSlipNo, int ProdYearCode)
        {
            var JSON = await _IProductionEntry.CheckEditOrDelete(ProdSlipNo, ProdYearCode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<IActionResult> ProductionEntryDashboard(string FromDate = "", string ToDate = "", string Flag = "True", string SlipNo = "", string ItemName = "", string PartCode = "", string ProdPlanNo = "", string ProdSchNo = "", string ReqNo = "", string Searchbox = "", string DashboardType = "")
        {
            try
            {
                HttpContext.Session.Remove("KeyProductionEntryGrid");
                HttpContext.Session.Remove("KeyProductionEntryOperatordetail");
                HttpContext.Session.Remove("KeyProductionEntryBreakdowndetail");
                HttpContext.Session.Remove("KeyProductionEntryScrapdetail");
                HttpContext.Session.Remove("KeyProductionEntryProductdetail");
                var model = new ProductionDashboard();
  //              var Result = await _IProductionEntry.GetDashboardData().ConfigureAwait(true);
  //              DateTime now = DateTime.Now;

  //              model.FromDate = CommonFunc.ParseFormattedDate(new DateTime(now.Year, now.Month, 1).ToString());
  //              model.ToDate = CommonFunc.ParseFormattedDate(new DateTime(DateTime.Today.Year + 1, 3, 31).ToString());

  //              if (Result != null)
  //              {
  //                  var _List = new List<TextValue>();
  //                  DataSet DS = Result.Result;
  //                  if (DS != null)
  //                  {
  //                      var DT = DS.Tables[0].DefaultView.ToTable(true, "PRODEntryId", "Entrydate", "PRODYearcode",
  //  "ProdAgainstPlanManual", "NewProdRework", "ProdSlipNo", "ProdDate", "nextstoreId", "NextToStore", "NextWCID", "NextToWorkCenter", "ProdPlanNo", "ProdPlanYearCode", "ProdPlanDate", "ProdPlanSchNo"
  //, "ProdPlanSchYearCode", "ProdPlanSchDate", "Reqno", "ReqThrBOMYearcode", "ReqDate", "FGPartCode", "FGItemName",
  //"WOQTY", "ProdSchQty", "FGProdQty", "FGOKQty", "FGRejQty", "RejQtyDuetoTrail", "PendQtyForProd", "PendQtyForQC"
  //, "PendingQtyToIssue", "BOMNO", "BOMDate", "MachineName", "StageDescription", "ProdInWC", "RejQtyInWC",
  //"rejStore", "TransferFGToWCorSTORE", "QCMandatory", "TransferToQc", "StartTime", "ToTime", "setupTime", "PrevWC"
  //, "ProducedINLineNo", "QCChecked", "InitialReading", "FinalReading", "Shots", "Completed", "UtilisedHours",
  //"ProdLineNo", "stdShots", "stdCycletime", "Remark", "CyclicTime", "ProductionHour", "ItemModel", "cavity"
  //, "startupRejQty", "efficiency", "ActualTimeRequired", "BatchNo", "UniqueBatchNo", "parentProdSchNo", "parentProdSchDate"
  //, "parentProdSchYearcode", "SONO", "SOYearcode", "SODate", "sotype", "QCOffered",
  //"QCOfferDate", "QCQTy", "OKQty", "RejQTy", "StockQTy", "matTransferd", "RewQcYearCode", "RewQcDate", "shiftclose",
  //"ComplProd", "CC", "EntryByMachineNo", "ActualEntryDate", "ActualEntryByEmp", "ActualEmpByName", "LastUpdatedBy", "LastUpdationDate",
  //"EntryByDesignation", "operatorName", "supervisior", "LastUpdatedByName");
  //                      model.ProductionDashboard = CommonFunc.DataTableToList<ProductionEntryDashboard>(DT, "ProductionEntry");

  //                  }
  //                  if (Flag != "True")
  //                  {
  //                      model.FromDate1 = FromDate;
  //                      model.ToDate1 = ToDate;
  //                      model.ProdSlipNo = SlipNo;
  //                      model.ItemName = ItemName;
  //                      model.PartCode = PartCode;
  //                      model.ProdPlanNo = ProdPlanNo;
  //                      model.ProdSchNo = ProdSchNo;
  //                      model.ReqNo = ReqNo;
  //                      model.Searchbox = Searchbox;
  //                      model.DashboardType = DashboardType;
  //                      return View(model);
  //                  }
  //              }
                return View(model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IActionResult> DeleteByID(int ID, int YC, string CC, string EntryByMachineName, string EntryDate, int ActualEntryBy, string FromDate = "", string ToDate = "", string SlipNo = "", string ItemName = "", string PartCode = "", string ProdPlanNo = "", string ProdSchNo = "", string ReqNo = "", string Searchbox = "", string DashboardType = "")
        {
            var Result = await _IProductionEntry.DeleteByID(ID, YC, CC, EntryByMachineName, EntryDate, ActualEntryBy);

            if (Result.StatusText == "Success" || Result.StatusCode == HttpStatusCode.Gone)
            {
                ViewBag.isSuccess = true;
                TempData["410"] = "410";
            }
            else if (Result.StatusText == "Error" || Result.StatusCode == HttpStatusCode.Accepted)
            {
                ViewBag.isSuccess = true;
                TempData["423"] = "423";
            }
            else
            {
                ViewBag.isSuccess = false;
                TempData["500"] = "500";
            }
            DateTime fromDt = DateTime.ParseExact(FromDate, "dd/MM/yyyy", null);
            string formattedFromDate = fromDt.ToString("dd/MMM/yyyy 00:00:00");
            DateTime toDt = DateTime.ParseExact(ToDate, "dd/MM/yyyy", null);
            string formattedToDate = toDt.ToString("dd/MMM/yyyy 00:00:00");

            return RedirectToAction("ProductionEntryDashboard", new { FromDate = fromDt, ToDate = toDt, EntryDate = EntryDate, Flag = "False", CC = CC, EntryByMachineName = EntryByMachineName, ActualEntryBy = ActualEntryBy, SlipNo = SlipNo, ItemName = ItemName, PartCode = PartCode, ProdPlanNo = ProdPlanNo, ProdSchNo = ProdSchNo, ReqNo = ReqNo, DashboardType = DashboardType, Searchbox = Searchbox });
        }
        private static DataTable GetDetailTable(IList<ProductionEntryItemDetail> DetailList)
        {
            try
            {
                var GIGrid = new DataTable();

                GIGrid.Columns.Add("Entryid", typeof(int));
                GIGrid.Columns.Add("YearCode", typeof(int));
                GIGrid.Columns.Add("FGItemCode", typeof(int));
                GIGrid.Columns.Add("ConsumedRMItemCode", typeof(int));
                GIGrid.Columns.Add("ConsumedRMQTY", typeof(float));
                GIGrid.Columns.Add("ConsumedRMUnit", typeof(string));
                GIGrid.Columns.Add("MainRMitemCode", typeof(int));
                GIGrid.Columns.Add("MainRMQTY", typeof(float));
                GIGrid.Columns.Add("MainRMUnit", typeof(string));
                GIGrid.Columns.Add("FGProdQty", typeof(float));
                GIGrid.Columns.Add("FGUnit", typeof(string));
                GIGrid.Columns.Add("TotalReqRMQty", typeof(float));
                GIGrid.Columns.Add("TotalStock", typeof(float));
                GIGrid.Columns.Add("BatchStock", typeof(float));
                GIGrid.Columns.Add("WCId", typeof(int));
                GIGrid.Columns.Add("AltRMItemCode", typeof(int));
                GIGrid.Columns.Add("AltRMQty", typeof(float));
                GIGrid.Columns.Add("AltRMUnit", typeof(string));
                GIGrid.Columns.Add("RMNetWt", typeof(float));
                GIGrid.Columns.Add("GrossWt", typeof(float));
                GIGrid.Columns.Add("BatchWise", typeof(string));
                GIGrid.Columns.Add("BatchNo", typeof(string));
                GIGrid.Columns.Add("UniqueBatchNo", typeof(string));
                GIGrid.Columns.Add("BOMRevNO", typeof(int));
                GIGrid.Columns.Add("BOMRevDate", typeof(string));
                GIGrid.Columns.Add("ManualAutoEntry", typeof(string));
                GIGrid.Columns.Add("SeqNo", typeof(int));
                foreach (var Item in DetailList)
                {
                    GIGrid.Rows.Add(
                        new object[]
                        {

                    Item.EntryId == 0 ? 0 : Item.EntryId,
                    Item.YearCode== 0 ? 0 : Item.YearCode,
                    Item.FGItemCode == 0 ? 0:Item.FGItemCode,
                    Item.ConsumedRMItemCode== 0 ? 0:Item.ConsumedRMItemCode,
                    Item.IssueQty== 0 ? 0:Item.IssueQty,
                    Item.Unit==null?"":Item.Unit,
                    Item.ConsumedRMItemCode== 0 ? 0:Item.ConsumedRMItemCode,
                    Item.IssueQty== 0 ? 0:Item.IssueQty,
                    Item.Unit==null?"":Item.Unit,
                    Item.FGProdQty== 0 ? 0:Item.FGProdQty,
                    Item.Unit == null ? "" : Item.Unit,
                    Item.ReqQty== 0 ? 0:Item.ReqQty,
                    Item.TotalStock == 0 ? 0:Item.TotalStock,
                    Item.BatchStock== 0 ? 0:Item.BatchStock,
                    Item.ProdInWCID== 0 ? 0:Item.ProdInWCID,
                    Item.AltRMItemCode== 0 ? 0:Item.AltRMItemCode,
                    Item.AltRMQty==0?0.0:Item.AltRMQty,
                    Item.AltRMUnit==null?0:Item.AltRMUnit,
                    Item.RMNetWt== 0 ? 0:Item.RMNetWt,
                    Item.GrossWt== 0 ? 0:Item.GrossWt,
                    Item.BatchWise == null ? "" : Item.BatchWise,
                    Item.BatchNo == null ? "" : Item.BatchNo,
                    Item.UniqueBatchNo == null ? "" : Item.UniqueBatchNo,
                    Item.BOMRevNO == 0 ? 0 : Item.BOMRevNO ,
                    Item.BOMRevDate == null ? string.Empty : ParseFormattedDate(Item.BOMRevDate.Split(" ")[0]),
                    Item.ManualAutoEntry == null ? "" : Item.ManualAutoEntry,
                    Item.SeqNo==0?0:Item.SeqNo
                        });
                }
                GIGrid.Dispose();
                return GIGrid;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        private static DataTable GetOperatorDetailTable(IList<ProductionEntryItemDetail> OperatorDetailGrid)
        {
            var OperatorGrid = new DataTable();

            OperatorGrid.Columns.Add("Entryid", typeof(int));
            OperatorGrid.Columns.Add("Yearcode", typeof(int));
            OperatorGrid.Columns.Add("EntryDate", typeof(string));
            OperatorGrid.Columns.Add("FGItemCode", typeof(int));
            OperatorGrid.Columns.Add("FGProdQty", typeof(float));
            OperatorGrid.Columns.Add("McId", typeof(int));
            OperatorGrid.Columns.Add("WCID", typeof(int));
            OperatorGrid.Columns.Add("ProcessId", typeof(int));
            OperatorGrid.Columns.Add("OperatorId", typeof(int));
            OperatorGrid.Columns.Add("OperatorName", typeof(string));
            OperatorGrid.Columns.Add("Fromtime", typeof(string));
            OperatorGrid.Columns.Add("totime", typeof(string));
            OperatorGrid.Columns.Add("TotalHrs", typeof(float));
            OperatorGrid.Columns.Add("OverTimeHrs", typeof(float));
            OperatorGrid.Columns.Add("MachineCharges", typeof(float));
            OperatorGrid.Columns.Add("SeqNo", typeof(int));

            if (OperatorDetailGrid != null)
            {
                foreach (var Item in OperatorDetailGrid)
                {
                    OperatorGrid.Rows.Add(
                        new object[]
                        {
                    Item.EntryId == 0 ? 0 : Item.EntryId,
                    Item.YearCode== 0 ? 0 : Item.YearCode,
                    Item.EntryDate == null ? string.Empty : ParseFormattedDate(Item.EntryDate.Split(" ")[0]),
                    Item.FGItemCode== 0 ? 0:Item.FGItemCode,
                    Item.FGProdQty== 0 ? 0:Item.FGProdQty,
                    Item.McId== 0 ? 0:Item.McId,
                    Item.WCId== 0 ? 0:Item.WCId,
                    Item.ProcessId== 0 ? 0:Item.ProcessId,
                    Item.OperatorId== 0 ? 0:Item.OperatorId,
                    Item.OperatorName??"",
                    Item.Fromtime,
                    Item.Totime,
                    Item.TotalHours == 0 ? 0:Item.TotalHours,
                    Item.OverTimeHrs == 0 ? 0 : Item.OverTimeHrs ,
                    Item.MachineCharges == 0 ? 0 : Item.MachineCharges,
                    Item.SeqNo == 0 ? 0 : Item.SeqNo,
                        });
                }
                OperatorGrid.Dispose();
            }
            return OperatorGrid;
        }
        private static DataTable GetBreakdownDetailTable(IList<ProductionEntryItemDetail> BreakdownDetailList)
        {
            var BreakDownGrid = new DataTable();

            BreakDownGrid.Columns.Add("Entryid", typeof(int));
            BreakDownGrid.Columns.Add("Yearcode", typeof(int));
            BreakDownGrid.Columns.Add("Proddate", typeof(string));
            BreakDownGrid.Columns.Add("FGItemCode", typeof(int));
            BreakDownGrid.Columns.Add("WCId", typeof(int));
            BreakDownGrid.Columns.Add("BreakfromTime", typeof(string));
            BreakDownGrid.Columns.Add("BreaktoTime", typeof(string));
            BreakDownGrid.Columns.Add("ReasonId", typeof(int));
            BreakDownGrid.Columns.Add("ReasonDetail", typeof(string));
            BreakDownGrid.Columns.Add("BreakTimeMin", typeof(float));
            BreakDownGrid.Columns.Add("ResponcibleEmp", typeof(int));
            BreakDownGrid.Columns.Add("ResEmpName", typeof(string));
            BreakDownGrid.Columns.Add("ResFactor", typeof(string));
            BreakDownGrid.Columns.Add("SeqNo", typeof(int));

            if (BreakdownDetailList != null)
            {
                foreach (var Item in BreakdownDetailList)
                {
                    BreakDownGrid.Rows.Add(
                        new object[]
                        {
                    Item.EntryId == 0 ? 0 : Item.EntryId,
                    Item.YearCode== 0 ? 0 : Item.YearCode,
                    Item.ProdDate == null ? string.Empty : ParseFormattedDate(Item.ProdDate.Split(" ")[0]),
                    Item.FGItemCode== 0 ? 0:Item.FGItemCode,
                    Item.WCId== 0 ? 0:Item.WCId,
                    Item.BreakfromTime == null ? string.Empty : ParseFormattedDateTime(Item.BreakfromTime),
                    Item.BreaktoTime == null ? string.Empty : ParseFormattedDateTime(Item.BreaktoTime),
                    Item.ReasonId == 0 ? 0:Item.ReasonId,
                    Item.ReasonDetail == null ? "" : Item.ReasonDetail ,
                    Item.BreakTimeMin == 0 ? 0 : Item.BreakTimeMin,
                    Item.ResponcibleEmp == 0?0 :Item.ResponcibleEmp,
                    Item.ResEmpName == null ? "" : Item.ResEmpName,
                    Item.ResFactor == null ? "" : Item.ResFactor,
                    Item.SeqNo == 0?0 :Item.SeqNo
                        });
                }
                BreakDownGrid.Dispose();
            }
            return BreakDownGrid;
        }
        private static DataTable GetScrapDetailTable(IList<ProductionEntryItemDetail> ScrapDetailList)
        {
            var ScrapDetailGrid = new DataTable();

            ScrapDetailGrid.Columns.Add("EntryId", typeof(int));
            ScrapDetailGrid.Columns.Add("EntryDate", typeof(string));
            ScrapDetailGrid.Columns.Add("SeqNo", typeof(int));
            ScrapDetailGrid.Columns.Add("YearCode", typeof(int));
            ScrapDetailGrid.Columns.Add("FGItemCode", typeof(int));
            ScrapDetailGrid.Columns.Add("FGUnit", typeof(string));
            ScrapDetailGrid.Columns.Add("ScrapItemCode", typeof(int));
            ScrapDetailGrid.Columns.Add("FGProdQTy", typeof(float));
            ScrapDetailGrid.Columns.Add("BOMRevNo", typeof(int));
            ScrapDetailGrid.Columns.Add("BOMEFFDate", typeof(string));
            ScrapDetailGrid.Columns.Add("ScrapType", typeof(string));
            ScrapDetailGrid.Columns.Add("ScrapQty", typeof(float));
            ScrapDetailGrid.Columns.Add("Scrapunit", typeof(string));
            ScrapDetailGrid.Columns.Add("TrnasferToWCStore", typeof(string));
            ScrapDetailGrid.Columns.Add("TransferToStoreId", typeof(int));
            ScrapDetailGrid.Columns.Add("TransferToWC", typeof(int));

            if (ScrapDetailList != null)
            {
                foreach (var Item in ScrapDetailList)
                {
                    ScrapDetailGrid.Rows.Add(
                        new object[]
                    {
                    Item.EntryId == 0 ? 0 : Item.EntryId,
                    Item.EntryDate == null ? string.Empty : ParseFormattedDate(Item.EntryDate.Split(" ")[0]),
                    Item.SeqNo == null ? 0 : Item.SeqNo,
                    Item.YearCode == 0 ? 0 : Item.YearCode,
                    Item.FGItemCode== 0 ? 0:Item.FGItemCode,
                    Item.FGUnit== null ? "":Item.FGUnit,
                    Item.ScrapItemCode== 0 ? 0 : Item.ScrapItemCode,
                    Item.FGProdQty == 0 ? 0:Item.FGProdQty,
                    Item.BOMRevNO == 0 ? 0 : Item.BOMRevNO ,
                    Item.BOMRevDate == null ? string.Empty : ParseFormattedDate(Item.BOMRevDate.Split(" ")[0]),
                    Item.ScrapType == null ? "" :Item.ScrapType,
                    Item.ScrapQty == 0 ? 0 : Item.ScrapQty,
                    Item.Scrapunit == null ? "" : Item.Scrapunit,
                    Item.StoreTransferScrap == null ? "" : Item.StoreTransferScrap,
                    Item.TransferToStoreId == 0 ? 0 : Item.TransferToStoreId,
                    Item.TransferToWC == 0 ? 0 : Item.TransferToWC
                        });
                }
                ScrapDetailGrid.Dispose();
            }
            return ScrapDetailGrid;
        }
        private static DataTable GetProductDetailTable(IList<ProductionEntryItemDetail> ProductDetailList)
        {
            var ProductDetailGrid = new DataTable();

            ProductDetailGrid.Columns.Add("ProdEntryId", typeof(int));
            ProductDetailGrid.Columns.Add("EntryDate", typeof(string));
            ProductDetailGrid.Columns.Add("ProdYearcode", typeof(int));
            ProductDetailGrid.Columns.Add("FGItemCode", typeof(int));
            ProductDetailGrid.Columns.Add("FGProdQty", typeof(decimal));
            ProductDetailGrid.Columns.Add("ProcessId", typeof(int));
            ProductDetailGrid.Columns.Add("SeqNo", typeof(int));
            ProductDetailGrid.Columns.Add("ByProdItemCode", typeof(int));
            ProductDetailGrid.Columns.Add("IdealQty", typeof(decimal));
            ProductDetailGrid.Columns.Add("Qty", typeof(decimal));
            ProductDetailGrid.Columns.Add("ByProdUnit", typeof(string));
            ProductDetailGrid.Columns.Add("TrasferTOStoreWC", typeof(string));
            ProductDetailGrid.Columns.Add("StoreID", typeof(int));
            ProductDetailGrid.Columns.Add("WCId", typeof(int));

            if (ProductDetailList != null)
            {
                foreach (var Item in ProductDetailList)
                {
                    ProductDetailGrid.Rows.Add(
                        new object[]
                    {
                    Item.EntryId == 0 ? 0 : Item.EntryId,
                    Item.EntryDate == null ? string.Empty : ParseFormattedDate(Item.EntryDate.Split(" ")[0]),
                    Item.YearCode == 0 ? 0 : Item.YearCode,
                    Item.FGItemCode== 0 ? 0:Item.FGItemCode,
                    Item.FGProdQty == 0 ? 0:Item.FGProdQty,
                    Item.ProcessId == 0 ? 0 : Item.ProcessId ,
                    Item.SeqNo == 0 ? 0 : Item.SeqNo,
                    Item.ProductItemCode == 0 ? 0 : Item.ProductItemCode,
                    Item.ProductQty == 0 ? 0 : Item.ProductQty,
                    Item.ProductQty == 0 ? 0 : Item.ProductQty,
                    Item.Productunit == null ? "" : Item.Productunit, 
                    Item.StoreTransferProduct == null ? "" : Item.StoreTransferProduct,
                    Item.ProductStoreId == 0 ? 0 : Item.ProductStoreId,
                    Item.ProductToWCId == 0 ? 0 : Item.ProductToWCId
                        });
                }
                ProductDetailGrid.Dispose();
            }
            return ProductDetailGrid;
        }
        public IActionResult DeleteBreakdownItemRow(int SeqNo, string Mode)
        {
            var MainModel = new ProductionEntryModel();
            if (Mode == "U")
            {
                string serializedGrid = HttpContext.Session.GetString("KeyProductionEntryBreakdowndetail");
                List<ProductionEntryItemDetail> BreakdownDetailGrid = new();
                if (!string.IsNullOrEmpty(serializedGrid))
                {
                    BreakdownDetailGrid = JsonConvert.DeserializeObject<List<ProductionEntryItemDetail>>(serializedGrid);
                }

                int Indx = Convert.ToInt32(SeqNo) - 1;

                if (BreakdownDetailGrid != null && BreakdownDetailGrid.Count > 0)
                {
                    BreakdownDetailGrid.RemoveAt(Convert.ToInt32(Indx));

                    Indx = 0;

                    foreach (var item in BreakdownDetailGrid)
                    {
                        Indx++;
                        item.SeqNo = Indx;
                    }
                    MainModel.BreakdownDetailGrid = BreakdownDetailGrid;

                    string serializedBreakdownGrid = JsonConvert.SerializeObject(MainModel.BreakdownDetailGrid);
                    HttpContext.Session.SetString("KeyProductionEntryBreakdowndetail", serializedBreakdownGrid);
                }
            }
            else
            {
                string serializedGrid = HttpContext.Session.GetString("KeyProductionEntryBreakdowndetail");
                List<ProductionEntryItemDetail> BreakdownDetailGrid = new();
                if (!string.IsNullOrEmpty(serializedGrid))
                {
                    BreakdownDetailGrid = JsonConvert.DeserializeObject<List<ProductionEntryItemDetail>>(serializedGrid);
                }
                int Indx = Convert.ToInt32(SeqNo) - 1;

                if (BreakdownDetailGrid != null && BreakdownDetailGrid.Count > 0)
                {
                    BreakdownDetailGrid.RemoveAt(Convert.ToInt32(Indx));

                    Indx = 0;

                    foreach (var item in BreakdownDetailGrid)
                    {
                        Indx++;
                        item.SeqNo = Indx;
                    }
                    MainModel.BreakdownDetailGrid = BreakdownDetailGrid;

                    string serializedBreakdownGrid = JsonConvert.SerializeObject(MainModel.BreakdownDetailGrid);
                    HttpContext.Session.SetString("KeyProductionEntryBreakdowndetail", serializedBreakdownGrid);
                }
            }

            return PartialView("_ProductionEntryBreakdownDetail", MainModel);
        }
        public IActionResult EditBreakdownItemRow(int SeqNo, string Mode)
        {
            IList<ProductionEntryItemDetail> BreakdownDetailGrid = new List<ProductionEntryItemDetail>();
            if (Mode == "U")
            {
                string serializedGrid = HttpContext.Session.GetString("KeyProductionEntryBreakdowndetail");
                if (!string.IsNullOrEmpty(serializedGrid))
                {
                    BreakdownDetailGrid = JsonConvert.DeserializeObject<List<ProductionEntryItemDetail>>(serializedGrid);
                }
            }
            else
            {
                string serializedGrid = HttpContext.Session.GetString("KeyProductionEntryBreakdowndetail");
                if (!string.IsNullOrEmpty(serializedGrid))
                {
                    BreakdownDetailGrid = JsonConvert.DeserializeObject<List<ProductionEntryItemDetail>>(serializedGrid);
                }
            }
            IEnumerable<ProductionEntryItemDetail> SSBreakdownGrid = BreakdownDetailGrid;
            if (BreakdownDetailGrid != null)
            {
                SSBreakdownGrid = BreakdownDetailGrid.Where(x => x.SeqNo == SeqNo);
            }
            string JsonString = JsonConvert.SerializeObject(SSBreakdownGrid);
            return Json(JsonString);
        }
        public IActionResult DeleteOperatorItemRow(int SeqNo, string Mode)
        {
            var MainModel = new ProductionEntryModel();
            if (Mode == "U")
            {
                string serializedGrid = HttpContext.Session.GetString("KeyProductionEntryOperatordetail");
                List<ProductionEntryItemDetail> OperatorDetailGrid = new();
                if (!string.IsNullOrEmpty(serializedGrid))
                {
                    OperatorDetailGrid = JsonConvert.DeserializeObject<List<ProductionEntryItemDetail>>(serializedGrid);
                }
                int Indx = Convert.ToInt32(SeqNo) - 1;

                if (OperatorDetailGrid != null && OperatorDetailGrid.Count > 0)
                {
                    OperatorDetailGrid.RemoveAt(Convert.ToInt32(Indx));

                    Indx = 0;

                    foreach (var item in OperatorDetailGrid)
                    {
                        Indx++;
                        item.SeqNo = Indx;
                    }
                    MainModel.OperatorDetailGrid = OperatorDetailGrid;

                    MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
                    {
                        AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                        SlidingExpiration = TimeSpan.FromMinutes(55),
                        Size = 1024,
                    };
                    string serializedOperatorGrid = JsonConvert.SerializeObject(MainModel.OperatorDetailGrid);
                    HttpContext.Session.SetString("KeyProductionEntryOperatordetail", serializedOperatorGrid);
                }
            }
            else
            {
                string serializedGrid = HttpContext.Session.GetString("KeyProductionEntryOperatordetail");
                List<ProductionEntryItemDetail> OperatorDetailGrid = new();
                if (!string.IsNullOrEmpty(serializedGrid))
                {
                    OperatorDetailGrid = JsonConvert.DeserializeObject<List<ProductionEntryItemDetail>>(serializedGrid);
                }
                int Indx = Convert.ToInt32(SeqNo) - 1;

                if (OperatorDetailGrid != null && OperatorDetailGrid.Count > 0)
                {
                    OperatorDetailGrid.RemoveAt(Convert.ToInt32(Indx));

                    Indx = 0;

                    foreach (var item in OperatorDetailGrid)
                    {
                        Indx++;
                        item.SeqNo = Indx;
                    }
                    MainModel.OperatorDetailGrid = OperatorDetailGrid;

                    string serializedOperatorGrid = JsonConvert.SerializeObject(MainModel.OperatorDetailGrid);
                    HttpContext.Session.SetString("KeyProductionEntryOperatordetail", serializedOperatorGrid);
                }
            }

            return PartialView("_ProductionEntryOperatorDetail", MainModel);
        }
        public IActionResult EditOperatorItemRow(int SeqNo, string Mode)
        {
            IList<ProductionEntryItemDetail> OperatorDetailGrid = new List<ProductionEntryItemDetail>();
            if (Mode == "U")
            {
                string serializedGrid = HttpContext.Session.GetString("KeyProductionEntryOperatordetail");
                if (!string.IsNullOrEmpty(serializedGrid))
                {
                    OperatorDetailGrid = JsonConvert.DeserializeObject<List<ProductionEntryItemDetail>>(serializedGrid);
                }
            }
            else
            {
                string serializedGrid = HttpContext.Session.GetString("KeyProductionEntryOperatordetail");
                if (!string.IsNullOrEmpty(serializedGrid))
                {
                    OperatorDetailGrid = JsonConvert.DeserializeObject<List<ProductionEntryItemDetail>>(serializedGrid);
                }
            }
            IEnumerable<ProductionEntryItemDetail> SSOperatorGrid = OperatorDetailGrid;
            if (OperatorDetailGrid != null)
            {
                SSOperatorGrid = OperatorDetailGrid.Where(x => x.SeqNo == SeqNo);
            }
            string JsonString = JsonConvert.SerializeObject(SSOperatorGrid);
            return Json(JsonString);
        }
        public IActionResult DeleteScrapItemRow(int SeqNo, string Mode)
        {
            var MainModel = new ProductionEntryModel();
            if (Mode == "U")
            {
                string serializedGrid = HttpContext.Session.GetString("KeyProductionEntryScrapdetail");
                List<ProductionEntryItemDetail> ScrapDetailGrid = new();
                if (!string.IsNullOrEmpty(serializedGrid))
                {
                    ScrapDetailGrid = JsonConvert.DeserializeObject<List<ProductionEntryItemDetail>>(serializedGrid);
                }
                int Indx = Convert.ToInt32(SeqNo) - 1;

                if (ScrapDetailGrid != null && ScrapDetailGrid.Count > 0)
                {
                    ScrapDetailGrid.RemoveAt(Convert.ToInt32(Indx));

                    Indx = 0;

                    foreach (var item in ScrapDetailGrid)
                    {
                        Indx++;
                        item.SeqNo = Indx;
                    }
                    MainModel.ScrapDetailGrid = ScrapDetailGrid;

                    string serializedScrapGrid = JsonConvert.SerializeObject(MainModel.ScrapDetailGrid);
                    HttpContext.Session.SetString("KeyProductionEntryScrapdetail", serializedScrapGrid);
                }
            }
            else
            {
                string serializedGrid = HttpContext.Session.GetString("KeyProductionEntryScrapdetail");
                List<ProductionEntryItemDetail> ScrapDetailGrid = new();
                if (!string.IsNullOrEmpty(serializedGrid))
                {
                    ScrapDetailGrid = JsonConvert.DeserializeObject<List<ProductionEntryItemDetail>>(serializedGrid);
                }

                int Indx = Convert.ToInt32(SeqNo) - 1;

                if (ScrapDetailGrid != null && ScrapDetailGrid.Count > 0)
                {
                    ScrapDetailGrid.RemoveAt(Convert.ToInt32(Indx));

                    Indx = 0;

                    foreach (var item in ScrapDetailGrid)
                    {
                        Indx++;
                        item.SeqNo = Indx;
                    }
                    MainModel.ScrapDetailGrid = ScrapDetailGrid;

                    MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
                    {
                        AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                        SlidingExpiration = TimeSpan.FromMinutes(55),
                        Size = 1024,
                    };

                    string serializedScrapGrid = JsonConvert.SerializeObject(MainModel.ScrapDetailGrid);
                    HttpContext.Session.SetString("KeyProductionEntryScrapdetail", serializedScrapGrid);
                }
            }

            return PartialView("_ProductionEntryScrapDetail", MainModel);
        }
        public IActionResult EditScrapItemRow(int SeqNo, string Mode)
        {
            IList<ProductionEntryItemDetail> ScrapDetailGrid = new List<ProductionEntryItemDetail>();
            if (Mode == "U")
            {
                string serializedGrid = HttpContext.Session.GetString("KeyProductionEntryScrapdetail");
                if (!string.IsNullOrEmpty(serializedGrid))
                {
                    ScrapDetailGrid = JsonConvert.DeserializeObject<List<ProductionEntryItemDetail>>(serializedGrid);
                }
            }
            else
            {
                string serializedGrid = HttpContext.Session.GetString("KeyProductionEntryScrapdetail");
                if (!string.IsNullOrEmpty(serializedGrid))
                {
                    ScrapDetailGrid = JsonConvert.DeserializeObject<List<ProductionEntryItemDetail>>(serializedGrid);
                }
            }
            IEnumerable<ProductionEntryItemDetail> SSScrapGrid = ScrapDetailGrid;
            if (ScrapDetailGrid != null)
            {
                SSScrapGrid = ScrapDetailGrid.Where(x => x.SeqNo == SeqNo);
            }
            string JsonString = JsonConvert.SerializeObject(SSScrapGrid);
            return Json(JsonString);
        }
        public IActionResult DeleteProductItemRow(int SeqNo, string Mode)
        {
            var MainModel = new ProductionEntryModel();
            if (Mode == "U")
            {
                string serializedGrid = HttpContext.Session.GetString("KeyProductionEntryProductdetail");
                List<ProductionEntryItemDetail> ProductDetailGrid = new();
                if (!string.IsNullOrEmpty(serializedGrid))
                {
                    ProductDetailGrid = JsonConvert.DeserializeObject<List<ProductionEntryItemDetail>>(serializedGrid);
                }
                int Indx = Convert.ToInt32(SeqNo) - 1;

                if (ProductDetailGrid != null && ProductDetailGrid.Count > 0)
                {
                    ProductDetailGrid.RemoveAt(Convert.ToInt32(Indx));

                    Indx = 0;

                    foreach (var item in ProductDetailGrid)
                    {
                        Indx++;
                        item.SeqNo = Indx;
                    }
                    MainModel.ProductDetailGrid = ProductDetailGrid;

                    string serializedScrapGrid = JsonConvert.SerializeObject(MainModel.ProductDetailGrid);
                    HttpContext.Session.SetString("KeyProductionEntryProductdetail", serializedScrapGrid);
                }
            }
            else
            {
                string serializedGrid = HttpContext.Session.GetString("KeyProductionEntryProductdetail");
                List<ProductionEntryItemDetail> ProductDetailGrid = new();
                if (!string.IsNullOrEmpty(serializedGrid))
                {
                    ProductDetailGrid = JsonConvert.DeserializeObject<List<ProductionEntryItemDetail>>(serializedGrid);
                }

                int Indx = Convert.ToInt32(SeqNo) - 1;

                if (ProductDetailGrid != null && ProductDetailGrid.Count > 0)
                {
                    ProductDetailGrid.RemoveAt(Convert.ToInt32(Indx));

                    Indx = 0;

                    foreach (var item in ProductDetailGrid)
                    {
                        Indx++;
                        item.SeqNo = Indx;
                    }
                    MainModel.ProductDetailGrid = ProductDetailGrid;

                    MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
                    {
                        AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                        SlidingExpiration = TimeSpan.FromMinutes(55),
                        Size = 1024,
                    };

                    string serializedScrapGrid = JsonConvert.SerializeObject(MainModel.ProductDetailGrid);
                    HttpContext.Session.SetString("KeyProductionEntryProductdetail", serializedScrapGrid);
                }
            }

            return PartialView("_ProductionEntryProductDetail", MainModel);
        }
        public IActionResult EditProductItemRow(int SeqNo, string Mode)
        {
            IList<ProductionEntryItemDetail> ProductDetailGrid = new List<ProductionEntryItemDetail>();
            if (Mode == "U")
            {
                string serializedGrid = HttpContext.Session.GetString("KeyProductionEntryProductdetail");
                if (!string.IsNullOrEmpty(serializedGrid))
                {
                    ProductDetailGrid = JsonConvert.DeserializeObject<List<ProductionEntryItemDetail>>(serializedGrid);
                }
            }
            else
            {
                string serializedGrid = HttpContext.Session.GetString("KeyProductionEntryProductdetail");
                if (!string.IsNullOrEmpty(serializedGrid))
                {
                    ProductDetailGrid = JsonConvert.DeserializeObject<List<ProductionEntryItemDetail>>(serializedGrid);
                }
            }
            IEnumerable<ProductionEntryItemDetail> SSScrapGrid = ProductDetailGrid;
            if (ProductDetailGrid != null)
            {
                SSScrapGrid = ProductDetailGrid.Where(x => x.SeqNo == SeqNo);
            }
            string JsonString = JsonConvert.SerializeObject(SSScrapGrid);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetTimeDiff(string Flag, string ToTime, string DiffType, string FromTime)
        {
            var JSON = await _IProductionEntry.GetTimeDiff(Flag, ToTime, DiffType, FromTime);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetDateforBreakdown(string Flag, string DiffType, string QtyOfTime, string FromTime)
        {
            var JSON = await _IProductionEntry.GetDateforBreakdown(Flag, DiffType, QtyOfTime, FromTime);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
    }
}
