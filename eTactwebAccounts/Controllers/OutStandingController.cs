using eTactWeb.Data.Common;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using eTactWeb.DOM.Models;
using System.Net;
using System.Data;
using System.Globalization;

namespace eTactWeb.Controllers
{
    public class OutStandingController : Controller
    {
        private readonly IDataLogic _IDataLogic;
        public IOutStanding _OutStanding { get; }
        private readonly ILogger<OutStandingController> _logger;
        private readonly IConfiguration iconfiguration;
        public IWebHostEnvironment _IWebHostEnvironment { get; }
        public OutStandingController(ILogger<OutStandingController> logger, IDataLogic iDataLogic, IOutStanding iOutStanding, EncryptDecrypt encryptDecrypt, IWebHostEnvironment iWebHostEnvironment, IConfiguration iconfiguration)
        {
            _logger = logger;
            _IDataLogic = iDataLogic;
            _OutStanding = iOutStanding;
            _IWebHostEnvironment = iWebHostEnvironment;
            this.iconfiguration = iconfiguration;
        }

        [Route("{controller}/Index")]
        [HttpGet]
        public async Task<ActionResult> OutStanding()
        {
            var MainModel = new OutStandingModel();
            MainModel.OutStandingGrid = new List<OutStandingModel>();
            //MainModel.FromDate = HttpContext.Session.GetString("FromDate");
            MainModel.TillDate = HttpContext.Session.GetString("TillDate");
            MainModel.currYearcode = Convert.ToInt32(HttpContext.Session.GetString("currYearcode"));

            return View(MainModel); // Pass the model with old data to the view
        }

        public async Task<JsonResult> GetPartyName(string outstandingType, string TillDate)
        {
            var JSON = await _OutStanding.GetPartyName(outstandingType, TillDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }

        public async Task<JsonResult> GetGroupName(string outstandingType, string TillDate)
        {
            var JSON = await _OutStanding.GetGroupName(outstandingType, TillDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }

        public async Task<IActionResult> GetDetailsData(string outstandingType, string TillDate,string GroupName,string[] AccountNameList,int AccountCode,string ShowOnlyApprovedBill,bool ShowZeroBal)
        {
            var model = new OutStandingModel();
            model = await _OutStanding.GetDetailsData(outstandingType, TillDate, GroupName, AccountNameList,AccountCode, ShowOnlyApprovedBill, ShowZeroBal);



            return PartialView("_OutStandingGrid", model);




        }
    }
}
