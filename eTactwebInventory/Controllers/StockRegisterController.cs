using eTactWeb.Data.Common;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using eTactWeb.DOM.Models;
using System.Globalization;
using System.Drawing.Printing;

namespace eTactWeb.Controllers
{
    public class StockRegisterController : Controller
    {
        private readonly IDataLogic _IDataLogic;
        public IStockRegister _IStockRegister { get; }

        private readonly ILogger<StockRegisterController> _logger;
        private readonly IConfiguration iconfiguration;
        private readonly IMemoryCache _MemoryCache;
        public IWebHostEnvironment _IWebHostEnvironment { get; }

        public StockRegisterController(ILogger<StockRegisterController> logger, IDataLogic iDataLogic, IStockRegister iStockRegister, IMemoryCache iMemoryCache, EncryptDecrypt encryptDecrypt, IWebHostEnvironment iWebHostEnvironment, IConfiguration iconfiguration)
        {
            _logger = logger;
            _IDataLogic = iDataLogic;
            _IStockRegister = iStockRegister;
            _MemoryCache = iMemoryCache;
            _IWebHostEnvironment = iWebHostEnvironment;
            this.iconfiguration = iconfiguration;
        }
        [Route("{controller}/Index")]
        public IActionResult StockRegister()
        {
            var model = new StockRegisterModel();
            model.StockRegisterDetail = new List<StockRegisterDetail>();
            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> GetStockRegisterData(string FromDate, string ToDate,string PartCode,string ItemName, string ItemGroup,string ItemType,int StoreId,string ReportType,string BatchNo,string UniqueBatchNo, int pageNumber = 1, int pageSize = 100)
        {
            var model = new StockRegisterModel();
            //model = await _IStockRegister.GetStockRegisterData(FromDate, ToDate,PartCode,ItemName,ItemGroup,ItemType,StoreId,ReportType,BatchNo,UniqueBatchNo);
            model.ReportMode= ReportType;
            var allData = model;
            MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                SlidingExpiration = TimeSpan.FromMinutes(55),
                Size = 1024,
            };

            var fullList = (await _IStockRegister.GetStockRegisterData(FromDate, ToDate, PartCode, ItemName, ItemGroup, ItemType, StoreId, ReportType, BatchNo, UniqueBatchNo))?.StockRegisterDetail ?? new List<StockRegisterDetail>();
           
            model.TotalRecords = fullList.Count();
            model.PageNumber = pageNumber;
            model.StockRegisterDetail = fullList;
            model.PageSize = pageSize;
           
            _MemoryCache.Set("KeyStockList", fullList, cacheEntryOptions);
            return PartialView("_StockRegisterGrid", model);
        }
        public IActionResult GlobalSearch(string searchString, int pageNumber = 1, int pageSize = 10)
        {
            StockRegisterModel model = new StockRegisterModel();
            if (string.IsNullOrWhiteSpace(searchString))
            {
                return PartialView("_StockRegisterGrid", new List<StockRegisterModel>());
            }

            if (!_MemoryCache.TryGetValue("KeyStockList", out IList<StockRegisterDetail> stockRegisterViewModel) || stockRegisterViewModel == null)
            {
                return PartialView("_StockRegisterGrid", new List<StockRegisterModel>());
            }

            List<StockRegisterDetail> filteredResults;

            if (string.IsNullOrWhiteSpace(searchString))
            {
                filteredResults = stockRegisterViewModel.ToList(); 
            }
            else
            {
                filteredResults = stockRegisterViewModel
                    .Where(i => i.GetType().GetProperties()
                        .Where(p => p.PropertyType == typeof(string))
                        .Select(p => p.GetValue(i)?.ToString())
                        .Any(value => !string.IsNullOrEmpty(value) &&
                                      value.Contains(searchString, StringComparison.OrdinalIgnoreCase)))
                    .ToList();

                
                if (filteredResults.Count == 0)
                {
                    filteredResults = stockRegisterViewModel.ToList();
                }
            }

            model.TotalRecords = filteredResults.Count;
            model.StockRegisterDetail = filteredResults.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            model.PageNumber = pageNumber;
            model.PageSize = pageSize;

            return PartialView("_StockRegisterGrid", model);
        }
        public async Task<JsonResult> GetAllItems()
        {
            var JSON = await _IStockRegister.GetAllItems();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }public async Task<JsonResult> GetAllItemGroups()
        {

            var JSON = await _IStockRegister.GetAllItemGroups();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetAllItemTypes()
        {
            var JSON = await _IStockRegister.GetAllItemTypes();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetAllStores()
        {
            var JSON = await _IStockRegister.GetAllStores();
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
