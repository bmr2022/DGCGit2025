using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Reflection;
using static eTactWeb.DOM.Models.Common;
using static eTactWeb.Data.Common.CommonFunc;

namespace eTactWeb.Data.DAL
{
    public class CreditNoteDAL
    {
        private readonly IDataLogic _IDataLogic;
        private readonly string DBConnectionString = string.Empty;
        private IDataReader? Reader;
        private readonly ConnectionStringService _connectionStringService;
        public CreditNoteDAL(IConfiguration configuration, IDataLogic iDataLogic, ConnectionStringService connectionStringService)
        {
            //DBConnectionString = configuration.GetConnectionString("eTactDB");
            _connectionStringService = connectionStringService;
            DBConnectionString = _connectionStringService.GetConnectionString();
            _IDataLogic = iDataLogic;
        }

        public async Task<AccCreditNoteModel> GetViewByID(int ID, int YearCode, string mode)
        {
            var model = new AccCreditNoteModel();
            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@Flag", "VIEWBYID"));
                SqlParams.Add(new SqlParameter("@CreditNoteEntryId", ID));
                SqlParams.Add(new SqlParameter("@CreditNoteYearCode", YearCode));
                var _ResponseResult = await _IDataLogic.ExecuteDataSet("AccSP_CreditNoteMainDetail", SqlParams);

                if (_ResponseResult.Result != null && _ResponseResult.StatusCode == HttpStatusCode.OK && _ResponseResult.StatusText == "Success")
                {
                    PrepareView(_ResponseResult.Result, ref model, mode);
                }
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return model;
        }

