using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.Extensions.Configuration;
using System.Globalization;
using System.Reflection;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Data.DAL
{
    public class SaleRejectionDAL
    {
        public SaleRejectionDAL(IConfiguration configuration, IDataLogic iDataLogic)
        {
            DBConnectionString = configuration.GetConnectionString("eTactDB");
            _IDataLogic = iDataLogic;
        }

        public IDataLogic? _IDataLogic { get; }

        public IConfiguration? Configuration { get; }

        private string DBConnectionString { get; }

        public async Task<SaleRejectionModel> FillSaleRejectionGrid(string mrnNo, int mrnEntryId, int mrnYC,int yearCode)
        {
            var model = new SaleRejectionModel();
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "DisplayDataAgainstMrnINGrid"));
                SqlParams.Add(new SqlParameter("@mrnno", mrnNo));
                SqlParams.Add(new SqlParameter("@MRNYearCode", mrnYC));
                SqlParams.Add(new SqlParameter("@MRNEntryId", mrnEntryId));
                SqlParams.Add(new SqlParameter("@BalanceSheetClosed", 0));
                SqlParams.Add(new SqlParameter("@SaleRejYearCode", yearCode));
                _ResponseResult = await _IDataLogic.ExecuteDataSet("AccSpSalerejection", SqlParams);
                if (_ResponseResult.Result != null && _ResponseResult.StatusCode == HttpStatusCode.OK && _ResponseResult.StatusText == "Success")
                {
                    PrepareMRNView(_ResponseResult.Result, ref model);
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

        public async Task<ResponseResult> GetDashboardData(string summaryDetail, string custInvoiceNo, string custName, string mrnNo, string gateNo, string partCode, string itemName, string againstBillNo, string docName, string voucherNo, string fromdate, string toDate)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                 fromdate = CommonFunc.ParseFormattedDate(fromdate);
                toDate = CommonFunc.ParseFormattedDate(toDate);
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "DASHBOARDSUMMARY"));
                SqlParams.Add(new SqlParameter("@CustInvoiceNo", custInvoiceNo ?? ""));
                SqlParams.Add(new SqlParameter("@CustomerName", custName ?? ""));
                SqlParams.Add(new SqlParameter("@MrnNo", mrnNo ?? ""));
                SqlParams.Add(new SqlParameter("@GateNo", gateNo ?? ""));
                SqlParams.Add(new SqlParameter("@PartCode", partCode ?? ""));
                SqlParams.Add(new SqlParameter("@itemName", itemName ?? ""));
                SqlParams.Add(new SqlParameter("@AgainstBillNo", againstBillNo ?? ""));
                SqlParams.Add(new SqlParameter("@DocumentName", docName ?? ""));
                SqlParams.Add(new SqlParameter("@VoucherNo", voucherNo ?? ""));
                SqlParams.Add(new SqlParameter("@FromDate", fromdate));
                SqlParams.Add(new SqlParameter("@ToDate", toDate));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("AccSpSalerejection", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> NewEntryId(int YearCode)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "NewEntryId"));
                SqlParams.Add(new SqlParameter("@SaleRejYearCode", YearCode));
                _ResponseResult = await _IDataLogic.ExecuteDataSet("AccSpSalerejection", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }

        internal async Task<ResponseResult> DeleteByID(int ID, int YC,int accountCode,int createdBy, string machineName,string cc)
        {
            var ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@Flag", "delete"));
                SqlParams.Add(new SqlParameter("@SaleRejEntryId", ID));
                SqlParams.Add(new SqlParameter("@accountcode", accountCode));
                SqlParams.Add(new SqlParameter("@SaleRejYearCode", YC));
                SqlParams.Add(new SqlParameter("@EntryByMachineName", machineName));
                SqlParams.Add(new SqlParameter("@ActualEnteredBy", createdBy));
                SqlParams.Add(new SqlParameter("@cc", machineName));

                ResponseResult = await _IDataLogic.ExecuteDataSet("AccSpSalerejection", SqlParams);

            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return ResponseResult;
        }
        internal async Task<SaleRejectionModel> GetViewByID(int ID, int YC, string Mode)
        {
            var model = new SaleRejectionModel();
            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@Flag", "VIEWBYID"));
                SqlParams.Add(new SqlParameter("@SaleRejEntryId", ID));
                SqlParams.Add(new SqlParameter("@SaleRejYearCode", YC));

                var ResponseResult = await _IDataLogic.ExecuteDataSet("AccSpSalerejection", SqlParams);

                if (ResponseResult.Result != null && ResponseResult.StatusCode == HttpStatusCode.OK && ResponseResult.StatusText == "Success")
                {
                    PrepareView(ResponseResult.Result, ref model, Mode);
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

        private static SaleRejectionModel PrepareView(DataSet DS, ref SaleRejectionModel? model, string Mode)
        {
            var ItemGrid = new List<SaleRejectionDetail>();
            var SaleRejectionGrid = new List<SaleRejectionDetail>();
            var TaxGrid = new List<TaxModel>();
            var DRCRGrid = new List<DbCrModel>();
            var adjustGrid = new List<AdjustmentModel>();
            DS.Tables[0].TableName = "saleRejectionModel";
            DS.Tables[1].TableName = "saleRejectionDetail";
            DS.Tables[2].TableName = "saleRejectionTaxDetail";
            DS.Tables[3].TableName = "DRCRDetail";
            DS.Tables[4].TableName = "AdjustmentDetail";
            int cnt = 0;

            model.SaleRejEntryId = Convert.ToInt32(DS.Tables[0].Rows[0]["SaleRejEntryId"]);
            model.SaleRejYearCode = Convert.ToInt32(DS.Tables[0].Rows[0]["SaleRejYearCode"]);
            model.SaleRejEntryDate = DS.Tables[0].Rows[0]["SaleRejEntryDate"]?.ToString();
            model.GateNo = DS.Tables[0].Rows[0]["GateNo"]?.ToString();
            model.Gateyearcode = Convert.ToInt32(DS.Tables[0].Rows[0]["Gateyearcode"]);
            model.GateDate = DS.Tables[0].Rows[0]["GateDate"]?.ToString();
            model.MRNEntryId = Convert.ToInt32(DS.Tables[0].Rows[0]["MRNEntryId"]);
            model.MrnNo = DS.Tables[0].Rows[0]["MrnNo"]?.ToString();
            model.MRNDate = DS.Tables[0].Rows[0]["MRNDate"]?.ToString();
            model.Mrnyearcode = Convert.ToInt32(DS.Tables[0].Rows[0]["Mrnyearcode"]);
            model.SalerejCreditNoteVoucherNo = DS.Tables[0].Rows[0]["SalerejCreditNoteVoucherNo"]?.ToString();
            model.VoucherNo = DS.Tables[0].Rows[0]["VoucherNo"]?.ToString();
            model.CustInvoiceNo = DS.Tables[0].Rows[0]["CustInvoiceNo"]?.ToString();
            model.CustInvoiceDate = DS.Tables[0].Rows[0]["CustInvoiceDate"]?.ToString();
            model.CustInvoiceTime = DS.Tables[0].Rows[0]["CustInvoiceTime"]?.ToString();
            model.AccountCode = Convert.ToInt32(DS.Tables[0].Rows[0]["AccountCode"]);
            model.Account_Name = DS.Tables[0].Rows[0]["CustomerName"]?.ToString();
            model.PaymentTerm = Convert.ToInt32(DS.Tables[0].Rows[0]["PaymentTerm"]);
            model.GSTNO = DS.Tables[0].Rows[0]["GSTNO"]?.ToString();
            model.DomesticExportNEPZ = DS.Tables[0].Rows[0]["DomesticExportNEPZ"]?.ToString();
            model.Transporter = DS.Tables[0].Rows[0]["Transporter"]?.ToString();
            model.Vehicleno = DS.Tables[0].Rows[0]["Vehicleno"]?.ToString();
            model.BillAmt = Convert.ToInt32(DS.Tables[0].Rows[0]["BillAmt"]);
            model.RoundOffAmt = Convert.ToInt32(DS.Tables[0].Rows[0]["RoundOffAmt"]);
            model.RoundoffType = DS.Tables[0].Rows[0]["RoundoffType"]?.ToString();
            model.Taxableamt = Convert.ToInt32(DS.Tables[0].Rows[0]["Taxableamt"]);
            model.ToatlDiscountPercent = Convert.ToInt32(DS.Tables[0].Rows[0]["ToatlDiscountPercent"]);
            model.TotalDiscountAmount = Convert.ToInt32(DS.Tables[0].Rows[0]["TotalDiscountAmount"]);
            model.InvNetAmt = Convert.ToInt32(DS.Tables[0].Rows[0]["InvNetAmt"]);
            model.SalerejRemark = DS.Tables[0].Rows[0]["SalerejRemark"]?.ToString();
            model.CC = DS.Tables[0].Rows[0]["CC"]?.ToString();
            model.Uid = Convert.ToInt32(DS.Tables[0].Rows[0]["Uid"]);
            model.EntryByempId = Convert.ToInt32(DS.Tables[0].Rows[0]["EntryByempId"]);
            model.MachineName = DS.Tables[0].Rows[0]["MachineName"]?.ToString();
            model.ActualEntryDate = DS.Tables[0].Rows[0]["ActualEntryDate"]?.ToString();
            model.ActualEnteredBy = Convert.ToInt32(DS.Tables[0].Rows[0]["ActualEnteredBy"]);
            model.ActualEnteredByName = DS.Tables[0].Rows[0]["ActualEntryByName"]?.ToString();
            model.BalanceSheetClosed = Convert.ToChar(DS.Tables[0].Rows[0]["BalanceSheetClosed"]);

            if (Mode == "U" || Mode == "V")
            {
                if (DS.Tables[0].Rows[0]["UpdatedByName"].ToString() != "")
                {
                    model.LastUpdatedBy = Convert.ToInt32(DS.Tables[0].Rows[0]["LastUpdatedBy"]);
                    model.LastUpdatedByName = DS.Tables[0].Rows[0]["UpdatedByName"]?.ToString();
                    model.LastUpdationDate = DS.Tables[0].Rows[0]["LastUpdationDate"]?.ToString();
                }
            }

            if (DS.Tables.Count != 0 && DS.Tables[1].Rows.Count > 0)
            {
                foreach (DataRow row in DS.Tables[1].Rows)
                {
                    SaleRejectionGrid.Add(new SaleRejectionDetail
                    {
                        AgainstBillTypeJWSALE = row["AgainstBillTypeJWSALE"]?.ToString(),
                        AgainstBillNo = row["AgainstBillNo"]?.ToString(),
                        AgainstBillYearCode = row["AgainstBillYearCode"] != DBNull.Value ? Convert.ToInt32(row["AgainstBillYearCode"]) : 0,
                        AgainstBillEntryId = row["AgainstOpenBillEntryId"] != DBNull.Value ? Convert.ToInt32(row["AgainstOpenBillEntryId"]) : 0,
                        AgainstOpnOrBill = row["AgainstOpnOrBill"]?.ToString(),
                        DocTypeAccountCode = row["DocTypeAccountCode"] != DBNull.Value ? Convert.ToInt32(row["DocTypeAccountCode"]) : 0,
                        ItemCode = row["ItemCode"] != DBNull.Value ? Convert.ToInt32(row["ItemCode"]) : 0,
                        ItemName = row["ItemName"]?.ToString(),
                        PartCode = row["PartCode"]?.ToString(),
                        Unit = row["Unit"]?.ToString(),
                        HSNNo = row["HSNNo"] != DBNull.Value ? Convert.ToInt32(row["HSNNo"]) : 0,
                        NoOfCase = row["NoOfCase"] != DBNull.Value ? Convert.ToInt32(row["NoOfCase"]) : 0,
                        SaleBillQty = row["SaleBillQty"] != DBNull.Value ? Convert.ToInt32(row["SaleBillQty"]) : 0,
                        RejQty = row["RejQty"] != DBNull.Value ? Convert.ToInt32(row["RejQty"]) : 0,
                        RecQty = row["MRNRecQty"] != DBNull.Value ? Convert.ToInt32(row["MRNRecQty"]) : 0,
                        Rate = row["RejRate"] != DBNull.Value ? Convert.ToInt32(row["RejRate"]) : 0,
                        DiscountPer = row["DiscountPer"] != DBNull.Value ? Convert.ToInt32(row["DiscountPer"]) : 0,
                        DiscountAmt = row["DiscountAmt"] != DBNull.Value ? Convert.ToInt32(row["DiscountAmt"]) : 0,
                        SONO = row["SONO"]?.ToString(),
                        SOyearcode = row["SOyearcode"] != DBNull.Value ? Convert.ToInt32(row["SOyearcode"]) : 0,
                        SODate = row["SODate"]?.ToString(),
                        CustOrderNo = row["CustOrderNo"]?.ToString(),
                        SOAmmNo = row["SOAmmNo"]?.ToString(),
                        Itemsize = row["Itemsize"]?.ToString(),
                        RecStoreId = row["RecStoreId"] != DBNull.Value ? Convert.ToInt32(row["RecStoreId"]) : 0,
                        //RecStoreName = row["SONO"] != DBNull.Value ? Convert.ToInt32(row["SONO"]) : 0,
                        OtherDetail = row["OtherDetail"]?.ToString(),
                        Amount = row["ItemAmount"] != DBNull.Value ? Convert.ToInt32(row["ItemAmount"]) : 0,
                        RejectionReason = row["RejectionReason"]?.ToString(),
                        SaleorderRemark = row["SaleorderRemark"]?.ToString(),
                        SaleBillremark = row["SaleBillremark"]?.ToString(),
                        ItemNetAmount = row["ItemAmount"] != DBNull.Value ? Convert.ToInt32(row["ItemAmount"]) : 0
                    });
                }
                model.SaleRejectionDetails = SaleRejectionGrid;
                model.ItemDetailGrid = SaleRejectionGrid;
            }

            if (DS.Tables.Count != 0 && DS.Tables[2].Rows.Count > 0)
            {
                foreach (DataRow row in DS.Tables[2].Rows)
                {
                    TaxGrid.Add(new TaxModel
                    {
                        TxType = row["Type"]?.ToString(),
                        TxItemCode = row["ItemCode"] != DBNull.Value ? Convert.ToInt32(row["ItemCode"]) : 0,
                        TxTaxTypeName = row["TaxTypeID"]?.ToString(),
                        TxAccountCode = row["TaxAccountCode"] != DBNull.Value ? Convert.ToInt32(row["TaxAccountCode"]) : 0,
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

            //if (DS.Tables.Count != 0 && DS.Tables[3].Rows.Count > 0)
            //{
            //    foreach (DataRow row in DS.Tables[3].Rows)
            //    {
            //        DRCRGrid.Add(new DbCrModel
            //        {
            //            AccEntryId = row["AccEntryId"] != DBNull.Value ? Convert.ToInt32(row["AccEntryId"]) : 0,
            //            AccYearCode = row["AccYearCode"] != DBNull.Value ? Convert.ToInt32(row["AccYearCode"]) : 0,
            //            //SeqNo = row["Type"] != DBNull.Value ? Convert.ToInt32(row["Type"]) : 0,
            //            //InvoiceNo = row["Type"]?.ToString(),
            //            VoucherNo = row["BillVouchNo"]?.ToString(),
            //            //AgainstInvNo = row["Type"]?.ToString(),
            //            //AginstVoucherYearcode = row["Type"] != DBNull.Value ? Convert.ToInt32(row["Type"]) : 0,
            //            AccountCode = row["Accountcode"] != DBNull.Value ? Convert.ToInt32(row["Accountcode"]) : 0,
            //            //DocTypeID = row["Type"] != DBNull.Value ? Convert.ToInt32(row["Type"]) : 0,
            //            //BillQty = row["Type"] != DBNull.Value ? Convert.ToInt32(row["Type"]) : 0,
            //            //Rate = row["Type"] != DBNull.Value ? Convert.ToInt32(row["Type"]) : 0,
            //            //AccountAmount = row["Type"] != DBNull.Value ? Convert.ToInt32(row["Type"]) : 0,
            //            //DRCR = row["Type"]?.ToString(),
            //            AccountName = row["Account_Name"]?.ToString(),
            //            //DrAmt = row["Type"] != DBNull.Value ? Convert.ToSingle(row["Type"]) : 0,
            //            CrAmt = row["CrAmt"] != DBNull.Value ? Convert.ToSingle(row["CrAmt"]) : 0
            //        });
            //    }
            //    model.DRCRGrid = DRCRGrid;
            //}

            if (DS.Tables.Count != 0 && DS.Tables[4].Rows.Count > 0)
            {
                foreach (DataRow row in DS.Tables[4].Rows)
                {
                    adjustGrid.Add(new AdjustmentModel
                    {
                        //AdjSeqNo = row["AccEntryId"] != DBNull.Value ? Convert.ToInt32(row["AccEntryId"]) : 0,
                        AdjModeOfAdjstment = row["ModOfAdjust"]?.ToString(),
                        //AdjModeOfAdjstmentName = row["AccEntryId"]?.ToString(),
                        //AdjNewRefNo = row["AccEntryId"]?.ToString(),
                        //AdjDescription = row["AccEntryId"]?.ToString(),
                        //AdjDrCrName = row["AccEntryId"]?.ToString(),
                        AdjDrCr = row["DR/CR"]?.ToString(),
                        //AdjPendAmt = row["AccEntryId"] != DBNull.Value ? Convert.ToInt32(row["AccEntryId"]) : 0,
                        AdjAdjstedAmt = row["AdjustmentAmt"] != DBNull.Value ? Convert.ToInt32(row["AdjustmentAmt"]) : 0,
                        AdjTotalAmt = row["BillAmt"] != DBNull.Value ? Convert.ToInt32(row["BillAmt"]) : 0, // BillAmt
                        //AdjRemainingAmt = row["AccEntryId"] != DBNull.Value ? Convert.ToInt32(row["AccEntryId"]) : 0,
                        //AdjOpenEntryID = row["AgainstAccEntryId"] != DBNull.Value ? Convert.ToInt32(row["AgainstAccEntryId"]) : 0,
                        //AdjOpeningYearCode = row["AccEntryId"] != DBNull.Value ? Convert.ToInt32(row["AccEntryId"]) : 0,
                        //AdjDueDate = row["AccEntryId"] != DBNull.Value ? Convert.ToDateTime(row["AccEntryId"]) : new DateTime(),
                        //AdjPurchOrderNo = row["AccEntryId"]?.ToString(),
                        //AdjPOYear = row["AccEntryId"] != DBNull.Value ? Convert.ToInt32(row["AccEntryId"]) : 0,
                        //AdjPODate = row["AccEntryId"] != DBNull.Value ? Convert.ToDateTime(row["AccEntryId"]) : new DateTime(),
                        //AdjPageName = row["AccEntryId"]?.ToString(),
                        //AdjAgnstAccEntryID = row["AccEntryId"] != DBNull.Value ? Convert.ToInt32(row["AccEntryId"]) : 0,
                        AdjAgnstAccYearCode = row["yearcode"] != DBNull.Value ? Convert.ToInt32(row["yearcode"]) : 0,
                        //AdjAgnstModeOfAdjstment = row["AccEntryId"]?.ToString(),
                        //AdjAgnstModeOfAdjstmentName = row["AccEntryId"]?.ToString(),
                        //AdjAgnstNewRefNo = row["AccEntryId"]?.ToString(),
                        AdjAgnstVouchNo = row["AgainstVoucherNo"]?.ToString(),
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
                        //AdjAgnstTransType = row["AccEntryId"]?.ToString()
                    });
                }

                if (model.adjustmentModel == null)
                {
                    model.adjustmentModel = new AdjustmentModel();
                }

                model.adjustmentModel.AdjAdjustmentDetailGrid = adjustGrid;
            }

            return model;
        }
        public static DateTime ParseDate(string dateString)
        {
            if (string.IsNullOrEmpty(dateString))
            {
                return default;
            }

            if (DateTime.TryParseExact(dateString, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate))
            {
                return parsedDate;
            }
            else
            {
                return DateTime.Parse(dateString);
            }
        }
        internal async Task<ResponseResult> SaveSaleRejection(SaleRejectionModel model, DataTable SBGrid, DataTable TaxDetailDT, DataTable DrCrDetailDT, DataTable AdjDetailDT)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var upDt = CommonFunc.ParseFormattedDate(DateTime.Now.ToString("dd/MM/yyyy"));
                var SqlParams = new List<dynamic>();
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

                //DateTime entDt = new DateTime();
                //DateTime GateDate = new DateTime();
                //DateTime MrnDt = new DateTime();
                //DateTime custInvDate = new DateTime();
                //DateTime custInvTime = new DateTime();
                //DateTime actualEntryDt = new DateTime();
                //DateTime lastUpdateDt = new DateTime();
                //DateTime RemovalTime = new DateTime();

                var entDt = CommonFunc.ParseFormattedDate(model.SaleRejEntryDate);
                var GateDate = CommonFunc.ParseFormattedDate(model.GateDate);
                var MrnDt = CommonFunc.ParseFormattedDate(model.MRNDate);
                var custInvDate = CommonFunc.ParseFormattedDate(model.CustInvoiceDate);
                var custInvTime = CommonFunc.ParseFormattedDate(model.CustInvoiceTime);
                var actualEntryDt = CommonFunc.ParseFormattedDate(model.ActualEntryDate);
                var lastUpdateDt = CommonFunc.ParseFormattedDate(model.LastUpdationDate);

                //RemovalTime = ParseDate(model.RemovalDate);


                SqlParams.Add(new SqlParameter("@SaleRejEntryId", model.SaleRejEntryId));
                SqlParams.Add(new SqlParameter("@SaleRejYearCode", model.SaleRejYearCode));
                SqlParams.Add(new SqlParameter("@SaleRejEntryDate", entDt == default ? string.Empty : entDt));
                SqlParams.Add(new SqlParameter("@GateEntryId",  string.Empty));
                SqlParams.Add(new SqlParameter("@GateNo", model.GateNo ?? string.Empty));
                SqlParams.Add(new SqlParameter("@Gateyearcode", model.Gateyearcode));
                SqlParams.Add(new SqlParameter("@GateDate", GateDate == default ? string.Empty : GateDate));
                SqlParams.Add(new SqlParameter("@MRNEntryId", model.MRNEntryId));
                SqlParams.Add(new SqlParameter("@MrnNo", model.MrnNo ?? string.Empty));
                SqlParams.Add(new SqlParameter("@MRNDate", MrnDt == default ? string.Empty : MrnDt));
                SqlParams.Add(new SqlParameter("@Mrnyearcode", model.Mrnyearcode));

                SqlParams.Add(new SqlParameter("@SalerejCreditNoteVoucherNo", model.SalerejCreditNoteVoucherNo ?? string.Empty));
                SqlParams.Add(new SqlParameter("@VoucherNo", model.VoucherNo ?? string.Empty));
                //SqlParams.Add(new SqlParameter("@RemovalTime", RemovalTime == default ? string.Empty : RemovalTime));
                SqlParams.Add(new SqlParameter("@CustInvoiceNo", model.CustInvoiceNo));
                SqlParams.Add(new SqlParameter("@CustInvoiceDate", custInvDate == default ? string.Empty : custInvDate));
                SqlParams.Add(new SqlParameter("@CustInvoiceTime", custInvTime == default ? string.Empty : custInvTime));
                SqlParams.Add(new SqlParameter("@AccountCode", model.AccountCode));
                SqlParams.Add(new SqlParameter("@PaymentTerm", model.PaymentTerm));
                SqlParams.Add(new SqlParameter("@GSTNO", model.GSTNO ?? string.Empty));
                SqlParams.Add(new SqlParameter("@DomesticExportNEPZ", model.DomesticExportNEPZ ?? string.Empty));
                SqlParams.Add(new SqlParameter("@Transporter", model.Transporter ?? string.Empty));
                SqlParams.Add(new SqlParameter("@Vehicleno", model.Vehicleno ?? string.Empty));
                SqlParams.Add(new SqlParameter("@BillAmt", model.BillAmt));
                SqlParams.Add(new SqlParameter("@RoundOffAmt", model.RoundOffAmt));
                SqlParams.Add(new SqlParameter("@RoundoffType", model.RoundoffType ?? string.Empty));
                SqlParams.Add(new SqlParameter("@Taxableamt", model.Taxableamt));
                SqlParams.Add(new SqlParameter("@ToatlDiscountPercent", model.TotalDiscountPercentage));
                SqlParams.Add(new SqlParameter("@TotalDiscountAmount", model.TotalDiscountAmount));
                SqlParams.Add(new SqlParameter("@InvNetAmt", model.InvNetAmt));
                SqlParams.Add(new SqlParameter("@SalerejRemark", model.SalerejRemark ?? string.Empty));
                SqlParams.Add(new SqlParameter("@CC", model.CC ?? string.Empty));
                SqlParams.Add(new SqlParameter("@Uid", model.Uid));
                SqlParams.Add(new SqlParameter("@EntryByempId", model.ActualEnteredBy));
                SqlParams.Add(new SqlParameter("@EntryByMachineName", model.MachineName));
                SqlParams.Add(new SqlParameter("@ActualEntryDate", actualEntryDt == default ? string.Empty : actualEntryDt));
                SqlParams.Add(new SqlParameter("@ActualEnteredBy", model.ActualEnteredBy));
                SqlParams.Add(new SqlParameter("@LastUpdatedBy", model.LastUpdatedBy));
                SqlParams.Add(new SqlParameter("@LastUpdationDate", lastUpdateDt == default ? string.Empty : lastUpdateDt));
                SqlParams.Add(new SqlParameter("@BalanceSheetClosed", model.BalanceSheetClosed ?? 0));
                SqlParams.Add(new SqlParameter("@SubVoucherName", model.VoucherNo ?? string.Empty));
                SqlParams.Add(new SqlParameter("@Currencyid", 0));
                SqlParams.Add(new SqlParameter("@ExchangeRate", 0));

                SqlParams.Add(new SqlParameter("@DTGrid", SBGrid));
                SqlParams.Add(new SqlParameter("@DTTaxGrid", TaxDetailDT));

                SqlParams.Add(new SqlParameter("@DRCRDATA", DrCrDetailDT));
                SqlParams.Add(new SqlParameter("@AgainstRef", AdjDetailDT));

                _ResponseResult = await _IDataLogic.ExecuteDataTable("AccSpSalerejection", SqlParams);

            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }

        private static SaleRejectionModel PrepareMRNView(DataSet DS, ref SaleRejectionModel? model)
        {
            var ItemGrid = new List<SaleRejectionDetail>();
            var SaleRejectionGrid = new List<SaleRejectionDetail>();

            DS.Tables[0].TableName = "saleRejectionModel";
            DS.Tables[1].TableName = "saleRejectionDetail";
            int cnt = 0;

             model.GateNo = DS.Tables[0].Rows[0]["GateNo"]?.ToString();
            model.Gateyearcode = Convert.ToInt32(DS.Tables[0].Rows[0]["GateYearCode"]);
            model.GateDate = DS.Tables[0].Rows[0]["GateDate"]?.ToString();
            //model.MRNEntryId = Convert.ToInt32(DS.Tables[0].Rows[0]["MRNEntryId"]);
            model.MrnNo = DS.Tables[0].Rows[0]["MRNNo"]?.ToString();
            model.MRNDate = DS.Tables[0].Rows[0]["MRNEntryDate"]?.ToString();
            model.Mrnyearcode = Convert.ToInt32(DS.Tables[0].Rows[0]["MRNYearCode"]);
            model.CustInvoiceNo = DS.Tables[0].Rows[0]["CustInvoiceNo"]?.ToString();
            model.CustInvoiceDate = DS.Tables[0].Rows[0]["CustInvoiceDate"]?.ToString();
            //model.CustInvoiceTime = DS.Tables[0].Rows[0]["CustInvoiceTime"]?.ToString();
            model.AccountCode = Convert.ToInt32(DS.Tables[0].Rows[0]["Account_Code"]);
            model.Account_Name = DS.Tables[0].Rows[0]["Account_Name"]?.ToString();
            model.PaymentTerm = Convert.ToInt32(DS.Tables[0].Rows[0]["PaymentTerms"]);
            model.GSTNO = DS.Tables[0].Rows[0]["GSTNO"]?.ToString();
            model.BillAmt = Convert.ToInt32(DS.Tables[0].Rows[0]["TotalAmt"]);
            model.InvNetAmt = Convert.ToInt32(DS.Tables[0].Rows[0]["NetAmt"]);
            model.ActualEntryDate = DS.Tables[0].Rows[0]["ActualEntryDate"]?.ToString();
            //model.BalanceSheetClosed = DS.Tables[0].Rows[0]["SaleBillEntryDate"]?.ToString();

            if (DS.Tables.Count != 0 && DS.Tables[1].Rows.Count > 0)
            {
                foreach (DataRow row in DS.Tables[1].Rows)
                {
                    SaleRejectionGrid.Add(new SaleRejectionDetail
                    {
                        AgainstBillNo = row["AgainstBillNo"]?.ToString(),
                        AgainstBillYearCode = row["AgainstBillYearCode"] != DBNull.Value ? Convert.ToInt32(row["AgainstBillYearCode"]) : 0,
                        AgainstBillEntryId = row["AgainstBillENtryId"] != DBNull.Value ? Convert.ToInt32(row["AgainstBillENtryId"]) : 0,
                        AgainstOpnOrBill = row["AgainstOpnOrBill"]?.ToString(),
                        ItemCode = row["ItemCode"] != DBNull.Value ? Convert.ToInt32(row["ItemCode"]) : 0,
                        PartCode = row["PartCode"]?.ToString(),
                        ItemName = row["ItemName"]?.ToString(),
                        RecQty = row["RecQty"] != DBNull.Value ? Convert.ToInt32(row["RecQty"]) : 0,
                        Unit   = row["Unit"]?.ToString(),
                        AltQty = row["altQty"] != DBNull.Value ? Convert.ToInt32(row["altQty"]) : 0,
                        SaleBillQty = row["SaleBillQty"] != DBNull.Value ? Convert.ToInt32(row["SaleBillQty"]) : 0,
                        RejQty = row["RejQty"] != DBNull.Value ? Convert.ToInt32(row["RejQty"]) : 0,
                        Rate = row["Rate"] != DBNull.Value ? Convert.ToInt32(row["Rate"]) : 0,
                        DiscountPer = row["DiscountPer"] != DBNull.Value ? Convert.ToInt32(row["DiscountPer"]) : 0,
                        DiscountAmt = row["DiscountAmt"] != DBNull.Value ? Convert.ToInt32(row["DiscountAmt"]) : 0,
                        SONO = row["SONO"]?.ToString(),
                        SODate = row["SODate"] != DBNull.Value ? row["SODate"]?.ToString() : string.Empty,
                        SOyearcode = row["SOYearCode"] != DBNull.Value ? Convert.ToInt32(row["SOYearCode"]) : 0,
                        CustOrderNo = row["CustOrderNo"]?.ToString(),
                        RecStoreId = row["RecStoreid"] != DBNull.Value ? Convert.ToInt32(row["RecStoreid"]) : 0,
                        SaleorderRemark = row["SaleorderRemark"]?.ToString(),
                        SaleBillremark = row["SaleBillremark"]?.ToString(),
                        HSNNo = row["HsnNo"] != DBNull.Value ? Convert.ToInt32(row["HsnNo"]) : 0,
                        Itemsize = row["ItemSize"]?.ToString(),
                        NoOfCase = row["NoOfCase"] != DBNull.Value ?  Convert.ToInt32( row["NoOfCase"]) : 0,
                        RecStoreName = row["ReceiveInStore"]?.ToString(),
                        Amount = row["ItemAmount"] != DBNull.Value ? Convert.ToDecimal(row["ItemAmount"]) : 0
                    });
                }
                //model.SaleRejectionDetails = SaleRejectionGrid;
                model.SaleRejectionInputGrid = SaleRejectionGrid;
                model.ItemDetailGrid = SaleRejectionGrid;
            }

            return model;
        }

        public async Task<ResponseResult> FillCustomerName(string fromDate, string toDate)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FILLCUSTOMERINDASHBOARD"));
                SqlParams.Add(new SqlParameter("@fromDate", fromDate));
                SqlParams.Add(new SqlParameter("@toDate", toDate));
                _ResponseResult = await _IDataLogic.ExecuteDataSet("AccSpSalerejection", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> FillItemName(string fromDate, string toDate)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FILLItemNameINDASHBOARD"));
                SqlParams.Add(new SqlParameter("@fromDate", fromDate));
                SqlParams.Add(new SqlParameter("@toDate", toDate));
                _ResponseResult = await _IDataLogic.ExecuteDataSet("AccSpSalerejection", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> FillPartCode(string fromDate, string toDate)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FILLPartCodeInDASHBOARD"));
                SqlParams.Add(new SqlParameter("@fromDate", fromDate));
                SqlParams.Add(new SqlParameter("@toDate", toDate));
                _ResponseResult = await _IDataLogic.ExecuteDataSet("AccSpSalerejection", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> FillInvoiceNo(string fromDate, string toDate)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FILLInvoiceNoInDASHBOARD"));
                SqlParams.Add(new SqlParameter("@fromDate", fromDate));
                SqlParams.Add(new SqlParameter("@toDate", toDate));
                _ResponseResult = await _IDataLogic.ExecuteDataSet("AccSpSalerejection", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> FillVoucherNo(string fromDate, string toDate)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FILLVoucherNoINDASHBOARD"));
                SqlParams.Add(new SqlParameter("@fromDate", fromDate));
                SqlParams.Add(new SqlParameter("@toDate", toDate));
                _ResponseResult = await _IDataLogic.ExecuteDataSet("AccSpSalerejection", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> FillDocument(string fromDate, string toDate)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FILLDocumentINDASHBOARD"));
                SqlParams.Add(new SqlParameter("@fromDate", fromDate));
                SqlParams.Add(new SqlParameter("@toDate", toDate));
                _ResponseResult = await _IDataLogic.ExecuteDataSet("AccSpSalerejection", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> FillAgainstSaleBillNo(string fromDate, string toDate)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FILLAgainstSaleBillNoINDASHBOARD"));
                SqlParams.Add(new SqlParameter("@fromDate", fromDate));
                SqlParams.Add(new SqlParameter("@toDate", toDate));
                _ResponseResult = await _IDataLogic.ExecuteDataSet("AccSpSalerejection", SqlParams);
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
