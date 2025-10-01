using eTactWeb.Data.Common;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using eTactWeb.DOM.Models;
using System.Globalization;
using ClosedXML.Excel;
using System;
using Microsoft.AspNetCore.Components;
using OfficeOpenXml;
namespace eTactWeb.Controllers
{
    public class PORegisterController : Controller
    {
        private readonly IDataLogic _IDataLogic;
        public IPORegister _IPORegister { get; }
        private readonly ILogger<PORegisterController> _logger;
        private readonly IConfiguration iconfiguration;
        public IWebHostEnvironment _IWebHostEnvironment { get; }
        private readonly IMemoryCache _MemoryCache;

        public PORegisterController(ILogger<PORegisterController> logger, IDataLogic iDataLogic, EncryptDecrypt encryptDecrypt, IWebHostEnvironment iWebHostEnvironment, IConfiguration iconfiguration, IPORegister pORegister, IMemoryCache iMemoryCache)
        {
            _logger = logger;
            _IDataLogic = iDataLogic;
            _IPORegister = pORegister;
            _IWebHostEnvironment = iWebHostEnvironment;
            this.iconfiguration = iconfiguration;
            _MemoryCache = iMemoryCache;
        }
        public IActionResult PORegister()
        {
            var model = new PORegisterModel();
            model.PORegisterDetails = new List<PORegisterDetail>();
            return View(model);
        }

        public async Task<IActionResult> GetPORegisterData(string FromDate, string ToDate, string ReportType, string  Partyname, string  partcode, string  itemName, string  POno, string  SchNo, string  OrderType, string  POFor, string  ItemType, string  ItemGroup, string showOnlyCompletedPO, string showClosedPO, string showOnlyActivePO, int pageNumber = 1, int pageSize = 50, string SearchBox = "")
        {
            var YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
            var model = new PORegisterModel();
            model = await _IPORegister.GetPORegisterData(FromDate, ToDate, ReportType, YearCode, Partyname, partcode, itemName, POno, SchNo, OrderType, POFor, ItemType, ItemGroup,  showOnlyCompletedPO,  showClosedPO,  showOnlyActivePO);
            model.ReportMode = ReportType;
            var modelList = model?.PORegisterDetails ?? new List<PORegisterDetail>();


            if (string.IsNullOrWhiteSpace(SearchBox))
            {
                model.TotalRecords = modelList.Count();
                model.PageNumber = pageNumber;
                model.PageSize = pageSize;
                model.PORegisterDetails = modelList
                .Skip((pageNumber - 1) * pageSize)
                   .Take(pageSize)
                   .ToList();
            }
            else
            {
                List<PORegisterDetail> filteredResults;
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
                model.PORegisterDetails = filteredResults.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
                model.PageNumber = pageNumber;
                model.PageSize = pageSize;
            }
            MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                SlidingExpiration = TimeSpan.FromMinutes(55),
                Size = 1024,
            };

            _MemoryCache.Set("KeyPOList", modelList, cacheEntryOptions);

