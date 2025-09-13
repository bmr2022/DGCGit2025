using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Wordprocessing;
using eTactWeb.Data.BLL;
using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using static eTactWeb.Data.Common.CommonFunc;
using static eTactWeb.DOM.Models.Common;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;

namespace eTactWeb.Controllers
{
    public class SalesPersonTransferController : Controller
    {
        private readonly IDataLogic _IDataLogic;
        public ISalesPersonTransfer _ISalesPersonTransfer { get; }
        private readonly ILogger<SalesPersonTransferController> _logger;
        private readonly IConfiguration iconfiguration;
        public IWebHostEnvironment _IWebHostEnvironment { get; }
        public SalesPersonTransferController(ILogger<SalesPersonTransferController> logger, IDataLogic iDataLogic, ISalesPersonTransfer iSalesPersonTransfer, EncryptDecrypt encryptDecrypt, IWebHostEnvironment iWebHostEnvironment, IConfiguration iconfiguration)
        {
            _logger = logger;
            _IDataLogic = iDataLogic;
            _ISalesPersonTransfer = iSalesPersonTransfer;
            _IWebHostEnvironment = iWebHostEnvironment;
            this.iconfiguration = iconfiguration;
        }
        [Route("{controller}/Index")]
        public async Task<ActionResult> SalesPersonTransfer(int ID, int YC, string Mode,string FromDate,string ToDate
        , string SlipNo, string NewSalesEmpName, string NewSalesCode, string Designation, string Department, string OldSalesEmpName, string OldSalesCode, string EntryByMachine, string CC,
        int RevNo, int NewSalesEmpId, int OldSalesEmpId,int AccountCode,int EntryByEmpId,int UpdatedByEmpId,
        string ShowAllEmp, string ShowAllOldEmp,string EntryDate, string EffFrom, string EffTillDate)
        {
            var MainModel = new SalesPersonTransferModel();

            MainModel.SalesPersTransfYearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
            MainModel.CreatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));

            MainModel.EntryByEmpId = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
            MainModel.ApprovedBy = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
            MainModel.ApprovedByEmpName = HttpContext.Session.GetString("EmpName");
            MainModel.EntryBYEmpName = HttpContext.Session.GetString("EmpName");
            MainModel.CC = HttpContext.Session.GetString("Branch");
            HttpContext.Session.Remove("KeySalesPersonTransferGrid");
            if (!string.IsNullOrEmpty(Mode) && ID > 0 && Mode == "U")
            {
                MainModel = await _ISalesPersonTransfer.GetViewByID(ID, YC, FromDate, ToDate).ConfigureAwait(false);
                var fullCustomerList = await _ISalesPersonTransfer.FillCustomerList("N");
                ViewBag.FullCustomerList = fullCustomerList.SalesPersonTransferGrid;
                ViewBag.SelectedCustCodes = _ISalesPersonTransfer.GetSelectedCustomerCodes(ID);
                //ViewBag.SelectedCustCodes = AccountCode;
                MainModel.Mode = Mode;
                MainModel.SalesPersTransfEntryId = ID;
                MainModel.SalesPersTransfYearCode = YC;
                MainModel.SalesPersTransfEntryDate = EntryDate;
                MainModel.SalesPersTransfSlipNo = SlipNo;
                MainModel.RevNo = RevNo;
                MainModel.ShowAllEmp = ShowAllEmp;
                MainModel.NewSalesEmpName = NewSalesEmpName;
                MainModel.NewSalesCode = NewSalesCode;
                MainModel.NewSalesEmpId = NewSalesEmpId;
                MainModel.EffFrom = EffFrom;
                MainModel.Designation = Designation;
                MainModel.Department = Department;
                MainModel.ShowAllOldEmp = ShowAllOldEmp;
                MainModel.OldSalesEmpName = OldSalesEmpName;
                MainModel.OldSalesEmpId = OldSalesEmpId;
                MainModel.OldSalesCode = OldSalesCode;
                MainModel.EffTillDate = EffTillDate;
                MainModel.AccountCode = AccountCode;
                MainModel.EntryByEmpId = EntryByEmpId;

                if (Mode == "U")
                {
                    MainModel.UpdatedByEmpId = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                    MainModel.UpdatedByEmpName = HttpContext.Session.GetString("EmpName");
					
					MainModel.UpdationDate = DateTime.Today.ToString("MM/dd/yyyy").Replace("-", "/");
                   
                    MainModel.EntryByEmpId = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
                    MainModel.SalesPersTransfEntryDate = DateTime.Today.ToString("MM/dd/yyyy").Replace("-", "/");
                    MainModel.CC = HttpContext.Session.GetString("Branch");
                }

                string serializedGrid = JsonConvert.SerializeObject(MainModel.SalesPersonTransferGrid);
                HttpContext.Session.SetString("KeySalesPersonTransferGrid", serializedGrid);
            }

