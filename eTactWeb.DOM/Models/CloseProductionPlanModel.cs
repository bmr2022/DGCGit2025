using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.DOM.Models
{
    public class CloseProductionPlanModel : TimeStamp
    {
        public string Flag { get; set; }
        public string CloseOpen { get; set; }
        public int EmpId { get; set; }
        public string? FromDate { get; set; }
        public string? ToDate { get; set; }
        public int ItemCode { get; set; }
        public string ItemName { get; set; }
        public int ApprovedBy { get; set; }
        public string? ApprovalDate { get; set; }
        public string ProdPlanNo { get; set; }
        public int ActualEntryId { get; set; }
        public string EntryByEmpName { get; set; }
        public string ActualEntryDate { get; set; }
        public string WONO { get; set; }
        public string CustomerOrderNo { get; set; }
        public int AltOrderQty { get; set; }
        public int AltQty { get; set; }
        public string ProdInst1 { get; set; }
        public string ProdInst2 { get; set; }
        public string SOInstruction { get; set; }
        public string PkgInstruction { get; set; }
        public string PendingRouteForSheet { get; set; }
        public string RouteSheetNo { get; set; }
        public string RouteSheetDate { get; set; }
        public string OrderType { get; set; }
        public string OrderWEF { get; set; }
        public string SOCloseDate { get; set; }
        public string SchEffTillDate { get; set; }
        public string ItemDescription { get; set; }
        public string ApproxStartDate { get; set; }
        public string ApproxEndDate { get; set; }
        public int StoreStock { get; set; }
        public int YearCode { get; set; }
        public string MachineName { get; set; }
        public int LastUpdatedBy { get; set; }
        public string UpdatedByEmpName { get; set; }
        public int SrNO { get; set; }
        public string ReportType { get; set; }
        public DataTable dt { get; set; }
        public IList<CloseProductionPlanModel> CloseProductionPlanGrid { get; set; }

        // Newly added fields
        public string WODate { get; set; }
        public string WOStatus { get; set; }
        public string EntryDate { get; set; }
        public string EffectiveFrom { get; set; }
        public string AccountName { get; set; }
        public string PartCode { get; set; }
        public int WOQty { get; set; }
        public int FGStock { get; set; }
        public int WIPStock { get; set; }
        public int PendRoutSheetQty { get; set; }
        public int PendProdQty { get; set; }
        public string SONO { get; set; }
        public string EntryMachineName { get; set; }
        public string SODate { get; set; }
        public string SchNo { get; set; }
        public string SchDate { get; set; }
        public string Color { get; set; }
        public int OrderQty { get; set; }
        public string Unit { get; set; }
        public string AltUnit { get; set; }
        public string ApprovedDate { get; set; }
        public string EffectiveTill { get; set; }
        public string DeactivateWO { get; set; }
        public string DeactivateDate { get; set; }
        public string RemarkForRouting { get; set; }
        public string RemarkForPacking { get; set; }
        public string OtherInstruction { get; set; }
        public string BillingStatus { get; set; }
        public string PendForRouteSheet { get; set; }
        public string ActualEntryByEmpName { get; set; }
        public string LastUpdatedByEmpName { get; set; }
        public string LastUpdatedDate { get; set; }
        public string CC { get; set; }
        public string UID { get; set; }
        public int EntryId { get; set; }
        public string ForMonth { get; set; }
        public string CloseWO { get; set; }
        public string CloseDate { get; set; }
        public string CloseBy { get; set; }
        public string RemarkProductSupplyStage { get; set; }
        public string RemarkForProduction { get; set; }
        public string Approved { get; set; }
        public string DrawingNo { get; set; }
    }

}

