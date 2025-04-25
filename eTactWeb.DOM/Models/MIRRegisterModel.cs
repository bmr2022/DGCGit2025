using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eTactWeb.DOM.Models
{
    public class MIRRegisterModel
    {
        public string? FromDate { get; set; }
        public string? ReportType { get; set; }
        public string? MRNType { get; set; }

        //public string? MRNType { get; set; }
        public string? ToDate { get; set;} 
         public string? PartCode { get; set; }
        public string? ItemName { get; set; }
         public string? ReportMode { get; set; }
        public string? GateNo {  get; set; }
        public string? MIRNo { get; set; }

        public string? MRNno { get; set; }
        public string? MRNJWCustJW { get; set; }
        public string? PONo { get; set; }
        public string? Schno { get; set; }
        public string? VendorName { get; set; }
        public string? invoiceNo { get; set; } 
        public IList<MIRRegisterDetail>? MIRRegisterDetail { get; set; }
        //public IList<GateEntryRegisterDetail>? GateEntryRegisterDetail { get; set; }
    }
    public class MIRRegisterDetail
    { 
        public int SeqNo { get; set; }
        public string? GateNo { get; set; }
        //public string? MRNJWCustJW { get; set; }
        public string? MRNNo { get; set; }
        public string? MRNDate { get; set; }

        public string? MIRNo { get; set; }
        public string? MIRDate { get; set; }
        public string? GateDate { get; set; }
        public string? DocumentType { get; set; }
        public string? State { get; set; }
        public string? EntryDate { get; set; }
        public string? VendorName { get; set; }
        public string? InvoiceNo { get; set; }
        public string? Account_Name { get; set; }
        public string? InvoiceDate { get; set; }
        public string? DocNo { get; set; } 
        public string? PartCode { get; set; }
        public string? MRNJWCustJW { get; set; }
        public string? FromStore { get; set; }
        public string? AcceptStore { get; set; }
        public string? RejStore { get; set; }
        public string? RewStore { get; set; }

        
        public string? ReportType { get; set; }
        public string? MRNType { get; set; }
        public string? PurchaseBillPosted { get; set; }

        //public string? MRNType { get; set; }
        public string? ItemName { get; set; }
        public string? PONo { get; set; }
        public string? PODate { get; set; }
        public string? POType { get; set; }
        public Int16? POYearCode { get; set; }
        public string? QCCompleted { get; set; }
        public string? NeedTochkQC { get; set; }
        public string? MRNQCCompleted { get; set; }
        public string? Batchno { get; set; }
        public string? Uniquebatchno { get; set; }
        public string? SupplierBatchNo { get; set; }
        public Int64? ShelfLife { get; set; }
        public decimal? TotalAmt { get; set; }
        public string? Currency { get; set; }
        public string? EntryByMachineName { get; set; }
        public string? SchNo { get; set; }
        public string? SchDate { get; set; }
        public decimal?BillQty { get; set;}
        public decimal? RecQty { get; set; } 
        public decimal TotalBillQty {  get; set; }
        public decimal ShortExcessQty {  get; set; }
        public string? Unit { get; set; }
        public decimal? Rate { get; set;}
        public decimal? AcceptedQty {  get; set; }
        public decimal? AltAcceptedQty { get; set; }
        public decimal? RejectedQty { get; set; }
        public decimal? HoldQty { get; set; }
        public decimal? Reworkqty { get; set; }
         
        public decimal? PendPOQty { get; set; }
        public decimal? AltPendQty { get; set; }
         public string? ItemRemark { get; set; }
        public string? Size { get; set; }
        public string? Color { get; set; }  
        public string? Remark { get; set; }
        public string? PreparedByEmp { get; set; }
        public string? ActualEntryByEmp { get; set; }
        public string? ActualEntryDate { get; set; }
        public string? LastUpdatedby { get; set; } 
         public string? CC { get; set; }  
        public decimal Total { get; set; } 
        public int SeqNum { get; set; }
        public string? FromDate { get; set; }
        public string? LastUpdatedDate { get; set; }
        public string? ToDate { get; set; }
        public decimal? Amount { get; set; } 
        public decimal? AltQty { get; set; } 
        public string? UpdatedByEMp { get; set; }
        public string? AltUnit { get; set; }
        public decimal? NetAmout { get; set; }
        public string? DepName { get; set; } 
        public string? RecInStore { get; set; }
        public string? VendAddress { get; set; }  
        public string? EntryBy { get; set; }

       
        public int NoOfMRN { get; set; }
        public int MRNITEM { get; set; }
        public int NoOfQC { get; set; }
        public int MIRITEM { get; set; }
        public int PendMRNForQC { get; set; }
        public int PendItemForQC { get; set; }
        public int TotalMRNPending { get; set; }
        public int TotalItemPending { get; set; }
    }
}