            return View(MainModel);
        }
		[HttpPost]
		public IActionResult StoreSelectedGrid([FromBody] List<SelectedRow> rows)
		{
			string json = JsonConvert.SerializeObject(rows);
			HttpContext.Session.SetString("KeySalesPersonTransferGrid", json);
			return Ok();
		}

		public class GridWrapper
		{
			public List<SalesPersonTransferModel> rows { get; set; }
		}

		[HttpPost]
		[Route("{controller}/Index")]
		public async Task<IActionResult> SalesPersonTransfer(SalesPersonTransferModel model)
		{
			try
			{
				var GIGrid = new DataTable();
				string modelJson = HttpContext.Session.GetString("KeySalesPersonTransferGrid");
				List<SalesPersonTransferModel> SalesPersonTransferGrid = new List<SalesPersonTransferModel>();
				if (!string.IsNullOrEmpty(modelJson))
				{
					SalesPersonTransferGrid = JsonConvert.DeserializeObject<List<SalesPersonTransferModel>>(modelJson);
				}
				
				model.CreatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
				model.EntryByEmpId = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
				model.CC = HttpContext.Session.GetString("Branch");
				if (model.Mode == "U")
				{
					model.UpdatedByEmpId = Convert.ToInt32(HttpContext.Session.GetString("UID"));
					model.UpdatedByEmpName = HttpContext.Session.GetString("EmpName");
					model.UpdationDate = DateTime.Today.ToString("MM/dd/yyyy").Replace("-", "/");

				}
				GIGrid = GetDetailTable(SalesPersonTransferGrid);
				var Result = await _ISalesPersonTransfer.SaveSalesPersonTransfer(model, GIGrid);
				if (Result != null)
				{
					if (Result.StatusText == "Success" && Result.StatusCode == HttpStatusCode.OK)
					{
						ViewBag.isSuccess = true;
						TempData["200"] = "200";
						HttpContext.Session.Remove("KeySalesPersonTransferGrid");
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

				return RedirectToAction(nameof(SalesPersonTransferDashBoard));

			}
			catch (Exception ex)
			{
				// Log and return the error
				LogException<SalesPersonTransferController>.WriteException(_logger, ex);
				var ResponseResult = new ResponseResult
				{
					StatusCode = HttpStatusCode.InternalServerError,
					StatusText = "Error",
					Result = ex
				};
				return View("Error", ResponseResult);
			}
		}

		private static DataTable GetDetailTable(IList<SalesPersonTransferModel> DetailList)
		{
			try
			{
				var GIGrid = new DataTable();
				GIGrid.Columns.Add("SeqNo", typeof(int));
				GIGrid.Columns.Add("NewSalesEmpId", typeof(int));
				GIGrid.Columns.Add("OldSalesEmpId", typeof(int));
				GIGrid.Columns.Add("NewSalesPersEffdate", typeof(DateTime));
				GIGrid.Columns.Add("OldSalesPersTillDate", typeof(DateTime));
				GIGrid.Columns.Add("AccountCode", typeof(int));
				
				foreach (var Item in DetailList)
				{
					GIGrid.Rows.Add(
						new object[]
						{
							Item.seqno == null ? 0 : Item.seqno,
							Item.NewSalesEmpId == null ?0 : Item.NewSalesEmpId,
							Item.OldSalesEmpId == null ? 0 : Item.OldSalesEmpId,
							 string.IsNullOrWhiteSpace(Item.EffFrom) ? DBNull.Value : Convert.ToDateTime(Item.EffFrom),
		string.IsNullOrWhiteSpace(Item.EffTillDate) ? DBNull.Value : Convert.ToDateTime(Item.EffTillDate),
							Item.CustNameCode == null ? 0 : Item.CustNameCode,
							

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
		public async Task<JsonResult> FillNewSalesEmpName(string ShowAllEmp)
		{
			var JSON = await _ISalesPersonTransfer.FillNewSalesEmpName(ShowAllEmp);
			string JsonString = JsonConvert.SerializeObject(JSON);
			return Json(JsonString);
		}
        public async Task<JsonResult> FillOldSalesEmpName(string ShowAllEmp)
		{
			var JSON = await _ISalesPersonTransfer.FillOldSalesEmpName(ShowAllEmp);
			string JsonString = JsonConvert.SerializeObject(JSON);
			return Json(JsonString);
		}
        public async Task<JsonResult> FillEntryID(int YearCode, string EntryDate)
		{
			var JSON = await _ISalesPersonTransfer.FillEntryID(YearCode,  EntryDate);
			string JsonString = JsonConvert.SerializeObject(JSON);
			return Json(JsonString);
		}
        public async Task<JsonResult> FillDesignation(int NewSalesEmpId, int OldSalesEmpId)
		{
			var JSON = await _ISalesPersonTransfer.FillDesignation(NewSalesEmpId,OldSalesEmpId);
			string JsonString = JsonConvert.SerializeObject(JSON);
			return Json(JsonString);
		}

        public async Task<IActionResult> FillCustomerList(int? entryId,string ShowAllCust="")
        {
            //model.Mode = "Search";
            var model = new SalesPersonTransferModel();
            model = await _ISalesPersonTransfer.FillCustomerList(ShowAllCust);
			if (entryId.HasValue)
			{
				var selectedCustCodes = _ISalesPersonTransfer.GetSelectedCustomerCodes(entryId.Value);
				ViewBag.SelectedCustCodes = selectedCustCodes;
			}
			return PartialView("_SalesPersonTransCustomerList", model);

        }

        public async Task<IActionResult> SalesPersonTransferDashBoard(string ReportType, string FromDate, string ToDate)
        {
            var model = new SalesPersonTransferModel();
            var yearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
           
            model.EntryByEmpId = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
			model.UpdatedByEmpName = HttpContext.Session.GetString("EmpName");
			model.EntryBYEmpName = HttpContext.Session.GetString("EmpName");
			var Result = await _ISalesPersonTransfer.GetDashboardData(model);

            if (Result.Result != null)
            {
                var _List = new List<TextValue>();
                DataSet DS = Result.Result;
                if (DS != null && DS.Tables.Count > 0)
                {
                    var dt = DS.Tables[0];
                    model.SalesPersonTransferGrid = CommonFunc.DataTableToList<SalesPersonTransferModel>(dt, "SalesPersonTransferDashBoard");
                }
                
            }

            return View(model);
        }
        public async Task<IActionResult> GetDetailData(string FromDate, string ToDate, string NewSalesEmpName, string OldSalesEmpName, string CustomerName)
        {
            //model.Mode = "Search";
            var model = new SalesPersonTransferModel();
            model = await _ISalesPersonTransfer.GetDashboardDetailData(FromDate, ToDate,  NewSalesEmpName,  OldSalesEmpName,  CustomerName);
         
            return PartialView("_SalesPersonTransferDashBoardGrid", model);
           
        }
        public async Task<IActionResult> DeleteByID(int EntryId, int YearCode, string EntryDate, int EntryByempId)
        {
            var Result = await _ISalesPersonTransfer.DeleteByID(EntryId, YearCode, EntryDate, EntryByempId);

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

            return RedirectToAction("SalesPersonTransferDashBoard");

        }
    }
}
