using eTactWeb.Data.Common;
using FastReport;
using FastReport.Export.Html;
using FastReport.Export.Image;
using FastReport.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using static eTactWeb.Data.Common.CommonFunc;
using static eTactWeb.DOM.Models.Common;
using static Grpc.Core.Metadata;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using System.Data;
using System.Net;
using Microsoft.Extensions.Configuration;
using System.Configuration;
using ClosedXML.Excel;

namespace eTactWeb.Controllers
{
    public class ReceiveChallanController : Controller
    {
        public IDataLogic IDataLogic { get; }
        public IReceiveChallan IReceiveChallan { get; }
        public IWebHostEnvironment IWebHostEnvironment { get; }
        public ILogger<ReceiveChallanController> Logger { get; }
        private EncryptDecrypt EncryptDecrypt { get; }
        private readonly IConfiguration iconfiguration;
        private readonly ConnectionStringService _connectionStringService;
        public ReceiveChallanController(IReceiveChallan iReceiveChallan, IConfiguration configuration, IDataLogic iDataLogic, ILogger<ReceiveChallanController> logger, EncryptDecrypt encryptDecrypt, IWebHostEnvironment iWebHostEnvironment, ConnectionStringService connectionStringService)
        {
            IReceiveChallan = iReceiveChallan;
            IDataLogic = iDataLogic;
            Logger = logger;
            EncryptDecrypt = encryptDecrypt;
            IWebHostEnvironment = iWebHostEnvironment;
            iconfiguration = configuration;
            _connectionStringService = connectionStringService;
        }

        [HttpGet]
        [Route("{controller}/Index")]
        public async Task<IActionResult> ReceiveChallan(int ID, string Mode, int YC, string FromDate = "", string ToDate = "", string Account_Name = "", string GateNo= "", string PartCode = "", string ItemName = "", string ChallanNo = "", string SummaryDetail = "", string Searchbox = "")
        {
            //RoutingModel model = new RoutingModel();  
            ViewData["Title"] = "ReceiveChllan Details";
            TempData.Clear();
            HttpContext.Session.Remove("KeyReceiveChallan");
            var MainModel = new ReceiveChallanModel();
            var featuresoptions = IReceiveChallan.GetFeatureOption();

            if (featuresoptions?.Result?.Result != null &&
               featuresoptions.Result.Result.Rows.Count > 0)
            {
                MainModel.ShowOtherReqFields =
                    featuresoptions.Result.Result.Rows[0]["ShowOtherReqFields"]?.ToString() ?? "";
            }
            else
            {
                MainModel.ShowOtherReqFields = "";
            }

            MainModel.CC = HttpContext.Session.GetString("Branch");
            MainModel.Yearcode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));

            if (Mode != "U")
            {
                MainModel.UID = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                MainModel.CreatedByEmpid = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
                MainModel.CreatedByEmpName = HttpContext.Session.GetString("EmpName");
            }

            if (!string.IsNullOrEmpty(Mode) && ID > 0 && (Mode == "V" || Mode == "U"))
            {
                MainModel = await IReceiveChallan.GetViewByID(ID, YC, Mode);
                MainModel.Mode = Mode;
                MainModel.ID = ID;
                //MainModel = await BindModel(MainModel);
            }
            else
            {
                //MainModel = await BindModel(MainModel);
            }

            //MainModel.FinFromDate = ParseDate(HttpContext.Session.GetString("FromDate")).ToString("dd/MM/yyyy");
            //MainModel.FinToDate = ParseDate(HttpContext.Session.GetString("ToDate")).ToString("dd/MM/yyyy");
            MainModel.FinFromDate = ParseFormattedDate(HttpContext.Session.GetString("FromDate").Split(" ")[0]);
            MainModel.FinToDate = ParseFormattedDate(HttpContext.Session.GetString("ToDate").Split(" ")[0]);
            

