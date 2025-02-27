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

namespace eTactWeb.Controllers
{

    [Authorize]
    public class ProductionEntryController : Controller
    {
        private readonly IDataLogic _IDataLogic;
        //private readonly IGateInward _IGateInward;
        public IProductionEntry _IProductionEntry { get; }
        private readonly ILogger<ProductionEntryController> _logger;
        private readonly IConfiguration iconfiguration;
        private readonly IMemoryCache _MemoryCache;
        public IWebHostEnvironment _IWebHostEnvironment { get; }
        public ProductionEntryController(ILogger<ProductionEntryController> logger, IDataLogic iDataLogic, IProductionEntry iProductionEntry, IMemoryCache iMemoryCache, EncryptDecrypt encryptDecrypt, IWebHostEnvironment iWebHostEnvironment, IConfiguration iconfiguration)
        {
            _logger = logger;
            _IDataLogic = iDataLogic;
            _IProductionEntry = iProductionEntry;
            _MemoryCache = iMemoryCache;
            _IWebHostEnvironment = iWebHostEnvironment;
            this.iconfiguration = iconfiguration;
        }
        public IActionResult PrintReport(int EntryId = 0, int YearCode = 0)
        {
            string my_connection_string;
            string contentRootPath = _IWebHostEnvironment.ContentRootPath;
            string webRootPath = _IWebHostEnvironment.WebRootPath;
            //string frx = Path.Combine(_env.ContentRootPath, "reports", value.file);
            var webReport = new WebReport();

            webReport.Report.Load(webRootPath + "\\ProductionEntryPrint.frx"); // default report


            //webReport.Report.SetParameterValue("flagparam", "PURCHASEORDERPRINT");
            webReport.Report.SetParameterValue("entryid", EntryId);
            webReport.Report.SetParameterValue("yearcode", YearCode);


            my_connection_string = iconfiguration.GetConnectionString("eTactDB");
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


            return View(MainModel); // Pass the model with old data to the view
        }
        public async Task<IActionResult> GetPendingProductionEntry(int Yearcode)
        {
            var model = new PendingProductionEntryModel();
            model = await _IProductionEntry.GetPendingProductionEntry(Yearcode);
            return PartialView("_PendingProductionEntry", model);
        }

        public async Task<IActionResult> GetDataForProductionEntry(PendingProductionEntryModel ItemData)
        {
            _MemoryCache.Remove("KeyProductionEntryDataGrid");
            MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                SlidingExpiration = TimeSpan.FromMinutes(55),
                Size = 1024
            };

            _MemoryCache.Set("KeyProductionEntryDataGrid", ItemData, cacheEntryOptions);
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
        //1   -129

        [Route("{controller}/Index")]
        public async Task<IActionResult> ProductionEntry()
        {
            ViewData["Title"] = "Production Entry Details";
            TempData.Clear();
            _MemoryCache.Remove("KeyProductionEntryGrid");
            _MemoryCache.Remove("KeyProductionEntryOperatordetail");
            _MemoryCache.Remove("KeyProductionEntryBreakdowndetail");
            _MemoryCache.Remove("KeyProductionEntryScrapdetail");
            var model = await BindModels(null);
            model.FinFromDate = HttpContext.Session.GetString("FromDate");
            model.FinToDate = HttpContext.Session.GetString("ToDate");
            model.YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
            model.CC = HttpContext.Session.GetString("Branch");
            model.PreparedByEmp = HttpContext.Session.GetString("EmpName");
            model.ActualEnteredByName = HttpContext.Session.GetString("EmpName");
            model.CreatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));

            MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                SlidingExpiration = TimeSpan.FromMinutes(55),
                Size = 1024
            };

            _MemoryCache.Set("KeyProductionEntryGrid", model, cacheEntryOptions);
            return View(model);
        }
        [Route("{controller}/Index")]
        [HttpGet]
        public async Task<ActionResult> ProductionEntry(int ID, string Mode, int YC, string FromDate = "", string ToDate = "", string SlipNo = "", string ItemName = "", string PartCode = "", string ProdPlanNo = "", string ProdSchNo = "", string ReqNo = "", string Searchbox = "", string DashboardType = "")//, ILogger logger)
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
            _MemoryCache.Remove("KeyProductionEntryGrid");
            _MemoryCache.Remove("KeyProductionEntryBreakdowndetail");
            _MemoryCache.Remove("KeyProductionEntryOperatordetail");
            _MemoryCache.Remove("KeyProductionEntryScrapdetail");

            if (!string.IsNullOrEmpty(Mode) && ID > 0 && (Mode == "V" || Mode == "U"))
            {
                MainModel = await _IProductionEntry.GetViewByID(ID, YC).ConfigureAwait(false);
                MainModel.Mode = Mode;
                MainModel.ID = ID;
                MainModel.YearCode=YC;
                MainModel = await BindModels(MainModel).ConfigureAwait(false);
                MainModel.FinFromDate = HttpContext.Session.GetString("FromDate");
                MainModel.FinToDate = HttpContext.Session.GetString("ToDate");
                MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                    SlidingExpiration = TimeSpan.FromMinutes(55),
                    Size = 1024,
                };
                _MemoryCache.Set("KeyProductionEntryGrid", MainModel.ItemDetailGrid, cacheEntryOptions);
                _MemoryCache.Set("KeyProductionEntryBreakdowndetail", MainModel.BreakdownDetailGrid, cacheEntryOptions);
                _MemoryCache.Set("KeyProductionEntryOperatordetail", MainModel.OperatorDetailGrid, cacheEntryOptions);
                _MemoryCache.Set("KeyProductionEntryScrapdetail", MainModel.ScrapDetailGrid, cacheEntryOptions);
            }
            else
            {
                MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                    SlidingExpiration = TimeSpan.FromMinutes(55),
                    Size = 1024,
                };
                _MemoryCache.TryGetValue("KeyProductionEntryDataGrid", out PendingProductionEntryModel PendingProductionEntryModel);
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
                }
                //_MemoryCache.Set("KeyMIRGrid", MainModel1, cacheEntryOptions);
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
        //2-- save code
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

                _MemoryCache.TryGetValue("KeyProductionEntryGrid", out List<ProductionEntryItemDetail> ProductionEntryItemDetail);
                _MemoryCache.TryGetValue("KeyProductionEntryBreakdowndetail", out List<ProductionEntryItemDetail> ProductionEntryBreakdownDetail);
                _MemoryCache.TryGetValue("KeyProductionEntryOperatordetail", out List<ProductionEntryItemDetail> ProductionEntryOperatorDetail);
                _MemoryCache.TryGetValue("KeyProductionEntryScrapdetail", out List<ProductionEntryItemDetail> ProductionEntryScrapDetail);


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
                    }
                    else
                    {
                        GIGrid = GetDetailTable(ProductionEntryItemDetail);
                        BreakDownGrid = GetBreakdownDetailTable(ProductionEntryBreakdownDetail);
                        OperatorGrid = GetOperatorDetailTable(ProductionEntryOperatorDetail);
                        ScrapGrid = GetScrapDetailTable(ProductionEntryScrapDetail);
                    }

                    var Result = await _IProductionEntry.SaveProductionEntry(model, GIGrid, BreakDownGrid, OperatorGrid, ScrapGrid);

                    if (Result != null)
                    {
                        if (Result.StatusText == "Success" && Result.StatusCode == HttpStatusCode.OK)
                        {
                            ViewBag.isSuccess = true;
                            TempData["200"] = "200";
                            _MemoryCache.Remove(GIGrid);
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
        public async Task<JsonResult> GetFormRights()
        {
            var userID = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
            var JSON = await _IProductionEntry.GetFormRights(userID);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> ClearGridAjax()
        {
            _MemoryCache.Remove("KeyProductionEntryGrid");
            _MemoryCache.Remove("KeyProductionEntryBreakdowndetail");
            _MemoryCache.Remove("KeyProductionEntryOperatordetail");
            _MemoryCache.Remove("KeyProductionEntryScrapdetail");
            return Json("done");
        }
        public async Task<IActionResult> GetSearchData(string FromDate, string ToDate, string SlipNo, string ItemName, string PartCode, string ProdPlanNo, string ProdSchNo, string ReqNo, string DashboardType)
        {
            //model.Mode = "Search";
            var model = new ProductionEntryDashboard();
            model = await _IProductionEntry.GetDashboardData(FromDate, ToDate, SlipNo, ItemName, PartCode, ProdPlanNo, ProdSchNo, ReqNo, DashboardType);
            model.DashboardType = "Summary";
            return PartialView("_ProductionEntryDashboardGrid", model);
        }
        public async Task<IActionResult> GetDetailData(string FromDate, string ToDate, string SlipNo, string ItemName, string PartCode, string ProdPlanNo, string ProdSchNo, string ReqNo, string DashboardType)
        {
            //model.Mode = "Search";
            var model = new ProductionEntryDashboard();
            model = await _IProductionEntry.GetDashboardDetailData(FromDate, ToDate, SlipNo, ItemName, PartCode, ProdPlanNo, ProdSchNo, ReqNo, DashboardType);
            model.DashboardType = "Detail";
            return PartialView("_ProductionEntryDashboardGrid", model);
        }
        public async Task<IActionResult> GetBatchwiseDetail(string FromDate, string ToDate, string SlipNo, string ItemName, string PartCode, string ProdPlanNo, string ProdSchNo, string ReqNo, string DashboardType)
        {
            //model.Mode = "Search";
            var model = new ProductionEntryDashboard();
            model = await _IProductionEntry.GetBatchwiseDetail(FromDate, ToDate, SlipNo, ItemName, PartCode, ProdPlanNo, ProdSchNo, ReqNo, DashboardType);
            model.DashboardType = "DetailWithBatchwise";
            return PartialView("_ProductionEntryDashboardGrid", model);
        }
        public async Task<IActionResult> GetBreakdownData(string FromDate, string ToDate)
        {
            //model.Mode = "Search";
            var model = new ProductionEntryDashboard();
            model = await _IProductionEntry.GetBreakdownData(FromDate, ToDate);
            model.DashboardType = "Breakdown";
            return PartialView("_ProductionEntryDashboardGrid", model);
        }
        public async Task<IActionResult> GetOperationData(string FromDate, string ToDate)
        {
            //model.Mode = "Search";
            var model = new ProductionEntryDashboard();
            model = await _IProductionEntry.GetOperationData(FromDate, ToDate);
            model.DashboardType = "Operator";
            return PartialView("_ProductionEntryDashboardGrid", model);
        }
        public async Task<IActionResult> GetScrapData(string FromDate, string ToDate)
        {
            //model.Mode = "Search";
            var model = new ProductionEntryDashboard();
            model = await _IProductionEntry.GetScrapData(FromDate, ToDate);
            model.DashboardType = "Scrap";
            return PartialView("_ProductionEntryDashboardGrid", model);
        }
        public async Task<ProductionEntryModel> BindModels(ProductionEntryModel model)
        {
            if (model == null)
            {
                model = new ProductionEntryModel();

                model.YearCode = Constants.FinincialYear;
                model.EntryId = _IDataLogic.GetEntryID("SP_ProductionEntry", Constants.FinincialYear, "PRODEntryId", "PRODYearcode");
                //model.Date = DateTime.Today;
                model.EntryTime = DateTime.Now.ToString("hh:mm tt");

            }
            //model.ProcessList = await _IDataLogic.GetDropDownList("ProcessList", "SP_GetDropDownList");
            //model.pro = await _IDataLogic.GetDropDownList("ProcessList", "SP_GetDropDownList");
            //model.PONO = await _IDataLogic.GetDropDownList("PENDINGPOLIST","I", "SP_GateMainDetail");


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

            // Render profile page with username
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
                DateTime time = DateTime.Now;
                string format = "MMM ddd d HH:mm yyyy";
                string formattedDate = time.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);

                var dt = time.ToString(format);
                return Json(formattedDate);
                //string apiUrl = "https://worldtimeapi.org/api/ip";

                //using (HttpClient client = new HttpClient())
                //{
                //    HttpResponseMessage response = await client.GetAsync(apiUrl);

                //    if (response.IsSuccessStatusCode)
                //    {
                //        string content = await response.Content.ReadAsStringAsync();
                //        JObject jsonObj = JObject.Parse(content);

                //        string datetimestring = (string)jsonObj["datetime"];
                //        var formattedDateTime = datetimestring.Split(" ")[0];

                //        DateTime parsedDate = DateTime.ParseExact(formattedDateTime, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                //        string formattedDate = parsedDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);

                //        return Json(formattedDate);
                //    }
                //    else
                //    {
                //        string errorContent = await response.Content.ReadAsStringAsync();
                //        throw new HttpRequestException($"Failed to fetch server date and time. Status Code: {response.StatusCode}. Content: {errorContent}");
                //    }
                //}
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
            _MemoryCache.TryGetValue("KeyProductionEntryGrid", out IList<ProductionEntryItemDetail> ProductionEntryItemDetail);
            var SSGrid = ProductionEntryItemDetail.Where(x => x.SeqNo == SeqNo);
            string JsonString = JsonConvert.SerializeObject(SSGrid);
            return Json(JsonString);
        }
        public IActionResult DeleteItemRow(int SeqNo, string ProdType)
        {
            var MainModel = new ProductionEntryModel();
            _MemoryCache.TryGetValue("KeyProductionEntryGrid", out List<ProductionEntryItemDetail> ProductionEntryItemDetail);
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

                MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                    SlidingExpiration = TimeSpan.FromMinutes(55),
                    Size = 1024,
                };

                if (ProductionEntryItemDetail.Count == 0)
                {
                    _MemoryCache.Remove("KeyProductionEntryGrid");
                }
                //_MemoryCache.Set("KeyMaterialReceiptGrid", MainModel.ItemDetailGrid, cacheEntryOptions);
            }
            MainModel.ProdType=ProdType;
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
                MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                    SlidingExpiration = TimeSpan.FromMinutes(55),
                    Size = 1024,
                };
                var seqNo = 0;
                foreach (var item in model)
                {
                    _MemoryCache.TryGetValue("KeyProductionEntryGrid", out IList<ProductionEntryItemDetail> ProductionEntryItemDetail);
                    if (item != null)
                    {
                        if (ProductionEntryItemDetail == null)
                        {
                            item.SeqNo=seqNo;
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
                        MainModel.ProdType=ProdType;
                        var ProdEntryAllow = item.ProdEntryAllowToAddRMItem;
                        MainModel.ProdEntryAllowToAddRMItem= ProdEntryAllow;
                        _MemoryCache.Set("KeyProductionEntryGrid", MainModel.ItemDetailGrid, cacheEntryOptions);
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
                _MemoryCache.TryGetValue("KeyProductionEntryGrid", out IList<ProductionEntryItemDetail> ProductionEntryItemDetail);

                var MainModel = new ProductionEntryModel();
                var ProductionEntryGrid = new List<ProductionEntryItemDetail>();
                var ProductionGrid = new List<ProductionEntryItemDetail>();
                var SSGrid = new List<ProductionEntryItemDetail>();
                MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                    SlidingExpiration = TimeSpan.FromMinutes(55),
                    Size = 1024,
                };
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
                    MainModel.ProdType=ProdType;
                    var ProdEntryAllow = modelData.ProdEntryAllowToAddRMItem;
                    MainModel.ProdEntryAllowToAddRMItem= ProdEntryAllow;
                    _MemoryCache.Set("KeyProductionEntryGrid", MainModel.ItemDetailGrid, cacheEntryOptions);
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
                _MemoryCache.TryGetValue("KeyProductionEntryOperatordetail", out IList<ProductionEntryItemDetail> ProductionEntryOperatorDetail);

                var MainModel = new ProductionEntryModel();
                var ProductionEntryGrid = new List<ProductionEntryItemDetail>();
                var ProductionGrid = new List<ProductionEntryItemDetail>();
                var SSGrid = new List<ProductionEntryItemDetail>();
                MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                    SlidingExpiration = TimeSpan.FromMinutes(55),
                    Size = 1024,
                };
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

                    _MemoryCache.Set("KeyProductionEntryOperatordetail", MainModel.OperatorDetailGrid, cacheEntryOptions);
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
                _MemoryCache.TryGetValue("KeyProductionEntryBreakdowndetail", out IList<ProductionEntryItemDetail> ProductionEntryBreakdownDetail);

                var MainModel = new ProductionEntryModel();
                var ProductionEntryGrid = new List<ProductionEntryItemDetail>();
                var ProductionGrid = new List<ProductionEntryItemDetail>();
                var SSGrid = new List<ProductionEntryItemDetail>();
                MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                    SlidingExpiration = TimeSpan.FromMinutes(55),
                    Size = 1024,
                };
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

                    _MemoryCache.Set("KeyProductionEntryBreakdowndetail", MainModel.BreakdownDetailGrid, cacheEntryOptions);
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
                _MemoryCache.TryGetValue("KeyProductionEntryScrapdetail", out IList<ProductionEntryItemDetail> ProductionEntryScrapDetail);

                var MainModel = new ProductionEntryModel();
                var ProductionEntryGrid = new List<ProductionEntryItemDetail>();
                var ProductionGrid = new List<ProductionEntryItemDetail>();
                var SSGrid = new List<ProductionEntryItemDetail>();
                MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                    SlidingExpiration = TimeSpan.FromMinutes(55),
                    Size = 1024,
                };
                if (model != null)
                {
                    if (ProductionEntryScrapDetail == null)
                    {
                        model.SeqNo =  1;
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

                    _MemoryCache.Set("KeyProductionEntryScrapdetail", MainModel.ScrapDetailGrid, cacheEntryOptions);
                }

                return PartialView("_ProductionEntryScrapDetail", MainModel);
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
        public async Task<JsonResult> FillMachineGroup()
        {
            var JSON = await _IProductionEntry.FillMachineGroup();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillMachineName()
        {
            var JSON = await _IProductionEntry.FillMachineName();
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
        public async Task<JsonResult> GetItems(string ProdAgainst, int YearCode)
        {
            var JSON = await _IProductionEntry.GetItems(ProdAgainst, YearCode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetPartCode(string ProdAgainst, int YearCode)
        {
            var JSON = await _IProductionEntry.GetPartCode(ProdAgainst, YearCode);
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
            _MemoryCache.TryGetValue("KeyProductionGridOnLoad", out List<ProductionEntryItemDetail> EntryItemDetail);
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

                MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                    SlidingExpiration = TimeSpan.FromMinutes(55),
                    Size = 1024,
                };


                if (EntryItemDetail.Count == 0)
                {
                    _MemoryCache.Remove("KeyProductionGridOnLoad");
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
            _MemoryCache.Set("KeyGetChidData", model.ProductionChilDataDetail, cacheEntryOptions);
            if (model.ProductionChilDataDetail != null)
            {
                model.ProductionChilDataDetail = model.ProductionChilDataDetail.ToList();
            }
            return PartialView("_ProductionChildDataDetail", model);
        }
        public IActionResult DeleteByItem(int SeqNo)
        {
            var MainModel = new ProductionEntryModel();
            _MemoryCache.TryGetValue("KeyGetChidData", out List<ProductionEntryItemDetail> ProductionChilDataDetail);
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

                MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                    SlidingExpiration = TimeSpan.FromMinutes(55),
                    Size = 1024,
                };


                if (ProductionChilDataDetail.Count == 0)
                {
                    _MemoryCache.Remove("KeyGetChidData");
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
        //public async Task<JsonResult> GetPopUpData(int AccountCode, int IssYear, string FinYearFromDate, string billchallandate, string prodUnProd, string BOMINd, int RMItemCode, string RMPartcode, string RMItemNAme, string ACCOUNTNAME, int Processid)
        //{
        //    var JSON = await _IProductionEntry.GetPopUpData("JOBWORKISSUESUMMARY", AccountCode, IssYear, FinYearFromDate, billchallandate, prodUnProd, BOMINd, RMItemCode, RMPartcode, RMItemNAme, ACCOUNTNAME, Processid);
        //    string JsonString = JsonConvert.SerializeObject(JSON);
        //    return Json(JsonString);
        //}
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
                _MemoryCache.Remove("KeyProductionEntryGrid");
                _MemoryCache.Remove("KeyProductionEntryOperatordetail");
                _MemoryCache.Remove("KeyProductionEntryBreakdowndetail");
                _MemoryCache.Remove("KeyProductionEntryScrapdetail");

                var model = new ProductionDashboard();
                var Result = await _IProductionEntry.GetDashboardData().ConfigureAwait(true);
                DateTime now = DateTime.Now;

                model.FromDate = new DateTime(now.Year, now.Month, 1).ToString("dd/MM/yyyy").Replace("-", "/");
                model.ToDate = new DateTime(DateTime.Today.Year + 1, 3, 31).ToString("dd/MM/yyyy").Replace("-", "/");

                if (Result != null)
                {
                    var _List = new List<TextValue>();
                    DataSet DS = Result.Result;
                    if (DS != null)
                    {
                        var DT = DS.Tables[0].DefaultView.ToTable(true, "PRODEntryId", "Entrydate", "PRODYearcode",
    "ProdAgainstPlanManual", "NewProdRework", "ProdSlipNo", "ProdDate", "nextstoreId", "NextToStore", "NextWCID", "NextToWorkCenter", "ProdPlanNo", "ProdPlanYearCode", "ProdPlanDate", "ProdPlanSchNo"
  , "ProdPlanSchYearCode", "ProdPlanSchDate", "Reqno", "ReqThrBOMYearcode", "ReqDate", "FGPartCode", "FGItemName",
  "WOQTY", "ProdSchQty", "FGProdQty", "FGOKQty", "FGRejQty", "RejQtyDuetoTrail", "PendQtyForProd", "PendQtyForQC"
  , "PendingQtyToIssue", "BOMNO", "BOMDate", "MachineName", "StageDescription", "ProdInWC", "RejQtyInWC",
  "rejStore", "TransferFGToWCorSTORE", "QCMandatory", "TransferToQc", "StartTime", "ToTime", "setupTime", "PrevWC"
  , "ProducedINLineNo", "QCChecked", "InitialReading", "FinalReading", "Shots", "Completed", "UtilisedHours",
  "ProdLineNo", "stdShots", "stdCycletime", "Remark", "CyclicTime", "ProductionHour", "ItemModel", "cavity"
  , "startupRejQty", "efficiency", "ActualTimeRequired", "BatchNo", "UniqueBatchNo", "parentProdSchNo", "parentProdSchDate"
  , "parentProdSchYearcode", "SONO", "SOYearcode", "SODate", "sotype", "QCOffered",
  "QCOfferDate", "QCQTy", "OKQty", "RejQTy", "StockQTy", "matTransferd", "RewQcYearCode", "RewQcDate", "shiftclose",
  "ComplProd", "CC", "EntryByMachineNo", "ActualEntryDate", "ActualEntryByEmp", "ActualEmpByName", "LastUpdatedBy", "LastUpdationDate",
  "EntryByDesignation", "operatorName", "supervisior", "LastUpdatedByName");
                        model.ProductionDashboard = CommonFunc.DataTableToList<ProductionEntryDashboard>(DT, "ProductionEntry");

                    }
                    if (Flag != "True")
                    {
                        model.FromDate1 = FromDate;
                        model.ToDate1 = ToDate;
                        model.ProdSlipNo=SlipNo;
                        model.ItemName=ItemName;
                        model.PartCode=PartCode;
                        model.ProdPlanNo=ProdPlanNo;
                        model.ProdSchNo=ProdSchNo;
                        model.ReqNo=ReqNo;
                        model.Searchbox=Searchbox;
                        model.DashboardType=DashboardType;
                        return View(model);
                    }
                }
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
        private static DataTable GetOperatorDetailTable(IList<ProductionEntryItemDetail> OperatorDetailList)
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

            if (OperatorDetailList != null)
            {
                foreach (var Item in OperatorDetailList)
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
                    Item.TransferToWCStore == null ? "" : Item.TransferToWCStore,
                    Item.TransferToStoreId == 0 ? 0 : Item.TransferToStoreId,
                    Item.TransferToWC == 0 ? 0 : Item.TransferToWC
                        });
                }
                ScrapDetailGrid.Dispose();
            }
            return ScrapDetailGrid;
        }
        public IActionResult DeleteBreakdownItemRow(int SeqNo, string Mode)
        {
            var MainModel = new ProductionEntryModel();
            if (Mode == "U")
            {
                _MemoryCache.TryGetValue("KeyProductionEntryBreakdowndetail", out List<ProductionEntryItemDetail> BreakdownDetailGrid);
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

                    MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
                    {
                        AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                        SlidingExpiration = TimeSpan.FromMinutes(55),
                        Size = 1024,
                    };

                    _MemoryCache.Set("KeyProductionEntryBreakdowndetail", MainModel.BreakdownDetailGrid, cacheEntryOptions);
                }
            }
            else
            {
                _MemoryCache.TryGetValue("KeyProductionEntryBreakdowndetail", out List<ProductionEntryItemDetail> BreakdownDetailGrid);
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

                    MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
                    {
                        AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                        SlidingExpiration = TimeSpan.FromMinutes(55),
                        Size = 1024,
                    };

                    _MemoryCache.Set("KeyProductionEntryBreakdowndetail", MainModel.BreakdownDetailGrid, cacheEntryOptions);
                }
            }

            return PartialView("_ProductionEntryBreakdownDetail", MainModel);
        }
        public IActionResult EditBreakdownItemRow(int SeqNo, string Mode)
        {
            IList<ProductionEntryItemDetail> BreakdownDetailGrid = new List<ProductionEntryItemDetail>();
            if (Mode == "U")
            {
                _MemoryCache.TryGetValue("KeyProductionEntryBreakdowndetail", out BreakdownDetailGrid);
            }
            else
            {
                _MemoryCache.TryGetValue("KeyProductionEntryBreakdowndetail", out BreakdownDetailGrid);
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
                _MemoryCache.TryGetValue("KeyProductionEntryOperatordetail", out List<ProductionEntryItemDetail> OperatorDetailGrid);
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

                    _MemoryCache.Set("KeyProductionEntryOperatordetail", MainModel.OperatorDetailGrid, cacheEntryOptions);
                }
            }
            else
            {
                _MemoryCache.TryGetValue("KeyProductionEntryOperatordetail", out List<ProductionEntryItemDetail> OperatorDetailGrid);
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

                    _MemoryCache.Set("KeyProductionEntryOperatordetail", MainModel.OperatorDetailGrid, cacheEntryOptions);
                }
            }

            return PartialView("_ProductionEntryOperatorDetail", MainModel);
        }
        public IActionResult EditOperatorItemRow(int SeqNo, string Mode)
        {
            IList<ProductionEntryItemDetail> OperatorDetailGrid = new List<ProductionEntryItemDetail>();
            if (Mode == "U")
            {
                _MemoryCache.TryGetValue("KeyProductionEntryOperatordetail", out OperatorDetailGrid);
            }
            else
            {
                _MemoryCache.TryGetValue("KeyProductionEntryOperatordetail", out OperatorDetailGrid);
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
                _MemoryCache.TryGetValue("KeyProductionEntryScrapdetail", out List<ProductionEntryItemDetail> ScrapDetailGrid);
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

                    _MemoryCache.Set("KeyProductionEntryScrapdetail", MainModel.ScrapDetailGrid, cacheEntryOptions);
                }
            }
            else
            {
                _MemoryCache.TryGetValue("KeyProductionEntryScrapdetail", out List<ProductionEntryItemDetail> ScrapDetailGrid);
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

                    _MemoryCache.Set("KeyProductionEntryScrapdetail", MainModel.ScrapDetailGrid, cacheEntryOptions);
                }
            }

            return PartialView("_ProductionEntryScrapDetail", MainModel);
        }
        public IActionResult EditScrapItemRow(int SeqNo, string Mode)
        {
            IList<ProductionEntryItemDetail> ScrapDetailGrid = new List<ProductionEntryItemDetail>();
            if (Mode == "U")
            {
                _MemoryCache.TryGetValue("KeyProductionEntryScrapdetail", out ScrapDetailGrid);
            }
            else
            {
                _MemoryCache.TryGetValue("KeyProductionEntryScrapdetail", out ScrapDetailGrid);
            }
            IEnumerable<ProductionEntryItemDetail> SSScrapGrid = ScrapDetailGrid;
            if (ScrapDetailGrid != null)
            {
                SSScrapGrid = ScrapDetailGrid.Where(x => x.SeqNo == SeqNo);
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
