using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.DOM.Models.Master
{
    public class MachineMasterModel : TimeStamp
    {
        public string Flag { get; set; }
        public int EntryId { get; set; } 
        public int MachGroupId { get; set; }
        public int MachineId { get; set; }
        public string MachineCode { get; set; } 
        public string MachineGroup { get; set; } 
        public string MachineName { get; set; } 
        public string Searchbox { get; set; } 
        public double LabourCost { get; set; }
        public string NeedHelper { get; set; } 
        public long TotalHelper { get; set; }
        public double HelperCost { get; set; }
        public double ElectricityCost { get; set; }
        public double OtherCost { get; set; }
        public double TotalCost { get; set; }
        public string? EntryDate { get; set; }
        public string Make { get; set; } 
        public string Location { get; set; }
        public string TechSpecification { get; set; } 
        public string? LastCalibraDate { get; set; }
        public double CalibraDur { get; set; }
        public string UId { get; set; }
        public string CC { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public IList<MachineMasterModel>? MachineMasterGrid{ get; set; }

    }
}
