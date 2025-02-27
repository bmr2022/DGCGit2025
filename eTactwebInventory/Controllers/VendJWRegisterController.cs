using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.DOM.Models.Master;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace eTactWeb.Controllers
{
    public class VendJWRegisterController : Controller
    {
        private readonly IDataLogic _IDataLogic;
        public IVendJWRegister _IVendJWRegister { get; }

        private readonly ILogger<VendJWRegisterController> _logger;
        private readonly IConfiguration iconfiguration;
        private readonly IMemoryCache _MemoryCache;
        public IWebHostEnvironment _IWebHostEnvironment { get; }

        public VendJWRegisterController(ILogger<VendJWRegisterController> logger, IDataLogic iDataLogic, IMemoryCache iMemoryCache, EncryptDecrypt encryptDecrypt, IWebHostEnvironment iWebHostEnvironment, IConfiguration iconfiguration, IVendJWRegister VendJWRegister)
        {
            _logger = logger;
            _IDataLogic = iDataLogic;
            _IVendJWRegister = VendJWRegister;
            _MemoryCache = iMemoryCache;
            _IWebHostEnvironment = iWebHostEnvironment;
            this.iconfiguration = iconfiguration;
        }
        [Route("{controller}/Index")]
        public IActionResult VendJWRegister()
        {
            var model = new VendJWRegisterModel();
            model.VendJWRegisterDetails = new List<VendJWRegisterDetail>();
            return View(model);
        }

        public async Task<IActionResult> GetJWRegisterData(string FromDate, string ToDate,string RecChallanNo,string IssChallanNo, string PartyName, string PartCode, string ItemName, string IssueChallanType, string ReportMode)
        {
            var YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
            var model = new VendJWRegisterModel();
            model = await _IVendJWRegister.GetJWRegisterData(FromDate, ToDate,RecChallanNo,IssChallanNo, PartyName,PartCode, ItemName, IssueChallanType, ReportMode);
            model.ReportMode = ReportMode;
            model.IssueChallanType = IssueChallanType;
            if(IssueChallanType == "ISSUE")
            {
                return PartialView("_VenderJWIssueChallanGrid", model);
            }
            else if(IssueChallanType == "REC")
            {
                return PartialView("_VenderJWReceiveGrid", model);
            }
            else
            {
                return PartialView("_VendorRecoIssueReceiveGrid", model);
            }
        }
    }
}
