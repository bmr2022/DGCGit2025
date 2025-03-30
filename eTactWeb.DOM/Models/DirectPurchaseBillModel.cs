using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Numerics;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.DOM.Models;

/// <inheritdoc/>

[Serializable()]
public class DPBDashBoard : TimeStamp
{
    public IList<DPBDashBoard> DPBDashboard { get; set; }
    public string? CC { get; set; }
    public string? ApprovedDate { get; set; }

    public string? Approved { get; set; }
    public string? ApproveAmm { get; set; }
    public string? Approval1Levelapproved { get; set; }
    public string? Currency { get; set; }
    public string? DeliveryDate { get; set; }
    public int EntryID { get; set; }
    public string? FOC { get; set; }
    public string? FromDate { get; set; }
    public string? ItemName { get; set; }
    public string? PartCode { get; set; }
    public string? PaymentTerms { get; set; }
    public string? FromDate1 { get; set; }
    public string? ToDate1 { get; set; }
    public string? DPBDate { get; set; }
    public IList<TextValue>? PONoList { get; set; }
    public string? DPNTypeServItem { get; set; }
    public string? ToDate { get; set; }
    public string? VendorAddress { get; set; }
    public string? VendorName { get; set; }
    public int YearCode { get; set; }
    public string? StateName { get; set; }
    public string? GSTType { get; set; }
    public string? TypeITEMSERVASSETS { get; set; }
    public string? DomesticImport { get; set; }
    public string? Transporter { get; set; }
    public string? VehicleNo { get; set; }
    public string? ModeOfTrans { get; set; }
    public string? VoucherDate { get; set; }

    public string? PurchVouchNo { get; set; }
    public string? InvoiceNo { get; set; }
    public string? DocumentType { get; set; }
    public IList<TextValue>? DocumentList { get; set; }
    public decimal DPBAmt { get; set; }
    public decimal DPBNetAmt { get; set; }
    public string? EnteredBy { get; set; }
    public DateTime? EntryDate { get; set; }
    public DateTime? InvoiceDate { get; set; }
    public string? UpdatedByName { get; set; }
    public string? DashboardType { get; set; }
    public float BasicAmount { get; set; }
    public float NetAmount { get; set; }
    public float GSTAmount { get; set; }
    public float TaxableAmount { get; set; }
    public float TotalDiscPer { get; set; }
    public float TDSAmount { get; set; }
    public float TotalAmtInOtherCurr { get; set; }
    public string? POYearCode { get; set; }
    public string? SchYearCode { get; set; }
    public string? ApprovedBy { get; set; }
    public string? TaxVariationPOvsBill { get; set; }
    public string? BlockRateVariation { get; set; }
    public string? BlockReworkDebitNote { get; set; }
    public string? BOEDate { get; set; }
    public string? PONetAmt { get; set; }
    public string? HsnNo { get; set; }
    public string? PONo { get; set; }
    public string? PODate { get; set; }
    public string? SchNo { get; set; }
    public string? SchDate { get; set; }
    public float DPBQty { get; set; }
    public float BillQty { get; set; }
    public string? Unit { get; set; }
    public float AltQty { get; set; }
    public string? AltUnit { get; set; }
    public string? Rate { get; set; }
    public string? RateInOtherCurr { get; set; }
    public float DiscPer { get; set; }
    public float DiscRs { get; set; }

    [Column(TypeName = "decimal(10, 4)")]
    public decimal Amount { get; set; }
    public float PendQty { get; set; }
    public float PendAltQty { get; set; }
    public string? UnitRate { get; set; }
    public string?  SummaryDetail { get; set; }
    //public int EntryId { get; set; }
    public int SeqNo { get; set; }
    public int ItemCode { get; set; }
    public int HSNNo { get; set; }
    public string? Remark { get; set; }
    public string? Description { get; set; }
    public int Process { get; set; }
    public string? SizeDetail { get; set; }
    public string? Colour { get; set; }
    public int CostCenter { get; set; }
    public string? RateApplicableOnUnit { get; set; }
    public float MRP { get; set; }
    public float RateUnit { get; set; }
    public float RateIncludingTaxes { get; set; }
    public float AmtinOtherCurr { get; set; }
    public float RateConversionFactor { get; set; }
    public string? Commodity { get; set; }
    public string? BalanceSheetClosed { get; set; }
    public string? PORemarks { get; set; }
    public string? CostCenterName { get; set; }
    public string? ItemColor { get; set; }
    public string? ItemModel { get; set; }
    public string? POAmmNo { get; set; }
    public float? PoRate { get; set; }
    public string? POType { get; set; }
    public string? ProjectNo { get; set; }
    public string? ProjectDate { get; set; }
    public string? ProjectYearCode { get; set; }
    public string? AgainstImportAccountCode { get; set; }
    public string? AgainstImportInvoiceNo { get; set; }
    public string? AgainstImportYearCode { get; set; }
    public string? AgainstImportInvDate { get; set; }
    public string? ExpenseHead { get; set; }
    public decimal? ExpenseAmt { get; set; }
    public string? CGSTHead { get; set; }
    public decimal? CGSTper { get; set; }
    public decimal? CGSTAmt { get; set; }
    public string? SGSTHead { get; set; }
    public decimal? SGSTper { get; set; }
    public decimal? SGSTAmt { get; set; }
    public string? IGSTHead { get; set; }
    public decimal? IGSTper { get; set; }
    public decimal? IGSTAmt { get; set; }
    public bool? IsFormFromDelete { get; set; }
    public string? AgainstInvNo { get; set; }
    public string? AgainstVoucherNo { get; set; }
    public DateTime? AgainstVoucherDate { get; set; }
}

