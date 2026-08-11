using ClosedXML.Excel;
using DocumentFormat.OpenXml.Office2010.PowerPoint;
using DocumentFormat.OpenXml.Wordprocessing;
using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data;
using System.Globalization;
using System.Net;
using System.Xml.Linq;
using iTextSharp.text.pdf;
using iTextSharp.text;
using System.IO;

namespace eTactWeb.Controllers
{
    public class SaleBillRegisterController : Controller
    {
        private readonly IDataLogic _IDataLogic;
        public ISaleBillRegister _ISaleBillRegister { get; }
        private readonly ILogger<SaleBillRegisterController> _logger;
        private readonly IConfiguration iconfiguration;
        public IWebHostEnvironment _IWebHostEnvironment { get; }
        public SaleBillRegisterController(ILogger<SaleBillRegisterController> logger, IDataLogic iDataLogic, ISaleBillRegister iSaleBillRegister, EncryptDecrypt encryptDecrypt, IWebHostEnvironment iWebHostEnvironment, IConfiguration iconfiguration)
        {
            _logger = logger;
            _IDataLogic = iDataLogic;
            _ISaleBillRegister = iSaleBillRegister;
            _IWebHostEnvironment = iWebHostEnvironment;
            this.iconfiguration = iconfiguration;
        }
        [Route("{controller}/Index")]
        public IActionResult SaleBillRegister(string formKey)
        {
            ViewBag.formKey = formKey;
            var uniqueKey = Guid.NewGuid().ToString();
            ViewBag.uniqueKey = uniqueKey;
            var model = new SaleBillRegisterModel();
            model.SaleBillRegisterDetail = new List<SaleBillRegisterDetail>();
            return View(model);
        }

        public async Task<IActionResult> GetSaleBillRegisterData(string ReportType, string FromDate, string ToDate, string docname, string SONo, string Schno, int itemCode, string ItemName, string SaleBillNo, string CustomerName, string HSNNO, string GSTNO, int AccountCode,int ItemParentGroup, int pageNumber = 1, int pageSize = 100, string SearchBox = "")

        {
            var model = new SaleBillRegisterModel();
            if (string.IsNullOrEmpty(SaleBillNo) || SaleBillNo == "0")
            { SaleBillNo = ""; }
            if (string.IsNullOrEmpty(docname) || docname == "0")
            { docname = ""; }
            if (string.IsNullOrEmpty(SONo) || SONo == "0")
            { SONo = ""; }
            if (string.IsNullOrEmpty(Schno) || Schno == "0")
            { Schno = ""; }
            //if (string.IsNullOrEmpty(itemCode) || itemCode == "0")
            //{ itemCode = ""; }
            if (string.IsNullOrEmpty(ItemName) || ItemName == "0")
            { ItemName = ""; }
            if (string.IsNullOrEmpty(CustomerName) || CustomerName == "0")
            { CustomerName = ""; }
            if (string.IsNullOrEmpty(HSNNO) || HSNNO == "0")
            { HSNNO = ""; }
            if (string.IsNullOrEmpty(GSTNO) || GSTNO == "0")
            { GSTNO = ""; }

            int yearcode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
            var result = await _ISaleBillRegister.GetSaleBillRegisterData(ReportType, FromDate, ToDate, docname, SONo, Schno, itemCode, ItemName, SaleBillNo, CustomerName, HSNNO, GSTNO, AccountCode, yearcode, ItemParentGroup);

            if (result == null || !(result.Result is DataTable dt))
            {
                model.TotalRecords = 0;
                model.Rows = new List<Dictionary<string, object>>();
                return PartialView("_SaleBillRegisterGrid", model);
            }

            model.TotalRecords = dt.Rows.Count;
            model.PageNumber = pageNumber;
            model.PageSize = pageSize;
            var allRows = dt.AsEnumerable()
    .Select(r => dt.Columns
        .Cast<DataColumn>()
        .ToDictionary(
            c => c.ColumnName,
            c => r[c] == DBNull.Value ? null : r[c]
        ))
    .ToList();

            // 🔹 Store FULL data in session
            HttpContext.Session.SetString("KeySaleBillRegsiterList", JsonConvert.SerializeObject(allRows));
            // 🔥 Dynamic Headers
            model.Headers = dt.Columns
                .Cast<DataColumn>()
                .Select(c => new DashboardColumn
                {
                    Title = c.ColumnName,
                    Field = c.ColumnName
                })
                .ToList();

            // 🔥 Dynamic Rows with Pagination
            model.Rows = dt.AsEnumerable()
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(r => dt.Columns
                    .Cast<DataColumn>()
                    .ToDictionary(
                        c => c.ColumnName,
                        c => r[c] == DBNull.Value ? null : r[c]
                    ))
                .ToList();
            // 🔹 Store in session if needed
            //HttpContext.Session.SetString("KeySaleBillRegsiterList", JsonConvert.SerializeObject(model.Rows));

            return PartialView("_SaleBillRegisterGrid", model);

        }
        //public async Task<IActionResult> GetSaleBillRegisterData(string ReportType, string FromDate, string ToDate, string docname, string SONo, string Schno, string PartCode, string ItemName, string SaleBillNo, string CustomerName, string HSNNO, string GSTNO)

