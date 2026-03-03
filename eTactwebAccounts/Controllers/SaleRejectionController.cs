using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using NuGet.Packaging;
using static eTactWeb.DOM.Models.Common;
using static eTactWeb.Data.Common.CommonFunc;
using eTactWeb.Data.Common;
using eTactWeb.Services.Interface;
using Microsoft.Identity.Client;
using eTactWeb.DOM.Models;
using System.Net;
using System.Data;
using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.Office2010.Excel;
using PdfSharp.Drawing.BarCodes;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;
using eTactWeb.Data.BLL;

namespace eTactWeb.Controllers
{
	public class SaleRejectionController : Controller
	{
		private readonly ISaleRejection _saleRejection;
		private readonly ILogger<SaleRejectionController> _logger;
		public SaleRejectionController(ISaleRejection saleRejection, ILogger<SaleRejectionController> logger)
		{
			_saleRejection = saleRejection;
			_logger = logger;
		}


		[HttpGet]
		[Route("{controller}/Index")]
		public async Task<IActionResult> SaleRejection(string mrnNo, int mrnEntryId, int mrnYC)
		{
			SaleRejectionModel model = new SaleRejectionModel();
			ViewData["Title"] = "Sale Rejection";
			//TempData.Clear();
			HttpContext.Session.Remove("KeySaleRejectionGrid");
			
			HttpContext.Session.Remove("SaleRejectionModel");
			HttpContext.Session.Remove("KeyAdjGrid");
			HttpContext.Session.Remove("KeyTaxGrid");

			string encID = Request.Query["mrnEntryId"].ToString();
			string encYC = Request.Query["mrnYC"].ToString();
			if (!string.IsNullOrEmpty(encID) || !string.IsNullOrEmpty(encYC))
			{
				int decryptedID = EncryptDecrypt.DecodeID(encID);
				int decryptedYC = EncryptDecrypt.DecodeID(encYC);
				mrnYC = decryptedYC;
				mrnEntryId = decryptedID;





			}


			//model.Uid = Convert.ToInt32(HttpContext.Session.GetString("UID"));
			//model.ActualEnteredBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
			//model.ActualEnteredByName = GetEmpByMachineName();
			//model.ActualEnteredByName = HttpContext.Session.GetString("EmpName");
			model.MrnNo = mrnNo;
			model.Mrnyearcode = mrnYC;
			model.MRNEntryId = mrnEntryId;
			// model.ActualEnteredByName = HttpContext.Session.GetString("EmpName");
			model.SaleRejYearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));

			model = await _saleRejection.FillSaleRejectionGrid(mrnNo, mrnEntryId, mrnYC, model.SaleRejYearCode);

			model.adjustmentModel = new AdjustmentModel();
			model.MrnNo = mrnNo;
			model.Mrnyearcode = mrnYC;
			model.MRNEntryId = mrnEntryId;

			MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
			{
				AbsoluteExpiration = DateTime.Now.AddMinutes(60),
				SlidingExpiration = TimeSpan.FromMinutes(55),
				Size = 1024,
			};



			model.FinFromDate = HttpContext.Session.GetString("FromDate");
			model.FinToDate = HttpContext.Session.GetString("ToDate");
			model.SaleRejYearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
			model.CC = HttpContext.Session.GetString("Branch");
			HttpContext.Session.SetString("KeySaleRejectionGrid", JsonConvert.SerializeObject(model.SaleRejectionDetails));
			HttpContext.Session.SetString("KeyAdjGrid", JsonConvert.SerializeObject(model.adjustmentModel == null ? new AdjustmentModel() : model.adjustmentModel));
			HttpContext.Session.SetString("SaleRejectionModel", JsonConvert.SerializeObject(model));
			HttpContext.Session.SetString("SaleRejection", JsonConvert.SerializeObject(model));

			if (model.Mode == "U")
			{
				model.UpdatedByName = HttpContext.Session.GetString("EmpName").ToString();
			}
			model.Uid = Convert.ToInt32(HttpContext.Session.GetString("UID"));

			model.ActualEnteredBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
			model.ActualEnteredByName = GetEmpByMachineName();

