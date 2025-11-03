using ClosedXML.Excel;
using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace eTactWeb.Controllers
{
    public class PurchaseMISController : Controller
    {
        private readonly IDataLogic _IDataLogic;
        public IPurchaseMIS _IPurchaseMIS { get; }
        private readonly ILogger<PurchaseMISController> _logger;
        private readonly IConfiguration iconfiguration;
        private readonly IMemoryCache _MemoryCache;
        public IWebHostEnvironment _IWebHostEnvironment { get; }
        public PurchaseMISController(ILogger<PurchaseMISController> logger, IDataLogic iDataLogic, IPurchaseMIS iPurchaseMIS, EncryptDecrypt encryptDecrypt, IWebHostEnvironment iWebHostEnvironment, IConfiguration iconfiguration, IMemoryCache iMemoryCache)
        {
            _logger = logger;
            _IDataLogic = iDataLogic;
            _IPurchaseMIS = iPurchaseMIS;
            _IWebHostEnvironment = iWebHostEnvironment;
            this.iconfiguration = iconfiguration;
            _MemoryCache = iMemoryCache;
        }
        [Route("{controller}/Index")]
        public async Task<ActionResult> PurchaseMIS()
        {
            var model = new PurchaseMISModel();
            model.PurchaseMISGrid = new List<PurchaseMISModel>();
            model.YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
            return View(model);
        }
        public async Task<JsonResult> FillItemName()
        {
            var JSON = await _IPurchaseMIS.FillItemName();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillPartCode()
        {
            var JSON = await _IPurchaseMIS.FillPartCode();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillAccountName()
        {
            var JSON = await _IPurchaseMIS.FillAccountName();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<IActionResult>  GetPurchaseMISDetailsData(string ReportType, string ToDate, int YearCode, int Itemcode, int AccountCode, int pageNumber = 1, int pageSize = 50, string SearchBox = "")
        {
            var model = new PurchaseMISModel();
            model = await _IPurchaseMIS.GetPurchaseMISDetailsData( ReportType,  ToDate,  YearCode,  Itemcode,  AccountCode);

            var modelList = model?.PurchaseMISGrid ?? new List<PurchaseMISModel>();


            if (string.IsNullOrWhiteSpace(SearchBox))
            {
                model.TotalRecords = modelList.Count();
                model.PageNumber = pageNumber;
                model.PageSize = pageSize;
                model.PurchaseMISGrid = modelList
                .Skip((pageNumber - 1) * pageSize)
                   .Take(pageSize)
                   .ToList();
            }
            else
            {
                List<PurchaseMISModel> filteredResults;
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
                model.PurchaseMISGrid = filteredResults.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
                model.PageNumber = pageNumber;
                model.PageSize = pageSize;
            }
            MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                SlidingExpiration = TimeSpan.FromMinutes(55),
                Size = 1024,
            };

            _MemoryCache.Set("KeyPurchaseMISList", modelList, cacheEntryOptions);

            return PartialView("_PurchaseMISMonthlyAnalysisGrid", model);
           
        }
        [HttpGet]
        public IActionResult GlobalSearch(string searchString, string dashboardType , int pageNumber = 1, int pageSize = 50)
        {
            PurchaseMISModel model = new PurchaseMISModel();
            model.ReportType = dashboardType;
            if (string.IsNullOrWhiteSpace(searchString))
            {
                return PartialView("_PurchaseMISMonthlyAnalysisGrid", new List<PurchaseMISModel>());
            }
            //string cacheKey = $"KeyProdList_{dashboardType}";
            if (!_MemoryCache.TryGetValue("KeyPurchaseMISList", out IList<PurchaseMISModel> purchaseMISModel) || purchaseMISModel == null)
            {
                return PartialView("_PurchaseMISMonthlyAnalysisGrid", new List<PurchaseMISModel>());
            }

            List<PurchaseMISModel> filteredResults;

            if (string.IsNullOrWhiteSpace(searchString))
            {
                filteredResults = purchaseMISModel.ToList();
            }
            else
            {
                filteredResults = purchaseMISModel
                    .Where(i => i.GetType().GetProperties()
                        .Where(p => p.PropertyType == typeof(string))
                        .Select(p => p.GetValue(i)?.ToString())
                        .Any(value => !string.IsNullOrEmpty(value) &&
                                      value.Contains(searchString, StringComparison.OrdinalIgnoreCase)))
                    .ToList();


                if (filteredResults.Count == 0)
                {
                    filteredResults = purchaseMISModel.ToList();
                }
            }

            model.TotalRecords = filteredResults.Count;
            model.PurchaseMISGrid = filteredResults.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            model.PageNumber = pageNumber;
            model.PageSize = pageSize;
          
            return PartialView("_PurchaseMISMonthlyAnalysisGrid", model);
            

        }
        [HttpGet]
        public IActionResult GetPurchaseMISReportForPDF()
        {
            if (_MemoryCache.TryGetValue("KeyPurchaseMISList", out List<PurchaseMISModel> purchaseMISList))
            {
                return Json(purchaseMISList);
            }
            return Json(new List<PurchaseMISModel>());
        }
        public async Task<IActionResult> ExportPurchaseMISToExcel(string ReportType)
        {
            //string modelJson = HttpContext.Session.GetString("KeyPOList");
            if (!_MemoryCache.TryGetValue("KeyPurchaseMISList", out List<PurchaseMISModel> modelList))
            {
                return NotFound("No data available to export.");
            }


            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("PUrchaseMIS Register");

            var reportGenerators = new Dictionary<string, Action<IXLWorksheet, IList<PurchaseMISModel>>>
            {
                { "Monthly Rate Analysis", EXPORT_MonthlyRateAnalysisGrid }
               

            };

            if (reportGenerators.TryGetValue(ReportType, out var generator))
            {
                generator(worksheet, modelList);
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
                "PurchaseMISReport.xlsx"
            );
        }
        private void EXPORT_MonthlyRateAnalysisGrid(IXLWorksheet sheet, IList<PurchaseMISModel> list)
        {
            string[] headers = {
                 "#Sr", "Item Name", "Part Code", "Account Name", "APR", "MAY", "JUN", "JUL",
                        "AUG", "SEP", "OCT", "NOV", "DEC", "JAN", "FEB", "MAR"
            };



            for (int i = 0; i < headers.Length; i++)
                sheet.Cell(1, i + 1).Value = headers[i];

            int row = 2, srNo = 1;
            foreach (var item in list)
            {
                sheet.Cell(row, 1).Value = srNo;                     // #Sr
                sheet.Cell(row, 2).Value = item.ItemName;            // Item Name
                sheet.Cell(row, 3).Value = item.PartCode;            // Part Code
                sheet.Cell(row, 4).Value = item.AccountName;         // Account Name
                sheet.Cell(row, 5).Value = item.APR;                 // APR
                sheet.Cell(row, 6).Value = item.MAY;                 // MAY
                sheet.Cell(row, 7).Value = item.JUN;                 // JUN
                sheet.Cell(row, 8).Value = item.JUL;                 // JUL
                sheet.Cell(row, 9).Value = item.AUG;                 // AUG
                sheet.Cell(row, 10).Value = item.SEP;                // SEP
                sheet.Cell(row, 11).Value = item.OCT;                // OCT
                sheet.Cell(row, 12).Value = item.NOV;                // NOV
                sheet.Cell(row, 13).Value = item.DEC;                // DEC
                sheet.Cell(row, 14).Value = item.JAN;                // JAN
                sheet.Cell(row, 15).Value = item.FEB;                // FEB
                sheet.Cell(row, 16).Value = item.MAR;                // MAR


                row++;
            }
        }
    }
}
