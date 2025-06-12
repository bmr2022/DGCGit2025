using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eTactWeb.DOM.Models
{
    public class OrderBasedProdPlanModel
    {
        public int SaleSchEntryID { get; set; }
        public int ForTheMonth { get; set; }
        public int SaleSchYearCode { get; set; }
        public int SOEntryID { get; set; }
        public string SONO { get; set; }
        public int SOYearCode { get; set; }
        public int Accountcode { get; set; }
        public string AccountName{ get; set; }
        public string ScheduleNo { get; set; }
        public string TentetiveConfirm { get; set; }
        public string Active { get; set; }
        public string SchCompleted { get; set; }
        public string SchApproved { get; set; }
        public int ItemCode { get; set; }
        public string DeliveryDate { get; set; }
        public decimal Rate { get; set; }
        public decimal Qty { get; set; }
        public decimal AltSchQty { get; set; }
        public string SCHOrder { get; set; }
        public string? SODate { get; set; }
        public string? Schdate { get; set; }
        public string MAinBomSubBom { get; set; }
        public int FGItemcode { get; set; }
        public string CustOrderNo { get; set; }
        public string SheduleNo { get; set; }
        public int BOMITEMCODE { get; set; }
        public int Finishitemcode { get; set; }
        public decimal RMQty { get; set; }
        public decimal SOQty { get; set; }
        public decimal PlanQty { get; set; }
        public int WCID { get; set; }
        public string ItemName { get; set; }
        public string WorkCenterName { get; set; }
        public string PartCode { get; set; }
        public string WCName { get; set; }
        public string MainSubBOm { get; set; }
      

        public decimal? Day1 { get; set; }
        public decimal? Day2 { get; set; }
        public decimal? Day3 { get; set; }
        public decimal? Day4 { get; set; }
        public decimal? Day5 { get; set; }
        public decimal? Day6 { get; set; }
        public decimal? Day7 { get; set; }
        public decimal? Day8 { get; set; }
        public decimal? Day9 { get; set; }
        public decimal? Day10 { get; set; }
        public decimal? Day11 { get; set; }
        public decimal? Day12 { get; set; }
        public decimal? Day13 { get; set; }
        public decimal? Day14 { get; set; }
        public decimal? Day15 { get; set; }
        public decimal? Day16 { get; set; }
        public decimal? Day17 { get; set; }

        public decimal? Day18 { get; set; }
        public decimal? Day19 { get; set; }
        public decimal? Day20 { get; set; }
        public decimal? Day21 { get; set; }
        public decimal? Day22 { get; set; }
        public decimal? Day23 { get; set; }
        public decimal? Day24 { get; set; }
        public decimal? Day25 { get; set; }
        public decimal? Day26 { get; set; }
        public decimal? Day27 { get; set; }
        public decimal? Day28 { get; set; }
        public decimal? Day29 { get; set; }
        public decimal? Day30 { get; set; }
        public decimal? Day31 { get; set; }

        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string ReportMode { get; set; }

      
        public List<SelectListItem> SONOs { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> CustOrderNos { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> ScheduleNos { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> PartCodes { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> ItemNames { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> AccountNames { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> WorkCenterNames { get; set; } = new List<SelectListItem>();
          public int TotalRecords { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public IList<OrderBasedProdPlanModel> OrderBasedProdPlanList { get; set; }
    }
}
