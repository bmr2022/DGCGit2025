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
using System.Runtime.Caching;
using DocumentFormat.OpenXml.Vml.Office;

namespace eTactWeb.Controllers
{

    public class DirectPurchaseBillController : Controller
    {
        private readonly IMemoryCacheService _iMemoryCacheService;
        private readonly IWebHostEnvironment _IWebHostEnvironment;
        private readonly IConfiguration iconfiguration;
        private readonly IMemoryCache _MemoryCache;
        private readonly ConnectionStringService _connectionStringService;
        public ILogger<DirectPurchaseBillModel> _Logger { get; set; }
        public CultureInfo CI { get; private set; }
        public EncryptDecrypt EncryptDecrypt { get; private set; }
        public IDataLogic IDataLogic { get; private set; }
        public IDirectPurchaseBill IDirectPurchaseBill { get; set; }

        public DirectPurchaseBillController(IDirectPurchaseBill iDirectPurchaseBill, IDataLogic iDataLogic, ILogger<DirectPurchaseBillModel> logger, EncryptDecrypt encryptDecrypt, IMemoryCacheService iMemoryCacheService, IWebHostEnvironment iWebHostEnvironment, IConfiguration configuration, IMemoryCache iMemoryCache, ConnectionStringService connectionStringService)
        {
            _iMemoryCacheService = iMemoryCacheService;
            IDirectPurchaseBill = iDirectPurchaseBill;
            IDataLogic = iDataLogic;
            _Logger = logger;
            EncryptDecrypt = encryptDecrypt;
            CI = new CultureInfo("en-GB");
            _IWebHostEnvironment = iWebHostEnvironment;
            iconfiguration = configuration;
            _MemoryCache = iMemoryCache;
            _connectionStringService = connectionStringService;
        }