        //{
        //    var model = new SaleBillRegisterModel();
        //    if (string.IsNullOrEmpty(SaleBillNo) || SaleBillNo == "0" )
        //        { SaleBillNo = "";            }
        //    if (string.IsNullOrEmpty(docname) || docname == "0")
        //    { docname = ""; }
        //    if (string.IsNullOrEmpty(SONo) || SONo == "0")
        //    { SONo = ""; }
        //    if (string.IsNullOrEmpty(Schno) || Schno == "0")
        //    { Schno = ""; }
        //    if (string.IsNullOrEmpty(PartCode) || PartCode == "0")
        //    { PartCode = ""; }
        //    if (string.IsNullOrEmpty(ItemName) || ItemName == "0")
        //    { ItemName = ""; }
        //    if (string.IsNullOrEmpty(CustomerName) || CustomerName == "0")
        //    { CustomerName = ""; }
        //    if (string.IsNullOrEmpty(HSNNO) || HSNNO == "0")
        //    { HSNNO = ""; }
        //    if (string.IsNullOrEmpty(GSTNO) || GSTNO == "0")
        //    { GSTNO = ""; }
        //    model = await _ISaleBillRegister.GetSaleBillRegisterData(ReportType, FromDate, ToDate, docname, SONo, Schno, PartCode, ItemName, SaleBillNo, CustomerName, HSNNO, GSTNO);
        //    model.ReportMode= ReportType;
        //    MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
        //    {
        //        AbsoluteExpiration = DateTime.Now.AddMinutes(60),
        //        SlidingExpiration = TimeSpan.FromMinutes(55),
        //        Size = 1024,
        //    };
        //    string serializedGrid = JsonConvert.SerializeObject(model.SaleBillRegisterDetail);
        //    HttpContext.Session.SetString("KeySaleBillRegsiterList", serializedGrid);
        //    //return PartialView("_SaleBillRegisterSaleDetail", model);
        //    return PartialView("_SaleBillRegisterGrid", model);

        //}
        [HttpGet]
        public IActionResult GetSaleBillRegistergridData()
        {
            string modelJson = HttpContext.Session.GetString("KeySaleBillRegsiterList");
            var data = new List<Dictionary<string, object>>();
            if (!string.IsNullOrEmpty(modelJson))
            {
                data = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(modelJson);
            }
            return Json(data);
        }


        [HttpGet]
        public IActionResult ExportCustomerWisePDF()
        {
            try
            {
                var sessionData = HttpContext.Session.GetString("KeySaleBillRegsiterList");

                if (string.IsNullOrEmpty(sessionData))
                {
                    return Json(new { status = false, message = "No data found" });
                }

                var rows = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(sessionData);

                using (MemoryStream ms = new MemoryStream())
                {
                    iTextSharp.text.Document document = new iTextSharp.text.Document(iTextSharp.text.PageSize.A4.Rotate(), 10, 10, 20, 20);

                    PdfWriter.GetInstance(document, ms);

                    document.Open();

                    iTextSharp.text.Font titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 14);
                    iTextSharp.text.Font headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 9);
                    iTextSharp.text.Font normalFont = FontFactory.GetFont(FontFactory.HELVETICA, 8);
                    iTextSharp.text.Font boldFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 8);

                    // TITLE
                    iTextSharp.text.Paragraph title = new iTextSharp.text.Paragraph(
                        "CUSTOMER WISE SALE BILL REPORT",
                        titleFont
                    );

