using eTactWeb.DOM.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using System.Data.SqlClient;
using System.Runtime.Caching;
using System.Text.Json;
using static eTactWeb.DOM.Models.Common;
using static eTactWeb.Data.Common.CommonFunc;
using eTactWeb.Services.Interface;
using System.Data;
using System.Globalization;
using eTactWeb.Data.Common;

namespace eTactWeb.Controllers
{
    public class CommonController : Controller
    {
        private readonly IWebHostEnvironment _IWebHostEnvironment;
        private readonly IConfiguration iconfiguration;
        public CommonController(ILogger<CommonController> logger, IDataLogic iDataLogic)
        {
            Logger = logger;
            IDataLogic = iDataLogic;
        }

        public IDataLogic IDataLogic { get; }
        public ILogger<CommonController> Logger { get; }
        public async Task<JsonResult> GetServerDate()
        {
            try
            {
                var now = DateTime.Now;

                // if you need your CommonFunc logic
                var parsedDate = CommonFunc.ParseFormattedDate(now.ToString());

                // return awaited Task
                return await Task.FromResult(Json(now.ToString("yyyy-MM-dd")));
            }
            catch (Exception ex)
            {
                // Optional: return error as JSON
                return await Task.FromResult(Json(new { error = ex.Message }));
            }
        }
        public void StoreInSession(string sessionKey, object sessionObject)
        {
            string serializedObject = JsonConvert.SerializeObject(sessionObject);
            HttpContext.Session.SetString(sessionKey, serializedObject);
        }

