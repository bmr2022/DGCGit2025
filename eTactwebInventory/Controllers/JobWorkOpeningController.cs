using eTactWeb.Data.Common;
using eTactWeb.Services.Interface;
using FastReport.Utils;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System.Runtime.Caching;
using static eTactWeb.Data.Common.CommonFunc;
using static eTactWeb.DOM.Models.Common;
using static FastReport.Utils.FileSize;
using static Grpc.Core.Metadata;
using static System.Runtime.InteropServices.JavaScript.JSType;
using eTactWeb.DOM.Models;
using System.Globalization;
using System.Data;
using System.Net;
using OfficeOpenXml;

namespace eTactWeb.Controllers
{
    public class JobWorkOpeningController : Controller
    {
        private readonly ILogger<JobWorkOpeningController> _logger;
        private readonly IMemoryCache _MemoryCache;
        public IJobWorkOpening _IJobWorkOpening { get; }
        public IWebHostEnvironment _IWebHostEnvironment { get; }

        public JobWorkOpeningController(ILogger<JobWorkOpeningController> logger, IMemoryCache iMemoryCache, IWebHostEnvironment iWebHostEnvironment, IJobWorkOpening iJobWorkOpening)
        {
            _logger = logger;
            _MemoryCache = iMemoryCache;
            _IWebHostEnvironment = iWebHostEnvironment;
            _IJobWorkOpening = iJobWorkOpening;
        }

        public async Task<IActionResult> JobWorkOpening(int ID, string Mode, int YC,string OpeningType)
        {
            ViewData["Title"] = "Inventory Details";
            TempData.Clear();
            _MemoryCache.Remove("KeyJobWorkOpeningGrid");
            var model = new JobWorkOpeningModel();
            model.UID = Convert.ToInt32(HttpContext.Session.GetString("UID"));
            model.MachineName = Environment.MachineName;
            model.CreatedOn = DateTime.Now;
            model.UpdatedOn = DateTime.Now;
            model.CreatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
            model.YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode")) - 1;
            model.cc = HttpContext.Session.GetString("Branch");
            model.FinStartDate = HttpContext.Session.GetString("FromDate");
            model.ActualEnteredByName = HttpContext.Session.GetString("EmpName");

            if (!string.IsNullOrEmpty(Mode) && ID > 0 && (Mode == "V" || Mode == "U"))
            {
                model = await _IJobWorkOpening.GetViewByID(ID, Mode, YC, OpeningType);
                model.Mode = Mode;
                model.ID = ID;
            }

            MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                SlidingExpiration = TimeSpan.FromMinutes(55),
                Size = 1024
            };

            _MemoryCache.Set("KeyJobWorkOpeningGrid", model.ItemDetailGrid, cacheEntryOptions);
            
            return View(model);
        }

        public async Task<JsonResult> GetFormRights()
        {
            var userID = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
            var JSON = await _IJobWorkOpening.GetFormRights(userID);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }

