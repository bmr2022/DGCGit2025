using ClosedXML.Excel;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Wordprocessing;
using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using PdfSharp;
using PdfSharp.Drawing.BarCodes;
using System.Runtime.Caching;
using static eTactWeb.Data.Common.CommonFunc;
using static eTactWeb.DOM.Models.Common;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;

namespace eTactWeb.Controllers
{
    public class InProcessInspectionController : Controller
    {
        private readonly IDataLogic _IDataLogic;
        public IInProcessInspection _IInProcessInspection { get; }

        private readonly ILogger<InProcessInspectionController> _logger;
        private readonly IConfiguration iconfiguration;
        public IWebHostEnvironment _IWebHostEnvironment { get; }
        private readonly IMemoryCache _MemoryCache;
        public InProcessInspectionController(ILogger<InProcessInspectionController> logger, IDataLogic iDataLogic, IInProcessInspection iIInProcessInspection, EncryptDecrypt encryptDecrypt, IWebHostEnvironment iWebHostEnvironment, IConfiguration iconfiguration, IMemoryCache iMemoryCache)
        {
            _logger = logger;
            _IDataLogic = iDataLogic;
            _IInProcessInspection = iIInProcessInspection;
            _IWebHostEnvironment = iWebHostEnvironment;
            this.iconfiguration = iconfiguration;
            _MemoryCache = iMemoryCache;
        }
        [Route("{controller}/Index")]
        [HttpGet]
        public async Task<ActionResult> InProcessInspection(int ID, string Mode, int YC, string FromDate, string ToDate,string EntryDate,
		string TestingDate, string InspectionType, string SlipNo,

        int ShiftID, string Shift, string InspTimeFrom, string InspTimeTo, int ItemCode, string PartName,string PartCode,

        int SampleSize, string ProjectNo, string ProjectDate, int ProjectYearCode, string Color,
		int MachineId, string MachineNo, int AccountCode, string CustomerName, int NoOfCavity, string MRNNo,

		int MRNYearCode, string MRNDate, string ProdSlipNo, int ProdYearCode, string ProdDate, decimal MRNQty,
		decimal ProdQty, decimal InspActqty, decimal OkQty, decimal Rejqty, string LotNo, decimal Weight,
		 string Remark, string CC, string ActualEntryDate, int ActualEntryBy,string Material,int RevNo,
		string ActualEntryByName, string EntryByMachine, int LastUpdatedBy, string LastUpdationDate,

		string LastUpdatedByName, int ApprovedBy, int InspectedBy, string Attachment1, string Attachment2

            )
        {
            var MainModel = new InProcessInspectionModel();
            MainModel.DTSSGrid = new List<InProcessInspectionDetailModel>();
            MainModel.FromDate = HttpContext.Session.GetString("FromDate");
            MainModel.ToDate = HttpContext.Session.GetString("ToDate"); 
			MainModel.Entry_Date = DateTime.Today.ToString("dd/MM/yyyy").Replace("-", "/");
			MainModel.YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
            MainModel.ActualEntryByName = HttpContext.Session.GetString("EmpName");
            MainModel.ApprovedByEmpName = HttpContext.Session.GetString("EmpName");
            MainModel.InspectedByEmpName = HttpContext.Session.GetString("EmpName");
            MainModel.ActualEntryBy = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
            MainModel.ApprovedBy = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
            MainModel.InspectedBy = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
            MainModel.CC = HttpContext.Session.GetString("Branch");
            //MainModel.YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
         

            HttpContext.Session.Remove("KeyInProcessInspectionGrid");
            if (!string.IsNullOrEmpty(Mode) && ID > 0 && (Mode == "U" || Mode == "V"))
            {


                MainModel.Mode = Mode;
                MainModel = await _IInProcessInspection.GetViewByID(ID, YC, FromDate, ToDate).ConfigureAwait(false);
                MainModel.Mode = Mode; // Set Mode to Update
                MainModel.EntryId = ID;
                MainModel.YearCode = YC;
                MainModel.Entry_Date = EntryDate;
                MainModel.TestingDate = TestingDate;
                MainModel.InspectionType = InspectionType;
                MainModel.SlipNo = SlipNo;
                MainModel.ShiftID = ShiftID;
                MainModel.Shift = Shift;
                MainModel.InspTimeFrom = InspTimeFrom;
                MainModel.InspTimeTo = InspTimeTo;
                MainModel.ItemCode = ItemCode;
                MainModel.PartName = PartName;
                MainModel.PartNo = PartCode;
                MainModel.SampleSize = SampleSize;
                MainModel.ProjectNo = ProjectNo;
                MainModel.ProjectDate = ProjectDate;
                MainModel.ProjectYearCode = ProjectYearCode;
                MainModel.Color = Color;
                MainModel.MachineId = MachineId;
                MainModel.MachineNo = MachineNo;
                MainModel.AccountCode = AccountCode;
                MainModel.CustomerName = CustomerName;
                MainModel.NoOfCavity = NoOfCavity;
                MainModel.MRNNo = MRNNo;
                MainModel.MRNYearCode = MRNYearCode;
                MainModel.MRNDate = MRNDate;
                MainModel.ProdSlipNo = ProdSlipNo;
                MainModel.ProdYearCode = ProdYearCode;
                MainModel.ProdDate = ProdDate;
                MainModel.MRNQty = MRNQty;
                MainModel.ProdQty = ProdQty;
                MainModel.InspActqty = InspActqty;
                MainModel.OkQty = OkQty;
                MainModel.Rejqty = Rejqty;
                MainModel.LotNo = LotNo;
                MainModel.Weight = Weight;
                MainModel.Remark = Remark;
                MainModel.CC = CC;
                MainModel.ActualEntryDate = ActualEntryDate;
                MainModel.ActualEntryBy = ActualEntryBy;
                MainModel.ActualEntryByName = ActualEntryByName;
                MainModel.EntryByMachine = EntryByMachine;
                MainModel.LastUpdatedBy = LastUpdatedBy;
                MainModel.LastUpdationDate = LastUpdationDate;
                MainModel.LastUpdatedByName = LastUpdatedByName;
                MainModel.ApprovedBy = ApprovedBy;
                MainModel.InspectedBy = InspectedBy;
                MainModel.Attachment1 = Attachment1;
                MainModel.Attachment2 = Attachment2;
                MainModel.Material = Material;
                MainModel.RevNo = RevNo;

                ViewBag.SampleSize = SampleSize;
                MainModel.SelectedInspections = new List<string>();

				if (MainModel.InspectedBeginingOfProd)
					MainModel.SelectedInspections.Add("BegingOfProduction");

				if (MainModel.InspectedAfterMoldCorrection)
					MainModel.SelectedInspections.Add("AfterMouldCorrection");

				if (MainModel.InspectedAfterLotChange)
					MainModel.SelectedInspections.Add("AfterMaterialLotChange");

				if (MainModel.InspectedAfterMachinIdel)
					MainModel.SelectedInspections.Add("AfterMachineIdel");

				if (MainModel.InspectedEndOfProd)
					MainModel.SelectedInspections.Add("EndOfProduction");

				if (MainModel.InspectedEndOfProd)
					MainModel.SelectedInspections.Add("AfterMachineBreackDown");

				string serializedGrid = JsonConvert.SerializeObject(MainModel.DTSSGrid);
                HttpContext.Session.SetString("KeyInProcessInspectionGrid", serializedGrid);


                if (Mode == "U")
                {
                    MainModel.LastUpdatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                    MainModel.LastUpdatedByName = HttpContext.Session.GetString("EmpName");
                    MainModel.LastUpdationDate = DateTime.Today.ToString("MM/dd/yyyy").Replace("-", "/");
					MainModel.ApprovedByEmpName = HttpContext.Session.GetString("EmpName");
					MainModel.InspectedByEmpName = HttpContext.Session.GetString("EmpName");
					MainModel.ApprovedBy = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
					MainModel.InspectedBy = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
				}
            }
            if (MainModel.InspectionOptions == null || !MainModel.InspectionOptions.Any())
            {
                MainModel.InspectionOptions = new List<InspectionOption>
    {
        new InspectionOption { Key = "BegingOfProduction", Text = "Beginning of Production" },
        new InspectionOption { Key = "AfterMouldCorrection", Text = "After Mould Correction" },
        new InspectionOption { Key = "AfterMachineBreackDown", Text = "After Machine Breakdown" },
        new InspectionOption { Key = "AfterMaterialLotChange", Text = "After Material Lot Change" },
        new InspectionOption { Key = "AfterMachineIdel", Text = "After Machine Idle" },
        new InspectionOption { Key = "EndOfProduction", Text = "End of Production" }
    };
            }

            return View(MainModel); // Pass the model with old data to the view
        }
		[Route("{controller}/Index")]
		[HttpPost]

