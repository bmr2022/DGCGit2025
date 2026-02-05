using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using ClosedXML.Excel;
using System.Text.RegularExpressions;

namespace eTactwebAccounts.Controllers
{
    public class ImportToTallyReportController : Controller
    {
        private readonly IDataLogic _IDataLogic;
        public IImportToTallyReport _IImportToTallyReport { get; }
        private readonly ILogger<ImportToTallyReportController> _logger;
        private readonly IConfiguration iconfiguration;
        public IWebHostEnvironment _IWebHostEnvironment { get; }

        public ImportToTallyReportController(ILogger<ImportToTallyReportController> logger, IDataLogic iDataLogic, EncryptDecrypt encryptDecrypt, IWebHostEnvironment iWebHostEnvironment, IConfiguration iconfiguration, IImportToTallyReport IImportToTallyReport)
        {
            _logger = logger;
            _IDataLogic = iDataLogic;
            _IImportToTallyReport = IImportToTallyReport;
            _IWebHostEnvironment = iWebHostEnvironment;
            this.iconfiguration = iconfiguration;
        }
        [Route("{controller}/Index")]
        public IActionResult ImportToTallyReport()
        {
           
            return View();
        }



        public async Task<IActionResult> GetReportData(string Flag, string FromDate, string Todate,int AccountCode,string BillNo)
        {
            var result = await _IImportToTallyReport.GetReportData(Flag, FromDate, Todate, AccountCode, BillNo);

            if (result == null || !(result.Result is DataTable dt) || dt.Rows.Count == 0)
            {
                return BadRequest("No data found");
            }
            // 🔹 Build safe file name parts
            string safeFlag = string.IsNullOrWhiteSpace(Flag) ? "Report" : Flag;
            string safeBillNo = string.IsNullOrWhiteSpace(BillNo) ? "" : $"_Bill-{BillNo}";
            string safeAccount = AccountCode > 0 ? $"_Acc-{AccountCode}" : "";

            // Remove invalid filename characters
            string MakeSafe(string input) =>
                Regex.Replace(input, @"[\\/:*?""<>|]", "");

            string fileName =
                $"{MakeSafe(safeFlag)}" +
                $"_{FromDate}_to_{Todate}" +
                $"{safeAccount}" +
                $"{MakeSafe(safeBillNo)}" +
                ".xlsx";

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Report");

                // This line handles headers + rows automatically
                worksheet.Cell(1, 1).InsertTable(dt);

                worksheet.Columns().AdjustToContents();

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    stream.Position = 0;

                    return File(
                stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileName
            );
                }
            }
        }
    }
}
