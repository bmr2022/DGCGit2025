using eTactWeb.Data.Common;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using eTactWeb.DOM.Models;
using System.Globalization;
using ClosedXML.Excel;
using FastReport.Web;
using FastReport;

namespace eTactWeb.Controllers
{
    public class StockRegisterController : Controller
    {
        private readonly IDataLogic _IDataLogic;
        public IStockRegister _IStockRegister { get; }
        private readonly ILogger<StockRegisterController> _logger;
        private readonly IConfiguration iconfiguration;
        public IWebHostEnvironment _IWebHostEnvironment { get; }
        private readonly ConnectionStringService _connectionStringService;
        public StockRegisterController(ILogger<StockRegisterController> logger, IDataLogic iDataLogic, IStockRegister iStockRegister, EncryptDecrypt encryptDecrypt, IWebHostEnvironment iWebHostEnvironment, IConfiguration iconfiguration, ConnectionStringService connectionStringService)
        {
            _logger = logger;
            _IDataLogic = iDataLogic;
            _IStockRegister = iStockRegister;
            _IWebHostEnvironment = iWebHostEnvironment;
            this.iconfiguration = iconfiguration;
            _connectionStringService = connectionStringService;
        }
        [HttpGet]
        public IActionResult PrintReport(string FromDate = "",string ToDate = "",int StoreId = 0,string PartCode = "",string ItemName = "",
        string ItemGroupName = "",string ItemType = "",string ReportType = "",string BatchNo = "",string UniqueBatchNo ="")
        {
            string my_connection_string;
            string contentRootPath = _IWebHostEnvironment.ContentRootPath;
            string webRootPath = _IWebHostEnvironment.WebRootPath;

            var webReport = new WebReport();
            webReport.Report.Clear();
            webReport.Report.Dispose();
            webReport.Report = new Report();

            webReport.Report.Load(webRootPath + "\\StockRegister.frx");

            webReport.Report.SetParameterValue("FromDateparam", CommonFunc.ParseFormattedDate(FromDate));
            webReport.Report.SetParameterValue("ToDateparam", CommonFunc.ParseFormattedDate(ToDate));
            webReport.Report.SetParameterValue("StoreIdparam", StoreId);
            webReport.Report.SetParameterValue("PartCodeparam", PartCode);
            webReport.Report.SetParameterValue("ItemNameparam", ItemName);
            webReport.Report.SetParameterValue("ItemGroupNameparam", ItemGroupName);
            webReport.Report.SetParameterValue("ItemTypeparam", ItemType);
            webReport.Report.SetParameterValue("BatchNoparam", BatchNo);
            webReport.Report.SetParameterValue("UniqueBatchNoparam", UniqueBatchNo);

            my_connection_string = _connectionStringService.GetConnectionString();
            webReport.Report.Dictionary.Connections[0].ConnectionString = my_connection_string;
            webReport.Report.Dictionary.Connections[0].ConnectionStringExpression = "";
            webReport.Report.SetParameterValue("MyParameter", my_connection_string);

            webReport.Report.Refresh();

            return View(webReport);
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
            if (ReportType == "NegativeStock")
            {
                string serializedGridForNegativeStock = JsonConvert.SerializeObject(fullList);
                HttpContext.Session.SetString("KeyStockListNeg", serializedGridForNegativeStock);
            }
            else
            {
                string serializedGrid = JsonConvert.SerializeObject(fullList);
                HttpContext.Session.SetString("KeyStockList", serializedGrid);
            }

            return PartialView("_StockRegisterGrid", model);
        }
        [HttpGet]
        public IActionResult GlobalSearch(string searchString,string ReportType, int pageNumber = 1, int pageSize = 500)
        {
            StockRegisterModel model = new StockRegisterModel();
            if (string.IsNullOrWhiteSpace(searchString))
            {
                return PartialView("_StockRegisterGrid", new List<StockRegisterModel>());
            }
            string modelJson = "";
            if (ReportType == "NegativeStock")
            {
                 modelJson = HttpContext.Session.GetString("KeyStockListNeg");
            }
            else
            {
                 modelJson = HttpContext.Session.GetString("KeyStockList");
            }
                
            List<StockRegisterDetail> stockRegisterViewModel = new List<StockRegisterDetail>();
            if (!string.IsNullOrEmpty(modelJson))
            {
                stockRegisterViewModel = JsonConvert.DeserializeObject<List<StockRegisterDetail>>(modelJson);
            }
            if ( stockRegisterViewModel == null)
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
            if (ReportType == "NegativeStock")
            {
                return PartialView("_ShowNegativeStockReport", model);
            }
            else
            {
                return PartialView("_StockRegisterGrid", model);
            }
            return null;
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
            string modelJson;
            if(ReportType== "NegativeStock")
            {
                 modelJson = HttpContext.Session.GetString("KeyStockListNeg");
            }
            else
            {
                 modelJson = HttpContext.Session.GetString("KeyStockList");
            }
            List<StockRegisterDetail> stockRegisterList = new List<StockRegisterDetail>();
            if (!string.IsNullOrEmpty(modelJson))
            {
                stockRegisterList = JsonConvert.DeserializeObject<List<StockRegisterDetail>>(modelJson);
            }

            if (stockRegisterList == null)
                return NotFound("No data available to export.");

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Stock Register");

            var reportGenerators = new Dictionary<string, Action<IXLWorksheet, IList<StockRegisterDetail>>>
            {
                { "STOCKSUMMARY", ExportStockSummary },
                { "STOCKDETAIL", ExportStockDetail },
                { "SHOWALLSTORESTOCK", ExportSHOWALLSTORESTOCK },
                { "STOCKWITHZEROINVENTORY", ExportStockSummary },

                { "SHOWONLYRECDATA", ExportSHOWONLYRECDATA },                
                { "SHOWONLYISSUEDATA", ExportSHOWONLYISSUEDATA },               
                { "BATCHWISESTOCKSUMMARY", ExportBATCHWISESTOCKSUMMARY },            
                
                { "SHOWALLSTORE+WIPSTOCK", ExportSHOWALLSTORE_WIPSTOCK },               
                { "SHOWALLSTORE+WIPSTOCK+JOBWORK", ExportSHOWALLSTORE_WIPSTOCK_JOBWORK },             
                { "NegativeStock", ExportNegativeStock },
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

            // Dynamic file name with only date
            string fileName = $"{ReportType}_Report_{DateTime.Now:dd-MMM-yyyy}.xlsx";

            return File(
                stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileName
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

        private void ExportSHOWALLSTORESTOCK(IXLWorksheet sheet, IList<StockRegisterDetail> list)
        {
            string[] headers = { "Sr#", "Item Name", "Part Code", "Total", "Unit", "Rate", "Amount", "Item Type", "Item Group", "Main Store", 
                "Rej Store" };


            // Header row
            for (int i = 0; i < headers.Length; i++)
                sheet.Cell(1, i + 1).Value = headers[i];

            int row = 2;
            int srNo = 1;

            foreach (var item in list)
            {
                sheet.Cell(row, 1).Value = srNo++;               // Sr#
                sheet.Cell(row, 2).Value = item.ItemName;         // Item Name
                sheet.Cell(row, 3).Value = item.PartCode;         // Part Code
                sheet.Cell(row, 4).Value = item.Total;            // Total
                sheet.Cell(row, 5).Value = item.Unit;             // Unit
                sheet.Cell(row, 6).Value = item.Rate;             // Rate
                sheet.Cell(row, 7).Value = item.Amount;           // Amount
                sheet.Cell(row, 8).Value = item.ItemType;         // Item Type
                sheet.Cell(row, 9).Value = item.ItemGroup;        // Item Group
                sheet.Cell(row, 10).Value = item.MAINSTORE;       // Main Store
                sheet.Cell(row, 11).Value = item.REJSTORE;        // Rej Store

                // Optional: Highlight negative stock just like UI
                if (item.Total < 0)
                {
                    sheet.Cell(row, 4).Style.Font.FontColor = XLColor.Red;
                }

                row++;
            }

            // Optional: Auto adjust column width
            sheet.Columns().AdjustToContents();
        }

        private void ExportSHOWONLYRECDATA(IXLWorksheet sheet, IList<StockRegisterDetail> list)
        {
            string[] headers = { "Sr#", "Store Name", "Transaction Type", "Trans Date", "Part Code", "Item Name", "Rec Qty", "Unit", "Rate", "Amount", "Bill No", "Bill Date", "Party Name", "MRN No", "Batch No", "Unique Batch No", "Entry Id", "Alt Rec Qty", "Group Name" };

            for (int i = 0; i < headers.Length; i++)
                sheet.Cell(1, i + 1).Value = headers[i];

            int row = 2, srNo = 1;

            foreach (var item in list)
            {
                sheet.Cell(row, 1).Value = srNo++;
                sheet.Cell(row, 2).Value = item.StoreName;
                sheet.Cell(row, 3).Value = item.TransactionType;
                sheet.Cell(row, 4).Value = item.TransDate != null ? item.TransDate.Split(" ")[0] : item.TransDate;
                sheet.Cell(row, 5).Value = item.PartCode;
                sheet.Cell(row, 6).Value = item.ItemName;
                sheet.Cell(row, 7).Value = item.RecQty;
                sheet.Cell(row, 8).Value = item.Unit;
                sheet.Cell(row, 9).Value = item.Rate;
                sheet.Cell(row, 10).Value = item.Amount;
                sheet.Cell(row, 11).Value = item.BillNo;
                sheet.Cell(row, 12).Value = item.BillDate != null ? item.BillDate.Split(" ")[0] : item.BillDate;
                sheet.Cell(row, 13).Value = item.PartyName;
                sheet.Cell(row, 14).Value = item.MRNNo;
                sheet.Cell(row, 15).Value = item.BatchNo;
                sheet.Cell(row, 16).Value = item.UniquebatchNo;
                sheet.Cell(row, 17).Value = item.EntryId;
                sheet.Cell(row, 18).Value = item.AltRecQty;
                sheet.Cell(row, 19).Value = item.GroupName;

                row++;
            }

            sheet.Columns().AdjustToContents();
        }

        private void ExportSHOWONLYISSUEDATA(IXLWorksheet sheet, IList<StockRegisterDetail> list)
        {
            string[] headers = {
        "Sr#", "Store Name", "Transaction Type", "Trans Date",
        "Part Code", "Item Name", "Iss Qty", "Unit", "Rate", "Amount",
        "Min Level", "Alt Unit", "Bill No", "Bill Date", "Party Name",
        "MRN No", "Batch No", "Unique Batch No", "Entry Id",
        "Alt Rec Qty", "Alt Iss Qty", "Group Name"
    };

            for (int i = 0; i < headers.Length; i++)
                sheet.Cell(1, i + 1).Value = headers[i];

            int row = 2, srNo = 1;

            foreach (var item in list)
            {
                sheet.Cell(row, 1).Value = srNo++;
                sheet.Cell(row, 2).Value = item.StoreName;
                sheet.Cell(row, 3).Value = item.TransactionType;
                sheet.Cell(row, 4).Value = item.TransDate != null ? item.TransDate.Split(" ")[0] : item.TransDate;
                sheet.Cell(row, 5).Value = item.PartCode;
                sheet.Cell(row, 6).Value = item.ItemName;
                sheet.Cell(row, 7).Value = item.IssQty;
                sheet.Cell(row, 8).Value = item.Unit;
                sheet.Cell(row, 9).Value = item.Rate;
                sheet.Cell(row, 10).Value = item.Amount;
                sheet.Cell(row, 11).Value = item.MinLevel;
                sheet.Cell(row, 12).Value = item.AltUnit;
                sheet.Cell(row, 13).Value = item.BillNo;
                sheet.Cell(row, 14).Value = item.BillDate != null ? item.BillDate.Split(" ")[0] : item.BillDate;
                sheet.Cell(row, 15).Value = item.PartyName;
                sheet.Cell(row, 16).Value = item.MRNNo;
                sheet.Cell(row, 17).Value = item.BatchNo;
                sheet.Cell(row, 18).Value = item.UniquebatchNo;
                sheet.Cell(row, 19).Value = item.EntryId;
                sheet.Cell(row, 20).Value = item.AltRecQty;
                sheet.Cell(row, 21).Value = item.AltIssQty;
                sheet.Cell(row, 22).Value = item.GroupName;

                row++;
            }

            sheet.Columns().AdjustToContents();
        }
        private void ExportBATCHWISESTOCKSUMMARY(IXLWorksheet sheet, IList<StockRegisterDetail> list)
        {
            string[] headers = { "Sr#", "Store Name", "Part Code", "Item Name", "Opn Stk", "Rec Qty", "Iss Qty", "Tot Stock", "Batch Stock", "Unit", "Min Level", "Alt Unit", "Alt Stock", "Avg Rate", "Amount", "Group Name", "Std Packing", "Maximum Level", "Reorder Level", "Bin No", "Party Name", "Batch No", "Unique Batch No" };

            // HEADER
            for (int i = 0; i < headers.Length; i++)
                sheet.Cell(1, i + 1).Value = headers[i];

            int row = 2, srNo = 1;

            // DATA
            foreach (var item in list)
            {
                sheet.Cell(row, 1).Value = srNo++;                 // Sr#
                sheet.Cell(row, 2).Value = item.StoreName;
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

            sheet.Columns().AdjustToContents();
        }
        private void ExportSHOWALLSTORE_WIPSTOCK(IXLWorksheet sheet, IList<StockRegisterDetail> list)
        {
            string[] headers = { "Sr#", "Item Name", "Part Code", "Total", "Unit", "Rate", "Amount", "Item Type", "Item Group", "Main Store", "Rej Store", "QC Store", "Assembly" };

            // HEADER
            for (int i = 0; i < headers.Length; i++)
                sheet.Cell(1, i + 1).Value = headers[i];

            int row = 2, sr = 1;

            // DATA
            foreach (var item in list)
            {
                sheet.Cell(row, 1).Value = sr++;                 // Sr#
                sheet.Cell(row, 2).Value = item.ItemName;
                sheet.Cell(row, 3).Value = item.PartCode;
                sheet.Cell(row, 4).Value = item.Total;
                sheet.Cell(row, 5).Value = item.Unit;
                sheet.Cell(row, 6).Value = item.Rate;
                sheet.Cell(row, 7).Value = item.Amount;
                sheet.Cell(row, 8).Value = item.ItemType;
                sheet.Cell(row, 9).Value = item.ItemGroup;
                sheet.Cell(row, 10).Value = item.MAINSTORE;
                sheet.Cell(row, 11).Value = item.REJSTORE;
                sheet.Cell(row, 12).Value = item.QCSTORE;
                sheet.Cell(row, 13).Value = item.Assembly;

                // Optional: Negative stock in RED
                if (item.Total < 0)
                    sheet.Cell(row, 4).Style.Font.FontColor = XLColor.Red;

                row++;
            }

            sheet.Columns().AdjustToContents();
            sheet.SheetView.FreezeRows(1); // freeze header
        }

        private void ExportSHOWALLSTORE_WIPSTOCK_JOBWORK(IXLWorksheet sheet, IList<StockRegisterDetail> list)
        {
            string[] headers = { "Sr#", "Item Name", "Part Code", "Total", "Unit", "Rate", "Amount", "Item Type", "Item Group", "Main Store", "Assembly" };

            // Header
            for (int i = 0; i < headers.Length; i++)
                sheet.Cell(1, i + 1).Value = headers[i];

            int row = 2, sr = 1;

            // Data
            foreach (var item in list)
            {
                sheet.Cell(row, 1).Value = sr++;
                sheet.Cell(row, 2).Value = item.ItemName;
                sheet.Cell(row, 3).Value = item.PartCode;
                sheet.Cell(row, 4).Value = item.Total;
                sheet.Cell(row, 5).Value = item.Unit;
                sheet.Cell(row, 6).Value = item.Rate;
                sheet.Cell(row, 7).Value = item.Amount;
                sheet.Cell(row, 8).Value = item.ItemType;
                sheet.Cell(row, 9).Value = item.ItemGroup;
                sheet.Cell(row, 10).Value = item.MAINSTORE;
                sheet.Cell(row, 11).Value = item.Assembly;

                // Negative stock red
                if (item.Total < 0)
                    sheet.Cell(row, 4).Style.Font.FontColor = XLColor.Red;

                row++;
            }

            sheet.Columns().AdjustToContents();
            sheet.SheetView.FreezeRows(1);
        }
        private void ExportNegativeStock(IXLWorksheet sheet, IList<StockRegisterDetail> list)
        {
            string[] headers = { "Sr#", "Item Name", "Part Code", "Stock", "Store Name", "Unique Batch No", "Batch No", "Item Code", "Store ID" };

            // HEADER
            for (int i = 0; i < headers.Length; i++)
                sheet.Cell(1, i + 1).Value = headers[i];

            int row = 2, sr = 1;

            // DATA
            foreach (var item in list)
            {
                sheet.Cell(row, 1).Value = sr++;
                sheet.Cell(row, 2).Value = item.ItemName;
                sheet.Cell(row, 3).Value = item.PartCode;
                sheet.Cell(row, 4).Value = item.TotStk;          // Stock
                sheet.Cell(row, 5).Value = item.StoreName;
                sheet.Cell(row, 6).Value = item.UniquebatchNo;
                sheet.Cell(row, 7).Value = item.BatchNo;
                sheet.Cell(row, 8).Value = item.ItemCode;
                sheet.Cell(row, 9).Value = item.StoreId;

                // Highlight negative stock
                if (item.TotStk < 0)
                    sheet.Cell(row, 4).Style.Font.FontColor = XLColor.Red;

                row++;
            }

            sheet.Columns().AdjustToContents();
            sheet.SheetView.FreezeRows(1);   // Freeze header
        }

        public IActionResult GetStockDataForPDF(string ReportType)
        {
            //string modelJson = HttpContext.Session.GetString("KeyStockList");
            string modelJson;
            if (ReportType == "NegativeStock")
            {
                modelJson = HttpContext.Session.GetString("KeyStockListNeg");
            }
            else
            {
                modelJson = HttpContext.Session.GetString("KeyStockList");
            }
            List<StockRegisterDetail> stockRegisterList = new List<StockRegisterDetail>();
            if (!string.IsNullOrEmpty(modelJson))
            {
                stockRegisterList = JsonConvert.DeserializeObject<List<StockRegisterDetail>>(modelJson);
            }

            return Json(stockRegisterList);
        }
    }
}

