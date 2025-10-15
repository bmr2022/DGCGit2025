using ClosedXML.Excel;
using DocumentFormat.OpenXml.Wordprocessing;
using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.DOM.Models.Master;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Runtime.Caching;

namespace eTactWeb.Controllers
{
    public class VendJWRegisterController : Controller
    {
        private readonly IDataLogic _IDataLogic;
        public IVendJWRegister _IVendJWRegister { get; }

        private readonly ILogger<VendJWRegisterController> _logger;
        private readonly IConfiguration iconfiguration;
        public IWebHostEnvironment _IWebHostEnvironment { get; }
        private readonly IMemoryCache _MemoryCache;
        public VendJWRegisterController(ILogger<VendJWRegisterController> logger, IDataLogic iDataLogic, EncryptDecrypt encryptDecrypt, IWebHostEnvironment iWebHostEnvironment, IConfiguration iconfiguration, IVendJWRegister VendJWRegister, IMemoryCache iMemoryCache)
        {
            _logger = logger;
            _IDataLogic = iDataLogic;
            _IVendJWRegister = VendJWRegister;
            _IWebHostEnvironment = iWebHostEnvironment;
            this.iconfiguration = iconfiguration;
            _MemoryCache = iMemoryCache;
        }
        [Route("{controller}/Index")]
        public IActionResult VendJWRegister()
        {
            var model = new VendJWRegisterModel();
            model.VendJWRegisterDetails = new List<VendJWRegisterDetail>();
            return View(model);
        }

        public async Task<IActionResult> GetJWRegisterData(string FromDate, string ToDate,string RecChallanNo,string IssChallanNo, string PartyName, string PartCode, string ItemName, string IssueChallanType, string ReportMode, int pageNumber = 1, int pageSize = 50, string SearchBox = "")
        {
            var YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
            var model = new VendJWRegisterModel();
            model = await _IVendJWRegister.GetJWRegisterData(FromDate, ToDate,RecChallanNo,IssChallanNo, PartyName,PartCode, ItemName, IssueChallanType, ReportMode);
            model.ReportMode = ReportMode;
            model.IssueChallanType = IssueChallanType;
            var modelList = model?.VendJWRegisterDetails ?? new List<VendJWRegisterDetail>();


            if (string.IsNullOrWhiteSpace(SearchBox))
            {
                model.TotalRecords = modelList.Count();
                model.PageNumber = pageNumber;
                model.PageSize = pageSize;
                model.VendJWRegisterDetails = modelList
                .Skip((pageNumber - 1) * pageSize)
                   .Take(pageSize)
                   .ToList();
            }
            else
            {
                List<VendJWRegisterDetail> filteredResults;
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
                model.VendJWRegisterDetails = filteredResults.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
                model.PageNumber = pageNumber;
                model.PageSize = pageSize;
            }
            MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                SlidingExpiration = TimeSpan.FromMinutes(55),
                Size = 1024,
            };

            _MemoryCache.Set("KeyVendJWList", modelList, cacheEntryOptions);
            if (IssueChallanType == "ISSUE")
            {
                return PartialView("_VenderJWIssueChallanGrid", model);
            }
            else if(IssueChallanType == "REC")
            {
                return PartialView("_VenderJWReceiveGrid", model);
            }
   //         else if (IssueChallanType == "RECO" && ReportMode== "RECO SUMMARY")
   //         {
			//	return PartialView("_VendorRecoSummaryGrid", model);
			//}

