using eTactWeb.Data.Common;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using eTactWeb.DOM.Models;
using System.Net;
using System.Data;
using System.Globalization;

namespace eTactWeb.Controllers
{
    public class MIRRegisterController : Controller
    {
        private readonly IDataLogic _IDataLogic;
        public IMIRRegister _IMIRRegister { get; }

        private readonly ILogger<MIRRegisterController> _logger;
        private readonly IConfiguration iconfiguration;
        private readonly IMemoryCache _MemoryCache;
        public IWebHostEnvironment _IWebHostEnvironment { get; }

        public MIRRegisterController(ILogger<MIRRegisterController> logger, IDataLogic iDataLogic, IMIRRegister iMIRRegister, IMemoryCache iMemoryCache, EncryptDecrypt encryptDecrypt, IWebHostEnvironment iWebHostEnvironment, IConfiguration iconfiguration)
        {
            _logger = logger;
            _IDataLogic = iDataLogic;
            _IMIRRegister = iMIRRegister;
            _MemoryCache = iMemoryCache;
            _IWebHostEnvironment = iWebHostEnvironment;
            this.iconfiguration = iconfiguration;
        }
        [Route("{controller}/Index")]
        public IActionResult MIRRegister()
        {
            var model = new MIRRegisterModel();
            model.MIRRegisterDetail = new List<MIRRegisterDetail>();
            return View(model);
        }
        public async Task<IActionResult> GetRegisterData(string MRNType, string ReportType, string FromDate, string ToDate, string gateno,string MRNno, string docname, string PONo, string Schno, string PartCode, string ItemName, string invoiceNo, string VendorName,string MRNStatus)
        {
            var model = new MIRRegisterModel();
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
       
            model = await _IMIRRegister.GetRegisterData(MRNType,ReportType,  FromDate,  ToDate,  gateno,  MRNno,docname,  PONo,  Schno,  PartCode,  ItemName,  invoiceNo,  VendorName,MRNStatus);
            model.ReportMode= ReportType;
            return PartialView("_MIRRegisterGrid", model);
        }
        public async Task<JsonResult> FillGateNo(string FromDate, string ToDate)
        {
            var JSON = await _IMIRRegister.FillGateNo(FromDate, ToDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillMIRNo(string FromDate, string ToDate)
        {

            var JSON = await _IMIRRegister.FillMIRNo(FromDate, ToDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillVendor(string FromDate, string ToDate)
        {
            var JSON = await _IMIRRegister.FillVendor(FromDate, ToDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillInvoice(string FromDate, string ToDate)
        {
            var JSON = await _IMIRRegister.FillInvoice(FromDate, ToDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        } 
        public async Task<JsonResult> FillItemName(string FromDate, string ToDate)
        {
            var JSON = await _IMIRRegister.FillItemName(FromDate, ToDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillItemPartcode(string FromDate, string ToDate)
        {
            var JSON = await _IMIRRegister.FillItemPartcode(FromDate, ToDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillPONO(string FromDate, string ToDate)
        {
            var JSON = await _IMIRRegister.FillPONO(FromDate, ToDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillSchNo(string FromDate, string ToDate)
        {
            var JSON = await _IMIRRegister.FillSchNo(  FromDate,  ToDate);
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
