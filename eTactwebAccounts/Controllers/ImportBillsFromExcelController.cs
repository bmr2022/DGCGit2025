using DocumentFormat.OpenXml.EMMA;
using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using System.Net;

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

        public async Task<IActionResult> ImportBill(string ReportType)
        {

            int ForFinYear= Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
            int CreatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));

            var Result = await _IImportBillsFromExcel.ImportBills(ReportType, ForFinYear, CreatedBy);


            if (Result != null)
            {
                try
                {
                    if (Result.StatusText == "Success" && Result.StatusCode == HttpStatusCode.OK)
                    {
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
                    else if (!string.IsNullOrEmpty(Result.StatusText))
                    {
                        // If SP returned a message (like adjustment error)
                        TempData["ErrorMessage"] = Result.StatusText;
                        return Json(new
                        {
                            status = "Error",
                            message = Result.StatusText,
                            

                        });
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError("\n \n ********** Exception in SaleInvoice Action ********** \n " + ex.ToString() + "\n \n");
                    return Json(new
                    {
                        status = "Error",
                        message = "An error occurred while processing the sale invoice. Please try again later."
                    });
                }


            }
            return Json(new
            {
                status = "Error",
                message = "Invalid Server Error",


            });

        }
    }
}
