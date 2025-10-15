using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Security.Claims;
using System.Xml.Linq;
using eTactWeb.Data.Common;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Caching.Memory;
using static eTactWeb.DOM.Models.Common;
using eTactWeb.DOM.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System.Runtime.Caching;
using Newtonsoft.Json;
using NuGet.Packaging;
using FastReport.Web;
using common = eTactWeb.Data.Common;
using static eTactWeb.Data.Common.CommonFunc;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using FastReport.Data;
using FastReport;
using System.Configuration;
using Microsoft.AspNetCore.Http;
using System.Drawing.Printing;
using Org.BouncyCastle.Asn1.Ocsp;
using Newtonsoft.Json.Linq;
using DocumentFormat.OpenXml.Vml.Office;


namespace eTactWeb.Controllers
{


    public class SaleInvoiceController : Controller
    {
        private readonly IDataLogic _IDataLogic;
        public ISaleBill _SaleBill { get; }
        public readonly IEinvoiceService _IEinvoiceService;
        private readonly IConfiguration _iconfiguration;
        private readonly ILogger<SaleBillController> _logger;
        private readonly ICustomerJobWorkIssue _ICustomerJobWorkIssue;
        private readonly ICommon _ICommon;
        public IWebHostEnvironment _IWebHostEnvironment { get; }
        private readonly IMemoryCache _MemoryCache;
        private readonly ConnectionStringService _connectionStringService;
        public SaleInvoiceController(ILogger<SaleBillController> logger, IDataLogic iDataLogic, ISaleBill iSaleBill, IEinvoiceService IEinvoiceService, IConfiguration configuration, EncryptDecrypt encryptDecrypt, IWebHostEnvironment iWebHostEnvironment, ICustomerJobWorkIssue CustomerJobWorkIssue, IMemoryCache iMemoryCache, ICommon ICommon, ConnectionStringService connectionStringService)
        {
            _logger = logger;
            _IDataLogic = iDataLogic;
            _SaleBill = iSaleBill;
            _IEinvoiceService = IEinvoiceService;
            _IWebHostEnvironment = iWebHostEnvironment;
            _iconfiguration = configuration;
            _ICustomerJobWorkIssue = CustomerJobWorkIssue;
            _MemoryCache = iMemoryCache;
            _ICommon = ICommon;
            _connectionStringService = connectionStringService;
        }
        public async Task<JsonResult> AutoFillPartCode ( string SearchPartCode)
        {
            var JSON = await _SaleBill.AutoFillitem("AutoFillPartCode", SearchPartCode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetItemGroup()
        {
            var JSON = await _SaleBill.GetItemGroup();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<IActionResult> ShowGroupWiseItems(int Group_Code, int AccountCode, int storeid, string GroupName, string ToDate, string PartCode)
        {
            var FromDate = HttpContext.Session.GetString("FromDate");

            var model = new SaleBillModel();
            model = await _SaleBill.ShowGroupWiseItems( Group_Code,  AccountCode,  storeid,  GroupName,  ToDate,  FromDate,  PartCode);


            return PartialView("_SaleBillGroupWiseItems", model);

        }

        public async Task<IActionResult> GetlastBillDetail(string invoicedate, int currentYearcode, int AccountCode,int ItemCode)
        {
           
            var model = new SaleBillModel();
            model = await _SaleBill.GetlastBillDetail( invoicedate,  currentYearcode,  AccountCode, ItemCode);


            return PartialView("_SaleBillHistoryGrid", model);

        }
        public async Task<JsonResult> GetFormRights()
        {
            var userID = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
            var JSON = await _SaleBill.GetFormRights(userID);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> getdiscCategoryName(int Group_Code, int AccountCode)
        {
            ResponseResult JsonString = await _SaleBill.getdiscCategoryName(Group_Code, AccountCode);
            _logger.LogError(JsonConvert.SerializeObject(JsonString));
            return Json(JsonString);
        }
        public async Task<JsonResult> GETGROUPWISEITEM(int Group_Code)
        {
            var JSON = await _SaleBill.GETGROUPWISEITEM(Group_Code);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<IActionResult> SaleBillList()
        {
            ViewData["Title"] = "Pending SaleBill List";
            TempData.Clear();
            HttpContext.Session.Remove("KeyPendingSaleBill");
            var MainModel = new PendingSaleBillList();
            var model = new PendingSaleBillList();
            MainModel.FromDate = HttpContext.Session.GetString("FromDate");
            MainModel.ToDate = HttpContext.Session.GetString("ToDate");
            MainModel.CurrentYear = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));


            HttpContext.Session.SetString("KeyPendingSaleBill", JsonConvert.SerializeObject(model));
            return View(MainModel);
        }
        [HttpPost]
        public IActionResult RemovePackingChargesTax(string taxName)
        {
            var modelTxGridJson = HttpContext.Session.GetString("KeyTaxGrid");
            if (!string.IsNullOrEmpty(modelTxGridJson))
            {
                var taxGrid = JsonConvert.DeserializeObject<List<TaxModel>>(modelTxGridJson);

                if (taxGrid != null)
                {
                    // ✅ Remove items directly from list
                    taxGrid.RemoveAll(x => x.TxAccountName == taxName);

                    // ✅ Save updated list back to session
                    HttpContext.Session.SetString("KeyTaxGrid", JsonConvert.SerializeObject(taxGrid));
                }
            }

            return Json(new { success = true });
        }

        [HttpPost]
        public IActionResult RemovePForwardingChargesTax(string taxName)
        {
            var modelTxGridJson = HttpContext.Session.GetString("KeyTaxGrid");
            if (!string.IsNullOrEmpty(modelTxGridJson))
            {
                var taxGrid = JsonConvert.DeserializeObject<List<TaxModel>>(modelTxGridJson);

                if (taxGrid != null)
                {
                    // ✅ Remove items directly from list
                    taxGrid.RemoveAll(x => x.TxAccountName == taxName);

                    // ✅ Save updated list back to session
                    HttpContext.Session.SetString("KeyTaxGrid", JsonConvert.SerializeObject(taxGrid));
                }
            }

            return Json(new { success = true });
        }

        [HttpPost]
        public IActionResult RemoveCourieerChargesTax(string taxName)
        {
            var modelTxGridJson = HttpContext.Session.GetString("KeyTaxGrid");
            if (!string.IsNullOrEmpty(modelTxGridJson))
            {
                var taxGrid = JsonConvert.DeserializeObject<List<TaxModel>>(modelTxGridJson);

                if (taxGrid != null)
                {
                    // ✅ Remove items directly from list
                    taxGrid.RemoveAll(x => x.TxAccountName == taxName);

                    // ✅ Save updated list back to session
                    HttpContext.Session.SetString("KeyTaxGrid", JsonConvert.SerializeObject(taxGrid));
                }
            }

            return Json(new { success = true });
        }


        [HttpPost]
        public IActionResult RemoveGSTTax(string taxName)
        {
            var modelTxGridJson = HttpContext.Session.GetString("KeyTaxGrid");
            if (!string.IsNullOrEmpty(modelTxGridJson))
            {
                var taxGrid = JsonConvert.DeserializeObject<List<TaxModel>>(modelTxGridJson);

                if (taxGrid != null)
                {
                    // ✅ Remove items directly from list
                    taxGrid.RemoveAll(x => x.TxAccountName == taxName);

                    // ✅ Save updated list back to session
                    HttpContext.Session.SetString("KeyTaxGrid", JsonConvert.SerializeObject(taxGrid));
                }
            }

            return Json(new { success = true });
        }


        public async Task<JsonResult> FillStoreList()
        {
            var JSON = await _SaleBill.FillStoreList();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillCustomerListForPending()
        {
            var JSON = await _SaleBill.FillCustomerListForPending();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> ShowPendingSaleorderforBill(string Flag, int CurrentYear, string FromDate, string Todate, string InvoiceDate, int BillFromStoreId, int accountCode,string SONo,string PartCode,string CompanyType)
        {
            var JSON = await _SaleBill.ShowPendingSaleorderforBill(Flag, CurrentYear, FromDate, Todate, InvoiceDate, BillFromStoreId, accountCode,  SONo,  PartCode, CompanyType);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }

        public async Task<JsonResult> FILLPendingSONO()
        {
            var JSON = await _SaleBill.FILLPendingSONO();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillPendingPartCOde()
        {
            var JSON = await _SaleBill.FillPendingPartCOde();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }

        [HttpPost]
        public IActionResult StoreCheckedRowsToSession([FromBody] List<SaleBillModel> selectedRows)
        {
            if (selectedRows != null && selectedRows.Any())
            {
                HttpContext.Session.SetString("SaleBillListItem", JsonConvert.SerializeObject(selectedRows));
                return Json(new { success = true });
            }
            return Json(new { success = false, message = "No rows received" });
        }
        public IActionResult FillSaleBillGridFromMemoryCache()
        {
            try
            {
                string modelJson = HttpContext.Session.GetString("SaleBillListItem");
                List<SaleBillDetail> SaleBillDetail = new List<SaleBillDetail>();
                if (!string.IsNullOrEmpty(modelJson))
                {
                    SaleBillDetail = JsonConvert.DeserializeObject<List<SaleBillDetail>>(modelJson);
                }

                var MainModel = new SaleBillModel();
                var IssueGrid = new List<SaleBillDetail>();
                var SSGrid = new List<SaleBillDetail>();
                //MainModel.FromDate = HttpContext.Session.GetString("FromDate");
                //MainModel.ToDate = HttpContext.Session.GetString("ToDate");
                MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                    SlidingExpiration = TimeSpan.FromMinutes(55),
                    Size = 1024,
                };
                var seqNo = 1;
                if (SaleBillDetail != null)
                {
                    for (int i = 0; i < SaleBillDetail.Count; i++)
                    {


                        if (SaleBillDetail[i] != null)
                        {
                            //TransferFromWorkCenterDetail[i].seqno = seqNo++;
                            SSGrid.AddRange(IssueGrid);
                            IssueGrid.Add(SaleBillDetail[i]);

                            MainModel.ItemDetailGrid = IssueGrid;

                            string serializedGrid = JsonConvert.SerializeObject(MainModel.ItemDetailGrid);
                            HttpContext.Session.SetString("SaleBillListItem", serializedGrid);
                        }
                    }
                }

                return PartialView("_SaleBillItemMemoryGrid", MainModel);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public IActionResult AddMultiSaleBillDetail([FromBody] List<SaleBillDetail> model)
        {
            try
            {
                var MainModel = new SaleBillModel();
                var RCGrid = new List<SaleBillDetail>();
                var ReceiveChallanGrid = new List<SaleBillDetail>();

                var SeqNo = 1;
                foreach (var item in model)
                {
                    //string modelJson = HttpContext.Session.GetString("ReceiveItems");
                    //IList<TransferFromWorkCenterDetail> RCDetail = new List<TransferFromWorkCenterDetail>();
                    //if (modelJson != null)
                    //{
                    //    RCDetail = JsonConvert.DeserializeObject<List<TransferFromWorkCenterDetail>>(modelJson);
                    //}

                    if (model != null)
                    {

                        {
                            item.SeqNo = SeqNo;
                            //RCGrid = RCDetail.Where(x => x != null).ToList();
                            ReceiveChallanGrid.AddRange(RCGrid);
                            RCGrid.Add(item);
                            SeqNo++;


                        }
                        RCGrid = RCGrid.OrderBy(item => item.SeqNo).ToList();
                        MainModel.saleBillDetails = RCGrid;
                        MainModel.ItemDetailGrid = RCGrid;


                        HttpContext.Session.SetString("KeySaleBillGrid", JsonConvert.SerializeObject(MainModel.saleBillDetails));
                        HttpContext.Session.SetString("SaleBillModel", JsonConvert.SerializeObject(MainModel));

                        //HttpContext.Session.SetString("KeyTransferFromWorkCenterGrid", JsonConvert.SerializeObject(MainModel.ItemDetailGrid));
                    }
                    else
                    {
                        ModelState.TryAddModelError("Error", "Receive Challan List Cannot Be Empty...!");
                    }
                }


                return PartialView("_SaleBillGrid", MainModel);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<JsonResult> AutoFillStore(string SearchStoreName)
        {
            var JSON = await _SaleBill.AutoFillStore(SearchStoreName);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        [HttpPost]
        public async Task<IActionResult> SaleInvoice(SaleBillModel model, string ShouldEinvoice)
        {
            var SBGrid = new DataTable();
            DataTable TaxDetailDT = null;
            DataTable AdjDetailDT = null;
            DataTable DrCrDetailDT = null;
            DataTable AdjChallanDetailDT = null;
            string SaleBillModel = HttpContext.Session.GetString("SaleBillModel");
            SaleBillModel MainModel = new SaleBillModel();
          
            if (!string.IsNullOrEmpty(SaleBillModel))
            {
                MainModel = JsonConvert.DeserializeObject<SaleBillModel>(SaleBillModel);
            }
            string modelJson = HttpContext.Session.GetString("KeySaleBillGrid");
            IList<SaleBillDetail> saleBillDetail = new List<SaleBillDetail>();
            if (!string.IsNullOrEmpty(modelJson))
            {
                saleBillDetail = JsonConvert.DeserializeObject<IList<SaleBillDetail>>(modelJson);
            }

            string serializedGrid = HttpContext.Session.GetString("KeyAdjGrid");
            var adjustmentModel = new AdjustmentModel();
            if (!string.IsNullOrEmpty(serializedGrid))
            {
                adjustmentModel = JsonConvert.DeserializeObject<AdjustmentModel>(serializedGrid);
                // Use adjustmentModel as needed
            }

            string AdjChallanGridJson = HttpContext.Session.GetString("KeyAdjChallanGrid");
            List<CustomerJobWorkChallanAdj> AdjChallanGrid = new List<CustomerJobWorkChallanAdj>();
            if (!string.IsNullOrEmpty(AdjChallanGridJson))
            {
                AdjChallanGrid = JsonConvert.DeserializeObject<List<CustomerJobWorkChallanAdj>>(AdjChallanGridJson);
            }
            string TaxGridJson = HttpContext.Session.GetString("KeyTaxGrid");
            List<TaxModel> TaxGrid = new List<TaxModel>();
            if (!string.IsNullOrEmpty(TaxGridJson))
            {
                TaxGrid = JsonConvert.DeserializeObject<List<TaxModel>>(TaxGridJson);
            }

            //string modelAdjJson = HttpContext.Session.GetString("KeyAdjGrid");
            //List<AdjustmentModel> adjModel = new();
            //if (!string.IsNullOrEmpty(modelAdjJson))
            //{
            //     adjModel = JsonConvert.DeserializeObject<List<AdjustmentModel>>(modelAdjJson);
            //}


            string DrCrGridJson = HttpContext.Session.GetString("KeyDrCrGrid");
            List<DbCrModel> DrCrGrid = new List<DbCrModel>();
            if (!string.IsNullOrEmpty(DrCrGridJson))
            {
                DrCrGrid = JsonConvert.DeserializeObject<List<DbCrModel>>(DrCrGridJson);
            }
            if (saleBillDetail == null)
            {
                ModelState.Clear();
                ModelState.TryAddModelError("SaleBillDetail", "Sale Bill Grid Should Have Atleast 1 Item...!");
                return View("StockADjustment", model);
            }
            else if (saleBillDetail == null)
            {
                ModelState.Clear();
                ModelState.TryAddModelError("TaxDetail", "Tax Grid Should Have Atleast 1 Item...!");
                return View("StockADjustment", model);
            }
            else
            {
                model.CC = HttpContext.Session.GetString("Branch");
                //model.ActualEnteredBy   = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
                model.Uid = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                if (model.Mode == "U")
                {
                    model.LastUpdatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                    model.LastUpdatedByName = HttpContext.Session.GetString("EmpName");
                    SBGrid = GetDetailTable(saleBillDetail);
                }
                else
                {
                    model.ActualEnteredBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                    model.ActualEnteredByName = HttpContext.Session.GetString("EmpName");
                    model.EntryByempId = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                    SBGrid = GetDetailTable(saleBillDetail);
                }

                if (AdjChallanGrid != null && AdjChallanGrid.Count > 0)
                {
                    AdjChallanDetailDT = GetCustomerJobWorkChallanAdjTable(AdjChallanGrid);
                }

                if (TaxGrid != null && TaxGrid.Count > 0)
                {
                    TaxDetailDT = GetTaxDetailTable(TaxGrid);
                }
                if (DrCrGrid != null && DrCrGrid.Count > 0)
                {
                    DrCrDetailDT = CommonController.GetDrCrDetailTable(DrCrGrid);
                }

                if (adjustmentModel.AdjAdjustmentDetailGrid != null && adjustmentModel.AdjAdjustmentDetailGrid.Count > 0)
                {
                    AdjDetailDT = CommonController.GetAdjDetailTable(adjustmentModel.AdjAdjustmentDetailGrid.ToList(), model.SaleBillEntryId, model.SaleBillYearCode, model.AccountCode);
                }
                string serverFolderPath = Path.Combine(_IWebHostEnvironment.WebRootPath, "Uploads", "SaleBill");
                if (!Directory.Exists(serverFolderPath))
                {
                    Directory.CreateDirectory(serverFolderPath);
                }

                if (model.AttachmentFile1 != null)
                {
                    
                    string ImagePath = "Uploads/SaleBill/";
                    string extension = Path.GetExtension(model.AttachmentFile1.FileName)?.ToLowerInvariant();
                    string VouchNo = model.SaleBillNo.Replace("\\", "_").Replace("/", "_");
                    ImagePath += VouchNo + "_" + model.SaleBillYearCode + "_" + model.SaleBillEntryDate.Replace("\\", "_").Replace("/", "_") + "_" + Guid.NewGuid().ToString() + extension;

                    string ServerPath = Path.Combine(_IWebHostEnvironment.WebRootPath, ImagePath);
                    using (FileStream FileStream = new FileStream(ServerPath, FileMode.Create))
                    {
                        await model.AttachmentFile1.CopyToAsync(FileStream);
                    }
                    model.AttachmentFilePath1 = "/" + ImagePath;
                }
                else
                {
                    model.AttachmentFilePath1 = MainModel.AttachmentFilePath1;
                }

                if (model.AttachmentFile2 != null)
                {
                    string ImagePath = "Uploads/SaleBill/";
                    string extension = Path.GetExtension(model.AttachmentFile2.FileName)?.ToLowerInvariant();
                    string VouchNo = model.SaleBillNo.Replace("\\", "_").Replace("/", "_");
                    ImagePath += VouchNo + "_" + model.SaleBillYearCode + "_" + model.SaleBillEntryDate.Replace("\\", "_").Replace("/", "_") + "_" + "2" + "_" + Guid.NewGuid().ToString() + extension;
                    //var ImagePath = Path.Combine("Uploads", "SaleBill", Guid.NewGuid().ToString() + "_" + model.AttachmentFile2.FileName);
                    string ServerPath = Path.Combine(_IWebHostEnvironment.WebRootPath, ImagePath);
                    using (FileStream FileStream = new FileStream(ServerPath, FileMode.Create))
                    {
                        await model.AttachmentFile2.CopyToAsync(FileStream);
                    }
                    model.AttachmentFilePath2 = "/" + ImagePath;
                }
                else
                {
                    model.AttachmentFilePath2 = MainModel.AttachmentFilePath2;
                }

                if (model.AttachmentFile3 != null)
                {
                    //var ImagePath = Path.Combine("Uploads", "SaleBill", Guid.NewGuid().ToString() + "_" + model.AttachmentFile3.FileName);
                    string ImagePath = "Uploads/SaleBill/";
                    string extension = Path.GetExtension(model.AttachmentFile3.FileName)?.ToLowerInvariant();
                    string VouchNo = model.SaleBillNo.Replace("\\", "_").Replace("/", "_");
                    ImagePath += VouchNo + "_" + model.SaleBillYearCode + "_" + model.SaleBillEntryDate.Replace("\\", "_").Replace("/", "_") + "_" + "3" + "_" + Guid.NewGuid().ToString() + extension;

                    string ServerPath = Path.Combine(_IWebHostEnvironment.WebRootPath, ImagePath);
                    using (FileStream FileStream = new FileStream(ServerPath, FileMode.Create))
                    {
                        await model.AttachmentFile3.CopyToAsync(FileStream);
                    }
                    model.AttachmentFilePath3 = "/" + ImagePath;
                }
                else
                {
                    model.AttachmentFilePath2 = MainModel.AttachmentFilePath2;
                }

                var Result = await _SaleBill.SaveSaleBill(model, SBGrid, TaxDetailDT, DrCrDetailDT, AdjDetailDT, AdjChallanDetailDT);

                if (Result != null)
                {
                    if (Result.StatusText == "Success" && Result.StatusCode == HttpStatusCode.OK)
                    {
                        ViewBag.isSuccess = true;
                        TempData["200"] = "200";
                        var model1 = new SaleBillModel();
                        TempData["ShowEinvoicePopup"] = "true";
                        TempData["SaleBillModelJson"] = JsonConvert.SerializeObject(model1);
                        model1.adjustmentModel = model1.adjustmentModel ?? new AdjustmentModel();

                        model1.FinFromDate = HttpContext.Session.GetString("FromDate");
                        model1.FinToDate = HttpContext.Session.GetString("ToDate");
                        var yearCodeStr = HttpContext.Session.GetString("YearCode");
                        model1.SaleBillYearCode = !string.IsNullOrEmpty(yearCodeStr) ? Convert.ToInt32(yearCodeStr) : 0;
                        model1.CC = HttpContext.Session.GetString("Branch");
                        var uidStr = HttpContext.Session.GetString("UID");
                        model1.CreatedBy = !string.IsNullOrEmpty(uidStr) ? Convert.ToInt32(uidStr) : 0;
                        //model1.ActualEnteredByName = HttpContext.Session.GetString("EmpName");
                        model1.CreatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                        HttpContext.Session.Remove("SaleBillListItem");
                        TempData["ShowEinvoicePopup"] = "true";
                        if (ShouldEinvoice == "true")
                        {
                            return Json(new
                            {
                                status = "Success",
                                EntryId = model.SaleBillEntryId,
                                InvoiceNo = model.SaleBillNo,
                                YearCode = model.SaleBillYearCode,
                                saleBillType = model.SupplyType,
                                customerPartCode = model.PartCode,
                                transporterName = model.TransporterName,
                                vehicleNo = model.vehicleNo,
                                distanceKM = model.DistanceKM,
                                EntrybyId = model.EntryByempId,
                                MachineName = model.MachineName,
                                AccountCode = model.AccountCode

                            });
                        }
                        
                        HttpContext.Session.Remove("KeySaleBillGrid");
                        HttpContext.Session.Remove("SaleBillModel");
                        //return RedirectToAction(nameof(SaleInvoice), new { Id = 0, Mode = "", YC = 0 });
                    }
                    if (Result.StatusText == "Updated" && Result.StatusCode == HttpStatusCode.Accepted)
                    {
                        ViewBag.isSuccess = true;
                        TempData["202"] = "202";

                        var model1 = new SaleBillModel();

                        model1.adjustmentModel = new AdjustmentModel();
                        model1.adjustmentModel = model.adjustmentModel ?? new AdjustmentModel();
                        model1.FinFromDate = HttpContext.Session.GetString("FromDate");
                        model1.FinToDate = HttpContext.Session.GetString("ToDate");
                        model1.SaleBillYearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
                        model1.CC = HttpContext.Session.GetString("Branch");
                        //model1.ActualEnteredByName = HttpContext.Session.GetString("EmpName");
                        model1.CreatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));

                        ViewBag.ShowEinvoicePrompt = true;
                        //if (ShouldPrint == "true")
                        //{
                        //    return Json(new
                        //    {
                        //        status = "Success", // ✅ Add this!
                        //        EntryId = model1.SaleBillEntryId,
                        //        YearCode = model1.SaleBillYearCode,
                        //        InvoiceNo = model1.SaleBillNo,
                        //        saleBillType = model1.SupplyType,
                        //        customerPartCode = "" // optional
                        //    });
                        //}

                        //  return View(model1);
                        HttpContext.Session.Remove("SaleBillListItem");
                        if (ShouldEinvoice == "true")
                        {
                            return Json(new
                            {
                                status = "Success",
                                EntryId = model.SaleBillEntryId,
                                InvoiceNo = model.SaleBillNo,
                                YearCode = model.SaleBillYearCode,
                                saleBillType = model.SupplyType,
                                customerPartCode = model.PartCode,
                                transporterName = model.TransporterName,
                                vehicleNo = model.vehicleNo,
                                distanceKM = model.DistanceKM,
                                EntrybyId = model.EntryByempId,
                                MachineName = model.MachineName,
                                AccountCode = model.AccountCode

                            });

                        }
                       
                            HttpContext.Session.Remove("KeySaleBillGrid");
                        HttpContext.Session.Remove("SaleBillModel");
                    }
                    if (Result.StatusText == "Error" && Result.StatusCode == HttpStatusCode.InternalServerError)
                    {
                        var errNum = Result.Result.Message.ToString().Split(":")[1];
                        model.adjustmentModel = model.adjustmentModel ?? new AdjustmentModel();
                        if (errNum == " 2627")
                        {
                            ViewBag.isSuccess = false;
                            TempData["2627"] = "2627";
                            _logger.LogError("\n \n ********** LogError ********** \n " + JsonConvert.SerializeObject(Result) + "\n \n");

                            return View(model);
                        }

                        ViewBag.isSuccess = false;
                        TempData["500"] = "500";
                        _logger.LogError("\n \n ********** LogError ********** \n " + JsonConvert.SerializeObject(Result) + "\n \n");
                        HttpContext.Session.Remove("SaleBillListItem");
                        // return View("Error", Result);
                        return View(model);
                    }
                    if (Result.StatusText == "TransDate" || Result.StatusCode == HttpStatusCode.InternalServerError)
                    {
                        ViewBag.isSuccess = false;
                        var input = "";
                        if (Result?.Result != null)
                        {
                            if (Result.Result is string str)
                            {
                                input = str;
                            }
                            else
                            {
                                input = JsonConvert.SerializeObject(Result.Result);
                            }

                            TempData["ErrorMessage"] = input;
                        }
                        else
                        {
                            TempData["500"] = "500";
                        }


                        _logger.LogError("\n \n ********** LogError ********** \n " + JsonConvert.SerializeObject(Result) + "\n \n");
                        //model.IsError = "true";
                        //return View("Error", Result);
                    }

                    HttpContext.Session.SetString("SaleInvoice", JsonConvert.SerializeObject(model));
                }
                HttpContext.Session.Remove("SaleBillListItem");
               // return Json(new { status = "Success" });
                return Json(new
                {
                    status = "Success",
                    EntryId = model.SaleBillEntryId,
                    InvoiceNo = model.SaleBillNo,
                    YearCode = model.SaleBillYearCode,
                    saleBillType = model.SupplyType,
                    AccountCode = model.AccountCode

                });

                // return View();
            }
        }

        public static DataTable GetAdjustChallanDetailTable(List<CustomerInputJobWorkIssueAdjustDetail> model)
        {
            DataTable Table = new();
            Table.Columns.Add("ItemCode", typeof(int));
            Table.Columns.Add("Unit", typeof(string));
            Table.Columns.Add("BillQty", typeof(float));
            Table.Columns.Add("JWRate", typeof(float));
            Table.Columns.Add("ProcessId", typeof(int));
            Table.Columns.Add("SONO", typeof(string));
            Table.Columns.Add("CustOrderNo", typeof(string));
            Table.Columns.Add("SOYearCode", typeof(int));
            Table.Columns.Add("SchNo", typeof(string));
            Table.Columns.Add("SchYearCode", typeof(int));
            Table.Columns.Add("BOMIND", typeof(string));
            Table.Columns.Add("BOMNO", typeof(int));
            Table.Columns.Add("BOMEffDate", typeof(string));
            Table.Columns.Add("Produnprod", typeof(string));
            Table.Columns.Add("fromChallanOrSalebill", typeof(string));
            Table.Columns.Add("ItemAdjustmentRequired", typeof(string));

            if (model != null && model.Count > 0)
            {
                foreach (var Item in model)
                {
                    Table.Rows.Add(
                    new object[]
                    {
                        Item.ItemCode,
                        Item.Unit  ?? string.Empty,
                        Item.BillQty,
                        Item.JWRate,
                        Item.ProcessId,
                        Item.SONO ?? string.Empty,
                        Item.CustOrderNo ?? string.Empty,
                        Item.SOYearCode,
                        Item.SchNo ?? string.Empty,
                        Item.SchYearCode,
                        Item.BOMIND ?? string.Empty,
                        Item.BOMNO,
                        Item.BOMEffDate == null ? string.Empty : ParseFormattedDate(Item.BOMEffDate) ,
                        Item.Produnprod ?? string.Empty,
                        Item.fromChallanOrSalebill ?? string.Empty,
                        Item.ItemAdjustmentRequired ?? string.Empty
                    });
                }
            }
            return Table;
        }

        [HttpPost]
        public async Task<IActionResult> GetAdjustedChallanDetailsData(List<CustomerInputJobWorkIssueAdjustDetail> model, int YearCode, string EntryDate, string ChallanDate, int AccountCode, int itemCode)
        {
            try
            {
                if (model == null || !model.Any())
                {
                    return Json(new { success = false, message = "No data received." });
                }

                var adjustChallanDt = GetAdjustChallanDetailTable(model);
                var result = await _SaleBill.GetAdjustedChallanDetailsData(adjustChallanDt, YearCode, EntryDate, ChallanDate, AccountCode);

                HttpContext.Session.SetString("KeyAdjChallanGrid", JsonConvert.SerializeObject(result.CustomerJobWorkChallanAdj));

                // result.CustomerJobWorkIssueAdjustDetails
                return PartialView("_CustomerJwisschallanAdjustment", result.CustomerJobWorkChallanAdj);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetBomAdjustedChallanDetailsData(List<CustomerInputJobWorkIssueAdjustDetail> model, int YearCode, string EntryDate, string ChallanDate, int AccountCode, int itemCode)
        {
            try
            {
                if (model == null || !model.Any())
                {
                    return Json(new { success = false, message = "No data received." });
                }

                var adjustChallanDt = GetAdjustChallanDetailTable(model);
                var result = await _SaleBill.GetAdjustedChallanDetailsData(adjustChallanDt, YearCode, EntryDate, ChallanDate, AccountCode);

                HttpContext.Session.SetString("KeyAdjChallanGrid", JsonConvert.SerializeObject(result.CustomerJobWorkChallanAdj));

                return PartialView("_BomCustJwisschallanAdjustment", result.BomCustomerJWIssChallanAdj);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> SaleInvoice(int ID, string Mode, int YearCode, string DashboardType = "", string FromDate = "", string ToDate = "", string partCode = "", string itemName = "", string VoucherNo = "", string custName = "", string sono = "", string custOrderNo = "", string schNo = "", string PerformaInvNo = "", string saleQuoteNo = "", string domExportNEPZ = "", string Searchbox = "", string summaryDetail = "", int? GroupName = null, int? AccountCode = null, int? AccountCodeBack = null, string VoucherTypeBack = "", string[] AccountList = null,string? Narration = "", float? Amount = null, string? DR="", string? CR="")
        {
            var model = new SaleBillModel(); // Create a new model instance for the view

            ViewData["Title"] = "Sale Bill Details";
            TempData.Clear();
            HttpContext.Session.Remove("KeySaleBillGrid");
            HttpContext.Session.Remove("SaleBillModel");
            HttpContext.Session.Remove("KeyAdjGrid");
            HttpContext.Session.Remove("KeyAdjChallanGrid");
            var featuresoptions = _SaleBill.GetFeatureOption();

            if (featuresoptions?.Result?.Result != null &&
                featuresoptions.Result.Result.Rows.Count > 0)
            {
                model.AllowToChangeSaleBillStoreName =
                    featuresoptions.Result.Result.Rows[0]["AllowToChangeSaleBillStoreName"]?.ToString() ?? "";
            }
            else
            {
                model.AllowToChangeSaleBillStoreName = "";
            }

            if (model.Mode != "U" && model.Mode != "V")
            {
                model.Uid = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                model.ActualEnteredBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                model.ActualEnteredByName = HttpContext.Session.GetString("EmpName");
            }
            model.adjustmentModel = new AdjustmentModel();

            if (!string.IsNullOrEmpty(Mode) && ID > 0 && (Mode == "V" || Mode == "U"))
            {
                model = await _SaleBill.GetViewByID(ID, Mode, YearCode);
                model.Mode = Mode;
                model.ID = ID;
            }
            else
            {
                model.FinFromDate = HttpContext.Session.GetString("FromDate");
                model.FinToDate = HttpContext.Session.GetString("ToDate");
                model.SaleBillYearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
                model.CC = HttpContext.Session.GetString("Branch");
            }

            MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                SlidingExpiration = TimeSpan.FromMinutes(55),
                Size = 1024,
            };

            model.CustomerJobWorkChallanAdj = model.CustomerJobWorkChallanAdj == null ? new List<CustomerJobWorkChallanAdj>() : model.CustomerJobWorkChallanAdj;
            HttpContext.Session.SetString("KeySaleBillGrid", JsonConvert.SerializeObject(model.saleBillDetails));
            HttpContext.Session.SetString("KeyAdjChallanGrid", JsonConvert.SerializeObject(model.CustomerJobWorkChallanAdj));
            HttpContext.Session.SetString("KeyAdjGrid", JsonConvert.SerializeObject(model.adjustmentModel == null ? new AdjustmentModel() : model.adjustmentModel));
            HttpContext.Session.SetString("KeyTaxGrid", JsonConvert.SerializeObject(model.TaxDetailGridd == null ? new List<TaxModel>() : model.TaxDetailGridd));
            HttpContext.Session.SetString("SaleBillModel", JsonConvert.SerializeObject(model == null ? new SaleBillModel() : model));
            HttpContext.Session.SetString("SaleInvoice", JsonConvert.SerializeObject(model));

            model.FromDateBack = FromDate;
            model.ToDateBack = ToDate;
            model.PartCodeBack = partCode;
            model.ItemNameBack = itemName;
            model.SaleBillNoBack = VoucherNo;
            model.CustNameBack = custName;
            model.SonoBack = sono;
            model.CustOrderNoBack = custOrderNo;
            model.SchNoBack = schNo;
            model.PerformaInvNoBack = PerformaInvNo;
            model.SaleQuoteNoBack = saleQuoteNo;
            model.DomesticExportNEPZBack = domExportNEPZ;
            model.SearchBoxBack = Searchbox;
            model.SummaryDetailBack = summaryDetail;
            model.DashboardTypeBack = DashboardType;
            model.GroupCodeBack = GroupName;
            model.AccountCodeBack = AccountCode;
            model.AccountNameBack = AccountCodeBack;
            model.VoucherTypeBack = VoucherTypeBack;
            model.AccountList = AccountList;
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> SaleBillOnCounter(int ID, string Mode, int YearCode, string dashboardType = "", string fromDate = "", string toDate = "", string partCode = "", string itemName = "", string VoucherNo = "", string custName = "", string sono = "", string custOrderNo = "", string schNo = "", string performaInvNo = "", string saleQuoteNo = "", string domExportNEPZ = "", string Searchbox = "", string summaryDetail = "")
        {
            var model = new SaleBillModel(); // Create a new model instance for the view

            ViewData["Title"] = "Sale Bill Details";
            TempData.Clear();
            HttpContext.Session.Remove("KeySaleBillGrid");
            HttpContext.Session.Remove("SaleBillModel");
            HttpContext.Session.Remove("KeyAdjGrid");
            HttpContext.Session.Remove("KeyAdjChallanGrid");
            var featuresoptions = _SaleBill.GetFeatureOption();

            if (featuresoptions?.Result?.Result != null &&
                featuresoptions.Result.Result.Rows.Count > 0)
            {
                model.AllowToChangeSaleBillStoreName =
                    featuresoptions.Result.Result.Rows[0]["AllowToChangeSaleBillStoreName"]?.ToString() ?? "";
            }
            else
            {
                model.AllowToChangeSaleBillStoreName = "";
            }

            if (model.Mode != "U" && model.Mode != "V")
            {
                model.Uid = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                model.ActualEnteredBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                model.ActualEnteredByName = HttpContext.Session.GetString("EmpName");
            }
            model.adjustmentModel = new AdjustmentModel();

            if (!string.IsNullOrEmpty(Mode) && ID > 0 && (Mode == "V" || Mode == "U"))
            {
                model = await _SaleBill.GetViewByID(ID, Mode, YearCode);
                model.Mode = Mode;
                model.ID = ID;
            }
            else
            {
                model.FinFromDate = HttpContext.Session.GetString("FromDate");
                model.FinToDate = HttpContext.Session.GetString("ToDate");
                model.SaleBillYearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
                model.CC = HttpContext.Session.GetString("Branch");
            }

            MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                SlidingExpiration = TimeSpan.FromMinutes(55),
                Size = 1024,
            };

            model.CustomerJobWorkChallanAdj = model.CustomerJobWorkChallanAdj == null ? new List<CustomerJobWorkChallanAdj>() : model.CustomerJobWorkChallanAdj;
            HttpContext.Session.SetString("KeySaleBillGrid", JsonConvert.SerializeObject(model.saleBillDetails));
            HttpContext.Session.SetString("KeyAdjChallanGrid", JsonConvert.SerializeObject(model.CustomerJobWorkChallanAdj));
            HttpContext.Session.SetString("KeyAdjGrid", JsonConvert.SerializeObject(model.adjustmentModel == null ? new AdjustmentModel() : model.adjustmentModel));
            HttpContext.Session.SetString("KeyTaxGrid", JsonConvert.SerializeObject(model.TaxDetailGridd == null ? new List<TaxModel>() : model.TaxDetailGridd));
            HttpContext.Session.SetString("SaleBillModel", JsonConvert.SerializeObject(model == null ? new SaleBillModel() : model));
            HttpContext.Session.SetString("SaleInvoice", JsonConvert.SerializeObject(model));

            model.FromDateBack = fromDate;
            model.ToDateBack = toDate;
            model.PartCodeBack = partCode;
            model.ItemNameBack = itemName;
            model.SaleBillNoBack = VoucherNo;
            model.CustNameBack = custName;
            model.SonoBack = sono;
            model.CustOrderNoBack = custOrderNo;
            model.SchNoBack = schNo;
            model.PerformaInvNoBack = performaInvNo;
            model.SaleQuoteNoBack = saleQuoteNo;
            model.DomesticExportNEPZBack = domExportNEPZ;
            model.SearchBoxBack = Searchbox;
            model.SummaryDetailBack = summaryDetail;

            return View(model);

        }
            public async Task<IActionResult> SaleInvoiceMemoryGrid()
        {
            
            HttpContext.Session.Remove("SaleBillListItem");



            return RedirectToAction("SaleInvoice");
        }



        [HttpGet]
        [Route("{controller}/Dashboard")]
        public async Task<IActionResult> SBDashboard(string summaryDetail = "", string Flag = "True", string partCode = "", string itemName = "", string saleBillno = "", string customerName = "", string sono = "", string custOrderNo = "", string schNo = "", string performaInvNo = "", string saleQuoteNo = "", string domensticExportNEPZ = "", string fromdate = "", string toDate = "", string searchBox = "",string SaleBillEntryFrom="")
        {
            try
            {
                HttpContext.Session.Remove("KeySaleBillGrid");
                HttpContext.Session.Remove("KeyTaxGrid");
                var model = new SaleBillDashboard();

                var FromDt = HttpContext.Session.GetString("FromDate");
                model.FinFromDate = Convert.ToDateTime(FromDt).ToString("dd/MM/yyyy");
                //DateTime ToDate = DateTime.Today;
                var ToDt = HttpContext.Session.GetString("ToDate");
                model.FinToDate = Convert.ToDateTime(ToDt).ToString("dd/MM/yyyy");

                model.SaleBillYearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
                model.SummaryDetail = "Summary";
                model.SONO = "";
                var Result = await _SaleBill.GetDashboardData(model.SummaryDetail, partCode, itemName, saleBillno, customerName, sono, custOrderNo, schNo, performaInvNo, saleQuoteNo, domensticExportNEPZ, ParseFormattedDate(model.FinFromDate.Split(" ")[0]), common.CommonFunc.ParseFormattedDate(model.FinToDate.Split(" ")[0]), SaleBillEntryFrom).ConfigureAwait(true);
                if (Result != null)
                {
                    var _List = new List<TextValue>();
                    DataSet DS = Result.Result;
                    if (DS != null)
                    {
                        var DT = DS.Tables[0].DefaultView.ToTable(true, "SaleBillNo", "SaleBillDate", "GSTNO", "AccountCode", "AccountName", "SupplyType", "CustAddress", "StateNameofSupply", "AgainstVoucherNo"
                            , "CityofSupply", "DocumentHead", "ConsigneeAccountName", "ConsigneeAddress", "PaymentTerm", "Currency", "BillAmt", "TaxableAmt", "GSTAmount", "RoundType", "RoundOffAmt", "INVNetAmt"
                            , "Ewaybillno", "EInvNo", "EinvGenerated", "CountryOfSupply", "TransporterdocNo", "TransportModeBYRoadAIR", "DispatchTo", "DispatchThrough", "Remark", "Approved", "ApprovDate", "ApprovedBy", "ExchangeRate"
                            , "SaleBillEntryId", "SaleBillYearCode", "SaleBillEntryDate", "Shippingdate", "DistanceKM", "vehicleNo", "TransporterName", "DomesticExportNEPZ", "PaymentCreditDay", "ReceivedAmt", "pendAmount"
                            , "CancelBill", "Canceldate", "CancelBy", "Cancelreason", "BankName", "FreightPaid", "DispatchDelayReason", "AttachmentFilePath1", "AttachmentFilePath2", "AttachmentFilePath3", "DocketNo", "DispatchDelayreson", "Commodity", "CC"
                            , "Uid", "MachineName", "ActualEnteredByName", "ActualEntryDate", "LastUpdatedByName"
                            , "LastUpdationDate", "TypeItemServAssets", "SaleBillJobwork", "PerformaInvNo", "PerformaInvDate", "PerformaInvYearCode"
                            , "BILLAgainstWarrenty", "ExportInvoiceNo", "InvoiceTime", "RemovalDate", "RemovalTime", "EntryFreezToAccounts", "BalanceSheetClosed", "SaleQuotNo", "SaleQuotDate", "salesperson_name", "SalesPersonMobile", "SaleBillEntryFrom"
                            );
                        model.SaleBillDataDashboard = CommonFunc.DataTableToList<SaleBillDashboard>(DT, "SaleBillSummTable");
                    }

                    if (Flag != "True")
                    {
                        model.FromDate1 = fromdate;
                        model.ToDate1 = toDate;
                        model.FinFromDate = fromdate;
                        model.FinToDate = toDate;
                        model.SaleBillNo = saleBillno;
                        model.PartCode = partCode;
                        model.ItemName = itemName;
                        model.AccountName = customerName;
                        model.SONO = sono;
                        model.CustOrderNo = custOrderNo;
                        model.SchNo = schNo;
                        model.PerformaInvNo = performaInvNo;
                        model.SaleQuotNo = saleQuoteNo;
                        model.DomesticExportNEPZ = domensticExportNEPZ;
                        model.SummaryDetail = summaryDetail;
                        model.Searchbox = searchBox;
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

        [HttpGet]
        [Route("{controller}/SaleBillOnCounterDashboard")]
        public async Task<IActionResult> SaleBillOnCounterDashboard(string summaryDetail = "", string Flag = "True", string partCode = "", string itemName = "", string saleBillno = "", string customerName = "", string sono = "", string custOrderNo = "", string schNo = "", string performaInvNo = "", string saleQuoteNo = "", string domensticExportNEPZ = "", string fromdate = "", string toDate = "", string searchBox = "",string SaleBillEntryFrom="")
        {
            try
            {
                HttpContext.Session.Remove("KeySaleBillGrid");
                HttpContext.Session.Remove("KeyTaxGrid");
                var model = new SaleBillDashboard();

                var FromDt = HttpContext.Session.GetString("FromDate");
                model.FinFromDate = Convert.ToDateTime(FromDt).ToString("dd/MM/yyyy");
                //DateTime ToDate = DateTime.Today;
                var ToDt = HttpContext.Session.GetString("ToDate");
                model.FinToDate = Convert.ToDateTime(ToDt).ToString("dd/MM/yyyy");

                model.SaleBillYearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
                model.SummaryDetail = "Summary";
                model.SONO = "";
                var Result = await _SaleBill.GetDashboardData(model.SummaryDetail, partCode, itemName, saleBillno, customerName, sono, custOrderNo, schNo, performaInvNo, saleQuoteNo, domensticExportNEPZ, ParseFormattedDate(model.FinFromDate.Split(" ")[0]), common.CommonFunc.ParseFormattedDate(model.FinToDate.Split(" ")[0]), SaleBillEntryFrom).ConfigureAwait(true);
                if (Result != null)
                {
                    var _List = new List<TextValue>();
                    DataSet DS = Result.Result;
                    if (DS != null)
                    {
                        var DT = DS.Tables[0].DefaultView.ToTable(true, "SaleBillNo", "SaleBillDate", "GSTNO", "AccountCode", "AccountName", "SupplyType", "CustAddress", "StateNameofSupply", "AgainstVoucherNo"
                            , "CityofSupply", "DocumentHead", "ConsigneeAccountName", "ConsigneeAddress", "PaymentTerm", "Currency", "BillAmt", "TaxableAmt", "GSTAmount", "RoundType", "RoundOffAmt", "INVNetAmt"
                            , "Ewaybillno", "EInvNo", "EinvGenerated", "CountryOfSupply", "TransporterdocNo", "TransportModeBYRoadAIR", "DispatchTo", "DispatchThrough", "Remark", "Approved", "ApprovDate", "ApprovedBy", "ExchangeRate"
                            , "SaleBillEntryId", "SaleBillYearCode", "SaleBillEntryDate", "Shippingdate", "DistanceKM", "vehicleNo", "TransporterName", "DomesticExportNEPZ", "PaymentCreditDay", "ReceivedAmt", "pendAmount"
                            , "CancelBill", "Canceldate", "CancelBy", "Cancelreason", "BankName", "FreightPaid", "DispatchDelayReason", "AttachmentFilePath1", "AttachmentFilePath2", "AttachmentFilePath3", "DocketNo", "DispatchDelayreson", "Commodity", "CC"
                            , "Uid", "MachineName", "ActualEnteredByName", "ActualEntryDate", "LastUpdatedByName"
                            , "LastUpdationDate", "TypeItemServAssets", "SaleBillJobwork", "PerformaInvNo", "PerformaInvDate", "PerformaInvYearCode"
                            , "BILLAgainstWarrenty", "ExportInvoiceNo", "InvoiceTime", "RemovalDate", "RemovalTime", "EntryFreezToAccounts", "BalanceSheetClosed", "SaleQuotNo", "SaleQuotDate", "salesperson_name", "SalesPersonMobile", "SaleBillEntryFrom"
                            );
                        model.SaleBillDataDashboard = CommonFunc.DataTableToList<SaleBillDashboard>(DT, "SaleBillSummTable");
                    }

                    if (Flag != "True")
                    {
                        model.FromDate1 = fromdate;
                        model.ToDate1 = toDate;
                        model.FinFromDate = fromdate;
                        model.FinToDate = toDate;
                        model.SaleBillNo = saleBillno;
                        model.PartCode = partCode;
                        model.ItemName = itemName;
                        model.AccountName = customerName;
                        model.SONO = sono;
                        model.CustOrderNo = custOrderNo;
                        model.SchNo = schNo;
                        model.PerformaInvNo = performaInvNo;
                        model.SaleQuotNo = saleQuoteNo;
                        model.DomesticExportNEPZ = domensticExportNEPZ;
                        model.SummaryDetail = summaryDetail;
                        model.Searchbox = searchBox;
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
        public async Task<IActionResult> GetSearchData(string summaryDetail, string partCode, string itemName, string saleBillno, string customerName, string sono, string custOrderNo, string schNo, string performaInvNo, string saleQuoteNo, string domensticExportNEPZ, string fromdate, string toDate,string SaleBillEntryFrom, int pageNumber = 1, int pageSize = 50, string SearchBox = "")
        {
            try
            {
                var model = new SaleBillDashboard();
                model.SaleBillYearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
                var Result = await _SaleBill.GetDashboardData(summaryDetail, partCode, itemName, saleBillno, customerName, sono, custOrderNo, schNo, performaInvNo, saleQuoteNo, domensticExportNEPZ, ParseFormattedDate((fromdate).Split(" ")[0]), ParseFormattedDate(toDate.Split(" ")[0]), SaleBillEntryFrom).ConfigureAwait(true);
                if (Result != null)
                {
                    var _List = new List<TextValue>();
                    DataSet DS = Result.Result;
                    if (DS != null)
                    {
                        if (summaryDetail == "Summary")
                        {
                            var DT = DS.Tables[0].DefaultView.ToTable(true, "SaleBillNo", "SaleBillDate", "GSTNO", "AccountCode", "AccountName", "SupplyType", "CustAddress", "StateNameofSupply", "AgainstVoucherNo"
                                    , "CityofSupply", "DocumentHead", "ConsigneeAccountName", "ConsigneeAddress", "PaymentTerm", "Currency", "BillAmt", "TaxableAmt", "GSTAmount", "RoundType", "RoundOffAmt", "INVNetAmt"
                                    , "Ewaybillno", "EInvNo", "EinvGenerated", "CountryOfSupply", "TransporterdocNo", "TransportModeBYRoadAIR", "DispatchTo", "DispatchThrough", "Remark", "Approved", "ApprovDate", "ApprovedBy", "ExchangeRate"
                                    , "SaleBillEntryId", "SaleBillYearCode", "SaleBillEntryDate", "Shippingdate", "DistanceKM", "vehicleNo", "TransporterName", "DomesticExportNEPZ", "PaymentCreditDay", "ReceivedAmt", "pendAmount"
                                    , "CancelBill", "Canceldate", "CancelBy", "Cancelreason", "BankName", "FreightPaid", "DispatchDelayReason", "AttachmentFilePath1", "AttachmentFilePath2", "AttachmentFilePath3", "DocketNo", "DispatchDelayreson", "Commodity", "CC"
                                    , "Uid", "MachineName", "ActualEnteredByName", "ActualEntryDate", "LastUpdatedByName"
                                    , "LastUpdationDate", "TypeItemServAssets", "SaleBillJobwork", "PerformaInvNo", "PerformaInvDate", "PerformaInvYearCode"
                                    , "BILLAgainstWarrenty", "ExportInvoiceNo", "InvoiceTime", "RemovalDate", "RemovalTime", "EntryFreezToAccounts", "BalanceSheetClosed", "SaleQuotNo", "SaleQuotDate", "salesperson_name", "SalesPersonMobile", "SaleBillEntryFrom"
                                    );
                            model.SaleBillDataDashboard = CommonFunc.DataTableToList<SaleBillDashboard>(DT, "SaleBillSummTable");

                            model.SaleBillDataDashboard = model.SaleBillDataDashboard
                                .GroupBy(psd => psd.SaleBillEntryId)
                                .Select(group => group.First())
                                .ToList();

                        }
                        else
                        {
                            var DT = DS.Tables[0].DefaultView.ToTable(true, "SaleBillNo", "SaleBillDate", "CustomerName", "GSTNO", "SupplyType", "CustAddress", "StateNameofSupply"
                                    , "CityofSupply", "DocumentHead", "ConsigneeName", "AgainstVoucherNo","ConsigneeAddress", "ProdSchEntryId", "ProdSchDate", "SchdeliveryDate"
                                    , "PaymentTerm", "currency", "BillAmt", "SONO", "CustOrderNo", "SODate", "SchNo", "Schdate", "SOAmendNo", "SchAmendNo", "SchAmendDate", "PartCode", "ItemName", "CustomerPartCode", "HSNNO"
                                    , "Unit", "NoofCase", "Qty", "Rate", "ItemAmount", "StoreId", "StoreName", "batchno", "uniquebatchno"
                                    , "LotStock", "TotalStock", "RateInOtherCurr", "AltUnit", "AltQty", "SOPendQty", "AltSOPendQty", "AccountName"
                                    , "DiscountPer", "DiscountAmt", "Itemcolor", "ItemSize", "PacketsDetail", "OtherDetail", "ItemRemark", "TaxableAmt"
                                    , "GSTAmount", "RoundType", "RoundOffAmt", "INVNetAmt", "Ewaybillno", "EInvNo", "EinvGenerated", "CountryOfSupply"
                                    , "TransporterdocNo", "TransportModeBYRoadAIR", "DispatchTo", "DispatchThrough", "Remark", "Approved", "ApprovDate", "ApprovedBy"
                                    , "ExchangeRate", "SaleBillEntryId", "SaleBillYearCode", "SaleBillEntryDate", "Shippingdate", "DistanceKM", "vehicleNo"
                                    , "TransporterName", "DomesticExportNEPZ", "PaymentCreditDay", "ReceivedAmt", "pendAmount", "CancelBill", "Canceldate"
                                    , "CancelBy", "Cancelreason", "BankName", "FreightPaid", "DispatchDelayReason", "AttachmentFilePath1", "AttachmentFilePath2"
                                    , "AttachmentFilePath3", "DocketNo", "DispatchDelayreson", "Commodity", "CC", "Uid", "MachineName", "ActualEnteredByName"
                                    , "ActualEntryDate", "LastUpdatedByName", "LastUpdationDate", "TypeItemServAssets", "SaleBillJobwork", "PerformaInvNo",
                                    "AgainstProdPlanNo", "AgainstProdPlanYearCode", "AgaisntProdPlanDate", "GSTPer", "GSTType", "ProdSchno", "CostCenterId", "ProdSchYearcode"
                                    , "PerformaInvDate", "PerformaInvYearCode", "BILLAgainstWarrenty", "ExportInvoiceNo", "InvoiceTime", "RemovalDate", "ProcessId"
                                    , "RemovalTime", "EntryFreezToAccounts", "BalanceSheetClosed", "SaleQuotNo", "SaleQuotDate", "AdviceNo", "AdviceYearCode", "AdviseDate", "AdviseEntryId", "SaleBillEntryFrom"
                                );
                            model.SaleBillDataDashboard = CommonFunc.DataTableToList<SaleBillDashboard>(DT, "SaleBillDetailTable");
                        }
                    }
                }
                if (model.SaleBillDataDashboard != null && model.SaleBillDataDashboard.Any())
                {
                    if (!string.IsNullOrWhiteSpace(SearchBox))
                    {
                        var filteredResults = model.SaleBillDataDashboard
                            .Where(item => item.GetType().GetProperties()
                                .Where(prop => prop.PropertyType == typeof(string))
                                .Select(prop => prop.GetValue(item)?.ToString())
                                .Any(val => !string.IsNullOrEmpty(val) &&
                                            val.Contains(SearchBox, StringComparison.OrdinalIgnoreCase)))
                            .ToList();

                        if (filteredResults.Any())
                        {
                            model.SaleBillDataDashboard = filteredResults;
                        }
                    }

                    model.TotalRecords = model.SaleBillDataDashboard.Count;
                    model.PageNumber = pageNumber;
                    model.PageSize = pageSize;

                    model.SaleBillDataDashboard = model.SaleBillDataDashboard
                        .Skip((pageNumber - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();
                }
                else
                {
                    model.SaleBillDataDashboard = new List<SaleBillDashboard>();
                    model.TotalRecords = 0;
                    model.PageNumber = pageNumber;
                    model.PageSize = pageSize;
                }
                MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                    SlidingExpiration = TimeSpan.FromMinutes(55),
                    Size = 1024,
                };
                if (summaryDetail == "Summary")
                {
                    _MemoryCache.Set("KeySaleBillList_Summary", model.SaleBillDataDashboard, cacheEntryOptions);

                }
                else
                {
                    _MemoryCache.Set("KeySaleBillList_Detail", model.SaleBillDataDashboard, cacheEntryOptions);

                }
                model.SummaryDetail = summaryDetail;
                return PartialView("_SBDashboardGrid", model);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        [HttpGet]
        public IActionResult GlobalSearch(string searchString, string dashboardType = "Summary", int pageNumber = 1, int pageSize = 50)
        {
            SaleBillDashboard model = new SaleBillDashboard();
            if (string.IsNullOrWhiteSpace(searchString))
            {
                return PartialView("_SBDashboardGrid", new List<SaleBillDashboard>());
            }
            string cacheKey = $"KeySaleBillList_{dashboardType}";
            if (!_MemoryCache.TryGetValue(cacheKey, out IList<SaleBillDashboard> saleBillDashboard) || saleBillDashboard == null)
            {
                return PartialView("_SBDashboardGrid", new List<SaleBillDashboard>());
            }

            List<SaleBillDashboard> filteredResults;

            if (string.IsNullOrWhiteSpace(searchString))
            {
                filteredResults = saleBillDashboard.ToList();
            }
            else
            {
                filteredResults = saleBillDashboard
                    .Where(i => i.GetType().GetProperties()
                        .Where(p => p.PropertyType == typeof(string))
                        .Select(p => p.GetValue(i)?.ToString())
                        .Any(value => !string.IsNullOrEmpty(value) &&
                                      value.Contains(searchString, StringComparison.OrdinalIgnoreCase)))
                    .ToList();


                if (filteredResults.Count == 0)
                {
                    filteredResults = saleBillDashboard.ToList();
                }
            }

            model.TotalRecords = filteredResults.Count;
            model.SaleBillDataDashboard = filteredResults.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            model.PageNumber = pageNumber;
            model.PageSize = pageSize;


            return PartialView("_SBDashboardGrid", model);

        }
        public async Task<JsonResult> GetBatchInventory()
        {
            var JSON = await _SaleBill.GetBatchInventory();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> NewEntryId(int YearCode,string SubInvoicetype)
        {
            var JSON = await _SaleBill.NewEntryId(YearCode, SubInvoicetype);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }

        public async Task<JsonResult> GetCompanyType()
        {
            var JSON = await _SaleBill.GetCompanyType();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }


        public async Task<JsonResult> EditableRateAndDiscountONSaleInvoice()
        {
            var JSON = await _SaleBill.EditableRateAndDiscountONSaleInvoice();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }   
        public async Task<JsonResult> GetCustomerBasedDetails(int Code)
        {
            var JSON = await _SaleBill.GetCustomerBasedDetails(Code);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillCurrency(int accountCode)
        {
            var JSON = await _SaleBill.FillCurrency(accountCode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetAutocompleteValue()
        {
            var JSON = await _SaleBill.GetAutocompleteValue();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }

        public IActionResult DeleteItemRow(int SeqNo, string Mode)
        {
            var MainModel = new SaleBillModel();
            if (Mode == "U")
            {
                int Indx = Convert.ToInt32(SeqNo) - 1;
                string modelJson = HttpContext.Session.GetString("KeySaleBillGrid");
                List<SaleBillDetail> saleBillDetail = new List<SaleBillDetail>();
                if (!string.IsNullOrEmpty(modelJson))
                {
                    saleBillDetail = JsonConvert.DeserializeObject<List<SaleBillDetail>>(modelJson);
                }

                if (saleBillDetail != null && saleBillDetail.Count > 0)
                {
                    saleBillDetail.RemoveAt(Convert.ToInt32(Indx));

                    Indx = 0;

                    foreach (var item in saleBillDetail)
                    {
                        Indx++;
                        item.SeqNo = Indx;
                    }
                    MainModel.saleBillDetails = saleBillDetail;

                    HttpContext.Session.SetString("KeySaleBillGrid", JsonConvert.SerializeObject(MainModel.saleBillDetails));
                }
            }
            else
            {
                string modelJson = HttpContext.Session.GetString("KeySaleBillGrid");
                List<SaleBillDetail> saleBillGrid = new List<SaleBillDetail>();
                if (!string.IsNullOrEmpty(modelJson))
                {
                    saleBillGrid = JsonConvert.DeserializeObject<List<SaleBillDetail>>(modelJson);
                }
                int Indx = Convert.ToInt32(SeqNo) - 1;

                if (saleBillGrid != null && saleBillGrid.Count > 0)
                {
                    saleBillGrid.RemoveAt(Convert.ToInt32(Indx));

                    Indx = 0;

                    foreach (var item in saleBillGrid)
                    {
                        Indx++;
                        item.SeqNo = Indx;
                    }
                    MainModel.saleBillDetails = saleBillGrid;

                    HttpContext.Session.SetString("KeySaleBillGrid", JsonConvert.SerializeObject(MainModel.saleBillDetails));
                }
            }

            return PartialView("_SaleBillGrid", MainModel);
        }
        public async Task<JsonResult> EditItemRows(int SeqNo)
        {
            var MainModel = new SaleBillModel();
            string modelJson = HttpContext.Session.GetString("KeySaleBillGrid");
            List<SaleBillDetail> saleBillGrid = new List<SaleBillDetail>();
            if (!string.IsNullOrEmpty(modelJson))
            {
                saleBillGrid = JsonConvert.DeserializeObject<List<SaleBillDetail>>(modelJson);
            }
            var SAGrid = saleBillGrid.Where(x => x.SeqNo == SeqNo);
            string JsonString = JsonConvert.SerializeObject(SAGrid);
            return Json(JsonString);
        }
        public IActionResult AddMultipleItemDetail(List<SaleBillDetail> model)
        {
            try
            {
                var MainModel = new SaleBillModel();
                var StockGrid = new List<SaleBillDetail>();
                var StockAdjustGrid = new List<SaleBillDetail>();

                var SeqNo = 1;
                foreach (var item in model)
                {
                    string modelJson = HttpContext.Session.GetString("KeySaleBillGrid");
                    IList<SaleBillDetail> ItemDetail = new List<SaleBillDetail>();
                    if (!string.IsNullOrEmpty(modelJson))
                    {
                        ItemDetail = JsonConvert.DeserializeObject<List<SaleBillDetail>>(modelJson);
                    }

                    //_MemoryCache.TryGetValue("ItemList", out List<SaleBillDetail> ItemDetail);


                    if (model != null)
                    {
                        if (ItemDetail == null)
                        {
                            item.SeqNo = SeqNo++;
                            StockGrid.Add(item);
                        }
                        else
                        {


                            if (ItemDetail.Where(x => x.ItemCode == item.ItemCode && x.Batchno == item.Batchno && x.Uniquebatchno == item.Uniquebatchno).Any())
                            {
                                return StatusCode(207, "Duplicate");
                            }


                            item.SeqNo = ItemDetail.Count + 1;
                            StockGrid = ItemDetail.Where(x => x != null).ToList();
                            StockAdjustGrid.AddRange(StockGrid);
                            StockGrid.Add(item);
                        }
                        MainModel.ItemDetailGrid = StockGrid;
                        MainModel.saleBillDetails = StockGrid;
                        //MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
                        //{
                        //    AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                        //    SlidingExpiration = TimeSpan.FromMinutes(55),
                        //    Size = 1024,
                        //};

                        HttpContext.Session.SetString("KeySaleBillGrid", JsonConvert.SerializeObject(MainModel.saleBillDetails));
                        
                       
                        HttpContext.Session.SetString("KeySaleBillGrid", JsonConvert.SerializeObject(MainModel.saleBillDetails));
                        HttpContext.Session.SetString("SaleBillModel", JsonConvert.SerializeObject(MainModel));
                    }
                    else
                    {
                        ModelState.TryAddModelError("Error", "Schedule List Cannot Be Empty...!");
                    }
                }


                return PartialView("_SaleBillGrid", MainModel);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IActionResult AddSaleBillDetail(SaleBillDetail model)
        {
            try
            {
                string modelJson = HttpContext.Session.GetString("KeySaleBillGrid");
                IList<SaleBillDetail> SaleBillDetail = new List<SaleBillDetail>();
                if (!string.IsNullOrEmpty(modelJson))
                {
                    SaleBillDetail = JsonConvert.DeserializeObject<List<SaleBillDetail>>(modelJson);
                }
                string SaleBillModelJson = HttpContext.Session.GetString("SaleBillModel");
                SaleBillModel saleBillModel = new SaleBillModel();
                if (!string.IsNullOrEmpty(SaleBillModelJson))
                {
                    saleBillModel = JsonConvert.DeserializeObject<SaleBillModel>(SaleBillModelJson);
                }

                var MainModel = new SaleBillModel();
                var saleBillDetail = new List<SaleBillDetail>();
                var rangeSaleBillGrid = new List<SaleBillDetail>();

                if (model != null)
                {
                    if (SaleBillDetail == null)
                    {
                        model.SeqNo = 1;
                        saleBillDetail.Add(model);
                    }
                    else
                    {
                        if (SaleBillDetail.Any(x => x.ItemCode == model.ItemCode && x.StoreId == model.StoreId && x.Batchno == model.Batchno))
                        {
                            return StatusCode(207, "Duplicate");
                        }

                        model.SeqNo = SaleBillDetail.Count + 1;
                        saleBillDetail = SaleBillDetail.Where(x => x != null).ToList();
                        rangeSaleBillGrid.AddRange(saleBillDetail);
                        saleBillDetail.Add(model);

                    }
                    //MainModel = BindItem4Grid(model);
                    saleBillDetail = saleBillDetail.OrderBy(item => item.SeqNo).ToList();
                    MainModel.saleBillDetails = saleBillDetail;
                    MainModel.ItemDetailGrid = saleBillDetail;

                    HttpContext.Session.SetString("KeySaleBillGrid", JsonConvert.SerializeObject(MainModel.saleBillDetails));
                    //MainModel = BindItem4Grid(MainModel);
                    MainModel.saleBillDetails = saleBillDetail;
                    MainModel.ItemDetailGrid = saleBillDetail;
                    HttpContext.Session.SetString("KeySaleBillGrid", JsonConvert.SerializeObject(MainModel.saleBillDetails));
                    HttpContext.Session.SetString("SaleBillModel", JsonConvert.SerializeObject(MainModel));
                }
                else
                {
                    ModelState.TryAddModelError("Error", "SaleBill List Cannot Be Empty...!");
                }
                return PartialView("_SaleBillGrid", MainModel);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IActionResult ClearDRCRGrid()
        {
            HttpContext.Session.Remove("KeyDrCrGrid");
            return Json("Ok");
        }

        public IActionResult ClearGrid()
        {
            HttpContext.Session.Remove("KeySaleBillGrid");
            HttpContext.Session.Remove("SaleBillModel");
            var MainModel = new SaleBillModel();
            return PartialView("_SaleBillGrid", MainModel);
        }
        public IActionResult ClearCustomerJWGrid()
        {
            HttpContext.Session.Remove("KeyAdjChallanGrid");
            var MainModel = new AdjChallanDetail();
            return PartialView("_CustomerJwisschallanAdjustment", MainModel.CustomerJobWorkIssueAdjustDetails);
        }
        public IActionResult ClearBomCustomerJWGrid()
        {
            HttpContext.Session.Remove("KeyAdjChallanGrid");
            var MainModel = new AdjChallanDetail();
            return PartialView("_BomCustJwisschallanAdjustment", MainModel.BomCustomerJWIssChallanAdj);
        }
        private SaleBillModel BindItem4Grid(SaleBillModel model)
        {
            var _List = new List<DPBItemDetail>();

            string modelJson = HttpContext.Session.GetString("KeySaleBillGrid");
            SaleBillModel MainModel = new SaleBillModel();
            if (!string.IsNullOrEmpty(modelJson))
            {
                MainModel = JsonConvert.DeserializeObject<SaleBillModel>(modelJson);
            }

            _List.Add(
                new DPBItemDetail
                {
                    SeqNo = MainModel.ItemDetailGrid == null ? 1 : MainModel.ItemDetailGrid.Count + 1,
                    docTypeId = 1, //model.docTypeId,
                    DocTypeText = string.Empty, //model.DocTypeText,
                    BillQty = Convert.ToDecimal(model.Qty),

                    Amount = model.Amount,
                    Description = string.Empty, //model.Description,
                    DiscPer = Convert.ToDecimal(model.DiscountPer),
                    DiscRs = model.DiscountAmt,

                    HSNNo = model.HSNNo,
                    ItemCode = model.ItemCode,
                    ItemText = model.ItemName,

                    OtherRateCurr = Convert.ToDecimal(model.RateInOtherCurr),
                    PartCode = model.ItemCode,
                    PartText = model.PartCode,

                    DPBQty = Convert.ToDecimal(model.Qty),
                    Process = model.ProcessId,
                    ProcessName = model.ProcessName,
                    CostCenter = model.CostCenterId,
                    CostCenterName = model.CostCenterName,
                    Rate = Convert.ToDecimal(model.Rate),

                    PONo = string.Empty,//model.pono,
                    POYear = 0,//model.POYear,
                    PODate = string.Empty,//model.PODate,
                    ScheduleNo = model.SchNo,
                    ScheduleYear = model.SaleSchYearCode,
                    ScheduleDate = model.Schdate,

                    Unit = model.Unit,
                });

            if (MainModel.DPBItemDetails == null)
                MainModel.DPBItemDetails = _List;
            else
                MainModel.DPBItemDetails.AddRange(_List);

            MainModel.ItemNetAmount = decimal.Parse(MainModel.DPBItemDetails.Sum(x => x.Amount).ToString("#.#0"));

            return MainModel;
        }
        public static DataTable GetCustomerJobWorkChallanAdjTable(List<CustomerJobWorkChallanAdj> model)
        {
            DataTable Table = new();

            Table.Columns.Add("EntryDate", typeof(string));
            Table.Columns.Add("CustJwRecEntryId", typeof(long));
            Table.Columns.Add("CustJwRecYearCode", typeof(long));
            Table.Columns.Add("CustJwRecChallanNo", typeof(string));
            Table.Columns.Add("CustJwRecEntryDate", typeof(string));
            Table.Columns.Add("RecItemCode", typeof(long));
            Table.Columns.Add("CustJwIssEntryid", typeof(long));
            Table.Columns.Add("CustJwIssYearCode", typeof(long));
            Table.Columns.Add("CustJwIssChallanNo", typeof(string));
            Table.Columns.Add("CustJwIssChallanDate", typeof(string));
            Table.Columns.Add("AccountCode", typeof(long));
            Table.Columns.Add("FinishItemCode", typeof(long));
            Table.Columns.Add("AdjQty", typeof(float));
            Table.Columns.Add("CC", typeof(string));
            Table.Columns.Add("UID", typeof(long));
            Table.Columns.Add("AdjFormType", typeof(string));
            Table.Columns.Add("TillDate", typeof(string));
            Table.Columns.Add("TotIssQty", typeof(float));
            Table.Columns.Add("PendQty", typeof(float));
            Table.Columns.Add("BOMQty", typeof(float));
            Table.Columns.Add("BomRevNo", typeof(long));
            Table.Columns.Add("BOMRevDate", typeof(string));
            Table.Columns.Add("ProcessID", typeof(long));
            Table.Columns.Add("BOMInd", typeof(string));
            Table.Columns.Add("IssQty", typeof(float));
            Table.Columns.Add("TotadjQty", typeof(float));
            Table.Columns.Add("TotalIssQty", typeof(float));
            Table.Columns.Add("TotalRecQty", typeof(float));
            Table.Columns.Add("RunnerItemCode", typeof(long));
            Table.Columns.Add("ScrapItemCode", typeof(long));
            Table.Columns.Add("IdealScrapQty", typeof(float));
            Table.Columns.Add("IssuedScrapQty", typeof(float));
            Table.Columns.Add("PreRecChallanNo", typeof(string));
            Table.Columns.Add("ScrapqtyagainstRcvqty", typeof(float));
            Table.Columns.Add("Recbatchno", typeof(string));
            Table.Columns.Add("Recuniquebatchno", typeof(string));
            Table.Columns.Add("Issbatchno", typeof(string));
            Table.Columns.Add("Issuniquebatchno", typeof(string));
            Table.Columns.Add("ScrapAdjusted", typeof(string));

            if (model != null && model.Count > 0)
            {
                foreach (var Item in model)
                {
                    Table.Rows.Add(
                        new object[]
                        {
                               Item.EntryDate == null ? string.Empty : ParseFormattedDate(Item.EntryDate) ,
                                Item.CustJwRecEntryId ,
                                Item.CustJwRecYearCode ,
                                Item.CustJwRecChallanNo ?? string.Empty,
                                Item.CustJwRecEntryDate == null ? string.Empty : ParseFormattedDate(Item.CustJwRecEntryDate) ,
                                Item.RecItemCode ,
                                Item.CustJwIssEntryid ,
                                Item.CustJwIssYearCode ,
                                Item.CustJwIssChallanNo ?? string.Empty,
                                Item.CustJwIssChallanDate == null ? string.Empty : ParseFormattedDate(Item.CustJwIssChallanDate),
                                Item.AccountCode ,
                                Item.FinishItemCode ,
                                Item.AdjQty ,
                                Item.CC ?? string.Empty,
                                Item.UID ,
                                Item.AdjFormType ?? string.Empty,
                                Item.TillDate == null ? string.Empty : ParseFormattedDate(Item.TillDate) ,
                                Item.TotIssQty ,
                                Item.PendQty ,
                                Item.BOMQty ,
                                Item.BomRevNo ,
                                Item.BOMRevDate == null ? string.Empty : ParseFormattedDate(Item.BOMRevDate) ,
                                Item.ProcessID ,
                                Item.BOMInd ?? string.Empty,
                                Item.IssQty ,
                                Item.TotadjQty ,
                                Item.TotalIssQty ,
                                Item.TotalRecQty ,
                                Item.RunnerItemCode ,
                                Item.ScrapItemCode ,
                                Item.IdealScrapQty ,
                                Item.IssuedScrapQty ,
                                Item.PreRecChallanNo ?? string.Empty,
                                Item.ScrapqtyagainstRcvqty ,
                                Item.Recbatchno ?? string.Empty,
                                Item.Recuniquebatchno ?? string.Empty,
                                Item.Issbatchno ?? string.Empty,
                                Item.Issuniquebatchno ?? string.Empty,
                                Item.ScrapAdjusted ?? string.Empty
                        });
                }
            }
            return Table;
        }

        private static DataTable GetDetailTable(IList<SaleBillDetail> DetailList)
        {
            var DTSSGrid = new DataTable();

            DTSSGrid.Columns.Add("SeqNo", typeof(int));
            DTSSGrid.Columns.Add("SONO", typeof(int));
            DTSSGrid.Columns.Add("CustOrderNo", typeof(string));
            DTSSGrid.Columns.Add("SOYearCode", typeof(int));
            DTSSGrid.Columns.Add("SODate", typeof(string));
            DTSSGrid.Columns.Add("SchNo", typeof(string));
            DTSSGrid.Columns.Add("SchDate", typeof(string));
            DTSSGrid.Columns.Add("SaleSchYearCode", typeof(int));
            DTSSGrid.Columns.Add("SOAmendNo", typeof(string));
            DTSSGrid.Columns.Add("SOAmendDate", typeof(string));
            DTSSGrid.Columns.Add("SchAmendDate", typeof(string));
            DTSSGrid.Columns.Add("ItemCode", typeof(int));
            DTSSGrid.Columns.Add("HSNNO", typeof(string));
            DTSSGrid.Columns.Add("Unit", typeof(string));
            DTSSGrid.Columns.Add("NoofCase", typeof(float));
            DTSSGrid.Columns.Add("Qty", typeof(float));
            DTSSGrid.Columns.Add("UnitOfRate", typeof(string));
            DTSSGrid.Columns.Add("RateInOtherCurr", typeof(float));
            DTSSGrid.Columns.Add("Rate", typeof(float));
            DTSSGrid.Columns.Add("AltUnit", typeof(string));
            DTSSGrid.Columns.Add("AltQty", typeof(float));
            DTSSGrid.Columns.Add("ItemWeight", typeof(float));
            DTSSGrid.Columns.Add("NoofPcs", typeof(int));
            DTSSGrid.Columns.Add("CustomerPartCode", typeof(string));
            DTSSGrid.Columns.Add("MRP", typeof(float));
            DTSSGrid.Columns.Add("OriginalMRP", typeof(float));
            DTSSGrid.Columns.Add("SOPendQty", typeof(float));
            DTSSGrid.Columns.Add("AltSOPendQty", typeof(int));
            DTSSGrid.Columns.Add("DisountPer", typeof(float));
            DTSSGrid.Columns.Add("DiscountAmt", typeof(float));
            DTSSGrid.Columns.Add("ItemSize", typeof(string));
            DTSSGrid.Columns.Add("Itemcolor", typeof(string));
            DTSSGrid.Columns.Add("StoreId", typeof(int));
            DTSSGrid.Columns.Add("ItemAmount", typeof(float));
            DTSSGrid.Columns.Add("AdviceNo", typeof(string));
            DTSSGrid.Columns.Add("AdviseEntryId", typeof(int));
            DTSSGrid.Columns.Add("AdviceYearCode", typeof(int));
            DTSSGrid.Columns.Add("AdviseDate", typeof(string));
            DTSSGrid.Columns.Add("ProcessId", typeof(int));
            DTSSGrid.Columns.Add("batchno", typeof(string));
            DTSSGrid.Columns.Add("uniquebatchno", typeof(string));
            DTSSGrid.Columns.Add("LotStock", typeof(float));
            DTSSGrid.Columns.Add("TotalStock", typeof(float));
            DTSSGrid.Columns.Add("AgainstProdPlanNo", typeof(string));
            DTSSGrid.Columns.Add("AgainstProdPlanYearCode", typeof(int));
            DTSSGrid.Columns.Add("AgaisntProdPlanDate", typeof(string));
            DTSSGrid.Columns.Add("GSTPer", typeof(float));
            DTSSGrid.Columns.Add("GSTType", typeof(string));
            DTSSGrid.Columns.Add("PacketsDetail", typeof(string));
            DTSSGrid.Columns.Add("OtherDetail", typeof(string));
            DTSSGrid.Columns.Add("ItemRemark", typeof(string));
            DTSSGrid.Columns.Add("prodSchno", typeof(string));
            DTSSGrid.Columns.Add("ProdSchYearcode", typeof(int));
            DTSSGrid.Columns.Add("prodSchEntryId", typeof(int));
            DTSSGrid.Columns.Add("ProdSchDate", typeof(string));
            DTSSGrid.Columns.Add("SchdeliveryDate", typeof(string));
            DTSSGrid.Columns.Add("CostCenterid", typeof(int));
            DTSSGrid.Columns.Add("ProdUnProduced", typeof(string));
            DTSSGrid.Columns.Add("BOMInd", typeof(string));
            DTSSGrid.Columns.Add("BomNo", typeof(int));
            DTSSGrid.Columns.Add("CustJWmanadatory", typeof(string));
            DTSSGrid.Columns.Add("StockableNonStockable", typeof(string));
            DTSSGrid.Columns.Add("ItemGroupCode", typeof(int));

            //DateTime DeliveryDt = new DateTime();
            foreach (var Item in DetailList)
            {
                string uniqueString = Guid.NewGuid().ToString();
                DTSSGrid.Rows.Add(
                    new object[]
                    {
                    Item.SeqNo,
                    Item.SONO,
                    Item.CustOrderNo ?? string.Empty,
                    Item.SOYearCode,
                    //Item.SODate == null ? string.Empty : (Item.SODate.Split(" ")[0]),
                    Item.SODate == null ? string.Empty : common.CommonFunc.ParseFormattedDate(Item.SODate.Split(" ")[0]),
                    Item.SchNo ?? "",
                    Item.Schdate == null ? string.Empty : common.CommonFunc.ParseFormattedDate(Item.Schdate.Split(" ")[0]),
                    Item.SaleSchYearCode,
                    Item.SOAmendNo ?? string.Empty,
                    Item.SOAmendDate == null ? string.Empty : common.CommonFunc.ParseFormattedDate(Item.SOAmendDate.Split(" ")[0]),
                    Item.SchAmendDate == null ? string.Empty : common.CommonFunc.ParseFormattedDate(Item.SchAmendDate.Split(" ")[0]),
                    Item.ItemCode,
                    Item.HSNNo.ToString() ?? string.Empty,
                    Item.Unit ?? string.Empty,
                    Item.NoofCase,
                    Item.Qty,
                    Item.UnitOfRate ?? string.Empty,
                    Item.RateInOtherCurr,
                    Item.Rate,
                    Item.AltUnit ?? string.Empty,
                    Item.AltQty,
                    Item.ItemWeight,
                    Item.NoofPcs,
                    Item.CustomerPartCode ?? string.Empty,
                    Item.MRP,
                    Item.OriginalMRP,
                    Item.SOPendQty,
                    Item.AltSOPendQty,
                    Item.DiscountPer,
                    Item.DiscountAmt,
                    Item.ItemSize ?? string.Empty,
                    Item.Itemcolor ?? string.Empty,
                    Item.StoreId,
                    Item.Amount,
                    Item.AdviceNo ?? string.Empty,
                    Item.AdviseEntryId,
                    Item.AdviceYearCode,
                    Item.AdviseDate == null ? string.Empty : common.CommonFunc.ParseFormattedDate(Item.AdviseDate.Split(" ")[0]),
                    Item.ProcessId,
                    Item.Batchno ?? string.Empty,
                    Item.Uniquebatchno ?? string.Empty,
                    Item.LotStock,
                    Item.TotalStock,
                    Item.AgainstProdPlanNo ?? string.Empty,
                    Item.AgainstProdPlanYearCode,
                    Item.AgaisntProdPlanDate == null ? string.Empty : common.CommonFunc.ParseFormattedDate(Item.AgaisntProdPlanDate.Split(" ")[0]),
                    Item.GSTPer,
                    Item.GSTType ?? string.Empty,
                    Item.PacketsDetail ?? string.Empty,
                    Item.OtherDetail ?? string.Empty,
                    Item.ItemRemark ?? string.Empty,
                    Item.ProdSchno ?? string.Empty,
                    Item.ProdSchYearcode,
                    Item.ProdSchEntryId,
                    Item.ProdSchDate == null ? string.Empty : common.CommonFunc.ParseFormattedDate(Item.ProdSchDate.Split(" ")[0]),
                    Item.SchdeliveryDate == null ? string.Empty : common.CommonFunc.ParseFormattedDate(Item.SchdeliveryDate.Split(" ")[0]),
                    Item.CostCenterId,
                    Item.ProducedUnprod ?? string.Empty,
                    Item.BOMInd ?? string.Empty,
                    Item.BomNo,
                    Item.CustJwAdjustmentMandatory ?? string.Empty,
                    Item.StockableNonStockable ?? string.Empty,
                    Item.Group_Code == null ? 0 : Item.Group_Code,
                    });
            }
            DTSSGrid.Dispose();
            return DTSSGrid;
        }

        public async Task<JsonResult> GetDistance(int accountCode)
        {
            var JSON = await _SaleBill.GetDistance(accountCode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillCustomerList(string SBJobwork, string ShowAllCustomer)
        {
            var JSON = await _SaleBill.FillCustomerList(SBJobwork, ShowAllCustomer);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillJWCustomerList(string SBJobwork, int yearCode)
        {
            var JSON = await _SaleBill.FillJWCustomerList(SBJobwork, yearCode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillDocument(string ShowAllDoc)
        {
            var JSON = await _SaleBill.FillDocument(ShowAllDoc);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FILLSOScheduleDate(string sono, int accountCode, int soYearCode, string schNo, int schYearCode)
        {
            var JSON = await _SaleBill.FILLSOScheduleDate(sono, accountCode, soYearCode, schNo, schYearCode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillSONO(string billDate, string accountCode, string billType)
        {
            var JSON = await _SaleBill.FillSONO(billDate, accountCode, billType);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillSOPendQty(int saleBillEntryId, string saleBillNo, int saleBillYearCode, string sono, int soYearcode, string custOrderNo, int itemCode, int accountCode, string schNo, int schYearCode)
        {
            var JSON = await _SaleBill.FillSOPendQty(saleBillEntryId, saleBillNo, saleBillYearCode, sono, soYearcode, custOrderNo, itemCode, accountCode, schNo, soYearcode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillConsigneeList(string showAllConsignee)
        {
            var JSON = await _SaleBill.FillConsigneeList(showAllConsignee);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillSOYearCode(string sono, string accountCode)
        {
            var JSON = await _SaleBill.FillSOYearCode(sono, accountCode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
    

        public async Task<JsonResult> DisplaySODetail(string accountName, string itemName, string partCode, string sono, int soYearCode, string custOrderNo, string schNo, int schYearCode)
        {
            var JSON = await _SaleBill.DisplaySODetail(accountName, itemName, partCode, sono, soYearCode, custOrderNo, schNo, schYearCode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillItems(string showAll, string TypeItemServAssets, string sbJobWork,string SearchItemCode, string SearchPartCode)
        {
            var JSON = await _SaleBill.FillItems(showAll, TypeItemServAssets, sbJobWork ,SearchItemCode, SearchPartCode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillSOWiseItems(string invoiceDate, string sono, int soYearCode, int accountCode, string schNo, int schYearCode, string sbJobWork)
        {
            invoiceDate = ParseFormattedDate(invoiceDate);
            var JSON = await _SaleBill.FillSOWiseItems(invoiceDate, sono, soYearCode, accountCode, schNo, schYearCode, sbJobWork);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> JWItemList(string typeItemServAssets, string showAll, string bomInd, string schNo, int schYearCode)
        {
            var JSON = await _SaleBill.JWItemList(typeItemServAssets, showAll, bomInd, schNo, schYearCode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillStore()
        {
            var JSON = await _SaleBill.FillStore();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillTransporter()
        {
            var JSON = await _SaleBill.FillTransporter();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillSOItemRate(string sono, int soYearCode, int accountCode, string custOrderNo, int itemCode)
        {
            var JSON = await _SaleBill.FillSOItemRate(sono, soYearCode, accountCode, custOrderNo, itemCode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FILLCustomerOrderAndSPDate(string billDate, int accountCode, string sono, int soYearCode)
        {
            billDate = (billDate);
            var JSON = await _SaleBill.FILLCustomerOrderAndSPDate(billDate, accountCode, sono, soYearCode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> AllowTaxPassword()
        {
            var JSON = await _SaleBill.AllowTaxPassword();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillSOSchedule(string sono, string accountCode, int soYearCode, int ItemCode)
        {
            var JSON = await _SaleBill.FillSOSchedule(sono, accountCode, soYearCode, ItemCode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public IActionResult PrintReport(int EntryId, int YearCode = 0, string Type = "", string InvoiceNo = "", int AccountCode = 0)
        {
            var ReportData = _SaleBill.GetReportData(EntryId, YearCode, Type, InvoiceNo, AccountCode);

            string my_connection_string;
            string webRootPath = _IWebHostEnvironment.WebRootPath;
            List<WebReport> reports = new List<WebReport>();
            string[] copyTypes = { "Original", "Duplicate", "Triplicate", "Office Copy" };
            foreach (var copyType in copyTypes)
            {
                var webReport = new WebReport();
                webReport.Report.Clear();
                var ReportName = _SaleBill.GetReportName();
                webReport.Report.Dispose();
                webReport.Report = new Report();
                //if (!String.Equals(ReportName.Result.Result.Rows[0].ItemArray[0], System.DBNull.Value))
                //{
                //    webReport.Report.Load(webRootPath + "\\" + ReportName.Result.Result.Rows[0].ItemArray[0]); // from database
                //}
                //else
                //{
                  
                //}
                webReport.Report.Load(webRootPath + "\\SaleEstimate.frx"); // default report
                webReport.Report.SetParameterValue("entryparam", EntryId);
                webReport.Report.SetParameterValue("yearparam", YearCode);
                my_connection_string = _connectionStringService.GetConnectionString();
                //my_connection_string = _iconfiguration.GetConnectionString("eTactDB");
                webReport.Report.Dictionary.Connections[0].ConnectionString = my_connection_string;
                webReport.Report.Dictionary.Connections[0].ConnectionStringExpression = "";
                webReport.Report.SetParameterValue("MyParameter", my_connection_string);
                //webReport.Report.SetParameterValue("copyType", copyType);
                webReport.Report.Dictionary.Connections[0].ConnectionString = my_connection_string;
                //webReport.Report.SetParameterValue("copyType", copyType);
                webReport.Report.Prepare();
                foreach (var dataSource in webReport.Report.Dictionary.DataSources)
                {
                    if (dataSource is TableDataSource tableDataSource)
                    {
                        tableDataSource.Enabled = true;
                        tableDataSource.Init(); // Refresh the data source
                    }
                }
                webReport.Report.Refresh();
                reports.Add(webReport);
            }
            return View(reports);

            //Additional CODE STARTS
            // Create 4 copies with tags
            //string[] tags = { "Original", "Duplicate", "Triplicate", "Office Copy" };
            //var preparedPages = new List<ReportPage>();
            //foreach (var tag in tags)
            //{
            //    // Set the tag value
            //    webReport.Report.SetParameterValue("CopyTag", tag);
            //    // Append pages for this copy
            //    using (var tempReport = new Report())
            //    {
            //        tempReport.Load("path-to-your-report.frx");
            //        tempReport.Prepare();
            //        preparedPages.AddRange(tempReport.Pages);
            //    }
            //}
            //// Combine all copies into one print job
            //foreach (var page in preparedPages)
            //{
            //    webReport.Report.Pages.Add(page);
            //}
            ////Additional CODE END HERE



        }
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
                     Item.TxAmount != null ? Math.Round((decimal)Item.TxAmount, 2) : 0,
                    Item.TxRefundable,
                    Item.TxOnExp,
                    Item.TxRemark,
                    });
            }

            return Table;
        }

        public async Task<IActionResult> DeleteByID(int ID, int YC, string entryByMachineName, string partCode = "", string itemName = "", string saleBillno = "", string customerName = "", int sono = 0, string custOrderNo = "", string schNo = "", string performaInvNo = "", string saleQuoteNo = "", string domensticExportNEPZ = "", string fromdate = "", string toDate = "", string Searchbox = "")
        {
            var Result = await _SaleBill.DeleteByID(ID, YC, entryByMachineName).ConfigureAwait(false);

            //if (result.statustext == "deleted" || result.statuscode == httpstatuscode.gone || result.statustext == "success")
            //{
            //    viewbag.issuccess = true;
            //    tempdata["410"] = "410";

            //    tempdata["deletemessage"] = result.statustext;

            //}
            //else
            //{
            //    viewbag.issuccess = false;
            //    tempdata["500"] = "500";
            //}
            if (Result.StatusText == "Success" || Result.StatusText == "deleted" || Result.StatusCode == HttpStatusCode.Gone)
            {
                ViewBag.isSuccess = true;
                TempData["410"] = "410";
            }
            else if (Result.StatusText == "Error" || Result.StatusCode == HttpStatusCode.Accepted)
            {
                ViewBag.isSuccess = true;
                TempData["423"] = "423";
                TempData["DeleteMessage"] = Result.StatusText;

            }
            else
            {
                ViewBag.isSuccess = false;
                TempData["500"] = "500";

            }
            return RedirectToAction("SBDashboard", new { Flag = "False", ItemName = itemName, PartCode = partCode, saleBillno = saleBillno, customerName = customerName, sono = sono, custOrderNo = custOrderNo, schNo = schNo, performaInvNo = performaInvNo, saleQuoteNo = saleQuoteNo, domensticExportNEPZ = domensticExportNEPZ, fromdate = fromdate, todate = toDate, searchBox = Searchbox });
        }
        private async Task<string> GenerateQRCodeImage(string qrText, string filePath)
        {
            try
            {
                // Example using Zint barcode generator
                string tempInputPath = Path.GetTempFileName();
                await System.IO.File.WriteAllTextAsync(tempInputPath, qrText);

                string zintPath = @"C:\Program Files (x86)\Zint\zint.exe";
                if (!System.IO.File.Exists(zintPath))
                    return "Zint not found";

                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = zintPath,
                        Arguments = $"-b 58 -o \"{filePath}\" -i \"{tempInputPath}\"",
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                };

                process.Start();
                await process.WaitForExitAsync();

                if (!System.IO.File.Exists(filePath))
                    return "Failed";

                return "Success";
            }
            catch
            {
                return "Error";
            }
        }

        public async Task<IActionResult> GenerateInvoice([FromBody] EInvoiceItemModel input)
        {
            try
            {
                if (input == null)
                    return BadRequest("Invalid input");

                var duplicateIRNResult = await _IEinvoiceService.CheckDuplicateIRN(
                    input.EntryId,
                    input.InvoiceNo,
                    input.YearCode
                );
                var token = await _IEinvoiceService.GetAccessTokenAsync();

                var result = await _IEinvoiceService.CreateIRNAsync(
                    token,
                    input.EntryId,
                    input.InvoiceNo,
                    input.YearCode,
                    input.saleBillType,
                    input.customerPartCode,
                    input.transporterName,
                    input.vehicleNo,
                    input.distanceKM,
                    input.EntrybyId,
                    input.MachineName,
                    "Sale Bill",
                    input.generateEway,
                    "SaleBillEInvoice"
                );
                var responseObj = JObject.FromObject(result.Result);
                var rawResponse = responseObj["rawResponse"] as JObject;

                if (rawResponse == null)
                    return BadRequest("Invalid raw response format.");

                var eInvoiceStr = rawResponse["eInvoiceResponse"]?.ToString();
                if (string.IsNullOrWhiteSpace(eInvoiceStr))
                    return BadRequest("Missing eInvoice response.");

                JObject eInvoiceObj;
                try
                {
                    eInvoiceObj = JObject.Parse(eInvoiceStr);
                }
                catch (Exception ex)
                {
                    return Ok(new
                    {
                        qrCodeUrl = "",
                        ewbPdfUrl = "",
                        rawEInvoice = rawResponse["eInvoiceResponse"]?.ToString(),
                        rawEWayBill = rawResponse["eWayBillResponse"]?.ToString()
                    });
                    //  return BadRequest("Failed to parse eInvoice JSON: " + ex.Message);
                }
                string uploadsFolder = Path.Combine(_IWebHostEnvironment.WebRootPath, "Uploads", "QRCode");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);
                string signedQrText = eInvoiceObj["results"]?["message"]?["SignedQRCode"]?.ToString();

                string fileName = $"{Guid.NewGuid()}.png";
                string outputPath = Path.Combine(uploadsFolder, fileName);

                var qrResult = await GenerateQRCodeImage(signedQrText, outputPath);
                if (qrResult != "Success")
                    return BadRequest("QR generation failed");

                string publicUrl = $"{Request.Scheme}://{Request.Host}/Uploads/QRCode/{fileName}";
                if (input.generateEway == "EInvoice")
                {
                    return Ok(new
                    {
                        qrCodeUrl = publicUrl,
                        ewbPdfUrl = "",
                        rawEInvoice = rawResponse["eInvoiceResponse"]?.ToString(),
                        rawEWayBill = rawResponse["eWayBillResponse"]?.ToString()
                    });
                }
                //   var ewayInvoiceStr = rawResponse["eWayBillResponse"]?.ToString();
                string ewayInvoiceStr = rawResponse["eWayBillResponse"]?.ToString();

                if (string.IsNullOrWhiteSpace(ewayInvoiceStr))
                    return BadRequest("Missing eInvoice response.");

                int jsonStart = ewayInvoiceStr.IndexOf("responseString:");
                if (jsonStart == -1)
                    return BadRequest("Invalid format: 'responseString' not found.");

                string jsonPart = ewayInvoiceStr.Substring(jsonStart + "responseString:".Length).Trim();

                JObject eInvoiceObj1;
                try
                {
                    eInvoiceObj1 = JObject.Parse(jsonPart);
                }
                catch (Exception ex)
                {
                    //  return BadRequest("Failed to parse eInvoice JSON: " + ex.Message);
                    return Ok(new
                    {
                        qrCodeUrl = publicUrl,
                        ewbPdfUrl = "",
                        rawEInvoice = eInvoiceObj.ToString(Formatting.Indented),
                        rawEWayBill = rawResponse["eWayBillResponse"]?.ToString()
                    });
                }
                string ewbPdfUrl = eInvoiceObj1["results"]?["message"]?["EwaybillPdf"]?.ToString() ?? "";

                //  string ewbPdfUrl = rawResponse["PDF"]?.ToString() ?? "";

                return Ok(new
                {
                    qrCodeUrl = publicUrl,
                    ewbPdfUrl,
                    rawEInvoice = eInvoiceObj.ToString(Formatting.Indented),
                    rawEWayBill = rawResponse["eWayBillResponse"]?.ToString()
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Server Error: {ex.Message}");
            }
        }
        //        var responseObj = result.Result as JObject;


        //        //if (!string.IsNullOrWhiteSpace(ewbUrl))
        //        //{
        //        if (responseObj != null)
        //        {
        //            if (input.generateEway == "EInvoice")
        //            {
        //                string ewbUrl = (string)responseObj["ewbUrl"];
        //                string uploadsFolder = Path.Combine(_IWebHostEnvironment.WebRootPath, "Uploads", "QRCode");
        //                if (!Directory.Exists(uploadsFolder))
        //                    Directory.CreateDirectory(uploadsFolder);

        //                string fileName = $"{Guid.NewGuid()}.png";
        //                string outputPath = Path.Combine(uploadsFolder, fileName);

        //                var qrResult = await GenerateQRCodeImage(ewbUrl, outputPath);
        //                if (qrResult != "Success")
        //                    return BadRequest("QR generation failed");

        //                string publicUrl = $"{Request.Scheme}://{Request.Host}/Uploads/QRCode/{fileName}";
        //                return Ok(new
        //                {
        //                    redirectUrl = publicUrl,
        //                    rawResponse = (string)responseObj["rawResponse"]
        //                });
        //          //      return Ok(new { redirectUrl = publicUrl });
        //            }
        //            //return Ok(new { redirectUrl = ewbUrl }); ;

        //                return Ok(new
        //                {
        //                    redirectUrl = (string)responseObj["ewbUrl"],
        //                    rawResponse = (string)responseObj["rawResponse"]
        //                });
        //            }
        //        //}
        //        else
        //        {
        //            return BadRequest("Invoice generation failed");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, $"Server Error: {ex.Message}");
        //    }
        //}
        public ResponseResult isDuplicate(string ColVal, string ColName)
        {
            var Result = _IDataLogic.isDuplicate(ColVal, ColName, "SaleBillMain");
            return Result;
        }

        [HttpPost]
        public JsonResult AutoComplete(string ColumnName, string prefix)
        {
            var iList = _IDataLogic.AutoComplete("SaleBillMain", ColumnName, "", "", 0, 0);
            var Result = (from item in iList
                          where item.Text.Contains(prefix)
                          select new
                          {
                              item.Text
                          }).Distinct().ToList();

            return Json(Result);
        }
        public async Task<JsonResult> CheckFinYearBeforeSave(int YearCode, string Date, string DateName)
        {
            var JSON = await _ICommon.CheckFinYearBeforeSave(YearCode, Date, DateName);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetMaxSaleInvoiceEntryDate(int YearCode)
        {
            var JSON = await _SaleBill.GetMaxSaleInvoiceEntryDate(YearCode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetFeatureOption()
        {
            var JSON = await _SaleBill.GetFeatureOption();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
    }
}
