using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eTactWeb.DOM.Models
{
    public class PendingSaleRejectionModel
    {
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string CustInvoiceNo { get; set; }
        public string GateNo { get; set; }
        public string MrnNo { get; set; }
        public string CustomerName { get; set; }
        public string GlobalSearch { get; set; }
    }
}
