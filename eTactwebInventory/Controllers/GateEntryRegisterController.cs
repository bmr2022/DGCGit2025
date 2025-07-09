using eTactWeb.Data.Common;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using eTactWeb.DOM.Models;
using System.Globalization;
using DocumentFormat.OpenXml.Drawing.Diagrams;
using System.Runtime.Caching;
using ClosedXML.Excel;

namespace eTactWeb.Controllers
{
    public class GateEntryRegisterController : Controller
    {
        private readonly IDataLogic _IDataLogic;
        public IGateEntryRegister _IGateEntryRegister { get; }
        private readonly ILogger<GateEntryRegisterController> _logger;
        private readonly IConfiguration iconfiguration;
        public IWebHostEnvironment _IWebHostEnvironment { get; }
		private readonly IMemoryCache _MemoryCache;
		public GateEntryRegisterController(ILogger<GateEntryRegisterController> logger, IDataLogic iDataLogic, IGateEntryRegister iGateEntryRegister, EncryptDecrypt encryptDecrypt, IWebHostEnvironment iWebHostEnvironment, IConfiguration iconfiguration, IMemoryCache iMemoryCache)
        {
            _logger = logger;
            _IDataLogic = iDataLogic;
            _IGateEntryRegister = iGateEntryRegister;
            _IWebHostEnvironment = iWebHostEnvironment;
            this.iconfiguration = iconfiguration;
			_MemoryCache = iMemoryCache;
		}
        [Route("{controller}/Index")]
        public IActionResult GateEntryRegister()
        {
            var model = new GateEntryRegisterModel();
            model.GateEntryRegisterDetail = new List<GateEntryRegisterDetail>();
            return View(model);
        }
        public async Task<IActionResult> GetGateRegisterData(string ReportType, string FromDate, string ToDate, string gateno, string docname, string PONo, string Schno, string PartCode, string ItemName, string invoiceNo, string VendorName, int pageNumber = 1, int pageSize = 50, string SearchBox = "")
        {
            var model = new GateEntryRegisterModel();
            if (string.IsNullOrEmpty(gateno)||gateno == "0" )
                {                gateno = "";            }
            if (string.IsNullOrEmpty(docname) || docname == "0")
            { docname = ""; }
            if (string.IsNullOrEmpty(PONo) || PONo == "0")
            { PONo = ""; }
            if (string.IsNullOrEmpty(Schno) || Schno == "0")
            { Schno = ""; }
            if (string.IsNullOrEmpty(PartCode) || PartCode == "0")
            { PartCode = ""; }
            if (string.IsNullOrEmpty(ItemName) || ItemName == "0")
            { ItemName = ""; }
            if (string.IsNullOrEmpty(invoiceNo) || invoiceNo == "0")
            { invoiceNo = ""; }
            if (string.IsNullOrEmpty(VendorName) || VendorName == "0")
            { VendorName = ""; }
       
            model = await _IGateEntryRegister.GetGateRegisterData(ReportType,  FromDate,  ToDate,  gateno,  docname,  PONo,  Schno,  PartCode,  ItemName,  invoiceNo,  VendorName);
            model.ReportMode= ReportType;

			var modelList = model?.GateEntryRegisterDetail ?? new List<GateEntryRegisterDetail>();


			if (string.IsNullOrWhiteSpace(SearchBox))
			{
				model.TotalRecords = modelList.Count();
				model.PageNumber = pageNumber;
				model.PageSize = pageSize;
				model.GateEntryRegisterDetail = modelList
				.Skip((pageNumber - 1) * pageSize)
				   .Take(pageSize)
				   .ToList();
			}
			else
			{
				List<GateEntryRegisterDetail> filteredResults;
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
				model.GateEntryRegisterDetail = filteredResults.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
				model.PageNumber = pageNumber;
				model.PageSize = pageSize;
			}

			MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                SlidingExpiration = TimeSpan.FromMinutes(55),
				Size = 1024,
            };
			_MemoryCache.Set("KeyGateEntryList", modelList, cacheEntryOptions);
			string serializedGrid = JsonConvert.SerializeObject(model.GateEntryRegisterDetail);
            HttpContext.Session.SetString("KeyGateEntryList", serializedGrid);
            return PartialView("_GateEntryRegisterGrid", model);
        }
        [HttpGet]
        public IActionResult GlobalSearch(string searchString, string dashboardType = "Summary", int pageNumber = 1, int pageSize = 50)
        {
            GateEntryRegisterModel model = new GateEntryRegisterModel();
            model.ReportType = dashboardType;
            if (string.IsNullOrWhiteSpace(searchString))
            {
                return PartialView("_GateEntryRegisterGrid", new List<GateEntryRegisterDetail>());
            }
            //string cacheKey = $"KeyProdList_{dashboardType}";
            if (!_MemoryCache.TryGetValue("KeyGateEntryList", out IList<GateEntryRegisterDetail> gateEntryRegisterDetail) || gateEntryRegisterDetail == null)
            {
                return PartialView("_GateEntryRegisterGrid", new List<GateEntryRegisterDetail>());
            }

            List<GateEntryRegisterDetail> filteredResults;

            if (string.IsNullOrWhiteSpace(searchString))
            {
                filteredResults = gateEntryRegisterDetail.ToList();
            }
            else
            {
                filteredResults = gateEntryRegisterDetail
                    .Where(i => i.GetType().GetProperties()
                        .Where(p => p.PropertyType == typeof(string))
                        .Select(p => p.GetValue(i)?.ToString())
                        .Any(value => !string.IsNullOrEmpty(value) &&
                                      value.Contains(searchString, StringComparison.OrdinalIgnoreCase)))
                    .ToList();


                if (filteredResults.Count == 0)
                {
                    filteredResults = gateEntryRegisterDetail.ToList();
                }
            }

            model.TotalRecords = filteredResults.Count;
            model.GateEntryRegisterDetail = filteredResults.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            model.PageNumber = pageNumber;
            model.PageSize = pageSize;
            if (model.ReportType == "ITEMSUMMARY")
            {
                return PartialView("_GateEntryRegisterItemSumm", model);
            }
            if (model.ReportType == "PARTYITEMSUMMARY")
            {
                return PartialView("_GateEntryRegisterPartyItemSumm", model);
            }
            if (model.ReportType == "PARTYITEMPOSUMMARY")
            {
                return PartialView("_GateEntryRegisterPartyPOSumm", model);
            }
            if (model.ReportType == "DAYWISEGATEENTRYLIST")
            {
                return PartialView("_GateEntryRegisterDayWiseList", model);
            }
            if (model.ReportType == "PENDGATEFORMRN")
            {
                return PartialView("_GateEntryRegisterPendGateForMRN", model);
            }
            else
            {
                return PartialView("_GateEntryRegisterGrid", model);
            }

        }
        public async Task<IActionResult> ExportGateEntryRegisterToExcel(string ReportType)
        {
            //string modelJson = HttpContext.Session.GetString("KeyPOList");
            if (!_MemoryCache.TryGetValue("KeyGateEntryList", out List<GateEntryRegisterDetail> modelList))
            {
                return NotFound("No data available to export.");
            }


            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("GateEntry Register");

            var reportGenerators = new Dictionary<string, Action<IXLWorksheet, IList<GateEntryRegisterDetail>>>
            {
                { "Detail", EXPORT_GateEntryDetailGrid },
                { "ITEMSUMMARY", EXPORT_GateEntryRegisterItemSummGrid },
                { "PARTYITEMSUMMARY", EXPORT_GateEntryRegisterPartyItemSummGrid },
                { "PARTYITEMPOSUMMARY", EXPORT_GateEntryRegisterPartyPOSummGrid },
                { "DAYWISEGATEENTRYLIST", EXPORT_GateEntryRegisterDayWiseListGrid },
                { "PENDGATEFORMRN", EXPORT_GateEntryRegisterPendGateForMRNGrid }

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
                "GateEntryReport.xlsx"
            );
        }
        private void EXPORT_GateEntryDetailGrid(IXLWorksheet sheet, IList<GateEntryRegisterDetail> list)
        {
            string[] headers = {
                "#Sr","Gate No", "Gate Date", "Entry Date", "Store Name", "Invoice No", "Invoice Date", "Document No",
                "Type Item", "Part Code", "Item Name", "PO No", "Schedule No", "Total Stock", "Unit", "Rate",
                "Total Amount", "Alt Qty", "Alt Unit", "Pending PO Qty", "Sale Bill No", "Sale Bill Qty",
                "Against Challan No", "Against Challan Qty", "PO Type", "Shelf Life", "Remark",
                "Prepared By", "Actual Entry By", "Updated By", "Last Updated Date", "Entry By Machine",
                "Gate Entry ID", "Gate Year Code"
            };



            for (int i = 0; i < headers.Length; i++)
                sheet.Cell(1, i + 1).Value = headers[i];

            int row = 2, srNo = 1;
            foreach (var item in list)
            {
                sheet.Cell(row, 1).Value = srNo++;
                sheet.Cell(row, 2).Value = item.GateNo;
                sheet.Cell(row, 3).Value = item.GDate;
                sheet.Cell(row, 4).Value = item.EntryDate;
                sheet.Cell(row, 5).Value = item.VendorName;
                sheet.Cell(row, 6).Value = item.InvoiceNo;
                sheet.Cell(row, 7).Value = item.InvoiceDate;
                sheet.Cell(row, 8).Value = item.DocNo;
                sheet.Cell(row, 9).Value = item.POTypeServiceItem;
                sheet.Cell(row, 10).Value = item.PartCode;
                sheet.Cell(row, 11).Value = item.ItemName;
                sheet.Cell(row, 12).Value = item.PONo;
                sheet.Cell(row, 13).Value = item.SchNo;
                sheet.Cell(row, 14).Value = item.Qty;
                sheet.Cell(row, 15).Value = item.Unit;
                sheet.Cell(row, 16).Value = item.Rate;
                sheet.Cell(row, 17).Value = item.Amount;
                sheet.Cell(row, 18).Value = item.AltQty;
                sheet.Cell(row, 19).Value = item.AltUnit;
                sheet.Cell(row, 20).Value = item.PendPOQty;
                sheet.Cell(row, 21).Value = item.SaleBillNo;
                sheet.Cell(row, 22).Value = item.SaleBillQty;
                sheet.Cell(row, 23).Value = item.AgainstChallanNo;
                sheet.Cell(row, 24).Value = item.AgainstChallanQty;
                sheet.Cell(row, 25).Value = item.POtype;
                sheet.Cell(row, 26).Value = item.ShelfLife;
                sheet.Cell(row, 27).Value = item.Remark;
                sheet.Cell(row, 28).Value = item.PreparedByEmp;
                sheet.Cell(row, 29).Value = item.ActualEntryByEMp;
                sheet.Cell(row, 30).Value = item.UpdatedByEMp;
                sheet.Cell(row, 31).Value = item.LastUpdatedDate;
                sheet.Cell(row, 32).Value = item.EntryByMachineName;
                sheet.Cell(row, 33).Value = item.GateEntryId;
                sheet.Cell(row, 34).Value = item.GateYearCode;

                row++;
            }
        }
        private void EXPORT_GateEntryRegisterItemSummGrid(IXLWorksheet sheet, IList<GateEntryRegisterDetail> list)
        {
            string[] headers = {
                "#Sr","Part Code", "Item Name", "Qty", "Unit", "Total Amount", "Alt Qty", "Alt Unit"
            };



            for (int i = 0; i < headers.Length; i++)
                sheet.Cell(1, i + 1).Value = headers[i];

            int row = 2, srNo = 1;
            foreach (var item in list)
            {
                sheet.Cell(row, 1).Value = srNo++;
                sheet.Cell(row, 2).Value = item.PartCode;
                sheet.Cell(row, 3).Value = item.ItemName;
                sheet.Cell(row, 4).Value = item.Qty;        
                sheet.Cell(row, 5).Value = item.Unit;
                sheet.Cell(row, 6).Value = item.Amount;      
                sheet.Cell(row, 7).Value = item.AltQty;
                sheet.Cell(row, 8).Value = item.AltUnit;
                row++;
            }
        }
        private void EXPORT_GateEntryRegisterPartyItemSummGrid(IXLWorksheet sheet, IList<GateEntryRegisterDetail> list)
        {
            string[] headers = {
               "#Sr", "Vendor Name", "Part Code", "Item Name", "Qty", "Unit",
    "Total Amount", "Alt Qty", "Alt Unit", "Company Address"
            };



            for (int i = 0; i < headers.Length; i++)
                sheet.Cell(1, i + 1).Value = headers[i];

            int row = 2, srNo = 1;
            foreach (var item in list)
            {
                sheet.Cell(row, 1).Value = srNo++;
                sheet.Cell(row, 2).Value = item.VendorName;
                sheet.Cell(row, 3).Value = item.PartCode;
                sheet.Cell(row, 4).Value = item.ItemName;
                sheet.Cell(row, 5).Value = item.Qty;
                sheet.Cell(row, 6).Value = item.Unit;
                sheet.Cell(row, 7).Value = item.Amount;
                sheet.Cell(row, 8).Value = item.AltQty;
                sheet.Cell(row, 9).Value = item.AltUnit;
                sheet.Cell(row, 10).Value = item.CompanyAddress;
                row++;
            }
        }
        private void EXPORT_GateEntryRegisterPartyPOSummGrid(IXLWorksheet sheet, IList<GateEntryRegisterDetail> list)
        {
            string[] headers = {
               "#Sr",  "Part Code", "Item Name", "Qty", "Unit", "Amount",
                "PO No", "Schedule No", "Alt Qty", "Alt Unit", "Company Address"
            };



            for (int i = 0; i < headers.Length; i++)
                sheet.Cell(1, i + 1).Value = headers[i];

            int row = 2, srNo = 1;
            foreach (var item in list)
            {
                sheet.Cell(row, 1).Value = srNo++;
                sheet.Cell(row, 2).Value = item.PartCode;
                sheet.Cell(row, 3).Value = item.ItemName;
                sheet.Cell(row, 4).Value = item.Qty;
                sheet.Cell(row, 5).Value = item.Unit;
                sheet.Cell(row, 6).Value = item.Amount;
                sheet.Cell(row, 7).Value = item.PONo;
                sheet.Cell(row, 8).Value = item.SchNo;
                sheet.Cell(row, 9).Value = item.AltQty;
                sheet.Cell(row, 10).Value = item.AltUnit;
                sheet.Cell(row, 11).Value = item.CompanyAddress;
                row++;
            }
        }
        private void EXPORT_GateEntryRegisterDayWiseListGrid(IXLWorksheet sheet, IList<GateEntryRegisterDetail> list)
        {
            string[] headers = {
               "#Sr",  "Gate No", "Gate Date", "Entry Date", "Vendor Name", "Invoice No",
                "Invoice Date", "Document No", "Gate Entry ID", "Gate Year Code",
                "Actual Entry By", "Last Updated By", "Last Updated Date", "Entry By Machine"
            };



            for (int i = 0; i < headers.Length; i++)
                sheet.Cell(1, i + 1).Value = headers[i];

            int row = 2, srNo = 1;
            foreach (var item in list)
            {
                sheet.Cell(row, 1).Value = srNo++;
                sheet.Cell(row, 3).Value = item.GDate == null ? string.Empty : item.GDate.Split(' ')[0];
                sheet.Cell(row, 4).Value = item.EntryDate == null ? string.Empty : item.EntryDate.Split(' ')[0];
                sheet.Cell(row, 5).Value = item.VendorName;
                sheet.Cell(row, 6).Value = item.InvoiceNo;
                sheet.Cell(row, 7).Value = item.InvoiceDate == null ? string.Empty : item.InvoiceDate.Split(' ')[0];
                sheet.Cell(row, 8).Value = item.DocNo;
                sheet.Cell(row, 9).Value = item.GateEntryId;
                sheet.Cell(row, 10).Value = item.GateYearCode;
                sheet.Cell(row, 11).Value = item.ActualEntryByEMp;
                sheet.Cell(row, 12).Value = item.LastUpdatedby;
                sheet.Cell(row, 13).Value = item.LastUpdatedDate == null ? string.Empty : item.LastUpdatedDate.Split(' ')[0];
                sheet.Cell(row, 14).Value = item.EntryByMachineName;
                row++;
            }
        }
        private void EXPORT_GateEntryRegisterPendGateForMRNGrid(IXLWorksheet sheet, IList<GateEntryRegisterDetail> list)
        {
            string[] headers = {
               "#Sr", "Gate No", "Gate Date", "Entry Date", "Vendor Name", "Invoice No",
                "Invoice Date", "Document No", "Remark", "Prepared By",
                "Actual Entry By", "Actual Entry Date", "Updated By", "Last Updated Date",
                "Gate Entry ID", "Gate Year Code", "Entry By Machine"
            };



            for (int i = 0; i < headers.Length; i++)
                sheet.Cell(1, i + 1).Value = headers[i];

            int row = 2, srNo = 1;
            foreach (var item in list)
            {
                sheet.Cell(row, 1).Value = srNo++;
                sheet.Cell(row, 2).Value = item.GateNo;
                sheet.Cell(row, 3).Value = item.GDate == null ? string.Empty : item.GDate.Split(' ')[0];
                sheet.Cell(row, 4).Value = item.EntryDate == null ? string.Empty : item.EntryDate.Split(' ')[0];
                sheet.Cell(row, 5).Value = item.VendorName;
                sheet.Cell(row, 6).Value = item.InvoiceNo;
                sheet.Cell(row, 7).Value = item.InvoiceDate == null ? string.Empty : item.InvoiceDate.Split(' ')[0];
                sheet.Cell(row, 8).Value = item.DocNo;
                sheet.Cell(row, 9).Value = item.Remark;
                sheet.Cell(row, 10).Value = item.PreparedByEmp;
                sheet.Cell(row, 11).Value = item.ActualEntryByEMp;
                sheet.Cell(row, 12).Value = item.ActualEntryDate == null ? string.Empty : item.ActualEntryDate.Split(' ')[0];
                sheet.Cell(row, 13).Value = item.UpdatedByEMp;
                sheet.Cell(row, 14).Value = item.LastUpdatedDate == null ? string.Empty : item.LastUpdatedDate.Split(' ')[0];
                sheet.Cell(row, 15).Value = item.GateEntryId;
                sheet.Cell(row, 16).Value = item.GateYearCode;
                sheet.Cell(row, 17).Value = item.EntryByMachineName;
                row++;
            }
        }

