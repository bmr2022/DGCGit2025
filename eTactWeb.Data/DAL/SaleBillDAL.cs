using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Globalization;
using static eTactWeb.DOM.Models.Common;
using Common = eTactWeb.Data.Common;

namespace eTactWeb.Data.DAL
{
    public class SaleBillDAL
    {
        public SaleBillDAL(IConfiguration configuration, IDataLogic iDataLogic)
        {
            DBConnectionString = configuration.GetConnectionString("eTactDB");
            _IDataLogic = iDataLogic;
        }
        public IDataLogic? _IDataLogic { get; }

        public IConfiguration? Configuration { get; }

        private string DBConnectionString { get; }

        public async Task<ResponseResult> GetReportName()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "GetReportName"));

                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_SaleBillMainDetail", SqlParams);

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
                SqlParams.Add(new SqlParameter("@YearCode", YearCode));
                _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_SaleBillMainDetail", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> GetBatchInventory()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "GetBatchInventory"));
                _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_SaleBillMainDetail", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> GetCustomerBasedDetails(int Code)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "GetAddress"));
                SqlParams.Add(new SqlParameter("@accountcode", Code));
                _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_SaleBillMainDetail", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> FillCurrency(int accountCode)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "Currency"));
                SqlParams.Add(new SqlParameter("@accountcode", accountCode));
                _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_SaleBillMainDetail", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }

        public async Task<ResponseResult> GetAutocompleteValue()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "GetAutocompleteValue"));
                _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_SaleBillMainDetail", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }

        internal async Task<ResponseResult> SaveSaleBill(SaleBillModel model, DataTable SBGrid, DataTable TaxDetailDT, DataTable DrCrDetailDT, DataTable AdjDetailDT,DataTable AdjChallanDetailDT)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                if (model.Mode == "V" || model.Mode == "U")
                {
                    SqlParams.Add(new SqlParameter("@Flag", "UPDATE"));
                    SqlParams.Add(new SqlParameter("@LastUpdatedBy", model.LastUpdatedBy));
                    SqlParams.Add(new SqlParameter("@LastUpdationDate", DateTime.Today));
                }
                else
                {
                    SqlParams.Add(new SqlParameter("@Flag", "INSERT"));
                }

                var entDt = Common.CommonFunc.ParseFormattedDate(model.SaleBillEntryDate);
                var SaleBillDate = Common.CommonFunc.ParseFormattedDate(model.SaleBillDate);
                var InvoiceTime = Common.CommonFunc.ParseFormattedDate(model.InvoiceTime);
                var performaInvDate = Common.CommonFunc.ParseFormattedDate(model.PerformaInvDate);
                var RemovalDate = Common.CommonFunc.ParseFormattedDate(model.RemovalDate);
                var ApprovalDate = Common.CommonFunc.ParseFormattedDate(model.ApprovDate);
                var Shippingdate = Common.CommonFunc.ParseFormattedDate(model.Shippingdate);
                var Canceldate = Common.CommonFunc.ParseFormattedDate(model.Canceldate);
                var ActualEntryDate = Common.CommonFunc.ParseFormattedDate(model.ActualEntryDate);
                var LastUpdationDate = Common.CommonFunc.ParseFormattedDate(model.LastUpdationDate);
                var ChallanDate = Common.CommonFunc.ParseFormattedDate(model.ChallanDate);
                var SaleQuotDate = Common.CommonFunc.ParseFormattedDate(model.SaleQuotDate);
                var RemovalTime = Common.CommonFunc.ParseFormattedDate(model.RemovalTime);

                SqlParams.Add(new SqlParameter("@EntryId", model.SaleBillEntryId));
                SqlParams.Add(new SqlParameter("@Yearcode", model.SaleBillYearCode));
                SqlParams.Add(new SqlParameter("@SaleBillEntryDate", entDt == default ? string.Empty : entDt));
                SqlParams.Add(new SqlParameter("@salebillno", model.SaleBillNo ?? string.Empty));
                SqlParams.Add(new SqlParameter("@InvNo", model.SaleBillNo ?? string.Empty));
                SqlParams.Add(new SqlParameter("@InvoiceDate", SaleBillDate == default ? string.Empty : SaleBillDate));
                SqlParams.Add(new SqlParameter("@InvoiceTime", model.InvoiceTime ?? string.Empty));
                //SqlParams.Add(new SqlParameter("@SaleBillJobwork", model.SaleBillJobwork ?? string.Empty));
                SqlParams.Add(new SqlParameter("@SaleBillJobwork", "S"));
                SqlParams.Add(new SqlParameter("@ExportInvoiceNo", model.ExportInvoiceNo ?? string.Empty));
                SqlParams.Add(new SqlParameter("@PerformaInvNo", model.PerformaInvNo ?? string.Empty));
                SqlParams.Add(new SqlParameter("@PerformaInvDate", performaInvDate == default ? string.Empty : performaInvDate));
                SqlParams.Add(new SqlParameter("@PerformaInvYearCode", model.PerformaInvYearCode));
                SqlParams.Add(new SqlParameter("@BILLAgainstWarrenty", model.BILLAgainstWarrenty ?? string.Empty));
                SqlParams.Add(new SqlParameter("@RemovalDate", RemovalDate == default ? string.Empty : RemovalDate));
                SqlParams.Add(new SqlParameter("@RemovalTime", RemovalTime == default ? string.Empty : RemovalTime));
                SqlParams.Add(new SqlParameter("@AccountCode", model.AccountCode));
                SqlParams.Add(new SqlParameter("@GSTNO", model.GSTNO ?? string.Empty));
                SqlParams.Add(new SqlParameter("@DomesticExportNEPZ", model.DomesticExportNEPZ ?? string.Empty));
                SqlParams.Add(new SqlParameter("@SupplyType", model.SupplyType ?? string.Empty));
                SqlParams.Add(new SqlParameter("@CustAddress", model.CustAddress ?? string.Empty));
                SqlParams.Add(new SqlParameter("@StateNameofSupply", model.StateNameofSupply ?? string.Empty));
                SqlParams.Add(new SqlParameter("@StateCode", model.StateCode ?? string.Empty));
                SqlParams.Add(new SqlParameter("@CityofSupply", model.CityofSupply ?? string.Empty));
                SqlParams.Add(new SqlParameter("@CountryOfSupply", model.CountryOfSupply ?? string.Empty));
                SqlParams.Add(new SqlParameter("@DistanceKM", model.DistanceKM));
                SqlParams.Add(new SqlParameter("@vehicleNo", model.vehicleNo ?? string.Empty));
                SqlParams.Add(new SqlParameter("@TransporterName", model.TransporterName ?? string.Empty));
                SqlParams.Add(new SqlParameter("@TransporterdocNo", model.TransporterdocNo ?? string.Empty));
                SqlParams.Add(new SqlParameter("@TransporterId", model.TransporterId));
                SqlParams.Add(new SqlParameter("@TransportModeBYRoadAIR", model.TransportModeBYRoadAIR ?? string.Empty));
                SqlParams.Add(new SqlParameter("@ConsigneeAccountcode", model.ConsigneeAccountcode));
                SqlParams.Add(new SqlParameter("@ConsigneeAddress", model.ConsigneeAddress ?? string.Empty));
                SqlParams.Add(new SqlParameter("@DispatchTo", model.DispatchTo ?? string.Empty));
                SqlParams.Add(new SqlParameter("@DispatchThrough", model.DispatchThrough ?? string.Empty));
                SqlParams.Add(new SqlParameter("@DocTypeAccountCode", model.DocTypeAccountCode));
                SqlParams.Add(new SqlParameter("@PaymentTerm", model.PaymentTerm));
                SqlParams.Add(new SqlParameter("@BillAmt", model.BillAmt));
                SqlParams.Add(new SqlParameter("@BillAmtWord", model.BillAmtWord ?? string.Empty));
                SqlParams.Add(new SqlParameter("@TaxableAmt", model.TaxableAmt));
                SqlParams.Add(new SqlParameter("@TaxbaleAmtInWord", model.TaxbaleAmtInWord ?? string.Empty));
                SqlParams.Add(new SqlParameter("@GSTAmount", model.GSTAmount));
                SqlParams.Add(new SqlParameter("@RoundType", model.RoundTypea ?? string.Empty));
                SqlParams.Add(new SqlParameter("@RoundOffAmt", model.RoundOffAmt));
                SqlParams.Add(new SqlParameter("@DiscountPercent", model.DiscountPercent));
                SqlParams.Add(new SqlParameter("@DiscountAmt", model.DiscountAmt));
                SqlParams.Add(new SqlParameter("@NetAmtInWords", model.NetAmtInWords ?? string.Empty));
                SqlParams.Add(new SqlParameter("@INVNetAmt", model.NetTotal));
                SqlParams.Add(new SqlParameter("@Remark", model.Remark ?? string.Empty));
                SqlParams.Add(new SqlParameter("@PermitNo", model.PermitNo ?? string.Empty));
                SqlParams.Add(new SqlParameter("@CashDisPer", model.CashDisPer));
                SqlParams.Add(new SqlParameter("@CashDisRs", model.CashDisRs));
                SqlParams.Add(new SqlParameter("@SoDelTime", model.SoDelTime ?? string.Empty));
                SqlParams.Add(new SqlParameter("@TypeJob", model.TypeJob ?? string.Empty));
                SqlParams.Add(new SqlParameter("@ApprovedBy", model.ApprovedBy));
                SqlParams.Add(new SqlParameter("@currencyId", model.currencyId));
                SqlParams.Add(new SqlParameter("@ExchangeRate", model.ExchangeRate));
                SqlParams.Add(new SqlParameter("@TypeItemServAssets", model.TypeItemServAssets ?? string.Empty));
                //SqlParams.Add(new SqlParameter("@CostCenterId", model.CostCenter));
                SqlParams.Add(new SqlParameter("@Shippingdate", Shippingdate == default ? string.Empty : Shippingdate));
                SqlParams.Add(new SqlParameter("@BankName", model.BankName ?? string.Empty));
                SqlParams.Add(new SqlParameter("@Ewaybillno", model.Ewaybillno ?? string.Empty));
                SqlParams.Add(new SqlParameter("@FreightPaid", model.FreightPaid ?? string.Empty));
                SqlParams.Add(new SqlParameter("@dispatchLocation", model.dispatchLocation ?? string.Empty));
                //SqlParams.Add(new SqlParameter("@currExchangeRate", model.currExchangeRate));
                SqlParams.Add(new SqlParameter("@DispatchDelayReason", model.DispatchDelayReason ?? string.Empty));
                SqlParams.Add(new SqlParameter("@AttachmentFilePath1", model.AttachmentFilePath1 ?? string.Empty));
                SqlParams.Add(new SqlParameter("@AttachmentFilePath2", model.AttachmentFilePath2 ?? string.Empty));
                SqlParams.Add(new SqlParameter("@AttachmentFilePath3", model.AttachmentFilePath3 ?? string.Empty));
                SqlParams.Add(new SqlParameter("@DocketNo", model.DocketNo ?? string.Empty));
                SqlParams.Add(new SqlParameter("@DispatchDelayreson", model.DispatchDelayreson ?? string.Empty));
                SqlParams.Add(new SqlParameter("@Commodity", model.Commodity ?? string.Empty));
                SqlParams.Add(new SqlParameter("@EInvNo", model.EInvNo ?? string.Empty));
                SqlParams.Add(new SqlParameter("@EinvGenerated", model.EinvGenerated ?? string.Empty));
                SqlParams.Add(new SqlParameter("@CC", model.CC ?? string.Empty));
                SqlParams.Add(new SqlParameter("@Uid", model.Uid));
                SqlParams.Add(new SqlParameter("@EntryByempId", model.EntryByempId));
                SqlParams.Add(new SqlParameter("@MachineName", model.MachineName ?? string.Empty));
                SqlParams.Add(new SqlParameter("@ActualEntryDate", ActualEntryDate == default ? string.Empty : ActualEntryDate));
                SqlParams.Add(new SqlParameter("@ActualEnteredBy", model.ActualEnteredBy));
                SqlParams.Add(new SqlParameter("@PaymentCreditDay", model.PaymentCreditDay));
                SqlParams.Add(new SqlParameter("@ChallanNo", model.ChallanNo ?? string.Empty));
                SqlParams.Add(new SqlParameter("@ChallanDate", ChallanDate == default ? string.Empty : ChallanDate));
                SqlParams.Add(new SqlParameter("@ChallanYearCode", model.ChallanYearCode));
                SqlParams.Add(new SqlParameter("@ChallanEntryid", model.ChallanEntryid));
                SqlParams.Add(new SqlParameter("@SaleQuotNo", model.SaleQuotNo ?? string.Empty));
                SqlParams.Add(new SqlParameter("@SaleQuotEntryID", model.SaleQuotEntryID));
                SqlParams.Add(new SqlParameter("@SaleQuotyearCode", model.SaleQuotyearCode));
                SqlParams.Add(new SqlParameter("@SaleQuotDate", SaleQuotDate == default ? string.Empty : SaleQuotDate));
                //SqlParams.Add(new SqlParameter("@BooktrnsEntryId", model.SaleBillEntryId));

                SqlParams.Add(new SqlParameter("@DGGrid", SBGrid));
                SqlParams.Add(new SqlParameter("@DTTaxGrid", TaxDetailDT));

                SqlParams.Add(new SqlParameter("@DRCRDATA", DrCrDetailDT));
                SqlParams.Add(new SqlParameter("@AgainstRef", AdjDetailDT));
                SqlParams.Add(new SqlParameter("@DTSSGridAdjust", AdjChallanDetailDT));

                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_SaleBillMainDetail", SqlParams);

            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }
            return _ResponseResult;
        }
        public async Task<ResponseResult> FillJWCustomerList(string SBJobwork, int yearCode)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FILLCustomerList"));
                SqlParams.Add(new SqlParameter("@SaleBillJobwork", SBJobwork));
                SqlParams.Add(new SqlParameter("@YearCode", yearCode));
                _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_SaleBillMainDetail", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        
        public async Task<ResponseResult> FillCustomerList(string SBJobwork, string ShowAllCustomer)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FillCustomerList"));
                SqlParams.Add(new SqlParameter("@SaleBillJobwork", SBJobwork));
                SqlParams.Add(new SqlParameter("@ShowAllCustomer", ShowAllCustomer));
                _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_SaleBillMainDetail", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }

        public async Task<ResponseResult> GetDistance(int accountCode)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "GetDistance"));
                SqlParams.Add(new SqlParameter("@accountcode", accountCode));
                _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_SaleBillMainDetail", SqlParams);
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
                _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_SaleBillMainDetail", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
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
        public async Task<ResponseResult> FillSONO(string billDate, string accountCode)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FILLSONO"));
                SqlParams.Add(new SqlParameter("@billdate", ParseDate(billDate)));
                SqlParams.Add(new SqlParameter("@Accountcode", accountCode));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_SaleBillMainDetail", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> FillConsigneeList(string showAllConsignee)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FILLConsigneeList"));
                SqlParams.Add(new SqlParameter("@ShowAllCustomer", showAllConsignee));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_SaleBillMainDetail", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> FillSOYearCode(string sono, string accountCode)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FillSOYearCode"));
                SqlParams.Add(new SqlParameter("@billdate", DateTime.Now));
                SqlParams.Add(new SqlParameter("@SOno", sono));
                SqlParams.Add(new SqlParameter("@accountcode", accountCode));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_SaleBillMainDetail", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> FillSOSchedule(string sono, string accountCode, int soYearCode, int ItemCode)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FILLSOSchedule"));
                SqlParams.Add(new SqlParameter("@billdate", DateTime.Now));
                SqlParams.Add(new SqlParameter("@SOno", sono));
                SqlParams.Add(new SqlParameter("@accountcode", accountCode));
                SqlParams.Add(new SqlParameter("@soYearCode", soYearCode));
                SqlParams.Add(new SqlParameter("@Itemcode", ItemCode));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_SaleBillMainDetail", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> FillSOItemRate(string sono, int soYearCode, int accountCode, string custOrderNo, int itemCode)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FillSOItemRate"));
                SqlParams.Add(new SqlParameter("@invoicedate", DateTime.Now));
                SqlParams.Add(new SqlParameter("@sono", sono));
                SqlParams.Add(new SqlParameter("@SoYearCode", soYearCode));
                SqlParams.Add(new SqlParameter("@accountcode", accountCode));
                SqlParams.Add(new SqlParameter("@custOrderNo", custOrderNo));
                SqlParams.Add(new SqlParameter("@itemcode", itemCode));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_SaleBillMainDetail", SqlParams);
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
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_SaleBillMainDetail", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }

        public async Task<ResponseResult> DisplaySODetail(string accountName, string itemName, string partCode, string sono, int soYearCode, string custOrderNo, string schNo, int schYearCode)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                var date = ParseDate((DateTime.Now).ToString());
                SqlParams.Add(new SqlParameter("@accountName", accountName ?? ""));
                SqlParams.Add(new SqlParameter("@itemName", itemName ?? ""));
                SqlParams.Add(new SqlParameter("@partcode", partCode ?? ""));
                SqlParams.Add(new SqlParameter("@SONO", sono ?? ""));
                SqlParams.Add(new SqlParameter("@CustOrderNo", custOrderNo ?? ""));
                SqlParams.Add(new SqlParameter("@SOYearCode", soYearCode));
                SqlParams.Add(new SqlParameter("@SchNo", schNo ?? ""));
                SqlParams.Add(new SqlParameter("@SchYearCode", schYearCode));
                SqlParams.Add(new SqlParameter("@CurrenctDate", date));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("AccSpGetPendingSaleOrderForBilling", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> GetDashboardData(string summaryDetail, string partCode, string itemName, string saleBillno, string customerName, string sono, string custOrderNo, string schNo, string performaInvNo, string saleQuoteNo, string domensticExportNEPZ, string fromdate, string toDate)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "DASHBOARD"));
                SqlParams.Add(new SqlParameter("@SummDetail", summaryDetail));
                SqlParams.Add(new SqlParameter("@partcode", partCode ?? ""));
                SqlParams.Add(new SqlParameter("@ItemName", itemName ?? ""));
                SqlParams.Add(new SqlParameter("@salebillno", saleBillno ?? ""));
                SqlParams.Add(new SqlParameter("@customerName", customerName ?? ""));
                SqlParams.Add(new SqlParameter("@SOno", sono));
                SqlParams.Add(new SqlParameter("@custOrderNo", custOrderNo ?? ""));
                SqlParams.Add(new SqlParameter("@ScheduleNo", schNo ?? ""));
                SqlParams.Add(new SqlParameter("@PerformaInvNo", performaInvNo ?? ""));
                SqlParams.Add(new SqlParameter("@SaleQuotNo", saleQuoteNo ?? ""));
                SqlParams.Add(new SqlParameter("@DomesticExportNEPZ", domensticExportNEPZ ?? ""));
                SqlParams.Add(new SqlParameter("@FromDate", fromdate));
                SqlParams.Add(new SqlParameter("@ToDate", toDate));
                _ResponseResult = await _IDataLogic.ExecuteDataSet("SP_SaleBillMainDetail", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> FILLSOScheduleDate(string sono, int accountCode, int soYearCode, string schNo, int schYearCode)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FILLSOScheduleDate"));
                SqlParams.Add(new SqlParameter("@billdate", DateTime.Now));
                SqlParams.Add(new SqlParameter("@Accountcode", accountCode));
                SqlParams.Add(new SqlParameter("@sOno", sono));
                SqlParams.Add(new SqlParameter("@soYearCode", soYearCode));
                SqlParams.Add(new SqlParameter("@ScheduleNo", schNo));
                SqlParams.Add(new SqlParameter("@ScheduleYearCode", schYearCode));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_SaleBillMainDetail", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }

        public async Task<ResponseResult> FILLCustomerOrderAndSPDate(string billDate, int accountCode, string sono, int soYearCode)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "FILLCustomerOrderAndSPDate"));

                SqlParams.Add(new SqlParameter("@billdate", ParseDate(billDate)));
                //SqlParams.Add(new SqlParameter("@billdate", billDate));
                SqlParams.Add(new SqlParameter("@Accountcode", accountCode));
                SqlParams.Add(new SqlParameter("@SOno", sono));
                SqlParams.Add(new SqlParameter("@SoYearCode", soYearCode));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_SaleBillMainDetail", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> AllowTaxPassword()
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "AllowTaxPassword"));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_SaleBillMainDetail", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> FillSOPendQty(int saleBillEntryId, string saleBillNo, int saleBillYearCode, string sono, int soYearcode, string custOrderNo, int itemCode, int accountCode, string schNo, int schYearCode)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();
                SqlParams.Add(new SqlParameter("@Flag", "getPendQtyofSOITEM"));
                SqlParams.Add(new SqlParameter("@SOno", sono));
                SqlParams.Add(new SqlParameter("@SoYearCode", soYearcode));
                SqlParams.Add(new SqlParameter("@custOrderNo", custOrderNo));
                SqlParams.Add(new SqlParameter("@itemCode", itemCode));
                SqlParams.Add(new SqlParameter("@AccountCode", accountCode));
                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_SaleBillMainDetail", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }

        public async Task<ResponseResult> FillSOWiseItems(string invoiceDate, string sono, int soYearCode, int accountCode, string schNo, int schYearCode, string sbJobWork)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@Flag", "SaleOrderWiseItemsList"));
                SqlParams.Add(new SqlParameter("@invoicedate", invoiceDate));
                SqlParams.Add(new SqlParameter("@sono", sono));
                SqlParams.Add(new SqlParameter("@SoYearCode",soYearCode));
                SqlParams.Add(new SqlParameter("@accountcode", accountCode));
                SqlParams.Add(new SqlParameter("@ScheduleNo", schNo));
                SqlParams.Add(new SqlParameter("@ScheduleYearCode", schYearCode));
                SqlParams.Add(new SqlParameter("@SaleBillJobwork", sbJobWork));

                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_SaleBillMainDetail", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> JWItemList(string typeItemServAssets, string showAll,string bomInd,string schNo,int schYearCode)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@Flag", "JWSaleBillItemList"));
                SqlParams.Add(new SqlParameter("@ShowAll", showAll));
                SqlParams.Add(new SqlParameter("@TypeItemServAssets", typeItemServAssets));
                SqlParams.Add(new SqlParameter("@ScheduleNo", schNo));
                SqlParams.Add(new SqlParameter("@ScheduleYearCode", schYearCode));
                SqlParams.Add(new SqlParameter("@SaleBillJobwork", "JOBWORK-SALEBILL"));
                SqlParams.Add(new SqlParameter("@BOMInd", bomInd));

                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_SaleBillMainDetail", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }
        public async Task<ResponseResult> FillItems(string showAll, string TypeItemServAssets, string sbJobwok)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@Flag", "ItemList"));
                SqlParams.Add(new SqlParameter("@ShowAll", showAll));
                SqlParams.Add(new SqlParameter("@TypeItemServAssets", TypeItemServAssets));
                SqlParams.Add(new SqlParameter("@SaleBillJobwork", sbJobwok));

                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_SaleBillMainDetail", SqlParams);
            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return _ResponseResult;
        }

        public async Task<SaleBillModel> GetViewByID(int ID, int YC, string Mode)
        {
            var model = new SaleBillModel();
            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@Flag", "VIEWBYID"));
                SqlParams.Add(new SqlParameter("@EntryID", ID));
                SqlParams.Add(new SqlParameter("@Yearcode", YC));

                var ResponseResult = await _IDataLogic.ExecuteDataSet("SP_SaleBillMainDetail", SqlParams);

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
        public async Task<List<CustomerJobWorkIssueAdjustDetail>> GetAdjustedChallanDetailsData(DataTable adjustedData, int YearCode, string EntryDate, string ChallanDate, int AccountCode)
        {
            var model = new List<CustomerJobWorkIssueAdjustDetail>();
            try
            {
                var SqlParams = new List<dynamic>();
                EntryDate = Common.CommonFunc.ParseFormattedDate(EntryDate);
                ChallanDate = Common.CommonFunc.ParseFormattedDate(ChallanDate);
                SqlParams.Add(new SqlParameter("@yearCode", YearCode));
                SqlParams.Add(new SqlParameter("@FromFinStartDate", EntryDate));
                SqlParams.Add(new SqlParameter("@billchallandate", ChallanDate));
                SqlParams.Add(new SqlParameter("@AccountCode", AccountCode));
                SqlParams.Add(new SqlParameter("@DTTItemGrid", adjustedData));

                var ResponseResult = await _IDataLogic.ExecuteDataSet("SpCustomerJobworkAdjustedChallanInGrid", SqlParams);

                if (ResponseResult.Result != null && ResponseResult.StatusCode == HttpStatusCode.OK && ResponseResult.StatusText == "Success")
                {
                    PrepareAdjustChallanView(ResponseResult.Result);
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

        //public List<CustomerJobWorkIssueAdjustDetail> GetAdjustedChallanDetailsData(DataTable adjustedData, int YearCode, string EntryDate, string ChallanDate, int AccountCode,int itemCode)
        //{
        //    List<CustomerJobWorkIssueAdjustDetail> result = new List<CustomerJobWorkIssueAdjustDetail>();

        //    using (SqlConnection conn = new SqlConnection(DBConnectionString))
        //    {
        //        conn.Open();
        //        EntryDate = Common.CommonFunc.ParseFormattedDate(EntryDate);
        //        ChallanDate = Common.CommonFunc.ParseFormattedDate(ChallanDate);

        //            using (SqlCommand cmd = new SqlCommand("SpCustomerJobworkAdjustedChallanInGrid", conn))
        //            {
        //                cmd.CommandType = CommandType.StoredProcedure;

        //                //cmd.Parameters.AddWithValue("@Flag", "JOBWORKISSUESUMMARY");
        //                cmd.Parameters.AddWithValue("@yearCode", YearCode);
        //                cmd.Parameters.AddWithValue("@FinYearFromDate", EntryDate);
        //                cmd.Parameters.AddWithValue("@billchallandate", ChallanDate);
        //                cmd.Parameters.AddWithValue("@AccountCode", AccountCode);
        //                cmd.Parameters.AddWithValue("@DTTItemGrid", adjustedData);
        //                //cmd.Parameters.AddWithValue("@BOMINd", item.BOMInd);
        //                cmd.Parameters.AddWithValue("@RMItemCode", itemCode);
        //                //cmd.Parameters.AddWithValue("@RMPartcode", item.PartCode);
        //                //cmd.Parameters.AddWithValue("@RMItemNAme", item.ItemName);

        //                try
        //                {
        //                    using (SqlDataReader reader = cmd.ExecuteReader())
        //                    {
        //                        while (reader.Read())
        //                        {
        //                            result.Add(new CustomerJobWorkIssueAdjustDetail
        //                            {
        //                                //CustomerName = reader["CustomerName"].ToString(),
        //                                //PartCode = reader["RecPartcode"].ToString(),
        //                                //ItemName = reader["RecItemName"].ToString(),
        //                                //BOMInd = reader["BOMIND"].ToString(),
        //                                //ChallanNo = reader["RecJWChallan"].ToString(),
        //                                //YearCode = Convert.ToInt32(reader["RecChallanYearCode"]),
        //                                //ChallanDate = DateTime.Parse(reader["RecChallandate"].ToString()).ToString("dd/MM/yyyy"),
        //                                //RecQty = Convert.ToInt32(reader["RecQty"]),
        //                                //IssQty = Convert.ToInt32(reader["IssQty"]),
        //                                //AccPendQty = Convert.ToInt32(reader["ActualPendQty"]),
        //                                //PendQty = Convert.ToInt32(reader["PendQty"]),
        //                            });
        //                        }
        //                    }
        //                }
        //                catch (Exception ex)
        //                {
        //                    Console.WriteLine($"Error: {ex.Message}");
        //                    throw; // Rethrow exception to see the detailed error
        //                }
        //            }

        //    }

        //    return result;
        //}
        internal async Task<ResponseResult> DeleteByID(int ID, int YC, string machineName)
        {
            var ResponseResult = new ResponseResult();
            try
            {
                var SqlParams = new List<dynamic>();

                SqlParams.Add(new SqlParameter("@Flag", "delete"));
                SqlParams.Add(new SqlParameter("@ENTRYID", ID));
                SqlParams.Add(new SqlParameter("@YearCode", YC));
                SqlParams.Add(new SqlParameter("@SaleBillEntryDate", DateTime.Now));
                SqlParams.Add(new SqlParameter("@EntryByMachineName", machineName));

                ResponseResult = await _IDataLogic.ExecuteDataSet("SP_SaleBillMainDetail", SqlParams);

            }
            catch (Exception ex)
            {
                dynamic Error = new ExpandoObject();
                Error.Message = ex.Message;
                Error.Source = ex.Source;
            }

            return ResponseResult;
        }
        private static SaleBillModel PrepareView(DataSet DS, ref SaleBillModel? model, string Mode)
        {
            var ItemGrid = new List<SaleBillDetail>();
            var SaleBillGrid = new List<SaleBillDetail>();
            var TaxGrid = new List<TaxModel>();
            var DRCRGrid = new List<DbCrModel>();
            var adjustGrid = new List<AdjustmentModel>();
            DS.Tables[0].TableName = "saleBillModel";
            DS.Tables[1].TableName = "saleBillDetail";
            DS.Tables[2].TableName = "saleBillTaxDetail";
            DS.Tables[3].TableName = "DRCRDetail";
            DS.Tables[4].TableName = "AdjustmentDetail";
            int cnt = 0;

            model.SaleBillEntryId = Convert.ToInt32(DS.Tables[0].Rows[0]["SaleBillEntryId"]);
            model.SaleBillYearCode = Convert.ToInt32(DS.Tables[0].Rows[0]["SaleBillYearCode"]);
            model.SaleBillEntryDate = DS.Tables[0].Rows[0]["SaleBillEntryDate"]?.ToString();
            model.InvPrefix = DS.Tables[0].Rows[0]["InvPrefix"]?.ToString();
            model.SaleBillJobwork = DS.Tables[0].Rows[0]["SaleBillJobwork"]?.ToString();
            model.SaleBillNo = DS.Tables[0].Rows[0]["SaleBillNo"]?.ToString();
            model.SaleBillDate = DS.Tables[0].Rows[0]["SaleBillDate"]?.ToString();
            model.InvoiceTime = DS.Tables[0].Rows[0]["InvoiceTime"]?.ToString();
            model.ExportInvoiceNo = DS.Tables[0].Rows[0]["ExportInvoiceNo"]?.ToString();
            model.PerformaInvNo = DS.Tables[0].Rows[0]["PerformaInvNo"]?.ToString();
            model.PerformaInvDate = DS.Tables[0].Rows[0]["PerformaInvDate"]?.ToString();
            model.PerformaInvYearCode = Convert.ToInt32(DS.Tables[0].Rows[0]["PerformaInvYearCode"]);
            model.BILLAgainstWarrenty = DS.Tables[0].Rows[0]["BILLAgainstWarrenty"]?.ToString();
            model.RemovalDate = DS.Tables[0].Rows[0]["RemovalDate"]?.ToString();
            model.RemovalTime = DS.Tables[0].Rows[0]["RemovalTime"]?.ToString();
            model.AccountCode = Convert.ToInt32(DS.Tables[0].Rows[0]["AccountCode"]);
            model.AccountName = DS.Tables[0].Rows[0]["Account_Name"]?.ToString();
            model.GSTNO = DS.Tables[0].Rows[0]["GSTNO"]?.ToString();
            model.DomesticExportNEPZ = DS.Tables[0].Rows[0]["DomesticExportNEPZ"]?.ToString();
            model.SupplyType = DS.Tables[0].Rows[0]["SupplyType"]?.ToString();
            model.CustAddress = DS.Tables[0].Rows[0]["CustAddress"]?.ToString();
            model.StateNameofSupply = DS.Tables[0].Rows[0]["StateNameofSupply"]?.ToString();
            model.StateCode = DS.Tables[0].Rows[0]["StateCode"]?.ToString();
            model.CityofSupply = DS.Tables[0].Rows[0]["CityofSupply"]?.ToString();
            model.CountryOfSupply = DS.Tables[0].Rows[0]["CountryOfSupply"]?.ToString();
            model.DistanceKM = Convert.ToInt32(DS.Tables[0].Rows[0]["DistanceKM"]);
            model.vehicleNo = DS.Tables[0].Rows[0]["vehicleNo"]?.ToString();
            model.TransporterName = DS.Tables[0].Rows[0]["TransporterName"]?.ToString();
            model.TransporterdocNo = DS.Tables[0].Rows[0]["TransporterdocNo"]?.ToString();
            model.TransporterId = Convert.ToInt32(DS.Tables[0].Rows[0]["TransporterId"]);
            model.TransportModeBYRoadAIR = DS.Tables[0].Rows[0]["TransportModeBYRoadAIR"]?.ToString();
            model.ConsigneeAccountcode = Convert.ToInt32(DS.Tables[0].Rows[0]["ConsigneeAccountcode"]);
            model.ConsigneeAccountName = DS.Tables[0].Rows[0]["ConsigneeName"]?.ToString();
            model.ConsigneeAddress = DS.Tables[0].Rows[0]["ConsigneeAddress"]?.ToString();
            model.DispatchTo = DS.Tables[0].Rows[0]["DispatchTo"]?.ToString();
            model.DispatchThrough = DS.Tables[0].Rows[0]["DispatchThrough"]?.ToString();
            model.DocTypeAccountCode = Convert.ToInt32(DS.Tables[0].Rows[0]["DocTypeAccountCode"]);
            //model.DocTypeAccountName = DS.Tables[0].Rows[0]["Entrydate"]?.ToString();
            model.PaymentTerm = Convert.ToInt32(DS.Tables[0].Rows[0]["PaymentTerm"]);
            model.BillAmt = Convert.ToInt32(DS.Tables[0].Rows[0]["BillAmt"]);
            model.BillAmtWord = DS.Tables[0].Rows[0]["BillAmtWord"]?.ToString();
            model.TaxableAmt = Convert.ToInt32(DS.Tables[0].Rows[0]["TaxableAmt"]);
            model.TaxbaleAmtInWord = DS.Tables[0].Rows[0]["TaxbaleAmtInWord"]?.ToString();
            model.GSTAmount = Convert.ToInt32(DS.Tables[0].Rows[0]["GSTAmount"]);
            model.RoundTypea = DS.Tables[0].Rows[0]["RoundType"]?.ToString();
            model.RoundOffAmt = Convert.ToInt32(DS.Tables[0].Rows[0]["RoundOffAmt"]);
            model.DiscountPercent = Convert.ToInt32(DS.Tables[0].Rows[0]["DiscountPercent"]);
            model.DiscountAmt = Convert.ToInt32(DS.Tables[0].Rows[0]["DiscountAmt"]);
            model.INVNetAmt = Convert.ToInt32(DS.Tables[0].Rows[0]["INVNetAmt"]);
            model.NetAmtInWords = DS.Tables[0].Rows[0]["NetAmtInWords"]?.ToString();
            model.Remark = DS.Tables[0].Rows[0]["Remark"]?.ToString();
            model.PermitNo = DS.Tables[0].Rows[0]["PermitNo"]?.ToString();
            model.CashDisPer = Convert.ToInt32(DS.Tables[0].Rows[0]["CashDisPer"]);
            model.CashDisRs = Convert.ToInt32(DS.Tables[0].Rows[0]["CashDisRs"]);
            model.SoDelTime = DS.Tables[0].Rows[0]["SoDelTime"]?.ToString();
            model.TypeJob = DS.Tables[0].Rows[0]["TypeJob"]?.ToString();
            model.Approved = DS.Tables[0].Rows[0]["Approved"]?.ToString();
            model.ApprovDate = DS.Tables[0].Rows[0]["ApprovDate"]?.ToString();
            model.ApprovedBy = Convert.ToInt32(DS.Tables[0].Rows[0]["ApprovedBy"]);
            model.currencyId = Convert.ToInt32(DS.Tables[0].Rows[0]["currencyId"]);
            model.ExchangeRate = Convert.ToInt32(DS.Tables[0].Rows[0]["ExchangeRate"]);
            model.TypeItemServAssets = DS.Tables[0].Rows[0]["TypeItemServAssets"]?.ToString();
            model.Shippingdate = DS.Tables[0].Rows[0]["Shippingdate"]?.ToString();
            model.CancelBill = DS.Tables[0].Rows[0]["CancelBill"]?.ToString();
            model.Canceldate = DS.Tables[0].Rows[0]["Canceldate"]?.ToString();
            model.CancelBy = Convert.ToInt32(DS.Tables[0].Rows[0]["CancelBy"]);
            model.Cancelreason = DS.Tables[0].Rows[0]["Cancelreason"]?.ToString();
            model.BankName = DS.Tables[0].Rows[0]["BankName"]?.ToString();
            model.Ewaybillno = DS.Tables[0].Rows[0]["Ewaybillno"]?.ToString();
            model.FreightPaid = DS.Tables[0].Rows[0]["FreightPaid"]?.ToString();
            model.dispatchLocation = DS.Tables[0].Rows[0]["dispatchLocation"]?.ToString();
            model.currExchangeRate = Convert.ToInt32(DS.Tables[0].Rows[0]["ExchangeRate"]);
            model.DispatchDelayReason = DS.Tables[0].Rows[0]["DispatchDelayReason"]?.ToString();
            model.AttachmentFilePath1 = DS.Tables[0].Rows[0]["AttachmentFilePath1"]?.ToString();
            model.AttachmentFilePath2 = DS.Tables[0].Rows[0]["AttachmentFilePath2"]?.ToString();
            model.AttachmentFilePath3 = DS.Tables[0].Rows[0]["AttachmentFilePath3"]?.ToString();
            model.DocketNo = DS.Tables[0].Rows[0]["DocketNo"]?.ToString();
            model.DispatchDelayreson = DS.Tables[0].Rows[0]["DispatchDelayreson"]?.ToString();
            model.Commodity = DS.Tables[0].Rows[0]["Commodity"]?.ToString();
            model.EInvNo = DS.Tables[0].Rows[0]["EInvNo"]?.ToString();
            model.EinvGenerated = DS.Tables[0].Rows[0]["EinvGenerated"]?.ToString();
            model.CC = DS.Tables[0].Rows[0]["CC"]?.ToString();
            model.Uid = Convert.ToInt32(DS.Tables[0].Rows[0]["Uid"]);
            model.EntryByempId = Convert.ToInt32(DS.Tables[0].Rows[0]["EntryByempId"]);
            model.MachineName = DS.Tables[0].Rows[0]["MachineName"]?.ToString();
            model.ActualEntryDate = DS.Tables[0].Rows[0]["ActualEntryDate"]?.ToString();
            model.ActualEnteredBy = Convert.ToInt32(DS.Tables[0].Rows[0]["ActualEnteredBy"]);
            model.ActualEnteredByName = DS.Tables[0].Rows[0]["EntrybyEmp"]?.ToString();
            model.EntryFreezToAccounts = DS.Tables[0].Rows[0]["EntryFreezToAccounts"]?.ToString();
            model.PaymentCreditDay = Convert.ToInt32(DS.Tables[0].Rows[0]["PaymentCreditDay"]);
            model.ChallanNo = DS.Tables[0].Rows[0]["ChallanNo"]?.ToString();
            model.ChallanDate = DS.Tables[0].Rows[0]["ChallanDate"]?.ToString();
            model.ChallanYearCode = Convert.ToInt32(DS.Tables[0].Rows[0]["ChallanYearCode"]);
            model.ChallanEntryid = Convert.ToInt32(DS.Tables[0].Rows[0]["ChallanEntryid"]);
            model.BalanceSheetClosed = DS.Tables[0].Rows[0]["BalanceSheetClosed"]?.ToString();
            model.SaleQuotNo = DS.Tables[0].Rows[0]["SaleQuotNo"]?.ToString();
            model.SaleQuotEntryID = Convert.ToInt32(DS.Tables[0].Rows[0]["SaleQuotEntryID"]);
            model.SaleQuotyearCode = Convert.ToInt32(DS.Tables[0].Rows[0]["SaleQuotyearCode"]);
            model.SaleQuotDate = DS.Tables[0].Rows[0]["SaleQuotDate"]?.ToString();

            //if (model.AttachmentFilePath1 != null)
            //{
            //    byte[] fileBytes = File.ReadAllBytes(model.AttachmentFilePath1);
            //    var memoryStream = new MemoryStream(fileBytes);

            //    model.AttachmentFile1 = new FormFile(memoryStream, 0, memoryStream.Length, "file", Path.GetFileName(model.AttachmentFilePath1))
            //    {
            //        ContentType = "application/octet-stream"
            //    };
            //}

            //if (model.AttachmentFilePath2 != null)
            //{
            //    byte[] fileBytes = File.ReadAllBytes(model.AttachmentFilePath2);
            //    var memoryStream = new MemoryStream(fileBytes);

            //    model.AttachmentFile2 = new FormFile(memoryStream, 0, memoryStream.Length, "file", Path.GetFileName(model.AttachmentFilePath2))
            //    {
            //        ContentType = "application/octet-stream"
            //    };
            //}

            //if (model.AttachmentFilePath3 != null)
            //{
            //    byte[] fileBytes = File.ReadAllBytes(model.AttachmentFilePath3);
            //    var memoryStream = new MemoryStream(fileBytes);

            //    model.AttachmentFile3 = new FormFile(memoryStream, 0, memoryStream.Length, "file", Path.GetFileName(model.AttachmentFilePath3))
            //    {
            //        ContentType = "application/octet-stream"
            //    };
            //}

            if (Mode == "U" || Mode == "V")
            {
                if (DS.Tables[0].Rows[0]["UpdatedbyEmp"].ToString() != "")
                {
                    model.LastUpdatedBy = Convert.ToInt32(DS.Tables[0].Rows[0]["LastUpdatedBy"].ToString());
                    model.LastUpdatedByName = DS.Tables[0].Rows[0]["UpdatedbyEmp"].ToString();
                    model.LastUpdationDate = DS.Tables[0].Rows[0]["LastUpdatedDate"].ToString();
                }
            }

            if (DS.Tables.Count != 0 && DS.Tables[1].Rows.Count > 0)
            {
                foreach (DataRow row in DS.Tables[1].Rows)
                {
                    SaleBillGrid.Add(new SaleBillDetail
                    {
                        SeqNo = Convert.ToInt32(row["SeqNo"]),
                        ItemCode = row["ItemCode"] != DBNull.Value ? Convert.ToInt32(row["ItemCode"]) : 0,
                        PartCode = row["PartCode"]?.ToString(),
                        Amount = row["ItemAmount"] != DBNull.Value ? Convert.ToDecimal(row["ItemAmount"]) : 0,
                        ItemName = row["ItemName"]?.ToString(),
                        CustOrderNo = row["CustOrderNo"]?.ToString(),
                        SOYearCode = row["SOYearCode"] != DBNull.Value ? Convert.ToInt32(row["SOYearCode"]) : 0,
                        SONO = row["SONO"]?.ToString(),
                        StoreName = row["StoreName"]?.ToString(),
                        SODate = row["SODate"]?.ToString(),
                        SchNo = row["SchNo"]?.ToString(),
                        Schdate = row["Schdate"]?.ToString(),
                        SaleSchYearCode = row["SaleSchYearCode"] != DBNull.Value ? Convert.ToInt32(row["SaleSchYearCode"]) : 0,
                        SOAmendNo = row["SOAmendNo"]?.ToString(),
                        SOAmendDate = row["SOAmendDate"]?.ToString(),
                        HSNNo = row["HSNNO"] != DBNull.Value ? Convert.ToInt32(row["HSNNO"]) : 0,
                        Unit = row["Unit"]?.ToString(),
                        NoofCase = row["NoofCase"] != DBNull.Value ? Convert.ToInt32(row["NoofCase"]) : 0,
                        Qty = row["BillQty"] != DBNull.Value ? Convert.ToInt32(row["BillQty"]) : 0,
                        UnitOfRate = row["UnitOfRate"]?.ToString(),
                        RateInOtherCurr = row["RateInOtherCurr"] != DBNull.Value ? Convert.ToInt32(row["RateInOtherCurr"]) : 0,
                        Rate = row["BillRate"] != DBNull.Value ? Convert.ToInt32(row["BillRate"]) : 0,
                        AltUnit = row["AltUnit"]?.ToString(),
                        AltQty = row["AltQty"] != DBNull.Value ? Convert.ToInt32(row["AltQty"]) : 0,
                        ItemWeight = row["ItemWeight"] != DBNull.Value ? Convert.ToInt32(row["ItemWeight"]) : 0,
                        NoofPcs = row["NoofPcs"] != DBNull.Value ? Convert.ToInt32(row["NoofPcs"]) : 0,
                        CustomerPartCode = row["CustomerPartCode"]?.ToString(),
                        MRP = row["MRP"] != DBNull.Value ? Convert.ToInt32(row["MRP"]) : 0,
                        OriginalMRP = row["OriginalMRP"] != DBNull.Value ? Convert.ToInt32(row["OriginalMRP"]) : 0,
                        SOPendQty = row["SOPendQty"] != DBNull.Value ? Convert.ToInt32(row["SOPendQty"]) : 0,
                        AltSOPendQty = row["AltSOPendQty"] != DBNull.Value ? Convert.ToInt32(row["AltSOPendQty"]) : 0,
                        DiscountPer = row["DiscountPer"] != DBNull.Value ? Convert.ToInt32(row["DiscountPer"]) : 0,
                        DiscountAmt = row["DiscountAmt"] != DBNull.Value ? Convert.ToInt32(row["DiscountAmt"]) : 0,
                        ItemSize = row["ItemSize"]?.ToString(),
                        Itemcolor = row["Itemcolor"]?.ToString(),
                        StoreId = row["StoreId"] != DBNull.Value ? Convert.ToInt32(row["StoreId"]) : 0,
                        ItemNetAmount = row["ItemAmount"] != DBNull.Value ? Convert.ToInt32(row["ItemAmount"]) : 0,
                        AdviceNo = row["AdviceNo"]?.ToString(),
                        AdviseEntryId = row["AdviseEntryId"] != DBNull.Value ? Convert.ToInt32(row["AdviseEntryId"]) : 0,
                        AdviceYearCode = row["AdviceYearCode"] != DBNull.Value ? Convert.ToInt32(row["AdviceYearCode"]) : 0,
                        AdviseDate = row["AdviseDate"]?.ToString(),
                        ProcessId = row["ProcessId"] != DBNull.Value ? Convert.ToInt32(row["ProcessId"]) : 0,
                        Batchno = row["batchno"]?.ToString(),
                        Uniquebatchno = row["uniquebatchno"]?.ToString(),
                        LotStock = row["LotStock"] != DBNull.Value ? Convert.ToInt32(row["LotStock"]) : 0,
                        TotalStock = row["TotalStock"] != DBNull.Value ? Convert.ToInt32(row["TotalStock"]) : 0,
                        AgainstProdPlanNo = row["AgainstProdPlanNo"]?.ToString(),
                        AgainstProdPlanYearCode = row["AgainstProdPlanYearCode"] != DBNull.Value ? Convert.ToInt32(row["AgainstProdPlanYearCode"]) : 0,
                        AgaisntProdPlanDate = row["AgaisntProdPlanDate"]?.ToString(),
                        GSTPer = row["GSTPer"] != DBNull.Value ? Convert.ToInt32(row["GSTPer"]) : 0,
                        GSTType = row["GSTType"]?.ToString(),
                        PacketsDetail = row["PacketsDetail"]?.ToString(),
                        OtherDetail = row["OtherDetail"]?.ToString(),
                        ItemRemark = row["ItemRemark"]?.ToString(),
                        ProdSchno = row["prodSchno"]?.ToString(),
                        ProdSchYearcode = row["ProdSchYearcode"] != DBNull.Value ? Convert.ToInt32(row["ProdSchYearcode"]) : 0,
                        ProdSchEntryId = row["prodSchEntryId"] != DBNull.Value ? Convert.ToInt32(row["prodSchEntryId"]) : 0,
                        ProdSchDate = row["ProdSchDate"]?.ToString(),
                        SchdeliveryDate = row["SchdeliveryDate"]?.ToString(),
                        CostCenterId = row["CostCenterid"] != DBNull.Value ? Convert.ToInt32(row["CostCenterid"]) : 0
                    });
                }
                SaleBillGrid = SaleBillGrid.OrderBy(item => item.SeqNo).ToList();
                model.saleBillDetails = SaleBillGrid;
                model.ItemDetailGrid = SaleBillGrid;
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

        private static List<CustomerJobWorkIssueAdjustDetail> PrepareAdjustChallanView(DataSet DS)
        {
            var ItemGrid = new List<CustomerJobWorkIssueAdjustDetail>();
            DS.Tables[0].TableName = "CustomerJobWorkAdjustDetail";
            int cnt = 0;

            if (DS.Tables.Count != 0 && DS.Tables[1].Rows.Count > 0)
            {
                foreach (DataRow row in DS.Tables[1].Rows)
                {
                    ItemGrid.AddRange(new List<CustomerJobWorkIssueAdjustDetail>
                        {
                            new CustomerJobWorkIssueAdjustDetail
                            {
                                CustomerName = row["CustomerName"].ToString(),
                                PartCode = row["RecPartcode"].ToString(),
                                ItemName = row["RecItemName"].ToString(),
                                BOMIND = row["BOMIND"].ToString(),
                                ChallanNo = row["RecJWChallan"].ToString(),
                                YearCode = Convert.ToInt32(row["RecChallanYearCode"]),
                                ChallanDate = row["RecChallandate"].ToString(),
                                RecQty = row["RecQty"] != DBNull.Value ? Convert.ToSingle(row["RecQty"]) : 0,
                                IssQty = row["IssQty"] != null ? Convert.ToSingle(row["IssQty"]) : 0,
                                AccPendQty = row["ActualPendQty"] != DBNull.Value ? Convert.ToSingle(row["ActualPendQty"]) : 0,
                                PendQty = row["PendQty"] != DBNull.Value ? Convert.ToSingle(row["PendQty"]) : 0
                            }
                        });
                }

            }

            return ItemGrid;
        }



    }
}