        [HttpPost]
        public async Task<IActionResult> JobWorkOpening(JobWorkOpeningModel model)
        {
            try
            {
                var JOGrid = new DataTable();

                _MemoryCache.TryGetValue("KeyJobWorkOpeningGrid", out List<JobWorkOpeningModel> JobWorkOpeningDetail);
                if (JobWorkOpeningDetail == null)
                {
                    ModelState.Clear();
                    ModelState.TryAddModelError("JobWorkOpeningDetail", "Gate Inward Grid Should Have Atleast 1 Item...!");
                    return View("JobWorkOpening", model);
                }
                else
                {
                    model.UpdatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                    model.UpdatedOn = DateTime.Now;
                    model.MachineName = Environment.MachineName;
                    model.CreatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                    model.CreatedOn = DateTime.Now;
                    model.YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode")) - 1;
                    model.cc = HttpContext.Session.GetString("Branch");

                    if (model.OpeningType == "CustomerJobwork")
                    {
                        JOGrid = GetCustomerDetailTable(JobWorkOpeningDetail);
                    }
                    else if (model.OpeningType == "RGPChallaan")
                    {
                        JOGrid = GetRGPChallanDetailTable(JobWorkOpeningDetail);
                    }
                    else
                    {
                        if (model.Mode == "U")
                        {
                            JOGrid = GetDetailTable(JobWorkOpeningDetail);
                        }
                        else
                        {
                            JOGrid = GetDetailTable(JobWorkOpeningDetail);
                        }
                    }


                    var Result = await _IJobWorkOpening.SaveJobWorkOpening(model, JOGrid);

                    if (Result != null)
                    {
                        if (Result.StatusText == "Success" && Result.StatusCode == HttpStatusCode.OK)
                        {
                            ViewBag.isSuccess = true;
                            TempData["200"] = "200";
                            _MemoryCache.Remove("KeyJobWorkOpeningGrid");
                            var model1 = new JobWorkOpeningModel();
                            model1.YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode")) - 1;
                            model1.cc = HttpContext.Session.GetString("Branch");
                            model1.CreatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                            _MemoryCache.Remove(JOGrid);
                            return View(model1);

                        }
                        if (Result.StatusText == "Updated" && Result.StatusCode == HttpStatusCode.Accepted)
                        {
                            ViewBag.isSuccess = true;
                            TempData["202"] = "202";
                            var model1 = new JobWorkOpeningModel();
                            model1.YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode")) - 1;
                            model1.cc = HttpContext.Session.GetString("Branch");
                            model1.CreatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                            _MemoryCache.Remove(JOGrid);
                            return RedirectToAction("Dashboard");
                        }
                        if (Result.StatusText == "Error" && Result.StatusCode == HttpStatusCode.InternalServerError)
                        {
                            var errNum = Result.Result.Message.ToString().Split(":")[1];
                            if (errNum == " 2627")
                            {
                                ViewBag.isSuccess = false;
                                TempData["2627"] = "2627";
                                _logger.LogError("\n \n ********** LogError ********** \n " + JsonConvert.SerializeObject(Result) + "\n \n");
                            }
                        }
                    }
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                LogException<JobWorkOpeningController>.WriteException(_logger, ex);
                var ResponseResult = new ResponseResult()
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    StatusText = "Error",
                    Result = ex
                };
                return View("Error", ResponseResult);
            }
        }
        private static DataTable GetDetailTable(IList<JobWorkOpeningModel> DetailList)
        {
            var JOGrid = new DataTable();

            JOGrid.Columns.Add("EntryId", typeof(int));
            JOGrid.Columns.Add("YearCode", typeof(int));
            JOGrid.Columns.Add("TransactionType", typeof(string));
            JOGrid.Columns.Add("IssJWChallanNo", typeof(string));
            JOGrid.Columns.Add("IssChalanEntryId", typeof(int));
            JOGrid.Columns.Add("IssChallanYearcode", typeof(int));
            JOGrid.Columns.Add("Isschallandate", typeof(DateTime));
            JOGrid.Columns.Add("Accountcode", typeof(int));
            JOGrid.Columns.Add("EntryDate", typeof(DateTime));
            JOGrid.Columns.Add("ItemCode", typeof(int));
            JOGrid.Columns.Add("OpnIssQty", typeof(decimal));
            JOGrid.Columns.Add("Rate", typeof(decimal));
            JOGrid.Columns.Add("unit", typeof(string));
            JOGrid.Columns.Add("Amount", typeof(decimal));
            JOGrid.Columns.Add("RecQty", typeof(decimal));
            JOGrid.Columns.Add("pendqty", typeof(decimal));
            JOGrid.Columns.Add("ScrapItemCode", typeof(int));
            JOGrid.Columns.Add("ScrapQty", typeof(decimal));
            JOGrid.Columns.Add("PendScrapToRec", typeof(decimal));
            JOGrid.Columns.Add("BomStatus", typeof(string));
            JOGrid.Columns.Add("BOMNO", typeof(int));
            JOGrid.Columns.Add("Bomdate", typeof(DateTime));
            JOGrid.Columns.Add("RecItemCode", typeof(int));
            JOGrid.Columns.Add("ProcessId", typeof(int));
            JOGrid.Columns.Add("ChallanQty", typeof(decimal));
            JOGrid.Columns.Add("BatchWise", typeof(string));
            JOGrid.Columns.Add("BatchNo", typeof(string));
            JOGrid.Columns.Add("UniqueBatchNo", typeof(string));
            JOGrid.Columns.Add("CC", typeof(string));
            JOGrid.Columns.Add("UID", typeof(int));
            JOGrid.Columns.Add("EnteredByEmpId", typeof(int));
            JOGrid.Columns.Add("Closed", typeof(string));

            foreach (var Item in DetailList)
            {

                JOGrid.Rows.Add(
                    new object[]
                    {
                    Item.EntryID,
                    Item.YearCode,
                    Item.TransactionType,
                    Item.IssJWChallanNo,
                    Item.IssChalanEntryId,
                    Item.IssChallanYearcode,
                    Item.Isschallandate,
                    Item.Accountcode,
                    Item.EntryDate,
                    Item.ItemCode,
                    Item.OpnIssQty,
                    Item.Rate,
                    Item.unit,
                    Item.Amount,
                    Item.RecQty,
                    Item.pendqty,
                    Item.ScrapItemCode,
                    Item.ScrapQty,
                    Item.PendScrapToRec,
                    Item.BomType == null ? "" : Item.BomType,
                    Item.BomNo,
                    Item.BomDate,
                    Item.RecItemCode,
                    Item.ProcessId,
                    Item.ChallanQty,
                    Item.BatchWise == null ? "" : Item.BatchWise,
                    Item.BatchNo,
                    Item.UniqueBatchNo,
                    Item.cc,
                    Item.UID,
                    Item.CreatedBy,
                    Item.Closed == null ? "" : Item.Closed,
                    });
            }
            JOGrid.Dispose();
            return JOGrid;
        }

