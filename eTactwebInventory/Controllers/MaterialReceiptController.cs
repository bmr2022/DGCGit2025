using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using FastReport.Export.Html;
using FastReport.Export.Image;
using FastReport.Web;
using FastReport;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using static eTactWeb.Data.Common.CommonFunc;
using static eTactWeb.DOM.Models.Common;
using static eTactWeb.DOM.Models.MaterialReceiptModel;
using static Grpc.Core.Metadata;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Data;
using System.Globalization;
using FastReport.Data;
using System.Configuration;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Runtime.Caching;
using DocumentFormat.OpenXml.Bibliography;
using System.Drawing.Printing;
using ClosedXML.Excel;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System.Drawing;
using eTactWeb.Services;
using MimeKit;
using MailKit.Net.Smtp;



namespace eTactWeb.Controllers
{
    public class MaterialReceiptController : Controller
    {
        public IMaterialReceipt _MRN { get; }
        private readonly IDataLogic _IDataLogic;
        private readonly IMaterialReceipt _IMaterialReceipt;
        private readonly ILogger<MaterialReceiptController> _logger;
        private readonly IConfiguration _iconfiguration;
        private readonly IMemoryCache _MemoryCache;
        private readonly IEmailService _emailService;
        public IWebHostEnvironment _IWebHostEnvironment { get; }
        private readonly ConnectionStringService _connectionStringService;
        public MaterialReceiptController(ILogger<MaterialReceiptController> logger, IDataLogic iDataLogic, IMaterialReceipt iMaterialReceipt, IWebHostEnvironment iWebHostEnvironment, IConfiguration iconfiguration, IMemoryCache iMemoryCache, IEmailService emailService, ConnectionStringService connectionStringService)
        {
            _logger = logger;
            _IDataLogic = iDataLogic;
            _IMaterialReceipt = iMaterialReceipt;
            _IWebHostEnvironment = iWebHostEnvironment;
            this._iconfiguration = iconfiguration;
            _MemoryCache = iMemoryCache;
            _emailService = emailService;
            _connectionStringService = connectionStringService;
        }

        [HttpPost]

        public async Task<JsonResult> GenerateMultiMRNPrint(string MRNNo, int YearCode)
        {
            YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
            var JSON = await _IMaterialReceipt.GenerateMultiMRNPrint(MRNNo, YearCode);

            // if SP saved successfully, build PrintReport URL
            if (JSON != null && JSON.Result != null && JSON.Result.Tables.Count > 0 && JSON.Result.Tables[0].Rows.Count > 0)
            {
                // Example: take EntryId from your SP result (adjust column name accordingly)
                //int entryId = Convert.ToInt32(JSON.Result.Tables[0].Rows[0]["EntryId"]);

                // Build PrintReport URL
                string reportUrl = Url.Action("PrintMultiMRN", "MaterialReceipt", new
                {
                    MRNNo = MRNNo,
                    YearCode = YearCode

                }, protocol: Request.Scheme);

                return Json(new { url = reportUrl });// return URL to AJAX
            }

            return Json(null);
        }
        public IActionResult PrintMultiMRN(string MRNNo, int YearCode)
        {
            string my_connection_string;
            string contentRootPath = _IWebHostEnvironment.ContentRootPath;
            string webRootPath = _IWebHostEnvironment.WebRootPath;
            var webReport = new WebReport();
            webReport.Report.Clear();
            var ReportName = _IMaterialReceipt.GetReportName();
            webReport.Report.Dispose();
            webReport.Report = new Report();

            //webReport.Report.Load(webRootPath + "\\MRNMultiReportYauto.frx");
            if (!String.Equals(ReportName.Result.Result.Rows[0].ItemArray[0], System.DBNull.Value))
            {
                webReport.Report.Load(webRootPath + "\\" + ReportName.Result.Result.Rows[0].ItemArray[0].ToString() + ".frx");
            }
            else
            {
                webReport.Report.Load(webRootPath + "\\MRNMultiReportYauto.frx"); 
            }
            my_connection_string = _connectionStringService.GetConnectionString();
            //my_connection_string = _iconfiguration.GetConnectionString("eTactDB");
            webReport.Report.Dictionary.Connections[0].ConnectionString = my_connection_string;
            webReport.Report.Dictionary.Connections[0].ConnectionStringExpression = "";
            webReport.Report.SetParameterValue("MrnNoparam", MRNNo);
            webReport.Report.SetParameterValue("MrnYearcodeparam", YearCode);

            webReport.Report.SetParameterValue("MyParameter", my_connection_string);
            webReport.Report.Refresh();
            return View(webReport);
        }
       
 

public IActionResult PrintReport(int EntryId = 0, int YearCode = 0, string MrnNo = "")
        {
            string my_connection_string;
            string contentRootPath = _IWebHostEnvironment.ContentRootPath;
            string webRootPath = _IWebHostEnvironment.WebRootPath;
            var webReport = new WebReport();
            webReport.Report.Clear();
            var ReportName = _IMaterialReceipt.GetReportName();
            webReport.Report.Dispose();
            webReport.Report = new Report();
            if (!String.Equals(ReportName.Result.Result.Rows[0].ItemArray[0], System.DBNull.Value))
            {
                webReport.Report.Load(webRootPath + "\\" + ReportName.Result.Result.Rows[0].ItemArray[0].ToString() + ".frx");
            }
            else
            {
                webReport.Report.Load(webRootPath + "\\MRN.frx"); // default report
            }
            my_connection_string = _connectionStringService.GetConnectionString();
            //my_connection_string = _iconfiguration.GetConnectionString("eTactDB");
            webReport.Report.Dictionary.Connections[0].ConnectionString = my_connection_string;
            webReport.Report.Dictionary.Connections[0].ConnectionStringExpression = "";
            webReport.Report.SetParameterValue("MrnNoparam", MrnNo);
            webReport.Report.SetParameterValue("MrnYearcodeparam", YearCode);
            webReport.Report.SetParameterValue("MyParameter", my_connection_string);
            webReport.Report.Refresh();
            return View(webReport);
        }

        public IActionResult SendReport(int EntryId = 0, int YearCode = 0, string MrnNo = "", int AccountCode = 0, string emailTo = "", string CC1 = "", string CC2 = "", string CC3 = "")
        {
            string my_connection_string;
            string contentRootPath = _IWebHostEnvironment.ContentRootPath;
            string webRootPath = _IWebHostEnvironment.WebRootPath;
            var webReport = new WebReport();
            webReport.Report.Clear();
            // var ReportName = _IMaterialReceipt.GetReportName();
            webReport.Report.Dispose();
            webReport.Report = new Report();

            webReport.Report.Load(webRootPath + "\\MRNShortExcess.frx"); // default repor 
            my_connection_string = _connectionStringService.GetConnectionString();
            webReport.Report.Dictionary.Connections[0].ConnectionString = my_connection_string;
            webReport.Report.Dictionary.Connections[0].ConnectionStringExpression = "";
            //webReport.Report.SetParameterValue("MrnNoparam", MrnNo);
            webReport.Report.SetParameterValue("MrnYearcodeparam", YearCode);
            webReport.Report.SetParameterValue("entryidparam", EntryId);
            webReport.Report.SetParameterValue("accountcodeparam", AccountCode);
            webReport.Report.SetParameterValue("MyParameter", my_connection_string);
            webReport.Report.Refresh();


            // Now call EmailReport
            return EmailReport(webReport, emailTo, CC1, CC2, CC3, MrnNo);
        }


