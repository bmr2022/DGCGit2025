using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;
using static eTactWeb.DOM.Models.MirModel;

namespace eTactWeb.DOM.Models;

public class JobWorkReceiveItemDetail : TimeStamp
{
    public int SeqNo { get; set; }
    public int ItemCode { get; set; }
    public string ItemName { get; set; }
    public string PartCode { get; set; }
    public string Unit { get; set; }    
    public decimal BillQty { get; set; }
    public decimal RecQty { get; set; }
    public decimal JWRate { get; set; }
    public decimal Amount { get; set; }
    public string Remark { get; set; }
    public string ProducedUnprod { get; set; }
    public int ProcessId { get; set; }
    public int StoreId { get; set; }
    public string Adjusted { get; set; }
    public int BomRevNo { get; set; }
    public string BomRevDate { get; set; }
    public decimal NoOfCase { get; set; }
    public string QCCompleted { get; set; }
    public string PONO { get; set; }
    public int POYearCode { get; set; }
    public string PODate { get; set; }
    public string SchNo { get; set; }
    public string SchYearCode { get; set; }
    public string SchDate { get; set; }
    public string POType { get; set; }
    public string BomInd { get; set; }
    public string BatchWise { get; set; }
    public string JWRateUnit { get; set; }
    public decimal ProcessUnitQty { get; set; }
    public string ProcessUnit { get; set; }
    public int POAmendNo { get; set; }
    public int AccountCode { get; set; }
    public string BatchNo { get; set; }
    public string UniqueBatchNo { get; set; }
    public decimal TotalPrice { get; set; }
    public IList<JobWorkReceiveItemDetail>? JobWorkReceiveItemDetails { get; set; }
}
public class JobWorkReceiveModel : JobWorkReceiveItemDetail
{
    public string? FinFromDate { get; set; }
    public string? FinToDate { get; set; }
    public string? IPAddress { get; set; }
    public string? EntryByMachine { get; set; }

    public int EntryId { get; set; }
    public int YearCode { get; set; }
    public string? EntryDate { get; set; }
    public bool AC1 { get; set; }
    public int AccountCode { get; set; }
    public string AccountName { get; set; }
    public string? RecInStore { get; set; }
    public string InvNo { get; set; }
    public string? InvDate { get; set; }
    public string MRNNo { get; set; }
    public string GateNo { get; set; }
    public int GateYearCode { get; set; }
    public string? GateDate { get; set; }    
    public int RecStoreid { get; set; }
    public string Through { get; set; }
    public string Remark { get; set; }
    public int UID { get; set; }
    public string CC { get; set; }
    public IList<TextValue>? BranchList { get; set; }
    public string QCCheck { get; set; }
    public string DocumentTypeBillChal { get; set; }
    private IList<SelectListItem> _Type = new List<SelectListItem>()
        {
            new() { Value = "Bill", Text = "Bill" },
            new() { Value = "Challan", Text = "Challan" },
        };

    public IList<SelectListItem> DocTypeList
    {
        get => _Type;
        set => _Type = value;
    }
    public string JobTypeJwRW { get; set; }
    private IList<SelectListItem> _JobType = new List<SelectListItem>()
        {
            new() { Value = "Jobwork", Text = "Jobwork" },
            new() { Value = "Rework", Text = "Rework" },
        };

    public IList<SelectListItem> JobTypeList
    {
        get => _JobType;
        set => _JobType = value;
    }
    public string PurBillNo { get; set; }
    public int PurBillYear { get; set; }
    public string Adjusted { get; set; }
    public string QCCompleted { get; set; }
    public string BlockShortageDebitNote { get; set; }
    public string PurchaseBillPosted { get; set; }
    public string TypesBOMIND { get; set; }
    private IList<SelectListItem> _TypeBI = new List<SelectListItem>()
        {
            new() { Value = "BOM", Text = "BOM" },
            new() { Value = "Individual", Text = "Individual" },
        };

