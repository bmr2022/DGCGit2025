using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.DOM.Models
{
    public class PendingProductionSchedule
    {
        public string? FromDate { get; set; }
        public string? ToDate { get; set; }
        public string? PartCode { get; set; }
        public string? ItemName { get; set; }
        public string? StoreName {  get; set; }
        public string? IssueDate {  get; set; }
        public string? PendingProdPlan {  get; set; }
        public string? PendingProdPlanYearCode {  get; set; }
        public string? ProdScheduleNo {  get; set; }
        public string? ToWorkCenter { get; set; }
        public string? ProdScheduleNo1 {  get; set; }
        public int ProdSchYearCode1 {  get; set; }
        public IList<TextValue>? ItemNameList { get; set; }
        public IList<TextValue>? PartCodeList { get; set; }
    }
}
