using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eTactWeb.DOM.Models
{
    public class ApiResponse
    {
        public object Result { get; set; }
        public int StatusCode { get; set; }
        public string StatusText { get; set; }
    }
}
