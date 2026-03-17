using ClosedXML.Excel;
using DocumentFormat.OpenXml.Drawing.Diagrams;
using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using FastReport;
using FastReport.Web;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Org.BouncyCastle.Ocsp;
using PdfSharp.Drawing.BarCodes;
using System.Data;
using System.Globalization;

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
        public IActionResult ExportStockregisterExcel(string ReportType)
        {
            string json = HttpContext.Session.GetString("StockRegisterList");

            if (string.IsNullOrEmpty(json))
                return Content("No data available for export.");

            // Deserialize as dynamic list
            var data = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(json);

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("StockRegister");

            if (data != null && data.Count > 0)
            {
                // 🔥 Add Headers
                int col = 1;
                foreach (var key in data[0].Keys)
                {
                    worksheet.Cell(1, col).Value = key;
                    worksheet.Cell(1, col).Style.Font.Bold = true;
                    col++;
                }

                // 🔥 Add Rows
                int row = 2;
                foreach (var item in data)
                {
                    col = 1;
                    foreach (var value in item.Values)
                    {
                        worksheet.Cell(row, col).Value = value?.ToString();
                        col++;
                    }
                    row++;
                }
            }

            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Position = 0;

            return File(
                stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "StockRegister.xlsx"
            );
        }

        [HttpGet]
        public async Task<IActionResult> GetStockRegisterData(string FromDate, string ToDate, string PartCode, string ItemName, string ItemGroup, string ItemType, int StoreId, string ReportType, string BatchNo, string UniqueBatchNo, int pageNumber = 1, int pageSize = 500, string SearchBox = "")
        {
            
            //var fullList = (await _IStockRegister.GetStockRegisterData(FromDate, ToDate, PartCode, ItemName, ItemGroup, ItemType, StoreId, ReportType, BatchNo, UniqueBatchNo))?.StockRegisterDetail ?? new List<StockRegisterDetail>();
            var YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
            var model = new StockRegisterModel();
            var result = (await _IStockRegister.GetStockRegisterData(FromDate, ToDate, PartCode, ItemName, ItemGroup, ItemType, StoreId, ReportType, BatchNo, UniqueBatchNo));
            //model.ReportMode = Flag;
            //model.ReqType = ReQType;
           
            if (result == null || !(result.Result is DataTable dt))
            {
                model.TotalRecords = 0;
                model.Rows = new List<Dictionary<string, object>>();
                return PartialView("_StockRegisterGrid", model);
            }

            model.TotalRecords = dt.Rows.Count;
            model.PageNumber = pageNumber;
            model.PageSize = pageSize;

            model.Headers = dt.Columns
                .Cast<DataColumn>()
                .Select(c => new DashboardColumn
                {
                    Title = c.ColumnName,
                    Field = c.ColumnName
                })
                .ToList();

            //model.Rows = dt.AsEnumerable()
            //    .Select(r => dt.Columns
            //        .Cast<DataColumn>()
            //        .ToDictionary(
            //            c => c.ColumnName,
            //            c => r[c] == DBNull.Value ? null : r[c]
            //        ))
            //    .ToList();

            model.ReportType = ReportType;
            model.Rows = dt.AsEnumerable()
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(r => dt.Columns
                .Cast<DataColumn>()
                .ToDictionary(
                    c => c.ColumnName,
                    c => r[c] == DBNull.Value ? null : r[c]
                ))
            .ToList();
            HttpContext.Session.SetString("StockRegisterList", JsonConvert.SerializeObject(dt));
            return PartialView("_StockRegisterGrid", model);



        }

        [HttpGet]
        public IActionResult GlobalSearch(string searchString, int pageNumber = 1, int pageSize = 50)
        {
            StockRegisterModel model = new StockRegisterModel();

            // 1️⃣ Get session data
            string modelJson = HttpContext.Session.GetString("StockRegisterList");

            if (string.IsNullOrWhiteSpace(modelJson))
            {
                model.Rows = new List<Dictionary<string, object>>();
                model.Headers = new List<DashboardColumn>();
                model.TotalRecords = 0;
                return PartialView("_StockRegisterGrid", model);
            }

            // 2️⃣ Deserialize rows
            var allRows = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(modelJson);

            if (allRows == null || allRows.Count == 0)
            {
                model.Rows = new List<Dictionary<string, object>>();
                model.Headers = new List<DashboardColumn>();
                model.TotalRecords = 0;
                return PartialView("_StockRegisterGrid", model);
            }

            // 3️⃣ Dynamic search (all columns)
            var filteredRows = string.IsNullOrWhiteSpace(searchString)
                ? allRows
                : allRows.Where(row =>
                    row.Values.Any(val =>
                        val != null &&
                        val.ToString()
                           .Contains(searchString, StringComparison.OrdinalIgnoreCase)))
                  .ToList();

            // fallback → show all
            if (filteredRows.Count == 0)
                filteredRows = allRows;

            // 4️⃣ Dynamic headers (from keys)
            model.Headers = filteredRows.First()
                .Keys
                .Select(k => new DashboardColumn
                {
                    Title = k,
                    Field = k
                })
                .ToList();

            // 5️⃣ Pagination
            model.TotalRecords = filteredRows.Count;
            model.PageNumber = pageNumber;
            model.PageSize = pageSize;

            model.Rows = filteredRows
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return PartialView("_StockRegisterGrid", model);
        }
        //[HttpGet]
        //public IActionResult GlobalSearch(string searchString,string ReportType, int pageNumber = 1, int pageSize = 500)
        //{
        //    StockRegisterModel model = new StockRegisterModel();
        //    if (string.IsNullOrWhiteSpace(searchString))
        //    {
        //        return PartialView("_StockRegisterGrid", new List<StockRegisterModel>());
        //    }
        //    string modelJson = "";
        //    if (ReportType == "NegativeStock")
        //    {
        //         modelJson = HttpContext.Session.GetString("KeyStockListNeg");
        //    }
        //    else
        //    {
        //         modelJson = HttpContext.Session.GetString("KeyStockList");
        //    }
                
        //    List<StockRegisterDetail> stockRegisterViewModel = new List<StockRegisterDetail>();
        //    if (!string.IsNullOrEmpty(modelJson))
        //    {
        //        stockRegisterViewModel = JsonConvert.DeserializeObject<List<StockRegisterDetail>>(modelJson);
        //    }
        //    if ( stockRegisterViewModel == null)
        //    {
        //        return PartialView("_StockRegisterGrid", new List<StockRegisterModel>());
        //    }

        //    List<StockRegisterDetail> filteredResults;

        //    if (string.IsNullOrWhiteSpace(searchString))
        //    {
        //        filteredResults = stockRegisterViewModel.ToList();
        //    }
        //    else
        //    {
        //        filteredResults = stockRegisterViewModel
        //            .Where(i => i.GetType().GetProperties()
        //                .Where(p => p.PropertyType == typeof(string))
        //                .Select(p => p.GetValue(i)?.ToString())
        //                .Any(value => !string.IsNullOrEmpty(value) &&
        //                              value.Contains(searchString, StringComparison.OrdinalIgnoreCase)))
        //            .ToList();


        //        if (filteredResults.Count == 0)
        //        {
        //            filteredResults = stockRegisterViewModel.ToList();
        //        }
        //    }

        //    model.TotalRecords = filteredResults.Count;
        //    model.StockRegisterDetail = filteredResults.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
        //    model.PageNumber = pageNumber;
        //    model.PageSize = pageSize;
        //    if (ReportType == "NegativeStock")
        //    {
        //        return PartialView("_ShowNegativeStockReport", model);
        //    }
        //    else
        //    {
        //        return PartialView("_StockRegisterGrid", model);
        //    }
        //    return null;
        //}
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