        private static AccCreditNoteModel PrepareView(DataSet DS, ref AccCreditNoteModel? model, string Mode)
        {
            var ItemGrid = new List<AccCreditNoteDetail>();
            var creditNoteGrid = new List<AccCreditNoteDetail>();
            var creditNoteAgainstBillGrid = new List<AccCreditNoteAgainstBillDetail>();
            var TaxGrid = new List<TaxModel>();
            var DRCRGrid = new List<DbCrModel>();
            var adjustGrid = new List<AdjustmentModel>();
            DS.Tables[0].TableName = "CreditNoteModel";
            DS.Tables[1].TableName = "CreditNoteDetail";
            DS.Tables[2].TableName = "CreditNoteAgainstBillGrid";
            DS.Tables[3].TableName = "CreditNoteTaxDetail";
            DS.Tables[4].TableName = "DRCRDetail";
            DS.Tables[5].TableName = "AdjustmentDetail";
            int cnt = 0;

            model.CreditNoteEntryId = DS.Tables[0].Rows[0]["CreditNoteEntryId"] != DBNull.Value ? Convert.ToInt32(DS.Tables[0].Rows[0]["CreditNoteEntryId"]) : 0;
            model.CreditNoteYearCode = DS.Tables[0].Rows[0]["CreditNoteYearCode"] != DBNull.Value ? Convert.ToInt32(DS.Tables[0].Rows[0]["CreditNoteYearCode"]) : 0;
            model.CreditNoteInvoiceNo = DS.Tables[0].Rows[0]["CreditNoteInvoiceNo"]?.ToString();
            model.CreditNoteInvoiceDate = DS.Tables[0].Rows[0]["CreditNoteInvoiceDate"]?.ToString();
            model.SubVoucherName = DS.Tables[0].Rows[0]["SubVoucherName"]?.ToString();
            model.CreditNoteVoucherNo = DS.Tables[0].Rows[0]["CreditNoteVoucherNo"]?.ToString();
            model.CreditNoteVoucherDate = DS.Tables[0].Rows[0]["CreditNoteVoucherDate"]?.ToString();
            model.AgainstSalePurchase = DS.Tables[0].Rows[0]["AgainstSalePurchase"]?.ToString();
            model.AccountCode = DS.Tables[0].Rows[0]["AccountCode"] != DBNull.Value ? Convert.ToInt32(DS.Tables[0].Rows[0]["AccountCode"]) : 0;
            model.AccountName = DS.Tables[0].Rows[0]["CustVendName"]?.ToString();
            model.CustVendAddress = DS.Tables[0].Rows[0]["CustVendAddress"]?.ToString();
            model.StateNameofSupply = DS.Tables[0].Rows[0]["StateNameofSupply"]?.ToString();
            model.StateCode = DS.Tables[0].Rows[0]["StateCode"]?.ToString();
            model.CityofSupply = DS.Tables[0].Rows[0]["CityofSupply"]?.ToString();
            model.CountryOfSupply = DS.Tables[0].Rows[0]["CountryOfSupply"]?.ToString();
            model.PaymentTerm = DS.Tables[0].Rows[0]["PaymentTerm"]?.ToString();
            model.PaymentCreditDay = DS.Tables[0].Rows[0]["PaymentCreditDay"] != DBNull.Value ? Convert.ToInt32(DS.Tables[0].Rows[0]["PaymentCreditDay"]) : 0;
            model.GSTNO = DS.Tables[0].Rows[0]["GSTNO"]?.ToString();
            model.GstRegUnreg = DS.Tables[0].Rows[0]["GstRegUnreg"]?.ToString();
            model.Transporter = DS.Tables[0].Rows[0]["Transporter"]?.ToString();
            model.Vehicleno = DS.Tables[0].Rows[0]["Vehicleno"]?.ToString();
            model.BillAmt = DS.Tables[0].Rows[0]["BillAmt"] != DBNull.Value ? Convert.ToInt32(DS.Tables[0].Rows[0]["BillAmt"]) : 0;
            model.RoundOffAmt = DS.Tables[0].Rows[0]["RoundOffAmt"] != DBNull.Value ? Convert.ToInt32(DS.Tables[0].Rows[0]["RoundOffAmt"]) : 0;
            model.RoundoffType = DS.Tables[0].Rows[0]["RoundoffType"]?.ToString();
            model.Taxableamt = DS.Tables[0].Rows[0]["Taxableamt"] != DBNull.Value ? Convert.ToInt32(DS.Tables[0].Rows[0]["Taxableamt"]) : 0;
            model.ToatlDiscountPercent = DS.Tables[0].Rows[0]["ToatlDiscountPercent"] != DBNull.Value ? Convert.ToInt32(DS.Tables[0].Rows[0]["ToatlDiscountPercent"]) : 0;
            model.TotalDiscountAmount = DS.Tables[0].Rows[0]["TotalDiscountAmount"] != DBNull.Value ? Convert.ToInt32(DS.Tables[0].Rows[0]["TotalDiscountAmount"]) : 0;
            model.NetAmt = DS.Tables[0].Rows[0]["NetAmt"] != DBNull.Value ? Convert.ToInt32(DS.Tables[0].Rows[0]["NetAmt"]) : 0;
            model.Remark = DS.Tables[0].Rows[0]["Remark"]?.ToString();
            model.CC = DS.Tables[0].Rows[0]["CC"]?.ToString();
            model.Uid = DS.Tables[0].Rows[0]["Uid"] != DBNull.Value ? Convert.ToInt32(DS.Tables[0].Rows[0]["Uid"]) : 0;
            model.ItemService = DS.Tables[0].Rows[0]["ItemService"]?.ToString();
            model.INVOICETYPE = DS.Tables[0].Rows[0]["INVOICETYPE"]?.ToString();
            model.MachineName = DS.Tables[0].Rows[0]["MachineName"]?.ToString();
            model.ActualEntryDate = DS.Tables[0].Rows[0]["ActualEntryDate"]?.ToString();
            model.ActualEnteredBy = DS.Tables[0].Rows[0]["ActualEnteredBy"] != DBNull.Value ? Convert.ToInt32(DS.Tables[0].Rows[0]["ActualEnteredBy"]) : 0;
            model.ActualEnteredByName = DS.Tables[0].Rows[0]["ActualEntryByEmp"]?.ToString();
            model.LastUpdatedBy = DS.Tables[0].Rows[0]["LastUpdatedBy"] != DBNull.Value ? Convert.ToInt32(DS.Tables[0].Rows[0]["LastUpdatedBy"]) : 0;
            model.LastUpdatedByName = DS.Tables[0].Rows[0]["LastUpdatedByEmp"]?.ToString();
            model.LastUpdationDate = DS.Tables[0].Rows[0]["LastUpdationDate"]?.ToString();
            model.EntryFreezToAccounts = DS.Tables[0].Rows[0]["EntryFreezToAccounts"]?.ToString();
            model.BalanceSheetClosed = DS.Tables[0].Rows[0]["BalanceSheetClosed"]?.ToString();
            model.EInvNo = DS.Tables[0].Rows[0]["EInvNo"]?.ToString();
            model.EinvGenerated = DS.Tables[0].Rows[0]["EinvGenerated"]?.ToString();
            model.AttachmentFilePath1 = DS.Tables[0].Rows[0]["AttachmentFilePath1"]?.ToString();
            model.AttachmentFilePath2 = DS.Tables[0].Rows[0]["AttachmentFilePath2"]?.ToString();
            model.AttachmentFilePath3 = DS.Tables[0].Rows[0]["AttachmentFilePath3"]?.ToString();

            if (Mode == "U" || Mode == "V")
            {
                if (DS.Tables[0].Rows[0]["LastUpdatedByEmp"].ToString() != "")
                {
                    model.LastUpdatedBy = Convert.ToInt32(DS.Tables[0].Rows[0]["LastUpdatedBy"].ToString());
                    model.LastUpdatedByName = DS.Tables[0].Rows[0]["LastUpdatedByEmp"].ToString();
                    model.LastUpdationDate = DS.Tables[0].Rows[0]["LastUpdationDate"].ToString();
                }
            }

            if (DS.Tables.Count != 0 && DS.Tables[1].Rows.Count > 0)
            {
                foreach (DataRow row in DS.Tables[1].Rows)
                {
                    creditNoteGrid.Add(new AccCreditNoteDetail
                    {
                        ItemCode = row["Itemcode"] != DBNull.Value ? Convert.ToInt32(row["Itemcode"]) : 0,
                        ItemName = row["ItemName"]?.ToString(),
                        PartCode = row["PartCode"]?.ToString(),
                        HSNNo = row["HSNNo"]?.ToString(),
                        BillQty = row["BillQty"] != DBNull.Value ? Convert.ToInt32(row["BillQty"]) : 0,
                        RejectedQty = row["RejectedQty"] != DBNull.Value ? Convert.ToInt32(row["RejectedQty"]) : 0,
                        Unit = row["Unit"]?.ToString(),
                        AltQty = row["AltQty"] != DBNull.Value ? Convert.ToInt32(row["AltQty"]) : 0,
                        AltUnit = row["AltUnit"]?.ToString(),
                        CRNRate = row["CRNRate"] != DBNull.Value ? Convert.ToInt32(row["CRNRate"]) : 0,
                        BillRate = row["BillRate"] != DBNull.Value ? Convert.ToInt32(row["BillRate"]) : 0,
                        UnitRate = row["UnitRate"]?.ToString(),
                        AltRate = row["AltRate"] != DBNull.Value ? Convert.ToInt32(row["AltRate"]) : 0,
                        NoOfCase = row["NoOfCase"] != DBNull.Value ? Convert.ToInt32(row["NoOfCase"]) : 0,
                        CostCenterId = row["CostcenetrId"] != DBNull.Value ? Convert.ToInt32(row["CostcenetrId"]) : 0,
                        DocAccountCode = row["DocAccountCode"] != DBNull.Value ? Convert.ToInt32(row["DocAccountCode"]) : 0,
                        DocAccountName = row["DocAccountName"]?.ToString(),
                        ItemAmount = row["ItemAmount"] != DBNull.Value ? Convert.ToInt32(row["ItemAmount"]) : 0,
                        DiscountPer = row["DiscountPer"] != DBNull.Value ? Convert.ToInt32(row["DiscountPer"]) : 0,
                        DiscountAmt = row["DiscountAmt"] != DBNull.Value ? Convert.ToInt32(row["DiscountAmt"]) : 0,
                        StoreId = row["StoreId"] != DBNull.Value ? Convert.ToInt32(row["StoreId"]) : 0,
                        //StoreName = row["SchdeliveryDate"]?.ToString(),
                        ItemSize = row["itemSize"]?.ToString(),
                        ItemDescription = row["ItemDescription"]?.ToString(),
                        Remark = row["Remark"]?.ToString(),
                    });
                }
                creditNoteGrid = creditNoteGrid.OrderBy(item => item.SeqNo).ToList();
                model.AccCreditNoteDetails = creditNoteGrid;
                model.ItemDetailGrid = creditNoteGrid;
            }

            if (DS.Tables.Count != 0 && DS.Tables[2].Rows.Count > 0)
            {
                foreach (DataRow row in DS.Tables[2].Rows)
                {
                    creditNoteAgainstBillGrid.Add(new AccCreditNoteAgainstBillDetail
                    {
                        //CreditNoteInvoiceNo = row["SchdeliveryDate"]?.ToString(),
                        //CreditNoteVoucherNo = row["SchdeliveryDate"]?.ToString(),
                        AgainstSaleBillBillNo = row["AgainstSalebillBillNo"]?.ToString(),
                        AgainstSaleBillYearCode = row["AgainstSaleBillYearCode"] != DBNull.Value ? Convert.ToInt32(row["AgainstSaleBillYearCode"]) : 0,
                        AgainstSaleBillDate = row["AgainstSaleBilldate"]?.ToString(),
                        AgainstSaleBillEntryId = row["AgainstSaleBillEntryId"] != DBNull.Value ? Convert.ToInt32(row["AgainstSaleBillEntryId"]) : 0,
                        AgainstSaleBillVoucherNo = row["AgainstSalebillVoucherNo"]?.ToString(),
                        SaleBillType = row["SaleBillTYpe"]?.ToString(),
                        AgainstPurchaseBillBillNo = row["AgainstPurchasebillBillNo"]?.ToString(),
                        AgainstPurchaseBillYearCode = row["AgainstPurchaseBillYearCode"] != DBNull.Value ? Convert.ToInt32(row["AgainstPurchaseBillYearCode"]) : 0,
                        AgainstPurchaseBillDate = row["AgainstPurchaseBilldate"]?.ToString(),
                        AgainstPurchaseBillEntryId = row["AgainstPurchaseBillEntryId"] != DBNull.Value ? Convert.ToInt32(row["AgainstPurchaseBillEntryId"]) : 0,
                        AgainstPurchaseVoucherNo = row["AgainstPurchaseVoucherNo"]?.ToString(),
                        PurchaseBillType = row["PurchaseBilltype"]?.ToString(),
                        ItemCode = row["BillItemCode"] != DBNull.Value ? Convert.ToInt32(row["BillItemCode"]) : 0,
                        PartCode = row["Partcode"]?.ToString(),
                        ItemName = row["ItemName"]?.ToString(),
                        BillQty = row["BillQty"] != DBNull.Value ? Convert.ToInt32(row["BillQty"]) : 0,
                        Unit = row["Unit"]?.ToString(),
                        BillRate = row["BillRate"] != DBNull.Value ? Convert.ToInt32(row["BillRate"]) : 0,
                        DiscountPer = row["DiscountPer"] != DBNull.Value ? Convert.ToInt32(row["DiscountPer"]) : 0,
                        DiscountAmt = row["DiscountAmt"] != DBNull.Value ? Convert.ToInt32(row["DiscountAmt"]) : 0,
                        ItemSize = row["Itemsize"]?.ToString(),
                        Amount = row["Amount"] != DBNull.Value ? Convert.ToInt32(row["Amount"]) : 0,
                        PONO = row["PONO"]?.ToString(),
                        PODate = row["PODate"]?.ToString(),
                        POEntryId = row["POEntryId"] != DBNull.Value ? Convert.ToInt32(row["POEntryId"]) : 0,
                        POYearCode = row["POYearCode"] != DBNull.Value ? Convert.ToInt32(row["POYearCode"]) : 0,
                        PoRate = row["PoRate"] != DBNull.Value ? Convert.ToInt32(row["PoRate"]) : 0,
                        PoAmmNo = row["poammno"]?.ToString(),
                        SONO = row["SONO"]?.ToString(),
                        SOYearCode = row["SOYearcode"] != DBNull.Value ? Convert.ToInt32(row["SOYearcode"]) : 0,
                        SODate = row["SODate"]?.ToString(),
                        CustOrderNo = row["CustOrderNo"]?.ToString(),
                        SOEntryId = row["SOEntryId"] != DBNull.Value ? Convert.ToInt32(row["SOEntryId"]) : 0,
                        BatchNo = row["BatchNo"]?.ToString(),
                        UniqueBatchNo = row["UniqueBatchNo"]?.ToString(),
                        AltQty = row["AltQty"] != DBNull.Value ? Convert.ToInt32(row["AltQty"]) : 0,
                        AltUnit = row["AltUnit"]?.ToString(),
                    });
                }
                model.AccCreditNoteAgainstBillDetails = creditNoteAgainstBillGrid;
            }

            if (DS.Tables.Count != 0 && DS.Tables[3].Rows.Count > 0)
            {
                foreach (DataRow row in DS.Tables[3].Rows)
                {
                    TaxGrid.Add(new TaxModel
                    {
                        TxSeqNo = row["SeqNo"] != DBNull.Value ? Convert.ToInt32(row["SeqNo"]) : 0,
                        TxType = row["Type"]?.ToString(),
                        TxPartName = row["PartCode"]?.ToString(),
                        TxItemName = row["itemName"]?.ToString(),
                        TxItemCode = row["ItemCode"] != DBNull.Value ? Convert.ToInt32(row["ItemCode"]) : 0,
                        TxTaxTypeName = row["TaxTypeID"]?.ToString(),
                        TxAccountCode = row["TaxAccountCode"] != DBNull.Value ? Convert.ToInt32(row["TaxAccountCode"]) : 0,
                        TxAccountName = row["TaxName"]?.ToString(),
                        TxPercentg = row["TaxPer"] != DBNull.Value ? Convert.ToDecimal(row["TaxPer"]) : 0,
                        TxRoundOff = row["RoundOff"]?.ToString(),
                        TxAmount = row["Amount"] != DBNull.Value ? Convert.ToDecimal(row["Amount"]) : 0,
                        TxOnExp = row["TaxonExp"] != DBNull.Value ? Convert.ToDecimal(row["TaxonExp"]) : 0,
                        TxRefundable = row["TaxRefundable"]?.ToString(),
                        TxAdInTxable = row["AddInTaxable"]?.ToString(),
                        TxRemark = row["Remarks"]?.ToString()
                    });
                }
                model.TaxDetailGridd = TaxGrid;
            }

            if (DS.Tables.Count != 0 && DS.Tables[4].Rows.Count > 0)
            {
                var cnt1 = 1;
                foreach (DataRow row in DS.Tables[4].Rows)
                {
                    DRCRGrid.Add(new DbCrModel
                    {
                        AccountCode = row["Accountcode"] != DBNull.Value ? Convert.ToInt32(row["Accountcode"]) : 0,
                        AccountName = row["Account_Name"]?.ToString(),
                        DrAmt = row["DrAmt"] != DBNull.Value ? Convert.ToInt32(row["DrAmt"]) : 0,
                        CrAmt = row["CrAmt"] != DBNull.Value ? Convert.ToInt32(row["CrAmt"]) : 0,
                        AccEntryId = row["AccEntryId"] != DBNull.Value ? Convert.ToInt32(row["AccEntryId"]) : 0,
                        AccYearCode = row["AccYearCode"] != DBNull.Value ? Convert.ToInt32(row["AccYearCode"]) : 0,
                        VoucherNo = row["BillVouchNo"]?.ToString(),
                    });
                    cnt1++;
                }

                if (model.DRCRGrid == null)
                {
                    model.DRCRGrid = new DbCrModel();
                }

                model.DRCRGrid.DbCrDetailGrid = DRCRGrid;
            }

            if (DS.Tables.Count != 0 && DS.Tables[5].Rows.Count > 0)
            {
                var cnt1 = 1;
                foreach (DataRow row in DS.Tables[5].Rows)
                {
                    adjustGrid.Add(new AdjustmentModel
                    {
                        AdjSeqNo = cnt1,
                        AdjModeOfAdjstment = row["ModOfAdjust"]?.ToString(),
                        //AdjModeOfAdjstmentName = row["AccEntryId"]?.ToString(),
                        AdjNewRefNo = row["NewrefNo"]?.ToString(),
                        AdjDescription = row["Description"]?.ToString(),
                        //AdjDrCrName = row["AccEntryId"]?.ToString(),
                        AdjDrCr = row["DR/CR"]?.ToString(),
                        AdjPendAmt = row["AdjustmentAmt"] != DBNull.Value ? Convert.ToInt32(row["AdjustmentAmt"]) : 0,
                        AdjAdjstedAmt = row["AdjustmentAmt"] != DBNull.Value ? Convert.ToInt32(row["AdjustmentAmt"]) : 0,
                        AdjTotalAmt = row["BillAmt"] != DBNull.Value ? Convert.ToInt32(row["BillAmt"]) : 0, // BillAmt
                        //AdjRemainingAmt = row["AccEntryId"] != DBNull.Value ? Convert.ToInt32(row["AccEntryId"]) : 0,
                        AdjOpenEntryID = row["AgainstAccOpeningEntryId"] != DBNull.Value ? Convert.ToInt32(row["AgainstAccOpeningEntryId"]) : 0,
                        AdjOpeningYearCode = row["AgainstOpeningVoucheryearcode"] != DBNull.Value ? Convert.ToInt32(row["AgainstOpeningVoucheryearcode"]) : 0,
                        AdjDueDate = string.IsNullOrEmpty(row["DueDate"]?.ToString()) ? new DateTime() : Convert.ToDateTime(row["DueDate"]),
                        //AdjPurchOrderNo = row["AccEntryId"]?.ToString(),
                        //AdjPOYear = row["AccEntryId"] != DBNull.Value ? Convert.ToInt32(row["AccEntryId"]) : 0,
                        //AdjPODate = row["AccEntryId"] != DBNull.Value ? Convert.ToDateTime(row["AccEntryId"]) : new DateTime(),
                        //AdjPageName = row["AccEntryId"]?.ToString(),
                        //AdjAgnstAccEntryID = row["AccEntryId"] != DBNull.Value ? Convert.ToInt32(row["AccEntryId"]) : 0,
                        //AdjAgnstAccYearCode = row["yearcode"] != DBNull.Value ? Convert.ToInt32(row["yearcode"]) : 0,
                        //AdjAgnstModeOfAdjstment = row["AccEntryId"]?.ToString(),
                        //AdjAgnstModeOfAdjstmentName = row["AccEntryId"]?.ToString(),
                        //AdjAgnstNewRefNo = row["AccEntryId"]?.ToString(),
                        AdjAgnstVouchNo = row["VoucherNo"]?.ToString(),
                        AdjAgnstVouchType = row["VoucherType"]?.ToString(),
                        //AdjAgnstDrCrName = row["AccEntryId"]?.ToString(),
                        AdjAgnstDrCr = row["DR/CR"]?.ToString(),
                        //AdjAgnstPendAmt = row["AccEntryId"] != DBNull.Value ? Convert.ToInt32(row["AccEntryId"]) : 0,
                        AdjAgnstAdjstedAmt = row["AdjustmentAmt"] != DBNull.Value ? Convert.ToInt32(row["AdjustmentAmt"]) : 0,
                        AdjAgnstTotalAmt = row["BillAmt"] != DBNull.Value ? Convert.ToInt32(row["BillAmt"]) : 0,
                        AdjAgnstRemainingAmt = row["RemaingAmt"] != DBNull.Value ? Convert.ToInt32(row["RemaingAmt"]) : 0,
                        AdjAgnstOpenEntryID = row["AgainstAccOpeningEntryId"] != DBNull.Value ? Convert.ToInt32(row["AgainstAccOpeningEntryId"]) : 0,
                        AdjAgnstOpeningYearCode = row["AgainstOpeningVoucheryearcode"] != DBNull.Value ? Convert.ToInt32(row["AgainstOpeningVoucheryearcode"]) : 0,
                        AdjAgnstVouchDate = row["voucherDate"] != DBNull.Value ? Convert.ToDateTime(row["voucherDate"]) : new DateTime(),
                        AdjAgnstAccEntryID = row["AccEntryId"] != DBNull.Value ? Convert.ToInt32(row["AccEntryId"]) : 0,
                        AdjAgnstAccYearCode = row["AccYearCode"] != DBNull.Value ? Convert.ToInt32(row["AccYearCode"]) : 0
                        //AdjAgnstTransType = row["AccEntryId"]?.ToString()
                    });
                    cnt1++;
                }

                if (model.adjustmentModel == null)
                {
                    model.adjustmentModel = new AdjustmentModel();
                }

                model.adjustmentModel.AdjAdjustmentDetailGrid = adjustGrid;
            }

            return model;
        }

