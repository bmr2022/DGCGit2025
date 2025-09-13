using ClosedXML.Excel;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
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
        //public async Task<IActionResult> GetSaleOrderDetailsData(string OrderSchedule, string ReportType, string PartCode, string ItemName, string Sono, string CustOrderNo, string CustomerName, string SalesPersonName, string SchNo, string FromDate, string ToDate, int pageNumber = 1, int pageSize = 50, string SearchBox = "")
        //{
        //    var model = new SaleOrderRegisterModel();
        //    model = await _ISaleOrderRegister.GetSaleOrderDetailsData( OrderSchedule,  ReportType,  PartCode,  ItemName,  Sono,  CustOrderNo,  CustomerName,  SalesPersonName,  SchNo,  FromDate,  ToDate);

        //    var modelList = model?.saleOrderRegisterGrid ?? new List<SaleOrderRegisterModel>();


        //    if (string.IsNullOrWhiteSpace(SearchBox))
        //    {
        //        model.TotalRecords = modelList.Count();
        //        model.PageNumber = pageNumber;
        //        model.PageSize = pageSize;
        //        model.saleOrderRegisterGrid = modelList
        //        .Skip((pageNumber - 1) * pageSize)
        //           .Take(pageSize)
        //           .ToList();
        //    }
        //    else
        //    {
        //        List<SaleOrderRegisterModel> filteredResults;
        //        if (string.IsNullOrWhiteSpace(SearchBox))
        //        {
        //            filteredResults = modelList.ToList();
        //        }
        //        else
        //        {
        //            filteredResults = modelList
        //                .Where(i => i.GetType().GetProperties()
        //                    .Where(p => p.PropertyType == typeof(string))
        //                    .Select(p => p.GetValue(i)?.ToString())
        //                    .Any(value => !string.IsNullOrEmpty(value) &&
        //                                  value.Contains(SearchBox, StringComparison.OrdinalIgnoreCase)))
        //                .ToList();


        //            if (filteredResults.Count == 0)
        //            {
        //                filteredResults = modelList.ToList();
        //            }
        //        }

        //        model.TotalRecords = filteredResults.Count;
        //        model.saleOrderRegisterGrid = filteredResults.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
        //        model.PageNumber = pageNumber;
        //        model.PageSize = pageSize;
        //    }
        //    MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
        //    {
        //        AbsoluteExpiration = DateTime.Now.AddMinutes(60),
        //        SlidingExpiration = TimeSpan.FromMinutes(55),
        //        Size = 1024,
        //    };

        //    _MemoryCache.Set("KeySaleOrderList", modelList, cacheEntryOptions);

        //    //string serializedGrid = JsonConvert.SerializeObject(modelList);
        //    if (ReportType == "Sale Order Summary")
        //    {
        //        return PartialView("_SaleOrderSummary", model);
        //    }
        //    if (ReportType == "Sale Order Detail")
        //    {
        //        return PartialView("_SaleOrderDetail", model);
        //    }
        //    if (ReportType == "Schedule Summary")
        //    {
        //        return PartialView("_SaleOrderScheduleSummaryRegisterGrid", model);
        //    }
        //    if (ReportType == "Schedule Summary Detail")
        //    {
        //        return PartialView("_SaleOrderScheduleDetailRegisterGrid", model);
        //    }
        //    if (ReportType == "Monthly Order+Schedule Summary")
        //    {
        //        return PartialView("_SaleOrderMonthlyOrder+ScheduleSummary", model);
        //    }
        //    if (ReportType == "Day Wise Order+Schedule (Item Wise)")
        //    {
        //        return PartialView("_SaleOrderDayWiseOrder+Schedule(Item Wise)", model);
        //    }
        //    if (ReportType == "Monthly Order+Schedule+Pending Summary")
        //    {
        //        return PartialView("_SaleOrderMonthlyOrder+Schedule+PendingSummary", model);
        //    }
        //    if (ReportType == "Day Wise Order+Schedule (Item + Customer Wise)")
        //    {
        //        return PartialView("_SaleOrderDayWiseOrder+Schedule(Item + Customer Wise)", model);
        //    }

        //    return null;
        //}
        public async Task<IActionResult> GetSaleOrderDetailsData(string OrderSchedule, string ReportType, string PartCode, string ItemName, string Sono, string CustOrderNo, string CustomerName, string SalesPersonName, string SchNo, string FromDate, string ToDate, int pageNumber = 1, int pageSize = 50, string SearchBox = "")
        {
          
            string cacheKey = $"KeySaleOrderList_{ReportType}";

            List<SaleOrderRegisterModel> modelList;

            if (!_MemoryCache.TryGetValue(cacheKey, out modelList))
            {
                var model = await _ISaleOrderRegister.GetSaleOrderDetailsData(OrderSchedule, ReportType, PartCode, ItemName, Sono, CustOrderNo, CustomerName, SalesPersonName, SchNo, FromDate, ToDate);
                modelList = (List<SaleOrderRegisterModel>?)(model?.saleOrderRegisterGrid ?? new List<SaleOrderRegisterModel>());

                MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(60),
                    SlidingExpiration = TimeSpan.FromMinutes(55),
                    Size = 1024
                };

                _MemoryCache.Set(cacheKey, modelList, cacheEntryOptions);
            }

            var modelResult = new SaleOrderRegisterModel();

            List<SaleOrderRegisterModel> filteredResults = string.IsNullOrWhiteSpace(SearchBox)
                ? modelList
                : modelList.Where(i => i.GetType().GetProperties()
                    .Where(p => p.PropertyType == typeof(string))
                    .Select(p => p.GetValue(i)?.ToString())
                    .Any(value => !string.IsNullOrEmpty(value) && value.Contains(SearchBox, StringComparison.OrdinalIgnoreCase)))
                    .ToList();

            modelResult.TotalRecords = filteredResults.Count;
            modelResult.PageNumber = pageNumber;
            modelResult.PageSize = pageSize;
            modelResult.saleOrderRegisterGrid = filteredResults.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            return ReportType switch
            {
                "Sale Order Summary" => PartialView("_SaleOrderSummary", modelResult),
                "Sale Order Detail" => PartialView("_SaleOrderDetail", modelResult),
                "Schedule Summary" => PartialView("_SaleOrderScheduleSummaryRegisterGrid", modelResult),
                "Schedule Summary Detail" => PartialView("_SaleOrderScheduleDetailRegisterGrid", modelResult),
                "Monthly Order+Schedule Summary" => PartialView("_SaleOrderMonthlyOrder+ScheduleSummary", modelResult),
                "Day Wise Order+Schedule (Item Wise)" => PartialView("_SaleOrderDayWiseOrder+Schedule(Item Wise)", modelResult),
                "Monthly Order+Schedule+Pending Summary" => PartialView("_SaleOrderMonthlyOrder+Schedule+PendingSummary", modelResult),
                "Day Wise Order+Schedule (Item + Customer Wise)" => PartialView("_SaleOrderDayWiseOrder+Schedule(Item + Customer Wise)", modelResult),
                _ => null
            };
        }

        [HttpGet]
        public IActionResult GlobalSearch(string searchString, string ReportType = "Summary", int pageNumber = 1, int pageSize = 50)
        {
            SaleOrderRegisterModel model = new SaleOrderRegisterModel();
            model.ReportType = ReportType;
            if (string.IsNullOrWhiteSpace(searchString))
            {
                return PartialView("_SaleOrderSummary", new List<SaleOrderRegisterModel>());
            }
            string cacheKey = $"KeySaleOrderList_{ReportType}";
            if (!_MemoryCache.TryGetValue(cacheKey, out IList<SaleOrderRegisterModel> SaleOrderRegister) || SaleOrderRegister == null)
            {
                return PartialView("_SaleOrderSummary", new List<SaleOrderRegisterModel>());
            }

            List<SaleOrderRegisterModel> filteredResults;

            if (string.IsNullOrWhiteSpace(searchString))
            {
                filteredResults = SaleOrderRegister.ToList();
            }
            else
            {
                filteredResults = SaleOrderRegister
                    .Where(i => i.GetType().GetProperties()
                        .Where(p => p.PropertyType == typeof(string))
                        .Select(p => p.GetValue(i)?.ToString())
                        .Any(value => !string.IsNullOrEmpty(value) &&
                                      value.Contains(searchString, StringComparison.OrdinalIgnoreCase)))
                    .ToList();


                if (filteredResults.Count == 0)
                {
                    filteredResults = SaleOrderRegister.ToList();
                }
            }

            model.TotalRecords = filteredResults.Count;
            model.saleOrderRegisterGrid = filteredResults.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            model.PageNumber = pageNumber;
            model.PageSize = pageSize;
            if (model.ReportType == "Sale Order Summary")
            {
                return PartialView("_SaleOrderSummary", model);
            }
            if (model.ReportType == "Sale Order Detail")
            {
                return PartialView("_SaleOrderDetail", model);
            }
            if (model.ReportType == "Schedule Summary")
            {
                return PartialView("_SaleOrderScheduleSummaryRegisterGrid", model);
            }
            if (model.ReportType == "Schedule Summary Detail")
            {
                return PartialView("_SaleOrderScheduleDetailRegisterGrid", model);
            }
            if (model.ReportType == "Monthly Order+Schedule Summary")
            {
                return PartialView("_SaleOrderMonthlyOrder+ScheduleSummary", model);
            }
            if (model.ReportType == "Day Wise Order+Schedule (Item Wise)")
            {
                return PartialView("_SaleOrderDayWiseOrder+Schedule(Item Wise)", model);
            }
            if (model.ReportType == "Monthly Order+Schedule+Pending Summary")
            {
                return PartialView("_SaleOrderMonthlyOrder+Schedule+PendingSummary", model);
            }
            if (model.ReportType == "Day Wise Order+Schedule (Item + Customer Wise)")
            {
                return PartialView("_SaleOrderDayWiseOrder+Schedule(Item + Customer Wise)", model);
            }
            return null;
        }

        public async Task<IActionResult> ExportSaleOrderRegisterToExcel(string ReportType)
        {
            string cacheKey = $"KeySaleOrderList_{ReportType}";
            if (!_MemoryCache.TryGetValue(cacheKey, out List<SaleOrderRegisterModel> modelList))
            {
                return NotFound("No data available to export.");
            }


            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Sale Order Register");

            var reportGenerators = new Dictionary<string, Action<IXLWorksheet, IList<SaleOrderRegisterModel>>>
            {
                { "Sale Order Summary", EXPORT_SaleOrderSummary },
                { "Sale Order Detail", EXPORT_SaleOrderDetail },
                { "Schedule Summary", EXPORT_SaleOrderScheduleSummary },
                { "Schedule Summary Detail", EXPORT_SaleOrderScheduleDetail },
                { "Monthly Order+Schedule Summary", EXPORT_SaleOrderMonthlyOrderScheduleSummary },
                { "Day Wise Order+Schedule (Item Wise)", EXPORT_SaleOrderDayWiseOrderScheduleItemWise },
                { "Monthly Order+Schedule+Pending Summary", EXPORT_SaleOrderMonthlyOrderSchedulePendingSummary },
                { "Day Wise Order+Schedule (Item + Customer Wise)", EXPORT_SaleOrderDayWiseOrderScheduleItemCustomerWise }

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
                "SaleOrderRegister.xlsx"
            );
        }
        private void EXPORT_SaleOrderSummary(IXLWorksheet sheet, IList<SaleOrderRegisterModel> list)
        {
            string[] headers = {
                "#Sr", "Customer Name", "SO No", "Customer Order No", "SO Date", "Order Type", "SO Type", "SO For", "WEF", "SO Close Date", "SO Amm. No", "SO Amm. Eff. Date",
                "Delivery Address", "Consignee Name", "Consignee Address", "Order Amount", "Order Net Amount", "SO Confirm Date", "SO Complete", "Approved", "Approved Date", "Approved By",
                "Sales Person Name", "Sales Email ID", "Sales Mobile No", "Freight Paid By", "Insurance Applicable", "Mode of Transport", "Remark", "Deactivation Date", "Deactivated By", "Entry Machine Name", "Customer Location", "SO Entry ID", "SO Year Code"
            };



            for (int i = 0; i < headers.Length; i++)
                sheet.Cell(1, i + 1).Value = headers[i];

            int row = 2, srNo = 1;
            foreach (var item in list)
            {
                sheet.Cell(row, 1).Value = srNo++;
                sheet.Cell(row, 2).Value = item.CustomerName;
                sheet.Cell(row, 3).Value = item.SONo;
                sheet.Cell(row, 4).Value = item.CustOrderNo;
                sheet.Cell(row, 5).Value = string.IsNullOrEmpty(item.SODate) ? "" : item.SODate.Split(" ")[0];
                sheet.Cell(row, 6).Value = item.OrderType;
                sheet.Cell(row, 7).Value = item.SOType;
                sheet.Cell(row, 8).Value = item.SOFor;
                sheet.Cell(row, 9).Value = string.IsNullOrEmpty(item.WEF) ? "" : item.WEF.Split(" ")[0];
                sheet.Cell(row, 10).Value = string.IsNullOrEmpty(item.SOCloseDate) ? "" : item.SOCloseDate.Split(" ")[0];
                sheet.Cell(row, 11).Value = item.SOAmmNo;
                sheet.Cell(row, 12).Value = string.IsNullOrEmpty(item.SOAmmEffDate) ? "" : item.SOAmmEffDate.Split(" ")[0];
                sheet.Cell(row, 13).Value = item.DeliveryAddress;
                sheet.Cell(row, 14).Value = item.ConsigneeName;
                sheet.Cell(row, 15).Value = item.ConsigneeAddress;
                sheet.Cell(row, 16).Value = item.OrderAmt;
                sheet.Cell(row, 17).Value = item.OrderNetAmt;
                sheet.Cell(row, 18).Value = string.IsNullOrEmpty(item.SOConfirmDate) ? "" : item.SOConfirmDate.Split(" ")[0];
                sheet.Cell(row, 19).Value = item.SoComplete;
                sheet.Cell(row, 20).Value = item.Approved;
                sheet.Cell(row, 21).Value = string.IsNullOrEmpty(item.ApprovedDate) ? "" : item.ApprovedDate.Split(" ")[0];
                sheet.Cell(row, 22).Value = item.ApprovedByEmp;
                sheet.Cell(row, 23).Value = item.SalesPersonName;
                sheet.Cell(row, 24).Value = item.SalesEmailId;
                sheet.Cell(row, 25).Value = item.SalesMobileNo;
                sheet.Cell(row, 26).Value = item.FreightPaidBy;
                sheet.Cell(row, 27).Value = item.InsuApplicable;
                sheet.Cell(row, 28).Value = item.ModeTransport;
                sheet.Cell(row, 29).Value = item.Remark;
                sheet.Cell(row, 30).Value = string.IsNullOrEmpty(item.DeActiveDate) ? "" : item.DeActiveDate.Split(" ")[0];
                sheet.Cell(row, 31).Value = item.DeActiveByEmp;
                sheet.Cell(row, 32).Value = item.EntryByMachineName;
                sheet.Cell(row, 33).Value = item.CustomerLocation;
                sheet.Cell(row, 34).Value = item.SOEntryID;
                sheet.Cell(row, 35).Value = item.SOYearCode;

                row++;
            }
        }
        private void EXPORT_SaleOrderDetail(IXLWorksheet sheet, IList<SaleOrderRegisterModel> list)
        {
            string[] headers = {
              "#Sr", "Customer Name", "SO No", "Customer Order No", "SO Date", "Part Code", "Item Name", "Qty", "Pend. Qty", "Unit", "Rate", "Amount",
              "Delivery Date", "Order Type", "SO Type", "SO For", "WEF", "SO Close Date", "SO Amm. No", "SO Amm. Eff. Date"
            };



            for (int i = 0; i < headers.Length; i++)
                sheet.Cell(1, i + 1).Value = headers[i];

            int row = 2, srNo = 1;
            foreach (var item in list)
            {
                sheet.Cell(row, 1).Value = srNo++;
                sheet.Cell(row, 2).Value = item.CustomerName;
                sheet.Cell(row, 3).Value = item.SONo;
                sheet.Cell(row, 4).Value = item.CustOrderNo;
                sheet.Cell(row, 5).Value = string.IsNullOrEmpty(item.SODate) ? "" : item.SODate.Split(" ")[0];
                sheet.Cell(row, 6).Value = item.PartCode;
                sheet.Cell(row, 7).Value = item.ItemName;
                sheet.Cell(row, 8).Value = item.Qty;
                sheet.Cell(row, 9).Value = item.PendQty;
                sheet.Cell(row, 10).Value = item.Unit;
                sheet.Cell(row, 11).Value = item.Rate;
                sheet.Cell(row, 12).Value = item.Amount;
                sheet.Cell(row, 13).Value = string.IsNullOrEmpty(item.DeliveryDate) ? "" : item.DeliveryDate.Split(" ")[0];
                sheet.Cell(row, 14).Value = item.OrderType;
                sheet.Cell(row, 15).Value = item.SOType;
                sheet.Cell(row, 16).Value = item.SOFor;
                sheet.Cell(row, 17).Value = string.IsNullOrEmpty(item.WEF) ? "" : item.WEF.Split(" ")[0];
                sheet.Cell(row, 18).Value = string.IsNullOrEmpty(item.SOCloseDate) ? "" : item.SOCloseDate.Split(" ")[0];
                sheet.Cell(row, 19).Value = item.SOAmmNo;
                sheet.Cell(row, 20).Value = string.IsNullOrEmpty(item.SOAmmEffDate) ? "" : item.SOAmmEffDate.Split(" ")[0];

                row++;
            }
        }
        private void EXPORT_SaleOrderScheduleSummary(IXLWorksheet sheet, IList<SaleOrderRegisterModel> list)
        {
            string[] headers = {
              "#Sr", "Customer Name", "Schedule No", "Schedule Date", "Schedule Effective From", "Schedule Effective Till",
                "SO No", "Customer Order No", "SO Date", "Order Type", "Tentative Confirmation", "Schedule Completed",
                "Delivery Address", "Consignee Name", "Consignee Address", "Order Amount", "Order Net Amount", "Schedule Amendment No",
                "WEF", "SO Close Date", "SO Amendment No", "SO Amendment Eff. Date",
                "Sales Person Name", "Sales Email ID", "Sales Mobile No", "Approved By", "Mode of Transport",
                "Remark", "Entry Machine Name", "Customer Location"
            };



            for (int i = 0; i < headers.Length; i++)
                sheet.Cell(1, i + 1).Value = headers[i];

            int row = 2, srNo = 1;
            foreach (var item in list)
            {
                sheet.Cell(row, 1).Value = srNo++;
                sheet.Cell(row, 2).Value = item.CustomerName;
                sheet.Cell(row, 3).Value = item.SchNo;
                sheet.Cell(row, 4).Value = string.IsNullOrEmpty(item.SchDate) ? "" : item.SchDate.Split(" ")[0];
                sheet.Cell(row, 5).Value = string.IsNullOrEmpty(item.SchEffectFrom) ? "" : item.SchEffectFrom.Split(" ")[0];
                sheet.Cell(row, 6).Value = string.IsNullOrEmpty(item.SchEffTill) ? "" : item.SchEffTill.Split(" ")[0];
                sheet.Cell(row, 7).Value = item.SONo;
                sheet.Cell(row, 8).Value = item.CustOrderNo;
                sheet.Cell(row, 9).Value = string.IsNullOrEmpty(item.SODate) ? "" : item.SODate.Split(" ")[0];
                sheet.Cell(row, 10).Value = item.OrderType;
                sheet.Cell(row, 11).Value = item.TentConfirm;
                sheet.Cell(row, 12).Value = item.SchCompleted;
                sheet.Cell(row, 13).Value = item.DeliveryAddress;
                sheet.Cell(row, 14).Value = item.ConsigneeName;
                sheet.Cell(row, 15).Value = item.ConsigneeAddress;
                sheet.Cell(row, 16).Value = item.OrderAmt;
                sheet.Cell(row, 17).Value = item.OrderNetAmt;
                sheet.Cell(row, 18).Value = item.SchAmndNo;
                sheet.Cell(row, 19).Value = string.IsNullOrEmpty(item.WEF) ? "" : item.WEF.Split(" ")[0];
                sheet.Cell(row, 20).Value = string.IsNullOrEmpty(item.SOCloseDate) ? "" : item.SOCloseDate.Split(" ")[0];
                sheet.Cell(row, 21).Value = item.SOAmmNo;
                sheet.Cell(row, 22).Value = string.IsNullOrEmpty(item.SOAmmEffDate) ? "" : item.SOAmmEffDate.Split(" ")[0];
                sheet.Cell(row, 23).Value = item.SalesPersonName;
                sheet.Cell(row, 24).Value = item.SalesEmailId;
                sheet.Cell(row, 25).Value = item.SalesMobileNo;
                sheet.Cell(row, 26).Value = item.ApprovedByEmp;
                sheet.Cell(row, 27).Value = item.ModeTransport;
                sheet.Cell(row, 28).Value = item.Remark;
                sheet.Cell(row, 29).Value = item.EntryByMachineName;
                sheet.Cell(row, 30).Value = item.CustomerLocation;

                row++;
            }
        }
        private void EXPORT_SaleOrderScheduleDetail(IXLWorksheet sheet, IList<SaleOrderRegisterModel> list)
        {
            string[] headers = {
               "#Sr", "Customer Name", "SO No", "Customer Order No", "Schedule No", "Schedule Date",
                "Part Code", "Item Name", "Qty", "Unit", "Rate", "Amount",
                "Order Type", "SO Type", "SO For", "WEF", "SO Close Date", "SO Amendment No", "SO Amendment Eff. Date"
            };



            for (int i = 0; i < headers.Length; i++)
                sheet.Cell(1, i + 1).Value = headers[i];

            int row = 2, srNo = 1;
            foreach (var item in list)
            {
                sheet.Cell(row, 1).Value = srNo++;
                sheet.Cell(row, 2).Value = item.CustomerName;
                sheet.Cell(row, 3).Value = item.SONo;
                sheet.Cell(row, 4).Value = item.CustOrderNo;
                sheet.Cell(row, 5).Value = item.SchNo;
                sheet.Cell(row, 6).Value = string.IsNullOrEmpty(item.SchDate) ? "" : item.SchDate.Split(" ")[0];
                sheet.Cell(row, 7).Value = item.PartCode;
                sheet.Cell(row, 8).Value = item.ItemName;
                sheet.Cell(row, 9).Value = item.Qty;
                sheet.Cell(row, 10).Value = item.Unit;
                sheet.Cell(row, 11).Value = item.Rate;
                sheet.Cell(row, 12).Value = item.Amount;
                sheet.Cell(row, 13).Value = item.OrderType;
                sheet.Cell(row, 14).Value = item.SOType;
                sheet.Cell(row, 15).Value = item.SOFor;
                sheet.Cell(row, 16).Value = string.IsNullOrEmpty(item.WEF) ? "" : item.WEF.Split(" ")[0];
                sheet.Cell(row, 17).Value = string.IsNullOrEmpty(item.SOCloseDate) ? "" : item.SOCloseDate.Split(" ")[0];
                sheet.Cell(row, 18).Value = item.SOAmmNo;
                sheet.Cell(row, 19).Value = string.IsNullOrEmpty(item.SOAmmEffDate) ? "" : item.SOAmmEffDate.Split(" ")[0];


                row++;
            }
        }
        private void EXPORT_SaleOrderMonthlyOrderScheduleSummary(IXLWorksheet sheet, IList<SaleOrderRegisterModel> list)
        {
            string[] headers = {
               "#Sr", "Customer Name", "SO No", "Customer Order No", "SO Date", "Schedule No", "Schedule Date",
                "Part Code", "Item Name", "Qty", "Unit", "Rate", "Amount",
                "Order Type", "SO Type", "SO For", "WEF", "SO Close Date", "SO Amendment No", "SO Amendment Eff. Date"
            };



            for (int i = 0; i < headers.Length; i++)
                sheet.Cell(1, i + 1).Value = headers[i];

            int row = 2, srNo = 1;
            foreach (var item in list)
            {
                sheet.Cell(row, 1).Value = srNo++;
                sheet.Cell(row, 2).Value = item.CustomerName;
                sheet.Cell(row, 3).Value = item.SONo;
                sheet.Cell(row, 4).Value = item.CustOrderNo;
                sheet.Cell(row, 5).Value = string.IsNullOrEmpty(item.SODate) ? "" : item.SODate.Split(" ")[0];
                sheet.Cell(row, 6).Value = item.SchNo;
                sheet.Cell(row, 7).Value = string.IsNullOrEmpty(item.SchDate) ? "" : item.SchDate.Split(" ")[0];
                sheet.Cell(row, 8).Value = item.PartCode;
                sheet.Cell(row, 9).Value = item.ItemName;
                sheet.Cell(row, 10).Value = item.Qty;
                sheet.Cell(row, 11).Value = item.Unit;
                sheet.Cell(row, 12).Value = item.Rate;
                sheet.Cell(row, 13).Value = item.Amount;
                sheet.Cell(row, 14).Value = item.OrderType;
                sheet.Cell(row, 15).Value = item.SOType;
                sheet.Cell(row, 16).Value = item.SOFor;
                sheet.Cell(row, 17).Value = string.IsNullOrEmpty(item.WEF) ? "" : item.WEF.Split(" ")[0];
                sheet.Cell(row, 18).Value = string.IsNullOrEmpty(item.SOCloseDate) ? "" : item.SOCloseDate.Split(" ")[0];
                sheet.Cell(row, 19).Value = item.SOAmmNo;
                sheet.Cell(row, 20).Value = string.IsNullOrEmpty(item.SOAmmEffDate) ? "" : item.SOAmmEffDate.Split(" ")[0];


                row++;
            }
        }
        private void EXPORT_SaleOrderDayWiseOrderScheduleItemWise(IXLWorksheet sheet, IList<SaleOrderRegisterModel> list)
        {
            string[] headers = {
                 "#Sr","Item Name", "Part Code",
                "1", "2", "3", "4", "5", "6", "7", "8", "9", "10",
                "11", "12", "13", "14", "15", "16", "17", "18", "19", "20",
                "21", "22", "23", "24", "25", "26", "27", "28", "29", "30", "31"
            };



            for (int i = 0; i < headers.Length; i++)
                sheet.Cell(1, i + 1).Value = headers[i];

            int row = 2, srNo = 1;
            foreach (var item in list)
            {
                sheet.Cell(row, 1).Value = srNo++;
                sheet.Cell(row, 2).Value = item.ItemName;
                sheet.Cell(row, 3).Value = item.PartCode;
                sheet.Cell(row, 4).Value = item.Day1;
                sheet.Cell(row, 5).Value = item.Day2;
                sheet.Cell(row, 6).Value = item.Day3;
                sheet.Cell(row, 7).Value = item.Day4;
                sheet.Cell(row, 8).Value = item.Day5;
                sheet.Cell(row, 9).Value = item.Day6;
                sheet.Cell(row, 10).Value = item.Day7;
                sheet.Cell(row, 11).Value = item.Day8;
                sheet.Cell(row, 12).Value = item.Day9;
                sheet.Cell(row, 13).Value = item.Day10;
                sheet.Cell(row, 14).Value = item.Day11;
                sheet.Cell(row, 15).Value = item.Day12;
                sheet.Cell(row, 16).Value = item.Day13;
                sheet.Cell(row, 17).Value = item.Day14;
                sheet.Cell(row, 18).Value = item.Day15;
                sheet.Cell(row, 19).Value = item.Day16;
                sheet.Cell(row, 20).Value = item.Day17;
                sheet.Cell(row, 21).Value = item.Day18;
                sheet.Cell(row, 22).Value = item.Day19;
                sheet.Cell(row, 23).Value = item.Day20;
                sheet.Cell(row, 24).Value = item.Day21;
                sheet.Cell(row, 25).Value = item.Day22;
                sheet.Cell(row, 26).Value = item.Day23;
                sheet.Cell(row, 27).Value = item.Day24;
                sheet.Cell(row, 28).Value = item.Day25;
                sheet.Cell(row, 29).Value = item.Day26;
                sheet.Cell(row, 30).Value = item.Day27;
                sheet.Cell(row, 31).Value = item.Day28;
                sheet.Cell(row, 32).Value = item.Day29;
                sheet.Cell(row, 33).Value = item.Day30;
                sheet.Cell(row, 34).Value = item.Day31;


                row++;
            }
        }
        private void EXPORT_SaleOrderMonthlyOrderSchedulePendingSummary(IXLWorksheet sheet, IList<SaleOrderRegisterModel> list)
        {
            string[] headers = {
                "#Sr", "Customer Name", "SO No", "Customer Order No", "SO Date", "Schedule No", "Schedule Date",
                "Part Code", "Item Name", "Order Qty", "Bill Qty", "Pend Qty", "Unit", "Rate", "Amount",
                "Order Type", "WEF", "SO Close Date", "SO Amm No", "SO Amm Eff Date"
            };



            for (int i = 0; i < headers.Length; i++)
                sheet.Cell(1, i + 1).Value = headers[i];

            int row = 2, srNo = 1;
            foreach (var item in list)
            {
                sheet.Cell(row, 1).Value = srNo++;
                sheet.Cell(row, 2).Value = item.CustomerName;
                sheet.Cell(row, 3).Value = item.SONo;
                sheet.Cell(row, 4).Value = item.CustOrderNo;
                sheet.Cell(row, 5).Value = string.IsNullOrEmpty(item.SODate) ? "" : item.SODate.Split(' ')[0];
                sheet.Cell(row, 6).Value = item.SchNo;
                sheet.Cell(row, 7).Value = string.IsNullOrEmpty(item.SchDate) ? "" : item.SchDate.Split(' ')[0];
                sheet.Cell(row, 8).Value = item.PartCode;
                sheet.Cell(row, 9).Value = item.ItemName;
                sheet.Cell(row, 10).Value = item.OrderQty;
                sheet.Cell(row, 11).Value = item.BillQty;
                sheet.Cell(row, 12).Value = item.PendQty;
                sheet.Cell(row, 13).Value = item.Unit;
                sheet.Cell(row, 14).Value = item.Rate;
                sheet.Cell(row, 15).Value = item.Amount;
                sheet.Cell(row, 16).Value = item.OrderType;
                sheet.Cell(row, 17).Value = string.IsNullOrEmpty(item.WEF) ? "" : item.WEF.Split(' ')[0];
                sheet.Cell(row, 18).Value = string.IsNullOrEmpty(item.SOCloseDate) ? "" : item.SOCloseDate.Split(' ')[0];
                sheet.Cell(row, 19).Value = item.SOAmmNo;
                sheet.Cell(row, 20).Value = string.IsNullOrEmpty(item.SOAmmEffDate) ? "" : item.SOAmmEffDate.Split(' ')[0];

                row++;
            }
        }
         private void EXPORT_SaleOrderDayWiseOrderScheduleItemCustomerWise(IXLWorksheet sheet, IList<SaleOrderRegisterModel> list)
        {
            string[] headers = {
                "#Sr", "Customer Name", "Item Name", "Part Code",
    "Day 1", "Day 2", "Day 3", "Day 4", "Day 5", "Day 6", "Day 7",
    "Day 8", "Day 9", "Day 10", "Day 11", "Day 12", "Day 13", "Day 14",
    "Day 15", "Day 16", "Day 17", "Day 18", "Day 19", "Day 20",
    "Day 21", "Day 22", "Day 23", "Day 24", "Day 25", "Day 26",
    "Day 27", "Day 28", "Day 29", "Day 30", "Day 31"
            };



            for (int i = 0; i < headers.Length; i++)
                sheet.Cell(1, i + 1).Value = headers[i];

            int row = 2, srNo = 1;
            foreach (var item in list)
            {
                sheet.Cell(row, 1).Value = srNo++;
                sheet.Cell(row, 2).Value = item.CustomerName;
                sheet.Cell(row, 3).Value = item.ItemName;
                sheet.Cell(row, 4).Value = item.PartCode;

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
                sheet.Cell(row, 16).Value = item.Day12;
                sheet.Cell(row, 17).Value = item.Day13;
                sheet.Cell(row, 18).Value = item.Day14;
                sheet.Cell(row, 19).Value = item.Day15;
                sheet.Cell(row, 20).Value = item.Day16;
                sheet.Cell(row, 21).Value = item.Day17;
                sheet.Cell(row, 22).Value = item.Day18;
                sheet.Cell(row, 23).Value = item.Day19;
                sheet.Cell(row, 24).Value = item.Day20;
                sheet.Cell(row, 25).Value = item.Day21;
                sheet.Cell(row, 26).Value = item.Day22;
                sheet.Cell(row, 27).Value = item.Day23;
                sheet.Cell(row, 28).Value = item.Day24;
                sheet.Cell(row, 29).Value = item.Day25;
                sheet.Cell(row, 30).Value = item.Day26;
                sheet.Cell(row, 31).Value = item.Day27;
                sheet.Cell(row, 32).Value = item.Day28;
                sheet.Cell(row, 33).Value = item.Day29;
                sheet.Cell(row, 34).Value = item.Day30;
                sheet.Cell(row, 35).Value = item.Day31;

                row++;
            }
        }

    }
}
