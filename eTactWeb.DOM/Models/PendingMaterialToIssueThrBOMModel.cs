using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.DOM.Models
{
    public class PendingMaterialToIssueThrBOMModel
    {
        public string? FromDate { get; set; }
        public string? ToDate { get; set; }
        public string? ReqNo { get; set; }
        public int ReqYearCode { get; set; }
        public int YearCode { get; set; }
        public int ItemCode { get; set; }
        public string? ItemName { get; set; }
        public string? PartCode { get; set; }
        public string? WONO { get; set; }
        public string? WorkCenter { get; set; }
        public string? DeptName { get; set; }
        public string? CC { get; set; }
        public string? IssueDate { get; set; }
        public string? GlobalSearch { get; set; }
        public string? FromStore { get; set; }
        public IList<TextValue>? ReqList { get; set; }
        public IList<TextValue>? ItemList { get; set; }
        public IList<TextValue>? PartCodeList { get; set; }
        public IList<TextValue>? DeptList { get; set; }
        public IList<TextValue>? WorkCenterList { get; set; }
        public IList<TextValue>? BranchList { get; set; }

    }
}