            string serializedGrid = JsonConvert.SerializeObject(modelList);
            HttpContext.Session.SetString("KeyPOList", serializedGrid);
            return PartialView("_PORegisterGrid", model);
        }
        [HttpGet]
        public IActionResult GlobalSearch(string searchString, string dashboardType = "Summary", int pageNumber = 1, int pageSize = 50)
        {
            PORegisterModel model = new PORegisterModel();
            model.ReportMode = dashboardType;
            if (string.IsNullOrWhiteSpace(searchString))
            {
                return PartialView("_PORegisterGrid", new List<PORegisterDetail>());
            }
            //string cacheKey = $"KeyProdList_{dashboardType}";
            if (!_MemoryCache.TryGetValue("KeyPOList", out IList<PORegisterDetail> poRegisterDetail) || poRegisterDetail == null)
            {
                return PartialView("_PORegisterGrid", new List<PORegisterDetail>());
            }

            List<PORegisterDetail> filteredResults;

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
            model.PORegisterDetails = filteredResults.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            model.PageNumber = pageNumber;
            model.PageSize = pageSize;
            if (model.ReportMode == "PO vs Receiving Detail ( GateMRNQC)")
            {
                return PartialView("_POvsReceivingDetail", model);
            }
            else if (model.ReportMode == "LIST OF PO")
            {
                return PartialView("_ListOfPOReport", model);
            }

            else if (model.ReportMode == "PO+Sch Vs Receiving Summary")
            {
                return PartialView("_POSchVsReceivingSummary", model);
            }
            else if (model.ReportMode == "LISTOFSCHEDULESUMMARY")
            {
                return PartialView("_ListOfPOSummaryReport", model);
            }
            else if (model.ReportMode == "PARTYWISECONSOLIDATED")
            {
                return PartialView("_POPartyWiseConsolidatedReport", model);
            }
            else if (model.ReportMode == "CONSOLIDATED ( Part+Item Wise)")
            {
                return PartialView("_POConsolidatedReport", model);
            }
            else if (model.ReportMode == "SUMMRATEING")
            {
                return PartialView("_POSummRatingReport", model);
            }

            else if (model.ReportMode == "LISTOFSCHEDULE")
            {
                return PartialView("_ListOfSchduleReport", model);
            }
           
            else if (model.ReportMode == "ITEMWISECONSOLIDATED")
            {
                return PartialView("_POItemWiseConsolidatedReport", model);
            }
            else if (model.ReportMode == "PRICEHISTORY")
            {
                return PartialView("_POPriceHistoryReport", model);
            }
            else if (model.ReportMode == "Order vs Dispatch")
            {
                return PartialView("_POOrderDispatchReport", model);
            }
            //LIST OF PO WITH ITEM SUMMARY
            else
            {
                return PartialView("_PORegisterGrid", model);
            }
        }          
            
    
        //[HttpGet]
        //public IActionResult GetPORegisterDataForPDF()
        //{
        //    string modelJson = HttpContext.Session.GetString("KeyPORegsiterList");
        //    List<PORegisterDetail> stockRegisterList = new List<PORegisterDetail>();
        //    if (!string.IsNullOrEmpty(modelJson))
        //    {
        //        stockRegisterList = JsonConvert.DeserializeObject<List<PORegisterDetail>>(modelJson);
        //    }