        public IActionResult EmailReport(WebReport webReport, string emailTo, string CC1, string CC2, string CC3,string MRNNo)
        {
            try
            {
                webReport.Report.Prepare(); // Prepare the report before exporting
                // First export the report to an image
                using (MemoryStream imageStream = new MemoryStream())
                {
                    // Configure image export
                    var imageExport = new ImageExport()
                    {
                        ImageFormat = ImageExportFormat.Png, // Force PNG format
                        Resolution = 300, // Higher quality
                        //ExportQuality = 100 // Maximum quality
                    };



                    // Export the report
                    webReport.Report.Export(imageExport, imageStream);
                    imageStream.Position = 0;

                    // Verify the image data
                    if (imageStream.Length == 0)
                        throw new Exception("Report export failed - empty image stream");

                    // Convert to PDF with additional validation
                    byte[] pdfBytes;
                    try
                    {
                        pdfBytes = ConvertImageToPdf(imageStream.ToArray());
                    }
                    catch (Exception ex)
                    {
                        // Try alternative conversion if first attempt fails
                        pdfBytes = ConvertImageToPdf(imageStream.ToArray());
                    }
                    //emailTo = "infotech.bmr@gmail.com,bmr.client2021@gmail.com";
                    emailTo = string.Join(",", new[] { emailTo, CC1, CC2, CC3 }
                         .Where(x => !string.IsNullOrWhiteSpace(x))
                         .Select(x => x.Trim()));
                    string body = $@"
                        Dear Sir,<br/>
                        Please find the attachment from AutoComponent.<br/><br/>
                        Regards,<br/>
                        AutoComponent Team
                        ";
                    var emailToList = emailTo.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries)
                                   .Select(e => e.Trim())
                                   .ToList();
                    // Send email
                    //_emailService.SendEmailAsync(
                    //    emailTo,
                    //    "Soft Copy Of Challan No: " +Challanno + " From AutoComponent",
                    //    CC1,
                    //    CC2,
                    //    CC3,
                    //    body,
                    //    pdfBytes,
                    //    "Report.pdf").Wait();
                    foreach (var recipient in emailToList)
                    {
                        _emailService.SendEmailAsync(
                            recipient,
                            "Soft Copy Of Short & Excess Of MRN No: " + MRNNo + " From AutoComponent",
                            CC1,
                            CC2,
                            CC3,
                            body,
                            pdfBytes,
                            "Report.pdf").Wait();
                    }

                    return Content("Report sent successfully");
                }
            }
            catch (Exception ex)
            {
                return Content($"Error: {ex.Message}\n\nStack Trace: {ex.StackTrace}");
            }
        }