        public async Task<ResponseResult> GetHSNUNIT(int itemCode)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "GetHSNUNIT"));
                SqlParams.Add(new SqlParameter("@itemcode", itemCode));
                _ResponseResult = await _IDataLogic.ExecuteDataSet("AccSP_CreditNoteMainDetail", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> NewEntryId(int YearCode, string CreditNoteVoucherDate, string SubVoucherName)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "NewEntryId"));
                SqlParams.Add(new SqlParameter("@CreditNoteYearCode", YearCode));
                SqlParams.Add(new SqlParameter("@CreditNoteVoucherDate", ParseFormattedDate(CreditNoteVoucherDate)));
                SqlParams.Add(new SqlParameter("@SubVoucherName", SubVoucherName));
                _ResponseResult = await _IDataLogic.ExecuteDataSet("AccSP_CreditNoteMainDetail", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }

        public async Task<ResponseResult> FillDetailFromPopupGrid(DataTable model, int itemCode, int popCt)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FillDetailFrompopupGrid"));
                SqlParams.Add(new SqlParameter("@dtAgaintBillNo", model));
                SqlParams.Add(new SqlParameter("@itemcode", itemCode));
                //SqlParams.Add(new SqlParameter("@popCt", popCt));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("AccSP_CreditNoteMainDetail", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }

        public async Task<ResponseResult> FillCustomerName(string againstSalePurchase)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FILLCUSTOMERNAME"));
                SqlParams.Add(new SqlParameter("@AgainstSalePurchase", againstSalePurchase));
                _ResponseResult = await _IDataLogic.ExecuteDataSet("AccSP_CreditNoteMainDetail", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> FillCreditNotePopUp(string againstSalePurchase, string fromBillDate, string toBillDate, int itemCode, int accountCode, int yearCode, string showAllBill)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "PopupData"));
                SqlParams.Add(new SqlParameter("@AgainstSalePurchase", againstSalePurchase));
                SqlParams.Add(new SqlParameter("@fromBilldate", fromBillDate));
                SqlParams.Add(new SqlParameter("@ToBilldate", toBillDate));
                SqlParams.Add(new SqlParameter("@Accountcode", accountCode));
                SqlParams.Add(new SqlParameter("@itemcode", itemCode));
                SqlParams.Add(new SqlParameter("@showAllBills", showAllBill));
                SqlParams.Add(new SqlParameter("@CreditNoteYearCode", yearCode));
                _ResponseResult = await _IDataLogic.ExecuteDataSet("AccSP_CreditNoteMainDetail", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }

        public async Task<ResponseResult> FillItems(string fromSaleBillDate, string toSaleBillDate, int accountCode, string showAllItems)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FILLItemNameFORNEWENTRY"));
                SqlParams.Add(new SqlParameter("@fromBilldate", fromSaleBillDate));
                SqlParams.Add(new SqlParameter("@ToBilldate", toSaleBillDate));
                SqlParams.Add(new SqlParameter("@Accountcode", accountCode));
                SqlParams.Add(new SqlParameter("@ShowAllItem", showAllItems));
                _ResponseResult = await _IDataLogic.ExecuteDataSet("AccSP_CreditNoteMainDetail", SqlParams);

                //if (_ResponseResult.Result != null && _ResponseResult.StatusCode == HttpStatusCode.OK && _ResponseResult.StatusText == "Success")
                //{
                //    PrepareView(_ResponseResult.Result, ref model, Mode);
                //}
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> GetFormRights(int userId)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "GetRights"));
                SqlParams.Add(new SqlParameter("@EmpId", userId));
                SqlParams.Add(new SqlParameter("@MainMenu", "Credit Note"));

                _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_ItemGroup", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }
        public async Task<ResponseResult> GetCostCenter()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "GetCostCenter"));
                _ResponseResult = await _IDataLogic.ExecuteDataSet("AccSP_CreditNoteMainDetail", SqlParams);

            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> GetDashboardData(string summaryDetail, string fromdate, string toDate)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                fromdate = CommonFunc.ParseFormattedDate(fromdate);
                toDate = CommonFunc.ParseFormattedDate(toDate);
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "DASHBOARD"));
                SqlParams.Add(new SqlParameter("@SummDetail", summaryDetail));
                SqlParams.Add(new SqlParameter("@FromDate", fromdate));
                SqlParams.Add(new SqlParameter("@ToDate", toDate));
                _ResponseResult = await _IDataLogic.ExecuteDataSet("AccSP_CreditNoteMainDetail", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<AccCreditNoteDashboard> GetDashboardData(string FromDate, string ToDate, string ItemName, string PartCode, string CustomerName, string CreditNoteInvoiceNumber, string CreditNoteVoucherNo, string DashboardType)
        {
            DataSet? oDataSet = new DataSet();
            var model = new AccCreditNoteDashboard();
            try
            {
                using (SqlConnection myConnection = new SqlConnection(DBConnectionString))
                {
                    SqlCommand oCmd = new SqlCommand("AccSP_CreditNoteMainDetail", myConnection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    var fromDt = CommonFunc.ParseFormattedDate(FromDate);
                    var toDt = CommonFunc.ParseFormattedDate(ToDate);
                    oCmd.Parameters.AddWithValue("@Flag", "DASHBOARD");
                    oCmd.Parameters.AddWithValue("@SummDetail", DashboardType);
                    oCmd.Parameters.AddWithValue("@Fromdate", fromDt);
                    oCmd.Parameters.AddWithValue("@toDate", toDt);
                    oCmd.Parameters.AddWithValue("@CreditNoteInvoiceNo", CreditNoteInvoiceNumber);
                    oCmd.Parameters.AddWithValue("@CreditNoteVoucherNo", CreditNoteVoucherNo);
                    oCmd.Parameters.AddWithValue("@partcode", PartCode);
                    oCmd.Parameters.AddWithValue("@ItemName", ItemName);
                    oCmd.Parameters.AddWithValue("@CustVendName", CustomerName);

                    await myConnection.OpenAsync();
                    using (SqlDataAdapter oDataAdapter = new SqlDataAdapter(oCmd))
                    {
                        oDataAdapter.Fill(oDataSet);
                    }
                }
                if (DashboardType == "Summary")
                {
                    if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                    {
                        model.CreditNoteDashboard = (from DataRow dr in oDataSet.Tables[0].Rows
                                                     select new AccCreditNoteDashboard
                                                     {
                                                         AccountName = dr["AccountName"].ToString(),
                                                         AccountCode = Convert.ToInt32(dr["AccountCode"]),
                                                         GSTNO = dr["GSTNO"].ToString(),
                                                         CreditNoteEntryId = Convert.ToInt32(dr["CreditNoteEntryId"]),
                                                         CreditNoteInvoiceNo = dr["CreditNoteInvoiceNo"].ToString(),
                                                         CreditNoteInvoiceDate = dr["CreditNoteInvoiceDate"].ToString().Split(" ")[0],
                                                         SubVoucherName = dr["SubVoucherName"].ToString(),
                                                         CreditNoteVoucherNo = dr["CreditNoteVoucherNo"].ToString(),
                                                         CreditNoteVoucherDate = dr["CreditNoteVoucherDate"].ToString(),
                                                         AgainstSalePurchase = dr["AgainstSalePurchase"].ToString(),
                                                         Taxableamt = Convert.ToDecimal(dr["Taxableamt"]),
                                                         BillAmt = Convert.ToDecimal(dr["BillAmt"]),
                                                         NetAmt = Convert.ToDecimal(dr["NetAmt"]),
                                                         ItemService = dr["ItemService"].ToString(),
                                                         INVOICETYPE = dr["INVOICETYPE"].ToString(),
                                                         EInvNo = dr["EInvNo"].ToString(),
                                                         EinvGenerated = dr["EinvGenerated"].ToString(),
                                                         MachineName = dr["MachineName"].ToString(),
                                                         ActualEnteredByName = dr["ActualEntryByEmp"].ToString(),
                                                         ActualEntryDate = dr["ActualEntryDate"].ToString(),
                                                         LastUpdatedByName = dr["LastUpdatedByEmp"].ToString(),
                                                         LastUpdationDate = dr["LastUpdationDate"].ToString(),
                                                         RoundoffType = dr["RoundoffType"].ToString(),
                                                         CustVendAddress = dr["CustVendAddress"].ToString(),
                                                         StateNameofSupply = dr["StateNameofSupply"].ToString(),
                                                         StateCode = dr["StateCode"].ToString(),
                                                         CityofSupply = dr["CityofSupply"].ToString(),
                                                         CountryOfSupply = dr["CountryOfSupply"].ToString(),
                                                         PaymentTerm = dr["PaymentTerm"].ToString(),
                                                         Transporter = dr["Transporter"].ToString(),
                                                         ToatlDiscountPercent = Convert.ToDecimal(dr["ToatlDiscountPercent"]),
                                                         RoundOffAmt = Convert.ToDecimal(dr["RoundOffAmt"]),
                                                         Vehicleno = dr["Vehicleno"].ToString(),
                                                         TotalDiscountAmount = Convert.ToDecimal(dr["TotalDiscountAmount"]),
                                                         Remark = dr["Remark"].ToString(),
                                                         CC = dr["CC"].ToString(),
                                                         Uid = Convert.ToInt32(dr["Uid"]),
                                                         EntryFreezToAccounts = dr["EntryFreezToAccounts"].ToString(),
                                                         BalanceSheetClosed = dr["BalanceSheetClosed"].ToString(),
                                                         AttachmentFilePath1 = dr["AttachmentFilePath1"].ToString(),
                                                         AttachmentFilePath2 = dr["AttachmentFilePath2"].ToString(),
                                                         AttachmentFilePath3 = dr["AttachmentFilePath3"].ToString(),
                                                         CreditNoteYearCode = Convert.ToInt32(dr["CreditNoteYearCode"])
                                                     }).ToList();
                    }
                }
                else
                {
                    if (oDataSet.Tables.Count > 0 && oDataSet.Tables[0].Rows.Count > 0)
                    {
                        model.CreditNoteDashboard = (from DataRow dr in oDataSet.Tables[0].Rows
                                                     select new AccCreditNoteDashboard
                                                     {
                                                         AccountName = dr["AccountName"].ToString(),
                                                         AccountCode = Convert.ToInt32(dr["AccountCode"]),
                                                         GSTNO = dr["GSTNO"].ToString(),
                                                         CreditNoteEntryId = Convert.ToInt32(dr["CreditNoteEntryId"]),
                                                         CreditNoteInvoiceNo = dr["CreditNoteInvoiceNo"].ToString(),
                                                         CreditNoteInvoiceDate = dr["CreditNoteInvoiceDate"].ToString().Split(" ")[0],
                                                         SubVoucherName = dr["SubVoucherName"].ToString(),
                                                         CreditNoteVoucherNo = dr["CreditNoteVoucherNo"].ToString(),
                                                         CreditNoteVoucherDate = dr["CreditNoteVoucherDate"].ToString(),
                                                         AgainstSalePurchase = dr["AgainstSalePurchase"].ToString(),
                                                         Taxableamt = Convert.ToDecimal(dr["Taxableamt"]),
                                                         BillAmt = Convert.ToDecimal(dr["BillAmt"]),
                                                         NetAmt = Convert.ToDecimal(dr["NetAmt"]),
                                                         ItemService = dr["ItemService"].ToString(),
                                                         INVOICETYPE = dr["INVOICETYPE"].ToString(),
                                                         EInvNo = dr["EInvNo"].ToString(),
                                                         EinvGenerated = dr["EinvGenerated"].ToString(),
                                                         MachineName = dr["MachineName"].ToString(),
                                                         ActualEnteredByName = dr["ActualEntryByEmp"].ToString(),
                                                         ActualEntryDate = dr["ActualEntryDate"].ToString(),
                                                         LastUpdatedByName = dr["LastUpdatedByEmp"].ToString(),
                                                         LastUpdationDate = dr["LastUpdationDate"].ToString(),
                                                         RoundoffType = dr["RoundoffType"].ToString(),
                                                         CustVendAddress = dr["CustVendAddress"].ToString(),
                                                         StateNameofSupply = dr["StateNameofSupply"].ToString(),
                                                         StateCode = dr["StateCode"].ToString(),
                                                         CityofSupply = dr["CityofSupply"].ToString(),
                                                         CountryOfSupply = dr["CountryOfSupply"].ToString(),
                                                         PaymentTerm = dr["PaymentTerm"].ToString(),
                                                         Transporter = dr["Transporter"].ToString(),
                                                         ToatlDiscountPercent = Convert.ToDecimal(dr["ToatlDiscountPercent"]),
                                                         RoundOffAmt = Convert.ToDecimal(dr["RoundOffAmt"]),
                                                         Vehicleno = dr["Vehicleno"].ToString(),
                                                         TotalDiscountAmount = Convert.ToDecimal(dr["TotalDiscountAmount"]),
                                                         Remark = dr["Remark"].ToString(),
                                                         CC = dr["CC"].ToString(),
                                                         Uid = Convert.ToInt32(dr["Uid"]),
                                                         EntryFreezToAccounts = dr["EntryFreezToAccounts"].ToString(),
                                                         BalanceSheetClosed = dr["BalanceSheetClosed"].ToString(),
                                                         AttachmentFilePath1 = dr["AttachmentFilePath1"].ToString(),
                                                         AttachmentFilePath2 = dr["AttachmentFilePath2"].ToString(),
                                                         AttachmentFilePath3 = dr["AttachmentFilePath3"].ToString(),
                                                         CreditNoteYearCode = Convert.ToInt32(dr["CreditNoteYearCode"])
                                                     }).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            finally
            {
                oDataSet.Dispose();
            }
            return model;
        }
        internal async Task<ResponseResult> DeleteByID(int ID, int YC, int accountCode, string machineName)
        {
            var ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@Flag", "DELETE"));
                SqlParams.Add(new SqlParameter("@CreditNoteEntryId", ID));
                SqlParams.Add(new SqlParameter("@CreditNoteYearCode", YC));
                SqlParams.Add(new SqlParameter("@Accountcode", accountCode));
                SqlParams.Add(new SqlParameter("@MachineName", machineName));

                ResponseResult = await _IDataLogic.ExecuteDataSet("AccSP_CreditNoteMainDetail", SqlParams);

            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return ResponseResult;
        }

        internal async Task<ResponseResult> SaveCreditNote(AccCreditNoteModel model, DataTable CNGrid, DataTable TaxDetailDT, DataTable DrCrDetailDT, DataTable AdjDetailDT, DataTable DTAgainstBillDetail)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                var upDt = CommonFunc.ParseFormattedDate(DateTime.Today.ToString("dd/MM/yyyy"));
                if (model.Mode == "V" || model.Mode == "U")
                {
                    SqlParams.Add(new SqlParameter("@Flag", "UPDATE"));
                    SqlParams.Add(new SqlParameter("@LastUpdatedBy", model.LastUpdatedBy));
                    SqlParams.Add(new SqlParameter("@LastUpdationDate", upDt));
                }
                else
                {
                    SqlParams.Add(new SqlParameter("@Flag", "INSERT"));
                }

                //DateTime creditNoteInvoiceDt = new DateTime();
                //DateTime creditNoteVoucherDt = new DateTime();
                //DateTime lastUpdationDt = new DateTime();
                //DateTime actualDt = new DateTime();

                var creditNoteInvoiceDt = CommonFunc.ParseFormattedDate(model.CreditNoteInvoiceDate);
                var creditNoteVoucherDt = CommonFunc.ParseFormattedDate(model.CreditNoteVoucherDate);
                var actualDt = CommonFunc.ParseFormattedDate(model.ActualEntryDate);
                var lastUpdationDt = CommonFunc.ParseFormattedDate(DateTime.Now.ToString("dd/MM/yyyy"));

                SqlParams.Add(new SqlParameter("@CreditNoteEntryId", model.CreditNoteEntryId));
                SqlParams.Add(new SqlParameter("@CreditNoteYearCode", model.CreditNoteYearCode));
                SqlParams.Add(new SqlParameter("@CreditNoteInvoiceNo", model.CreditNoteInvoiceNo ?? string.Empty));
                SqlParams.Add(new SqlParameter("@CreditNoteInvoiceDate", creditNoteInvoiceDt == default ? string.Empty : creditNoteInvoiceDt));
                SqlParams.Add(new SqlParameter("@SubVoucherName", model.SubVoucherName ?? string.Empty));
                SqlParams.Add(new SqlParameter("@CreditNoteVoucherNo", model.CreditNoteVoucherNo ?? string.Empty));
                SqlParams.Add(new SqlParameter("@CreditNoteVoucherDate", creditNoteVoucherDt == default ? string.Empty : creditNoteVoucherDt));
                SqlParams.Add(new SqlParameter("@AgainstSalePurchase", model.AgainstSalePurchase ?? string.Empty));
                SqlParams.Add(new SqlParameter("@AccountCode", model.AccountCode));
                SqlParams.Add(new SqlParameter("@CustVendAddress", model.CustVendAddress ?? string.Empty));
                SqlParams.Add(new SqlParameter("@StateNameofSupply", model.StateNameofSupply ?? string.Empty));
                SqlParams.Add(new SqlParameter("@StateCode", model.StateCode ?? string.Empty));
                SqlParams.Add(new SqlParameter("@CityofSupply", model.CityofSupply ?? string.Empty));
                SqlParams.Add(new SqlParameter("@CountryOfSupply", model.CountryOfSupply ?? string.Empty));
                SqlParams.Add(new SqlParameter("@PaymentTerm", model.PaymentTerm ?? string.Empty));
                SqlParams.Add(new SqlParameter("@PaymentCreditDay", model.PaymentCreditDay));
                SqlParams.Add(new SqlParameter("@GSTNO", model.GSTNO ?? string.Empty));
                SqlParams.Add(new SqlParameter("@GstRegUnreg", model.GstRegUnreg ?? string.Empty));
                SqlParams.Add(new SqlParameter("@Transporter", model.Transporter ?? string.Empty));
                SqlParams.Add(new SqlParameter("@Vehicleno", model.Vehicleno ?? string.Empty));
                SqlParams.Add(new SqlParameter("@BillAmt", model.BillAmt));
                SqlParams.Add(new SqlParameter("@RoundOffAmt", model.RoundOffAmt));
                SqlParams.Add(new SqlParameter("@RoundoffType", model.RoundoffType ?? string.Empty));
                SqlParams.Add(new SqlParameter("@Taxableamt", model.Taxableamt));
                SqlParams.Add(new SqlParameter("@ToatlDiscountPercent", model.ToatlDiscountPercent));
                SqlParams.Add(new SqlParameter("@TotalDiscountAmount", model.TotalDiscountAmount));
                SqlParams.Add(new SqlParameter("@NetAmt", model.NetAmt));
                SqlParams.Add(new SqlParameter("@Remark", model.Remark ?? string.Empty));
                SqlParams.Add(new SqlParameter("@CC", model.CC ?? string.Empty));
                SqlParams.Add(new SqlParameter("@Uid", model.Uid));
                SqlParams.Add(new SqlParameter("@ItemService", model.ItemService ?? string.Empty));
                SqlParams.Add(new SqlParameter("@INVOICETYPE", model.INVOICETYPE ?? string.Empty));
                SqlParams.Add(new SqlParameter("@MachineName", model.MachineName ?? string.Empty));
                SqlParams.Add(new SqlParameter("@ActualEntryDate", actualDt == default ? string.Empty : actualDt));
                SqlParams.Add(new SqlParameter("@ActualEnteredBy", model.ActualEnteredBy));
                SqlParams.Add(new SqlParameter("@EntryFreezToAccounts", model.EntryFreezToAccounts));
                SqlParams.Add(new SqlParameter("@BalanceSheetClosed", model.BalanceSheetClosed));
                SqlParams.Add(new SqlParameter("@EInvNo", model.EInvNo ?? string.Empty));
                SqlParams.Add(new SqlParameter("@EinvGenerated", model.EinvGenerated));
                SqlParams.Add(new SqlParameter("@AttachmentFilePath1", model.AttachmentFilePath1 ?? string.Empty));
                SqlParams.Add(new SqlParameter("@AttachmentFilePath2", model.AttachmentFilePath2 ?? string.Empty));
                SqlParams.Add(new SqlParameter("@AttachmentFilePath3", model.AttachmentFilePath3 ?? string.Empty));

                //SqlParams.Add(new SqlParameter("@BooktrnsEntryId", model.CreditNoteEntryId));

                SqlParams.Add(new SqlParameter("@DTItemGrid", CNGrid));
                SqlParams.Add(new SqlParameter("@DTTaxGrid", TaxDetailDT));
                SqlParams.Add(new SqlParameter("@DRCRDATA", DrCrDetailDT));
                SqlParams.Add(new SqlParameter("@AgainstRef", AdjDetailDT));
                SqlParams.Add(new SqlParameter("@dtAgaintBillNo", DTAgainstBillDetail));

                _ResponseResult = await _IDataLogic.ExecuteDataTable("AccSP_CreditNoteMainDetail", SqlParams);

            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }

        public async Task<ResponseResult> FillSubVoucher()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FillSubvoucher"));
                _ResponseResult = await _IDataLogic.ExecuteDataSet("AccSP_CreditNoteMainDetail", SqlParams);

            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }

    }
}
