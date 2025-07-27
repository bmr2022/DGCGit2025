using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eTactWeb.DOM.Models
{
    public class UserRightDashboard
    {
        public int EmpId {  get; set; }
        public int UserId { get; set; }
        public string? DashboardName {  get; set; }
        public string? DashboardSubScreen {  get; set; }
        public int CreatedById {  get; set; }
        public DateTime CreatedOn { get; set; }
        public int UpdatedById {  get; set; }
        public DateTime UpdatedOn { get; set; }
    }
}
