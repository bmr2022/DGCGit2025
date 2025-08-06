using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Newtonsoft.Json;

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
        public async Task<ActionResult> PartyItemGroupDiscount(int ID, int YC, string Mode, string SlipNo,
            int DiscountCategoryId, string DiscountCatSlipNo, string EffectiveFromDate,
            decimal MinDiscountPer, decimal MaxDiscountPer, string ApplicableMonthlyYearlyAfterEachSale,
            string ApplicableOnAdvancePayment, decimal MinmumAdvancePaymentPercent, string CategoryActive,
            string EntryByMachine, string ActualEntryByEmpName, string ActualEntryDate,
            string LastUpdatedbyEmpName, string LastupDationDate, string CC, int LastUpdatedbyEmpId, int ApprovedByEmpId, int ActualEntryByEmpId
            )
        {
            var MainModel = new PartyItemGroupDiscountModel();

            MainModel.YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));

            MainModel.ActualEntryByEmpId = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
            MainModel.ApprovedByEmpId = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
            MainModel.ApprovedByEmpName = HttpContext.Session.GetString("EmpName");
            MainModel.ActualEntryByEmpName = HttpContext.Session.GetString("EmpName");
            MainModel.CC = HttpContext.Session.GetString("Branch");
            HttpContext.Session.Remove("KeyPartyItemGroupDiscountGrid");
            if (!string.IsNullOrEmpty(Mode) && ID > 0 && Mode == "U")
            {
                //MainModel = await _IDiscountCustomerCategoryMaster.GetViewByID(ID, YC).ConfigureAwait(false);
                //MainModel.Mode = Mode; // Set Mode to Update
                //MainModel.DiscountCustCatEntryId = ID;
                //MainModel.DiscountCustCatYearCode = YC;
                //MainModel.DiscountCategoryId = DiscountCategoryId;
                //MainModel.DiscountCatSlipNo = DiscountCatSlipNo;
                //MainModel.EffectiveFromDate = EffectiveFromDate;
                //MainModel.MinDiscountPer = MinDiscountPer;
                //MainModel.MaxDiscountPer = MaxDiscountPer;
                //MainModel.ApplicableMonthlyYearlyAfterEachSale = ApplicableMonthlyYearlyAfterEachSale;
                //MainModel.ApplicableOnAdvancePayment = ApplicableOnAdvancePayment;
                //MainModel.MinmumAdvancePaymentPercent = MinmumAdvancePaymentPercent;
                //MainModel.CategoryActive = CategoryActive;
                //MainModel.EntryByMachine = EntryByMachine;
                //MainModel.ActualEntryByEmpName = ActualEntryByEmpName;
                //MainModel.ActualEntryDate = ActualEntryDate;
                //MainModel.LastUpdatedbyEmpName = LastUpdatedbyEmpName;
                //MainModel.LastUpdatedbyEmpId = LastUpdatedbyEmpId;
                //MainModel.ApprovedByEmpId = ApprovedByEmpId;
                //MainModel.ActualEntryByEmpId = ActualEntryByEmpId;

                //MainModel.LastupDationDate = LastupDationDate;
                //MainModel.CC = CC;


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

	}
}
