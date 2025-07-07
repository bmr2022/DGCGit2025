using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Data;
using Newtonsoft.Json;
using System.Diagnostics;
using static eTactWeb.Data.Common.CommonFunc;
using static eTactWeb.DOM.Models.Common;
using System.Diagnostics.SymbolStore;
using FastReport.Web;
using FastReport;
using FastReport.Export.Html;
using FastReport.Export.Image;
using Microsoft.Extensions.Configuration;
using System.Configuration;
using System.Net;
using System.Globalization;
using FastReport.Data;
using PdfSharp.Pdf;
using PdfSharp.Drawing;
using PdfSharp.Drawing.BarCodes;
using eTactWeb.Services;
using MimeKit;
using MailKit.Net.Smtp;
using System.Drawing;
using System.Security.Cryptography;
using Newtonsoft.Json.Linq;
namespace eTactWeb.Controllers
{
    public class NRGPController : Controller
    {
        public WebReport webReport;
        private readonly IDataLogic _IDataLogic;
        private readonly IIssueNRGP _IIssueNRGP;
        private readonly ITaxModule _ITaxModule;
        private readonly ILogger<NRGPController> _logger;
        private readonly IItemMaster itemMaster;
        public readonly IEinvoiceService _IEinvoiceService;
        private EncryptDecrypt _EncryptDecrypt { get; }
        private IWebHostEnvironment _IWebHostEnvironment { get; }
        private LoggerInfo LoggerInfo { get; }
        private readonly IConfiguration _iconfiguration;
        private readonly IEmailService _emailService;
        public NRGPController(ILogger<NRGPController> logger, IConfiguration configuration, IDataLogic iDataLogic, IIssueNRGP iIssueNRGP, ITaxModule iTaxModule, IWebHostEnvironment iWebHostEnvironment, IItemMaster itemMaster, EncryptDecrypt encryptDecrypt, LoggerInfo loggerInfo, IEmailService emailService, IEinvoiceService IEinvoiceService)
        {
            _logger = logger;
            _IDataLogic = iDataLogic;
            _IIssueNRGP = iIssueNRGP;
            _ITaxModule = iTaxModule;
            _IWebHostEnvironment = iWebHostEnvironment;
            this.itemMaster = itemMaster;
            _EncryptDecrypt = encryptDecrypt;
            LoggerInfo = loggerInfo;
            _iconfiguration = configuration;
            _emailService = emailService;
            _IEinvoiceService = IEinvoiceService;
        }

        [Route("{controller}/Index")]
        //[HttpGet]
        public async Task<ActionResult> IssueNRGP(int ID, string Mode, int YC, string FromDate = "", string ToDate = "", string VendorName = "", string RGPNRGP = "", string PartCode = "", string ItemName = "", string ChallanNo = "", string ChallanType = "", string DashboardType = "")
        {
            var model = new IssueNRGPModel();
            model.RGPNRGP = "NRGP";
            //model.EntryId = 4;
            model.YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));

            var fillEntryChallanResult = await FillEntryandChallanNo(model.YearCode, model.RGPNRGP);
            var fillChallanTypeResult = await FillChallanType(model.RGPNRGP);
            // Extract JSON string from JsonResult
            var jsonResult = fillEntryChallanResult.Value?.ToString();
            var challanTypeJson = fillChallanTypeResult.Value?.ToString();

            // Deserialize JSON into a dynamic object or a specific class
            if (!string.IsNullOrEmpty(jsonResult))
            {
                var entryChallanData = JsonConvert.DeserializeObject<dynamic>(jsonResult);

                // Assign values to the model
                model.EntryId = entryChallanData.Result[0].EntryId;
                model.ChallanNo = entryChallanData.Result[0].ChallanNo;
                //model.ChallanNo = entryChallanData.ChallanNo;
            }

            if (!string.IsNullOrEmpty(challanTypeJson))
            {
                var entryChallanData = JsonConvert.DeserializeObject<dynamic>(challanTypeJson);

                // Assign values to the model
                var _List = new List<TextValue>();

                //foreach (var row in entryChallanData)
                //{
                //    _List.Add(new TextValue
                //    {
                //        Value = row["ChallanType"].ToString(),
                //        Text = row["ChallanType"].ToString()
                //    });
                //}
                //model.ChallanTypeList = _List;
                foreach (var row in entryChallanData.Result)
                {
                    _List.Add(new TextValue
                    {
                        Value = row.ChallanType.ToString(),
                        Text = row.ChallanType.ToString()
                    });
                }

                model.ChallanTypeList = _List;
            }

            model.ActualEnteredEMpBy = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
            model.ActualEnteredEmpByName = HttpContext.Session.GetString("EmpName");
            model.FinFromDate = CommonFunc.ParseFormattedDate(HttpContext.Session.GetString("FromDate"));
            model.FinToDate = CommonFunc.ParseFormattedDate(HttpContext.Session.GetString("ToDate"));
            //model.FinToDate = HttpContext.Session.GetString("ToDate");
            model.CC = HttpContext.Session.GetString("Branch");
            HttpContext.Session.Remove("KeyIssueNRGPGrid");
            HttpContext.Session.Remove("KeyIssueNRGPTaxGrid");
            if (Mode != "U")
            {
                model.Uid = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                model.ActualEnteredEMpBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                model.ActualEnteredEmpByName = HttpContext.Session.GetString("EmpName");
                model.IssByEmpCode = Convert.ToInt32(HttpContext.Session.GetString("EmployeeList"));
            }

