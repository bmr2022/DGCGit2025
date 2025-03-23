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
            public string? ApprovalType { get; set; }
            public string? CC { get; set; }

            public IList<POCancleDetail>? POCancelDetailGrid { get; set; }

        }
        public class POCancleDetail
        {
            public string? Vendor { get; set; }
            public string? PONo { get; set; }
            public string? PoDate { get; set; }
            public string? WEF { get; set; }
            public string? OrderType { get; set; }
            public string? POType { get; set; }
            public string? POFor { get; set; }
            public string? POCloseDate { get; set; }
            public string? POTypeServItem { get; set; }
            public string? AmmNo { get; set; }
            public string? AmmEffDate { get; set; }
            public int? VendorAddress { get; set; }
            public decimal ShipTo { get; set; }
            public decimal? basicamount { get; set; }
            public decimal? NetAmount { get; set; }
            public int? POentryid { get; set; }
            public int POYearCode { get; set; }
            
        }
    }

