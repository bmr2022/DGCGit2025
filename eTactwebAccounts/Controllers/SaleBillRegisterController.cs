using eTactWeb.DOM.Models;
using eTactWeb.Data.Common;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Data;
using System.Globalization;

namespace eTactWeb.Controllers
{
    public class SaleBillRegisterController : Controller
    {
        private readonly IDataLogic _IDataLogic;
        public ISaleBillRegister _ISaleBillRegister { get; }
        private readonly ILogger<SaleBillRegisterController> _logger;
        private readonly IConfiguration iconfiguration;
        public IWebHostEnvironment _IWebHostEnvironment { get; }
        public SaleBillRegisterController(ILogger<SaleBillRegisterController> logger, IDataLogic iDataLogic, ISaleBillRegister iSaleBillRegister, EncryptDecrypt encryptDecrypt, IWebHostEnvironment iWebHostEnvironment, IConfiguration iconfiguration)
        {
            _logger = logger;
            _IDataLogic = iDataLogic;
            _ISaleBillRegister = iSaleBillRegister;
            _IWebHostEnvironment = iWebHostEnvironment;
            this.iconfiguration = iconfiguration;
        }
        [Route("{controller}/Index")]
        public IActionResult SaleBillRegister()
        {
            var model = new SaleBillRegisterModel();
            model.SaleBillRegisterDetail = new List<SaleBillRegisterDetail>();
            return View(model);
        }
        public async Task<IActionResult> GetSaleBillRegisterData(string ReportType, string FromDate, string ToDate, string docname, string SONo, string Schno, string PartCode, string ItemName, string SaleBillNo, string CustomerName, string HSNNO, string GSTNO)

        {
            var model = new SaleBillRegisterModel();
            if (string.IsNullOrEmpty(SaleBillNo) || SaleBillNo == "0" )
                { SaleBillNo = "";            }
            if (string.IsNullOrEmpty(docname) || docname == "0")
            { docname = ""; }
            if (string.IsNullOrEmpty(SONo) || SONo == "0")
            { SONo = ""; }
            if (string.IsNullOrEmpty(Schno) || Schno == "0")
            { Schno = ""; }
            if (string.IsNullOrEmpty(PartCode) || PartCode == "0")
            { PartCode = ""; }
            if (string.IsNullOrEmpty(ItemName) || ItemName == "0")
            { ItemName = ""; }
            if (string.IsNullOrEmpty(CustomerName) || CustomerName == "0")
            { CustomerName = ""; }
            if (string.IsNullOrEmpty(HSNNO) || HSNNO == "0")
            { HSNNO = ""; }
            if (string.IsNullOrEmpty(GSTNO) || GSTNO == "0")
            { GSTNO = ""; }
            model = await _ISaleBillRegister.GetSaleBillRegisterData(ReportType, FromDate, ToDate, docname, SONo, Schno, PartCode, ItemName, SaleBillNo, CustomerName, HSNNO, GSTNO);
            model.ReportMode= ReportType;
            MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                SlidingExpiration = TimeSpan.FromMinutes(55),
                Size = 1024,
            };
            string serializedGrid = JsonConvert.SerializeObject(model.SaleBillRegisterDetail);
            HttpContext.Session.SetString("KeySaleBillRegsiterList", serializedGrid);
            //return PartialView("_SaleBillRegisterSaleDetail", model);
            return PartialView("_SaleBillRegisterGrid", model);

        }
        [HttpGet]
        public IActionResult GetSaleBillRegistergridData()
        {
            string modelJson = HttpContext.Session.GetString("KeySaleBillRegsiterList");
            List<SaleBillRegisterDetail> stockRegisterList = new List<SaleBillRegisterDetail>();
            if (!string.IsNullOrEmpty(modelJson))
            {
                stockRegisterList = JsonConvert.DeserializeObject<List<SaleBillRegisterDetail>>(modelJson);
            }
            return Json(stockRegisterList);
        }
        public async Task<JsonResult> FillCustomerList(string FromDate, string ToDate)
        {
            var JSON = await _ISaleBillRegister.FillCustomerList(FromDate, ToDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillDocumentList(string FromDate, string ToDate)
        {

            var JSON = await _ISaleBillRegister.FillDocumentList(FromDate, ToDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillSaleBillList(string FromDate, string ToDate)
        {
            var JSON = await _ISaleBillRegister.FillSaleBillList(FromDate, ToDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillItemNamePartcodeList(string FromDate, string ToDate)
        {
            var JSON = await _ISaleBillRegister.FillItemNamePartcodeList(FromDate, ToDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillSONO(string FromDate, string ToDate)
        {
            var JSON = await _ISaleBillRegister.FillSONO(FromDate, ToDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillSchNo(string FromDate, string ToDate)
        {
            var JSON = await _ISaleBillRegister.FillSchNo(FromDate, ToDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillHSNNo(string FromDate, string ToDate)
        {
            var JSON = await _ISaleBillRegister.FillHSNNo(  FromDate,  ToDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillGSTNo(string FromDate, string ToDate)
        {
            var JSON = await _ISaleBillRegister.FillGSTNo(FromDate, ToDate);
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
    }
}
