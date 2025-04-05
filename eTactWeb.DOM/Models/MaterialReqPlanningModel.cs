using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eTactWeb.DOM.Models
{
    public class MaterialReqPlanningModel
    {
        public string Mode { get; set; }
        public string Flag { get; set; }

        public int? Year_Code { get; set; }
        public int? mrpno { get; set; }
        public string? months { get; set; }
        public string? FromDate { get; set; }
        public string? ToDate { get; set; }
        public string? PartCode { get; set; }
        public string? ItemName { get; set; }
        public string? SChNo { get; set; }
        public string? ReportType { get; set; }

        public IList<DayWiseMRPData>? DayWiseMRPDataGrid { get; set; }
    }

    public class DayWiseMRPData
    {
        public string? Party_Name { get; set; }
        public string? Part_code { get; set; }
        public string? Item_Name { get; set; }
        public decimal? Req_Qty { get; set; }
        public string? unit { get; set; }
        public decimal? Current_Stock { get; set; }
        public decimal? StoreStock { get; set; }
        public decimal? TotalStock { get; set; }
        public decimal? MinLevel { get; set; }
        public decimal? OrderQty { get; set; }
        public decimal? WIPStock { get; set; }
        public decimal WIP_Qty { get; set; }
        public decimal? Order_Qty { get; set; }
        public decimal? SChOrderQty { get; set; }
        public decimal? d1 { get; set; }
        public decimal? d2 { get; set; }
        public decimal? d3 { get; set; }
        public decimal? d4 { get; set; }
        public decimal? d5 { get; set; }
        public decimal? d6 { get; set; }
        public decimal? d7 { get; set; }
        public decimal? d8 { get; set; }
        public decimal? d9 { get; set; }
        public decimal? d10 { get; set; }
        public decimal? d11 { get; set; }
        public decimal? d12 { get; set; }
        public decimal? d13 { get; set; }
        public decimal? d14 { get; set; }
        public decimal? d15 { get; set; }
        public decimal? d16 { get; set; }
        public decimal? d17 { get; set; }
        public decimal? d18 { get; set; }
        public decimal? d19 { get; set; }
        public decimal? d20 { get; set; }
        public decimal? d21 { get; set; }
        public decimal? d22 { get; set; }
        public decimal? d23 { get; set; }
        public decimal? d24 { get; set; }
        public decimal? d25 { get; set; }
        public decimal? d26 { get; set; }
        public decimal? d27 { get; set; }
        public decimal? d28 { get; set; }
        public decimal? d29 { get; set; }
        public decimal? d30 { get; set; }
        public decimal? d31 { get; set; }
        public decimal? Total_Qty { get; set; }
        public int? Year_Code { get; set; }
        public int? item_code { get; set; }
        public string? Sch_No { get; set; }
        public string? MRP_No { get; set; }

    }
}
