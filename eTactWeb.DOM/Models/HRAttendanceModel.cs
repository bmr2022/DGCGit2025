using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.DOM.Models;

/// <inheritdoc/>

[Serializable()]
public class HRAttendanceModel
{
}

public class HRAListDataModel
{
    public int? HRAListDataSeqNo { get; set; }
    public string? FromDate { get; set; }
    public string? ToDate { get; set; }
    public int? HRAttYearCode { get; set; }
    public string? EmpCateg { get; set; }
    public IList<TextValue>? EmpCategList { get; set; }
    //filter
    public IList<TextValue>? DashDepartmentList { get; set; }
    public string? DashDepartment { get; set; }
    public IList<TextValue>? DashDesignationList { get; set; }
    public string? DashDesignation { get; set; }
    public IList<TextValue>? DashEmployeeList { get; set; }
    public string? DashEmployee { get; set; }
    public IList<TextValue>? DashCategoryList { get; set; }
    public string? DashCategory { get; set; }
    public IList<TextValue>? DashAttendanceDateList { get; set; }
    public string? DashAttendanceDate { get; set; }
    public string? Searchbox { get; set; }
    //pagintion
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalRecords { get; set; }
    public IList<HRAListDataModel> HRAListData { get; set; }
    //grid details
    public DateTime? LeaveDate { get; set; }
    public int? LeaveEntryId { get; set; }
    public string HalfDayFullDay { get; set; }
    public string EmpName { get; set; }
    public string EmpCode { get; set; }
    public int? EmpId { get; set; }
    public DateTime? AttandanceDate { get; set; }
    public int? GateAttEntryId { get; set; }
    public int? GateAttYearCode { get; set; }
    public string CardOrBiometricId { get; set; }
    public string HRAttStatus { get; set; }
    public string AttendStatus { get; set; }
    public DateTime? AttInTime { get; set; }
    public DateTime? AttOutTime { get; set; }
    [Column(TypeName = "decimal(5, 2)")]
    public decimal? TotalNoOfHours { get; set; }
    [Column(TypeName = "decimal(5, 2)")]
    public decimal? TotalNoOfShiftHours { get; set; }
    public string LateEntry { get; set; }
    public string EarlyExit { get; set; }
    public int? LeaveTypeId { get; set; }
    public string ShiftName { get; set; }
    public int? AttShiftId { get; set; }
    public int? CategoryCode { get; set; }
    public string CC { get; set; }
    public DateTime? EmpAttDate { get; set; }
    public int? EmpAttYear { get; set; }
    public DateTime? EmpAttTime { get; set; }
    public string Designation { get; set; }
    public string DeptName { get; set; }
    public int? DeptId { get; set; }
    public int? DesigId { get; set; }
}