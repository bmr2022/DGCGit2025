using System.ComponentModel.DataAnnotations.Schema;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.DOM.Models
{
    public class AccPurchaseRejectionModel : AccPurchaseRejectionDetail
    {
        public int PurchaseRejEntryId { get; set; }
        public int PurchaseRejYearCode { get; set; }
        public string SubVoucherName { get; set; } = string.Empty;
        public string DebitNotePurchaseRejection { get; set; } = string.Empty;
        public string PurchaseRejEntryDate { get; set; }
        public string PurchaseRejectionVoucherNo { get; set; } = string.Empty;
        public string VoucherNo { get; set; } = string.Empty;
        public string PurchaseRejectionInvoiceNo { get; set; } = string.Empty;
        public string PurchaseRejectionInvoiceDate { get; set; }
        public string PurchaseRejectionInvoiceTime { get; set; }
        public int AccountCode { get; set; }
        public string AccountName { get; set; }
        public string CustVendAddress { get; set; } = string.Empty;
        public string StateNameofSupply { get; set; } = string.Empty;
        public string StateCode { get; set; } = string.Empty;
        public string CityofSupply { get; set; } = string.Empty;
        public string CountryOfSupply { get; set; } = string.Empty;
        public int? CurrencyId { get; set; }
        public string Currency { get; set; }
        public IList<TextValue>? CurrencyList { get; set; }
        public string ExchangeRate { get; set; } = string.Empty;
        public string PaymentTerm { get; set; } = string.Empty;
        public string GSTNO { get; set; } = string.Empty;
        public string DomesticExportNEPZ { get; set; } = string.Empty;
        public string Transporter { get; set; } = string.Empty;
        public string Vehicleno { get; set; } = string.Empty;
        public string Distance { get; set; } = string.Empty;
        public float BillAmt { get; set; }
        public float RoundOffAmt { get; set; }
        public string RoundoffType { get; set; } = string.Empty;
        public float Taxableamt { get; set; }
        public float ToatlDiscountPercent { get; set; }
        public float TotalDiscountAmount { get; set; }
        public float NetAmt { get; set; }
        //public float InvNetAmt { get; set; } //uncomment if needed afterwards for pr
        public string PurchaserejRemark { get; set; } = string.Empty;
        public string CC { get; set; } = string.Empty;
        public int Uid { get; set; }
        public int EntryByempId { get; set; }
        public string MachineName { get; set; } = string.Empty;
        public string ActualEntryDate { get; set; }
        public int ActualEnteredBy { get; set; }
        public string ActualEnteredByName { get; set; }
        public int? LastUpdatedBy { get; set; }
        public string LastUpdatedByName { get; set; }
        public string? LastUpdationDate { get; set; }
        public string? BalanceSheetClosed { get; set; }

        public string PurchaseRejectionVoucherDate { get; set; }
        public int PaymentCreditDay { get; set; }
        
        //below are newly added
        public string GSTRegistered { get; set; }
        public string GSTType { get; set; }
        public string? Batchno { get; set; }
        public string? Uniquebatchno { get; set; }
        public float LotStock { get; set; }
        public float TotalStock { get; set; }
        public string fromBillDate { get; set; }
        public string toBillDate { get; set; }
        //below are defaults
        public IFormFile? PathOfFile1 { get; set; }
        public string? PathOfFile1URL { get; set; }
        public IFormFile? PathOfFile2 { get; set; }
        public string? PathOfFile2URL { get; set; }
        public IFormFile? PathOfFile3 { get; set; }
        public string? PathOfFile3URL { get; set; }
        public string FinFromDate { get; set; }
        public string FinToDate { get; set; }

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
        public string FromDateBack { get; set; }
        public string ToDateBack { get; set; }
        public string VendorNameBack { get; set; }
        public string VoucherNoBack { get; set; }
        public string InvoiceNoBack { get; set; }
        public string PartCodeBack { get; set; }
        public string GlobalSearchBack { get; set; }
        public List<AccPurchaseRejectionDetail> AccPurchaseRejectionDetails { get; set; }
        public List<AccPurchaseRejectionAgainstBillDetail> AccPurchaseRejectionAgainstBillDetails { get; set; }
        public IList<AccPurchaseRejectionDetail>? ItemDetailGrid { get; set; }
        public IList<DbCrModel>? DbCrGrid { get; set; }
    }
    public class AccPurchaseRejectionDetail : AccPurchaseRejectionAgainstBillDetail
    {
        public AdjustmentModel adjustmentModel { get; set; }
        public int SeqNo { get; set; }
        public int ItemCode { get; set; }
        public bool ItemSA { get; set; }
        public bool DA { get; set; }
        public bool SA { get; set; }
        public bool ShowAllBill { get; set; }
        public string ItemName { get; set; }
        public string PartCode { get; set; }
        public string HSNNo { get; set; }
        public float BillQty { get; set; }
        public float RejectedQty { get; set; }
        public string Unit { get; set; } = string.Empty;
        public float AltQty { get; set; }
        public string AltUnit { get; set; } = string.Empty;
        public float PRRate { get; set; }
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
        public string MIRNo { get; set; } = string.Empty;
        public int MIREntryID { get; set; }
        public int MIRYearCode { get; set; }
        public string MIRDate { get; set; } = string.Empty;

        public string InprocessNo { get; set; } = string.Empty;
        public int InprocessEntryID { get; set; }
        public int InprocessYearCode { get; set; }
        public string InprocessDate { get; set; } = string.Empty;
        public string? Batchno { get; set; } = string.Empty;
        public string? Uniquebatchno { get; set; } = string.Empty;
        public float LotStock { get; set; }
        public float TotalStock { get; set; }
        public string hdnuniquekey { get; set; }
    }
    public class AccPurchaseRejectionAgainstBillDetail : TaxModel
    {
        public int CheckBoxNo { get; set; }
        public string PurchaseRejectionInvoiceNo { get; set; } = string.Empty;
        public string? InvoiceNo { get; set; } = string.Empty;
        public string? InvoiceDate { get; set; } = string.Empty;
        public string PurchaseRejectionVoucherNo { get; set; } = string.Empty;
        public string? AgainstPurchaseBillBillNo { get; set; }
        public int? AgainstPurchaseBillYearCode { get; set; }
        public string? AgainstPurchaseBillDate { get; set; }
        public int? AgainstPurchaseBillEntryId { get; set; }
        public string? AgainstPurchaseVoucherNo { get; set; }
        public string? PurchaseBillType { get; set; }
        public int ItemCode { get; set; }
        public int PurchBillItemCode { get; set; }
        public string PartCode { get; set; }
        public string ItemName { get; set; }
        public float? BillQty { get; set; }
        public string? Unit { get; set; }
        public float? BillRate { get; set; }
        public float? DiscountPer { get; set; }
        public float? DiscountAmt { get; set; }
        public string? ItemSize { get; set; }
        public decimal? Amount { get; set; }
        public string? PONO { get; set; }
        public string? PODate { get; set; }
        public int? POEntryId { get; set; }
        public int? POYearCode { get; set; }
        public float? PoRate { get; set; }
        public string? PoAmmNo { get; set; }
        public string? BatchNo { get; set; }
        public string? UniqueBatchNo { get; set; }
        public float? AltQty { get; set; }
        public string? AltUnit { get; set; }
        [Column(TypeName = "decimal(10, 4)")]
        public decimal BillAmount { get; set; }
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
    public class AccPurchaseRejectionDashboard : AccPurchaseRejectionModel
    {
        public string? SummaryDetail { get; set; }
        public string? FromDate { get; set; }
        public string? ToDate { get; set; }
        public string? FromDate1 { get; set; }
        public string? ToDate1 { get; set; }
        public string? Searchbox { get; set; }
        public IList<TextValue>? VoucherNoList { get; set; }
        public IList<TextValue>? InvoiceNoList { get; set; }
        public IList<TextValue>? VendorNameList { get; set; }
        public IList<TextValue>? PartCodeList { get; set; }
        //public IList<TextValue>? ItemNameList { get; set; }
        public string? DashboardType { get; set; }

        //pagintion
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalRecords { get; set; }


        //Dashboard Grid
        public IList<AccPurchaseRejectionDashboard> PurchaseRejectionDashboard { get; set; }
        public int? PurchaseRejEntryId { get; set; }
        public int? SeqNo { get; set; }
        public string? ActualEntryByEmpName { get; set; }
        public string? UpdatedByEmpName { get; set; }

        public string? PurchaseRejEntryDate { get; set; }
        public DateTime? ActualEntryDate { get; set; }
        public string? DebitNotePurchaseRejection { get; set; }
        public string? InvoiceNo { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public string? PurchaserejVoucherNo { get; set; }
        public string? VoucherNo { get; set; }
        //public string? VoucherDate { get; set; }
        public string? VendorName { get; set; }
        public string? VendoreAddress { get; set; }
        public string? StateName { get; set; }
        public int? StateCode { get; set; }
        public string? CreditDays { get; set; }
        public int? PurchaseRejYearCode { get; set; }
        public string? PurchaserejRemark { get; set; }
        public string? PaymentTerm { get; set; }
        public string? Transporter { get; set; }
        public string? Vehicleno { get; set; }
        public string? Distance { get; set; }
        public string? SubVoucherName { get; set; }
        public string? DomesticExportNEPZ { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public string? Currency { get; set; }
        public string? ExchangeRate { get; set; }
        public float? BillAmt { get; set; }
        public float? Taxableamt { get; set; }
        public float? InvNetAmt { get; set; }
        public float? RoundOffAmt { get; set; }
        public string? RoundoffType { get; set; }
        public string? BalanceSheetClosed { get; set; }
        public string? LastUpdationDate { get; set; }
        public string? MachineName { get; set; }
        public string? CC { get; set; }
        public string? Uid { get; set; }

        //Detail List
        public string? PartCode { get; set; }
        public string? ItemName { get; set; }
        public string? HSNNO { get; set; }
        public string? Unit { get; set; }
        public string? NoOfCase { get; set; }
        public string BillQty { get; set; }
        public string RecQty { get; set; }
        public string? Rate { get; set; }
        public string? PORate { get; set; }
        public string? DiscountPer { get; set; }
        public string? DiscountAmt { get; set; }
        //[Column(TypeName = "decimal(10, 4)")]

        public decimal? Amount { get; set; }
        public string? PONo { get; set; }
        public string? PODate { get; set; }
        public string? POType { get; set; }
        public string? SchNo { get; set; }
        public string? SchDate { get; set; }
        public string? ItemSize { get; set; }
        public string? OtherDetail { get; set; }
        public string? MRP { get; set; }
        public string? RateUnit { get; set; }
        public string? RateIncludingTaxes { get; set; }
        public string? AmtinOtherCurr { get; set; }
        public string? RateConversionFactor { get; set; }
        public string? CostCenterName { get; set; }
        public string? ItemColor { get; set; }
        public string? ItemModel { get; set; }
        public string? POYearCode { get; set; }
        public string? SchYearCode { get; set; }
        public string? POAmmNo { get; set; }
        public string? PoRate { get; set; }
        public string? ProjectNo { get; set; }
        public string? ProjectDate { get; set; }
        public string? ProjectYearCode { get; set; }
        public string? AgainstImportAccountCode { get; set; }
        public string? AgainstImportInvoiceNo { get; set; }
        public string? AgainstImportYearCode { get; set; }
        public string? AgainstImportInvDate { get; set; }
        public string? PurchBillEntryId { get; set; }
        public int? PurchBillYearCode { get; set; }
        public string? ItemOrService { get; set; }
        public string? PurchaseBillDirectPB { get; set; }
        public string? ExpenseHead { get; set; }
        public decimal? ExpenseAmt { get; set; }
        //Tax detail
        public string? CGSTHead { get; set; }
        public decimal? CGSTper { get; set; }
        public decimal? CGSTAmt { get; set; }
        public string? SGSTHead { get; set; }
        public decimal? SGSTper { get; set; }
        public decimal? SGSTAmt { get; set; }
        public string? IGSTHead { get; set; }
        public decimal? IGSTper { get; set; }
        public decimal? IGSTAmt { get; set; }
    }
}
