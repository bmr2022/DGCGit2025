using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.DOM.Models.Master
{
   public  class WorkCenterMasterModel : TimeStamp
    {
        public int WCID { get; set; }
        public string? WorkCenterCode { get; set; }
        public string? WorkCenterDescription { get; set; }
        public string? WorkCenterType { get; set; }
        public string? Searchbox { get; set; }
        public IList<WorkCenterMasterModel>? WorkCenterMasterList { get; set; }
    }
}