        //    return Json(stockRegisterList);
        //}
        [HttpGet]
        public IActionResult GetPORegisterDataForPDF()
        {
            if (_MemoryCache.TryGetValue("KeyPOList", out List<PORegisterDetail> stockRegisterList))
            {
                return Json(stockRegisterList);
            }
            return Json(new List<PORegisterDetail>());
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


        public async Task<IActionResult> ExportPOStockRegisterToExcel(string ReportType)
        {
            string modelJson = HttpContext.Session.GetString("KeyPOList");
            List<PORegisterDetail> stockRegisterList = new List<PORegisterDetail>();
            if (!string.IsNullOrEmpty(modelJson))
            {
                stockRegisterList = JsonConvert.DeserializeObject<List<PORegisterDetail>>(modelJson);
            }

            if (stockRegisterList == null)
                return NotFound("No data available to export.");

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("PO Register");

            var reportGenerators = new Dictionary<string, Action<IXLWorksheet, IList<PORegisterDetail>>>
            {
                { "LISTOFSCHEDULESUMMARY", EXPORTLISTOFSCHEDULESUMMARY },
                { "LISTOFPO", EXPORTLISTOFPO },

                { "LISTOFSCHEDULE", EXPORTLISTOFSCHEDULE },
                { "SUMM", EXPORTSUMM },
                { "DETAIL", EXPORTDETAIL },
                { "SUMMRATEING", EXPORTSUMMRATEING },
                 { "CONSOLIDATED", EXPORTCONSOLIDATED },
                 { "PARTYWISECONSOLIDATED", EXPORTPARTYWISECONSOLIDATED },
                 { "ITEMWISECONSOLIDATED", EXPORTITEMWISECONSOLIDATED },
                { "PRICEHISTORY", EXPORTPRICEHISTORY },
                { "Order vs Dispatch", EXPORTOrdervsDispatch },
               
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
                "POStockRegisterReport.xlsx"
            );
        }


        private void EXPORTLISTOFSCHEDULESUMMARY(IXLWorksheet sheet, IList<PORegisterDetail> list)
        {
            string[] headers = {
                "Sr#", "PONO", "PO Date", "Schedule No", "Schedule Date", "Vendor",
                "Schedule Effective From Date", "Schedule Effective Till Date",
                "PO Effective Date", "PO Close Date", "PO Amendment Effective Date",
                "Order Type", "Delivery Terms", "Delivery Date", "PO FOR", "Currency"
            };


            for (int i = 0; i < headers.Length; i++)
                sheet.Cell(1, i + 1).Value = headers[i];

            int row = 2, srNo = 1;
            foreach (var item in list)
            {
                sheet.Cell(row, 1).Value = srNo++;
                sheet.Cell(row, 2).Value = item.PONO;
                sheet.Cell(row, 3).Value = item.PODate;
                sheet.Cell(row, 4).Value = item.SchNO;
                sheet.Cell(row, 5).Value = item.Schdate;
                sheet.Cell(row, 6).Value = item.Vendor;
                sheet.Cell(row, 7).Value = item.ScheduleEffectiveFromDate;
                sheet.Cell(row, 8).Value = item.ScheduleEffectiveTillDate;
                sheet.Cell(row, 9).Value = item.POEffDate;
                sheet.Cell(row, 10).Value = item.POclosedate;
                sheet.Cell(row, 11).Value = item.poAmmeffdate;
                sheet.Cell(row, 12).Value = item.ordertype;
                sheet.Cell(row, 13).Value = item.Deliveryterms;
                sheet.Cell(row, 14).Value = item.deliveryDate;
                sheet.Cell(row, 15).Value = item.POFOR;
                sheet.Cell(row, 16).Value = item.Currency;
                
                row++;
            }
        }

        private void EXPORTLISTOFPO(IXLWorksheet sheet, IList<PORegisterDetail> list)
        {
            string[] headers = {
            "srNo", "PONO", "PODate", "POtype", "POQty", "POEffDate", "POclosedate", "poAmmeffdate", "ordertype", "Vendor",
                "PartCode", "ItemName", "unit", "Rate", "DisPer", "DisAmt", "ItemAmount", "Ammendmentno", "Ammendmentdate", 
                "Assrate", "Deliveryterms", "deliveryDate", "POQty", "POFOR", "Currency", "Ammno", "AmmDate"


            };


            for (int i = 0; i < headers.Length; i++)
                sheet.Cell(1, i + 1).Value = headers[i];

            int row = 2, srNo = 1;
            foreach (var item in list)
            {
                sheet.Cell(row, 1).Value = srNo++;
                sheet.Cell(row, 2).Value = item.PONO;
                sheet.Cell(row, 3).Value = item.PODate;
                sheet.Cell(row, 4).Value = item.POtype;
                sheet.Cell(row, 5).Value = item.POQty;
                sheet.Cell(row, 6).Value = item.POEffDate;
                sheet.Cell(row, 7).Value = item.POclosedate;
                sheet.Cell(row, 8).Value = item.poAmmeffdate;
                sheet.Cell(row, 9).Value = item.ordertype;
                sheet.Cell(row, 10).Value = item.Vendor;
                sheet.Cell(row, 11).Value = item.PartCode;
                sheet.Cell(row, 12).Value = item.ItemName;
                sheet.Cell(row, 13).Value = item.unit;
                sheet.Cell(row, 14).Value = item.Rate;
                sheet.Cell(row, 15).Value = item.DisPer;
                sheet.Cell(row, 16).Value = item.DisAmt;
                sheet.Cell(row, 17).Value = item.ItemAmount;
                sheet.Cell(row, 18).Value = item.Ammendmentno;
                sheet.Cell(row, 19).Value = item.Ammendmentdate;
                sheet.Cell(row, 20).Value = item.Assrate;
                sheet.Cell(row, 21).Value = item.Deliveryterms;
                sheet.Cell(row, 22).Value = item.deliveryDate;
                sheet.Cell(row, 23).Value = item.POQty;
                sheet.Cell(row, 24).Value = item.POFOR;
                sheet.Cell(row, 25).Value = item.Currency;
                sheet.Cell(row, 26).Value = item.Ammno;
                sheet.Cell(row, 27).Value = item.AmmDate;




                row++;
            }
        }



        private void EXPORTLISTOFSCHEDULE(IXLWorksheet sheet, IList<PORegisterDetail> list)
        {
            string[] headers = {
            "srNo", "PONO", "PODate", "SchNO", "Schdate", "ScheduleEffectiveFromDate", "ScheduleEffectiveTillDate", 
                "POtype", "POEffDate", "POclosedate", "poAmmeffdate", "ordertype", "Vendor", "PartCode", "ItemName", "unit",
                "Rate", "SchQty", "DisPer", "DisAmt", "ItemAmount", "Assrate", "Deliveryterms", "deliveryDate", "POFOR", "Currency", "Ammno", "AmmDate"



            };


            for (int i = 0; i < headers.Length; i++)
                sheet.Cell(1, i + 1).Value = headers[i];

            int row = 2, srNo = 1;
            foreach (var item in list)
            {
                sheet.Cell(row, 1).Value = srNo++;
                sheet.Cell(row, 2).Value = item.PONO;
                sheet.Cell(row, 3).Value = item.PODate == null ? string.Empty : item.PODate.Split(" ")[0];
                sheet.Cell(row, 4).Value = item.SchNO;
                sheet.Cell(row, 5).Value = item.Schdate == null ? string.Empty : item.Schdate.Split(" ")[0];
                sheet.Cell(row, 6).Value = item.ScheduleEffectiveFromDate == null ? string.Empty : item.ScheduleEffectiveFromDate.Split(" ")[0];
                sheet.Cell(row, 7).Value = item.ScheduleEffectiveTillDate == null ? string.Empty : item.ScheduleEffectiveTillDate.Split(" ")[0];
                sheet.Cell(row, 8).Value = item.POtype;
                sheet.Cell(row, 9).Value = item.POEffDate == null ? string.Empty : item.POEffDate.Split(" ")[0];
                sheet.Cell(row, 10).Value = item.POclosedate == null ? string.Empty : item.POclosedate.Split(" ")[0];
                sheet.Cell(row, 11).Value = item.poAmmeffdate == null ? string.Empty : item.poAmmeffdate.Split(" ")[0];
                sheet.Cell(row, 12).Value = item.ordertype;
                sheet.Cell(row, 13).Value = item.Vendor;
                sheet.Cell(row, 14).Value = item.PartCode;
                sheet.Cell(row, 15).Value = item.ItemName;
                sheet.Cell(row, 16).Value = item.unit;
                sheet.Cell(row, 17).Value = item.Rate;
                sheet.Cell(row, 18).Value = item.SchQty;
                sheet.Cell(row, 19).Value = item.DisPer;
                sheet.Cell(row, 20).Value = item.DisAmt;
                sheet.Cell(row, 21).Value = item.ItemAmount;
                sheet.Cell(row, 22).Value = item.Assrate;
                sheet.Cell(row, 23).Value = item.Deliveryterms;
                sheet.Cell(row, 24).Value = item.deliveryDate == null ? string.Empty : item.deliveryDate.Split(" ")[0];
                sheet.Cell(row, 25).Value = item.POFOR;
                sheet.Cell(row, 26).Value = item.Currency;
                sheet.Cell(row, 27).Value = item.Ammno;
                sheet.Cell(row, 28).Value = item.AmmDate == null ? string.Empty : item.AmmDate.Split(" ")[0];



                row++;
            }
        }


        private void EXPORTSUMM(IXLWorksheet sheet, IList<PORegisterDetail> list)
        {
            string[] headers = {
           "srNo", "Vendor", "PONO", "POEffDate", "POEndDate", "POFOR", "SchNO", "Schdate", "SchYear", "PartCode", "ItemName", "Currency",
                "PORate", "POQty", "POValue", "RECQty", "QCOKQty", "PendQty", "MinLevel", "unit", "Minlvldays", "ItemGroup", "ItemCategory"


            };


            for (int i = 0; i < headers.Length; i++)
                sheet.Cell(1, i + 1).Value = headers[i];

            int row = 2, srNo = 1;
            foreach (var item in list)
            {
                sheet.Cell(row, 1).Value = srNo++;
                sheet.Cell(row, 2).Value = item.Vendor;
                sheet.Cell(row, 3).Value = item.PONO;
                sheet.Cell(row, 4).Value = item.POEffDate == null ? string.Empty : item.POEffDate.Split(" ")[0];
                sheet.Cell(row, 5).Value = item.POEndDate == null ? string.Empty : item.POEndDate.Split(" ")[0];
                sheet.Cell(row, 6).Value = item.POFOR;
                sheet.Cell(row, 7).Value = item.SchNO;
                sheet.Cell(row, 8).Value = item.Schdate == null ? string.Empty : item.Schdate.Split(" ")[0];
                sheet.Cell(row, 9).Value = item.SchYear;
                sheet.Cell(row, 10).Value = item.PartCode;
                sheet.Cell(row, 11).Value = item.ItemName;
                sheet.Cell(row, 12).Value = item.Currency;
                sheet.Cell(row, 13).Value = item.PORate;
                sheet.Cell(row, 14).Value = item.POQty;
                sheet.Cell(row, 15).Value = item.POValue;
                sheet.Cell(row, 16).Value = item.RECQty;
                sheet.Cell(row, 17).Value = item.QCOKQty;
                sheet.Cell(row, 18).Value = item.PendQty;
                sheet.Cell(row, 19).Value = item.MinLevel;
                sheet.Cell(row, 20).Value = item.unit;
                sheet.Cell(row, 21).Value = item.Minlvldays;
                sheet.Cell(row, 22).Value = item.ItemGroup;
                sheet.Cell(row, 23).Value = item.ItemCategory;




                row++;
            }
        }

        private void EXPORTDETAIL(IXLWorksheet sheet, IList<PORegisterDetail> list)
        {
            string[] headers = {
            "srNo", "PONO", "PODate", "Account_name", "POFOR", "SchNO", "Schdate", "PartCode", "ItemName", "POQty", "Currency",
                "PORate", "MRNNo", "MRNDATE", "GateNo", "GateYearCode", "GateDate", "BillQty", "RECQty", "AcceptedQty", "InvNo",
                "INVDate", "MIRNo", "MIRDate", "BatchNo", "UniqueBatchno", "ItemGroup", "ItemCategory", "POYearCode", "SchYear", "DOMESTICIMPORT"

            };


            for (int i = 0; i < headers.Length; i++)
                sheet.Cell(1, i + 1).Value = headers[i];

            int row = 2, srNo = 1;
            foreach (var item in list)
            {
                sheet.Cell(row, 1).Value = srNo++;
                sheet.Cell(row, 2).Value = item.PONO;
                sheet.Cell(row, 3).Value = item.PODate;
                sheet.Cell(row, 4).Value = item.Account_name;
                sheet.Cell(row, 5).Value = item.POFOR;
                sheet.Cell(row, 6).Value = item.SchNO;
                sheet.Cell(row, 7).Value = item.Schdate;
                sheet.Cell(row, 8).Value = item.PartCode;
                sheet.Cell(row, 9).Value = item.ItemName;
                sheet.Cell(row, 10).Value = item.POQty;
                sheet.Cell(row, 11).Value = item.Currency;
                sheet.Cell(row, 12).Value = item.PORate;
                sheet.Cell(row, 13).Value = item.MRNNo;
                sheet.Cell(row, 14).Value = item.MRNDATE;
                sheet.Cell(row, 15).Value = item.GateNo;
                sheet.Cell(row, 16).Value = item.GateYearCode;
                sheet.Cell(row, 17).Value = item.GateDate;
                sheet.Cell(row, 18).Value = item.BillQty;
                sheet.Cell(row, 19).Value = item.RECQty;
                sheet.Cell(row, 20).Value = item.AcceptedQty;
                sheet.Cell(row, 21).Value = item.InvNo;
                sheet.Cell(row, 22).Value = item.INVDate;
                sheet.Cell(row, 23).Value = item.MIRNo;
                sheet.Cell(row, 24).Value = item.MIRDate;
                sheet.Cell(row, 25).Value = item.BatchNo;
                sheet.Cell(row, 26).Value = item.UniqueBatchno;
                sheet.Cell(row, 27).Value = item.ItemGroup;
                sheet.Cell(row, 28).Value = item.ItemCategory;
                sheet.Cell(row, 29).Value = item.POYearCode;
                sheet.Cell(row, 30).Value = item.SchYear;
                sheet.Cell(row, 31).Value = item.DOMESTICIMPORT;



                row++;
            }
        }

        private void EXPORTSUMMRATEING(IXLWorksheet sheet, IList<PORegisterDetail> list)
        {
            string[] headers = {
            "srNo", "ItemName", "PartCode", "Currency", "POQty", "POValue", "RECQty", "RecValue", "QCOKQty", "BalQty", "BalValue", "Rating", "PartyName"


            };


            for (int i = 0; i < headers.Length; i++)
                sheet.Cell(1, i + 1).Value = headers[i];

            int row = 2, srNo = 1;
            foreach (var item in list)
            {
                sheet.Cell(row, 1).Value = srNo++;
                sheet.Cell(row, 2).Value = item.ItemName;
                sheet.Cell(row, 3).Value = item.PartCode;
                sheet.Cell(row, 4).Value = item.Currency;
                sheet.Cell(row, 5).Value = item.POQty;
                sheet.Cell(row, 6).Value = item.POValue;
                sheet.Cell(row, 7).Value = item.RECQty;
                sheet.Cell(row, 8).Value = item.RecValue;
                sheet.Cell(row, 9).Value = item.QCOKQty;
                sheet.Cell(row, 10).Value = item.BalQty;
                sheet.Cell(row, 11).Value = item.BalValue;
                sheet.Cell(row, 12).Value = item.Rating;
                sheet.Cell(row, 13).Value = item.PartyName;


                row++;
            }
        }
         private void EXPORTCONSOLIDATED(IXLWorksheet sheet, IList<PORegisterDetail> list)
         {
            string[] headers = {
                "srNo", "ItemName", "PartCode", "ItemName", "POQty", 
                "RECQty", "QCOKQty", "PendQty", "MinLevel","Unit","Minlvldays", "Rating", "ItemGroup","ItemCategory"
            };


            for (int i = 0; i < headers.Length; i++)
                sheet.Cell(1, i + 1).Value = headers[i];

            int row = 2, srNo = 1;
            foreach (var item in list)
            {
                sheet.Cell(row, 1).Value = srNo++;
                sheet.Cell(row, 2).Value = item.ItemName;
                sheet.Cell(row, 3).Value = item.PartCode;
                sheet.Cell(row, 4).Value = item.ItemName;
                sheet.Cell(row, 5).Value = item.POQty;
                sheet.Cell(row, 6).Value = item.RECQty;
                sheet.Cell(row, 7).Value = item.QCOKQty;
                sheet.Cell(row,8).Value = item.PendQty;     
                sheet.Cell(row, 9).Value = item.MinLevel;
                sheet.Cell(row, 10).Value = item.unit;
                sheet.Cell(row, 11).Value = item.Minlvldays;
                sheet.Cell(row, 12).Value = item.Rating;
                sheet.Cell(row, 13).Value = item.ItemGroup; 
                sheet.Cell(row, 14).Value = item.ItemCategory; 

                row++;
            }
          }

       private void EXPORTPARTYWISECONSOLIDATED(IXLWorksheet sheet, IList<PORegisterDetail> list)
         {
            string[] headers = {
                "srNo", "Vendor Name", "POQty", "RECQty", "QCOKQty",
                "PendQty"
            };


            for (int i = 0; i < headers.Length; i++)
                sheet.Cell(1, i + 1).Value = headers[i];

            int row = 2, srNo = 1;
            foreach (var item in list)
            {
                sheet.Cell(row, 1).Value = srNo++;
                sheet.Cell(row, 2).Value = item.Account_name;
                sheet.Cell(row, 3).Value = item.POQty;
                sheet.Cell(row, 4).Value = item.RECQty;
                sheet.Cell(row, 5).Value = item.QCOKQty;
                sheet.Cell(row, 6).Value = item.PendQty;
                row++;
            }
       }
        private void EXPORTITEMWISECONSOLIDATED(IXLWorksheet sheet, IList<PORegisterDetail> list)
        {
            string[] headers = {
                "srNo","PartCode", "ItemName", "POQty","POValue","RECQty","RecValue","QCOKQty",
                "PendQty","MinLevel","unit", "Minlvldays","Rating","ItemGroup","ItemCategory"
            };



            for (int i = 0; i < headers.Length; i++)
                sheet.Cell(1, i + 1).Value = headers[i];

            int row = 2, srNo = 1;
            foreach (var item in list)
            {
                sheet.Cell(row, 1).Value = srNo++;
                sheet.Cell(row, 2).Value = item.PartCode;
                sheet.Cell(row, 3).Value = item.ItemName;
                sheet.Cell(row, 4).Value = item.POQty;
                sheet.Cell(row, 5).Value = item.POValue;
                sheet.Cell(row, 6).Value = item.RECQty;
                sheet.Cell(row, 7).Value = item.RecValue;
                sheet.Cell(row, 8).Value = item.QCOKQty;
                sheet.Cell(row, 9).Value = item.PendQty;
                sheet.Cell(row, 10).Value = item.MinLevel;
                sheet.Cell(row, 11).Value = item.unit;
                sheet.Cell(row, 12).Value = item.Minlvldays;
                sheet.Cell(row, 13).Value = item.Rating;
                sheet.Cell(row, 14).Value = item.ItemGroup;
                sheet.Cell(row, 15).Value = item.ItemCategory;

                row++;
            }
        }
        private void EXPORTPRICEHISTORY(IXLWorksheet sheet, IList<PORegisterDetail> list)
        {
            string[] headers = {
               "srNo", "PartyName", "PONO", "PODate", "ItemName", "PartCode",
                "PoRate", "Currency", "POQty", "DisPer", "DisAmt",
                "POValue", "POEffDate", "POCloseDate", "POEntryId", "YearCode"
            };




            for (int i = 0; i < headers.Length; i++)
                sheet.Cell(1, i + 1).Value = headers[i];

            int row = 2, srNo = 1;
            foreach (var item in list)
            {
                sheet.Cell(row, 1).Value = srNo++;
                sheet.Cell(row, 2).Value = item.PartyName;
                sheet.Cell(row, 3).Value = item.PONO;
                sheet.Cell(row, 4).Value = item.PODate?.Split(' ')[0] ?? string.Empty;
                sheet.Cell(row, 5).Value = item.ItemName;
                sheet.Cell(row, 6).Value = item.PartCode;
                sheet.Cell(row, 7).Value = item.PORate;
                sheet.Cell(row, 8).Value = item.Currency;
                sheet.Cell(row, 9).Value = item.POQty;
                sheet.Cell(row, 10).Value = item.DisPer;
                sheet.Cell(row, 11).Value = item.DisAmt;
                sheet.Cell(row, 12).Value = item.POValue;
                sheet.Cell(row, 13).Value = item.poAmmeffdate?.Split(' ')[0] ?? string.Empty;
                sheet.Cell(row, 14).Value = item.POclosedate?.Split(' ')[0] ?? string.Empty;
                sheet.Cell(row, 15).Value = item.POEntryId;
                sheet.Cell(row, 16).Value = item.POYearCode;


                row++;
            }
        }
         private void EXPORTOrdervsDispatch(IXLWorksheet sheet, IList<PORegisterDetail> list)
        {
            string[] headers = {
               "srNo","PartyName", "PONo", "PODate","Domestic/Import", "Sch No", "Sch Date", "Sch Year",
                "PartCode","ItemName", "POQty", "Rec Qty", "Qc Ok Qty", "Pend Qty","Unit"
            };




            for (int i = 0; i < headers.Length; i++)
                sheet.Cell(1, i + 1).Value = headers[i];

            int row = 2, srNo = 1;
            foreach (var item in list)
            {
                sheet.Cell(row, 1).Value = srNo++;
                sheet.Cell(row, 2).Value = item.Vendor;
                sheet.Cell(row, 3).Value = item.PONO;
                sheet.Cell(row, 4).Value = item.PODate?.Split(' ')[0] ?? string.Empty;
                sheet.Cell(row, 5).Value = item.DOMESTICIMPORT;
                sheet.Cell(row, 6).Value = item.SchNO;
                sheet.Cell(row, 7).Value = item.Schdate?.Split(' ')[0] ?? string.Empty;
                sheet.Cell(row, 8).Value = item.SchYear;
                sheet.Cell(row, 9).Value = item.PartCode;
                sheet.Cell(row, 10).Value = item.ItemName;
                sheet.Cell(row, 11).Value = item.POQty;
                sheet.Cell(row, 12).Value = item.RECQty;
                sheet.Cell(row, 13).Value = item.QCOKQty;
                sheet.Cell(row, 14).Value = item.PendQty;
                sheet.Cell(row, 15).Value = item.unit;


                row++;
            }
        }

       





    }
}
