using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Numerics;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.DOM.Models;

/// <inheritdoc/>

[Serializable()]
public class PODashBoard : TimeStamp
{


    public string? POCloseDate { get; set; }
    public string? POComplete { get; set; } 
    public string? CC { get; set; }
    public string? ApprovedDate { get; set; }
    public string? EntryByMachineName { get; set; }
    public string? Approved { get; set; }
    public string? PoallowtoprintWithoutApproval { get; set; }
    public string? ShowOnlyAmendItem { get; set; }
    
    public string? ApproveAmm { get; set; }
    public string? Approval1Levelapproved { get; set; }
    public string? Currency { get; set; }
    public string? DeliveryDate { get; set; }
    public int EntryID { get; set; }
    public string? FOC { get; set; }
    public string? FromDate { get; set; }
    public string? ItemName { get; set; }
    public string? OrderType { get; set; }
    public string? PartCode { get; set; }
    public string? PaymentTerms { get; set; }
    public IList<PODashBoard> PODashboard { get; set; }
    public string? PODate { get; set; }
    public string? POFor { get; set; }
    public string? PONo { get; set; }
    public string? OrderNo { get; set; }
    public IList<TextValue>? PONoList { get; set; }
    public string? POType { get; set; }
    public string? POTypeServItem { get; set; }
    public string? ToDate { get; set; }
    public string? VendorAddress { get; set; }
    public string? VendorName { get; set; }
    public string? WEF { get; set; }
    public int YearCode { get; set; }

    public string? PONO { get; set; }
    public decimal OrderAmt { get; set; }
    public decimal OrderNetAmt { get; set; }
    public string? AmmType { get; set; }
    public int? AmmNo { get; set; }
    public string? EnteredBy { get; set; }
    public string? UpdatedByName { get; set; }
    public string? DashboardType { get; set; }
    public int QuotNo { get; set; }
    public int QuotYear { get; set; }
    public float BasicAmount { get; set; }
    public float NetAmount { get; set; }
    public string? HsnNo { get; set; }
    public string? POQty { get; set; }
  //  public string? POEntryId { get; set; }
    //public string? POyearcode { get; set; }
    public string? Unit { get; set; }
    public string? AltPOQty { get; set; }
    public string? AltUnit { get; set; }
    public string? Rate { get; set; }   
    public string? RateInOtherCurr { get; set; }   
    public string? DiscPer { get; set; }   
    public string? DiscRs { get; set; }   
    public string? Amount { get; set; }   
    public decimal PendQty { get; set; }   
    public decimal PendAltQty { get; set; }   
    public string? UnitRate { get; set; }   
    public string SummaryDetail { get; set; }
    public int EntryId { get; set; }
    public int SeqNo { get; set; }
    public int ItemCode { get; set; }
    public int HSNNo { get; set; }
    public decimal TolLimitPercentage { get; set; }
    public decimal TolLimitQty { get; set; }
    public decimal AdditionalRate { get; set; }
    public decimal OldRate { get; set; }
    public string? Remark { get; set; }
    public string? Description { get; set; }
    public int Process { get; set; }
    public decimal PkgStd { get; set; }
    public int AmmendmentNo { get; set; }
    public DateTime AmmendmentDate { get; set; }
    public string? AmmendmentReason { get; set; }
    public decimal FirstMonthTentQty { get; set; }
    public decimal SecMonthTentQty { get; set; }
    public string? SizeDetail { get; set; }
    public string? Colour { get; set; }
    public int CostCenter { get; set; }
    public string? Active { get; set; }
    public string? EntryDate { get; set; }
    public string? Branch { get; set; }
    public string? ActualEntryBy { get; set; }
    public string? AmmEffDate { get; set; }
    public string? ApproveddByEmp { get; set; }
    public string DeliveryTerm { get; set; }
    public string DeliveryTerms { get; set; }
    public string UpdatedOn { get; set; }
    public string? RateApplicableOnUnit { get; set; }

}

[Serializable()]
public class POItemDetail : TaxModel
{
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
    public decimal AltPOQty { get; set; }

    public string? AltUnit { get; set; }
    public string? AmendmentDate { get; set; }
    public int AmendmentNo { get; set; }
    public string? AmendmentReason { get; set; }

    [Column(TypeName = "decimal(10, 4)")]
    public decimal Amount { get; set; }

    public string? Color { get; set; }
    public int CostCenter { get; set; }

    public IList<DeliverySchedule>? DeliveryScheduleList { get; set; }

    public string? Description { get; set; }

    [Column(TypeName = "decimal(10, 4)")]
    [Range(0, 100, ErrorMessage = "Should ve in between 0-100")]
    public decimal DiscPer { get; set; }

    [Column(TypeName = "decimal(10, 4)")]
    public decimal DiscRs { get; set; }

    [Column(TypeName = "decimal(10, 4)")]
    public decimal FirstMonthTentQty { get; set; }

    public int HSNNo { get; set; }
    public bool IN1 { get; set; }
    public int? ItemCode { get; set; }
    public IList<TextValue>? ItemNameList { get; set; }

