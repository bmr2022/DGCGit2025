using eTactWeb.Data.Common;
using FastReport;
using FastReport.Export.Html;
using FastReport.Export.Image;
using FastReport.Web;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using static eTactWeb.Data.Common.CommonFunc;
using static eTactWeb.DOM.Models.Common;
using eTactWeb.DOM.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using eTactWeb.Services.Interface;
using System.Net;
using System.Globalization;
using System.Data;
using DocumentFormat.OpenXml.Drawing.Diagrams;
using DocumentFormat.OpenXml.Wordprocessing;
using PdfSharp.Drawing.BarCodes;
using PdfSharp.Pdf.Content.Objects;
using System.Net.Http.Headers;
using DocumentFormat.OpenXml.Office2010.Excel;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using ClosedXML.Excel;
using System.Collections.Generic;

namespace eTactWeb.Controllers
{

    [Authorize]
    public class GateInwardController : Controller
    {
        private readonly IDataLogic _IDataLogic;
        public readonly IEinvoiceService _IEinvoiceService;
        public EncryptDecrypt EncryptDecrypt { get; }
        public IGateInward _IGateInward { get; }
        private readonly ILogger<GateInwardController> _logger;
        private readonly IConfiguration iconfiguration;
        public IWebHostEnvironment _IWebHostEnvironment { get; }
        private readonly ConnectionStringService _connectionStringService;
        public GateInwardController(ILogger<GateInwardController> logger, IDataLogic iDataLogic, IGateInward iGateInward, EncryptDecrypt encryptDecrypt, IWebHostEnvironment iWebHostEnvironment, IConfiguration iconfiguration, ConnectionStringService connectionStringService, IEinvoiceService IEinvoiceService)
        {
            _logger = logger;
            _IDataLogic = iDataLogic;
            _IGateInward = iGateInward;
            _IWebHostEnvironment = iWebHostEnvironment;
            this.iconfiguration = iconfiguration;
            _connectionStringService = connectionStringService;
            _IEinvoiceService = IEinvoiceService;
            EncryptDecrypt = encryptDecrypt;
        }
        public IActionResult PrintReport(int EntryId = 0, int YearCode = 0)
        {
            string my_connection_string;
            string contentRootPath = _IWebHostEnvironment.ContentRootPath;
            string webRootPath = _IWebHostEnvironment.WebRootPath;
            //string frx = Path.Combine(_env.ContentRootPath, "reports", value.file);
            var webReport = new WebReport();

            webReport.Report.Load(webRootPath + "\\GateEntry.frx"); // default report


            //webReport.Report.SetParameterValue("flagparam", "PURCHASEORDERPRINT");
            webReport.Report.SetParameterValue("entryparam", EntryId);
            webReport.Report.SetParameterValue("yearparam", YearCode);

            my_connection_string = _connectionStringService.GetConnectionString();
            //my_connection_string = iconfiguration.GetConnectionString("eTactDB");
            //my_connection_string = "Data Source=192.168.1.224\\sqlexpress;Initial  Catalog = etactweb; Integrated Security = False; Persist Security Info = False; User
            //         ID = web; Password = bmr2401";
            webReport.Report.SetParameterValue("MyParameter", my_connection_string);


            // webReport.Report.SetParameterValue("accountparam", 1731);


            // webReport.Report.Dictionary.Connections[0].ConnectionString = @"Data Source=103.10.234.95;AttachDbFilename=;Initial Catalog=eTactWeb;Integrated Security=False;Persist Security Info=True;User ID=web;Password=bmr2401";
            //ViewBag.WebReport = webReport;
            return View(webReport);
        }
        [HttpGet]
        public async Task<IActionResult> GetEwayBillData(string ewayBillNo)
        {
            try
            {
                if (string.IsNullOrEmpty(ewayBillNo))
                {
                    return Json(new { success = false, error = "E-Way Bill number is required" });
                }

                string GSTNo = string.Empty;
                var token = await _IEinvoiceService.GetAccessTokenAsync();
                var gstResult = await _IEinvoiceService.GETGSTNO();

                if (gstResult.Result != null && gstResult.Result.Rows.Count > 0)
                {
                    GSTNo = gstResult.Result.Rows[0]["GSTIN"].ToString();
                }

                string apiUrl = $"https://pro.mastersindia.co/getEwayBillData" +
                                $"?access_token={token}" +
                                $"&action=GetEwayBill" +
                                $"&gstin={GSTNo}" +
                                $"&eway_bill_number={ewayBillNo}";

                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(
                        new MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage response = await client.GetAsync(apiUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        string jsonResponse = await response.Content.ReadAsStringAsync();

                        // ✅ RETURN the result of EwayBillToGateInward
                        return await EwayBillToGateInward(jsonResponse).ConfigureAwait(false);
                    }
                    else
                    {
                        return Json(new { success = false, message = "Failed to fetch E-Way Bill data from external API" });
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }


        public async Task<IActionResult> EwayBillToGateInward(string issueDataJson)
        {
            try
            {
                // Deserialize E-way Bill response
                var ewayResponse = JsonConvert.DeserializeObject<EwayBillResponse>(issueDataJson);

                var IssueGrid = new List<GateInwardModel>();
                var docTypeId = 1;

                if (ewayResponse?.results?.message != null)
                {
                    var message = ewayResponse.results.message;
                    var AccountCodeId = _IGateInward.GetAccountCode(message.legal_name_of_consignee);
                    int AccountCode = 0;

                    if (AccountCodeId.Result.Result != null && AccountCodeId.Result.Result.Tables.Count > 0)
                    {
                        var table = AccountCodeId.Result.Result.Tables[0];
                        if (table.Rows.Count > 0)
                        {
                            AccountCode = Convert.ToInt32(table.Rows[0].ItemArray[0]);
                        }
                    }
                    if (AccountCode == 0)
                    {
                        return Json(new
                        {
                            success = false,
                            message = $"No valid Account Found : {message.legal_name_of_consignee}"
                        });
                    }
                    int seqNo = 1; // Initialize sequence number

                    foreach (var item in message.itemList)
                    {
                        var ItemCodeId = _IGateInward.GetItemCode(item.product_description);
                        int itemcode = 0;
                        string PartCode = string.Empty;
                        string AltUnit = string.Empty;

                        if (ItemCodeId.Result.Result != null && ItemCodeId.Result.Result.Tables.Count > 0)
                        {
                            var table = ItemCodeId.Result.Result.Tables[0];
                            if (table.Rows.Count > 0)
                            {
                                itemcode = Convert.ToInt32(table.Rows[0].ItemArray[0]);   // Item_Code
                                AltUnit = table.Rows[0].ItemArray[1].ToString();         // AlternateUnit
                                PartCode = table.Rows[0].ItemArray[2].ToString();        // PartCode
                            }
                        }
                        if (itemcode == 0 || string.IsNullOrWhiteSpace(PartCode))
                        {
                            return Json(new
                            {
                                success = false,
                                message = $"No Item found in ItemMaster : {item.product_description}"
                            });
                        }
                        var inward = new GateInwardModel
                        {
                            SeqNo = seqNo++, // Assign sequence number and increment
                            Invoiceno = message.document_number,
                            InvoiceDate = message.document_date,
                            AccountCode = AccountCode,
                            Address = message.address1_of_consignor,
                            docTypeId = docTypeId,
                            PartCode = PartCode,
                            ItemCode = itemcode,
                            ItemName = item.product_description,
                            Unit = item.unit_of_product,
                            Qty = item.quantity,
                            Rate = item.taxable_amount / item.quantity, // Calculate rate
                            Remarks = $"EwayBill No: {message.eway_bill_number}",
                            AltUnit = AltUnit,
                            AltQty = 0
                        };

                        IssueGrid.Add(inward);
                    }
                }

                // Save list into Session
                HttpContext.Session.SetString("KeyGateInwardGrid", JsonConvert.SerializeObject(IssueGrid));

                var GIGrid = new DataTable();

                string modelJson = HttpContext.Session.GetString("KeyGateInwardGrid");
                List<GateInwardModel> modelList = string.IsNullOrEmpty(modelJson)
                     ? new List<GateInwardModel>()
                     : JsonConvert.DeserializeObject<List<GateInwardModel>>(modelJson);

                // Pick one model (e.g. first one) if GetEwayBillDataforPo needs a single model
                GateInwardModel model = modelList.FirstOrDefault() ?? new GateInwardModel();

                List<GateInwardItemDetail> GateInwardItemDetail = new List<GateInwardItemDetail>();
                if (!string.IsNullOrEmpty(modelJson))
                {
                    GateInwardItemDetail = JsonConvert.DeserializeObject<List<GateInwardItemDetail>>(modelJson);
                }

                GIGrid = GetDetailTable(GateInwardItemDetail);
                model = await _IGateInward.GetEwayBillDataforPo(model, GIGrid);


                HttpContext.Session.Remove("KeyGateInwardItemDetail");
                string serializedGrid = JsonConvert.SerializeObject(model);
                HttpContext.Session.SetString("KeyGateInwardGrid", serializedGrid);



                return Json(new { success = true, message = "E-Way Bill saved to Gate Inward session successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public ActionResult HtmlSave(int EntryId = 0, int YearCode = 0)
        {
            using (Report report = new Report())
            {
                string webRootPath = _IWebHostEnvironment.WebRootPath;
                var webReport = new WebReport();


                webReport.Report.Load(webRootPath + "\\GateInwardPrint.frx");
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
            // Creatint the Report object
            using (Report report = new Report())
            {
                string webRootPath = _IWebHostEnvironment.WebRootPath;
                var webReport = new WebReport();


                webReport.Report.Load(webRootPath + "\\GateInwardPrint.frx");
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
        //public IActionResult Index()
        //{
        //    return View();
        //}

        //[Route("GateInward/Index")]
        public async Task<IActionResult> GateInward()
        {
            ViewData["Title"] = "Inventory Details";
            //TempData.Clear();
            HttpContext.Session.Remove("KeyGateInwardGrid");
            var model = await BindModels(null);
            model.FinFromDate = CommonFunc.ParseFormattedDate(HttpContext.Session.GetString("FromDate"));
            model.FinToDate = CommonFunc.ParseFormattedDate(HttpContext.Session.GetString("ToDate"));

            model.YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
            model.CC = HttpContext.Session.GetString("Branch");
            model.PreparedByEmp = HttpContext.Session.GetString("EmpName");
            model.ActualEnteredByName = HttpContext.Session.GetString("EmpName");
            model.CreatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));

            string serializedGrid = JsonConvert.SerializeObject(model);
            HttpContext.Session.SetString("KeyGateInwardGrid", serializedGrid);
            return View(model);
        }
        //[Route("GateInward/Index")]
        //added encrption and rights code by sajni
        [HttpGet]
        public async Task<ActionResult> GateInward(int ID, string Mode, int YC, string FromDate = "", string ToDate = "", string VendorName = "", string GateNo = "", string PartCode = "", string ItemName = "", string DocName = "", string PONO = "", string ScheduleNo = "", string Searchbox = "", string DashboardType = "", string AccountCode = "", string docTypeId = "", string Invoiceno = "", int VPSaleBillEntryId = 0)//, ILogger logger)
        {
            //HttpContext.Session.Remove("KeyGateInwardGrid");
            HttpContext.Session.Remove("KeyGateInwardItemDetail");
            int userID = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
            var rights = await _IGateInward.GetFormRights(userID);
            if (rights?.Result == null || rights.Result.Tables.Count == 0 || rights.Result.Tables[0].Rows.Count == 0)
            {
                return RedirectToAction("Dashboard", "Home");
            }

            var table = rights.Result.Tables[0];
            string encID = RouteData.Values["id"]?.ToString();
            string encYC = Request.Query["YC"].ToString();

            string encAccountCode = Request.Query["AccountCode"];
            string encDocTypeId = Request.Query["docTypeId"];
            string encInvoiceNo = Request.Query["InvoiceNo"];
            string encVPSaleBillEntryId = Request.Query["VPSaleBillEntryId"];



            if (!string.IsNullOrEmpty(encID) && !string.IsNullOrEmpty(encYC))
            {
                int decryptedID = EncryptDecrypt.DecodeID(encID);
                int decryptedYC = EncryptDecrypt.DecodeID(encYC);
                string decryptedMode = EncryptDecrypt.Decrypt(Mode);
                ID = decryptedID;
                YC = decryptedYC;
                Mode = decryptedMode;

            }
            if (!string.IsNullOrEmpty(encAccountCode) && !string.IsNullOrEmpty(encDocTypeId))
            {
                int decryptedAccountCode = EncryptDecrypt.DecodeID(encAccountCode);
                int decryptedDocTypeId = EncryptDecrypt.DecodeID(encDocTypeId);
                string decryptedMode = EncryptDecrypt.Decrypt(Mode);

                AccountCode = decryptedAccountCode.ToString();
                docTypeId = decryptedDocTypeId.ToString();
                Mode = decryptedMode;

            }
            if (!string.IsNullOrEmpty(encVPSaleBillEntryId))
            {
                VPSaleBillEntryId = EncryptDecrypt.DecodeID(encVPSaleBillEntryId);
            }

            if (!string.IsNullOrEmpty(encInvoiceNo))
            {
                Invoiceno = Uri.UnescapeDataString(encInvoiceNo);
            }
            bool optAll = Convert.ToBoolean(table.Rows[0]["OptAll"]);
            bool optView = Convert.ToBoolean(table.Rows[0]["OptView"]);
            bool optUpdate = Convert.ToBoolean(table.Rows[0]["OptUpdate"]);
            bool optSave = Convert.ToBoolean(table.Rows[0]["OptSave"]);


            if (Mode == "U")
            {
                if (!(optUpdate))
                {
                    return RedirectToAction("Dashboard", "Home");
                }
            }
            else if (Mode == "V")
            {
                if (!(optView))
                {
                    return RedirectToAction("Dashboard", "Home");
                }
            }
            else if (ID <= 0)
            {
                if (!optSave)
                {
                    return RedirectToAction("DashBoard", " GateInward");
                }

            }

            _logger.LogInformation("\n \n ********** Page Gate Inward ********** \n \n " + _IWebHostEnvironment.EnvironmentName.ToString() + "\n \n");
            //TempData.Clear();
            var MainModel = new GateInwardModel();
            MainModel.FinFromDate = HttpContext.Session.GetString("FromDate");
            MainModel.FinToDate = HttpContext.Session.GetString("ToDate");
            MainModel.YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
            MainModel.CC = HttpContext.Session.GetString("Branch");
            MainModel.PreparedByEmp = HttpContext.Session.GetString("EmpName");
            //MainModel.ActualEnteredByName = HttpContext.Session.GetString("EmpName");
            MainModel.CreatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
            if (Mode == "I" && ID == 0)
            {
                MainModel.AccountCode = Convert.ToInt32(AccountCode.ToString());
                MainModel.docTypeId = Convert.ToInt32(docTypeId.ToString());
                MainModel.Invoiceno = Invoiceno.ToString();
                MainModel.VPSaleBillEntryId = Convert.ToInt32(VPSaleBillEntryId.ToString());

                var selectedJson = HttpContext.Session.GetString("KeyGateInwardGrid");
                if (!string.IsNullOrEmpty(selectedJson))
                {
                    var selectedItems = JsonConvert.DeserializeObject<List<GateInwardItemDetail>>(selectedJson);
                    MainModel.ItemDetailGrid = selectedItems;

                }
            }
            if (Mode == "Eway" && ID == 0)
            {

                var selectedJson = HttpContext.Session.GetString("KeyGateInwardGrid");
                if (!string.IsNullOrEmpty(selectedJson))
                {
                    var mainModel = JsonConvert.DeserializeObject<GateInwardModel>(selectedJson);

                    if (mainModel != null)
                    {
                        // Map main properties
                        MainModel.AccountCode = mainModel.AccountCode;
                        MainModel.Invoiceno = mainModel.Invoiceno ?? string.Empty;
                        MainModel.Address = mainModel.Address ?? string.Empty;
                        MainModel.InvoiceDate = mainModel.InvoiceDate ?? string.Empty;

                        // Get ItemDetailGrid
                        MainModel.ItemDetailGrid = mainModel.ItemDetailGrid ?? new List<GateInwardItemDetail>();
                    }

                }
            }
            if (!string.IsNullOrEmpty(Mode) && ID > 0 && (Mode == "V" || Mode == "U"))
            {
                HttpContext.Session.Remove("KeyGateInwardItemDetail");

                MainModel = await _IGateInward.GetViewByID(ID, YC).ConfigureAwait(false);
                MainModel.Mode = Mode;
                MainModel.ID = ID;
                MainModel = await BindModels(MainModel).ConfigureAwait(false);
                MainModel.FinFromDate = HttpContext.Session.GetString("FromDate");
                MainModel.FinToDate = HttpContext.Session.GetString("ToDate");

                string serializedGrid = JsonConvert.SerializeObject(MainModel.ItemDetailGrid);
                HttpContext.Session.SetString("KeyGateInwardItemDetail", serializedGrid);
            }
            else
            {
                MainModel = await BindModels(MainModel);
            }
            if (Mode != "U")
            {
                MainModel.ActualEnteredBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                MainModel.ActualEnteredByName = HttpContext.Session.GetString("EmpName");
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
            MainModel.GateNoBack = GateNo;
            MainModel.PartCodeBack = PartCode;
            MainModel.ItemNameBack = ItemName;
            MainModel.DocTypeBack = DocName;
            MainModel.PoNOBack = PONO;
            MainModel.SchNoBack = ScheduleNo;
            MainModel.GlobalSearchBack = Searchbox;
            MainModel.DashboardTypeBack = DashboardType;
            MainModel.VendorNameBack = VendorName;
            return View(MainModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GateInward(GateInwardModel model)
        {
            try
            {
                var GIGrid = new DataTable();

                string modelJson = HttpContext.Session.GetString("KeyGateInwardGrid");
                List<GateInwardItemDetail> GateInwardItemDetail = new List<GateInwardItemDetail>();
                if (!string.IsNullOrEmpty(modelJson))
                {
                    GateInwardItemDetail = JsonConvert.DeserializeObject<List<GateInwardItemDetail>>(modelJson);
                }
                string modelEditJson = HttpContext.Session.GetString("KeyGateInwardItemDetail");
                List<GateInwardItemDetail> GateInwardItemDetailEdit = new List<GateInwardItemDetail>();
                if (!string.IsNullOrEmpty(modelEditJson))
                {
                    GateInwardItemDetailEdit = JsonConvert.DeserializeObject<List<GateInwardItemDetail>>(modelEditJson);
                }

                if (GateInwardItemDetail == null && GateInwardItemDetailEdit == null)
                {
                    ModelState.Clear();
                    ModelState.TryAddModelError("GateInwardItemDetail", "Gate Inward Grid Should Have Atleast 1 Item...!");
                    model = await BindModels(model);
                    return View("GateInward", model);
                }

                else
                {
                    model.CC = HttpContext.Session.GetString("Branch");
                    model.PreparedByEmpId = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
                    //model.ActualEnteredBy   = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
                    model.CreatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                    model.IPAddress = HttpContext.Session.GetString("ClientIP");
                    if (model.Mode == "U")
                    {
                        model.UpdatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                        model.UpdatedByName = HttpContext.Session.GetString("EmpName");
                        GIGrid = GetDetailTable(GateInwardItemDetailEdit);
                    }
                    else
                    {
                        GIGrid = GetDetailTable(GateInwardItemDetail);
                    }
                    model.EntrybyMachineName = HttpContext.Session.GetString("ClientMachineName");
                    model.IPAddress = HttpContext.Session.GetString("ClientIP");
                    var Result = await _IGateInward.SaveGateInward(model, GIGrid);

                    if (Result != null)
                    {
                        var dt = Result?.Result;

                        string? gateNo = null;

                        if (Result.StatusText == "Success" && Result.StatusCode == HttpStatusCode.OK)
                        {


                            if (dt != null && dt.Rows.Count > 0)
                            {
                                gateNo = (dt.Rows[0]["GateNo"]);

                            }


                            ViewBag.isSuccess = true;
                            TempData["200"] = $"Data saved successfully OF Gate No: {gateNo}";
                            HttpContext.Session.Remove("KeyGateInwardItemDetail");
                            HttpContext.Session.Remove("KeyGateInwardGrid");
                            model.AccountCode = 0;
                            return Json(new
                            {
                                success = true,
                                message = $"Data saved successfully OF Gate No: {gateNo}",
                                redirectUrl = Url.Action(
      "GateInward",
      "GateInward"

  )
                            });
                        }
                        else if (Result.StatusText == "Success" && Result.StatusCode == HttpStatusCode.Accepted)
                        {
                            if (dt != null && dt.Rows.Count > 0)
                            {
                                gateNo = (dt.Rows[0]["GateNo"]);

                            }
                            ViewBag.isSuccess = true;
                            TempData["202"] = $"Data saved successfully OF Gate No: {gateNo}";
                            model.AccountCode = 0;
                            return Json(new
                            {
                                success = true,
                                message = $"Data saved successfully OF Gate No: {gateNo}",
                                redirectUrl = Url.Action(
     "GateInward",
     "GateInward"

 )
                            });
                        }
                        else if (Result.StatusText == "Duplicate")
                        {
                            //string gateNo = string.Empty;
                            gateNo = Result.Result.Rows[0]["Result"].ToString();
                            ViewBag.isSuccess = false;
                            TempData["409"] = "409";
                            return Json(new
                            {
                                success = false,
                                message = Result.StatusText
                            });
                        }
                        else if (Result.StatusText == "Unapproved")
                        {
                            ViewBag.isSuccess = false;
                            TempData["2627"] = "2627";
                            return Json(new
                            {
                                success = false,
                                message = Result.StatusText
                            });
                        }
                        //else if (!string.IsNullOrEmpty(Result.StatusText))
                        //{
                        //    ViewBag.isSuccess = false;
                        //    // If SP returned a message (like adjustment error)
                        //    TempData["ErrorMessage"] = Result.StatusText;
                        //    //return View(model);
                        //}
                        else if (!string.IsNullOrEmpty(Result.StatusText))
                        {
                            // If SP returned a message (like adjustment error)
                            return Json(new
                            {
                                success = false,
                                message = Result.StatusText
                            });
                        }
                        else if (Result.StatusText == "Error" && Result.StatusCode == HttpStatusCode.InternalServerError)
                        {
                            var errNum = Result.Result.Message.ToString().Split(":")[1];
                            if (errNum == " 2627")
                            {
                                ViewBag.isSuccess = false;
                                TempData["2627"] = "2627";
                                _logger.LogError("\n \n ********** LogError ********** \n " + JsonConvert.SerializeObject(Result) + "\n \n");
                                var model2 = await BindModels(null);
                                model2.FinFromDate = HttpContext.Session.GetString("FromDate");
                                model2.FinToDate = HttpContext.Session.GetString("ToDate");
                                model2.YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
                                model2.CC = HttpContext.Session.GetString("Branch");
                                model2.PreparedByEmp = HttpContext.Session.GetString("EmpName");
                                model2.ActualEnteredByName = HttpContext.Session.GetString("EmpName");
                                model2.CreatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                                //return View(model2);
                                return Json(new
                                {
                                    success = false,
                                    message = "An unexpected error occurred."
                                });
                            }
                            else
                            {
                                TempData["500"] = "500";
                                model = await BindModels(model);
                                model.FinFromDate = HttpContext.Session.GetString("FromDate");
                                model.FinToDate = HttpContext.Session.GetString("ToDate");
                                model.YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
                                model.CC = HttpContext.Session.GetString("Branch");
                                model.PreparedByEmp = HttpContext.Session.GetString("EmpName");
                                model.ActualEnteredByName = HttpContext.Session.GetString("EmpName");
                                model.CreatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                                model.ItemDetailGrid = GateInwardItemDetail;
                                //return View(model);
                                return Json(new
                                {
                                    success = false,
                                    message = "An unexpected error occurred."
                                });
                            }
                        }
                        return Json(new
                        {
                            success = false,
                            message = Result.StatusText
                        });
                    }
                    var model1 = await BindModels(null);
                    model1.AccountCode = 0;
                    model1.FinFromDate = HttpContext.Session.GetString("FromDate");
                    model1.FinToDate = HttpContext.Session.GetString("ToDate");
                    model1.YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
                    model1.CC = HttpContext.Session.GetString("Branch");
                    model1.PreparedByEmp = HttpContext.Session.GetString("EmpName");
                    model1.ActualEnteredByName = HttpContext.Session.GetString("EmpName");
                    model1.CreatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                    return Json(new
                    {
                        success = false,
                        message = "An unexpected error occurred."
                    });
                    //return RedirectToAction("GateInward");

                }
            }
            catch (Exception ex)
            {
                LogException<GateInwardController>.WriteException(_logger, ex);


                var ResponseResult = new ResponseResult()
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    StatusText = "Error",
                    Result = ex
                };
                return Json(new
                {
                    success = false,
                    message = "An unexpected error occurred."
                });
                // return View("Error", ResponseResult);
            }
        }
        public async Task<JsonResult> GetFormRights()
        {
            var userID = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
            var JSON = await _IGateInward.GetFormRights(userID);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }

        public IActionResult ClearGrid()
        {
            HttpContext.Session.Remove("KeyGateInwardGrid");
            var MainModel = new GateInwardModel();
            return PartialView("_GateInwardGrid", MainModel);
        }
        public async Task<JsonResult> GetFeatureOption()
        {
            var JSON = await _IGateInward.GetFeatureOption();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<IActionResult> GetSearchData(string VendorName, string Gateno, string ItemName, string PartCode, string DocName, string PONO, string ScheduleNo, string FromDate, string ToDate, string DashboardType, int pageNumber = 1, int pageSize = 10, string SearchBox = "")
        {
            int userID = Convert.ToInt32(HttpContext.Session.GetString("UID"));

            //model.Mode = "Search";
            var model = new GateInwardDashboard();
            model = await _IGateInward.GetDashboardData(VendorName, Gateno, ItemName, PartCode, DocName, PONO, ScheduleNo, FromDate, ToDate, DashboardType, userID);
            model.DashboardType = "Summary";
            var modelList = model?.GateDashboard ?? new List<GateInwardDashboard>();


            if (string.IsNullOrWhiteSpace(SearchBox))
            {
                model.TotalRecords = modelList.Count();
                model.PageNumber = pageNumber;
                model.PageSize = pageSize;
                model.GateDashboard = modelList
                .Skip((pageNumber - 1) * pageSize)
                   .Take(pageSize)
                   .ToList();
            }
            else
            {
                List<GateInwardDashboard> filteredResults;
                if (string.IsNullOrWhiteSpace(SearchBox))
                {
                    filteredResults = modelList.ToList();
                }
                else
                {
                    filteredResults = modelList
                        .Where(i => i.GetType().GetProperties()
                            .Where(p => p.PropertyType == typeof(string))
                            .Select(p => p.GetValue(i)?.ToString())
                            .Any(value => !string.IsNullOrEmpty(value) &&
                                          value.Contains(SearchBox, StringComparison.OrdinalIgnoreCase)))
                        .ToList();


                    if (filteredResults.Count == 0)
                    {
                        filteredResults = modelList.ToList();
                    }
                }

                model.TotalRecords = filteredResults.Count;
                model.GateDashboard = filteredResults.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
                model.PageNumber = pageNumber;
                model.PageSize = pageSize;
            }
            MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                SlidingExpiration = TimeSpan.FromMinutes(55),
                Size = 1024,
            };

            string serializedGrid = JsonConvert.SerializeObject(modelList);
            HttpContext.Session.SetString("KeyGateInardList", serializedGrid);
            return PartialView("_GateInwardDashboardGrid", model);
        }
        public async Task<IActionResult> GetPendingGateEntrySearchData(
    int AccountCode,
    int docTypeId,
    string PoNo,
    int PoYearCode,
    int ItemCode,
    string FromDate,
    string ToDate,
    string ScheduleNo,
    string DashboardType,
    string PartCode,
    string ItemName,
    string GetDataFrom,
    string Invoiceno,

    int pageNumber = 1,
    int pageSize = 10,
    string SearchBox = "")

        {
            //model.Mode = "Search";
            var model = new PendingGateInwardDashboard();
            model = await _IGateInward.GetPendingGateEntryDashboardData(AccountCode, docTypeId, PoNo, PoYearCode, ItemCode, FromDate, ToDate, PartCode, ItemName, GetDataFrom, Invoiceno);

            var modelList = model?.PendingGateEntryDashboard ?? new List<PendingGateInwardDashboard>();


            if (string.IsNullOrWhiteSpace(SearchBox))
            {
                model.TotalRecords = modelList.Count();
                model.PageNumber = pageNumber;
                model.PageSize = pageSize;
                model.PendingGateEntryDashboard = modelList
                .Skip((pageNumber - 1) * pageSize)
                   .Take(pageSize)
                   .ToList();
            }
            else
            {
                List<PendingGateInwardDashboard> filteredResults;
                if (string.IsNullOrWhiteSpace(SearchBox))
                {
                    filteredResults = modelList.ToList();
                }
                else
                {
                    filteredResults = modelList
                        .Where(i => i.GetType().GetProperties()
                            .Where(p => p.PropertyType == typeof(string))
                            .Select(p => p.GetValue(i)?.ToString())
                            .Any(value => !string.IsNullOrEmpty(value) &&
                                          value.Contains(SearchBox, StringComparison.OrdinalIgnoreCase)))
                        .ToList();


                    if (filteredResults.Count == 0)
                    {
                        filteredResults = modelList.ToList();
                    }
                }

                model.TotalRecords = filteredResults.Count;
                model.PageNumber = pageNumber;
                model.PageSize = pageSize;
            }
            MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                SlidingExpiration = TimeSpan.FromMinutes(55),
                Size = 1024,
            };

            string serializedGrid = JsonConvert.SerializeObject(modelList);
            HttpContext.Session.SetString("KeyPendingGateInwardList", serializedGrid);
            if (GetDataFrom == "PendingPO")
            {
                return PartialView("_GateInwardDisplayDataDetail", model);
            }
            else
            {
                return PartialView("_GateInwardDisplayVenderPortalDataSummery", model);
            }

        }

        [HttpGet]
        public IActionResult PendingGateEntryGlobalSearch(string searchString, int pageNumber = 1, int pageSize = 10)
        {
            PendingGateInwardDashboard model = new PendingGateInwardDashboard();
            if (string.IsNullOrWhiteSpace(searchString))
            {
                return PartialView("_GateInwardDisplayDataDetail", new List<GateInwardDashboard>());
            }

            string modelJson = HttpContext.Session.GetString("KeyPendingGateInwardList");
            List<PendingGateInwardDashboard> gateInwardDashboard = new List<PendingGateInwardDashboard>();
            if (!string.IsNullOrEmpty(modelJson))
            {
                gateInwardDashboard = JsonConvert.DeserializeObject<List<PendingGateInwardDashboard>>(modelJson);
            }
            if (gateInwardDashboard == null)
            {
                return PartialView("_GateInwardDisplayDataDetail", new List<PendingGateInwardDashboard>());
            }

            List<PendingGateInwardDashboard> filteredResults;

            if (string.IsNullOrWhiteSpace(searchString))
            {
                filteredResults = gateInwardDashboard.ToList();
            }
            else
            {
                filteredResults = gateInwardDashboard
                    .Where(i => i.GetType().GetProperties()
                        .Where(p => p.PropertyType == typeof(string))
                        .Select(p => p.GetValue(i)?.ToString())
                        .Any(value => !string.IsNullOrEmpty(value) &&
                                      value.Contains(searchString, StringComparison.OrdinalIgnoreCase)))
                    .ToList();


                if (filteredResults.Count == 0)
                {
                    filteredResults = gateInwardDashboard.ToList();
                }
            }
            model.TotalRecords = filteredResults.Count;
            model.PageNumber = pageNumber;
            model.PendingGateEntryDashboard = filteredResults.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            model.PageSize = pageSize;


            return PartialView("_GateInwardDisplayDataDetail", model);
        }

        public async Task<IActionResult> GetDetailData(string VendorName, string Gateno, string ItemName, string PartCode, string DocName, string PONO, string ScheduleNo, string FromDate, string ToDate, int pageNumber = 1, int pageSize = 10, string SearchBox = "")
        {
            //model.Mode = "Search";
            var model = new GateInwardDashboard();
            model = await _IGateInward.GetDashboardDetailData(VendorName, Gateno, ItemName, PartCode, DocName, PONO, ScheduleNo, FromDate, ToDate);
            // model = (await _IGateInward.GetDashboardDetailData(...))?.GateDashboard ?? new List<GateInwardDashboard>();
            var modelList = model?.GateDashboard ?? new List<GateInwardDashboard>();

            model.DashboardType = "Detail";
            if (string.IsNullOrWhiteSpace(SearchBox))
            {
                model.TotalRecords = modelList.Count();
                model.PageNumber = pageNumber;
                model.PageSize = pageSize;

                model.GateDashboard = modelList
                   .Skip((pageNumber - 1) * pageSize)
                   .Take(pageSize)
                   .ToList();
            }
            else
            {
                List<GateInwardDashboard> filteredResults;
                if (string.IsNullOrWhiteSpace(SearchBox))
                {
                    filteredResults = modelList.ToList();
                }
                else
                {
                    filteredResults = modelList
                        .Where(i => i.GetType().GetProperties()
                            .Where(p => p.PropertyType == typeof(string))
                            .Select(p => p.GetValue(i)?.ToString())
                            .Any(value => !string.IsNullOrEmpty(value) &&
                                          value.Contains(SearchBox, StringComparison.OrdinalIgnoreCase)))
                        .ToList();


                    if (filteredResults.Count == 0)
                    {
                        filteredResults = modelList.ToList();
                    }
                }

                model.TotalRecords = filteredResults.Count;
                model.GateDashboard = filteredResults.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
                model.PageNumber = pageNumber;
                model.PageSize = pageSize;
            }
            MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                SlidingExpiration = TimeSpan.FromMinutes(55),
                Size = 1024,
            };

            string serializedGrid = JsonConvert.SerializeObject(modelList);
            HttpContext.Session.SetString("KeyGateInardList", serializedGrid);
            return PartialView("_GateInwardDashboardGrid", model);
        }
        [HttpGet]
        public IActionResult GlobalSearch(string searchString, int pageNumber = 1, int pageSize = 10)
        {
            GateInwardDashboard model = new GateInwardDashboard();
            if (string.IsNullOrWhiteSpace(searchString))
            {
                return PartialView("_GateInwardDashboardGrid", new List<GateInwardDashboard>());
            }

            string modelJson = HttpContext.Session.GetString("KeyGateInardList");
            List<GateInwardDashboard> gateInwardDashboard = new List<GateInwardDashboard>();
            if (!string.IsNullOrEmpty(modelJson))
            {
                gateInwardDashboard = JsonConvert.DeserializeObject<List<GateInwardDashboard>>(modelJson);
            }
            if (gateInwardDashboard == null)
            {
                return PartialView("_GateInwardDashboardGrid", new List<GateInwardDashboard>());
            }

            List<GateInwardDashboard> filteredResults;

            if (string.IsNullOrWhiteSpace(searchString))
            {
                filteredResults = gateInwardDashboard.ToList();
            }
            else
            {
                filteredResults = gateInwardDashboard
                    .Where(i => i.GetType().GetProperties()
                        .Where(p => p.PropertyType == typeof(string))
                        .Select(p => p.GetValue(i)?.ToString())
                        .Any(value => !string.IsNullOrEmpty(value) &&
                                      value.Contains(searchString, StringComparison.OrdinalIgnoreCase)))
                    .ToList();


                if (filteredResults.Count == 0)
                {
                    filteredResults = gateInwardDashboard.ToList();
                }
            }

            model.TotalRecords = filteredResults.Count;
            model.GateDashboard = filteredResults.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            model.PageNumber = pageNumber;
            model.PageSize = pageSize;

            return PartialView("_GateInwardDashboardGrid", model);
        }

        public async Task<JsonResult> ClearGridAjax(int AccountCode, int docType, int ItemCode, string ChallanNo)
        {
            HttpContext.Session.Remove("KeyGateInwardGrid");
            HttpContext.Session.Remove("KeyGateInwardItemDetail");
            var JSON = await _IGateInward.FillSaleBillChallan(AccountCode, docType, ItemCode, ChallanNo);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }

        public async Task<GateInwardModel> BindModels(GateInwardModel model)
        {
            if (model == null)
            {
                model = new GateInwardModel();

                model.YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
                model.EntryId = _IDataLogic.GetEntryID("GateDetail", model.YearCode, "GateEntryID", "Gateyearcode");
                //model.EntryDate = DateTime.Today;
                model.EntryTime = DateTime.Now.ToString("hh:mm tt");

            }

            model.AccountList = await _IDataLogic.GetDropDownList("CREDITORDEBTORLIST", "F", "SP_GetDropDownList");
            model.DocumentList = await _IDataLogic.GetDropDownList("DocumentList", "SP_GetDropDownList");
            model.RecUnitList = await _IDataLogic.GetDropDownList("RecUnitList", "SP_GetDropDownList");
            model.ProcessList = await _IDataLogic.GetDropDownList("ProcessList", "SP_GetDropDownList");
            //model.PONO = await _IDataLogic.GetDropDownList("PENDINGPOLIST","I", "SP_GateMainDetail");


            return model;
        }
        public async Task<PendingGateEntryDashboard> PendingBindModel(PendingGateEntryDashboard model)
        {
            if (model == null)
            {
                model = new PendingGateEntryDashboard();
            }

            model.AccountList = await _IDataLogic.GetDropDownList("PendingPOAccountList", "SP_GateMainDetail");
            model.DocumentList = await _IDataLogic.GetDropDownList("DocumentList", "SP_GetDropDownList");
            model.InvoiceList = await _IDataLogic.GetDropDownList("FillVPInvoiceNo", "SP_GateMainDetail");
            //model.PONO = await _IDataLogic.GetDropDownList("PENDINGPOLIST","I", "SP_GateMainDetail");


            return model;
        }

        public async Task<JsonResult> GetAllowBackDate()
        {
            var JSON = await _IGateInward.GetAllowBackDate();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> CheckFeatureOption()
        {
            var JSON = await _IGateInward.CheckFeatureOption();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetDefaultBranch()
        {
            var username = HttpContext.Session.GetString("Branch");

            // Render profile page with username
            return Json(username);
        }
        public async Task<JsonResult> CCEnableDisable()
        {
            var JSON = await _IGateInward.CCEnableDisable();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetServerDate()
        {
            try
            {
                //DateTime time = DateTime.Now;
                //string format = "MMM ddd d HH:mm yyyy";
                //string formattedDate = time.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);

                //var dt = time.ToString(format);
                var time = CommonFunc.ParseFormattedDate(DateTime.Now.ToString());
                return Json(DateTime.Now.ToString("yyyy-MM-dd"));
                // return Json(formattedDate);

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

        public async Task<JsonResult> FillSaleBillChallan(int AccountCode, int docType, int ItemCode, string ChallanNo)
        {
            var JSON = await _IGateInward.FillSaleBillChallan(AccountCode, docType, ItemCode, ChallanNo);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillChallanQty(int AccountCode, int ItemCode, string ChallanNo)
        {
            var JSON = await _IGateInward.FillChallanQty(AccountCode, ItemCode, ChallanNo);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillSaleBillQty(int AccountCode, int ItemCode, string SaleBillNo, int SaleBillYearCode)
        {
            var JSON = await _IGateInward.FillSaleBillQty(AccountCode, ItemCode, SaleBillNo, SaleBillYearCode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> AltUnitConversion(int ItemCode, decimal AltQty, decimal UnitQty)
        {
            var JSON = await _IGateInward.AltUnitConversion(ItemCode, AltQty, UnitQty);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetAccountList(string Check)
        {
            var JSON = await _IDataLogic.GetDropDownList("CREDITORDEBTORLIST", Check, "SP_GetDropDownList");
            _logger.LogError(JsonConvert.SerializeObject(JSON));
            return Json(JSON);
        }
        public async Task<JsonResult> GetBranchList(string Check)
        {
            var JSON = await _IDataLogic.GetDropDownList("BRANCHLIST", Check, "SP_GetDropDownList");
            _logger.LogError(JsonConvert.SerializeObject(JSON));
            return Json(JSON);
        }
        public async Task<JsonResult> FillEntryandGate(int YearCode)
        {
            var JSON = await _IGateInward.FillEntryandGate("NewEntryId", YearCode, "SP_GateMainDetail");
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetPOList(string Code, string Type, int Year, int DocTypeId)
        {
            var JSON = await _IGateInward.GetPoNumberDropDownList("PENDINGPOLIST", Type, "SP_GateMainDetail", Code, Year, DocTypeId);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetPOYearList(string accountCode, string yearCode, string poNo, int docTypeId, string invoiceDate, string ItemService, string EntryDate)
        {
            var JSON = await _IGateInward.GetScheDuleByYearCodeandAccountCode("PENDINGPOLIST", accountCode, yearCode, poNo, docTypeId, invoiceDate, ItemService, EntryDate);
            _logger.LogError(JsonConvert.SerializeObject(JSON));
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetItems(int DocType, string Check, int AccountCode, string SearchText)
        {
            var JSON = await _IGateInward.GetItems("GETITEMS", DocType, Check, AccountCode, SearchText);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetPopUpData(int AccountCode, string PONO)
        {
            var JSON = await _IGateInward.GetPopUpData("POPUPDATA", AccountCode, PONO);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> CheckDuplicateEntry(int YearCode, int AccountCode, string InvNo, int DocType)
        {
            var JSON = await _IGateInward.CheckDuplicateEntry(YearCode, AccountCode, InvNo, DocType);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillPendQty(int ItemCode, int PartyCode, string PONO, int POYear, int Year, string SchNo, int SchYearCode, int ProcessId, int EntryId, int YearCode)
        {
            var JSON = await _IGateInward.FillPendQty(ItemCode, PartyCode, PONO, POYear, Year, SchNo, SchYearCode, ProcessId, EntryId, YearCode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public IActionResult AddGateInwardDetail(GateInwardItemDetail model)
        {
            try
            {
                if (model.Mode == "U" || model.Mode == "V")
                {
                    string modelJson = HttpContext.Session.GetString("KeyGateInwardItemDetail");
                    List<GateInwardItemDetail> GateInwardItemDetail = new List<GateInwardItemDetail>();
                    if (!string.IsNullOrEmpty(modelJson))
                    {
                        GateInwardItemDetail = JsonConvert.DeserializeObject<List<GateInwardItemDetail>>(modelJson);
                    }

                    var MainModel = new GateInwardModel();
                    var GateInwardGrid = new List<GateInwardItemDetail>();
                    var GateGrid = new List<GateInwardItemDetail>();
                    var SSGrid = new List<GateInwardItemDetail>();

                    if (model != null)
                    {
                        if (GateInwardItemDetail == null)
                        {
                            model.SeqNo = 1;
                            GateGrid.Add(model);
                        }
                        else
                        {
                            if (model.docTypeId == 3 ? GateInwardItemDetail.Any(x =>
                               string.Equals(x.PartCode?.Trim(), model.PartCode?.Trim(), StringComparison.OrdinalIgnoreCase) &&
                               string.Equals(x.PoNo?.Trim(), model.PoNo?.Trim(), StringComparison.OrdinalIgnoreCase) &&
                               x.PoYear == model.PoYear &&
                                  string.Equals((x.SchNo ?? "").Trim(), (model.SchNo ?? "").Trim(), StringComparison.OrdinalIgnoreCase) &&
                               //    string.Equals(x.SchNo?.Trim(), model.SchNo?.Trim(), StringComparison.OrdinalIgnoreCase) &&
                               x.SchYearCode == model.SchYearCode &&
                               string.Equals(x.AgainstChallanNo ?? "", model.AgainstChallanNo ?? "", StringComparison.OrdinalIgnoreCase) &&
                               string.Equals(x.SaleBillNo ?? "", model.SaleBillNo ?? "", StringComparison.OrdinalIgnoreCase) &&
                               x.SaleBillYearCode == model.SaleBillYearCode)
                         : GateInwardItemDetail.Any(x =>
                               string.Equals(x.PartCode?.Trim(), model.PartCode?.Trim(), StringComparison.OrdinalIgnoreCase) &&
                               string.Equals(x.PoNo?.Trim(), model.PoNo?.Trim(), StringComparison.OrdinalIgnoreCase) &&
                               x.PoYear == model.PoYear &&
                               string.Equals((x.SchNo ?? "").Trim(), (model.SchNo ?? "").Trim(), StringComparison.OrdinalIgnoreCase)
                               &&
                               //   string.Equals(x.SchNo?.Trim(), model.SchNo?.Trim(), StringComparison.OrdinalIgnoreCase) &&
                               x.SchYearCode == model.SchYearCode &&
                               string.Equals(x.AgainstChallanNo ?? "", model.AgainstChallanNo ?? "", StringComparison.OrdinalIgnoreCase)))
                            {
                                return StatusCode(207, "Duplicate");
                            }

                            //if (model.docTypeId == 3 ? GateInwardItemDetail.Any(x => x.PartCode == model.PartCode && x.AgainstChallanNo == model.AgainstChallanNo && x.SaleBillNo == model.SaleBillNo && x.SaleBillYearCode == model.SaleBillYearCode) : GateInwardItemDetail.Any(x => x.PartCode == model.PartCode && x.AgainstChallanNo == model.AgainstChallanNo && x.PoNo == model.PoNo))
                            //{
                            //    return StatusCode(207, "Duplicate");
                            //}
                            else
                            {
                                //model.SeqNo = GateInwardItemDetail.Count + 1;
                                //GateGrid = GateInwardItemDetail.Where(x => x != null).ToList();
                                //SSGrid.AddRange(GateGrid);
                                //GateGrid.Add(model);
                                GateGrid = GateInwardItemDetail.Where(x => x != null).ToList();

                                if (model.SeqNo > 0)
                                {
                                    // 🔥 INSERT at SAME position
                                    GateGrid.Insert(model.SeqNo.Value - 1, model);

                                    // 🔥 Resequence ONCE
                                    int i = 1;
                                    foreach (var item in GateGrid)
                                        item.SeqNo = i++;
                                }
                                else
                                {
                                    model.SeqNo = GateGrid.Count + 1;
                                    GateGrid.Add(model);
                                }

                            }
                        }

                        MainModel.ItemDetailGrid = GateGrid;

                        string serializedGrid = JsonConvert.SerializeObject(MainModel.ItemDetailGrid);
                        HttpContext.Session.SetString("KeyGateInwardItemDetail", serializedGrid);
                    }
                    else
                    {
                        ModelState.TryAddModelError("Error", "Schedule List Cannot Be Empty...!");
                    }

                    return PartialView("_GateInwardGrid", MainModel);
                }
                else
                {
                    string modelJson = HttpContext.Session.GetString("KeyGateInwardGrid");
                    List<GateInwardItemDetail> GateInwardItemDetail = new List<GateInwardItemDetail>();
                    if (!string.IsNullOrEmpty(modelJson))
                    {
                        GateInwardItemDetail = JsonConvert.DeserializeObject<List<GateInwardItemDetail>>(modelJson);
                    }

                    var MainModel = new GateInwardModel();
                    var GateInwardGrid = new List<GateInwardItemDetail>();
                    var GateGrid = new List<GateInwardItemDetail>();
                    var SSGrid = new List<GateInwardItemDetail>();

                    if (model != null)
                    {
                        if (GateInwardItemDetail == null)
                        {
                            model.SeqNo = 1;
                            GateGrid.Add(model);
                        }
                        else
                        {
                            if (model.docTypeId == 3 ? GateInwardItemDetail.Any(x => x.PartCode == model.PartCode && x.PoNo == model.PoNo && x.PoYear == model.PoYear && x.SchNo == model.SchNo && x.SchYearCode == model.SchYearCode && x.AgainstChallanNo == model.AgainstChallanNo && x.SaleBillNo == model.SaleBillNo && x.SaleBillYearCode == model.SaleBillYearCode) : GateInwardItemDetail.Any(x => x.PartCode == model.PartCode && x.PoNo == model.PoNo && x.PoYear == model.PoYear && x.SchNo == model.SchNo && x.SchYearCode == model.SchYearCode && x.AgainstChallanNo == model.AgainstChallanNo))
                            {
                                return StatusCode(207, "Duplicate");
                            }
                            else
                            {
                                //model.SeqNo = GateInwardItemDetail.Count + 1;
                                //GateGrid = GateInwardItemDetail.Where(x => x != null).ToList();
                                //SSGrid.AddRange(GateGrid);
                                //GateGrid.Add(model);
                                GateGrid = GateInwardItemDetail.Where(x => x != null).ToList();

                                if (model.SeqNo > 0)
                                {
                                    // 🔥 INSERT at SAME position
                                    GateGrid.Insert(model.SeqNo.Value - 1, model);

                                    // 🔥 Resequence ONCE
                                    int i = 1;
                                    foreach (var item in GateGrid)
                                        item.SeqNo = i++;
                                }
                                else
                                {
                                    model.SeqNo = GateGrid.Count + 1;
                                    GateGrid.Add(model);
                                }

                            }


                        }

                        MainModel.ItemDetailGrid = GateGrid;

                        string serializedGrid = JsonConvert.SerializeObject(MainModel.ItemDetailGrid);
                        HttpContext.Session.SetString("KeyGateInwardGrid", serializedGrid);
                    }
                    else
                    {
                        ModelState.TryAddModelError("Error", "Schedule List Cannot Be Empty...!");
                    }

                    return PartialView("_GateInwardGrid", MainModel);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<JsonResult> GetScheDuleByYearCodeandAccountCode(string accountCode, string Year, string poNo, int docTypeId, string InvoiceDate, string ItemService, string EntryDate)
        {
            var JSON = await _IGateInward.GetScheDuleByYearCodeandAccountCode("PURCHSCHEDULE", accountCode, Year, poNo, docTypeId, InvoiceDate, ItemService, EntryDate);
            _logger.LogError(JsonConvert.SerializeObject(JSON));
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> CheckEditOrDelete(string GateNo, int YearCode)
        {
            var JSON = await _IGateInward.CheckEditOrDelete(GateNo, YearCode);
            _logger.LogError(JsonConvert.SerializeObject(JSON));
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillItems(string accountCode, string Year, string poNo, string Type, string scheduleNO = "", string scheduleYear = "", string Check = "")
        {
            var JSON = await _IGateInward.FillItems("PENDPOITEM", accountCode, Year, poNo, Type, scheduleNO, scheduleYear, Check);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public IActionResult DeleteItemRow(int SeqNo, string Mode, bool IsEdit = false)
        {
            var MainModel = new GateInwardModel();
            if (Mode == "U" || Mode == "V")
            {
                string modelJson = HttpContext.Session.GetString("KeyGateInwardItemDetail");
                List<GateInwardItemDetail> GateInwardItemDetail = new List<GateInwardItemDetail>();
                if (!string.IsNullOrEmpty(modelJson))
                {
                    GateInwardItemDetail = JsonConvert.DeserializeObject<List<GateInwardItemDetail>>(modelJson);
                }

                // int Indx = Convert.ToInt32(SeqNo) - 1;

                if (GateInwardItemDetail != null && GateInwardItemDetail.Count > 0)
                {
                    var removeItem = GateInwardItemDetail.FirstOrDefault(x => x.SeqNo == SeqNo);
                    if (removeItem != null)
                        GateInwardItemDetail.Remove(removeItem);

                    // 🔥 ONLY re-sequence if NOT editing
                    if (!IsEdit)
                    {
                        int indx = 1;
                        foreach (var item in GateInwardItemDetail)
                        {
                            item.SeqNo = indx++;
                        }
                    }

                    //GateInwardItemDetail.RemoveAt(Convert.ToInt32(Indx));

                    //Indx = 0;

                    //foreach (var item in GateInwardItemDetail)
                    //{
                    //    Indx++;
                    //  item.SeqNo = Indx;
                    //}
                    MainModel.ItemDetailGrid = GateInwardItemDetail;

                    MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
                    {
                        AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                        SlidingExpiration = TimeSpan.FromMinutes(55),
                        Size = 1024,
                    };

                    string serializedGrid = JsonConvert.SerializeObject(MainModel.ItemDetailGrid);
                    HttpContext.Session.SetString("KeyGateInwardItemDetail", serializedGrid);
                }
            }
            else
            {
                string modelJson = HttpContext.Session.GetString("KeyGateInwardGrid");
                List<GateInwardItemDetail> GateInwardItemDetail = new List<GateInwardItemDetail>();
                if (!string.IsNullOrEmpty(modelJson))
                {
                    GateInwardItemDetail = JsonConvert.DeserializeObject<List<GateInwardItemDetail>>(modelJson);
                }
                if (GateInwardItemDetail.Any())
                {
                    // ✅ SAME SAFE REMOVE LOGIC
                    var removeItem = GateInwardItemDetail.FirstOrDefault(x => x.SeqNo == SeqNo);
                    if (removeItem != null)
                        GateInwardItemDetail.Remove(removeItem);

                    // ✅ Normal mode → ALWAYS re-sequence
                    int i = 1;
                    foreach (var item in GateInwardItemDetail)
                        item.SeqNo = i++;

                    //int Indx = Convert.ToInt32(SeqNo) - 1;

                    //if (GateInwardItemDetail != null && GateInwardItemDetail.Count > 0)
                    //{
                    //    GateInwardItemDetail.RemoveAt(Convert.ToInt32(Indx));

                    //    Indx = 0;

                    //    foreach (var item in GateInwardItemDetail)
                    //    {
                    //        Indx++;
                    //        item.SeqNo = Indx;
                    //    }
                    MainModel.ItemDetailGrid = GateInwardItemDetail;

                    string serializedGrid = JsonConvert.SerializeObject(MainModel.ItemDetailGrid);
                    HttpContext.Session.SetString("KeyGateInwardGrid", serializedGrid);
                }
            }

            return PartialView("_GateInwardGrid", MainModel);
        }
        public IActionResult EditItemRow(int SeqNo, string Mode)
        {
            IList<GateInwardItemDetail> GateInwardItemDetail = new List<GateInwardItemDetail>();
            if (Mode == "U" || Mode == "V")
            {
                string modelJson = HttpContext.Session.GetString("KeyGateInwardItemDetail");
                if (!string.IsNullOrEmpty(modelJson))
                {
                    GateInwardItemDetail = JsonConvert.DeserializeObject<List<GateInwardItemDetail>>(modelJson);
                }
            }
            else
            {
                string modelJson = HttpContext.Session.GetString("KeyGateInwardGrid");
                if (!string.IsNullOrEmpty(modelJson))
                {
                    GateInwardItemDetail = JsonConvert.DeserializeObject<List<GateInwardItemDetail>>(modelJson);
                }
            }
            IEnumerable<GateInwardItemDetail> SSGrid = GateInwardItemDetail;
            if (GateInwardItemDetail != null)
            {
                SSGrid = GateInwardItemDetail.Where(x => x.SeqNo == SeqNo);
            }
            string JsonString = JsonConvert.SerializeObject(SSGrid);
            return Json(JsonString);
        }

        //added rights code by sajni
        public async Task<IActionResult> Dashboard(string FromDate = "", string ToDate = "", string Flag = "True", string VendorName = "", string GateNo = "", string PartCode = "", string DocName = "", string ItemName = "", string PONO = "", string ScheduleNo = "", string Searchbox = "", string DashboardType = "")
        {
            try
            {
                int userID = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
                var rights = await _IGateInward.GetFormRights(userID);
                if (rights?.Result == null || rights.Result.Tables.Count == 0 || rights.Result.Tables[0].Rows.Count == 0)
                {
                    return RedirectToAction("Dashboard", "Home");
                }
                var table = rights.Result.Tables[0];
                bool optAll = Convert.ToBoolean(table.Rows[0]["OptAll"]);
                bool optView = Convert.ToBoolean(table.Rows[0]["OptView"]);
                bool optUpdate = Convert.ToBoolean(table.Rows[0]["OptUpdate"]);
                bool optDelete = Convert.ToBoolean(table.Rows[0]["OptDelete"]);

                if (!(optView || optUpdate || optDelete))
                {
                    return RedirectToAction("Dashboard", "Home");
                }
                HttpContext.Session.Remove("KeyGateInwardGrid");

                var model = new GateDashboard();
                var Result = await _IGateInward.GetDashboardData(userID).ConfigureAwait(true);


                if (Result != null)
                {
                    var _List = new List<TextValue>();
                    DataSet DS = Result.Result;
                    if (DS != null)
                    {
                        var DT = DS.Tables[0].DefaultView.ToTable(true, "GateNo", "GDATE", "VendorName", "address",
                          "Invoiceno", "InvoiceDate", "DocName", "CompGateNo", "POTypeServItem", "entryId", "yearcode",
                           "MRNNO", "MRNYEARCODE", "MRNDate", "EnteredBy", "UpdatedBy");
                        model.GateDashboard = CommonFunc.DataTableToList<GateInwardDashboard>(DT, "GateInward");
                        foreach (var row in DS.Tables[0].AsEnumerable())
                        {
                            _List.Add(new TextValue()
                            {
                                Text = row["Gateno"].ToString(),
                                Value = row["Gateno"].ToString()
                            });
                        }
                        //var dd = _List.Select(x => x.Value).Distinct();
                        model.SessionYearCode = HttpContext.Session.GetString("FromDate");
                        var _PONOList = _List.DistinctBy(x => x.Value).ToList();
                        model.GateNOList = _PONOList;
                        _List = new List<TextValue>();
                    }
                    if (Flag != "True")
                    {
                        model.FromDate1 = FromDate;
                        model.ToDate1 = ToDate;
                        //model.Dashboardtype = DashboardType;
                        model.VendorName = VendorName;
                        model.PartCode = PartCode;
                        model.ItemName = ItemName;
                        model.DocName = DocName;
                        model.Gateno = GateNo;
                        model.PONO = PONO;
                        model.ScheduleNo = ScheduleNo;
                        model.Searchbox = Searchbox;
                        model.DashboardType = DashboardType;
                        return View(model);


                    }
                }
                return View(model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        [HttpPost]
        public async Task<IActionResult> GetPendingGateEntryVPDetailData(int AccountCode, string InvoiceNo)
        {
            try
            {
                // 1️⃣ Get data from DB
                var dataResult = await _IGateInward.GetPendingGateEntryVPDetailData(AccountCode, InvoiceNo);

                // Assuming the service returns a model containing a list named PendingGateEntryDashboard
                var pendingDetails = dataResult?.PendingGateEntryDashboard ?? new List<PendingGateInwardDashboard>();

                // 2️⃣ Remove any old session data
                HttpContext.Session.Remove("KeyGateInwardGrid");

                // 3️⃣ Serialize and store new data
                var serializedData = JsonConvert.SerializeObject(pendingDetails);
                HttpContext.Session.SetString("KeyGateInwardGrid", serializedData);

                // 4️⃣ Optionally return data for confirmation/debug
                return Json(new
                {
                    message = "done",
                    count = pendingDetails.Count
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    message = "error",
                    details = ex.Message
                });
            }
        }







        [HttpPost]
        public IActionResult StoreCheckedRowsToSession(List<PendingGateInwardDashboard> model)
        {
            try
            {
                //HttpContext.Session.Remove("KeyGateInwardItemDetail");
                //string serializedGrid = JsonConvert.SerializeObject(ItemData);
                //HttpContext.Session.SetString("KeyGateInwardItemDetail", serializedGrid);
                //return PartialView("PendingGateInward", ItemData);

                HttpContext.Session.Remove("KeyGateInwardGrid");
                var sessionData = HttpContext.Session.GetString("KeyGateInwardGrid");
                var PendingDetails = string.IsNullOrEmpty(sessionData)
    ? new List<PendingGateInwardDashboard>()
    : JsonConvert.DeserializeObject<List<PendingGateInwardDashboard>>(sessionData);

                //TempData.Clear();

                var MainModel = new GateInwardModel();
                var IssueWithoutBomGrid = new List<PendingGateInwardDashboard>();
                var IssueGrid = new List<PendingGateInwardDashboard>();
                //var IssueGrid = new List<PendingGateInwardDashboard>();
                var SSGrid = new List<PendingGateInwardDashboard>();

                var seqNo = 0;
                if (model != null)
                {
                    foreach (var item in model)
                    {
                        if (item != null)
                        {
                            if (PendingDetails == null)
                            {
                                item.seqno += seqNo + 1;
                                IssueGrid.Add(item);
                                seqNo++;
                            }
                            else
                            {

                                item.seqno = PendingDetails.Count + 1;
                                //   IssueGrid = IssueAgainstProdScheduleDetail.Where(x => x != null).ToList();
                                SSGrid.AddRange(IssueGrid);
                                IssueGrid.Add(item);

                            }


                            HttpContext.Session.SetString("KeyGateInwardGrid", JsonConvert.SerializeObject(IssueGrid));

                        }

                    }
                    //MainModel.ItemDetailGrid = IssueGrid;
                    //var jsonData = JsonConvert.SerializeObject(MainModel.ItemDetailGrid);
                    //HttpContext.Session.SetString("KeyPendingProductionSchedule", jsonData);
                }
                var sessionGridData = HttpContext.Session.GetString("KeyGateInwardGrid");
                var grid = string.IsNullOrEmpty(sessionGridData)
    ? new List<PendingGateInwardDashboard>()
    : JsonConvert.DeserializeObject<List<PendingGateInwardDashboard>>(sessionGridData);
                var issueDataJson = JsonConvert.SerializeObject(IssueGrid);
                HttpContext.Session.SetString("KeyGateInwardGrid", issueDataJson);


                return Json("done");
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        [Route("{controller}/PendingGateInward")]
        public async Task<IActionResult> PendingGateInward()
        {
            var model = new PendingGateEntryDashboard();
            model.PendingGateEntryDashboard = new List<PendingGateInwardDashboard>();
            model = await PendingBindModel(model);
            model.FromDate = HttpContext.Session.GetString("FromDate");
            return View(model);
        }

        public async Task<IActionResult> DeleteByID(int ID, int YC, string GateNo, string FromDate = "", string ToDate = "", string VendorName = "", string PartCode = "", string ItemName = "", string DocName = "", string PONO = "", string ScheduleNo = "", string Searchbox = "", string DashboardType = "")
        {
            int ActualEnteredBy = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
            int EmpId = Convert.ToInt32(HttpContext.Session.GetString("UID"));
            var EntryByMachineName = @Environment.MachineName;
            string IPAddress = HttpContext.Session.GetString("ClientIP");
            var Result = await _IGateInward.DeleteByID(ID, YC, EmpId, EntryByMachineName, GateNo, IPAddress);

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

            DateTime fromDt = DateTime.ParseExact(FromDate, "dd/MM/yyyy", null);
            string formattedFromDate = fromDt.ToString("dd/MMM/yyyy 00:00:00");
            DateTime toDt = DateTime.ParseExact(ToDate, "dd/MM/yyyy", null);
            string formattedToDate = toDt.ToString("dd/MMM/yyyy 00:00:00");

            return RedirectToAction("Dashboard", new { FromDate = formattedFromDate, ToDate = formattedToDate, Flag = "False", VendorName = VendorName, GateNo = GateNo, PartCode = PartCode, ItemName = ItemName, DocName = DocName, PONO = PONO, ScheduleNo = ScheduleNo, Searchbox = Searchbox, DashboardType = DashboardType });
        }
        private static DataTable GetDetailTable(IList<GateInwardItemDetail> DetailList)
        {
            try
            {
                var GIGrid = new DataTable();

                GIGrid.Columns.Add("SeqNo", typeof(int));
                GIGrid.Columns.Add("PONo", typeof(string));
                GIGrid.Columns.Add("POYearCode", typeof(int));
                GIGrid.Columns.Add("PODate", typeof(string));
                GIGrid.Columns.Add("POEntryId", typeof(int));
                GIGrid.Columns.Add("SchNo", typeof(string));
                GIGrid.Columns.Add("SchYearCode", typeof(int));
                GIGrid.Columns.Add("SchDate", typeof(string));
                GIGrid.Columns.Add("SchEntryId", typeof(int));
                GIGrid.Columns.Add("ItemCode", typeof(int));
                GIGrid.Columns.Add("Unit", typeof(string));
                GIGrid.Columns.Add("UnitRate", typeof(string));
                GIGrid.Columns.Add("Qty", typeof(decimal));
                GIGrid.Columns.Add("Rate", typeof(decimal));
                GIGrid.Columns.Add("altqty", typeof(decimal));
                GIGrid.Columns.Add("altunit", typeof(string));
                GIGrid.Columns.Add("SaleBillNo", typeof(string));
                GIGrid.Columns.Add("SaleBillYearCode", typeof(int));
                GIGrid.Columns.Add("SaleBillQty", typeof(decimal));
                GIGrid.Columns.Add("Remarks", typeof(string));
                GIGrid.Columns.Add("AgainstChallanNo", typeof(string));
                GIGrid.Columns.Add("AgainstChallanYearcode", typeof(int));
                GIGrid.Columns.Add("ChallanQty", typeof(decimal));
                GIGrid.Columns.Add("processid", typeof(int));
                GIGrid.Columns.Add("Size", typeof(string));
                GIGrid.Columns.Add("Color", typeof(string));
                GIGrid.Columns.Add("SupplierBatchNo", typeof(string));
                GIGrid.Columns.Add("ShelfLife", typeof(decimal));
                GIGrid.Columns.Add("potype", typeof(string));
                GIGrid.Columns.Add("PendPOQty", typeof(decimal));
                GIGrid.Columns.Add("AltPendQty", typeof(decimal));
                GIGrid.Columns.Add("NoOfBoxes", typeof(int));
                foreach (var Item in DetailList)
                {
                    if (Item.PoNo == null || Item.PoNo == "null" || Item.PoNo == "")
                    {
                        Item.PoNo = "";
                    }
                    if (Item.PoYear == null || Item.PoYear == 0)
                    {
                        Item.PoYear = 0;
                    }
                    if (Item.PoEntryId == null || Item.PoEntryId == 0)
                    {
                        Item.PoEntryId = 0;
                    }
                    if (Item.SchYearCode == null || Item.SchYearCode == 0)
                    {
                        Item.SchYearCode = 0;
                    }
                    if (Item.SchEntryId == null || Item.SchEntryId == 0)
                    {
                        Item.SchEntryId = 0;
                    }
                    if (Item.Qty == null || Item.Qty == 0)
                    {
                        Item.Qty = 0;
                    }
                    if (Item.SchNo == null || Item.SchNo == "null" || Item.SchNo == "")
                    {
                        Item.SchNo = "";
                    }
                    if (Item.Unit == null || Item.Unit == "null" || Item.Unit == "")
                    {
                        Item.Unit = "";
                    }
                    GIGrid.Rows.Add(
                        new object[]
                        {
                    Item.SeqNo,
                    Item.PoNo??"",
                    Item.PoYear,
                    Item.PoDate == null ? string.Empty : ParseFormattedDate(Item.PoDate.Split(" ")[0]),
                    Item.PoEntryId,
                    Item.SchNo??"",
                    Item.SchYearCode,
                    Item.SchDate == null ? string.Empty : ParseFormattedDate(Item.SchDate.Split(" ")[0]),
                    Item.SchEntryId,
                    Item.ItemCode,
                    Item.Unit??"",
                    Item.UnitRate??"",
                    Item.Qty,
                    Item.Rate,
                    Item.AltQty,
                    Item.AltUnit??"",
                    Item.SaleBillNo??"",
                    Item.SaleBillYearCode==null?0:Item.SaleBillYearCode,
                    Item.SaleBillQty ==null?0.0:Item.SaleBillQty,
                    Item.Remarks??"",
                    Item.AgainstChallanNo??"",
                    Item.AgainstChallanYearcode==null?0:Item.AgainstChallanYearcode,
                    Item.ChallanQty==null?0.0:Item.ChallanQty,
                    Item.ProcessId==null?0:Item.ProcessId,
                    Item.ItemSize??"",
                    Item.ItemColor??"",
                    Item.SupplierBatchNo??"",
                    Item.ShelfLife == null ? 0.0 : Item.ShelfLife,
                    Item.POType ?? "",
                    Item.PendQty == null ? 0.0 : Item.PendQty,
                    Item.AltPendQty == null ? 0.0 : Item.AltPendQty,
                    Item.NoOfBoxes== null ? 0.0 : Item.NoOfBoxes,
                        });
                }
                GIGrid.Dispose();
                return GIGrid;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<JsonResult> FillSaleBillRate(int AccountCode, int ItemCode, string SaleBillNo, int SaleBillYearCode)
        {
            var JSON = await _IGateInward.FillSaleBillRate(AccountCode, ItemCode, SaleBillNo, SaleBillYearCode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }


        [HttpGet]
        public IActionResult ExportGateInwardDashboardToExcel()
        {
            string modelJson = HttpContext.Session.GetString("KeyGateInardList");

            List<GateInwardDashboard> gateList = new();

            if (!string.IsNullOrEmpty(modelJson))
                gateList = JsonConvert.DeserializeObject<List<GateInwardDashboard>>(modelJson);

            if (gateList == null || gateList.Count == 0)
                return NotFound("No data available to export.");

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Gate Inward Dashboard");

            // ✅ Headers (same order as your HTML table – Action column skipped)
            string[] headers =
            {
        "Sr#","Gate No","Gate Date","Vendor Name","Delivery Address",
        "Invoice No","Invoice Date","Doc Name","Item Name","Part Code",
        "Qty","Alt Qty","Unit","Alt Unit","PO No","PO Year Code",
        "Sch No","Sch Year Code","Size","Color","Remarks","Process",
        "Sale Bill No","Sale Bill Year Code","Sale Bill Qty",
        "Against Challan No","Challan Year Code","Challan Qty",
        "Supplier Batch No","Shelf Life","Comp Gate No","Type",
        "Tare Weight","Gross Weight","Net Weight","Show PO Till Date",
        "CC","PO Type","Rate","Pending PO Qty","Alt Pending Qty",
        "Entry By Machine","Entry ID","Year Code",
        "MRN No","MRN Year Code","MRN Date",
        "Entered By","Updated By","No Of Boxes"
    };

            // 🔹 Write headers
            for (int i = 0; i < headers.Length; i++)
                worksheet.Cell(1, i + 1).Value = headers[i];

            int row = 2;
            int sr = 1;

            foreach (var item in gateList)
            {
                int col = 1;

                worksheet.Cell(row, col++).Value = sr++;
                worksheet.Cell(row, col++).Value = item.Gateno;
                worksheet.Cell(row, col++).Value = SafeDate(item.GDate);
                worksheet.Cell(row, col++).Value = item.VendorName;
                worksheet.Cell(row, col++).Value = item.address;
                worksheet.Cell(row, col++).Value = item.Invoiceno;
                worksheet.Cell(row, col++).Value = SafeDate(item.InvoiceDate);
                worksheet.Cell(row, col++).Value = item.DocName;
                worksheet.Cell(row, col++).Value = item.ItemName;
                worksheet.Cell(row, col++).Value = item.PartCode;
                worksheet.Cell(row, col++).Value = item.Qty;
                worksheet.Cell(row, col++).Value = item.AltQty;
                worksheet.Cell(row, col++).Value = item.Unit;
                worksheet.Cell(row, col++).Value = item.AltUnit;
                worksheet.Cell(row, col++).Value = item.PoNo;
                worksheet.Cell(row, col++).Value = item.PoYearCode;
                worksheet.Cell(row, col++).Value = item.SchNo;
                worksheet.Cell(row, col++).Value = item.SchYearCode;
                worksheet.Cell(row, col++).Value = item.Size;
                worksheet.Cell(row, col++).Value = item.Color;
                worksheet.Cell(row, col++).Value = item.Remarks;
                worksheet.Cell(row, col++).Value = item.ProcessName;
                worksheet.Cell(row, col++).Value = item.SaleBillNo;
                worksheet.Cell(row, col++).Value = item.SaleBillYearCode;
                worksheet.Cell(row, col++).Value = item.SaleBillQty;
                worksheet.Cell(row, col++).Value = item.AgainstChallanNo;
                worksheet.Cell(row, col++).Value = item.AgainstChallanYearCode;
                worksheet.Cell(row, col++).Value = item.ChallanQty;
                worksheet.Cell(row, col++).Value = item.SupplierBatchNo;
                worksheet.Cell(row, col++).Value = item.ShelfLife;
                worksheet.Cell(row, col++).Value = item.CompGateNo;
                worksheet.Cell(row, col++).Value = item.Types;
                worksheet.Cell(row, col++).Value = item.TareWeight;
                worksheet.Cell(row, col++).Value = item.GrossWeight;
                worksheet.Cell(row, col++).Value = item.NetWeight;
                worksheet.Cell(row, col++).Value = item.ShowPoTillDate;
                worksheet.Cell(row, col++).Value = item.CC;
                worksheet.Cell(row, col++).Value = item.POtype;
                worksheet.Cell(row, col++).Value = item.Rate;
                worksheet.Cell(row, col++).Value = item.PendPOQty;
                worksheet.Cell(row, col++).Value = item.AltPendQty;
                worksheet.Cell(row, col++).Value = item.EntryByMachineName;
                worksheet.Cell(row, col++).Value = item.entryId;
                worksheet.Cell(row, col++).Value = item.yearcode;
                worksheet.Cell(row, col++).Value = item.MrnNo;
                worksheet.Cell(row, col++).Value = item.MRNYEARCODE;
                worksheet.Cell(row, col++).Value = SafeDate(item.MRNDate);
                worksheet.Cell(row, col++).Value = item.EnteredBy;
                worksheet.Cell(row, col++).Value = item.UpdatedBy;
                worksheet.Cell(row, col++).Value = item.NoOfBoxes;

                row++;
            }

            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Position = 0;

            return File(
                stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "GateInwardDashboard.xlsx"
            );
        }
        private string SafeDate(string input)
        {
            if (string.IsNullOrEmpty(input))
                return "";

            return input.Contains(" ") ? input.Split(" ")[0] : input;
        }
    }
}
