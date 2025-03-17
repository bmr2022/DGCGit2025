using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using static eTactWeb.DOM.Models.Common;
using static eTactWeb.Data.Common.CommonFunc;
using System.Data;

namespace eTactWeb.Controllers
{
    public class PurchaseRejectionController : Controller
    {
        private readonly IMemoryCache _MemoryCache;
        private readonly IPurchaseRejection _purchRej;
        private readonly ILogger<PurchaseRejectionController> _logger;
        public IWebHostEnvironment _IWebHostEnvironment { get; }

        public PurchaseRejectionController(IMemoryCache memoryCache, IPurchaseRejection purchRej, IWebHostEnvironment IWebHostEnvironment, ILogger<PurchaseRejectionController> logger)
        {
            _MemoryCache = memoryCache;
            _purchRej = purchRej;
            _IWebHostEnvironment = IWebHostEnvironment;
            _logger = logger;
        }

        [HttpGet]
        [Route("{controller}/Index")]
        public async Task<IActionResult> PurchaseRejection(int ID, string Mode, int YC)
        {
            AccPurchaseRejectionModel model = new AccPurchaseRejectionModel();
            ViewData["Title"] = "Purchase Rejection Details";
            TempData.Clear();
            _MemoryCache.Remove("KeyPurchaseRejectionGrid");
            _MemoryCache.Remove("PurchaseRejectionModel");
            _MemoryCache.Remove("KeyAdjGrid");
            _MemoryCache.Remove("KeyPurchaseRejectionPopupGrid");
            // var model = await BindModel(MainModel);

            if (model.Mode != "U")
            {
                model.Uid = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                model.ActualEnteredBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                model.ActualEnteredByName = HttpContext.Session.GetString("EmpName");
            }

            if (!string.IsNullOrEmpty(Mode) && ID > 0 && (Mode == "V" || Mode == "U"))
            {
                model = await _purchRej.GetViewByID(ID, YC, Mode);
                model.Mode = Mode;
                model.ID = ID;
            }

            MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                SlidingExpiration = TimeSpan.FromMinutes(55),
                Size = 1024,
            };
            model.FinFromDate = HttpContext.Session.GetString("FromDate");
            model.FinToDate = HttpContext.Session.GetString("ToDate");
            model.PurchaseRejYearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
            model.CC = HttpContext.Session.GetString("Branch");
            _MemoryCache.Set("KeyPurchaseRejectionGrid", model.AccPurchaseRejectionDetails, cacheEntryOptions);
            _MemoryCache.Set("KeyAdjGrid", model.adjustmentModel == null ? new AdjustmentModel() : model.adjustmentModel, DateTimeOffset.Now.AddMinutes(60));
            _MemoryCache.Set("KeyTaxGrid", model.TaxDetailGridd == null ? new List<TaxModel>() : model.TaxDetailGridd, DateTimeOffset.Now.AddMinutes(60));
            _MemoryCache.Set("PurchaseRejectionModel", model, cacheEntryOptions);
            HttpContext.Session.SetString("PurchaseRejection", JsonConvert.SerializeObject(model));
            return View(model);
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
            var dataGrid = GetDetailFromPopup(model);
            var JSON = await _purchRej.FillDetailFromPopupGrid(dataGrid, itemCode, popCt);
            string JsonString = JsonConvert.SerializeObject(JSON);
            MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                SlidingExpiration = TimeSpan.FromMinutes(55),
                Size = 1024,
            };
            _MemoryCache.Set("KeyPurchaseRejectionPopupGrid", model, cacheEntryOptions);
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
                //table.Columns.Add("AgainstSalebillBillNo", typeof(string));
                //table.Columns.Add("AgainstSaleBillYearCode", typeof(int));
                //table.Columns.Add("AgainstSaleBilldate", typeof(string));
                //table.Columns.Add("AgainstSaleBillEntryId", typeof(int));
                //table.Columns.Add("AgainstSalebillVoucherNo", typeof(string));
                //table.Columns.Add("SaleBillTYpe", typeof(string));
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
                //table.Columns.Add("SONO", typeof(string));
                //table.Columns.Add("SOYearcode", typeof(int));
                //table.Columns.Add("SODate", typeof(string));
                //table.Columns.Add("CustOrderNo", typeof(string));
                //table.Columns.Add("SOEntryId", typeof(int));
                table.Columns.Add("BatchNo", typeof(string));
                table.Columns.Add("UniqueBatchNo", typeof(string));

