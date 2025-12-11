using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static eTactWeb.DOM.Models.Common;

namespace eTactWeb.DOM.Models
{
    public class HRLeaveApplicationMasterModel: HRLeaveApplicationDetail
    {

        public int? LeaveAppEntryId { get; set; }
        public string? IPAddress { get; set; }
        public int LeaveAppYearCode { get; set; }
        public string? BranchCC { get; set; }
        public string? LeaveAppEntryDate { get; set; }
        public string? ApplicationNo { get; set; }
        public string? ApplicationDate { get; set; }
        public int? EmpId { get; set; }
        public string  EmpName { get; set; }
        public string EmpCode { get; set; }
        public string? MobileNo { get; set; }
        public string? PhoneNo { get; set; }
        public int? desigEntryId { get; set; }
        public string? Designation { get; set; }
        public int? CategoryId { get; set; }
        public string? EmpCategory { get; set; }
        public int DeptId { get; set; }
        public string? Department { get; set; }
        public int ShiftId { get; set; }
        public string? Shift { get; set; }
        public decimal? TotalYearlyLeave { get; set; }
        public decimal? TotalMonthlyLeave { get; set; }
        public decimal? totalpresentDaysTillYet { get; set; }
        public decimal? totalpresentDaysinCurrentMonth { get; set; }

        public decimal? GrossSalary { get; set; }
        public decimal? BalanceMonthlyLeave { get; set; }
        public decimal? MaxAllowedLeave { get; set; }
        public string? TraineePermanent { get; set; }
        public decimal? BalanceAdvanceAmt { get; set; }
        public string? DepartHeadApprovedBy { get; set; }
        public string? HRApprovedBy { get; set; }
        public string? DepartAppdate { get; set; }
        public string? HRAppDate { get; set; }
        public int? ActualEntryBy { get; set; }
        public int? Updatedby { get; set; }
        public string? LastUPdatedDate { get; set; }
        public string? Canceled { get; set; }
        public string? Approved { get; set; }
        public string? Mode { get; set; }
        public IList<HRLeaveApplicationDetail>? ItemDetailGrid { get; set; }
    }
    public class HRLeaveApplicationDetail : TimeStamp
    {
        public int SeqNo { get; set; }
        public int? LeaveAppEntryId { get; set; }
        public int LeaveAppYearCode { get; set; }
        public int EmpId { get; set; }
        public int? LeaveEntryId { get; set; }
        public string? LeaveName { get; set; }
        public string? EntryByMachineName { get; set; }
        public string? FromDate { get; set; }
        public string? ToDate { get; set; }
        public string? HalfDayFullDay { get; set; }
        public decimal? Duration { get; set; }
        public decimal? BalanceLeaveMonthly { get; set; }
        public decimal? BalanceLeaveYearly { get; set; }
        public decimal? MaxLeaveInMonth { get; set; }
        public string? Canceled { get; set; }
        public string? Approved { get; set; }
    }

    public class HRLeaveApplicationDashBoard : TimeStamp
    {
        public int? LeaveAppEntryId { get; set; }
        public int LeaveAppYearCode { get; set; }
        public string? BranchCC { get; set; }
        public string? LeaveAppEntryDate { get; set; }
        public string? ApplicationNo { get; set; }
        public string? ApplicationDate { get; set; }
        public int? EmpId { get; set; }
        public string? EmpName { get; set; }
        public string? EmpCode { get; set; }
        public string? MobileNo { get; set; }
        public string? PhoneNo { get; set; }
        public int? desigEntryId { get; set; }
        public string? Designation { get; set; }
        public int? CategoryId { get; set; }
        public string? EmpCategory { get; set; }
        public int DeptId { get; set; }
        public string? Department { get; set; }
        public int ShiftId { get; set; }
        public string  Shift { get; set; }
        public decimal? TotalYearlyLeave { get; set; }
        public decimal? TotalMonthlyLeave { get; set; }

        public decimal? GrossSalary { get; set; }
        public decimal? BalanceMonthlyLeave { get; set; }
        public decimal? MaxAllowedLeave { get; set; }
        public string? TraineePermanent { get; set; }
        public decimal? BalanceAdvanceAmt { get; set; }
        public string? DepartHeadApprovedBy { get; set; }
        public string? HRApprovedBy { get; set; }
        public string? DepartAppdate { get; set; }
        public string? HRAppDate { get; set; }
        public int? ActualEntryBy { get; set; }
        public int? Updatedby { get; set; }
        public string? LastUPdatedDate { get; set; }
        public string? ActualEntrybyEmp { get; set; }
        public string? UpdatedbyEmp { get; set; }
        public string? Canceled { get; set; }
        public string? Approved { get; set; }
        public int? SeqNo { get; set; }
        public int? LeaveEntryId { get; set; }
        public string? LeaveName { get; set; }
        public string? FromDate { get; set; }
        public string? ToDate { get; set; }
        public decimal Duration { get; set; }
        public decimal? BalanceLeaveMonthly { get; set; }
        public decimal BalanceLeaveYearly { get; set; }
        public decimal? MaxLeaveInMonth { get; set; }
        public string? DashFromDate { get; set; }
        public string? DashToDate { get; set; }
        public string? ReportType { get; set; }
       public string? Searchbox {  get; set; }

        public List<HRLeaveApplicationDashBoard>? HRLeaveApplicationDashBoardDetail { get; set; }
    }
}
