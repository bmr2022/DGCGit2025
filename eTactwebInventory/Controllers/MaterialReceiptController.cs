using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using FastReport.Export.Html;
using FastReport.Export.Image;
using FastReport.Web;
using FastReport;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using static eTactWeb.Data.Common.CommonFunc;
using static eTactWeb.DOM.Models.Common;
using static eTactWeb.DOM.Models.MaterialReceiptModel;
using static Grpc.Core.Metadata;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Data;
using System.Globalization;


namespace eTactWeb.Controllers
{
    public class MaterialReceiptController : Controller
    {
        private readonly IDataLogic _IDataLogic;
        private readonly IMaterialReceipt _IMaterialReceipt;
        private readonly ILogger<MaterialReceiptController> _logger;
        private readonly IMemoryCache _MemoryCache;
        private readonly IConfiguration iconfiguration;

        public IWebHostEnvironment _IWebHostEnvironment { get; }
        public MaterialReceiptController(ILogger<MaterialReceiptController> logger, IDataLogic iDataLogic, IMaterialReceipt iMaterialReceipt, IMemoryCache iMemoryCache, IWebHostEnvironment iWebHostEnvironment, IConfiguration iconfiguration)
        {
            _logger = logger;
            _IDataLogic = iDataLogic;
            _IMaterialReceipt = iMaterialReceipt;
            _MemoryCache = iMemoryCache;
            _IWebHostEnvironment = iWebHostEnvironment;
            this.iconfiguration = iconfiguration;
        }

