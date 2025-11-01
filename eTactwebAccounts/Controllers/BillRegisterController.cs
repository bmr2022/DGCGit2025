using eTactWeb.Controllers;
using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;

namespace eTactwebAccounts.Controllers
{
    public class BillRegisterController : Controller
    {
        private readonly IDataLogic _IDataLogic;
        public IBillRegister _IBillRegister { get; }
        private readonly ILogger<BillRegisterController> _logger;
        private readonly IConfiguration iconfiguration;
        public IWebHostEnvironment _IWebHostEnvironment { get; }
        public BillRegisterController(ILogger<BillRegisterController> logger, IDataLogic iDataLogic, IBillRegister iBillRegister, EncryptDecrypt encryptDecrypt, IWebHostEnvironment iWebHostEnvironment, IConfiguration iconfiguration)
        {
            _logger = logger;
            _IDataLogic = iDataLogic;
            _IBillRegister = iBillRegister;
            _IWebHostEnvironment = iWebHostEnvironment;
            this.iconfiguration = iconfiguration;
        }
        [Route("{controller}/Index")]
        public IActionResult BillRegister()
        {
            var model = new BillRegisterModel();
            model.BillRegisterList = new List<BillRegisterModel>();
            return View(model);
        }
        public async Task<JsonResult> FillVendoreList(string FromDate, string ToDate)
        {
            var JSON = await _IBillRegister.FillVendoreList(FromDate, ToDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillDocList(string FromDate, string ToDate)
        {
            var JSON = await _IBillRegister.FillDocList(FromDate, ToDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        } 
        public async Task<JsonResult> FillPONOList(string FromDate, string ToDate)
        {
            var JSON = await _IBillRegister.FillPONOList(FromDate, ToDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillSchNOList(string FromDate, string ToDate)
        {
            var JSON = await _IBillRegister.FillSchNOList(FromDate, ToDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        } 
        public async Task<JsonResult> FillHsnNOList(string FromDate, string ToDate)
        {
            var JSON = await _IBillRegister.FillHsnNOList(FromDate, ToDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        } 
        public async Task<JsonResult> FillGstNOList(string FromDate, string ToDate)
        {
            var JSON = await _IBillRegister.FillGstNOList(FromDate, ToDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        } 
        public async Task<JsonResult> FillInvoiceNOList(string FromDate, string ToDate)
        {
            var JSON = await _IBillRegister.FillInvoiceNOList(FromDate, ToDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
         public async Task<JsonResult> FillItemName(string FromDate, string ToDate)
        {
            var JSON = await _IBillRegister.FillItemName(FromDate, ToDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
         public async Task<JsonResult> FillPartCode(string FromDate, string ToDate)
        {
            var JSON = await _IBillRegister.FillPartCode(FromDate, ToDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<IActionResult> GetRegisterData(string ReportType, string FromDate, string ToDate, string DocumentName, string PONO, string SchNo, string HsnNo, string GstNo, string InvoiceNo, string PurchaseBill, string PurchaseRejection, string DebitNote, string CreditNote, string SaleRejection, string PartCode, string ItemName, string VendorName,int ForFinYear)
        {
            var model = new BillRegisterModel();
            ForFinYear= Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
            model = await _IBillRegister.GetRegisterData( ReportType,  FromDate,  ToDate,  DocumentName,  PONO,  SchNo,  HsnNo,  GstNo,  InvoiceNo,  PurchaseBill,  PurchaseRejection,  DebitNote,  CreditNote,  SaleRejection,  PartCode,  ItemName,  VendorName, ForFinYear);
            model.ReportType = ReportType;
            MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                SlidingExpiration = TimeSpan.FromMinutes(55),
                Size = 1024,
            };
            string serializedGrid = JsonConvert.SerializeObject(model.BillRegisterList);
            HttpContext.Session.SetString("KeyBillRegisterList", serializedGrid);
            if(ReportType== "PurchaseSummary")
            {
                return PartialView("_BillRegisterGrid", model);
            }
            else if(ReportType== "PurchaseBillDetail")
            {
                return PartialView("_BillRegisterPurchaseBillDetailGrid", model);
            }
            else if(ReportType== "PURCHASESUMMARYREG"||ReportType== "GSTSUMMARY")
            {
                return PartialView("_BillRegisterGrid", model);
            }

            else if(ReportType== "VendorItemWiseTrend" )
            {
                return PartialView("_BillRegisterVendorItemWiseTrendGrid", model);
            }
            else if(ReportType== "VendorWiseMonthlyTrend")
            {
                return PartialView("_BillRegisterVendorWiseMonthlyTrendGrid", model);
            }
             else if(ReportType== "VendorItemWiseMonthlyData")
            {
                return PartialView("_BillRegisterVendorItemWiseMonthlyDataGrid", model);
            }
            else if(ReportType== "ItemWiseMonthlyTrend")
            {
                return PartialView("_BillRegisterItemWiseMonthlyTrendGrid", model);
            }
             else if(ReportType== "VendorWiseMonthlyValueTrend")
            {
                return PartialView("_BillRegisterVendorWiseMonthlyValueTrendGrid", model);
            }

            return null;
        }
        [HttpGet]
        public IActionResult GetBillRegistergridPDFData()
        {
            string modelJson = HttpContext.Session.GetString("KeyBillRegisterList");
            List<BillRegisterModel> billRegisterList = new List<BillRegisterModel>();
            if (!string.IsNullOrEmpty(modelJson))
            {
                billRegisterList = JsonConvert.DeserializeObject<List<BillRegisterModel>>(modelJson);
            }
            return Json(billRegisterList);
        }
    }
}