            if (!string.IsNullOrEmpty(Mode) && ID > 0 && (Mode == "V" || Mode == "U"))
            {
                model = await _IIssueNRGP.GetViewByID(ID, YC, Mode).ConfigureAwait(true);

                model.Mode = Mode;
                model.YearCode = YC;
                model.EntryTime = model.EntryTime;
                model = await BindModel(model);

                model.ID = ID;

                if (model.IssueNRGPDetailGrid?.Count != 0 && model.IssueNRGPDetailGrid != null)
                {
                    HttpContext.Session.SetString("KeyIssueNRGPGrid", JsonConvert.SerializeObject(model.IssueNRGPDetailGrid));
                }

                if (model.IssueNRGPTaxGrid != null)
                {
                    //MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
                    //{
                    //    AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                    //    SlidingExpiration = TimeSpan.FromMinutes(55),
                    //    Size = 1024,
                    //};
                    HttpContext.Session.SetString("KeyIssueNRGPGrid", JsonConvert.SerializeObject(model.IssueNRGPDetailGrid));
                    HttpContext.Session.SetString("KeyIssueNRGPTaxGrid", JsonConvert.SerializeObject(model.IssueNRGPTaxGrid));
                    string modelNRGPJson = HttpContext.Session.GetString("KeyIssueNRGPTaxGrid");
                    List<IssueNRGPDetail> TaxGrid11 = new List<IssueNRGPDetail>();
                    if (string.IsNullOrEmpty(modelNRGPJson))
                    {
                        TaxGrid11 = JsonConvert.DeserializeObject<List<IssueNRGPDetail>>(modelNRGPJson);
                    }
                }
            }
            else
            {
                model = await BindModel(model);
                //model.ChallanDate = ParseFormattedDate(DateTime.Now.ToString("yyyy-MM-dd"));
                model.EntryTime = DateTime.Now.ToString("hh:mm:ss tt");
            }
            HttpContext.Session.SetString("IssueNRGP", JsonConvert.SerializeObject(model));
            string modelJson = HttpContext.Session.GetString("KeyIssueNRGPTaxGrid");
            List<IssueNRGPDetail> TaxGrid = new List<IssueNRGPDetail>();
            if (!string.IsNullOrEmpty(modelJson))
            {
                TaxGrid = JsonConvert.DeserializeObject<List<IssueNRGPDetail>>(modelJson);
            }
            model.FromDateBack = FromDate;
            model.ToDateBack = ToDate;
            model.PartCodeBack = PartCode;
            model.ItemNameBack = ItemName;
            model.VendorNameBack = VendorName;
            model.RGPNRGPBack = RGPNRGP;
            model.ChallanNoBack = ChallanNo;
            model.ChallanTypeBack = ChallanType;
            model.DashboardTypeBack = DashboardType;
            return View(model);
        }

        public IActionResult SendReport(string emailTo = "", int EntryId = 0, int YearCode = 0, string Type = "",string CC1="",string CC2="",string CC3="",string Challanno="")
        {
            string my_connection_string;
            string contentRootPath = _IWebHostEnvironment.ContentRootPath;
            string webRootPath = _IWebHostEnvironment.WebRootPath;
            webReport = new WebReport();
            var ReportName = _IIssueNRGP.GetReportName();
            ViewBag.EntryId = EntryId;
            ViewBag.YearCode = YearCode;
            if (ReportName.Result.Result.Rows[0].ItemArray[0] is string s && s != "")
            //if (!string.Equals(ReportName.Result.Result.Rows[0].ItemArray[0], System.DBNull.Value))
            {
                webReport.Report.Load(webRootPath + "\\" + ReportName.Result.Result.Rows[0].ItemArray[0] + ".frx"); // from database
            }
            else
            {
                webReport.Report.Load(webRootPath + "\\IssueChallan.frx"); // default report

            }
            my_connection_string = _iconfiguration.GetConnectionString("eTactDB");
            webReport.Report.Dictionary.Connections[0].ConnectionString = my_connection_string;
            webReport.Report.Dictionary.Connections[0].ConnectionStringExpression = "";
            webReport.Report.SetParameterValue("entryparam", EntryId);
            webReport.Report.SetParameterValue("yearparam", YearCode);
            webReport.Report.SetParameterValue("MyParameter", my_connection_string);
            webReport.Report.Refresh();


            // Now call EmailReport
            return EmailReport(webReport, emailTo,Challanno,CC1,CC2,CC3);
        }

        public IActionResult EmailReport(WebReport webReport, string emailTo,string Challanno,string CC1,string CC2,string CC3)
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
                        Please find the attachment for the Challan No: <strong>{Challanno}</strong> from AutoComponent.<br/><br/>
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
                            "Soft Copy Of Challan No: " + Challanno + " From AutoComponent",
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
        bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch { return false; }
        }
        public async Task SendEmailAsync(string emailTo, string subject, string message, byte[] attachment = null, string attachmentName = null,string CC1="",string CC2="",string CC3="",string Challanno="" )
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
        }
        public async Task<JsonResult> GetItemRate(int ItemCode, string TillDate, int YearCode, string BatchNo, string UniqueBatchNo, int accountcode)
        {
            var JSON = await _IIssueNRGP.GetItemRate(ItemCode, TillDate, YearCode, BatchNo, UniqueBatchNo, accountcode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetFormRights()
        {
            var userID = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
            var JSON = await _IIssueNRGP.GetFormRights(userID);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public IActionResult PrintReport(int EntryId = 0, int YearCode = 0, string Type = "")
        {

            string my_connection_string;
            string contentRootPath = _IWebHostEnvironment.ContentRootPath;
            string webRootPath = _IWebHostEnvironment.WebRootPath;
            webReport = new WebReport();
            var ReportName = _IIssueNRGP.GetReportName();
            ViewBag.EntryId = EntryId;
            ViewBag.YearCode = YearCode;

            //if (!string.Equals(ReportName.Result.Result.Rows[0].ItemArray[0], System.DBNull.Value))
            if (ReportName.Result.Result.Rows[0].ItemArray[0] is string s && s != "")
            {
                webReport.Report.Load(webRootPath + "\\" + ReportName.Result.Result.Rows[0].ItemArray[0] + ".frx"); // from database
            }
            else
            {
                webReport.Report.Load(webRootPath + "\\IssueChallan.frx"); // default report

            }
            my_connection_string = _iconfiguration.GetConnectionString("eTactDB");
            webReport.Report.Dictionary.Connections[0].ConnectionString = my_connection_string;
            webReport.Report.Dictionary.Connections[0].ConnectionStringExpression = "";
            webReport.Report.SetParameterValue("entryparam", EntryId);
            webReport.Report.SetParameterValue("yearparam", YearCode);
            webReport.Report.SetParameterValue("MyParameter", my_connection_string);
            webReport.Report.Refresh();
            return View(webReport);
        }
        public ActionResult HtmlSave(int EntryId = 0, int YearCode = 0)
        {
            using (Report report = new Report())
            {
                string webRootPath = _IWebHostEnvironment.WebRootPath;
                var webReport = new WebReport();

                webReport.Report.Load(webRootPath + "\\IssueRGPNRGPChallan.frx");
                //webReport.Report.SetParameterValue("flagparam", "PURCHASEORDERPRINT");
                webReport.Report.SetParameterValue("entryparam", EntryId);
                webReport.Report.SetParameterValue("yearparam", YearCode);
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
        public IActionResult GetImage(int EntryId = 0, int YearCode = 0)
        {
           
            using (Report report = new Report())
            {
                string webRootPath = _IWebHostEnvironment.WebRootPath;
                var webReport = new WebReport();


                webReport.Report.Load(webRootPath + "\\IndentPrint.frx");
                //webReport.Report.SetParameterValue("flagparam", "PURCHASEORDERPRINT");
                webReport.Report.SetParameterValue("entryparam", EntryId);
                webReport.Report.SetParameterValue("yearparam", YearCode);
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
        [Route("{controller}/Index")]
        public async Task<IActionResult> IssueNRGP(IssueNRGPModel model, string ShouldPrint, string ShouldEway)
        {
            try
            {
                var INGrid = new DataTable();
                DataTable TaxDetailDT = null;

                string modelJson = HttpContext.Session.GetString("KeyIssueNRGPGrid");
                List<IssueNRGPDetail> NRGPDetail = new List<IssueNRGPDetail>();
                if (!string.IsNullOrEmpty(modelJson))
                {
                    NRGPDetail = JsonConvert.DeserializeObject<List<IssueNRGPDetail>>(modelJson);
                }
                string modelJson1 = HttpContext.Session.GetString("KeyIssueNRGPTaxGrid");
                List<IssueNRGPTaxDetail> TaxGrid = new List<IssueNRGPTaxDetail>();
                if (!string.IsNullOrEmpty(modelJson1))
                {
                    TaxGrid = JsonConvert.DeserializeObject<List<IssueNRGPTaxDetail>>(modelJson1);
                }

                if (NRGPDetail == null)
                {
                    ModelState.Clear();
                    ModelState.TryAddModelError("IssueNRGPGridDetail", "Issue NRGP Grid Should Have Atleast 1 Item...!");
                    model = await BindModel(model);
                    return View("IssueNRGP", model);
                }
                else
                {
                    if (model.CC == null)
                    {
                        model.CC = HttpContext.Session.GetString("Branch");
                    }
                    model.ActualEnteredEMpBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                    model.CreatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                    model.Uid = Convert.ToInt32(HttpContext.Session.GetString("UID"));

                    INGrid = GetDetailTable(NRGPDetail, "");
                    if (TaxGrid != null)
                    {
                        TaxDetailDT = GetTaxDetailTable(TaxGrid);
                    }
                    var Result = await _IIssueNRGP.SaveIssueNRGP(model, INGrid, TaxDetailDT);

                    if (Result != null)
                    {
                        if (Result.StatusText == "Success" && Result.StatusCode == HttpStatusCode.OK)
                        {
                            ViewBag.isSuccess = true;
                            TempData["200"] = "200";
                            if (ShouldPrint == "true")
                            {
                                return Json(new
                                {
                                    status = "Success",
                                    entryId = model.EntryId,
                                    yearCode = model.YearCode
                                });
                            }
                            if (ShouldEway == "true")
                            {
                                return Json(new
                                {
                                    status = "Success",
                                    entryId = model.EntryId,
                                    ChallanNo = model.ChallanNo,
                                    PartCode = model.PartCode,
                                    TransporterName = model.Transporter,
                                    VehicleNo = model.VehicleNo,
                                    Distance = model.Distance,
                                    EnteredByEmpid = model.UpdatedByEmpId,
                                    EnterByMachineName = model.MachineName,
                                    yearCode = model.YearCode
                                });
                            }

                            HttpContext.Session.Remove("KeyIssueNRGPGrid");
                            HttpContext.Session.Remove("KeyIssueNRGPTaxGrid");
                            return Json(new { status = "Success" });
                        }
                        if (Result.StatusText == "Updated" && Result.StatusCode == HttpStatusCode.Accepted)
                        {
                            ViewBag.isSuccess = true;
                            TempData["202"] = "202";
                            if (ShouldPrint == "true")
                            {
                                return Json(new
                                {
                                    status = "Success",
                                    entryId = model.EntryId,
                                    yearCode = model.YearCode
                                });
                            }
                            if (ShouldEway == "true")
                            {
                                return Json(new
                                {
                                    status = "Success",
                                    entryId = model.EntryId,
                                    ChallanNo = model.ChallanNo,
                                    PartCode = model.PartCode,
                                    TransporterName = model.Transporter,
                                    VehicleNo = model.VehicleNo,
                                    Distance = model.Distance,
                                    EnteredByEmpid = model.UpdatedByEmpId,
                                    EnterByMachineName = model.MachineName,
                                    yearCode = model.YearCode
                                });
                            }

                            HttpContext.Session.Remove("KeyIssueNRGPGrid");
                            HttpContext.Session.Remove("KeyIssueNRGPTaxGrid");
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
                                    TempData["500"] = "500";
                                }
                            }
                            else
                            {
                                TempData["500"] = "500";
                            }


                            _logger.LogError("\n \n ********** LogError ********** \n " + JsonConvert.SerializeObject(Result) + "\n \n");
                            //model.IsError = "true";
                            //return View("Error", Result);
                        }
                    }
                    string modelIssueNRGPGrid = HttpContext.Session.GetString("KeyIssueNRGPGrid");
                    List<IssueNRGPDetail> NRGPDetail1 = new List<IssueNRGPDetail>();
                    if (!string.IsNullOrEmpty(modelIssueNRGPGrid))
                    {
                        NRGPDetail1 = JsonConvert.DeserializeObject<List<IssueNRGPDetail>>(modelIssueNRGPGrid);
                    }

                    string modelIssueNRGPTaxGrid = HttpContext.Session.GetString("KeyIssueNRGPTaxGrid");
                    List<IssueNRGPTaxDetail> TaxGrid1 = new List<IssueNRGPTaxDetail>();
                    if (!string.IsNullOrEmpty(modelIssueNRGPTaxGrid))
                    {
                        TaxGrid1 = JsonConvert.DeserializeObject<List<IssueNRGPTaxDetail>>(modelIssueNRGPTaxGrid);
                    }
                    model.IssueNRGPDetailGrid = NRGPDetail1;
                    model.IssueNRGPTaxGrid = TaxGrid1;
                    model.ChallanType = model.ChallanType;
                    model = await BindModel(model);
                    ModelState.Clear();
                    return Json(new { status = "Success" });
                }
            }
            catch (Exception ex)
            {
                LogException<NRGPController>.WriteException(_logger, ex);


                var ResponseResult = new ResponseResult()
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    StatusText = "Error",
                    Result = ex
                };

                return View("Error", ResponseResult);
            }
        }
        public async Task<IActionResult> GenerateEwayBill([FromBody] EInvoiceItemModel input)
        {
            try
            {
                if (input == null)
                    return BadRequest("Invalid input");

                var duplicateIRNResult = await _IEinvoiceService.CheckDuplicateIRN(
                    input.EntryId,
                    input.InvoiceNo,
                    input.YearCode
                );
                var token = await _IEinvoiceService.GetAccessTokenAsync();

                var result = await _IEinvoiceService.CreateIRNAsync(
                    token,
                    input.EntryId,
                    input.InvoiceNo,
                    input.YearCode,
                    input.saleBillType,
                    input.customerPartCode,
                    input.transporterName,
                    input.vehicleNo,
                    input.distanceKM,
                    input.EntrybyId,
                    input.MachineName,
                    "NRGP Challan",
                    input.generateEway,
                    "NRGPEwayBill"
                );
                var responseObj = result.Result as JObject;
                if (responseObj != null)
                {
                    return Ok(new
                    {
                        redirectUrl = (string)responseObj["ewbUrl"],
                        rawResponse = (string)responseObj["rawResponse"]
                    });
                }
                //string ewbUrl = result.Result as string;
                //if (!string.IsNullOrWhiteSpace(ewbUrl))
                //{
                //    return Ok(new { redirectUrl = ewbUrl }); 

                //}
                else
                {
                    return BadRequest("Invoice generation failed");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Server Error: {ex.Message}");
            }
        }
        [HttpPost]
        public async Task<IActionResult> CancelEwayBill(int SaleBillYearCode, string SaleBillNo)
        {
            try
            {

                string invoiceMessage = null;
                var token = await _IEinvoiceService.GetAccessTokenAsync();
                var cancelResult = await _IEinvoiceService.CancelEInvoice(token, SaleBillYearCode, SaleBillNo);

                if (cancelResult?.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                    return StatusCode(500, new { message = cancelResult?.Result ?? "E-Invoice cancellation failed." });

                invoiceMessage = cancelResult?.Result?.ToString();
                return Ok(new { message = invoiceMessage ?? "Cancellation saved successfully!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Server Error: {ex.Message}" });
            }
        }
        public async Task<JsonResult> GetAddressDetails(int AccountCode)
        {
            var JSON = await _IIssueNRGP.GetAddressDetails(AccountCode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }

        public async Task<JsonResult> GetEmails(int AccountCode)
        {
            var JSON = await _IIssueNRGP.GetEmails(AccountCode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetAccountList(string Check)
        {
            var JSON = await _IDataLogic.GetDropDownList("CREDITORDEBTORLIST", Check, "SP_GetDropDownList");
            _logger.LogError(JsonConvert.SerializeObject(JSON));
            return Json(JSON);
        }

        private static DataTable GetTaxDetailTable(List<IssueNRGPTaxDetail> TaxDetailList)
        {
            DataTable Table = new();
            Table.Columns.Add("SeqNo", typeof(int));
            Table.Columns.Add("Type", typeof(string));
            Table.Columns.Add("ItemCode", typeof(int));
            Table.Columns.Add("TaxTypeID", typeof(int));
            Table.Columns.Add("TaxAccountCode", typeof(int));
            Table.Columns.Add("TaxPercentg", typeof(decimal));
            Table.Columns.Add("AddInTaxable", typeof(string));
            Table.Columns.Add("RountOff", typeof(string));
            Table.Columns.Add("Amount", typeof(decimal));
            Table.Columns.Add("TaxRefundable", typeof(string));
            Table.Columns.Add("TaxOnExp", typeof(decimal));
            Table.Columns.Add("Remark", typeof(string));

            foreach (IssueNRGPTaxDetail Item in TaxDetailList)
            {
                Table.Rows.Add(
                    new object[]
                    {
                    Item.SeqNo,
                    Item.TxType,
                    Item.TxItemCode,
                    Item.TxTaxType,
                    Item.TxAccountCode,
                    Item.TxPercentg,
                    Item.TxAdInTxable,
                    Item.TxRoundOff,
                    Item.TxAmount,
                    Item.TxRefundable,
                    Item.TxOnExp,
                    Item.TxRemark ?? "",
                    });
            }

            return Table;
        }
        public async Task<JsonResult> CheckGateEntry(int ID, string Mode, int YC)
        {
            var JSON = await _IIssueNRGP.CheckGateEntry(ID, YC);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillEntryandChallanNo(int YearCode, string RGPNRGP)
        {
            var JSON = await _IIssueNRGP.FillEntryandChallanNo(YearCode, RGPNRGP);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }

        public async Task<JsonResult> IssueChaallanTaxIsMandatory()
        {
            var JSON = await _IIssueNRGP.IssueChaallanTaxIsMandatory();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetVendorList()
        {
            var JSON = await _IIssueNRGP.GetVendorList();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetProcessList()
        {
            var JSON = await _IIssueNRGP.GetProcessList();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetStoreList()
        {
            var JSON = await _IIssueNRGP.GetStoreList();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetBatchInventory()
        {
            var JSON = await _IIssueNRGP.GetBatchInventory();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetAllowBackDate()
        {
            var JSON = await _IIssueNRGP.GetAllowBackDate();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetPrevQty(int EntryId, int YearCode, int ItemCode, string uniqueBatchNo)
        {
            var JSON = await _IIssueNRGP.GetPrevQty(EntryId, YearCode, ItemCode, uniqueBatchNo);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }

        public IActionResult AddIssueNRGPDetail(IssueNRGPDetail model)
        {
            try
            {
                if (model.Mode == "U")
                {
                    string modelJson = HttpContext.Session.GetString("KeyIssueNRGPGrid");
                    List<IssueNRGPDetail> GridDetail = new List<IssueNRGPDetail>();
                    if (!string.IsNullOrEmpty(modelJson))
                    {
                        GridDetail = JsonConvert.DeserializeObject<List<IssueNRGPDetail>>(modelJson);
                    }

                    var MainModel = new IssueNRGPModel();
                    var IssueNrgpGrid = new List<IssueNRGPDetail>();
                    var IssueNGrid = new List<IssueNRGPDetail>();
                    var NRGPGrid = new List<IssueNRGPDetail>();

                    if (model != null)
                    {
                        if (GridDetail == null)
                        {
                            //model.SEQNo = 1;
                            model.ItemNetAmount = decimal.Parse(IssueNGrid.Sum(x => x.Amount).ToString("#.#0"));
                            IssueNGrid.Add(model);
                        }
                        else
                        {
                            if (GridDetail.Where(x => x.ItemCode == model.ItemCode && x.BatchNo == model.BatchNo && x.uniquebatchno == model.uniquebatchno).Any())
                            {
                                return StatusCode(207, "Duplicate");
                            }
                            else
                            {
                                //model.SEQNo = GridDetail.Count + 1;
                                model.ItemNetAmount = decimal.Round(IssueNGrid.Sum(x => x.Amount), 2);
                                IssueNGrid = GridDetail.Where(x => x != null).ToList();
                                NRGPGrid.AddRange(IssueNrgpGrid);
                                IssueNGrid.Add(model);
                            }
                        }
                        IssueNGrid = IssueNGrid.OrderBy(item => item.SEQNo).ToList();
                        MainModel.IssueNRGPDetailGrid = IssueNGrid;
                        MainModel.Mode = "U";

                        HttpContext.Session.SetString("KeyIssueNRGPGrid", JsonConvert.SerializeObject(MainModel.IssueNRGPDetailGrid));
                        HttpContext.Session.SetString("IssueNRGP", JsonConvert.SerializeObject(MainModel));
                    }
                    else
                    {
                        ModelState.TryAddModelError("Error", "Issue NRGP List Cannot Be Empty...!");
                    }

                    return PartialView("_IssueNRGPGrid", MainModel);
                }
                else
                {
                    string modelJson = HttpContext.Session.GetString("KeyIssueNRGPGrid");
                    List<IssueNRGPDetail> GridDetail = new List<IssueNRGPDetail>();
                    if (!string.IsNullOrEmpty(modelJson))
                    {
                        GridDetail = JsonConvert.DeserializeObject<List<IssueNRGPDetail>>(modelJson);
                    }

                    var MainModel = new IssueNRGPModel();
                    var IssueNrgpGrid = new List<IssueNRGPDetail>();
                    var IssueNGrid = new List<IssueNRGPDetail>();
                    var NRGPGrid = new List<IssueNRGPDetail>();

                    if (model != null)
                    {
                        if (GridDetail == null)
                        {
                            //model.SEQNo = 1;
                            model.ItemNetAmount = decimal.Parse(IssueNGrid.Sum(x => x.Amount).ToString("#.#0"));
                            IssueNGrid.Add(model);
                        }
                        else
                        {
                            if (GridDetail.Where(x => x.ItemCode == model.ItemCode && x.BatchNo == model.BatchNo && x.uniquebatchno == model.uniquebatchno).Any())
                            {
                                return StatusCode(207, "Duplicate");
                            }
                            else
                            {
                                //model.SEQNo = GridDetail.Count + 1;
                                model.ItemNetAmount = decimal.Round(IssueNGrid.Sum(x => x.Amount), 2);
                                IssueNGrid = GridDetail.Where(x => x != null).ToList();
                                NRGPGrid.AddRange(IssueNGrid);
                                IssueNGrid.Add(model);
                            }
                        }
                        IssueNGrid = IssueNGrid.OrderBy(item => item.SEQNo).ToList();
                        MainModel.IssueNRGPDetailGrid = IssueNGrid;

                        HttpContext.Session.SetString("KeyIssueNRGPGrid", JsonConvert.SerializeObject(MainModel.IssueNRGPDetailGrid));
                        HttpContext.Session.SetString("IssueNRGP", JsonConvert.SerializeObject(MainModel));

                    }
                    else
                    {
                        ModelState.TryAddModelError("Error", "Issue NRGP List Cannot Be Empty...!");
                    }

                    return PartialView("_IssueNRGPGrid", MainModel);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IActionResult> DeleteByID(int ID, int YC, string machineName, int actuaEntryBy, int ItemCode, string PartCode, string ItemName, string VendorName, string RGPNRGP, string ChallanNo, string ChallanType, string FromDate, string ToDate)
        {
            actuaEntryBy = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));

            var Result = await _IIssueNRGP.DeleteByID(ID, YC, machineName, actuaEntryBy).ConfigureAwait(false);

            if (Result.StatusText == "Deleted" || Result.StatusCode == HttpStatusCode.Gone)
            {
                ViewBag.isSuccess = true;
                TempData["410"] = "410";
            }
            else if (Result.Result.Rows[0].ItemArray[1] == "GateEntryNo")
            {
                ViewBag.isSuccess = false;
                var input = "";
                input = Result.Result.Rows[0].ItemArray[0];
                TempData["ErrorMessage"] = input;
            }
            else
            {
                ViewBag.isSuccess = false;
                TempData["500"] = "500";
            }
            DateTime fromDt = DateTime.ParseExact(FromDate, "dd/MM/yyyy", null);
            string formattedFromDate = fromDt.ToString("dd/MMM/yyyy 00:00:00");
            DateTime toDt = DateTime.ParseExact(ToDate, "dd/MM/yyyy", null);
            string formattedToDate = toDt.ToString("dd/MMM/yyyy 00:00:00");

            return RedirectToAction("Dashboard", new { FromDate = formattedFromDate, ToDate = formattedToDate, ItemCode = ItemCode, PartCode = PartCode, ItemName = ItemName, VendorName = VendorName, RGPNRGP = RGPNRGP, ChallanNo = ChallanNo, ChallanType = ChallanType });
        }

        public IActionResult DeleteItemRow(string SeqNo)
        {
            bool exists = false;
            var model = new IssueNRGPModel();
            string modelJson = HttpContext.Session.GetString("KeyIssueNRGPTaxGrid");
            List<IssueNRGPTaxDetail> TaxGrid = new List<IssueNRGPTaxDetail>();
            if (!string.IsNullOrEmpty(modelJson))
            {
                TaxGrid = JsonConvert.DeserializeObject<List<IssueNRGPTaxDetail>>(modelJson);
            }

            string IssueNRGPGridJson = HttpContext.Session.GetString("KeyIssueNRGPGrid");
            List<IssueNRGPDetail> IssueNRGPGrid = new List<IssueNRGPDetail>();
            if (!string.IsNullOrEmpty(IssueNRGPGridJson))
            {
                IssueNRGPGrid = JsonConvert.DeserializeObject<List<IssueNRGPDetail>>(IssueNRGPGridJson);
            }

            int Indx = Convert.ToInt32(SeqNo) - 1;

            if (IssueNRGPGrid != null)
            {
                var itemfound = IssueNRGPGrid.FirstOrDefault(item => item.SEQNo == Convert.ToInt32(SeqNo)).PartCode;

                var ItmPartCode = (from item in IssueNRGPGrid
                                   where item.SEQNo == Convert.ToInt32(SeqNo)
                                   select item.PartCode).FirstOrDefault();

                if (TaxGrid != null)
                {
                    exists = TaxGrid.Any(x => x.TxPartCode == ItmPartCode);
                }

                if (exists)
                {
                    return StatusCode(207, "Duplicate");
                    //return Problem();
                }

                IssueNRGPGrid.RemoveAt(Convert.ToInt32(Indx));

                Indx = 0;

                foreach (IssueNRGPDetail item in IssueNRGPGrid)
                {
                    Indx++;
                    //  item.SEQNo = Indx;
                }
                model.NetAmount = IssueNRGPGrid.Sum(x => (float)x.Amount);
                model.ItemNetAmount = IssueNRGPGrid.Sum(x => (decimal)x.Amount);
                if (IssueNRGPGrid.Count <= 0)
                {
                    HttpContext.Session.Remove("KeyIssueNRGPTaxGrid");
                    HttpContext.Session.Remove("KeyIssueNRGPGrid");
                }
                else
                {
                    HttpContext.Session.SetString("KeyIssueNRGPGrid", JsonConvert.SerializeObject(IssueNRGPGrid));
                }
                model.IssueNRGPDetailGrid = IssueNRGPGrid;
            }
            return PartialView("_IssueNRGPGrid", model);
        }

        public async Task<IActionResult> Dashboard(int ItemCode = 0, string PartCode = "", string ItemName = "", string VendorName = "", string RGPNRGP = "", string ChallanNo = "", string ChallanType = "", string FromDate = "", string ToDate = "")
        {
            HttpContext.Session.Remove("KeyIssueNRGPGrid");
            HttpContext.Session.Remove("IssueNRGP");
            HttpContext.Session.Remove("KeyIssueNRGPTaxGrid");
            var model = new INDashboard();
            DateTime now = DateTime.Now;
            //DateTime firstDayOfMonth = new DateTime(now.Year, now.Month, 1);
            if (FromDate == "" && ToDate == "")
            {
                model.FromDate = new DateTime(now.Year, now.Month, 1).ToString("dd/MM/yyyy").Replace("-", "/");
                model.ToDate = new DateTime(DateTime.Today.Year + 1, 3, 31).ToString("dd/MM/yyyy").Replace("-", "/");
            }
            else
            {
                model.FromDate = FromDate;
                model.ToDate = ToDate;
            }
            var Result = await _IIssueNRGP.GetDashboardData(model);

            if (Result != null)
            {
                var _List = new List<TextValue>();
                DataSet DS = Result.Result;

                var DT = DS.Tables[0].DefaultView.ToTable(true, "VendorName", "ChallanNo",
                    "ChallanDate", "EntryDate", "DeliveryAddress", "VendorStateCode",
                                "Remarks", "Closed", "EntryId", "YearCode", "RGPNRGP",
                                 "ChallanType", "ActualEnteredEmp", "ActualEntryDate",
                               "UpdatedByEmpName", "UpdatedDate", "MachinName", "SalesPersonEmailId", "eMailFromCC1", "eMailFromCC2", "eMailFromCC3");

                model.INNDashboard = CommonFunc.DataTableToList<IssueNRGPDashboard>(DT, "IssueNRGPDetail");
                model.FromDate1 = FromDate;
                model.ToDate1 = ToDate;
                model.PartCode = PartCode;
                model.ItemCode = ItemCode;
                model.ItemName = ItemName;
                model.VendorName = VendorName;
                model.RGPNRGP = RGPNRGP;
                model.ChallanNo = ChallanNo;
                model.ChallanType = ChallanType;
            }

            return View(model);
        }
        public async Task<IActionResult> GetSearchData(INDashboard model)
        {

            var Result = await _IIssueNRGP.GetDashboardData(model);
            DataSet DS = Result.Result;
            var DT = new DataTable();
            if (model.SummaryDetail == "Detail")
            {
                DT = DS.Tables[0].DefaultView.ToTable(true, "VendorName", "ChallanNo",
                  "ChallanDate", "EntryDate", "DeliveryAddress", "VendorStateCode",
                              "Remarks", "Closed", "EntryId", "YearCode", "RGPNRGP",
                               "ChallanType", "partcode", "ItemNamePartCode", "ItemCode",
                               "HSNNO", "Store", "BatchNo", "uniquebatchno", "Qty", "TotalStock",
                               "BatchStock", "ProcessId", "unit", "Rate", "Amount", "PurchasePrice",
                               "AltQty", "altUnit", "StageDescription", "ActualEnteredEmp", "ActualEntryDate",
                               "UpdatedByEmpName", "UpdatedDate", "MachinName", "PONo", "PoYear", "PODate",
                               "POAmmendNo", "discper", "discamt", "AgainstChallanNoEntryId", "AgainstChallanNo",
                               "AgainstChallanYearCode", "AgainstChallanType", "ItemColor", "ItemSize",
                               "ItemModel", "PendQty", "PendAltQty");
                model.INNDashboard = CommonFunc.DataTableToList<IssueNRGPDashboard>(DT, "IssueNRGP");

            }
            else
            {
                DT = DS.Tables[0].DefaultView.ToTable(true, "VendorName", "ChallanNo",
                   "ChallanDate", "EntryDate", "DeliveryAddress", "VendorStateCode",
                               "Remarks", "Closed", "EntryId", "YearCode", "RGPNRGP",
                                "ChallanType", "ActualEnteredEmp", "ActualEntryDate",
                               "UpdatedByEmpName", "UpdatedDate", "MachinName","SalesPersonEmailId", "eMailFromCC1", "eMailFromCC2", "eMailFromCC3");
                model.INNDashboard = CommonFunc.DataTableToList<IssueNRGPDashboard>(DT, "IssueNRGPDetail");
            }


            return PartialView("_INDashboardGrid", model);
        }
        private static DataTable GetDetailTable(IList<IssueNRGPDetail> DetailList, string Mode)
        {
            var INGrid = new DataTable();

            INGrid.Columns.Add("EntryId", typeof(int));
            INGrid.Columns.Add("YearCode", typeof(int));
            INGrid.Columns.Add("PONO", typeof(string));
            INGrid.Columns.Add("POYearCode", typeof(int));
            INGrid.Columns.Add("PODate", typeof(string));
            INGrid.Columns.Add("POAmendmnetNo", typeof(int));
            INGrid.Columns.Add("SeqNo", typeof(int));
            INGrid.Columns.Add("ItemCode", typeof(int));
            INGrid.Columns.Add("HSNNO", typeof(string));
            INGrid.Columns.Add("unit", typeof(string));
            INGrid.Columns.Add("Qty", typeof(float));
            INGrid.Columns.Add("AltQty", typeof(float));
            INGrid.Columns.Add("AltUnit", typeof(string));
            INGrid.Columns.Add("Rate", typeof(float));
            INGrid.Columns.Add("PurchasePrice", typeof(float));
            INGrid.Columns.Add("Purpose", typeof(string));
            INGrid.Columns.Add("Storeid", typeof(int));
            INGrid.Columns.Add("TotalStock", typeof(float));
            INGrid.Columns.Add("BatchStock", typeof(float));
            INGrid.Columns.Add("BatchNo", typeof(string));
            INGrid.Columns.Add("uniquebatchno", typeof(string));
            INGrid.Columns.Add("discper", typeof(float));
            INGrid.Columns.Add("disamt", typeof(float));
            INGrid.Columns.Add("Amount", typeof(float));
            INGrid.Columns.Add("AgainstChallanNoEntryId", typeof(int));
            INGrid.Columns.Add("AgainstChallanNo", typeof(string));
            INGrid.Columns.Add("AgainstChallanYearCode", typeof(int));
            INGrid.Columns.Add("BatchWise", typeof(char));
            INGrid.Columns.Add("Stockable", typeof(string));
            INGrid.Columns.Add("ItemSize", typeof(string));
            INGrid.Columns.Add("ItemColor", typeof(string));
            INGrid.Columns.Add("ItemModel", typeof(string));
            INGrid.Columns.Add("ProcessId", typeof(int));
            INGrid.Columns.Add("Closed", typeof(string));
            INGrid.Columns.Add("PendQty", typeof(float));
            INGrid.Columns.Add("PendAltQty", typeof(float));
            INGrid.Columns.Add("AgainstChallanType", typeof(string));
            INGrid.Columns.Add("Remark", typeof(string));

            foreach (var Item in DetailList)
            {
                INGrid.Rows.Add(
                    new object[]
                    {
                    Item.EntryId,
                    Item.YearCode,
                    Item.PONO ?? "",
                    Item.POYearCode,
                    Item.PODate == null ? string.Empty : ParseFormattedDate(Item.PODate.Split(" ")[0]),
                    Item.POAmendementNo,
                    Item.SEQNo,
                    Item.ItemCode,
                    Item.HSNNo == null ? 0 : Item.HSNNo,
                    Item.unit ?? "",
                    (float)Math.Round(Convert.ToSingle(Item.Qty),2),
                    Item.AltQty,
                    Item.AltUnit ?? "",
                    (float)Math.Round(Convert.ToSingle(Item.Rate),2),
                    Item.PurchasePrice,
                    Item.Purpose ?? "",
                    Item.Storeid,
                    Item.TotalStock == null ? 0 : Item.TotalStock,
                    Item.BatchStock == null ? 0 : Item.BatchStock,
                    Item.BatchNo ?? "",
                    Item.uniquebatchno ?? "",
                    Item.discper,
                    Item.disamt == float.NaN ? 0:Item.disamt,
                    (decimal)Math.Round(Convert.ToDecimal(Item.Amount),4),
                    Item.AgainstChallanEntryId,
                    Item.AgainstChallanNo ?? "",
                    Item.AgainstChallanYearCode,
                    Item.BatchWise == null ? "A":Item.BatchWise,
                    Item.Stockable ?? "",
                    Item.ItemSize ?? "",
                    Item.ItemColor ?? "",
                    Item.ItemModel ?? "",
                    Item.ProcessId,
                    Item.Closed ?? "",
                    Item.PendQty == 0 ? 0.00 : Item.PendQty ,
                    Item.PendAltQty == 0 ? 0.00 : Item.PendAltQty,
                    Item.AgainstChallanType ?? "",
                    Item.RemarkDetail ?? ""
                    });
            }
            INGrid.Dispose();
            return INGrid;
        }
        private async Task<IssueNRGPModel> BindModel(IssueNRGPModel model)
        {
            var oDataSet = new DataSet();
            var _List = new List<TextValue>();
            oDataSet = await _IIssueNRGP.BindAllDropDowns("BINDALLDROPDOWN");

            if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in oDataSet.Tables[0].Rows)
                {
                    _List.Add(new TextValue
                    {
                        Value = row["Com_Name"].ToString(),
                        Text = row["Com_Name"].ToString()
                    });
                }
                model.BranchList = _List;
                _List = new List<TextValue>();

                foreach (DataRow row in oDataSet.Tables[1].Rows)
                {
                    _List.Add(new TextValue
                    {
                        Value = row["Account_Code"].ToString(),
                        Text = row["Account_Name"].ToString()
                    });
                }
                model.VendorList = _List;
                _List = new List<TextValue>();

                foreach (DataRow row in oDataSet.Tables[2].Rows)
                {
                    _List.Add(new TextValue
                    {
                        Value = row["EntryID"].ToString(),
                        Text = row["StageDescription"].ToString()
                    });
                }
                model.ProcessList = _List;
                _List = new List<TextValue>();
                foreach (DataRow row in oDataSet.Tables[3].Rows)
                {
                    _List.Add(new TextValue
                    {
                        Value = row["Emp_Id"].ToString(),
                        Text = row["EmpNameCode"].ToString()
                    });
                }
                model.EmployeeList = _List;
                _List = new List<TextValue>();
                foreach (DataRow row in oDataSet.Tables[4].Rows)
                {
                    _List.Add(new TextValue
                    {
                        Value = row["entryid"].ToString(),
                        Text = row["store_name"].ToString()
                    });
                }
                model.StoreList = _List;
                _List = new List<TextValue>();

                foreach (DataRow row in oDataSet.Tables[5].Rows)
                {
                    _List.Add(new TextValue
                    {
                        Value = row["Item_Code"].ToString(),
                        Text = row["ItemName"].ToString()
                    });
                }
                model.RecScrapItemNameList = _List;
                _List = new List<TextValue>();

                //foreach (DataRow row in oDataSet.Tables[8].Rows)
                //{
                //    _List.Add(new TextValue
                //    {
                //        Value = row["Item_Code"].ToString(),
                //        Text = row["PartCode"].ToString()
                //    });
                //}
                //model.RecScrapPartCodeList = _List;
                //_List = new List<TextValue>();


            }
            return model;
        }
        public async Task<JsonResult> FillCurrentBatchINStore(int ItemCode, int YearCode, string StoreName, string batchno)
        {
            var FinStartDate = HttpContext.Session.GetString("FromDate");
            var JSON = await _IIssueNRGP.FillCurrentBatchINStore(ItemCode, YearCode, FinStartDate, StoreName, batchno);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetBatchNumber(int StoreId, string StoreName, int ItemCode, string TransDate, int YearCode, string BatchNo)
        {
            var FinStartDate = HttpContext.Session.GetString("FromDate");
            FinStartDate = ParseFormattedDate(FinStartDate);
            TransDate = ParseFormattedDate(TransDate);
            var JSON = await _IIssueNRGP.GetBatchNumber("FillCurrentBatchINStore", StoreId, FinStartDate, StoreName, ItemCode, TransDate, YearCode, BatchNo);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetStoreTotalStock(int ItemCode, int StoreId, string TillDate)
        {
            var JSON = await _IIssueNRGP.GetStoreTotalStock("GETSTORETotalSTOCK", ItemCode, StoreId, TillDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetBatchStockQty(int ItemCode, int StoreId, string TillDate, string BatchNo, string UniqueBatchNo)
        {
            var JSON = await _IIssueNRGP.GetBatchStockQty("GETSTORESTOCKBATCHWISE", ItemCode, StoreId, TillDate, BatchNo, UniqueBatchNo);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetPONOByAccount(int AccountCode, string PONO, int POYear, int ItemCode)
        {
            var JSON = await _IIssueNRGP.GetPONOByAccount("PONOBYACCOUNT", AccountCode, PONO, POYear, ItemCode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillAgainstChallanNo(int AccountCode, int ItemCode, int YearCode, string ChallanDate)
        {
            var JSON = await _IIssueNRGP.FillAgainstChallanNo("ChallanAgainstRec", AccountCode, ItemCode, YearCode, ChallanDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillAgainstChallanYC(int AccountCode, int ItemCode, int YearCode, string ChallanDate, string AgainstChallanNo)
        {
            var JSON = await _IIssueNRGP.FillAgainstChallanYC("ChallanAgainstRecYearCode", AccountCode, ItemCode, YearCode, ChallanDate, AgainstChallanNo);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillAgainstChallanEntryId(int AccountCode, int ItemCode, string ChallanDate, string AgainstChallanNo, string AgainstChallanYC)
        {
            var JSON = await _IIssueNRGP.FillAgainstChallanEntryId("ChallanAgainstRecYearCode", AccountCode, ItemCode, ChallanDate, AgainstChallanNo, AgainstChallanYC);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetAllItems()
        {
            var JSON = await _IIssueNRGP.GetAllItems("FillItem");
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillItemName()
        {
            var JSON = await _IIssueNRGP.FillItemName("FillItem");
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> CheckItems()
        {
            var JSON = await _IIssueNRGP.CheckItems("CheckItems");
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillChallanType(string RGPNRGP)
        {
            var JSON = await _IIssueNRGP.FillChallanType(RGPNRGP);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> StockableItems(int ItemCode)
        {
            var JSON = await _IIssueNRGP.StockableItems("StockableItems", ItemCode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }

        public async Task<JsonResult> EditItemRow(int SeqNo)
        {
            var MainModel = new IssueNRGPModel();
            string modelJson = HttpContext.Session.GetString("KeyIssueNRGPGrid");
            List<IssueNRGPDetail> INGrid = new List<IssueNRGPDetail>();
            if (!string.IsNullOrEmpty(modelJson))
            {
                INGrid = JsonConvert.DeserializeObject<List<IssueNRGPDetail>>(modelJson);
            }
            var INDetail = INGrid.Where(x => x.SEQNo == SeqNo);
            string JsonString = JsonConvert.SerializeObject(INDetail);
            return Json(JsonString);
        }
    }
}
