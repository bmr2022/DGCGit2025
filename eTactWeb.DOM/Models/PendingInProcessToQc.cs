using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.DOM.Models
{
    public class PendingInProcessToQc
    {
        public string FromDate {  get; set; }
        public string ToDate { get; set; }
        public int ItemCode {  get; set; }
        public string PartCode {  get; set; }
        public string ItemName {  get; set; }
        public string ProdSlipNo {  get; set; }
        public string WorkCenter {  get; set; }
        public IList<TextValue>? ItemNameList { get; set; }
        public IList<TextValue>? PartCodeList { get; set; }
        public IList<TextValue>? WorkCenterList { get; set; }
        public IList<TextValue>? ProdSlipList { get; set; }
    }
    public class DisplayPendForQCGrid
    {
        public int? ProdEntryId { get; set; }
        public int Prodyearcode { get; set; }
        public string? ProdSlipNo { get; set; }
        public int FGItemCode {  get; set; }
    }
}
