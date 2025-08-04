using eTactWeb.Services.Interface;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using static eTactWeb.DOM.Models.Common;
using static eTactWeb.Data.Common.CommonFunc;
//using Microsoft.EntityFrameworkCore.Metadata.Internal;
using eTactWeb.DOM.Models;
using eTactWeb.Data.Common;

using System.Net;
using System.Data;
using System.Globalization;
using Microsoft.AspNetCore.Mvc;

namespace eTactWeb.Controllers
{
    public class CreditNoteController : Controller
    {
        private readonly ICreditNote _creditNote;
        private readonly ILogger<CreditNoteController> _logger;
        public IWebHostEnvironment _IWebHostEnvironment { get; }

        public CreditNoteController(ICreditNote creditNote, IWebHostEnvironment IWebHostEnvironment, ILogger<CreditNoteController> logger)
        {
            _creditNote = creditNote;
            _IWebHostEnvironment = IWebHostEnvironment;
            _logger = logger;
        }

        [HttpGet]
        [Route("{controller}/Index")]
        public async Task<IActionResult> CreditNote(int ID, string Mode, int YearCode)
        {
            AccCreditNoteModel model = new AccCreditNoteModel();
            ViewData["Title"] = "Credit Note Details";
            TempData.Clear();
            HttpContext.Session.Remove("KeyCreditNoteGrid");
            HttpContext.Session.Remove("CreditNoteModel");
            HttpContext.Session.Remove("KeyAdjGrid");
            HttpContext.Session.Remove("KeyCreditNotePopupGrid");

            if (model.Mode != "U")
            {
                model.Uid = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                model.ActualEnteredBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                model.ActualEnteredByName = HttpContext.Session.GetString("EmpName");
            }

            if (!string.IsNullOrEmpty(Mode) && ID > 0 && (Mode == "V" || Mode == "U"))
            {
                model = await _creditNote.GetViewByID(ID, YearCode,Mode);
                model.Mode = Mode;
                model.ID = ID;
            }

            MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now.AddMinutes(60),
                SlidingExpiration = TimeSpan.FromMinutes(55),
                Size = 1024,
            };
            model.FinFromDate = HttpContext.Session.GetString("FromDate");
            model.FinToDate = HttpContext.Session.GetString("ToDate");
            model.CreditNoteYearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
            model.CC = HttpContext.Session.GetString("Branch");
            string serializedGrid = JsonConvert.SerializeObject(model.AccCreditNoteDetails);
            HttpContext.Session.SetString("KeyCreditNoteGrid", serializedGrid);
            var adjGrid = model.adjustmentModel == null ? new AdjustmentModel() : model.adjustmentModel;
            string serializedAdjGrid = JsonConvert.SerializeObject(adjGrid);
            HttpContext.Session.SetString("KeyAdjGrid", serializedAdjGrid);
            var taxGrid = model.TaxDetailGridd == null ? new List<TaxModel>() : model.TaxDetailGridd;
            string serializedTaxGrid = JsonConvert.SerializeObject(taxGrid);
            HttpContext.Session.SetString("KeyTaxGrid", serializedTaxGrid);
            string serializedCreditGrid = JsonConvert.SerializeObject(model.AccCreditNoteDetails);
            HttpContext.Session.SetString("CreditNoteModel", serializedCreditGrid);
            HttpContext.Session.SetString("CreditNote", JsonConvert.SerializeObject(model));
            return View(model);
        }

