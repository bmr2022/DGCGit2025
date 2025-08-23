using DocumentFormat.OpenXml.Bibliography;
using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using static eTactWeb.Data.Common.CommonFunc;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Controllers
{
    public class AssetsMasterController : Controller
    {
        private readonly IDataLogic _IDataLogic;
        public IAssetsMaster _IAssetsMaster { get; }
        private readonly ILogger<AssetsMasterController> _logger;
        private readonly IConfiguration iconfiguration;

        public IWebHostEnvironment _IWebHostEnvironment { get; }
        public AssetsMasterController(ILogger<AssetsMasterController> logger, IDataLogic iDataLogic, IAssetsMaster iAssetsMaster, EncryptDecrypt encryptDecrypt, IWebHostEnvironment iWebHostEnvironment, IConfiguration iconfiguration)
        {
            _logger = logger;
            _IDataLogic = iDataLogic;
            _IAssetsMaster = iAssetsMaster;
            _IWebHostEnvironment = iWebHostEnvironment;
            this.iconfiguration = iconfiguration;
        }
        [Route("{controller}/Index")]
        public async Task<ActionResult> AssetsMaster(int ID, int YC, string Mode,
            int AssetsEntryId, int AccountCode, string AssetsCode, int ItemCode, int AssetsCateogryId, int CostCenterId, int VendoreAccountCode, string PONO, int POYear, int InvoiceYearCode, int CustoidianEmpId, int ActualEntryBy, int LastupdatedBy, int ForDepartmentId, string CC, int UID,
        string EntryDate, string AssetsName, string ParentAccountName, int ParentAccountCode, string MainGroup, string SubGroup, string UnderGroup, int SubSubGroup, string VendorName, string PODate, string InvoiceNo, string InvoiceDate, string DepreciationMethod, string PurchaseNewUsed, string CountryOfOrigin, string FirstAqusitionOn, decimal OriginalValue, string CapatalizationDate, string BarCode, string SerialNo, string LocationOfInsallation, string Technician, string TechnicialcontactNo, string TechEmployeeName, string CustodiaEmployee, string ActualEntryByEmployee, string InsuranceCompany, string InsuranceDetail,
        decimal NetBookValue, decimal PurchaseValue, decimal ResidualValue, decimal InsuredAmount,
        string ActualEntryDate, string LastUpdatedDate, string EntryByMachine)
        {
            var MainModel = new AssetsMasterModel();

            MainModel.YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));

            MainModel.ActualEntryByEmpId = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
            MainModel.ApprovedByEmpId = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
            MainModel.ApprovedByEmpName = HttpContext.Session.GetString("EmpName");
            MainModel.ActualEntryByEmpName = HttpContext.Session.GetString("EmpName");
            MainModel.CC = HttpContext.Session.GetString("Branch");
			MainModel.LastUpdatedbyEmpId = Convert.ToInt32(HttpContext.Session.GetString("UID"));
			HttpContext.Session.Remove("KeyAssetsMasterGrid");
            if (!string.IsNullOrEmpty(Mode) && ID > 0 && (Mode == "U" || Mode == "V"))
            {
                MainModel = await _IAssetsMaster.GetViewByID(ID, YC).ConfigureAwait(false);
                MainModel.Mode = Mode; // Set Mode to Update
                MainModel.AssetsEntryId = AssetsEntryId;
                MainModel.YearCode = YC;
                MainModel.AccountCode = AccountCode;
                MainModel.AssetsCode = AssetsCode;
                MainModel.ItemCode = ItemCode;
                MainModel.AssetsCateogryId = AssetsCateogryId;
                MainModel.CostCenterId = CostCenterId;
                MainModel.VendoreAccountCode = VendoreAccountCode;
                MainModel.PONO = PONO;
                MainModel.POYear = POYear;
                MainModel.InvoiceYearCode = InvoiceYearCode;
                MainModel.CustoidianEmpId = CustoidianEmpId;
                MainModel.ActualEntryBy = ActualEntryBy;
                MainModel.LastupdatedBy = LastupdatedBy;
                MainModel.ForDepartmentId = ForDepartmentId;
                MainModel.CC = CC;
                MainModel.UID = UID;

                MainModel.EntryDate = EntryDate;
                MainModel.AssetsName = AssetsName;
                MainModel.ParentAccountName = ParentAccountName;
                MainModel.ParentAccountCode = ParentAccountCode;
                MainModel.MainGroup = MainGroup;
                MainModel.SubGroup = SubGroup;
                MainModel.UnderGroup = UnderGroup;
                MainModel.SubSubGroup = SubSubGroup;
                //MainModel.ParentAccountName = VendorName;
                MainModel.PODate = PODate;
                MainModel.InvoiceNo = InvoiceNo;
                MainModel.InvoiceDate = InvoiceDate;
                MainModel.DepreciationMethod = DepreciationMethod;
                MainModel.PurchaseNewUsed = PurchaseNewUsed;
                MainModel.CountryOfOrigin = CountryOfOrigin;
                MainModel.FirstAqusitionOn = FirstAqusitionOn;
                MainModel.OriginalValue = OriginalValue;
                MainModel.CapatalizationDate = CapatalizationDate;
                MainModel.BarCode = BarCode;
                MainModel.SerialNo = SerialNo;
                MainModel.LocationOfInsallation = LocationOfInsallation;
                MainModel.Technician = Technician;
                MainModel.TechnicialcontactNo = TechnicialcontactNo;
                MainModel.TechEmployeeName = TechEmployeeName;
                //MainModel.CustodiaEmployee = CustodiaEmployee;
                MainModel.ActualEntryByEmpName = ActualEntryByEmployee;
                MainModel.InsuranceCompany = InsuranceCompany;
                MainModel.InsuranceDetail = InsuranceDetail;

                MainModel.NetBookValue = NetBookValue;
                MainModel.PurchaseValue = PurchaseValue;
                MainModel.ResidualValue = ResidualValue;
                MainModel.InsuredAmount = InsuredAmount;

                MainModel.ActualEntryDate = ActualEntryDate;
                MainModel.LastUpdatedDate = LastUpdatedDate;
                MainModel.EntryByMachine = EntryByMachine;


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

                string serializedGrid = JsonConvert.SerializeObject(MainModel.AssetsMasterGrid);
                HttpContext.Session.SetString("KeyAssetsMasterGrid", serializedGrid);
            }

            return View(MainModel);
        }
		[HttpPost]
		[Route("{controller}/Index")]
		public async Task<IActionResult> AssetsMaster(AssetsMasterModel model)
		{
			try
			{
                 model.ActualEntryByEmpId = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
                var Result = await _IAssetsMaster.SaveAssetsMaster(model);
				if (Result != null)
				{
					if (Result.StatusText == "Success" && Result.StatusCode == HttpStatusCode.OK)
					{
						ViewBag.isSuccess = true;
						TempData["200"] = "200";

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

				return RedirectToAction(nameof(AssetsMaster));

			}
			catch (Exception ex)
			{
				LogException<AssetsMasterController>.WriteException(_logger, ex);
				var ResponseResult = new ResponseResult
				{
					StatusCode = HttpStatusCode.InternalServerError,
					StatusText = "Error",
					Result = ex
				};
				return View("Error", ResponseResult);
			}
		}
		public async Task<JsonResult> FillItemName()
		{
			var JSON = await _IAssetsMaster.FillItemName();
			string JsonString = JsonConvert.SerializeObject(JSON);
			return Json(JsonString);
		}
        public async Task<JsonResult> FillEntryId(int YearCode, string EntryDate)
		{
			var JSON = await _IAssetsMaster.FillEntryId(YearCode, EntryDate);
			string JsonString = JsonConvert.SerializeObject(JSON);
			return Json(JsonString);
		}
        public async Task<JsonResult> FillCostCenterName()
		{
			var JSON = await _IAssetsMaster.FillCostCenterName();
			string JsonString = JsonConvert.SerializeObject(JSON);
			return Json(JsonString);
		}
        public async Task<JsonResult> FillCategoryName()
		{
			var JSON = await _IAssetsMaster.FillCategoryName();
			string JsonString = JsonConvert.SerializeObject(JSON);
			return Json(JsonString);
		}
         public async Task<JsonResult> FillCustoidianEmpName()
		{
			var JSON = await _IAssetsMaster.FillCustoidianEmpName();
			string JsonString = JsonConvert.SerializeObject(JSON);
			return Json(JsonString);
		}

        public async Task<JsonResult> FillDepartmentName()
		{
			var JSON = await _IAssetsMaster.FillDepartmentName();
			string JsonString = JsonConvert.SerializeObject(JSON);
			return Json(JsonString);
		}
        public async Task<JsonResult> FillParentAccountName()
		{
			var JSON = await _IAssetsMaster.FillParentAccountName();
			string JsonString = JsonConvert.SerializeObject(JSON);
			return Json(JsonString);
		}
        public async Task<JsonResult> FillParentGoupDetail(int ParentAccountCode)
		{
			var JSON = await _IAssetsMaster.FillParentGoupDetail(ParentAccountCode);
			string JsonString = JsonConvert.SerializeObject(JSON);
			return Json(JsonString);
		}
        public async Task<IActionResult> AssetsMasterDashBoard(string FromDate, string ToDate)
        {
            var model = new AssetsMasterModel();
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

            model.ActualEntryByEmpId = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));

            var Result = await _IAssetsMaster.GetDashboardData(model);

            if (Result.Result != null)
            {
                var _List = new List<TextValue>();
                DataSet DS = Result.Result;
                if (DS != null && DS.Tables.Count > 0)
                {
                    var dt = DS.Tables[0];
                    model.AssetsMasterGrid = CommonFunc.DataTableToList<AssetsMasterModel>(dt, "AssetsMasterDashBoard");
                }

            }

            return View(model);
        }
        public async Task<IActionResult> GetDetailData(string FromDate, string ToDate, string ReportType,string AssetsName)
        {
            var model = new AssetsMasterModel();
            model = await _IAssetsMaster.GetDashboardDetailData(FromDate, ToDate, AssetsName);

            return PartialView("_AssetsMasterDashBoardGrid", model);
        }
        public async Task<IActionResult> DeleteByID(int EntryId, int YearCode)
        {
            var Result = await _IAssetsMaster.DeleteByID(EntryId, YearCode);

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

            return RedirectToAction("AssetsMasterDashBoard");

        }
    }
}
