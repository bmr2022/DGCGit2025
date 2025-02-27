using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eTactWeb.DOM.Models
{
    public class DayBookModel
    {
        public string Ledger { get; set; }
        public string VoucherNo { get; set; }
        public string InvoiceNo { get; set; }
        public string VoucherDate { get; set; }
        public string VoucherType { get; set; }
        public string DrAmt { get; set; }
        public string CrAmt { get; set; }
        public string EntryDate { get; set; }
        public string Category { get; set; }
        public string ParentHead { get; set; }
        public string EnteredBy { get; set; }
        public string EntryByMachine { get; set; }
        public int AccountCode { get; set; }
        public int AccEntryId { get; set; }
        public int AccYearCode { get; set; }
        public string SaleDocNo { get; set; }
        public string SaleVoucherType { get; set; } 
        public decimal SaleDrAmt { get; set; }  
        public decimal SaleCrAmt { get; set; }
        public int SaleBillEntryId { get; set; }
        public int SaleBillYearCode { get; set; } 
        public string FromDate { get; set; } 
        public string ToDate { get; set; } 
        public IList<DayBookModel> DayBookGrid { get; set; } 
    }
}
