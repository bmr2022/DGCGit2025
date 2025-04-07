using eTactWeb.Data.Common;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using eTactWeb.DOM.Models;
using System.Globalization;
using System.Drawing.Printing;
using ClosedXML.Excel;

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
        public async Task<IActionResult> GetStockRegisterData(string FromDate, string ToDate, string PartCode, string ItemName, string ItemGroup, string ItemType, int StoreId, string ReportType, string BatchNo, string UniqueBatchNo, int pageNumber = 1, int pageSize = 500, string SearchBox = "")
        {
            var model = new StockRegisterModel();
            model.ReportMode = ReportType;
            var fullList = (await _IStockRegister.GetStockRegisterData(FromDate, ToDate, PartCode, ItemName, ItemGroup, ItemType, StoreId, ReportType, BatchNo, UniqueBatchNo))?.StockRegisterDetail ?? new List<StockRegisterDetail>();
            if (string.IsNullOrWhiteSpace(SearchBox))
            {
                model.TotalRecords = fullList.Count();
                model.PageNumber = pageNumber;
                model.PageSize = pageSize;

                // Apply server-side paging here
                model.StockRegisterDetail = fullList
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();
            }
            else
            {
                List<StockRegisterDetail> filteredResults;
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
                model.StockRegisterDetail = filteredResults.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
                model.PageNumber = pageNumber;
                model.PageSize = pageSize;
            }
            // Optional: Cache full list if needed
            MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                SlidingExpiration = TimeSpan.FromMinutes(55),
                Size = 1024,
            };
            _MemoryCache.Set("KeyStockList", fullList, cacheEntryOptions);

            return PartialView("_StockRegisterGrid", model);
        }
        [HttpGet]
        public IActionResult GlobalSearch(string searchString, int pageNumber = 1, int pageSize = 500)
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
        public async Task<JsonResult> FillItemName()
        {
            var JSON = await _IStockRegister.FillItemName();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetAllItemGroups()
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
        [HttpGet]
        public async Task<IActionResult> ExportStockRegisterToExcel(string ReportType)
        {
            if (!_MemoryCache.TryGetValue("KeyStockList", out IList<StockRegisterDetail> stockRegisterList))
                return NotFound("No data available to export.");

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Stock Register");

            var reportGenerators = new Dictionary<string, Action<IXLWorksheet, IList<StockRegisterDetail>>>
            {
                { "STOCKSUMMARY", ExportStockSummary },
                { "STOCKDETAIL", ExportStockDetail }
                // Add more report types here if needed
            };

            if (reportGenerators.TryGetValue(ReportType, out var generator))
            {
                generator(worksheet, stockRegisterList);
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
                "StockRegisterReport.xlsx"
            );
        }
        private void ExportStockSummary(IXLWorksheet sheet, IList<StockRegisterDetail> list)
        {
            string[] headers = {
        "Sr#", "Store Name", "Part Code", "Item Name", "Opn Stk", "Rec Qty", "Iss Qty", "Tot Stk",
        "Std Packing", "Unit", "Avg Rate", "Amount", "Min Level", "Alt Unit", "Alt Stock",
        "Group Name", "Bin No", "Maximum Level", "Reorder Level"
    };

            for (int i = 0; i < headers.Length; i++)
                sheet.Cell(1, i + 1).Value = headers[i];

            int row = 2, srNo = 1;
            foreach (var item in list)
            {
                sheet.Cell(row, 1).Value = srNo++;
                sheet.Cell(row, 2).Value = item.StoreName;
                sheet.Cell(row, 3).Value = item.PartCode;
                sheet.Cell(row, 4).Value = item.ItemName;
                sheet.Cell(row, 5).Value = item.OpnStk;
                sheet.Cell(row, 6).Value = item.RecQty;
                sheet.Cell(row, 7).Value = item.IssQty;
                sheet.Cell(row, 8).Value = item.TotStk;
                sheet.Cell(row, 9).Value = item.StdPacking;
                sheet.Cell(row, 10).Value = item.Unit;
                sheet.Cell(row, 11).Value = item.AvgRate;
                sheet.Cell(row, 12).Value = item.Amount;
                sheet.Cell(row, 13).Value = item.MinLevel;
                sheet.Cell(row, 14).Value = item.AltUnit;
                sheet.Cell(row, 15).Value = item.AltStock;
                sheet.Cell(row, 16).Value = item.GroupName;
                sheet.Cell(row, 17).Value = item.BinNo;
                sheet.Cell(row, 18).Value = item.MaximumLevel;
                sheet.Cell(row, 19).Value = item.ReorderLevel;
                row++;
            }
        }

        private void ExportStockDetail(IXLWorksheet sheet, IList<StockRegisterDetail> list)
        {
            string[] headers = {
        "Sr#", "Store Name", "Transaction Type", "Trans Date", "Part Code", "Item Name", "Opn Stk",
        "Rec Qty", "Iss Qty", "Tot Stk", "Unit", "Rate", "Amount", "Min Level", "Alt Unit",
        "Alt Stock", "Bill No", "Bill Date", "Party Name", "MRN No", "Batch No", "Unique Batch No",
        "Entry Id", "Alt Rec Qty", "Alt Iss Qty", "Group Name", "Package"
    };

            for (int i = 0; i < headers.Length; i++)
                sheet.Cell(1, i + 1).Value = headers[i];

            int row = 2, srNo = 1;
            foreach (var item in list)
            {
                sheet.Cell(row, 1).Value = srNo++;
                sheet.Cell(row, 2).Value = item.StoreName;
                sheet.Cell(row, 3).Value = item.TransactionType;
                sheet.Cell(row, 4).Value = item.TransDate;
                sheet.Cell(row, 5).Value = item.PartCode;
                sheet.Cell(row, 6).Value = item.ItemName;
                sheet.Cell(row, 7).Value = item.OpnStk;
                sheet.Cell(row, 8).Value = item.RecQty;
                sheet.Cell(row, 9).Value = item.IssQty;
                sheet.Cell(row, 10).Value = item.TotStk;
                sheet.Cell(row, 11).Value = item.Unit;
                sheet.Cell(row, 12).Value = item.Rate;
                sheet.Cell(row, 13).Value = item.Amount;
                sheet.Cell(row, 14).Value = item.MinLevel;
                sheet.Cell(row, 15).Value = item.AltUnit;
                sheet.Cell(row, 16).Value = item.AltStock;
                sheet.Cell(row, 17).Value = item.BillNo;
                sheet.Cell(row, 18).Value = item.BillDate;
                sheet.Cell(row, 19).Value = item.PartyName;
                sheet.Cell(row, 20).Value = item.MRNNo;
                sheet.Cell(row, 21).Value = item.BatchNo;
                sheet.Cell(row, 22).Value = item.UniquebatchNo;
                sheet.Cell(row, 23).Value = item.EntryId;
                sheet.Cell(row, 24).Value = item.AltRecQty;
                sheet.Cell(row, 25).Value = item.AltIssQty;
                sheet.Cell(row, 26).Value = item.GroupName;
                sheet.Cell(row, 27).Value = item.package;
                row++;
            }
        }
    }
}
