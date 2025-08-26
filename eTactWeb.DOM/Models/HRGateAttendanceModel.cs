using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.DOM.Models;

public class HRGateAttendanceModel : TimeStamp
{
    private IList<SelectListItem> _AttendanceTypeList = new List<SelectListItem>()
    {
        new() { Value = "Manual", Text = "Manual" },
        new() { Value = "Import", Text = "Import" },
    };
    private IList<SelectListItem> _DayOrMonthTypeList = new List<SelectListItem>()
    {
        new() { Value = "Daily", Text = "Daily" },
        new() { Value = "Monthly", Text = "Monthly" },
    };
    private IList<SelectListItem> _AttendanceEntryMethodTypeList = new List<SelectListItem>()
    {
        new() { Value = "Manual", Text = "Manual" },
        new() { Value = "Biometric", Text = "Biometric" },
        new() { Value = "RFID", Text = "RFID" },
        new() { Value = "Import", Text = "Import" },
    };
    public int GateAttEntryId { get; set; }
    public string GateAttEntryDate { get; set; }
    public int GateAttYearCode { get; set; }
    public string CardOrBiometricId { get; set; }
    public string EmpId { get; set; }
    public DateTime? AttInTime { get; set; }
    public DateTime? AttOutTime { get; set; }
    [Column(TypeName = "decimal(5, 2)")]
    public decimal? TotalNoOfHours { get; set; }
    public string LateEntry { get; set; }
    public string EarlyExit { get; set; }
    public int? LeaveTypeId { get; set; }
    public int? AttShiftId { get; set; }
    public DateTime? GateOutDate { get; set; }
    public DateTime? GateOutTime { get; set; }
    public bool ApproveByDept { get; set; }
    public DateTime? DeptApprovaldate { get; set; }
    public int? DeptApprovalEmpId { get; set; }
    public string ApproveByHR { get; set; }
    public DateTime? HRApprovaldate { get; set; }
    public int? HRApprovalEmpId { get; set; }
    public string CC { get; set; }
    public string CategoryCode { get; set; }
    public DateTime? EmpAttDate { get; set; }
    public int? EmpAttYear { get; set; }
    public DateTime? EmpAttTime { get; set; }
    public int? ActualEmpShift { get; set; }
    public DateTime? ActualShiftInTime { get; set; }
    public DateTime? ActualShiftoutTime { get; set; }
    [Column(TypeName = "decimal(5, 2)")]
    public decimal? OvertTime { get; set; }
    public int? DeptId { get; set; }
    public int? desigEntryId { get; set; }
    public string? AttendanceEntryMethodType { get; set; }
    public IList<SelectListItem> AttendanceEntryMethodTypeList
    {
        get => _AttendanceEntryMethodTypeList;
        set => _AttendanceEntryMethodTypeList = value;
    }
    public string? DayOrMonthType { get; set; }
    public IList<SelectListItem> DayOrMonthTypeList
    {
        get => _DayOrMonthTypeList;
        set => _DayOrMonthTypeList = value;
    }
    public string? AttendanceType { get; set; }
    public IList<SelectListItem> AttendanceTypeList
    {
        get => _AttendanceTypeList;
        set => _AttendanceTypeList = value;
    }
    public string EmployeeCode { get; set; }
    public string EmployeeName { get; set; }
    public IList<TextValue>? DeptList { get; set; } // for future filter
    public IList<TextValue>? DesignationList { get; set; } // for future filter
    public string TotalEmployees { get; set; }
    public string TotalEmployeesOnLeave { get; set; }
    public string TotalEmployeesAttDone { get; set; }
    //Needed for basic details
    public string EntryByMachineName { get; set; }
    public string TypeOfSave { get; set; }
    public string FinFromDate { get; set; }
    public string FinToDate { get; set; }
    public string CreatedByName { get; set; }
    public string UpdatedByName { get; set; }
    public string Branch { get; set; }
}

