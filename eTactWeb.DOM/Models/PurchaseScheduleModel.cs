using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.DOM.Models;

[Serializable()]
public class PurchSchResult
{
    public string? AltUnit { get; set; }
    public int ItemCode { get; set; }
    public string? ItemName { get; set; }
    public string? PartCode { get; set; }
    public double Rate { get; set; }
    public string? Unit { get; set; }
}

[Serializable]
public class PurchaseScheduleDashboard
{
    //"SchNo", "SchDate","PONO" , "PODate" ,"VendorName", "DeliveryAddress", "SchEffFromDate", "SchEffTillDate",
    //                "SchApproved", "SchYear", "POYearCode", "EntryID", "CreatedBy", "CreatedOn", "ApprovedBy");
    public string? SchNo { get; set; }
    public string? Schdate { get; set; }
    public string? schAmendDate { get; set; }
    public string? Branch { get; set; }

    
    public string? PONO { get; set; }
    public string? PODate { get; set; }
    public string? VendorName { get; set; }
    public string? DeliveryAddress { get; set; }
    public string? SchEffFromDate { get; set; }
    public string? SchEffTillDate { get; set; }
    public string? ApprovedBy { get; set; }
    public string? UserName { get; set; }
    //public string? CreatedBy { get; set; }
    public int? CreatedBy {  get; set; }
    public string? CreatedByEmpName { get; set; }
    public string? SchAmendApprovedByEmp { get; set; }
    public string? SchAmendApprove { get; set; }

    public string? schAmendNo { get; set; }

    public string? MRPNO { get; set; }
    public string? Active { get; set; }

    public string? CreatedOn { get; set; }
     public int EntryID { get; set; }
    public int? SchYear { get; set; }
    public int POYearCode { get; set; }
    public string? Mode { get; set; }
    public string? PartCode { get; set; }
    public string? ItemName { get; set; }
    public string? Rate { get; set; }
    public string? SchQty { get; set; }
    public string? PendQty { get; set; }
    public string? Unit { get; set; }
    public string? AltUnit { get; set; }
    public string? AltSchQty { get; set; }
    public string? AltPendQty { get; set; }//Canceled
    //public string? SchCanceled { get; set; }//

    public int ItemAmendNo { get; set; }
    public string? SchApproved { get; set; }

    public string? DeliveryDate { get; set; }
    public string? EntryByMachineName { get; set; }
    public string? Canceled { get; set; }

    public string? SchCompleted { get; set; }
    public string? DashboardType { get; set; }
      
    public IList<PurchaseScheduleDashboard>? PSDashboard { get; set; }

}

[Serializable]
public class PurchaseScheduleGrid : TimeStamp
{
    [Column(TypeName = "decimal(10, 4)")]
    public decimal AltPendQty { get; set; }
    [Column(TypeName = "decimal(10, 4)")]
    public decimal AltSchQty { get; set; }
    public string? AltUnit { get; set; }
    //[DataType(DataType.Date)]
    //[DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    [DisplayFormat(DataFormatString = @"{0:yyyy-mm-dd HH:mm:ss}", ApplyFormatInEditMode = false)]
     public string? DeliveryDate { get; set; }

    public string? DeliveryWeek { get; set; }
    public IFormFile? ExcelFile { get; set; }
    public int ItemCode { get; set; }
    public string? ItemColor { get; set; }
    public string? ItemName { get; set; }
    public string? ItemSize { get; set; }
    public string? OtherDetail { get; set; }
    public string? PartCode { get; set; }
    [Column(TypeName = "decimal(10, 4)")]
    public decimal PendQty { get; set; }
    [Column(TypeName = "decimal(10, 4)")]
    public decimal Rate { get; set; }
    [Column(TypeName = "decimal(10, 4)")]
    public decimal RateInOthCurr { get; set; }
    public string? Remarks { get; set; }
    [Column(TypeName = "decimal(10, 4)")]
    public decimal SchQty { get; set; }
    public int SeqNo { get; set; }
    public string? Unit { get; set; }
    //[DataType(DataType.Date)]
    //[DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    [DisplayFormat(DataFormatString = @"{0:yyyy-mm-dd HH:mm:ss}", ApplyFormatInEditMode = false)]
    //[DataType(DataType.Date)]
    //[DisplayFormat(DataFormatString = "{0:dd/MMM/yyyy}")]
    public string schAmendDate { get; set; }
    public int? schAmendNo { get; set; }
    public int? SchAmendYear { get; set; }
    [Column(TypeName = "decimal(10, 4)")]
    public decimal TentQtyFor1stMonth { get; set; }
    [Column(TypeName = "decimal(10, 4)")]
    public decimal TentQtyFor2stMonth { get; set; }
}

[Serializable]
public class PurchaseSubScheduleModel : PurchaseScheduleGrid
{
    private IList<SelectListItem> _OrderPriority = new List<SelectListItem>()
    {
        new() { Value = "Low", Text = "Low" },
        new() { Value = "High ",Text = "High" },
        new() { Value = "Medium", Text = "Medium" }
    };

    private IList<SelectListItem> _TentetiveConfirm = new List<SelectListItem>()
    {
        new() { Value = "Tentetive", Text = "Tentetive" },
        new() { Value = "Confirm", Text = "Confirm" }
    };

