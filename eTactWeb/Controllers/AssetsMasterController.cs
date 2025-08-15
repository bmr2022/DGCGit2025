using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using static eTactWeb.Data.Common.CommonFunc;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Controllers
{
    public class AssetsMasterController : Controller
    {
        private readonly IDataLogic _IDataLogic;
        public IAssetsMaster _IAssetsMaster { get; }
        private readonly ILogger<AssetsMasterController> _logger;
        private readonly IConfiguration iconfiguration;

        public IWebHostEnvironment _IWebHostEnvironment { get; }
        public AssetsMasterController(ILogger<AssetsMasterController> logger, IDataLogic iDataLogic, IAssetsMaster iAssetsMaster, EncryptDecrypt encryptDecrypt, IWebHostEnvironment iWebHostEnvironment, IConfiguration iconfiguration)
        {
            _logger = logger;
            _IDataLogic = iDataLogic;
            _IAssetsMaster = iAssetsMaster;
            _IWebHostEnvironment = iWebHostEnvironment;
            this.iconfiguration = iconfiguration;
        }
        [Route("{controller}/Index")]
        public async Task<ActionResult> AssetsMaster(int ID, int YC, string Mode)
        {
            var MainModel = new AssetsMasterModel();

            MainModel.YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));

            MainModel.ActualEntryByEmpId = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
            MainModel.ApprovedByEmpId = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
            MainModel.ApprovedByEmpName = HttpContext.Session.GetString("EmpName");
            MainModel.ActualEntryByEmpName = HttpContext.Session.GetString("EmpName");
            MainModel.CC = HttpContext.Session.GetString("Branch");
            HttpContext.Session.Remove("KeyAssetsMasterGrid");
            if (!string.IsNullOrEmpty(Mode) && ID > 0 && Mode == "U")
            {
                //MainModel = await _IDiscountCustomerCategoryMaster.GetViewByID(ID, YC).ConfigureAwait(false);
                //MainModel.Mode = Mode; // Set Mode to Update
                //MainModel.DiscountCustCatEntryId = ID;
                //MainModel.DiscountCustCatYearCode = YC;
               
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

                string serializedGrid = JsonConvert.SerializeObject(MainModel.AssetsMasterGrid);
                HttpContext.Session.SetString("KeyAssetsMasterGrid", serializedGrid);
            }

            return View(MainModel);
        }
		[HttpPost]
		[Route("{controller}/Index")]
		public async Task<IActionResult> AssetsMaster(AssetsMasterModel model)
		{
			try
			{
				var Result = await _IAssetsMaster.SaveAssetsMaster(model);
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

				return RedirectToAction(nameof(AssetsMaster));

			}
			catch (Exception ex)
			{
				LogException<AssetsMasterController>.WriteException(_logger, ex);
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
			var JSON = await _IAssetsMaster.FillItemName();
			string JsonString = JsonConvert.SerializeObject(JSON);
			return Json(JsonString);
		}
        public async Task<JsonResult> FillCostCenterName()
		{
			var JSON = await _IAssetsMaster.FillCostCenterName();
			string JsonString = JsonConvert.SerializeObject(JSON);
			return Json(JsonString);
		}
        public async Task<JsonResult> FillDepartmentName()
		{
			var JSON = await _IAssetsMaster.FillDepartmentName();
			string JsonString = JsonConvert.SerializeObject(JSON);
			return Json(JsonString);
		}
        public async Task<JsonResult> FillParentAccountName()
		{
			var JSON = await _IAssetsMaster.FillParentAccountName();
			string JsonString = JsonConvert.SerializeObject(JSON);
			return Json(JsonString);
		}
         public async Task<JsonResult> FillParentGoupDetail(int ParentAccountCode)
		{
			var JSON = await _IAssetsMaster.FillParentGoupDetail(ParentAccountCode);
			string JsonString = JsonConvert.SerializeObject(JSON);
			return Json(JsonString);
		}

	}
}
