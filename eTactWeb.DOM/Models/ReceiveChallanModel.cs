using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.DOM.Models
{
    public class ReceiveChallanModel : ReceiveChallanDetail
    {
        public int EntryId { get; set; }
        public int Yearcode { get; set; }
        public string Entrydate { get; set; }
        public string RetNotRetChallan { get; set; }
        public string ShowOtherReqFields { get; set; }
        public string AgainstMRNNOrGate { get; set; }
        public string MRNNo { get; set; }
        public int AgainstMRNYearCode { get; set; }
        public string BillOrChallan{ get; set; }
        public string gateno { get; set; }
        public int GateYearCode { get; set; }
        public string ChallanNo { get; set; }
        public string ChallanDate { get; set; }
        public int AccountCode { get; set; }
        public string AccountName{ get; set; }
        public string TruckNo { get; set; }
        public string TransPort { get; set; }
        public int DeptTo { get; set; }
        public string Remark { get; set; }
        public string SendforQC { get; set; }
        public float TotalAmount { get; set; }
        public float NetAmt { get; set; }
        public float TotalDiscountPercent { get; set; }
        public float TotalDiscountAmount { get; set; }
        public string ChallanType { get; set; }
        public string DocTypeCode { get; set; }
        public string DocTypeName { get; set; }
        public string InvoiceNo { get; set; }
        public int InvoiceYearCode { get; set; }
        public string pendcompleted { get; set; }
        public int EnteredByEmpid { get; set; }
        public string ForChallan { get; set; }
        public string MachineName { get; set; }
        public int CreatedByEmpid { get; set; }
        public string CreatedByEmpName { get; set; }
        public string CreatedOn { get; set; }
        public int UpdatedBy { get; set; }
        public string UpdatedByEmpName { get; set; }
        public string UpdatedOn { get; set; }
        public string CC { get; set; }
        public int UID { get; set; }
        public string GateDate { get; set; }
        public string MRNDate { get; set; }
        public string FinFromDate { get; set; }
        public string FinToDate { get; set; }
        public string RGPNRGP { get; set; }

        public string FinFromDateBack { get; set; }
        public string FinToDateBack { get; set; }
        public IList<ReceiveChallanDetail> ReceiveChallanList { get; set; }
    }
    
    public class ReceiveChallanDetail : TimeStamp
    {
        public int IssueChallanEntryId { get; set; }
        public int IssueChallanYearCode { get; set; }
        public string IssueChallanNo { get; set; }
        public int ItemCode { get; set; }
        public string PartCode { get; set; }
        public string ItemName { get; set; }
        public int SeqNo { get; set; }
        public string Unit { get; set; }
        public float RecQty { get; set; }
        public float Rate { get; set; }
        public float Amount { get; set; }
        public float IssuedQty{ get; set; }
        public float PendQty { get; set; }
        public string Produced { get; set; }
        public string RemarkDetail { get; set; }
        public float GateQty { get; set; }
        public int Storeid { get; set; }
        public string StoreName { get; set; }
        public string pendtoissue { get; set; }
        public string BatchNo { get; set; }
        public string UniqueBatchno { get; set; }
        public string ItemSize { get; set; }
        public string AltUnit { get; set; }
        public float AltQty { get; set; }
        public string PONO { get; set; }
        public int POYearCode { get; set; }
        public string PODate { get; set; }
        public string SchNo { get; set; }
        public string SchDate { get; set; }
        public int SchYearCode { get; set; }
        public string FromDateBack { get; set; }
        public string ToDateBack { get; set; }
        public string GateNoBack { get; set; }
        public string PartCodeBack { get; set; }
        public string ItemNameBack { get; set; }
        public string ChallanNoBack { get; set; }
        public string GlobalSearchBack { get; set; }
        public string SummaryDetailBack { get; set; }
        public string AccountNameBack { get; set; }
    }

    public class ReceiveChallanDashboard
    {
        public int EntryId { get; set; }
        public int Yearcode { get; set; }
        public string Entrydate { get; set; }
        public string RetNonRetChallan { get; set; }
        public string AgainstMRNOrGate { get; set; }
        public string MRNNo { get; set; }
        public int AgainstMRNYearCode { get; set; }
        public string BillOrChallan { get; set; }
        public string gateno { get; set; }
        public int GateYearCode { get; set; }
        public string ChallanNo { get; set; }
        public string ChallanDate { get; set; }
        public string Account_Name { get; set; }
        public string TruckNo { get; set; }
        public string TransPort { get; set; }
        public int DeptTo { get; set; }
        public string Remark { get; set; }
        public string SendforQC { get; set; }
        public float TotalAmount { get; set; }
        public float NetAmt { get; set; }
        public float TotalDiscountPercent { get; set; }
        public float TotalDiscountAmount { get; set; }
        public string ChallanType { get; set; }
        public string DocTypeCode { get; set; }
        public string InvoiceNo { get; set; }
        public int InvoiceYearCode { get; set; }
        public string pendcompleted { get; set; }
        public string MachineName { get; set; }
        public string CreatedByEmp { get; set; }
        public string UpdatedByEmp { get; set; }
        public string CC { get; set; }
        public int UID { get; set; }
        public string GateDate { get; set; }
        public string MRNDate { get; set; }
        public string CreatedOn { get; set; }
        public string UpdatedOn { get; set; }
        public int IssueChallanEntryId { get; set; }
        public int IssueChallanYearCode { get; set; }
        public string IssueChallanNo { get; set; }
        public int ItemCode { get; set; }
        public string PartCode { get; set; }
        public string ItemName { get; set; }
        public int SeqNo { get; set; }
        public string Unit { get; set; }
        public float RecQty { get; set; }
        public float Rate { get; set; }
        public float Amount { get; set; }
        public float IssuedQty { get; set; }
        public float PendQty { get; set; }
        public string Produced { get; set; }
        public float GateQty { get; set; }
        public int Storeid { get; set; }
        public string StoreName { get; set; }
        public string pendtoissue { get; set; }
        public string BatchNo { get; set; }
        public string UniqueBatchno { get; set; }
        public string ItemSize { get; set; }
        public string AltUnit { get; set; }
        public float AltQty { get; set; }
        public string PONO { get; set; }
        public int POYearCode { get; set; }
        public string PODate { get; set; }
        public string SchNo { get; set; }
        public string SchDate { get; set; }
        public int SchYearCode { get; set; }
    }
    public class RCDashboard : ReceiveChallanDashboard
    {
        public string FromDate { get; set; }    
        public string ToDate { get; set; }  
        public string SummaryDetail { get; set; }
        public IList<ReceiveChallanDashboard> RCDashboardList { get; set; }
         public string Searchbox { get; set; }
    }
}
