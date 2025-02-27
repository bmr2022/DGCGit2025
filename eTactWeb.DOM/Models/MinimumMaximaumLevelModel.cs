using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.DOM.Models
{
    public class MinimumMaximaumLevelModel: TimeStamp
    {
        public int YearCode { get; set; }
        public string CurrentDate { get; set; }
        public string ItemName { get; set; }
        public string PartCode { get; set; }
        public float MinimumLevel { get; set; }
        public float MaximumLevel { get; set; }
        public float MaxWipStock { get; set; }
        public string Unit { get; set; }
        public string ParentGroup { get; set; }
        public string ItemCategory { get; set; }
        public float StoreStock { get; set; }
        public float WIPStock { get; set; }
        public string StockSource { get; set; }
        public float StoreShortQty { get; set; }
        public float StoreExcessQty { get; set; }
        public float WIPExcessQty { get; set; }
        public int ItemCode { get; set; }
        public float ReorderLevel { get; set; }
        public float PendPOQty { get; set; }
        public float ShortQtyAfterPOPendQty { get; set; }
        public string VendorName { get; set; }
        public string PONO { get; set; }
        public int AccountCode { get; set; }
        public DateTime? PODate { get; set; }
        public string SchNO { get; set; }
        public DateTime? SchDate { get; set; }
        public int POQty { get; set; }
        public float Rate { get; set; }
        public float PendValue { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string ReportType { get; set; }
        public string Store_Name { get; set; }
        public int Storeid { get; set; }
        public string ShowItem { get; set; }
        public int POYearCode { get; set; }
        public string ScheduleNo { get; set; }
        public int ScheduleYearCode { get; set; }
        public string ScheduleDate { get; set; }
        public IList<MinimumMaximaumLevelModel>? MinimumMaximaumLevelGrid { get; set; }
    }
}
