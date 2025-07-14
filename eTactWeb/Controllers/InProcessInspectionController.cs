using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
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
		public async Task<IActionResult> GetInprocessInspectionGridData(int ItemCode, int SampleSize)
		{
            //model.Mode = "Search";
            ViewBag.SampleSize = SampleSize;
            var model = new InProcessInspectionModel();
			model = await _IInProcessInspection.GetInprocessInspectionGridData(ItemCode,SampleSize);
			
			return PartialView("_InprocessInspectionGrid", model);
			

		}
		//public IActionResult AddToGridData(InProcessInspectionDetailModel model)
		//{
		//	try
		//	{
		//		string modelJson = HttpContext.Session.GetString("KeyInProcessInspectionGrid");
		//		List<InProcessInspectionDetailModel> MaterialConversionGrid = new List<InProcessInspectionDetailModel>();
		//		if (!string.IsNullOrEmpty(modelJson))
		//		{
		//			MaterialConversionGrid = JsonConvert.DeserializeObject<List<InProcessInspectionDetailModel>>(modelJson);
		//		}

		//		var MainModel = new InProcessInspectionModel();
		//		var WorkOrderPGrid = new List<InProcessInspectionModel>();
		//		var OrderGrid = new List<InProcessInspectionDetailModel>();
		//		var ssGrid = new List<InProcessInspectionDetailModel>();

		//		if (model != null)
		//		{
		//			if (MaterialConversionGrid == null)
		//			{
		//				model.SeqNo = 1;
		//				OrderGrid.Add(model);
		//			}
		//			else
		//			{
		//				if (MaterialConversionGrid.Any(x => (x.PartCode == model.PartCode)))
		//				{
		//					return StatusCode(207, "Duplicate");
		//				}
		//				else
		//				{
		//					//count = WorkOrderProcessGrid.Count();
		//					model.SeqNo = MaterialConversionGrid.Count + 1;
		//					OrderGrid = MaterialConversionGrid.Where(x => x != null).ToList();
		//					ssGrid.AddRange(OrderGrid);
		//					OrderGrid.Add(model);

		//				}

		//			}

		//			MainModel.DTSSGrid = OrderGrid;

		//			MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
		//			{
		//				AbsoluteExpiration = DateTime.Now.AddMinutes(60),
		//				SlidingExpiration = TimeSpan.FromMinutes(55),
		//				Size = 1024,
		//			};

		//			string serializedGrid = JsonConvert.SerializeObject(MainModel.DTSSGrid);
		//			HttpContext.Session.SetString("KeyInProcessInspectionGrid", serializedGrid);
		//		}
		//		else
		//		{
		//			ModelState.TryAddModelError("Error", " List Cannot Be Empty...!");
		//		}
		//		return PartialView("_InProcessInspectionAddtoGrid", MainModel);

		//	}
		//	catch (Exception ex)
		//	{
		//		throw ex;
		//	}
		//}
		[HttpPost]
		public IActionResult AddToGridData(List<InProcessInspectionDetailModel> modelList)
		{
			try
			{
				if (modelList == null || !modelList.Any())
				{
					ModelState.TryAddModelError("Error", "List cannot be empty.");
					return BadRequest("No data received.");
				}

				string modelJson = HttpContext.Session.GetString("KeyInProcessInspectionGrid");
				List<InProcessInspectionDetailModel> existingGrid = new List<InProcessInspectionDetailModel>();

				if (!string.IsNullOrEmpty(modelJson))
				{
					existingGrid = JsonConvert.DeserializeObject<List<InProcessInspectionDetailModel>>(modelJson);
				}

				foreach (var model in modelList)
				{
					if (existingGrid.Any(x => x.Characteristic == model.Characteristic))
					{
						// skip duplicates
						continue;
					}

					model.SeqNo = existingGrid.Count + 1;
					existingGrid.Add(model);
				}

				HttpContext.Session.SetString("KeyInProcessInspectionGrid", JsonConvert.SerializeObject(existingGrid));

				var MainModel = new InProcessInspectionModel
				{
					DTSSGrid = existingGrid
				};

				return PartialView("_InProcessInspectionAddtoGrid", MainModel);
			}
			catch (Exception ex)
			{
				return StatusCode(500, "Internal server error: " + ex.Message);
			}
		}

	}
}
