using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.DOM.Models;
[Serializable]



public class JWIssueDashboard
{
    public string? VendorName { get; set; }
    public string? EntryId { get; set; }
    public int? YearCode { get; set; }
    public string? ChallanNo { get; set; }
    public string? Closed { get; set; }
    public string? Process { get; set; }
    public string? ChallanDate { get; set; }
    public string? DeliveryAdd { get; set; }
    public string? CompletlyReceive { get; set; }
    public string? BOMIND { get; set; }
    public string? VendorStateCode { get; set; }
    public decimal? TolApprVal { get; set; }
    public decimal? TotalWt { get; set; }
    public string? EntryDate { get; set; }
    public string? UpdatedDate { get; set; }    
    public string? EntryByMachineName { get; set; }    
    public string? Remarks { get; set; }
    public string? EnteredBy { get; set; }
    public string? UpdatedBy { get; set; }
    public string? ItemName { get; set; }
    public string? PartCode { get; set; }
    public string? Types { get; set; }
    public string? TimeOfRemoval { get; set; }
    public string? JobWorkNewRework { get; set; }
    public string? TransporterName { get; set; }
    public string? VehicleNo { get; set; }
    public string? DispatchTo { get; set; }
    public string? HsnNo { get; set; }
    public float? IssQty { get; set; }
    public string? Unit { get; set;  }     
    public float Rate { get; set; }
    public float Amount { get; set; }
    public float PurchasePrice { get; set; }
    public string? StageDescription { get; set; }
    public string? Store { get; set; }
    public float PendQty { get; set; }
    public string? ScrapPartCode { get; set; }
    public string? ScrapItemName { get; set; }
    public float RecScrapQty { get; set; }
    public float AltQty { get; set; }
    public string? AltUnit { get; set; }
    public string? CC { get; set; }
    public float PendAltQty { get; set; }
    public decimal RecQty { get; set; }
    public string? RecItemName { get; set; }

    public string? SalesPersonEmailId { get; set; }
    public string? eMailFromCC1 { get; set; }
    public string? eMailFromCC2 { get; set; }
    public string? eMailFromCC3 { get; set; }


    public IList<JWIssueDashboard>? JWIssQDashboard { get; set; }
    public IList<JobWorkGridDetail> ItemDetailGrid { get; set; }
}


//grid data for dashboard
public class JWIssQDashboard : JWIssueDashboard
{
    public string? FromDate { get; set; }
    public string? ItemName { get; set; }
    public string? PartCode { get; set; }
    //right
    public IList<TextValue>? ChallanList { get; set; }
    public string? ToDate { get; set; }
    public string? Dashboardtype { get; set; }
    public string? FromDate1 { get; set; }
    public string? ToDate1 { get; set; }
}
[Serializable()]
public class JobWorkGridDetail : TaxModel
{
    public string? VendJwAllowToSelectBatchNo { get; set; }
    public int? SeqForBatch { get; set; }
    public string? Stockable { get; set; }
    public string? ItemServAssets { get; set; }
    public int SeqNo { get; set; }
    public int ItemCode { get; set; }
    public string PartCode { get; set; }
    public string ItemName { get; set; }
    public string ProcessName { get; set; }
    public string StoreName { get; set; }
    public int HSNNo { get; set; }
    public decimal IssQty { get; set; }
    public string Unit { get; set; }
    public decimal Rate { get; set; }
    public decimal pendqty { get; set; }

