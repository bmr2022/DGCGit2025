using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.DOM.Models
{
    public class UserRightDashboard
    {
        public int EmpId {  get; set; }
        public int UserId { get; set; }
        public IList<TextValue>? UserList { get; set; }
        public string? Mode {  get; set; }
        public string? UserName { get; set; }
        public IList<TextValue>? DashboardNameList { get; set; }
        public string? DashboardName {  get; set; }
        public string? DashboardSubScreen {  get; set; }
        public bool IsView {  get; set; }
        public int CreatedById {  get; set; }
        public DateTime CreatedOn { get; set; }
        public int UpdatedById {  get; set; }
        public DateTime UpdatedOn { get; set; }
        public IList<UserRightDashboard>? UserRightsDashboard { get; set; }
    }
}
