using ClosedXML.Excel;
using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;

namespace eTactWeb.Controllers
{
    public class OrderBasedProdPlanController : Controller
    {
        private readonly IDataLogic _IDataLogic;
        public IOrderBasedProdPlan _IOrderBasedProdPlan { get; }
        private readonly ILogger<OrderBasedProdPlanController> _logger;
        private readonly IConfiguration iconfiguration;
        public IWebHostEnvironment _IWebHostEnvironment { get; }
        private readonly IMemoryCache _MemoryCache;

        public OrderBasedProdPlanController(ILogger<OrderBasedProdPlanController> logger, IDataLogic iDataLogic, EncryptDecrypt encryptDecrypt, IWebHostEnvironment iWebHostEnvironment, IConfiguration iconfiguration, IOrderBasedProdPlan IOrderBasedProdPlan, IMemoryCache iMemoryCache)
        {
            _logger = logger;
            _IDataLogic = iDataLogic;
            _IOrderBasedProdPlan = IOrderBasedProdPlan;
            _IWebHostEnvironment = iWebHostEnvironment;
            this.iconfiguration = iconfiguration;
            _MemoryCache = iMemoryCache;
        }
        public IActionResult OrderBasedProdPlan()
        {
            var model = new OrderBasedProdPlanModel();
            model.OrderBasedProdPlanList = new List<OrderBasedProdPlanModel>();
            return View(model);
        }
        public async Task<JsonResult> FillSONO_OrderNO_SchNo(string FromDate, string ToDate)
        {
            var result = await _IOrderBasedProdPlan.FillSONO_OrderNO_SchNo(FromDate, ToDate);
            //return Json(new { StatusCode = 200, StatusText = "Success", Result = result }); 
            return Json(result);
        }
        public async Task<IActionResult> GetOrderBasedProdPlanData(string FromDate, string ToDate, string ReportType, int AccountCode, string PartCode, string ItemName, int ItemCode, int pageNumber = 1, int pageSize = 20, string SearchBox = "")
        {
            var YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
            var model = new OrderBasedProdPlanModel();
            model = await _IOrderBasedProdPlan.GetOrderBasedProdPlanData(FromDate, ToDate, ReportType, AccountCode, PartCode, ItemName, ItemCode);
            model.ReportMode = ReportType;
            var modelList = model?.OrderBasedProdPlanList ?? new List<OrderBasedProdPlanModel>();


            if (string.IsNullOrWhiteSpace(SearchBox))
            {
                model.TotalRecords = modelList.Count();
                model.PageNumber = pageNumber;
                model.PageSize = pageSize;
                model.OrderBasedProdPlanList = modelList
                .Skip((pageNumber - 1) * pageSize)
                   .Take(pageSize)
                   .ToList();
            }
            else
            {
                List<OrderBasedProdPlanModel> filteredResults;
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
                model.OrderBasedProdPlanList = filteredResults.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
                model.PageNumber = pageNumber;
                model.PageSize = pageSize;
            }
            MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                SlidingExpiration = TimeSpan.FromMinutes(55),
                Size = 1024,
            };

            _MemoryCache.Set("KeyOrderBasedProdPlanGrid", modelList, cacheEntryOptions);

