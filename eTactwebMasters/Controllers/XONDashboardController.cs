using System.Net;
using eTactWeb.Data.BLL;
using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using static eTactWeb.Data.Common.CommonFunc;

namespace eTactwebMasters.Controllers
{
    public class XONDashboardController : Controller
    {
        public IDashboard _IDashboard { get; set; }
        private readonly ILogger<XONDashboardController> _logger;
        public XONDashboardController(ILogger<XONDashboardController> logger, IDashboard IDashboard)
        {
            _logger = logger;
            _IDashboard = IDashboard;
        }
        public IActionResult XONDashboard()
        {
            return View();
        }
        public async Task<JsonResult> NoOfItemInStock()
        {
            var JSON = await _IDashboard.NoOfItemInStock();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> StockValuation()
        {
            var JSON = await _IDashboard.StockValuation();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> PendingInventoryTask()
        {
            var JSON = await _IDashboard.PendingInventoryTask();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
    }
}
