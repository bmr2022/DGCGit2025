using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Newtonsoft.Json;
using static eTactWeb.Data.Common.CommonFunc;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Controllers
{
    public class PartyItemGroupDiscountController : Controller
    {
        private readonly IDataLogic _IDataLogic;
        public IPartyItemGroupDiscount _IPartyItemGroupDiscount { get; }
        private readonly ILogger<PartyItemGroupDiscountController> _logger;
        private readonly IConfiguration iconfiguration;

        public IWebHostEnvironment _IWebHostEnvironment { get; }
        public PartyItemGroupDiscountController(ILogger<PartyItemGroupDiscountController> logger, IPartyItemGroupDiscount iPartyItemGroupDiscount, IDataLogic iDataLogic,  EncryptDecrypt encryptDecrypt, IWebHostEnvironment iWebHostEnvironment, IConfiguration iconfiguration)
        {
            _logger = logger;
            _IDataLogic = iDataLogic;
            _IWebHostEnvironment = iWebHostEnvironment;
            this.iconfiguration = iconfiguration;
            _IPartyItemGroupDiscount = iPartyItemGroupDiscount;

		}
        [Route("{controller}/Index")]
        public async Task<ActionResult> PartyItemGroupDiscount(int ID, int YC, string Mode, string AccountName,
		string CategoryName,string GroupName,string GroupCode,int PartyWIseGrpDiscEntryId,int AccountCode,
		string ActualEntryByEmpName,string EntryByMachine,string CC, int CategoryId,string CategoryCode,decimal PurchaseDiscount,decimal SaleDiscount)
        {
            var MainModel = new PartyItemGroupDiscountModel();

            MainModel.YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));

            MainModel.ActualEntryByEmpId = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
            MainModel.ApprovedByEmpId = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
            MainModel.ApprovedByEmpName = HttpContext.Session.GetString("EmpName");
            MainModel.ActualEntryByEmpName = HttpContext.Session.GetString("EmpName");
            MainModel.CC = HttpContext.Session.GetString("Branch");
            HttpContext.Session.Remove("KeyPartyItemGroupDiscountGrid");
			//bool partyExists = await _IPartyItemGroupDiscount.CheckPartyExists(AccountCode,ID);
			if (!string.IsNullOrEmpty(Mode) && ID > 0 && (Mode == "U" || Mode == "V"))
			{
				MainModel = await _IPartyItemGroupDiscount.GetViewByID(ID).ConfigureAwait(false);
				MainModel.Mode = Mode; // Set Mode to Update
                MainModel.EntryId = ID;
                MainModel.YearCode = YC;
                MainModel.AccountName = AccountName;
                MainModel.CategoryName = CategoryName;
                MainModel.CategoryId = CategoryId;
                MainModel.CategoryCode = CategoryCode;
                MainModel.GroupName = GroupName;
                MainModel.GroupCode = GroupCode;
                MainModel.PartyWIseGrpDiscEntryId = PartyWIseGrpDiscEntryId;
                MainModel.AccountCode = AccountCode;
                MainModel.ActualEntryByEmpName = ActualEntryByEmpName;
                MainModel.EntryByMachine = EntryByMachine;
                MainModel.CC = CC;
                MainModel.PurchaseDiscount = PurchaseDiscount;
                MainModel.SaleDiscount = SaleDiscount;

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

                string serializedGrid = JsonConvert.SerializeObject(MainModel.PartyItemGroupDiscountGrid);
                HttpContext.Session.SetString("KeyPartyItemGroupDiscountGrid", serializedGrid);
            }

            return View(MainModel);
        }
		[Route("{controller}/Index")]
		[HttpPost]

		public async Task<IActionResult> PartyItemGroupDiscount(PartyItemGroupDiscountModel model)
		{
			try
			{
				//string modelJson = HttpContext.Session.GetString("KeyPartyItemGroupDiscountGrid");
				//List<PartyItemGroupDiscountModel> PartyItemGroupDiscountDetail = new List<PartyItemGroupDiscountModel>();
			
				//HttpContext.Session.SetString("KeyPartyItemGroupDiscountGrid", JsonConvert.SerializeObject(PartyItemGroupDiscountDetail));

				//var GIGrid = GetDetailTable(PartyItemGroupDiscountDetail);

				

				string modelJson = HttpContext.Session.GetString("KeyPartyItemGroupDiscountGrid");
				List<PartyItemGroupDiscountModel> PartyItemGroupDiscountDetail = new List<PartyItemGroupDiscountModel>();

				if (!string.IsNullOrEmpty(modelJson))
				{
					PartyItemGroupDiscountDetail = JsonConvert.DeserializeObject<List<PartyItemGroupDiscountModel>>(modelJson);
				}

				// Now use this list to build DataTable
				model.EntryBy = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
				var GIGrid = GetDetailTable(PartyItemGroupDiscountDetail);
				var Result = await _IPartyItemGroupDiscount.SavePartyItemGroupDiscount(model, GIGrid);

				if (Result != null)
				{
					if (Result.StatusText == "Success" && Result.StatusCode == HttpStatusCode.OK)
					{
						ViewBag.isSuccess = true;
						TempData["200"] = "200";
						HttpContext.Session.Remove("KeyPartyItemGroupDiscountGrid");
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

				return RedirectToAction(nameof(PartyItemGroupDiscountDashBoard));
			}
			catch (Exception ex)
			{
				LogException<PartyItemGroupDiscountController>.WriteException(_logger, ex);
				var ResponseResult = new ResponseResult
				{
					StatusCode = HttpStatusCode.InternalServerError,
					StatusText = "Error",
					Result = ex
				};
				return View("Error", ResponseResult);
			}
		}
		private static DataTable GetDetailTable(IList<PartyItemGroupDiscountModel> DetailList)
		{
			try
			{
				var GIGrid = new DataTable();

				GIGrid.Columns.Add("SeqNo", typeof(long));
				GIGrid.Columns.Add("DiscCategoryEntryId", typeof(long));
				GIGrid.Columns.Add("GroupId", typeof(long));
				GIGrid.Columns.Add("SaleDiscount", typeof(decimal));
				GIGrid.Columns.Add("PurchaseDiscount", typeof(decimal));
				
				foreach (var Item in DetailList)
				{
					GIGrid.Rows.Add(
						new object[]
						{
					Item.SeqNo,
					Item.CategoryId,
					Item.GroupCode,
					Item.SaleDiscount ,
					Item.PurchaseDiscount ,
					
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
		public async Task<JsonResult> FillPartyName()
		{
			var JSON = await _IPartyItemGroupDiscount.FillPartyName();
			string JsonString = JsonConvert.SerializeObject(JSON);
			return Json(JsonString);
		}
        public async Task<JsonResult> FillCategoryName()
		{
			var JSON = await _IPartyItemGroupDiscount.FillCategoryName();
			string JsonString = JsonConvert.SerializeObject(JSON);
			return Json(JsonString);
		}
        public async Task<JsonResult> FillCategoryCode()
		{
			var JSON = await _IPartyItemGroupDiscount.FillCategoryCode();
			string JsonString = JsonConvert.SerializeObject(JSON);
			return Json(JsonString);
		}
        public async Task<JsonResult> FillGroupName()
		{
			var JSON = await _IPartyItemGroupDiscount.FillGroupName();
			string JsonString = JsonConvert.SerializeObject(JSON);
			return Json(JsonString);
		}
        public async Task<JsonResult> FillGroupCode()
		{
			var JSON = await _IPartyItemGroupDiscount.FillGroupCode();
			string JsonString = JsonConvert.SerializeObject(JSON);
			return Json(JsonString);
		}
		public async Task<JsonResult> FillEntryID(string EntryDate)
		{
			var JSON = await _IPartyItemGroupDiscount.FillEntryID(EntryDate);
			string JsonString = JsonConvert.SerializeObject(JSON);
			return Json(JsonString);
		}
		public async Task<JsonResult> FillPartyNameDashBoard()
		{
			var JSON = await _IPartyItemGroupDiscount.FillPartyNameDashBoard();
			string JsonString = JsonConvert.SerializeObject(JSON);
			return Json(JsonString);
		}
        public async Task<JsonResult> FillCategoryNameDashBoard()
		{
			var JSON = await _IPartyItemGroupDiscount.FillCategoryNameDashBoard();
			string JsonString = JsonConvert.SerializeObject(JSON);
			return Json(JsonString);
		}
        public async Task<JsonResult> FillCategoryCodeDashBoard()
		{
			var JSON = await _IPartyItemGroupDiscount.FillCategoryCodeDashBoard();
			string JsonString = JsonConvert.SerializeObject(JSON);
			return Json(JsonString);
		}
        public async Task<JsonResult> FillGroupNameDashBoard()
		{
			var JSON = await _IPartyItemGroupDiscount.FillGroupNameDashBoard();
			string JsonString = JsonConvert.SerializeObject(JSON);
			return Json(JsonString);
		}
        public async Task<JsonResult> FillGroupCodeDashBoard()
		{
			var JSON = await _IPartyItemGroupDiscount.FillGroupCodeDashBoard();
			string JsonString = JsonConvert.SerializeObject(JSON);
			return Json(JsonString);
		}
	

		public IActionResult AddPartyItemGroupDiscountDetail(PartyItemGroupDiscountModel model)
		{
			try
			{
				string jsonString = HttpContext.Session.GetString("KeyPartyItemGroupDiscountGrid");
				IList<PartyItemGroupDiscountModel> GridDetail = new List<PartyItemGroupDiscountModel>();

				if (!string.IsNullOrEmpty(jsonString))
				{
					GridDetail = JsonConvert.DeserializeObject<List<PartyItemGroupDiscountModel>>(jsonString);
				}

				var MainModel = new PartyItemGroupDiscountModel();

				if (model != null)
				{
					
					bool isDuplicate = GridDetail.Any(x =>
						x.SeqNo == model.SeqNo);

					if (isDuplicate)
						return StatusCode(207, "Duplicate");

					if (model.Mode == "U")
					{
						GridDetail.Add(model);
					}
					else
					{
						//int nextSeqNo = GridDetail.Count > 0 ? GridDetail.Max(x => x.SeqNo) + 1 : 1;
						int nextSeqNo = Enumerable.Range(1, GridDetail.Count + 1)
							   .Except(GridDetail.Select(x => x.SeqNo))
							   .First();
						model.SeqNo = nextSeqNo;

						GridDetail.Add(model);
					}


					MainModel.PartyItemGroupDiscountGrid = GridDetail.OrderBy(x => x.SeqNo).ToList();
					HttpContext.Session.SetString("KeyPartyItemGroupDiscountGrid", JsonConvert.SerializeObject(MainModel.PartyItemGroupDiscountGrid));
				}

				return PartialView("_PartyItemGroupDiscountGrid", MainModel);
			}
			catch (Exception)
			{
				throw;
			}
		}
			public async Task<JsonResult> EditItemRows(int SeqNo)
			 {
				var MainModel = new PartyItemGroupDiscountModel();
				string jsonString = HttpContext.Session.GetString("KeyPartyItemGroupDiscountGrid");
				IList<PartyItemGroupDiscountModel> GridDetail = new List<PartyItemGroupDiscountModel>();

				if (!string.IsNullOrEmpty(jsonString))
				{
					GridDetail = JsonConvert.DeserializeObject<List<PartyItemGroupDiscountModel>>(jsonString);
				}

				var result = GridDetail.Where(x => x.SeqNo == SeqNo).ToList();
				string JsonString = JsonConvert.SerializeObject(result);
				return Json(JsonString);
			}


			public IActionResult DeleteItemRow(int SeqNo)
			{
				var MainModel = new PartyItemGroupDiscountModel();
				string jsonString = HttpContext.Session.GetString("KeyPartyItemGroupDiscountGrid");
				IList<PartyItemGroupDiscountModel> ControlPlanDetail = new List<PartyItemGroupDiscountModel>();

				if (!string.IsNullOrEmpty(jsonString))
				{
					ControlPlanDetail = JsonConvert.DeserializeObject<List<PartyItemGroupDiscountModel>>(jsonString);
				}

				if (ControlPlanDetail != null && ControlPlanDetail.Count > 0)
				{
					var itemToRemove = ControlPlanDetail.FirstOrDefault(x => x.SeqNo == SeqNo);
					if (itemToRemove != null)
						ControlPlanDetail.Remove(itemToRemove);

					MainModel.PartyItemGroupDiscountGrid = ControlPlanDetail.OrderBy(x => x.SeqNo).ToList();
					HttpContext.Session.SetString("KeyPartyItemGroupDiscountGrid", JsonConvert.SerializeObject(MainModel.PartyItemGroupDiscountGrid));
				}

				return PartialView("_PartyItemGroupDiscountGrid", MainModel);
			}
        public async Task<IActionResult> PartyItemGroupDiscountDashBoard(string ReportType, string FromDate, string ToDate)
        {
            var model = new PartyItemGroupDiscountModel();
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
            //DateTime now = DateTime.Now;
            //DateTime firstDayOfMonth = new DateTime(yearCode, now.Month, 1);
            //model.FromDate = new DateTime(yearCode, now.Month, 1).ToString("dd/MM/yyyy").Replace("-", "/");
            //model.ToDate = new DateTime(yearCode + 1, 3, 31).ToString("dd/MM/yyyy").Replace("-", "/");
            model.ActualEntryByEmpId = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
            model.ReportType = "SUMMARY";
            var Result = await _IPartyItemGroupDiscount.GetDashboardData(model);

            if (Result.Result != null)
            {
                var _List = new List<TextValue>();
                DataSet DS = Result.Result;
                if (DS != null && DS.Tables.Count > 0)
                {
                    var dt = DS.Tables[0];
                    model.PartyItemGroupDiscountGrid = CommonFunc.DataTableToList<PartyItemGroupDiscountModel>(dt, "PartyItemGroupDiscountDashBoard");
                }

            }

            return View(model);
        }
        public async Task<IActionResult> GetDetailData(string FromDate, string ToDate, string ReportType, int AccountCode, int CategoryId, string GroupName)
        {
            //model.Mode = "Search";
            var model = new PartyItemGroupDiscountModel();
            model = await _IPartyItemGroupDiscount.GetDashboardDetailData(FromDate, ToDate, ReportType,  AccountCode,  CategoryId,  GroupName);
			if (ReportType == "SUMMARY")
			{
                return PartialView("_PartyItemGroupDiscountDashBoardSummaryGrid", model);
            }
			if (ReportType == "DETAIL")
			{
                return PartialView("_PartyItemGroupDiscountDashBoardDetailGrid", model);
            }


			return null;

        }
        public async Task<IActionResult> DeleteByID(int EntryId, int AccountCode, string EntryDate)
        {
            var Result = await _IPartyItemGroupDiscount.DeleteByID(EntryId, AccountCode, EntryDate);

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

            return RedirectToAction("PartyItemGroupDiscountDashBoard");

        }
		public async Task<JsonResult> CheckParty(int AccountCode)
		{
			var result = await _IPartyItemGroupDiscount.CheckPartyExists(AccountCode);
			return Json(new
			{
				exists = result.Exists,
				entryId = result.EntryId,
				accountCode = result.AccountCode,
				accountName = result.AccountName,
				categoryCode = result.CategoryCode,
				categoryName = result.CategoryName,
				categoryId = result.CategoryId
			});
		}


	}
}
