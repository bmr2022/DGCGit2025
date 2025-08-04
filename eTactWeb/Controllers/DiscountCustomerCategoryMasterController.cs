using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using static eTactWeb.Data.Common.CommonFunc;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Controllers
{
    public class DiscountCustomerCategoryMasterController : Controller
    {
        private readonly IDataLogic _IDataLogic;
        public IDiscountCustomerCategoryMaster _IDiscountCustomerCategoryMaster { get; }
        private readonly ILogger<DiscountCustomerCategoryMasterController> _logger;
        private readonly IConfiguration iconfiguration;

        public IWebHostEnvironment _IWebHostEnvironment { get; }
        public DiscountCustomerCategoryMasterController(ILogger<DiscountCustomerCategoryMasterController> logger, IDataLogic iDataLogic, IDiscountCustomerCategoryMaster iDiscountCustomerCategoryMaster, EncryptDecrypt encryptDecrypt, IWebHostEnvironment iWebHostEnvironment, IConfiguration iconfiguration)
        {
            _logger = logger;
            _IDataLogic = iDataLogic;
			_IDiscountCustomerCategoryMaster = iDiscountCustomerCategoryMaster;
            _IWebHostEnvironment = iWebHostEnvironment;
            this.iconfiguration = iconfiguration;
        }
		[Route("{controller}/Index")]
		public async Task<ActionResult> DiscountCustomerCategoryMaster(int ID, int YC, string Mode, string SlipNo)
		{
			var MainModel = new DiscountCustomerCategoryMasterModel();

			MainModel.DiscountCustCatYearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
			
			MainModel.ActualEntryByEmpId = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
			MainModel.ApprovedByEmpId = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
			MainModel.ApprovedByEmpName = HttpContext.Session.GetString("EmpName");
			MainModel.ActualEntryByEmpName = HttpContext.Session.GetString("EmpName");
			MainModel.CC = HttpContext.Session.GetString("Branch");
			HttpContext.Session.Remove("KeyMaterialConversionGrid");
			if (!string.IsNullOrEmpty(Mode) && ID > 0 && Mode == "U")
			{
				//MainModel = await _IMaterialConversion.GetViewByID(ID, YC, FromDate, ToDate).ConfigureAwait(false);
				//MainModel.Mode = Mode; // Set Mode to Update
				//MainModel.EntryId = ID;
				//MainModel.OpeningYearCode = YC;
				//MainModel.StoreId = StoreId;
				

				if (Mode == "U")
				{
					MainModel.LastUpdatedbyEmpId = Convert.ToInt32(HttpContext.Session.GetString("UID"));
					MainModel.LastUpdatedbyEmpName = HttpContext.Session.GetString("EmpName");
					MainModel.LastupDationDate = DateTime.Today.ToString("MM/dd/yyyy").Replace("-", "/");
					MainModel.ActualEntryDate = DateTime.Today.ToString("MM/dd/yyyy").Replace("-", "/");
					MainModel.ActualEntryByEmpId = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
					MainModel.ActualEntryDate = DateTime.Today.ToString("MM/dd/yyyy").Replace("-", "/");
					MainModel.CC = HttpContext.Session.GetString("Branch");
				}

				string serializedGrid = JsonConvert.SerializeObject(MainModel.DiscountCustomerCategoryMasterGrid);
				HttpContext.Session.SetString("KeyDiscountCustomerCategoryMasterGrid", serializedGrid);
			}

			return View(MainModel);
		}
		[HttpPost]
		[Route("{controller}/Index")]
		public async Task<IActionResult> DiscountCustomerCategoryMaster(DiscountCustomerCategoryMasterModel model)
		{
			try
			{
				var Result = await _IDiscountCustomerCategoryMaster.SaveDiscountCustomerCategoryMaster(model);
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

				return RedirectToAction(nameof(DiscountCustomerCategoryMaster));

			}
			catch (Exception ex)
			{
				// Log and return the error
				LogException<DiscountCustomerCategoryMasterController>.WriteException(_logger, ex);
				var ResponseResult = new ResponseResult
				{
					StatusCode = HttpStatusCode.InternalServerError,
					StatusText = "Error",
					Result = ex
				};
				return View("Error", ResponseResult);
			}
		}
		public async Task<JsonResult> GetFormRights()
		{
			var userID = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
			var JSON = await _IDiscountCustomerCategoryMaster.GetFormRights(userID);
			string JsonString = JsonConvert.SerializeObject(JSON);
			return Json(JsonString);
		}
		public async Task<JsonResult> FillEntryID(int YearCode)
		{
			var JSON = await _IDiscountCustomerCategoryMaster.FillEntryID(YearCode);
			string JsonString = JsonConvert.SerializeObject(JSON);
			return Json(JsonString);
		}
		
		public async Task<JsonResult> FillDiscountCategory()
		{
			var JSON = await _IDiscountCustomerCategoryMaster.FillDiscountCategory();
			string JsonString = JsonConvert.SerializeObject(JSON);
			return Json(JsonString);
		}
        public async Task<IActionResult> DiscountCustomerCategoryMasterDashBoard( string FromDate, string ToDate)
        {
            var model = new DiscountCustomerCategoryMasterModel();
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
         
            var Result = await _IDiscountCustomerCategoryMaster.GetDashboardData(model);

         

            return View(model);
        }
        public async Task<IActionResult> GetDetailData(string FromDate, string ToDate, string ReportType)
        {
            //model.Mode = "Search";
            var model = new DiscountCustomerCategoryMasterModel();
            model = await _IDiscountCustomerCategoryMaster.GetDashboardDetailData(FromDate, ToDate);
            
             return PartialView("_DiscountCustomerCategoryMasterDashBoardGrid", model);
        }
    }
}
