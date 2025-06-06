using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;

namespace eTactWeb.Controllers
{
    public class OrderAmendHistoryController : Controller
    {
        private readonly IDataLogic _IDataLogic;
        public IOrderAmendHistory _IOrderAmendHistory { get; }
        private readonly ILogger<OrderAmendHistoryController> _logger;
        private readonly IConfiguration iconfiguration;
        public IWebHostEnvironment _IWebHostEnvironment { get; }
        private readonly IMemoryCache _MemoryCache;

        public OrderAmendHistoryController(ILogger<OrderAmendHistoryController> logger, IDataLogic iDataLogic, EncryptDecrypt encryptDecrypt, IWebHostEnvironment iWebHostEnvironment, IConfiguration iconfiguration, IOrderAmendHistory IOrderAmendHistory, IMemoryCache iMemoryCache)
        {
            _logger = logger;
            _IDataLogic = iDataLogic;
            _IOrderAmendHistory = IOrderAmendHistory;
            _IWebHostEnvironment = iWebHostEnvironment;
            this.iconfiguration = iconfiguration;
            _MemoryCache = iMemoryCache;
        }
        public IActionResult OrderAmendHistory()
        {
            var model = new OrderAmendHistoryModel();
            model.OrderAmendHistoryGrid = new List<OrderAmendHistoryModel>();
            return View(model);
        }

        public async Task<IActionResult> GetOrderAmendHistoryData(string FromDate, string ToDate, string ReportType, int AccountCode, string PartCode, string ItemName, string PONO, int ItemCode,string HistoryReportMode, int pageNumber = 1, int pageSize = 20, string SearchBox = "")
        {
            var YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
            var model = new OrderAmendHistoryModel();
            model = await _IOrderAmendHistory.GetOrderAmendHistoryData( FromDate,  ToDate,  ReportType,  AccountCode,  PartCode,  ItemName,  PONO,  ItemCode, HistoryReportMode);
            model.ReportMode = ReportType;
            var modelList = model?.OrderAmendHistoryGrid ?? new List<OrderAmendHistoryModel>();


            if (string.IsNullOrWhiteSpace(SearchBox))
            {
                model.TotalRecords = modelList.Count();
                model.PageNumber = pageNumber;
                model.PageSize = pageSize;
                model.OrderAmendHistoryGrid = modelList
                .Skip((pageNumber - 1) * pageSize)
                   .Take(pageSize)
                   .ToList();
            }
            else
            {
                List<OrderAmendHistoryModel> filteredResults;
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
                model.OrderAmendHistoryGrid = filteredResults.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
                model.PageNumber = pageNumber;
                model.PageSize = pageSize;
            }
            MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                SlidingExpiration = TimeSpan.FromMinutes(55),
                Size = 1024,
            };

            _MemoryCache.Set("KeyOrderAmendHistoryGrid", modelList, cacheEntryOptions);

            string serializedGrid = JsonConvert.SerializeObject(modelList);
            HttpContext.Session.SetString("KeyOrderAmendHistoryGrid", serializedGrid);
            if (ReportType== "POSummary")
            {
                return PartialView("_OrderAmendHistorySummaryGrid", model);
            }
            else
            {

                return PartialView("_OrderAmendHistoryDetailGrid", model);
            }
            
        }

        [HttpGet]
        public IActionResult GlobalSearch(string searchString, string dashboardType = "Summary", int pageNumber = 1, int pageSize = 20)
        {
            OrderAmendHistoryModel model = new OrderAmendHistoryModel();
            model.ReportMode = dashboardType;
            if (string.IsNullOrWhiteSpace(searchString))
            {
                return PartialView("_OrderAmendHistorySummaryGrid", new List<OrderAmendHistoryModel>());
            }
            //string cacheKey = $"KeyProdList_{dashboardType}";
            if (!_MemoryCache.TryGetValue("KeyOrderAmendHistoryGrid", out IList<OrderAmendHistoryModel> poRegisterDetail) || poRegisterDetail == null)
            {
                return PartialView("_OrderAmendHistorySummaryGrid", new List<OrderAmendHistoryModel>());
            }

            List<OrderAmendHistoryModel> filteredResults;

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
            model.OrderAmendHistoryGrid = filteredResults.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            model.PageNumber = pageNumber;
            model.PageSize = pageSize;
            if (dashboardType == "POSummary")
            {
                return PartialView("_OrderAmendHistorySummaryGrid", model);
            }
            else
            {

                return PartialView("_OrderAmendHistoryDetailGrid", model);
            }

        }

    }
}