        [HttpGet]
        public async Task<IActionResult> DirectPurchaseBill( int ID,  int YearCode, string Mode, string? TypeITEMSERVASSETS, string FromDate = "", string ToDate = "",  string DashboardType = "", string DocumentType = "", string VendorName = "", string PurchVouchNo = "", string InvoiceNo = "", string PartCode = "", string ItemName = "", string HSNNo = "", string Searchbox = "")
        {
            HttpContext.Session.Remove("KeyTaxGrid");
            HttpContext.Session.Remove("KeyTDSGrid");
            HttpContext.Session.Remove("DirectPurchaseBill");
            HttpContext.Session.Remove("KeyAdjGrid");
            var MainModel = new DirectPurchaseBillModel();
            MainModel.YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
            MainModel.FinFromDate = HttpContext.Session.GetString("FromDate");
            MainModel.FinToDate = HttpContext.Session.GetString("ToDate");
            MainModel.PreparedBy = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
             MainModel.PreparedByName = HttpContext.Session.GetString("EmpName");
            MainModel.Branch = HttpContext.Session.GetString("Branch");

            if (!string.IsNullOrEmpty(Mode) && ID > 0 && (Mode == "V" || Mode == "U"))
            {
                MainModel = await IDirectPurchaseBill.GetViewByID(ID, YearCode, "ViewByID").ConfigureAwait(false);
                MainModel.Mode = Mode;
                MainModel.TDSMode = Mode;
                MainModel.ID = ID;
                MainModel.YearCode = YearCode;
                MainModel = await BindModels(MainModel).ConfigureAwait(false);
                MainModel.FinFromDate = HttpContext.Session.GetString("FromDate");
                MainModel.FinToDate = HttpContext.Session.GetString("ToDate");
                string serializedGrid = JsonConvert.SerializeObject(MainModel);
                HttpContext.Session.SetString("DirectPurchaseBill", serializedGrid);
                var taxGrid = MainModel.TaxDetailGridd == null ? new List<TaxModel>() : MainModel.TaxDetailGridd;
                string serializedKeyTaxGrid = JsonConvert.SerializeObject(taxGrid);
                HttpContext.Session.SetString("KeyTaxGrid", serializedKeyTaxGrid);
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

            string serializedDirectPurchaseBill = JsonConvert.SerializeObject(MainModel);
            HttpContext.Session.SetString("DirectPurchaseBill", serializedDirectPurchaseBill);

            var taxGridList = MainModel.TaxDetailGridd == null ? new List<TaxModel>() : MainModel.TaxDetailGridd;
            string serializedTaxGrid = JsonConvert.SerializeObject(taxGridList);
            HttpContext.Session.SetString("KeyTaxGrid", serializedTaxGrid);

            var tdsGridList = MainModel.TDSDetailGridd == null ? new List<TDSModel>() : MainModel.TDSDetailGridd;
            string serializedTDSGrid = JsonConvert.SerializeObject(tdsGridList);
            HttpContext.Session.SetString("KeyTDSGrid", serializedTDSGrid);

            var adjGrid = MainModel.adjustmentModel == null ? new AdjustmentModel() : MainModel.adjustmentModel;
            string serializedAdjGrid = JsonConvert.SerializeObject(adjGrid);
            HttpContext.Session.SetString("KeyAdjGrid", serializedAdjGrid);

            HttpContext.Session.SetString("DirectPurchaseBill", JsonConvert.SerializeObject(MainModel));
            MainModel.adjustmentModel = (MainModel.adjustmentModel != null && MainModel.adjustmentModel.AdjAdjustmentDetailGrid != null) ? MainModel.adjustmentModel : new AdjustmentModel();
            return View(MainModel);

        }
        public async Task<JsonResult> GetFeatureOption()
        {
            var JSON = await IDirectPurchaseBill.GetFeatureOption();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }

        [HttpPost]
        public async Task<IActionResult> DirectPurchaseBill(DirectPurchaseBillModel model)
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

                // 1. Get DirectPurchaseBill
                string directPurchaseBillJson = HttpContext.Session.GetString("DirectPurchaseBill");
                DirectPurchaseBillModel MainModel = string.IsNullOrEmpty(directPurchaseBillJson)
                    ? new DirectPurchaseBillModel()
                    : JsonConvert.DeserializeObject<DirectPurchaseBillModel>(directPurchaseBillJson);

                // 2. Get TaxGrid
                string taxGridJson = HttpContext.Session.GetString("KeyTaxGrid");
                List<TaxModel> TaxGrid = string.IsNullOrEmpty(taxGridJson)
                    ? new List<TaxModel>()
                    : JsonConvert.DeserializeObject<List<TaxModel>>(taxGridJson);

                // 3. Get TdsGrid
                string tdsGridJson = HttpContext.Session.GetString("KeyTDSGrid");
                List<TDSModel> TdsGrid = string.IsNullOrEmpty(tdsGridJson)
                    ? new List<TDSModel>()
                    : JsonConvert.DeserializeObject<List<TDSModel>>(tdsGridJson);

                // 4. Get DrCrGrid
                string drCrGridJson = HttpContext.Session.GetString("KeyDrCrGrid");
                List<DbCrModel> DrCrGrid = string.IsNullOrEmpty(drCrGridJson)
                    ? new List<DbCrModel>()
                    : JsonConvert.DeserializeObject<List<DbCrModel>>(drCrGridJson);

                string serializedGrid = HttpContext.Session.GetString("KeyAdjGrid");
                AdjustmentModel adjustmentModel = JsonConvert.DeserializeObject<AdjustmentModel>(serializedGrid);
                List<AdjustmentModel> gridData = adjustmentModel.AdjAdjustmentDetailGrid;


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

                if (gridData != null && gridData.Count > 0)
                {

                    AdjDetailDT = CommonController.GetAdjDetailTable(gridData, MainModel.EntryID, MainModel.YearCode, MainModel.AccountCode);
                    //AdjDetailDT = CommonController.GetAdjDetailTable(MainModel.adjustmentModel.AdjAdjustmentDetailGrid.ToList(), model.EntryID, model.YearCode, model.AccountCode);
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
                            string extension = Path.GetExtension(model.PathOfFile1.FileName)?.ToLowerInvariant();
                            string safePurchVouchNo = model.PurchVouchNo.Replace("\\", "_").Replace("/", "_");
                            string safeInvNo = model.InvoiceNo.Replace("\\", "_").Replace("/", "_");
                            ImagePath += safePurchVouchNo + "_" + model.YearCode + "_" + safeInvNo + "_" + model.VouchDate.Replace("\\", "_").Replace("/", "_") + "_" + Guid.NewGuid().ToString() + extension;
                            model.PathOfFile1URL = "/" + ImagePath;
                            //ImagePath += Guid.NewGuid().ToString() + "_" + model.PathOfFile1.FileName;
                            //model.PathOfFile1URL = "/" + ImagePath;
                            string ServerPath = Path.Combine(_IWebHostEnvironment.WebRootPath, ImagePath);
                            using (FileStream FileStream = new FileStream(ServerPath, FileMode.Create))
                            {
                                await model.PathOfFile1.CopyToAsync(FileStream);
                            }
                        }
                        else
                        {
                            model.PathOfFile1URL = MainModel.PathOfFile1URL;
                        }

                        if (model.PathOfFile2 != null)
                        {
                            string ImagePath = "Uploads/DirectPurchaseBill/";

                            if (!Directory.Exists(_IWebHostEnvironment.WebRootPath + "\\" + ImagePath))
                            {
                                Directory.CreateDirectory(_IWebHostEnvironment.WebRootPath + "\\" + ImagePath);
                            }
                            string extension = Path.GetExtension(model.PathOfFile2.FileName)?.ToLowerInvariant();
                            string safePurchVouchNo = model.PurchVouchNo.Replace("\\", "_").Replace("/", "_");
                            string safeInvNo = model.InvoiceNo.Replace("\\", "_").Replace("/", "_");
                            ImagePath += safePurchVouchNo + "_" + model.YearCode + "_" + safeInvNo + "_" + model.VouchDate.Replace("\\", "_").Replace("/", "_") + "_" + "2" + "_" + Guid.NewGuid().ToString() + extension;
                            model.PathOfFile2URL = "/" + ImagePath;
                            //ImagePath += Guid.NewGuid().ToString() + "_" + model.PathOfFile2.FileName;
                            //model.PathOfFile2URL = "/" + ImagePath;
                            string ServerPath = Path.Combine(_IWebHostEnvironment.WebRootPath, ImagePath);
                            using (FileStream FileStream = new FileStream(ServerPath, FileMode.Create))
                            {
                                await model.PathOfFile2.CopyToAsync(FileStream);
                            }
                        }

                        else
                        {
                            model.PathOfFile2URL = MainModel.PathOfFile2URL;
                        }

                        if (model.PathOfFile3 != null)
                        {
                            string ImagePath = "Uploads/DirectPurchaseBill/";

                            if (!Directory.Exists(_IWebHostEnvironment.WebRootPath + "\\" + ImagePath))
                            {
                                Directory.CreateDirectory(_IWebHostEnvironment.WebRootPath + "\\" + ImagePath);
                            }
                            string extension = Path.GetExtension(model.PathOfFile3.FileName)?.ToLowerInvariant();
                            string safePurchVouchNo = model.PurchVouchNo.Replace("\\", "_").Replace("/", "_");
                            string safeInvNo = model.InvoiceNo.Replace("\\", "_").Replace("/", "_");
                            ImagePath += safePurchVouchNo + "_" + model.YearCode + "_" + safeInvNo + "_" + model.VouchDate.Replace("\\", "_").Replace("/", "_") + "_" + "3" + "_" + Guid.NewGuid().ToString() + extension;
                            model.PathOfFile3URL = "/" + ImagePath;
                            //ImagePath += Guid.NewGuid().ToString() + "_" + model.PathOfFile3.FileName;
                            //model.PathOfFile3URL = "/" + ImagePath;
                            string ServerPath = Path.Combine(_IWebHostEnvironment.WebRootPath, ImagePath);
                            using (FileStream FileStream = new FileStream(ServerPath, FileMode.Create))
                            {
                                await model.PathOfFile3.CopyToAsync(FileStream);
                            }
                        }
                        else
                        {
                            model.PathOfFile3URL = MainModel.PathOfFile3URL;
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
                            HttpContext.Session.Remove("KeyTaxGrid");
                            HttpContext.Session.Remove("KeyTDSGrid");
                            HttpContext.Session.Remove("DirectPurchaseBill");
                        }
                        else if (Result.StatusText == "Inserted Successfully" && Result.StatusCode == HttpStatusCode.Accepted)
                        {
                            ViewBag.isSuccess = true;
                            TempData["200"] = "200";
                            HttpContext.Session.Remove("KeyTaxGrid");
                            HttpContext.Session.Remove("KeyTDSGrid");
                            HttpContext.Session.Remove("DirectPurchaseBill");
                        }
                        else if (Result.StatusText == "Updated Successfully" && Result.StatusCode == HttpStatusCode.Accepted)
                        {
                            ViewBag.isSuccess = true;
                            TempData["202"] = "202";
                            HttpContext.Session.Remove("KeyTaxGrid");
                            HttpContext.Session.Remove("KeyTDSGrid");
                            HttpContext.Session.Remove("DirectPurchaseBill");
                            return RedirectToAction(nameof(DirectPurchaseBill));
                        }
                        else if (Result.StatusText == "Deleted Successfully" && Result.StatusCode == HttpStatusCode.Accepted)
                        {
                            ViewBag.isSuccess = true;
                            TempData["410"] = "410";
                            HttpContext.Session.Remove("KeyTaxGrid");
                            HttpContext.Session.Remove("KeyTDSGrid");
                            HttpContext.Session.Remove("DirectPurchaseBill");
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
                                HttpContext.Session.Remove("KeyTaxGrid");
                                HttpContext.Session.Remove("KeyTDSGrid");
                                HttpContext.Session.Remove("DirectPurchaseBill");
                                return RedirectToAction(nameof(DirectPurchaseBill));
                            }
                            else
                            {
                                TempData["ErrorMessage"] = Result.StatusText;
                                HttpContext.Session.Remove("KeyTaxGrid");
                                HttpContext.Session.Remove("KeyTDSGrid");
                                HttpContext.Session.Remove("DirectPurchaseBill");
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
        public async Task<JsonResult> CheckDuplicateEntry(int YearCode, int AccountCode, string InvNo, int EntryId)
        {
            var JSON = await IDirectPurchaseBill.CheckDuplicateEntry(YearCode, AccountCode, InvNo, EntryId);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public IActionResult PrintReport(int EntryId = 0, int YearCode = 0, string PONO = "")
        {
            string my_connection_string;
            string contentRootPath = _IWebHostEnvironment.ContentRootPath;
            string webRootPath = _IWebHostEnvironment.WebRootPath;
            //string frx = Path.Combine(_env.ContentRootPath, "reports", value.file);
            var webReport = new WebReport();

           
           webReport.Report.Load(webRootPath + "\\DirectPurchaseBillReport.frx"); // default report

            webReport.Report.SetParameterValue("entryparam", EntryId);
            webReport.Report.SetParameterValue("yearparam", YearCode);
            //webReport.Report.SetParameterValue("ponoparam", PONO);

            my_connection_string = _connectionStringService.GetConnectionString();
            //my_connection_string = iconfiguration.GetConnectionString("eTactDB");
            webReport.Report.Dictionary.Connections[0].ConnectionString = my_connection_string;
            webReport.Report.Dictionary.Connections[0].ConnectionStringExpression = "";
            webReport.Report.SetParameterValue("MyParameter", my_connection_string);
            webReport.Report.Refresh();
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

            // 1. Get DirectPurchaseBill
            string mainModelJson = HttpContext.Session.GetString("DirectPurchaseBill");
            DirectPurchaseBillModel MainModel = string.IsNullOrEmpty(mainModelJson)
                ? new DirectPurchaseBillModel()
                : JsonConvert.DeserializeObject<DirectPurchaseBillModel>(mainModelJson);

            // 2. Get Tax Details
            //string taxDetailJson = HttpContext.Session.GetString("KeyTaxGrid");
            //IList<TaxModel> DPBTaxdetail = string.IsNullOrEmpty(taxDetailJson)
            //    ? new List<TaxModel>()
            //    : JsonConvert.DeserializeObject<IList<TaxModel>>(taxDetailJson);

            //// 3. Get TDS Details
            //string tdsDetailJson = HttpContext.Session.GetString("KeyTDSGrid");
            //IList<TDSModel> DPBTDSdetail = string.IsNullOrEmpty(tdsDetailJson)
            //    ? new List<TDSModel>()
            //    : JsonConvert.DeserializeObject<IList<TDSModel>>(tdsDetailJson);

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
                HttpContext.Session.Remove("DirectPurchaseBill");
                string serializedModel = JsonConvert.SerializeObject(model);
                HttpContext.Session.SetString("DirectPurchaseBill", serializedModel);
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
        }    public async Task<JsonResult> GetDocTypeId(string Dooctype)
        {
            var JSON = await IDirectPurchaseBill.GetDocTypeId(Dooctype);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        } 
        public async Task<JsonResult> GetItemDetail(string PartCode)
        {
            var JSON = await IDirectPurchaseBill.GetItemDetail(PartCode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }

        public async Task<IActionResult> GetItemServiceFORPO(string ItemService)
        {
            var JSONString = await IDirectPurchaseBill.GetItemServiceFORPO(ItemService);

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
            HttpContext.Session.Remove("TaxGrid");
            HttpContext.Session.Remove("KeyTaxGrid");
            var _List = new List<TextValue>();
            var MainModel = await IDirectPurchaseBill.GetDashBoardData();
            MainModel.FromDate = new DateTime(DateTime.Today.Year, 4, 1).ToString("dd/MM/yyyy").Replace("-", "/");
            MainModel.ToDate = new DateTime(DateTime.Today.Year + 1, 3, 31).ToString("dd/MM/yyyy").Replace("-", "/");// Last day in January next year

            return View(MainModel);
        }
        public IActionResult ClearDRCRGrid()
        {
            HttpContext.Session.Remove("KeyDrCrGrid");
            return Json("Ok");
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
            return Json(new { success = rslt, message = Result.StatusText });

            //return Json(rslt);
            //return RedirectToAction(nameof(DashBoard));
        }
        public async Task<JsonResult> CheckEditOrDelete(int EntryId, int YearCode)
        {
           
            var Result = await IDirectPurchaseBill.CheckEditOrDelete(EntryId, YearCode);
             var rslt = string.Empty;
            return Json(new { success = rslt, message = Result.StatusText });

            //return Json(rslt);
            //return RedirectToAction(nameof(DashBoard));
        }
        public IActionResult DeleteItemRow(string SeqNo)
        {
            bool exists = false;

            // 1. Get TDSGrid
            string tdsGridJson = HttpContext.Session.GetString("KeyTDSGrid");
            List<TDSModel> TDSGrid = string.IsNullOrEmpty(tdsGridJson)
                ? new List<TDSModel>()
                : JsonConvert.DeserializeObject<List<TDSModel>>(tdsGridJson);

            // 2. Get TaxGrid
            string taxGridJson = HttpContext.Session.GetString("KeyTaxGrid");
            List<TaxModel> TaxGrid = string.IsNullOrEmpty(taxGridJson)
                ? new List<TaxModel>()
                : JsonConvert.DeserializeObject<List<TaxModel>>(taxGridJson);

            // 3. Get DirectPurchaseBill
            string mainModelJson = HttpContext.Session.GetString("DirectPurchaseBill");
            DirectPurchaseBillModel MainModel = string.IsNullOrEmpty(mainModelJson)
                ? new DirectPurchaseBillModel()
                : JsonConvert.DeserializeObject<DirectPurchaseBillModel>(mainModelJson);

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

                string serializedMainModel = JsonConvert.SerializeObject(MainModel);
                HttpContext.Session.SetString("DirectPurchaseBill", serializedMainModel);
            }
            return PartialView("_DPBItemGrid", MainModel);
        }

        public IActionResult EditItemRow(DirectPurchaseBillModel model)
        {
            // Read JSON string from session
            string mainModelJson = HttpContext.Session.GetString("DirectPurchaseBill");

            // Deserialize back into object
            DirectPurchaseBillModel MainModel = string.IsNullOrEmpty(mainModelJson)
                ? new DirectPurchaseBillModel()  // if not found, create new
                : JsonConvert.DeserializeObject<DirectPurchaseBillModel>(mainModelJson);
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
            HttpContext.Session.Remove("KeyTaxGrid");
            var JSON = await IDirectPurchaseBill.FillEntryandVouchNoNumber(YearCode, VODate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> ClearTDSGrid(int YearCode, string VODate)
        {
            HttpContext.Session.Remove("KeyTDSGrid");
            var JSON = await IDirectPurchaseBill.FillEntryandVouchNoNumber(YearCode, VODate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> ClearItemGrid(int YearCode, string VODate)
        {
            HttpContext.Session.Remove("DirectPurchaseBill");
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

        public async Task<IActionResult> GetSearchData(DPBDashBoard model, int pageNumber = 1, int pageSize = 25, string SearchBox = "")
        {
            model.Mode = "SEARCH";
            model = await IDirectPurchaseBill.GetSummaryData(model);
            model.DashboardType = "Summary";
            var modelList = model?.DPBDashboard ?? new List<DPBDashBoard>();


            if (string.IsNullOrWhiteSpace(SearchBox))
            {
                model.TotalRecords = modelList.Count();
                model.PageNumber = pageNumber;
                model.PageSize = pageSize;
                model.DPBDashboard = modelList
                .Skip((pageNumber - 1) * pageSize)
                   .Take(pageSize)
                   .ToList();
            }
            else
            {
                List<DPBDashBoard> filteredResults;
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
                model.DPBDashboard = filteredResults.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
                model.PageNumber = pageNumber;
                model.PageSize = pageSize;
            }
            MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                SlidingExpiration = TimeSpan.FromMinutes(55),
                Size = 1024,
            };

            _MemoryCache.Set("KeyDirectPurchaseBillList_Summary", modelList, cacheEntryOptions);
            return PartialView("_DashBoardGrid", model);
        }
        public async Task<IActionResult> GetDetailData(DPBDashBoard model, int pageNumber = 1, int pageSize = 25, string SearchBox = "")
        {
            model.Mode = "SEARCH";
            var type = model.DashboardType;
            model = await IDirectPurchaseBill.GetDetailData(model);
            model.DashboardType = type;
            var modelList = model?.DPBDashboard ?? new List<DPBDashBoard>();


            if (string.IsNullOrWhiteSpace(SearchBox))
            {
                model.TotalRecords = modelList.Count();
                model.PageNumber = pageNumber;
                model.PageSize = pageSize;
                model.DPBDashboard = modelList
                .Skip((pageNumber - 1) * pageSize)
                   .Take(pageSize)
                   .ToList();
            }
            else
            {
                List<DPBDashBoard> filteredResults;
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
                model.DPBDashboard = filteredResults.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
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
                _MemoryCache.Set("KeyDirectPurchaseBillList_TAXDetail", modelList, cacheEntryOptions);

            }
            else
            {

                _MemoryCache.Set("KeyDirectPurchaseBillList_Detail", modelList, cacheEntryOptions);
            }
            return PartialView("_DashBoardGrid", model);
        }
        [HttpGet]
        public IActionResult GlobalSearch(string searchString, string dashboardType = "Summary", int pageNumber = 1, int pageSize = 50)
        {
            DPBDashBoard model = new DPBDashBoard();
            if (string.IsNullOrWhiteSpace(searchString))
            {
                return PartialView("_DashBoardGrid", new List<DPBDashBoard>());
            }
            string cacheKey = $"KeyDirectPurchaseBillList_{dashboardType}";
            if (!_MemoryCache.TryGetValue(cacheKey, out IList<DPBDashBoard> dPBDashBoard) || dPBDashBoard == null)
            {
                return PartialView("_DashBoardGrid", new List<DPBDashBoard>());
            }

            List<DPBDashBoard> filteredResults;

            if (string.IsNullOrWhiteSpace(searchString))
            {
                filteredResults = dPBDashBoard.ToList();
            }
            else
            {
                filteredResults = dPBDashBoard
                    .Where(i => i.GetType().GetProperties()
                        .Where(p => p.PropertyType == typeof(string))
                        .Select(p => p.GetValue(i)?.ToString())
                        .Any(value => !string.IsNullOrEmpty(value) &&
                                      value.Contains(searchString, StringComparison.OrdinalIgnoreCase)))
                    .ToList();


                if (filteredResults.Count == 0)
                {
                    filteredResults = dPBDashBoard.ToList();
                }
            }

            model.TotalRecords = filteredResults.Count;
            model.DPBDashboard = filteredResults.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
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
        public async Task<IActionResult> GetAmmCompSearchData(DPBDashBoard model)
        {
            model = await IDirectPurchaseBill.GetSearchCompData(model);
            model.Mode = "Completed";
            return PartialView("_AmmListGrid", model);
        }

        public JsonResult ResetGridItems()
        {
            HttpContext.Session.Remove("POItemList");
            HttpContext.Session.Remove("DirectPurchaseBill");
            HttpContext.Session.Remove("KeyTaxGrid");
            HttpContext.Session.Remove("KeyTDSGrid");

            var MainModel = new DirectPurchaseBillModel();
            List<TaxModel> taxList = new List<TaxModel>();
            List<TDSModel> tdsList = new List<TDSModel>();

            MainModel.YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
            MainModel.FinFromDate = HttpContext.Session.GetString("FromDate");
            MainModel.FinToDate = HttpContext.Session.GetString("ToDate");
            MainModel.PreparedBy = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
            MainModel.PreparedByName = HttpContext.Session.GetString("EmpName");
            MainModel.Branch = HttpContext.Session.GetString("Branch");

            // 1. Serialize the objects
            string serializedMainModel = JsonConvert.SerializeObject(MainModel);
            string serializedTaxList = JsonConvert.SerializeObject(taxList);
            string serializedTdsList = JsonConvert.SerializeObject(tdsList);

            // 2. Store in Session
            HttpContext.Session.SetString("DirectPurchaseBill", serializedMainModel);
            HttpContext.Session.SetString("KeyTaxGrid", serializedTaxList);
            HttpContext.Session.SetString("KeyTDSGrid", serializedTdsList);

            HttpContext.Session.SetString("DirectPurchaseBill", JsonConvert.SerializeObject(MainModel));
            List<TaxModel> TaxGrid;
            List<TDSModel> TdsGrid;
            // 1. Get DirectPurchaseBill from Session
            string mainModelJson = HttpContext.Session.GetString("DirectPurchaseBill");
            MainModel = string.IsNullOrEmpty(mainModelJson)
                ? new DirectPurchaseBillModel()  // Default if not found
                : JsonConvert.DeserializeObject<DirectPurchaseBillModel>(mainModelJson);

            // 2. Get KeyTaxGrid from Session
            string taxGridJson = HttpContext.Session.GetString("KeyTaxGrid");
            TaxGrid = string.IsNullOrEmpty(taxGridJson)
                ? new List<TaxModel>()  // Default if not found
                : JsonConvert.DeserializeObject<List<TaxModel>>(taxGridJson);

            // 3. Get KeyTDSGrid from Session
            string tdsGridJson = HttpContext.Session.GetString("KeyTDSGrid");
            TdsGrid = string.IsNullOrEmpty(tdsGridJson)
                ? new List<TDSModel>()  // Default if not found
                : JsonConvert.DeserializeObject<List<TDSModel>>(tdsGridJson);


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
            Table.Columns.Add("PODate", typeof(string));
            Table.Columns.Add("SchNo", typeof(string));
            Table.Columns.Add("SchYearCode", typeof(int));
            Table.Columns.Add("SchDate", typeof(string));
            Table.Columns.Add("POAmmNo", typeof(string));
            Table.Columns.Add("PoRate", typeof(float));
            Table.Columns.Add("POType", typeof(string));
            Table.Columns.Add("MIRNO", typeof(string));
            Table.Columns.Add("MIRYearCode", typeof(int));
            Table.Columns.Add("MIREntryId", typeof(int));
            Table.Columns.Add("MIRDate", typeof(string));
            Table.Columns.Add("AllowDebitNote", typeof(string));
            Table.Columns.Add("DebitNotePending", typeof(string));
            Table.Columns.Add("ProjectNo", typeof(string));
            Table.Columns.Add("ProjectDate", typeof(string));
            Table.Columns.Add("ProjectYearCode", typeof(int));
            Table.Columns.Add("AgainstImportAccountCode", typeof(int));
            Table.Columns.Add("AgainstImportInvoiceNo", typeof(string));
            Table.Columns.Add("AgainstImportYearCode", typeof(int));
            Table.Columns.Add("AgainstImportInvDate", typeof(string));
            Table.Columns.Add("HSNNO", typeof(string));
           
            Table.Columns.Add("AcceptedQty", typeof(float));
            Table.Columns.Add("ReworkQty", typeof(float));
            Table.Columns.Add("HoldQty", typeof(float));
            Table.Columns.Add("ItemLocation", typeof(string));

            foreach (DPBItemDetail Item in itemDetailList)
            {
               
                string poDt = "";
                string schDt = "";
                string mirDt = "";
                string projectDt = "";
                string againstImportInvDt = "";
               
                    poDt = CommonFunc.ParseFormattedDate(Item.PODate);




                schDt = CommonFunc.ParseFormattedDate(Item.ScheduleDate);
                   
               
               
                mirDt = CommonFunc.ParseFormattedDate(DateTime.Today.ToString());
                projectDt = CommonFunc.ParseFormattedDate(DateTime.Today.ToString());
                againstImportInvDt = CommonFunc.ParseFormattedDate(DateTime.Today.ToString());

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
                    Item.Description?? string.Empty,
                    string.Empty, // Item.DebitNoteType
                    Item.Process > 0 ? Item.Process : 0,
                    0f, // Item.NewPoRate
                    Item.PONo ?? string.Empty,
                    Item.POYear ?? 0,
                    poDt ?? "",
                    Item.ScheduleNo ?? string.Empty,
                    Item.ScheduleYear ?? 0,
                   schDt ?? "",
                    string.Empty, // Item.POAmmNo
                    0f, // Item.PoRate
                    string.Empty, // Item.POType
                    string.Empty, // Item.MIRNO
                    0, // Item.MIRYearCode
                    0,
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
                Item.ReworkQty,
                Item.ItemLocation ?? string.Empty
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
            Table.Columns.Add("InvoiceDate", typeof(string));
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
            Table.Columns.Add("challanDate", typeof(string));
            Table.Columns.Add("BankVoucherNo", typeof(string));
            Table.Columns.Add("BankVoucherDate", typeof(string));
            Table.Columns.Add("BankVouchEntryId", typeof(int));
            Table.Columns.Add("BankYearCode", typeof(int));
            Table.Columns.Add("RemainingAmt", typeof(float));
            Table.Columns.Add("RoundoffAmt", typeof(float));

            if (TDSDetailList != null && TDSDetailList.Count > 0)
            {
                foreach (TDSModel Item in TDSDetailList)
                {
                    //DateTime InvoiceDate = new DateTime();
                    //DateTime challanDate = new DateTime();
                    //DateTime BankVoucherDate = new DateTime();
                    //string InvoiceDt = "";
                    //string challanDt = "";
                    //string BankVoucherDt = "";

                    
                    string InvoiceDt = "";
                    string challanDt = "";
                    string BankVoucherDt = "";

                    #region Formats
                    string[] formats = {
                    "dd-MM-yyyy HH:mm:ss",
                    "dd/MM/yyyy HH:mm:ss",
                    "yyyy-MM-dd HH:mm:ss",
                    "MM/dd/yyyy HH:mm:ss",
                    "dd-MM-yyyy",
                    "dd/MM/yyyy",
                    "yyyy-MM-dd",
                    "MM/dd/yyyy"
                };
                    #endregion


                    InvoiceDt= CommonFunc.ParseFormattedDate(MainModel.InvDate);
                        //DateTime.TryParse(MainModel.InvDate, CultureInfo.InvariantCulture, out InvoiceDate);
                       
                  
                    challanDt =CommonFunc.ParseFormattedDate( DateTime.Today.ToString());
                    BankVoucherDt = CommonFunc.ParseFormattedDate(DateTime.Today.ToString());

                    Table.Rows.Add(
                        new object[]
                        {
                    MainModel.EntryID > 0 ? MainModel.EntryID : 0,
                    MainModel.YearCode > 0 ? MainModel.YearCode : 0,
                    Item.TDSSeqNo,
                    MainModel.InvoiceNo ?? string.Empty,
                    InvoiceDt ?? "",
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
            .GroupBy(item => new { item.TxItemCode, item.TxTaxType, item.TxAccountCode })
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
            // 1. Get "DirectPurchaseBill" from Session
            string mainModelJson = HttpContext.Session.GetString("DirectPurchaseBill");
            DirectPurchaseBillModel MainModel = string.IsNullOrEmpty(mainModelJson)
                ? new DirectPurchaseBillModel()  // Default if not found
                : JsonConvert.DeserializeObject<DirectPurchaseBillModel>(mainModelJson);

            // 2. Get "KeyTaxGrid" from Session
            string taxGridJson = HttpContext.Session.GetString("KeyTaxGrid");
            List<TaxModel> TaxGrid = string.IsNullOrEmpty(taxGridJson)
                ? new List<TaxModel>()  // Default if not found
                : JsonConvert.DeserializeObject<List<TaxModel>>(taxGridJson);

            // 3. Get "KeyTDSGrid" from Session
            string tdsGridJson = HttpContext.Session.GetString("KeyTDSGrid");
            List<TDSModel> TdsGrid = string.IsNullOrEmpty(tdsGridJson)
                ? new List<TDSModel>()  // Default if not found
                : JsonConvert.DeserializeObject<List<TDSModel>>(tdsGridJson);

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

            // Retrieve DirectPurchaseBill from Session
            string mainModelJson = HttpContext.Session.GetString("DirectPurchaseBill");

            // Deserialize it back to DirectPurchaseBillModel
            DirectPurchaseBillModel MainModel = string.IsNullOrEmpty(mainModelJson)
                ? new DirectPurchaseBillModel()  // If not found, return new object
                : JsonConvert.DeserializeObject<DirectPurchaseBillModel>(mainModelJson);

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
                    ItemLocation = model.ItemLocation,

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
                    // 1. Serialize model.ItemDetailGrid to a JSON string
                    string serializedItemDetailGrid = JsonConvert.SerializeObject(model.ItemDetailGrid);

                    // 2. Store the serialized string in Session
                    HttpContext.Session.SetString("DirectPurchaseBill", serializedItemDetailGrid);
                }

                if (model.TaxDetailGridd != null)
                {
                    // 1. Serialize model.TaxDetailGridd to a JSON string
                    string serializedTaxGrid = JsonConvert.SerializeObject(model.TaxDetailGridd);

                    // 2. Store the serialized string in Session
                    HttpContext.Session.SetString("KeyTaxGrid", serializedTaxGrid);
                }
                if (model.TDSDetailGridd != null)
                {
                    // 1. Serialize model.TDSDetailGridd to a JSON string
                    string serializedTDSGrid = JsonConvert.SerializeObject(model.TDSDetailGridd);

                    // 2. Store the serialized string in Session
                    HttpContext.Session.SetString("KeyTDSGrid", serializedTDSGrid);

                }
            }
            else
            {
                model = await BindModels(null);
                HttpContext.Session.Remove("POTaxGrid");
                HttpContext.Session.Remove("KeyTaxGrid");
                HttpContext.Session.Remove("KeyTDSGrid");
                HttpContext.Session.Remove("DirectPurchaseBill");
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
            string pono = Request.Form["PoNo"];
            int poYearcode = Convert.ToInt32(Request.Form["POYearcode"]);
            int AccountCode = Convert.ToInt32(Request.Form["AccountCode"]);
            string SchNo = Request.Form["SchNo"];
            int SchYearCode = Convert.ToInt32(Request.Form["SchYearCode"]);
            string Currency = Request.Form["Currency"];
            string Flag = Request.Form["Flag"];

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            List<DPBItemDetail> data = new List<DPBItemDetail>();

            using (var stream = excelFile.OpenReadStream())
            using (var package = new ExcelPackage(stream))
            {
                var worksheet = package.Workbook.Worksheets[0];
                int cnt = 1;

                for (int row = 2; row <= worksheet.Dimension.Rows; row++)
                {
                    // Read cell values safely
                    var itemName = worksheet.Cells[row, 2].Value?.ToString()?.Trim();
                    var rateValue = worksheet.Cells[row, 3].Value?.ToString();
                    var qtyValue = worksheet.Cells[row, 4].Value?.ToString();
                    var docTypeText = worksheet.Cells[row, 7].Value?.ToString();
                    var locationValue = worksheet.Cells[row, 6].Value?.ToString();

                    // 🔹 Basic Required Field Validation
                 
                    if (string.IsNullOrEmpty(qtyValue) || !decimal.TryParse(qtyValue, out decimal qty) || qty <= 0)
                        return BadRequest($"Row {row}: Valid Quantity is required.");

                    if (string.IsNullOrEmpty(docTypeText))
                        return BadRequest($"Row {row}: Document Type is required.");

                    // 🔹 Fetch Item Code from DB
                    var itemCodeResult = IDirectPurchaseBill.GetItemCode(worksheet.Cells[row, 1].Value.ToString());
                    int partcode = 0;
                    int itemCodeValue = 0;

                    if (itemCodeResult.Result.Result != null)
                    {
                        partcode = itemCodeResult.Result.Result.Rows.Count <= 0 ? 0 : (int)itemCodeResult.Result.Result.Rows[0].ItemArray[0];
                        itemCodeValue = partcode;
                    }

                    // 🔹 Validate PartCode
                    if (partcode == 0)
                        return BadRequest($"Row {row}: Invalid Part Code or Item not found.");

                    // Continue your original logic...
                    var GetExchange = GetExchangeRate(Currency);
                    var GetDocTypeId1 = GetDocTypeId(docTypeText);
                    var GetItem = GetItemDetail(worksheet.Cells[row, 1].Value.ToString());

                    JObject Jsonstring = JObject.Parse(GetItem.Result.Value.ToString());
                    var Unit = Jsonstring["Result"][0]["Unit"];
                    var HsnNo = Jsonstring["Result"][0]["HsnNo"];
                    var AlternateUnit = Jsonstring["Result"][0]["AlternateUnit"];
                    var Rackid = Jsonstring["Result"][0]["Rackid"]?.ToString();
                    var purchaseprice = Jsonstring["Result"][0]["purchaseprice"].ToString();
                    var item_name = Jsonstring["Result"][0]["item_name"].ToString();


                    string location = !string.IsNullOrWhiteSpace(locationValue) ? locationValue: (!string.IsNullOrWhiteSpace(Rackid) ? Rackid : null);

                    if (string.IsNullOrWhiteSpace(location))
                        return BadRequest($"Row {row}: Location is required.");

                    if (string.IsNullOrWhiteSpace(itemName))  itemName = item_name;
                    if (string.IsNullOrWhiteSpace(itemName))
                        return BadRequest($"Row {row}: Item Name is required.");

                    decimal rate = 0;

                    if (!string.IsNullOrWhiteSpace(rateValue) && decimal.TryParse(rateValue, out decimal excelRate))
                    {
                        rate = excelRate;
                    }
                    else if (!string.IsNullOrWhiteSpace(purchaseprice) && decimal.TryParse(purchaseprice, out decimal dbRate))
                    {
                        rate = dbRate;
                    }
                    else
                    {
                        return BadRequest($"Row {row}: Valid Rate is required.");
                    }
                    if (rate <= 0)
                    {
                        return BadRequest($"Row {row}: Rate must be greater than 0.");
                    }

                    JObject AltRate = JObject.Parse(GetExchange.Result.Value.ToString());
                    decimal AltRateToken = (decimal)AltRate["Result"][0]["IndianValue"];
                    var RateInOther = rate * AltRateToken;

                    JObject DocTypeIdjson = JObject.Parse(GetDocTypeId1.Result.Value.ToString());
                    int DocTypeId = (int)DocTypeIdjson["Result"][0]["DocTypeId"];

                    var DisRs = qty * rate * (Convert.ToDecimal(worksheet.Cells[row, 5].Value?.ToString() ?? "0") / 100);
                    var Amount = (qty * rate) - DisRs;

                    data.Add(new DPBItemDetail()
                    {
                        SeqNo = cnt++,
                        PartText = worksheet.Cells[row, 1].Value?.ToString(),
                        ItemText = itemName,
                        ItemCode = itemCodeValue,
                        PartCode = partcode,
                        HSNNo = Convert.ToInt32(HsnNo.ToString()),
                        DPBQty = qty,
                        BillQty = qty,
                        Unit = Unit.ToString(),
                        AltQty = 0,
                        AltUnit = AlternateUnit.ToString(),
                        AltPendQty = 0,
                        Process = 0,
                        Rate = rate,
                        OtherRateCurr = RateInOther,
                        UnitRate = "",
                        DiscPer = Convert.ToDecimal(worksheet.Cells[row, 5].Value?.ToString() ?? "0"),
                        DiscRs = Math.Round(DisRs, 2),
                        Amount = Math.Round(Amount, 2),
                        TxRemark = "",
                        Description = "",
                        AdditionalRate = 0,
                        Color = "",
                        CostCenter = 0,
                        ItemLocation = location,
                        DocTypeText = docTypeText,
                        docTypeId = DocTypeId,
                    });
                }
            }

            // ✅ Continue with session logic...
            var MainModel = new DirectPurchaseBillModel { ItemDetailGrid = data };
            string serializedMainModel = JsonConvert.SerializeObject(MainModel);
            HttpContext.Session.SetString("DirectPurchaseBill", serializedMainModel);

            return PartialView("_DPBItemGrid", MainModel);
        }

        //[HttpPost]
        //public IActionResult UploadExcel()
        //{
        //        var excelFile = Request.Form.Files[0];
        //    string pono = Request.Form.Where(x => x.Key == "PoNo").FirstOrDefault().Value;
        //    int poYearcode = Convert.ToInt32(Request.Form.Where(x => x.Key == "POYearcode").FirstOrDefault().Value);
        //    int AccountCode = Convert.ToInt32(Request.Form.Where(x => x.Key == "AccountCode").FirstOrDefault().Value);
        //    string SchNo = Request.Form.Where(x => x.Key == "SchNo").FirstOrDefault().Value;
        //    var SchYearCode = Convert.ToInt32(Request.Form.Where(x => x.Key == "SchYearCode").FirstOrDefault().Value);
        //    string Currency = Request.Form.Where(x => x.Key == "Currency").FirstOrDefault().Value;
        //    string Flag = Request.Form.Where(x => x.Key == "Flag").FirstOrDefault().Value;


        //    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        //    List<DPBItemDetail> data = new List<DPBItemDetail>();

        //    using (var stream = excelFile.OpenReadStream())
        //    using (var package = new ExcelPackage(stream))
        //    {
        //        var worksheet = package.Workbook.Worksheets[0];
        //        int cnt = 1;
        //        for (int row = 2; row <= worksheet.Dimension.Rows; row++)
        //        {
        //            //var itemCatCode = IStockAdjust.GetItemCatCode(worksheet.Cells[row, 6].Value.ToString());
        //            var itemCode = IDirectPurchaseBill.GetItemCode(worksheet.Cells[row, 1].Value.ToString());
        //            var partcode = 0;
        //            var itemCodeValue = 0;
        //            if (itemCode.Result.Result != null)
        //            {
        //                partcode = itemCode.Result.Result.Rows.Count <= 0 ? 0 : (int)itemCode.Result.Result.Rows[0].ItemArray[0];
        //                itemCodeValue = itemCode.Result.Result.Rows.Count <= 0 ? 0 : (int)itemCode.Result.Result.Rows[0].ItemArray[0];
        //            }

        //            if (partcode == 0)
        //            {
        //                return Json("Partcode not available");
        //            }
        //            // for pending qty validation -- still need to change
        //            var DPBQty = Convert.ToDecimal(worksheet.Cells[row, 4].Value.ToString());


        //            //for altunit conversion
        //            //var altUnitConversion = AltUnitConversion(partcode, 0, Convert.ToDecimal(worksheet.Cells[row, 6].Value.ToString()));
        //            //JObject AltUnitCon = JObject.Parse(altUnitConversion.Result.Value.ToString());
        //            //decimal altUnitValue = (decimal)AltUnitCon["Result"][0]["AltUnitValue"];


        //            var GetExhange = GetExchangeRate(Currency);
        //            var GetDocTypeId1 = GetDocTypeId(worksheet.Cells[row, 7].Value.ToString());
        //             var GetItem =GetItemDetail(worksheet.Cells[row, 1].Value.ToString());
        //            JObject Jsonstring = JObject.Parse(GetItem.Result.Value.ToString());
        //            var Unit =Jsonstring["Result"][0]["Unit"];
        //            var HsnNo = Jsonstring["Result"][0]["HsnNo"];
        //            var AlternateUnit = Jsonstring["Result"][0]["AlternateUnit"];
        //            var Rackid = Jsonstring["Result"][0]["Rackid"];

        //            JObject AltRate = JObject.Parse(GetExhange.Result.Value.ToString());
        //            decimal AltRateToken = (decimal)AltRate["Result"][0]["IndianValue"];
        //            var RateInOther = Convert.ToDecimal(worksheet.Cells[row, 3].Value) * AltRateToken;

        //            JObject DocTypeIdjson = JObject.Parse(GetDocTypeId1.Result.Value.ToString());
        //            decimal DocTypeId = (int)DocTypeIdjson["Result"][0]["DocTypeId"];


        //            var DisRs = DPBQty * Convert.ToDecimal(worksheet.Cells[row, 3].Value) * (Convert.ToDecimal(worksheet.Cells[row, 5].Value.ToString()) / 100);
        //            var Amount = (DPBQty * Convert.ToDecimal(worksheet.Cells[row, 3].Value)) - DisRs;



        //            data.Add(new DPBItemDetail()
        //            {
        //                SeqNo = cnt++,
        //                PartText = worksheet.Cells[row, 1].Value.ToString(),
        //                ItemText = worksheet.Cells[row, 2].Value.ToString(),
        //                ItemCode = itemCodeValue,
        //                PartCode = partcode,
        //                HSNNo = Convert.ToInt32(HsnNo.ToString()),
        //                DPBQty = Convert.ToDecimal(worksheet.Cells[row, 4].Value.ToString()),
        //                BillQty = Convert.ToDecimal(worksheet.Cells[row, 4].Value.ToString()),
        //                Unit = (Unit.ToString()),
        //                AltQty = 0,
        //                AltUnit = (AlternateUnit.ToString()),
        //                AltPendQty =0,
        //                Process =0,
        //                Rate = Convert.ToDecimal(worksheet.Cells[row, 3].Value.ToString()),
        //                OtherRateCurr = RateInOther,
        //                UnitRate ="",
        //                DiscPer = Convert.ToDecimal(worksheet.Cells[row, 5].Value.ToString()),
        //                DiscRs = Convert.ToDecimal(DisRs.ToString("F2")),
        //                Amount = Convert.ToDecimal(Amount.ToString("F2")),
        //                TxRemark = "",
        //                Description ="",
        //                AdditionalRate = 0,
        //                Color = "",
        //                CostCenter = 0,
        //                ItemLocation = string.IsNullOrEmpty(worksheet.Cells[row, 6].Value.ToString())  ? Rackid.ToString() : worksheet.Cells[row, 6].Value.ToString(),
        //                DocTypeText= (worksheet.Cells[row, 7].Value.ToString()),
        //                docTypeId= Convert.ToInt32(DocTypeId),
        //            });
        //        }
        //    }


        //    var MainModel = new DirectPurchaseBillModel();
        //    var POItemGrid = new List<DPBItemDetail>();
        //    var POGrid = new List<DPBItemDetail>();
        //    var SSGrid = new List<DPBItemDetail>();

        //    MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
        //    {
        //        AbsoluteExpiration = DateTime.Now.AddMinutes(60),
        //        SlidingExpiration = TimeSpan.FromMinutes(55),
        //        Size = 1024,
        //    };
        //    var seqNo = 0;
        //    HttpContext.Session.Remove("DirectPurchaseBill");

        //    foreach (var item in data)
        //    {
        //        if (item != null)
        //        {
        //            // 1. Get the serialized string from session
        //            string serializedModel = HttpContext.Session.GetString("DirectPurchaseBill");

        //            // 2. Deserialize the string back into the original object (DirectPurchaseBillModel)
        //            DirectPurchaseBillModel Model = string.IsNullOrEmpty(serializedModel)
        //                ? new DirectPurchaseBillModel()  // Default if not found
        //                : JsonConvert.DeserializeObject<DirectPurchaseBillModel>(serializedModel);
        //            Model.ItemDetailGrid ??= new List<DPBItemDetail>();
        //            if (Model == null)
        //            {
        //                item.SeqNo += seqNo + 1;
        //                POItemGrid.Add(item);
        //                seqNo++;
        //            }
        //            else
        //            {
        //                if (Model.ItemDetailGrid.Where(x => x.ItemCode == item.ItemCode).Any())
        //                {
        //                    return StatusCode(207, "Duplicate");
        //                }
        //                else
        //                {
        //                    item.SeqNo = Model.ItemDetailGrid.Count + 1;
        //                    POItemGrid = Model.ItemDetailGrid.Where(x => x != null).ToList();
        //                    SSGrid.AddRange(POItemGrid);
        //                    POItemGrid.Add(item);
        //                }
        //            }
        //            MainModel.ItemDetailGrid = POItemGrid;

        //            // 1. Serialize MainModel to a JSON string
        //            string serializedMainModel = JsonConvert.SerializeObject(MainModel);

        //            // 2. Store the serialized string in Session
        //            HttpContext.Session.SetString("DirectPurchaseBill", serializedMainModel);


        //        }
        //    }
        //    // 1. Get the serialized string from Session
        //    string serializedMainModelFromSession = HttpContext.Session.GetString("DirectPurchaseBill");

        //    // 2. Deserialize the string back to DirectPurchaseBillModel
        //    DirectPurchaseBillModel MainModel1 = string.IsNullOrEmpty(serializedMainModelFromSession)
        //        ? new DirectPurchaseBillModel()  // Default if not found
        //        : JsonConvert.DeserializeObject<DirectPurchaseBillModel>(serializedMainModelFromSession);

        //    return PartialView("_DPBItemGrid", MainModel);
        //}
    }

}

