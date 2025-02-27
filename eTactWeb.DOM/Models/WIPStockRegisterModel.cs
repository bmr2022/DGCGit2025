using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eTactWeb.DOM.Models
{
    public class WIPStockRegisterModel
    {
        public string? FromDate { get; set; }
        public string? ToDate { get; set; }
        public int WCID { get; set; }
        public string? ReportType { get; set; }
        public string? Type { get; set; }
        public string? PartCode { get; set; }
        public string? ItemName { get; set; }
        public string ItemGroup { get; set; }
        public string ItemType { get; set; }
        public string? ReportMode { get; set; }
        public string? BatchNo { get; set; }
        public string? UniqueBatchNo { get; set; }

        public IList<WIPStockRegisterDetail>? WIPStockRegisterDetail { get; set; }
    }

    public class WIPStockRegisterDetail
    {
        public int SeqNo { get; set; }        
        public string? WorkCenterName { get; set; }
        public string? TransactionType { get; set; }
        public string? TransDate { get; set; }
        public string? PartCode { get; set; }
        public string? ItemName { get; set; }
        public decimal OpnStk { get; set; }
        public decimal RecQty { get; set; }
        public decimal IssQty { get; set; }
        public decimal TotStk { get; set; }
        public string? Unit { get; set; }
        public decimal MinLevel { get; set; }
        public string? AltUnit { get; set; }
        public decimal AltStock { get; set; }
        public string BillNo { get; set; }
        public string FromWc { get; set; }
        
        public string FromStore { get; set; }
        public string ToWC { get; set; }
        public string ToStore { get; set; }


        public string BillDate { get; set; }
        public string PartyName { get; set; }
        public string MRNNo {  get; set; }
        public decimal Rate { get; set; }
        public decimal Amount { get; set; }
        public string BatchNo { get; set; }
        public string UniquebatchNo { get; set; }
        public int EntryId { get; set; }
        public decimal AltRecQty { get; set; }
        public decimal AltIssQty { get; set; }
        public string GroupName { get; set; }
        public decimal StdPacking { get; set; }
        public decimal AvgRate { get; set; }
        public decimal MaximumLevel { get; set; }
        public decimal ReorderLevel { get; set; }
        public string? BinNo { get; set; }
        public decimal Total { get; set; }
        public string? ItemType { get; set; }
        public string? ItemGroup { get; set; }
         public decimal? BatchStock { get; set; }
        public int SeqNum { get; set; }
        public string? FromDate { get; set; }
        public string? ToDate { get; set; }
        public string? Address { get; set; }
        public string? Address2 { get; set; }
        }
}
