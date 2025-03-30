using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using FastReport.Export.Html;
using FastReport.Export.Image;
using FastReport.Web;
using FastReport;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NuGet.Packaging;
using OfficeOpenXml;
using System.Diagnostics;
using static eTactWeb.DOM.Models.Common;
using static eTactWeb.Data.Common.CommonFunc;
using System.Net;
using System.Data;
using System.Globalization;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Drawing.Drawing2D;

namespace eTactWeb.Controllers
{
  
    public class DirectPurchaseBillController : Controller
    {
        private readonly IMemoryCacheService _iMemoryCacheService;
        private readonly IWebHostEnvironment _IWebHostEnvironment;
        private readonly IConfiguration iconfiguration;
        public ILogger<DirectPurchaseBillModel> _Logger { get; set; }
        public CultureInfo CI { get; private set; }
        public EncryptDecrypt EncryptDecrypt { get; private set; }
        public IDataLogic IDataLogic { get; private set; }
        public IMemoryCache IMemoryCache { get; private set; }
        public IDirectPurchaseBill IDirectPurchaseBill { get; set; }

        
        public DirectPurchaseBillController(IDirectPurchaseBill iDirectPurchaseBill, IDataLogic iDataLogic, IMemoryCache iMemoryCache, ILogger<DirectPurchaseBillModel> logger, EncryptDecrypt encryptDecrypt, IMemoryCacheService iMemoryCacheService, IWebHostEnvironment iWebHostEnvironment, IConfiguration configuration)
        {
            _iMemoryCacheService = iMemoryCacheService;
            IDirectPurchaseBill = iDirectPurchaseBill;
            IDataLogic = iDataLogic;
            IMemoryCache = iMemoryCache;
            _Logger = logger;
            EncryptDecrypt = encryptDecrypt;
            CI = new CultureInfo("en-GB");
            _IWebHostEnvironment = iWebHostEnvironment;
            iconfiguration = configuration;
        }

