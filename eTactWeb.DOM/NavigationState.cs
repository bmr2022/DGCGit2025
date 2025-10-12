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
        public object RouteValues { get; set; } = new();
    }
}
