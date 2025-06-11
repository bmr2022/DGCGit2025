using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eTactWeb.DOM.Models
{
    public class SaleOrderAmendHistoryModel
    {
        public string? FromDate { get; set; }
        public string? ReportType { get; set; }
        public string? Dashboardflag { get; set; }
        public string? ToDate { get; set; }
        public string? flag { get; set; }
        public int? Accountcode { get; set; }
        public string? partCode { get; set; }
        public string? ItemCode { get; set; }
        public string? SOno { get; set; }
        public string? PONo { get; set; }
        public IList<SaleOrderAmendHistoryDetail>? SaleOrderAmendHistoryDetail { get; set; }

    }
    public class SaleOrderAmendHistoryDetail
    {
        public string? VendorName { get; set; }
        public string? SONO { get; set; }
        public DateTime? SODate { get; set; }
        public DateTime? WEF { get; set; }
        public DateTime? SOClosedate { get; set; }
        public string? OrderType { get; set; }
        public string? SOType { get; set; }
        public string? SOFor { get; set; }
        public string? AmmNo { get; set; }
        public DateTime? AmmEffDate { get; set; }
        public string? PartCode { get; set; }
        public string? ItemName { get; set; }
        public string? HSNNo { get; set; }
        public decimal? SOQty { get; set; }
        public string? Unit { get; set; }
        public decimal? Rate { get; set; }
        public decimal? DiscPer { get; set; }
        public decimal? DiscRs { get; set; }
        public decimal? Amount { get; set; }
        public decimal? OldRate { get; set; }
        public string? Remark { get; set; }
        public string? AmmendmentReason { get; set; }
        public decimal? RateInOtherCurr { get; set; }
        public string? RateApplicableOnUnit { get; set; }
        public decimal? AltSOQty { get; set; }
        public string? AltUnit { get; set; }
        public string? ShippingAddress { get; set; }
        public decimal? BasicAmount { get; set; }
        public decimal? NetAmount { get; set; }
        public int SOAmendEntryID { get; set; }
        public int SOAmendYearCode { get; set; }
        public string? AmendSO { get; set; }
        public int AmendSOSeq { get; set; }
        public int SOEntryId { get; set; }
        public int SOYearCode { get; set; }
        public string SOCanceled { get; set; }
        public string SOComplete { get; set; }
        public string Active { get; set; }
        public int AccountCode { get; set; }
    }
}