        #region For Dbit Credit Grid
        public async Task<JsonResult> GetDbCrDataGrid(string PageName, int docAccountCode, int AccountCode, decimal? BillAmt, decimal? NetAmt)
        {
            dynamic MainModel = new DirectPurchaseBillModel();
            dynamic TaxGrid = new List<TaxModel>();
            dynamic TdsGrid = new List<TDSModel>();

            string modelTaxJson = HttpContext.Session.GetString("KeyTaxGrid");
            if (!string.IsNullOrEmpty(modelTaxJson))
            {
                TaxGrid = JsonConvert.DeserializeObject<List<TaxModel>>(modelTaxJson);
            }

            string modelTDSJson = HttpContext.Session.GetString("KeyTDSGrid");
            if (!string.IsNullOrEmpty(modelTDSJson))
            {
                TdsGrid = JsonConvert.DeserializeObject<List<TDSModel>>(modelTDSJson);
            }

            DataTable DbCrGridd = new DataTable();
            DataTable TaxGridd = new DataTable();
            DataTable TdsGridd = new DataTable();

            if (PageName == "ItemList")
            {
                MainModel = new SaleOrderModel();
            }
            else if (PageName == "PurchaseOrder")
            {
                MainModel = new PurchaseOrderModel();
            }
            else if (PageName == "DirectPurchaseBill")
            {
                MainModel = new DirectPurchaseBillModel();
            }
            else if (PageName == "PurchaseBill")
            {
                MainModel = new PurchaseBillModel();
            }
            else if (PageName == "SaleInvoice")
            {
                MainModel = new SaleBillModel();
            }
            else if (PageName == "SaleBillOnCounter")
            {
                MainModel = new SaleBillModel();
            }
            else if (PageName == "CreditNote")
            {
                MainModel = new AccCreditNoteModel();
            }
            else if (PageName == "PurchaseRejection")
            {
                MainModel = new AccPurchaseRejectionModel();
            }
            else if (PageName == "SaleRejection")
            {
                MainModel = new SaleRejectionModel();
            }
            else if (PageName == "JobWorkIssue")
            {
                MainModel = new JobWorkIssueModel();
            }

            //if (HttpContext.Session.GetString(PageName) != null)
            //{
            if (PageName == "ItemList")
            {
                MainModel.ItemDetailGrid = JsonConvert.DeserializeObject<List<ItemDetail>>(HttpContext.Session.GetString(PageName));
            }
            else if (PageName == "PurchaseOrder")
            {
                string modelJson = HttpContext.Session.GetString("PurchaseOrder");
                if (!string.IsNullOrEmpty(modelJson))
                {
                    MainModel = JsonConvert.DeserializeObject<PurchaseOrderModel>(modelJson);
                }
            }
            else if (PageName == "DirectPurchaseBill")
            {
                string modelJson = HttpContext.Session.GetString("DirectPurchaseBill");
                if (!string.IsNullOrEmpty(modelJson))
                {
                    MainModel = JsonConvert.DeserializeObject<DirectPurchaseBillModel>(modelJson);
                }
                DbCrGridd = GetDbCrDetailTable(MainModel);
                TdsGridd = GetTDSDetailTableForDPB(TdsGrid, MainModel);
            }
            else if (PageName == "PurchaseBill")
            {
                string modelJson = HttpContext.Session.GetString("PurchaseBill");
                if (!string.IsNullOrEmpty(modelJson))
                {
                    MainModel = JsonConvert.DeserializeObject<PurchaseBillModel>(modelJson);
                }
                DbCrGridd = GetPbDbCrDetailTable(MainModel);
                TdsGridd = GetTDSDetailTableForPB(TdsGrid, MainModel);
            }
            else if (PageName == "SaleInvoice")
            {
                string modelJson = HttpContext.Session.GetString("SaleBillModel");
                if (!string.IsNullOrEmpty(modelJson))
                {
                    MainModel = JsonConvert.DeserializeObject<SaleBillModel>(modelJson);
                }

                DbCrGridd = GetSbDbCrDetailTable(MainModel);
                //TdsGridd = GetTDSDetailTableForDPB(TdsGrid, MainModel);
            }
            else if (PageName == "SaleBillOnCounter")
            {
                string modelJson = HttpContext.Session.GetString("SaleBillModel");
                if (!string.IsNullOrEmpty(modelJson))
                {
                    MainModel = JsonConvert.DeserializeObject<SaleBillModel>(modelJson);
                }

                DbCrGridd = GetSbDbCrDetailTable(MainModel);
                //TdsGridd = GetTDSDetailTableForDPB(TdsGrid, MainModel);
            }
            else if (PageName == "CreditNote")
            {
                string modelJson = HttpContext.Session.GetString("CreditNoteModel");
                if (!string.IsNullOrEmpty(modelJson))
                {
                    MainModel = JsonConvert.DeserializeObject<AccCreditNoteModel>(modelJson);
                }
                MainModel.AccountCode = AccountCode;
                DbCrGridd = GetCNDbCrDetailTable(MainModel);
                HttpContext.Session.SetString("CreditNoteModel", JsonConvert.SerializeObject((AccCreditNoteModel)MainModel));
                //TdsGridd = GetTDSDetailTableForDPB(TdsGrid, MainModel);
            }
            else if (PageName == "PurchaseRejection")
            {
                string modelJson = HttpContext.Session.GetString("PurchaseRejectionModel");
                if (!string.IsNullOrEmpty(modelJson))
                {
                    MainModel = JsonConvert.DeserializeObject<AccPurchaseRejectionModel>(modelJson);
                }
                DbCrGridd = GetPRDbCrDetailTable(MainModel);
                //TdsGridd = GetTDSDetailTableForDPB(TdsGrid, MainModel);
            }
            else if (PageName == "SaleRejection")
            {
                string modelJson = HttpContext.Session.GetString("SaleRejectionModel");
                if (!string.IsNullOrEmpty(modelJson))
                {
                    MainModel = JsonConvert.DeserializeObject<SaleRejectionModel>(modelJson);
                }
                DbCrGridd = GetSRDbCrDetailTable(MainModel);
            }
            else if (PageName == "JobWorkIssue")
            {
                string modelJson = HttpContext.Session.GetString("JobWorkIssue");
                if (!string.IsNullOrEmpty(modelJson))
                {
                    MainModel = JsonConvert.DeserializeObject<JobWorkIssueModel>(modelJson);
                }
            }
            // TaxGrid=JsonConvert.SerializeObject(MainModel.TaxDetailGridd);
            TaxGridd = GetTaxDetailTable(TaxGrid);
            //}


            var JSON = await IDataLogic.GetDbCrDataGrid(DbCrGridd, TaxGridd, TdsGridd, PageName.ToUpper().ToString(), docAccountCode, AccountCode, BillAmt, NetAmt);
            //var JSON = "success" + PageName + "_" + AccountCode + "_" + NetAmt + "_" + BillAmt;
            string JsonString = JsonConvert.SerializeObject(JSON);
            var DrCrGridd = TransformJsonToDbCrModel(JsonString);
            HttpContext.Session.SetString("KeyDrCrGrid", JsonConvert.SerializeObject(DrCrGridd));
            return Json(JsonString);
        }
        private static DataTable GetTaxDetailTable(List<TaxModel> TaxDetailList)
        {
            DataTable Table = new();
            Table.Columns.Add("SeqNo", typeof(int));
            Table.Columns.Add("[Type]", typeof(string));
            Table.Columns.Add("ItemCode", typeof(int));
            Table.Columns.Add("TaxTypeID", typeof(int));
            Table.Columns.Add("TaxAccountCode", typeof(string));
            Table.Columns.Add("TaxPercentg", typeof(float));
            Table.Columns.Add("AddInTaxable", typeof(char));
            Table.Columns.Add("RountOff", typeof(string));
            Table.Columns.Add("Amount", typeof(float));
            Table.Columns.Add("TaxRefundable", typeof(char));
            Table.Columns.Add("TaxonExp", typeof(string));
            Table.Columns.Add("Remark", typeof(string));

            if (TaxDetailList != null && TaxDetailList.Count > 0)
            {
                var groupedTaxDetails = TaxDetailList
                .GroupBy(item => new { item.TxItemCode, item.TxTaxType, item.TxAccountCode })
                .Select(group => new
                {
                    FirstItem = group.First(),
                    TotalAmount = group.Sum(item => item.TxAmount)
                });

                foreach (var group in groupedTaxDetails)
                {
                    var Item = group.FirstItem;
                    Table.Rows.Add(
                    new object[]
                    {
                    Item.TxSeqNo,
                    Item.TxType ?? string.Empty,
                    Item.TxItemCode,
                    Item.TxTaxType,
                    Item.TxAccountCode,
                    Item.TxPercentg,
                    !string.IsNullOrEmpty(Item.TxAdInTxable) && Item.TxAdInTxable.Length == 1 ? Convert.ToChar(Item.TxAdInTxable) : 'N',
                    Item.TxRoundOff,
                    //Math.Round(Item.TxAmount, 2, MidpointRounding.AwayFromZero),
                    Math.Round(group.TotalAmount, 2, MidpointRounding.AwayFromZero),
                    !string.IsNullOrEmpty(Item.TxRefundable) && Item.TxRefundable.Length == 1 ? Convert.ToChar(Item.TxRefundable) : 'N',
                    Item.TxOnExp,
                    Item.TxRemark,
                        });
                }
            }

            return Table;
        }
        private static DataTable GetTDSDetailTableForDPB(List<TDSModel> TDSDetailList, dynamic MainModel)
        {
            try
            {
                DataTable Table = new();
                Table.Columns.Add("PurchBillEntryId", typeof(int));
                Table.Columns.Add("PurchBillYearCode", typeof(int));
                Table.Columns.Add("SeqNo", typeof(int));
                Table.Columns.Add("InvoiceNo", typeof(string));
                Table.Columns.Add("InvoiceDate", typeof(DateTime));
                Table.Columns.Add("PurchVoucherNo", typeof(string));
                Table.Columns.Add("AccountCode", typeof(int));
                Table.Columns.Add("TaxTypeID", typeof(int));
                Table.Columns.Add("TaxNameCode", typeof(int));
                Table.Columns.Add("TaxPer", typeof(float));
                Table.Columns.Add("RoundOff", typeof(string));
                Table.Columns.Add("TDSAmount", typeof(float));
                Table.Columns.Add("InvBasicAmt", typeof(float));
                Table.Columns.Add("InvNetAmt", typeof(float));
                Table.Columns.Add("Remark", typeof(string));
                Table.Columns.Add("TypePBDirectPBVouch", typeof(string));
                Table.Columns.Add("BankChallanNo", typeof(string));
                Table.Columns.Add("challanDate", typeof(DateTime));
                Table.Columns.Add("BankVoucherNo", typeof(string));
                Table.Columns.Add("BankVoucherDate", typeof(DateTime));
                Table.Columns.Add("BankVouchEntryId", typeof(int));
                Table.Columns.Add("BankYearCode", typeof(int));
                Table.Columns.Add("RemainingAmt", typeof(float));
                Table.Columns.Add("RoundoffAmt", typeof(float));

                if (TDSDetailList != null && TDSDetailList.Count > 0)
                {
                    foreach (TDSModel Item in TDSDetailList)
                    {
                        DateTime InvoiceDate = new DateTime();
                        DateTime challanDate = new DateTime();
                        DateTime BankVoucherDate = new DateTime();
                        string InvoiceDt = "";
                        string challanDt = "";
                        string BankVoucherDt = "";
                        if (MainModel.InvDate != null)
                        {
                            InvoiceDate = DateTime.Parse(MainModel.InvDate, new CultureInfo("en-GB"));
                            InvoiceDt = InvoiceDate.ToString("yyyy/MM/dd");
                        }
                        else
                        {
                            InvoiceDt = DateTime.Today.ToString();
                        }
                        challanDt = DateTime.Today.ToString();
                        BankVoucherDt = DateTime.Today.ToString();

                        Table.Rows.Add(
                            new object[]
                            {
                            MainModel.EntryID > 0 ? MainModel.EntryID : 0,
                            MainModel.YearCode > 0 ? MainModel.YearCode : 0,
                            Item.TDSSeqNo,
                            MainModel.InvoiceNo ?? string.Empty,
                            InvoiceDt,
                            !string.IsNullOrEmpty(MainModel.PurchVouchNo) ? MainModel.PurchVouchNo : string.Empty,
                            MainModel.AccountCode,
                            Item.TDSTaxType,
                            Item.TDSAccountCode,
                            Item.TDSPercentg,
                            Item.TDSRoundOff,
                            Item.TDSAmount,
                            MainModel.ItemNetAmount,
                            MainModel.NetTotal,
                            Item.TDSRemark ?? string.Empty,
                            "DirectPurchaseBill",
                            string.Empty,
                            challanDt,
                            string.Empty,
                            BankVoucherDt,
                            0,
                            0,
                            0f,
                            Item.TDSRoundOffAmt ?? 0,
                            });
                    }
                }
                return Table;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        private static DataTable GetTDSDetailTableForPB(List<TDSModel> TDSDetailList, dynamic MainModel)
        {
            try
            {
                DataTable Table = new();
                Table.Columns.Add("PurchBillEntryId", typeof(int));
                Table.Columns.Add("PurchBillYearCode", typeof(int));
                Table.Columns.Add("SeqNo", typeof(int));
                Table.Columns.Add("InvoiceNo", typeof(string));
                Table.Columns.Add("InvoiceDate", typeof(DateTime));
                Table.Columns.Add("PurchVoucherNo", typeof(string));
                Table.Columns.Add("AccountCode", typeof(int));
                Table.Columns.Add("TaxTypeID", typeof(int));
                Table.Columns.Add("TaxNameCode", typeof(int));
                Table.Columns.Add("TaxPer", typeof(float));
                Table.Columns.Add("RoundOff", typeof(string));
                Table.Columns.Add("TDSAmount", typeof(float));
                Table.Columns.Add("InvBasicAmt", typeof(float));
                Table.Columns.Add("InvNetAmt", typeof(float));
                Table.Columns.Add("Remark", typeof(string));
                Table.Columns.Add("TypePBDirectPBVouch", typeof(string));
                Table.Columns.Add("BankChallanNo", typeof(string));
                Table.Columns.Add("challanDate", typeof(DateTime));
                Table.Columns.Add("BankVoucherNo", typeof(string));
                Table.Columns.Add("BankVoucherDate", typeof(DateTime));
                Table.Columns.Add("BankVouchEntryId", typeof(int));
                Table.Columns.Add("BankYearCode", typeof(int));
                Table.Columns.Add("RemainingAmt", typeof(float));
                Table.Columns.Add("RoundoffAmt", typeof(float));

                if (TDSDetailList != null && TDSDetailList.Count > 0)
                {
                    foreach (TDSModel Item in TDSDetailList)
                    {
                        DateTime InvoiceDate = new DateTime();
                        DateTime challanDate = new DateTime();
                        DateTime BankVoucherDate = new DateTime();
                        string InvoiceDt = "";
                        string challanDt = "";
                        string BankVoucherDt = "";
                        if (MainModel.InvDate != null)
                        {
                            InvoiceDate = DateTime.Parse(MainModel.InvDate, new CultureInfo("en-GB"));
                            InvoiceDt = InvoiceDate.ToString("yyyy/MM/dd");
                        }
                        else
                        {
                            InvoiceDt = DateTime.Today.ToString();
                        }
                        challanDt = DateTime.Today.ToString();
                        BankVoucherDt = DateTime.Today.ToString();

                        Table.Rows.Add(
                            new object[]
                            {
                            MainModel.EntryID > 0 ? MainModel.EntryID : 0,
                            MainModel.YearCode > 0 ? MainModel.YearCode : 0,
                            Item.TDSSeqNo,
                            MainModel.InvNo ?? string.Empty,
                            InvoiceDt,
                            !string.IsNullOrEmpty(MainModel.PurchVouchNo) ? MainModel.PurchVouchNo : string.Empty,
                            MainModel.AccountCode,
                            Item.TDSTaxType,
                            Item.TDSAccountCode,
                            Item.TDSPercentg,
                            Item.TDSRoundOff,
                            Item.TDSAmount,
                            MainModel.ItemNetAmount,
                            MainModel.NetTotal,
                            Item.TDSRemark ?? string.Empty,
                            "PurchaseBill",
                            string.Empty,
                            challanDt,
                            string.Empty,
                            BankVoucherDt,
                            0,
                            0,
                            0f,
                            Item.TDSRoundOffAmt ?? 0,
                            });
                    }
                }
                return Table;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        private static DataTable GetDbCrDetailTable(DirectPurchaseBillModel MainModel)
        {
            DataTable Table = new();
            Table.Columns.Add("AccEntryId", typeof(int));
            Table.Columns.Add("AccYearCode", typeof(int));
            Table.Columns.Add("SeqNo", typeof(int));
            Table.Columns.Add("InvoiceNo", typeof(string));
            Table.Columns.Add("VoucherNo", typeof(string));
            Table.Columns.Add("AginstInvNo", typeof(string));
            Table.Columns.Add("AginstVoucherYearCode", typeof(int));
            Table.Columns.Add("AccountCode", typeof(int));
            Table.Columns.Add("DocTypeID", typeof(int));
            Table.Columns.Add("ItemCode", typeof(int));
            Table.Columns.Add("BillQty", typeof(float));
            Table.Columns.Add("Rate", typeof(float));
            Table.Columns.Add("DiscountPer", typeof(float));
            Table.Columns.Add("DiscountAmt", typeof(float));
            Table.Columns.Add("AccountAmount", typeof(float));
            Table.Columns.Add("DRCR", typeof(string));

            IList<DPBItemDetail> itemDetailList = MainModel.ItemDetailGrid;
            if (itemDetailList != null && itemDetailList.Any())
            {
                foreach (var Item in itemDetailList)
                {
                    Table.Rows.Add(
                    new object[]
                    {
                    MainModel.EntryID,
                    MainModel.YearCode,
                    Item.SeqNo,
                    MainModel.InvoiceNo ?? string.Empty,
                    MainModel.PurchVouchNo ?? string.Empty,
                    string.Empty,
                    0,
                    MainModel.AccountCode,
                    Item.docTypeId,
                    Item.ItemCode,
                    Math.Round(Item.BillQty, 2, MidpointRounding.AwayFromZero),
                    Math.Round(Item.Rate, 2, MidpointRounding.AwayFromZero),
                    Math.Round(Item.DiscPer, 2, MidpointRounding.AwayFromZero),
                    Math.Round(Item.DiscRs, 2, MidpointRounding.AwayFromZero),
                    Math.Round(Item.Amount, 2, MidpointRounding.AwayFromZero),
                    "CR",
                        });
                }
            }
            return Table;
        }
        private static DataTable GetPbDbCrDetailTable(PurchaseBillModel MainModel)
        {
            DataTable Table = new();
            Table.Columns.Add("AccEntryId", typeof(int));
            Table.Columns.Add("AccYearCode", typeof(int));
            Table.Columns.Add("SeqNo", typeof(int));
            Table.Columns.Add("InvoiceNo", typeof(string));
            Table.Columns.Add("VoucherNo", typeof(string));
            Table.Columns.Add("AginstInvNo", typeof(string));
            Table.Columns.Add("AginstVoucherYearCode", typeof(int));
            Table.Columns.Add("AccountCode", typeof(int));
            Table.Columns.Add("DocTypeID", typeof(int));
            Table.Columns.Add("ItemCode", typeof(int));
            Table.Columns.Add("BillQty", typeof(float));
            Table.Columns.Add("Rate", typeof(float));
            Table.Columns.Add("DiscountPer", typeof(float));
            Table.Columns.Add("DiscountAmt", typeof(float));
            Table.Columns.Add("AccountAmount", typeof(float));
            Table.Columns.Add("DRCR", typeof(string));

            IList<PBItemDetail> itemDetailList = MainModel.ItemDetailGrid != null && MainModel.ItemDetailGrid.Count > 0 ? MainModel.ItemDetailGrid : MainModel.ItemDetailGridd;
            if (itemDetailList != null && itemDetailList.Any())
            {
                foreach (var Item in itemDetailList)
                {
                    Table.Rows.Add(
                    new object[]
                    {
                    MainModel.EntryID,
                    MainModel.YearCode,
                    Item.SeqNo,
                    MainModel.InvNo ?? string.Empty,
                    MainModel.PurchVouchNo ?? string.Empty,
                    string.Empty,
                    0,
                    MainModel.AccountCode,
                    Item.DocTypeID,
                    Item.ItemCode,
                    Math.Round(Item.BillQty ?? 0, 2, MidpointRounding.AwayFromZero),
                    Math.Round(Convert.ToDecimal(Item.BillRate != null ? Item.BillRate : 0), 2, MidpointRounding.AwayFromZero),
                    Math.Round(Item.DisPer ?? 0, 2, MidpointRounding.AwayFromZero),
                    Math.Round(Item.DisAmt ?? 0, 2, MidpointRounding.AwayFromZero),
                    Math.Round(Convert.ToDecimal(Item.Amount != null ? Item.Amount : 0), 2, MidpointRounding.AwayFromZero),
                    "CR",
                        });
                }
            }
            return Table;
        }
        private static DataTable GetSbDbCrDetailTable(SaleBillModel MainModel)
        {
            try
            {
                DataTable Table = new();
                Table.Columns.Add("AccEntryId", typeof(int));
                Table.Columns.Add("AccYearCode", typeof(int));
                Table.Columns.Add("SeqNo", typeof(int));
                Table.Columns.Add("InvoiceNo", typeof(string));
                Table.Columns.Add("VoucherNo", typeof(string));
                Table.Columns.Add("AginstInvNo", typeof(string));
                Table.Columns.Add("AginstVoucherYearCode", typeof(int));
                Table.Columns.Add("AccountCode", typeof(int));
                Table.Columns.Add("DocTypeID", typeof(int));
                Table.Columns.Add("ItemCode", typeof(int));
                Table.Columns.Add("BillQty", typeof(float));
                Table.Columns.Add("Rate", typeof(float));
                Table.Columns.Add("DiscountPer", typeof(float));
                Table.Columns.Add("DiscountAmt", typeof(float));
                Table.Columns.Add("AccountAmount", typeof(float));
                Table.Columns.Add("DRCR", typeof(string));

                IList<SaleBillDetail> itemDetailList = MainModel.ItemDetailGrid;
                foreach (var Item in itemDetailList)
                {
                    Table.Rows.Add(
                    new object[]
                    {
                    MainModel.SaleBillEntryId,
                    MainModel.SaleBillYearCode,
                    Item.SeqNo,
                    MainModel.ExportInvoiceNo ?? string.Empty, //invoice no
                    MainModel.adjustmentModel?.AdjAgnstVouchNo, //MainModel.voucherNo ?? string.Empty,
                    string.Empty, // AginstInvNo
                    2024, // AginstVoucherYearCode
                    MainModel.AccountCode,
                    MainModel.DocTypeAccountCode,
                    Item.ItemCode,
                    Math.Round(Item.Qty, 2, MidpointRounding.AwayFromZero),
                    Math.Round(Item.Rate, 2, MidpointRounding.AwayFromZero),
                    Math.Round(Item.DiscountPer, 2, MidpointRounding.AwayFromZero),
                    Math.Round(Item.DiscountAmt, 2, MidpointRounding.AwayFromZero), //DisRs
                    Math.Round(Item.Amount, 2, MidpointRounding.AwayFromZero),
                    "CR",
                        });
                }

                return Table;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        private static DataTable GetCNDbCrDetailTable(AccCreditNoteModel MainModel)
        {
            try
            {
                DataTable Table = new();
                Table.Columns.Add("AccEntryId", typeof(int));
                Table.Columns.Add("AccYearCode", typeof(int));
                Table.Columns.Add("SeqNo", typeof(int));
                Table.Columns.Add("InvoiceNo", typeof(string));
                Table.Columns.Add("VoucherNo", typeof(string));
                Table.Columns.Add("AginstInvNo", typeof(string));
                Table.Columns.Add("AginstVoucherYearCode", typeof(int));
                Table.Columns.Add("AccountCode", typeof(int));
                Table.Columns.Add("DocTypeID", typeof(int));
                Table.Columns.Add("ItemCode", typeof(int));
                Table.Columns.Add("BillQty", typeof(float));
                Table.Columns.Add("Rate", typeof(float));
                Table.Columns.Add("DiscountPer", typeof(float));
                Table.Columns.Add("DiscountAmt", typeof(float));
                Table.Columns.Add("AccountAmount", typeof(float));
                Table.Columns.Add("DRCR", typeof(string));

                IList<AccCreditNoteDetail> itemDetailList = MainModel.AccCreditNoteDetails;
                foreach (var Item in itemDetailList)
                {
                    Table.Rows.Add(
                    new object[]
                    {
                    MainModel.CreditNoteEntryId,
                    MainModel.CreditNoteYearCode,
                    Item.SeqNo,
                    MainModel.CreditNoteInvoiceNo ?? string.Empty, //invoice no
                    MainModel.adjustmentModel == null ? string.Empty : MainModel.adjustmentModel.AdjAgnstVouchNo, //MainModel.voucherNo ?? string.Empty,
                    string.Empty, // AginstInvNo
                    2024, // AginstVoucherYearCode
                    MainModel.AccountCode,
                    MainModel.DocAccountCode,
                    Item.ItemCode,
                    Math.Round(Item.BillQty, 2, MidpointRounding.AwayFromZero), // qty
                    Math.Round(Item.CRNRate, 2, MidpointRounding.AwayFromZero), // creditnote rate
                    Math.Round(Item.DiscountPer, 2, MidpointRounding.AwayFromZero),
                    Math.Round(Item.DiscountAmt, 2, MidpointRounding.AwayFromZero), //DisRs
                    Math.Round(Item.Amount , 2, MidpointRounding.AwayFromZero),
                    "CR",
                        });
                }

                return Table;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private static DataTable GetPRDbCrDetailTable(AccPurchaseRejectionModel MainModel)
        {
            try
            {
                DataTable Table = new();
                Table.Columns.Add("AccEntryId", typeof(int));
                Table.Columns.Add("AccYearCode", typeof(int));
                Table.Columns.Add("SeqNo", typeof(int));
                Table.Columns.Add("InvoiceNo", typeof(string));
                Table.Columns.Add("VoucherNo", typeof(string));
                Table.Columns.Add("AginstInvNo", typeof(string));
                Table.Columns.Add("AginstVoucherYearCode", typeof(int));
                Table.Columns.Add("AccountCode", typeof(int));
                Table.Columns.Add("DocTypeID", typeof(int));
                Table.Columns.Add("ItemCode", typeof(int));
                Table.Columns.Add("BillQty", typeof(float));
                Table.Columns.Add("Rate", typeof(float));
                Table.Columns.Add("DiscountPer", typeof(float));
                Table.Columns.Add("DiscountAmt", typeof(float));
                Table.Columns.Add("AccountAmount", typeof(float));
                Table.Columns.Add("DRCR", typeof(string));

                IList<AccPurchaseRejectionDetail> itemDetailList = MainModel.AccPurchaseRejectionDetails;
                if (itemDetailList != null && itemDetailList.Any())
                {
                    foreach (var Item in itemDetailList)
                    {
                        Table.Rows.Add(
                        new object[]
                        {
                    MainModel.PurchaseRejEntryId,
                    MainModel.PurchaseRejYearCode,
                    Item.SeqNo,
                    MainModel.PurchaseRejectionInvoiceNo ?? string.Empty, //invoice no
                    MainModel.adjustmentModel == null ? string.Empty : MainModel.adjustmentModel.AdjAgnstVouchNo, //MainModel.voucherNo ?? string.Empty,
                    string.Empty, // AginstInvNo
                    MainModel.AgainstPurchaseBillYearCode, // AginstVoucherYearCode
                    MainModel.AccountCode,
                    Item.DocAccountCode,
                    Item.ItemCode,
                    Math.Round(Item.BillQty, 2, MidpointRounding.AwayFromZero), // qty
                    Math.Round(Item.PRRate, 2, MidpointRounding.AwayFromZero), // PurchaseRejection rate
                    Math.Round(Item.DiscountPer, 2, MidpointRounding.AwayFromZero),
                    Math.Round(Item.DiscountAmt, 2, MidpointRounding.AwayFromZero), //DisRs
                    Math.Round(Item.ItemAmount, 2, MidpointRounding.AwayFromZero),
                    "CR",
                            });
                    }
                }
                return Table;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private static DataTable GetSRDbCrDetailTable(SaleRejectionModel MainModel)
        {
            try
            {
                DataTable Table = new();
                Table.Columns.Add("AccEntryId", typeof(int));
                Table.Columns.Add("AccYearCode", typeof(int));
                Table.Columns.Add("SeqNo", typeof(int));
                Table.Columns.Add("InvoiceNo", typeof(string));
                Table.Columns.Add("VoucherNo", typeof(string));
                Table.Columns.Add("AginstInvNo", typeof(string));
                Table.Columns.Add("AginstVoucherYearCode", typeof(int));
                Table.Columns.Add("AccountCode", typeof(int));
                Table.Columns.Add("DocTypeID", typeof(int));
                Table.Columns.Add("ItemCode", typeof(int));
                Table.Columns.Add("BillQty", typeof(float));
                Table.Columns.Add("Rate", typeof(float));
                Table.Columns.Add("DiscountPer", typeof(float));
                Table.Columns.Add("DiscountAmt", typeof(float));
                Table.Columns.Add("AccountAmount", typeof(float));
                Table.Columns.Add("DRCR", typeof(string));

                IList<SaleRejectionDetail> itemDetailList = MainModel.ItemDetailGrid;
                foreach (var Item in itemDetailList)
                {
                    Table.Rows.Add(
                    new object[]
                    {
                    MainModel.SaleRejEntryId,
                    MainModel.SaleRejYearCode,
                   1, //Item.SeqNo,
                    MainModel.CustInvoiceNo ?? string.Empty, //invoice no
                    string.Empty, //MainModel.voucherNo ?? string.Empty,
                    string.Empty,
                    0,
                    MainModel.AccountCode,
                    MainModel.DocTypeAccountCode,
                    Item.ItemCode,
                    Math.Round(Item.RejQty, 2, MidpointRounding.AwayFromZero),
                    Math.Round(Item.Rate, 2, MidpointRounding.AwayFromZero),
                    Math.Round(Item.DiscountPer, 2, MidpointRounding.AwayFromZero),
                    Math.Round(Item.DiscountAmt, 2, MidpointRounding.AwayFromZero), //DisRs
                    Math.Round(Item.Amount, 2, MidpointRounding.AwayFromZero),
                    "CR",
                        });
                }

                return Table;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        private List<DbCrModel> TransformJsonToDbCrModel(string jsonData)
        {
            var jsonObject = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonData);

            // Extract the "Result" property as a JSON array string
            var resultArrayString = jsonObject["Result"].ToString();

            // Deserialize the JSON array string into a list of dynamic objects
            var rawData = JsonConvert.DeserializeObject<List<dynamic>>(resultArrayString);
            var dbCrModels = new List<DbCrModel>();

            foreach (var item in rawData)
            {
                // Transform dynamic data into DbCrModel
                dbCrModels.Add(new DbCrModel
                {
                    AccountCode = item.AccountCode != null ? (int)item.AccountCode : 0,
                    //AccountName = (string)item.AccountName,
                    DrAmt = item.DrAmt != null ? (float)item.DrAmt : 0,
                    CrAmt = item.CrAmt != null ? (float)item.CrAmt : 0,
                    //DRCR = (item.DrAmt > 0) ? "DR" : "CR" // DR/CR based on the DrAmt
                });
            }

            return dbCrModels;
        }
        public static DataTable GetDrCrDetailTable(List<DbCrModel> dbCrDetailList)
        {
            DataTable Table = new();
            Table.Columns.Add("AccountCode", typeof(int));
            Table.Columns.Add("DrAmt", typeof(float));
            Table.Columns.Add("CrAmt", typeof(float));

            if (dbCrDetailList != null && dbCrDetailList.Count > 0)
            {
                foreach (var Item in dbCrDetailList)
                {
                    Table.Rows.Add(
                    new object[]
                    {
                        Item.AccountCode,
                     Item.DrAmt != null ? Math.Round((decimal)Item.DrAmt, 2) : 0,
                      Item.CrAmt != null ? Math.Round((decimal)Item.CrAmt, 2) : 0
                    });
                }
            }
            return Table;
        }
        #endregion

        #region For Adjustment Grid
        public async Task<IActionResult> AddAdjstmntDetail(AdjustmentModel model)
        {
            var isDuplicate = false;
            var isExpAdded = isDuplicate;
            var taxFound = isExpAdded;

            model.AdjDueDate = model.DueDate == null ? new DateTime() : Convert.ToDateTime(ParseFormattedDate(model.DueDate));

            var _List = new List<AdjustmentModel>();

            dynamic MainModel = new SaleOrderModel();

            if (model.AdjPageName == "ItemList")
            {
                MainModel = new SaleOrderModel();
            }
            else if (model.AdjPageName == "PurchaseOrder")
            {
                MainModel = new PurchaseOrderModel();
            }
            else if (model.AdjPageName == "DirectPurchaseBill")
            {
                MainModel = new DirectPurchaseBillModel();
            }
            else if (model.AdjPageName == "PurchaseBill")
            {
                MainModel = new PurchaseBillModel();
            }
            else if (model.AdjPageName == "SaleInvoice")
            {
                MainModel = new SaleBillModel();
            }
            else if (model.AdjPageName == "SaleBillOnCounter")
            {
                MainModel = new SaleBillModel();
            }
            else if (model.AdjPageName == "SaleRejection")
            {
                MainModel = new SaleRejectionModel();
            }
            else if (model.AdjPageName == "CreditNote")
            {
                MainModel = new AccCreditNoteModel();
            }
            else if (model.AdjPageName == "PurchaseRejection")
            {
                MainModel = new AccPurchaseRejectionModel();
            }
            else if (model.AdjPageName == "JobWorkIssue")
            {
                MainModel = new JobWorkIssueModel();
            }

            //var CgstSgst = ITDSModule.SgstCgst(model.TDSAccountCode);
            string modelJson = HttpContext.Session.GetString("KeyAdjGrid");
            AdjustmentModel AdjGrid = new AdjustmentModel();
            if (!string.IsNullOrEmpty(modelJson))
            {
                AdjGrid = JsonConvert.DeserializeObject<AdjustmentModel>(modelJson);
            }


            if (model.AdjPageName == "ItemList")
            {
                MainModel.ItemDetailGrid = JsonConvert.DeserializeObject<List<ItemDetail>>(HttpContext.Session.GetString(model.AdjPageName));
            }
            else if (model.AdjPageName == "PurchaseOrder")
            {
                string modelJsonData = HttpContext.Session.GetString("PurchaseOrder");
                if (!string.IsNullOrEmpty(modelJsonData))
                {
                    MainModel = JsonConvert.DeserializeObject<PurchaseOrderModel>(modelJsonData);
                }
            }
            else if (model.AdjPageName == "DirectPurchaseBill")
            {
                string modelJsonData = HttpContext.Session.GetString("DirectPurchaseBill");
                if (!string.IsNullOrEmpty(modelJsonData))
                {
                    MainModel = JsonConvert.DeserializeObject<DirectPurchaseBillModel>(modelJsonData);
                }
            }
            else if (model.AdjPageName == "PurchaseBill")
            {

                string modelJsonData = HttpContext.Session.GetString("PurchaseBill");
                if (!string.IsNullOrEmpty(modelJsonData))
                {
                    MainModel = JsonConvert.DeserializeObject<PurchaseBillModel>(modelJsonData);
                }
            }
            else if (model.AdjPageName == "SaleInvoice")
            {
                string modelJsonData = HttpContext.Session.GetString("SaleBillModel");
                if (!string.IsNullOrEmpty(modelJsonData))
                {
                    MainModel = JsonConvert.DeserializeObject<SaleBillModel>(modelJsonData);
                }
            }
            else if (model.AdjPageName == "SaleBillOnCounter")
            {
                string modelJsonData = HttpContext.Session.GetString("SaleBillModel");
                if (!string.IsNullOrEmpty(modelJsonData))
                {
                    MainModel = JsonConvert.DeserializeObject<SaleBillModel>(modelJsonData);
                }
            }
            else if (model.AdjPageName == "SaleRejection")
            {
                string modelJsonData = HttpContext.Session.GetString("SaleRejectionModel");
                if (!string.IsNullOrEmpty(modelJsonData))
                {
                    MainModel = JsonConvert.DeserializeObject<SaleRejectionModel>(modelJsonData);
                }
            }
            else if (model.AdjPageName == "CreditNote")
            {
                string modelJsonData = HttpContext.Session.GetString("CreditNoteModel");
                if (!string.IsNullOrEmpty(modelJsonData))
                {
                    MainModel = JsonConvert.DeserializeObject<AccCreditNoteModel>(modelJsonData);
                }
            }
            else if (model.AdjPageName == "PurchaseRejection")
            {
                string modelJsonData = HttpContext.Session.GetString("PurchaseRejectionModel");
                if (!string.IsNullOrEmpty(modelJsonData))
                {
                    MainModel = JsonConvert.DeserializeObject<AccPurchaseRejectionModel>(modelJsonData);
                }
            }
            else if (model.AdjPageName == "JobWorkIssue")
            {
                string modelJsonData = HttpContext.Session.GetString("JobWorkIssue");
                if (!string.IsNullOrEmpty(modelJsonData))
                {
                    MainModel = JsonConvert.DeserializeObject<JobWorkIssueModel>(modelJsonData);
                }
            }
            if (AdjGrid?.AdjAdjustmentDetailGrid != null && AdjGrid?.AdjAdjustmentDetailGrid?.Count > 0)
            {
                MainModel.adjustmentModel = AdjGrid;
                isDuplicate = AdjGrid.AdjAdjustmentDetailGrid.Any(a => (a.AdjModeOfAdjstment.Equals(model.AdjModeOfAdjstment) && a.AdjModeOfAdjstment == "NewRef" && a.AdjNewRefNo.Equals(model.AdjNewRefNo)) || (a.AdjModeOfAdjstment.Equals(model.AdjModeOfAdjstment) && a.AdjNewRefNo.Equals(model.AdjNewRefNo) && (a.AdjPurchOrderNo != null && a.AdjPurchOrderNo.Equals(model.AdjPurchOrderNo)) && (a.AdjPOYear != null && a.AdjPOYear.Equals(model.AdjPOYear)) && a.AdjAgnstAccEntryID.Equals(model.AdjAgnstAccEntryID) && a.AdjAgnstAccYearCode.Equals(model.AdjAgnstAccYearCode)));
            }

            if (!isDuplicate)
            {
                if (AdjGrid?.AdjAdjustmentDetailGrid != null && AdjGrid?.AdjAdjustmentDetailGrid?.Count > 0)
                {
                    _List.AddRange(MainModel.adjustmentModel.AdjAdjustmentDetailGrid);
                    _List.AddRange(Add2List(model, _List));
                }
                else
                {
                    var AdjModelList = Add2List(model, _List);
                    _List.AddRange(AdjModelList);
                }
            }
            else
            {
                return StatusCode(200);
            }
            if (MainModel.adjustmentModel == null)
            {
                MainModel.adjustmentModel = new AdjustmentModel();
            }
            MainModel.adjustmentModel.AdjAdjustmentDetailGrid = _List;
            //AdjustmentModel = MainModel.adjustmentModel.AdjAdjustmentDetailGrid;

            //StoreInSession("KeyAdjGrid", MainModel.adjustmentModel);

            if (MainModel.adjustmentModel != null)
            {
                string serializedGrid = JsonConvert.SerializeObject(MainModel.adjustmentModel);
                HttpContext.Session.SetString("KeyAdjGrid", serializedGrid);
                await Task.Delay(100);
            }
            else
            {
                // Log or debug
                Console.WriteLine("adjustmentModel is NULL");
                return StatusCode(500, "Adjustment model is null.");
            }



            return PartialView("_AdjGrid", MainModel.adjustmentModel);
        }

        public IList<AdjustmentModel> Add2List(AdjustmentModel model, IList<AdjustmentModel> AdjGrid, bool? IsAgnstRefPopupData = false)
        {
            var _List = new List<AdjustmentModel>();
            if (IsAgnstRefPopupData != true)
            {
                _List.Add(new AdjustmentModel
                {
                    AdjSeqNo = AdjGrid == null ? 1 : AdjGrid.Count + 1,
                    AdjModeOfAdjstment = model.AdjModeOfAdjstment,
                    AdjModeOfAdjstmentName = model.AdjModeOfAdjstmentName,
                    AdjDescription = model.AdjDescription,
                    AdjDueDate = model.AdjDueDate,
                    AdjNewRefNo = model.AdjNewRefNo,
                    AdjPendAmt = model.AdjPendAmt,
                    AdjDrCr = model.AdjDrCr,
                    AdjDrCrName = model.AdjDrCrName,
                    AdjPurchOrderNo = model.AdjPurchOrderNo,
                    AdjPOYear = model.AdjPOYear,
                    AdjPODate = model.AdjPODate,
                    AdjOpenEntryID = model.AdjOpenEntryID ?? 0,
                    AdjOpeningYearCode = model.AdjOpeningYearCode ?? 0,
                    AdjAgnstAccEntryID = model.AdjAgnstAccEntryID ?? 0,
                    AdjAgnstAccYearCode = model.AdjAgnstAccYearCode ?? 0,
                });
            }
            else
            {
                foreach (var item in model.AdjAdjustmentDetailGrid)
                {
                    _List.Add(new AdjustmentModel
                    {
                        AdjSeqNo = AdjGrid == null ? 1 : AdjGrid.Count + 1,
                        AdjModeOfAdjstment = "AgainstRef",
                        AdjModeOfAdjstmentName = "Against Ref",
                        AdjDescription = item.AdjDescription,
                        AdjDueDate = item.AdjAgnstVouchDate,
                        AdjNewRefNo = item.AdjNewRefNo,
                        AdjPendAmt = Convert.ToDecimal(item.AdjAgnstAdjstedAmt),
                        AdjDrCr = item.AdjAgnstDrCr,
                        AdjDrCrName = item.AdjAgnstDrCr,
                        AdjPurchOrderNo = string.Empty,
                        AdjPOYear = 0,
                        AdjPODate = null,
                        AdjAgnstVouchNo = item.AdjAgnstVouchNo,
                        AdjAgnstVouchType = item.AdjAgnstVouchType,
                        AdjOpenEntryID = item.AdjAgnstOpenEntryID ?? 0,
                        AdjOpeningYearCode = item.AdjAgnstOpeningYearCode ?? 0,
                        AdjAgnstAccEntryID = item.AdjAgnstAccEntryID ?? 0,
                        AdjAgnstAccYearCode = item.AdjAgnstAccYearCode ?? 0,
                    });
                }
            }
            return _List;
        }
        public static DataTable GetAdjDetailTable(List<AdjustmentModel> AdjDetailList, int? entryid, int? yearcode, int? acccode)
        {
            DataTable Table = new();
            Table.Columns.Add("EntryId", typeof(int));
            Table.Columns.Add("YearCode", typeof(int));
            Table.Columns.Add("AccountCode", typeof(int));
            Table.Columns.Add("ModOfAdjustment", typeof(string));
            Table.Columns.Add("DrCr", typeof(string));
            Table.Columns.Add("AdjustedAmount", typeof(float));
            Table.Columns.Add("AgainstAccEntryId", typeof(int));
            Table.Columns.Add("AgainstVoucheryearcode", typeof(int));
            Table.Columns.Add("AgainstvoucherType", typeof(string));
            Table.Columns.Add("AgainstVoucherNo", typeof(string));
            Table.Columns.Add("AgainstVoucherAmount", typeof(float));
            Table.Columns.Add("AgainstVoucherModAdjustment", typeof(string));
            Table.Columns.Add("AgainstAccOpeningEntryId", typeof(int));
            Table.Columns.Add("AgainstOpeningVoucheryearcode", typeof(int));
            Table.Columns.Add("RefNo", typeof(string));
            Table.Columns.Add("VchDescription", typeof(string));
            Table.Columns.Add("DueDate", typeof(string));
            Table.Columns.Add("AgainstOrderno", typeof(string));
            Table.Columns.Add("AgainstOrderYearCode", typeof(int));
            Table.Columns.Add("AgainstOrderDate", typeof(string));
            Table.Columns.Add("field1", typeof(string));
            Table.Columns.Add("field2", typeof(string));
            Table.Columns.Add("field3", typeof(string));
            Table.Columns.Add("fieldNumINT1", typeof(int));
            Table.Columns.Add("fieldNumINT2", typeof(int));
            Table.Columns.Add("fieldNumINT3", typeof(int));
            Table.Columns.Add("fieldNum1", typeof(float));
            Table.Columns.Add("fieldNum2", typeof(float));
            Table.Columns.Add("fieldNum3", typeof(float));
            Table.Columns.Add("fieldDate1", typeof(string));
            Table.Columns.Add("fieldDate2", typeof(string));
            Table.Columns.Add("fieldDate3", typeof(string));

            if (AdjDetailList != null && AdjDetailList.Count > 0)
            {
                foreach (var Item in AdjDetailList)
                {
                    var AdjOrderDt = ParseFormattedDate(DateTime.Today.ToString());
                    var AdjDueDt = ParseFormattedDate(DateTime.Today.ToString());
                    var fdt1 = ParseFormattedDate(DateTime.Today.ToString());
                    var fdt2 = ParseFormattedDate(DateTime.Today.ToString());
                    var fdt3 = ParseFormattedDate(DateTime.Today.ToString());
                    if (Item.AdjModeOfAdjstment != null && Item.AdjModeOfAdjstment != "AgainstRef")
                    {
                        Table.Rows.Add(
                        new object[]
                        {
                        entryid ?? 0,
                        yearcode ?? 0,
                        acccode ?? 0,
                        // adjustment detail
                        Item.AdjModeOfAdjstment,
                        Item.AdjDrCr ?? string.Empty,
                       (float)Math.Round((Item.AdjPendAmt != null && Item.AdjPendAmt > 0) ? Convert.ToSingle(Item.AdjPendAmt) : 0, 2),//Item.AdjAdjstedAmt ?? 0,
                        0,//AgainstAccEntryId
                        0,//AgainstVoucheryearcode
                        string.Empty,//AgainstvoucherType
                        string.Empty,//AgainstVoucherNo
                        0,//AgainstVoucherAmount
                        string.Empty,//AgainstVoucherModAdjustment
                        0,//AgainstAccOpeningEntryId
                        0,//AgainstOpeningVoucheryearcode
                        Item.AdjNewRefNo,
                        Item.AdjDescription,
                        Item.DueDate == null
    ? (Item.AdjDueDate == null
        ? AdjDueDt
        : ParseFormattedDate(Item.AdjDueDate.ToString()))
    : ParseFormattedDate(Item.DueDate.ToString()),
                        string.Empty,//AgainstOrderno
                        0,//AgainstOrderYeearCode
                        AdjOrderDt,//AgainstOrderDate
                        string.Empty,//field1
                        string.Empty,//field2
                        string.Empty,//field3
                        0,//fieldNumINT1
                        0,//fieldNumINT2
                        0,//fieldNumINT3
                        0,//fieldNum1
                        0,//fieldNum2
                        0,//fieldNum3
                        fdt1,//fieldDate1
                        fdt2,//fieldDate2
                        fdt3,//fieldDate3
                        });
                    }
                    else
                    {
                        Table.Rows.Add(
                        new object[]
                        {
                        entryid ?? 0,
                        yearcode ?? 0,
                        acccode ?? 0,
                        Item.AdjModeOfAdjstment ?? string.Empty,
                        Item.AdjDrCr ?? string.Empty,
                        (Item.AdjPendAmt != null && Item.AdjPendAmt > 0) ? Convert.ToSingle(Item.AdjPendAmt) : 0,
                        Item.AdjAgnstAccEntryID ?? 0,
                        Item.AdjAgnstAccYearCode ?? 0,
                        Item.AdjAgnstVouchType ?? (Item.AdjNewRefNo ?? string.Empty),
                        Item.AdjAgnstVouchNo ?? (Item.AdjDescription ?? string.Empty),
                       (Item.AdjAgnstPendAmt.HasValue ? Convert.ToDecimal(Item.AdjAgnstPendAmt.Value)
                               : Convert.ToDecimal(Item.AdjPendAmt ?? 0)),
                        Item.AdjAgnstModeOfAdjstment ?? (Item.AdjModeOfAdjstment ?? string.Empty),
                        Item.AdjAgnstOpenEntryID ?? (Item.AdjOpenEntryID ?? 0),
                        Item.AdjAgnstOpeningYearCode ?? (Item.AdjOpeningYearCode ?? 0),
                        string.Empty,//Item.AdjNewRefNo,
                        string.Empty,//Item.AdjDescription,
                        Item.DueDate == null ? (Item.AdjDueDate != null ? ParseFormattedDate(Item.AdjDueDate.ToString()) :  AdjDueDt) : ParseFormattedDate(Item.DueDate.ToString()),
                        string.Empty,//AgainstOrderno
                        0,//AgainstOrderYeearCode
                        AdjOrderDt,//AgainstOrderDate
                        string.Empty,//field1
                        string.Empty,//field2
                        string.Empty,//field3
                        0,//fieldNumINT1
                        0,//fieldNumINT2
                        0,//fieldNumINT3
                        0,//fieldNum1
                        0,//fieldNum2
                        0,//fieldNum3
                        fdt1,//fieldDate1
                        fdt2,//fieldDate2
                        fdt3,//fieldDate3
                        });
                    }
                }
            }
            return Table;
        }
        public IActionResult EditAdjRow(string SeqNo, string SN)
        {
            string modelJson = HttpContext.Session.GetString("KeyAdjGrid");
            AdjustmentModel AdjGrid = new AdjustmentModel();
            if (!string.IsNullOrEmpty(modelJson))
            {
                AdjGrid = JsonConvert.DeserializeObject<AdjustmentModel>(modelJson);
            }
            var SSGrid = AdjGrid.AdjAdjustmentDetailGrid.Where(x => x.AdjSeqNo.ToString() == SeqNo);
            string JsonString = JsonConvert.SerializeObject(SSGrid);
            return Json(JsonString);
        }
        public IActionResult DeleteAdjRow(string SeqNo, string SN)
        {
            dynamic MainModel = null;
            if (SN == "ItemList")
            {
                MainModel = new SaleOrderModel();
            }
            else if (SN == "PurchaseOrder")
            {
                MainModel = new PurchaseOrderModel();
            }
            else if (SN == "DirectPurchaseBill")
            {
                MainModel = new DirectPurchaseBillModel();
            }
            else if (SN == "PurchaseBill")
            {
                MainModel = new PurchaseBillModel();
            }
            else if (SN == "PurchaseRejection")
            {
                MainModel = new AccPurchaseRejectionModel();
            }
            else if (SN == "JobWorkIssue")
            {
                MainModel = new JobWorkIssueModel();
            }
            else if (SN == "SaleInvoice")
            {
                MainModel = new SaleBillModel();
            }
            //MainModel = SN == "ItemList" ? new SaleOrderModel() : new PurchaseOrderModel();
            string modelJson = HttpContext.Session.GetString("KeyAdjGrid");
            AdjustmentModel AdjGrid = new AdjustmentModel();
            if (!string.IsNullOrEmpty(modelJson))
            {
                AdjGrid = JsonConvert.DeserializeObject<AdjustmentModel>(modelJson);
            }

            if (AdjGrid != null)
            {
                bool canDelete = true;

                MainModel.adjustmentModel = AdjGrid;
                int Indx = Convert.ToInt32(SeqNo) - 1;

                if (canDelete)
                {
                    var Remove = AdjGrid.AdjAdjustmentDetailGrid.Where(s => s.AdjSeqNo == Convert.ToInt32(SeqNo)).ToList();

                    foreach (var item in Remove)
                    {
                        AdjGrid.AdjAdjustmentDetailGrid.Remove(item);
                    }

                    Indx = 0;
                    foreach (AdjustmentModel item in AdjGrid.AdjAdjustmentDetailGrid)
                    {
                        Indx++;
                        item.AdjSeqNo = Indx;
                    }
                    HttpContext.Session.SetString("KeyAdjGrid", JsonConvert.SerializeObject(AdjGrid));
                }
                else
                {
                    return StatusCode(statusCode: 300);
                }
            }
            return PartialView("_AdjGrid", MainModel.adjustmentModel);
        }

        [Route("Common/GetPendVouchBillAgainstRefPopupByID")]
        public async Task<JsonResult> GetPendVouchBillAgainstRefPopupByID(int AC, int? YC, int? PayRecEntryId, int? PayRecYearcode, string DRCR, string TransVouchType, string TransVouchDate)
        {
            string Flag = "";
            var JSON = await IDataLogic.GetPendVouchBillAgainstRefPopupByID(AC, YC, PayRecEntryId, PayRecYearcode, DRCR, TransVouchType, TransVouchDate, Flag);
            string JsonString = JsonConvert.SerializeObject(JSON);
            return Json(JsonString);
        }
        public IActionResult AddAgnstRefToAdjstmntDetail([FromBody] AdjustmentModel model)
        {
            if (model == null || model.AdjAdjustmentDetailGrid == null || !model.AdjAdjustmentDetailGrid.Any())
            {
                return BadRequest("No adjustment data provided.");
            }

            var isDuplicate = false;
            var combinedList = new List<AdjustmentModel>();

            // Determine main model type
            dynamic MainModel = new DirectPurchaseBillModel();
            string SN = model.AdjPageName;
            foreach (var item in model.AdjAdjustmentDetailGrid)
            {
                SN = item.AdjPageName;
            }

            if (SN == "DirectPurchaseBill") MainModel = new DirectPurchaseBillModel();
            else if (SN == "PurchaseBill") MainModel = new PurchaseBillModel();
            else if (SN == "SaleInvoice") MainModel = new SaleBillModel();

            // Get existing grid from session
            string modelJson = HttpContext.Session.GetString("KeyAdjGrid");
            AdjustmentModel existingGrid = !string.IsNullOrEmpty(modelJson)
                ? JsonConvert.DeserializeObject<AdjustmentModel>(modelJson)
                : new AdjustmentModel();

            MainModel.adjustmentModel = existingGrid;

            // Mark mode
            model.AdjAgnstModeOfAdjstment = "AgainstRef";

            // Check duplicate (if needed)
            isDuplicate = existingGrid?.AdjAdjustmentDetailGrid?.Any(a =>
                (a.AdjModeOfAdjstment != null && a.AdjModeOfAdjstment.Equals(model.AdjModeOfAdjstmentName ?? string.Empty)) &&
                (a.AdjNewRefNo != null && a.AdjNewRefNo.Equals(model.AdjAgnstNewRefNo ?? string.Empty))
            ) ?? false;

            if (isDuplicate)
            {
                return StatusCode(200); // duplicate found
            }

            // Combine existing rows and new rows
            if (existingGrid.AdjAdjustmentDetailGrid != null && existingGrid.AdjAdjustmentDetailGrid.Count > 0)
            {
                combinedList.AddRange(existingGrid.AdjAdjustmentDetailGrid);
            }

            var newRows = Add2List(model, combinedList, true);

            // Assign sequence numbers
            int seqStart = combinedList.Count + 1;
            foreach (var row in newRows)
            {
                row.AdjSeqNo = seqStart;
                seqStart++;
            }

            combinedList.AddRange(newRows);

            // Save back to main model and session
            MainModel.adjustmentModel.AdjAdjustmentDetailGrid = combinedList;
            StoreInSession("KeyAdjGrid", MainModel.adjustmentModel);

            // Return JSON
            string jsonString = JsonConvert.SerializeObject(combinedList);
            return Json(jsonString);
        }
        public IActionResult GetUpdatedAdjGridData()
        {
            // Retrieve the updated data from the cache
            string modelJson = HttpContext.Session.GetString("KeyAdjGrid");
            AdjustmentModel AdjGrid = new AdjustmentModel();
            if (!string.IsNullOrEmpty(modelJson))
            {
                AdjGrid = JsonConvert.DeserializeObject<AdjustmentModel>(modelJson);
            }

            if (AdjGrid != null)
            {
                return Json(AdjGrid.AdjAdjustmentDetailGrid);
            }
            return Json(new List<AdjustmentModel>());
        }
        #endregion


    }
}
