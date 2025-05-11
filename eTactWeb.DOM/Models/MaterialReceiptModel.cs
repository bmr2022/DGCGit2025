using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;
using static eTactWeb.DOM.Models.GateInwardModel;

namespace eTactWeb.DOM.Models;

[Serializable]
public class MRNDashboard
{
    // mm.Account_Name'VendorName', mm.MRNNo, mm.EntryDate 'MrnDate', mm.GateNo,mm.GateDate,
    //mm.InvNo, mm.InvDate, mm.Docname,mm.MRNQCCompleted,TotalAmt,NetAmt
    public string? GateNo { get; set; }
    public string? VendorName { get; set; }
    public string? EntryId { get; set; }
    public int? YearCode { get; set; }
    public string? MrnNo { get; set; }
    public string? InvNo { get; set; }
    public string? InvDate { get; set; }
    public string? Docname { get; set; }
    public string? MRNQCCompleted { get; set; }
    [Column(TypeName = "decimal(18, 4)")]
    public decimal? NetAmt { get; set; }
    [Column(TypeName = "decimal(18, 4)")]
    public decimal? TotalAmt { get; set; }
    public string? MrnDate { get; set; }
    public string? GateDate { get; set; }
    public string? EntryBy { get; set; }
    public string? UpdatedBy { get; set; }
    public string? ItemName { get; set; }
    public string? PartCode { get; set; }
    public string PONO { get; set; }
    public int PoYearCode { get; set; }
    public string SchNo { get; set; }
    public int SchYearCode { get; set; }
    public string PoType { get; set; }
    public int POAmendNo { get; set; }
    public string PODate { get; set; }
    public string Unit { get; set; }
    public string RateUnit { get; set; }
    public string AltUnit { get; set; }
    public decimal Qty { get; set; }
    public decimal AltQty { get; set; }
    public int NoOfCase { get; set; }
    public decimal BillQty { get; set; }
    public decimal RecQty { get; set; }
    public decimal AltRecQty { get; set; }
    public decimal ShortExcessQty { get; set; }
    public decimal Rate { get; set; }
    public decimal RateinOther { get; set; }
    public decimal Amount { get; set; }
    public decimal PendPOQty { get; set; }
    public string QCCompleted { get; set; }
    public decimal RetChallanPendQty { get; set; }
    public string BatchWise { get; set; }
    public string SaleBillNo { get; set; }
    public int SaleBillYearCode { get; set; }
    public string AgainstChallanNo { get; set; }
    public string BatchNo { get; set; }
    public string UniqueBatchNo { get; set; }
    public string SupplierBatchNo { get; set; }
    public decimal ShelfLife { get; set; }
    public string ItemSize { get; set; }
    public string ItemColor { get; set; }
    public int TotalRecords { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public IList<MRNDashboard>? MRNQDashboard { get; set; }
}


//grid data for dashboard
public class MRNQDashboard : MRNDashboard
{
    public string? FromDate { get; set; }
    public string? ItemName { get; set; }
    public string? PartCode { get; set; }
    public string? PONo { get; set; }
    //right
    public IList<TextValue>? MRNList { get; set; }
    public IList<TextValue>? GateNOList { get; set; }
    public string? ToDate { get; set; }
    public string? FromDate1 { get; set; }
    public string? ToDate1 { get; set; }
    public string? DashboardType { get; set; }
}
[Serializable()]
public class MaterialReceiptDetail : TimeStamp
{
    public string? VendorBatchMand { get; set; }
    public string PONO { get; set; }
    public int PoYearCode { get; set; }
    public string SchNo { get; set; }
    public string SchDate { get; set; }
    public int SchYearCode { get; set; }
    public string PoType { get; set; }
    public int POAmendNo { get; set; }
    public string PODate { get; set; }
    public int ItemNumber { get; set; }
    public string ItemName { get; set; }
    public string PartCode { get; set; }
    public int ItemCode { get; set; }
    public string Unit { get; set; }
    public string RateUnit { get; set; }
    public string AltUnit { get; set; }
    public decimal Qty { get; set; }
    public decimal AltQty { get; set; }
    public int NoOfCase { get; set; }
    public decimal BillQty { get; set; }
    public decimal RecQty { get; set; }
    public decimal AltRecQty { get; set; }
    public decimal ShortExcessQty { get; set; }
    public decimal Rate { get; set; }
    public decimal RateinOther { get; set; }
    public decimal Amount { get; set; }
    public decimal PendPOQty { get; set; }
    public string QCCompleted { get; set; }
    public decimal RetChallanPendQty { get; set; }
    public string BatchWise { get; set; }
    public string SaleBillNo { get; set; }
    public int SaleBillYearCode { get; set; }
    public string AgainstChallanNo { get; set; }
    public string BatchNo { get; set; }
    public string UniqueBatchNo { get; set; }
    public string SupplierBatchNo { get; set; }
    public decimal ShelfLife { get; set; }
    public string ItemSize { get; set; }
    public string ItemColor { get; set; }
    public int SeqNo { get; set; }
}

public class BatchDetailModel
{
    public int SeqNo { get; set; }
    public int EntryId { get; set; }
    public int YearCode { get; set; }
    public int? ItemCode { get; set; }
    public string? ItemName { get; set; }
    public string? PartCode { get; set; }
    public string? PONO { get; set; }

