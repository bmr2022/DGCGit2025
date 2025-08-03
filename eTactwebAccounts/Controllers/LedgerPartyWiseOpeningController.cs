using eTactWeb.Data.Common;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using static eTactWeb.Data.Common.CommonFunc;
using static eTactWeb.DOM.Models.Common;
using eTactWeb.DOM.Models;
using System.Net;
using System.Data;
using System.Globalization;

namespace eTactWeb.Controllers
{
    public class LedgerPartyWiseOpeningController : Controller
    {
        private readonly IDataLogic _IDataLogic;
        public ILedgerPartyWiseOpening _ILedgerPartyWiseOpening { get; }
        private readonly ILogger<LedgerPartyWiseOpeningController> _logger;
        private readonly IConfiguration iconfiguration;
        public IWebHostEnvironment _IWebHostEnvironment { get; }
        public LedgerPartyWiseOpeningController(ILogger<LedgerPartyWiseOpeningController> logger, IDataLogic iDataLogic, ILedgerPartyWiseOpening iLedgerPartyWiseOpening, EncryptDecrypt encryptDecrypt, IWebHostEnvironment iWebHostEnvironment, IConfiguration iconfiguration)
        {
            _logger = logger;
            _IDataLogic = iDataLogic;
            _ILedgerPartyWiseOpening = iLedgerPartyWiseOpening;
            _IWebHostEnvironment = iWebHostEnvironment;
            this.iconfiguration = iconfiguration;
        }
        [Route("{controller}/Index")]
        [HttpGet]
        public async Task<ActionResult> LedgerPartyWiseOpening(int ID,string Mode, int OpeningYearCode,int LedgerOpnEntryId,int AccBookTransEntryId,int AccBookTransYearCode,int AccountCode,
        double OpeningAmt,string BillNo, string BillDate, float BillNetAmt, float PendAmt, string Type,
        string TransactionType,string DueDate,string CC,int ActualEntryBy,DateTime ActualEntryDate, int UpdatedBy, string LastUpdatedDate, string EntryByMachine,string AccountNarration, string Unit)
        {
            _logger.LogInformation("\n \n ********** Page Gate Inward ********** \n \n " + _IWebHostEnvironment.EnvironmentName.ToString() + "\n \n");
            TempData.Clear();
            var MainModel = new LedgerPartyWiseOpeningModel();
            MainModel.FromDate = HttpContext.Session.GetString("FromDate");
            MainModel.ToDate = HttpContext.Session.GetString("ToDate");
            MainModel.CC = HttpContext.Session.GetString("Branch");
            MainModel.EntryDate = DateTime.Now.ToString();
            //MainModel.OpeningYearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));

            int currentYear = DateTime.Now.Year;
            int currentMonth = DateTime.Now.Month;
            int financialYearStart = (currentMonth < 4) ? currentYear - 1 : currentYear;
            MainModel.OpeningYearCode = financialYearStart - 1;


            
            MainModel.ActualEntryByEmp = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
            MainModel.EntryByEmpName = HttpContext.Session.GetString("EmpName");
            MainModel.ActualEntryDate = DateTime.Now;
            MainModel.InvoiceDate = DateTime.Now;
            HttpContext.Session.Remove("KeyLedgerPartyWiseOpeningGrid");
            if (!string.IsNullOrEmpty(Mode) && ID > 0 && Mode == "U")
            {
                
                MainModel = await _ILedgerPartyWiseOpening.GetViewByID(OpeningYearCode, LedgerOpnEntryId).ConfigureAwait(false);
                MainModel.EntryId = ID;
                MainModel.Mode = Mode;
                MainModel.OpeningYearCode = OpeningYearCode;
                MainModel.AccBookTransEntryId = AccBookTransEntryId;
                MainModel.AccBookTransYearCode = AccBookTransYearCode;
                MainModel.AccountCode = AccountCode;
                MainModel.OpeningAmt = OpeningAmt;
                MainModel.BillNo = BillNo;
                MainModel.BillDate = BillDate;
                MainModel.BillNetAmt = BillNetAmt;
                MainModel.PendAmt = PendAmt;
                MainModel.Type = Type;
                MainModel.TransactionType = TransactionType;
                MainModel.DueDate = DueDate;
                MainModel.CC = CC;
                MainModel.ActualEntryBy = ActualEntryBy;
                MainModel.ActualEntryDate = ActualEntryDate;
                MainModel.UpdatedBy = UpdatedBy;
                MainModel.LastUpdatedDate = LastUpdatedDate;
                MainModel.EntryByMachine = EntryByMachine;
                MainModel.AccountNarration = AccountNarration;
                MainModel.Unit = Unit;
                if (Mode == "U")
                {
                    MainModel.UpdatedByEmp = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                    MainModel.LastUpdatedBy = HttpContext.Session.GetString("EmpName");
                    MainModel.UpdationDate = DateTime.Now.ToString();
                 
                }
                string serializedGrid = JsonConvert.SerializeObject(MainModel.LedgerPartyWiseOpeningDetails);
                HttpContext.Session.SetString("KeyLedgerPartyWiseOpeningGrid", serializedGrid);
            }
            
            return View(MainModel);
        }
        [Route("{controller}/Index")]
        [HttpPost]
        public async Task<IActionResult> LedgerPartyWiseOpening(LedgerPartyWiseOpeningModel model)
        {
            try
            {
                var GIGrid = new DataTable();
                string modelJson = HttpContext.Session.GetString("KeyLedgerPartyWiseOpeningGrid");
                List<LedgerPartyWiseOpeningDetailModel> LedgerPartyWiseOpeningDetail = new List<LedgerPartyWiseOpeningDetailModel>();
                if (!string.IsNullOrEmpty(modelJson))
                {
                    LedgerPartyWiseOpeningDetail = JsonConvert.DeserializeObject<List<LedgerPartyWiseOpeningDetailModel>>(modelJson);
                }

                model.CreatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                if (model.Mode == "U")
                {
                    GIGrid = GetDetailTable(LedgerPartyWiseOpeningDetail);
                }
                else
                {
                    GIGrid = GetDetailTable(LedgerPartyWiseOpeningDetail);
                }
                var Result = await _ILedgerPartyWiseOpening.SaveLedgerPartyWiseOpening(model, GIGrid);
                if (Result != null)
                {
                    if (Result.StatusText == "Success" && Result.StatusCode == HttpStatusCode.OK)
                    {
                        ViewBag.isSuccess = true;
                        TempData["200"] = "200";
                        HttpContext.Session.Remove("KeyLedgerPartyWiseOpeningGrid");
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

                return RedirectToAction(nameof(LedgerPartyWiseOpeningDashBoard));

            }
            catch (Exception ex)
            {
                // Log and return the error
                LogException<LedgerPartyWiseOpeningController>.WriteException(_logger, ex);
                var ResponseResult = new ResponseResult
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    StatusText = "Error",
                    Result = ex
                };
                return View("Error", ResponseResult);
            }
        }
		public async Task<JsonResult> FillEntryId(int YearCode, string EntryDate)
		{
			var JSON = await _ILedgerPartyWiseOpening.FillEntryId( YearCode,  EntryDate);
			string JsonString = JsonConvert.SerializeObject(JSON);
			return Json(JsonString);
		}
		public async Task<JsonResult> GetAllDataAccountCodeWise(int OpeningYearCode, int AccountCode)
        {
            var model = new LedgerPartyWiseOpeningModel();
            model = await _ILedgerPartyWiseOpening.GetAllDataAccountCodeWise(OpeningYearCode, AccountCode);
            return Json(model.EntryId);
        }
        public async Task<JsonResult> FillLedgerName(int OpeningYearCode)
        {
            var JSON = await _ILedgerPartyWiseOpening.FillLedgerName(OpeningYearCode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillAccountNameForDashBoard()
        {
            var JSON = await _ILedgerPartyWiseOpening.FillAccountNameForDashBoard();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillInvoiceForDashBoard()
        {
            var JSON = await _ILedgerPartyWiseOpening.FillInvoiceForDashBoard();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetOpeningAmt(int OpeningYearCode,int AccountCode)
        {
            var JSON = await _ILedgerPartyWiseOpening.GetOpeningAmt(OpeningYearCode,AccountCode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillDueDate(int AccountCode)
        {
            var JSON = await _ILedgerPartyWiseOpening.FillDueDate(AccountCode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public IActionResult AddLedgerPartWiseOpeningDetail(LedgerPartyWiseOpeningDetailModel model)
        {
            try
            {
                if (model.Mode == "U")
                {
                    string modelJson = HttpContext.Session.GetString("KeyLedgerPartyWiseOpeningGrid");
                    List<LedgerPartyWiseOpeningDetailModel> LedgerPartyWiseOpeningDetail = new List<LedgerPartyWiseOpeningDetailModel>();
                    if (!string.IsNullOrEmpty(modelJson))
                    {
                        LedgerPartyWiseOpeningDetail = JsonConvert.DeserializeObject<List<LedgerPartyWiseOpeningDetailModel>>(modelJson);
                    }

                    var MainModel = new LedgerPartyWiseOpeningModel();
                    var WorkOrderPGrid = new List<LedgerPartyWiseOpeningDetailModel>();
                    var OrderGrid = new List<LedgerPartyWiseOpeningDetailModel>();
                    var ssGrid = new List<LedgerPartyWiseOpeningDetailModel>();

                    var count = 0;
                    if (model != null)
                    {
                        if (LedgerPartyWiseOpeningDetail == null)
                        {
                            model.SrNO = 1;
                            OrderGrid.Add(model);
                        }
                        else
                        {
                            if (LedgerPartyWiseOpeningDetail.Any(x => (x.BillNo == model.BillNo)))
                            {
                                return StatusCode(207, "Duplicate");
                            }
                            else
                            {
                                //count = WorkOrderProcessGrid.Count();
                                model.SrNO = LedgerPartyWiseOpeningDetail.Count + 1;
                                OrderGrid = LedgerPartyWiseOpeningDetail.Where(x => x != null).ToList();
                                ssGrid.AddRange(OrderGrid);
                                OrderGrid.Add(model);

                            }

                        }

                        MainModel.LedgerPartyWiseOpeningDetails = OrderGrid;

                        string serializedGrid = JsonConvert.SerializeObject(MainModel.LedgerPartyWiseOpeningDetails);
                        HttpContext.Session.SetString("KeyLedgerPartyWiseOpeningGrid", serializedGrid);

                    }
                    else
                    {
                        ModelState.TryAddModelError("Error", "Schedule List Cannot Be Empty...!");
                    }
                    return PartialView("_LedgerPartyWiseOpeningGrid", MainModel);
                }
                else
                {
                    string modelJson = HttpContext.Session.GetString("KeyLedgerPartyWiseOpeningGrid");
                    List<LedgerPartyWiseOpeningDetailModel> LedgerPartyWiseOpeningDetail = new List<LedgerPartyWiseOpeningDetailModel>();
                    if (!string.IsNullOrEmpty(modelJson))
                    {
                        LedgerPartyWiseOpeningDetail = JsonConvert.DeserializeObject<List<LedgerPartyWiseOpeningDetailModel>>(modelJson);
                    }

                    var MainModel = new LedgerPartyWiseOpeningModel();
                    var WorkOrderPGrid = new List<LedgerPartyWiseOpeningDetailModel>();
                    var OrderGrid = new List<LedgerPartyWiseOpeningDetailModel>();
                    var ssGrid = new List<LedgerPartyWiseOpeningDetailModel>();

                    if (model != null)
                    {
                        if (LedgerPartyWiseOpeningDetail == null)
                        {
                            model.SrNO = 1;
                            OrderGrid.Add(model);
                        }
                        else
                        {
                            if (LedgerPartyWiseOpeningDetail.Any(x => (x.BillNo == model.BillNo)))
                            {
                                return StatusCode(207, "Duplicate");
                            }
                            else
                            {
                                //count = WorkOrderProcessGrid.Count();
                                model.SrNO = LedgerPartyWiseOpeningDetail.Count + 1;
                                OrderGrid = LedgerPartyWiseOpeningDetail.Where(x => x != null).ToList();
                                ssGrid.AddRange(OrderGrid);
                                OrderGrid.Add(model);

                            }

                        }

                        MainModel.LedgerPartyWiseOpeningDetails = OrderGrid;

                        string serializedGrid = JsonConvert.SerializeObject(MainModel.LedgerPartyWiseOpeningDetails);
                        HttpContext.Session.SetString("KeyLedgerPartyWiseOpeningGrid", serializedGrid);
                    }
                    else
                    {
                        ModelState.TryAddModelError("Error", "Schedule List Cannot Be Empty...!");
                    }
                    return PartialView("_LedgerPartyWiseOpeningGrid", MainModel);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IActionResult ClearGrid()
        {
            HttpContext.Session.Remove("KeyLedgerPartyWiseOpeningGrid");  
            HttpContext.Session.Remove("LedgerPartyWiseOpeningModel");
            var MainModel = new LedgerPartyWiseOpeningModel();
            return PartialView("_LedgerPartyWiseOpeningGrid", MainModel);
        }
        public IActionResult EditItemRow(int SrNO, string Mode)
        {
            IList<LedgerPartyWiseOpeningDetailModel> LedgerPartyWiseOpeningDetail = new List<LedgerPartyWiseOpeningDetailModel>();
            if (Mode == "U")
            {
                string modelJson = HttpContext.Session.GetString("KeyLedgerPartyWiseOpeningGrid");
                if (!string.IsNullOrEmpty(modelJson))
                {
                    LedgerPartyWiseOpeningDetail = JsonConvert.DeserializeObject<List<LedgerPartyWiseOpeningDetailModel>>(modelJson);
                }
            }
            else
            {
                string modelJson = HttpContext.Session.GetString("KeyLedgerPartyWiseOpeningGrid");
                if (!string.IsNullOrEmpty(modelJson))
                {
                    LedgerPartyWiseOpeningDetail = JsonConvert.DeserializeObject<List<LedgerPartyWiseOpeningDetailModel>>(modelJson);
                }
            }
            IEnumerable<LedgerPartyWiseOpeningDetailModel> SSBreakdownGrid = LedgerPartyWiseOpeningDetail;
            if (LedgerPartyWiseOpeningDetail != null)
            {
                SSBreakdownGrid = LedgerPartyWiseOpeningDetail.Where(x => x.SrNO == SrNO);
              
            }
            string JsonString = JsonConvert.SerializeObject(SSBreakdownGrid);
            return Json(JsonString);
        }
        public IActionResult DeleteItemRow(int SrNO, string Mode)
        {
            var MainModel = new LedgerPartyWiseOpeningModel();
            if (Mode == "U")
            {
                string modelJson = HttpContext.Session.GetString("KeyLedgerPartyWiseOpeningGrid");
                List<LedgerPartyWiseOpeningDetailModel> LedgerPartyWiseOpeningDetail = new List<LedgerPartyWiseOpeningDetailModel>();
                if (!string.IsNullOrEmpty(modelJson))
                {
                    LedgerPartyWiseOpeningDetail = JsonConvert.DeserializeObject<List<LedgerPartyWiseOpeningDetailModel>>(modelJson);
                }

                int Indx = Convert.ToInt32(SrNO) - 1;

                if (LedgerPartyWiseOpeningDetail != null && LedgerPartyWiseOpeningDetail.Count > 0)
                {

                    LedgerPartyWiseOpeningDetail.RemoveAt(Convert.ToInt32(Indx));

                    Indx = 0;

                    foreach (var item in LedgerPartyWiseOpeningDetail)
                    {
                        Indx++;
                        item.SrNO = Indx;
                    }
                    MainModel.LedgerPartyWiseOpeningDetails = LedgerPartyWiseOpeningDetail;

                    string serializedGrid = JsonConvert.SerializeObject(MainModel.LedgerPartyWiseOpeningDetails);
                    HttpContext.Session.SetString("KeyLedgerPartyWiseOpeningGrid", serializedGrid);
                }
            }
            else
            {
                string modelJson = HttpContext.Session.GetString("KeyLedgerPartyWiseOpeningGrid");
                List<LedgerPartyWiseOpeningDetailModel> LedgerPartyWiseOpeningDetail = new List<LedgerPartyWiseOpeningDetailModel>();
                if (!string.IsNullOrEmpty(modelJson))
                {
                    LedgerPartyWiseOpeningDetail = JsonConvert.DeserializeObject<List<LedgerPartyWiseOpeningDetailModel>>(modelJson);
                }

                int Indx = Convert.ToInt32(SrNO) - 1;
                if (LedgerPartyWiseOpeningDetail != null && LedgerPartyWiseOpeningDetail.Count > 0)
                {
                    LedgerPartyWiseOpeningDetail.RemoveAt(Convert.ToInt32(Indx));

                    Indx = 0;

                    foreach (var item in LedgerPartyWiseOpeningDetail)
                    {
                        Indx++;
                        item.SrNO = Indx;
                    }
                    MainModel.LedgerPartyWiseOpeningDetails = LedgerPartyWiseOpeningDetail;

                    string serializedGrid = JsonConvert.SerializeObject(MainModel.LedgerPartyWiseOpeningDetails);
                    HttpContext.Session.SetString("KeyLedgerPartyWiseOpeningGrid", serializedGrid);
                }
            }

            return PartialView("_LedgerPartyWiseOpeningGrid", MainModel);
        }
        private static DataTable GetDetailTable(IList<LedgerPartyWiseOpeningDetailModel> DetailList)
        {
            try
            {
                var GIGrid = new DataTable();

                GIGrid.Columns.Add("LedgerOpnEntryId", typeof(int));
                GIGrid.Columns.Add("AccBookTransEntryId", typeof(int));
                GIGrid.Columns.Add("AccBookTransYearCode", typeof(int));
                GIGrid.Columns.Add("AccountCode", typeof(int));
                GIGrid.Columns.Add("OpeningAmt", typeof(decimal));
                GIGrid.Columns.Add("InvoiceNo", typeof(string));
                GIGrid.Columns.Add("InvoiceDate", typeof(DateTime));
                GIGrid.Columns.Add("InvoiceYearCode", typeof(int));
                GIGrid.Columns.Add("InvNetAmt", typeof(decimal));
                GIGrid.Columns.Add("InvPendAmt", typeof(decimal));
                GIGrid.Columns.Add("DrCrType", typeof(string));
                GIGrid.Columns.Add("TransactionType", typeof(string));
                GIGrid.Columns.Add("DueDate", typeof(DateTime));
                GIGrid.Columns.Add("CC", typeof(string));
                GIGrid.Columns.Add("ActualEntryBy", typeof(int));
                GIGrid.Columns.Add("ActualEntryDate", typeof(DateTime));
                GIGrid.Columns.Add("UpdatedBy", typeof(int));
                GIGrid.Columns.Add("LastUpdatedDate", typeof(DateTime));
                GIGrid.Columns.Add("EntryByMachine", typeof(string));
                GIGrid.Columns.Add("AccountNarration", typeof(string));
                GIGrid.Columns.Add("SaveUpdate", typeof(string));
                GIGrid.Columns.Add("SeqNo", typeof(int));
                foreach (var Item in DetailList)
                {
                    GIGrid.Rows.Add(
                        new object[]
                        {
                    Item.EntryId,
                    Item.AccBookTransEntryId,
                    Item.AccBookTransYearCode,
                    Item.AccountCode ,
                    Item.OpeningAmt,
                    Item.BillNo ,
                    Item.BillDate ,
                    Item.BillYear,
                    Item.BillNetAmt,
                    Item.PendAmt,
                    Item.Type ,
                    Item.TransactionType,
                    Item.DueDate ,
                    Item.CC  ,
                    Item.ActualEntryBy,
                    Item.ActualEntryDate ,
                    Item.UpdatedBy,
                    Item.LastUpdatedDate ,
                    Item.EntryByMachine ,
                     string.IsNullOrEmpty(Item.AccountNarration) ? "" : Item.AccountNarration,
                    Item.Unit,
                    Item.SrNO
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
        [HttpGet]
        public async Task<IActionResult> LedgerPartyWiseOpeningDashBoard()
        {
            try
            {
                var model = new LedgerPartyWiseOpeningDashBoardModel();
                var result = await _ILedgerPartyWiseOpening.GetDashboardData().ConfigureAwait(true);
                DateTime now = DateTime.Now;
                model.FromDate = new DateTime(now.Year, now.Month, 1).ToString("dd/MM/yyyy");
                model.ToDate = new DateTime(now.Year + 1, 3, 31).ToString("dd/MM/yyyy");
                if (result != null && result.Result != null)
                {
                    DataSet ds = result.Result;
                    if (ds != null && ds.Tables.Count > 0)
                    {
                        var dt = ds.Tables[0];
                        model.LedgerPartyWiseOpeningDashBoardDetail = CommonFunc.DataTableToList<LedgerPartyWiseOpeningDashBoardModel>(dt, "LedgerPartyWiseOpeningDashBoard");
                    }

                }

                return View(model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IActionResult> GetDetailData(string LedgerName, string BillNo)
        {
            //model.Mode = "Search";
            var model = new LedgerPartyWiseOpeningDashBoardModel();
            model = await _ILedgerPartyWiseOpening.GetDashboardDetailData( LedgerName,  BillNo);
            return PartialView("_LedgerPartyWiseOpeningDashBoardGrid", model);
        }
        public async Task<IActionResult> DeleteByID(string EntryByMachine, int OpeningYearCode, int LedgerOpnEntryId, int AccountCode)
        {
            var Result = await _ILedgerPartyWiseOpening.DeleteByID( EntryByMachine,  OpeningYearCode,  LedgerOpnEntryId,  AccountCode);

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

            return RedirectToAction("LedgerPartyWiseOpeningDashBoard");

        }
    }
}
