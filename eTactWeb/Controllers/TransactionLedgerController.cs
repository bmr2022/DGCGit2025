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
		public async Task<ActionResult> TransactionLedger(string ReportType = "", int Accountcode = 0)
		{
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

			decimal totalDr = 0;
			decimal totalCr = 0;

			return Document.Create(container =>
			{
				container.Page(page =>
				{
					page.Size(PageSizes.A4);
					page.Margin(20);
					page.DefaultTextStyle(x => x.FontSize(9));

					page.Content().Column(col =>
					{
						// 🔷 HEADER
						col.Item().AlignCenter().Text("DGC INDUSTRIES")
							.FontSize(14).Bold();

						col.Item().AlignCenter().Text("LEDGER VOUCHER")
							.FontSize(12).Bold();

						col.Item().AlignCenter()
							.Text($"From {data.First().FromDate} To {data.First().ToDate}");
						col.Item().AlignCenter()
							.Text(data.First().AccountName);

						col.Item().PaddingVertical(10);

						// 🔷 TABLE
						col.Item().Table(table =>
						{
							table.ColumnsDefinition(columns =>
							{
								columns.ConstantColumn(80);   // Voucher No (13-14 chars safe)
								columns.ConstantColumn(65);   // Date
								columns.RelativeColumn(2);    // Description (auto adjust)
								columns.RelativeColumn(2);    // Narration (auto adjust)
								columns.ConstantColumn(55);   // Type
								columns.ConstantColumn(70);   // DR
								columns.ConstantColumn(70);   // CR
								columns.ConstantColumn(80);   // Balance
							});

							// 🔹 HEADER
							table.Header(header =>
							{
								header.Cell().Border(1).Padding(4).Text("VCH NO").Bold();
								header.Cell().Border(1).Padding(4).Text("DATE").Bold();
								header.Cell().Border(1).Padding(4).Text("DESCRIPTION").Bold();

								header.Cell().Border(1).Padding(4).Text("TYPE").Bold();
								header.Cell().Border(1).Padding(4).AlignRight().Text("DR").Bold();
								header.Cell().Border(1).Padding(4).AlignRight().Text("CR").Bold();
								header.Cell().Border(1).Padding(4).AlignRight().Text("BALANCE").Bold();
								header.Cell().Border(1).Padding(4).Text("NARRATION").Bold();
							});

							// 🔹 DATA ROWS
							foreach (var row in data)
							{
								totalDr += row.DrAmt;
								totalCr += row.CrAmt;

								string description =
									(row.Particulars ?? "") +
									(string.IsNullOrWhiteSpace(row.SumDet)
										? ""
										: " - " + row.SumDet);   // Add Remark in description

								table.Cell().Border(0.5f).Padding(3)
									.Text(row.VchNo ?? "");

								table.Cell().Border(0.5f).Padding(3)
									.Text(row.VoucherDocDate ?? "");

								table.Cell().Border(0.5f).Padding(3)
									.Text(description);



								table.Cell().Border(0.5f).Padding(3)
									.Text(row.VoucherType ?? "");

								table.Cell().Border(0.5f).Padding(3)
									.AlignRight()
									.Text(row.DrAmt == 0 ? "" : row.DrAmt.ToString("N2"));

								table.Cell().Border(0.5f).Padding(3)
									.AlignRight()
									.Text(row.CrAmt == 0 ? "" : row.CrAmt.ToString("N2"));

								table.Cell().Border(0.5f).Padding(3)
									.AlignRight()
									.Text(row.Balance.ToString("N2"));
								table.Cell().Border(0.5f).Padding(3)
									.Text(row.HeadWiseNarration ?? "");
							}

							// 🔹 TOTAL ROW
							table.Cell().ColumnSpan(4)
								.BorderTop(2)
								.Padding(5)
								.AlignRight()
								.Text("TOTAL")
								.Bold();

							table.Cell().BorderTop(2)
								.Padding(5)
								.AlignRight()
								.Text(totalDr.ToString("N2"))
								.Bold();

							table.Cell().BorderTop(2)
								.Padding(5)
								.AlignRight()
								.Text(totalCr.ToString("N2"))
								.Bold();

							table.Cell().BorderTop(2)
								.Padding(5)
								.AlignRight()
								.Text("")
								.Bold();
						});
					});

					// 🔷 FOOTER
					page.Footer()
						.AlignCenter()
						.Text(x =>
						{
							x.Span("Page ");
							x.CurrentPageNumber();
						});
				});
			}).GeneratePdf();
		}
		public async Task<IActionResult> DownloadLedgerPdf(
	string FromDate, string ToDate, string ReportType,
	string GroupOrLedger, int? ParentAccountCode,
	int? AccountCode, string VoucherType,
	string VoucherNo, string InvoiceNo,
	string Narration, float? Amount,
	string DR, string CR, string Ledger, string AccountName)
		{
			var data = await _TransactionLedger.GetDetailsData(
				FromDate, ToDate, ReportType,
				GroupOrLedger, ParentAccountCode,
				AccountCode, VoucherType,
				VoucherNo, InvoiceNo,
				Narration, Amount, DR, CR, Ledger, AccountName);

			if (data.TransactionLedgerGrid == null || !data.TransactionLedgerGrid.Any())
				return NotFound("No data available.");

			//var generator = new LedgerPdfGenerator();
			var pdfBytes = GenerateLedgerPdf(data.TransactionLedgerGrid);

			return File(pdfBytes, "application/pdf", "LedgerVoucher.pdf");
		}

		public async Task<IActionResult> GetDetailsData(
			string FromDate = null, string ToDate = null, string ReportType = null,
			string GroupOrLedger = null, int? ParentAccountCode = null, int? AccountCode = null,
			string VoucherType = null, string VoucherNo = null, string InvoiceNo = null,
			string Narration = null, float? Amount = null, string DR = null, string CR = null,
			string Ledger = null, string AccountName = null)
		{
			var model = await _TransactionLedger.GetDetailsData(
				FromDate, ToDate, ReportType, GroupOrLedger,
				ParentAccountCode, AccountCode, VoucherType,
				VoucherNo, InvoiceNo, Narration, Amount, DR, CR, Ledger, AccountName
			);

			return PartialView("_TransactionLedgerGrid", model);
		}
		public async Task<IActionResult> GetTransactionLedgerMonthlySummaryDetailsData(string FromentryDate, string ToEntryDate, int AccountCode, string ReportType)
		{
			var model = new TransactionLedgerModel();
			model = await _TransactionLedger.GetTransactionLedgerMonthlySummaryDetailsData(FromentryDate, ToEntryDate, AccountCode);
			var sessionData = JsonConvert.SerializeObject(model);
			HttpContext.Session.SetString("TransactionLedgerData", sessionData);
			return PartialView("_TransactionLedgerMonthlySummaryGrid", model);

		}
		public async Task<IActionResult> GetTransactionLedgerGroupSummaryDetailsData(string FromDate, string ToDate, string ReportType, string GroupOrLedger, int? ParentAccountCode = null, int AccountCode = 0, string? VoucherType = null, string? VoucherNo = null, string? InvoiceNo = null, string? Narration = null, float? Amount = null, string? DR = null, string? CR = null, string? Ledger = null)
		{
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
		public IActionResult ExportTransactionLedgerToExcel(string ReportType, string FromDate, string ToDate)
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