[Serializable()]
public class DPBItemDetail : TaxModel, ITDSModel
{
    public AdjustmentModel? adjustmentModel { get; set; }
    #region Inherit TDS
    private readonly TDSModel _tdsModel = new TDSModel();
    public float AcceptedQty { get; set; }

    public float ReworkQty { get; set; }

    public float HoldQty { get; set; }

    // Implement ITDSModel properties using the TDSModel instance
    public TDSModel TDSModel
    {
        get { return _tdsModel; }
    }
    public IList<TDSModel>? TDSDetailGridd
    {
        get { return _tdsModel.TDSDetailGridd; }
        set { _tdsModel.TDSDetailGridd = value; }
    }
    public decimal TotalTDSAmt
    {
        get { return _tdsModel.TotalTDSAmt; }
        set { _tdsModel.TotalTDSAmt = value; }
    }
    public int TDSAccountCode
    {
        get { return _tdsModel.TDSAccountCode; }
        set { _tdsModel.TDSAccountCode = value; }
    }
    public string? TDSAccountName
    {
        get { return _tdsModel.TDSAccountName; }
        set { _tdsModel.TDSAccountName = value; }
    }
    public decimal TDSAmount
    {
        get { return _tdsModel.TDSAmount; }
        set { _tdsModel.TDSAmount = value; }
    }
    public decimal TDSPercentg
    {
        get { return _tdsModel.TDSPercentg; }
        set { _tdsModel.TDSPercentg = value; }
    }
    public string? TDSRemark
    {
        get { return _tdsModel.TDSRemark; }
        set { _tdsModel.TDSRemark = value; }
    }
    public string? TDSRoundOff
    {
        get { return _tdsModel.TDSRoundOff; }
        set { _tdsModel.TDSRoundOff = value; }
    }
    public decimal? TDSRoundOffAmt
    {
        get { return _tdsModel.TDSRoundOffAmt; }
        set { _tdsModel.TDSRoundOffAmt = value; }
    }
    public int TDSSeqNo
    {
        get { return _tdsModel.TDSSeqNo; }
        set { _tdsModel.TDSSeqNo = value; }
    }
    public int TDSTaxType
    {
        get { return _tdsModel.TDSTaxType; }
        set { _tdsModel.TDSTaxType = value; }
    }
    public string? TDSTaxTypeName
    {
        get { return _tdsModel.TDSTaxTypeName; }
        set { _tdsModel.TDSTaxTypeName = value; }
    }
    public IList<SelectListItem> TdsYesNo
    {
        get { return _tdsModel.TdsYesNo; }
        set { _tdsModel.TdsYesNo = value; }
    }
    #endregion

    #region Implement Timestamp properties using the TDSModel instance
    //public string? Active
    //{
    //    get { return _tdsModel.Active; }
    //    set { _tdsModel.Active = value; }
    //}
    //public int CreatedBy
    //{
    //    get { return _tdsModel.CreatedBy; }
    //    set { _tdsModel.CreatedBy = value; }
    //}
    //public DateTime? CreatedOn
    //{
    //    get { return _tdsModel.CreatedOn; }
    //    set { _tdsModel.CreatedOn = value; }
    //}
    //public string? EID
    //{
    //    get { return _tdsModel.EID; }
    //    set { _tdsModel.EID = value; }
    //}
    //public int ID
    //{
    //    get { return _tdsModel.ID; }
    //    set { _tdsModel.ID = value; }
    //}
    public string? TDSMode
    {
        get { return _tdsModel.Mode; }
        set { _tdsModel.Mode = value; }
    }
    //public string TxPageName
    //{
    //    get { return _tdsModel.TxPageName; }
    //    set { _tdsModel.TxPageName = value; }
    //}
    //public int UpdatedBy
    //{
    //    get { return _tdsModel.UpdatedBy; }
    //    set { _tdsModel.UpdatedBy = value; }
    //}
    //public DateTime? UpdatedOn
    //{
    //    get { return _tdsModel.UpdatedOn; }
    //    set { _tdsModel.UpdatedOn = value; }
    //}
    #endregion

