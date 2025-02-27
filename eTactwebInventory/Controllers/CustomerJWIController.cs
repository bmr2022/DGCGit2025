using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.DOM.Models.Master;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System.Data;
using System.Net;
using System.Runtime.Caching;
using static eTactWeb.Data.Common.CommonFunc;
using static eTactWeb.DOM.Models.Common;
using static eTactWeb.DOM.Models.JobWorkReceiveModel;

namespace eTactWeb.Controllers
{
    public class CustomerJWIController : Controller
    {
        private readonly IDataLogic _IDataLogic;
        private readonly ILogger<CustomerJWIController> _logger;
        private readonly ICustomerJobWorkIssue _ICustomerJobWorkIssue;
        private readonly IMemoryCache _MemoryCache;
        private readonly IWebHostEnvironment _IWebHostEnvironment;
        private readonly IConfiguration iconfiguration;

        public CustomerJWIController(IDataLogic iDataLogic, ILogger<CustomerJWIController> logger, ICustomerJobWorkIssue iCustomerJobWorkIssue, IMemoryCache memoryCache, IWebHostEnvironment iWebHostEnvironment, IConfiguration iconfiguration)
        {
            _IDataLogic = iDataLogic;
            _logger = logger;
            _ICustomerJobWorkIssue = iCustomerJobWorkIssue;
            _MemoryCache = memoryCache;
            _IWebHostEnvironment = iWebHostEnvironment;
            this.iconfiguration = iconfiguration;
        }

        [Route("{controller}/Index")]
        public async Task<IActionResult> CustomerJWI(int ID, int YearCode, string Mode,
                                             int Account_Code,
                                             string CustomerName,
                                             string CustomerAddress,
                                             string GSTNO,
                                             string State,
                                             string StateCode,
                                             string JWType,
                                             float DistKm,
                                             string VehicleNo,
                                             string TransporterName,
                                             string DispatchThrough,
                                             string DispatchTo,
                                             string ChallanNo,
                                             int CustJwIssEntryId,
                                             string GSTType,
                                             string EntryDate,
                                             string TimeOfRemovel,
                                             string Remark,
                                             string ChallanDate,
                                             string Branch,
                                             string Types)
        {
            ViewData["Title"] = "Customer JWI";
            TempData.Clear();
            _MemoryCache.Remove("KeyCustJWIGrid");

            var MainModel = new CustomerJobWorkIssueModel();
            MainModel.FinFromDate = HttpContext.Session.GetString("FromDate");
            MainModel.FinToDate = HttpContext.Session.GetString("ToDate");
            MainModel.YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
            MainModel.CreatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
            MainModel.Branch = HttpContext.Session.GetString("Branch");
            MainModel.EntryByEmp = HttpContext.Session.GetString("EmpName");
            MainModel.EntryById = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
            MainModel.IssuedByEmpName = HttpContext.Session.GetString("EmpName");

            if (!string.IsNullOrEmpty(Mode) && ID > 0 && (Mode == "V" || Mode == "U"))
            {
                MainModel = await _ICustomerJobWorkIssue.GetViewByID(ID, MainModel.YearCode).ConfigureAwait(false);
                MainModel.Branch = HttpContext.Session.GetString("Branch");
                MainModel.Mode = Mode;
                MainModel.EntryId = ID;
                MainModel.YearCode = YearCode;
                MainModel.EntryDate = EntryDate;
                MainModel.Account_Code = Account_Code;
                MainModel.CustomerName = CustomerName;
                MainModel.CustomerAddress = CustomerAddress;
                MainModel.GSTNO = GSTNO;
                MainModel.State = State;
                MainModel.StateCode = StateCode;
                MainModel.JWType = JWType;
                MainModel.DistKm = DistKm;
                MainModel.VehicleNo = VehicleNo;
                MainModel.TransporterName = TransporterName;
                MainModel.DispatchFrom = DispatchThrough;
                MainModel.DispatchTo = DispatchTo;
                MainModel.Types = Types;
                MainModel.ChallanNo = ChallanNo;
                MainModel.Remark = Remark;
                MainModel.TimeOfRemovel = TimeOfRemovel;
                MainModel.ChallanDate = ChallanDate;
                MainModel = await BindModel(MainModel).ConfigureAwait(false);
                MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                    SlidingExpiration = TimeSpan.FromMinutes(55),
                    Size = 1024,
                };
                _MemoryCache.Set("KeyCustIssue", MainModel, cacheEntryOptions);
                _MemoryCache.Set("CustIssue", MainModel, cacheEntryOptions);
                HttpContext.Session.SetString("CustIssue", JsonConvert.SerializeObject(MainModel));
            }
            else
            {
                MainModel = await BindModel(MainModel);
            }


