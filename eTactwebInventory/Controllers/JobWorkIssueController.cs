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
using System.Data;
using static eTactWeb.Data.Common.CommonFunc;
using static eTactWeb.DOM.Models.Common;
using static eTactWeb.DOM.Models.JobWorkIssueModel;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Globalization;
using FastReport.Data;
using System.Configuration;
//using JobWorkGridDetail = eTactWeb.DOM.Models.JobWorkGridDetail;

namespace eTactWeb.Controllers
{
    public class JobWorkIssueController : Controller
    {
        private readonly IConfiguration _iconfiguration;
        private readonly IDataLogic _IDataLogic;
        private readonly IJobWorkIssue _IJobWorkIssue;
        private readonly ILogger<JobWorkIssueController> _logger;
        private readonly IIssueWithoutBom _IIssueWOBOM;
        public IWebHostEnvironment _IWebHostEnvironment { get; }

        public JobWorkIssueController(ILogger<JobWorkIssueController> logger, IDataLogic iDataLogic, IJobWorkIssue iJobWorkIssue, EncryptDecrypt encryptDecrypt, IWebHostEnvironment iWebHostEnvironment, IConfiguration iconfiguration, IIssueWithoutBom IIssueWOBOM)
        {
            _logger = logger;
            _IDataLogic = iDataLogic;
            _IJobWorkIssue = iJobWorkIssue;
            _IWebHostEnvironment = iWebHostEnvironment;
            this._iconfiguration = iconfiguration;
            _IIssueWOBOM = IIssueWOBOM;
        }


