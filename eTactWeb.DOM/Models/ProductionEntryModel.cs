using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;
using static eTactWeb.DOM.Models.JobWorkReceiveModel;

namespace eTactWeb.DOM.Models;
[Serializable]
public class ProductionEntryDashboard
{

    public int EntryId { get; set; }
    public int PRODEntryId { get; set; }
    public string? Entrydate { get; set; }
    public int? Yearcode { get; set; }
    public int? PRODYearcode { get; set; }
    public string? ProdAgainstPlanManual { get; set; }
    public string? NewProdRework { get; set; }
    public string? ProdSlipNo { get; set; }
    public string? ProdDate { get; set; }
    public string? ProdPlanNo { get; set; }
    public int ProdPlanYearCode { get; set; }
    public string? ProdPlanDate { get; set; }
    public string? ProdPlanSchNo { get; set; }
    public int ProdPlanSchYearCode { get; set; }
    public string? ProdPlanSchDate { get; set; }
    public string? Reqno { get; set; }
    public int ReqThrBOMYearCode { get; set; }
    public string? ReqDate { get; set; }
    public decimal FGProdQty {  get; set; }
    public string? FGPartCode { get; set; }
    public string? FGItemName { get; set; }
    public string WorkCenter { get; set; }
    public string OperatorName { get; set; }
    public string Fromtime { get; set; }
    public string totime { get; set; }
    public decimal TotalHrs { get; set; }
    public decimal OverTimeHrs { get; set; }
    public decimal MachineCharges { get; set; }
    public decimal WOQTY { get; set; }
    public decimal ProdSchQty { get; set; }
    public decimal ProdQty { get; set; }
    public decimal FGOKQty { get; set; }
    public decimal FGRejQty { get; set; }
    public string? NextToStore {  get; set; }
    public string? NextToWorkCenter {  get; set; }
    public decimal RejQtyDuetoTrail { get; set; }
    public string QCChecked { get; set; }
    public decimal PendQtyForProd { get; set; }
    public decimal PendQtyForQC { get; set; }
    public decimal PendingQtyToIssue { get; set; }
    public int BOMNO { get; set; }
    public string? ConsumedRMUnit {  get; set; }
    public string? BOMDate { get; set; }
    public string? MachineName { get; set; }
    public string? ScrapType { get; set; }
    public decimal ScrapQty { get; set; }
    public string? Scrapunit { get; set; }
    public string? StageDescription { get; set; }
    public string? ProdInWC { get; set; }
    public string? RejQtyInWC { get; set; }
    public string? rejStore { get; set; }
    public string StoreWC { get; set; }
    public string? TransferFGToWCorSTORE { get; set; }
    public string? QCMandatory { get; set; }
    public string? TransferToQc { get; set; }
    public string? StartTime { get; set; }
    public string? ToTime { get; set; }
    public decimal? setupTime { get; set; }
    public float PrevWC { get; set; }
    public string? ProducedINLineNo { get; set; }
    public string? QCCheckedDate { get; set; }
    public decimal? InitialReading { get; set; }
    public decimal? FinalReading { get; set; }
    public int Shots { get; set; }
    public string? Completed { get; set; }
    public decimal UtilisedHours { get; set; }
    public string SoDate {  get; set; }
    public string? ProdLineNo { get; set; }
    public int stdShots { get; set; }
    public int stdCycletime { get; set; }
    public string? Remark { get; set; }
    public decimal CyclicTime { get; set; }
    public int ProductionHour { get; set; }
    public string? ItemModel { get; set; }
    public int cavity { get; set; }
    public decimal startupRejQty { get; set; }
    public decimal efficiency { get; set; }
    public decimal ActualTimeRequired { get; set; }
    public string? BatchNo { get; set; }
    public string? UniqueBatchNo { get; set; }
    public string? parentProdSchNo { get; set; }
    public string? parentProdSchDate { get; set; }
    public int parentProdSchYearcode { get; set; }
    public string? SONO { get; set; }
    public int SOYearcode { get; set; }
    public string? SODate { get; set; }
    public string? sotype { get; set; }
    public string? QCOffered { get; set; }
    public string? QCOfferDate { get; set; }
    public decimal QCQTy { get; set; }
    public decimal OKQty { get; set; }
    public decimal RejQTy { get; set; }
    public decimal StockQTy { get; set; }
    public string matTransferd { get; set; }
    public int RewQcYearCode { get; set; }
    public string? RewQcDate { get; set; }
    public string? shiftclose { get; set; }
    public string? ComplProd { get; set; }
    public string? CC { get; set; }
    public string? EntryByMachineNo { get; set; }
    public int ActualEntryByEmp { get; set; }
    public string? ActualEmpByName {  get; set; }
    public string? ActualEntryDate { get; set; }
    public int LastUpdatedBy { get; set; }
    public string? LastUpdatedByName {  get; set; }
    public string? LastUpdationDate { get; set; }
    public string? EntryByDesignation { get; set; }
    public string? supervisior { get; set; }
    public string? FromDate { get; set; }
    public string? ToDate { get; set; }
    public string DashboardType { get; set; }
    public decimal ConsumedRMQTY { get; set; }
    public decimal MainRMQTY { get; set; }
    public string? MainRMUnit { get; set; }
    public string? Unit { get; set; }
    public decimal TotalReqRMQty { get; set; }
    public decimal TotalStock { get; set; }
    public decimal BatchStock { get; set; }
    public decimal AltRMQty { get; set; }
    public string? AltRMUnit { get; set; }
    public decimal RMNetWt { get; set; }
    public decimal GrossWt { get; set; }
    public string? Batchwise { get; set; }
    public string? BOMRevDate { get; set; }
    public string? ManualAutoEntry { get; set; }
    public string Proddate { get; set; }
    public string BreakfromTime { get; set; }
    public string BreaktoTime { get; set; }
    public string ResonDetail { get; set; }
    public decimal BreakTimeMin { get; set; }
    public string ResEmpName { get; set; }
    public string ResFactor { get; set; }
    public string? RMPartCode {  get; set; }
    public string? RMItemName {  get; set; }
    public int TotalRecords { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public IList<ProductionEntryDashboard>? ProductionDashboard { get; set; }
}
[Serializable]
public class ProductionDashboard : ProductionEntryDashboard
{
    public string? FromDate { get; set; }
    public string? ToDate { get; set; }
    public string? ItemName { get; set; }
    public string? PartCode { get; set; }
    public string? SlipNo { get; set; }
    public string? ProdPlanNo { get; set; }
    public string? ProdSchNo { get; set; }
    public string? ReqNo { get; set; }
    public string? Searchbox { get; set; }
    public string? Process { get; set; }
    public string? FromDate1 { get; set; }
    public string? ToDate1 { get; set; }

}


[Serializable()]

public class ProductionEntryItemDetail : TimeStamp
{
    public string? PoNo { get; set; }
    public int PoYear { get; set; }
    public string POType { get; set; }
    public string SchNo { get; set; }
    public int SchYearCode { get; set; }
    public int? NoOfBoxes { get; set; }
    public int? ItemCode { get; set; }
    public string PartCode { get; set; }
    public decimal ReqQty { get; set; }
    public decimal IssueQty { get; set; }
    public decimal IssuedQty { get; set; }
    public decimal ProdQty { get; set; }
    public decimal TotalStock { get; set; }
    public decimal LotStock { get; set; }
    public decimal BatchStock { get; set; }
    public int ProdInWCID { get; set; }
    public string ItemName { get; set; }
    public string? Unit { get; set; }
    public decimal PendQty { get; set; }
    public decimal StdPacking { get; set; }
    public decimal PendAfterIssue { get; set; }
    public string? StoreName { get; set; }
    public string BatchDate { get; set; }
    public string Remark { get; set; }

