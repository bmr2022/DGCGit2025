using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace eTactwebAccounts.Controllers
{
    public class BalanceSheetController : Controller
    {
        private readonly IDataLogic _IDataLogic;
        public IBalanceSheet _IBalanceSheet { get; }
        private readonly ILogger<BalanceSheetController> _logger;
        private readonly IConfiguration iconfiguration;
        public IWebHostEnvironment _IWebHostEnvironment { get; }
        public BalanceSheetController(ILogger<BalanceSheetController> logger, IDataLogic iDataLogic, IBalanceSheet IBalanceSheet, EncryptDecrypt encryptDecrypt, IWebHostEnvironment iWebHostEnvironment, IConfiguration iconfiguration)
        {
            _logger = logger;
            _IDataLogic = iDataLogic;
            _IBalanceSheet = IBalanceSheet;
            _IWebHostEnvironment = iWebHostEnvironment;
            this.iconfiguration = iconfiguration;
        }
        [Route("{controller}/Index")]
        [HttpGet]
        public async Task<ActionResult> BalanceSheet()
        {
            var MainModel = new BalanceSheetModel();
            MainModel.FromDate = HttpContext.Session.GetString("FromDate");
            return View(MainModel); // Pass the model with old data to the view
        }
    }
}
