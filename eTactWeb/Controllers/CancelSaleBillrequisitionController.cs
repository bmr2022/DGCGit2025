using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using static eTactWeb.Data.Common.CommonFunc;
using static eTactWeb.DOM.Models.Common;


namespace eTactWeb.Controllers
{
    public class CancelSaleBillrequisitionController : Controller
    {
        private readonly IDataLogic _IDataLogic;
        public ICancelSaleBillrequisition _ICancelSaleBillrequisition { get; }
        private readonly ILogger<CancelSaleBillrequisitionController> _logger;
        private readonly IConfiguration iconfiguration;
        public IWebHostEnvironment _IWebHostEnvironment { get; }
        public CancelSaleBillrequisitionController(ILogger<CancelSaleBillrequisitionController> logger, IDataLogic iDataLogic, ICancelSaleBillrequisition iCancelSaleBillrequisition, EncryptDecrypt encryptDecrypt, IWebHostEnvironment iWebHostEnvironment, IConfiguration iconfiguration)
        {
            _logger = logger;
            _IDataLogic = iDataLogic;
            _ICancelSaleBillrequisition = iCancelSaleBillrequisition;
            _IWebHostEnvironment = iWebHostEnvironment;
            this.iconfiguration = iconfiguration;
        }
        [Route("{controller}/Index")]
        [HttpGet]
        public async Task<ActionResult> CancelSaleBillrequisition(int AccountCode,int ID,string Mode,string Requisitionby,int CanSaleBillReqYearcode,string  CanRequisitionNo,string CustomerName, int SaleBillEntryId,string SaleBillNo,int SaleBillYearCode,string SaleBillDate,int BillAmt,int INVNetAmt,string ReasonOfCancel, int Approvedby,string CC,int uid,string Canceled,string VoucherType,string ApprovalDate,string CancelDate,string MachineName)
        {
            _logger.LogInformation("\n \n ********** Page Gate Inward ********** \n \n " + _IWebHostEnvironment.EnvironmentName.ToString() + "\n \n");

            // Clear TempData and set session variables
            TempData.Clear();
            var MainModel = new CancelSaleBillrequisitionModel();
            MainModel.FromDate = HttpContext.Session.GetString("FromDate");
            MainModel.ToDate = HttpContext.Session.GetString("ToDate");
            MainModel.CanSaleBillReqYearcode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
            MainModel.uid = Convert.ToInt32(HttpContext.Session.GetString("UID"));
            MainModel.CC = HttpContext.Session.GetString("Branch");
            //MainModel.EntryByEmpName = HttpContext.Session.GetString("EmpName");
            // MainModel.ActualEntryDate = HttpContext.Session.GetString("EntryDate");


            if (!string.IsNullOrEmpty(Mode) && ID > 0 && Mode == "U")
            {
                // Retrieve the old data by AccountCode and populate the model with existing values
                MainModel = await _ICancelSaleBillrequisition.GetViewByID( CanRequisitionNo,  CanSaleBillReqYearcode).ConfigureAwait(false);
                MainModel.Mode = Mode; // Set Mode to Update
                MainModel.CanSaleBillReqEntryid = ID;
                MainModel.CanSaleBillReqYearcode = CanSaleBillReqYearcode;
                MainModel.CanRequisitionNo = CanRequisitionNo;
                MainModel.CustomerName = CustomerName;
                MainModel.SaleBillEntryId = SaleBillEntryId;
                MainModel.SaleBillNo = SaleBillNo;
                MainModel.SaleBillYearCode = SaleBillYearCode;
                MainModel.SaleBillDate = SaleBillDate;
                MainModel.BillAmt = BillAmt;
                MainModel.INVNetAmt = INVNetAmt;
                MainModel.ReasonOfCancel = ReasonOfCancel;
                MainModel.Approvedby = Approvedby;
                MainModel.CC = CC;
                MainModel.uid = uid;
                MainModel.Canceled = Canceled;
                MainModel.VoucherType = VoucherType;
                MainModel.ApprovalDate = ApprovalDate;
                MainModel.CancelDate = CancelDate;
                MainModel.MachineName = MachineName;
                MainModel.Requisitionby = Requisitionby;
                MainModel.AccountCode = AccountCode;

                //MainModel = await BindModels(MainModel).ConfigureAwait(false);
                //MainModel.FinFromDate = HttpContext.Session.GetString("FromDate");
                // MainModel.FinToDate = HttpContext.Session.GetString("ToDate");
                MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                    SlidingExpiration = TimeSpan.FromMinutes(55),
                    Size = 1024
                };
            }

