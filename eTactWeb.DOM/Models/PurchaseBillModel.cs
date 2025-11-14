using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.DOM.Models;

/// <inheritdoc/>

[Serializable()]
public class PBDashBoard : TimeStamp
{
    public string? SummaryDetail { get; set; }
    public string? FromDate { get; set; }
    public string? ToDate { get; set; }
    public string? FromDate1 { get; set; }
    public string? ToDate1 { get; set; }
    public string? Searchbox { get; set; }
    public IList<TextValue>? VendorNameList { get; set; }
    public IList<TextValue>? VoucherNoList { get; set; }
    public IList<TextValue>? InvoiceNoList { get; set; } 
    public IList<TextValue>? MRNNoList { get; set; }   
    public IList<TextValue>? GateNoList { get; set; }
    public IList<TextValue>? PartCodeList { get; set; }
    public IList<TextValue>? ItemNameList { get; set; }
    public string? DocumentName { get; set; }
    public IList<TextValue>? DocumentNameList { get; set; }
    public IList<TextValue>? HSNNOList { get; set; }
    public string? DashboardType { get; set; }
    public string? MRNType { get; set; }

    //pagintion
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalRecords { get; set; }


    //Dashboard Grid
    public IList<PBDashBoard> PBDashboard { get; set; }
    public int? EntryID { get; set; }
    public int? SeqNo { get; set; }
    public string? EnteredBy { get; set; }
    public string? UpdatedByName { get; set; }
    
    public DateTime? EntryDate { get; set; }
    public string? InvoiceNo { get; set; }
    public DateTime? InvoiceDate { get; set; }
    public string? VoucherNo { get; set; }
    public string? VoucherDate { get; set; }
    public string? VendorName { get; set; }
    public string? VendorAddress { get; set; }
    public string? StateName { get; set; }
    public string? GSTType { get; set; }
    public string? MRNNo { get; set; }
    public string? MRNDate { get; set; }
    public string? GateNo { get; set; }
    public string? GateDate { get; set; }
    public string? DomesticImport { get; set; }
    public float? BillAmount { get; set; }
    public float? TaxableAmount { get; set; }
    public float?  GSTAmount { get; set; }
    public float? NetAmt { get; set; }
    public float? PendAmt { get; set; }
    public float? PaidAmt { get; set; }
    public string? PaymentTerm { get; set; }
    public string? Transporter { get; set; }
    public string? VehicleNo { get; set; }
    public string? Currency { get; set; }
    public string? ExchangeRate { get; set; }
    public float? RoundOffAmt { get; set; }
    public string? RoundoffType { get; set; }
    public float? TotalDiscountPercent { get; set; }
    public float? PONetAmt { get; set; }
    public float? TDSAmount { get; set; }
    public string? Remark { get; set; }
    public string? Branch { get; set; }
    public string? Approved { get; set; }
    public string? ApprovedDate { get; set; }
    public string? ApprovedBy { get; set; }
    public string? BOEDate { get; set; }
    public string? ModeOfTrans { get; set; }
    public string? TotalAmtInOtherCurr { get; set; }
    public string? boeno { get; set; }
    public string? Commodity { get; set; }
    public string? PathOfFile1 { get; set; }
    public string? PathOfFile2 { get; set; }
    public string? PathOfFile3 { get; set; }
    public string? PathOfFile4 { get; set; }
    public string? PathOfFile5 { get; set; }
    public string? BalanceSheetClosed { get; set; }
    public string? ActualEntryBy { get; set; }
    public string? ActualEntryDate { get; set; }
    public string? LastUpdatedBy { get; set; }
    public string? LastUpdatedDate { get; set; }
    public string? EntryByMachine { get; set; }
    public string? PORemarks { get; set; }
    public string? UID { get; set; }

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
    public string? AgainstVoucherNo { get; set; }
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

[Serializable()]
public class PBItemDetail : TaxModel, ITDSModel
{
    public AdjustmentModel? adjustmentModel { get; set; }
    #region Inherit TDS
    private readonly TDSModel _tdsModel = new TDSModel();

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
    public string? Active
    {
        get { return _tdsModel.Active; }
        set { _tdsModel.Active = value; }
    }
    public int CreatedBy
    {
        get { return _tdsModel.CreatedBy; }
        set { _tdsModel.CreatedBy = value; }
    }
    public DateTime? CreatedOn
    {
        get { return _tdsModel.CreatedOn; }
        set { _tdsModel.CreatedOn = value; }
    }
    public string? EID
    {
        get { return _tdsModel.EID; }
        set { _tdsModel.EID = value; }
    }
    public int ID
    {
        get { return _tdsModel.ID; }
        set { _tdsModel.ID = value; }
    }
    public string? TDSMode
    {
        get { return _tdsModel.Mode; }
        set { _tdsModel.Mode = value; }
    }
    public string TxPageName
    {
        get { return _tdsModel.TxPageName; }
        set { _tdsModel.TxPageName = value; }
    }
    public int? UpdatedBy
    {
        get { return _tdsModel.UpdatedBy; }
        set { _tdsModel.UpdatedBy = value; }
    }
    public DateTime? UpdatedOn
    {
        get { return _tdsModel.UpdatedOn; }
        set { _tdsModel.UpdatedOn = value; }
    }
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

