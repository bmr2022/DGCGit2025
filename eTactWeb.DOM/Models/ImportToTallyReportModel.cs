using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eTactWeb.DOM.Models
{
    public class ImportToTallyReportModel
    {
        public SaleRejectionFilter Filters { get; set; }
        public List<DashboardColumn> Headers { get; set; }
        public List<Dictionary<string, object>> Rows { get; set; }

        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string ReportType { get; set; }
        public int? AccountCode { get; set; }
        //public IList<REQUISITIONRegisterDetail>? REQUISITIONRegisterDetails { get; set; }
    }
}