            //if (Mode == "U")
            //{
            //    MainModel.UpdatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
            //    //MainModel.Up = HttpContext.Session.GetString("EmpName");
            //    MainModel.Updationdate = DateTime.Now.ToString();
            //}

            return View(MainModel); // Pass the model with old data to the view
        }
        [Route("{controller}/Index")]
        [HttpPost]
        public async Task<IActionResult> CancelSaleBillrequisition(CancelSaleBillrequisitionModel model)
        {
            try
            {
                model.CreatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                model.MachineName = HttpContext.Session.GetString("ClientMachineName");
                model.IPAddress = HttpContext.Session.GetString("ClientIP");
                var Result = await _ICancelSaleBillrequisition.SaveCancelSaleBillRequisition(model);
                if (Result != null)
                {
                    if (Result.StatusText == "Success" && Result.StatusCode == HttpStatusCode.OK)
                    {
                        ViewBag.isSuccess = true;
                        TempData["200"] = "200";
                        //_MemoryCache.Remove("KeyLedgerOpeningEntryGrid");
                    }
                    else if (Result.StatusText == "Success" && Result.StatusCode == HttpStatusCode.Accepted)
                    {
                        ViewBag.isSuccess = true;
                        TempData["202"] = "202";
                    }
                    else if (Result.StatusText == "Error" && Result.StatusCode == HttpStatusCode.InternalServerError)
                    {
                        ViewBag.isSuccess = false;
                        TempData["500"] = "500";
                        _logger.LogError($"\n \n ********** LogError ********** \n {JsonConvert.SerializeObject(Result)}\n \n");
                        return View("Error", Result);
                    }
                }

                return RedirectToAction(nameof(CancelSaleBillrequisitionDashBoard));

            }
            catch (Exception ex)
            {
                // Log and return the error
                LogException<CancelSaleBillrequisitionController>.WriteException(_logger, ex);
                var ResponseResult = new ResponseResult
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    StatusText = "Error",
                    Result = ex
                };
                return View("Error", ResponseResult);
            }
        }
        public async Task<JsonResult> FillCanSaleBillReqEntryid(string CurrentDate, string SaleBillDate,int CanSaleBillReqYearcode)
        {
            var JSON = await _ICancelSaleBillrequisition.FillCanSaleBillReqEntryid(CurrentDate, SaleBillDate, CanSaleBillReqYearcode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        } 
        public async Task<JsonResult> FillCanRequisitionNo(string CurrentDate, string SaleBillDate,int CanSaleBillReqYearcode)
        {
            var JSON = await _ICancelSaleBillrequisition.FillCanRequisitionNo(CurrentDate, SaleBillDate, CanSaleBillReqYearcode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillSaleBillNo(string CurrentDate, string SaleBillDate)
        {
            var JSON = await _ICancelSaleBillrequisition.FillSaleBillNo(CurrentDate, SaleBillDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        } 
        public async Task<JsonResult> FillSaleBillNoYear(string CurrentDate, string SaleBillDate,string SaleBillNo)
        {
            var JSON = await _ICancelSaleBillrequisition.FillSaleBillNoYear(CurrentDate, SaleBillDate, SaleBillNo);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillCustomerName(string CurrentDate, string SaleBillDate)
        {
            var JSON = await _ICancelSaleBillrequisition.FillCustomerName(CurrentDate, SaleBillDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillSaleBillNoDate(string CurrentDate, string SaleBillNo, string SaleBillYearCode, string SaleBillDate)
        {
            var JSON = await _ICancelSaleBillrequisition.FillSaleBillNoDate(  CurrentDate,  SaleBillNo,  SaleBillYearCode,  SaleBillDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        [HttpGet]
        public async Task<IActionResult> CancelSaleBillrequisitionDashBoard(string FromDate, string ToDate)
        {
            try
            {
                var model = new CancelSaleBillrequisitionModel();
                model.FromDate = HttpContext.Session.GetString("FromDate");
                model.ToDate = HttpContext.Session.GetString("ToDate");
                //model.OpeningForYear = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
                //model.DashboardType = "SUMMARY";
                DateTime now = DateTime.Now;
                //model.ToDate = new DateTime(DateTime.Today.Year + 1, 3, 31).ToString("dd/MM/yyyy").Replace("-", "/");
                //model.FromDate = new DateTime(now.Year, now.Month, 1).ToString("dd/MM/yyyy").Replace("-", "/");
                if (string.IsNullOrEmpty(FromDate))
                {
                    FromDate = HttpContext.Session.GetString("FromDate") ?? new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).ToString("dd/MM/yyyy");
                }
                if (string.IsNullOrEmpty(ToDate))
                {
                    ToDate = HttpContext.Session.GetString("ToDate") ?? DateTime.Now.ToString("dd/MM/yyyy");
                }

                var Result = await _ICancelSaleBillrequisition.GetDashBoardData(FromDate, ToDate).ConfigureAwait(true);
                if (Result != null)
                {
                    DataSet ds = Result.Result;
                    if (ds != null && ds.Tables.Count > 0)
                    {
                        var dt = ds.Tables[0];
                        model.CancelSaleBillrequisitionsGrid = CommonFunc.DataTableToList<CancelSaleBillrequisitionModel>(dt, "CancelSaleBillrequisitionDashBoard");
                    }
                }  
                return View(model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IActionResult> GetDashBoardDetailData(string FromDate, string ToDate)
        {
            //model.Mode = "Search";
            var model = new CancelSaleBillrequisitionModel();
            model.FromDate = HttpContext.Session.GetString("FromDate");
            model.ToDate = HttpContext.Session.GetString("ToDate");
            if (string.IsNullOrEmpty(FromDate))
            {
                FromDate = HttpContext.Session.GetString("FromDate") ?? new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).ToString("dd/MM/yyyy");
            }
            if (string.IsNullOrEmpty(ToDate))
            {
                ToDate = HttpContext.Session.GetString("ToDate") ?? DateTime.Now.ToString("dd/MM/yyyy");
            }
            model = await _ICancelSaleBillrequisition.GetDashBoardDetailData( FromDate,  ToDate);
            return PartialView("_CancelSaleBillrequisitionGrid", model);
        }
        public async Task<IActionResult> DeleteByID(int ID, string CanReqNo, string MachineName, int CanSaleBillReqYC)
        {
            var Result = await _ICancelSaleBillrequisition.DeleteByID( ID,  CanReqNo,  MachineName,  CanSaleBillReqYC);

            if (Result.StatusText == "Success" || Result.StatusCode == HttpStatusCode.Gone)
            {
                ViewBag.isSuccess = true;
                TempData["410"] = "410";
                //TempData["Message"] = "Data deleted successfully.";
            }
            else if (Result.StatusText == "Error" || Result.StatusCode == HttpStatusCode.Accepted)
            {
                ViewBag.isSuccess = true;
                TempData["410"] = "410";
            }
            else
            {
                ViewBag.isSuccess = false;
                TempData["500"] = "500";
            }

            return RedirectToAction("CancelSaleBillrequisitionDashBoard");

        }
    }
}