    //[Column(TypeName = "decimal(10, 4)")]
    
    public decimal? Amount { get; set; }

    public string? Color { get; set; }
    public int CostCenter { get; set; }
    public string? CostCenterName { get; set; }
    public IList<TextValue>? CostCenterList { get; set; }

    //public IList<DeliverySchedule>? DeliveryScheduleList { get; set; }

    public string? Description { get; set; }

    public IList<TextValue>? DocTypeList { get; set; }
    public string? DocTypeText { get; set; }
    public bool IN1 { get; set; }
    public IList<TextValue>? ItemNameList { get; set; }

    [Column(TypeName = "decimal(10, 4)")]
    public decimal? ItemNetAmount { get; set; }
    
    [Column(TypeName = "decimal(10, 4)")]
    public decimal? GSTAmount { get; set; }
    
    [Column(TypeName = "decimal(10, 4)")]
    public decimal? Taxableamt { get; set; }

    public string? ItemText { get; set; }

    [Column(TypeName = "decimal(10, 4)")]
    public decimal OtherRateCurr { get; set; }
    public IList<TextValue>? PartCodeList { get; set; }
    public string? PartText { get; set; }
    public string? TaxVariationPOvsBill { get; set; }

    [Column(TypeName = "decimal(10, 4)")]
    public decimal PBQty { get; set; }

    public int Process { get; set; }
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

    #region Item detail grid
    public string? ParentCode { get; set; }
    public string? DocumentName { get; set; }
    public int? DocTypeID { get; set; }
    public int? ItemCode { get; set; }
    public string? Item_Name { get; set; }
    public string? PartCode { get; set; }
    public int? HSNNO { get; set; }
    [Column(TypeName = "decimal(10, 4)")]
    public decimal? BillQty { get; set; }
    [Column(TypeName = "decimal(10, 4)")]
    public decimal? RecQty { get; set; }
    [Column(TypeName = "decimal(10, 4)")]
    public decimal? AcceptedQty { get; set; }
    [Column(TypeName = "decimal(10, 4)")]
    public decimal? rejectedQty { get; set; }
    [Column(TypeName = "decimal(10, 4)")]
    public decimal? ReworkQty { get; set; }
    [Column(TypeName = "decimal(10, 4)")]
    public decimal? HoldQty { get; set; }
    public string? ItemLocation { get; set; }

