using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace eTactWeb.Controllers
{
    public class HRWeekOffMasterController : Controller
    {
        private readonly IDataLogic _IDataLogic;
        public IHRWeekOffMaster _IHRWeekOffMaster { get; }

        private readonly ILogger<HRWeekOffMasterController> _logger;
        private readonly IConfiguration iconfiguration;
        private readonly IMemoryCache _MemoryCache;
        public IWebHostEnvironment _IWebHostEnvironment { get; }
        public HRWeekOffMasterController(ILogger<HRWeekOffMasterController> logger, IDataLogic iDataLogic, IHRWeekOffMaster iHRWeekOffMaster, IMemoryCache iMemoryCache, EncryptDecrypt encryptDecrypt, IWebHostEnvironment iWebHostEnvironment, IConfiguration iconfiguration)
        {
            _logger = logger;
            _IDataLogic = iDataLogic;
            _IHRWeekOffMaster = iHRWeekOffMaster;
            _MemoryCache = iMemoryCache;
            _IWebHostEnvironment = iWebHostEnvironment;
            this.iconfiguration = iconfiguration;
        }

        [Route("{controller}/Index")]
        [HttpGet]
        public async Task<ActionResult> HRWeekOffMaster(int ID, string Mode)
        {
            _logger.LogInformation("\n \n ********** Page Gate Inward ********** \n \n " + _IWebHostEnvironment.EnvironmentName.ToString() + "\n \n");
            TempData.Clear();
            var MainModel = new HRWeekOffMasterModel();
          
            //if (!string.IsNullOrEmpty(Mode) && ID > 0 && Mode == "U")
            //{
            //   // MainModel = await _IHRWeekOffMaster.GetViewByID(ID).ConfigureAwait(false);
            //    MainModel.Mode = Mode; // Set Mode to Update
            //    MainModel.StoreId = ID;
            //    MainModel.Store_Name = Store_Name;
            //    MainModel.StoreType = StoreType;
            //    MainModel.CC = CC;
            //    MainModel.EntryDate = EntryDate;

            //    MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
            //    {
            //        AbsoluteExpiration = DateTime.Now.AddMinutes(60),
            //        SlidingExpiration = TimeSpan.FromMinutes(55),
            //        Size = 1024
            //    };
            //}

            return View(MainModel);
        }


    }
}
