using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eTactWeb.DOM.Models
{
    public class TransferMaterialReportModel
    {
        public string? FromDate {  get; set; }
        public string? ToDate { get; set; }
        public string? PartCode {  get; set; }
        public string? ItemName {  get; set; }
        public int? ItemCode {  get; set; }
        public string? FromWorkCenter {  get; set; }
        public string? ToWorkCenter {  get; set; }
        public string? Tostore {  get; set; }   
        public string? TransferSlipNo {  get; set; }
        public string? TransferTowc {  get; set; }
        public string? ReportType {  get; set; }
        public string? ProdSlipNo { get; set; }
        public string? ProdPlanNo {  get; set; }
        public string? ProdSchNo {  get; set; }
        public string? ProcessName {  get; set; }
        public IList<TransferMaterialReportDetail>? TransferMaterialReportDetail { get; set; }
    }
    public class TransferMaterialReportDetail
    {
        public string? TransferMatEntrydate {  get; set; }
        public string? TransferMatSlipNo { get; set; }
        public string? TransferMatSlipDate {  get; set; }
        public string? Status {  get; set; }
        public string? SlipNo {  get; set; }
        public string? TransfDate {  get; set; }
        public string? IssFromWC {  get; set; }
        public string? TOWC {  get; set; }
        public string? TransferTo {  get; set; }
        public string? PRODSTATUSProdUnProdRej {  get; set; }
        public string? IssueToStoreWC {  get; set; }
        public string? TransferFromWC {  get; set; }
        public string? TransferToWC { get; set; }
        public string? TransferToStore {  get; set; }
        public string? TOStore {  get; set; }
        public string? PartCode {  get; set; }
        public string? ItemName {  get; set; }
        public int? ItemCode {  get; set; }
        public string? IssuedByEmp {  get; set; }
        public decimal TransferQty {  get; set; }
        public decimal QCOkQty {  get; set; }
        public decimal ProdQty {  get; set; }
        public string? Unit {  get; set; }
        public decimal AltTransferQty {  get; set; }
        public string? AltUnit {  get; set; }
        public string? PendingToAcknowledge {  get; set; }
        public decimal PendingQtyToAcknowledge { get; set; }
        public string? ItemSize { get; set; }
        public string? ItemColor {  get; set; }
        public decimal Rate {  get; set; }
        public string? InProcQCSlipNo {  get; set; }
        public int InProcQCEntryId {  get; set; }
        public string? QCClearingDate {  get; set; }
        public int InProcQCYearCode {  get; set; }
        public string? ProcessName {  get; set; }
        public decimal TotalStock {  get; set; }
        public string? BatchNo {  get; set; }
        public string? uniquebatchno {  get; set; }
        public decimal BatchStock {  get; set; }
        public decimal ReceivedByStoreQty {  get; set; }
        public string? ItemRecCompleted {  get; set; }
        public string? IssuedByEmpName {  get; set; }
        public string? RecByEmpName { get; set; }
        public string? PendingToRecByStore {  get; set; }
        public string? Remark {  get; set; }
        public string? CC { get; set; }
        public string? EntryByMachineNo { get; set; }
        public string? ActualEntryByEmpName { get; set; }
        public string? ActualEntryDate {  get; set; }
        public string? UpdatedByEmpName { get; set; }
        public string? LastUpdationDate {  get; set; }
        public int seqno {  get; set; }
        public int ProdEntryId {  get; set; }
        public string? ProdSlipNo {  get; set; }
        public int ProdYearCode {  get; set; }
        public string? ProdEntryDate {  get; set; }
        public string? ProdPlanNo {  get; set; }
        public int ProdPlanYearCode {  get; set; }
        public string? ProdSchNo {  get; set; }
        public int ProdSchYearCode { get;set; }
        public string? ParentProdSchNo {  get; set; }
        public int ParentProdSchYearCode { get; set;}
        public string? ItemRemark {  get; set; }
        public decimal ItemWeight {  get; set; }
        public int TransferMatEntryId {  get; set; }
        public int TransferMatYearCode { get; set; }
    }
}
