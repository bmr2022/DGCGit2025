using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.DOM.Models
{
    public class IssueWithoutBom : IssueWithoutBomDetail
    {
        public string RecDept { get; set; }
        public int EntryId { get; set; }
        public int YearCode { get; set; }
        public string? FromDate { get; set; }
        public string? ToDate { get; set; }
        public string EntryDate { get; set; }
        public string EntryTime { get; set; }
        public string PreFix { get; set; }
        public string IssueSlipNo { get; set; }
        public string? IssueDate { get; set; }
        public int IssuedByDepCode { get; set; }
        public int? IssuedByEmpCode { get; set; }
        public string? IssuedByEmpName { get; set; }
        public int RecDepCode { get; set; }
        public int? RecByEmpCode { get; set; }
        public string? MachineCode { get;set; }
        public string Remark { get; set; }
        public int Uid { get; set; }
        public string CC { get; set; }
        public DateTime ActualEntrydate { get; set; }
        public int ActualEnteredBy { get; set; }
        public string? ActualEnteredByName { get; set; }
        public DateTime LastUpdationDate { get;set; }
        public int LastupdatedBy { get; set; }
        public string? LastupdatedByName { get; set; }
        public string? ReqNo { get; set; }
        public string? ReqDate { get; set; }
        public int ReqyearCode { get; set; }
        public string FromDateBack { get; set; }
        public string ToDateBack { get; set; }
        public string REQNoBack { get; set; }
        public string PartCodeBack { get; set; }
        public string ItemNameBack { get; set; }
        public string WorkCenterBack { get; set; }
        public string DeptNameBack { get; set; }

        public string DashboardTypeBack { get; set; }
        public string GlobalSearchBack { get; set; }
        public int FromStoreBack { get; set; }
        public string BackFlag { get; set; }
        public string? ReqComplated { get; set; }
        public string? ReqCanceled { get; set; }
        public IList<TextValue>? EmployeeList { get; set; }
        public IList<IssueWithoutBomDetail>? ItemDetailGrid { get; set; }

    }
    public class IssueWithoutBomDetail : TimeStamp
    {
        public int EntryId { get; set; }
        public int YearCode { get; set; }
        public int seqno { get; set; }
        public string? ReqNo1 { get; set; }
        public string? ReqDate1 { get; set; }
        public string?  ReqyearCode1 { get; set; }
        public int ReqEntryId {  get; set; }
        public int ItemCode { get; set; }
        public string? IssuedDate { get; set; }
        public string?  ItemName { get; set; }
        public string? DeptName { get; set; }
        public string? PartCode { get; set; }
        public string? TransactionDate { get; set; }
        public decimal ReqQty { get; set; }
        public int StoreId { get; set; }
        public string? StoreName { get; set; }
        public string? BatchNo { get; set; }
        public string? uniqueBatchNo { get; set; }
        public decimal IssueQty { get; set; }
        public string? Unit { get; set; }
        public decimal LotStock { get; set; }
        public decimal TotalStock { get; set; }
        public decimal AltQty { get; set; }
        public string AltUnit { get; set; }
        public decimal Rate { get; set; }
        public string Remark { get; set; }
        public int WCId { get; set; }
        public string? WorkCenter { get; set; }
        public int AltItemCode { get; set; }
        public int CostCenterId { get; set; }
        public string? ItemSize { get; set; }
        public string? ItemColor { get; set; }
        public string? ProjectNo { get; set; }
        public int ProjectYearCode { get; set; }
        public string? MachineCodee { get; set; }
        public float? StdPacking { get; set; }
        public string? IssuedAlternateItem { get; set; }
        public int? OriginalItemCode { get; set; }
        public int ReqDepartmentID { get; set; }
        public string? ReqDept { get; set;}
        public string? ReqItemCancel { get; set;}

        public int IssWOBOMEntryId { get; set; }
        public int IssWOBOMYearCode { get; set; }
        public string? IssWOBOMEntryDate { get; set; }
        public string? PreFix { get; set; }
        public string? IssWOBOMSlipNo { get; set; }
        public string? IssWOBOMIssueDate { get; set; }
        public int IssuedByEmpCode { get; set; }
    }
    public class IssueWithoutBomDashboard : IssueWOBomMainDashboard
    {
        public string? ReqNo { get; set; }
        public string? Item_Name { get; set; }
        public string? PartCode { get; set; }
        public string? WorkCenterDescription { get; set; }
        public string? FromDate1 { get; set; }
        public string? ToDate1 { get; set; }
        public string Searchbox { get; set; }
    }

    public class IssueWOBomMainDashboard
    {
        public int EntryId { get; set; }
        public int YearCode { get; set; }
        public string Mode { get; set; }
        public string ReqNo { get; set; }
        public string ReqDate { get; set; }
        public string Item_Name { get; set; }
        public string PartCode { get; set; }
        public int ReqYearCode { get; set; }
        public string IssueSlipNo { get; set; }
        public string IssueDate { get; set; }
        public string EntryTime { get; set; }
        public string? DashboardType { get; set; }
        public string WorkCenterDescription { get; set; }
        public string RecDepartment { get; set; }
        public int ActualEnteredBy { get; set; }
        public decimal IssueQty { get; set; }
        public string? MachineCode { get; set; }
        public int TotalRecords { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public IList<IssueWOBomMainDashboard>? IssueWOBOMDashboard { get; set; }
    }

}