                    title.Alignment = Element.ALIGN_CENTER;
                    title.SpacingAfter = 10f;

                    document.Add(title);

                    // DATE HEADER
                    string fromDate = HttpContext.Request.Query["FromDate"];
                    string toDate = HttpContext.Request.Query["ToDate"];

                    iTextSharp.text.Paragraph dateHeader = new iTextSharp.text.Paragraph(
                        "From Date : " + fromDate + "    To Date : " + toDate,
                        boldFont
                    );

                    dateHeader.Alignment = Element.ALIGN_CENTER;
                    dateHeader.SpacingAfter = 10f;

                    document.Add(dateHeader);

                    decimal grandQty = 0;
                    decimal grandAmount = 0;

                    // CUSTOMER GROUP
                    var groupedCustomers = rows
                        .GroupBy(x => x.ContainsKey("Customer")
                            ? Convert.ToString(x["Customer"])
                            : "UNKNOWN CUSTOMER")
                        .ToList();

                    foreach (var customer in groupedCustomers)
                    {
                        iTextSharp.text.Paragraph customerName = new iTextSharp.text.Paragraph(
                            "PARTY NAME : " + customer.Key,
                            boldFont
                        );

                        customerName.SpacingBefore = 10f;
                        customerName.SpacingAfter = 5f;

                        document.Add(customerName);

                        decimal partyQty = 0;
                        decimal partyAmount = 0;

                        // ITEM + PARTCODE GROUP
                        var itemGroups = customer
                            .GroupBy(x => new
                            {
                                PartCode = Convert.ToString(x.GetValueOrDefault("PartCode")),
                                ItemName = Convert.ToString(x.GetValueOrDefault("ItemName"))
                            })
                            .ToList();

                        foreach (var itemGroup in itemGroups)
                        {
                            string itemNameText = itemGroup.Key.ItemName;
                            string partCodeText = itemGroup.Key.PartCode;

                            iTextSharp.text.Paragraph itemHeader = new iTextSharp.text.Paragraph(
                                "ITEM NAME : " + itemNameText +
                                "    PART CODE : " + partCodeText,
                                boldFont
                            );

                            itemHeader.SpacingBefore = 5f;
                            itemHeader.SpacingAfter = 5f;

                            document.Add(itemHeader);

                            PdfPTable table = new PdfPTable(8);

                            table.WidthPercentage = 100;

                            table.SetWidths(new float[]
                            {
                        5f,   // SR
                        12f,  // BILL NO
                        12f,  // DATE
                        10f,  // QTY
                        8f,   // UNIT
                        10f,  // RATE
                        10f,  // ASS RATE
                        12f   // AMOUNT
                            });

                            string[] headers =
                            {
                        "SR NO",
                        "BILL NO",
                        "DATE",
                        "QTY",
                        "UNIT",
                        "RATE",
                        "ASS RATE",
                        "AMOUNT"
                    };

                            foreach (var h in headers)
                            {
                                PdfPCell cell = new PdfPCell(new Phrase(h, headerFont));

                                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                                cell.Padding = 4;

                                table.AddCell(cell);
                            }

                            int srNo = 1;

                            decimal itemQty = 0;
                            decimal itemAmount = 0;

                            foreach (var item in itemGroup)
                            {
                                decimal qty = 0;
                                decimal rate = 0;
                                decimal amount = 0;

                                decimal.TryParse(
                                    Convert.ToString(item.GetValueOrDefault("BillQty")),
                                    out qty
                                );

                                decimal.TryParse(
                                    Convert.ToString(item.GetValueOrDefault("Rate")),
                                    out rate
                                );

                                decimal.TryParse(
                                    Convert.ToString(item.GetValueOrDefault("Amount")),
                                    out amount
                                );

                                itemQty += qty;
                                itemAmount += amount;

                                // SR NO
                                table.AddCell(new Phrase(srNo.ToString(), normalFont));

                                // BILL NO
                                table.AddCell(new Phrase(
                                    Convert.ToString(item.GetValueOrDefault("InvoiceNo")),
                                    normalFont
                                ));

                                // DATE
                                table.AddCell(new Phrase(
                                    Convert.ToString(item.GetValueOrDefault("InvoiceDate")),
                                    normalFont
                                ));

                                // QTY
                                PdfPCell qtyCell = new PdfPCell(
                                    new Phrase(qty.ToString("0.00"), normalFont)
                                );

                                qtyCell.HorizontalAlignment = Element.ALIGN_RIGHT;

                                table.AddCell(qtyCell);

                                // UNIT
                                table.AddCell(new Phrase("NOS", normalFont));

                                // RATE
                                PdfPCell rateCell = new PdfPCell(
                                    new Phrase(rate.ToString("0.00"), normalFont)
                                );

                                rateCell.HorizontalAlignment = Element.ALIGN_RIGHT;

                                table.AddCell(rateCell);

                                // ASS RATE
                                PdfPCell assRateCell = new PdfPCell(
                                    new Phrase(rate.ToString("0.00"), normalFont)
                                );

                                assRateCell.HorizontalAlignment = Element.ALIGN_RIGHT;

                                table.AddCell(assRateCell);

                                // AMOUNT
                                PdfPCell amtCell = new PdfPCell(
                                    new Phrase(amount.ToString("0.00"), normalFont)
                                );

                                amtCell.HorizontalAlignment = Element.ALIGN_RIGHT;

                                table.AddCell(amtCell);

                                srNo++;
                            }

                            // ITEM TOTAL ROW
                            PdfPCell totalText = new PdfPCell(
                                new Phrase("TOTAL", boldFont)
                            );

                            totalText.Colspan = 3;
                            totalText.HorizontalAlignment = Element.ALIGN_RIGHT;
                            totalText.Padding = 5;

                            table.AddCell(totalText);

                            PdfPCell totalQtyCell = new PdfPCell(
                                new Phrase(itemQty.ToString("0.00"), boldFont)
                            );

                            totalQtyCell.HorizontalAlignment = Element.ALIGN_RIGHT;

                            table.AddCell(totalQtyCell);

                            table.AddCell(new Phrase("", boldFont));
                            table.AddCell(new Phrase("", boldFont));
                            table.AddCell(new Phrase("", boldFont));

                            PdfPCell totalAmtCell = new PdfPCell(
                                new Phrase(itemAmount.ToString("0.00"), boldFont)
                            );

                            totalAmtCell.HorizontalAlignment = Element.ALIGN_RIGHT;

                            table.AddCell(totalAmtCell);

                            document.Add(table);

                            partyQty += itemQty;
                            partyAmount += itemAmount;
                        }

                        // PARTY TOTAL
                        iTextSharp.text.Paragraph partyTotal = new iTextSharp.text.Paragraph(
                            "PARTY TOTAL    Qty : " +
                            partyQty.ToString("0.00") +
                            "    Amount : " +
                            partyAmount.ToString("0.00"),
                            boldFont
                        );

                        partyTotal.Alignment = Element.ALIGN_RIGHT;
                        partyTotal.SpacingBefore = 5f;
                        partyTotal.SpacingAfter = 10f;

                        document.Add(partyTotal);

                        grandQty += partyQty;
                        grandAmount += partyAmount;
                    }

