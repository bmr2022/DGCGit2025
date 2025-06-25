using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eTactWeb.DOM.Models
{
    public class VendJWRegisterModel
    {
        public string? FromDate { get; set; }
        public string? ToDate { get; set; }
        public string? PartyName { get; set; }
        public string? PartCode { get; set; }
        public string? ItemName { get; set; }
        public string? IssueChallanType { get; set; }
        public string? ReportMode { get; set; }
        public string? IssueChallanNo { get; set; }
        public string? RecChallanNo { get; set; }
        public int TotalRecords { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public IList<VendJWRegisterDetail>? VendJWRegisterDetails { get; set; }
    }
}

    public class VendJWRegisterDetail
    {
        public string Account_Name { get; set; }
        public string VendorName { get; set; }
        public string ChallanNo { get; set; }
        public string JInvNo { get; set; }
        public string InvDate { get; set; }
        public string MrnNo { get; set; }
        public string Gateno { get; set; }
        public int Gateyearcode { get; set; }
        public string GateDate { get; set; }
        public string partcode { get; set; }
        public string Itemname { get; set; } 
        public string BillQty { get; set; }
        public string RecQty { get; set; }
        public string ShortExcess { get; set; }
        public string JwRate { get; set; }
        public string JwRateUnit { get; set; }
        public string unit { get; set; }
        public string ProcessUnit { get; set; }
        public string ItemAmount { get; set; }
        public string ProdUnprod { get; set; }
        public string BOMIND { get; set; }
        public int BomNo { get; set; }
        public string BomDate { get; set; }
        public string batchno { get; set; }
        public string uniquebatchno { get; set; }
        public int NoOfCase { get; set; }
        public string QCCompleted { get; set; }
        public string PONo { get; set; }
        public string PODate { get; set; }
        public string SchNo { get; set; }
        public string SchDate { get; set; }
        public string ItemRemark { get; set; }
        public int RecStoreId { get; set; }
        public int ProcessId { get; set; }
        public string Processsname { get; set; }
        public float Totalamt { get; set; }
        public string EneterdByEmpName { get; set; }
        public string UpdatedByEmpName { get; set; }
        public float NetAmt { get; set; }
        public string Remark { get; set; }
        public string EntryByMachineName { get; set; }
        public string ActualEnteredBy { get; set; }
        public string UpdatedBy { get; set; }
        public int ItemCode { get; set; }
        public int entryid { get; set; }
        public int YearCode { get; set; }
        public int PoYear { get; set; }
        public int SchYear { get; set; }
        public string ToStore { get; set; }
    public string JWChallanNo { get; set; }
    public string ChallanDate { get; set; }
    public string Gsttype { get; set; }
    public string VendorStateCode { get; set; }
    public float TolApprVal { get; set; }
    public float TotalWt { get; set; }
    public string TransporterName { get; set; }
    public float Distance { get; set; }
    public string EwayBillNo { get; set; }
    public string Closed { get; set; }
    public string Types { get; set; }
    public string CompletlyReceive { get; set; }
    //public string BatchNo { get; set; }
    public float IssQty { get; set; }
    public string RemarkDetail { get; set; }
    public int StoreId { get; set; }
    public float PurchasePrice { get; set; }
    public float StockQty { get; set;}
    public float BatchStockQty { get; set;}
    public int RecItemCode { get; set; }
    public float AltQty { get; set; }
    public string altUnit { get; set; }
    public float pendqty { get; set;}
    public float PendAltQty { get; set; }
    public float Actualpendqty { get; set; }
    public float Amount { get; set; }
    public string FORPROCESS { get; set; }
    public string STORENAME { get; set; }
    public int Seqno { get; set; }
    public decimal BOMqTY { get; set; }
    public string IssChallanNo { get; set; }
    public string RecChallanno { get; set; }
    public string IssChallanDate { get; set; }
    public string RecChallanDate { get; set; }
    public float AdjqTY { get; set; }
    public string RecItemName { get; set; }
    public string RecPartCode { get; set; }
    public string IssueChallanNo { get; set; }
    public string IssuePartCode { get; set; }
    public string IssueItemName { get; set; }
    //public int AdjQty { get; set; }
    //public int PendQty { get; set; }
    //public string BOMInd { get; set; }
    public int TotRecQty { get; set; }
    //public string CLOSED { get; set; }
}


