using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System.Data;
using System.Net;
using System.Runtime.Caching;
using static eTactWeb.Data.Common.CommonFunc;
using static eTactWeb.DOM.Models.Common;

namespace eTactwebAccounts.Controllers
{
    public class BankPaymentController : Controller
    {
        private readonly IDataLogic _IDataLogic;
        public IBankPayment _IBankPayment { get; }

        private readonly ILogger<BankPaymentController> _logger;
        private readonly IConfiguration iconfiguration;
        private readonly IMemoryCache _MemoryCache;
        public IWebHostEnvironment _IWebHostEnvironment { get; }
        public BankPaymentController(ILogger<BankPaymentController> logger, IDataLogic iDataLogic, IBankPayment iBankPayment, IMemoryCache iMemoryCache, EncryptDecrypt encryptDecrypt, IWebHostEnvironment iWebHostEnvironment, IConfiguration iconfiguration)
        {
            _logger = logger;
            _IDataLogic = iDataLogic;
            _IBankPayment = iBankPayment;
            _MemoryCache = iMemoryCache;
            _IWebHostEnvironment = iWebHostEnvironment;
            this.iconfiguration = iconfiguration;
        }
        [Route("{controller}/Index")]
        [HttpGet]
        public async Task<ActionResult> BankPayment(int ID, string Mode, int YearCode, string VoucherNo)
        {
            _logger.LogInformation("\n \n ********** Page Gate Inward ********** \n \n " + _IWebHostEnvironment.EnvironmentName.ToString() + "\n \n");

            TempData.Clear();
            var MainModel = new BankPaymentModel();
            MainModel.CC = HttpContext.Session.GetString("Branch");
            MainModel.YearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
            MainModel.ActualEntryBy = HttpContext.Session.GetString("UID");
            //MainModel.ActualEntryDate = DateTime.Now.ToString("dd/MM/yy");
            MainModel.UID = Convert.ToInt32(HttpContext.Session.GetString("UID"));

            if (MainModel.Mode == "U")
            {
                MainModel.UpdatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                MainModel.UpdatedOn = DateTime.Now;
            }

            _MemoryCache.Remove("KeyBankPaymentGrid");

            if (!string.IsNullOrEmpty(Mode) && ID > 0 && Mode == "U")
            {
               MainModel = await _IBankPayment.GetViewByID(ID, YearCode, VoucherNo).ConfigureAwait(false);
                MainModel.Mode = Mode; // Set Mode to Update
                MainModel.ID = ID;
                MainModel.VoucherNo = VoucherNo;

                if (Mode == "U")
                {
                    //MainModel.UpdatedByEmp = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                    //MainModel.LastUpdatedBy = HttpContext.Session.GetString("EmpName");
                    //MainModel.UpdationDate = DateTime.Now.ToString();
                    //MainModel.ActualEntryDate = DateTime.Today.ToString("MM/dd/yyyy").Replace("-", "/");
                    //MainModel.ActualEntryByEmp = Convert.ToInt32(HttpContext.Session.GetString("EmpID"));
                    //MainModel.EffectiveDate = DateTime.Today.ToString("MM/dd/yyyy").Replace("-", "/");

                }
                MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                    SlidingExpiration = TimeSpan.FromMinutes(55),
                    Size = 1024
                };
                _MemoryCache.Set("KeyBankPaymentGrid", MainModel.BankPaymentGrid, cacheEntryOptions);
            }

            else
            {
                // MainModel = await BindModels(MainModel);
            }

