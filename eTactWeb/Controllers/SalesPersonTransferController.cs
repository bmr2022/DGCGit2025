using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

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
        public async Task<ActionResult> SalesPersonTransfer(int ID, int YC, string Mode, string FromDate, string ToDate)
        {
            var MainModel = new SalesPersonTransferModel();

            MainModel.SalesPersTransfYearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
            MainModel.CreatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));

            MainModel.EntryByEmpId = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
            MainModel.ApprovedBy = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
            MainModel.ApprovedByEmpName = HttpContext.Session.GetString("EmpName");
            MainModel.CC = HttpContext.Session.GetString("Branch");
            HttpContext.Session.Remove("KeySalesPersonTransferGrid");
            if (!string.IsNullOrEmpty(Mode) && ID > 0 && Mode == "U")
            {
                //MainModel = await _IMaterialConversion.GetViewByID(ID, YC, FromDate, ToDate).ConfigureAwait(false);
                //MainModel.Mode = Mode;
                //MainModel.EntryId = ID;
                //MainModel.OpeningYearCode = YC;
                //MainModel.StoreId = StoreId;
                //MainModel.StoreName = StoreName;
                //MainModel.OriginalItemCode = OriginalItemCode;
                //MainModel.OriginalPartCode = OriginalPartCode;
                //MainModel.OriginalItemName = OriginalItemName;
                //MainModel.OriginalQty = OriginalQty;
                //MainModel.Unit = Unit;
               
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
                HttpContext.Session.SetString("KeyMaterialConversionGrid", serializedGrid);
            }

            return View(MainModel);
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
        public async Task<IActionResult> FillCustomerList(string ShowAllCust)
        {
            //model.Mode = "Search";
            var model = new SalesPersonTransferModel();
            model = await _ISalesPersonTransfer.FillCustomerList(ShowAllCust);

            return PartialView("_SalesPersonTransCustomerList", model);

        }
    }
}
