using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.DOM.Models
{
    public class HRLeaveMasterModel: TimeStamp
    {
        [Key]
        public long LeaveId { get; set; } // Primary Key	

        [Required]
        [StringLength(50)]
        public string LeaveCode { get; set; } // CL, SL, EL, etc.	

        [Required]
        [StringLength(100)]
        public string LeaveName { get; set; } // Casual Leave, Sick Leave	

        [Required]
        [StringLength(50)]
        public string? LeaveType { get; set; } // Paid, Unpaid, Compensatory	

        [Required]
        [StringLength(50)]
        public string? LeaveCategory { get; set; } // Earned, Maternity, Study, Special	

        [Required]
        [StringLength(10)]
        public string GenderApplicable { get; set; } // Male, Female, All	

        public int MaxLeavePerYear { get; set; } // Max allowed in a year	

        public int? MinDaysForApplication { get; set; } // Minimum notice period	

        public int? MaxConsecutiveDaysAllowed { get; set; } // Max continuous leave	

        [StringLength(3)]
        public string Encashable { get; set; } // Yes/No	

        [StringLength(3)]
        public string CarryForward { get; set; } // Yes/No	

        public int? MaxCarryForwardLimit { get; set; } // Limit on carry-forward	

        [StringLength(3)]
        public string HalfDayAllowed { get; set; } // Yes/No	

        [StringLength(3)]
        public string LeaveApprovalRequired { get; set; } // Yes/No	

        [StringLength(3)]
        public string CompensatoryOffRequired { get; set; } // Yes/No	

        [StringLength(3)]
        public string LeaveDeductionApplicable { get; set; } // Yes/No	

        public int? EligibilityAfterMonths { get; set; } // New employees eligible after X months	

        [StringLength(100)]
        public string RestrictedToEmployeeCategory { get; set; } // Example: Managerial, Staff	
        public IList<LeaveEmpCategDetail>? EmpCategDetailList { get; set; }      
        public IList<TextValue>? EmpCategList { get; set; }

        [StringLength(100)]
        public string RestrictedToDepartment { get; set; } // Example: IT, HR, Finance	
        public IList<LeaveDeptWiseCategDetail>? DeptWiseCategDetailList { get; set; }
        public IList<TextValue>? DeptWiseCategList { get; set; }

        [StringLength(100)]
        public string RestrictedToLocation { get; set; } // Example: India, GCC	
        public IList<LeaveLocationDetail>? LocationDetailList { get; set; }
        public IList<TextValue>? LocationList { get; set; }

        public int? MinWorkDaysRequired { get; set; } // Required workdays before applying	

        public int? AutoApproveLimitDays { get; set; } // Auto-approved up to X days	

        public long? ApprovalLevel1 { get; set; } // First approver (Manager ID)	

        public long? ApprovalLevel2 { get; set; } // Second approver (HR ID)	

        public long? ApprovalLevel3 { get; set; } // Third approver (if any)	

        [StringLength(3)]
        public string Active { get; set; } // Yes/No	

        public DateTime? EffectiveFrom { get; set; } // Policy start date	

        public long? CreatedBy { get; set; } // Created by (User ID)	

        public long? UpdatedBy { get; set; } // Last modified by	
        public DateTime? CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public string? CC { get; set; }

    }
}

[Serializable()]
public class LeaveEmpCategDetail
{
    //public int CategoryId { get; set; }
    public int SalHeadEntryId { get; set; }
    public string? CategoryId { get; set; }


}

[Serializable()]
public class LeaveDeptWiseCategDetail
{
    // public int DeptId { get; set; }
    public int SalHeadEntryId { get; set; }
    public string? DeptId { get; set; }

}

[Serializable()]
public class LeaveLocationDetail
{
    // public int DeptId { get; set; }
    public int SalHeadEntryId { get; set; }
    public string? DeptId { get; set; }

}

