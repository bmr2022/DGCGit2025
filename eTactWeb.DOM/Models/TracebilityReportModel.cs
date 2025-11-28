using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eTactWeb.DOM.Models
{
    public class TracebilityReportModel
    {
        public DataTable? HeaderTable { get; set; }
        public DataTable? DetailTable { get; set; }
        public string FromDate {  get; set; }
        public string ToDate { get; set; }
        public string SaleBillNo {  get; set; }
        public string BatchNo {  get; set; }
        public string UniqueBatchNo {  get; set; }
    }
}