                foreach (AccPurchaseRejectionAgainstBillDetail Item in List)
                {
                    table.Rows.Add(
                        new object[]
                        {
                      1,
                     Item.AgainstPurchaseBillYearCode,
                    Item.PurchaseRejectionInvoiceNo ?? string.Empty,
                   Item.PurchaseRejectionVoucherNo ?? string.Empty,
                   //Item.AgainstSaleBillBillNo ?? string.Empty,
                   //Item.AgainstSaleBillYearCode,
                   //Item.AgainstSaleBillDate == null ? string.Empty : ParseFormattedDate(Item.AgainstSaleBillDate),
                   //Item.AgainstSaleBillEntryId,
                   //Item.AgainstSaleBillVoucherNo ?? string.Empty,
                   //Item.SaleBillType ?? string.Empty,
                   Item.AgainstPurchaseBillBillNo ?? string.Empty,
                   Item.AgainstPurchaseBillYearCode,
                   Item.AgainstPurchaseBillDate == null ? string.Empty : ParseFormattedDate(Item.AgainstPurchaseBillDate),
                   Item.AgainstPurchaseBillEntryId,
                   Item.AgainstPurchaseVoucherNo ?? string.Empty,
                   Item.PurchaseBillType ?? string.Empty,
                   Item.ItemCode,// CreditNoteItemCode
                   1, //BillItemCode,
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
                   //Item.SONO ?? string.Empty,
                   //Item.SOYearCode,
                   //Item.SODate == null ? string.Empty : ParseFormattedDate(Item.SODate),
                   //Item.CustOrderNo ?? string.Empty,
                   //Item.SOEntryId,
                   Item.BatchNo ?? string.Empty,
                   Item.UniqueBatchNo ?? string.Empty
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
                _MemoryCache.TryGetValue("KeyPurchaseRejectionGrid", out IList<AccPurchaseRejectionDetail> purchaseRejectionGrid);
                _MemoryCache.TryGetValue("PurchaseRejectionModel", out AccPurchaseRejectionModel purchaseRejectionlModel);
                //_MemoryCache.TryGetValue("SaleBillModel", out AccCreditNoteModel saleBillModel);

                var MainModel = new AccPurchaseRejectionModel();
                var purchaseRejectionDetail = new List<AccPurchaseRejectionDetail>();
                var rangeSaleBillGrid = new List<AccPurchaseRejectionDetail>();

                if (model != null)
                {
                    if (purchaseRejectionGrid == null)
                    {
                        //model.SeqNo = 1;
                        purchaseRejectionDetail.Add(model);
                    }
                    else
                    {
                        if (purchaseRejectionGrid.Any(x => x.ItemCode == model.ItemCode && x.StoreId == model.StoreId))
                        {
                            return StatusCode(207, "Duplicate");
                        }

                        //model.SeqNo = SaleBillDetail.Count + 1;
                        purchaseRejectionDetail = purchaseRejectionGrid.Where(x => x != null).ToList();
                        rangeSaleBillGrid.AddRange(purchaseRejectionDetail);
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


                    _MemoryCache.Set("KeyPurchaseRejectionGrid", MainModel.AccPurchaseRejectionDetails, cacheEntryOptions);

                    MainModel.AccPurchaseRejectionDetails = purchaseRejectionDetail;
                    MainModel.ItemDetailGrid = purchaseRejectionDetail;
                    _MemoryCache.Set("PurchaseRejectionModel", MainModel, cacheEntryOptions);
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
        public async Task<JsonResult> EditItemRows(int itemCode)
        {
            var MainModel = new AccPurchaseRejectionModel();
            _MemoryCache.TryGetValue("KeyPurchaseRejectionGrid", out List<AccPurchaseRejectionDetail> purchaseRejectionGrid);
            _MemoryCache.TryGetValue("KeyPurchaseRejectionPopupGrid", out IList<AccPurchaseRejectionAgainstBillDetail> purchaseRejectionPopupGrid);
            //var CNGrid = creditNoteGrid.Where(x => x.ItemCode == itemCode);
            var combinedData = new
            {
                PurchaseRejectionGrid = purchaseRejectionGrid?.Where(x => x.ItemCode == itemCode),
                PurchaseRejectionPopupGrid = purchaseRejectionPopupGrid
            };
            string JsonString = JsonConvert.SerializeObject(combinedData);
            return Json(JsonString);
        }
        public IActionResult DeleteItemRow(int itemCode, string Mode)
        {
            var MainModel = new AccPurchaseRejectionModel();
            if (Mode == "U")
            {
                _MemoryCache.TryGetValue("KeyPurchaseRejectionGrid", out List<AccPurchaseRejectionDetail> purchaseRejectionDetail);

                if (purchaseRejectionDetail != null && purchaseRejectionDetail.Count > 0)
                {
                    purchaseRejectionDetail.RemoveAll(x => x.ItemCode == itemCode);

                    MainModel.AccPurchaseRejectionDetails = purchaseRejectionDetail;

                    MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
                    {
                        AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                        SlidingExpiration = TimeSpan.FromMinutes(55),
                        Size = 1024,
                    };

                    _MemoryCache.Set("KeyPurchaseRejectionGrid", MainModel.AccPurchaseRejectionDetails, cacheEntryOptions);
                }
            }
            else
            {
                _MemoryCache.TryGetValue("KeyPurchaseRejectionGrid", out List<AccPurchaseRejectionDetail> purchaseRejectionDetail);

                if (purchaseRejectionDetail != null && purchaseRejectionDetail.Count > 0)
                {
                    purchaseRejectionDetail.RemoveAll(x => x.ItemCode == itemCode);
                    MainModel.AccPurchaseRejectionDetails = purchaseRejectionDetail;

                    MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
                    {
                        AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                        SlidingExpiration = TimeSpan.FromMinutes(55),
                        Size = 1024,
                    };

                    _MemoryCache.Set("KeyPurchaseRejectionGrid", MainModel.AccPurchaseRejectionDetails, cacheEntryOptions);
                }
            }

            return PartialView("_PurchaseRejectionGrid", MainModel);
        }
    }
}
