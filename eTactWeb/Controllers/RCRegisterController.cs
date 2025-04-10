using eTactWeb.Data.Common;
using eTactWeb.DOM.Models.Master;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using eTactWeb.DOM.Models;
namespace eTactWeb.Controllers
{
    public class RCRegisterController : Controller
    {
        private readonly IDataLogic _IDataLogic;
        public IRCRegister _IRCRegister { get; }
        private readonly ILogger<RCRegisterController> _logger;
        private readonly IConfiguration iconfiguration;
        public IWebHostEnvironment _IWebHostEnvironment { get; }

        public RCRegisterController(ILogger<RCRegisterController> logger, IDataLogic iDataLogic, EncryptDecrypt encryptDecrypt, IWebHostEnvironment iWebHostEnvironment, IConfiguration iconfiguration, IRCRegister RCRegister)
        {
            _logger = logger;
            _IDataLogic = iDataLogic;
            _IRCRegister = RCRegister;
            _IWebHostEnvironment = iWebHostEnvironment;
            this.iconfiguration = iconfiguration;
        }
        public IActionResult RCRegister()
        {
            var model = new RCRegisterModel();
            model.RCRegisterDetails = new List<RCRegisterDetail>();
            return View(model);
        }

        public async Task<IActionResult> GetRCRegisterData(string FromDate, string ToDate, string Partyname, string IssueChallanNo, string RecChallanNo, string PartCode, string ItemName, string IssueChallanType, string RGPNRGP, string ReportMode)
        {
            var YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
            var model = new RCRegisterModel();
            model = await _IRCRegister.GetRCRegisterData(FromDate, ToDate, Partyname,IssueChallanNo,RecChallanNo, PartCode, ItemName, IssueChallanType, RGPNRGP, ReportMode);
            model.ReportMode = ReportMode;
            return PartialView("_RCRegisterGrid", model);
        }
    }
}
