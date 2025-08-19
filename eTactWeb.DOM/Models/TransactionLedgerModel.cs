using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.DOM.Models
{
    public class TransactionLedgerModel:TimeStamp
    {
        public string Flag { get; set; } 
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public int YearCode { get; set; } 
        public string LedgerName { get; set; }
        public string Group_Ledger { get; set; }
        public int AccountCode { get; set; } 
        public string ParentLedgerName { get; set; } 
        public string FillAllGroup { get; set; }
        public string GroupName { get; set; }
        public string ParentLedger { get; set; }
        public string ReportType { get; set; }
        public int ParentAccountCode { get; set; }
        public string VoucherType { get; set; }
        public string VoucherNo { get; set; }
        public string INVNo { get; set; }
        public string Narration { get; set; }
        public float Amount { get; set; }
        public string Dr { get; set; }
        public string Cr { get; set; }
        public string Mode { get; set; }
        public string AccountName {  get; set; }
        public int AccEntryId { get; set; }
        public int AccEntryYearCode { get; set; }
        public string VoucherDocDate { get; set; }
        public string Particulars { get; set; }
        public string InvoiceVoucherNo { get; set; }
        public decimal DrAmt { get; set; }
        public decimal CrAmt { get; set; }
        public decimal Balance { get; set; }
        public string Types { get; set; }
        public string HeadWiseNarration { get; set; }
        public string? BillDate { get; set; }
        public int DocEntryId { get; set; }
        public string SumDet { get; set; }
        public string VCHEMark { get; set; }
        public string VchNo { get; set; }
        public string MOnthFullName { get; set; }
        public decimal TotalDr { get; set; }
        public decimal TotalCr { get; set; }
        public decimal ClosingAmt { get; set; }
        public string Dr_CR { get; set; }
        public int  SeqNo{ get; set; }
        public decimal OpnDr {  get; set; }
        public decimal OpnCr {  get; set; }
        public decimal TotalOpening {  get; set; }
        public decimal CurrDrAmt {  get; set; }
        public decimal CurrCrAmt {  get; set; }
        public decimal NetCurrentAmt {  get; set; }
        public decimal NetAmount {  get; set; }
        public string? GroupLedger {  get; set; }
		public IList<TransactionLedgerModel> TransactionLedgerGrid{ get;set; }
    } 
}