    public int? NoOfCase { get; set; }
    [Column(TypeName = "decimal(10, 4)")]
    public decimal? AltRecQty { get; set; }
    public string? AltUnit { get; set; }
    [Column(TypeName = "decimal(10, 4)")]
    public decimal? BillRate { get; set; }
    [Column(TypeName = "decimal(10, 4)")]
    public decimal? MRP { get; set; }
    [Column(TypeName = "decimal(10, 4)")]
    public decimal? RateIncludingTax { get; set; }
    [Column(TypeName = "decimal(10, 4)")]
    public decimal? AmtInOtherCurrency { get; set; }
    public int? RateOfConvFactor { get; set; }
    [Column(TypeName = "decimal(10, 4)")]
    public decimal? AssessRate { get; set; }
    [Column(TypeName = "decimal(10, 4)")]
    [Range(0, 100, ErrorMessage = "Should ve in between 0-100")]
    public decimal? DisPer { get; set; }
    [Column(TypeName = "decimal(10, 4)")]
    public decimal? DisAmt { get; set; }
    public string? ItemSize { get; set; }
    public string? ItemColor { get; set; }
    public string? ItemModel { get; set; }
    public int? DepartmentId { get; set; }
    public string? ForDepartment { get; set; }
    public int? ProcessId { get; set; }
    public string? ProcessName { get; set; }
    [Column(TypeName = "decimal(10, 4)")]
    public decimal? NewPoRate { get; set; }
    public string? pono { get; set; }
    public int? poyearcode { get; set; }
    public string? PODate { get; set; }
    public string? schno { get; set; }
    public int? schyearcode { get; set; }
    public string? SchDate { get; set; }
    public int? PoAmendNo { get; set; }
    [Column(TypeName = "decimal(10, 4)")]
    public decimal? PORate { get; set; }
    public string? MIRNO { get; set; }
    public int? MIRYEARCODE { get; set; }
    public int? MIREntryId { get; set; }
    public string? MIRDATE { get; set; }
    public string? ProjectNo { get; set; }
    public string? ProjectDate { get; set; }
    public int? ProjectyearCode { get; set; }
    public int? AgainstImportAccountCode { get; set; }
    public int? AgainstImportYearCode { get; set; }
    public int? AgainstImportInvoiceNo { get; set; }
    public string? AgainstImportInvDate { get; set; }
    #endregion
}
public class PBFormDataModel
{
    public string PurchVouchNo { get; set; }
    public int POYearCode { get; set; }
    public int AccountCode { get; set; }
    public string SchNo { get; set; }
    public int SchYearCode { get; set; }
    public IFormFile excelFile { get; set; }
}
[Serializable()]
public class PurchaseBillModel : PBItemDetail
{
    private IList<SelectListItem> _PBTypeList = new List<SelectListItem>()
        {
            new() { Value = "Domestic", Text = "Domestic" },
            new() { Value = "Import", Text = "Import" },
        };


    private IList<SelectListItem> _PBTypeServList = new List<SelectListItem>()
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
    #region old fields
    //just for display
    public string? UniversalPartCode { get; set; }
    public string? UniversalDescription { get; set; }
    public string? FinFromDate { get; set; }
    public string? FinToDate { get; set; }
    public string? TypeOfSave { get; set; }
    public IList<TextValue>? AccountList { get; set; }

    public string? Branch { get; set; }
    public IList<TextValue>? BranchList { get; set; }

    public IList<TextValue>? CurrencyList { get; set; }
    public IList<TextValue>? DepartmentList { get; set; }
    public int? DocTypeID { get; set; }
    public IList<TextValue>? DocumentList { get; set; }

    [Column(TypeName = "decimal(10, 4)")]
    public decimal Discount { get; set; }

    //[DataType(DataType.DateTime)]
    //[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true, NullDisplayText = "NULL")]
    public string? EntryDate { get; set; }

    public int EntryID { get; set; }
    public string? GSTIncludedONRate { get; set; }
    public int? CreatedBy { get; set; }
    public DateTime? CreatedOn { get; set; }
    public string? CretaedByName { get; set; }
    public int? UpdatedBy { get; set; }
    public DateTime? UpdatedOn { get; set; }
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
    public int PaymentPartyAccountCode { get; set; }
    public string? PaymentPartyAccountDetail { get; set; }
    public bool PN1 { get; set; }
    public bool PN2 { get; set; }
    public string? VouchDate { get; set; }
    public int PBItemCode { get; set; }
    public string? PurchVouchNo { get; set; }
    public string? VendorStateName { get; set; }
    public string? Transport { get; set; }
    public string? VehicleNo { get; set; }
    public float? ExchangeRate { get; set; }
    public string? EntryByMachineName { get; set; }
    public string? PBTypeServItem { get; set; }
    public int? ResposibleEmplforQC { get; set; }

    public IList<SelectListItem> PBTypeServList = new List<SelectListItem>()
    {
        new() { Value = "Service", Text = "Service" },
        new() { Value = "Item", Text = "Item" },
        new() { Value = "Asset", Text = "Asset" },
    };
    public string? PBType { get; set; }
    public IList<SelectListItem> PBTypeList
    {
        get => _PBTypeList;
        set => _PBTypeList = value;
    }

    public int PreparedBy { get; set; }
    public string? PreparedByName { get; set; }
    public IList<TextValue>? PreparedByList { get; set; }
    public string? Remark { get; set; }

    [Column(TypeName = "decimal(10, 4)")]
    public decimal TotalAmtAftrDiscount { get; set; }