        public IActionResult AddCreditNoteDetail(AccCreditNoteDetail model)
        {
            try
            {
                string modelJson = HttpContext.Session.GetString("KeyCreditNoteGrid");
                List<AccCreditNoteDetail> creditNoteGrid = new List<AccCreditNoteDetail>();
                if (!string.IsNullOrEmpty(modelJson))
                {
                    creditNoteGrid = JsonConvert.DeserializeObject<List<AccCreditNoteDetail>>(modelJson);
                }
                string modelCreditJson = HttpContext.Session.GetString("CreditNoteModel");
                List<AccCreditNoteDetail> creditNotelModel = new List<AccCreditNoteDetail>();
                if (!string.IsNullOrEmpty(modelCreditJson))
                {
                    creditNotelModel = JsonConvert.DeserializeObject<List<AccCreditNoteDetail>>(modelCreditJson);
                }

                var MainModel = new AccCreditNoteModel();
                var creditNoteDetail = new List<AccCreditNoteDetail>();
                var rangeSaleBillGrid = new List<AccCreditNoteDetail>();

                if (model != null)
                {
                    if (creditNoteGrid == null)
                    {
                        //model.SeqNo = 1;
                        creditNoteDetail.Add(model);
                    }
                    else
                    {
                        if (creditNoteGrid.Any(x => x.ItemCode == model.ItemCode && x.StoreId == model.StoreId))
                        {
                            return StatusCode(207, "Duplicate");
                        }

                        //model.SeqNo = SaleBillDetail.Count + 1;
                        creditNoteDetail = creditNoteGrid.Where(x => x != null).ToList();
                        rangeSaleBillGrid.AddRange(creditNoteDetail);
                        creditNoteDetail.Add(model);

                    }
                    //MainModel = BindItem4Grid(model);
                    //saleBillDetail = saleBillDetail.OrderBy(item => item.SeqNo).ToList();
                    MainModel.AccCreditNoteDetails = creditNoteDetail;
                    MainModel.ItemDetailGrid = creditNoteDetail;

                    string serializedGrid = JsonConvert.SerializeObject(MainModel.AccCreditNoteDetails);
                    HttpContext.Session.SetString("KeyCreditNoteGrid", serializedGrid);

                    MainModel.AccCreditNoteDetails = creditNoteDetail;
                    MainModel.ItemDetailGrid = creditNoteDetail;
                    string serializedCreditGrid = JsonConvert.SerializeObject(MainModel);
                    HttpContext.Session.SetString("CreditNoteModel", serializedCreditGrid);
                    MainModel.AccCreditNoteDetails = creditNoteDetail;
                }
                else
                {
                    ModelState.TryAddModelError("Error", "Credit note Cannot Be Empty...!");
                }
                return PartialView("_CreditNoteGrid", MainModel);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IActionResult> DeleteByID(int ID, int YC,int accountCode, string entryByMachineName)
        {
            var Result = await _creditNote.DeleteByID(ID, YC,accountCode, entryByMachineName).ConfigureAwait(false);

            if (Result.StatusText == "Deleted" || Result.StatusCode == HttpStatusCode.Gone || Result.StatusText == "Success")
            {
                ViewBag.isSuccess = true;
                TempData["410"] = "410";
            }
            else
            {
                ViewBag.isSuccess = false;
                TempData["500"] = "500";
            }
            return RedirectToAction("CNDashboard");
        }



        [HttpPost]
        [Route("{controller}/Index")]
        public async Task<IActionResult> CreditNote(AccCreditNoteModel model)
        {
            try
            {
                var CNGrid = new DataTable();
                DataTable DTAgainstBillDetail = new();
                DataTable TaxDetailDT = null;
                DataTable AdjDetailDT = null;
                DataTable DrCrDetailDT = null;

                string modelJson = HttpContext.Session.GetString("CreditNoteModel");
                AccCreditNoteModel MainModel = new AccCreditNoteModel();
                if (!string.IsNullOrEmpty(modelJson))
                {
                    MainModel = JsonConvert.DeserializeObject<AccCreditNoteModel>(modelJson);
                }
                string modelCreditJson = HttpContext.Session.GetString("KeyCreditNotePopupGrid");
                List<AccCreditNoteAgainstBillDetail> CreditNoteAgainstBillDetail = new List<AccCreditNoteAgainstBillDetail>();
                if (!string.IsNullOrEmpty(modelCreditJson))
                {
                    CreditNoteAgainstBillDetail = JsonConvert.DeserializeObject<List<AccCreditNoteAgainstBillDetail>>(modelCreditJson);
                }
                string modelCreditNoteJson = HttpContext.Session.GetString("KeyCreditNoteGrid");
                List<AccCreditNoteDetail> CreditNoteDetail = new List<AccCreditNoteDetail>();
                if (!string.IsNullOrEmpty(modelCreditNoteJson))
                {
                    CreditNoteDetail = JsonConvert.DeserializeObject<List<AccCreditNoteDetail>>(modelCreditNoteJson);
                }
                string modelTaxJson = HttpContext.Session.GetString("KeyTaxGrid");
                List<TaxModel> TaxGrid = new List<TaxModel>();
                if (!string.IsNullOrEmpty(modelTaxJson))
                {
                    TaxGrid = JsonConvert.DeserializeObject<List<TaxModel>>(modelTaxJson);
                }
                string modelDrCrJson = HttpContext.Session.GetString("KeyDrCrGrid");
                List<DbCrModel> DrCrGrid = new List<DbCrModel>();
                if (!string.IsNullOrEmpty(modelTaxJson))
                {
                    DrCrGrid = JsonConvert.DeserializeObject<List<DbCrModel>>(modelTaxJson);
                }

                if (CreditNoteDetail == null)
                {
                    ModelState.Clear();
                    ModelState.TryAddModelError("CreditNoteDetail", "Credit Note Grid Should Have Atleast 1 Item...!");
                    return View("CreditNote", model);
                }
                else if (CreditNoteDetail == null)
                {
                    ModelState.Clear();
                    ModelState.TryAddModelError("TaxDetail", "Tax Grid Should Have Atleast 1 Item...!");
                    return View("CreditNote", model);
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
                        CNGrid = GetDetailTable(CreditNoteDetail);
                    }
                    else
                    {
                        model.ActualEnteredBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                        model.ActualEnteredByName = HttpContext.Session.GetString("EmpName");
                        CNGrid = GetDetailTable(CreditNoteDetail);
                    }

                    if (TaxGrid != null && TaxGrid.Count > 0)
                    {
                        TaxDetailDT = GetTaxDetailTable(TaxGrid);
                    }
                    if (DTAgainstBillDetail != null)
                    {
                        DTAgainstBillDetail = GetAgainstDetailTable(CreditNoteAgainstBillDetail);
                    }
                    if (DrCrGrid != null && DrCrGrid.Count > 0)
                    {

                        DrCrDetailDT = CommonController.GetDrCrDetailTable(DrCrGrid);
                    }

                    if (MainModel.adjustmentModel != null && MainModel.adjustmentModel.AdjAdjustmentDetailGrid != null && MainModel.adjustmentModel.AdjAdjustmentDetailGrid.Count > 0)
                    {
                        AdjDetailDT = CommonController.GetAdjDetailTable(MainModel.adjustmentModel.AdjAdjustmentDetailGrid.ToList(), model.CreditNoteEntryId, model.CreditNoteYearCode, model.AccountCode);
                    }
                    string serverFolderPath = Path.Combine(_IWebHostEnvironment.WebRootPath, "Uploads", "SaleBill");
                    if (!Directory.Exists(serverFolderPath))
                    {
                        Directory.CreateDirectory(serverFolderPath);
                    }

                    var Result = await _creditNote.SaveCreditNote(model, CNGrid, TaxDetailDT, DrCrDetailDT, AdjDetailDT, DTAgainstBillDetail);

                    if (Result != null)
                    {
                        if (Result.StatusText == "Success" && Result.StatusCode == HttpStatusCode.OK)
                        {
                            ViewBag.isSuccess = true;
                            TempData["200"] = "200";
                            HttpContext.Session.Remove("KeyCreditNoteGrid");
                            var model1 = new SaleBillModel();
                            model1.adjustmentModel = model1.adjustmentModel ?? new AdjustmentModel();
                            model1.FinFromDate = HttpContext.Session.GetString("FromDate");
                            model1.FinToDate = HttpContext.Session.GetString("ToDate");
                            var yearCodeStr = HttpContext.Session.GetString("YearCode");
                            model1.SaleBillYearCode = !string.IsNullOrEmpty(yearCodeStr) ? Convert.ToInt32(yearCodeStr) : 0;
                            model1.CC = HttpContext.Session.GetString("Branch");
                            var uidStr = HttpContext.Session.GetString("UID");
                            model1.CreatedBy = !string.IsNullOrEmpty(uidStr) ? Convert.ToInt32(uidStr) : 0;
                            //model1.ActualEnteredByName = HttpContext.Session.GetString("EmpName");
                            model1.CreatedBy = Convert.ToInt32(HttpContext.Session.GetString("UID"));
                            HttpContext.Session.Remove("KeyCreditNoteGrid");
                            HttpContext.Session.Remove("CreditNoteModel");
                            return RedirectToAction(nameof(CreditNote), new { Id = 0, Mode = "", YC = 0 });
                        }
                        if (Result.StatusText == "Updated" && Result.StatusCode == HttpStatusCode.Accepted)
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
                            HttpContext.Session.Remove("KeyCreditNoteGrid");
                            HttpContext.Session.Remove("CreditNoteModel");
                            return View(model1);
                        }
                        if (Result.StatusText == "Error" && Result.StatusCode == HttpStatusCode.InternalServerError)
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
                            return View(model);
                        }
                        HttpContext.Session.SetString("CreditNote", JsonConvert.SerializeObject(model));
                    }

                    return View(model);
                }
            }
            catch (Exception ex)
            {
                LogException<CreditNoteController>.WriteException(_logger, ex);


                var _ResponseResult = new ResponseResult()
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    StatusText = "Error",
                    Result = ex
                };

