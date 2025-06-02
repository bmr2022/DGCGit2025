using eTactWeb.Data.Common;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using static eTactWeb.Data.Common.CommonFunc;
using static eTactWeb.DOM.Models.Common;
using eTactWeb.DOM.Models;
using System.Net;
using System.Data;
using System.Globalization;
using FastReport.Web;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System.Configuration;
using eTactWeb.Controllers;

namespace eTactwebInventory.Controllers
{
    public class ReOfferItemController : Controller
    {
        public WebReport webReport;
        public IDataLogic IDataLogic { get; }
        public IReofferItem _IReofferItem { get; }
        public IWebHostEnvironment IWebHostEnvironment { get; }
        public ILogger<ReOfferItemController> Logger { get; }
        private EncryptDecrypt EncryptDecrypt { get; }
        private readonly IConfiguration iconfiguration;
        public ReOfferItemController(IReofferItem iReofferItem, IConfiguration configuration, IDataLogic iDataLogic, ILogger<ReOfferItemController> logger, EncryptDecrypt encryptDecrypt, IWebHostEnvironment iWebHostEnvironment)
        {
            _IReofferItem = iReofferItem;
            IDataLogic = iDataLogic;
            Logger = logger;
            EncryptDecrypt = encryptDecrypt;
            IWebHostEnvironment = iWebHostEnvironment;
            iconfiguration = configuration;
        }

        [HttpGet]
        [Route("{controller}/Index")]
        public async Task<IActionResult> ReOfferItem(int ID, string Mode, int YC, string FromDate = "", string ToDate = "", string SlipNo = "", string BatchNo = "", string PartCode = "", string ItemName = "", string Searchbox = "", string SummaryDetail = "")
        {
            //RoutingModel model = new RoutingModel();  
            ViewData["Title"] = "ReOfferItem";
            TempData.Clear();
            HttpContext.Session.Remove("KeyReOfferItemGrid");
            var MainModel = new ReOfferItemModel();

            MainModel.FinFromDate = ParseDate(HttpContext.Session.GetString("FromDate")).ToString().Replace("-", "/");
            MainModel.FinToDate = ParseDate(HttpContext.Session.GetString("ToDate")).ToString().Replace("-", "/");
            MainModel.CC = HttpContext.Session.GetString("Branch");
            MainModel.ReofferYearcode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));

