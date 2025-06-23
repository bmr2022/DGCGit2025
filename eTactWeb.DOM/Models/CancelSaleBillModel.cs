using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eTactWeb.DOM.Models
{
    public class CancelSaleBillModel: CancelSaleBillDetails
    {
        public string? FromDate { get; set; }
        public string? ToDate { get; set; }
        public string? SaleBillNo { get; set; }
      
        public IList<CancelSaleBillDetails>? CancelSaleBillDetails { get; set; }
    }
   public class CancelSaleBillDetails
    {
        public string CanRequisitionNo { get; set; }
        public DateTime CanSaleBillReqDate { get; set; }
        public string CustomerName { get; set; }
        public string GSTNO { get; set; }

        public string SaleBillNo { get; set; }
        public int SaleBillYearCode { get; set; }
        public DateTime SaleBillDate { get; set; }

        public decimal BillAmt { get; set; }
        public decimal INVNetAmt { get; set; }

        public string PartCode { get; set; }
        public string Item_Name { get; set; }
        public string HSNNO { get; set; }

        public decimal BillQty { get; set; }
        public decimal BillRate { get; set; }
        public string Unit { get; set; }
        public decimal ItemAmount { get; set; }
        public string Reasonofcancel { get; set; }

        public string SONO { get; set; }
        public DateTime SODate { get; set; }
        public int SOYearCode { get; set; }

    }
}