			model.EntryByempId = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
			model.ActualEntryDate = HttpContext.Session.GetString("ActualEntryDate") ?? ParseFormattedDate(DateTime.Today.ToString("dd/MM/yyyy"));
			model.MachineName = GetEmpByMachineName();
			return View(model);
		}
		public async Task<JsonResult> GetFormRights()
		{
			var userID = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
			var JSON = await _saleRejection.GetFormRights(userID);
			string JsonString = JsonConvert.SerializeObject(JSON);
			return Json(JsonString);
		}
		public string GetEmpByMachineName()
		{
			try
			{
				string empname = string.Empty;
				empname = HttpContext.Session.GetString("EmpName").ToString();
				if (string.IsNullOrEmpty(empname)) { empname = Environment.UserDomainName; }
				return empname;
			}
			catch
			{
				return "";
			}
		}
		[HttpGet]
		public async Task<IActionResult> SaleRejectionEdit(int ID, string Mode, int YearCode)
		{
			SaleRejectionModel model = new SaleRejectionModel();
			ViewData["Title"] = "Sale Rejection";
			//TempData.Clear();
			HttpContext.Session.Remove("KeySaleRejectionGrid");
			HttpContext.Session.Remove("SaleRejectionModel");
			HttpContext.Session.Remove("KeyAdjGrid");
			HttpContext.Session.Remove("KeyTaxGrid");

			int userID = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
			var rights = await _saleRejection.GetFormRights(userID);
			if (rights?.Result == null || rights.Result.Tables.Count == 0 || rights.Result.Tables[0].Rows.Count == 0)
			{
				return RedirectToAction("Dashboard", "Home");
			}
			var table = rights.Result.Tables[0];



			string encID = RouteData.Values["id"]?.ToString();
			string encYC = Request.Query["YearCode"].ToString();

			if (!string.IsNullOrEmpty(encID) || !string.IsNullOrEmpty(encYC))
			{
				int decryptedID = EncryptDecrypt.DecodeID(encID);
				int decryptedYC = EncryptDecrypt.DecodeID(encYC);

				ID = decryptedID;
				YearCode = decryptedYC;


			}

			bool optAll = Convert.ToBoolean(table.Rows[0]["OptAll"]);
			bool optView = Convert.ToBoolean(table.Rows[0]["OptView"]);
			bool optUpdate = Convert.ToBoolean(table.Rows[0]["OptUpdate"]);
			bool optSave = Convert.ToBoolean(table.Rows[0]["OptSave"]);


			if (Mode == "U")
			{
				if (!(optUpdate))
				{
					return RedirectToAction("Dashboard", "Home");
				}
			}
			else if (Mode == "V")
			{
				if (!(optView))
				{
					return RedirectToAction("Dashboard", "Home");
				}
			}
			else if (ID <= 0)
			{
				if (!optSave)
				{
					return RedirectToAction("BankReceiptDashBoard", "BankReceipt");
				}
				//if (!(optAll || optSave))
				//{
				//    return RedirectToAction("Dashboard", "Home");
				//}

			}





			if (model.Mode != "U")
			{
				model.Uid = Convert.ToInt32(HttpContext.Session.GetString("UID"));
				model.ActualEnteredBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
				model.ActualEnteredByName = GetEmpByMachineName();
				model.ActualEntryDate = HttpContext.Session.GetString("ActualEntryDate") ?? ParseFormattedDate(DateTime.Today.ToString("dd/MM/yyyy"));
				model.MachineName = GetEmpByMachineName();
			}
			else
			{
				model.ActualEnteredByName = GetEmpByMachineName();
				model.UpdatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
				model.LastUpdatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));

				//model.LastUpdatedByName = GetEmpByMachineName();
				//model.LastUpdationDate = HttpContext.Session.GetString("LastUpdatedDate");
				model.UpdatedOn = ParseSafeDate(HttpContext.Session.GetString("LastUpdatedDate"));
				model.MachineName = GetEmpByMachineName();
			}
			//model.adjustmentModel = new AdjustmentModel();

			
			if (!string.IsNullOrEmpty(Mode) && ID > 0 && (Mode == "V" || Mode == "U"))
			{
				model = await _saleRejection.GetViewByID(ID, Mode, YearCode);
				model.Mode = Mode;
				model.ID = ID;

				model.FinFromDate = model.FinFromDate ?? HttpContext.Session.GetString("FromDate");
				model.FinToDate = model.FinToDate ?? HttpContext.Session.GetString("ToDate");
				model.SaleRejYearCode = model.SaleRejYearCode != null && model.SaleRejYearCode > 0 ? model.SaleRejYearCode : Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
				model.CC = model.CC ?? HttpContext.Session.GetString("Branch");
			}
			else
			{
				model.FinFromDate = HttpContext.Session.GetString("FromDate");
				model.FinToDate = HttpContext.Session.GetString("ToDate");
				model.SaleRejYearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
				model.CC = HttpContext.Session.GetString("Branch");
			}
			MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
			{
				AbsoluteExpiration = DateTime.Now.AddMinutes(60),
				SlidingExpiration = TimeSpan.FromMinutes(55),
				Size = 1024,
			};
			model.FinFromDate = HttpContext.Session.GetString("FromDate");
			model.FinToDate = HttpContext.Session.GetString("ToDate");
			model.SaleRejYearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
			model.CC = HttpContext.Session.GetString("Branch");
			model.UpdatedByName = HttpContext.Session.GetString("EmpName");

			HttpContext.Session.SetString("SaleRejectionModel", JsonConvert.SerializeObject(model));

			string serializedKeyAdjGrid = JsonConvert.SerializeObject(model.adjustmentModel == null ? new AdjustmentModel() : model.adjustmentModel);
			string serializedKeyTaxGrid = JsonConvert.SerializeObject(model.TaxDetailGridd == null ? new List<TaxModel>() : model.TaxDetailGridd);
			HttpContext.Session.SetString("KeySaleRejectionGrid", JsonConvert.SerializeObject(model.SaleRejectionDetails));
			// HttpContext.Session.SetString("KeyAdjGrid", JsonConvert.SerializeObject(model.adjustmentModel == null ? new AdjustmentModel() : model.adjustmentModel));
			HttpContext.Session.SetString("SaleRejectionModel", JsonConvert.SerializeObject(model));
			HttpContext.Session.SetString("SaleRejection", JsonConvert.SerializeObject(model));
			string serializedKeyDbCrGrid = JsonConvert.SerializeObject(model.DbCrGrid == null ? new List<DbCrModel>() : model.DbCrGrid);
			if (!string.IsNullOrEmpty(Mode) && ID > 0 && (Mode == "V" || Mode == "U"))
			{
				HttpContext.Session.SetString("KeyDrCrGrid", serializedKeyDbCrGrid);
			}
			HttpContext.Session.SetString("KeyTaxGrid", serializedKeyTaxGrid);

			//HttpContext.Session.SetString("KeySaleRejectionGrid", JsonConvert.SerializeObject(model.SaleRejectionDetails));
			HttpContext.Session.SetString("KeyAdjGrid", JsonConvert.SerializeObject(model.adjustmentModel == null ? new AdjustmentModel() : model.adjustmentModel));
			//HttpContext.Session.SetString("SaleRejectionModel", JsonConvert.SerializeObject(model));
			//HttpContext.Session.SetString("SaleRejection", JsonConvert.SerializeObject(model));
			return View("SaleRejection", model);
		}
		[HttpPost]
		public async Task<IActionResult> UpdateRejectionItem(List<SaleRejectionDetail> model)
		{
			try
			{
				// 🔹 Get the existing grid data from session
				string modelJson = HttpContext.Session.GetString("KeySaleRejectionGrid");
				var SaleBillDetail = new List<SaleRejectionDetail>();
				if (!string.IsNullOrEmpty(modelJson))
				{
					SaleBillDetail = JsonConvert.DeserializeObject<List<SaleRejectionDetail>>(modelJson);
				}

				// 🔹 Get the main sale rejection model from session
				string SaleBillModelJson = HttpContext.Session.GetString("SaleRejectionModel");
				var saleBillModel = new SaleRejectionModel();
				if (!string.IsNullOrEmpty(SaleBillModelJson))
				{
					saleBillModel = JsonConvert.DeserializeObject<SaleRejectionModel>(SaleBillModelJson);
				}

				if (model != null)
				{
					foreach (var item in model)
					{
						var existing = SaleBillDetail.FirstOrDefault(x =>
							x.ItemCode == item.ItemCode &&
							x.AgainstBillEntryId == item.AgainstBillEntryId &&
							x.AgainstBillYearCode == item.AgainstBillYearCode);

						if (existing != null)
						{
							// ✅ Correctly update properties
							existing.RejRate = item.RejRate;
							existing.Amount = item.Amount;
							existing.ItemNetAmount = item.Amount;
						}
					}

					// ✅ Sort & update the same model object
					SaleBillDetail = SaleBillDetail.OrderBy(x => x.SeqNo).ToList();
					saleBillModel.SaleRejectionDetails = SaleBillDetail;
					saleBillModel.ItemDetailGrid = SaleBillDetail;

					// ✅ Overwrite session ONCE cleanly
					HttpContext.Session.SetString("KeySaleRejectionGrid", JsonConvert.SerializeObject(SaleBillDetail));
					HttpContext.Session.SetString("SaleRejectionModel", JsonConvert.SerializeObject(saleBillModel));

					await HttpContext.Session.CommitAsync();
				}
				else
				{
					ModelState.TryAddModelError("Error", "Sale Rejection List cannot be empty...!");
				}

				return Json(new { success = true });
			}
			catch (Exception ex)
			{
				throw;
			}
		}
		[HttpGet]
		[Route("{controller}/Dashboard")]
		public async Task<IActionResult> SRDashboard(string fromDate, string toDate, string custInvoiceNo, int AccountCode, string mrnNo, string gateNo, int ItemCode, string againstBillNo, string voucherNo, string summaryDetail, string searchBox, string Flag = "True")
		{
			try
			{
				HttpContext.Session.Remove("KeySaleRejectionGrid");
				HttpContext.Session.Remove("KeyTaxGrid");

				int userID = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
				var rights = await _saleRejection.GetFormRights(userID);
				if (rights?.Result == null || rights.Result.Tables.Count == 0 || rights.Result.Tables[0].Rows.Count == 0)
				{
					return RedirectToAction("Dashboard", "Home");
				}
				var table = rights.Result.Tables[0];

				bool optAll = Convert.ToBoolean(table.Rows[0]["OptAll"]);
				bool optView = Convert.ToBoolean(table.Rows[0]["OptView"]);
				bool optUpdate = Convert.ToBoolean(table.Rows[0]["OptUpdate"]);
				bool optDelete = Convert.ToBoolean(table.Rows[0]["OptDelete"]);
				if (!(optAll || optView || optUpdate || optDelete))
				{
					return RedirectToAction("Dashboard", "Home");
				}

				var model = new SaleRejectionDashboard();
				var finFromSession = HttpContext.Session.GetString("FromDate");

				model.FinFromDate = string.IsNullOrEmpty(fromDate)
					? Convert.ToDateTime(finFromSession).ToString("dd/MM/yyyy")
					: ParseFormattedDate(fromDate);

				model.FinToDate = string.IsNullOrEmpty(toDate)
					? DateTime.Today.ToString("dd/MM/yyyy")
					: ParseFormattedDate(toDate);

				model.SaleRejYearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
				model.SummaryDetail = string.IsNullOrEmpty(summaryDetail) ? "Summary" : summaryDetail;

				var result = await _saleRejection.GetDashboardData(model.FinFromDate, model.FinToDate, custInvoiceNo, AccountCode, mrnNo, gateNo, ItemCode, againstBillNo, voucherNo, model.SummaryDetail, searchBox);

				if (result != null && result.Result is DataTable dt)
				{
					model.Headers = dt.Columns
						.Cast<DataColumn>()
						.Select(c => new DashboardColumn
						{
							Title = c.ColumnName,
							Field = c.ColumnName
						})
						.ToList();

					model.Rows = dt.AsEnumerable()
						.Select(r => dt.Columns
							.Cast<DataColumn>()
							.ToDictionary(
								c => c.ColumnName,
								c => r[c] == DBNull.Value ? null : r[c]
							))
						.ToList();
				}

				model.MrnNo = mrnNo;
				model.ItemCode = ItemCode;
				model.GateNo = gateNo;
				model.CustInvoiceNo = custInvoiceNo;
				model.AccountCode = AccountCode;
				model.AgainstBillNo = againstBillNo;
				model.VoucherNo = voucherNo;
				model.SearchBox = searchBox;

				return View(model);
			}
			catch
			{
				throw;
			}
		}
		public async Task<IActionResult> GetSearchData(string fromDate, string toDate, string custInvoiceNo, int AccountCode, string mrnNo, string gateNo, int ItemCode, string againstBillNo, string voucherNo, string summaryDetail, string searchBox, string Flag = "True")
		{
			try
			{
				var model = new SaleRejectionDashboard
				{
					SaleRejYearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode")),
					SummaryDetail = summaryDetail,
					SearchBox = searchBox
				};

				var result = await _saleRejection.GetDashboardData(ParseFormattedDate(fromDate), ParseFormattedDate(toDate), custInvoiceNo, AccountCode, mrnNo, gateNo, ItemCode, againstBillNo, voucherNo, summaryDetail, searchBox);

				if (result == null || !(result.Result is DataTable dt))
				{
					return PartialView("_SRDashboardGrid", model);
				}

				model.Headers = dt.Columns
					.Cast<DataColumn>()
					.Select(c => new DashboardColumn
					{
						Title = c.ColumnName,
						Field = c.ColumnName
					})
					.ToList();

				model.Rows = dt.AsEnumerable()
					.Select(r => dt.Columns
						.Cast<DataColumn>()
						.ToDictionary(
							c => c.ColumnName,
							c => r[c] == DBNull.Value ? null : r[c]
						))
					.ToList();

				return PartialView("_SRDashboardGrid", model);
			}
			catch
			{
				throw;
			}
		}
		private SaleRejectionModel BindItem4Grid(SaleRejectionModel model)
		{
			var _List = new List<DPBItemDetail>();

			string modelJson = HttpContext.Session.GetString("SaleRejectionModel");
			SaleRejectionModel MainModel = new SaleRejectionModel();
			if (!string.IsNullOrEmpty(modelJson))
			{
				MainModel = JsonConvert.DeserializeObject<SaleRejectionModel>(modelJson);
			}

			_List.Add(
				new DPBItemDetail
				{
					SeqNo = MainModel.ItemDetailGrid == null ? 1 : MainModel.ItemDetailGrid.Count + 1,
					docTypeId = 1, //model.docTypeId,
					DocTypeText = string.Empty, //model.DocTypeText,
					BillQty = Convert.ToDecimal(model.RejQty),

					Amount = model.Amount,
					Description = string.Empty, //model.Description,
					DiscPer = Convert.ToDecimal(model.DiscountPer),
					DiscRs = model.DiscountAmt,

					HSNNo = model.HSNNo,
					ItemCode = model.ItemCode,
					ItemText = model.ItemName,

					//OtherRateCurr = Convert.ToDecimal(model.Rate),
					PartCode = model.ItemCode,
					PartText = model.PartCode,

					DPBQty = Convert.ToDecimal(model.RejQty),
					//Process = model.ProcessId,
					//ProcessName = model.ProcessName,
					//CostCenter = model.CostCenterId,
					//CostCenterName = model.CostCenterName,
					Rate = Convert.ToDecimal(model.Rate),

					PONo = string.Empty,//model.pono,
					POYear = 0,//model.POYear,
					PODate = string.Empty,//model.PODate,
										  //ScheduleNo = model.schNo,
										  //ScheduleYear = model.SaleSchYearCode,
										  //ScheduleDate = model.Schdate,

					Unit = model.Unit,
				});

			if (MainModel.DPBItemDetails == null)
				MainModel.DPBItemDetails = _List;
			else
				MainModel.DPBItemDetails.AddRange(_List);

			MainModel.ItemNetAmount = decimal.Parse(MainModel.DPBItemDetails.Sum(x => x.Amount).ToString());

			return MainModel;
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[Route("{controller}/Index")]
		public async Task<IActionResult> SaleRejection(SaleRejectionModel model)
		{
			try
			{
				var SBGrid = new DataTable();
				DataTable TaxDetailDT = null;
				DataTable AdjDetailDT = null;
				DataTable DrCrDetailDT = null;
				string modelJson = HttpContext.Session.GetString("SaleRejectionModel");
				SaleRejectionModel MainModel = new SaleRejectionModel();
				if (!string.IsNullOrEmpty(modelJson))
				{
					MainModel = JsonConvert.DeserializeObject<SaleRejectionModel>(modelJson);
				}
				string saleRejectionModelJson = HttpContext.Session.GetString("KeySaleRejectionGrid");
				List<SaleRejectionDetail> saleRejectionDetail = new List<SaleRejectionDetail>();
				if (!string.IsNullOrEmpty(saleRejectionModelJson))
				{
					saleRejectionDetail = JsonConvert.DeserializeObject<List<SaleRejectionDetail>>(saleRejectionModelJson);
				}
				string taxGridJson = HttpContext.Session.GetString("KeyTaxGrid");
				List<TaxModel> TaxGrid = new List<TaxModel>();
				if (!string.IsNullOrEmpty(taxGridJson))
				{
					TaxGrid = JsonConvert.DeserializeObject<List<TaxModel>>(taxGridJson);
				}
				string drCrGridJson = HttpContext.Session.GetString("KeyDrCrGrid");
				List<DbCrModel> DrCrGrid = new List<DbCrModel>();
				if (!string.IsNullOrEmpty(drCrGridJson))
				{
					DrCrGrid = JsonConvert.DeserializeObject<List<DbCrModel>>(drCrGridJson);
				}

				if (saleRejectionDetail == null)
				{
					ModelState.Clear();
					ModelState.TryAddModelError("SaleRejectionDetail", "Sale Rejection Grid Should Have Atleast 1 Item...!");
					return View("StockADjustment", model);
				}
				else if (saleRejectionDetail == null)
				{
					ModelState.Clear();
					ModelState.TryAddModelError("TaxDetail", "Tax Grid Should Have Atleast 1 Item...!");
					return View("StockADjustment", model);
				}
				else
				{
					model.CC = HttpContext.Session.GetString("Branch");
					//model.ActualEnteredBy   = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
					model.Uid = Convert.ToInt32(HttpContext.Session.GetString("UID"));
					if (model.Mode == "U")
					{
						model.LastUpdatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
						model.LastUpdatedByName = HttpContext.Session.GetString("EmpName");
						SBGrid = GetDetailTable(model.SaleRejectionDetails);
					}
					else
					{
						model.ActualEnteredBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
						model.ActualEnteredByName = HttpContext.Session.GetString("EmpName");
						model.EntryByempId = Convert.ToInt32(HttpContext.Session.GetString("UID"));
						SBGrid = GetDetailTable(model.SaleRejectionDetails);
					}

					if (TaxGrid != null && TaxGrid.Count > 0)
					{
						TaxDetailDT = GetTaxDetailTable(TaxGrid);
					}
					if (DrCrGrid != null && DrCrGrid.Count > 0)
					{
						DrCrDetailDT = CommonController.GetDrCrDetailTable(DrCrGrid);
					}
					string serializedGrid = HttpContext.Session.GetString("KeyAdjGrid");
					var adjustmentModel = new AdjustmentModel();
					if (!string.IsNullOrEmpty(serializedGrid))
					{
						adjustmentModel = JsonConvert.DeserializeObject<AdjustmentModel>(serializedGrid);
						// Use adjustmentModel as needed
					}

					if (adjustmentModel.AdjAdjustmentDetailGrid != null && adjustmentModel.AdjAdjustmentDetailGrid.Count > 0)
					{
						AdjDetailDT = CommonController.GetAdjDetailTable(adjustmentModel.AdjAdjustmentDetailGrid.ToList(), model.SaleRejEntryId, model.SaleRejYearCode, model.AccountCode);
					}

					model.MachineName = HttpContext.Session.GetString("ClientMachineName");
					model.IPAddress = HttpContext.Session.GetString("ClientIP");

					var Result = await _saleRejection.SaveSaleRejection(model, SBGrid, TaxDetailDT, DrCrDetailDT, AdjDetailDT);

					if (Result != null)
					{
						if (Result.StatusText == "Success" && Result.StatusCode == HttpStatusCode.OK)
						{
							ViewBag.isSuccess = true;
							TempData["200"] = "200";
							var model1 = new SaleRejectionModel();
							model1.adjustmentModel = model1.adjustmentModel ?? new AdjustmentModel();

							model1.FinFromDate = HttpContext.Session.GetString("FromDate");
							model1.FinToDate = HttpContext.Session.GetString("ToDate");
							var yearCodeStr = HttpContext.Session.GetString("YearCode");
							model1.SaleRejYearCode = !string.IsNullOrEmpty(yearCodeStr) ? Convert.ToInt32(yearCodeStr) : 0;
							model1.CC = HttpContext.Session.GetString("Branch");
							var uidStr = HttpContext.Session.GetString("UID");
							model1.CreatedBy = !string.IsNullOrEmpty(uidStr) ? Convert.ToInt32(uidStr) : 0;
							//model1.ActualEnteredByName = HttpContext.Session.GetString("EmpName");
							model1.CreatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
							HttpContext.Session.Remove("KeySaleRejectionGrid");
							HttpContext.Session.Remove("SaleRejectionModel");
							//return RedirectToAction("PendingSaleRejection", "PendingSaleRejection", new { id = 0 });
							return Json(new
							{
								success = true,
								message = "Sale Rejection saved successfully",
								redirectUrl = Url.Action(
                                    "Dashboard",
        "SaleRejection"
        
		
	)
							});
						}

						else if (Result.StatusText == "Updated" && Result.StatusCode == HttpStatusCode.Accepted)
						{
							ViewBag.isSuccess = true;
							TempData["202"] = "202";
							var model1 = new SaleBillModel();
							model1.adjustmentModel = new AdjustmentModel();
							model1.adjustmentModel = model.adjustmentModel ?? new AdjustmentModel();
							model1.FinFromDate = HttpContext.Session.GetString("FromDate");
							model1.FinToDate = HttpContext.Session.GetString("ToDate");
							model1.SaleBillYearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
							model1.CC = HttpContext.Session.GetString("Branch");
							//model1.ActualEnteredByName = HttpContext.Session.GetString("EmpName");
							model1.CreatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
							HttpContext.Session.Remove("KeySaleRejectionGrid");
							HttpContext.Session.Remove("SaleRejectionModel");
							//return RedirectToAction("PendingSaleRejection", "PendingSaleRejection", new { id = 0 });

							return Json(new
							{
								success = true,
								message = "Sale Rejection Updated successfully",
								redirectUrl = Url.Action(
        "Dashboard",
        "SaleRejection"
    )
							});
						}

						else if (Result.StatusText == "Error" && Result.StatusCode == HttpStatusCode.InternalServerError)
						{
							var errNum = Result.Result.Message.ToString().Split(":")[1];
							model.adjustmentModel = model.adjustmentModel ?? new AdjustmentModel();
							if (errNum == " 2627")
							{
								ViewBag.isSuccess = false;
								TempData["2627"] = "2627";
								_logger.LogError("\n \n ********** LogError ********** \n " + JsonConvert.SerializeObject(Result) + "\n \n");

								return View(model);
							}

							ViewBag.isSuccess = false;
							TempData["500"] = "500";
							_logger.LogError("\n \n ********** LogError ********** \n " + JsonConvert.SerializeObject(Result) + "\n \n");
							// return View("Error", Result);
							//return View(model);
							return Json(new
							{
								success = false,
								message = Result.StatusText
							});
						}
						else if (!string.IsNullOrEmpty(Result.StatusText))
						{
							//// If SP returned a message (like adjustment error)
							//TempData["ErrorMessage"] = Result.StatusText;
							//return View(model);

							return Json(new
							{
								success = false,
								message = Result.StatusText
							});
						}
						else
						{
							TempData["ErrorMessage"] = "Error while deleting transaction.";
						}

						HttpContext.Session.SetString("SaleRejection", JsonConvert.SerializeObject(model));
						HttpContext.Session.Remove("KeySaleRejectionGrid");
						HttpContext.Session.Remove("SaleRejectionModel");
						//return RedirectToAction("PendingSaleRejection", "PendingSaleRejection", new { id = 0 });
					}
					//return View(model);
					return Json(new
					{
						success = false,
						message = "An unexpected error occurred."
					});

				}
			}
			catch (Exception ex)
			{
				//  _logger.LogException<SaleRejectionController>.WriteException(_logger, ex);

				var _ResponseResult = new ResponseResult()
				{
					StatusCode = HttpStatusCode.InternalServerError,
					StatusText = "Error",
					Result = ex
				};

				return Json(new
				{
					success = false,
					message = _ResponseResult
				});

				//return View("Error", _ResponseResult);
				//return View(model);


			}
		}
		public async Task<IActionResult> DeleteByID(int ID, int YearCode, int accountCode, int createdBy, string machineName, string cc, string fromDate, string toDate, string custInvoiceNo, string accountName, string mrnNo, string gateNo, string partCode, string itemName, string againstBillNo, string voucherNo)
		{
			var Result = await _saleRejection.DeleteByID(ID, YearCode, accountCode, createdBy, machineName, cc).ConfigureAwait(false);

			if (Result.StatusText == "Success" || Result.StatusText == "deleted" || Result.StatusCode == HttpStatusCode.Gone)
			{
				ViewBag.isSuccess = true;
				TempData["410"] = "410";
			}
			else if (Result.StatusText == "Error" || Result.StatusCode == HttpStatusCode.Accepted)
			{
				ViewBag.isSuccess = true;
				TempData["423"] = "423";
				TempData["DeleteMessage"] = Result.StatusText;

			}
			else
			{
				ViewBag.isSuccess = false;
				TempData["500"] = "500";

			}
			return RedirectToAction("SRDashboard", new { fromDate = fromDate, toDate = toDate, custInvoiceNo = custInvoiceNo, accountName = accountName, mrnNo = mrnNo, gateNo = gateNo, partCode = partCode, itemName = itemName, againstBillNo = againstBillNo, voucherNo = voucherNo });
		}
		private static DataTable GetTaxDetailTable(List<TaxModel> TaxDetailList)
		{
			DataTable Table = new();
			Table.Columns.Add("TxSeqNo", typeof(int));
			Table.Columns.Add("TxType", typeof(string));
			Table.Columns.Add("TxItemCode", typeof(int));
			Table.Columns.Add("TxTaxType", typeof(int));
			Table.Columns.Add("TxAccountCode", typeof(int));
			Table.Columns.Add("TxPercentg", typeof(float));
			Table.Columns.Add("TxAdInTxable", typeof(string));
			Table.Columns.Add("TxRoundOff", typeof(string));
			Table.Columns.Add("TxAmount", typeof(float));
			Table.Columns.Add("TxRefundable", typeof(string));
			Table.Columns.Add("TxOnExp", typeof(float));
			Table.Columns.Add("TxRemark", typeof(string));

			foreach (TaxModel Item in TaxDetailList)
			{
				Table.Rows.Add(
					new object[]
					{
					Item.TxSeqNo,
					Item.TxType,
					Item.TxItemCode,
					Item.TxTaxType,
					Item.TxAccountCode,
					Item.TxPercentg,
					Item.TxAdInTxable,
					Item.TxRoundOff,
					Item.TxAmount,
					Item.TxRefundable,
					Item.TxOnExp,
					Item.TxRemark,
					});
			}

			return Table;
		}
		private static DataTable GetDetailTable(IList<SaleRejectionDetail> DetailList)
		{
			var DTSSGrid = new DataTable();

			DTSSGrid.Columns.Add("SaleRejEntryId", typeof(int));
			DTSSGrid.Columns.Add("SaleRejYearCode", typeof(int));
			DTSSGrid.Columns.Add("AccountCode", typeof(int));
			DTSSGrid.Columns.Add("AgainstBillTypeJWSALE", typeof(string));
			DTSSGrid.Columns.Add("AgainstBillNo", typeof(string));
			DTSSGrid.Columns.Add("AgainstBillYearCode", typeof(int));
			DTSSGrid.Columns.Add("AgainstBillEntryId", typeof(int));
			DTSSGrid.Columns.Add("AgainstOpenBillEntryId", typeof(int));
			DTSSGrid.Columns.Add("AgainstOpenOrBill", typeof(string));
			DTSSGrid.Columns.Add("AgainstOpenBillYearCode", typeof(int));
			DTSSGrid.Columns.Add("DocTypeAccountCode", typeof(int));
			DTSSGrid.Columns.Add("ItemCode", typeof(int));
			DTSSGrid.Columns.Add("Unit", typeof(string));
			DTSSGrid.Columns.Add("HSNNo", typeof(int));
			DTSSGrid.Columns.Add("NoOfCase", typeof(float));
			DTSSGrid.Columns.Add("SaleBillQty", typeof(float));
			DTSSGrid.Columns.Add("RejQty", typeof(float));
			DTSSGrid.Columns.Add("MRNRecQty", typeof(float));
			DTSSGrid.Columns.Add("RejRate", typeof(float));
			DTSSGrid.Columns.Add("SaleBillRate", typeof(float));
			DTSSGrid.Columns.Add("DiscountPer", typeof(float));
			DTSSGrid.Columns.Add("DiscountAmt", typeof(float));
			DTSSGrid.Columns.Add("SONO", typeof(string));
			DTSSGrid.Columns.Add("SOyearcode", typeof(int));
			DTSSGrid.Columns.Add("SODate", typeof(string));
			DTSSGrid.Columns.Add("CustOrderNo", typeof(string));
			DTSSGrid.Columns.Add("SOAmmNo", typeof(string));
			DTSSGrid.Columns.Add("Itemsize", typeof(string));
			DTSSGrid.Columns.Add("RecStoreId", typeof(int));
			DTSSGrid.Columns.Add("OtherDetail", typeof(string));
			DTSSGrid.Columns.Add("Amount", typeof(float));
			DTSSGrid.Columns.Add("RejectionReason", typeof(string));
			DTSSGrid.Columns.Add("SaleorderRemark", typeof(string));
			DTSSGrid.Columns.Add("SaleBillRemark", typeof(string));
			DTSSGrid.Columns.Add("ItemCGSTPer", typeof(decimal));
			DTSSGrid.Columns.Add("ItemSGSTPer", typeof(decimal));
			DTSSGrid.Columns.Add("ItemIGSTPer", typeof(decimal));
            DTSSGrid.Columns.Add("ItemCGSTAmt", typeof(decimal));
            DTSSGrid.Columns.Add("ItemSGSTAmt", typeof(decimal));
            DTSSGrid.Columns.Add("ItemIGSTAmt", typeof(decimal));
            DTSSGrid.Columns.Add("ItemExpense", typeof(decimal));

			//DateTime DeliveryDt = new DateTime();
			foreach (var Item in DetailList)
			{
				string uniqueString = Guid.NewGuid().ToString();
				DTSSGrid.Rows.Add(
					new object[]
					{
					1,
					2024,
					1,//Item.AccountCode,
                    Item.AgainstBillTypeJWSALE ?? string.Empty,
					Item.AgainstBillNo ?? string.Empty,
					Item.AgainstBillYearCode,
					Item.AgainstBillEntryId,
					1,//Item.AgainstOpenEntryId,
                    Item.AgainstOpnOrBill ?? string.Empty,
					Item.AgainstBillYearCode, //openBillYearCode
                    0,
					Item.ItemCode,
					Item.Unit ?? string.Empty,
					Item.HSNNo,
					Item.NoOfCase,
					Item.SaleBillQty,
					Item.RejQty,
					Item.RecQty, //MRNRecQty
                    Item.RejRate, //RejRate
                    Item.Rate, //SaleBillRate
                    Item.DiscountPer,
					Item.DiscountAmt,
					Item.SONO ?? string.Empty,
					Item.SOyearcode,
					Item.SODate == null ? string.Empty : ParseFormattedDate(Item.SODate.Split(" ")[0]) ,
					Item.CustOrderNo ?? string.Empty,
					Item.SOAmmNo ?? string.Empty,
					Item.Itemsize ?? string.Empty,
					Item.RecStoreId,
					Item.OtherDetail ?? string.Empty,
					Item.Amount,
					Item.RejectionReason ?? string.Empty,
					Item.SaleorderRemark ?? string.Empty,
					Item.SaleBillremark ?? string.Empty,
					Item.CGSTPer,
					Item.SGSTPer,
					Item.IGSTPer,
					Item.CGSTAmt,
					Item.SGSTAmt,
					Item.IGSTAmt,
                    Item.Amount//ItemExpense
                  });
			}
			DTSSGrid.Dispose();
			return DTSSGrid;
		}
		public async Task<JsonResult> NewEntryId(int YearCode, string SaleRejEntryDate, string SubVoucherName)
		{
			var JSON = await _saleRejection.NewEntryId(YearCode, SaleRejEntryDate, SubVoucherName);
			string JsonString = JsonConvert.SerializeObject(JSON);
			return Json(JsonString);
		}
		public async Task<JsonResult> FillSaleRejectionGrid(string mrnNo, int mrnEntryId, int mrnYC, int yearCode)
		{
			var JSON = await _saleRejection.FillSaleRejectionGrid(mrnNo, mrnEntryId, mrnYC, yearCode);
			string JsonString = JsonConvert.SerializeObject(JSON);
			return Json(JsonString);
		}
		public async Task<JsonResult> GetTotalAmount(SaleRejectionFilter model)
		{
			var JSON = await _saleRejection.GetTotalAmount(model);
			string JsonString = JsonConvert.SerializeObject(JSON);
			return Json(JsonString);
		}
		public async Task<JsonResult> FillCustomerName(string fromDate, string toDate)
		{
			fromDate = ParseFormattedDate(fromDate);
			toDate = ParseFormattedDate(toDate);
			var JSON = await _saleRejection.FillCustomerName(fromDate, toDate);
			string JsonString = JsonConvert.SerializeObject(JSON);
			return Json(JsonString);
		}
		public async Task<JsonResult> FillItemName(string fromDate, string toDate)
		{
			fromDate = ParseFormattedDate(fromDate);
			toDate = ParseFormattedDate(toDate);
			var JSON = await _saleRejection.FillItemName(fromDate, toDate);
			string JsonString = JsonConvert.SerializeObject(JSON);
			return Json(JsonString);
		}
		public async Task<JsonResult> FillPartCode(string fromDate, string toDate)
		{
			fromDate = ParseFormattedDate(fromDate);
			toDate = ParseFormattedDate(toDate);
			var JSON = await _saleRejection.FillPartCode(fromDate, toDate);
			string JsonString = JsonConvert.SerializeObject(JSON);
			return Json(JsonString);
		}
		public async Task<JsonResult> FillMRNNo(string fromDate, string toDate)
		{
			fromDate = ParseFormattedDate(fromDate);
			toDate = ParseFormattedDate(toDate);
			var JSON = await _saleRejection.FillMRNNo(fromDate, toDate);
			string JsonString = JsonConvert.SerializeObject(JSON);
			return Json(JsonString);
		}
		public async Task<JsonResult> FillGateNo(string fromDate, string toDate)
		{
			fromDate = ParseFormattedDate(fromDate);
			toDate = ParseFormattedDate(toDate);
			var JSON = await _saleRejection.FillGateNo(fromDate, toDate);
			string JsonString = JsonConvert.SerializeObject(JSON);
			return Json(JsonString);
		}
		public async Task<JsonResult> FillInvoiceNo(string fromDate, string toDate)
		{
			fromDate = ParseFormattedDate(fromDate);
			toDate = ParseFormattedDate(toDate);
			var JSON = await _saleRejection.FillInvoiceNo(fromDate, toDate);
			string JsonString = JsonConvert.SerializeObject(JSON);
			return Json(JsonString);
		}
		public async Task<JsonResult> FillVoucherNo(string fromDate, string toDate)
		{
			fromDate = ParseFormattedDate(fromDate);
			toDate = ParseFormattedDate(toDate);
			var JSON = await _saleRejection.FillVoucherNo(fromDate, toDate);
			string JsonString = JsonConvert.SerializeObject(JSON);
			return Json(JsonString);
		}
		public async Task<JsonResult> FillDocument(string fromDate, string toDate)
		{
			fromDate = ParseFormattedDate(fromDate);
			toDate = ParseFormattedDate(toDate);
			var JSON = await _saleRejection.FillDocument(fromDate, toDate);
			string JsonString = JsonConvert.SerializeObject(JSON);
			return Json(JsonString);
		}
		public async Task<JsonResult> FillAgainstSaleBillNo(string fromDate, string toDate)
		{
			fromDate = ParseFormattedDate(fromDate);
			toDate = ParseFormattedDate(toDate);
			var JSON = await _saleRejection.FillAgainstSaleBillNo(fromDate, toDate);
			string JsonString = JsonConvert.SerializeObject(JSON);
			return Json(JsonString);
		}

		public async Task<JsonResult> FillSubvoucher()
		{
			var JSON = await _saleRejection.FillSubvoucher();
			string JsonString = JsonConvert.SerializeObject(JSON);
			return Json(JsonString);
		}

        //[HttpPost]
        //public async Task<IActionResult> LoadSaleBillData(string saleBillNo)
        //{
        //    SaleRejectionModel model = new SaleRejectionModel();

        //    var data = await _saleRejection.GetSaleBillData(saleBillNo);

        //    model.SaleRejectionDetails = data;

        //    return PartialView("_AddSaleRejectionGrid", model);
        //}
       

        [HttpPost]
        public async Task<IActionResult> LoadSaleBillData(string saleBillNo, int AccountCode)
        {
            // Get new data from BLL
            var newData = await _saleRejection.GetSaleBillData(saleBillNo, AccountCode);

            // Get existing session data
            string saleRejectionModelJson = HttpContext.Session.GetString("KeySaleRejectionGrid");
            List<SaleRejectionDetail> saleRejectionDetail = new List<SaleRejectionDetail>();
            if (!string.IsNullOrEmpty(saleRejectionModelJson) && saleRejectionModelJson != "null")
            {
                saleRejectionDetail = JsonConvert.DeserializeObject<List<SaleRejectionDetail>>(saleRejectionModelJson);
            }
            // Append new data
            saleRejectionDetail.AddRange(newData);


            // Save back to session
            HttpContext.Session.SetString("KeySaleRejectionGrid", JsonConvert.SerializeObject(saleRejectionDetail));
            
            SaleRejectionModel model = new SaleRejectionModel
            {
                SaleRejectionDetails = saleRejectionDetail
            };
            HttpContext.Session.SetString("SaleRejectionModel", JsonConvert.SerializeObject(model));


            return PartialView("_AddSaleRejectionGrid", model);
        }
        public async Task<JsonResult> GETSaleBillNo(int AccountCode, string SaleBillNo)
        {
            var JSON = await _saleRejection.GETSaleBillNo(AccountCode, SaleBillNo);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateSaleRejectionSession( SaleRejectionModel Model)
        {
            if (Model != null)
            {
                HttpContext.Session.SetString(
                    "KeySaleRejectionGrid",
                    JsonConvert.SerializeObject(Model.SaleRejectionDetails)
                );
                HttpContext.Session.SetString("SaleRejectionModel", JsonConvert.SerializeObject(Model));

            }

            return Json(new { success = true });
        }


    }

   
}
