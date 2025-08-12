using eTactWeb.Data.Common;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using eTactWeb.DOM.Models;
using System.Globalization;
using Microsoft.AspNetCore.Http;
using DocumentFormat.OpenXml.Drawing.Diagrams;
using System.Runtime.Caching;
using ClosedXML.Excel;

namespace eTactWeb.Controllers
{
    public class MRNRegisterController : Controller
    {
        private readonly IDataLogic _IDataLogic;
        public IMRNRegister _IMRNRegister { get; }
        private readonly ILogger<MRNRegisterController> _logger;
        private readonly IConfiguration iconfiguration;
        private readonly IMemoryCache _MemoryCache;
        public IWebHostEnvironment _IWebHostEnvironment { get; }

        public MRNRegisterController(ILogger<MRNRegisterController> logger, IDataLogic iDataLogic, IMRNRegister iMRNRegister, EncryptDecrypt encryptDecrypt, IWebHostEnvironment iWebHostEnvironment, IConfiguration iconfiguration, IMemoryCache memoryCache)
        {
            _logger = logger;
            _IDataLogic = iDataLogic;
            _IMRNRegister = iMRNRegister;
            _IWebHostEnvironment = iWebHostEnvironment;
            this.iconfiguration = iconfiguration;
            _MemoryCache = memoryCache;
        }
        [Route("{controller}/Index")]
        public IActionResult MRNRegister()
        {
            var model = new MRNRegisterModel();
            model.MRNRegisterDetail = new List<MRNRegisterDetail>();
            return View(model);
        }
        //public async Task<IActionResult> GetMRNRegisterData(string MRNType, string ReportType, string FromDate, string ToDate, string gateno,string MRNno, string docname, string PONo, string Schno, string PartCode, string ItemName, string invoiceNo, string VendorName)
        //{
        //    var model = new MRNRegisterModel();
        //    if (string.IsNullOrEmpty(gateno)||gateno == "0" )
        //        {                gateno = "";            }
        //    if (string.IsNullOrEmpty(MRNno) || MRNno == "0")
        //    { MRNno = ""; }
        //    if (string.IsNullOrEmpty(docname) || docname == "0")
        //    { docname = ""; }
        //    if (string.IsNullOrEmpty(PONo) || PONo == "0")
        //    { PONo = ""; }
        //    if (string.IsNullOrEmpty(Schno) || Schno == "0")
        //    { Schno = ""; }
        //    if (string.IsNullOrEmpty(PartCode) || PartCode == "0")
        //    { PartCode = ""; }
        //    if (string.IsNullOrEmpty(ItemName) || ItemName == "0")
        //    { ItemName = ""; }
        //    if (string.IsNullOrEmpty(invoiceNo) || invoiceNo == "0")
        //    { invoiceNo = ""; }
        //    if (string.IsNullOrEmpty(VendorName) || VendorName == "0")
        //    { VendorName = ""; }

