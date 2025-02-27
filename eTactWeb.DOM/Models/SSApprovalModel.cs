using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eTactWeb.DOM.Models
{
    public class SSApprovalModel : SSApprovalDetail
    {
        public string? FromDate { get; set; }
        public string? ToDate { get; set; }
        public string? VendorName { get; set; }
        public string? SONO { get; set; }
        public string? SchNo { get; set; }
        public string? ApprovalType { get; set; }
        public string? CC { get; set; }
        public IList<SSApprovalDetail>? SSApprovalGrid { get; set; }
    }
    public class SSApprovalDetail 
    {
        public string? ApprovedBy { get; set; }
        public string? CreatedBy { get; set; }
        public string? CreatedOn { get; set; }
        public string? CustomerName { get; set; }
        public string? DeliveryAddress { get; set; }
        public int EntryID { get; set; }
        public string? SchApproved { get; set; }
        public string? SchDate { get; set; }
        public string? SchEffFromDate { get; set; }
        public string? SchEffTillDate { get; set; }
        public string? SchNo { get; set; }
        public string? SchYear { get; set; }
        public string? SODate { get; set; }
        public int SONO { get; set; }
        public string? DeliveryDate { get; set; }
        public decimal SchQty { get; set; }
        public decimal Rate { get; set; }
        public int SOYearCode { get; set; }
        public string? PartCode { get; set; }
        public string? ItemName { get; set; }
    }

}
