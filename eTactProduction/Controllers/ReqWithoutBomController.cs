using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using FastReport.Export.Html;
using FastReport.Export.Image;
using FastReport;
using FastReport.Web;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System.Composition;
using static eTactWeb.Data.Common.CommonFunc;
using static eTactWeb.DOM.Models.Common;
using Microsoft.Extensions.Configuration;
using static Grpc.Core.Metadata;
using System.Net;
using System.Globalization;
using System.Data;
using System.Configuration;
using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.Office2010.Excel;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;
using System.Text.RegularExpressions;

namespace eTactWeb.Controllers
{
    public class ReqWithoutBomController : Controller
    {
        private readonly IDataLogic _IDataLogic;
        private readonly IReqWithoutBOM _IReqWithoutBOM;
        private readonly ILogger<ReqWithoutBomController> _logger;
        private readonly IWebHostEnvironment _IWebHostEnvironment;
        private readonly IConfiguration _iconfiguration;
        public EncryptDecrypt EncryptDecrypt { get; }
        private readonly ConnectionStringService _connectionStringService;
        public ReqWithoutBomController(ILogger<ReqWithoutBomController> logger, IDataLogic iDataLogic, IReqWithoutBOM iReqWithoutBOM, EncryptDecrypt encryptDecrypt, IWebHostEnvironment iWebHostEnvironment, IConfiguration configuration, ConnectionStringService connectionStringService)
        {
            _logger = logger;
            _IDataLogic = iDataLogic;
            _IReqWithoutBOM = iReqWithoutBOM;
            _IWebHostEnvironment = iWebHostEnvironment;
            _iconfiguration = configuration;
            EncryptDecrypt = encryptDecrypt;
            _connectionStringService = connectionStringService;
        }
        bool IsValidBase64(string value)
        {
            if (string.IsNullOrEmpty(value))
                return false;

            return Regex.IsMatch(value,
                @"^[a-zA-Z0-9\+/]*={0,2}$",
                RegexOptions.None);
        }

        public async Task<IActionResult> PrintReport(int EntryId, int YearCode = 0, string Type = "")
        {
            try
            {
                //       int userID = Convert.ToInt32(HttpContext.Session.GetString($"EmpID_{formKey}"));
                //       var rights = await _IReqWithoutBOM.GetFormRights(userID);
                //       if (rights?.Result == null || rights.Result.Tables.Count == 0 || rights.Result.Tables[0].Rows.Count == 0)
                //       {
                //           return RedirectToAction("Dashboard", "Home",new {formKey=formKey});
                //       }

                //       var table = rights.Result.Tables[0];
                //       string encID = Request.Query["EntryId"].ToString();
                //       string encYC = Request.Query["YearCode"].ToString();
                //       if (!string.IsNullOrEmpty(encID) && encID != "0" &&
                //!string.IsNullOrEmpty(encYC) && encYC != "0" &&
                //IsValidBase64(encID) && IsValidBase64(encYC))
                //       {
                //           int decryptedID = EncryptDecrypt.DecodeID(encID);
                //           int decryptedYC = EncryptDecrypt.DecodeID(encYC);

                //           EntryId = decryptedID;
                //           YearCode = decryptedYC;

                //       }

                //       bool optAll = Convert.ToBoolean(table.Rows[0]["OptAll"]);
                //       bool optSave = Convert.ToBoolean(table.Rows[0]["OptSave"]);
                //       bool optUpdate = Convert.ToBoolean(table.Rows[0]["OptUpdate"]);

                //       if (!(optAll || optUpdate || optSave))
                //       {
                //           return RedirectToAction("Dashboard", "Home",new {formKey=formKey});
                //       }

                string contentRootPath = _IWebHostEnvironment.ContentRootPath;
                string webRootPath = _IWebHostEnvironment.WebRootPath;
                var webReport = new WebReport();
                // string reportPath = Path.Combine(webRootPath, "ReqWithoutBom.frx");
                var ReportName = _IReqWithoutBOM.GetReportName();
                //string reportPath = Path.Combine(webRootPath, "ReqWithoutBOMF.frx");
                //webReport.Report.Load(reportPath);
                if (!string.Equals(ReportName.Result.Result.Rows[0].ItemArray[0], System.DBNull.Value))
                {
                    webReport.Report.Load(webRootPath + "\\" + ReportName.Result.Result.Rows[0].ItemArray[0] + ".frx"); // from database
                }
                else
                {
                    webReport.Report.Load(webRootPath + "\\ReqWithoutBOMF.frx"); // default report

                }
                //string my_connection_string = _iconfiguration.GetConnectionString("eTactDB");
                string my_connection_string = _connectionStringService.GetConnectionString();
                webReport.Report.Dictionary.Connections[0].ConnectionString = my_connection_string;
                webReport.Report.Dictionary.Connections[0].ConnectionStringExpression = "";
                webReport.Report.SetParameterValue("entryparam", EntryId);
                webReport.Report.SetParameterValue("yearparam", YearCode);
                webReport.Report.SetParameterValue("MyParameter", my_connection_string);
                webReport.Report.Refresh();
                return View(webReport);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Report generation failed: {ex.Message}");
            }
        }

