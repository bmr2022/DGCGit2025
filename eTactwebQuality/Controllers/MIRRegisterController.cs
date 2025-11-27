using eTactWeb.Data.Common;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using eTactWeb.DOM.Models;
using System.Net;
using System.Data;
using System.Globalization;
using ClosedXML.Excel;

namespace eTactWeb.Controllers
{
    public class MIRRegisterController : Controller
    {
        private readonly IDataLogic _IDataLogic;
        public IMIRRegister _IMIRRegister { get; }

        private readonly ILogger<MIRRegisterController> _logger;
        private readonly IConfiguration iconfiguration;
        private readonly IMemoryCache _MemoryCache;
        public IWebHostEnvironment _IWebHostEnvironment { get; }

        public MIRRegisterController(ILogger<MIRRegisterController> logger, IDataLogic iDataLogic, IMIRRegister iMIRRegister, IMemoryCache iMemoryCache, EncryptDecrypt encryptDecrypt, IWebHostEnvironment iWebHostEnvironment, IConfiguration iconfiguration)
        {
            _logger = logger;
            _IDataLogic = iDataLogic;
            _IMIRRegister = iMIRRegister;
            _MemoryCache = iMemoryCache;
            _IWebHostEnvironment = iWebHostEnvironment;
            this.iconfiguration = iconfiguration;
        }
        [Route("{controller}/Index")]
        public IActionResult MIRRegister()
        {
            var model = new MIRRegisterModel();
            model.MIRRegisterDetail = new List<MIRRegisterDetail>();
            return View(model);
        }
        public async Task<IActionResult> GetRegisterData(string MRNType, string ReportType, string FromDate, string ToDate, string gateno,string MRNno, string docname, string PONo, string Schno, string PartCode, string ItemName, string invoiceNo, string VendorName,string MRNStatus,string MIRNo, int pageNumber = 1, int pageSize = 50, string SearchBox = "")
        {
            var model = new MIRRegisterModel();
            if (string.IsNullOrEmpty(gateno)||gateno == "0" )
                {                gateno = "";            }
            if (string.IsNullOrEmpty(MRNno) || MRNno == "0")
            { MRNno = ""; }
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
       
            model = await _IMIRRegister.GetRegisterData(MRNType,ReportType,  FromDate,  ToDate,  gateno,  MRNno,MIRNo,  PONo,  Schno,  PartCode,  ItemName,  invoiceNo,  VendorName,MRNStatus);
          
            model.ReportMode= ReportType;
            var modelList = model?.MIRRegisterDetail ?? new List<MIRRegisterDetail>();


            if (string.IsNullOrWhiteSpace(SearchBox))
            {
                model.TotalRecords = modelList.Count();
                model.PageNumber = pageNumber;
                model.PageSize = pageSize;
                model.MIRRegisterDetail = modelList
                .Skip((pageNumber - 1) * pageSize)
                   .Take(pageSize)
                   .ToList();
            }
            else
            {
                List<MIRRegisterDetail> filteredResults;
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
                model.MIRRegisterDetail = filteredResults.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
                model.PageNumber = pageNumber;
                model.PageSize = pageSize;
            }
            MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                SlidingExpiration = TimeSpan.FromMinutes(55),
                Size = 1024,
            };

