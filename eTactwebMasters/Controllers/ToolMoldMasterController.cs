using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

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
				//MainModel = await _IAssetsMaster.GetViewByID(ID, YC).ConfigureAwait(false);
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
	}
}
