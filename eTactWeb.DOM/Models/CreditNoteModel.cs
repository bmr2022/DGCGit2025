using System.ComponentModel.DataAnnotations.Schema;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.DOM.Models
{
    public class AccCreditNoteModel : AccCreditNoteDetail
    {
        public int CreditNoteEntryId { get; set; }
        public int CreditNoteYearCode { get; set; }
        public string CreditNoteInvoiceNo { get; set; } = string.Empty;
        public string CreditNoteInvoiceDate { get; set; }
        public string SubVoucherName { get; set; } = string.Empty;
        public string CreditNoteVoucherNo { get; set; } = string.Empty;
        public string CreditNoteVoucherDate { get; set; }
        public string AgainstSalePurchase { get; set; } = string.Empty;
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
        public string GstRegUnreg { get; set; } = string.Empty;
        public string Transporter { get; set; } = string.Empty;
        public string Vehicleno { get; set; } = string.Empty;
        public decimal BillAmt { get; set; }
        public decimal RoundOffAmt { get; set; }
        public string RoundoffType { get; set; } = string.Empty;
        public decimal Taxableamt { get; set; }
        public decimal ToatlDiscountPercent { get; set; }
        public decimal TotalDiscountAmount { get; set; }
        public decimal NetAmt { get; set; }
        public string Remark { get; set; } = string.Empty;
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
        public string? EinvGenerated { get; set; }
        public string AttachmentFilePath1 { get; set; } = string.Empty;
        public string AttachmentFilePath2 { get; set; } = string.Empty;
        public string AttachmentFilePath3 { get; set; } = string.Empty;
        public string FinFromDate { get; set; }
        public string FinToDate { get; set; }
        public int PageNumber {  get; set; }
        public int TotalRecords {  get; set; }
        public int PageSize { get; set; }
        public string DashboardTypeBack {  get; set; }
        public string FromDateBack {  get; set; }
        public string ToDateBack { get; set; }
        public int AccountCodeBack {  get; set; }
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
        public List<AccCreditNoteDetail> AccCreditNoteDetails { get; set; }
        public List<AccCreditNoteAgainstBillDetail> AccCreditNoteAgainstBillDetails { get; set; }
        public IList<AccCreditNoteDetail>? ItemDetailGrid { get; set; }
    }

    public class AccCreditNoteDetail : AccCreditNoteAgainstBillDetail
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
        public float BillQty { get; set; }
        public float RejectedQty { get; set; }
        public string Unit { get; set; } = string.Empty;
        public float AltQty { get; set; }
        public string AltUnit { get; set; } = string.Empty;
        public float CRNRate { get; set; }
        public float BillRate { get; set; }
        public string UnitRate { get; set; } = string.Empty;
        public float AltRate { get; set; }
        public float NoOfCase { get; set; }
        public int CostCenterId { get; set; }
        public int DocAccountCode { get; set; }
        public string DocAccountName { get; set; }
        public decimal ItemAmount { get; set; }
        public float DiscountPer { get; set; }
        public float DiscountAmt { get; set; }
        public int StoreId { get; set; }
        public string StoreName { get; set; }
        public string ItemSize { get; set; } = string.Empty;
        public string ItemDescription { get; set; } = string.Empty;
        public string Remark { get; set; } = string.Empty;
        public DbCrModel DRCRGrid { get; set; }
    }
    public class AccCreditNoteAgainstBillDetail : TaxModel
    {
        public int CheckBoxNo { get; set; }
        public string CreditNoteInvoiceNo { get; set; } = string.Empty;
        public string CreditNoteVoucherNo { get; set; } = string.Empty;
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
        public float BillQty { get; set; }
        public string? Unit { get; set; }
        public float BillRate { get; set; }
        public float DiscountPer { get; set; }
        public float DiscountAmt { get; set; }
        public string? ItemSize { get; set; }
        public decimal Amount { get; set; }
        public string? PONO { get; set; }
        public string? PODate { get; set; }
        public int POEntryId { get; set; }
        public int POYearCode { get; set; }
        public float PoRate { get; set; }
        public string? PoAmmNo { get; set; }
        public string? SONO { get; set; }
        public int SOYearCode { get; set; }
        public string? SODate { get; set; }
        public string? CustOrderNo { get; set; }
        public int SOEntryId { get; set; }
        public string? BatchNo { get; set; }
        public string? UniqueBatchNo { get; set; }
        public float AltQty { get; set; }
        public string? AltUnit { get; set; }

        [Column(TypeName = "decimal(10, 4)")]
        public decimal ItemNetAmount { get; set; }

        [Column(TypeName = "decimal(10, 4)")]
        public decimal NetTotal { get; set; }

        [Column(TypeName = "decimal(10, 4)")]
        public decimal TotalAmtAftrDiscount { get; set; }

        [Column(TypeName = "decimal(10, 4)")]
        public decimal TotalDiscountPercentage { get; set; }

        public string? TotalRoundOff { get; set; }
        [Column(TypeName = "decimal(10, 4)")]
        public decimal TotalRoundOffAmt { get; set; }
    }

    public class AccCreditNoteDashboard : AccCreditNoteModel
    {
        public string? FromDate1 { get; set; }
        public string? ToDate1 { get; set; }
        public string? Searchbox { get; set; }
        public string SummaryDetail { get; set; }
        public List<AccCreditNoteDashboard> CreditNoteDashboard { get; set; }

    }

}
