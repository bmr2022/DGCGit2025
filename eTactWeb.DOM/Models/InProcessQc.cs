using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;
using static eTactWeb.DOM.Models.MirModel;

namespace eTactWeb.DOM.Models
{
    public class InProcessQc
    {
        public string? FromDate { get; set; }
        public string? ToDate { get; set; }
        public string? CC {  get; set; }
        public int ActualEnteredBy {  get; set; }
        public string? ActualEnteredByName {  get; set; }
        public string ActualEntrydate { get; set; }
        public string? Mode { get;set; }
        public int ID {  get; set; }    
        public int UpdatedBy { get; set; }
        public string? UpdatedByName {  get; set; }
        public string UpdatedOn {  get; set; }
        public string? FromDateBack {  get; set; }
        public string? ToDateBack { get; set; }
        public string? QcSlipNoBack {  get; set; }           
        public string? PartCodeBack { get; set; }
        public string? ItemNameBack { get; set; }
        public string? ProdSlipNoBack { get; set; }
        public string? WorkCenterBack { get; set; }
        public string? ProcessNameBack {  get; set; }
        public string? ProdSchNoBack {  get; set; }
        public string? ProdPlanNoBack {  get; set; }
        public string? DashboardTypeBack { get; set; }
        public string? GlobalSearchBack { get; set; }
        public int InProcessEntryId {  get; set; }
        public string? InProcQcSlipNo {  get; set; }
        public int QcClearedbyEmpId {  get; set; }
        public string? QcClearedbyEmpName {  get; set; }
        public string? QcCleaningDate {  get; set; }
        public string? EnteredbyMachineName {  get; set; }
        public string? Materialtransfer {  get; set; }
        public int CreatedBy { get; set; }
        public int LastUpdatedBy {  get; set; }
        public string? LastUpdatedDate { get; set; }
        public string? FinFromDate {  get; set; }
        public string? FinToDate {  get; set; }
        public int InProcQCEntryId {  get; set; }
        public int InProcQCYearCode {  get; set; }
        public int UID {  get; set; }
        public int RewQty {  get; set; }
        public IList<InProcessQcDetail>? ItemDetailGrid { get; set; }
        public IList<TextValue>? EmpNameList { get; set; }
        public IList<InProcessPend>? InProcessFromPendDetail { get; set; }

    }
    public class InProcessQcDetail : TimeStamp
    {
        public int SeqNo { get; set; }
        public string? ProdSlipNo { get; set; }
        public int ProdYearcode {  get; set; }
        public string? ProdDate {  get; set; }
        public string? PartCode {  get; set; }
        public string? ItemName {  get; set; }
        public string? ProdPlanSchNo {  get; set; }
        public string? ProdPlanSchDate {  get; set; }   
        public string? ProdPlanNo {  get; set; }
        public int ProdPlanYearCode {  get; set; }
        public string? ProdPlanDate {  get; set; }
        public string? Reqno {  get; set; }
        public string? ReqDate {  get; set; }
        public decimal TotProdQty { get; set; }
        public decimal FGRejQty { get; set; }
        public string? BatchNo {  get; set; }
        public string? UniqueBatchNo { get; set; }
        public int ProdPlanSchYearCode {  get; set; }
        public string? NewProdRework {  get; set; }
        public string? ShiftName {  get; set; }
        public decimal SchQty {  get; set; }
        public decimal FGOkQty {  get; set; }
        public decimal FGProdqty { get; set; }
        public decimal QCOKQty {  get; set; }
        public decimal QCRejQty { get; set; }
        public int ItemCode {  get; set; }
        public int ProdEntryId {  get; set; }
        public decimal OKProdQty {  get; set; }
        public decimal ProdRejQty { get; set; }
        public string? WorkCenter { get; set; }
        public int WcId {  get; set; }
        public string? OperatorName { get; set; }
        public string? MachineName {  get; set; }
        public int InProcessEntryId {  get; set; }
        public int InProcQCYearCode {  get; set; }  
        public string? ProdEntryDate {  get; set; }
        public int ReqYearCode {  get; set; }
        public decimal RewQty {  get; set; }
        public string? RejReason {  get; set; }
        public string? RewRemark {  get; set; }
        public string? otherdetail {  get; set; }
        public int QcClearByEMPId {  get; set; }
        public int ApprovedByEmpId {  get; set; }
        public decimal TransferedQty {  get; set; }
        public decimal PendForTransfQty { get; set; }
        public string? ToStoreOrWc {  get; set; }
        public int ToWcId {  get; set; }
        public string? ToWorkCenter {  get; set; }
        public int ToStoreId {  get; set; }
        public string? ToStoreName {  get; set; }
        public int ProcessId {  get; set; }
        public string? MaterialIstransfered {  get; set; }
        public decimal Sampleqtytested {  get; set; }
        public decimal SampleQty {  get; set; }
        public string? TestingMethod {  get; set; }
    }
    public class InProcessPend
    {
        public int ProdEntryId { get; set; }
        public int Prodyearcode { get; set; }
        public string? ProdSlipNo { get; set; }
        public int FGItemCode { get; set; }
    }
    [Serializable]
    public class InProcessQcDashboard
    {
        public int InProcQCEntryId { get; set; }
        public int InProcQCYearCode { get; set; }
        public string? InProcQCSlipNo { get; set; }
        public int QcClearedEmpId {  get; set; }
        public string? QcClearedEmp {  get; set; }
        public string? QCClearingDate { get; set; }
        public string? EnteredByMachine { get; set; }
        public int ActualEntryById {  get; set; }
        public string? ActualEmpName {  get; set; }
        public string? ActualEntryDate { get; set; }
        public int LastUpdatedBy {  get; set; }
        public string? LastUpdatedByEmp {  get; set; }
        public string? LastUpdatedDate { get; set; }
        public string? CC { get; set; }
        public string? ItemName {  get; set; }
        public string? PartCode {  get; set; }
        public int ProdEntryid {  get; set; }
        public int ProdYearCode {  get; set; }
        public string? ProdSlipNo { get; set; }
        public string? ProdEntryDate {  get; set; }
        public string? MaterialIstransfered { get; set; }
        public string? ProdSchNo{  get; set; }
        public int ProdSchYearCode {  get; set; }
        public string? ProdSchdate {  get; set; }
        public string? ProdPlanNo {  get; set; }
        public int ProdPlanYearCode {  get; set; }
        public string? ProdPlanDate {  get; set; }
        public string? ReqNo {  get; set; }
        public string? ReqDate {  get; set; }
        public int ReqYearCode { get; set; }
        public decimal ProdQty {  get; set; }
        public decimal ProdOkQty { get; set; }
        public decimal OkQty {  get; set; }
        public decimal RejQty {  get; set; }
        public decimal RewQty {  get; set; }    
        public string? RejReason {  get; set; }
        public string? RewRemark {  get; set; }
        public string? otherdetail {  get; set; }
        public string? Batchno {  get; set; }
        public string? Uniquebatchno {  get; set; }
        public decimal TransferedQty {  get; set; }
        public decimal PendForTransfQty {  get; set; }
        public int ToWcId {  get; set; }
        public string? TransfertoWorkCenter {  get; set; }
        public int ToStoreId {  get; set; }
        public string? TransfertoStoreName {  get; set; }
        public int WcId {  get; set; }
        public string? WorkCenter {  get; set; }
        public string? ProcessName {  get; set; }
        public decimal Sampleqtytested {  get; set; }
        public string? TestingMethod {  get; set; }
        public IList<InProcessQcDashboard>? InProcessDashboard { get; set; }
    }
    [Serializable]
    public class InProcessDashboard : InProcessQcDashboard
    {
        public string? FromDate { get; set; }
        public string? ToDate {  get; set; }
        public string? FromDate1 { get; set; }
        public string? ToDate1 { get; set; }
        public string? InProcQcSlipNo { get; set; }
        public string? ItemName {  get; set; }
        public string? PartCode {  get; set; }
        public string? ProdSlipNo { get; set; }
        public string? Workcenter { get; set; }
        public string? ProcessName {  get; set; }
        public string? ProdPlanSchNo { get; set; }
        public string? ProdPlanNo { get; set; }
        public string? DashboardType {  get; set; }
        public string? Searchbox { get; set; }
    }
}
