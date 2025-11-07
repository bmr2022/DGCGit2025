using ClosedXML.Excel;
using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using static eTactWeb.DOM.Models.Common;

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
            model = await BindModel(model).ConfigureAwait(false);
            model = await BindModel1(model).ConfigureAwait(false);
            return View(model);
        }

        private async Task<InventoryAgingReportModel> BindModel(InventoryAgingReportModel model)
        {
            var oDataSet = new DataSet();
            var _List = new List<TextValue>();
            oDataSet = await _IInventoryAgingReport.GetCategory().ConfigureAwait(true);

            if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
            {


                foreach (DataRow row in oDataSet.Tables[0].Rows)
                {
                    _List.Add(new TextValue
                    {
                        Value = row["Entry_Id"].ToString(),
                        Text = row["ItemCategory"].ToString()
                    });
                }
                model.CategList = _List;
                _List = new List<TextValue>();

            }


            return model;
        }
        private async Task<InventoryAgingReportModel> BindModel1(InventoryAgingReportModel model)
        {
            var oDataSet = new DataSet();
            var _List = new List<TextValue>();
            oDataSet = await _IInventoryAgingReport.GetGroupName().ConfigureAwait(true);

            if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
            {


                foreach (DataRow row in oDataSet.Tables[0].Rows)
                {
                    _List.Add(new TextValue
                    {
                        Value = row["Group_Code"].ToString(),
                        Text = row["ParentGroup"].ToString()
                    });
                }
                model.GroupNameList = _List;
                _List = new List<TextValue>();

            }


            return model;
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
        public async Task<IActionResult> GetInventoryAgingReportDetailsData(string fromDate, string toDate,string CurrentDate, int WorkCenterid, string ReportType, int RMItemCode, int Storeid, int Foduration, string GroupName, string ItemCateg, int pageNumber = 1, int pageSize = 50, string SearchBox = "")
        {
            var model = new InventoryAgingReportModel();
            model = await _IInventoryAgingReport.GetInventoryAgingReportDetailsData(fromDate, toDate, CurrentDate, WorkCenterid, ReportType, RMItemCode, Storeid, Foduration,  GroupName,  ItemCateg);

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
            if (ReportType == "Day Wise Aging")
            {
                return PartialView("_InventoryAgingReportDayWiseGrid", model);
            }
            if (ReportType == "Agining Data Summary (As Per Actual NoOf Days)")
            {
                return PartialView("_InventoryAgingReportSummaryAsPerActualNoOfDays", model);
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
            else if(model.ReportType == "Day Wise Aging")
            {
                return PartialView("_InventoryAgingReportDayWiseGrid", model);
            }
            else if(model.ReportType == "Agining Data Summary (As Per Actual NoOf Days)")
            {
                return PartialView("_InventoryAgingReportSummaryAsPerActualNoOfDays", model);
            }

            else 
            {
                return PartialView("_InventoryAgingReportDetailGrid", model);
            }
           
        }
        public async Task<IActionResult> ExportInventoryAgeingToExcel(string ReportType)
        {
            //string modelJson = HttpContext.Session.GetString("KeyPOList");
            if (!_MemoryCache.TryGetValue("KeyInventoryAgeingList", out List<InventoryAgingReportModel> modelList))
            {
                return NotFound("No data available to export.");
            }

           
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("PO Register");

            var reportGenerators = new Dictionary<string, Action<IXLWorksheet, IList<InventoryAgingReportModel>>>
            {
                { "AginingDataSummary", EXPORT_InventoryAgingReportSummaryGrid },
                { "AginingDataBatchWise", EXPORT_InventoryAgingReportBatchWiseGrid },
                { "Day Wise Aging", EXPORT_InventoryAgingReportDayWiseGrid },
                { "Agining Data Summary (As Per Actual NoOf Days", EXPORT_InventoryAgingReportAsPerActualNoOfDaysGrid }

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
                "InventoryAgeingReport.xlsx"
            );
        }
        private void EXPORT_InventoryAgingReportSummaryGrid(IXLWorksheet sheet, IList<InventoryAgingReportModel> list)
        {
            string[] headers = {
                "#Sr","Store Name", "Part Code", "Item Name", "Unit", "Total Stock", "Rate", "Total Amount",
                "0–30", "31–60", "61–90", ">=91", "Type Item", "Group Name"
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
                sheet.Cell(row, 5).Value = item.Unit;
                sheet.Cell(row, 6).Value = item.TotalStock;
                sheet.Cell(row, 7).Value = item.Rate;
                sheet.Cell(row, 8).Value = item.TotalAmt;
                sheet.Cell(row, 9).Value = item.Aging_0_30;
                sheet.Cell(row, 10).Value = item.Aging_31_60;
                sheet.Cell(row, 11).Value = item.Aging_61_90;
                sheet.Cell(row, 12).Value = item.Aging_91;
                sheet.Cell(row, 13).Value = item.Type_Item;
                sheet.Cell(row, 14).Value = item.Group_Name;


                row++;
            }
        }
        private void EXPORT_InventoryAgingReportBatchWiseGrid(IXLWorksheet sheet, IList<InventoryAgingReportModel> list)
        {
            string[] headers = {
                "#Sr", "Store Name", "Part Code", "Item Name", "Unit", "Total Stock", "Rate", "Total Amount",
    "0-30", "31-60", "61-90", "≥91", "Batch No", "Unique Batch No", "Type Item", "Group Name"
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
                sheet.Cell(row, 5).Value = item.Unit;
                sheet.Cell(row, 6).Value = item.TotalStock;
                sheet.Cell(row, 7).Value = item.Rate;
                sheet.Cell(row, 8).Value = item.TotalAmt;

                sheet.Cell(row, 9).Value = item.Aging_0_30;
                sheet.Cell(row, 10).Value = item.Aging_31_60;
                sheet.Cell(row, 11).Value = item.Aging_61_90;
                sheet.Cell(row, 12).Value = item.Aging_91;
                sheet.Cell(row, 13).Value = item.BatchNo;
                sheet.Cell(row, 14).Value = item.UniqueBatchNo;
                sheet.Cell(row, 15).Value = item.Type_Item;
                sheet.Cell(row, 16).Value = item.Group_Name;



                row++;
            }
        }
        private void EXPORT_InventoryAgingReportDayWiseGrid(IXLWorksheet sheet, IList<InventoryAgingReportModel> list)
        {
            string[] headers = {
                "#Sr", "Store Name", "Part Code", "Item Name", "Total Stock", "Unit", "Rate",
                "Total Amount", "Item Age", "Batch No", "Unique Batch No", "Type Item", "Group Name"

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
                sheet.Cell(row, 5).Value = item.TotalStock;
                sheet.Cell(row, 6).Value = item.Unit;
                sheet.Cell(row, 7).Value = item.Rate;
                sheet.Cell(row, 8).Value = item.TotalAmt;
                sheet.Cell(row, 9).Value = item.ItemAge;
                sheet.Cell(row, 10).Value = item.BatchNo;
                sheet.Cell(row, 11).Value = item.UniqueBatchNo;
                sheet.Cell(row, 12).Value = item.Type_Item;
                sheet.Cell(row, 13).Value = item.Group_Name;



                row++;
            }
        }
         private void EXPORT_InventoryAgingReportAsPerActualNoOfDaysGrid(IXLWorksheet sheet, IList<InventoryAgingReportModel> list)
        {
            string[] headers = {
                "#Sr","Store Name", "Part Code", "Item Name", "Unit", "Total Stock",
"Item Age", "Rate", "Total Amount", "Type Item", "Group Name"

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
                sheet.Cell(row, 5).Value = item.Unit;
                sheet.Cell(row, 6).Value = item.TotalStock;
                sheet.Cell(row, 7).Value = item.ItemAge;
                sheet.Cell(row, 8).Value = item.Rate;
                sheet.Cell(row, 9).Value = item.TotalAmt;
                sheet.Cell(row, 10).Value = item.Type_Item;
                sheet.Cell(row, 11).Value = item.Group_Name;



                row++;
            }
        }

        [HttpGet]
        public IActionResult GetInventoryAgingReportForPDF()
        {
            if (_MemoryCache.TryGetValue("KeyInventoryAgeingList", out List<InventoryAgingReportModel> stockRegisterList))
            {
                return Json(stockRegisterList);
            }
            return Json(new List<InventoryAgingReportModel>());
        }

    }
}