        //    MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
        //    {
        //        AbsoluteExpiration = DateTime.Now.AddMinutes(60),
        //        SlidingExpiration = TimeSpan.FromMinutes(55),
        //        Size = 1024,
        //    };
        //    model = await _IMRNRegister.GetMRNRegisterData(MRNType,ReportType,  FromDate,  ToDate,  gateno,  MRNno,docname,  PONo,  Schno,  PartCode,  ItemName,  invoiceNo,  VendorName);
        //    string serializedGrid = JsonConvert.SerializeObject(model.MRNRegisterDetail);
        //    HttpContext.Session.SetString("KeyMRNList", serializedGrid);
        //    model.ReportMode= ReportType;
        //    return PartialView("_MRNRegisterGrid", model);
        //}
        public async Task<IActionResult> GetMRNRegisterData(string MRNType,string ReportType,string FromDate,string ToDate,string gateno,string MRNno,string docname,
               string PONo,string Schno,string PartCode,string ItemName,string invoiceNo,string VendorName,int pageNumber = 1,int pageSize = 2,string SearchBox = "")
        {
            
            gateno = string.IsNullOrEmpty(gateno) || gateno == "0" ? "" : gateno;
            MRNno = string.IsNullOrEmpty(MRNno) || MRNno == "0" ? "" : MRNno;
            docname = string.IsNullOrEmpty(docname) || docname == "0" ? "" : docname;
            PONo = string.IsNullOrEmpty(PONo) || PONo == "0" ? "" : PONo;
            Schno = string.IsNullOrEmpty(Schno) || Schno == "0" ? "" : Schno;
            PartCode = string.IsNullOrEmpty(PartCode) || PartCode == "0" ? "" : PartCode;
            ItemName = string.IsNullOrEmpty(ItemName) || ItemName == "0" ? "" : ItemName;
            invoiceNo = string.IsNullOrEmpty(invoiceNo) || invoiceNo == "0" ? "" : invoiceNo;
            VendorName = string.IsNullOrEmpty(VendorName) || VendorName == "0" ? "" : VendorName;

           
            var model = await _IMRNRegister.GetMRNRegisterData(
                MRNType, ReportType, FromDate, ToDate, gateno, MRNno, docname, PONo, Schno, PartCode, ItemName, invoiceNo, VendorName);

            var modelList = model?.MRNRegisterDetail ?? new List<MRNRegisterDetail>();

            
            IEnumerable<MRNRegisterDetail> filteredResults = modelList;
            if (!string.IsNullOrWhiteSpace(SearchBox))
            {
                filteredResults = modelList
                    .Where(i => i.GetType().GetProperties()
                        .Where(p => p.PropertyType == typeof(string))
                        .Select(p => p.GetValue(i)?.ToString())
                        .Any(value => !string.IsNullOrEmpty(value) &&
                                      value.Contains(SearchBox, StringComparison.OrdinalIgnoreCase)))
                    .ToList();

                if (!filteredResults.Any())
                    filteredResults = modelList;
            }

        
            model.TotalRecords = filteredResults.Count();
            model.PageNumber = pageNumber;
            model.PageSize = pageSize;
            model.MRNRegisterDetail = filteredResults
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            
            MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                SlidingExpiration = TimeSpan.FromMinutes(55),
                Size = 1024,
            };
            _MemoryCache.Set("KeyMRNList", modelList, cacheEntryOptions);

            
            model.ReportMode = ReportType;
            return PartialView("_MRNRegisterGrid", model);
        }
        [HttpGet]
        public IActionResult GlobalSearch(string searchString, string dashboardType = "Summary", int pageNumber = 1, int pageSize = 2)
        {
            MRNRegisterModel model = new MRNRegisterModel();
            model.ReportType = dashboardType;
            if (string.IsNullOrWhiteSpace(searchString))
            {
                return PartialView("_MRNRegisterGrid", new List<MRNRegisterDetail>());
            }

            
            if (!_MemoryCache.TryGetValue("KeyMRNList", out IList<MRNRegisterDetail> mrnList) || mrnList == null)
            {
                return PartialView("_MRNRegisterGrid", new List<MRNRegisterDetail>());
            }

            
            List<MRNRegisterDetail> filteredResults;
            if (string.IsNullOrWhiteSpace(searchString))
            {
                filteredResults = mrnList.ToList();
            }
            else
            {
                filteredResults = mrnList
                    .Where(i => i.GetType().GetProperties()
                        .Where(p => p.PropertyType == typeof(string))
                        .Select(p => p.GetValue(i)?.ToString())
                        .Any(value => !string.IsNullOrEmpty(value) &&
                                      value.Contains(searchString, StringComparison.OrdinalIgnoreCase)))
                    .ToList();

                if (filteredResults.Count == 0)
                {
                    filteredResults = mrnList.ToList();
                }
            }

          
            model.TotalRecords = filteredResults.Count;
            model.PageNumber = pageNumber;
            model.PageSize = pageSize;
            model.MRNRegisterDetail = filteredResults
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            if (model.ReportType == "vendorItemWiseSummary")
            {
                return PartialView("_MRNRegisterVendorItemSumm", model);
            }
            else if (model.ReportType == "vendorItemWiseConsolidated")
            {
                return PartialView("_MRNRegisterVendorItemConsolidated", model);
            }
            else if (model.ReportType == "vendorWiseConsolidated")
            {
                return PartialView("_MRNRegisterVendorWiseConsolidated", model);
            }
            else if (model.ReportType == "ItemWiseConsolidated")
            {
                return PartialView("_MRNRegisterItemConsolidated", model);
            }
            else if (model.ReportType == "mrnqcdetail")
            {
                return PartialView("_MRNRegisterPartyPOSumm", model);
            }
            else if (model.ReportType == "DAYWISEMRNENTRYLIST")
            {
                return PartialView("_MRNRegisterDayWise1", model);
            }
            else if (model.ReportType == "PENDMRNFORQC(SUMMARY)")
            {
                return PartialView("_MRNRegisterPendMRNForQCSummary", model);
            }
            else if (model.ReportType == "PENDMRNFORQC(DEATIL)")
            {
                return PartialView("_MRNRegisterPendMRNForQCDetail", model);
            }
            else if (model.ReportType == "Short Excess Register")
            {
                return PartialView("_MRNRegisterShortExcess1", model);
            }
            else
            {
                 return PartialView("_MRNRegisterGrid", model);
            }

        }
        public async Task<IActionResult> ExportIMRNRegisterToExcel(string ReportType)
        {
            //string modelJson = HttpContext.Session.GetString("KeyPOList");
            if (!_MemoryCache.TryGetValue("KeyMRNList", out List<MRNRegisterDetail> modelList))
            {
                return NotFound("No data available to export.");
            }


            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("PO Register");

            var reportGenerators = new Dictionary<string, Action<IXLWorksheet, IList<MRNRegisterDetail>>>
            {
                { "DETAIL", EXPORT_MRNRegisterDetail},
                { "vendorItemWiseSummary", EXPORT_MRNRegisterVendorItemSumm},
    { "vendorItemWiseConsolidated", EXPORT_MRNRegisterVendorItemConsolidated },
    { "vendorWiseConsolidated", EXPORT_MRNRegisterVendorWiseConsolidated },
    { "ItemWiseConsolidated", EXPORT_MRNRegisterItemConsolidated },
    //{ "mrnqcdetail", EXPORT_MRNRegisterPartyPOSumm},
    { "DAYWISEMRNENTRYLIST", EXPORT_MRNRegisterDayWise1 },
    { "PENDMRNFORQC(SUMMARY)", EXPORT_MRNRegisterPendMRNForQCSummary },
    { "PENDMRNFORQC(DEATIL)", EXPORT_MRNRegisterPendMRNForQCDetail },
    { "Short Excess Register", EXPORT_MRNRegisterShortExcess1 },

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
                "MRNNRegisterReport.xlsx"
            );
        }

