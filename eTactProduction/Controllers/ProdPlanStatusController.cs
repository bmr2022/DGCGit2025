using eTactWeb.Data.Common;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using eTactWeb.DOM.Models;

namespace eTactWeb.Controllers
{
    public class ProdPlanStatusController : Controller
    {
        private readonly IDataLogic _IDataLogic;
        public IProdPlanStatus _IProdPlanStatus { get; }

        private readonly ILogger<ProdPlanStatusController> _logger;
        private readonly IConfiguration iconfiguration;
        private readonly IMemoryCache _MemoryCache;
        public IWebHostEnvironment _IWebHostEnvironment { get; }
        public ProdPlanStatusController(ILogger<ProdPlanStatusController> logger, IDataLogic iDataLogic, IProdPlanStatus iProdPlanStatus, IMemoryCache iMemoryCache, EncryptDecrypt encryptDecrypt, IWebHostEnvironment iWebHostEnvironment, IConfiguration iconfiguration)
        {
            _logger = logger;
            _IDataLogic = iDataLogic;
            _IProdPlanStatus = iProdPlanStatus;
            _MemoryCache = iMemoryCache;
            _IWebHostEnvironment = iWebHostEnvironment;
            this.iconfiguration = iconfiguration;
        }
        [Route("{controller}/Index")]
        public async Task<ActionResult> ProdPlanStatus()
        {
            var model = new ProdPlanStatusModel();
            model.ProdPlanStatusGrid = new List<ProdPlanStatusModel>();
            //model.YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
            return View(model);
        }
        public async Task<IActionResult> GetProdPlanStatus()
        {
            var model = new ProdPlanStatusModel();
            model = await _IProdPlanStatus.GetProdPlanStatus();
            return PartialView("_ProdPlanStatusGrid", model);
        }
    }
}
