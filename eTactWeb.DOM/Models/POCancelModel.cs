using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eTactWeb.DOM.Models
{
    public class POCancelModel:POCancleDetail
    {
       
            public string? FromDate { get; set; }
            public string? ToDate { get; set; }
            public string? VendorName { get; set; }
            public string? PONo { get; set; }
            public string? CancelationType { get; set; }
            public string? CC { get; set; }

            public IList<POCancleDetail>? POCancelDetailGrid { get; set; }

        }
        public class POCancleDetail
        {
        public string? PONum { get; set; }
        public int SeqNo { get; set; }
        public string? PoDate { get; set; }
        public string? WEF { get; set; }
        public string? OrderType { get; set; }
        public string? POType { get; set; }
        public string? POFor { get; set; }
        public string? PoTypeServItem { get; set; }
        public string? PartyName { get; set; }
        public string? ItemName { get; set; }
        public string? PartCode { get; set; }
        public int? HSNNo { get; set; }
        public decimal POQty { get; set; }
        public string? Unit { get; set; }
        public decimal AltPoQty { get; set; }
        public string? AltUnit { get; set; }
        public decimal TolLimitPer { get; set; }
        public decimal TolLimitQty { get; set; }
        public string? UnitRate { get; set; }
        public decimal Rate { get; set; }
        public decimal RateInOther { get; set; }
        public decimal DiscPer { get; set; }
        public decimal DiscRs { get; set; }
        public decimal Amount { get; set; }
        public decimal AdditionalRate { get; set; }
        public decimal OldRate { get; set; }
        public string? Remark { get; set; }
        public string? Description { get; set; }
        public decimal PendQty { get; set; }
        public decimal PendAltQty { get; set; }
        public int Process { get; set; }
        public int AmmendmentNo { get; set; }
        public string? AmmendmentDate { get; set; }
        public string? AmmendmentReason { get; set; }
        public decimal FirstMonthTentQty { get; set; }
        public decimal SecMonthTentQty { get; set; }
        public int SizeDetail { get; set; }
        public string? Colour { get; set; }
        public int CostCenter { get; set; }
        public string? Active { get; set; }
        public string? RateApplicableOnUnit { get; set; }
        public decimal PkgStd { get; set; }
        public int EntryID { get; set; }
        public int YearCode { get; set; }
        public string? TypeOfCancel { get; set; }
        public string? Approved { get; set; }
        public string? Approval1Levelapproved { get; set; }
        public string? ApproveAmm { get; set; }


    }
    }

