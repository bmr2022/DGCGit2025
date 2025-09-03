using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.DOM.Models;
[Serializable]
public class GateInwardDashboard
{

    public string? Gateno { get; set; }
    public string? GDate { get; set; }
    public string? VendorName { get; set; }
    public string? address { get; set; }
    public string? Invoiceno { get; set; }
    public string? InvoiceDate { get; set; }
    public string? DocName { get; set; }
    public string? CompGateNo { get; set; }
    public string? POTypeServItem { get; set; }
    public string? entryId { get; set; }
    public int? yearcode { get; set; }
    public string? MrnNo { get; set; }
    public int? MRNYEARCODE { get; set; }
    public string? MRNDate { get; set; }
    public string? EnteredBy { get; set; }
    public string? UpdatedBy { get; set; }
    public string? DashboardType { get; set; }
    public float TareWeight { get; set; }
    public float GrossWeight { get; set; }
    public float NetWeight { get; set; }
    public float Qty { get; set; }
    public float Rate { get; set; }
    public float AltQty { get; set; }
    public float PendPOQty { get; set; }
    public float AltPendQty { get; set; }
    public string? ShowPoTillDate { get; set; }
    public string? CC { get; set; }
    public string? PoNo { get; set; }
    public string? Remarks { get; set; }
    public int ProcessId { get; set; }
    public string? ProcessName { get; set; }
    public string? SaleBillNo { get; set; }
    public int? SaleBillYearCode { get; set; }
    public decimal? SaleBillQty { get; set; }
    public string? AgainstChallanNo { get; set; }
    public decimal? ChallanQty { get; set; }
    public int? AgainstChallanYearCode { get; set; }
    public string? SupplierBatchNo { get; set; }
    public decimal? ShelfLife { get; set; }
    public int? SeqNo { get; set; }
    public string? Types { get; set; }
    public string? OtherDetail { get; set; }
    public decimal? PendQty { get; set; }
    public int PoYearCode { get; set; }
    public int SchYearCode { get; set; }
    public string? POtype { get; set; }
    public string? SchNo { get; set; }
    public string? Unit { get; set; }
    public string? AltUnit { get; set; }
    public string? Size { get; set; }
    public string? Color { get; set; }
    public string? PartCode { get; set; }
    public string? ItemName { get; set; }
    public string? EntryByMachineName { get; set; }
    public string? Summary { get; set; }
    public string FromDate { get; set; }
    public string ToDate { get; set; }
    public int? NoOfBoxes { get; set; }
    public int TotalRecords { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public IList<GateInwardDashboard>? GateDashboard { get; set; }
}
[Serializable]
public class PendingGateInwardDashboard
{
    // Master entry info
    public int seqno { get; set; }
    public string? Gateno { get; set; }
    public string? GDate { get; set; }
    public string? VendorName { get; set; }
    public string? PODate { get; set; }
    public string? SODate { get; set; }
    public int? AccountCode { get; set; }
    public string? address { get; set; }
    public string? Invoiceno { get; set; }
    public string? InvoiceDate { get; set; }
    public string? DocName { get; set; }
    public string? CompGateNo { get; set; }
    public string? POTypeServItem { get; set; }
    public string? entryId { get; set; }
    public int? yearcode { get; set; }
    public string? MrnNo { get; set; }
    public int? MRNYEARCODE { get; set; }
    public string? MRNDate { get; set; }
    public string? EnteredBy { get; set; }
    public string? UpdatedBy { get; set; }

    // Item details
    public string? PartCode { get; set; }
    public string? ItemName { get; set; }
    public string? ItemCode { get; set; }
    public string? Unit { get; set; }
    public string? AltUnit { get; set; }
    public float Rate { get; set; }
    public float Qty { get; set; }
    public float AltQty { get; set; }
    public float PendPOQty { get; set; }
    public float POQty { get; set; }
    public float AltPOQty { get; set; }
    public float AltPendQty { get; set; }
    public decimal? PendQty { get; set; }
    public string? Size { get; set; }
    public string? Color { get; set; }
    public string? ItemSize { get; set; }
    public string? ItemColor { get; set; }

    // Weight info
    public float TareWeight { get; set; }
    public float GrossWeight { get; set; }
    public float NetWeight { get; set; }

    // PO and Schedule Info
    public string? PONo { get; set; }
    public string? SchNo { get; set; }
    public int PoYear { get; set; }
    public int SoYearCode { get; set; }
    public int SchYearCode { get; set; }
    public string? POtype { get; set; }
    public string? POType { get; set; }
    public string? ShowPoTillDate { get; set; }

