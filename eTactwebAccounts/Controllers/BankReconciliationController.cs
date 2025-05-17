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
    public class BankReconciliationController : Controller
    {
        private readonly IDataLogic _IDataLogic;
        public IBankReconciliation _IBackReconciliation { get; }
        private readonly ILogger<BankReconciliationController> _logger;
        private readonly IConfiguration iconfiguration;
        public IWebHostEnvironment _IWebHostEnvironment { get; }
        public BankReconciliationController(ILogger<BankReconciliationController> logger, IDataLogic iDataLogic, IBankReconciliation iBackReconciliation, EncryptDecrypt encryptDecrypt, IWebHostEnvironment iWebHostEnvironment, IConfiguration iconfiguration)
        {
            _logger = logger;
            _IDataLogic = iDataLogic;
            _IBackReconciliation = iBackReconciliation;
            _IWebHostEnvironment = iWebHostEnvironment;
            this.iconfiguration = iconfiguration;
        }

        [Route("{controller}/Index")]
        [HttpGet]
        public async Task<ActionResult> BankReconciliation()
        {
            var MainModel = new BankReconciliationModel();
            MainModel.BankReconciliationGrid = new List<BankReconciliationModel>();
            MainModel.DateFrom = HttpContext.Session.GetString("DateFrom");
            MainModel.DateFrom = HttpContext.Session.GetString("DateFrom");
            //MainModel.YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));

            return View(MainModel); // Pass the model with old data to the view
        }

        public async Task<JsonResult> GetBankName(string DateFrom, string DateTo, string NewOrEdit)
        {
            var JSON = await _IBackReconciliation.GetBankName( DateFrom,  DateTo,  NewOrEdit);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }

        public async Task<IActionResult> GetDetailsData(string DateFrom, string DateTo, string chequeNo, string NewOrEdit,string Account_Code)
        {
            var model = new BankReconciliationModel();
            model = await _IBackReconciliation.GetDetailsData( DateFrom,  DateTo,  chequeNo,  NewOrEdit, Account_Code);
            return PartialView("_BankReconciliationGrid", model);
        }
    }
}