        public IActionResult PrintReport(int EntryId = 0, int YearCode = 0, string MrnNo = "")
        {
            string my_connection_string;
            string contentRootPath = _IWebHostEnvironment.ContentRootPath;
            string webRootPath = _IWebHostEnvironment.WebRootPath;
            //string frx = Path.Combine(_env.ContentRootPath, "reports", value.file);
            var webReport = new WebReport();

            webReport.Report.Load(webRootPath + "\\MRN.frx"); // default report


            //webReport.Report.SetParameterValue("flagparam", "PURCHASEORDERPRINT");
            webReport.Report.SetParameterValue("MrnEntryparam", EntryId);
            webReport.Report.SetParameterValue("MrnYearcodeparam", YearCode);


            my_connection_string = iconfiguration.GetConnectionString("eTactDB");
            //my_connection_string = "Data Source=192.168.1.224\\sqlexpress;Initial  Catalog = etactweb; Integrated Security = False; Persist Security Info = False; User
            //         ID = web; Password = bmr2401";
            webReport.Report.SetParameterValue("MyParameter", my_connection_string);


            // webReport.Report.SetParameterValue("accountparam", 1731);


            // webReport.Report.Dictionary.Connections[0].ConnectionString = @"Data Source=103.10.234.95;AttachDbFilename=;Initial Catalog=eTactWeb;Integrated Security=False;Persist Security Info=True;User ID=web;Password=bmr2401";
            //ViewBag.WebReport = webReport;
            return View(webReport);
        }
        public ActionResult HtmlSave(int EntryId = 0, int YearCode = 0, string MrnNo = "")
        {
            using (Report report = new Report())
            {
                string webRootPath = _IWebHostEnvironment.WebRootPath;
                var webReport = new WebReport();


                webReport.Report.Load(webRootPath + "\\MRNPrint.frx");
                //webReport.Report.SetParameterValue("flagparam", "PURCHASEORDERPRINT");
                webReport.Report.SetParameterValue("MrnEntry", EntryId);
                webReport.Report.SetParameterValue("MrnYearcode", YearCode);
                webReport.Report.SetParameterValue("MrnNo", MrnNo);
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

        public IActionResult GetImage(int EntryId = 0, int YearCode = 0, string MrnNo = "")
        {
            // Creatint the Report object
            using (Report report = new Report())
            {
                string webRootPath = _IWebHostEnvironment.WebRootPath;
                var webReport = new WebReport();


                webReport.Report.Load(webRootPath + "\\MRNPrint.frx");
                //webReport.Report.SetParameterValue("flagparam", "PURCHASEORDERPRINT");
                webReport.Report.SetParameterValue("MrnEntry", EntryId);
                webReport.Report.SetParameterValue("MrnYearcode", YearCode);
                webReport.Report.SetParameterValue("MrnNo", MrnNo);
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
      //  [Route("{controller}/Index")]
        //public async Task<IActionResult> MaterialReceipt()
        //{
        //    ViewData["Title"] = "Inventory Details";
        //    TempData.Clear();
        //    _MemoryCache.Remove("KeyMaterialReceiptGrid");
        //    _MemoryCache.Remove("KeyBatchDetailGrid");
        //    var MainModel = new MaterialReceiptModel();
        //    MainModel = await BindModels(MainModel);
        //    MainModel.FinFromDate = HttpContext.Session.GetString("FromDate");
        //    MainModel.FinToDate = HttpContext.Session.GetString("ToDate");
        //    MainModel.YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
        //    MainModel.EnteredByEmpname = HttpContext.Session.GetString("EmpName");
        //    MainModel.ActualEntryByName = HttpContext.Session.GetString("EmpName");
        //    MainModel.CreatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
        //    MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
        //    {
        //        AbsoluteExpiration = DateTime.Now.AddMinutes(60),
        //        SlidingExpiration = TimeSpan.FromMinutes(55),
        //        Size = 1024,
        //    };

        //    _MemoryCache.Set("KeyMaterialReceiptGrid", MainModel, cacheEntryOptions);
        //    MainModel.DateIntact = "N";
        //    return View(MainModel);
        //}
        [HttpPost]
        [ValidateAntiForgeryToken]
       // [Route("{controller}/Index")]
        public async Task<IActionResult> MaterialReceipt(MaterialReceiptModel model)
        {
            var fromDt = model.FromDate;
            var toDt = model.ToDate;
            model.EntryDate = string.IsNullOrEmpty(model.EntryDate) ? DateTime.Today.ToString() : model.EntryDate;
            try
            {
                var MRGrid = new DataTable();
                var BatchGrid = new DataTable();
                _MemoryCache.TryGetValue("KeyMaterialReceiptGrid", out List<MaterialReceiptDetail> MaterialReceiptDetail);
                _MemoryCache.TryGetValue("KeyBatchDetailGrid", out List<BatchDetailModel> BatchDetail);
                if (MaterialReceiptDetail == null)
                {
                    ModelState.Clear();
                    ModelState.TryAddModelError("Material Receipt Detail", "Material Receipt Grid Should Have At least 1 Item...!");
                    model = await BindModels(model);
                    return View("MaterialReceipt", model);
                }
                else
                {
                    model.CreatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));

                    model.CC = HttpContext.Session.GetString("Branch");
                    model.ItemDetailGrid = MaterialReceiptDetail;
                    model.BatchDetailGrid = BatchDetail;
                    MRGrid = GetDetailTable(MaterialReceiptDetail);
                    if (BatchDetail != null)
                        BatchGrid = GetBatchDetailTable(BatchDetail, model.CC);
                    if (model.Mode == "U")
                    {
                        model.UpdatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                    }
                    var Result = await _IMaterialReceipt.SaveMaterialReceipt(model, MRGrid, BatchGrid);

                    if (Result != null)
                    {
                        if (Result.StatusText == "Success" && Result.StatusCode == HttpStatusCode.OK)
                        {
                            ViewBag.isSuccess = true;
                            TempData["200"] = "200";
                            var MainModel = new MaterialReceiptModel();
                            MainModel.FinFromDate = HttpContext.Session.GetString("FromDate");
                            MainModel.YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
                            MainModel.FinToDate = HttpContext.Session.GetString("ToDate");
                            MainModel.EnteredByEmpname = HttpContext.Session.GetString("EmpName");
                            MainModel.CreatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                            _MemoryCache.Remove("KeyMaterialReceiptGrid");
                            _MemoryCache.Remove("KeyBatchDetailGrid");
                            MainModel.ActualEnteredBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                            MainModel.ActualEntryByName = HttpContext.Session.GetString("EmpName");
                            MainModel.ActualEntryDate = DateTime.Now;
                            //MainModel = await BindModels(MainModel);
                            MainModel.EmployeeList = await _IMaterialReceipt.GetEmployeeList();
                            MainModel.DateIntact = "Y";
                            MainModel.FromDate = fromDt;
                            MainModel.ToDate = toDt;
                            return View(MainModel);
                        }
                        if (Result.StatusText == "Success" && Result.StatusCode == HttpStatusCode.Accepted)
                        {
                            ViewBag.isSuccess = true;
                            TempData["202"] = "202";
                            var MainModel = new MaterialReceiptModel();
                            MainModel.FinFromDate = HttpContext.Session.GetString("FromDate");
                            MainModel.YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
                            MainModel.FinToDate = HttpContext.Session.GetString("ToDate");
                            MainModel.EnteredByEmpname = HttpContext.Session.GetString("EmpName");
                            MainModel.CreatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                            _MemoryCache.Remove("KeyMaterialReceiptGrid");
                            _MemoryCache.Remove("KeyBatchDetailGrid");
                            MainModel.ActualEnteredBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                            MainModel.ActualEntryByName = HttpContext.Session.GetString("EmpName");
                            MainModel.ActualEntryDate = DateTime.Now;
                            //MainModel = await BindModels(MainModel);
                            MainModel.EmployeeList = await _IMaterialReceipt.GetEmployeeList();
                            MainModel.Mode = "I";
                            MainModel.FromDate = fromDt;
                            MainModel.ToDate = toDt;
                            return View(MainModel);
                        }
                        if (Result.StatusText == "Error" && Result.StatusCode == HttpStatusCode.InternalServerError)
                        {
                            ViewBag.isSuccess = false;
                            var input = "";
                            if (Result != null)
                            {
                                input = Result.Result.ToString();
                                int index = input.IndexOf("#ERROR_MESSAGE");

                                if (index != -1)
                                {
                                    string errorMessage = input.Substring(index + "#ERROR_MESSAGE :".Length).Trim();
                                    TempData["ErrorMessage"] = errorMessage;
                                }
                                else
                                {
                                    string errorMessage = Result.Result.ToString();
                                    TempData["ErrorMessage"] = errorMessage;
                                }
                            }
                            else
                            {
                                TempData["500"] = "500";
                            }


                            _logger.LogError("\n \n ********** LogError ********** \n " + JsonConvert.SerializeObject(Result) + "\n \n");
                            model.IsError = "true";
                            return View("Error", Result);
                        }
                    }
                    model.DateIntact = "Y";

                    model.FromDate = fromDt;
                    model.ToDate = toDt;
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                LogException<MaterialReceiptController>.WriteException(_logger, ex);

                var ResponseResult = new ResponseResult()
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    StatusText = "Error",
                    Result = ex
                };

                return View("Error", ResponseResult);
            }
        }
       // [Route("{controller}/Index")]
        [HttpGet]
        public async Task<ActionResult> MaterialReceipt(int ID, string Mode, int YC, string FromDate = "", string ToDate = "", string VendorName = "", string GateNo = "", string PartCode = "", string ItemName = "", string MrnNo = "", string PoNo = "", string Type = "", string Searchbox = "")
        {
            _logger.LogInformation("\n \n ********** Page Gate Inward ********** \n \n " + _IWebHostEnvironment.EnvironmentName.ToString() + "\n \n");
            TempData.Clear();
            var MainModel = new MaterialReceiptModel();
            MainModel.FinFromDate = HttpContext.Session.GetString("FromDate");
            MainModel.YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
            MainModel.FinToDate = HttpContext.Session.GetString("ToDate");
            MainModel.EnteredByEmpname = HttpContext.Session.GetString("EmpName");
            MainModel.CreatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
            MainModel.EmployeeList = await _IMaterialReceipt.GetEmployeeList();
            MainModel.EnteredEmpId = Convert.ToInt32(HttpContext.Session.GetString("UID"));
            _MemoryCache.Remove("KeyMaterialReceiptGrid");
            _MemoryCache.Remove("KeyBatchDetailGrid");
            if (!string.IsNullOrEmpty(Mode) && ID > 0 && (Mode == "V" || Mode == "U"))
            {
                MainModel = await _IMaterialReceipt.GetViewByID(ID, YC).ConfigureAwait(false);
                MainModel.Mode = Mode;
                MainModel.ID = ID;
                //MainModel = await BindModels(MainModel).ConfigureAwait(false);
                MainModel.FinFromDate = HttpContext.Session.GetString("FromDate");
                MainModel.YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
                MainModel.FinToDate = HttpContext.Session.GetString("ToDate");
                MainModel.EmployeeList = await _IMaterialReceipt.GetEmployeeList();

                MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                    SlidingExpiration = TimeSpan.FromMinutes(55),
                    Size = 1024,
                };
                _MemoryCache.Set("KeyMaterialReceiptGrid", MainModel.ItemDetailGrid, cacheEntryOptions);
                _MemoryCache.Set("KeyBatchDetailGrid", MainModel.BatchDetailGrid, cacheEntryOptions);
            }
            else
            {
                MainModel = await BindModels(MainModel);
            }
            if (Mode != "U")
            {
                MainModel.ActualEnteredBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                MainModel.ActualEntryByName = HttpContext.Session.GetString("EmpName");
                MainModel.ActualEntryDate = DateTime.Now;
            }
            else
            {
                MainModel.UpdatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                MainModel.UpdatedByName = HttpContext.Session.GetString("EmpName");
                MainModel.UpdatedOn = DateTime.Now;
            }
            MainModel.FromDateBack = FromDate;
            MainModel.ToDateBack = ToDate;
            MainModel.VendorNameBack= VendorName;
            MainModel.GateNoBack = GateNo;
            MainModel.MrnNoBack = MrnNo;
            MainModel.DashboardTypeBack = Type;
            MainModel.GlobalSearchBack = Searchbox;
            MainModel.PoNoBack = PoNo;
            MainModel.PartCodeBack = PartCode;
            MainModel.ItemNameBack= ItemName;
            return View(MainModel);
        }
        public IActionResult AddMaterialReceiptDetail(List<MaterialReceiptDetail> model)
        {
            try
            {
                _MemoryCache.TryGetValue("KeyMaterialReceiptGrid", out IList<MaterialReceiptDetail> MaterialReceiptDetail);

                var MainModel = new MaterialReceiptModel();
                var MaterialReceiptGrid = new List<MaterialReceiptDetail>();
                var MaterialGrid = new List<MaterialReceiptDetail>();
                var SSGrid = new List<MaterialReceiptDetail>();
                MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                    SlidingExpiration = TimeSpan.FromMinutes(55),
                    Size = 1024,
                };
                var seqNo = 0;
                foreach (var item in model)
                {
                    if (item != null)
                    {
                        if (MaterialReceiptDetail == null)
                        {
                            item.SeqNo += seqNo + 1;
                            MaterialGrid.Add(item);
                            seqNo++;
                        }
                        else
                        {
                            if (MaterialReceiptDetail.Where(x => x.ItemNumber == item.ItemNumber).Any())
                            {
                                return StatusCode(207, "Duplicate");
                            }
                            else
                            {
                                item.SeqNo = MaterialReceiptDetail.Count + 1;
                                MaterialGrid = MaterialReceiptDetail.Where(x => x != null).ToList();
                                SSGrid.AddRange(MaterialGrid);
                                MaterialGrid.Add(item);
                            }
                        }
                        MainModel.ItemDetailGrid = MaterialGrid;

                        _MemoryCache.Set("KeyMaterialReceiptGrid", MainModel.ItemDetailGrid, cacheEntryOptions);
                    }
                }
                return PartialView("_MaterialReceiptGrid", MainModel);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //[HttpGet]
        public async Task<IActionResult> MRNDashboard(string FromDate = "", string ToDate = "", string Flag = "", string VendorName = "", string MrnNo = "", string GateNo = "", string PONo = "", string ItemName = "", string PartCode = "", string Type = "")
        {
            try
            {
                _MemoryCache.Remove("KeyMaterialReceiptGrid");
                var model = new MRNQDashboard();
                var Result = await _IMaterialReceipt.GetDashboardData().ConfigureAwait(true);

                if (Result != null)
                {
                    var _List = new List<TextValue>();
                    DataSet DS = Result.Result;
                    if (DS != null)
                    {
                        var DT = DS.Tables[0].DefaultView.ToTable(false, "VendorName", "MRNNo", "MrnDate", "GateNo", "GateDate",
  "InvNo", "InvDate", "Docname", "MRNQCCompleted", "TotalAmt", "NetAmt", "EntryId", "YearCode", "EntryBy", "UpdatedBy");
                        model.MRNQDashboard = CommonFunc.DataTableToList<MRNDashboard>(DT, "MRN");
                        foreach (var row in DS.Tables[0].AsEnumerable())
                        {
                            _List.Add(new TextValue()
                            {
                                Text = row["MrnNo"].ToString(),
                                Value = row["MrnNo"].ToString()
                            });
                        }
                        //var dd = _List.Select(x => x.Value).Distinct();
                        var _MRNList = _List.DistinctBy(x => x.Value).ToList();
                        model.MRNList = _MRNList;
                        if (Flag == "False")
                        {
                            model.FromDate1 = FromDate;
                            model.ToDate1 = ToDate;
                            model.VendorName = VendorName;
                            model.MrnNo = MrnNo;
                            model.GateNo = GateNo;
                            model.PONo = PONo;
                            model.ItemName = ItemName;
                            model.PartCode = PartCode;
                            model.DashboardType = Type;
                        }
                        _List = new List<TextValue>();
                    }
                }

                return View(model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IActionResult> MRNDetailDashboard(string FromDate = "", string ToDate = "", string Flag = "", string VendorName = "", string MrnNo = "", string GateNo = "", string PONo = "", string ItemName = "", string PartCode = "", string Type = "")
        {
            var model = new MRNQDashboard();
            model = await _IMaterialReceipt.GetDetailDashboardData(VendorName, MrnNo, GateNo, PONo, ItemName, PartCode, FromDate, ToDate);
            model.DashboardType = "Detail";
            return PartialView("_MRNDashboardGrid", model);
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
        public async Task<JsonResult> GetFormRights()
        {
            var userID = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
            var JSON = await _IMaterialReceipt.GetFormRights(userID);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }

        public IActionResult DeleteItemRow(int SeqNo)
        {
            var MainModel = new MaterialReceiptModel();
            _MemoryCache.TryGetValue("KeyMaterialReceiptGrid", out List<MaterialReceiptDetail> MaterialReceiptGrid);
            int Indx = Convert.ToInt32(SeqNo) - 1;

            if (MaterialReceiptGrid != null && MaterialReceiptGrid.Count > 0)
            {
                MaterialReceiptGrid.RemoveAt(Convert.ToInt32(Indx));

                Indx = 0;

                foreach (var item in MaterialReceiptGrid)
                {
                    Indx++;
                    item.SeqNo = Indx;
                }
                MainModel.ItemDetailGrid = MaterialReceiptGrid;

                MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                    SlidingExpiration = TimeSpan.FromMinutes(55),
                    Size = 1024,
                };


                if (MaterialReceiptGrid.Count == 0)
                {
                    _MemoryCache.Remove("KeyMaterialReceiptGrid");
                }
                //_MemoryCache.Set("KeyMaterialReceiptGrid", MainModel.ItemDetailGrid, cacheEntryOptions);
            }
            return PartialView("_MaterialReceiptGrid", MainModel);
        }
        public IActionResult DeleteBatchItemRow(int SeqNo)
        {
            var MainModel = new MaterialReceiptModel();
            _MemoryCache.TryGetValue("KeyBatchDetailGrid", out List<BatchDetailModel> BatchDetailModel);
            int Indx = Convert.ToInt32(SeqNo) - 1;

            if (BatchDetailModel != null && BatchDetailModel.Count > 0)
            {
                BatchDetailModel.RemoveAt(Convert.ToInt32(Indx));

                Indx = 0;

                foreach (var item in BatchDetailModel)
                {
                    Indx++;
                    item.SeqNo = Indx;
                }
                MainModel.BatchDetailGrid = BatchDetailModel;

                MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                    SlidingExpiration = TimeSpan.FromMinutes(55),
                    Size = 1024,
                };


                if (BatchDetailModel.Count == 0)
                {
                    _MemoryCache.Remove("KeyBatchDetailGrid");
                }
                //_MemoryCache.Set("KeyMaterialReceiptGrid", MainModel.ItemDetailGrid, cacheEntryOptions);
            }
            return PartialView("_BatchDetailAdd", MainModel);
        }
        private static DataTable GetDetailTable(IList<MaterialReceiptDetail> DetailList)
        {
            var MRGrid = new DataTable();

            MRGrid.Columns.Add("SeqNo", typeof(int));
            MRGrid.Columns.Add("pono", typeof(string));
            MRGrid.Columns.Add("poyearcode", typeof(int));
            MRGrid.Columns.Add("schno", typeof(string));
            MRGrid.Columns.Add("schyearcode", typeof(int));
           // MRGrid.Columns.Add("SchDate", typeof(string));
            MRGrid.Columns.Add("PoType", typeof(string));
            MRGrid.Columns.Add("PoAmendNo", typeof(int));
            MRGrid.Columns.Add("PODate", typeof(string));
            MRGrid.Columns.Add("ItemCode", typeof(int));
            MRGrid.Columns.Add("Unit", typeof(string));
            MRGrid.Columns.Add("RateUnit", typeof(string));
            MRGrid.Columns.Add("AltUnit", typeof(string));
            MRGrid.Columns.Add("NoOfCase", typeof(int));
            MRGrid.Columns.Add("BillQty", typeof(decimal));
            MRGrid.Columns.Add("RecQty", typeof(decimal));
            MRGrid.Columns.Add("AltRecQty", typeof(decimal));
            MRGrid.Columns.Add("ShortExcessQty", typeof(decimal));
            MRGrid.Columns.Add("Rate", typeof(decimal));
            MRGrid.Columns.Add("rateinother", typeof(decimal));
            MRGrid.Columns.Add("Amount", typeof(decimal));
            MRGrid.Columns.Add("PendPOQty", typeof(decimal));
            MRGrid.Columns.Add("QCCompleted", typeof(char));
            MRGrid.Columns.Add("RetChallanPendQty", typeof(decimal));
            MRGrid.Columns.Add("batchWise", typeof(string));
            MRGrid.Columns.Add("Salebillno", typeof(string));
            MRGrid.Columns.Add("salebillYearCode", typeof(int));
            MRGrid.Columns.Add("AgainstChallanNo", typeof(string));
            MRGrid.Columns.Add("Batchno", typeof(string));
            MRGrid.Columns.Add("Uniquebatchno", typeof(string));
            MRGrid.Columns.Add("SupplierBatchNo", typeof(string));
            MRGrid.Columns.Add("ShelfLife", typeof(decimal));
            MRGrid.Columns.Add("ItemSize", typeof(string));
            MRGrid.Columns.Add("ItemColor", typeof(string));
            MRGrid.Columns.Add("PartCode", typeof(string));
            DateTime currentDate = DateTime.Now;
            foreach (var Item in DetailList)
            {
                if (Item.AltUnit == "undefined" || Item.AltUnit == null || Item.AltUnit == "null")
                {
                    Item.AltUnit = "";
                }
                DateTime poDt = new DateTime();

                poDt = Convert.ToDateTime(Item.PODate);
                MRGrid.Rows.Add(
                    new object[]
                    {
                    Item.SeqNo,
                    Item.PONO == null ? "" : Item.PONO,
                    Item.PoYearCode,
                    Item.SchNo == null ? "" : Item.SchNo,
                    Item.SchYearCode,
                  //  Item.SchDate==null?currentDate :ParseFormattedDate(Item.SchDate),
                    Item.PoType == null ? "" : Item.PoType,
                    Item.POAmendNo,
                    //"2024/02/11",
                    Item.PODate == null ? currentDate : ParseFormattedDate(Item.PODate),
                    Item.ItemCode,
                    Item.Unit == null ? "" : Item.Unit,
                    Item.RateUnit == null ? "" : Item.RateUnit,
                    Item.AltUnit == null ? "" : Item.AltUnit,
                    Item.NoOfCase,
                    Item.BillQty,
                    Item.RecQty,
                    Item.AltRecQty,
                    Item.ShortExcessQty,
                    Item.Rate,
                    Item.RateinOther,
                    Item.Amount,
                    Item.PendPOQty,
                    Item.QCCompleted == null ? "" : Item.QCCompleted,
                    Item.RetChallanPendQty,
                    Item.BatchWise == null ? "" : Item.BatchWise,
                    Item.SaleBillNo == null ? "" : Item.SaleBillNo,
                    Item.SaleBillYearCode,
                    Item.AgainstChallanNo == null ? "" : Item.AgainstChallanNo,
                    Item.BatchNo== null ? "" : Item.BatchNo,
                    Item.UniqueBatchNo== null ? "" : Item.UniqueBatchNo,
                    Item.SupplierBatchNo== null ? "" : Item.SupplierBatchNo,
                    Item.ShelfLife,
                    Item.ItemSize == null ? "" : Item.ItemSize,
                    Item.ItemColor == null ? "" : Item.ItemColor,
                    Item.PartCode== null ? "" : Item.PartCode,
                    });
            }
            MRGrid.Dispose();
            return MRGrid;
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
        private static DataTable GetBatchDetailTable(IList<BatchDetailModel> DetailList, string Branch)
        {
            var MRGrid = new DataTable();

            MRGrid.Columns.Add("EntryId", typeof(int));
            MRGrid.Columns.Add("Yearcode", typeof(int));
            MRGrid.Columns.Add("ItemCode", typeof(int));
            MRGrid.Columns.Add("PONo", typeof(string));
            MRGrid.Columns.Add("PODate", typeof(string));
            MRGrid.Columns.Add("POYearcode", typeof(int));
            MRGrid.Columns.Add("SchNo", typeof(string));
            MRGrid.Columns.Add("SchDate", typeof(string));
            MRGrid.Columns.Add("SchYearcode", typeof(int));
            MRGrid.Columns.Add("TotalQty", typeof(float));
            MRGrid.Columns.Add("TotalRecQty", typeof(float));
            MRGrid.Columns.Add("VendorBatchQty", typeof(float));
            MRGrid.Columns.Add("VendorBatchNo", typeof(string));
            MRGrid.Columns.Add("Uniquebatchno", typeof(string));
            MRGrid.Columns.Add("CC", typeof(string));
            MRGrid.Columns.Add("ManufactureDate", typeof(string));
            MRGrid.Columns.Add("ExpiryDate", typeof(string));

            var currentDate = ParseFormattedDate((DateTime.Today).ToString());
            foreach (var Item in DetailList)
            {

                MRGrid.Rows.Add(
                    new object[]
                    {
                        Item.EntryId,
                        Item.YearCode,
                        Item.ItemCode,
                        Item.PONO,
                        Item.PODate?? currentDate,
                        //currentDate,//change after ward - PODATE
                        Item.POYearCode,
                        Item.SchNO ?? "",
                        //Item.SchDate??
                        currentDate,
                        //currentDate,//change afterward - schdate
                        Item.SchYearCode ?? 0,
                        Item.Qty,
                        Item.RecQty,
                        Item.VendorBatchQty,
                        Item.VendorBatchNo,
                        Item.UniqueBatchNO,
                        Item.CC,
                        currentDate,
                        currentDate
                    });
            }
            MRGrid.Dispose();
            return MRGrid;
        }
        public async Task<MaterialReceiptModel> BindModels(MaterialReceiptModel model)
        {
            if (model == null)
            {
                //model = new MaterialReceiptModel
                //{
                //    Mo
                //};
            }
            //model.PONO = await _IDataLogic.GetDropDownList("PENDINGPOLIST","I", "SP_GateMainDetail");
            return model;
        }
        public async Task<JsonResult> CheckFeatureOption()
        {
            var JSON = await _IMaterialReceipt.CheckFeatureOption();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetGateNo(string FromDate, string ToDate)
        {
            var JSON = await _IMaterialReceipt.GetGateNo("PENDINGGATEFORMRN", "SP_MRN", FromDate, ToDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> BindDept()
        {
            var JSON = await _IMaterialReceipt.BindDept("BINDDEPT", "SP_MRN");
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> ClearGridAjax(string FromDate, string ToDate)
        {
            _MemoryCache.Remove("KeyMaterialReceiptGrid");
            _MemoryCache.Remove("KeyBatchDetailGrid");
            var JSON = await _IMaterialReceipt.GetGateNo("PENDINGGATEFORMRN", "SP_MRN", FromDate, ToDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> ClearbatchGridAjax(string FromDate, string ToDate)
        {
            _MemoryCache.Remove("KeyBatchDetailGrid");
            var JSON = await _IMaterialReceipt.GetGateNo("PENDINGGATEFORMRN", "SP_MRN", FromDate, ToDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetGateMainData(string GateNo, string GateYearCode, int GateEntryId)
        {
            var JSON = await _IMaterialReceipt.GetGateMainData("GATEMAINDATA", "SP_MRN", GateNo, GateYearCode, GateEntryId);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetGateItemData(string GateNo, string GateYearCode, int GateEntryId)
        {
            var JSON = await _IMaterialReceipt.GetGateMainData("GATEMAINITEM", "SP_MRN", GateNo, GateYearCode, GateEntryId);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> AltUnitConversion(int ItemCode, decimal AltQty, decimal UnitQty)
        {
            var JSON = await _IMaterialReceipt.AltUnitConversion(ItemCode, AltQty, UnitQty);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetDeptAndEmp(int DeptId, int RespEmp)
        {
            var JSON = await _IMaterialReceipt.GetDeptAndEmp("GetDEPTANDEMP", "SP_MRN", DeptId, RespEmp);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<IActionResult> GetSearchData(string VendorName, string MrnNo, string GateNo, string PONo, string ItemName, string PartCode, string FromDate, string ToDate)
        {
            //model.Mode = "Search";
            var model = new MRNQDashboard();
            model = await _IMaterialReceipt.GetDashboardData(VendorName, MrnNo, GateNo, PONo, ItemName, PartCode, FromDate, ToDate);
            return PartialView("_MRNDashboardGrid", model);
        }
        public async Task<IActionResult> DeleteByID(int ID, int YC, string FromDate = "", string ToDate = "", string VendorName = "", string MrnNo = "", string GateNo = "", string PONo = "", string ItemName = "", string PartCode = "", string Type = "")
        {
            var Result = await _IMaterialReceipt.DeleteByID(ID, YC);

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

            return RedirectToAction("MRNDashboard", new { FromDate = formattedFromDate, ToDate = formattedToDate, Flag = "False", VendorName = VendorName, MrnNo = MrnNo, GateNo = GateNo, PONo = PONo, ItemName = ItemName, PartCode = PartCode, Type = Type });

        }
        public async Task<JsonResult> FillEntryandMRN(int YearCode)
        {
            var JSON = await _IMaterialReceipt.FillEntryandMRN("NewEntryId", YearCode, "SP_MRN");
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }

        public IActionResult AddBatchDetail(BatchDetailModel model)
        {
            try
            {
                _MemoryCache.TryGetValue("KeyBatchDetailGrid", out IList<BatchDetailModel> BatchDetailGrid);


                var MainModel = new MaterialReceiptModel();
                var MaterialReceiptGrid = new List<BatchDetailModel>();
                var MaterialGrid = new List<BatchDetailModel>();
                var SSGrid = new List<BatchDetailModel>();

                if (model != null)
                {
                    if (BatchDetailGrid == null)
                    {
                        if (model.VendorBatchQty > model.RecQty)
                        {
                            return StatusCode(207, "Duplicate");

                        }

                        else
                        {
                            model.SeqNo = 1;
                            MaterialGrid.Add(model);
                        }
                    }
                    else
                    {
                        decimal? sum = 0;
                        foreach (var item in BatchDetailGrid)
                        {
                            if (string.IsNullOrEmpty(model.SchNO))
                            {
                                if (item.PartCode == model.PartCode && item.PONO == model.PONO)
                                {
                                    sum += item.VendorBatchQty;
                                }
                            }
                            else
                            {
                                if (item.PartCode == model.PartCode && item.PONO == model.PONO && item.SchNO == model.SchNO)
                                {
                                    sum += item.VendorBatchQty;
                                }
                            }

                        }
                        sum += model.VendorBatchQty;

                        if (BatchDetailGrid.Where(x => x.PartCode == model.PartCode && sum > x.RecQty && x.PONO == model.PONO && (x.SchNO == model.SchNO || string.IsNullOrEmpty(x.SchNO))).Any())
                        {
                            return StatusCode(207, "Duplicate");
                        }
                        else if (BatchDetailGrid.Where(x => x.VendorBatchNo == model.VendorBatchNo && x.PartCode == model.PartCode && x.PONO == model.PONO && x.SchNO == model.SchNO).Any())
                        {
                            return StatusCode(205, "DuplicateVendor");
                        }
                        else
                        {
                            model.SeqNo = BatchDetailGrid.Count + 1;
                            MaterialGrid = BatchDetailGrid.Where(x => x != null).ToList();
                            SSGrid.AddRange(MaterialGrid);
                            MaterialGrid.Add(model);
                        }
                    }

                    MainModel.BatchDetailGrid = MaterialGrid;

                    MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
                    {
                        AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                        SlidingExpiration = TimeSpan.FromMinutes(55),
                        Size = 1024,
                    };

                    _MemoryCache.Set("KeyBatchDetailGrid", MainModel.BatchDetailGrid, cacheEntryOptions);
                }
                else
                {
                    ModelState.TryAddModelError("Error", "Schedule List Cannot Be Empty...!");
                }

                return PartialView("_BatchDetailAdd", MainModel);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IActionResult EditItemRow(int SeqNo)
        {
            _MemoryCache.TryGetValue("KeyMaterialReceiptGrid", out List<MaterialReceiptDetail> MaterialGrid);

            var SSGrid = MaterialGrid.Where(x => x.SeqNo == SeqNo);
            string JsonString = JsonConvert.SerializeObject(SSGrid);
            return Json(JsonString);
        }
        public async Task<JsonResult> CheckEditOrDelete(string MRNNo, int YearCode)
        {
            var JSON = await _IMaterialReceipt.CheckEditOrDelete(MRNNo, YearCode);
            _logger.LogError(JsonConvert.SerializeObject(JSON));
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> CheckBeforeInsert(string GateNo, int GateYearCode)
        {
            var JSON = await _IMaterialReceipt.CheckBeforeInsert(GateNo, GateYearCode);
            _logger.LogError(JsonConvert.SerializeObject(JSON));
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }


    }
}
