using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using static eTactWeb.Data.Common.CommonFunc;
using static eTactWeb.DOM.Models.Common;
using System.Net;
using System.Data;

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
		public async Task<ActionResult> AccDepriciationCalculationdetail(int ID, int YC, string Mode)
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
		[Route("{controller}/Index")]
		[HttpPost]

		public async Task<IActionResult> AccDepriciationCalculationdetail(AccDepriciationCalculationdetailModel model)
		{
			try
			{
				string modelJson = HttpContext.Session.GetString("KeyAccDepriciationCalculationdetailGrid");
				List<AccDepriciationCalculationdetailModel> DepriciationCalculationdetail = new List<AccDepriciationCalculationdetailModel>();

				if (!string.IsNullOrEmpty(modelJson))
				{
					DepriciationCalculationdetail = JsonConvert.DeserializeObject<List<AccDepriciationCalculationdetailModel>>(modelJson);
				}

				// Now use this list to build DataTable
				model.ActualEntryBy = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
				var GIGrid = GetDetailTable(DepriciationCalculationdetail);
				var Result = await _IAccDepriciationCalculationdetail.SaveDepriciationCalculationdetail(model, GIGrid);

				if (Result != null)
				{
					if (Result.StatusText == "Success" && Result.StatusCode == HttpStatusCode.OK)
					{
						ViewBag.isSuccess = true;
						TempData["200"] = "200";
						HttpContext.Session.Remove("KeyAccDepriciationCalculationdetailGrid");
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

				return RedirectToAction(nameof(AccDepriciationCalculationdetail));
			}
			catch (Exception ex)
			{
				LogException<AccDepriciationCalculationdetailController>.WriteException(_logger, ex);
				var ResponseResult = new ResponseResult
				{
					StatusCode = HttpStatusCode.InternalServerError,
					StatusText = "Error",
					Result = ex
				};
				return View("Error", ResponseResult);
			}
		}
		private static DataTable GetDetailTable(IList<AccDepriciationCalculationdetailModel> DetailList)
		{
			try
			{
				var GIGrid = new DataTable();
				GIGrid.Columns.Add("SeqNo", typeof(long));
				GIGrid.Columns.Add("DepriciationEntryId", typeof(long));
				GIGrid.Columns.Add("DepriciationYearCode", typeof(long));
				GIGrid.Columns.Add("DepricationForFinancialYear", typeof(long));
				GIGrid.Columns.Add("AssetsEntryId", typeof(long));
				GIGrid.Columns.Add("AccountCode", typeof(long));
				GIGrid.Columns.Add("ItemCode", typeof(long));
				GIGrid.Columns.Add("MainGroup", typeof(string));
				GIGrid.Columns.Add("ParentAccountCode", typeof(long));
				GIGrid.Columns.Add("SubGroup", typeof(string));
				GIGrid.Columns.Add("UnderGroup", typeof(string));
				GIGrid.Columns.Add("SubSubGroup", typeof(string));
				GIGrid.Columns.Add("originalNetBookValue", typeof(decimal));
				GIGrid.Columns.Add("PreviousYearValue", typeof(decimal));
				GIGrid.Columns.Add("DepreciationMethod", typeof(string));
				GIGrid.Columns.Add("DepreciationRate", typeof(decimal));
				GIGrid.Columns.Add("AfterDepriciationNetValue", typeof(decimal));
				GIGrid.Columns.Add("RemainingUseFullLifeInYear", typeof(decimal));
				GIGrid.Columns.Add("CarryForwarded", typeof(string));
				foreach (var item in DetailList)
				{
					GIGrid.Rows.Add(
						new object[]
						{
					item.SeqNo,
		item.DepriciationEntryId,
		item.DepriciationYearCode,
		item.ForClosingOfFinancialYear,
		item.AssetsEntryId,
		item.AccountCode,
		item.ItemCode,
		item.MainGroup,
		item.ParentAccountCode,
		item.SubGroup,
		item.UnderGroup,
		item.SubSubGroup,
		item.OriginalNetBookValue,
		item.PreviousYearValue,
		item.DepreciationMethod,
		item.DepreciationRate,
		item.AfterDepriciationNetValue,
		item.RemainingUseFullLifeInYear,
		item.CarryForwarded

						});
				}
				GIGrid.Dispose();
				return GIGrid;
			}
			catch (Exception ex)
			{
				throw;
			}
		}
		public async Task<IActionResult> GetAssets(int DepriciationYearCode)
		{
			var model = new AccDepriciationCalculationdetailModel();
			model = await _IAccDepriciationCalculationdetail.GetAssets(DepriciationYearCode);
			var serializedGrid = System.Text.Json.JsonSerializer.Serialize(model.AccDepriciationCalculationdetailGrid);
			HttpContext.Session.SetString("KeyAccDepriciationCalculationdetailGrid", serializedGrid);
			return PartialView("_AccDepriciationCalculationdetailGrid", model);
		}
		
		public IActionResult DeleteItemRow(int SeqNo)
		{
			var MainModel = new AccDepriciationCalculationdetailModel();
			string jsonString = HttpContext.Session.GetString("KeyAccDepriciationCalculationdetailGrid");
			IList<AccDepriciationCalculationdetailModel> ControlPlanDetail = new List<AccDepriciationCalculationdetailModel>();

			if (!string.IsNullOrEmpty(jsonString))
			{
				ControlPlanDetail = JsonConvert.DeserializeObject<List<AccDepriciationCalculationdetailModel>>(jsonString);
			}

			if (ControlPlanDetail != null && ControlPlanDetail.Count > 0)
			{
				var itemToRemove = ControlPlanDetail.FirstOrDefault(x => x.AssetsEntryId == SeqNo);
				if (itemToRemove != null)
					ControlPlanDetail.Remove(itemToRemove);

				MainModel.AccDepriciationCalculationdetailGrid = ControlPlanDetail.OrderBy(x => x.SeqNo).ToList();
				HttpContext.Session.SetString("KeyAccDepriciationCalculationdetailGrid", JsonConvert.SerializeObject(MainModel.AccDepriciationCalculationdetailGrid));
			}

			return PartialView("_AccDepriciationCalculationdetailGrid", MainModel);
		}
	}
}
