using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.DOM.Models
{
    public class TransferFromWorkCenterModel
    {
        public string? FromDate { get; set; }
        public string? FinFromDate {  get; set; }
        public string? FinToDate {  get; set; }
        public string? ToDate { get; set; }
        public string? CC { get; set; }
        public int WCID { get; set; }
        public string? WorkCenterName { get; set; }
        public int Uid {  get; set; }
        public int ActualEnteredBy {  get; set; }
        public string? ActualEnteredByName {  get; set; }
        public DateTime ActualEntrydate {  get; set; }
        public int TransferMatYearCode {  get; set; }
        public string? Mode {  get; set; }
        public int ID {  get; set; }
        public int UpdatedBy {  get; set; }
        public string? UpdatedByName {  get; set; }
        public DateTime UpdatedOn {  get; set; }
        public string? FromDateBack {  get; set; }
        public string? ToDateBack {  get; set; }
        public string? TransferSlipNoBack {  get; set; }
        public string? ItemNameBack {  get; set; }
        public string? PartCodeBack {  get; set; }
        public string? FromWorkCenterBack {  get; set; }
        public string? ToWorkCenterBack { get; set; }
        public string? StoreNameBack {  get; set; }
        public string? ProdSlipNoBack { get; set; }
        public string? ProdSchNoBack {  get; set; }
        public string? GlobalSearchBack {  get; set; }
        public string? DashboardTypeBack {  get; set; }
        public int TransferMatEntryId {  get; set; }
        public string? TransferMatEntrydate {  get; set; }
        public string? TransferMatSlipNo {  get; set; }
        public string? TransferMatSlipDate {  get; set; }
        public string? PRODSTATUSProdUnProdRej {  get; set; }
        public string? IssueToStoreWC {  get; set; }
        public int IssueFromWCid {  get; set; }
        public string? IssueFromWc {  get; set; }
        public int IssueTOWCid {  get; set; }
        public string? IssueToWcStore { get; set; }
        public int IssueToStoreId {  get; set; }
        public int IssuedByEmp {  get; set; }
        public string? IssueByEmpName {  get; set; }
        public int RecByEmpId {  get; set; }
        public string? RecByEmpName { get; set; }
        public string? Remark {  get; set; }
        public string? PendingToRecByStore {  get; set; }
        public string? EntryByMachineNo {  get; set; }
        public string? PartCode {  get; set; }
        public string? ItemName { get; set; }
        public int ItemCode {  get; set; }
        public int ProdEntryId {  get; set; }
        public string? ProdSlipNo {  get; set; }
        public int ProdEntryYearCode {  get; set; }
        public string? ProdDate {  get; set; }
        public string? ProdPlanNo {  get; set; }
        public int ProdPlanYearCode {  get; set; }
        public string? ProdSchNo {  get; set; }
        public int ProdSchYearCode {  get; set; }
        public string? ParentProdSchNo {  get; set; }
        public int ParentProdSchYearCode { get; set; }
        public decimal TransferQty {  get; set; }
        public decimal QcOkQty {  get; set; }
        public decimal ProdQty {  get; set; }
        public string? Unit {  get; set; }
        public decimal AltTransferQty {  get; set; }
        public string? AltUnit {  get; set; }
        public string? PendingToAcknowledge {  get; set; }
        public decimal PendingQtyToAcknowledge { get; set; }
        public string? ItemSize {  get; set; }
        public string? ItemColor {  get; set; }
        public string? InProcessQcSlipNo { get; set; }
        public int InProcessQcEntryId {  get; set; }
        public string? QcCleaningDate {  get; set; }
        public int InProcessQcYearCode {  get; set; }
        public int ProcessId {  get; set; }
        public string? ProcessName {  get; set; }
        public decimal TotalStock {  get; set; }
        public string? BatchNo {  get; set; }
        public string? UniqueBatchNo {  get; set; }
        public decimal BatchStock {  get; set; }
        public decimal ReceivedByStoreQty {  get; set; }
        public string? ReceivedCompleted {  get; set; }
        public int ReceivedByEmpId {  get; set; }
        public decimal Rate {  get; set; }
        public decimal ItemWeight {  get; set; }
        public string? ReceivebyEmpName {  get; set; }
        public IList<TransferFromWorkCenterDetail>? ItemDetailGrid { get; set; }
        public List<string> WIPErrorList { get; set; } = new List<string>();
        //public IList<TextValue>? PartcodeList { get; set; }
    }
	public class CheckedItemRequest
	{
		public List<string> CheckedItems { get; set; }
	}
	public class TransferFromWorkCenterDetail: TimeStamp
    {
        public int SeqNo {  get; set; }
        public int WCID {  get; set; }
        public string? WorkCenterName {  get; set; }
        public string? PartCode {  get; set; }
        public string? ItemName {  get; set; }
        public int ItemCode {  get; set; }
        public int TransferMatEntryId { get; set; }
        public int TransferMatYearCode { get; set; }
        public int ProdEntryId { get; set; }
        public string? ProdSlipNo { get; set; }
        public int ProdEntryYearCode { get; set; }
        public string? Remark {  get; set; }
        public string? ProdDate { get; set; }
        public string? ProdPlanNo { get; set; }
        public int ProdPlanYearCode { get; set; }
        public string? ProdSchNo { get; set; }
        public int ProdSchYearCode { get; set; }
        public string? ParentProdSchNo { get; set; }
        public int ParentProdSchYearCode { get; set; }
        public decimal TransferQty { get; set; }
        public decimal QcOkQty { get; set; }
        public decimal ProdQty { get; set; }
        public string? Unit { get; set; }
        public decimal AltTransferQty { get; set; }
        public string? AltUnit { get; set; }
        public string? PendingToAcknowledge { get; set; }
        public decimal PendingQtyToAcknowledge { get; set; }
        public string? ItemSize { get; set; }
        public string? ItemColor { get; set; }
        public string? InProcessQcSlipNo { get; set; }
        public int InProcessQcEntryId { get; set; }
        public string? QcCleaningDate { get; set; }
        public int InProcessQcYearCode { get; set; }
        public int ProcessId { get; set; }
        public string? ProcessName { get; set; }
        public decimal TotalStock { get; set; }
        public string? BatchNo { get; set; }
        public string? UniqueBatchNo { get; set; }
        public decimal BatchStock { get; set; }
        public decimal ReceivedByStoreQty { get; set; }
        public string? ReceivedCompleted { get; set; }
        public int ReceivedByEmpId { get; set; }
        public decimal Rate { get; set; }
        public decimal ItemWeight { get; set; }
        public string? ReceivebyEmpName { get; set; }
        
    }
    [Serializable]
    public class TransferFromDashboard
    {
        public string? TransferMatSlipNo { get; set; }
        public string? TransferMatEntrydate {  get; set; }
        public string? TransferMatSlipDate {  get; set; }
        public string? IssueToStoreWC {  get; set; }
        public string? TransferFromWC {  get; set; }
        public string? TransferToWC {  get; set; }  
        public string? TransferToStore {  get; set; }
        public string? Remark {  get; set; }
        public string? PendingToRecByStore {  get; set; }
        public string? IssuedByEmpName {  get; set; }
        public string? ActualEntryByEmpName { get; set;}
        public int UID {  get; set; }
        public string? CC {  get; set; }
        public string? EntryByMachineNo {  get; set; }
        public string? ActualEntryDate {  get; set; }
        public string? UpdatedByEmpName { get; set;}
        public string? LastUpdationDate {  get; set; }
        public int TransferMatEntryId {  get; set; }
        public int TransferMatYearCode {  get; set; }
        public string? PartCode {  get; set; }
        public string? ItemName {  get; set; }
        public decimal TransferQty {  get; set; }
        public decimal QCOkQty {  get; set; }   
        public decimal ProdQty {  get; set; }
        public string? Unit { get; set; }
        public decimal AltTransferQty {  get; set; }
        public string? AltUnit {  get; set; }
        public decimal TotalStock {  get; set; }
        public string? BatchNo {  get; set; }
        public string? uniquebatchno {  get; set; }
        public decimal BatchStock {  get; set; }
        public decimal Rate {  get; set; }
        public decimal ItemWeight {  get; set; }
        public string? ItemSize {  get; set; }
        public string? ItemColor {  get; set; }
        public string? ItemRecCompleted {  get; set; }
        public decimal PendingQtyToAcknowledge {  get; set; }
        public string? ItemRemark {  get; set; }
        public int ProdEntryId {  get; set; }
        public string? ProdSlipNo {  get; set; }
        public int ProdYearCode {  get; set; }
        public string? ProdEntryDate {  get; set; }
        public string? ProdPlanNo {  get; set; }
        public int ProdPlanYearCode {  get; set; }
        public string? ProdSchNo {  get; set; }
        public int ProdSchYearCode {  get; set; }
        public IList<TransferFromDashboard>? TransferFromWorkCenterDashboard { get; set; }
    }
    public class TransferFromWorkCenterDashboard : TransferFromDashboard
    {
        public string? FromDate {  get; set; }
        public string? ToDate { get; set; }
        public string? FromDate1 { get; set; }
        public string? ToDate1 { get; set; }
        public string? TransferMatSlipNo {  get; set; }
        public string? PartCode {  get; set; }
        public string? ItemName {  get; set; }
        public string? TransferFromWC { get; set; }
        public string? TransferToWC { get;set; }
        public string? TransferToStore { get; set; }
        public string? ProdSlipNo { get;set; }
        public string? ProdSchNo { get; set; }
        public string? DashboardType {  get; set; }
        public string? Searchbox { get; set; }
    }
}