            HttpContext.Session.SetString("KeyReceiveChallan", JsonConvert.SerializeObject(MainModel.ReceiveChallanList));
            MainModel.FromDateBack = FromDate;
            MainModel.ToDateBack = ToDate;
            MainModel.AccountNameBack = Account_Name;
            MainModel.PartCodeBack = PartCode;
            MainModel.ItemNameBack = ItemName;
            MainModel.GateNoBack = GateNo;
            MainModel.ChallanNoBack = ChallanNo;
            MainModel.SummaryDetailBack = SummaryDetail;
            MainModel.GlobalSearchBack=Searchbox;
            return View(MainModel);
        }

        public async Task<JsonResult> GetFormRights()
        {
            var userID = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
            var JSON = await IReceiveChallan.GetFormRights(userID);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> ListOfPendingChallaFromOtherBranch(int AccountCode)
        {
            var JSON = await IReceiveChallan.ListOfPendingChallaFromOtherBranch(AccountCode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }

        public async Task<JsonResult> PendingChallaItemDetailFromOtherBranch(int AccountCode,int EntryId,int YearCode,string SourceDB)
        {
            var JSON = await IReceiveChallan.PendingChallaItemDetailFromOtherBranch(AccountCode, EntryId, YearCode, SourceDB);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }

        [Route("{controller}/Dashboard")]
        public async Task<IActionResult> RCDashboard(string AccountName, string PartCode, string ItemName, string GateNo, string ChallanNo, string SummaryDetail, string Flag = "True", string FromDate = "", string ToDate = "")
        {
            HttpContext.Session.Remove("KeyReceiveChallan");
            var model = new RCDashboard();
            DateTime now = DateTime.Now;

            var Result = await IReceiveChallan.GetDashboardData(model);

            if (Result != null)
            {
                var _List = new List<TextValue>();
                DataSet DS = Result.Result;

                var DT = DS.Tables[0].DefaultView.ToTable(true, "EntryId", "Yearcode", "Entrydate", "RetNonRetChallan", "AgainstMRNOrGate", "MRNNo", "AgainstMRNYearCode", "BillOrChallan",
                "Account_Name", "TruckNo", "TransPort", "DeptTo", "Remark", "SendforQC", "gateno", "GateYearCode", "ChallanNo", "Challandate",
                "TotalAmount", "NetAmt", "TotalDiscountPercent", "TotalDiscountAmount", "ChallanType", "DocTypeCode", "InvoiceNo", "InvoiceYearCode", "pendcompleted",
                "MachineName", "CreatedByEmp", "CreatedOn", "UpdatedByEmp", "UpdatedOn", "CC", "UID", "gatedate", "mrndate", "IssueChallanEntryID", "IssueChallanNo", "IssueChallanYearCode", "ItemCode",
                "PartCode", "ItemName", "SeqNo", "Unit", "RecQty", "Rate", "Amount", "IssuedQty", "PendQty", "Produced", "GateQty", "Storeid", "StoreName", "pendtoissue",
                "Batchno", "Uniquebatchno", "ItemSize", "AltUnit", "AltQty", "PONO", "POYearCode", "PODate", "SchNo", "SchDate", "SchYearcode");

                model.RCDashboardList = CommonFunc.DataTableToList<ReceiveChallanDashboard>(DT, "RCDashboard");
                model.RCDashboardList = model.RCDashboardList
                    .GroupBy(d => d.EntryId)
                    .Select(g => g.First())
                    .ToList();

                if (Flag != "True")
                {
                    model.Account_Name = AccountName;
                    model.PartCode = PartCode;
                    model.ItemName = ItemName;
                    model.gateno = GateNo;
                    model.ChallanNo = ChallanNo;
                    model.FromDate = FromDate;
                    model.ToDate = ToDate;
                    model.SummaryDetail = SummaryDetail;
                }
            }
            model.SummaryDetail = "Summary";
            return View(model);
        }

        //public async Task<IActionResult> GetSearchData(RCDashboard model)
        //{
        //    var Result = await IReceiveChallan.GetDashboardData(model);
        //    DataSet DS = Result.Result;

        //    var DT = DS.Tables[0].DefaultView.ToTable(true, "EntryId", "Yearcode", "Entrydate", "RetNonRetChallan", "AgainstMRNOrGate", "MRNNo", "AgainstMRNYearCode", "BillOrChallan",
        //        "Account_Name", "TruckNo", "TransPort", "DeptTo", "Remark", "SendforQC", "gateno", "GateYearCode", "ChallanNo", "Challandate",
        //        "TotalAmount", "NetAmt", "TotalDiscountPercent", "TotalDiscountAmount", "ChallanType", "DocTypeCode", "InvoiceNo", "InvoiceYearCode", "pendcompleted",
        //        "MachineName", "CreatedByEmp", "CreatedOn", "UpdatedByEmp", "UpdatedOn", "CC", "UID", "gatedate", "mrndate", "IssueChallanEntryID", "IssueChallanNo", "IssueChallanYearCode", "ItemCode",
        //        "PartCode", "ItemName", "SeqNo", "Unit", "RecQty", "Rate", "Amount", "IssuedQty", "PendQty", "Produced", "GateQty", "Storeid", "StoreName", "pendtoissue",
        //        "Batchno", "Uniquebatchno", "ItemSize", "AltUnit", "AltQty", "PONO", "POYearCode", "PODate", "SchNo", "SchDate", "SchYearcode");

        //    model.RCDashboardList = CommonFunc.DataTableToList<ReceiveChallanDashboard>(DT, "RCDashboard");
        //    if (model.SummaryDetail == "Summary")
        //    {
        //        model.RCDashboardList = model.RCDashboardList
        //            .GroupBy(d => d.EntryId)
        //            .Select(g => g.First())
        //            .ToList();
        //    }

        //    return PartialView("_RCDashboardGrid", model);
        //}


        public async Task<IActionResult> GetSearchData(RCDashboard model, int pageNumber = 1, int pageSize = 15)
        {
            // Fetch data
            var Result = await IReceiveChallan.GetDashboardData(model);
            DataSet DS = Result.Result;

            var DT = DS.Tables[0].DefaultView.ToTable(true,
                "EntryId", "Yearcode", "Entrydate", "RetNonRetChallan", "AgainstMRNOrGate", "MRNNo", "AgainstMRNYearCode", "BillOrChallan",
                "Account_Name", "TruckNo", "TransPort", "DeptTo", "Remark", "SendforQC", "gateno", "GateYearCode", "ChallanNo", "Challandate",
                "TotalAmount", "NetAmt", "TotalDiscountPercent", "TotalDiscountAmount", "ChallanType", "DocTypeCode", "InvoiceNo", "InvoiceYearCode", "pendcompleted",
                "MachineName", "CreatedByEmp", "CreatedOn", "UpdatedByEmp", "UpdatedOn", "CC", "UID", "gatedate", "mrndate", "IssueChallanEntryID", "IssueChallanNo", "IssueChallanYearCode", "ItemCode",
                "PartCode", "ItemName", "SeqNo", "Unit", "RecQty", "Rate", "Amount", "IssuedQty", "PendQty", "Produced", "GateQty", "Storeid", "StoreName", "pendtoissue",
                "Batchno", "Uniquebatchno", "ItemSize", "AltUnit", "AltQty", "PONO", "POYearCode", "PODate", "SchNo", "SchDate", "SchYearcode");

            // Convert to list
            model.RCDashboardList = CommonFunc.DataTableToList<ReceiveChallanDashboard>(DT, "RCDashboard");

            // Apply summary grouping if needed
            if (model.SummaryDetail == "Summary")
            {
                model.RCDashboardList = model.RCDashboardList
                    .GroupBy(d => d.EntryId)
                    .Select(g => g.First())
                    .ToList();
            }

            // ✅ Store full list in session for delete/update operations
            var fullListJson = JsonConvert.SerializeObject(model.RCDashboardList);
            HttpContext.Session.SetString("KeyReceiveChallan", fullListJson);

            // ✅ Pagination logic
            model.TotalRecords = model.RCDashboardList.Count;
            model.PageNumber = pageNumber;
            model.PageSize = pageSize;

            model.RCDashboardList = model.RCDashboardList
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return PartialView("_RCDashboardGrid", model);
        }

        public IActionResult DeleteItemRow(int SeqNo)
        {
            var MainModel = new ReceiveChallanModel();
            string modelJson = HttpContext.Session.GetString("KeyReceiveChallan");
            List<ReceiveChallanDetail> ReceiveChallanDetail = new List<ReceiveChallanDetail>();
            if (modelJson != null)
            {
                ReceiveChallanDetail = JsonConvert.DeserializeObject<List<ReceiveChallanDetail>>(modelJson);
            }
            int Indx = Convert.ToInt32(SeqNo) - 1;

            if (ReceiveChallanDetail != null && ReceiveChallanDetail.Count > 0)
            {
                ReceiveChallanDetail.RemoveAt(Convert.ToInt32(Indx));

                Indx = 0;

                foreach (var item in ReceiveChallanDetail)
                {
                    Indx++;
                    //item.SeqNo = Indx;
                }
                MainModel.ReceiveChallanList = ReceiveChallanDetail;

                HttpContext.Session.SetString("KeyWorkOrderGrid", JsonConvert.SerializeObject(MainModel.ReceiveChallanList));
            }
            return PartialView("_ReceiveChallanGrid", MainModel);
        }
        public async Task<IActionResult> DeleteByID(int ID, int YC, string SummaryDetail, string FromDate = "", string ToDate = "", string AccountName = "", string PartCode = "", string ItemName = "", string GateNo = "", string ChallanNo = "")
        {
            var Result = await IReceiveChallan.DeleteByID(ID, YC).ConfigureAwait(false);

            if (Result.StatusText == "Deleted" || Result.StatusCode == HttpStatusCode.Gone)
            {
                ViewBag.isSuccess = true;
                TempData["410"] = "410";
            }
            else
            {
                ViewBag.isSuccess = false;
                TempData["500"] = "500";
            }
            return RedirectToAction("Dashboard", new { Flag = "false", FromDate = FromDate, ToDate = ToDate, AccountName = AccountName, PartCode = PartCode, ItemName = ItemName, GateNo = GateNo, ChallanNo = ChallanNo, SummaryDetail = SummaryDetail });
        }
        public async Task<JsonResult> NewEntryId(int YearCode)
        {
            var JSON = await IReceiveChallan.NewEntryId(YearCode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetMRNNo(int yearcode, string FromDate, string ToDate)
        {
            var FromDt = ParseFormattedDate(FromDate);
            var ToDt = ParseFormattedDate(ToDate);
            var JSON = await IReceiveChallan.GetMRNNo(yearcode, FromDt, ToDt);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetGateNo(int yearcode, string FromDate, string ToDate)
        {
            var FromDt = ParseFormattedDate(FromDate);
            var ToDt = ParseFormattedDate(ToDate);
            var JSON = await IReceiveChallan.GetGateNo(yearcode, FromDt, ToDt);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetMRNDetail(int EntryId)
        {
            var JSON = await IReceiveChallan.GetMRNDetail(EntryId);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetGateDetail(string GateNo, int GateYc, int GateEntryId, string FromDate, string ToDate, string Flag)
        {
            var JSON = await IReceiveChallan.GetGateDetail(GateNo, GateYc, GateEntryId, FromDate, ToDate, Flag);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetMRNYear(string MRNNO)
        {
            var JSON = await IReceiveChallan.GetMRNYear(MRNNO);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetMRNDate(string MRNNO, int MRNYC)
        {
            var JSON = await IReceiveChallan.GetMRNDate(MRNNO, MRNYC);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetGateYear(string Gateno, string FromDate, string ToDate, int yearcode)
        {
            var JSON = await IReceiveChallan.GetGateYear(Gateno, FromDate, ToDate, yearcode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetGateDate(string Gateno, int GateYC)
        {
            var JSON = await IReceiveChallan.GetGateDate(Gateno, GateYC);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillAlltDetail(string Gateno, int GateYC)
        {
            var JSON = await IReceiveChallan.FillAlltDetail(Gateno, GateYC);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillStore()
        {
            var JSON = await IReceiveChallan.FillStore();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public IActionResult AddReceiveChallanDetail(List<ReceiveChallanDetail> model)
        {
            try
            {
                var MainModel = new ReceiveChallanModel();
                var RCGrid = new List<ReceiveChallanDetail>();
                var ReceiveChallanGrid = new List<ReceiveChallanDetail>();

                var SeqNo = 0;
                foreach (var item in model)
                {
                    string modelJson = HttpContext.Session.GetString("KeyReceiveChallan");
                    IList<ReceiveChallanDetail> RCDetail = new List<ReceiveChallanDetail>();
                    if (modelJson != null)
                    {
                        RCDetail = JsonConvert.DeserializeObject<List<ReceiveChallanDetail>>(modelJson);
                    }

                    if (model != null)
                    {
                        if (RCDetail == null)
                        {
                            //item.SeqNo += SeqNo + 1;
                            RCGrid.Add(item);
                        }
                        else
                        {
                            if (RCDetail.Any(x => x.ItemCode == item.ItemCode && x.BatchNo == item.BatchNo && x.Storeid == item.Storeid && x.IssueChallanNo==item.IssueChallanNo && x.UniqueBatchno == item.UniqueBatchno))
                            {
                                
                                var duplicateInfo = new
                                {
                                    item.ItemCode,
                                    item.Storeid,
                                    item.BatchNo,
                                    item.IssueChallanNo
                                };
                                return StatusCode(207, "Duplicate");
                            }
                            else
                            {
                                //item.SeqNo = RCDetail.Count + 1;
                                RCGrid = RCDetail.Where(x => x != null).ToList();
                                ReceiveChallanGrid.AddRange(RCGrid);
                                RCGrid.Add(item);
                            }
                        }
                        RCGrid = RCGrid.OrderBy(item => item.SeqNo).ToList();
                        MainModel.ReceiveChallanList = RCGrid;

                        HttpContext.Session.SetString("KeyReceiveChallan", JsonConvert.SerializeObject(MainModel.ReceiveChallanList));
                    }
                    else
                    {
                        ModelState.TryAddModelError("Error", "Receive Challan List Cannot Be Empty...!");
                    }
                }


                return PartialView("_ReceiveChallanGrid", MainModel);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<JsonResult> EditItemRows(int SeqNo)
        {
            var MainModel = new ReceiveChallanModel();
            string modelJson = HttpContext.Session.GetString("KeyReceiveChallan");
            List<ReceiveChallanDetail> RCDetail = new List<ReceiveChallanDetail>();
            if (modelJson != null)
            {
                RCDetail = JsonConvert.DeserializeObject<List<ReceiveChallanDetail>>(modelJson);
            }
            var RCGrid = RCDetail.Where(x => x.SeqNo == SeqNo);
            string JsonString = JsonConvert.SerializeObject(RCGrid);
            return Json(JsonString);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("{controller}/Index")]
        public async Task<IActionResult> ReceiveChallan(ReceiveChallanModel model)
        {
            try
            {
                var RCGrid = new DataTable();

                string modelJson = HttpContext.Session.GetString("KeyReceiveChallan");
                IList<ReceiveChallanDetail> RCDetail = new List<ReceiveChallanDetail>();
                if (!string.IsNullOrEmpty(modelJson))
                {
                    RCDetail = JsonConvert.DeserializeObject<List<ReceiveChallanDetail>>(modelJson);
                }

                if (RCDetail == null)
                {
                    ModelState.Clear();
                    ModelState.TryAddModelError("ReceiveChallanDetail", "ReceiveChallan Grid Should Have Atleast 1 Item...!");
                    return View("ReceiveChallan", model);
                }
                else
                {
                    model.CC = HttpContext.Session.GetString("Branch");
                    model.UID = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                    model.CreatedByEmpid = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                    model.CreatedByEmpName = HttpContext.Session.GetString("EmpName");
                    if (model.Mode == "U")
                    {
                        model.UpdatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                        model.UpdatedByEmpName = HttpContext.Session.GetString("EmpName");
                    }

                    RCGrid = GetDetailTable(RCDetail);
                    model.MachineName = HttpContext.Session.GetString("ClientMachineName");
                    model.IPAddress = HttpContext.Session.GetString("ClientIP");
                    var Result = await IReceiveChallan.SaveReceiveChallan(model, RCGrid);

                    if (Result != null)
                    {
                        if (Result.StatusText == "Success" && Result.StatusCode == HttpStatusCode.OK)
                        {
                            ViewBag.isSuccess = true;
                            TempData["200"] = "200";
                            var model1 = new ReceiveChallanModel();
                            model1.FinFromDate = HttpContext.Session.GetString("FromDate");
                            model1.FinToDate = HttpContext.Session.GetString("ToDate");
                            model1.Yearcode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
                            model1.CC = HttpContext.Session.GetString("Branch");
                            //model1.ActualEnteredByName = HttpContext.Session.GetString("EmpName");
                            model1.CreatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                            HttpContext.Session.Remove("KeyReceiveChallan");
                            //return RedirectToAction("ReceiveChallan", model1);
                            return RedirectToAction(nameof(ReceiveChallan));
                        }
                        if (Result.StatusText == "Updated" && Result.StatusCode == HttpStatusCode.Accepted)
                        {
                            ViewBag.isSuccess = true;
                            TempData["202"] = "202";
                            var model1 = new ReceiveChallanModel();
                            model1.FinFromDate = HttpContext.Session.GetString("FromDate");
                            model1.FinToDate = HttpContext.Session.GetString("ToDate");
                            model1.Yearcode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
                            model1.CC = HttpContext.Session.GetString("Branch");
                            //model1.ActualEnteredByName = HttpContext.Session.GetString("EmpName");
                            model1.CreatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                            HttpContext.Session.Remove("KeyReceiveChallan");
                            //return RedirectToAction("ReceiveChallan", model1);
                            return RedirectToAction(nameof(ReceiveChallan));
                        }
                        if (Result.StatusText == "Error" && Result.StatusCode == HttpStatusCode.InternalServerError)
                        {
                            var errNum = Result.Result.Message.ToString().Split(":")[1];
                            if (errNum == " 2627")
                            {
                                ViewBag.isSuccess = false;
                                TempData["2627"] = "2627";
                                Logger.LogError("\n \n ********** LogError ********** \n " + JsonConvert.SerializeObject(Result) + "\n \n");
                                var model2 = new ReceiveChallanModel();
                                model2.FinFromDate = HttpContext.Session.GetString("FromDate");
                                model2.FinToDate = HttpContext.Session.GetString("ToDate");
                                model2.Yearcode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
                                model2.CC = HttpContext.Session.GetString("Branch");
                                // model2.ActualEnteredByName = HttpContext.Session.GetString("EmpName");
                                model2.CreatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                                return View(model2);
                            }
                           
                        }
                        if (Result.StatusText == "UnSuccess" && Result.Result.Rows[0].ItemArray[1] == 410)
                        {
                            ViewBag.isSuccess = false;
                            var input = "";
                            input = Result.Result.Rows[0].ItemArray[2];
                            TempData["ErrorMessage"] = input;
                            Logger.LogError("\n \n ********** LogError ********** \n " + JsonConvert.SerializeObject(Result) + "\n \n");
                            // return View("Error", Result);
                            return View(model);
                        }
                    }
                        return View(model);
                    }
                }
            catch (Exception ex)
            {
                LogException<ReceiveChallanController>.WriteException(Logger, ex);


                var ResponseResult = new ResponseResult()
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    StatusText = "Error",
                    Result = ex
                };

                return View("Error", ResponseResult);
                //return View(model);
            }
        }

        public IActionResult PrintReport(int EntryId = 0, int YearCode = 0, string Type = "")
        {
            string my_connection_string;
            string contentRootPath = IWebHostEnvironment.ContentRootPath;
            string webRootPath = IWebHostEnvironment.WebRootPath;
            //string frx = Path.Combine(_env.ContentRootPath, "reports", value.file);
            var webReport = new WebReport();

            webReport.Report.Load(webRootPath + "\\RecChallanReport.frx"); 

            webReport.Report.SetParameterValue("entryparam", EntryId);
            webReport.Report.SetParameterValue("yearparam", YearCode);
            my_connection_string = _connectionStringService.GetConnectionString();
            //my_connection_string = iconfiguration.GetConnectionString("eTactDB");
            webReport.Report.Dictionary.Connections[0].ConnectionString = my_connection_string;
            webReport.Report.Dictionary.Connections[0].ConnectionStringExpression = "";
            webReport.Report.SetParameterValue("MyParameter", my_connection_string);
            return View(webReport);
        }
        public ActionResult HtmlSave(int EntryId = 0, int YearCode = 0)
        {
            using (Report report = new Report())
            {
                string webRootPath = IWebHostEnvironment.WebRootPath;
                var webReport = new WebReport();

                webReport.Report.Load(webRootPath + "\\IndentPrint.frx");
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
                string webRootPath = IWebHostEnvironment.WebRootPath;
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
        private static DataTable GetDetailTable(IList<ReceiveChallanDetail> DetailList)
        {
            var DTSSGrid = new DataTable();

            DTSSGrid.Columns.Add("EntryId", typeof(int));
            DTSSGrid.Columns.Add("YearCode", typeof(int));
            DTSSGrid.Columns.Add("IssueChallanEntryID", typeof(int));
            DTSSGrid.Columns.Add("IssueChallanYearCode", typeof(int));
            DTSSGrid.Columns.Add("IssueChallanNo", typeof(string));
            DTSSGrid.Columns.Add("ItemCode", typeof(int));
            DTSSGrid.Columns.Add("PartCode", typeof(string));
            DTSSGrid.Columns.Add("SeqNo", typeof(int));
            DTSSGrid.Columns.Add("Unit", typeof(string));
            DTSSGrid.Columns.Add("RecQty", typeof(float));
            DTSSGrid.Columns.Add("Rate", typeof(float));
            DTSSGrid.Columns.Add("Amount", typeof(float));
            DTSSGrid.Columns.Add("IssuedQty", typeof(float));
            DTSSGrid.Columns.Add("PendQty", typeof(float));
            DTSSGrid.Columns.Add("Produced", typeof(string));
            DTSSGrid.Columns.Add("Remark", typeof(string));
            DTSSGrid.Columns.Add("GateQty", typeof(float));
            DTSSGrid.Columns.Add("Storeid", typeof(int));
            DTSSGrid.Columns.Add("pendtoissue", typeof(string));
            DTSSGrid.Columns.Add("Batchno", typeof(string));
            DTSSGrid.Columns.Add("Uniquebatchno", typeof(string));
            DTSSGrid.Columns.Add("ItemSize", typeof(string));
            DTSSGrid.Columns.Add("AltUnit", typeof(string));
            DTSSGrid.Columns.Add("AltQty", typeof(float));
            DTSSGrid.Columns.Add("PONO", typeof(string));
            DTSSGrid.Columns.Add("POYearCode", typeof(int));
            DTSSGrid.Columns.Add("PODate", typeof(string));
            DTSSGrid.Columns.Add("SchNo", typeof(string));
            DTSSGrid.Columns.Add("SchDate", typeof(string));
            DTSSGrid.Columns.Add("SchYearcode", typeof(int));
            //DateTime DeliveryDt = new DateTime();
            foreach (var Item in DetailList)
            {
                string uniqueString = Guid.NewGuid().ToString();
                DTSSGrid.Rows.Add(
                    new object[]
                    {
                   0,
                   0,
                    Item.IssueChallanEntryId,
                    Item.IssueChallanYearCode,
                    Item.IssueChallanNo ?? "",
                    Item.ItemCode,
                    Item.PartCode,
                    Item.SeqNo,
                    Item.Unit ?? "",
                    Item.RecQty,
                    Item.Rate,
                    Item.Amount,
                    Item.IssuedQty,
                    Item.PendQty,
                    Item.Produced ?? "",
                    Item.RemarkDetail ?? "",
                    Item.GateQty,
                    Item.Storeid,
                    Item.pendtoissue == null ? "" : Item.pendtoissue,
                    Item.BatchNo ?? "",
                    Item.UniqueBatchno ?? "",
                    Item.ItemSize ?? "",
                    Item.AltUnit ?? "",
                    Item.AltQty,
                    Item.PONO ?? "",
                    Item.POYearCode,
                    //Item.PoDate == null ? string.Empty : ParseFormattedDate(Item.PoDate.Split(" ")[0]),
                    //Item.PODate == null ? string.Empty : ParseDate(Item.PODate),
                    Item.PODate == null ? string.Empty : ParseFormattedDate(Item.PODate.Split(" ")[0]),

                    Item.SchNo ?? "",
                    Item.SchDate == null ? string.Empty :  ParseFormattedDate(Item.SchDate.Split(" ")[0]),
                    Item.SchYearCode
                    });
            }
            DTSSGrid.Dispose();
            return DTSSGrid;
        }

        public async Task<JsonResult> GetFeatureOption()
        {
            var JSON = await IReceiveChallan.GetFeatureOption();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }

        public async Task<JsonResult> GetTotalAmount(RCDashboard model)
        {
            var JSON = await IReceiveChallan.GetTotalAmount(model);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }

        [HttpGet]
        public IActionResult ExportRCDashboardToExcel()
        {
            // Load data
            string modelJson = HttpContext.Session.GetString("KeyReceiveChallan");

            List<RCDashboard> dashboardList = new List<RCDashboard>();

            if (!string.IsNullOrEmpty(modelJson))
            {
                dashboardList = JsonConvert.DeserializeObject<List<RCDashboard>>(modelJson);
            }

            if (dashboardList == null || dashboardList.Count == 0)
                return NotFound("No data available to export.");

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Recieve Challan");

            // ================================
            // HEADERS (ALL COLUMNS)
            // ================================
            string[] headers = {
        "Sr#","MRN NO","Gate No","Ret/NonRet Challan","Against MRNOrGate","Aginst MRNYearCode",
        "BillOrChallan","Gate YearCode","Challan No","ChallanDate","Account Name","Truck No",
        "Transport","Dept To","Remark","Total Amount","Net Amount","Total Dis%","Total Dis Amount",
        "Challan Type","Doc Type Code","Invoice No","Invoice YC","Gate Date","MRN Date","Issue Challan EntryId",
        "Issue Challan YC","Issue Challan No","ItemName","PartCode","Unit","RecQty","Rate","Amount","Issued Qty",
        "Pend Qty","Produced","Remark2","Gate Qty","Store Name","Pend To Issue","BatchNo","Unique BatchNo",
        "ItemSize","AltUnit","AltQty","PONO","PODate","SchNo"
    };

            for (int i = 0; i < headers.Length; i++)
                worksheet.Cell(1, i + 1).Value = headers[i];

            // ================================
            // ROWS
            // ================================
            int row = 2;
            int sr = 1;

            foreach (var d in dashboardList)
            {
                int col = 1;

                worksheet.Cell(row, col++).Value = sr++;
                worksheet.Cell(row, col++).Value = d.MRNNo;
                worksheet.Cell(row, col++).Value = d.gateno;
                worksheet.Cell(row, col++).Value = d.RetNonRetChallan;
                worksheet.Cell(row, col++).Value = d.AgainstMRNOrGate;
                worksheet.Cell(row, col++).Value = d.AgainstMRNYearCode;
                worksheet.Cell(row, col++).Value = d.BillOrChallan;
                worksheet.Cell(row, col++).Value = d.GateYearCode;
                worksheet.Cell(row, col++).Value = d.ChallanNo;
                worksheet.Cell(row, col++).Value = d.ChallanDate;
                worksheet.Cell(row, col++).Value = d.Account_Name;
                worksheet.Cell(row, col++).Value = d.TruckNo;
                worksheet.Cell(row, col++).Value = d.TransPort;
                worksheet.Cell(row, col++).Value = d.DeptTo;
                worksheet.Cell(row, col++).Value = d.Remark;
                worksheet.Cell(row, col++).Value = d.TotalAmount;
                worksheet.Cell(row, col++).Value = d.NetAmt;
                worksheet.Cell(row, col++).Value = d.TotalDiscountPercent;
                worksheet.Cell(row, col++).Value = d.TotalDiscountAmount;
                worksheet.Cell(row, col++).Value = d.ChallanType;
                worksheet.Cell(row, col++).Value = d.DocTypeCode;
                worksheet.Cell(row, col++).Value = d.InvoiceNo;
                worksheet.Cell(row, col++).Value = d.InvoiceYearCode;
                worksheet.Cell(row, col++).Value = d.GateDate;
                worksheet.Cell(row, col++).Value = d.MRNDate;
                worksheet.Cell(row, col++).Value = d.IssueChallanEntryId;
                worksheet.Cell(row, col++).Value = d.IssueChallanYearCode;
                worksheet.Cell(row, col++).Value = d.IssueChallanNo;

                // Newly added remaining columns:
                worksheet.Cell(row, col++).Value = d.ItemName;
                worksheet.Cell(row, col++).Value = d.PartCode;
                worksheet.Cell(row, col++).Value = d.Unit;
                worksheet.Cell(row, col++).Value = d.RecQty;
                worksheet.Cell(row, col++).Value = d.Rate;
                worksheet.Cell(row, col++).Value = d.Amount;
                worksheet.Cell(row, col++).Value = d.IssuedQty;
                worksheet.Cell(row, col++).Value = d.PendQty;
                worksheet.Cell(row, col++).Value = d.Produced;
                worksheet.Cell(row, col++).Value = d.Remark;
                worksheet.Cell(row, col++).Value = d.GateQty;
                worksheet.Cell(row, col++).Value = d.StoreName;
                worksheet.Cell(row, col++).Value = d.pendtoissue;
                worksheet.Cell(row, col++).Value = d.BatchNo;
                worksheet.Cell(row, col++).Value = d.UniqueBatchno;
                worksheet.Cell(row, col++).Value = d.ItemSize;
                worksheet.Cell(row, col++).Value = d.AltUnit;
                worksheet.Cell(row, col++).Value = d.AltQty;
                worksheet.Cell(row, col++).Value = d.PONO;
                worksheet.Cell(row, col++).Value = d.PODate;
                worksheet.Cell(row, col++).Value = d.SchNo;

                row++;
            }

            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Position = 0;

            return File(
                stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "RecieveChallan.xlsx"
            );
        }


    }
}
