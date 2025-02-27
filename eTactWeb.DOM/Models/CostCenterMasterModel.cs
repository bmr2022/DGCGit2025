using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.DOM.Models
{
    public  class CostCenterMasterModel: TimeStamp
    {
        public string Flag { get; set; } 
        public int CostcenterId { get; set; }
        public string CostCenterName { get; set; } 
        public string CC { get; set; }
        public string EntryDate { get; set; } 
        public string ShortName { get; set; } 
        public int DepartmentID { get; set; }
        public int UnderGroupId { get; set; } 
        public string UnderGroupName{ get; set; } 
        public string Remarks { get; set; } 
        public string DepartmentName { get; set; } 
        public string CostCenterCode { get; set; } 
        public int CostcentergroupID { get; set; } 
        public string CostcenetrGroupName { get; set; } 
        public string FromDate { get; set; } 
        public string ToDate { get; set; } 
        public string Searchbox { get; set; } 
        public string DRCR { get; set; } 
        public IList<CostCenterMasterModel> CostCenterMasterGrid { get; set; }
    }
}