        public ActionResult HtmlSave(int EntryId = 0, int YearCode = 0)
        {
            using (Report report = new Report())
            {
                string webRootPath = _IWebHostEnvironment.WebRootPath;
                var webReport = new WebReport();


                webReport.Report.Load(webRootPath + "\\requisitionWithoutBOM.frx");
                //webReport.Report.SetParameterValue("flagparam", "PURCHASEORDERPRINT");
                webReport.Report.SetParameterValue("EntryId", EntryId);
                webReport.Report.SetParameterValue("YearCode", YearCode);
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


                webReport.Report.Load(webRootPath + "\\requisitionWithoutBOM.frx");
                //webReport.Report.SetParameterValue("flagparam", "PURCHASEORDERPRINT");
                webReport.Report.SetParameterValue("EntryId", EntryId);
                webReport.Report.SetParameterValue("YearCode", YearCode);
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

        //[Route("{controller}/Index")]
        //public async Task<IActionResult> ReqWithoutBom(string formKey)
        //{
        //    ViewBag.formKey = formKey;
        //    var uniqueKey = Guid.NewGuid().ToString();
        //    ViewBag.uniqueKey = uniqueKey;
        //    ViewData["Title"] = "Requisition Without BOM Detail";
        //    TempData.Clear();
        //    HttpContext.Session.Remove($"KeyReqWithoutBOMGrid_{uniqueKey}");
        //    var MainModel = new RequisitionWithoutBOMModel();


        //    MainModel = await BindModel(MainModel);
        //    MainModel.FinFromDate = HttpContext.Session.GetString($"FromDate_{formKey}");
        //    MainModel.FinToDate = HttpContext.Session.GetString($"ToDate_{formKey}");
        //    MainModel.Mode = "F";
        //    MainModel.YearCode = Convert.ToInt32(HttpContext.Session.GetString($"YearCode_{formKey}"));

        //    string serializedGrid = JsonConvert.SerializeObject(MainModel);
        //    HttpContext.Session.SetString($"KeyReqWithoutBOMGrid_{uniqueKey}", serializedGrid);
        //    return View(MainModel);
        //}

        [Route("{controller}/Index")]
        [HttpGet]
        //public async Task<ActionResult> ReqWithoutBom(int ID, string Mode, int YC, string REQNo = "",string ItemName = "",string PartCode = "", string WorkCenter = "", string WONo = "", string DeptName = "", string DashboardType = "", string FromDate = "", string ToDate = "", string GlobalSearch = "")//, ILogger logger)
        public async Task<ActionResult> ReqWithoutBom(int ID, int MenuId, string Mode, int YC, string formKey)//, ILogger logger)
        {
            ViewBag.formKey = formKey;
            var uniqueKey = Guid.NewGuid().ToString();
            ViewBag.uniqueKey = uniqueKey;
            int userID = Convert.ToInt32(HttpContext.Session.GetString($"EmpID_{formKey}"));
            var rights = await _IReqWithoutBOM.GetFormRights(userID);
            if (rights?.Result == null || rights.Result.Tables.Count == 0 || rights.Result.Tables[0].Rows.Count == 0)
            {
                return RedirectToAction("Dashboard", "Home", new { formKey = formKey });
            }

            var table = rights.Result.Tables[0];
            string encID = Request.Query["ID"].ToString();
            string encYC = Request.Query["YC"].ToString();

            if (!string.IsNullOrEmpty(encID))
            {
                int decryptedID = EncryptDecrypt.DecodeID(encID);
                int decryptedYC = EncryptDecrypt.DecodeID(encYC);
                string decryptedMode = EncryptDecrypt.Decrypt(Mode);
                ID = decryptedID;
                Mode = decryptedMode;
                YC = decryptedYC;

            }

            bool optAll = Convert.ToBoolean(table.Rows[0]["OptAll"]);
            bool optView = Convert.ToBoolean(table.Rows[0]["OptView"]);
            bool optUpdate = Convert.ToBoolean(table.Rows[0]["OptUpdate"]);
            bool optSave = Convert.ToBoolean(table.Rows[0]["OptSave"]);


            if (Mode == "U")
            {
                if (!(optUpdate))
                {
                    return RedirectToAction("Dashboard", "Home", new { formKey = formKey });
                }
            }
            else if (Mode == "V")
            {
                if (!(optView))
                {
                    return RedirectToAction("Dashboard", "Home", new { formKey = formKey });
                }
            }
            else if (ID <= 0)
            {
                if (!optSave)
                {
                    return RedirectToAction("DashBoard", "ReqWithoutBom", new { formKey = formKey });
                }
                //if (!(optAll || optSave))
                //{
                //    return RedirectToAction("Dashboard", "Home",new {formKey=formKey});
                //}

            }
            //_logger.LogInformation("\n \n ********** Page Gate Inward ********** \n \n " + IWebHostEnvironment.EnvironmentName.ToString() + "\n \n");
            //TempData.Clear();
            var MainModel = new RequisitionWithoutBOMModel();

            MainModel.CC = HttpContext.Session.GetString($"Branch_{formKey}");
            MainModel.YearCode = Convert.ToInt32(HttpContext.Session.GetString($"YearCode_{formKey}"));
            HttpContext.Session.Remove($"KeyReqWithoutBOMGrid_{uniqueKey}");
            if (!string.IsNullOrEmpty(Mode) && ID > 0 && (Mode == "V" || Mode == "U"))
            {
                MainModel = await _IReqWithoutBOM.GetViewByID(ID, YC).ConfigureAwait(false);
                MainModel.Mode = Mode;
                MainModel.ID = ID;
                MainModel = await BindModel(MainModel).ConfigureAwait(false);

                string serializedGrid = JsonConvert.SerializeObject(MainModel.ReqDetailGrid);
                HttpContext.Session.SetString($"KeyReqWithoutBOMGrid_{uniqueKey}", serializedGrid);
            }
            else
            {
                MainModel = await BindModel(MainModel);
            }
            if (Mode != "U")
            {
                MainModel.CreatedBy = Convert.ToInt32(HttpContext.Session.GetString($"UID_{formKey}"));
                MainModel.CreatedByName = HttpContext.Session.GetString($"EmpName_{formKey}");
                MainModel.CreatedOn = DateTime.Now;
            }
            else
            {
                //MainModel.UpdatedBy = Convert.ToInt32(HttpContext.Session.GetString($"UID_{formKey}"));
                // MainModel.UpdatedByName = HttpContext.Session.GetString($"EmpName_{formKey}");
                // MainModel.UpdatedOn = DateTime.Now;
            }

            MainModel.FinFromDate = CommonFunc.ParseFormattedDate(HttpContext.Session.GetString($"FromDate_{formKey}"));
            MainModel.FinToDate = CommonFunc.ParseFormattedDate(HttpContext.Session.GetString($"ToDate_{formKey}"));

            //MainModel.FromDateBack = FromDate;
            //MainModel.ToDateBack = ToDate;
            //MainModel.REQNoBack = REQNo;
            //MainModel.PartCodeBack = PartCode;
            //MainModel.ItemNameBack = ItemName;
            //MainModel.WorkCenterBack = WorkCenter;
            //MainModel.WorkOrderNoback = WONo;
            //MainModel.DeptNameBack = DeptName;
            //MainModel.DashboardTypeBack = DashboardType;
            //MainModel.GlobalSearchBack = GlobalSearch;
            MainModel.MenuId = MenuId;
            return View(MainModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("{controller}/Index")]
        public async Task<IActionResult> ReqWithoutBom(RequisitionWithoutBOMModel model, string ShouldPrint)
        {
            try
            {
                var ReqGrid = new DataTable();
                var formKey = model.formKey;
                var uniqueKey = model.uniqueKey;
                var MenuId = model.MenuId;
                var mainmodel2 = model;
                string modelJson = HttpContext.Session.GetString($"KeyReqWithoutBOMGrid_{uniqueKey}");
                List<RequisitionDetail> RequisitionDetail = new List<RequisitionDetail>();
                if (!string.IsNullOrEmpty(modelJson))
                {
                    RequisitionDetail = JsonConvert.DeserializeObject<List<RequisitionDetail>>(modelJson);
                }

                mainmodel2.ReqDetailGrid = RequisitionDetail;
                if (RequisitionDetail == null)
                {
                    ModelState.Clear();
                    ModelState.TryAddModelError("ReqWithoutBom", "ReqWithoutBom Grid Should Have Atleast 1 Item...!");
                    model = await BindModel(model);
                    return View("ReqWithoutBom", model);
                }
                else
                {
                    //model.CreatedBy = Constants.UserID;
                    if (model.Mode == "U")
                    {
                        model.UpdatedBy = Convert.ToInt32(HttpContext.Session.GetString($"UID_{formKey}"));
                        model.UpdatedByName = HttpContext.Session.GetString($"EmpName_{formKey}");
                    }
                    string IPAddress = HttpContext.Session.GetString($"ClientIP_{formKey}");
                    ReqGrid = GetDetailTable(RequisitionDetail, model.Mode);
                    model.EntryByMachineName = HttpContext.Session.GetString($"ClientMachineName_{formKey}");
                    model.IPAddress = HttpContext.Session.GetString($"ClientIP_{formKey}");
                    var Result = await _IReqWithoutBOM.SaveRequisition(model, ReqGrid, IPAddress);

                    if (Result != null)
                    {
                        if ((Result.StatusText == "Success" && Result.StatusCode == HttpStatusCode.OK) || (Result.StatusText == "Completed is Y" && Result.StatusCode == HttpStatusCode.Accepted))
                        {
                            ViewBag.isSuccess = true;
                            TempData["200"] = "200";
                            HttpContext.Session.Remove($"KeyReqWithoutBOMGrid_{uniqueKey}");
                            //if (ShouldPrint == "true")
                            //{
                            //    return RedirectToAction("PrintReport", new { EntryId = model.EntryId, YearCode = model.YearCode });
                            //}
                            //var MainModel = new RequisitionWithoutBOMModel();
                            //MainModel = await BindModel(MainModel);
                            //HttpContext.Session.Remove($"KeyReqWithoutBOMGrid_{uniqueKey}");
                            //return RedirectToAction(nameof(ReqWithoutBom));
                            if (ShouldPrint == "true")
                            {
                                return Json(new
                                {
                                    status = "Success",
                                    entryId = model.EntryId,
                                    yearCode = model.YearCode
                                });
                            }
                        }
                        else if ((Result.StatusText == "Updated" && Result.StatusCode == HttpStatusCode.Accepted) || (Result.StatusText == "Success" && Result.StatusCode == HttpStatusCode.Accepted))
                        {
                            ViewBag.isSuccess = true;
                            TempData["202"] = "202";
                            HttpContext.Session.Remove($"KeyReqWithoutBOMGrid_{uniqueKey}");
                            //if (ShouldPrint == "true")
                            //{
                            //    return RedirectToAction("PrintReport", new { EntryId = model.EntryId, YearCode = model.YearCode });
                            //}
                            //var MainModel = new RequisitionWithoutBOMModel();
                            //MainModel = await BindModel(MainModel);

                            //return RedirectToAction(nameof(ReqWithoutBom));
                            if (ShouldPrint == "true")
                            {
                                return Json(new
                                {
                                    status = "Success",
                                    entryId = model.EntryId,
                                    yearCode = model.YearCode
                                });
                            }
                        }
                        else if (Result.StatusText == "Error" && Result.StatusCode == HttpStatusCode.InternalServerError)
                        {
                            ViewBag.isSuccess = false;
                            TempData["500"] = "500";
                            _logger.LogError("\n \n ********** LogError ********** \n " + JsonConvert.SerializeObject(Result) + "\n \n");
                            //return View("Error", Result);
                            return Json(new
                            {
                                status = "error",
                                message = "something went wrong"
                            });
                        }
                        else if (!string.IsNullOrEmpty(Result.StatusText))
                        {
                            // If SP returned a message (like adjustment error)
                            TempData["ErrorMessage"] = Result.StatusText;

                            return Json(new
                            {
                                status = "error",
                                message = Result.StatusText
                            });
                            //return RedirectToAction("PendingMaterialToIssueThrBOM", "PendingMaterialToIssueThrBOM");
                            //return View(model);
                        }
                        else if (Result.StatusCode == System.Net.HttpStatusCode.BadRequest)
                        {
                            TempData["ErrorMessage"] = Result.StatusText;
                        }

                    }
                    //mainmodel2 = await BindModel(mainmodel2);
                    //return View(mainmodel2);
                    return Json(new { status = "Success" });

                }
            }
            catch (Exception ex)
            {
                LogException<ReqWithoutBomController>.WriteException(_logger, ex);


                var ResponseResult = new ResponseResult()
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    StatusText = "Error",
                    Result = ex
                };

                return View("Error", ResponseResult);
            }
        }
        public async Task<JsonResult> AutoFillPartCode(string TF, string SearchItemCode, string SearchPartCode)
        {
            var JSON = await _IReqWithoutBOM.AutoFillitem("AutoFillPartCode", TF, SearchItemCode, SearchPartCode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> AutoFillItemName(string TF, string SearchItemCode, string SearchPartCode)
        {
            var JSON = await _IReqWithoutBOM.AutoFillitem("AutoFillItemName", TF, SearchItemCode, SearchPartCode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetFormRights(string formKey)
        {
            var userID = Convert.ToInt32(HttpContext.Session.GetString($"EmpID_{formKey}"));
            var JSON = await _IReqWithoutBOM.GetFormRights(userID);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetProjectNo()
        {
            var JSON = await _IReqWithoutBOM.GetProjectNo();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<IActionResult> Dashboard(int MenuId, string FromDate, string Todate, string Flag, string formKey)
        {
            try
            {
                ViewBag.formKey = formKey;
                var uniqueKey = Guid.NewGuid().ToString();
                ViewBag.uniqueKey = uniqueKey;
                int userID = Convert.ToInt32(HttpContext.Session.GetString($"EmpID_{formKey}"));
                var rights = await _IReqWithoutBOM.GetFormRights(userID);
                if (rights?.Result == null || rights.Result.Tables.Count == 0 || rights.Result.Tables[0].Rows.Count == 0)
                {
                    return RedirectToAction("Dashboard", "Home", new { formKey = formKey });
                }
                var table = rights.Result.Tables[0];

                bool optAll = Convert.ToBoolean(table.Rows[0]["OptAll"]);
                bool optView = Convert.ToBoolean(table.Rows[0]["OptView"]);
                bool optUpdate = Convert.ToBoolean(table.Rows[0]["OptUpdate"]);
                bool optDelete = Convert.ToBoolean(table.Rows[0]["OptDelete"]);
                if (!(optAll || optView || optUpdate || optDelete))
                {
                    return RedirectToAction("Dashboard", "Home", new { formKey = formKey });
                }
                HttpContext.Session.Remove($"KeyReqWithoutBOMGrid_{uniqueKey}");
                var model = new ReqMainDashboard();
                model.Mode = "Summary";
                model.CC = HttpContext.Session.GetString($"Branch_{formKey}");
                var Result = await _IReqWithoutBOM.GetDashboardData(FromDate, Todate, Flag).ConfigureAwait(true);

                if (Result != null)
                {
                    var _List = new List<TextValue>();
                    DataSet DS = Result.Result;
                    if (DS != null)
                    {
                        var DT = DS.Tables[0].DefaultView.ToTable(false,
   "REQNo", "ReqDate", "EntryDate", "EntryTime", "WorkCenter", "DeptName", "WONO",
   "BranchName", "Reason", "Cancel", "MachName", "WOYearcode",
   "EntryId", "YearCode", "TotalReqQty", "TotalPendQty", "Completed",
   "CreatedByName", "UpdatedByName");
                        model.ReqMainDashboard = CommonFunc.DataTableToList<RWBDashboard>(DT, "ReqDashboard");
                    }
                }
                if (Flag != "True")
                {
                    model.FromDate1 = FromDate;
                    model.ToDate1 = Todate;

                }
                // if (Flag == "True")
                return View(model);
                //else
                //{
                //    return PartialView("_ReqWithoutBomDashboardGrid", model);
                //}
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IActionResult> DeleteByID(int MenuId, string formKey, int ID, int YC, string FromDate, string ToDate, string REQNo, string WCName, string WONo, string DepName, string PartCode, string ItemName)
        {
            int UpdatedBy = Convert.ToInt32(HttpContext.Session.GetString($"EmpID_{formKey}"));
            var EntryByMachineName = Environment.MachineName;
            var IPAddress = HttpContext.Session.GetString($"ClientIP_{formKey}");
            var Result = await _IReqWithoutBOM.DeleteByID(ID, YC, UpdatedBy, EntryByMachineName, IPAddress);
            var CC = HttpContext.Session.GetString($"Branch_{formKey}");
            if (Result.StatusText == "Success" || Result.StatusCode == HttpStatusCode.Gone)
            {
                ViewBag.isSuccess = true;
                TempData["410"] = "410";
            }
            else if (Result.StatusText == "Error")
            {
                ViewBag.isSuccess = true;
                TempData["423"] = "423";
            }
            else
            {
                //    ViewBag.isSuccess = false;
                //    TempData["500"] = "500";
                //}
                //if (Result.StatusCode == System.Net.HttpStatusCode.BadRequest)
                //{
                TempData["ErrorMessage"] = Result.StatusText;
            }
            DateTime fromDt = DateTime.ParseExact(FromDate, "dd/MM/yyyy", null);
            string formattedFromDate = fromDt.ToString("dd/MMM/yyyy 00:00:00");
            DateTime toDt = DateTime.ParseExact(ToDate, "dd/MM/yyyy", null);
            string formattedToDate = toDt.ToString("dd/MMM/yyyy 00:00:00");

            return RedirectToAction("Dashboard", new { MenuId = MenuId, FromDate = formattedFromDate, ToDate = formattedToDate, Flag = "False", REQNo = REQNo, WCName = WCName, WONo = WONo, DepName = DepName, PartCode = PartCode, ItemName = ItemName, BranchName = CC });
        }
        private static DataTable GetDetailTable(IList<RequisitionDetail> DetailList, string mode)
        {
            var ReqGrid = new DataTable();

            ReqGrid.Columns.Add("EntryId", typeof(int));
            ReqGrid.Columns.Add("YearCode", typeof(int));
            ReqGrid.Columns.Add("Seqno", typeof(int));
            ReqGrid.Columns.Add("ItemCode", typeof(int));
            ReqGrid.Columns.Add("Unit", typeof(string));
            ReqGrid.Columns.Add("Qty", typeof(decimal));
            ReqGrid.Columns.Add("AltUnit", typeof(string));
            ReqGrid.Columns.Add("AltQty", typeof(decimal));
            ReqGrid.Columns.Add("ItemModel", typeof(string));
            ReqGrid.Columns.Add("ItemSize", typeof(string));
            ReqGrid.Columns.Add("ExpectedDate", typeof(DateTime));
            ReqGrid.Columns.Add("Remark", typeof(string));
            ReqGrid.Columns.Add("PendQty", typeof(decimal));
            ReqGrid.Columns.Add("PendAltQty", typeof(decimal));
            ReqGrid.Columns.Add("StoreId", typeof(int));
            ReqGrid.Columns.Add("TotalStock", typeof(decimal));
            ReqGrid.Columns.Add("Cancel", typeof(string));
            ReqGrid.Columns.Add("ProjectNo", typeof(string));
            ReqGrid.Columns.Add("ProjectYearCode", typeof(int));
            ReqGrid.Columns.Add("CostCenterId", typeof(int));
            ReqGrid.Columns.Add("ItemLocation", typeof(string));
            ReqGrid.Columns.Add("ItemBinRackNo", typeof(string));
            ReqGrid.Columns.Add("ItemRemark", typeof(string));

            foreach (var Item in DetailList)
            {
                DateTime expDt = new DateTime();
                if (mode != "U")
                {
                    if (Item.ExpectedDate != null)
                        expDt = DateTime.ParseExact(Item.ExpectedDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                }
                ReqGrid.Rows.Add(
                    new object[]
                    {
                    1,
                    2023,
                    Item.SeqNo,
                    Item.ItemCode,
                    Item.Unit,
                    Item.Qty,
                    Item.AltUnit,
                    Item.AltQty,
                    Item.ItemModel,
                    Item.ItemSize,
                    mode=="U"? Item.ExpectedDate: expDt.ToString("yyyy/MM/dd"),
                    Item.Remark,
                    Item.PendQty,
                    Item.PendAltQty,
                    Item.StoreId ?? 0,
                    Item.TotalStock,
                    Item.Cancle,
                    Item.ProjectNo,
                    Item.ProjectYearCode,
                    Item.CostCenterId,
                    Item.ItemLocation,
                    Item.ItemBinRackNo,
                    Item.ItemRemark
                    });
            }
            ReqGrid.Dispose();
            return ReqGrid;
        }
        public async Task<IActionResult> DeleteItemRow(int SeqNo, string uniqueKey)
        {
            var MainModel = new RequisitionWithoutBOMModel();
            string modelJson = HttpContext.Session.GetString($"KeyReqWithoutBOMGrid_{uniqueKey}");
            List<RequisitionDetail> RequisitionDetail = new List<RequisitionDetail>();
            if (!string.IsNullOrEmpty(modelJson))
            {
                RequisitionDetail = JsonConvert.DeserializeObject<List<RequisitionDetail>>(modelJson);
            }

            int Indx = Convert.ToInt32(SeqNo) - 1;

            if (RequisitionDetail != null && RequisitionDetail.Count > 0)
            {
                RequisitionDetail.RemoveAt(Convert.ToInt32(Indx));

                Indx = 0;

                foreach (var item in RequisitionDetail)
                {
                    Indx++;
                    item.SeqNo = Indx;
                }
                MainModel.ReqDetailGrid = RequisitionDetail;


                if (RequisitionDetail.Count == 0)
                {
                    HttpContext.Session.Remove($"KeyReqWithoutBOMGrid_{uniqueKey}");
                }
                HttpContext.Session.SetString($"KeyReqWithoutBOMGrid_{uniqueKey}", JsonConvert.SerializeObject(RequisitionDetail));
            }
            return PartialView("_ReqWithoutBomGrid", MainModel);
        }
        public IActionResult AddReqWithoutBomDetail(RequisitionDetail model, string uniqueKey)
        {
            try
            {
                string modelJson = HttpContext.Session.GetString($"KeyReqWithoutBOMGrid_{uniqueKey}");
                List<RequisitionDetail> GridDetail = new List<RequisitionDetail>();
                if (!string.IsNullOrEmpty(modelJson))
                {
                    GridDetail = JsonConvert.DeserializeObject<List<RequisitionDetail>>(modelJson);
                }

                var MainModel = new RequisitionWithoutBOMModel();
                var ReqWithoutBOMGrid = new List<RequisitionDetail>();
                var ReqGrid = new List<RequisitionDetail>();
                var SSGrid = new List<RequisitionDetail>();

                if (model != null)
                {
                    if (GridDetail == null)
                    {
                        model.SeqNo = 1;
                        if (model.CostCenterName == "-Select-")
                        {
                            model.CostCenterName = "NA";
                        }
                        if (model.StoreName == "-Select-")
                        {
                            model.StoreName = "NA";
                        }
                        ReqGrid.Add(model);
                    }
                    else
                    {
                        if (GridDetail.Where(x => x.ItemCode == model.ItemCode).Any())
                        {
                            return StatusCode(207, "Duplicate");
                        }
                        else
                        {
                            if (model.CostCenterName == "-Select-")
                            {
                                model.CostCenterName = "NA";
                            }
                            if (model.StoreName == "-Select-")
                            {
                                model.StoreName = "NA";
                            }
                            model.SeqNo = GridDetail.Count + 1;
                            ReqGrid = GridDetail.Where(x => x != null).ToList();
                            SSGrid.AddRange(ReqGrid);
                            ReqGrid.Add(model);
                        }
                    }

                    MainModel.ReqDetailGrid = ReqGrid;

                    MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
                    {
                        AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                        SlidingExpiration = TimeSpan.FromMinutes(55),
                        Size = 1024,
                    };
                    string serializedGrid = JsonConvert.SerializeObject(MainModel.ReqDetailGrid);
                    HttpContext.Session.SetString($"KeyReqWithoutBOMGrid_{uniqueKey}", serializedGrid);
                }
                else
                {
                    ModelState.TryAddModelError("Error", "Schedule List Cannot Be Empty...!");
                }

                return PartialView("_ReqWithoutBomGrid", MainModel);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private async Task<RequisitionWithoutBOMModel> BindModel(RequisitionWithoutBOMModel model)
        {
            if (model.ID == 0)
            {
                model.EntryTime = DateTime.Now.ToString("hh:mm tt");
            }
            var oDataSet = new DataSet();
            var _List = new List<TextValue>();
            oDataSet = await _IReqWithoutBOM.BindAllDropDowns("BINDALLDROPDOWN");

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
                        Value = row["projectname"].ToString(),
                        Text = row["yearcode"].ToString()
                    });
                }
                model.ProjectList = _List;
                _List = new List<TextValue>();
                foreach (DataRow row in oDataSet.Tables[2].Rows)
                {
                    _List.Add(new TextValue
                    {
                        Value = row["EntryId"].ToString(),
                        Text = row["DeptName"].ToString()
                    });
                }
                model.DepartmentList = _List;

                _List = new List<TextValue>();
                foreach (DataRow row in oDataSet.Tables[3].Rows)
                {
                    _List.Add(new TextValue
                    {
                        Value = row["EntryID"].ToString(),
                        Text = row["CostCenterName"].ToString()
                    });
                }
                model.CostCenterList = _List;
                _List = new List<TextValue>();
                foreach (DataRow row in oDataSet.Tables[4].Rows)
                {
                    _List.Add(new TextValue
                    {
                        Value = row["Emp_Id"].ToString(),
                        Text = row["EmpNameCode"].ToString()
                    });
                }
                model.EmployeeList = _List;
                _List = new List<TextValue>();
                foreach (DataRow row in oDataSet.Tables[5].Rows)
                {
                    _List.Add(new TextValue
                    {
                        Value = row["entryid"].ToString(),
                        Text = row["machinename"].ToString()
                    });
                }
                model.MachineList = _List;
                _List = new List<TextValue>();
                foreach (DataRow row in oDataSet.Tables[6].Rows)
                {
                    if (string.IsNullOrEmpty(model.Mode) || (model.Mode != "U" && model.Mode != "V"))
                        if (row["Store_Type"]?.ToString() == "MAIN STORE")
                        {

                            model.StoreId = Convert.ToInt32(row["EntryID"]);
                        }
                    _List.Add(new TextValue
                    {
                        Value = row["EntryID"].ToString(),
                        Text = row["Store_Name"].ToString()
                    });
                }
                model.StoreList = _List;

            }
            return model;
        }
        public async Task<JsonResult> FillItems(string TF)
        {
            var JSON = await _IReqWithoutBOM.FillItems(TF);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillDept()
        {
            var JSON = await _IReqWithoutBOM.FillDept();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillPartCode(string TF)
        {
            var JSON = await _IReqWithoutBOM.FillPartCode(TF);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillWorkOrder()
        {
            var JSON = await _IReqWithoutBOM.FillWorkOrder();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> CheckFeatureOption()
        {
            var JSON = await _IReqWithoutBOM.CheckFeatureOption();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillWorkCenter()
        {
            var JSON = await _IReqWithoutBOM.FillWorkCenter();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillStore()
        {
            var JSON = await _IReqWithoutBOM.FillStore();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillTotalStock(int ItemCode, int Store)
        {
            var JSON = await _IReqWithoutBOM.FillTotalStock(ItemCode, Store);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<IActionResult> GetSearchData(string formKey, string uniqueKey, string REQNo, string WCName, string WONo, string DepName, string PartCode, string ItemName, string BranchName, string FromDate, string ToDate)
        {
            int userID = Convert.ToInt32(HttpContext.Session.GetString($"EmpID_{formKey}"));
            //model.Mode = "Search";
            var model = new RWBDashboard();
            model.formKey = formKey;
            model.uniqueKey = uniqueKey;
            ViewBag.formKey = formKey;

            model = await _IReqWithoutBOM.GetDashboardData(REQNo, WCName, WONo, DepName, PartCode, ItemName, BranchName, FromDate, ToDate, userID);
            model.Mode = "Summary";
            MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                SlidingExpiration = TimeSpan.FromMinutes(55),
                Size = 1024,
            };

            string serializedGrid = JsonConvert.SerializeObject(model.ReqMainDashboard);
            HttpContext.Session.SetString("KeyRWBList", serializedGrid);
            return PartialView("_ReqWithoutBomDashboardGrid", model);
        }
        public async Task<IActionResult> GetDetailData(string formKey, string uniqueKey, string REQNo, string WCName, string WONo, string DepName, string PartCode, string ItemName, string BranchName, string FromDate, string ToDate)
        {
            //model.Mode = "Search";
            var model = new RWBDashboard();
            model.formKey = formKey;
            model.uniqueKey = uniqueKey;
            ViewBag.formKey = formKey;
            model = await _IReqWithoutBOM.GetDetailData(REQNo, WCName, WONo, DepName, PartCode, ItemName, BranchName, FromDate, ToDate);
            model.Mode = "Detail";
            MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                SlidingExpiration = TimeSpan.FromMinutes(55),
                Size = 1024,
            };
            string serializedGrid = JsonConvert.SerializeObject(model.ReqMainDashboard);
            HttpContext.Session.SetString("KeyRWBList", serializedGrid);
            return PartialView("_ReqWithoutBomDashboardGrid", model);
        }
        public async Task<JsonResult> GetNewEntry(int YearCode)
        {
            var JSON = await _IReqWithoutBOM.GetNewEntry("NewEntryId", YearCode, "SP_RequisitionWithoutBOM");
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> AltUnitConversion(int ItemCode, decimal AltQty, decimal UnitQty)
        {
            var JSON = await _IReqWithoutBOM.AltUnitConversion(ItemCode, AltQty, UnitQty);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public IActionResult EditItemRow(int SeqNo, string uniqueKey)
        {
            var model = new RequisitionWithoutBOMModel();
            string modelJson = HttpContext.Session.GetString($"KeyReqWithoutBOMGrid_{uniqueKey}");
            List<RequisitionDetail> RequisitionDetail = new List<RequisitionDetail>();
            if (!string.IsNullOrEmpty(modelJson))
            {
                RequisitionDetail = JsonConvert.DeserializeObject<List<RequisitionDetail>>(modelJson);
            }

            var SSGrid = RequisitionDetail.Where(x => x.SeqNo == SeqNo);
            string JsonString = JsonConvert.SerializeObject(SSGrid);
            return Json(JsonString);
        }
        [HttpGet]
        public IActionResult GetReqWithoutBomDashBoardGridData()
        {
            string modelJson = HttpContext.Session.GetString("KeyRWBList");
            List<RWBDashboard> stockRegisterList = new List<RWBDashboard>();
            if (!string.IsNullOrEmpty(modelJson))
            {
                stockRegisterList = JsonConvert.DeserializeObject<List<RWBDashboard>>(modelJson);
            }

            return Json(stockRegisterList);
        }
    }
}
