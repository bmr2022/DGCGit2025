using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.DOM.Models
{
    public class StoreMasterModel:TimeStamp
    {
        public int StoreId { get; set; } 
        public string CC { get; set; } 
        public string Store_Name { get; set; } 
        public string StoreType { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string EntryDate { get; set; }
        public string Searchbox { get; set; }
        public IList<StoreMasterModel> StoreMasterDashBoardGrid { get; set; }
    }
}
