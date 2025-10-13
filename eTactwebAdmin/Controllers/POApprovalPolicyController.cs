using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using static eTactWeb.Data.Common.CommonFunc;
using static eTactWeb.DOM.Models.Common;
using System.Net;

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
		[Route("{controller}/Index")]
		[HttpGet]
		public async Task<ActionResult> POApprovalPolicy(int ID, string Mode)
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
			
			if (!string.IsNullOrEmpty(Mode) && ID > 0 && Mode == "U")
			{

				
				//MainModel = await _IAlternateItemMaster.GetViewByID(MainItemCode, AlternateItemCode).ConfigureAwait(false);
				MainModel.Mode = Mode; // Set Mode to Update
				
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

	}
}