			else
            {
                return PartialView("_VendorRecoIssueReceiveGrid", model);
            }
        }
        [HttpGet]
        public IActionResult GlobalSearch(string searchString,string IssueChallanType,  string dashboardType = "JOBWORKReceiveSUMMARY", int pageNumber = 1, int pageSize = 50)
        {
            VendJWRegisterModel model = new VendJWRegisterModel();
            model.ReportMode = dashboardType;
            if (string.IsNullOrWhiteSpace(searchString))
            {
                //return PartialView("_PORegisterGrid", new List<PORegisterDetail>());
                if (IssueChallanType == "ISSUE")
                {
                    return PartialView("_VenderJWIssueChallanGrid", new List<VendJWRegisterModel>());
                }
                else if (IssueChallanType == "REC")
                {
                    return PartialView("_VenderJWReceiveGrid", new List<VendJWRegisterModel>());
                }
                else
                {
                    return PartialView("_VendorRecoIssueReceiveGrid",  new List<VendJWRegisterModel>());
                }
            }
            //string cacheKey = $"KeyProdList_{dashboardType}";
            if (!_MemoryCache.TryGetValue("KeyVendJWList", out IList<VendJWRegisterDetail> vendJWRegisterDetail) || vendJWRegisterDetail == null)
            {
                if (IssueChallanType == "ISSUE")
                {
                    return PartialView("_VenderJWIssueChallanGrid", new List<VendJWRegisterModel>());
                }
                else if (IssueChallanType == "REC")
                {
                    return PartialView("_VenderJWReceiveGrid", new List<VendJWRegisterModel>());
                }
                else
                {
                    return PartialView("_VendorRecoIssueReceiveGrid", new List<VendJWRegisterModel>());
                }
            }

            List<VendJWRegisterDetail> filteredResults;

            if (string.IsNullOrWhiteSpace(searchString))
            {
                filteredResults = vendJWRegisterDetail.ToList();
            }
            else
            {
                filteredResults = vendJWRegisterDetail
                    .Where(i => i.GetType().GetProperties()
                        .Where(p => p.PropertyType == typeof(string))
                        .Select(p => p.GetValue(i)?.ToString())
                        .Any(value => !string.IsNullOrEmpty(value) &&
                                      value.Contains(searchString, StringComparison.OrdinalIgnoreCase)))
                    .ToList();


                if (filteredResults.Count == 0)
                {
                    filteredResults = vendJWRegisterDetail.ToList();
                }
            }

            model.TotalRecords = filteredResults.Count;
            model.VendJWRegisterDetails = filteredResults.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            model.PageNumber = pageNumber;
            model.PageSize = pageSize;
            //receive
            if (model.ReportMode == "JOBWORKReceiveSUMMARY")
            {
                return PartialView("_JWReceiveSummaryReport", model);
            }
            else if (model.ReportMode == "JOBWORKReceiveWithAdjustemntDETAIL")
            {
                return PartialView("_JWReceiveAdjustmentDetailReport", model);
            }
            else if (model.ReportMode == "JOBWORKReceiveItemWiseSUMM")
            {
                return PartialView("_JWReceiveItemWiseSumReport", model);
            } 
            else if (model.ReportMode == "JOBWORKReceiveDETAIL")
            {
                return PartialView("_VenderJWReceiveGrid", model);
            }
            //reco
            else if (model.ReportMode == "JOBWORKRecoDETAIL")
            {
                return PartialView("_JWRecoIssueReceiveSummaryReport", model);
            }
            else if (model.ReportMode == "JOBWORKRecoSummary")
            {
                return PartialView("_VendorRecoIssueReceiveGrid", model);
            }
            else if (model.ReportMode == "ONLY PENDING CHALLAN")
            {
                return PartialView("_ONLYPENDINGCHALLAN", model);
            }

            //issue
            else if (model.ReportMode == "JOBWORKISSUECHALLANITEMDETAIL")
            {
                return PartialView("_JWIssueChallanDetalGridReport", model);
            }
            else if (model.ReportMode == "JOBWORKISSUEITEMDETAIL")
            {
                return PartialView("_JWIssueItemDetailReport", model);
            }
            else if (model.ReportMode == "JOBWORKISSUEPatyITEMDETAIL")
            {
                return PartialView("_JWIssuePartyItemDetailReport", model);
            }
            else if (model.ReportMode == "JOBWORKISSUECHALLANSUMMARY")
            {
                return PartialView("_VenderJWIssueChallanGrid", model);
            }
            else if (model.ReportMode == "JOBWORK ISSUECHALLAN CONSOLIDATED")
            {
                return PartialView("_JOBWORKISSUECHALLANCONSOLIDATED", model);
            }
            else if (model.ReportMode == "JOBWORK RECEIVE CONSOLIDATED")
            {
                return PartialView("_JOBWORKRECEIVECONSOLIDATED", model);
            }

            else
            {
                return PartialView("_VenderJWReceiveGrid", model);
            }
        }

