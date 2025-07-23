using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eTactWeb.DOM.Models
{
    public class VendoreRatingAnalysisReportModel
    {
        public string PONO { get; set; }
        public string CurrentDate { get; set; }
        public int YearCode { get; set; }
        public decimal DeliveryRating { get; set; }
        public string ReportType { get; set; }
        public string RatingType { get; set; }
        public long POYearCode { get; set; }
        public DateTime? POEffDate { get; set; }
        public DateTime? PODate { get; set; }
        public string SchNo { get; set; }
        public DateTime? SchDate { get; set; }
        public long SchYearCode { get; set; }
        public string CustomerName { get; set; }
        public long AccountCode { get; set; }
        public long ItemCode { get; set; }
        public string ItemName { get; set; }
        public string PartCode { get; set; }
        public string Unit { get; set; }
        public decimal POQty { get; set; }
        public decimal BillQty { get; set; }
        public decimal RecQty { get; set; }
        public decimal RejectedQty { get; set; }
        public decimal ReWorkQty { get; set; }
        public decimal QualityRating { get; set; }
        public decimal QualityPercent { get; set; }
        public decimal GateQty { get; set; }
        public decimal PORate { get; set; }
        public decimal POItemAmt { get; set; }
        public decimal PONetAmt { get; set; }
        public decimal POBasicAmt { get; set; }
        public string ItemCategory { get; set; }
        public long ItemCategId { get; set; }
        public string ItemGroupName { get; set; }
        public long GroupId { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public decimal VendorRate { get; set; }
        public decimal MinPORate { get; set; }
        public decimal MaxPORate { get; set; }
        public decimal PriceRating { get; set; }
        public string Mode { get; set; }
        public int TotalRecords { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public IList<VendoreRatingAnalysisReportModel> VendoreRatingAnalysisReportGrid { get; set; }
    }
}
