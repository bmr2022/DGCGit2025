using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.DOM.Models
{
    public class VendorUserModel : TimeStamp
    {
        public int UserEntryId { get; set; }
        public int AccountCode { get; set; }
        public string? AccountName { get; set; }
        public string? SaleBillPrefix { get; set; }
        public string? VendorEmpName { get; set; }
        public string UserId { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string ConfirmPassword { get; set; } = null!;
        public string Active { get; set; } = null!;
        public string AllowToDelete { get; set; } = null!;
        public string AllowToUpdate { get; set; } = null!;
        public string RightsForReport { get; set; } = null!;
        public string RightsForPurchaseModule { get; set; } = null!;
        public string RightsForQCModule { get; set; } = null!;
        public string RightsForAccountModule { get; set; } = null!;
        public string AdminUser { get; set; } = null!;
        public string OurServerName { get; set; } = null!;
        public string DatabaseName { get; set; } = null!;
        public string BranchName { get; set; } = null!;
        public long ActualEntryBy { get; set; }
        public string ActualEntryByName { get; set; } = null!;
        public string ActualEntryDate { get; set; }
        public long LastUpdatedBy { get; set; }
        public string UpdatedByName { get; set; } = null!;
        public string LastUpdationDate { get; set; }
        public string EntryByMachineName { get; set; } = null!;
        public string FinFromDate { get; set; }
        public string FinToDate { get; set; }
    }

    public class VendorUserDashboard : VendorUserModel
    {
       public List<VendorUserDashboard>? VendorUserDashboards { get; set; }    
    }
}
