using eTactWeb.Data.Common;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace eTactwebAccounts.Controllers
{
    public class ImportBillsFromExcelController : Controller
    {
        private readonly IDataLogic _IDataLogic;
        public IImportBillsFromExcel _IImportBillsFromExcel { get; }
        private readonly ILogger<ImportBillsFromExcelController> _logger;
        private readonly IConfiguration iconfiguration;
        public IWebHostEnvironment _IWebHostEnvironment { get; }

        public ImportBillsFromExcelController(ILogger<ImportBillsFromExcelController> logger, IDataLogic iDataLogic, EncryptDecrypt encryptDecrypt, IWebHostEnvironment iWebHostEnvironment, IConfiguration iconfiguration, IImportBillsFromExcel IImportBillsFromExcel)
        {
            _logger = logger;
            _IDataLogic = iDataLogic;
            _IImportBillsFromExcel = IImportBillsFromExcel;
            _IWebHostEnvironment = iWebHostEnvironment;
            this.iconfiguration = iconfiguration;
        }
        public IActionResult ImportBillsFromExcel()
        {
            return View();
        }

        [HttpPost]

        public IActionResult ImportBill(string ReportType)
        {

            int ForFinYear= Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
            int CreatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));

            var result = _IImportBillsFromExcel.ImportBills(ReportType, ForFinYear, CreatedBy);
            return Json(new
            {
                success = true,
                message = "Bill imported successfully",
                redirectUrl = Url.Action(
      "ImportBillsFromExcel",
      "ImportBillsFromExcel"

  )
            });

        }
    }
}
