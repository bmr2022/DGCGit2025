using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eTactWeb.DOM.Models
{
    public class SOCancelModel: SoCancelDetail
    {
        public string? FromDate { get; set; }
        public string? ToDate { get; set; }
        public string? VendorName { get; set; }
        public string? SONo { get; set; }
        public string? CancelationType { get; set; }
        public string? CC { get; set; }
        public string? CustOrderNo { get; set; }
        public IList<SoCancelDetail>? SOCancelDetailGrid { get; set; }
    }
    public class SoCancelDetail
    {
        public string? SONO { get; set; }
        public string? SODate { get; set; }
        public string? WEF { get; set; }
        public string? OrderType { get; set; }
        public string? SOType { get; set; }
        public string? SOFor { get; set; }
        public string? CustomerName { get; set; }
        public string? ItemName { get; set; }
        public string? PartCode { get; set; }
        public int HSNNo { get; set; }
        public decimal Qty { get; set; }
        public string? Unit { get; set; }
        public decimal AltQty { get; set; }
        public string? AltUnit { get; set; }
        public decimal Rate { get; set; }
        public string? UnitRate { get; set; }
        public string? Remark { get; set; }
        public decimal PendQty { get; set; }
        public decimal PendAltQty { get; set; }
        public int EntryID { get; set; }
        public int Year { get; set; }

    }

}
