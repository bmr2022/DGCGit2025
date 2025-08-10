using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

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
		[Route("{controller}/Index")]
		public async Task<ActionResult> MachineGroupMaster(int ID, int YC, string Mode)
		{
			var MainModel = new MachineGroupMasterModel();

			MainModel.YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));

			MainModel.ActualEntryBy = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
			MainModel.ApprovedByEmpId = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
			MainModel.ApprovedByEmpName = HttpContext.Session.GetString("EmpName");
			MainModel.ActualEntryByName = HttpContext.Session.GetString("EmpName");
			MainModel.CC = HttpContext.Session.GetString("Branch");
			HttpContext.Session.Remove("KeyMaterialConversionGrid");
			if (!string.IsNullOrEmpty(Mode) && ID > 0 && Mode == "U")
			{
				//MainModel = await _IDiscountCustomerCategoryMaster.GetViewByID(ID, YC).ConfigureAwait(false);
				//MainModel.Mode = Mode; // Set Mode to Update
				//MainModel.DiscountCustCatEntryId = ID;
				//MainModel.DiscountCustCatYearCode = YC;
				//MainModel.DiscountCategoryId = DiscountCategoryId;
				
				if (Mode == "U")
				{
					MainModel.LastUpdatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
					MainModel.LastUpdatedByName = HttpContext.Session.GetString("EmpName");
					MainModel.LastUpdationDate = DateTime.Today.ToString("MM/dd/yyyy").Replace("-", "/");
					MainModel.ActualEntryDate = DateTime.Today.ToString("MM/dd/yyyy").Replace("-", "/");
					MainModel.ActualEntryBy = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
					MainModel.ActualEntryDate = DateTime.Today.ToString("MM/dd/yyyy").Replace("-", "/");
					MainModel.CC = HttpContext.Session.GetString("Branch");
				}

				//string serializedGrid = JsonConvert.SerializeObject(MainModel.DiscountCustomerCategoryMasterGrid);
				//HttpContext.Session.SetString("KeyDiscountCustomerCategoryMasterGrid", serializedGrid);
			}

			return View(MainModel);
		}
	}
}
