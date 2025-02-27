using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using static eTactWeb.Data.Common.CommonFunc;
using eTactWeb.DOM.Models;
using System.Net;
using System.Data;
using System.Globalization;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace eTactWeb.Controllers
{
    public class PendingSaleRejectionController : Controller
    {
        private readonly IMemoryCache _MemoryCache;
        private readonly IPendingSaleRejection _IPendingSaleRejection;

        public PendingSaleRejectionController(IMemoryCache memoryCache, IPendingSaleRejection iPendingSaleRejection)
        {
            _MemoryCache = memoryCache;
            _IPendingSaleRejection = iPendingSaleRejection;
        }

        public async Task<IActionResult> PendingSaleRejection()
        {
            ViewData["Title"] = "Pending Requisition to Issue Details";
            TempData.Clear();
            _MemoryCache.Remove("KeyPendingSaleRejection");
            var MainModel = new PendingSaleRejectionModel();
            //var model = new IssueWithoutBomDetail();
            //MainModel = await BindModel(MainModel);
            //MainModel.CC = HttpContext.Session.GetString("Branch");
            MainModel.FromDate = HttpContext.Session.GetString("FromDate");
            MainModel.ToDate = HttpContext.Session.GetString("ToDate");
            MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                SlidingExpiration = TimeSpan.FromMinutes(55),
                Size = 1024,
            };
            _MemoryCache.Set("KeyPendingSaleRejection", MainModel, cacheEntryOptions);
            return View(MainModel);
        }

        public async Task<JsonResult> PendingMRNForSaleRejection(string fromDate, string toDate,string mrnNo,string gateNo, string customerName)
        {
            fromDate = ParseFormattedDate((fromDate).Split(" ")[0]);
            toDate = ParseFormattedDate((toDate).Split(" ")[0]);
            var JSON = await _IPendingSaleRejection.PendingMRNForSaleRejection(fromDate, toDate,mrnNo,gateNo,customerName);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillMRNNO(string fromDate, string toDate,string mrnNo,string gateNo)
        {
            fromDate = ParseFormattedDate((fromDate).Split(" ")[0]);
            toDate = ParseFormattedDate((toDate).Split(" ")[0]);
            var JSON = await _IPendingSaleRejection.FillMRNNO(fromDate, toDate,mrnNo,gateNo);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillPartyName(string fromDate, string toDate)
        {
            fromDate = ParseFormattedDate((fromDate).Split(" ")[0]);
            toDate = ParseFormattedDate((toDate).Split(" ")[0]);
            var JSON = await _IPendingSaleRejection.FillPartyName(fromDate, toDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillGateNO(string fromDate, string toDate,string mrnNo,string gateNo)
        {
            fromDate = ParseFormattedDate((fromDate).Split(" ")[0]);
            toDate = ParseFormattedDate((toDate).Split(" ")[0]);
            var JSON = await _IPendingSaleRejection.FillGateNO(fromDate, toDate,mrnNo,gateNo);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillCustomerInvNO(string fromDate, string toDate,string mrnNo,string gateNo)
        {
            fromDate = ParseFormattedDate((fromDate).Split(" ")[0]);
            toDate = ParseFormattedDate((toDate).Split(" ")[0]);
            var JSON = await _IPendingSaleRejection.FillCustomerInvNO(fromDate, toDate,mrnNo,gateNo);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
    }

}
