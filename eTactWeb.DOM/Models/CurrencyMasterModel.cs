using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.DOM.Models
{
    public class CurrencyMasterModel: TimeStamp
    {
        public int CurrencyId { get; set; }
        public string Currency { get; set; }
        public string IsDefault { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
       public string EntryDate { get; set; }
        public string Searchbox { get; set; }
        public IList<CurrencyMasterModel> CurrencyMasterDashBoardGrid { get; set; }
    }
}
