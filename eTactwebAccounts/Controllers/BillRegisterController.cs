using eTactWeb.Controllers;
using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System.Data;

namespace eTactwebAccounts.Controllers
{
    public class BillRegisterController : Controller
    {
        private readonly IDataLogic _IDataLogic;
        public IBillRegister _IBillRegister { get; }
        private readonly ILogger<BillRegisterController> _logger;
        private readonly IConfiguration iconfiguration;
        public IWebHostEnvironment _IWebHostEnvironment { get; }
        public BillRegisterController(ILogger<BillRegisterController> logger, IDataLogic iDataLogic, IBillRegister iBillRegister, EncryptDecrypt encryptDecrypt, IWebHostEnvironment iWebHostEnvironment, IConfiguration iconfiguration)
        {
            _logger = logger;
            _IDataLogic = iDataLogic;
            _IBillRegister = iBillRegister;
            _IWebHostEnvironment = iWebHostEnvironment;
            this.iconfiguration = iconfiguration;
        }
        [Route("{controller}/Index")]
        public IActionResult BillRegister()
        {
            var model = new BillRegisterModel();
            model.BillRegisterList = new List<BillRegisterModel>();
            return View(model);
        }
        public async Task<JsonResult> FillVendoreList(string FromDate, string ToDate)
        {
            var JSON = await _IBillRegister.FillVendoreList(FromDate, ToDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillDocList(string FromDate, string ToDate)
        {
            var JSON = await _IBillRegister.FillDocList(FromDate, ToDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        } 
        public async Task<JsonResult> FillPONOList(string FromDate, string ToDate)
        {
            var JSON = await _IBillRegister.FillPONOList(FromDate, ToDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillSchNOList(string FromDate, string ToDate)
        {
            var JSON = await _IBillRegister.FillSchNOList(FromDate, ToDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        } 
        public async Task<JsonResult> FillHsnNOList(string FromDate, string ToDate)
        {
            var JSON = await _IBillRegister.FillHsnNOList(FromDate, ToDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        } 
        public async Task<JsonResult> FillGstNOList(string FromDate, string ToDate)
        {
            var JSON = await _IBillRegister.FillGstNOList(FromDate, ToDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        } 
        public async Task<JsonResult> FillInvoiceNOList(string FromDate, string ToDate)
        {
            var JSON = await _IBillRegister.FillInvoiceNOList(FromDate, ToDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
         public async Task<JsonResult> FillItemName(string FromDate, string ToDate)
        {
            var JSON = await _IBillRegister.FillItemName(FromDate, ToDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
         public async Task<JsonResult> FillPartCode(string FromDate, string ToDate)
        {
            var JSON = await _IBillRegister.FillPartCode(FromDate, ToDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<IActionResult> GetRegisterData(string ReportType, string FromDate, string ToDate, string DocumentName, string PONO, string SchNo, string HsnNo, string GstNo, string InvoiceNo, string PurchaseBill, string PurchaseRejection, string DebitNote, string CreditNote, string SaleRejection, string PartCode, string ItemName, string VendorName,int ForFinYear)
        {
            var model = new BillRegisterModel();
            ForFinYear= Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
            model = await _IBillRegister.GetRegisterData( ReportType,  FromDate,  ToDate,  DocumentName,  PONO,  SchNo,  HsnNo,  GstNo,  InvoiceNo,  PurchaseBill,  PurchaseRejection,  DebitNote,  CreditNote,  SaleRejection,  PartCode,  ItemName,  VendorName, ForFinYear);
            model.ReportType = ReportType;
            MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                SlidingExpiration = TimeSpan.FromMinutes(55),
                Size = 1024,
            };
            string serializedGrid = JsonConvert.SerializeObject(model.BillRegisterList);
            HttpContext.Session.SetString("KeyBillRegisterList", serializedGrid);
            if(ReportType== "PurchaseSummary")
            {
                return PartialView("_BillRegisterGrid", model);
            }
            else if(ReportType== "PurchaseBillDetail")
            {
                return PartialView("_BillRegisterPurchaseBillDetailGrid", model);
            }
            else if(ReportType== "PURCHASESUMMARYREG"||ReportType== "GSTSUMMARY")
            {
                return PartialView("_BillRegisterGrid", model);
            }

            else if(ReportType== "VendorItemWiseTrend" )
            {
                return PartialView("_BillRegisterVendorItemWiseTrendGrid", model);
            }
            else if(ReportType== "VendorWiseMonthlyTrend")
            {
                return PartialView("_BillRegisterVendorWiseMonthlyTrendGrid", model);
            }
             else if(ReportType== "VendorItemWiseMonthlyData")
            {
                return PartialView("_BillRegisterVendorItemWiseMonthlyDataGrid", model);
            }
            else if(ReportType== "ItemWiseMonthlyTrend")
            {
                return PartialView("_BillRegisterItemWiseMonthlyTrendGrid", model);
            }
             else if(ReportType== "VendorWiseMonthlyValueTrend")
            {
                return PartialView("_BillRegisterVendorWiseMonthlyValueTrendGrid", model);
            }

            return null;
        }
        [HttpGet]
        public IActionResult GetBillRegistergridPDFData()
        {
            string modelJson = HttpContext.Session.GetString("KeyBillRegisterList");
            List<BillRegisterModel> billRegisterList = new List<BillRegisterModel>();
            if (!string.IsNullOrEmpty(modelJson))
            {
                billRegisterList = JsonConvert.DeserializeObject<List<BillRegisterModel>>(modelJson);
            }
            return Json(billRegisterList);
        }
        [HttpGet]
        public IActionResult ExportBillRegisterToExcel(string ReportType, string FromDate, string ToDate)
        {
            var BranchName = HttpContext.Session.GetString("Branch");
            var CompanyName = HttpContext.Session.GetString("CompanyName");

            // Get session data
            var sessionData = HttpContext.Session.GetString("KeyBillRegisterList");
            if (string.IsNullOrEmpty(sessionData))
                return BadRequest("BillRegister data not found in session.");
            var modelList = JsonConvert.DeserializeObject<List<BillRegisterModel>>(sessionData);
            //var model = JsonConvert.DeserializeObject<BillRegisterModel>(sessionData);
            DataTable dt = new DataTable("BillRegister");
            int sr = 1;
            string sheetName = ReportType;

            if (ReportType == "PurchaseSummary")
            {
                dt.Columns.Add("Sr#", typeof(int));
                dt.Columns.Add("Description", typeof(string));
                dt.Columns.Add("For The Duration", typeof(string));
                dt.Columns.Add("For Fin Year", typeof(decimal));
              
                sr = 1;
                foreach (var row in modelList)
                {
                    dt.Rows.Add(
                        sr++,
                        row.Description,
                        row.ForTheDuration,
                        row.ForFinYear
                        
                    );
                }

                sheetName = "PurchaseSummary";
            }
            else if (ReportType == "PurchaseBillDetail")
            {
                dt.Columns.Add("Sr#", typeof(int));
                dt.Columns.Add("VendorName", typeof(string));
                dt.Columns.Add("GstNo", typeof(string));
                dt.Columns.Add("VendorAddress", typeof(string));
                dt.Columns.Add("State", typeof(string));
                dt.Columns.Add("InvoiceNo", typeof(string));
                dt.Columns.Add("InvDate", typeof(string));
                dt.Columns.Add("VoucherNo", typeof(string));
                dt.Columns.Add("VoucherDate", typeof(string));
                dt.Columns.Add("MRNNo", typeof(string));
                dt.Columns.Add("MRNDate", typeof(string));
                dt.Columns.Add("GateNo", typeof(string));
                dt.Columns.Add("GateDate", typeof(string));
                dt.Columns.Add("PartCode", typeof(string));
                dt.Columns.Add("ItemName", typeof(string));
                dt.Columns.Add("HsnNo", typeof(string));
                dt.Columns.Add("BillQty", typeof(decimal));
                dt.Columns.Add("RecQty", typeof(decimal));
                dt.Columns.Add("RejectedQty", typeof(decimal));
                dt.Columns.Add("Unit", typeof(string));
                dt.Columns.Add("BillRate", typeof(decimal));
                dt.Columns.Add("PoRate", typeof(decimal));
                dt.Columns.Add("DiscountPer", typeof(decimal));
                dt.Columns.Add("DisAmt", typeof(decimal));
                dt.Columns.Add("ItemAmount", typeof(decimal));
                dt.Columns.Add("CGSTPer", typeof(decimal));
                dt.Columns.Add("CGSTAmt", typeof(decimal));
                dt.Columns.Add("SGSTPer", typeof(decimal));
                dt.Columns.Add("SGSTAmt", typeof(decimal));
                dt.Columns.Add("IGSTPer", typeof(decimal));
                dt.Columns.Add("IGSTAmt", typeof(decimal));
                dt.Columns.Add("ExpenseAmt", typeof(decimal));
                dt.Columns.Add("TotalBillAmt", typeof(decimal));
                dt.Columns.Add("TotaltaxableAmt", typeof(decimal));
                dt.Columns.Add("GSTAmount", typeof(decimal));
                dt.Columns.Add("Currency", typeof(string));
                dt.Columns.Add("InvAmt", typeof(decimal));
                dt.Columns.Add("MIRNO", typeof(string));
                dt.Columns.Add("MIRDate", typeof(string));
                dt.Columns.Add("ItemParentGroup", typeof(string));
                dt.Columns.Add("ItemCategory", typeof(string));
                dt.Columns.Add("TypeItemServAssets", typeof(string));
                dt.Columns.Add("DomesticImport", typeof(string));
                dt.Columns.Add("PurchaseBillDirectPB", typeof(string));
                dt.Columns.Add("PurchaseBillTypeMRNJWChallan", typeof(string));
                dt.Columns.Add("PaymentDays", typeof(int));
                dt.Columns.Add("PaymentTerm", typeof(string));
                dt.Columns.Add("Approved", typeof(string));
                dt.Columns.Add("ApprovedDate", typeof(string));
                dt.Columns.Add("EntryByMachineName", typeof(string));
                dt.Columns.Add("PurchBillEntryId", typeof(int));
                dt.Columns.Add("PurchBillYearCode", typeof(int));
                dt.Columns.Add("EntryByEmployee", typeof(string));
                dt.Columns.Add("ActualEntryDate", typeof(string));
                dt.Columns.Add("LastUpdatedByEmp", typeof(string));
                dt.Columns.Add("LastUpdatedDate", typeof(string));


                sr = 1;
                foreach (var row in modelList)
                {
                    dt.Rows.Add(
                        sr++,
                        row.VendorName,
                        row.GstNo,
                        row.VendorAddress,
                        row.State,
                        row.InvoiceNo,
                        row.InvDate,
                        row.VoucherNo,
                        row.VoucherDate,
                        row.MRNNo,
                        row.MRNDate,
                        row.GateNo,
                        row.GateDate,
                        row.PartCode,
                        row.ItemName,
                        row.HsnNo,
                        row.BillQty,
                        row.RecQty,
                        row.RejectedQty,
                        row.Unit,
                        row.BillRate,
                        row.PoRate,
                        row.DiscountPer,
                        row.DisAmt,
                        row.ItemAmount,
                        row.CGSTPer,
                        row.CGSTAmt,
                        row.SGSTPer,
                        row.SGSTAmt,
                        row.IGSTPer,
                        row.IGSTAmt,
                        row.ExpenseAmt,
                        row.TotalBillAmt,
                        row.TotaltaxableAmt,
                        row.GSTAmount,
                        row.Currency,
                        row.InvAmt,
                        row.MIRNO,
                        row.MIRDate,
                        row.ItemParentGroup,
                        row.ItemCategory,
                        row.TypeItemServAssets,
                        row.DomesticImport,
                        row.PurchaseBillDirectPB,
                        row.PurchaseBillTypeMRNJWChallan,
                        row.PaymentDays,
                        row.PaymentTerm,
                        row.Approved,
                        row.ApprovedDate,
                        row.EntryByMachineName,
                        row.PurchBillEntryId,
                        row.PurchBillYearCode,
                        row.EntryByEmployee,
                        row.ActualEntryDate,
                        row.LastUpdatedByEmp,
                        row.LastUpdatedDate


                    );
                }

                sheetName = "PurchaseBillDetail";
            }

            else if (ReportType == "PURCHASESUMMARYREG")
            {
                dt.Columns.Add("Sr#", typeof(int));
                dt.Columns.Add("VendorName", typeof(string));
                dt.Columns.Add("InvoiceNo", typeof(string));
                dt.Columns.Add("InvoiceDate", typeof(string));
                dt.Columns.Add("VoucherNo", typeof(string));
                dt.Columns.Add("VoucherDate", typeof(string));
                dt.Columns.Add("MRNNo", typeof(string));
                dt.Columns.Add("MRNDate", typeof(string));
                dt.Columns.Add("BillAmt", typeof(decimal));
                dt.Columns.Add("TaxableAmt", typeof(decimal));
                dt.Columns.Add("GSTAmount", typeof(decimal));
                dt.Columns.Add("Currency", typeof(string));
                dt.Columns.Add("TotalBillQty", typeof(decimal));
                dt.Columns.Add("TotalDisAmt", typeof(decimal));
                dt.Columns.Add("TotalItemAmt", typeof(decimal));
                dt.Columns.Add("CGSTPer", typeof(decimal));
                dt.Columns.Add("CGSTAmt", typeof(decimal));
                dt.Columns.Add("SGSTPer", typeof(decimal));
                dt.Columns.Add("SGSTAmt", typeof(decimal));
                dt.Columns.Add("IGSTPer", typeof(decimal));
                dt.Columns.Add("IGSTAmt", typeof(decimal));
                dt.Columns.Add("ExpenseAmt", typeof(decimal));
                dt.Columns.Add("InvAmt", typeof(decimal));
                dt.Columns.Add("GSTType", typeof(string));
                dt.Columns.Add("GstNo", typeof(string));
                dt.Columns.Add("VendorAddress", typeof(string));
                dt.Columns.Add("State", typeof(string));
                dt.Columns.Add("DirectPBOrAgainstMRN", typeof(string));
                dt.Columns.Add("EntryByMachineName", typeof(string));
                dt.Columns.Add("TypeItemServAssets", typeof(string));
                dt.Columns.Add("DomesticImport", typeof(string));
                dt.Columns.Add("PaymentDays", typeof(int));
                dt.Columns.Add("PurchBillEntryId", typeof(int));
                dt.Columns.Add("PurchBillYearCode", typeof(int));
                dt.Columns.Add("ActualEntryByEmp", typeof(string));
                dt.Columns.Add("ActualEntryDate", typeof(string));
                dt.Columns.Add("LastUpdatedByEmp", typeof(string));
                dt.Columns.Add("LastUpdatedDate", typeof(string));
                dt.Columns.Add("Remark", typeof(string));
                dt.Columns.Add("Country", typeof(string));

                sr = 1;
                foreach (var row in modelList)
                {
                    dt.Rows.Add(
                        sr++,
                        row.VendorName,
                        row.InvoiceNo,
                        row.InvoiceDate,
                        row.VoucherNo,
                        row.VoucherDate,
                        row.MRNNo,
                        row.MRNDate,
                        row.BillAmt,
                        row.TaxableAmt,
                        row.GSTAmount,
                        row.Currency,
                        row.TotalBillQty,
                        row.TotalDisAmt,
                        row.TotalItemAmt,
                        row.CGSTPer,
                        row.CGSTAmt,
                        row.SGSTPer,
                        row.SGSTAmt,
                        row.IGSTPer,
                        row.IGSTAmt,
                        row.ExpenseAmt,
                        row.InvAmt,
                        row.GSTType,
                        row.GstNo,
                        row.VendorAddress,
                        row.State,
                        row.DirectPBOrAgainstMRN,
                        row.EntryByMachineName,
                        row.TypeItemServAssets,
                        row.DomesticImport,
                        row.PaymentDays,
                        row.PurchBillEntryId,
                        row.PurchBillYearCode,
                        row.ActualEntryByEmp,
                        row.ActualEntryDate,
                        row.LastUpdatedByEmp,
                        row.LastUpdatedDate,
                        row.Remark,
                        row.Country


                    );
                }

                sheetName = "PurchaseSummary Reg";
            }
            else if (ReportType == "VendorItemWiseTrend")
            {
                dt.Columns.Add("Sr#", typeof(int));
                dt.Columns.Add("VendorName", typeof(string));
                dt.Columns.Add("PartCode", typeof(string));
                dt.Columns.Add("ItemName", typeof(string));
                dt.Columns.Add("HsnNo", typeof(string));
                dt.Columns.Add("TotalQty", typeof(decimal));
                dt.Columns.Add("Unit", typeof(string));
                dt.Columns.Add("TotalAmount", typeof(decimal));
                
                sr = 1;
                foreach (var row in modelList)
                {
                    dt.Rows.Add(
                        sr++,
                        row.VendorName,
                        row.PartCode,
                        row.ItemName,
                        row.HsnNo,
                        row.TotalQty,
                        row.Unit,
                        row.TotalAmount
                    );
                }

                sheetName = "Vendor Item Wise Trend";
            }
            else if (ReportType == "VendorWiseMonthlyTrend")
            {
                dt.Columns.Add("Sr#", typeof(int));
                dt.Columns.Add("VendorName", typeof(string));
                dt.Columns.Add("AprAmt", typeof(decimal));
                dt.Columns.Add("MayAmt", typeof(decimal));
                dt.Columns.Add("JunAmt", typeof(decimal));
                dt.Columns.Add("JulAmt", typeof(decimal));
                dt.Columns.Add("AugAmt", typeof(decimal));
                dt.Columns.Add("SepAmt", typeof(decimal));
                dt.Columns.Add("OctAmt", typeof(decimal));
                dt.Columns.Add("NovAmt", typeof(decimal));
                dt.Columns.Add("DecAmt", typeof(decimal));
                dt.Columns.Add("JanAmt", typeof(decimal));
                dt.Columns.Add("FebAmt", typeof(decimal));
                dt.Columns.Add("MarAmt", typeof(decimal));

                dt.Columns.Add("AprQty", typeof(decimal));
                dt.Columns.Add("MayQty", typeof(decimal));
                dt.Columns.Add("JunQty", typeof(decimal));
                dt.Columns.Add("JulQty", typeof(decimal));
                dt.Columns.Add("AugQty", typeof(decimal));
                dt.Columns.Add("SepQty", typeof(decimal));
                dt.Columns.Add("OctQty", typeof(decimal));
                dt.Columns.Add("NovQty", typeof(decimal));
                dt.Columns.Add("DecQty", typeof(decimal));
                dt.Columns.Add("JanQty", typeof(decimal));
                dt.Columns.Add("FebQty", typeof(decimal));
                dt.Columns.Add("MarQty", typeof(decimal));


                sr = 1;
                foreach (var row in modelList)
                {
                    dt.Rows.Add(
                        sr++,
                        row.VendorName,
                        row.AprAmt,
                        row.MayAmt,
                        row.JunAmt,
                        row.JulAmt,
                        row.AugAmt,
                        row.SepAmt,
                        row.OctAmt,
                        row.NovAmt,
                        row.DecAmt,
                        row.JanAmt,
                        row.FebAmt,
                        row.MarAmt,

                        row.AprQty,
                        row.MayQty,
                        row.JunQty,
                        row.JulQty,
                        row.AugQty,
                        row.SepQty,
                        row.OctQty,
                        row.NovQty,
                        row.DecQty,
                        row.JanQty,
                        row.FebQty,
                        row.MarQty

                    );
                }

                sheetName = "Vendor Wise Monthly Trend";
            }
            else if (ReportType == "VendorItemWiseMonthlyData")
            {
                dt.Columns.Add("Sr#", typeof(int));
                dt.Columns.Add("VendorName", typeof(string));
                dt.Columns.Add("PartCode", typeof(string));
                dt.Columns.Add("ItemName", typeof(string));
                dt.Columns.Add("Unit", typeof(string));
                dt.Columns.Add("MonthName", typeof(string));
                dt.Columns.Add("TotalQty", typeof(decimal));
                dt.Columns.Add("TotalAmount", typeof(decimal));
                
                sr = 1;
                foreach (var row in modelList)
                {
                    dt.Rows.Add(
                        sr++,
                        row.VendorName,
                        row.PartCode,
                        row.ItemName,
                        row.Unit,
                        row.MonthName,
                        row.TotalQty,
                        row.TotalAmount
                    );
                }

                sheetName = "Vendor Item Wise Monthly Data";
            }
            else if (ReportType == "ItemWiseMonthlyTrend")
            {
                dt.Columns.Add("Sr#", typeof(int));
                dt.Columns.Add("ItemName", typeof(string));
                dt.Columns.Add("PartCode", typeof(string));

                dt.Columns.Add("AprAmt", typeof(decimal));
                dt.Columns.Add("MayAmt", typeof(decimal));
                dt.Columns.Add("JunAmt", typeof(decimal));
                dt.Columns.Add("JulAmt", typeof(decimal));
                dt.Columns.Add("AugAmt", typeof(decimal));
                dt.Columns.Add("SepAmt", typeof(decimal));
                dt.Columns.Add("OctAmt", typeof(decimal));
                dt.Columns.Add("NovAmt", typeof(decimal));
                dt.Columns.Add("DecAmt", typeof(decimal));
                dt.Columns.Add("JanAmt", typeof(decimal));
                dt.Columns.Add("FebAmt", typeof(decimal));
                dt.Columns.Add("MarAmt", typeof(decimal));

                dt.Columns.Add("AprQty", typeof(decimal));
                dt.Columns.Add("MayQty", typeof(decimal));
                dt.Columns.Add("JunQty", typeof(decimal));
                dt.Columns.Add("JulQty", typeof(decimal));
                dt.Columns.Add("AugQty", typeof(decimal));
                dt.Columns.Add("SepQty", typeof(decimal));
                dt.Columns.Add("OctQty", typeof(decimal));
                dt.Columns.Add("NovQty", typeof(decimal));
                dt.Columns.Add("DecQty", typeof(decimal));
                dt.Columns.Add("JanQty", typeof(decimal));
                dt.Columns.Add("FebQty", typeof(decimal));
                dt.Columns.Add("MarQty", typeof(decimal));


                sr = 1;
                foreach (var row in modelList)
                {
                    dt.Rows.Add(
                        sr++,
                        row.ItemName,
                        row.PartCode,

                        row.AprAmt,
                        row.MayAmt,
                        row.JunAmt,
                        row.JulAmt,
                        row.AugAmt,
                        row.SepAmt,
                        row.OctAmt,
                        row.NovAmt,
                        row.DecAmt,
                        row.JanAmt,
                        row.FebAmt,
                        row.MarAmt,

                        row.AprQty,
                        row.MayQty,
                        row.JunQty,
                        row.JulQty,
                        row.AugQty,
                        row.SepQty,
                        row.OctQty,
                        row.NovQty,
                        row.DecQty,
                        row.JanQty,
                        row.FebQty,
                        row.MarQty

                    );
                }

                sheetName = "Item Wise Monthly Trend";
            }
            else if (ReportType == "VendorWiseMonthlyValueTrend")
            {
                dt.Columns.Add("Sr#", typeof(int));
                dt.Columns.Add("VendorName", typeof(string));
                dt.Columns.Add("AprAmt", typeof(decimal));
                dt.Columns.Add("MayAmt", typeof(decimal));
                dt.Columns.Add("JunAmt", typeof(decimal));
                dt.Columns.Add("JulAmt", typeof(decimal));
                dt.Columns.Add("AugAmt", typeof(decimal));
                dt.Columns.Add("SepAmt", typeof(decimal));
                dt.Columns.Add("OctAmt", typeof(decimal));
                dt.Columns.Add("NovAmt", typeof(decimal));
                dt.Columns.Add("DecAmt", typeof(decimal));
                dt.Columns.Add("JanAmt", typeof(decimal));
                dt.Columns.Add("FebAmt", typeof(decimal));
                dt.Columns.Add("MarAmt", typeof(decimal));

                sr = 1;
                foreach (var row in modelList)
                {
                    dt.Rows.Add(
                        sr++,
                        row.VendorName,
                        row.AprAmt,
                        row.MayAmt,
                        row.JunAmt,
                        row.JulAmt,
                        row.AugAmt,
                        row.SepAmt,
                        row.OctAmt,
                        row.NovAmt,
                        row.DecAmt,
                        row.JanAmt,
                        row.FebAmt,
                        row.MarAmt
                    );
                }

                sheetName = "Vendor Wise Monthly Value Trend";
            }

            
            else
            {
                return BadRequest("Invalid report type.");
            }

            // Generate Excel
            var stream = ExcelHelper.GenerateExcel(
                dt,
                sheetName,
                CompanyName,
                BranchName,
                FromDate,
                ToDate
            );

            string excelName = $"{sheetName}.xlsx";

            // Force ASCII-safe header
            Response.Headers.Add("Content-Disposition", $"attachment; filename=\"{excelName}\"");

            return File(
                stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
            );
        }
    }
}
