using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace eTactWeb.Controllers
{
    public class CancelSaleBillController : Controller
    {
        private readonly IDataLogic _IDataLogic;
        private readonly ICancelSaleBill _ICancelSaleBill;
        private readonly ILogger<CancelSaleBillController> _logger;
        private readonly IWebHostEnvironment _IWebHostEnvironment;
        public CancelSaleBillController(ILogger<CancelSaleBillController> logger, IDataLogic iDataLogic, ICancelSaleBill ICancelSaleBill, IWebHostEnvironment iWebHostEnvironment)
        {
            _logger = logger;
            _IDataLogic = iDataLogic;
            _ICancelSaleBill = ICancelSaleBill;
            _IWebHostEnvironment = iWebHostEnvironment;
        }

        [Route("{controller}/CancelSaleBill")]
        [HttpGet]
        public IActionResult CancelSaleBill()
        {
            var model = new CancelSaleBillModel();
            return View("CancelSaleBill");
        }

        public async Task<JsonResult> GetSearchData(string FromDate, string ToDate, string SaleBillNo, string CustomerName, string CanRequisitionNo)
        {
            var JSON = await _ICancelSaleBill.GetSearchData(FromDate, ToDate, SaleBillNo, CustomerName, CanRequisitionNo);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }

        [HttpGet]
        public async Task<IActionResult> ShowSaleBillDetail(int SaleBillEntryId,int SaleBillYearCode, int CanSaleBillReqYearcode, string CanRequisitionNo, string SaleBillNo)
        {
            HttpContext.Session.Remove("KeyCancelSaleBillDetails");
            var MainModel = new List<CancelSaleBillDetails>();
            MainModel = await _ICancelSaleBill.ShowSaleBillDetail(SaleBillEntryId,SaleBillYearCode, CanSaleBillReqYearcode, CanRequisitionNo, SaleBillNo).ConfigureAwait(true);
            return View(MainModel);
        }
        public async Task<JsonResult> FillCanRequisitionNo(string CurrentDate, int accountcode, string SaleBillNo)
        {
            var JSON = await _ICancelSaleBill.FillCanRequisitionNo(CurrentDate, accountcode, SaleBillNo);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillSaleBillNo(string CurrentDate, int accountcode)
        {
            var JSON = await _ICancelSaleBill.FillSaleBillNo(CurrentDate, accountcode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillCustomerName(string CurrentDate, string SaleBillNo)
        {
            var JSON = await _ICancelSaleBill.FillCustomerName(CurrentDate, SaleBillNo);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }


    }
}
