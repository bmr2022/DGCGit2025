using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.DOM.Models
{
    public class RequisitionWithoutBOMModel : RequisitionDetail
    {
        public string? EntryByMachineName { get; set; }
        public string? CreatedByName { get; set; }
        public string? UpdatedByName { get; set; }
        public string? FinFromDate { get; set; }
        public string? FinToDate { get; set; }
        public int EntryId { get; set; }
        public int YearCode { get; set; }
        public string Prefix { get; set; }
        public string EntryDate { get; set; }
        public string REQNo { get; set; }
        public string ReqDate { get; set; }
        public string ReqTime { get; set; }
        public int FromDepartmentId { get; set; }
        public string WONo { get; set; }
        public int WoYearCode { get; set; }
        public string? WODate { get; set; }
        public int MachineId { get; set; }
        public int WorkCenterId { get; set; }
        public string Remarks { get; set; }
        public string ReqReason { get; set; }
        public int UID { get; set; }
        public string CC { get; set; }
        public string Completed { get; set;  }
        public string Cancel { get; set; }
        public string CancelDate { get; set; }
        public string CancelReason { get; set; }
        public int EnteredBy { get;set; }
        public int RequByEmpId { get; set; }
        public string NeedApproval { get; set; }
        public string Approved { get; set; }
        public string ApproveDate { get; set; }
        public int ApproveBy { get; set; }
        public int LineRejEntryId { get; set; }
        public int LineRejYearCode { get; set; }
        public int? LastUpdatedBy { get; set; }
        public string? lastUpdationDate { get; set; }
        public bool IC1 { get; set; }
        public string? FromDate { get; set; }
        public string? ToDate { get; set;  }
        public string FromDateBack { get; set; }
        public string ToDateBack { get; set; }
        public string REQNoBack { get; set; }
        public string PartCodeBack { get; set; }
        public string ItemNameBack { get; set; }
        public string WorkCenterBack { get; set; }
        public string WorkOrderNoback { get; set; }
        public string DeptNameBack { get; set; }
        public string DashboardTypeBack { get; set; }
        public string GlobalSearchBack { get; set; }
        public IList<RequisitionDetail> ReqDetailGrid { get; set; }

        //List Binding

        public IList<TextValue>? BranchList { get; set; }
        public IList<TextValue>? ProjectList { get; set; }
        public IList<TextValue>? DepartmentList { get; set; }
        public IList<TextValue>? CostCenterList { get; set; }
        public IList<TextValue>? EmployeeList { get; set; }
        public IList<TextValue>? MachineList { get; set; }
        public IList<TextValue>? StoreList { get; set; }


        //Detail Table
        public int SeqNo { get; set; }
        public int ItemCode { get; set; }
        public string ItemName { get; set; }
        public string PartCode { get; set; }
        public string Unit { get; set; }
        public decimal Qty { get; set; }
        public string AltUnit { get; set; }
        public decimal AltQty { get; set; }
        public string ItemModel { get; set; }
        public string ItemSize { get; set; }
        public string ExpectedDate { get; set; }
        public string Remark { get; set; }
        public decimal PendQty { get; set; }
        public decimal PendAltQty { get; set; }
        public int? StoreId { get; set; }
        public decimal TotalStock { get; set; }
        public string Cancle { get; set; }
        public string ProjectNo { get; set; }
        public int ProjectYearCode { get; set; }
        public int? CostCenterId { get; set; }
        public string ItemLocation { get; set; }
        public string ItemBinRackNo { get; set; }

    }
    public class RequisitionDetail : TimeStamp
    {
        public int SeqNo { get; set; }
        public int ItemCode { get; set; }
        public string ItemName { get; set; }
        public string PartCode { get; set; }
        public string? Unit { get; set; }
        public decimal? Qty { get; set; }
        public string? AltUnit { get; set; }
        public decimal? AltQty { get; set;}
        public string? ItemModel { get; set; }
        public string? ItemSize { get; set; }
        public string? ExpectedDate { get; set; }
        public string? Remark { get; set; }
        public decimal? PendQty { get; set; }
        public decimal? PendAltQty { get; set; }
        public int? StoreId { get; set; }
        public string? StoreName { get; set; }
        public decimal? TotalStock { get; set; }
        public string? Cancle { get; set; }
        public string? ProjectNo { get; set; }
        public int? ProjectYearCode { get; set; }
        public int? CostCenterId { get; set; }
        public string? CostCenterName { get; set; }
        public string? ItemLocation { get; set; }
        public string ItemBinRackNo { get; set; }


    }
    public class RWBDashboard
    {
        public string? Mode { get; set; }

        public string REQNo { get; set; }

        public string ReqDate { get; set; }
        public string EntryDate { get; set; }
        public string WorkCenter { get; set; }
        public string WONo { get; set; }
        public string BranchName { get; set; }
        public string Reason { get; set; }
        public string Cancel { get; set; }
        public string MachName { get; set; }
        public string WOYearcode { get; set; }
        public int EntryId { get; set; }
        public int YearCode { get; set; }
        public decimal TotalReqQty { get; set; }
        public decimal TotalPendQty { get; set; }
        public string Completed { get; set; }
        public float? Qty { get; set; }
        public string? Unit { get; set; }
        public float? AltQty { get; set; }
        public string? AltUnit { get; set; }
        public float? PendQty { get; set; }
        public string? Location { get; set; }
        public string? BinNo { get; set; }
        public string ItemName { get; set; }
        public string PartCode { get; set; }        
        public string CreatedBy { get; set; }        
        public string UpdatedBy { get; set; }        

        public IList<RWBDashboard>? ReqMainDashboard { get; set; }
    }
    public class ReqMainDashboard : RWBDashboard
    {
        public string REQNo { get; set; }
        public string ItemName { get; set; }
        public string PartCode { get; set; }
        public string EntryDate { get; set; }
        public string WorkCenter { get; set; }
        public string WONo { get; set; }
        public string Reason { get; set; }
        public string Cancel { get; set; }
        public string MachName { get; set; }
        public string WOYearcode{get;set;}
        public int EntryId { get;set;}
        public int YearCode { get;set;}
        public decimal TotalReqQty { get; set; }
        public decimal TotalPendQty { get; set; }
        public string ReqDate { get; set; }
        public string? CC { get; set; }
        public string? SearchBox { get; set; }
        public string? FromDate1 { get; set; }
        public string? ToDate1 { get; set;}
       
    }
}
