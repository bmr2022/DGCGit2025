using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eTactWeb.DOM.Models
{
    public class CancelRequitionModel
    {
        public string Flag { get; set; } 
        public string RequitionType { get; set; }
        public string ReqNo { get; set; }
        public string VendorName { get; set; }
        public string ApprovalType { get; set; }
        public int ReqYearCode { get; set; } 
        public string? ReqDate { get; set; }
        public string EntryByMachine { get; set; } 
        public int EntryByEmpId { get; set; }
        public string PendCanceledReq { get; set; } 
        public string CC { get; set; } 
        public string? FromDate { get; set; }
        public string? ToDate { get; set; }
        public int ReqEntryId { get; set; } 
        public string? CancelDate { get; set; }
        public string CancelReason { get; set; }
        public int ItemCode { get; set; }

        public string Workcenter { get; set; }
        public string Department { get; set; }
        public string Remarks { get; set; } 
        public string ReqByEmp { get; set; } 
        public string EntryByEmp { get; set; } 
        public string Branch { get; set; } 
        public string Completed { get; set; } 
        public string EntryByMachineName { get; set; } 
        public string Cancel { get; set; }
        public string CancelReasonDisplay { get; set; }

        public int SeqNo { get; set; }
        public string PartCode { get; set; } 
        public string ItemName { get; set; }
        public decimal ReqQty { get; set; }
        public decimal PendQty { get; set; }
        public string? ExpectedDate { get; set; }
        public string Unit { get; set; } 
        public decimal Stock { get; set; }
        public string StoreName { get; set; } 
        public string ItemRemark { get; set; } 
        public string ItemCanceled { get; set; } 
        public string ItemLocation { get; set; }
        public string ItemBinRackNo { get; set; }

        public int TotItemCt { get; set; }
        public int CanItemCt { get; set; }
    }
}
