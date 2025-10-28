using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eTactWeb.DOM.Models
{
    public class FilterState
    {
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string ReportType { get; set; }
        public string GroupOrLedger { get; set; }
        public int? ParentAccountCode { get; set; }
        public int? AccountCode { get; set; }
        public string VoucherType { get; set; }
        public string VoucherNo { get; set; }
        public string InvoiceNo { get; set; }
        public string Narration { get; set; }
        public float? Amount { get; set; }
        public string DR { get; set; }
        public string CR { get; set; }
        public string Ledger { get; set; }
        public string GlobalSearch { get; set; }
    }
}