		public async Task<IActionResult> InProcessInspection(InProcessInspectionModel model)
		
		{
			try
			{
				// Get session-stored detail grid
				string modelJson = HttpContext.Session.GetString("KeyInProcessInspectionGrid");
				List<InProcessInspectionDetailModel> InProcessInspectionDetail = new List<InProcessInspectionDetailModel>();
				if (!string.IsNullOrEmpty(modelJson))
				{
					InProcessInspectionDetail = JsonConvert.DeserializeObject<List<InProcessInspectionDetailModel>>(modelJson);
				}
				
				// Store updated detail back in session
				HttpContext.Session.SetString("KeyInProcessInspectionGrid", JsonConvert.SerializeObject(InProcessInspectionDetail));

				var GIGrid = GetDetailTable(InProcessInspectionDetail);

				model.CreatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
				model.ActualEntryBy = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
				var selectedKeys = model.SelectedInspections ?? new List<string>();
				model.InspectedBeginingOfProd = selectedKeys.Contains("BegingOfProduction");
				model.InspectedAfterMoldCorrection = selectedKeys.Contains("AfterMouldCorrection");
				model.InspectedAfterLotChange = selectedKeys.Contains("AfterMaterialLotChange");
				model.InspectedAfterMachinIdel = selectedKeys.Contains("AfterMachineIdel");
				model.InspectedEndOfProd = selectedKeys.Contains("EndOfProduction");

				var Result = await _IInProcessInspection.SaveInprocessInspection(model, GIGrid);

				if (Result != null)
				{
					if (Result.StatusText == "Success" && Result.StatusCode == HttpStatusCode.OK)
					{
						ViewBag.isSuccess = true;
						TempData["200"] = "200";
						HttpContext.Session.Remove("KeyInProcessInspectionGrid");
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

				return RedirectToAction(nameof(InProcessInspectionDashBoard));
			}
			catch (Exception ex)
			{
				LogException<InProcessInspectionController>.WriteException(_logger, ex);
				var ResponseResult = new ResponseResult
				{
					StatusCode = HttpStatusCode.InternalServerError,
					StatusText = "Error",
					Result = ex
				};
				return View("Error", ResponseResult);
			}
		}
		private static DataTable GetDetailTable(IList<InProcessInspectionDetailModel> DetailList)
			{
			try
			{
				var GIGrid = new DataTable();
				GIGrid.Columns.Add("InspEntryId", typeof(long));
				GIGrid.Columns.Add("InspYearCode", typeof(int));
				GIGrid.Columns.Add("SeqNo", typeof(int));
				GIGrid.Columns.Add("Characteristic", typeof(string));
				GIGrid.Columns.Add("EvalutionMeasurmentTechnique", typeof(string));
				GIGrid.Columns.Add("SpecificationFrom", typeof(string));
				GIGrid.Columns.Add("Operator", typeof(string));
				GIGrid.Columns.Add("SpecificationTo", typeof(string));
				GIGrid.Columns.Add("FrequencyofTesting", typeof(string));
				GIGrid.Columns.Add("InspectionBy", typeof(string));
				GIGrid.Columns.Add("ControlMethod", typeof(string));
				GIGrid.Columns.Add("RejectionPlan", typeof(string));
				GIGrid.Columns.Add("Remarks", typeof(string));
				for (int i = 1; i <= 25; i++)
				{
					GIGrid.Columns.Add($"InspValue{i}", typeof(string));
				}

				foreach (var Item in DetailList)
				{
					
					var rowValues = new List<object>
			{
				Item.InspEntryId ?? 0,
				Item.InspYearCode ?? 0,
				Item.SeqNo ?? 0,
				Item.Characteristic ?? "",
				Item.EvalutionMeasurmentTechnique ?? "",
				Item.SpecificationFrom ?? "",
				Item.Operator ?? "",
				Item.SpecificationTo ?? "",
				Item.FrequencyofTesting ?? "",
				Item.InspectionBy ?? "",
				Item.ControlMethod ?? "",
				Item.RejectionPlan ?? "",
				Item.Remarks ?? ""
			};

					
					//for (int i = 1; i <= 25; i++)
					//{
					//	var propertyName = $"InspValue{i}";
					//	var prop = Item.GetType().GetProperty(propertyName);
					//	decimal value = 0;

					//	//if (prop != null)
					//	//{
					//	//	var propValue = prop.GetValue(Item);
					//	//	value = propValue != null ? Convert.ToDecimal(propValue) : 0;
					//	//}
					//	if (prop != null && decimal.TryParse(prop.ToString(), out var result))
					//	{
					//		value = result; 
					//	}
					//	rowValues.Add(value);
					//}

					for (int i = 1; i <= 25; i++)
{
	var propertyName = $"InspValue{i}";
	var prop = Item.GetType().GetProperty(propertyName);

	if (prop != null)
	{
		var value = prop.GetValue(Item)?.ToString() ?? "0";
		rowValues.Add(value);
	}
	else
	{
		rowValues.Add("0");
	}
}
					GIGrid.Rows.Add(rowValues.ToArray());
				}

				return GIGrid;
			}
			catch (Exception ex)
			{
				throw;
			}
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
		public async Task<JsonResult> FillEntryID(int YearCode,string TestingDate)
		{
			var JSON = await _IInProcessInspection.FillEntryID(YearCode, TestingDate);
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
		public async Task<IActionResult> GetInprocessInspectionGridData(int ItemCode, int SampleSize, int? DeletedSeqNo,string? DeletedCharacteristic)
		{
            //model.Mode = "Search";
            ViewBag.SampleSize = SampleSize;
			ViewBag.DeletedSeqNo = DeletedSeqNo;
			ViewBag.DeletedCharacteristic = DeletedCharacteristic?.Trim().ToLower() ?? "";
			var model = new InProcessInspectionModel();
			model = await _IInProcessInspection.GetInprocessInspectionGridData(ItemCode,SampleSize);
			
			return PartialView("_InprocessInspectionGrid", model);
			

		
		}

		[HttpPost]
        [HttpPost]
        public IActionResult AddToGridData(List<InProcessInspectionDetailModel> modelList, int sampleSize, int? deletedSeqNo)
        {
            try
            {
                if (modelList == null || !modelList.Any())
                    return BadRequest("No data received.");

                string modelJson = HttpContext.Session.GetString("KeyInProcessInspectionGrid");
                List<InProcessInspectionDetailModel> existingGrid = new();

                if (!string.IsNullOrEmpty(modelJson))
                    existingGrid = JsonConvert.DeserializeObject<List<InProcessInspectionDetailModel>>(modelJson);

                var usedSeqNos = existingGrid.Select(x => x.SeqNo).ToList();

                // Restore soft-deleted row if applicable
                if (deletedSeqNo.HasValue)
                {
                    var deletedRow = existingGrid.FirstOrDefault(x => x.SeqNo == deletedSeqNo.Value && x.Copied == true);
                    if (deletedRow != null)
                    {
                        var formModel = modelList.FirstOrDefault(x => x.SeqNo == deletedSeqNo.Value)
                                     ?? modelList.First();

                        deletedRow.Copied = false;
                        deletedRow.EvalutionMeasurmentTechnique = formModel.EvalutionMeasurmentTechnique;
                        deletedRow.ControlMethod = formModel.ControlMethod;
                        deletedRow.SpecificationFrom = formModel.SpecificationFrom;
                        deletedRow.SpecificationTo = formModel.SpecificationTo;
                        deletedRow.RejectionPlan = formModel.RejectionPlan;
                        deletedRow.FrequencyofTesting = formModel.FrequencyofTesting;
                        //deletedRow.Samples = formModel.Samples?.ToList() ?? new List<string>();
                        deletedRow.Samples.Clear();
                        foreach (var sample in formModel.Samples)
                        {
                            deletedRow.Samples.Add(sample);
                        }
                        // Remove restored item from the list so it doesn't get added again
                        modelList = modelList.Where(x => x.SeqNo != deletedSeqNo.Value).ToList();
                    }
                }

                foreach (var model in modelList)
                {
                    if (existingGrid.Any(x => x.Characteristic == model.Characteristic) && !model.Copied)
                        continue;

                    int newSeqNo = 1;
                    while (usedSeqNos.Contains(newSeqNo))
                        newSeqNo++;

                    model.SeqNo = newSeqNo;
                    usedSeqNos.Add(newSeqNo);
                    existingGrid.Add(model);
                }

                HttpContext.Session.SetString("KeyInProcessInspectionGrid", JsonConvert.SerializeObject(existingGrid));

                var MainModel = new InProcessInspectionModel
                {
                    DTSSGrid = existingGrid.OrderBy(x => x.SeqNo).ToList(),
                    SampleSize = sampleSize
                };

                ViewBag.SampleSize = sampleSize;
                return PartialView("_InProcessInspectionAddtoGrid", MainModel);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        //public IActionResult AddToGridData(List<InProcessInspectionDetailModel> modelList, int sampleSize)
        //{
        //	try
        //	{
        //		if (modelList == null || !modelList.Any())
        //		{
        //			ModelState.TryAddModelError("Error", "List cannot be empty.");
        //			return BadRequest("No data received.");
        //		}

        //		string modelJson = HttpContext.Session.GetString("KeyInProcessInspectionGrid");
        //		List<InProcessInspectionDetailModel> existingGrid = new List<InProcessInspectionDetailModel>();

        //		if (!string.IsNullOrEmpty(modelJson))
        //		{
        //			existingGrid = JsonConvert.DeserializeObject<List<InProcessInspectionDetailModel>>(modelJson);
        //		}
        //              var usedSeqNos = existingGrid.Select(x => x.SeqNo).ToList();
        //              foreach (var model in modelList)
        //		{

        //			//var deletedRow = existingGrid.FirstOrDefault(x => x.Characteristic == model.Characteristic && x.Copied == true);
        //			var deletedRow = existingGrid.FirstOrDefault(x => x.SeqNo == model.SeqNo && x.Characteristic == model.Characteristic && x.Copied == true);
        //			if (deletedRow != null)
        //			{
        //				// Restore the soft-deleted row with updated values
        //				deletedRow.Copied = false;

        //				// Update other fields as needed (example below, update your actual fields)
        //				deletedRow.EvalutionMeasurmentTechnique = model.EvalutionMeasurmentTechnique;
        //				deletedRow.ControlMethod = model.ControlMethod;
        //				deletedRow.SpecificationFrom = model.SpecificationFrom;
        //				deletedRow.SpecificationTo = model.SpecificationTo;
        //				deletedRow.RejectionPlan = model.RejectionPlan;
        //				deletedRow.FrequencyofTesting = model.FrequencyofTesting;
        //				//deletedRow.Samples = model.Samples;
        //				deletedRow.Samples.Clear();
        //				foreach (var sample in model.Samples)
        //				{
        //					deletedRow.Samples.Add(sample);
        //				}
        //				// No need to add again because we updated the existing soft-deleted row
        //				continue;
        //			}
        //			if (existingGrid.Any(x => x.Characteristic == model.Characteristic) && !model.Copied)
        //			//if (existingGrid.Any(x => x.Characteristic == model.Characteristic && x.SeqNo == model.SeqNo) && !model.Copied)
        //			{
        //				// skip duplicates
        //				continue;
        //			}

        //                  //model.SeqNo = existingGrid.Count + 1;

        //                  int newSeqNo = 1;
        //                  while (usedSeqNos.Contains(newSeqNo))
        //                      newSeqNo++;

        //                  model.SeqNo = newSeqNo;
        //                  usedSeqNos.Add(newSeqNo);

        //			existingGrid.Add(model);
        //		}

        //		HttpContext.Session.SetString("KeyInProcessInspectionGrid", JsonConvert.SerializeObject(existingGrid));
        //		ViewBag.SampleSize = sampleSize;
        //		var MainModel = new InProcessInspectionModel
        //		{
        //                  //DTSSGrid = existingGrid,
        //                  DTSSGrid = existingGrid.OrderBy(x => x.SeqNo).ToList(),
        //                  SampleSize = sampleSize
        //		};

        //		return PartialView("_InProcessInspectionAddtoGrid", MainModel);
        //	}
        //	catch (Exception ex)
        //	{
        //		return StatusCode(500, "Internal server error: " + ex.Message);
        //	}
        //}

        // GET: Fetch item by SeqNo to edit
        public IActionResult EditItemRow(int SeqNo)
			{
				string modelJson = HttpContext.Session.GetString("KeyInProcessInspectionGrid");
				List<InProcessInspectionDetailModel> existingGrid = new List<InProcessInspectionDetailModel>();

				if (!string.IsNullOrEmpty(modelJson))
				{
					existingGrid = JsonConvert.DeserializeObject<List<InProcessInspectionDetailModel>>(modelJson);
				}

				var itemToEdit = existingGrid.FirstOrDefault(x => x.SeqNo == SeqNo);

				if (itemToEdit == null)
				{
					return NotFound("Item not found.");
				}

				return Json(itemToEdit);
			}

			// POST: Update edited item back to session grid
			[HttpPost]
			public IActionResult UpdateItemRow([FromBody] InProcessInspectionDetailModel updatedItem)
			{
				string modelJson = HttpContext.Session.GetString("KeyInProcessInspectionGrid");
				List<InProcessInspectionDetailModel> existingGrid = new List<InProcessInspectionDetailModel>();

				if (!string.IsNullOrEmpty(modelJson))
				{
					existingGrid = JsonConvert.DeserializeObject<List<InProcessInspectionDetailModel>>(modelJson);
				}

				var index = existingGrid.FindIndex(x => x.SeqNo == updatedItem.SeqNo);

				if (index == -1)
				{
					return NotFound("Item not found.");
				}

				// Replace old item with updated one
				existingGrid[index] = updatedItem;

				// Save updated list back to session
				HttpContext.Session.SetString("KeyInProcessInspectionGrid", JsonConvert.SerializeObject(existingGrid));

				// Return updated partial view to refresh grid UI
				var model = new InProcessInspectionModel
				{
					DTSSGrid = existingGrid,
					SampleSize = updatedItem.Samples?.Count ?? 1
				};

				return PartialView("_InProcessInspectionAddtoGrid", model);
			}

		// GET or POST: Delete item by SeqNo and update session grid
		//public IActionResult DeleteItemRow(int SeqNo)
		//{
		//	string modelJson = HttpContext.Session.GetString("KeyInProcessInspectionGrid");
		//	List<InProcessInspectionDetailModel> existingGrid = new List<InProcessInspectionDetailModel>();

		//	if (!string.IsNullOrEmpty(modelJson))
		//	{
		//		existingGrid = JsonConvert.DeserializeObject<List<InProcessInspectionDetailModel>>(modelJson);
		//	}

		//	var itemToRemove = existingGrid.FirstOrDefault(x => x.SeqNo == SeqNo);
		//	if (itemToRemove != null)
		//	{
		//		existingGrid.Remove(itemToRemove);


		//		for (int i = 0; i < existingGrid.Count; i++)
		//		{
		//			existingGrid[i].SeqNo = i + 1;
		//		}

		//	HttpContext.Session.SetString("KeyInProcessInspectionGrid", JsonConvert.SerializeObject(existingGrid));
		//	}

		//	var model = new InProcessInspectionModel
		//	{
		//		DTSSGrid = existingGrid,
		//		SampleSize = existingGrid.FirstOrDefault()?.Samples?.Count ?? 1
		//	};
		//	ViewBag.SampleSize = model.SampleSize;
		//return PartialView("_InProcessInspectionAddtoGrid", model);
		//}

		//sec
		public IActionResult DeleteItemRow(int SeqNo, int SampleSize,string Characteristic)
		{
			string modelJson = HttpContext.Session.GetString("KeyInProcessInspectionGrid");
			List<InProcessInspectionDetailModel> existingGrid = new();

			if (!string.IsNullOrEmpty(modelJson))
			{
				existingGrid = JsonConvert.DeserializeObject<List<InProcessInspectionDetailModel>>(modelJson);
			}

			// Find and remove by SeqNo
			var itemToRemove = existingGrid.FirstOrDefault(x => x.SeqNo == SeqNo);
			if (itemToRemove != null)
			{
				existingGrid.Remove(itemToRemove);
				itemToRemove.Copied = true;
				// Reassign SeqNo to keep them continuous
				//int seq = 1;
				//foreach (var item in existingGrid)
				//{
				//	item.SeqNo = seq++;
				//}

				// Update session
				HttpContext.Session.SetString("KeyInProcessInspectionGrid", JsonConvert.SerializeObject(existingGrid));
			}

			var model = new InProcessInspectionModel
			{
				DTSSGrid = existingGrid,
				//SampleSize = existingGrid.FirstOrDefault()?.Samples?.Count ?? 1
				SampleSize = SampleSize
			};
			ViewBag.SampleSize = model.SampleSize;
            ViewBag.DeletedSeqNo = SeqNo;
            ViewBag.DeletedCharacteristic = itemToRemove.Characteristic;
            return PartialView("_InProcessInspectionAddtoGrid", model);
		}

		public async Task<IActionResult> InProcessInspectionDashBoard(string ReportType, string FromDate, string ToDate)
        {
            var model = new InProcessInspectionModel();
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

			model.ActualEntryBy = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
            model.ReportType = "SUMMARY";
            var Result = await _IInProcessInspection.GetDashboardData(model);

            if (Result.Result != null)
            {
                var _List = new List<TextValue>();
                DataSet DS = Result.Result;
                if (DS != null && DS.Tables.Count > 0)
                {
                    var dt = DS.Tables[0];
                    //model.DTSSGrid = CommonFunc.DataTableToList<InProcessInspectionDetailModel>(dt, "InProcessInspectionDashBoard");
                }

            }

            return View(model);
        }
        public async Task<IActionResult> GetDetailData(string FromDate, string ToDate, string ReportType, string ItemName, string PartCode, string SlipNo, string MachinNo, int pageNumber = 1, int pageSize = 1, string SearchBox = "")
        {
            var model = new InProcessInspectionModel();
            model = await _IInProcessInspection.GetDashboardDetailData(FromDate, ToDate, ReportType,  ItemName,  PartCode,  SlipNo,  MachinNo);
            var modelList = model?.DTSSGrid ?? new List<InProcessInspectionDetailModel>();


            if (string.IsNullOrWhiteSpace(SearchBox))
            {
                model.TotalRecords = modelList.Count();
                model.PageNumber = pageNumber;
                model.PageSize = pageSize;
                model.DTSSGrid = modelList
                .Skip((pageNumber - 1) * pageSize)
                   .Take(pageSize)
                   .ToList();
            }
            else
            {
                List<InProcessInspectionDetailModel> filteredResults;
                if (string.IsNullOrWhiteSpace(SearchBox))
                {
                    filteredResults = modelList.ToList();
                }
                else
                {
                    filteredResults = modelList
                        .Where(i => i.GetType().GetProperties()
                            .Where(p => p.PropertyType == typeof(string))
                            .Select(p => p.GetValue(i)?.ToString())
                            .Any(value => !string.IsNullOrEmpty(value) &&
                                          value.Contains(SearchBox, StringComparison.OrdinalIgnoreCase)))
                        .ToList();


                    if (filteredResults.Count == 0)
                    {
                        filteredResults = modelList.ToList();
                    }
                }

                model.TotalRecords = filteredResults.Count;
                model.DTSSGrid = filteredResults.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
                model.PageNumber = pageNumber;
                model.PageSize = pageSize;
            }
            MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                SlidingExpiration = TimeSpan.FromMinutes(55),
                Size = 1024,
            };

            _MemoryCache.Set("KeynProcessInspectionDashBoardList", modelList, cacheEntryOptions);

            return PartialView("_InProcessInspectionDashBoardGrid", model);
        }
        [HttpGet]
        public IActionResult GlobalSearch(string searchString, string dashboardType = "Summary", int pageNumber = 1, int pageSize = 1)
        {
            var model = new InProcessInspectionModel();
            model.ReportType = dashboardType;

            // Retrieve all cached data (not just 1 page)
            if (!_MemoryCache.TryGetValue("KeynProcessInspectionDashBoardList", out List<InProcessInspectionDetailModel> allData) || allData == null)
            {
                return PartialView("_InProcessInspectionDashBoardGrid", model);
            }

            List<InProcessInspectionDetailModel> filteredResults;

            // Global search across all string properties
            if (string.IsNullOrWhiteSpace(searchString))
            {
                filteredResults = allData;
            }
            else
            {
                filteredResults = allData
                    .Where(i => i.GetType().GetProperties()
                        .Where(p => p.PropertyType == typeof(string))
                        .Select(p => p.GetValue(i)?.ToString())
                        .Any(value => !string.IsNullOrEmpty(value) &&
                                      value.Contains(searchString, StringComparison.OrdinalIgnoreCase)))
                    .ToList();
            }

            // Update model with filtered data and pagination
            model.TotalRecords = filteredResults.Count;
            model.DTSSGrid = filteredResults
                                .Skip((pageNumber - 1) * pageSize)
                                .Take(pageSize)
                                .ToList();
            model.PageNumber = pageNumber;
            model.PageSize = pageSize;

            return PartialView("_InProcessInspectionDashBoardGrid", model);
        }

        public async Task<IActionResult> DeleteByID(int EntryId, int YearCode, string EntryDate, int EntryByempId)
        {
            var Result = await _IInProcessInspection.DeleteByID(EntryId, YearCode, EntryDate, EntryByempId);

            if (Result.StatusText == "Success" || Result.StatusCode == HttpStatusCode.Gone)
            {
                ViewBag.isSuccess = true;
                TempData["410"] = "410";
            }
            else if (Result.StatusText == "Error" || Result.StatusCode == HttpStatusCode.Accepted)
            {
                ViewBag.isSuccess = true;
                TempData["423"] = "423";
            }
            else
            {
                ViewBag.isSuccess = false;
                TempData["500"] = "500";
            }

            return RedirectToAction("InProcessInspectionDashBoard");

        }
        public async Task<IActionResult> ExportInProcessInspectionToExcel(string ReportType)
        {
            //string modelJson = HttpContext.Session.GetString("KeyPOList");
            if (!_MemoryCache.TryGetValue("KeynProcessInspectionDashBoardList", out List<InProcessInspectionDetailModel> modelList))
            {
                return NotFound("No data available to export.");
            }


            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("InProcessInspectionReport");

            var reportGenerators = new Dictionary<string, Action<IXLWorksheet, IList<InProcessInspectionDetailModel>>>
            {
                { "SUMMARY", EXPORT_InProcessInspectionSummaryGrid },

            };

            if (reportGenerators.TryGetValue(ReportType, out var generator))
            {
                generator(worksheet, modelList);
            }
            else
            {
                return BadRequest("Invalid report type.");
            }

            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Position = 0;

            return File(
                stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "InProcess Inspection Report.xlsx"
            );
        }
        private void EXPORT_InProcessInspectionSummaryGrid(IXLWorksheet sheet, IList<InProcessInspectionDetailModel> list)
        {
            string[] headers = {
                "#Sr", "Insp Entry Id", "Insp Year Code", "Entry Date", "Testing Date",
				"Incoming/Inprocess/Outgoing", "Slip No", "Shift Name",
				"Insp Time From", "Insp Time To", "Item Name", "Part Code",
				"Sample Size", "Project No", "Project Date", "Project Year Code",
				"Color", "Material", "RevNo", "Machine Name", "Customer Name",
				"No Of Cavity", "MRN No", "MRN Year Code", "MRN Date",
				"Prod Slip No", "Prod Year Code", "Prod Date", "MRN Qty",
				"Prod Qty", "Insp Actual Qty", "Ok Qty", "Rej Qty", "Lot No",
				"Weight", "Remark", "CC", "Actual Entry Date", "Actual Entered By",
				"Last Updated By", "Last Updated Date", "Approved By", "Machine Name"
            };



            for (int i = 0; i < headers.Length; i++)
                sheet.Cell(1, i + 1).Value = headers[i];

            int row = 2, srNo = 1;
            foreach (var item in list)
            {
                sheet.Cell(row, 1).Value = srNo++;
                sheet.Cell(row, 2).Value = item.InspEntryId;
                sheet.Cell(row, 3).Value = item.InspYearCode;
                sheet.Cell(row, 4).Value = item.Entry_Date?.Split(' ')[0];
                sheet.Cell(row, 5).Value = item.TestingDate?.Split(' ')[0];
                sheet.Cell(row, 6).Value = item.InspectionType;
                sheet.Cell(row, 7).Value = item.SlipNo;
                sheet.Cell(row, 8).Value = item.ShiftName;
                sheet.Cell(row, 9).Value = item.InspTimeFrom?.Split(' ')[1];
                sheet.Cell(row, 10).Value = item.InspTimeTo?.Split(' ')[1];
                sheet.Cell(row, 11).Value = item.ItemName;
                sheet.Cell(row, 12).Value = item.PartCode;
                sheet.Cell(row, 13).Value = item.SampleSize;
                sheet.Cell(row, 14).Value = item.ProjectNo;
                sheet.Cell(row, 15).Value = item.ProjectDate?.Split(' ')[0];
                sheet.Cell(row, 16).Value = item.ProjectYearCode;
                sheet.Cell(row, 17).Value = item.Color;
                sheet.Cell(row, 18).Value = item.Material;
                sheet.Cell(row, 19).Value = item.RevNo;
                sheet.Cell(row, 20).Value = item.MachineName;
                sheet.Cell(row, 21).Value = item.AccountName;
                sheet.Cell(row, 22).Value = item.NoOfCavity;
                sheet.Cell(row, 23).Value = item.MRNNo;
                sheet.Cell(row, 24).Value = item.MRNYearCode;
                sheet.Cell(row, 25).Value = item.MRNDate?.Split(' ')[0];
                sheet.Cell(row, 26).Value = item.ProdSlipNo;
                sheet.Cell(row, 27).Value = item.ProdYearCode;
                sheet.Cell(row, 28).Value = item.ProdDate?.Split(' ')[0];
                sheet.Cell(row, 29).Value = item.MRNQty;
                sheet.Cell(row, 30).Value = item.ProdQty;
                sheet.Cell(row, 31).Value = item.InspActqty;
                sheet.Cell(row, 32).Value = item.OkQty;
                sheet.Cell(row, 33).Value = item.Rejqty;
                sheet.Cell(row, 34).Value = item.LotNo;
                sheet.Cell(row, 35).Value = item.Weight;
                sheet.Cell(row, 36).Value = item.Remarks;
                sheet.Cell(row, 37).Value = item.CC;
                sheet.Cell(row, 38).Value = item.ActualEntryDate?.Split(' ')[0];
                sheet.Cell(row, 39).Value = item.ActualEntryByName;
                sheet.Cell(row, 40).Value = item.LastUpdatedBy;
                sheet.Cell(row, 41).Value = item.LastUpdationDate?.Split(' ')[0];
                sheet.Cell(row, 42).Value = item.ApprovedByName;
                sheet.Cell(row, 43).Value = item.EntryByMachine;


                row++;
            }
        }
    }
}
