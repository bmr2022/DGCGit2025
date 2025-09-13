using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using static eTactWeb.Data.Common.CommonFunc;

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
        [HttpPost]
        public async Task<IActionResult> SaveCancelation(int SaleBillYearCode, int CanSaleBillReqYearcode, string CanRequisitionNo, string SaleBillNo, int CancelBy, String Cancelreason, String Canceldate,bool CancelEInvoice,string CustomerName)
        {
            try
            {
                await _ICancelSaleBill.SaveCancelation(SaleBillYearCode, CanSaleBillReqYearcode, CanRequisitionNo, SaleBillNo, CancelBy, Cancelreason, Canceldate);

                string invoiceMessage = null;

                if (CancelEInvoice)
                {
                    var cancelResult = await _ICancelSaleBill.CancelEInvoice(SaleBillYearCode, CanRequisitionNo, SaleBillNo, CustomerName);

                    if (cancelResult?.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                        return StatusCode(500, new { message = cancelResult?.Result ?? "E-Invoice cancellation failed." });

                    invoiceMessage = cancelResult?.Result?.ToString();
                }

                return Ok(new { message = invoiceMessage ?? "Cancellation saved successfully!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Server Error: {ex.Message}" });
            }
        }

        //public async Task<IActionResult> CancelEInvoice(int SaleBillYearCode, string CanRequisitionNo, string SaleBillNo, string CustomerName)
        //{
        //    try
        //    {
        //        var result = await _ICancelSaleBill.CancelEInvoice(SaleBillYearCode, CanRequisitionNo, SaleBillNo, CustomerName);

        //        if (result == null || result.StatusCode == System.Net.HttpStatusCode.InternalServerError)
        //        {
        //            return StatusCode(500, result?.Result ?? "Something went wrong while cancelling the e-invoice.");
        //        }

        //        return Ok(new
        //        {
        //            Status = "Success",
        //            Message = result.Result
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, $"Server Error: {ex.Message}");
        //    }
        //}

    }
}
