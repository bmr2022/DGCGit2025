using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;

namespace eTactWeb.Controllers
{
    public class SaleOrderRegisterController : Controller
    {
        private readonly IDataLogic _IDataLogic;
        public ISaleOrderRegister _ISaleOrderRegister { get; }
        private readonly ILogger<SaleOrderRegisterController> _logger;
        private readonly IConfiguration iconfiguration;
        private readonly IMemoryCache _MemoryCache;
        public IWebHostEnvironment _IWebHostEnvironment { get; }
        public SaleOrderRegisterController(ILogger<SaleOrderRegisterController> logger, IDataLogic iDataLogic, ISaleOrderRegister iISaleOrderRegister, EncryptDecrypt encryptDecrypt, IWebHostEnvironment iWebHostEnvironment, IConfiguration iconfiguration, IMemoryCache iMemoryCache)
        {
            _logger = logger;
            _IDataLogic = iDataLogic;
            _ISaleOrderRegister = iISaleOrderRegister;
            _IWebHostEnvironment = iWebHostEnvironment;
            this.iconfiguration = iconfiguration;
            _MemoryCache = iMemoryCache;
        }
        [Route("{controller}/Index")]
        public async Task<ActionResult> SaleOrderRegister()
        {
            var model = new SaleOrderRegisterModel();
            model.saleOrderRegisterGrid = new List<SaleOrderRegisterModel>();

            return View(model);
        }
        public async Task<JsonResult> FillPartCode(string FromDate, string ToDate)
        {
            var JSON = await _ISaleOrderRegister.FillPartCode(FromDate,ToDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
         public async Task<JsonResult> FillSaleOrderNo(string FromDate, string ToDate)
        {
            var JSON = await _ISaleOrderRegister.FillSaleOrderNo(FromDate,ToDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
         public async Task<JsonResult> FillCustOrderNo(string FromDate, string ToDate)
        {
            var JSON = await _ISaleOrderRegister.FillCustOrderNo(FromDate,ToDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
         public async Task<JsonResult> FillSchNo(string FromDate, string ToDate)
        {
            var JSON = await _ISaleOrderRegister.FillSchNo(FromDate,ToDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
         public async Task<JsonResult> FillCustomerName(string FromDate, string ToDate)
        {
            var JSON = await _ISaleOrderRegister.FillCustomerName(FromDate,ToDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
         public async Task<JsonResult> FillSalesPerson(string FromDate, string ToDate)
        {
            var JSON = await _ISaleOrderRegister.FillSalesPerson(FromDate,ToDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<IActionResult> GetSaleOrderDetailsData(string OrderSchedule, string ReportType, string PartCode, string ItemName, string Sono, string CustOrderNo, string CustomerName, string SalesPersonName, string SchNo, string FromDate, string ToDate, int pageNumber = 1, int pageSize = 50, string SearchBox = "")
        {
            var model = new SaleOrderRegisterModel();
            model = await _ISaleOrderRegister.GetSaleOrderDetailsData( OrderSchedule,  ReportType,  PartCode,  ItemName,  Sono,  CustOrderNo,  CustomerName,  SalesPersonName,  SchNo,  FromDate,  ToDate);

            var modelList = model?.saleOrderRegisterGrid ?? new List<SaleOrderRegisterModel>();


            if (string.IsNullOrWhiteSpace(SearchBox))
            {
                model.TotalRecords = modelList.Count();
                model.PageNumber = pageNumber;
                model.PageSize = pageSize;
                model.saleOrderRegisterGrid = modelList
                .Skip((pageNumber - 1) * pageSize)
                   .Take(pageSize)
                   .ToList();
            }
            else
            {
                List<SaleOrderRegisterModel> filteredResults;
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
                model.saleOrderRegisterGrid = filteredResults.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
                model.PageNumber = pageNumber;
                model.PageSize = pageSize;
            }
            MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                SlidingExpiration = TimeSpan.FromMinutes(55),
                Size = 1024,
            };

            _MemoryCache.Set("KeySaleOrderList", modelList, cacheEntryOptions);

            //string serializedGrid = JsonConvert.SerializeObject(modelList);
            if (ReportType == "Schedule Summary")
            {
                return PartialView("_SaleOrderScheduleSummaryRegisterGrid", model);
            }
            
            return null;
        }
    }
}