                    // GRAND TOTAL
                    iTextSharp.text.Paragraph grand = new iTextSharp.text.Paragraph(
                        "GRAND TOTAL    Qty : " +
                        grandQty.ToString("0.00") +
                        "    Amount : " +
                        grandAmount.ToString("0.00"),
                        titleFont
                    );

                    grand.Alignment = Element.ALIGN_RIGHT;
                    grand.SpacingBefore = 10f;

                    document.Add(grand);

                    document.Close();

                    byte[] bytes = ms.ToArray();

                    return File(
                        bytes,
                        "application/pdf",
                        "CustomerWiseSaleBill.pdf"
                    );
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    status = false,
                    message = ex.Message
                });
            }
        }
        public IActionResult GlobalSearch(string searchString, string ReportType, int pageNumber = 1, int pageSize = 10)
        {
            SaleBillRegisterModel model = new SaleBillRegisterModel();
            model.ReportType = ReportType;
            // 1️⃣ Get session data
            string modelJson = HttpContext.Session.GetString("KeySaleBillRegsiterList");

            if (string.IsNullOrWhiteSpace(modelJson))
            {
                model.Rows = new List<Dictionary<string, object>>();
                model.Headers = new List<DashboardColumn>();
                model.TotalRecords = 0;
                return PartialView("_SaleBillRegisterGrid", model);
            }

            // 2️⃣ Deserialize rows
            var allRows = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(modelJson);

            if (allRows == null || allRows.Count == 0)
            {
                model.Rows = new List<Dictionary<string, object>>();
                model.Headers = new List<DashboardColumn>();
                model.TotalRecords = 0;
                return PartialView("_SaleBillRegisterGrid", model);
            }

            // 3️⃣ Dynamic search (all columns)
            var filteredRows = string.IsNullOrWhiteSpace(searchString)
                ? allRows
                : allRows.Where(row =>
                    row.Values.Any(val =>
                        val != null &&
                        val.ToString()
                           .Contains(searchString, StringComparison.OrdinalIgnoreCase)))
                  .ToList();

            // fallback → show all
            if (filteredRows.Count == 0)
                filteredRows = allRows;

            // 4️⃣ Dynamic headers (from keys)
            model.Headers = filteredRows.First()
                .Keys
                .Select(k => new DashboardColumn
                {
                    Title = k,
                    Field = k
                })
                .ToList();
            // 5️⃣ Pagination
            model.TotalRecords = filteredRows.Count;
            model.PageNumber = pageNumber;
            model.PageSize = pageSize;

            model.Rows = filteredRows
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return PartialView("_SaleBillRegisterGrid", model);

        }
        public async Task<IActionResult> SaleBillRegisterToExcel(string ReportType)
        {

            if (ReportType == "SALE Detail (Tally Export-Option 2)")
            {
                // 1. Get data from session
                string json = HttpContext.Session.GetString("KeySaleBillRegsiterList");

                if (string.IsNullOrEmpty(json))
                    return Content("No data available for export.");

                var data = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(json);

                // 2. Load Excel template (MUST be .xlsx)
                var templatePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "templates", "saleswithInventory.xlsx");

                using var workbook = new XLWorkbook(templatePath);

                // 3. Get existing worksheet (change name if needed)
                var worksheet = workbook.Worksheet("Sheet1");

                // 4. Start writing from row 6 (YOUR REQUIREMENT)
                int row = 6;

                if (data != null && data.Count > 0)
                {
                    if (ReportType == "CustomerItemWiseMonthlyData")
                    {
                        var groupedData = data.GroupBy(x => x["CustomerName"]?.ToString());

                        foreach (var group in groupedData)
                        {
                            decimal totalQty = 0;
                            decimal totalAmount = 0;

                            foreach (var item in group)
                            {
                                worksheet.Cell(row, 1).Value = item["CustomerName"]?.ToString();
                                worksheet.Cell(row, 2).Value = item["PartCode"]?.ToString();
                                worksheet.Cell(row, 3).Value = item["ItemName"]?.ToString();

                                decimal qty = Convert.ToDecimal(item["TotalQty"] ?? 0);
                                decimal amount = Convert.ToDecimal(item["TotalAmount"] ?? 0);

                                worksheet.Cell(row, 4).Value = qty;
                                worksheet.Cell(row, 5).Value = amount;

                                totalQty += qty;
                                totalAmount += amount;

                                row++;
                            }

                            // 🔹 Total row
                            worksheet.Cell(row, 1).Value = group.Key + " Total";
                            worksheet.Cell(row, 4).Value = totalQty;
                            worksheet.Cell(row, 5).Value = totalAmount;

                            var totalRange = worksheet.Range(row, 1, row, 5);

                            totalRange.Style.Font.Bold = true;
                            totalRange.Style.Border.TopBorder = XLBorderStyleValues.Thick;
                            totalRange.Style.Border.BottomBorder = XLBorderStyleValues.Thick;
                            totalRange.Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                            totalRange.Style.Border.RightBorder = XLBorderStyleValues.Thick;

                            row++;
                        }
                    }
                    else
                    {
                        // Generic data write
                        foreach (var item in data)
                        {
                            int col = 1;

                            foreach (var key in item.Keys)
                            {
                                var value = item[key];
                                var cell = worksheet.Cell(row, col);

                                if (value == null)
                                {
                                    cell.Value = "";
                                }
                                else if (value is int || value is long)
                                {
                                    cell.Value = Convert.ToInt64(value);
                                }
                                else if (value is decimal || value is double || value is float)
                                {
                                    cell.Value = Convert.ToDecimal(value);
                                }
                                else if (value is DateTime)
                                {
                                    cell.Value = (DateTime)value;
                                    cell.Style.DateFormat.Format = "dd-MMM-yyyy";
                                }
                                else
                                {
                                    cell.Value = value.ToString();
                                }

                                col++;
                            }

                            row++;
                        }
                    }
                }

                // 5. Save and return file
                using var stream = new MemoryStream();
                workbook.SaveAs(stream);
                stream.Position = 0;

                return File(
                    stream.ToArray(),
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    "saleswithInventory.xlsx"
                );
            }
            else
            {
                string json = HttpContext.Session.GetString("KeySaleBillRegsiterList");

                if (string.IsNullOrEmpty(json))
                    return Content("No data available for export.");

                // Deserialize as dynamic list
                var data = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(json);

                using var workbook = new XLWorkbook();
                var worksheet = workbook.Worksheets.Add("SaleBill Register");

                if (data != null && data.Count > 0)
                {
                    int row = 1;

                    if (ReportType == "CustomerItemWiseMonthlyData")
                    {
                        // ✅ Headers
                        var headers = new List<string> { "CustomerName", "PartCode", "ItemName", "TotalQty", "TotalAmount" };

                        for (int col = 0; col < headers.Count; col++)
                        {
                            worksheet.Cell(row, col + 1).Value = headers[col];
                            worksheet.Cell(row, col + 1).Style.Font.Bold = true;
                        }

                        row++;

                        // ✅ Group by AccountName
                        var groupedData = data.GroupBy(x => x["CustomerName"]?.ToString());

                        foreach (var group in groupedData)
                        {
                            decimal totalQty = 0;
                            decimal totalAmount = 0;

                            foreach (var item in group)
                            {
                                worksheet.Cell(row, 1).Value = item["CustomerName"]?.ToString();
                                worksheet.Cell(row, 2).Value = item["PartCode"]?.ToString();
                                worksheet.Cell(row, 3).Value = item["ItemName"]?.ToString();

                                decimal qty = Convert.ToDecimal(item["TotalQty"] ?? 0);
                                decimal amount = Convert.ToDecimal(item["TotalAmount"] ?? 0);

                                worksheet.Cell(row, 4).Value = qty;
                                worksheet.Cell(row, 5).Value = amount;

                                totalQty += qty;
                                totalAmount += amount;

                                row++;
                            }

                            // ✅ Total Row per AccountName
                            worksheet.Cell(row, 1).Value = group.Key + " Total";
                            worksheet.Cell(row, 4).Value = totalQty;
                            worksheet.Cell(row, 5).Value = totalAmount;

                            // Style total row

                            var totalRange = worksheet.Range(row, 1, row, 5);

                            // Bold text
                            totalRange.Style.Font.Bold = true;

                            // ✅ Dark (Thick) Border
                            totalRange.Style.Border.TopBorder = XLBorderStyleValues.Thick;
                            totalRange.Style.Border.BottomBorder = XLBorderStyleValues.Thick;
                            totalRange.Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                            totalRange.Style.Border.RightBorder = XLBorderStyleValues.Thick;
                            worksheet.Range(row, 1, row, 5).Style.Font.Bold = true;

                            row++;
                        }
                    }
                    else
                    {
                        // 🔁 Your existing generic export
                        int col = 1;
                        foreach (var key in data[0].Keys)
                        {
                            worksheet.Cell(1, col).Value = key;
                            worksheet.Cell(1, col).Style.Font.Bold = true;
                            col++;
                        }

                        row = 2;
                        foreach (var item in data)
                        {
                            col = 1;

                            foreach (var value in item.Values)
                            {
                                var cell = worksheet.Cell(row, col);

                                if (value == null)
                                {
                                    cell.Value = "";
                                }
                                else if (value is long || value is int)
                                {
                                    cell.Value = Convert.ToInt64(value);
                                }
                                else if (value is double || value is float || value is decimal)
                                {
                                    cell.Value = Convert.ToDecimal(value);
                                }
                                else if (value is DateTime)
                                {
                                    cell.Value = (DateTime)value;
                                    cell.Style.DateFormat.Format = "dd-MMM-yyyy";
                                }
                                else
                                {
                                    // fallback (string)
                                    if (decimal.TryParse(value.ToString(), out decimal d))
                                    {
                                        cell.Value = d;
                                    }
                                    else
                                    {
                                        cell.Value = value.ToString();
                                    }
                                }

                                col++;
                            }

                            row++;
                        }
                        //foreach (var item in data)
                        //{
                        //    col = 1;
                        //    foreach (var value in item.Values)
                        //    {
                        //        worksheet.Cell(row, col).Value = value?.ToString();
                        //        col++;
                        //    }
                        //    row++;
                        //}
                    }
                }

                //if (data != null && data.Count > 0)
                //{
                //    // 🔥 Add Headers
                //    int col = 1;
                //    foreach (var key in data[0].Keys)
                //    {
                //        worksheet.Cell(1, col).Value = key;
                //        worksheet.Cell(1, col).Style.Font.Bold = true;
                //        col++;
                //    }

                //    // 🔥 Add Rows
                //    int row = 2;
                //    foreach (var item in data)
                //    {
                //        col = 1;
                //        foreach (var value in item.Values)
                //        {
                //            worksheet.Cell(row, col).Value = value?.ToString();
                //            col++;
                //        }
                //        row++;
                //    }
                //}

                worksheet.Columns().AdjustToContents();

                using var stream = new MemoryStream();
                workbook.SaveAs(stream);
                stream.Position = 0;

                return File(
                    stream.ToArray(),
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    "SaleBillRegister.xlsx"
                );

            }
        }

        //public async Task<IActionResult> SaleBillRegisterToExcel(string ReportType)
        //{
        //    string json = HttpContext.Session.GetString("KeySaleBillRegsiterList");

        //    if (string.IsNullOrEmpty(json))
        //        return Content("No data available for export.");

        //    // Deserialize as dynamic list
        //    var data = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(json);

        //    using var workbook = new XLWorkbook();
        //    var worksheet = workbook.Worksheets.Add("SaleBill Register");

        //    if (data != null && data.Count > 0)
        //    {
        //        int row = 1;

        //        if (ReportType == "CustomerItemWiseMonthlyData")
        //        {
        //            // ✅ Headers
        //            var headers = new List<string> { "CustomerName", "PartCode", "ItemName", "TotalQty", "TotalAmount" };

        //            for (int col = 0; col < headers.Count; col++)
        //            {
        //                worksheet.Cell(row, col + 1).Value = headers[col];
        //                worksheet.Cell(row, col + 1).Style.Font.Bold = true;
        //            }

        //            row++;

        //            // ✅ Group by AccountName
        //            var groupedData = data.GroupBy(x => x["CustomerName"]?.ToString());

        //            foreach (var group in groupedData)
        //            {
        //                decimal totalQty = 0;
        //                decimal totalAmount = 0;

        //                foreach (var item in group)
        //                {
        //                    worksheet.Cell(row, 1).Value = item["CustomerName"]?.ToString();
        //                    worksheet.Cell(row, 2).Value = item["PartCode"]?.ToString();
        //                    worksheet.Cell(row, 3).Value = item["ItemName"]?.ToString();

        //                    decimal qty = Convert.ToDecimal(item["TotalQty"] ?? 0);
        //                    decimal amount = Convert.ToDecimal(item["TotalAmount"] ?? 0);

        //                    worksheet.Cell(row, 4).Value = qty;
        //                    worksheet.Cell(row, 5).Value = amount;

        //                    totalQty += qty;
        //                    totalAmount += amount;

        //                    row++;
        //                }

        //                // ✅ Total Row per AccountName
        //                worksheet.Cell(row, 1).Value = group.Key + " Total";
        //                worksheet.Cell(row, 4).Value = totalQty;
        //                worksheet.Cell(row, 5).Value = totalAmount;

        //                // Style total row

        //                var totalRange = worksheet.Range(row, 1, row, 5);

        //                // Bold text
        //                totalRange.Style.Font.Bold = true;

        //                // ✅ Dark (Thick) Border
        //                totalRange.Style.Border.TopBorder = XLBorderStyleValues.Thick;
        //                totalRange.Style.Border.BottomBorder = XLBorderStyleValues.Thick;
        //                totalRange.Style.Border.LeftBorder = XLBorderStyleValues.Thick;
        //                totalRange.Style.Border.RightBorder = XLBorderStyleValues.Thick;
        //                worksheet.Range(row, 1, row, 5).Style.Font.Bold = true;

        //                row++;
        //            }
        //        }
        //        else
        //        {
        //            // 🔁 Your existing generic export
        //            int col = 1;
        //            foreach (var key in data[0].Keys)
        //            {
        //                worksheet.Cell(1, col).Value = key;
        //                worksheet.Cell(1, col).Style.Font.Bold = true;
        //                col++;
        //            }

        //            row = 2;
        //            foreach (var item in data)
        //            {
        //                col = 1;

        //                foreach (var value in item.Values)
        //                {
        //                    var cell = worksheet.Cell(row, col);

        //                    if (value == null)
        //                    {
        //                        cell.Value = "";
        //                    }
        //                    else if (value is long || value is int)
        //                    {
        //                        cell.Value = Convert.ToInt64(value);
        //                    }
        //                    else if (value is double || value is float || value is decimal)
        //                    {
        //                        cell.Value = Convert.ToDecimal(value);
        //                    }
        //                    else if (value is DateTime)
        //                    {
        //                        cell.Value = (DateTime)value;
        //                        cell.Style.DateFormat.Format = "dd-MMM-yyyy";
        //                    }
        //                    else
        //                    {
        //                        // fallback (string)
        //                        if (decimal.TryParse(value.ToString(), out decimal d))
        //                        {
        //                            cell.Value = d;
        //                        }
        //                        else
        //                        {
        //                            cell.Value = value.ToString();
        //                        }
        //                    }

        //                    col++;
        //                }

        //                row++;
        //            }
        //            //foreach (var item in data)
        //            //{
        //            //    col = 1;
        //            //    foreach (var value in item.Values)
        //            //    {
        //            //        worksheet.Cell(row, col).Value = value?.ToString();
        //            //        col++;
        //            //    }
        //            //    row++;
        //            //}
        //        }
        //    }

        //    //if (data != null && data.Count > 0)
        //    //{
        //    //    // 🔥 Add Headers
        //    //    int col = 1;
        //    //    foreach (var key in data[0].Keys)
        //    //    {
        //    //        worksheet.Cell(1, col).Value = key;
        //    //        worksheet.Cell(1, col).Style.Font.Bold = true;
        //    //        col++;
        //    //    }

        //    //    // 🔥 Add Rows
        //    //    int row = 2;
        //    //    foreach (var item in data)
        //    //    {
        //    //        col = 1;
        //    //        foreach (var value in item.Values)
        //    //        {
        //    //            worksheet.Cell(row, col).Value = value?.ToString();
        //    //            col++;
        //    //        }
        //    //        row++;
        //    //    }
        //    //}

        //    worksheet.Columns().AdjustToContents();

        //    using var stream = new MemoryStream();
        //    workbook.SaveAs(stream);
        //    stream.Position = 0;

        //    return File(
        //        stream.ToArray(),
        //        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
        //        "SaleBillRegister.xlsx"
        //    );
        //}
        public async Task<JsonResult> FillCustomerList(string FromDate, string ToDate, string SearchText)
        {
            var JSON = await _ISaleBillRegister.FillCustomerList(FromDate, ToDate, SearchText);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillItemGroupList(string FromDate, string ToDate, string SearchText)
        {
            var JSON = await _ISaleBillRegister.FillItemGroupList(FromDate, ToDate, SearchText);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillDocumentList(string FromDate, string ToDate)
        {

            var JSON = await _ISaleBillRegister.FillDocumentList(FromDate, ToDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillSaleBillList(string FromDate, string ToDate, string SearchText)
        {
            var JSON = await _ISaleBillRegister.FillSaleBillList(FromDate, ToDate, SearchText);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillItemNamePartcodeList(string FromDate, string ToDate, string PartCode, string ItemName)
        {
            var JSON = await _ISaleBillRegister.FillItemNamePartcodeList(FromDate, ToDate, PartCode, ItemName);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillSONO(string FromDate, string ToDate, string SearchText)
        {
            var JSON = await _ISaleBillRegister.FillSONO(FromDate, ToDate, SearchText);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillSchNo(string FromDate, string ToDate, string SearchText)
        {
            var JSON = await _ISaleBillRegister.FillSchNo(FromDate, ToDate, SearchText);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillHSNNo(string FromDate, string ToDate, string SearchText)
        {
            var JSON = await _ISaleBillRegister.FillHSNNo(FromDate, ToDate, SearchText);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillGSTNo(string FromDate, string ToDate, string SearchText)
        {
            var JSON = await _ISaleBillRegister.FillGSTNo(FromDate, ToDate, SearchText);
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
