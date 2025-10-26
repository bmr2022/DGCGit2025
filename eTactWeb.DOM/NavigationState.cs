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
        public Dictionary<string, object> RouteValues { get; set; } = new();
        public Dictionary<string, object> Filters { get; set; } = new();
        public string ReportType { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is not NavigationState other) return false;

            bool routeEqual = RouteValues.Count == other.RouteValues.Count &&
                              !RouteValues.Except(other.RouteValues).Any();

            bool filterEqual = Filters.Count == other.Filters.Count &&
                               !Filters.Except(other.Filters).Any();

            return Controller == other.Controller &&
                   Action == other.Action &&
                   routeEqual &&
                   filterEqual &&
                   ReportType == other.ReportType;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Controller, Action, RouteValues, Filters, ReportType);
        }
    }
}
