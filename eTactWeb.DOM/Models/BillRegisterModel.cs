using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eTactWeb.DOM.Models
{
    public class BillRegisterModel
    {
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string PartCode { get; set; }
        public string BillNo { get; set; }
        public string? BillDate { get; set; }
        public string VendorName { get; set; }
        public decimal Qty { get; set; }
        public decimal Rate { get; set; }
        public decimal GrossAmount { get; set; }
        public decimal GSTAmount { get; set; }
        public decimal NetAmount { get; set; }

       
        public decimal TotalQty { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal TotalGST { get; set; }
        public decimal TotalNet { get; set; }

      
        public decimal Discount { get; set; }
        public string EntryBy { get; set; }
        public string? EntryDate { get; set; }

        public string MonthName { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string ReportType { get; set; }
        public string DocumentName { get; set; }
        public string PONO { get; set; }
        public string SchNo { get; set; }
        public string HsnNo { get; set; }
        public string GstNo { get; set; }
        public string InvoiceNo { get; set; }
        public string PurchaseBill { get; set; }
        public string PurchaseRejection { get; set; }
        public string DebitNote { get; set; }
        public string CreditNote { get; set; }
        public string SaleRejection { get; set; }
        public string Description { get; set; }
        public string ForTheDuration { get; set; }
        public string ForFinYear { get; set; }
       
        public DateTime? InvoiceDate { get; set; }
        public string VoucherNo { get; set; }
        public DateTime? VoucherDate { get; set; }
        public string MRNNo { get; set; }
        public DateTime? MRNDate { get; set; }
        public decimal? BillAmt { get; set; }
        public decimal? TaxableAmt { get; set; }
    
        public string Currency { get; set; }
        public decimal? TotalBillQty { get; set; }
        public decimal? TotalDisAmt { get; set; }
        public decimal? TotalItemAmt { get; set; }
        public decimal? CGSTPer { get; set; }
        public decimal? CGSTAmt { get; set; }
        public decimal? SGSTPer { get; set; }
        public decimal? SGSTAmt { get; set; }
        public decimal? IGSTPer { get; set; }
        public decimal? IGSTAmt { get; set; }
        public decimal? ExpenseAmt { get; set; }
        public decimal? InvAmt { get; set; }
        public string GSTType { get; set; }
        public string GSTNO { get; set; }
        public string VendorAddress { get; set; }
        public string State { get; set; }
        public string DirectPBOrAgainstMRN { get; set; }
        public string EntryByMachineName { get; set; }
        public string TypeItemServAssets { get; set; }
        public string DomesticImport { get; set; }
        public int? PaymentDays { get; set; }
        public int PurchBillEntryId { get; set; }
        public int PurchBillYearCode { get; set; }
        public string ActualEntryByEmp { get; set; }
        public DateTime? ActualEntryDate { get; set; }
        public string LastUpdatedByEmp { get; set; }
        public DateTime? LastUpdatedDate { get; set; }
        public string Remark { get; set; }
        public string Country { get; set; }
        public IList<BillRegisterModel> BillRegisterList { get; set; }
    }
}
