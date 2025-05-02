using eTactWeb.Data.Common;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using eTactWeb.DOM.Models;
using System.Globalization;
using ClosedXML.Excel;

namespace eTactWeb.Controllers
{
	public class WIPStockRegisterController : Controller
    {
        private readonly IDataLogic _IDataLogic;
        public IWIPStockRegister _IWIPStockRegister { get; }
        private readonly ILogger<WIPStockRegisterController> _logger;
        private readonly IConfiguration iconfiguration;
        public IWebHostEnvironment _IWebHostEnvironment { get; }
        public WIPStockRegisterController(ILogger<WIPStockRegisterController> logger, IDataLogic iDataLogic, IWIPStockRegister iWIPStockRegister, EncryptDecrypt encryptDecrypt, IWebHostEnvironment iWebHostEnvironment, IConfiguration iconfiguration)
        {
            _logger = logger;
            _IDataLogic = iDataLogic;
            _IWIPStockRegister = iWIPStockRegister;
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
		[HttpGet]
		public async Task<IActionResult> ExportWIPStockRegisterToExcel(string ReportType)
		{
			string modelJson = HttpContext.Session.GetString("KeyWIPStockList");
			List<WIPStockRegisterDetail> stockRegisterList = new List<WIPStockRegisterDetail>();
			if (!string.IsNullOrEmpty(modelJson))
			{
				stockRegisterList = JsonConvert.DeserializeObject<List<WIPStockRegisterDetail>>(modelJson);
			}

			if (stockRegisterList == null)
				return NotFound("No data available to export.");

			using var workbook = new XLWorkbook();
			var worksheet = workbook.Worksheets.Add("WIPStock Register");

			var reportGenerators = new Dictionary<string, Action<IXLWorksheet, IList<WIPStockRegisterDetail>>>
			{
				{ "WIPSTOCKBATCHWISESUMMARY", ExportWipStockBatchWiseSummary },
				{ "WIPSTOCKSUMMARY", ExportWipStockSummary },
				{ "SHOWALLWIPSTOCKSUMMARY", ExportShowAllWipStockSummary },
				{ "SHOWONLYISSUEDATA", ExportShowOnlyIssue },
				{ "SHOWONLYRECDATA", ExportShowOnlyRec }
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
				"WIPStockRegisterReport.xlsx"
			);
		}
		private void ExportWipStockSummary(IXLWorksheet sheet, IList<WIPStockRegisterDetail> list)
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
				sheet.Cell(row, 2).Value = item.WorkCenterName;
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
		private void ExportWipStockBatchWiseSummary(IXLWorksheet sheet, IList<WIPStockRegisterDetail> list)
		{
			string[] headers = {
	        "Sr#", "Store Name", "Part Code", "Item Name", "Opn Stk", "Rec Qty", "Iss Qty", "Tot Stk",
	        "Batch Stock", "Unit", "Min Level", "Alt Unit", "Alt Stock", "Avg Rate", "Amount",
	        "Group Name", "Std Packing", "Maximum Level", "Reorder Level", "Bin No",
	        "Party Name", "Batch No", "Unique Batch No"
            };


			for (int i = 0; i < headers.Length; i++)
				sheet.Cell(1, i + 1).Value = headers[i];

			int row = 2, srNo = 1;
			foreach (var item in list)
			{
				sheet.Cell(row, 1).Value = srNo++;
				sheet.Cell(row, 2).Value = item.WorkCenterName;
				sheet.Cell(row, 3).Value = item.PartCode;
				sheet.Cell(row, 4).Value = item.ItemName;
				sheet.Cell(row, 5).Value = item.OpnStk;
				sheet.Cell(row, 6).Value = item.RecQty;
				sheet.Cell(row, 7).Value = item.IssQty;
				sheet.Cell(row, 8).Value = item.TotStk;
				sheet.Cell(row, 9).Value = item.BatchStock;
				sheet.Cell(row, 10).Value = item.Unit;
				sheet.Cell(row, 11).Value = item.MinLevel;
				sheet.Cell(row, 12).Value = item.AltUnit;
				sheet.Cell(row, 13).Value = item.AltStock;
				sheet.Cell(row, 14).Value = item.AvgRate;
				sheet.Cell(row, 15).Value = item.Amount;
				sheet.Cell(row, 16).Value = item.GroupName;
				sheet.Cell(row, 17).Value = item.StdPacking;
				sheet.Cell(row, 18).Value = item.MaximumLevel;
				sheet.Cell(row, 19).Value = item.ReorderLevel;
				sheet.Cell(row, 20).Value = item.BinNo;
				sheet.Cell(row, 21).Value = item.PartyName;
				sheet.Cell(row, 22).Value = item.BatchNo;
				sheet.Cell(row, 23).Value = item.UniquebatchNo;
				row++;
			}
		}
        private void ExportShowAllWipStockSummary(IXLWorksheet sheet, IList<WIPStockRegisterDetail> list)
		{
            string[] headers = {
             "Sr#","Item Name", "Part Code", "Total Stock", "Unit", "Rate", "Amount", "Item Type", "Item Group"
            };



            for (int i = 0; i < headers.Length; i++)
				sheet.Cell(1, i + 1).Value = headers[i];

			int row = 2, srNo = 1;
			foreach (var item in list)
			{
				sheet.Cell(row, 1).Value = srNo++;
                sheet.Cell(row, 1).Value = item.ItemName;
                sheet.Cell(row, 2).Value = item.PartCode;
                sheet.Cell(row, 3).Value = item.Total;
                sheet.Cell(row, 4).Value = item.Unit;
                sheet.Cell(row, 5).Value = item.Rate;
                sheet.Cell(row, 6).Value = item.Amount;
                sheet.Cell(row, 7).Value = item.ItemType;
                sheet.Cell(row, 8).Value = item.ItemGroup;

                row++;
			}
		}
        private void ExportShowOnlyIssue(IXLWorksheet sheet, IList<WIPStockRegisterDetail> list)
		{
            string[] headers = {
               "Sr#", "Store Name", "Transaction Type", "Trans Date", "Part Code", "Item Name", "Rec Qty", "Unit",
                "Rate", "Amount", "Bill No", "Bill Date", "Party Name", "MRN No", "Batch No", "Unique Batch No",
                "Entry Id", "Alt Rec Qty", "Group Name"
            };




            for (int i = 0; i < headers.Length; i++)
				sheet.Cell(1, i + 1).Value = headers[i];

			int row = 2, srNo = 1;
			foreach (var item in list)
			{
				sheet.Cell(row, 1).Value = srNo++;
                sheet.Cell(row, 1).Value = item.WorkCenterName;
                sheet.Cell(row, 2).Value = item.TransactionType;
                sheet.Cell(row, 3).Value = item.TransDate?.Split(" ")[0];  // Null-safe date handling
                sheet.Cell(row, 4).Value = item.PartCode;
                sheet.Cell(row, 5).Value = item.ItemName;
                sheet.Cell(row, 6).Value = item.RecQty;
                sheet.Cell(row, 7).Value = item.Unit;
                sheet.Cell(row, 8).Value = item.Rate.ToString("0.00");
                sheet.Cell(row, 9).Value = item.Amount;
                sheet.Cell(row, 10).Value = item.BillNo;
                sheet.Cell(row, 11).Value = item.BillDate?.Split(" ")[0];  // Null-safe date handling
                sheet.Cell(row, 12).Value = item.PartyName;
                sheet.Cell(row, 13).Value = item.MRNNo;
                sheet.Cell(row, 14).Value = item.BatchNo;
                sheet.Cell(row, 15).Value = item.UniquebatchNo;
                sheet.Cell(row, 16).Value = item.EntryId;
                sheet.Cell(row, 17).Value = item.AltRecQty;
                sheet.Cell(row, 18).Value = item.GroupName;


                row++;
			}
		}
        private void ExportShowOnlyRec(IXLWorksheet sheet, IList<WIPStockRegisterDetail> list)
		{
            string[] headers = {
                "Store Name", "Transaction Type", "Trans Date", "Part Code", "Item Name", "Rec Qty", "Unit",
                "Rate", "Amount", "Bill No", "Bill Date", "Party Name", "MRN No", "Batch No", "Unique Batch No",
                "Entry Id", "Alt Rec Qty", "Group Name"
            };





            for (int i = 0; i < headers.Length; i++)
				sheet.Cell(1, i + 1).Value = headers[i];

			int row = 2, srNo = 1;
			foreach (var item in list)
			{
				sheet.Cell(row, 1).Value = srNo++;
                sheet.Cell(row, 1).Value = item.WorkCenterName;
                sheet.Cell(row, 2).Value = item.TransactionType;
                sheet.Cell(row, 3).Value = item.TransDate?.Split(" ")[0]; // null check for safe split
                sheet.Cell(row, 4).Value = item.PartCode;
                sheet.Cell(row, 5).Value = item.ItemName;
                sheet.Cell(row, 6).Value = item.RecQty;
                sheet.Cell(row, 7).Value = item.Unit;
                sheet.Cell(row, 8).Value = item.Rate.ToString("0.00");
                sheet.Cell(row, 9).Value = item.Amount;
                sheet.Cell(row, 10).Value = item.BillNo;
                sheet.Cell(row, 11).Value = item.BillDate?.Split(" ")[0]; // null check for safe split
                sheet.Cell(row, 12).Value = item.PartyName;
                sheet.Cell(row, 13).Value = item.MRNNo;
                sheet.Cell(row, 14).Value = item.BatchNo;
                sheet.Cell(row, 15).Value = item.UniquebatchNo;
                sheet.Cell(row, 16).Value = item.EntryId;
                sheet.Cell(row, 17).Value = item.AltRecQty;
                sheet.Cell(row, 18).Value = item.GroupName;



                row++;
			}
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

            string serializedGrid = JsonConvert.SerializeObject(fullList);
            HttpContext.Session.SetString("KeyWIPStockList", serializedGrid);
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

            string modelJson = HttpContext.Session.GetString("KeyWIPStockList");
            List<WIPStockRegisterDetail> wipRegisterViewModel = new List<WIPStockRegisterDetail>();
            if (!string.IsNullOrEmpty(modelJson))
            {
                wipRegisterViewModel = JsonConvert.DeserializeObject<List<WIPStockRegisterDetail>>(modelJson);
            }
            if ( wipRegisterViewModel == null)
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
        [HttpGet]
        public IActionResult GetWIPStockRegisterData()
        {
            string modelJson = HttpContext.Session.GetString("KeyWIPStockList");
            List<WIPStockRegisterDetail> stockRegisterList = new List<WIPStockRegisterDetail>();
            if (!string.IsNullOrEmpty(modelJson))
            {
                stockRegisterList = JsonConvert.DeserializeObject<List<WIPStockRegisterDetail>>(modelJson);
            }

            return Json(stockRegisterList);
        }
    }
}
