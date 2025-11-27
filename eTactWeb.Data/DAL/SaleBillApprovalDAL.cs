using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eTactWeb.Data.Common;
using eTactWeb.DOM.Models;
using eTactWeb.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.Data.DAL
{
    public class SaleBillApprovalDAL
    {
        private readonly IDataLogic _IDataLogic;
        private readonly string DBConnectionString = string.Empty;
        private IDataReader? Reader;
        private readonly ConnectionStringService _connectionStringService;
        public SaleBillApprovalDAL(IConfiguration configuration, IDataLogic iDataLogic, ConnectionStringService connectionStringService)
        {
            //DBConnectionString = configuration.GetConnectionString("eTactDB");
            _connectionStringService = connectionStringService;
            DBConnectionString = _connectionStringService.GetConnectionString();
            _IDataLogic = iDataLogic;
        }


        public async Task<ResponseResult> GetPendingSaleBillSummary(string fromDate, string toDate, string saleBillEntryFrom)
        {
            var _ResponseResult = new ResponseResult();
            try
            {
                var sqlParams = new List<dynamic>
                {
                    new SqlParameter("@Flag", "ListOfPendingSaleBillforApproval"),
                    new SqlParameter("@FromDate", fromDate),
                    new SqlParameter("@ToDate", toDate),
                    new SqlParameter("@SaleBillEntryFrom", saleBillEntryFrom),
                    new SqlParameter("@SummDetail", "Summary")
                };

                _ResponseResult = await _IDataLogic.ExecuteDataTable("SP_SaleBillMainDetail", sqlParams);

                // Map DataTable to List<SaleBillApprovalDashboard> if needed
                if (_ResponseResult.Result is DataTable dt && dt.Rows.Count > 0)
                {
                    var list = (from DataRow dr in dt.Rows
                                select new SaleBillApprovalDashboard
                                {
                                    SaleBillNo = dr["SaleBillNo"]?.ToString(),
                                    SaleBillDate = dr["SaleBillDate"]?.ToString(),
                                    AccountName = dr["AccountName"]?.ToString(),
                                    GSTNO = dr["GSTNO"]?.ToString(),
                                    BillAmt = dr["BillAmt"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["BillAmt"]),
                                    TaxableAmt = dr["TaxableAmt"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["TaxableAmt"]),
                                    INVNetAmt = dr["INVNetAmt"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["INVNetAmt"]),
                                    SupplyType = dr["SupplyType"]?.ToString(),
                                    StateNameofSupply = dr["StateNameofSupply"]?.ToString(),
                                    CityofSupply = dr["CityofSupply"]?.ToString(),
                                    ConsigneeAccountName = dr["ConsigneeAccountName"]?.ToString(),
                                    ConsigneeAddress = dr["ConsigneeAddress"]?.ToString(),
                                    PaymentTerm = dr["PaymentTerm"]?.ToString(),
                                    Currency = dr["currency"]?.ToString(),
                                    GSTAmount = dr["GSTAmount"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["GSTAmount"]),
                                    RoundTypea = dr["RoundType"]?.ToString(),
                                    RoundOffAmt = dr["RoundOffAmt"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["RoundOffAmt"]),
                                    Ewaybillno = dr["Ewaybillno"]?.ToString(),
                                    EInvNo = dr["EInvNo"]?.ToString(),
                                    EinvGenerated = dr["EinvGenerated"]?.ToString(),
                                    TransporterdocNo = dr["TransporterdocNo"]?.ToString(),
                                    TransportModeBYRoadAIR = dr["TransportModeBYRoadAIR"]?.ToString(),
                                    DispatchTo = dr["DispatchTo"]?.ToString(),
                                    DispatchThrough = dr["DispatchThrough"]?.ToString(),
                                    ExchangeRate = dr["ExchangeRate"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["ExchangeRate"]),
                                    SaleBillEntryId = dr["SaleBillEntryId"] == DBNull.Value ? 0 : Convert.ToInt32(dr["SaleBillEntryId"]),
                                    SaleBillYearCode = dr["SaleBillYearCode"]?.ToString(),
                                    SaleBillEntryDate = dr["SaleBillEntryDate"]?.ToString(),
                                    Shippingdate = dr["Shippingdate"]?.ToString(),
                                    DistanceKM = dr["DistanceKM"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["DistanceKM"]),
                                    vehicleNo = dr["vehicleNo"]?.ToString(),
                                    TransporterName = dr["TransporterName"]?.ToString(),
                                    DomesticExportNEPZ = dr["DomesticExportNEPZ"]?.ToString(),
                                    PaymentCreditDay = dr["PaymentCreditDay"] == DBNull.Value ? 0 : Convert.ToInt32(dr["PaymentCreditDay"]),
                                    CancelBill = dr["CancelBill"]?.ToString(),
                                    Canceldate = dr["Canceldate"]?.ToString(),
                                    CancelBy = dr["CancelBy"]?.ToString(),
                                    Cancelreason = dr["Cancelreason"]?.ToString(),
                                    BankName = dr["BankName"]?.ToString(),
                                    FreightPaid = dr["FreightPaid"]?.ToString(),
                                    DispatchDelayReason = dr["DispatchDelayReason"]?.ToString(),
                                    AttachmentFilePath1 = dr["AttachmentFilePath1"]?.ToString(),
                                    AttachmentFilePath2 = dr["AttachmentFilePath2"]?.ToString(),
                                    AttachmentFilePath3 = dr["AttachmentFilePath3"]?.ToString(),
                                    DocketNo = dr["DocketNo"]?.ToString(),
                                    DispatchDelayreson = dr["DispatchDelayreson"]?.ToString(),
                                    Commodity = dr["Commodity"]?.ToString(),
                                    TypeItemServAssets = dr["TypeItemServAssets"]?.ToString(),
                                    SaleBillJobwork = dr["SaleBillJobwork"]?.ToString(),
                                    PerformaInvNo = dr["PerformaInvNo"]?.ToString(),
                                    PerformaInvDate = dr["PerformaInvDate"]?.ToString(),
                                    PerformaInvYearCode = dr["PerformaInvYearCode"]?.ToString(),
                                    BILLAgainstWarrenty = dr["BILLAgainstWarrenty"]?.ToString(),
                                    ExportInvoiceNo = dr["ExportInvoiceNo"]?.ToString(),
                                    InvoiceTime = dr["InvoiceTime"]?.ToString(),
                                    RemovalDate = dr["RemovalDate"]?.ToString(),
                                    RemovalTime = dr["RemovalTime"]?.ToString(),
                                    EntryFreezToAccounts = dr["EntryFreezToAccounts"]?.ToString(),
                                    BalanceSheetClosed = dr["BalanceSheetClosed"]?.ToString(),
                                    SaleQuotNo = dr["SaleQuotNo"]?.ToString(),
                                    SaleQuotDate = dr["SaleQuotDate"]?.ToString(),
                                    Remark = dr["Remark"]?.ToString(),
                                    Approved = dr["Approved"]?.ToString(),
                                    ApprovDate = dr["ApprovDate"]?.ToString(),
                                    ApprovedBy = dr["ApprovedBy"]?.ToString(),
                                    CustAddress = dr["CustAddress"]?.ToString(),
                                    LastUpdatedBy = dr["LastUpdatedByName"]?.ToString(),
                                    LastUpdationDate = dr["LastUpdationDate"]?.ToString(),
                                    CC = dr["CC"]?.ToString(),
                                    Uid = dr["Uid"]?.ToString(),
                                    ActualEnteredByName = dr["ActualEnteredByName"]?.ToString(),
                                    ActualEntryDate = dr["ActualEntryDate"]?.ToString(),
                                    MachineName = dr["MachineName"]?.ToString()
                                }).ToList();

                    _ResponseResult.Result = list;
                    _ResponseResult.StatusCode = System.Net.HttpStatusCode.OK;
                    _ResponseResult.StatusText = "Success";
                }
            }
            catch (Exception ex)
            {
                _ResponseResult.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                _ResponseResult.StatusText = "Error";
                _ResponseResult.Result = new { ex.Message, ex.StackTrace };
            }

            return _ResponseResult;
        }

    }
}