    [Column(TypeName = "decimal(10, 4)")]
    public decimal ItemNetAmount { get; set; }

    public string? ItemText { get; set; }

    [Column(TypeName = "decimal(10, 4)")]
    public decimal OldRate { get; set; }

    [Column(TypeName = "decimal(10, 4)")]
    public decimal OtherRateCurr { get; set; }

    public int PartCode { get; set; }
    public IList<TextValue>? PartCodeList { get; set; }
    public string? PartText { get; set; }

    [Column(TypeName = "decimal(10, 4)")]
    public decimal PendQty { get; set; }

    public string? PIRemark { get; set; }

    [Column(TypeName = "decimal(10, 4)")]
    public decimal PkgStd { get; set; }

    [Column(TypeName = "decimal(10, 4)")]
    public decimal POQty { get; set; }

    public int Process { get; set; }
    public IList<TextValue>? ProcessList { get; set; }

    [Column(TypeName = "decimal(10, 4)")]
    public decimal Rate { get; set; }

    [Column(TypeName = "decimal(10, 4)")]
    public decimal SecMonthTentQty { get; set; }

    public int SeqNo { get; set; }
    public bool SH { get; set; }
    public string? SizeDetail { get; set; }

    [Column(TypeName = "decimal(10, 4)")]
    public decimal TolLimitPercent { get; set; }

    [Column(TypeName = "decimal(10, 4)")]
    public decimal TolLimitQty { get; set; }

    public string? Unit { get; set; }
    public string? UnitRate { get; set; }

    public IList<SelectListItem> UnitRateList
    {
        get => _UnitRate;
        set => _UnitRate = value;
    }
}
public class FormDataModel
{
    public string PoNo { get; set; }
    public int POYearCode { get; set; }
    public int AccountCode { get; set; }
    public string SchNo { get; set; }
    public int SchYearCode { get; set; }
    public IFormFile excelFile { get; set; }
}
[Serializable()]
public class PurchaseOrderModel : POItemDetail
{
    private IList<SelectListItem> _ManufTradeList = new List<SelectListItem>()
        {
            new() { Value = "Manufaturer", Text = "Manufaturer" },
            new() { Value = "Trader", Text = "Trader" },
        };

    private IList<SelectListItem> _OrderTypeList = new List<SelectListItem>()
        {
            new() { Value = "Domestic", Text = "Domestic" },
            new() { Value = "Import", Text = "Import" },
        };

    private IList<SelectListItem> _PackingChg = new List<SelectListItem>()
    {
        new() { Value = "Paid By Us", Text = "Paid By Us" },
        new() { Value = "Paid By Vendor", Text = "Paid By Vendor" },
    };

    private IList<SelectListItem> _POTypeList = new List<SelectListItem>()
        {
            new() { Value = "Close", Text = "Close" },
            new() { Value = "Open", Text = "Open" },
        };

    private IList<SelectListItem> _POTypeServList = new List<SelectListItem>()
        {
            new() { Value = "I", Text = "Item" },
            new() { Value = "S", Text = "Service" },
        };

    private IList<SelectListItem> _YesNo = new List<SelectListItem>()
        {
            new() { Value = "N", Text = "No" },
            new() { Value = "Y", Text = "Yes" },
        };
    

    //just for display
    public string? UniversalPartCode { get; set; }
    public string? ShowOnlyAmendItem { get; set; }
    public string? UniversalDescription { get; set; }
    public string? FinFromDate { get; set; }
    public string? FinToDate { get; set; }
    public string? TypeOfSave { get; set; }
    public int AccountCode { get; set; }
    public IList<TextValue>? AccountList { get; set; }
    public string? AmmDate { get; set; }
    public int AmmNo { get; set; }
    public int AmmEntryId { get; set; } 
    public int AmmYearCode { get; set; }

    public string? Branch { get; set; }
    public IList<TextValue>? BranchList { get; set; }
    public string? Clause { get; set; }
    public IList<TextValue>? CostCenterList { get; set; }

    [Column(TypeName = "decimal(10, 4)")]
    public decimal Currency { get; set; }

    public IList<TextValue>? CurrencyList { get; set; }
    public int DAltQty { get; set; }
    public string? DDate { get; set; }
    public int DDays { get; set; }
    public string? DeliverySch { get; set; }
    public string? DeliveryTerms { get; set; }
    public IList<TextValue>? DepartmentList { get; set; }

    [Column(TypeName = "decimal(10, 4)")]
    public decimal Discount { get; set; }
    
    public int DPartCode { get; set; }
    public int DQty { get; set; }
    public string? DRemark { get; set; }
    public int DTQty { get; set; }

    //[DataType(DataType.DateTime)]
    //[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true, NullDisplayText = "NULL")]
    public string? EntryDate { get; set; }

