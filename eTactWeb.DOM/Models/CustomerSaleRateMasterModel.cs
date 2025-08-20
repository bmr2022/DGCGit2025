using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.DOM.Models
{
    public class CustomerSaleRateMasterModel: TimeStamp
    {
        public long CNRMEntryId { get; set; }
        public long CNRMYearCode { get; set; }
        public string? CNRMSlipNo { get; set; }
        public long RevNo { get; set; }
        public string? CNRMEntryDate { get; set; }
        public long SeqNo { get; set; }
        public string? EffectiveDate { get; set; }
        public long AccountCode { get; set; }
        public long ItemCode { get; set; }
        public string? CustLocation { get; set; }
        public string? PartCode { get; set; }
        public string? ItemName { get; set; }

        public decimal Rate { get; set; }
        public string? RateUnit { get; set; }
        public decimal PrevRate { get; set; }
        public string? EffectiveTillDate { get; set; }
        public long LastRateEntryId { get; set; }
        public long LastrateYearCode { get; set; }
        public string? EntryByMachineName { get; set; }
        public long ActualEnteredBy { get; set; }
        public long LastUpdatedBy { get; set; }
        public string? LastUpdatedDate { get; set; }
       
    }
}
