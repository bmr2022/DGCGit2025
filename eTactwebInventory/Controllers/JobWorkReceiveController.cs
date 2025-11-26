using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.DOM.Models.Master;
using eTactWeb.Services.Interface;
using FastReport.Web;
using Grpc.Core;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Net;
using static eTactWeb.Data.Common.CommonFunc;
using static eTactWeb.DOM.Models.Common;
using static eTactWeb.DOM.Models.JobWorkReceiveModel;
using static eTactWeb.DOM.Models.MirModel;

namespace eTactWeb.Controllers
{
    public class JobWorkReceiveController : Controller
    {
        public WebReport webReport;
        private readonly IWebHostEnvironment _IWebHostEnvironment;
        private readonly IConfiguration _iconfiguration;
        private readonly IDataLogic _IDataLogic;
        private readonly IJobWorkReceive _IJobWorkReceive;
        private readonly ILogger<JobWorkReceiveController> _logger;
        private readonly ConnectionStringService _connectionStringService;
        public JobWorkReceiveController(ILogger<JobWorkReceiveController> logger, IDataLogic iDataLogic, IJobWorkReceive iJobWorkReceive, IWebHostEnvironment iWebHostEnvironment, IConfiguration configuration, ConnectionStringService connectionStringService)
        {
            _logger = logger;
            _IDataLogic = iDataLogic;
            _IJobWorkReceive = iJobWorkReceive;
            _IWebHostEnvironment = iWebHostEnvironment;
            _iconfiguration = configuration;
            _connectionStringService = connectionStringService;
        }