    public decimal RemainingStock { get; set; }
    public decimal Qty { get; set; }
    public decimal Rate { get; set; }
    public string? DetailRemark {  get; set; }    
    public decimal AltQty { get; set; }
    public string? AltUnit { get; set; }
    public string? BatchWise { get; set; }
    public string? BatchNo { get; set; }
    public string? UniqueBatchNo { get; set; }
    public decimal NetWt { get; set; }
    public int McId { get; set; }
    public int OperatorId { get; set; }
    //public string? operatorName { get; set; }
    public string? supervisior { get; set; }
    public string? OperatorName { get; set; }
    public decimal GrossWt { get; set; }
    public string Batchwise {  get; set; }
    public string? Fromtime { get; set; }
    public int WCId { get; set; }
    public string? Totime { get; set; }
    public decimal TotalHours { get; set; }
    public decimal OverTimeHrs { get; set; }
    public decimal MachineCharges { get; set; }
    public string? BreakfromTime { get; set; }
    public string? BreaktoTime { get; set; }
    public int ReasonId { get; set; }
    public string? ReasonDetail { get; set; }
    public decimal BreakTimeMin { get; set; }
    public int ResponcibleEmp { get; set; }
    public string? ResEmpName { get; set; }
    public string? ResFactor { get; set; }
    public string? ItemSize { get; set; }
    public string? ItemColor { get; set; }
    public int? docTypeId { get; set; }
    public int ProcessId { get; set; }
    public string ProcessName { get; set; }
    public string? SaleBillNo { get; set; }
    public int? SaleBillYearCode { get; set; }
    public int RmItemCode { get; set; }
    public string RmPartCode {  get; set; }
    public string RmItemName {  get; set; }
    public decimal? SaleBillQty { get; set; }
    public string? SupplierBatchNo { get; set; }
    public decimal? ShelfLife { get; set; }
    public int? SeqNo { get; set; }
    public int EntryId { get; set; }
    public string? OtherDetail { get; set; }
    public int? AgainstChallanYearcode { get; set; }
    //public int Entryid { get; set; }
    public string ProdDate { get; set; }
    public string EntryDate { get; set; }
    public int YearCode { get; set; }
    public int FGItemCode { get; set; }
    public string FGPartCode { get; set; }
    public string FGItemName { get; set; }
    public string MainPartCode { get; set; }
    public string MainItemName { get; set; }
    public int MainItemCode {  get; set; }
    public int ConsumedRMItemCode { get; set; }
    public decimal ConsumedRMQTY { get; set; }
    public string RmUnit {  get; set; }
    public string ConsumedRMUnit { get; set; }
    public int MainRMitemCode { get; set; }
    public decimal MainRMQTY { get; set; }
    public string MainRMUnit { get; set; }
    public decimal FGProdQty { get; set; }
    public string FGUnit { get; set; }
    public decimal TotalReqRMQty { get; set; }
    public int AltRMItemCode { get; set; }
    public decimal AltRMQty { get; set; }
    public string AltRMUnit { get; set; }
    //public string Batchwise { get; set; }
    public decimal RMNetWt { get; set; }
    public string BOMRevDate { get; set; }
    public int BOMRevNO { get; set; }
    public string ManualAutoEntry { get; set; }
    public string ScrapType { get; set; }
    public decimal ScrapQty { get; set; }
    public string StoreTransferScrap { get; set; }
    public string Scrapunit { get; set; }
    public string TransferToWCStore { get; set; }
    public string? ScrapItemName { get; set; }
    public string? ScrapPartCode { get; set; }
    public int ScrapItemCode { get; set; }
    public int TransferToStoreId { get; set; }
    public int TransferToWC { get; set; }
    public string? ProdType { get; set; }
    public string? ProdEntryAllowToAddRMItem { get; set; }
    public IList<ProductionEntryItemDetail>? ProductionEntryDetail { get; set; }
}

[Serializable]
public class ProductionEntryModel : ProductionEntryItemDetail
{
    public string? FinFromDate { get; set; }
    public string? FinToDate { get; set; }
    public string? FromDate { get; set; }
    public string? ToDate { get; set; }
    public int TransferMatYearCode {  get; set; }
    public string? EntrybyMachineName { get; set; }
    public int ID { get; set; }
    //public string? Mode { get; set; }
    public int EntryId { get; set; }
    public int YearCode { get; set; }
    public string? OperatorName { get; set; }
    public string? Fromtime { get; set; }
    public decimal TotalHours { get; set; }
    public decimal OverTimeHrs { get; set; }
    public decimal MachineCharges { get; set; }
    public string? CurrentDate { get; set; }
    public string? CC { get; set; }
    public DateTime? Date { get; set; }
    public string? EntryDate { get; set; }
    public string? EntryTime { get; set; }
    public decimal WOQTY { get; set; }
    public decimal ProdSchQty { get; set; }
    public decimal FGProdQty { get; set; }
    public decimal FGOkQty { get; set; }
    public decimal FGRejQty { get; set; }
    public decimal RejQtyDuetoTrail { get; set; }
    public decimal PendQtyForProd { get; set; }
    public decimal PendQtyForQC { get; set; }
    public decimal PendingQtyToIssue { get; set; }
    public int MachineId { get; set; }
    public int ProcessId { get; set; }
    public int ProdInWCID { get; set; }
    public int RejWCId { get; set; }
    public int TransferToRejStoreId { get; set; }
    public string TransferFGToWCorSTORE { get; set; }
    public string QCMandatory { get; set; }
    public string TransferToQc { get; set; }
    public int NextWCID { get; set; }
    public int NextStoreId { get; set; }
    public string StartTime { get; set; }
    public string ToTime { get; set; }
    public int setupTime { get; set; }
    public int PrevWC { get; set; }
    public int PrevProcessId { get; set; }
    public string ProducedINLineNo { get; set; }
    public string QCChecked { get; set; }
    public decimal InitialReading { get; set; }
    public decimal FinalReading { get; set; }
    public int Shots { get; set; }
    public string Completed { get; set; }
    public int UtilisedHours { get; set; }
    public string ProdLineNo { get; set; }
    public int stdShots { get; set; }
    public decimal CyclicTime { get; set; }
    public decimal ProductionHour { get; set; }
    public string ItemModel { get; set; }
    public decimal startupRejQty { get; set; }
    public decimal efficiency { get; set; }
    public decimal ActualTimeRequired { get; set; }
    public string BatchNo { get; set; }
    public string RmBatchNo { get; set; }
    public string RmUniqueBatchNo { get; set; }
    public string UniqueBatchNo { get; set; }
    public string parentProdSchNo { get; set; }
    public string parentProdSchDate { get; set; }
    public int parentProdSchItemCode { get; set; }
    public string sotype { get; set; }
    public string QCOffered { get; set; }
    public string ScrapType { get; set; }
    public decimal ScrapQty { get; set; }
    public string StoreTransferScrap { get; set; }
    public string Scrapunit { get; set; }
    public string TransferToWCStore { get; set; }
    public decimal? GrossWeight { get; set; }
    public string ProdPlanSchDate { get; set; }
    public decimal? NetWeight { get; set; }
    public string? ShowPOTillDate { get; set; }
    public string? ModeOfTransport { get; set; }
    public int? PreparedByEmpId { get; set; }
    public string? PreparedByEmp { get; set; }
    public int ShiftId { get; set; }
    public string Shift { get; set; }
    public bool ItemCheck { get; set; }
    public string? PoNo { get; set; }
    public int PoYear { get; set; }
    public string? PODate { get; set; }
    public string SchNo { get; set; }
    public int SchYearCode { get; set; }
    public string? SchDate { get; set; }
    public int? ItemCode { get; set; }
    public string? Unit { get; set; }
    public string ProdType { get; set; }
    public decimal Qty { get; set; }
    public string ActualProdDate { get; set; }
    public string ProdEntryAllowBackDate {  get; set; }
    public decimal Rate { get; set; }
    public string? DetailRemark {  get; set; }
    public string? AltUnit { get; set; }
    public string NewProdRework { get; set; }
    public string ProdSlipNo { get; set; }
    public string ProdAgainstPlanManual { get; set; }
    public string ProdSchNo { get; set; }
    public string? ActualEnteredByName { get; set; }
    public string? UpdatedByName { get; set; }
    public string? ActualEntryDate { get; set; }
    public int ActualEnteredBy { get; set; }
    public string WorkCenter { get; set; }
    public int WcId { get; set; }
    public string? ItemSize { get; set; }
    public string? ItemColor { get; set; }
    public string? OtherDetail { get; set; }
    public string ReqNo { get; set; }
    public float Stock { get; set; }
    public IList<TextValue>? ShiftList { get; set; }
    public IList<TextValue>? ItemNameList { get; set; }
    public string? SupplierBatchNo { get; set; }
    public decimal? ShelfLife { get; set; }
    public int SeqNo { get; set; }
    public string POType { get; set; }
    public string PartCode { get; set; }
    public string ItemName { get; set; }
    public string AltItemName { get; set; }
    public string AltPartCode { get; set; }
    public string RmItemName { get; set; }
    public int RmItemCode { get; set; }
    public string RmPartCode { get; set; }
    public string? ScrapItemName { get; set; }
    public string? ScrapPartCode { get; set; }
    public string FromDateBack { get; set; }
    public string ToDateBack { get; set; }
    public string SlipNoBack { get; set; }
    public string ItemNameBack { get; set; }
    public string PartCodeBack { get; set; }
    public string ProdPlanNoBack { get; set; }
    public string ProdSchNoBack { get; set; }
    public string ReqNoBack { get; set; }
    public string GlobalSearchBack { get; set; }
    public string DashboardTypeBack { get; set; }
    public int ReqYear { get; set; }
    public string ReqDate { get; set; }
    public string ProdSch { get; set; }
    public int ProdSchYear { get; set; }
    public string ProdSchDate { get; set; }
    public string ProdPlan { get; set; }
    public int ProdPlanYear { get; set; }
    public string ProdPlanDate { get; set; }
    public string ParentProdSch { get; set; }
    public int ParentProdSchYear { get; set; }
    public string MachineGroup { get; set; }
    public string MachineName { get; set; }
    public string Operation { get; set; }
    public int BOMNo { get; set; }
    public string BomEffecDate { get; set; }
    public decimal TargetQty { get; set; }
    public decimal TargetQtyUnplanned { get; set; }
    public decimal ProdQty { get; set; }
    public decimal PendQty { get; set; }
    public int ProdSeqNo { get; set; }
    public decimal StartupRej { get; set; }
    public int TransferTo { get; set; }
    public string Store { get; set; }
    public string StoreTransferTo { get; set; }
    public string RejTransferTo { get; set; }
    public string StoreTransfer { get; set; }
    public string SenToQc { get; set; }
    public string ProdStartTime { get; set; }
    public string ProdEndTime { get; set; }
    public string ToolName { get; set; }
    public int ToolItemCode {  get; set; }
    public string NextWorkCenter { get; set; }
    public decimal InitalReading { get; set; }