    public int AccountCode { get; set; }
    public IList<TextValue>? AccountList { get; set; }
    public IList<TextValue>? BranchList { get; set; }
    public string? CC { get; set; }
    public string? DeliveryAddress { get; set; }
    //[DataType(DataType.Text)]
    //[DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    [DisplayFormat(DataFormatString = @"{0:yyyy-mm-dd HH:mm:ss}", ApplyFormatInEditMode = false)]
    //[DataType(DataType.Date)]
    //[DisplayFormat(DataFormatString = "{0:dd/MMM/yyyy}")]
    public string? EntryDate { get; set; }
    [DisplayFormat(DataFormatString = @"{0::dd-mm-yy}", ApplyFormatInEditMode = false)]
    public string? DeliveryDate { get; set; }
    public int EntryID { get; set; }
    public string? ModeOfTransport { get; set; }
    public string? OrderPriority { get; set; }

    
    public IList<SelectListItem> OrderPriorityList
    {
        get => _OrderPriority;
        set => _OrderPriority = value;
    }

    public IList<PurchaseScheduleGrid>? PurchaseScheduleList { get; set; }
    //[DataType(DataType.Date)]
    //[DisplayFormat(DataFormatString = "{0:dd/MMM/yyyy}")]
    //[DataType(DataType.Text)]
    //[DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    [DisplayFormat(DataFormatString = @"{0:yyyy-mm-dd HH:mm:ss}", ApplyFormatInEditMode = false)]
    public string? SchAmendmentDate { get; set; }
    public string? Approved { get; set; }
    public string? ApprovedDate { get; set; }
    public int Approvedby { get; set; }
    public string? AmmApproved { get; set; }
    public string? AmmApprovedDate { get; set; }
    public int AmmApprovedby { get; set; }
    public int SchAmendmentNo { get; set; }
    public string? EntryByMachineName { get; set; }
    public string? CreatedByName { get; set; }
    public string? UpdatedByName { get; set; }
    public int schAmendNo { get; set; }
    //[DataType(DataType.Text)]
    //[DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    [DisplayFormat(DataFormatString = @"{0:yyyy-mm-dd HH:mm:ss}", ApplyFormatInEditMode = false)]
    //[DataType(DataType.Date)]
    //[DisplayFormat(DataFormatString = "{0:dd/MMM/yyyy}")]
    public string? ScheduleDate { get; set; }

    public string? ScheduleNo { get; set; }
    //[DataType(DataType.Text)]
    //[DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    [DisplayFormat(DataFormatString = @"{0:yyyy-mm-dd HH:mm:ss}", ApplyFormatInEditMode = false)]
    //[DataType(DataType.Date)]
    //[DisplayFormat(DataFormatString = "{0:dd/MMM/yyyy}")]
    public string? SchEffFromDate { get; set; }
    //[DataType(DataType.Text)]
    //[DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    [DisplayFormat(DataFormatString = @"{0:yyyy-mm-dd HH:mm:ss}", ApplyFormatInEditMode = false)]
    //[DataType(DataType.Date)]
    //[DisplayFormat(DataFormatString = "{0:dd/MMM/yyyy}")]
    public string? SchEffTillDate { get; set; }
    //[DataType(DataType.Text)]
    //[DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    [DisplayFormat(DataFormatString = @"{0::dd-mm-yy}", ApplyFormatInEditMode = false)]
    //[DataType(DataType.Date)]
    //[DisplayFormat(DataFormatString = "{0:dd/MMM/yyyy}")]
    public string? POCloseDate { get; set; }
    //[DataType(DataType.Date)]
    //[DataType(DataType.Text)]
    //[DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    [DisplayFormat(DataFormatString = @"{0:dd-mm-yy}", ApplyFormatInEditMode = false)]
    //[DataType(DataType.Date)]
    //[DisplayFormat(DataFormatString = "{0:dd/MMM/yyyy}")]
    public string? PODate { get; set; }

    public string  PONO { get; set; }

    public int POAmenNo { get; set; }
    //[DataType(DataType.Text)]
    //[DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    [DisplayFormat(DataFormatString = @"{0:yyyy-mm-dd HH:mm:ss}", ApplyFormatInEditMode = false)]
    //[DataType(DataType.Date)]
    //[DisplayFormat(DataFormatString = "{0:dd/MMM/yyyy}")]
    public string POAmendDate { get; set; }
    public int POAmendYearCode { get; set; }
    public string? OrderTypeJWPurch { get; set; }
    public string? ItemService { get; set; }
    public int FirstMonthTentRatio { get; set; }
    public int? SecMonthTentRatio { get; set; }
    public string? BillingAddress { get; set; }

    public int AmmEntryId { get; set; }
    public int AmmYearCode { get; set; }
    public int? MRPNO { get; set; }
    public int? MRPentryId { get; set; }
    public int? MRPNoYearCode { get; set; }


    public string? FinFromDate { get; set; }
    public string? FinToDate { get; set; }

    public int POYearCode { get; set; }

    public string? TentetiveConfirm { get; set; }

    public IList<SelectListItem> TentetiveConfirmList
    {
        get => _TentetiveConfirm;
        set => _TentetiveConfirm = value;
    }
    public int UID { get; set; }
    public int YearCode { get; set; }
}
[Serializable]
public class PSDashboard : PurchaseScheduleDashboard
{
    public string? PONo { get; set; }    
    public string? FromDate { get; set; }
    public string? ItemName { get; set; }
    public string? PartCode { get; set; }
    //right
    //public IList<TextValue>? PONOList { get; set; }
    public string? ToDate { get; set; }
    public string? DashboardType { get; set; }
}
[Serializable]
public class PurchSchRoot
{
    public List<Result>? Result { get; set; }
    public int StatusCode { get; set; }
    public string? StatusText { get; set; }
}