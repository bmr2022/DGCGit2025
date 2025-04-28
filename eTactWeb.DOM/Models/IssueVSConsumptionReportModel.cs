using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eTactWeb.DOM.Models
{
    public class IssueVSConsumptionReportModel
    {
        public string? PartCode { get; set; }
        public string? ItemName { get; set; }
        public float? ReqQty { get; set; }
        public float? IssuQty { get; set; }
        public float? ConsQty { get; set; }
        public string? RMUnit { get; set; }
        public IList<IssueVSConsumptionReportModel>? IssueVSConsumptionReportGrid { get; set; }

    }
}
