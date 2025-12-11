using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.DOM.Models;

[Serializable]
public class MIRDashboard
{

    //                     mm.Account_Name 'VendorName',mm.MIRNo, mm.MIRDate, mm.MRNNO, mm.MRNDate, mm.GateNo,
    //mm.MRNJWCustJW, mm.INVNo, mm.Invdate,
    //mm.EntryId, mm.EntryDate, mm.YearCode
    public string? VendorName { get; set; }
    public int? EntryId { get; set; }
    public int? YearCode { get; set; }
    public string? InvNo { get; set; }
    public string? MRNNo { get; set; }
    public string? GateNo { get; set; }

    public string? EnteredBy { get; set; }
    public string? UpdatedBy { get; set; }
    public string? MIRNo { get; set; }
    public string? MRNJWCustJW { get; set; }

    public string? MRNDate { get; set; }
    public string? MIRDate { get; set; }


    public string? InvDate { get; set; }
    public string? EntryDate { get; set; }
    public string? PurchaseBillBooked { get; set; }
    public string? MaterialIssued { get; set; }
    public string? partcode { get; set; }
    public string? item_name { get; set; }
    public decimal BillQty { get; set; }
    public decimal RecQty { get; set; }
    public decimal AcceptedQty { get; set; }
    public decimal AltAcceptedQty { get; set; }
    public int OkRecStore { get; set; }
    public decimal RejectedQty { get; set; }
    public decimal AltRejectedQty { get; set; }
    public int RejRecStore { get; set; }
    public decimal HoldQty { get; set; }
    public decimal Reworkqty { get; set; }
    public decimal DeviationQty { get; set; }
    public int ResponsibleEmpForDeviation { get; set; }
    public string? PONo { get; set; }
    public int POYearCode { get; set; }
    public string? SchNo { get; set; }
    public int SchYearCode { get; set; }
    public string? Unit { get; set; }
    public string? AltUnit { get; set; }
    public decimal AltRecQty { get; set; }
    public string? Remarks { get; set; }
    public string? Defaulttype { get; set; }
    public int ApprovedByEmp { get; set; }
    public string? Color { get; set; }
    public string? ItemSize { get; set; }
    public string? ResponsibleFactor { get; set; }
    public string? SupplierBatchno { get; set; }
    public decimal shelfLife { get; set; }
    public string? BatchNo { get; set; }
    public string? uniqueBatchno { get; set; }
    public string? AllowDebitNote { get; set; }
    public string? FilePath { get; set; }
    public IList<MIRDashboard>? MIRQDashboard { get; set; }
}


public class MIRQDashboard : MIRDashboard
{
    public IList<MIRDashboard>? MIRDashboard { get; set; }

    public string? FromDate { get; set; }
    public string? ItemName { get; set; }
    public string? PartCode { get; set; }
    //right
    public IList<TextValue>? InvList { get; set; }
    public IList<TextValue>? MRNList { get; set; }
    public IList<TextValue>? GateList { get; set; }
    public IList<TextValue>? MIRList { get; set; }
    public string? DashboardType { get; set; }
    public string? ToDate { get; set; }
    public string? Searchbox { get; set; }
}
[Serializable()]
public class MirDetail : TimeStamp
{
    public int? MirTotalRows { get; set; }

