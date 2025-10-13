using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eTactWeb.DOM
{
    public class NavigationState
    {
        public string Controller { get; set; }
        public string Action { get; set; }

        // Optional route values (for URL-safe parameters like ID, Mode, etc.)
        public Dictionary<string, object> RouteValues { get; set; }

        // Internal filters you don't want in URL (FromDate, ToDate, etc.)
        public Dictionary<string, object> Filters { get; set; }
    }
}
