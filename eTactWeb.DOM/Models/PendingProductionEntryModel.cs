using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eTactWeb.DOM.Models
{
    public class PendingProductionEntryModel
    {
        public string? PlanNo { get; set; }
        public string? PlanNoDate { get; set; }
        public string? ProdSchNo { get; set; }
        public string? ProdSchdate { get; set; }
        public string? PartCode { get; set; }
        public string? Item_Name { get; set; }
        public int Item_Code { get; set; }
        public string? Qty { get; set; }
        public string? PlanNoEntryId { get; set; }
        public int PlanNoYearCode { get; set; }
        public string? ProdSchEntryid { get; set; }
        public int ProdSchYearCode { get; set; }
        public string? WorkCenter { get; set; }
        public int WcId {  get; set; }
        public IList<PendingProductionEntryModel> PendingProductionEntryGrid { get; set; }

    }
}
