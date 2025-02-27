using System;
using System.Collections.Generic;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.DOM.Models
{
    public class ReturnFromDepartmentMainModel : ReturnFromDepartmentDetail
    {
        public int EntryId { get; set; }
        public string? CreatedByName { get; set; }
        public int YearCode { get; set; }
        public string? EntryDate { get; set; }
        public string? FinToDate { get; set; }
        public string? FinFromDate { get; set; }
        public string SlipNo { get; set; }
        public int? ReturnByEmpId { get; set; }
        public int? ReceivedByEmpId { get; set; }
        public int? DeptId { get; set; }
        public string? ReturnDate { get; set; }
        public int? ReturnByDepartment { get; set; }
        public int? ReturnByEmployee { get; set; }
        public string? UpdatedByName { get; set; }

        //List binding
        public IList<TextValue>? DepartmentList { get; set; }
        public IList<TextValue>? EmployeeList { get; set; }
        public IList<TextValue>? BatchNoList { get; set; }
        public IList<TextValue?> UniqueBatchNoList { get; set; }
        public IList<TextValue>? AssetPartCodeList { get; set; }
        public IList<TextValue>? AssetItemsList { get; set; }

        public IList<ReturnFromDepartmentDetail> ReturnDetailGrid { get; set; }

        public string EntryByMC { get; set; }
        public string CreatedBy { get; set; }
        public string LastUpdatedBy { get; set; }
        public string UpdateBy { get; set; }
        public string UpdateOn { get; set; }
        public string Approved { get; set; }
        public string ApproveBy { get; set; }
        public string ReqDate { get; set; }
    }

    public class ReturnFromDepartmentDetail : TimeStamp
    {
        public int SeqNo { get; set; }
        public int EntryId { get; set; }
        public int YearCode { get; set; }
        public float? CurrentRate { get; set; }
        public string PartCode { get; set; }
        public string ItemName { get; set; }
        public int ItemCode { get; set; }
        public int Qty { get; set; }
        public string? ReturnnDate { get; set; }
        public string BatchNo { get; set; }
        public string UniqueBatchNo { get; set; }
        public int ProdValue { get; set; }
        public string IDMark { get; set; }
        public string CC { get; set; }
        public string ReasonOfReturn { get; set; }
        public string Damaged { get; set; } = "No";
        public string DamageDetail { get; set; }
        public string Pic1 { get; set; }
        public string Pic2 { get; set; }
        public string Pic3 { get; set; }
        public string Pic4 { get; set; }
        public IList<ReturnFromDepartmentDetail> ReqDetailGrid { get; set; }
    }

    public class RDMDashboard
    {
        public string? Mode { get; set; }
        //public string REQNo { get; set; }
        public string RetFromDepEntrydate { get; set; }
        public string RetFromDepSlipNo { get; set; }
        public string RetFromDepActualReturnDate { get; set; }
        public string ReturnByEmpName { get; set; }
        public string DeptName { get; set; }
        public string Remarks { get; set; }
        public string ActualEntryDate { get; set; }
        public string ActualEntryByEmpName { get; set; }
        public string LastUpdatedByEmpName { get; set; }
        public string LastUpdatedDate { get; set; }
        public string Approved { get; set; }
        public string ReceivedByEmpName { get; set; }
        public string ApprovalDate { get; set; }
        public string EntryByMachineName { get; set; }
        public int RetFromDepEntryId { get; set; }
        public int RetFromDepYearCode { get; set; }
        public decimal PurchRate { get; set; }
        public decimal CurrentRate { get; set; }
        //public string ItemName { get; set; }
        //public string PartCode { get; set; }
        //public decimal? Qty { get; set; }
        //public string? Unit { get; set; }
        //public float? AltQty { get; set; }
        //public string? AltUnit { get; set; }
        //public float? PendQty { get; set; }
        //public string? Location { get; set; }
        //public string? BinNo { get; set; }
        //public string CreatedBy { get; set; }
        //public string UpdatedBy { get; set; }
        public IList<RDMDashboard>? RetFromDeptMainDashboard { get; set; }

    }

    public class RetFromDepMainDashboard : RDMDashboard
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
        public string WOYearcode { get; set; }
        public int EntryId { get; set; }
        public int YearCode { get; set; }
        public decimal TotalReqQty { get; set; }
        public decimal TotalPendQty { get; set; }
        public string ReqDate { get; set; }
        public string? CC { get; set; }
        public string? SearchBox { get; set; }
        public string? FromDate1 { get; set; }
        public string? ToDate1 { get; set; }
    }
}
