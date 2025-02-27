using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eTactWeb.DOM.Models
{
    public class UserRightReportModel
    {
        public string UserName { get; set; }
        public int Uid { get; set; }
        public string EmpCode { get; set; }
        public string EmployeeName { get; set; }
        public int EmpID { get; set; }
        public int UID { get; set; }
        public string FormName { get; set; }
        public string ModuleName { get; set; }
        public string MachineName { get; set; }
        public string Active { get; set; }
        public string AllRights { get; set; }
        public string Save { get; set; }
        public string Update { get; set; }
        public string Delete { get; set; }
        public string View { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedByEmp { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public string UpdatedByEmp { get; set; }
        public string ActionTaken { get; set; }
        public DateTime ActionDate { get; set; }
        public string ActionTime { get; set; }
        public string SlipNo { get; set; }
        public int EntryIdOfTrans { get; set; }
        public string ActionTakenBy { get; set; }
        public string CustVendName { get; set; }
        public string CustomerVendorName { get; set; }
        public string OtherDetail { get; set; }
        public string Remarks { get; set; }
        public int YearCode { get; set; }
        public IList<UserRightReportModel> UserRightReportGrid { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string ReportType { get; set; }
    }
}
