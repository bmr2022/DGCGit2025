using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Data.DAL
{
    public class PurchaseRejectionDAL
    {
        private readonly IDataLogic _IDataLogic;
        private readonly string DBConnectionString = string.Empty;
        private IDataReader? Reader;
        public PurchaseRejectionDAL(IConfiguration configuration, IDataLogic iDataLogic)
        {
            DBConnectionString = configuration.GetConnectionString("eTactDB");
            _IDataLogic = iDataLogic;
        }

        public async Task<AccPurchaseRejectionModel> GetViewByID(int ID, int YearCode, string mode)
        {
            var model = new AccPurchaseRejectionModel();
            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@Flag", "VIEWBYID"));
                SqlParams.Add(new SqlParameter("@EntryID", ID));
                SqlParams.Add(new SqlParameter("@YearCode", YearCode));
                var _ResponseResult = await _IDataLogic.ExecuteDataSet("AccSPPurchaseRejectionMainDetail", SqlParams);

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
        private static AccPurchaseRejectionModel PrepareView(DataSet DS, ref AccPurchaseRejectionModel? model, string Mode)
        {
            var ItemGrid = new List<AccPurchaseRejectionDetail>();
            var creditNoteGrid = new List<AccPurchaseRejectionDetail>();
            var creditNoteAgainstBillGrid = new List<AccPurchaseRejectionAgainstBillDetail>();
            var TaxGrid = new List<TaxModel>();
            var DRCRGrid = new List<DbCrModel>();
            var adjustGrid = new List<AdjustmentModel>();
            DS.Tables[0].TableName = "PurchaseRejectionModel";
            DS.Tables[1].TableName = "PurchaseRejectionDetail";
            DS.Tables[2].TableName = "PurchaseRejectionAgainstBillGrid";
            DS.Tables[2].TableName = "PurchaseRejectionTaxDetail";
            DS.Tables[3].TableName = "DRCRDetail";
            DS.Tables[4].TableName = "AdjustmentDetail";
            int cnt = 0;

            model.PurchaseRejEntryId = DS.Tables[0].Rows[0]["PurchaseRejectionEntryId"] != DBNull.Value ? Convert.ToInt32(DS.Tables[0].Rows[0]["PurchaseRejectionEntryId"]) : 0;
            model.PurchaseRejYearCode = DS.Tables[0].Rows[0]["PurchaseRejectionYearCode"] != DBNull.Value ? Convert.ToInt32(DS.Tables[0].Rows[0]["PurchaseRejectionYearCode"]) : 0;
            model.PurchaseRejectionInvoiceNo = DS.Tables[0].Rows[0]["PurchaseRejectionInvoiceNo"]?.ToString();
            model.PurchaseRejectionInvoiceDate = DS.Tables[0].Rows[0]["PurchaseRejectionInvoiceDate"]?.ToString();
            model.SubVoucherName = DS.Tables[0].Rows[0]["SubVoucherName"]?.ToString();
            model.PurchaseRejectionVoucherNo = DS.Tables[0].Rows[0]["PurchaseRejectionVoucherNo"]?.ToString();
            //model.PurchaseRejectionVoucherDate = DS.Tables[0].Rows[0]["PurchaseRejectionVoucherDate"]?.ToString();
            //model.AgainstSalePurchase = DS.Tables[0].Rows[0]["AgainstSalePurchase"]?.ToString();
            model.AccountCode = DS.Tables[0].Rows[0]["AccountCode"] != DBNull.Value ? Convert.ToInt32(DS.Tables[0].Rows[0]["AccountCode"]) : 0;
            model.AccountName = DS.Tables[0].Rows[0]["CustVendName"]?.ToString();
            model.CustVendAddress = DS.Tables[0].Rows[0]["CustVendAddress"]?.ToString();
            model.StateNameofSupply = DS.Tables[0].Rows[0]["StateNameofSupply"]?.ToString();
            model.StateCode = DS.Tables[0].Rows[0]["StateCode"]?.ToString();
            model.CityofSupply = DS.Tables[0].Rows[0]["CityofSupply"]?.ToString();
            model.CountryOfSupply = DS.Tables[0].Rows[0]["CountryOfSupply"]?.ToString();
            model.PaymentTerm = DS.Tables[0].Rows[0]["PaymentTerm"]?.ToString();
            //model.PaymentCreditDay = DS.Tables[0].Rows[0]["PaymentCreditDay"] != DBNull.Value ? Convert.ToInt32(DS.Tables[0].Rows[0]["PaymentCreditDay"]) : 0;
            model.GSTNO = DS.Tables[0].Rows[0]["GSTNO"]?.ToString();
            //model.GstRegUnreg = DS.Tables[0].Rows[0]["GstRegUnreg"]?.ToString();
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
            //model.ItemService = DS.Tables[0].Rows[0]["ItemService"]?.ToString();
            //model.INVOICETYPE = DS.Tables[0].Rows[0]["INVOICETYPE"]?.ToString();
            model.MachineName = DS.Tables[0].Rows[0]["MachineName"]?.ToString();
            model.ActualEntryDate = DS.Tables[0].Rows[0]["ActualEntryDate"]?.ToString();
            model.ActualEnteredBy = DS.Tables[0].Rows[0]["ActualEnteredBy"] != DBNull.Value ? Convert.ToInt32(DS.Tables[0].Rows[0]["ActualEnteredBy"]) : 0;
            model.ActualEnteredByName = DS.Tables[0].Rows[0]["ActualEntryByEmp"]?.ToString();
            model.LastUpdatedBy = DS.Tables[0].Rows[0]["LastUpdatedBy"] != DBNull.Value ? Convert.ToInt32(DS.Tables[0].Rows[0]["LastUpdatedBy"]) : 0;
            model.LastUpdatedByName = DS.Tables[0].Rows[0]["LastUpdatedByEmp"]?.ToString();
            model.LastUpdationDate = DS.Tables[0].Rows[0]["LastUpdationDate"]?.ToString();
            //model.EntryFreezToAccounts = DS.Tables[0].Rows[0]["EntryFreezToAccounts"]?.ToString();
            model.BalanceSheetClosed = DS.Tables[0].Rows[0]["BalanceSheetClosed"]?.ToString();
            //model.EInvNo = DS.Tables[0].Rows[0]["EInvNo"]?.ToString();
            //model.EinvGenerated = DS.Tables[0].Rows[0]["EinvGenerated"]?.ToString();
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
                    creditNoteGrid.Add(new AccPurchaseRejectionDetail
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
                        PRRate = row["PRRate"] != DBNull.Value ? Convert.ToInt32(row["PRRate"]) : 0,
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
                model.AccPurchaseRejectionDetails = creditNoteGrid;
                model.ItemDetailGrid = creditNoteGrid;
            }

            if (DS.Tables.Count != 0 && DS.Tables[1].Rows.Count > 0)
            {
                foreach (DataRow row in DS.Tables[1].Rows)
                {
                    creditNoteAgainstBillGrid.Add(new AccPurchaseRejectionAgainstBillDetail
                    {
                        //PurchaseRejectionInvoiceNo = row["SchdeliveryDate"]?.ToString(),
                        //PurchaseRejectionVoucherNo = row["SchdeliveryDate"]?.ToString(),
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
                        //PartCode = row["SchdeliveryDate"]?.ToString(),
                        //ItemName = row["BillQty"]?.ToString(),
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
                model.AccPurchaseRejectionAgainstBillDetails = creditNoteAgainstBillGrid;
            }

            if (DS.Tables.Count != 0 && DS.Tables[2].Rows.Count > 0)
            {
                foreach (DataRow row in DS.Tables[2].Rows)
                {
                    TaxGrid.Add(new TaxModel
                    {
                        TxSeqNo = row["SeqNo"] != DBNull.Value ? Convert.ToInt32(row["SeqNo"]) : 0,
                        TxType = row["Type"]?.ToString(),
                        TxPartName = row["PartCode"]?.ToString(),
                        TxItemName = row["Item_Name"]?.ToString(),
                        TxItemCode = row["ItemCode"] != DBNull.Value ? Convert.ToInt32(row["ItemCode"]) : 0,
                        TxTaxTypeName = row["TaxTypeID"]?.ToString(),
                        TxAccountCode = row["TaxAccountCode"] != DBNull.Value ? Convert.ToInt32(row["TaxAccountCode"]) : 0,
                        TxAccountName = row["TaxAccountName"]?.ToString(),
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
        public async Task<ResponseResult> NewEntryId(int YearCode)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "NewEntryId"));
                SqlParams.Add(new SqlParameter("@PurchaseRejYearCode", YearCode));
                _ResponseResult = await _IDataLogic.ExecuteDataSet("AccSPPurchaseRejectionMainDetail", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> FillDocument(string ShowAllDoc)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FillDocument"));
                SqlParams.Add(new SqlParameter("@ShowAllDoc", ShowAllDoc));
                _ResponseResult = await _IDataLogic.ExecuteDataSet("AccSPPurchaseRejectionMainDetail", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> FillCustomerName(string ShowAllParty, int? PurchaseRejYearCode)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@flag", "FillAccountName"));
                SqlParams.Add(new SqlParameter("@ShowAllParty", ShowAllParty));
                SqlParams.Add(new SqlParameter("@PurchaseRejYearCode", PurchaseRejYearCode ?? 0));
                _ResponseResult = await _IDataLogic.ExecuteDataSet("AccSPPurchaseRejectionMainDetail", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> GetStateGST(int Code)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "GetStateGST"));
                SqlParams.Add(new SqlParameter("@accountcode", Code));
                _ResponseResult = await _IDataLogic.ExecuteDataSet("AccSPPurchaseRejectionMainDetail", SqlParams);
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
