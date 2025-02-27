using eTactWeb.Data.Common;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json.Linq;
using eTactWeb.DOM.Models;
using System.Net;
using System.Data;
using System.Globalization;

namespace eTactWeb.Controllers
{
    public class PORegisterController : Controller
    {
        private readonly IDataLogic _IDataLogic;
        public IPORegister _IPORegister { get; }

        private readonly ILogger<PORegisterController> _logger;
        private readonly IConfiguration iconfiguration;
        private readonly IMemoryCache _MemoryCache;
        public IWebHostEnvironment _IWebHostEnvironment { get; }

        public PORegisterController(ILogger<PORegisterController> logger, IDataLogic iDataLogic, IMemoryCache iMemoryCache, EncryptDecrypt encryptDecrypt, IWebHostEnvironment iWebHostEnvironment, IConfiguration iconfiguration, IPORegister pORegister)
        {
            _logger = logger;
            _IDataLogic = iDataLogic;
            _IPORegister = pORegister;
            _MemoryCache = iMemoryCache;
            _IWebHostEnvironment = iWebHostEnvironment;
            this.iconfiguration = iconfiguration;
        }
        public IActionResult PORegister()
        {
            var model = new PORegisterModel();
            model.PORegisterDetails = new List<PORegisterDetail>();
            return View(model);
        }

        public async Task<IActionResult> GetPORegisterData(string FromDate, string ToDate, string ReportType, string  Partyname, string  partcode, string  itemName, string  POno, string  SchNo, string  OrderType, string  POFor, string  ItemType, string  ItemGroup)
        {
            var YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
            var model = new PORegisterModel();
            model = await _IPORegister.GetPORegisterData(FromDate, ToDate, ReportType, YearCode, Partyname, partcode, itemName, POno, SchNo, OrderType, POFor, ItemType, ItemGroup);
            model.ReportMode = ReportType;
            return PartialView("_PORegisterGrid", model);
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

                //using (HttpClient client = new HttpClient())
                //{
                //    HttpResponseMessage response = await client.GetAsync(apiUrl);

                //    if (response.IsSuccessStatusCode)
                //    {
                //        string content = await response.Content.ReadAsStringAsync();
                //        JObject jsonObj = JObject.Parse(content);

                //        string datetimestring = (string)jsonObj["datetime"];
                //        var formattedDateTime = datetimestring.Split(" ")[0];

                //        DateTime parsedDate = DateTime.ParseExact(formattedDateTime, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                //        string formattedDate = parsedDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);

                //        return Json(formattedDate);
                //    }
                //    else
                //    {
                //        string errorContent = await response.Content.ReadAsStringAsync();
                //        throw new HttpRequestException($"Failed to fetch server date and time. Status Code: {response.StatusCode}. Content: {errorContent}");
                //    }
                //}
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
