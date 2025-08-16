using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using static eTactWeb.Data.Common.CommonFunc;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Controllers
{
    public class MachineGroupMasterController : Controller
    {
		private readonly IDataLogic _IDataLogic;
		public IMachineGroupMaster _IMachineGroupMaster { get; }
		private readonly ILogger<MachineGroupMasterController> _logger;
		private readonly IConfiguration iconfiguration;

		public IWebHostEnvironment _IWebHostEnvironment { get; }
		public MachineGroupMasterController(ILogger<MachineGroupMasterController> logger, IDataLogic iDataLogic, IMachineGroupMaster iMachineGroupMaster, EncryptDecrypt encryptDecrypt, IWebHostEnvironment iWebHostEnvironment, IConfiguration iconfiguration)
		{
			_logger = logger;
			_IDataLogic = iDataLogic;
			_IMachineGroupMaster = iMachineGroupMaster;
			_IWebHostEnvironment = iWebHostEnvironment;
			this.iconfiguration = iconfiguration;
		}
        public async Task<JsonResult> GetFormRights()
        {
            var userID = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
            var JSON = await _IMachineGroupMaster.GetFormRights(userID);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        [Route("{controller}/Index")]
		public async Task<ActionResult> MachineGroupMaster(int ID, int YC, string Mode,string MachineGroup,string CC,int UId)
		{
			var MainModel = new MachineGroupMasterModel();

			MainModel.YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));

			MainModel.ActualEntryBy = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
			MainModel.ApprovedByEmpId = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
			MainModel.ApprovedByEmpName = HttpContext.Session.GetString("EmpName");
			MainModel.ActualEntryByName = HttpContext.Session.GetString("EmpName");
			MainModel.CC = HttpContext.Session.GetString("Branch");
			MainModel.LastUpdatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
			HttpContext.Session.Remove("KeyMaterialConversionGrid");
			if (!string.IsNullOrEmpty(Mode) && ID > 0 && (Mode == "U" || Mode == "V"))
			{
				MainModel = await _IMachineGroupMaster.GetViewByID(ID).ConfigureAwait(false);
				MainModel.Mode = Mode; 
				MainModel.EntryId = ID;
				MainModel.MachineGroup = MachineGroup;
				MainModel.CC = CC;
				MainModel.UId = UId;

				if (Mode == "U")
				{
					MainModel.LastUpdatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
					MainModel.LastUpdatedByName = HttpContext.Session.GetString("EmpName");
					MainModel.LastUpdationDate = DateTime.Today.ToString("MM/dd/yyyy").Replace("-", "/");
					MainModel.ActualEntryDate = DateTime.Today.ToString("MM/dd/yyyy").Replace("-", "/");
					MainModel.ActualEntryBy = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
					MainModel.ActualEntryDate = DateTime.Today.ToString("MM/dd/yyyy").Replace("-", "/");
					MainModel.CC = HttpContext.Session.GetString("Branch");
                    MainModel.ActualEntryBy = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
                    MainModel.ApprovedByEmpId = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
                    MainModel.ApprovedByEmpName = HttpContext.Session.GetString("EmpName");
                    MainModel.ActualEntryByName = HttpContext.Session.GetString("EmpName");
					MainModel.YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
				}

				//string serializedGrid = JsonConvert.SerializeObject(MainModel.DiscountCustomerCategoryMasterGrid);
				//HttpContext.Session.SetString("KeyDiscountCustomerCategoryMasterGrid", serializedGrid);
			}

			return View(MainModel);
		}
		[HttpPost]
		[Route("{controller}/Index")]
		public async Task<IActionResult> MachineGroupMaster(MachineGroupMasterModel model)
		{
			try
			{
				var Result = await _IMachineGroupMaster.SaveMachineGroupMaster(model);
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

				return RedirectToAction(nameof(MachineGroupMasterDashBoard));

			}
			catch (Exception ex)
			{
				// Log and return the error
				LogException<MachineGroupMasterController>.WriteException(_logger, ex);
				var ResponseResult = new ResponseResult
				{
					StatusCode = HttpStatusCode.InternalServerError,
					StatusText = "Error",
					Result = ex
				};
				return View("Error", ResponseResult);
			}
		}
		public async Task<JsonResult> FillMachineGroup()
		{
			var JSON = await _IMachineGroupMaster.FillMachineGroup();
			string JsonString = JsonConvert.SerializeObject(JSON);
			return Json(JsonString);
		}
        public async Task<IActionResult> MachineGroupMasterDashBoard(string FromDate, string ToDate)
        {
            var model = new MachineGroupMasterModel();
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


            var Result = await _IMachineGroupMaster.GetDashboardData(model);

            if (Result.Result != null)
            {
                var _List = new List<TextValue>();
                DataSet DS = Result.Result;
                if (DS != null && DS.Tables.Count > 0)
                {
                    var dt = DS.Tables[0];
                    model.MachineGroupGrid = CommonFunc.DataTableToList<MachineGroupMasterModel>(dt, "MachineGroupMasterDashBoard");
                }

            }

            return View(model);
        }
        public async Task<IActionResult> GetDetailData(string FromDate, string ToDate, string ReportType)
        {
            //model.Mode = "Search";
            var model = new MachineGroupMasterModel();
            model = await _IMachineGroupMaster.GetDashboardDetailData(FromDate, ToDate);

            return PartialView("_MachineGroupMasterDashBoardGrid", model);
        }
        public async Task<IActionResult> DeleteByID(int EntryId)
        {
            var Result = await _IMachineGroupMaster.DeleteByID(EntryId);

            if (Result.StatusText == "Success" || Result.StatusCode == HttpStatusCode.Gone)
            {
                ViewBag.isSuccess = true;
                TempData["410"] = "410";
            }
            else if (Result.StatusText == "Error" || Result.StatusCode == HttpStatusCode.Accepted)
            {
                ViewBag.isSuccess = true;
				TempData["Message"] = Result.Result;
			}
            else
            {
                ViewBag.isSuccess = false;
                TempData["500"] = "500";
            }

            return RedirectToAction("MachineGroupMasterDashBoard");

        }
        public async Task<JsonResult> CheckMachineGroup(string machGroup)
        {
            var result = await _IMachineGroupMaster.CheckMachineGroupExists(machGroup);
            return Json(new
            {
                exists = result.Exists,
                entryId = result.EntryId,
                machGroup = result.MachGroup,
                uId = result.UId,
                cc = result.CC
            });
        }
    }
}