            return View(MainModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("{controller}/Index")]
        public async Task<IActionResult> BankPayment(BankPaymentModel model)
        {
            try
            {
                var GIGrid = new DataTable();
                _MemoryCache.TryGetValue("KeyBankPaymentGrid", out List<BankPaymentModel> BankPaymentGrid);


                model.CreatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                if (model.Mode == "U")
                {
                    model.UpdatedOn = DateTime.Now;
                    GIGrid = GetDetailTable(BankPaymentGrid);
                }
                else
                {
                    GIGrid = GetDetailTable(BankPaymentGrid);
                }
                var Result = await _IBankPayment.SaveBankPayment(model, GIGrid);
                if (Result != null)
                {
                    if (Result.StatusText == "Success" && Result.StatusCode == HttpStatusCode.OK)
                    {
                        ViewBag.isSuccess = true;
                        TempData["200"] = "200";
                        _MemoryCache.Remove("KeyBankReceiptGrid");
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

                return RedirectToAction(nameof(BankPayment));

            }
            catch (Exception ex)
            {
                // Log and return the error
                LogException<BankPaymentController>.WriteException(_logger, ex);
                var ResponseResult = new ResponseResult
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    StatusText = "Error",
                    Result = ex
                };
                return View("Error", ResponseResult);
            }
        }

        private static DataTable GetDetailTable(IList<BankPaymentModel> DetailList)
        {
            try
            {
                var GIGrid = new DataTable();
                GIGrid.Columns.Add("AccEntryId", typeof(int));
                GIGrid.Columns.Add("AccYearCode", typeof(int));
                GIGrid.Columns.Add("EntryDate", typeof(DateTime));
                GIGrid.Columns.Add("DocEntryId", typeof(int));
                GIGrid.Columns.Add("VoucherDocNo", typeof(string));
                GIGrid.Columns.Add("BillVouchNo", typeof(string));
                GIGrid.Columns.Add("VoucherDocDate", typeof(DateTime));
                GIGrid.Columns.Add("BillInvoiceDate", typeof(DateTime));
                GIGrid.Columns.Add("BillYearCode", typeof(int));
                GIGrid.Columns.Add("VoucherRefNo", typeof(string));
                GIGrid.Columns.Add("SeqNo", typeof(int));
                GIGrid.Columns.Add("Accountcode", typeof(int));
                GIGrid.Columns.Add("BankCashAccountCode", typeof(int));
                GIGrid.Columns.Add("AccountGroupType", typeof(string));
                GIGrid.Columns.Add("Description", typeof(string));
                GIGrid.Columns.Add("VoucherRemark", typeof(string));
                GIGrid.Columns.Add("DrAmt", typeof(decimal));
                GIGrid.Columns.Add("CrAmt", typeof(decimal));
                GIGrid.Columns.Add("entryBankCash", typeof(string));
                GIGrid.Columns.Add("Vouchertype", typeof(string));
                GIGrid.Columns.Add("chequeDate", typeof(DateTime));
                GIGrid.Columns.Add("chequeClearDate", typeof(DateTime));
                GIGrid.Columns.Add("UID", typeof(int));
                GIGrid.Columns.Add("CC", typeof(string));
                GIGrid.Columns.Add("TDSNatureOfPayment", typeof(string));
                GIGrid.Columns.Add("RoundDr", typeof(decimal));
                GIGrid.Columns.Add("RoundCr", typeof(decimal));
                GIGrid.Columns.Add("AgainstEntryid", typeof(int));
                GIGrid.Columns.Add("AgainstVoucheryearcode", typeof(int));
                GIGrid.Columns.Add("AgainstVoucherType", typeof(string));
                GIGrid.Columns.Add("againstVoucherRefNo", typeof(string));
                GIGrid.Columns.Add("AgainstVoucherNo", typeof(string));
                GIGrid.Columns.Add("AgainstBillno", typeof(string));
                GIGrid.Columns.Add("PONo", typeof(string));
                GIGrid.Columns.Add("PoDate", typeof(DateTime));
                GIGrid.Columns.Add("POYear", typeof(int));
                GIGrid.Columns.Add("SONo", typeof(int));
                GIGrid.Columns.Add("CustOrderNo", typeof(string));
                GIGrid.Columns.Add("SoDate", typeof(DateTime));
                GIGrid.Columns.Add("SOYear", typeof(int));
                GIGrid.Columns.Add("ApprovedBy", typeof(int));
                GIGrid.Columns.Add("ApprovedDate", typeof(DateTime));
                GIGrid.Columns.Add("Approved", typeof(string));
                GIGrid.Columns.Add("AccountNarration", typeof(string));
                GIGrid.Columns.Add("CurrencyId", typeof(int));
                GIGrid.Columns.Add("CurrentValue", typeof(decimal));
                GIGrid.Columns.Add("AdjAmountInOtherCurrency", typeof(decimal));
                GIGrid.Columns.Add("AmountInOtherCurr", typeof(decimal));
                GIGrid.Columns.Add("ItemCode", typeof(int));
                GIGrid.Columns.Add("VoucherNo", typeof(string));
                GIGrid.Columns.Add("ChequePrintAC", typeof(string));
                GIGrid.Columns.Add("EmpCode", typeof(int));
                GIGrid.Columns.Add("DeptCode", typeof(int));
                GIGrid.Columns.Add("MRNNO", typeof(string));
                GIGrid.Columns.Add("MRNDate", typeof(DateTime));
                GIGrid.Columns.Add("MRNYearCode", typeof(int));
                GIGrid.Columns.Add("CostCenterId", typeof(int));
                GIGrid.Columns.Add("PaymentMode", typeof(string));
                GIGrid.Columns.Add("EntryTypebankcashLedger", typeof(string));
                GIGrid.Columns.Add("TDSApplicable", typeof(string));
                GIGrid.Columns.Add("TDSChallanNo", typeof(string));
                GIGrid.Columns.Add("TDSChallanDate", typeof(DateTime));
                GIGrid.Columns.Add("PreparedByEmpId", typeof(int));
                GIGrid.Columns.Add("CGSTAccountCode", typeof(int));
                GIGrid.Columns.Add("CGSTPer", typeof(decimal));
                GIGrid.Columns.Add("CGSTAmt", typeof(decimal));
                GIGrid.Columns.Add("SGSTAccountCode", typeof(int));
                GIGrid.Columns.Add("SGSTPer", typeof(decimal));
                GIGrid.Columns.Add("SGSTAmt", typeof(decimal));
                GIGrid.Columns.Add("IGSTAccountCode", typeof(int));
                GIGrid.Columns.Add("IGSTPer", typeof(decimal));
                GIGrid.Columns.Add("IGSTAmt", typeof(decimal));
                GIGrid.Columns.Add("ModeOfAdjustment", typeof(string));
                GIGrid.Columns.Add("NameOnCheque", typeof(string));
                GIGrid.Columns.Add("BalanceSheetClosed", typeof(string));
                GIGrid.Columns.Add("ProjectNo", typeof(string));
                GIGrid.Columns.Add("ProjectYearcode", typeof(int));
                GIGrid.Columns.Add("ProjectDate", typeof(DateTime));
                GIGrid.Columns.Add("ActualEntryBy", typeof(int));
                GIGrid.Columns.Add("ActualEntryDate", typeof(DateTime));
                GIGrid.Columns.Add("UpdatedBy", typeof(int));
                GIGrid.Columns.Add("LastUpdatedDate", typeof(DateTime));
                GIGrid.Columns.Add("EntryByMachine", typeof(string));
                GIGrid.Columns.Add("OursalespersonId", typeof(int));
                GIGrid.Columns.Add("SubVoucherName", typeof(string));
                foreach (var Item in DetailList)
                {
                    GIGrid.Rows.Add(
                        new object[]
                        {
                Item.AccEntryId ,
                Item.YearCode ,
                Item.EntryDate=DateTime.Now.ToString("dd/MM/yy") ,
                Item.DocEntryId ,
                Item.VoucherDocNo ?? string.Empty,
                Item.BillVouchNo ?? string.Empty,
                Item.VoucherDocDate=DateTime.Now.ToString("dd/MM/yy") ,
                Item.BillInvoiceDate=DateTime.Now.ToString("dd/MM/yy") ,
                Item.BillYearCode ,
                Item.VoucherRefNo ?? string.Empty,
                Item.SeqNo ,
                Item.AccountCode,
                Item.BankCashAccountCode ,
                Item.AccountGroupType ?? string.Empty,
                Item.Description ?? string.Empty,
                Item.VoucherRemark ?? string.Empty,
                Item.DrAmt,
                Item.CrAmt ,
                Item.EntryBankCash,
                Item.VoucherType ?? string.Empty,
                Item.ChequeDate = DateTime.Now.ToString("dd/MM/yy") ,
                Item.ChequeClearDate =DateTime.Now.ToString("dd/MM/yy") ,
                Item.UID ,
                Item.CC ?? string.Empty,
                Item.TDSNatureOfPayment ?? string.Empty,
                Item.RoundDr ,
                Item.RoundCr ,
                Item.AgainstVoucherEntryId ,
                Item.AgainstVoucheryearCode ,
                Item.AgainstVoucherType ?? string.Empty,
                Item.AgainstVoucherRefNo ?? string.Empty,
                Item.AgainstVoucherNo ?? string.Empty,
                Item.AgainstBillno ?? string.Empty,
                Item.PONo ?? string.Empty,
                Item.PoDate =DateTime.Now.ToString("dd/MM/yy"),
                Item.POYear ,
                Item.SONo ,
                Item.CustOrderNo ?? string.Empty,
                Item.SoDate  ,
                Item.SOYear ,
                Item.ApprovedBy,
                Item.ApprovedDate = DateTime.Now.ToString("dd/MM/yy"),
                Item.Approved ,
                Item.AccountNarration ?? string.Empty,
                Item.CurrencyId ,
                Item.CurrentValue ,
                Item.AdjustmentAmtOthCur ,
                Item.AdjustmentAmt ,
                Item.ItemCode ,
                Item.VoucherNo ?? string.Empty,
                Item.ChequePrintAC ?? string.Empty,
                Item.EmpCode ,
                Item.DeptCode,
                Item.MRNO ?? string.Empty,
                Item.MRNDate =DateTime.Now.ToString("dd/MM/yy") ,
                Item.MRNYearCode ,
                Item.CostCenterId ,
                Item.PaymentMode ?? string.Empty,
                Item.EntryTypebankcashLedger ?? string.Empty,
                Item.TDSApplicable ?? string.Empty,
                Item.TDSChallanNo ?? string.Empty,
                Item.TDSChallanDate = DateTime.Now.ToString("dd/MM/yy") ,
                Item.PreparedByEmpId ,
                Item.CGSTAccountCode ,
                Item.CGSTPer ,
                Item.CGSTAmt,
                Item.SGSTAccountCode ,
                Item.SGSTPer,
                Item.SGSTAmt ,
                Item.IGSTAccountCode ,
                Item.IGSTPer,
                Item.IGSTAmt ,
                Item.ModeOfAdjustment ?? string.Empty,
                Item.NameOnCheque ?? string.Empty,
                Item.BalanceSheetClosed ?? string.Empty,
                Item.ProjectNo ,
                Item.ProjectYearcode ,
                Item.ProjectDate = DateTime.Now.ToString("dd/MM/yy"),
                Item.ActualEntryBy ,
                Item.ActualEntryDate = DateTime.Now.ToString("dd/MM/yy"),
                Item.UpdatedBy ,
                Item.UpdatedOn ,
                Item.EntryByMachine ?? string.Empty,
                Item.OursalespersonId ,
                Item.SubVoucher ?? string.Empty
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

        public async Task<JsonResult> GetLedgerBalance(int OpeningYearCode, int AccountCode, string VoucherDate)
        {
            var JSON = await _IBankPayment.GetLedgerBalance(OpeningYearCode, AccountCode, VoucherDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillLedgerName(string VoucherType, string Type)
        {
            var JSON = await _IBankPayment.FillLedgerName(VoucherType, Type);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillSubVoucherName(string VoucherType)
        {
            var JSON = await _IBankPayment.FillSubVoucherName(VoucherType);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }

        public async Task<JsonResult> FillBankType(int AccountCode)
        {
            var JSON = await _IBankPayment.FillBankType(AccountCode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillIntrument(string VoucherType)
        {
            var JSON = await _IBankPayment.FillIntrument(VoucherType);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillModeofAdjust(string VoucherType)
        {
            var JSON = await _IBankPayment.FillModeofAdjust(VoucherType);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillCostCenterName()
        {
            var JSON = await _IBankPayment.FillCostCenterName();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillEntryID(int YearCode,string VoucherDate)
        {
            var JSON = await _IBankPayment.FillEntryID(YearCode, VoucherDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillCurrency()
        {
            var JSON = await _IBankPayment.FillCurrency();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }

        public async Task<JsonResult> FillPONO(string accountcode, string VoucherDate)
        {
            var JSON = await _IBankPayment.FillPONO( accountcode, VoucherDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetPODetail(int PONO,string accountcode, string VoucherDate)
        {
            var JSON = await _IBankPayment.GetPODetail(PONO,  accountcode,  VoucherDate);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetPODate(int PONO, string accountcode, string VoucherDate, string POYearCode)
        {
            var JSON = await _IBankPayment.GetPODate(PONO, accountcode, VoucherDate,POYearCode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> CheckAmountBeforeSave(string VoucherDate, int YearCode, int AgainstVoucherYearCode, int AgainstVoucherEntryId, string AgainstVoucherNo, int AccountCode)
        {
            var JSON = await _IBankPayment.CheckAmountBeforeSave(VoucherDate, YearCode, AgainstVoucherYearCode, AgainstVoucherEntryId, AgainstVoucherNo, AccountCode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }

        public IActionResult AddBankPaymentDetail(BankPaymentModel model)
        {
            try
            {
                if (model.Mode == "U")
                {

                    _MemoryCache.TryGetValue("KeyBankPaymentGrid", out IList<BankPaymentModel> BankPaymentGrid);

                    var MainModel = new BankPaymentModel();
                    var WorkOrderPGrid = new List<BankPaymentModel>();
                    var OrderGrid = new List<BankPaymentModel>();
                    var ssGrid = new List<BankPaymentModel>();

                    var count = 0;
                    if (model != null)
                    {
                        if (BankPaymentGrid == null)
                        {
                            model.SeqNo = 1;
                            OrderGrid.Add(model);
                        }
                        else
                        {
                            if (model.ModeOfAdjustment.ToLower() == "new ref")
                            {
                                if (BankPaymentGrid.Any(x => (x.LedgerName == model.LedgerName)))
                                {
                                    return StatusCode(207, "Duplicate");
                                }
                            }

                            else
                            {
                                //count = WorkOrderProcessGrid.Count();
                                model.SeqNo = BankPaymentGrid.Count + 1;
                                OrderGrid = BankPaymentGrid.Where(x => x != null).ToList();
                                ssGrid.AddRange(OrderGrid);
                                OrderGrid.Add(model);

                            }

                        }

                        MainModel.BankPaymentGrid = OrderGrid;

                        MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
                        {
                            AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                            SlidingExpiration = TimeSpan.FromMinutes(55),
                            Size = 1024,
                        };

                        _MemoryCache.Set("KeyBankPaymentGrid", MainModel.BankPaymentGrid, cacheEntryOptions);
                    }
                    else
                    {
                        ModelState.TryAddModelError("Error", "Schedule List Cannot Be Empty...!");
                    }
                    return PartialView("_BankPaymentGrid", MainModel);
                }
                else
                {

                    _MemoryCache.TryGetValue("KeyBankPaymentGrid", out IList<BankPaymentModel> BankPaymentGrid);

                    var MainModel = new BankPaymentModel();
                    var WorkOrderPGrid = new List<BankPaymentModel>();
                    var OrderGrid = new List<BankPaymentModel>();
                    var ssGrid = new List<BankPaymentModel>();

                    if (model != null)
                    {
                        if (BankPaymentGrid == null)
                        {
                            model.SeqNo = 1;
                            OrderGrid.Add(model);
                        }
                        else
                        {
                            if (model.BankType.ToLower() == "bank")
                            {
                                if (BankPaymentGrid.Any(x => (x.LedgerName == model.LedgerName) && x.BankType == "Bank"))
                                {
                                    return StatusCode(210, "Duplicate");
                                }
                                else
                                {
                                    model.SeqNo = BankPaymentGrid.Count + 1;
                                    OrderGrid = BankPaymentGrid.Where(x => x != null).ToList();
                                    ssGrid.AddRange(OrderGrid);
                                    OrderGrid.Add(model);

                                }
                            }
                            else if (model.ModeOfAdjustment.ToLower() == "new ref")
                            {
                                if (BankPaymentGrid.Any(x => (x.LedgerName == model.LedgerName) && (x.ModeOfAdjustment == model.ModeOfAdjustment)))
                                {
                                    return StatusCode(207, "Duplicate");
                                }
                                else
                                {
                                    model.SeqNo = BankPaymentGrid.Count + 1;
                                    OrderGrid = BankPaymentGrid.Where(x => x != null).ToList();
                                    ssGrid.AddRange(OrderGrid);
                                    OrderGrid.Add(model);

                                }
                            }
                            else if (model.ModeOfAdjustment.ToLower() == "advance")
                            {
                                if (BankPaymentGrid.Any(x => (x.LedgerName == model.LedgerName) && (x.ModeOfAdjustment == model.ModeOfAdjustment)))
                                {
                                    return StatusCode(208, "Duplicate");
                                }
                                else
                                {
                                    model.SeqNo = BankPaymentGrid.Count + 1;
                                    OrderGrid = BankPaymentGrid.Where(x => x != null).ToList();
                                    ssGrid.AddRange(OrderGrid);
                                    OrderGrid.Add(model);

                                }
                            }
                            else if (model.ModeOfAdjustment.ToLower() == "against ref")
                            {
                                if (BankPaymentGrid.Any(x => (x.LedgerName == model.LedgerName) && x.AgainstVoucherNo == model.AgainstVoucherNo))
                                {
                                    return StatusCode(209, "Duplicate");
                                }
                                else
                                {
                                    model.SeqNo = BankPaymentGrid.Count + 1;
                                    OrderGrid = BankPaymentGrid.Where(x => x != null).ToList();
                                    ssGrid.AddRange(OrderGrid);
                                    OrderGrid.Add(model);

                                }
                            }
                        }

                        MainModel.BankPaymentGrid = OrderGrid;

                        MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
                        {
                            AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                            SlidingExpiration = TimeSpan.FromMinutes(55),
                            Size = 1024,
                        };

                        _MemoryCache.Set("KeyBankPaymentGrid", MainModel.BankPaymentGrid, cacheEntryOptions);
                    }
                    else
                    {
                        ModelState.TryAddModelError("Error", "Schedule List Cannot Be Empty...!");
                    }
                    return PartialView("_BankPaymentGrid", MainModel);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IActionResult AddBankPaymentAdjustDetail(List<BankPaymentModel> model)
        {
            try
            {
                var MainModel = new BankPaymentModel();
                var ProductionEntryDetail = new List<BankPaymentModel>();

                MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                    SlidingExpiration = TimeSpan.FromMinutes(55),
                    Size = 1024,
                };

                // Retrieve existing cached data
                _MemoryCache.TryGetValue("KeyBankPaymentGrid", out List<BankPaymentModel> ProductionEntryItemDetail);

                // If cache exists, use it; otherwise, initialize a new list
                if (ProductionEntryItemDetail != null)
                {
                    ProductionEntryDetail = ProductionEntryItemDetail;
                }

                foreach (var item in model)
                {
                    if (item != null)
                    {
                        bool isDuplicate = ProductionEntryDetail.Any(x => x.LedgerName == item.LedgerName && x.InVoiceNo == item.InVoiceNo);
                        if (!isDuplicate)  // Only add if not duplicate
                        {
                            // Assign sequence number correctly
                            item.SeqNo = ProductionEntryDetail.Count + 1;

                            // Swap Type values
                           // item.Type = item.Type.ToLower() == "dr" ? "CR" : "DR";

                            // Add new item to list
                            ProductionEntryDetail.Add(item);
                        }
                    }
                }

                // Update the main model and cache
                MainModel.BankPaymentGrid = ProductionEntryDetail;
                _MemoryCache.Set("KeyBankPaymentGrid", MainModel.BankPaymentGrid, cacheEntryOptions);

                return PartialView("_BankPaymentGrid", MainModel);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<JsonResult> EditItemRows(int SrNO)
        {
            var MainModel = new BankPaymentModel();
            _MemoryCache.TryGetValue("KeyBankPaymentGrid", out IList<BankPaymentModel> GridDetail);
           

            IEnumerable<BankPaymentModel> SSGrid = GridDetail;
            if (GridDetail != null)
            {
                SSGrid = GridDetail.Where(x => x.SeqNo == SrNO);
            }
            string JsonString = JsonConvert.SerializeObject(SSGrid);
            return Json(JsonString);

            //var SAGrid = GridDetail.Where(x => x.SeqNo == SrNO);
            //string JsonString = JsonConvert.SerializeObject(SAGrid);
            //return Json(JsonString);
        }
        public IActionResult DeleteItemRow(int SeqNo, string PopUpData)
        {
            if (PopUpData == "PopUpData")
            {
                var MainModel = new BankPaymentModel();
                _MemoryCache.TryGetValue("KeyBankPaymentGridPopUpData", out List<BankPaymentModel> BankPaymentGrid);
                int Indx = Convert.ToInt32(SeqNo) - 1;

                if (BankPaymentGrid != null && BankPaymentGrid.Count > 0)
                {
                    BankPaymentGrid.RemoveAt(Convert.ToInt32(Indx));
                    Indx = 0;

                    foreach (var item in BankPaymentGrid)
                    {
                        Indx++;
                        // item.SequenceNo = Indx;
                    }
                    MainModel.BankPaymentGrid = BankPaymentGrid;

                    MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
                    {
                        AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                        SlidingExpiration = TimeSpan.FromMinutes(55),
                        Size = 1024,
                    };

                    _MemoryCache.Set("KeyBankPaymentGridPopUpData", MainModel.BankPaymentGrid, cacheEntryOptions);
                }
                return PartialView("_DisplayPopupForPendingVouchers", MainModel);
            }
            else
            {
                var MainModel = new BankPaymentModel();
                _MemoryCache.TryGetValue("KeyBankPaymentGrid", out List<BankPaymentModel> BankPaymentGrid);
                int Indx = Convert.ToInt32(SeqNo) - 1;

                if (BankPaymentGrid != null && BankPaymentGrid.Count > 0)
                {
                    BankPaymentGrid.RemoveAt(Convert.ToInt32(Indx));
                    Indx = 0;

                    foreach (var item in BankPaymentGrid)
                    {
                        Indx++;
                        // item.SequenceNo = Indx;
                    }
                    MainModel.BankPaymentGrid = BankPaymentGrid;

                    MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
                    {
                        AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                        SlidingExpiration = TimeSpan.FromMinutes(55),
                        Size = 1024,
                    };

                    _MemoryCache.Set("KeyBankPaymentGrid", MainModel.BankPaymentGrid, cacheEntryOptions);
                }
                return PartialView("_BankPaymentGrid", MainModel);
            }
        }


        public async Task<IActionResult> BankPaymentDashBoard(string FromDate, string ToDate)
        {
            try
            {
                var model = new BankPaymentModel();
                FromDate = HttpContext.Session.GetString("FromDate");
                ToDate = HttpContext.Session.GetString("ToDate");
                var Result = await _IBankPayment.GetDashBoardData(FromDate, ToDate).ConfigureAwait(true);
                if (Result != null)
                {
                    DataSet ds = Result.Result;
                    if (ds != null && ds.Tables.Count > 0)
                    {
                        var dt = ds.Tables[0];
                        model.BankPaymentGrid = CommonFunc.DataTableToList<BankPaymentModel>(dt, "BankPaymentDashBoard");
                    }
                }
                return View(model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IActionResult> GetDashBoardDetailData(string FromDate, string ToDate)
        {
            var model = new BankPaymentModel();
            model = await _IBankPayment.GetDashBoardDetailData(FromDate, ToDate);
            return PartialView("_BankPaymentDashBoardDetailGrid", model);
        }
        public async Task<IActionResult> GetDashBoardSummaryData(string FromDate, string ToDate)
        {
            var model = new BankPaymentModel();
            model = await _IBankPayment.GetDashBoardSummaryData(FromDate, ToDate);
            return PartialView("_BankPaymentDashBoardGrid", model);
        }
        public async Task<IActionResult> PopUpForPendingVouchers(PopUpDataTable DataTable)
        {
            _MemoryCache.Remove("KeyBankPaymentGridPopUpData");
            MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                SlidingExpiration = TimeSpan.FromMinutes(55),
                Size = 1024,
            };
            var model = await _IBankPayment.PopUpForPendingVouchers(DataTable);
            _MemoryCache.Set("KeyBankPaymentGridPopUpData", model.BankPaymentGrid, cacheEntryOptions);
            return PartialView("_DisplayPopupForPendingVouchers", model);
        }
        public async Task<IActionResult> DeleteByID(int ID, int YearCode, int ActualEntryBy, string EntryByMachine, string ActualEntryDate)
        {
            var Result = await _IBankPayment.DeleteByID(ID, YearCode, ActualEntryBy, EntryByMachine, ActualEntryDate);

            if (Result.StatusText == "Success" || Result.StatusCode == HttpStatusCode.Gone)
            {
                ViewBag.isSuccess = true;
                TempData["410"] = "410";
                //TempData["Message"] = "Data deleted successfully.";
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

            return RedirectToAction("BankPaymentDashBoard");

        }
    }
}
