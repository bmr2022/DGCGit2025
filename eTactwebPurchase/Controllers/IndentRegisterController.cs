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
    public class IndentRegisterController : Controller
    {
        private readonly IDataLogic _IDataLogic;
        public IIndentRegister _IndentRegister { get; }

        private readonly ILogger<IndentRegisterController> _logger;
        private readonly IConfiguration iconfiguration;
        private readonly IMemoryCache _MemoryCache;
        public IWebHostEnvironment _IWebHostEnvironment { get; }
        public IndentRegisterController(ILogger<IndentRegisterController> logger, IDataLogic iDataLogic, IIndentRegister iIndentRegister, IMemoryCache iMemoryCache, EncryptDecrypt encryptDecrypt, IWebHostEnvironment iWebHostEnvironment, IConfiguration iconfiguration)
        {
            _logger = logger;
            _IDataLogic = iDataLogic;
            _IndentRegister = iIndentRegister;
            _MemoryCache = iMemoryCache;
            _IWebHostEnvironment = iWebHostEnvironment;
            this.iconfiguration = iconfiguration;
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


        [Route("{controller}/Index")]
        [HttpGet]
        public async Task<ActionResult> IndentRegister()
        {
            var MainModel = new IndentRegisterModel();
            MainModel.IndentRegisterGrid = new List<IndentRegisterModel>();
            MainModel.FromDate = HttpContext.Session.GetString("FromDate");
            MainModel.ToDate = HttpContext.Session.GetString("ToDate");
            //MainModel.YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));

            return View(MainModel); // Pass the model with old data to the view
        }

        public async Task<JsonResult> GetItemName(string FromDate, string ToDate)
        {
            var JSON = await _IndentRegister.GetItemName(FromDate,ToDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }

        public async Task<JsonResult> GetPartCode(string FromDate, string ToDate)
        {
            var JSON = await _IndentRegister.GetPartCode(FromDate, ToDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }

        public async Task<JsonResult> GetIndentNo(string FromDate, string ToDate)
        {
            var JSON = await _IndentRegister.GetIndentNo(FromDate, ToDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }

        public async Task<IActionResult> GetDetailsData(string FromDate, string ToDate, string ItemName, string PartCode, string IndentNo, string ReportType)
        {
            var model = new IndentRegisterModel();
            model = await _IndentRegister.GetDetailsData(FromDate, ToDate, ItemName, PartCode, IndentNo, ReportType);


            if (ReportType == "Summary") { 
                return PartialView("_IndentRegisterGrid", model);
            }
            else if(ReportType =="Detail")
            {
                return PartialView("_IndentDetailRegisterGrid", model);

            }
            else
            {
                return PartialView("_IndentPORegisterGrid", model);
            }
            return null;

        }
    }
}
