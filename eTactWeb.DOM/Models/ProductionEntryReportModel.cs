using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eTactWeb.DOM.Models
{
    public class ProductionEntryReportModel
    {
        public string? FromDate {  get; set; }
        public string? ToDate { get; set; } 
        public string? FGPartCode {  get; set; }
        public string? FGItemName {  get; set; }
        public string? RMPartCode {  get; set; }
        public string? RMItemName {  get; set; }
        public string? ProdSlipNo {  get; set; }
        public string? ProdPlanNo {  get; set; }
        public string? ProdSchNo {  get; set; }
        public string? ReqNo {  get; set; }
        public string? WorkCenter {  get; set; }
        public string? MachineName {  get; set; }
        public string? OperatorName {  get; set; }
        public string? Process {  get; set; }
        public string? ReportType { get; set; }
        public IList<ProductionEntryReportDetail>? ProductionEntryReportDetail { get; set; }
    }
    public class ProductionEntryReportDetail
    {
        public int PRODEntryId { get; set; }
        public string? Entrydate { get; set; }
        public int PRODYearcode { get; set; }
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
        public string? FGPartCode { get; set; }
        public string? FGItemName { get; set; }
        public decimal WOQTY { get; set; }
        public decimal ProdSchQty { get; set; }
        public decimal FGProdQty { get; set; }
        public decimal FGOKQty { get; set; }
        public decimal FGRejQty { get; set; }
        public decimal RejQtyDuetoTrail { get; set; }
        public decimal PendQtyForProd { get; set; }
        public decimal PendQtyForQC { get; set; }
        public decimal PendingQtyToIssue { get; set; }
        public int BOMNO { get; set; }
        public string? BOMDate { get; set; }
        public string? TransferToWCSTr { get; set; }

        
        public string? TransferToWCStr { get; set; }

        public string? MachineName { get; set; }
        public string? StageDescription { get; set; }
        public string? ProdInWC { get; set; }
        public string? RejQtyInWC { get; set; }
        public string? RejStore { get; set; }
        public string? TransferFGToWCorSTORE { get; set; }
        public string? QCMandatory { get; set; }
        public string? TransferToQc { get; set; }
        public string? StartTime { get; set; }
        public string? ToTime { get; set; }
        public decimal setupTime { get; set; }
        public int PrevWC { get; set; }
        public string? ProducedINLineNo { get; set; }
        public string? QCChecked { get; set; }
        public decimal InitialReading { get; set; }
        public decimal FinalReading { get; set; }
        public int Shots { get; set; }
        public string? Completed { get; set; }
        public decimal UtilisedHours { get; set; }
        public string? SODate { get; set; }
        public string? ProdLineNo { get; set; }
        public int stdShots { get; set; }
        public int stdCycletime { get; set; }
        public string? Remark { get; set; }
        public decimal CyclicTime { get; set; }
        public decimal ProductionHour { get; set; }
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
        public string? sotype { get; set; }
        public string? QCOffered { get; set; }
        public string? QCOfferDate { get; set; }
        public decimal QCQTy { get; set; }
        public decimal OKQty { get; set; }
        public decimal RejQTy { get; set; }
        public decimal StockQTy { get; set; }
        public string? matTransferd { get; set; }
        public int RewQcYearCode { get; set; }
        public string? RewQcDate { get; set; }
        public string? shiftclose { get; set; }
        public string? ComplProd { get; set; }
        public string? CC { get; set; }
        public string? EntryByMachineNo { get; set; }
        public string? ActualEmpByName { get; set; }
        public string? ActualEntryDate { get; set; }
        public string? LastUpdatedByName { get; set; }
        public string? LastUpdationDate { get; set; }
        public string? EntryByDesignation { get; set; }
        public string? operatorName { get; set; }
        public string? supervisior { get; set; }
        public string? Workcenter { get; set; }
        public decimal PendingQtyToTransfer { get; set; }
        public string? RMPartCode { get; set; }
        public string? RMItemName { get; set; }
        public decimal RMConsQty { get; set; }
        public string? RMUnit { get; set; }
        public string? Reason { get; set; }
        public string? BreakfromTime { get; set; }
        public string? BreaktoTime { get; set;}
        public decimal BreakTimeMin {  get; set; }
        public string? ResEmpName {  get; set; }
        public string? ResFactor {  get; set; }
        public decimal Breakdown {  get; set; }
        public decimal TotalHrs {  get; set; }
        public decimal OverTimeHrs { get;set; }
        public decimal MachineCharges { get; set; }
        public string? ShiftName {  get; set; }
        public decimal TotProdQty { get; set; }
        public decimal PendQtyForQc {  get; set; }
        public decimal QCOKQty {  get; set; }
        public decimal RejQty {  get; set; }
        public decimal RewQty {  get; set; }
    }
}
