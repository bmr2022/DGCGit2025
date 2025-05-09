using eTactWeb.Data.Common;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using eTactWeb.DOM.Models;
using System.Net;
using System.Data;
using System.Globalization;

namespace eTactWeb.Controllers
{
    public class DayBookController : Controller
    {
        private readonly IDataLogic _IDataLogic;
        public IDayBook _IDayBook { get; }
        private readonly ILogger<DayBookController> _logger;
        private readonly IConfiguration iconfiguration;
        public IWebHostEnvironment _IWebHostEnvironment { get; }
        public DayBookController(ILogger<DayBookController> logger, IDataLogic iDataLogic, IDayBook iDayBook, EncryptDecrypt encryptDecrypt, IWebHostEnvironment iWebHostEnvironment, IConfiguration iconfiguration)
        {
            _logger = logger;
            _IDataLogic = iDataLogic;
            _IDayBook = iDayBook;
            _IWebHostEnvironment = iWebHostEnvironment;
            this.iconfiguration = iconfiguration;
        }
        [Route("{controller}/Index")]
        [HttpGet]
        public async Task<ActionResult> DayBook()
        {
            var MainModel = new DayBookModel();
            MainModel.DayBookGrid = new List<DayBookModel>();
            MainModel.FromDate = HttpContext.Session.GetString("FromDate");
            MainModel.ToDate = HttpContext.Session.GetString("ToDate");
          //MainModel.YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));

            return View(MainModel); // Pass the model with old data to the view
        }
        public async Task<JsonResult> FillVoucherName(string FromDate, string ToDate)
        {
            var JSON = await _IDayBook.FillVoucherName(FromDate,ToDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillLedgerName(string FromDate, string ToDate)
        {
            var JSON = await _IDayBook.FillLedgerName(FromDate, ToDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<IActionResult> GetDayBookDetailsData(string FromDate, string ToDate,string Ledger,string VoucherType, string CrAmt, string DrAmt)
        {
            var model = new DayBookModel();
            model = await _IDayBook.GetDayBookDetailsData(FromDate , ToDate,  Ledger,  VoucherType,  CrAmt,  DrAmt);
            MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                SlidingExpiration = TimeSpan.FromMinutes(55),
                Size = 1024,
            };

            string serializedGrid = JsonConvert.SerializeObject(model.DayBookGrid);
            HttpContext.Session.SetString("KeyDayBookList", serializedGrid);
            return PartialView("_DayBookGrid", model);
           
        }
        [HttpGet]
        public IActionResult DayBookGridDataForPDF()
        {
            string modelJson = HttpContext.Session.GetString("KeyDayBookList");
            List<DayBookModel> stockRegisterList = new List<DayBookModel>();
            if (!string.IsNullOrEmpty(modelJson))
            {
                stockRegisterList = JsonConvert.DeserializeObject<List<DayBookModel>>(modelJson);
            }

            return Json(stockRegisterList);
        }
    }
}
