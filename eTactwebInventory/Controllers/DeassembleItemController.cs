using eTactWeb.Controllers;
using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using FastReport.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;

namespace eTactwebInventory.Controllers
{
    public class DeassembleItemController : Controller
    {
        public WebReport webReport;
        public IDataLogic IDataLogic { get; }
        public IDeassembleItem _IDeassembleItem { get; }
        public IWebHostEnvironment IWebHostEnvironment { get; }
        public ILogger<DeassembleItemController> Logger { get; }
        private EncryptDecrypt EncryptDecrypt { get; }
        private readonly IConfiguration iconfiguration;
        public DeassembleItemController(IDeassembleItem iDeassembleItem, IConfiguration configuration, IDataLogic iDataLogic, ILogger<DeassembleItemController> logger, EncryptDecrypt encryptDecrypt, IWebHostEnvironment iWebHostEnvironment)
        {
            _IDeassembleItem = iDeassembleItem;
            IDataLogic = iDataLogic;
            Logger = logger;
            EncryptDecrypt = encryptDecrypt;
            IWebHostEnvironment = iWebHostEnvironment;
            iconfiguration = configuration;
        }

        [HttpGet]
        [Route("{controller}/Index")]
        public async Task<IActionResult> DeassembleItem(int ID, string Mode, int YC, string FromDate = "", string ToDate = "", string SlipNo = "", string BatchNo = "", string PartCode = "", string ItemName = "", string Searchbox = "", string SummaryDetail = "")
        {
            //RoutingModel model = new RoutingModel();  
            ViewData["Title"] = "DeassembleItem";
            TempData.Clear();
            HttpContext.Session.Remove("KeyDeassembleItemGrid");
            var MainModel = new DeassembleItemModel();

            //MainModel.FinFromDate = ParseDate(HttpContext.Session.GetString("FromDate")).ToString().Replace("-", "/");
            //MainModel.FinToDate = ParseDate(HttpContext.Session.GetString("ToDate")).ToString().Replace("-", "/");
            //MainModel.CC = HttpContext.Session.GetString("Branch");
            //MainModel.YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));

            //if (!string.IsNullOrEmpty(Mode) && ID > 0 && (Mode == "V" || Mode == "U"))
            //{
            //    MainModel = await IInterStore.GetViewByID(ID, Mode, YC);
            //    MainModel.Mode = Mode;
            //    MainModel.ID = ID;
            //    //  MainModel = await BindModel(MainModel);

            //}
            //else
            //{
            //    // MainModel = await BindModel(MainModel);
            //}
            //if (Mode != "U")
            //{
            //    MainModel.Uid = Convert.ToInt32(HttpContext.Session.GetString("UID"));
            //    MainModel.ActualEntryBy = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
            //    MainModel.ActualEntryByName = HttpContext.Session.GetString("EmpName");
            //    MainModel.ActualEntryDate = DateTime.Now.ToString();
            //}
            //else
            //{
            //    MainModel.Uid = Convert.ToInt32(HttpContext.Session.GetString("UID"));
            //    MainModel.UpdatedBy = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
            //    MainModel.LastUpdatedByName = HttpContext.Session.GetString("EmpName");
            //    MainModel.LastUpdationDate = DateTime.Now.ToString();
            //}
            MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                SlidingExpiration = TimeSpan.FromMinutes(55),
                Size = 1024,
            };

            //string serializedGrid = JsonConvert.SerializeObject(MainModel.InterStoreDetails);
            //HttpContext.Session.SetString("KeyInterStoreTransferGrid", serializedGrid);

            //MainModel.FromDateBack = FromDate;
            //MainModel.ToDateBack = ToDate;
            //MainModel.SlipNoBack = SlipNo;
            //MainModel.BatchNoback = BatchNo;
            //MainModel.PartCodeBack = PartCode;
            //MainModel.ItemNameBack = ItemName;
            //MainModel.DashboardTypeBack = SummaryDetail;
            //MainModel.GlobalSearchBack = Searchbox;
            return View(MainModel);
        }

        public async Task<JsonResult> NewEntryId()
        {
            var JSON = await _IDeassembleItem.NewEntryId();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }

        public async Task<JsonResult> FillStore()
        {
            var JSON = await _IDeassembleItem.FillStore();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }

    }
}