            string serializedGrid = JsonConvert.SerializeObject(modelList);
            HttpContext.Session.SetString("KeyOrderBasedProdPlanGrid", serializedGrid);
            if (ReportType == "Item+WC wise Plan")
            {
                return PartialView("_OrderBasedProdPlanItem+WCWiseGrid", model);
            }
            else
            {

                return PartialView("_OrderBasedProdPlanItem+WCWiseGrid", model);
            }

        }
        [HttpGet]
        public IActionResult GlobalSearch(string searchString, string dashboardType = "Summary", int pageNumber = 1, int pageSize = 20)
       {
            OrderBasedProdPlanModel model = new OrderBasedProdPlanModel();
            model.ReportMode = dashboardType;
            if (string.IsNullOrWhiteSpace(searchString))
            {
                return PartialView("_OrderBasedProdPlanItem+WCWiseGrid", new List<OrderBasedProdPlanModel>());
            }
            //string cacheKey = $"KeyProdList_{dashboardType}";
            if (!_MemoryCache.TryGetValue("KeyOrderBasedProdPlanGrid", out IList<OrderBasedProdPlanModel> OrderBasedProdPlanList) || OrderBasedProdPlanList == null)
            {
                return PartialView("_OrderBasedProdPlanItem+WCWiseGrid", new List<OrderBasedProdPlanModel>());
            }

            List<OrderBasedProdPlanModel> filteredResults;

            if (string.IsNullOrWhiteSpace(searchString))
            {
                filteredResults = OrderBasedProdPlanList.ToList();
            }
            else
            {
                filteredResults = OrderBasedProdPlanList
                    .Where(i => i.GetType().GetProperties()
                        .Where(p => p.PropertyType == typeof(string))
                        .Select(p => p.GetValue(i)?.ToString())
                        .Any(value => !string.IsNullOrEmpty(value) &&
                                      value.Contains(searchString, StringComparison.OrdinalIgnoreCase)))
                    .ToList();


                if (filteredResults.Count == 0)
                {
                    filteredResults = OrderBasedProdPlanList.ToList();
                }
            }

            model.TotalRecords = filteredResults.Count;
            model.OrderBasedProdPlanList = filteredResults.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            model.PageNumber = pageNumber;
            model.PageSize = pageSize;
            if (dashboardType == "Item+WC wise Plan")
            {
                return PartialView("_OrderBasedProdPlanItem+WCWiseGrid", model);
            }
            else
            {

                return PartialView("_OrderBasedProdPlanItem+WCWiseGrid", model);
            }

        }
        public async Task<IActionResult> ExportOrderBasedProdPlanToExcel(string ReportType)
        {
            string modelJson = HttpContext.Session.GetString("KeyOrderBasedProdPlanGrid");
            List<OrderBasedProdPlanModel> OrderBasedProdPlanList = new List<OrderBasedProdPlanModel>();
            if (!string.IsNullOrEmpty(modelJson))
            {
                OrderBasedProdPlanList = JsonConvert.DeserializeObject<List<OrderBasedProdPlanModel>>(modelJson);
            }

            if (OrderBasedProdPlanList == null)
                return NotFound("No data available to export.");

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("PO Register");

            var reportGenerators = new Dictionary<string, Action<IXLWorksheet, IList<OrderBasedProdPlanModel>>>
            {
                { "Item+WC wise Plan", EXPORTLISTOFItemWCWise }


            };

            if (reportGenerators.TryGetValue(ReportType, out var generator))
            {
                generator(worksheet, OrderBasedProdPlanList);
            }
            else
            {
                return BadRequest("Invalid report type.");
            }

            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Position = 0;

            return File(
                stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "OrderBasedProdPlan.xlsx"
            );
        }
        private void EXPORTLISTOFItemWCWise(IXLWorksheet sheet, IList<OrderBasedProdPlanModel> list)
        {
            string[] headers = {
                "Sr#", "PartCode", "ItemName", "WCName", "1", "2", "3",
                "4", "5", "6", "7", "8", "9",
                "10", "11", "18"
            };


            for (int i = 0; i < headers.Length; i++)
                sheet.Cell(1, i + 1).Value = headers[i];

            int row = 2, srNo = 1;
            foreach (var item in list)
            {
                sheet.Cell(row, 1).Value = srNo++;
                sheet.Cell(row, 2).Value = item.PartCode;
                sheet.Cell(row, 3).Value = item.ItemName;
                sheet.Cell(row, 4).Value = item.WCName;
                sheet.Cell(row, 5).Value = item.Day1;
                sheet.Cell(row, 6).Value = item.Day2;
                sheet.Cell(row, 7).Value = item.Day3;
                sheet.Cell(row, 8).Value = item.Day4;
                sheet.Cell(row, 9).Value = item.Day5;
                sheet.Cell(row, 10).Value = item.Day6;
                sheet.Cell(row, 11).Value = item.Day7;
                sheet.Cell(row, 12).Value = item.Day8;
                sheet.Cell(row, 13).Value = item.Day9;
                sheet.Cell(row, 14).Value = item.Day10;
                sheet.Cell(row, 15).Value = item.Day11;
                sheet.Cell(row, 16).Value = item.Day18;
               

                row++;
            }
        }
    }
}
