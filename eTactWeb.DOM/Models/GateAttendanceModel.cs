using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.DOM.Models;

public class GateAttendanceModel : TimeStamp
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
    private IList<SelectListItem> _strEmpAttMonthList = new List<SelectListItem>()
    {
        new() { Value = "1", Text = "January" },
        new() { Value = "2", Text = "February" },
        new() { Value = "3", Text = "March" },
        new() { Value = "4", Text = "April" },
        new() { Value = "5", Text = "May" },
        new() { Value = "6", Text = "June" },
        new() { Value = "7", Text = "July" },
        new() { Value = "8", Text = "August" },
        new() { Value = "9", Text = "September" },
        new() { Value = "10", Text = "October" },
        new() { Value = "11", Text = "November" },
        new() { Value = "12", Text = "December" },
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
    public string GateAttEntryDay { get; set; }
    public int GateAttYearCode { get; set; }
    public string CardOrBiometricId { get; set; }
    public int EmpId { get; set; }
    public DateTime? AttInTime { get; set; }
    public DateTime? AttOutTime { get; set; }
    [Column(TypeName = "decimal(5, 2)")]
    public decimal? TotalNoOfHours { get; set; }
    public string LateEntry { get; set; }
    public string EarlyExit { get; set; }
    public int? LeaveTypeId { get; set; }
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
    public int? ActualEmpShiftId { get; set; }
    public string ActualEmpShiftName { get; set; }
    public DateTime? ActualShiftInTime { get; set; }
    public DateTime? ActualShiftoutTime { get; set; }
    [Column(TypeName = "decimal(5, 2)")]
    public decimal? OvertTime { get; set; }
    public int? DeptId { get; set; }
    public string DeptName { get; set; }
    public int? DesignationEntryId { get; set; }
    public string DesignationName { get; set; }
    public string strEmpAttDate { get; set; }
    public string EmpCategoryId { get; set; }
    public string EmpCategory { get; set; }
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
    public string strEmpAttMonth { get; set; }
    public IList<SelectListItem> strEmpAttMonthList
    {
        get => _strEmpAttMonthList;
        set => _strEmpAttMonthList = value;
    }
    public string? AttendanceType { get; set; }
    public IList<SelectListItem> AttendanceTypeList
    {
        get => _AttendanceTypeList;
        set => _AttendanceTypeList = value;
    }
    public bool PN1 { get; set; } //for show all employee at employee filter
    public string EmployeeCode { get; set; }
    public string EmployeeName { get; set; }
    public IList<TextValue>? EmployeeList { get; set; }
    public IList<TextValue>? DeptList { get; set; }
    public IList<TextValue>? ShiftList { get; set; }
    public IList<TextValue>? DesignationList { get; set; }
    public IList<TextValue>? CategoryList { get; set; }
    public string TotalEmployees { get; set; }
    public string TotalEmployeesOnLeave { get; set; }
    public string TotalEmployeesAttDone { get; set; }
    //Needed for basic details
    public string EntryByMachineName { get; set; }
    public string TypeOfSave { get; set; }
    public string FinFromDate { get; set; }
    public string FinToDate { get; set; }
    public string NFromDate { get; set; }
    public string NToDate { get; set; }
    public string CreatedByName { get; set; }
    public string UpdatedByName { get; set; }
    public string Branch { get; set; }
    // Dynamic support
    public List<string> DayHeaders { get; set; } = new(); // dynamic column headers
    public Dictionary<string, string> Attendance { get; set; } = new(); // dynamic day-wise In/Out
    public List<GateAttendanceModel> GateAttDetailsList { get; set; }
}