    public string? PODate { get; set; }
    public int? POYearCode { get; set; }
    public string? SchNO { get; set; }

    public string? SchDate { get; set; }
    public int? SchYearCode { get; set; }
    public decimal? Qty { get; set; }
    public decimal? RecQty { get; set; }
    public decimal? VendorBatchQty { get; set; }
    public string? VendorBatchNo { get; set; }
    public string? UniqueBatchNO { get; set; }
    public string? ManufactureDate {get; set; }
    public string? ExpiryDate { get; set; }
    public string CC { get; set; }

}
public class MaterialReceiptModel : MaterialReceiptDetail
{
    public string? IsError { get; set; }
    public string? FinFromDate { get; set; }
    public string? FinToDate { get; set; }
    public int EntryID { get; set; }
    public string EntryDate { get; set; }
    public int YearCode { get; set; }
    public string MRNNo { get; set; }
    public string GateNo { get; set; }
    public string GateDate { get; set; }
    public int GateYearCode { get; set; }
    public string InvoiceNo { get; set; }
    public string InvoiceDate { get; set; }
    public string DocType { get; set; }
    public int DOCID { get; set; }

    public string? DateIntact {  get; set; }
    public string? FromDate { get; set; }
    public string? ToDate { get; set; }
    public int AccountID { get; set; }
    public string AccountCode { get; set; }
    public decimal? TotalAmt { get; set; }
    public decimal? NetAmt { get; set; }
    public string CheckQC { get; set; }
    public string Remark { get; set; }
    public int RecStoreId { get; set; }
    public string RecInStore { get; set; }
    public string QCCompleted { get; set; }
    public string ItemServType { get; set; }
    public decimal TareWeight { get; set; }
    public decimal GrossWeight { get; set; }
    public decimal NetWeight { get; set; }
    public int CurrencyID { get; set; }
    
    public string CurrencyName { get; set; }
    public string? Address { get; set; }


    public string PurchBillVoucherNo { get; set; }
    public int PurchBillNoYearCode { get; set; }
    public string PurchaseBillPosted { get; set; }
    public int DepartId { get; set; }
    public string? DepartIdName { get; set; }
    public int RespEmdId { get; set; }
    public string? ResEmpName { get; set; }
    public string ModeOfTransport { get; set; }
    public string FOC { get; set; }
    public string CC { get; set; }
    public int UID { get; set; }
    public int EnteredEmpId { get; set; }
    public DateTime? ActualEntryDate { get; set; }
    public int ActualEnteredBy { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public int? UpdatedBy { get; set; }
    public string? UpdatedByName { get; set; }
    public string PONO { get; set; }
    public int PoYearCode { get; set; }
    public string SchNo { get; set; }
    public int SchYearCode { get; set; }
    public string PoType { get; set; }
    public string ItemCode { get; set; }
    public string PartCode { get; set; }
    public string Unit { get; set; }
    public string RateUnit { get; set; }
    public string AltUnit { get; set; }
    public int NoOfCase { get; set; }
    public decimal BillQty { get; set; }
    public decimal RecQty { get; set; }
    public decimal AltRecQty { get; set; }
    public decimal Qty { get; set; }
    public decimal AltQty { get; set; }
    public int SeqNo { get; set; }
    public decimal ShortExcessQty { get; set; }
    public decimal BatchNo { get; set; }
    public decimal Rate { get; set; }
    public decimal RateinOther { get; set; }
    public decimal Amount { get; set; }
    public decimal PendPOQty { get; set; }
    public decimal RetChallanPendQty { get; set; }
    public int ItemNumber { get; set; }
    public string BatchWise { get; set; }
    public string SaleBillNo { get; set; }
    public int SaleBillYearCode { get; set; }
    public string AgainstChallanNo { get; set; }
    public string UniqueBatchNo { get; set; }
    public string SupplierBatchNo { get; set; }
    public decimal ShelfLife { get; set; }
    public string ItemSize { get; set; }
    public string ItemColor { get; set; }
    public int POAmendNo { get; set; }
    public string PODate { get; set; }
    public string? EnteredByEmpname { get; set; }
    public string? ActualEntryByName { get; set; }
    public string? EntryByMachineName { get; set; }
    public IList<TextValue>? DocumentList { get; set; }
    public IList<TextValue>? AccountList { get; set; }
    public IList<TextValue>? EmployeeList { get; set; }
    public IList<MaterialReceiptDetail>? ItemDetailGrid { get; set; }
    public IList<BatchDetailModel>? BatchDetailGrid { get; set; }
    public string VendorNameBack { get; set; }
    public string FromDateBack { get; set; }
    public string ToDateBack { get; set;}
    public string GateNoBack { get; set; }  
    public string ItemNameBack { get; set; }
    public string PartCodeBack { get; set; }
    public string MrnNoBack { get; set; }
    public string PoNoBack { get; set; }
    public string GlobalSearchBack { get; set; }
    public string DashboardTypeBack { get; set; }
}

