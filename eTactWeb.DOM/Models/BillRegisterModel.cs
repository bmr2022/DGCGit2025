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
        public string BillType { get; set; }
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
       
        public string? InvoiceDate { get; set; }
        public string VoucherNo { get; set; }
        public string? VoucherDate { get; set; }
        public string MRNNo { get; set; }
        public string? MRNDate { get; set; }
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
        public string? ActualEntryDate { get; set; }
        public string LastUpdatedByEmp { get; set; }
        public string? LastUpdatedDate { get; set; }
        public string Remark { get; set; }
        public string Country { get; set; }
        public string Unit { get; set; }
        public double AprAmt { get; set; }
        public double MayAmt { get; set; }
        public double JunAmt { get; set; }
        public double JulAmt { get; set; }
        public double AugAmt { get; set; }
        public double SepAmt { get; set; }
        public double OctAmt { get; set; }
        public double NovAmt { get; set; }
        public double DecAmt { get; set; }
        public double JanAmt { get; set; }
        public double FebAmt { get; set; }
        public double MarAmt { get; set; }
        public double AprQty { get; set; }
        public double MayQty { get; set; }
        public double JunQty { get; set; }
        public double JulQty { get; set; }
        public double AugQty { get; set; }
        public double SepQty { get; set; }
        public double OctQty { get; set; }
        public double NovQty { get; set; }
        public double DecQty { get; set; }
        public double JanQty { get; set; }
        public double FebQty { get; set; }
        public double MarQty { get; set; }

        public string StateCode { get; set; }
        public string InvDate { get; set; }
        public string GateNo { get; set; }
        public string GateDate { get; set; }

        public decimal BillQty { get; set; }
        public decimal RecQty { get; set; }
        public decimal RejectedQty { get; set; }
        public decimal BillRate { get; set; }
        public decimal PoRate { get; set; }
        public decimal DiscountPer { get; set; }
        public decimal DisAmt { get; set; }
        public decimal ItemAmount { get; set; }
        public decimal TotalBillAmt { get; set; }
        public decimal TotaltaxableAmt { get; set; }
        public string MIRNO { get; set; }
        public string MIRDate { get; set; }
        public string ItemParentGroup { get; set; }
        public string ItemCategory { get; set; }
        public string PurchaseBillDirectPB { get; set; }
        public string PurchaseBillTypeMRNJWChallan { get; set; }
        
        public string PaymentTerm { get; set; }
        public string Approved { get; set; }
        public string ApprovedDate { get; set; }
        public string EntryByEmployee { get; set; }
        public IList<BillRegisterModel> BillRegisterList { get; set; }
    }
}