    private IList<SelectListItem> _UnitRate = new List<SelectListItem>()
    {
        new() { Value = "Unit", Text = "Unit" },
        new() { Value = "AltUnit", Text = "AltUnit" }
    };

    [Column(TypeName = "decimal(10, 4)")]
    public decimal AdditionalRate { get; set; }

    [Column(TypeName = "decimal(10, 4)")]
    public decimal AltPendQty { get; set; }

    [Column(TypeName = "decimal(10, 4)")]
    public decimal AltQty { get; set; }

    public string? AltUnit { get; set; }

    [Column(TypeName = "decimal(10, 4)")]
    public decimal Amount { get; set; }

    public string? Color { get; set; }
    public int CostCenter { get; set; }
    public string? CostCenterName { get; set; }
    public IList<TextValue>? CostCenterList { get; set; }

    //public IList<DeliverySchedule>? DeliveryScheduleList { get; set; }

    public string? Description { get; set; }

    [Column(TypeName = "decimal(10, 4)")]
    [Range(0, 100, ErrorMessage = "Should ve in between 0-100")]
    public decimal DiscPer { get; set; }

    [Column(TypeName = "decimal(10, 4)")]
    public decimal DiscRs { get; set; }
    public bool DT1 { get; set; }
    public int? docTypeId { get; set; }
    public IList<TextValue>? DocTypeList { get; set; }
    public string? DocTypeText { get; set; }
    public string? PONo { get; set; }
    public int? POYear { get; set; }
    public string? PODate { get; set; }
    public string? ScheduleNo { get; set; }
    public int? ScheduleYear { get; set; }
    public string? ScheduleDate { get; set; }
    [Column(TypeName = "decimal(10, 4)")]
    public decimal BillQty { get; set; }
    public int HSNNo { get; set; }
    public bool IN1 { get; set; }
    public int? ItemCode { get; set; }
    public IList<TextValue>? ItemNameList { get; set; }

    [Column(TypeName = "decimal(10, 4)")]
    public decimal ItemNetAmount { get; set; }

    public string? ItemText { get; set; }

    [Column(TypeName = "decimal(10, 4)")]
    public decimal OtherRateCurr { get; set; }

    public int PartCode { get; set; }
    public IList<TextValue>? PartCodeList { get; set; }
    public string? PartText { get; set; }


    [Column(TypeName = "decimal(10, 4)")]
    public decimal DPBQty { get; set; }

    public int Process { get; set; }
    public string? ProcessName { get; set; }
    public IList<TextValue>? ProcessList { get; set; }

    [Column(TypeName = "decimal(10, 4)")]
    public decimal Rate { get; set; }

    public int SeqNo { get; set; }
    public bool SH { get; set; }

    [Column(TypeName = "decimal(10, 4)")]

    public string? Unit { get; set; }
    public string? UnitRate { get; set; }

    public IList<SelectListItem> UnitRateList
    {
        get => _UnitRate;
        set => _UnitRate = value;
    }
}
public class DPBFormDataModel
{
    public string?  PurchVouchNo { get; set; }
    public int POYearCode { get; set; }
    public int AccountCode { get; set; }
    public string?  SchNo { get; set; }
    public int SchYearCode { get; set; }
    public IFormFile?  excelFile { get; set; }
}
[Serializable()]
public class DirectPurchaseBillModel : DPBItemDetail
{
 

    private IList<SelectListItem> _DPBTypeList = new List<SelectListItem>()
        {
            new() { Value = "Domestic", Text = "Domestic" },
            new() { Value = "Import", Text = "Import" },
        };


    private IList<SelectListItem> _DPBTypeServList = new List<SelectListItem>()
        {
            new() { Value = "I", Text = "Item" },
            new() { Value = "S", Text = "Service" },
            new() { Value = "A", Text = "Assets" },
        };

    private IList<SelectListItem> _YesNo = new List<SelectListItem>()
        {
            new() { Value = "Y", Text = "Yes" },
            new() { Value = "N", Text = "No" },
        };

    //just for display
    public string? UniversalPartCode { get; set; }
    public string? UniversalDescription { get; set; }

    public string ? TextBox1Value { get; set; }
    public string? TextBox2Value { get; set; }
    public string? FinFromDate { get; set; }
    public string? FinToDate { get; set; }
    public string? TypeOfSave { get; set; }
    public int AccountCode { get; set; }
    public IList<TextValue>? AccountList { get; set; }

