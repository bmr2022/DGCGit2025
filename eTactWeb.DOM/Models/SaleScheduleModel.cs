using System.ComponentModel.DataAnnotations.Schema;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.DOM.Models;

[Serializable()]
public class Result
{
    public string? AltUnit { get; set; }
    public int ItemCode { get; set; }
    public string? ItemName { get; set; }
    public string? PartCode { get; set; }
    public double Rate { get; set; }
    public string? Unit { get; set; }
    public List<InProcessQcDetail> Table { get; set; }
}

[Serializable]
 public class SaleScheduleDashboard
{
    public string? ApprovedBy { get; set; }
    public string? CreatedBy { get; set; }
    public string? CreatedOn { get; set; }
    public string? CustomerName { get; set; }
    public string? CustomerOrderNo { get; set; }
    public string? DeliveryAddress { get; set; }
    public int EntryID { get; set; }
    public string EntryDate { get; set; }
    public string? SchApproved { get; set; }
    public string? SchAmendApprove { get; set; }
    public string? SchDate { get; set; }
    public string? SchEffFromDate { get; set; }
    public string? SchEffTillDate { get; set; }
    public string? SchNo { get; set; }
    public string? SchYear { get; set; }
    public string? SODate { get; set; }
    public List<TextValue>? SONoList { get; set; }
    public string? SOCloseDate { get; set; }
    public string? SchCompleted { get; set; }
    public string? SchClosed { get; set; }
    public string? FromDate { get; set; }
    public string? ToDate { get; set; }
    public int SchAmendNo { get; set; }
    public int SONO { get; set; }
    public int SOYearCode { get; set; }
    public string? CreatedByName { get; set; }
    public string? Mode { get; set; }
    public string? ModeOfTransport { get; set; }
    public string? OrderPriority { get; set; }
    public string? CC { get; set; }
    public string? UpdatedByName { get; set; }
    public string? UpdatedOn { get; set; }
    public string? EntryByMachineName { get; set; }
    public string? PartCode { get; set; }
    public string? ItemName { get; set; }
    public string? Unit { get; set; }
    public float SchQty { get; set; }
    public float PendQty { get; set; }
    public string? AltUnit { get; set; }
    public float AltPendQty { get; set; }
    public decimal Rate { get; set; }
    public decimal RateInOthCurr { get; set; }
    public string DeliveryDate { get; set; }
    public string ItemSize { get; set; }
    public string ItemColor { get; set; }
    public string OtherDetail { get; set; }
    public string Remarks { get; set; }
    public string SummaryDetail { get; set; }
    public List<SaleScheduleDashboard>? SSDashboard { get; set; }
}

[Serializable]
public class SaleScheduleGrid : TimeStamp
{
    [Column(TypeName = "decimal(10, 4)")]
    public decimal AltPendQty { get; set; }

    [Column(TypeName = "decimal(10, 4)")]
    public decimal AltSchQty { get; set; }

    public string? AltUnit { get; set; }
    public string? DeliveryDate { get; set; }
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
}

[Serializable]
public class SaleSubScheduleModel : SaleScheduleGrid
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

    public SaleSubScheduleModel()
    {
        SaleScheduleList = new List<SaleScheduleGrid>();
    }

    public string? TypeOfSave { get; set; }
    public int AccountCode { get; set; }
    public IList<TextValue>? AccountList { get; set; }
    public IList<TextValue>? BranchList { get; set; }
    public IList<TextValue>? CustomerOrderList { get; set; }
    public string? CC { get; set; }
    public string? CustomerOrderNo { get; set; }
    public string? DeliveryAddress { get; set; }
    public int AmmEntryId { get; set; }
    public int AmmYearCode { get; set; }
    public string? SchApproved { get; set; }
    public string? SchAppBy { get; set; }
    public string? SchApprovalDate { get; set; }
    public string? SchAmendApproved { get; set; }
    public string? SchAmendAppBy { get; set; }
    public string? SchAmendApprovalDate { get; set; }
    public string? EntryDate { get; set; }
    public int EntryID { get; set; }
    public string? ModeOfTransport { get; set; }
    public string? OrderPriority { get; set; }
    public string? EnterByMachineName { get; set; }

    public IList<SelectListItem> OrderPriorityList
    {
        get => _OrderPriority;
        set => _OrderPriority = value;
    }

    public IList<SaleScheduleGrid>? SaleScheduleList { get; set; }

    public string? SchAmendmentDate { get; set; }

    public int SchAmendmentNo { get; set; }

    public string? ScheduleDate { get; set; }
    public string? UpdatedByName { get; set; }
    public string? CreatedByName { get; set; }

    public string ScheduleNo { get; set; }
    public string? SchEffFromDate { get; set; }
    public string? SchEffTillDate { get; set; }
    public string? SOCloseDate { get; set; }
    public string? SODate { get; set; }
    public string? ManualMonthSplit { get; set; }

    public int SONO { get; set; }

    public IList<TextValue>? SONOList { get; set; }

    public int SOYearCode { get; set; }

    public string? TentetiveConfirm { get; set; }

    public IList<SelectListItem> TentetiveConfirmList
    {
        get => _TentetiveConfirm;
        set => _TentetiveConfirm = value;
    }

    public int UID { get; set; }
    public string UserName { get; set; }
    public int YearCode { get; set; }
    public string FromDateBack { get; set; }
    public string ToDateBack { get; set; }
    public string DashboardTypeBack { get; set; }
    public string CustomerNameBack { get; set; }
    public string CustomerOrderNoBack { get; set; }
    public string SonoBack { get; set; }
    public string PartCodeBack { get; set; }
    public string ItemNameBack { get; set; }
}

[Serializable]
public class SSDashboard : SaleScheduleDashboard
{
    public string? CustOrderNo { get; set; }
    public string? FromDate { get; set; }
    public string? ItemName { get; set; }
    public string? PartCode { get; set; }
    public string? SummaryDetail { get; set; }
    public IList<TextValue>? SONOList { get; set; }
    public string? ToDate { get; set; }
}

[Serializable]
public class Root
{
    public List<Result>? Result { get; set; }
    public int StatusCode { get; set; }
    public string? StatusText { get; set; }

}