            return View(MainModel);
        }

        [Route("{controller}/Index")]
        [HttpPost]
        public async Task<IActionResult> CustomerJWI(CustomerJobWorkIssueModel model)
        {
            try
            {
                var JWIGrid = new DataTable();
                var mainmodel2 = model;
                _MemoryCache.TryGetValue("KeyCustJWIGrid", out List<CustomerJobWorkIssueDetail> CustJWIDetail);
                mainmodel2.CustJWIDetailGrid = CustJWIDetail;
                var GIGrid = new DataTable();

                if (CustJWIDetail == null)
                {
                    ModelState.Clear();
                    ModelState.TryAddModelError("GateInwardItemDetail", "Gate Inward Grid Should Have Atleast 1 Item...!");
                    //model = await BindModels(model);
                    return View("CustomerJWI", model);
                }

                else
                {
                    model.CC = HttpContext.Session.GetString("Branch");
                    model.EntryByEmp = HttpContext.Session.GetString("EmpName");
                    model.EntryById = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
                    //model.ActualEnteredBy   = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
                    model.CreatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                    if (model.Mode == "U")
                    {
                        model.UpdatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                        model.UpdatedByEmp = HttpContext.Session.GetString("EmpName");
                        GIGrid = GetDetailTable(CustJWIDetail);
                    }
                    else
                    {
                        GIGrid = GetDetailTable(CustJWIDetail);
                    }

                    var Result = await _ICustomerJobWorkIssue.SaveCustomerJWI(model, GIGrid);

                    if (Result != null)
                    {
                        if (Result.StatusText == "Success" && Result.StatusCode == HttpStatusCode.OK)
                        {
                            ViewBag.isSuccess = true;
                            TempData["200"] = "200";
                            _MemoryCache.Remove(GIGrid);
                        }
                        if (Result.StatusText == "Success" && Result.StatusCode == HttpStatusCode.Accepted)
                        {
                            ViewBag.isSuccess = true;
                            TempData["202"] = "202";
                        }
                        if (Result.StatusText == "Error" && Result.StatusCode == HttpStatusCode.InternalServerError)
                        {
                            var errNum = Result.Result.Message.ToString().Split(":")[1];
                            if (errNum == " 2627")
                            {
                                ViewBag.isSuccess = false;
                                TempData["2627"] = "2627";
                                _logger.LogError("\n \n ********** LogError ********** \n " + JsonConvert.SerializeObject(Result) + "\n \n");
                                //var model2 = await BindModels(null);
                                //model2.FinFromDate = HttpContext.Session.GetString("FromDate");
                                //model2.FinToDate = HttpContext.Session.GetString("ToDate");
                                //model2.YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
                                //model2.CC = HttpContext.Session.GetString("Branch");
                                //model2.PreparedByEmp = HttpContext.Session.GetString("EmpName");
                                //model2.ActualEnteredByName = HttpContext.Session.GetString("EmpName");
                                //model2.CreatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                                //return View(model2);
                            }
                            else
                            {
                                ViewBag.isSuccess = false;
                                TempData["500"] = "500";
                                //var model2 = new CustomerJobWorkIssueModel();
                                //model2.FinFromDate = HttpContext.Session.GetString("FromDate");
                                //model2.FinToDate = HttpContext.Session.GetString("ToDate");
                                //model2.YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
                                //model2.CC = HttpContext.Session.GetString("Branch");
                                //model2.EntryByEmp = HttpContext.Session.GetString("EmpName");
                                //model2.CreatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                                //model2.CustJWIDetailGrid = CustJWIDetail;
                                return View(model);
                            }

                            //ViewBag.isSuccess = false;
                            //TempData["500"] = "500";
                            //_logger.LogError("\n \n ********** LogError ********** \n " + JsonConvert.SerializeObject(Result) + "\n \n");
                            //return View("Error", Result);
                        }
                    }
                    var model1 = new CustomerJobWorkIssueModel();
                    model1.FinFromDate = HttpContext.Session.GetString("FromDate");
                    model1.FinToDate = HttpContext.Session.GetString("ToDate");
                    model1.YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
                    model1.CC = HttpContext.Session.GetString("Branch");
                    model1.EntryByEmp = HttpContext.Session.GetString("EmpName");
                    model1.CreatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                    //return RedirectToAction(nameof(Dashboard));
                    // return RedirectToAction("Index", "GateInward", new { ID = 0 });
                    return View(model1);

                }

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<IActionResult> CustomerJWIDashboard(string FromDate, string ToDate, string ReportType)
        {
            try
            {
                _MemoryCache.Remove("KeyCustJWIGrid");
                var model = new CustJWIssQDashboard();
                ReportType ??= "SUMMARY";

                //FromDate = new DateTime(now.Year, now.Month, 1).ToString("dd/MM/yyyy");
                //ToDate = new DateTime(now.Year + 1, 3, 31).ToString("dd/MM/yyyy");

                var Result = await _ICustomerJobWorkIssue.GetDashboardData(FromDate, ToDate, ReportType).ConfigureAwait(true);
                if (Result != null)
                {
                    var _List = new List<TextValue>();
                    DataSet DS = Result.Result;
                    if (DS != null)
                    {
                        var DT = DS.Tables[0].DefaultView.ToTable(true, "ChallanNo", "ChallanDate", "CustomerName", "CustomerAddress", "CustState",
                            "CustStateCode", "CustJwIssEntryId", "CustJwIssYearCode");
                        model.CustJWIssQDashboard = CommonFunc.DataTableToList<CustJWIssQDashboard>(DT, "CustomerJWI");
                        //foreach (var row in DS.Tables[0].AsEnumerable())
                        //{
                        //    _List.Add(new TextValue()
                        //    {
                        //        Text = row["ChallanNo"].ToString(),
                        //        Value = row["ChallanNo"].ToString()
                        //    });
                        //}
                        var _ChallanList = _List.DistinctBy(x => x.Value).ToList();
                        model.ChallanList = _ChallanList;
                        _List = new List<TextValue>();
                    }
                    //if (Flag != "True")
                    //{
                    //    model.FromDate1 = FromDate;
                    //    model.ToDate1 = ToDate;
                    //    model.Reporttype = ReportType;
                    //    model.Account_Code = AccountCode;
                    //    model.ChallanNo = ChallanNo;
                    //    model.PartCode = PartCode;
                    //    model.ItemName = ItemName;
                    //}
                }
                return View(model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IActionResult> GetSummaryDetailData(string FromDate, string ToDate, string ReportType)
        {
            var model = new CustJWIssQDashboard();
            model = await _ICustomerJobWorkIssue.GetDashboardDetailsData(FromDate, ToDate, ReportType);
            //model.Reporttype = "Detail";
            if (ReportType == "SUMMARY")
            {
                return PartialView("_CustJobworkDashboardSummaryGrid", model);
            }
            else
            {
                // You can perform some other logic here if needed, or return the same partial view.
                return PartialView("_CustomerJwIssDetailDashBoardGrid", model);
            }
            return null;
        }


        private static DataTable GetDetailTable(IList<CustomerJobWorkIssueDetail> DetailList)
        {
            var JWIGrid = new DataTable();

            JWIGrid.Columns.Add("SeqNo", typeof(long));
            JWIGrid.Columns.Add("CustJwIssEntryId", typeof(long));
            JWIGrid.Columns.Add("CustJwIssYearCode", typeof(long));
            JWIGrid.Columns.Add("producedUnproduced", typeof(char));
            JWIGrid.Columns.Add("SONO", typeof(string));
            JWIGrid.Columns.Add("CustOrderNo", typeof(string));
            JWIGrid.Columns.Add("SOyearCode", typeof(long));
            JWIGrid.Columns.Add("SODate", typeof(DateTime));
            JWIGrid.Columns.Add("SOAmmNo", typeof(string));
            JWIGrid.Columns.Add("SOAmmDate", typeof(DateTime));
            JWIGrid.Columns.Add("SchNo", typeof(string));
            JWIGrid.Columns.Add("SchYearCode", typeof(long));
            JWIGrid.Columns.Add("SchDate", typeof(DateTime));
            JWIGrid.Columns.Add("ItemCode", typeof(long));
            JWIGrid.Columns.Add("Batchno", typeof(string));
            JWIGrid.Columns.Add("uniquebatchno", typeof(string));
            JWIGrid.Columns.Add("processid", typeof(long));
            JWIGrid.Columns.Add("StoreId", typeof(long));
            JWIGrid.Columns.Add("Qty", typeof(float));
            JWIGrid.Columns.Add("Unit", typeof(string));
            JWIGrid.Columns.Add("PendQty", typeof(float));
            JWIGrid.Columns.Add("NoofCase", typeof(float));
            JWIGrid.Columns.Add("Rate", typeof(float));
            JWIGrid.Columns.Add("Amount", typeof(float));
            JWIGrid.Columns.Add("Discountper", typeof(float));
            JWIGrid.Columns.Add("DiscountAmt", typeof(float));
            JWIGrid.Columns.Add("BatchStock", typeof(float));
            JWIGrid.Columns.Add("TotalStock", typeof(float));
            JWIGrid.Columns.Add("ItemSize", typeof(string));
            JWIGrid.Columns.Add("PacketsDetail", typeof(string));
            JWIGrid.Columns.Add("OtherDetail", typeof(string));
            JWIGrid.Columns.Add("HSSNO", typeof(string));
            JWIGrid.Columns.Add("BOMInd", typeof(string));
            JWIGrid.Columns.Add("ChallanAdjustRate", typeof(float));
            JWIGrid.Columns.Add("StdPacking", typeof(float));
            JWIGrid.Columns.Add("color", typeof(string));
            JWIGrid.Columns.Add("BOMNO", typeof(long));
            JWIGrid.Columns.Add("BOMname", typeof(string));
            JWIGrid.Columns.Add("Bomdate", typeof(DateTime));
            JWIGrid.Columns.Add("AltUnit", typeof(string));
            JWIGrid.Columns.Add("AltQty", typeof(float));

            foreach (var item in DetailList)
            {
                JWIGrid.Rows.Add(
                item.SEQNo == 0 ? (object)0 : (object)item.SEQNo,
                item.CustJwIssEntryId == 0 ? (object)0 : (object)item.CustJwIssEntryId,
                item.CustJwIssYearCode == 0 ? (object)0 : (object)item.CustJwIssYearCode,
                string.IsNullOrEmpty(item.ProduceUnproduce) ? (object)"" : (object)item.ProduceUnproduce,
                item.SONO == 0 ? (object)"" : (object)item.SONO,  // Fix: SONO is a string
                string.IsNullOrEmpty(item.CustOrderNo) ? (object)"" : (object)item.CustOrderNo,
                item.SOYear == 0 ? (object)0 : (object)item.SOYear,
                string.IsNullOrEmpty(item.SoDate) ? (object)DBNull.Value : Convert.ToDateTime(item.SoDate),
                string.IsNullOrEmpty(item.SOAmmNo) ? (object)"" : (object)item.SOAmmNo,
                string.IsNullOrEmpty(item.SOAmmDate) ? (object)DBNull.Value : Convert.ToDateTime(item.SOAmmDate),
                string.IsNullOrEmpty(item.SchNo) ? (object)"" : (object)item.SchNo,
                item.SchYearcode == 0 ? (object)0 : (object)item.SchYearcode,
                string.IsNullOrEmpty(item.SchDate) ? (object)DBNull.Value : Convert.ToDateTime(item.SchDate),
                item.ItemCode == 0 ? (object)0 : (object)item.ItemCode,
                string.IsNullOrEmpty(item.BatchNo) ? (object)"" : (object)item.BatchNo,
                string.IsNullOrEmpty(item.UNiqueBatchNo) ? (object)"" : (object)item.UNiqueBatchNo,
                item.ProcessId == 0 ? (object)0 : (object)item.ProcessId,
                item.StoreId == 0 ? (object)0 : (object)item.StoreId,
                item.Qty == 0 ? (object)0 : (object)item.Qty,
                string.IsNullOrEmpty(item.Unit) ? (object)"" : (object)item.Unit,
                item.PendQty == 0 ? (object)0 : (object)item.PendQty,
                item.NoOfCases == 0 ? (object)0 : (object)item.NoOfCases,
                item.Rate == 0 ? (object)0 : (object)item.Rate,
                item.ItemAmount == 0 ? (object)0 : (object)item.ItemAmount,
                item.Discountper == 0 ? (object)0 : (object)item.Discountper,
                item.DiscountAmt == 0 ? (object)0 : (object)item.DiscountAmt,
                item.BatchStock == 0 ? (object)0 : (object)item.BatchStock,
                item.TotalStock == 0 ? (object)0 : (object)item.TotalStock,
                string.IsNullOrEmpty(item.ItemSize) ? (object)"" : (object)item.ItemSize,
                string.IsNullOrEmpty(item.PacketsDetail) ? (object)"" : (object)item.PacketsDetail,
                string.IsNullOrEmpty(item.OtherDetail) ? (object)"" : (object)item.OtherDetail,
                string.IsNullOrEmpty(item.HSNNo) ? (object)"" : (object)item.HSNNo,
                string.IsNullOrEmpty(item.BOMInd) ? (object)"" : (object)item.BOMInd,
                item.ChallanAdjustRate == 0 ? (object)0 : (object)item.ChallanAdjustRate,
                item.StdPacking == 0 ? (object)0 : (object)item.StdPacking,
                string.IsNullOrEmpty(item.color) ? (object)"" : (object)item.color,
                item.BOMNO == 0 ? (object)0 : (object)item.BOMNO,
                string.IsNullOrEmpty(item.BOMname) ? (object)"" : (object)item.BOMname,
                string.IsNullOrEmpty(item.Bomdate) ? (object)DBNull.Value : Convert.ToDateTime(item.Bomdate),
                string.IsNullOrEmpty(item.AltUnit) ? (object)"" : (object)item.AltUnit,
                item.AltQty == 0 ? (object)0 : (object)item.AltQty
                 );
            }

            return JWIGrid;
        }
        
        public IActionResult GetAdjustedChallanDetailsData1(List<CustomerJobWorkIssueModel> model)
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> GetAdjustedChallanDetailsData(List<CustomerJobWorkIssueModel> model, int YearCode, string EntryDate, string ChallanDate, int AccountCode)
        {
            try
            {
                if (model == null || !model.Any())
                {
                    return Json(new { success = false, message = "No data received." });
                }

                var result = _ICustomerJobWorkIssue.GetAdjustedChallanDetailsData(model, YearCode, EntryDate, ChallanDate, AccountCode);

                return PartialView("_CustomerJwisschallanAdjustment", result);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        //public async Task<JsonResult> GetAdjustedChallanDetailsData(string model,int YearCode, string EntryDate, string ChallanDate)
        //{
        //    try
        //    {
        //        var ChallanGrid = new DataTable();
        //        List<CustomerJobWorkIssueDetail> Challandetails = JsonConvert.DeserializeObject<List<CustomerJobWorkIssueDetail>>(model);
        //        ChallanGrid = GetAdjustedChallanTable(Challandetails);
        //        var JSON = await _ICustomerJobWorkIssue.GetAdjustedChallanDetailsData( YearCode,  EntryDate,  ChallanDate, ChallanGrid);
        //        string JsonString = JsonConvert.SerializeObject(JSON);
        //        List<CustomerJobWorkIssueDetail> JobWorkReceiveDetail = new List<CustomerJobWorkIssueDetail>();

        //        foreach (DataRow row in JSON.Result.Rows)
        //        {
        //            CustomerJobWorkIssueDetail jobWorkRec = new CustomerJobWorkIssueDetail
        //            {
        //                //EntryIdIssJw = row["EntryIdIssJw"] != DBNull.Value ? Convert.ToInt32(row["EntryIdIssJw"]) : 0,
        //                //IssChallanNo = row["IssJWChallanNo"]?.ToString(),
        //                //IssYearCode = row["IssYearCode"] != DBNull.Value ? Convert.ToInt32(row["IssYearCode"]) : 0,
        //                //IssChallanDate = row["ChallanDate"]?.ToString(),
        //                //IssPartCode = row["IssPartCode"]?.ToString(),
        //                //IssItemName = row["IssItemName"]?.ToString(),
        //                //ItemCode = row["IssItemCode"] != DBNull.Value ? Convert.ToInt32(row["IssItemCode"]) : 0,
        //                //BOMrevno = row["BomNo"] != DBNull.Value ? Convert.ToInt32(row["BomNo"]) : 0,
        //                //BOMRevDate = row["BOMDate"]?.ToString(),
        //                //BOMInd = row["BomStatus"]?.ToString(),
        //                //PendQty = row["PendQty"] != DBNull.Value ? Convert.ToDecimal(row["PendQty"]) : 0,
        //                //FinishPartCode = row["FinishPartcode"]?.ToString(),
        //                //FinishItemName = row["FinishItemName"]?.ToString(),
        //                //FinishItemCode = row["RecItemCode"] != DBNull.Value ? Convert.ToInt32(row["RecItemCode"]) : 0,
        //                //BOMQty = row["bomqty"] != DBNull.Value ? Convert.ToDecimal(row["bomqty"]) : 0,

        //                //AdjQty = row["ActualAdjQty"] != DBNull.Value ? Convert.ToDecimal(row["ActualAdjQty"]) : 0,
        //                //Through = row["through"]?.ToString(),
        //                ////TotaladjQty = Convert.ToDecimal(row["QtyToBeRec"]),
        //                //TotaladjQty = row["ActualAdjQty"] != DBNull.Value ? Convert.ToDecimal(row["ActualAdjQty"]) : 0,
        //                //TotalIssuedQty = row["ActualAdjQty"] != DBNull.Value ? Convert.ToDecimal(row["ActualAdjQty"]) : 0,
        //                //IssuedBatchNO = row["batchno"]?.ToString(),
        //                //IssuedUniqueBatchNo = row["uniquebatchno"]?.ToString(),
        //                //RecQty = row["RecQty"] != DBNull.Value ? Convert.ToDecimal(row["RecQty"]) : 0,
        //                //TotalRecQty = row["RecQty"] != DBNull.Value ? Convert.ToDecimal(row["RecQty"]) : 0,
        //                //YearCodeIssJw = row["IssYearCode"] != null ? Convert.ToInt32(row["IssYearCode"]) : 0
        //            };

        //            JobWorkReceiveDetail.Add(jobWorkRec);
        //        }
        //        var dataresult = AddChallanDetail2Grid(JobWorkReceiveDetail);

        //        return Json(JsonString);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw;
        //    }
        //}
        private static DataTable GetAdjustedChallanTable(List<CustomerJobWorkIssueDetail> DTTItemGrid)
        {
            try
            {
                var ChallanGrid = new DataTable();
                //ChallanGrid.Columns.Add("SeqNo", typeof(int));
                ChallanGrid.Columns.Add("EntryDate", typeof(DateTime));
                ChallanGrid.Columns.Add("CustJwRecEntryId", typeof(long));
                ChallanGrid.Columns.Add("CustJWRecYearCode", typeof(long));
                ChallanGrid.Columns.Add("CustJWRecChallanNo", typeof(string));
                ChallanGrid.Columns.Add("CustJWRecEntryDate", typeof(DateTime));
                ChallanGrid.Columns.Add("RecItemcode", typeof(long));
                ChallanGrid.Columns.Add("CustJWIssEntryid", typeof(long));
                ChallanGrid.Columns.Add("CustJWIssYearCode", typeof(long));
                ChallanGrid.Columns.Add("CustJWIssChallanNo", typeof(string));
                ChallanGrid.Columns.Add("CustJWIssChallanDate", typeof(DateTime));
                ChallanGrid.Columns.Add("AccountCode", typeof(long));
                ChallanGrid.Columns.Add("FinishItemCode", typeof(long));
                ChallanGrid.Columns.Add("AdjQty", typeof(float));
                ChallanGrid.Columns.Add("CC", typeof(string));
                ChallanGrid.Columns.Add("UID", typeof(long));
                ChallanGrid.Columns.Add("AdjFormType", typeof(string));
                ChallanGrid.Columns.Add("TillDate", typeof(DateTime));
                ChallanGrid.Columns.Add("TotalSQty", typeof(float));
                ChallanGrid.Columns.Add("PendQty", typeof(float));
                ChallanGrid.Columns.Add("BOMQty", typeof(float));
                ChallanGrid.Columns.Add("BOMRecDate", typeof(DateTime));
                ChallanGrid.Columns.Add("ProcessID", typeof(long));
                ChallanGrid.Columns.Add("BOMInd", typeof(string));
                ChallanGrid.Columns.Add("IssQty", typeof(float));
                ChallanGrid.Columns.Add("TotalAdjQty", typeof(float));
                ChallanGrid.Columns.Add("TotalIssQty", typeof(float));
                ChallanGrid.Columns.Add("TotalRecQty", typeof(float));
                ChallanGrid.Columns.Add("RunnerItemCode", typeof(long));
                ChallanGrid.Columns.Add("ScrapItemCode", typeof(long));
                ChallanGrid.Columns.Add("IdealScrapQty", typeof(float));
                ChallanGrid.Columns.Add("IssuedScrapQty", typeof(float));
                ChallanGrid.Columns.Add("PreRecChallanNo", typeof(string));
                ChallanGrid.Columns.Add("ScrapQtyAgainstRecQty", typeof(float));
                ChallanGrid.Columns.Add("Recbatchno", typeof(string));
                ChallanGrid.Columns.Add("Recuniquebatchno", typeof(string));
                ChallanGrid.Columns.Add("Issbatchno", typeof(string));
                ChallanGrid.Columns.Add("Issuniquebatchno", typeof(string));
                ChallanGrid.Columns.Add("ScrapAdjusted", typeof(string));

                foreach (var Item in DTTItemGrid)
                {
                    ChallanGrid.Rows.Add(
                        new object[]
                        {
                    Item.EntryDate,
                    Item.CustJwRecEntryId,
                    Item.CustJWRecYearCode,
                    Item.CustJWRecChallanNo,
                    Item.CustJWRecEntryDate,
                    Item.RecItemcode,
                    Item.CustJWIssEntryid,
                    Item.CustJWIssYearCode,
                    Item.CustJWIssChallanNo,
                    Item.CustJWIssChallanDate,
                    Item.AccountCode,
                    Item.FinishItemCode,
                    Item.AdjQty,
                    Item.CC,
                    Item.UID,
                    Item.AdjFormType,
                    Item.TillDate,
                    Item.TotalSQty,
                    Item.PendQty,
                    Item.BOMQty,
                    Item.BOMRecDate,
                    Item.ProcessID,
                    Item.BOMInd,
                    Item.IssQty,
                    Item.TotalAdjQty,
                    Item.TotalIssQty,
                    Item.TotalRecQty,
                    Item.RunnerItemCode,
                    Item.ScrapItemCode,
                    Item.IdealScrapQty,
                    Item.IssuedScrapQty,
                    Item.PreRecChallanNo,
                    Item.ScrapQtyAgainstRecQty,
                    Item.Recbatchno,
                    Item.Recuniquebatchno,
                    Item.Issbatchno,
                    Item.Issuniquebatchno,
                    Item.ScrapAdjusted

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
        public async Task<JsonResult> GetPopUpData(int YearCode, string EntryDate, string ChallanDate, int AccountCode, string prodUnProd, string BOMINd, int RMItemCode, string Partcode)
        {
            var JSON = await _ICustomerJobWorkIssue.GetPopUpData( YearCode,  EntryDate,  ChallanDate,  AccountCode,  prodUnProd,  BOMINd,  RMItemCode,  Partcode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public IActionResult AddChallanDetail2Grid(List<CustomerJobWorkIssueDetail> model)
        {
            try
            {
                var MainModel = new CustomerJobWorkIssueModel();
                var JobWorkReceiveGrid = new List<CustomerJobWorkIssueDetail>();
                var JobReceiveGrid = new List<CustomerJobWorkIssueDetail>();
                var SSGrid = new List<CustomerJobWorkIssueDetail>();
                MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                    SlidingExpiration = TimeSpan.FromMinutes(55),
                    Size = 1024,
                };
                var seqNo = 0;
                foreach (var item in model)
                {
                    _MemoryCache.TryGetValue("KeyCustJWIGrid", out IList<CustomerJobWorkIssueDetail> CustomerJobWorkIssueDetail);
                    if (item != null)
                    {
                        if (CustomerJobWorkIssueDetail == null)
                        {
                            item.SEQNo += seqNo + 1;
                            JobWorkReceiveGrid.Add(item);
                            seqNo++;
                        }
                        else
                        {
                            item.SEQNo = CustomerJobWorkIssueDetail.Count + 1;
                            JobWorkReceiveGrid = CustomerJobWorkIssueDetail.Where(x => x != null).ToList();
                            SSGrid.AddRange(JobWorkReceiveGrid);
                            JobWorkReceiveGrid.Add(item);
                        }
                        MainModel.CustJWIDetailGrid = JobWorkReceiveGrid;

                        _MemoryCache.Set("KeyCustJWIGrid", MainModel.CustJWIDetailGrid, cacheEntryOptions);
                    }
                }
                return PartialView("_CustomerJWIDetailGrid", MainModel);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IActionResult AddCustJWIDetail(CustomerJobWorkIssueDetail model)
        {
            try
            {
                // Retrieve the existing grid data from memory cache
                _MemoryCache.TryGetValue("KeyCustJWIGrid", out IList<CustomerJobWorkIssueDetail> GridDetail);

                var MainModel = new CustomerJobWorkIssueModel();
                var SSGrid = new List<CustomerJobWorkIssueDetail>();

                // If no data is found in the cache, initialize with the new item
                if (model != null)
                {
                    if (GridDetail == null)
                    {
                        model.SEQNo = 1;
                        SSGrid.Add(model);
                    }
                    else
                    {
                        if (GridDetail.Any(x => x.SEQNo == model.SEQNo))
                        {
                            return StatusCode(207, "Duplicate");
                        }
                        else
                        {
                            model.SEQNo = GridDetail.Count + 1;

                            SSGrid.AddRange(GridDetail);
                            SSGrid.Add(model);
                        }
                    }
                    MainModel.CustJWIDetailGrid = SSGrid;
                    MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
                    {
                        AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                        SlidingExpiration = TimeSpan.FromMinutes(55),
                        Size = 1024,
                    };
                    _MemoryCache.Set("KeyCustJWIGrid", SSGrid, cacheEntryOptions);
                }
                else
                {
                    ModelState.TryAddModelError("Error", "Schedule List Cannot Be Empty...!");
                }

                // Return the updated grid view
                return PartialView("_CustomerJWIDetailGrid", MainModel);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IActionResult EditItemRow(int SEQNo)
        {
            var model = new CustomerJobWorkIssueModel();
            _MemoryCache.TryGetValue("KeyCustJWIGrid", out List<CustomerJobWorkIssueDetail> ReturnFromDepartmentDetail);
            var SSGrid = ReturnFromDepartmentDetail.Where(x => x.SEQNo == SEQNo);
            string JsonString = JsonConvert.SerializeObject(SSGrid);
            return Json(JsonString);
        }

        public async Task<IActionResult> DeleteItemRow(int SEQNo)
        {
            var MainModel = new CustomerJobWorkIssueModel();
            _MemoryCache.TryGetValue("KeyCustJWIGrid", out List<CustomerJobWorkIssueDetail> RequisitionDetail);
            int Indx = Convert.ToInt32(SEQNo) - 1;

            if (RequisitionDetail != null && RequisitionDetail.Count > 0)
            {
                RequisitionDetail.RemoveAt(Convert.ToInt32(Indx));

                Indx = 0;

                foreach (var item in RequisitionDetail)
                {
                    Indx++;
                    item.SEQNo = Indx;
                }
                MainModel.CustJWIDetailGrid = RequisitionDetail;

                MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                    SlidingExpiration = TimeSpan.FromMinutes(55),
                    Size = 1024,
                };


                if (RequisitionDetail.Count == 0)
                {
                    _MemoryCache.Remove("KeyCustJWIGrid");
                }
            }
            return PartialView("_CustomerJWIDetailGrid", MainModel);
        }

        public async Task<JsonResult> NewEntryId(int yearCode)
        {
            var JSON = await _ICustomerJobWorkIssue.GetNewEntry(yearCode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }

        public async Task<JsonResult> FillCustomers(int yearCode)
        {
            var JSON = await _ICustomerJobWorkIssue.GetCustomers(yearCode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetAddressDetails(int AccountCode)
        {
            var JSON = await _ICustomerJobWorkIssue.GetCustomerDetails(AccountCode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FILLSONO(string ChallanDate, int AccountCode)
        {
            var JSON = await _ICustomerJobWorkIssue.FillSoNoDetails(ChallanDate, AccountCode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillSOSchedule(string ChallanDate, int AccountCode, int SoNo, int SoNoYearCode)
        {
            var JSON = await _ICustomerJobWorkIssue.FillSOSchedule(ChallanDate, AccountCode, SoNo, SoNoYearCode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetDistance(int AccountCode)
        {
            var JSON = await _ICustomerJobWorkIssue.GetDistanceData(AccountCode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillSoNoYearCode(int AccountCode, int SoNo, string ChallanDate)
        {
            var JSON = await _ICustomerJobWorkIssue.FillSoNoYearCode(AccountCode, SoNo, ChallanDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillCustOrderNo(int AccountCode, int SoNo, string ChallanDate, int SoNoYearCode)
        {
            var JSON = await _ICustomerJobWorkIssue.FillCustOrderNo(AccountCode, SoNo, ChallanDate, SoNoYearCode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }

        public async Task<JsonResult> FillScheduleNoAndYear(int AccountCode, int SoNo, string ChallanDate, int SoNoYearCode)
        {
            var JSON = await _ICustomerJobWorkIssue.FillScheduleNoAndYear(AccountCode, SoNo, ChallanDate, SoNoYearCode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillPartCode(int yearCode)
        {
            var JSON = await _ICustomerJobWorkIssue.FillPartCode(yearCode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillItems(int yearCode)
        {
            var JSON = await _ICustomerJobWorkIssue.FillItemList(yearCode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetStoreList()
        {
            var JSON = await _ICustomerJobWorkIssue.GetStoreList();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<IActionResult> DeleteByID(int CustJwIssEntryId, int CustJwIssYearCode, string EntryMachineName, int EntryById,string ActualEntryDate,int Account_Code)
        {
            var Result = await _ICustomerJobWorkIssue.DeleteByID(CustJwIssEntryId, CustJwIssYearCode, EntryMachineName, EntryById, ActualEntryDate, Account_Code);

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

            return RedirectToAction("CustomerJWIDashboard");

        }
      
        private async Task<CustomerJobWorkIssueModel> BindModel(CustomerJobWorkIssueModel model)
        {
            model.CustomerList = await _IDataLogic.GetDropDownList("FillCustomerList", "SP_CustomerJobworkIssueMainDetail");
            return model;
        }

    }
}