        [HttpGet]
        public IActionResult GetVendJWRegisterForPDF()
        {
            if (_MemoryCache.TryGetValue("KeyVendJWList", out List<VendJWRegisterDetail> jwRegisterList))
            {
                return Json(jwRegisterList);
            }
            return Json(new List<VendJWRegisterDetail>());
        }
        public async Task<IActionResult> ExportVendJWRegisterToExcel(string ReportType)
        {
            //string modelJson = HttpContext.Session.GetString("KeyPOList");
            if (!_MemoryCache.TryGetValue("KeyVendJWList", out List<VendJWRegisterDetail> modelList))
            {
                return NotFound("No data available to export.");
            }


            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("VendJw Register");

            var reportGenerators = new Dictionary<string, Action<IXLWorksheet, IList<VendJWRegisterDetail>>>
            {

                { "JOBWORKReceiveSUMMARY", EXPORT_JOBWORKReceiveSUMMARYGrid },
                { "JOBWORKReceiveWithAdjustemntDETAIL", EXPORT_JOBWORKReceiveWithAdjustemntDETAILGrid },
                { "JOBWORKReceiveItemWiseSUMM", EXPORT_JOBWORKReceiveItemWiseSUMMGrid },
                { "JOBWORK RECEIVE CONSOLIDATED", EXPORT_JOBWORKRECEIVECONSOLIDATEDGrid },
                { "JOBWORKReceiveDETAIL", EXPORT_JOBWORKReceiveDETAILGrid },
                { "JOBWORKRecoDETAIL", EXPORT_JOBWORKRecoDETAILGrid },
                { "JOBWORKRecoSummary", EXPORT_JOBWORKRecoSummaryGrid },
                { "ONLY PENDING CHALLAN", EXPORT_ONLYPENDINGCHALLANGrid },
                { "JOBWORKISSUECHALLANITEMDETAIL", EXPORT_JOBWORKISSUECHALLANITEMDETAILGrid },
                { "JOBWORKISSUEITEMDETAIL", EXPORT_JOBWORKISSUEITEMDETAILGrid },
                { "JOBWORKISSUEPatyITEMDETAIL", EXPORT_JOBWORKISSUEPatyITEMDETAILGrid },
                { "JOBWORKISSUECHALLANSUMMARY", EXPORT_JOBWORKISSUECHALLANSUMMARYGrid },
                { "JOBWORK ISSUECHALLAN CONSOLIDATED", EXPORT_JOBWORKISSUECHALLANCONSOLIDATEDGrid },
                



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
                "VendJWReport.xlsx"
            );
        }
        private void EXPORT_JOBWORKReceiveSUMMARYGrid(IXLWorksheet sheet, IList<VendJWRegisterDetail> list)
        {
            string[] headers = {
                "#Sr","Vendor Name", "Invoice No", "Invoice Date", "MRN No",
                        "Gate No", "Gate Year Code", "GateDate", "QCCompleted", "Totalamt", "EneterdByEmpName",
                        "UpdatedByEmpName", "NetAmt", "entryid", "YearCode", "ToStore"
            };

            for (int i = 0; i < headers.Length; i++)
                sheet.Cell(1, i + 1).Value = headers[i];

            int row = 2, srNo = 1;
            foreach (var item in list)
            {
                sheet.Cell(row, 1).Value = srNo++;                          // Sr No
                sheet.Cell(row, 2).Value = item.Account_Name;               // Account Name
                sheet.Cell(row, 3).Value = item.JInvNo;                     // Invoice No
                sheet.Cell(row, 4).Value = item.InvDate?.Split(' ')[0];     // Invoice Date (split date)
                sheet.Cell(row, 5).Value = item.MrnNo;                      // MRN No
                sheet.Cell(row, 6).Value = item.Gateno;                     // Gate No
                sheet.Cell(row, 7).Value = item.Gateyearcode;               // Gate Year Code
                sheet.Cell(row, 8).Value = item.GateDate;                   // Gate Date
                sheet.Cell(row, 9).Value = item.QCCompleted;                // QC Completed
                sheet.Cell(row, 10).Value = item.Totalamt;                  // Total Amount
                sheet.Cell(row, 11).Value = item.EntryByMachineName;        // Entry By Machine Name
                sheet.Cell(row, 12).Value = item.UpdatedByEmpName;          // Updated By Employee Name
                sheet.Cell(row, 13).Value = item.NetAmt;                    // Net Amount
                sheet.Cell(row, 14).Value = item.entryid;                   // Entry ID
                sheet.Cell(row, 15).Value = item.YearCode;                  // Year Code
                sheet.Cell(row, 16).Value = item.ToStore;
                row++;
            }
        }
        private void EXPORT_JOBWORKReceiveWithAdjustemntDETAILGrid(IXLWorksheet sheet, IList<VendJWRegisterDetail> list)
        {
            string[] headers = {
                "#Sr","Vendor Name", "Invoice No", "Invoice Date", "MRN No", "Gate No", "Gate Year Code", "Gate Date",
                "Part Code", "Item Name", "Entry By Machine", "Actual Entered By", "Updated By", "Seq No", "Bill Qty", "Received Qty",
                "Job Work Rate", "Job Work Rate Unit", "Unit", "Process Unit", "Batch No", "Unique Batch No", "Item Amount", "Prod/Unprod",
                "BOM Indicator", "BOM No", "BOM Date", "BOM Qty", "No Of Case", "QC Completed", "Issue Challan No", "Issue Challan Date",
                "Adjust Qty", "PO No", "PO Date", "Schedule No", "Schedule Date", "Item Remark", "Process Name", "Total Amount",
                "Entered By Name", "Updated By Name", "Net Amount", "Entry ID", "Remark", "To Store"
            };

            for (int i = 0; i < headers.Length; i++)
                sheet.Cell(1, i + 1).Value = headers[i];

            int row = 2, srNo = 1;
            foreach (var item in list)
            {
                sheet.Cell(row, 1).Value = srNo++;                          // Sr No
                sheet.Cell(row, 2).Value = item.Account_Name;               // Vendor Name
                sheet.Cell(row, 3).Value = item.JInvNo;                     // Invoice No
                sheet.Cell(row, 4).Value = item.InvDate?.Split(' ')[0];     // Invoice Date (split date)
                sheet.Cell(row, 5).Value = item.MrnNo;                      // MRN No
                sheet.Cell(row, 6).Value = item.Gateno;                     // Gate No
                sheet.Cell(row, 7).Value = item.Gateyearcode;               // Gate Year Code
                sheet.Cell(row, 8).Value = item.GateDate;                   // Gate Date
                sheet.Cell(row, 9).Value = item.partcode;                   // Part Code
                sheet.Cell(row, 10).Value = item.Itemname;                  // Item Name
                sheet.Cell(row, 11).Value = item.EntryByMachineName;        // Entry By Machine
                sheet.Cell(row, 12).Value = item.ActualEnteredBy;           // Actual Entered By
                sheet.Cell(row, 13).Value = item.UpdatedBy;                 // Updated By
                sheet.Cell(row, 14).Value = item.Seqno;                     // Seq No
                sheet.Cell(row, 15).Value = item.BillQty;                   // Bill Qty
                sheet.Cell(row, 16).Value = item.RecQty;                    // Received Qty
                sheet.Cell(row, 17).Value = item.JwRate;                    // Job Work Rate
                sheet.Cell(row, 18).Value = item.JwRateUnit;                // Job Work Rate Unit
                sheet.Cell(row, 19).Value = item.unit;                      // Unit
                sheet.Cell(row, 20).Value = item.ProcessUnit;               // Process Unit
                sheet.Cell(row, 21).Value = item.batchno;                   // Batch No
                sheet.Cell(row, 22).Value = item.uniquebatchno;             // Unique Batch No
                sheet.Cell(row, 23).Value = item.ItemAmount;                // Item Amount
                sheet.Cell(row, 24).Value = item.ProdUnprod;                // Prod/Unprod
                sheet.Cell(row, 25).Value = item.BOMIND;                    // BOM Indicator
                sheet.Cell(row, 26).Value = item.BomNo;                     // BOM No
                sheet.Cell(row, 27).Value = item.BomDate;                   // BOM Date
                sheet.Cell(row, 28).Value = item.BOMqTY;                    // BOM Qty
                sheet.Cell(row, 29).Value = item.NoOfCase;                  // No Of Case
                sheet.Cell(row, 30).Value = item.QCCompleted;               // QC Completed
                sheet.Cell(row, 31).Value = item.IssChallanNo;              // Issue Challan No
                sheet.Cell(row, 32).Value = item.IssChallanDate;            // Issue Challan Date
                sheet.Cell(row, 33).Value = item.AdjqTY;                    // Adjust Qty
                sheet.Cell(row, 34).Value = item.PONo;                      // PO No
                sheet.Cell(row, 35).Value = item.PODate;                    // PO Date
                sheet.Cell(row, 36).Value = item.SchNo;                     // Schedule No
                sheet.Cell(row, 37).Value = item.SchDate;                   // Schedule Date
                sheet.Cell(row, 38).Value = item.ItemRemark;                // Item Remark
                sheet.Cell(row, 39).Value = item.Processsname;              // Process Name
                sheet.Cell(row, 40).Value = item.Totalamt;                  // Total Amount
                sheet.Cell(row, 41).Value = item.EneterdByEmpName;          // Entered By Name
                sheet.Cell(row, 42).Value = item.UpdatedByEmpName;          // Updated By Name
                sheet.Cell(row, 43).Value = item.NetAmt;                    // Net Amount
                sheet.Cell(row, 44).Value = item.entryid;                   // Entry ID
                sheet.Cell(row, 45).Value = item.Remark;                    // Remark
                sheet.Cell(row, 46).Value = item.ToStore;
                row++;
            }
        }
        private void EXPORT_JOBWORKReceiveItemWiseSUMMGrid(IXLWorksheet sheet, IList<VendJWRegisterDetail> list)
        {
            string[] headers = {
                "#Sr","Vendor Name", "Part Code", "Item Name", "Bill Qty", "Received Qty",
                 "Job Work Rate", "Job Work Rate Unit", "Unit", "Process Unit", "Item Amount"
            };

            for (int i = 0; i < headers.Length; i++)
                sheet.Cell(1, i + 1).Value = headers[i];

            int row = 2, srNo = 1;
            foreach (var item in list)
            {
                sheet.Cell(row, 1).Value = srNo++;                          // Sr No
                sheet.Cell(row, 2).Value = item.Account_Name;     // Vendor Name
                sheet.Cell(row, 3).Value = item.partcode;         // Part Code
                sheet.Cell(row, 4).Value = item.Itemname;         // Item Name
                sheet.Cell(row, 5).Value = item.BillQty;          // Bill Qty
                sheet.Cell(row, 6).Value = item.RecQty;           // Received Qty
                sheet.Cell(row, 7).Value = item.JwRate;           // Job Work Rate
                sheet.Cell(row, 8).Value = item.JwRateUnit;       // Job Work Rate Unit
                sheet.Cell(row, 9).Value = item.unit;             // Unit
                sheet.Cell(row, 10).Value = item.ProcessUnit;     // Process Unit
                sheet.Cell(row, 11).Value = item.ItemAmount;      // Item Amount
                row++;
            }
        }
        private void EXPORT_JOBWORKRECEIVECONSOLIDATEDGrid(IXLWorksheet sheet, IList<VendJWRegisterDetail> list)
        {
            string[] headers = {
                "#Sr","Vendor Name", "Challan No", "Challan Date", "MRN No", "Part Code", "Item Name",
                "Bill Qty", "Received Qty", "Short/Excess", "Unit", "Job Work Rate", "Item Amount"
            };

            for (int i = 0; i < headers.Length; i++)
                sheet.Cell(1, i + 1).Value = headers[i];

            int row = 2, srNo = 1;
            foreach (var item in list)
            {
                sheet.Cell(row, 1).Value = srNo++;                          // Sr No
                sheet.Cell(row, 2).Value = item.Account_Name;               // Vendor Name
                sheet.Cell(row, 3).Value = item.ChallanNo;                  // Challan No
                sheet.Cell(row, 4).Value = item.ChallanDate?.Split(' ')[0]; // Challan Date (only date)
                sheet.Cell(row, 5).Value = item.MrnNo;                      // MRN No
                sheet.Cell(row, 6).Value = item.partcode;                   // Part Code
                sheet.Cell(row, 7).Value = item.Itemname;                   // Item Name
                sheet.Cell(row, 8).Value = item.BillQty;                    // Bill Qty
                sheet.Cell(row, 9).Value = item.RecQty;                     // Received Qty
                sheet.Cell(row, 10).Value = item.ShortExcess;               // Short/Excess
                sheet.Cell(row, 11).Value = item.unit;                      // Unit
                sheet.Cell(row, 12).Value = item.JwRate;                    // Job Work Rate
                sheet.Cell(row, 13).Value = item.Amount;
                row++;
            }
        }
        private void EXPORT_JOBWORKReceiveDETAILGrid(IXLWorksheet sheet, IList<VendJWRegisterDetail> list)
        {
            string[] headers = {
                "#Sr", "Vendor Name", "Invoice No", "Invoice Date", "MRN No", "Gate No", "Gate Year Code", "Gate Date",
                "Part Code", "Item Name", "Bill Qty", "Received Qty", "Job Work Rate", "Job Work Rate Unit", "Unit",
                "Process Name", "Item Amount", "Prod/Unprod", "BOM Indicator", "BOM No", "BOM Date", "Batch No",
                "Unique Batch No", "No Of Case", "QC Completed", "PO No", "PO Date", "Schedule No", "Schedule Date",
                "Item Remark", "Process Name", "Total Amount", "Entered By Emp Name", "Updated By Emp Name",
                "Net Amount", "Remark", "Entry By Machine Name", "Actual Entered By", "Updated By", "Item Code",
                "Entry ID", "Year Code", "PO Year", "Schedule Year", "To Store"
            };

            for (int i = 0; i < headers.Length; i++)
                sheet.Cell(1, i + 1).Value = headers[i];

            int row = 2, srNo = 1;
            foreach (var item in list)
            {
                sheet.Cell(row, 1).Value = srNo++;                          // Sr No
                sheet.Cell(row, 2).Value = item.Account_Name;                        // Vendor Name
                sheet.Cell(row, 3).Value = item.JInvNo;                               // Invoice No
                sheet.Cell(row, 4).Value = item.InvDate?.Split(' ')[0];               // Invoice Date
                sheet.Cell(row, 5).Value = item.MrnNo;                                // MRN No
                sheet.Cell(row, 6).Value = item.Gateno;                               // Gate No
                sheet.Cell(row, 7).Value = item.Gateyearcode;                         // Gate Year Code
                sheet.Cell(row, 8).Value = item.GateDate;                             // Gate Date
                sheet.Cell(row, 9).Value = item.partcode;                             // Part Code
                sheet.Cell(row, 10).Value = item.Itemname;                            // Item Name
                sheet.Cell(row, 11).Value = item.BillQty;                             // Bill Qty
                sheet.Cell(row, 12).Value = item.RecQty;                              // Received Qty
                sheet.Cell(row, 13).Value = item.JwRate;                              // Job Work Rate
                sheet.Cell(row, 14).Value = item.JwRateUnit;                          // Job Work Rate Unit
                sheet.Cell(row, 15).Value = item.unit;                                // Unit
                sheet.Cell(row, 16).Value = item.Processsname;                        // Process Name
                sheet.Cell(row, 17).Value = item.ItemAmount;                          // Item Amount
                sheet.Cell(row, 18).Value = item.ProdUnprod;                          // Prod/Unprod
                sheet.Cell(row, 19).Value = item.BOMIND;                              // BOM Indicator
                sheet.Cell(row, 20).Value = item.BomNo;                               // BOM No
                sheet.Cell(row, 21).Value = item.BomDate;                             // BOM Date
                sheet.Cell(row, 22).Value = item.batchno;                             // Batch No
                sheet.Cell(row, 23).Value = item.uniquebatchno;                       // Unique Batch No
                sheet.Cell(row, 24).Value = item.NoOfCase;                            // No Of Case
                sheet.Cell(row, 25).Value = item.QCCompleted;                         // QC Completed
                sheet.Cell(row, 26).Value = item.PONo;                                // PO No
                sheet.Cell(row, 27).Value = item.PODate;                              // PO Date
                sheet.Cell(row, 28).Value = item.SchNo;                               // Schedule No
                sheet.Cell(row, 29).Value = item.SchDate;                             // Schedule Date
                sheet.Cell(row, 30).Value = item.ItemRemark;                          // Item Remark
                sheet.Cell(row, 31).Value = item.Processsname;                        // Process Name
                sheet.Cell(row, 32).Value = item.Totalamt;                            // Total Amount
                sheet.Cell(row, 33).Value = item.EneterdByEmpName;                    // Entered By Emp Name
                sheet.Cell(row, 34).Value = item.UpdatedByEmpName;                    // Updated By Emp Name
                sheet.Cell(row, 35).Value = item.NetAmt;                              // Net Amount
                sheet.Cell(row, 36).Value = item.Remark;                              // Remark
                sheet.Cell(row, 37).Value = item.EntryByMachineName;                  // Entry By Machine Name
                sheet.Cell(row, 38).Value = item.ActualEnteredBy;                     // Actual Entered By
                sheet.Cell(row, 39).Value = item.UpdatedBy;                           // Updated By
                sheet.Cell(row, 40).Value = item.ItemCode;                            // Item Code
                sheet.Cell(row, 41).Value = item.entryid;                             // Entry ID
                sheet.Cell(row, 42).Value = item.YearCode;                            // Year Code
                sheet.Cell(row, 43).Value = item.PoYear;                              // PO Year
                sheet.Cell(row, 44).Value = item.SchYear;                             // Schedule Year
                sheet.Cell(row, 45).Value = item.ToStore;
                row++;
            }
        }
        private void EXPORT_JOBWORKRecoDETAILGrid(IXLWorksheet sheet, IList<VendJWRegisterDetail> list)
        {
            string[] headers = {
                "#Sr","Issue Challan No", "Challan Date", "Vendor Name", "Issue Part Code",
                "Issue Item Name", "Issued Qty", "Adjusted Qty", "Pending Qty"
            };

            for (int i = 0; i < headers.Length; i++)
                sheet.Cell(1, i + 1).Value = headers[i];

            int row = 2, srNo = 1;
            foreach (var item in list)
            {
                sheet.Cell(row, 1).Value = srNo++;                          // Sr No
                sheet.Cell(row, 2).Value = item.IssueChallanNo;                     // Issue Challan No
                sheet.Cell(row, 3).Value = item.ChallanDate?.Split(' ')[0];         // Challan Date (only date part)
                sheet.Cell(row, 4).Value = item.Account_Name;                       // Vendor Name
                sheet.Cell(row, 5).Value = item.IssuePartCode;                      // Issue Part Code
                sheet.Cell(row, 6).Value = item.IssueItemName;                      // Issue Item Name
                sheet.Cell(row, 7).Value = item.IssQty;                             // Issued Qty
                sheet.Cell(row, 8).Value = item.AdjqTY;                             // Adjusted Qty
                sheet.Cell(row, 9).Value = item.pendqty;
                row++;
            }
        }
        private void EXPORT_JOBWORKRecoSummaryGrid(IXLWorksheet sheet, IList<VendJWRegisterDetail> list)
        {
            string[] headers = {
                "#Sr","Issue Challan No", "Challan Date", "Vendor Name", "Issue Part Code",
                "Issue Item Name", "Issued Qty", "Adjusted Qty", "Pending Qty",
                "Receive Challan No", "Receive Challan Date", "BOM Indicator",
                "Receive Item Name", "Receive Part Code", "Total Receive Qty",
                "Types", "Completely Received", "Closed"
            };

            for (int i = 0; i < headers.Length; i++)
                sheet.Cell(1, i + 1).Value = headers[i];

            int row = 2, srNo = 1;
            foreach (var item in list)
            {
                sheet.Cell(row, 1).Value = srNo++;                          // Sr No
                sheet.Cell(row, 2).Value = item.IssueChallanNo;                            // Issue Challan No
                sheet.Cell(row, 3).Value = item.ChallanDate?.Split(' ')[0];                // Challan Date
                sheet.Cell(row, 4).Value = item.Account_Name;                              // Vendor Name
                sheet.Cell(row, 5).Value = item.IssuePartCode;                             // Issue Part Code
                sheet.Cell(row, 6).Value = item.IssueItemName;                             // Issue Item Name
                sheet.Cell(row, 7).Value = item.IssQty;                                    // Issued Qty
                sheet.Cell(row, 8).Value = item.AdjqTY;                                    // Adjusted Qty
                sheet.Cell(row, 9).Value = item.pendqty;                                   // Pending Qty
                sheet.Cell(row, 10).Value = item.RecChallanno;                             // Receive Challan No
                sheet.Cell(row, 11).Value = item.RecChallanDate?.Split(' ')[0];            // Receive Challan Date
                sheet.Cell(row, 12).Value = item.BOMIND;                                   // BOM Indicator
                sheet.Cell(row, 13).Value = item.RecItemName;                              // Receive Item Name
                sheet.Cell(row, 14).Value = item.RecPartCode;                              // Receive Part Code
                sheet.Cell(row, 15).Value = item.TotRecQty;                                // Total Receive Qty
                sheet.Cell(row, 16).Value = item.Types;                                    // Types
                sheet.Cell(row, 17).Value = item.CompletlyReceive;                         // Completely Received
                sheet.Cell(row, 18).Value = item.Closed;
                row++;
            }
        }
        private void EXPORT_ONLYPENDINGCHALLANGrid(IXLWorksheet sheet, IList<VendJWRegisterDetail> list)
        {
            string[] headers = {
                "#Sr","Issue Challan No", "Challan Date", "Vendor Name",
                "Issue Part Code", "Issue Item Name", "Issued Qty",
                "Pending Qty", "Adjusted Qty", "Actual Pending Qty"
            };

            for (int i = 0; i < headers.Length; i++)
                sheet.Cell(1, i + 1).Value = headers[i];

            int row = 2, srNo = 1;
            foreach (var item in list)
            {
                sheet.Cell(row, 1).Value = srNo++;                          // Sr No
                sheet.Cell(row, 2).Value = item.IssueChallanNo;                      // Issue Challan No
                sheet.Cell(row, 3).Value = item.ChallanDate?.Split(' ')[0];          // Challan Date
                sheet.Cell(row, 4).Value = item.Account_Name;                        // Vendor Name
                sheet.Cell(row, 5).Value = item.IssuePartCode;                       // Issue Part Code
                sheet.Cell(row, 6).Value = item.IssueItemName;                       // Issue Item Name
                sheet.Cell(row, 7).Value = item.IssQty;                              // Issued Qty
                sheet.Cell(row, 8).Value = item.pendqty;                             // Pending Qty
                sheet.Cell(row, 9).Value = item.AdjqTY;                              // Adjusted Qty
                sheet.Cell(row, 10).Value = item.Actualpendqty;
                row++;
            }
        }
        private void EXPORT_JOBWORKISSUECHALLANITEMDETAILGrid(IXLWorksheet sheet, IList<VendJWRegisterDetail> list)
        {
            string[] headers = {
                "#Sr","JW Challan No", "Challan Date", "Vendor Name", "GST Type", "Vendor State Code",
    "Item Name", "Part Code", "Issued Qty", "Received Qty", "Alt Qty", "Pending Qty", "Pending Alt Qty",
    "Unit", "Batch No", "Unique Batch No", "Item Amount", "Remark Detail", "Purchase Price",
    "Stock Qty", "Batch Stock Qty", "Rec Item Code", "Alt Unit", "PO No", "PO Date", "PO Year",
    "Total Approved Value", "Total Weight", "Total Amount", "Total Net Amount", "Completely Received",
    "Entry By Machine Name", "Actual Entered By", "Updated By", "Remark", "Entry ID", "Year Code"
            };

            for (int i = 0; i < headers.Length; i++)
                sheet.Cell(1, i + 1).Value = headers[i];

            int row = 2, srNo = 1;
            foreach (var item in list)
            {
                sheet.Cell(row, 1).Value = srNo++;                          // Sr No
                sheet.Cell(row, 2).Value = item.JWChallanNo;                        // JW Challan No
                sheet.Cell(row, 3).Value = item.ChallanDate?.Split(' ')[0];         // Challan Date
                sheet.Cell(row, 4).Value = item.Account_Name;                       // Vendor Name
                sheet.Cell(row, 5).Value = item.Gsttype;                            // GST Type
                sheet.Cell(row, 6).Value = item.VendorStateCode;                    // Vendor State Code
                sheet.Cell(row, 7).Value = item.Itemname;                           // Item Name
                sheet.Cell(row, 8).Value = item.partcode;                           // Part Code
                sheet.Cell(row, 9).Value = item.IssQty;                             // Issued Qty
                sheet.Cell(row, 10).Value = item.RecQty;                            // Received Qty
                sheet.Cell(row, 11).Value = item.AltQty;                            // Alt Qty
                sheet.Cell(row, 12).Value = item.pendqty;                           // Pending Qty
                sheet.Cell(row, 13).Value = item.PendAltQty;                        // Pending Alt Qty
                sheet.Cell(row, 14).Value = item.unit;                              // Unit
                sheet.Cell(row, 15).Value = item.batchno;                           // Batch No
                sheet.Cell(row, 16).Value = item.uniquebatchno;                     // Unique Batch No
                sheet.Cell(row, 17).Value = item.ItemAmount;                        // Item Amount
                sheet.Cell(row, 18).Value = item.RemarkDetail;                      // Remark Detail
                sheet.Cell(row, 19).Value = item.PurchasePrice;                     // Purchase Price
                sheet.Cell(row, 20).Value = item.StockQty;                          // Stock Qty
                sheet.Cell(row, 21).Value = item.BatchStockQty;                     // Batch Stock Qty
                sheet.Cell(row, 22).Value = item.RecItemCode;                       // Rec Item Code
                sheet.Cell(row, 23).Value = item.altUnit;                           // Alt Unit
                sheet.Cell(row, 24).Value = item.PONo;                              // PO No
                sheet.Cell(row, 25).Value = item.PODate;                            // PO Date
                sheet.Cell(row, 26).Value = item.PoYear;                            // PO Year
                sheet.Cell(row, 27).Value = item.TolApprVal;                        // Total Approved Value
                sheet.Cell(row, 28).Value = item.TotalWt;                           // Total Weight
                sheet.Cell(row, 29).Value = item.Totalamt;                          // Total Amount
                sheet.Cell(row, 30).Value = item.NetAmt;                            // Total Net Amount
                sheet.Cell(row, 31).Value = item.CompletlyReceive;                  // Completely Received
                sheet.Cell(row, 32).Value = item.EntryByMachineName;                // Entry By Machine Name
                sheet.Cell(row, 33).Value = item.ActualEnteredBy;                   // Actual Entered By
                sheet.Cell(row, 34).Value = item.UpdatedBy;                         // Updated By
                sheet.Cell(row, 35).Value = item.Remark;                            // Remark
                sheet.Cell(row, 36).Value = item.entryid;                           // Entry ID
                sheet.Cell(row, 37).Value = item.YearCode;
                row++;
            }
        }
        private void EXPORT_JOBWORKISSUEITEMDETAILGrid(IXLWorksheet sheet, IList<VendJWRegisterDetail> list)
        {
            string[] headers = {
                "#Sr",    "Item Name", "Part Code", "Issued Qty", "Unit",
                "Amount", "For Process", "Store Name", "Purchase Price", "Pending Qty"
            };

            for (int i = 0; i < headers.Length; i++)
                sheet.Cell(1, i + 1).Value = headers[i];

            int row = 2, srNo = 1;
            foreach (var item in list)
            {
                sheet.Cell(row, 1).Value = srNo++;                          // Sr No
                sheet.Cell(row, 2).Value = item.Itemname;         // Item Name
                sheet.Cell(row, 3).Value = item.partcode;         // Part Code
                sheet.Cell(row, 4).Value = item.IssQty;           // Issued Qty
                sheet.Cell(row, 5).Value = item.unit;              // Unit
                sheet.Cell(row, 6).Value = item.Amount;            // Amount
                sheet.Cell(row, 7).Value = item.FORPROCESS;        // For Process
                sheet.Cell(row, 8).Value = item.STORENAME;         // Store Name
                sheet.Cell(row, 9).Value = item.PurchasePrice;     // Purchase Price
                sheet.Cell(row, 10).Value = item.pendqty;
                row++;
            }
        }
        private void EXPORT_JOBWORKISSUEPatyITEMDETAILGrid(IXLWorksheet sheet, IList<VendJWRegisterDetail> list)
        {
            string[] headers = {
                "#Sr",  "Account Name", "Item Name", "Part Code", "Issued Qty",
                 "Unit", "Amount", "For Process", "Store Name", "Purchase Price", "Pending Qty"
            };

            for (int i = 0; i < headers.Length; i++)
                sheet.Cell(1, i + 1).Value = headers[i];

            int row = 2, srNo = 1;
            foreach (var item in list)
            {
                sheet.Cell(row, 1).Value = srNo++;                          // Sr No
                sheet.Cell(row, 2).Value = item.Account_Name;   // Account Name
                sheet.Cell(row, 3).Value = item.Itemname;       // Item Name
                sheet.Cell(row, 4).Value = item.partcode;       // Part Code
                sheet.Cell(row, 5).Value = item.IssQty;         // Issued Qty
                sheet.Cell(row, 6).Value = item.unit;            // Unit
                sheet.Cell(row, 7).Value = item.Amount;          // Amount
                sheet.Cell(row, 8).Value = item.FORPROCESS;      // For Process
                sheet.Cell(row, 9).Value = item.STORENAME;       // Store Name
                sheet.Cell(row, 10).Value = item.PurchasePrice;  // Purchase Price
                sheet.Cell(row, 11).Value = item.pendqty;
                row++;
            }
        }
        private void EXPORT_JOBWORKISSUECHALLANSUMMARYGrid(IXLWorksheet sheet, IList<VendJWRegisterDetail> list)
        {
            string[] headers = {
                "#Sr","JW Challan No", "Challan Date", "Account Name", "GST Type", "Vendor State Code",
                "Total Approved Value", "Total Weight", "Total Amount", "Net Amount",
                "Transporter Name", "Distance", "Eway Bill No", "Closed", "Types",
                "Completely Receive", "Entry By Machine Name", "Actual Entered By",
                "Updated By", "Remark", "Entry ID", "Year Code"
                        };

            for (int i = 0; i < headers.Length; i++)
                sheet.Cell(1, i + 1).Value = headers[i];

            int row = 2, srNo = 1;
            foreach (var item in list)
            {
                sheet.Cell(row, 1).Value = srNo++;                          // Sr No
                sheet.Cell(row, 2).Value = item.JWChallanNo;                       // JW Challan No
                sheet.Cell(row, 3).Value = item.ChallanDate?.Split(' ')[0];        // Challan Date (date part)
                sheet.Cell(row, 4).Value = item.Account_Name;                      // Account Name
                sheet.Cell(row, 5).Value = item.Gsttype;                           // GST Type
                sheet.Cell(row, 6).Value = item.VendorStateCode;                   // Vendor State Code
                sheet.Cell(row, 7).Value = item.TolApprVal;                        // Total Approved Value
                sheet.Cell(row, 8).Value = item.TotalWt;                           // Total Weight
                sheet.Cell(row, 9).Value = item.Totalamt;                          // Total Amount
                sheet.Cell(row, 10).Value = item.NetAmt;                           // Net Amount
                sheet.Cell(row, 11).Value = item.TransporterName;                  // Transporter Name
                sheet.Cell(row, 12).Value = item.Distance;                         // Distance
                sheet.Cell(row, 13).Value = item.EwayBillNo;                       // Eway Bill No
                sheet.Cell(row, 14).Value = item.Closed;                           // Closed
                sheet.Cell(row, 15).Value = item.Types;                            // Types
                sheet.Cell(row, 16).Value = item.CompletlyReceive;                 // Completely Receive
                sheet.Cell(row, 17).Value = item.EntryByMachineName;               // Entry By Machine Name
                sheet.Cell(row, 18).Value = item.ActualEnteredBy;                  // Actual Entered By
                sheet.Cell(row, 19).Value = item.UpdatedBy;                        // Updated By
                sheet.Cell(row, 20).Value = item.Remark;                           // Remark
                sheet.Cell(row, 21).Value = item.entryid;                          // Entry ID
                sheet.Cell(row, 22).Value = item.YearCode;
                row++;
            }
        }
         private void EXPORT_JOBWORKISSUECHALLANCONSOLIDATEDGrid(IXLWorksheet sheet, IList<VendJWRegisterDetail> list)
        {
            string[] headers = {
                "#Sr","Vendor Name", "Challan No", "Challan Date", "Part Code", "Item Name",
                "Issued Qty", "Unit", "Purchase Price", "Pending Qty", "For Process",
                "Remark", "Closed", "Completely Receive"
            };

            for (int i = 0; i < headers.Length; i++)
                sheet.Cell(1, i + 1).Value = headers[i];

            int row = 2, srNo = 1;
            foreach (var item in list)
            {
                sheet.Cell(row, 1).Value = srNo++;                          // Sr No
                sheet.Cell(row, 2).Value = item.VendorName;                           // Vendor Name
                sheet.Cell(row, 3).Value = item.ChallanNo;                           // Challan No
                sheet.Cell(row, 4).Value = item.ChallanDate?.Split(' ')[0];          // Challan Date
                sheet.Cell(row, 5).Value = item.partcode;                            // Part Code
                sheet.Cell(row, 6).Value = item.Itemname;                            // Item Name
                sheet.Cell(row, 7).Value = item.IssQty;                              // Issued Qty
                sheet.Cell(row, 8).Value = item.unit;                                // Unit
                sheet.Cell(row, 9).Value = item.PurchasePrice;                       // Purchase Price
                sheet.Cell(row, 10).Value = item.pendqty;                            // Pending Qty
                sheet.Cell(row, 11).Value = item.FORPROCESS;                         // For Process
                sheet.Cell(row, 12).Value = item.Remark;                             // Remark
                sheet.Cell(row, 13).Value = item.Closed;                             // Closed
                sheet.Cell(row, 14).Value = item.CompletlyReceive;
                row++;
            }
         }

    }
}