    // Status Info
    public string? Remarks { get; set; }
    public string? DashboardType { get; set; }
    public string? CC { get; set; }
    public string? EntryByMachineName { get; set; }

    // Challan/Sale Bill
    public string? SaleBillNo { get; set; }
    public int? SaleBillYearCode { get; set; }
    public decimal? SaleBillQty { get; set; }
    public string? AgainstChallanNo { get; set; }
    public decimal? ChallanQty { get; set; }
    public int? AgainstChallanYearCode { get; set; }
    public int? ChallanYearCode { get; set; }

    // Batch / Other
    public string? SupplierBatchNo { get; set; }
    public decimal? ShelfLife { get; set; }
    public int? SeqNo { get; set; }
    public string? Types { get; set; }
    public string? OtherDetail { get; set; }

    // Process Info
    public int ProcessId { get; set; }
    public string? Process { get; set; }
    public string? ProcessName { get; set; }

    // UI/Filter support
    public int? NoOfBoxes { get; set; }
    public string? Summary { get; set; }

    public int TotalRecords { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }

    public IList<PendingGateInwardDashboard>? PendingGateEntryDashboard { get; set; }
}
[Serializable]
public class PendingGateEntryDashboard : PendingGateInwardDashboard
{
    public string? FromDate { get; set; }
    public string? ItemName { get; set; }
    public string? ItemCode { get; set; }
    public string? PartCode { get; set; }
    public string? DocName { get; set; }
    public string? PONO { get; set; }
    public string? ScheduleNo { get; set; }
    public string? SessionYearCode { get; set; }
    public string? Searchbox { get; set; }
    public IList<TextValue>? AccountList { get; set; }
    public IList<TextValue>? DocumentList { get; set; }
    //right
    //public IList<TextValue>? PONOList { get; set; }
    public IList<TextValue>? GateNOList { get; set; }
    public string? ToDate { get; set; }
    public string? FromDate1 { get; set; }
    public string? ToDate1 { get; set; }

}
[Serializable]
public class GateDashboard : GateInwardDashboard
{
    public string? FromDate { get; set; }
    public string? ItemName { get; set; }
    public string? PartCode { get; set; }
    public string? DocName { get; set; }
    public string? PONO { get; set; }
    public string? ScheduleNo { get; set; }
    public string? SessionYearCode { get; set; }
    public string? Searchbox { get; set; }
    //right
    //public IList<TextValue>? PONOList { get; set; }
    public IList<TextValue>? GateNOList { get; set; }
    public string? ToDate { get; set; }
    public string? FromDate1 { get; set; }
    public string? ToDate1 { get; set; }

}


[Serializable()]

public class GateInwardItemDetail : TimeStamp
{
    public string? PoNo { get; set; }
    public int PoYear { get; set; }
    public int PoEntryId {  get; set; }
    public string? PoDate {  get; set; }
    public string POType { get; set; }
    public string SchNo { get; set; }
    public int SchYearCode { get; set; }
    public string? SchDate {  get; set; }
    public int SchEntryId {  get; set; }
    //public DateTime? SchDate { get; set; }
    public int? NoOfBoxes { get; set; }
    public int? ItemCode { get; set; }
    public string PartCode { get; set; }
    public string ItemName { get; set; }
    public string? Unit { get; set; }
    public string? UnitRate { get; set; }
    public decimal Qty { get; set; }
    public decimal Rate { get; set; }
    public string? AltUnit { get; set; }
    public decimal AltQty { get; set; }
    public string? ItemSize { get; set; }
    public string? ItemColor { get; set; }
    public int? docTypeId { get; set; }
    public string? Remarks { get; set; }
    public int ProcessId { get; set; }
    public string ProcessName { get; set; }
    public string? SaleBillNo { get; set; }
    public int? SaleBillYearCode { get; set; }
    public decimal? SaleBillQty { get; set; }
    public string? AgainstChallanNo { get; set; }
    public decimal? ChallanQty { get; set; }
    public string? SupplierBatchNo { get; set; }
    public decimal? ShelfLife { get; set; }
    public int? SeqNo { get; set; }
    public string? OtherDetail { get; set; }
    public decimal? PendQty { get; set; }
    public float? AltPendQty { get; set; }
    public int? AgainstChallanYearcode { get; set; }

}

[Serializable]
public class GateInwardModel : GateInwardItemDetail
{
    public string? FinFromDate { get; set; }
    public string? FinToDate { get; set; }
    public string? EntrybyMachineName { get; set; }
    public int ID { get; set; }
    //public string? Mode { get; set; }
    public int EntryId { get; set; }
    public int YearCode { get; set; }
    public string? GateNo { get; set; }
    public string? CC { get; set; }
    public string? CompGateNo { get; set; }
    [DisplayFormat(DataFormatString = @"{0:yyyy-mm-dd HH:mm:ss}", ApplyFormatInEditMode = false)]
    public DateTime? GDate { get; set; }
    public string? EntryDate { get; set; }
    public string? EntryTime { get; set; }
    public int? AccountCode { get; set; }
    [DisplayFormat(DataFormatString = @"{0:yyyy-mm-dd HH:mm:ss}", ApplyFormatInEditMode = false)]
    public string? InvoiceDate { get; set; }
    public string? Invoiceno { get; set; }
    public string? Transporter { get; set; }
    public string? Truck { get; set; }
    public int? docTypeId { get; set; }
    public string? DriverName { get; set; }
    public string? BiltyNo { get; set; }
    public string? BiltyDate { get; set; }
    public string? PaymentMode { get; set; }
    public string? RefNo { get; set; }
    public string? Remark { get; set; }
    public string? Types { get; set; }
    public decimal? TareWeight { get; set; }
    public decimal? GrossWeight { get; set; }
    public decimal? NetWeight { get; set; }
    public string? Address { get; set; }
    public string? ShowPOTillDate { get; set; }
    public string? ModeOfTransport { get; set; }
    public int? PreparedByEmpId { get; set; }
    public string? PreparedByEmp { get; set; }
    public bool AC1 { get; set; }
    public bool ItemCheck { get; set; }
    public string? PoNo { get; set; }
    public string? NeedPo {  get; set; }
    public int PoYear { get; set; }
    public string? PODate { get; set; }
    public string SchNo { get; set; }
    public int SchYearCode { get; set; }
    public DateTime? SchDate { get; set; }
    public int? ItemCode { get; set; }
    public string? Unit { get; set; }
    public decimal Qty { get; set; }
    public decimal Rate { get; set; }
    public string ? UnitRate { get; set; }
    public string? AltUnit { get; set; }
    public decimal? PendQty { get; set; }

