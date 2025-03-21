using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace eTactWeb.Controllers
{
    public class MaterialConversionController : Controller
    {
        private readonly IDataLogic _IDataLogic;
        public IMaterialConversion _IMaterialConversion { get; }

        private readonly ILogger<MaterialConversionController> _logger;
        private readonly IConfiguration iconfiguration;
        private readonly IMemoryCache _MemoryCache;
        public IWebHostEnvironment _IWebHostEnvironment { get; }
        public MaterialConversionController(ILogger<MaterialConversionController> logger, IDataLogic iDataLogic, IMaterialConversion iMaterialConversion, IMemoryCache iMemoryCache, EncryptDecrypt encryptDecrypt, IWebHostEnvironment iWebHostEnvironment, IConfiguration iconfiguration)
        {
            _logger = logger;
            _IDataLogic = iDataLogic;
            _IMaterialConversion = iMaterialConversion;
            _MemoryCache = iMemoryCache;
            _IWebHostEnvironment = iWebHostEnvironment;
            this.iconfiguration = iconfiguration;
        }
        [Route("{controller}/Index")]
        public async Task<ActionResult> MaterialConversion()
        {
            var model = new MaterialConversionModel();
          
            model.OpeningYearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
            return View(model);
        }
    }
}