    public IList<SelectListItem> TypesBOMINDList
    {
        get => _TypeBI;
        set => _TypeBI = value;
    }
    public int EmpID { get; set; }
    public string FOC { get; set; }
    public decimal TotalAmt { get; set; }
    public decimal NetAmt { get; set; }
    public int EnteredByEmpId { get; set; } //
    public DateTime ActualEntryDate { get; set; }//
    public int ActualEnteredBy { get; set; }//
    public string? ActualEnteredByName { get; set; }
    public int SeqNo { get; set; }
    public int ItemCode { get; set; }
    public string ItemName { get; set; }
    public string PartCode { get; set; }
    public string Unit { get; set; }
    public decimal BillQty { get; set; }
    public decimal RecQty { get; set; }
    public decimal JWRate { get; set; }
    public decimal Amount { get; set; }
    public string ProducedUnprod { get; set; }
    public int ProcessId { get; set; }
    public int StoreId { get; set; }
    public int BomRevNo { get; set; }
    public string BomRevDate { get; set; }
    public decimal NoOfCase { get; set; }
    public string PONO { get; set; }
    public int POYearCode { get; set; }
    public string PODate { get; set; }
    public string SchNo { get; set; }
    public string SchDate { get; set; }
    public string POType { get; set; }
    public string BomInd { get; set; }
    public string BatchNo { get; set; }
    public string UniqueBatchNo { get; set; }
    public string BatchWise { get; set; }
    public string JWRateUnit { get; set; }
    public decimal ProcessUnitQty { get; set; }
    public string ProcessUnit { get; set; }
    public int POAmendNo { get; set; }
    public int AgainstChallanYearCode { get; set; }
    public string ChallanDate { get; set; }
    public string IssItem { get; set; }
    public decimal PendQty { get; set; }
    public string RecItem { get; set; }
    public int RecQtyGrid { get; set; }
    public int TotalQty { get; set; }
    public int TotalAdjQty { get; set; }
    public string CreatedByName { get; set; }
    public string UpdatedByname { get; set; }
    public string VendorNameBack { get; set; }
    public string ItemNameBack { get; set; }
    public string PartCodeBack { get; set; }
    public string InvoiceNoBack { get; set; }
    public string MRNNoBack { get; set; }
    public string GateNoBack { get; set; }
    public string BranchBack { get; set; }
    public string FromDateBack { get; set; }
    public string ToDateBack { get; set; }
    public string DashboardTypeBack { get; set; }
    public string GlobalSearchBack { get; set; }
    public IList<JobWorkReceiveDetail>? ItemDetailGrid { get; set; }
    public IList<JobWorkReceiveItemDetail>? JobWorkReceiveGrid { get; set; }

    public class JobWorkReceiveDetail
    {
        public int SeqNo { get; set; }
        public int EntryIdIssJw { get; set; }
        public int YearCodeIssJw { get; set; }
        public string IssChallanNo { get; set; }
        public string IssChallanDate { get; set; }
        public int IssYearCode { get; set; }
        public int AccountCode { get; set; }
        public int ItemCode { get; set; }
        public string IssItemName { get; set; }
        public string ItemName { get; set; }
        public string IssPartCode { get; set; }
        public int EntryIdRecJw { get; set; }
        public int YearCodeRecJw { get; set; }
        public int FinishItemCode { get; set; }
        
        public string FinishPartCode { get; set; }
        public string FinishItemName { get; set; }
        public decimal AdjQty { get; set; }
        public string AdjFormType { get; set; }
        public string TillDate { get; set; }
        public decimal TotRecQTy { get; set; }
        public decimal PendQty { get; set; }
        public decimal BOMQty { get; set; }
        public int BOMrevno { get; set; }
        public string BOMRevDate { get; set; }

        public int ProcessId { get; set; }
        public string CC { get; set; }
        public string BOMInd { get; set; }
        public decimal RecQty { get; set; }
        public decimal TotaladjQty { get; set; }
        public decimal TotalRecQty { get; set; }
        public decimal TotalIssuedQty { get; set; }
        public int RunnerItemCode { get; set; }
        public int ScrapItemCode { get; set; }
        public decimal IdealScrapQty { get; set; }
        public decimal IssuedScrapQty { get; set; }
        public string PreRecChallanNo { get; set; }
        public decimal ScrapqtyagainstRcvQty { get; set; }
        public string IssuedBatchNO { get; set; }
        public string IssuedUniqueBatchNo { get; set; }
        public string Recbatchno { get; set; }
        public string Recuniquebatchno { get; set; }
        public string ScrapAdjusted { get; set; }
        public string Through { get; set; }
        public IList<JobWorkReceiveDetail>? JobWorkReceiveGrid { get; set; }


