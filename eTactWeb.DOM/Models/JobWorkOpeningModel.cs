using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.DOM.Models
{
    public class JobWorkOpeningModel : TimeStamp
    {
        public int? SeqNo { get; set; }
        public int EntryID { get; set; }
        public int YearCode { get; set; }
        public string TransactionType { get; set; }
        public string IssJWChallanNo { get; set; }
        public string? IPAddress { get; set; }
        public int IssChalanEntryId { get; set; }
        public int IssChallanYearcode { get; set; }
        public string Isschallandate { get; set; }
        public int Accountcode { get; set; }
        public string AccountName { get; set; }
        public string? EntryDate { get; set; }
        public decimal OpnIssQty { get; set; }
        public decimal Rate { get; set; }
        public string unit { get; set; }
        public decimal Amount { get; set; }
        public decimal RecQty { get; set; }
        public decimal pendqty { get; set; }
        public int ScrapItemCode { get; set; }
        public string ScrapPartCode { get; set; }
        public string ScrapItemName { get; set; }
        public decimal ScrapQty { get; set; }
        public decimal PendScrapToRec { get; set; }
        public string BomStatus { get; set; }
        public string BomType { get; set; }
        public int RecItemCode { get; set; }
        public string RecItemName { get; set; }
        public string RecPartCode { get; set; }
        public int ProcessId { get; set; }
        public decimal ChallanQty { get; set; }
        public string BatchWise { get; set; }
        public string BatchNo { get; set; }
        public string UniqueBatchNo { get; set; }
        public string cc { get; set; }
        public int UID { get; set; }
        public int EnteredByEmpId { get; set; }
        public string Closed { get; set; }
        public int BomNo { get; set; }
        public string BomDate { get; set; }
        public int IssueQty { get; set; }
        //public string ChallanNo { get; set; }
        //public string ChallanYear { get; set; }
        //public string ChallanDate { get; set; }
        public int? ItemCode { get; set; }
        public string PartCode { get; set; }
        public string ItemName { get; set; }
        public string Mode { get; set; }
        public string MachineName { get; set; }
        public string FinStartDate { get; set; }
        public string? ActualEnteredByName { get; set; }
        public string? UpdatedByName { get; set; }
        public string? IsCustomerJobWork { get; set; }
        public string? IsRGPChallan { get; set; }
        public string? ClosedChallan { get; set; }
        public string? CloseChallandate { get; set; }
        public int? CloseChallanByEmpId { get; set; }
        public string? RGPNRGP { get; set; }
        public string? ChallanType { get; set; }
        public string? OpeningType { get; set; }

        public IList<JobWorkOpeningModel>? ItemDetailGrid { get; set; }
    }

    public class JobWorkOpeningDashboard : JobWorkOpeningModel
    {
        public string? FromDate { get; set; }
        public string? ToDate { get; set; }
        public string FromDate1 { get; set; }
        public string ToDate1 { get; set; }
        public string PartCode { get; set; }
        public string ItemName { get; set; }
        public int ItemCode { get; set; }
        public string VendoreName { get; set; }
        public int AccountCode { get; set; }
        public string IssJWChallanNo { get; set; }
        public string SearchBox { get; set; }
        public string SummaryDetail { get; set; }        
        public string OpeningType { get; set; }        
        public IList<DashboardGrid>? DashboardGrid { get; set; }
        public IList<DetailDashboardGrid>? DetailDashboardGrid { get; set; }

    }

    public class DetailDashboardGrid
    {
        public string VendoreName { get; set; }
        public string IssJWChallanNo { get; set; }
        public int IssChallanYearcode { get; set; }
        public string Isschallandate { get; set; }
        public string EntryDate { get; set; }
        public string PartCode { get; set; }
        public string ItemName { get; set; }
        public int ItemCode { get; set; }
        public int OpnIssQty { get; set; }
        public int Rate { get; set; }
        public string unit { get; set; }
        public int Amount { get; set; }
        public int RecQty { get; set; }
        public int pendqty { get; set; }
        public string RecPartCode { get; set; }
        public string RecItemName { get; set; }
        public string ScrapPartCode { get; set; }
        public string? ScrapItemName { get; set; }
        public int ScrapQty { get; set; }
        public string PendScrapToRec { get; set; }
        public string BomStatus { get; set; }
        public string ProcessName { get; set; }
        public int ChallanQty { get; set; }
        public string BatchNo { get; set; }
        public string UniqueBatchNo { get; set; }
        public string Branch { get; set; }
        public int Emp_id { get; set; }
        public string? Empname { get; set; }
        public string Closed { get; set; }
        public int IssChalanEntryId { get; set; }
        public int VJOBOPENEntryID { get; set; }
        public int VJOBOPENYearCode { get; set; }
        public string ActualEntryByEmp { get; set; }
        public string LastUpdatedByEmp { get; set; }
        public string EnteredByMachine { get; set; }
        public string Mode { get; set; }
        public string repotype { get; set; }
        public int uid { get; set; }
        public int BOMNO { get; set; }
        public string BOMDate { get; set; }
        public string OpeningType { get; set; }
        public int Accountcode { get; set; }

    }

    public class DashboardGrid
    {
        public string VendoreName { get; set; }
        public string IssJWChallanNo { get; set; }
        public int IssChallanYearcode { get; set; }
        public string Isschallandate { get; set; }
        public string EntryDate { get; set; }
        public string Branch { get; set; }
        public int Emp_id { get; set; }
        public int UID { get; set; }
        public string Empname { get; set; }
        public string Closed { get; set; }
        public int IssChalanEntryId { get; set; }
        public int VJOBOPENEntryID { get; set; }
        public int VJOBOPENYearCode { get; set; }
        public string ActualEntryByEmp { get; set; }
        public string LastUpdatedByEmp { get; set; }
        public string EnteredByMachine { get; set; }
        public string Mode { get; set; }
        public string repotype { get; set; }
        public string OpeningType { get; set; }        
        public string ClosedChallan { get; set; }
        public string CloseChallandate { get; set; }
        public string? RGPNRGP { get; set; }
        public string? ChallanType { get; set; }
        public string? PartCode { get; set; }
        public string? ItemName { get; set; }
        public int? ItemCode { get; set; }
        public int? Accountcode { get; set; }
    }
}
