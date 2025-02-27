using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.DOM.Models
{
    public class IndentModel : IndentDetail
    {
        public int EntryId { get; set; }
        public int YearCode { get; set; }
        public string EntryDate { get; set; }
        public string IndentNo { get; set; }
        public string IndentDate { get; set; }
        public string ItemService { get; set; }
        public string BOMIND { get; set; }
        public int BomRevNo { get; set; }
        public int BomItemCode { get; set; }
        public string BomPartCode { get; set; }
        public string BomItemName { get; set; }
        public float Bomqty { get; set; }
        public int IndentorEmpId { get; set; }
        public string IndentorEmpName { get; set; }
        public int DepartmentId { get; set; }
        public string DeptName { get; set; }
        public string CC { get; set; }
        public string Approved { get; set; }
        public int ApprovedbyEmpId { get; set; }
        public string ApprovedDate { get; set; }
        public int UID { get; set; }
        public string IndentRemark { get; set; }
        public string IndentCompleted { get; set; }
        public string canceled { get; set; }
        public string closed { get; set; }
        public string firstapproved { get; set; }
        public string firstapproveddate { get; set; }
        public int firstapprovedby { get; set; }
        public int MRPNO { get; set; }
        public int MRPEntryId { get; set; }
        public int MRPyearcode { get; set; }
        public string MachineNo { get; set; }
        public int LastUpdatedBy { get; set; }
        public string LastUpdatedByName { get; set; }
        public string LastUpdationDate { get; set; }
        public int CreatedBy { get; set; }
        public string CreatedByName { get; set; }
        public string FinFromDate { get; set; }
        public string FinToDate { get; set; }
        public IList<TextValue>? BranchList { get; set; }
        public IList<TextValue>? ProjectList { get; set; }
        public IList<TextValue>? DeptList { get; set; }
        public IList<TextValue>? CostCenterList { get; set; }
        public IList<TextValue>? EmpList { get; set; }
        public IList<TextValue>? MachineList { get; set; }
        public IList<TextValue>? StoreList { get; set; }
        public IList<TextValue>? PartCodeList { get; set; }
        public IList<TextValue>? ItemNameList { get; set; }
        public IList<TextValue>? VendorList { get; set; }
        public List<IndentDetail>? IndentDetails { get; set; }
        public string IndentNoBack { get; set; }
        public string FromDateBack { get; set; }
        public string ToDateBack { get; set; }  
        public string ItemNameBack { get; set; }
        public string PartCodeBack { get; set; }
        public string DashboardTypeBack { get; set; }
        public string DeptNameBack { get; set; }
        public string GlobalSearchBack { get; set; }
    }

    public class IndentDetail : TimeStamp
    {
        public int SeqNo { get; set; }
        public int ItemCode { get; set; }
        public string ItemName { get; set; }
        public string PartCode { get; set; }
        public string PendReqNo { get; set; }
        public int ReqYearCode { get; set; }
        public string Specification { get; set; }
        public float Qty { get; set; }
        public float PendQtyForPO { get; set; }
        public string Unit { get; set; }
        public float AltQty { get; set; }
        public string AltUnit { get; set; }
        public int StoreID { get; set; }
        public string StoreName { get; set; }
        public float TotalStock { get; set; }
        public float PendAltQtyForPO { get; set; }
        public string ReqDate { get; set; }
        public int AccountCode1 { get; set; }
        public int AccountCode2 { get; set; }
        public string AccountName1 { get; set; }
        public string AccountName2 { get; set; }
        public string Model { get; set; }
        public string Size { get; set; }
        public string Color { get; set; }
        public string ItemRemark { get; set; }
        public float ReqQty { get; set; }
        public float Approvalue { get; set; }
        public string ItemDescription { get; set; }
    }

    public class IndentDetailDashboard
    {
        public int EntryId { get; set; }
        public string EntryDate { get; set; }
        public int YearCode { get; set; }
        public string IndentNo { get; set; }
        public string IndentDate { get; set; }
        public string PendReqNo { get; set; }
        public int ReqYearCode { get; set; }
        public string ReqDate { get; set; }
        public float Qty { get; set; }
        public string Unit { get; set; }
        public string StoreName{ get; set; }
        public float TotalStock{ get; set; }
        public float AltQty { get; set; }
        public string AltUnit{ get; set; }
        public string Model { get; set; }
        public string Size { get; set; }
        public string Color { get; set; }
        public string Account_Name { get; set; }
        public string Account_Name2 { get; set; }
        public string ItemDescription { get; set; }
        public string ItemRemark { get; set; }
        public string Specification { get; set; }
        public float PendQtyForPO { get; set; }
        public float PendAltQtyForPO { get; set; }
        public float ApproValue { get; set; }
        public string itemservice { get; set; }
        public string BOMIND { get; set; }
        public int BomItemCode { get; set; }
        public string BomPartCode { get; set; }
        public string BOMtem { get; set; }
        public int IndentorEmpId { get; set; }
        public string IndentCompleted { get; set; }
        public float Bomqty { get; set; }
        public string CC { get; set; }
        public string Approved { get; set; }
        public int ApprovedbyEmpId { get; set; }
        public string ApprovedDate { get; set; }
        public int UID { get; set; }
        public string IndentRemark { get; set; }
        public int MRPNO { get; set; }
        public int MRPEntryId { get; set; }
        public string canceled { get; set; }
        public string closed { get; set; }
        public string firstapproved { get; set; }
        public int firstapprovedby { get; set; }
        public string firstapproveddate { get; set; }
        public string MachineNo { get; set; }
        public int LastUpdatedBy { get; set; }
        public string LastUpdatedByName { get; set; }
        public string LastUpdationDate { get; set; }
        public int CreatedBy { get; set; }
        public string CreatedByName { get; set; }
        public List<IndentDashboard>? IndentDashboardGrid { get; set; }

    }

    public class IndentDashboard : IndentDetailDashboard
    {
        public string IndentNo { get; set; }
        public string DeptName { get; set; }
        public int DepartmentId { get; set; }
        public int ItemCode { get; set; }
        public string PartCode { get; set; }
        public string ItemName { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string SummaryDetail { get; set; }
        public string Searchbox {  get; set; }
    }
}