        [Route("{controller}/Index")]
        public async Task<IActionResult> JobWorkReceive()
        {
            ViewData["Title"] = "Job Work Receive Details";
            TempData.Clear();
            HttpContext.Session.Remove("KeyJobWorkRecieve");
            HttpContext.Session.Remove("KeyJobWorkRecieveGrid");
            var MainModel = new JobWorkReceiveModel();
            MainModel = await BindModel(MainModel);
            MainModel.FinFromDate = HttpContext.Session.GetString("FromDate");
            MainModel.FinToDate = HttpContext.Session.GetString("ToDate");
            MainModel.YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
            MainModel.CC = HttpContext.Session.GetString("Branch");

            HttpContext.Session.SetString("KeyJobWorkRecieve", JsonConvert.SerializeObject(MainModel));
            HttpContext.Session.SetString("KeyJobWorkRecieveGrid", JsonConvert.SerializeObject(MainModel));
            return View(MainModel);
        }
        [Route("{controller}/Index")]
        [HttpGet]
        public async Task<ActionResult> JobWorkReceive(int ID, string Mode, int YC, string VendorName = "", string ItemName = "", string PartCode = "", string InvoiceNo = "", string MRNNo = "", string GateNo = "", string Branch = "", string FromDate = "", string ToDate = "", string DashboardType = "", string SearchBox = "")//, ILogger logger)
        {
            //_logger.LogInformation("\n \n ********** Page Gate Inward ********** \n \n " + IWebHostEnvironment.EnvironmentName.ToString() + "\n \n");
            TempData.Clear();
            var MainModel = new JobWorkReceiveModel();
            MainModel.FinFromDate = HttpContext.Session.GetString("FromDate");
            MainModel.FinToDate = HttpContext.Session.GetString("ToDate");
            MainModel.YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
            MainModel.CC = HttpContext.Session.GetString("Branch");
            MainModel.CreatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
            MainModel.CreatedByName = HttpContext.Session.GetString("EmpName");

            HttpContext.Session.Remove("KeyJobWorkRecieve");
            HttpContext.Session.Remove("KeyJobWorkRecieveGrid");
            if (!string.IsNullOrEmpty(Mode) && ID > 0 && (Mode == "V" || Mode == "U"))
            {
                MainModel = await _IJobWorkReceive.GetViewByID(ID, YC).ConfigureAwait(false);
                MainModel.Mode = Mode;
                MainModel.ID = ID;
                MainModel.CC = HttpContext.Session.GetString("Branch");
                MainModel.FinFromDate = HttpContext.Session.GetString("FromDate");
                MainModel.FinToDate = HttpContext.Session.GetString("ToDate");
                MainModel.GateDate = ParseDate(MainModel.GateDate).ToString("dd/MM/yyyy");
                MainModel.InvDate = ParseDate(MainModel.InvDate).ToString("dd/MM/yyyy");
                MainModel = await BindModel(MainModel).ConfigureAwait(false);

                HttpContext.Session.SetString("KeyJobWorkRecieveGrid", JsonConvert.SerializeObject(MainModel.JobWorkReceiveGrid));
                HttpContext.Session.SetString("KeyJobWorkRecieve", JsonConvert.SerializeObject(MainModel.ItemDetailGrid));
            }
            else
            {
                MainModel = await BindModel(MainModel);
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
                MainModel.UpdatedByname = HttpContext.Session.GetString("EmpName");
                MainModel.UpdatedOn = DateTime.Now;
            }
            MainModel.VendorNameBack = VendorName;
            MainModel.ItemNameBack = ItemName;
            MainModel.PartCodeBack = PartCode;
            MainModel.InvoiceNoBack = InvoiceNo;
            MainModel.MRNNoBack = MRNNo;
            MainModel.GateNoBack = GateNo;
            MainModel.BranchBack = Branch;
            MainModel.FromDateBack = FromDate;
            MainModel.ToDateBack = ToDate;
            MainModel.DashboardTypeBack = DashboardType;
            MainModel.GlobalSearchBack = SearchBox;
            return View(MainModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("{controller}/Index")]
        public async Task<IActionResult> JobWorkReceive(JobWorkReceiveModel model)
        {
            try
            {
                var JWRGrid = new DataTable();
                var ChallanGrid = new DataTable();
                string modelJson = HttpContext.Session.GetString("KeyJobWorkRecieve");
                List<JobWorkReceiveDetail> JobWorkReceiveDetail = new List<JobWorkReceiveDetail>();
                if (!string.IsNullOrEmpty(modelJson))
                {
                    JobWorkReceiveDetail = JsonConvert.DeserializeObject<List<JobWorkReceiveDetail>>(modelJson);
                }
                string modelReceiveGridJson = HttpContext.Session.GetString("KeyJobWorkRecieveGrid");
                List<JobWorkReceiveItemDetail> JobWorkReceiveItemDetail = new List<JobWorkReceiveItemDetail>();
                if (!string.IsNullOrEmpty(modelReceiveGridJson))
                {
                    JobWorkReceiveItemDetail = JsonConvert.DeserializeObject<List<JobWorkReceiveItemDetail>>(modelReceiveGridJson);
                }
                if (JobWorkReceiveDetail == null && JobWorkReceiveItemDetail == null)
                {
                    ModelState.Clear();
                    ModelState.TryAddModelError("JobWorkReceive", "Grid Should Have Atleast 1 Item...!");
                    model = await BindModel(model);
                    return View("JobWorkReceive", model);
                }

                else
                {
                    model.CC = HttpContext.Session.GetString("Branch");
                    model.ActualEnteredBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                    if (model.Mode == "U")
                    {
                        model.UpdatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                        model.UpdatedByname = HttpContext.Session.GetString("EmpName");
                        JWRGrid = GetJWRTable(JobWorkReceiveItemDetail);
                        ChallanGrid = GetChallanTable(JobWorkReceiveDetail);
                    }
                    else
                    {
                        JWRGrid = GetJWRTable(JobWorkReceiveItemDetail);
                        ChallanGrid = GetChallanTable(JobWorkReceiveDetail);
                    }
                    model = await BindModel(model);
                    var Result = await _IJobWorkReceive.SaveJobReceive(model, JWRGrid, ChallanGrid);
                    if (Result != null)
                    {
                        if (Result.StatusText == "Success" && Result.StatusCode == HttpStatusCode.OK)
                        {
                            ViewBag.isSuccess = true;
                            TempData["200"] = "200";
                            TempData["Message"] = "Data saved successfully.";
                        }
                        if (Result.StatusText == "Success" && Result.StatusCode == HttpStatusCode.Accepted)
                        {
                            ViewBag.isSuccess = true;
                            TempData["202"] = "202";
                            TempData["Message"] = "Data saved successfully.";
                        }
                        if (Result.StatusText == "Error" && Result.StatusCode == HttpStatusCode.InternalServerError)
                        {
                            var errNum = Result.Result.Message.ToString().Split(":")[1];
                            if (errNum == " 2627")
                            {
                                ViewBag.isSuccess = false;
                                TempData["2627"] = "2627";
                                _logger.LogError("\n \n ********** LogError ********** \n " + JsonConvert.SerializeObject(Result) + "\n \n");
                                var model2 = await BindModel(null);
                                model2.FinFromDate = HttpContext.Session.GetString("FromDate");
                                model2.FinToDate = HttpContext.Session.GetString("ToDate");
                                model2.YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
                                model2.CC = HttpContext.Session.GetString("Branch");
                                //model2.PreparedByEmp = HttpContext.Session.GetString("EmpName");
                                model2.ActualEnteredByName = HttpContext.Session.GetString("EmpName");
                                model2.CreatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                                return View(model2);
                            }
                            else
                            {
                                TempData["500"] = "500";
                                var input = "";
                                input = Result.StatusText;
                                int index = input.IndexOf("#ERROR_MESSAGE");

                                if (index != -1)
                                {
                                    int messageStartIndex = index + "#ERROR_MESSAGE".Length; // Remove the extra space and colon
                                    string errorMessage = input.Substring(messageStartIndex).Trim();
                                    int maxLength = 100;
                                    int wrapLength = Math.Min(maxLength, errorMessage.Length);
                                    TempData["ErrorMessage"] = errorMessage.Substring(0, wrapLength);
                                }

                                model = await BindModel(model);
                                model.FinFromDate = HttpContext.Session.GetString("FromDate");
                                model.FinToDate = HttpContext.Session.GetString("ToDate");
                                model.YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
                                model.CC = HttpContext.Session.GetString("Branch");
                                //model.PreparedByEmp = HttpContext.Session.GetString("EmpName");
                                model.ActualEnteredByName = HttpContext.Session.GetString("EmpName");
                                model.CreatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                                model.ItemDetailGrid = JobWorkReceiveDetail;

                                return View(model);
                            }
                        }
                    }
                    var model1 = await BindModel(null);
                    model1.FinFromDate = HttpContext.Session.GetString("FromDate");
                    model1.FinToDate = HttpContext.Session.GetString("ToDate");
                    model1.YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
                    model1.CC = HttpContext.Session.GetString("Branch");
                    model1.ActualEnteredByName = HttpContext.Session.GetString("EmpName");
                    model1.CreatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                    return RedirectToAction(nameof(JobWorkReceive));
                    //return View(model);
                }
            }
            catch (Exception ex)
            {
                LogException<JobWorkReceiveController>.WriteException(_logger, ex);

                TempData["ErrorMessage"] = ex.Message;
                var ResponseResult = new ResponseResult()
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    StatusText = "Error",
                    Result = ex
                };

                return View("Error", ResponseResult);
            }
        }
        public async Task<JsonResult> GetFormRights()
        {
            var userID = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
            var JSON = await _IJobWorkReceive.GetFormRights(userID);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public IActionResult PrintReport(string MRNNo = "", int YearCode = 0, string PONO = "")
        {

            string my_connection_string;
            string contentRootPath = _IWebHostEnvironment.ContentRootPath;
            string webRootPath = _IWebHostEnvironment.WebRootPath;
            webReport = new WebReport();

            webReport.Report.Load(webRootPath + "\\jobworkMRN.frx"); // default report
            //my_connection_string = _iconfiguration.GetConnectionString("eTactDB");
            my_connection_string = _connectionStringService.GetConnectionString();
            webReport.Report.Dictionary.Connections[0].ConnectionString = my_connection_string;
            webReport.Report.Dictionary.Connections[0].ConnectionStringExpression = "";
            webReport.Report.SetParameterValue("mrnnoparam", MRNNo);
            webReport.Report.SetParameterValue("yearcodeparam", YearCode);
            webReport.Report.SetParameterValue("MyParameter", my_connection_string);
            webReport.Report.Refresh();
            return View(webReport);
        }
        public async Task<IActionResult> JWRDashboard(string FromDate, string Todate, string Flag, string DeleteFlag = "True", string VendorName = "", string ItemName = "", string PartCode = "", string InvNo = "", string MRNNo = "", string GateNo = "", string CC = "", string DashboardType = "")
        {
            try
            {
                HttpContext.Session.Remove("KeyJobWorkRecieve");
                HttpContext.Session.Remove("KeyJobWorkRecieveGrid");
                var model = new JWRecQDashboard();
                var Result = await _IJobWorkReceive.GetDashboardData(FromDate, Todate, Flag).ConfigureAwait(true);

                if (Result != null)
                {
                    var _List = new List<TextValue>();
                    DataSet DS = Result.Result;
                    if (DS != null)
                    {
                        var DT = DS.Tables[0].DefaultView.ToTable(false, "VendorName", "GateNo", "GateDAte", "MRNNo",
                            "MRNDate", "InvNo", "InvDate", "QCCompleted", "TotalAmt", "NetAmt", "PurchaseBillPosted", "ENtryId", "YearCode", "GateYearCode", "BranchName", "EnteredBy", "UpdatedBy");
                        model.JWRecQDashboard = CommonFunc.DataTableToList<JWReceiveDashboard>(DT, "RECVendJW");
                    }
                }

                if (DeleteFlag != "True")
                {
                    model.FromDate1 = FromDate;
                    model.ToDate1 = Todate;
                    //model.Dashboardtype = DashboardType;
                    model.VendorName = VendorName;
                    model.PartCode = PartCode;
                    model.ItemName = ItemName;
                    model.GateNo = GateNo;
                    model.MRNNo = MRNNo;
                    model.InvNo = InvNo;
                    model.CC = CC;
                    model.DashboardType = DashboardType;
                    return View(model);


                }

                if (Flag == "True")
                    return View(model);
                else
                    return PartialView("_JobWorkRecDashboardGrid", model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IActionResult> DeleteByID(int ID, int YC, string FromDate = "", string ToDate = "", string VendorName = "", string ItemName = "", string PartCode = "", string InvNo = "", string MRNNo = "", string GateNo = "", string CC = "", string DashboardType = "")
        {
            var Result = await _IJobWorkReceive.DeleteByID(ID, YC);

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

            return RedirectToAction("JWRDashboard", new { FromDate = formattedFromDate, Todate = formattedToDate, Flag = "False", DeleteFlag = "False", VendorName = VendorName, ItemName = ItemName, PartCode = PartCode, InvNo = InvNo, MRNNo = MRNNo, GateNo = GateNo, CC = CC, DashboardType = DashboardType });
        }
        public async Task<IActionResult> GetSearchData(string VendorName, string MRNNo, string GateNo, string PartCode, string ItemName, string BranchName, string InvNo, string Fromdate, string Todate)
        {
            //model.Mode = "Search";
            var model = new JWReceiveDashboard();
            model = await _IJobWorkReceive.GetDashboardData(VendorName, MRNNo, GateNo, PartCode, ItemName, BranchName, InvNo, Fromdate, Todate);
            model.SearchMode = "Summary";
            return PartialView("_JobWorkRecDashboardGrid", model);
        }
        public async Task<IActionResult> GetDetailData(string VendorName, string MRNNo, string GateNo, string PartCode, string ItemName, string BranchName, string InvNo, string Fromdate, string Todate)
        {
            //model.Mode = "Search";
            var model = new JWReceiveDashboard();
            model = await _IJobWorkReceive.GetDetailDashboardData(VendorName, MRNNo, GateNo, PartCode, ItemName, BranchName, InvNo, Fromdate, Todate);
            model.SearchMode = "Detail";
            return PartialView("_JobWorkRecDashboardGrid", model);
        }
        private static DataTable GetJWRTable(IList<JobWorkReceiveItemDetail> DetailList)
        {
            var JWRGRid = new DataTable();
            JWRGRid.Columns.Add("SeqNo", typeof(int));
            JWRGRid.Columns.Add("ItemCode", typeof(int));
            JWRGRid.Columns.Add("Unit", typeof(string));
            JWRGRid.Columns.Add("BillQty", typeof(decimal));
            JWRGRid.Columns.Add("RecQty", typeof(decimal));
            JWRGRid.Columns.Add("JWRate", typeof(decimal));
            JWRGRid.Columns.Add("Amount", typeof(decimal));
            JWRGRid.Columns.Add("Remark", typeof(string));
            JWRGRid.Columns.Add("ProducedUnprod", typeof(string));
            JWRGRid.Columns.Add("ProcessId", typeof(int));
            JWRGRid.Columns.Add("Adjusted", typeof(string));
            JWRGRid.Columns.Add("BomRevNo", typeof(int));
            JWRGRid.Columns.Add("BOMRevDate", typeof(string));
            JWRGRid.Columns.Add("NoOfCase", typeof(decimal));
            JWRGRid.Columns.Add("QCCompleted", typeof(string));
            JWRGRid.Columns.Add("PONo", typeof(string));
            JWRGRid.Columns.Add("POYearCode", typeof(int));
            JWRGRid.Columns.Add("PODate", typeof(string));
            JWRGRid.Columns.Add("SchNo", typeof(string));
            JWRGRid.Columns.Add("SchYearCode", typeof(int));
            JWRGRid.Columns.Add("SchDate", typeof(string));
            JWRGRid.Columns.Add("POType", typeof(string));
            JWRGRid.Columns.Add("BOMINd", typeof(string));
            JWRGRid.Columns.Add("BatchNo", typeof(string));
            JWRGRid.Columns.Add("Uniquebatchno", typeof(string));
            JWRGRid.Columns.Add("batchwise", typeof(string));
            JWRGRid.Columns.Add("JWRateUnit", typeof(string));
            JWRGRid.Columns.Add("ProcessUnitQty", typeof(decimal));
            JWRGRid.Columns.Add("ProcessUnit", typeof(string));
            JWRGRid.Columns.Add("POAmendNo", typeof(int));
            JWRGRid.Columns.Add("PartCode", typeof(string));
            JWRGRid.Columns.Add("TotalPrice", typeof(decimal));

            foreach (var Item in DetailList)
            {
                JWRGRid.Rows.Add(
                    new object[]
                    {
                    Item.SeqNo,
                    Item.ItemCode,
                    Item.Unit == null ? "":Item.Unit,
                    Item.BillQty,
                    Item.RecQty,
                    Item.JWRate,
                    Item.Amount,
                    Item.Remark == null ? "":Item.Remark,
                    Item.ProducedUnprod == null ? "":Item.ProducedUnprod,
                    Item.ProcessId,
                    Item.Adjusted == null ? "":Item.Adjusted,
                    Item.BomRevNo,
                    Item.BomRevDate == null ? string.Empty : ParseFormattedDate(Item.BomRevDate),
                    Item.NoOfCase,
                    Item.QCCompleted == null ? "":Item.QCCompleted,
                    Item.PONO == null ? string.Empty : Item.PONO, // Pono,
                    Item.POYearCode == 0 ? 2024 : Item.POYearCode,
                    Item.PODate == null ? string.Empty : ParseFormattedDate(Item.PODate),
                    Item.SchNo == null ? string.Empty : Item.SchNo, //SchNo
                    Item.SchYearCode == "0" ? "2024" : Item.SchYearCode,
                    Item.SchDate == null ? string.Empty : ParseFormattedDate(Item.SchDate),
                    Item.POType,
                    Item.BomInd == null ? "":Item.BomInd,
                    Item.BatchNo == null ? "":Item.BatchNo,
                    Item.UniqueBatchNo == null ? "":Item.UniqueBatchNo,
                    Item.BatchWise == null ? "":Item.BatchWise,
                    Item.JWRateUnit == null ? "":Item.JWRateUnit,
                    Item.ProcessUnitQty,
                    Item.ProcessUnit,
                    Item.POAmendNo,
                    Item.PartCode == null ? "":Item.PartCode,
                    100
                    });
            }
            JWRGRid.Dispose();
            return JWRGRid;
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
        private static DataTable GetChallanTable(IList<JobWorkReceiveDetail> DetailList)
        {
            var ChallanGrid = new DataTable();
            var todate = (DateTime.Today).ToString();
            //ChallanGrid.Columns.Add("SeqNo", typeof(int));
            ChallanGrid.Columns.Add("EntryDate", typeof(string));
            ChallanGrid.Columns.Add("EntryIdIssJw", typeof(int));
            ChallanGrid.Columns.Add("YearCodeIssJw", typeof(int));
            ChallanGrid.Columns.Add("IssChallanNo", typeof(string));
            ChallanGrid.Columns.Add("IssChallanDate", typeof(string));
            ChallanGrid.Columns.Add("Itemcode", typeof(int));
            ChallanGrid.Columns.Add("EntryIdRecJw", typeof(int));
            ChallanGrid.Columns.Add("YearCodeRecJw", typeof(int));
            ChallanGrid.Columns.Add("RecChallanNo", typeof(string));
            ChallanGrid.Columns.Add("RecChallanDate", typeof(string));
            ChallanGrid.Columns.Add("FinishItemCode", typeof(int));
            ChallanGrid.Columns.Add("AccountCode", typeof(int));
            ChallanGrid.Columns.Add("AdjQty", typeof(float));
            ChallanGrid.Columns.Add("CC", typeof(string));
            ChallanGrid.Columns.Add("AdjFormType", typeof(string));
            ChallanGrid.Columns.Add("TillDate", typeof(string));
            ChallanGrid.Columns.Add("TotRecQty", typeof(decimal));
            ChallanGrid.Columns.Add("PendQty", typeof(decimal));
            ChallanGrid.Columns.Add("BOMQty", typeof(decimal));
            ChallanGrid.Columns.Add("BomRevNo", typeof(int));
            ChallanGrid.Columns.Add("BOMRevDate", typeof(string));
            ChallanGrid.Columns.Add("ProcessID", typeof(int));
            ChallanGrid.Columns.Add("BOMInd", typeof(string));
            ChallanGrid.Columns.Add("RecQty", typeof(decimal));
            ChallanGrid.Columns.Add("TotadjQty", typeof(decimal));
            ChallanGrid.Columns.Add("TotalRecQty", typeof(decimal));
            ChallanGrid.Columns.Add("TotalIssuedQty", typeof(decimal));
            ChallanGrid.Columns.Add("RunnerItemCode", typeof(int));
            ChallanGrid.Columns.Add("ScrapItemCode", typeof(int));
            ChallanGrid.Columns.Add("IdealScrapQty", typeof(float));
            ChallanGrid.Columns.Add("IssuedScrapQty", typeof(float));
            ChallanGrid.Columns.Add("PreRecChallanNo", typeof(string));
            ChallanGrid.Columns.Add("ScrapqtyagainstRcvqty", typeof(float));
            ChallanGrid.Columns.Add("Issuedbatchno", typeof(string));
            ChallanGrid.Columns.Add("Issueduniquebatchno", typeof(string));
            ChallanGrid.Columns.Add("Recbatchno", typeof(string));
            ChallanGrid.Columns.Add("Recuniquebatchno", typeof(string));
            ChallanGrid.Columns.Add("ScrapAdjusted", typeof(string));

            foreach (var Item in DetailList)
            {
                ChallanGrid.Rows.Add(
                    new object[]
                    {
                  ParseFormattedDate(todate),
                    Item.EntryIdIssJw,
                    Item.YearCodeIssJw, //Item.IssYearCode,
                    Item.IssChallanNo ?? "",
                    ParseFormattedDate(todate),
                    Item.ItemCode,
                    Item.EntryIdRecJw,
                    Item.YearCodeRecJw,
                    Item.PreRecChallanNo ?? "",
                 ParseFormattedDate(todate),
                    Item.FinishItemCode,
                    Item.AccountCode,
                    Item.AdjQty,
                    Item.CC ?? "",
                    Item.AdjFormType ?? "",
                   ParseFormattedDate(todate),
                    Item.TotalRecQty,
                    Item.PendQty,
                    Item.BOMQty,
                    Item.BOMrevno,
                    ParseFormattedDate(todate),
                    Item.ProcessId,
                    Item.BOMInd ?? "",
                    Item.RecQty,
                    Item.TotaladjQty,
                    Item.TotalRecQty,
                    Item.TotalIssuedQty,
                    Item.RunnerItemCode,
                    Item.ScrapItemCode,
                    Item.IdealScrapQty,
                    Item.IssuedScrapQty,
                    Item.PreRecChallanNo ?? "",
                    Item.ScrapqtyagainstRcvQty,
                    Item.IssuedBatchNO ?? "",
                    Item.IssuedUniqueBatchNo ?? "",
                    Item.Recbatchno ?? "",
                    Item.Recuniquebatchno ?? "",
                    Item.ScrapAdjusted ?? "",
                    });
            }
            ChallanGrid.Dispose();
            return ChallanGrid;
        }
        private static DataTable GetAdjustedChallanTable(List<ChallanDetail> DTTItemGrid)
        {
            try
            {
                var ChallanGrid = new DataTable();
                //ChallanGrid.Columns.Add("SeqNo", typeof(int));
                ChallanGrid.Columns.Add("ItemCode", typeof(int));
                ChallanGrid.Columns.Add("Unit", typeof(string));
                ChallanGrid.Columns.Add("BillQty", typeof(float));
                ChallanGrid.Columns.Add("RecQty", typeof(float));
                ChallanGrid.Columns.Add("JWRate", typeof(float));
                ChallanGrid.Columns.Add("ProcessId", typeof(int));
                ChallanGrid.Columns.Add("PONO", typeof(string));
                ChallanGrid.Columns.Add("POYearCode", typeof(int));
                ChallanGrid.Columns.Add("SchNo", typeof(string));
                ChallanGrid.Columns.Add("SchYearCode", typeof(int));
                ChallanGrid.Columns.Add("BOMIND", typeof(string));
                ChallanGrid.Columns.Add("BOMNO", typeof(int));
                ChallanGrid.Columns.Add("BOMEffDate", typeof(string));
                ChallanGrid.Columns.Add("Produnprod", typeof(string));

                foreach (var Item in DTTItemGrid)
                {
                    ChallanGrid.Rows.Add(
                        new object[]
                        {
                    Item.ItemCode,
                    Item.Unit,
                    Item.BillQty,
                    Item.RecQty,
                    Item.JWRate,
                    Item.ProcessId,
                    Item.PONO,
                    Item.POYearCode,
                    Item.SchNo,
                    Item.SchYearCode,
                    Item.BOMInd ?? string.Empty,
                    Item.BOMrevno,
                    ParseFormattedDate(Item.BOMRevDate),
                    Item.ProdUpProd
                        });
                }
                ChallanGrid.Dispose();
                return ChallanGrid;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<JsonResult> GetGateNo(string FromDate, string ToDate)
        {
            var JSON = await _IJobWorkReceive.GetGateNo("PENDINGGATEFORMRN", "SP_JobworkRec", FromDate, ToDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> CheckQtyBeforeInsertOrUpdate(string TypesBOMIND)
        {
            var JWRGrid = new DataTable();
            var ChallanGrid = new DataTable();
            string serializedGrid = HttpContext.Session.GetString("KeyJobWorkRecieve");
            List<JobWorkReceiveDetail> JobWorkReceiveDetail = new List<JobWorkReceiveDetail>();
            if (!string.IsNullOrEmpty(serializedGrid))
            {
                JobWorkReceiveDetail = JsonConvert.DeserializeObject<List<JobWorkReceiveDetail>>(serializedGrid);
            }
            string modelReceiveGridJson = HttpContext.Session.GetString("KeyJobWorkRecieveGrid");
            List<JobWorkReceiveItemDetail> JobWorkReceiveItemDetail = new List<JobWorkReceiveItemDetail>();
            if (!string.IsNullOrEmpty(modelReceiveGridJson))
            {
                JobWorkReceiveItemDetail = JsonConvert.DeserializeObject<List<JobWorkReceiveItemDetail>>(modelReceiveGridJson);
            }

            JWRGrid = GetJWRTable(JobWorkReceiveItemDetail);
            ChallanGrid = GetChallanTable(JobWorkReceiveDetail);
            var ChechedData = await _IJobWorkReceive.CheckQtyBeforeInsertOrUpdate(TypesBOMIND,JWRGrid, ChallanGrid);
            if (ChechedData.StatusCode == HttpStatusCode.OK && ChechedData.StatusText == "Success")
            {
                DataTable dt = ChechedData.Result;

                List<string> errorMessages = new List<string>();
                JsonConvert.SerializeObject(ChechedData);
                foreach (DataRow row in dt.Rows)
                {
                    string itemName = row["Item_Name"].ToString();
                    decimal availableQty = 0;

                    try
                    {
                        if (row["AdjQty"] != DBNull.Value && !string.IsNullOrWhiteSpace(row["AdjQty"].ToString()))
                            availableQty = Convert.ToDecimal(row["AdjQty"]);
                    }
                    catch
                    {
                        availableQty = 0; // if column doesn't exist or conversion fails
                    }
                 //   decimal availableQty = Convert.ToDecimal(row["AdjQty"]);

                    string error = $"{itemName}  has only {availableQty} quantity available in stock.";
                    errorMessages.Add(error);
                }

                return Json(new
                {
                    success = false,
                    errors = errorMessages
                });
            }
            return Json(new
            {
                success = true,
                message = "No errors found."
            });
        }
        public async Task<JsonResult> GetEmployeeList()
        {
            var JSON = await _IJobWorkReceive.GetEmployeeList("EMPLIST", "SP_JobworkRec");
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> ClearGridAjax()
        {
            HttpContext.Session.Remove("KeyJobWorkRecieve");
            HttpContext.Session.Remove("KeyJobWorkRecieveGrid");
            return Json("done");
        }
        public async Task<JsonResult> GetBomRevNo(int ItemCode)
        {
            var JSON = await _IJobWorkReceive.GetBomRevNo(ItemCode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> ViewDetailSection(int yearCode, int entryId)
        {
            var JSON = await _IJobWorkReceive.ViewDetailSection(yearCode, entryId);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetGateMainData(string GateNo, string GateYearCode, int GateEntryId)
        {
            var JSON = await _IJobWorkReceive.GetGateMainData("GATEMAINDATA", "SP_JobworkRec", GateNo, GateYearCode, GateEntryId);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> DisplayBomDetail(int ItemCode, float WOQty, int BomRevNo)
        {
            var JSON = await _IJobWorkReceive.DisplayBomDetail(ItemCode, WOQty, BomRevNo);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetGateItemData(string GateNo, string GateYearCode, int GateEntryId)
        {
            var JSON = await _IJobWorkReceive.GetGateItemData("GATEMAINITEM", "SP_JobworkRec", GateNo, GateYearCode, GateEntryId);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetPopUpChallanData(int AccountCode, int YearCode, string FromDate, string ToDate, int RecItemCode, int BomRevNo, string BomRevDate, string BOMIND, string BillChallanDate, string JobType, string ProdUnProd)
        {
            var JSON = await _IJobWorkReceive.GetPopUpChallanData(AccountCode, YearCode, FromDate, ToDate, RecItemCode, BomRevNo, BomRevDate, BOMIND, BillChallanDate, JobType, ProdUnProd);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetBomValidated(int RecItemCode, int BomRevNo, string BomRevDate, int RecQty)
        {
            var JSON = await _IJobWorkReceive.GetBomValidated(RecItemCode, BomRevNo, BomRevDate, RecQty);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetPopUpData(int AccountCode, int IssYear, string FinYearFromDate, string billchallandate, string prodUnProd, string BOMINd, int RMItemCode, string RMPartcode, string RMItemNAme, string ACCOUNTNAME, int Processid)
        {
            var JSON = await _IJobWorkReceive.GetPopUpData("JOBWORKISSUESUMMARY", AccountCode, IssYear, FinYearFromDate, billchallandate, prodUnProd, BOMINd, RMItemCode, RMPartcode, RMItemNAme, ACCOUNTNAME, Processid);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }

        [HttpPost]
        public async Task<JsonResult> GetAdjustedChallan(string DTTItemGrid, int AccountCode, int YearCode, string FinYearFromDate, string billchallandate, string GateNo, int GateYearCode)
        {
            try
            {
                var ChallanGrid = new DataTable();
                List<ChallanDetail> Challandetails = JsonConvert.DeserializeObject<List<ChallanDetail>>(DTTItemGrid);
                ChallanGrid = GetAdjustedChallanTable(Challandetails);
                var JSON = await _IJobWorkReceive.GetAdjustedChallan(AccountCode, YearCode, FinYearFromDate, billchallandate, GateNo, GateYearCode, ChallanGrid);
                string JsonString = JsonConvert.SerializeObject(JSON);
                List<JobWorkReceiveDetail> JobWorkReceiveDetail = new List<JobWorkReceiveDetail>();

                foreach (DataRow row in JSON.Result.Rows)
                {
                    JobWorkReceiveDetail jobWorkRec = new JobWorkReceiveDetail
                    {
                        EntryIdIssJw = row["EntryIdIssJw"] != DBNull.Value ? Convert.ToInt32(row["EntryIdIssJw"]) : 0,
                        IssChallanNo = row["IssJWChallanNo"]?.ToString(),
                        IssYearCode = row["IssYearCode"] != DBNull.Value ? Convert.ToInt32(row["IssYearCode"]) : 0,
                        IssChallanDate = row["ChallanDate"]?.ToString(),
                        IssPartCode = row["IssPartCode"]?.ToString(),
                        IssItemName = row["IssItemName"]?.ToString(),
                        ItemCode = row["IssItemCode"] != DBNull.Value ? Convert.ToInt32(row["IssItemCode"]) : 0,
                        BOMrevno = row["BomNo"] != DBNull.Value ? Convert.ToInt32(row["BomNo"]) : 0,
                        BOMRevDate = row["BOMDate"]?.ToString(),
                        BOMInd = row["BomStatus"]?.ToString(),
                        PendQty = row["PendQty"] != DBNull.Value ? Convert.ToDecimal(row["PendQty"]) : 0,
                        FinishPartCode = row["FinishPartcode"]?.ToString(),
                        FinishItemName = row["FinishItemName"]?.ToString(),
                        FinishItemCode = row["RecItemCode"] != DBNull.Value ? Convert.ToInt32(row["RecItemCode"]) : 0,
                        BOMQty = row["bomqty"] != DBNull.Value ? Convert.ToDecimal(row["bomqty"]) : 0,

                        AdjQty = row["ActualAdjQty"] != DBNull.Value ? Convert.ToDecimal(row["ActualAdjQty"]) : 0,
                        Through = row["through"]?.ToString(),
                        //TotaladjQty = Convert.ToDecimal(row["QtyToBeRec"]),
                        TotaladjQty = row["ActualAdjQty"] != DBNull.Value ? Convert.ToDecimal(row["ActualAdjQty"]) : 0,
                        TotalIssuedQty = row["ActualAdjQty"] != DBNull.Value ? Convert.ToDecimal(row["ActualAdjQty"]) : 0,
                        IssuedBatchNO = row["batchno"]?.ToString(),
                        IssuedUniqueBatchNo = row["uniquebatchno"]?.ToString(),
                        RecQty = row["RecQty"] != DBNull.Value ? Convert.ToDecimal(row["RecQty"]) : 0,
                        TotalRecQty = row["RecQty"] != DBNull.Value ? Convert.ToDecimal(row["RecQty"]) : 0,
                        YearCodeIssJw = row["IssYearCode"] != null ? Convert.ToInt32(row["IssYearCode"]) : 0
                    };

                    JobWorkReceiveDetail.Add(jobWorkRec);
                }
                var dataresult = AddChallanDetail2Grid(JobWorkReceiveDetail);

                return Json(JsonString);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<JsonResult> GetProcessList()
        {
            var JSON = await _IJobWorkReceive.GetProcessList("PROCESSLIST", "SP_JobworkRec");
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> CheckEditOrDelete(string MRNNo, int YearCode)
        {
            var JSON = await _IJobWorkReceive.CheckEditOrDelete(MRNNo, YearCode);
            _logger.LogError(JsonConvert.SerializeObject(JSON));
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetProcessUnit()
        {
            var JSON = await _IJobWorkReceive.GetProcessUnit("PROCESSUNITLIST", "SP_JobworkRec");
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetFeatureOption()
        {
            var JSON = await _IJobWorkReceive.GetFeatureOption("FeatureOption", "SP_JobworkRec");
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public IActionResult AddChallanDetail2Grid(List<JobWorkReceiveDetail> model)
        {
            try
            {
                var MainModel = new JobWorkReceiveModel();
                var JobWorkReceiveGrid = new List<JobWorkReceiveDetail>();
                var JobReceiveGrid = new List<JobWorkReceiveDetail>();
                var SSGrid = new List<JobWorkReceiveDetail>();
                MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                    SlidingExpiration = TimeSpan.FromMinutes(55),
                    Size = 1024,
                };
                var seqNo = 0;
                foreach (var item in model)
                {
                    string modelJson = HttpContext.Session.GetString("KeyJobWorkRecieve");
                    List<JobWorkReceiveDetail> JobWorkReceiveDetail = new List<JobWorkReceiveDetail>();
                    if (modelJson != null)
                    {
                        JobWorkReceiveDetail = JsonConvert.DeserializeObject<List<JobWorkReceiveDetail>>(modelJson);
                    }
                    if (item != null)
                    {
                        if (JobWorkReceiveDetail == null)
                        {
                            item.SeqNo += seqNo + 1;
                            JobWorkReceiveGrid.Add(item);
                            seqNo++;
                        }
                        else
                        {
                            item.SeqNo = JobWorkReceiveDetail.Count + 1;
                            //JobWorkReceiveGrid = JobWorkReceiveDetail.Where(x => x != null).ToList();
                            SSGrid.AddRange(JobWorkReceiveGrid);
                            JobWorkReceiveGrid.Add(item);
                        }
                        MainModel.ItemDetailGrid = JobWorkReceiveGrid;

                        //HttpContext.Session.SetString("KeyJobWorkRecieve", JsonConvert.SerializeObject(MainModel.ItemDetailGrid));
                        //HttpContext.Session.SetString("KeyJobWorkRecieve", JsonConvert.SerializeObject(JobWorkReceiveGrid));
                    }
                }
                HttpContext.Session.SetString("KeyJobWorkRecieve", JsonConvert.SerializeObject(MainModel.ItemDetailGrid));
                Console.WriteLine("Session Data: " + HttpContext.Session.GetString("KeyJobWorkRecieve"));
                Console.WriteLine("Session Data: " + HttpContext.Session.GetString("KeyJobWorkRecieveGrid"));
                return PartialView("_JobWorkReceiveGrid", MainModel);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IActionResult AddJobReceiveGrid(JobWorkReceiveItemDetail model)
        {
            try
            {
                string modelJson = HttpContext.Session.GetString("KeyJobWorkRecieveGrid");
                List<JobWorkReceiveItemDetail> JobWorkReceiveItemDetail = new List<JobWorkReceiveItemDetail>();
                if (modelJson != null)
                {
                    JobWorkReceiveItemDetail = JsonConvert.DeserializeObject<List<JobWorkReceiveItemDetail>>(modelJson);
                }

                var MainModel = new JobWorkReceiveModel();
                var JobWorkReceiveGrid = new List<JobWorkReceiveItemDetail>();
                var JobReceiveGrid = new List<JobWorkReceiveItemDetail>();
                var SSGrid = new List<JobWorkReceiveItemDetail>();

                var seqNo = 0;
                if (model != null)
                {
                    if (JobWorkReceiveItemDetail == null)
                    {
                        model.SeqNo += seqNo + 1;
                        JobWorkReceiveGrid.Add(model);
                        seqNo++;
                    }
                    else
                    {
                        model.SeqNo = JobWorkReceiveItemDetail.Count + 1;
                        //JobWorkReceiveGrid = JobWorkReceiveItemDetail.Where(x => x != null).ToList();
                        SSGrid.AddRange(JobWorkReceiveGrid);
                        JobWorkReceiveGrid.Add(model);
                    }
                    MainModel.JobWorkReceiveGrid = JobWorkReceiveGrid;

                    HttpContext.Session.SetString("KeyJobWorkRecieveGrid", JsonConvert.SerializeObject(MainModel.JobWorkReceiveGrid));
                }

                return PartialView("_JobWorkReceiveGrid", MainModel);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IActionResult JWRChallanList(List<JobWorkReceiveItemDetail> model)
        {
            try
            {

                var MainModel1 = new JobWorkReceiveModel();
                var JobWorkReceiveGrid1 = new List<JobWorkReceiveItemDetail>();
                var JobReceiveGrid1 = new List<JobWorkReceiveItemDetail>();
                var SSGrid1 = new List<JobWorkReceiveItemDetail>();
                var seqNo = 0;
                foreach (var item in model)
                {
                    string modelJson = HttpContext.Session.GetString("KeyJobWorkRecieveGrid");
                    List<JobWorkReceiveItemDetail> JobWorkReceiveItemDetail = new List<JobWorkReceiveItemDetail>();
                    if (modelJson != null)
                    {
                        JobWorkReceiveItemDetail = JsonConvert.DeserializeObject<List<JobWorkReceiveItemDetail>>(modelJson);
                    }
                    if (item != null)
                    {
                        if (JobWorkReceiveItemDetail == null)
                        {
                            item.SeqNo += seqNo + 1;
                            JobWorkReceiveGrid1.Add(item);
                            seqNo++;
                        }
                        else
                        {
                            item.SeqNo = JobWorkReceiveItemDetail.Count + 1;
                            //JobWorkReceiveGrid = JobWorkReceiveItemDetail.Where(x => x != null).ToList();
                            SSGrid1.AddRange(JobWorkReceiveGrid1);
                            JobWorkReceiveGrid1.Add(item);
                        }
                        MainModel1.JobWorkReceiveGrid = JobWorkReceiveGrid1;

                        //HttpContext.Session.SetString("KeyJobWorkRecieveGrid", JsonConvert.SerializeObject(MainModel1.JobWorkReceiveGrid));
                        //HttpContext.Session.SetString("KeyJobWorkRecieveGrid", JsonConvert.SerializeObject(model));
                    }
                }
                HttpContext.Session.SetString("KeyJobWorkRecieveGrid", JsonConvert.SerializeObject(MainModel1.JobWorkReceiveGrid));
                return PartialView("_JobWorkReceiveGrid", MainModel1);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private async Task<JobWorkReceiveModel> BindModel(JobWorkReceiveModel model)
        {
            if (model == null)
            {
                model = new JobWorkReceiveModel();

                model.YearCode = Constants.FinincialYear;
                model.EntryId = _IDataLogic.GetEntryID("JobworkReceivemain", Constants.FinincialYear, "GateEntryID", "Gateyearcode");
            }
            var oDataSet = new DataSet();
            var _List = new List<TextValue>();
            oDataSet = await _IJobWorkReceive.BindBranch("BINDBRANCH");

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
            }
            return model;
        }
        public async Task<JsonResult> FillNewEntry(int YearCode)
        {
            var JSON = await _IJobWorkReceive.FillNewEntry(YearCode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
    }
}
