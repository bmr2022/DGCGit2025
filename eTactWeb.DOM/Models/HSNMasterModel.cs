using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eTactWeb.DOM.Models
{
    public class HSNMasterModel
    {
        public int HSNEntryID { get; set; }
        public string HSNNo { get; set; }
        public string HSNTypeItemServ { get; set; }
        public string EntryByMachineName { get; set; }
        public int ActualEntryBy { get; set; }
        public string ActualEntryByName { get; set; }

        public string Mode { get; set; }
    }
}
