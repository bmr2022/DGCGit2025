using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eTactWeb.DOM.Models
{
    public class MachineGroupMasterModel
    {
        public string Flag { get; set; } 
        public int EntryId { get; set; }
        public int YearCode { get; set; }
        public string EntryDate { get; set; }
        public string MachineGroup { get; set; }
        public int MachineId{ get; set; }
        public int UId { get; set; }
        public string CC { get; set; }
        public string EntryByMachine { get; set; } 
        public int ActualEntryBy { get; set; }
        public string ActualEntryByName { get; set; }
        public string? ActualEntryDate { get; set; }
        public int LastUpdatedBy { get; set; }
        public string LastUpdatedByName { get; set; }
        public string ApprovedByEmpName { get; set; }
        public string? LastUpdationDate { get; set; }
        public int ApprovedByEmpId { get; set; }
        public int SeqNo { get; set; }
        public string Mode { get; set; }

    }
}
