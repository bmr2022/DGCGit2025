using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.DOM.Models
{
    public class PendingMRNToQC
    {
        public string? UserType { get; set; }
        public string? FromDate { get; set; }
        public string? ToDate { get; set;}
        public string? MrnJW { get; set; }
        public string? MrnNo { get; set; }
        public string? VendorName { get; set; }
        public string? InvoiceNo { get; set; }
        public int? ItemCode { get; set; }
        public string? ItemName { get; set; }
        public string? PartCode { get; set; }
        public int? DeptName { get; set; }  

        public IList<TextValue>? AccountList { get; set; }
        public IList<TextValue>? InvNoList { get; set; }
        public IList<TextValue>? MRNNoList { get; set; }
        public IList<TextValue>? ItemNameList { get; set; }
        public IList<TextValue>? PartCodeList { get; set; }
        public IList<TextValue>? DeptList { get; set; }

        private IList<SelectListItem> _Type = new List<SelectListItem>()
        {
            new() { Value = "MRN", Text = "MRN" },
            new() { Value = "VENDOR JOBWORK", Text = "Vendor JW" },
            new() { Value = "CUSTOMER JOBWORK", Text = "Customer JW" },
        };

        public IList<SelectListItem> MRNJWList
        {
            get => _Type;
            set => _Type = value;
        }
    }
}
