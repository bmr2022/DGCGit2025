using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.DOM.Models
{
    public class StockAdjustmentModel : StockAdjustmentDetail
    {
        public int EntryId { get; set; }
        public int YearCode { get; set; }
        public string SlipNo { get; set; }
        public string EntryDate { get; set; }
        public string StoreWorkCenter { get; set; }
        public string StoreWorkcenterBack { get; set; }
        public string? ImportMode { get; set; }
        public string CC { get; set; }
        public int Uid { get; set; }
        public string Remark { get; set; }
        public int entryByEmp { get; set; }
        public string? entryByEmpName { get; set; }
        public int UpdatedByEmp { get; set; }
        public string? UpdatedByEmpName { get; set; }
        public string UpdatedOn { get; set; }
        public string MachineNo { get; set; }
        public string StockAdjustmentDate { get; set; }
        public string? FinFromDate { get; set; }
        public string? FinToDate { get; set; }
        public string ImageURL { get; set; }
        public string? GlobalSearch { get; set; }
        public string FromDateBack { get; set; }
        public string ToDateBack { get; set; }
        public string PartCodeBack { get; set; }
        public string ItemNameBack { get; set; }
        public string StoreNameBack { get; set; }
        public string WorkCenterBack { get; set; }
        public string SummaryDetailBack { get; set; }
        public string GlobalSearchBack { get; set; }
        public IList<TextValue>? BranchList { get; set; }
        public IList<StockAdjustmentDetail>? StockAdjustModelGrid { get; set; }
        public List<StockAdjustmentViewModel>? ExcelDataList { get; set; }
        public List<StockAdjustmentModel>? ExcelDetailGrid { get; set; }



    }
    public class StockAdjustmentDetail : TimeStamp
    {
        public int SeqNo { get; set; }
        public int ItemCode { get; set; }
        public string PartCode { get; set; }
        public string ItemName { get; set; }
        public string Unit { get; set; }
        public float LotStock { get; set; }
        public float TotalStock { get; set; }
        public string altUnit { get; set; }
        public float AltQty { get; set; }
        public float ActualStockQty { get; set; }
        public float AdjQty { get; set; }
        public string AdjType { get; set; }
        public int Storeid { get; set; }
        public string StoreName { get; set; }
        public int Wcid { get; set; }
        public string WCName { get; set; }
        public float Rate { get; set; }
        public float Amount { get; set; }
        public string batchno { get; set; }
        public string uniqbatchno { get; set; }
        public string reasonOfAdjustment { get; set; }

    }
    [Serializable]
    public class StockAdjustDashboard
    {
        public int ItemCode { get; set; }
        public string? PartCode { get; set; }
        public string? ItemName { get; set; }
        public int EntryId { get; set; }
        public int YearCode { get; set; }
        public string Unit { get; set; }
        public float TotalStock { get; set; }
        public float LotStock { get; set; }
        public string altUnit { get; set; }
        public float AltQty { get; set; }
        public float AdjQty { get; set; }
        public float ActuleStockQty { get; set; }
        public string AdjType { get; set; }
        public float Rate { get; set; }
        public float Amount { get; set; }
        public string batchno { get; set; }
        public string uniquebatchno { get; set; }
        public int Storeid { get; set; }
        public string? StoreName { get; set; }
        public int wcid { get; set; }
        public string? WorkCenter { get; set; }
        public string SlipNo { get; set; }
        public string StockAdjustmentDate { get; set; }
        public string? reasonOfAdjutment { get; set; }
        public string? EntryDate { get; set; }
        public string? StoreWorkcenter { get; set; }
        public int entryByEmp { get; set; }
        public string? enterByEmpName { get; set; }
        public string? UpdatedByEmp { get; set; }
        public string? UpdatedByEmpName { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public string? SummaryDetail { get; set; }
        public string EntryByMachineName {  get; set; }
        public List<StockAdjustDashboard>? SADashboard { get; set; }
    }
    [Serializable]
    public class SADashborad : StockAdjustDashboard
    {
        public string? PartCode { get; set; }
        public string? ItemName { get; set; }
        public string? StoreName { get; set; }
        public string? WCName { get; set; }
        public string? StoreWorkcenter { get; set; }
        public string? FromDate { get; set; }
        public string? ToDate { get; set; }
        public string FromDate1 { get; set; }
        public string ToDate1 { get; set; }
        public string Searchbox { get; set; }
    }

    public class StockAdjustmentViewModel
    {
        public int SeqNo { get; set; }
        public string StoreWorkCenter { get; set; }
        public int ItemCode { get; set; }
        public string PartCode { get; set; }
        public string ItemName { get; set; }
        public string StoreName { get; set; }
        public string WCName { get; set; }
        public float ActualStock { get; set; }
        public float AdjStock { get; set; }
        public string Unit { get; set; }
        public string? AltUnit { get; set; }
        public string? batchno { get; set; }
        public string? uniqbatchno { get; set; }
        public string? Reason { get; set; }
    }
}
