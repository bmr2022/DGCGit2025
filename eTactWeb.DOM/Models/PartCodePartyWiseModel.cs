using System.ComponentModel.DataAnnotations;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.DOM.Models
{
    public class PartCodePartyWiseDashboard :TimeStamp
    {
        public string? AccountName { get; set; }
        public DataTable? DTDashboard { get; set; }
        public string? ItemName { get; set; }
        public int? ItemCode { get; set; }
        public string? PartCode { get; set; }
        public IList<TextValue>? ItemNameList { get; set; }
        public IList<TextValue>? PartCodeList { get; set; }
        public string? VendCustPartCode { get; set; }
        public string? VendCustitemname { get; set; }
        public decimal? ItemRate { get; set; }
        public decimal? BusinessPercentage { get; set; }
        public decimal? MOQ { get; set; }
        public int LeadTimeInDays { get; set; }
        public string? ActualEnteredByName { get; set; }
        public string? EntryByMachineName {  get; set; }
        public int SeqNo {  get; set; }
        public string? DashboardType { get; set; }
        public string? Searchbox {  get; set; }
        public IList<PartCodePartyWiseDashboard>? PartCodePartyDashboard { get; set; }

    }

    public class PartCodePartyDashboard : PartCodePartyWiseDashboard
    {
        public string? AccountName { get; set; }

        public string? ItemName { get; set; }
        public string? PartCode { get; set; }

    }


    public class PartCodePartyWiseModel
    { 
        public string? AccountName { get; set; }
        public int AccountCode {  get; set; }
        public int? ItemCode { get; set; }
         public string? ItemName { get; set; }
        public string? PartCode { get; set; }
        public string? VendCustPartCode {  get; set; }
        public string? VendCustitemname { get; set; }
        public string? Mode {  get; set; }
        public decimal? ItemRate { get; set; }
        public decimal? BusinessPercentage { get; set; }
        public decimal? MOQ { get; set; }
        public int LeadTimeInDays { get; set; } 
        public IList<TextValue>? ItemNameList { get; set; }
        public IList<TextValue>? AccountNameList { get; set; }
        public IList<TextValue>? PartCodeList { get; set; }
        public string? PreparedByEmp {  get; set; }
        public string? ActualEnteredByName { get; set; }
        public int ActualEnteredBy { get; set; }
        public string? CreatedOn { get; set; }
        public int UpdatedBy {  get; set; }
        public string? UpdatedOn {  get; set; }
        public int? SeqNo { get; set; }
        public string? UID { get; set; }
        public string? Unit { get; set; }
        public string? UpdatedByName { get; set; }
        public string? EntryByMachineName { get; set; }
        public string? PartCodeBack {  get; set; }
        public string? ItemNameBack {  get; set; }
        public string? VendCustPartCodeBack {  get; set; }
        public string? VendCustitemnameBack {  get; set; }
        public string? AccountNameBack {  get; set; }
        public string? SummaryDetailBack { get; set; }
        public string? GlobalSearchBack { get; set; }
        public IList<PartCodePartyWiseItemDetail>? ItemDetailGrid { get; set; }
    }

    public class PartCodePartyWiseItemDetail : TimeStamp
    {
        public int SeqNo {  get; set; }
        public string? AccountName { get; set; }
        public int AccountCode { get; set; }
        public int ItemCode {  get; set; }
        public string? VendCustitemname {  get; set; }
        public string? VendCustPartCode {  get; set; }
        public decimal ItemRate { get; set; }
        public decimal BusinessPercentage { get; set; }
        public string? ItemName {  get; set; }
        public decimal MOQ {  get; set; }
        public string? PartCode {  get; set; }
        public int LeadTimeInDays {  get; set; }
    }
}