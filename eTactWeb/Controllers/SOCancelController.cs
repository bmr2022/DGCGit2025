using eTactWeb.DOM.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;

namespace eTactWeb.Controllers
{
    public class SOCancelController : Controller
    {
        private readonly IDataLogic _IDataLogic;
        private readonly ISOCancel _ISOCancel;
        private readonly ILogger<SOCancelController> _logger;
        private readonly IMemoryCache _MemoryCache;
        private readonly IWebHostEnvironment _IWebHostEnvironment;
        public SOCancelController(ILogger<SOCancelController> logger, IDataLogic iDataLogic, ISOCancel iSOCancel, IMemoryCache iMemoryCache, IWebHostEnvironment iWebHostEnvironment)
        {
            _logger = logger;
            _IDataLogic = iDataLogic;
            _ISOCancel = iSOCancel;
            _MemoryCache = iMemoryCache;
            _IWebHostEnvironment = iWebHostEnvironment;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult SOCancel(string type = "")
        {
            var model = new SOCancelModel();
            model.CC = HttpContext.Session.GetString("Branch");
            model.CancelationType = type;
            return View("SOCancel", model);
        }

        public async Task<JsonResult> GetSearchData(string FromDate, string ToDate, string CancelType, string SONO, string AccountName, string CustOrderNo)
        {
            int EmpID = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
            string UID = HttpContext.Session.GetString("UID");
            var JSON = await _ISOCancel.GetSearchData(FromDate, ToDate, CancelType, UID, EmpID, SONO, AccountName, CustOrderNo).ConfigureAwait(true);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }

        [HttpGet]
        public async Task<IActionResult> ShowSODetail(int ID, int YC, string SONo, string CustOrderNo, string TypeOfApproval, string SONum, string CustOrderNum, string VendorNm)
        {
            var MainModel = new List<SoCancelDetail>();
            MainModel = await _ISOCancel.ShowSODetail(ID, YC, SONo).ConfigureAwait(true);
            return View(MainModel);
        }
    }
}