        public async Task SendEmailAsync(string emailTo, string subject, string message, byte[] attachment = null, string attachmentName = null, string CC1 = "", string CC2 = "", string CC3 = "")
        {
            var emailSettings = _iconfiguration.GetSection("EmailSettings");
            emailTo = string.Join(",", new[] { emailTo, CC1, CC2, CC3 }
                          .Where(x => !string.IsNullOrWhiteSpace(x))
                          .Select(x => x.Trim()));
            var mimeMessage = new MimeMessage();
            mimeMessage.From.Add(new MailboxAddress(emailSettings["FromName"], emailSettings["FromEmail"]));
            //mimeMessage.To.Add(MailboxAddress.Parse("infotech.bmr@gmail.com"));
            //mimeMessage.To.Add(MailboxAddress.Parse(CC1));
            //mimeMessage.To.Add(MailboxAddress.Parse(CC2));
            var toEmails = emailTo.Split(',')
                              .Where(x => !string.IsNullOrWhiteSpace(x))
                              .Select(x => x.Trim());

            foreach (var email in toEmails)
            {
                if (IsValidEmail(email))
                    mimeMessage.To.Add(MailboxAddress.Parse(email));
            }
            mimeMessage.Subject = subject;
            //if (!string.IsNullOrWhiteSpace(CC1))
            //    mimeMessage.Cc.Add(new MailboxAddress("CC",CC1));
            //if (!string.IsNullOrWhiteSpace(CC2))
            //    mimeMessage.Cc.Add(MailboxAddress.Parse(CC2));
            //if (!string.IsNullOrWhiteSpace(CC3))
            //    mimeMessage.Cc.Add(MailboxAddress.Parse(CC3));

            // if (!string.IsNullOrWhiteSpace(CC1))
            //  mimeMessage.Cc.Add(MailboxAddress.Parse("bmr.client2021@gmail.com"));
            //if (!string.IsNullOrWhiteSpace(CC2))
            //   mimeMessage.Cc.Add(MailboxAddress.Parse("bmr.client2021@gmail.com"));
            //  if (!string.IsNullOrWhiteSpace(CC3))
            //   mimeMessage.Cc.Add(MailboxAddress.Parse("bmr.client2021@gmail.com"));

            var builder = new BodyBuilder();
            builder.HtmlBody = message;

            if (attachment != null && !string.IsNullOrEmpty(attachmentName))
            {
                builder.Attachments.Add(attachmentName, attachment);
            }

            mimeMessage.Body = builder.ToMessageBody();

            using (var client = new SmtpClient())
            {
                try
                {
                    await client.ConnectAsync(emailSettings["SmtpServer"],
                        int.Parse(emailSettings["SmtpPort"]),
                        MailKit.Security.SecureSocketOptions.StartTls);

                    await client.AuthenticateAsync(emailSettings["SmtpUsername"],
                        emailSettings["SmtpPassword"]);

                    await client.SendAsync(mimeMessage);
                }
                catch (Exception ex)
                {
                    // Handle exception
                    throw;
                }
                finally
                {
                    await client.DisconnectAsync(true);
                }
            }
        }
        bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch { return false; }
        }

        private byte[] ConvertImageToPdf(byte[] imageBytes)
        {
            // First ensure the image is in a supported format
            using (var ms = new MemoryStream(imageBytes))
            using (var image = Image.FromStream(ms))
            using (var pdfStream = new MemoryStream())
            {
                // Convert to PNG if needed (PdfSharp works best with PNG)
                if (image.RawFormat.Equals(ImageFormat.Png))
                {
                    using (var pngMs = new MemoryStream())
                    {
                        image.Save(pngMs, System.Drawing.Imaging.ImageFormat.Png);
                        imageBytes = pngMs.ToArray();
                    }
                }

                // Now create PDF
                var document = new PdfDocument();
                var page = document.AddPage();
                page.Width = XUnit.FromMillimeter(image.Width / image.HorizontalResolution * 25.4);
                page.Height = XUnit.FromMillimeter(image.Height / image.VerticalResolution * 25.4);

                using (var xImage = XImage.FromStream(new MemoryStream(imageBytes)))
                {
                    XGraphics gfx = XGraphics.FromPdfPage(page);
                    gfx.DrawImage(xImage, 0, 0, page.Width, page.Height);
                }

                document.Save(pdfStream, false);
                return pdfStream.ToArray();
            }
        }





        public IActionResult PrintReportShortExcess(int EntryId = 0, int YearCode = 0, string MrnNo = "", int AccountCode = 0)
        {
            string my_connection_string;
            string contentRootPath = _IWebHostEnvironment.ContentRootPath;
            string webRootPath = _IWebHostEnvironment.WebRootPath;
            var webReport = new WebReport();
            webReport.Report.Clear();
            // var ReportName = _IMaterialReceipt.GetReportName();
            webReport.Report.Dispose();
            webReport.Report = new Report();

            webReport.Report.Load(webRootPath + "\\MRNShortExcess.frx"); // default repor 
            my_connection_string = _connectionStringService.GetConnectionString();
            webReport.Report.Dictionary.Connections[0].ConnectionString = my_connection_string;
            webReport.Report.Dictionary.Connections[0].ConnectionStringExpression = "";
            //webReport.Report.SetParameterValue("MrnNoparam", MrnNo);
            webReport.Report.SetParameterValue("MrnYearcodeparam", YearCode);
            webReport.Report.SetParameterValue("entryidparam", EntryId);
            webReport.Report.SetParameterValue("accountcodeparam", AccountCode);
            webReport.Report.SetParameterValue("MyParameter", my_connection_string);
            webReport.Report.Refresh();
            return View(webReport);
        }
        public IActionResult MRNTag(int EntryId = 0, int YearCode = 0, string MrnNo = "", int AccountCode = 0)
        {
            string my_connection_string;
            string contentRootPath = _IWebHostEnvironment.ContentRootPath;
            string webRootPath = _IWebHostEnvironment.WebRootPath;
            var webReport = new WebReport();
            webReport.Report.Clear();
            // var ReportName = _IMaterialReceipt.GetReportName();
            webReport.Report.Dispose();
            webReport.Report = new Report();

            webReport.Report.Load(webRootPath + "\\MRNTAGFinal.frx"); 
            my_connection_string = _connectionStringService.GetConnectionString();
            webReport.Report.Dictionary.Connections[0].ConnectionString = my_connection_string;
            webReport.Report.Dictionary.Connections[0].ConnectionStringExpression = "";
           // webReport.Report.SetParameterValue("MrnNoparam", MrnNo);
            webReport.Report.SetParameterValue("MrnYearcodeparam", YearCode);
            webReport.Report.SetParameterValue("entryidparam", EntryId);
            //webReport.Report.SetParameterValue("accountcodeparam", AccountCode);
            webReport.Report.SetParameterValue("MyParameter", my_connection_string);
            webReport.Report.Refresh();
            return View(webReport);
        }


        public ActionResult HtmlSave(int EntryId = 0, int YearCode = 0, string MrnNo = "")
        {
            using (Report report = new Report())
            {
                string webRootPath = _IWebHostEnvironment.WebRootPath;
                var webReport = new WebReport();


                webReport.Report.Load(webRootPath + "\\MRNPrint.frx");
                //webReport.Report.SetParameterValue("flagparam", "PURCHASEORDERPRINT");
                webReport.Report.SetParameterValue("MrnEntry", EntryId);
                webReport.Report.SetParameterValue("MrnYearcode", YearCode);
                webReport.Report.SetParameterValue("MrnNo", MrnNo);
                webReport.Report.Prepare();// Preparing a report

                // Creating the HTML export
                using (HTMLExport html = new HTMLExport())
                {
                    using (FileStream st = new FileStream(webRootPath + "\\test.html", FileMode.Create))
                    {
                        webReport.Report.Export(html, st);
                        return File("App_Data/test.html", "application/octet-stream", "Test.html");
                    }
                }
            }
        }

        public IActionResult GetImage(int EntryId = 0, int YearCode = 0, string MrnNo = "")
        {
            // Creatint the Report object
            using (Report report = new Report())
            {
                string webRootPath = _IWebHostEnvironment.WebRootPath;
                var webReport = new WebReport();


                webReport.Report.Load(webRootPath + "\\MRNPrint.frx");
                //webReport.Report.SetParameterValue("flagparam", "PURCHASEORDERPRINT");
                webReport.Report.SetParameterValue("MrnEntry", EntryId);
                webReport.Report.SetParameterValue("MrnYearcode", YearCode);
                webReport.Report.SetParameterValue("MrnNo", MrnNo);
                webReport.Report.Prepare();// Preparing a report

                // Creating the Image export
                using (ImageExport image = new ImageExport())
                {
                    image.ImageFormat = ImageExportFormat.Jpeg;
                    image.JpegQuality = 100; // Set up the quality
                    image.Resolution = 100; // Set up a resolution 
                    image.SeparateFiles = false; // We need all pages in one big single file

                    using (MemoryStream st = new MemoryStream())// Using stream to save export
                    {
                        webReport.Report.Export(image, st);
                        return base.File(st.ToArray(), "image/jpeg");
                    }
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        // [Route("{controller}/Index")]
        public async Task<IActionResult> MaterialReceipt(MaterialReceiptModel model, string ShouldPrint)
        {
            var fromDt = model.FromDate;
            var toDt = model.ToDate;
            model.EntryDate = string.IsNullOrEmpty(model.EntryDate) ? DateTime.Today.ToString() : model.EntryDate;
            try
            {
                var MRGrid = new DataTable();
                var BatchGrid = new DataTable();
                string materialGrid = HttpContext.Session.GetString("KeyMaterialReceiptGrid");
                List<MaterialReceiptDetail> MaterialReceiptDetail = new List<MaterialReceiptDetail>();
                if (!string.IsNullOrEmpty(materialGrid))
                {
                    MaterialReceiptDetail = JsonConvert.DeserializeObject<List<MaterialReceiptDetail>>(materialGrid);
                }
                string batchGrid = HttpContext.Session.GetString("KeyBatchDetailGrid");
                List<BatchDetailModel> BatchDetail = new List<BatchDetailModel>();
                if (!string.IsNullOrEmpty(batchGrid))
                {
                    BatchDetail = JsonConvert.DeserializeObject<List<BatchDetailModel>>(batchGrid);
                }
                if (MaterialReceiptDetail == null)
                {
                    ModelState.Clear();
                    ModelState.TryAddModelError("Material Receipt Detail", "Material Receipt Grid Should Have At least 1 Item...!");
                    model = await BindModels(model);
                    return View("MaterialReceipt", model);
                }
                else
                {
                    model.CreatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));

                    model.CC = HttpContext.Session.GetString("Branch");
                    model.ItemDetailGrid = MaterialReceiptDetail;
                    model.BatchDetailGrid = BatchDetail;
                    MRGrid = GetDetailTable(MaterialReceiptDetail);
                    if (BatchDetail != null)
                        BatchGrid = GetBatchDetailTable(BatchDetail, model.CC);
                    if (model.Mode == "U")
                    {
                        model.UpdatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                    }
                    var Result = await _IMaterialReceipt.SaveMaterialReceipt(model, MRGrid, BatchGrid);

                    if (Result != null)
                    {
                        if (Result.StatusText == "Success" && Result.StatusCode == HttpStatusCode.OK)
                        {
                            ViewBag.isSuccess = true;
                            TempData["200"] = "200";
                            var MainModel = new MaterialReceiptModel();
                            MainModel.FinFromDate = HttpContext.Session.GetString("FromDate");
                            MainModel.YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
                            MainModel.FinToDate = HttpContext.Session.GetString("ToDate");
                            MainModel.EnteredByEmpname = HttpContext.Session.GetString("EmpName");
                            MainModel.CreatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                            HttpContext.Session.Remove("KeyMaterialReceiptGrid");
                            HttpContext.Session.Remove("KeyBatchDetailGrid");
                            MainModel.ActualEnteredBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                            MainModel.ActualEntryByName = HttpContext.Session.GetString("EmpName");
                            MainModel.ActualEntryDate = DateTime.Now;
                            //MainModel = await BindModels(MainModel);
                            MainModel.EmployeeList = await _IMaterialReceipt.GetEmployeeList();
                            MainModel.DateIntact = "Y";
                            MainModel.FromDate = fromDt;
                            MainModel.ToDate = toDt;
                            if (ShouldPrint == "true")
                            {
                                return Json(new
                                {
                                    status = "Success",
                                    entryId = model.EntryID,
                                    yearCode = model.YearCode,
                                    mrnno = model.MRNNo
                                });
                            }
                            return Json(new { status = "Success" });
                        }
                        if (Result.StatusText == "Success" && Result.StatusCode == HttpStatusCode.Accepted)
                        {
                            ViewBag.isSuccess = true;
                            TempData["202"] = "202";
                            var MainModel = new MaterialReceiptModel();
                            MainModel.FinFromDate = HttpContext.Session.GetString("FromDate");
                            MainModel.YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
                            MainModel.FinToDate = HttpContext.Session.GetString("ToDate");
                            MainModel.EnteredByEmpname = HttpContext.Session.GetString("EmpName");
                            MainModel.CreatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                            HttpContext.Session.Remove("KeyMaterialReceiptGrid");
                            HttpContext.Session.Remove("KeyBatchDetailGrid");
                            MainModel.ActualEnteredBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                            MainModel.ActualEntryByName = HttpContext.Session.GetString("EmpName");
                            MainModel.ActualEntryDate = DateTime.Now;
                            //MainModel = await BindModels(MainModel);
                            MainModel.EmployeeList = await _IMaterialReceipt.GetEmployeeList();
                            MainModel.Mode = "I";
                            MainModel.FromDate = fromDt;
                            MainModel.ToDate = toDt;
                            if (ShouldPrint == "true")
                            {
                                return Json(new
                                {
                                    status = "Success",
                                    entryId = model.EntryID,
                                    yearCode = model.YearCode,
                                    mrnno = model.MRNNo
                                });
                            }
                            return Json(new { status = "Success" });
                        }
                        if (Result.StatusText == "Error" && Result.StatusCode == HttpStatusCode.InternalServerError)
                        {
                            ViewBag.isSuccess = false;
                            var input = "";
                            if (Result != null)
                            {
                                input = Result.Result.ToString();
                                int index = input.IndexOf("#ERROR_MESSAGE");

                                if (index != -1)
                                {
                                    string errorMessage = input.Substring(index + "#ERROR_MESSAGE :".Length).Trim();
                                    TempData["ErrorMessage"] = errorMessage;
                                }
                                else
                                {
                                    string errorMessage = Result.Result.ToString();
                                    TempData["ErrorMessage"] = errorMessage;
                                }
                            }
                            else
                            {
                                TempData["500"] = "500";
                            }


                            _logger.LogError("\n \n ********** LogError ********** \n " + JsonConvert.SerializeObject(Result) + "\n \n");
                            model.IsError = "true";
                            return View("Error", Result);
                        }
                    }
                    model.DateIntact = "Y";

                    model.FromDate = fromDt;
                    model.ToDate = toDt;
                   
                    return Json(new { status = "Success" });
                }
            }
            catch (Exception ex)
            {
                LogException<MaterialReceiptController>.WriteException(_logger, ex);

                var ResponseResult = new ResponseResult()
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    StatusText = "Error",
                    Result = ex
                };

                return View("Error", ResponseResult);
            }
        }
        // [Route("{controller}/Index")]
        [HttpGet]
        public async Task<ActionResult> MaterialReceipt(int ID, string Mode, int YC, string FromDate = "", string ToDate = "", string VendorName = "", string GateNo = "", string PartCode = "", string ItemName = "", string MrnNo = "", string PoNo = "", string Type = "", string Searchbox = "")
        {
            _logger.LogInformation("\n \n ********** Page Gate Inward ********** \n \n " + _IWebHostEnvironment.EnvironmentName.ToString() + "\n \n");
            TempData.Clear();
            var MainModel = new MaterialReceiptModel();
            MainModel.FinFromDate = CommonFunc.ParseFormattedDate(HttpContext.Session.GetString("FromDate"));
            MainModel.YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
            MainModel.FinToDate = CommonFunc.ParseFormattedDate(HttpContext.Session.GetString("ToDate"));
            MainModel.EnteredByEmpname = HttpContext.Session.GetString("EmpName");
            MainModel.CreatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
            MainModel.EmployeeList = await _IMaterialReceipt.GetEmployeeList();
            MainModel.EnteredEmpId = Convert.ToInt32(HttpContext.Session.GetString("UID"));
            HttpContext.Session.Remove("KeyMaterialReceiptGrid");
            HttpContext.Session.Remove("KeyBatchDetailGrid");
            if (!string.IsNullOrEmpty(Mode) && ID > 0 && (Mode == "V" || Mode == "U"))
            {
                MainModel = await _IMaterialReceipt.GetViewByID(ID, YC).ConfigureAwait(false);
                MainModel.Mode = Mode;
                MainModel.ID = ID;
                //MainModel = await BindModels(MainModel).ConfigureAwait(false);
                MainModel.FinFromDate = HttpContext.Session.GetString("FromDate");
                MainModel.YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
                MainModel.FinToDate = HttpContext.Session.GetString("ToDate");
                MainModel.EmployeeList = await _IMaterialReceipt.GetEmployeeList();

                HttpContext.Session.SetString("KeyMaterialReceiptGrid", JsonConvert.SerializeObject(MainModel.ItemDetailGrid));
                HttpContext.Session.SetString("KeyBatchDetailGrid", JsonConvert.SerializeObject(MainModel.BatchDetailGrid));
            }
            else
            {
                MainModel = await BindModels(MainModel);
            }
            if (Mode != "U")
            {
                MainModel.ActualEnteredBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                MainModel.ActualEntryByName = HttpContext.Session.GetString("EmpName");
                MainModel.ActualEntryDate = DateTime.Now;
            }
            else
            {
                MainModel.UpdatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                MainModel.UpdatedByName = HttpContext.Session.GetString("EmpName");
                MainModel.UpdatedOn = DateTime.Now;
            }
            MainModel.FromDateBack = FromDate;
            MainModel.ToDateBack = ToDate;
            MainModel.VendorNameBack = VendorName;
            MainModel.GateNoBack = GateNo;
            MainModel.MrnNoBack = MrnNo;
            MainModel.DashboardTypeBack = Type;
            MainModel.GlobalSearchBack = Searchbox;
            MainModel.PoNoBack = PoNo;
            MainModel.PartCodeBack = PartCode;
            MainModel.ItemNameBack = ItemName;
            return View(MainModel);
        }
        public IActionResult AddMaterialReceiptDetail(List<MaterialReceiptDetail> model)
        {
            try
            {
                string materialGrid = HttpContext.Session.GetString("KeyMaterialReceiptGrid");
                List<MaterialReceiptDetail> MaterialReceiptDetail = new List<MaterialReceiptDetail>();
                
                if (!string.IsNullOrEmpty(materialGrid))
                {
                    MaterialReceiptDetail = JsonConvert.DeserializeObject<List<MaterialReceiptDetail>>(materialGrid);
                }

                var MainModel = new MaterialReceiptModel();
                var MaterialReceiptGrid = new List<MaterialReceiptDetail>();
                var MaterialGrid = new List<MaterialReceiptDetail>();
                var SSGrid = new List<MaterialReceiptDetail>();
                var seqNo = 0;
                foreach (var item in model)
                {
                    if (item != null)
                    {
                        if (MaterialReceiptDetail == null)
                        {
                            //item.SeqNo += seqNo + 1;
                            MaterialGrid.Add(item);
                            //seqNo++;
                        }
                        else
                        {
                            if (MaterialReceiptDetail.Where(x => x.ItemNumber == item.ItemNumber
                            && x.PONO == item.PONO
                 && x.SchNo == item.SchNo).Any())
                            {
                                return StatusCode(207, "Duplicate");
                            }
                            else
                            {
                                //item.SeqNo = MaterialReceiptDetail.Count + 1;
                                //MaterialGrid = MaterialReceiptDetail.Where(x => x != null).ToList();
                                SSGrid.AddRange(MaterialGrid);
                                MaterialReceiptDetail.Add(item);
                            }
                        }
                        MainModel.ItemDetailGrid = MaterialReceiptDetail;

                        HttpContext.Session.SetString("KeyMaterialReceiptGrid", JsonConvert.SerializeObject(MainModel.ItemDetailGrid));
                    }
                }
                return PartialView("_MaterialReceiptGrid", MainModel);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //[HttpGet]
        public async Task<IActionResult> MRNDashboard(string FromDate = "", string ToDate = "", string Flag = "", string VendorName = "", string MrnNo = "", string GateNo = "", string PONo = "", string ItemName = "", string PartCode = "", string Type = "")
        {
            try
            {
                HttpContext.Session.Remove("KeyMaterialReceiptGrid");
                var model = new MRNQDashboard();
                FromDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).ToString("dd/MM/yyyy").Replace("-", "/");

                ToDate = string.IsNullOrEmpty(model.ToDate) ? DateTime.Now.ToString("dd/MM/yyyy") : model.ToDate;
                var Result = await _IMaterialReceipt.GetDashboardData().ConfigureAwait(true);

                if (Result != null)
                {
                    var _List = new List<TextValue>();
                    DataSet DS = Result.Result;
                    if (DS != null)
                    {
                        var DT = DS.Tables[0].DefaultView.ToTable(false, "VendorName", "MRNNo", "MrnDate", "GateNo", "GateDate",
  "InvNo", "InvDate", "Docname", "MRNQCCompleted", "TotalAmt", "NetAmt", "EntryId", "YearCode", "EntryBy", "UpdatedBy");
                        model.MRNQDashboard = CommonFunc.DataTableToList<MRNDashboard>(DT, "MRN");
                        foreach (var row in DS.Tables[0].AsEnumerable())
                        {
                            _List.Add(new TextValue()
                            {
                                Text = row["MrnNo"].ToString(),
                                Value = row["MrnNo"].ToString()
                            });
                        }
                        //var dd = _List.Select(x => x.Value).Distinct();
                        var _MRNList = _List.DistinctBy(x => x.Value).ToList();
                        model.MRNList = _MRNList;
                        if (Flag == "False")
                        {
                            model.FromDate1 = FromDate;
                            model.ToDate1 = ToDate;
                            model.VendorName = VendorName;
                            model.MrnNo = MrnNo;
                            model.GateNo = GateNo;
                            model.PONo = PONo;
                            model.ItemName = ItemName;
                            model.PartCode = PartCode;
                            model.DashboardType = Type;
                        }
                        _List = new List<TextValue>();
                    }
                }

                return View(model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IActionResult> MRNDetailDashboard(string FromDate = "", string ToDate = "", string Flag = "", string VendorName = "", string MrnNo = "", string GateNo = "", string PONo = "", string ItemName = "", string PartCode = "", string Type = "", int pageNumber = 1, int pageSize = 50, string SearchBox = "")
        {
            //var model = new MRNQDashboard();
            var model = await _IMaterialReceipt.GetDetailDashboardData(VendorName, MrnNo, GateNo, PONo, ItemName, PartCode, FromDate, ToDate);
            model.DashboardType = "Detail";
            var modelList = model?.MRNQDashboard ?? new List<MRNDashboard>();
            List<MRNDashboard> filteredResults;



            if (!string.IsNullOrWhiteSpace(SearchBox))
            {
                filteredResults = modelList
                    .Where(i => i.GetType().GetProperties()
                        .Where(p => p.PropertyType == typeof(string))
                        .Select(p => p.GetValue(i)?.ToString())
                        .Any(value => !string.IsNullOrEmpty(value) &&
                                      value.Contains(SearchBox, StringComparison.OrdinalIgnoreCase)))
                    .ToList();

                // fallback to full list if search yields no result
                if (filteredResults.Count == 0)
                {
                    filteredResults = modelList.ToList();
                }
            }
            else
            {
                filteredResults = modelList.ToList();
            }

            model.TotalRecords = filteredResults.Count;
            model.PageNumber = pageNumber;
            model.PageSize = pageSize;
            model.MRNQDashboard = filteredResults
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();
            MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                SlidingExpiration = TimeSpan.FromMinutes(55),
                Size = 1024,
            };

            _MemoryCache.Set("KeyMRNList_Detail", modelList, cacheEntryOptions);
            return PartialView("_MRNDashboardGrid", model);
        }
        public async Task<JsonResult> GetServerDate()
        {
            try
            {
                //DateTime time = DateTime.Now;
                //string format = "MMM ddd d HH:mm yyyy";
                //string formattedDate = time.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);

                //var dt = time.ToString(format);
                //return Json(formattedDate);

                //  var time = CommonFunc.ParseFormattedDate(DateTime.Now.ToString());
                 var time = CommonFunc.ParseFormattedDate(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                //return Json(DateTime.Now.ToString("yyyy-MM-dd"));
                 return Json(time);
                //return Json(DateTime.Now.ToString("dd/MMM/yyyy", CultureInfo.InvariantCulture));

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
        public async Task<JsonResult> GetFormRights()
        {
            var userID = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
            var JSON = await _IMaterialReceipt.GetFormRights(userID);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }

        public IActionResult DeleteItemRow(int SeqNo)
        {
            var MainModel = new MaterialReceiptModel();

            string modelJson = HttpContext.Session.GetString("KeyMaterialReceiptGrid");
            List<MaterialReceiptDetail> MaterialReceiptGrid = new List<MaterialReceiptDetail>();
            if (!string.IsNullOrEmpty(modelJson))
            {
                MaterialReceiptGrid = JsonConvert.DeserializeObject<List<MaterialReceiptDetail>>(modelJson);
            }
            int Indx = Convert.ToInt32(SeqNo) - 1;

            if (MaterialReceiptGrid != null && MaterialReceiptGrid.Count > 0)
            {
                MaterialReceiptGrid.RemoveAt(Convert.ToInt32(Indx));

                Indx = 0;

                foreach (var item in MaterialReceiptGrid)
                {
                    Indx++;
                    item.SeqNo = Indx;
                }
                MainModel.ItemDetailGrid = MaterialReceiptGrid;

                if (MaterialReceiptGrid.Count == 0)
                {
                    HttpContext.Session.Remove("KeyMaterialReceiptGrid");
                }
                
                HttpContext.Session.SetString("KeyMaterialReceiptGrid", JsonConvert.SerializeObject(MaterialReceiptGrid));
            }
            return PartialView("_MaterialReceiptGrid", MainModel);
        }
        public IActionResult DeleteBatchItemRow(int SeqNo)
        {
            var MainModel = new MaterialReceiptModel();
            string modelJson = HttpContext.Session.GetString("KeyBatchDetailGrid");
            List<BatchDetailModel> BatchDetailModel = new List<BatchDetailModel>();
            if (!string.IsNullOrEmpty(modelJson))
            {
                BatchDetailModel = JsonConvert.DeserializeObject<List<BatchDetailModel>>(modelJson);
            }
            int Indx = Convert.ToInt32(SeqNo) - 1;

            if (BatchDetailModel != null && BatchDetailModel.Count > 0)
            {
                BatchDetailModel.RemoveAt(Convert.ToInt32(Indx));

                Indx = 0;

                foreach (var item in BatchDetailModel)
                {
                    Indx++;
                    item.SeqNo = Indx;
                }
                MainModel.BatchDetailGrid = BatchDetailModel;

                if (BatchDetailModel.Count == 0)
                {
                    HttpContext.Session.Remove("KeyBatchDetailGrid");
                }
            }
            return PartialView("_BatchDetailAdd", MainModel);
        }
        private static DataTable GetDetailTable(IList<MaterialReceiptDetail> DetailList)
        {
            var MRGrid = new DataTable();

            MRGrid.Columns.Add("SeqNo", typeof(int));
            MRGrid.Columns.Add("pono", typeof(string));
            MRGrid.Columns.Add("poyearcode", typeof(int));
            MRGrid.Columns.Add("schno", typeof(string));
            MRGrid.Columns.Add("schyearcode", typeof(int));
            // MRGrid.Columns.Add("SchDate", typeof(string));
            MRGrid.Columns.Add("PoType", typeof(string));
            MRGrid.Columns.Add("PoAmendNo", typeof(int));
            MRGrid.Columns.Add("PODate", typeof(string));
            MRGrid.Columns.Add("ItemCode", typeof(int));
            MRGrid.Columns.Add("Unit", typeof(string));
            MRGrid.Columns.Add("RateUnit", typeof(string));
            MRGrid.Columns.Add("AltUnit", typeof(string));
            MRGrid.Columns.Add("NoOfCase", typeof(int));
            MRGrid.Columns.Add("BillQty", typeof(decimal));
            MRGrid.Columns.Add("RecQty", typeof(decimal));
            MRGrid.Columns.Add("AltRecQty", typeof(decimal));
            MRGrid.Columns.Add("ShortExcessQty", typeof(decimal));
            MRGrid.Columns.Add("Rate", typeof(decimal));
            MRGrid.Columns.Add("rateinother", typeof(decimal));
            MRGrid.Columns.Add("Amount", typeof(decimal));
            MRGrid.Columns.Add("PendPOQty", typeof(decimal));
            MRGrid.Columns.Add("QCCompleted", typeof(char));
            MRGrid.Columns.Add("RetChallanPendQty", typeof(decimal));
            MRGrid.Columns.Add("batchWise", typeof(string));
            MRGrid.Columns.Add("Salebillno", typeof(string));
            MRGrid.Columns.Add("salebillYearCode", typeof(int));
            MRGrid.Columns.Add("AgainstChallanNo", typeof(string));
            MRGrid.Columns.Add("Batchno", typeof(string));
            MRGrid.Columns.Add("Uniquebatchno", typeof(string));
            MRGrid.Columns.Add("SupplierBatchNo", typeof(string));
            MRGrid.Columns.Add("ShelfLife", typeof(decimal));
            MRGrid.Columns.Add("ItemSize", typeof(string));
            MRGrid.Columns.Add("ItemColor", typeof(string));
            MRGrid.Columns.Add("PartCode", typeof(string));
            MRGrid.Columns.Add("DisPer", typeof(decimal));
            MRGrid.Columns.Add("DiscRs", typeof(decimal));
            DateTime currentDate = DateTime.Now;
            foreach (var Item in DetailList)
            {
                if (Item.AltUnit == "undefined" || Item.AltUnit == null || Item.AltUnit == "null")
                {
                    Item.AltUnit = "";
                }
                DateTime poDt = new DateTime();

                poDt = Convert.ToDateTime(Item.PODate);
                MRGrid.Rows.Add(
                    new object[]
                    {
                    Item.SeqNo,
                    Item.PONO == null ? "" : Item.PONO,
                    Item.PoYearCode,
                    Item.SchNo == null ? "" : Item.SchNo,
                    Item.SchYearCode,
                  //  Item.SchDate==null?currentDate :ParseFormattedDate(Item.SchDate),
                    Item.PoType == null ? "" : Item.PoType,
                    Item.POAmendNo,
                    //"2024/02/11",
                    Item.PODate == null ? currentDate : ParseFormattedDate(Item.PODate),
                    Item.ItemCode,
                    Item.Unit == null ? "" : Item.Unit,
                    Item.RateUnit == null ? "" : Item.RateUnit,
                    Item.AltUnit == null ? "" : Item.AltUnit,
                    Item.NoOfCase,
                    Item.BillQty,
                    Item.RecQty,
                    Item.AltRecQty,
                    Item.ShortExcessQty,
                    Item.Rate,
                    Item.RateinOther,
                    Item.Amount,
                    Item.PendPOQty,
                    Item.QCCompleted == null ? "" : Item.QCCompleted,
                    Item.RetChallanPendQty,
                    Item.BatchWise == null ? "" : Item.BatchWise,
                    Item.SaleBillNo == null ? "" : Item.SaleBillNo,
                    Item.SaleBillYearCode,
                    Item.AgainstChallanNo == null ? "" : Item.AgainstChallanNo,
                    Item.BatchNo== null ? "" : Item.BatchNo,
                    Item.UniqueBatchNo== null ? "" : Item.UniqueBatchNo,
                    Item.SupplierBatchNo== null ? "" : Item.SupplierBatchNo,
                    Item.ShelfLife,
                    Item.ItemSize == null ? "" : Item.ItemSize,
                    Item.ItemColor == null ? "" : Item.ItemColor,
                    Item.PartCode== null ? "" : Item.PartCode,
                      Item.DisPer,
                    Item.DiscRs,
                    });
            }
            MRGrid.Dispose();
            return MRGrid;
        }
        public static DateTime ParseDate(string dateString)
        {
            if (string.IsNullOrEmpty(dateString))
            {
                return default;
            }

            if (DateTime.TryParseExact(dateString, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate))
            {
                return parsedDate;
            }
            else
            {
                return DateTime.Parse(dateString);
            }

            //    throw new FormatException("Invalid date format. Expected format: dd/MM/yyyy");

        }
        private static DataTable GetBatchDetailTable(IList<BatchDetailModel> DetailList, string Branch)
        {
            var MRGrid = new DataTable();

            MRGrid.Columns.Add("EntryId", typeof(int));
            MRGrid.Columns.Add("Yearcode", typeof(int));
            MRGrid.Columns.Add("ItemCode", typeof(int));
            MRGrid.Columns.Add("PONo", typeof(string));
            MRGrid.Columns.Add("PODate", typeof(string));
            MRGrid.Columns.Add("POYearcode", typeof(int));
            MRGrid.Columns.Add("SchNo", typeof(string));
            MRGrid.Columns.Add("SchDate", typeof(string));
            MRGrid.Columns.Add("SchYearcode", typeof(int));
            MRGrid.Columns.Add("TotalQty", typeof(float));
            MRGrid.Columns.Add("TotalRecQty", typeof(float));
            MRGrid.Columns.Add("VendorBatchQty", typeof(float));
            MRGrid.Columns.Add("VendorBatchNo", typeof(string));
            MRGrid.Columns.Add("Uniquebatchno", typeof(string));
            MRGrid.Columns.Add("CC", typeof(string));
            MRGrid.Columns.Add("ManufactureDate", typeof(string));
            MRGrid.Columns.Add("ExpiryDate", typeof(string));

            var currentDate = ParseFormattedDate((DateTime.Today).ToString());
            foreach (var Item in DetailList)
            {

                MRGrid.Rows.Add(
                    new object[]
                    {
                        Item.EntryId,
                        Item.YearCode,
                        Item.ItemCode,
                        Item.PONO,
                        Item.PODate?? currentDate,
                        //currentDate,//change after ward - PODATE
                        Item.POYearCode,
                        Item.SchNO ?? "",
                        //Item.SchDate??
                        currentDate,
                        //currentDate,//change afterward - schdate
                        Item.SchYearCode ?? 0,
                        Item.Qty,
                        Item.RecQty,
                        Item.VendorBatchQty,
                        Item.VendorBatchNo,
                        Item.UniqueBatchNO,
                        Item.CC,
                        currentDate,
                        currentDate
                    });
            }
            MRGrid.Dispose();
            return MRGrid;
        }
        public async Task<MaterialReceiptModel> BindModels(MaterialReceiptModel model)
        {
            if (model == null)
            {
                //model = new MaterialReceiptModel
                //{
                //    Mo
                //};
            }
            //model.PONO = await _IDataLogic.GetDropDownList("PENDINGPOLIST","I", "SP_GateMainDetail");
            return model;
        }
        public async Task<JsonResult> CheckFeatureOption()
        {
            var JSON = await _IMaterialReceipt.CheckFeatureOption();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetGateNo(string FromDate, string ToDate)
        {
            var JSON = await _IMaterialReceipt.GetGateNo("PENDINGGATEFORMRN", "SP_MRN", FromDate, ToDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> BindDept()
        {
            var JSON = await _IMaterialReceipt.BindDept("BINDDEPT", "SP_MRN");
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> ClearGridAjax(string FromDate, string ToDate)
        {
            HttpContext.Session.Remove("KeyMaterialReceiptGrid");
            HttpContext.Session.Remove("KeyBatchDetailGrid");
            var JSON = await _IMaterialReceipt.GetGateNo("PENDINGGATEFORMRN", "SP_MRN", FromDate, ToDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> ClearbatchGridAjax(string FromDate, string ToDate)
        {
            HttpContext.Session.Remove("KeyBatchDetailGrid");
            var JSON = await _IMaterialReceipt.GetGateNo("PENDINGGATEFORMRN", "SP_MRN", FromDate, ToDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetGateMainData(string GateNo, string GateYearCode, int GateEntryId)
        {
            
            var JSON = await _IMaterialReceipt.GetGateMainData("GATEMAINDATA", "SP_MRN", GateNo, GateYearCode, GateEntryId);
            string JsonString = JsonConvert.SerializeObject(JSON);
            //HttpContext.Session.SetString("KeyMaterialReceiptGrid", JsonString);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetGateItemData(string GateNo, string GateYearCode, int GateEntryId)
        {
            //HttpContext.Session.Remove("KeyMaterialReceiptGrid");
            var JSON = await _IMaterialReceipt.GetGateItemData("GATEMAINITEM", "SP_MRN", GateNo, GateYearCode, GateEntryId);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> AltUnitConversion(int ItemCode, decimal AltQty, decimal UnitQty)
        {
            var JSON = await _IMaterialReceipt.AltUnitConversion(ItemCode, AltQty, UnitQty);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetDeptAndEmp(int DeptId, int RespEmp)
        {
            var JSON = await _IMaterialReceipt.GetDeptAndEmp("GetDEPTANDEMP", "SP_MRN", DeptId, RespEmp);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<IActionResult> GetSearchData(string VendorName, string MrnNo, string GateNo, string PONo, string ItemName, string PartCode, string FromDate, string ToDate, int FromMRNNo, int ToMRNNo, int pageNumber = 1, int pageSize = 50, string SearchBox = "")
        {
            //model.Mode = "Search";
            var model = new MRNQDashboard();
            model = await _IMaterialReceipt.GetDashboardData(VendorName, MrnNo, GateNo, PONo, ItemName, PartCode, FromDate, ToDate, FromMRNNo, ToMRNNo);
            model.DashboardType = "Summary";
            var modelList = model?.MRNQDashboard ?? new List<MRNDashboard>();
            List<MRNDashboard> filteredResults;



            if (!string.IsNullOrWhiteSpace(SearchBox))
            {
                filteredResults = modelList
                    .Where(i => i.GetType().GetProperties()
                        .Where(p => p.PropertyType == typeof(string))
                        .Select(p => p.GetValue(i)?.ToString())
                        .Any(value => !string.IsNullOrEmpty(value) &&
                                      value.Contains(SearchBox, StringComparison.OrdinalIgnoreCase)))
                    .ToList();

                // fallback to full list if search yields no result
                if (filteredResults.Count == 0)
                {
                    filteredResults = modelList.ToList();
                }
            }
            else
            {
                filteredResults = modelList.ToList();
            }

            model.TotalRecords = filteredResults.Count;
            model.PageNumber = pageNumber;
            model.PageSize = pageSize;
            model.MRNQDashboard = filteredResults
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();
            MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                SlidingExpiration = TimeSpan.FromMinutes(55),
                Size = 1024,
            };

            _MemoryCache.Set("KeyMRNList_Summary", modelList, cacheEntryOptions);
            return PartialView("_MRNDashboardGrid", model);
        }
        [HttpGet]
        public IActionResult GlobalSearch(string searchString, string dashboardType = "Summary", int pageNumber = 1, int pageSize = 50)
        {
            MRNQDashboard model = new MRNQDashboard();
            if (string.IsNullOrWhiteSpace(searchString))
            {
                return PartialView("_MRNDashboardGrid", new List<MRNDashboard>());
            }
            string cacheKey = $"KeyMRNList_{dashboardType}";
            if (!_MemoryCache.TryGetValue(cacheKey, out IList<MRNDashboard> MRNDashboard) || MRNDashboard == null)
            {
                return PartialView("_MRNDashboardGrid", new List<MRNDashboard>());
            }

            List<MRNDashboard> filteredResults;

            if (string.IsNullOrWhiteSpace(searchString))
            {
                filteredResults = MRNDashboard.ToList();
            }
            else
            {
                filteredResults = MRNDashboard
                    .Where(i => i.GetType().GetProperties()
                        .Where(p => p.PropertyType == typeof(string))
                        .Select(p => p.GetValue(i)?.ToString())
                        .Any(value => !string.IsNullOrEmpty(value) &&
                                      value.Contains(searchString, StringComparison.OrdinalIgnoreCase)))
                    .ToList();


                if (filteredResults.Count == 0)
                {
                    filteredResults = MRNDashboard.ToList();
                }
            }

            model.TotalRecords = filteredResults.Count;
            model.MRNQDashboard = filteredResults.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            model.PageNumber = pageNumber;
            model.PageSize = pageSize;

            if (dashboardType == "Summary")
            {
                return PartialView("_MRNDashboardGrid", model);
            }
            else
            {

                return PartialView("_MRNDetailDashboardGrid", model);
            }
        }
        public async Task<IActionResult> ExportMaterialReceiptDataToExcel(string ReportType)
        {
            //string modelJson = HttpContext.Session.GetString("KeyPOList");
            //if (!_MemoryCache.TryGetValue("KeyInventoryAgeingList", out List<MRNDashboard> modelList))
            //{
            //    return NotFound("No data available to export.");
            //}
            string cacheKey = $"KeyMRNList_{ReportType}";
            if (!_MemoryCache.TryGetValue(cacheKey, out IList<MRNDashboard> modelList) )
            {
                return NotFound("No data available to export.");
            }

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("MRN");

            var reportGenerators = new Dictionary<string, Action<IXLWorksheet, IList<MRNDashboard>>>
            {
                { "Summary", EXPORT_MRNSummaryGrid },
                { "Detail", EXPORT_MRNDetailGrid }

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
                "MaterialReceipt.xlsx"
            );
        }

        private void EXPORT_MRNSummaryGrid(IXLWorksheet sheet, IList<MRNDashboard> list)
        {
            string[] headers = {
                "#Sr","MRN No", "MRN Date", "Gate No", "Gate Date", "Vendor Name",
                "InvNo", "InvoiceDate", "DocName", "QC Status", "Total Amt",
                "Net Amt", "Entry ID", "Year Code", "Entered By", "Updated By"
            };



            for (int i = 0; i < headers.Length; i++)
                sheet.Cell(1, i + 1).Value = headers[i];

            int row = 2, srNo = 1;
            foreach (var item in list)
            {
                sheet.Cell(row, 1).Value = srNo++;
                sheet.Cell(row, 2).Value = item.MrnNo;
                sheet.Cell(row, 3).Value = item.MrnDate?.Split(" ")[0];
                sheet.Cell(row, 4).Value = item.GateNo;
                sheet.Cell(row, 5).Value = item.GateDate?.Split(" ")[0];
                sheet.Cell(row, 6).Value = item.VendorName;
                sheet.Cell(row, 7).Value = item.InvNo;
                sheet.Cell(row, 8).Value = item.InvDate?.Split(" ")[0];
                sheet.Cell(row, 9).Value = item.Docname;
                sheet.Cell(row, 10).Value = item.MRNQCCompleted;
                sheet.Cell(row, 11).Value = item.TotalAmt;
                sheet.Cell(row, 12).Value = item.NetAmt;
                sheet.Cell(row, 13).Value = item.EntryId;
                sheet.Cell(row, 14).Value = item.YearCode;
                sheet.Cell(row, 15).Value = item.EntryBy;
                sheet.Cell(row, 16).Value = item.UpdatedBy;

                row++;
            }
        }
        private void EXPORT_MRNDetailGrid(IXLWorksheet sheet, IList<MRNDashboard> list)
        {
            string[] headers = {
                "#Sr", "MRN No", "MRN Date", "Gate No", "Gate Date", "Item Name",
    "Part Code", "Vendor Name", "InvNo", "InvoiceDate", "DocName",
    "QC Status", "Total Amt", "Net Amt", "Entry ID", "Year Code",
    "Entered By", "Updated By", "Po No", "Po Year", "Sch No",
    "Sch Year Code", "PO Type.", "PO Date", "Unit", "Qty",
    "Bill Qty", "Rec Qty", "Alt Qty", "Rate Unit", "Alt Unit",
    "No of case", "Alt Rec Qty", "ShortExcessQty", "Rate", "RateInOtherCurr",
    "Amount", "Pend PO QTY", "QCCompleted", "Ret Challan Pend Qty", "BatchWise",
    "SaleBillNo", "SaleBillYearCode", "AgainstChallanNo", "BatchNo", "Unique Batch No",
    "SupplierBatchNo", "ShelfLife", "Item Size", "Item Color"
            };



            for (int i = 0; i < headers.Length; i++)
                sheet.Cell(1, i + 1).Value = headers[i];

            int row = 2, srNo = 1;
            foreach (var item in list)
            {
                sheet.Cell(row, 1).Value = srNo++;
                sheet.Cell(row, 2).Value = item.MrnNo;
                sheet.Cell(row, 3).Value = item.MrnDate?.Split(" ")[0];
                sheet.Cell(row, 4).Value = item.GateNo;
                sheet.Cell(row, 5).Value = item.GateDate?.Split(" ")[0];
                sheet.Cell(row, 6).Value = item.ItemName;
                sheet.Cell(row, 7).Value = item.PartCode;
                sheet.Cell(row, 8).Value = item.VendorName;
                sheet.Cell(row, 9).Value = item.InvNo;
                sheet.Cell(row, 10).Value = item.InvDate?.Split(" ")[0];
                sheet.Cell(row, 11).Value = item.Docname;
                sheet.Cell(row, 12).Value = item.MRNQCCompleted;
                sheet.Cell(row, 13).Value = item.TotalAmt;
                sheet.Cell(row, 14).Value = item.NetAmt;
                sheet.Cell(row, 15).Value = item.EntryId;
                sheet.Cell(row, 16).Value = item.YearCode;
                sheet.Cell(row, 17).Value = item.EntryBy;
                sheet.Cell(row, 18).Value = item.UpdatedBy;
                sheet.Cell(row, 19).Value = item.PONO;
                sheet.Cell(row, 20).Value = item.PoYearCode;
                sheet.Cell(row, 21).Value = item.SchNo;
                sheet.Cell(row, 22).Value = item.SchYearCode;
                sheet.Cell(row, 23).Value = item.PoType;
                sheet.Cell(row, 24).Value = item.PODate;
                sheet.Cell(row, 25).Value = item.Unit;
                sheet.Cell(row, 26).Value = item.Qty;
                sheet.Cell(row, 27).Value = item.BillQty;
                sheet.Cell(row, 28).Value = item.RecQty;
                sheet.Cell(row, 29).Value = item.AltQty;
                sheet.Cell(row, 30).Value = item.RateUnit;
                sheet.Cell(row, 31).Value = item.AltUnit;
                sheet.Cell(row, 32).Value = item.NoOfCase;
                sheet.Cell(row, 33).Value = item.AltRecQty;
                sheet.Cell(row, 34).Value = item.ShortExcessQty;
                sheet.Cell(row, 35).Value = item.Rate;
                sheet.Cell(row, 36).Value = item.RateinOther;
                sheet.Cell(row, 37).Value = item.Amount;
                sheet.Cell(row, 38).Value = item.PendPOQty;
                sheet.Cell(row, 39).Value = item.QCCompleted;
                sheet.Cell(row, 40).Value = item.RetChallanPendQty;
                sheet.Cell(row, 41).Value = item.BatchWise;
                sheet.Cell(row, 42).Value = item.SaleBillNo;
                sheet.Cell(row, 43).Value = item.SaleBillYearCode;
                sheet.Cell(row, 44).Value = item.AgainstChallanNo;
                sheet.Cell(row, 45).Value = item.BatchNo;
                sheet.Cell(row, 46).Value = item.UniqueBatchNo;
                sheet.Cell(row, 47).Value = item.SupplierBatchNo;
                sheet.Cell(row, 48).Value = item.ShelfLife;
                sheet.Cell(row, 49).Value = item.ItemSize;
                sheet.Cell(row, 50).Value = item.ItemColor;

                row++;
            }
        }

        public async Task<IActionResult> DeleteByID(int ID, int YC, string FromDate = "", string ToDate = "", string VendorName = "", string MrnNo = "", string GateNo = "", string PONo = "", string ItemName = "", string PartCode = "", string Type = "")
        {
            var Result = await _IMaterialReceipt.DeleteByID(ID, YC);

            if (Result.StatusText == "Success" || Result.StatusCode == HttpStatusCode.Gone)
            {
                ViewBag.isSuccess = true;
                TempData["410"] = "410";
            }
            else if (Result.StatusText == "Error" || Result.StatusCode == HttpStatusCode.Accepted)
            {
                ViewBag.isSuccess = true;
                TempData["423"] = "423";
            }
            else
            {
                ViewBag.isSuccess = false;
                TempData["500"] = "500";
            }

            return RedirectToAction("MRNDashboard", new { FromDate = FromDate, ToDate = ToDate, Flag = "False", VendorName = VendorName, MrnNo = MrnNo, GateNo = GateNo, PONo = PONo, ItemName = ItemName, PartCode = PartCode, Type = Type });

        }
        public async Task<JsonResult> FillEntryandMRN(int YearCode)
        {
            var JSON = await _IMaterialReceipt.FillEntryandMRN("NewEntryId", YearCode, "SP_MRN");
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }

        public IActionResult AddBatchDetail(BatchDetailModel model)
        {
            try
            {
                string batchGrid = HttpContext.Session.GetString("KeyBatchDetailGrid");
                List<BatchDetailModel> BatchDetailGrid = new List<BatchDetailModel>();
                if (string.IsNullOrEmpty(batchGrid))
                {
                    BatchDetailGrid = JsonConvert.DeserializeObject<List<BatchDetailModel>>(batchGrid);
                }

                var MainModel = new MaterialReceiptModel();
                var MaterialReceiptGrid = new List<BatchDetailModel>();
                var MaterialGrid = new List<BatchDetailModel>();
                var SSGrid = new List<BatchDetailModel>();

                if (model != null)
                {
                    if (BatchDetailGrid == null)
                    {
                        if (model.VendorBatchQty > model.RecQty)
                        {
                            return StatusCode(207, "Duplicate");

                        }

                        else
                        {
                            model.SeqNo = 1;
                            MaterialGrid.Add(model);
                        }
                    }
                    else
                    {
                        decimal? sum = 0;
                        foreach (var item in BatchDetailGrid)
                        {
                            if (string.IsNullOrEmpty(model.SchNO))
                            {
                                if (item.PartCode == model.PartCode && item.PONO == model.PONO)
                                {
                                    sum += item.VendorBatchQty;
                                }
                            }
                            else
                            {
                                if (item.PartCode == model.PartCode && item.PONO == model.PONO && item.SchNO == model.SchNO)
                                {
                                    sum += item.VendorBatchQty;
                                }
                            }

                        }
                        sum += model.VendorBatchQty;

                        if (BatchDetailGrid.Where(x => x.PartCode == model.PartCode && sum > x.RecQty && x.PONO == model.PONO && (x.SchNO == model.SchNO || string.IsNullOrEmpty(x.SchNO))).Any())
                        {
                            return StatusCode(207, "Duplicate");
                        }
                        else if (BatchDetailGrid.Where(x => x.VendorBatchNo == model.VendorBatchNo && x.PartCode == model.PartCode && x.PONO == model.PONO && x.SchNO == model.SchNO).Any())
                        {
                            return StatusCode(205, "DuplicateVendor");
                        }
                        else
                        {
                            model.SeqNo = BatchDetailGrid.Count + 1;
                            MaterialGrid = BatchDetailGrid.Where(x => x != null).ToList();
                            SSGrid.AddRange(MaterialGrid);
                            MaterialGrid.Add(model);
                        }
                    }

                    MainModel.BatchDetailGrid = MaterialGrid;

                    HttpContext.Session.SetString("KeyBatchDetailGrid", JsonConvert.SerializeObject(MainModel.BatchDetailGrid));
                }
                else
                {
                    ModelState.TryAddModelError("Error", "Schedule List Cannot Be Empty...!");
                }

                return PartialView("_BatchDetailAdd", MainModel);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IActionResult EditItemRow(int SeqNo)
        {
            string modelJson = HttpContext.Session.GetString("KeyMaterialReceiptGrid");
            List<MaterialReceiptDetail> MaterialGrid = new List<MaterialReceiptDetail>();
            if (!string.IsNullOrEmpty(modelJson))
            {
                MaterialGrid = JsonConvert.DeserializeObject<List<MaterialReceiptDetail>>(modelJson);
            }

            var SSGrid = MaterialGrid.Where(x => x.SeqNo == SeqNo);
            string JsonString = JsonConvert.SerializeObject(SSGrid);
            return Json(JsonString);
        }
        public async Task<JsonResult> CheckEditOrDelete(string MRNNo, int YearCode)
        {
            var JSON = await _IMaterialReceipt.CheckEditOrDelete(MRNNo, YearCode);
            _logger.LogError(JsonConvert.SerializeObject(JSON));
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> CheckBeforeInsert(string GateNo, int GateYearCode)
        {
            var JSON = await _IMaterialReceipt.CheckBeforeInsert(GateNo, GateYearCode);
            _logger.LogError(JsonConvert.SerializeObject(JSON));
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
    }
}
