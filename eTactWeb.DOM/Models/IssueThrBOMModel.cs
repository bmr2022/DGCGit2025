using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.DOM.Models
{
    public class IssueThrBom : IssueThrBomDetail
    {
        public int EntryId { get; set; }
        public int YearCode { get; set; }
        public string? FromDate { get; set; }
        public string? ToDate { get; set; }
        public string EntryDate { get; set; }
        public string PreFix { get; set; }
        public string IssueSlipNo { get; set; }
        public string? IssueDate { get; set; }
        public string? WONO { get; set; }
        public int? WOYearCode { get; set; }
        public string? WODate { get; set; }
        public int WCID { get; set; }
        public string? WorkCenter { get; set; }
        public int? IssuedByEmpCode { get; set; }
        public string? IssuedByEmpName { get; set; }
        public int RecByEmpCode { get; set; }
        public string? RecByEmpCodeName {  get; set; }
        public string? MachineCode { get; set; }
        public int Uid { get; set; }
        public string? CC { get; set; }
        public DateTime ActualEntrydate { get; set; }
        public int ActualEnteredBy { get; set; }
        public string? ActualEnteredByName { get; set; }
        public DateTime LastUpdationDate { get; set; }
        public int LastupdatedBy { get; set; }
        public string? LastupdatedByName { get; set; }
        public string? ReqNo { get; set; }
        public string? ReqDate { get; set; }
        public int ReqyearCode { get; set; }
        public string? JobCardNo { get; set; }
        public int? JobYearCode { get; set; }
        public string? JobCardDate { get; set; }
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
        public IList<TextValue>? EmployeeList { get; set; }
        public IList<IssueThrBomDetail>? ItemDetailGrid { get; set; }
        public IList<IssueThrBomFGData>? FGItemDetailGrid { get; set; }

    }
    public class IssueThrBomDetail : TimeStamp
    {
        // just for sending
        public string? ReqNo1 { get; set; }
        public string? ReqDate1 { get; set; }
        public int ReqyearCode1 { get; set; }
        public int EntryId { get; set; }
        public int YearCode { get; set; }
        public int seqno { get; set; }
        public int FGItemCode { get; set; }
        public int ItemCode { get; set; } //RMItemCode
        public string? IssuedDate { get; set; }
        public string ItemName { get; set; } //RMItemName
        public string PartCode { get; set; } //RMPartCode
        public decimal ReqQty { get; set; }
        public decimal AltReqQty { get; set; }
        public int StoreId { get; set; }
        public string StoreName { get; set; }
        public string BatchNo { get; set; }
        public string uniqueBatchNo { get; set; }
        public decimal IssueQty { get; set; }
        public decimal AltIssueQty { get; set; }
        public decimal PendQty { get; set; }
        public string Unit { get; set; }
        public decimal LotStock { get; set; }
        public decimal TotalStock { get; set; }
        public decimal AltQty { get; set; }
        public string AltUnit { get; set; }
        public decimal Rate { get; set; }
        public string Remark { get; set; }
        public int WCId { get; set; }
        public string WorkCenter { get; set; }
        public int AltItemCode { get; set; }
        public string? AltItemName {  get; set; }
        public string? AltPartCode {  get; set; }

        public int CostCenterId { get; set; }
        public string ItemSize { get; set; }
        public string ItemColor { get; set; }
        public float? StdPacking { get; set; }
        public string? IssuedAlternateItem { get; set; }
        public int? OriginalItemCode { get; set; }
        public string? ProjectNo { get; set; }
        public int? ProjectYearCode { get; set; }
        public string? WONO { get; set; }
        public int WOYearCode { get; set; }
        public string WoDate { get; set; }
        public string jcDate { get; set; }
        public string MachineCodee { get; set; }
        public float WipStock { get; set; }
    }

    public class IssueThrBomFGData
    {
        public int Seqno { get; set; }
        public int EntryId { get; set; }
        public int YearCode { get; set; }
        public string EntryDate { get; set;  }
        public string? WONO { get; set; }
        public int WOYearCode { get; set; }
        public int FGItemCode { get; set; }
        public string? FGItemName { get; set; }
        public string? FGPartCode { get; set; }
        public string? Unit { get; set; }
        public decimal FGQty { get; set; }
        public int BOMNO { get; set; }
        public string BOMDate { get; set; }
        public decimal FGStockINStore { get; set; }
        public int IssueFromStoreID { get; set; }
        public string Remark { get; set; }
        public int WCID { get; set; }
        public string ReqNo { get; set; }
        public int ReqYearCode { get; set; }
        public string ReqDate { get; set; }
        public string? ProjectNo { get; set; }
        public int ProjectYearCode { get; set; }
    }

    public class IssueThrBomDashboard : IssueThrBomMainDashboard
    {
        public string? ReqNo { get; set; }
        public string? FGItemName { get; set; }
        public string? FGPartCode { get; set; }
       // public string? WorkCenterDescription { get; set; }
        public string? FromDate1 { get; set; }
        public string? ToDate1 { get; set; }
        public string? SearchBox { get; set; }
        public string DashboardType { get; set; }
        public string IssueSlipNo { get; set; }
        public string WCName { get; set; }
    }

    public class IssueThrBomMainDashboard
    {      
        public string Mode { get; set; }
        public int EntryId { get; set; }
        public int YearCode { get; set; }
        public string ReqNo { get; set; }
        public string ReqDate { get; set; }
        public int ReqYearCode { get; set; }
        public string FGItemName { get; set; }
        public string FGPartCode { get; set; }
        public string IssueSlipno { get; set; }
        public string IssueDate { get; set; }
        public string RMItemName { get; set; }       
        public string RMPartCode { get; set; }
        public float ReqQty { get; set; }
        public float IssueQty { get; set; }
        public string rmUnit { get; set; }
        public float PendQty { get; set; }
        public float AltReqQty { get; set; }
        public float AltIssueQty { get; set; }
        public string AltUnit { get; set; }
        public string BatchNo { get; set; }       
        public string uniquebatchNo { get; set; }
        public float lotStock { get; set; }
        public float TotalStock { get; set; }
        public string IssuedAlternateItem { get; set; }
        public string WorkCenter { get; set; }
        public string jobCardNo { get; set; }
        public string JobcardDate { get; set; }
        public string OrginalItemName { get; set; }
        public string OriginalPartCode { get; set; }                
        public string RMRemark { get; set; }
        public string itemsize { get; set; }
        public string itemcolor { get; set; }       
        public string WONO { get; set; }       
        public string WODate { get; set; }       
        public string Remark { get; set; }       
        public string ActENterdByEmpName { get; set; }        
        public string ActENterdByEmpCode { get; set; }        
        public string ActualEntryDate { get; set; }        
        public string UpdatedEmpName { get; set; }        
        public string UpdatedByEmpcode { get; set; }    
        public int RecEmpCode {  get; set; }
        public string? RecEmpByCodeName {  get; set; }
        public int IssueByEmpCode { get; set; }
        public IList<IssueThrBomMainDashboard>? IssueThrBOMDashboard { get; set; }
    }

}