        [HttpGet]
        public  async Task <IActionResult> DirectPurchaseBill(int ID, int YC, string Mode, string? TypeITEMSERVASSETS, string FromDate = "", string ToDate = "", string DashboardType = "", string DocumentType     = "", string VendorName = "", string PurchVouchNo = "", string InvoiceNo = "", string PartCode = "", string ItemName = "", string HSNNo = "", string Searchbox = "")
        {
            //IMemoryCache.Remove("PBTaxGrid");
            IMemoryCache.Remove("KeyTaxGrid");
            //IMemoryCache.Remove("PBTDSGrid");
            IMemoryCache.Remove("KeyTDSGrid");
            IMemoryCache.Remove("DirectPurchaseBill");
            IMemoryCache.Remove("KeyAdjGrid");
            //var model = new DirectPurchaseBillModel();
            //sumu extracode starts
            var MainModel = new DirectPurchaseBillModel();
            MainModel.YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
            MainModel.FinFromDate = HttpContext.Session.GetString("FromDate");
            MainModel.FinToDate = HttpContext.Session.GetString("ToDate");
            MainModel.PreparedBy = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
            MainModel.PreparedByName = HttpContext.Session.GetString("EmpName");
            MainModel.Branch = HttpContext.Session.GetString("Branch");

            if (!string.IsNullOrEmpty(Mode) && ID > 0 && (Mode == "V" || Mode == "U"))
            {
                MainModel = await IDirectPurchaseBill.GetViewByID(ID, YC, "ViewByID").ConfigureAwait(false);
                MainModel.Mode = Mode;
                MainModel.TDSMode = Mode;
                MainModel.ID = ID;
                MainModel.YearCode = YC;
                MainModel = await BindModels(MainModel).ConfigureAwait(false);
                MainModel.FinFromDate = HttpContext.Session.GetString("FromDate");
                MainModel.FinToDate = HttpContext.Session.GetString("ToDate");
                //MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
                //{
                //    AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                //    SlidingExpiration = TimeSpan.FromMinutes(55),
                //    Size = 1024,
                //};
                IMemoryCache.Set("DirectPurchaseBill", MainModel, DateTimeOffset.Now.AddMinutes(60));
                IMemoryCache.Set("KeyTaxGrid", MainModel.TaxDetailGridd == null ? new List<TaxModel>() : MainModel.TaxDetailGridd, DateTimeOffset.Now.AddMinutes(60));
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

            MainModel.FromDateBack = FromDate;
            MainModel.ToDateBack = ToDate;
            MainModel.TypeITEMSERVASSETSBack = TypeITEMSERVASSETS != null && TypeITEMSERVASSETS != "0" && TypeITEMSERVASSETS != "undefined" ? TypeITEMSERVASSETS : "";
            MainModel.DocumentTypeBack = DocumentType != null && DocumentType != "0" && DocumentType != "undefined" ? DocumentType : "";
            MainModel.DashboardTypeBack = DashboardType != null && DashboardType != "0" && DashboardType != "undefined" ? DashboardType : "";
            MainModel.PurchVouchNoBack = PurchVouchNo != null && PurchVouchNo != "0" && PurchVouchNo != "undefined" ? PurchVouchNo : "";
            MainModel.InvoiceNoBack = InvoiceNo != null && InvoiceNo != "0" && InvoiceNo != "undefined" ? InvoiceNo : "";
            MainModel.VendorNameBack = VendorName != null && VendorName != "0" && VendorName != "undefined" ? VendorName : "";
            MainModel.PartCodeBack = PartCode != null && PartCode != "0" && PartCode != "undefined" ? PartCode : "";
            MainModel.ItemNameBack = ItemName != null && ItemName != "0" && ItemName != "undefined" ? ItemName : "";
            MainModel.HSNNoBack = HSNNo != null && HSNNo != "0" && HSNNo != "undefined" ? HSNNo : "";
            MainModel.GlobalSearchBack = Searchbox != null && Searchbox != "0" && Searchbox != "undefined" ? Searchbox : "";

            IMemoryCache.Set("DirectPurchaseBill", MainModel, DateTimeOffset.Now.AddMinutes(60));
            IMemoryCache.Set("KeyTaxGrid", MainModel.TaxDetailGridd == null ? new List<TaxModel>() : MainModel.TaxDetailGridd, DateTimeOffset.Now.AddMinutes(60));
            IMemoryCache.Set("KeyTDSGrid", MainModel.TDSDetailGridd == null ? new List<TDSModel>() : MainModel.TDSDetailGridd, DateTimeOffset.Now.AddMinutes(60));
            IMemoryCache.Set("KeyAdjGrid", MainModel.adjustmentModel == null ? new AdjustmentModel() : MainModel.adjustmentModel, DateTimeOffset.Now.AddMinutes(60));
            HttpContext.Session.SetString("DirectPurchaseBill", JsonConvert.SerializeObject(MainModel));
            MainModel.adjustmentModel = (MainModel.adjustmentModel != null && MainModel.adjustmentModel.AdjAdjustmentDetailGrid != null) ? MainModel.adjustmentModel : new AdjustmentModel();

            //extra codes end


            return View(MainModel);
            //return View(model);
        }

        [HttpPost]
        public async Task <IActionResult> DirectPurchaseBill(DirectPurchaseBillModel model)
        {
            //if (ModelState.IsValid)
            //{

            //    ViewBag.Message = $"Textbox1: {model.TextBox1Value}, Textbox2: {model.TextBox2Value}"; // Store to display in the View.

            //    return View(model);  
            //}
            //return View(model);  

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

                IMemoryCache.TryGetValue("DirectPurchaseBill", out DirectPurchaseBillModel MainModel);

                IMemoryCache.TryGetValue("KeyTaxGrid", out List<TaxModel> TaxGrid);
                IMemoryCache.TryGetValue("KeyTDSGrid", out List<TDSModel> TdsGrid);
                IMemoryCache.TryGetValue("KeyDrCrGrid", out List<DbCrModel> DrCrGrid);
                //IMemoryCache.TryGetValue("KeyAdjGrid", out AdjustmentModel AdjGrid);

                var cc = stat.CurrentEntryCount;
                var pp = stat.CurrentEstimatedSize;

                ModelState.Clear();

                if (MainModel.ItemDetailGrid != null && MainModel.ItemDetailGrid.Count > 0)
                {
                    DS = GetItemDetailTable(MainModel.ItemDetailGrid, model.Mode, MainModel.EntryID, MainModel.YearCode);
                    ItemDetailDT = DS.Tables[0];
                    model.ItemDetailGrid = MainModel.ItemDetailGrid;

                    isError = false;
                    if (MainModel.ItemDetailGrid != null && MainModel.ItemDetailGrid.Any())
                    {
                        var hasDupes = MainModel.ItemDetailGrid.GroupBy(x => new { x.ItemCode, x.docTypeId, x.Description })
                       .Where(x => x.Skip(1).Any()).Any();
                        if (hasDupes)
                        {
                            isError = true;
                            ErrList.Add("ItemDetailGrid", "Document Type + ItemCode + Description In ItemDetails can not be Duplicate...!");
                        }
                    }
                }
                else
                {
                    ErrList.Add("ItemDetailGrid", "Item Details Cannot Be Blank..!");
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

                if (MainModel.adjustmentModel != null && MainModel.adjustmentModel.AdjAdjustmentDetailGrid != null && MainModel.adjustmentModel.AdjAdjustmentDetailGrid.Count > 0)
                {
                    AdjDetailDT = CommonController.GetAdjDetailTable(MainModel.adjustmentModel.AdjAdjustmentDetailGrid.ToList(), model.EntryID, model.YearCode, model.AccountCode);
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
                            string ImagePath = "Uploads/DirectPurchaseBill/";

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
                            string ImagePath = "Uploads/DirectPurchaseBill/";

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
                            string ImagePath = "Uploads/DirectPurchaseBill/";

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
                        model.EntryByMachineName = HttpContext.Session.GetString("EmpName");
                        model.CreatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                        Result = await IDirectPurchaseBill.SaveDirectPurchaseBILL(ItemDetailDT, TaxDetailDT, TDSDetailDT, model, DrCrDetailDT, AdjDetailDT);
                    }

                    if (Result != null)
                    {
                        if (Result.StatusText == "Success" && Result.StatusCode == HttpStatusCode.OK)
                        {
                            ViewBag.isSuccess = true;
                            TempData["200"] = "200";
                            IMemoryCache.Remove("KeyTaxGrid");
                            IMemoryCache.Remove("KeyTDSGrid");
                            IMemoryCache.Remove(ItemDetailDT);
                            IMemoryCache.Remove("DirectPurchaseBill");
                        }
                        else if (Result.StatusText == "Inserted Successfully" && Result.StatusCode == HttpStatusCode.Accepted)
                        {
                            ViewBag.isSuccess = true;
                            TempData["200"] = "200";
                            IMemoryCache.Remove("KeyTaxGrid");
                            IMemoryCache.Remove("KeyTDSGrid");
                            IMemoryCache.Remove(ItemDetailDT);
                            IMemoryCache.Remove("DirectPurchaseBill");
                        }
                        else if (Result.StatusText == "Updated Successfully" && Result.StatusCode == HttpStatusCode.Accepted)
                        {
                            ViewBag.isSuccess = true;
                            TempData["202"] = "202";
                            IMemoryCache.Remove("KeyTaxGrid");
                            IMemoryCache.Remove("KeyTDSGrid");
                            IMemoryCache.Remove(ItemDetailDT);
                            IMemoryCache.Remove("DirectPurchaseBill");
                            return RedirectToAction(nameof(DirectPurchaseBill));
                        }
                        else if (Result.StatusText == "Deleted Successfully" && Result.StatusCode == HttpStatusCode.Accepted)
                        {
                            ViewBag.isSuccess = true;
                            TempData["410"] = "410";
                            IMemoryCache.Remove("KeyTaxGrid");
                            IMemoryCache.Remove("KeyTDSGrid");
                            IMemoryCache.Remove("DirectPurchaseBill");
                        }
                        else if (Result.StatusText == "Error" && Result.StatusCode == HttpStatusCode.InternalServerError)
                        {
                            var errNum = Result.ToString(); //.Result.Message.ToString().Split(":")[1];
                            if (errNum == " 2627")
                            {
                                ViewBag.isSuccess = false;
                                ViewBag.ResponseResult = Result.StatusCode + "Occurred while saving data" + Result.Result;
                                TempData["2627"] = "2627";
                                _Logger.LogError("\n \n ********** LogError ********** \n " + JsonConvert.SerializeObject(Result) + "\n \n");
                                var model2 = await BindModels(model);
                                model2.FinFromDate = HttpContext.Session.GetString("FromDate");
                                model2.FinToDate = HttpContext.Session.GetString("ToDate");
                                model2.YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
                                model2.Branch = HttpContext.Session.GetString("Branch");
                                model2.PreparedByName = HttpContext.Session.GetString("EmpName");
                                model2.UpdatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                                model2.UpdatedByName = HttpContext.Session.GetString("EmpName");
                                model2.CreatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                                return View("DirectPurchaseBill", model);
                            }
                            else
                            {
                                ViewBag.ResponseResult = Result.StatusCode + "Occurred while saving data" + Result.Result;
                                TempData["500"] = "500";
                                model = await BindModels(model);
                                model.adjustmentModel = model.adjustmentModel ?? new AdjustmentModel();
                                model.FinFromDate = HttpContext.Session.GetString("FromDate");
                                model.FinToDate = HttpContext.Session.GetString("ToDate");
                                model.YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
                                model.Branch = HttpContext.Session.GetString("Branch");
                                model.PreparedByName = HttpContext.Session.GetString("EmpName");
                                model.UpdatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                                model.UpdatedByName = HttpContext.Session.GetString("EmpName");
                                model.CreatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                                return View("DirectPurchaseBill", model);
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
                            model.PreparedByName = HttpContext.Session.GetString("EmpName");
                            model.UpdatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                            model.UpdatedByName = HttpContext.Session.GetString("EmpName");
                            model.CreatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                            if (Result.StatusText.Contains("success") && (Result.StatusCode == HttpStatusCode.OK || Result.StatusCode == HttpStatusCode.Accepted))
                            {
                                ViewBag.isSuccess = true;
                                TempData["202"] = "202";
                                IMemoryCache.Remove("KeyTaxGrid");
                                IMemoryCache.Remove("KeyTDSGrid");
                                IMemoryCache.Remove("DirectPurchaseBill");
                                return RedirectToAction(nameof(DirectPurchaseBill));
                            }
                            else
                            {
                                TempData["ErrorMessage"] = Result.StatusText;
                                IMemoryCache.Remove("KeyTaxGrid");
                                IMemoryCache.Remove("KeyTDSGrid");
                                return View("DirectPurchaseBill", model);
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
                    model1.PreparedByName = HttpContext.Session.GetString("EmpName");
                    model1.UpdatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                    model1.UpdatedByName = HttpContext.Session.GetString("EmpName");
                    model1.CreatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));

                    return RedirectToAction(nameof(DirectPurchaseBill));

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
            return View("DirectPurchaseBill", model);

        }





        public IActionResult PrintReport(int EntryId = 0, int YearCode = 0, string PONO = "")
        {
            string my_connection_string;
            string contentRootPath = _IWebHostEnvironment.ContentRootPath;
            string webRootPath = _IWebHostEnvironment.WebRootPath;
            //string frx = Path.Combine(_env.ContentRootPath, "reports", value.file);
            var webReport = new WebReport();

            var ReportName = IDirectPurchaseBill.GetReportName();
            if (ReportName.Result.Result.Rows[0].ItemArray[0] != System.DBNull.Value)
            {
                webReport.Report.Load(webRootPath + "\\" + ReportName.Result.Result.Rows[0].ItemArray[0] + ".frx"); // from database
            }
            else
            {
                webReport.Report.Load(webRootPath + "\\PO.frx"); // default report

            }
            //webReport.Report.SetParameterValue("flagparam", "PURCHASEORDERPRINT");
            webReport.Report.SetParameterValue("entryparam", EntryId);
            webReport.Report.SetParameterValue("yearparam", YearCode);
            webReport.Report.SetParameterValue("ponoparam", PONO);


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
        public IActionResult AddItem2Grid(DirectPurchaseBillModel model)
        {
            bool TF = false;

            IMemoryCache.TryGetValue("KeyTaxGrid", out IList<TaxModel> DPBTaxdetail);
            IMemoryCache.TryGetValue("KeyTDSGrid", out IList<TDSModel> DPBTDSdetail);
            IMemoryCache.TryGetValue("DirectPurchaseBill", out DirectPurchaseBillModel MainModel);

            //if (POTaxGrid != null && POTaxGrid.Count > 0)
            //{
            //    return StatusCode(205, "Reset Tax Detail");
            //}

            if (MainModel != null && MainModel.ItemDetailGrid != null)
            {
                TF = MainModel.ItemDetailGrid.Any(x => x.ItemCode == model.ItemCode && x.docTypeId == model.docTypeId && x.Description == model.Description);
            }

            if (TF)
            {
                return StatusCode(208, "Duplicate Item");
            }
            else
            {
                model = BindItem4Grid(model);
                IMemoryCache.Remove("DirectPurchaseBill");
                IMemoryCache.Set("DirectPurchaseBill", model, DateTimeOffset.Now.AddMinutes(60));
            }

            return PartialView("_DPBItemGrid", model);
        }
        public async Task<JsonResult> GetFormRights()
        {
            var userID = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
            var JSON = await IDirectPurchaseBill.GetFormRights(userID);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillItems(string Type, string ShowAllItem)
        {
            var JSON = await IDirectPurchaseBill.FillItems(Type, ShowAllItem);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FILLDocumentList(string ShowAllDocument)
        {
            var JSON = await IDirectPurchaseBill.FILLDocumentList(ShowAllDocument);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> CheckLockYear(int YearCode)
        {
            var JSON = await IDirectPurchaseBill.CheckLockYear(YearCode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillCurrency(string Ctrl)
        {
            var JSON = await IDirectPurchaseBill.FillCurrency(Ctrl);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetExchangeRate(string Currency)
        {
            var JSON = await IDirectPurchaseBill.GetExchangeRate(Currency);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }

        public async Task<IActionResult> GetItemServiceFORPO(string ItemService)
        {
            var JSONString = await IDirectPurchaseBill.GetItemServiceFORPO(ItemService);

            //var Dlist = JsonConvert.DeserializeObject<Dictionary<object, object>>(JSONString);
            //JObject json = JObject.Parse(JSONString);
            //object obj = JsonConvert.DeserializeObject(JSONString, typeof(object));
            //JToken jToken = (JToken)json;

            //object value = "";
            //Dlist.TryGetValue(key: "Result", out value);

            return Json(JSONString);
        }

        public async Task<DirectPurchaseBillModel> BindModels(DirectPurchaseBillModel model)
        {
            CommonFunc.LogException<DirectPurchaseBillModel>.LogInfo(_Logger, "**********  *************");

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

            SqlParams.Add(new KeyValuePair<string, string>("@Flag", "GETITEMS"));
            SqlParams.Add(new KeyValuePair<string, string>("@ShowAll", "T"));

            oDataSet = await IDataLogic.BindAllDropDown("DirectPurchaseBill", "SP_DirectPurchaseBillMainDetail", SqlParams).ConfigureAwait(false);


            model.AccountList = await IDataLogic.GetDropDownList("FILLVendorList", "SP_DirectPurchaseBillMainDetail");
            model.DocumentList = await IDataLogic.GetDropDownList("FILLDocumentList", "SP_DirectPurchaseBillMainDetail");
            model.ProcessList = await IDataLogic.GetDropDownList("ProcessList", "SP_GetDropDownList");
            model.CostCenterList = await IDataLogic.GetDropDownList("COSTCENTER", "SP_GetDropDownList");
            if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
            {
                model.PartCodeList = (from DataRow row in oDataSet.Tables[0].Rows select new Common.TextValue { Value = row[Constants.ItemCode].ToString(), Text = row[Constants.PartCode].ToString() }).ToList();

                model.ItemNameList = (from DataRow row in oDataSet.Tables[0].Rows select new Common.TextValue { Value = row[Constants.ItemCode].ToString(), Text = row[Constants.ItemName].ToString() }).ToList();

            }

            return model;
        }

        public async Task<IActionResult> DashBoard()
        {
            HttpContext.Session.Remove("DirectPurchaseBill");
            IMemoryCache.Remove("DirectPurchaseBill");
            HttpContext.Session.Remove("TaxGrid");
            IMemoryCache.Remove("KeyTaxGrid");

            var _List = new List<TextValue>();

            var MainModel = await IDirectPurchaseBill.GetDashBoardData();

            MainModel.FromDate = new DateTime(DateTime.Today.Year, 4, 1).ToString("dd/MM/yyyy").Replace("-", "/");
            MainModel.ToDate = new DateTime(DateTime.Today.Year + 1, 3, 31).ToString("dd/MM/yyyy").Replace("-", "/");// Last day in January next year

            return View(MainModel);
        }

        public async Task<IActionResult> DeleteByIDOld(int ID, int YC, string PurchVoucherNo, string InvNo = "", bool? IsDetail = false)
        {
            int EntryBy = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
            string EntryByMachineName = HttpContext.Session.GetString("EmpName");
            DateTime EntryDate = DateTime.Today;
            var Result = await IDirectPurchaseBill.DeleteByID(ID, YC, "DELETE", PurchVoucherNo, InvNo, EntryBy, EntryByMachineName, EntryDate);

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

            //return View();
            return RedirectToAction(nameof(DashBoard));
        }

        public async Task<JsonResult> DeleteByID(int ID, int YC, string PurchVoucherNo, string InvNo = "", bool? IsDetail = false)
        {
            int EntryBy = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
            string EntryByMachineName = HttpContext.Session.GetString("EmpName");
            DateTime EntryDate = DateTime.Today;
            var Result = await IDirectPurchaseBill.DeleteByID(ID, YC, "DELETE", PurchVoucherNo, InvNo, EntryBy, EntryByMachineName, EntryDate);

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

            return Json(rslt);
            //return RedirectToAction(nameof(DashBoard));
        }

        public IActionResult DeleteItemRow(string SeqNo)
        {
            bool exists = false;

            IMemoryCache.TryGetValue("KeyTDSGrid", out List<TDSModel> TDSGrid);
            IMemoryCache.TryGetValue("KeyTaxGrid", out List<TaxModel> TaxGrid);
            IMemoryCache.TryGetValue("DirectPurchaseBill", out DirectPurchaseBillModel MainModel);

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

                foreach (DPBItemDetail item in MainModel.ItemDetailGrid)
                {
                    Indx++;
                    item.SeqNo = Indx;
                }
                MainModel.ItemNetAmount = MainModel.ItemDetailGrid.Sum(x => x.Amount);

                //if (MainModel.ItemDetailGrid.Count <= 0)
                //{
                //    IMemoryCache.Remove("DirectPurchaseBill");
                //}
                //else
                //{
                //    IMemoryCache.Set("DirectPurchaseBill", MainModel, DateTimeOffset.Now.AddMinutes(60));
                //}

                IMemoryCache.Set("DirectPurchaseBill", MainModel, DateTimeOffset.Now.AddMinutes(60));
            }
            return PartialView("_DPBItemGrid", MainModel);
        }

        public IActionResult EditItemRow(DirectPurchaseBillModel model)
        {
            IMemoryCache.TryGetValue("DirectPurchaseBill", out DirectPurchaseBillModel MainModel);
            var SSGrid = MainModel.ItemDetailGrid.Where(x => x.SeqNo == model.SeqNo);
            string JsonString = JsonConvert.SerializeObject(SSGrid);
            return Json(JsonString);
        }

        public async Task<JsonResult> GetPartyList(string Check)
        {
            var JSON = await IDataLogic.GetDropDownList("CREDITORDEBTORLIST", Check, "SP_GetDropDownList");
            _Logger.LogError(JsonConvert.SerializeObject(JSON));
            return Json(JSON);
        }
        public async Task<JsonResult> GetGstRegister(int Code)
        {
            var JSON = await IDirectPurchaseBill.GetGstRegister("GSTRegistered", Code);
            string JSONString = JsonConvert.SerializeObject(JSON);
            _Logger.LogError(JsonConvert.SerializeObject(JSON));
            return Json(JSONString);
        }
        public async Task<JsonResult> GetStateGST(int Code)
        {
            var JSON = await IDirectPurchaseBill.GetStateGST("GetStateGST", Code);
            string JSONString = JsonConvert.SerializeObject(JSON);
            _Logger.LogError(JsonConvert.SerializeObject(JSON));
            return Json(JSONString);
        }
        public async Task<JsonResult> FillEntryandPONumber(int YearCode, string VODate)
        {
            var JSON = await IDirectPurchaseBill.FillEntryandVouchNoNumber(YearCode, VODate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> ClearTaxGrid(int YearCode, string VODate)
        {
            IMemoryCache.Remove("KeyTaxGrid");
            var JSON = await IDirectPurchaseBill.FillEntryandVouchNoNumber(YearCode, VODate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> ClearTDSGrid(int YearCode, string VODate)
        {
            IMemoryCache.Remove("KeyTDSGrid");
            var JSON = await IDirectPurchaseBill.FillEntryandVouchNoNumber(YearCode, VODate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillPONumber(int YearCode, string OrderType, string PODate)
        {
            var JSON = await IDirectPurchaseBill.FillPONumber(YearCode, OrderType, PODate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetPOData(string BillDate, int? AccountCode, int? ItemCode)
        {
            var JSON = await IDirectPurchaseBill.GetPOData(BillDate, AccountCode, ItemCode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetScheduleData(string PONo, int? POYear, string BillDate, int? AccountCode, int? ItemCode)
        {
            var JSON = await IDirectPurchaseBill.GetScheduleData(PONo, POYear, BillDate, AccountCode, ItemCode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetPOItems(string PONo, string BillDate, int? AccountCode, int? ItemCode)
        {
            var JSON = await IDirectPurchaseBill.GetPOData(BillDate, AccountCode, ItemCode);
            DataTable poItems = JSON.Result.Tables[0];
            if (!string.IsNullOrEmpty(PONo) && poItems != null)
            {
                var rows = poItems.AsEnumerable();
                rows = rows.Where(row => row.Field<string>("PONo") == PONo);
                var filteredTable = poItems.Clone();
                foreach (var row in rows.Take(1))
                {
                    filteredTable.ImportRow(row);
                }
                JSON.Result = filteredTable;
            }
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetScheduleItems(string ScheduleNo, string PONo, int? POYear, string BillDate, int? AccountCode, int? ItemCode)
        {
            var JSON = await IDirectPurchaseBill.GetScheduleData(PONo, POYear, BillDate, AccountCode, ItemCode);
            DataTable schItems = JSON.Result.Tables[0];
            if (!string.IsNullOrEmpty(ScheduleNo) && schItems != null)
            {
                var rows = schItems.AsEnumerable();
                rows = rows.Where(row => row.Field<string>("ScheduleNo") == ScheduleNo);
                var filteredTable = schItems.Clone();
                foreach (var row in rows.Take(1))
                {
                    filteredTable.ImportRow(row);
                }
                JSON.Result = filteredTable;
            }
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillVouchNumber(int YearCode, string VODate)
        {
            var JSON = await IDirectPurchaseBill.FillVouchNumber(YearCode, VODate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }

        public async Task<IActionResult> GetSearchData(DPBDashBoard model)
        {
            model.Mode = "SEARCH";
            model = await IDirectPurchaseBill.GetSummaryData(model);
            model.DashboardType = "Summary";
            return PartialView("_DashBoardGrid", model);
        }
        public async Task<IActionResult> GetDetailData(DPBDashBoard model)
        {
            model.Mode = "SEARCH";
            var type = model.DashboardType;
            model = await IDirectPurchaseBill.GetDetailData(model);
            model.DashboardType = type;
            return PartialView("_DashBoardGrid", model);
        }



        public async Task<IActionResult> GetAmmCompSearchData(DPBDashBoard model)
        {
            model = await IDirectPurchaseBill.GetSearchCompData(model);
            model.Mode = "Completed";
            return PartialView("_AmmListGrid", model);
        }

        public JsonResult ResetGridItems()
        {
            HttpContext.Session.Remove("POItemList");
            IMemoryCache.Remove("DirectPurchaseBill");
            IMemoryCache.Remove("KeyTaxGrid");
            IMemoryCache.Remove("KeyTDSGrid");

            var MainModel = new DirectPurchaseBillModel();
            List<TaxModel> taxList = new List<TaxModel>();
            List<TDSModel> tdsList = new List<TDSModel>();

            MainModel.YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
            MainModel.FinFromDate = HttpContext.Session.GetString("FromDate");
            MainModel.FinToDate = HttpContext.Session.GetString("ToDate");
            MainModel.PreparedBy = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
            MainModel.PreparedByName = HttpContext.Session.GetString("EmpName");
            MainModel.Branch = HttpContext.Session.GetString("Branch");

            IMemoryCache.Set("DirectPurchaseBill", MainModel, DateTimeOffset.Now.AddMinutes(60));
            IMemoryCache.Set("KeyTaxGrid", taxList, DateTimeOffset.Now.AddMinutes(60));
            IMemoryCache.Set("KeyTDSGrid", tdsList, DateTimeOffset.Now.AddMinutes(60));
            HttpContext.Session.SetString("DirectPurchaseBill", JsonConvert.SerializeObject(MainModel));
            IMemoryCache.TryGetValue("DirectPurchaseBill", out MainModel);
            IMemoryCache.TryGetValue("KeyTaxGrid", out List<TaxModel> TaxGrid);
            IMemoryCache.TryGetValue("KeyTDSGrid", out List<TDSModel> TdsGrid);

            return new(StatusCodes.Status200OK);
        }
        private static DataSet GetItemDetailTable(IList<DPBItemDetail> itemDetailList, string Mode, int? EntryID, int? YearCode)
        {
            DataSet DS = new();
            DataTable Table = new();

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

            foreach (DPBItemDetail Item in itemDetailList)
            {
                DateTime poDate = new DateTime();
                DateTime schDate = new DateTime();
                DateTime MIRDate = new DateTime();
                DateTime ProjectDate = new DateTime();
                DateTime AgainstImportInvDate = new DateTime();
                string poDt = "";
                string schDt = "";
                string mirDt = "";
                string projectDt = "";
                string againstImportInvDt = "";
                if (Item.PODate != null)
                {
                    poDate = DateTime.Parse(Item.PODate, new CultureInfo("en-GB"));
                    poDt = poDate.ToString("yyyy/MM/dd");
                }
                else
                {
                    poDt = DateTime.Today.ToString();
                }
                if (Item.ScheduleDate != null)
                {
                    schDate = DateTime.Parse(Item.ScheduleDate, new CultureInfo("en-GB"));
                    schDt = schDate.ToString("yyyy/MM/dd");
                }
                else
                {
                    schDt = DateTime.Today.ToString();
                }
                mirDt = DateTime.Today.ToString();
                projectDt = DateTime.Today.ToString();
                againstImportInvDt = DateTime.Today.ToString();

                Table.Rows.Add(
                    new object[]
                    {
                    EntryID ?? 0,
                    YearCode ?? 0,
                    Item.SeqNo,
                    0, // Item.Parentcode
                    Item.docTypeId,
                    Item.ItemCode ?? 0,
                    Item.Unit ?? string.Empty,
                    0f, // Item.NoOfCase
                    Item.BillQty > 0 ? Math.Round(Item.BillQty, 2, MidpointRounding.AwayFromZero) : 0,
                    Item.DPBQty > 0 ? Math.Round(Item.DPBQty, 2, MidpointRounding.AwayFromZero) : 0,
                    0f, // Item.RejectedQty
                    Item.AltQty > 0 ? Math.Round(Item.AltQty, 2, MidpointRounding.AwayFromZero) : 0,
                    Item.AltUnit ?? string.Empty,
                    Item.Rate > 0 ? Math.Round(Item.Rate, 2, MidpointRounding.AwayFromZero) : 0,
                    0f, // Item.MRP
                    Item.UnitRate ?? string.Empty,
                    0f, // Item.RateIncludingTaxes
                    Item.OtherRateCurr > 0 ? Math.Round(Item.OtherRateCurr, 2, MidpointRounding.AwayFromZero) : 0,
                    0f, // Item.RateConversionFactor
                    Item.CostCenter > 0 ? Item.CostCenter : 0,
                    0f, // Item.AssesRate
                    0f, // Item.AssesAmount
                    Item.DiscPer > 0 ? Math.Round(Item.DiscPer, 2, MidpointRounding.AwayFromZero) : 0,
                    Item.DiscRs > 0 ? Math.Round(Item.DiscRs, 2, MidpointRounding.AwayFromZero) : 0,
                    Item.Amount > 0 ? Math.Round(Item.Amount, 2, MidpointRounding.AwayFromZero) : 0,
                    string.Empty, // Item.Itemsize
                    Item.Color ?? string.Empty,
                    string.Empty, // Item.ItemModel
                    0, // Item.Deaprtmentid
                    Item.Description,
                    string.Empty, // Item.DebitNoteType
                    Item.Process > 0 ? Item.Process : 0,
                    0f, // Item.NewPoRate
                    Item.PONo ?? string.Empty,
                    Item.POYear ?? 0,
                    !string.IsNullOrEmpty(Item.PODate) ? poDate : poDt,
                    Item.ScheduleNo ?? string.Empty,
                    Item.ScheduleYear ?? 0,
                    !string.IsNullOrEmpty(Item.ScheduleDate) ? schDate : schDt,
                    string.Empty, // Item.POAmmNo
                    0f, // Item.PoRate
                    string.Empty, // Item.POType
                    string.Empty, // Item.MIRNO
                    0, // Item.MIRYearCode
                    mirDt, // Item.MIRDate
                    string.Empty, // Item.AllowDebitNote
                    string.Empty, // Item.DebitNotePending
                    string.Empty, // Item.ProjectNo
                    projectDt, // Item.ProjectDate
                    0, // Item.ProjectYearCode
                    0, // Item.AgainstImportAccountCode
                    string.Empty, // Item.AgainstImportInvoiceNo
                    0, // Item.AgainstImportYearCode
                    againstImportInvDt, // Item.AgainstImportInvDate
                    Item.HSNNo.ToString(),
                    Item.AcceptedQty,
                    Item.HoldQty,
                Item.ReworkQty
                    });
            }

            DS.Tables.Add(Table);
            return DS;
        }
        private static DataTable GetTDSDetailTable(List<TDSModel> TDSDetailList, DirectPurchaseBillModel MainModel)
        {
            DataTable Table = new();
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

            if (TDSDetailList != null && TDSDetailList.Count > 0)
            {
                foreach (TDSModel Item in TDSDetailList)
                {
                    DateTime InvoiceDate = new DateTime();
                    DateTime challanDate = new DateTime();
                    DateTime BankVoucherDate = new DateTime();
                    string InvoiceDt = "";
                    string challanDt = "";
                    string BankVoucherDt = "";
                    if (MainModel.InvDate != null)
                    {
                        InvoiceDate = DateTime.Parse(MainModel.InvDate, new CultureInfo("en-GB"));
                        InvoiceDt = InvoiceDate.ToString("dd/MMM/yyyy");
                    }
                    else
                    {
                        InvoiceDt = DateTime.Now.ToString("dd/MMM/yyyy");
                    }
                    challanDt = DateTime.Now.ToString("dd/MMM/yyyy");
                    BankVoucherDt = DateTime.Now.ToString("dd/MMM/yyyy");

                    Table.Rows.Add(
                        new object[]
                        {
                    MainModel.EntryID > 0 ? MainModel.EntryID : 0,
                    MainModel.YearCode > 0 ? MainModel.YearCode : 0,
                    Item.TDSSeqNo,
                    MainModel.InvoiceNo ?? string.Empty,
                    InvoiceDt,
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
                    "DirectPurchaseBill",
                    string.Empty,
                    challanDt,
                    string.Empty,
                    BankVoucherDt,
                    0,
                    0,
                    0f,
                    Item.TDSRoundOffAmt ?? 0,
                        });
                }
            }
            return Table;
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

                foreach (var group in groupedTaxDetails)
                {
                    var Item = group.FirstItem;
                    Table.Rows.Add(
                    new object[]
                    {
                    Item.TxSeqNo,
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
        

        private static DataTable GetDbCrDetailTable(DirectPurchaseBillModel MainModel)
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

            IList<DPBItemDetail> itemDetailList = MainModel.ItemDetailGrid;
            foreach (var Item in itemDetailList)
            {
                Table.Rows.Add(
                new object[]
                {
                    MainModel.EntryID,
                    MainModel.YearCode,
                    Item.SeqNo,
                    MainModel.InvoiceNo ?? string.Empty,
                    MainModel.PurchVouchNo ?? string.Empty,
                    string.Empty,
                    0,
                    MainModel.AccountCode,
                    Item.docTypeId,
                    Item.ItemCode,
                    Math.Round(Item.BillQty, 2, MidpointRounding.AwayFromZero),
                    Math.Round(Item.Rate, 2, MidpointRounding.AwayFromZero),
                    Math.Round(Item.DiscPer, 2, MidpointRounding.AwayFromZero),
                    Math.Round(Item.DiscRs, 2, MidpointRounding.AwayFromZero),
                    Math.Round(Item.Amount, 2, MidpointRounding.AwayFromZero),
                    "CR",
                    });
            }

            return Table;
        }
        public async Task<JsonResult> GetDbCrDataGrid()
        {
            IMemoryCache.TryGetValue("DirectPurchaseBill", out DirectPurchaseBillModel MainModel);
            IMemoryCache.TryGetValue("KeyTaxGrid", out List<TaxModel> TaxGrid);
            IMemoryCache.TryGetValue("KeyTDSGrid", out List<TDSModel> TdsGrid);
            DataTable DbCrGridd = new DataTable();
            DataTable TaxGridd = new DataTable();
            DataTable TdsGridd = new DataTable();

            DbCrGridd = GetDbCrDetailTable(MainModel);
            TaxGridd = GetTaxDetailTable(TaxGrid);
            TdsGridd = GetTDSDetailTable(TdsGrid, MainModel);

            var JSON = await IDataLogic.GetDbCrDataGrid(DbCrGridd, TaxGridd, TdsGridd, "DIRECTPURCHASEBILL", MainModel.docTypeId, MainModel.AccountCode, MainModel.ItemNetAmount, MainModel.NetTotal);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        private DirectPurchaseBillModel BindItem4Grid(DirectPurchaseBillModel model)
        {
            var _List = new List<DPBItemDetail>();

            IMemoryCache.TryGetValue("DirectPurchaseBill", out DirectPurchaseBillModel MainModel);
            //var SeqNo = 0;
            //if(MainModel == null)
            //{
            //    SeqNo++;
            //}
            _List.Add(
                new DPBItemDetail
                {
                    SeqNo = MainModel.ItemDetailGrid == null ? 1 : MainModel.ItemDetailGrid.Count + 1,
                    docTypeId = model.docTypeId,
                    DocTypeText = model.DocTypeText,
                    BillQty = model.BillQty,

                    Amount = model.Amount,
                    Description = model.Description,
                    DiscPer = model.DiscPer,
                    DiscRs = model.DiscRs,

                    HSNNo = model.HSNNo,
                    ItemCode = model.ItemCode,
                    ItemText = model.ItemText,

                    OtherRateCurr = model.OtherRateCurr,
                    PartCode = model.PartCode,
                    PartText = model.PartText,

                    DPBQty = model.DPBQty,
                    Process = model.Process,
                    ProcessName = model.ProcessName,
                    CostCenter = model.CostCenter,
                    CostCenterName = model.CostCenterName,
                    Rate = model.Rate,

                    PONo = model.PONo,
                    POYear = model.POYear,
                    PODate = model.PODate,
                    ScheduleNo = model.ScheduleNo,
                    ScheduleYear = model.ScheduleYear,
                    ScheduleDate = model.ScheduleDate,

                    Unit = model.Unit,
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
            var JSON = await IDirectPurchaseBill.AltUnitConversion(ItemCode, AltQty, UnitQty);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }

        [HttpGet]

        public async Task<ActionResult> ViewPOCompleted(string Mode, int ID, int YC, string PONO)
        {
            var model = new DirectPurchaseBillModel();
            MemoryCacheEntryOptions cacheEntryOptions = new()
            {
                AbsoluteExpiration = DateTime.Now.AddMinutes(55),
                SlidingExpiration = TimeSpan.FromMinutes(60),
                Size = 1024,
            };
            if (!string.IsNullOrEmpty(Mode) && ID > 0 && (Mode == "POC"))
            {
                model = await IDirectPurchaseBill.GetViewPOCcompletedByID(ID, YC, PONO, "VIEWPOCOMPLETEDBYID").ConfigureAwait(true);

                model.Mode = Mode;
                model = await BindModels(model);

                model.ID = ID;

                if (model.ItemDetailGrid?.Count != 0 && model.ItemDetailGrid != null)
                {
                    IMemoryCache.Set("DirectPurchaseBill", model.ItemDetailGrid, cacheEntryOptions);

                }

                if (model.TaxDetailGridd != null)
                {
                    IMemoryCache.Set("KeyTaxGrid", model.TaxDetailGridd, cacheEntryOptions);
                }
                if (model.TDSDetailGridd != null)
                {
                    IMemoryCache.Set("KeyTDSGrid", model.TDSDetailGridd, cacheEntryOptions);
                }
            }
            else
            {
                model = await BindModels(null);
                IMemoryCache.Remove("POTaxGrid");
                IMemoryCache.Remove("KeyTaxGrid");
                IMemoryCache.Remove("KeyTDSGrid");
                IMemoryCache.Remove("DirectPurchaseBill");
            }


            return View("DirectPurchaseBill", model);
        }

        
        public async Task<JsonResult> NewAmmEntryId()
        {
            var JSON = await IDirectPurchaseBill.NewAmmEntryId();
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
            List<DPBItemDetail> data = new List<DPBItemDetail>();

            using (var stream = excelFile.OpenReadStream())
            using (var package = new ExcelPackage(stream))
            {
                var worksheet = package.Workbook.Worksheets[0];
                int cnt = 1;
                for (int row = 2; row <= worksheet.Dimension.Rows; row++)
                {
                    //var itemCatCode = IStockAdjust.GetItemCatCode(worksheet.Cells[row, 6].Value.ToString());
                    var itemCode = IDirectPurchaseBill.GetItemCode(worksheet.Cells[row, 3].Value.ToString());
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
                    var DPBQty = Convert.ToDecimal(worksheet.Cells[row, 6].Value.ToString());


                    //for altunit conversion
                    var altUnitConversion = AltUnitConversion(partcode, 0, Convert.ToDecimal(worksheet.Cells[row, 6].Value.ToString()));
                    JObject AltUnitCon = JObject.Parse(altUnitConversion.Result.Value.ToString());
                    decimal altUnitValue = (decimal)AltUnitCon["Result"][0]["AltUnitValue"];


                    var GetExhange = GetExchangeRate(Currency);

                    JObject AltRate = JObject.Parse(GetExhange.Result.Value.ToString());
                    decimal AltRateToken = (decimal)AltRate["Result"][0]["Rate"];
                    var RateInOther = Convert.ToDecimal(worksheet.Cells[row, 14].Value) * AltRateToken;


                    var DisRs = DPBQty * Convert.ToDecimal(worksheet.Cells[row, 14].Value) * (Convert.ToDecimal(worksheet.Cells[row, 18].Value.ToString()) / 100);
                    var Amount = (DPBQty * Convert.ToDecimal(worksheet.Cells[row, 14].Value)) - DisRs;



                    data.Add(new DPBItemDetail()
                    {
                        SeqNo = cnt++,
                        PartText = worksheet.Cells[row, 3].Value.ToString(),
                        ItemText = worksheet.Cells[row, 4].Value.ToString(),
                        ItemCode = itemCodeValue,
                        PartCode = partcode,
                        HSNNo = Convert.ToInt32(worksheet.Cells[row, 5].Value.ToString()),
                        DPBQty = Convert.ToDecimal(worksheet.Cells[row, 6].Value.ToString()),
                        Unit = worksheet.Cells[row, 7].Value.ToString(),
                        AltQty = altUnitValue,
                        AltUnit = worksheet.Cells[row, 9].Value.ToString(),

                        AltPendQty = Convert.ToDecimal(worksheet.Cells[row, 11].Value.ToString()),
                        Process = Convert.ToInt32(worksheet.Cells[row, 13].Value.ToString()),
                        Rate = Convert.ToDecimal(worksheet.Cells[row, 14].Value.ToString()),
                        OtherRateCurr = RateInOther,
                        UnitRate = worksheet.Cells[row, 17].Value.ToString(),
                        DiscPer = Convert.ToDecimal(worksheet.Cells[row, 18].Value.ToString()),
                        DiscRs = Convert.ToDecimal(DisRs.ToString("F2")),
                        Amount = Convert.ToDecimal(Amount.ToString("F2")),
                        TxRemark = worksheet.Cells[row, 24].Value == null ? "" : worksheet.Cells[row, 24].Value.ToString(),
                        Description = worksheet.Cells[row, 25].Value == null ? "" : worksheet.Cells[row, 25].Value.ToString(),
                        AdditionalRate = Convert.ToDecimal(worksheet.Cells[row, 26].Value.ToString()),
                        Color = worksheet.Cells[row, 27].Value == null ? "" : worksheet.Cells[row, 27].Value.ToString(),
                        CostCenter = Convert.ToInt32(worksheet.Cells[row, 28].Value.ToString()),

                    });
                }
            }


            var MainModel = new DirectPurchaseBillModel();
            var POItemGrid = new List<DPBItemDetail>();
            var POGrid = new List<DPBItemDetail>();
            var SSGrid = new List<DPBItemDetail>();

            MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                SlidingExpiration = TimeSpan.FromMinutes(55),
                Size = 1024,
            };
            var seqNo = 0;
            IMemoryCache.Remove("DirectPurchaseBill");

            foreach (var item in data)
            {
                if (item != null)
                {
                    IMemoryCache.TryGetValue("DirectPurchaseBill", out DirectPurchaseBillModel Model);

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

                    IMemoryCache.Set("DirectPurchaseBill", MainModel, cacheEntryOptions);

                }
            }
            IMemoryCache.TryGetValue("DirectPurchaseBill", out DirectPurchaseBillModel MainModel1);

            return PartialView("_POItemGrid", MainModel);
        }



    }
}

