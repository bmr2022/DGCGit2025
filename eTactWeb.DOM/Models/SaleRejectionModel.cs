using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.DOM.Models
{
    public class SaleRejectionModel : SaleRejectionDetail
    {
        public int SaleRejEntryId { get; set; }
        public int SaleRejYearCode { get; set; }
       public string ? AgainstVoucherNo { get; set; }
        public string SaleRejEntryDate { get; set; }
        public string GateNo { get; set; }
        public int Gateyearcode { get; set; }
        public string GateDate { get; set; }
        public bool PN1 { get; set; }
        public bool DT { get; set; }
        public int MRNEntryId { get; set; }
        public string MrnNo { get; set; }
        public string MRNDate { get; set; }
        public int Mrnyearcode { get; set; }
        public string SalerejCreditNoteVoucherNo { get; set; }
        public string VoucherNo { get; set; }
        public string CustInvoiceNo { get; set; }
        public string CustInvoiceDate { get; set; }
        public string CustInvoiceTime { get; set; }
        public int AccountCode { get; set; }
        public string Account_Name { get; set; }
        public float PaymentTerm { get; set; }
        public string GSTNO { get; set; }
        public string DomesticExportNEPZ { get; set; }
        public string ? Transporter { get; set; }
        public string ? Vehicleno { get; set; }
        public float BillAmt { get; set; }
        public float RoundOffAmt { get; set; }
        public string? RoundoffType { get; set; }
        public decimal Taxableamt { get; set; }
        public decimal ToatlDiscountPercent { get; set; }
        public decimal TotalDiscountAmount { get; set; }
        public decimal InvNetAmt { get; set; }
        public string   ? SalerejRemark { get; set; }
        public string CC { get; set; }
        public int Uid { get; set; }
        public int EntryByempId { get; set; }
        public string? MachineName { get; set; }
        public string   ?ActualEntryDate { get; set; }
        public int ActualEnteredBy { get; set; }
        public string ? ActualEnteredByName { get; set; }
        public int LastUpdatedBy { get; set; }
        public string? LastUpdatedByName { get; set; }
        public string   ? LastUpdationDate { get; set; }
        public int CurrencyId { get; set; }
        public char? BalanceSheetClosed { get; set; }
        public string? FinFromDate { get; set; }
        public string? FinToDate { get; set; }
        public int DocTypeAccountCode { get; set; }

        [Column(TypeName = "decimal(10, 4)")]
        public decimal NetTotal { get; set; }

        [Column(TypeName = "decimal(10, 4)")]
        public decimal TotalAmtAftrDiscount { get; set; }

        [Column(TypeName = "decimal(10, 4)")]
        public decimal TotalDiscountPercentage { get; set; }

        public string? TotalRoundOff { get; set; }
        public bool? RDT { get; set; }
        public int? RoundOffAccountCode { get; set; }
        [Column(TypeName = "decimal(10, 4)")]
        public decimal TotalRoundOffAmt { get; set; }
        public List<SaleRejectionDetail> SaleRejectionInputGrid { get; set; }
        public List<SaleRejectionDetail> SaleRejectionDetails { get; set; }
        public IList<SaleRejectionDetail>? ItemDetailGrid { get; set; }
        public IList<DPBItemDetail>? DPBItemDetails { get; set; }
        public IList<DbCrModel>? DbCrGrid { get; set; }
        //public IList<DbCrModel>? DRCRGrid { get; set; }   

        private IList<SelectListItem> _YesNo = new List<SelectListItem>()
        {
            new() { Value = "Y", Text = "Yes" },
            new() { Value = "N", Text = "No" },
        };

        public IList<SelectListItem> YesNoList
        {
            get => _YesNo;
            set => _YesNo = value;
        }
    }

    public class SaleRejectionDetail : TaxModel
    {
        public AdjustmentModel adjustmentModel { get; set; }
        public string? AgainstBillTypeJWSALE { get; set; }
        public string? AgainstBillNo { get; set; }
        public long AgainstBillYearCode { get; set; }
        public long AgainstBillEntryId { get; set; }
        public string? AgainstOpnOrBill { get; set; }
        public int SeqNo { get; set; }

        public int ItemCode { get; set; }
        public string ItemName { get; set; }
        public string PartCode { get; set; }
        public bool ItemSA { get; set; }
        public string? Unit { get; set; }
        public int HSNNo { get; set; }
        public double NoOfCase { get; set; }
        public float SaleBillQty { get; set; }
        public double RejQty { get; set; }
        public float RejRate {  get; set; } 
        public double RecQty { get; set; }
        public double Rate { get; set; }
        public float AltQty { get; set; }
        public double DiscountPer { get; set; }
        public decimal DiscountAmt { get; set; }
        public string   ?SONO { get; set; }
        public long SOyearcode { get; set; }
        public string ? SODate { get; set; }
        public string? CustOrderNo { get; set; }
        public string ?SOAmmNo { get; set; }
        public string? Itemsize { get; set; }
        public long RecStoreId { get; set; }
        public string? RecStoreName { get; set; }
        public string? OtherDetail { get; set; }
        public decimal Amount { get; set; }
        public string? RejectionReason { get; set; }
        public string? SaleorderRemark { get; set; }
        public string   ? SaleBillremark { get; set; }
        [Column(TypeName = "decimal(10, 4)")]
        public decimal ItemNetAmount { get; set; }
        public bool SH { get; set; }
    }
    public class SaleRejectionDashboard : SaleRejectionModel
    {
        public string? SummaryDetail { get; set; }
        public string   ?SearchBox { get; set; }
        public List<SaleRejectionDashboard> saleRejectionDashboard { get; set; }
    }
}