        private static DataTable GetCustomerDetailTable(IList<JobWorkOpeningModel> DetailList)
        {
            var JOGrid = new DataTable();
    

            JOGrid.Columns.Add("EntryID", typeof(int));
            JOGrid.Columns.Add("YearCode", typeof(int));
            JOGrid.Columns.Add("EntryDate", typeof(DateTime));
            JOGrid.Columns.Add("RecItemCode", typeof(int));
            JOGrid.Columns.Add("RecQty", typeof(decimal));
            JOGrid.Columns.Add("unit", typeof(string));
            JOGrid.Columns.Add("Rate", typeof(Decimal));
            JOGrid.Columns.Add("Amount", typeof(decimal));
            JOGrid.Columns.Add("Closed", typeof(string));
            JOGrid.Columns.Add("OpnRecQty", typeof(decimal));
            JOGrid.Columns.Add("BomInd", typeof(string));
            JOGrid.Columns.Add("BOMNO", typeof(int));
            JOGrid.Columns.Add("BOMDate", typeof(DateTime));
            JOGrid.Columns.Add("Accountcode", typeof(int));
            JOGrid.Columns.Add("Recchallanno", typeof(string));
            JOGrid.Columns.Add("Recchallandate", typeof(string));
            JOGrid.Columns.Add("RecchallanYearCode", typeof(int));
            JOGrid.Columns.Add("cc", typeof(string));
            JOGrid.Columns.Add("processid", typeof(int));
            JOGrid.Columns.Add("PendOpnQty", typeof(decimal));
            JOGrid.Columns.Add("RemainingPendQty", typeof(decimal));
            JOGrid.Columns.Add("BatchNo", typeof(string));
            JOGrid.Columns.Add("UniqueBatchno", typeof(string));
            JOGrid.Columns.Add("ScrapItemCode", typeof(string));
            JOGrid.Columns.Add("recscrapQty", typeof(decimal));
            JOGrid.Columns.Add("UID", typeof(int));
            JOGrid.Columns.Add("EnteredBy", typeof(int));
            JOGrid.Columns.Add("ActualEntryDate", typeof(DateTime));
            JOGrid.Columns.Add("LastUpdateBy", typeof(int));
            JOGrid.Columns.Add("LastUpdateDate", typeof(DateTime));
            JOGrid.Columns.Add("IssueItemCode", typeof(int));

            foreach (var Item in DetailList)
            {
                var CreatedOn = ParseDate(Item.CreatedOn.ToString());
                var LastUpdatedOn = ParseDate(Item.UpdatedOn.ToString());
                JOGrid.Rows.Add(
                    new object[]
                    {
                    Item.EntryID,
                    Item.YearCode,
                    Item.EntryDate,
                    Item.ItemCode,
                    Item.RecQty,
                    Item.unit,
                    Item.Rate,
                    Item.Amount,
                    Item.Closed,
                    Item.pendqty,
                    Item.BomType,
                    Item.BomNo,
                    Item.BomDate,
                    Item.Accountcode,
                    Item.IssJWChallanNo,
                    Item.Isschallandate,
                    Item.IssChallanYearcode,
                    Item.cc,
                    Item.ProcessId,
                    Item.pendqty,
                    Item.pendqty,
                    Item.BatchNo,
                    Item.UniqueBatchNo,
                    Item.ScrapItemCode,
                    Item.ScrapQty,
                    Item.UID,
                    Item.UID,
                    CreatedOn.ToString("yyyy/MM/dd") ?? "",
                    Item.UpdatedBy,
                    LastUpdatedOn.ToString("yyyy/MM/dd") ?? "",
                    Item.RecItemCode
                    });
            }
            JOGrid.Dispose();
            return JOGrid;
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

        private static DataTable GetRGPChallanDetailTable(IList<JobWorkOpeningModel> DetailList)
        {
            var JOGrid = new DataTable();

            JOGrid.Columns.Add("EntryID", typeof(int));
            JOGrid.Columns.Add("YearCode", typeof(int));
            JOGrid.Columns.Add("RGPNRGP", typeof(string));
            JOGrid.Columns.Add("ChallanType", typeof(string));
            JOGrid.Columns.Add("ChallanNo", typeof(string));
            JOGrid.Columns.Add("ChalanEntryId", typeof(int));
            JOGrid.Columns.Add("ChallanYearcode", typeof(int));
            JOGrid.Columns.Add("challandate", typeof(DateTime));
            JOGrid.Columns.Add("Accountcode", typeof(int));
            JOGrid.Columns.Add("EntryDate", typeof(DateTime));
            JOGrid.Columns.Add("ItemCode", typeof(int));
            JOGrid.Columns.Add("Qty", typeof(decimal));
            JOGrid.Columns.Add("Rate", typeof(decimal));
            JOGrid.Columns.Add("unit", typeof(string));
            JOGrid.Columns.Add("Amount", typeof(decimal));
            JOGrid.Columns.Add("pendqty", typeof(decimal));
            JOGrid.Columns.Add("RecItemCode", typeof(int));
            JOGrid.Columns.Add("ProcessId", typeof(int));
            JOGrid.Columns.Add("ChallanQty", typeof(decimal));
            JOGrid.Columns.Add("BatchNo", typeof(string));
            JOGrid.Columns.Add("UniqueBatchNo", typeof(string));
            JOGrid.Columns.Add("cc", typeof(string));
            JOGrid.Columns.Add("UID", typeof(string));
            JOGrid.Columns.Add("EnteredByEmpId", typeof(int));
            JOGrid.Columns.Add("ClosedChallan", typeof(string));
            JOGrid.Columns.Add("CloseChallandate", typeof(DateTime));
            JOGrid.Columns.Add("CloseChallanByEmpId", typeof(int));

            foreach (var Item in DetailList)
            {

                JOGrid.Rows.Add(
                    new object[]
                    {
                    Item.EntryID,
                    Item.YearCode,
                    Item.RGPNRGP,
                    Item.ChallanType,
                    Item.IssJWChallanNo,
                    Item.IssChalanEntryId,
                    Item.IssChallanYearcode,
                    Item.Isschallandate,
                    Item.Accountcode,
                    Item.EntryDate,
                    Item.ItemCode,
                    Item.RecQty,
                    Item.Rate,
                    Item.unit,
                    Item.Amount,
                    Item.pendqty,
                    Item.ItemCode,
                    Item.ProcessId,
                    Item.ChallanQty,
                    Item.BatchNo,
                    Item.UniqueBatchNo,
                    Item.cc,
                    Item.UID,
                    Item.EnteredByEmpId,
                    Item.ClosedChallan == null ? "" : Item.ClosedChallan,
                    Item.CloseChallandate,
                    Item.CloseChallanByEmpId == null ? 0 : Item.CloseChallanByEmpId,
                    });
            }
            JOGrid.Dispose();
            return JOGrid;
        }
       
        public async Task<IActionResult> GetSearchData(JobWorkOpeningDashboard model)
        {
            model.FromDate = ParseFormattedDate((model.FromDate).Split(" ")[0]);
            model.ToDate = ParseFormattedDate((model.ToDate).Split(" ")[0]);
            var Result = await _IJobWorkOpening.GetDashboardData(model);
            DataSet DS = Result.Result;
            var DT = new DataTable();
            if (Result != null)
            {
                if (model.OpeningType == "CustomerJobwork")
                {
                    DT = DS.Tables[0].DefaultView.ToTable(true, "VendoreName", "IssJWChallanNo", "IssChallanYearcode", "Isschallandate",
                              "EntryDate", "Branch", "ActualEntryByEmp", "Closed","ItemCode",
                              "VJOBOPENEntryID", "VJOBOPENYearCode", "LastUpdatedByEmp", "repotype", "OpeningType", "PartCode", "ItemName", "Accountcode",
                              "EnteredByMachine"
                              );
                    model.DashboardGrid = CommonFunc.DataTableToList<DashboardGrid>(DT, "CustomerSummary");
                }
                else if (model.OpeningType == "RGPChallaan")
                {
                    DT = DS.Tables[0].DefaultView.ToTable(true, "VendoreName", "IssJWChallanNo", "IssChallanYearcode", "Isschallandate",
                              "EntryDate", "Branch", "ActualEntryByEmp", "ClosedChallan", "ItemCode",
                              "CloseChallandate", "VJOBOPENEntryID", "VJOBOPENYearCode", "RGPNRGP", "ChallanType", "repotype", "OpeningType", "PartCode", "ItemName", "Accountcode",
                              "UID"
                              );
                    model.DashboardGrid = CommonFunc.DataTableToList<DashboardGrid>(DT, "RGPChallan");
                }
                else
                {
                    DT = DS.Tables[0].DefaultView.ToTable(true, "VendoreName", "IssJWChallanNo", "IssChallanYearcode", "Isschallandate", "EntryDate",
                                 "Branch", "Emp_id", "Empname", "Closed", "IssChalanEntryId", "ItemCode",
                                 "VJOBOPENEntryID", "VJOBOPENYearCode", "ActualEntryByEmp", "LastUpdatedByEmp", "EnteredByMachine", "repotype", "OpeningType", "Accountcode", "PartCode", "ItemName"
                                 );
                    model.DashboardGrid = CommonFunc.DataTableToList<DashboardGrid>(DT, "jobWorkopening");
                }
            }
            return PartialView("_JobWorkOpeningDashboardGrid", model);
        }

        public async Task<IActionResult> GetDetailData(JobWorkOpeningDashboard model)
        {
            model.FromDate = ParseFormattedDate((model.FromDate).Split(" ")[0]);
            model.ToDate = ParseFormattedDate((model.ToDate).Split(" ")[0]);
            var Result = await _IJobWorkOpening.GetDetailData(model);
            DataSet DS = Result.Result;
            var DT = new DataTable();
            if (Result != null)
            {
                if (model.OpeningType == "CustomerJobwork")
                {
                    DT = DS.Tables[0].DefaultView.ToTable(true, "EntryDate", "VendoreName", "IssJWChallanNo", "Isschallandate",
                              "IssChallanYearcode", "RecPartCode", "RecItemName", "RecQty",
                              "unit", "Rate", "Amount", "BomStatus", "BOMNO", "BOMDate", 
                              "pendqty", "RemainingPendQty", "BatchNo", "UniqueBatchno", "ProcessName", "PartCode",
                              "ItemName", "ScrapItemName", "ScrapPartCode", "Closed", "ActualEntryByEmp", "LastUpdatedByEmp", 
                              "EnteredByMachine", "VJOBOPENEntryID", "VJOBOPENYearCode",
                              "Branch", "uid", "repotype", "Accountcode", "ItemCode",
                              "OpeningType");
                    model.DetailDashboardGrid = CommonFunc.DataTableToList<DetailDashboardGrid>(DT, "customerJobWorkOpeningDetail");
                }
                else if (model.OpeningType == "RGPChallaan")
                {
                    DT = DS.Tables[0].DefaultView.ToTable(true, "VendoreName", "IssJWChallanNo", "IssChallanYearcode", "Isschallandate", "EntryDate",
                              "ItemName", "PartCode", "ChallanQty", "RecQty",
                              "pendqty","Rate", "unit", "Amount", "BatchNo", "UniqueBatchNo", "Branch",
                              "uid", "ActualEntryByEmp", "ClosedChallan", "CloseChallandate", "repotype", "OpeningType",
                              "VJOBOPENEntryID", "VJOBOPENYearCode","RGPNRGP","ChallanType", "ItemCode","Accountcode"
                              );
                    model.DetailDashboardGrid = CommonFunc.DataTableToList<DetailDashboardGrid>(DT, "RGPChallanDetail");
                }
                else
                {
                    DT = DS.Tables[0].DefaultView.ToTable(true, "VendoreName", "IssJWChallanNo", "IssChallanYearcode", "Isschallandate", "EntryDate",
                       "PartCode", "ItemName", "OpnIssQty", "Rate", "unit",
                       "Amount", "RecQty", "pendqty", "RecPartCode", "RecItemName",
                       "ScrapPartCode", "ScrapItemName", "ScrapQty", "PendScrapToRec", "BomStatus",
                       "ProcessName", "ChallanQty", "BatchNo", "UniqueBatchNo", "Branch",
                       "Emp_id", "Empname", "Closed", "IssChalanEntryId", "VJOBOPENEntryID", "Accountcode",
                       "VJOBOPENYearCode", "ActualEntryByEmp", "LastUpdatedByEmp", "EnteredByMachine", "ItemCode", "repotype", "OpeningType"
                       );
                    model.DetailDashboardGrid = CommonFunc.DataTableToList<DetailDashboardGrid>(DT, "jobWorkopeningDetail");
                }                
            }

            

            return PartialView("_JobWorkDetailDashboardGrid", model);
        }

        public async Task<IActionResult> DeleteByID(int ID, int YC, string repotype, string OpeningType, int ItemCode, string Challanno, 
            string Partcode, string ItemName, string FromDate, string ToDate, string SummaryDetail, string AccountName,
            int ActualEntryById,string MachineName,int AccountCode)
        {
            var Result = await _IJobWorkOpening.DeleteByID(ID, YC, repotype, ItemCode, Challanno,OpeningType, ActualEntryById, MachineName,AccountCode,Partcode,ItemName).ConfigureAwait(false);

            if (Result.StatusText == "Success" || Result.StatusCode == HttpStatusCode.Gone)
            {
                ViewBag.isSuccess = true;
                TempData["410"] = "410";
            }
            else
            {
                ViewBag.isSuccess = false;
                TempData["500"] = "500";
            }
            //int ItemCode = 0, string PartCode = "", string ItemName = "", string FromDate = "", string ToDate = "", string SummaryDetail = "",int Challanno = 0,string AccountName = ""
            return RedirectToAction("Dashboard", new { FromDate = FromDate, ToDate = ToDate, SummaryDetail = SummaryDetail, Challanno = Challanno, 
                AccountName = AccountName,ItemName = ItemName,PartCode=Partcode,OpeningType = OpeningType});
        }
        public async Task<IActionResult> Dashboard(int ItemCode = 0, string PartCode = "", string ItemName = "", string FromDate = "", string ToDate = "", string SummaryDetail = "", string Challanno = "", string AccountName = "",string OpeningType = "")
        {
            //_MemoryCache.Remove("KeyJobWorkOpeningGrid");
            var model = new JobWorkOpeningDashboard();
            DateTime now = DateTime.Now;
            //DateTime firstDayOfMonth = new DateTime(now.Year, now.Month, 1);
            if (FromDate == "" && ToDate == "")
            {
                model.FromDate = new DateTime(now.Year, now.Month, 1).ToString("dd/MM/yyyy").Replace("-", "/");
                //model.FromDate = ParseFormattedDate((model.FromDate).Split(" ")[0]);
                model.ToDate = (DateTime.Today).ToString("dd/MM/yyyy").Replace("-", "/");
                //model.ToDate = ParseFormattedDate((model.ToDate).Split(" ")[0]);
            }
            else
            {
                model.FromDate = FromDate;
                model.ToDate = ToDate;
            }
            model.FromDate = ParseFormattedDate((model.FromDate).Split(" ")[0]);
            model.ToDate = ParseFormattedDate((model.ToDate).Split(" ")[0]);
            var Result = await _IJobWorkOpening.GetDashboardData(model);

            if (Result != null)
            {
                var _List = new List<TextValue>();
                DataSet DS = Result.Result;
                if (model.OpeningType == "CustomerJobwork")
                {
                    var DT = DS.Tables[0].DefaultView.ToTable(true, "VendoreName", "IssJWChallanNo", "IssChallanYearcode", "Isschallandate",
                              "EntryDate", "Branch", "ActualEntryByEmp", "Closed", "ItemCode",
                              "VJOBOPENEntryID", "VJOBOPENYearCode", "LastUpdatedByEmp", "repotype", "OpeningType", "PartCode", "ItemName", "Accountcode",
                              "EnteredByMachine"
                              );
                    model.DashboardGrid = CommonFunc.DataTableToList<DashboardGrid>(DT, "CustomerSummary");
                }
                else if (model.OpeningType == "RGPChallaan")
                {
                    var DT = DS.Tables[0].DefaultView.ToTable(true, "VendoreName", "IssJWChallanNo", "IssChallanYearcode", "Isschallandate",
                              "EntryDate", "Branch", "ActualEntryByEmp", "ClosedChallan", "ItemCode",
                              "CloseChallandate", "VJOBOPENEntryID", "VJOBOPENYearCode", "RGPNRGP", "ChallanType", "repotype", "OpeningType", "PartCode", "ItemName", "Accountcode",
                              "UID"
                              );
                    model.DashboardGrid = CommonFunc.DataTableToList<DashboardGrid>(DT, "RGPChallan");
                }
                else
                {
                    var DT = DS.Tables[0].DefaultView.ToTable(true, "VendoreName", "IssJWChallanNo", "IssChallanYearcode", "Isschallandate", "EntryDate",
                                 "Branch", "Emp_id", "Empname", "Closed", "IssChalanEntryId", "ItemCode",
                                 "VJOBOPENEntryID", "VJOBOPENYearCode", "ActualEntryByEmp", "LastUpdatedByEmp", "EnteredByMachine", "repotype", "OpeningType", "Accountcode", "PartCode", "ItemName"
                                 );
                    model.DashboardGrid = CommonFunc.DataTableToList<DashboardGrid>(DT, "jobWorkopening");
                }
                model.ItemCode = ItemCode;
                model.PartCode = PartCode;
                model.ItemName = ItemName;
                model.FromDate1 = FromDate;
                model.ToDate1 = ToDate;
                model.IssJWChallanNo = Challanno;                
                model.OpeningType = OpeningType;                
            }

            return View(model);
        }

        public async Task<JsonResult> FillEntryId(int YearCode,string FormTypeCustJWNRGP)
        {
            var JSON = await _IJobWorkOpening.FillEntryId("NewEntryId", YearCode, FormTypeCustJWNRGP, "SP_JobworkOpeningDetail");
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillUnitAltUnit(int ItemCode)
        {
            var JSON = await _IJobWorkOpening.FillUnitAltUnit(ItemCode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }

        public async Task<JsonResult> FillItemPartCode()
        {
            var JSON = await _IJobWorkOpening.FillItemPartCode();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillBomNo(int ItemCode, int RecItemCode, string BomType)
        {
            var JSON = await _IJobWorkOpening.FillBomNo(ItemCode, RecItemCode, BomType);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillPartyName()
        {
            var JSON = await _IJobWorkOpening.FillPartyName();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillProcessName()
        {
            var JSON = await _IJobWorkOpening.FillProcessName();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillRecItemPartCode(string BOMIND, int ItemCode)
        {
            var JSON = await _IJobWorkOpening.FillRecItemPartCode(BOMIND, ItemCode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillScrapItemPartCode(string BOMIND, int ItemCode)
        {
            var JSON = await _IJobWorkOpening.FillScrapItemPartCode(BOMIND, ItemCode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }

        public IActionResult EditItemRow(int SeqNo, string Mode)
        {
            //IList<JobWorkOpeningModel> JobWorkOpeningDetail = new List<JobWorkOpeningModel>();
            //if (Mode == "U")
            //{
            //    _MemoryCache.TryGetValue("KeyJobWorkOpeningGrid", out JobWorkOpeningDetail);
            //}
            //else
            //{
            _MemoryCache.TryGetValue("KeyJobWorkOpeningGrid", out IList<JobWorkOpeningModel> JobWorkOpeningDetail);
            //}
            IEnumerable<JobWorkOpeningModel> SSGrid = JobWorkOpeningDetail;
            if (JobWorkOpeningDetail != null)
            {
                SSGrid = JobWorkOpeningDetail.Where(x => x.SeqNo == SeqNo);
            }
            string JsonString = JsonConvert.SerializeObject(SSGrid);
            return Json(JsonString);
        }

        public IActionResult DeleteItemRow(int SeqNo)
        {
            var MainModel = new JobWorkOpeningModel();
            _MemoryCache.TryGetValue("KeyJobWorkOpeningGrid", out List<JobWorkOpeningModel> UserRightDetail);
            int Indx = Convert.ToInt32(SeqNo) - 1;

            if (UserRightDetail != null && UserRightDetail.Count > 0)
            {
                UserRightDetail.RemoveAt(Convert.ToInt32(Indx));

                Indx = 0;

                foreach (var item in UserRightDetail)
                {
                    Indx++;
                    item.SeqNo = Indx;
                }
                MainModel.ItemDetailGrid = UserRightDetail;

                MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                    SlidingExpiration = TimeSpan.FromMinutes(55),
                    Size = 1024,
                };

                _MemoryCache.Set("KeyJobWorkOpeningGrid", MainModel.ItemDetailGrid, cacheEntryOptions);
            }


            return PartialView("_JobWorkOpeningGrid", MainModel);
        }


        public IActionResult AddJobWorkOpeningDetail(JobWorkOpeningModel model)
        {
            try
            {
                _MemoryCache.TryGetValue("KeyJobWorkOpeningGrid", out IList<JobWorkOpeningModel> JobWorkOpeningItemDetail);

                var MainModel = new JobWorkOpeningModel();
                var JobWorkOpeningGrid = new List<JobWorkOpeningModel>();
                var JobWorkOpening = new List<JobWorkOpeningModel>();
                var SSGrid = new List<JobWorkOpeningModel>();

                if (model != null)
                {
                    if (JobWorkOpeningItemDetail == null)
                    {
                        model.SeqNo = 1;
                        JobWorkOpening.Add(model);
                    }
                    else
                    {
                        if (model.IsCustomerJobWork == "CustomerJobworkOpening")
                        {
                            if (JobWorkOpeningItemDetail.Where(x => x.RecItemCode == model.RecItemCode && x.Accountcode == model.Accountcode && x.IssJWChallanNo == model.IssJWChallanNo && x.UniqueBatchNo == model.UniqueBatchNo).Any())
                            {
                                return StatusCode(207, "Duplicate");
                            }
                            else
                            {
                                model.SeqNo = JobWorkOpeningItemDetail.Count + 1;
                                JobWorkOpening = JobWorkOpeningItemDetail.Where(x => x != null).ToList();
                                SSGrid.AddRange(JobWorkOpening);
                                JobWorkOpening.Add(model);
                            }
                        }
                        else
                        {
                            if (JobWorkOpeningItemDetail.Where(x => x.PartCode == model.PartCode && x.BatchNo == model.BatchNo && x.IssJWChallanNo == model.IssJWChallanNo).Any())
                            {
                                return StatusCode(207, "Duplicate");
                            }
                            else
                            {
                                model.SeqNo = JobWorkOpeningItemDetail.Count + 1;
                                JobWorkOpening = JobWorkOpeningItemDetail.Where(x => x != null).ToList();
                                SSGrid.AddRange(JobWorkOpening);
                                JobWorkOpening.Add(model);
                            }
                        }
                    }

                    MainModel.ItemDetailGrid = JobWorkOpening;

                    MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
                    {
                        AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                        SlidingExpiration = TimeSpan.FromMinutes(55),
                        Size = 1024,
                    };

                    _MemoryCache.Set("KeyJobWorkOpeningGrid", MainModel.ItemDetailGrid, cacheEntryOptions);
                }
                else
                {
                    ModelState.TryAddModelError("Error", "Schedule List Cannot Be Empty...!");
                }

                return PartialView("_JobWorkOpeningGrid", MainModel);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }       
        public async Task<JsonResult> GetServerDate()
        {
            try
            {
                DateTime time = DateTime.Now;
                string format = "MMM ddd d HH:mm yyyy";
                string formattedDate = time.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);

                var dt = time.ToString(format);
                return Json(formattedDate);
                //string apiUrl = "https://worldtimeapi.org/api/ip";

                //using (HttpClient client = new HttpClient())
                //{
                //    HttpResponseMessage response = await client.GetAsync(apiUrl);

                //    if (response.IsSuccessStatusCode)
                //    {
                //        string content = await response.Content.ReadAsStringAsync();
                //        JObject jsonObj = JObject.Parse(content);

                //        string datetimestring = (string)jsonObj["datetime"];
                //        var formattedDateTime = datetimestring.Split(" ")[0];

                //        DateTime parsedDate = DateTime.ParseExact(formattedDateTime, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                //        string formattedDate = parsedDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);

                //        return Json(formattedDate);
                //    }
                //    else
                //    {
                //        string errorContent = await response.Content.ReadAsStringAsync();
                //        throw new HttpRequestException($"Failed to fetch server date and time. Status Code: {response.StatusCode}. Content: {errorContent}");
                //    }
                //}
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



        [HttpPost]
        public async Task<IActionResult> UploadExcel()
        {
            try
            {
                IFormFile ExcelFile = Request.Form.Files.FirstOrDefault();
                if (ExcelFile == null || ExcelFile.Length == 0)
                {
                    return BadRequest("Invalid file. Please upload a valid Excel file.");
                }

                //string validPartCodesString = Request.Form["validPartCodes"];
                //var validPartCodes = new HashSet<string>(validPartCodesString.Split(','), StringComparer.OrdinalIgnoreCase);

                string path = Path.Combine(this._IWebHostEnvironment.WebRootPath, "Uploads");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                string fileName = Path.GetFileName(ExcelFile.FileName);
                string filePath = Path.Combine(path, fileName);
                using (FileStream stream = new FileStream(filePath, FileMode.Create))
                {
                    await ExcelFile.CopyToAsync(stream);
                }

                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                var JobWorkGridList = new List<JobWorkOpeningModel>();
                var MainModel = new JobWorkOpeningModel();
                var errors = new List<string>(); // List to collect validation errors

                using (var package = new ExcelPackage(new FileInfo(filePath)))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.FirstOrDefault();
                    if (worksheet == null)
                    {
                        return BadRequest("Uploaded file does not contain any worksheet.");
                    }

                    var rowCount = worksheet.Dimension.Rows;

                    for (int row = 2; row <= rowCount; row++)
                    {
                        bool isRowEmpty = true;
                        for (int col = 1; col <= worksheet.Dimension.Columns; col++)
                        {
                            if (!string.IsNullOrEmpty((worksheet.Cells[row, col].Value ?? string.Empty).ToString().Trim()))
                            {
                                isRowEmpty = false;
                                break;
                            }
                        }
                        if (isRowEmpty) continue;

                        //var partCode = (worksheet.Cells[row, 1].Value ?? string.Empty).ToString().Trim();
                        //var validateUnit = (worksheet.Cells[row, 2].Value ?? string.Empty).ToString().Trim();

                        //// **Validate Unit**
                        //if (validateUnit.Length > 3)
                        //{
                        //    errors.Add($"Invalid Unit at row {row}: {validateUnit}");
                        //    continue;
                        //}

                        //// **Validate PartCode**
                        //if (!validPartCodes.Contains(partCode))
                        //{
                        //    errors.Add($"Invalid PartCode at row {row}: {partCode}");
                        //    continue;
                        //}

                        // **Fetch Item Details from Database**
                        //var response = await _ISaleOrder.FillItemPartCode(partCode);

                        //if (response?.Result is not DataTable itemData || itemData.Rows.Count == 0)
                        //{
                        //    errors.Add($"No data found for PartCode '{partCode}' at row {row}.");
                        //    continue;
                        //}

                        //string hsnNo = itemData.Rows[0]["HSNNo"].ToString();
                        //string unit = itemData.Rows[0]["Unit"].ToString();
                        //string altUnit = itemData.Rows[0]["AltUnit"].ToString();
                        //string itemName = itemData.Rows[0]["Item_Name"].ToString();
                        //int itemCode = Convert.ToInt32(itemData.Rows[0]["Item_Code"]);

                        //string soType = Request.Form["SOType"];

                        int EntryID = Convert.ToInt32(Request.Form["EntryID"]);
                        int YearCode = Convert.ToInt32(Request.Form["YearCode"]);
                        string EntryDate = Request.Form["EntryDate"];
                        int Accountcode = Convert.ToInt32(Request.Form["Accountcode"]);
                        string PartyName = Request.Form["PartyName"];


                        //bool isSOTypeClose = soType.Equals("Close", StringComparison.OrdinalIgnoreCase);

                        // **Quantity and Rate Validation**
                        //decimal qty = isSOTypeClose
                        //    ? decimal.TryParse(worksheet.Cells[row, 4].Value?.ToString(), out decimal tempQty) ? tempQty : 0
                        //    : 0;

                        decimal rate = decimal.TryParse(worksheet.Cells[row, 7].Value?.ToString(), out decimal tempRate) ? tempRate : 0;
                        decimal ChallanQty = decimal.TryParse(worksheet.Cells[row, 5].Value?.ToString(), out decimal TEMPChallanQty) ? TEMPChallanQty : 0;
                        decimal PendingQty = decimal.TryParse(worksheet.Cells[row, 6].Value?.ToString(), out decimal tempPendingQty) ? tempPendingQty : 0;


                        //if (isSOTypeClose && qty <= 0)
                        //{
                        //    errors.Add($"Qty should be greater than 0 at row {row} ");
                        //    continue; // Skip processing this row
                        //}

                        // **Delivery Date Validation**
                        //string deliveryDateStr = worksheet.Cells[row, 7].Value?.ToString();
                        //DateTime? deliveryDate = null;
                        //if (DateTime.TryParse(deliveryDateStr, out DateTime tempDeliveryDate))
                        //{
                        //    if (tempDeliveryDate <= DateTime.Today)
                        //    {
                        //        errors.Add($"Delivery Date at row {row} must be greater than today ({DateTime.Today:dd/MMM/yyyy}).");
                        //    }
                        //    else
                        //    {
                        //        deliveryDate = tempDeliveryDate;
                        //    }
                        //}

                        //// **Calculate Amount (Qty * Rate)**
                        //decimal amount = qty * rate;

                        // **Add to SaleGridList**
                        JobWorkGridList.Add(new JobWorkOpeningModel
                        {
                            SeqNo = JobWorkGridList.Count + 1,
                            EntryID = EntryID,
                            YearCode = YearCode,
                            EntryDate = EntryDate,
                            ItemName=worksheet.Cells[row, 4].Value?.ToString() ?? "",
                            PartCode = worksheet.Cells[row, 4].Value?.ToString() ?? "",
                            ItemCode= int.TryParse(worksheet.Cells[row, 4].Value?.ToString(), out int tempAltQty) ? tempAltQty : 0,
                            IssJWChallanNo=worksheet.Cells[row, 1].Value?.ToString() ?? "",
                            IssChallanYearcode = YearCode,
                            Isschallandate= DateTime.TryParse(worksheet.Cells[row, 2].Value?.ToString(), out DateTime tempChallanDate)
                                ? tempChallanDate.ToString("yyyy-MM-dd")
                                : DateTime.Now.ToString("yyyy-MM-dd"),
                            Accountcode=Accountcode,
                            AccountName=PartyName,
                            Rate=rate,
                            unit=worksheet.Cells[row, 8].Value?.ToString() ?? "",
                            ChallanQty=ChallanQty,
                            pendqty=PendingQty,
                            RecQty=ChallanQty-PendingQty,
                            Amount=PendingQty*rate,
                            //Amount
                            //rec qnt
                            //pendqty 
                            ScrapQty=decimal.TryParse(worksheet.Cells[row, 23].Value?.ToString(), out decimal tempscrapQty) ? tempscrapQty : 0,
                            ScrapItemCode = int.TryParse(worksheet.Cells[row, 17].Value?.ToString(), out int tempScrapItemCode) ? tempScrapItemCode : 0,
                            ScrapPartCode = worksheet.Cells[row, 17].Value?.ToString() ?? "",
                            ScrapItemName = worksheet.Cells[row, 17].Value?.ToString() ?? "",
                            RecItemCode = int.TryParse(worksheet.Cells[row, 9].Value?.ToString(), out int tempRecItemCode) ? tempRecItemCode : 0,
                            RecItemName = worksheet.Cells[row, 9].Value?.ToString() ?? "",
                            RecPartCode = worksheet.Cells[row, 9].Value?.ToString() ?? "",
                            PendScrapToRec = int.TryParse(worksheet.Cells[row, 9].Value?.ToString(), out int tempPendScrapToRec) ? tempPendScrapToRec : 0,
                            BomType=worksheet.Cells[row, 3].Value?.ToString() ?? "",
                            BatchNo=worksheet.Cells[row, 12].Value?.ToString() ?? "",
                            UniqueBatchNo=worksheet.Cells[row, 13].Value?.ToString() ?? "",
                            ProcessId=int.TryParse(worksheet.Cells[row, 11].Value?.ToString(), out int tempProcessId) ? tempProcessId : 0,

                            Closed="N",

                            //challanqty



                        });
                    }

                    if (errors.Count > 0)
                    {
                        return BadRequest(string.Join("\n", errors));
                    }
                }

                MainModel.ItemDetailGrid = JobWorkGridList;
                HttpContext.Session.SetString("KeyJobWorkOpeningGrid", JsonConvert.SerializeObject(JobWorkGridList));
                return PartialView("_JobWorkOpeningGrid", MainModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while processing the Excel file.");
                return StatusCode(500, "An internal server error occurred. Please check the file format.");
            }
        }

    }
}