                return View("Error", _ResponseResult);
                //return View(model);
            }
        }

        [HttpGet]
        [Route("{controller}/Dashboard")]
        public async Task<IActionResult> CNDashboard()
        {
            try
            {
                HttpContext.Session.Remove("KeyCreditNoteGrid");
                HttpContext.Session.Remove("KeyTaxGrid");
                var model = new AccCreditNoteDashboard();

                var FromDt = HttpContext.Session.GetString("FromDate");
                model.FinFromDate = Convert.ToDateTime(FromDt).ToString("dd/MM/yyyy");
                //DateTime ToDate = DateTime.Today;
                var ToDt = HttpContext.Session.GetString("ToDate");
                model.FinToDate = Convert.ToDateTime(ToDt).ToString("dd/MM/yyyy");

                model.CreditNoteYearCode = Convert.ToInt32(HttpContext.Session.GetString("YearCode"));
                model.SummaryDetail = "Summary";
                var Result = await _creditNote.GetDashboardData(model.SummaryDetail,ParseFormattedDate(model.FinFromDate.Split(" ")[0]), ParseFormattedDate(model.FinToDate.Split(" ")[0])).ConfigureAwait(true);
                if (Result != null)
                {
                    var _List = new List<TextValue>();
                    DataSet DS = Result.Result;
                    if (DS != null)
                    {
                        var DT = DS.Tables[0].DefaultView.ToTable(true, "AccountName", "AccountCode", "GSTNO", "CreditNoteEntryId", "CreditNoteInvoiceNo", "CreditNoteInvoiceDate", "SubVoucherName"
                                            , "CreditNoteVoucherNo", "CreditNoteVoucherDate", "AgainstSalePurchase", "Taxableamt", "BillAmt", "NetAmt", "ItemService"
                                            , "INVOICETYPE", "EInvNo", "EinvGenerated", "MachineName", "ActualEntryByEmp", "ActualEntryDate", "LastUpdatedByEmp", "LastUpdationDate"
                                            , "RoundoffType", "CustVendAddress", "StateNameofSupply", "StateCode", "CityofSupply", "CountryOfSupply"
                                            , "PaymentTerm", "Transporter", "ToatlDiscountPercent", "RoundOffAmt", "Vehicleno", "TotalDiscountAmount", "Remark"
                                            , "CC", "Uid", "EntryFreezToAccounts", "BalanceSheetClosed", "AttachmentFilePath1", "AttachmentFilePath2"
                                            , "AttachmentFilePath3", "CreditNoteYearCode"
                            );
                        model.CreditNoteDashboard = CommonFunc.DataTableToList<AccCreditNoteDashboard>(DT, "CreditNoteSummTable");
                    }
                }
                return View(model);
            }
            catch (Exception ex)
            {
                throw ex;
            }

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


        private static DataTable GetDetailTable(IList<AccCreditNoteDetail> DetailList)
        {
            var DTSSGrid = new DataTable();

            DTSSGrid.Columns.Add("CreditNoteEntryId", typeof(int));
            DTSSGrid.Columns.Add("CreditNoteYearCode", typeof(int));
            DTSSGrid.Columns.Add("Itemcode", typeof(int));
            DTSSGrid.Columns.Add("HSNNo", typeof(string));
            DTSSGrid.Columns.Add("BillQty", typeof(float));
            DTSSGrid.Columns.Add("RejectedQty", typeof(float));
            DTSSGrid.Columns.Add("Unit", typeof(string));
            DTSSGrid.Columns.Add("AltQty", typeof(float));
            DTSSGrid.Columns.Add("AltUnit", typeof(string));
            DTSSGrid.Columns.Add("CRNRate", typeof(float));
            DTSSGrid.Columns.Add("BillRate", typeof(float));
            DTSSGrid.Columns.Add("UnitRate", typeof(string));
            DTSSGrid.Columns.Add("AltRate", typeof(float));
            DTSSGrid.Columns.Add("NoOfCase", typeof(float));
            DTSSGrid.Columns.Add("CostcenetrId", typeof(int));
            DTSSGrid.Columns.Add("DocAccountCode", typeof(int));
            DTSSGrid.Columns.Add("ItemAmount", typeof(float));
            DTSSGrid.Columns.Add("DiscountPer", typeof(float));
            DTSSGrid.Columns.Add("DiscountAmt", typeof(float));
            DTSSGrid.Columns.Add("StoreId", typeof(int));
            DTSSGrid.Columns.Add("itemSize", typeof(string));
            DTSSGrid.Columns.Add("ItemDescription", typeof(string));
            DTSSGrid.Columns.Add("Remark", typeof(string));

            foreach (var Item in DetailList)
            {
                string uniqueString = Guid.NewGuid().ToString();
                DTSSGrid.Rows.Add(
                    new object[]
                    {
                    1,
                        2025,
                        Item.ItemCode,
                        Item.HSNNo ?? string.Empty,
                        Item.BillQty,
                        Item.RejectedQty,
                        Item.Unit ?? string.Empty,
                        Item.AltQty,
                        Item.AltUnit ?? string.Empty,
                        Item.CRNRate,
                        Item.BillRate,
                        Item.UnitRate ?? string.Empty,
                        Item.AltRate,
                        Item.NoOfCase,
                        Item.CostCenterId,
                        Item.DocAccountCode,
                        Item.ItemAmount,
                        Item.DiscountPer,
                        Item.DiscountAmt,
                        Item.StoreId,
                        Item.ItemSize ?? string.Empty,
                        Item.ItemDescription ?? string.Empty,
                        Item.Remark ?? string.Empty
                    });
            }
            DTSSGrid.Dispose();
            return DTSSGrid;
        }
        private static DataTable GetAgainstDetailTable(IList<AccCreditNoteAgainstBillDetail> DetailList)
        {
            var DTSSGrid = new DataTable();

            DTSSGrid.Columns.Add("CreditNoteEntryId", typeof(int));
            DTSSGrid.Columns.Add("CreditNoteYearCode", typeof(int));
            DTSSGrid.Columns.Add("CreditNoteInvoiceNo", typeof(string));
            DTSSGrid.Columns.Add("CreditNoteVoucherNo", typeof(string));
            DTSSGrid.Columns.Add("AgainstSalebillBillNo", typeof(string));
            DTSSGrid.Columns.Add("AgainstSaleBillYearCode", typeof(int));
            DTSSGrid.Columns.Add("AgainstSaleBilldate", typeof(string));
            DTSSGrid.Columns.Add("AgainstSaleBillEntryId", typeof(int));
            DTSSGrid.Columns.Add("AgainstSalebillVoucherNo", typeof(string));
            DTSSGrid.Columns.Add("SaleBillTYpe", typeof(string));
            DTSSGrid.Columns.Add("AgainstPurchasebillBillNo", typeof(string));
            DTSSGrid.Columns.Add("AgainstPurchaseBillYearCode", typeof(int));
            DTSSGrid.Columns.Add("AgainstPurchaseBilldate", typeof(string));
            DTSSGrid.Columns.Add("AgainstPurchaseBillEntryId", typeof(int));
            DTSSGrid.Columns.Add("AgainstPurchaseVoucherNo", typeof(string));
            DTSSGrid.Columns.Add("PurchaseBilltype", typeof(string));
            DTSSGrid.Columns.Add("CreditNoteItemCode", typeof(int));
            DTSSGrid.Columns.Add("BillItemCode", typeof(int));
            DTSSGrid.Columns.Add("BillQty", typeof(float));
            DTSSGrid.Columns.Add("Unit", typeof(string));
            DTSSGrid.Columns.Add("AltQty", typeof(float));
            DTSSGrid.Columns.Add("AltUnit", typeof(string));
            DTSSGrid.Columns.Add("BillRate", typeof(float));
            DTSSGrid.Columns.Add("DiscountPer", typeof(float));
            DTSSGrid.Columns.Add("DiscountAmt", typeof(float));
            DTSSGrid.Columns.Add("Itemsize", typeof(string));
            DTSSGrid.Columns.Add("Amount", typeof(float));
            DTSSGrid.Columns.Add("PONO", typeof(string));
            DTSSGrid.Columns.Add("PODate", typeof(string));
            DTSSGrid.Columns.Add("POEntryId", typeof(int));
            DTSSGrid.Columns.Add("POYearCode", typeof(int));
            DTSSGrid.Columns.Add("PoRate", typeof(float));
            DTSSGrid.Columns.Add("poammno", typeof(string));
            DTSSGrid.Columns.Add("SONO", typeof(string));
            DTSSGrid.Columns.Add("SOYearcode", typeof(int));
            DTSSGrid.Columns.Add("SODate", typeof(string));
            DTSSGrid.Columns.Add("CustOrderNo", typeof(string));
            DTSSGrid.Columns.Add("SOEntryId", typeof(int));
            DTSSGrid.Columns.Add("BatchNo", typeof(string));
            DTSSGrid.Columns.Add("UniqueBatchNo", typeof(string));

            foreach (var Item in DetailList)
            {
                string uniqueString = Guid.NewGuid().ToString();
                DTSSGrid.Rows.Add(
                    new object[]
                    {
                    1,
                        2025,
                    Item.CreditNoteInvoiceNo,
                    Item.CreditNoteVoucherNo,
                    Item.AgainstSaleBillBillNo,
                    Item.AgainstSaleBillYearCode,
                    Item.AgainstSaleBillDate == null ? string.Empty : (Item.AgainstSaleBillDate.Split(" ")[0]),
                    Item.AgainstSaleBillEntryId,
                    Item.AgainstSaleBillVoucherNo,
                    Item.SaleBillType,
                    Item.AgainstPurchaseBillBillNo,
                    Item.AgainstPurchaseBillYearCode,
                    Item.AgainstPurchaseBillDate == null ? string.Empty : (Item.AgainstPurchaseBillDate.Split(" ")[0]),
                    Item.AgainstPurchaseBillEntryId,
                    Item.AgainstPurchaseVoucherNo,
                    Item.PurchaseBillType,
                    Item.ItemCode, //CreditNoteItemCode
                    Item.ItemCode, // BillItemCode
                    Item.BillQty,
                    Item.Unit,
                    Item.AltQty,
                    Item.AltUnit,
                    Item.BillRate,
                    Item.DiscountPer,
                    Item.DiscountAmt,
                    Item.ItemSize,
                    Item.Amount,
                    Item.PONO,
                    Item.PODate == null ? string.Empty :(Item.PODate.Split(" ")[0]),
                    Item.POEntryId,
                    Item.POYearCode,
                    Item.PoRate,
                    Item.PoAmmNo,
                    Item.SONO,
                    Item.SOYearCode,
                    Item.SODate == null ? string.Empty : (Item.SODate.Split(" ")[0]),
                    Item.CustOrderNo,
                    Item.SOEntryId,
                    Item.BatchNo,
                    Item.UniqueBatchNo
                    });
            }
            DTSSGrid.Dispose();
            return DTSSGrid;
        }

        public IActionResult DeleteItemRow(int itemCode,string Mode)
        {
            var MainModel = new AccCreditNoteModel();
            if (Mode == "U")
            {
                string modelJson = HttpContext.Session.GetString("KeyCreditNoteGrid");
                List<AccCreditNoteDetail> creditNoteDetail = new List<AccCreditNoteDetail>();
                if (!string.IsNullOrEmpty(modelJson))
                {
                    creditNoteDetail = JsonConvert.DeserializeObject<List<AccCreditNoteDetail>>(modelJson);
                }

                if (creditNoteDetail != null && creditNoteDetail.Count > 0)
                {
                    creditNoteDetail.RemoveAll(x => x.ItemCode == itemCode);

                    MainModel.AccCreditNoteDetails = creditNoteDetail;

                    string serializedGrid = JsonConvert.SerializeObject(MainModel.AccCreditNoteDetails);
                    HttpContext.Session.SetString("KeyCreditNoteGrid", serializedGrid);
                }
            }
            else
            {
                string modelJson = HttpContext.Session.GetString("KeyCreditNoteGrid");
                List<AccCreditNoteDetail> creditNoteDetail = new List<AccCreditNoteDetail>();
                if (!string.IsNullOrEmpty(modelJson))
                {
                    creditNoteDetail = JsonConvert.DeserializeObject<List<AccCreditNoteDetail>>(modelJson);
                }
                
                if (creditNoteDetail != null && creditNoteDetail.Count > 0)
                {
                    creditNoteDetail.RemoveAll(x => x.ItemCode == itemCode);
                    MainModel.AccCreditNoteDetails = creditNoteDetail;

                    string serializedGrid = JsonConvert.SerializeObject(MainModel.AccCreditNoteDetails);
                    HttpContext.Session.SetString("KeyCreditNoteGrid", serializedGrid);
                }
            }

            return PartialView("_CreditNoteGrid", MainModel);
        }

        public async Task<JsonResult> EditItemRows(int itemCode)
        {
            var MainModel = new AccCreditNoteModel();
            string modelJson = HttpContext.Session.GetString("KeyCreditNoteGrid");
            List<AccCreditNoteDetail> creditNoteGrid = new List<AccCreditNoteDetail>();
            if (!string.IsNullOrEmpty(modelJson))
            {
                creditNoteGrid = JsonConvert.DeserializeObject<List<AccCreditNoteDetail>>(modelJson);
            }
            string modelPopupGridJson = HttpContext.Session.GetString("KeyCreditNotePopupGrid");
            List<AccCreditNoteAgainstBillDetail> creditNotePopupGrid = new List<AccCreditNoteAgainstBillDetail>();
            if (!string.IsNullOrEmpty(modelPopupGridJson))
            {
                creditNotePopupGrid = JsonConvert.DeserializeObject<List<AccCreditNoteAgainstBillDetail>>(modelPopupGridJson);
            }

            var combinedData = new
            {
                CreditNoteGrid = creditNoteGrid?.Where(x => x.ItemCode == itemCode),
                CreditNotePopupGrid = creditNotePopupGrid
            };
            string JsonString = JsonConvert.SerializeObject(combinedData);
            return Json(JsonString);
        }
        private static DataTable GetDetailFromPopup(List<AccCreditNoteAgainstBillDetail> List)
        {
            try
            {
                DataTable table = new();
                table.Columns.Add("CreditNoteEntryId", typeof(int));
                table.Columns.Add("CreditNoteYearCode", typeof(int));
                table.Columns.Add("CreditNoteInvoiceNo", typeof(string));
                table.Columns.Add("CreditNoteVoucherNo", typeof(string));
                table.Columns.Add("AgainstSalebillBillNo", typeof(string));
                table.Columns.Add("AgainstSaleBillYearCode", typeof(int));
                table.Columns.Add("AgainstSaleBilldate", typeof(string));
                table.Columns.Add("AgainstSaleBillEntryId", typeof(int));
                table.Columns.Add("AgainstSalebillVoucherNo", typeof(string));
                table.Columns.Add("SaleBillTYpe", typeof(string));
                table.Columns.Add("AgainstPurchasebillBillNo", typeof(string));
                table.Columns.Add("AgainstPurchaseBillYearCode", typeof(int));
                table.Columns.Add("AgainstPurchaseBilldate", typeof(string));
                table.Columns.Add("AgainstPurchaseBillEntryId", typeof(int));
                table.Columns.Add("AgainstPurchaseVoucherNo", typeof(string));
                table.Columns.Add("PurchaseBilltype", typeof(string));
                table.Columns.Add("CreditNoteItemCode", typeof(int));
                table.Columns.Add("BillItemCode", typeof(int));
                table.Columns.Add("BillQty", typeof(float));
                table.Columns.Add("Unit", typeof(string));
                table.Columns.Add("AltQty", typeof(float));
                table.Columns.Add("AltUnit", typeof(string));
                table.Columns.Add("BillRate", typeof(float));
                table.Columns.Add("DiscountPer", typeof(float));
                table.Columns.Add("DiscountAmt", typeof(float));
                table.Columns.Add("Itemsize", typeof(string));
                table.Columns.Add("Amount", typeof(float));
                table.Columns.Add("PONO", typeof(string));
                table.Columns.Add("PODate", typeof(string));
                table.Columns.Add("POEntryId", typeof(int));
                table.Columns.Add("POYearCode", typeof(int));
                table.Columns.Add("PoRate", typeof(float));
                table.Columns.Add("poammno", typeof(string));
                table.Columns.Add("SONO", typeof(string));
                table.Columns.Add("SOYearcode", typeof(int));
                table.Columns.Add("SODate", typeof(string));
                table.Columns.Add("CustOrderNo", typeof(string));
                table.Columns.Add("SOEntryId", typeof(int));
                table.Columns.Add("BatchNo", typeof(string));
                table.Columns.Add("UniqueBatchNo", typeof(string));

                foreach (AccCreditNoteAgainstBillDetail Item in List)
                {
                    table.Rows.Add(
                        new object[]
                        {
                      1,
                     2025,
                    Item.CreditNoteInvoiceNo ?? string.Empty,
                   Item.CreditNoteVoucherNo ?? string.Empty,
                   Item.AgainstSaleBillBillNo ?? string.Empty,
                   Item.AgainstSaleBillYearCode,
                   Item.AgainstSaleBillDate == null ? string.Empty : ParseFormattedDate(Item.AgainstSaleBillDate),
                   Item.AgainstSaleBillEntryId,
                   Item.AgainstSaleBillVoucherNo ?? string.Empty,
                   Item.SaleBillType ?? string.Empty,
                   Item.AgainstPurchaseBillBillNo ?? string.Empty,
                   Item.AgainstPurchaseBillYearCode,
                   Item.AgainstPurchaseBillDate == null ? string.Empty : ParseFormattedDate(Item.AgainstPurchaseBillDate),
                   Item.AgainstPurchaseBillEntryId,
                   Item.AgainstPurchaseVoucherNo ?? string.Empty,
                   Item.PurchaseBillType ?? string.Empty,
                   Item.ItemCode,// CreditNoteItemCode
                   1, //BillItemCode,
                   Item.BillQty,
                   Item.Unit ?? string.Empty,
                   Item.AltQty,
                   Item.AltUnit ?? string.Empty,
                   Item.BillRate,
                   Item.DiscountPer,
                   Item.DiscountAmt,
                   Item.ItemSize ?? string.Empty,
                   Item.Amount,
                   Item.PONO ?? string.Empty,
                   Item.PODate == null ? string.Empty : ParseFormattedDate(Item.PODate),
                   Item.POEntryId,
                   Item.POYearCode,
                   Item.PoRate,
                   Item.PoAmmNo ?? string.Empty,
                   Item.SONO ?? string.Empty,
                   Item.SOYearCode,
                   Item.SODate == null ? string.Empty : ParseFormattedDate(Item.SODate),
                   Item.CustOrderNo ?? string.Empty,
                   Item.SOEntryId,
                   Item.BatchNo ?? string.Empty,
                   Item.UniqueBatchNo ?? string.Empty
                        });
                }

                return table;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<JsonResult> FillDetailFromPopupGrid(List<AccCreditNoteAgainstBillDetail> model, int itemCode, int popCt)
        {
            var dataGrid = GetDetailFromPopup(model);
            var JSON = await _creditNote.FillDetailFromPopupGrid(dataGrid, itemCode, popCt);
            string JsonString = JsonConvert.SerializeObject(JSON);
            string serializedGrid = JsonConvert.SerializeObject(model);
            HttpContext.Session.SetString("KeyCreditNotePopupGrid", serializedGrid);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetHSNUNIT(int itemCode)
        {
            var JSON = await _creditNote.GetHSNUNIT(itemCode);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> NewEntryId(int YearCode,string CreditNoteVoucherDate,string SubVoucherName)
        {
            var JSON = await _creditNote.NewEntryId(YearCode,CreditNoteVoucherDate,SubVoucherName);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillCustomerName(string againstSalePurchase)
        {
            var JSON = await _creditNote.FillCustomerName(againstSalePurchase);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        //"AgainstSalePurchase": $('#AgainstSalePurchase').val(), "fromBillDate": $('#fromBillDate').val(), "toBillDate": $('#toBillDate').val(), "accountCode": $('#AccountCode').val(), "yearCode": $('#CreditNoteYearCode').val()
        public async Task<JsonResult> FillCreditNotePopUp(string againstSalePurchase, string fromBillDate, string toBillDate, int itemCode, int accountCode, int yearCode,string showAllBill)
        {
            fromBillDate = ParseFormattedDate(fromBillDate);
            toBillDate = ParseFormattedDate(toBillDate);
            var JSON = await _creditNote.FillCreditNotePopUp(againstSalePurchase, fromBillDate, toBillDate, itemCode, accountCode, yearCode,showAllBill);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> FillItems(string fromSaleBillDate, string toSaleBillDate, int accountCode, string showAllItems)
        {
            fromSaleBillDate = ParseFormattedDate(fromSaleBillDate);
            toSaleBillDate = ParseFormattedDate(toSaleBillDate);
            var JSON = await _creditNote.FillItems(fromSaleBillDate, toSaleBillDate, accountCode, showAllItems);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public async Task<JsonResult> GetCostCenter()
        {
            var JSON = await _creditNote.GetCostCenter();
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
    }
}
