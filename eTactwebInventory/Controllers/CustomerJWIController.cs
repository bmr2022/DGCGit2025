using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.DOM.Models.Master;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using OfficeOpenXml.Drawing.Chart;
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
        private readonly IWebHostEnvironment _IWebHostEnvironment;
        private readonly IConfiguration iconfiguration;

        public CustomerJWIController(IDataLogic iDataLogic, ILogger<CustomerJWIController> logger, ICustomerJobWorkIssue iCustomerJobWorkIssue, IWebHostEnvironment iWebHostEnvironment, IConfiguration iconfiguration)
        {
            _IDataLogic = iDataLogic;
            _logger = logger;
            _ICustomerJobWorkIssue = iCustomerJobWorkIssue;
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
            HttpContext.Session.Remove("KeyCustJWIGrid");
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
                string serializedKeyGrid = JsonConvert.SerializeObject(MainModel);
                HttpContext.Session.SetString("KeyCustIssue", serializedKeyGrid);
                string serializedGrid = JsonConvert.SerializeObject(MainModel.CustJWIDetailGrid);
                HttpContext.Session.SetString("CustIssue", serializedGrid);
                HttpContext.Session.SetString("KeyCustJWIGrid", serializedGrid);
            }
            else
            {
                MainModel = await BindModel(MainModel);
            }


            return View(MainModel);
        }

        public async Task<JsonResult> GetFormRights()
        {
            var userID = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
            var JSON = await _ICustomerJobWorkIssue.GetFormRights(userID);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }

        [Route("{controller}/Index")]
        [HttpPost]
        public async Task<IActionResult> CustomerJWI(CustomerJobWorkIssueModel model)
        {
            try
            {
                var JWIGrid = new DataTable();
                var ChallanGrid = new DataTable();
                var mainmodel2 = model;
                string modelJson = HttpContext.Session.GetString("KeyCustJWIGrid");
                List<CustomerJobWorkIssueDetail> CustJWIDetail = new List<CustomerJobWorkIssueDetail>();
                if (!string.IsNullOrEmpty(modelJson))
                {
                    CustJWIDetail = JsonConvert.DeserializeObject<List<CustomerJobWorkIssueDetail>>(modelJson);
                }
                string modelCWIJson = HttpContext.Session.GetString("KeyCWIAdjustGrid");
                List<CustomerJobWorkIssueDetail> CWIAdjustDetail = new List<CustomerJobWorkIssueDetail>();
                if (!string.IsNullOrEmpty(modelCWIJson))
                {
                    CWIAdjustDetail = JsonConvert.DeserializeObject<List<CustomerJobWorkIssueDetail>>(modelCWIJson);
                }

                mainmodel2.CustJWIDetailGrid = CustJWIDetail;
                var GIGrid = new DataTable();

                if (CustJWIDetail == null&& CWIAdjustDetail==null)
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
                        ChallanGrid = GetChallanTable(CWIAdjustDetail);
                    }
                    else
                    {
                        GIGrid = GetDetailTable(CustJWIDetail);
                        ChallanGrid = GetChallanTable(CWIAdjustDetail);
                    }

                    var Result = await _ICustomerJobWorkIssue.SaveCustomerJWI(model, GIGrid, ChallanGrid);

                    if (Result != null)
                    {
                        if (Result.StatusText == "Success" && Result.StatusCode == HttpStatusCode.OK)
                        {
                            ViewBag.isSuccess = true;
                            TempData["200"] = "200";
                            HttpContext.Session.Remove("KeyCustJWIGrid");
                            HttpContext.Session.Remove("KeyCWIAdjustGrid");
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
                            }
                            else
                            {
                                ViewBag.isSuccess = false;
                                TempData["500"] = "500";
                                return View(model);
                            }
                        }
                    }
                    var model1 = new CustomerJobWorkIssueModel();
                    model1.FinFromDate = HttpContext.Session.GetString("FromDate");
                    model1.FinToDate = HttpContext.Session.GetString("ToDate");
                    model1.YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
                    model1.CC = HttpContext.Session.GetString("Branch");
                    model1.EntryByEmp = HttpContext.Session.GetString("EmpName");
                    model1.CreatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                    return View(model1);

                }

            }
            catch (Exception ex)
            {
                throw;
            }
        }
        private static DataTable GetChallanTable(IList<CustomerJobWorkIssueDetail> DetailList)
        {
            var ChallanGrid = new DataTable();

            ChallanGrid.Columns.Add("EntryDate", typeof(DateTime));
            ChallanGrid.Columns.Add("CustJwRecEntryId", typeof(int));
            ChallanGrid.Columns.Add("CustJwRecYearCode", typeof(int));
            ChallanGrid.Columns.Add("CustJwRecChallanNo", typeof(string));
            ChallanGrid.Columns.Add("CustJwRecEntryDate", typeof(DateTime));
            ChallanGrid.Columns.Add("RecItemCode", typeof(int));
            ChallanGrid.Columns.Add("CustJwIssEntryid", typeof(int));
            ChallanGrid.Columns.Add("CustJwIssYearCode", typeof(int));
            ChallanGrid.Columns.Add("CustJwIssChallanNo", typeof(string));
            ChallanGrid.Columns.Add("CustJwIssChallanDate", typeof(DateTime));
            ChallanGrid.Columns.Add("AccountCode", typeof(long));
            ChallanGrid.Columns.Add("FinishItemCode", typeof(long));
            ChallanGrid.Columns.Add("AdjQty", typeof(float));
            ChallanGrid.Columns.Add("CC", typeof(string));
            ChallanGrid.Columns.Add("UID", typeof(long));
            ChallanGrid.Columns.Add("AdjFromType", typeof(string));
            ChallanGrid.Columns.Add("TillDate", typeof(DateTime));
            ChallanGrid.Columns.Add("TotIssQty", typeof(float));
            ChallanGrid.Columns.Add("PendQty", typeof(float));
            ChallanGrid.Columns.Add("BOMQty", typeof(float));
            ChallanGrid.Columns.Add("BomRevNo", typeof(long));
            ChallanGrid.Columns.Add("BOMRevDate", typeof(DateTime));
            ChallanGrid.Columns.Add("ProcessID", typeof(long));
            ChallanGrid.Columns.Add("BOMInd", typeof(char));
            ChallanGrid.Columns.Add("IssQty", typeof(float));
            ChallanGrid.Columns.Add("TotalAdjQty", typeof(float));
            ChallanGrid.Columns.Add("TotalIssQty", typeof(float));
            ChallanGrid.Columns.Add("TotalRecQty", typeof(float));
            ChallanGrid.Columns.Add("RunnerItemCode", typeof(long));
            ChallanGrid.Columns.Add("ScrapItemCode", typeof(long));
            ChallanGrid.Columns.Add("IdealScrapQty", typeof(float));
            ChallanGrid.Columns.Add("IssuedScrapQty", typeof(float));
            ChallanGrid.Columns.Add("PreRecChallanNo", typeof(string));
            ChallanGrid.Columns.Add("ScrapAgainstRecQty", typeof(float));
            ChallanGrid.Columns.Add("Recbatchno", typeof(string));
            ChallanGrid.Columns.Add("RecuniqueBatchno", typeof(string));
            ChallanGrid.Columns.Add("Issbatchno", typeof(string));
            ChallanGrid.Columns.Add("IssueuniqueBatchno", typeof(string));
            ChallanGrid.Columns.Add("ScrapAdjusted", typeof(string));

          
            if (DetailList == null || DetailList.Count == 0)
                return ChallanGrid;

            foreach (var Item in DetailList)
            {
                if (Item == null) continue; 

                ChallanGrid.Rows.Add(
                    DateTime.Now,//Item.EntryDate ,
                    Item.CustJwRecEntryId ?? 0,
                    Item.CustJWRecYearCode ?? 0,
                    Item.CustJWRecChallanNo ?? string.Empty,
                    DateTime.Now,//Item.CustJWRecEntryDate ?? DateTime.MinValue,
                    Item.RecItemcode ?? 0,
                    Item.CustJwIssEntryId ?? 0,
                    Item.CustJwIssYearCode ?? 0,
                    Item.CustJWIssChallanNo ?? string.Empty,
                    DateTime.Now,//Item.CustJWIssChallanDate ?? DateTime.MinValue,
                    Item.AccountCode ?? 0L,
                    Item.FinishItemCode ?? 0L,
                    Item.AdjQty ?? 0.0f,
                    Item.CC ?? string.Empty,
                    Item.UID ?? 0L,
                    Item.AdjFormType ?? string.Empty,
                    DateTime.Now,//Item.TillDate ?? DateTime.MinValue,
                    Item.TotalSQty ?? 0.0f,
                    Item.PendQty ?? 0.0f,
                    Item.BOMQty ?? 0.0f,
                    Item.BomRevNo ?? 0L,
                    Item.Bomdate ?? string.Empty,
                    Item.ProcessId ,
                    Item.BOMInd == null || Item.BOMInd == '\0' ? ' ' : Item.BOMInd, 
                    Item.IssQty ?? 0.0f,
                    Item.TotalAdjQty ?? 0.0f,
                    Item.TotalIssQty ?? 0.0f,
                    Item.TotalRecQty ?? 0.0f,
                    Item.RunnerItemCode ?? 0L,
                    Item.ScrapItemCode ?? 0L,
                    Item.IdealScrapQty ?? 0.0f,
                    Item.IssuedScrapQty ?? 0.0f,
                    Item.PreRecChallanNo ?? string.Empty,
                    Item.ScrapQtyAgainstRecQty ?? 0.0f,
                    Item.Recbatchno ?? string.Empty,
                    Item.Recuniquebatchno ?? string.Empty,
                    Item.Issbatchno ?? string.Empty,
                    Item.Issuniquebatchno ?? string.Empty,
                    Item.ScrapAdjusted ?? string.Empty
                );
            }

            return ChallanGrid;
        }

        public async Task<IActionResult> CustomerJWIDashboard(string FromDate, string ToDate, string ReportType)
        {
            try
            {
                HttpContext.Session.Remove("KeyCustJWIGrid");
                var model = new CustJWIssQDashboard();
                ReportType ??= "SUMMARY";

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
                        var _ChallanList = _List.DistinctBy(x => x.Value).ToList();
                        model.ChallanList = _ChallanList;
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
        public async Task<IActionResult> GetSummaryDetailData(string FromDate, string ToDate, string ReportType)
        {
            var model = new CustJWIssQDashboard();
            model = await _ICustomerJobWorkIssue.GetDashboardDetailsData(FromDate, ToDate, ReportType);
            //model.Reporttype = "Detail";
            MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                SlidingExpiration = TimeSpan.FromMinutes(55),
                Size = 1024,
            };
            string serializedGrid = JsonConvert.SerializeObject(model.CustJWIssQDashboardGrid);
            HttpContext.Session.SetString("KeyCustJWIList", serializedGrid);
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
        [HttpGet]
        public IActionResult GetCustJWIDashBoardGridData()
        {
            string modelJson = HttpContext.Session.GetString("KeyCustJWIList");
            List<CustJWIssQDashboard> custJWIList = new List<CustJWIssQDashboard>();
            if (!string.IsNullOrEmpty(modelJson))
            {
                custJWIList = JsonConvert.DeserializeObject<List<CustJWIssQDashboard>>(modelJson);
            }

            return Json(custJWIList);
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
            JWIGrid.Columns.Add("BOMInd", typeof(char));
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
                //string.IsNullOrEmpty(item.ProduceUnproduce) ? (object)"" : (object)item.ProduceUnproduce,
                string.IsNullOrEmpty(item.ProduceUnproduce) ? (object)' ' : (object)item.ProduceUnproduce[0],

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
              //  item.Qty == 0 ? (object)0 : (object)item.Qty,
               item.ChallanQty == 0 ? 0 : item.ChallanQty,
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
                //string.IsNullOrEmpty(item.BOMInd.ToString()) ? (object)DBNull.Value : (object)item.BOMInd,
                //item.BOMInd == '\0' ? (object)DBNull.Value : (object)item.BOMInd,
                item.BOMInd == '\0' ? ' ' : item.BOMInd,
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

        public async Task<JsonResult> GetAdjustedChallanDetailsData(string model,int AccountCode, int YearCode, string EntryDate, string ChallanDate)
        {
            try
            {
                var ChallanGrid = new DataTable();
                List<CustomerJobWorkIssueDetail> Challandetails = JsonConvert.DeserializeObject<List<CustomerJobWorkIssueDetail>>(model);
                ChallanGrid = GetAdjustedChallanTable(Challandetails);
                var JSON = await _ICustomerJobWorkIssue.GetAdjustedChallanDetailsData(YearCode, EntryDate, ChallanDate, AccountCode, ChallanGrid);
                string JsonString = JsonConvert.SerializeObject(JSON);
                List<CustomerJobWorkIssueDetail> JobWorkReceiveDetail = new List<CustomerJobWorkIssueDetail>();

                foreach (DataRow row in JSON.Result.Rows)
                {
                    CustomerJobWorkIssueDetail jobWorkRec = new CustomerJobWorkIssueDetail
                    {
                        CustJwRecEntryId = row["EntryIdRecJw"] != DBNull.Value ? Convert.ToInt32(row["EntryIdRecJw"]) : 0,
                        CustJWRecChallanNo = row["RecJWChallanNo"]?.ToString(),
                        CustJWRecYearCode = row["RecYearCode"] != DBNull.Value ? Convert.ToInt32(row["RecYearCode"]) : 0,
                        ChallanDate = row["ChallanDate"]?.ToString(),
                        RecPartCode = row["RecPartCode"]?.ToString(),
                        RecItemName = row["RecItemName"]?.ToString(),
                        RecItemcode = row["RecItemCode"] != DBNull.Value ? Convert.ToInt32(row["RecItemCode"]) : 0,
                        BOMNO = row["BomNo"] != DBNull.Value ? Convert.ToInt32(row["BomNo"]) : 0,
                        Bomdate = row["BOMDate"]?.ToString(),
                        //BOMInd = row["BomStatus"]?.ToString()?.FirstOrDefault() ?? ' ',
                        BOMInd = row["BomStatus"] != DBNull.Value && !string.IsNullOrEmpty(row["BomStatus"]?.ToString())
    ? row["BomStatus"].ToString()[0]
    : ' ',// Default to space if null or empty

                        PendQty = row["PendQty"] != DBNull.Value ? Convert.ToInt32(row["PendQty"]) : 0,
                        IssPartCode  = row["IssuePartcode"]?.ToString(),
                        IssItemName = row["IssueItemName"]?.ToString(),
                        //RecItemcode = row["RecItemCode"] != DBNull.Value ? Convert.ToInt64(row["RecItemCode"]) : 0,
                        BOMQty = row["bomqty"] != DBNull.Value ? Convert.ToInt16(row["bomqty"]) : 0,

                        //AdjQty = row["ActualAdjQty"] != DBNull.Value ? Convert.ToDecimal(row["ActualAdjQty"]) : 0,
                        Through = row["through"]?.ToString(),
                        //TotaladjQty = Convert.ToDecimal(row["QtyToBeRec"]),
                        
                        QtyToBeRec = row["QtyToBeRec"] != DBNull.Value ? Convert.ToInt16(row["QtyToBeRec"]) : 0,
                        TotalAdjQty = row["ActualAdjQty"] != DBNull.Value ? Convert.ToInt16(row["ActualAdjQty"]) : 0,
                        Recbatchno = row["batchno"]?.ToString(),
                        Recuniquebatchno = row["uniquebatchno"]?.ToString(),
                        SEQNo = row["seqno"] != DBNull.Value ? Convert.ToInt32(row["seqno"]) : 0,
                        IssQty = row["IssueQty"] != DBNull.Value ? Convert.ToInt32(row["IssueQty"]) : 0,
                        Rate = row["Rate"] != DBNull.Value ? Convert.ToDecimal(row["Rate"]) : 0,
                        
                        QriginalRecQty = row["OriginalRecQty"] != DBNull.Value ? Convert.ToDecimal(row["OriginalRecQty"]) : 0,
                        IdealScrapQty = row["IdealScrap"] != DBNull.Value ? Convert.ToInt32(row["IdealScrap"]) : 0,
                        IssuedScrapQty = row["IssuedScrap"] != null ? Convert.ToInt32(row["IssuedScrap"]) : 0
                      
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

        private static DataTable GetAdjustedChallanTable(List<CustomerJobWorkIssueDetail> DTTItemGrid)
        {
            try
            {
                var ChallanGrid = new DataTable();

                ChallanGrid.Columns.Add("ItemCode", typeof(long));  // bigint in SQL
                ChallanGrid.Columns.Add("Unit", typeof(string));
                ChallanGrid.Columns.Add("BillQty", typeof(long));  // bigint in SQL
                ChallanGrid.Columns.Add("JWRate", typeof(float));
                ChallanGrid.Columns.Add("ProcessId", typeof(int));
                ChallanGrid.Columns.Add("SONO", typeof(string));
                ChallanGrid.Columns.Add("CustOrderNo", typeof(string));
                ChallanGrid.Columns.Add("SOYearCode", typeof(long));  // bigint in SQL
                ChallanGrid.Columns.Add("SchNo", typeof(string));
                ChallanGrid.Columns.Add("SchYearcode", typeof(long)); // bigint in SQL
                ChallanGrid.Columns.Add("BOMIND", typeof(string));
                ChallanGrid.Columns.Add("BOMNO", typeof(long));  // bigint in SQL
                ChallanGrid.Columns.Add("BOMEffDate", typeof(DateTime));
                ChallanGrid.Columns.Add("Produnprod", typeof(string));
                ChallanGrid.Columns.Add("fromChallanOrSalebill", typeof(string));
                ChallanGrid.Columns.Add("ItemAdjustmentRequired", typeof(string));

                foreach (var Item in DTTItemGrid)
                {
                    ChallanGrid.Rows.Add(
                        new object[]
                        {
                    Item.ItemCode,  // Ensure it's long (bigint)
                    Item.Unit?.Trim().Substring(0, Math.Min(3, Item.Unit.Length)),
                    Convert.ToInt64(Item.ChallanQty), // Ensure bigint
                    Item.Rate,
                    Item.ProcessId,
                    Item.GridSONO?.Trim().Substring(0, Math.Min(200, Item.GridSONO.Length)),
                    Item.CustOrderNo?.Trim().Substring(0, Math.Min(200, Item.CustOrderNo.Length)),
                    Convert.ToInt64(Item.SOYear),  // Ensure bigint
                    Item.SchNo?.Trim().Substring(0, Math.Min(200, Item.SchNo.Length)),
                    Convert.ToInt64(Item.SchYearcode), // Ensure bigint
                    Item.BOMInd, // Ensure nchar(1)
                    Convert.ToInt64(Item.BOMNO),  // Ensure bigint
                    Item.Bomdate,
                    Item.ProduceUnproduce?.Trim().Substring(0, Math.Min(200, Item.ProduceUnproduce.Length)),
                    Item.fromChallanOrSalebill?.Trim().Substring(0, Math.Min(100, Item.fromChallanOrSalebill.Length)),
                    Item.ItemAdjustmentRequired?.Trim().Substring(0, Math.Min(3, Item.ItemAdjustmentRequired.Length))
                        });
                }

                return ChallanGrid;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<JsonResult> GetPopUpData(int YearCode, string EntryDate, string ChallanDate, int AccountCode, string prodUnProd, string BOMINd, int RMItemCode, string Partcode)
        {
            EntryDate = ParseFormattedDate(EntryDate);
            ChallanDate = ParseFormattedDate(ChallanDate);
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
                    string modelJson = HttpContext.Session.GetString("KeyCWIAdjustGrid");
                    List<CustomerJobWorkIssueDetail> CustomerJobWorkIssueDetail = new List<CustomerJobWorkIssueDetail>();
                    if (!string.IsNullOrEmpty(modelJson))
                    {
                        CustomerJobWorkIssueDetail = JsonConvert.DeserializeObject<List<CustomerJobWorkIssueDetail>>(modelJson);
                    }
                    
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

                        string serializedGrid = JsonConvert.SerializeObject(MainModel.CustJWIDetailGrid);
                        HttpContext.Session.SetString("KeyCWIAdjustGrid", serializedGrid);
                    }
                }
                return PartialView("_CustomerJwisschallanAdjustment", MainModel);
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
                string modelJson = HttpContext.Session.GetString("KeyCustJWIGrid");
                List<CustomerJobWorkIssueDetail> GridDetail = new List<CustomerJobWorkIssueDetail>();
                if (!string.IsNullOrEmpty(modelJson))
                {
                    GridDetail = JsonConvert.DeserializeObject<List<CustomerJobWorkIssueDetail>>(modelJson);
                }

                var MainModel = new CustomerJobWorkIssueModel();
                var SSGrid = new List<CustomerJobWorkIssueDetail>();

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

                    string serializedGrid = JsonConvert.SerializeObject(SSGrid);
                    HttpContext.Session.SetString("KeyCustJWIGrid", serializedGrid);
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
            string modelJson = HttpContext.Session.GetString("KeyCustJWIGrid");
            List<CustomerJobWorkIssueDetail> ReturnFromDepartmentDetail = new List<CustomerJobWorkIssueDetail>();
            if (!string.IsNullOrEmpty(modelJson))
            {
                ReturnFromDepartmentDetail = JsonConvert.DeserializeObject<List<CustomerJobWorkIssueDetail>>(modelJson);
            }
            var SSGrid = ReturnFromDepartmentDetail.Where(x => x.SEQNo == SEQNo);
            string JsonString = JsonConvert.SerializeObject(SSGrid);
            return Json(JsonString);
        }

        public async Task<IActionResult> DeleteItemRow(int SEQNo)
        {
            var MainModel = new CustomerJobWorkIssueModel();
            string modelJson = HttpContext.Session.GetString("KeyCustJWIGrid");
            List<CustomerJobWorkIssueDetail> RequisitionDetail = new List<CustomerJobWorkIssueDetail>();
            if (!string.IsNullOrEmpty(modelJson))
            {
                RequisitionDetail = JsonConvert.DeserializeObject<List<CustomerJobWorkIssueDetail>>(modelJson);
            }
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

                if (RequisitionDetail.Count == 0)
                {
                    HttpContext.Session.Remove("KeyCustJWIGrid");
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
