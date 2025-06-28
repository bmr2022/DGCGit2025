using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using static eTactWeb.DOM.Models.Common;
using static eTactWeb.Data.Common.CommonFunc;
using System.Data;
using System.Net;
using System.Globalization;
using Microsoft.AspNetCore.Http;
using NuGet.Packaging;
using System.Runtime.Caching;
using System.Reflection;
using Newtonsoft.Json.Linq;
using System.Diagnostics;

namespace eTactWeb.Controllers
{
    public class PurchaseRejectionController : Controller
    {
        private readonly IPurchaseRejection _purchRej;
        private readonly ILogger<PurchaseRejectionController> _logger;
        private readonly IDataLogic _IDataLogic;
        private readonly IMemoryCache _MemoryCache;
        public readonly IEinvoiceService _IEinvoiceService;
        public IWebHostEnvironment _IWebHostEnvironment { get; }

        public PurchaseRejectionController(IPurchaseRejection purchRej, IDataLogic iDataLogic, IWebHostEnvironment IWebHostEnvironment, ILogger<PurchaseRejectionController> logger, IMemoryCache memoryCache, IEinvoiceService IEinvoiceService)
        {
            _purchRej = purchRej;
            _IDataLogic = iDataLogic;
            _IWebHostEnvironment = IWebHostEnvironment;
            _logger = logger;
            _MemoryCache = memoryCache;
            _IEinvoiceService = IEinvoiceService;
        }

        [HttpGet]
        [Route("{controller}/Index")]
        public async Task<IActionResult> PurchaseRejection(int ID, string Mode, int YC, string FromDate = "", string ToDate = "", string VendorName = "", string VoucherNo = "", string InvoiceNo = "", string PartCode = "", string Searchbox = "")
        {
            AccPurchaseRejectionModel model = new AccPurchaseRejectionModel();
            ViewData["Title"] = "Purchase Rejection Details";
            TempData.Clear();
            HttpContext.Session.Remove("KeyPurchaseRejectionGrid");
            HttpContext.Session.Remove("PurchaseRejectionModel");
            HttpContext.Session.Remove("KeyAdjGrid");
            HttpContext.Session.Remove("KeyTaxGrid");
            HttpContext.Session.Remove("KeyPurchaseRejectionPopupGrid");
            // var model = await BindModel(MainModel);

            if (Mode != "U")
            {
                model.Uid = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                model.ActualEnteredBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                model.ActualEnteredByName = GetEmpByMachineName();
                model.ActualEntryDate = HttpContext.Session.GetString("ActualEntryDate") ?? ParseFormattedDate(DateTime.Today.ToString("dd/MM/yyyy"));
                model.MachineName = GetEmpByMachineName();
            }
            else
            {
                model.ActualEnteredByName = GetEmpByMachineName();
                model.UpdatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                model.LastUpdatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                model.LastUpdatedByName = GetEmpByMachineName();
                model.LastUpdationDate = HttpContext.Session.GetString("LastUpdatedDate");
                model.UpdatedOn = ParseSafeDate(HttpContext.Session.GetString("LastUpdatedDate"));
                model.MachineName = GetEmpByMachineName();
            }

            if (!string.IsNullOrEmpty(Mode) && ID > 0 && (Mode == "V" || Mode == "U"))
            {
                model = await _purchRej.GetViewByID(ID, YC, Mode);
                model.Mode = Mode;
                model.ID = ID;
                model.DocAccountCode = Convert.ToInt32(model.AccPurchaseRejectionDetails?.FirstOrDefault().DocAccountCode ?? 0);
                model.DocAccountName = model.AccPurchaseRejectionDetails?.FirstOrDefault().DocAccountName;
                
                model.FinFromDate = model.FinFromDate ?? HttpContext.Session.GetString("FromDate");
                model.FinToDate = model.FinToDate ?? HttpContext.Session.GetString("ToDate");
                model.PurchaseRejYearCode = model.PurchaseRejYearCode != null && model.PurchaseRejYearCode > 0 ? model.PurchaseRejYearCode : Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
                model.CC = model.CC ?? HttpContext.Session.GetString("Branch");
            }
            else
            {
                model.FinFromDate = HttpContext.Session.GetString("FromDate");
                model.FinToDate = HttpContext.Session.GetString("ToDate");
                model.PurchaseRejYearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
                model.CC = HttpContext.Session.GetString("Branch");
            }

             MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
             {
                 AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                 SlidingExpiration = TimeSpan.FromMinutes(55),
                 Size = 1024,
             };

            model.FromDateBack = FromDate;
            model.ToDateBack = ToDate;
            model.VendorNameBack = VendorName != null && VendorName != "0" && VendorName != "undefined" ? VendorName : "0";
            model.VoucherNoBack = VoucherNo != null && VoucherNo != "0" && VoucherNo != "undefined" ? VoucherNo : "0";
            model.InvoiceNoBack = InvoiceNo != null && InvoiceNo != "0" && InvoiceNo != "undefined" ? InvoiceNo : "0";
            model.PartCodeBack = PartCode != null && PartCode != "0" && PartCode != "undefined" ? PartCode : "0";
            model.GlobalSearchBack = Searchbox != null && Searchbox != "0" && Searchbox != "undefined" ? Searchbox : "";

            HttpContext.Session.SetString("PurchaseRejection", JsonConvert.SerializeObject(model));
            string serializedPurchaseRejectionGrid = JsonConvert.SerializeObject(model.AccPurchaseRejectionDetails);
            string serializedPurchaseRejectionModelGrid = JsonConvert.SerializeObject(model);
            string serializedPurchaseRejectionAgainstModelGrid = JsonConvert.SerializeObject(model.AccPurchaseRejectionAgainstBillDetails == null ? new List<AccPurchaseRejectionAgainstBillDetail>() : model.AccPurchaseRejectionAgainstBillDetails);
            string serializedKeyAdjGrid = JsonConvert.SerializeObject(model.adjustmentModel == null ? new AdjustmentModel() : model.adjustmentModel);
            string serializedKeyTaxGrid = JsonConvert.SerializeObject(model.TaxDetailGridd == null ? new List<TaxModel>() : model.TaxDetailGridd);
            string serializedKeyDbCrGrid = JsonConvert.SerializeObject(model.DbCrGrid == null ? new List<DbCrModel>() : model.DbCrGrid);
            HttpContext.Session.SetString("KeyPurchaseRejectionGrid", serializedPurchaseRejectionGrid);
            HttpContext.Session.SetString("PurchaseRejectionModel", serializedPurchaseRejectionModelGrid);
            HttpContext.Session.SetString("KeyPurchaseRejectionPopupGrid", serializedPurchaseRejectionAgainstModelGrid);
            HttpContext.Session.SetString("KeyAdjGrid", serializedKeyAdjGrid);
            HttpContext.Session.SetString("KeyTaxGrid", serializedKeyTaxGrid);
            if (!string.IsNullOrEmpty(Mode) && ID > 0 && (Mode == "V" || Mode == "U"))
            {
                HttpContext.Session.SetString("KeyDrCrGrid", serializedKeyDbCrGrid);
            }
            return View(model);
        }

        public async Task<JsonResult> GetCostCenter()
        {
            var JSON = await _purchRej.GetCostCenter();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }

