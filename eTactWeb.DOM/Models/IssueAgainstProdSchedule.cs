using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.DOM.Models
{
    public class IssueAgainstProdSchedule : IssueAgainstProdScheduleDetail
    {
        public int IssAgtProdSchEntryId {  get; set; }
        public int IssAgtProdSchYearCode {  get; set; }
        public string? IssAgtProdSchEntryDate {  get; set; }
        public string? IssAgtProdSchSlipNo {  get; set; }
        public string? IssAgtProdSchSlipDate {  get; set; }
        public string? WorkCenter {  get; set; }
        public int WcId {  get; set; }
        public string? StoreName {  get; set; }
        public int StoreId {  get; set; }
        public string? IssuedByEmpName {  get; set; }
        public int IssuedByEmpId {  get; set; }
        public string? ReceivedByEmpName { get; set; }
        public int ReceivedByEmpId { get;set; }
        public string? Acknowledged {  get; set; }
        public string? AckByEmpName { get; set; }
        public int AckByEmpId { get; set; }
        public string? AckDate {  get; set; }
        public string? ActualEntryDate {  get; set; }
        public string? ActualEntryByName {  get; set; }
        public int ActualEntryById {  get; set; }
        public string? LastUpdatedByName {  get; set; }
        public int LastUpdatedById { get; set; }
        public string? LastUpdatedDate { get;set; }
        public string? CC {  get; set; }
        public string? MachineName { get; set; }
        public int UID {  get; set; }
        public string? Mode {  get; set; }
        public string? FromDate {  get; set; }
        public string? ToDate { get; set; }
        public IList<IssueAgainstProdScheduleDetail>? ItemDetailGrid { get; set; }
    }

    public class IssueAgainstProdScheduleDetail : TimeStamp
    {
        public int IssAgtProdSchEntryId { get; set; }
        public int IssAgtProdSchYearCode { get; set; }
        public string? IssAgtProdSchEntryDate { get; set; }
        public string? IssAgtProdSchSlipNo { get; set; }
        public string? IssAgtProdSchSlipDate { get; set; }
        public string? ProdPlanNo {  get; set; }
        public int ProdPlanYearcode {  get; set; }
        public string? ProdPlanDate {  get; set; }
        public int ProdPlanEntryId { get; set; }
        public int ProdPlanFGItemCode {  get; set; }
        public string? ProdSchNo {  get; set; }
        public int ProdSchEntryId {  get; set; }
        public int ProdSchYearcode {  get; set; }
        public string? ProdSchDate {  get; set; }
        public string? ParentProdSchNo {  get; set; }
        public int ParentProdSchEntryId { get; set; }
        public int ParentProdSchYearCode { get; set; }
        public string? ParentProdSchDate {  get; set; }
        public int PRODSCHFGItemCode { get; set; }
        public string? FGPartCode {  get; set; }
        public string? FGItemName {  get; set; }
        public string? RMPartCode {  get; set; }
        public string? TransactionDate {  get; set; }
        public string? RMItemName {  get; set; }
        public int ProdSchFGItemCode { get; set; }
        public int IssueItemCode {  get; set; }
        public int StoreId {  get; set; }
        public string? StoreName {  get; set; }
        public string? BatchNo {  get; set; }
        public string? UniqueBatchNo {  get; set; }
        public decimal ToatlStock {  get; set; }
        public decimal BatchStock {  get; set; }
        public decimal WIPStock {  get; set; }
        public decimal PrevissueQty {  get; set; }
        public decimal PreIssueAltQty {  get; set; }
        public int BomNo {  get; set; }
        public decimal BomQty { get; set; }
        public decimal WIPMaxQty {  get; set; }
        public int InProcessQCEntryId {  get; set; }
        public string? InprocessQCSlipNo {  get; set; }
        public int InProcessQCYearCode {  get; set; }
        public string? InProcessQCDate {  get; set; }
        public string? ItemHasSubBom {  get; set; }
        public string? MaterialIsIssuedDirectlyFrmWC {  get; set; }
        public int IssuedFromWC {  get; set; }
        public string? IssueFrmWCSlipNo { get; set; }
        public int IssueFrmWCYearCode {  get; set; }
        public string? IssueFrmWCDate {  get; set; }
        public string? IssueFrmQCorTransferForm {  get; set; }
        public decimal StdPacking {  get; set; }
        public decimal IssueQty { get; set; }
        public decimal MaxIssueQty { get; set; }
        public string? Unit {  get; set; }
        public decimal AltQty {  get; set; }
        public string? AltUnit {  get; set; }
        public decimal ProdSchQty {  get; set; }
        public decimal PendQty {  get; set; }
        public decimal PendAfterIssue {  get; set; }
        public string? Batchdate {  get; set; }
        public decimal Rate {  get; set; }
        public string? Remark {  get; set; }
        public string? ItemSize {  get; set; }
        public string? ItemColor {  get; set; }
        public int RemainingStock {  get; set; }
        public string? WorkCenter {  get; set; }
        public int WCId {  get; set; }
        public string? ItemType {  get; set; }
        public string? IssuedAgainstNewRew {  get; set; }
        public int seqno {  get; set; }
        public string? MainGridStockable {  get; set; }
        public string? otherdetail1 {  get; set; }
        public string? otherdetail2 { get; set; }
        public decimal ItemRate {  get; set; }
        public decimal ItemWeight {  get; set; }
        public string? AltItemCode {  get; set; }
        public int IssuedAlternateItem {  get; set; }
        public int OriginalItemCode {  get; set; }
    }
    public class IssueAgainstProductionScheduleDashboard
    {
        public int IssAgtProdSchEntryId { get; set; }
        public int IssAgtProdSchYearCode { get; set; }
        public string? IssAgtProdSchEntryDate {  get; set; }
        public string? IssAgtProdSchSlipNo {  get; set; }
        public string? IssAgtProdSchSlipDate {  get; set; }
        public string? IssuedByEmp {  get; set; }
        public int IssuedByEmpId { get; set; }
        public string? ReceivedByEmp {  get; set; }
        public int ReceivedByEmpId { get; set; }
        public string? AcknowledgedByProd {  get; set; }
        public string? AckDate {  get; set; }
        public string? ActualEntryDate {  get; set; }
        public int ActualEntryBy {  get; set; }
        public string? ActualEntryByEmp {  get; set; }
        public int LastUpdatedBy { get; set; }
        public string? UpdatedByEmp { get; set; }
        public string? LastUpdatedDate { get; set; }
        public string? CC {  get; set; }
        public string? MachineName {  get; set; }
        public int UID {  get; set; }
        public int IssuedFromStoreId {  get; set; }
        public string? IssuedFromStoreName {  get; set; }
        public string? IssueFromStore {  get; set; }    
        public string? WorkCenter {  get; set; }
        public string? ProdPlanNo {  get; set; }
        public int PlanNoYearCode {  get; set; }
        public int PlanNoEntryId {  get; set; }
        public string? PlanDate {  get; set; }
        public int PRODPlanFGItemCode {  get; set; }
        public int PRODSCHFGItemCode { get; set; }
        public string? FGItemName {  get; set; }
        public string? FGPartCode {  get; set; }
        public string? ProdSchNo {  get; set; }
        public int ProdSchEntryId {  get; set; }
        public int ProdSchYearCode {  get; set; }
        public string? ProdSchDate {  get; set; }
        public string? ParentProdSchNo {  get; set; }
        public int ParentProdSchEntryId { get; set; }
        public int ParentProdSchYearCode { get; set; }
        public string? ParentProdSchDate {  get; set; }
        public string? PartCode {  get; set; }
        public string? ItemName {  get; set; }
        public int IssueItemCode {  get; set; }
        public string? BatchNo {  get; set; }
        public string? UniqueBatchno {  get; set; }
        public decimal TotalStock {  get; set; }
        public decimal BatchStock {  get; set; }
        public decimal WIPStock {  get; set; }
        public decimal StdPkg {  get; set; }
        public decimal MaxIssueQtyAllowed {  get; set; }
        public decimal IssueQty { get; set; }
        public string? Unit {  get; set; }
        public decimal AltIssueQty {  get; set; }
        public string? Altunit {  get; set; }
        public decimal PrevissueQty { get; set; }
        public decimal PreIssueAltQty {  get; set; }
        public int BOMNo { get; set; }
        public decimal BomQty {  get; set; }
        public decimal WIPMaxQty {  get; set; }
        public int InProcessQCEntryId {  get; set; }
        public string? InprocessQCSlipNo { get; set; }
        public int InProcessQCYearCode {  get; set; }
        public string? InProcessQCDate {  get; set; }
        public string? ItemHasSubBom {  get; set; }
        public string? MaterialIsIssuedDirectlyFrmWC {  get; set; }
        public IList<IssueAgainstProductionScheduleDashboard>? IssueAgainstProdScheduleDashboard { get; set; }
    }
    public class IssueAgainstProdScheduleDashboard : IssueAgainstProductionScheduleDashboard
    {
        public string? FromDate { get; set; }
        public string? ToDate { get; set; }
        public string? DashboardType { get; set; }
        public string? Searchbox { get; set; }
        public string? FromDate1 { get; set; }
        public string? ToDate1 { get;set; }
        public string? IssueFromStore { get;set; }
        public string? PartCode {  get; set; }
        public string? ItemName {  get; set; }
        public string? ProdPlanNo {  get; set; }
        public string? ProdSchNo { get;set; }
    }
}
