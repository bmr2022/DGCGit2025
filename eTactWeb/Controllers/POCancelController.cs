using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

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
            //model.CC = HttpContext.Session.GetString("Branch");

            return View("POCancel", model);
        }

    }
}
