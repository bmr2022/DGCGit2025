using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.DOM.Models
{
    public class InterStoreTransferModel : InterStoreTransferDetail
    {
        public int EntryId { get; set; }
        public int YearCode { get; set; }
        public string? IPAddress { get; set; }
        public string EntryDate { get; set; }
        public string SlipNo { get; set; }
        public string SlipDate { get; set; }
        public string IssueToStoreWC { get; set; }
        public int FromStoreId { get; set; }
        public string ToStoreName { get; set; }
        public int ToStoreId { get; set; }
        public string FromStoreName { get; set; }
        public int ToWCID { get; set; }
        public string ToWCName { get; set; }
        public int IssuedBy { get; set; }
        public string Remark { get; set; }
        public int ActualEntryBy { get; set; }
        public string ActualEntryByName { get; set; }
        public string ActualEntryDate { get; set; }
        public int LastUpdatedBy { get; set; }
        public string LastUpdatedByName { get; set; }
        public string LastUpdationDate { get; set; }
        public string TransferReason { get; set; }
        public string CC { get; set; }
        public int Uid { get; set; }
        public string MachineName { get; set; }
        public string FinFromDate { get; set; }
        public string FinToDate { get; set; }
        public string FromDateBack { get; set; }
        public string ToDateBack { get; set; }
        public string SlipNoBack { get; set; }
        public string PartCodeBack { get; set; }
        public string ItemNameBack { get; set; }
        public string BatchNoback { get; set; }
        public string DashboardTypeBack { get; set; }
        public string GlobalSearchBack { get; set; }
        public IList<InterStoreTransferDetail> InterStoreDetails { get; set; }
    }
    public class InterStoreTransferDetail : TimeStamp
    {
        public int ItemCode { get; set; }
        public string PartCode { get; set; }
        public string ItemName { get; set; }
        public float TotalStockQty { get; set; }
        public float LotStockQty { get; set; }
        public float Qty { get; set; }
        public string Unit { get; set; }
        public float AltQty { get; set; }
        public string AltUnit { get; set; }
        public float Rate { get; set; }
        public string BatchNo { get; set; }
        public string UniqueBatchNo { get; set; }
        public string ReasonOfTransfer { get; set; }
        public float RecStoreStock { get; set; }
        public int SeqNo { get; set; }
    }
    public class InterStoreDashboard
    {
        public int EntryId { get; set; }
        public int YearCode { get; set; }
        public string EntryDate { get; set; }
        public string SlipDate { get; set; }
        public string IssueToStoreWC { get; set; }
        public string FromStoreName { get; set; }
        public string Remark { get; set; }
        public int ActualEntryBy { get; set; }
        public string ActualEntryByName { get; set; }
        public string ActualEntryDate { get; set; }
        public string LastUpdatedByName { get; set; }
        public string LastUpdatetionDate { get; set; }
        public string TransferReason { get; set; }
        public string CC { get; set; }
        public string MachineName { get; set; }
        public float TotalStockQty { get; set; }
        public float LotStockQty { get; set; }
        public float Qty { get; set; }
        public string Unit { get; set; }
        public float AltQty { get; set; }
        public string AltUnit { get; set; }
        public float Rate { get; set; }
        public string UniqueBatchNo { get; set; }
        public string ReasonOfTransfer { get; set; }
        public float RecStoreStock { get; set; }
        public string PartCode { get; set; }
        public string ItemName { get; set; }
        public int ItemCode { get; set; }
        public string ToStorename { get; set; }
        public string ToWCName { get; set; }
        public string Batchno { get; set; }
        public string SlipNo { get; set; }
        public int ToStoreId { get; set; }
        public int ToWCID { get; set; }
    }
    public class ISTDashboard : InterStoreDashboard
    {
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string  SummaryDetail{ get; set; }
        public string Searchbox { get; set; }
        public IList<InterStoreDashboard> ISTDashboardGrid { get; set; }
    }
}