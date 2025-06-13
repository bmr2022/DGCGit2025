using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eTactWeb.DOM.Models
{
    public class EInvoiceItemModel
    {
        public int EntryId { get; set; }
        public string InvoiceNo { get; set; }
        public int YearCode { get; set; }
        public string saleBillType { get; set; }
        public string customerPartCode { get; set; }
    }
}
