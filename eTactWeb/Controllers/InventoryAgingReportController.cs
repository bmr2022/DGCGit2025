using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;

namespace eTactWeb.Controllers
{
    public class InventoryAgingReportController : Controller
    {
        private readonly IDataLogic _IDataLogic;
        public IInventoryAgingReport _IInventoryAgingReport { get; }
        private readonly ILogger<InventoryAgingReportController> _logger;
        private readonly IConfiguration iconfiguration;
        private readonly IMemoryCache _MemoryCache;
        public IWebHostEnvironment _IWebHostEnvironment { get; }
        public InventoryAgingReportController(ILogger<InventoryAgingReportController> logger, IDataLogic iDataLogic, IInventoryAgingReport iInventoryAgingReport, EncryptDecrypt encryptDecrypt, IWebHostEnvironment iWebHostEnvironment, IConfiguration iconfiguration, IMemoryCache iMemoryCache)
        {
            _logger = logger;
            _IDataLogic = iDataLogic;
            _IInventoryAgingReport = iInventoryAgingReport;
            _IWebHostEnvironment = iWebHostEnvironment;
            this.iconfiguration = iconfiguration;
            _MemoryCache = iMemoryCache;
        }
        [Route("{controller}/Index")]
        public async Task<ActionResult> InventoryAgingReport()
        {
            var model = new InventoryAgingReportModel();
            model.InventoryAgingReportGrid = new List<InventoryAgingReportModel>();

            return View(model);
        }
        public async Task<JsonResult> FillRMItemName(string FromDate, string ToDate, string CurrentDate, int Storeid)
        {
            var JSON = await _IInventoryAgingReport.FillRMItemName( FromDate,  ToDate,  CurrentDate,  Storeid);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillRMPartCode(string FromDate, string ToDate, string CurrentDate, int Storeid)
        {
            var JSON = await _IInventoryAgingReport.FillRMPartCode( FromDate,  ToDate,  CurrentDate,  Storeid);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillStoreName(string FromDate, string ToDate, string CurrentDate, int Storeid)
        {
            var JSON = await _IInventoryAgingReport.FillStoreName( FromDate,  ToDate,  CurrentDate,  Storeid);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillWorkCenterName(string FromDate, string ToDate, string CurrentDate, int Storeid)
        {
            var JSON = await _IInventoryAgingReport.FillWorkCenterName( FromDate,  ToDate,  CurrentDate,  Storeid);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<IActionResult> GetInventoryAgingReportDetailsData(string fromDate, string toDate,string CurrentDate, int WorkCenterid, string ReportType, int RMItemCode, int Storeid, int Foduration, int pageNumber = 1, int pageSize = 50, string SearchBox = "")
        {
            var model = new InventoryAgingReportModel();
            model = await _IInventoryAgingReport.GetInventoryAgingReportDetailsData(fromDate, toDate, CurrentDate, WorkCenterid, ReportType, RMItemCode, Storeid, Foduration);

            var modelList = model?.InventoryAgingReportGrid ?? new List<InventoryAgingReportModel>();


            if (string.IsNullOrWhiteSpace(SearchBox))
            {
                model.TotalRecords = modelList.Count();
                model.PageNumber = pageNumber;
                model.PageSize = pageSize;
                model.InventoryAgingReportGrid = modelList
                .Skip((pageNumber - 1) * pageSize)
                   .Take(pageSize)
                   .ToList();
            }
            else
            {
                List<InventoryAgingReportModel> filteredResults;
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
                model.InventoryAgingReportGrid = filteredResults.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
                model.PageNumber = pageNumber;
                model.PageSize = pageSize;
            }
            MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                SlidingExpiration = TimeSpan.FromMinutes(55),
                Size = 1024,
            };

            _MemoryCache.Set("KeyInventoryAgeingList", modelList, cacheEntryOptions);

            //string serializedGrid = JsonConvert.SerializeObject(modelList);
            if (ReportType == "AginingDataSummary")
            {
                return PartialView("_InventoryAgingReportSlabWiseGrid", model);
            }
            if (ReportType == "AginingDataBatchWise")
            {
                return PartialView("_InventoryAgingReportDetailGrid", model);
            }
            return null;
        }
        [HttpGet]
        public IActionResult GlobalSearch(string searchString, string dashboardType = "Summary", int pageNumber = 1, int pageSize = 50)
        {
            InventoryAgingReportModel model = new InventoryAgingReportModel();
            model.ReportType = dashboardType;
            if (string.IsNullOrWhiteSpace(searchString))
            {
                return PartialView("_InventoryAgingReportSlabWiseGrid", new List<InventoryAgingReportModel>());
            }
            //string cacheKey = $"KeyProdList_{dashboardType}";
            if (!_MemoryCache.TryGetValue("KeyInventoryAgeingList", out IList<InventoryAgingReportModel> inventoryAgingReport) || inventoryAgingReport == null)
            {
                return PartialView("_InventoryAgingReportSlabWiseGrid", new List<InventoryAgingReportModel>());
            }

            List<InventoryAgingReportModel> filteredResults;

            if (string.IsNullOrWhiteSpace(searchString))
            {
                filteredResults = inventoryAgingReport.ToList();
            }
            else
            {
                filteredResults = inventoryAgingReport
                    .Where(i => i.GetType().GetProperties()
                        .Where(p => p.PropertyType == typeof(string))
                        .Select(p => p.GetValue(i)?.ToString())
                        .Any(value => !string.IsNullOrEmpty(value) &&
                                      value.Contains(searchString, StringComparison.OrdinalIgnoreCase)))
                    .ToList();


                if (filteredResults.Count == 0)
                {
                    filteredResults = inventoryAgingReport.ToList();
                }
            }

            model.TotalRecords = filteredResults.Count;
            model.InventoryAgingReportGrid = filteredResults.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            model.PageNumber = pageNumber;
            model.PageSize = pageSize;
            if (model.ReportType == "AginingDataSummary")
            {
                return PartialView("_InventoryAgingReportSlabWiseGrid", model);
            }
            else 
            {
                return PartialView("_InventoryAgingReportDetailGrid", model);
            }
           
        }

    }
}
