using eTactWeb.Data.Common;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using eTactWeb.DOM.Models;
using eTactWeb.Services;
using eTactWeb.DOM;
using eTactWeb.Services.Helpers;
using NuGet.Protocol.Core.Types;
using QuestPDF.Helpers;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Globalization;


namespace eTactWeb.Controllers
{
    public class TransactionLedgerController : Controller
    {
        private readonly IDataLogic _IDataLogic;
        public ITransactionLedger _TransactionLedger { get; }

        private readonly ILogger<TransactionLedgerController> _logger;
        private readonly IConfiguration iconfiguration;
        public IWebHostEnvironment _IWebHostEnvironment { get; }
        public TransactionLedgerController(ILogger<TransactionLedgerController> logger, IDataLogic iDataLogic, ITransactionLedger iTransactionLedger, EncryptDecrypt encryptDecrypt, IWebHostEnvironment iWebHostEnvironment, IConfiguration iconfiguration)
        {
            _logger = logger;
            _IDataLogic = iDataLogic;
            _TransactionLedger = iTransactionLedger;
            _IWebHostEnvironment = iWebHostEnvironment;
            this.iconfiguration = iconfiguration;
        }
        [Route("{controller}/Index")]
        [HttpGet]
        public async Task<ActionResult> TransactionLedger(string formKey, string ReportType = "", int Accountcode = 0)
        {
            ViewBag.formKey = formKey;
            var uniqueKey = Guid.NewGuid().ToString();
            ViewBag.uniqueKey = uniqueKey;
            var MainModel = new TransactionLedgerModel();
            MainModel.TransactionLedgerGrid = new List<TransactionLedgerModel>();
            MainModel.ReportType = ReportType;
            MainModel.AccountCode = Accountcode;
            MainModel.FromDate = HttpContext.Session.GetString("FromDate");
            MainModel.ToDate = HttpContext.Session.GetString("ToDate");
            MainModel.YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
            return View(MainModel);
        }
        public async Task<JsonResult> GetLedgerName(int? ParentAccountCode)
        {
            var JSON = await _TransactionLedger.GetLedgerName(ParentAccountCode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillLedgerName()
        {
            var JSON = await _TransactionLedger.FillLedgerName();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public byte[] GenerateLedgerPdf(IList<TransactionLedgerModel> data)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            if (data == null || !data.Any())
                return Array.Empty<byte>();

            CompanyDetails company = GetCompanyDetails(data);

            List<LedgerPage> pages = BuildPages(data);

            decimal totalDr = data.Sum(x => x.DrAmt);

            decimal totalCr = data.Sum(x => x.CrAmt);

            return Document.Create(document =>
            {
                foreach (var pageData in pages)
                {
                    document.Page(page =>
                    {
                        //page.Size(PageSizes.A4);
                        page.Size(PageSizes.A4);

                        page.MarginLeft(10);
                        page.MarginRight(10);
                        page.MarginTop(15);
                        page.MarginBottom(15);

                        page.DefaultTextStyle(x =>
                            x.FontFamily("Arial")
                             .FontSize(7));

                        DrawHeader(page, company, data.First(), pageData);

                        DrawBody(page, pageData, totalDr, totalCr);

                        DrawFooter(page, company);
                    });
                }

            }).GeneratePdf();
        }
        private List<LedgerPage> BuildPages(IList<TransactionLedgerModel> data)
        {
            var pages = new List<LedgerPage>();

            if (data == null || data.Count == 0)
                return pages;

            // Number of transaction rows per page.
            // Adjust this after final PDF design.
            const int rowsPerPage = 21;

            int totalRows = data.Count;
            int pageIndex = 0;

            decimal openingBalance = 0;
            string openingType = "Dr";
            var openingRow = data.FirstOrDefault(x =>
    x.Particulars != null &&
    x.Particulars.Trim().Equals("Opening", StringComparison.OrdinalIgnoreCase)
);
            // Calculate Opening Balance
            var firstRow = data.First();

            openingBalance = firstRow.Balance - firstRow.DrAmt + firstRow.CrAmt;
            if (openingRow != null)
            {
                openingBalance = Math.Abs(openingRow.Balance);
                openingType = openingRow.Types;   // Dr / Cr
            }
            else if (openingBalance < 0)
            {
                openingType = "Cr";
                openingBalance = Math.Abs(openingBalance);
            }

            decimal previousBalance = openingBalance;
            string previousType = openingType;

            while (pageIndex * rowsPerPage < totalRows)
            {
                var pageRows = data
                    .Skip(pageIndex * rowsPerPage)
                    .Take(rowsPerPage)
                    .ToList();

                LedgerPage page = new LedgerPage();

                page.Rows = pageRows;

                page.IsFirstPage = pageIndex == 0;

                page.IsLastPage = ((pageIndex + 1) * rowsPerPage) >= totalRows;

                // B/F Balance
                page.BFBalance = previousBalance;
                page.BFType = previousType;

                // C/F Balance
                var lastRow = pageRows.Last();

                page.CFBalance = Math.Abs(lastRow.Balance);
                page.CFType = lastRow.Types;

                // Calculate page totals
                page.PageDrTotal = pageRows.Sum(x => x.DrAmt);
                page.PageCrTotal = pageRows.Sum(x => x.CrAmt);

                // Grand totals till current page
                page.RunningDrTotal = data
                    .Take((pageIndex * rowsPerPage) + pageRows.Count)
                    .Sum(x => x.DrAmt);

                page.RunningCrTotal = data
                    .Take((pageIndex * rowsPerPage) + pageRows.Count)
                    .Sum(x => x.CrAmt);

                // Last page closing
                if (page.IsLastPage)
                {
                    page.ClosingBalance = Math.Abs(lastRow.Balance);
                    page.ClosingType = lastRow.Types;
                }

                pages.Add(page);

                // Carry Forward
                previousBalance = page.CFBalance;
                previousType = page.CFType;

                pageIndex++;
            }

            return pages;
        }
        private CompanyDetails GetCompanyDetails(IList<TransactionLedgerModel> data)
        {
            CompanyDetails company = new CompanyDetails();

            if (data == null || !data.Any())
                return company;

            var companyDetailparameters = _TransactionLedger.GetCompanyDetail(data.First().AccountCodeBack);

            if (companyDetailparameters != null &&
                companyDetailparameters.Result != null &&
                companyDetailparameters.Result.Result != null &&
                companyDetailparameters.Result.Result.Rows.Count > 0)
            {
                DataRow dr = companyDetailparameters.Result.Result.Rows[0];

                company.CompanyName = dr["CompanyName"]?.ToString() ?? "";

                company.CompanyAddress1 = dr["CompanyAddress1"]?.ToString() ?? "";

                company.CompanyAddress2 = dr["CompanyAddress2"]?.ToString() ?? "";

                company.LedgerAddress = dr["LedgerAddress"]?.ToString() ?? "";

                if (dr.Table.Columns.Contains("CompanyPhone"))
                    company.CompanyPhone = dr["CompanyPhone"]?.ToString() ?? "";

                if (dr.Table.Columns.Contains("CompanyEmail"))
                    company.CompanyEmail = dr["CompanyEmail"]?.ToString() ?? "";

                if (dr.Table.Columns.Contains("GSTIN"))
                    company.GSTIN = dr["GSTIN"]?.ToString() ?? "";
            }

            return company;
        }
        private void DrawHeader(
    PageDescriptor page,
    CompanyDetails company,
    TransactionLedgerModel model,
    LedgerPage pageData)
        {
            page.Header().Column(header =>
            {
                //==========================================
                // COMPANY NAME
                //==========================================

                header.Item()
                    .AlignCenter()
                    .Text(company.CompanyName)
                    .Bold()
                    .FontSize(18);

                //==========================================
                // ADDRESS 1
                //==========================================

                if (!string.IsNullOrWhiteSpace(company.CompanyAddress1))
                {
                    header.Item()
                        .AlignCenter()
                        .Text(company.CompanyAddress1)
                        .FontSize(9);
                }

                //==========================================
                // ADDRESS 2
                //==========================================

                if (!string.IsNullOrWhiteSpace(company.CompanyAddress2))
                {
                    header.Item()
                        .AlignCenter()
                        .Text(company.CompanyAddress2)
                        .FontSize(9);
                }

                //==========================================
                // PHONE & EMAIL (Optional)
                //==========================================

                if (!string.IsNullOrWhiteSpace(company.CompanyPhone) ||
                    !string.IsNullOrWhiteSpace(company.CompanyEmail))
                {
                    header.Item()
                        .AlignCenter()
                        .Text(txt =>
                        {
                            if (!string.IsNullOrWhiteSpace(company.CompanyPhone))
                                txt.Span(company.CompanyPhone);

                            if (!string.IsNullOrWhiteSpace(company.CompanyPhone) &&
                                !string.IsNullOrWhiteSpace(company.CompanyEmail))
                                txt.Span("   |   ");

                            if (!string.IsNullOrWhiteSpace(company.CompanyEmail))
                                txt.Span(company.CompanyEmail);
                        });
                }

                //==========================================
                // GST
                //==========================================

                if (!string.IsNullOrWhiteSpace(company.GSTIN))
                {
                    header.Item()
                        .AlignCenter()
                        .Text($"GSTIN : {company.GSTIN}")
                        .FontSize(7);
                }

                header.Item().PaddingTop(6);

                //==========================================
                // REPORT TITLE
                //==========================================

                header.Item()
                    .AlignCenter()
                   .Text("LEDGER ACCOUNT")
.Bold()
.FontSize(13)
.FontColor(Colors.Blue.Darken2);
                //==========================================
                // DATE RANGE
                //==========================================

                header.Item()
                    .PaddingTop(2)
                    .AlignCenter()
                    .Text(text =>
                    {
                        text.Span("From : ");
                        text.Span(model.FromDate).SemiBold();

                        text.Span("     To : ");

                        text.Span(model.ToDate).SemiBold();
                    });

                header.Item().PaddingTop(6);

                //==========================================
                // PARTY NAME
                //==========================================

                header.Item().Row(row =>
                {
                    row.ConstantItem(90)
                        .Text("Party Name")
                        .Bold();

                    row.RelativeItem()
                        .Text(model.AccountName)
                        .SemiBold();
                });

                //==========================================
                // LEDGER ADDRESS
                //==========================================

                if (!string.IsNullOrWhiteSpace(company.LedgerAddress))
                {
                    header.Item()
                        .PaddingLeft(90)
                        .Text(company.LedgerAddress)
                        .FontSize(7);
                }

                header.Item().PaddingTop(5);

                //==========================================
                // OPENING / B/F BALANCE
                //==========================================

                header.Item().Row(row =>
                {
                    row.ConstantItem(90);

                    row.RelativeItem()
                        .Text(pageData.IsFirstPage
                            ? "Opening Balance"
                            : "B/F Balance")
                        .Bold();

                    row.ConstantItem(120)
                        .AlignRight()
                        .Text($"{pageData.BFBalance:N2} {pageData.BFType}")
                        .Bold();
                });

                header.Item().PaddingTop(5);

                //==========================================
                // LINE
                //==========================================

                header.Item()
                    .LineHorizontal(1)
                    .LineColor(Colors.Black);
            });
        }
        private void DrawTableHeader(TableDescriptor table)
        {
            table.ColumnsDefinition(columns =>
            {
                columns.ConstantColumn(45);      // Date
                columns.RelativeColumn(5);       // Particulars
                columns.ConstantColumn(40);      // Vch Type
                columns.ConstantColumn(55);      // Vch No
                columns.ConstantColumn(45);      // Inv Date
                columns.ConstantColumn(55);      // Inv No
                columns.ConstantColumn(55);      // Dr
                columns.ConstantColumn(55);      // Cr
                columns.ConstantColumn(60);      // Balance
            });

            table.Header(header =>
            {
                static IContainer HeaderCell(IContainer container)
                {
                    return container
                        .Border(0.8f)
                        .BorderColor(Colors.Black)
                        .Background("#D9D9D9")
                        .PaddingVertical(4)
                        .PaddingHorizontal(3)
                        .AlignCenter()
                        .AlignMiddle();
                }

                //==========================
                // FIRST ROW
                //==========================

                header.Cell()
                    .RowSpan(2)
                    .Element(HeaderCell)
                    .Text("DATE")
                    .Bold()
                    .FontSize(7);

                header.Cell()
                    .RowSpan(2)
                    .Element(HeaderCell)
                    .Text("PARTICULARS")
                    .Bold()
                    .FontSize(7);

                header.Cell()
                    .RowSpan(2)
                    .Element(HeaderCell)
                    .Text("VCH\nTYPE")
                    .Bold()
                    .FontSize(7);

                header.Cell()
                    .RowSpan(2)
                    .Element(HeaderCell)
                    .Text("VCH\nNO")
                    .Bold()
                    .FontSize(7);

                header.Cell()
                    .RowSpan(2)
                    .Element(HeaderCell)
                    .Text("INV\nDATE")
                    .Bold()
                    .FontSize(7);

                header.Cell()
                    .RowSpan(2)
                    .Element(HeaderCell)
                    .Text("INV\nNO")
                    .Bold()
                    .FontSize(7);

                header.Cell()
                    .Element(HeaderCell)
                    .Text("DR")
                    .Bold()
                    .FontSize(7);

                header.Cell()
                    .Element(HeaderCell)
                    .Text("CR")
                    .Bold()
                    .FontSize(7);

                header.Cell()
                    .Element(HeaderCell)
                    .Text("BALANCE")
                    .Bold()
                    .FontSize(7);

                //==========================
                // SECOND ROW
                //==========================

                header.Cell()
                    .Element(HeaderCell)
                    .AlignRight()
                    .Text("AMOUNT")
                    .Bold()
                    .FontSize(7);

                header.Cell()
                    .Element(HeaderCell)
                    .AlignRight()
                    .Text("AMOUNT")
                    .Bold()
                    .FontSize(7);

                header.Cell()
                    .Element(HeaderCell)
                    .Text("")
                    .Bold();
            });
        }
        private void DrawTransactionRows(
    TableDescriptor table,
    LedgerPage pageData)
        {
            static IContainer BodyCell(IContainer container)
            {
                return container
                    .BorderBottom(0.5f)
                    .BorderColor(Colors.Grey.Lighten2)
                    .PaddingHorizontal(3)
                    .PaddingVertical(2)
                    .MinHeight(25);      // Fixed row height
            }

            ////======================================================
            //// OPENING BALANCE / B/F BALANCE
            ////======================================================

            //table.Cell()
            //    .Element(BodyCell)
            //    .Text("");

            //table.Cell()
            //    .Element(BodyCell)
            //    .Text(pageData.IsFirstPage
            //        ? "OPENING BALANCE"
            //        : "B/F BALANCE")
            //    .Bold();

            //table.Cell().Element(BodyCell).Text("");

            //table.Cell().Element(BodyCell).Text("");

            //table.Cell().Element(BodyCell).Text("");

            //table.Cell().Element(BodyCell).Text("");

            //table.Cell()
            //    .Element(BodyCell)
            //    .AlignRight()
            //    .Text("");

            //table.Cell()
            //    .Element(BodyCell)
            //    .AlignRight()
            //    .Text("");

            //table.Cell()
            //    .Element(BodyCell)
            //    .AlignRight()
            //    .Text($"{pageData.BFBalance:N2} {pageData.BFType}")
            //    .Bold();

            //======================================================
            // TRANSACTIONS
            //======================================================

            foreach (var item in pageData.Rows)
            {
                string particulars = item.Particulars ?? "";

                //// Additional Details
                //if (!string.IsNullOrWhiteSpace(item.SumDet))
                //    particulars += "\n" + item.SumDet;

                //// Narration
                //if (!string.IsNullOrWhiteSpace(item.HeadWiseNarration))
                //    particulars += "\n" + item.HeadWiseNarration;

                //// Voucher Remark
                //if (!string.IsNullOrWhiteSpace(item.Narration))
                //    particulars += "\n" + item.Narration;

                //----------------------------------------------------
                // DATE
                //----------------------------------------------------

                table.Cell()
                    .Element(BodyCell)
                    .Text(item.VoucherDocDate ?? "")
                    .FontSize(7);

                //----------------------------------------------------
                // PARTICULARS
                //----------------------------------------------------

                table.Cell()
                    .Element(BodyCell)
                    .Text(particulars)
.FontSize(7)
.LineHeight(1.15f);
                //----------------------------------------------------
                // VOUCHER TYPE
                //----------------------------------------------------

                table.Cell()
                    .Element(BodyCell)
                    .AlignCenter()
                    .Text(item.VoucherType ?? "")
                    .FontSize(7);

                //----------------------------------------------------
                // VOUCHER NO
                //----------------------------------------------------

                table.Cell()
                    .Element(BodyCell)
                   .AlignLeft()
.Text(item.VchNo ?? "")

                    .FontSize(7);

                //----------------------------------------------------
                // INVOICE DATE
                //----------------------------------------------------

                table.Cell()
                    .Element(BodyCell)
                    .AlignCenter()
                    .Text(item.VoucherDocDate ?? "")
                    .FontSize(7);

                //----------------------------------------------------
                // INVOICE NO
                //----------------------------------------------------

                table.Cell()
                    .Element(BodyCell)
                   .AlignLeft()
.Text(item.INVNo ?? "")
                    .FontSize(7);

                //----------------------------------------------------
                // DR AMOUNT
                //----------------------------------------------------

                table.Cell()
                    .Element(BodyCell)
                    .AlignRight()
                    .Text(item.DrAmt == 0
                        ? ""
                        : item.DrAmt.ToString("N2"))
                    .FontSize(7);

                //----------------------------------------------------
                // CR AMOUNT
                //----------------------------------------------------

                table.Cell()
                    .Element(BodyCell)
                    .AlignRight()
                    .Text(item.CrAmt == 0
                        ? ""
                        : item.CrAmt.ToString("N2"))
                    .FontSize(7);

                //----------------------------------------------------
                // BALANCE
                //----------------------------------------------------

                table.Cell()
                    .Element(BodyCell)
                    .AlignRight()
                    .Text($"{item.Balance:N2} {item.Types}")
                    .FontSize(7);
            }

            //======================================================
            // C/F BALANCE
            //======================================================

            table.Cell()
                .Element(BodyCell)
                .Text("");

            table.Cell()
                .Element(BodyCell)
                .Text("C/F BALANCE")
                .Bold();

            table.Cell().Element(BodyCell).Text("");

            table.Cell().Element(BodyCell).Text("");

            table.Cell().Element(BodyCell).Text("");

            table.Cell().Element(BodyCell).Text("");

            table.Cell()
                .Element(BodyCell)
                .AlignRight()
                .Text("");

            table.Cell()
                .Element(BodyCell)
                .AlignRight()
                .Text("");

            table.Cell()
                .Element(BodyCell)
                .AlignRight()
                .Text($"{pageData.CFBalance:N2} {pageData.CFType}")
                .Bold();
        }
        private void DrawBody(
    PageDescriptor page,
    LedgerPage pageData,
    decimal totalDr,
    decimal totalCr)
        {
            page.Content()
                .PaddingTop(8)
                .Column(column =>
                {
                    column.Item()
                        .Table(table =>
                        {
                            // Create Header
                            DrawTableHeader(table);

                            // Draw all ledger rows
                            DrawTransactionRows(table, pageData);

                            //---------------------------------------------------
                            // TOTAL (Only Last Page)
                            //---------------------------------------------------

                            if (pageData.IsLastPage)
                            {
                                DrawTotal(
                                    table,
                                    totalDr,
                                    totalCr);
                            }

                            //---------------------------------------------------
                            // CLOSING BALANCE (Only Last Page)
                            //---------------------------------------------------

                            if (pageData.IsLastPage)
                            {
                                DrawClosingAmount(
                                    table,
                                    pageData.ClosingBalance,
                                    pageData.ClosingType);
                            }
                        });
                });
        }
        private void DrawTotal(
    TableDescriptor table,
    decimal totalDr,
    decimal totalCr)
        {
            static IContainer TotalCell(IContainer container)
            {
                return container
                    .BorderTop(1)
                    .BorderBottom(1)
                    .BorderColor(Colors.Black)
                    .Background("#E8E8E8")
                    .PaddingVertical(5)
                    .PaddingHorizontal(3);
            }

            //----------------------------------------------------------
            // TOTAL LABEL
            //----------------------------------------------------------

            table.Cell()
                .ColumnSpan(6)
                .Element(TotalCell)
                .AlignRight()
                .Text("TOTAL")
                .Bold()
                .FontSize(9);

            //----------------------------------------------------------
            // TOTAL DR
            //----------------------------------------------------------

            table.Cell()
                .Element(TotalCell)
                .AlignRight()
                .Text(totalDr == 0
                    ? ""
                    : totalDr.ToString("N2"))
                .Bold()
                .FontSize(8);

            //----------------------------------------------------------
            // TOTAL CR
            //----------------------------------------------------------

            table.Cell()
                .Element(TotalCell)
                .AlignRight()
                .Text(totalCr == 0
                    ? ""
                    : totalCr.ToString("N2"))
                .Bold()
                .FontSize(8);

            //----------------------------------------------------------
            // BALANCE COLUMN
            //----------------------------------------------------------

            table.Cell()
                .Element(TotalCell)
                .Text("");
        }
        private void DrawClosingAmount(
    TableDescriptor table,
    decimal closingBalance,
    string closingType)
        {
            static IContainer ClosingCell(IContainer container)
            {
                return container
                    .BorderTop(1)
                    .BorderBottom(1)
                    .BorderColor(Colors.Black)
                   .Background("#F5F5F5")
.PaddingVertical(6)
                    .PaddingHorizontal(3);
            }

            //----------------------------------------------------------
            // LABEL
            //----------------------------------------------------------

            table.Cell()
                .ColumnSpan(8)
                .Element(ClosingCell)
                .AlignRight()
                .Text("CLOSING BALANCE")
                .Bold()
                .FontSize(9);

            //----------------------------------------------------------
            // BALANCE
            //----------------------------------------------------------

            table.Cell()
    .Element(ClosingCell)
    .AlignRight()
    .AlignMiddle()
    .Text(text =>
    {
        text.Span($"{Math.Abs(closingBalance):N2} {closingType}")
            .Bold()
            .FontSize(8);
    });
        }

        private void DrawFooter(
    PageDescriptor page,
    CompanyDetails company)
        {
            page.Footer()
                .PaddingTop(5)
                .BorderTop(1)
                .BorderColor(Colors.Grey.Lighten2)
                .Row(row =>
                {
                    //---------------------------------------------
                    // PRINT DATE
                    //---------------------------------------------

                    row.RelativeItem()
                        .AlignLeft()
                        .Text(text =>
                        {
                            text.Span("Printed On : ")
                                .SemiBold();

                            text.Span(
                                DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
                        });

                    //---------------------------------------------
                    // COMPANY NAME
                    //---------------------------------------------

                    row.RelativeItem()
                        .AlignCenter()
                        .Text(company.CompanyName)
                        .FontSize(7)
                        .SemiBold();

                    //---------------------------------------------
                    // PAGE NUMBER
                    //---------------------------------------------

                    row.RelativeItem()
                        .AlignRight()
                        .Text(text =>
                        {
                            text.Span("Page ");

                            text.CurrentPageNumber();

                            text.Span(" of ");

                            text.TotalPages();
                        });
                });
        }
        public async Task<IActionResult> DownloadLedgerPdf(
    string FromDate, string ToDate, string ReportType,
    string GroupOrLedger, int? ParentAccountCode,
    int? AccountCode, string VoucherType,
    string VoucherNo, string InvoiceNo,
    string Narration, float? Amount,
    string DR, string CR, string Ledger, string AccountName, string SubVoucherType)
        {
            var data = await _TransactionLedger.GetDetailsData(
                FromDate, ToDate, ReportType,
                GroupOrLedger, ParentAccountCode,
                AccountCode, VoucherType,
                VoucherNo, InvoiceNo,
                Narration, Amount, DR, CR, Ledger, AccountName, SubVoucherType);

            if (data.TransactionLedgerGrid == null || !data.TransactionLedgerGrid.Any())
                return NotFound("No data available.");

            //var generator = new LedgerPdfGenerator();
            var pdfBytes = GenerateLedgerPdf(data.TransactionLedgerGrid);

            return File(pdfBytes, "application/pdf", "LedgerVoucher.pdf");
        }

        public async Task<IActionResult> GetDetailsData(string formKey,
            string FromDate = null, string ToDate = null, string ReportType = null,
            string GroupOrLedger = null, int? ParentAccountCode = null, int? AccountCode = null,
            string VoucherType = null, string VoucherNo = null, string InvoiceNo = null,
            string Narration = null, float? Amount = null, string DR = null, string CR = null,
            string Ledger = null, string AccountName = null, string SubVoucherType = "")
        {
            var model = await _TransactionLedger.GetDetailsData(
                FromDate, ToDate, ReportType, GroupOrLedger,
                ParentAccountCode, AccountCode, VoucherType,
                VoucherNo, InvoiceNo, Narration, Amount, DR, CR, Ledger, AccountName, SubVoucherType
            );
            ViewBag.formKey = formKey;

            var sessionData = JsonConvert.SerializeObject(model);
            model.ReportType = ReportType;
            HttpContext.Session.SetString("TransactionLedgerData", sessionData);

            return PartialView("_TransactionLedgerGrid", model);

        }
        public async Task<IActionResult> GetTransactionLedgerMonthlySummaryDetailsData(string formKey, string FromentryDate, string ToEntryDate, int AccountCode, string ReportType)
        {
            ViewBag.formKey = formKey;

            var model = new TransactionLedgerModel();
            model = await _TransactionLedger.GetTransactionLedgerMonthlySummaryDetailsData(FromentryDate, ToEntryDate, AccountCode);
            var sessionData = JsonConvert.SerializeObject(model);
            HttpContext.Session.SetString("TransactionLedgerData", sessionData);
            return PartialView("_TransactionLedgerMonthlySummaryGrid", model);

        }
        public async Task<IActionResult> GetTransactionLedgerGroupSummaryDetailsData(string formKey, string FromDate, string ToDate, string ReportType, string GroupOrLedger, int? ParentAccountCode = null, int AccountCode = 0, string? VoucherType = null, string? VoucherNo = null, string? InvoiceNo = null, string? Narration = null, float? Amount = null, string? DR = null, string? CR = null, string? Ledger = null)
        {
            ViewBag.formKey = formKey;

            var model = new TransactionLedgerModel();
            model = await _TransactionLedger.GetTransactionLedgerGroupSummaryDetailsData(FromDate, ToDate, ReportType, GroupOrLedger, ParentAccountCode, AccountCode, VoucherType, VoucherNo, InvoiceNo, Narration, Amount, DR, CR, Ledger);
            var sessionData = JsonConvert.SerializeObject(model);
            HttpContext.Session.SetString("TransactionLedgerData", sessionData);
            return PartialView("_TransactionLedgerGroupSummaryGrid", model);
        }
        public async Task<JsonResult> FillVoucherName()
        {
            var JSON = await _TransactionLedger.FillVoucherName();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        [HttpGet]
        public IActionResult ExportTransactionLedgerToExcel(string ReportType, string FromDate, string ToDate, string LedgerCode, decimal totBal)
        {
            var BranchName = HttpContext.Session.GetString("Branch");
            var CompanyName = HttpContext.Session.GetString("CompanyName");

            // Get session data
            var sessionData = HttpContext.Session.GetString("TransactionLedgerData");
            if (string.IsNullOrEmpty(sessionData))
                return BadRequest("Transaction ledger data not found in session.");

            var model = JsonConvert.DeserializeObject<TransactionLedgerModel>(sessionData);
            DataTable dt = new DataTable("TransactionLedger");
            int sr = 1;
            string sheetName = ReportType;

            if (ReportType == "TransactionLedgerSummary" || ReportType == "TransactionLedgerDetail")
            {
                dt.Columns.Add("Sr#", typeof(int));
                dt.Columns.Add("Voucher Doc Date", typeof(string));
                dt.Columns.Add("Particulars", typeof(string));
                dt.Columns.Add("Voucher Type", typeof(string));
                dt.Columns.Add("Inv/Vch No", typeof(string));
                dt.Columns.Add("DR Amt", typeof(decimal));
                dt.Columns.Add("CR Amt", typeof(decimal));
                dt.Columns.Add("Balance", typeof(decimal));
                dt.Columns.Add("Types", typeof(string));
                dt.Columns.Add("HeadWiseNarration", typeof(string));
                dt.Columns.Add("Inv No", typeof(string));
                dt.Columns.Add("Inv Date", typeof(string));
                dt.Columns.Add("Doc Entry Id", typeof(int));
                dt.Columns.Add("SumDet", typeof(string));
                dt.Columns.Add("VCHEMark", typeof(string));
                dt.Columns.Add("AccountCode", typeof(int));
                dt.Columns.Add("ReportType", typeof(string));
                dt.Columns.Add("Vch No", typeof(string));
                dt.Columns.Add("AccEntryId", typeof(int));
                dt.Columns.Add("AccEntry YearCode", typeof(int));

                sr = 1;
                foreach (var row in model.TransactionLedgerGrid)
                {
                    dt.Rows.Add(
                        sr++,
                        row.VoucherDocDate,
                        row.Particulars,
                        row.VoucherType,
                        row.InvoiceVoucherNo,
                        row.DrAmt,
                        row.CrAmt,
                        row.Balance,
                        row.Types,
                        row.HeadWiseNarration,
                        row.INVNo,
                        row.BillDate,
                        row.DocEntryId,
                        row.SumDet,
                        row.VCHEMark,
                        row.AccountCode,
                        row.ReportType,
                        row.VchNo,
                        row.AccEntryId,
                        row.AccEntryYearCode
                    );
                }

                sheetName = "TransactionLedgerDetail";
            }
            else if (ReportType == "MonthlySummary")
            {
                dt.Columns.Add("Sr#", typeof(int));
                dt.Columns.Add("MonthFullName", typeof(string));
                dt.Columns.Add("Total DR", typeof(decimal));
                dt.Columns.Add("Total CR", typeof(decimal));
                dt.Columns.Add("Closing Amt", typeof(decimal));
                dt.Columns.Add("Dr/Cr", typeof(string));
                dt.Columns.Add("YearCode", typeof(string));
                dt.Columns.Add("SeqNo", typeof(int));

                foreach (var row in model.TransactionLedgerGrid)
                {
                    dt.Rows.Add(
                        sr++,
                        row.MonthFullName,
                        row.TotalDr,
                        row.TotalCr,
                        row.ClosingAmt,
                        row.Dr_CR,
                        row.YearCode,
                        row.SeqNo
                    );
                }
            }
            else if (ReportType == "GROUPSUMMARY" || ReportType == "GROUPDETAIL" || ReportType == "BalanceConfirmation")
            {
                dt.Columns.Add("Sr#", typeof(int));
                dt.Columns.Add("Parent Group Name", typeof(string));
                dt.Columns.Add("Account Name", typeof(string));
                dt.Columns.Add("Open DR", typeof(decimal));
                dt.Columns.Add("Open CR", typeof(decimal));
                dt.Columns.Add("Total Opening", typeof(decimal));
                dt.Columns.Add("Current DR", typeof(decimal));
                dt.Columns.Add("Current CR", typeof(decimal));
                dt.Columns.Add("Net Current Amt", typeof(decimal));
                dt.Columns.Add("Net Amt", typeof(decimal));
                dt.Columns.Add("Group/Ledger", typeof(string));

                sr = 1;
                foreach (var row in model.TransactionLedgerGrid) // assuming GroupSummary uses TransactionLedgerGrid
                {
                    dt.Rows.Add(
                        sr++,
                        row.ParentLedgerName,
                        row.AccountName,
                        row.OpnDr,
                        row.OpnCr,
                        row.TotalOpening,
                        row.CurrDrAmt,
                        row.CurrCrAmt,
                        row.NetCurrentAmt,
                        row.NetAmount,
                        row.GroupLedger == "G" ? "Group" : "Ledger"
                    );
                }

                sheetName = "GroupSummary";
            }
            else if (ReportType == "BalanceConfirmationSummary")
            {
                dt.Columns.Add("Sr#", typeof(int));
                dt.Columns.Add("LedgerName", typeof(string));
                dt.Columns.Add("VoucherDocDate", typeof(string));
                dt.Columns.Add("VoucherNo", typeof(string));
                dt.Columns.Add("VoucherType", typeof(string));
                dt.Columns.Add("DR Amt", typeof(decimal));
                dt.Columns.Add("CR Amt", typeof(decimal));
                dt.Columns.Add("AdjustedDrAmt", typeof(decimal));
                dt.Columns.Add("AdjustedCrAmt", typeof(decimal));
                dt.Columns.Add("Balance", typeof(decimal));
                dt.Columns.Add("NetAmount", typeof(decimal));
                dt.Columns.Add("DueDate", typeof(string));
                dt.Columns.Add("SubVoucherName", typeof(string));


                sr = 1;
                foreach (var row in model.TransactionLedgerGrid)
                {
                    dt.Rows.Add(
                        sr++,
                        row.LedgerName,
                        row.VoucherDocDate,
                        row.VoucherNo,
                        row.VoucherType,
                        row.DrAmt,
                        row.CrAmt,
                        row.AdjustedDrAmt,
                        row.AdjustedCrAmt,
                        row.Balance,
                        row.NetAmount,
                        row.DueDate,
                        row.SubVoucherName

                    );
                }

                sheetName = "BalanceConfirmationSummary";
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
                LedgerCode,
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
