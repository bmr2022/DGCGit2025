using eTactWeb.Data.Common;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json.Linq;
using eTactWeb.DOM.Models;
using System.Net;
using System.Data;
using System.Globalization;
using Newtonsoft.Json;
using System.Runtime.Caching;
using System.Reflection;
using System.Xml.Linq;

namespace eTactWeb.Controllers
{
    public class PORegisterController : Controller
    {
        private readonly IDataLogic _IDataLogic;
        public IPORegister _IPORegister { get; }
        private readonly ILogger<PORegisterController> _logger;
        private readonly IConfiguration iconfiguration;
        public IWebHostEnvironment _IWebHostEnvironment { get; }
        private readonly IMemoryCache _MemoryCache;

        public PORegisterController(ILogger<PORegisterController> logger, IDataLogic iDataLogic, EncryptDecrypt encryptDecrypt, IWebHostEnvironment iWebHostEnvironment, IConfiguration iconfiguration, IPORegister pORegister, IMemoryCache iMemoryCache)
        {
            _logger = logger;
            _IDataLogic = iDataLogic;
            _IPORegister = pORegister;
            _IWebHostEnvironment = iWebHostEnvironment;
            this.iconfiguration = iconfiguration;
            _MemoryCache = iMemoryCache;
        }
        public IActionResult PORegister()
        {
            var model = new PORegisterModel();
            model.PORegisterDetails = new List<PORegisterDetail>();
            return View(model);
        }

        public async Task<IActionResult> GetPORegisterData(string FromDate, string ToDate, string ReportType, string  Partyname, string  partcode, string  itemName, string  POno, string  SchNo, string  OrderType, string  POFor, string  ItemType, string  ItemGroup, int pageNumber = 1, int pageSize = 50, string SearchBox = "")
        {
            var YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
            var model = new PORegisterModel();
            model = await _IPORegister.GetPORegisterData(FromDate, ToDate, ReportType, YearCode, Partyname, partcode, itemName, POno, SchNo, OrderType, POFor, ItemType, ItemGroup);
            model.ReportMode = ReportType;
            var modelList = model?.PORegisterDetails ?? new List<PORegisterDetail>();


            if (string.IsNullOrWhiteSpace(SearchBox))
            {
                model.TotalRecords = modelList.Count();
                model.PageNumber = pageNumber;
                model.PageSize = pageSize;
                model.PORegisterDetails = modelList
                .Skip((pageNumber - 1) * pageSize)
                   .Take(pageSize)
                   .ToList();
            }
            else
            {
                List<PORegisterDetail> filteredResults;
                if (string.IsNullOrWhiteSpace(SearchBox))
                {
                    filteredResults = modelList.ToList();
                }
                else
                {
                    filteredResults = modelList
                        .Where(i => i.GetType().GetProperties()
                            .Where(p => p.PropertyType == typeof(string))
                            .Select(p => p.GetValue(i)?.ToString())
                            .Any(value => !string.IsNullOrEmpty(value) &&
                                          value.Contains(SearchBox, StringComparison.OrdinalIgnoreCase)))
                        .ToList();


                    if (filteredResults.Count == 0)
                    {
                        filteredResults = modelList.ToList();
                    }
                }

                model.TotalRecords = filteredResults.Count;
                model.PORegisterDetails = filteredResults.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
                model.PageNumber = pageNumber;
                model.PageSize = pageSize;
            }
            MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                SlidingExpiration = TimeSpan.FromMinutes(55),
                Size = 1024,
            };

            _MemoryCache.Set("KeyPOList", modelList, cacheEntryOptions);
          
            //string serializedGrid = JsonConvert.SerializeObject(model.PORegisterDetails);
            //HttpContext.Session.SetString("KeyPORegsiterList", serializedGrid);
            return PartialView("_PORegisterGrid", model);
        }
        [HttpGet]
        public IActionResult GlobalSearch(string searchString, string dashboardType = "Summary", int pageNumber = 1, int pageSize = 50)
        {
            PORegisterModel model = new PORegisterModel();
            model.ReportMode = dashboardType;
            if (string.IsNullOrWhiteSpace(searchString))
            {
                return PartialView("_PORegisterGrid", new List<PORegisterDetail>());
            }
            //string cacheKey = $"KeyProdList_{dashboardType}";
            if (!_MemoryCache.TryGetValue("KeyPOList", out IList<PORegisterDetail> poRegisterDetail) || poRegisterDetail == null)
            {
                return PartialView("_PORegisterGrid", new List<PORegisterDetail>());
            }

            List<PORegisterDetail> filteredResults;

            if (string.IsNullOrWhiteSpace(searchString))
            {
                filteredResults = poRegisterDetail.ToList();
            }
            else
            {
                filteredResults = poRegisterDetail
                    .Where(i => i.GetType().GetProperties()
                        .Where(p => p.PropertyType == typeof(string))
                        .Select(p => p.GetValue(i)?.ToString())
                        .Any(value => !string.IsNullOrEmpty(value) &&
                                      value.Contains(searchString, StringComparison.OrdinalIgnoreCase)))
                    .ToList();


                if (filteredResults.Count == 0)
                {
                    filteredResults = poRegisterDetail.ToList();
                }
            }

            model.TotalRecords = filteredResults.Count;
            model.PORegisterDetails = filteredResults.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            model.PageNumber = pageNumber;
            model.PageSize = pageSize;
            if (model.ReportMode == "SUMM")
            {
                return PartialView("_POSummaryReport", model);
            }
            else if (model.ReportMode == "SUMMRATEING")
            {
                return PartialView("_POSumRatingReport", model);
            }
            else if (model.ReportMode == "LISTOFSCHEDULESUMMARY")
            {
                return PartialView("_ListOfPOSummaryReport", model);
            }
            else if (model.ReportMode == "LISTOFSCHEDULE")
            {
                return PartialView("_ListOfSchduleReport", model);
            }
            else if (model.ReportMode == "CONSOLIDATED")
            {
                return PartialView("_POConsolidatedReport", model);
            }
            else if (model.ReportMode == "PARTYWISECONSOLIDATED")
            {
                return PartialView("_POPartyWiseConsolidatedReport", model);
            }
            else if (model.ReportMode == "ITEMWISECONSOLIDATED")
            {
                return PartialView("_POItemWiseConsolidatedReport", model);
            }
            else if (model.ReportMode == "PRICEHISTORY")
            {
                return PartialView("_POPriceHistoryReport", model);
            }
            else
            {
                return PartialView("_PORegisterGrid", model);
            }
        }          
            
    
        //[HttpGet]
        //public IActionResult GetPORegisterDataForPDF()
        //{
        //    string modelJson = HttpContext.Session.GetString("KeyPORegsiterList");
        //    List<PORegisterDetail> stockRegisterList = new List<PORegisterDetail>();
        //    if (!string.IsNullOrEmpty(modelJson))
        //    {
        //        stockRegisterList = JsonConvert.DeserializeObject<List<PORegisterDetail>>(modelJson);
        //    }

        //    return Json(stockRegisterList);
        //}
        [HttpGet]
        public IActionResult GetPORegisterDataForPDF()
        {
            if (_MemoryCache.TryGetValue("KeyPOList", out List<PORegisterDetail> stockRegisterList))
            {
                return Json(stockRegisterList);
            }
            return Json(new List<PORegisterDetail>());
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
