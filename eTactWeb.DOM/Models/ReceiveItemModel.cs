using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.DOM.Models
{
    public class ReceiveItemModel
    {
        public string? Mode {  get; set; }
        public string? IPAddress {  get; set; }
        public string? FromDate {  get; set; }
        public string? ToDate { get; set; }
        public int RecMatEntryId {  get; set; }
        public string? RecMatEntryDate {  get; set; }
        public int RecMatYearCode {  get; set; }
        public string? RecMatSlipNo {  get; set; }
        public string? RecMatSlipDate {  get; set; }
        public string? CC {  get; set; }
        public int DepID {  get; set; }
        public string? DepName { get; set; }
        public int UID {  get; set; }
        public int ID { get; set; }
        public int ActualEnteredBy {  get; set; }
        public string? ActualEnteredByName {  get; set; }
        public string? ActualEntrydate {  get; set; }
        public int UpdatedBy {  get; set; }
        public string? UpdatedByName {  get; set; }
        public string? UpdatedOn {  get; set; }
        public string? EnteredbyMachineName {  get; set; }
        public string? FromDateBack {  get; set; }
        public string? ToDateBack {  get; set; }
        public string? PartCodeBack {  get; set; }
        public string? ItemNameBack {  get; set; }
        public string? DashboardTypeBack {  get; set; }
        public string? GlobalSearchBack {  get; set; }
        public IList<ReceiveItemDetail>? ItemDetailGrid { get; set; }
    }
    public class ReceiveItemDetail : TimeStamp
    {
        public int RecMatEntryId { get; set; }
        public string? RecMatEntryDate { get; set; }
        public int RecMatYearCode { get; set; }
        public string? RecMatSlipNo { get; set; }
        public string? RecMatSlipDate { get; set; }
        public string? MaterialType { get; set; }
        public string? FromDepWorkCenter { get; set; }
        public int DepID { get; set; }  
        public string? FromWorkcenter { get; set; }
        public int WCID { get; set; }
        public int ActualEnteredBy { get; set; }
        public string? ActualEntrydate { get; set; }
        public int UpdatedBy { get; set; }
        public string? UpdatedOn { get; set; }
        public string? EnteredbyMachineName { get; set; }
        public string? PartCode { get; set; }
        public string? ItemName { get; set; }
        public int ItemCode { get; set; }
        public decimal RecQty {  get; set; }
        public decimal AltTransferQty { get; set; }
        public decimal Qty { get; set; }
        public string? Unit { get; set; }
        public string? AltUnit { get; set; }
        public string? Itemremark { get; set; }
        public int RecStoreId { get; set; }
        public string? RecInStore { get; set; }
        public int ProdEntryId { get; set; }
        public int ProdYearCode { get; set; }
        public string? ProdEntryDate { get; set; }
        public int PlanNoEntryId {  get; set; }
        public string? ProdPlanNo { get; set; }
        public int ProdPlanYearCode { get; set; }
        public int ProdSchEntryId {  get; set; }
        public string? ProdSchNo { get; set; }
        public int ProdSchYearCode { get; set; }
        public int InProcQCEntryId {  get; set; }
        public string? InProcQCSlipNo { get; set; }
        public int InProcQCYearCode { get; set; }
        public decimal ProdQty { get; set; }
        public decimal QCOkQty { get; set; }
        public decimal RejQty { get; set; }
        public string? BatchNo { get; set; }
        public string? uniquebatchno { get; set; }
        public int TransferMatEntryId { get; set; }
        public int TransferMatYearCode { get; set; }
        public string? TransferMatSlipNo { get; set; }
        public int SeqNo { get; set; }
        public string? ReceiveDate { get; set; }
    }
    [Serializable]
    public class ReceiveItemDetailDashboard
    {
        public int RecMatEntryId { get; set; }
        public int RecMatYearCode { get; set; }
        public string? RecMatEntryDate {  get; set; }
        public string? RecMatSlipNo { get; set; }
        public string? RecMatSlipDate {  get; set; }
        public string? ReceiveDate { get; set; }
        public string? MaterialType {  get; set; }
        public string? FromDepWorkCenter {  get; set; }
        public string? ItemName {  get; set; }
        public string? PartCode {  get; set; }
        public decimal ActualRecQtyInStr {  get; set; }
        public decimal ActualTransferQtyFrmWC {  get; set; }
        public string? Unit {  get; set; }
        public decimal AltQty {  get; set; }
        public string? AltUnit {  get; set; }
        public decimal QCOkQty {  get; set; }
        public string? Remark {  get; set; }
        public string? StoreName {  get; set; }
        public string? CC {  get; set; }
        public int Prodentryid {  get; set; }
        public int ProdyearCode {  get; set; }
        public string? ProdDateAndTime {  get; set; }
        public int PlanNoEntryId {  get; set; }
        public string? ProdPlanNo {  get; set; }
        public int ProdPlanYearCode {  get; set; }
        public int ProdSchEntryId {  get; set; }
        public string? ProdSchNo {  get; set; }
        public int ProdSchYearCode {  get; set; }
        public string? InProcQCSlipNo {  get; set; }
        public int InProcQCEntryId {  get; set; }
        public int InProcQCYearCode {  get; set; }
        public decimal ProdQty {  get; set; }
        public decimal RejQty {  get; set; }
        public string? Batchno {  get; set; }
        public string? UniqueBatchno {  get; set; }
        public int TransferMatEntryId {  get; set; }
        public int TransferMatYearCode {  get; set; }
        public string? TransferMatSlipNo {  get; set; }
        public int UID {  get; set; }
        public int ActualEntryByEmpid {  get; set; }
        public string? ActualEntryByname {  get; set; }
        public string? ActualEntryDate {  get; set; }
        public int UpdatedByEmpId {  get; set; }
        public string? UpdatedByName {  get; set; }
        public string? UpdationDate {  get; set; }
        public string? EntryByMachine {  get; set; }
        public IList<ReceiveItemDetailDashboard>? ReceiveItemDashboard { get; set; }
    }
    [Serializable]
    public class ReceiveItemDashboard : ReceiveItemDetailDashboard
    {
        public string? FromDate { get; set; }
        public string? ToDate { get; set; }
        public string? FromDate1 { get; set; }
        public string? ToDate1 { get; set; }
        public string? ItemName { get; set; }
        public string? PartCode { get; set; }
        public string? DashboardType { get; set; }
        public string? Searchbox { get; set; }
    }
}
