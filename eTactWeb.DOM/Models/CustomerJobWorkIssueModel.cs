using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.DOM.Models
{
    public class CustomerJobWorkIssueModel : CustomerJobWorkIssueDetail
    {
        public string Branch { get; set; }
        public int EntryId { get; set; }
        public int YearCode { get; set; }
        public string? IPAddress { get; set; }
        public string? EntryDate { get; set; }
        public string? ChallanNo { get; set; }
        public string? ChallanDate { get; set; }
        public int? RecQty { get; set; }
        public int? IssQty { get; set; }
        public int? AccPendQty { get; set; }
        public int? PendQty { get; set; }
        public string EntryTime { get; set; }
        public string CustomerAddress { get; set; }
        public string CustomerName { get; set; }
        public string CustomerState { get; set; }
        public string GSTNO { get; set; }
        public string GSTType { get; set; }
        public decimal NetAmount { get; set; }
        public string EWayBill { get; set; }
        public string State { get; set; }
        public string StateCode { get; set; }
        public string JWType { get; set; }
        public float? DistKm { get; set; }
        public string VehicleNo { get; set; }
        public string TransporterName { get; set; }
        public string DispatchFrom { get; set; }
        public string DispatchTo { get; set; }
        public string Types { get; set; }
        public string? FinFromDate { get; set; }
        public string? FinToDate { get; set; }
        public string? ActualEntryDate { get; set; }
        public string? UpdatedByEmp { get; set; }
        public string? Remark { get; set; }
        public string? CC { get; set; }
        public int BillYearCode { get; set; }
        public int? Account_Code { get; set; }
        public string? UpdatedOn { get; set; }
        public string? ItemName { get; set; }
        public IList<TextValue>? CustomerList { get; set; }
        public IList<CustomerJobWorkIssueDetail> CustJWIDetailGrid { get; set; }
        public IList<CustomerJobWorkIssueModel> JWIGrid { get; set; }

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

        // Other Details
        public int? EntryBill { get; set; }
        public string? EntryByEmp { get; set; }
        public int? EntryById { get; set; }
        public string? DateEntry { get; set; }
        public string BillNo { get; set; }
        public int UID { get; set; }
        public decimal TotalAmount { get; set; }
        public string? BillDate { get; set; }
        public int? DeptFromID { get; set; }
        public string? LastUpdatedBy { get; set; }
        public string? LastUpdatedByDate { get; set; }
        public string? EntryMachineName { get; set; }
        public string? IssuedByEmpName { get; set; }
        public string? TimeOfRemovel { get; set; }
        public string? DispatchThrough { get; set; }
       

        public IList<CustJWIssQDashboard>? CustJWIssQDashboard { get; set; }

    }

    public class CustomerJobWorkIssueDetail : TimeStamp
    {
        public int? CustJwIssEntryId { get; set; }
        public int? CustJwIssYearCode { get; set; }
        public int SEQNo { get; set; }
        public string ProduceUnproduce { get; set; }
        public int SONO { get; set; }
        public string GridSONO { get; set; }
        public int SOYear { get; set; }
        public string? SoDate { get; set; }
        public string? SOAmmDate { get; set; }
        public string? SOAmmNo { get; set; }
        public string? CustOrderNo { get; set; }
        public string? SchNo { get; set; }
        public string? SchDate { get; set; }
        public int? SchYearcode { get; set; }
        public string? fromChallanOrSalebill { get; set; }
        public string? ItemAdjustmentRequired { get; set; }

        public string PartCode { get; set; }
        public string ItemName { get; set; }
        public char BOMInd { get; set; }
        public string ChallanNo { get; set; }
        public int ItemCode { get; set; }
        public int StoreId { get; set; }
        public string StoreName { get; set; }
        public string? BatchNo { get; set; }
        public string? UNiqueBatchNo { get; set; }

        public int BatchStock { get; set; }
        public int TotalStock { get; set; }
        public string ItemSize { get; set; }
        public string PacketsDetail { get; set; }
        public int? PendQty { get; set; }
        public int? ChallanQty { get; set; }
        public decimal Rate { get; set; }
        public decimal QriginalRecQty { get; set; }
        public string IdealScrap { get; set; }
        public int ItemAmount { get; set; }
        public int Discountper { get; set; }
        public int DiscountAmt { get; set; }
        public string Process { get; set; }
        public int ProcessId { get; set; }
        public int NoOfCases { get; set; }

        public string OtherDetail { get; set; }
        public string HSNNo { get; set; }
        public decimal Qty { get; set; }
        public string Unit { get; set; }
        public int ChallanAdjustRate { get; set; }
        public int StdPacking { get; set; }
        public int BOMNO { get; set; }
        public string BOMname { get; set; }
        public string Bomdate { get; set; }
        public string AltUnit { get; set; }
        public string color { get; set; }
        public int AltQty { get; set; }
        public int YearCode { get; set; }
        public string EntryDate { get; set; }
        public string ChallanDate { get; set; }
        public int Account_Code { get; set; }
        public IList<CustomerJobWorkIssueModel> adjustedData { get; set; }
        //chalanlist
        public long? CustJwRecEntryId { get; set; }
        public long? CustJWRecYearCode { get; set; }
        public string? CustJWRecChallanNo { get; set; }
        public DateTime? CustJWRecEntryDate { get; set; }
        public long? RecItemcode { get; set; }
        public string? RecItemName { get; set; }
       // public long? CustJWIssEntryid { get; set; }
       // public long? CustJWIssYearCode { get; set; }
        public string? CustJWIssChallanNo { get; set; }
        public DateTime? CustJWIssChallanDate { get; set; }
        public long? AccountCode { get; set; }
        public long? FinishItemCode { get; set; }
        public float? AdjQty { get; set; }
        public string? CC { get; set; }
        public long? UID { get; set; }
        public string? AdjFormType { get; set; }
        public DateTime? TillDate { get; set; }
        public float? TotalSQty { get; set; }
        public float? BOMQty { get; set; }
        public string? Through { get; set; }
        public float? QtyToBeRec { get; set; }
        public DateTime? BOMRecDate { get; set; }
        public int? BomRevNo { get; set; }
       // public long? ProcessID { get; set; }
        public float? IssQty { get; set; }
        public float? TotalAdjQty { get; set; }
        public float? TotalIssQty { get; set; }
        public float? TotalRecQty { get; set; }
        public long? RunnerItemCode { get; set; }
        public long? ScrapItemCode { get; set; }
        public float? IdealScrapQty { get; set; }
        public float? IssuedScrapQty { get; set; }
        public string? PreRecChallanNo { get; set; }
        public float? ScrapQtyAgainstRecQty { get; set; }
        public string? Recbatchno { get; set; }
       // public int? RecItemCode { get; set; }
        public string? RecPartCode { get; set; }
        public string? Recuniquebatchno { get; set; }
        public string? Issbatchno { get; set; }
        public string? IssPartCode { get; set; }
        public string? IssItemName { get; set; }
        public string? Issuniquebatchno { get; set; }
        public string? ScrapAdjusted { get; set; }
    }

    public class CustJWIssQDashboard : CustomerJobWorkIssueModel
    {
        public string? FromDate { get; set; }
        public string? PartCode { get; set; }
        public string ItemName { get; set; }
        public string? BOMNO { get; set; }
        //right
        public IList<TextValue>? ChallanList { get; set; }
        public string? ToDate { get; set; }
        public string? Reporttype { get; set; }
        public string? FromDate1 { get; set; }
        public string? ToDate1 { get; set; }
        public string CustomerName { get; set; }
        public string? SchDate { get; set; }
        public int? SchYearcode { get; set; }
        public IList<CustJWIssQDashboard> CustJWIssQDashboardGrid {  get; set; }
    }
}
