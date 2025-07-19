using DocumentFormat.OpenXml.Office2010.Excel;
using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using PdfSharp.Drawing.BarCodes;
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
        public async Task<ActionResult> InProcessInspection(int ID, string Mode, int YC, string FromDate, string ToDate,string EntryDate)
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
            MainModel.InspectionOptions = new List<InspectionOption>
			{
				new InspectionOption { Key = "BegingOfProduction", Text = "Beginning of Production" },
				new InspectionOption { Key = "AfterMouldCorrection", Text = "After Mould Correction" },
				new InspectionOption { Key = "AfterMachineBreackDown", Text = "After Machine Breakdown" },
				new InspectionOption { Key = "AfterMaterialLotChange", Text = "After Material Lot Change" },
				new InspectionOption { Key = "AfterMachineIdel", Text = "After Machine Idle" },
				new InspectionOption { Key = "EndOfProduction", Text = "End of Production" }
            };

            HttpContext.Session.Remove("KeyInProcessInspectionGrid");
            if (!string.IsNullOrEmpty(Mode) && ID > 0 && (Mode == "U" || Mode == "V"))
            {


                MainModel.Mode = Mode;
                MainModel = await _IInProcessInspection.GetViewByID(ID, YC, FromDate, ToDate).ConfigureAwait(false);
                MainModel.Mode = Mode; // Set Mode to Update
                MainModel.EntryId = ID;
                MainModel.YearCode = YC;
                MainModel.Entry_Date = EntryDate;

                string serializedGrid = JsonConvert.SerializeObject(MainModel.DTSSGrid);
                HttpContext.Session.SetString("KeyControlPlanGrid", serializedGrid);


                if (Mode == "U")
                {
                    MainModel.LastUpdatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                    MainModel.LastUpdatedByName = HttpContext.Session.GetString("EmpName");
                    MainModel.LastUpdationDate = DateTime.Today.ToString("MM/dd/yyyy").Replace("-", "/");
                }
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
					GIGrid.Columns.Add($"InspValue{i}", typeof(decimal));
				}

				foreach (var Item in DetailList)
				{
					// Build row values
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

					// Add InspValue1 to InspValue25
					for (int i = 1; i <= 25; i++)
					{
						var propertyName = $"InspValue{i}";
						var prop = Item.GetType().GetProperty(propertyName);
						decimal value = 0;

						if (prop != null)
						{
							var propValue = prop.GetValue(Item);
							value = propValue != null ? Convert.ToDecimal(propValue) : 0;
						}

						rowValues.Add(value);
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

		[HttpPost]
		public IActionResult AddToGridData(List<InProcessInspectionDetailModel> modelList, int sampleSize)
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
				ViewBag.SampleSize = sampleSize;
				var MainModel = new InProcessInspectionModel
				{
					DTSSGrid = existingGrid,
                    SampleSize = sampleSize
				};

				return PartialView("_InProcessInspectionAddtoGrid", MainModel);
			}
			catch (Exception ex)
			{
				return StatusCode(500, "Internal server error: " + ex.Message);
			}
		}
		
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
			public IActionResult DeleteItemRow(int SeqNo)
			{
				string modelJson = HttpContext.Session.GetString("KeyInProcessInspectionGrid");
				List<InProcessInspectionDetailModel> existingGrid = new List<InProcessInspectionDetailModel>();

				if (!string.IsNullOrEmpty(modelJson))
				{
					existingGrid = JsonConvert.DeserializeObject<List<InProcessInspectionDetailModel>>(modelJson);
				}

				var itemToRemove = existingGrid.FirstOrDefault(x => x.SeqNo == SeqNo);
				if (itemToRemove != null)
				{
					existingGrid.Remove(itemToRemove);

				
					for (int i = 0; i < existingGrid.Count; i++)
					{
						existingGrid[i].SeqNo = i + 1;
					}

				HttpContext.Session.SetString("KeyInProcessInspectionGrid", JsonConvert.SerializeObject(existingGrid));
				}

				var model = new InProcessInspectionModel
				{
					DTSSGrid = existingGrid,
					SampleSize = existingGrid.FirstOrDefault()?.Samples?.Count ?? 1
				};
				ViewBag.SampleSize = model.SampleSize;
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
        public async Task<IActionResult> GetDetailData(string FromDate, string ToDate, string ReportType)
        {
            var model = new InProcessInspectionModel();
            model = await _IInProcessInspection.GetDashboardDetailData(FromDate, ToDate, ReportType);
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
    }
}
