using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eTactWeb.DOM.Models
{
    public class HSNMasterModel
    {
        public int HSNEntryID { get; set; }

        [Required(ErrorMessage = "HSN No is required")]
        [RegularExpression(@"^\d{8}$", ErrorMessage = "HSN No must be exactly 8 digits (numbers only)")]
        public string HSNNo { get; set; }
        [Required(ErrorMessage = "HSN Type is required")]
        public string HSNTypeItemServ { get; set; }
        public string EntryByMachineName { get; set; }
        public int ActualEntryBy { get; set; }
        public string ActualEntryByName { get; set; }

        public string Mode { get; set; }
    }
}
