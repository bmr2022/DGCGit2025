using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.DOM.Models
{
    public class InventoryAgingReportModel: TimeStamp
    {
        public string PartCode { get; set; }
        public string ItemName { get; set; }
        public string MRNNo { get; set; }
        public string? MRNEntryDate { get; set; }
        public string PONo { get; set; }
        public string? PODate { get; set; }
        public decimal RecQty { get; set; }
        public string BatchNo { get; set; }
        public string UniqueBatchNo { get; set; }
        public decimal Rate { get; set; }
        public decimal TotalAmt { get; set; }
        public decimal TotalValue { get; set; }
        public decimal TotalStock { get; set; }
        public string Unit { get; set; }
        public string PartyName { get; set; }
        public string Type_Item { get; set; }
        public string Group_Name { get; set; }
        public int Foduration { get; set; }
        public string? LastMovementDate { get; set; }
        public int InvDays { get; set; }
        public string AgingStatus { get; set; }
        public long AccountCode { get; set; }
        //public long ItemCode { get; set; }
        public decimal ZeroToThirty { get; set; }
        public decimal ThirtyOneToSixty { get; set; }
        public decimal SixtyOneToNinety { get; set; }
        public decimal NinetyToOneEighty { get; set; }
        public decimal OneEightyToThreeSixty { get; set; }
        public decimal GreaterThanThreeSixty { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string ReportType { get; set; }
        public string RMPartCode { get; set; }
        public string RMItemName { get; set; }
        public string StoreName { get; set; }
        public int Storeid { get; set; }
        public string WorkCenterName { get; set; }
        public int WorkCenterId { get; set; }
        public int Itemcode { get; set; }
        public string CurrentDate { get; set; }
        public int Aging_0_30 { get; set; }
        public int Aging_31_60 { get; set; }
        public int Aging_61_90 { get; set; }
        public int Aging_90_180 { get; set; }
        public int Aging_180_360 { get; set; }
        public int Aging_91 { get; set; }
        public int Aging_Above360 { get; set; }
        public int StoreStock { get; set; }
        public int WIPStock { get; set; }
        public int TotalRecords { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public IList<InventoryAgingReportModel> InventoryAgingReportGrid { get; set; }
    }
}
