using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eTactWeb.DOM.Models
{
    public class OrderAmendHistoryModel
    {
        public string VendorName { get; set; }
        public string PONO { get; set; }
        public string? PODate { get; set; }
        public string? WEF { get; set; }
        public string? POClosedate { get; set; }
        public string OrderType { get; set; }
        public string POType { get; set; }
        public string POFor { get; set; }
        public string AmmNo { get; set; }
        public string? AmmEffDate { get; set; }
        public string PartCode { get; set; }
        public string ItemName { get; set; }
        public int ItemCode { get; set; }
        public string HSNNo { get; set; }
        public decimal POQty { get; set; }
        public string Unit { get; set; }
        public decimal Rate { get; set; }
        public decimal DiscPer { get; set; }
        public decimal DiscRs { get; set; }
        public decimal Amount { get; set; }
        public decimal OldRate { get; set; }
        public string Remark { get; set; }
        public string AmmendmentReason { get; set; }
        public decimal RateInOtherCurr { get; set; }
        public string RateApplicableOnUnit { get; set; }
        public decimal AltPOQty { get; set; }
        public string AltUnit { get; set; }
        public string ShippingAddress { get; set; }
        public decimal BasicAmount { get; set; }
        public decimal NetAmount { get; set; }
        public int POAmendEntryID { get; set; }
        public int POAmendYearCode { get; set; }
        public string AmendPO { get; set; }
        public int AmendPOSeq { get; set; }
        public int POEntryId { get; set; }
        public int POYearCode { get; set; }
        public string POCanceled { get; set; }
        public string POComplete { get; set; }
        public string Active { get; set; }
        public int AccountCode { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string ReportMode { get; set; }
        public string HistoryReportMode { get; set; }
        public int TotalRecords { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public IList<OrderAmendHistoryModel> OrderAmendHistoryGrid { get; set; }
    }
}
