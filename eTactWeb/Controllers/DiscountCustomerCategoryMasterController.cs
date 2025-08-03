using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

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
		public async Task<JsonResult> FillEntryID(int YearCode)
		{
			var JSON = await _IDiscountCustomerCategoryMaster.FillEntryID(YearCode);
			string JsonString = JsonConvert.SerializeObject(JSON);
			return Json(JsonString);
		}
	}
}
