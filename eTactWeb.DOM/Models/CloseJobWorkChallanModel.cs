using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eTactWeb.DOM.Models
{
    public class CloseJobWorkChallanModel
    {

        // Master (JobworkIssueMain + Account info)
        public string ShowClsoedPendingAll { get; set; }
        public string MachineName { get; set; }
        public string Mode { get; set; }
        public int ActualEntryId { get; set; }
        public string ActualEntryByEmpName { get; set; }
        public string EntryDate { get; set; }
        public int EmpId { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string VendorName { get; set; }

        public string VendJWIssChallanNo { get; set; }
        public string VendJWIssChallanDate { get; set; }
        public decimal TolApprVal { get; set; }
        public string Closed { get; set; }
        public string PartCode { get; set; }
        public string Item_Name { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal NetAmount { get; set; }
        public int? Processdays { get; set; }
        public string Remark { get; set; }
        public int VendJWIssEntryId { get; set; }
        public int VendJWIssYearCode { get; set; }

        // Detail (JobworkIssueDetail)
        public int ItemCode { get; set; }
        public decimal Rate { get; set; }
        public decimal Amount { get; set; }
        public decimal IssQty { get; set; }
        public string DetailClosed { get; set; }   // renamed to avoid conflict
        public decimal PendQty { get; set; }
        public decimal PendAltQty { get; set; }

        public List<CloseJobWorkChallanModel> Details { get; set; }
    }
}