    [Column(TypeName = "decimal(10, 4)")]
    public decimal TotalDiscountPercentage { get; set; }
    public int RoundOffAccountCode { get; set; }
    public bool RDT { get; set; }
    public string? TotalRoundOff { get; set; }
    [Column(TypeName = "decimal(10, 4)")]
    public decimal TotalRoundOffAmt { get; set; }
    public int YearCode { get; set; }
    #endregion 
    public string? Approved { get; set; }
    public string? ApprovedDate { get; set; }
    public int? Approvedby { get; set; }

    public IList<SelectListItem> YesNoList
    {
        get => _YesNo;
        set => _YesNo = value;
    }
    #region req param purchbill
    public string? MRNNo { get; set; }
    public int? MRNYearCode { get; set; }
    public int? MRNEntryId { get; set; }
    public DateTime? MRNEntryDate { get; set; }
    public string? StrMRNEntryDate { get; set; }
    public string? GateNo { get; set; }
    public int? GateYearCode { get; set; }
    public int? GateEntryId { get; set; }
    public DateTime? GateDate { get; set; }
    public string? StrGateDate { get; set; }
    public string? InvNo { get; set; }
    public string? InvDate { get; set; }

    public DateTime? InvoiceDate { get; set; }
    public int? AccountCode { get; set; }
    public string? VendorName { get; set; }
    public int? StateId { get; set; }
    public string? StateName { get; set; }
    public string? GSTNO { get; set; }
    public string? GSTType { get; set; }
    public string? GSTRegistered { get; set; }
    public string? VendorAddress { get; set; }
    public string? PurchaseBillDirectPB { get; set; }
    public string? TypeITEMSERVASSETS { get; set; }
    public string? PurchaseBillTypeMRNJWChallan { get; set; }
    public string? PaymentTerms { get; set; }
    public int? PaymentDays { get; set; }
    public string? ModeOfTransport { get; set; }
    public string? FOC { get; set; }
    [Column(TypeName = "decimal(10, 4)")]
    public int? CurrencyId { get; set; }
    public string? Currency { get; set; }
    public string? CC { get; set; }
    public int? UID { get; set; }
    public DateTime? ActualEntryDate { get; set; }
    public int? ActualEntryBy { get; set; }
    public string? MRNRemark { get; set; }
    #endregion

    public string FromDateBack { get; set; }
    public string ToDateBack { get; set; }
    public string DashboardTypeBack { get; set; }
    public string MRNTypeBack { get; set; }
    public string DocumentNameBack { get; set; }
    public string VendorNameBack { get; set; }
    public string VoucherNoBack { get; set; }
    public string InvoiceNoBack { get; set; }
    public string MRNNoBack { get; set; }
    public string GateNoBack { get; set; }
    public string PartCodeBack { get; set; }
    public string ItemNameBack { get; set; }
    public string HSNNoBack { get; set; }
    public string GlobalSearchBack { get; set; }

    public IList<PBItemDetail>? ItemDetailGrid { get; set; }
    public IList<PBItemDetail>? ItemDetailGridd { get; set; }
}

public class PBListDataModel
{
    public int? PBListDataSeqNo { get; set; }
    public string? MRNType { get; set; }
    public string? PartyName { get; set; }
    public IList<TextValue>? PartyNameList { get; set; }
    public string? MRNNo { get; set; }
    public IList<TextValue>? MRNNoList { get; set; }
    public int? MRNYearCode { get; set; }
    public DateTime? MRNEntryDate { get; set; }
    public string? PONO { get; set; }
    public IList<TextValue>? PONOList { get; set; }
    public string? InvoiceNo { get; set; }
    public IList<TextValue>? InvoiceNoList { get; set; }
    public DateTime? InvoiceDate { get; set; }
    public string? GateNo { get; set; }
    public IList<TextValue>? GateNoList { get; set; }
    public DateTime? GateDate { get; set; }
    public string? DocumentName { get; set; }
    public IList<TextValue>? DocumentNameList { get; set; }
    public string? ItemName { get; set; }
    public IList<TextValue>? ItemNameList { get; set; }
    public string? PartCode { get; set; }
    public IList<TextValue>? PartCodeList { get; set; }
    public string? FromDate { get; set; }
    public string? ToDate { get; set; }
    public string? DashboardType { get; set; }
    public string? CheckQc { get; set; }
    public string? QCCompleted { get; set; }
    public int? TotalMRNItemCount { get; set; }
    public int? QCtotalQty { get; set; }
    public int? ItemQCCompledCount { get; set; }
    public int? AccountCode { get; set; }
    public IList<PBListDataModel> PBListData { get; set; }
}