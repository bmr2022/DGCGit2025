using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using static eTactWeb.DOM.Models.Common;
using static eTactWeb.Data.Common.CommonFunc;

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
        public async Task<JsonResult> FillItems(int YearCode, int accountCode, string showAllItems)
        {
            var JSON = await _purchRej.FillItems(YearCode, accountCode, showAllItems);
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
    }
}