    public decimal EndReading { get; set; }
    public decimal Cavity { get; set; }
    public int StdCycleTime { get; set; }
    public string? Remark { get; set; }
    public string? Superwiser { get; set; }
    public string Operator { get; set; }
    public string Customerorderno { get; set; }
    public string SONo { get; set; }
    public int SoYear { get; set; }
    public string SoDate { get; set; }

    public string Tranfererd { get; set; }
    public int ReWorkEntryId { get; set; }
    public string ReWorkEntrydate { get; set; }
    public int RewYearcode { get; set; }
    public int recbyempid { get; set; }
    public decimal QCQTy { get; set; }
    public decimal OKQty { get; set; }
    public decimal StockQTy { get; set; }
    public int RewQcEntryId { get; set; }
    public int RewQcYearCode { get; set; }
    public string? RewQcDate { get; set; }
    public string? ComplProd { get; set; }
    public int Uid { get; set; }
    public string? EntryByMachineNo { get; set; }
    public int LastUpdatedBy { get; set; }
    public string? LastUpdationDate { get; set; }
    public string? EntryByDesignation { get; set; }
    public string? QcOfferedDate { get; set; }
    public decimal QcRejQty { get; set; }
    public string? LastUpdatedDate { get; set; }
    public string? ShiftClosed { get; set; }
    public string? QcCompleted { get; set; }
    public string? MaterialTransferd { get; set; }
    public string? ProdEntryAllowToAddRMItem { get; set; }
    public string? QcMandatory {  get; set; }
    public IList<ProductionEntryItemDetail>? ItemDetailGrid { get; set; }
    public IList<ProductionEntryItemDetail>? BreakdownDetailGrid { get; set; }
    public IList<ProductionEntryItemDetail>? OperatorDetailGrid { get; set; }
    public IList<ProductionEntryItemDetail>? ScrapDetailGrid { get; set; }
    public IList<ProductionEntryItemDetail>? ProductionChilDataDetail { get; set; }

    public IList<TextValue>? PONO { get; set; }


    private IList<SelectListItem> _Type = new List<SelectListItem>()
        {
            new() { Value = "Item", Text = "Item" },
            new() { Value = "Service", Text = "Service" },
        };

    public IList<SelectListItem> TypeList
    {
        get => _Type;
        set => _Type = value;
    }


}




