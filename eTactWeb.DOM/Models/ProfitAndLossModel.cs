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
        public string ShowRecordWithZeroAmt {  get; set; }
        public string ShowOpening {  get; set; }
        public IList<ProfitAndLossRow> ProfitAndLossGrid { get; set; } = new List<ProfitAndLossRow>();
    }
    public class ProfitAndLossRow
    {
        public Dictionary<string, object> DynamicColumns { get; set; } = new Dictionary<string, object>();
        public string DrGroup { get; set; }
        public string DrLedger { get; set; }
        public decimal GroupDrAmt { get; set; }
        public decimal DrAmt { get; set; }
        public string CrGroup { get; set; }
        public string CrLedger { get; set; }
        public decimal GroupCrAmt { get; set; }
        public decimal CrAmt { get; set; }
        public string ProfitLossDRCRSide { get; set; }
    }
}