        [HttpPost]
        [Route("{controller}/Index")]
        public async Task<IActionResult> PurchaseRejection(AccPurchaseRejectionModel model,string ShouldEinvoice)
        {
            try
            {
                var PRGrid = new DataTable();
                DataTable DTAgainstBillDetail = new();
                DataTable TaxDetailDT = null;
                DataTable AdjDetailDT = null;
                DataTable DrCrDetailDT = null;

                string modelJson = HttpContext.Session.GetString("PurchaseRejectionModel");
                string modelPRJson = HttpContext.Session.GetString("KeyPurchaseRejectionPopupGrid");
                string modelPRGridJson = HttpContext.Session.GetString("KeyPurchaseRejectionGrid");
                string modelTaxJson = HttpContext.Session.GetString("KeyTaxGrid");
                string modelDrCrJson = HttpContext.Session.GetString("KeyDrCrGrid");
                string modelAdjJson = HttpContext.Session.GetString("KeyAdjGrid");
                AccPurchaseRejectionModel MainModel = new AccPurchaseRejectionModel();
                List<AccPurchaseRejectionAgainstBillDetail> PurchaseRejectionAgainstBillDetail = new List<AccPurchaseRejectionAgainstBillDetail>();
                List<AccPurchaseRejectionDetail> PurchaseRejectionDetail = new List<AccPurchaseRejectionDetail>();
                List<TaxModel> TaxGrid = new List<TaxModel>();
                List<DbCrModel> DrCrGrid = new List<DbCrModel>();
                AdjustmentModel AdjGrid = new AdjustmentModel();
                if (!string.IsNullOrEmpty(modelJson))
                {
                    MainModel = JsonConvert.DeserializeObject<AccPurchaseRejectionModel>(modelJson);
                }
                if (!string.IsNullOrEmpty(modelPRJson))
                {
                    PurchaseRejectionAgainstBillDetail = JsonConvert.DeserializeObject<List<AccPurchaseRejectionAgainstBillDetail>>(modelPRJson);
                }
                if (!string.IsNullOrEmpty(modelPRGridJson))
                {
                    PurchaseRejectionDetail = JsonConvert.DeserializeObject<List<AccPurchaseRejectionDetail>>(modelPRGridJson);
                }
                if (!string.IsNullOrEmpty(modelTaxJson))
                {
                    TaxGrid = JsonConvert.DeserializeObject<List<TaxModel>>(modelTaxJson);
                }
                if (!string.IsNullOrEmpty(modelDrCrJson))
                {
                    DrCrGrid = JsonConvert.DeserializeObject<List<DbCrModel>>(modelDrCrJson);
                }
                if (!string.IsNullOrEmpty(modelAdjJson))
                {
                    AdjGrid = JsonConvert.DeserializeObject<AdjustmentModel>(modelAdjJson);
                }

                if (PurchaseRejectionDetail == null)
                {
                    ModelState.Clear();
                    ModelState.TryAddModelError("PurchaseRejectionDetail", "Purchase Rejection Grid Should Have Atleast 1 Item...!");
                    return View("PurchaseRejection", model);
                }
                else if (TaxGrid == null)
                {
                    ModelState.Clear();
                    ModelState.TryAddModelError("TaxDetail", "Tax Grid Should Have Atleast 1 Item...!");
                    return View("PurchaseRejection", model);
                }
                else
                {
                    model.CC = HttpContext.Session.GetString("Branch");
                    model.Uid = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                    if (model.Mode != "U")
                    {
                        model.Uid = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                        model.ActualEnteredBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                        model.ActualEnteredByName = GetEmpByMachineName();
                        model.ActualEntryDate = HttpContext.Session.GetString("ActualEntryDate") ?? ParseFormattedDate(DateTime.Today.ToString("dd/MM/yyyy"));
                        model.MachineName = GetEmpByMachineName();
                        PRGrid = GetDetailTable(PurchaseRejectionDetail, MainModel);
                    }
                    else
                    {
                        model.ActualEnteredByName = GetEmpByMachineName();
                        model.UpdatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                        model.LastUpdatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                        model.LastUpdatedByName = GetEmpByMachineName();
                        model.LastUpdationDate = HttpContext.Session.GetString("LastUpdatedDate");
                        model.UpdatedOn = ParseSafeDate(HttpContext.Session.GetString("LastUpdatedDate"));
                        model.MachineName = GetEmpByMachineName();
                        PRGrid = GetDetailTable(PurchaseRejectionDetail, MainModel);
                    }

                    if (TaxGrid != null && TaxGrid.Count > 0)
                    {
                        TaxDetailDT = GetTaxDetailTable(TaxGrid);
                    }

                    if (DTAgainstBillDetail != null)
                    {
                        DTAgainstBillDetail = GetAgainstDetailTable(PurchaseRejectionAgainstBillDetail, MainModel);
                    }
                    if (DrCrGrid != null && DrCrGrid.Count > 0)
                    {

                        DrCrDetailDT = CommonController.GetDrCrDetailTable(DrCrGrid);
                    }

                    if ((MainModel.adjustmentModel != null && MainModel.adjustmentModel.AdjAdjustmentDetailGrid != null && MainModel.adjustmentModel.AdjAdjustmentDetailGrid.Count > 0) || (AdjGrid != null && AdjGrid.AdjAdjustmentDetailGrid != null && AdjGrid.AdjAdjustmentDetailGrid.Count > 0))
                    {
                        if (MainModel.adjustmentModel != null && MainModel.adjustmentModel.AdjAdjustmentDetailGrid.Any())
                        {
                            AdjDetailDT = CommonController.GetAdjDetailTable(MainModel.adjustmentModel.AdjAdjustmentDetailGrid.ToList(), model.PurchaseRejEntryId, model.PurchaseRejYearCode, model.AccountCode);
                        }
                        if (AdjGrid != null && AdjGrid.AdjAdjustmentDetailGrid.Any())
                        {
                            AdjDetailDT = CommonController.GetAdjDetailTable(AdjGrid.AdjAdjustmentDetailGrid.ToList(), model.PurchaseRejEntryId, model.PurchaseRejYearCode, model.AccountCode);
                        }
                    }

                    if (model.PathOfFile1 != null)
                    {
                        string ImagePath = "Uploads/PurchaseRejection/";

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
                        string ImagePath = "Uploads/PurchaseRejection/";

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
                        string ImagePath = "Uploads/PurchaseRejection/";

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


                    var Result = await _purchRej.SavePurchaseRejection(model, PRGrid, TaxDetailDT, DrCrDetailDT, AdjDetailDT, DTAgainstBillDetail);

                    if (Result != null)
                    {
                        if (Result.StatusText == "Success" && Result.StatusCode == HttpStatusCode.OK)
                        {
                            ViewBag.isSuccess = true;
                            TempData["200"] = "200";

                            var model1 = new SaleBillModel();
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
                            //_MemoryCache.Remove("KeyPurchaseRejectionGrid");
                            //_MemoryCache.Remove("PurchaseRejectionModel");
                            if (ShouldEinvoice == "true")
                            {
                                return Json(new
                                {
                                    status = "Success",
                                    EntryId = model.PurchaseRejEntryId,
                                    InvoiceNo = model.PurchaseRejectionVoucherNo,
                                    YearCode = model.PurchaseRejYearCode,
                                    saleBillType = model.PurchaseBillType,
                                    customerPartCode = model.PartCode,
                                    transporterName = model.Transporter,
                                    vehicleNo = model.Vehicleno,
                                    distanceKM = model.Distance,
                                    EntrybyId = model.EntryByempId,
                                    MachineName = model.MachineName

                                });
                            }
                            HttpContext.Session.Remove("KeyPurchaseRejectionGrid");
                            HttpContext.Session.Remove("PurchaseRejectionModel");

                            //return View(model1);
                         //   return RedirectToAction(nameof(PurchaseRejection), new { Id = 0, Mode = "", YC = 0 });
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
                            //_MemoryCache.Remove("KeyPurchaseRejectionGrid");
                            //_MemoryCache.Remove("PurchaseRejectionModel");
                            if (ShouldEinvoice == "true")
                            {
                                return Json(new
                                {
                                    status = "Success",
                                    EntryId = model.PurchaseRejEntryId,
                                    InvoiceNo = model.PurchaseRejectionVoucherNo,
                                    YearCode = model.PurchaseRejYearCode,
                                    saleBillType = model.PurchaseBillType,
                                    customerPartCode = model.PartCode,
                                    transporterName = model.Transporter,
                                    vehicleNo = model.Vehicleno,
                                    distanceKM = model.Distance,
                                    EntrybyId = model.EntryByempId,
                                    MachineName = model.MachineName

                                });
                            }
                            HttpContext.Session.Remove("KeyPurchaseRejectionGrid");
                            HttpContext.Session.Remove("PurchaseRejectionModel");
                         //   return View(model1);
                        }
                        if (Result.StatusText == "Error" && Result.StatusCode == HttpStatusCode.InternalServerError)
                        {
                            var errNum = string.Empty;
                            if (Result != null)
                            {
                                if (Result.Result != null)
                                {
                                    if (Result.Result.Rows.Count > 0)
                                    {
                                        errNum = Result?.Result?.Rows.Message?.ToString().Split(":")[1];
                                    }
                                }
                            }
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
                            // return View("Error", Result);
                            return View(model);
                        }
                        HttpContext.Session.SetString("PurchaseRejection", JsonConvert.SerializeObject(model));
                    }
                    return Json(new { status = "Success" });
                    //   return View(model);
                }
            }
            catch (Exception ex)
            {
                LogException<PurchaseRejectionController>.WriteException(_logger, ex);


                var _ResponseResult = new ResponseResult()
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    StatusText = "Error",
                    Result = ex
                };

                return View("Error", _ResponseResult);
                //return View(model);
            }
        }
        private static DataTable GetDetailTable(IList<AccPurchaseRejectionDetail> DetailList, AccPurchaseRejectionModel model)
        {
            var DTSSGrid = new DataTable();

            DTSSGrid.Columns.Add("PurchaseRejEntryId", typeof(int));
            DTSSGrid.Columns.Add("PurchaseRejYearCode", typeof(int));
            DTSSGrid.Columns.Add("DebitNotePurchaseRejection", typeof(string));
            DTSSGrid.Columns.Add("DocAccountCode", typeof(int));
            DTSSGrid.Columns.Add("Itemcode", typeof(int));
            DTSSGrid.Columns.Add("HSNNo", typeof(string));
            DTSSGrid.Columns.Add("RejectedQty", typeof(float));
            DTSSGrid.Columns.Add("Unit", typeof(string));
            DTSSGrid.Columns.Add("AltQty", typeof(float));
            DTSSGrid.Columns.Add("AltUnit", typeof(string));
            DTSSGrid.Columns.Add("RejRate", typeof(float));
            DTSSGrid.Columns.Add("AltRate", typeof(float));
            DTSSGrid.Columns.Add("NoOfCase", typeof(float));
            DTSSGrid.Columns.Add("DiscountPer", typeof(float));
            DTSSGrid.Columns.Add("DiscountAmt", typeof(float));
            DTSSGrid.Columns.Add("StoreId", typeof(int));
            DTSSGrid.Columns.Add("Stockable", typeof(string));
            DTSSGrid.Columns.Add("BatchNo", typeof(string));
            DTSSGrid.Columns.Add("UniqueBatchNo", typeof(string));
            DTSSGrid.Columns.Add("LotStock", typeof(float));
            DTSSGrid.Columns.Add("TotalStock", typeof(float));
            DTSSGrid.Columns.Add("ItemAmount", typeof(float));
            DTSSGrid.Columns.Add("UnitRate", typeof(string));
            DTSSGrid.Columns.Add("PurchBillQty", typeof(float));
            DTSSGrid.Columns.Add("PurchBillRate", typeof(float));
            DTSSGrid.Columns.Add("CostcenetrId", typeof(int));
            DTSSGrid.Columns.Add("itemSize", typeof(string));
            DTSSGrid.Columns.Add("ItemDescription", typeof(string));
            DTSSGrid.Columns.Add("Remark", typeof(string));

            foreach (var Item in DetailList)
            {
                string uniqueString = Guid.NewGuid().ToString();
                DTSSGrid.Rows.Add(
                    new object[]
                    {
                        //1,
                        //2025,
                        (model != null ? model.PurchaseRejEntryId : 0),
                        (model != null ? model.PurchaseRejYearCode : 0),
                        (model != null ? model.DebitNotePurchaseRejection : string.Empty),
                        Item.DocAccountCode,
                        Item.ItemCode,
                        Item.HSNNo ?? string.Empty,
                        Item.RejectedQty,
                        Item.Unit ?? string.Empty,
                        Item.AltQty,
                        Item.AltUnit ?? string.Empty,
                        Item.PRRate,
                        Item.AltRate,
                        Item.NoOfCase,
                        Item.DiscountPer,
                        Item.DiscountAmt,
                        Item.StoreId,
                        string.Empty,
                        Item.Batchno, //BatchNo check once
                        Item.Uniquebatchno, //UniqueBatchNo check once
                        Item.LotStock,
                        Item.TotalStock,
                        Item.ItemAmount,
                        Item.UnitRate ?? string.Empty,
                        Item.BillQty,
                        Item.BillRate,
                        Item.CostCenterId,
                        Item.ItemSize ?? string.Empty,
                        Item.ItemDescription ?? string.Empty,
                        Item.Remark ?? string.Empty
                    });
            }
            DTSSGrid.Dispose();
            return DTSSGrid;
        }
        private static DataTable GetAgainstDetailTable(IList<AccPurchaseRejectionAgainstBillDetail> DetailList, AccPurchaseRejectionModel model)
        {
            var DTSSGrid = new DataTable();

            DTSSGrid.Columns.Add("PurchaseRejEntryId", typeof(int));
            DTSSGrid.Columns.Add("PurchaseRejYearCode", typeof(int));
            DTSSGrid.Columns.Add("PurchaseRejInvoiceNo", typeof(string));
            DTSSGrid.Columns.Add("PurchaseRejVoucherNo", typeof(string));
            DTSSGrid.Columns.Add("AgainstPurchasebillBillNo", typeof(string));
            DTSSGrid.Columns.Add("AgainstPurchaseBillYearCode", typeof(int));
            DTSSGrid.Columns.Add("AgainstPurchaseBilldate", typeof(string));
            DTSSGrid.Columns.Add("AgainstPurchaseBillEntryId", typeof(int));
            DTSSGrid.Columns.Add("AgainstPurchaseVoucherNo", typeof(string));
            DTSSGrid.Columns.Add("PurchaseBilltype", typeof(string));
            DTSSGrid.Columns.Add("DebitNoteItemCode", typeof(int));
            DTSSGrid.Columns.Add("BillItemCode", typeof(int));
            DTSSGrid.Columns.Add("BillQty", typeof(float));
            DTSSGrid.Columns.Add("Unit", typeof(string));
            DTSSGrid.Columns.Add("AltQty", typeof(float));
            DTSSGrid.Columns.Add("AltUnit", typeof(string));
            DTSSGrid.Columns.Add("BillRate", typeof(float));
            DTSSGrid.Columns.Add("DiscountPer", typeof(float));
            DTSSGrid.Columns.Add("DiscountAmt", typeof(float));
            DTSSGrid.Columns.Add("Itemsize", typeof(string));
            DTSSGrid.Columns.Add("Amount", typeof(float));
            DTSSGrid.Columns.Add("PONO", typeof(string));
            DTSSGrid.Columns.Add("PODate", typeof(string));
            DTSSGrid.Columns.Add("POEntryId", typeof(int));
            DTSSGrid.Columns.Add("POYearCode", typeof(int));
            DTSSGrid.Columns.Add("PoRate", typeof(float));
            DTSSGrid.Columns.Add("poammno", typeof(string));
            DTSSGrid.Columns.Add("BatchNo", typeof(string));
            DTSSGrid.Columns.Add("UniqueBatchNo", typeof(string));
            DTSSGrid.Columns.Add("BillAmount", typeof(float));

            foreach (var Item in DetailList)
            {
                string uniqueString = Guid.NewGuid().ToString();
                DTSSGrid.Rows.Add(
                    new object[]
                    {
                     (model != null ? model.PurchaseRejEntryId : 0),
                     (model != null ? model.PurchaseRejYearCode : 0),
                    Item.PurchaseRejectionInvoiceNo ?? string.Empty,
                    Item.PurchaseRejectionVoucherNo ?? string.Empty,
                    Item.InvoiceNo ?? (Item.AgainstPurchaseBillBillNo ?? string.Empty), //Item.AgainstPurchaseBillBillNo
                    Item.AgainstPurchaseBillYearCode ?? 0,
                    Item.InvoiceDate == null ? (Item.AgainstPurchaseBillDate == null ? string.Empty : ParseFormattedDate(Item.AgainstPurchaseBillDate.Split(" ")[0])) : ParseFormattedDate(Item.InvoiceDate.Split(" ")[0]), //Item.AgainstPurchaseBillDate
                    Item.AgainstPurchaseBillEntryId ?? 0,
                    Item.AgainstPurchaseVoucherNo ?? string.Empty,
                    Item.PurchaseBillType ?? string.Empty,
                    Item.ItemCode, //DebitNoteItemCode
                    Item.PurchBillItemCode, // BillItemCode
                    Item.BillQty ?? 0,
                    Item.Unit ?? string.Empty,
                    Item.AltQty ?? 0,
                    Item.AltUnit ?? string.Empty,
                    Item.BillRate ?? 0,
                    Item.DiscountPer ?? 0,
                    Item.DiscountAmt ?? 0,
                    Item.ItemSize ?? string.Empty,
                    (Item.Amount > 0) ? Item.Amount : 0,
                    Item.PONO ?? string.Empty,
                    Item.PODate == null ? string.Empty :ParseFormattedDate(Item.PODate.Split(" ")[0]),
                    Item.POEntryId ?? 0,
                    Item.POYearCode ?? 0,
                    Item.PoRate ?? 0,
                    Item.PoAmmNo ?? string.Empty,
                    Item.BatchNo ?? string.Empty,
                    Item.UniqueBatchNo ?? string.Empty,
                    Item.BillAmount
                    });
            }
            DTSSGrid.Dispose();
            return DTSSGrid;
        }
        private static DataTable GetTaxDetailTable(List<TaxModel> TaxDetailList)
        {
            DataTable Table = new();
            Table.Columns.Add("SeqNo", typeof(int));
            Table.Columns.Add("Type", typeof(string));
            Table.Columns.Add("ItemCode", typeof(int));
            Table.Columns.Add("TaxTypeID", typeof(int));
            Table.Columns.Add("TaxAccountCode", typeof(int));
            Table.Columns.Add("TaxPercentg", typeof(decimal));
            Table.Columns.Add("AddInTaxable", typeof(string));
            Table.Columns.Add("RountOff", typeof(string));
            Table.Columns.Add("Amount", typeof(decimal));
            Table.Columns.Add("TaxRefundable", typeof(string));
            Table.Columns.Add("TaxOnExp", typeof(decimal));
            Table.Columns.Add("Remark", typeof(string));

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
        [Route("{controller}/Dashboard")]
        public async Task<IActionResult> PRDashBoard(string FromDate = "", string ToDate = "", string VendorName = "", string VoucherNo = "", string InvoiceNo = "", string PartCode = "", string Searchbox = "", string Flag = "True")
        {
            HttpContext.Session.Remove("PurchaseBill");
            HttpContext.Session.Remove("TaxGrid");
            HttpContext.Session.Remove("KeyTaxGrid");

            var _List = new List<TextValue>();

            var MainModel = await _purchRej.GetDashBoardData();
            AccPurchaseRejectionDashboard model = new AccPurchaseRejectionDashboard();
            DateTime now = DateTime.Now;
            DateTime firstDayOfMonth = new DateTime(now.Year, now.Month, 1);
            DateTime today = DateTime.Now;
            var commonparams = new Dictionary<string, object>()
        {
            { "@fromBilldate", ParseFormattedDate(firstDayOfMonth.ToString()) },
            { "@ToBilldate", ParseFormattedDate(today.ToString()) }
        };
            MainModel = await BindDashboardList(MainModel, commonparams);
            MainModel.FromDate = new DateTime(DateTime.Today.Year, 4, 1).ToString("dd/MM/yyyy").Replace("-", "/");
            MainModel.ToDate = new DateTime(DateTime.Today.Year + 1, 3, 31).ToString("dd/MM/yyyy").Replace("-", "/");// Last day in January next year
            if (Flag != "True")
            {
                MainModel.FromDate = FromDate;
                MainModel.ToDate = ToDate;
                MainModel.VendorName = VendorName != null && VendorName != "0" && VendorName != "undefined" ? VendorName : "0";
                MainModel.VoucherNo = VoucherNo != null && VoucherNo != "0" && VoucherNo != "undefined" ? VoucherNo : "0";
                MainModel.InvoiceNo = InvoiceNo != null && InvoiceNo != "0" && InvoiceNo != "undefined" ? InvoiceNo : "0";
                MainModel.PartCode = PartCode != null && PartCode != "0" && PartCode != "undefined" ? PartCode : "0";
                MainModel.Searchbox = Searchbox != null && Searchbox != "0" && Searchbox != "undefined" ? Searchbox : "";
            }
            return View(MainModel);
        }
        public async Task<IActionResult> GetSearchData(AccPurchaseRejectionDashboard model, int pageNumber = 1, int pageSize = 5, string SearchBox = "")
        {
            model = await _purchRej.GetSearchData(model);
            //model.DashboardType = "Summary";
            var modelList = model?.PurchaseRejectionDashboard ?? new List<AccPurchaseRejectionDashboard>();


            if (string.IsNullOrWhiteSpace(SearchBox))
            {
                model.TotalRecords = modelList.Count();
                model.PageNumber = pageNumber;
                model.PageSize = pageSize;
                model.PurchaseRejectionDashboard = modelList
                .Skip((pageNumber - 1) * pageSize)
                   .Take(pageSize)
                   .ToList();
            }
            else
            {
                List<AccPurchaseRejectionDashboard> filteredResults;
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
                model.PurchaseRejectionDashboard = filteredResults.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
                model.PageNumber = pageNumber;
                model.PageSize = pageSize;
            }
            MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                SlidingExpiration = TimeSpan.FromMinutes(55),
                Size = 1024,
            };

            _MemoryCache.Set("KeyPurchaseRejectionlList", modelList, cacheEntryOptions);
            return PartialView("_PRDashBoardGrid", model);
        }
        public IActionResult GlobalSearch(string searchString, int pageNumber = 1, int pageSize = 50)
        {
            AccPurchaseRejectionDashboard model = new AccPurchaseRejectionDashboard();
            if (string.IsNullOrWhiteSpace(searchString))
            {
                return PartialView("_PRDashBoardGrid", new List<AccPurchaseRejectionDashboard>());
            }
            string cacheKey = $"KeyPurchaseRejectionlList";
            if (!_MemoryCache.TryGetValue(cacheKey, out IList<AccPurchaseRejectionDashboard> purchaseRejectionDashboard) || purchaseRejectionDashboard == null)
            {
                return PartialView("_PRDashBoardGrid", new List<AccPurchaseRejectionDashboard>());
            }

            List<AccPurchaseRejectionDashboard> filteredResults;

            if (string.IsNullOrWhiteSpace(searchString))
            {
                filteredResults = purchaseRejectionDashboard.ToList();
            }
            else
            {
                filteredResults = purchaseRejectionDashboard
                    .Where(i => i.GetType().GetProperties()
                        .Where(p => p.PropertyType == typeof(string))
                        .Select(p => p.GetValue(i)?.ToString())
                        .Any(value => !string.IsNullOrEmpty(value) &&
                                      value.Contains(searchString, StringComparison.OrdinalIgnoreCase)))
                    .ToList();


                if (filteredResults.Count == 0)
                {
                    filteredResults = purchaseRejectionDashboard.ToList();
                }
            }

            model.TotalRecords = filteredResults.Count;
            model.PurchaseRejectionDashboard = filteredResults.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            model.PageNumber = pageNumber;
            model.PageSize = pageSize;

            return PartialView("_PRDashBoardGrid", model);
        }
        public async Task<AccPurchaseRejectionDashboard> BindDashboardList(AccPurchaseRejectionDashboard MainModel, Dictionary<string, object> commonparams)
        {
            //var docnameparams = new Dictionary<string, object>() { { "@flag", "FillDocumentDASHBOARD" } };
            //docnameparams.AddRange(commonparams);
            //MainModel.DocumentNameList = await _IDataLogic.GetDropDownListWithCustomeVar("AccSPPurchaseRejectionMainDetail", docnameparams, true);
            MainModel.VendorNameList = new List<TextValue>();
            MainModel.VoucherNoList = new List<TextValue>();
            MainModel.InvoiceNoList = new List<TextValue>();
            MainModel.PartCodeList = new List<TextValue>();
            var vendornameparams = new Dictionary<string, object>() { { "@Flag", "FILLVENDORNAMEASHBOARD" } };
            vendornameparams.AddRange(commonparams);
            MainModel.VendorNameList = await _IDataLogic.GetDropDownListWithCustomeVar("AccSPPurchaseRejectionMainDetail", vendornameparams, true);

            var vouchnoparams = new Dictionary<string, object>() { { "@Flag", "FILLVOUCHERDASHBOARD" } };
            vouchnoparams.AddRange(commonparams);
            MainModel.VoucherNoList = await _IDataLogic.GetDropDownListWithCustomeVar("AccSPPurchaseRejectionMainDetail", vouchnoparams, false, false);

            var invparams = new Dictionary<string, object>() { { "@Flag", "FILLINVOICEDASHBOARD" } };
            invparams.AddRange(commonparams);
            MainModel.InvoiceNoList = await _IDataLogic.GetDropDownListWithCustomeVar("AccSPPurchaseRejectionMainDetail", invparams, true, false);

            var partcodeparams = new Dictionary<string, object>() { { "@Flag", "FILLPartCodeDASHBOARD" } };
            partcodeparams.AddRange(commonparams);
            //MainModel.PartCodeList = await _IDataLogic.GetDropDownListWithCustomeVar("AccSPPurchaseRejectionMainDetail", partcodeparams, false, false);
            MainModel.PartCodeList = await _IDataLogic.GetDropDownListWithCustomeVar("AccSPPurchaseRejectionMainDetail", partcodeparams, true);
            return MainModel;
        }
        public async Task<JsonResult> NewEntryId(int YearCode)
        {
            var JSON = await _purchRej.NewEntryId(YearCode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillDocument(string ShowAllDoc)
        {
            var JSON = await _purchRej.FillDocument(ShowAllDoc);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillCustomerName(string ShowAllParty, int? PurchaseRejYearCode, string DebitNotePurchaseRejection)
        {
            var JSON = await _purchRej.FillCustomerName(ShowAllParty, PurchaseRejYearCode, DebitNotePurchaseRejection);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetStateGST(int Code)
        {
            var JSON = await _purchRej.GetStateGST(Code);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillItems(int YearCode, int accountCode, string showAllItems, string Flag)
        {
            var JSON = await _purchRej.FillItems(YearCode, accountCode, showAllItems, Flag);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillCurrency(int? AccountCode)
        {
            var JSON = await _purchRej.FillCurrency(AccountCode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetExchangeRate(string Currency)
        {
            var JSON = await _purchRej.GetExchangeRate(Currency);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillStore()
        {
            var JSON = await _purchRej.FillStore();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillSubvoucher(int? PurchaseRejYearCode, string DebitNotePurchaseRejection)
        {
            var JSON = await _purchRej.FillSubvoucher(PurchaseRejYearCode, DebitNotePurchaseRejection);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetHSNUNIT(int itemCode)
        {
            var JSON = await _purchRej.GetHSNUNIT(itemCode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillPurchaseRejectionPopUp(string DebitNotePurchaseRejection, string fromBillDate, string toBillDate, int itemCode, int accountCode, int yearCode, string showAllBill)
        {
            fromBillDate = ParseFormattedDate(fromBillDate);
            toBillDate = ParseFormattedDate(toBillDate);
            var JSON = await _purchRej.FillPurchaseRejectionPopUp(DebitNotePurchaseRejection, fromBillDate, toBillDate, itemCode, accountCode, yearCode, showAllBill);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillDetailFromPopupGrid(List<AccPurchaseRejectionAgainstBillDetail> model, int itemCode, int popCt)
        {
            List<AccPurchaseRejectionAgainstBillDetail> purchaseRejectionPopupGrid = new List<AccPurchaseRejectionAgainstBillDetail>();
            string modelPRJson = HttpContext.Session.GetString("KeyPurchaseRejectionPopupGrid");
            if (!string.IsNullOrEmpty(modelPRJson))
            {
                purchaseRejectionPopupGrid = JsonConvert.DeserializeObject<List<AccPurchaseRejectionAgainstBillDetail>>(modelPRJson);
            }
            if (model != null && purchaseRejectionPopupGrid != null)
            {
                if (purchaseRejectionPopupGrid.Any())
                {
                    model.AddRange(purchaseRejectionPopupGrid.Where(p => !model.Any(m => m.ItemCode == p.ItemCode && m.PurchBillItemCode == p.PurchBillItemCode && m.AgainstPurchaseBillEntryId == p.AgainstPurchaseBillEntryId && m.AgainstPurchaseBillYearCode == p.AgainstPurchaseBillYearCode && m.InvoiceNo == p.InvoiceNo)).ToList());
                }
            }
            var dataGrid = GetDetailFromPopup(model);
            var JSON = await _purchRej.FillDetailFromPopupGrid(dataGrid, itemCode, popCt);
            string JsonString = JsonConvert.SerializeObject(JSON);
            MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                SlidingExpiration = TimeSpan.FromMinutes(55),
                Size = 1024,
            };
            //_MemoryCache.Set("KeyPurchaseRejectionPopupGrid", model, cacheEntryOptions);
            HttpContext.Session.SetString("KeyPurchaseRejectionPopupGrid", JsonConvert.SerializeObject(model));
            return Json(JsonString);
        }
        private static DataTable GetDetailFromPopup(List<AccPurchaseRejectionAgainstBillDetail> List)
        {
            try
            {
                DataTable table = new();
                table.Columns.Add("PurchaseRejEntryId", typeof(int));
                table.Columns.Add("PurchaseRejYearCode", typeof(int));
                table.Columns.Add("PurchaseRejectionInvoiceNo", typeof(string));
                table.Columns.Add("PurchaseRejectionVoucherNo", typeof(string));
                table.Columns.Add("AgainstPurchasebillBillNo", typeof(string));
                table.Columns.Add("AgainstPurchaseBillYearCode", typeof(int));
                table.Columns.Add("AgainstPurchaseBilldate", typeof(string));
                table.Columns.Add("AgainstPurchaseBillEntryId", typeof(int));
                table.Columns.Add("AgainstPurchaseVoucherNo", typeof(string));
                table.Columns.Add("PurchaseBilltype", typeof(string));
                table.Columns.Add("CreditNoteItemCode", typeof(int));
                table.Columns.Add("BillItemCode", typeof(int));
                table.Columns.Add("BillQty", typeof(float));
                table.Columns.Add("Unit", typeof(string));
                table.Columns.Add("AltQty", typeof(float));
                table.Columns.Add("AltUnit", typeof(string));
                table.Columns.Add("BillRate", typeof(float));
                table.Columns.Add("DiscountPer", typeof(float));
                table.Columns.Add("DiscountAmt", typeof(float));
                table.Columns.Add("Itemsize", typeof(string));
                table.Columns.Add("Amount", typeof(float));
                table.Columns.Add("PONO", typeof(string));
                table.Columns.Add("PODate", typeof(string));
                table.Columns.Add("POEntryId", typeof(int));
                table.Columns.Add("POYearCode", typeof(int));
                table.Columns.Add("PoRate", typeof(float));
                table.Columns.Add("poammno", typeof(string));
                table.Columns.Add("BatchNo", typeof(string));
                table.Columns.Add("UniqueBatchNo", typeof(string));
                table.Columns.Add("BillAmount", typeof(float));

                foreach (AccPurchaseRejectionAgainstBillDetail Item in List)
                {
                    table.Rows.Add(
                        new object[]
                        {
                    1,
                    DateTime.Now.Year,
                    Item.PurchaseRejectionInvoiceNo ?? string.Empty,
                   Item.PurchaseRejectionVoucherNo ?? string.Empty,
                   Item.InvoiceNo ?? (Item.AgainstPurchaseBillBillNo ?? string.Empty), //AgainstPurchaseBillBillNo
                   Item.AgainstPurchaseBillYearCode,
                   Item.InvoiceDate == null ? (Item.AgainstPurchaseBillDate == null ? string.Empty : ParseFormattedDate(Item.AgainstPurchaseBillDate.Split(" ")[0])) : ParseFormattedDate(Item.InvoiceDate.Split(" ")[0]), //InvoiceDate
                   Item.AgainstPurchaseBillEntryId,
                   Item.AgainstPurchaseVoucherNo ?? string.Empty,
                   Item.PurchaseBillType ?? string.Empty,
                   Item.ItemCode,// CreditNoteItemCode
                   Item.PurchBillItemCode, //BillItemCode,
                   Item.BillQty,
                   Item.Unit ?? string.Empty,
                   Item.AltQty,
                   Item.AltUnit ?? string.Empty,
                   Item.BillRate,
                   Item.DiscountPer,
                   Item.DiscountAmt,
                   Item.ItemSize ?? string.Empty,
                   Item.Amount,
                   Item.PONO ?? string.Empty,
                   Item.PODate == null ? string.Empty : ParseFormattedDate(Item.PODate),
                   Item.POEntryId,
                   Item.POYearCode,
                   Item.PoRate,
                   Item.PoAmmNo ?? string.Empty,
                   Item.BatchNo ?? string.Empty,
                   Item.UniqueBatchNo ?? string.Empty,
                   Item.BillAmount
                        });
                }

                return table;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public IActionResult AddPurchaseRejectionDetail(AccPurchaseRejectionDetail model)
        {
            try
            {
                //_MemoryCache.TryGetValue("KeyPurchaseRejectionGrid", out IList<AccPurchaseRejectionDetail> purchaseRejectionGrid);
                //_MemoryCache.TryGetValue("PurchaseRejectionModel", out AccPurchaseRejectionModel purchaseRejectionModel);
                //_MemoryCache.TryGetValue("SaleBillModel", out AccCreditNoteModel saleBillModel);
                string modelPRJson = HttpContext.Session.GetString("PurchaseRejectionModel");
                string modelPRGridJson = HttpContext.Session.GetString("KeyPurchaseRejectionGrid");
                AccPurchaseRejectionModel purchaseRejectionModel = new AccPurchaseRejectionModel();
                IList<AccPurchaseRejectionDetail> purchaseRejectionGrid = new List<AccPurchaseRejectionDetail>();
                if (!string.IsNullOrEmpty(modelPRJson))
                {
                    purchaseRejectionModel = JsonConvert.DeserializeObject<AccPurchaseRejectionModel>(modelPRJson);
                }
                if (!string.IsNullOrEmpty(modelPRGridJson))
                {
                    purchaseRejectionGrid = JsonConvert.DeserializeObject<List<AccPurchaseRejectionDetail>>(modelPRGridJson);
                }

                var MainModel = new AccPurchaseRejectionModel();
                var purchaseRejectionDetail = new List<AccPurchaseRejectionDetail>();
                var rangeSaleBillGrid = new List<AccPurchaseRejectionDetail>();

                if (model != null)
                {
                    if (purchaseRejectionGrid == null)
                    {
                        model.SeqNo = 1;
                        purchaseRejectionDetail.Add(model);
                    }
                    else
                    {
                        if (purchaseRejectionGrid.Any(x => x.DocAccountCode == model.DocAccountCode && x.ItemCode == model.ItemCode && x.StoreId == model.StoreId && x.Batchno == model.Batchno))
                        {
                            return StatusCode(207, "Duplicate");
                        }

                        model.SeqNo = purchaseRejectionGrid.Count + 1;
                        purchaseRejectionDetail = purchaseRejectionGrid.Where(x => x != null).ToList();
                        purchaseRejectionDetail.Add(model);

                    }
                    //MainModel = BindItem4Grid(model);
                    //saleBillDetail = saleBillDetail.OrderBy(item => item.SeqNo).ToList();
                    MainModel.AccPurchaseRejectionDetails = purchaseRejectionDetail;
                    MainModel.ItemDetailGrid = purchaseRejectionDetail;
                    MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
                    {
                        AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                        SlidingExpiration = TimeSpan.FromMinutes(55),
                        Size = 1024,
                    };
                    //_MemoryCache.Set("KeyPurchaseRejectionGrid", MainModel.AccPurchaseRejectionDetails, cacheEntryOptions);
                    HttpContext.Session.SetString("KeyPurchaseRejectionGrid", JsonConvert.SerializeObject(MainModel.AccPurchaseRejectionDetails));
                    MainModel.AccPurchaseRejectionDetails = purchaseRejectionDetail;
                    MainModel.ItemDetailGrid = purchaseRejectionDetail;
                    //_MemoryCache.Set("PurchaseRejectionModel", MainModel, cacheEntryOptions);
                    HttpContext.Session.SetString("PurchaseRejectionModel", JsonConvert.SerializeObject(MainModel));
                    MainModel.AccPurchaseRejectionDetails = purchaseRejectionDetail;
                    //MainModel.ItemDetailGrid = saleBillDetail;
                    //_MemoryCache.Set("SaleBillModel", MainModel, cacheEntryOptions);
                }
                else
                {
                    ModelState.TryAddModelError("Error", "Purchase rejection Cannot Be Empty...!");
                }
                return PartialView("_PurchaseRejectionGrid", MainModel);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<JsonResult> EditItemRows(int itemCode, int Seq)
        {
            var MainModel = new AccPurchaseRejectionModel();
            string modelPRJson = HttpContext.Session.GetString("KeyPurchaseRejectionPopupGrid");
            string modelPRGridJson = HttpContext.Session.GetString("KeyPurchaseRejectionGrid");
            List<AccPurchaseRejectionAgainstBillDetail> purchaseRejectionPopupGrid = new List<AccPurchaseRejectionAgainstBillDetail>();
            List<AccPurchaseRejectionDetail> purchaseRejectionGrid = new List<AccPurchaseRejectionDetail>();
            if (!string.IsNullOrEmpty(modelPRJson))
            {
                purchaseRejectionPopupGrid = JsonConvert.DeserializeObject<List<AccPurchaseRejectionAgainstBillDetail>>(modelPRJson);
            }
            if (!string.IsNullOrEmpty(modelPRGridJson))
            {
                purchaseRejectionGrid = JsonConvert.DeserializeObject<List<AccPurchaseRejectionDetail>>(modelPRGridJson);
            }
            //_MemoryCache.TryGetValue("KeyPurchaseRejectionGrid", out List<AccPurchaseRejectionDetail> purchaseRejectionGrid);
            //_MemoryCache.TryGetValue("KeyPurchaseRejectionPopupGrid", out IList<AccPurchaseRejectionAgainstBillDetail> purchaseRejectionPopupGrid);
            //var CNGrid = creditNoteGrid.Where(x => x.ItemCode == itemCode);
            var combinedData = new
            {
                PurchaseRejectionGrid = purchaseRejectionGrid?.Where(x => x.ItemCode == itemCode && x.SeqNo == Seq),
                PurchaseRejectionPopupGrid = purchaseRejectionPopupGrid
            };
            string JsonString = JsonConvert.SerializeObject(combinedData);
            return Json(JsonString);
        }
        public IActionResult DeleteItemRow(int itemCode, string Mode, int Seq, string uniquekeyid)
        {
            var MainModel = new AccPurchaseRejectionModel();
            //_MemoryCache.TryGetValue("KeyPurchaseRejectionGrid", out List<AccPurchaseRejectionDetail> purchaseRejectionDetail);
            string modelPRGridJson = HttpContext.Session.GetString("KeyPurchaseRejectionGrid");
            List<AccPurchaseRejectionDetail> purchaseRejectionDetail = new List<AccPurchaseRejectionDetail>();
            if (!string.IsNullOrEmpty(modelPRGridJson))
            {
                purchaseRejectionDetail = JsonConvert.DeserializeObject<List<AccPurchaseRejectionDetail>>(modelPRGridJson);
            }
            string modelPRAgainstGridJson = HttpContext.Session.GetString("KeyPurchaseRejectionPopupGrid");
            List<AccPurchaseRejectionAgainstBillDetail> PRPopupGrid = new List<AccPurchaseRejectionAgainstBillDetail>();
            if (!string.IsNullOrEmpty(modelPRAgainstGridJson))
            {
                PRPopupGrid = JsonConvert.DeserializeObject<List<AccPurchaseRejectionAgainstBillDetail>>(modelPRAgainstGridJson);
            }
            string uniquekey = string.Empty;
            if (purchaseRejectionDetail != null && purchaseRejectionDetail.Count > 0)
            {
                uniquekey = purchaseRejectionDetail.Where(x => x.ItemCode == itemCode && x.SeqNo == Seq).Select(x => x.hdnuniquekey).FirstOrDefault()?.ToString() ?? (uniquekeyid ?? string.Empty);
                purchaseRejectionDetail.RemoveAll(x => x.ItemCode == itemCode && x.SeqNo == Seq);
                MainModel.AccPurchaseRejectionDetails = purchaseRejectionDetail;

                MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                    SlidingExpiration = TimeSpan.FromMinutes(55),
                    Size = 1024,
                };

                //_MemoryCache.Set("KeyPurchaseRejectionGrid", MainModel.AccPurchaseRejectionDetails, cacheEntryOptions);
                HttpContext.Session.SetString("KeyPurchaseRejectionGrid", JsonConvert.SerializeObject(MainModel.AccPurchaseRejectionDetails));
            }
            if (PRPopupGrid != null && PRPopupGrid.Count > 0)
            {
                int EntryId = 0;
                int YearCode = 0;
                var InvoiceNo = string.Empty;
                var PurchBillItemCode = 0;
                if (!string.IsNullOrEmpty(uniquekey))
                {
                    var uniquekeyArray = uniquekey.Split("_").ToArray();
                    EntryId = !string.IsNullOrEmpty(uniquekeyArray[0]) ? Convert.ToInt32(uniquekeyArray[0]) : 0;
                    YearCode = !string.IsNullOrEmpty(uniquekeyArray[1]) ? Convert.ToInt32(uniquekeyArray[1]) : 0;
                    InvoiceNo = uniquekeyArray[2];
                    PurchBillItemCode = !string.IsNullOrEmpty(uniquekeyArray[3]) ? Convert.ToInt32(uniquekeyArray[3]) : 0;
                }
                var PopupGrid = PRPopupGrid;
                if (MainModel != null && MainModel.AccPurchaseRejectionDetails != null)
                {
                    if (MainModel.AccPurchaseRejectionDetails.Any())
                    {
                        bool IsExistsSameItem = MainModel.AccPurchaseRejectionDetails.Exists(a => a.ItemCode == itemCode && a.hdnuniquekey == uniquekey); // && a.AgainstPurchaseBillEntryId == EntryId && a.AgainstPurchaseBillYearCode == YearCode
                        if (!IsExistsSameItem)
                        {
                            foreach(var item in PRPopupGrid.ToList())
                            {
                                if(item.ItemCode == itemCode && item.AgainstPurchaseBillEntryId == EntryId && item.AgainstPurchaseBillYearCode == YearCode && item.InvoiceNo == InvoiceNo && item.PurchBillItemCode == PurchBillItemCode)
                                {
                                    PopupGrid.Remove(item);
                                }
                            }
                        }
                    }
                    else
                    {
                        foreach (var item in PRPopupGrid.ToList())
                        {
                            if (item.ItemCode == itemCode && item.AgainstPurchaseBillEntryId == EntryId && item.AgainstPurchaseBillYearCode == YearCode && item.InvoiceNo == InvoiceNo && item.PurchBillItemCode == PurchBillItemCode)
                            {
                                PopupGrid.Remove(item);
                            }
                        }
                    }
                }
                else
                {
                    foreach (var item in PRPopupGrid.ToList())
                    {
                        if (item.ItemCode == itemCode && item.AgainstPurchaseBillEntryId == EntryId && item.AgainstPurchaseBillYearCode == YearCode && item.InvoiceNo == InvoiceNo && item.PurchBillItemCode == PurchBillItemCode)
                        {
                            PopupGrid.Remove(item);
                        }
                    }
                }
                MainModel.AccPurchaseRejectionAgainstBillDetails = PopupGrid;

                MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                    SlidingExpiration = TimeSpan.FromMinutes(55),
                    Size = 1024,
                };

                //_MemoryCache.Set("KeyPurchaseRejectionGrid", MainModel.AccPurchaseRejectionDetails, cacheEntryOptions);
                HttpContext.Session.SetString("KeyPurchaseRejectionPopupGrid", JsonConvert.SerializeObject(MainModel.AccPurchaseRejectionAgainstBillDetails));
            }

            return PartialView("_PurchaseRejectionGrid", MainModel);
        }
        public async Task<JsonResult> CheckLockYear(int YearCode)
        {
            var JSON = await _purchRej.CheckLockYear(YearCode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetFormRights()
        {
            var userID = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
            var JSON = await _purchRej.GetFormRights(userID);
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
        public async Task<JsonResult> DeleteByID(int ID, int YC, string VoucherNo, string CC, int AccountCode, string EnteredBy, string InvNo = "", bool? IsDetail = false)
        {
            int EntryBy = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
            string cc = string.IsNullOrEmpty(CC) ? HttpContext.Session.GetString("Branch") : CC;
            DateTime EntryDate = DateTime.Today;
            var Result = await _purchRej.DeleteByID(ID, YC, "DELETE", VoucherNo, cc, AccountCode, InvNo, EntryBy, EnteredBy, EntryDate);

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
            if ((Result.StatusText == "Deleted Successfully" || Result.StatusText == "deleted Successfully" || Result.StatusText == "Success") && (Result.StatusCode == HttpStatusCode.Accepted || Result.StatusCode == HttpStatusCode.OK))
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
        [HttpPost]
        public JsonResult MergePopupGridToSession([FromBody]  List<AccPurchaseRejectionAgainstBillDetail> model)
        {
            List<AccPurchaseRejectionAgainstBillDetail> purchaseRejectionPopupGrid = new List<AccPurchaseRejectionAgainstBillDetail>();
            string modelPRJson = HttpContext.Session.GetString("KeyPurchaseRejectionPopupGrid");
            if (!string.IsNullOrEmpty(modelPRJson))
            {
                purchaseRejectionPopupGrid = JsonConvert.DeserializeObject<List<AccPurchaseRejectionAgainstBillDetail>>(modelPRJson);
            }
            if (model != null && purchaseRejectionPopupGrid != null)
            {
                if (purchaseRejectionPopupGrid.Any())
                {
                    foreach (var item in model)
                    {
                        var existing = purchaseRejectionPopupGrid.FirstOrDefault(x =>
                            x.ItemCode == item.ItemCode &&
                            x.PurchBillItemCode == item.PurchBillItemCode &&
                            x.AgainstPurchaseBillEntryId == item.AgainstPurchaseBillEntryId &&
                            x.AgainstPurchaseBillYearCode == item.AgainstPurchaseBillYearCode &&
                            x.InvoiceNo == item.InvoiceNo);

                        if (existing == null)
                        {
                            purchaseRejectionPopupGrid.Add(item);
                        }
                        else
                        {
                            UpdateBlankFields(existing, item);
                        }
                    }
                }
            }
            HttpContext.Session.SetString("KeyPurchaseRejectionPopupGrid", JsonConvert.SerializeObject(purchaseRejectionPopupGrid));
            return Json(new { success = true, count = (model == null ? 0 : model.Count) });
        }
        private void UpdateBlankFields<T>(T target, T source)
        {
            foreach (var prop in typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                var targetValue = prop.GetValue(target);
                var sourceValue = prop.GetValue(source);

                if (IsBlank(targetValue) && !IsBlank(sourceValue))
                {
                    prop.SetValue(target, sourceValue);
                }
            }
        }

        private bool IsBlank(object value)
        {
            if (value == null) return true;

            if (value is string s) return string.IsNullOrWhiteSpace(s);
            if (value is int i) return i == 0;
            if (value is long l) return l == 0;
            if (value is decimal d) return d == 0;
            if (value is double db) return db == 0;
            if (value is float f) return f == 0;

            return false;
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
                    "AccPurchaseRejectionEInvoice"
                );
                var responseObj = result.Result as JObject;


                //if (!string.IsNullOrWhiteSpace(ewbUrl))
                //{
                if (responseObj != null)
                {
                    if (input.generateEway == "EInvoice")
                    {
                        string ewbUrl = (string)responseObj["ewbUrl"];
                        string uploadsFolder = Path.Combine(_IWebHostEnvironment.WebRootPath, "Uploads", "QRCode");
                        if (!Directory.Exists(uploadsFolder))
                            Directory.CreateDirectory(uploadsFolder);

                        string fileName = $"{Guid.NewGuid()}.png";
                        string outputPath = Path.Combine(uploadsFolder, fileName);

                        var qrResult = await GenerateQRCodeImage(ewbUrl, outputPath);
                        if (qrResult != "Success")
                            return BadRequest("QR generation failed");

                        string publicUrl = $"{Request.Scheme}://{Request.Host}/Uploads/QRCode/{fileName}";
                        return Ok(new
                        {
                            redirectUrl = publicUrl,
                            rawResponse = (string)responseObj["rawResponse"]
                        });
                        //      return Ok(new { redirectUrl = publicUrl });
                    }
                    //return Ok(new { redirectUrl = ewbUrl }); ;

                    return Ok(new
                    {
                        redirectUrl = (string)responseObj["ewbUrl"],
                        rawResponse = (string)responseObj["rawResponse"]
                    });
                }
                //}
                else
                {
                    return BadRequest("Invoice generation failed");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Server Error: {ex.Message}");
            }
        }

    }
}
