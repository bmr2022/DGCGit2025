using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

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

	}
}