        public IActionResult GetGateEntryDataForPDF()
        {
            string modelJson = HttpContext.Session.GetString("KeyGateEntryList");
            List<GateEntryRegisterDetail> GateEntryRegisterList = new List<GateEntryRegisterDetail>();
            if (!string.IsNullOrEmpty(modelJson))
            {
                GateEntryRegisterList = JsonConvert.DeserializeObject<List<GateEntryRegisterDetail>>(modelJson);
            }

            return Json(GateEntryRegisterList);
        }
        public async Task<JsonResult> FillGateNo(string FromDate, string ToDate)
        {
            var JSON = await _IGateEntryRegister.FillGateNo(FromDate, ToDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillDocument(string FromDate, string ToDate)
        {

            var JSON = await _IGateEntryRegister.FillDocument(FromDate, ToDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillVendor(string FromDate, string ToDate)
        {
            var JSON = await _IGateEntryRegister.FillVendor(FromDate, ToDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillInvoice(string FromDate, string ToDate)
        {
            var JSON = await _IGateEntryRegister.FillInvoice(FromDate, ToDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        } 
        public async Task<JsonResult> FillItemNamePartcode(string FromDate, string ToDate)
        {
            var JSON = await _IGateEntryRegister.FillItemNamePartcode(FromDate, ToDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillPONO(string FromDate, string ToDate)
        {
            var JSON = await _IGateEntryRegister.FillPONO(FromDate, ToDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillSchNo(string FromDate, string ToDate)
        {
            var JSON = await _IGateEntryRegister.FillSchNo(  FromDate,  ToDate);
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
    }
}
