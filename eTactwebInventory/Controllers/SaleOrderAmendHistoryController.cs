using System.Globalization;
using eTactWeb.Controllers;
using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;

namespace eTactwebInventory.Controllers
{
    public class SaleOrderAmendHistoryController : Controller
    {
        private readonly IDataLogic _IDataLogic;
        public ISaleOrderAmendHistory _ISaleOrderAmendHistory { get; }
        private readonly ILogger<SaleOrderAmendHistoryController> _logger;
        private readonly IConfiguration iconfiguration;
        public IWebHostEnvironment _IWebHostEnvironment { get; }

        public SaleOrderAmendHistoryController(ILogger<SaleOrderAmendHistoryController> logger, IDataLogic iDataLogic, ISaleOrderAmendHistory SaleOrderAmendHistory, EncryptDecrypt encryptDecrypt, IWebHostEnvironment iWebHostEnvironment, IConfiguration iconfiguration)
        {
            _logger = logger;
            _IDataLogic = iDataLogic;
            _ISaleOrderAmendHistory = SaleOrderAmendHistory;
            _IWebHostEnvironment = iWebHostEnvironment;
            this.iconfiguration = iconfiguration;
        }
        [Route("{controller}/Index")]
        public IActionResult SaleOrderAmendHistory()
        {
            var model = new SaleOrderAmendHistoryModel();
            model.SaleOrderAmendHistoryDetail = new List<SaleOrderAmendHistoryDetail>();
            return View(model);
        }

        public async Task<IActionResult> SaleOrderAmendHistoryData(
            string reportType,
            string flag,
            string dashboardFlag,
            string fromDate,
            string toDate,
            int accountCode,
            string poNo,
            string partCode,
            int itemCode,
            string soNo)
        {
            var model = await _ISaleOrderAmendHistory.SaleOrderAmendHistory(
                 reportType,
                 flag,
                 dashboardFlag,
                 fromDate,
                 toDate,
                 accountCode,
                 poNo,
                 partCode,
                 itemCode,
                 soNo
            );

            model.ReportType = reportType;

            string serializedGrid = JsonConvert.SerializeObject(model.SaleOrderAmendHistoryDetail);
            HttpContext.Session.SetString("KeyGateEntryList", serializedGrid);

            return PartialView("SaleOrderAmendHistorygrid", model);
        }
        public async Task<JsonResult> FillItemNamePartcode(string FromDate, string ToDate, int AccountCode, string SOno)
        {
            var JSON = await _ISaleOrderAmendHistory.FillItemNamePartcode(FromDate, ToDate, AccountCode, SOno);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }

        public async Task<JsonResult> FillAccountCode(string FromDate, string ToDate, string partCode, string ItemCode, string SOno)
        {
            var JSON = await _ISaleOrderAmendHistory.FillAccountCode(FromDate, ToDate, partCode, ItemCode, SOno);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }

        public async Task<JsonResult> FillSONO(string FromDate, string ToDate, int AccountCode)
        {
            var JSON = await _ISaleOrderAmendHistory.FillSONO(FromDate, ToDate, AccountCode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }

        public async Task<JsonResult> FillPONO(string FromDate, string ToDate, int AccountCode)
        {
            var JSON = await _ISaleOrderAmendHistory.FillPONO(FromDate, ToDate, AccountCode);
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
