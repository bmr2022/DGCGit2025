using System;
using System.Collections.Generic;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.DOM.Models
{
    public class SaleBillApprovalModel
    {
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string ApproveAndUnApprove { get; set; }
        public string SaleBillNumber { get; set; }
        public string SaleBillType { get; set; }
        public string Mode { get; set; }
        public int ActualEntryId { get; set; }
        public int EmpId { get; set; }
        public string EntryByEmpName { get; set; }
        public string ActualEntryDate { get; set; }
        public string MachineName { get; set; }
        public string? EntryDate { get; set; }
        public string ActualEntryByEmpName { get; set; }

        // List of sale bills to display
        public List<SaleBillApprovalDashboard> SaleBillList { get; set; } = new List<SaleBillApprovalDashboard>();
    }

    // Model for each sale bill row in the dashboard
    public class SaleBillApprovalDashboard
    {
        public string SaleBillNo { get; set; }
        public string SaleBillDate { get; set; }
        public string AccountName { get; set; }
        public string GSTNO { get; set; }
        public decimal? BillAmt { get; set; }
        public decimal? TaxableAmt { get; set; }
        public decimal? INVNetAmt { get; set; }
        public string SupplyType { get; set; }
        public string StateNameofSupply { get; set; }
        public string CityofSupply { get; set; }
        public string ConsigneeAccountName { get; set; }
        public string ConsigneeAddress { get; set; }
        public string PaymentTerm { get; set; }
        public string Currency { get; set; }
        public decimal? GSTAmount { get; set; }
        public string RoundTypea { get; set; }
        public decimal? RoundOffAmt { get; set; }
        public string Ewaybillno { get; set; }
        public string EInvNo { get; set; }
        public string EinvGenerated { get; set; }
        public string TransporterdocNo { get; set; }
        public string TransportModeBYRoadAIR { get; set; }
        public string DispatchTo { get; set; }
        public string DispatchThrough { get; set; }
        public decimal? ExchangeRate { get; set; }
        public decimal? currExchangeRate { get; set; }
        public int SaleBillEntryId { get; set; }
        public string SaleBillYearCode { get; set; }
        public string SaleBillEntryDate { get; set; }
        public string Shippingdate { get; set; }
        public decimal? DistanceKM { get; set; }
        public string vehicleNo { get; set; }
        public string TransporterName { get; set; }
        public string DomesticExportNEPZ { get; set; }
        public int? PaymentCreditDay { get; set; }
        public decimal? DiscountAmt { get; set; }
        public decimal? SOPendQty { get; set; }
        public string CancelBill { get; set; }
        public string Canceldate { get; set; }
        public string CancelBy { get; set; }
        public string Cancelreason { get; set; }
        public string BankName { get; set; }
        public string FreightPaid { get; set; }
        public string DispatchDelayReason { get; set; }
        public string AttachmentFilePath1 { get; set; }
        public string AttachmentFilePath2 { get; set; }
        public string AttachmentFilePath3 { get; set; }
        public string DocketNo { get; set; }
        public string DispatchDelayreson { get; set; }
        public string Commodity { get; set; }
        public string TypeItemServAssets { get; set; }
        public string SaleBillJobwork { get; set; }
        public string PerformaInvNo { get; set; }
        public string PerformaInvDate { get; set; }
        public string PerformaInvYearCode { get; set; }
        public string BILLAgainstWarrenty { get; set; }
        public string ExportInvoiceNo { get; set; }
        public string InvoiceTime { get; set; }
        public string RemovalDate { get; set; }
        public string RemovalTime { get; set; }
        public string EntryFreezToAccounts { get; set; }
        public string BalanceSheetClosed { get; set; }
        public string SaleQuotNo { get; set; }
        public string SaleQuotDate { get; set; }
        public string Remark { get; set; }
        public string Approved { get; set; }
        public string ApprovDate { get; set; }
        public string ApprovedBy { get; set; }
        public string CustAddress { get; set; }
        public string LastUpdatedBy { get; set; }
        public string LastUpdationDate { get; set; }
        public string CC { get; set; }
        public string Uid { get; set; }
        public string ActualEnteredByName { get; set; }
        public string ActualEntryDate { get; set; }
        public string MachineName { get; set; }
        public string AgainstVoucherNo { get; set; }
    }
}