            _MemoryCache.Set("KeyMIRRegisterList", modelList, cacheEntryOptions);
            return PartialView("_MIRRegisterGrid", model);
        }
        [HttpGet]
        public IActionResult GlobalSearch(string searchString, string dashboardType = "Summary", int pageNumber = 1, int pageSize = 50)
        {
            MIRRegisterModel model = new MIRRegisterModel();
            model.ReportType = dashboardType;
            if (string.IsNullOrWhiteSpace(searchString))
            {
                return PartialView("_MIRRegisterGrid", new List<MIRRegisterDetail>());
            }
            //string cacheKey = $"KeyProdList_{dashboardType}";
            if (!_MemoryCache.TryGetValue("KeyMIRRegisterList", out IList<MIRRegisterDetail> mIRRegisterDetail) || mIRRegisterDetail == null)
            {
                return PartialView("_MIRRegisterGrid", new List<MIRRegisterDetail>());
            }

            List<MIRRegisterDetail> filteredResults;

            if (string.IsNullOrWhiteSpace(searchString))
            {
                filteredResults = mIRRegisterDetail.ToList();
            }
            else
            {
                filteredResults = mIRRegisterDetail
                    .Where(i => i.GetType().GetProperties()
                        .Where(p => p.PropertyType == typeof(string))
                        .Select(p => p.GetValue(i)?.ToString())
                        .Any(value => !string.IsNullOrEmpty(value) &&
                                      value.Contains(searchString, StringComparison.OrdinalIgnoreCase)))
                    .ToList();


                if (filteredResults.Count == 0)
                {
                    filteredResults = mIRRegisterDetail.ToList();
                }
            }

            model.TotalRecords = filteredResults.Count;
            model.MIRRegisterDetail = filteredResults.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            model.PageNumber = pageNumber;
            model.PageSize = pageSize;
            if (model.ReportMode == "vendorItemWiseSummary")
            {
                return PartialView("_MIRRegisterVendorItemWiseSummary", model);
            }
            else if (model.ReportMode == "POBATCHWISEDETAIL")
            {
                return PartialView("_MRNRegisterVendorItemConsolidated", model);
            }
            else if (model.ReportMode == "PPMRating")
            {
                return PartialView("_MIRRegisterPPM", model);
            }
            else if (model.ReportMode == "vendorWiseConsolidated")
            {
                return PartialView("_MRNRegisterVendorWiseConsolidated", model);
            }
            else if (model.ReportMode == "DAYWISEMIRENTRYLIST")
            {
                return PartialView("_MRNRegisterDayWiseList", model);
            }
            else if (model.ReportMode == "PENDMRNFORQC(SUMMARY)")
            {
                return PartialView("_MIRRegisterPendMRNForQCSummary", model);
            }
            else if (model.ReportMode == "PENDMRNFORQC(Detail)")
            {
                return PartialView("_MIRRegisterPendMRNForQCDetail", model);
            }
            else if (model.ReportMode == "vendorItemRejectionSummary")
            {
                return PartialView("_MIRRegisterVendorItemRejectionSummary", model);
            }
            else if (model.ReportMode == "ItemWiseSummary")
            {
                return PartialView("_MIRRegisterItemWiseSummary", model);
            }
            else if (model.ReportMode == "MRNWiseSummary")
            {
                return PartialView("_MRNWiseSummary", model);
            }
            else
            {
                return PartialView("_MIRRegisterGrid", model);
            }
        }
        public async Task<IActionResult> ExportMIRToExcel(string ReportType)
        {
            //string modelJson = HttpContext.Session.GetString("KeyPOList");
            if (!_MemoryCache.TryGetValue("KeyMIRRegisterList", out List<MIRRegisterDetail> modelList))
            {
                return NotFound("No data available to export.");
            }


            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("PO Register");

            var reportGenerators = new Dictionary<string, Action<IXLWorksheet, IList<MIRRegisterDetail>>>
            {
                { "vendorItemWiseSummary", EXPORT_MIRRegisterVendorItemWiseSummary },
        { "POBATCHWISEDETAIL", EXPORT_MRNRegisterVendorItemConsolidated},
        { "PPMRating", EXPORT_MIRRegisterPPM },
        { "vendorWiseConsolidated", EXPORT_MRNRegisterVendorWiseConsolidated },
        { "DAYWISEMIRENTRYLIST",EXPORT_MRNRegisterDayWiseList},
        { "PENDMRNFORQC(SUMMARY)", EXPORT_MIRRegisterPendMRNForQCSummary },
        { "PENDMRNFORQC(Detail)", EXPORT_MIRRegisterPendMRNForQCDetail },
        { "vendorItemRejectionSummary", EXPORT_MIRRegisterVendorItemRejectionSummary},
        { "ItemWiseSummary", EXPORT_MIRRegisterItemWiseSummary },
        { "MRNWiseSummary", EXPORT_MRNWiseSummary },
        { "DETAIL", EXPORT_MRNDETAIL }
                    
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
                "MIRRegister.xlsx"
            );
        }
        private void EXPORT_MIRRegisterVendorItemWiseSummary(IXLWorksheet sheet, IList<MIRRegisterDetail> list)
        {
            string[] headers = {
                "#Sr", "Vendor Name", "Item Name", "Part Code", "MRN No", "MRN Date",
                "Gate No", "Gate Date", "Rate", "Amount",
                "Bill Qty", "Received Qty", "Accepted Qty", "Rejected Qty",
                "Rework Qty", "Hold Qty", "Deviation Qty", "Unit"
            };



            for (int i = 0; i < headers.Length; i++)
                sheet.Cell(1, i + 1).Value = headers[i];

            int row = 2, srNo = 1;
            foreach (var item in list)
            {
                sheet.Cell(row, 1).Value = srNo++;
                sheet.Cell(row, 2).Value = item.VendorName;
                sheet.Cell(row, 3).Value = item.ItemName;
                sheet.Cell(row, 4).Value = item.PartCode;
                sheet.Cell(row, 5).Value = item.MRNNo;
                sheet.Cell(row, 6).Value = item.MRNDate;
                sheet.Cell(row, 7).Value = item.GateNo;
                sheet.Cell(row, 8).Value = item.GateDate;
                sheet.Cell(row, 9).Value = item.Rate;
                sheet.Cell(row, 10).Value = item.Amount;
                sheet.Cell(row, 11).Value = item.BillQty;
                sheet.Cell(row, 12).Value = item.RecQty;
                sheet.Cell(row, 13).Value = item.AcceptedQty;
                sheet.Cell(row, 14).Value = item.RejectedQty;
                sheet.Cell(row, 15).Value = item.Reworkqty;
                sheet.Cell(row, 16).Value = item.HoldQty;
                sheet.Cell(row, 17).Value = item.DeviationQty;
                sheet.Cell(row, 18).Value = item.Unit;
                row++;
            }
        }
        private void EXPORT_MRNRegisterVendorItemConsolidated(IXLWorksheet sheet, IList<MIRRegisterDetail> list)
        {
            string[] headers = {
                 "#Sr", "Vendor Name", "Part Code", "Item Name",
    "Bill Qty", "Received Qty", "Unit", "Total Amount", "MRN Type"
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
                sheet.Cell(row, 7).Value = item.Unit;
                sheet.Cell(row, 8).Value = item.TotalAmt;
                sheet.Cell(row, 9).Value = item.MRNType;
                row++;
            }
        }
        private void EXPORT_MIRRegisterPPM(IXLWorksheet sheet, IList<MIRRegisterDetail> list)
        {
            string[] headers = {
                "#Sr", "Vendor Name","Bill Qty", "Received Qty", "Total Amt", "MRN Type"
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
                sheet.Cell(row, 5).Value = item.TotalAmt;
                sheet.Cell(row, 6).Value = item.MRNType;
               
                row++;
            }
        }
        private void EXPORT_MRNRegisterVendorWiseConsolidated(IXLWorksheet sheet, IList<MIRRegisterDetail> list)
        {
            string[] headers = {
                "#Sr", "Vendor Name","Bill Qty", "Received Qty", "ShortExcessQty","Total Amt", "MRN Type"
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
        private void EXPORT_MRNRegisterDayWiseList(IXLWorksheet sheet, IList<MIRRegisterDetail> list)
        {
            string[] headers = {
                "#Sr", "MRN No", "MRN Date", "Gate No", "Gate Date", "Vendor Name",
    "Invoice No", "Invoice Date", "Doc No", "Bill Qty", "Received Qty",
    "Currency", "Received In Store", "MRN Type", "MRN QC Completed",
    "Need To Check QC", "Vendor Address", "Actual Entry By", "Actual Entry Date",
    "Last Updated By", "Last Updated Date", "Entry By Machine Name"
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
                sheet.Cell(row, 5).Value = item.GateDate;
                sheet.Cell(row, 6).Value = item.VendorName;
                sheet.Cell(row, 7).Value = item.InvoiceNo;
                sheet.Cell(row, 8).Value = item.InvoiceDate;
                sheet.Cell(row, 9).Value = item.DocNo;
                sheet.Cell(row, 10).Value = item.BillQty;
                sheet.Cell(row, 11).Value = item.RecQty;
                sheet.Cell(row, 12).Value = item.Currency;
                sheet.Cell(row, 13).Value = item.RecInStore;
                sheet.Cell(row, 14).Value = item.MRNType;
                sheet.Cell(row, 15).Value = item.MRNQCCompleted;
                sheet.Cell(row, 16).Value = item.NeedTochkQC;
                sheet.Cell(row, 17).Value = item.VendAddress;
                sheet.Cell(row, 18).Value = item.ActualEntryByEmp;
                sheet.Cell(row, 19).Value = item.ActualEntryDate;
                sheet.Cell(row, 20).Value = item.LastUpdatedby;
                sheet.Cell(row, 21).Value = item.LastUpdatedDate;
                sheet.Cell(row, 22).Value = item.EntryByMachineName;
                row++;
            }
        }
        private void EXPORT_MIRRegisterPendMRNForQCSummary(IXLWorksheet sheet, IList<MIRRegisterDetail> list)
        {
            string[] headers = {
                "#Sr", "Vendor Name", "MRN No", "MRN Date", "Gate No", "Gate Date",
                "Invoice No", "Invoice Date", "Total Bill Qty", "Received Qty",
                "Total Amount", "Purchase Bill Posted", "MRN Type"
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
                sheet.Cell(row, 6).Value = item.GateDate;
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
        private void EXPORT_MIRRegisterPendMRNForQCDetail(IXLWorksheet sheet, IList<MIRRegisterDetail> list)
        {
            string[] headers = {
                "#Sr", "Vendor Name", "MRN No", "MRN Date", "Gate No", "Gate Date",
                "Invoice No", "Invoice Date", "Part Code", "Item Name","Total Bill Qty", "Received Qty", "Rate", "Total Amount",
                "PO No", "PO Date", "PO Type","Schedule No", "Schedule Date","QC Completed", "Purchase Bill Posted"
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
                sheet.Cell(row, 6).Value = item.GateDate;
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
        private void EXPORT_MIRRegisterVendorItemRejectionSummary(IXLWorksheet sheet, IList<MIRRegisterDetail> list)
        {
            string[] headers = {
                "#Sr", "Vendor Name", "Item Name", "Part Code",
    "Bill Qty", "Received Qty", "Accepted Qty", "Rejected Qty",
    "MRN No", "MRN Date", "Invoice No", "Invoice Date",
    "Rate", "Amount", "Remark", "Rework Qty", "Hold Qty", "Unit"
            };

            for (int i = 0; i < headers.Length; i++)
                sheet.Cell(1, i + 1).Value = headers[i];

            int row = 2, srNo = 1;
            foreach (var item in list)
            {
                sheet.Cell(row, 1).Value = srNo++;
                sheet.Cell(row, 2).Value = item.VendorName;
                sheet.Cell(row, 3).Value = item.ItemName;
                sheet.Cell(row, 4).Value = item.PartCode;
                sheet.Cell(row, 5).Value = item.BillQty;
                sheet.Cell(row, 6).Value = item.RecQty;
                sheet.Cell(row, 7).Value = item.AcceptedQty;
                sheet.Cell(row, 8).Value = item.RejectedQty;
                sheet.Cell(row, 9).Value = item.MRNNo;
                sheet.Cell(row, 10).Value = item.MRNDate;
                sheet.Cell(row, 11).Value = item.InvoiceNo;
                sheet.Cell(row, 12).Value = item.InvoiceDate;
                sheet.Cell(row, 13).Value = item.Rate;
                sheet.Cell(row, 14).Value = item.Amount;
                sheet.Cell(row, 15).Value = item.Remark;
                sheet.Cell(row, 16).Value = item.Reworkqty;
                sheet.Cell(row, 17).Value = item.HoldQty;
                sheet.Cell(row, 18).Value = item.Unit;
                row++;
            }
        }
         private void EXPORT_MIRRegisterItemWiseSummary(IXLWorksheet sheet, IList<MIRRegisterDetail> list)
        {
            string[] headers = {
             "#Sr", "Vendor Name", "Item Name", "Part Code", "MRN No",
    "Bill Qty", "Received Qty", "Accepted Qty", "Rejected Qty",
    "Remark", "Rework Qty", "Hold Qty", "Unit"
            };

            for (int i = 0; i < headers.Length; i++)
                sheet.Cell(1, i + 1).Value = headers[i];

            int row = 2, srNo = 1;
            foreach (var item in list)
            {
                sheet.Cell(row, 1).Value = srNo++;
                sheet.Cell(row, 2).Value = item.VendorName;
                sheet.Cell(row, 3).Value = item.ItemName;
                sheet.Cell(row, 4).Value = item.PartCode;
                sheet.Cell(row, 5).Value = item.MRNNo;
                sheet.Cell(row, 6).Value = item.BillQty;
                sheet.Cell(row, 7).Value = item.RecQty;
                sheet.Cell(row, 8).Value = item.AcceptedQty;
                sheet.Cell(row, 9).Value = item.RejectedQty;
                sheet.Cell(row, 10).Value = item.Remark;
                sheet.Cell(row, 11).Value = item.Reworkqty;
                sheet.Cell(row, 12).Value = item.HoldQty;
                sheet.Cell(row, 13).Value = item.Unit;
                row++;
            }
        }
         private void EXPORT_MRNWiseSummary(IXLWorksheet sheet, IList<MIRRegisterDetail> list)
        {
            string[] headers = {
            "#Sr", "Vendor Name", "OK MRN", "Rejected MRN"
            };

            for (int i = 0; i < headers.Length; i++)
                sheet.Cell(1, i + 1).Value = headers[i];

            int row = 2, srNo = 1;
            foreach (var item in list)
            {
                sheet.Cell(row, 1).Value = srNo++;
                sheet.Cell(row, 2).Value = item.VendorName;
                sheet.Cell(row, 3).Value = item.OK_MRN;
                sheet.Cell(row, 4).Value = item.Rej_MRN;
                row++;
            }
        }
        private void EXPORT_MRNDETAIL(IXLWorksheet sheet, IList<MIRRegisterDetail> list)
        {
            string[] headers = {
            "#Sr","Vendor Name", "MRN No", "MRN Date", "Gate No", "Entry Date",
    "Invoice No", "Invoice Date", "Part Code", "Item Name",
    "Bill Qty", "Received Qty", "Unit", "Rate", "Amount",
    "Alt Accepted Qty", "Alt Unit", "Total Amount", "Net Amount", "Remark",
    "Actual Entry By", "Actual Entry Date", "Last Updated By", "Last Updated Date",
    "Received In Store", "MRN Type", "Vendor Address", "Entry By Machine Name"
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
                sheet.Cell(row, 6).Value = item.EntryDate;
                sheet.Cell(row, 7).Value = item.InvoiceNo;
                sheet.Cell(row, 8).Value = item.InvoiceDate;
                sheet.Cell(row, 9).Value = item.PartCode;
                sheet.Cell(row, 10).Value = item.ItemName;
                sheet.Cell(row, 11).Value = item.BillQty;
                sheet.Cell(row, 12).Value = item.RecQty;
                sheet.Cell(row, 13).Value = item.Unit;
                sheet.Cell(row, 14).Value = item.Rate;
                sheet.Cell(row, 15).Value = item.Amount;
                sheet.Cell(row, 16).Value = item.AltAcceptedQty;
                sheet.Cell(row, 17).Value = item.AltUnit;
                sheet.Cell(row, 18).Value = item.TotalAmt;
                sheet.Cell(row, 19).Value = item.NetAmout;
                sheet.Cell(row, 20).Value = item.Remark;
                sheet.Cell(row, 21).Value = item.ActualEntryByEmp;
                sheet.Cell(row, 22).Value = item.ActualEntryDate;
                sheet.Cell(row, 23).Value = item.LastUpdatedby;
                sheet.Cell(row, 24).Value = item.LastUpdatedDate;
                sheet.Cell(row, 25).Value = item.RecInStore;
                sheet.Cell(row, 26).Value = item.MRNType;
                sheet.Cell(row, 27).Value = item.VendAddress;
                sheet.Cell(row, 28).Value = item.EntryByMachineName;
                row++;
            }
        }

        public async Task<JsonResult> FillGateNo(string FromDate, string ToDate)
        {
            var JSON = await _IMIRRegister.FillGateNo(FromDate, ToDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillMRNNo(string FromDate, string ToDate)
        {
            var JSON = await _IMIRRegister.FillMRNNo(FromDate, ToDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillMIRNo(string FromDate, string ToDate)
        {

            var JSON = await _IMIRRegister.FillMIRNo(FromDate, ToDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillVendor(string FromDate, string ToDate)
        {
            var JSON = await _IMIRRegister.FillVendor(FromDate, ToDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillInvoice(string FromDate, string ToDate)
        {
            var JSON = await _IMIRRegister.FillInvoice(FromDate, ToDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        } 
        public async Task<JsonResult> FillItemName(string FromDate, string ToDate)
        {
            var JSON = await _IMIRRegister.FillItemName(FromDate, ToDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillItemPartcode(string FromDate, string ToDate)
        {
            var JSON = await _IMIRRegister.FillItemPartcode(FromDate, ToDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillPONO(string FromDate, string ToDate)
        {
            var JSON = await _IMIRRegister.FillPONO(FromDate, ToDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillSchNo(string FromDate, string ToDate)
        {
            var JSON = await _IMIRRegister.FillSchNo(  FromDate,  ToDate);
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