    public int? SeqNo { get; set; } //
    public string PONo { get; set; }
    public int POYearCode { get; set; }
    public string SchNo { get; set; }
    public int SchYearCode { get; set; }
    public string SchDate { get; set; }
    public int ItemCode { get; set; }
    public string PartCode { get; set; }
    public string ItemName { get; set; }
    public string Unit { get; set; }
    public string AltUnit { get; set; }
    public decimal BillQty { get; set; }
    public decimal RecQty { get; set; }
    public decimal ALtRecQty { get; set; }
    public decimal Rate { get; set; }
    public decimal RateInOtherCurr { get; set; }
    public decimal Amount { get; set; }
    public decimal PendPOQty { get; set; }
    public string QCCompleted { get; set; }
    public decimal RetChallanPendQty { get; set; }
    public string BatchWise { get; set; }
    public string SaleBillNo { get; set; }
    public int SaleBillYearCode { get; set; }
    public string AgainstChallanNo { get; set; }
    public string SupplierBatchNo { get; set; }
    public int ShelfLife { get; set; }
    public string ItemSize { get; set; }
    public string Itemcolor { get; set; }
    public string IssueToStore { get; set; }
    public decimal batchqty { get; set; }
    public string batchnotype { get; set; }
    public int number { get; set; }
    public decimal AcceptedQty { get; set; }
    public decimal AltAcceptedQty { get; set; }
    public int OkRecStore { get; set; }//
    public string OkRecStoreName { get; set; }
    public string RejRecStoreName { get; set; }
    public string HoldStoreName { get; set; }
    public string RewokStoreName { get; set; }
    public string PoType { get; set; }
    public string PODate { get; set; }
    public string RateUnit { get; set; }
    public int NoofCase { get; set; }
    public decimal DeviationQty { get; set; }
    public int ResponsibleEmpForDeviation { get; set; }//
    public decimal RejectedQty { get; set; }
    public decimal AltRejectedQty { get; set; }
    public string StoreName { get; set; }
    public int RejRecStore { get; set; }//
    public string Remarks { get; set; }
    public string DefaultType { get; set; }
    public int ApprovedByEmp { get; set; }//
    public decimal HoldQty { get; set; }//
    public int HoldStoreId { get; set; }//
    public int ProcessId { get; set; }
    public decimal ReworkQty { get; set; }//
    public int RewokStoreId { get; set; }//
    public string Color { get; set; }
    public string ResponsibleFactor { get; set; }
    public string BatchNo { get; set; }
    public string UniqueBatchNo { get; set; }
    public string AllowDebitNote { get; set; }
    public string? PathOfFile { get; set; }
    public IFormFile? PathOfFile1 { get; set; }
    public string? PathOfFileURL { get; set; }
    public string? MRNNo { get; set; }
    public int MRNYearCode { get; set; }
    public string? MRNJwCust { get; set; }
    public IList<MirDetail>? ItemDetail { get; set; }
    public IFormFile? ServerFilePath { get; set; }
}

public class MirModel : MirDetail
{
    public string? EntryByMachineName { get; set; }
    public string? IPAddress { get; set; }
    public string? ActualEnteredByName { get; set; }
    public string? FinFromDate { get; set; }
    public string? UpdatedByName { get; set; }
    public string? DateIntact { get; set; }
    public int EntryId { get; set; }
    public int YearCode { get; set; }
    public string EntryDate { get; set; }
    public string MIRNo { get; set; }
    public string MIRDate { get; set; }
    public string MRNJWCustJW { get; set; }
    private IList<SelectListItem> _MRNCombo = new List<SelectListItem>()
        {
            new() { Value = "MRN", Text = "MRN" },
            new() { Value = "JW", Text = "JW" },
            new() { Value = "CustJW", Text = "Cust JW" },
        };
    public IList<SelectListItem> MRNList
    {
        get => _MRNCombo;
        set => _MRNCombo = value;
    }

    public string? AccountName { get;set; }
    public string? FromPend { get; set; }
    public string? FromDate { get; set; }
    public string? ToDate { get; set; } 

    public int MRNYearCode { get; set; }
    public int MRNEntryId { get; set; }
    public string MRNNo { get; set; }
    public string MRNDate { get; set; }
    public string GateNo { get; set; }
    public int GateEntryId { get; set; }

    public int GateYearCode { get; set; }
    public bool AC1 { get; set; }
    public int AccountCode { get; set; }
    public IList<TextValue>? VendorList { get; set; }
    public string INVNo { get; set; }
    public string INVDate { get; set; }
    public string Hold { get; set; }//
    public string QcType { get; set; }//
    public int JobWorkEntryId { get; set; }//
    public int FromStoreId { get; set; }
    public string ItemServTypes { get; set; }

