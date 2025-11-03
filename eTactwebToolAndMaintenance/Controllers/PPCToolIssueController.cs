using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using static eTactWeb.DOM.Models.Common;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Globalization;
using System.Data;
using System.Configuration;

namespace eTactweb.Controllers
{
    public class PPCToolIssueController : Controller
    {
        private readonly IDataLogic _IDataLogic;
        private readonly IReqWithoutBOM _IReqWithoutBOM;
        private readonly IPPCToolIssue _IPPCToolIssue;
        private readonly ILogger<PPCToolIssueController> _logger;
        private readonly IWebHostEnvironment _IWebHostEnvironment;
        private readonly IConfiguration _iconfiguration;

        public PPCToolIssueController(ILogger<PPCToolIssueController> logger, IDataLogic iDataLogic, IPPCToolIssue iPPCToolIssue, IWebHostEnvironment iWebHostEnvironment, IConfiguration configuration)
        {
            _logger = logger;
            _IDataLogic = iDataLogic;
            _IPPCToolIssue = iPPCToolIssue;
            _IWebHostEnvironment = iWebHostEnvironment;
            _iconfiguration = configuration;
        }
        [HttpGet]
        public IActionResult PPCToolIssue()
        {
            var model = new PPCToolIssueMainModel
            {
                ToolIssueDetails = new List<PPCToolIssueDetailModel>()
            };

            model.ToolIssueYearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
            model.CC = HttpContext.Session.GetString("Branch");
            model.UID = Convert.ToInt32(HttpContext.Session.GetString("UID"));
            model.ToolIssueDate = DateTime.Now;

            return View(model);
        }



        [HttpGet]
        public async Task<JsonResult> GetNewEntry()
        {
            int yearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
            var result = await _IPPCToolIssue.GetNewEntry("NEWENTRY", yearCode);
            string JsonString = JsonConvert.SerializeObject(result);
            // Directly return the object, no need to serialize manually
            return Json(JsonString);
        }

        [HttpGet]
        public async Task<JsonResult> FillToolList()
        {
            try
            {
                var result = await _IPPCToolIssue.FillToolList("FillToolList");
                return Json(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while filling tool list.");
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<JsonResult> FillToolBarCode(long ToolIssueEntryId)
        {
            try
            {
                var result = await _IPPCToolIssue.FillToolBarCode("FillToolBarCode", ToolIssueEntryId);
                // Manually serialize the result
                string jsonString = JsonConvert.SerializeObject(result);

                return Json(jsonString); // return as string
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while filling tool barcode.");
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<JsonResult> FillProdPlan(long ToolIssueEntryId)
        {
            try
            {
                var result = await _IPPCToolIssue.FillProdPlan("FillProdPlan", ToolIssueEntryId);
                return Json(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while filling production plan.");
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<JsonResult> FillProdPlanYearCode(long ToolIssueEntryId, string ProdPlanNo)
        {
            try
            {
                var result = await _IPPCToolIssue.FillProdPlanYearCode("FillProdPlanYearCode", ToolIssueEntryId, ProdPlanNo);
                return Json(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while filling production plan year code.");
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<JsonResult> FillProdPlanSchedule(long ToolIssueEntryId, string ProdPlanNo, long ProdPlanYearCode, long PlanNoEntryId)
        {
            try
            {
                var result = await _IPPCToolIssue.FillProdPlanSchedule("FillProdPlanSchedule", ToolIssueEntryId, ProdPlanNo, ProdPlanYearCode, PlanNoEntryId);
                return Json(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while filling production plan schedule.");
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<JsonResult> FillMachineList()
        {
            try
            {
                var result = await _IPPCToolIssue.FillMachineList("FillMachineList");
                return Json(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while filling machine list.");
                return Json(new { success = false, message = ex.Message });
            }
        }

    }
}