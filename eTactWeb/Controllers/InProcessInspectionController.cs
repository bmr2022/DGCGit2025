using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace eTactWeb.Controllers
{
    public class InProcessInspectionController : Controller
    {
        private readonly IDataLogic _IDataLogic;
        public IInProcessInspection _IInProcessInspection { get; }

        private readonly ILogger<InProcessInspectionController> _logger;
        private readonly IConfiguration iconfiguration;
        public IWebHostEnvironment _IWebHostEnvironment { get; }
        public InProcessInspectionController(ILogger<InProcessInspectionController> logger, IDataLogic iDataLogic, IInProcessInspection iIInProcessInspection, EncryptDecrypt encryptDecrypt, IWebHostEnvironment iWebHostEnvironment, IConfiguration iconfiguration)
        {
            _logger = logger;
            _IDataLogic = iDataLogic;
            _IInProcessInspection = iIInProcessInspection;
            _IWebHostEnvironment = iWebHostEnvironment;
            this.iconfiguration = iconfiguration;
        }
        [Route("{controller}/Index")]
        [HttpGet]
        public async Task<ActionResult> InProcessInspection()
        {
            var MainModel = new InProcessInspectionModel();
            MainModel.DTSSGrid = new List<InProcessInspectionDetailModel>();
            MainModel.FromDate = HttpContext.Session.GetString("FromDate");
            MainModel.ToDate = HttpContext.Session.GetString("ToDate");
			MainModel.YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
			//MainModel.YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
			MainModel.InspectionOptions = new List<InspectionOption>
			{
				new InspectionOption { Key = "BegingOfProduction", Text = "Beginning of Production" },
				new InspectionOption { Key = "AfterMouldCorrection", Text = "After Mould Correction" },
				new InspectionOption { Key = "AfterMachineBreackDown", Text = "After Machine Breakdown" },
				new InspectionOption { Key = "AfterMaterialLotChange", Text = "After Material Lot Change" },
				new InspectionOption { Key = "AfterMachineIdel", Text = "After Machine Idle" },
				new InspectionOption { Key = "EndOfProduction", Text = "End of Production" }
			};
			return View(MainModel); // Pass the model with old data to the view
        }
		public InProcessInspectionDetailModel ConvertToDetail(IList<string> selected)
		{
			return new InProcessInspectionDetailModel
			{
				BegingOfProduction = selected.Contains("BegingOfProduction"),
				AfterMouldCorrection = selected.Contains("AfterMouldCorrection"),
				AfterMachineBreackDown = selected.Contains("AfterMachineBreackDown"),
				AfterMaterialLotChange = selected.Contains("AfterMaterialLotChange"),
				AfterMachineIdel = selected.Contains("AfterMachineIdel"),
				EndOfProduction = selected.Contains("EndOfProduction")
			};
		}
		public async Task<JsonResult> FillEntryID(int YearCode)
		{
			var JSON = await _IInProcessInspection.FillEntryID(YearCode);
			string JsonString = JsonConvert.SerializeObject(JSON);
			return Json(JsonString);
		}
		public async Task<JsonResult> FillPartCode(string InspectionType)
		{
			var JSON = await _IInProcessInspection.FillPartCode(InspectionType);
			string JsonString = JsonConvert.SerializeObject(JSON);
			return Json(JsonString);
		}
        public async Task<JsonResult> FillItemName()
		{
			var JSON = await _IInProcessInspection.FillItemName();
			string JsonString = JsonConvert.SerializeObject(JSON);
			return Json(JsonString);
		}
		 public async Task<JsonResult> FillShift()
		{
			var JSON = await _IInProcessInspection.FillShift();
			string JsonString = JsonConvert.SerializeObject(JSON);
			return Json(JsonString);
		}

        public async Task<JsonResult> FillCustomer()
		{
			var JSON = await _IInProcessInspection.FillCustomer();
			string JsonString = JsonConvert.SerializeObject(JSON);
			return Json(JsonString);
		}
        public async Task<JsonResult> FillColor(string PartNo)
		{
			var JSON = await _IInProcessInspection.FillColor(PartNo);
			string JsonString = JsonConvert.SerializeObject(JSON);
			return Json(JsonString);
		}
         public async Task<JsonResult> FillMachineName()
		{
			var JSON = await _IInProcessInspection.FillMachineName();
			string JsonString = JsonConvert.SerializeObject(JSON);
			return Json(JsonString);
		}
		public async Task<IActionResult> GetInprocessInspectionGridData(int ItemCode)
		{
			//model.Mode = "Search";
			var model = new InProcessInspectionModel();
			model = await _IInProcessInspection.GetInprocessInspectionGridData(ItemCode);
			
			return PartialView("_InprocessInspectionGrid", model);
			

		}
	}
}
