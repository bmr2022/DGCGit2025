using ClosedXML.Excel;
using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
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
        public async Task<JsonResult> FillPONO(string FromDate, string ToDate)
        {
            var JSON = await _IOrderAmendHistory.FillPONO(FromDate, ToDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
         public async Task<JsonResult> FillVendorName(string FromDate, string ToDate)
        {
            var JSON = await _IOrderAmendHistory.FillVendorName(FromDate, ToDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
         public async Task<JsonResult> FillItemName(string FromDate, string ToDate)
        {
            var JSON = await _IOrderAmendHistory.FillItemName(FromDate, ToDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
         public async Task<JsonResult> FillPartCode(string FromDate, string ToDate)
        {
            var JSON = await _IOrderAmendHistory.FillPartCode(FromDate, ToDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
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
        public async Task<IActionResult> ExportOrderAmendHistoryToExcel(string ReportType)
        {
            string modelJson = HttpContext.Session.GetString("KeyOrderAmendHistoryGrid");
            List<OrderAmendHistoryModel> OrderAmendHistoryList = new List<OrderAmendHistoryModel>();
            if (!string.IsNullOrEmpty(modelJson))
            {
                OrderAmendHistoryList = JsonConvert.DeserializeObject<List<OrderAmendHistoryModel>>(modelJson);
            }

            if (OrderAmendHistoryList == null)
                return NotFound("No data available to export.");

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("PO Register");

            var reportGenerators = new Dictionary<string, Action<IXLWorksheet, IList<OrderAmendHistoryModel>>>
            {
                { "POSummary", EXPORTLISTOFPOSUMMARY },
                { "PODetail", EXPORTLISTOFPODETAIL },


            };

            if (reportGenerators.TryGetValue(ReportType, out var generator))
            {
                generator(worksheet, OrderAmendHistoryList);
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
                "OrderAmendHistory.xlsx"
            );
        }

        private void EXPORTLISTOFPOSUMMARY(IXLWorksheet sheet, IList<OrderAmendHistoryModel> list)
        {
            string[] headers = {
                "Sr#", "Vendor Name", "PONO", "PO Date", "Amm No", "Amm Effective Date", "WEF",
                "PO Close Date", "Order Type", "PO Type", "PO For", "Basic Amount", "Net Amount",
                "PO Canceled", "PO Complete", "Active", "Shipping Address",
                "PO Amend Year Code", "Amend PO Seq", "PO Entry ID", "PO Year Code"
            };


            for (int i = 0; i < headers.Length; i++)
                sheet.Cell(1, i + 1).Value = headers[i];

            int row = 2, srNo = 1;
            foreach (var item in list)
            {
                sheet.Cell(row, 1).Value = srNo++;
                sheet.Cell(row, 2).Value = item.PONO;
                sheet.Cell(row, 3).Value = string.IsNullOrEmpty(item.PODate) ? "" : item.PODate.Split(" ")[0];
                sheet.Cell(row, 4).Value = item.AmmNo;
                sheet.Cell(row, 5).Value = string.IsNullOrEmpty(item.AmmEffDate) ? "" : item.AmmEffDate.Split(" ")[0];
                sheet.Cell(row, 6).Value = string.IsNullOrEmpty(item.WEF) ? "" : item.WEF.Split(" ")[0];
                sheet.Cell(row, 7).Value = string.IsNullOrEmpty(item.POClosedate) ? "" : item.POClosedate.Split(" ")[0];
                sheet.Cell(row, 8).Value = item.OrderType;
                sheet.Cell(row, 9).Value = item.POType;
                sheet.Cell(row, 10).Value = item.POFor;
                sheet.Cell(row, 11).Value = item.BasicAmount;
                sheet.Cell(row, 12).Value = item.NetAmount;
                sheet.Cell(row, 13).Value = item.POCanceled;
                sheet.Cell(row, 14).Value = item.POComplete;
                sheet.Cell(row, 15).Value = item.Active;
                sheet.Cell(row, 16).Value = item.ShippingAddress;
                sheet.Cell(row, 17).Value = item.POAmendYearCode;
                sheet.Cell(row, 18).Value = item.AmendPOSeq;
                sheet.Cell(row, 19).Value = item.POEntryId;
                sheet.Cell(row, 20).Value = item.POYearCode;


                row++;
            }
        }     
        private void EXPORTLISTOFPODETAIL(IXLWorksheet sheet, IList<OrderAmendHistoryModel> list)
        {
            string[] headers = {
                "Sr#", "Vendor Name", "PONO", "PO Date", "WEF", "PO Close Date", "Order Type", "PO Type", "PO For", "Amm No",
    "Amm Eff Date", "Part Code", "Item Name", "HSN No", "PO Qty", "Unit", "Rate", "Disc %", "Disc Rs", "Amount",
    "Old Rate", "Remark", "Ammendment Reason", "Rate In Other Curr", "Rate Applicable On Unit", "Alt PO Qty",
    "Alt Unit", "Shipping Address", "Basic Amount", "Net Amount", "PO Amend Entry ID", "PO Amend Year Code",
    "Amend PO", "Amend PO Seq", "PO Entry ID", "PO Year Code", "PO Canceled", "PO Complete", "Active", "Account Code"
            };


            for (int i = 0; i < headers.Length; i++)
                sheet.Cell(1, i + 1).Value = headers[i];

            int row = 2, srNo = 1;
            foreach (var item in list)
            {
                sheet.Cell(row, 1).Value = srNo++;
                sheet.Cell(row, 2).Value = item.VendorName;
                sheet.Cell(row, 3).Value = item.PONO;
                sheet.Cell(row, 4).Value = string.IsNullOrEmpty(item.PODate) ? "" : item.PODate.Split(' ')[0];
                sheet.Cell(row, 5).Value = string.IsNullOrEmpty(item.WEF) ? "" : item.WEF.Split(' ')[0];
                sheet.Cell(row, 6).Value = string.IsNullOrEmpty(item.POClosedate) ? "" : item.POClosedate.Split(' ')[0];
                sheet.Cell(row, 7).Value = item.OrderType;
                sheet.Cell(row, 8).Value = item.POType;
                sheet.Cell(row, 9).Value = item.POFor;
                sheet.Cell(row, 10).Value = item.AmmNo;
                sheet.Cell(row, 11).Value = string.IsNullOrEmpty(item.AmmEffDate) ? "" : item.AmmEffDate.Split(' ')[0];
                sheet.Cell(row, 12).Value = item.PartCode;
                sheet.Cell(row, 13).Value = item.ItemName;
                sheet.Cell(row, 14).Value = item.HSNNo;
                sheet.Cell(row, 15).Value = item.POQty;
                sheet.Cell(row, 16).Value = item.Unit;
                sheet.Cell(row, 17).Value = item.Rate;
                sheet.Cell(row, 18).Value = item.DiscPer;
                sheet.Cell(row, 19).Value = item.DiscRs;
                sheet.Cell(row, 20).Value = item.Amount;
                sheet.Cell(row, 21).Value = item.OldRate;
                sheet.Cell(row, 22).Value = item.Remark;
                sheet.Cell(row, 23).Value = item.AmmendmentReason;
                sheet.Cell(row, 24).Value = item.RateInOtherCurr;
                sheet.Cell(row, 25).Value = item.RateApplicableOnUnit;
                sheet.Cell(row, 26).Value = item.AltPOQty;
                sheet.Cell(row, 27).Value = item.AltUnit;
                sheet.Cell(row, 28).Value = item.ShippingAddress;
                sheet.Cell(row, 29).Value = item.BasicAmount;
                sheet.Cell(row, 30).Value = item.NetAmount;
                sheet.Cell(row, 31).Value = item.POAmendEntryID;
                sheet.Cell(row, 32).Value = item.POAmendYearCode;
                sheet.Cell(row, 33).Value = item.AmendPO;
                sheet.Cell(row, 34).Value = item.AmendPOSeq;
                sheet.Cell(row, 35).Value = item.POEntryId;
                sheet.Cell(row, 36).Value = item.POYearCode;
                sheet.Cell(row, 37).Value = item.POCanceled;
                sheet.Cell(row, 38).Value = item.POComplete;
                sheet.Cell(row, 39).Value = item.Active;
                sheet.Cell(row, 40).Value = item.AccountCode;
                row++;
            }
        }

    }
}
