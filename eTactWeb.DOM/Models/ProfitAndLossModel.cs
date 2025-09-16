using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eTactWeb.DOM.Models
{
    public class ProfitAndLossModel
    {
        public string FromDate {  get; set; }
        public string ToDate { get; set; }
        public string EntryByMachine {  get; set; }
        public string ReportType {  get; set; }
        public decimal TotalDRbeforeGrossProfit {  get; set; }
        public decimal TotalCRbeforeGrossProfit { get; set; }
        public IList<ProfitAndLossModel> ProfitAndLossGrid { get; set; }
    }
}
