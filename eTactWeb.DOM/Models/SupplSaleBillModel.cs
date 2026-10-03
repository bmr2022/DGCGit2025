using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.DOM.Models
{
    public class AccSupplSaleBillModel : AccSupplSaleBillDetail
    {
        public int SupplSaleBillEntryId { get; set; }
        public int SupplSaleBillYearCode { get; set; }
        public string? IPAddress { get; set; }
        public string? formKey { get; set; }
        public string? InvoiceType { get; set; }
        public string? uniqueKey { get; set; }
        public string SupplSaleBillInvoiceNo { get; set; } = string.Empty;
        public string SupplSaleBillInvoiceDate { get; set; }
        public string SubVoucherName { get; set; } = string.Empty;
        //public string SupplSaleBillVoucherNo { get; set; } = string.Empty;
        // public string SupplSaleBillVoucherDate { get; set; }
        // public string AgainstSalePurchase { get; set; } = string.Empty;
        public int AccountCode { get; set; }
        public string AccountName { get; set; }
        public string CustVendAddress { get; set; } = string.Empty;
        public string StateNameofSupply { get; set; } = string.Empty;
        public string StateCode { get; set; } = string.Empty;
        public string CityofSupply { get; set; } = string.Empty;
        public string CountryOfSupply { get; set; } = string.Empty;
        public string PaymentTerm { get; set; } = string.Empty;
        public int PaymentCreditDay { get; set; }
        public string GSTNO { get; set; } = string.Empty;
        public string? SaleBillJobwork { get; set; } = string.Empty;
        public string GstRegUnreg { get; set; } = string.Empty;
        public string Transporter { get; set; } = string.Empty;
        public string Vehicleno { get; set; } = string.Empty;
        public decimal BillAmt { get; set; }
        public decimal RoundOffAmt { get; set; }
        public string RoundoffType { get; set; } = string.Empty;
        public decimal Taxableamt { get; set; }
        public decimal NetAmt { get; set; }
        public string? OtherRemark { get; set; }
        public string CC { get; set; } = string.Empty;
        public int Uid { get; set; }
        public string ItemService { get; set; } = string.Empty;
        public string INVOICETYPE { get; set; } = string.Empty;
        public string MachineName { get; set; } = string.Empty;
        public string ActualEntryDate { get; set; }
        public int ActualEnteredBy { get; set; }
        public string ActualEnteredByName { get; set; }
        public int? LastUpdatedBy { get; set; }
        public string LastUpdatedByName { get; set; }
        public string? LastUpdationDate { get; set; }
        public string? EntryFreezToAccounts { get; set; }
        public string? BalanceSheetClosed { get; set; }
        public string? EInvNo { get; set; }
        public string? SupplyType { get; set; }
        public string? EinvGenerated { get; set; }
        public string AttachmentFilePath1 { get; set; } = string.Empty;
        public string AttachmentFilePath2 { get; set; } = string.Empty;
        public string AttachmentFilePath3 { get; set; } = string.Empty;
        public string FinFromDate { get; set; }
        public string FinToDate { get; set; }
        public int PageNumber { get; set; }
        public int TotalRecords { get; set; }
        public int PageSize { get; set; }
        public string DashboardTypeBack { get; set; }
        public string FromDateBack { get; set; }
        public string ToDateBack { get; set; }
        public int? AccountCodeBack { get; set; }
        public int? GroupCodeBack { get; set; }
        public int? AccountNameBack { get; set; }
        public string? VoucherTypeBack { get; set; }
        public string? VoucherNoBack { get; set; }
        public string[] AccountList { get; set; }
        private IList<SelectListItem> _YesNo = new List<SelectListItem>()
        {
            new() { Value = "Y", Text = "Yes" },
            new() { Value = "N", Text = "No" },
        };

        public IList<SelectListItem> YesNoList
        {
            get => _YesNo;
            set => _YesNo = value;
        }
        public List<AccSupplSaleBillDetail> AccSupplSaleBillDetails { get; set; }
        public List<AccSupplSaleBillAgainstBillDetail> AccSupplSaleBillAgainstBillDetails { get; set; }
        public IList<AccSupplSaleBillDetail>? ItemDetailGrid { get; set; }
        public string? TallyCompanyName { get; set; }
        public string? TallyGUID { get; set; }
    }

    public class AccSupplSaleBillDetail : AccSupplSaleBillAgainstBillDetail
    {
        public AdjustmentModel adjustmentModel { get; set; }
        public int SeqNo { get; set; }
        public int ItemCode { get; set; }
        public bool ItemSA { get; set; }
        public bool DA { get; set; }
        public bool ShowAllBill { get; set; }
        public string ItemName { get; set; }
        public string PartCode { get; set; }
        public string HSNNo { get; set; }
        public decimal BillQty { get; set; }
        public decimal SupplBillQty { get; set; }
        public string Unit { get; set; } = string.Empty;
        public decimal AltQty { get; set; }
        public string AltUnit { get; set; } = string.Empty;
        public decimal AdditionalRate { get; set; }
        public decimal BillRate { get; set; }
        public string UnitRate { get; set; } = string.Empty;
        public decimal AltRate { get; set; }
        public int CostCenterId { get; set; }
        public int DocAccountCode { get; set; }
        public string DocAccountName { get; set; }
        public decimal ItemAmount { get; set; }
        public decimal DiscountPer { get; set; }
        public decimal DiscountAmt { get; set; }
        //public int StoreId { get; set; }
        // public string StoreName { get; set; }
        public string ItemSize { get; set; } = string.Empty;
        public string ItemDescription { get; set; } = string.Empty;
        public string Remark { get; set; } = string.Empty;
        public string hdnuniquekey { get; set; }
        public DbCrModel DRCRGrid { get; set; }
    }
    public class AccSupplSaleBillAgainstBillDetail : TaxModel
    {
        public int DocAccountCode { get; set; }
        public int CheckBoxNo { get; set; }
        public string SupplSaleBillInvoiceNo { get; set; } = string.Empty;
        //public string ?SupplSaleBillVoucherNo { get; set; } = string.Empty;
        public string? InvoiceNo { get; set; } = string.Empty;
        public string? InvoiceDate { get; set; } = string.Empty;
        public string? AgainstSaleBillBillNo { get; set; }
        public int? AgainstSaleBillYearCode { get; set; }
        public string? AgainstSaleBillDate { get; set; }
        public int? AgainstSaleBillEntryId { get; set; }
        public string? AgainstSaleBillVoucherNo { get; set; }
        public string? SaleBillType { get; set; }
        public string? AgainstPurchaseBillBillNo { get; set; }
        public int? AgainstPurchaseBillYearCode { get; set; }
        public string? AgainstPurchaseBillDate { get; set; }
        public int? AgainstPurchaseBillEntryId { get; set; }
        public string? AgainstPurchaseVoucherNo { get; set; }
        public string? PurchaseBillType { get; set; }
        public int ItemCode { get; set; }
        public string PartCode { get; set; }
        public string ItemName { get; set; }
        public decimal BillQty { get; set; }
        public string? Unit { get; set; }
        public decimal BillRate { get; set; }
        public decimal DiscountPer { get; set; }
        public decimal DiscountAmt { get; set; }
        public string? ItemSize { get; set; }
        public decimal Amount { get; set; }
        public string? PONO { get; set; }
        public string? PODate { get; set; }
        public int POEntryId { get; set; }
        public int POYearCode { get; set; }
        public decimal PoRate { get; set; }
        public string? PoAmmNo { get; set; }
        public string? SONO { get; set; }
        public int SOYearCode { get; set; }
        public string? SODate { get; set; }

        public string? CustOrderNo { get; set; }
        public int SOEntryId { get; set; }
        public string? BatchNo { get; set; }
        public string? UniqueBatchNo { get; set; }
        public decimal AltQty { get; set; }
        public string? AltUnit { get; set; }
        [Column(TypeName = "decimal(10, 4)")]
        public decimal BillAmount { get; set; }
        [Column(TypeName = "decimal(10, 4)")]
        public decimal PaidAmt { get; set; }
        [Column(TypeName = "decimal(10, 4)")]
        public decimal RemainingAmt { get; set; }

        [Column(TypeName = "decimal(10, 4)")]
        public decimal ItemNetAmount { get; set; }

        [Column(TypeName = "decimal(10, 4)")]
        public decimal NetTotal { get; set; }

        [Column(TypeName = "decimal(10, 4)")]
        public decimal TotalAmtAftrDiscount { get; set; }

        [Column(TypeName = "decimal(10, 4)")]
        public decimal TotalDiscountPercentage { get; set; }

        public string? TotalRoundOff { get; set; }
        public string? RDT { get; set; }
        public string? AttachmentFile1 { get; set; }
        [Column(TypeName = "decimal(10, 4)")]
        public decimal TotalRoundOffAmt { get; set; }
        public int RoundOffAccountCode { get; set; }
    }

    public class AccSupplSaleBillDashboard : AccSupplSaleBillModel
    {
        public string? FromDate1 { get; set; }
        public string? ToDate1 { get; set; }
        public string? Searchbox { get; set; }
        public string SummaryDetail { get; set; }
        public List<AccSupplSaleBillDashboard> SupplSaleBillDashboard { get; set; }

    }
}
