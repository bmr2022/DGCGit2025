using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace eTactwebAccounts.Controllers
{
	public class AccDepriciationCalculationdetailController : Controller
	{
		private readonly IDataLogic _IDataLogic;
		public IAccDepriciationCalculationdetail _IAccDepriciationCalculationdetail { get; }
		private readonly ILogger<AccDepriciationCalculationdetailController> _logger;
		private readonly IConfiguration iconfiguration;

		public IWebHostEnvironment _IWebHostEnvironment { get; }
		public AccDepriciationCalculationdetailController(ILogger<AccDepriciationCalculationdetailController> logger, IDataLogic iDataLogic, IAccDepriciationCalculationdetail iAccDepriciationCalculationdetail, IWebHostEnvironment iWebHostEnvironment, IConfiguration iconfiguration)
		{
			_logger = logger;
			_IDataLogic = iDataLogic;
			_IAccDepriciationCalculationdetail = iAccDepriciationCalculationdetail;
			_IWebHostEnvironment = iWebHostEnvironment;
			this.iconfiguration = iconfiguration;
		}
		[Route("{controller}/Index")]
		public async Task<ActionResult> AccDepriciationCalculationdetail(int ID, int YC, string Mode
)
		{
			var MainModel = new AccDepriciationCalculationdetailModel();

			MainModel.DepriciationYearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));

			MainModel.ActualEntryBy = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
			
			MainModel.ActualEntryByEmpName = HttpContext.Session.GetString("EmpName");
			MainModel.CC = HttpContext.Session.GetString("Branch");
			HttpContext.Session.Remove("KeyAccDepriciationCalculationdetailGrid");
			if (!string.IsNullOrEmpty(Mode) && ID > 0 && Mode == "U")
			{
				//MainModel = await _IDiscountCustomerCategoryMaster.GetViewByID(ID, YC).ConfigureAwait(false);
				MainModel.Mode = Mode; // Set Mode to Update
				MainModel.DepriciationEntryId = ID;
				MainModel.DepriciationYearCode = YC;
				

				if (Mode == "U")
				{
					MainModel.LastupdatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
					MainModel.LastUpdatedByEmpName = HttpContext.Session.GetString("EmpName");
					MainModel.LastUpdatedDate = DateTime.Today.ToString("MM/dd/yyyy").Replace("-", "/");
					MainModel.ActualEntryDate = DateTime.Today.ToString("MM/dd/yyyy").Replace("-", "/");
					MainModel.ActualEntryBy = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
					MainModel.ActualEntryDate = DateTime.Today.ToString("MM/dd/yyyy").Replace("-", "/");
					MainModel.CC = HttpContext.Session.GetString("Branch");
				}

				string serializedGrid = JsonConvert.SerializeObject(MainModel.AccDepriciationCalculationdetailGrid);
				HttpContext.Session.SetString("KeyAccDepriciationCalculationdetailGrid", serializedGrid);
			}

			return View(MainModel);
		}
		public async Task<IActionResult> GetAssets()
		{
			var model = new AccDepriciationCalculationdetailModel();
			model = await _IAccDepriciationCalculationdetail.GetAssets();

			return PartialView("_AccDepriciationCalculationdetailGrid", model);
		}
	}
}
