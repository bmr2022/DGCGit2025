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
    public class WIPStockRegisterController : Controller
    {
        private readonly IDataLogic _IDataLogic;
        public IWIPStockRegister _IWIPStockRegister { get; }

        private readonly ILogger<WIPStockRegisterController> _logger;
        private readonly IConfiguration iconfiguration;
        private readonly IMemoryCache _MemoryCache;
        public IWebHostEnvironment _IWebHostEnvironment { get; }

        public WIPStockRegisterController(ILogger<WIPStockRegisterController> logger, IDataLogic iDataLogic, IWIPStockRegister iWIPStockRegister, IMemoryCache iMemoryCache, EncryptDecrypt encryptDecrypt, IWebHostEnvironment iWebHostEnvironment, IConfiguration iconfiguration)
        {
            _logger = logger;
            _IDataLogic = iDataLogic;
            _IWIPStockRegister = iWIPStockRegister;
            _MemoryCache = iMemoryCache;
            _IWebHostEnvironment = iWebHostEnvironment;
            this.iconfiguration = iconfiguration;
        }
        [Route("{controller}/Index")]
        public IActionResult WIPStockRegister()
        {
            var model = new WIPStockRegisterModel();
            model.WIPStockRegisterDetail = new List<WIPStockRegisterDetail>();
            return View(model);
        }

        public async Task<IActionResult> GetWIPRegisterData(string FromDate, string ToDate, string PartCode, string ItemName, string ItemGroup, string ItemType, int WCID, string ReportType, string BatchNo, string UniqueBatchNo, string WorkCenter, int pageNumber = 1, int pageSize = 500, string SearchBox = "")
        {
            var model = new WIPStockRegisterModel();
            var fullList = (await _IWIPStockRegister.GetStockRegisterData(FromDate, ToDate, PartCode, ItemName, ItemGroup, ItemType, WCID, ReportType, BatchNo, UniqueBatchNo, WorkCenter))?.WIPStockRegisterDetail ?? new List<WIPStockRegisterDetail>();
            model.ReportMode= ReportType;
            if (string.IsNullOrWhiteSpace(SearchBox))
            {
                model.TotalRecords = fullList.Count();
                model.PageNumber = pageNumber;
                model.PageSize = pageSize;

                model.WIPStockRegisterDetail = fullList
                   .Skip((pageNumber - 1) * pageSize)
                   .Take(pageSize)
                   .ToList();
            }
            else
            {
                List<WIPStockRegisterDetail> filteredResults;
                if (string.IsNullOrWhiteSpace(SearchBox))
                {
                    filteredResults = fullList.ToList();
                }
                else
                {
                    filteredResults = fullList
                        .Where(i => i.GetType().GetProperties()
                            .Where(p => p.PropertyType == typeof(string))
                            .Select(p => p.GetValue(i)?.ToString())
                            .Any(value => !string.IsNullOrEmpty(value) &&
                                          value.Contains(SearchBox, StringComparison.OrdinalIgnoreCase)))
                        .ToList();


                    if (filteredResults.Count == 0)
                    {
                        filteredResults = fullList.ToList();
                    }
                }

                model.TotalRecords = filteredResults.Count;
                model.WIPStockRegisterDetail = filteredResults.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
                model.PageNumber = pageNumber;
                model.PageSize = pageSize;
            }
           
            MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                SlidingExpiration = TimeSpan.FromMinutes(55),
                Size = 1024,
            };

            _MemoryCache.Set("KeyWIPStockList", fullList, cacheEntryOptions);
            return PartialView("_WIPStockRegisterGrid", model);
        }
        [HttpGet]
        public IActionResult GlobalSearch(string searchString, int pageNumber = 1, int pageSize = 500)
        {
            WIPStockRegisterModel model = new WIPStockRegisterModel();
            if (string.IsNullOrWhiteSpace(searchString))
            {
                return PartialView("_WIPStockRegisterGrid", new List<WIPStockRegisterModel>());
            }

            if (!_MemoryCache.TryGetValue("KeyWIPStockList", out IList<WIPStockRegisterDetail> wipRegisterViewModel) || wipRegisterViewModel == null)
            {
                return PartialView("_WIPStockRegisterGrid", new List<WIPStockRegisterModel>());
            }

            List<WIPStockRegisterDetail> filteredResults;

            if (string.IsNullOrWhiteSpace(searchString))
            {
                filteredResults = wipRegisterViewModel.ToList();
            }
            else
            {
                filteredResults = wipRegisterViewModel
                    .Where(i => i.GetType().GetProperties()
                        .Where(p => p.PropertyType == typeof(string))
                        .Select(p => p.GetValue(i)?.ToString())
                        .Any(value => !string.IsNullOrEmpty(value) &&
                                      value.Contains(searchString, StringComparison.OrdinalIgnoreCase)))
                    .ToList();


                if (filteredResults.Count == 0)
                {
                    filteredResults = wipRegisterViewModel.ToList();
                }
            }

            model.TotalRecords = filteredResults.Count;
            model.WIPStockRegisterDetail = filteredResults.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            model.PageNumber = pageNumber;
            model.PageSize = pageSize;

            return PartialView("_WIPStockRegisterGrid", model);
        }
        public async Task<JsonResult> GetAllWorkCenter()
        {
            var JSON = await _IWIPStockRegister.GetAllWorkCenter();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }

        public async Task<JsonResult> GetAllItemGroups()
        {

            var JSON = await _IWIPStockRegister.GetAllItemGroups();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetAllItemTypes()
        {
            var JSON = await _IWIPStockRegister.GetAllItemTypes();
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
