using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eTactWeb.DOM.Models
{
    public class PSApprovalModel : PSApprovalDetail
    {
        public string? FromDate { get; set; }
        public string? ToDate { get; set; }
        public string? VendorName { get; set; }
        public string? PONo { get; set; }
        public string? SchNo { get; set; }
        public string? ApprovalType { get; set; }
        public string? CC { get; set; }

        public IList<PSApprovalDetail>? PSAppDetailGrid { get; set; }

    }
    public class PSApprovalDetail 
    {
        //public string? ShowOnlyAmendItem { get; set; }
        public string? SchNo { get; set; }
        public DateTime? Schdate { get; set; }
        public string? PONO { get; set; }
        public DateTime? PODate { get; set; }
        public string? VendorName { get; set; }
        public string? DeliveryAddress { get; set; }
        public DateTime? SchEffFromDate { get; set; }
        public DateTime? SchEffTillDate { get; set; }
        public string? SchApproved { get; set; }
        public string? ApprovedBy { get; set; }
        public string? Remarks { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int EntryID { get; set; }
        public int? SchYear { get; set; }
        public int POYearCode { get; set; }
        public string PartCode { get; set; }
        public string ItemName { get; set; }
        public string DeliveryDate { get; set; }
        public decimal SchQty { get; set; }
        public decimal Rate { get; set; }
        public string UserName { get; set; }
    }

}