    public int? ActualEnteredBy { get; set; }
    public string? ActualEnteredByName { get; set; }
    public string? UpdatedByName { get; set; }
    public DateTime? ActualEntryDate { get; set; }
    public decimal? AltQty { get; set; }
    public string? UpdatedDate { get; set; }
    public decimal? AltPendQty { get; set; }
    public string? ItemSize { get; set; }
    public string? ItemColor { get; set; }
    public string? OtherDetail { get; set; }

    public string? Remarks { get; set; }
    public int ProcessId { get; set; }
    public IList<TextValue>? ProcessList { get; set; }
    public string? SaleBillNo { get; set; }
    public int? SaleBillYearCode { get; set; }
    public string? SaleBillQty { get; set; }
    public string? AgainstChallanNo { get; set; }
    public decimal? ChallanQty { get; set; }
    public int? AgainstChallanYearCode { get; set; }
    public string? SupplierBatchNo { get; set; }
    public decimal? ShelfLife { get; set; }
    public int? SeqNo { get; set; }
    public string POType { get; set; }
    public string PartCode { get; set; }
    public string ItemName { get; set; }
    public string VendorNameBack { get; set; }
    public string FromDateBack { get; set; }
    public string ToDateBack { get; set; }
    public string GateNoBack { get; set; }
    public string PartCodeBack { get; set; }
    public string ItemNameBack { get; set; }
    public string DocTypeBack { get; set; }
    public string PoNOBack { get; set; }
    public string SchNoBack { get; set; }
    public string GlobalSearchBack { get; set; }
    public string DashboardTypeBack { get; set; }
    public IList<TextValue>? AccountList { get; set; }
    public IList<TextValue>? DocumentList { get; set; }
     public int TotalRecords { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    public IList<GateInwardItemDetail>? ItemDetailGrid { get; set; }

    public IList<TextValue>? PONO { get; set; }


    private IList<SelectListItem> _Type = new List<SelectListItem>()
        {
            new() { Value = "Item", Text = "Item" },
            new() { Value = "Service", Text = "Service" },
        };

    public IList<SelectListItem> TypeList
    {
        get => _Type;
        set => _Type = value;
    }


}




