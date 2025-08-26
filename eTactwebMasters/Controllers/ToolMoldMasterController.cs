using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using static eTactWeb.Data.Common.CommonFunc;
using static eTactWeb.DOM.Models.Common;
using System.Net;
using System.Data;

namespace eTactwebMasters.Controllers
{
	public class ToolMoldMasterController : Controller
	{
		private readonly IDataLogic _IDataLogic;
		public IToolMoldMaster _IToolMoldMaster { get; }
		private readonly ILogger<ToolMoldMasterController> _logger;
		private readonly IConfiguration iconfiguration;

		public IWebHostEnvironment _IWebHostEnvironment { get; }
		public ToolMoldMasterController(ILogger<ToolMoldMasterController> logger, IDataLogic iDataLogic, IToolMoldMaster iToolMoldMaster, EncryptDecrypt encryptDecrypt, IWebHostEnvironment iWebHostEnvironment, IConfiguration iconfiguration)
		{
			_logger = logger;
			_IDataLogic = iDataLogic;
			_IToolMoldMaster = iToolMoldMaster;
			_IWebHostEnvironment = iWebHostEnvironment;
			this.iconfiguration = iconfiguration;
		}
		[Route("{controller}/Index")]
		public async Task<ActionResult> ToolMoldMaster(int ID, int YC, string Mode)
		{
			var MainModel = new ToolMoldMasterModel();

			MainModel.YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
			MainModel.ActualEntryBy = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
			MainModel.ActualEntryByEmpName = HttpContext.Session.GetString("EmpName");
			MainModel.CC = HttpContext.Session.GetString("Branch");
			MainModel.LastupdatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
			HttpContext.Session.Remove("KeyToolMoldGrid");
			if (!string.IsNullOrEmpty(Mode) && ID > 0 && (Mode == "U" || Mode == "V"))
			{
				MainModel = await _IToolMoldMaster.GetViewByID(ID, YC).ConfigureAwait(false);
				MainModel.Mode = Mode; // Set Mode to Update
				
				if (Mode == "U")
				{
					MainModel.LastupdatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
					MainModel.LastUpdatedbyEmpName = HttpContext.Session.GetString("EmpName");
					MainModel.LastUpdatedDate = DateTime.Today.ToString("MM/dd/yyyy").Replace("-", "/");
					MainModel.ActualEntryDate = DateTime.Today.ToString("MM/dd/yyyy").Replace("-", "/");
					MainModel.ActualEntryBy = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
					MainModel.ActualEntryDate = DateTime.Today.ToString("MM/dd/yyyy").Replace("-", "/");
					MainModel.CC = HttpContext.Session.GetString("Branch");
				}

				string serializedGrid = JsonConvert.SerializeObject(MainModel.ToolMoldGrid);
				HttpContext.Session.SetString("KeyToolMoldGrid", serializedGrid);
			}

			return View(MainModel);
		}
		[HttpPost]
		[Route("{controller}/Index")]
		public async Task<IActionResult> ToolMoldMaster(ToolMoldMasterModel model)
		{
			try
			{
				model.ActualEntryBy = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
				var Result = await _IToolMoldMaster.SaveToolMoldMaster(model);
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

				return RedirectToAction(nameof(ToolMoldMaster));

			}
			catch (Exception ex)
			{
				LogException<ToolMoldMasterController>.WriteException(_logger, ex);
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
			var JSON = await _IToolMoldMaster.FillItemName();
			string JsonString = JsonConvert.SerializeObject(JSON);
			return Json(JsonString);
		}
		public async Task<JsonResult> FillEntryId(int YearCode, string EntryDate)
		{
			var JSON = await _IToolMoldMaster.FillEntryId(YearCode, EntryDate);
			string JsonString = JsonConvert.SerializeObject(JSON);
			return Json(JsonString);
		}
		public async Task<JsonResult> FillCostCenterName()
		{
			var JSON = await _IToolMoldMaster.FillCostCenterName();
			string JsonString = JsonConvert.SerializeObject(JSON);
			return Json(JsonString);
		}
		public async Task<JsonResult> FillCategoryName()
		{
			var JSON = await _IToolMoldMaster.FillCategoryName();
			string JsonString = JsonConvert.SerializeObject(JSON);
			return Json(JsonString);
		}
		public async Task<JsonResult> FillCustoidianEmpName()
		{
			var JSON = await _IToolMoldMaster.FillCustoidianEmpName();
			string JsonString = JsonConvert.SerializeObject(JSON);
			return Json(JsonString);
		}

		public async Task<JsonResult> FillDepartmentName()
		{
			var JSON = await _IToolMoldMaster.FillDepartmentName();
			string JsonString = JsonConvert.SerializeObject(JSON);
			return Json(JsonString);
		}
		public async Task<JsonResult> FillParentAccountName()
		{
			var JSON = await _IToolMoldMaster.FillParentAccountName();
			string JsonString = JsonConvert.SerializeObject(JSON);
			return Json(JsonString);
		}
		public async Task<JsonResult> FillParentGoupDetail(int ParentAccountCode)
		{
			var JSON = await _IToolMoldMaster.FillParentGoupDetail(ParentAccountCode);
			string JsonString = JsonConvert.SerializeObject(JSON);
			return Json(JsonString);
		}
        public async Task<IActionResult> ToolMoldMasterDashBoard(string FromDate, string ToDate)
        {
            var model = new ToolMoldMasterModel();
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

            model.ActualEntryBy = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));

            var Result = await _IToolMoldMaster.GetDashboardData(model);

            if (Result.Result != null)
            {
                var _List = new List<TextValue>();
                DataSet DS = Result.Result;
                if (DS != null && DS.Tables.Count > 0)
                {
                    var dt = DS.Tables[0];
                    model.ToolMoldGrid = CommonFunc.DataTableToList<ToolMoldMasterModel>(dt, "ToolMoldMasterDashBoard");
                }

            }

            return View(model);
        }
        public async Task<IActionResult> GetDetailData(string FromDate, string ToDate, string ReportType, string ToolName)
        {
            var model = new ToolMoldMasterModel();
            model = await _IToolMoldMaster.GetDashboardDetailData(FromDate, ToDate, ToolName);

            return PartialView("_ToolMoldMasterDashBoardGrid", model);
        }
        public async Task<IActionResult> DeleteByID(int EntryId, int YearCode, string EntryDate, string MachineName)
        {
            DateTime parsedDate;

            if (DateTime.TryParse(EntryDate, out parsedDate))
            {
                EntryDate = parsedDate.ToString("dd/MMM/yyyy");
            }
            var Result = await _IToolMoldMaster.DeleteByID(EntryId, YearCode, EntryDate, MachineName);

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

            return RedirectToAction("ToolMoldMasterDashBoard");

        }
    }
}
