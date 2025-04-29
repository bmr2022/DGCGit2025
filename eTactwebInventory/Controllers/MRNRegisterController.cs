using eTactWeb.Data.Common;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using eTactWeb.DOM.Models;
using System.Globalization;
using Microsoft.AspNetCore.Http;
using DocumentFormat.OpenXml.Drawing.Diagrams;

namespace eTactWeb.Controllers
{
    public class MRNRegisterController : Controller
    {
        private readonly IDataLogic _IDataLogic;
        public IMRNRegister _IMRNRegister { get; }
        private readonly ILogger<MRNRegisterController> _logger;
        private readonly IConfiguration iconfiguration;
        public IWebHostEnvironment _IWebHostEnvironment { get; }

        public MRNRegisterController(ILogger<MRNRegisterController> logger, IDataLogic iDataLogic, IMRNRegister iMRNRegister, EncryptDecrypt encryptDecrypt, IWebHostEnvironment iWebHostEnvironment, IConfiguration iconfiguration)
        {
            _logger = logger;
            _IDataLogic = iDataLogic;
            _IMRNRegister = iMRNRegister;
            _IWebHostEnvironment = iWebHostEnvironment;
            this.iconfiguration = iconfiguration;
        }
        [Route("{controller}/Index")]
        public IActionResult MRNRegister()
        {
            var model = new MRNRegisterModel();
            model.MRNRegisterDetail = new List<MRNRegisterDetail>();
            return View(model);
        }
        public async Task<IActionResult> GetMRNRegisterData(string MRNType, string ReportType, string FromDate, string ToDate, string gateno,string MRNno, string docname, string PONo, string Schno, string PartCode, string ItemName, string invoiceNo, string VendorName)
        {
            var model = new MRNRegisterModel();
            if (string.IsNullOrEmpty(gateno)||gateno == "0" )
                {                gateno = "";            }
            if (string.IsNullOrEmpty(MRNno) || MRNno == "0")
            { MRNno = ""; }
            if (string.IsNullOrEmpty(docname) || docname == "0")
            { docname = ""; }
            if (string.IsNullOrEmpty(PONo) || PONo == "0")
            { PONo = ""; }
            if (string.IsNullOrEmpty(Schno) || Schno == "0")
            { Schno = ""; }
            if (string.IsNullOrEmpty(PartCode) || PartCode == "0")
            { PartCode = ""; }
            if (string.IsNullOrEmpty(ItemName) || ItemName == "0")
            { ItemName = ""; }
            if (string.IsNullOrEmpty(invoiceNo) || invoiceNo == "0")
            { invoiceNo = ""; }
            if (string.IsNullOrEmpty(VendorName) || VendorName == "0")
            { VendorName = ""; }

            MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                SlidingExpiration = TimeSpan.FromMinutes(55),
                Size = 1024,
            };
            model = await _IMRNRegister.GetMRNRegisterData(MRNType,ReportType,  FromDate,  ToDate,  gateno,  MRNno,docname,  PONo,  Schno,  PartCode,  ItemName,  invoiceNo,  VendorName);
            string serializedGrid = JsonConvert.SerializeObject(model.MRNRegisterDetail);
            HttpContext.Session.SetString("KeyMRNList", serializedGrid);
            model.ReportMode= ReportType;
            return PartialView("_MRNRegisterGrid", model);
        }
       
        public async Task<JsonResult> FillGateNo(string FromDate, string ToDate)
        {
            var JSON = await _IMRNRegister.FillGateNo(FromDate, ToDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillDocument(string FromDate, string ToDate)
        {

            var JSON = await _IMRNRegister.FillDocument(FromDate, ToDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillVendor(string FromDate, string ToDate)
        {
            var JSON = await _IMRNRegister.FillVendor(FromDate, ToDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillInvoice(string FromDate, string ToDate)
        {
            var JSON = await _IMRNRegister.FillInvoice(FromDate, ToDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        } 
        public async Task<JsonResult> FillItemNamePartcode(string FromDate, string ToDate)
        {
            var JSON = await _IMRNRegister.FillItemNamePartcode(FromDate, ToDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillPONO(string FromDate, string ToDate)
        {
            var JSON = await _IMRNRegister.FillPONO(FromDate, ToDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillSchNo(string FromDate, string ToDate)
        {
            var JSON = await _IMRNRegister.FillSchNo(  FromDate,  ToDate);
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
        [HttpGet]
        public IActionResult GetMRNRegisterData()
        {
            string modelJson = HttpContext.Session.GetString("KeyMRNList");
            List<MRNRegisterDetail> stockRegisterList = new List<MRNRegisterDetail>();
            if (!string.IsNullOrEmpty(modelJson))
            {
                stockRegisterList = JsonConvert.DeserializeObject<List<MRNRegisterDetail>>(modelJson);
            }

            return Json(stockRegisterList);
        }
    }
}
