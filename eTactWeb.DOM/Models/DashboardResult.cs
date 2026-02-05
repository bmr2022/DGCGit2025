using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eTactWeb.DOM.Models
{
    public class DashboardResult
    {
        public List<DashboardColumn> Headers { get; set; } = new();
        public List<Dictionary<string, object>> Rows { get; set; } = new();
    }
    public class DashboardColumn
    {
        public string Title { get; set; }
        public string Field { get; set; }
    }
}