        private void EXPORT_MRNRegisterDetail(IXLWorksheet sheet, IList<MRNRegisterDetail> list)
        {
            string[] headers = {
            "#Sr", "Vendor Name", "MRN No", "MRN Date", "Gate No", "G Date", "Entry Date",
            "Invoice No", "Invoice Date", "Doc No", "PO No", "PO Date", "PO Type",
            "Sch No", "Sch Date", "Part Code", "Item Name", "Bill Qty", "Rec Qty", "Accepted Qty",
            "Short/Excess Qty", "Unit", "Rate", "Amount", "Alt Qty", "Alt Unit", "QC Completed",
            "MRN QC Completed", "Need To Check QC", "Batch No", "Unique Batch No", "Supplier Batch No",
            "Shelf Life", "Total Amt", "Net Amount", "Remark", "Actual Entry By Emp", "Actual Entry Date",
            "Last Updated By", "Last Updated Date", "FOC", "Department Name", "Currency", "Rec In Store",
            "MRN Type", "Vendor Address", "Entry By Machine Name"
            };



            for (int i = 0; i < headers.Length; i++)
                sheet.Cell(1, i + 1).Value = headers[i];

            int row = 2, srNo = 1;
            foreach (var item in list)
            {
                sheet.Cell(row, 1).Value = srNo++;
                sheet.Cell(row, 2).Value = item.VendorName;
                sheet.Cell(row, 3).Value = item.MRNNo;
                sheet.Cell(row, 4).Value = item.MRNDate;
                sheet.Cell(row, 5).Value = item.GateNo;
                sheet.Cell(row, 6).Value = item.GDate;
                sheet.Cell(row, 7).Value = item.EntryDate;

                sheet.Cell(row, 8).Value = item.InvoiceNo;
                sheet.Cell(row, 9).Value = item.InvoiceDate;
                sheet.Cell(row, 10).Value = item.DocNo;
                sheet.Cell(row, 11).Value = item.PONo;
                sheet.Cell(row, 12).Value = item.PODate;
                sheet.Cell(row, 13).Value = item.POType;

                sheet.Cell(row, 14).Value = item.SchNo;
                sheet.Cell(row, 15).Value = item.SchDate;
                sheet.Cell(row, 16).Value = item.PartCode;
                sheet.Cell(row, 17).Value = item.ItemName;
                sheet.Cell(row, 18).Value = item.BillQty;
                sheet.Cell(row, 19).Value = item.RecQty;
                sheet.Cell(row, 20).Value = item.AcceptedQty;

                sheet.Cell(row, 21).Value = item.ShortExcessQty;
                sheet.Cell(row, 22).Value = item.Unit;
                sheet.Cell(row, 23).Value = item.Rate;
                sheet.Cell(row, 24).Value = item.Amount;
                sheet.Cell(row, 25).Value = item.AltQty;
                sheet.Cell(row, 26).Value = item.AltUnit;
                sheet.Cell(row, 27).Value = item.QCCompleted;

                sheet.Cell(row, 28).Value = item.MRNQCCompleted;
                sheet.Cell(row, 29).Value = item.NeedTochkQC;
                sheet.Cell(row, 30).Value = item.Batchno;
                sheet.Cell(row, 31).Value = item.Uniquebatchno;
                sheet.Cell(row, 32).Value = item.SupplierBatchNo;
                sheet.Cell(row, 33).Value = item.ShelfLife;
                sheet.Cell(row, 34).Value = item.TotalAmt;

                sheet.Cell(row, 35).Value = item.NetAmout;
                sheet.Cell(row, 36).Value = item.Remark;
                sheet.Cell(row, 37).Value = item.ActualEntryByEmp;
                sheet.Cell(row, 38).Value = item.ActualEntryDate;
                sheet.Cell(row, 39).Value = item.LastUpdatedby;
                sheet.Cell(row, 40).Value = item.LastUpdatedDate;

                sheet.Cell(row, 41).Value = item.FOC;
                sheet.Cell(row, 42).Value = item.DepName;
                sheet.Cell(row, 43).Value = item.Currency;
                sheet.Cell(row, 44).Value = item.RecInStore;
                sheet.Cell(row, 45).Value = item.MRNType;

                sheet.Cell(row, 46).Value = item.VendAddress;
                sheet.Cell(row, 47).Value = item.EntryByMachineName;

                row++;
            }
        }
        private void EXPORT_MRNRegisterVendorItemSumm(IXLWorksheet sheet, IList<MRNRegisterDetail> list)
        {
            string[] headers = {
            "#Sr", "Vendor Name", "MRN No", "MRN Date", "Gate No", "G Date",
    "Invoice No", "Invoice Date", "Doc No",
    "Part Code", "Item Name", "Bill Qty", "Rec Qty", "Short/Excess Qty",
    "Rate", "Unit", "Amount", "Total Amount", "Net Amount", "Remark",
    "Actual Entry By Emp", "Actual Entry Date", "Updated By Emp", "Last Updated Date",
    "FOC", "Department Name", "Currency", "Rec In Store", "MRN Type"
            };



            for (int i = 0; i < headers.Length; i++)
                sheet.Cell(1, i + 1).Value = headers[i];

            int row = 2, srNo = 1;
            foreach (var item in list)
            {
                sheet.Cell(row, 1).Value = srNo++;
                sheet.Cell(row, 2).Value = item.VendorName;
                sheet.Cell(row, 3).Value = item.MRNNo;
                sheet.Cell(row, 4).Value = item.MRNDate;
                sheet.Cell(row, 5).Value = item.GateNo;
                sheet.Cell(row, 6).Value = item.GDate;

                sheet.Cell(row, 7).Value = item.InvoiceNo;
                sheet.Cell(row, 8).Value = item.InvoiceDate;
                sheet.Cell(row, 9).Value = item.DocNo;

                sheet.Cell(row, 10).Value = item.PartCode;
                sheet.Cell(row, 11).Value = item.ItemName;
                sheet.Cell(row, 12).Value = item.BillQty;
                sheet.Cell(row, 13).Value = item.RecQty;
                sheet.Cell(row, 14).Value = item.ShortExcessQty;

                sheet.Cell(row, 15).Value = item.Rate;
                sheet.Cell(row, 16).Value = item.Unit;
                sheet.Cell(row, 17).Value = item.Amount;
                sheet.Cell(row, 18).Value = item.TotalAmt;
                sheet.Cell(row, 19).Value = item.NetAmout;

                sheet.Cell(row, 20).Value = item.Remark;
                sheet.Cell(row, 21).Value = item.ActualEntryByEmp;
                sheet.Cell(row, 22).Value = item.ActualEntryDate;
                sheet.Cell(row, 23).Value = item.UpdatedByEMp;
                sheet.Cell(row, 24).Value = item.LastUpdatedDate;

                sheet.Cell(row, 25).Value = item.FOC;
                sheet.Cell(row, 26).Value = item.DepName;
                sheet.Cell(row, 27).Value = item.Currency;

                sheet.Cell(row, 28).Value = item.RecInStore;
                sheet.Cell(row, 29).Value = item.MRNType;


                row++;
            }
        }
        private void EXPORT_MRNRegisterVendorItemConsolidated(IXLWorksheet sheet, IList<MRNRegisterDetail> list)
        {
            string[] headers = {
                "#Sr", "Vendor Name", "Part Code", "Item Name", "Bill Qty", "Rec Qty", "Short/Excess Qty", "Unit", "Total Amount", "MRN Type"
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
                sheet.Cell(row, 5).Value = item.BillQty;
                sheet.Cell(row, 6).Value = item.RecQty;
                sheet.Cell(row, 7).Value = item.ShortExcessQty;
                sheet.Cell(row, 8).Value = item.Unit;
                sheet.Cell(row, 9).Value = item.TotalAmt;
                sheet.Cell(row, 10).Value = item.MRNType;

                row++;
            }
        }
        private void EXPORT_MRNRegisterVendorWiseConsolidated(IXLWorksheet sheet, IList<MRNRegisterDetail> list)
        {
            string[] headers = {
                "#Sr", "Vendor Name", "Bill Qty", "Rec Qty", "Short/Excess Qty", "Total Amount", "MRN Type"
            };




            for (int i = 0; i < headers.Length; i++)
                sheet.Cell(1, i + 1).Value = headers[i];

            int row = 2, srNo = 1;
            foreach (var item in list)
            {
                sheet.Cell(row, 1).Value = srNo++;
                sheet.Cell(row, 2).Value = item.VendorName;
                sheet.Cell(row, 3).Value = item.BillQty;
                sheet.Cell(row, 4).Value = item.RecQty;
                sheet.Cell(row, 5).Value = item.ShortExcessQty;
                sheet.Cell(row, 6).Value = item.TotalAmt;
                sheet.Cell(row, 7).Value = item.MRNType;

                row++;
            }
        }
        private void EXPORT_MRNRegisterItemConsolidated(IXLWorksheet sheet, IList<MRNRegisterDetail> list)
        {
            string[] headers = {
                "#Sr", "Part Code", "Item Name", "Bill Qty", "Rec Qty", "Short/Excess Qty", "Unit", "Rate", "Total Amount", "MRN Type"
            };

            for (int i = 0; i < headers.Length; i++)
                sheet.Cell(1, i + 1).Value = headers[i];

            int row = 2, srNo = 1;
            foreach (var item in list)
            {
                sheet.Cell(row, 1).Value = srNo++;
                sheet.Cell(row, 2).Value = item.PartCode;
                sheet.Cell(row, 3).Value = item.ItemName;
                sheet.Cell(row, 4).Value = item.BillQty;
                sheet.Cell(row, 5).Value = item.RecQty;
                sheet.Cell(row, 6).Value = item.ShortExcessQty;
                sheet.Cell(row, 7).Value = item.Unit;
                sheet.Cell(row, 8).Value = item.Rate;
                sheet.Cell(row, 9).Value = item.TotalAmt;
                sheet.Cell(row, 10).Value = item.MRNType;


                row++;
            }
        }
        private void EXPORT_MRNRegisterDayWise1(IXLWorksheet sheet, IList<MRNRegisterDetail> list)
        {
            string[] headers = {
                "#Sr", "MRN No", "MRN Date", "Gate No", "Vendor Name", "Invoice No", "Invoice Date", "Doc No",
                "Bill Qty", "Rec Qty", "Currency", "Rec In Store", "MRN Type", "MRN QC Completed", "Need To Check QC",
                "Vendor Address", "Actual Entry By Emp", "Actual Entry Date", "Last Updated By", "Last Updated Date", "Entry By Machine Name"
            };


            for (int i = 0; i < headers.Length; i++)
                sheet.Cell(1, i + 1).Value = headers[i];

            int row = 2, srNo = 1;
            foreach (var item in list)
            {
                sheet.Cell(row, 1).Value = srNo++;
                sheet.Cell(row, 2).Value = item.MRNNo;
                sheet.Cell(row, 3).Value = item.MRNDate;
                sheet.Cell(row, 4).Value = item.GateNo;
                sheet.Cell(row, 5).Value = item.VendorName;
                sheet.Cell(row, 6).Value = item.InvoiceNo;
                sheet.Cell(row, 7).Value = item.InvoiceDate;
                sheet.Cell(row, 8).Value = item.DocNo;
                sheet.Cell(row, 9).Value = item.BillQty;
                sheet.Cell(row, 10).Value = item.RecQty;
                sheet.Cell(row, 11).Value = item.Currency;
                sheet.Cell(row, 12).Value = item.RecInStore;
                sheet.Cell(row, 13).Value = item.MRNType;
                sheet.Cell(row, 14).Value = item.MRNQCCompleted;
                sheet.Cell(row, 15).Value = item.NeedTochkQC;
                sheet.Cell(row, 16).Value = item.VendAddress;
                sheet.Cell(row, 17).Value = item.ActualEntryByEmp;
                sheet.Cell(row, 18).Value = item.ActualEntryDate;
                sheet.Cell(row, 19).Value = item.LastUpdatedby;
                sheet.Cell(row, 20).Value = item.LastUpdatedDate;
                sheet.Cell(row, 21).Value = item.EntryByMachineName;



                row++;
            }
        }
         private void EXPORT_MRNRegisterPendMRNForQCSummary(IXLWorksheet sheet, IList<MRNRegisterDetail> list)
         {
            string[] headers = {
                "#Sr", "Vendor Name", "MRN No", "MRN Date", "Gate No", "G Date",
                "Invoice No", "Invoice Date", "Total Bill Qty", "Rec Qty", "Total Amount", "Purchase Bill Posted", "MRN Type"
            };



            for (int i = 0; i < headers.Length; i++)
                sheet.Cell(1, i + 1).Value = headers[i];

            int row = 2, srNo = 1;
            foreach (var item in list)
            {
                sheet.Cell(row, 1).Value = srNo++;
                sheet.Cell(row, 2).Value = item.VendorName;
                sheet.Cell(row, 3).Value = item.MRNNo;
                sheet.Cell(row, 4).Value = item.MRNDate;
                sheet.Cell(row, 5).Value = item.GateNo;
                sheet.Cell(row, 6).Value = item.GDate;
                sheet.Cell(row, 7).Value = item.InvoiceNo;
                sheet.Cell(row, 8).Value = item.InvoiceDate;
                sheet.Cell(row, 9).Value = item.TotalBillQty;
                sheet.Cell(row, 10).Value = item.RecQty;
                sheet.Cell(row, 11).Value = item.TotalAmt;
                sheet.Cell(row, 12).Value = item.PurchaseBillPosted;
                sheet.Cell(row, 13).Value = item.MRNType;




                row++;
            }
         }
        private void EXPORT_MRNRegisterPendMRNForQCDetail(IXLWorksheet sheet, IList<MRNRegisterDetail> list)
         {
            string[] headers = {
                "#Sr", "Vendor Name", "MRN No", "MRN Date", "Gate No", "G Date",
                "Invoice No", "Invoice Date", "Part Code", "Item Name",
                "Total Bill Qty", "Rec Qty", "Rate", "Total Amount",
                "PO No", "PO Date", "PO Type", "Sch No", "Sch Date",
                "QC Status", "Purchase Bill Posted"
            };




            for (int i = 0; i < headers.Length; i++)
                sheet.Cell(1, i + 1).Value = headers[i];

            int row = 2, srNo = 1;
            foreach (var item in list)
            {
                sheet.Cell(row, 1).Value = srNo++;
                sheet.Cell(row, 2).Value = item.VendorName;
                sheet.Cell(row, 3).Value = item.MRNNo;
                sheet.Cell(row, 4).Value = item.MRNDate;
                sheet.Cell(row, 5).Value = item.GateNo;
                sheet.Cell(row, 6).Value = item.GDate;
                sheet.Cell(row, 7).Value = item.InvoiceNo;
                sheet.Cell(row, 8).Value = item.InvoiceDate;
                sheet.Cell(row, 9).Value = item.PartCode;
                sheet.Cell(row, 10).Value = item.ItemName;
                sheet.Cell(row, 11).Value = item.TotalBillQty;
                sheet.Cell(row, 12).Value = item.RecQty;
                sheet.Cell(row, 13).Value = item.Rate;
                sheet.Cell(row, 14).Value = item.TotalAmt;
                sheet.Cell(row, 15).Value = item.PONo;
                sheet.Cell(row, 16).Value = item.PODate;
                sheet.Cell(row, 17).Value = item.POType;
                sheet.Cell(row, 18).Value = item.SchNo;
                sheet.Cell(row, 19).Value = item.SchDate;
                sheet.Cell(row, 20).Value = item.QCCompleted;
                sheet.Cell(row, 21).Value = item.PurchaseBillPosted;


                row++;
            }
        }
        private void EXPORT_MRNRegisterShortExcess1(IXLWorksheet sheet, IList<MRNRegisterDetail> list)
         {
            string[] headers = {
                "#Sr", "MRN No", "MRN Date", "MRN Type", "Gate No", "G Date", "Invoice No", "Invoice Date",
                "Account Name", "PO No", "PO Date", "Sch No", "Part Code", "Item Name", "Bill Qty",
                "Rec Qty", "Short/Excess Qty", "Unit", "Rate", "Total Amount", "Net Amount", "Currency",
                "Department Name", "Alt Qty", "Alt Unit", "Batch No", "Unique Batch No"
            };





            for (int i = 0; i < headers.Length; i++)
                sheet.Cell(1, i + 1).Value = headers[i];

            int row = 2, srNo = 1;
            foreach (var item in list)
            {
                sheet.Cell(row, 1).Value = srNo++;
                sheet.Cell(row, 2).Value = item.MRNNo;
                sheet.Cell(row, 3).Value = item.MRNDate;
                sheet.Cell(row, 4).Value = item.MRNType;
                sheet.Cell(row, 5).Value = item.GateNo;
                sheet.Cell(row, 6).Value = item.GDate;
                sheet.Cell(row, 7).Value = item.InvoiceNo;
                sheet.Cell(row, 8).Value = item.InvoiceDate;
                sheet.Cell(row, 9).Value = item.Account_Name;
                sheet.Cell(row, 10).Value = item.PONo;
                sheet.Cell(row, 11).Value = item.PODate;
                sheet.Cell(row, 12).Value = item.SchNo;
                sheet.Cell(row, 13).Value = item.PartCode;
                sheet.Cell(row, 14).Value = item.ItemName;
                sheet.Cell(row, 15).Value = item.BillQty;
                sheet.Cell(row, 16).Value = item.RecQty;
                sheet.Cell(row, 17).Value = item.ShortExcessQty;
                sheet.Cell(row, 18).Value = item.Unit;
                sheet.Cell(row, 19).Value = item.Rate;
                sheet.Cell(row, 20).Value = item.TotalAmt;
                sheet.Cell(row, 21).Value = item.NetAmout;
                sheet.Cell(row, 22).Value = item.Currency;
                sheet.Cell(row, 23).Value = item.DepName;
                sheet.Cell(row, 24).Value = item.AltQty;
                sheet.Cell(row, 25).Value = item.AltUnit;
                sheet.Cell(row, 26).Value = item.Batchno;
                sheet.Cell(row, 27).Value = item.Uniquebatchno;

                row++;
            }
        }