    public decimal Amount { get; set; }
    //1111
    public string RemarkDetail { get; set; }
    public decimal PurchasePrice { get; set; }
    public int ProcessId { get; set; }
    public int StoreId { get; set; }
    public string BatchNo { get; set; }
    public string UniqueBatchNo { get; set; }
    //nusrat -1
    public decimal StockQty { get; set; }
    public decimal BatchStockQty { get; set; }
    public string Closed { get; set; }
    public decimal PendQty { get; set; }
    public int RecScrapCode { get; set; }
    public int RecScrapItemCode { get; set; }
    public string RecScrapPartCode { get; set; }
    public string RecScrapItemName { get; set; }
    public decimal RecScrapQty { get; set; }
    public decimal AltQTy { get; set; }
    public string AltUnit { get; set; }
    public decimal PendAltQty { get; set; }
    public decimal PendScrapQty { get; set; }
    public int RecItemCode { get; set; }
    public string RecPartCode { get; set; }
    public string RecItemName { get; set; }

    public decimal RecQty { get; set; }
    public decimal ToLimit { get; set; }
    public string AgainstAdaviceNo { get; set; }
    public int AgainstAdaviceYear { get; set; }
    public string AgainstAdaviceDate { get; set; }
    public string ItemSize { get; set; }
    public string ItemColor { get; set; }
    public string OtherInstruction { get; set; }
    public string PONO { get; set; }
    public int POYear { get; set; }
    public string PODate { get; set; }
    public string SchNo { get; set; }
    public int SchYear { get; set; }
    public string SchDate { get; set; }
    public string POAAmmendNo { get; set; }
    public int SeqNoforeach { get; set; }
}
[Serializable()]
public class JobWorkIssueModel : JobWorkGridDetail
{
    public JobWorkIssueModel()
    {
        TaxDetailGrid = new TaxModel();
    }
    public string? FinFromDate { get; set; }
    public string? FinToDate { get; set; }
    public string? ChangeEventTriggered { get; set; }
    public string? VendJwAllowToSelectBatchNo { get; set; }
    public float? Distance { get; set; }
    public int EntryId { get; set; }
    public int YearCode { get; set; }
    public string EntryDate { get; set; }
    public string JobWorkNewRework { get; set; }
    public string JWChallanNo { get; set; }
    public string ChallanDate { get; set; }
    public bool AC1 { get; set; }
    public int AccountCode { get; set; }
    public bool SH { get; set; }
    public string DeliveryAdd { get; set; }
    public string VendorStateCode { get; set; }
    public string Remark { get; set; }
    public decimal TolApprVal { get; set; }
    public decimal TotalWt { get; set; }
    public string BomStatus { get; set; }
    public string Closed { get; set; }
    public string Types { get; set; }
    public string GstType { get; set; }
    public int EnteredByEmpid { get; set; }
    public string CompletlyReceive { get; set; }
    public string timeofremoval { get; set; }
    public int Processdays { get; set; }
    public string? TransporterName { get; set; }
    public string? VehicleNo { get; set; }
    public string? DispatchTo { get; set; }
    public int UID { get; set; }
    public string CC { get; set; }
    public string? EnterByMachineName { get; set; }
    public string ActualEntryDate { get; set; }
    public int ActualEnteredBy { get; set; }
    public string? CreatedByName { get; set; }
    public string? UpdatedByName { get; set; }
    public string? FromDateBack { get; set; }
    public string? ToDateBack { get; set; }
    public string? PartCodeBack { get; set; }
    public string? ItemNameBack { get; set; }
    public string? DashboardTypeBack { get; set; }
    public string? VendorNameBack { get; set; }
    public string? ChallanNoBack { get; set; }
    public int SeqNoforeach { get; set; }
    public bool IC1 { get; set; }
    //public int ItemCode { get; set; }
    //public string PartCode { get; set; }
    //public string ItemName { get; set; }
    //public int HSNNo { get; set; }
    //public decimal IssQty { get; set; }
    //public string Unit { get; set; }
    //public decimal Rate { get; set; }
    //public decimal Amount { get; set; }
    //public string RemarkDetail { get; set; }
    //public decimal PurchasePrice { get; set; }
    //public int ProcessId { get; set; }
    //public string ProcessName { get; set; }
    //public int StoreId { get; set; }
    //public string StoreName { get; set; }
    //public string BatchNo { get; set; }
    //public string UniqueBatchNo { get; set; }
    //public decimal StockQty { get; set; }
    //public decimal BatchStockQty { get; set; }
    //public decimal PendQty { get; set; }
    //public int RecScrapCode { get; set; }
    //public decimal RecScrapQty { get; set; }
    //public decimal AltQTy { get;set; }
    //public string AltUnit { get; set; }
    //public decimal PendAltQty { get; set; }
    //public decimal PendScrapQty { get; set; }
    //public int RecItemCode { get; set; }
    //public decimal RecQty { get; set; }
    //public decimal ToLimit { get; set; }
    //public string AgainstAdaviceNo { get; set; }
    //public int AgainstAdaviceYear { get; set; }
    //public string AgainstAdaviceDate { get; set; }
    //public string ItemSize { get; set; }
    //public string ItemColor { get; set; }
    //public string OtherInstruction { get; set; }
    //public string PONO { get; set; }
    //public int POYear { get; set; }
    //public string PODate { get; set; }
    //public string SchNo { get; set; }
    //public int SchYear { get; set; }
    //public string SchDate { get; set;}
    //public string POAAmmendNo { get; set; }


