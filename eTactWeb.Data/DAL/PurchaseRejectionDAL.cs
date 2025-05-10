using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace eTactWeb.Data.DAL
{
    public class PurchaseRejectionDAL
    {
        private readonly IDataLogic _IDataLogic;
        private readonly string DBConnectionString = string.Empty;
        private IDataReader? Reader;
        private readonly ConnectionStringService _connectionStringService;
        public PurchaseRejectionDAL(IConfiguration configuration, IDataLogic iDataLogic, ConnectionStringService connectionStringService)
        {
            //DBConnectionString = configuration.GetConnectionString("eTactDB");
            _connectionStringService = connectionStringService;
            DBConnectionString = _connectionStringService.GetConnectionString();
            _IDataLogic = iDataLogic;
        }

        public async Task<AccPurchaseRejectionModel> GetViewByID(int ID, int YearCode, string mode)
        {
            var model = new AccPurchaseRejectionModel();
            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@Flag", "VIEWBYID"));
                SqlParams.Add(new SqlParameter("@PurchaseRejEntryId", ID));
                SqlParams.Add(new SqlParameter("@PurchaseRejYearCode", YearCode));
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
            var purchaseRejectionGrid = new List<AccPurchaseRejectionDetail>();
            var purchaseRejectionAgainstBillGrid = new List<AccPurchaseRejectionAgainstBillDetail>();
            var TaxGrid = new List<TaxModel>();
            var DRCRGrid = new List<DbCrModel>();
            var adjustGrid = new List<AdjustmentModel>();
            DS.Tables[0].TableName = "PurchaseRejectionModel";
            DS.Tables[1].TableName = "PurchaseRejectionDetail";
            DS.Tables[2].TableName = "PurchaseRejectionTaxDetail";
            DS.Tables[3].TableName = "DRCRDetail";
            DS.Tables[4].TableName = "AdjustmentDetail";
            DS.Tables[5].TableName = "PurchaseRejectionAgainstBillGrid";
            int cnt = 0;
            #region MainTable
            model.PurchaseRejEntryId = DS.Tables[0].Rows[0]["PurchaseRejEntryId"] != DBNull.Value ? Convert.ToInt32(DS.Tables[0].Rows[0]["PurchaseRejEntryId"]) : 0;
            model.PurchaseRejYearCode = DS.Tables[0].Rows[0]["PurchaseRejYearCode"] != DBNull.Value ? Convert.ToInt32(DS.Tables[0].Rows[0]["PurchaseRejYearCode"]) : 0;
            model.SubVoucherName = DS.Tables[0].Rows[0]["SubVoucherName"]?.ToString();
            model.DebitNotePurchaseRejection = DS.Tables[0].Rows[0]["DebitNotePurchaseRejection"]?.ToString();
            model.PurchaseRejEntryDate = DS.Tables[0].Rows[0]["PurchaseRejEntryDate"]?.ToString();
            model.PurchaseRejectionInvoiceNo = DS.Tables[0].Rows[0]["InvoiceNo"]?.ToString();
            model.PurchaseRejectionInvoiceDate = DS.Tables[0].Rows[0]["InvoiceDate"]?.ToString();
            model.PurchaseRejectionVoucherNo = DS.Tables[0].Rows[0]["PurchaserejVoucherNo"]?.ToString();
            model.VoucherNo = DS.Tables[0].Rows[0]["VoucherNo"]?.ToString();
            //model.PurchaseRejectionVoucherDate = DS.Tables[0].Rows[0]["PurchaseRejectionVoucherDate"]?.ToString();
            //model.AgainstSalePurchase = DS.Tables[0].Rows[0]["AgainstSalePurchase"]?.ToString();
            model.AccountCode = DS.Tables[0].Rows[0]["AccountCode"] != DBNull.Value ? Convert.ToInt32(DS.Tables[0].Rows[0]["AccountCode"]) : 0;
            model.AccountName = DS.Tables[0].Rows[0]["VendorName"]?.ToString();
            model.CustVendAddress = DS.Tables[0].Rows[0]["VendoreAddress"]?.ToString();
            model.StateNameofSupply = DS.Tables[0].Rows[0]["StateName"]?.ToString();
            model.StateCode = DS.Tables[0].Rows[0]["StateCode"]?.ToString();
            model.CityofSupply = DS.Tables[0].Rows[0]["City"]?.ToString();
            model.CountryOfSupply = DS.Tables[0].Rows[0]["Country"]?.ToString();
            model.CurrencyId = !string.IsNullOrEmpty(DS.Tables[0].Rows[0]["CurrencyId"].ToString()) && DS.Tables[0].Rows[0]["CurrencyId"].ToString() !=  "0" ? Convert.ToInt32(DS.Tables[0].Rows[0]["CurrencyId"].ToString()) : 0;
            model.ExchangeRate = DS.Tables[0].Rows[0]["ExchangeRate"].ToString();
            model.PaymentTerm = DS.Tables[0].Rows[0]["PaymentTerm"]?.ToString();
            //model.PaymentCreditDay = DS.Tables[0].Rows[0]["PaymentCreditDay"] != DBNull.Value ? Convert.ToInt32(DS.Tables[0].Rows[0]["PaymentCreditDay"]) : 0;
            model.GSTNO = DS.Tables[0].Rows[0]["GSTNO"]?.ToString();
            //model.GstRegUnreg = DS.Tables[0].Rows[0]["GstRegUnreg"]?.ToString();
            model.DomesticExportNEPZ = DS.Tables[0].Rows[0]["DomesticExportNEPZ"]?.ToString();
            model.Transporter = DS.Tables[0].Rows[0]["Transporter"]?.ToString();
            model.Vehicleno = DS.Tables[0].Rows[0]["Vehicleno"]?.ToString();
            model.Distance = DS.Tables[0].Rows[0]["Distance"]?.ToString();
            model.BillAmt = DS.Tables[0].Rows[0]["BillAmt"] != DBNull.Value ? Convert.ToInt32(DS.Tables[0].Rows[0]["BillAmt"]) : 0;
            model.RoundOffAmt = DS.Tables[0].Rows[0]["RoundOffAmt"] != DBNull.Value ? Convert.ToInt32(DS.Tables[0].Rows[0]["RoundOffAmt"]) : 0;
            model.RoundoffType = DS.Tables[0].Rows[0]["RoundoffType"]?.ToString();
            model.Taxableamt = DS.Tables[0].Rows[0]["Taxableamt"] != DBNull.Value ? Convert.ToInt32(DS.Tables[0].Rows[0]["Taxableamt"]) : 0;
            model.ToatlDiscountPercent = DS.Tables[0].Rows[0]["ToatlDiscountPercent"] != DBNull.Value ? Convert.ToInt32(DS.Tables[0].Rows[0]["ToatlDiscountPercent"]) : 0;
            model.TotalDiscountAmount = DS.Tables[0].Rows[0]["TotalDiscountAmount"] != DBNull.Value ? Convert.ToInt32(DS.Tables[0].Rows[0]["TotalDiscountAmount"]) : 0;
            model.NetAmt = DS.Tables[0].Rows[0]["InvNetAmt"] != DBNull.Value ? Convert.ToInt32(DS.Tables[0].Rows[0]["InvNetAmt"]) : 0;
            model.PurchaserejRemark = DS.Tables[0].Rows[0]["PurchaserejRemark"]?.ToString();
            model.Remark = DS.Tables[0].Rows[0]["PurchaserejRemark"]?.ToString();
            model.CC = DS.Tables[0].Rows[0]["CC"]?.ToString();
            model.Uid = DS.Tables[0].Rows[0]["Uid"] != DBNull.Value ? Convert.ToInt32(DS.Tables[0].Rows[0]["Uid"]) : 0;
            //model.ItemService = DS.Tables[0].Rows[0]["ItemService"]?.ToString();
            //model.INVOICETYPE = DS.Tables[0].Rows[0]["INVOICETYPE"]?.ToString();
            model.MachineName = DS.Tables[0].Rows[0]["MachineName"]?.ToString();
            model.ActualEntryDate = DS.Tables[0].Rows[0]["ActualEntryDate"]?.ToString();
            model.ActualEnteredBy = DS.Tables[0].Rows[0]["ActualEnteredBy"] != DBNull.Value ? Convert.ToInt32(DS.Tables[0].Rows[0]["ActualEnteredBy"]) : 0;
            model.ActualEnteredByName = DS.Tables[0].Rows[0]["ActualEntryByEmpName"]?.ToString();
            model.LastUpdatedBy = DS.Tables[0].Rows[0]["LastUpdatedBy"] != DBNull.Value ? Convert.ToInt32(DS.Tables[0].Rows[0]["LastUpdatedBy"]) : 0;
            model.LastUpdatedByName = DS.Tables[0].Rows[0]["UpdatedByEmpName"]?.ToString();
            model.LastUpdationDate = DS.Tables[0].Rows[0]["LastUpdationDate"]?.ToString();
            //model.EntryFreezToAccounts = DS.Tables[0].Rows[0]["EntryFreezToAccounts"]?.ToString();
            model.BalanceSheetClosed = DS.Tables[0].Rows[0]["BalanceSheetClosed"]?.ToString();
            //model.EInvNo = DS.Tables[0].Rows[0]["EInvNo"]?.ToString();
            //model.EinvGenerated = DS.Tables[0].Rows[0]["EinvGenerated"]?.ToString();
            //model.AttachmentFilePath1 = DS.Tables[0].Rows[0]["AttachmentFilePath1"]?.ToString();
            //model.AttachmentFilePath2 = DS.Tables[0].Rows[0]["AttachmentFilePath2"]?.ToString();
            //model.AttachmentFilePath3 = DS.Tables[0].Rows[0]["AttachmentFilePath3"]?.ToString();

            if (Mode == "U" || Mode == "V")
            {
                if (DS.Tables[0].Rows[0]["UpdatedByEmpName"].ToString() != "")
                {
                    model.EntryByempId = !string.IsNullOrEmpty(DS.Tables[0].Rows[0]["EntryByempId"].ToString()) ? Convert.ToInt32(DS.Tables[0].Rows[0]["EntryByempId"].ToString()) : 0;
                    model.LastUpdatedBy = Convert.ToInt32(DS.Tables[0].Rows[0]["LastUpdatedBy"].ToString());
                    model.LastUpdatedByName = DS.Tables[0].Rows[0]["UpdatedByEmpName"].ToString();
                    model.LastUpdationDate = DS.Tables[0].Rows[0]["LastUpdationDate"].ToString();
                }
            }
            #endregion
            if (DS.Tables.Count != 0 && DS.Tables[1].Rows.Count > 0)
            {
                foreach (DataRow row in DS.Tables[1].Rows)
                {
                    purchaseRejectionGrid.Add(new AccPurchaseRejectionDetail
                    {
                        ItemCode = row["Itemcode"] != DBNull.Value ? Convert.ToInt32(row["Itemcode"]) : 0,
                        ItemName = row["ItemName"]?.ToString(),
                        PartCode = row["PartCode"]?.ToString(),
                        HSNNo = row["HSNNo"]?.ToString(),
                        BillQty = row["PurchBillQty"] != DBNull.Value ? Convert.ToInt32(row["PurchBillQty"]) : 0,
                        RejectedQty = row["RejectedQty"] != DBNull.Value ? Convert.ToInt32(row["RejectedQty"]) : 0,
                        Unit = row["Unit"]?.ToString(),
                        AltQty = row["AltQty"] != DBNull.Value ? Convert.ToInt32(row["AltQty"]) : 0,
                        AltUnit = row["AltUnit"]?.ToString(),
                        PRRate = row["RejRate"] != DBNull.Value ? Convert.ToInt32(row["RejRate"]) : 0,
                        BillRate = row["PurchBillRate"] != DBNull.Value ? Convert.ToInt32(row["PurchBillRate"]) : 0,
                        UnitRate = row["UnitRate"]?.ToString(),
                        AltRate = row["AltRate"] != DBNull.Value ? Convert.ToInt32(row["AltRate"]) : 0,
                        NoOfCase = row["NoOfCase"] != DBNull.Value ? Convert.ToInt32(row["NoOfCase"]) : 0,
                        CostCenterId = row["CostcenetrId"] != DBNull.Value ? Convert.ToInt32(row["CostcenetrId"]) : 0,
                        DocAccountCode = row["DocAccountCode"] != DBNull.Value ? Convert.ToInt32(row["DocAccountCode"]) : 0,
                        DocAccountName = row["DocumentName"]?.ToString(),
                        ItemAmount = row["ItemAmount"] != DBNull.Value ? Convert.ToInt32(row["ItemAmount"]) : 0,
                        DiscountPer = row["DiscountPer"] != DBNull.Value ? Convert.ToInt32(row["DiscountPer"]) : 0,
                        DiscountAmt = row["DiscountAmt"] != DBNull.Value ? Convert.ToInt32(row["DiscountAmt"]) : 0,
                        StoreId = row["StoreId"] != DBNull.Value ? Convert.ToInt32(row["StoreId"]) : 0,
                        StoreName = row["StoreName"]?.ToString(),
                        //Stockable = row["Stockable"]?.ToString(),
                        BatchNo = row["BatchNo"]?.ToString(),
                        UniqueBatchNo = row["UniqueBatchNo"]?.ToString(),
                        //CostCenterName = row["CostCenterName"]?.ToString(),
                        LotStock = row["LotStock"] != DBNull.Value ? Convert.ToInt32(row["LotStock"]) : 0,
                        TotalStock = row["TotalStock"] != DBNull.Value ? Convert.ToInt32(row["TotalStock"]) : 0,
                        ItemSize = row["itemSize"]?.ToString(),
                        ItemDescription = row["ItemDescription"]?.ToString(),
                        Remark = row["Remark"]?.ToString(),
                    });
                }
                purchaseRejectionGrid = purchaseRejectionGrid.OrderBy(item => item.SeqNo).ToList();
                model.AccPurchaseRejectionDetails = purchaseRejectionGrid;
                model.ItemDetailGrid = purchaseRejectionGrid;
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
                        TxItemName = row["itemNamePartCode"]?.ToString(),
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
                        AdjNewRefNo = row["NewrfNo"]?.ToString(),
                        //AdjDescription = row["Description"]?.ToString(),
                        //AdjDrCrName = row["AccEntryId"]?.ToString(),
                        AdjDrCr = row["DR/CR"]?.ToString(),
                        AdjPendAmt = row["AdjustmentAmt"] != DBNull.Value ? Convert.ToInt32(row["AdjustmentAmt"]) : 0,
                        AdjAdjstedAmt = row["AdjustmentAmt"] != DBNull.Value ? Convert.ToInt32(row["AdjustmentAmt"]) : 0,
                        AdjTotalAmt = row["BillAmt"] != DBNull.Value ? Convert.ToInt32(row["BillAmt"]) : 0, // BillAmt
                        //AdjRemainingAmt = row["AccEntryId"] != DBNull.Value ? Convert.ToInt32(row["AccEntryId"]) : 0,
                        AdjOpenEntryID = row["AgainstAccOpeningEntryId"] != DBNull.Value ? Convert.ToInt32(row["AgainstAccOpeningEntryId"]) : 0,
                        AdjOpeningYearCode = row["AgainstOpeningVoucheryearcode"] != DBNull.Value ? Convert.ToInt32(row["AgainstOpeningVoucheryearcode"]) : 0,
                        //AdjDueDate = string.IsNullOrEmpty(row["DueDate"]?.ToString()) ? new DateTime() : Convert.ToDateTime(row["DueDate"]),
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

            if (DS.Tables.Count != 0 && DS.Tables[5].Rows.Count > 0)
            {
                foreach (DataRow row in DS.Tables[5].Rows)
                {
                    purchaseRejectionAgainstBillGrid.Add(new AccPurchaseRejectionAgainstBillDetail
                    {
                        PurchaseRejectionInvoiceNo = row["PurchaseRejInvoiceNo"]?.ToString(),
                        PurchaseRejectionVoucherNo = row["PurchaseRejVoucherNo"]?.ToString(),
                        
                        //AgainstSaleBillBillNo = row["AgainstSalebillBillNo"]?.ToString(),
                        //AgainstSaleBillYearCode = row["AgainstSaleBillYearCode"] != DBNull.Value ? Convert.ToInt32(row["AgainstSaleBillYearCode"]) : 0,
                        //AgainstSaleBillDate = row["AgainstSaleBilldate"]?.ToString(),
                        //AgainstSaleBillEntryId = row["AgainstSaleBillEntryId"] != DBNull.Value ? Convert.ToInt32(row["AgainstSaleBillEntryId"]) : 0,
                        //AgainstSaleBillVoucherNo = row["AgainstSalebillVoucherNo"]?.ToString(),
                        //SaleBillType = row["SaleBillTYpe"]?.ToString(),
                        AgainstPurchaseBillBillNo = row["AgainstPurchasebillBillNo"]?.ToString(),
                        AgainstPurchaseBillYearCode = row["AgainstPurchaseBillYearCode"] != DBNull.Value ? Convert.ToInt32(row["AgainstPurchaseBillYearCode"]) : 0,
                        AgainstPurchaseBillDate = row["AgainstPurchaseBilldate"]?.ToString(),
                        AgainstPurchaseBillEntryId = row["AgainstPurchaseBillEntryId"] != DBNull.Value ? Convert.ToInt32(row["AgainstPurchaseBillEntryId"]) : 0,
                        AgainstPurchaseVoucherNo = row["AgainstPurchaseVoucherNo"]?.ToString(),
                        PurchaseBillType = row["DebitNotePurchaseRejection"]?.ToString(),
                        ItemCode = row["PurchaseRejItemCode"] != DBNull.Value ? Convert.ToInt32(row["PurchaseRejItemCode"]) : 0,
                        //PartCode = row["PartCode"]?.ToString(),
                        //ItemName = row["itemNamePartCode"]?.ToString(),
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
                        //SONO = row["SONO"]?.ToString(),
                        //SOYearCode = row["SOYearcode"] != DBNull.Value ? Convert.ToInt32(row["SOYearcode"]) : 0,
                        //SODate = row["SODate"]?.ToString(),
                        //CustOrderNo = row["CustOrderNo"]?.ToString(),
                        //SOEntryId = row["SOEntryId"] != DBNull.Value ? Convert.ToInt32(row["SOEntryId"]) : 0,
                        BatchNo = row["BatchNo"]?.ToString(),
                        UniqueBatchNo = row["UniqueBatchNo"]?.ToString(),
                        AltQty = row["AltQty"] != DBNull.Value ? Convert.ToInt32(row["AltQty"]) : 0,
                        AltUnit = row["AltUnit"]?.ToString(),
                    });
                }
                model.AccPurchaseRejectionAgainstBillDetails = purchaseRejectionAgainstBillGrid ?? new List<AccPurchaseRejectionAgainstBillDetail>();
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
        public async Task<ResponseResult> GetCostCenter()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "CostCenter"));
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
        public async Task<ResponseResult> FillCustomerName(string ShowAllParty, int? PurchaseRejYearCode, string DebitNotePurchaseRejection)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@flag", "FillAccountName"));
                SqlParams.Add(new SqlParameter("@ShowAllParty", ShowAllParty));
                SqlParams.Add(new SqlParameter("@PurchaseRejYearCode", PurchaseRejYearCode ?? 0));
                SqlParams.Add(new SqlParameter("@DebitNotePurchaseRejection", DebitNotePurchaseRejection));
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
        public async Task<ResponseResult> FillItems(int YearCode, int accountCode, string showAllItems, string Flag)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", Flag));
                SqlParams.Add(new SqlParameter("@PurchaseRejYearCode", YearCode));
                SqlParams.Add(new SqlParameter("@accountcode", accountCode));
                SqlParams.Add(new SqlParameter("@ShowAllItem", showAllItems));
                _ResponseResult = await _IDataLogic.ExecuteDataSet("AccSPPurchaseRejectionMainDetail", SqlParams);

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
        public async Task<ResponseResult> FillCurrency(int? AccountCode)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "Currency"));
                SqlParams.Add(new SqlParameter("@accountcode", AccountCode ?? 0));

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
        public async Task<ResponseResult> GetExchangeRate(string Currency)
        {
            var _ResponseResult = new ResponseResult();

            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@flag", "ExchangeRate"));
                SqlParams.Add(new SqlParameter("@EntryDate", DateTime.Today));
                SqlParams.Add(new SqlParameter("@Currency", Currency));

                _ResponseResult = await _IDataLogic.ExecuteDataTable("AccSPPurchaseRejectionMainDetail", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> FillStore()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FillStore"));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("AccSPPurchaseRejectionMainDetail", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> FillSubvoucher(int? PurchaseRejYearCode, string DebitNotePurchaseRejection)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@flag", "FillSubvoucher"));
                SqlParams.Add(new SqlParameter("@PurchaseRejYearCode", PurchaseRejYearCode ?? 0));
                SqlParams.Add(new SqlParameter("@DebitNotePurchaseRejection", DebitNotePurchaseRejection));
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
        public async Task<ResponseResult> GetHSNUNIT(int itemCode)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "GetHSNUNIT"));
                SqlParams.Add(new SqlParameter("@itemcode", itemCode));
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
        public async Task<ResponseResult> FillPurchaseRejectionPopUp(string DebitNotePurchaseRejection, string fromBillDate, string toBillDate, int itemCode, int accountCode, int yearCode, string showAllBill)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                fromBillDate = fromBillDate != null  ? CommonFunc.ParseFormattedDate(fromBillDate) : string.Empty;
                toBillDate = toBillDate != null ? CommonFunc.ParseFormattedDate(toBillDate) : string.Empty;
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "PopupData"));
                SqlParams.Add(new SqlParameter("@DebitNotePurchaseRejection", DebitNotePurchaseRejection));
                SqlParams.Add(new SqlParameter("@fromBilldate", fromBillDate));
                SqlParams.Add(new SqlParameter("@ToBilldate", toBillDate));
                SqlParams.Add(new SqlParameter("@Accountcode", accountCode));
                SqlParams.Add(new SqlParameter("@itemcode", itemCode));
                SqlParams.Add(new SqlParameter("@showAllBills", showAllBill));
                SqlParams.Add(new SqlParameter("@PurchaseRejYearCode", yearCode));
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
                _ResponseResult = await _IDataLogic.ExecuteDataTable("AccSPPurchaseRejectionMainDetail", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        internal async Task<ResponseResult> SavePurchaseRejection(AccPurchaseRejectionModel model, DataTable PRGrid, DataTable TaxDetailDT, DataTable DrCrDetailDT, DataTable AdjDetailDT, DataTable DTAgainstBillDetail)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var upDt = CommonFunc.ParseFormattedDate(DateTime.Now.ToString("dd/MM/yyyy"));
                var SqlParams = new List<dynamic>();
                if (model.Mode == "V" || model.Mode == "U")
                {
                    SqlParams.Add(new SqlParameter("@flag", "UPDATE"));
                    SqlParams.Add(new SqlParameter("@UpdatedBy", model.LastUpdatedBy));
                    SqlParams.Add(new SqlParameter("@LastUpdatedDate", upDt));
                }
                else
                {
                    SqlParams.Add(new SqlParameter("@flag", "INSERT"));
                }

                //DateTime purchaseRejectionInvoiceDt = new DateTime();
                //DateTime purchaseRejectionVoucherDt = new DateTime();
                //DateTime purchaseRejectionEntryDt = new DateTime();
                //DateTime lastUpdationDt = new DateTime();
                //DateTime actualDt = new DateTime();

                var purchaseRejectionInvoiceDt = CommonFunc.ParseFormattedDate(model.PurchaseRejectionInvoiceDate);
                var purchaseRejectionVoucherDt = CommonFunc.ParseFormattedDate(model.PurchaseRejectionVoucherDate);
                var purchaseRejectionEntryDt = CommonFunc.ParseFormattedDate(model.PurchaseRejEntryDate);
                var actualDt = CommonFunc.ParseFormattedDate(model.ActualEntryDate);
                

                SqlParams.Add(new SqlParameter("@PurchaseRejEntryId", model.PurchaseRejEntryId));
                SqlParams.Add(new SqlParameter("@PurchaseRejYearCode", model.PurchaseRejYearCode));
                SqlParams.Add(new SqlParameter("@PurchaseRejEntryDate", purchaseRejectionEntryDt == default ? string.Empty : purchaseRejectionEntryDt));
                SqlParams.Add(new SqlParameter("@DebitNotePurchaseRejection", model.DebitNotePurchaseRejection));
                SqlParams.Add(new SqlParameter("@PurchaseRejInvoiceNo", model.PurchaseRejectionInvoiceNo ?? string.Empty));
                SqlParams.Add(new SqlParameter("@InvoiceNo", model.PurchaseRejectionInvoiceNo ?? string.Empty));
                SqlParams.Add(new SqlParameter("@InvoiceDate", purchaseRejectionInvoiceDt == default ? string.Empty : purchaseRejectionInvoiceDt));
                SqlParams.Add(new SqlParameter("@InvoiceTime", purchaseRejectionInvoiceDt == default ? string.Empty : purchaseRejectionInvoiceDt));
                SqlParams.Add(new SqlParameter("@SubVoucherName", model.SubVoucherName ?? string.Empty));
                SqlParams.Add(new SqlParameter("@PurchaserejVoucherNo", model.PurchaseRejectionVoucherNo ?? string.Empty));
                SqlParams.Add(new SqlParameter("@VoucherNo", model.VoucherNo ?? string.Empty));
                //SqlParams.Add(new SqlParameter("@VoucherDate", purchaseRejectionInvoiceDt == default ? string.Empty : purchaseRejectionInvoiceDt));
                SqlParams.Add(new SqlParameter("@AccountCode", model.AccountCode));
                SqlParams.Add(new SqlParameter("@VendoreAddress", model.CustVendAddress ?? string.Empty));
                SqlParams.Add(new SqlParameter("@StateName", model.StateNameofSupply ?? string.Empty));
                SqlParams.Add(new SqlParameter("@StateCode", model.StateCode ?? string.Empty));
                SqlParams.Add(new SqlParameter("@City", model.CityofSupply ?? string.Empty));
                SqlParams.Add(new SqlParameter("@Country", model.CountryOfSupply ?? string.Empty));
                SqlParams.Add(new SqlParameter("@CurrencyId", model.CurrencyId ?? 0));
                SqlParams.Add(new SqlParameter("@ExchangeRate", model.ExchangeRate ?? string.Empty));
                SqlParams.Add(new SqlParameter("@PaymentTerm", model.PaymentTerm ?? string.Empty));
                //SqlParams.Add(new SqlParameter("@PaymentTerm", Convert.ToInt32(!string.IsNullOrEmpty(model.PaymentTerm) ? 1 : 0)));
                SqlParams.Add(new SqlParameter("@PaymentCreditDay", model.PaymentCreditDay));
                SqlParams.Add(new SqlParameter("@GSTNO", model.GSTNO ?? string.Empty));
                SqlParams.Add(new SqlParameter("@DomesticExportNEPZ", model.DomesticExportNEPZ ?? string.Empty));
                //SqlParams.Add(new SqlParameter("@GSTRegistered", model.GSTRegistered ?? string.Empty));
                SqlParams.Add(new SqlParameter("@Transporter", model.Transporter ?? string.Empty));
                SqlParams.Add(new SqlParameter("@Vehicleno", model.Vehicleno ?? string.Empty));
                SqlParams.Add(new SqlParameter("@Distance", model.Distance ?? string.Empty));
                SqlParams.Add(new SqlParameter("@BillAmt", model.BillAmt));
                SqlParams.Add(new SqlParameter("@RoundOffAmt", model.RoundOffAmt));
                //SqlParams.Add(new SqlParameter("@RoundOffAmt", model.TotalRoundOffAmt));
                SqlParams.Add(new SqlParameter("@RoundoffType", model.RoundoffType ?? string.Empty));
                SqlParams.Add(new SqlParameter("@Taxableamt", model.Taxableamt));
                //SqlParams.Add(new SqlParameter("@ToatlDiscountPercent", model.TotalDiscountPercentage));
                SqlParams.Add(new SqlParameter("@ToatlDiscountPercent", model.ToatlDiscountPercent));
                SqlParams.Add(new SqlParameter("@TotalDiscountAmount", model.TotalDiscountAmount));
                SqlParams.Add(new SqlParameter("@InvNetAmt", model.NetAmt));
                SqlParams.Add(new SqlParameter("@PurchaserejRemark", model.Remark ?? string.Empty));
                SqlParams.Add(new SqlParameter("@CC", model.CC ?? string.Empty));
                SqlParams.Add(new SqlParameter("@Uid", model.Uid));
                SqlParams.Add(new SqlParameter("@MachineName", model.MachineName ?? string.Empty));
                SqlParams.Add(new SqlParameter("@ActualEntryDate", actualDt == default ? string.Empty : actualDt));
                SqlParams.Add(new SqlParameter("@ActualEnteredBy", model.ActualEnteredBy));
                SqlParams.Add(new SqlParameter("@LastUpdatedBy", model.LastUpdatedBy));
                SqlParams.Add(new SqlParameter("@LastUpdationDate", upDt == default ? string.Empty : upDt));
                SqlParams.Add(new SqlParameter("@BalanceSheetClosed", model.BalanceSheetClosed));
                //SqlParams.Add(new SqlParameter("@AttachmentFilePath1", model.AttachmentFilePath1 ?? string.Empty));
                //SqlParams.Add(new SqlParameter("@AttachmentFilePath2", model.AttachmentFilePath2 ?? string.Empty));
                //SqlParams.Add(new SqlParameter("@AttachmentFilePath3", model.AttachmentFilePath3 ?? string.Empty));

                //SqlParams.Add(new SqlParameter("@BooktrnsEntryId", model.SaleBillEntryId));

                SqlParams.Add(new SqlParameter("@Dtdeatil", PRGrid));
                SqlParams.Add(new SqlParameter("@DTTaxGrid", TaxDetailDT));

                SqlParams.Add(new SqlParameter("@DRCRDATA", DrCrDetailDT));
                SqlParams.Add(new SqlParameter("@AgainstRef", AdjDetailDT));
                SqlParams.Add(new SqlParameter("@dtAgaintBillNo", DTAgainstBillDetail));

                _ResponseResult = await _IDataLogic.ExecuteDataTable("AccSPPurchaseRejectionMainDetail", SqlParams);

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
