using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace eTactWeb.Controllers
{
    public class VendoreRatingAnalysisReportController : Controller
    {
        private readonly IDataLogic _IDataLogic;
        public IVendoreRatingAnalysisReport _IVendoreRatingAnalysisReport { get; }
        private readonly ILogger<VendoreRatingAnalysisReportController> _logger;
        private readonly IConfiguration iconfiguration;
        private readonly IMemoryCache _MemoryCache;
        public IWebHostEnvironment _IWebHostEnvironment { get; }
        public VendoreRatingAnalysisReportController(ILogger<VendoreRatingAnalysisReportController> logger, IDataLogic iDataLogic, IVendoreRatingAnalysisReport iVendoreRatingAnalysisReport, EncryptDecrypt encryptDecrypt, IWebHostEnvironment iWebHostEnvironment, IConfiguration iconfiguration, IMemoryCache iMemoryCache)
        {
            _logger = logger;
            _IDataLogic = iDataLogic;
            _IVendoreRatingAnalysisReport = iVendoreRatingAnalysisReport;
            _IWebHostEnvironment = iWebHostEnvironment;
            this.iconfiguration = iconfiguration;
            _MemoryCache = iMemoryCache;
        }
        [Route("{controller}/Index")]
        public async Task<ActionResult> VendoreRatingAnalysisReport()
        {
            var model = new VendoreRatingAnalysisReportModel();
            model.YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
            model.VendoreRatingAnalysisReportGrid = new List<VendoreRatingAnalysisReportModel>();

            return View(model);
        }
        public async Task<IActionResult> GetVendoreRatingDetailsData(string ReportType,string RatingType, string CurrentDate, string PartCode, string ItemName, string CustomerName, int YearCode, int pageNumber = 1, int pageSize = 50, string SearchBox = "")
        {
            var model = new VendoreRatingAnalysisReportModel();
            model = await _IVendoreRatingAnalysisReport.GetVendoreRatingDetailsData( ReportType, RatingType,  CurrentDate,  PartCode,  ItemName,  CustomerName,  YearCode);

            var modelList = model?.VendoreRatingAnalysisReportGrid ?? new List<VendoreRatingAnalysisReportModel>();


            if (string.IsNullOrWhiteSpace(SearchBox))
            {
                model.TotalRecords = modelList.Count();
                model.PageNumber = pageNumber;
                model.PageSize = pageSize;
                model.VendoreRatingAnalysisReportGrid = modelList
                .Skip((pageNumber - 1) * pageSize)
                   .Take(pageSize)
                   .ToList();
            }
            else
            {
                List<VendoreRatingAnalysisReportModel> filteredResults;
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
                model.VendoreRatingAnalysisReportGrid = filteredResults.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
                model.PageNumber = pageNumber;
                model.PageSize = pageSize;
            }
            MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                SlidingExpiration = TimeSpan.FromMinutes(55),
                Size = 1024,
            };

            _MemoryCache.Set("KeyVendoreRatingAnalysisReportList", modelList, cacheEntryOptions);

            if (ReportType == "SUMMARY" && RatingType== "DELIVERY RATING")
            {
                return PartialView("_VendorRatingAnalysisDeliverySummary", model);
            }
            if (ReportType == "DETAIL" && RatingType== "DELIVERY RATING")
            {
                return PartialView("_VendorRatingAnalysisDeliveryDetail", model);
            }
            
            return null;
        }
        [HttpGet]
        public IActionResult GlobalSearch(string searchString, string ReportType, string RatingType, int pageNumber = 1, int pageSize = 50)
        {
            VendoreRatingAnalysisReportModel model = new VendoreRatingAnalysisReportModel();
            model.ReportType = ReportType;
            model.RatingType = RatingType;
            if (string.IsNullOrWhiteSpace(searchString))
            {
                return PartialView("_VendorRatingAnalysisDeliverySummary", new List<VendoreRatingAnalysisReportModel>());
            }
       
            if (!_MemoryCache.TryGetValue("KeyVendoreRatingAnalysisReportList", out IList<VendoreRatingAnalysisReportModel> inventoryAgingReport) || inventoryAgingReport == null)
            {
                return PartialView("_VendorRatingAnalysisDeliverySummary", new List<VendoreRatingAnalysisReportModel>());
            }

            List<VendoreRatingAnalysisReportModel> filteredResults;

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
            model.VendoreRatingAnalysisReportGrid = filteredResults.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            model.PageNumber = pageNumber;
            model.PageSize = pageSize;
            if (ReportType == "SUMMARY" && RatingType == "DELIVERY RATING")
            {
                return PartialView("_VendorRatingAnalysisDeliverySummary", model);
            }
            if (ReportType == "DETAIL" && RatingType == "DELIVERY RATING")
            {
                return PartialView("_VendorRatingAnalysisDeliveryDetail", model);
            }
            return null;
        }
    }
}
