using eTactWeb.Data.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
namespace eTactWeb.Controllers
{
    public class REQUISITIONRegisterController : Controller
    {
        private readonly IDataLogic _IDataLogic;
        public IREQUISITIONRegister _IREQUISITIONRegister { get; }

        private readonly ILogger<REQUISITIONRegisterController> _logger;
        private readonly IConfiguration iconfiguration;
        private readonly IMemoryCache _MemoryCache;
        public IWebHostEnvironment _IWebHostEnvironment { get; }

        public REQUISITIONRegisterController(ILogger<REQUISITIONRegisterController> logger, IDataLogic iDataLogic, IMemoryCache iMemoryCache, EncryptDecrypt encryptDecrypt, IWebHostEnvironment iWebHostEnvironment, IConfiguration iconfiguration, IREQUISITIONRegister REQUISITIONRegister)
        {
            _logger = logger;
            _IDataLogic = iDataLogic;
            _IREQUISITIONRegister = REQUISITIONRegister;
            _MemoryCache = iMemoryCache;
            _IWebHostEnvironment = iWebHostEnvironment;
            this.iconfiguration = iconfiguration;
        }
        [Route("{controller}/Index")]
        public IActionResult REQUISITIONRegister()
        {
            var model = new REQUISITIONRegistermodel();
            model.REQUISITIONRegisterDetails = new List<REQUISITIONRegisterDetail>();
            return View(model);
        }

        public async Task<IActionResult> GetREQUISITIONRegisterData(string Flag,string ReQType,string fromDate, string ToDate, string REQNo, string Partcode, string ItemName, string FromstoreId, string Toworkcenter, int ReqYearcode)
        {
            var YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
            var model = new REQUISITIONRegistermodel();
            model = await _IREQUISITIONRegister.GetREQUISITIONRegisterData(Flag,ReQType,fromDate, ToDate, Partcode, ItemName, REQNo, FromstoreId,Toworkcenter,ReqYearcode);
            model.ReportMode=Flag;
            model.ReqType = ReQType;
            if (ReQType == "REQUISITIONWITHOUTBOM")
            {
                return PartialView("_ReQWithoutBomListReport", model);
            }
            else 
            {
                return PartialView("_ReQWithBomListReport", model);
            }           
        }
    }
}
