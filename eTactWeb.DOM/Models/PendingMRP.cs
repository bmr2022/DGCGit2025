using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.DOM.Models
{
    public class PendingMRPDetail : TimeStamp
    {
        public string SODate { get; set; }
        public int SOEntryId { get; set; }
        public string SOTYPE { get; set; }
        public int schYearCode { get; set; }
        public int schEntryID { get; set; }
        public string sch_date { get; set; }
        public float orderQty { get; set; }
        public float FGstock { get; set; }
        public float SaleRate { get; set; }
        public float IIndMonthQty { get; set; }
        public float IIIrdMonthQty { get; set; }
        public string BOM { get; set; }

    }

    [Serializable]
    public class PendingMRP : PendingMRPDetail
    {
        public int SeqNo { get; set; }
        public int Month { get; set; }
        public int ForMonthYear { get; set; }
        public int YearCode { get; set; }
        public int accountcode { get; set; }
        public string account_name { get; set; }
        public string sono { get; set; }
        public int SOYearCode { get; set; }
        public string ScheduleNo { get; set; }
        public string MRPGenDate { get; set; }
        public int MRPEntryId { get; set; }
        public string FGItem { get; set; }
        public string MRPNo { get; set; }
        public string StoreId { get; set; }
        public string WcId { get; set; }
        public List<PendingMRP>? PendingMRPGrid { get; set; }
        public string? IncludeProjection { get; set; }
    }
}