    public string? Branch { get; set; }
    public IList<TextValue>? BranchList { get; set; }

    [Column(TypeName = "decimal(10, 4)")]
    public decimal Currency { get; set; }

    public IList<TextValue>? CurrencyList { get; set; }
    public IList<TextValue>? DepartmentList { get; set; }
    //public int? docTypeId { get; set; }
    public IList<TextValue>? DocumentList { get; set; }

    [Column(TypeName = "decimal(10, 4)")]
    public decimal Discount { get; set; }

    //[DataType(DataType.DateTime)]
    //[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true, NullDisplayText = "NULL")]
    public string? EntryDate { get; set; }

    public int EntryID { get; set; }
    public string? GSTIncludedONRate { get; set; }
    public IList<DPBItemDetail>? ItemDetailGrid { get; set; }
    //public int? CreatedBy { get; set; }
    //public DateTime? CreatedOn { get; set; }
    public string? CretaedByName { get; set; }
    //public int? UpdatedBy { get; set; }
    //public DateTime? UpdatedOn { get; set; }
    public string? UpdatedByName { get; set; }
    
    [Column(TypeName = "decimal(10, 4)")]
    public decimal NetTotal { get; set; }

    public int OrderNo { get; set; }
    public IFormFile? PathOfFile1 { get; set; }
    public string? PathOfFile1URL { get; set; }
    public IFormFile? PathOfFile2 { get; set; }
    public string? PathOfFile2URL { get; set; }
    public IFormFile? PathOfFile3 { get; set; }
    public string? PathOfFile3URL { get; set; }
    public int PaymentDays { get; set; }
    public int PaymentPartyAccountCode { get; set; }
    public string? PaymentPartyAccountDetail { get; set; }
    public string? PaymentTerms { get; set; }
    public bool PN1 { get; set; }
    public bool PN2 { get; set; }
    public string? VouchDate { get; set; }
    public int DPBItemCode { get; set; }
    public string? PurchVouchNo { get; set; }
    public string? InvoiceNo { get; set; }
    public string? InvDate { get; set; }
    //public string? PONo { get; set; }
    //public int? POYear { get; set; }
    //public string? PODate { get; set; }
    //public string? ScheduleNo { get; set; }
    //public int? ScheduleYear { get; set; }
    //public string? ScheduleDate { get; set; }
    public string? GstType { get; set; }
    public string? VendorStateName { get; set; }
    public string? Transport { get; set; }
    public string? VehicleNo { get; set; }
    public string? ModeOfTransport { get; set; }
    public float? ExchangeRate { get; set; }
    [Column(TypeName = "decimal(10, 4)")]
   // public decimal BillQty { get; set; }
    public string? EntryByMachineName { get; set; }
    public string? DPBTypeServItem { get; set; }
    public int? ResposibleEmplforQC { get; set; }

    public IList<SelectListItem> DPBTypeServList = new List<SelectListItem>()
    {
        new() { Value = "Service", Text = "Service" },
        new() { Value = "Item", Text = "Item" },
        new() { Value = "Asset", Text = "Asset" },
    };
    public string? DPBType { get; set; }
    public IList<SelectListItem> DPBTypeList
    {
        get => _DPBTypeList;
        set => _DPBTypeList = value;
    }

    public int PreparedBy { get; set; }
    public string? PreparedByName { get; set; }
    public IList<TextValue>? PreparedByList { get; set; }
    public string? Remark { get; set; }

    [Column(TypeName = "decimal(10, 4)")]
    public decimal TotalAmtAftrDiscount { get; set; }

    [Column(TypeName = "decimal(10, 4)")]
    public decimal TotalDiscountPercentage { get; set; }

    public string? TotalRoundOff { get; set; }
    [Column(TypeName = "decimal(10, 4)")]
    public decimal TotalRoundOffAmt { get; set; }
    public string? VendorAddress { get; set; }
    public int YearCode { get; set; }

    public string? Approved { get; set; }
    public string? ApprovedDate { get; set; }
    public int? Approvedby { get; set; }

    public string FromDateBack { get; set; }
    public string ToDateBack { get; set; }
    public string TypeITEMSERVASSETSBack { get; set; }
    public string DocumentTypeBack { get; set; }
    public string DashboardTypeBack { get; set; }
    public string PurchVouchNoBack { get; set; }
    public string InvoiceNoBack { get; set; }
    public string VendorNameBack { get; set; }
    public string PartCodeBack { get; set; }
    public string ItemNameBack { get; set; }
    public string HSNNoBack { get; set; }
    public string GlobalSearchBack { get; set; }

    public IList<SelectListItem> YesNoList
    {
        get => _YesNo;
        set => _YesNo = value;
    }
}