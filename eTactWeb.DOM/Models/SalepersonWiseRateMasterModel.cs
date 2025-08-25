using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.DOM.Models
{
    public class SalepersonWiseRateMasterModel: TimeStamp
    {
        public int? EntryId { get; set; }
        public int? YearCode { get; set; }
        public string? EntryDate { get; set; }
        public string? PartCode { get; set; }
        public string? ItemName { get; set; }
        public int? SalesPersonId { get; set; }
        public int? ItemGroupId { get; set; }
        public int? ItemCode { get; set; }   // bigint, not datetime
        public decimal? OriginalRate { get; set; }
        public decimal? NewRate { get; set; }
        public int? ActualEntryBy { get; set; }
        public int? UpdatedBy { get; set; }
        public string? UpdationDate { get; set; }
        public string? MachineName { get; set; }
        public string? CC { get; set; }

        public decimal? AllRatePer { get; set; } // This property is not in the original model, added for completeness
        public decimal? AllRateAmount { get; set; }
        public string? ActualEntryDate { get; set; }
        public string? GroupName { get; set; }
        public string? SalePersonName { get; set; }
        public string? ActualEntryByName { get; set; }
        public IList<SalepersonWiseRateDetail>? ItemDetailGrid { get; set; }
        public IList<SalepersonWiseRateMasterModel>? DashboardDetail { get; set; }
    }

    public class SalepersonWiseRateDetail
    {

       public int? ItemCode { get; set; }
        public string? ItemName { get; set; }
        public string? PartCode { get; set; }
        public decimal? OriginalRate { get; set; }
        public decimal? NewRate { get; set; }
       
    }
}
