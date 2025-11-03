using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eTactWeb.DOM.Models
{
    public class ReportRequest
    {
        public string ReportType { get; set; }
        public ReportFilter Filters { get; set; }
    }
}
