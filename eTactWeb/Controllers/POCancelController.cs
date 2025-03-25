using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;

namespace eTactWeb.Controllers
{
    public class POCancelController : Controller
    {
        private readonly IDataLogic _IDataLogic;
        private readonly IPOCancel _IPOCancel;
        private readonly ILogger<POCancelController> _logger;
        private readonly IMemoryCache _MemoryCache;
        private readonly IWebHostEnvironment _IWebHostEnvironment;
        public POCancelController(ILogger<POCancelController> logger, IDataLogic iDataLogic, IPOCancel iPOCancel, IMemoryCache iMemoryCache, IWebHostEnvironment iWebHostEnvironment)
        {
            _logger = logger;
            _IDataLogic = iDataLogic;
            _IPOCancel = iPOCancel;
            _MemoryCache = iMemoryCache;
            _IWebHostEnvironment = iWebHostEnvironment;
        }

        [Route("{controller}/Index")]
        [HttpGet]
        public IActionResult POCancel()
        {
            var model = new POCancelModel();
            model.CC = HttpContext.Session.GetString("Branch");

            return View("POCancel", model);
        }


        public async Task<JsonResult> GetSearchData(string FromDate, string ToDate, string CancelationType, string PONO, string VendorName)
        {
            int EmpID = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
            string UID = HttpContext.Session.GetString("UID");
            var JSON = await _IPOCancel.GetSearchData(FromDate, ToDate, CancelationType, PONO, VendorName, EmpID, UID);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }

        [HttpGet]
        public async Task<IActionResult> ShowPODetail(int ID, int YC, string PONo, string TypeOfCancle)
        {
            _MemoryCache.Remove("KeyPoCancleDetail");
            var MainModel = new List<POCancleDetail>();
            MainModel = await _IPOCancel.ShowPODetail(ID, YC, PONo, TypeOfCancle).ConfigureAwait(true);
            //string JsonString = JsonConvert.SerializeObject(JSON);
            return View(MainModel);
        }
    }
}
