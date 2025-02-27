using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Controllers
{
    public class PendingProductionScheduleController : Controller
    {
        private readonly IDataLogic _IDataLogic;
        private readonly IPendingProductionSchedule _IPendingProductionSchedule;
        private readonly ILogger<PendingProductionScheduleController> _logger;
        private readonly IMemoryCache _MemoryCache;
        private readonly IWebHostEnvironment _IWebHostEnvironment;
        public PendingProductionScheduleController(ILogger<PendingProductionScheduleController> logger, IDataLogic iDataLogic, IPendingProductionSchedule IPendingProductionSchedule, IMemoryCache iMemoryCache, IWebHostEnvironment iWebHostEnvironment)
        {
            _logger = logger;
            _IDataLogic = iDataLogic;
            _IPendingProductionSchedule = IPendingProductionSchedule;
            _MemoryCache = iMemoryCache;
            _IWebHostEnvironment = iWebHostEnvironment;
        }
        public async Task<IActionResult> PendingProductionSchedule()
        {
            ViewData["Title"] = "Pending Production Schedule";
            TempData.Clear();
            var MainModel = new PendingProductionSchedule();
            MainModel.FromDate = HttpContext.Session.GetString("FromDate");
            MainModel.ToDate = HttpContext.Session.GetString("ToDate");
            return View(MainModel);
        }
        public async Task<JsonResult> FillStore()
        {
            var JSON = await _IPendingProductionSchedule.FillStore();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillWorkCenter()
        {
            var JSON = await _IPendingProductionSchedule.FillWorkCenter();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillItemName()
        {
            var JSON = await _IPendingProductionSchedule.FillItemName();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillPartCode()
        {
            var JSON = await _IPendingProductionSchedule.FillPartCode();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillPendingProdPlanNo()
        {
            var JSON = await _IPendingProductionSchedule.FillPendingProdPlanNo();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillPendingProdPlanYearCode(string ProdPlanNo)
        {
            var JSON = await _IPendingProductionSchedule.FillPendingProdPlanYearCode(ProdPlanNo);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillProdScheduleNo(string ProdPlanNo, int ProdPlanYearCode)
        {
            var JSON = await _IPendingProductionSchedule.FillProdScheduleNo(ProdPlanNo, ProdPlanYearCode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetDataForPendingProductionSchedule(string Flag, string FromDate, string ToDate, int StoreId, int YearCode, string GlobalSearch, string ProdSchNo, int WcId)
        {
            var JSON = await _IPendingProductionSchedule.GetDataForPendingProductionSchedule(Flag, FromDate, ToDate, StoreId, YearCode, GlobalSearch, ProdSchNo, WcId);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public IActionResult AddIssuePendingProduction(List<IssueAgainstProdScheduleDetail> model)
        {
            try
            {
                _MemoryCache.Remove("KeyPendingProductionSchedule");
                _MemoryCache.TryGetValue("KeyPendingProductionSchedule", out IList<IssueAgainstProdScheduleDetail> IssueAgainstProdScheduleDetail);
                TempData.Clear();

                var MainModel = new IssueAgainstProdSchedule();
                var IssueWithoutBomGrid = new List<IssueAgainstProdScheduleDetail>();
                var IssueGrid = new List<IssueAgainstProdScheduleDetail>();
                var SSGrid = new List<IssueAgainstProdScheduleDetail>();
                MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                    SlidingExpiration = TimeSpan.FromMinutes(55),
                    Size = 1024,
                };
                var seqNo = 0;
                if (model != null)
                {
                    foreach (var item in model)
                    {
                        if (item != null)
                        {
                            if (IssueAgainstProdScheduleDetail == null)
                            {
                                item.seqno += seqNo + 1;
                                IssueGrid.Add(item);
                                seqNo++;
                            }
                            else
                            {
                                if (IssueAgainstProdScheduleDetail.Where(x => x.IssueItemCode == item.IssueItemCode).Any())
                                {
                                    return StatusCode(207, "Duplicate");
                                }
                                else
                                {
                                    item.seqno = IssueAgainstProdScheduleDetail.Count + 1;
                                    IssueGrid = IssueAgainstProdScheduleDetail.Where(x => x != null).ToList();
                                    SSGrid.AddRange(IssueGrid);
                                    IssueGrid.Add(item);
                                }
                            }

                            MainModel.ItemDetailGrid = IssueGrid;

                            _MemoryCache.Set("KeyPendingProductionSchedule", MainModel.ItemDetailGrid, cacheEntryOptions);
                        }
                    }
                }
                _MemoryCache.TryGetValue("KeyPendingProductionSchedule", out IList<IssueThrBomDetail> grid);
                _MemoryCache.Set("KeyIssAgainstProduction", MainModel.ItemDetailGrid, cacheEntryOptions);


                return Json("done");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