    public IList<MirDetail>? ItemDetailGrid { get; set; }
    public IList<MIRFromPend>? MIRFromPendDetail { get; set; }
    public IList<MirDetail>? ItemDetail { get; set; }

    private IList<SelectListItem> _Type = new List<SelectListItem>()
        {
            new() { Value = "I", Text = "Item" },
            new() { Value = "S", Text = "Service" },
        };

    public IList<SelectListItem> TypeList
    {
        get => _Type;
        set => _Type = value;
    }
    public string ModeOfTransport { get; set; }
    public string HoldQC { get; set; }//
    public int EnteredByEmpId { get; set; }//
    public int Uid { get; set; }//
    public string CC { get; set; }
    public IList<TextValue>? BranchList { get; set; }

    //grid detail
    public int? SeqNo { get; set; } //
    public string PONo { get; set; }
    public int POYearCode { get; set; }
    public string SchNo { get; set; }
    public int SchYearCode { get; set; }
    public int ItemCode { get; set; }
    public string PartCode { get; set; }
    public string ItemName { get; set; }
    public string Unit { get; set; }
    public string AltUnit { get; set; }
    public decimal BillQty { get; set; }
    public decimal RecQty { get; set; }
    public decimal ALtRecQty { get; set; }
    public decimal AcceptedQty { get; set; }
    public decimal AltAcceptedQty { get; set; }
    public int OkRecStore { get; set; }//
    public decimal DeviationQty { get; set; }
    public int ResponsibleEmpForDeviation { get; set; }//
    public decimal RejectedQty { get; set; }
    public decimal AltRejectedQty { get; set; }
    public int RejRecStore { get; set; }//
    public string StoreName { get; set; }
    public string Remarks { get; set; }
    public string DefaultType { get; set; }
    public int ApprovedByEmp { get; set; }//
    public decimal HoldQty { get; set; }//
    public int HoldStoreId { get; set; }//
    public int ProcessId { get; set; }
    public decimal ReworkQty { get; set; }//
    public int RewokStoreId { get; set; }//
    public string Color { get; set; }
    public string ItemSize { get; set; }
    public string ResponsibleFactor { get; set; }
    public string SupplierBatchNo { get; set; }
    public decimal ShelfLife { get; set; }
    public string BatchNo { get; set; }
    public string UniqueBatchNo { get; set; }
    public string AllowDebitNote { get; set; }
    public string? PathOfFile { get; set; }
    public string? PathOfFileURL { get; set; }
    public IFormFile? ServerFilePath { get; set; }
    public string MRNNoBack { get; set; }
    public string FromDateBack { get; set; }
    public string ToDateBack { get; set; }
    public string? ItemNameBack { get; set; }
    public string? VendorNameBack { get; set; }
    public string MRNJWBack { get; set; }
    public string? PartCodeBack { get; set; }
    public string? InvNoBack { get; set; }
    public string? DeptNameBack { get; set; }
    public string? DashboardTypeBack {  get; set; }
    public string? Searchbox {  get; set; }
    public string OkRecStoreName { get; set; }
    public string RejRecStoreName { get; set; }
    public string HoldStoreName { get; set; }
    public string RewokStoreName { get; set; }
    //grid detail over

    public class ListModelMIR
    {
        public ListModelMIR()
        {
            ItemDetailGrid = new List<MirDetail>();
        }

        public IEnumerable<IFormFile> formFileArray;
        public List<MirDetail> ItemDetailGrid { get; set; }
    }

    public class MIRFromPend
    {
        public string MRNNo { get; set; }
        public string MRNJW { get; set; }
        public int MRNYear { get; set; }
        public int MRNEntryId { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string GateNo { get; set; }
        
    }
}