using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.DOM.Models
{
    public class WorkOrderModel : WorkOrderDetail
    {
        public int Entryid { get; set; }
        public string Entrydate { get; set; }
        public int YearCode { get; set; }
        public string EffectiveFrom { get; set; }
        public string EffectiveTill { get; set; }
        public string ForMonth { get; set; }
        public string WONO { get; set; }
        public string WODate { get; set; }
        public int workcenterId { get; set; }
        public string WoStatus { get; set; }
        public string Remarkproductsupplystage { get; set; }
        public string RemarkForProduction { get; set; }
        public string RemarkForRouting { get; set; }
        public string RemarkForPacking { get; set; }
        public string otherInstruction { get; set; }
        public string BillingStatus { get; set; }
        public string PendForRouteSheet { get; set; }
        public string Approved { get; set; }
        public int ApprovedBy { get; set; }
        public string ApprovedDate { get; set; }
        public string CloseWo { get; set; }
        public int CloseBy { get; set; }
        public string closedate { get; set; }
        public string DeactivateWo { get; set; }
        public int DeactivateBy { get; set; }
        public string DeactivateDate { get; set; }
        public string ActualEntryDate { get; set; }
        public int ActualEntryBy { get; set; }
        public string ActualEntryByEmpName { get; set; }
        public int LastUpdateBy { get; set; }
        public string LastUpdateByEmpName { get; set; }
        public string LastUpdateDate { get; set; }
        public string CC { get; set; }
        public int Uid { get; set; }
        public string MachineName { get; set; }
        public string WorevNo { get; set; }
        public string WorevDate { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string FinFromDate { get; set; }
        public string FinToDate { get; set; }
        public IList<WorkOrderDetail>? WorkDetailGrid { get; set; }

    }

    public class WorkOrderDetail : TimeStamp
    {
        public int SeqNo { get; set; }
        public int Sequence { get; set; }
        public int Accountcode { get; set; }
        public string AccountName { get; set; }
        public string SONO { get; set; }
        public string SOType { get; set; }
        public string CustomerOrderNo { get; set; }
        public int SOYearCode { get; set; }
        public string SODATE { get; set; }
        public string SchNo { get; set; }
        public int SchYearcode { get; set; }
        public string SCHDATE { get; set; }
        public string SOCloseDate { get; set; }
        public string SchEffTillDate { get; set; }
        public int Itemcode { get; set; }
        public string PartCode { get; set; }
        public string ItemName { get; set; }
        public string COLOR { get; set; }
        public float PendSOQty { get; set; }
        public float PendWOQty { get; set; }
        public float ProdQty { get; set; }
        public float OrderQty { get; set; }
        public float AltOrderQty { get; set; }
        public float PendRoutSheetQTy { get; set; }
        public float PendProdQty { get; set; }
        public float WOQty { get; set; }
        public float AltQty { get; set; }
        public string ItemDescription{ get; set; }
        public float FGStock { get; set; }
        public float WIPStock { get; set; }
        public string OrderWEF { get; set; }
        public int AmendNo { get; set; }
        public string AmendEffDate { get; set; }
        public string Unit { get; set; }
        public string AltUnit { get; set; }
        public string OrderType { get; set; }
        public string drawingNo { get; set; }
        public string ProdInst1 { get; set; }
        public string ProdInst2 { get; set; }
        public string SOInstruction { get; set; }
        public string PkgInstruction { get; set; }
        public int Bomno { get; set; }
        public string BomName { get; set; }
        public string BomEffectiveDate { get; set; }
        public string MainBomSubBom { get; set; }
        public int MachineGroupId { get; set; }
        public string MachineGroupName { get; set; }
        public int PrefMachineId1 { get; set; }
        public string PrefMachineName1 { get; set; }
        public int PrefMachineId2 { get; set; }
        public string PrefMachineName2 { get; set; }
        public int PrefMachineId3 { get; set; }
        public string PrefMachineName3 { get; set; }
        public string ApproxStartDate { get; set; }
        public string ApproxEndDate { get; set; }
        public string PendingRouteForSheet { get; set; }
        public string RouteSheetNo { get; set; }
        public int RouteSheetYearCode { get; set; }
        public string RouteSheetDate { get; set; }
        public int RouteSheetEntryNo { get; set; }
        public string WorevNo { get; set; }
        public string WORevDate { get; set; }
        public float PrevWOQty { get; set; }
        public string ItemActive { get; set; }
        public int FGStoreId { get; set; }
        public string FGStore { get; set; }
        public int WIPStoreId { get; set; }
        public string WIPStore { get; set; }
        public int StoreId { get; set; }
        public string StoreName { get; set; }  
        public float StoreStock { get; set; }

    }

    public class WorkOrderGridDashboard
    {
        public int Entryid { get; set; }
        public int YearCode { get; set; }
        public string WONO { get; set; }
        public string WODate { get; set; }
        public string EffectiveFrom { get; set; }
        public string EffectiveTill { get; set; }
        public string WorkCenterName { get; set; }
        public string WoStataus { get; set; }
        public string RemarkForProduction { get; set; }
        public string Approved { get; set; }
        public string CloseWo { get; set; }
        public string WorevNo { get; set; }
        public string WORevDate { get; set; }
        public string EntryDate { get; set; }
        public string RemarkProductSupplyStage { get; set; }
        public string RemarkForRouting { get; set; }
        public string RemarkForPacking { get; set; }
        public string OtherInstruction { get; set; }
        public string BillingStatus { get; set; }
        public string PendRouteSheet { get; set; }
        public float PendProdQty { get; set; }
        public float WOQty { get; set; }
        public float FGStock { get; set; }
        public float WIPStock { get; set; }
        public int ApprovedBy { get; set; }
        public string ApprovedDate { get; set; }
        public string CloseDate { get; set; }
        public string MachineName { get; set; }
        public string AccountName { get; set; }
        public string SONO { get; set; }
        public string CustomerOrderNo { get; set; }
        public int SOYearCode { get; set; }
        public string SODate { get; set; }
        public string SchNo { get; set; }
        public int SchYearCode { get; set; }
        public string SchDate { get; set; }
        public int ItemCode { get; set; }
        public string COLOR { get; set; }
        public float OrderQty { get; set; }
        public string PartCode { get; set; }
        public string ItemName { get; set; }
        public float PendRouteSheetQTy { get; set; }
        public string drawingNo { get; set; }
        public string ProdInst1 { get; set; }
        public string ProdInst2 { get; set; }
        public string SOInstruction { get; set; }
        public string PkgInstruction { get; set; }
        public string PendingRouteForSheet { get; set; }
        public string RouteSheetNo { get; set; }
        public string RouteSheetYearCode { get; set; }
        public string RouteSheetDate { get; set; }
        public int RouteSheetEntryNo { get; set; }
        public float PrevWoQty { get; set; }
        public string FGStoreName { get; set; }
        public string WIPStoreName { get; set; }
        public string ActualEntryBy { get; set; }
        public string ActualEntryDate { get; set; }
        public string LastUpdatedBy { get; set; }
        public string LastUpdatedDate { get; set; }
        public string SummaryDetail { get; set; }
        public IList<WorkOrderGridDashboard>? WorkOrderGrid { get; set; }
    }

    public class WorkOrderMainDashboard : WorkOrderGridDashboard
    {
        public string CC { get; set; }
        public string SONO { get; set; }
        public string SchNo { get; set; }
        public string AccountName { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
    }
}
