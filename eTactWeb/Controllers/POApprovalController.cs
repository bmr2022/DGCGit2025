using DocumentFormat.OpenXml.Office2010.Excel;
using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using FastReport;
using FastReport.Export.PdfSimple;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using PdfSharp.Drawing.BarCodes;
using System.Collections.Generic;
using System.Net.Http;
using System.Web;

namespace eTactWeb.Controllers
{
    public class POApprovalController : Controller
    {
        private readonly IDataLogic _IDataLogic;
        private readonly HttpClient _httpClient;
        private readonly IPOApproval _IPOApproval;
        private readonly ILogger<POApprovalController> _logger;
        private readonly IWebHostEnvironment _IWebHostEnvironment;
        private readonly ConnectionStringService _connectionStringService;
        private const string BaseUrl = "http://bhashsms.com/api/sendmsgutil.php";

        public POApprovalController(ILogger<POApprovalController> logger, IDataLogic iDataLogic, IPOApproval iPOApproval, IWebHostEnvironment iWebHostEnvironment, ConnectionStringService connectionStringService, HttpClient httpClient)
        {
            _logger = logger;
            _IDataLogic = iDataLogic;
            _IPOApproval = iPOApproval;
            _IWebHostEnvironment = iWebHostEnvironment;
            _connectionStringService = connectionStringService;
            _httpClient = httpClient;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Pdf(int EntryId, int YearCode, string PONO)
        {
            try
            {
                string webRootPath = _IWebHostEnvironment.WebRootPath;
                var report = new Report(); // Use the Report class instead of WebReport for exports

                // Get the report name (await properly if async)
                var ReportNameResult = _IPOApproval.GetReportName().Result.Result;
                string reportFileName = !string.Equals(ReportNameResult.Rows[0].ItemArray[0], DBNull.Value)
                    ? ReportNameResult.Rows[0].ItemArray[0].ToString()
                    : "PO"; // Default to "PO.frx"

                string reportPath = Path.Combine(webRootPath, $"{reportFileName}.frx");

                // Ensure the report file exists
                if (!System.IO.File.Exists(reportPath))
                    throw new FileNotFoundException($"Report file not found: {reportPath}");

                // Load the report
                report.Load(reportPath);

                // Set parameters
                report.SetParameterValue("entryparam", EntryId);
                report.SetParameterValue("yearparam", YearCode);
                report.SetParameterValue("ponoparam", PONO);

                // Set connection string
                string myConnectionString = _connectionStringService.GetConnectionString();
                //string myConnectionString = _iconfiguration.GetConnectionString("eTactDB");
                report.SetParameterValue("MyParameter", myConnectionString);

                // Prepare the report (generate data)
                report.Prepare();

                // Export to PDF
                using (MemoryStream ms = new MemoryStream())
                {
                    PDFSimpleExport pdfExport = new PDFSimpleExport();
                    pdfExport.Export(report, ms);
                    ms.Position = 0;

                    return File(ms.ToArray(), "application/pdf", $"PurchaseOrder_{EntryId}.pdf");
                }
            }
            catch (Exception ex)
            {
                // Log the error (e.g., using ILogger)
                return StatusCode(500, $"Error generating PDF: {ex.Message}");
            }
        }
        [Route("{controller}/Index")]
        [HttpGet]
        public IActionResult POApproval()
        {
            var model = new POApprovalModel();
            model.CC = HttpContext.Session.GetString("Branch");
            model.FromDate = HttpContext.Session.GetString("FromDate");

            return View("POApproval", model);
        }
        public async Task<JsonResult> GetInitialData()
        {
            int EmpID = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
            string UID = HttpContext.Session.GetString("UID");
            var JSON = await _IPOApproval.GetInitialData("UNAPPROVEDFIRSTLVLPOSUMM", UID, EmpID);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetFinalApprovalPO()
        {
            int EmpID = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
            string UID = HttpContext.Session.GetString("UID");
            var JSON = await _IPOApproval.GetInitialData("UNAPPROVEDFINALAPPPOSUMM", UID, EmpID);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetAmmApprovalPO()
        {
            int EmpID = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
            string UID = HttpContext.Session.GetString("UID");
            var JSON = await _IPOApproval.GetInitialData("UNAPPROVEDAmendmentPOSUMM", UID, EmpID);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetAmmUnApprovalPO()
        {
            int EmpID = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
            string UID = HttpContext.Session.GetString("UID");
            var JSON = await _IPOApproval.GetInitialData("APPROVEDAmendmentPO", UID, EmpID);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetFinalUnApprovalPO()
        {
            int EmpID = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
            string UID = HttpContext.Session.GetString("UID");
            var JSON = await _IPOApproval.GetInitialData("APPROVEDFinallevelPO", UID, EmpID);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetFirstLvlUnApprovalPO()
        {
            int EmpID = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
            string UID = HttpContext.Session.GetString("UID");
            var JSON = await _IPOApproval.GetInitialData("APPROVEDFirstlevelPO", UID, EmpID);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetAllowedAction()
        {
            int EmpID = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
            var JSON = await _IPOApproval.GetAllowedAction("GetAllowedAction", EmpID);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }

        public async Task<JsonResult> GetSearchData(string FromDate, string ToDate, string ApprovalType, string PONO, string VendorName)
        {
            int EmpID = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
            string UID = HttpContext.Session.GetString("UID");
            var JSON = await _IPOApproval.GetSearchData(FromDate, ToDate, ApprovalType, PONO, VendorName, EmpID, UID);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }

        [HttpGet]
        public async Task<IActionResult> ShowPODetail(int ID, int YC, string PONo, string TypeOfApproval, string showonlyamenditem)
        {
            HttpContext.Session.Remove("KeyPoApprovalDetail");
            var MainModel = new List<POApprovalDetail>();
            MainModel = await _IPOApproval.ShowPODetail(ID, YC, PONo, TypeOfApproval, showonlyamenditem).ConfigureAwait(true);
            //string JsonString = JsonConvert.SerializeObject(JSON);
            return View(MainModel);
        }
        public async Task<JsonResult> GetFeatureOption()
        {
            var JSON = await _IPOApproval.GetFeaturesOptions();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        
        [HttpGet]
        public async Task<IActionResult> SaveApproval(int EntryId, int YC, string PONO, string type)
        {
            int EmpID = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));

            var featuresResponse = await _IPOApproval.GetFeaturesOptions();
         
            string companyIp = "";
            if (featuresResponse != null && featuresResponse.Result is DataTable dt1)
            {
                if (dt1.Rows.Count > 0)
                {
                    companyIp = dt1.Rows[0]["CompanyIPAddforwhatappMessage"].ToString();
                }
            }
            if (!string.IsNullOrEmpty(companyIp))
            {
                companyIp = companyIp.Replace("\\", "/");
            }


            var pdfResult = Pdf(EntryId, YC, PONO) as FileContentResult;
            byte[] pdfBytes = pdfResult.FileContents;

            // Save to server folder
            string uploadsFolder = Path.Combine(_IWebHostEnvironment.WebRootPath, "uploads", $"PurchaseOrder_{YC}");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            // Sanitize PONO for file name (remove invalid chars like / \ : * ? " < > |)
            string safePONO = string.Join("_", PONO.Split(Path.GetInvalidFileNameChars()));
            string fileName = $"{safePONO}.pdf";
            string filePath = Path.Combine(uploadsFolder, fileName);

            // Save PDF locally
            System.IO.File.WriteAllBytes(filePath, pdfBytes);
            companyIp = companyIp.TrimEnd('/');
            string fileUrl = $"{companyIp}/uploads/PurchaseOrder_{YC}/{fileName}";
         
            var MobileNoResponse = await _IPOApproval.GetMobileNo(EntryId, YC, PONO);

            string MobileNo = "";
            if (MobileNoResponse != null && MobileNoResponse.Result is DataTable ds2)
            {
                if (ds2.Rows.Count > 0)
                {
                    MobileNo = ds2.Rows[0]["MobileNo"].ToString();
                }
            }

           // string fileUrl = companyIp + fileName;
            //$"{Request.Scheme}://{Request.Host}/uploads/PurchaseOrder_{YC}/{fileName}";

         
            var queryParams = HttpUtility.ParseQueryString(string.Empty);
            queryParams["user"] = "XON_Cybernetics";
            queryParams["pass"] = "123456";
            queryParams["sender"] = "BUZWAP";
            queryParams["phone"] = MobileNo;
            queryParams["text"] = "xon_temp";
            queryParams["priority"] = "wa";
            queryParams["stype"] = "normal";
            queryParams["htype"] = "document";
            queryParams["fname"] = "PDF File";
            queryParams["url"] = fileUrl;

            var apiUrl = $"{BaseUrl}?{queryParams}";

            var response = await _httpClient.GetAsync(apiUrl);
            var responseContent = await response.Content.ReadAsStringAsync();
          
            var Result = await _IPOApproval.SaveApproval(EntryId, YC, PONO, type, EmpID);

            if (Result != null)
            {
                if (Result.StatusText == "Success")
                {
                    var status = "";
                    var message = "";

                    var ds = (DataSet)Result.Result;
                    if (ds.Tables.Count > 0)
                    {
                        var dt = ds.Tables[0];
                        if (dt.Rows.Count > 0)
                        {
                            status = Convert.ToString(dt.Rows[0][0]);
                            if (dt.Columns.Count > 1)
                                message = dt.Rows[0][1].ToString();
                        }
                    }


                    var model1 = new POApprovalModel();
                    ViewBag.isSuccess = true;
                    if (string.IsNullOrEmpty(message))
                        TempData["200"] = "200";
                    else
                    {
                        TempData["302"] = message;
                    }
                    //return View("POApproval", model1);
                    return RedirectToAction("POApproval");
                }
            }
            var model = new POApprovalModel();
            //return View("POApproval", model);
            return RedirectToAction("POApproval");
        }

    }
}