        //public int ChallanEntryID { get; set; }
        //public string ChallanNo { get; set; }
        //public int ChallanYearCode { get; set; }
        //public string ChallanDate { get; set; }
        //public string IssItem { get; set; }
        //public int PendQty { get; set; }
        //public string RecItem { get; set; }
        //public string RecQty { get; set; }
        //public int RecQtyGrid { get; set; }
        //public int TotalQty { get; set; }
        //public int TotalAdjQty { get; set; }

        //public string BatchNo { get; set; }
        public string UniqueBatchNo { get; set; }
    }
}
public class JWRData
{
    public int EntryIdIssJw { get; set; }
    public string IssJWChallanNo { get; set; }
    public int IssYearCode { get; set; }
    public DateTime ChallanDate { get; set; }
    public string IssPartCode { get; set; }
    public string IssItemName { get; set; }
    public int IssItemCode { get; set; }
    public int BomNo { get; set; }
    public DateTime BOMDate { get; set; }
    public string BomStatus { get; set; }
    public double PendQty { get; set; }
    public string FinishPartcode { get; set; }
    public string FinishItemName { get; set; }
    public int RecItemCode { get; set; }
    public double bomqty { get; set; }
    public string through { get; set; }
    public double QtyToBeRec { get; set; }
    public double ActualAdjQty { get; set; }
    public string batchno { get; set; }
    public string uniquebatchno { get; set; }
}
//data for partial GRID
public class JWReceiveDashboard
{
    public string? VendorName { get; set; }
    public string GateNo { get; set; }
    public string GateDate { get; set; }
    public int GateYearCode { get; set; }
    public string MRNNo { get; set; }
    public string MRNDate { get; set;}
    public string InvNo { get; set; }
    public string InvDate { get; set;}
    public string QCCompleted { get; set; }
    public decimal TotalAmt { get; set; }
    public decimal NetAmt { get; set; }
    public string PurchaseBillPosted { get; set; }
    public int EntryId { get; set; }
    public int YearCode { get; set; }
    public string BranchName { get; set; }
    public string EnteredBy { get; set; }
    public string UpdatedBy { get; set; }
    //
    public string? SearchMode { get; set; }
    public string? ItemName { get; set; }
    public string? PartCode { get; set; }
    public string? Unit { get; set; }
    public string? BillQty { get; set; }
    public string? RecQty { get; set; }
    public string? Amount { get; set; }
    public string? ProducedUnProd { get; set; }
    public string? BomRevNo { get; set; }
    public string? PONo { get; set; }
    public string? POYearCode { get; set; }
    public string? PODate { get; set; }
    public string? BatchNo { get; set; }
    public string? UniqueBatchNo { get; set; }

    public IList<JWReceiveDashboard>? JWRecQDashboard { get; set; }
}

//for search
public class JWRecQDashboard : JWReceiveDashboard
{
    public string? VendorName { get; set; }
    public string? ItemName { get; set; }
    public string? PartCode { get; set; }
    public string? CC { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set;}
    public string? FromDate1 { get; set; }
    public string? ToDate1 { get; set; }
    public string SearchBox { get; set; }
    public string? DashboardType { get; set; }
}

public class ChallanDetail
{
    public string Unit { get; set; }
    public float BillQty { get; set; }
    public float JWRate { get; set; }
    public string PONO { get; set; }
    public int POYearCode { get; set; }
    public string SchNo { get; set; }
    public int SchYearCode { get; set; }
    public string ProdUpProd { get; set; }
    public float RecQty { get; set; }
    public int ItemCode { get; set; }
    public string ItemName { get; set; }
    public int BOMrevno { get; set; }
    public string BOMRevDate { get; set; }
    public int ProcessId { get; set; }
    public string BOMInd { get; set; }
}