        public async Task<JsonResult> FillGateNo(string FromDate, string ToDate)
        {
            var JSON = await _IMRNRegister.FillGateNo(FromDate, ToDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillDocument(string FromDate, string ToDate)
        {

            var JSON = await _IMRNRegister.FillDocument(FromDate, ToDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillVendor(string FromDate, string ToDate)
        {
            var JSON = await _IMRNRegister.FillVendor(FromDate, ToDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillInvoice(string FromDate, string ToDate)
        {
            var JSON = await _IMRNRegister.FillInvoice(FromDate, ToDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        } 
        public async Task<JsonResult> FillItemNamePartcode(string FromDate, string ToDate)
        {
            var JSON = await _IMRNRegister.FillItemNamePartcode(FromDate, ToDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillPONO(string FromDate, string ToDate)
        {
            var JSON = await _IMRNRegister.FillPONO(FromDate, ToDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillSchNo(string FromDate, string ToDate)
        {
            var JSON = await _IMRNRegister.FillSchNo(  FromDate,  ToDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }

        public async Task<JsonResult> FillMRNNo(string FromDate, string ToDate)
        {
            var JSON = await _IMRNRegister.FillMRNNo(FromDate, ToDate);
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
        [HttpGet]
        public IActionResult GetMRNRegisterData()
        {
            string modelJson = HttpContext.Session.GetString("KeyMRNList");
            List<MRNRegisterDetail> stockRegisterList = new List<MRNRegisterDetail>();
            if (!string.IsNullOrEmpty(modelJson))
            {
                stockRegisterList = JsonConvert.DeserializeObject<List<MRNRegisterDetail>>(modelJson);
            }

            return Json(stockRegisterList);
        }
    }
}
