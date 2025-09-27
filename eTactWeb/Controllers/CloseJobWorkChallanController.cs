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
        public async Task<JsonResult> GetSearchData(string FromDate, string ToDate, int AccountCode, string ChallanNO, string ShowClsoedPendingAll)
        {
            int EmpID = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
            string UID = HttpContext.Session.GetString("UID");
            var JSON = await _ICloseJobWorkChallan.GetSearchData(FromDate, ToDate, AccountCode, ChallanNO, ShowClsoedPendingAll).ConfigureAwait(true);
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
            MainModel = await _ICloseJobWorkChallan.ShowDetail(VendJWIssEntryId, VendJWIssYearCode).ConfigureAwait(true);
            return View(MainModel);
        }

    }
}
    