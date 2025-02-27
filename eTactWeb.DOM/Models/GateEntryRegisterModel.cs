using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eTactWeb.DOM.Models
{
    public class GateEntryRegisterModel
    {

        public string? FromDate { get; set; }
        public string? ReportType { get; set; }
        public string? ToDate { get; set;} 
         public string? PartCode { get; set; }
        public string? ItemName { get; set; }
         public string? ReportMode { get; set; }
        public string? gateno {  get; set; }
        public string? docname { get; set; }
        public string? PONo { get; set; }
        public string? Schno { get; set; }
        public string? VendorName { get; set; }
        public string? invoiceNo { get; set; } 
        public IList<GateEntryRegisterDetail>? GateEntryRegisterDetail { get; set; }
    }
    public class GateEntryRegisterDetail
    {


        public decimal TotStk { get; set; }
        public int SeqNo { get; set; }
        public string? GateNo { get; set; }
        public string? GDate { get; set; }
        public string? EntryDate { get; set; }
        public string? VendorName { get; set; }
        public string? InvoiceNo { get; set; }
        public string? InvoiceDate { get; set; }
        public string? DocNo { get; set; }
        public string? POTypeServiceItem { get; set; }
        public string? PartCode { get; set; }
        public string? ReportType { get; set; }
        public string? ItemName { get; set; }
        public string? PONo { get; set; }
        public string? EntryByMachineName { get; set; }
        public string? SchNo { get; set; }
        public decimal? Qty { get; set;}
         public string? Unit { get; set; }
        public decimal? Rate { get; set;}
        public decimal? AltQty { get; set; }
        public decimal? PendPOQty { get; set; }
        public decimal? AltPendQty { get; set; }
        public string? SaleBillNo { get; set; }
        public int? SaleBillYearCode { get; set; }
        public decimal? SaleBillQty { get; set; }
        public decimal? ChallanQty { get; set; }
        public string? ItemRemark { get; set; }
        public string? Size { get; set; }
        public string? Color { get; set; }
        public string? SupplierBatchNo { get; set; }
        public int? ShelfLife { get; set; }
        public string? POtype { get; set; }
        public string? Remark { get; set; }
        public string? PreparedByEmp { get; set; }
        public string? ActualEntryByEmp { get; set; }
        public string? ActualEntryDate { get; set; }
        public string? LastUpdatedby { get; set; }
        public DateTime? Updationdate { get; set; }

        public string? CompanyAddress { get; set; }
        public string? City { get; set; }
        public string? Transporter { get; set; }
        public string? Truck{ get; set; }
        public string? DriverName { get; set; }
        public string? CC { get; set; }
        public int GateEntryId { get; set; }
        public int GateYearCode { get; set; }
        public int POYearCode { get; set; }
        public int SchYearCode { get; set; }

        public decimal Total { get; set; } 
        public int SeqNum { get; set; }
        public string? FromDate { get; set; }
        public string? LastUpdatedDate { get; set; }
        public string? ToDate { get; set; }
        public decimal? Amount { get; set; }
        public decimal? AgainstChallanQty { get; set; }
        public string? AgainstChallanNo { get; set; }
        public string? ActualEntryByEMp {  get; set; }
        public string? UpdatedByEMp { get; set; }
        public string? AltUnit { get; set; }
        public decimal? Amout { get; set; }


    }
}