    public int EntryID { get; set; }
    public string? FOC { get; set; }
    public string? FreightPaidBy { get; set; }
    public string? GSTIncludedONRate { get; set; }
    public string? Inspection { get; set; }
    public string? Installation { get; set; }
    public string? InsurancePaidBy { get; set; }
    public bool isPONo { get; set; }
    public IList<POItemDetail>? ItemDetailGrid { get; set; }
    public IList<PendingIndentDetailModel>? PendingIndentDetailGrid { get; set; }
    public int ItemsForDepartment { get; set; }
    public string? LDClause { get; set; }
    public string? ManufTrade { get; set; }
    public int? CreatedBy { get; set; }
    public DateTime? CreatedOn { get; set; }
    public string? CretaedByName { get; set; }
    public int? UpdatedBy { get; set; }
    public DateTime? UpdatedOn { get; set; }
    public string? UpdatedByName { get; set; }  

    public IList<SelectListItem> ManufTradeList
    {
        get => _ManufTradeList;
        set => _ManufTradeList = value;
    }

    public string? ModeOFTransport { get; set; }
    public int MRPNo { get; set; }
    public IList<TextValue>? MRPNoList { get; set; }
    public int MRPYearCode { get; set; }

    [Column(TypeName = "decimal(10, 4)")]
    public decimal NetTotal { get; set; }

    public int OrderNo { get; set; }
    public string? OrderType { get; set; }

    public IList<SelectListItem> OrderTypeList
    {
        get => _OrderTypeList;
        set => _OrderTypeList = value;
    }

    public int PackingChg { get; set; }

    public IList<SelectListItem> PackingChgList
    {
        get => _PackingChg;
        set => _PackingChg = value;
    }

    public string? PackingType { get; set; }
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
    public string? POCloseDate { get; set; }
    public string? PODate { get; set; }
    public string POFor { get; set; }
    public IList<TextValue>? POForList { get; set; }
    public int POItemCode { get; set; }
    public string? PONo { get; set; }
    public string? POPrefix { get; set; }
    public string? PORemark { get; set; }
    public string? POType { get; set; }

    public IList<SelectListItem> POTypeList
    {
        get => _POTypeList;
        set => _POTypeList = value;
    }
    public string? EntryByMachineName { get; set; }
    public string? POTypeServItem { get; set; }
    public int? ResposibleEmplforQC { get; set; }

    //public IList<SelectListItem> POTypeServList
    //{
    //    get => _POTypeServList;
    //    set => _POTypeServList = value;
    //}
    //public IList<SelectListItem> POTypeServList
    //{
    //    get ;
    //    set ;
    //}

    public IList<SelectListItem> POTypeServList = new List<SelectListItem>()
        {
            new() { Value = "Service", Text = "Service" },
            new() { Value = "Item", Text = "Item" },
            new() { Value = "Asset", Text = "Asset" },
        };

    public int PreparedBy { get; set; }
    public string? PreparedByName { get; set; }
    public IList<TextValue>? PreparedByList { get; set; }
    public string? PriceBasis { get; set; }
    public string? QuotDate { get; set; }
    public IList<TextValue>? QuotList { get; set; }
    public string? QuotNo { get; set; }
    public int QuotYear { get; set; }
    public string? RefDate { get; set; }
    public string? RefNo { get; set; }
    public string? Remark { get; set; }
    public string? ShipmentTerm { get; set; }
    public string? ShippingAddress { get; set; }
    public int ShipTo { get; set; }

    [Column(TypeName = "decimal(10, 4)")]
    public decimal TotalAmtAftrDiscount { get; set; }

    [Column(TypeName = "decimal(10, 4)")]
    public decimal TotalDiscountPercentage { get; set; }

    public string? TotalRoundOff { get; set; }
    public string? VendorAddress { get; set; }
    public string? Warrenty { get; set; }
    public string? WEF { get; set; }
    public int YearCode { get; set; }

    public string? Approved { get; set; }
    public string? ApprovedDate { get; set; }
    public int? Approvedby { get; set; }
    public string? AmmApproved { get; set; }
    public string? AmmApprovedDate { get; set; }
    public int? AmmApprovedby { get; set; }
    public string? FirstLvlApproved { get; set; }
    public string? FirstLvlApprovedDate { get; set; }
    public int? FirstLvlApprovedby { get; set; }

    public IList<SelectListItem> YesNoList
    {
        get => _YesNo;
        set => _YesNo = value;
    }
}

public class PendingIndentDetailModel
{
    public string IndentNo { get; set; }
    public string IndentDate { get; set; }
    public string IndentYearCode { get; set; }
    public string IndentEntryId { get; set; }
    public string PartCode { get; set; }
    public string ItemName { get; set; }
    public string PONO { get; set; }
    public string POEntryId { get; set; }
    public string POYearCode { get; set; }
    public string POAccountCode { get; set; }
    public string Qty { get; set; }
    public string AltQty { get; set; }
    public string PendQtyForPO { get; set; }
    public string Unit { get; set; }
    public string AltUnit { get; set; }
    public string ReqDate { get; set; }
    public string PODate { get; set; }
    public string ItemCode{ get; set; }
}