        public IActionResult PrintReport(int EntryId = 0, int YearCode = 0)
        {
            string my_connection_string;
            string contentRootPath = _IWebHostEnvironment.ContentRootPath;
            string webRootPath = _IWebHostEnvironment.WebRootPath;
            var webReport = new WebReport();
            webReport.Report.Clear();
            var ReportName = _IJobWorkIssue.GetReportName();
            webReport.Report.Dispose();
            webReport.Report = new Report();
            if (!String.Equals(ReportName.Result.Result.Rows[0].ItemArray[0], System.DBNull.Value))
            {
                webReport.Report.Load(webRootPath + "\\" + ReportName.Result.Result.Rows[0].ItemArray[0]); // from database
            }
            else
            {
                webReport.Report.Load(webRootPath + "\\IssueVendJobworkChallan.frx"); // default report
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


                webReport.Report.Load(webRootPath + "\\IssueVendJobworkChallan.frx");
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


                webReport.Report.Load(webRootPath + "\\IssueVendJobworkChallan.frx");
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
        [Route("{controller}/Index")]
        public async Task<IActionResult> JobWorkIssue()
        {
            ViewData["Title"] = "Job Work Details";
            TempData.Clear();
            HttpContext.Session.Remove("KeyJobWorkIssue");
            HttpContext.Session.Remove("JobWorkIssue");
            HttpContext.Session.Remove("KeyTaxGrid");
            var MainModel = new JobWorkIssueModel();
            var TaxModel = new TaxModel();
            MainModel = await BindModel(MainModel);
            MainModel.FinFromDate = HttpContext.Session.GetString("FromDate");
            MainModel.FinToDate = HttpContext.Session.GetString("ToDate");
            MainModel.YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
            MainModel.CC = HttpContext.Session.GetString("Branch");
            MainModel.EnterByMachineName = Environment.MachineName;;

            string serializedIssueGrid = JsonConvert.SerializeObject(MainModel);
            HttpContext.Session.SetString("KeyJobWorkIssue", serializedIssueGrid);
            string serializedGrid = JsonConvert.SerializeObject(MainModel);
            HttpContext.Session.SetString("JobWorkIssue", serializedGrid);
            string serializedTaxGrid = JsonConvert.SerializeObject(MainModel.TaxDetailGridd);
            HttpContext.Session.SetString("KeyTaxGrid", serializedTaxGrid);
            
            HttpContext.Session.SetString("JobWorkIssue", JsonConvert.SerializeObject(MainModel));
            return View(MainModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("{controller}/Index")]
        public async Task<IActionResult> JobWorkIssue(JobWorkIssueModel model)
        {
            try
            {
                var JWGrid = new DataTable();
                DataTable TaxDetailDT = null;

                string modelJson = HttpContext.Session.GetString("KeyJobWorkIssue");
                List<JobWorkGridDetail> JobWorkGridDetail = new List<JobWorkGridDetail>();
                if (!string.IsNullOrEmpty(modelJson))
                {
                    JobWorkGridDetail = JsonConvert.DeserializeObject<List<JobWorkGridDetail>>(modelJson);
                }

                string modelEditJson = HttpContext.Session.GetString("KeyJobWorkIssueEdit");
                List<JobWorkGridDetail> JobWorkGridDetailEdit = new List<JobWorkGridDetail>();
                if (!string.IsNullOrEmpty(modelEditJson))
                {
                    JobWorkGridDetailEdit = JsonConvert.DeserializeObject<List<JobWorkGridDetail>>(modelEditJson);
                }

                string modelTaxJson = HttpContext.Session.GetString("KeyTaxGrid");
                List<TaxModel> TaxGrid = new List<TaxModel>();
                if (!string.IsNullOrEmpty(modelTaxJson))
                {
                    TaxGrid = JsonConvert.DeserializeObject<List<TaxModel>>(modelTaxJson);
                }

                if (model.CC == null)
                {
                    model.CC = HttpContext.Session.GetString("Branch");
                }
                model.EnteredByEmpid = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                model.EnterByMachineName = Environment.MachineName;
                model.UID = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                if (model.Mode == "U")
                {

                    JWGrid = GetDetailTable(JobWorkGridDetailEdit, "U");
                    model.UpdatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                }
                else
                    JWGrid = GetDetailTable(JobWorkGridDetail, "");
                var Result = new ResponseResult();
                
                if (TaxGrid != null)
                {
                    TaxDetailDT = GetTaxDetailTable(TaxGrid);

                    
                }
                Result = await _IJobWorkIssue.SaveJobWorkIssue(model, JWGrid, TaxDetailDT);
                if (Result != null)
                {
                    if (Result.StatusText == "Success" && Result.StatusCode == HttpStatusCode.OK)
                    {
                        ViewBag.isSuccess = true;
                        TempData["200"] = "200";
                        return RedirectToAction(nameof(JobworkDashboard));

                    }
                    if (Result.StatusText == "Updated" && Result.StatusCode == HttpStatusCode.Accepted)
                    {
                        ViewBag.isSuccess = true;
                        TempData["202"] = "202";
                        return RedirectToAction(nameof(JobworkDashboard));

                    }
                    if (Result.StatusText == "Error" && Result.StatusCode == HttpStatusCode.InternalServerError)
                    {
                        ViewBag.isSuccess = false;
                        TempData["500"] = "500";
                        _logger.LogError("\n \n ********** LogError ********** \n " + JsonConvert.SerializeObject(Result) + "\n \n");
                        var MainModel = new JobWorkIssueModel();
                        MainModel = await BindModel(MainModel);
                        MainModel.FinFromDate = HttpContext.Session.GetString("FromDate");
                        MainModel.FinToDate = HttpContext.Session.GetString("ToDate");
                        MainModel.YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
                        MainModel.CC = HttpContext.Session.GetString("Branch");
                        if (model.Mode == "U")
                        {
                            MainModel.JobDetailGrid = JobWorkGridDetailEdit;
                        }
                        else
                        {
                            MainModel.JobDetailGrid = JobWorkGridDetail;
                        }
                        var Taxmodel = new TaxModel();

                        Taxmodel.TaxDetailGridd = TaxGrid;
                        MainModel.TaxDetailGrid = Taxmodel;

                        MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
                        {
                            AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                            SlidingExpiration = TimeSpan.FromMinutes(55),
                            Size = 1024,
                        };
                        HttpContext.Session.Remove("KeyJobWorkIssue");
                        HttpContext.Session.Remove("KeyJobWorkIssueEdit");
                        HttpContext.Session.Remove("KeyTaxGrid");
                        string serializedGrid = JsonConvert.SerializeObject(MainModel.JobDetailGrid);
                        HttpContext.Session.SetString("KeyJobWorkIssueEdit", serializedGrid);
                        string serializedJobWorkGrid = JsonConvert.SerializeObject(MainModel.JobDetailGrid);
                        HttpContext.Session.SetString("KeyJobWorkIssue", serializedJobWorkGrid);
                        string serializedTaxGrid = JsonConvert.SerializeObject(MainModel.TaxDetailGridd);
                        HttpContext.Session.SetString("KeyTaxGrid", serializedTaxGrid);

                        return View(MainModel);
                    }
                }
                return View(model);

            }
            catch (Exception ex)
            {
                LogException<JobWorkIssueController>.WriteException(_logger, ex);


                var ResponseResult = new ResponseResult()
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    StatusText = "Error",
                    Result = ex
                };

                return View("Error", ResponseResult);
            }
        }

        [Route("{controller}/Index")]
        [HttpGet]
        public async Task<ActionResult> JobWorkIssue(int ID, string Mode, int YC, string FromDate = "", string ToDate = "", string VendorName = "", string ChallanNo = "", string PartCode = "", string ItemName = "", string DashboardType = "")//, ILogger logger)
        {
            _logger.LogInformation("\n \n ********** Page Jobwork Issue ********** \n \n " + _IWebHostEnvironment.EnvironmentName.ToString() + "\n \n");
            TempData.Clear();
            var MainModel = new JobWorkIssueModel();
            var TaxModel = new TaxModel();

            MainModel.YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
            MainModel.FinFromDate = HttpContext.Session.GetString("FromDate");
            MainModel.FinToDate = HttpContext.Session.GetString("ToDate");
            MainModel.CC = HttpContext.Session.GetString("Branch");

            MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                SlidingExpiration = TimeSpan.FromMinutes(55),
                Size = 1024,
            };
            HttpContext.Session.Remove("KeyJobWorkIssue");  
            HttpContext.Session.Remove("KeyJobWorkIssueEdit");
            HttpContext.Session.Remove("KeyTaxGrid");
            if (!string.IsNullOrEmpty(Mode) && ID > 0 && (Mode == "V" || Mode == "U"))
            {
                MainModel = await _IJobWorkIssue.GetViewByID(ID, YC).ConfigureAwait(false);
                MainModel.Mode = Mode;
                MainModel.ID = ID;
                MainModel.ChangeEventTriggered = "Y";
                MainModel = await BindModel(MainModel).ConfigureAwait(false);
                MainModel.YearCode = YC;
                MainModel.FinFromDate = HttpContext.Session.GetString("FromDate");
                MainModel.FinToDate = HttpContext.Session.GetString("ToDate");
                MainModel.CC = HttpContext.Session.GetString("Branch");
                MainModel.EntryDate = ParseDate(MainModel.EntryDate).ToString();
                MainModel.ChallanDate = ParseDate(MainModel.ChallanDate).ToString();
                string serializedEditGrid = JsonConvert.SerializeObject(MainModel.JobDetailGrid);
                HttpContext.Session.SetString("KeyJobWorkIssueEdit", serializedEditGrid);
                string sTaxGrid = JsonConvert.SerializeObject(MainModel.JobDetailGrid);
                HttpContext.Session.SetString("KeyTaxGrid", sTaxGrid);
            }
            else
            {
                MainModel = await BindModel(MainModel);
            }
            if (Mode != "U")
            {
                MainModel.CreatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                MainModel.EnteredByEmpid = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                MainModel.CreatedByName = HttpContext.Session.GetString("EmpName");
                MainModel.CreatedOn = DateTime.Now;
            }
            else
            {
                MainModel.UpdatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                MainModel.UpdatedByName = HttpContext.Session.GetString("EmpName");
                MainModel.UpdatedOn = DateTime.Now;
            }
            string serializedGrid = JsonConvert.SerializeObject(MainModel);
            HttpContext.Session.SetString("JobWorkIssue", serializedGrid);
            HttpContext.Session.SetString("JobWorkIssue", JsonConvert.SerializeObject(MainModel));
            string serializedTaxGrid = JsonConvert.SerializeObject(MainModel);
            HttpContext.Session.SetString("KeyTaxGrid", serializedTaxGrid);
            MainModel.FromDateBack = FromDate;
            MainModel.ToDateBack = ToDate;
            MainModel.VendorNameBack = VendorName;
            MainModel.ItemNameBack = ItemName;
            MainModel.PartCodeBack = PartCode;
            MainModel.ChallanNoBack = ChallanNo;
            MainModel.DashboardTypeBack = DashboardType;
            return View(MainModel);
        }

        public async Task<IActionResult> JobworkDashboard(string FromDate = "", string ToDate = "", string Flag = "True", string DashboardType = "", string VendorName = "", string ChallanNo = "", string ItemName = "", string PartCode = "")
        {
            try
            {
                HttpContext.Session.Remove("KeyJobWorkIssue");
                var model = new JWIssQDashboard();
                var Result = await _IJobWorkIssue.GetDashboardData().ConfigureAwait(true);
                if (Result != null)
                {
                    var _List = new List<TextValue>();
                    DataSet DS = Result.Result;
                    if (DS != null)
                    {
                        var DT = DS.Tables[0].DefaultView.ToTable(true, "ChallanNo", "ChallanDate", "VendorName", "DeliveryAdd",
                            "VendorStateCode", "EntryDate", "BOMINd", "Closed", "CompletlyReceive", "Remarks", "EntryId", "YearCode", "TolApprVal", "TotalWt", "EnteredBy", "UpdatedBy");
                        model.JWIssQDashboard = CommonFunc.DataTableToList<JWIssueDashboard>(DT, "ISSVendJW");
                        foreach (var row in DS.Tables[0].AsEnumerable())
                        {
                            _List.Add(new TextValue()
                            {
                                Text = row["ChallanNo"].ToString(),
                                Value = row["ChallanNo"].ToString()
                            });
                        }
                        var _ChallanList = _List.DistinctBy(x => x.Value).ToList();
                        model.ChallanList = _ChallanList;
                        _List = new List<TextValue>();
                    }
                    if (Flag != "True")
                    {
                        model.FromDate1 = FromDate;
                        model.ToDate1 = ToDate;
                        model.Dashboardtype = DashboardType;
                        model.VendorName = VendorName;
                        model.ChallanNo = ChallanNo;
                        model.PartCode = PartCode;
                        model.ItemName = ItemName;
                    }
                }
                return View(model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<JsonResult> GetDefaultBranch()
        {
            var username = HttpContext.Session.GetString("Branch");
            return Json(username);
        }

        public IActionResult AddJobWorkIssueDetail(JobWorkGridDetail model)
        {
            try
            {
                if (model.Mode == "U")
                {
                    string modelJson = HttpContext.Session.GetString("KeyJobWorkIssueEdit");
                    List<JobWorkGridDetail> GridDetail = new List<JobWorkGridDetail>();
                    if (!string.IsNullOrEmpty(modelJson))
                    {
                        GridDetail = JsonConvert.DeserializeObject<List<JobWorkGridDetail>>(modelJson);
                    }

                    var MainModel = new JobWorkIssueModel();
                    var JobWorkGrid = new List<JobWorkGridDetail>();
                    var JobGrid = new List<JobWorkGridDetail>();
                    var SSGrid = new List<JobWorkGridDetail>();

                    if (model != null)
                    {
                        if (GridDetail == null)
                        {
                            model.SeqNo = 1;
                            JobGrid.Add(model);
                        }
                        else
                        {
                            if (GridDetail.Where(x => x.ItemCode == model.ItemCode && x.BatchNo == model.BatchNo && x.UniqueBatchNo == model.UniqueBatchNo).Any())
                            {
                                return StatusCode(207, "Duplicate");
                            }
                            else
                            {
                                model.SeqNo = GridDetail.Count + 1;
                                JobGrid = GridDetail.Where(x => x != null).ToList();
                                SSGrid.AddRange(JobGrid);
                                JobGrid.Add(model);
                            }
                        }
                        JobGrid = JobGrid.OrderBy(item => item.ItemCode).ThenBy(item=>item.SeqForBatch).ToList();
                        MainModel.JobDetailGrid = JobGrid;
                        MainModel.Mode = "U";
                        MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
                        {
                            AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                            SlidingExpiration = TimeSpan.FromMinutes(55),
                            Size = 1024,
                        };

                        string serializedGrid = JsonConvert.SerializeObject(MainModel.JobDetailGrid);
                        HttpContext.Session.SetString("KeyJobWorkIssueEdit", serializedGrid);
                        
                    }
                    else
                    {
                        ModelState.TryAddModelError("Error", "Schedule List Cannot Be Empty...!");
                    }

                    return PartialView("_JobWorkIssueGrid", MainModel);
                }
                else
                {
                    string modelJson = HttpContext.Session.GetString("KeyJobWorkIssue");
                    List<JobWorkGridDetail> GridDetail = new List<JobWorkGridDetail>();
                    if (!string.IsNullOrEmpty(modelJson))
                    {
                        GridDetail = JsonConvert.DeserializeObject<List<JobWorkGridDetail>>(modelJson);
                    }

                    var MainModel = new JobWorkIssueModel();
                    var JobWorkGrid = new List<JobWorkGridDetail>();
                    var JobGrid = new List<JobWorkGridDetail>();
                    var SSGrid = new List<JobWorkGridDetail>();

                    if (model != null)
                    {
                        if (GridDetail == null)
                        {
                            model.SeqNo = 1;
                            JobGrid.Add(model);
                        }
                        else
                        {
                            if (GridDetail.Where(x => x.ItemCode == model.ItemCode && x.BatchNo == model.BatchNo && x.UniqueBatchNo == model.UniqueBatchNo).Any())
                            {
                                return StatusCode(207, "Duplicate");
                            }
                            else
                            {
                                model.SeqNo = GridDetail.Count + 1;
                                JobGrid = GridDetail.Where(x => x != null).ToList();
                                SSGrid.AddRange(JobGrid);
                                JobGrid.Add(model);
                            }
                        }
                        JobGrid = JobGrid.OrderBy(item => item.ItemCode).ThenBy(item=>item.SeqForBatch).ToList();
                        MainModel.JobDetailGrid = JobGrid;

                        string serializedGrid = JsonConvert.SerializeObject(MainModel.JobDetailGrid);
                        HttpContext.Session.SetString("KeyJobWorkIssue", serializedGrid);
                        
                    }
                    else
                    {
                        ModelState.TryAddModelError("Error", "Schedule List Cannot Be Empty...!");
                    }

                    return PartialView("_JobWorkIssueGrid", MainModel);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<JsonResult> GetFormRights()
        {
            var userID = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
            var JSON = await _IJobWorkIssue.GetFormRights(userID);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> CheckFeatureOption()
        {
            var JSON = await _IJobWorkIssue.CheckFeatureOption();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetBatchNumber(int StoreId, string StoreName, int ItemCode, string TransDate, int YearCode, string BatchNo)
        {
            var FinFromDate = HttpContext.Session.GetString("FromDate");
            var JSON = await _IJobWorkIssue.GetBatchNumber("FillCurrentBatchINStore", StoreId, StoreName, ItemCode, TransDate, YearCode, BatchNo, FinFromDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillEmployee()
        {
            var JSON = await _IJobWorkIssue.FillEmployee();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillStore()
        {
            var JSON = await _IJobWorkIssue.FillStore();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillItem(string bomStatus)
        {
            var JSON = await _IJobWorkIssue.FillItem(bomStatus);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillPartCode(string bomStatus)
        {
            var JSON = await _IJobWorkIssue.FillPartCode(bomStatus);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillVendors()
        {
            var JSON = await _IJobWorkIssue.FillVendors();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillProcess()
        {
            var JSON = await _IJobWorkIssue.FillProcess();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetAddressDetails(int AccountCode)
        {
            var JSON = await _IJobWorkIssue.GetAddressDetails(AccountCode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillAdditionalFields(int AccountCode)
        {
            var JSON = await _IJobWorkIssue.FillAdditionalFields("AdviseDetail", AccountCode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetAllItems(string TF, string Types)
        {
            var JSON = await _IJobWorkIssue.GetAllItems("GetAllItems", TF, Types);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetAllPartCodes(string TF, string Types)
        {
            var JSON = await _IJobWorkIssue.GetAllItems("GetAllPartCode", TF, Types);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetStoreTotalStock(int ItemCode, int StoreId, string TillDate)
        {
            var JSON = await _IJobWorkIssue.GetStoreTotalStock("GETSTORETotalSTOCK", ItemCode, StoreId, TillDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetStockQty(int ItemCode, int StoreId, string TillDate, string BatchNo, string UniqueBatchNo)
        {
            var JSON = await _IJobWorkIssue.GetStockQty("GETSTORESTOCKBATCHWISE", ItemCode, StoreId, TillDate, BatchNo, UniqueBatchNo);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> ClearJobWorkGrid(int AccountCode)
        {
            HttpContext.Session.Remove("KeyJobWorkIssue");
            HttpContext.Session.Remove("KeyJobWorkIssueEdit");
            HttpContext.Session.Remove("KeyTaxGrid");
            var JSON = await _IJobWorkIssue.GetAddressDetails(AccountCode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> ClearTaxGrid(int AccountCode)
        {
            HttpContext.Session.Remove("KeyTaxGrid");
            var JSON = await _IJobWorkIssue.GetAddressDetails(AccountCode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetAmtAfterDiscount()
        {
            decimal amtAftrDiscount = 0;
            string modelJson = HttpContext.Session.GetString("KeyJobWorkIssue");
            List<JobWorkGridDetail> JobWorkGridDetail = new List<JobWorkGridDetail>();
            if (!string.IsNullOrEmpty(modelJson))
            {
                JobWorkGridDetail = JsonConvert.DeserializeObject<List<JobWorkGridDetail>>(modelJson);
            }
            string modelEditJson = HttpContext.Session.GetString("KeyJobWorkIssueEdit");
            List<JobWorkGridDetail> JobWorkEditGridDetail = new List<JobWorkGridDetail>();
            if (!string.IsNullOrEmpty(modelEditJson))
            {
                JobWorkEditGridDetail = JsonConvert.DeserializeObject<List<JobWorkGridDetail>>(modelEditJson);
            }
            if (JobWorkGridDetail != null)
            {
                if (JobWorkGridDetail != null && JobWorkGridDetail.Count > 0)
                {
                    foreach (var item in JobWorkGridDetail)
                    {
                        amtAftrDiscount += item.Amount;
                    }
                }
            }
            else
            {
                if (JobWorkEditGridDetail != null && JobWorkEditGridDetail.Count > 0)
                {
                    foreach (var item in JobWorkEditGridDetail)
                    {
                        amtAftrDiscount += item.Amount;
                    }
                }
            }
            string amtFinal = amtAftrDiscount.ToString();
            return Json(amtFinal);

        }
        public async Task<JsonResult> GetPrevQty(int EntryId, int YearCode, int ItemCode, string uniqueBatchNo)
        {
            var JSON = await _IJobWorkIssue.GetPrevQty(EntryId, YearCode, ItemCode, uniqueBatchNo);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetItemRate(int ItemCode, string TillDate, int YearCode, string BatchNo, string UniqueBatchNo)
        {
            var JSON = await _IJobWorkIssue.GetItemRate(ItemCode, TillDate, YearCode, BatchNo, UniqueBatchNo);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetSumofTaxGrid()
        {
            decimal NetTotal = 0;
            string modelJson = HttpContext.Session.GetString("KeyTaxGrid");
            List<TaxModel> TaxGrid = new List<TaxModel>();
            if (!string.IsNullOrEmpty(modelJson))
            {
                TaxGrid = JsonConvert.DeserializeObject<List<TaxModel>>(modelJson);
            }
            if (TaxGrid != null && TaxGrid.Count > 0)
            {
                foreach (var item in TaxGrid)
                {
                    NetTotal += item.TxAmount;
                }
            }
            string amtFinal = NetTotal.ToString();
            return Json(amtFinal);

        }
        public async Task<IActionResult> DeleteItemRow(int SeqNo, string Mode,int ItemCode,string BatchNo,int Index=0)
        {
            var MainModel = new JobWorkIssueModel();
            string modelJson = HttpContext.Session.GetString("KeyJobWorkIssue");
            List<JobWorkGridDetail> JobWorkGridDetail = new List<JobWorkGridDetail>();
            if (!string.IsNullOrEmpty(modelJson))
            {
                JobWorkGridDetail = JsonConvert.DeserializeObject<List<JobWorkGridDetail>>(modelJson);
            }
            string modelEditJson = HttpContext.Session.GetString("KeyJobWorkIssueEdit");
            List<JobWorkGridDetail> JobWorkEditGridDetail = new List<JobWorkGridDetail>();
            if (!string.IsNullOrEmpty(modelEditJson))
            {
                JobWorkEditGridDetail = JsonConvert.DeserializeObject<List<JobWorkGridDetail>>(modelEditJson);
            }
            string modelTaxJson = HttpContext.Session.GetString("KeyTaxGrid");
            List<TaxModel> TaxGrid = new List<TaxModel>();
            if (!string.IsNullOrEmpty(modelTaxJson))
            {
                TaxGrid = JsonConvert.DeserializeObject<List<TaxModel>>(modelTaxJson);
            }

            if (TaxGrid != null)
            {
                int ItemCodeCheck = 0;
                if (Mode == "U")
                    ItemCodeCheck = JobWorkEditGridDetail.Where(j => j.SeqForBatch == SeqNo && j.ItemCode==ItemCode && j.BatchNo==BatchNo).Select(j => j.ItemCode).FirstOrDefault();
                else
                    ItemCodeCheck = JobWorkGridDetail.Where(j => j.SeqForBatch == SeqNo && j.ItemCode == ItemCode && j.BatchNo == BatchNo).Select(j => j.ItemCode).FirstOrDefault();
                int TaxItemCode = TaxGrid.Where(t => t.TxItemCode == ItemCodeCheck).Select(t => t.TxItemCode).FirstOrDefault();
                if (TaxItemCode != 0)
                {
                    var ResponseResult = new ResponseResult()
                    {
                        StatusCode = HttpStatusCode.InternalServerError,
                        StatusText = "Error",
                        Result = 1
                    };

                    return PartialView("_JobWorkIssueGrid", ResponseResult);
                }
                else
                {
                    if (Mode == "U")
                    {

                        int Indx = Convert.ToInt32(SeqNo) - 1;

                        if (JobWorkEditGridDetail != null && JobWorkEditGridDetail.Count > 0)
                        {
                            JobWorkEditGridDetail.RemoveAt(Convert.ToInt32(Indx));

                            Indx = 0;
                            foreach (var item in JobWorkEditGridDetail)
                            {
                                Indx++;
                                item.SeqNo = Indx;
                            }
                            MainModel.JobDetailGrid = JobWorkEditGridDetail;

                            MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
                            {
                                AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                                SlidingExpiration = TimeSpan.FromMinutes(55),
                                Size = 1024,
                            };
                            string serializedGrid = JsonConvert.SerializeObject(MainModel.JobDetailGrid);
                            HttpContext.Session.SetString("KeyJobWorkIssueEdit", serializedGrid);

                        }
                    }
                    else
                    {
                        int Indx = Convert.ToInt32(SeqNo) - 1;

                        if (JobWorkGridDetail != null && JobWorkGridDetail.Count > 0)
                        {
                            JobWorkGridDetail.RemoveAt(Convert.ToInt32(Indx));

                            Indx = 0;
                            foreach (var item in JobWorkGridDetail)
                            {
                                Indx++;
                                item.SeqNo = Indx;
                            }
                            MainModel.JobDetailGrid = JobWorkGridDetail;

                            MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
                            {
                                AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                                SlidingExpiration = TimeSpan.FromMinutes(55),
                                Size = 1024,
                            };
                            string serializedGrid = JsonConvert.SerializeObject(MainModel.JobDetailGrid);
                            HttpContext.Session.SetString("KeyJobWorkIssue", serializedGrid);
                        }
                    }
                }
            }
            else
            {
                if (Mode == "U")
                {
                    int Indx = 0;
                    if (Index > 0)
                    {
                        Indx = Index - 1;
                    }
                    else
                    {
                        Indx = Convert.ToInt32(SeqNo) - 1;
                    }

                    if (JobWorkEditGridDetail != null && JobWorkEditGridDetail.Count > 0)
                    {
                        JobWorkEditGridDetail.RemoveAt(Convert.ToInt32(Indx));

                        Indx = 0;

                        foreach (var item in JobWorkEditGridDetail)
                        {
                            Indx++;
                            item.SeqNo = Indx;
                        }
                        MainModel.JobDetailGrid = JobWorkEditGridDetail;

                        string serializedGrid = JsonConvert.SerializeObject(MainModel.JobDetailGrid);
                        HttpContext.Session.SetString("KeyJobWorkIssueEdit", serializedGrid);
                    }
                }
                else
                {
                    int Indx = Convert.ToInt32(SeqNo) - 1;

                    if (JobWorkGridDetail != null && JobWorkGridDetail.Count > 0)
                    {
                        JobWorkGridDetail.RemoveAt(Convert.ToInt32(Indx));

                        Indx = 0;

                        foreach (var item in JobWorkGridDetail)
                        {
                            Indx++;
                            item.SeqNo = Indx;
                        }
                        MainModel.JobDetailGrid = JobWorkGridDetail;

                        string serializedGrid = JsonConvert.SerializeObject(MainModel.JobDetailGrid);
                        HttpContext.Session.SetString("KeyJobWorkIssue", serializedGrid);
                    }
                }
            }


            return PartialView("_JobWorkIssueGrid", MainModel);
        }
        public async Task<JsonResult> EditItemRow(int SeqNo,int ItemCode,string BatchNo,int Index=0)
        {
            var MainModel = new JobWorkIssueModel();
            var _List = new List<JobWorkGridDetail>();
            string modelJson = HttpContext.Session.GetString("KeyJobWorkIssueEdit");
            if (!string.IsNullOrEmpty(modelJson))
            {
                _List = JsonConvert.DeserializeObject<List<JobWorkGridDetail>>(modelJson);
            }
            if (_List == null)
            {
                string modelEditJson = HttpContext.Session.GetString("KeyJobWorkIssue");
                if (!string.IsNullOrEmpty(modelEditJson))
                {
                    _List = JsonConvert.DeserializeObject<List<JobWorkGridDetail>>(modelEditJson);
                }
            }

            var JobGrid = _List.Where(x => x.SeqForBatch == SeqNo && x.ItemCode == ItemCode && x.BatchNo == BatchNo);
            string JsonString = JsonConvert.SerializeObject(JobGrid);
            return Json(JsonString);
        }
        private static DataTable GetDetailTable(IList<JobWorkGridDetail> DetailList, string Mode)
        {
            var JWGrid = new DataTable();

            JWGrid.Columns.Add("Seqno", typeof(int));
            JWGrid.Columns.Add("ItemCode", typeof(int));
            JWGrid.Columns.Add("HSNNO", typeof(string));
            JWGrid.Columns.Add("IssQty", typeof(float));
            JWGrid.Columns.Add("unit", typeof(string));
            JWGrid.Columns.Add("Rate", typeof(float));
            JWGrid.Columns.Add("Amount", typeof(float));
            JWGrid.Columns.Add("RemarkDetail", typeof(string));
            JWGrid.Columns.Add("PurchasePrice", typeof(float));
            JWGrid.Columns.Add("ProcessId", typeof(int));
            JWGrid.Columns.Add("StoreId", typeof(int));
            JWGrid.Columns.Add("BatchNo", typeof(string));
            JWGrid.Columns.Add("uniquebatchno", typeof(string));
            JWGrid.Columns.Add("StockQty", typeof(decimal));
            JWGrid.Columns.Add("BatchStockQty", typeof(decimal));
            JWGrid.Columns.Add("Closed", typeof(string));
            JWGrid.Columns.Add("pendqty", typeof(float));
            JWGrid.Columns.Add("RecSrapCode", typeof(int));
            JWGrid.Columns.Add("RecScrapQty", typeof(float));
            JWGrid.Columns.Add("AltQty", typeof(float));
            JWGrid.Columns.Add("altunit", typeof(string));
            JWGrid.Columns.Add("PendAltQty", typeof(float));
            JWGrid.Columns.Add("PendScrapQty", typeof(float));
            JWGrid.Columns.Add("RecItemCode", typeof(int));
            JWGrid.Columns.Add("RecQTy", typeof(decimal));
            JWGrid.Columns.Add("tolimit", typeof(float));
            JWGrid.Columns.Add("AgainstAdaviceNo", typeof(string));
            JWGrid.Columns.Add("AgainstAdaviceYear", typeof(int));
            JWGrid.Columns.Add("AgainstAdaviceDate", typeof(DateTime));
            JWGrid.Columns.Add("ItemSize", typeof(string));
            JWGrid.Columns.Add("ItemColor", typeof(string));
            JWGrid.Columns.Add("OtherInstruction", typeof(string));
            JWGrid.Columns.Add("PONo", typeof(string));
            JWGrid.Columns.Add("PoYear", typeof(int));
            JWGrid.Columns.Add("PODate", typeof(DateTime));
            JWGrid.Columns.Add("SchNo", typeof(string));
            JWGrid.Columns.Add("SchYear", typeof(int));
            JWGrid.Columns.Add("SchDate", typeof(DateTime));
            JWGrid.Columns.Add("POAmmendNo", typeof(string));

            foreach (var Item in DetailList)
            {
                DateTime poDate = new DateTime();
                DateTime schDate = new DateTime();
                DateTime againstAdaviceDate = new DateTime();
                string poDt = "";
                string schDt = "";
                string againstAdaviceDt = "";
                if (Item.PODate != null)
                {
                    if (Mode == "U")
                    {
                        poDt = Item.PODate;
                    }
                    else
                    {

                        poDate = DateTime.ParseExact(Item.PODate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        poDt = poDate.ToString("yyyy/MM/dd");
                    }
                }
                else
                {
                    poDt = DateTime.Today.ToString();
                }
                if (Item.SchDate != null)
                {
                    if (Mode == "U")
                    {
                        schDt = Item.SchDate;
                    }
                    else
                    {

                        schDate = DateTime.ParseExact(Item.SchDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        schDt = schDate.ToString("yyyy/MM/dd");
                    }
                }
                else
                {
                    schDt = DateTime.Today.ToString();
                }
                if (Item.AgainstAdaviceDate != null)
                {
                    if (Mode == "U")
                    {
                        againstAdaviceDt = Item.AgainstAdaviceDate;
                    }
                    else
                    {
                        againstAdaviceDate = DateTime.ParseExact(Item.AgainstAdaviceDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        againstAdaviceDt = againstAdaviceDate.ToString("yyyy/MM/dd");
                    }
                }
                else
                {
                    againstAdaviceDt = DateTime.Today.ToString();
                }
                JWGrid.Rows.Add(
                    new object[]
                    {
                    Item.SeqForBatch,
                    Item.ItemCode,
                    Item.HSNNo,
                    Item.IssQty,
                    Item.Unit,
                    Item.Rate,
                    Item.Amount,
                    Item.RemarkDetail,
                    Item.PurchasePrice,
                    Item.ProcessId,
                    Item.StoreId,
                    Item.BatchNo,
                    Item.UniqueBatchNo,
                    Item.StockQty,
                    Item.BatchStockQty,
                    Item.Closed,
                    Item.PendQty,
                    Item.RecScrapCode,
                    Item.RecScrapQty,
                    Item.AltQTy,
                    Item.AltUnit ?? "",
                    Item.PendAltQty,
                    Item.PendScrapQty,
                    Item.RecItemCode,
                    Item.RecQty,
                    Item.ToLimit,
                    Item.AgainstAdaviceNo,
                    Item.AgainstAdaviceYear,
                    againstAdaviceDt == "" ? againstAdaviceDate:againstAdaviceDt,
                    Item.ItemSize,
                    Item.ItemColor,
                    Item.OtherInstruction,
                    Item.PONO,
                    Item.POYear,
                    poDt == "" ? poDate : poDt,
                    Item.SchNo,
                    Item.SchYear,
                    schDt == "" ? schDate : schDt,
                    Item.POAAmmendNo
                    });
            }
            JWGrid.Dispose();
            return JWGrid;
        }
        public async Task<JsonResult> GetAccountList(string Check)
        {
            var JSON = await _IDataLogic.GetDropDownList("CREDITORDEBTORLIST", Check, "SP_GetDropDownList");
            _logger.LogError(JsonConvert.SerializeObject(JSON));
            return Json(JSON);
        }
        public async Task<JsonResult> FillItemsBom(string BomStatus, string Types)
        {
            var JSON = await _IJobWorkIssue.FillItemsBom("FillItem", BomStatus, Types);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillPartCodesBom(string BomStatus, string Types)
        {
            var JSON = await _IJobWorkIssue.FillItemsBom("FillPartCode", BomStatus, Types);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillEntryandJWNo(int YearCode)
        {
            var JSON = await _IJobWorkIssue.FillEntryandJWNo(YearCode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetPONOByAccount(int AccountCode, string PONO, int POYear, int ItemCode, string ChallanDate)
        {
            var JSON = await _IJobWorkIssue.GetPONOByAccount("PONOBYACCOUNT", AccountCode, PONO, POYear, ItemCode, ChallanDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }

        private async Task<JobWorkIssueModel> BindModel(JobWorkIssueModel model)
        {
            var oDataSet = new DataSet();
            var _List = new List<TextValue>();
            oDataSet = await _IJobWorkIssue.BindAllDropDowns("BINDALLDROPDOWN");
            model.BranchList = _List;
            model.VendorList = _List;
            model.ItemNameList = _List;
            model.PartCodeList = _List;
            model.ProcessList = _List;
            model.EmployeeList = _List;
            model.StoreList = _List;
            model.RecScrapItemNameList = _List;
            model.RecScrapPartCodeList = _List;
            model.RecItemNameList = _List;
            model.RecPartCodeList = _List;


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
                foreach (DataRow row in oDataSet.Tables[5].Rows)
                {
                    _List.Add(new TextValue
                    {
                        Value = row["Item_Code"].ToString(),
                        Text = row["PartCode"].ToString()
                    });
                }
                model.RecScrapPartCodeList = _List;
                _List = new List<TextValue>();
                foreach (DataRow row in oDataSet.Tables[5].Rows)
                {
                    _List.Add(new TextValue
                    {
                        Value = row["Item_Code"].ToString(),
                        Text = row["ItemName"].ToString()
                    });
                }
                model.RecItemNameList = _List;
                _List = new List<TextValue>();
                foreach (DataRow row in oDataSet.Tables[5].Rows)
                {
                    _List.Add(new TextValue
                    {
                        Value = row["Item_Code"].ToString(),
                        Text = row["PartCode"].ToString()
                    });
                }
                model.RecPartCodeList = _List;
            }
            return model;
        }
        private static DataTable GetTaxDetailTable(List<TaxModel> TaxDetailList)
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

            foreach (TaxModel Item in TaxDetailList)
            {
                Table.Rows.Add(
                    new object[]
                    {
                    Item.TxSeqNo,
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
                    Item.TxRemark,
                    });
            }

            return Table;
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

        public async Task<IActionResult> GetSearchData(string VendorName, string ChallanNo, string ItemName, string PartCode, string FromDate, string ToDate)
        {
            //model.Mode = "Search";
            var model = new JWIssQDashboard();
            model = await _IJobWorkIssue.GetDashboardData(VendorName, ChallanNo, ItemName, PartCode, FromDate, ToDate);
            model.Dashboardtype = "Summary";
            return PartialView("_JobworkDashboardGrid", model);
        }
        public async Task<IActionResult> GetDetailSearchData(string VendorName, string ChallanNo, string ItemName, string PartCode, string FromDate, string ToDate)
        {
            //model.Mode = "Search";
            var model = new JWIssQDashboard();
            model = await _IJobWorkIssue.GetDetailDashboardData(VendorName, ChallanNo, ItemName, PartCode, FromDate, ToDate);
            model.Dashboardtype = "Detail";
            return PartialView("_JobworkDashboardGrid", model);
        }
        public async Task<IActionResult> DeleteByID(int ID, int YC, string FromDate = "", string ToDate = "", string DashboardType = "", string VendorName = "", string ChallanNo = "", string ItemName = "", string PartCode = "")
        {
            var Result = await _IJobWorkIssue.DeleteByID(ID, YC);
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


            return RedirectToAction("JobworkDashboard", new { FromDate = formattedFromDate, ToDate = formattedToDate, Flag = "False", DashboardType = DashboardType, VendorName = VendorName, ChallanNo = ChallanNo, ItemName = ItemName, PartCode = PartCode });
            //return RedirectToAction("JobworkDashboard");
        }
        public async Task<IActionResult> GetItemDetailFromUniqBatch(string UniqBatchNo, int YearCode, string TransDate, string JWChallanNo, int ChallanYearCode, string ChallanDate)
        {
            var MainModel = new JWIssueDashboard();
            try
            {
                ResponseResult StockData = new ResponseResult();
                var ItemDetailData = await _IIssueWOBOM.GetItemDetailFromUniqBatch(UniqBatchNo, YearCode, TransDate);
                if (ItemDetailData.Result != null)
                {
                    if (ItemDetailData.Result.Rows.Count != 0)
                    {
                        StockData = await _IIssueWOBOM.FillLotandTotalStock(Convert.ToInt32(ItemDetailData.Result.Rows[0].ItemArray[4]), 1, TransDate, ItemDetailData.Result.Rows[0].ItemArray[2], UniqBatchNo);
                    }
                    else
                    {
                        return StatusCode(203, "Invalid barcode, item do not exist in this requisition");

                    }
                }
                else
                {
                    return StatusCode(203, "Invalid barcode, item do not exist in this requisition");

                }
                ResponseResult ReqQty = await _IIssueWOBOM.GetReqQtyForScan(JWChallanNo, ChallanYearCode, ChallanDate, Convert.ToInt32(ItemDetailData.Result.Rows[0].ItemArray[4]));

                decimal ReqQuantity = 0;

                if (ReqQty.Result.Rows.Count != 0)
                {
                    ReqQuantity = Convert.ToDecimal(ReqQty.Result.Rows[0].ItemArray[0]);
                }
                else
                {
                    return StatusCode(203, "Invalid barcode this item " + ItemDetailData.Result.Rows[0].ItemArray[0] + " do not exist in this requisition");
                }
                //var t = StockData.
                //string JsonString = JsonConvert.SerializeObject(JSON);

                var ItemList = new List<JobWorkGridDetail>();

                var lotStock = Convert.ToDecimal(StockData.Result.Rows[0].ItemArray[0]);
                var totStock = Convert.ToDecimal(StockData.Result.Rows[0].ItemArray[1]);

                var stock = lotStock <= totStock ? lotStock : totStock;

                var issueQty = stock <= ReqQuantity ? stock : ReqQuantity;

                ItemList.Add(new JobWorkGridDetail
                {
                    ItemName = ItemDetailData.Result.Rows[0].ItemArray[0],
                    PartCode = ItemDetailData.Result.Rows[0].ItemArray[1],
                    ItemCode = Convert.ToInt32(ItemDetailData.Result.Rows[0].ItemArray[4]),
                    BatchNo = ItemDetailData.Result.Rows[0].ItemArray[2],
                    UniqueBatchNo = UniqBatchNo,
                    Unit = ItemDetailData.Result.Rows[0].ItemArray[3],
                    TotalTaxAmt = totStock,
                    IssQty = issueQty,
                    RecQty = ReqQuantity
                });

                var model = ItemList;

                var IssueWithoutBomGrid = new List<JobWorkGridDetail>();
                var IssueGrid = new List<JobWorkGridDetail>();
                var SSGrid = new List<JobWorkGridDetail>();
                MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                    SlidingExpiration = TimeSpan.FromMinutes(55),
                    Size = 1024,
                };
                var seqNo = 1;
                if (model != null)
                {
                    foreach (var item in model)
                    {
                        string modelJson = HttpContext.Session.GetString("KeyIssWOBomScannedGrid");
                        List<JobWorkGridDetail> JobWorkGridDetail = new List<JobWorkGridDetail>();
                        if (!string.IsNullOrEmpty(modelJson))
                        {
                            JobWorkGridDetail = JsonConvert.DeserializeObject<List<JobWorkGridDetail>>(modelJson);
                        }
                        
                        if (item != null)
                        {
                            //if(item.LotStock < item.ReqQty)
                            //{
                            //    return StatusCode(203, "Stock can't be zero");
                            //}
                            if (JobWorkGridDetail == null)
                            {
                                if (item.TotalTaxAmt <= 0)
                                {
                                    return StatusCode(203, "Stock can't be zero");
                                }
                                item.SeqNo += seqNo;
                                IssueGrid.Add(item);
                                seqNo++;
                            }
                            else
                            {
                                if (JobWorkGridDetail.Where(x => x.UniqueBatchNo == item.UniqueBatchNo).Any())
                                {
                                    return StatusCode(207, "Duplicate");
                                }
                                if (item.TotalTaxAmt <= 0)
                                {
                                    return StatusCode(203, "Stock can't be zero");
                                }
                                else
                                {
                                    item.SeqNo = JobWorkGridDetail.Count + 1;
                                    IssueGrid = JobWorkGridDetail.Where(x => x != null).ToList();
                                    SSGrid.AddRange(IssueGrid);
                                    IssueGrid.Add(item);
                                }
                            }
                            MainModel.ItemDetailGrid = IssueGrid;

                            string serializedGrid = JsonConvert.SerializeObject(MainModel.ItemDetailGrid);
                            HttpContext.Session.SetString("KeyIssWOBomScannedGrid", serializedGrid);
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return PartialView("_IssueByScanningGrid", MainModel);
        }
    }
}