    //tax detail
    public string? TotalRoundOff { get; set; }
    [Column(TypeName = "decimal(10, 4)")]
    public decimal TotalDiscountPercentage { get; set; }
    [Column(TypeName = "decimal(10, 4)")]
    public decimal TotalAmtAftrDiscount { get; set; }
    [Column(TypeName = "decimal(10, 4)")]
    public decimal NetTotal { get; set; }
    [Column(TypeName = "decimal(10, 4)")]
    public decimal TotalAmount { get; set; }
    [Column(TypeName = "decimal(10, 4)")]
    public decimal ItemNetAmount { get; set; }
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
    //
    public IList<TextValue>? BranchList { get; set; }
    public IList<TextValue>? VendorList { get; set; }
    public IList<TextValue>? ItemNameList { get; set; }
    public IList<TextValue>? PartCodeList { get; set; }
    public IList<TextValue>? ProcessList { get; set; }
    public IList<TextValue>? EmployeeList { get; set; }
    public IList<TextValue>? StoreList { get; set; }

    public IList<TextValue>? RecScrapPartCodeList { get; set; }
    public IList<TextValue>? RecScrapItemNameList { get; set; }
    public int RecScrapItemCode { get; set; }
    public string RecScrapPartCode { get; set; }
    public string RecScrapItemName { get; set; }
    public IList<TextValue>? RecPartCodeList { get; set; }
    public IList<TextValue>? RecItemNameList { get; set; }
    public string RecPartCode { get; set; }
    public string RecItemName { get; set; }

    private IList<SelectListItem> BOMStatus = new List<SelectListItem>()
        {
            new() { Value = "BOM", Text = "BOM" },
            new() { Value = "Individual", Text = "Individual" },
        };

    public IList<SelectListItem> BOMStatusList
    {
        get => BOMStatus;
        set => BOMStatus = value;
    }
    private IList<SelectListItem> _Type = new List<SelectListItem>()
        {
            new() { Value = "Item", Text = "Item" },
            new() { Value = "Service", Text = "Services" },
            new() { Value = "Assets", Text = "Assets" },
        };

    public IList<SelectListItem> TypeList
    {
        get => _Type;
        set => _Type = value;
    }
    private IList<SelectListItem> JobWorkRework = new List<SelectListItem>()
        {
            new() { Value = "Job Work", Text = "Job Work" },
            new() { Value = "Rework", Text = "Rework" },
        };

    public IList<SelectListItem> JobWorkReworkList
    {
        get => JobWorkRework;
        set => JobWorkRework = value;
    }
    public IList<JobWorkGridDetail> JobDetailGrid { get; set; }
    public TaxModel TaxDetailGrid { get; set; }

}

