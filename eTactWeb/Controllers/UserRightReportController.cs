using eTactWeb.Data.Common;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using eTactWeb.DOM.Models;

namespace eTactWeb.Controllers
{
    public class UserRightReportController : Controller
    {
        private readonly IDataLogic _IDataLogic;
        public IUserRightReport _IUserRightReport { get; }
        private readonly ILogger<UserRightReportController> _logger;
        private readonly IConfiguration iconfiguration;
        public IWebHostEnvironment _IWebHostEnvironment { get; }
        public UserRightReportController(ILogger<UserRightReportController> logger, IDataLogic iDataLogic, IUserRightReport iUserRightReport, EncryptDecrypt encryptDecrypt, IWebHostEnvironment iWebHostEnvironment, IConfiguration iconfiguration)
        {
            _logger = logger;
            _IDataLogic = iDataLogic;
            _IUserRightReport = iUserRightReport;
            _IWebHostEnvironment = iWebHostEnvironment;
            this.iconfiguration = iconfiguration;
        }
        [Route("{controller}/Index")]
        public async Task<ActionResult> UserRightReport()
        {
            var model = new UserRightReportModel();
            model.UserRightReportGrid = new List<UserRightReportModel>();
            model.YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
            return View(model);
        }
        public async Task<IActionResult> GetUserRightsReportDetailData(string fromDate, string toDate, string ReportType, string UserName, string EmployeeName, string FormName, string ModuleName, string MachineName)
        {
            var model = new UserRightReportModel();
            model = await _IUserRightReport.GetUserRightsReportDetailData( fromDate,  toDate,  ReportType,  UserName,  EmployeeName,  FormName,  ModuleName,  MachineName);
            if (ReportType == "UserRights")
            {
                return PartialView("_UserRightsReportDetails", model);
            }
            if (ReportType == "LogBook")
            {
                return PartialView("_LogBookDetails", model);
            }
            return null;
        } 
        public async Task<JsonResult> FillUserName(string ReportType)
        {
            var JSON = await _IUserRightReport.FillUserName(ReportType);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillEmployeeName(string ReportType)
        {
            var JSON = await _IUserRightReport.FillEmployeeName(ReportType);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillFormName(string ReportType)
        {
            var JSON = await _IUserRightReport.FillFormName(ReportType);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillModuleName(string ReportType)
        {
            var JSON = await _IUserRightReport.FillModuleName(ReportType);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillMachineName(string ReportType)
        {
            var JSON = await _IUserRightReport.FillMachineName(ReportType);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
    }
}