            if (!string.IsNullOrEmpty(Mode) && ID > 0 && (Mode == "V" || Mode == "U"))
            {
                //MainModel = await _IDeassembleItem.GetViewByID(ID, Mode, YC);
                MainModel.Mode = Mode;
                MainModel.ID = ID;
                

            }
          
            
            if (Mode != "U")
            {

                MainModel.EnteredByEmpId = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
                MainModel.ActualEnteredBy = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
                MainModel.UID = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                MainModel.EnteredByEmpName = HttpContext.Session.GetString("EmpName");

            }
            else
            {

                MainModel.UpdatedBy = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
                MainModel.UpdatedByName = HttpContext.Session.GetString("EmpName");

            }
            MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                SlidingExpiration = TimeSpan.FromMinutes(55),
                Size = 1024,
            };

            string serializedGrid = JsonConvert.SerializeObject(MainModel.ReofferItemDetail);
            HttpContext.Session.SetString("KeyReOfferItemGrid", serializedGrid);

            MainModel.FromDateBack = FromDate;
            MainModel.ToDateBack = ToDate;
            MainModel.SlipNoBack = SlipNo;
            MainModel.BatchNoback = BatchNo;
            MainModel.PartCodeBack = PartCode;
            MainModel.ItemNameBack = ItemName;
            MainModel.DashboardTypeBack = SummaryDetail;
            MainModel.GlobalSearchBack = Searchbox;
            return View(MainModel);
        }
        public async Task<JsonResult> GETNEWENTRY(int ReofferYearcode)
        {
            var JSON = await _IReofferItem.GETNEWENTRY( ReofferYearcode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FILLQCTYPE()
        {
            var JSON = await _IReofferItem.FILLQCTYPE();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FILLMIRNO()
        {
            var JSON = await _IReofferItem.FILLMIRNO();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FILLMIRYearCode(string MIRNO)
        {
            var JSON = await _IReofferItem.FILLMIRYearCode(MIRNO);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }

        public async Task<JsonResult> FILLMIRData(string MIRNO, int MIRYearCode)
        {
            var JSON = await _IReofferItem.FILLMIRData(MIRNO,  MIRYearCode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> BINDSTORE()
        {
            var JSON = await _IReofferItem.BINDSTORE();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }

        public async Task<JsonResult> GetItemDeatil(string MIRNO, int MIRYearCode, int accountcode, string ReofferMir)
        {
            var JSON = await _IReofferItem.GetItemDeatil( MIRNO,  MIRYearCode,  accountcode,  ReofferMir);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }

        public async Task<JsonResult> GetItemQty(string MIRNO, int MIRYearCode, int accountcode, string ReofferMir, int itemcode)
        {
            var JSON = await _IReofferItem.GetItemQty(MIRNO, MIRYearCode, accountcode, ReofferMir, itemcode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }

        public async Task<JsonResult> FillOkRecStore(int itemcode, string ShowAllStore)
        {
            var JSON = await _IReofferItem.FillOkRecStore( itemcode,  ShowAllStore);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }

        public async Task<JsonResult> ALLOWSHOWALLSTORE()
        {
            var JSON = await _IReofferItem.ALLOWSHOWALLSTORE();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> RejSTORE()
        {
            var JSON = await _IReofferItem.RejSTORE();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> RewSTORE()
        {
            var JSON = await _IReofferItem.RewSTORE();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> HoldSTORE()
        {
            var JSON = await _IReofferItem.HoldSTORE();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> BINDEMP()
        {
            var JSON = await _IReofferItem.BINDEMP();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillPODetail(string MIRNO, int MIRYearCode, int accountcode, int itemcode)
        {
            var JSON = await _IReofferItem.FillPODetail( MIRNO,  MIRYearCode,  accountcode,  itemcode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public IActionResult AddReOfferDetail(ReofferItemDetail model)
        {
            try
            {
                string modelJson = HttpContext.Session.GetString("KeyReOfferItemGrid");
                List<ReofferItemDetail> ISTDetail = new List<ReofferItemDetail>();
                if (!string.IsNullOrEmpty(modelJson))
                {
                    ISTDetail = JsonConvert.DeserializeObject<List<ReofferItemDetail>>(modelJson);
                }

                var MainModel = new ReOfferItemModel();
                var ISTGrid = new List<ReofferItemDetail>();
                var ISTList = new List<ReofferItemDetail>();

                if (model != null)
                {
                    if (ISTDetail == null)
                    {
                        // model.SeqNo = 1;
                        ISTGrid.Add(model);
                    }
                    else
                    {
                        if (ISTDetail.Any(x => x.Itemcode == model.Itemcode))
                        {
                            return StatusCode(207, "Duplicate");
                        }
                        else
                        {

                            ISTGrid = ISTDetail.Where(x => x != null).ToList();
                            ISTList.AddRange(ISTGrid);
                            ISTGrid.Add(model);
                        }
                    }

                    ISTGrid = ISTGrid.OrderBy(item => item.SeqNo).ToList();
                    MainModel.ReofferItemDetail = ISTGrid;

                    string serializedGrid = JsonConvert.SerializeObject(MainModel.ReofferItemDetail);
                    HttpContext.Session.SetString("KeyReOfferItemGrid", serializedGrid);
                }
                else
                {
                    ModelState.TryAddModelError("Error", "Schedule List Cannot Be Empty...!");
                }
                return PartialView("_ReOfferItemGrid", MainModel);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<JsonResult> EditItemRows(int SeqNo)
        {
            var MainModel = new DeassembleItemModel();
            string modelJson = HttpContext.Session.GetString("KeyReOfferItemGrid");
            List<ReofferItemDetail> ReofferItemDetail = new List<ReofferItemDetail>();
            if (!string.IsNullOrEmpty(modelJson))
            {
                ReofferItemDetail = JsonConvert.DeserializeObject<List<ReofferItemDetail>>(modelJson);
            }
            var ISTGrid = ReofferItemDetail.Where(x => x.SeqNo == SeqNo);
            string JsonString = JsonConvert.SerializeObject(ISTGrid);
            return Json(JsonString);
        }
        public IActionResult DeleteItemRow(int SeqNo, string Mode)
        {
            var MainModel = new ReOfferItemModel();
            if (Mode == "U")
            {
                int Indx = Convert.ToInt32(SeqNo) - 1;

                string modelJson = HttpContext.Session.GetString("KeyReOfferItemGrid");
                List<ReofferItemDetail> ISTDetail = new List<ReofferItemDetail>();
                if (!string.IsNullOrEmpty(modelJson))
                {
                    ISTDetail = JsonConvert.DeserializeObject<List<ReofferItemDetail>>(modelJson);
                }

                if (ISTDetail != null && ISTDetail.Count > 0)
                {
                    ISTDetail.RemoveAt(Convert.ToInt32(Indx));

                    Indx = 0;

                    foreach (var item in ISTDetail)
                    {
                        Indx++;
                        //item.SeqNo = Indx;
                    }
                    MainModel.ReofferItemDetail = ISTDetail;

                    string serializedGrid = JsonConvert.SerializeObject(MainModel.ReofferItemDetail);
                    HttpContext.Session.SetString("KeyReOfferItemGrid", serializedGrid);
                }
            }
            else
            {
                string modelJson = HttpContext.Session.GetString("KeyReOfferItemGrid");
                List<ReofferItemDetail> ISTDetail = new List<ReofferItemDetail>();
                if (!string.IsNullOrEmpty(modelJson))
                {
                    ISTDetail = JsonConvert.DeserializeObject<List<ReofferItemDetail>>(modelJson);
                }

                int Indx = Convert.ToInt32(SeqNo) - 1;

                if (ISTDetail != null && ISTDetail.Count > 0)
                {
                    ISTDetail.RemoveAt(Convert.ToInt32(Indx));

                    Indx = 0;

                    foreach (var item in ISTDetail)
                    {
                        Indx++;
                        //item.SeqNo = Indx;
                    }
                    MainModel.ReofferItemDetail = ISTDetail;

                    string serializedGrid = JsonConvert.SerializeObject(MainModel.ReofferItemDetail);
                    HttpContext.Session.SetString("KeyReOfferItemGrid", serializedGrid);
                }
            }
            return PartialView("_ReOfferItemGrid", MainModel);
        }

    }
}
