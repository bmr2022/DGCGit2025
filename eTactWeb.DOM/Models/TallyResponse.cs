using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eTactWeb.DOM.Models
{
    public class TallyResponse
    {
        public bool Success { get; set; }
        public int Created { get; set; }
        public int Altered { get; set; }
        public int Deleted { get; set; }
        public int Errors { get; set; }
        public string ResponseXml { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }
}
