using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System.Data;
using System.Globalization;
using System.Net;
using static eTactWeb.Data.Common.CommonFunc;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Controllers
{
    public class CustomerJWRController : Controller
    {
        private readonly IDataLogic _IDataLogic;
        private readonly ICustomerJWR _ICustomerJWR;
        private readonly ILogger<CustomerJWRController> _logger;
        private readonly IConfiguration iconfiguration;

        public IWebHostEnvironment _IWebHostEnvironment { get; }
        public CustomerJWRController(ILogger<CustomerJWRController> logger, IDataLogic iDataLogic, ICustomerJWR iCustomerJWR, IWebHostEnvironment iWebHostEnvironment, IConfiguration iconfiguration)
        {
            _logger = logger;
            _IDataLogic = iDataLogic;
            _ICustomerJWR = iCustomerJWR;
            _IWebHostEnvironment = iWebHostEnvironment;
            this.iconfiguration = iconfiguration;
        }

        [Route("{controller}/Index")]
        public async Task<IActionResult> CustomerJWR()
        {
            ViewData["Title"] = "Customer JWR";
            TempData.Clear();
            HttpContext.Session.Remove("KeyCustomerJWRDetailGrid");
            var MainModel = new CustomerJobWorkReceiveModel();
            MainModel.FinFromDate = HttpContext.Session.GetString("FromDate");
            MainModel.FinToDate = HttpContext.Session.GetString("ToDate");
            MainModel.YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
            MainModel.CreatedByName = HttpContext.Session.GetString("EmpName");
            MainModel.UpdatedByName = HttpContext.Session.GetString("EmpName");
            MainModel.CreatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
            MainModel.CC = HttpContext.Session.GetString("Branch");
            MainModel.UID = Convert.ToInt32(HttpContext.Session.GetString("UID"));

            return View(MainModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("{controller}/Index")]
        public async Task<IActionResult> CustomerJWR(CustomerJobWorkReceiveModel model)
        {
            var fromDt = model.FromDate;
            var toDt = model.ToDate;
            model.EntryDate = string.IsNullOrEmpty(model.EntryDate) ? DateTime.Today.ToString() : model.EntryDate;
            try
            {
                var CustJWRGrid = new DataTable();
                string modelJson = HttpContext.Session.GetString("KeyCustomerJWRDetailGrid");
                List<CustomerJobWorkReceiveDetail> CustomerJWRDetail = new List<CustomerJobWorkReceiveDetail>();
                if (!string.IsNullOrEmpty(modelJson))
                {
                    CustomerJWRDetail = JsonConvert.DeserializeObject<List<CustomerJobWorkReceiveDetail>>(modelJson);
                }

                model.CreatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                model.ReceivedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                model.CC = HttpContext.Session.GetString("Branch");
                model.CustomerJWRGrid = CustomerJWRDetail;
                CustJWRGrid = GetDetailTable(CustomerJWRDetail);
                if (model.Mode == "U")
                {
                    model.UpdatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                }
                var Result = await _ICustomerJWR.SaveCustJWR(model, CustJWRGrid);

                if (Result != null)
                {
                    if (Result.StatusText == "Success" && Result.StatusCode == HttpStatusCode.OK)
                    {
                        ViewBag.isSuccess = true;
                        TempData["200"] = "200";
                        HttpContext.Session.Remove("KeyCustomerJWRDetailGrid");
                        var MainModel = new CustomerJobWorkReceiveModel();
                        MainModel.FinFromDate = HttpContext.Session.GetString("FromDate");
                        MainModel.FinToDate = HttpContext.Session.GetString("ToDate");
                        MainModel.YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
                        MainModel.CreatedByName = HttpContext.Session.GetString("EmpName");
                        MainModel.UpdatedByName = HttpContext.Session.GetString("EmpName");
                        MainModel.CreatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                        MainModel.EmployeeList = await _ICustomerJWR.GetEmployeeList();
                        MainModel.CC = HttpContext.Session.GetString("Branch");
                        MainModel.UID = Convert.ToInt32(HttpContext.Session.GetString("UID"));

                        return View(MainModel);
                    }
                    if (Result.StatusText == "Success" && Result.StatusCode == HttpStatusCode.Accepted)
                    {
                        ViewBag.isSuccess = true;
                        TempData["202"] = "202";
                        HttpContext.Session.Remove("KeyCustomerJWRDetailGrid");
                        var MainModel = new CustomerJobWorkReceiveModel();
                        MainModel.FinFromDate = HttpContext.Session.GetString("FromDate");
                        MainModel.FinToDate = HttpContext.Session.GetString("ToDate");
                        MainModel.YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
                        MainModel.CreatedByName = HttpContext.Session.GetString("EmpName");
                        MainModel.UpdatedByName = HttpContext.Session.GetString("EmpName");
                        MainModel.CreatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                        MainModel.CC = HttpContext.Session.GetString("Branch");
                        MainModel.UID = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                        MainModel.EmployeeList = await _ICustomerJWR.GetEmployeeList();

                        return View(MainModel);
                    }
                    if (Result.StatusText == "Error" && Result.StatusCode == HttpStatusCode.InternalServerError)
                    {
                        ViewBag.isSuccess = false;
                        var MainModel = new CustomerJobWorkReceiveModel();
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
                        //return View("Error", Result);
                    }
                }

                model.FromDate = fromDt;
                model.ToDate = toDt;
                return View(model);
                //}
            }
            catch (Exception ex)
            {
                LogException<CustomerJWRController>.WriteException(_logger, ex);

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
            var JSON = await _ICustomerJWR.GetFormRights(userID);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }

        [Route("{controller}/Index")]
        [HttpGet]
        public async Task<IActionResult> CustomerJWR(int ID, string Mode, int YC, string FromDate = "", string ToDate = "", string VendorName = "", string MrnNo = "", string ChallanNo = "", string ItemName = "", string PartCode = "", string DashboardType = "", string Searchbox = "")
        {
            _logger.LogInformation("\n \n ********** Page Customer JWR ********** \n \n " + _IWebHostEnvironment.EnvironmentName.ToString() + "\n \n");
            TempData.Clear();
            var MainModel = new CustomerJobWorkReceiveModel();
            MainModel.FinFromDate = HttpContext.Session.GetString("FromDate");
            MainModel.YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
            MainModel.FinToDate = HttpContext.Session.GetString("ToDate");
            MainModel.CreatedByName = HttpContext.Session.GetString("EmpName");
            MainModel.CreatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
            MainModel.EmployeeList = await _ICustomerJWR.GetEmployeeList();
            MainModel.CC = HttpContext.Session.GetString("Branch");
            HttpContext.Session.Remove("KeyCustomerJWRDetailGrid");

            if (!string.IsNullOrEmpty(Mode) && ID > 0 && (Mode == "V" || Mode == "U"))
            {
                MainModel = await _ICustomerJWR.GetViewByID(ID, YC).ConfigureAwait(false);
                MainModel.Mode = Mode;
                MainModel.ID = ID;
                MainModel.FinFromDate = HttpContext.Session.GetString("FromDate");
                MainModel.YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
                MainModel.ReceivedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                MainModel.FinToDate = HttpContext.Session.GetString("ToDate");
                MainModel.CC = HttpContext.Session.GetString("Branch");
                MainModel.EmployeeList = await _ICustomerJWR.GetEmployeeList();
                string serializedGrid = JsonConvert.SerializeObject(MainModel.CustomerJWRGrid);
                HttpContext.Session.SetString("KeyCustomerJWRDetailGrid", serializedGrid);
            }
            else
            {

            }
            if (Mode != "U")
            {
                MainModel.CreatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                MainModel.CreatedByName = HttpContext.Session.GetString("EmpName");
                MainModel.CreatedOn = DateTime.Now;
            }
            else
            {
                MainModel.UpdatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                MainModel.UpdatedByName = HttpContext.Session.GetString("EmpName");
                MainModel.UpdatedOn = DateTime.Now;
            }
            MainModel.FromDateBack = FromDate;
            MainModel.ToDateBack = ToDate;
            MainModel.PartCodeBack = PartCode;
            MainModel.ItemNameBack = ItemName;
            MainModel.MRNNoBack = MrnNo;
            MainModel.ChallanNoBack = ChallanNo;
            MainModel.GlobalSearchBack = Searchbox;
            MainModel.DashboardTypeBack = DashboardType;
            MainModel.VendorNameBack = VendorName;
            return View(MainModel);
        }
        public async Task<JsonResult> GetNewEntry(int YearCode)
        {
            var JSON = await _ICustomerJWR.GetNewEntry(YearCode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetGateNo(string FromDate, string ToDate)
        {
            var JSON = await _ICustomerJWR.GetGateNo(FromDate, ToDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetGateMainData(string GateNo, string GateYearCode, int GateEntryId)
        {
            var JSON = await _ICustomerJWR.GetGateMainData(GateNo, GateYearCode, GateEntryId);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetGateItemData(string GateNo, string GateYearCode, int GateEntryId)
        {
            var JSON = await _ICustomerJWR.GetGateItemData(GateNo, GateYearCode, GateEntryId);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public IActionResult AddCustomerJWRDetail(List<CustomerJobWorkReceiveDetail> model)
        {
            try
            {
                string modelJson = HttpContext.Session.GetString("KeyCustomerJWRDetailGrid");
                List<CustomerJobWorkReceiveDetail> CustomerJobWorkReceiveDetail = new List<CustomerJobWorkReceiveDetail>();
                if (!string.IsNullOrEmpty(modelJson))
                {
                    CustomerJobWorkReceiveDetail = JsonConvert.DeserializeObject<List<CustomerJobWorkReceiveDetail>>(modelJson);
                }

                var MainModel = new CustomerJobWorkReceiveModel();
                var MaterialReceiptGrid = new List<CustomerJobWorkReceiveDetail>();
                var MaterialGrid = new List<CustomerJobWorkReceiveDetail>();
                var SSGrid = new List<CustomerJobWorkReceiveDetail>();

                var seqNo = 0;
                foreach (var item in model)
                {
                    if (item != null)
                    {
                        if (CustomerJobWorkReceiveDetail == null || CustomerJobWorkReceiveDetail.Count() == 0)
                        {
                            item.SeqNo += seqNo + 1;
                            MaterialGrid.Add(item);
                            seqNo++;
                        }
                        else
                        {
                            item.SeqNo = CustomerJobWorkReceiveDetail.Count + 1;
                            MaterialGrid = CustomerJobWorkReceiveDetail.Where(x => x != null).ToList();
                            SSGrid.AddRange(MaterialGrid);
                            MaterialGrid.Add(item);
                        }
                        MainModel.CustomerJWRGrid = MaterialGrid;

                        string serializedGrid = JsonConvert.SerializeObject(MainModel.CustomerJWRGrid);
                        HttpContext.Session.SetString("KeyCustomerJWRDetailGrid", serializedGrid);
                    }
                }
                return PartialView("_CustomerJWRGrid", MainModel);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IActionResult EditItemRow(int SeqNo)
        {
            string modelJson = HttpContext.Session.GetString("KeyCustomerJWRDetailGrid");
            List<CustomerJobWorkReceiveDetail> CustomerJWRGrid = new List<CustomerJobWorkReceiveDetail>();
            if (!string.IsNullOrEmpty(modelJson))
            {
                CustomerJWRGrid = JsonConvert.DeserializeObject<List<CustomerJobWorkReceiveDetail>>(modelJson);
            }

            var SSGrid = CustomerJWRGrid.Where(x => x.SeqNo == SeqNo);
            string JsonString = JsonConvert.SerializeObject(SSGrid);
            return Json(JsonString);
        }
        public async Task<JsonResult> CheckEditOrDelete(string MRNNo, int YearCode)
        {
            var JSON = await _ICustomerJWR.CheckEditOrDelete(MRNNo, YearCode);
            _logger.LogError(JsonConvert.SerializeObject(JSON));
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
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
        private static DataTable GetDetailTable(IList<CustomerJobWorkReceiveDetail> DetailList)
        {
            var CustJWRGrid = new DataTable();

            CustJWRGrid.Columns.Add("SeqNo", typeof(int));
            CustJWRGrid.Columns.Add("EntryId", typeof(int));
            CustJWRGrid.Columns.Add("YearCode", typeof(int));
            CustJWRGrid.Columns.Add("RecItemCode", typeof(int));
            CustJWRGrid.Columns.Add("Billqty", typeof(float));
            CustJWRGrid.Columns.Add("RecQty", typeof(float));
            CustJWRGrid.Columns.Add("Unit", typeof(string));
            CustJWRGrid.Columns.Add("RecAltQty", typeof(float));
            CustJWRGrid.Columns.Add("AltUnit", typeof(string));
            CustJWRGrid.Columns.Add("Rate", typeof(float));
            CustJWRGrid.Columns.Add("Amount", typeof(float));
            CustJWRGrid.Columns.Add("ShortExcessQty", typeof(float));
            CustJWRGrid.Columns.Add("Remark", typeof(string));
            CustJWRGrid.Columns.Add("Purpose", typeof(string));
            CustJWRGrid.Columns.Add("FinishedItemCode", typeof(int));
            CustJWRGrid.Columns.Add("FinishedQty", typeof(float));
            CustJWRGrid.Columns.Add("PendQty", typeof(float));
            CustJWRGrid.Columns.Add("RecScrap", typeof(float));
            CustJWRGrid.Columns.Add("AllowedRejPer", typeof(float));
            CustJWRGrid.Columns.Add("ProcessId", typeof(int));
            CustJWRGrid.Columns.Add("Color", typeof(string));
            CustJWRGrid.Columns.Add("QcCompleted", typeof(string));
            CustJWRGrid.Columns.Add("CustBatchno", typeof(string));
            CustJWRGrid.Columns.Add("batchno", typeof(string));
            CustJWRGrid.Columns.Add("UniqueBatchNo", typeof(string));
            CustJWRGrid.Columns.Add("SoNo", typeof(string));
            CustJWRGrid.Columns.Add("SoYearCode", typeof(int));
            CustJWRGrid.Columns.Add("CustOrderno", typeof(string));
            CustJWRGrid.Columns.Add("SOSchNo", typeof(string));
            CustJWRGrid.Columns.Add("SOSchYearCode", typeof(int));
            CustJWRGrid.Columns.Add("SODate", typeof(DateTime));
            CustJWRGrid.Columns.Add("SchDate", typeof(DateTime));
            CustJWRGrid.Columns.Add("bomno", typeof(int));
            CustJWRGrid.Columns.Add("BomName", typeof(string));
            CustJWRGrid.Columns.Add("BomDate", typeof(DateTime));
            CustJWRGrid.Columns.Add("INDBOM", typeof(string));

            foreach (var Item in DetailList)
            {
                CustJWRGrid.Rows.Add(
                    new object[]
                    {
                    Item.SeqNo,
                    0,
                    0,
                    Item.RecitemCode,
                    Item.BillQty,
                    Item.RecQty,
                    Item.Unit,
                    Item.RecAltQty,
                    Item.AltUnit??"",
                    Item.Rate,
                    Item.Amount,
                    Item.ShortExcessQty,
                    Item.Remark??"",
                    Item.Purpose??"",
                    Item.FinishItemCode,
                    Item.FinsihedQty,
                    Item.PendQty,
                    Item.RecScrap,
                    Item.AllowedRejPer,
                    Item.ProcessId,
                    Item.Color??"",
                    Item.QcCompleted??"",
                    Item.CustbatchNo??"",
                    Item.BatchNo??"",
                    Item.UniqueBatchNo??"",
                    Item.SoNo ?? "",
                    Item.SoYearCode,
                    Item.CustOrderno ?? "",
                    Item.SOSchNo ?? "",
                    Item.SOSchYearCode,
                    DateTime.Today, //change
                    DateTime.Today, // change
                    Item.BomNo,
                    Item.BomName ?? "",
                    DateTime.Today, // change
                    Item.INDBOM ?? ""
                    });
            }
            CustJWRGrid.Dispose();
            return CustJWRGrid;
        }
        public async Task<IActionResult> CustomerJWRDashboard(string FromDate = "", string ToDate = "", string Flag = "True", string VendorName = "", string MrnNo = "", string ChallanNo = "", string ItemName = "", string PartCode = "", string DashboardType = "")
        {
            try
            {
                HttpContext.Session.Remove("KeyCustomerJWRDetailGrid");
                var model = new CustomerJWRQDashboard();
                var Result = await _ICustomerJWR.GetDashboardData(FromDate, ToDate, VendorName, ChallanNo, PartCode, ItemName, MrnNo).ConfigureAwait(true);

                if (Result != null)
                {
                    var _List = new List<TextValue>();
                    DataSet DS = Result.Result;
                    if (DS != null)
                    {
                        var DT = DS.Tables[0].DefaultView.ToTable(false, "VendorName", "MrnNo", "MrnDate", "GateNo", "GateDate", "ChallanNo",
                            "ChallanDate", "EntryId", "YearCode", "EntryBy", "ReceByEmp", "LastUpdatedByEmp", "JobworkType", "CC", "UID"
                            , "GateYearCode", "Remark", "MRNQCCompleted", "Complete", "Closed", "ActualEntryDate", "UpdatedOn", "EntryByMachineName");
                        model.CustomerJWRQDashboard = CommonFunc.DataTableToList<CustomerJWRDashboard>(DT, "CustomerJWR");

                        if (Flag == "False")
                        {
                            model.FromDate1 = FromDate;
                            model.ToDate1 = ToDate;
                            model.VendorName = VendorName;
                            model.MRNNo = MrnNo;
                            model.ChallanNo = ChallanNo;
                            model.ItemName = ItemName;
                            model.PartCode = PartCode;
                            model.DashboardType = DashboardType;
                        }
                    }
                }

                return View(model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IActionResult> GetSearchData(string FromDate, string ToDate, string VendorName, string MrnNo, string ChallanNo, string ItemName, string PartCode)
        {
            //model.Mode = "Search";
            var model = new CustomerJWRQDashboard();
            model = await _ICustomerJWR.GetSearchData(FromDate, ToDate, VendorName, MrnNo, ChallanNo, ItemName, PartCode);
            model.DashboardType = "Summary";
            return PartialView("_CustomerJWRDashboardGrid", model);
        }
        public async Task<IActionResult> GetSearchDetailData(string FromDate, string ToDate, string VendorName, string MrnNo, string ChallanNo, string ItemName, string PartCode)
        {
            //model.Mode = "Search";
            var model = new CustomerJWRQDashboard();
            model = await _ICustomerJWR.GetSearchDetailData(FromDate, ToDate, VendorName, MrnNo, ChallanNo, ItemName, PartCode);
            model.DashboardType = "Detail";
            return PartialView("_CustomerJWRDashboardGrid", model);
        }
        public IActionResult DeleteItemRow(int SeqNo)
        {
            var MainModel = new CustomerJobWorkReceiveModel();
            string modelJson = HttpContext.Session.GetString("KeyCustomerJWRDetailGrid");
            List<CustomerJobWorkReceiveDetail> CustomerJWRGrid = new List<CustomerJobWorkReceiveDetail>();
            if (!string.IsNullOrEmpty(modelJson))
            {
                CustomerJWRGrid = JsonConvert.DeserializeObject<List<CustomerJobWorkReceiveDetail>>(modelJson);
            }

            int Indx = Convert.ToInt32(SeqNo) - 1;

            if (CustomerJWRGrid != null && CustomerJWRGrid.Count > 0)
            {
                CustomerJWRGrid.RemoveAt(Convert.ToInt32(Indx));

                Indx = 0;

                foreach (var item in CustomerJWRGrid)
                {
                    Indx++;
                    item.SeqNo = Indx;
                }
                MainModel.CustomerJWRGrid = CustomerJWRGrid;

                MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                    SlidingExpiration = TimeSpan.FromMinutes(55),
                    Size = 1024,
                };

                if (CustomerJWRGrid.Count == 0)
                {
                    HttpContext.Session.Remove("KeyCustomerJWRDetailGrid");
                }
            }
            return PartialView("_CustomerJWRGrid", MainModel);
        }
        public async Task<IActionResult> DeleteByID(int ID, int YC, string EntryByMachineName, string FromDate = "", string ToDate = "", string VendorName = "", string MrnNo = "", string ChallanNo = "", string ItemName = "", string PartCode = "", string DashboardType = "")
        {
            var Result = await _ICustomerJWR.DeleteByID(ID, YC, EntryByMachineName);

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

            return RedirectToAction("CustomerJWRDashboard", new { FromDate = formattedFromDate, ToDate = formattedToDate, Flag = "False", VendorName = VendorName, MrnNo = MrnNo, ChallanNo = ChallanNo, ItemName = ItemName, PartCode = PartCode, DashboardType = DashboardType });

        }
    }
}