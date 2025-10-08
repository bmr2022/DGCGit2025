using eTactWeb.Controllers;
using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using static eTactWeb.Data.Common.CommonFunc;
using static eTactWeb.DOM.Models.Common;
using System.Data;
using System.Net;
using FastReport.Web;
using FastReport;
using System.Globalization;
using NuGet.Packaging;
using Newtonsoft.Json.Linq;

namespace eTactwebAccounts.Controllers
{

  
    public class AgainstAdjustVoucherController : Controller
    {
        private readonly IDataLogic _IDataLogic;
        public IAgainstAdjustVoucher _IAgainstAdjustVoucher { get; }
        private readonly ILogger<AgainstAdjustVoucherController> _logger;
        private readonly IConfiguration iconfiguration;
        public IWebHostEnvironment _IWebHostEnvironment { get; }
        private readonly ConnectionStringService _connectionStringService;
        public AgainstAdjustVoucherController(ILogger<AgainstAdjustVoucherController> logger, IDataLogic iDataLogic, IAgainstAdjustVoucher IAgainstAdjustVoucher, EncryptDecrypt encryptDecrypt, IWebHostEnvironment iWebHostEnvironment, IConfiguration iconfiguration, ConnectionStringService connectionStringService)
        {
            _logger = logger;
            _IDataLogic = iDataLogic;
            _IAgainstAdjustVoucher = IAgainstAdjustVoucher;
            _IWebHostEnvironment = iWebHostEnvironment;
            this.iconfiguration = iconfiguration;
            _connectionStringService = connectionStringService;
        }
        public async Task<JsonResult> GetFormRights()
        {
            var userID = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
            var JSON = await _IAgainstAdjustVoucher.GetFormRights(userID);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public IActionResult PrintReport(int EntryId = 0, int YearCode = 0, string VoucherName = "")
        {
            string my_connection_string;
            string contentRootPath = _IWebHostEnvironment.ContentRootPath;
            string webRootPath = _IWebHostEnvironment.WebRootPath;
            var webReport = new WebReport();
            webReport.Report.Clear();
            webReport.Report.Dispose();
            webReport.Report = new Report();

            webReport.Report.Load(webRootPath + "\\VoucherReport.frx");
            my_connection_string = _connectionStringService.GetConnectionString();
            my_connection_string = iconfiguration.GetConnectionString("eTactDB");
            webReport.Report.Dictionary.Connections[0].ConnectionString = my_connection_string;
            webReport.Report.Dictionary.Connections[0].ConnectionStringExpression = "";
            webReport.Report.SetParameterValue("vouchernameparam", VoucherName);
            webReport.Report.SetParameterValue("yearcodeparam", YearCode);
            webReport.Report.SetParameterValue("entryidparam", EntryId);
            webReport.Report.SetParameterValue("MyParameter", my_connection_string);
            webReport.Report.Refresh();
            return View(webReport);
        }
        [Route("{controller}/Index")]
        [HttpGet]
        public async Task<ActionResult> AgainstAdjustVoucher(int ID, string Mode, int YearCode, string VoucherNo, string FromDate = "", string ToDate = "", string LedgerName = "", string AgainstVoucherRefNo = "", string AgainstVoucherNo = "", string Searchbox = "", string DashboardType = "")
        {
            HttpContext.Session.Remove("KeyAgainstAdjustVoucherGrid");
            HttpContext.Session.Remove("KeyAgainstAdjustVoucherGridEdit");
            TempData.Clear();
            var MainModel = new AgainstAdjustVoucherModel();
            MainModel.CC = HttpContext.Session.GetString("Branch");
            MainModel.YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
            MainModel.ActualEntryBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
            MainModel.ActualEntryByEmp = HttpContext.Session.GetString("EmpName");
            MainModel.ActualEntryDate = DateTime.Now.ToString("dd/MM/yy");
            MainModel.UID = Convert.ToInt32(HttpContext.Session.GetString("UID"));
            MainModel.FromDate = HttpContext.Session.GetString("FromDate");
            MainModel.ToDate = HttpContext.Session.GetString("ToDate");

            if (!string.IsNullOrEmpty(Mode) && ID > 0 && (Mode == "U" || Mode == "V"))
            {
                MainModel = await _IAgainstAdjustVoucher.GetViewByID(ID, YearCode, VoucherNo).ConfigureAwait(false);
                MainModel.Mode = Mode; // Set Mode to Update
                MainModel.ID = ID;
                MainModel.VoucherNo = VoucherNo;

                string serializedGrid = JsonConvert.SerializeObject(MainModel.AgainstAdjustVoucherList);
                HttpContext.Session.SetString("KeyAgainstAdjustVoucherGridEdit", serializedGrid);
            }

            if (Mode == "U" && MainModel.UpdatedBy == 0)
            {
                MainModel.UpdatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                MainModel.UpdatedByEmp = HttpContext.Session.GetString("EmpName");
                MainModel.UpdatedOn = DateTime.Now;
            }

            MainModel.FromDateBack = FromDate;
            MainModel.ToDateBack = ToDate;
            MainModel.VoucherNoBack = VoucherNo;
            MainModel.LedgerNameBack = LedgerName;
            MainModel.AgainstVoucherRefNo = AgainstVoucherRefNo;
            MainModel.AgainstVoucherNoBack = AgainstVoucherNo;
            MainModel.GlobalSearchBack = Searchbox;
            MainModel.DashboardTypeBack = DashboardType;
            return View(MainModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("{controller}/Index")]
        public async Task<IActionResult> AgainstAdjustVoucher(AgainstAdjustVoucherModel model)
        {
            try
            {
                var GIGrid = new DataTable();
                string modelJson = HttpContext.Session.GetString("KeyAdjGrid");
                List<AgainstAdjustVoucherModel> AgainstAdjustVoucherGrid = new List<AgainstAdjustVoucherModel>();
                List<AgainstAdjustVoucherModel> AgainstAdjustVoucherGridEdit = new List<AgainstAdjustVoucherModel>();

                // Deserialize JSON for new entries
                if (!string.IsNullOrEmpty(modelJson))
                {
                    var jObject = JObject.Parse(modelJson);
                    var array = jObject["AdjAdjustmentDetailGrid"];
                    AgainstAdjustVoucherGrid = array?.ToObject<List<AgainstAdjustVoucherModel>>() ?? new List<AgainstAdjustVoucherModel>();
                }
                string modelEditJson = HttpContext.Session.GetString("KeyAdjGrid");
                if (!string.IsNullOrEmpty(modelEditJson))
                {
                    var jObjectEdit = JObject.Parse(modelEditJson);
                    var arrayEdit = jObjectEdit["AdjAdjustmentDetailGrid"];
                    AgainstAdjustVoucherGridEdit = arrayEdit?.ToObject<List<AgainstAdjustVoucherModel>>() ?? new List<AgainstAdjustVoucherModel>();
                }

                model.ActualEntryBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                model.ActualEntryByEmp = HttpContext.Session.GetString("UID");
                if (model.Mode == "U")
                {
                    GIGrid = GetDetailTable(AgainstAdjustVoucherGridEdit);
                }
                else
                {
                    GIGrid = GetDetailTable(AgainstAdjustVoucherGrid);
                }
                var Result = await _IAgainstAdjustVoucher.SaveAgainstAdjustVoucher(model, GIGrid);
                if (Result != null)
                {
                    if (Result.StatusText == "Success" && Result.StatusCode == HttpStatusCode.OK)
                    {
                        ViewBag.isSuccess = true;
                        TempData["200"] = "200";
                        HttpContext.Session.Remove("KeyAgainstAdjustVoucherGrid");
                        HttpContext.Session.Remove("KeyAdjGrid");
                    }
                    else if (Result.StatusText == "Success" && Result.StatusCode == HttpStatusCode.Accepted)
                    {
                        ViewBag.isSuccess = true;
                        TempData["202"] = "202";
                        HttpContext.Session.Remove("KeyAgainstAdjustVoucherGrid");
                        HttpContext.Session.Remove("KeyAdjGrid");
                    }
                    else if (Result.StatusText == "Error" && Result.StatusCode == HttpStatusCode.InternalServerError)
                    {
                        ViewBag.isSuccess = false;
                        TempData["500"] = "500";
                        _logger.LogError($"\n \n ********** LogError ********** \n {JsonConvert.SerializeObject(Result)}\n \n");
                        return View("Error", Result);
                    }
                }
                return RedirectToAction(nameof(AgainstAdjustVoucher));
            }
            catch (Exception ex)
            {
                // Log and return the error
                LogException<AgainstAdjustVoucherController>.WriteException(_logger, ex);
                var ResponseResult = new ResponseResult
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    StatusText = "Error",
                    Result = ex
                };
                return View("Error", ResponseResult);
            }
        }
        private DataTable GetDetailTable(List<AgainstAdjustVoucherModel> list)
        {
            // Create DataTable matching the SQL Type_AccAgainstRefAdjustmentVoucherDetail
            DataTable dt = new DataTable();

            dt.Columns.Add("AgainstRefEntryId", typeof(long));
            dt.Columns.Add("AgainstRefYearCode", typeof(long));
            dt.Columns.Add("SeqNo", typeof(long));
            dt.Columns.Add("ModOfAdjust", typeof(string));
            dt.Columns.Add("VoucherDate", typeof(DateTime));
            dt.Columns.Add("VoucherType", typeof(string));
            dt.Columns.Add("DrAmt", typeof(decimal));
            dt.Columns.Add("CrAmt", typeof(decimal));
            dt.Columns.Add("BillNetAmt", typeof(decimal));
            dt.Columns.Add("AgainstAccEntryId", typeof(long));
            dt.Columns.Add("AgainstVoucheryearcode", typeof(long));
            dt.Columns.Add("AgainstvoucherType", typeof(string));
            dt.Columns.Add("AgainstVoucherNo", typeof(string));
            dt.Columns.Add("AgainstAccOpeningEntryId", typeof(long));
            dt.Columns.Add("AgainstOpeningVoucheryearcode", typeof(long));
            dt.Columns.Add("RemainingBalanceAmt", typeof(decimal));
            dt.Columns.Add("DueDate", typeof(DateTime));
            dt.Columns.Add("RemainingAmountInOtherCurrency", typeof(decimal));
            dt.Columns.Add("AdjAmountInOtherCurrency", typeof(decimal));
            dt.Columns.Add("AddAmountInOtherCurr", typeof(decimal));
            dt.Columns.Add("AmountInothercurrency", typeof(decimal));
            dt.Columns.Add("OursalespersonId", typeof(long));
            dt.Columns.Add("VoucherDescription", typeof(string));
            dt.Columns.Add("NewrefNo", typeof(string));
            dt.Columns.Add("SubVoucherName", typeof(string));

            // Populate rows
            foreach (var item in list)
            {
                var row = dt.NewRow();

                row["AgainstRefEntryId"] = item.EntryId;
                row["AgainstRefYearCode"] = item.YearCode;
                row["SeqNo"] = item.SeqNo;
                row["ModOfAdjust"] = item.ModeOfAdjustment ?? string.Empty;
                row["VoucherDate"] = item.VoucherDate ?? (object)DBNull.Value;
                row["VoucherType"] = item.VoucherType ?? string.Empty;
                row["DrAmt"] = item.DrAmt;
                row["CrAmt"] = item.CrAmt;
                row["BillNetAmt"] = item.BillAmount??0;
                row["AgainstAccEntryId"] = item.AgainstVoucherEntryId;
                row["AgainstVoucheryearcode"] = item.AgainstVoucheryearCode;
                row["AgainstvoucherType"] = item.AgainstVoucherType ?? string.Empty;
                row["AgainstVoucherNo"] = item.AgainstVoucherNo ?? string.Empty;
                row["AgainstAccOpeningEntryId"] = item.AgainstAccOpeningEntryId;
                row["AgainstOpeningVoucheryearcode"] = item.AgainstOpeningVoucheryearcode;
                row["RemainingBalanceAmt"] = item.PendBillAmt;
                row["DueDate"] = item.DueDate ?? (object)DBNull.Value;
                row["RemainingAmountInOtherCurrency"] = item.PendBillAmt;
                row["AdjAmountInOtherCurrency"] = item.AdjAmountInOtherCurrency;
                row["AddAmountInOtherCurr"] = item.AdjustAmount;
                row["AmountInothercurrency"] = item.AdjustAmount;
                row["OursalespersonId"] = item.OursalespersonId;
                row["VoucherDescription"] = item.Description ?? string.Empty;
                row["NewrefNo"] = item.NewrefNo ?? string.Empty;
                row["SubVoucherName"] = item.SubVoucherName ?? string.Empty;

                dt.Rows.Add(row);
            }

            return dt;
        }

        public async Task<JsonResult> FillEntryID(int YearCode, string VoucherDate)
        {
            var JSON = await _IAgainstAdjustVoucher.FillEntryID(YearCode, VoucherDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        } 
        public async Task<JsonResult> GetAccEntryId(int YearCode, string VoucherType, string VoucherNo, int AccountCode, string InvoiceNo)
        {
            var JSON = await _IAgainstAdjustVoucher.GetAccEntryId(YearCode, VoucherType, VoucherNo, AccountCode, InvoiceNo);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillVoucherType(int yearcode)
        { 
            yearcode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
            var JSON = await _IAgainstAdjustVoucher.FillVoucherType(yearcode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        
        public async Task<IActionResult> GetAdjustedData(int YearCode, string VoucherType, string VoucherNo, int AccountCode, string InvoiceNo, int AccEntryId)
        {
            var model = new AgainstAdjustVoucherModel
            {
                Mode = "Adjust",
                adjustmentModel = new AdjustmentModel
                {
                    AdjAdjustmentDetailGrid = new List<AdjustmentModel>()
                }
            };

            model.AgainstAdjustVoucherList = await _IAgainstAdjustVoucher.GetAdjustedData(YearCode, VoucherType, VoucherNo, AccountCode, InvoiceNo, AccEntryId);
            int seqNo = 1;
            foreach (var item in model.AgainstAdjustVoucherList)
            {
                var adjItem = new AdjustmentModel
                {
                    AdjSeqNo = seqNo++,
                    AdjAgnstVouchNo = item.AgainstVoucherNo,
                    AdjNewRefNo = item.VoucherNo,
                    AdjDueDate = DateTime.TryParse(item.VoucherDocDate, out var dt) ? dt : (DateTime?)null ,
                    AdjAgnstVouchType = item.AgainstVoucherType,
                    AdjAgnstAccEntryID = item.AgainstVoucherEntryId,
                    AdjModeOfAdjstment = item.ModeOfAdjustment,
                    AdjPendAmt = (decimal)item.AdjustmentAmt,
                    AdjTotalAmt = (float)item.VoucherBillAmt,
                    AdjRemainingAmt = (float)item.PendBillAmt,
                    AdjAgnstDrCr = item.DRCR,
                    AdjDescription = item.VoucherNo,
                    AdjAgnstPendAmt = (float?)item.AdjustmentAmt,
                    AdjAgnstAccYearCode=item.AgainstVoucheryearCode,
                    AdjAgnstVouchDate = DateTime.TryParse(item.AgainstVoucherDate, out var date) ? date : (DateTime?)null
                };

                model.adjustmentModel.AdjAdjustmentDetailGrid.Add(adjItem);
            }

            string serializedObject = JsonConvert.SerializeObject(model.adjustmentModel);
            HttpContext.Session.SetString("KeyAdjGrid", serializedObject);

            return PartialView("_AgainstAdjustVoucher", model);
        }

        //[HttpPost]
        //public IActionResult AddAgnstRefToAdjstmntDetail([FromBody] AgainstAdjustVoucherModel model)
        //{
        //    if (model == null || model.AdjAdjustmentDetailGrid == null || !model.AdjAdjustmentDetailGrid.Any())
        //    {
        //        return BadRequest("No adjustment data provided.");
        //    }

        //    // Get existing model from session
        //    string modelJson = HttpContext.Session.GetString("KeyAdjGrid");
        //    var existingGrid = !string.IsNullOrEmpty(modelJson)
        //        ? JsonConvert.DeserializeObject<AgainstAdjustVoucherModel>(modelJson)
        //        : new AgainstAdjustVoucherModel { AdjAdjustmentDetailGrid = new List<AdjustmentModel>() };

        //    if (existingGrid.AdjAdjustmentDetailGrid == null)
        //        existingGrid.AdjAdjustmentDetailGrid = new List<AdjustmentModel>();

        //    // Force mode = AgainstRef
        //    foreach (var row in model.AdjAdjustmentDetailGrid)
        //    {
        //        row.AdjModeOfAdjstment = "AgainstRef";
        //    }

        //    // Duplicate check (based on Mode + RefNo)
        //    foreach (var newRow in model.AdjAdjustmentDetailGrid)
        //    {
        //        bool isDuplicate = existingGrid.AdjAdjustmentDetailGrid.Any(a =>
        //            a.AdjModeOfAdjstment != null &&
        //            a.AdjModeOfAdjstment.Equals(newRow.AdjModeOfAdjstment, StringComparison.OrdinalIgnoreCase) &&
        //            a.AdjNewRefNo != null &&
        //            a.AdjNewRefNo.Equals(newRow.AdjNewRefNo, StringComparison.OrdinalIgnoreCase)
        //        );

        //        if (isDuplicate)
        //        {
        //            return Conflict("Duplicate adjustment found.");
        //        }
        //    }

        //    // Assign sequence numbers
        //    int seqStart = existingGrid.AdjAdjustmentDetailGrid.Count + 1;
        //    foreach (var row in model.AdjAdjustmentDetailGrid)
        //    {
        //        row.AdjSeqNo = seqStart++;
        //        existingGrid.AdjAdjustmentDetailGrid.Add(row);
        //    }

        //    // Save back into session
        //    string updatedJson = JsonConvert.SerializeObject(existingGrid);
        //    HttpContext.Session.SetString("KeyAdjGrid", updatedJson);

        //    return Json(existingGrid.AdjAdjustmentDetailGrid);
        //}

        //public IActionResult GetUpdatedAdjGridData()
        //{
        //    // Retrieve the updated data from the cache
        //    string modelJson = HttpContext.Session.GetString("KeyAdjGrid");
        //    AgainstAdjustVoucherModel AdjGrid = new AgainstAdjustVoucherModel();
        //    if (!string.IsNullOrEmpty(modelJson))
        //    {
        //        AdjGrid = JsonConvert.DeserializeObject<AgainstAdjustVoucherModel>(modelJson);
        //    }

        //    if (AdjGrid != null)
        //    {
        //        return Json(AdjGrid.AdjAdjustmentDetailGrid);
        //    }
        //    return Json(new List<AgainstAdjustVoucherModel>());
        //}
        public void StoreInSession(string sessionKey, object sessionObject)
        {
            string serializedObject = JsonConvert.SerializeObject(sessionObject);
            HttpContext.Session.SetString(sessionKey, serializedObject);
        }
        //public IList<AgainstAdjustVoucherModel> Add2List(AgainstAdjustVoucherModel model, IList<AgainstAdjustVoucherModel> AdjGrid, bool? IsAgnstRefPopupData = false)
        //{
        //    var _List = new List<AgainstAdjustVoucherModel>();
        //    if (IsAgnstRefPopupData != true)
        //    {
        //        _List.Add(new AgainstAdjustVoucherModel
        //        {
        //            AdjSeqNo = AdjGrid == null ? 1 : AdjGrid.Count + 1,
        //            AdjModeOfAdjstment = model.AdjModeOfAdjstment,
        //            AdjModeOfAdjstmentName = model.AdjModeOfAdjstmentName,
        //            AdjDescription = model.AdjDescription,
        //            AdjDueDate = model.AdjDueDate,
        //            AdjNewRefNo = model.AdjNewRefNo,
        //            AdjPendAmt = model.AdjPendAmt,
        //            AdjDrCr = model.AdjDrCr,
        //            AdjDrCrName = model.AdjDrCrName,
        //            AdjPurchOrderNo = model.AdjPurchOrderNo,
        //            AdjPOYear = model.AdjPOYear,
        //            AdjPODate = model.AdjPODate,
        //            AdjOpenEntryID = model.AdjOpenEntryID ?? 0,
        //            AdjOpeningYearCode = model.AdjOpeningYearCode ?? 0,
        //            AdjAgnstAccEntryID = model.AdjAgnstAccEntryID ?? 0,
        //            AdjAgnstAccYearCode = model.AdjAgnstAccYearCode ?? 0,
        //        });
        //    }
        //    else
        //    {
        //        foreach (var item in model.AdjAdjustmentDetailGrid)
        //        {
        //            _List.Add(new AgainstAdjustVoucherModel
        //            {
        //                AdjSeqNo = AdjGrid == null ? 1 : AdjGrid.Count + 1,
        //                AdjModeOfAdjstment = "AgainstRef",
        //                AdjModeOfAdjstmentName = "Against Ref",
        //                AdjDescription = item.AdjDescription,
        //                AdjDueDate = item.AdjAgnstVouchDate,
        //                AdjNewRefNo = item.AdjNewRefNo,
        //                AdjPendAmt = Convert.ToDecimal(item.AdjAgnstAdjstedAmt),
        //                AdjDrCr = item.AdjAgnstDrCr,
        //                AdjDrCrName = item.AdjAgnstDrCr,
        //                AdjPurchOrderNo = string.Empty,
        //                AdjPOYear = 0,
        //                AdjPODate = null,
        //                AdjAgnstVouchNo = item.AdjAgnstVouchNo,
        //                AdjAgnstVouchType = item.AdjAgnstVouchType,
        //                AdjOpenEntryID = item.AdjAgnstOpenEntryID ?? 0,
        //                AdjOpeningYearCode = item.AdjAgnstOpeningYearCode ?? 0,
        //                AdjAgnstAccEntryID = item.AdjAgnstAccEntryID ?? 0,
        //                AdjAgnstAccYearCode = item.AdjAgnstAccYearCode ?? 0,
        //            });
        //        }
        //    }
        //    return _List;
        //}
        public async Task<JsonResult> GetLedgerBalance(int OpeningYearCode, int AccountCode, string VoucherDate)
        {
            var JSON = await _IAgainstAdjustVoucher.GetLedgerBalance(OpeningYearCode, AccountCode, VoucherDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillVoucherNo(int YearCode, string VoucherType, string FromDate, string ToDate, int AccountCode)
        {
            YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
            var JSON = await _IAgainstAdjustVoucher.FillVoucherNo(YearCode,VoucherType, FromDate,ToDate,AccountCode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        } 
        public async Task<JsonResult> FillInvoiceNo(int YearCode, string VoucherType, string FromDate, string ToDate, string VoucherNo, int AccountCode)
        {
            YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
            var JSON = await _IAgainstAdjustVoucher.FillInvoiceNo(YearCode,VoucherType, FromDate,ToDate, VoucherNo,AccountCode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillLedgerName(string VoucherType, string ShowAll)
        {
            var JSON = await _IAgainstAdjustVoucher.FillLedgerName(VoucherType, ShowAll);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
   
        [HttpPost]
        public IActionResult AddAgainstAdjustVoucherDetail([FromBody] AgainstAdjustVoucherModel models)
        {
            try
            {
                if (models == null || models.AgainstAdjustVoucherList == null || !models.AgainstAdjustVoucherList.Any())
                    return BadRequest("No rows to add.");

                string sessionKey = (models.AgainstAdjustVoucherList.First().Mode == "U" ||
                                     models.AgainstAdjustVoucherList.First().Mode == "V")
                                    ? "KeyAdjGrid"
                                    : "KeyAgainstAdjustVoucherGrid";

                var existingGridJson = HttpContext.Session.GetString(sessionKey);
                var gridList = string.IsNullOrEmpty(existingGridJson)
                    ? new List<AgainstAdjustVoucherModel>()
                    : JsonConvert.DeserializeObject<List<AgainstAdjustVoucherModel>>(existingGridJson);

                foreach (var model in models.AgainstAdjustVoucherList)
                {
                    var mode = model.ModeOfAdjustment?.Replace(" ", "").ToLower();

                    bool isDuplicate = mode switch
                    {
                        "newref" => gridList.Any(x => x.LedgerName == model.LedgerName && x.ModeOfAdjustment?.Replace(" ", "").ToLower() == mode),
                        "advance" => gridList.Any(x => x.LedgerName == model.LedgerName && x.ModeOfAdjustment?.Replace(" ", "").ToLower() == mode),
                        "againstref" => gridList.Any(x => x.LedgerName == model.LedgerName && x.AgainstVoucherNo == model.AgainstVoucherNo),
                        _ => model.BankType?.ToLower() == "bank" && gridList.Any(x => x.BankType == "Bank" || x.LedgerName == model.LedgerName)
                    };

                    //if (isDuplicate)
                    //{
                    //    return mode switch
                    //    {
                    //        "newref" => StatusCode(207, "Duplicate"),
                    //        "advance" => StatusCode(208, "Duplicate"),
                    //        "againstref" => StatusCode(209, "Duplicate"),
                    //        _ => StatusCode(210, "Duplicate")
                    //    };
                    //}

                    model.SrNO = gridList.Count + 1;
                    gridList.Add(model);
                }

                HttpContext.Session.SetString(sessionKey, JsonConvert.SerializeObject(gridList));

                var mainModel = new AgainstAdjustVoucherModel
                {
                    AgainstAdjustVoucherList = gridList.OrderBy(x => x.SrNO).ToList()
                };

                return PartialView("_AgainstAdjustVoucherGrid", mainModel); // or Json(mainModel)
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        public async Task<JsonResult> GetPendVouchBillAgainstRefPopupByID(int AC, int? YC, int? PayRecEntryId, int? PayRecYearcode, string DRCR, string TransVouchType, string TransVouchDate)
        {
            string Flag = "";
            var JSON = await _IDataLogic.GetPendVouchBillAgainstRefPopupByID(AC, YC, PayRecEntryId, PayRecYearcode, DRCR, TransVouchType, TransVouchDate, Flag);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
    }
}
