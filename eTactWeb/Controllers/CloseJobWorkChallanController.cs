using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace eTactWeb.Controllers
{
    public class CloseJobWorkChallanController : Controller
    {
        private readonly IDataLogic _IDataLogic;
        public ICloseJobWorkChallan _ICloseJobWorkChallan { get; }
        private readonly ILogger<CloseJobWorkChallanController> _logger;
        private readonly IConfiguration iconfiguration;
        public IWebHostEnvironment _IWebHostEnvironment { get; }
        public CloseJobWorkChallanController(ILogger<CloseJobWorkChallanController> logger, IDataLogic iDataLogic, ICloseJobWorkChallan iCloseJobWorkChallan, EncryptDecrypt encryptDecrypt, IWebHostEnvironment iWebHostEnvironment, IConfiguration iconfiguration)
        {
            _logger = logger;
            _IDataLogic = iDataLogic;
            _ICloseJobWorkChallan =iCloseJobWorkChallan;
            _IWebHostEnvironment = iWebHostEnvironment;
            this.iconfiguration = iconfiguration;
        }

        [Route("{controller}/Index")]
        [HttpGet]
        public async Task<ActionResult> CloseJobWorkChallan(int ID, string Mode)
        {
            _logger.LogInformation("\n \n ********** Page Gate Inward ********** \n \n " + _IWebHostEnvironment.EnvironmentName.ToString() + "\n \n");

            // Clear TempData and set session variables
            TempData.Clear();
            var MainModel = new CloseJobWorkChallanModel();
            MainModel.FromDate = HttpContext.Session.GetString("FromDate");
            MainModel.ToDate = HttpContext.Session.GetString("ToDate");
            MainModel.ActualEntryId = Convert.ToInt32(HttpContext.Session.GetString("UID"));
            MainModel.VendJWIssYearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
            MainModel.ActualEntryId = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
            MainModel.EmpId = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
            MainModel.ActualEntryByEmpName = HttpContext.Session.GetString("EmpName");
            MainModel.EntryDate = DateTime.Today.ToString("MM/dd/yyyy").Replace("-", "/");

            //HttpContext.Session.Remove("CloseProductionPlanGrid");

           
            return View(MainModel);
        }
        public async Task<JsonResult> GetSearchData(string FromDate, string ToDate, int AccountCode, string ChallanNO, string ShowClsoedPendingAll,string ChallanYear)
        {
            int EmpID = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
            string UID = HttpContext.Session.GetString("UID");
            var JSON = await _ICloseJobWorkChallan.GetSearchData(FromDate, ToDate, AccountCode, ChallanNO, ShowClsoedPendingAll, ChallanYear).ConfigureAwait(true);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }

        [HttpGet]
        public async Task<IActionResult> ShowDetail(
    int JWCloseEntryId,
    int JWCloseYearCode,
    DateTime JWCloseEntryDate,
    int JWCloseEntryByEmpid,
    string VendJwCustomerJW,
    string ShowClsoedPendingAll,
    string ChallanYear,
    int AccountCode,
    int VendJWIssEntryId,
    int VendJWIssYearCode,
    string VendJWIssChallanNo,
    DateTime VendJWIssChallanDate,
    int CustJwIssEntryid,
    int CustJwIssYearCode,
    string CustJwIssChallanNo,
    DateTime CustJwIssChallanDate,
    int ItemCode,
    string HSNNO,
    decimal TotalQty,
    decimal TotalPendQty,
    string Unit,
    decimal ToatlAltQty,
    decimal TotalAltpendQty,
    decimal Rate,
    decimal Amount,
    string ItemClosed,
    string ChallanClosed,
    decimal TotalChallanAmount,
    decimal NetAmount,
    string ClosingReason,
    string OtherDetail,
    int UId,
    string CC,
    DateTime ActualEntryDate,
    string ActualEnteredBy,
    string EntryByMachineName,
    string UpdatedBy,
    DateTime? UpdatedOn)
        {
            var MainModel = new List<CloseJobWorkChallanModel>();
            MainModel = await _ICloseJobWorkChallan.ShowDetail(VendJWIssEntryId, VendJWIssYearCode, ShowClsoedPendingAll, ChallanYear).ConfigureAwait(true);
            return View(MainModel);
        }

        [HttpGet]
        public async Task<IActionResult> SaveActivation(int JWCloseEntryId, string JWCloseEntryDate, string VendJwCustomerJW, int AccountCode, int VendJWIssEntryId, int VendJWIssYearCode, string VendJWIssChallanNo,
           string VendJWIssChallanDate, int CustJwIssEntryid, int CustJwIssYearCode, string CustJwIssChallanNo, string CustJwIssChallanDate, float TotalChallanAmount, float NetAmount, string ClosingReason,
            string ActualEntryDate,string ShowClsoedPendingAll,string ChallanYear)
        {
            int JWCloseEntryByEmpid = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
            int ActualEnteredBy = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
            int JWCloseYearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
            var CC= HttpContext.Session.GetString("Branch");
            string EntryByMachineName= Environment.MachineName;
            var Result = await _ICloseJobWorkChallan.SaveActivation(JWCloseEntryId, JWCloseYearCode, JWCloseEntryDate, JWCloseEntryByEmpid, VendJwCustomerJW, AccountCode, VendJWIssEntryId, VendJWIssYearCode, VendJWIssChallanNo,
            VendJWIssChallanDate, CustJwIssEntryid, CustJwIssYearCode, CustJwIssChallanNo, CustJwIssChallanDate, TotalChallanAmount, NetAmount, ClosingReason,
            CC, ActualEntryDate, ActualEnteredBy, EntryByMachineName, ShowClsoedPendingAll, ChallanYear).ConfigureAwait(true);
            if (Result != null)
            {
                if (Result.StatusText == "Success")
                {
                    var status = "";
                    var message = "";

                    var ds = (DataSet)Result.Result;
                    if (ds.Tables.Count > 0)
                    {
                        var dt = ds.Tables[0];
                        if (dt.Rows.Count > 0)
                        {
                            status = Convert.ToString(dt.Rows[0][0]);
                            if (dt.Columns.Count > 1)
                                message = dt.Rows[0][1].ToString();
                        }
                    }


                    var model1 = new SOApprovalModel();
                    ViewBag.isSuccess = true;
                    if (string.IsNullOrEmpty(message))
                        TempData["200"] = "200";
                    else
                    {
                        TempData["302"] = message;
                    }
                    //return RedirectToAction("SOApproval", new { type = type, CustOrderNo = CustOrderNum, SONO = SONum, VendorName = VendorNm });
                    return Json(new { redirectUrl = Url.Action("CloseJobWorkChallan") });
                }
            }
            var model = new SOApprovalModel();
            //return RedirectToAction("SOApproval", new { type = type, CustOrderNo = CustOrderNum, SONO = SONum, VendorName = VendorNm });
            return Json(new { redirectUrl = Url.Action("CloseJobWorkChallan") });
        }

        public async Task<JsonResult> FillVendorList(string fromDate, string toDate, string ShowClsoedPendingAll,string ChallanYear)
        {
            var JSON = await _ICloseJobWorkChallan.FillVendorList(fromDate, toDate, ShowClsoedPendingAll, ChallanYear);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
         public async Task<JsonResult> FillJWChallanList(string fromDate, string toDate, string ShowClsoedPendingAll, string ChallanYear)
        {
            var JSON = await _ICloseJobWorkChallan.FillJWChallanList(fromDate, toDate, ShowClsoedPendingAll, ChallanYear);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }


    }
}
    