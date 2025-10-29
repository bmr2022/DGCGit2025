using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using static eTactWeb.Data.Common.CommonFunc;
using static eTactWeb.DOM.Models.Common;
using System.Net;
using System.Data;

namespace eTactwebAdmin.Controllers
{
    public class POApprovalPolicyController : Controller
    {
		private readonly IDataLogic _IDataLogic;
		public IPOApprovalPolicy _IPOApprovalPolicy { get; }
		private readonly ILogger<POApprovalPolicyController> _logger;
		private readonly IConfiguration iconfiguration;
		public IWebHostEnvironment _IWebHostEnvironment { get; }
		public POApprovalPolicyController(ILogger<POApprovalPolicyController> logger, IDataLogic iDataLogic, IPOApprovalPolicy iPOApprovalPolicy, EncryptDecrypt encryptDecrypt, IWebHostEnvironment iWebHostEnvironment, IConfiguration iconfiguration)
		{
			_logger = logger;
			_IDataLogic = iDataLogic;
			_IPOApprovalPolicy = iPOApprovalPolicy;
			_IWebHostEnvironment = iWebHostEnvironment;
			this.iconfiguration = iconfiguration;
		}
        public async Task<JsonResult> FillItems(string SearchItemCode)
        {
            var JSON = await _IPOApprovalPolicy.FillItems(SearchItemCode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
		public async Task<JsonResult> FillGroups(string SearchGroupName)
        {
            var JSON = await _IPOApprovalPolicy.FillGroups(SearchGroupName);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
		public async Task<JsonResult> FillCateName(string SearchCatName)
        {
            var JSON = await _IPOApprovalPolicy.FillCateName(SearchCatName);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }

        //[HttpPost]
        //public JsonResult AutoComplete(string ColumnName, string prefix)
        //{
        //    IList<Common.TextValue> iList = _IDataLogic.AutoComplete("POApprovalPolicy", ColumnName, "", "", 0, 0);
        //    var Result = (from item in iList where item.Text.Contains(prefix) select new { item.Text })
        //        .Distinct()
        //        .ToList();

        //    return Json(Result);
        //}
        [Route("{controller}/Index")]
		[HttpGet]
		public async Task<ActionResult> POApprovalPolicy(int ID, string Mode, int YC, string FromDate, string ToDate)
		{
			_logger.LogInformation("\n \n ********** Page  PO Approval Policy ********** \n \n " + _IWebHostEnvironment.EnvironmentName.ToString() + "\n \n");

			TempData.Clear();
			var MainModel = new POApprovalPolicyModel();
			MainModel.FromDate = HttpContext.Session.GetString("FromDate");
			MainModel.ToDate = HttpContext.Session.GetString("ToDate");
			MainModel.YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
			MainModel.ActualEntryByEmpId = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
			MainModel.ActualEntryByName = HttpContext.Session.GetString("EmpName");
			MainModel.CC = HttpContext.Session.GetString("Branch");
			MainModel.ActualEntryDate = DateTime.Today.ToString("MM/dd/yyyy").Replace("-", "/");

            if (!string.IsNullOrEmpty(Mode) && ID > 0 && (Mode == "U" || Mode == "V"))
            {
				MainModel = await _IPOApprovalPolicy.GetViewByID(ID, YC, FromDate, ToDate).ConfigureAwait(false);
				MainModel.Mode = Mode; 
				MainModel.YearCode = YC; 
				
				if (Mode == "U")
				{
					MainModel.LastUpdatedByEmpId = Convert.ToInt32(HttpContext.Session.GetString("UID"));
					MainModel.LastUpdatedByName = HttpContext.Session.GetString("EmpName");
					MainModel.LastUpdatedDate = DateTime.Now.ToString();
					MainModel.ActualEntryDate = DateTime.Today.ToString("MM/dd/yyyy").Replace("-", "/");
					MainModel.ActualEntryByEmpId = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));

				}
			}
			return View(MainModel); 
		}
		[Route("{controller}/Index")]
		[HttpPost]
		public async Task<IActionResult> POApprovalPolicy(POApprovalPolicyModel model)
		{
			try
			{
				var Result = await _IPOApprovalPolicy.SavePOApprovalPolicy(model);
				if (Result != null)
				{
					if (Result.StatusText == "Success" && Result.StatusCode == HttpStatusCode.OK)
					{
						ViewBag.isSuccess = true;
						TempData["200"] = "200";
					}
					else if (Result.StatusText == "Success" && Result.StatusCode == HttpStatusCode.Accepted)
					{
						ViewBag.isSuccess = true;
						TempData["202"] = "202";
					}
					else if (Result.StatusText == "Error" && Result.StatusCode == HttpStatusCode.InternalServerError)
					{
						ViewBag.isSuccess = false;
						TempData["500"] = "500";
						_logger.LogError($"\n \n ********** LogError ********** \n {JsonConvert.SerializeObject(Result)}\n \n");
						return View("Error", Result);
					}
				}

				return RedirectToAction(nameof(POApprovalPolicy));

			}
			catch (Exception ex)
			{
				LogException<POApprovalPolicyController>.WriteException(_logger, ex);
				var ResponseResult = new ResponseResult
				{
					StatusCode = HttpStatusCode.InternalServerError,
					StatusText = "Error",
					Result = ex
				};
				return View("Error", ResponseResult);
			}
		}
		public async Task<JsonResult> FillItemName()
		{
			var JSON = await _IPOApprovalPolicy.FillItemName();
			string JsonString = JsonConvert.SerializeObject(JSON);
			return Json(JsonString);
		}
		public async Task<JsonResult> FillPartCode()
		{
			var JSON = await _IPOApprovalPolicy.FillPartCode();
			string JsonString = JsonConvert.SerializeObject(JSON);
			return Json(JsonString);
		}
		
		public async Task<JsonResult> FillEmpName()
		{
			var JSON = await _IPOApprovalPolicy.FillEmpName();
			string JsonString = JsonConvert.SerializeObject(JSON);
			return Json(JsonString);
		}
		public async Task<JsonResult> FillCatName()
		{
			var JSON = await _IPOApprovalPolicy.FillCatName();
			string JsonString = JsonConvert.SerializeObject(JSON);
			return Json(JsonString);
		}
		public async Task<JsonResult> FillGroupName()
		{
			var JSON = await _IPOApprovalPolicy.FillGroupName();
			string JsonString = JsonConvert.SerializeObject(JSON);
			return Json(JsonString);
		}
		public async Task<JsonResult> FillEntryID()
		{
			var JSON = await _IPOApprovalPolicy.FillEntryID();
			string JsonString = JsonConvert.SerializeObject(JSON);
			return Json(JsonString);
		}
        public async Task<IActionResult> POApprovalPolicyDashBoard(string ReportType, string FromDate, string ToDate)
        {
            var model = new POApprovalPolicyModel();
            var yearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
            DateTime now = DateTime.Now;
            DateTime firstDayOfMonth = new DateTime(yearCode, now.Month, 1);
            Dictionary<int, string> monthNames = new Dictionary<int, string>
            {
                {1, "Jan"}, {2, "Feb"}, {3, "Mar"}, {4, "Apr"}, {5, "May"}, {6, "Jun"},
                {7, "Jul"}, {8, "Aug"}, {9, "Sep"}, {10, "Oct"}, {11, "Nov"}, {12, "Dec"}
            };

            model.FromDate = $"{firstDayOfMonth.Day}/{monthNames[firstDayOfMonth.Month]}/{firstDayOfMonth.Year}";
            model.ToDate = $"{now.Day}/{monthNames[now.Month]}/{now.Year}";
            
            model.ActualEntryByEmpId = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
          
            var Result = await _IPOApprovalPolicy.GetDashboardData(model);

            return View(model);
        }
        public async Task<IActionResult> GetDetailData(string FromDate, string ToDate, string ReportType, string GroupName, string CateName, string ItemName)
        {
            var model = new POApprovalPolicyModel();
            model = await _IPOApprovalPolicy.GetDashboardDetailData(FromDate, ToDate, ReportType,  GroupName,  CateName,  ItemName);
            return PartialView("_POApprovalPolicyDashBoardGrid", model);

        }

        public async Task<IActionResult> DeleteByID(int EntryId, string EntryDate, int EntryByempId)
        {
            var Result = await _IPOApprovalPolicy.DeleteByID(EntryId, EntryDate, EntryByempId);

            if (Result.StatusText == "Success" || Result.StatusCode == HttpStatusCode.Gone)
            {
                ViewBag.isSuccess = true;
                TempData["410"] = "410";
            }
            else if (Result.StatusText == "Error" || Result.StatusCode == HttpStatusCode.Accepted)
            {
                ViewBag.isSuccess = true;
                TempData["423"] = "423";
            }
            else
            {
                ViewBag.isSuccess = false;
                TempData["500"] = "500";
            }

            return RedirectToAction("POApprovalPolicyDashBoard");

        }
    }
}
