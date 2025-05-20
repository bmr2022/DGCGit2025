using eTactWeb.Data.Common;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using eTactWeb.DOM.Models;
using System.Net;
using System.Data;
using System.Globalization;
using static eTactWeb.Data.Common.CommonFunc;
using static eTactWeb.DOM.Models.Common;
using System.Data.SqlClient;
using System.Dynamic;

namespace eTactWeb.Controllers
{
    public class BankReconciliationController : Controller
    {
        private readonly IDataLogic _IDataLogic;
        public IBankReconciliation _IBackReconciliation { get; }
        private readonly ILogger<BankReconciliationController> _logger;
        private readonly IConfiguration iconfiguration;
        public IWebHostEnvironment _IWebHostEnvironment { get; }
        public BankReconciliationController(ILogger<BankReconciliationController> logger, IDataLogic iDataLogic, IBankReconciliation iBackReconciliation, EncryptDecrypt encryptDecrypt, IWebHostEnvironment iWebHostEnvironment, IConfiguration iconfiguration)
        {
            _logger = logger;
            _IDataLogic = iDataLogic;
            _IBackReconciliation = iBackReconciliation;
            _IWebHostEnvironment = iWebHostEnvironment;
            this.iconfiguration = iconfiguration;
        }

        [Route("{controller}/Index")]
        [HttpGet]
        public async Task<ActionResult> BankReconciliation()
        {
            var MainModel = new BankReconciliationModel();
            MainModel.BankReconciliationGrid = new List<BankReconciliationModel>();
            MainModel.DateFrom = HttpContext.Session.GetString("DateFrom");
            MainModel.DateFrom = HttpContext.Session.GetString("DateFrom");
            //MainModel.YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));

            return View(MainModel); // Pass the model with old data to the view
        }
        [Route("{controller}/Index")]
        [HttpPost]
        public async Task<IActionResult> BankReconciliation(List<BankReconciliationModel> model)
        {
            try
            {
                var GIGrid = new DataTable();
                GIGrid = GetDetailTable(model);

                var Result = await _IBackReconciliation.SaveBankReceipt(model, GIGrid);
                if (Result != null)
                {
                    if (Result.StatusText == "Success" && Result.StatusCode == HttpStatusCode.OK)
                    {
                        ViewBag.isSuccess = true;
                        TempData["200"] = "200";
                        HttpContext.Session.Remove("KeyBankReconciliationGrid");
                    }
                    else if (Result.StatusText == "Success" && Result.StatusCode == HttpStatusCode.Accepted)
                    {
                        ViewBag.isSuccess = true;
                        TempData["202"] = "202";
                        HttpContext.Session.Remove("KeyBankReconciliationGrid");
                    }
                    else if (Result.StatusText == "Error" && Result.StatusCode == HttpStatusCode.InternalServerError)
                    {
                        ViewBag.isSuccess = false;
                        TempData["500"] = "500";
                        _logger.LogError($"\n \n ********** LogError ********** \n {JsonConvert.SerializeObject(Result)}\n \n");
                        return View("Error", Result);
                    }
                }

                return RedirectToAction(nameof(BankReconciliation));

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private static DataTable GetDetailTable(IList<BankReconciliationModel> DetailList)
        {
            try
            {
                var GIGrid = new DataTable();
                GIGrid.Columns.Add("SeqNo", typeof(int));
                GIGrid.Columns.Add("AccEntryId", typeof(int));
                GIGrid.Columns.Add("AccYaerCode", typeof(int));
                GIGrid.Columns.Add("VoucherNo", typeof(string));
                GIGrid.Columns.Add("VoucherType", typeof(string));
                GIGrid.Columns.Add("AccountCode", typeof(int));
                GIGrid.Columns.Add("chequeno", typeof(string));
                GIGrid.Columns.Add("DrAmt", typeof(decimal));
                GIGrid.Columns.Add("CrAmt", typeof(decimal));
                GIGrid.Columns.Add("chequeClearDate", typeof(DateTime));

                foreach (var Item in DetailList)
                {
                    GIGrid.Rows.Add(
                        new object[]
                        {
                Item.SeqNo ,
                Item.entryid ,
                Item.AccYearCode ,
                Item.VoucherNo ?? string.Empty,
                Item.Type ?? string.Empty,
                Item.Account_Code ,
                Item.chequeNo ?? string.Empty,
                Item.DrAmt,
                Item.CrAmt ,
                Item.ChequeClearDate
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
        public async Task<JsonResult> GetBankName(string DateFrom, string DateTo, string NewOrEdit)
        {
            var JSON = await _IBackReconciliation.GetBankName(DateFrom, DateTo, NewOrEdit);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<IActionResult> GetDetailsData(string DateFrom, string DateTo, string chequeNo, string NewOrEdit, string Account_Code)
        {
            var model = new BankReconciliationModel();
            model = await _IBackReconciliation.GetDetailsData(DateFrom, DateTo, chequeNo, NewOrEdit, Account_Code);
            return PartialView("_BankReconciliationGrid", model);
        }
        public async Task<JsonResult> GetLedgerBalance(int OpeningYearCode, int AccountCode, string VoucherDate)
        {
            var JSON = await _IBackReconciliation.GetLedgerBalance(OpeningYearCode, AccountCode, VoucherDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
    }
}
