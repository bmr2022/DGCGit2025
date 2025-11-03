using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eTactWeb.DOM.Models
{
    public class BalanceSheetModel
    {
        public string FromDate {  get; set; }
        public string ToDate { get; set; }
        public string EntryByMachine {  get; set; }
        public string ReportType {  get; set; }
        public string LiabilitiesAndAssets {  get; set; }
        public string ParentAccountName {  get; set; }
        public string AccountName {  get; set; }
        public IList<BalanceSheetRow> BalanceSheetGrid { get; set; } = new List<BalanceSheetRow>();
    }
    public class BalanceSheetRow
    {
        public Dictionary<string, object> DynamicColumns { get; set; } = new Dictionary<string, object>();
    }
}
